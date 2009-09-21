using System;
using System.Drawing;
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

        public TestingQuestionControl(StoryEditor theSE, TestQuestionData aTQData)
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

            tableLayoutPanel.DumpTable();

            // show the row label
            Label label = new Label
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Name = CstrFieldNameTestQuestionsLabel,
                Text = CstrTestQuestionsLabelFormat
            };
            tableLayoutPanel.Controls.Add(label, 0, 1);

            // the Crafter definitely wants to see both, so even if the vern button is off, 
            //  show the vern for the testing question--except for the coach
            bool bShowVernAndShowHeaders = (theSE.viewVernacularLangFieldMenuItem.Checked
                || (StageLogic.MemberTypeWithEditToken != TeamMemberData.UserTypes.eCoach));

            int nNumColumns = 1;

            // insert the vernacular representation of the testing question
            if (bShowVernAndShowHeaders)
            {
                InsertColumn(nNumColumns);
                InitColumnLabel(theSE.Stories.ProjSettings.Vernacular.LangName, nNumColumns);
                InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionVernacular, 
                    theSE.Stories.ProjSettings.Vernacular, nNumColumns);
                nNumColumns++;
            }

            // the only time we show the National BT is if there's an "other" English BTr (who will
            //  do the EnglishBT from the NationalBT) AND only if there *is* a national BT
            if (theSE.Stories.ProjSettings.NationalBT.HasData && 
                theSE.Stories.TeamMembers.IsThereASeparateEnglishBackTranslator)
            {
                InsertColumn(nNumColumns);
                if (bShowVernAndShowHeaders)    // but no need to show the lang title header unless there are two
                    InitColumnLabel(theSE.Stories.ProjSettings.NationalBT.LangName, nNumColumns);
                InitTextBox(CstrFieldNameNationalBt, _aTQData.QuestionNationalBT, theSE.Stories.ProjSettings.NationalBT, nNumColumns);
                nNumColumns++;
            }

            if (theSE.Stories.ProjSettings.InternationalBT.HasData
                && (!theSE.Stories.TeamMembers.IsThereASeparateEnglishBackTranslator
                    || (StageLogic.MemberTypeWithEditToken ==
                            TeamMemberData.UserTypes.eEnglishBacktranslator)))
            {
                InsertColumn(nNumColumns);
                if (bShowVernAndShowHeaders)    // but no need to show the lang title header unless there are two
                    InitColumnLabel(theSE.Stories.ProjSettings.InternationalBT.LangName, nNumColumns);
                InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionInternationalBT, theSE.Stories.ProjSettings.InternationalBT, nNumColumns);
            }

            // add a row so we can display a multiple line control with the answers
            if ((_aTQData.Answers != null) && (_aTQData.Answers.Count > 0))
            {
                MultiLineControl aAnswersCtrl = new MultiLineControl(StageLogic, _aTQData.Answers);
                aAnswersCtrl.Name = CstrFieldNameAnswers;
                aAnswersCtrl.ParentControl = this;

                const int nLayoutRow = 2;
                InsertRow(nLayoutRow);
                if (nNumColumns > 1)
                    tableLayoutPanel.SetColumnSpan(aAnswersCtrl, 2);
                tableLayoutPanel.Controls.Add(aAnswersCtrl, 1, nLayoutRow);
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
                                Anchor = System.Windows.Forms.AnchorStyles.Top,
                                AutoSize = true,
                                Text = strTestQuestionLangLableName
                            };
            tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(string strTbName, StringTransfer strTbText, ProjectSettings.LanguageInfo li, int nLayoutColumn)
        {
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, this, strTbText, li);
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
