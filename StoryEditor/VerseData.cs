using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class VerseData
    {
        public string guid;
        public bool IsFirstVerse;
        public bool IsVisible = true;
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

            if (!theVerseRow.IsfirstNull())
                IsFirstVerse = theVerseRow.first;

            if (!theVerseRow.IsvisibleNull())
                IsVisible = theVerseRow.visible;

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
            IsFirstVerse = rhs.IsFirstVerse;
            IsVisible = rhs.IsVisible;

            VernacularText = new StringTransfer(rhs.VernacularText.ToString());
            NationalBTText = new StringTransfer(rhs.NationalBTText.ToString());
            InternationalBTText = new StringTransfer(rhs.InternationalBTText.ToString());
            Anchors = new AnchorsData(rhs.Anchors);
            TestQuestions = new TestQuestionsData(rhs.TestQuestions);
            Retellings = new RetellingsData(rhs.Retellings);
            ConsultantNotes = new ConsultantNotesData(rhs.ConsultantNotes);
            CoachNotes = new CoachNotesData(rhs.CoachNotes);
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (VernacularText.HasData && findProperties.StoryLanguage)
                lstBoxesToSearch.AddNewVerseString(VernacularText,
                    ViewItemToInsureOn.eVernacularLangField);
            if (NationalBTText.HasData && findProperties.NationalBT)
                lstBoxesToSearch.AddNewVerseString(NationalBTText,
                    ViewItemToInsureOn.eNationalLangField);
            if (InternationalBTText.HasData && findProperties.EnglishBT)
                lstBoxesToSearch.AddNewVerseString(InternationalBTText,
                    ViewItemToInsureOn.eEnglishBTField);
            if (TestQuestions.HasData && findProperties.TestQnA)
                TestQuestions.IndexSearch(findProperties, ref lstBoxesToSearch);
            if (Retellings.HasData && findProperties.Retellings)
                Retellings.IndexSearch(findProperties, ref lstBoxesToSearch);
            if (ConsultantNotes.HasData && findProperties.ConsultantNotes)
                ConsultantNotes.IndexSearch(findProperties, ref lstBoxesToSearch);
            if (CoachNotes.HasData && findProperties.CoachNotes)
                CoachNotes.IndexSearch(findProperties, ref lstBoxesToSearch);
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
                XElement elemVerse = new XElement("verse", 
                    new XAttribute("guid", guid));
                
                // only need to write out the 'first' attribute if it's true
                if (IsFirstVerse)
                    elemVerse.Add(new XAttribute("first", IsFirstVerse));

                // only need to write out the 'visible' attribute if it's false
                if (!IsVisible)
                    elemVerse.Add(new XAttribute("visible", IsVisible));
                
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

        public string Html
        {
            get
            {
                string strHtml = String.Format(Properties.Resources.HTML_TableCell, "ln n:");
                return strHtml;
            }
        }

        public static bool IsViewItemOn(ViewItemToInsureOn eValue, ViewItemToInsureOn eFlag)
        {
            return ((eValue & eFlag) == eFlag);
        }

        public static ViewItemToInsureOn SetItemsToInsureOn
            (
            bool bLangVernacular,
            bool bLangNationalBT,
            bool bLangInternationalBT,
            bool bAnchors,
            bool bStoryTestingQuestions,
            bool bRetellings,
            bool bConsultantNotes,
            bool bCoachNotes,
            bool bBibleViewer
            )
        {
            ViewItemToInsureOn items = 0;
            if (bLangVernacular)
                items |= VerseData.ViewItemToInsureOn.eVernacularLangField;
            if (bLangNationalBT)
                items |= VerseData.ViewItemToInsureOn.eNationalLangField;
            if (bLangInternationalBT)
                items |= VerseData.ViewItemToInsureOn.eEnglishBTField;
            if (bAnchors)
                items |= VerseData.ViewItemToInsureOn.eAnchorFields;
            if (bStoryTestingQuestions)
                items |= VerseData.ViewItemToInsureOn.eStoryTestingQuestionFields;
            if (bRetellings)
                items |= VerseData.ViewItemToInsureOn.eRetellingFields;
            if (bConsultantNotes)
                items |= VerseData.ViewItemToInsureOn.eConsultantNoteFields;
            if (bCoachNotes)
                items |= VerseData.ViewItemToInsureOn.eCoachNotesFields;
            if (bBibleViewer)
                items |= VerseData.ViewItemToInsureOn.eBibleViewer;
            return items;
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
            eCoachNotesFields = 128,
            eBibleViewer = 256
        }
    }

    public class VersesData : List<VerseData>
    {
        public VerseData FirstVerse;

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

            // the zeroth verse is special for global connotes
            if ((Count > 0) && this[0].IsFirstVerse)
            {
                FirstVerse = this[0];
                RemoveAt(0);
            }
            else
                CreateFirstVerse();
        }

        public VersesData(VersesData rhs)
        {
            FirstVerse = new VerseData(rhs.FirstVerse);
            foreach (VerseData aVerse in rhs)
                Add(new VerseData(aVerse));
        }

        public VersesData()
        {
        }

        public void CreateFirstVerse()
        {
            FirstVerse = new VerseData { IsFirstVerse = true };
        }

        public VerseData InsertVerse(int nIndex, string strVernacular, string strNationalBT, string strInternationalBT)
        {
            var dataVerse = new VerseData
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
            get { return (Count > 0) || ((FirstVerse != null) && (FirstVerse.HasData)); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemVerses = new XElement("verses");
                
                // write out the zeroth verse first
                elemVerses.Add(FirstVerse.GetXml);

                // then write out the rest
                foreach (VerseData aVerseData in this)
                    elemVerses.Add(aVerseData.GetXml);

                return elemVerses;
            }
        }

        public string Html
        {
            get
            {
                string strHtml = null;
                foreach (VerseData aVerseData in this)
                    strHtml = aVerseData.Html;

                return String.Format(Properties.Resources.HTML_Table, strHtml);
            }
        }

        protected string GetHeaderRow(string strHeader, int nVerseIndex,
            ConsultNotesDataConverter theCNsDC, TeamMemberData LoggedOnMember)
        {
            string strHtmlAddNoteButton = null;
            if (theCNsDC.HasAddNotePrivilege(LoggedOnMember.MemberType))
                strHtmlAddNoteButton = String.Format(Properties.Resources.HTML_TableCell,
                                                     String.Format(Properties.Resources.HTML_Button,
                                                                   nVerseIndex,
                                                                   "return OnAddNote(this);",
                                                                   "Add Note"));

            return String.Format(Properties.Resources.HTML_TableRowColor, "#AACCFF",
                                 String.Format("{0}{1}",
                                               String.Format(Properties.Resources.HTML_TableCellWidth, 100,
                                                             strHeader),
                                                             strHtmlAddNoteButton));
        }

        public string ConsultantNotesHtml(StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember, bool bViewHidden)
        {
            string strHtml = null;
            strHtml += GetHeaderRow("Story:", 0, FirstVerse.ConsultantNotes, LoggedOnMember);

            strHtml += FirstVerse.ConsultantNotes.Html(theStoryStage, LoggedOnMember, bViewHidden, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.ConsultantNotes, LoggedOnMember);

                strHtml += aVerseData.ConsultantNotes.Html(theStoryStage, LoggedOnMember, bViewHidden, i);
            }

            return String.Format(Properties.Resources.HTML_Table, strHtml);
        }

        public string CoachNotesHtml(StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember, bool bViewHidden)
        {
            string strHtml = null;
            strHtml += GetHeaderRow("Story:", 0, FirstVerse.CoachNotes, LoggedOnMember);

            strHtml += FirstVerse.CoachNotes.Html(theStoryStage, LoggedOnMember, bViewHidden, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.CoachNotes, LoggedOnMember);

                strHtml += aVerseData.CoachNotes.Html(theStoryStage, LoggedOnMember, bViewHidden, i);
            }

            return String.Format(Properties.Resources.HTML_Table, strHtml);
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            // put the zeroth ConNotes box in the search queue
            FirstVerse.IndexSearch(findProperties, ref lstBoxesToSearch);

            for (int nVerseNum = 0; nVerseNum < Count; nVerseNum++)
            {
                VerseData aVerseData = this[nVerseNum];
                aVerseData.IndexSearch(findProperties, ref lstBoxesToSearch);
            }
        }
    }
}
