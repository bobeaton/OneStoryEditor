using System;
using System.Drawing;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TestingQuestionControl : ResizableControl
    {
        protected const string CstrFieldNameVernacular = "TQVernacular";
        protected const string CstrFieldNameInternationalBt = "TQInternationalBT";
        protected const string CstrFieldNameAnswers = "Answers";

        protected TestQuestionData _aTQData = null;
        protected int _nNumAnswerRows = 0;

        public TestingQuestionControl(StoryEditor aSE, TestQuestionData aTQData)
            : base(aSE.theCurrentStory.ProjStage)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            _aTQData = aTQData;

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

            // clobber the base class table layout panel's configuration. We're 'column-oriented' instead
            // first add another row so that we have two rows (row(0)=label, row(1)=text)
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here: TestingQuestionControl.cs.34");
            InsertRow(1);

            // remove the columns, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 0)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            UpdateView(aSE);

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public override void UpdateView(StoryEditor aSE)
        {
            int nNumColumns = 0;

            // insert the vernacular representation of the testing question
            InsertColumn(nNumColumns);
            InitColumnLabel(aSE.Stories.ProjSettings.Vernacular.LangName, nNumColumns);
            InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionVernacular, aSE.Stories.ProjSettings.Vernacular.Font, aSE.Stories.ProjSettings.Vernacular.FontColor, nNumColumns);
            
            nNumColumns++;

            // insert the EnglishBT representation of the testing question
            InsertColumn(nNumColumns);
            InitColumnLabel(aSE.Stories.ProjSettings.InternationalBT.LangName, nNumColumns);
            InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionEnglish, aSE.Stories.ProjSettings.InternationalBT.Font, aSE.Stories.ProjSettings.InternationalBT.FontColor, nNumColumns);

            // add a row so we can display a multiple line control with the answers
            if ((_aTQData.Answers != null) && (_aTQData.Answers.Count > 0))
            {
                MultiLineControl aAnswersCtrl = new MultiLineControl(StageLogic, _aTQData.Answers);
                aAnswersCtrl.Name = CstrFieldNameAnswers;
                aAnswersCtrl.ParentControl = this;

                int nLayoutRow = 2;
                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aAnswersCtrl, 2);
                tableLayoutPanel.Controls.Add(aAnswersCtrl, 0, nLayoutRow);
            }
        }

        protected void InitColumnLabel(string strTestQuestionLangLableName, int nLayoutColumn)
        {
            // add the row0 column label
            Label lbl = new Label();
            lbl.Name = strTestQuestionLangLableName + CstrSuffixLabel;
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lbl.AutoSize = true;
            lbl.Text = strTestQuestionLangLableName;
            this.tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(string strTbName, StringTransfer strTbText, Font font, Color color, int nLayoutColumn)
        {
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, this, strTbText, font, color);
            this.tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
