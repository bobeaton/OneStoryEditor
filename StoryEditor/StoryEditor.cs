using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using Chorus.UI.Sync;
using Chorus.VcsDrivers.Mercurial;
using Chorus.sync;

namespace OneStoryProjectEditor
{
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string CstrCaption = "OneStory Project Editor";
        internal const string CstrButtonDropTargetName = "buttonDropTarget";

        protected string m_strProjectFilename = null;

        // protected StoryProject m_projFile = null;
        internal StoriesData Stories = null;
        internal StoryData theCurrentStory = null;

        // we keep a copy of this, because it ought to persist across multiple files
        internal TeamMemberData LoggedOnMember = null;

        internal bool Modified = false;

        protected const int nMaxRecentFiles = 15;

        public StoryEditor()
        {
            InitializeComponent();
            try
            {
                InitializeNetBibleViewer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Problem initializing Sword (the Net Bible viewer):{0}{0}{1}", Environment.NewLine, ex.Message), StoryEditor.CstrCaption);
            }

            if (String.IsNullOrEmpty(Properties.Settings.Default.LastUserType))
                NewProjectFile();
            else if ((Properties.Settings.Default.LastUserType == TeamMemberData.CstrCrafter)
                    && !String.IsNullOrEmpty(Properties.Settings.Default.LastProjectFile))
                OpenProjectFile(Properties.Settings.Default.LastProjectFile);
        }

        // this is now import.
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProjectFile(openFileDialog.FileName);
            }
        }

        protected void CloseProjectFile()
        {
            System.Diagnostics.Debug.Assert(!Modified);
            Stories = null;
            theCurrentStory = null;
            comboBoxStorySelector.Items.Clear();
            comboBoxStorySelector.Text = "<type the name of a story to create and hit Enter>";
        }

        protected void NewProjectFile()
        {
            CheckForSaveDirtyFile();
            CloseProjectFile();
            comboBoxStorySelector.Focus();
            
            // for a new project, we don't want to automatically log in (since this will be the first
            //  time editing the new project and we need to add at least the current user)
            LoggedOnMember = null;
            System.Diagnostics.Debug.Assert(Stories == null);
            teamMembersToolStripMenuItem_Click(null, null);
            buttonsStoryStage.Enabled = true;
        }

        private void teamMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Stories == null)
            {
                try
                {
                    Stories = new StoriesData(ref LoggedOnMember);
                    buttonsStoryStage.Enabled = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(String.Format("Unable to open the member list{0}{0}{1}{0}{0}Contact bob_eaton@sall.com for help",
                        Environment.NewLine, ex.Message), CstrCaption);
                    return;
                }
            }
            else
            {
                try
                {
                    LoggedOnMember = Stories.EditTeamMembers(LoggedOnMember.Name, TeamMemberForm.CstrDefaultOKLabel);
                }
                catch { }   // this might throw if the user cancels, but we don't care
            }
        }

        protected void OpenProjectFile(string strProjectFilename)
        {
            // add this filename to the list of recently used files
            if (Properties.Settings.Default.RecentFiles.Contains(strProjectFilename))
                Properties.Settings.Default.RecentFiles.Remove(strProjectFilename);
            else if (Properties.Settings.Default.RecentFiles.Count > nMaxRecentFiles)
                Properties.Settings.Default.RecentFiles.RemoveAt(nMaxRecentFiles);

            Properties.Settings.Default.RecentFiles.Insert(0, strProjectFilename);
            Properties.Settings.Default.LastProjectFile = strProjectFilename;
            Properties.Settings.Default.Save();

            CheckForSaveDirtyFile();
            CloseProjectFile();
            try
            {
                StoryProject projFile = new StoryProject();
                projFile.ReadXml(strProjectFilename);

                // get *all* the data
                Stories = new StoriesData(projFile, ref LoggedOnMember);
                buttonsStoryStage.Enabled = true;

                string strStoryToLoad = null;
                if (Stories.Count > 0)
                {
                    // populate the combo boxes with all the existing story names
                    foreach (StoryData aStory in Stories)
                        comboBoxStorySelector.Items.Add(aStory.StoryName);
                    strStoryToLoad = Stories[0].StoryName;    // default
                }

                // check for project settings that might have been saved from a previous session
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastStoryWorkedOn) && comboBoxStorySelector.Items.Contains(Properties.Settings.Default.LastStoryWorkedOn))
                    strStoryToLoad = Properties.Settings.Default.LastStoryWorkedOn;

                if (!String.IsNullOrEmpty(strStoryToLoad) && comboBoxStorySelector.Items.Contains(strStoryToLoad))
                    comboBoxStorySelector.SelectedItem = strStoryToLoad;
            }
            catch (System.Exception ex)
            {
                string strErrorMsg = String.Format("Unable to open project file '{1}'{0}{0}{2}{0}{0}{3}{0}{0}Send the project file along with the error message to bob_eaton@sall.com for help",
                    Environment.NewLine, strProjectFilename, 
                    ((ex.InnerException != null) ? ex.InnerException.Message : Environment.NewLine), ex.Message);
                MessageBox.Show(strErrorMsg, CstrCaption);
            }
        }

        private void comboBoxStorySelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // user just finished entering a story name to select (or add)
            {
                if (Stories == null)
                {
                    Stories = new StoriesData(ref LoggedOnMember);
                    buttonsStoryStage.Enabled = true;
                }

                int nInsertIndex = 0;
                StoryData theStory = null;
                string strStoryToLoad = comboBoxStorySelector.Text;
                for (int i = 0; i < Stories.Count; i++)
                {
                    StoryData aStory = Stories[i];
                    if ((theCurrentStory != null) && (theCurrentStory == aStory))
                        nInsertIndex = i + 1;
                    if (aStory.StoryName == strStoryToLoad)
                        theStory = aStory;
                }

                if (theStory == null)
                {
                    if (MessageBox.Show(String.Format("Unable to find the story '{0}'. Would you like to add a new one with that name?", strStoryToLoad), CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.Assert(!comboBoxStorySelector.Items.Contains(strStoryToLoad));
                        comboBoxStorySelector.Items.Add(strStoryToLoad);
                        theCurrentStory = new StoryData(strStoryToLoad, LoggedOnMember.MemberGuid);
                        Stories.Insert(nInsertIndex, theCurrentStory);
                        comboBoxStorySelector.SelectedItem = strStoryToLoad;
                    }
                }
                else
                    comboBoxStorySelector.SelectedItem = theStory.StoryName;
            }
        }

        private void comboBoxStorySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: we have to do something. We have to save to move to another story, but we don't want to 
            // close the m_projFile if we're actually going to another story in this same project....
            CheckForSaveDirtyFile();    // to see if we should save the current story before moving on.

            System.Diagnostics.Debug.Assert(!Modified 
                || (flowLayoutPanelVerses.Controls.Count != 0)
                || (flowLayoutPanelConsultantNotes.Controls.Count != 0)
                || (flowLayoutPanelCoachNotes.Controls.Count != 0)); // if this happens, it means we didn't save or cleanup the document

            // we might could come thru here without having opened any file (e.g. after New)
            if (Stories == null)
            {
                Stories = new StoriesData(ref LoggedOnMember);
                buttonsStoryStage.Enabled = true;
            }

            // find the story they've chosen (this shouldn't be possible to fail)
            foreach (StoryData aStory in Stories)
                if (aStory.StoryName == (string)comboBoxStorySelector.SelectedItem)
                {
                    theCurrentStory = aStory;
                    break;
                }
            System.Diagnostics.Debug.Assert(theCurrentStory != null);

            // initialize the text box showing the storying they're editing
            textBoxStoryVerse.Text = "Story: " + theCurrentStory.StoryName;
            this.Text = String.Format("OneStory Editor -- {0} Story Project", Stories.ProjSettings.ProjectName);

            // initialize the project stage details (which might hide certain views)
            //  (do this *after* initializing the whole thing, because if we save, we'll
            //  want to save even the hidden pieces)
            SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage);

            // finally, initialize the verse controls
            InitAllPanes();
        }

        protected void InitAllPanes(VersesData theVerses)
        {
            ClearFlowControls();
            int nVerseIndex = 0;
            if (theVerses.Count == 0)
                theCurrentStory.Verses.InsertVerse(0, "<Type the UNS's back translation>");

            flowLayoutPanelVerses.SuspendLayout();
            flowLayoutPanelConsultantNotes.SuspendLayout();
            flowLayoutPanelCoachNotes.SuspendLayout();
            SuspendLayout();

            AddDropTargetToFlowLayout(nVerseIndex++);
            foreach (VerseData aVerse in theVerses)
            {
                InitVerseControls(aVerse, nVerseIndex);

                InitConsultNotesPane(flowLayoutPanelConsultantNotes, aVerse.ConsultantNotes, nVerseIndex);

                InitConsultNotesPane(flowLayoutPanelCoachNotes, aVerse.CoachNotes, nVerseIndex);

                nVerseIndex++;
            }

            flowLayoutPanelVerses.ResumeLayout(true);
            flowLayoutPanelConsultantNotes.ResumeLayout(true);
            flowLayoutPanelCoachNotes.ResumeLayout(true);
            ResumeLayout(true);
        }

        protected void InitVerseControls(VerseData aVerse, int nVerseIndex)
        {
            VerseBtControl aVerseCtrl = new VerseBtControl(this, aVerse, nVerseIndex);
            aVerseCtrl.UpdateHeight(Panel1_Width);
            flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
            AddDropTargetToFlowLayout(nVerseIndex);
        }

        // this is for use by the consultant panes if we add or remove or hide a note
        internal void ReInitVerseControls()
        {
            int nVerseIndex = 0;
            flowLayoutPanelVerses.Controls.Clear();
            flowLayoutPanelVerses.SuspendLayout();
            SuspendLayout();

            AddDropTargetToFlowLayout(nVerseIndex++);
            foreach (VerseData aVerse in theCurrentStory.Verses)
                InitVerseControls(aVerse, nVerseIndex++);

            flowLayoutPanelVerses.ResumeLayout(true);
            ResumeLayout(true);
        }

        protected Dictionary<ConsultNotesDataConverter, FlowLayoutPanel> _WhichConNoteToWhichFlowPanel = new Dictionary<ConsultNotesDataConverter, FlowLayoutPanel>();
        protected void InitConsultNotesPane(FlowLayoutPanel theFLP, ConsultNotesDataConverter aCNsDC, int nVerseIndex)
        {
            ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(theCurrentStory.ProjStage, aCNsDC, nVerseIndex);
            aConsultNotesCtrl.UpdateHeight(Panel2_Width);
            theFLP.Controls.Add(aConsultNotesCtrl);
            if (!_WhichConNoteToWhichFlowPanel.ContainsKey(aCNsDC))
                _WhichConNoteToWhichFlowPanel.Add(aCNsDC, theFLP);
        }

        // this is for use by the consultant panes if we add or remove or hide a note
        internal void ReInitConsultNotesPane(ConsultNotesDataConverter aCNsDC)
        {
            System.Diagnostics.Debug.Assert(_WhichConNoteToWhichFlowPanel.ContainsKey(aCNsDC));
            FlowLayoutPanel theFLP = _WhichConNoteToWhichFlowPanel[aCNsDC];
            theFLP.Controls.Clear();
            
            theFLP.SuspendLayout();
            SuspendLayout();

            int nVerseIndex = 0;
            foreach (VerseData aVerse in theCurrentStory.Verses)
                InitConsultNotesPane(theFLP, aCNsDC, nVerseIndex++);

            theFLP.ResumeLayout(true);
            ResumeLayout(true);
        }

        internal void AddNewVerse(VerseBtControl theVerse, int nNumberToAdd, bool bAfter)
        {
            int nInsertionIndex = theVerse.VerseNumber - 1;
            if (bAfter)
                nInsertionIndex++;

            VersesData lstNewVerses = new VersesData();
            for (int i = 0; i < nNumberToAdd; i++)
                lstNewVerses.Add(new VerseData());
            
            theCurrentStory.Verses.InsertRange(nInsertionIndex, lstNewVerses);
            InitAllPanes();
        }

        internal void AddNewVerse(int nInsertionIndex, string strNationalBT)
        {
            System.Diagnostics.Debug.Assert((theCurrentStory != null) && (theCurrentStory.Verses != null));
            theCurrentStory.Verses.InsertVerse(nInsertionIndex, strNationalBT);
        }

        internal void InitAllPanes()
        {
            try
            {
                InitAllPanes(theCurrentStory.Verses);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to continue! Cause: {0}", ex.Message), CstrCaption);
                return;
            }
        }

        internal void DeleteVerse(VerseData theVerseDataToDelete)
        {
            theCurrentStory.Verses.Remove(theVerseDataToDelete);
            InitAllPanes();
        }

        internal void SetViewBasedOnProjectStage(StoryStageLogic.ProjectStages eStage)
        {
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[eStage];

            st.SetView(this);
            helpProvider.SetHelpString(this, st.StageInstructions);
            SetStatusBar(String.Format("{0}  Press F1 for instructions", st.StageDisplayString), st.StageInstructions);
        }

        protected Button AddDropTargetToFlowLayout(int nVerseIndex)
        {
            Button buttonDropTarget = new Button();
            buttonDropTarget.AllowDrop = true;
            buttonDropTarget.Location = new System.Drawing.Point(3, 3);
            buttonDropTarget.Name = CstrButtonDropTargetName + nVerseIndex.ToString();
            buttonDropTarget.Size = new System.Drawing.Size(this.Panel1_Width, 10);
            buttonDropTarget.Dock = DockStyle.Fill;
            buttonDropTarget.TabIndex = nVerseIndex;
            buttonDropTarget.UseVisualStyleBackColor = true;
            buttonDropTarget.Visible = false;
            buttonDropTarget.Tag = nVerseIndex;
            buttonDropTarget.DragEnter += buttonDropTarget_DragEnter;
            buttonDropTarget.DragDrop += buttonDropTarget_DragDrop;
            flowLayoutPanelVerses.Controls.Add(buttonDropTarget);
            return buttonDropTarget;
        }

        void buttonDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseData)))
            {
                VerseData aVerseData = (VerseData)e.Data.GetData(typeof(VerseData));
                System.Diagnostics.Debug.Assert(sender is Button);
                int nInsertionIndex = (flowLayoutPanelVerses.Controls.IndexOf((Button)sender) / 2);
                DoMove(nInsertionIndex, aVerseData);
            }
        }

        void DoMove(int nInsertionIndex, VerseData theVerseToMove)
        {
            int nCurIndex = theCurrentStory.Verses.IndexOf(theVerseToMove);
            theCurrentStory.Verses.Remove(theVerseToMove);

            // if we're moving the verse to an earlier position, then remove it from its higher index, 
            //  just insert it at the new lower index. However, if an earlier verse is being moved later, 
            //  then once we remove it, then the insertion index will be one too many
            if (nInsertionIndex > nCurIndex)
                --nInsertionIndex;
            
            theCurrentStory.Verses.Insert(nInsertionIndex, theVerseToMove);
            InitAllPanes();
        }

        void buttonDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseData)))
                e.Effect = DragDropEffects.Move;
        }
        
        internal void LightUpDropTargetButtons(VerseBtControl aVerseCtrl)
        {
            int nIndex = flowLayoutPanelVerses.Controls.IndexOf(aVerseCtrl);
            for (int i = 0; i < flowLayoutPanelVerses.Controls.Count; i += 2)
            {
                Control ctrl = flowLayoutPanelVerses.Controls[i];
                System.Diagnostics.Debug.Assert(ctrl is Button);
                if (Math.Abs(nIndex - i) > 1)
                    ctrl.Visible = true;
            }
        }

        internal void DimDropTargetButtons()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
                if (ctrl is Button)
                    ctrl.Visible = false;
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
                    SystemInformation.VerticalScrollBarWidth;
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
                res = MessageBox.Show("Do you want to save your changes?", CstrCaption, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                    SaveClicked();
                else if (res != DialogResult.Cancel)
                    Modified = false;
            }

            // do cleanup, because this is always called before starting something new (new file or empty project)
            ClearFlowControls();
            textBoxStoryVerse.Text = "Story";
            return res;
        }

        protected void ClearFlowControls()
        {
            flowLayoutPanelVerses.Controls.Clear();
            flowLayoutPanelConsultantNotes.Controls.Clear();
            flowLayoutPanelCoachNotes.Controls.Clear();
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
            if (Stories.ProjSettings != null)
            {
                Directory.CreateDirectory(Stories.ProjSettings.ProjectFolder);
                saveFileDialog.InitialDirectory = Stories.ProjSettings.ProjectFolder;
            }

            if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_strProjectFilename = saveFileDialog.FileName;
                SaveFile(m_strProjectFilename);
            }
        }

        protected void SaveXElement(XElement elem, string strFilename)
        {
            // create the root portions of the XML document and tack on the fragment we've been building
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(StoriesData.ns + "StoryProject",
                    elem));

            // save it with an extra extn.
            doc.Save(strFilename + CstrExtraExtnToAvoidClobberingFilesWithFailedSaves);

            // backup the last version to appdata
            // Note: doing File.Move leaves the old file security settings rather than replacing them
            // based on the target directory. Copy, on the other hand, inherits
            // security settings from the target folder, which is what we want to do.
            if (File.Exists(strFilename))
                File.Copy(strFilename, GetBackupFilename(strFilename), true);
            File.Delete(strFilename);
            File.Copy(strFilename + CstrExtraExtnToAvoidClobberingFilesWithFailedSaves, strFilename, true);
            File.Delete(strFilename + CstrExtraExtnToAvoidClobberingFilesWithFailedSaves);
        }

        protected const string CstrExtraExtnToAvoidClobberingFilesWithFailedSaves = ".out";

        protected void QueryStoryPurpose()
        {
            StoryFrontMatterForm dlg = new StoryFrontMatterForm(Stories, theCurrentStory);
            dlg.ShowDialog();
        }

        protected void SaveFile(string strFilename)
        {
            try
            {
                // let's see if the UNS entered the purpose of this story
                System.Diagnostics.Debug.Assert((theCurrentStory != null) && (theCurrentStory.CraftingInfo != null));
                if (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.StoryPurpose)
                    || String.IsNullOrEmpty(theCurrentStory.CraftingInfo.ResourcesUsed))
                    QueryStoryPurpose();

                SaveXElement(GetXml, strFilename);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(String.Format("The project file '{0}' is locked. Is it read-only? Or opened in some other program? Unlock it and try again. Or try to save it as a different name.", strFilename), CstrCaption);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to save the project file '{1}'{0}{0}{2}", Environment.NewLine, strFilename, ex.Message), CstrCaption);
                return;
            }

            Modified = false;
        }

        private static string GetBackupFilename(string strFilename)
        {
            return Application.UserAppDataPath + @"\Backup of " + Path.GetFileName(strFilename);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveClicked();
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_strProjectFilename = null;
            SaveClicked();
        }

        protected void SetupTitleBar(string strProjectName, string strStoryName)
        {
            String str = String.Format("{0} -- {1} -- {2}", CstrCaption, strProjectName, strStoryName);
        }
        
        /*
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
        */

        private void viewFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ReInitVerseControls();
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
            // if (!splitContainerMentorNotes.Panel1Collapsed)
                foreach (Control ctrl in flowLayoutPanelConsultantNotes.Controls)
                {
                    if (ctrl is ConsultNotesControl)
                    {
                        ConsultNotesControl aConsultNoteCtrl = (ConsultNotesControl)ctrl;
                        aConsultNoteCtrl.UpdateHeight(Panel2_Width);
                    }
                }

            // if (!splitContainerMentorNotes.Panel2Collapsed)  these should be done even if invisible
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

            saveasToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled = ((Stories != null) && (Stories.Count > 0));
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
                MessageBox.Show(ex.Message, CstrCaption);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForSaveDirtyFile();
            this.Close();
        }

        protected string StoryName
        {
            get { return (string)comboBoxStorySelector.SelectedItem; }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert((Stories != null) && (Stories.Count > 0));
                return Stories.GetXml;
            }
        }

        internal void SetStatusBar(string strText, string strToolTipText)
        {
            statusLabel.Text = strText;
            statusLabel.ToolTipText = strToolTipText;
        }

        private void buttonsStoryStage_DropDownOpening(object sender, EventArgs e)
        {
            if ((Stories == null) || (theCurrentStory == null))
                return;

            buttonsStoryStage.DropDown.Items.Clear();

            // get the current StateTransition object and find all of the allowable transition states
            StoryStageLogic.StateTransition theCurrentST = StoryStageLogic.stateTransitions[theCurrentStory.ProjStage.ProjectStage];
            System.Diagnostics.Debug.Assert(theCurrentST != null);

            if (AddListOfButtons(theCurrentST.AllowableBackwardsTransitions))
                buttonsStoryStage.DropDown.Items.Add(new ToolStripSeparator());
            AddListOfButtons(theCurrentST.AllowableForwardsTransitions);
        }

        protected bool AddListOfButtons(List<StoryStageLogic.ProjectStages> allowableTransitions)
        {
            if (allowableTransitions.Count == 0)
                return false;

            foreach (StoryStageLogic.ProjectStages eAllowableTransition in allowableTransitions)
            {
                // put the allowable transitions into the DropDown list
                StoryStageLogic.StateTransition aST = StoryStageLogic.stateTransitions[eAllowableTransition];
                ToolStripItem tsi = buttonsStoryStage.DropDown.Items.Add(
                    aST.StageDisplayString, null, OnSelectOtherState);
                tsi.Tag = aST;
            }
            return true;
        }

        protected void OnSelectOtherState(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripItem);
            ToolStripItem tsi = (ToolStripItem)sender;
            StoryStageLogic.StateTransition theNewST = (StoryStageLogic.StateTransition)tsi.Tag;
            DoNextSeveral(theNewST);
        }

        protected void DoNextSeveral(StoryStageLogic.StateTransition theNewST)
        {
            if (!theCurrentStory.ProjStage.IsChangeOfStateAllowed(LoggedOnMember))
                return;

            // NOTE: the new state may actually be a previous state
            StoryStageLogic.StateTransition theCurrentST = null;
            do
            {
                theCurrentST = StoryStageLogic.stateTransitions[theCurrentStory.ProjStage.ProjectStage];
                
                // if we're going backwards, then just set the new state and update the view
                if ((int)theCurrentST.CurrentStage > (int)theNewST.CurrentStage)
                {
                    System.Diagnostics.Debug.Assert(theCurrentST.IsTransitionValid(theNewST.CurrentStage));
                    // if this is the last transition before they lose edit privilege, then make
                    //  sure they really want to do this.
                    if (theCurrentST.IsTerminalTransition(theNewST.CurrentStage))
                        if (MessageBox.Show(
                                String.Format(theCurrentST.TerminalTransitionMessage,
                                TeamMemberData.GetMemberTypeAsDisplayString(theNewST.MemberTypeWithEditToken), 
                                theNewST.StageDisplayString), 
                            CstrCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return;

                    theCurrentStory.ProjStage.ProjectStage = theNewST.CurrentStage;
                    SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage);
                    break;
                }
                else if (theCurrentST.CurrentStage != theNewST.CurrentStage)
                    if (!DoNextState(false))
                        break;
            }
            while (theCurrentST.NextState != theNewST.CurrentStage);
            InitAllPanes();
        }

        private void buttonsStoryStage_ButtonClick(object sender, EventArgs e)
        {
            DoNextState(true);
        }

        protected bool DoNextState(bool bDoUpdateCtrls)
        {
            System.Diagnostics.Debug.Assert((Stories != null) && (Stories.ProjSettings != null));
            if (SetNextStateIfReady())
            {
                SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage);
                if (bDoUpdateCtrls)
                    InitAllPanes();    // just in case there were changes
                return true;
            }
            return false;
        }

        protected bool SetNextStateIfReady()
        {
            if (!theCurrentStory.ProjStage.IsChangeOfStateAllowed(LoggedOnMember))
                return false;

            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[theCurrentStory.ProjStage.ProjectStage];
            bool bRet = st.IsReadyForTransition(Stories.ProjSettings, theCurrentStory);
            if (bRet)
            {
                StoryStageLogic.StateTransition stNext = StoryStageLogic.stateTransitions[st.NextState];
                if (st.IsTerminalTransition(st.NextState))
                    if (MessageBox.Show(
                            String.Format(st.TerminalTransitionMessage,
                                TeamMemberData.GetMemberTypeAsDisplayString(stNext.MemberTypeWithEditToken),
                                stNext.StageDisplayString), 
                            CstrCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                        return false;
                theCurrentStory.ProjStage.ProjectStage = st.NextState;  // if we are ready, then go ahead and transition
            }
            return bRet;
        }

        private void storyToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Enabled = ((theCurrentStory != null) &&
                                                                          (theCurrentStory.CraftingInfo != null));
            deleteStoryToolStripMenuItem.Enabled = (theCurrentStory != null);
            showFullStorySetToolStripMenuItem.Enabled = ((Stories != null) && (Stories.Count > 0));
        }

        private void enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryStoryPurpose();
        }

        private void showFullStorySetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanoramaView dlg = new PanoramaView(Stories);
            dlg.ShowDialog();
        }

        private void deleteStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory != null);
            int nIndex = Stories.IndexOf(theCurrentStory);
            Stories.RemoveAt(nIndex);
            System.Diagnostics.Debug.Assert(comboBoxStorySelector.Items.IndexOf(theCurrentStory.StoryName) == nIndex);
            comboBoxStorySelector.Items.Remove(theCurrentStory.StoryName);

            if (nIndex > 0)
                nIndex--;
            if (nIndex < Stories.Count)
                comboBoxStorySelector.SelectedItem = Stories[nIndex].StoryName;
        }

        /*
            Repository repo = new Repository(this, true);
            if (!repo.Exists)
                repo.Create();

            repo.SynchronizeWithRemote();
        */
    }
}