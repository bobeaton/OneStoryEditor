using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
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

        public VerseData(XmlNode node)
        {
            XmlAttribute attr;
            guid = ((attr = node.Attributes[CstrAttributeGuid]) != null) ? attr.Value : null;   // can't really happen
            IsFirstVerse = ((attr = node.Attributes[CstrAttributeFirstVerse]) != null) ? (attr.Value == "true") : false;

            XmlNode elem;
            VernacularText = new StringTransfer(((elem = node.SelectSingleNode(CstrFieldNameVernacular)) != null) ? elem.InnerText : null);
            NationalBTText = new StringTransfer(((elem = node.SelectSingleNode(CstrFieldNameNationalBt)) != null) ? elem.InnerText : null);
            InternationalBTText = new StringTransfer(((elem = node.SelectSingleNode(CstrFieldNameInternationalBt)) != null) ? elem.InnerText : null);
            Anchors = new AnchorsData(node.SelectSingleNode(AnchorsData.CstrElementLabelAnchors));
            TestQuestions = new TestQuestionsData(node.SelectSingleNode(TestQuestionsData.CstrElementLabelTestQuestions));
            Retellings = new RetellingsData(node.SelectSingleNode(RetellingsData.CstrElementLableRetellings));
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

        public class SearchLookInProperties
        {
            public bool ViewLookInOptions { get; set; }
            public bool StoryLanguage { get; set; }
            public bool NationalBT { get; set; }
            public bool EnglishBT { get; set; }
            public bool ConsultantNotes { get; set; }
            public bool CoachNotes { get; set; }
            public bool Retellings { get; set; }
            public bool TestQnA { get; set; }
            public bool SearchAll { get; set; }
            public bool UseRegex { get; set; }
        }

        public class VerseString
        {
            public StringTransfer StringTransfer { get; set; }
            public ViewItemToInsureOn ViewToInsureIsOn { get; set; }

            public VerseString(StringTransfer strStringTransfer,
                ViewItemToInsureOn viewItemToInsureOn)
            {
                StringTransfer = strStringTransfer;
                ViewToInsureIsOn = viewItemToInsureOn;
            }
        }

        public class StringTransferSearchIndex : List<VerseString>
        {
            public string StoryName { get; set; }

            public StringTransferSearchIndex(string strStoryName)
            {
                StoryName = strStoryName;
            }

            public VerseString AddNewVerseString(StringTransfer strStringTransfer,
                ViewItemToInsureOn viewItemToInsureOn)
            {
                var vs = new VerseString(strStringTransfer, viewItemToInsureOn);
                Add(vs);
                return vs;
            }
        }

        public class StorySearchIndex : List<StringTransferSearchIndex>
        {
            public StringTransferSearchIndex GetNewStorySearchIndex(string strStoryName)
            {
                var stsi = new StringTransferSearchIndex(strStoryName);
                Add(stsi);
                return stsi;
            }

            public StringTransferSearchIndex this[string strStoryName]
            {
                get
                {
                    foreach (StringTransferSearchIndex stsi in this)
                        if (stsi.StoryName == strStoryName)
                            return stsi;

                    System.Diagnostics.Debug.Assert(false);
                    return null;
                }
            }
        }

        public void IndexSearch(SearchLookInProperties findProperties,
            ref StringTransferSearchIndex lstBoxesToSearch)
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

        public const string CstrFieldNameVernacular = "Vernacular";
        public const string CstrFieldNameNationalBt = "NationalBT";
        public const string CstrFieldNameInternationalBt = "InternationalBT";

        public const string CstrAttributeGuid = "guid";
        public const string CstrElementLabelVerse = "verse";
        public const string CstrAttributeFirstVerse = "first";
        public const string CstrAttributeVisible = "visible";

        public XElement GetXml
        {
            get
            {
                XElement elemVerse = new XElement(CstrElementLabelVerse,
                    new XAttribute(CstrAttributeGuid, guid));
                
                // only need to write out the 'first' attribute if it's true
                if (IsFirstVerse)
                    elemVerse.Add(new XAttribute(CstrAttributeFirstVerse, IsFirstVerse));

                // only need to write out the 'visible' attribute if it's false
                if (!IsVisible)
                    elemVerse.Add(new XAttribute(CstrAttributeVisible, IsVisible));
                
                if (VernacularText.HasData)
                    elemVerse.Add(new XElement(CstrFieldNameVernacular, VernacularText));
                if (NationalBTText.HasData)
                    elemVerse.Add(new XElement(CstrFieldNameNationalBt, NationalBTText));
                if (InternationalBTText.HasData)
                    elemVerse.Add(new XElement(CstrFieldNameInternationalBt, InternationalBTText));
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
                items |= ViewItemToInsureOn.eVernacularLangField;
            if (bLangNationalBT)
                items |= ViewItemToInsureOn.eNationalLangField;
            if (bLangInternationalBT)
                items |= ViewItemToInsureOn.eEnglishBTField;
            if (bAnchors)
                items |= ViewItemToInsureOn.eAnchorFields;
            if (bStoryTestingQuestions)
                items |= ViewItemToInsureOn.eStoryTestingQuestionFields;
            if (bRetellings)
                items |= ViewItemToInsureOn.eRetellingFields;
            if (bConsultantNotes)
                items |= ViewItemToInsureOn.eConsultantNoteFields;
            if (bCoachNotes)
                items |= ViewItemToInsureOn.eCoachNotesFields;
            if (bBibleViewer)
                items |= ViewItemToInsureOn.eBibleViewer;
            return items;
        }

        public void AllowConNoteButtonsOverride()
        {
            ConsultantNotes.AllowButtonsOverride();
            CoachNotes.AllowButtonsOverride();
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

        public static string TextareaId(int nVerseIndex, string strTextElementName)
        {
            return String.Format("ta_{0}_{1}", nVerseIndex, strTextElementName);
        }

        public static string HtmlColor(Color clrRow)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}",
                                 clrRow.R, clrRow.G, clrRow.B);
        }

        public string StoryBtHtml(ProjectSettings projectSettings, TeamMembersData membersData,
            StoryStageLogic stageLogic, TeamMemberData loggedOnMember, int nVerseIndex, 
            ViewItemToInsureOn viewItemToInsureOn, int nNumCols)
        {
            string strRow = null;
            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eVernacularLangField))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, CstrFieldNameVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      VernacularText));
            }

            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eNationalLangField))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, CstrFieldNameNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      NationalBTText));
            }

            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eEnglishBTField))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                      TextareaId(nVerseIndex, CstrFieldNameInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      InternationalBTText));
            }

            string strStoryLineRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                   strRow);

            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eAnchorFields))
                strStoryLineRow += Anchors.Html(nVerseIndex, nNumCols);

            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eRetellingFields)
                && (Retellings.Count > 0))
                strStoryLineRow += Retellings.Html(nVerseIndex, nNumCols);

            if (IsViewItemOn(viewItemToInsureOn, ViewItemToInsureOn.eStoryTestingQuestionFields)
                && (TestQuestions.Count > 0))
                strStoryLineRow += TestQuestions.Html(projectSettings, viewItemToInsureOn,
                    stageLogic, loggedOnMember, nVerseIndex, nNumCols, 
                    membersData.HasOutsideEnglishBTer);

            return strStoryLineRow;
        }

        // Html that shows the data in the StoryBt file, but in a fully read-only manner
        public string PresentationHtml(int nVerseIndex, int nNumCols, CraftingInfoData craftingInfo,
            ViewItemToInsureOn viewSettings, VersesData child)
        {
            VerseData theChildVerse = null;
            if (child != null)
                foreach (VerseData aVerse in child)
                    if (aVerse.guid == guid)
                    {
                        theChildVerse = aVerse;
                        break;
                    }

            string strRow = null;
            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eVernacularLangField))
            {
                string str = (theChildVerse != null) 
                    ? Diff.HtmlDiff(VernacularText, theChildVerse.VernacularText) 
                    : VernacularText.ToString();

                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(nVerseIndex, CstrFieldNameVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      str));
            }

            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eNationalLangField))
            {
                string str = (theChildVerse != null)
                    ? Diff.HtmlDiff(NationalBTText, theChildVerse.NationalBTText)
                    : NationalBTText.ToString();

                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(nVerseIndex, CstrFieldNameNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      str));
            }

            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eEnglishBTField))
            {
                string str = (theChildVerse != null)
                    ? Diff.HtmlDiff(InternationalBTText, theChildVerse.InternationalBTText)
                    : InternationalBTText.ToString();

                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(nVerseIndex, CstrFieldNameInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      str));
            }

            string strStoryLineRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                   strRow);

            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eAnchorFields) && (Anchors.Count > 0))
                strStoryLineRow += Anchors.PresentationHtml(nVerseIndex, nNumCols, 
                    (theChildVerse != null) ? theChildVerse.Anchors : null);

            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eRetellingFields) && (Retellings.Count > 0))
                strStoryLineRow += Retellings.PresentationHtml(nVerseIndex, nNumCols,
                    craftingInfo.Testors, (theChildVerse != null) ? theChildVerse.Retellings : null);

            if (IsViewItemOn(viewSettings, ViewItemToInsureOn.eStoryTestingQuestionFields) && (TestQuestions.Count > 0))
                strStoryLineRow += TestQuestions.PresentationHtml(nVerseIndex, nNumCols,
                    craftingInfo.Testors, (theChildVerse != null) ? theChildVerse.TestQuestions : null);

            return strStoryLineRow;
        }
    }

    public class VersesData : List<VerseData>
    {
        internal const string CstrZerothLineName = "Story:";
        internal const string CstrHiddenVerseSuffix = " (Hidden)";

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

        public VersesData(XmlNode node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(VerseData.CstrElementLabelVerse);
            if (list == null)
                return;

            foreach (XmlNode nodeVerse in list)
                Add(new VerseData(nodeVerse));

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

        protected int CalculateColumns(VerseData.ViewItemToInsureOn viewItemToInsureOn)
        {
            int nColSpan = 0;
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eVernacularLangField))
                nColSpan++;
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eNationalLangField))
                nColSpan++;
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eEnglishBTField))
                nColSpan++;
            return nColSpan;
        }

        public string StoryBtHtml(ProjectSettings projectSettings, bool bViewHidden,
            StoryStageLogic stageLogic, TeamMembersData membersData, TeamMemberData loggedOnMember,
            VerseData.ViewItemToInsureOn viewItemToInsureOn)
        {
            int nColSpan = CalculateColumns(viewItemToInsureOn);

            // add a row indicating which languages are in what columns
            string strHtml = null;
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eVernacularLangField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.Vernacular.LangName);
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eNationalLangField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.NationalBT.LangName);
            if (VerseData.IsViewItemOn(viewItemToInsureOn, VerseData.ViewItemToInsureOn.eEnglishBTField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.InternationalBT.LangName);

            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                     strHtml);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (aVerseData.IsVisible || bViewHidden)
                {
                    strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.IsVisible, true, nColSpan);

                    strHtml += aVerseData.StoryBtHtml(projectSettings, membersData, 
                        stageLogic, loggedOnMember, i, viewItemToInsureOn, nColSpan);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        public string PresentationHtml(CraftingInfoData craftingInfo, VersesData child, int nNumCols,
            VerseData.ViewItemToInsureOn viewSettings)
        {
            string strHtml = null;
            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (aVerseData.IsVisible && !aVerseData.IsFirstVerse)
                {
                    strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.IsVisible, false, nNumCols);

                    strHtml += aVerseData.PresentationHtml(i, nNumCols, craftingInfo,
                        viewSettings, child);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        public static string ButtonId(int nVerseIndex)
        {
            return String.Format("btnLn_{0}", nVerseIndex);
        }

        protected string GetHeaderRow(string strHeader, int nVerseIndex, bool bVerseVisible, bool bShowButton, int nColSpan)
        {
            string strLink = String.Format(OseResources.Properties.Resources.HTML_LinkJumpLine,
                                           nVerseIndex, strHeader);
            if (!bVerseVisible)
                strLink += CstrHiddenVerseSuffix;

            string strButton = null;
            if (bShowButton)
                strButton = String.Format(OseResources.Properties.Resources.HTML_ButtonRightAlignCtxMenu,
                                          nVerseIndex, // ButtonId(nVerseIndex),
                                          " ");

            string strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRowColor, "#AACCFF",
                                           String.Format(OseResources.Properties.Resources.HTML_TableCellWidthSpanId,
                                                         LineId(nVerseIndex),
                                                         100,
                                                         nColSpan,
                                                         strLink + strButton));

            return strHtml;
        }

        public static string LineId(int nVerseIndex)
        {
            return String.Format("ln_{0}", nVerseIndex);
        }

        protected string GetHeaderRow(string strHeader, int nVerseIndex, bool bVerseVisible,
            ConsultNotesDataConverter theCNsDC, TeamMemberData LoggedOnMember)
        {
            string strHtmlAddNoteButton = null;
            if (theCNsDC.HasAddNotePrivilege(LoggedOnMember.MemberType))
                strHtmlAddNoteButton = String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                                     String.Format(OseResources.Properties.Resources.HTML_Button,
                                                                   nVerseIndex,
                                                                   "return window.external.OnAddNote(this.id, null);",
                                                                   "Add Note"));

            string strLink = String.Format(OseResources.Properties.Resources.HTML_LinkJumpLine,
                                           nVerseIndex, strHeader);
            if (!bVerseVisible)
                strLink += CstrHiddenVerseSuffix;
            return String.Format(OseResources.Properties.Resources.HTML_TableRowColor, "#AACCFF",
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellWidthId,
                                                             LineId(nVerseIndex),
                                                             100,
                                                             strLink),
                                               strHtmlAddNoteButton));
        }

        public string ConsultantNotesHtml(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, 
            bool bViewHidden)
        {
            string strHtml = null;
            strHtml += GetHeaderRow(CstrZerothLineName, 0, FirstVerse.IsVisible, 
                FirstVerse.ConsultantNotes, LoggedOnMember);

            strHtml += FirstVerse.ConsultantNotes.Html(htmlConNoteCtrl, theStoryStage, 
                LoggedOnMember, bViewHidden, FirstVerse.IsVisible, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (aVerseData.IsVisible || bViewHidden)
                {
                    strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.IsVisible, 
                        aVerseData.ConsultantNotes, LoggedOnMember);

                    strHtml += aVerseData.ConsultantNotes.Html(htmlConNoteCtrl, 
                        theStoryStage, LoggedOnMember, bViewHidden, aVerseData.IsVisible, i);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        public string CoachNotesHtml(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, bool bViewHidden)
        {
            string strHtml = null;
            strHtml += GetHeaderRow(CstrZerothLineName, 0, FirstVerse.IsVisible,
                FirstVerse.CoachNotes, LoggedOnMember);

            strHtml += FirstVerse.CoachNotes.Html(htmlConNoteCtrl, theStoryStage, 
                LoggedOnMember, bViewHidden, FirstVerse.IsVisible, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (aVerseData.IsVisible || bViewHidden)
                {
                    strHtml += GetHeaderRow("Ln: " + i, i, aVerseData.IsVisible,
                        aVerseData.CoachNotes, LoggedOnMember);

                    strHtml += aVerseData.CoachNotes.Html(htmlConNoteCtrl, theStoryStage, 
                        LoggedOnMember, bViewHidden, aVerseData.IsVisible, i);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
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
