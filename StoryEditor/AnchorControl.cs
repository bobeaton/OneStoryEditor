using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class AnchorControl : ResizableControl
    {
        protected const string CstrFieldNameAnchor = "AnchorButton";
        protected int m_nNumRows = 1;
        protected VerseControl _ctrlVerse;
        protected AnchorsData _myAnchorsData;
        // protected Dictionary<ToolStripButton, List<TextBox>> _mapAnchorsToTextBoxes = new Dictionary<ToolStripButton, List<TextBox>>();

        public AnchorControl(VerseControl ctrlVerse, StoryStageLogic storyStageLogic,
            AnchorsData anAnchorsData)
            : base(storyStageLogic)
        {
            _ctrlVerse = ctrlVerse;
            _myAnchorsData = anAnchorsData;
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            tableLayoutPanel.Controls.Add(labelAnchor, 0, 0);
            tableLayoutPanel.Controls.Add(toolStripAnchors, 1, 0);

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            foreach (AnchorData anAnchorData in anAnchorsData)
                InitAnchorButton(toolStripAnchors, anAnchorData);

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected ToolStripButton InitAnchorButton(ToolStrip ts, AnchorData theAnchorData)
        {
            string strText = theAnchorData.JumpTarget;
            if (!String.IsNullOrEmpty(theAnchorData.ToolTipText)
                && (theAnchorData.JumpTarget != theAnchorData.ToolTipText))
            {
                strText += AnchorData.CstrTooltipIndicator; // give an indication that there's a tooltip
            }

            var aButton = new ToolStripButton
                              {
                                  DisplayStyle = ToolStripItemDisplayStyle.Text,
                                  Tag = theAnchorData,
                                  Name = CstrFieldNameAnchor + theAnchorData.JumpTarget,
                                  Text = strText,
                                  ToolTipText = theAnchorData.ToolTipText,
                                  Margin = new Padding(0)
                              };
            aButton.Click += aButton_Click;
            aButton.MouseDown += aButton_MouseDown;
            ts.Items.Add(aButton);
            aButton.Height = 18;
            return aButton;
        }

        protected ToolStripButton m_theLastButtonClicked = null;

        void aButton_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.Assert(sender is ToolStripButton);
            if (e.Button == MouseButtons.Right)
                m_theLastButtonClicked = (ToolStripButton)sender;
        }

        void aButton_Click(object sender, EventArgs e)
        {
            Form form = FindForm();
            if ((form != null) && (form is StoryEditor))
            {
                var aSE = (StoryEditor)form;
                var tssb = (ToolStripButton)sender;

                // if this is the null anchor, then do nothing
                if (tssb.Text == CstrNullAnchor)
                    return;

                // the button may have the extra indicator that there's a tooltip.
                string strJumpTarget = tssb.Text;
                int nIndLen = AnchorData.CstrTooltipIndicator.Length;
                if (strJumpTarget.Substring(strJumpTarget.Length - nIndLen, nIndLen) == AnchorData.CstrTooltipIndicator)
                    strJumpTarget = strJumpTarget.Substring(0, strJumpTarget.Length - nIndLen);

                aSE.SetNetBibleVerse(strJumpTarget);
                aSE.FocusOnVerse(_ctrlVerse.VerseNumber, true, true);
                
                // if we aren't already in some text box, then set the focus on the 
                //  parent verse form so that scroll wheel can work
                if (CtrlTextBox._inTextBox == null)
                    _ctrlVerse.Focus();
            }
        }

        private void toolStripAnchors_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                if (TeamMemberData.IsUser(theSE.LoggedOnMember.MemberType,
                                          TeamMemberData.UserTypes.ProjectFacilitator)
                    && !TasksPf.IsTaskOn(theSE.theCurrentStory.TasksAllowedPf, 
                                         TasksPf.TaskSettings.Anchors))
                {
                    MessageBox.Show(Properties.Resources.IDS_CantAddAnchors,
                                    OseResources.Properties.Resources.IDS_Caption);
                    return;
                }

                var theNetBibleViewer = (NetBibleViewer)e.Data.GetData(typeof(NetBibleViewer));
                if (toolStripAnchors.Items.Cast<ToolStripButton>().Any(btn => btn.Text == theNetBibleViewer.ScriptureReference))
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
                e.Effect = DragDropEffects.Link;
            }
        }

        private void toolStripAnchors_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                NetBibleViewer theNetBibleViewer = (NetBibleViewer)e.Data.GetData(typeof(NetBibleViewer));
                AnchorData theAnchorData = _myAnchorsData.AddAnchorData(theNetBibleViewer.ScriptureReference, theNetBibleViewer.ScriptureReference);
                InitAnchorButton(toolStripAnchors, theAnchorData);

                // indicate that we've changed something so that we don't exit without offering
                //  to save.
                theSE.Modified = true;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_theLastButtonClicked != null)
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                /*
                if (_mapAnchorsToTextBoxes.ContainsKey(m_theLastButtonClicked))
                {
                    DialogResult res = MessageBox.Show(String.Format("The anchor you are about to delete has exegetical or cultural note(s) attached to it. These will be deleted also. Click 'OK' to continue with the deletion.{0}{0}[if you would rather have kept them, say associated to another anchor, then tell bob_eaton@sall.com and he may implement that feature. For now, you can copy the note and paste it into a new note added to a new or existing anchor (right-click on the anchor and choose 'Add Exegetical/Cultural Note'). Then come back here and delete this anchor]", Environment.NewLine),  OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.OKCancel);
                    if (res != DialogResult.OK)
                        return;

                    List<TextBox> lstTextBoxes = _mapAnchorsToTextBoxes[m_theLastButtonClicked];
                    foreach (TextBox tb in lstTextBoxes)
                    {
                        int nRowIndex = tableLayoutPanel.GetRow(tb);
                        RemoveRow(nRowIndex);
                        m_nNumRows--;   // I think it's enough to just decrement these counts
                    }
                    _mapAnchorsToTextBoxes.Remove(m_theLastButtonClicked);  // I think these will be cleaned up now
                    AdjustHeightWithSuspendLayout(null);
                }
                */
                toolStripAnchors.Items.RemoveByKey(m_theLastButtonClicked.Name);
                Debug.Assert((m_theLastButtonClicked.Tag != null) && (m_theLastButtonClicked.Tag is AnchorData));
                AnchorData theAnchorData = (AnchorData)m_theLastButtonClicked.Tag;
                _myAnchorsData.Remove(theAnchorData);
                m_theLastButtonClicked = null;

                // indicate that we've changed something so that we don't exit without offering
                //  to save.
                theSE.Modified = true;
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to delete",  OseResources.Properties.Resources.IDS_Caption);
        }

        private void addCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_theLastButtonClicked != null)
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;
                Debug.Assert(m_theLastButtonClicked.Tag is AnchorData);

                AnchorAddCommentForm dlg = new AnchorAddCommentForm(m_theLastButtonClicked.Text, m_theLastButtonClicked.ToolTipText);
                DialogResult res = dlg.ShowDialog();
                if ((res == DialogResult.OK) || (res == DialogResult.Yes))
                {
                    Debug.Assert((m_theLastButtonClicked.Tag != null) && (m_theLastButtonClicked.Tag is AnchorData));
                    AnchorData theAnchorData = (AnchorData)m_theLastButtonClicked.Tag;
                    theAnchorData.ToolTipText = m_theLastButtonClicked.ToolTipText = dlg.CommentText;

                    string strJumpTarget = m_theLastButtonClicked.Text;
                    int nIndLen = AnchorData.CstrTooltipIndicator.Length;
                    if (strJumpTarget.Substring(strJumpTarget.Length - nIndLen, nIndLen) != AnchorData.CstrTooltipIndicator)
                        m_theLastButtonClicked.Text += AnchorData.CstrTooltipIndicator;

                    theSE.Modified = true;
                }
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to",  OseResources.Properties.Resources.IDS_Caption);
        }

        private void toolStripAnchors_MouseDown(object sender, MouseEventArgs e)
        {
            m_theLastButtonClicked = null;
        }

        /*
        private void addExegeticalCulturalNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            ExegeticalHelpNoteData anEHN = _myExegeticalHelpNotes.AddExegeticalHelpNote("Re: " + ((m_theLastButtonClicked != null) ? m_theLastButtonClicked.Text : null));
            SetExegeticalHelpControls(_ctrlVerse, _li, anEHN, ref m_nNumRows);
            AdjustHeightWithSuspendLayout(null);
            theSE.Modified = true;
        }
        */

        internal static BiblicalKeyTermsForm m_dlgKeyTerms = null;

        private void editKeyTermsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            try
            {
                // if the language code changed (such that we'll need to build a new key terms db)
                ProjectSettings.LanguageInfo liMainLang = WhichLangInfoToUseForKeyTerms;
                if ((m_dlgKeyTerms != null) &&
                    ((m_dlgKeyTerms.MainLang.LangCode != liMainLang.LangCode)
                    || (m_dlgKeyTerms._projSettings.ProjectFolder != theSE.StoryProject.ProjSettings.ProjectFolder)))
                {
                    m_dlgKeyTerms = null;
                }

                if (m_dlgKeyTerms == null)
                {
                    Debug.Assert(theSE.StoryProject.ProjSettings.Vernacular.HasData
                        || theSE.StoryProject.ProjSettings.NationalBT.HasData
                        || theSE.StoryProject.ProjSettings.InternationalBT.HasData);

                    m_dlgKeyTerms = new BiblicalKeyTermsForm(theSE, theSE.StoryProject.ProjSettings, liMainLang);
                }
                m_dlgKeyTerms.Show(_myAnchorsData, theSE.StoryProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_KeyTermsProblem,
                    Environment.NewLine, ex.Message), OseResources.Properties.Resources.IDS_Caption);
                return;
            }
        }

        protected ProjectSettings.LanguageInfo WhichLangInfoToUseForKeyTerms
        {
            get
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return null;

                ProjectSettings.LanguageInfo liToUse;
                if (theSE.StoryProject.ProjSettings.Vernacular.HasData)
                    liToUse = theSE.StoryProject.ProjSettings.Vernacular;
                else if (theSE.StoryProject.ProjSettings.NationalBT.HasData)
                    liToUse = theSE.StoryProject.ProjSettings.NationalBT;
                else
                    liToUse = theSE.StoryProject.ProjSettings.InternationalBT;
                return liToUse;
            }
        }

        private void addConsultantCoachNoteOnThisAnchorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            if (m_theLastButtonClicked != null)
            {
                Debug.Assert(theSE.LoggedOnMember != null);
                string strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + ": Re: anc: ";
                strNote += m_theLastButtonClicked.Text;

                if (m_theLastButtonClicked.ToolTipText != m_theLastButtonClicked.Text)
                    strNote += String.Format(" ({0})", m_theLastButtonClicked.ToolTipText);

                strNote += ". ";

                theSE.SendNoteToCorrectPane(_ctrlVerse.VerseNumber, strNote, false);
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to", OseResources.Properties.Resources.IDS_Caption);
        }

        private const string CstrNullAnchor = "No Anchor";

        private void insertNullAnchorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            AnchorData theAnchorData = _myAnchorsData.AddAnchorData(CstrNullAnchor, CstrNullAnchor);
            InitAnchorButton(toolStripAnchors, theAnchorData);

            // indicate that we've changed something so that we don't exit without offering
            //  to save.
            theSE.Modified = true;
        }

        private void contextMenuStripAnchorOptions_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            insertNullAnchorToolStripMenuItem.Visible = (_myAnchorsData.Count == 0);
        }
    }
}
