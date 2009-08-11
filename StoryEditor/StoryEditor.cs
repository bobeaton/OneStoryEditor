// #define UsingOneFilePerStory

using System;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;

namespace OneStoryProjectEditor
{
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string CstrCaption = "OneStory Project Editor";
        internal const string CstrButtonDropTargetName = "buttonDropTarget";

        protected string m_strProjectFilename = null;

#if UsingOneFilePerStory
        protected StoryProject m_projFileFM = null;
#endif

        // protected StoryProject m_projFile = null;
        internal StoriesData Stories = null;
        internal StoryData theCurrentStory = null;

        // we keep a copy of this, because it ought to persist across multiple files
        internal TeamMemberData LoggedOnMember = null;

        internal static XNamespace ns = "http://www.sil.org/computing/schemas/StoryProject.xsd";

        internal bool Modified = false;

        protected const int nMaxRecentFiles = 15;

        public enum UserTypes
        {
            eUndefined = 0,
            eCrafter,
            eUNS,
            eConsultantInTraining,
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
                MessageBox.Show(String.Format("Problem initializing Sword (the Net Bible viewer):{0}{0}{1}", Environment.NewLine, ex.Message), StoryEditor.CstrCaption);
            }

            if ((!String.IsNullOrEmpty(Properties.Settings.Default.LastUserType))
                && (Properties.Settings.Default.LastUserType == TeamMemberData.CstrCrafter)
                && (!String.IsNullOrEmpty(Properties.Settings.Default.LastProjectFile)))
            {
                OpenProjectFile(Properties.Settings.Default.LastProjectFile);
            }
            else
            {
                NewProjectFile();
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

        protected void CloseProjectFile()
        {
            System.Diagnostics.Debug.Assert(!Modified);
            Stories = null;
            theCurrentStory = null;
#if UsingOneFilePerStory
            m_projFileFM = null;
#endif
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
        }

        private void teamMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Stories == null)
            {
                try
                {
                    Stories = new StoriesData(this);
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
                    Stories.EditTeamMembers(LoggedOnMember.Name);
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
#if UsingOneFilePerStory
            m_projFileFM = new StoryProject();  // one for the front matter file
            try
            {
                m_projFileFM.ReadXml(strProjectFilename);
                m_strProjectFilename = strProjectFilename;  // so Save means "Save" rather than "Save As"
                InsureProjectPlusFrontMatter();

                StoryProject.storyRow theStoryRow = null;
                string strStoryToLoad = null;
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastStoryWorkedOn))
                    strStoryToLoad = Properties.Settings.Default.LastStoryWorkedOn;

                // populate the combo boxes with all the existing story names
                DirectoryInfo di = new DirectoryInfo(ProjSettings.ProjectFolder);
                FileInfo[] aFIs = di.GetFiles(String.Format(CstrStoryFilenameFormat + "*.osp", ProjSettings.ProjectName));
                foreach (FileInfo aFI in aFIs)
                {
                    string strStoryName = Path.GetFileNameWithoutExtension(aFI.Name).Substring(CstrStoryFilenameFormat.Length);
#if DEBUG
                    // in debug, let's go ahead and try to load it!
                    string strStoryFilename = aFI.Name;
                    System.Diagnostics.Debug.Assert(File.Exists(strStoryFilename));
                    StoryProject projFile = new StoryProject();
                    projFile.ReadXml(strStoryFilename);
                    if (projFile.story.Count > 0)
                        theStoryRow = projFile.story[0];
                    System.Diagnostics.Debug.Assert((theStoryRow != null) && (theStoryRow.name == strStoryName));
#endif
                    comboBoxStorySelector.Items.Add(strStoryName);
                }

                if ((!String.IsNullOrEmpty(strStoryToLoad)) && (comboBoxStorySelector.Items.Contains(strStoryToLoad)))
                    comboBoxStorySelector.SelectedItem = strStoryToLoad;
                else if (comboBoxStorySelector.Items.Count > 0)
                    comboBoxStorySelector.SelectedIndex = 0;
#else
            try
            {
                StoryProject projFile = new StoryProject();
                projFile.ReadXml(strProjectFilename);

                // get *all* the data
                Stories = new StoriesData(projFile, this);

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
#endif
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(String.Format("Unable to open project file '{1}'{0}{0}{2}{0}{0}Send the project file to bob_eaton@sall.com for help",
                    Environment.NewLine, strProjectFilename, ex.Message), CstrCaption);
            }
        }

        private void comboBoxStorySelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // user just finished entering a story name to select (or add)
            {
#if UsingOneFilePerStory
                string strStoryToLoad = comboBoxStorySelector.Text;
                // see if we have a file that correponds to this...
                string strStoryFilename = FilenameFromStoryInfo(m_strProjectFilename, strStoryToLoad);
                if (!File.Exists(strStoryFilename))
                    //TODO: I got stuck here doing the 'UsingOneFilePerStory' refactoring
#else
                if (Stories == null)
                    Stories = new StoriesData(this);

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
#endif
                {
                    if (MessageBox.Show(String.Format("Unable to find the story '{0}'. Would you like to add a new one with that name?", strStoryToLoad), CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.Assert(!comboBoxStorySelector.Items.Contains(strStoryToLoad));
                        comboBoxStorySelector.Items.Add(strStoryToLoad);
                        theCurrentStory = Stories.AddStory(strStoryToLoad, this);
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
                Stories = new StoriesData(this);

#if UsingOneFilePerStory
            string strStoryFilename = FilenameFromStoryInfo(m_strProjectFilename, (string)comboBoxStorySelector.SelectedItem);
            StoryProject.storyRow theStoryRow = null;
            if (File.Exists(strStoryFilename))
            {
                m_projFile = new StoryProject();
                m_projFile.ReadXml(strStoryFilename);
                if (m_projFile.story.Count == 1)
                    theStoryRow = m_projFile.story[0];
            }
#else
            // find the story they've chosen (this shouldn't be possible to fail)
            foreach (StoryData aStory in Stories)
                if (aStory.StoryName == (string)comboBoxStorySelector.SelectedItem)
                {
                    theCurrentStory = aStory;
                    break;
                }
#endif
            System.Diagnostics.Debug.Assert(theCurrentStory != null);

            // initialize the text box showing the storying they're editing
            textBoxStoryVerse.Text = "Story: " + theCurrentStory.StoryName;
            this.Text = String.Format("OneStory Editor -- {0} Story Project", Stories.ProjSettings.ProjectName);

            InitVerseControls();

            // initialize the project stage details (which might hide certain views)
            //  (do this *after* initializing the whole thing, because if we save, we'll
            //  want to save even the hidden pieces)
            SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage);
        }

        internal void InitVerseControls(VersesData theVerses)
        {
            ClearFlowControls();
            int nVerseIndex = 0;
            if (theVerses.Count == 0)
                theCurrentStory.Verses.InsertVerse(0, "<Type the UNS's back translation>");

            AddDropTargetToFlowLayout(nVerseIndex++);
            foreach (VerseData aVerse in theVerses)
            {
                VerseBtControl aVerseCtrl = new VerseBtControl(this, aVerse, nVerseIndex);
                aVerseCtrl.UpdateHeight(Panel1_Width);
                flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                AddDropTargetToFlowLayout(nVerseIndex);

                ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(theCurrentStory.ProjStage, aVerse.ConsultantNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelConsultantNotes.Controls.Add(aConsultNotesCtrl);

                aConsultNotesCtrl = new ConsultNotesControl(theCurrentStory.ProjStage, aVerse.CoachNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelCoachNotes.Controls.Add(aConsultNotesCtrl);

                nVerseIndex++;
            }
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
            InitVerseControls();
        }

        internal void AddNewVerse(VerseBtControl theVerse, string strNationalBT)
        {
            int nInsertionIndex = theVerse.VerseNumber;
            VerseData theNewVerse = theCurrentStory.Verses.InsertVerse(nInsertionIndex, strNationalBT);
        }

        internal void InitVerseControls()
        {
            try
            {
                InitVerseControls(theCurrentStory.Verses);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to continue! Cause: {0}", ex.Message), CstrCaption);
                return;
            }
        }

        internal void DeleteVerse(VerseBtControl theVerseToDelete)
        {
            theCurrentStory.Verses.Remove(theVerseToDelete._verseData);
            InitVerseControls();
        }

        protected void SetViewBasedOnProjectStage(StoryStageLogic.ProjectStages eStage)
        {
            string strStatusMsg, strStatusTooltipMsg;
            theCurrentStory.ProjStage.SetViewBasedOnProjectStage(this, eStage, out strStatusMsg, out strStatusTooltipMsg);
            helpProvider.SetHelpString(this, strStatusTooltipMsg);
            SetStatusBar(String.Format("{0}  Press F1 for instructions", strStatusMsg), strStatusTooltipMsg);
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
            buttonDropTarget.DragEnter += new DragEventHandler(buttonDropTarget_DragEnter);
            buttonDropTarget.DragDrop += new DragEventHandler(buttonDropTarget_DragDrop);
            flowLayoutPanelVerses.Controls.Add(buttonDropTarget);
            return buttonDropTarget;
        }

        void buttonDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                System.Diagnostics.Debug.Assert(sender is Button);
                int nInsertionIndex = flowLayoutPanelVerses.Controls.IndexOf((Button)sender);
                DoMove(nInsertionIndex, aVerseCtrl);
            }
        }

        void DumpFlow()
        {
            for (int i = 0; i < flowLayoutPanelVerses.Controls.Count; i++)
            {
                Control ctrl = flowLayoutPanelVerses.Controls[i];
                if (ctrl is VerseBtControl)
                    Console.WriteLine(String.Format("{0}: verse: {1}", i.ToString(), ((VerseBtControl)ctrl).VerseNumber.ToString()));
                else
                    Console.WriteLine(String.Format("{0}: button: {1}", i.ToString(), ctrl.Name));
            }
        }

        void DoMove(int nInsertionIndex, VerseBtControl aVerseCtrl)
        {
            DumpFlow();
            int nIndex = flowLayoutPanelVerses.Controls.IndexOf(aVerseCtrl);
            System.Diagnostics.Debug.Assert(Math.Abs(nIndex - nInsertionIndex) > 1);
            Control btnAfter = flowLayoutPanelVerses.Controls[nIndex + 1];
            flowLayoutPanelVerses.Controls.SetChildIndex(aVerseCtrl, nInsertionIndex);
            DumpFlow();
            flowLayoutPanelVerses.Controls.SetChildIndex(btnAfter, nInsertionIndex);
            DumpFlow();
            if (nIndex > nInsertionIndex)
                nIndex = nInsertionIndex + 1;
            for (int i = nIndex; i < flowLayoutPanelVerses.Controls.Count; i += 2, nIndex++)
            {
                Control ctrl = flowLayoutPanelVerses.Controls[i];
                System.Diagnostics.Debug.Assert(ctrl is VerseBtControl);
                aVerseCtrl = (VerseBtControl)ctrl;
                aVerseCtrl.VerseNumber = nIndex;
            }
            DumpFlow();
            UpdateVersePanel();
        }

        void buttonDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                int nIndex = flowLayoutPanelVerses.Controls.IndexOf(aVerseCtrl);
                Button btnTarget = (Button)sender;
                int nInsertionIndex = flowLayoutPanelVerses.Controls.IndexOf(btnTarget);
                System.Diagnostics.Debug.Assert(Math.Abs(nIndex - nInsertionIndex) > 1);
                e.Effect = DragDropEffects.Move;
                /*
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                string strTargetName = btnTarget.Name;
                string strTargetVerse = strTargetName.Substring(CstrButtonDropTargetName.Length);
                int nInsertionIndex = (int)Convert.ToInt32(strTargetVerse);
                if ((nInsertionIndex < (aVerseCtrl.VerseNumber - 1)) || (nInsertionIndex > (aVerseCtrl.VerseNumber)))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
                */
            }
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
                new XElement(ns + "StoryProject",
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

#if UsingOneFilePerStory
        protected const string CstrStoryFilenameFormat = "{0} -- ";

        protected string FilenameFromStoryInfo(string strFrontMatterFilename, string strStoryName)
        {
            return String.Format(@"{0}\{1}{2}",
                Path.GetDirectoryName(strFrontMatterFilename),
                String.Format(CstrStoryFilenameFormat, ProjSettings.ProjectName),
                strStoryName + ".osp");
        }
#endif

        protected void QueryStoryPurpose()
        {
            StoryFrontMatterForm dlg = new StoryFrontMatterForm(Stories, theCurrentStory);
            dlg.ShowDialog();
        }

        protected void SaveFile(string strFilename)
        {
            try
            {
#if UsingOneFilePerStory
                // for the story, let's use the project name as part of it (in case the user tries to save multiple
                //  in the same folder) and the story name as part of it (so they don't collide)
                string strStoryFilename = FilenameFromStoryInfo(strFilename, _StorySettings.StoryName);

                // for the initial release, I'm going to save the project settings/front matter in one file and the 
                //  the stories in each their own file. So we'll be writing two documents here.
                SaveXElement(GetFrontMatterXml, strFilename);
#endif
                // let's see if the UNS entered the purpose of this story
                System.Diagnostics.Debug.Assert((theCurrentStory != null) && (theCurrentStory.CraftingInfo != null));
                if (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.StoryPurpose))
                    QueryStoryPurpose();

#if UsingOneFilePerStory
                SaveXElement(GetStoryXml, strStoryFilename);
#else
                SaveXElement(GetXml, strFilename);
#endif
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

        private string GetBackupFilename(string strFilename)
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

#if UsingOneFilePerStory
        public XElement GetFrontMatterXml
        {
            get
            {
                System.Diagnostics.Debug.Assert((m_projFile != null) && (m_projFile.stories.Count > 0));

                return new XElement(ns + "stories", new XAttribute("ProjectName", ProjSettings.ProjectName),
                    TeamMemberForm.GetXmlMembers(m_projFile),
                    ProjSettings.GetXml);
            }
        }

        public XElement GetStoryXml
        {
            get
            {
                System.Diagnostics.Debug.Assert((m_projFile != null) && (m_projFile.stories.Count > 0));

                XElement elemVerses = new XElement(ns + "verses");

                foreach (Control ctrl in flowLayoutPanelVerses.Controls)
                {
                    if (ctrl is VerseBtControl)
                    {
                        VerseBtControl aVerseBtCtrl = (VerseBtControl)ctrl;
                        elemVerses.Add(aVerseBtCtrl.VerseData.GetXml);
                    }
                }

                XElement elemStory = _StorySettings.GetXml;
                elemStory.Add(elemVerses);

                XElement elemStories = new XElement(ns + "stories", new XAttribute("ProjectName", ProjSettings.ProjectName),
                    elemStory);

                return elemStories;
            }
        }
#else
        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert((Stories != null) && (Stories.Count > 0));
                return Stories.GetXml;
            }
        }
#endif
        internal void SetStatusBar(string strText, string strToolTipText)
        {
            statusLabel.Text = strText;
            statusLabel.ToolTipText = strToolTipText;
        }

        private void buttonsStoryStage_DropDownOpening(object sender, EventArgs e)
        {
            buttonsStoryStage.DropDown.Items.Clear();
            buttonsStoryStage.DropDown.Items.Add("Next Stage", null, new EventHandler(buttonsStoryStage_ButtonClick));
            buttonsStoryStage.DropDown.Items.Add("Send to consultant");
        }

        private void buttonsStoryStage_ButtonClick(object sender, EventArgs e)
        {
            DoNextStage();
        }

        protected void DoNextStage()
        {
        }

        private void storyToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Enabled = ((theCurrentStory != null) &&
                                                                          (theCurrentStory.CraftingInfo != null));
            showFullStorySetToolStripMenuItem.Enabled = ((Stories != null) && (Stories.Count > 0));
        }

        private void enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryStoryPurpose();
        }

        private void showFullStorySetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanoramaView dlg = new PanoramaView(Stories);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
            }
        }
    }
}