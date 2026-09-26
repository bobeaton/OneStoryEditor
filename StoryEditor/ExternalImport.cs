using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    // Format-neutral result of reading a transcribed text from another program
    //  (SayMore, ELAN, FLEx/FlexText Editor). Every tier has exactly one entry per
    //  segment of the baseline (i.e. per story line), so they can be mapped onto
    //  the OSE fields of a line independently of where they came from.
    public class ImportedText
    {
        public string Title;
        public string Speaker;
        public string MediaFile;
        public string SourceFile;
        public List<ImportedTier> Tiers = new List<ImportedTier>();

        public ImportedTier Baseline
        {
            get { return Tiers.FirstOrDefault(t => t.Kind == ImportedTier.TierKind.Baseline); }
        }

        public int LineCount
        {
            get { return (Tiers.Count > 0) ? Tiers.Max(t => t.Lines.Count) : 0; }
        }

        public bool HasData
        {
            get { return Tiers.Any(t => t.Lines.Any(l => !String.IsNullOrEmpty(l))); }
        }

        public override string ToString()
        {
            return Title ?? Path.GetFileNameWithoutExtension(SourceFile);
        }
    }

    public class ImportedTier
    {
        public enum TierKind
        {
            Baseline,           // the transcription (one segment per story line)
            PhraseTranslation,  // one value per segment (e.g. SayMore 'Translation', FLEx phrase 'gls')
            WordGloss           // several values per segment, joined with spaces (e.g. FLEx word 'gls')
        }

        public string Name;
        public string LangCode;
        public TierKind Kind;
        public List<string> Lines = new List<string>();

        // for a word gloss tier: the (distinct) pairs of word and its gloss (e.g. for adding
        //  to an Adapt It knowledge base) and the language of the words
        public string WordLangCode;
        public List<KeyValuePair<string, string>> WordPairs = new List<KeyValuePair<string, string>>();

        public string FirstNonEmptyLine
        {
            get { return Lines.FirstOrDefault(l => !String.IsNullOrEmpty(l)); }
        }

        // a copy of this tier, but with different lines (e.g. after lining them up)
        public ImportedTier WithLines(List<string> lines)
        {
            return new ImportedTier
            {
                Name = Name,
                LangCode = LangCode,
                Kind = Kind,
                Lines = lines,
                WordLangCode = WordLangCode,
                WordPairs = WordPairs
            };
        }

        public void AddWordPair(string strWord, string strGloss)
        {
            if (String.IsNullOrEmpty(strWord) || String.IsNullOrEmpty(strGloss))
                return;
            var pair = new KeyValuePair<string, string>(strWord, strGloss);
            if (!WordPairs.Contains(pair))
                WordPairs.Add(pair);
        }
    }

    // one tier of the imported text to go into one field of the OSE line
    public class ImportMapping
    {
        public StoryEditor.TextFields Field;
        public ImportedTier Tier;

        // leaves out the lines that don't have any text in any of the mapped tiers (e.g. an
        //  audio segment that was never transcribed)
        public static List<ImportMapping> WithoutEmptyLines(List<ImportMapping> mappings)
        {
            var nLines = mappings.Max(m => m.Tier.Lines.Count);
            var lines = Enumerable.Range(0, nLines)
                                  .Where(n => mappings.Any(m => (n < m.Tier.Lines.Count) &&
                                                                !String.IsNullOrEmpty(m.Tier.Lines[n])))
                                  .ToList();
            return mappings.Select(m => new ImportMapping
            {
                Field = m.Field,
                Tier = m.Tier.WithLines(lines.Select(n => (n < m.Tier.Lines.Count) ? m.Tier.Lines[n] : null).ToList())
            }).ToList();
        }
    }

    // Reads any ELAN .eaf file: both the 2-tier files SayMore writes (Transcription +
    //  Translation) and interlinear ones (e.g. the FLEx-style phrase/word/wordGloss/
    //  phraseGloss hierarchy). Dependent annotations are attached to their baseline
    //  segment by following ANNOTATION_REF (or, for time-aligned tiers, by time), rather
    //  than by position, so tiers with a different number of annotations still line up.
    public static class EafReader
    {
        private const string CstrTypeSayMoreTranscription = "Transcription";
        private const string CstrTypeFlexPhrase = "phrase";
        private const string CstrTypeFlexTitle = "interlinear-text";
        private const string CstrTypeGloss = "Gloss"; // cf. Seth's MTT tool

        private class Annotation
        {
            public string Id;
            public string RefId;     // for REF_ANNOTATIONs
            public long? Start, End; // for ALIGNABLE_ANNOTATIONs
            public string Value;
            public Tier Tier;
        }

        private class Tier
        {
            public string Id;
            public string TypeRef;
            public string ParentId;
            public string LangCode;
            public Tier Parent;
            public List<Annotation> Annotations = new List<Annotation>();

            public bool IsAncestorOf(Tier tier)
            {
                for (var t = tier.Parent; t != null; t = t.Parent)
                    if (t == this)
                        return true;
                return false;
            }

            public int Depth
            {
                get
                {
                    var n = 0;
                    for (var t = Parent; (t != null) && (n < 100); t = t.Parent)
                        n++;
                    return n;
                }
            }
        }

        public static ImportedText Read(string strEafFile)
        {
            var doc = XDocument.Load(strEafFile);
            if (doc.Root == null)
                return null;

            var timeSlots = new Dictionary<string, long>();
            foreach (var ts in doc.Root.Descendants("TIME_SLOT"))
            {
                long value;
                var id = (string)ts.Attribute("TIME_SLOT_ID");
                if ((id != null) && Int64.TryParse((string)ts.Attribute("TIME_VALUE"), out value))
                    timeSlots[id] = value;
            }

            var tiers = new List<Tier>();
            var annotations = new Dictionary<string, Annotation>();
            foreach (var elemTier in doc.Root.Elements("TIER"))
            {
                var tier = new Tier
                {
                    Id = (string)elemTier.Attribute("TIER_ID"),
                    TypeRef = (string)elemTier.Attribute("LINGUISTIC_TYPE_REF"),
                    ParentId = (string)elemTier.Attribute("PARENT_REF")
                };
                tier.LangCode = (string)elemTier.Attribute("LANG_REF") ??
                                (string)elemTier.Attribute("DEFAULT_LOCALE") ??
                                LangCodeFromFlexTierId(tier.Id);

                foreach (var elemAnn in elemTier.Elements("ANNOTATION").Elements())
                {
                    var ann = new Annotation
                    {
                        Id = (string)elemAnn.Attribute("ANNOTATION_ID"),
                        RefId = (string)elemAnn.Attribute("ANNOTATION_REF"),
                        Start = TimeValue(timeSlots, (string)elemAnn.Attribute("TIME_SLOT_REF1")),
                        End = TimeValue(timeSlots, (string)elemAnn.Attribute("TIME_SLOT_REF2")),
                        Value = ((string)elemAnn.Element("ANNOTATION_VALUE") ?? String.Empty).Trim(),
                        Tier = tier
                    };
                    tier.Annotations.Add(ann);
                    if (ann.Id != null)
                        annotations[ann.Id] = ann;
                }
                tiers.Add(tier);
            }

            foreach (var tier in tiers.Where(t => t.ParentId != null))
                tier.Parent = tiers.FirstOrDefault(t => t.Id == tier.ParentId);

            var importedText = new ImportedText
            {
                SourceFile = strEafFile,
                MediaFile = GetMediaFile(doc, strEafFile)
            };

            var titleTier = tiers.FirstOrDefault(t => t.TypeRef == CstrTypeFlexTitle);
            if (titleTier != null)
                importedText.Title = titleTier.Annotations.Select(a => a.Value)
                                                          .FirstOrDefault(v => !String.IsNullOrEmpty(v));

            var baselineTier = ChooseBaselineTier(tiers);
            if (baselineTier == null)
                return importedText;

            // the baseline segments are the story lines
            var segments = baselineTier.Annotations
                .Select((a, i) => new {a, i})
                .OrderBy(x => x.a.Start ?? Int64.MinValue).ThenBy(x => x.i)
                .Select(x => x.a).ToList();
            var segmentIndex = new Dictionary<Annotation, int>();
            for (var i = 0; i < segments.Count; i++)
                segmentIndex[segments[i]] = i;

            importedText.Tiers.Add(new ImportedTier
            {
                Name = TierDisplayName(baselineTier),
                LangCode = baselineTier.LangCode,
                Kind = ImportedTier.TierKind.Baseline,
                Lines = segments.Select(a => a.Value).ToList()
            });

            foreach (var tier in tiers)
            {
                // the title and paragraph tiers above the baseline don't have per-line data
                if ((tier == baselineTier) || tier.IsAncestorOf(baselineTier))
                    continue;

                var values = new List<string>[segments.Count];
                var bMultiplePerSegment = false;
                var wordPairs = new List<KeyValuePair<Annotation, Annotation>>();
                foreach (var ann in tier.Annotations)
                {
                    var nSegment = FindSegment(ann, annotations, segmentIndex, segments);
                    if (nSegment < 0)
                        continue;
                    if (values[nSegment] == null)
                        values[nSegment] = new List<string>();
                    else
                        bMultiplePerSegment = true;
                    if (!String.IsNullOrEmpty(ann.Value))
                        values[nSegment].Add(ann.Value);

                    // e.g. a FLEx 'wordGloss' refers to the 'word' it glosses
                    Annotation annWord;
                    if ((ann.RefId != null) && annotations.TryGetValue(ann.RefId, out annWord) &&
                        !segmentIndex.ContainsKey(annWord) && (annWord.Tier.LangCode != tier.LangCode))
                        wordPairs.Add(new KeyValuePair<Annotation, Annotation>(annWord, ann));
                }

                if (values.All(v => v == null))
                    continue;

                var kind = (bMultiplePerSegment || IsBelowSubdivision(tier))
                               ? ImportedTier.TierKind.WordGloss
                               : ImportedTier.TierKind.PhraseTranslation;

                // a word-level tier in the baseline language just repeats the baseline
                if ((kind == ImportedTier.TierKind.WordGloss) && !String.IsNullOrEmpty(tier.LangCode) &&
                    (tier.LangCode == baselineTier.LangCode))
                    continue;

                // Seth's 'Gloss' tier is a word-by-word gloss, even if it's one annotation per line
                if (tier.TypeRef == CstrTypeGloss)
                    kind = ImportedTier.TierKind.WordGloss;

                var importedTier = new ImportedTier
                {
                    Name = TierDisplayName(tier),
                    LangCode = tier.LangCode,
                    Kind = kind,
                    Lines = values.Select(v => (v == null) ? null : String.Join(" ", v)).ToList()
                };

                if (kind == ImportedTier.TierKind.WordGloss)
                {
                    importedTier.WordLangCode = wordPairs.Select(p => p.Key.Tier.LangCode).FirstOrDefault();
                    foreach (var pair in wordPairs.Where(p => p.Key.Tier.LangCode == importedTier.WordLangCode))
                        importedTier.AddWordPair(pair.Key.Value, pair.Value.Value);
                }

                importedText.Tiers.Add(importedTier);
            }

            return importedText;
        }

        private static Tier ChooseBaselineTier(List<Tier> tiers)
        {
            var baselineTier = tiers.FirstOrDefault(t => t.TypeRef == CstrTypeSayMoreTranscription) ??
                               tiers.FirstOrDefault(t => t.TypeRef == CstrTypeFlexPhrase);
            if (baselineTier != null)
                return baselineTier;

            // otherwise, the time-aligned tier with the most (non-empty) segments; if it's
            //  a tie (e.g. a paragraph tier with one phrase per paragraph), the lower one
            return tiers.Where(t => t.Annotations.Any(a => a.Start != null))
                        .OrderByDescending(t => t.Annotations.Count(a => !String.IsNullOrEmpty(a.Value)))
                        .ThenByDescending(t => t.Depth)
                        .FirstOrDefault();
        }

        private static int FindSegment(Annotation ann, Dictionary<string, Annotation> annotations,
                                       Dictionary<Annotation, int> segmentIndex, List<Annotation> segments)
        {
            // follow the chain of references up to the baseline (e.g. wordGloss -> word -> phrase)
            var a = ann;
            for (var i = 0; (a != null) && (i < 100); i++)
            {
                int nIndex;
                if (segmentIndex.TryGetValue(a, out nIndex))
                    return nIndex;
                if (a.RefId == null)
                    break;
                annotations.TryGetValue(a.RefId, out a);
            }

            // a time-aligned annotation goes with whichever segment its midpoint falls in
            if ((a == null) || (a.Start == null) || (a.End == null))
                return -1;
            var mid = (a.Start.Value + a.End.Value) / 2;
            return segments.FindIndex(s => (s.Start <= mid) && (mid < s.End));
        }

        private static bool IsBelowSubdivision(Tier tier)
        {
            // e.g. FLEx 'wordGloss' (Symbolic_Association) under 'word' (Symbolic_Subdivision)
            for (var t = tier; t != null; t = t.Parent)
                if ((t.TypeRef != null) && t.TypeRef.StartsWith("word", StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // FLEx-style tier ids end with the item type and language, e.g. "A_phrase-gls-id"
        private static readonly Regex RegexFlexTierId = new Regex(@"-(?:txt|gls|lit|punct)-(.+)$");

        // FLEx-style tier ids (e.g. "A_phrase-gls-id") are shown as "phraseGloss (id)"
        private static string TierDisplayName(Tier tier)
        {
            return ((tier.Id != null) && RegexFlexTierId.IsMatch(tier.Id) && !String.IsNullOrEmpty(tier.TypeRef))
                       ? String.Format("{0} ({1})", tier.TypeRef, tier.LangCode)
                       : tier.Id;
        }

        private static string LangCodeFromFlexTierId(string strTierId)
        {
            if (strTierId == null)
                return null;
            var match = RegexFlexTierId.Match(strTierId);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static long? TimeValue(Dictionary<string, long> timeSlots, string strSlotId)
        {
            long value;
            if ((strSlotId != null) && timeSlots.TryGetValue(strSlotId, out value))
                return value;
            return null;
        }

        private static string GetMediaFile(XDocument doc, string strEafFile)
        {
            var folder = Path.GetDirectoryName(strEafFile) ?? String.Empty;
            foreach (var media in doc.Descendants("MEDIA_DESCRIPTOR"))
            {
                var strFile = ExternalImportHelper.LocalFileFromUrl((string)media.Attribute("RELATIVE_MEDIA_URL"), folder) ??
                              ExternalImportHelper.LocalFileFromUrl((string)media.Attribute("MEDIA_URL"), folder);
                if (strFile != null)
                    return strFile;
            }
            return null;
        }
    }

    // Reads the interlinear texts in a FLEx (or FlexText Editor) .flextext file. The
    //  phrase is the story line: its 'txt' item is the baseline, its 'gls'/'lit' items
    //  are the translations (one tier per language), and the word glosses are joined
    //  into one line per phrase.
    public static class FlexTextReader
    {
        private const string CstrItemTypeTranscription = "txt";
        private const string CstrItemTypeGloss = "gls";
        private const string CstrItemTypeLiteral = "lit";
        private const string CstrItemTypePunctuation = "punct";

        public static List<ImportedText> Read(string strFlexTextFile)
        {
            var doc = XDocument.Load(strFlexTextFile);
            var folder = Path.GetDirectoryName(strFlexTextFile) ?? String.Empty;
            var texts = new List<ImportedText>();
            foreach (var elemText in doc.Descendants("interlinear-text"))
            {
                var importedText = new ImportedText
                {
                    SourceFile = strFlexTextFile,
                    Title = elemText.Elements("item")
                                    .Where(i => (string)i.Attribute("type") == "title")
                                    .Select(i => i.Value.Trim())
                                    .FirstOrDefault(v => !String.IsNullOrEmpty(v)),
                    MediaFile = elemText.Descendants("media")
                                        .Select(m => ExternalImportHelper.LocalFileFromUrl((string)m.Attribute("location"), folder))
                                        .FirstOrDefault(f => f != null)
                };

                var vernacularLangs = new HashSet<string>(
                    elemText.Descendants("language")
                            .Where(l => (string)l.Attribute("vernacular") == "true")
                            .Select(l => (string)l.Attribute("lang")));

                var phrases = elemText.Descendants("phrase").ToList();
                var tiers = new Dictionary<string, ImportedTier>();
                for (var i = 0; i < phrases.Count; i++)
                {
                    var phrase = phrases[i];
                    var words = phrase.Elements("words").Elements("word").ToList();

                    // phrase-level items (the transcription and its translations)
                    var bHasBaseline = false;
                    foreach (var item in phrase.Elements("item"))
                    {
                        var strType = (string)item.Attribute("type");
                        var strLang = (string)item.Attribute("lang");
                        ImportedTier.TierKind kind;
                        string strName;
                        switch (strType)
                        {
                            case CstrItemTypeTranscription:
                                kind = ImportedTier.TierKind.Baseline;
                                strName = Localizer.Str("Transcription");
                                bHasBaseline = true;
                                break;
                            case CstrItemTypeGloss:
                                kind = ImportedTier.TierKind.PhraseTranslation;
                                strName = Localizer.Str("Free translation");
                                break;
                            case CstrItemTypeLiteral:
                                kind = ImportedTier.TierKind.PhraseTranslation;
                                strName = Localizer.Str("Literal translation");
                                break;
                            default:
                                continue; // e.g. segnum, note
                        }
                        AddValue(tiers, strType, strLang, strName, kind, i, item.Value.Trim());
                    }

                    // if there's no phrase-level transcription, build it from the words
                    if (!bHasBaseline && words.Any())
                    {
                        var strLang = vernacularLangs.FirstOrDefault();
                        var value = String.Join(" ", words.Elements("item")
                                                          .Where(it => ((string)it.Attribute("type") == CstrItemTypeTranscription) ||
                                                                       ((string)it.Attribute("type") == CstrItemTypePunctuation))
                                                          .Select(it => it.Value.Trim()));
                        AddValue(tiers, CstrItemTypeTranscription, strLang, Localizer.Str("Transcription"),
                                 ImportedTier.TierKind.Baseline, i, value);
                    }

                    // word glosses, joined into one line per phrase (one tier per language)
                    foreach (var langGroup in words.Elements("item")
                                                   .Where(it => (string)it.Attribute("type") == CstrItemTypeGloss)
                                                   .GroupBy(it => (string)it.Attribute("lang")))
                    {
                        var tier = AddValue(tiers, "word-" + CstrItemTypeGloss, langGroup.Key, Localizer.Str("Word gloss"),
                                            ImportedTier.TierKind.WordGloss, i,
                                            String.Join(" ", langGroup.Select(it => it.Value.Trim())));

                        // and each word with its gloss (e.g. for an Adapt It knowledge base)
                        foreach (var elemGloss in langGroup)
                        {
                            var elemWord = elemGloss.Parent?.Elements("item")
                                                    .FirstOrDefault(it => (string)it.Attribute("type") == CstrItemTypeTranscription);
                            if (elemWord == null)
                                continue;
                            var strWordLang = (string)elemWord.Attribute("lang");
                            if (tier.WordLangCode == null)
                                tier.WordLangCode = strWordLang;
                            if (strWordLang == tier.WordLangCode)
                                tier.AddWordPair(elemWord.Value.Trim(), elemGloss.Value.Trim());
                        }
                    }
                }

                // pad all tiers to the same number of lines
                foreach (var tier in tiers.Values)
                    while (tier.Lines.Count < phrases.Count)
                        tier.Lines.Add(null);

                // if there are baselines in more than one language, only the first is the baseline
                var bFoundBaseline = false;
                foreach (var tier in tiers.Values.Where(t => t.Kind == ImportedTier.TierKind.Baseline).ToList())
                {
                    if (bFoundBaseline)
                        tier.Kind = ImportedTier.TierKind.PhraseTranslation;
                    bFoundBaseline = true;
                }

                importedText.Tiers.AddRange(tiers.Values.OrderBy(t => t.Kind));
                texts.Add(importedText);
            }
            return texts;
        }

        private static ImportedTier AddValue(Dictionary<string, ImportedTier> tiers, string strType, string strLang,
                                             string strName, ImportedTier.TierKind kind, int nLine, string value)
        {
            var key = strType + "|" + strLang;
            ImportedTier tier;
            if (!tiers.TryGetValue(key, out tier))
            {
                tier = new ImportedTier
                {
                    Name = String.IsNullOrEmpty(strLang) ? strName : String.Format("{0} ({1})", strName, strLang),
                    LangCode = strLang,
                    Kind = kind
                };
                tiers.Add(key, tier);
            }

            while (tier.Lines.Count < nLine)
                tier.Lines.Add(null);

            if (tier.Lines.Count == nLine)
                tier.Lines.Add(value);
            else if (!String.IsNullOrEmpty(value))  // shouldn't happen, but don't lose anything
                tier.Lines[nLine] = String.IsNullOrEmpty(tier.Lines[nLine]) ? value : tier.Lines[nLine] + " " + value;
            return tier;
        }
    }

    public static class ExternalImportHelper
    {
        // returns the local file for a (possibly relative) media url, if it exists
        public static string LocalFileFromUrl(string strUrl, string strFolder)
        {
            if (String.IsNullOrEmpty(strUrl))
                return null;

            try
            {
                string strPath;
                if (strUrl.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                    strPath = Uri.UnescapeDataString(Regex.Replace(strUrl, "^file:/*", String.Empty, RegexOptions.IgnoreCase));
                else if (strUrl.Contains("://"))
                    return null;  // e.g. an http url
                else
                    strPath = Uri.UnescapeDataString(strUrl);

                if (strPath.StartsWith("./"))
                    strPath = strPath.Substring(2);

                var strFile = Path.IsPathRooted(strPath) ? strPath : Path.Combine(strFolder, strPath);
                if (File.Exists(strFile))
                    return Path.GetFullPath(strFile);

                // relative urls are sometimes stale, so try the file name alongside the source file
                strFile = Path.Combine(strFolder, Path.GetFileName(strPath));
                return File.Exists(strFile) ? strFile : null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        // lets the user pick one of several texts (e.g. from a .flextext with more than one)
        public static ImportedText ChooseText(IWin32Window owner, List<ImportedText> texts)
        {
            if (texts.Count <= 1)
                return texts.FirstOrDefault();

            using (var dlg = new Form())
            using (var listBox = new ListBox())
            using (var buttonOk = new Button())
            using (var buttonCancel = new Button())
            {
                dlg.Text = Localizer.Str("Choose the text to import");
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new Size(400, 300);
                dlg.MinimizeBox = dlg.MaximizeBox = false;
                dlg.ShowInTaskbar = false;

                buttonOk.Text = Localizer.Str("&OK");
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                buttonOk.Location = new Point(232, 268);
                buttonCancel.Text = Localizer.Str("&Cancel");
                buttonCancel.DialogResult = DialogResult.Cancel;
                buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                buttonCancel.Location = new Point(313, 268);

                listBox.Location = new Point(12, 12);
                listBox.Size = new Size(376, 248);
                listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                listBox.Items.AddRange(texts.Cast<object>().ToArray());
                listBox.SelectedIndex = 0;
                listBox.DoubleClick += (s, e) => dlg.DialogResult = DialogResult.OK;

                dlg.Controls.AddRange(new Control[] {listBox, buttonOk, buttonCancel});
                dlg.AcceptButton = buttonOk;
                dlg.CancelButton = buttonCancel;

                return (dlg.ShowDialog(owner) == DialogResult.OK)
                           ? listBox.SelectedItem as ImportedText
                           : null;
            }
        }
    }
}
