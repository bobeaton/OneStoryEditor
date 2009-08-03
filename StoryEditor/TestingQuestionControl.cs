using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TestingQuestionControl : OneStoryProjectEditor.ResizableControl
    {
        protected const string cstrFieldNameVernacular = "TQVernacular";
        protected const string cstrFieldNameInternationalBT = "TQInternationalBT";
        protected const string cstrFieldNameAnswers = "Answers";

        protected StoryProject.TestQuestionRow m_aTQRow = null;
        protected int m_nNumAnswerRows = 0;

        public TestingQuestionControl(StoryEditor aSE, StoryProject.TestQuestionRow aTQRow)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            m_aTQRow = aTQRow;

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
            System.Diagnostics.Debug.Assert(m_aTQRow.GetTQVernacularRows().Length > 0, "otherwise, fix bad assumption: TestingQuestionControl.cs.31");
            InsertColumn(nNumColumns);
            StoryProject.TQVernacularRow aTQVRow = m_aTQRow.GetTQVernacularRows()[0];
            InitColumnLabel(aTQVRow.lang, nNumColumns);
            InitTextBox(cstrFieldNameVernacular, aTQVRow.TQVernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, nNumColumns);
            
            nNumColumns++;

            // insert the EnglishBT representation of the testing question
            System.Diagnostics.Debug.Assert(m_aTQRow.GetTQInternationalBTRows().Length > 0, "otherwise, fix bad assumption: TestingQuestionControl.cs.37");
            InsertColumn(nNumColumns);
            StoryProject.TQInternationalBTRow aTQERow = m_aTQRow.GetTQInternationalBTRows()[0];
            InitColumnLabel(aTQERow.lang, nNumColumns);
            InitTextBox(cstrFieldNameVernacular, aTQERow.TQInternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, nNumColumns);

            // add a row so we can display a multiple line control with the answers
            StoryProject.AnswersRow[] anAsRows = m_aTQRow.GetAnswersRows();
            if ((anAsRows != null) && (anAsRows.Length > 0))
            {
                MultiLineControl aAnswersCtrl = new MultiLineControl(new AnswersData(anAsRows[0]));
                aAnswersCtrl.Name = cstrFieldNameAnswers;
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
            lbl.Name = strTestQuestionLangLableName + cstrSuffixLabel;
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lbl.AutoSize = true;
            lbl.Text = strTestQuestionLangLableName;
            this.tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(string strTbName, string strTbText, Font font, Color color, int nLayoutColumn)
        {
            TextBox tb = new TextBox();
            tb.Name = strTbName + cstrSuffixTextBox;
            tb.Multiline = true;
            tb.Font = font;
            tb.ForeColor = color;
            tb.Dock = DockStyle.Fill;
            tb.Text = strTbText;
            tb.TextChanged += new EventHandler(textBox_TextChanged);
            this.tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
