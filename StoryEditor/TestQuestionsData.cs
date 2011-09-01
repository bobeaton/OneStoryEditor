using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public class TestQuestionData
    {
        public string guid;
        public bool IsVisible = true;
        public LineData TestQuestionLine;
        public AnswersData Answers;

        public TestQuestionData(NewDataSet.TestQuestionRow theTestQuestionRow, NewDataSet projFile)
        {
            guid = theTestQuestionRow.guid;
            IsVisible = theTestQuestionRow.visible;

            TestQuestionLine = new LineData();
            foreach (NewDataSet.TestQuestionLineRow aTqLine in theTestQuestionRow.GetTestQuestionLineRows())
                TestQuestionLine.SetValue(aTqLine.lang,
                                          (aTqLine.IsTestQuestionLine_textNull())
                                              ? null
                                              : aTqLine.TestQuestionLine_text);

            Answers = new AnswersData(theTestQuestionRow, projFile);
        }

        public TestQuestionData(XmlNode node)
        {
            guid = node.Attributes[CstrAttributeGuid].Value;

            XmlAttribute attr;
            IsVisible = ((attr = node.Attributes[CstrAttributeVisible]) != null) ? (attr.Value == "true") : true;
            TestQuestionLine = new LineData(node, CstrElementLabelTestQuestionLine);
            Answers = new AnswersData(node.SelectSingleNode(AnswersData.CstrElementLableAnswers));
        }

        public TestQuestionData(TestQuestionData rhs)
        {
            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();   // rhs.guid;

            IsVisible = rhs.IsVisible;
            TestQuestionLine = new LineData(rhs.TestQuestionLine);
            Answers = new AnswersData(rhs.Answers);
        }

        public TestQuestionData()
        {
            guid = Guid.NewGuid().ToString();
            TestQuestionLine = new LineData();
            Answers = new AnswersData();
        }

        public bool HasData
        {
            get
            {
                return (TestQuestionLine.HasData
                    || Answers.HasData);
            }
        }

        public const string CstrElementLabelTestQuestion = "TestQuestion";
        public const string CstrAttributeGuid = "guid";
        public const string CstrAttributeVisible = "visible";
        public const string CstrElementLabelTestQuestionLine = "TestQuestionLine";

        public XElement GetXml
        {
            get
            {
                var eleTQ = new XElement(CstrElementLabelTestQuestion,
                    new XAttribute(CstrAttributeVisible, IsVisible),
                    new XAttribute(CstrAttributeGuid, guid));
                
                TestQuestionLine.AddXml(eleTQ, CstrElementLabelTestQuestionLine);
                eleTQ.Add(Answers.GetXml);

                return eleTQ;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (findProperties.TestQs)
            {
                TestQuestionLine.IndexSearch(findProperties,
                                             VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions,
                                             ref lstBoxesToSearch);
            }

            if (Answers.HasData && findProperties.TestAs)
                Answers.IndexSearch(findProperties, ref lstBoxesToSearch);
        }

        public const string CstrTestQuestionsLabelFormat = "tst {0}:";

        public static string TextareaId(int nVerseIndex, int nTQNum, string strTextElementName)
        {
            return String.Format("taTQ_{0}_{1}_{2}", nVerseIndex, nTQNum, strTextElementName);
        }

        public string Html(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            VerseData.ViewSettings viewItemToInsureOn, 
            bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT)
        {
            string strRow = String.Format(Properties.Resources.HTML_TableCell,
                                          String.Format(CstrTestQuestionsLabelFormat, nTQNum + 1));

            if (bShowVernacular)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, LineData.CstrAttributeLangVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      TestQuestionLine.Vernacular));
            }

            if (bShowNationalBT)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, LineData.CstrAttributeLangNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      TestQuestionLine.NationalBt));
            }

            if (bShowEnglishBT)
            {
                strRow += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                        String.Format(Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, nTQNum, LineData.CstrAttributeLangInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      TestQuestionLine.InternationalBt));
            }

            string strTQRow = String.Format(Properties.Resources.HTML_TableRow,
                                                   strRow);

            strTQRow += Answers.Html(nVerseIndex, nNumTestQuestionCols,
                viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersVernacular),
                viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersNationalBT),
                viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersInternationalBT));
            return strTQRow;
        }

        protected string PresentationHtmlCell(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            string strStyleClass, string strTextAreaSuffix, string str)
        {
            if (String.IsNullOrEmpty(str))
                str = "-";  // just so there's something there (or the cell doesn't show)
            return String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                 String.Format(Properties.Resources.HTML_ParagraphText,
                                               TextareaId(nVerseIndex, nTQNum, strTextAreaSuffix),
                                               strStyleClass,
                                               str));
        }

        public string PresentationHtml(int nVerseIndex, int nTQNum, int nNumTestQuestionCols, 
            VerseData.ViewSettings viewSettings, bool bShowVernacular, bool bShowNationalBt, 
            bool bShowEnglishBt, TestInfo astrTesters, TestQuestionsData child, 
            bool bPrintPreview, bool bProcessingTheChild, bool bIsFirstVerse)
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
            if ((!bIsFirstVerse && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions)) ||
                (bIsFirstVerse && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions)))
            {
                string strRow = String.Format(Properties.Resources.HTML_TableCell,
                                              String.Format(CstrTestQuestionsLabelFormat, nTQNum + 1));
                if (bShowVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.Vernacular, (theChildTQ != null) ? theChildTQ.TestQuestionLine.Vernacular : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.Vernacular)
                        : TestQuestionLine.Vernacular.GetValue(transliterator);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangVernacularStyleClassName, LineData.CstrAttributeLangVernacular, str);
                }

                if (bShowNationalBt)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.NationalBt, (theChildTQ != null) ? theChildTQ.TestQuestionLine.NationalBt : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.NationalBt)
                        : TestQuestionLine.NationalBt.GetValue(transliterator);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangNationalBtStyleClassName, LineData.CstrAttributeLangNationalBt, str);
                }

                if (bShowEnglishBt)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorInternationalBt;
                    string str = (!bPrintPreview)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.InternationalBt, (theChildTQ != null) ? theChildTQ.TestQuestionLine.InternationalBt : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.InternationalBt)
                        : TestQuestionLine.InternationalBt.GetValue(transliterator);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangInternationalBtStyleClassName, LineData.CstrAttributeLangInternationalBt, str);
                }

                strTQRow = String.Format(Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTQRow += Answers.PresentationHtml(nVerseIndex, nNumTestQuestionCols + 1, 
                    astrTesters, (theChildTQ != null) ? theChildTQ.Answers : null, bPrintPreview, 
                    bProcessingTheChild,
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersVernacular),
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersNationalBT),
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersInternationalBT));
            }

            return strTQRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            VerseData.ViewSettings viewSettings, bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT,
            TestInfo astrTesters)
        {
            string strTQRow = null;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
            {
                string strRow = String.Format(Properties.Resources.HTML_TableCell,
                                              String.Format(CstrTestQuestionsLabelFormat,
                                                            nTQNum + 1));
                if (bShowVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.Vernacular);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangVernacularStyleClassName, LineData.CstrAttributeLangVernacular, str);
                }

                if (bShowNationalBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.NationalBt);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangNationalBtStyleClassName, LineData.CstrAttributeLangNationalBt, str);
                }

                if (bShowEnglishBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorInternationalBt;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.InternationalBt);

                    strRow += PresentationHtmlCell(nVerseIndex, nTQNum, nNumTestQuestionCols,
                        StoryData.CstrLangInternationalBtStyleClassName, LineData.CstrAttributeLangInternationalBt, str);
                }

                strTQRow = String.Format(Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTQRow += Answers.PresentationHtmlAsAddition(nVerseIndex, nNumTestQuestionCols + 1, astrTesters,
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersVernacular),
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersNationalBT),
                    viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnswersInternationalBT));
            }

            return strTQRow;
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            Answers.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
        }
    }

    public class TestQuestionsData : List<TestQuestionData>
    {
        public TestQuestionsData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
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
                var elemTestQuestions = new XElement(CstrElementLabelTestQuestions);
                foreach (TestQuestionData aTestQuestionData in this)
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
            int nVerseIndex, int nNumCols)
        {
            int nNumTestQuestionCols = 0;
            bool bShowVernacular =
                viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.TestQuestionsVernacular);
            bool bShowNationalBT = viewItemToInsureOn.IsViewItemOn(
                                        VerseData.ViewSettings.ItemToInsureOn.TestQuestionsNationalBT);
            bool bShowEnglishBT = viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.TestQuestionsInternationalBT);

            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.Html(nVerseIndex, i, nNumTestQuestionCols,
                    viewItemToInsureOn, bShowVernacular, bShowNationalBT, bShowEnglishBT);
            }

            // make a sub-table out of all this
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtml(int nVerseIndex, int nNumCols, 
            VerseData.ViewSettings viewSettings, TestInfo astrTesters,
            TestQuestionsData child, bool bPrintPreview, bool bIsFirstVerse)
        {
            // return nothing if there's nothing to do
            if ((!HasData && ((child == null) || !child.HasData)))
                return null;

            // just get the column count from the first question (in case there are multiple)
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField)
                                   &&
                                   ((!bIsFirstVerse && viewSettings.IsViewItemOn(
                                       VerseData.ViewSettings.ItemToInsureOn.TestQuestionsVernacular)) ||
                                    (bIsFirstVerse && viewSettings.IsViewItemOn(
                                        VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions)));

            bool bShowNationalBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField)
                                   &&
                                   ((!bIsFirstVerse && viewSettings.IsViewItemOn(
                                       VerseData.ViewSettings.ItemToInsureOn.TestQuestionsNationalBT)) ||
                                    (bIsFirstVerse && viewSettings.IsViewItemOn(
                                        VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions)));

            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField)
                                  &&
                                  ((!bIsFirstVerse && viewSettings.IsViewItemOn(
                                      VerseData.ViewSettings.ItemToInsureOn.TestQuestionsInternationalBT)) ||
                                   (bIsFirstVerse && viewSettings.IsViewItemOn(
                                       VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions)));

            int nNumTestQuestionCols = 0;
            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.PresentationHtml(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                    bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTesters, child, bPrintPreview, false, bIsFirstVerse);
            }

            if (child != null)
                for (int i = 0; i < child.Count; i++)
                {
                    TestQuestionData testQuestionData = child[i];
                    strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                        bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTesters);
                }

            // make a sub-table out of all this
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols,
            VerseData.ViewSettings viewSettings, TestInfo astrTesters, bool bHasOutsideEnglishBTer)
        {
            // return nothing if there's nothing to do
            if (!HasData)
                return null;

            // just get the column count from the first question (in case there are multiple)
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            bool bShowNationalBT = bHasOutsideEnglishBTer && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField);
            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField);

            int nNumTestQuestionCols = 0;
            if (bShowVernacular) nNumTestQuestionCols++;
            if (bShowNationalBT) nNumTestQuestionCols++;
            if (bShowEnglishBT) nNumTestQuestionCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                TestQuestionData testQuestionData = this[i];
                strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex, i, nNumTestQuestionCols, viewSettings,
                    bShowVernacular, bShowNationalBT, bShowEnglishBT, astrTesters);
            }

            // make a sub-table out of all this
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            foreach (var aTq in this)
                aTq.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
        }

        public bool DoesReferenceTqUns(string strMemberId)
        {
            return this.Any(testQuestion =>
                            testQuestion.Answers.DoesReferenceTqUns(strMemberId));

        }

        public void RemoveTestQuestionAnswers(string strUnsGuid)
        {
            foreach (var aTq in this)
                aTq.Answers.RemoveTestResult(strUnsGuid);
        }
    }
}
