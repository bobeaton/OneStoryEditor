using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    public class TestQuestionData
    {
        public string guid;
        public bool IsVisible = true;
        public StringTransfer QuestionVernacular = null;
        public StringTransfer QuestionNationalBT = null;
        public StringTransfer QuestionInternationalBT = null;
        public AnswersData Answers = null;

        public TestQuestionData(NewDataSet.TestQuestionRow theTestQuestionRow, NewDataSet projFile)
        {
            guid = theTestQuestionRow.guid;
            IsVisible = theTestQuestionRow.visible;

            QuestionVernacular = new StringTransfer((theTestQuestionRow.IsTQVernacularNull()) ? null : theTestQuestionRow.TQVernacular);
            QuestionNationalBT = new StringTransfer((theTestQuestionRow.IsTQNationalBTNull()) ? null : theTestQuestionRow.TQNationalBT);
            QuestionInternationalBT = new StringTransfer((theTestQuestionRow.IsTQInternationalBTNull()) ? null : theTestQuestionRow.TQInternationalBT);
            Answers = new AnswersData(theTestQuestionRow, projFile);
        }

        public TestQuestionData(TestQuestionData rhs)
        {
            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();   // rhs.guid;

            IsVisible = rhs.IsVisible;
            QuestionVernacular = new StringTransfer(rhs.QuestionVernacular.ToString());
            QuestionNationalBT = new StringTransfer(rhs.QuestionNationalBT.ToString());
            QuestionInternationalBT = new StringTransfer(rhs.QuestionInternationalBT.ToString());
            Answers = new AnswersData(rhs.Answers);
        }

        public TestQuestionData()
        {
            guid = Guid.NewGuid().ToString();
            QuestionVernacular = new StringTransfer(null);
            QuestionNationalBT = new StringTransfer(null);
            QuestionInternationalBT = new StringTransfer(null);
            Answers = new AnswersData();
        }

        public bool HasData
        {
            get
            {
                return (QuestionVernacular.HasData
                    || QuestionNationalBT.HasData
                    || QuestionInternationalBT.HasData 
                    || Answers.HasData);
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(QuestionVernacular.HasData
                    || QuestionNationalBT.HasData
                    || QuestionInternationalBT.HasData
                    || Answers.HasData, "you have an empty TestQuestionData");

                XElement eleTQ = new XElement("TestQuestion", 
                    new XAttribute("visible", IsVisible),
                    new XAttribute("guid", guid));
                
                if (QuestionVernacular.HasData)
                    eleTQ.Add(new XElement("TQVernacular", QuestionVernacular));
                if (QuestionNationalBT.HasData)
                    eleTQ.Add(new XElement("TQNationalBT", QuestionNationalBT));
                if (QuestionInternationalBT.HasData)
                    eleTQ.Add(new XElement("TQInternationalBT", QuestionInternationalBT));
                if (Answers.HasData)
                    eleTQ.Add(Answers.GetXml);

                return eleTQ;
            }
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties, 
            SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (QuestionVernacular.HasData && findProperties.StoryLanguage)
                lstBoxesToSearch.AddNewVerseString(QuestionVernacular,
                    VerseData.ViewItemToInsureOn.eStoryTestingQuestionFields |
                    VerseData.ViewItemToInsureOn.eVernacularLangField);
            if (QuestionNationalBT.HasData && findProperties.NationalBT)
                lstBoxesToSearch.AddNewVerseString(QuestionNationalBT,
                    VerseData.ViewItemToInsureOn.eStoryTestingQuestionFields | 
                    VerseData.ViewItemToInsureOn.eNationalLangField);
            if (QuestionInternationalBT.HasData && findProperties.EnglishBT)
                lstBoxesToSearch.AddNewVerseString(QuestionInternationalBT,
                    VerseData.ViewItemToInsureOn.eStoryTestingQuestionFields |
                    VerseData.ViewItemToInsureOn.eEnglishBTField);
            Answers.IndexSearch(findProperties, ref lstBoxesToSearch);
        }

        protected const string CstrTestQuestionsLabelFormat = "tst:";

        public static string TextareaId(int nVerseIndex, int nTQNum, string strTextElementName)
        {
            return String.Format("taTQ_{0}_{1}_{2}", nVerseIndex, nTQNum, strTextElementName);
        }

        public string Html(int nVerseIndex, int nTQNum, int nNumTestQuestionCols, bool bShowVernacular,
            bool bShowNationalBT, bool bShowEnglishBT)
        {
            string strRow = String.Format(Properties.Resources.HTML_TableCell,
                                          CstrTestQuestionsLabelFormat);
            if (bShowVernacular)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, StoryLineControl.CstrFieldNameVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      QuestionVernacular));
            }

            if (bShowNationalBT)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, StoryLineControl.CstrFieldNameNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      QuestionNationalBT));
            }

            if (bShowEnglishBT)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, StoryLineControl.CstrFieldNameInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      QuestionInternationalBT));
            }

            string strTQRow = String.Format(Properties.Resources.HTML_TableRow,
                                                   strRow);

            strTQRow += Answers.Html(nVerseIndex, nNumTestQuestionCols + 1);
            return strTQRow;
        }
    }

    public class TestQuestionsData : List<TestQuestionData>
    {
        public TestQuestionsData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
        {
            NewDataSet.TestQuestionsRow[] theTestQuestionsRows = theVerseRow.GetTestQuestionsRows();
            NewDataSet.TestQuestionsRow theTestQuestionsRow;
            if (theTestQuestionsRows.Length == 0)
                theTestQuestionsRow = projFile.TestQuestions.AddTestQuestionsRow(theVerseRow);
            else
                theTestQuestionsRow = theTestQuestionsRows[0];

            foreach (NewDataSet.TestQuestionRow aTestingQuestionRow in theTestQuestionsRow.GetTestQuestionRows())
                Add(new TestQuestionData(aTestingQuestionRow, projFile));
        }

        public TestQuestionsData(TestQuestionsData rhs)
        {
            foreach (TestQuestionData aTQ in rhs)
                Add(new TestQuestionData(aTQ));
        }

        public TestQuestionsData()
        {
        }

        public TestQuestionData AddTestQuestion()
        {
            TestQuestionData theTQ = new TestQuestionData();
            this.Add(theTQ);
            return theTQ;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an empty TestQuestionsData");
                XElement elemTestQuestions = new XElement("TestQuestions");
                foreach (TestQuestionData aTestQuestionData in this)
                    if (aTestQuestionData.HasData)
                        elemTestQuestions.Add(aTestQuestionData.GetXml);
                return elemTestQuestions;
            }
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (TestQuestionData testQuestionData in this)
                testQuestionData.IndexSearch(findProperties, lstBoxesToSearch);
        }

        public string Html(ProjectSettings projectSettings, 
            VerseData.ViewItemToInsureOn viewItemToInsureOn, 
            StoryStageLogic stageLogic, TeamMemberData loggedOnMember,
            int nVerseIndex, int nNumCols, bool bHasOutsideEnglishBTer)
        {
            int nNumTestQuestionCols = 0;
            bool bShowVernacular =
                (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eVernacularLangField));
            bool bShowNationalBT = (projectSettings.NationalBT.HasData
                                  && (bHasOutsideEnglishBTer
                                      ||
                                      (VerseData.IsViewItemOn(viewItemToInsureOn,
                                                              VerseData.ViewItemToInsureOn.eNationalLangField) &&
                                       !projectSettings.Vernacular.HasData)));
            bool bShowEnglishBT = (VerseData.IsViewItemOn(viewItemToInsureOn,
                                                          VerseData.ViewItemToInsureOn.eEnglishBTField)
                                   && (!bHasOutsideEnglishBTer
                                       || (stageLogic.MemberTypeWithEditToken !=
                                           TeamMemberData.UserTypes.eProjectFacilitator)
                                       ||
                                       (loggedOnMember.MemberType !=
                                        TeamMemberData.UserTypes.eProjectFacilitator)));
            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.Html(nVerseIndex, i, nNumTestQuestionCols,
                    bShowVernacular, bShowNationalBT, bShowEnglishBT);
            }

            /*
            // make a cell out of the testing question boxes
            string strHtmlCell = String.Format(Properties.Resources.HTML_TableCellWidth,
                                               100,
                                               strRow);

            // add combine with the 'tst:' header cell into a Table Row
            string strHtml = String.Format(Properties.Resources.HTML_TableRow,
                                           strHtmlCell);
            // add exegetical comments as their own rows
            for (int i = 0; i < Count; i++)
            {
                AnchorData anchorData = this[i];
                if (anchorData.ExegeticalHelpNotes.Count > 0)
                    strHtml += anchorData.ExegeticalHelpNotes.Html(nVerseIndex, i);
            }
            */

            // make a sub-table out of all this
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_TableNoBorder,
                                                                strRow)));
            return strRow;
        }
    }
}
