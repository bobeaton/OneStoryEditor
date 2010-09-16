using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using SilEncConverters31;

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

        public TestQuestionData(XmlNode node)
        {
            guid = node.Attributes[CstrAttributeGuid].Value;

            XmlAttribute attr;
            IsVisible = ((attr = node.Attributes[CstrAttributeVisible]) != null) ? (attr.Value == "true") : true;

            XmlNode elem;
            QuestionVernacular = new StringTransfer(((elem = node.SelectSingleNode(CstrElementLabelTQVernacular)) != null) ? elem.InnerText : null);
            QuestionNationalBT = new StringTransfer(((elem = node.SelectSingleNode(CstrElementLabelTQNationalBT)) != null) ? elem.InnerText : null);
            QuestionInternationalBT = new StringTransfer(((elem = node.SelectSingleNode(CstrElementLabelTQInternationalBT)) != null) ? elem.InnerText : null);
            Answers = new AnswersData(node.SelectSingleNode(AnswersData.CstrElementLableAnswers));
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

        public const string CstrElementLabelTestQuestion = "TestQuestion";
        public const string CstrAttributeGuid = "guid";
        public const string CstrAttributeVisible = "visible";
        public const string CstrElementLabelTQVernacular = "TQVernacular";
        public const string CstrElementLabelTQNationalBT = "TQNationalBT";
        public const string CstrElementLabelTQInternationalBT = "TQInternationalBT";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(QuestionVernacular.HasData
                    || QuestionNationalBT.HasData
                    || QuestionInternationalBT.HasData
                    || Answers.HasData, "you have an empty TestQuestionData");

                XElement eleTQ = new XElement(CstrElementLabelTestQuestion,
                    new XAttribute(CstrAttributeVisible, IsVisible),
                    new XAttribute(CstrAttributeGuid, guid));
                
                if (QuestionVernacular.HasData)
                    eleTQ.Add(new XElement(CstrElementLabelTQVernacular, QuestionVernacular));
                if (QuestionNationalBT.HasData)
                    eleTQ.Add(new XElement(CstrElementLabelTQNationalBT, QuestionNationalBT));
                if (QuestionInternationalBT.HasData)
                    eleTQ.Add(new XElement(CstrElementLabelTQInternationalBT, QuestionInternationalBT));
                if (Answers.HasData)
                    eleTQ.Add(Answers.GetXml);

                return eleTQ;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (findProperties.TestQs)
            {
                if (QuestionVernacular.HasData && findProperties.StoryLanguage)
                    lstBoxesToSearch.AddNewVerseString(QuestionVernacular,
                        VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions |
                        VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
                if (QuestionNationalBT.HasData && findProperties.NationalBT)
                    lstBoxesToSearch.AddNewVerseString(QuestionNationalBT,
                        VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions |
                        VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
                if (QuestionInternationalBT.HasData && findProperties.EnglishBT)
                    lstBoxesToSearch.AddNewVerseString(QuestionInternationalBT,
                        VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions |
                        VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);
            }

            if (Answers.HasData && findProperties.TestAs)
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
            string strRow = String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                          CstrTestQuestionsLabelFormat);
            if (bShowVernacular)
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, VerseData.CstrFieldNameVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      QuestionVernacular));
            }

            if (bShowNationalBT)
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, VerseData.CstrFieldNameNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      QuestionNationalBT));
            }

            if (bShowEnglishBT)
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, VerseData.CstrFieldNameInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      QuestionInternationalBT));
            }

            string strTQRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                   strRow);

            strTQRow += Answers.Html(nVerseIndex, nNumTestQuestionCols);
            return strTQRow;
        }

        protected string PresentationHtmlCell(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            string strStyleClass, string str)
        {
            if (String.IsNullOrEmpty(str))
                str = "-";  // just so there's something there (or the cell doesn't show)
            return String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                 String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                               TextareaId(nVerseIndex, nTQNum, VerseData.CstrFieldNameInternationalBt),
                                               strStyleClass,
                                               str));
        }

        public string PresentationHtml(int nVerseIndex, int nTQNum, int nNumTestQuestionCols, 
            VerseData.ViewSettings viewSettings, bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT,
            List<string> astrTestors, TestQuestionsData child, bool bPrintPreview, bool bProcessingTheChild)
        {
            TestQuestionData theChildTQ = null;
            if (child != null)
                foreach (TestQuestionData aTQ in child)
                    if (aTQ.guid == guid)
                    {
                        theChildTQ = aTQ;
                        child.Remove(aTQ);
                        break;
                    }

            string strTQRow = null;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
            {
                string strRow = String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                              CstrTestQuestionsLabelFormat);
                if (bShowVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, QuestionVernacular, (theChildTQ != null) ? theChildTQ.QuestionVernacular : null)
                            : Diff.HtmlDiff(transliterator, null, QuestionVernacular)
                        : QuestionVernacular.GetValue(transliterator);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangVernacularStyleClassName, str);
                }

                if (bShowNationalBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, QuestionNationalBT, (theChildTQ != null) ? theChildTQ.QuestionNationalBT : null)
                            : Diff.HtmlDiff(transliterator, null, QuestionNationalBT)
                        : QuestionNationalBT.GetValue(transliterator);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangNationalBtStyleClassName, str);
                }

                if (bShowEnglishBT)
                {
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(QuestionInternationalBT, (theChildTQ != null) ? theChildTQ.QuestionInternationalBT : null)
                            : Diff.HtmlDiff(null, QuestionInternationalBT)
                        : QuestionInternationalBT.ToString();

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangInternationalBtStyleClassName, str);
                }

                strTQRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTQRow += Answers.PresentationHtml(nVerseIndex, nNumTestQuestionCols + 1, astrTestors,
                    (theChildTQ != null) ? theChildTQ.Answers : null, bPrintPreview, bProcessingTheChild);
            }

            return strTQRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            VerseData.ViewSettings viewSettings, bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT,
            List<string> astrTestors)
        {
            string strTQRow = null;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
            {
                string strRow = String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                              CstrTestQuestionsLabelFormat);
                if (bShowVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = Diff.HtmlDiff(transliterator, null, QuestionVernacular);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangVernacularStyleClassName, str);
                }

                if (bShowNationalBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = Diff.HtmlDiff(transliterator, null, QuestionNationalBT);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangNationalBtStyleClassName, str);
                }

                if (bShowEnglishBT)
                {
                    string str = Diff.HtmlDiff(null, QuestionInternationalBT);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangInternationalBtStyleClassName, str);
                }

                strTQRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTQRow += Answers.PresentationHtmlAsAddition(nVerseIndex, nNumTestQuestionCols + 1, astrTestors);
            }

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

        public TestQuestionsData(XmlNode node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(TestQuestionData.CstrElementLabelTestQuestion);
            if (list == null)
                return;

            foreach (XmlNode nodeTQ in list)
                Add(new TestQuestionData(nodeTQ));
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
            get { return (Count > 0); }
        }

        public const string CstrElementLabelTestQuestions = "TestQuestions";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an empty TestQuestionsData");
                XElement elemTestQuestions = new XElement(CstrElementLabelTestQuestions);
                foreach (TestQuestionData aTestQuestionData in this)
                    if (aTestQuestionData.HasData)
                        elemTestQuestions.Add(aTestQuestionData.GetXml);
                return elemTestQuestions;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (TestQuestionData testQuestionData in this)
                testQuestionData.IndexSearch(findProperties, lstBoxesToSearch);
        }

        public string Html(ProjectSettings projectSettings, 
            VerseData.ViewSettings viewItemToInsureOn, 
            StoryStageLogic stageLogic, TeamMemberData loggedOnMember,
            int nVerseIndex, int nNumCols, bool bHasOutsideEnglishBTer)
        {
            int nNumTestQuestionCols = 0;
            bool bShowVernacular =
                (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField));
            bool bShowNationalBT = (projectSettings.NationalBT.HasData
                                    && bHasOutsideEnglishBTer
                                    &&
                                    viewItemToInsureOn.IsViewItemOn(
                                        VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField));
            bool bShowEnglishBT = (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField)
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

            // make a sub-table out of all this
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtml(int nVerseIndex, int nNumCols, VerseData.ViewSettings viewSettings,
            List<string> astrTestors, TestQuestionsData child, bool bPrintPreview, bool bHasOutsideEnglishBTer)
        {
            // return nothing if there's nothing to do
            if ((!HasData && ((child == null) || !child.HasData)))
                return null;

            // just get the column count from the first question (in case there are multiple)
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            bool bShowNationalBT = bHasOutsideEnglishBTer && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);

            int nNumTestQuestionCols = 0;
            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.PresentationHtml(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                    bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTestors, child, bPrintPreview, false);
            }

            if (child != null)
                for (int i = 0; i < child.Count; i++)
                {
                    TestQuestionData testQuestionData = child[i];
                    strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                        bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTestors);
                }

            // make a sub-table out of all this
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols,
            VerseData.ViewSettings viewSettings, List<string> astrTestors, bool bHasOutsideEnglishBTer)
        {
            // return nothing if there's nothing to do
            if (!HasData)
                return null;

            // just get the column count from the first question (in case there are multiple)
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            bool bShowNationalBT = bHasOutsideEnglishBTer && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);

            int nNumTestQuestionCols = 0;
            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                    bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTestors);
            }

            // make a sub-table out of all this
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }
    }
}
