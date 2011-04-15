using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    // class encapsulating one retelling or TQ answer possibly in multiple languages
    //  the List-ness of this is that there may be 3 StringTransfers for each of
    //  the StoryEditor.TextFieldType types
    public class LineData
    {
        public StringTransfer Vernacular;
        public StringTransfer NationalBt;
        public StringTransfer InternationalBt;
        public StringTransfer FreeTranslation;

        public const string CstrAttributeLang = "lang";

        public const string CstrAttributeLangVernacular = "Vernacular";
        public const string CstrAttributeLangNationalBt = "NationalBt";
        public const string CstrAttributeLangInternationalBt = "InternationalBt";
        public const string CstrAttributeLangFreeTranslation = "FreeTranslation";

        public void SetValue(string strLangAttribute, string strValue)
        {
            if (strLangAttribute == CstrAttributeLangVernacular)
                Vernacular.SetValue(strValue);
            if (strLangAttribute == CstrAttributeLangNationalBt)
                NationalBt.SetValue(strValue);
            if (strLangAttribute == CstrAttributeLangInternationalBt)
                InternationalBt.SetValue(strValue);
            if (strLangAttribute == CstrAttributeLangFreeTranslation)
                FreeTranslation.SetValue(strValue);
        }

        public bool HasData
        {
            get
            {
                return InternationalBt.HasData
                       || NationalBt.HasData
                       || Vernacular.HasData
                       || FreeTranslation.HasData;
            }
        }

        public LineData(LineData rhs)
        {
            Vernacular = new StringTransfer(rhs.Vernacular.ToString());
            NationalBt = new StringTransfer(rhs.NationalBt.ToString());
            InternationalBt = new StringTransfer(rhs.InternationalBt.ToString());
            FreeTranslation = new StringTransfer((rhs.FreeTranslation != null) ? rhs.FreeTranslation.ToString() : null);
        }

        public LineData()
        {
            InitEmpty();
        }

        public LineData(XmlNode node, string strNodeLabel)
        {
            InitEmpty();
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(strNodeLabel);
            if (list != null)
                foreach (XmlNode nodeData in list)
                {
                    if (nodeData.Attributes != null)
                    {
                        XmlAttribute attr = nodeData.Attributes[CstrAttributeLang];
                        string strLang = (attr != null) ? attr.Value : null;
                        SetValue(strLang, nodeData.InnerText);
                    }
                }
        }

        private void InitEmpty()
        {
            Vernacular = new StringTransfer(null);
            NationalBt = new StringTransfer(null);
            InternationalBt = new StringTransfer(null);
            FreeTranslation = new StringTransfer(null);
        }

        public virtual void IndexSearch(VerseData.SearchLookInProperties findProperties,
            VerseData.ViewSettings.ItemToInsureOn itemToInsureOn,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (Vernacular.HasData && findProperties.StoryLanguage)
                lstBoxesToSearch.AddNewVerseString(Vernacular,
                    itemToInsureOn | VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            if (NationalBt.HasData && findProperties.NationalBT)
                lstBoxesToSearch.AddNewVerseString(NationalBt,
                    itemToInsureOn | VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
            if (InternationalBt.HasData && findProperties.EnglishBT)
                lstBoxesToSearch.AddNewVerseString(InternationalBt,
                    itemToInsureOn | VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);
            if (FreeTranslation.HasData && findProperties.FreeTranslation)
                lstBoxesToSearch.AddNewVerseString(FreeTranslation,
                    itemToInsureOn | VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField);
        }

        public CtrlTextBox ExistingTextBox
        {
            get
            {
                if (InternationalBt.TextBox != null)
                    return InternationalBt.TextBox;
                if (NationalBt.TextBox != null)
                    return NationalBt.TextBox;
                if (Vernacular.TextBox != null)
                    return Vernacular.TextBox;
                if (FreeTranslation.TextBox != null)
                    return FreeTranslation.TextBox;
                return null;
            }
        }

        public virtual void AddXml(XElement elem, string strFieldName)
        {
            if (Vernacular.HasData)
                elem.Add(new XElement(strFieldName, 
                    new XAttribute(CstrAttributeLang, CstrAttributeLangVernacular),
                    Vernacular));
            if (NationalBt.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangNationalBt),
                    NationalBt));
            if (InternationalBt.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangInternationalBt), 
                    InternationalBt));
            if (FreeTranslation.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangFreeTranslation), 
                    FreeTranslation));
        }

        public void ExtractSelectedText(out string strVernacular, out string strNationalBt, out string strEnglishBt, out string strFreeTranslation)
        {
            Vernacular.ExtractSelectedText(out strVernacular);
            NationalBt.ExtractSelectedText(out strNationalBt);
            InternationalBt.ExtractSelectedText(out strEnglishBt);
            FreeTranslation.ExtractSelectedText(out strFreeTranslation);
        }
    }

    public class VerseData
    {
        public string guid;
        public bool IsFirstVerse;
        public bool IsVisible = true;
        public LineData StoryLine;
        public AnchorsData Anchors;
        public ExegeticalHelpNotesData ExegeticalHelpNotes;
        public TestQuestionsData TestQuestions;
        public RetellingsData Retellings;
        public ConsultantNotesData ConsultantNotes;
        public CoachNotesData CoachNotes;

        /*
        public StringTransfer VernacularText
        {
            get { return StoryLine.Vernacular; }
            // set { StoryLine.Vernacular = value; }
        }

        public StringTransfer NationalBTText
        {
            get { return StoryLine.NationalBt; }
            // set { StoryLine.NationalBt = value; }
        }

        public StringTransfer InternationalBTText
        {
            get { return StoryLine.InternationalBt; }
            // set { StoryLine.InternationalBt = value; }
        }

        public StringTransfer FreeTranslationText
        {
            get { return StoryLine.FreeTranslation; }
            // set { StoryLine.FreeTranslation = value; }
        }
        */
        public VerseData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
        {
            guid = theVerseRow.guid;

            if (!theVerseRow.IsfirstNull())
                IsFirstVerse = theVerseRow.first;

            if (!theVerseRow.IsvisibleNull())
                IsVisible = theVerseRow.visible;

            StoryLine = new LineData();
            foreach (NewDataSet.StoryLineRow aStoryLine in theVerseRow.GetStoryLineRows())
                StoryLine.SetValue(aStoryLine.lang,
                    (aStoryLine.IsStoryLine_textNull()) ? null : aStoryLine.StoryLine_text);

            Anchors = new AnchorsData(theVerseRow, projFile);
            ExegeticalHelpNotes = new ExegeticalHelpNotesData(theVerseRow, projFile);
            TestQuestions = new TestQuestionsData(theVerseRow, projFile);
            Retellings = new RetellingsData(theVerseRow, projFile);
            ConsultantNotes = new ConsultantNotesData(theVerseRow, projFile);
            CoachNotes = new CoachNotesData(theVerseRow, projFile);
        }

        public VerseData()
        {
            guid = Guid.NewGuid().ToString();
            StoryLine = new LineData();
            Anchors = new AnchorsData();
            ExegeticalHelpNotes = new ExegeticalHelpNotesData();
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
            IsVisible = ((attr = node.Attributes[CstrAttributeVisible]) != null) ? (attr.Value == "true") : true;
            StoryLine = new LineData(node, CstrFieldNameStoryLine);
            Anchors = new AnchorsData(node.SelectSingleNode(AnchorsData.CstrElementLabelAnchors));
            ExegeticalHelpNotes = new ExegeticalHelpNotesData(node.SelectSingleNode(ExegeticalHelpNotesData.CstrElementLabelExegeticalHelps));
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
            StoryLine = new LineData(rhs.StoryLine);
            Anchors = new AnchorsData(rhs.Anchors);
            ExegeticalHelpNotes = new ExegeticalHelpNotesData(rhs.ExegeticalHelpNotes);
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
            public bool FreeTranslation { get; set; }
            public bool ConsultantNotes { get; set; }
            public bool CoachNotes { get; set; }
            public bool Retellings { get; set; }
            public bool TestQs { get; set; }
            public bool TestAs { get; set; }
            public bool SearchAll { get; set; }
            public bool UseRegex { get; set; }
        }

        public class VerseString
        {
            public StringTransfer StringTransfer { get; set; }
            public ViewSettings.ItemToInsureOn ViewToInsureIsOn { get; set; }

            public VerseString(StringTransfer strStringTransfer,
                ViewSettings.ItemToInsureOn viewItemToInsureOn)
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
                ViewSettings.ItemToInsureOn viewItemToInsureOn)
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
            if (StoryLine.HasData)
                StoryLine.IndexSearch(findProperties, ViewSettings.ItemToInsureOn.Undefined,
                    ref lstBoxesToSearch);
            if (TestQuestions.HasData && (findProperties.TestQs || findProperties.TestAs))
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
                return (StoryLine.HasData || Anchors.HasData || TestQuestions.HasData 
                    || Retellings.HasData || ConsultantNotes.HasData || CoachNotes.HasData);
            }
        }

        public const string CstrFieldNameStoryLine = "StoryLine";

        public const string CstrAttributeGuid = "guid";
        public const string CstrElementLabelVerse = "Verse";
        public const string CstrAttributeFirstVerse = "first";
        public const string CstrAttributeLastVerse = "last";
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
                
                if (StoryLine.HasData)
                    StoryLine.AddXml(elemVerse, CstrFieldNameStoryLine);
                if (Anchors.HasData)
                    elemVerse.Add(Anchors.GetXml);
                if (ExegeticalHelpNotes.HasData)
                    elemVerse.Add(ExegeticalHelpNotes.GetXml);
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

        public void AllowConNoteButtonsOverride()
        {
            ConsultantNotes.AllowButtonsOverride();
            CoachNotes.AllowButtonsOverride();
        }

        public class ViewSettings
        {
            [Flags]
            public enum ItemToInsureOn
            {
                Undefined = 0,
                VernacularLangField = 1,
                VernacularTransliterationField = 2,
                NationalBTLangField = 4,
                NationalBTTransliterationField = 8,
                EnglishBTField = 16,
                FreeTranslationField = 32,
                AnchorFields = 64,
                ConsultantNoteFields = 128,
                CoachNotesFields = 256,
                BibleViewer = 512,
                StoryFrontMatter = 1024,
                HiddenStuff = 2048,
                OpenConNotesOnly = 4096,
                RetellingsVernacular = 8192,
                RetellingsNationalBT = 16384,
                RetellingsInternationalBT = 32768,
                TestQuestionsVernacular = 65536,
                TestQuestionsNationalBT = 131072,
                TestQuestionsInternationalBT = 262144,
                AnswersVernacular = 524288,
                AnswersNationalBT = 1048576,
                AnswersInternationalBT = 2097152,
                GeneralTestQuestions = 4194304,
                ExegeticalHelps = 8388608,
                RetellingFields = 8192 | 16384 | 32768,
                StoryTestingQuestions = 65536 | 131072 | 262144,
                StoryTestingQuestionAnswers = 524288 | 1048576 | 2097152
            }

            public DirectableEncConverter TransliteratorVernacular { get; set; }
            public DirectableEncConverter TransliteratorNationalBT { get; set; }
            protected ItemToInsureOn _itemToInsureOn;

            public long LongValue
            {
                get
                {
                    return (long)_itemToInsureOn;
                }

                set
                {
                    _itemToInsureOn = (ItemToInsureOn)value;
                }
            }

            public ViewSettings(long lSettings)
            {
                LongValue = lSettings;
            }

            public ViewSettings(ItemToInsureOn itemToInsureOn)
            {
                _itemToInsureOn = itemToInsureOn;
            }

            public ViewSettings
                (
                ProjectSettings projSettings,
                bool bLangVernacular,
                bool bLangNationalBT,
                bool bLangInternationalBT,
                bool bLangFreeTranslation,
                bool bAnchors,
                bool bExegeticalHelps,
                bool bStoryTestingQuestions,
                bool bStoryTestingQuestionAnswers,
                bool bRetellings,
                bool bConsultantNotes,
                bool bCoachNotes,
                bool bBibleViewer,
                bool bStoryFrontMatter,
                bool bHiddenStuff,
                bool bOpenConversationsOnly,
                bool bGeneralTestQuestions,
                DirectableEncConverter decTransliteratorVernacular,
                DirectableEncConverter decTransliteratorNationalBT
                )
            {
                SetItemsToInsureOn(projSettings,
                                   bLangVernacular,
                                   (decTransliteratorVernacular != null),
                                   bLangNationalBT,
                                   (decTransliteratorNationalBT != null),
                                   bLangInternationalBT,
                                   bLangFreeTranslation,
                                   bAnchors,
                                   bExegeticalHelps,
                                   bStoryTestingQuestions,
                                   bStoryTestingQuestionAnswers,
                                   bRetellings,
                                   bConsultantNotes,
                                   bCoachNotes,
                                   bBibleViewer,
                                   bStoryFrontMatter,
                                   bHiddenStuff,
                                   bOpenConversationsOnly,
                                   bGeneralTestQuestions);
                TransliteratorVernacular = decTransliteratorVernacular;
                TransliteratorNationalBT = decTransliteratorNationalBT;
            }

            public bool IsViewItemOn(ItemToInsureOn eFlag)
            {
                return ((_itemToInsureOn & eFlag) != ItemToInsureOn.Undefined);
            }

            public void SetItemsToInsureOn
                (
                ProjectSettings projSettings,
                bool bLangVernacular,
                bool bLangVernacularTransliterate,
                bool bLangNationalBT,
                bool bLangNationalBTTransliterate,
                bool bLangInternationalBT,
                bool bLangFreeTranslation,
                bool bAnchors,
                bool bExegeticalHelps,
                bool bStoryTestingQuestions,
                bool bStoryTestingQuestionAnswers,
                bool bRetellings,
                bool bConsultantNotes,
                bool bCoachNotes,
                bool bBibleViewer,
                bool bStoryFrontMatter,
                bool bHiddenStuff,
                bool bOpenConNotesOnly,
                bool bGeneralTestQuestions
                )
            {
                _itemToInsureOn = 0;
                if (bLangVernacular)
                    _itemToInsureOn |= ItemToInsureOn.VernacularLangField;
                if (bLangVernacularTransliterate)
                    _itemToInsureOn |= ItemToInsureOn.VernacularTransliterationField;
                if (bLangNationalBT)
                    _itemToInsureOn |= ItemToInsureOn.NationalBTLangField;
                if (bLangNationalBTTransliterate)
                    _itemToInsureOn |= ItemToInsureOn.NationalBTTransliterationField;
                if (bLangInternationalBT)
                    _itemToInsureOn |= ItemToInsureOn.EnglishBTField;
                if (bLangFreeTranslation)
                    _itemToInsureOn |= ItemToInsureOn.FreeTranslationField;
                if (bAnchors)
                    _itemToInsureOn |= ItemToInsureOn.AnchorFields;
                if (bExegeticalHelps)
                    _itemToInsureOn |= ItemToInsureOn.ExegeticalHelps;
                if (bStoryTestingQuestions)
                {
                    // break this down based on projSettings
                    if (projSettings.ShowTestQuestionsVernacular)
                        _itemToInsureOn |= ItemToInsureOn.TestQuestionsVernacular;
                    if (projSettings.ShowTestQuestionsNationalBT)
                        _itemToInsureOn |= ItemToInsureOn.TestQuestionsNationalBT;
                    if (projSettings.ShowTestQuestionsInternationalBT)
                        _itemToInsureOn |= ItemToInsureOn.TestQuestionsInternationalBT;
                }
                if (bStoryTestingQuestionAnswers)
                {
                    // break this down based on projSettings
                    if (projSettings.ShowAnswersVernacular)
                        _itemToInsureOn |= ItemToInsureOn.AnswersVernacular;
                    if (projSettings.ShowAnswersNationalBT)
                        _itemToInsureOn |= ItemToInsureOn.AnswersNationalBT;
                    if (projSettings.ShowAnswersInternationalBT)
                        _itemToInsureOn |= ItemToInsureOn.AnswersInternationalBT;
                }
                if (bRetellings)
                {
                    // break this down based on projSettings
                    if (projSettings.ShowRetellingVernacular)
                        _itemToInsureOn |= ItemToInsureOn.RetellingsVernacular;
                    if (projSettings.ShowRetellingNationalBT)
                        _itemToInsureOn |= ItemToInsureOn.RetellingsNationalBT;
                    if (projSettings.ShowRetellingInternationalBT)
                        _itemToInsureOn |= ItemToInsureOn.RetellingsInternationalBT;
                }
                if (bConsultantNotes)
                    _itemToInsureOn |= ItemToInsureOn.ConsultantNoteFields;
                if (bCoachNotes)
                    _itemToInsureOn |= ItemToInsureOn.CoachNotesFields;
                if (bBibleViewer)
                    _itemToInsureOn |= ItemToInsureOn.BibleViewer;
                if (bStoryFrontMatter)
                    _itemToInsureOn |= ItemToInsureOn.StoryFrontMatter;
                if (bHiddenStuff)
                    _itemToInsureOn |= ItemToInsureOn.HiddenStuff;
                if (bOpenConNotesOnly)
                    _itemToInsureOn |= ItemToInsureOn.OpenConNotesOnly;
                if (bGeneralTestQuestions)
                    _itemToInsureOn |= ItemToInsureOn.GeneralTestQuestions;
            }
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

        /* don't think this used and it surely needs to be fixed
        public string StoryBtHtml(ProjectSettings projectSettings, TeamMembersData membersData,
            StoryStageLogic stageLogic, TeamMemberData loggedOnMember, int nVerseIndex, 
            ViewSettings viewItemToInsureOn, int nNumCols)
        {
            string strRow = null;
            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.VernacularLangField))
            {
                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangVernacular,
                                               StoryData.CstrLangVernacularStyleClassName, 
                                               StoryLine.Vernacular.ToString());
            }

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.NationalBTLangField))
            {
                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangNationalBt,
                                               StoryData.CstrLangNationalBtStyleClassName, 
                                               StoryLine.NationalBt.ToString());
            }

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.EnglishBTField))
            {
                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangInternationalBt,
                                               StoryData.CstrLangInternationalBtStyleClassName, 
                                               StoryLine.InternationalBt.ToString());
            }

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.FreeTranslationField))
            {
                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangFreeTranslation,
                                               StoryData.CstrLangFreeTranslationStyleClassName, 
                                               StoryLine.FreeTranslation.ToString());
            }

            string strStoryLineRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                   strRow);

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.AnchorFields))
                strStoryLineRow += Anchors.Html(nVerseIndex, nNumCols);

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingFields)
                && (Retellings.Count > 0))
            {
                strStoryLineRow += Retellings.Html(nVerseIndex, nNumCols,
                    viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingsVernacular),
                    viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingsNationalBT),
                    viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingsInternationalBT));
            }

            if (viewItemToInsureOn.IsViewItemOn(ViewSettings.ItemToInsureOn.StoryTestingQuestions | ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers)
                && (TestQuestions.Count > 0))
                strStoryLineRow += TestQuestions.Html(projectSettings, viewItemToInsureOn,
                    stageLogic, loggedOnMember, nVerseIndex, nNumCols);

            return strStoryLineRow;
        }
        */

        public bool IsDiffProcessed;

        // figure out the logic of what to show for the language fields of the story line
        //  this should never be called in the scenario where there it no *this*
        protected bool TryStoryLineStringDiff(DirectableEncConverter transliterator,
            bool bPrintPreview, VerseData theChildVerse, StringTransfer stringTransfer,
            out string strValue)
        {
            strValue = null;
            // if we're not comparing two verses (e.g. print preview)...
            bool bNoDiff = bPrintPreview;
            if (bNoDiff)
            {
                // then just return the value (possibly transliterated)
                strValue = stringTransfer.GetValue(transliterator);
                return true;
            }

            // otherwise, if this should be represented as a deletion
            //  (e.g. there is not child verse corresponding to the parent)...
            bool bShowAsDeletion = (theChildVerse == null);
            if (bShowAsDeletion)
            {
                // then return it as a deletion
                strValue = Diff.HtmlDiff(transliterator, stringTransfer, null);
                return true;
            }
            return false;
        }

        // Html that shows the data in the StoryBt file, but in a fully read-only manner
        public string PresentationHtml(int nVerseIndex, int nNumCols, CraftingInfoData craftingInfo,
            ViewSettings viewSettings, VerseData theChildVerse, 
            bool bPrintPreview, bool bHasOutsideEnglishBTer)
        {
            string strRow = null;
            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.VernacularLangField))
            {
                DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                string str;
                if (!TryStoryLineStringDiff(transliterator, bPrintPreview, theChildVerse,
                    StoryLine.Vernacular, out str))
                {
                    // otherise, it must be compared
                    str = Diff.HtmlDiff(transliterator, StoryLine.Vernacular,
                        theChildVerse.StoryLine.Vernacular);
                }

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangVernacular,
                                               StoryData.CstrLangVernacularStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.NationalBTLangField))
            {
                DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                string str;
                if (!TryStoryLineStringDiff(transliterator, bPrintPreview, theChildVerse,
                    StoryLine.NationalBt, out str))
                {
                    // otherise, it must be compared
                    str = Diff.HtmlDiff(transliterator, StoryLine.NationalBt,
                        theChildVerse.StoryLine.NationalBt);
                }

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangNationalBt,
                                               StoryData.CstrLangNationalBtStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.EnglishBTField))
            {
                string str;
                if (!TryStoryLineStringDiff(null, bPrintPreview, theChildVerse,
                    StoryLine.InternationalBt, out str))
                {
                    // otherise, it must be compared
                    str = Diff.HtmlDiff(null, StoryLine.InternationalBt,
                        theChildVerse.StoryLine.InternationalBt);
                }

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangInternationalBt,
                                               StoryData.CstrLangInternationalBtStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.FreeTranslationField))
            {
                string str;
                if (!TryStoryLineStringDiff(null, bPrintPreview, theChildVerse,
                    StoryLine.FreeTranslation, out str))
                {
                    // otherise, it must be compared
                    str = Diff.HtmlDiff(null, StoryLine.FreeTranslation,
                        theChildVerse.StoryLine.FreeTranslation);
                }

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangFreeTranslation,
                                               StoryData.CstrLangFreeTranslationStyleClassName, str);
            }

            string strHtml = null;
            var astrExegeticalHelpNotes = new List<string>();
            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.AnchorFields))
            {
                strHtml += Anchors.PresentationHtml(nVerseIndex, nNumCols,
                                                    (theChildVerse != null) ? theChildVerse.Anchors : null,
                                                    bPrintPreview, 
                                                    ref astrExegeticalHelpNotes);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.ExegeticalHelps))
                ExegeticalHelpNotes.PresentationHtml((theChildVerse != null) ? theChildVerse.ExegeticalHelpNotes : null,
                                                     ref astrExegeticalHelpNotes);

            if (!String.IsNullOrEmpty(strHtml) || (astrExegeticalHelpNotes.Count > 0))
                strHtml = ExegeticalHelpNotes.FinishPresentationHtml(strHtml,
                                                                     nVerseIndex,
                                                                     nNumCols,
                                                                     astrExegeticalHelpNotes);

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingFields))
            {
                strHtml += Retellings.PresentationHtml(nVerseIndex, nNumCols,
                                                       craftingInfo.TestorsToCommentsRetellings,
                                                       (theChildVerse != null) ? theChildVerse.Retellings : null,
                                                       bPrintPreview, false,
                                                       viewSettings.IsViewItemOn(
                                                           ViewSettings.ItemToInsureOn.RetellingsVernacular),
                                                       viewSettings.IsViewItemOn(
                                                           ViewSettings.ItemToInsureOn.RetellingsNationalBT),
                                                       viewSettings.IsViewItemOn(
                                                           ViewSettings.ItemToInsureOn.RetellingsInternationalBT));
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.StoryTestingQuestions |
                                          ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
                strHtml += TestQuestions.PresentationHtml(nVerseIndex, nNumCols, viewSettings,
                                                          craftingInfo.TestorsToCommentsTqAnswers,
                                                          (theChildVerse != null) ? theChildVerse.TestQuestions : null,
                                                          bPrintPreview, bHasOutsideEnglishBTer);
            
            // show the row as hidden if either we're in print preview (and it's hidden)
            //  OR based on whether the child is hidden or not
            bool bShowAsHidden = (bPrintPreview)
                                     ? !IsVisible
                                     : ((theChildVerse != null) && !theChildVerse.IsVisible)
                                           ? true
                                           : false;
            return FinishPresentationHtml(strRow, strHtml, bShowAsHidden);
        }

        protected string FinishPresentationHtml(string strStoryLineRow, string strHtml, 
            bool bChildIsHidden)
        {
            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                                  String.Format(
                                                      OseResources.Properties.Resources.HTML_Table,
                                                      strStoryLineRow))) + strHtml;

            strStoryLineRow = String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            String.Format(OseResources.Properties.Resources.HTML_TableNoBorder, strHtml));

            // color changes if hidden
            if (bChildIsHidden)
            {
                strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRowColor,
                                                "#F0E68C", strStoryLineRow);
            }
            else
            {
                strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                                strStoryLineRow);
            }

            // indicate that we've done this one
            IsDiffProcessed = true;
            return strHtml;
        }

        private static string FormatLanguageColumn(int nVerseIndex, int nNumCols,
            string strFieldName, string strLangStyleClassName, string strValue)
        {
            return String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(nVerseIndex, strFieldName),
                                                      strLangStyleClassName,
                                                      strValue));
        }

        // for use when the data is to be marked as an addition (i.e. yellow highlight)
        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols,
            CraftingInfoData craftingInfo, ViewSettings viewSettings,
            bool bHasOutsideEnglishBTer)
        {
            string strRow = null;
            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.VernacularLangField))
            {
                DirectableEncConverter transliterator = viewSettings.TransliteratorVernacular;
                string str = Diff.HtmlDiff(transliterator, null, StoryLine.Vernacular);

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangVernacular,
                                               StoryData.CstrLangVernacularStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.NationalBTLangField))
            {
                DirectableEncConverter transliterator = viewSettings.TransliteratorNationalBT;
                string str = Diff.HtmlDiff(transliterator, null, StoryLine.NationalBt);

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangNationalBt,
                                               StoryData.CstrLangNationalBtStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.EnglishBTField))
            {
                string str = Diff.HtmlDiff(null, StoryLine.InternationalBt);

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangInternationalBt,
                                               StoryData.CstrLangInternationalBtStyleClassName, str);
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.FreeTranslationField))
            {
                string str = Diff.HtmlDiff(null, StoryLine.FreeTranslation);

                strRow += FormatLanguageColumn(nVerseIndex, nNumCols, LineData.CstrAttributeLangFreeTranslation,
                                               StoryData.CstrLangFreeTranslationStyleClassName, str);
            }

            string strHtml = null;
            var astrExegeticalHelpNotes = new List<string>();
            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.AnchorFields))
                strHtml += Anchors.PresentationHtmlAsAddition(nVerseIndex, nNumCols,
                    ref astrExegeticalHelpNotes);

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.ExegeticalHelps))
                ExegeticalHelpNotes.PresentationHtml(null, ref astrExegeticalHelpNotes);

            if (astrExegeticalHelpNotes.Count > 0)
                strHtml = ExegeticalHelpNotes.FinishPresentationHtml(strHtml,
                                                                     nVerseIndex,
                                                                     nNumCols,
                                                                     astrExegeticalHelpNotes);

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.RetellingFields))
            {
                strHtml += Retellings.PresentationHtmlAsAddition(nVerseIndex, nNumCols,
                                                                 craftingInfo.TestorsToCommentsRetellings,
                                                                 viewSettings.IsViewItemOn(
                                                                     ViewSettings.ItemToInsureOn.RetellingsVernacular),
                                                                 viewSettings.IsViewItemOn(
                                                                     ViewSettings.ItemToInsureOn.RetellingsNationalBT),
                                                                 viewSettings.IsViewItemOn(
                                                                     ViewSettings.ItemToInsureOn.
                                                                         RetellingsInternationalBT));
            }

            if (viewSettings.IsViewItemOn(ViewSettings.ItemToInsureOn.StoryTestingQuestions |
                                          ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
                strHtml += TestQuestions.PresentationHtmlAsAddition(nVerseIndex, nNumCols, viewSettings,
                                                                    craftingInfo.TestorsToCommentsTqAnswers,
                                                                    bHasOutsideEnglishBTer);

            return FinishPresentationHtml(strRow, strHtml, !IsVisible);
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            Retellings.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
            TestQuestions.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
        }

        public void SetCommentMemberId(string strPfMemberId, string strConsultant, string strCoach)
        {
            ConsultantNotes.SetCommentMemberId(strPfMemberId, strConsultant);
            CoachNotes.SetCommentMemberId(strConsultant, strCoach);
        }

        public bool HasAllConsultantNoteMentoreeMemberIdData
        {
            get { return ConsultantNotes.HasAllMentoreeMemberIdData; }
        }

        public bool HasAllConsultantNoteMentorMemberIdData
        {
            get { return ConsultantNotes.HasAllMentorMemberIdData; }
        }

        public bool HasAllCoachNoteMentoreeMemberIdData
        {
            get { return CoachNotes.HasAllMentoreeMemberIdData; }
        }

        public bool HasAllCoachNoteMentorMemberIdData
        {
            get { return CoachNotes.HasAllMentorMemberIdData; }
        }

        public bool AreUnapprovedConsultantNotes
        {
            get { return ConsultantNotes.AreUnapprovedComments; }
        }

        public bool AreUnrespondedToCoachNoteComments
        {
            get { return CoachNotes.AreUnrespondedToCoachNoteComments; }
        }

        public void UpdateCommentMemberId(string strOldMemberGuid, string strNewMemberGuid)
        {
            // for now, this is only used when update the PF (since we don't keep track
            //  of who the consultant for a story is), so PFs can only deal with the
            //  ConsultantNotes pane (if it were generalized, then add CoachNotes
            ConsultantNotes.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
        }
    }

    public class VersesData : List<VerseData>
    {
        internal const string CstrZerothLineNameConNotes = "Story:";
        internal const string CstrZerothLineNameBtPane = "Gen Qs:";
        public VerseData FirstVerse;    // full-story oriented line (no BT) for ConNotes

        public VersesData(NewDataSet.storyRow theStoryRow, NewDataSet projFile)
        {
            NewDataSet.VersesRow[] theVersesRows = theStoryRow.GetVersesRows();
            NewDataSet.VersesRow theVersesRow;
            if (theVersesRows.Length == 0)
                theVersesRow = projFile.Verses.AddVersesRow(theStoryRow);
            else
                theVersesRow = theVersesRows[0];

            foreach (NewDataSet.VerseRow aVerseRow in theVersesRow.GetVerseRows())
                Add(new VerseData(aVerseRow, projFile));

            AdjustmentForFirstVerse();
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

            AdjustmentForFirstVerse();
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

        private void AdjustmentForFirstVerse()
        {
            // the zeroth verse is special for global connotes and the new last one
            //  is for general testing questions
            if (Count > 0)
            {
                if (this[0].IsFirstVerse)
                {
                    FirstVerse = this[0];
                    RemoveAt(0);
                }
            }

            InsureFirstVerse();

            // sometimes the others think they are first verse too... (not sure how this
            //  happens)
            foreach (var aVerse in this)
                aVerse.IsFirstVerse = false;
        }

        public void InsureFirstVerse()
        {
            if (FirstVerse == null)
                FirstVerse = new VerseData {IsFirstVerse = true};
        }

        public VerseData InsertVerse(int nIndex, string strVernacular, 
            string strNationalBt, string strInternationalBt, string strFreeTranslation)
        {
            var dataVerse = new VerseData
                                {
                                    StoryLine =
                                        {
                                            Vernacular = new StringTransfer(strVernacular),
                                            NationalBt = new StringTransfer(strNationalBt),
                                            InternationalBt = new StringTransfer(strInternationalBt),
                                            FreeTranslation = new StringTransfer(strFreeTranslation)
                                        }
                                };
            Insert(nIndex, dataVerse);
            return dataVerse;
        }

        public bool HasData
        {
            get
            {
                return (Count > 0)
                       || ((FirstVerse != null) && (FirstVerse.HasData));
            }
        }

        public const string CstrElementLabelVerses = "Verses";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                var elemVerses = new XElement(CstrElementLabelVerses);
                
                // write out the zeroth verse first
                elemVerses.Add(FirstVerse.GetXml);

                // then write out the rest
                foreach (VerseData aVerseData in this)
                    elemVerses.Add(aVerseData.GetXml);

                return elemVerses;
            }
        }

        public int NumOfLines
        {
            get
            {
                return this.Count(aVerse => aVerse.IsVisible);
            }
        }

        public string NumOfWords(ProjectSettings projSettings)
        {
            string strCount = null;
            if (projSettings.Vernacular.HasData)
                strCount = GetWordCountVernacular(projSettings);
            else if (projSettings.NationalBT.HasData)
                strCount = GetWordCountNationalBT(projSettings);
            else if (projSettings.InternationalBT.HasData)
                strCount = GetWordCountInternationalBT(projSettings);
            else if (projSettings.FreeTranslation.HasData)
                strCount = GetWordCountFreeTranslation(projSettings);
            return strCount;
        }

        protected string GetWordCountVernacular(ProjectSettings projSettings)
        {
            ProjectSettings.LanguageInfo li = projSettings.Vernacular;
            char[] achToIgnore = GetSplitChars(li.FullStop);

            int nCount = 0;
            foreach (VerseData aVerse in this)
                if (aVerse.IsVisible)
                    nCount += aVerse.StoryLine.Vernacular.NumOfWords(achToIgnore);
            return String.Format("{0} (in {1})", nCount, li.LangName);
        }

        protected string GetWordCountNationalBT(ProjectSettings projSettings)
        {
            ProjectSettings.LanguageInfo li = projSettings.NationalBT;
            char[] achToIgnore = GetSplitChars(li.FullStop);

            int nCount = 0;
            foreach (VerseData aVerse in this)
                if (aVerse.IsVisible)
                    nCount += aVerse.StoryLine.NationalBt.NumOfWords(achToIgnore);
            return String.Format("{0} (in {1})", nCount, li.LangName);
        }

        protected string GetWordCountInternationalBT(ProjectSettings projSettings)
        {
            ProjectSettings.LanguageInfo li = projSettings.InternationalBT;
            char[] achToIgnore = GetSplitChars(li.FullStop);

            int nCount = 0;
            foreach (VerseData aVerse in this)
                if (aVerse.IsVisible)
                    nCount += aVerse.StoryLine.InternationalBt.NumOfWords(achToIgnore);
            return String.Format("{0} (in {1})", nCount, li.LangName);
        }

        protected string GetWordCountFreeTranslation(ProjectSettings projSettings)
        {
            ProjectSettings.LanguageInfo li = projSettings.FreeTranslation;
            char[] achToIgnore = GetSplitChars(li.FullStop);

            int nCount = 0;
            foreach (VerseData aVerse in this)
                if (aVerse.IsVisible)
                    nCount += aVerse.StoryLine.FreeTranslation.NumOfWords(achToIgnore);
            return String.Format("{0} (in {1})", nCount, li.LangName);
        }

        public static char[] GetSplitChars(string strFullStop)
        {
            string strCharsToIgnore = strFullStop + QuoteCharsAsString + CstrWhiteSpace;
            return strCharsToIgnore.ToCharArray();
        }

        protected int CalculateColumns(VerseData.ViewSettings viewItemToInsureOn)
        {
            int nColSpan = 0;
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                nColSpan++;
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField))
                nColSpan++;
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField))
                nColSpan++;
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                nColSpan++;
            return nColSpan;
        }

        /*
        internal static char[] achQuotesRightEdge = new[] { '"', '\'', '\u2019', '\u201d', '\u201E' };
        internal static char[] achQuotesLeftEdge = new[] { '\u2018', '\u201B', '\u201C', '\u201F' };
        */

        internal static char[] achQuotes
        {
            get
            {
                string strLeftEdgeQuotes = Properties.Settings.Default.RightEdgeQuotes;
                string strRightEdgeQuotes = Properties.Settings.Default.LeftEdgeQuotes;
                string strEitherEdgeQuotes = Properties.Settings.Default.EitherEdgeQuotes;
                var ach = new char[strLeftEdgeQuotes.Length + strRightEdgeQuotes.Length + strEitherEdgeQuotes.Length];

                int i = 0;
                foreach (char ch in strLeftEdgeQuotes)
                    ach[i++] += ch;
                foreach (char ch in strRightEdgeQuotes)
                    ach[i++] += ch;
                foreach (char ch in strEitherEdgeQuotes)
                    ach[i++] += ch;
                return ach;
            }
        }

        internal static string QuoteCharsAsString
        {
            get
            {
                string strLeftEdgeQuotes = Properties.Settings.Default.RightEdgeQuotes;
                string strRightEdgeQuotes = Properties.Settings.Default.LeftEdgeQuotes;
                string strEitherEdgeQuotes = Properties.Settings.Default.EitherEdgeQuotes;
                return strLeftEdgeQuotes + strRightEdgeQuotes + strEitherEdgeQuotes;
            }
        }

        public bool HasAllConsultantNoteMentoreeMemberIdData
        {
            get
            {
                return (FirstVerse.HasAllConsultantNoteMentoreeMemberIdData
                        && this.All(aVerse =>
                                    aVerse.HasAllConsultantNoteMentoreeMemberIdData));
            }
        }

        public bool HasAllConsultantNoteMentorMemberIdData
        {
            get
            {
                return (FirstVerse.HasAllConsultantNoteMentorMemberIdData
                        && this.All(aVerse =>
                                    aVerse.HasAllConsultantNoteMentorMemberIdData));
            }
        }

        public bool HasAllCoachNoteMentoreeMemberIdData
        {
            get
            {
                return (FirstVerse.HasAllCoachNoteMentoreeMemberIdData
                        && this.All(aVerse =>
                                    aVerse.HasAllCoachNoteMentoreeMemberIdData));
            }
        }

        public bool HasAllCoachNoteMentorMemberIdData
        {
            get
            {
                return (FirstVerse.HasAllCoachNoteMentorMemberIdData
                        && this.All(aVerse =>
                                    aVerse.HasAllCoachNoteMentorMemberIdData));
            }
        }

        public bool AreUnapprovedConsultantNotes
        {
            get
            {
                return FirstVerse.AreUnapprovedConsultantNotes ||
                       this.Any(aVerse =>
                                aVerse.AreUnapprovedConsultantNotes);
            }
        }

        public bool AreUnrespondedToCoachNoteComments
        {
            get
            {
                return FirstVerse.AreUnrespondedToCoachNoteComments ||
                       this.Any(aVerse =>
                                aVerse.AreUnrespondedToCoachNoteComments);
            }
        }

        public void CheckConNoteMemberIds(out bool bNeedPf, out bool bNeedCons, out bool bNeedCoach)
        {
            bNeedPf = !HasAllConsultantNoteMentoreeMemberIdData;
            bNeedCons = !HasAllConsultantNoteMentorMemberIdData ||
                        !HasAllCoachNoteMentoreeMemberIdData;
            bNeedCoach = !HasAllCoachNoteMentorMemberIdData;
        }

        internal const string CstrWhiteSpace = "\r\n ";
        /*
        public string StoryBtHtml(ProjectSettings projectSettings, bool bViewHidden,
            StoryStageLogic stageLogic, TeamMembersData membersData, TeamMemberData loggedOnMember,
            VerseData.ViewSettings viewItemToInsureOn)
        {
            int nColSpan = CalculateColumns(viewItemToInsureOn);

            // add a row indicating which languages are in what columns
            string strHtml = null;
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.Vernacular.LangName);
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.NationalBT.LangName);
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.InternationalBT.LangName);
            if (viewItemToInsureOn.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                         projectSettings.FreeTranslation.LangName);

            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                     strHtml);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (aVerseData.IsVisible || bViewHidden)
                {
                    strHtml += GetHeaderRow("Ln: " + i,
                                            (aVerseData.IsVisible)
                                                ? null
                                                : OseResources.Properties.Resources.IDS_HiddenLabel, i,
                                            true, nColSpan);

                    strHtml += aVerseData.StoryBtHtml(projectSettings, membersData, 
                        stageLogic, loggedOnMember, i, viewItemToInsureOn, nColSpan);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }
        */

        protected VerseData FindChildEquivalent(VerseData theParentVerse, VersesData child)
        {
            if (child != null)
                foreach (VerseData aVerse in child)
                    if (aVerse.guid == theParentVerse.guid)
                    {
                        aVerse.IsDiffProcessed = true;
                        return aVerse;
                    }
            return null;
        }

        public string PresentationHtml(CraftingInfoData craftingInfo, VersesData child, 
            int nNumCols, VerseData.ViewSettings viewSettings, bool bHasOutsideEnglishBTer)
        {
            string strHtml = null;
            int nInsertCount = 0;
            int i = 1;
            while (i <= Count)
            {
                // get the parent and child verses that match
                VerseData aVerseData = this[i - 1];

                VerseData theChildVerse = FindChildEquivalent(aVerseData, child);

                // process this as long as it *isn't* the first verse and either it's visible
                //  or we're showing hidden material
                if (!aVerseData.IsFirstVerse
                    && (((theChildVerse != null) ? theChildVerse.IsVisible : aVerseData.IsVisible) 
                        || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.HiddenStuff)))
                {
                    string strHeaderAdd = DetermineHiddenLabel(aVerseData.IsVisible, theChildVerse);

                    int nLineIndex = i + nInsertCount;
                    strHtml += GetHeaderRow("Ln: " + nLineIndex, strHeaderAdd, nLineIndex, false, nNumCols);

                    if (theChildVerse != null)
                    {
                        // see if there were any child verses that weren't processed
                        bool bFoundOne = false;
                        for (int j = i; j <= child.IndexOf(theChildVerse); j++)
                        {
                            VerseData aPassedByChild = child[j - 1];
                            if (!aPassedByChild.IsDiffProcessed)
                            {
                                strHtml += aPassedByChild.PresentationHtmlAsAddition(nLineIndex, 
                                    nNumCols, craftingInfo, viewSettings, 
                                    bHasOutsideEnglishBTer);
                                bFoundOne = true;
                                nInsertCount++;
                            }
                        }

                        if (bFoundOne)
                            continue;
                    }

                    strHtml += aVerseData.PresentationHtml(nLineIndex, nNumCols, craftingInfo, 
                        viewSettings, theChildVerse, (child == null), bHasOutsideEnglishBTer);

                    // if there is a child, but we couldn't find the equivalent verse...
                    if ((child != null) && (theChildVerse == null) && (child.Count >= i))
                    {
                        // this means the original verse (which we just showed as deleted) 
                        //  was replaced by whatever is the same verse in the child collection
                        theChildVerse = child[i - 1];
                        if (theChildVerse.IsVisible)
                            strHtml += theChildVerse.PresentationHtmlAsAddition(nLineIndex, nNumCols,
                                                                                craftingInfo, viewSettings,
                                                                                bHasOutsideEnglishBTer);
                    }
                }

                i++;    // do this here in case we redo one (from 'continue' above)
            }

            if (child != null)
                while (i <= child.Count)
                {
                    VerseData aVerseData = child[i - 1];
                    if (!aVerseData.IsDiffProcessed)
                    {
                        int nLineIndex = i + nInsertCount;
                        string strHeaderAdd = DetermineHiddenLabel(aVerseData.IsVisible, null);
                        strHtml += GetHeaderRow("Ln: " + nLineIndex, strHeaderAdd, nLineIndex, false, nNumCols);

                        strHtml += aVerseData.PresentationHtmlAsAddition(nLineIndex, nNumCols,
                                                                         craftingInfo, viewSettings,
                                                                         bHasOutsideEnglishBTer);
                    }
                    i++;
                }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        private static string DetermineHiddenLabel(bool isVisible, VerseData theChildVerse)
        {
            string str = null;
            if (!isVisible || ((theChildVerse != null) && (!theChildVerse.IsVisible)))
            {
                if (theChildVerse != null)
                {
                    if (!isVisible && !theChildVerse.IsVisible)
                        str = OseResources.Properties.Resources.IDS_HiddenLabel;
                    else if (!isVisible)
                        str = OseResources.Properties.Resources.IDS_WasHidden;
                    else
                        str = OseResources.Properties.Resources.IDS_NowIsHidden;
                }
                else
                    str = OseResources.Properties.Resources.IDS_HiddenLabel;
            }
            return str;
        }

        public static string ButtonId(int nVerseIndex)
        {
            return String.Format("btnLn_{0}", nVerseIndex);
        }

        public static string NoteToSelfButtonId(int nVerseIndex)
        {
            return String.Format("btnNoteToSelf_{0}", nVerseIndex);
        }

        protected string GetHeaderRow(string strHeader, string strHeaderAdd, int nVerseIndex, bool bShowButton, int nColSpan)
        {
            string strLink = String.Format(OseResources.Properties.Resources.HTML_LinkJumpLine,
                                           nVerseIndex, strHeader) + strHeaderAdd;

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

        internal const string CstrShowOpenHideClosed = "Hide Closed";
        internal const string CstrShowOpenShowAll = "Show All";

        protected string GetHeaderRow(string strHeader, int nVerseIndex, 
            bool bVerseVisible, bool bShowOnlyOpenConversations,
            ConsultNotesDataConverter theCNsDC, TeamMemberData LoggedOnMember,
            string strThePfMemberId)
        {
            string strHtmlButtons = null;
            if (theCNsDC.HasAddNotePrivilege(LoggedOnMember, strThePfMemberId))
            {
                strHtmlButtons += // String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                               String.Format(OseResources.Properties.Resources.HTML_Button,
                                                             nVerseIndex,
                                                             "return window.external.OnAddNote(this.id, null);",
                                                             "Add Note");
            }

            if (theCNsDC.HasAddNoteToSelfPrivilege(LoggedOnMember.MemberType))
            {
                strHtmlButtons += // String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                                String.Format(OseResources.Properties.Resources.HTML_Button,
                                                              NoteToSelfButtonId(nVerseIndex),
                                                              "return window.external.OnAddNoteToSelf(this.id, null);",
                                                              "Add Note to Self");
            }

            if (bShowOnlyOpenConversations)
                strHtmlButtons = // String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                               String.Format(OseResources.Properties.Resources.HTML_Button,
                                                             ButtonId(nVerseIndex),
                                                             "return window.external.OnShowHideOpenConversations(this.id);",
                                                             (theCNsDC.ShowOpenConversations)
                                                                 ? CstrShowOpenHideClosed
                                                                 : CstrShowOpenShowAll)
                                                                 + strHtmlButtons;  // to have 'Add Note' come last

            string strLink = String.Format(OseResources.Properties.Resources.HTML_LinkJumpLine,
                                           nVerseIndex, strHeader);
            if (!bVerseVisible)
                strLink += OseResources.Properties.Resources.IDS_HiddenLabel;

            return String.Format(OseResources.Properties.Resources.HTML_TableRowColor, "#AACCFF",
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellId,
                                                             LineId(nVerseIndex),
                                                             strLink),
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellRightAlign,
                                                             strHtmlButtons)));
        }

        public void ResetShowOpenConversationsFlags()
        {
            FirstVerse.ConsultantNotes.ShowOpenConversations =
                FirstVerse.CoachNotes.ShowOpenConversations = false;

            foreach (VerseData verseData in this)
                verseData.ConsultantNotes.ShowOpenConversations =
                    verseData.CoachNotes.ShowOpenConversations = false;
        }

        public string ConsultantNotesHtml(object htmlConNoteCtrl, 
            TeamMemberData LoggedOnMember, TeamMembersData teamMembers, 
            StoryData theStory, bool bViewHidden, bool bShowOnlyOpenConversations)
        {
            string strHtml = null;
            strHtml += GetHeaderRow(CstrZerothLineNameConNotes, 0,
                                    FirstVerse.IsVisible,
                                    bShowOnlyOpenConversations,
                                    FirstVerse.ConsultantNotes,
                                    LoggedOnMember,
                                    theStory.CraftingInfo.ProjectFacilitatorMemberID);

            strHtml += FirstVerse.ConsultantNotes.Html(htmlConNoteCtrl,
                                                       LoggedOnMember,
                                                       teamMembers,
                                                       theStory,
                                                       bViewHidden,
                                                       FirstVerse.IsVisible,
                                                       bShowOnlyOpenConversations, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (!aVerseData.IsVisible && !bViewHidden)
                    continue;

                strHtml += GetHeaderRow("Ln: " + i, i,
                                        aVerseData.IsVisible,
                                        bShowOnlyOpenConversations,
                                        aVerseData.ConsultantNotes,
                                        LoggedOnMember,
                                        theStory.CraftingInfo.ProjectFacilitatorMemberID);

                strHtml += aVerseData.ConsultantNotes.Html(htmlConNoteCtrl,
                                                           LoggedOnMember, 
                                                           teamMembers, 
                                                           theStory,
                                                           bViewHidden, 
                                                           aVerseData.IsVisible,
                                                           bShowOnlyOpenConversations, i);
            }

            return String.Format(OseResources.Properties.Resources.HTML_Table, strHtml);
        }

        public string CoachNotesHtml(object htmlConNoteCtrl, 
            TeamMemberData LoggedOnMember, TeamMembersData teamMembers, StoryData theStory,
            bool bViewHidden, bool bShowOnlyOpenConversations)
        {
            string strHtml = null;
            strHtml += GetHeaderRow(CstrZerothLineNameConNotes, 0,
                                    FirstVerse.IsVisible,
                                    bShowOnlyOpenConversations,
                                    FirstVerse.CoachNotes,
                                    LoggedOnMember,
                                    theStory.CraftingInfo.ProjectFacilitatorMemberID);

            strHtml += FirstVerse.CoachNotes.Html(htmlConNoteCtrl,
                                                  LoggedOnMember,
                                                  teamMembers,
                                                  theStory,
                                                  bViewHidden,
                                                  FirstVerse.IsVisible,
                                                  bShowOnlyOpenConversations, 0);

            for (int i = 1; i <= Count; i++)
            {
                VerseData aVerseData = this[i - 1];
                if (!aVerseData.IsVisible && !bViewHidden)
                    continue;
                
                strHtml += GetHeaderRow("Ln: " + i, i,
                                        aVerseData.IsVisible,
                                        bShowOnlyOpenConversations,
                                        aVerseData.CoachNotes,
                                        LoggedOnMember,
                                        theStory.CraftingInfo.ProjectFacilitatorMemberID);

                strHtml += aVerseData.CoachNotes.Html(htmlConNoteCtrl,
                                                      LoggedOnMember,
                                                      teamMembers,
                                                      theStory,
                                                      bViewHidden,
                                                      aVerseData.IsVisible,
                                                      bShowOnlyOpenConversations, i);
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

        public void ChangeRetellingTestorGuid(int nIndex, string strNewGuid)
        {
            foreach (var lineMemberData in this.Select(aVerseData => aVerseData.Retellings[nIndex]))
                lineMemberData.MemberId = strNewGuid;
        }

        public void ChangeTqAnswersTestorGuid(int nIndex, string strNewGuid)
        {
            foreach (var lineMemberData in
                this.SelectMany(aVerseData => aVerseData.TestQuestions.Select(aTqData => aTqData.Answers[nIndex])))
                lineMemberData.MemberId = strNewGuid;
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            FirstVerse.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
            foreach (var verse in this)
                verse.ReplaceUns(strOldUnsGuid, strNewUnsGuid);
        }

        public void SetCommentMemberId(string strPfMemberId, string strConsultant, string strCoach)
        {
            FirstVerse.SetCommentMemberId(strPfMemberId, strConsultant, strCoach);
            foreach (var verse in this)
                verse.SetCommentMemberId(strPfMemberId, strConsultant, strCoach);
        }

        public void UpdateCommentMemberId(string strOldMemberGuid, string strNewMemberGuid)
        {
            FirstVerse.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
            foreach (var verse in this)
                verse.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
        }
    }
}
