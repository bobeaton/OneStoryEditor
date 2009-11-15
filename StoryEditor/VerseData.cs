using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class VerseData
    {
        public string guid;
        public StringTransfer VernacularText = null;
        public StringTransfer NationalBTText = null;
        public StringTransfer InternationalBTText = null;
        public AnchorsData Anchors = null;
        public TestQuestionsData TestQuestions = null;
        public RetellingsData Retellings = null;
        public ConsultantNotesData ConsultantNotes = null;
        public CoachNotesData CoachNotes = null;

        public VerseData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
        {
            guid = theVerseRow.guid;
            VernacularText = new StringTransfer((!theVerseRow.IsVernacularNull()) ? theVerseRow.Vernacular : null);
            NationalBTText = new StringTransfer((!theVerseRow.IsNationalBTNull()) ? theVerseRow.NationalBT : null);
            InternationalBTText = new StringTransfer((!theVerseRow.IsInternationalBTNull()) ? theVerseRow.InternationalBT : null);

            Anchors = new AnchorsData(theVerseRow, projFile);
            TestQuestions = new TestQuestionsData(theVerseRow, projFile);
            Retellings = new RetellingsData(theVerseRow, projFile);
            ConsultantNotes = new ConsultantNotesData(theVerseRow, projFile);
            CoachNotes = new CoachNotesData(theVerseRow, projFile);
        }

        public VerseData()
        {
            guid = Guid.NewGuid().ToString();
            VernacularText = new StringTransfer(null);
            NationalBTText = new StringTransfer(null);
            InternationalBTText = new StringTransfer(null);
            Anchors = new AnchorsData();
            TestQuestions = new TestQuestionsData();
            Retellings = new RetellingsData();
            ConsultantNotes = new ConsultantNotesData();
            CoachNotes = new CoachNotesData();
        }

        public VerseData(VerseData rhs)
        {
            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();   // rhs.guid;

            VernacularText = new StringTransfer(rhs.VernacularText.ToString());
            NationalBTText = new StringTransfer(rhs.NationalBTText.ToString());
            InternationalBTText = new StringTransfer(rhs.InternationalBTText.ToString());
            Anchors = new AnchorsData(rhs.Anchors);
            TestQuestions = new TestQuestionsData(rhs.TestQuestions);
            Retellings = new RetellingsData(rhs.Retellings);
            ConsultantNotes = new ConsultantNotesData(rhs.ConsultantNotes);
            CoachNotes = new CoachNotesData(rhs.CoachNotes);
        }

        public void IndexSearch(int nVerseNum, SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (VernacularText.HasData && findProperties.StoryLanguage)
                lstBoxesToSearch.AddNewVerseString(nVerseNum, VernacularText,
                    ViewItemToInsureOn.eVernacularLangField);
            if (NationalBTText.HasData && findProperties.NationalBT)
                lstBoxesToSearch.AddNewVerseString(nVerseNum, NationalBTText,
                    ViewItemToInsureOn.eNationalLangField);
            if (InternationalBTText.HasData && findProperties.EnglishBT)
                lstBoxesToSearch.AddNewVerseString(nVerseNum, InternationalBTText,
                    ViewItemToInsureOn.eEnglishBTField);
            if (TestQuestions.HasData && findProperties.TestQnA)
                TestQuestions.IndexSearch(nVerseNum, findProperties, ref lstBoxesToSearch);
            if (Retellings.HasData && findProperties.Retellings)
                Retellings.IndexSearch(nVerseNum, findProperties, ref lstBoxesToSearch);
            if (ConsultantNotes.HasData && findProperties.ConsultantNotes)
                ConsultantNotes.IndexSearch(nVerseNum, findProperties, ref lstBoxesToSearch);
            if (CoachNotes.HasData && findProperties.CoachNotes)
                CoachNotes.IndexSearch(nVerseNum, findProperties, ref lstBoxesToSearch);
        }

        public bool HasData
        {
            get
            {
                return (VernacularText.HasData || NationalBTText.HasData || InternationalBTText.HasData
                    || Anchors.HasData || TestQuestions.HasData || Retellings.HasData 
                    || ConsultantNotes.HasData || CoachNotes.HasData);
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement elemVerse = new XElement("verse", new XAttribute("guid", guid));
                if (VernacularText.HasData)
                    elemVerse.Add(new XElement("Vernacular", VernacularText));
                if (NationalBTText.HasData)
                    elemVerse.Add(new XElement("NationalBT", NationalBTText));
                if (InternationalBTText.HasData)
                    elemVerse.Add(new XElement("InternationalBT", InternationalBTText));
                if (Anchors.HasData)
                    elemVerse.Add(Anchors.GetXml);
                if (TestQuestions.HasData)
                    elemVerse.Add(TestQuestions.GetXml);
                if (Retellings.HasData)
                    elemVerse.Add(Retellings.GetXml);
                if (ConsultantNotes.HasData)
                    elemVerse.Add(ConsultantNotes.GetXml);
                if (CoachNotes.HasData)
                    elemVerse.Add(CoachNotes.GetXml);

                return elemVerse;
            }
        }

        public static bool IsViewItemOn(ViewItemToInsureOn eValue, ViewItemToInsureOn eFlag)
        {
            return ((eValue & eFlag) == eFlag);
        }

        [Flags]
        public enum ViewItemToInsureOn
        {
            eVernacularLangField = 1,
            eNationalLangField = 2,
            eEnglishBTField = 4,
            eAnchorFields = 8,
            eStoryTestingQuestionFields = 16,
            eRetellingFields = 32,
            eConsultantNoteFields = 64,
            eCoachNotesFields = 128
        }
    }

    public class VersesData : List<VerseData>
    {
        public VersesData(NewDataSet.storyRow theStoryRow, NewDataSet projFile)
        {
            NewDataSet.versesRow[] theVersesRows = theStoryRow.GetversesRows();
            NewDataSet.versesRow theVersesRow;
            if (theVersesRows.Length == 0)
                theVersesRow = projFile.verses.AddversesRow(theStoryRow);
            else
                theVersesRow = theVersesRows[0];

            foreach (NewDataSet.verseRow aVerseRow in theVersesRow.GetverseRows())
                Add(new VerseData(aVerseRow, projFile));
        }

        public VersesData(VersesData rhs)
        {
            foreach (VerseData aVerse in rhs)
                Add(new VerseData(aVerse));
        }

        public VersesData()
        {
        }

        public VerseData InsertVerse(int nIndex, string strVernacular, string strNationalBT, string strInternationalBT)
        {
            VerseData dataVerse = new VerseData
                                        {
                                            VernacularText = new StringTransfer(strVernacular),
                                            NationalBTText = new StringTransfer(strNationalBT),
                                            InternationalBTText = new StringTransfer(strInternationalBT)
                                        };
            Insert(nIndex, dataVerse);
            return dataVerse;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemVerses = new XElement("verses");
                foreach (VerseData aVerseData in this)
                    elemVerses.Add(aVerseData.GetXml);
                return elemVerses;
            }
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            for (int nVerseNum = 0; nVerseNum < Count; nVerseNum++)
            {
                VerseData aVerseData = this[nVerseNum];
                aVerseData.IndexSearch(nVerseNum, findProperties, ref lstBoxesToSearch);
            }
        }
    }
}
