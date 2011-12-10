using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public class LnCNotesData : List<LnCNote>
    {
        public LnCNotesData()
        {
        }

        public LnCNotesData(NewDataSet projFile)
        {
            NewDataSet.StoryProjectRow theStoryProjectRow = projFile.StoryProject[0];
            NewDataSet.LnCNotesRow[] theLnCNotesRows = theStoryProjectRow.GetLnCNotesRows();
            NewDataSet.LnCNotesRow theLnCNotesRow;
            if (theLnCNotesRows.Length == 0)
                theLnCNotesRow = projFile.LnCNotes.AddLnCNotesRow(theStoryProjectRow);
            else
                theLnCNotesRow = theLnCNotesRows[0];

            foreach (NewDataSet.LnCNoteRow aLnCNoteRow in theLnCNotesRow.GetLnCNoteRows())
                Add(new LnCNote(aLnCNoteRow));
        }

        public XElement GetXml
        {
            get 
            {
                var elem = new XElement("LnCNotes");

                foreach (var aLnCNote in this)
                    elem.Add(aLnCNote.GetXml);

                return elem;
            }
        }

        private static TermRendering GetTermRendering(string strSearchPatterns)
        {
            return new TermRendering { Renderings = strSearchPatterns.Trim() };
        }

        private static bool StringContains(string strSearchPatterns, string strSearchString)
        {
            if (!String.IsNullOrEmpty(strSearchPatterns))
            {
                // try a simple index of without case to see if the search string is part
                //  of any of the L&C note words.
                if (strSearchPatterns.IndexOf(strSearchString, StringComparison.OrdinalIgnoreCase) != -1)
                    return true;

                // also try the reverse: see if the L&C note search patterns find the 
                //  search string (e.g. "God*" will find "God's")
                var termRendering = GetTermRendering(strSearchPatterns);
                var arrRegex = BiblicalTermsHTMLBuilder.GetRegexs(termRendering);
                return BiblicalTermsHTMLBuilder.SearchForHit(arrRegex, strSearchString);
            }
            return false;
        }

        public Dictionary<string,LnCNote> FindHits(string strSearchString, StoryEditor.TextFieldType eFieldType)
        {
            var map = new Dictionary<string, LnCNote>();
            strSearchString = strSearchString.Trim();

            foreach (LnCNote anLnCnote in this)
            {
                switch (eFieldType)
                {
                    case StoryEditor.TextFieldType.Vernacular:
                        if (StringContains(anLnCnote.VernacularRendering, strSearchString))
                            AddWithUniqueKey(map, anLnCnote.VernacularRendering, anLnCnote);
                        break;

                    case StoryEditor.TextFieldType.NationalBt:
                        if (StringContains(anLnCnote.NationalBtRendering, strSearchString))
                            AddWithUniqueKey(map, anLnCnote.NationalBtRendering, anLnCnote);
                        break;

                    case StoryEditor.TextFieldType.InternationalBt:
                        if (StringContains(anLnCnote.InternationalBtRendering, strSearchString))
                            AddWithUniqueKey(map, anLnCnote.InternationalBtRendering, anLnCnote);
                        break;
                }
            }
            return map;
        }

        private static void AddWithUniqueKey(Dictionary<string, LnCNote> map,
            string strKeyToTry, LnCNote anLnCnote)
        {
            int n = 1;
            string strOrigKey = strKeyToTry;
            while (map.ContainsKey(strKeyToTry))
            {
                strKeyToTry = String.Format("{0}.{1}", strOrigKey, n++);
            }
            map.Add(strKeyToTry, anLnCnote);
        }

        public DialogResult CheckForSimilarNote(StoryEditor theSe, string strToSearchForVernacular,
            string strToSearchForNationalBt, string strToSearchForInternationalBt)
        {
            string strMatch = null;
            return (from aLncNote in this
                    where (CheckForSimilarity(aLncNote.InternationalBtRendering, strToSearchForInternationalBt, ref strMatch) ||
                           CheckForSimilarity(aLncNote.NationalBtRendering, strToSearchForNationalBt, ref strMatch) || 
                           CheckForSimilarity(aLncNote.VernacularRendering, strToSearchForVernacular, ref strMatch))
                    select new AddLnCNoteForm(theSe, aLncNote)
                               {
                                   Text = String.Format(Localizer.Str("You already have this L & C note for the word '{0}'! See 'View', 'L&C Notes'"), strMatch)
                               }).Select(dlg => dlg.ShowDialog()).FirstOrDefault();
        }

        public bool CheckForSimilarity(string strRendering, string strToCompare, ref string strMatch)
        {
            if (!String.IsNullOrEmpty(strRendering) &&
                !String.IsNullOrEmpty(strToCompare) &&
                (strRendering.ToLowerInvariant() == strToCompare.ToLowerInvariant()))
            {
                strMatch = strRendering;
                return true;
            }
            return false;
        }

        public string PresentationHtml
        {
            get
            {
                bool bShowVernacular = this.Any(theNote => !String.IsNullOrEmpty(theNote.VernacularRendering));
                bool bShowNationalBt = this.Any(theNote => !String.IsNullOrEmpty(theNote.NationalBtRendering));
                bool bShowInternationalBt = this.Any(theNote => !String.IsNullOrEmpty(theNote.InternationalBtRendering));

                var strHtml = this.Aggregate<LnCNote, string>(null, (current, aNote) =>
                                                                    current + aNote.PresentationHtml(bShowVernacular,
                                                                                                     bShowNationalBt,
                                                                                                     bShowInternationalBt));
                return String.Format(Properties.Resources.HTML_TableBorder, strHtml);
            }
        }
    }

    public class LnCNote
    {
        protected string guid { get; set; }
        public string VernacularRendering { get; set; }
        public string NationalBtRendering { get; set; }
        public string InternationalBtRendering { get; set; }
        public string Notes { get; set; }

        public string PresentationHtml(bool bShowVernacular, 
            bool bShowNationalBt, bool bShowInternationalBt)
        {
            string strRow = null;
            if (bShowVernacular)
            {
                strRow += FormatLanguageColumn(StoryData.CstrLangVernacularStyleClassName,
                                               VernacularRendering);
            }

            if (bShowNationalBt)
            {
                strRow += FormatLanguageColumn(StoryData.CstrLangNationalBtStyleClassName,
                                               NationalBtRendering);
            }

            if (bShowInternationalBt)
            {
                strRow += FormatLanguageColumn(StoryData.CstrLangInternationalBtStyleClassName,
                                               InternationalBtRendering);
            }

            strRow += FormatLanguageColumn(StoryData.CstrLangFreeTranslationStyleClassName,
                                           Notes);

            return String.Format(Properties.Resources.HTML_TableRow, strRow);
        }

        private static string FormatLanguageColumn(string strLangStyleClassName, 
            string strValue)
        {
            return String.Format(Properties.Resources.HTML_TableCell,
                                 String.Format(Properties.Resources.HTML_ParagraphText,
                                               strLangStyleClassName,
                                               strValue));
        }

        private List<string> astrKeyTermId = new List<string>();

        private string KeyTermIds
        {
            get
            {
                return string.Join(", ", astrKeyTermId.ToArray());
            }
            set
            {
                astrKeyTermId.Clear();
                string[] astr = value.Split(new [] {", "}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in astr)
                    astrKeyTermId.Add(s);
            }
        }

#if UnhookParatextBiblicalTerms
        public List<Term> GetKeyTerms(BiblicalTermsList btl)
        {
            return astrKeyTermId.Select(s => btl.GetIfPresent(s)).ToList();
        }

        public string GetKeyTermsGlosses(BiblicalTermsList btl)
        {
            return String.Join(", ",
                astrKeyTermId.Select(s => btl.GetIfPresent(s))
                    .Select(term => term.Gloss).ToArray());
        }

        public void SetKeyTerms(List<Term> list)
        {
            astrKeyTermId.Clear();
            foreach (Term term in list)
                astrKeyTermId.Add(term.Id);
        }
#endif

        public XElement GetXml
        {
            get
            {
                string strNote = "-";   // can't be null
                if (!String.IsNullOrEmpty(Notes))
                    strNote = Notes;

                var elem = new XElement("LnCNote", strNote,
                    new XAttribute("guid", guid));

                if (!String.IsNullOrEmpty(VernacularRendering))
                    elem.Add(new XAttribute("VernacularRendering", VernacularRendering));

                if (!String.IsNullOrEmpty(NationalBtRendering))
                    elem.Add(new XAttribute("NationalBTRendering", NationalBtRendering));

                if (!String.IsNullOrEmpty(InternationalBtRendering))
                    elem.Add(new XAttribute("InternationalBTRendering", InternationalBtRendering));

                if (!String.IsNullOrEmpty(KeyTermIds))
                    elem.Add(new XAttribute("KeyTermId", KeyTermIds));

                return elem;
            }
        }

        public LnCNote()
        {
            guid = Guid.NewGuid().ToString();
            Notes = String.Empty; // xml doesn't like null for this
        }

        public LnCNote(NewDataSet.LnCNoteRow theLnCNoteRow)
        {
            guid = theLnCNoteRow.guid;  // the only thing absolutely required

            Notes = theLnCNoteRow.LnCNote_text;
            if (!theLnCNoteRow.IsVernacularRenderingNull())
                VernacularRendering = theLnCNoteRow.VernacularRendering;
            if (!theLnCNoteRow.IsNationalBTRenderingNull())
                NationalBtRendering = theLnCNoteRow.NationalBTRendering;
            if (!theLnCNoteRow.IsInternationalBTRenderingNull())
                InternationalBtRendering = theLnCNoteRow.InternationalBTRendering;
            if (!theLnCNoteRow.IsKeyTermIdsNull())
                KeyTermIds = theLnCNoteRow.KeyTermIds;
        }
    }
}
