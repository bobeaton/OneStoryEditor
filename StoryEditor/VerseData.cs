using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class VerseData
    {
        public string guid = null;
        public string VernacularText = null;
        public string NationalBTText = null;
        public string InternationalBTText = null;
        public AnchorsData Anchors = null;
        public TestQuestionsData TestQuestions = null;
        public RetellingsData Retellings = null;
        public ConsultantNotesData ConsultantNotes = null;
        public CoachNotesData CoachNotes = null;

        public VerseData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            guid = theVerseRow.guid;
            if (!theVerseRow.IsVernacularNull())
                VernacularText = theVerseRow.Vernacular;
            if (!theVerseRow.IsNationalBTNull())
                NationalBTText = theVerseRow.NationalBT;
            if (!theVerseRow.IsInternationalBTNull())
                InternationalBTText = theVerseRow.InternationalBT;

            Anchors = new AnchorsData(theVerseRow, projFile);
            TestQuestions = new TestQuestionsData(theVerseRow, projFile);
            Retellings = new RetellingsData(theVerseRow, projFile);
            ConsultantNotes = new ConsultantNotesData(theVerseRow, projFile);
            CoachNotes = new CoachNotesData(theVerseRow, projFile);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "verse", new XAttribute("guid", guid),
                    new XElement(StoryEditor.ns + "Vernacular", VernacularText),
                    new XElement(StoryEditor.ns + "NationalBT", NationalBTText),
                    new XElement(StoryEditor.ns + "InternationalBT", InternationalBTText),
                    Anchors.GetXml,
                    TestQuestions.GetXml,
                    Retellings.GetXml,
                    ConsultantNotes.GetXml,
                    CoachNotes.GetXml);
            }
        }
    }

    public class VersesData : List<VerseData>
    {
        public VersesData(StoryProject.storyRow theStoryRow, StoryProject projFile)
        {
            StoryProject.versesRow[] theVersesRows = theStoryRow.GetversesRows();
            StoryProject.versesRow theVersesRow;
            if (theVersesRows.Length == 0)
                theVersesRow = projFile.verses.AddversesRow(theStoryRow);
            else
                theVersesRow = theVersesRows[0];

            foreach (StoryProject.verseRow aVerseRow in theVersesRow.GetverseRows())
                Add(new VerseData(aVerseRow, projFile));
        }

        public XElement GetXml
        {
            get
            {
                XElement elemVerses = new XElement(StoryEditor.ns + "verses");
                foreach (VerseData aVerseData in this)
                    elemVerses.Add(aVerseData.GetXml);
                return elemVerses;
            }
        }
    }
}
