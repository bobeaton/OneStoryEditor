// #define UsingOneFilePerStory

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OneStoryProjectEditor
{
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string cstrCaption = "OneStory Project Editor";
        internal const string cstrButtonDropTargetName = "buttonDropTarget";

        protected string m_strProjectFilename = null;

#if UsingOneFilePerStory
        protected StoryProject m_projFileFM = null;
#endif

        // protected StoryProject m_projFile = null;
        internal StoriesData Stories = null;
        internal StoryData theCurrentStory = null;

        // we keep a copy of this, because it ought to persist across multiple files
        protected TeamMemberData LoggedOnMember = null;

        internal static XNamespace ns = "http://www.sil.org/computing/schemas/StoryProject.xsd";

        protected bool Modified = false;

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

            if ((!String.IsNullOrEmpty(Properties.Settings.Default.LastUserType))
                && (Properties.Settings.Default.LastUserType == TeamMemberData.cstrCrafter)
                && (!String.IsNullOrEmpty(Properties.Settings.Default.LastProjectFile)))
            {
                OpenProjectFile(Properties.Settings.Default.LastProjectFile);
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
        }

        /* moved to TeamMemberData.cs
        protected bool GetLogin()
        {
            // this method shouldn't be called until after InsureProjectPlusFrontMatter
#if UsingOneFilePerStory
            System.Diagnostics.Debug.Assert((m_projFileFM != null) && (m_projFileFM.stories != null)
                && (m_projFileFM.stories.Count > 0) && (ProjSettings != null));
#else
            System.Diagnostics.Debug.Assert((m_projFile != null) && (m_projFile.stories != null) 
                && (m_projFile.stories.Count > 0) && (ProjSettings != null));
#endif

            // if someone is already logged on, then the last login and type *will* be the same--
            //  see TeamMemberForm.buttonOK_Click--which is just the criteria for auto login)
            //  NB: in this case, the only way to change the logged in person is "Project", "Settings"
            if (_dataTeamMembers != null)
            {
                if (_dataTeamMembers.LoggedOn != null)
                    return true;
            }
            else
#if UsingOneFilePerStory
                _dataTeamMembers = new TeamMembersData(m_projFileFM);
#else
                _dataTeamMembers = new TeamMembersData(m_projFile);
#endif

            // look at the last person to log in and see if we ought to automatically log them in again
            //  (basically Crafters or others that are also the same role as last time)
            string strMemberName = null;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
            {
                strMemberName = Properties.Settings.Default.LastMemberLogin;
                string strMemberTypeString = Properties.Settings.Default.LastUserType;
                if (_dataTeamMembers.CanLoginMember(strMemberName, strMemberTypeString))    // sets LoggedOn if returning true
                    return true;
            }

            // otherwise, fall thru and make them pick it.
            return EditTeamMembers(strMemberName);
        }

        protected bool EditTeamMembers(string strMemberName)
        {
#if UsingOneFilePerStory
            System.Diagnostics.Debug.Assert((m_projFileFM != null) && (m_projFileFM.stories != null)
                && (m_projFileFM.stories.Count > 0) && (ProjSettings != null));
#else
            System.Diagnostics.Debug.Assert((m_projFile != null) && (m_projFile.stories != null)
                && (m_projFile.stories.Count > 0) && (ProjSettings != null));
#endif

            // if we haven't found the member, then get them to select it from the Team Member UI
            if (_dataTeamMembers == null)
#if UsingOneFilePerStory
                _dataTeamMembers = new TeamMembersData(m_projFileFM);
#else
                _dataTeamMembers = new TeamMembersData(m_projFile);
#endif

            TeamMemberForm dlg = new TeamMemberForm(_dataTeamMembers, ProjSettings);
            if (!String.IsNullOrEmpty(strMemberName))
            {
                try
                {
                    // if we did find the "last member" in the list, but couldn't accept it without question
                    //  (e.g. because the role was different), then at least pre-select the member
                    dlg.SelectedMember = strMemberName;
                }
                catch { }    // might fail if the "last user" on this machine is opening this project file for the first time... just ignore
            }

            return (dlg.ShowDialog() == DialogResult.OK);
        }

        protected bool InsureProjectPlusFrontMatter()
        {
#if UsingOneFilePerStory
            if (m_projFileFM == null)
                m_projFileFM = new StoryProject();

            StoryProject.storiesRow theStoriesRow = null;
            if (m_projFileFM.stories.Count == 0)
            {
                string strLanguage = Microsoft.VisualBasic.Interaction.InputBox(String.Format("You are creating a brand new OneStory project. Enter the name you want to give this project (e.g. the language name).{0}{0}(if you had intended to edit an existing project, cancel this dialog and use the 'File', 'Open' command)", Environment.NewLine), cstrCaption, null, Screen.PrimaryScreen.WorkingArea.Right / 2, Screen.PrimaryScreen.WorkingArea.Bottom / 2);
                if (String.IsNullOrEmpty(strLanguage))
                    return false;

                // otherwise, add the new Stories row
                theStoriesRow = m_projFileFM.stories.AddstoriesRow(strLanguage);
                ProjSettings = null;    // have to restart with a new project
            }
            else
                theStoriesRow = m_projFileFM.stories[0];

            SetTitleBar();

            if (ProjSettings == null)
                ProjSettings = new ProjectSettings(m_projFileFM, theStoriesRow.ProjectName);
#else
            if (m_projFile == null)
                m_projFile = new StoryProject();

            StoryProject.storiesRow theStoriesRow = null;
            if (m_projFile.stories.Count == 0)
            {
                string strLanguage = Microsoft.VisualBasic.Interaction.InputBox(String.Format("You are creating a brand new OneStory project. Enter the name you want to give this project (e.g. the language name).{0}{0}(if you had intended to edit an existing project, cancel this dialog and use the 'File', 'Open' command)", Environment.NewLine), cstrCaption, null, Screen.PrimaryScreen.WorkingArea.Right / 2, Screen.PrimaryScreen.WorkingArea.Bottom / 2);
                if (String.IsNullOrEmpty(strLanguage))
                    return false;

                // otherwise, add the new Stories row
                theStoriesRow = m_projFile.stories.AddstoriesRow(strLanguage);
            }
            else
                theStoriesRow = m_projFile.stories[0];

            SetTitleBar();

            if (ProjSettings == null)
                ProjSettings = new ProjectSettings(m_projFile, theStoriesRow.ProjectName);
#endif

            return true;
        }

        protected void InsureVersesRow(StoryProject.storyRow aStoryRow)
        {
            System.Diagnostics.Debug.Assert(m_projFile != null);
            if (aStoryRow.GetversesRows().Length == 0)
                m_projFile.verses.AddversesRow(aStoryRow);
        }

        */

        private void teamMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Stories == null)
            {
                StoryProject projFile = new StoryProject();
                Stories = new StoriesData(projFile, LoggedOnMember);
            }

            Stories.EditTeamMembers(LoggedOnMember.Name);
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
                FileInfo[] aFIs = di.GetFiles(String.Format(cstrStoryFilenameFormat + "*.osp", ProjSettings.ProjectName));
                foreach (FileInfo aFI in aFIs)
                {
                    string strStoryName = Path.GetFileNameWithoutExtension(aFI.Name).Substring(cstrStoryFilenameFormat.Length);
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
                Stories = new StoriesData(projFile, LoggedOnMember);

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
                    Environment.NewLine, strProjectFilename, ex.Message), cstrCaption);
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
                int nInsertIndex = -1;
                StoryData theStory = null;
                string strStoryToLoad = comboBoxStorySelector.Text;
                if (Stories != null)
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
                    if (MessageBox.Show(String.Format("Unable to find the story '{0}'. Would you like to add a new one with that name?", strStoryToLoad), cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.Assert(!comboBoxStorySelector.Items.Contains(strStoryToLoad));
                        comboBoxStorySelector.Items.Add(strStoryToLoad);
                        theCurrentStory = new StoryData(strStoryToLoad, LoggedOnMember);
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
                StoryProject projFile = new StoryProject();
                Stories = new StoriesData(projFile, LoggedOnMember);
            }

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

            int nVerseIndex = 0;
            AddDropTargetToFlowLayout(nVerseIndex++);
            foreach (VerseData aVerse in theCurrentStory.Verses)
            {
                VerseBtControl aVerseCtrl = new VerseBtControl(this, aVerse, nVerseIndex);
                aVerseCtrl.UpdateHeight(Panel1_Width);
                flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                AddDropTargetToFlowLayout(nVerseIndex);

                ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(aVerse.ConsultantNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelConsultantNotes.Controls.Add(aConsultNotesCtrl);

                aConsultNotesCtrl = new ConsultNotesControl(aVerse.CoachNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelCoachNotes.Controls.Add(aConsultNotesCtrl);

                nVerseIndex++;
            }

            // initialize the project stage details (which might hide certain views)
            //  (do this *after* initializing the whole thing, because if we save, we'll
            //  want to save even the hidden pieces)
            SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage);
        }

        protected void SetViewBasedOnProjectStage(StoryStageLogic.ProjectStages eStage)
        {
            // m_bDisableInterrupts = true;
            switch (eStage)
            {
                case StoryStageLogic.ProjectStages.eCrafterTypeNationalBT:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = false;
                    viewAnchorFieldMenuItem.Checked = false;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = false;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = false;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = false;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterAddAnchors:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterAddStoryQuestions:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eConsultantAddRound1Notes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;  // Consultant can turn this on during 2nd pass (otherwise, I mushroom the stages)
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCoachReviewRound1Notes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = true;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = true;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRound1Notes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterOnlineReview1WithConsultant:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTest1:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTest1:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eConsultantAddRoundZNotes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCoachReviewRoundZNotes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = true;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eConsultantReviseRoundZNotes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = true;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRoundZNotes:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterOnlineReviewZWithConsultant:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTestZ:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = false;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTestZ:
                    viewVernacularLangFieldMenuItem.Checked = false;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = false;
                    viewConsultantNoteFieldMenuItem.Checked = false;
                    viewCoachNotesFieldMenuItem.Checked = false;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eTeamComplete:
                    viewVernacularLangFieldMenuItem.Checked = true;
                    viewNationalLangFieldMenuItem.Checked = true;
                    viewEnglishBTFieldMenuItem.Checked = true;
                    viewAnchorFieldMenuItem.Checked = true;
                    viewStoryTestingQuestionFieldMenuItem.Checked = true;
                    viewRetellingFieldMenuItem.Checked = true;
                    viewConsultantNoteFieldMenuItem.Checked = true;
                    viewCoachNotesFieldMenuItem.Checked = true;
                    viewNetBibleMenuItem.Checked = true;
                    break;
                case StoryStageLogic.ProjectStages.eUndefined:
                default:
                    m_bDisableInterrupts = false;
                    throw new ApplicationException(String.Format("This project was edited by a newer version of the {0} program. You have to update your version of the program to edit this project.", cstrCaption));
            };

            // for now, the progress bar is just the eStage value as an int
            if (eStage > StoryStageLogic.ProjectStages.eUndefined)
                macTrackBarProjectStages.Value = (int)eStage;

            m_bDisableInterrupts = false;
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
            /*
            StoryProject.storyRow theStoryRow = m_projFile.story[0];
            StoryProject.verseRow theVerseRow = null;
            foreach (StoryProject.verseRow aRow in theStoryRow.GetversesRows()[0].GetverseRows())
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
                if (aRow.guid == aVerseCtrl.Guid)
                    theVerseRow = aRow;
            }
            foreach (StoryProject.verseRow aRow in m_projFile.verse.Rows)
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
            }

            StoryProject.versesRow theVersesRow = null;
            theVersesRow.M
            m_projFile.verse.Rows.CopyTo(m_projFile.verse.Rows, nInsertionIndex);
            m_projFile.verse.Rows.RemoveAt(m_projFile.verse.Rows.IndexOf(theVerseRow));

            foreach (StoryProject.verseRow aRow in theStoryRow.GetversesRows()[0].GetverseRows())
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
                if (aRow.guid == aVerseCtrl.Guid)
                    theVerseRow = aRow;
            }
            foreach (StoryProject.verseRow aRow in m_projFile.verse.Rows)
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
            }

            m_projFile.verse.Rows.InsertAt(theVerseRow, nInsertionIndex - 1);

            foreach (StoryProject.verseRow aRow in theStoryRow.GetversesRows()[0].GetverseRows())
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
                if (aRow.guid == aVerseCtrl.Guid)
                    theVerseRow = aRow;
            }

            foreach (StoryProject.verseRow aRow in m_projFile.verse.Rows)
            {
                Console.WriteLine(String.Format("DoMove: nInsertionIndex: {0}; aRow.guid: {1}", nInsertionIndex, aRow.guid));
            }
            */
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
                res = MessageBox.Show("Do you want to save your changes?", cstrCaption, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                    SaveClicked();
                else if (res != DialogResult.Cancel)
                    Modified = false;
            }

            // do cleanup, because this is always called before starting something new (new file or empty project)
            flowLayoutPanelVerses.Controls.Clear();
            flowLayoutPanelConsultantNotes.Controls.Clear();
            flowLayoutPanelCoachNotes.Controls.Clear();
            textBoxStoryVerse.Text = "Story";
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
            doc.Save(strFilename + cstrExtraExtnToAvoidClobberingFilesWithFailedSaves);

            // backup the last version to appdata
            // Note: doing File.Move leaves the old file security settings rather than replacing them
            // based on the target directory. Copy, on the other hand, inherits
            // security settings from the target folder, which is what we want to do.
            if (File.Exists(strFilename))
                File.Copy(strFilename, GetBackupFilename(strFilename), true);
            File.Delete(strFilename);
            File.Copy(strFilename + cstrExtraExtnToAvoidClobberingFilesWithFailedSaves, strFilename, true);
            File.Delete(strFilename + cstrExtraExtnToAvoidClobberingFilesWithFailedSaves);
        }

        protected const string cstrExtraExtnToAvoidClobberingFilesWithFailedSaves = ".out";

#if UsingOneFilePerStory
        protected const string cstrStoryFilenameFormat = "{0} -- ";

        protected string FilenameFromStoryInfo(string strFrontMatterFilename, string strStoryName)
        {
            return String.Format(@"{0}\{1}{2}",
                Path.GetDirectoryName(strFrontMatterFilename),
                String.Format(cstrStoryFilenameFormat, ProjSettings.ProjectName),
                strStoryName + ".osp");
        }
#endif

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
                // trigger the main display controls to update the data
                foreach (Control ctrl in flowLayoutPanelVerses.Controls)
                {
                    if (ctrl is VerseBtControl)
                    {
                        VerseBtControl aVerseCtrl = (VerseBtControl)ctrl;
                        aVerseCtrl.UpdateData();
                    }
                }

                foreach (ConsultNotesControl ctrl in flowLayoutPanelConsultantNotes.Controls)
                {
                    ctrl.UpdateData();
                }

                foreach (ConsultNotesControl ctrl in flowLayoutPanelCoachNotes.Controls)
                {
                    ctrl.UpdateData();
                }

                // let's see if the UNS entered the purpose of this story
                System.Diagnostics.Debug.Assert((theCurrentStory != null) && (theCurrentStory.CraftingInfo != null));
                if (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.StoryPurpose))
                {
                    string strStoryPurpose = Microsoft.VisualBasic.Interaction.InputBox(String.Format("Enter a brief description of the purpose of this story (that is, why is this story in the set?)", Environment.NewLine), cstrCaption, null, Screen.PrimaryScreen.WorkingArea.Right / 2, Screen.PrimaryScreen.WorkingArea.Bottom / 2);
                    if (!String.IsNullOrEmpty(strStoryPurpose))
                        theCurrentStory.CraftingInfo.StoryPurpose = strStoryPurpose;
                }

#if UsingOneFilePerStory
                SaveXElement(GetStoryXml, strStoryFilename);
#else
                SaveXElement(GetXml, strFilename);
#endif
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(String.Format("The project file '{0}' is locked. Is it read-only? Or opened in some other program? Unlock it and try again. Or try to save it as a different name.", strFilename), cstrCaption);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to save the project file '{1}'{0}{0}{2}", Environment.NewLine, strFilename, ex.Message), cstrCaption);
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
            if (!m_bDisableInterrupts)
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
                MessageBox.Show(ex.Message, cstrCaption);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForSaveDirtyFile();
            this.Close();
        }

        private void macTrackBarProjectStages_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            XComponent.SliderBar.MACTrackBar bar = (XComponent.SliderBar.MACTrackBar)sender;
            Console.WriteLine(String.Format("HelpRequested: value: {0}", bar.Value));
        }

        protected bool m_bDisableInterrupts = false;
        private void macTrackBarProjectStages_ValueChanged(object sender, decimal value)
        {
            if (theCurrentStory == null)
                return;
            System.Diagnostics.Debug.Assert(theCurrentStory.ProjStage != null);

            XComponent.SliderBar.MACTrackBar bar = (XComponent.SliderBar.MACTrackBar)sender;
            StoryStageLogic.ProjectStages eNewProjectStage = (StoryStageLogic.ProjectStages)bar.Value;
            Console.WriteLine(String.Format("ValueChanged: ProjectStage: {0}", eNewProjectStage.ToString()));
            if (theCurrentStory.ProjStage.CheckIfProjectTransitionIsAllowed(eNewProjectStage))
            {
                System.Diagnostics.Debug.Assert(eNewProjectStage == theCurrentStory.ProjStage.ProjectStage);
                SetViewBasedOnProjectStage(eNewProjectStage);
            }
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
    }
}