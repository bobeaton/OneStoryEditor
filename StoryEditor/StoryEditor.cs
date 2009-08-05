using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string cstrCaption = "OneStory Project Editor";
        internal const string cstrButtonDropTargetName = "buttonDropTarget";

        protected string m_strProjectFilename = null;

        protected StoryProject m_projFile = null;
        protected LoggedOnMemberInfo m_logonInfo = null;

        protected bool Modified = false;

        public Font VernacularFont = new Font("Arial Unicode MS", 12);
        public Color VernacularFontColor = Color.Maroon;
        public Font NationalBTFont = new Font("Arial Unicode MS", 12);
        public Color NationalBTFontColor = Color.Green;
        public Font InternationalBTFont = new Font("Times New Roman", 10);
        public Color InternationalBTFontColor = Color.Blue;

        protected const int nMaxRecentFiles = 15;

        public enum UserTypes
        {
            eUndefined = 0,
            eCrafter,
            eUNS,
            eConsultantInTraining,
            eIndependentConsultant,
            eCoach,
            eJustLooking
        }

        public StoryEditor()
        {
            InitializeComponent();
            try
            {
                InitializeNetBibleViewer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Problem initializing Sword (the Net Bible viewer):{0}{0}{1}", Environment.NewLine, ex.Message), StoryEditor.cstrCaption);
            }
#if false
            OpenProjectFile(@"C:\Code\StoryEditor\StoryEditor\StoryProject.onestory");
#endif
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProjectFile(openFileDialog.FileName);
            }
        }

        protected void NewProjectFile()
        {
            CheckForSaveDirtyFile();
            if (!InsureEmptyProjectFile())
                return;
            GetLogin();
        }

        protected bool GetLogin()
        {
            // first figure out which team member it is:
            string strMemberName = null;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMember))
            {
                strMemberName = Properties.Settings.Default.LastMember;

                foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                    if (aMemberRow.name == strMemberName)
                    {
                        // if (s)he's already logged on...
                        if ((m_logonInfo != null) 
                            && (m_logonInfo.MemberName == strMemberName)
                            && (m_logonInfo.Type == TeamMemberForm.GetUserType(aMemberRow.memberType)))
                            return true;    // same person and role is already logged in

                        // otherwise, if it is a known member and is a crafter (who are more likely
                        //  not to function in multiple, different roles), then we're done also...
                        if (aMemberRow.memberType == TeamMemberForm.cstrCrafter)
                        {
                            m_logonInfo = new LoggedOnMemberInfo(strMemberName, aMemberRow.memberKey,
                                TeamMemberForm.GetUserType(aMemberRow.memberType));
                            return true;
                        }
                        // otherwise, fall thru and make them pick it.
                    }
            }

            return EditTeamMembers(strMemberName);
        }

        protected bool EditTeamMembers(string strMemberName)
        {
            System.Diagnostics.Debug.Assert(m_projFile != null);

            // if we haven't found the member, then get them to select it from the Team Member UI
            TeamMemberForm dlg = new TeamMemberForm(m_projFile);
            if (!String.IsNullOrEmpty(strMemberName))
                // if we did find it, but couldn't accept it without question, then at least pre-select member
                dlg.SelectedMember = strMemberName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                    if (aMemberRow.name == dlg.SelectedMember)
                    {
                        m_logonInfo = new LoggedOnMemberInfo(dlg.SelectedMember, aMemberRow.memberKey, dlg.UserType);
                        return true;
                    }
            }

            return false;
        }

        private void teamMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsureProjectFile();
            EditTeamMembers((m_logonInfo != null) ? m_logonInfo.MemberName : null);
        }

        protected void OpenProjectFile(string strProjectFilename)
        {
            // add this filename to the list of recently used files
            if (Properties.Settings.Default.RecentFiles.Contains(strProjectFilename))
                Properties.Settings.Default.RecentFiles.Remove(strProjectFilename);
            else if (Properties.Settings.Default.RecentFiles.Count > nMaxRecentFiles)
                Properties.Settings.Default.RecentFiles.RemoveAt(nMaxRecentFiles);

            Properties.Settings.Default.RecentFiles.Insert(0, strProjectFilename);
            Properties.Settings.Default.Save();

            CheckForSaveDirtyFile();
            m_projFile = new StoryProject();

            try
            {
                System.Diagnostics.Debug.Assert(m_projFile != null);
                m_projFile.ReadXml(strProjectFilename);

                StoryProject.storyRow theStoryRow = null;
                string strStoryToLoad = null;
                InsureStoriesRow(); // has the side effect of setting the frame title to the project language name
                if (m_projFile.story.Count > 0)
                {
                    // defaults
                    theStoryRow = m_projFile.story[0];
                    strStoryToLoad = m_projFile.story[0].name;

                    // populate the combo boxes with all the existing story names
                    foreach (StoryProject.storyRow aStoryRow in m_projFile.story)
                        comboBoxStorySelector.Items.Add(aStoryRow.name);
                }

                // check for project settings that might have been saved from a previous session
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastStoryWorkedOn))
                {
                    strStoryToLoad = Properties.Settings.Default.LastStoryWorkedOn;
                    foreach (StoryProject.storyRow aStoryRow in m_projFile.story)
                    {
                        if (aStoryRow.name == strStoryToLoad)
                        {
                            theStoryRow = aStoryRow;
                            break;
                        }
                    }
                }

                if (theStoryRow != null)
                    comboBoxStorySelector.SelectedItem = theStoryRow.name;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(String.Format("Unable to open project file '{1}'{0}{0}{2}{0}{0}Send the project file to bob_eaton@sall.com for help",
                    Environment.NewLine, strProjectFilename, ex.Message), cstrCaption);
            }
        }

        protected bool InsureEmptyProjectFile()
        {
            InsureProjectFile();
            StoryProject.storiesRow theStoriesRow = InsureStoriesRow();
            if (theStoriesRow == null)
                return false;
            m_projFile.Members.AddMembersRow(theStoriesRow);
            StoryProject.FontsRow theFontsRow = m_projFile.Fonts.AddFontsRow(theStoriesRow);
            m_projFile.VernacularFont.AddVernacularFontRow("Arial Unicode MS", 12,
                Color.CornflowerBlue.ToString(), theFontsRow);
            m_projFile.NationalBTFont.AddNationalBTFontRow("Arial Unicode MS", 12,
                Color.Green.ToString(), theFontsRow);
            m_projFile.InternationalBTFont.AddInternationalBTFontRow("Arial Unicode MS", 11,
                Color.Blue.ToString(), theFontsRow);

            return true;
        }

        protected void InsureProjectFile()
        {
            if (m_projFile == null)
                m_projFile = new StoryProject();
        }

        private void comboBoxStorySelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // user just finished entering a story name to select (or add)
            {
                StoryProject.storyRow theStoryRow = null;
                string strStoryToLoad = comboBoxStorySelector.Text;
                InsureProjectFile();
                if (m_projFile.story.Count > 0)
                    foreach (StoryProject.storyRow aStoryRow in m_projFile.story)
                    {
                        if (aStoryRow.name == strStoryToLoad)
                        {
                            theStoryRow = aStoryRow;
                            break;
                        }
                    }

                if (theStoryRow == null)
                {
                    if (MessageBox.Show(String.Format("Unable to find the story '{0}'. Would you like to add a new one with that name?", strStoryToLoad), cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.Assert(!comboBoxStorySelector.Items.Contains(strStoryToLoad));
                        comboBoxStorySelector.Items.Add(strStoryToLoad);
                        StoryProject.storiesRow aStoriesRow = InsureStoriesRow();
                        if (aStoriesRow == null)
                            return; // user could enter nothing for a language name
                            
                        m_projFile.story.AddstoryRow(strStoryToLoad, Guid.NewGuid().ToString(), aStoriesRow);
                        comboBoxStorySelector.SelectedItem = strStoryToLoad;
                    }
                }
                else
                    comboBoxStorySelector.SelectedItem = theStoryRow.name;
            }
        }

        protected void InsureVersesRow(StoryProject.storyRow aStoryRow)
        {
            InsureProjectFile();
            if (aStoryRow.GetversesRows().Length == 0)
                m_projFile.verses.AddversesRow(aStoryRow);
        }

        protected StoryProject.storiesRow InsureStoriesRow()
        {
            InsureProjectFile();
            if (m_projFile.stories.Count == 0)
            {
                string strLanguage = Microsoft.VisualBasic.Interaction.InputBox("You are creating a brand new OneStory project. Enter the name of the language for this project. (if you had intended to edit an existing project, cancel this dialog and use the 'File', 'Open' command)", cstrCaption, null, Screen.PrimaryScreen.WorkingArea.Right / 2, Screen.PrimaryScreen.WorkingArea.Bottom / 2);
                if (String.IsNullOrEmpty(strLanguage))
                    return null;
                m_projFile.stories.AddstoriesRow(strLanguage);
            }

            this.Text = String.Format("OneStory Editor -- {0} Story Project", m_projFile.stories[0].ProjectLanguage);
            return m_projFile.stories[0];
        }

        private void comboBoxStorySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // find out which member we're working with
            if (!GetLogin())
                return;

            StoryProject.storyRow theStoryRow = null;
            foreach (StoryProject.storyRow aStoryRow in m_projFile.story)
                if (aStoryRow.name == (string)comboBoxStorySelector.SelectedItem)
                {
                    theStoryRow = aStoryRow;
                    break;
                }

            System.Diagnostics.Debug.Assert(theStoryRow != null);
            textBoxStoryVerse.Text = "Story: " + theStoryRow.name;
            /*
            StoryProject.VernacularFontRow aVFRow = aProj.VernacularFont[0];
            VernacularFont = new Font(aVFRow.FontName, aVFRow.FontSize);
            VernacularFontColor = Color.FromName(aVFRow.FontColor);

            StoryProject.NationalBTFontRow aNFRow = aProj.NationalBTFont[0];
            NationalBTFont = new Font(aNFRow.FontName, aNFRow.FontSize);
            NationalBTFontColor = Color.FromName(aNFRow.FontColor);

            StoryProject.InternationalBTFontRow aIFRow = aProj.InternationalBTFont[0];
            InternationalBTFont = new Font(aIFRow.FontName, aIFRow.FontSize);
            InternationalBTFontColor = Color.FromName(aIFRow.FontColor);
            */
            int nVerseIndex = 0;
            AddDropTargetToFlowLayout(nVerseIndex++);
            InsureVersesRow(theStoryRow);
            foreach (StoryProject.verseRow aRow in theStoryRow.GetversesRows()[0].GetverseRows())
            {
                VerseBtControl aVerseCtrl = new VerseBtControl(this, aRow, nVerseIndex);
                aVerseCtrl.UpdateHeight(Panel1_Width);
                flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                AddDropTargetToFlowLayout(nVerseIndex);

                ConsultNotesDataConverter aCNsDC = new ConsultantNotesData(aRow.GetConsultantNotesRows());
                ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(aCNsDC, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelConsultantNotes.Controls.Add(aConsultNotesCtrl);

                aCNsDC = new CoachNotesData(aRow.GetCoachNotesRows());
                aConsultNotesCtrl = new ConsultNotesControl(aCNsDC, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelCoachNotes.Controls.Add(aConsultNotesCtrl);

                nVerseIndex++;
            }
        }

        protected void AddDropTargetToFlowLayout(int nVerseIndex)
        {
            Button buttonDropTarget = new Button();
            buttonDropTarget.AllowDrop = true;
            buttonDropTarget.Location = new System.Drawing.Point(3, 3);
            buttonDropTarget.Name = cstrButtonDropTargetName + nVerseIndex.ToString();
            buttonDropTarget.Size = new System.Drawing.Size(75, 5);
            buttonDropTarget.TabIndex = nVerseIndex;
            buttonDropTarget.UseVisualStyleBackColor = true;
            buttonDropTarget.Visible = false;
            buttonDropTarget.DragEnter += new DragEventHandler(buttonDropTarget_DragEnter);
            buttonDropTarget.DragDrop += new DragEventHandler(buttonDropTarget_DragDrop);
            flowLayoutPanelVerses.Controls.Add(buttonDropTarget);
        }

        void buttonDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                Button btnTarget = (Button)sender;
                string strTargetName = btnTarget.Name;
                string strTargetVerse = strTargetName.Substring(cstrButtonDropTargetName.Length);
                int nInsertionIndex = (int)Convert.ToInt32(strTargetVerse);
                if ((nInsertionIndex == (aVerseCtrl.VerseNumber - 1)) || (nInsertionIndex == (aVerseCtrl.VerseNumber)))
                    DoCopy(nInsertionIndex, aVerseCtrl);
                else
                    DoMove(nInsertionIndex, aVerseCtrl);
            }
        }

        void DoCopy(int nInsertionIndex, VerseBtControl aVerseCtrl)
        {
            // TODO:
        }

        void DoMove(int nInsertionIndex, VerseBtControl aVerseCtrl)
        {
            // TODO:
        }

        void buttonDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                Button btnTarget = (Button)sender;
                string strTargetName = btnTarget.Name;
                string strTargetVerse = strTargetName.Substring(cstrButtonDropTargetName.Length);
                int nInsertionIndex = (int)Convert.ToInt32(strTargetVerse);
                if ((nInsertionIndex == (aVerseCtrl.VerseNumber - 1)) || (nInsertionIndex == (aVerseCtrl.VerseNumber)))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }
        
        internal void LightUpDropTargetButtons()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.Visible = true;
                }
            }
        }

        internal void DimDropTargetButtons()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is Button)
                    ctrl.Visible = false;
            }
        }

        protected void InitializeNetBibleViewer()
        {
            netBibleViewer.InitNetBibleViewer();
            string strLastRef = "gen 1:5";
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastNetBibleReference))
                strLastRef = Properties.Settings.Default.LastNetBibleReference;
            SetNetBibleVerse(strLastRef);
        }

        internal void SetNetBibleVerse(string strScriptureReference)
        {
            if (splitContainerUpDown.Panel2Collapsed == true)
                viewNetBibleMenuItem.Checked = true;

            netBibleViewer.DisplayVerses(strScriptureReference);
        }

        protected int Panel1_Width
        {
            get
            {
                return splitContainerLeftRight.Panel1.Width - splitContainerLeftRight.Margin.Horizontal -
                    SystemInformation.VerticalScrollBarWidth - 2;
            }
        }

        protected int Panel2_Width
        {
            get
            {
                return splitContainerLeftRight.Panel2.Width - splitContainerLeftRight.Margin.Horizontal -
                    SystemInformation.VerticalScrollBarWidth - 2;
            }
        }

        private DialogResult CheckForSaveDirtyFile()
        {
            DialogResult res = DialogResult.None;
            if (Modified)
            {
                res = MessageBox.Show("Do you want to save the opened project first?", cstrCaption, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                    SaveClicked();
                else if (res != DialogResult.Cancel)
                    Modified = false;
            }

            // do cleanup, because this is always called before starting something new (new file or empty project)
            m_projFile = null;
            flowLayoutPanelVerses.Controls.Clear();
            flowLayoutPanelConsultantNotes.Controls.Clear();
            flowLayoutPanelCoachNotes.Controls.Clear();
            textBoxStoryVerse.Text = "Story";
            comboBoxStorySelector.Items.Clear();
            return res;
        }

        protected void SaveClicked()
        {
            if (String.IsNullOrEmpty(m_strProjectFilename))
                SaveAsClicked();
            else
                SaveFile(m_strProjectFilename);
        }

        protected void SaveAsClicked()
        {
            if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_strProjectFilename = saveFileDialog.FileName;
                SaveFile(m_strProjectFilename);
            }
        }

        protected void SaveFile(string strFilename)
        {
            /*
            try
            {
                File.WriteAllLines(strFilename, this.richTextBoxMapEditor.Lines, m_enc);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(String.Format("The map file '{0}' is locked. Is it read-only? Or opened in some other program? Unlock it and try again.", strFilename), cstrCaption);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to save the map file '{1}'{0}{0}{2}", Environment.NewLine, strFilename, ex.Message), cstrCaption);
                return;
            }

            Program.Modified = (m_strMapNameReal != strFilename);
            Program.AddFilenameToTitle(strFilename);

            // if it's been 5 minutes since our last backup...
            if ((DateTime.Now - m_dtLastSave) > m_tsBetweenBackups)
            {
                // ... hide a copy in the user's Application Data file
                File.Copy(m_strMapNameReal, GetBackupFilename(strFilename), true);
            }
            */
            Modified = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveClicked();
        }

        protected void SetupTitleBar(string strProjectName, string strStoryName)
        {
            String str = String.Format("{0} -- {1} -- {2}", cstrCaption, strProjectName, strStoryName);
        }

        protected void UpdateVersePanel()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is VerseBtControl)
                {
                    VerseBtControl aVerseCtrl = (VerseBtControl)ctrl;
                    aVerseCtrl.UpdateView(this);
                    aVerseCtrl.UpdateHeight(Panel1_Width);
                }
            }
        }

        private void viewFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVersePanel();
        }

        private void viewNetBibleMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            splitContainerUpDown.Panel2Collapsed = !tsm.Checked;
        }

        private void viewConsultantNoteFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            bool bHidePanel1 = !tsm.Checked;
            if (bHidePanel1)
            {
                if (splitContainerMentorNotes.Panel2Collapsed)
                    splitContainerLeftRight.Panel2Collapsed = true;
                else
                    splitContainerMentorNotes.Panel1Collapsed = true;
                return;
            }
            // showing the Consultant's pane
            else if (splitContainerLeftRight.Panel2Collapsed)   // if the whole right-half is already collapsed...
            {
                // ... first enable it.
                splitContainerLeftRight.Panel2Collapsed = false;

                // glitch, whichever half (consultant's or coach's) was collapsed last will still be active even
                //  though it's menu item will be reset. So we need to hide it if we're enabling the other one
                if (!splitContainerMentorNotes.Panel2Collapsed) // this means it's not actually hidden
                    splitContainerMentorNotes.Panel2Collapsed = true;
            }

            splitContainerMentorNotes.Panel1Collapsed = false;
        }

        private void viewCoachNotesFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            bool bHidePanel2 = !tsm.Checked;
            if (bHidePanel2)
            {
                if (splitContainerMentorNotes.Panel1Collapsed)
                    splitContainerLeftRight.Panel2Collapsed = true;
                else
                    splitContainerMentorNotes.Panel2Collapsed = true;
                return;
            }
            // showing the Coach's pane
            else if (splitContainerLeftRight.Panel2Collapsed)   // if the whole right-half is already collapsed...
            {
                // ... first enable it.
                splitContainerLeftRight.Panel2Collapsed = false;

                // glitch, whichever half (consultant's or coach's) was collapsed last will still be active even
                //  though it's menu item will be reset. So we need to hide it if we're enabling the other one
                if (!splitContainerMentorNotes.Panel1Collapsed) // this means it's not actually hidden
                    splitContainerMentorNotes.Panel1Collapsed = true;
            }

            splitContainerMentorNotes.Panel2Collapsed = false;
        }

        private void splitContainerLeftRight_Panel2_SizeChanged(object sender, EventArgs e)
        {
            if (!splitContainerMentorNotes.Panel1Collapsed)
                foreach (Control ctrl in flowLayoutPanelConsultantNotes.Controls)
                {
                    if (ctrl is ConsultNotesControl)
                    {
                        ConsultNotesControl aConsultNoteCtrl = (ConsultNotesControl)ctrl;
                        aConsultNoteCtrl.UpdateHeight(Panel2_Width);
                    }
                }

            if (!splitContainerMentorNotes.Panel2Collapsed)
                foreach (Control ctrl in flowLayoutPanelCoachNotes.Controls)
                {
                    if (ctrl is ConsultNotesControl)
                    {
                        ConsultNotesControl aConsultNoteCtrl = (ConsultNotesControl)ctrl;
                        aConsultNoteCtrl.UpdateHeight(Panel2_Width);
                    }
                }
        }

        private void splitContainerLeftRight_Panel1_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is VerseBtControl)
                {
                    VerseBtControl aVerseCtrl = (VerseBtControl)ctrl;
                    aVerseCtrl.UpdateHeight(Panel1_Width);
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectFile();
        }

        private void projectToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            recentFilesToolStripMenuItem.DropDownItems.Clear();
            foreach (string strRecentFile in Properties.Settings.Default.RecentFiles)
                recentFilesToolStripMenuItem.DropDownItems.Add(strRecentFile, null, recentFilesToolStripMenuItem_Click);

            recentFilesToolStripMenuItem.Enabled = (recentFilesToolStripMenuItem.DropDownItems.Count > 0);
        }

        private void recentFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem aRecentFile = (ToolStripDropDownItem)sender;
            try
            {
                OpenProjectFile(aRecentFile.Text);
            }
            catch (Exception ex)
            {
                // probably means the file doesn't exist anymore, so remove it from the recent used list
                Properties.Settings.Default.RecentFiles.Remove(aRecentFile.Text);
                MessageBox.Show(ex.Message, cstrCaption);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForSaveDirtyFile();
            this.Close();
        }
    }
}
