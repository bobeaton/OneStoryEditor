using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConsultNoteControl : ResizableControl
    {
        protected const string CstrRoundLabel = "Round: ";
        protected int m_nRoundNum = -1;
        protected ConsultNoteDataConverter _myCNDC = null;
        protected ConsultNotesDataConverter _myCollection = null;

        public ConsultNoteControl(StoryStageLogic storyStageLogic, ConsultNotesDataConverter theCollection, ConsultNoteDataConverter aCNDC)
            : base(storyStageLogic)
        {
            _myCNDC = aCNDC;
            _myCollection = theCollection;
            m_nRoundNum = aCNDC.RoundNum;
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            InsertColumn(2);

            labelRound.Text = CstrRoundLabel + m_nRoundNum.ToString();
            tableLayoutPanel.SetColumnSpan(labelRound, 2);
            tableLayoutPanel.Controls.Add(labelRound, 0, 0);
            tableLayoutPanel.Controls.Add(buttonDragDropHandle, 1, 0);

            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, fix this assumption: ConsultNoteControl.cs.28");

            // finally populate the buttons on that tool strip
            int nNumRows = 1;
            InitRow(aCNDC.MentorLabel, aCNDC.MentorComment, aCNDC.CommentColor, aCNDC.MentorRequiredEditor, ref nNumRows);
            InitRow(aCNDC.MenteeLabel, aCNDC.MenteeResponse, aCNDC.ResponseColor, aCNDC.MenteeRequiredEditor, ref nNumRows);

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected void InitRow(string strRowLabel, StringTransfer strRowData, Color clrText, TeamMemberData.UserTypes eReqEditor, ref int nNumRows)
        {
            int nLayoutRow = nNumRows++;

            Label label = new Label();
            label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            label.AutoSize = true;
            label.Name = strRowLabel + nNumRows.ToString();
            label.Text = strRowLabel;

            CtrlTextBox tb = new CtrlTextBox(
                strRowLabel + CstrSuffixTextBox + nNumRows.ToString(),
                this, strRowData, eReqEditor);
            tb.ForeColor = clrText;

            // add the label and tool strip as a new row to the table layout panel
            InsertRow(nLayoutRow);
            tableLayoutPanel.Controls.Add(label, 0, nLayoutRow);
            tableLayoutPanel.SetColumnSpan(tb, 2);
            tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            _myCNDC.Visible = false;
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitConsultNotesPane(_myCollection);
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this note?", StoryEditor.CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                _myCollection.Remove(_myCNDC);
                StoryEditor theSE = (StoryEditor)FindForm();
                theSE.ReInitConsultNotesPane(_myCollection);
            }
        }
    }
}
