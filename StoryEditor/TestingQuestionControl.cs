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
            InsertRow(1);

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
                tableLayoutPanel.Controls.Add(label, 0, 1);
                nNumColumns++;

                // insert the vernacular representation of the testing question
                if (theSE.viewVernacularLangFieldMenuItem.Checked)
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.Vernacular.LangName, nNumColumns);
                    _aTQData.QuestionVernacular.Transliterator = ctrlVerse.TransliteratorVernacular;
                    InitTextBox(ctrlVerse, CstrFieldNameVernacular, _aTQData.QuestionVernacular,
                        theSE.StoryProject.ProjSettings.Vernacular, nNumColumns);
                    nNumColumns++;
                }

                // the only time we show the National BT is if there's an "other" English BTr (who will
                //  do the EnglishBT from the NationalBT) OR there's no vernacular
                if (theSE.StoryProject.ProjSettings.NationalBT.HasData 
                    && theSE.StoryProject.TeamMembers.HasOutsideEnglishBTer
                    && theSE.viewNationalLangFieldMenuItem.Checked)
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.NationalBT.LangName, nNumColumns);
                    _aTQData.QuestionNationalBT.Transliterator = ctrlVerse.TransliteratorNationalBT;
                    InitTextBox(ctrlVerse, CstrFieldNameNationalBt, _aTQData.QuestionNationalBT,
                        theSE.StoryProject.ProjSettings.NationalBT, nNumColumns);
                    nNumColumns++;
                }

                if (theSE.viewEnglishBTFieldMenuItem.Checked
                    && (!theSE.StoryProject.TeamMembers.HasOutsideEnglishBTer
                        || (StageLogic.MemberTypeWithEditToken !=
                                TeamMemberData.UserTypes.eProjectFacilitator)
                                || (theSE.LoggedOnMember.MemberType != TeamMemberData.UserTypes.eProjectFacilitator)))
                {
                    InsertColumn(nNumColumns);
                    if (bShowHeader)
                        InitColumnLabel(theSE.StoryProject.ProjSettings.InternationalBT.LangName, nNumColumns);
                    InitTextBox(ctrlVerse, CstrFieldNameVernacular, _aTQData.QuestionInternationalBT,
                        theSE.StoryProject.ProjSettings.InternationalBT, nNumColumns);
                    nNumColumns++;
                }
            }

            // add a row so we can display a multiple line control with the answers
            if (theSE.viewStoryTestingQuestionAnswerMenuItem.Checked 
                && (_aTQData.Answers != null) && (_aTQData.Answers.Count > 0))
            {
                System.Diagnostics.Debug.Assert(theSE.theCurrentStory.CraftingInfo.Testors.Count >= _aTQData.Answers.Count);
                MultiLineControl aAnswersCtrl = new MultiLineControl(ctrlVerse, StageLogic,
                    _aTQData.Answers, theSE.StoryProject.ProjSettings.InternationalBT,
                    theSE.theCurrentStory.CraftingInfo.Testors);
                aAnswersCtrl.Name = CstrFieldNameAnswers;
                aAnswersCtrl.ParentControl = this;

                const int nLayoutRow = 2;
                InsertRow(nLayoutRow);
                if (nNumColumns > 2)
                    tableLayoutPanel.SetColumnSpan(aAnswersCtrl, nNumColumns - 1);
                tableLayoutPanel.Controls.Add(aAnswersCtrl, 1, nLayoutRow);
                tableLayoutPanel.DumpTable();
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected void InitColumnLabel(string strTestQuestionLangLableName, int nLayoutColumn)
        {
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

        protected void InitTextBox(VerseControl ctrlVerse, string strTbName, 
            StringTransfer strTbText, ProjectSettings.LanguageInfo li, int nLayoutColumn)
        {
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, ctrlVerse, this,
                strTbText, li, CstrTestQuestionsLabelFormat, true);
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
