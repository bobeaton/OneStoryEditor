using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NetLoc;
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

            TestQuestionLine = new LineData(StoryEditor.TextFields.TestQuestion);
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
            TestQuestionLine = new LineData(node, CstrElementLabelTestQuestionLine, StoryEditor.TextFields.TestQuestion);
            Answers = new AnswersData(node.SelectSingleNode(AnswersData.CstrElementLableAnswers));
        }

        public TestQuestionData(TestQuestionData rhs)
        {
            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();   // rhs.guid;

            IsVisible = rhs.IsVisible;
            TestQuestionLine = new LineData(rhs.TestQuestionLine, StoryEditor.TextFields.TestQuestion);
            Answers = new AnswersData(rhs.Answers);
        }

        public TestQuestionData()
        {
            guid = Guid.NewGuid().ToString();
            TestQuestionLine = new LineData(StoryEditor.TextFields.TestQuestion);
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

        public static string TestQuestionsLabelFormat
        {
            get { return Localizer.Str("tst {0}:"); }
        }

        public static string TextareaId(int nVerseIndex, int nTQNum, string strTextElementName)
        {
            return String.Format("taTQ_{0}_{1}_{2}", nVerseIndex, nTQNum, strTextElementName);
        }

        /*
        public string Html(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            VerseData.ViewSettings viewItemToInsureOn, 
            bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT)
        {
            string strRow = String.Format(Properties.Resources.HTML_TableCell,
                                          String.Format(TestQuestionsLabelFormat, nTQNum + 1));

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
        
        protected string PresentationHtmlCell(int nVerseIndex, int nTQNum, 
            int nNumTestQuestionCols,
            string strStyleClass, 
            string strTextAreaSuffix, 
            string str, 
            bool bUseTextAreas)
        {
            if (String.IsNullOrEmpty(str))
                str = "-";  // just so there's something there (or the cell doesn't show)
            return String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                 String.Format(Properties.Resources.HTML_ParagraphTextId,
                                               TextareaId(nVerseIndex, nTQNum, strTextAreaSuffix),
                                               strStyleClass,
                                               str));
        }
        */

        public string PresentationHtml(int nVerseIndex, int nTQNum, 
            int nNumTestQuestionCols, 
            VerseData.ViewSettings viewSettings, 
            bool bShowVernacular, bool bShowNationalBt, bool bShowEnglishBt, 
            TestInfo astrTesters, TestQuestionsData child,
            StoryData.PresentationType presentationType, bool bProcessingTheChild, bool bIsFirstVerse)
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

            var bShowLangVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            var bShowLangNationalBt = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField);
            var bShowLangEnglishBt = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField);

            string strTqRow = null;
            if ((!bIsFirstVerse && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions)) ||
                (bIsFirstVerse && viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions)))
            {
                string strRow = String.Format(Properties.Resources.HTML_TableCellNoWrap,
                                              String.Format(TestQuestionsLabelFormat, nTQNum + 1));

                // whether we show the field is based both on whether it's configured in project settings, but also whether the
                //  language field (above) is checked.
                var bShowTqsVernacular = bShowLangVernacular && bShowVernacular;
                var bShowTqsNationalBt = bShowLangNationalBt && bShowNationalBt;
                var bShowTqsEnglishBt = bShowLangEnglishBt && bShowEnglishBt;

                // but if none of them are still on, that means we should at least warn the user...
                if (!bShowTqsVernacular && !bShowTqsNationalBt && !bShowTqsEnglishBt)
                    StoryEditor.WarnAboutNoLangsVisible(Localizer.Str("Testing Questions"));

                if (bShowTqsVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = (presentationType == StoryData.PresentationType.Differencing)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.Vernacular, (theChildTQ != null) ? theChildTQ.TestQuestionLine.Vernacular : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.Vernacular)
                        : TestQuestionLine.Vernacular.GetValue(transliterator);

                    strRow += TestQuestionLine.Vernacular.FormatLanguageColumnHtml(nVerseIndex,
                                                                                   nTQNum,
                                                                                   nNumTestQuestionCols,
                                                                                   str,
                                                                                   viewSettings);
                }

                if (bShowTqsNationalBt)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = (presentationType == StoryData.PresentationType.Differencing)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.NationalBt, (theChildTQ != null) ? theChildTQ.TestQuestionLine.NationalBt : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.NationalBt)
                        : TestQuestionLine.NationalBt.GetValue(transliterator);

                    strRow += TestQuestionLine.NationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                                   nTQNum,
                                                                                   nNumTestQuestionCols,
                                                                                   str,
                                                                                   viewSettings);
                }

                if (bShowTqsEnglishBt)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorInternationalBt;
                    string str = (presentationType == StoryData.PresentationType.Differencing)
                        ? (child != null)
                            ? Diff.HtmlDiff(transliterator, TestQuestionLine.InternationalBt, (theChildTQ != null) ? theChildTQ.TestQuestionLine.InternationalBt : null)
                            : Diff.HtmlDiff(transliterator, null, TestQuestionLine.InternationalBt)
                        : TestQuestionLine.InternationalBt.GetValue(transliterator);

                    strRow += TestQuestionLine.InternationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                                        nTQNum,
                                                                                        nNumTestQuestionCols,
                                                                                        str,
                                                                                        viewSettings);
                }

                strTqRow = String.Format(Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // whether we show answers or not depends on whether we're showing the language (e.g. bShowLangVernacular) 
                //  AND whether it's configured to be shown (e.g. IsViewItemOn...)
                var bShowAnswersVernacular = bShowLangVernacular &&
                                             viewSettings.IsViewItemOn(
                                                 VerseData.ViewSettings.ItemToInsureOn.AnswersVernacular);
                var bShowAnswersNationalBt = bShowLangNationalBt &&
                                             viewSettings.IsViewItemOn(
                                                 VerseData.ViewSettings.ItemToInsureOn.AnswersNationalBT);
                var bShowAnswersEnglishBt = bShowLangEnglishBt &&
                                            viewSettings.IsViewItemOn(
                                                VerseData.ViewSettings.ItemToInsureOn.AnswersInternationalBT);

                // but if none of them are still on, that means we should at least warn the user...
                if (!bShowAnswersVernacular && !bShowAnswersNationalBt && !bShowAnswersEnglishBt)
                    StoryEditor.WarnAboutNoLangsVisible(Localizer.Str("Answers"));

                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTqRow += Answers.PresentationHtml(nVerseIndex,
                                                     nNumTestQuestionCols + 1,
                                                     nTQNum,
                                                     astrTesters,
                                                     (theChildTQ != null) ? theChildTQ.Answers : null,
                                                     presentationType,
                                                     bProcessingTheChild,
                                                     bShowAnswersVernacular,
                                                     bShowAnswersNationalBt,
                                                     bShowAnswersEnglishBt,
                                                     viewSettings);
            }

            return strTqRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nTQNum, int nNumTestQuestionCols,
            VerseData.ViewSettings viewSettings, bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT,
            TestInfo astrTesters)
        {
            string strTQRow = null;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
            {
                string strRow = String.Format(Properties.Resources.HTML_TableCellNoWrap,
                                              String.Format(TestQuestionsLabelFormat,
                                                            nTQNum + 1));
                if (bShowVernacular)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.Vernacular);

                    strRow += TestQuestionLine.Vernacular.FormatLanguageColumnHtml(nVerseIndex,
                                                                                   nTQNum,
                                                                                   nNumTestQuestionCols,
                                                                                   str,
                                                                                   viewSettings);
                }

                if (bShowNationalBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.NationalBt);

                    strRow += TestQuestionLine.NationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                                   nTQNum,
                                                                                   nNumTestQuestionCols,
                                                                                   str,
                                                                                   viewSettings);
                }

                if (bShowEnglishBT)
                {
                    DirectableEncConverter transliterator = viewSettings.TransliteratorInternationalBt;
                    string str = Diff.HtmlDiff(transliterator, null, TestQuestionLine.InternationalBt);

                    strRow += TestQuestionLine.InternationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                                        nTQNum,
                                                                                        nNumTestQuestionCols,
                                                                                        str,
                                                                                        viewSettings);
                }

                strTQRow = String.Format(Properties.Resources.HTML_TableRow,
                                                       strRow);
            }

            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                // add 1 to the number of columns so it spans properly (including the 'tst:' label)
                strTQRow += Answers.PresentationHtmlAsAddition(nVerseIndex, nNumTestQuestionCols + 1, nTQNum, astrTesters,
                                                               viewSettings.IsViewItemOn(
                                                                   VerseData.ViewSettings.ItemToInsureOn.
                                                                       AnswersVernacular),
                                                               viewSettings.IsViewItemOn(
                                                                   VerseData.ViewSettings.ItemToInsureOn.
                                                                       AnswersNationalBT),
                                                               viewSettings.IsViewItemOn(
                                                                   VerseData.ViewSettings.ItemToInsureOn.
                                                                       AnswersInternationalBT),
                                                               viewSettings);
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

        /*
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
        */

        public string PresentationHtml(int nVerseIndex, int nNumCols, 
            VerseData.ViewSettings viewSettings, TestInfo astrTesters,
            TestQuestionsData child, StoryData.PresentationType presentationType, bool bIsFirstVerse)
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
                strRow += testQuestionData.PresentationHtml(nVerseIndex, i,
                                                            nNumTestQuestionCols, viewSettings,
                                                            bShowVernacular, bShowNationalBT, bShowEnglishBT,
                                                            astrTesters, child, presentationType, false, bIsFirstVerse);
            }

            if (child != null)
                for (int i = 0; i < child.Count; i++)
                {
                    TestQuestionData testQuestionData = child[i];
                    strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex, i,
                                                                          nNumTestQuestionCols, viewSettings,
                                                                          bShowVernacular, bShowNationalBT,
                                                                          bShowEnglishBT,
                                                                          astrTesters);
                }

            // make a sub-table out of all this
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols,
            VerseData.ViewSettings viewSettings, TestInfo astrTesters, 
            bool bHasOutsideEnglishBTer)
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
                strRow += testQuestionData.PresentationHtmlAsAddition(nVerseIndex,
                                                                      i,
                                                                      nNumTestQuestionCols,
                                                                      viewSettings,
                                                                      bShowVernacular,
                                                                      bShowNationalBT,
                                                                      bShowEnglishBT,
                                                                      astrTesters);
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
