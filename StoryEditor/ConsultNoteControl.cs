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
        internal ConsultNoteDataConverter _myCNDC = null;
        internal ConsultNotesDataConverter _myCollection = null;

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
            foreach (CommInstance aCI in aCNDC)
                if ((aCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eConsultantToCrafter)
                    || (aCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant))
                    InitRow(aCNDC.MentorLabel, aCI, aCNDC.CommentColor, aCNDC.MentorRequiredEditor, ref nNumRows);
                else
                    InitRow(aCNDC.MenteeLabel, aCI, aCNDC.ResponseColor, aCNDC.MenteeRequiredEditor, ref nNumRows);

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
            if (_myCNDC.HasData)
                if (MessageBox.Show("Are you sure you want to delete this note?",  StoriesData.CstrCaption, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

            _myCollection.Remove(_myCNDC);
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitConsultNotesPane(_myCollection);
        }

        void buttonDragDropHandle_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.HandleQueryContinueDrag((ConsultNotesControl)ParentControl, e);
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                buttonDragDropHandle.DoDragDrop(this, DragDropEffects.Move);
        }
    }
}
