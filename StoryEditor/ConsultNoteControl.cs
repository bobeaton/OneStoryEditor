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
        internal const string CstrRoundLabel = "Round: ";
        protected int m_nRoundNum = -1;
        internal ConsultNoteDataConverter _myCNDC = null;
        internal ConsultNotesDataConverter _myCollection = null;

        public ConsultNoteControl(VerseControl ctrlVerse, StoryStageLogic storyStageLogic, ConsultNotesDataConverter theCollection,
            ConsultNoteDataConverter aCNDC, TeamMemberData.UserTypes eLoggedOnMemberType)
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
            theCollection.InsureExtraBox(aCNDC, eLoggedOnMemberType);
            int nNumRows = 1;
            foreach (CommInstance aCI in aCNDC)
                if ((aCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eConsultantToProjFac)
                    || (aCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant))
                    InitRow(ctrlVerse, aCNDC.MentorLabel, aCI, aCNDC.CommentColor, aCNDC.ThrowIfWrongEditor,
                        aCNDC.MentorRequiredEditor, ref nNumRows);
                else
                    InitRow(ctrlVerse, aCNDC.MenteeLabel, aCI, aCNDC.ResponseColor, aCNDC.ThrowIfWrongEditor, 
                        aCNDC.MenteeRequiredEditor, ref nNumRows);

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected void InitRow(VerseControl ctrlVerse, string strRowLabel, StringTransfer strRowData, Color clrText,
            CtrlTextBox.ThrowIfNotCorrectEditor delegateCheckEditor, TeamMemberData.UserTypes eReqEditor, ref int nNumRows)
        {
            int nLayoutRow = nNumRows++;

            Label label = new Label();
            label.Anchor = AnchorStyles.Left;
            label.AutoSize = true;
            label.Name = strRowLabel + nNumRows;
            label.Text = strRowLabel;

            CtrlTextBox tb = new CtrlTextBox(
                strRowLabel + CstrSuffixTextBox + nNumRows,
                ctrlVerse, this, strRowData, delegateCheckEditor, eReqEditor);
            tb.ForeColor = clrText;

            // add the label and tool strip as a new row to the table layout panel
            InsertRow(nLayoutRow);
            tableLayoutPanel.Controls.Add(label, 0, nLayoutRow);
            tableLayoutPanel.SetColumnSpan(tb, 2);
            tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            _myCNDC.Visible = (_myCNDC.Visible) ? false : true;

            theSE.ReInitConsultNotesPane(_myCollection);
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            if (_myCNDC.HasData)
            {
                DialogResult res = MessageBox.Show(
                    Properties.Resources.IDS_NoteNotEmptyHideQuery,
                    Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    _myCNDC.Visible = false;
                    theSE.ReInitConsultNotesPane(_myCollection);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            _myCollection.Remove(_myCNDC);
            theSE.ReInitConsultNotesPane(_myCollection);
        }

        void buttonDragDropHandle_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.HandleQueryContinueDrag((ConsultNotesControl)ParentControl, e);
        }

        void buttonDragDropHandle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                buttonDragDropHandle.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        private void contextMenuStripNoteOptions_Opening(object sender, CancelEventArgs e)
        {
            if (_myCNDC.Visible)
                hideMenuItem.Text = "&Hide";
            else
                hideMenuItem.Text = "&Unhide";

            if (_myCNDC.IsFinished)
            {
                endConversationToolStripMenuItem.Text = "&Reopen conversation";
                endConversationToolStripMenuItem.ToolTipText = "Click to reopen the conversation";
            }
            else
            {
                endConversationToolStripMenuItem.Text = "&End conversation";
                endConversationToolStripMenuItem.ToolTipText = "Click to end this conversation";
            }
        }

        private void endConversationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            if (_myCNDC.IsFinished)
                _myCNDC.IsFinished = false;
            else
                _myCNDC.IsFinished = true;

            theSE.ReInitConsultNotesPane(_myCollection);
        }
    }
}
