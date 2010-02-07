using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class AnchorControl : ResizableControl
    {
        protected const string CstrFieldNameAnchor = "AnchorButton";
        protected const string CstrFieldNameExegeticalHelp = "ExegeticalHelp";
        protected const string CstrFieldNameExegeticalHelpLabel = "ExegeticalHelpLabel";
        protected int m_nNumRows = 1;
        protected VerseControl _ctrlVerse = null;
        protected AnchorsData _myAnchorsData = null;
        protected Font _font;
        protected Dictionary<ToolStripButton, List<TextBox>> _mapAnchorsToTextBoxes = new Dictionary<ToolStripButton, List<TextBox>>();

        public AnchorControl(VerseControl ctrlVerse, StoryStageLogic storyStageLogic,
            AnchorsData anAnchorsData, Font font)
            : base(storyStageLogic)
        {
            _ctrlVerse = ctrlVerse;
            _myAnchorsData = anAnchorsData;
            _font = font;
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            tableLayoutPanel.Controls.Add(labelAnchor, 0, 0);
            tableLayoutPanel.Controls.Add(toolStripAnchors, 1, 0);

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            foreach (AnchorData anAnchorData in anAnchorsData)
            {
                ToolStripButton theAnchorButton = InitAnchorButton(toolStripAnchors, anAnchorData);

                if (anAnchorData.ExegeticalHelpNotes.Count > 0)
                    InitExegeticalHelpsRow(ctrlVerse, theAnchorButton, anAnchorData.ExegeticalHelpNotes, ref m_nNumRows);
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected ToolStripButton InitAnchorButton(ToolStrip ts, AnchorData theAnchorData)
        {
            ToolStripButton aButton = new ToolStripButton();
            aButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            aButton.Tag = theAnchorData;
            aButton.Name = CstrFieldNameAnchor + theAnchorData.JumpTarget;
            aButton.AutoSize = true;
            aButton.Text = theAnchorData.JumpTarget;
            aButton.ToolTipText = theAnchorData.ToolTipText;
            aButton.Click += new EventHandler(aButton_Click);
            aButton.MouseDown += new MouseEventHandler(aButton_MouseDown);
            ts.Items.Add(aButton);
            return aButton;
        }

        protected ToolStripButton m_theLastButtonClicked = null;

        void aButton_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripButton);
            if (e.Button == MouseButtons.Right)
                m_theLastButtonClicked = (ToolStripButton)sender;
        }

        void aButton_Click(object sender, EventArgs e)
        {
            Form form = FindForm();
            if (form is StoryEditor)
            {
                StoryEditor aSE = (StoryEditor)form;
                ToolStripButton tssb = (ToolStripButton)sender;
                aSE.SetNetBibleVerse(tssb.Text);
            }
        }

        protected void SetExegeticalHelpControls(VerseControl ctrlVerse, 
            ToolStripButton theAnchorButton, Font font, StringTransfer strQuote, ref int nNumRows)
        {
            int nLayoutRow = nNumRows++;

            Label labelExegeticalHelp = new Label
                                            {
                                                Anchor = AnchorStyles.Left,
                                                AutoSize = true,
                                                Name = CstrFieldNameExegeticalHelpLabel + nLayoutRow.ToString(),
                                                Text = "cn:"
                                            };

            CtrlTextBox tb = new CtrlTextBox(
                CstrFieldNameExegeticalHelp + nLayoutRow, ctrlVerse, font, this, strQuote,
                labelExegeticalHelp.Text);

            // add the label and tool strip as a new row to the table layout panel
            InsertRow(nLayoutRow);
            tableLayoutPanel.Controls.Add(labelExegeticalHelp, 0, nLayoutRow);
            tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);

            // add this row to the list of exe help lines added for this anchor button (so 
            //  we can gracefully remove them if the anchor is deleted.
            List<TextBox> lstTBs;
            if (!_mapAnchorsToTextBoxes.TryGetValue(theAnchorButton, out lstTBs))
            {
                lstTBs = new List<TextBox>();
                _mapAnchorsToTextBoxes.Add(theAnchorButton, lstTBs);
            }

            lstTBs.Add(tb);
        }

        protected void InitExegeticalHelpsRow(VerseControl ctrlVerse, 
            ToolStripButton theAnchorButton, ExegeticalHelpNotesData anExHelpsNoteData, 
            ref int nNumRows)
        {
            foreach (ExegeticalHelpNoteData anExHelpNoteData in anExHelpsNoteData)
                SetExegeticalHelpControls(ctrlVerse, theAnchorButton, _font, anExHelpNoteData.ExegeticalHelpNote, ref nNumRows);
        }

        private void toolStripAnchors_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                NetBibleViewer theNetBibleViewer = (NetBibleViewer)e.Data.GetData(typeof(NetBibleViewer));
                foreach (ToolStripButton btn in toolStripAnchors.Items)
                    if (btn.Text == theNetBibleViewer.ScriptureReference)
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

                if (_mapAnchorsToTextBoxes.ContainsKey(m_theLastButtonClicked))
                {
                    DialogResult res = MessageBox.Show(String.Format("The anchor you are about to delete has exegetical or cultural note(s) attached to it. These will be deleted also. Click 'OK' to continue with the deletion.{0}{0}[if you would rather have kept them, say associated to another anchor, then tell bob_eaton@sall.com and he may implement that feature. For now, you can copy the note and paste it into a new note added to a new or existing anchor (right-click on the anchor and choose 'Add Exegetical/Cultural Note'). Then come back here and delete this anchor]", Environment.NewLine),  Properties.Resources.IDS_Caption, MessageBoxButtons.OKCancel);
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
                MessageBox.Show("Right-click on one of the buttons to choose which one to delete",  Properties.Resources.IDS_Caption);
        }

        private void addCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_theLastButtonClicked != null)
            {
                AnchorAddCommentForm dlg = new AnchorAddCommentForm(m_theLastButtonClicked.Text, m_theLastButtonClicked.ToolTipText);
                DialogResult res = dlg.ShowDialog();
                if ((res == DialogResult.OK) || (res == DialogResult.Yes))
                {
                    System.Diagnostics.Debug.Assert((m_theLastButtonClicked.Tag != null) && (m_theLastButtonClicked.Tag is AnchorData));
                    AnchorData theAnchorData = (AnchorData)m_theLastButtonClicked.Tag;
                    theAnchorData.ToolTipText = m_theLastButtonClicked.ToolTipText = dlg.CommentText;
                    if (res == DialogResult.Yes)
                    {
                        addExegeticalCulturalNoteToolStripMenuItem_Click(null, null);
                    }
                }
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to",  Properties.Resources.IDS_Caption);
        }

        private void toolStripAnchors_MouseDown(object sender, MouseEventArgs e)
        {
            m_theLastButtonClicked = null;
        }

        private void addExegeticalCulturalNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_theLastButtonClicked != null)
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                System.Diagnostics.Debug.Assert(m_theLastButtonClicked.Tag is AnchorData);
                AnchorData theAnchorData = (AnchorData)m_theLastButtonClicked.Tag;
                ExegeticalHelpNoteData anEHN = theAnchorData.ExegeticalHelpNotes.AddExegeticalHelpNote("Re: " + m_theLastButtonClicked.Text);
                SetExegeticalHelpControls(_ctrlVerse, m_theLastButtonClicked, _font, anEHN.ExegeticalHelpNote, ref m_nNumRows);
                AdjustHeightWithSuspendLayout(null);
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to add the exegetical or cultural note to",  Properties.Resources.IDS_Caption);
        }

        protected BiblicalKeyTermsForm m_dlgKeyTerms = null;

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
                    System.Diagnostics.Debug.Assert(theSE.StoryProject.ProjSettings.Vernacular.HasData
                        || theSE.StoryProject.ProjSettings.NationalBT.HasData
                        || theSE.StoryProject.ProjSettings.InternationalBT.HasData);

                    m_dlgKeyTerms = new BiblicalKeyTermsForm(theSE, theSE.StoryProject.ProjSettings, liMainLang);
                }
                m_dlgKeyTerms.Show(_myAnchorsData, theSE.StoryProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_KeyTermsProblem,
                    Environment.NewLine, ex.Message), Properties.Resources.IDS_Caption);
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

                theSE.SendNoteToCorrectPane(_ctrlVerse.VerseNumber, strNote);
            }
            else
                MessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to", Properties.Resources.IDS_Caption);
        }
    }
}
