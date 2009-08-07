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
        protected const string cstrDefaultProjectStage = "CrafterTypeNationalBT";

        protected string m_strProjectFilename = null;

        protected StoryProject m_projFile = null;
        protected LoggedOnMemberInfo m_logonInfo = null;
        protected StoryStageLogic m_projStage = null;
        internal ProjectSettings ProjSettings = null;

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
                && (Properties.Settings.Default.LastUserType == TeamMemberForm.cstrCrafter)
                && (!String.IsNullOrEmpty(Properties.Settings.Default.LastProjectFile)))
            {
                OpenProjectFile(Properties.Settings.Default.LastProjectFile);
            }
#if false
            OpenProjectFile(@"C:\Code\StoryEditor\StoryEditor\StoryProject.onestory");
#else
            // InitializeProjectStage(cstrDefaultProjectStage);
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
            m_projFile = null;
            comboBoxStorySelector.Items.Clear();
        }

        protected void NewProjectFile()
        {
            CheckForSaveDirtyFile();
            CloseProjectFile();
            if (!InsureProjectPlusFrontMatter())
                return;
            GetLogin();
        }

        protected bool GetLogin()
        {
            // first figure out which team member it is:
            string strMemberName = null;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
            {
                strMemberName = Properties.Settings.Default.LastMemberLogin;

                foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                    if (aMemberRow.name == strMemberName)
                    {
                        // if (s)he's already logged on...
                        if ((m_logonInfo != null)
                            && (m_logonInfo.MemberName == strMemberName)
                            && (m_logonInfo.Type == TeamMemberForm.GetUserType(aMemberRow.memberType)))
                        {
                            Properties.Settings.Default.LastUserType = aMemberRow.memberType;
                            Properties.Settings.Default.Save();
                            return true;    // same person and role is already logged in
                        }

                        // otherwise, if it is a known member and is a crafter (who are more likely
                        //  not to function in multiple, different roles), then we're done also...
                        if (aMemberRow.memberType == TeamMemberForm.cstrCrafter)
                        {
                            m_logonInfo = new LoggedOnMemberInfo(strMemberName, aMemberRow.memberKey,
                                TeamMemberForm.GetUserType(aMemberRow.memberType));
                            Properties.Settings.Default.LastUserType = aMemberRow.memberType;
                            Properties.Settings.Default.Save();
                            return true;
                        }
                        // otherwise, fall thru and make them pick it.
                    }
            }

            return EditTeamMembers(strMemberName);
        }

        protected bool EditTeamMembers(string strMemberName)
        {
            System.Diagnostics.Debug.Assert((m_projFile != null) && (ProjSettings != null));

            // if we haven't found the member, then get them to select it from the Team Member UI
            TeamMemberForm dlg = new TeamMemberForm(m_projFile, ProjSettings);
            if (!String.IsNullOrEmpty(strMemberName))
            {
                try
                {
                    // if we did find it, but couldn't accept it without question, then at least pre-select member
                    dlg.SelectedMember = strMemberName;
                }
                catch { }    // might fail if the "last user" on this machine is opening this project file for the first time... just fall thru
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                    if (aMemberRow.name == dlg.SelectedMember)
                    {
                        m_logonInfo = new LoggedOnMemberInfo(dlg.SelectedMember, aMemberRow.memberKey, dlg.UserType);
                        Properties.Settings.Default.LastUserType = dlg.MemberTypeString;
                        Properties.Settings.Default.Save();
                        return true;
                    }
            }

            return false;
        }

        private void teamMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InsureProjectPlusFrontMatter())
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
            Properties.Settings.Default.LastProjectFile = strProjectFilename;
            Properties.Settings.Default.Save();

            CheckForSaveDirtyFile();
            CloseProjectFile();
            m_projFile = new StoryProject();
            try
            {
                System.Diagnostics.Debug.Assert(m_projFile != null);
                m_projFile.ReadXml(strProjectFilename);
                InsureProjectPlusFrontMatter();

                StoryProject.storyRow theStoryRow = null;
                string strStoryToLoad = null;
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

        protected bool InsureProjectPlusFrontMatter()
        {
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

            return true;
        }

        private void comboBoxStorySelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // user just finished entering a story name to select (or add)
            {
                StoryProject.storyRow theStoryRow = null;
                string strStoryToLoad = comboBoxStorySelector.Text;
                if ((m_projFile != null) && (m_projFile.story.Count > 0))
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
                        if (!InsureProjectPlusFrontMatter())
                            return; // user could enter nothing for a language name

                        System.Diagnostics.Debug.Assert((m_projFile != null) && (m_projFile.stories.Count == 1));
                        m_projFile.story.AddstoryRow(strStoryToLoad, cstrDefaultProjectStage, 
                            Guid.NewGuid().ToString(), m_projFile.stories[0]);
                        comboBoxStorySelector.SelectedItem = strStoryToLoad;
                    }
                }
                else
                    comboBoxStorySelector.SelectedItem = theStoryRow.name;
            }
        }

        protected void InsureVersesRow(StoryProject.storyRow aStoryRow)
        {
            System.Diagnostics.Debug.Assert(m_projFile != null);
            if (aStoryRow.GetversesRows().Length == 0)
                m_projFile.verses.AddversesRow(aStoryRow);
        }

        protected void SetTitleBar()
        {
            this.Text = String.Format("OneStory Editor -- {0} Story Project", m_projFile.stories[0].ProjectName);
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

            // find out which member we're working with (if it isn't already clear)
            if (!GetLogin())
                return;

            // find the story they've chosen (this shouldn't be possible to fail)
            StoryProject.storyRow theStoryRow = null;
            foreach (StoryProject.storyRow aStoryRow in m_projFile.story)
                if (aStoryRow.name == (string)comboBoxStorySelector.SelectedItem)
                {
                    theStoryRow = aStoryRow;
                    break;
                }
            System.Diagnostics.Debug.Assert(theStoryRow != null);

            // initialize the text box showing the storying they're editing
            textBoxStoryVerse.Text = "Story: " + theStoryRow.name;

            int nVerseIndex = 0;
            AddDropTargetToFlowLayout(nVerseIndex++);
            InsureVersesRow(theStoryRow);
            foreach (StoryProject.verseRow aRow in theStoryRow.GetversesRows()[0].GetverseRows())
            {
                VerseData vd = new VerseData(aRow, m_projFile);
                VerseBtControl aVerseCtrl = new VerseBtControl(this, vd, nVerseIndex);
                aVerseCtrl.UpdateHeight(Panel1_Width);
                flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                AddDropTargetToFlowLayout(nVerseIndex);

                ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(vd.ConsultantNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelConsultantNotes.Controls.Add(aConsultNotesCtrl);

                aConsultNotesCtrl = new ConsultNotesControl(vd.CoachNotes, nVerseIndex);
                aConsultNotesCtrl.UpdateHeight(Panel2_Width);
                flowLayoutPanelCoachNotes.Controls.Add(aConsultNotesCtrl);

                nVerseIndex++;
            }

            // initialize the project stage details (which might hide certain views)
            //  (do this *after* initializing the whole thing, because if we save, we'll
            //  want to save even the hidden pieces)
            InitializeProjectStage(theStoryRow.stage, theStoryRow.name, theStoryRow.guid);
        }

        protected void InitializeProjectStage(string strProjectStageString, string strStoryName, string strStoryGuid)
        {
            m_projStage = new StoryStageLogic(strProjectStageString, m_logonInfo, strStoryName, strStoryGuid);
            SetViewBasedOnProjectStage(m_projStage.ProjectStage);
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
            if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_strProjectFilename = saveFileDialog.FileName;
                SaveFile(m_strProjectFilename);
            }
        }

        DateTime m_dtLastSave = DateTime.Now;
        TimeSpan m_tsBetweenBackups = new TimeSpan(0, 0, 0);    // every time

        protected void SaveFile(string strFilename)
        {
            try
            {
                // create the root portions of the XML document and tack on the fragment we've been building
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(ns + "StoryProject",
                        GetXml));

                doc.Save(strFilename);
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

            Modified = false;

            // if it's been 5 minutes since our last backup...
            if ((DateTime.Now - m_dtLastSave) > m_tsBetweenBackups)
            {
                // ... hide a copy in the user's Application Data file
                File.Copy(strFilename, GetBackupFilename(strFilename), true);
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

            saveToolStripMenuItem.Enabled = ((m_projFile != null) && (m_projFile.stories.Count > 0) && !String.IsNullOrEmpty(m_projFile.stories[0].ProjectName));
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
            if (m_projStage == null)
                return;
            XComponent.SliderBar.MACTrackBar bar = (XComponent.SliderBar.MACTrackBar)sender;
            StoryStageLogic.ProjectStages eNewProjectStage = (StoryStageLogic.ProjectStages)bar.Value;
            Console.WriteLine(String.Format("ValueChanged: ProjectStage: {0}", eNewProjectStage.ToString()));
            if (m_projStage.CheckIfProjectTransitionIsAllowed(eNewProjectStage))
            {
                System.Diagnostics.Debug.Assert(eNewProjectStage == m_projStage.ProjectStage);
                SetViewBasedOnProjectStage(eNewProjectStage);
            }
        }

        protected string StoryName
        {
            get { return (string)comboBoxStorySelector.SelectedItem; }
        }

        public XElement GetXml
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

                XElement elemStories = new XElement(ns + "stories", new XAttribute("ProjectName", ProjSettings.ProjectName),
                    TeamMemberForm.GetXmlMembers(m_projFile),
                    ProjSettings.GetXml,
                    new XElement(ns + "story",
                        new XAttribute("name", StoryName),
                        new XAttribute("stage", m_projStage.ProjectStageString),
                        new XAttribute("guid", m_projStage.StoryGuid),
                        elemVerses));

                return elemStories;
            }
        }
    }
}
