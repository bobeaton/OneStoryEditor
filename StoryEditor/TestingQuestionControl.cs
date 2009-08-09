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
            InitColumnLabel(aSE.Stories.ProjSettings.VernacularLangName, nNumColumns);
            InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionVernacular, aSE.Stories.ProjSettings.VernacularFont, aSE.Stories.ProjSettings.VernacularFontColor, nNumColumns);
            
            nNumColumns++;

            // insert the EnglishBT representation of the testing question
            InsertColumn(nNumColumns);
            InitColumnLabel(aSE.Stories.ProjSettings.InternationalBTLangName, nNumColumns);
            InitTextBox(CstrFieldNameVernacular, _aTQData.QuestionEnglish, aSE.Stories.ProjSettings.InternationalBTFont, aSE.Stories.ProjSettings.InternationalBTFontColor, nNumColumns);

            // add a row so we can display a multiple line control with the answers
            if ((_aTQData.Answers != null) && (_aTQData.Answers.Count > 0))
            {
                MultiLineControl aAnswersCtrl = new MultiLineControl(_aTQData.Answers);
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
            TextBox tb = new TextBox();
            tb.Name = strTbName + CstrSuffixTextBox;
            tb.Multiline = true;
            tb.Font = font;
            tb.ForeColor = color;
            tb.Dock = DockStyle.Fill;
            strTbText.SetAssociation(tb);   // tb.Text = strTbText;
            tb.TextChanged += new EventHandler(textBox_TextChanged);
            this.tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
