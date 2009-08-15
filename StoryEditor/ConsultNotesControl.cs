using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConsultNotesControl : OneStoryProjectEditor.ResizableControl
    {
        protected const string CstrFieldNameConsultantNote = "ConsultantNoteControl";

        internal int VerseNumber = -1;
        protected ConsultNotesDataConverter _theCNsDC = null;
        public ConsultNotesControl(StoryStageLogic storyStageLogic, ConsultNotesDataConverter aCNsDC, int nVerseNumber)
            : base(storyStageLogic)
        {
            _theCNsDC = aCNsDC;
            VerseNumber = nVerseNumber;
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            labelReference.Text = VerseBtControl.CstrVerseName + nVerseNumber.ToString();
            tableLayoutPanel.Controls.Add(labelReference, 0, 0);
            tableLayoutPanel.Controls.Add(buttonDragDropHandle, 1, 0);

            if (aCNsDC.Count > 0)
            {
                int nRowIndex = 1;
                foreach (ConsultNoteDataConverter aCNDC in aCNsDC)
                {
                    if (aCNDC.Visible || showhiddenNotesToolStripMenuItem.Checked)
                    {
                        ConsultNoteControl aCNCtrl = new ConsultNoteControl(storyStageLogic, aCNsDC, aCNDC);
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

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            DoAddNote();
        }

        private void addNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAddNote();
        }

        protected void DoAddNote()
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE = (StoryEditor)FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        "Unable to edit the file! Reboot and if it persists, contact bob_eaton@sall.com");

                if (!theSE.theCurrentStory.ProjStage.IsEditAllowed(theSE.LoggedOnMember))
                    throw theSE.theCurrentStory.ProjStage.WrongMemberTypeEx;
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message), null);
                return;
            }

            StoryStageLogic.ProjectStages eCurState = theSE.theCurrentStory.ProjStage.ProjectStage;
            int round = 1;
            if (eCurState > StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
            {
                round = 2;
                if (eCurState > StoryStageLogic.ProjectStages.eConsultantReviseRound2Notes)
                    round = 3;
            }
            _theCNsDC.AddEmpty(round);
            theSE.ReInitConsultNotesPane(_theCNsDC);
        }

        private void showhiddenNotesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitConsultNotesPane(_theCNsDC);
        }
    }
}
