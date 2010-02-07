using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConsultNotesControl : VerseControl
    {
        protected const string CstrFieldNameConsultantNote = "ConsultantNoteControl";
        protected const string CstrZerothVerseName = "Story:";

        internal ConsultNotesDataConverter _theCNsDC = null;
        public ConsultNotesControl(StoryEditor theSE, 
            LineFlowLayoutPanel parentFlowLayoutPanel, 
            StoryStageLogic storyStageLogic, 
            ConsultNotesDataConverter aCNsDC, int nVerseNumber, 
            TeamMemberData.UserTypes eLoggedOnMemberType)
            : base(storyStageLogic, nVerseNumber, theSE, parentFlowLayoutPanel)
        {
            _theCNsDC = aCNsDC;
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            labelReference.Text = (VerseNumber == 0) ? CstrZerothVerseName : CstrVerseName + VerseNumber;
            tableLayoutPanel.Controls.Add(labelReference, 0, 0);
            tableLayoutPanel.Controls.Add(buttonDragDropHandle, 1, 0);

            if (aCNsDC.Count > 0)
            {
                int nRowIndex = 1;
                foreach (ConsultNoteDataConverter aCNDC in aCNsDC)
                {
                    if (aCNDC.Visible || theSE.hiddenVersesToolStripMenuItem.Checked)
                    {
                        ConsultNoteControl aCNCtrl = new ConsultNoteControl(this, storyStageLogic, aCNsDC, aCNDC, eLoggedOnMemberType);
                        aCNCtrl.Name = CstrFieldNameConsultantNote + nRowIndex;
                        aCNCtrl.ParentControl = this;

                        InsertRow(nRowIndex);
                        tableLayoutPanel.SetColumnSpan(aCNCtrl, 2);
                        tableLayoutPanel.Controls.Add(aCNCtrl, 0, nRowIndex);
                        nRowIndex++;
                    }
                }
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public new bool Focus()
        {
            base.Focus();
            return labelReference.Focus();
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            StringTransfer st = DoAddNote(null);
            if ((st != null) && (st.TextBox != null))
                st.TextBox.Focus();
        }

        private void addNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringTransfer st = DoAddNote(null);
            if ((st != null) && (st.TextBox != null))
                st.TextBox.Focus();
        }

        public StringTransfer DoAddNote(string strNote)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return null;

            // if we're not given anything to put in the box, at least put in the logged
            //  in member's initials and re
            if (String.IsNullOrEmpty(strNote) && (theSE.LoggedOnMember != null))
                strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + ": Re: ";

            // if the coach tries to add a note in the consultant's pane, that should fail.
            // (but it's okay for a project facilitator to add one if they have a question
            //  for the consultant)
            if (!_theCNsDC.CheckAddNotePrivilege(theSE, theSE.LoggedOnMember.MemberType))
                return null;

            StoryStageLogic.ProjectStages eCurState = theSE.theCurrentStory.ProjStage.ProjectStage;
            int round = 1;
            if (eCurState > StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant)
            {
                round = 2;
                if (eCurState > StoryStageLogic.ProjectStages.eProjFacOnlineReview2WithConsultant)
                    round = 3;
            }

            ConsultNoteDataConverter cndc = 
                _theCNsDC.Add(round, theSE.LoggedOnMember.MemberType, strNote);
            System.Diagnostics.Debug.Assert(cndc.Count == 1);

            theSE.ReInitConsultNotesPane(_theCNsDC);

            // return the StringTransfer we just created
            return cndc[0];
        }

        void buttonDragDropHandle_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ConsultNoteControl)))
            {
                ConsultNoteControl theCNC = (ConsultNoteControl)e.Data.GetData(typeof(ConsultNoteControl));
                System.Diagnostics.Debug.Assert((sender is Button) && (sender == buttonDragDropHandle) && (theCNC.ParentControl != null) && (theCNC.ParentControl is ConsultNotesControl));
                ConsultNoteDataConverter theMovingCNDC = theCNC._myCNDC;
                theCNC._myCollection.Remove(theMovingCNDC);
                _theCNsDC.Insert(0, theMovingCNDC);

                StoryEditor theSE = (StoryEditor)FindForm();
                theSE.ReInitConsultNotesPane(_theCNsDC);
            }
        }

        void buttonDragDropHandle_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ConsultNoteControl)))
                e.Effect = DragDropEffects.Move;
        }
    }
}
