// rde: removing lable row to save pixels
// #define ShowLabelRow

using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TestingQuestionControl : ResizableControl
    {
        protected const string CstrFieldNameVernacular = "TQVernacular";
        protected const string CstrFieldNameNationalBt = "TQNationalBT";
        protected const string CstrFieldNameInternationalBt = "TQInternationalBT";
        protected const string CstrFieldNameAnswers = "Answers";
        protected const string CstrFieldNameTestQuestionsLabel = "TestQuestionsLabel";
        protected const string CstrTestQuestionsLabelFormat = "tst:";

        protected TestQuestionData _aTQData = null;
        protected int _nNumAnswerRows = 0;

        public TestingQuestionControl(StoryEditor theSE, VerseBtControl ctrlVerse, 
            TestQuestionData aTQData, bool bShowHeader)
            : base(theSE.theCurrentStory.ProjStage)
        {
            _aTQData = aTQData;

            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            // clobber the base class table layout panel's configuration. We're 'column-oriented' instead
            // first add another row so that we have two rows (row(0)=label, row(1)=text)
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here: TestingQuestionControl.cs.34");

#if ShowLabelRow
            InsertRow(1);
#endif

            // remove all but the left-most (autosize) column, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 1)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            int nNumColumns = 0;
            if (theSE.viewStoryTestingQuestionMenuItem.Checked)
            {
                // show the row label
                Label label = new Label
                {
                    Anchor = AnchorStyles.Left,
                    AutoSize = true,
                    Name = CstrFieldNameTestQuestionsLabel,
                    Text = CstrTestQuestionsLabelFormat
                };
#if ShowLabelRow
                tableLayoutPanel.Controls.Add(label, 0, 1);
#else
                tableLayoutPanel.Controls.Add(label, 0, 0);
                bShowHeader = false;
#endif
                nNumColumns++;

                // insert the vernacular representation of the testing question
                CtrlTextBox ctrlTextBoxVernacular = null;
                if (theSE.viewVernacularLangFieldMenuItem.Checked 
                    && theSE.StoryProject.ProjSettings.ShowTestQuestionsVernacular)
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.Vernacular.LangName, nNumColumns);
                    _aTQData.QuestionVernacular.Transliterator = ctrlVerse.TransliteratorVernacular;
                    ctrlTextBoxVernacular = InitTextBox(ctrlVerse, CstrFieldNameVernacular, _aTQData.QuestionVernacular,
                        theSE.StoryProject.ProjSettings.Vernacular, nNumColumns,
                        StoryEditor.TextFieldType.eVernacular);
                    nNumColumns++;
                }

#if !LimitWhenToShow
                CtrlTextBox ctrlTextBoxNationalBT = null;
                if (theSE.viewNationalLangFieldMenuItem.Checked
                    && theSE.StoryProject.ProjSettings.ShowTestQuestionsNationalBT)
#else
                // the only time we show the National BT is if there's an "other" English BTr (who will
                //  do the EnglishBT from the NationalBT) OR there's no vernacular
                if (theSE.StoryProject.ProjSettings.NationalBT.HasData 
                    && theSE.StoryProject.TeamMembers.HasOutsideEnglishBTer
                    && theSE.viewNationalLangFieldMenuItem.Checked)
#endif
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.NationalBT.LangName, nNumColumns);
                    _aTQData.QuestionNationalBT.Transliterator = ctrlVerse.TransliteratorNationalBT;
                    ctrlTextBoxNationalBT = InitTextBox(ctrlVerse, CstrFieldNameNationalBt, 
                        _aTQData.QuestionNationalBT, 
                        theSE.StoryProject.ProjSettings.NationalBT, nNumColumns,
                        StoryEditor.TextFieldType.eNational);
                    nNumColumns++;

                    if (ctrlTextBoxVernacular != null)
                        ctrlTextBoxVernacular.NationalBtSibling = ctrlTextBoxNationalBT;
                }

#if !LimitWhenToShow
                if (theSE.viewEnglishBTFieldMenuItem.Checked
                    && theSE.StoryProject.ProjSettings.ShowTestQuestionsInternationalBT)
#else
                if (theSE.viewEnglishBTFieldMenuItem.Checked
                    && (!theSE.StoryProject.TeamMembers.HasOutsideEnglishBTer
                        || (StageLogic.MemberTypeWithEditToken !=
                                TeamMemberData.UserTypes.eProjectFacilitator)
                                || (theSE.LoggedOnMember.MemberType != TeamMemberData.UserTypes.eProjectFacilitator)))
#endif
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.InternationalBT.LangName, nNumColumns);
                    CtrlTextBox ctrlTextBoxEnglishBT = InitTextBox(ctrlVerse, CstrFieldNameVernacular, _aTQData.QuestionInternationalBT,
                        theSE.StoryProject.ProjSettings.InternationalBT, nNumColumns,
                        StoryEditor.TextFieldType.eInternational);
                    nNumColumns++;

                    if (ctrlTextBoxVernacular != null)
                        ctrlTextBoxVernacular.EnglishBtSibling = ctrlTextBoxEnglishBT;

                    if (ctrlTextBoxNationalBT != null)
                        ctrlTextBoxNationalBT.EnglishBtSibling = ctrlTextBoxEnglishBT;
                }
            }

            // add a row so we can display a multiple line control with the answers
            if (theSE.viewStoryTestingQuestionAnswerMenuItem.Checked 
                && (_aTQData.Answers != null) && (_aTQData.Answers.Count > 0))
            {
                System.Diagnostics.Debug.Assert(theSE.theCurrentStory.CraftingInfo.Testors.Count >= _aTQData.Answers.Count);
                MultiLineControl aAnswersCtrl = new MultiLineControl(ctrlVerse, StageLogic,
                    _aTQData.Answers, theSE.StoryProject.ProjSettings,
                    theSE.theCurrentStory.CraftingInfo.Testors,
                    theSE.StoryProject.ProjSettings.ShowAnswersVernacular,
                    theSE.StoryProject.ProjSettings.ShowAnswersNationalBT,
                    theSE.StoryProject.ProjSettings.ShowAnswersInternationalBT);
                aAnswersCtrl.Name = CstrFieldNameAnswers;
                aAnswersCtrl.ParentControl = this;

#if ShowLabelRow
                const int nLayoutRow = 2;
#else
                const int nLayoutRow = 1;
#endif
                InsertRow(nLayoutRow);
                if (nNumColumns > 1)
                    tableLayoutPanel.SetColumnSpan(aAnswersCtrl, nNumColumns);
                tableLayoutPanel.Controls.Add(aAnswersCtrl, 0, nLayoutRow);
                tableLayoutPanel.DumpTable();
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected void InitColumnLabel(string strTestQuestionLangLableName, int nLayoutColumn)
        {
#if !ShowLabelRow
            System.Diagnostics.Debug.Assert(false);
#endif
            // add the row0 column label
            Label lbl = new Label
                            {
                                Name = strTestQuestionLangLableName + CstrSuffixLabel,
                                Anchor = AnchorStyles.Top,
                                AutoSize = true,
                                Text = strTestQuestionLangLableName
                            };
            tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected CtrlTextBox InitTextBox(VerseControl ctrlVerse, string strTbName, 
            StringTransfer strTbText, ProjectSettings.LanguageInfo li, int nLayoutColumn, 
            StoryEditor.TextFieldType eFieldtype)
        {
            var tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, ctrlVerse, this,
                strTbText, li, CstrTestQuestionsLabelFormat, true, eFieldtype);
#if ShowLabelRow
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
#else
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 0);
#endif
            return tb;
        }
    }
}
