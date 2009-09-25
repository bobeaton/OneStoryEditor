using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using NetLoc;
using System.Text.RegularExpressions;
using Paratext;
using SILUBS.ScriptureChecks;
using System.Linq;

namespace OneStoryProjectEditor
{
    public enum BiblicalTermStatus { SomeMissing, AllFound, AllFoundOrDenied };

    public class TermRendering : IComparable<TermRendering>
    {
        private BiblicalTermStatus status = BiblicalTermStatus.SomeMissing;
        private List<string> denials = new List<string>();
        private string id = "";
        private CharacterCategorizer cc;
        private bool guess = false;
        private string notes = "";
        private string tag = "";

        /// <summary>
        /// A list of renderings. These may include * as a wild card charater a beginning or end of words.
        /// No comment/note info is included.
        /// </summary>
        private List<string> renderingsList = new List<string>();

        /// <summary>
        /// List of complete entry for a single rendering include comments if any.
        /// e.g.:  buris* (domesticated animal)
        /// </summary>
        private List<string> renderingsEntries = new List<string>();

        public TermRendering()
        {}

        public TermRendering(TermRendering other)
        {
            Id = other.Id;
            Status = other.Status;
            Guess = other.Guess;
            Renderings = other.Renderings;
            Notes = other.Notes;
            Denials = new List<string>(other.Denials);
        }

        [XmlIgnore]
        public CharacterCategorizer CharacterCategorizer
        {
            get { return cc; }
            set { cc = value; }
        }

        /// <summary>
        /// Id to match Term.Id.
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public BiblicalTermStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Some or all renderings are missing for this term
        /// </summary>
        public bool IsMissing
        {
            get { return status == BiblicalTermStatus.SomeMissing; }
        }

        /// <summary>
        /// True if the current renderings are a guess
        /// </summary>
        public bool Guess
        {
            get { return guess; }
            set { guess = value; }
        }

        /// <summary>
        /// Comma separated list of allowed rendering for this term.
        /// </summary>
        public string Renderings
        {
            get
            {
                return string.Join(", ", renderingsEntries.ToArray());
            }
            set
            {
                ParseRenderings(value);
            }
        }

        /// <summary>
        /// Parse text field.
        /// Entries in field are separated by commas.
        /// Renderings can include comments in parenthesis.
        /// Entire entry goes into RenderingsEntries.
        /// Entry without comments goes into RenderingsList.
        /// </summary>
        /// <param name="value"></param>
        private void ParseRenderings(string value)
        {
            var list = new List<string>();
            var entries = new List<string>();

            string rendering = "";
            string entry = "";

            int parLevel = 0;

            foreach (char c in value)
            {
                if (c == '(')
                {
                    ++parLevel;
                    entry += c;
                }
                else if (c == ')')
                {
                    --parLevel;
                    if (parLevel < 0)
                    {
                        throw new Exception(Localizer.Str("Too many right parentheses."));
                    }
                    entry += c;
                }
                else if (c == ',')
                {
                    if (parLevel == 0)
                        AddEntry(ref entry, ref rendering, list, entries);
                    else
                        entry += c;
                }
                else
                {
                    if (parLevel == 0)
                        rendering += c;
                    entry += c;
                }
            }

            if (parLevel != 0)
            {
                throw new ArgumentException(Localizer.Str("Unbalanced parentheses."));
            }

            AddEntry(ref entry, ref rendering, list, entries);

            renderingsList = list;
            renderingsEntries = entries;
        }

        private void AddEntry(ref string entry, ref string rendering, List<string> list, List<string> entries)
        {
            rendering = Regex.Replace(rendering, @"\s+", " ");

            if (entry != "")
            {
                list.Add(rendering.Trim());
                entries.Add(entry.Trim());
            }

            entry = "";
            rendering = "";
        }

        /// <summary>
        /// Renderings for this term as a list.
        /// </summary>
        [XmlIgnore]
        public List<string> RenderingsList
        {
            get { return renderingsList; }
            //set { renderingsList = value; }    // can only set this via RenderingsEntries
        }

        /// <summary>
        /// Entry for each rendering. Entry may include comments in parenthesis.
        /// </summary>
        [XmlIgnore]
        public List<string> RenderingsEntries
        {
            get { return renderingsEntries; }
            set { Renderings = string.Join(", ", value.ToArray()); }
        }

        public string Notes
        {
            get
            {
                // Normalize newline sequences, xml serialization seems to make them all \n
                string notes2 = notes.Replace("\r", "");
                notes2 = notes2.Replace("\n", "\r\n");

                return notes2;
            }
            set { notes = value; }
        }

        public List<string> Denials
        {
            get { return denials; }
            set { denials = value; }
        }

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Return true if any one of the renderings for this term is found.
        /// </summary>
        /// <param name="parts">List of word, punct, word, punct, ...</param>
        /// <param name="i">position in list to inspect, multiple words may be matched starting here</param>
        /// <param name="wordCount">number of words matched</param>
        /// <returns>true, if a match is found starting at this position</returns>
        public bool Matches(List<string> parts, int i, out int wordCount)
        {
            wordCount = 0;

            foreach (string phrase in renderingsList)
            {
                int wordCount1;
                if (MatchPhrase(phrase, parts, i, out wordCount1))
                    if (wordCount1 > wordCount)
                        wordCount = wordCount1;
            }

            return wordCount > 0;
        }

        /// <summary>
        /// Return true if one specific rendering for this term is found.
        /// </summary>
        /// <param name="renderingPhrase">rendering word or phrase we are looking for</param>
        /// <param name="parts">List of word, punct, word, punct, ...</param>
        /// <param name="i">position in list to inspect, multiple words may be matched starting here</param>
        /// <param name="wordCount">number of words matched</param>
        /// <returns>true, if a match is found starting at this position</returns>
        private bool MatchPhrase(string renderingPhrase, List<string> parts, int i, out int wordCount)
        {
            /*
            var categorizer = scrText.CharacterCategorizer();
            string[] words = categorizer.WordAndPuncts(renderingPhrase).Select(wnp => wnp.Word).ToArray();

            wordCount = words.Length;
            if (wordCount == 0)
                return false;   // Ignore an empty rendering

            // Match the words in the rendering one at a time against the
            // words in starting at parts[i]
            int j;
            for (j = 0; (j < wordCount) && (i + 2 * j < parts.Count); ++j)
                if (!MatchWord(words[j], parts[i + 2 * j], wordCount > 1, scrText))
                    return false;

            if (j != wordCount)
                return false;
            */
            wordCount = 0;
            return true;
        }
        /*
        private bool MatchWord(string renderingWord, string word, bool multiword)
        {
            if (renderingWord.Length == 0)
                return false;

            // Make everything lower case before doing match
            renderingWord = cc.ToLower(renderingWord);
            word = cc.ToLower(word);

            if (renderingWord.Contains("*"))
                return MatchWildcardWord(renderingWord, word, multiword);

            return MatchNonWildcardWord(renderingWord, word, scrText);
        }

        private bool MatchNonWildcardWord(string renderingWord, string word)
        {
            if (renderingWord == word)
                return true;

            if (MatchAnySuffix(renderingWord, word, scrText.WordSuffixesList))
                return true;

            if (scrText.WordPrefixes == null)
                return false;

            foreach (string prefix in scrText.WordPrefixesList)
            {
                string wordWithPrefix = ApplyPrefix(prefix, renderingWord);
                if (MatchAnySuffix(wordWithPrefix, word, scrText.WordSuffixesList))
                    return true;
            }

            return false;
        }

        private bool MatchAnySuffix(string renderingWord, string word, List<string> suffixes)
        {
            if (renderingWord == word)
                return true;

            if (suffixes == null)
                return false;

            foreach (string suffix in suffixes)
            {
                string wordWithSuffix = ApplySuffix(suffix, renderingWord);
                if (wordWithSuffix == word)
                    return true;
            }

            return false;
        }

        private string ApplyPrefix(string prefix, string renderingWord)
        {
            string[] parts = prefix.Split('/');
            if (parts.Length > 1)
            {
                if (!renderingWord.StartsWith(parts[0]))
                    return null;

                return parts[1] + renderingWord.Substring(parts[0].Length);
            }

            return prefix + renderingWord;
        }

        private string ApplySuffix(string suffix, string renderingWord)
        {
            string[] parts = suffix.Split('/');
            if (parts.Length > 1)
            {
                if (!renderingWord.EndsWith(parts[0]))
                    return null;

                return renderingWord.Substring(0, renderingWord.Length - parts[0].Length) + parts[1];
            }

            return renderingWord + suffix;
        }

        // Match a rendering word to a word in text.
        // '*' is a wild card that can appear at the start or end of a rendering word.
        // A rendering word consisting of only '*' matched any word.
        private bool MatchWildcardWord(string renderingWord, string word, bool multiword)
        {
            // A pattern containing an 'any word' wild card is ignored unless we are
            // have a multiword phrase to match, otherwise every word in the text would match.
            if (renderingWord == "*")
                return multiword;

            bool wildCardStart = false;
            if (renderingWord[0] == '*')
            {
                wildCardStart = true;
                renderingWord = renderingWord.Substring(1);
            }

            bool wildCardEnd = false;
            if (renderingWord[renderingWord.Length - 1] == '*')
            {
                wildCardEnd = true;
                renderingWord = renderingWord.Substring(0, renderingWord.Length - 1);
            }

            if (wildCardStart && wildCardEnd)
                return word.Contains(renderingWord);

            if (wildCardEnd)
                return word.StartsWith(renderingWord);

            if (wildCardStart)
                return word.EndsWith(renderingWord);

            return renderingWord == word;  // no wild cards, so must be identical to match.
        }
        */
        /*
        /// <summary>
        /// Return true if ths phrase passed contains any of the existing renderings
        /// </summary>
        /// <param name="phrase">one or more words separated by spaces</param>
        /// <returns>true if it matches any rendering</returns>
        public bool Matches(string phrase, ScrText scrText)
        {
            int wordCount;

            var categorizer = scrText.CharacterCategorizer();
            var parts = new List<string>();
            categorizer.WordAndPuncts(phrase).ForEach(wnp => { parts.Add(wnp.Word); parts.Add(wnp.Punct); });

            for (int i = 0; i < parts.Count; i += 2)
                if (Matches(parts, i, out wordCount, scrText))
                    return true;

            return false;
        }
        */
        /// <summary>
        /// Compare renderings based on their ID.
        /// Used to sort lists of renderings so that they may be searched.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>-1/0/1</returns>
        public int CompareTo(TermRendering other)
        {
            return this.Id.CompareTo(other.Id);
        }

        public void MergeStatus(TermRendering parent, TermRendering theirs)
        {
            if (parent != null && Status == parent.Status)  // I didn't change, take the their status
            {
                Status = theirs.Status;
                return;
            }

            Status = MaxStatus(Status, theirs.Status);
        }

        private static BiblicalTermStatus MaxStatus(BiblicalTermStatus status1, BiblicalTermStatus status2)
        {
            if (status1 == BiblicalTermStatus.AllFound || status2 == BiblicalTermStatus.AllFound)
                return BiblicalTermStatus.AllFound;

            if (status1 == BiblicalTermStatus.AllFoundOrDenied || status2 == BiblicalTermStatus.AllFoundOrDenied)
                return BiblicalTermStatus.AllFoundOrDenied;

            return BiblicalTermStatus.SomeMissing;
        }

        public void MergeGuess(TermRendering parent, TermRendering theirs)
        {
            if (parent != null && Guess == parent.Guess)
            {
                Guess = theirs.Guess;
                return;
            }

            if (parent != null && theirs.Guess == parent.Guess)
                return;

            Guess = Guess && theirs.Guess;
        }

        public void MergeRenderings(TermRendering parent, TermRendering theirs)
        {
            if (parent != null && Renderings == parent.Renderings)  // I didn't change, take the theirs
            {
                Renderings = theirs.Renderings;
                return;
            }

            if (parent != null && theirs.Renderings == parent.Renderings)  // They didn't change either, nothing to do
                return;

            MergeRenderings(theirs);
        }

        // entries to be created for the same Id. MergeRenderings all renderings found.
        internal void MergeRenderings(TermRendering rend)
        {
            List<string> merged = new List<string>(RenderingsEntries);
            foreach (string other in rend.RenderingsEntries)
                if (!merged.Contains(other))
                    merged.Add(other);

            RenderingsEntries = merged;
        }

        public void MergeNotes(TermRendering parent, TermRendering theirs)
        {
            if (parent != null && Notes == parent.Notes)  // I didn't change, take the theirs
            {
                Notes = theirs.Notes;
                return;
            }

            if (parent != null && theirs.Notes == parent.Notes)  // They didn't change either, nothing to do
                return;

            int i = 0;
            while (i < notes.Count() && i < theirs.Notes.Count() && notes[i] == theirs.Notes[i])
                ++i;

            if (i < theirs.Notes.Count())
                notes += "\r\n" + theirs.Notes.Substring(i);
        }

        public void MergeDenials(TermRendering parent, TermRendering theirs)
        {
            if (parent != null && Denials.SequenceEqual(parent.Denials))  // I didn't change, take the theirs
            {
                Denials = new List<string>(theirs.Denials);
                return;
            }

            if (parent != null && theirs.Denials.SequenceEqual(parent.Denials))  // They didn't change either, nothing to do
                return;

            foreach (var denial in theirs.Denials)
                if (!Denials.Contains(denial))
                    Denials.Add(denial);

            Denials.Sort();
        }

        public override bool Equals(object obj)
        {
            TermRendering other2 = obj as TermRendering;
            if (other2 == null)
                return base.Equals(obj);

            return
                Id == other2.Id &&
                Status == other2.Status &&
                Guess == other2.Guess &&
                Renderings == other2.Renderings &&
                Notes == other2.Notes &&
                Denials.ToString() == other2.Denials.ToString();
        }

        /// <summary>
        /// Generate HashCode using same fields used for Equals
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                Id.GetHashCode() +
                Status.GetHashCode() +
                Guess.GetHashCode() +
                Renderings.GetHashCode() +
                Notes.GetHashCode() +
                Denials.ToString().GetHashCode();
        }
    }
}
