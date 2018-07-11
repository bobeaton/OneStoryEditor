#define UsingHtmlDisplayForConNotes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using Chorus.UI.Clone;
using ECInterfaces;
using Microsoft.Win32;
using OneStoryProjectEditor.Properties;
using SilEncConverters40;
using System.Diagnostics;               // Process
using devX;
using Control=System.Windows.Forms.Control;
using Timer=System.Windows.Forms.Timer;
using NetLoc;
using SIL.Reporting;
#if EmbedSayMore
using SayMore.UI.SessionRecording;
#endif

namespace OneStoryProjectEditor
{
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string CstrButtonDropTargetName = "buttonDropTarget";

        internal StoryProjectData StoryProject;
        internal string CurrentStoriesSetName;

        private StoryData _theCurrentStory;
        public StoryData TheCurrentStory
        {
            get { return _theCurrentStory; }
            set
            {
                _theCurrentStory = value;
                // since the task link visibility is based on this too, check and update
                if (LoggedOnMember != null)
                    UpdateTaskLinkVisibility(LoggedOnMember);
            }
        }

        // we keep a copy of this, because it ought to persist across multiple files
        private TeamMemberData _loggedOnMember;
        internal TeamMemberData LoggedOnMember
        {
            get { return _loggedOnMember; }
            set
            {
                _loggedOnMember = value;
                if ((StoryProject != null)
                    && (StoryProject.ProjSettings != null)
                    && (File.Exists(StoryProject.ProjSettings.ProjectFolder)))
                {
                    InitProjectNotes(StoryProject.ProjSettings, value.Name);
                }

                if (value == null)
                    return;

                UpdateTaskLinkVisibility(value);

                // check whether we should be showing the transliteration or not
                viewTransliterationVernacular.Checked = (value.TransliteratorVernacular != null);
                viewTransliterationNationalBT.Checked = (value.TransliteratorNationalBt != null);
                viewTransliterationInternationalBt.Checked = (value.TransliteratorInternationalBt != null);
                viewTransliterationFreeTranslation.Checked = (value.TransliteratorFreeTranslation != null);

                // update the frame title if a new person logs in
                Text = GetFrameTitle(true);
            }
        }

        private void UpdateTaskLinkVisibility(TeamMemberData loggedOnMember)
        {
            linkLabelTasks.Visible = TeamMemberData.IsUser(loggedOnMember.MemberType,
                                                           TeamMemberData.UserTypes.ProjectFacilitator) ||
                                     (TeamMemberData.IsUser(loggedOnMember.MemberType,
                                                            TeamMemberData.UserTypes.JustLooking |
                                                            TeamMemberData.UserTypes.EnglishBackTranslator |
                                                            TeamMemberData.UserTypes.IndependentConsultant |
                                                            TeamMemberData.UserTypes.ConsultantInTraining |
                                                            TeamMemberData.UserTypes.Coach) &&
                                      (TheCurrentStory != null));
        }

        internal bool Modified;
        internal Timer myFocusTimer = new Timer();
        internal Timer myReopenTimer = new Timer();
        internal static Timer mySaveTimer = new Timer();

        private const int CnIntervalBetweenAutoSaveReqs = 5 * 1000 * 60;
        protected DateTime tmLastSync = DateTime.Now;
        protected TimeSpan tsBackupTime = new TimeSpan(1, 0, 0);

        private NoteForm m_dlgNotes;
        private RevisionHistoryForm m_dlgHistDiffDlg;
        private PrintForm m_dlgPrintForm;

        public static TextPaster TextPaster = null;

        [Flags]
        public enum TextFields
        {
            Undefined = 0,

            // each field will be one of these 4
            Vernacular = 1,
            NationalBt = 2,
            InternationalBt = 4,
            FreeTranslation = 8,

            // and one of these
            StoryLine = 16,
            Anchor = 32,
            ExegeticalNote = 64,
            Retelling = 128,
            TestQuestion = 256,
            TestQuestionAnswer = 512,
            ConsultantNote = 1024,
            CoachNote = 2048,

            Languages = Vernacular | NationalBt | InternationalBt | FreeTranslation,
            Fields = StoryLine | Anchor | ExegeticalNote | Retelling |
                     TestQuestion | TestQuestionAnswer | ConsultantNote | CoachNote,

            Everything = Languages | Fields
        }

        public static bool IsFieldSet(TextFields eValues, TextFields eValue)
        {
            return ((eValues & eValue) != TextFields.Undefined);
        }

        public struct LocalizedEnum<T> where T : struct
        {
            private T _value;
            private static string LocalizedStoryLine { get { return Localizer.Str("StoryLine"); } }
            private static string LocalizedAnchor { get { return Localizer.Str("Anchor"); } }
            private static string LocalizedExegeticalNote { get { return Localizer.Str("ExegeticalNote"); } }
            private static string LocalizedRetelling { get { return Localizer.Str("Retelling"); } }
            private static string LocalizedTestQuestion { get { return Localizer.Str("TestQuestion"); } }
            private static string LocalizedTestQuestionAnswer { get { return Localizer.Str("TestQuestionAnswer"); } }
            private static string LocalizedConsultantNote { get { return Localizer.Str("ConsultantNote"); } }
            private static string LocalizedCoachNote { get { return Localizer.Str("CoachNote"); } }

            public LocalizedEnum(T value)
            {
                _value = value;
            }

            public override string ToString()
            {
                Debug.Assert(_value is TextFields);
                if (Equals(_value, TextFields.StoryLine))
                    return LocalizedStoryLine;
                if (Equals(_value, TextFields.Anchor))
                    return LocalizedAnchor;
                if (Equals(_value, TextFields.ExegeticalNote))
                    return LocalizedExegeticalNote;
                if (Equals(_value, TextFields.Retelling))
                    return LocalizedRetelling;
                if (Equals(_value, TextFields.TestQuestion))
                    return LocalizedTestQuestion;
                if (Equals(_value, TextFields.TestQuestionAnswer))
                    return LocalizedTestQuestionAnswer;
                if (Equals(_value, TextFields.ConsultantNote))
                    return LocalizedConsultantNote;
                return Equals(_value, TextFields.CoachNote)
                        ? LocalizedCoachNote 
                        : _value.ToString();
            }

            public static TextFields Parse(string str)
            {
                if (str == LocalizedStoryLine)
                    return TextFields.StoryLine;
                if (str == LocalizedAnchor)
                    return TextFields.Anchor;
                if (str == LocalizedExegeticalNote)
                    return TextFields.ExegeticalNote;
                if (str == LocalizedRetelling)
                    return TextFields.Retelling;
                if (str == LocalizedTestQuestion)
                    return TextFields.TestQuestion;
                if (str == LocalizedTestQuestionAnswer)
                    return TextFields.TestQuestionAnswer;
                if (str == LocalizedConsultantNote)
                    return TextFields.ConsultantNote;
                return (str == LocalizedCoachNote)
                            ? TextFields.CoachNote
                            : (TextFields) Enum.Parse(typeof (TextFields), str);
            }

            public static implicit operator LocalizedEnum<T>(T value)
            {
                return new LocalizedEnum<T>(value);
            }

            public static implicit operator T(LocalizedEnum<T> value)
            {
                return value._value;
            }
        }

        // needed by NetLoc
        private StoryEditor()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public StoryEditor(string strProjectFilePath)
        {
            myFocusTimer.Tick += TimeToSetFocus;
            myFocusTimer.Interval = 200;
            myReopenTimer.Tick += CheckForFileChanged;
            myReopenTimer.Interval = 1000;  // check every second

            InitializeComponent();
            Localizer.Ctrl(this);

            InitStoryBtPaneControl(Settings.Default.UsingHtmlForStoryBtPane);

            linkLabelConsultantNotes.Text = CstrFirstVerse;
            linkLabelCoachNotes.Text = CstrFirstVerse;

            panoramaToolStripMenu.Visible = true;   // not sure why this would ever be false IsInStoriesSet;
            viewUseSameSettingsForAllStoriesMenu.Checked = Settings.Default.LastUseForAllStories;
            advancedSaveTimeoutEnabledMenu.Checked = Settings.Default.AutoSaveTimeoutEnabled;
            advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked = Settings.Default.DoAutoSaveSilently;
            advancedUseDialogToPreviewConNotes.Checked = Settings.Default.UsePreviewDialogWhenAddingConNotes;
            viewUseSameSettingsForAllStoriesMenu.Checked = Settings.Default.LastUseForAllStories;
            advancedEmailMenu.Checked = Settings.Default.UseMapiPlus;
            advancedUseWordBreaks.Enabled = BreakIterator.IsAvailable;
            advancedAutomaticallyLoadProjectMenu.Checked = Settings.Default.AutoLoadLastProject;

            if (advancedSaveTimeoutEnabledMenu.Checked)
            {
                //autosave timer goes off every 5 minutes.
                mySaveTimer.Tick += TimeToSave;
                mySaveTimer.Interval = CnIntervalBetweenAutoSaveReqs;
                mySaveTimer.Start();
            }

            openFileDialog.InitialDirectory = ProjectSettings.OneStoryProjectFolderRoot;

            try
            {
                InitializeNetBibleViewer();
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(String.Format(Resources.IDS_NeedToReboot, Environment.NewLine, ex.Message),
                                OseCaption);
            }

            if (!String.IsNullOrEmpty(strProjectFilePath) && File.Exists(strProjectFilePath))
            {
                string strProjectPath = Path.GetDirectoryName(strProjectFilePath);
                string strProjectName = Path.GetFileNameWithoutExtension(strProjectFilePath);
                OpenProject(strProjectPath, strProjectName);
            }
            else
            {
                try
                {
                    if (String.IsNullOrEmpty(Settings.Default.LastUserType))
                        NewProjectFile();
                    else if (advancedAutomaticallyLoadProjectMenu.Checked)
                    {
                        var eRole = TeamMemberData.GetMemberType(Settings.Default.LastUserType);
                        if (TeamMemberData.IsUser(eRole, TeamMemberData.UserTypes.ProjectFacilitator)
                            && !String.IsNullOrEmpty(Settings.Default.LastProject))
                        {
                            OpenProject(Settings.Default.LastProjectPath, Settings.Default.LastProject);
                        }
                    }
                }
                catch (Program.RestartException)
                {
                    throw;
                }
                catch (DuplicateStoryStateTransitionException)
                {
                    // pass this one on so it triggers an email
                    throw;
                }
                catch { }   // this was only a bene anyway, so just ignore it
            }

#if !DEBUGBOB
            if (Settings.Default.AutoCheckForProgramUpdatesAtStartup)
                backgroundWorker.RunWorkerAsync();
#endif

#if EmbedSayMore
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(FindSaymoreAssembly);

                SaymoreInterface.LaunchSessionRecorder();
            }
            catch (Exception)
            {
            }
#endif
        }

        private static Assembly FindSaymoreAssembly(object sender, ResolveEventArgs args)
        {
            // "C:\Program Files (x86)\SayMore\SayMore.exe"
            var sayMoreAssembly = Assembly.LoadFrom(@"C:\Program Files (x86)\SayMore\SayMore.exe");

            //Return the loaded assembly.
            return sayMoreAssembly;
        }

        private void InitializeCurrentStoriesSetName(string strStoriesSet)
        {
            CurrentStoriesSetName = strStoriesSet;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string strManifestAddress = Program.IDS_OSEUpgradeServer;
            AutoUpgrade autoUpgrade = AutoUpgrade.Create(strManifestAddress, false);
            if (autoUpgrade.IsUpgradeAvailable(false))
                e.Result = autoUpgrade;
            else
                e.Result = null;

            return;
        }

        private AutoUpgrade _autoUpgrade;

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _autoUpgrade = e.Result as AutoUpgrade;

                //  info the user about the new release available
                if (_autoUpgrade != null)
                {
                    LocalizableMessageBox.Show(Localizer.Str("There's a new version of OSE available. To upgrade, click 'Advanced', 'Program Updates', 'Check now'"),
                                               OseCaption);
                }
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
            finally
            {
                // don't need this anymore
                backgroundWorker = null;
            }
        }

        internal bool _bAutoHide = false;
        private const int CnSecondsToDelyLastKeyPress = 7;
        private DateTime _tmLastKeyPressedTimeStamp;
        internal DateTime LastKeyPressedTimeStamp
        {
            get { return _tmLastKeyPressedTimeStamp; }
            set
            {
                _tmLastKeyPressedTimeStamp = value;

                // if the Bible Pane's auto hide checkbox is unchecked, then 
                //  hide it when typing
                if (!netBibleViewer.checkBoxAutoHide.Checked)
                {
                    _bAutoHide = true;
                    splitContainerUpDown.Minimize();
                }
            }
        }

        public static int SuspendSaveDialog;
        protected TimeSpan tsLastKeyPressDelay = new TimeSpan(0, 0, CnSecondsToDelyLastKeyPress);

        private void TimeToSave(object sender, EventArgs e)
        {
            mySaveTimer.Stop();

            if (Modified
                && !((LoggedOnMember != null)
                        && TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                                 TeamMemberData.UserTypes.JustLooking)))
            {
                // don't do it *now* if the user is typing
                if ((SuspendSaveDialog > 0) ||
                    (DateTime.Now - LastKeyPressedTimeStamp) < tsLastKeyPressDelay)
                {
                    // wait at least 3 secs from the last key press
                    mySaveTimer.Interval = CnSecondsToDelyLastKeyPress * 1000;
                }
                else
                {
                    DialogResult res = DialogResult.Yes;

                    if (!advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked)
                        res = QuerySave();

                    if (res == DialogResult.Yes)
                    {
                        SaveClicked();
                        return;
                    }
                }
            }

            mySaveTimer.Start();
        }

        private DialogResult QuerySave()
        {
            DialogResult res = LocalizableMessageBox.Show(Localizer.Str("Would you like to save changes?"),
                                               OseCaption,
                                               MessageBoxButtons.YesNoCancel);
            return res;
        }

        internal StoriesData TheCurrentStoriesSet
        {
            get
            {
                Debug.Assert((StoryProject != null) && !String.IsNullOrEmpty(CurrentStoriesSetName) && (StoryProject[CurrentStoriesSetName] != null));
                return StoryProject?[CurrentStoriesSetName];
            }
        }

        /// <summary>
        /// returns true if in either the Stories or Non-biblical stories sets
        /// (but not if in the old stories set)
        /// </summary>
        internal bool IsInStoriesSet
        {
            get { return (CurrentStoriesSetName != Resources.IDS_ObsoleteStoriesSet); }
        }

        internal ApplicationException CantEditOldStoriesEx
        {
            get { return new ApplicationException(Localizer.Str("The stories are not editable in the 'Old Stories' set")); }
        }

        // this is now browse for project in non-default location.
        private void browseForProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strProjectName = null;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // for this, we have to get the name to use for this project 
                    //  (which should be the filename without extension)
                    strProjectName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    // possible scenario. The user has copied a file/project from another machine
                    //  and has actually put it into the default location. In this case, we don't
                    //  want to query the user for the project (which has the side effect of
                    //  forcing the user to overwrite the file--based on the logic it needs
                    //  for other possible cases). So here, if the project file happens to be in
                    //  the default location, then just go ahead and open it directly and forget
                    //  about querying the user for the Project Name (i.e. don't do what's in this
                    //  if statement)
                    if (openFileDialog.FileName != ProjectSettings.GetDefaultProjectFilePath(strProjectName))
                    {
                        // this means that the file is not in the default location... But before we can go ahead, we need to 
                        //  check to see if a project already exists with this name in the default location on the disk.
                        // Here's the situation: the user has 'NewDataSet' in the default location and tries to 'browse/add'
                        //  a 'NewDataSet' from another location. In that case, it isn't strictly true that finding the one
                        //  in the default location means we will have to overwrite the existing project file (as threatened in 
                        //  the message box below). However, it is true, that the RecentProjects list will lose the reference to 
                        //  the existing one. So if the user cares anything about the existing one at all, they aren't going to 
                        //  want to do that... So let's be draconian and actually overwrite the file if they say 'yes'. This way, 
                        //  if they care, they'll say 'no' instead and give it a different name.
                        string strFilename = ProjectSettings.GetDefaultProjectFilePath(strProjectName);
                        if (File.Exists(strFilename))
                        {
                            DialogResult res = QueryOverwriteProject(strProjectName);
                            if (res != DialogResult.Yes)
                                throw StoryProjectData.BackOutWithNoUI;

                            NewProjectWizard.RemoveProject(strFilename, strProjectName);
                        }
                    }

                    var projSettings = new ProjectSettings(Path.GetDirectoryName(openFileDialog.FileName), strProjectName, true);
                    OpenProject(projSettings);
                }
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                // sub-routine has taken care of the UI, just exit without doing anything
            }
            catch (Program.RestartException)
            {
                Close();
            }
            catch (Exception ex)
            {
                string strErrorMsg = String.Format(Resources.IDS_UnableToOpenProjectFile,
                    Environment.NewLine, strProjectName,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""), ex.Message);
                LocalizableMessageBox.Show(strErrorMsg, OseCaption);
                return;
            }
        }

        public static DialogResult QueryOverwriteProject(string strProjectName)
        {
            return LocalizableMessageBox.Show(
                String.Format(
                    Localizer.Str(
                        "You already have a project with the name, '{0}'. Do you want to delete the existing one?"),
                    strProjectName), OseCaption,
                MessageBoxButtons.YesNoCancel);
        }

        private void projectFromTheInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var model = new GetCloneFromInternetModel(ProjectSettings.OneStoryProjectFolderRoot)
            {
                SelectedServerLabel = Resources.IDS_DefaultRepoServer
            };

            if (LoggedOnMember != null)
            {
                model.AccountName = LoggedOnMember.HgUsername;
                model.Password = LoggedOnMember.HgPassword;
            }

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.Cancel == dlg.ShowDialog())
                    return;

                string strProjectName = Path.GetFileNameWithoutExtension(dlg.PathToNewlyClonedFolder);

                // we can save this information so we can use it automatically during the next restart
                string strFullUrl = dlg.ThreadSafeUrl;
                var uri = new Uri(strFullUrl);
                string strUsername, strDummy, strBaseUrl = null;
                GetDetailsFromUri(uri, out strUsername, out strDummy, ref strBaseUrl);
                Program.SetHgParameters(dlg.PathToNewlyClonedFolder, strProjectName, strFullUrl, strUsername);
                var projSettings = new ProjectSettings(dlg.PathToNewlyClonedFolder, strProjectName, false)
                                       {
                                           HgRepoUrlHost = strBaseUrl
                                       };
                try
                {
                    OpenProject(projSettings);
                }
                catch (Program.RestartException)
                {
                    Close();
                }
                catch (Exception)
                {
                    string strErrorMsg = String.Format(Resources.IDS_NoProjectFromInternet,
                                                       Environment.NewLine, strFullUrl);
                    LocalizableMessageBox.Show(strErrorMsg, OseCaption);
                }
            }
        }

        internal static void GetDetailsFromUri(Uri uri, out string strUsername,
            out string strPassword, ref string strHgUrlBase)
        {
            strUsername = strPassword = null;
            if (!String.IsNullOrEmpty(uri.UserInfo) && (uri.UserInfo.IndexOf(':') != -1))
            {
                string[] astrUserInfo = uri.UserInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                strUsername = astrUserInfo[0];
                strPassword = astrUserInfo[1];
            }

            if (String.IsNullOrEmpty(strHgUrlBase))
                strHgUrlBase = uri.Scheme + "://" + uri.Host;
        }

        private void projectFromASharedNetworkDriveToolStripMenu_Click(object sender, EventArgs e)
        {
            // has to be a "same-named" project open currently that we're just going to push
            //  to the network drive
            Debug.Assert((StoryProject != null) && (StoryProject.ProjSettings != null));
#if !OldSharedNetworkApproach
            var dlg = new FolderBrowserDialog
                          {
                              Description =
                                  String.Format(
                                      Localizer.Str(
                                          "Browse to the shared network folder where the project will be stored (i.e. the path to parent folder in which the program will create a '{0}' folder in order to put the '{1}' sub-folder for this project)"),
                                      Resources.DefMyDocsSubfolder,
                                      StoryProject.ProjSettings.ProjectName)
                          };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string strSharedNetworkFolder = Path.Combine(dlg.SelectedPath,
                                                             Path.Combine(
                                                                 Resources.DefMyDocsSubfolder,
                                                                 StoryProject.ProjSettings.ProjectName));

                if (strSharedNetworkFolder.ToLower() == StoryProject.ProjSettings.ProjectFolder.ToLower())
                {
                    LocalizableMessageBox.Show(Localizer.Str("You chose the local project folder. You have to browse for a network drive on which to associate the current project"),
                                    OseCaption);
                    return;
                }

                if (!Directory.Exists(strSharedNetworkFolder))
                    Directory.CreateDirectory(strSharedNetworkFolder);

                Program.SetHgParametersNetworkDrive(StoryProject.ProjSettings.ProjectFolder,
                                                    StoryProject.ProjSettings.ProjectName,
                                                    strSharedNetworkFolder);
            }
#else
            openFileDialog.FileName = ProjectSettings.OneStoryFileName(StoryProject.ProjSettings.ProjectName);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // can't be the same as the current project!
                if (openFileDialog.FileName != StoryProject.ProjSettings.ProjectFilePath)
                {
                    if (Path.GetFileNameWithoutExtension(openFileDialog.FileName) == StoryProject.ProjSettings.ProjectName)
                    {
                        string strNetworkDriveFolder = Path.GetDirectoryName(openFileDialog.FileName);
                        Program.SetHgParametersNetworkDrive(StoryProject.ProjSettings.ProjectFolder,
                                                            StoryProject.ProjSettings.ProjectName,
                                                            strNetworkDriveFolder);
                    }
                    else
                    {
                        LocalizableMessageBox.Show(Properties.Resources.IDS_MustBeCloneRepo,
                                        StoryEditor.OseCaption);
                    }
                }
                else
                {
                    LocalizableMessageBox.Show(Properties.Resources.IDS_CantPushToTheLocalRepo,
                                    StoryEditor.OseCaption);
                }
            }
#endif
        }

        protected void CloseProjectFile()
        {
            StoryProject = null;
            LoggedOnMember = null;
            if (m_dlgNotes != null)
            {
                m_dlgNotes.Close();
                m_dlgNotes = null;
            }

            if (m_dlgHistDiffDlg != null)
            {
                m_dlgHistDiffDlg.Close();
                m_dlgHistDiffDlg = null;
            }

            if (m_dlgPrintForm != null)
            {
                m_dlgPrintForm.Close();
                m_dlgPrintForm = null;
            }

            ClearState();

            ReInitMenuVisibility();

            // restart the last sync timer whenever we switch projects
            tmLastSync = DateTime.Now;

            if (splitContainerUpDown.IsMinimized)
                splitContainerUpDown.Restore();

            _dateTimeLastSaved = DateTime.MinValue;
        }

        protected void ReInitMenuVisibility()
        {
            // some that might have been made invisible need to be given a fair chance for next time
            editCopyStoryMenu.Visible =
                editDeleteStoryLinesMenu.Visible =
                editDeleteNationalBtMenu.Visible =
                editCopyNationalBtMenu.Visible =
                editDeleteEnglishBtMenu.Visible =
                editCopyEnglishBtMenu.Visible =
                editDeleteFreeTranslationMenu.Visible =
                editCopyFreeTranslationMenu.Visible =
                editDeleteTestToolStripMenu.Visible = true;
        }

        protected void ClearState()
        {
            ClearFlowControls();

            HtmlStoryBtControl.LastTextareaInFocusId = null;
            CtrlTextBox._inTextBox = null;
            TheCurrentStory = null;
            // turning off in 2.4... StoryStageLogic.stateTransitions = null;
            comboBoxStorySelector.Items.Clear();
            ResetStorySelectorComboBox();
            textBoxStoryVerse.Text = Localizer.Str("Story");

            if (!viewUseSameSettingsForAllStoriesMenu.Checked)
            {
                viewConsultantNotesMenu.Checked =
                    viewCoachNotesMenu.Checked =
                    viewBibleMenu.Checked = false;
            }
        }

        private void ResetStorySelectorComboBox()
        {
            comboBoxStorySelector.Text = Localizer.Str("<type the name of a story to create and hit Enter>");
        }

        protected void NewProjectFile()
        {
            if (!SaveAndCloseProject())
                return;

            comboBoxStorySelector.Focus();

            // for a new project, we don't want to automatically log in (since this will be the first
            //  time editing the new project and we need to add at least the current user)
            LoggedOnMember = null;
            Debug.Assert(StoryProject == null);
            projectLoginToolStripMenuItem_Click(null, null);

            if ((StoryProject != null) && (StoryProject.ProjSettings != null))
            {
                UpdateRecentlyUsedListsEx(StoryProject.ProjSettings);
                UpdateUiMenusAfterProjectOpen();
            }
        }

        protected bool InitNewStoryProjectObject()
        {
            Debug.Assert(StoryProject == null);

            try
            {
                StoryProject = new StoryProjectData();    // null causes us to query for the project name
                Modified |= StoryProject.InitializeProjectSettings(LoggedOnMember);
                CheckForLogon(StoryProject);
                Debug.Assert(LoggedOnMember != null);

                if (Modified)
                {
                    SaveClicked();
                    Text = GetFrameTitle(true);
                }

                return true;
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                // sub-routine has taken care of the UI, just exit without doing anything
                // why??? StoryProject = null;
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(String.Format(Resources.IDS_UnableToOpenMemberList,
                    Environment.NewLine, ex.Message), OseCaption);
            }

            return false;
        }

        private void projectSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert((StoryProject != null) && (StoryProject.ProjSettings != null) && (LoggedOnMember != null));

            try
            {
                Modified = StoryProject.InitializeProjectSettings(LoggedOnMember);

                if (Modified)
                {
                    SaveClicked();

                    if (TheCurrentStory != null)
                    {
                        // reload the state transitions so that we can possible support a new
                        //  configuration (e.g. there might now be a FPM)
                        Debug.Assert(StoryProject.TeamMembers != null);
                        ReloadStateTransitionFile();
                        ReInitMenuVisibility();
                        SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, true);
                        InitAllPanes(); // just in case e.g. the font or RTL value changed
                    }
                }
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
            }
        }

        private void ReloadStateTransitionFile()
        {
            StoryStageLogic.stateTransitions =
                new StoryStageLogic.StateTransitions(((StoryProject != null) && (StoryProject.ProjSettings != null))
                                                         ? StoryProject.ProjSettings.ProjectFolder
                                                         : null);
        }

        private void projectLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (StoryProject == null)
                {
                    InitNewStoryProjectObject();
                }
                else
                {
                    // detect if the logged on member type changed, and if so, redo the Consult Notes panes
                    string strMemberName = null;
                    TeamMemberData.UserTypes eRole = TeamMemberData.UserTypes.Undefined;
                    bool bLoginRequired = true;
                    if (LoggedOnMember != null)
                    {
                        strMemberName = LoggedOnMember.Name;
                        eRole = LoggedOnMember.MemberType;
                        bLoginRequired = false;
                    }

                    LoggedOnMember = StoryProject.EditTeamMembers(strMemberName, eRole, true,
                        StoryProject.ProjSettings, bLoginRequired, ref Modified);

                    if (TheCurrentStory != null)
                    {
                        InitAllPanes(TheCurrentStory.Verses);
                        WarnIfNotProperMemberType();
                    }
                }
            }
            catch { }   // this might throw if the user cancels, but we don't care
        }

        private void UpdateRecentlyUsedListsEx(ProjectSettings projSettings)
        {
            UpdateRecentlyUsedLists(projSettings.ProjectFolder, projSettings.ProjectName);
        }

        public static void UpdateRecentlyUsedLists(string strProjectFolder, string strProjectName)
        {
            // if this is called from reflection, we need to initialize the Settings objects
            if (Settings.Default.RecentProjects == null)
                Program.InitializeLocalSettingsCollections(true);

            // update the recently-used-project-names list
            if (Settings.Default.RecentProjects.Contains(strProjectName))
            {
                int nIndex = Settings.Default.RecentProjects.IndexOf(strProjectName);
                Settings.Default.RecentProjects.RemoveAt(nIndex);
                Settings.Default.RecentProjectPaths.RemoveAt(nIndex);
            }

            Settings.Default.RecentProjects.Insert(0, strProjectName);
            Settings.Default.RecentProjectPaths.Insert(0, strProjectFolder);

            Settings.Default.LastProject = strProjectName;
            Settings.Default.LastProjectPath = strProjectFolder;
            Settings.Default.Save();
        }

        protected void OpenProject(string strProjectFolder, string strProjectName)
        {
            var projSettings = new ProjectSettings(strProjectFolder, strProjectName, true);

            // see if we can update from a repository first before opening.
            string strDotHgFolder = Path.Combine(projSettings.ProjectFolder, ".hg");
            if (IsInStoriesSet && Directory.Exists(strDotHgFolder))
            {
                string strUsername, strPassword;
                TeamMemberData.GetHgParameters(LoggedOnMember, out strUsername, out strPassword);

                // can't allow a sync if two instances are running with the same project
                Program.InsureSingleInstanceOfProgramName(strProjectName);

                // clean up any existing open projects
                if (!SaveAndCloseProject())
                    return;

                projSettings.SyncWithRepository(strUsername, strPassword);
                // Program.SyncWithRepository(projSettings.ProjectFolder, true);
            }

            OpenProject(projSettings);
        }

        private DateTime _dateTimeLastSaved;
        public void CheckForFileChanged(object sender, EventArgs e)
        {
            if ((StoryProject == null) || (StoryProject.ProjSettings == null))
                return;

            // if we just saved, then don't bother the user anymore
            var strProjectFilePath = StoryProject.ProjSettings.ProjectFilePath;
            if (!File.Exists(strProjectFilePath))
                return; // not written yet (during new project cycle)

            var dateTimeCurrent = File.GetLastWriteTime(strProjectFilePath);
            if (_dateTimeLastSaved == dateTimeCurrent)
                return;

            myReopenTimer.Stop();   // to prevent further calls

            _dateTimeLastSaved = DateTime.MinValue;

            // notify about reloading
            if (Modified)
                SaveXElementWithBadExtn(GetXml, strProjectFilePath);    // just in case someone complains

            var res = LocalizableMessageBox.Show(
                String.Format(Localizer.Str(
                    "The project file mentioned below was changed outside of {2} and must be reloaded.{0}{0}'{1}'{0}{0}You will not be able to save any further changes until you reopen the project. Click 'Yes' to reopen the project now."),
                              Environment.NewLine, strProjectFilePath, OseCaption),
                OseCaption, MessageBoxButtons.YesNoCancel);

            if (res != DialogResult.Yes)
                return;

            Modified = false;
            projectSendReceiveMenu.PerformClick();
        }

        protected void OpenProject(ProjectSettings projSettings)
        {
            // clean up any existing open projects
            if (!SaveAndCloseProject())
                return;

            // next, insure that the file for the project exists (do this outside the try,
            //  so the caller is informed of no file (so, for eg., it can remove from recently
            //  used list.
            projSettings.ThrowIfProjectFileDoesntExists();

            UpdateRecentlyUsedListsEx(projSettings);

            Program.InsureSingleInstanceOfProgramName(projSettings.ProjectName);

            try
            {
                // serialize in the file
                ProjectReader projFile;
                _dateTimeLastSaved = ProjectReader.ReadProjectFile(projSettings.ProjectFilePath, out projFile);
                myReopenTimer.Start();

                // get the data into another structure that we use internally (more flexible)
                StoryProject = GetOldStoryProjectData(projFile, projSettings);

                // this makes the most sense to choose the set name here (maybe based on the last one we edited for this project)
                var strStoriesSetName = Resources.IDS_MainStoriesSet;
                if (Program.MapProjectNameToLastStoriesSetWorkedOn.ContainsKey(projSettings.ProjectName))
                    strStoriesSetName = Program.MapProjectNameToLastStoriesSetWorkedOn[projSettings.ProjectName];
                if (!StoryProject.ContainsKey(strStoriesSetName))
                    strStoriesSetName = StoryProject.Values.First().SetName;

                InitializeCurrentStoriesSetName(strStoriesSetName);

                if (TheCurrentStoriesSet.Count > 0)
                    LoadComboBox();

                Debug.Assert(LoggedOnMember != null);

                // check for project settings that might have been saved from a previous session
                string strStoryToLoad;
                if (!Program.MapProjectNameToLastStoryWorkedOn.TryGetValue(projSettings.ProjectName, out strStoryToLoad))
                {
                    if (!String.IsNullOrEmpty(Settings.Default.LastStoryWorkedOn))
                        strStoryToLoad = Settings.Default.LastStoryWorkedOn;
                }

                if ((TheCurrentStoriesSet.Count > 0) && (String.IsNullOrEmpty(strStoryToLoad) || !comboBoxStorySelector.Items.Contains(strStoryToLoad)))
                    strStoryToLoad = TheCurrentStoriesSet[0].Name;    // default

                UpdateUiMenusAfterProjectOpen(); 

                // at least temporarily reset the 'Use for all stories' flag so that 
                //  we don't not reset the view for this new project settings (which
                //  calls SetViews during the SelectedItem handler below)
                bool bUseForAllStories = viewUseSameSettingsForAllStoriesMenu.Checked;
                viewUseSameSettingsForAllStoriesMenu.Checked = false;

                if (!String.IsNullOrEmpty(strStoryToLoad) && comboBoxStorySelector.Items.Contains(strStoryToLoad))
                    JumpToStory(strStoryToLoad);

                // reset the 'use for all stories' flag to whatever it used to be
                viewUseSameSettingsForAllStoriesMenu.Checked = bUseForAllStories;

                Text = GetFrameTitle(true);

#if OnGecko
                var dlg = new GeckoTestForm(this, TheCurrentStory);
                dlg.Show();
#endif
                // show the chorus notes at load time
                // InitProjectNotes(projSettings, LoggedOnMember.Name);
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                // sub-routine has taken care of the UI, just exit without doing anything
            }
            catch (StoryProjectData.Backout2ReOpenException ex)
            {
                SaveXElement(ex.XmlProjectFile, projSettings.ProjectFilePath, true);
                OpenProject(projSettings);
            }
            catch (DuplicateStoryStateTransitionException)
            {
                // pass this one on so it triggers an email
                throw;
            }
            catch (Exception ex)
            {
                string strErrorMsg = String.Format(Resources.IDS_UnableToOpenProjectFile,
                    Environment.NewLine, projSettings.ProjectName,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""), ex.Message);
                LocalizableMessageBox.Show(strErrorMsg, OseCaption);
            }
        }

        internal string GetFrameTitle(bool bAddLogin)
        {
            if ((StoryProject == null)
                || (StoryProject.ProjSettings == null)
                || (String.IsNullOrEmpty(StoryProject.ProjSettings.ProjectName)))
                return OseCaption;

            string strTitle = String.Format(Localizer.Str("OneStory Editor -- {0} Story Project") + Localizer.Str(" -- Viewing story set named {1}"),
                                            StoryProject.ProjSettings.ProjectName, CurrentStoriesSetName);

            if (bAddLogin && (LoggedOnMember != null))
                strTitle += String.Format(Localizer.Str(" -- ({0} logged in as '{1}')"),
                                          LoggedOnMember.Name,
                                          TeamMemberData.GetMemberTypeAsDisplayString(LoggedOnMember.MemberType));

            return strTitle;
        }

        private void InitProjectNotes(ProjectSettings projSettings, string strUsername)
        {
            if (m_dlgNotes != null)
            {
                m_dlgNotes.Close();
                m_dlgNotes = null;
            }
            m_dlgNotes = new NoteForm(projSettings, strUsername);
            m_dlgNotes.Show();
        }

        protected void LoadComboBox()
        {
            // populate the combo boxes with all the existing story names
            comboBoxStorySelector.Items.Clear();
            foreach (StoryData aStory in TheCurrentStoriesSet)
                comboBoxStorySelector.Items.Add(aStory.Name);
        }

        protected void CheckForLogon(StoryProjectData theStoryProject)
        {
            if (LoggedOnMember == null)
                LoggedOnMember = theStoryProject.GetLogin(ref Modified);
        }

        protected StoryProjectData GetOldStoryProjectData(NewDataSet projFile, ProjectSettings projSettings)
        {
            var theOldStoryProject = new StoryProjectData(projFile, projSettings);
            CheckForLogon(theOldStoryProject);
            return theOldStoryProject;
        }

        private void insertNewStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strStoryName = null;
            int nIndexOfCurrentStory = -1;
            if (AddNewStoryGetIndex(ref nIndexOfCurrentStory, ref strStoryName, true))
            {
                Debug.Assert(nIndexOfCurrentStory != -1);
                InsertNewStory(strStoryName, nIndexOfCurrentStory, null, null);
            }
        }

        protected bool CheckForProjFac()
        {
            Debug.Assert(LoggedOnMember != null);
            if ((LoggedOnMember == null)
                || !TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                          TeamMemberData.UserTypes.ProjectFacilitator))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "You have to be logged in as a 'Project Facilitator' to add stories. To login, click 'Project', 'Login'"),
                    OseCaption);
                return false;
            }
            return true;
        }

        protected bool AddNewStoryGetIndex(ref int nIndexForInsert, ref string strStoryName,
            bool bCheckThatProjFacIsLoggedIn)
        {
            Debug.Assert(LoggedOnMember != null);
            if (bCheckThatProjFacIsLoggedIn && !CheckForProjFac())
                return false;

            do
            {
                // ask the user for what story they want to add (i.e. the name)
                strStoryName =
                    LocalizableMessageBox.InputBox(Localizer.Str("Enter the name of the story to add"),
                                                   OseCaption, strStoryName);
                
                if (String.IsNullOrEmpty(strStoryName))
                    return false;

            } while (!FindIndexToInsertAt(strStoryName, out nIndexForInsert));

            return true;
        }

        private bool FindIndexToInsertAt(string strStoryName, out int nIndexForInsert)
        {
            if (TheCurrentStoriesSet.Count > 0)
            {
                if (TheCurrentStoriesSet.Any(s => s.Name == strStoryName))
                {
                    // can't be the same as the name of an existing story
                    LocalizableMessageBox.Show(
                        Localizer.Str("You already have a story by that name. Please choose another name"),
                        OseCaption);

                    nIndexForInsert = -1;
                    return false;
                }

                nIndexForInsert = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
                return true;
            }

            nIndexForInsert = 0;
            return true;
        }

        private void AddNewStoryAfterToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddNewStoryAfter(null, null, null);
        }

        internal StoryData AddNewStoryAfter(string strStoryName, string strFileToCopy, string strCrafterSuggestion)
        {
            int nIndexOfCurrentStory = -1;
            if (!AddNewStoryGetIndex(ref nIndexOfCurrentStory, ref strStoryName, true))
                return null;

            Debug.Assert(nIndexOfCurrentStory != -1);
            nIndexOfCurrentStory = Math.Min(nIndexOfCurrentStory + 1, TheCurrentStoriesSet.Count);
            return InsertNewStory(strStoryName, nIndexOfCurrentStory, strFileToCopy, strCrafterSuggestion);
        }

        TimeSpan tsFalseEnter = new TimeSpan(0, 0, 1);
        DateTime dtLastKey = DateTime.Now;
        private void comboBoxStorySelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // user just finished entering a story name to select (or add)
            {
                // ignore false double-Enters (from hitting enter in the dialog box for 'CheckForProjFac')
                if ((DateTime.Now - dtLastKey) < tsFalseEnter)
                    return;

                if (StoryProject == null)
                    if (!InitNewStoryProjectObject())
                    {
                        ResetStorySelectorComboBox();
                        return;
                    }

                CheckForLogon(StoryProject);

                if (!CheckForProjFac())
                {
                    ResetStorySelectorComboBox();
                    dtLastKey = DateTime.Now;
                    return;
                }

                int nInsertIndex = 0;
                StoryData theStory = null;
                string strStoryToLoad = comboBoxStorySelector.Text;
                for (int i = 0; i < TheCurrentStoriesSet.Count; i++)
                {
                    StoryData aStory = TheCurrentStoriesSet[i];
                    if ((TheCurrentStory != null) && (TheCurrentStory == aStory))
                        nInsertIndex = i + 1;
                    if (aStory.Name == strStoryToLoad)
                        theStory = aStory;
                }

                if (theStory == null)
                {
                    if (LocalizableMessageBox.Show(String.Format(Localizer.Str("Unable to find the story '{0}'. Would you like to add a new story with that name?"),
                                                      strStoryToLoad),
                                                      OseCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        Debug.Assert(!comboBoxStorySelector.Items.Contains(strStoryToLoad));
                        InsertNewStory(strStoryToLoad, nInsertIndex, null, null);
                    }
                    else
                        ResetStorySelectorComboBox();
                }
                else
                    JumpToStory(theStory.Name);
            }
        }

        protected StoryData InsertNewStory(string strStoryName, int nIndexToInsert, string strFileToCopy, string strCrafterSuggestion)
        {
            if (!CheckForSaveDirtyFile())
                return null;

            // query for the crafter
            var dlg = new MemberPicker(StoryProject, TeamMemberData.UserTypes.Crafter)
                          {
                Text = Localizer.Str("Choose the crafter that crafted this story"),
                InitialSelection = strCrafterSuggestion
                          };

            if ((dlg.ShowDialog() != DialogResult.OK) || (dlg.SelectedMember == null))
                return null;

            string strCrafterGuid = dlg.SelectedMember.MemberGuid;

            Debug.Assert(TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                               TeamMemberData.UserTypes.ProjectFacilitator));
            Debug.Assert(StoryProject.TeamMembers != null);
            bool bIsBiblicalStory = (CurrentStoriesSetName == Resources.IDS_MainStoriesSet);
            var theNewStory = new StoryData(strStoryName, strCrafterGuid,
                                            LoggedOnMember.MemberGuid,
                                            bIsBiblicalStory,
                                            StoryProject);

            InsertNewStoryAdjustComboBox(theNewStory, nIndexToInsert);

            // check for Dropbox copy (after setting the 'TheCurrentStory')
            TriggerDropboxCopyStory(StoryProject.ProjSettings.DropboxStory, strFileToCopy);
            theNewStory.JustAdded = true;

            SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeVernacular, false);
            return theNewStory;
        }

        protected void InsertNewStoryAdjustComboBox(StoryData theNewStory, int nIndexToInsert)
        {
            TheCurrentStory = theNewStory;
            comboBoxStorySelector.Items.Insert(nIndexToInsert, theNewStory.Name);
            TheCurrentStoriesSet.Insert(nIndexToInsert, TheCurrentStory);
            JumpToStory(theNewStory.Name);
            Modified = true;
            InitAllPanes();
        }

        public bool UsingHtmlForStoryBtPane
        {
            get { return !advancedUseOldStyleStoryBtPaneMenu.Checked; }
            set { advancedUseOldStyleStoryBtPaneMenu.Checked = !value; }
        }

        private bool _bCancellingChange = false;
        private void LoadStory(object sender, EventArgs e)
        {
            // do nothing if we're already on this story:
            if (_bCancellingChange
                && (TheCurrentStory != null)
                && (TheCurrentStory.Name == (string)comboBoxStorySelector.SelectedItem))
                return;

            // save the file before moving on.
            if (!CheckForSaveDirtyFile())
            {
                // if we're backing out, try to reset the combo box with the current story
                if ((TheCurrentStory != null) && (TheCurrentStory.Name != (string)comboBoxStorySelector.SelectedItem))
                {
                    _bCancellingChange = true;
                    comboBoxStorySelector.SelectedItem = TheCurrentStory.Name;
                    _bCancellingChange = false;
                }
                return;
            }

            if (!UsingHtmlForStoryBtPane)
            {
                // if this happens, it means we didn't save or cleanup the document
                Debug.Assert(!Modified
                             || (flowLayoutPanelVerses.Controls.Count != 0));
            }

            // we might could come thru here without having opened any file (e.g. after New)
            if (StoryProject == null)
                if (!InitNewStoryProjectObject())
                    return;

            // find the story they've chosen (this shouldn't be possible to fail)
            foreach (StoryData aStory in TheCurrentStoriesSet)
                if (aStory.Name == (string)comboBoxStorySelector.SelectedItem)
                {
                    TheCurrentStory = aStory;
                    break;
                }
            Debug.Assert(TheCurrentStory != null);

            if (IsInStoriesSet)
            {
                Debug.Assert(StoryProject != null, "StoryProject != null");
                if (!String.IsNullOrEmpty(StoryProject.ProjSettings.ProjectName))
                {
                    Program.MapProjectNameToLastStoryWorkedOn[StoryProject.ProjSettings.ProjectName] = TheCurrentStory.Name;
                    Settings.Default.ProjectNameToLastStoryWorkedOn = Program.DictionaryToArray(Program.MapProjectNameToLastStoryWorkedOn);
                }

                Settings.Default.LastStoryWorkedOn = TheCurrentStory.Name;
                Settings.Default.Save();

                // see if we have proper Member IDs on all connote conversation comments
                if (!TheCurrentStory.CheckForConNotesParticipants(StoryProject, ref Modified))
                {
                    // failed to indicate who the main commentors are, 
                    //  so we can't continue
                    TheCurrentStory = null;
                    return;
                }
            }

            // initialize the text box showing the storying they're editing
            textBoxStoryVerse.Text = Localizer.Str("Story") + ": " + TheCurrentStory.Name;

            // initialize the project stage details (which might hide certain views)
            //  (do this *after* initializing the whole thing, because if we save, we'll
            //  want to save even the hidden pieces)
            // BUT to avoid the multiple repaints, temporarily disable the painting
            SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, true);

            // forget things:
            HtmlStoryBtControl.LastTextareaInFocusId = null;
            CtrlTextBox._nLastVerse = -1;
            linkLabelVerseBT.Tag = 0;

            if (m_frmFind != null)
                // if the user switches stories, then we need to reindex the search
                m_frmFind.ResetSearchParameters();

            // finally, initialize the verse controls
            InitAllPanes();

            // get the focus off the combo box, so mouse scroll doesn't rip thru the stories!
            if (!UsingHtmlForStoryBtPane)
                flowLayoutPanelVerses.Focus();

            // see if we were in the process of swapping columns
            if (_lstToSwap != null)
            {
                SwapColumns();
                CheckForNextStoryColumnSwap();
            }
        }

        private bool _bNagOnce = true;
        private void WarnIfNotProperMemberType()
        {
            // inform the user that they won't be able to edit this if they aren't the proper member type
            Debug.Assert((TheCurrentStory != null) && (LoggedOnMember != null));
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ProjectFacilitator))
            {
                viewCoachNotesMenu.Checked = false;
            }

            if (_bNagOnce)
                LoggedOnMember.CheckIfThisAnAcceptableEditorForThisStory(TheCurrentStory);

            _bNagOnce = false;
        }

        protected void InitAllPanes(VersesData theVerses)
        {
            // the first verse (for global ConNotes) should have been initialized by now
            Debug.Assert(theVerses.FirstVerse != null);

            int nLastVerseInFocus = CtrlTextBox._nLastVerse;
            StringTransfer stLast = (CtrlTextBox._inTextBox != null)
                ? CtrlTextBox._inTextBox.MyStringTransfer : null;
            int nVerseIndex = 0;

            ClearFlowControls();
            if (theVerses.Count == 0)
                TheCurrentStory.Verses.InsertVerse(0, null, null, null, null);

            // must initialize the transliterators before calling CurrentViewSettings
            InitializeTransliterators();

            if (UsingHtmlForStoryBtPane)
            {
                htmlStoryBtControl.TheSE = this;
                htmlStoryBtControl.StoryData = TheCurrentStory;
                htmlStoryBtControl.SetLineNumberLink = (text, index) =>
                                                        {
                                                            linkLabelVerseBT.Text = text;
                                                            linkLabelVerseBT.Tag = index;
                                                        };
                htmlStoryBtControl.ViewSettings = CurrentViewSettings;
                buttonMoveToNextLine.Visible = buttonMoveToPrevLine.Visible = true;
            }
            else
            {
                flowLayoutPanelVerses.SuspendLayout();
                flowLayoutPanelVerses.LineNumberLink = linkLabelVerseBT;
            }

#if UsingHtmlDisplayForConNotes
            linkLabelVerseBT.Visible = true;
            
            if (Localizer.Default.LocLanguage.Font != null)
                StoryProject.ProjSettings.Localization.FontToUse = Localizer.Default.LocLanguage.Font;

            htmlConsultantNotesControl.TheSE = this;
            htmlConsultantNotesControl.StoryData = TheCurrentStory;
            htmlConsultantNotesControl.SetLineNumberLink = (text, index) =>
                                                               {
                                                                   linkLabelConsultantNotes.Text = text;
                                                                   linkLabelConsultantNotes.Tag = index;
                                                               };
            htmlConsultantNotesControl.MakeLineNumberLinkVisible = () => { linkLabelConsultantNotes.Visible = true; };

            htmlCoachNotesControl.TheSE = this;
            htmlCoachNotesControl.StoryData = TheCurrentStory;
            htmlCoachNotesControl.SetLineNumberLink = (text, index) =>
                                                               {
                                                                   linkLabelCoachNotes.Text = text;
                                                                   linkLabelCoachNotes.Tag = index;
                                                               };
            htmlCoachNotesControl.MakeLineNumberLinkVisible = () => { linkLabelCoachNotes.Visible = true; };
#else
            flowLayoutPanelConsultantNotes.SuspendLayout();
            flowLayoutPanelCoachNotes.SuspendLayout();
#endif
            SuspendLayout();

#if UsingHtmlDisplayForConNotes
#else
            // for ConNotes, there's a zeroth verse that's for global story comments
            InitConsultNotesPane(flowLayoutPanelConsultantNotes, theVerses.FirstVerse.ConsultantNotes, nVerseIndex);
            InitConsultNotesPane(flowLayoutPanelCoachNotes, theVerses.FirstVerse.CoachNotes, nVerseIndex);
#endif
            if (!UsingHtmlForStoryBtPane)
            {
                // either add the general testing question line (or a button)
                if (viewGeneralTestingsQuestionMenu.Checked)
                    InitVerseControls(theVerses.FirstVerse, nVerseIndex++);
                else
                    AddDropTargetToFlowLayout(nVerseIndex++);

                foreach (VerseData aVerse in theVerses)
                {
                    if (aVerse.IsVisible || viewHiddenVersesMenu.Checked)
                    {
                        InitVerseControls(aVerse, nVerseIndex);

                    }

                    // skip numbers, though, if we have hidden verses so that the verse nums
                    //  will be the same (in case we have references to lines in the connotes)
                    //  AND so it'll be a clue to the user that there are hidden verses present.
                    nVerseIndex++;
                }

                flowLayoutPanelVerses.ResumeLayout(true);
            }
            else
            {
                htmlStoryBtControl.LoadDocument();
            }

#if UsingHtmlDisplayForConNotes
            // ConNotes are not done in one swell-foop via an Html control
            htmlConsultantNotesControl.LoadDocument();
            htmlCoachNotesControl.LoadDocument();
#else
            flowLayoutPanelConsultantNotes.ResumeLayout(true);
            flowLayoutPanelCoachNotes.ResumeLayout(true);
#endif
            ResumeLayout(true);

            if (UsingHtmlForStoryBtPane)
            {
                /* handled in another way -- see StrIdToScrollTo = GetTopRowId; prior to calling this method
                if (String.IsNullOrEmpty(HtmlStoryBtControl.LastTextareaInFocusId) && (theVerses.Count > 0))
                    FocusOnVerse(1, false, false);
                else
                    htmlStoryBtControl.ScrollToElement(HtmlStoryBtControl.LastTextareaInFocusId, false);
                */
            }
            else
            {
                if ((nLastVerseInFocus == -1) && (theVerses.Count > 0))
                {
                    FocusOnVerse(1, false, false);
                    nLastVerseInFocus = 0;
                }
                else
                    FocusOnVerse(nLastVerseInFocus, true, true);

                if ((stLast != null) && (stLast.TextBox != null))
                    stLast.TextBox.Focus();
            }
        }

        private void InitializeTransliterators()
        {
            if (UsingHtmlForStoryBtPane)
            {
                HtmlStoryBtControl.TransliteratorVernacular = viewTransliterationVernacular.Checked
                                                                  ? LoggedOnMember.TransliteratorVernacular
                                                                  : null;

                HtmlStoryBtControl.TransliteratorNationalBt = viewTransliterationNationalBT.Checked
                                                                  ? LoggedOnMember.TransliteratorNationalBt
                                                                  : null;

                HtmlStoryBtControl.TransliteratorInternationalBt = viewTransliterationInternationalBt.Checked
                                                                       ? LoggedOnMember.TransliteratorInternationalBt
                                                                       : null;

                HtmlStoryBtControl.TransliteratorFreeTranslation = viewTransliterationFreeTranslation.Checked
                                                                       ? LoggedOnMember.TransliteratorFreeTranslation
                                                                       : null;
            }
            else
            {
                VerseBtControl.TransliteratorVernacular = viewTransliterationVernacular.Checked
                                                              ? LoggedOnMember.TransliteratorVernacular
                                                              : null;

                VerseBtControl.TransliteratorNationalBt = viewTransliterationNationalBT.Checked
                                                              ? LoggedOnMember.TransliteratorNationalBt
                                                              : null;

                VerseBtControl.TransliteratorInternationalBt = viewTransliterationInternationalBt.Checked
                                                                   ? LoggedOnMember.TransliteratorInternationalBt
                                                                   : null;

                VerseBtControl.TransliteratorFreeTranslation = viewTransliterationFreeTranslation.Checked
                                                                   ? LoggedOnMember.TransliteratorFreeTranslation
                                                                   : null;
            }
        }

        protected void InitVerseControls(VerseData aVerse, int nVerseIndex)
        {
            var aVerseCtrl = CreateVerseBtControl(flowLayoutPanelVerses, aVerse, nVerseIndex);
            flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
            AddDropTargetToFlowLayout(nVerseIndex);
        }

        public Control CreateVerseBtControl(VerseBtLineFlowLayoutPanel verseBtLineFlowLayoutPanel, VerseData aVerseData, int nVerseIndex)
        {
            var aVerseCtrl = new VerseBtControl(this, verseBtLineFlowLayoutPanel, aVerseData, nVerseIndex);
            if (!aVerseData.IsVisible)
                aVerseCtrl.BackColor = Color.Khaki;

            aVerseCtrl.UpdateHeight(Panel1_Width);
            return aVerseCtrl;
        }

        // this is for use by the consultant panes if we add or remove or hide a note
        internal void ReInitVerseControls()
        {
            if (UsingHtmlForStoryBtPane)
            {
                htmlStoryBtControl.ViewSettings = CurrentViewSettings;
                htmlStoryBtControl.LoadDocument();
                if (!String.IsNullOrEmpty(HtmlStoryBtControl.LastTextareaInFocusId))
                    htmlStoryBtControl.ScrollToElement(HtmlStoryBtControl.LastTextareaInFocusId, false);
            }
            else
            {
                // this sometimes gets called in bad times
                if ((TheCurrentStory == null) || (TheCurrentStory.Verses.Count == 0))
                    return;

                int nLastVerseInFocus = CtrlTextBox._nLastVerse;
                StringTransfer stLast = (CtrlTextBox._inTextBox != null)
                    ? CtrlTextBox._inTextBox.MyStringTransfer : null;

                // get a new index
                int nVerseIndex = 0;
                flowLayoutPanelVerses.Controls.Clear();
                flowLayoutPanelVerses.SuspendLayout();
                SuspendLayout();

                InitializeTransliterators();

                // add either the general testing question line or a button
                if (viewGeneralTestingsQuestionMenu.Checked)
                    InitVerseControls(TheCurrentStory.Verses.FirstVerse, nVerseIndex++);
                else
                    AddDropTargetToFlowLayout(nVerseIndex++);

                foreach (VerseData aVerse in TheCurrentStory.Verses)
                {
                    if (aVerse.IsVisible || viewHiddenVersesMenu.Checked)
                        InitVerseControls(aVerse, nVerseIndex);

                    // skip numbers, though, if we have hidden verses so that the verse nums
                    //  will be the same (in case we have references to lines in the connotes)
                    //  AND so it'll be a clue to the user that there are hidden verses present.
                    nVerseIndex++;
                }
                flowLayoutPanelVerses.ResumeLayout(true);
                ResumeLayout(true);

                FocusOnVerse(nLastVerseInFocus, true, true);
                if ((stLast != null) && (stLast.TextBox != null))
                    stLast.TextBox.Focus();
            }
        }

#if UsingHtmlDisplayForConNotes
#else
        protected void InitConsultNotesPane(ConNoteFlowLayoutPanel theFLP, ConsultNotesDataConverter aCNsDC, int nVerseIndex)
        {
            ConsultNotesControl aConsultNotesCtrl = new ConsultNotesControl(this, theFLP,
                theCurrentStory.ProjStage, aCNsDC, nVerseIndex, LoggedOnMember.MemberType);
            aConsultNotesCtrl.UpdateHeight(Panel2_Width);
            theFLP.AddCtrl(aConsultNotesCtrl);
        }

        // this is for use by the consultant panes if we add or remove or hide a single note
        internal void ReInitConsultNotesPane(ConsultNotesDataConverter aCNsD)
        {
            int nLastVerseInFocus = CtrlTextBox._nLastVerse;
            StringTransfer stLast = (CtrlTextBox._inTextBox != null)
                ? CtrlTextBox._inTextBox.MyStringTransfer : null;

            int nVerseIndex = 0;
            if (flowLayoutPanelConsultantNotes.Contains(aCNsD))
            {
                flowLayoutPanelConsultantNotes.Clear();
                flowLayoutPanelConsultantNotes.SuspendLayout();
                SuspendLayout();

                // display the zeroth note (which is only for ConNotes
                InitConsultNotesPane(flowLayoutPanelConsultantNotes,
                    theCurrentStory.Verses.FirstVerse.ConsultantNotes, nVerseIndex);

                foreach (VerseData aVerse in theCurrentStory.Verses)
                {
                    // skip numbers, though, if we have hidden verses so that the verse nums
                    //  will be the same (in case we have references to lines in the connotes)
                    //  AND so it'll be a clue to the user that there are hidden verses present.
                    ++nVerseIndex;

                    if (aVerse.IsVisible || hiddenVersesToolStripMenuItem.Checked)
                        InitConsultNotesPane(flowLayoutPanelConsultantNotes,
                                             aVerse.ConsultantNotes, nVerseIndex);
                }

                flowLayoutPanelConsultantNotes.ResumeLayout(true);
                ResumeLayout(true);
            }
            else
            {
                Debug.Assert(flowLayoutPanelCoachNotes.Contains(aCNsD));
                flowLayoutPanelCoachNotes.Clear();
                flowLayoutPanelCoachNotes.SuspendLayout();
                SuspendLayout();

                // display the zeroth note (which is only for ConNotes
                InitConsultNotesPane(flowLayoutPanelCoachNotes, 
                    theCurrentStory.Verses.FirstVerse.CoachNotes, nVerseIndex);

                foreach (VerseData aVerse in theCurrentStory.Verses)
                {
                    // skip numbers, though, if we have hidden verses so that the verse nums
                    //  will be the same (in case we have references to lines in the connotes)
                    //  AND so it'll be a clue to the user that there are hidden verses present.
                    ++nVerseIndex;

                    if (aVerse.IsVisible || hiddenVersesToolStripMenuItem.Checked)
                        InitConsultNotesPane(flowLayoutPanelCoachNotes,
                                             aVerse.CoachNotes, nVerseIndex);
                }

                flowLayoutPanelCoachNotes.ResumeLayout(true);
                ResumeLayout(true);
            }

            FocusOnVerse(nLastVerseInFocus);
            if ((stLast != null) && (stLast.TextBox != null))
                stLast.TextBox.Focus();

            // if we do this, it's because something changed
            Modified = true;
        }

        internal void HandleQueryContinueDrag(ConsultNotesControl aCNsDC, QueryContinueDragEventArgs e)
        {
            Debug.Assert(flowLayoutPanelConsultantNotes.Contains(aCNsDC._theCNsDC)
                || flowLayoutPanelCoachNotes.Contains(aCNsDC._theCNsDC));
            FlowLayoutPanel theFLP = (flowLayoutPanelConsultantNotes.Contains(aCNsDC._theCNsDC)) ? flowLayoutPanelConsultantNotes : flowLayoutPanelCoachNotes;

            // this code causes the vertical scroll bar to move if the user is dragging the mouse beyond
            //  the boundary of the flowLayout panel that these verse controls are sitting it.
            Point pt = theFLP.PointToClient(MousePosition);
            if (theFLP.Bounds.Height < (pt.Y + 10))    // close to the bottom edge...
                theFLP.VerticalScroll.Value += 10;     // bump the scroll bar down
            else if ((pt.Y < 10) && theFLP.VerticalScroll.Value > 0)   // close to the top edge, while the scroll bar position is non-zero
                theFLP.VerticalScroll.Value -= Math.Min(10, theFLP.VerticalScroll.Value);

            if (e.Action != DragAction.Continue)
                DimConsultNotesDropTargetButtons(theFLP, aCNsDC);
            else
                LightUpConsultNotesDropTargetButtons(theFLP, aCNsDC);
        }

        private static void LightUpConsultNotesDropTargetButtons(FlowLayoutPanel theFLP, ConsultNotesControl control)
        {
            foreach (Control ctrl in theFLP.Controls)
            {
                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl aCNsC = (ConsultNotesControl)ctrl;
                if (aCNsC != control)
                    aCNsC.buttonDragDropHandle.Dock = DockStyle.Fill;
            }
        }

        private static void DimConsultNotesDropTargetButtons(FlowLayoutPanel theFLP, ConsultNotesControl control)
        {
            foreach (Control ctrl in theFLP.Controls)
            {
                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl aCNsC = (ConsultNotesControl)ctrl;
                if (aCNsC != control)
                    aCNsC.buttonDragDropHandle.Dock = DockStyle.Right;
            }
        }
#endif

        internal void AddNewVerse(int nInsertionIndex, int nNumberToAdd, bool bAfter)
        {
            if (bAfter)
                nInsertionIndex++;

            var lstNewVerses = new VersesData();
            for (int i = 0; i < nNumberToAdd; i++)
            {
                var verse = new VerseData();

                // add boxes for retellings just in case
                foreach (var testInfo in TheCurrentStory.CraftingInfo.TestersToCommentsRetellings)
                    verse.Retellings.TryAddNewLine(testInfo.MemberId);

                lstNewVerses.Add(verse);
            }

            TheCurrentStory.Verses.InsertRange(nInsertionIndex, lstNewVerses);
            InitAllPanes();
            Debug.Assert(lstNewVerses.Count > 0);
            // shouldn't need to do this here (done in InitAllPanes): FocusOnVerse(nInsertionIndex);

            Modified = true;
        }

        public void SetFocusTimer(int nLastVerse)
        {
            myFocusTimer.Stop();
            myFocusTimer.Tag = nLastVerse;
            myFocusTimer.Start();
        }

        public void TimeToSetFocus(object sender, EventArgs e)
        {
            myFocusTimer.Stop();
            var nVerseIndex = (int)myFocusTimer.Tag;
            FocusOnVerse(nVerseIndex, true, true);
        }

        /// <summary>
        /// Scroll the controls in the flow layout controls to make sure nVerseIndex line is
        /// visible.
        /// </summary>
        /// <param name="nVerseIndex">corresponds to the line number (e.g. ln: 1 == 1), but could be 0 for ConNote panes</param>
        /// <param name="bDontSyncConsultantNotePane">Don't sync the Consultant Note pane</param>
        /// <param name="bDontSyncCoachNotePane">Don't sync the Coach Note pane</param>
        public void FocusOnVerse(int nVerseIndex, bool bSyncConsultantNotePane,
            bool bSyncCoachNotePane)
        {
            // if no box was actually ever selected, then this might be -1
            if (nVerseIndex < 0)
                return;

            // light up whichever text box is visible
            // from the verses pane... (for verse controls, this is the line number, which
            //  is one more than the index we're looking for. (if this is from the zeroth
            //  line of the ConNotes, then just skip it)
            if (nVerseIndex >= 0)
            {
                if (UsingHtmlForStoryBtPane)
                {
                    htmlStoryBtControl.ScrollToVerse(nVerseIndex);
                }
                else
                {
                    Control ctrl = flowLayoutPanelVerses.GetControlAtVerseIndex(nVerseIndex);
                    if (ctrl == null)
                        return;

                    Debug.Assert(ctrl is VerseBtControl);
                    VerseBtControl theVerse = ctrl as VerseBtControl;

                    // then scroll it into view (but not if this is the one that initiated
                    //  the scrolling since it's annoying that it jumps around when greater
                    //  than the height of the view).
                    if ((CtrlTextBox._inTextBox == null) || (CtrlTextBox._inTextBox._ctrlVerseParent != theVerse))
                        flowLayoutPanelVerses.ScrollIntoView(theVerse, false);
                    else
                        flowLayoutPanelVerses.LastControlIntoView = theVerse;
                }
            }

            // the ConNote controls have a zeroth line, so the index is one greater
            if (viewConsultantNotesMenu.Checked && bSyncConsultantNotePane)
            {
#if UsingHtmlDisplayForConNotes
                htmlConsultantNotesControl.ScrollToVerse(nVerseIndex);
#else
                Control ctrl = flowLayoutPanelConsultantNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return; 
                
                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl theConsultantNotes = ctrl as ConsultNotesControl;
                if ((CtrlTextBox._inTextBox == null) || (CtrlTextBox._inTextBox._ctrlVerseParent != theConsultantNotes))
                    flowLayoutPanelConsultantNotes.ScrollControlIntoView(theConsultantNotes);
#endif
            }

            if (viewCoachNotesMenu.Checked && bSyncCoachNotePane)
            {
#if UsingHtmlDisplayForConNotes
                htmlCoachNotesControl.ScrollToVerse(nVerseIndex);
#else
                Control ctrl = flowLayoutPanelCoachNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return; 
                
                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl theCoachNotes = ctrl as ConsultNotesControl;
                if ((CtrlTextBox._inTextBox == null) || (CtrlTextBox._inTextBox._ctrlVerseParent != theCoachNotes))
                    flowLayoutPanelCoachNotes.ScrollControlIntoView(theCoachNotes);
#endif
            }
        }

        public void AddNoteAbout(VerseControl ctrlParent, bool bNoteToSelf)
        {
            Debug.Assert(LoggedOnMember != null);
            string strNote = null; 
            if (ctrlParent is VerseBtControl)
            {
                var ctrl = ctrlParent as VerseBtControl;
                // if the control that was right-clicked on that led us here was one
                //  of the story lines, then take the selected portion of all story line
                //  controls and add it.
                if (IsInStoryLine(ctrl))
                {
                    // get selected text from all visible Story line controls
                    if (viewVernacularLangMenu.Checked)
                    {
                        string str = ctrl._verseData.StoryLine.Vernacular.TextBox.SelectedText.Trim();
                        if (!String.IsNullOrEmpty(str))
                            strNote += String.Format(" /{0}/", str);
                    }
                    if (viewNationalLangMenu.Checked)
                    {
                        string str = ctrl._verseData.StoryLine.NationalBt.TextBox.SelectedText.Trim();
                        if (!String.IsNullOrEmpty(str))
                            strNote += String.Format(" /{0}/", str);
                    }
                    if (viewEnglishBtMenu.Checked)
                    {
                        string str = ctrl._verseData.StoryLine.InternationalBt.TextBox.SelectedText.Trim();
                        if (!String.IsNullOrEmpty(str))
                            strNote += String.Format(" '{0}'", str);
                    }
                    if (viewFreeTranslationMenu.Checked)
                    {
                        string str = ctrl._verseData.StoryLine.FreeTranslation.TextBox.SelectedText.Trim();
                        if (!String.IsNullOrEmpty(str))
                            strNote += String.Format(" '{0}'", str);
                    }
                }
                else if (CtrlTextBox._inTextBox != null)
                {
                    // otherwise, it might have been a retelling or some other control
                    if (!String.IsNullOrEmpty(CtrlTextBox._inTextBox._strLabel))
                    {
                        try
                        {
                            AddExtraInfoBasedOnLabel(CtrlTextBox._inTextBox._strLabel,
                                                     ctrl, ref strNote);
                        }
                        catch (Exception ex)
                        {
                            var exApp = new ApplicationException(String.Format("Error while trying to add ConNote on: '{0}' in verse#: {1} with selected text: '{2}' (full text: '{3}')",
                                CtrlTextBox._inTextBox._strLabel,
                                ctrlParent.VerseNumber,
                                CtrlTextBox._inTextBox.SelectedText,
                                CtrlTextBox._inTextBox.Text), ex);
                            Program.ShowException(exApp);
                            return;
                        }
                    }
                    else
                    {
                        string str = CtrlTextBox._inTextBox.SelectedText.Trim();
                        if (!String.IsNullOrEmpty(str))
                            strNote += String.Format(" /{0}/", str);
                    }
                }
            }
            else if (CtrlTextBox._inTextBox != null)
            // otherwise, just get the selected text out of the one box that was 
            //  right-clicked in.
            {
                if (viewCoachNotesMenu.Checked)
                {
                    if (!String.IsNullOrEmpty(CtrlTextBox._inTextBox._strLabel))
                        strNote += CtrlTextBox._inTextBox._strLabel;

                    string str = CtrlTextBox._inTextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
            }
            strNote += ". ";

            SendNoteToCorrectPane(ctrlParent.VerseNumber, strNote, bNoteToSelf);
        }

        /*
        public static string StrRegarding
        {
            get { return Localizer.Str(": Re: "); }
        }
        */

        public static string DateForConNote
        {
            get { return String.Format(" ({0}): ", DateTime.Now.ToString("dd-MMM-yyyy")); }
        }

        private bool IsInStoryLine(VerseBtControl ctrl)
        {
            return (CtrlTextBox._inTextBox == ctrl._verseData.StoryLine.Vernacular.TextBox)
                   || (CtrlTextBox._inTextBox == ctrl._verseData.StoryLine.NationalBt.TextBox)
                   || (CtrlTextBox._inTextBox == ctrl._verseData.StoryLine.InternationalBt.TextBox)
                   || (CtrlTextBox._inTextBox == ctrl._verseData.StoryLine.FreeTranslation.TextBox);
        }

        public static bool IsFirstCharsEqual(string strLhs, string strRhs, int nNumChars)
        {
            return (!String.IsNullOrEmpty(strLhs) &&
                    (strLhs.Length >= nNumChars) &&
                    (strRhs.Length >= nNumChars) &&
                    (strLhs.Substring(0, nNumChars)
                     == strRhs.Substring(0, nNumChars)));
        }

        private static bool IsFirstCharsEqual(string strLhs, string strRhs)
        {
            int nCompareLen = 4;
            if (strRhs.Contains(' '))
                nCompareLen = strRhs.IndexOf(' ') + 1;

            return IsFirstCharsEqual(strLhs, strRhs, nCompareLen);
        }

        internal static bool IsTestQuestionBox(string strLabel)
        {
            return IsFirstCharsEqual(strLabel, TestQuestionData.TestQuestionsLabelFormat);
        }

        internal static bool IsRetellingBox(string strLabel)
        {
            return IsFirstCharsEqual(strLabel, RetellingsData.RetellingLabelFormat);
        }

        internal static bool IsTqAnswerBox(string strLabel)
        {
            return IsFirstCharsEqual(strLabel, AnswersData.AnswersLabelFormat);
        }

        private void AddExtraInfoBasedOnLabel(string strLabel, VerseBtControl ctrl,
            ref string strNote)
        {
            if (IsTestQuestionBox(strLabel))
            {
                TestQuestionData testQuestionData = GetTestQuestionData(strLabel, ctrl);

                strNote += strLabel;
                // get selected text from all visible Story line controls
                if (StoryProject.ProjSettings.ShowTestQuestions.Vernacular && viewVernacularLangMenu.Checked)
                {
                    string str = testQuestionData.TestQuestionLine.Vernacular.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowTestQuestions.NationalBt && viewNationalLangMenu.Checked)
                {
                    string str = testQuestionData.TestQuestionLine.NationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowTestQuestions.InternationalBt && viewEnglishBtMenu.Checked)
                {
                    string str = testQuestionData.TestQuestionLine.InternationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" '{0}'", str);
                }
            }
            else if (IsRetellingBox(strLabel))
            {
                LineMemberData retellingData = GetRetellingData(strLabel, ctrl);

                strNote += strLabel;
                // get selected text from all visible Story line controls
                if (StoryProject.ProjSettings.ShowRetellings.Vernacular && viewVernacularLangMenu.Checked)
                {
                    string str = retellingData.Vernacular.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowRetellings.NationalBt && viewNationalLangMenu.Checked)
                {
                    string str = retellingData.NationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowRetellings.InternationalBt && viewEnglishBtMenu.Checked)
                {
                    string str = retellingData.InternationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" '{0}'", str);
                }
            }
            else if (IsTqAnswerBox(strLabel))
            {
                // e.g. "ans 1:tst 1:"
                AnswersData answers;
                var answerData = GetTqAnswerData(strLabel, ctrl, out answers);

                strNote += strLabel;
                // get selected text from all visible Story line controls
                if (StoryProject.ProjSettings.ShowAnswers.Vernacular && viewVernacularLangMenu.Checked)
                {
                    string str = answerData.Vernacular.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowAnswers.NationalBt && viewNationalLangMenu.Checked)
                {
                    string str = answerData.NationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" /{0}/", str);
                }
                if (StoryProject.ProjSettings.ShowAnswers.InternationalBt && viewEnglishBtMenu.Checked)
                {
                    string str = answerData.InternationalBt.TextBox.SelectedText.Trim();
                    if (!String.IsNullOrEmpty(str))
                        strNote += String.Format(" '{0}'", str);
                }
            }
            else
                strNote += CtrlTextBox._inTextBox._strLabel;
        }

        private LineMemberData GetRetellingData(string strLabel, VerseBtControl ctrl)
        {
            RetellingsData retellings = ctrl._verseData.Retellings;

            // e.g. 'ret 1:'
            Debug.Assert(strLabel.Contains(' '));
            int nIndex = strLabel.LastIndexOf(' ');
            string strTestNumber = strLabel.Substring(nIndex + 1, 1);

            // there are two cases we have to treat specially:
            //  1) it's 'ret 0' (because somehow the member id was removed)
            //  2) there's no 'ret 1' (in which case 'ret 2' is the zeroth element)

            // there are two cases we have to treat specially:
            //  1) it's 'ans 0' (because somehow the member id was removed)
            //  2) there's no 'ans 1' (in which case 'ans 2' is the zeroth element)
            string strTesterId = GetTesterId(strTestNumber,
                                             TheCurrentStory.CraftingInfo.TestersToCommentsRetellings,
                                             retellings);
            return retellings.TryGetValue(strTesterId);
        }

        internal LineMemberData GetTqAnswerData(string strLabel, VerseBtControl ctrl,
            out AnswersData answers)
        {
            // e.g. "ans 1:tst 1:"
            TestQuestionData testQuestionData = GetTestQuestionDataFromAnswerLabel(strLabel, ctrl);
            answers = testQuestionData.Answers;
            Debug.Assert(strLabel.Contains(' '));

            // at one time, I had this as "LastIndexOf", but then it found the
            //  TQ # rather than the test (i.e. ans) number, so it needs to be
            //  IndexOf... but there's an issue with localization. If the person
            //  uses multiple words for 'ans', then it'd be the last of those 
            //  spaces...
            int nIndex = strLabel.IndexOf(' ');
            string strTestNumber = strLabel.Substring(nIndex + 1, 1);

            return GetTqAnswerData(answers, strTestNumber);
            }

        public LineMemberData GetTqAnswerData(AnswersData answers, string strTestNumber)
        {
            // there are two cases we have to treat specially:
            //  1) it's 'ans 0' (because somehow the member id was removed)
            //  2) there's no 'ans 1' (in which case 'ans 2' is the zeroth element)
            string strTesterId = GetTesterId(strTestNumber,
                                             TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers,
                                             answers);
            return answers.TryGetValue(strTesterId);
        }

        private static string GetTesterId(string strTestNumber, TestInfo testInfo,
            IEnumerable<LineMemberData> answersData)
        {
            int nTestNumber = Convert.ToInt32(strTestNumber) - 1;
            string strMemberId = null;
            if (nTestNumber < 0)
            {
                // means it was "ans 0", so figure out which one it was (it'll be the
                //  one that *isn't* in the testInfo structure)
                foreach (var answer in answersData.Where(answer =>
                    !testInfo.Contains(answer.MemberId)))
                {
                    strMemberId = answer.MemberId;
                    break;
                }
            }
            else
            {
                var tester = testInfo[nTestNumber];
                strMemberId = tester.MemberId;
            }
            return strMemberId;
        }

        internal static TestQuestionData GetTestQuestionData(string strLabel, VerseBtControl ctrl)
        {
            // e.g. "tst 1:" (but may be localized)
            Debug.Assert(strLabel.Contains(' '));
            var len = 1;
            int nIndexSc, nIndex = strLabel.LastIndexOf(' ');
            if ((nIndexSc = strLabel.LastIndexOf(':')) != -1)
                len = nIndexSc - nIndex - 1;
            string strTestNumber = strLabel.Substring(nIndex + 1, len);
            int nTestNumber = Convert.ToInt32(strTestNumber) - 1;
            return ctrl._verseData.TestQuestions[nTestNumber];
        }

        internal static TestQuestionData GetTestQuestionDataFromAnswerLabel(string strLabel, VerseBtControl ctrl)
        {
            // e.g. "ans 1:tst 1:"
            // see if we can get just the 2nd portion
            int nIndex = strLabel.IndexOf(':');
            var strTstPortion = strLabel.Substring(nIndex + 1);
            return GetTestQuestionData(strTstPortion, ctrl);
        }

        internal static string GetInitials(string name)
        {
            string[] astrNames = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string strInitials = astrNames.Aggregate<string, string>(null, (current, s) => current + s[0]);

            if ((strInitials != null) && (strInitials.Length == 1))
                strInitials += astrNames[0][1];
            return strInitials;
        }

        internal bool SendNoteToCorrectPane(int nVerseIndex, string strReferringText, bool bNoteToSelf)
        {
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.Coach))
            {
                if (!viewCoachNotesMenu.Checked)
                    viewCoachNotesMenu.Checked = true;

#if UsingHtmlDisplayForConNotes
                return htmlCoachNotesControl.OnAddNote(nVerseIndex, strReferringText, bNoteToSelf);
#else
                Control ctrl = flowLayoutPanelCoachNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;

                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl theCoachNotes = ctrl as ConsultNotesControl;
                StringTransfer st = theCoachNotes.DoAddNote(strNote);

                // after the note is added, the control references are no longer valid, but
                //  we want to go back to where we were... so requery the controls
                // Order: BT, then *other* connote pane and then *this* connote pane
                ctrl = flowLayoutPanelVerses.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;

                Debug.Assert(ctrl is VerseBtControl);
                flowLayoutPanelVerses.ScrollControlIntoView(ctrl);

                if (viewConsultantNoteFieldMenuItem.Checked)
                {
                    ctrl = flowLayoutPanelConsultantNotes.GetControlAtVerseIndex(nVerseIndex);
                    if (ctrl == null)
                        return;

                    Debug.Assert(ctrl is ConsultNotesControl);
                    flowLayoutPanelConsultantNotes.ScrollControlIntoView(ctrl);
                }

                ctrl = flowLayoutPanelCoachNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;

                Debug.Assert(ctrl is ConsultNotesControl);
                flowLayoutPanelCoachNotes.ScrollControlIntoView(ctrl);

                if ((st != null) && (st.TextBox != null))
                    st.TextBox.Focus();
#endif
            }
            else
            {
                if (!viewConsultantNotesMenu.Checked)
                    viewConsultantNotesMenu.Checked = true;

#if UsingHtmlDisplayForConNotes
                return htmlConsultantNotesControl.OnAddNote(nVerseIndex, strReferringText, bNoteToSelf);
#else
                Control ctrl = flowLayoutPanelConsultantNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;

                Debug.Assert(ctrl is ConsultNotesControl);
                ConsultNotesControl theConsultantNotes = ctrl as ConsultNotesControl;
                StringTransfer st = theConsultantNotes.DoAddNote(strNote);

                // after the note is added, the control references are no longer valid, but
                //  we want to go back to where we were... so requery the controls
                // Order: BT, then *other* connote pane and then *this* connote pane
                ctrl = flowLayoutPanelVerses.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;

                Debug.Assert(ctrl is VerseBtControl);
                flowLayoutPanelVerses.ScrollControlIntoView(ctrl);

                if (viewCoachNotesFieldMenuItem.Checked)
                {
                    ctrl = flowLayoutPanelCoachNotes.GetControlAtVerseIndex(nVerseIndex);
                    if (ctrl == null)
                        return;

                    Debug.Assert(ctrl is ConsultNotesControl);
                    flowLayoutPanelCoachNotes.ScrollControlIntoView(ctrl);
                }

                ctrl = flowLayoutPanelConsultantNotes.GetControlAtVerseIndex(nVerseIndex);
                if (ctrl == null)
                    return;
                
                Debug.Assert(ctrl is ConsultNotesControl);
                flowLayoutPanelConsultantNotes.ScrollControlIntoView(ctrl);

                if ((st != null) && (st.TextBox != null))
                    st.TextBox.Focus();
#endif
            }
        }

        // if this is a coach, then set the Crafting info if it isn't already
        //  (this assumes that the first consultant type to add a note to the 
        //  ConNote pane is *the* consultant.
        public void CheckUpdateMentorInfoCoach()
        {
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.Coach))
            {
                MemberIdInfo.SetCreateIfEmpty(ref TheCurrentStory.CraftingInfo.Coach,
                                              LoggedOnMember.MemberGuid, false);
            }
        }

        // if this is a consultant, then set the Crafting info if it isn't already
        //  (this assumes that the first consultant type to add a note to the 
        //  ConNote pane is *the* consultant.
        public void CheckUpdateMentorInfoConsultant()
        {
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.IndependentConsultant |
                                      TeamMemberData.UserTypes.ConsultantInTraining))
            {
                MemberIdInfo.SetCreateIfEmpty(ref TheCurrentStory.CraftingInfo.Consultant,
                                              LoggedOnMember.MemberGuid, false);
            }
        }

        internal void AddNewVerse(int nInsertionIndex, string strVernacular,
            string strNationalBT, string strInternationalBT, string strFreeTranslation)
        {
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses != null));
            TheCurrentStory.Verses.InsertVerse(nInsertionIndex,
                                               strVernacular,
                                               strNationalBT,
                                               strInternationalBT,
                                               strFreeTranslation);
            InitAllPanes();

            Modified = true;
        }

        internal void InitAllPanes()
        {
            try
            {
                if (TheCurrentStory != null)
                    InitAllPanes(TheCurrentStory.Verses);
                ResetStatusBar();
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        internal void DeleteVerse(VerseData theVerseDataToDelete)
        {
            MoveConNotes(theVerseDataToDelete);
            TheCurrentStory.Verses.Remove(theVerseDataToDelete);
            InitAllPanes();
            Modified = true;
        }

        internal void VisiblizeVerse(VerseData theVerseDataToVisiblize, bool bVisible)
        {
            // if making it hidden, then put the con notes in the prev (or following) verse
            if (!bVisible)
                MoveConNotes(theVerseDataToVisiblize);

            theVerseDataToVisiblize.IsVisible = bVisible;
            InitAllPanes();
            Modified = true;
        }

        private void MoveConNotes(VerseData theVerseToMoveFrom)
        {
            var nIndexVerse = TheCurrentStory.Verses.IndexOf(theVerseToMoveFrom);
            if (nIndexVerse > 0)
                nIndexVerse--;
            else if (TheCurrentStory.Verses.Count > 1)
                nIndexVerse++;
            else
                return;

            var theTargetVerse = TheCurrentStory.Verses[nIndexVerse];
            ShiftConNotesToOtherVerse(theVerseToMoveFrom.ConsultantNotes, theTargetVerse.ConsultantNotes);
            ShiftConNotesToOtherVerse(theVerseToMoveFrom.CoachNotes, theTargetVerse.CoachNotes);
        }

        private static void ShiftConNotesToOtherVerse(ICollection<ConsultNoteDataConverter> conNotesFrom, 
                                                      List<ConsultNoteDataConverter> conNotesTo)
        {
            var strNote = Localizer.Str("Note from OSE: this conversation was moved here from a now hidden or deleted line");
            foreach (var conNote in conNotesFrom)
            {
                Debug.Assert(conNote.Count > 0);
                var firstNote = conNote[0];
                if (firstNote.ToString().IndexOf(strNote, StringComparison.Ordinal) == -1)
                    firstNote.SetValue(strNote +
                                       Environment.NewLine +
                                       firstNote);

                conNote.guid = Guid.NewGuid().ToString();   // just in case someone else changes it and it comes back and becomes a duplicate
                conNotesTo.Add(conNote);
            }
            conNotesFrom.Clear();
        }

        internal void SetViewBasedOnProjectStage(StoryStageLogic.ProjectStages eStage,
            bool bDisableReInits)
        {
            Debug.Assert(StoryStageLogic.stateTransitions != null);
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[eStage];
            Debug.Assert(st != null);

            if (!viewUseSameSettingsForAllStoriesMenu.Checked)
            {
                // in case the caller is just about to call InitAllPanes anyway, we don't
                //  want the screen to thrash, so have the ability to disable the thrashing.
                _bDisableReInitVerseControls = bDisableReInits;

                // if the user is pressing the control key (e.g. while changing state or 
                //  selecting another story), then don't change the view settings
                if ((ModifierKeys & Keys.Control) != Keys.Control)
                {
                    st.SetView(this);
                }

                _bDisableReInitVerseControls = false;
            }

            helpProvider.SetHelpString(this, st.StageInstructions);
            SetDefaultStatusBar(st.StageDisplayString);
        }

        public static void SetDefaultStatusBar(ToolStripStatusLabel tssl, string strState)
        {
            SetStatusStripLabel(tssl, String.Format(Localizer.Str("{0}  Press F1 for instructions"),
                                                    strState));
        }

        public void SetDefaultStatusBar(string strState)
        {
            SetDefaultStatusBar(statusLabel, strState);
        }

        protected Button AddDropTargetToFlowLayout(int nVerseIndex)
        {
            var buttonDropTarget = new Button
                                       {
                                           AllowDrop = true,
                                           Location = new Point(3, 3),
                                           Name = CstrButtonDropTargetName + nVerseIndex.ToString(),
                                           Size = new Size(Panel1_Width, 10),
                                           Dock = DockStyle.Fill,
                                           TabIndex = nVerseIndex,
                                           UseVisualStyleBackColor = true,
                                           Visible = false,
                                           Tag = nVerseIndex
                                       };
            buttonDropTarget.DragEnter += buttonDropTarget_DragEnter;
            buttonDropTarget.DragDrop += buttonDropTarget_DragDrop;
            flowLayoutPanelVerses.Controls.Add(buttonDropTarget);
            return buttonDropTarget;
        }

        void buttonDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseData)))
            {
                var aVerseData = (VerseData)e.Data.GetData(typeof(VerseData));
                var theDropTarget = sender as Button;
                if (theDropTarget != null)
                {
                    var nInsertionIndex = (int)theDropTarget.Tag;    // (flowLayoutPanelVerses.Controls.IndexOf((Button)sender) / 2);
                    DoMove(nInsertionIndex, aVerseData);
                }
            }
        }

        public void DoMove(int nInsertionIndex, VerseData theVerseToMove)
        {
            int nCurIndex = TheCurrentStory.Verses.IndexOf(theVerseToMove);
            TheCurrentStory.Verses.Remove(theVerseToMove);

            // if we're moving the verse to an earlier position, then remove it from its higher index, 
            //  just insert it at the new lower index. However, if an earlier verse is being moved later, 
            //  then once we remove it, then the insertion index will be one too many
            if (nInsertionIndex > nCurIndex)
                --nInsertionIndex;

            TheCurrentStory.Verses.Insert(nInsertionIndex, theVerseToMove);
            InitAllPanes();
            Modified = true;
        }

        internal void DoPasteVerse(int nInsertionIndex, VerseData theVerseToPaste)
        {
            TheCurrentStory.Verses.Insert(nInsertionIndex, theVerseToPaste);
            // now done by callerInitAllPanes();
            Modified = true;
        }

        void buttonDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseData)))
                e.Effect = DragDropEffects.Move;
        }

        internal void LightUpDropTargetButtons(VerseBtControl aVerseCtrl)
        {
            int nIndex = flowLayoutPanelVerses.Controls.IndexOf(aVerseCtrl);
            int nOffset = 0;
            if (viewGeneralTestingsQuestionMenu.Checked)
                nOffset++;

            for (int i = nOffset; i < flowLayoutPanelVerses.Controls.Count; i += 2)
            {
                Control ctrl = flowLayoutPanelVerses.Controls[i];
                if (ctrl is Button)
                {
                    if (Math.Abs(nIndex - i) > 1)
                        ctrl.Visible = true;
                }
            }
        }

        internal void DimDropTargetButtons()
        {
            var buttons = from Control ctrl in flowLayoutPanelVerses.Controls
                          where (ctrl is Button)
                          select ctrl;
            foreach (Button button in buttons)
                button.Visible = false;
        }

        protected void InitializeNetBibleViewer()
        {
            netBibleViewer.InitNetBibleViewer();
            string strLastRef = "Gen 1:5";
            if (!String.IsNullOrEmpty(Settings.Default.LastNetBibleReference))
                strLastRef = Settings.Default.LastNetBibleReference;
            SetNetBibleVerse(strLastRef);
        }

        internal void SetNetBibleVerse(string strScriptureReference)
        {
            if (splitContainerUpDown.Panel2Collapsed)
                viewBibleMenu.Checked = true;

            if (!netBibleViewer.checkBoxAutoHide.Checked && splitContainerUpDown.IsMinimized)
            {
                // when the user clicks an anchor button, then turn off the auto hide until
                //  they start typing again (see LastKeyPressedTimeStamp)
                _bAutoHide = false;
                splitContainerUpDown.Restore();
            }

            netBibleViewer.DisplayVerses(strScriptureReference);
        }

        internal string GetNetBibleScriptureReference
        {
            get { return netBibleViewer.JumpTarget; }
        }

        protected int Panel1_Width
        {
            get
            {
                return splitContainerLeftRight.Panel1.Width - splitContainerLeftRight.Margin.Horizontal -
                    SystemInformation.VerticalScrollBarWidth;
            }
        }

        internal int Panel2_Width
        {
            get
            {
                return splitContainerLeftRight.Panel2.Width - splitContainerLeftRight.Margin.Horizontal -
                    SystemInformation.VerticalScrollBarWidth - 2;
            }
        }

        private bool CheckForSaveDirtyFile()
        {
            if (!CheckForSaveDirtyFileNoCleanup())
                return false;

            // do cleanup, because this is always called before starting something new (new file or empty project)
            ClearFlowControls();
            textBoxStoryVerse.Text = Localizer.Str("Story");
            return true;
        }

        private bool CheckForSaveDirtyFileNoCleanup()
        {
            // if we're in the 'old stories' window OR if it's a Just looking user, then
            //  ignore the modified flag and return
            if (!IsInStoriesSet ||
                ((LoggedOnMember != null) &&
                 TeamMemberData.IsUser(LoggedOnMember.MemberType, TeamMemberData.UserTypes.JustLooking)))
            {
                Modified = false; // just in case
                return true;
            }

            if (Modified)
            {
                // it's annoying that the keyboard doesn't deactivate so I can just type 'y' for "Yes"
                Program.ActivateDefaultKeyboard(); // ... do it manually

                if (!advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked)
                {
                    var res = QuerySave();
                    if (res == DialogResult.Cancel)
                        return false;
                    if (res == DialogResult.No)
                    {
                        Modified = false;
                        return true;
                    }
                }

                SaveClicked();
            }

            return true;
        }

        protected void ClearFlowControls()
        {
            if (UsingHtmlForStoryBtPane)
            {
                htmlStoryBtControl.ResetDocument();
            }
            else
            {
                flowLayoutPanelVerses.Clear();
            }

            buttonMoveToNextLine.Visible = buttonMoveToPrevLine.Visible = linkLabelVerseBT.Visible = false;
#if UsingHtmlDisplayForConNotes
            htmlConsultantNotesControl.ResetDocument();
            htmlCoachNotesControl.ResetDocument();
            Application.DoEvents(); // give them time to actually empty the webcontrols
#else
            flowLayoutPanelConsultantNotes.Clear();
            flowLayoutPanelCoachNotes.Clear();
#endif
        }

        internal void SaveClicked()
        {
            mySaveTimer.Stop();
            mySaveTimer.Interval = CnIntervalBetweenAutoSaveReqs;
            mySaveTimer.Start();

            if (!IsInStoriesSet || !Modified || (StoryProject == null) || (StoryProject.ProjSettings == null))
                return;

            string strFilename = StoryProject.ProjSettings.ProjectFilePath;

            bool bSaveThisSnapshotInRepo = (DateTime.Now - tmLastSync) > tsBackupTime;
            TriggerSaveUpdates();
            SaveFile(strFilename, bSaveThisSnapshotInRepo);

            if (bSaveThisSnapshotInRepo)
            {
                try
                {
                    Program.BackupInRepo(StoryProject.ProjSettings.ProjectFolder);
                }
                catch (Exception ex)
                {
                    LocalizableMessageBox.Show(String.Format(ex.Message, strFilename),
                                    OseCaption);
                    return;
                }
                tmLastSync = DateTime.Now;
            }
        }

        // if the user was editing and hasn't yet left the textarea, we need to trigger it now
        //  so that we'll get the new value of the textarea
        private void TriggerSaveUpdates()
        {
            if (UsingHtmlForStoryBtPane)
                htmlStoryBtControl.TriggerChangeUpdate();
        }

        protected void SaveXElement(XElement elem, string strFilename, bool bDoReloadTest)
        {
            var strTempFilename = SaveXElementWithBadExtn(elem, strFilename);

            // always do the reload (to stop the 'Root element is null' error)
            bDoReloadTest = true;

            // this reload test is nice, but costly at the end of a project (where time
            //  is of an essense... so just check this when we're storing in the repo
            if (bDoReloadTest)
            {
                // now try to load the xml file. it'll throw if it's malformed 
                //  (so we won't want to put it into the repo)
                ProjectReader projFile;
                ProjectReader.ReadProjectFile(strTempFilename, out projFile);
            }

            // backup the last version to appdata
            // Note: doing File.Move leaves the old file security settings rather than replacing them
            // based on the target directory. Copy, on the other hand, inherits
            // security settings from the target folder, which is what we want to do.
            if (File.Exists(strFilename))
                File.Copy(strFilename, GetBackupFilename(strFilename), true);
            File.Delete(strFilename);
            File.Copy(strTempFilename, strFilename, true);
            File.Delete(strTempFilename);
        }

        private static string SaveXElementWithBadExtn(XElement elem, string strFilename)
        {
            // create the root portions of the XML document and tack on the fragment we've been building
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                elem);

            var strFolderPath = Path.GetDirectoryName(strFilename);
            Debug.Assert(strFolderPath != null, "strFolderPath != null");
            if (!Directory.Exists(strFolderPath))
                Directory.CreateDirectory(strFolderPath);

            // save it with an extra extn.
            var strTempFilename = strFilename + CstrExtraExtnToAvoidClobberingFilesWithFailedSaves;
            doc.Save(strTempFilename);
            return strTempFilename;
        }

        protected const string CstrExtraExtnToAvoidClobberingFilesWithFailedSaves = ".bad";

        internal void QueryStoryPurpose()
        {
            var dlg = new StoryFrontMatterForm(this, StoryProject, TheCurrentStory);

            // since this can affect things, if things were changed, then update the panes
            //  (e.g. change of Consultant could result in different buttons)
            if ((dlg.ShowDialog() == DialogResult.OK) && dlg.Modified)
                InitAllPanes();
        }

        protected void SaveFile(string strFilename, bool bDoReloadTest)
        {
            try
            {
#if TurnOnQueryStoryPurpose
                // let's see if the UNS entered the purpose and resources used on this story
                if (TheCurrentStory != null)
                {
                    Debug.Assert(TheCurrentStory.CraftingInfo != null);
                    if (TheCurrentStory.CraftingInfo.IsBiblicalStory
                        && TeamMemberData.IsUser(LoggedOnMember.MemberType, TeamMemberData.UserTypes.ProjectFacilitator)
                        && (((int)TheCurrentStory.ProjStage.ProjectStage) > (int)StoryStageLogic.ProjectStages.eProjFacTypeVernacular)
                        && (String.IsNullOrEmpty(TheCurrentStory.CraftingInfo.StoryPurpose)
                        || String.IsNullOrEmpty(TheCurrentStory.CraftingInfo.ResourcesUsed)))
                        QueryStoryPurpose();
                }
#endif
                if (File.Exists(strFilename) && (_dateTimeLastSaved == DateTime.MinValue))
                {
                    LocalizableMessageBox.Show(
                        Localizer.Str("You must reload the project before it can be saved again."), OseCaption);
                    Modified = false;
                    return;
                }

                SaveXElement(GetXml, strFilename, bDoReloadTest);
                _dateTimeLastSaved = File.GetLastWriteTime(strFilename);
            }
            catch (UnauthorizedAccessException)
            {
                LocalizableMessageBox.Show(
                    String.Format(
                        Localizer.Str(
                            "The project file '{0}' is locked. Is it read-only? Or opened in some other program? Unlock it and try again. Or try to save it as a different name"),
                        strFilename),
                    OseCaption);
                return;
            }
            catch (Exception ex)
            {
                // too severe: ErrorReport.ReportNonFatalException(new Exception(ex.Message));
                LocalizableMessageBox.Show(ex.Message, OseCaption);
                return;
            }

            Program.SetProjectForSyncage(StoryProject.ProjSettings.ProjectFolder);
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

        protected bool _bDisableReInitVerseControls = false;

        private void viewFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bDisableReInitVerseControls)
                ReInitVerseControls();
        }

        private void viewNetBibleMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            splitContainerUpDown.Panel2Collapsed = !tsm.Checked;
        }

        private void viewConsultantNoteFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender is ToolStripMenuItem);
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
            if (splitContainerLeftRight.Panel2Collapsed)   // if the whole right-half is already collapsed...
            {
                // ... first enable it.
                splitContainerLeftRight.Panel2Collapsed = false;

                // glitch, whichever half (consultant's or coach's) was collapsed last will still be active even
                //  though it's menu item will be reset. So we need to hide it if we're enabling the other one
                if (!splitContainerMentorNotes.Panel2Collapsed) // this means it's not actually hidden
                    splitContainerMentorNotes.Panel2Collapsed = true;
                else
                    splitContainerLeftRight_Panel2_SizeChanged(sender, e);
            }

            splitContainerMentorNotes.Panel1Collapsed = false;
        }

        private void viewCoachNotesFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender is ToolStripMenuItem);
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
            if (splitContainerLeftRight.Panel2Collapsed)   // if the whole right-half is already collapsed...
            {
                // ... first enable it.
                splitContainerLeftRight.Panel2Collapsed = false;

                // glitch, whichever half (consultant's or coach's) was collapsed last will still be active even
                //  though it's menu item will be reset. So we need to hide it if we're enabling the other one
                if (!splitContainerMentorNotes.Panel1Collapsed) // this means it's not actually hidden
                    splitContainerMentorNotes.Panel1Collapsed = true;
                else
                    splitContainerLeftRight_Panel2_SizeChanged(sender, e);
            }

            splitContainerMentorNotes.Panel2Collapsed = false;
        }

        private void splitContainerLeftRight_Panel2_SizeChanged(object sender, EventArgs e)
        {
#if UsingHtmlDisplayForConNotes
#else
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
#endif
        }

        private void splitContainerLeftRight_Panel1_SizeChanged(object sender, EventArgs e)
        {
            if (!UsingHtmlForStoryBtPane)
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
            if (IsInStoriesSet)
            {
                projectRecentProjectsMenu.DropDownItems.Clear();
                Debug.Assert(Settings.Default.RecentProjects.Count == Settings.Default.RecentProjectPaths.Count);
                for (int i = 0; i < Settings.Default.RecentProjects.Count; i++)
                {
                    string strRecentFile = Settings.Default.RecentProjects[i];
                    ToolStripItem tsi = projectRecentProjectsMenu.DropDownItems.Add(strRecentFile, null, recentProjectsToolStripMenuItem_Click);
                    tsi.ToolTipText = String.Format(Localizer.Str("Located in folder '{0}'"),
                                                    Settings.Default.RecentProjectPaths[i]);
                }

                projectRecentProjectsMenu.Enabled = (projectRecentProjectsMenu.DropDownItems.Count > 0);

                projectToTheInternetMenu.Enabled =
                    projectFromASharedNetworkDriveMenu.Enabled =
                        ((StoryProject != null) && (StoryProject.ProjSettings != null));

                projectSaveProjectMenu.Enabled = Modified;

                projectExportToToolboxMenu.Enabled =
                    projectSettingsMenu.Enabled = ((StoryProject != null)
                        && (StoryProject.ProjSettings != null)
                        && (LoggedOnMember != null));

                projectLoginMenu.Enabled = (StoryProject != null);

                if ((StoryProject != null) && (StoryProject.ProjSettings != null))
                    projectSendReceiveMenu.Enabled = !String.IsNullOrEmpty(StoryProject.ProjSettings.HgRepoUrlHost);
                else
                    projectSendReceiveMenu.Enabled = false;
            }
            else
            {
                projectToTheInternetMenu.Enabled =
                    projectFromASharedNetworkDriveMenu.Enabled =
                    projectRecentProjectsMenu.Enabled =
                    projectSaveProjectMenu.Enabled =
                    projectBrowseForProjectFileMenu.Enabled =
                    projectSettingsMenu.Enabled =
                    projectLoginMenu.Enabled =
                    projectSendReceiveMenu.Enabled = false;
            }

            projectPrintMenu.Enabled = (StoryProject != null);
        }

        private void recentProjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aRecentFile = (ToolStripDropDownItem)sender;
            string strProjectName = aRecentFile.Text;
            Debug.Assert(Settings.Default.RecentProjects.Contains(strProjectName));
            int nIndexOfPath = Settings.Default.RecentProjects.IndexOf(strProjectName);
            string strProjectPath = Settings.Default.RecentProjectPaths[nIndexOfPath];
            DoReopen(strProjectPath, strProjectName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();    // "Closing" event will take care of checking for save
        }

        protected string StoryName
        {
            get { return (string)comboBoxStorySelector.SelectedItem; }
        }

        public XElement GetXml
        {
            get
            {
                Debug.Assert(StoryProject != null);
                return StoryProject.GetXml;
            }
        }

        internal void SetStatusBar(string strText)
        {
            SetStatusStripLabel(statusLabel, strText);
        }

        public static void SetStatusStripLabel(ToolStripStatusLabel tssl, string strText)
        {
            tssl.Text = strText;
        }

        internal void ResetStatusBar()
        {
            if (TheCurrentStory == null)
                return;

            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[TheCurrentStory.ProjStage.ProjectStage];
            SetDefaultStatusBar(st.StageDisplayString);
        }

        /*
        private void buttonsStoryStage_DropDownOpening(object sender, EventArgs e)
        {
            if ((StoryProject == null) || (theCurrentStory == null))
                return;

            buttonPrevState.DropDown.Items.Clear();

            // get the current StateTransition object and find all of the allowable transition states
            StoryStageLogic.StateTransition theCurrentST = StoryStageLogic.stateTransitions[theCurrentStory.ProjStage.ProjectStage];
            Debug.Assert(theCurrentST != null);

            AddListOfButtons(theCurrentST.AllowableBackwardsTransitions);
        }

        protected bool AddListOfButtons(List<StoryStageLogic.AllowableTransition> allowableTransitions)
        {
            if (allowableTransitions.Count == 0)
                return false;

            foreach (StoryStageLogic.AllowableTransition aps in allowableTransitions)
            {
                // put the allowable transitions into the DropDown list
                if ((!aps.RequiresUsingVernacular || StoryProject.ProjSettings.Vernacular.HasData)
                    && (!aps.RequiresUsingNationalBT || StoryProject.ProjSettings.NationalBT.HasData)
                    && (!aps.RequiresUsingEnglishBT || StoryProject.ProjSettings.InternationalBT.HasData)
                    && (!aps.RequiresBiblicalStory || theCurrentStory.CraftingInfo.IsBiblicalStory)
                    && (!aps.RequiresFirstPassMentor || StoryProject.TeamMembers.HasOutsideEnglishBTer)
                    && (!aps.HasUsingOtherEnglishBTer
                        || (aps.RequiresUsingOtherEnglishBTer ==
                            StoryProject.TeamMembers.HasOutsideEnglishBTer))
                    && (!aps.RequiresManageWithCoaching || !StoryProject.TeamMembers.HasIndependentConsultant)
                    )
                {
                    StoryStageLogic.StateTransition aST = StoryStageLogic.stateTransitions[aps.ProjectStage];
                    ToolStripItem tsi = buttonPrevState.DropDown.Items.Add(
                        aST.StageDisplayString, null, OnSelectOtherState);
                    tsi.Tag = aST;
                }
            }
            return true;
        }

        protected void OnSelectOtherState(object sender, EventArgs e)
        {
            Debug.Assert(sender is ToolStripItem);
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
                    Debug.Assert(theCurrentST.IsTransitionValid(theNewST.CurrentStage));
                    // if this is the last transition before they lose edit privilege, then make
                    //  sure they really want to do this.
                    if (theCurrentStory.ProjStage.IsTerminalTransition(theNewST.CurrentStage)
                        && (theNewST.MemberTypeWithEditToken != LoggedOnMember.MemberType))
                        if (LocalizableMessageBox.Show(
                                String.Format(Properties.Resources.IDS_TerminalTransitionMessage,
                                TeamMemberData.GetMemberTypeAsDisplayString(theNewST.MemberTypeWithEditToken),
                                theNewST.StageDisplayString),
                             StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return;

                    theCurrentStory.ProjStage.ProjectStage = theNewST.CurrentStage;
                    theCurrentStory.StageTimeStamp = DateTime.Now;
                    SetViewBasedOnProjectStage(theCurrentStory.ProjStage.ProjectStage, true);
                    tmLastSync = DateTime.Now - tsBackupTime;   // triggers a repository story when we ask if they want to save
                    Modified = true;
                    break;
                }

                if (theCurrentST.CurrentStage != theNewST.CurrentStage)
                    if (!DoNextState(false))
                        break;
            }
            while (theCurrentST.NextState != theNewST.CurrentStage);
            InitAllPanes();
        }
        */

        internal bool _bByNextStateButton = false;
        private void buttonsStoryStage_Click(object sender, EventArgs e)
        {
            if ((StoryProject == null) || (StoryProject.ProjSettings == null) || (TheCurrentStory == null))
                return;

            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[TheCurrentStory.ProjStage.ProjectStage];
            _bByNextStateButton = true;
            SetNextState(st.DefaultNextState(StoryProject, TheCurrentStory), true);
            _bByNextStateButton = false;
        }

        protected bool SetNextState(StoryStageLogic.ProjectStages stateToSet, bool bDoUpdateCtrls)
        {
            if ((StoryProject == null) || (StoryProject.ProjSettings == null) || (TheCurrentStory == null))
                return false;

            if (SetNextStateIfReady(stateToSet))
            {
                SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage,
                    bDoUpdateCtrls);

                if (bDoUpdateCtrls)
                    InitAllPanes();    // just in case there were changes

                Modified = true;
                return true;
            }
            return false;
        }

        protected bool SetNextStateIfReady(StoryStageLogic.ProjectStages stateToSet)
        {
            try
            {
                LoggedOnMember.ThrowIfEditIsntAllowed(TheCurrentStory);
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, OseCaption);
                return false;
            }

            if ((TheCurrentStory.ProjStage.ProjectStage == stateToSet)
                && (stateToSet == StoryStageLogic.ProjectStages.eTeamFinalApproval))
            {
                LocalizableMessageBox.Show(Resources.IDS_GoBackwardsYoungMan,
                                OseCaption);
                return false;
            }

            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[TheCurrentStory.ProjStage.ProjectStage];
            bool bRet = st.IsReadyForTransition(this, StoryProject, TheCurrentStory, ref stateToSet);
            if (bRet)
            {
                StoryStageLogic.ProjectStages eNextState = stateToSet;
                StoryStageLogic.StateTransition stNext = StoryStageLogic.stateTransitions[eNextState];
                bool bReqSave = false;
                if (TheCurrentStory.ProjStage.IsTerminalTransition(eNextState))
                {
                    if (!QueryTerminalTransition)
                        return false;

                    tmLastSync = DateTime.Now - tsBackupTime;   // triggers a repository story when we ask if they want to save
                    bReqSave = true;  // request a save if we've just done a terminal transition
                }
                // a record to our history
                TheCurrentStory.TransitionHistory.Add(LoggedOnMember.MemberGuid,
                    TheCurrentStory.ProjStage.ProjectStage, eNextState);
                TheCurrentStory.ProjStage.ProjectStage = eNextState;  // if we are ready, then go ahead and transition
                TheCurrentStory.StageTimeStamp = DateTime.Now;
                Modified = true;

                if (bReqSave)
                    if (!CheckForSaveDirtyFile())
                        return false;
            }
            return bRet;
        }

        internal void SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages stateToSet,
            bool bTriggerSnapshotSave)
        {
            if (bTriggerSnapshotSave && !TheCurrentStory.JustAdded && StoryProject.ProjSettings.DropboxStory &&
                (LoggedOnMember.MemberType == TeamMemberData.UserTypes.ProjectFacilitator))
            {
                TriggerDropboxCopyStory(true, null);
            }

            // a record to our history
            TheCurrentStory.TransitionHistory.Add(LoggedOnMember.MemberGuid,
                TheCurrentStory.ProjStage.ProjectStage, stateToSet);
            TheCurrentStory.ProjStage.ProjectStage = stateToSet;  // if we are ready, then go ahead and transition
            TheCurrentStory.StageTimeStamp = DateTime.Now;
            Modified = true;

            SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, true);

            if (bTriggerSnapshotSave)
            {
                tmLastSync = DateTime.Now - tsBackupTime;   // triggers a repository store when we ask if they want to save
                CheckForSaveDirtyFile();
            }

            InitAllPanes();
        }

        private void storyToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            storyStoryInformationMenu.Enabled = ((TheCurrentStory != null) &&
                                                 (TheCurrentStory.CraftingInfo != null));

            storyDeleteStoryMenu.Enabled =
                storyCopyWithNewNameMenu.Enabled =
                storyCopyToAnotherProjectMenu.Enabled = 
                    (TheCurrentStory != null);

            var isStoryInsertable = ((StoryProject != null) &&
                                     (IsInStoriesSet) &&
                                     (LoggedOnMember != null));

            panoramaInsertNewStoryMenu.Enabled =
                panoramaAddNewStoryAfterMenu.Enabled = isStoryInsertable;

            storyImportFromSayMore.Enabled = isStoryInsertable &&
                                             Directory.Exists(ProjectSettings.SayMoreFolderRoot);

            // the 'paste story from another project is enabled if ...
            if (isStoryInsertable)
            {
                var iData = Clipboard.GetDataObject();
                storyCopyFromAnotherProjectMenu.Enabled = (iData != null) &&
                                                          iData.GetDataPresent(DataFormats.UnicodeText) &&
                                                          (iData.GetData(DataFormats.UnicodeText).
                                                              ToString().Contains(
                                                                  StoryProjectData.
                                                                      CstrElementOseStoryToCopy));
            }
            else
                storyCopyFromAnotherProjectMenu.Enabled = false;
                                                      

            // if there's a story that has more than no verses, AND if it's a bible
            //  story and before the add anchors stage or a non-biblical story and
            //  before the consultant check stage...
            if ((TheCurrentStory != null) 
                && (_theCurrentStory.Verses != null) && (TheCurrentStory.Verses.Count > 0)
                && (TheCurrentStory.CraftingInfo != null)
                && ((TheCurrentStory.CraftingInfo.IsBiblicalStory ||
                        (TheCurrentStory.ProjStage.ProjectStage < StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory))))
            {
                // then we can do splitting and collapsing of the story
                storySplitIntoLinesMenu.Enabled =
                    storyRealignStoryLinesMenu.Enabled = true;

                if (TheCurrentStory.Verses.Count == 1)
                    storySplitIntoLinesMenu.Text = Localizer.Str("S&plit into Lines");
                else
                    storySplitIntoLinesMenu.Text = Localizer.Str("&Collapse into 1 line");
            }
            else
                storySplitIntoLinesMenu.Enabled =
                    storyRealignStoryLinesMenu.Enabled = false;

            if ((StoryProject != null)
                && (StoryProject.ProjSettings != null)
                && (TheCurrentStory != null)
                && (TheCurrentStory.Verses.Count > 0))
            {
                storyUseAdaptItForBackTranslationMenu.Enabled = true;
                bool bAnySharedProjects = false;
                if (StoryProject.ProjSettings.VernacularToNationalBt != null)
                {
                    storyAdaptItVernacularToNationalMenu.Text =
                        FromLangToLang(StoryProject.ProjSettings.Vernacular.LangName,
                                       StoryProject.ProjSettings.NationalBT.LangName);

                    storyAdaptItVernacularToNationalMenu.Visible = true;
                    bAnySharedProjects |= (StoryProject.ProjSettings.VernacularToNationalBt.ProjectType ==
                                           ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject);
                }
                else
                    storyAdaptItVernacularToNationalMenu.Visible = false;

                // if (StoryProject.ProjSettings.Vernacular.HasData && StoryProject.ProjSettings.InternationalBT.HasData)
                if (StoryProject.ProjSettings.VernacularToInternationalBt != null)
                {
                    storyAdaptItVernacularToEnglishMenu.Text =
                        FromLangToLang(StoryProject.ProjSettings.Vernacular.LangName,
                                       StoryProject.ProjSettings.InternationalBT.LangName);

                    storyAdaptItVernacularToEnglishMenu.Visible = true;
                    bAnySharedProjects |= (StoryProject.ProjSettings.VernacularToInternationalBt.ProjectType ==
                                           ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject);
                }
                else
                    storyAdaptItVernacularToEnglishMenu.Visible = false;

                // if (StoryProject.ProjSettings.NationalBT.HasData && StoryProject.ProjSettings.InternationalBT.HasData)
                if (StoryProject.ProjSettings.NationalBtToInternationalBt != null)
                {
                    storyAdaptItNationalToEnglishMenu.Text =
                        FromLangToLang(StoryProject.ProjSettings.NationalBT.LangName,
                                       StoryProject.ProjSettings.InternationalBT.LangName);

                    storyAdaptItNationalToEnglishMenu.Visible = true;
                    bAnySharedProjects |= (StoryProject.ProjSettings.NationalBtToInternationalBt.ProjectType ==
                                           ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject);
                }
                else
                    storyAdaptItNationalToEnglishMenu.Visible = false;

                storyUseAdaptItForBackTranslationMenu.Enabled = 
                    storySynchronizeSharedAdaptItProjectsMenu.Visible = bAnySharedProjects;

                storyOverrideTasksMenu.Enabled = ((_theCurrentStory != null) &&
                                                    TeamMemberData.IsUser(_theCurrentStory.ProjStage.MemberTypeWithEditToken,
                                                                          TeamMemberData.UserTypes.ProjectFacilitator |
                                                                          TeamMemberData.UserTypes.ConsultantInTraining));
            }
            else
                storyOverrideTasksMenu.Enabled = 
                    storyUseAdaptItForBackTranslationMenu.Enabled = false;
        }

        private static string FromLangToLang(string strFromLang, string strToLang)
        {
            return String.Format(Localizer.Str("From &{0} to {1}"),
                                 strFromLang,
                                 strToLang);
        }

        private void enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryStoryPurpose();
        }

        private void panoramaToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Debug.Assert(IsInStoriesSet);
            panoramaShowMenu.Enabled = (StoryProject != null);
            panoramaInsertNewStoryMenu.Enabled =
                panoramaAddNewStoryAfterMenu.Enabled = 
                ((StoryProject != null) && (LoggedOnMember != null));
        }

        internal void toolStripMenuItemShowPanorama_Click(object sender, EventArgs e)
        {
            if (StoryProject == null)
                return;

            // keep track of the index of the current story (in case it gets deleted)
            int nIndex = (TheCurrentStory != null) ? TheCurrentStoriesSet.IndexOf(TheCurrentStory) : -1;

            var dlg = new PanoramaView(StoryProject, LoggedOnMember);
            dlg.ShowDialog(CurrentStoriesSetName);

            if (dlg.Modified)
            {
                // this means that the order was probably switched, so we have to reload the combo box
                comboBoxStorySelector.Items.Clear();
                foreach (StoryData aStory in TheCurrentStoriesSet)
                    comboBoxStorySelector.Items.Add(aStory.Name);

                // if the current story has been deleted, then choose another
                if (TheCurrentStory != null)
                {
                    int nNewIndex = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
                    if (nNewIndex != -1)
                    {
                        nIndex = -1;

                        // also check to see if its name has been changed
                        UpdateStoryNameComboBox();
                    }
                }

                if (nIndex > 0)
                    nIndex--;

                // if we get here, it's because we deleted the current story
                if (TheCurrentStoriesSet.Count == 0)
                {
                    // just close the project
                    ClearState();
                }
                else if ((nIndex >= 0) && (nIndex < TheCurrentStoriesSet.Count))
                {
                    JumpToStory(TheCurrentStoriesSet[nIndex].Name);
                }

                Modified = true;
            }

            // not sure why we had this null case, but it's causing Irene grief by shifting to the 1st story in the set when you just exit the Panorama window
#if true
            if (!String.IsNullOrEmpty(dlg.JumpToStory))
                NavigateTo(dlg.SelectedStorySetName, dlg.JumpToStory, 1, null);
#else
            if (String.IsNullOrEmpty(dlg.JumpToStory))
            {
                if (comboBoxStorySelector.Items.Count > 0)
                    comboBoxStorySelector.SelectedIndex = 0;
            }
            else
            {
                NavigateTo(dlg.SelectedStorySetName, dlg.JumpToStory, 1, null);
                // JumpToStory(dlg.JumpToStory);
            }
#endif
            }

            private void JumpToStory(string jumpToStory)
        {
            comboBoxStorySelector.SelectedItem =
                comboBoxStorySelector.Text = jumpToStory;
        }

        private void UpdateStoryNameComboBox()
        {
            if ((TheCurrentStory != null) && (TheCurrentStory.Name != comboBoxStorySelector.Text))
                comboBoxStorySelector.Text = TheCurrentStory.Name;
        }

        private void deleteStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert(TheCurrentStory != null);

            int nIndex = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
            Debug.Assert(nIndex != -1);
            if (nIndex == -1)
                return;

            // make sure the user really wants to do this
            var strName = TheCurrentStory.Name;
            if (QueryDeleteStory(strName))
                return;

            // remove it from this set and move it to Old Stories set (unless
            //  this is the OldStories set)
            TheCurrentStoriesSet.RemoveAt(nIndex);
            if (IsInStoriesSet)
            {
                InsertInOtherSetInsureUnique(StoryProject[Resources.IDS_ObsoleteStoriesSet],
                                             TheCurrentStory);
            }

            TheCurrentStory = null; //clear this out so we don't try to query for story information

            // update the combox box bit
            Debug.Assert(comboBoxStorySelector.Items.IndexOf(strName) == nIndex);
            comboBoxStorySelector.Items.Remove(strName);

            if (nIndex > 0)
                nIndex--;
            if (nIndex < TheCurrentStoriesSet.Count)
            {
                JumpToStory(TheCurrentStoriesSet[nIndex].Name);
            }
            else if (TheCurrentStoriesSet.Count == 0)
            {
                // just close the project
                ClearState();
            }
            Modified = true;
        }

        public static void InsertInOtherSetInsureUnique(StoriesData theSds,
            StoryData theStory)
        {
            int n = 1;
            if (theSds.Contains(theStory))
            {
                var strName = theStory.Name;
                while (theSds.Contains(theStory))
                    theStory.Name = String.Format("{0}.{1}", strName, n++);
                theStory.guid = Guid.NewGuid().ToString();
            }
            theSds.Add(theStory);
        }

        public static bool QueryDeleteStory(string strName)
        {
            return (LocalizableMessageBox.Show(String.Format(Localizer.Str("Are you sure you want to delete the '{0}' story?"),
                                                  strName),
                                    OseCaption,
                                    MessageBoxButtons.YesNoCancel)
                    != DialogResult.Yes);
        }

        private void StoryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_frmFind = null;

            // if this is the Old Stories form, then we're done
            if (!IsInStoriesSet)
                return;

            if (Modified)
            {
                if (!advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked)
                {
                    DialogResult res = QuerySave();
                    if (res == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (res != DialogResult.Yes)
                    {
                        Modified = false;
                        return;
                    }
                }

                SaveClicked();
            }

            // do the sync'ing now before the main window goes away (or users are too quick
            //  to try to launch it again, before the first instance actually goes away)
            //  also, this could be time consuming.
            if (!_bRestarting)
            {
                Cursor = Cursors.WaitCursor;
                Program.SyncBeforeClose(false);
                Cursor = Cursors.Default;
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool bSomeVerses = ((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));
            editFindMenu.Enabled =
                editCopyToolStripMenu.Enabled =
                editCopyNationalBtMenu.Enabled =
                editCopyEnglishBtMenu.Enabled =
                editCopyFreeTranslationMenu.Enabled = bSomeVerses;

            editDeleteToolStripMenu.Enabled =
                editDeleteNationalBtMenu.Enabled =
                editDeleteEnglishBtMenu.Enabled =
                editDeleteFreeTranslationMenu.Enabled =
                    ((IsInStoriesSet) && bSomeVerses);

            var bCanEdit = (TheCurrentStory != null) && (LoggedOnMember != null) && CheckForProperEditToken();
            editAddRetellingTestResultsMenu.Enabled = 
                editAddInferenceTestResultsMenu.Enabled =
                editAddGeneralTestQuestionMenu.Enabled =
                    ((IsInStoriesSet)
                        && bSomeVerses
                        && TheCurrentStory.CraftingInfo.IsBiblicalStory
                        && bCanEdit);

            if (UsingHtmlForStoryBtPane)
            {
                editPasteMenu.Enabled = !String.IsNullOrEmpty(HtmlStoryBtControl.LastTextareaInFocusId);

                editCopySelectionMenu.Enabled = (!String.IsNullOrEmpty(HtmlStoryBtControl.LastTextareaInFocusId) &&
                                                 (!String.IsNullOrEmpty(htmlStoryBtControl.GetSelectedText)));
            }
            else
            {
                editPasteMenu.Enabled = (CtrlTextBox._inTextBox != null);

                editCopySelectionMenu.Enabled = ((CtrlTextBox._inTextBox != null) && (!String.IsNullOrEmpty(CtrlTextBox._inTextBox.SelectedText)));
            }

            if ((StoryProject != null) && (StoryProject.ProjSettings != null) && (TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0))
            {
                if (StoryProject.ProjSettings.Vernacular.HasData)
                    editCopyStoryMenu.Text = String.Format(Localizer.Str("{0} story lines to the clipboard"),
                                                           StoryProject.ProjSettings.Vernacular.LangName);
                else
                {
                    editCopyStoryMenu.Visible =
                        editDeleteStoryLinesMenu.Visible = false;
                }

                if (StoryProject.ProjSettings.NationalBT.HasData)
                {
                    editCopyNationalBtMenu.Text =
                        editDeleteNationalBtMenu.Text =
                        String.Format(Localizer.Str("{0} back-translation of the story to the clipboard"),
                                      StoryProject.ProjSettings.NationalBT.LangName);
                }
                else
                    editDeleteNationalBtMenu.Visible = editCopyNationalBtMenu.Visible = false;

                if (!StoryProject.ProjSettings.InternationalBT.HasData)
                    editDeleteEnglishBtMenu.Visible =
                        editCopyEnglishBtMenu.Visible = false;

                if (!StoryProject.ProjSettings.FreeTranslation.HasData)
                    editDeleteFreeTranslationMenu.Visible =
                        editCopyFreeTranslationMenu.Visible = false;

                editDeleteTestToolStripMenu.DropDownItems.Clear();
                if ((TheCurrentStory.CraftingInfo.TestersToCommentsRetellings.Count > 0)
                    || (TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Count > 0))
                {
                    for (int nTest = 0; nTest < TheCurrentStory.CraftingInfo.TestersToCommentsRetellings.Count; nTest++)
                    {
                        string strUnsGuid = TheCurrentStory.CraftingInfo.TestersToCommentsRetellings[nTest].MemberId;
                        AddDeleteTestSubmenu(editDeleteTestToolStripMenu,
                                             String.Format("Retelling Test {0} done by {1}", nTest + 1,
                                                           StoryProject.GetMemberNameFromMemberGuid(strUnsGuid)),
                                             nTest, OnRemoveRetellingTest);
                    }
                    for (int nTest = 0; nTest < TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Count; nTest++)
                    {
                        string strUnsGuid = TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers[nTest].MemberId;
                        AddDeleteTestSubmenu(editDeleteTestToolStripMenu,
                                             String.Format("Story Question Testing {0} done by {1}", nTest + 1,
                                                           StoryProject.GetMemberNameFromMemberGuid(strUnsGuid)),
                                             nTest, OnRemoveInferenceTest);
                    }
                }
                else
                    editDeleteTestToolStripMenu.Visible = false;
            }

            UpdateUIMenusWithShortCuts();
        }

        private void editAddTestResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRetellingAndPaint();
        }

        public void AddRetellingAndPaint()
        {
            AddRetellingTest(false);
            InitAllPanes();
        }

        private void editAddInferenceTestResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddInferenceTestBoxes();
        }

        internal bool AddInferenceTestBoxes()
        {
            if (!CheckEndOfStateTransition.CheckForCountOfTestingQuestions(TheCurrentStory, this))
                return false;

            if (LocalizableMessageBox.Show(Localizer.Str("Have you entered all of your testing questions? If not, then click 'Cancel' and finish entering all of the testing questions first. Then click 'OK' to add boxes for the answers to the testing questions"),
                                OseCaption,
                                MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return false;

            AddInferenceTest();
            InitAllPanes();
            return true;
        }

        internal bool AddRetellingTest(bool bBlockCopyToDropbox)
        {
            // query for the UNSs that will be doing this test
            string strUnsGuid = null;
            if (!GetUniqueTester(TheCurrentStory.CraftingInfo.TestersToCommentsRetellings,
                ref strUnsGuid))
                return false;

            // also need to query for the number of times the UNS heard the story
            string strAnswer =
                LocalizableMessageBox.InputBox(
                    Localizer.Str("How many times did the UNS hear the story before giving this retelling?"),
                    OseCaption, "3");

            strAnswer = String.Format(Localizer.Str("Test done on: {0}. UNS heard the story {1} times."),
                                      DateTime.Now.ToString("yyyy-MMM-dd"), strAnswer);

            var toi = new MemberIdInfo(strUnsGuid, strAnswer);
            TheCurrentStory.CraftingInfo.TestersToCommentsRetellings.Add(toi);
            foreach (VerseData aVerseData in TheCurrentStory.Verses)
                aVerseData.Retellings.TryAddNewLine(strUnsGuid);

            if (TheCurrentStory.CountRetellingsTests > 0)
                TheCurrentStory.CountRetellingsTests--; // to show we've added one

            // just in case it wasn't showing
            viewRetellingsMenu.Checked = true;

            Modified = true;

            // check for Dropbox copy
            if (!bBlockCopyToDropbox)
                TriggerDropboxCopyRetelling(StoryProject.ProjSettings.DropboxRetelling);
            return true;
        }

        private void LaunchWordPad(string strRecordingFilespec)
        {
            // else check in the person's registry for it
            const string cstrWavePadRegKey = @"Software\Conduit\AppPaths\WavePad.exe";
            var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(cstrWavePadRegKey);
            if (keyOneStoryHiveRoot == null)
                return;

            var strWavePadPath = (string)keyOneStoryHiveRoot.GetValue("AppPath");
            if (String.IsNullOrEmpty(strWavePadPath))
                return;

            LaunchProgram(ExcapePath(strWavePadPath), ExcapePath(strRecordingFilespec));
        }

        private string ExcapePath(string str)
        {
            return "\"" + str + "\"";
        }

        private string ShouldCopyFileToDropbox(string strFilename, bool bCopyToDropbox, string strFileToCopy)
        {
            // don't bother with querying for the file unless we're actually going to copy it to dropbox
            //  (the idea of opening it in WavePad is just too annoying
            Debug.Assert((StoryProject != null) && (StoryProject.ProjSettings != null));
            if (!StoryProject.ProjSettings.UseDropbox)
                return null;

            if (String.IsNullOrEmpty(strFileToCopy))
            {
                var openMediaFileDlg = new OpenFileDialog
                {
                    Filter = Resources.AudioFileSearchFilter,
                    Title = Localizer.Str("Select the recording for the ") + strFilename
                };

                SuspendSaveDialog++;
                var res = openMediaFileDlg.ShowDialog();
                SuspendSaveDialog--;
                if (res != DialogResult.OK)
                    return null;

                // if we were just getting the file name (e.g. to open it in wavepad), then 
                //  just return it.
                if (!bCopyToDropbox)
                    return openMediaFileDlg.FileName;

                strFileToCopy = openMediaFileDlg.FileName;
            }

            // otherwise, we're copying it to dropbox
            string strDropboxRoot = ProjectSettings.DropboxFolderRoot;
            if (strDropboxRoot == null)
                return null;

            var extn = Path.GetExtension(strFileToCopy);
            var strTargetPath = ConcatenateToPath(strDropboxRoot,
                                                  new List<string>
                                                      {
                                                          "OSE recordings",
                                                          "private.languageDepot.org",
                                                          StoryProject.ProjSettings.ProjectName,
                                                          TheCurrentStory.Name
                                                      });

            Debug.Assert(strTargetPath != null);
            if (!Directory.Exists(strTargetPath))
                Directory.CreateDirectory(strTargetPath);

            var strDropBoxFilepath = BuildDropboxFilename(strTargetPath, strFilename, extn);
            WriteAudioFile(strFileToCopy, strDropBoxFilepath);
            return strDropBoxFilepath;
        }

        private static string BuildDropboxFilename(string strTargetPath, string strFilename, string extn)
        {
            // give it the name one greater than the highest v# currently
            // start by finding out which files are already there.... eg. search for 'Story v*.*'
            var filter = strFilename + " v";
            var files = Directory.GetFiles(strTargetPath, filter + "*.*")
                .OrderByDescending(s => GetVersionNumber(s, filter));
            
            var nVersion = 1;
            if (files.Any())
                nVersion = GetVersionNumber(files.First(), filter) + 1;

            var strName = String.Format("{0}{1}{2}", filter, nVersion, extn);
            return Path.Combine(strTargetPath, strName);
        }

        private static int GetVersionNumber(string strFilepath, string filter)
        {
            var strVersion = Path.GetFileNameWithoutExtension(strFilepath).Substring(filter.Length);
            int nVersion;
            Int32.TryParse(strVersion, out nVersion);
            return nVersion;
        }

        private void WriteAudioFile(string strSourceFilepath, string strTargetFilepath)
        {
            // TODO: add convert capability also..
            File.Copy(strSourceFilepath, strTargetFilepath, true);
            File.SetLastWriteTime(strTargetFilepath, DateTime.Now);
        }

        internal string ConcatenateToPath(string strFolderRoot, List<string> lstFolderNames)
        {
            var str = lstFolderNames.Aggregate("", Path.Combine);
            return Path.Combine(strFolderRoot, str);
        }

        internal bool AddInferenceTest()
        {
            // query for the UNSs that will be doing this test
            string strUnsGuid = null;
            if (!GetUniqueTester(TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers,
                ref strUnsGuid))
                return false;

            string strAnswer = String.Format(InferenceTestCommentFormat,
                                             DateTime.Now.ToString("yyyy-MMM-dd"));
            var toi = new MemberIdInfo(strUnsGuid, strAnswer);
            TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Add(toi);

            // add boxes for any GTQs that are present and then to all the other verses
            AddAnswerBoxes(strUnsGuid, TheCurrentStory.Verses.FirstVerse);
            foreach (var aVerseData in TheCurrentStory.Verses)
                AddAnswerBoxes(strUnsGuid, aVerseData);

            if (TheCurrentStory.CountTestingQuestionTests > 0)
                TheCurrentStory.CountTestingQuestionTests--;    // to show we've added one

            // just in case it isn't currently showing
            viewStoryTestingQuestionAnswersMenu.Checked = true;

            Modified = true;

            // check for Dropbox copy
            TriggerDropboxCopyAnswer(StoryProject.ProjSettings.DropboxAnswers);
            return true;
        }

        private static void AddAnswerBoxes(string strUnsGuid, VerseData theVerseData)
        {
            foreach (var aTq in theVerseData.TestQuestions)
                aTq.Answers.TryAddNewLine(strUnsGuid);
        }

        internal static string InferenceTestCommentFormat
        {
            get { return Localizer.Str("Test done on: {0}"); }
        }

        private bool GetUniqueTester(TestInfo testInfo, ref string strUnsGuid)
        {
            while (String.IsNullOrEmpty(strUnsGuid))
            {
                strUnsGuid = QueryForUnsTester(StoryProject);
                if (String.IsNullOrEmpty(strUnsGuid))
                    return false;

                foreach (var ti in testInfo)
                    if (ti.MemberId == strUnsGuid)
                    {
                        DialogResult res = QueryNewUns(StoryProject.GetMemberNameFromMemberGuid(ti.MemberId));
                        if (res == DialogResult.Cancel)
                            return false;
                        strUnsGuid = null;
                        break;
                    }
            }
            return true;
        }

        public static string NewUnsMessage(string strTestIdentifier)
        {
            return String.Format(
                Localizer.Str(
                    "You can't use the same UNS for two different tests of the same story. Please select a different UNS (or just put their results in the existing boxes for test '{0}')"),
                strTestIdentifier);
        }

        private static DialogResult QueryNewUns(string strTestIdentifier)
        {
            return LocalizableMessageBox.Show(NewUnsMessage(strTestIdentifier),
                                   OseCaption,
                                   MessageBoxButtons.OKCancel);
        }

        internal bool AddSingleTestResult(TestQuestionData theTq, out LineMemberData theNewAnswer)
        {
            /* don't bother... they just misunderstand the question anyway
            // let's first just make sure that this isn't the wrong request
            DialogResult res = LocalizableMessageBox.Show(Properties.Resources.IDS_VerifyAddSingleTest,
                                               StoryEditor.OseCaption,
                                               MessageBoxButtons.YesNoCancel);

            if (res == DialogResult.No)
                return AddInferenceTest();

            if (res == DialogResult.Cancel)
                return false;
            */
            // so they want to just add a single box for this TQ's answer for a new UNS.
            // query for the UNSs that will be doing this test
            string strUnsGuid = QueryForUnsTester(StoryProject);
            if (String.IsNullOrEmpty(strUnsGuid))
            {
                theNewAnswer = null;
                return false;
            }

            theNewAnswer = theTq.Answers.TryGetValue(strUnsGuid);
            if (theNewAnswer != null)
            {
                int nIndex = TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.IndexOf(theNewAnswer.MemberId);
                QueryNewUns(String.Format(theTq.Answers.LabelTextFormat, nIndex + 1));
                return false;
            }

            // okay, so the selected UNS doesn't already have an Answer entry for this
            //  TQ. Now see if (s)he has been added to the story level list
            if (!TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Contains(strUnsGuid))
            {
                // this means that this UNS has never been added for this story. Add now
                TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Add(new MemberIdInfo(strUnsGuid, null));
            }

            // by now, the user-chosen UNS *is* in the CraftingInfo list and *isn't* in
            //  the Answer list for this particular TQ, so add it now.
            theNewAnswer = theTq.Answers.TryAddNewLine(strUnsGuid);

            // make sure the answers field is open in case it's not
            viewStoryTestingQuestionAnswersMenu.Checked = true;
            Modified = true;
            return true;
        }

        internal bool ChangeAnswerBoxUns(string strLabel, VerseBtControl theVerseCtrl)
        {
            var testQuestionData = GetTestQuestionDataFromAnswerLabel(strLabel, theVerseCtrl);
            LineMemberData theNewAnswer;
            if (!AddSingleTestResult(testQuestionData, out theNewAnswer))
                return false;

            Modified = true;

            AnswersData answers;
            var answerData = GetTqAnswerData(strLabel, theVerseCtrl, out answers);
            if (answerData != null)
            {
                // put the original data in the new box
                theNewAnswer.SetText(answerData, TextFields.TestQuestionAnswer);
                answers.Remove(answerData);

                // finally, if this is the last reference to that UNS, then remove the 
                //  reference to it
                if (!TheCurrentStory.DoesReferenceTqUns(answerData.MemberId))
                {
                    int nTestNum = TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.IndexOf(answerData.MemberId);
                    if (nTestNum >= 0)
                    {
                        TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.RemoveAt(nTestNum);

                        // since we have removed a UNS from the test, now all 'ans N' values
                        //  may change for other verses as well. So repaint all
                        InitAllPanes();
                        return false;    // and now we don't need to update this one verse only
                    }
                }
            }

            return true;
        }

        internal void ChangeAnswerBoxUns(TestQuestionData testQuestionData, AnswersData answers, LineMemberData answerData)
        {
            LineMemberData theNewAnswer;
            if (!AddSingleTestResult(testQuestionData, out theNewAnswer))
                return;

            Modified = true;
            if (answerData == null)
                return;

            // put the original data in the new box
            theNewAnswer.SetText(answerData, TextFields.TestQuestionAnswer);
            answers.Remove(answerData);

            // finally, if this is the last reference to that UNS, then remove the 
            //  reference to it
            if (!TheCurrentStory.DoesReferenceTqUns(answerData.MemberId))
            {
                var nTestNum = TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.IndexOf(answerData.MemberId);
                if (nTestNum >= 0)
                    TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.RemoveAt(nTestNum);
            }
        }

        protected string QueryForUnsTester(StoryProjectData theStoryProjectData)
        {
            string strUnsGuid = null;
            while (String.IsNullOrEmpty(strUnsGuid))
            {
                var dlg = new MemberPicker(theStoryProjectData, TeamMemberData.UserTypes.UNS)
                              {
                                  Text = Localizer.Str("Choose the UNS that gave answers for this test")
                              };
                DialogResult res = dlg.ShowDialog();
                if (res == DialogResult.OK)
                    strUnsGuid = dlg.SelectedMember.MemberGuid;
                else if (res == DialogResult.Cancel)
                    break;
            }
            return strUnsGuid;
        }

        private void OnRemoveRetellingTest(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            int nTestNum = (int)tsmi.Tag;
            if (!QueryForAndDeleteRetellingTest(nTestNum))
                return;

            Modified = true;
            InitAllPanes();
        }

        public bool QueryForAndDeleteRetellingTest(int nTestNum)
        {
            Debug.Assert((nTestNum >= 0) && (nTestNum < TheCurrentStory.CraftingInfo.TestersToCommentsRetellings.Count));
            string strUnsGuid = TheCurrentStory.CraftingInfo.TestersToCommentsRetellings[nTestNum].MemberId;
            string strTesterName = StoryProject.GetMemberNameFromMemberGuid(strUnsGuid);

            if (LocalizableMessageBox.Show(Localizer.Str("Are you sure you want to remove all of the retelling results for ") +
                                strTesterName,
                                OseCaption, MessageBoxButtons.YesNoCancel) !=
                DialogResult.Yes)
                return false;

            TheCurrentStory.DeleteRetellingTestResults(nTestNum, strUnsGuid);
            return true;
        }

        private void OnRemoveInferenceTest(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            int nTestNum = (int)tsmi.Tag;
            if (!QueryForAndDeleteAnswerTest(nTestNum))
                return;

            Modified = true;
            InitAllPanes();
        }

        public bool QueryForAndDeleteAnswerTest(int nTestNum)
        {
            Debug.Assert((nTestNum >= 0) && (nTestNum < TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Count));
            string strUnsGuid = TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers[nTestNum].MemberId;
            string strTesterName = StoryProject.GetMemberNameFromMemberGuid(strUnsGuid);

            if (LocalizableMessageBox.Show(Localizer.Str("Are you sure you want to remove all of the story question test results for ") +
                                strTesterName,
                                OseCaption, MessageBoxButtons.YesNoCancel) !=
                DialogResult.Yes)
                return false;

            TheCurrentStory.DeleteAnswerTestResults(nTestNum, strUnsGuid);
            return true;
        }

        protected void AddDeleteTestSubmenu(ToolStripMenuItem tsm, string strText, int nTestNum, EventHandler theEH)
        {
            ToolStripMenuItem tsmSub = new ToolStripMenuItem
            {
                Name = strText,
                Text = strText,
                Tag = nTestNum,
                ToolTipText = Localizer.Str("Delete the answers to the testing questions and the retellings associated with this testing helper (UNS). The text boxes will be deleted completely.")
            };
            tsmSub.Click += theEH;
            tsm.DropDown.Items.Add(tsmSub);
        }

        private void copyStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            string strStory = GetFullStoryContentsVernacular;
            PutOnClipboard(strStory);
        }

        protected string GetFullStoryContentsVernacular
        {
            get
            {
                Debug.Assert((TheCurrentStory != null)
                    && (TheCurrentStory.Verses.Count > 0));

                VerseData aVerse = TheCurrentStory.Verses[0];
                string strText = null;
                if (aVerse.IsVisible)
                    strText = aVerse.StoryLine.Vernacular.ToString();

                for (int i = 1; i < TheCurrentStory.Verses.Count; i++)
                {
                    aVerse = TheCurrentStory.Verses[i];
                    if (aVerse.IsVisible && aVerse.StoryLine.Vernacular.HasData)
                        strText += ' ' + aVerse.StoryLine.Vernacular.ToString();
                }

                return strText;
            }
        }

        protected string GetFullStoryContentsNationalBTText
        {
            get
            {
                Debug.Assert((TheCurrentStory != null)
                    && (TheCurrentStory.Verses.Count > 0));

                VerseData aVerse = TheCurrentStory.Verses[0];
                string strText = null;
                if (aVerse.IsVisible)
                    strText = aVerse.StoryLine.NationalBt.ToString();

                for (int i = 1; i < TheCurrentStory.Verses.Count; i++)
                {
                    aVerse = TheCurrentStory.Verses[i];
                    if (aVerse.IsVisible && aVerse.StoryLine.NationalBt.HasData)
                        strText += ' ' + aVerse.StoryLine.NationalBt.ToString();
                }

                return strText;
            }
        }

        protected string GetFullStoryContentsInternationalBTText
        {
            get
            {
                Debug.Assert((TheCurrentStory != null)
                    && (TheCurrentStory.Verses.Count > 0));

                VerseData aVerse = TheCurrentStory.Verses[0];
                string strText = null;
                if (aVerse.IsVisible)
                    strText = aVerse.StoryLine.InternationalBt.ToString();

                for (int i = 1; i < TheCurrentStory.Verses.Count; i++)
                {
                    aVerse = TheCurrentStory.Verses[i];
                    if (aVerse.IsVisible && aVerse.StoryLine.InternationalBt.HasData)
                        strText += ' ' + aVerse.StoryLine.InternationalBt.ToString();
                }

                return strText;
            }
        }

        protected string GetFullStoryContentsFreeTranslationText
        {
            get
            {
                Debug.Assert((TheCurrentStory != null)
                    && (TheCurrentStory.Verses.Count > 0));

                VerseData aVerse = TheCurrentStory.Verses[0];
                string strText = null;
                if (aVerse.IsVisible)
                    strText = aVerse.StoryLine.FreeTranslation.ToString();

                for (int i = 1; i < TheCurrentStory.Verses.Count; i++)
                {
                    aVerse = TheCurrentStory.Verses[i];
                    if (aVerse.IsVisible && aVerse.StoryLine.FreeTranslation.HasData)
                        strText += ' ' + aVerse.StoryLine.FreeTranslation.ToString();
                }

                return strText;
            }
        }

        protected void PutOnClipboard(string strText)
        {
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (Exception ex)
            {
                // seems to fail sometimes on Windows7. If it actually worked, then just ignore the exception
                IDataObject iData = Clipboard.GetDataObject();
                if (iData != null)
                    if (iData.GetDataPresent(DataFormats.UnicodeText))
                    {
                        string strInput = (string)iData.GetData(DataFormats.UnicodeText);
                        if (strInput == strText)
                            return;
                    }

                string strErrorMsg =
                    String.Format(Localizer.Str("Unable to copy text to the clipboard!{0}{0}{1}{0}{0}{2}"),
                                  Environment.NewLine, ex.Message,
                                  ((ex.InnerException != null) ? ex.InnerException.Message : ""));
                LocalizableMessageBox.Show(strErrorMsg, OseCaption);
            }
        }

        private void copyNationalBackTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            string strStory = GetFullStoryContentsNationalBTText;
            PutOnClipboard(strStory);
        }

        private void copyEnglishBackTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            string strStory = GetFullStoryContentsInternationalBTText;
            PutOnClipboard(strStory);
        }

        private void copyFreeTranslationMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            string strStory = GetFullStoryContentsFreeTranslationText;
            PutOnClipboard(strStory);
        }

        /*
        private void exportStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((theCurrentStory != null) && (theCurrentStory.Verses.Count > 0));

            string strStory = theCurrentStory.Verses[0].StoryLine.Vernacular.ToString();
            for (int i = 1; i < theCurrentStory.Verses.Count; i++)
            {
                VerseData aVerse = theCurrentStory.Verses[i];
                strStory += ' ' + aVerse.StoryLine.Vernacular.ToString();
            }

            GlossInAdaptIt(strStory, AdaptItGlossing.GlossType.eVernacularToEnglish);
        }
        */

        private delegate void ResetValue(VerseData verse);
        private static void DeleteVernacular(VerseData verse)
        {
            verse.StoryLine.Vernacular.SetValue(null);
        }
        private static void DeleteNationalBt(VerseData verse)
        {
            verse.StoryLine.NationalBt.SetValue(null);
        }
        private static void DeleteInternationalBt(VerseData verse)
        {
            verse.StoryLine.InternationalBt.SetValue(null);
        }
        private static void DeleteFreeTranslation(VerseData verse)
        {
            verse.StoryLine.FreeTranslation.SetValue(null);
        }
        private static void DeleteAnchors(VerseData verse)
        {
            verse.Anchors.RemoveAll();
        }
        private static void DeleteExegeticalHelps(VerseData verse)
        {
            verse.ExegeticalHelpNotes.RemoveAll();
        }
        private static void DeleteStoryTestingQuestionsAndAnswers(VerseData verse)
        {
            verse.TestQuestions.RemoveAll();
        }
        private static void DeleteConsultantNotes(VerseData verse)
        {
            verse.ConsultantNotes.RemoveAll();
        }
        private static void DeleteCoachNotes(VerseData verse)
        {
            verse.CoachNotes.RemoveAll();
        }
        private static void DeleteHiddenLines(VerseData verse)
        {
            if (!verse.IsVisible)
                verse.TestQuestions.RemoveAll();
        }

        private void DeleteFieldData(object sender, ResetValue delegateResetValue)
        {
            Debug.Assert((TheCurrentStory != null) && (sender is ToolStripItem));

            var tsi = sender as ToolStripItem;
            if (tsi != null)
                if (LocalizableMessageBox.Show(String.Format(Localizer.Str("Are you sure you want to delete the {0}?"),
                                                  tsi.Text.Replace("&", null)), OseCaption,
                                    MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    foreach (var aVerse in TheCurrentStory.Verses)
                        delegateResetValue(aVerse);
                    ReInitVerseControls();
                    Modified = true;
                }
        }

        private void deleteStoryVersesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFieldData(sender, DeleteVernacular);
        }

        private void deleteStoryNationalBackTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFieldData(sender, DeleteNationalBt);
        }

        private void deleteEnglishBacktranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFieldData(sender, DeleteInternationalBt);
        }

        private void deleteFreeTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFieldData(sender, DeleteFreeTranslation);
        }

        protected void GlossInAdaptIt(string strStoryText, ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection)
        {
            ProjectSettings.LanguageInfo liSourceLang, liTargetLang;
            AdaptItEncConverter theEC;
            string strAdaptationFilespec = null;
            try
            {
                theEC = AdaptItGlossing.InitLookupAdapter(StoryProject.ProjSettings,
                                                          eBtDirection, LoggedOnMember,
                                                          out liSourceLang, out liTargetLang);

                strAdaptationFilespec = AdaptationFilespec(theEC.ConverterIdentifier, TheCurrentStory.Name);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }

            string strProjectName =
                AdaptItGlossing.GetAiProjectFolderNameFromConverterIdentifier(theEC.ConverterIdentifier);

            DialogResult res = DialogResult.Yes;
            if (File.Exists(strAdaptationFilespec))
            {
                res =
                    LocalizableMessageBox.Show(
                        String.Format(
                            Localizer.Str(
                                "The Adapt It file for the project '{0}', story '{1}' already exists. If you want to delete that file and do the English translation of the story all over again, then click 'Yes'. If you just want to try to import the existing file again, choose 'No'. Otherwise, click 'Cancel' to stop this command."),
                            strProjectName, TheCurrentStory.Name), OseCaption,
                        MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Cancel)
                    return;

                if (res == DialogResult.Yes)
                {
                    string strBackupName = strAdaptationFilespec + ".bak";
                    if (File.Exists(strBackupName))
                        File.Delete(strBackupName);
                    File.Copy(strAdaptationFilespec, strBackupName);
                    File.Delete(strAdaptationFilespec);
                }
            }

            if (res == DialogResult.Yes)
            {
                List<string> SourceWords;
                List<string> TargetWords;
                List<string> SourceStringsInBetween;
                List<string> TargetStringsInBetween;
                theEC.SplitAndConvert(strStoryText, out SourceWords, out SourceStringsInBetween,
                    out TargetWords, out TargetStringsInBetween);
                Debug.Assert((SourceWords.Count == TargetWords.Count)
                    && (SourceWords.Count == (SourceStringsInBetween.Count - 1))
                    && (SourceStringsInBetween.Count == TargetStringsInBetween.Count));

                string strSourceSentFinalPunct = liSourceLang.FullStop;
                Debug.Assert(!String.IsNullOrEmpty(strSourceSentFinalPunct));

                XElement elem = new XElement("AdaptItDoc");
                for (int i = 0; i < SourceWords.Count; i++)
                {
                    // the SplitAndConvert routine will actually join multi-word segments into a phrase, but for 
                    //  initializing the adaptation file, we really don't want that, so split them back into 
                    //  single word bundles.
                    string strSourceWord = SourceWords[i];
                    string[] astrSourcePhraseWords = strSourceWord.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (astrSourcePhraseWords.Length > 1)
                    {
                        strSourceWord = SourceWords[i] = astrSourcePhraseWords[0];
                        for (int j = 1; j < astrSourcePhraseWords.Length; j++)
                        {
                            string str = astrSourcePhraseWords[j];
                            SourceWords.Insert(i + j, str);
                            SourceStringsInBetween.Insert(i, "");
                            TargetStringsInBetween.Insert(i, "");
                        }
                    }

                    // if the stuff in between is ", ", then clearly it wants to go on the end
                    // but if it's like " (", then it wants to go on the next word
                    string strBefore = SourceStringsInBetween[i].Trim(), strAfter = SourceStringsInBetween[i + 1];
                    if (!String.IsNullOrEmpty(strAfter))
                    {
                        Debug.Assert(strAfter.Length > 0);  // should at least be a space
                        int nIndexOfSpace = strAfter.IndexOf(' ');
                        if (nIndexOfSpace != -1)
                        {
                            string strBeforeNextWord = (nIndexOfSpace < strAfter.Length) ?
                                strAfter.Substring(nIndexOfSpace) : null;
                            strAfter = (nIndexOfSpace > 0) ? strAfter.Substring(0, nIndexOfSpace).Trim() : null;
                            SourceStringsInBetween[i + 1] = strBeforeNextWord;
                        }
                    }

                    string strFattr = AIBools(strSourceWord, strAfter, strSourceSentFinalPunct);

                    XElement elemWord =
                        new XElement("S",
                            new XAttribute("s", strBefore + strSourceWord + strAfter),
                            new XAttribute("k", strSourceWord),
                            new XAttribute("f", strFattr),
                            new XAttribute("sn", i),
                            new XAttribute("w", 1),
                            new XAttribute("ty", 1));

                    if (!String.IsNullOrEmpty(strBefore))
                        elemWord.Add(new XAttribute("pp", strBefore.Trim()));

                    if (!String.IsNullOrEmpty(strAfter))
                        elemWord.Add(new XAttribute("fp", strAfter.Trim()));

                    elemWord.Add("");

                    elem.Add(elemWord);
                }

                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    elem);

                doc.Save(strAdaptationFilespec);

                string strAdaptIt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Path.Combine("Adapt It WX Unicode", "Adapt_It_Unicode.exe"));

                // if AdaptIt is currently running, then close it first (so we can start
                //  it in "force review mode"
                foreach (Process aProcess in Process.GetProcesses())
                    if (aProcess.ProcessName == "Adapt_It_Unicode")
                        ReleaseProcess(aProcess);

                // todo: this probably needs to be selectable
                LaunchProgram(strAdaptIt,
                    // (eGlossType == GlossType.eNationalToEnglish) ?
                    null
                    // : "/frm"
                    );

                // indicate that we've called AI so we can be sure to read/write the 
                //  kb file in a way that facilitates minimal diffs
                Program.HaveCalledAdaptIt = true;

                res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Follow these steps to use the Adapt It program to translate the story from {1} into {2}:{0}{0}1) Switch to the Adapt It program using Alt+Tab keys{0}2) Select the '{3}' project and click the 'Next' button (if a file is already open in Adapt It, then choose the 'File' menu, 'Close Project' command and then the 'File' menu, 'Start Working' command first).{0}3) Select the '{4}.xls' document to open and press the 'Finished' button.{0}4) When you see the {1} in the adaptation window, then translate it into {2}. {0}5) When you're finished, return to this window and click the 'Yes' button to import the back-translated {2} text into the {2} fields.{0}{0}Have you finished back-translating the {1} to {2} in Adapt It, and are ready to import it back into the OneStory Editor?"),
                                                    Environment.NewLine, liSourceLang.LangName,
                                                    liTargetLang.LangName, strProjectName,
                                                    TheCurrentStory.Name),
                                      OseCaption, MessageBoxButtons.YesNoCancel);

                if (res != DialogResult.Yes)
                    return;

                // do a dummy conversion to trigger a reload of the KB now that we've surely
                //  added things.
                theEC.Convert("dummy");
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(strAdaptationFilespec);
                XPathNavigator navigator = doc.CreateNavigator();

                XPathNodeIterator xpIterator = navigator.Select("/AdaptItDoc/S");

                for (int nVerseNum = 0; nVerseNum < TheCurrentStory.Verses.Count; nVerseNum++)
                {
                    VerseData aVerse = TheCurrentStory.Verses[nVerseNum];
                    string strStoryVerse = (eBtDirection == ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt)
                                               ? aVerse.StoryLine.NationalBt.ToString()
                                               : aVerse.StoryLine.Vernacular.ToString();
                    if (String.IsNullOrEmpty(strStoryVerse))
                        continue;

                    List<string> TargetWords;
                    List<string> SourceWords;
                    List<string> SourceStringsInBetween;
                    List<string> TargetStringsInBetween;
                    theEC.SplitAndConvert(strStoryVerse, out SourceWords, out SourceStringsInBetween,
                        out TargetWords, out TargetStringsInBetween);
                    Debug.Assert((SourceWords.Count == TargetWords.Count)
                                 && (SourceWords.Count == (SourceStringsInBetween.Count - 1))
                                 && (SourceStringsInBetween.Count == TargetStringsInBetween.Count));

                    string strTargetBT = null;
                    for (int nWordNum = 0; nWordNum < SourceWords.Count; nWordNum++)
                    {
                        string strSourceWord = SourceWords[nWordNum];
                        string strTargetWord = TargetWords[nWordNum];

                        if (xpIterator.MoveNext())
                        {
                            string strSourceKey = xpIterator.Current.GetAttribute("k", navigator.NamespaceURI);
                            if (strSourceKey != strSourceWord)
                                ThrowAdaptationException(liSourceLang.LangName,
                                                         liTargetLang.LangName,
                                                         nVerseNum,
                                                         strSourceKey,
                                                         strSourceWord);

                            string strTargetKey = xpIterator.Current.GetAttribute("a", navigator.NamespaceURI);
                            if ((strTargetWord.IndexOf('%') == -1) && (strTargetWord != strTargetKey))
                                ThrowAdaptationException(liSourceLang.LangName,
                                                         liTargetLang.LangName,
                                                         nVerseNum,
                                                         strTargetKey,
                                                         strTargetWord);

                            strTargetWord = xpIterator.Current.GetAttribute("t", navigator.NamespaceURI);
                            strTargetBT += strTargetWord + ' ';
                        }
                    }

                    if (!String.IsNullOrEmpty(strTargetBT))
                        strTargetBT = strTargetBT.Remove(strTargetBT.Length - 1);

                    if (eBtDirection == ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt)
                        aVerse.StoryLine.NationalBt.SetValue(strTargetBT);
                    else
                        aVerse.StoryLine.InternationalBt.SetValue(strTargetBT);
                }

                Modified = true;
                if ((eBtDirection == ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt)
                    && !viewNationalLangMenu.Checked)
                {
                    viewNationalLangMenu.Checked = true;
                }
                else if ((eBtDirection != ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt)
                    && !viewEnglishBtMenu.Checked)
                {
                    viewEnglishBtMenu.Checked = true;
                }
                else
                    ReInitVerseControls();
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, OseCaption);
            }
        }

        private static void ThrowAdaptationException(string strSourceLanguageName,
            string strTargetLanguageName, int nVerseNum, string strKey, string strWord)
        {
            throw new ApplicationException(String.Format(Localizer.Str("The data from Adapt It doesn't appear to match what's in the story.{0}In line number {3}, I was expecting \"{4}\", but got \"{5}\" instead.{0}If you've made changes in the {1} or the {2} back-translation, then you need to do the back-translation in Adapt It again."),
                                                         Environment.NewLine, strSourceLanguageName,
                                                         strTargetLanguageName, nVerseNum + 1, strKey,
                                                         strWord));
        }

        private static void ReleaseProcess(Process aProcess)
        {
            try
            {
                aProcess.CloseMainWindow();
                aProcess.Close();
                int nloop = 0;
                while (!aProcess.HasExited && (nloop++ < 5))
                {
                    Thread.Sleep(2000);
                }
            }
            catch   // sometimes this throws up, but I don't care
            {
            }
        }

        internal static void LaunchProgram(string strProgram, string strArguments)
        {
            try
            {
                Process myProcess = new Process
                                        {
                                            StartInfo =
                                                {
                                                    FileName = strProgram,
                                                    Arguments = strArguments,
                                                    WindowStyle = ProcessWindowStyle.Minimized
                                                }
                                        };
                myProcess.Start();
                Thread.Sleep(2000);
            }
            catch { }    // we tried...
        }

        // from AdaptIt baseline XML.h
        const UInt32 boundaryMask = 32; // position 6
        const UInt32 paragraphMask = 2097152; // position 22

        protected string AIBools(string strSourceWord, string strAfter, string strFullStop)
        {
            UInt32 value = 0;
            if (!String.IsNullOrEmpty(strAfter) && (strFullStop.IndexOf(strAfter) != -1))
                value |= boundaryMask;
            if (strSourceWord.IndexOf(Environment.NewLine) != -1)
                value |= paragraphMask;

            string strValue = String.Format("{1}000000000000000{0}00000",
                (((value & boundaryMask) == boundaryMask) ? "1" : "0"),
                (((value & paragraphMask) == paragraphMask) ? "1" : "0"));

            return strValue;
        }

        protected string AdaptationFilespec(string strConverterFilespec, string strStoryName)
        {
            var path = Path.Combine(AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(strConverterFilespec),
                       Path.Combine("Adaptations", String.Format(@"{0}.xml", strStoryName)));

            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (path.IndexOfAny(invalidChars) != -1)
            {
                throw new ApplicationException(Localizer.Str("The story name contains characters which won't work in AdaptIt, so you'll have to change the name of the story to remove any of these characters: ") + String.Join(", ", invalidChars));
            }

            return path;
        }

        private void UpdateUiMenusAfterProjectOpen()
        {
            Debug.Assert(!_bDisableReInitVerseControls);
            _bDisableReInitVerseControls = true;
            viewVernacularLangMenu.Checked =
                viewVernacularLangMenu.Visible = StoryProject.ProjSettings.Vernacular.HasData;
            viewNationalLangMenu.Checked =
                viewNationalLangMenu.Visible = StoryProject.ProjSettings.NationalBT.HasData;
            viewEnglishBtMenu.Checked =
                viewEnglishBtMenu.Visible = StoryProject.ProjSettings.InternationalBT.HasData;
            viewFreeTranslationMenu.Checked =
                viewFreeTranslationMenu.Visible = StoryProject.ProjSettings.FreeTranslation.HasData;
            // viewStoriesSetMenu.Checked = false;
            UpdateUIMenusWithShortCuts();
            _bDisableReInitVerseControls = false;
        }

        protected void UpdateUIMenusWithShortCuts()
        {
            viewRefreshMenu.Enabled = (TheCurrentStory != null);

            editFindMenu.Enabled =
                editFindNextMenu.Enabled =
                editReplaceMenu.Enabled =
                    ((StoryProject != null)
                    && (StoryProject.ProjSettings != null)
                    && (TheCurrentStory != null)
                    && (TheCurrentStory.Verses.Count > 0));

            if ((StoryProject == null) || (StoryProject.ProjSettings == null))
                return;

            /* this shouldn't be done, because it's not taking into consideration whether
             * the user has turned off the field for any particular state. We should only
             * be doing this in StoryStageLogic.StateTransition.SetView
            viewVernacularLangMenu.Checked =
                viewVernacularLangMenu.Visible = StoryProject.ProjSettings.Vernacular.HasData;
            viewNationalLangMenu.Checked =
                viewNationalLangMenu.Visible = StoryProject.ProjSettings.NationalBT.HasData;
            viewEnglishBtMenu.Checked =
                viewEnglishBtMenu.Visible = StoryProject.ProjSettings.InternationalBT.HasData;
            viewFreeTranslationMenu.Checked =
                viewFreeTranslationMenu.Visible = StoryProject.ProjSettings.FreeTranslation.HasData;
            */
        }

        private void showHideFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new ViewEnableForm(this, StoryProject.ProjSettings, TheCurrentStory,
                                         viewUseSameSettingsForAllStoriesMenu.Checked)
                          {
                              ViewSettings = CurrentViewSettings
                          };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetViewSettings(dlg.ViewSettings, dlg.UseForAllStories);
            }
        }

        protected virtual void SetViewSettings(VerseData.ViewSettings viewSettings, bool bUseForAllStories)
        {
            // have to turn this off, or these new settings won't work
            viewUseSameSettingsForAllStoriesMenu.Checked = false;

            if (UsingHtmlForStoryBtPane)
                NavigateTo(TheCurrentStory.Name, viewSettings, true, HtmlStoryBtControl.LastTextareaInFocusId);
            else
                NavigateTo(TheCurrentStory.Name, viewSettings, true, CtrlTextBox._inTextBox);

            viewUseSameSettingsForAllStoriesMenu.Checked = bUseForAllStories;
        }

        private static readonly List<string> ListDoneWarnedAlready = new List<string>();
        public static void WarnAboutNoLangsVisible(string strFieldName)
        {
            // only warn the user once for each type
            if (ListDoneWarnedAlready.Contains(strFieldName))
                return;

            ListDoneWarnedAlready.Add(strFieldName);

            LocalizableMessageBox.Show(
                String.Format(
                    Localizer.Str(
                        "None of the currently visible languages are configured for {0}! See the 'Languages' tab in the 'Project', 'Settings' dialog OR make other language(s) visible in the 'View' menu"),
                    strFieldName),
                OseCaption);
        }

        internal VerseData.ViewSettings CurrentViewSettings
        {
            get
            {
                return new VerseData.ViewSettings
                    (
                    StoryProject.ProjSettings,
                    viewVernacularLangMenu.Checked,
                    viewNationalLangMenu.Checked,
                    viewEnglishBtMenu.Checked,
                    viewFreeTranslationMenu.Checked,
                    viewAnchorsMenu.Checked,
                    viewExegeticalHelps.Checked,
                    viewStoryTestingQuestionsMenu.Checked,
                    viewStoryTestingQuestionAnswersMenu.Checked,
                    viewRetellingsMenu.Checked,
                    viewConsultantNotesMenu.Checked,
                    viewCoachNotesMenu.Checked,
                    viewBibleMenu.Checked,
                    false,
                    viewHiddenVersesMenu.Checked,
                    viewOnlyOpenConversationsMenu.Checked,
                    viewGeneralTestingsQuestionMenu.Checked,
                    true,   // use textareas
                    CurrentFieldEditability(TheCurrentStory),
                    (UsingHtmlForStoryBtPane)
                        ? HtmlStoryBtControl.TransliteratorVernacular
                        : VerseBtControl.TransliteratorVernacular,
                    (UsingHtmlForStoryBtPane)
                        ? HtmlStoryBtControl.TransliteratorNationalBt
                        : VerseBtControl.TransliteratorNationalBt,
                    (UsingHtmlForStoryBtPane)
                        ? HtmlStoryBtControl.TransliteratorInternationalBt
                        : VerseBtControl.TransliteratorInternationalBt,
                    (UsingHtmlForStoryBtPane)
                        ? HtmlStoryBtControl.TransliteratorFreeTranslation
                        : VerseBtControl.TransliteratorFreeTranslation);
            }
        }

        internal TextFields CurrentFieldEditability(StoryData theCurrentStory)
        {
            TextFields fields = (CheckForProperEditToken() && !TeamMemberData.IsUser(LoggedOnMember.MemberType, TeamMemberData.UserTypes.JustLooking))
                                    ? TextFields.Everything
                                    : TextFields.Undefined;

            // the PF, though, might be restricted from editing certain fields by the Consultant
            if ((LoggedOnMember.MemberType == TeamMemberData.UserTypes.ProjectFacilitator) &&
                (LoggedOnMember.MemberGuid == theCurrentStory.CraftingInfo.ProjectFacilitator.MemberId))
            {
                fields = TasksPf.FilterTextFields(fields, theCurrentStory.TasksAllowedPf);
            }
            return fields;
        }

        private const string CstrAddNewStorySetMenuText = "<&Add new story set>";

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateUIMenusWithShortCuts();

            if ((StoryProject != null) && (StoryProject.ProjSettings != null))
            {
                viewShowHideFieldsMenu.Enabled =
                    viewHistoricalDifferencesMenu.Enabled =
                    viewHiddenVersesMenu.Enabled =
                    viewOnlyOpenConversationsMenu.Enabled = (TheCurrentStory != null);

                if (StoryProject.ProjSettings.Vernacular.HasData)
                {
                    viewVernacularLangMenu.Text = String.Format(Localizer.Str("{0} &language fields"),
                                                                StoryProject.ProjSettings.Vernacular.LangName);
                    viewVernacularLangMenu.Enabled = (TheCurrentStory != null);
                }
                else
                    viewVernacularLangMenu.Checked =
                        viewVernacularLangMenu.Visible = false;

                if (StoryProject.ProjSettings.NationalBT.HasData)
                {
                    viewNationalLangMenu.Text = String.Format(Localizer.Str("&{0} back-translation fields"),
                                                              StoryProject.ProjSettings.NationalBT.LangName);

                    viewNationalLangMenu.Enabled = (TheCurrentStory != null);
                }
                else
                    viewNationalLangMenu.Checked =
                        viewNationalLangMenu.Visible = false;

                if (StoryProject.ProjSettings.InternationalBT.HasData)
                    viewEnglishBtMenu.Enabled = (TheCurrentStory != null);
                else
                    viewEnglishBtMenu.Checked =
                        viewEnglishBtMenu.Visible = false;

                if (StoryProject.ProjSettings.FreeTranslation.HasData)
                    viewFreeTranslationMenu.Enabled = (TheCurrentStory != null);
                else
                    viewFreeTranslationMenu.Checked =
                        viewFreeTranslationMenu.Visible = false;

                viewConsultantNotesMenu.Enabled =
                    viewExegeticalHelps.Enabled =
                    viewStateTransitionHistoryMenu.Enabled = (TheCurrentStory != null);

                // this have the added requirement that it be a biblical story
                viewAnchorsMenu.Enabled =
                    (TheCurrentStory != null) && TheCurrentStory.CraftingInfo.IsBiblicalStory;

                viewRetellingsMenu.Enabled =
                    viewRetellingsChooseWhichRetellingsItem.Enabled =
                    (TheCurrentStory != null) && TheCurrentStory.CraftingInfo.IsBiblicalStory && StoryProject.ProjSettings.ShowRetellings.Configured;

                viewGeneralTestingsQuestionMenu.Enabled =
                    viewStoryTestingQuestionsMenu.Enabled =
                    (TheCurrentStory != null) && TheCurrentStory.CraftingInfo.IsBiblicalStory && StoryProject.ProjSettings.ShowTestQuestions.Configured;

                viewStoryTestingQuestionAnswersMenu.Enabled =
                    viewAnswersChooseWhichAnswersItem.Enabled =
                    (TheCurrentStory != null) && TheCurrentStory.CraftingInfo.IsBiblicalStory && StoryProject.ProjSettings.ShowAnswers.Configured;

                viewTransliterationsToolStripMenu.Enabled = (StoryProject.ProjSettings.Vernacular.HasData ||
                                                                 StoryProject.ProjSettings.NationalBT.HasData ||
                                                                 StoryProject.ProjSettings.InternationalBT.HasData ||
                                                                 StoryProject.ProjSettings.FreeTranslation.HasData);

                viewLnCNotesMenu.Enabled =
                    viewConcordanceMenu.Enabled = true;

                // the coach pane shouldn't be visible except to Consultants and Coach
                //  (here we want to use "!=" for PF, because if he's also a LSR, then
                //  we want to be able to see Coach note pane).
                viewCoachNotesMenu.Enabled = ((LoggedOnMember != null) &&
                                              !LoggedOnMember.IsPfAndNotLsr);

                viewProjectNotesMenu.Enabled = (Directory.Exists(StoryProject.ProjSettings.ProjectFolder));
            }
            else
                viewShowHideFieldsMenu.Enabled =
                    viewTransliterationsToolStripMenu.Enabled =
                    viewStateTransitionHistoryMenu.Enabled =
                    viewLnCNotesMenu.Enabled =
                    viewConcordanceMenu.Enabled =
                    viewHistoricalDifferencesMenu.Enabled =
                    viewHiddenVersesMenu.Enabled =
                    viewOnlyOpenConversationsMenu.Enabled =
                    viewProjectNotesMenu.Enabled = false;

            if (StoryProject != null)
            {
                InitializeViewStorySetNameDropDown();
            }
        }

        private void InitializeViewStorySetNameDropDown()
        {
            viewStoriesSetMenu.DropDownItems.Clear();
            foreach (var storiesData in StoryProject.Values)
            {
                AddMenuItem(storiesData.SetName);
            }
            viewStoriesSetMenu.DropDownItems.Add(new ToolStripSeparator { Name = "separateme" });
            AddMenuItem(CstrAddNewStorySetMenuText);
        }

        private void AddMenuItem(string setName)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)viewStoriesSetMenu.DropDownItems.Add(setName, null, onClickStorySet);
            tsi.CheckOnClick = true;
            if (CurrentStoriesSetName == setName)
                tsi.CheckState = CheckState.Checked;
        }

        private void onClickStorySet(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripItem;
            var strStoriesSetName = tsi.Text;
            SwitchToNewStoriesSet(strStoriesSetName);
        }

        private void SwitchToNewStoriesSet(string strStoriesSetName)
        {
            CheckForSaveDirtyFile();

            // query for new set name:
            if (strStoriesSetName == CstrAddNewStorySetMenuText)
            {
                // ask the user for what they want to call the new stories set
                strStoriesSetName =
                    LocalizableMessageBox.InputBox(Localizer.Str("Enter the name of the new story set to add"),
                                                   OseCaption, "New Panorama Name");
                if (String.IsNullOrEmpty(strStoriesSetName))
                    return;

                if (StoryProject.ContainsKey(strStoriesSetName))
                {
                    LocalizableMessageBox.Show(
                        Localizer.Str(
                            String.Format("A story set by the name '{0}' already exists. Choose a different name", strStoriesSetName)),
                        OseCaption);
                    return;
                }

                StoryProject.Add(strStoriesSetName, new StoriesData(strStoriesSetName));
                InitializeViewStorySetNameDropDown();
                Modified = true;
            }

            InitializeCurrentStoriesSetName(strStoriesSetName);

            Program.MapProjectNameToLastStoriesSetWorkedOn[StoryProject.ProjSettings.ProjectName] = CurrentStoriesSetName;
            Settings.Default.ProjectNameToLastStoriesSetWorkedOn = Program.DictionaryToArray(Program.MapProjectNameToLastStoriesSetWorkedOn);
            Settings.Default.Save();

            LoadComboBox();

            if (comboBoxStorySelector.Items.Count > 0)
                comboBoxStorySelector.SelectedIndex = 0;
            else
                ClearState();

            Text = GetFrameTitle(true);

            InitAllPanes();
        }

        /*
        private StoryEditor LaunchOldStoryWindow(string strStoryName)
        {
            var theOldStoryEditor = new StoryEditor(Resources.IDS_ObsoleteStoriesSet, null)
                                        {
                                            StoryProject = StoryProject,
                                            LoggedOnMember = LoggedOnMember
                                        };
            theOldStoryEditor.Show();
            theOldStoryEditor.LoadComboBox();
            theOldStoryEditor.JumpToStory(strStoryName);
            return theOldStoryEditor;
        }
        */

        internal void editCopySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strText = null;
            if (UsingHtmlForStoryBtPane)
                strText = htmlStoryBtControl.GetSelectedText;
            else if (CtrlTextBox._inTextBox != null)
                strText = CtrlTextBox._inTextBox.SelectedText;
            if (!String.IsNullOrEmpty(strText))
                Clipboard.SetDataObject(strText);
        }

        internal void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForProperEditToken())
                return;

            if (UsingHtmlForStoryBtPane)
            {
                var st = htmlStoryBtControl.GetStringTransferOfLastTextAreaInFocus;
                if ((st == null) || !htmlStoryBtControl.CheckShowErrorOnFieldNotEditable(st))
                    return;

                IDataObject iData = Clipboard.GetDataObject();
                if (iData != null)
                    if (iData.GetDataPresent(DataFormats.UnicodeText))
                    {
                        int nNewEndPoint;
                        var strText = (string)iData.GetData(DataFormats.UnicodeText);
                        htmlStoryBtControl.SetSelectedText(st, strText, out nNewEndPoint);
                    }
            }
            else
            {
                if (CtrlTextBox._inTextBox != null)
                {
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData != null)
                        if (iData.GetDataPresent(DataFormats.UnicodeText))
                        {
                            string strText = (string)iData.GetData(DataFormats.UnicodeText);
                            CtrlTextBox._inTextBox.SelectedText = strText;
                        }
                }
            }
        }

        protected List<string> GetSentencesVernacular(VerseData aVerseData)
        {
            string strSentenceFinalPunct = StoryProject.ProjSettings.Vernacular.FullStop;
            List<string> lstSentences;
            CheckEndOfStateTransition.GetListOfSentences(aVerseData.StoryLine.Vernacular, strSentenceFinalPunct, out lstSentences);
            return lstSentences;
        }

        protected List<string> GetSentencesNationalBT(VerseData aVerseData)
        {
            string strSentenceFinalPunct = StoryProject.ProjSettings.NationalBT.FullStop;
            List<string> lstSentences;
            CheckEndOfStateTransition.GetListOfSentences(aVerseData.StoryLine.NationalBt, strSentenceFinalPunct, out lstSentences);
            return lstSentences;
        }

        protected List<string> GetSentencesEnglishBT(VerseData aVerseData)
        {
            string strSentenceFinalPunct = StoryProject.ProjSettings.InternationalBT.FullStop;
            List<string> lstSentences;
            CheckEndOfStateTransition.GetListOfSentences(aVerseData.StoryLine.InternationalBt, strSentenceFinalPunct, out lstSentences);
            return lstSentences;
        }

        protected List<string> GetSentencesFreeTranslation(VerseData aVerseData)
        {
            string strSentenceFinalPunct = StoryProject.ProjSettings.FreeTranslation.FullStop;
            List<string> lstSentences;
            CheckEndOfStateTransition.GetListOfSentences(aVerseData.StoryLine.FreeTranslation, strSentenceFinalPunct, out lstSentences);
            return lstSentences;
        }

        protected string GetSentence(int nIndex, List<string> lstSentences)
        {
            if ((lstSentences != null) && (lstSentences.Count > nIndex))
                return lstSentences[nIndex];
            return null;
        }

        private void realignStoryVersesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RealignLines()) 
                return;
            InitAllPanes();
        }

        internal bool RealignLines()
        {
            if (((TheCurrentStory == null) || (TheCurrentStory.Verses.Count == 0)) || !CheckForLostData())
                return true;

            // first 'collapse into 1 line'
            string strVernacular = GetFullStoryContentsVernacular;
            string strNationalBT = GetFullStoryContentsNationalBTText;
            string strEnglishBT = GetFullStoryContentsInternationalBTText;
            string strFreeTranslation = GetFullStoryContentsFreeTranslationText;

            TheCurrentStory.Verses.RemoveRange(0, TheCurrentStory.Verses.Count);
            TheCurrentStory.Verses.InsertVerse(0, strVernacular, strNationalBT,
                                               strEnglishBT, strFreeTranslation);

            // then split into lines
            VerseData aVerseData = TheCurrentStory.Verses[0];
            List<string> lstSentencesVernacular = GetSentencesVernacular(aVerseData);
            List<string> lstSentencesNationalBT = GetSentencesNationalBT(aVerseData);
            List<string> lstSentencesEnglishBT = GetSentencesEnglishBT(aVerseData);
            List<string> lstSentencesFreeTranslation = GetSentencesFreeTranslation(aVerseData);

            int nNumVerses = Math.Max(lstSentencesVernacular.Count,
                                      Math.Max(lstSentencesNationalBT.Count,
                                               Math.Max(lstSentencesEnglishBT.Count,
                                                        lstSentencesFreeTranslation.Count)));

            // remove what's there so we can add the new ones from scratch
            TheCurrentStory.Verses.Remove(aVerseData);

            for (int i = 0; i < nNumVerses; i++)
            {
                TheCurrentStory.Verses.InsertVerse(i,
                                                   GetSentence(i, lstSentencesVernacular),
                                                   GetSentence(i, lstSentencesNationalBT),
                                                   GetSentence(i, lstSentencesEnglishBT),
                                                   GetSentence(i, lstSentencesFreeTranslation));
            }

            Modified = true;
            return false;
        }

        private bool CheckForLostData()
        {
            if (WillBeLossInVerse(TheCurrentStory.Verses))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "OSE can't automatically rearrange the story lines now because that would result in the loss of other information (e.g. Anchors, Testing Questions, and/or Consultant Notes). If you delete these other items, then you could use this command again"),
                    OseCaption);
                return false;
            }

            return CheckForSaveDirtyFile();
        }

        private void splitIntoLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert((TheCurrentStory != null)
                && (TheCurrentStory.Verses.Count > 0));

            if (!CheckForLostData())
                return;

            if (TheCurrentStory.Verses.Count == 1)
            {
                // means 'split into lines'
                VerseData aVerseData = TheCurrentStory.Verses[0];
                List<string> lstSentencesVernacular = GetSentencesVernacular(aVerseData);
                List<string> lstSentencesNationalBT = GetSentencesNationalBT(aVerseData);
                List<string> lstSentencesEnglishBT = GetSentencesEnglishBT(aVerseData);
                List<string> lstSentencesFreeTranslation = GetSentencesFreeTranslation(aVerseData);

                int nNumVerses = Math.Max(lstSentencesVernacular.Count,
                                          Math.Max(lstSentencesNationalBT.Count,
                                                   Math.Max(lstSentencesEnglishBT.Count,
                                                            lstSentencesFreeTranslation.Count)));

                // remove what's there so we can add the new ones from scratch
                TheCurrentStory.Verses.Remove(aVerseData);

                for (int i = 0; i < nNumVerses; i++)
                {
                    TheCurrentStory.Verses.InsertVerse(i,
                        GetSentence(i, lstSentencesVernacular),
                        GetSentence(i, lstSentencesNationalBT),
                        GetSentence(i, lstSentencesEnglishBT),
                        GetSentence(i, lstSentencesFreeTranslation));
                }
            }
            else
            {
                // means 'collapse into 1 line'
                string strVernacular = GetFullStoryContentsVernacular;
                string strNationalBT = GetFullStoryContentsNationalBTText;
                string strEnglishBT = GetFullStoryContentsInternationalBTText;
                string strFreeTranslation = GetFullStoryContentsFreeTranslationText;

                TheCurrentStory.Verses.RemoveRange(0, TheCurrentStory.Verses.Count);
                TheCurrentStory.Verses.InsertVerse(0, strVernacular, strNationalBT,
                    strEnglishBT, strFreeTranslation);
            }

            Modified = true;
            InitAllPanes();
        }

        public static bool WillBeLossInVerse(VersesData theVerses)
        {
            return theVerses.Any(aVerse =>
                                 aVerse.Anchors.HasData ||
                                 aVerse.ExegeticalHelpNotes.HasData ||
                                 aVerse.Retellings.HasData ||
                                 aVerse.ConsultantNotes.HasData ||
                                 aVerse.CoachNotes.HasData ||
                                 aVerse.TestQuestions.HasData);
        }

        public void NavigateTo(string strStorySet, string strStoryName, 
            int nLineIndex, string strAnchor)
        {
            // if it's the main set or the non-biblical story set, then make
            //  sure it's visible. If it's the obsolete story set, then launch
            //  it in another window (null means 'current set')
            if (!String.IsNullOrEmpty(strStorySet) &&
                (strStorySet != CurrentStoriesSetName))
            {
                SwitchToNewStoriesSet(strStorySet);
            }

            Debug.Assert(comboBoxStorySelector.Items.Contains(strStoryName));
            JumpToStory(strStoryName);
            if (strStoryName != TheCurrentStory.Name)
                return; // must have cancelled

            NavigateToSetFocus(strAnchor, nLineIndex);
        }

        private void NavigateToSetFocus(string strAnchor, int nLineIndex)
        {
            if (!String.IsNullOrEmpty(strAnchor))
                SetNetBibleVerse(strAnchor);
            Debug.Assert(TheCurrentStory.Verses.Count >= nLineIndex);
            FocusOnVerse(nLineIndex, true, true);
        }

        public void NavigateTo(string strStoryName,
            VerseData.ViewSettings viewItemToInsureOn,
            bool bDoOffToo)
        {
            Debug.Assert(comboBoxStorySelector.Items.Contains(strStoryName));
            if (strStoryName != TheCurrentStory.Name)
                JumpToStory(strStoryName);

            bool bSomethingChanged = false;
            _bDisableReInitVerseControls = true;
            bSomethingChanged |= InsureVisible(viewVernacularLangMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.VernacularLangField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewTransliterationVernacular,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.VernacularTransliterationField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewNationalLangMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewTransliterationNationalBT,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.NationalBtTransliterationField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewEnglishBtMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.InternationalBtField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewTransliterationInternationalBt,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.InternationalBtTransliterationField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewFreeTranslationMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewTransliterationFreeTranslation,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.FreeTranslationTransliterationField),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewAnchorsMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.AnchorFields),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewExegeticalHelps,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewGeneralTestingsQuestionMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewStoryTestingQuestionsMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewStoryTestingQuestionAnswersMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewRetellingsMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.RetellingFields),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewConsultantNotesMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewCoachNotesMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields),
                                               bDoOffToo);
            bSomethingChanged |= InsureVisible(viewBibleMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.BibleViewer),
                                               bDoOffToo);

            bSomethingChanged |= InsureVisible(viewHiddenVersesMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.HiddenStuff),
                                               bDoOffToo);

            bSomethingChanged |= InsureVisible(viewOnlyOpenConversationsMenu,
                                               viewItemToInsureOn.IsViewItemOn(
                                                   VerseData.ViewSettings.ItemToInsureOn.OpenConNotesOnly),
                                               bDoOffToo);

            _bDisableReInitVerseControls = false;

            if (bSomethingChanged)
                ReInitVerseControls();
        }

        public void NavigateTo(string strStoryName,
                               VerseData.ViewSettings viewItemToInsureOn, bool bDoOffToo,
                               string strTextareaToFocus)
        {
            NavigateTo(strStoryName, viewItemToInsureOn, bDoOffToo);
            if (!String.IsNullOrEmpty(strTextareaToFocus))
                htmlStoryBtControl.ScrollToElement(strTextareaToFocus, false);
        }

        public void NavigateTo(string strStoryName,
                               VerseData.ViewSettings viewItemToInsureOn, bool bDoOffToo,
                               CtrlTextBox ctbToFocus)
        {
            NavigateTo(strStoryName, viewItemToInsureOn, bDoOffToo);
            if (ctbToFocus != null)
                ctbToFocus.Focus();
        }

        protected bool InsureVisible(ToolStripMenuItem tsmi, bool bChecked, bool bDoOffToo)
        {
            Debug.Assert(tsmi != null);
            if (bDoOffToo)
            {
                if ((bChecked && !tsmi.Checked)
                    ||
                    (!bChecked && tsmi.Checked))
                {
                    tsmi.Checked = bChecked;
                    return true;
                }
            }
            else if (bChecked && !tsmi.Checked)
            {
                tsmi.Checked = true;
                return true;
            }
            return false;
        }

        internal static string GetOseVersion()
        {
            Assembly assy = Assembly.GetExecutingAssembly();
            string strLocation = assy.Location;
            if (String.IsNullOrEmpty(strLocation))
                return null;

            return FileVersionInfo.GetVersionInfo(strLocation).FileVersion;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strVersion = GetOseVersion();
            string strHeader = "About... OneStory Editor v" + strVersion;

            var dlg = new HtmlForm
                          {
                              Text = strHeader,
                              ClientText = Resources.IDS_CopyrightInfo
                          };

            dlg.ShowDialog();
        }

        internal SearchForm m_frmFind = null;
        private void editFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UsingHtmlForStoryBtPane)
            {
                // if this is called, it's very likely that CtrlTextBox._inTextBox has the first search box
                //  (if the search form is launched from the ConNotes panes, then they handle this themselves
                if (CtrlTextBox._inTextBox != null)
                    SearchForm.LastStringTransferSearched = CtrlTextBox._inTextBox.MyStringTransfer;
                LaunchSearchForm();
            }
        }

        internal void LaunchSearchForm()
        {
            if (m_frmFind == null)
                m_frmFind = new SearchForm();

            m_frmFind.Show(this, true);
        }

        internal void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UsingHtmlForStoryBtPane)
                return;

            if (m_frmFind == null)
            {
                m_frmFind = new SearchForm();
                m_frmFind.Show(this, true);
            }
            else
                m_frmFind.DoFindNext();
        }

        internal void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitAllPanes();

            if (m_frmFind != null)
                m_frmFind.ResetSearchParameters();

            Localizer.Default.Update();
            UpdateStoryNameComboBox();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UsingHtmlForStoryBtPane)
                return;

            // if this is called, it's very likely that CtrlTextBox._inTextBox has the first search box
            //  (if the search form is launched from the ConNotes panes, then they handle this themselves
            if (CtrlTextBox._inTextBox != null)
                SearchForm.LastStringTransferSearched = CtrlTextBox._inTextBox.MyStringTransfer;
            LaunchReplaceForm();
        }

        internal void LaunchReplaceForm()
        {
            if (m_frmFind == null)
                m_frmFind = new SearchForm();

            m_frmFind.Show(this, false);
        }

        private void changeProjectFolderRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog
                          {
                              Description =
                                  String.Format(
                                      Localizer.Str(
                                          "Browse to the folder where you want the program to create the '{0}' folder"),
                                      Resources.DefMyDocsSubfolder)
                          };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ProjectSettings.OneStoryProjectFolderRoot = dlg.SelectedPath;
                ProjectSettings.InsureOneStoryProjectFolderRootExists();

                string strOldProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Settings.Default.RecentProjectPaths.Count > 0)
                {
                    foreach (string projectPath in Settings.Default.RecentProjectPaths)
                    {
                        if (projectPath.IndexOf(strOldProjectPath) == 0)
                            strOldProjectPath = projectPath;
                    }
                    strOldProjectPath = Settings.Default.RecentProjectPaths[0];
                }
                else
                    strOldProjectPath = Path.Combine(strOldProjectPath, "OneStory Editor Projects");

                // clobber any recollection we had of existing projects, since they'll
                //  now need to be "browsed" for.
                Settings.Default.RecentProjects.Clear();
                Settings.Default.RecentProjectPaths.Clear();
                Settings.Default.Save();

                string strMessage = String.Format(Localizer.Str("The '{0}' folder is now the root folder for OneStory Editor projects. You can use Windows Explorer to move the projects (sub-folders) in '{1}' to '{0}'. Then click the 'Project' menu and choose the 'Browse for project file' command to re-load your projects from the new location"),
                                                  ProjectSettings.OneStoryProjectFolderRoot, strOldProjectPath);
                LocalizableMessageBox.Show(strMessage, OseCaption);
            }
        }

        private void toTheInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert(StoryProject.ProjSettings != null);

            var strProjectFolder = StoryProject.ProjSettings.ProjectFolder;
            var strProjectName = StoryProject.ProjSettings.ProjectName;
            Program.QueryHgRepoParameters(strProjectFolder,
                                          strProjectName,
                                          LoggedOnMember);

            // clean up any existing open project
            if (!SaveAndCloseProject())
                return;

            Program.SyncWithRepository(strProjectFolder, true);

            var projSettings = new ProjectSettings(strProjectFolder, strProjectName, true);
            OpenProject(projSettings);
        }

        private void toAThumbdriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if we're starting from a blank editor
            string strProjectFolder, strProjectName;
            if ((StoryProject == null) || (StoryProject.ProjSettings == null))
            {
                // we don't know where it's to go yet, so make the user browse for it
                // on the thumbdrive
                using (var dlg = new GetCloneFromUsbDialog(ProjectSettings.OneStoryProjectFolderRoot))
                {
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;

                    strProjectFolder = dlg.PathToNewlyClonedFolder;
                    strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);
                }
            }
            else
            {
                strProjectName = StoryProject.ProjSettings.ProjectName;
                strProjectFolder = StoryProject.ProjSettings.ProjectFolder;

                // clean up any existing open projects
                if (!SaveAndCloseProject())
                    return;
            }

            // next, do the sync
            Program.SyncWithRepositoryThumbdrive(strProjectFolder);
            var projSettings = new ProjectSettings(strProjectFolder, strProjectName, true);
            try
            {
                OpenProject(projSettings);
            }
            catch (Program.RestartException)
            {
                Close();
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private void storyCopyWithNewNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert(TheCurrentStory != null);

            InsertStoryHere(TheCurrentStory);
        }

        private void InsertStoryHere(StoryData theStoryToInsert)
        {
            string strStoryName = null;
            var nIndexOfCurrentStory = -1;
            if (!AddNewStoryGetIndex(ref nIndexOfCurrentStory, ref strStoryName, false)) 
                return;

            Debug.Assert(nIndexOfCurrentStory != -1);
            nIndexOfCurrentStory = Math.Min(nIndexOfCurrentStory + 1, TheCurrentStoriesSet.Count);
            var theNewStory = new StoryData(theStoryToInsert) {Name = strStoryName};
            InsertNewStoryAdjustComboBox(theNewStory, nIndexOfCurrentStory);
        }

        private void hiddenVersesToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            InitAllPanes();
        }

        protected string GetTbxDestPath(string strFilename)
        {
            return String.Format(@"{0}\Toolbox\{1}",
                                 StoryProject.ProjSettings.ProjectFolder, strFilename);
        }

        private void exportToToolboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // get the xml (.onestory) file into a memory string so it can be the 
                //  input to the transformer
                MemoryStream streamData = new MemoryStream(Encoding.UTF8.GetBytes(GetXml.ToString()));

                string strXslt = Resources.oneStory2storyingBT;
                // the 'document()' function in this Xslt needs the full path to the 
                //  running folder
                string strPathRunningFolder = StoryProjectData.GetRunningFolder;
                strXslt = strXslt.Replace("{0}", strPathRunningFolder);
                string strTbxStoriesBTFilePath = GetTbxDestPath("StoriesBT.txt");

                // make sure the folder exists
                if (!Directory.Exists(Path.GetDirectoryName(strTbxStoriesBTFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(strTbxStoriesBTFilePath));

                ExportToToolbox(strXslt, streamData, strTbxStoriesBTFilePath, "Stories");

                strTbxStoriesBTFilePath = GetTbxDestPath("NonBiblicalStoriesBT.txt");
                ExportToToolbox(strXslt, streamData, strTbxStoriesBTFilePath, "Non-Biblical Stories");

                strTbxStoriesBTFilePath = GetTbxDestPath("OldStoriesBT.txt");
                ExportToToolbox(strXslt, streamData, strTbxStoriesBTFilePath, "Old Stories");

                strXslt = Resources.oneStory2storyingRetelling;
                ExportToToolbox(strXslt, streamData,
                    GetTbxDestPath("TestRetellings.txt"), null);

                strXslt = Resources.oneStory2ConNotes;
                ExportToToolbox(strXslt, streamData,
                    GetTbxDestPath("ProjectConNotes.txt"), null);

                strXslt = Resources.oneStory2CoachNotes;
                ExportToToolbox(strXslt, streamData,
                    GetTbxDestPath("CoachingNotes.txt"), null);

                // for the key terms file, the xml is from the project key term db
                string strLnCNotesFilePath = GetTbxDestPath("L&CNotes.txt");
                string strKeyTermDb = TermRenderingsList.FileName(StoryProject.ProjSettings.ProjectFolder,
                                                                  StoryProject.ProjSettings.Vernacular.LangCode);

                if (File.Exists(strKeyTermDb))
                {
                    string strFileContents = File.ReadAllText(strKeyTermDb);
                    streamData = new MemoryStream(Encoding.UTF8.GetBytes(strFileContents));

                    strXslt = Resources.oneStory2KeyTerms;
                    // the 'document()' function in this Xslt needs the full path to the 
                    //  running folder
                    strXslt = strXslt.Replace("{0}", strPathRunningFolder);
                    ExportToToolbox(strXslt, streamData, strLnCNotesFilePath, null);
                }

                FileInfo fiLnC = new FileInfo(strLnCNotesFilePath);
                if (!fiLnC.Exists || fiLnC.Length == 0)
                    File.WriteAllText(strLnCNotesFilePath,
                        Resources.IDS_TbxFile_EmptyLnC);

                CopyDefaultToolboxProjectFiles();
            }
            catch (Exception ex)
            {
                string strErrorMsg = String.Format(Localizer.Str("Unable to export to Toolbox! Cause: {0} {1}"),
                    ex.Message,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""));
                LocalizableMessageBox.Show(strErrorMsg, OseCaption);
            }

            Cursor = Cursors.Default;
        }

        protected const string CstrProjectFilename = @"\StoryingProject.prj";
        protected const string CstrStoryBtTypFilename = @"\StoryBT.typ";
        protected const string CstrLCNoteTypFilename = @"\LCNote.typ";
        protected const string CstrStoryNotesTypFilename = @"\StoryNotes.typ";

        private void CopyDefaultToolboxProjectFiles()
        {
            // copy all of the files that are in [TARGETDIR]Toolbox into the same
            //  folder (but don't overwrite) so we can have a full, modifiable project
            string strDestFolder = GetTbxDestPath("");
            string strSrcFolder = StoryProjectData.GetRunningFolder + @"\Toolbox";
            string[] astrSrcFiles = Directory.GetFiles(strSrcFolder);
            foreach (string strSrcFile in astrSrcFiles)
            {
                string strFilename = Path.GetFileName(strSrcFile);
                string strDestFile = strDestFolder + strFilename;
                if (!File.Exists(strDestFile))
                    File.Copy(strSrcFile, strDestFile);
            }

            // the files in the PathFixup folder need to have folder information put in
            strSrcFolder += @"\PathFixup";

            // create the project file
            CreateTbxFileWithPathFixup(strSrcFolder, strDestFolder, CstrProjectFilename);

            // do the StoryBT.typ file
            string strStoryBTFilename = strSrcFolder + CstrStoryBtTypFilename;
            string strStoryBT = File.ReadAllText(strStoryBTFilename);
            strStoryBT = String.Format(strStoryBT,
                (String.IsNullOrEmpty(StoryProject.ProjSettings.Vernacular.LangCode) ? "vrn" : StoryProject.ProjSettings.Vernacular.LangCode),
                (String.IsNullOrEmpty(StoryProject.ProjSettings.Vernacular.LangName) ? "Vernacular Language" : StoryProject.ProjSettings.Vernacular.LangName),
                (String.IsNullOrEmpty(StoryProject.ProjSettings.NationalBT.LangCode) ? "n" : StoryProject.ProjSettings.NationalBT.LangCode),
                (String.IsNullOrEmpty(StoryProject.ProjSettings.NationalBT.LangName) ? "National Language BT" : StoryProject.ProjSettings.NationalBT.LangName),
                StoryProject.ProjSettings.InternationalBT.LangCode,
                (String.IsNullOrEmpty(StoryProject.ProjSettings.InternationalBT.LangName) ? "English Language BT" : StoryProject.ProjSettings.InternationalBT.LangName),
                (String.IsNullOrEmpty(StoryProject.ProjSettings.Vernacular.DefaultFontName) ? "Arial Unicode MS" : StoryProject.ProjSettings.Vernacular.DefaultFontName),
                (String.IsNullOrEmpty(StoryProject.ProjSettings.InternationalBT.DefaultFontName) ? "Arial Unicode MS" : StoryProject.ProjSettings.InternationalBT.DefaultFontName),
                ProjectSettings.OneStoryProjectFolderRoot,
                StoryProject.ProjSettings.ProjectName);

            strStoryBTFilename = strDestFolder + CstrStoryBtTypFilename;
            if (!File.Exists(strStoryBTFilename))
#if DEBUGBOB
            {
            }
            else
                File.Delete(strStoryBTFilename);
#endif
            File.WriteAllText(strStoryBTFilename, strStoryBT);

            // do the LCNote.typ file
            CreateTbxFileWithPathFixup(strSrcFolder, strDestFolder, CstrLCNoteTypFilename);

            // do the StoryNotes.typ file
            CreateTbxFileWithPathFixup(strSrcFolder, strDestFolder, CstrStoryNotesTypFilename);

            DialogResult res = LocalizableMessageBox.Show(String.Format(Localizer.Str(@"The project has been exported to the folder '{0}\Toolbox'. Click 'Yes' to launch Toolbox and open that project."),
                                                             StoryProject.ProjSettings.ProjectFolder),
                                               OseCaption,
                                               MessageBoxButtons.YesNoCancel);

            if (res == DialogResult.Yes)
                LaunchProgram(strDestFolder + CstrProjectFilename, null);
        }

        protected void CreateTbxFileWithPathFixup(string strSrcFolder,
            string strDestFolder, string strTbxFilename)
        {
            string strTbxFileFilename = strSrcFolder + strTbxFilename;
            string strTbxFileContents = File.ReadAllText(strTbxFileFilename);
            strTbxFileContents = String.Format(strTbxFileContents,
                                              ProjectSettings.OneStoryProjectFolderRoot,
                                              StoryProject.ProjSettings.ProjectName);

            // now get the destination address
            strTbxFileFilename = strDestFolder + strTbxFilename;
            if (!File.Exists(strTbxFileFilename))
#if DEBUGBOB
            {
            }
            else
                File.Delete(strTbxFileFilename);
#endif
            File.WriteAllText(strTbxFileFilename, strTbxFileContents);
        }

        protected void ExportToToolbox(string strXsltFile, MemoryStream streamData,
            string strTbxFilename, string strParameter)
        {
            // write the formatted XSLT to another memory stream.
            MemoryStream streamXSLT = new MemoryStream(Encoding.UTF8.GetBytes(strXsltFile));
            TransformedXmlDataToSfm(streamXSLT, streamData, strTbxFilename, strParameter);
        }

        protected void TransformedXmlDataToSfm(Stream streamXSLT, Stream streamData,
            string strTbxFilename, string strParameter)
        {
            var myProcessor = new XslCompiledTransform();
            var xslReader = XmlReader.Create(streamXSLT, new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse});
            var xsltSettings = new XsltSettings { EnableDocumentFunction = true, EnableScript = true };
            myProcessor.Load(xslReader, xsltSettings, null);

            // rewind
            streamData.Seek(0, SeekOrigin.Begin);

            XsltArgumentList xslArg = null;
            if (!String.IsNullOrEmpty(strParameter))
            {
                xslArg = new XsltArgumentList();
                xslArg.AddParam("storySet", "", strParameter);
            }

            XmlReader reader = XmlReader.Create(streamData);
            StringBuilder strBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            XmlWriter writer = XmlWriter.Create(strBuilder, settings);
            myProcessor.Transform(reader, xslArg, writer);

            Debug.Assert(Directory.Exists(Path.GetDirectoryName(strTbxFilename)));

            // overwrite existing version
            if (File.Exists(strTbxFilename))
                File.Delete(strTbxFilename);

            File.WriteAllText(strTbxFilename, strBuilder.ToString());
        }

        private void statusLabel_Click(object sender, EventArgs e)
        {
            if ((TheCurrentStory != null) && (TheCurrentStory.ProjStage != null))
            {
                // update the status bar (in case we previously put an error there
                StoryStageLogic.StateTransition st =
                    StoryStageLogic.stateTransitions[TheCurrentStory.ProjStage.ProjectStage];
                SetDefaultStatusBar(st.StageDisplayString);
            }
        }

        private void viewTransliterationVernacular_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi.Checked)
            {
                if (LoggedOnMember.TransliteratorVernacular == null)
                {
                    LoggedOnMember.TransliteratorVernacular =
                        GetDec(StoryProject.ProjSettings.Vernacular.LangName);
                }
            }

            InitializeTransliterators();
            ReInitVerseControls();
        }

        private DirectableEncConverter GetDec(string strLangName)
        {
            var aEc = DirectableEncConverter.EncConverters.AutoSelectWithTitle(ConvType.Unicode_to_from_Unicode,
                                                                               Localizer.Str("Choose the transliterator for ") +
                                                                               strLangName);
            Modified = true;
            return (aEc != null)
                       ? new DirectableEncConverter(aEc)
                       : null;
        }

        private void viewTransliterationNationalBT_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi.Checked)
            {
                if (LoggedOnMember.TransliteratorNationalBt == null)
                {
                    LoggedOnMember.TransliteratorNationalBt =
                        GetDec(StoryProject.ProjSettings.NationalBT.LangName);
                }
            }

            ReInitVerseControls();
        }

        private void viewTransliterationInternationalBt_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi.Checked)
            {
                if (LoggedOnMember.TransliteratorInternationalBt == null)
                {
                    LoggedOnMember.TransliteratorInternationalBt =
                        GetDec(StoryProject.ProjSettings.InternationalBT.LangName);
                }
            }

            ReInitVerseControls();
        }

        private void viewTransliterationFreeTranslation_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi.Checked)
            {
                if (LoggedOnMember.TransliteratorFreeTranslation == null)
                {
                    LoggedOnMember.TransliteratorFreeTranslation =
                        GetDec(StoryProject.ProjSettings.FreeTranslation.LangName);
                }
            }

            ReInitVerseControls();
        }

        private void viewTransliterationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if ((LoggedOnMember != null)
                && (StoryProject != null)
                && (StoryProject.ProjSettings != null))
            {
                SetTransliterationMenu(viewTransliterationVernacular,
                                       StoryProject.ProjSettings.Vernacular);

                SetTransliterationMenu(viewTransliterationNationalBT,
                                       StoryProject.ProjSettings.NationalBT);

                SetTransliterationMenu(viewTransliterationInternationalBt,
                                       StoryProject.ProjSettings.InternationalBT);

                SetTransliterationMenu(viewTransliterationFreeTranslation,
                                       StoryProject.ProjSettings.FreeTranslation);
            }
            else
            {
                viewTransliterationVernacular.Enabled =
                    viewTransliterationNationalBT.Enabled =
                    viewTransliterationInternationalBt.Enabled =
                    viewTransliterationFreeTranslation.Enabled = false;
            }
        }

        private static void SetTransliterationMenu(ToolStripMenuItem tsmi, ProjectSettings.LanguageInfo li)
        {
            tsmi.Text = li.LangName;
            tsmi.Visible = li.HasData;
        }

        private void viewTransliteratorVernacularConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoggedOnMember.TransliteratorVernacular = null;
            viewTransliterationVernacular.Checked = true;
            viewTransliterationVernacular_Click(viewTransliterationVernacular, null);
            viewTransliterationVernacular.Checked = (LoggedOnMember.TransliteratorVernacular != null);
        }

        private void viewTransliteratorNationalBTConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoggedOnMember.TransliteratorNationalBt = null;
            viewTransliterationNationalBT.Checked = true;
            viewTransliterationNationalBT_Click(viewTransliterationNationalBT, null);
            viewTransliterationNationalBT.Checked = (LoggedOnMember.TransliteratorNationalBt != null);
        }

        private void viewTransliteratorInternationalBtConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoggedOnMember.TransliteratorInternationalBt = null;
            viewTransliterationInternationalBt.Checked = true;
            viewTransliterationInternationalBt_Click(viewTransliterationInternationalBt, null);
            viewTransliterationInternationalBt.Checked = (LoggedOnMember.TransliteratorInternationalBt != null);
        }

        private void viewTransliteratorFreeTranslationConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoggedOnMember.TransliteratorFreeTranslation = null;
            viewTransliterationFreeTranslation.Checked = true;
            viewTransliterationFreeTranslation_Click(viewTransliterationFreeTranslation, null);
            viewTransliterationFreeTranslation.Checked = (LoggedOnMember.TransliteratorFreeTranslation != null);
        }

        private void linkLabelConsultantNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ll = sender as LinkLabel;
            if ((ll != null) && (e.Button == MouseButtons.Left))
                htmlConsultantNotesControl.OnVerseLineJump((int)ll.Tag);
        }

        private void linkLabelCoachNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ll = sender as LinkLabel;
            if ((ll != null) && (e.Button == MouseButtons.Left))
                htmlCoachNotesControl.OnVerseLineJump((int)ll.Tag);
        }

        private void linkLabelVerseBT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ll = sender as LinkLabel;
            if ((ll != null) && (e.Button == MouseButtons.Left))
            {
                int nVerseIndex = (int)ll.Tag;
                FocusOnVerse(nVerseIndex, true, true);
            }
        }

        public static string CstrFirstVerse
        {
            get { return Localizer.Str("Story (Ln: 0)"); }
        }

        public static bool QueryTerminalTransition
        {
            get
            {
                return (LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Click 'Yes' to confirm. If you click 'Yes', then you won't be able to make further changes to the story until it is returned to you. If you want to make any change first, then click 'No'"),
                    OseCaption,
                    MessageBoxButtons.YesNoCancel) == DialogResult.Yes);
            }
        }

        private void contextMenuStripVerseList_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStripVerseList.Items.Clear();
            if (TheCurrentStory != null)
            {
                if (contextMenuStripVerseList.SourceControl != linkLabelVerseBT)
                    contextMenuStripVerseList.Items.Add(CstrFirstVerse, null,
                        onClickVerseNumber);

                for (int i = 0; i < TheCurrentStory.Verses.Count; i++)
                {
                    VerseData aVerse = TheCurrentStory.Verses[i];
                    string strMenuText = VersesData.LinePrefix + (i + 1);
                    if (!aVerse.IsVisible)
                        strMenuText += VersesData.HiddenStringSpace;
                    contextMenuStripVerseList.Items.Add(strMenuText, null,
                        onClickVerseNumber);
                }
            }
        }

        private void onClickVerseNumber(object sender, EventArgs e)
        {
            string strMenuText = (sender as ToolStripMenuItem).Text;
            int nVerseNumber;
            if (strMenuText == CstrFirstVerse)
                nVerseNumber = 0;
            else
            {
                var str = VersesData.HiddenStringSpace;
                if (strMenuText.IndexOf(str) > 0)
                {
                    strMenuText = strMenuText.Substring(0, strMenuText.Length - str.Length);
                    viewHiddenVersesMenu.Checked = true;
                }
                int nIndex = strMenuText.IndexOf(' ');
                if (nIndex == -1)
                    return; // must be an error in the localization
                nVerseNumber = Convert.ToInt32(strMenuText.Substring(nIndex + 1));
            }
            FocusOnVerse(nVerseNumber, true, true);
        }

        private void historicalDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_dlgHistDiffDlg = new RevisionHistoryForm(this, TheCurrentStory);
            m_dlgHistDiffDlg.Show();
        }

        private void useSameSettingsForAllStoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.LastUseForAllStories = viewUseSameSettingsForAllStoriesMenu.Checked;
            Settings.Default.Save();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_dlgPrintForm = new PrintForm(this);
            m_dlgPrintForm.Show();
        }

        private void resetStoredInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.Reset();
            Program.InitializeLocalSettingsCollections(false);
            Settings.Default.Save();
        }

        internal void concordanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strVernacular = null, strNationalBt = null, strInternationalBt = null, strFreeTranslation = null;
            GetSelectedLanguageText(ref strVernacular, ref strNationalBt, ref strInternationalBt, ref strFreeTranslation);
            var dlg = new ConcordanceForm(this, strVernacular, strNationalBt, strInternationalBt, strFreeTranslation);
            dlg.Show();
        }

        private void GetSelectedLanguageText(ref string strVernacular, ref string strNationalBt, ref string strInternationalBt, ref string strFreeTranslation)
        {
            if (UsingHtmlForStoryBtPane)
                htmlStoryBtControl.GetSelectedLanguageText(out strVernacular, out strNationalBt, out strInternationalBt, out strFreeTranslation);
            else
                GetSelectedLanguageTextNetCtrls(ref strVernacular, ref strNationalBt, ref strInternationalBt, ref strFreeTranslation);
        }

        private void GetSelectedLanguageTextNetCtrls(ref string strVernacular, ref string strNationalBt,
            ref string strInternationalBt, ref string strFreeTranslation)
        {
            if ((CtrlTextBox._inTextBox == null) || (CtrlTextBox._nLastVerse <= 0))
                return;

            Control ctrl = flowLayoutPanelVerses.GetControlAtVerseIndex(CtrlTextBox._nLastVerse);
            if (ctrl == null)
                return;

            Debug.Assert(ctrl is VerseBtControl);
            var theVerse = ctrl as VerseBtControl;
            if (theVerse == null)
                return;

            if (IsInStoryLine(theVerse))
            {
                var lineData = theVerse._verseData.StoryLine;
                GetSelectedDataFromLineData(lineData,
                                            ref strVernacular,
                                            ref strNationalBt,
                                            ref strInternationalBt,
                                            ref strFreeTranslation);
            }
            else if ((CtrlTextBox._inTextBox != null) &&
                     !String.IsNullOrEmpty(CtrlTextBox._inTextBox._strLabel))
            {
                string strLabel = CtrlTextBox._inTextBox._strLabel;
                if (IsTestQuestionBox(strLabel))
                {
                    var testQuestionData = GetTestQuestionData(strLabel, theVerse);
                    var lineData = testQuestionData.TestQuestionLine;
                    GetSelectedDataFromLineData(lineData,
                                                ref strVernacular,
                                                ref strNationalBt,
                                                ref strInternationalBt,
                                                ref strFreeTranslation);
                }
                else if (IsRetellingBox(strLabel))
                {
                    LineMemberData retellingData = GetRetellingData(strLabel, theVerse);
                    GetSelectedDataFromLineData(retellingData,
                                                ref strVernacular,
                                                ref strNationalBt,
                                                ref strInternationalBt,
                                                ref strFreeTranslation);
                }
                else if (IsTqAnswerBox(strLabel))
                {
                    // e.g. "ans 1:tst 1:"
                    AnswersData answers;
                    var answerData = GetTqAnswerData(strLabel, theVerse, out answers);
                    GetSelectedDataFromLineData(answerData,
                                                ref strVernacular,
                                                ref strNationalBt,
                                                ref strInternationalBt,
                                                ref strFreeTranslation);
                }
            }
        }

        private void GetSelectedDataFromLineData(LineData lineData,
            ref string strVernacular, ref string strNationalBt,
            ref string strInternationalBt, ref string strFreeTranslation)
        {
            if (viewVernacularLangMenu.Checked &&
                (lineData.Vernacular.TextBox != null))
            {
                strVernacular = GrabTrimSelectedText(lineData.Vernacular.TextBox);
            }
            if (viewNationalLangMenu.Checked &&
                (lineData.NationalBt.TextBox != null))
            {
                strNationalBt = GrabTrimSelectedText(lineData.NationalBt.TextBox);
            }
            if (viewEnglishBtMenu.Checked &&
                (lineData.InternationalBt.TextBox != null))
            {
                strInternationalBt = GrabTrimSelectedText(lineData.InternationalBt.TextBox);
            }
            if (viewFreeTranslationMenu.Checked &&
                (lineData.FreeTranslation.TextBox != null))
            {
                strFreeTranslation = GrabTrimSelectedText(lineData.FreeTranslation.TextBox);
            }
        }

        private string GrabTrimSelectedText(CtrlTextBox tbx)
        {
            string str = tbx.SelectedText;
            if (!String.IsNullOrEmpty(str))
                str = str.Trim();
            return str;
        }

        private void viewLnCNotesMenu_Click(object sender, EventArgs e)
        {
            var dlg = new LnCNotesForm(this);
            dlg.Show();
        }

        internal void AddLnCNote()
        {
            string strVernacular = null, strNationalBT = null, strInternationalBT = null,
                strDummy = null;
            GetSelectedLanguageText(ref strVernacular, ref strNationalBT, ref strInternationalBT, ref strDummy);
            var dlg = new AddLnCNoteForm(this, strVernacular, strNationalBT, strInternationalBT);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            StoryProject.LnCNotes.CheckForSimilarNote(this, strVernacular, strNationalBT, strInternationalBT);

            StoryProject.LnCNotes.Add(dlg.TheLnCNote);
            Modified = true;
        }

        /*
        private void toolStripMenuItemSelectState_Click(object sender, EventArgs e)
        {
            if ((StoryProject == null) || (TheCurrentStory == null))
                return;

#if !UseAdvancedStatePickerByDefault
            SelectStateBasicForm dlg = new SelectStateBasicForm(StoryProject, TheCurrentStory);
#else
            // locate the window near the cursor...
            Point ptTooltip = Cursor.Position;
            StageEditorForm dlg = new StageEditorForm(StoryProject, theCurrentStory, ptTooltip, false);
#endif
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Debug.Assert(dlg.NextState != StoryStageLogic.ProjectStages.eUndefined);
                if (TheCurrentStory.ProjStage.ProjectStage != dlg.NextState)
                {
                    StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[dlg.NextState];
                    SetNextState(dlg.NextState, true);
                }
            }

            if (dlg.ViewStateChanged)
                SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, false);
        }
        */

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            GoToFirstStory();
        }

        private void GoToFirstStory()
        {
            if ((TheCurrentStoriesSet != null) && (TheCurrentStory != null))
                comboBoxStorySelector.SelectedIndex = 0;
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            GoToPreviousStory();
        }

        private void GoToPreviousStory()
        {
            if ((TheCurrentStoriesSet == null) || (TheCurrentStory == null))
                return;

            int nIndex = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
            if (nIndex > 0)
                comboBoxStorySelector.SelectedIndex = --nIndex;
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            GoToNextStory();
        }

        private void GoToNextStory()
        {
            if ((StoryProject == null)
                || String.IsNullOrEmpty(CurrentStoriesSetName)
                || (StoryProject[CurrentStoriesSetName] == null)
                || (TheCurrentStory == null))
                return;

            int nIndex = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
            if (nIndex < (TheCurrentStoriesSet.Count - 1))
                comboBoxStorySelector.SelectedIndex = ++nIndex;
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            GoToLastStory();
        }

        private void GoToLastStory()
        {
            if ((TheCurrentStoriesSet != null) && (TheCurrentStory != null))
                comboBoxStorySelector.SelectedIndex = (TheCurrentStoriesSet.Count - 1);
        }

        private void viewOnlyOpenConversationsMenu_CheckStateChanged(object sender, EventArgs e)
        {
            if (viewOnlyOpenConversationsMenu.Checked)
                TheCurrentStory.Verses.ResetShowOpenConversationsFlags();

            InitAllPanes();
        }

        private void stateTransitionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TheCurrentStory.TransitionHistory.HasData)
            {
                WarnNoTransitionHistory();
                return;
            }

            var dlg = new TransitionHistoryForm(TheCurrentStory.TransitionHistory, StoryProject.TeamMembers);
            dlg.ShowDialog();
        }

        public static void WarnNoTransitionHistory()
        {
            LocalizableMessageBox.Show(Localizer.Str("There is no turn transition history for this story"),
                            OseCaption);
        }

        private void changeStateWithoutChecksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // locate the window near the cursor...
            Point ptTooltip = Cursor.Position;

            if (LocalizableMessageBox.Show(Localizer.Str("Are you sure you want to change the turn of this story bypassing the normal checks (i.e. is the right person logged in, have they answered all their questions, etc). This is an advanced command and can easily result in a conflict if it's possible that two people will be editing the story at the same time. You should only use this command if you are sure that no one else will be editing this story prior to first synchronizing with the changes you are about to make"),
                OseCaption,
                MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            OverrideLocalizeStateInfo(ptTooltip, true);
        }

        private void advancedOverrideLocalizeStateViewSettingsMenu_Click(object sender, EventArgs e)
        {
            // locate the window near the cursor...
            Point ptTooltip = Cursor.Position;

            OverrideLocalizeStateInfo(ptTooltip, false);
        }

        private void OverrideLocalizeStateInfo(Point ptTooltip, bool bChangingState)
        {
            var dlg = new StageEditorForm(StoryProject, TheCurrentStory, ptTooltip, bChangingState);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Debug.Assert(dlg.NextState != StoryStageLogic.ProjectStages.eUndefined);
                if (TheCurrentStory.ProjStage.ProjectStage == dlg.NextState)
                    return;

                SetNextStateAdvancedOverride(dlg.NextState,
                                             TheCurrentStory.ProjStage.IsTerminalTransition(dlg.NextState));
            }

                // even if we don't change the state, we might 
            else if (dlg.ViewStateChanged)
            {
                SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, true);
                InitAllPanes(); // just in case there were changes
            }
        }

        private void advancedToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            advancedChangeStateWithoutChecksMenu.Enabled =
                advancedOverrideLocalizeStateViewSettingsMenu.Enabled =
                advancedImportHelper.Enabled =
                advancedCoachNotesToConsultantNotesPane.Enabled =
                advancedConsultantNotesToCoachNotesPane.Enabled =
                advancedReassignNotesToProperMember.Enabled =
                advancedSwapDataColumns.Enabled = 
                ((StoryProject != null) && (TheCurrentStory != null));

            advancedNewProjectMenu.Enabled = (IsInStoriesSet);
            advancedEmailMenu.Checked = Settings.Default.UseMapiPlus;
            advancedUseWordBreaks.Enabled = BreakIterator.IsAvailable;
            advancedOneStoryProjectMetaData.Enabled = (StoryProject != null) && (StoryProject.ProjSettings != null);
        }

        private void checkForProgramUpdatesNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpgrade(_autoUpgrade,
                            (ModifierKeys & Keys.Control) == Keys.Control
                                ? Program.IDS_OSEUpgradeServerTest
                                : Program.IDS_OSEUpgradeServer, false);
        }

        private void checkNowForNextMajorUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // make sure they have the latest of the current stream
            CheckForUpgrade(_autoUpgrade, Program.IDS_OSEUpgradeServer, true);

            // then check for the next major upgrade
            CheckForUpgrade(null, Program.IDS_OSEUpgradeServerNextMajorUpgrade, false);
        }

        private bool _bRestarting;

        private void CheckForUpgrade(AutoUpgrade autoUpgrade, string strManifestUrl, bool bDontRepaintYet)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                // save changes before checking (so we can close rapidly if need be)
                if (!CheckForSaveDirtyFile())
                    return;

                // this shouldn't be needed, but it seems that occasionally, it is...
                SuspendSaveDialog++;
                Program.CheckForProgramUpdate(autoUpgrade, strManifestUrl);
                SuspendSaveDialog--;

                if (bDontRepaintYet)
                    return;

                // since the call to SaveDirty will have removed them all
                if (TheCurrentStory != null)
                    InitAllPanes();

                // if it returns here without throwing an exception, it means there were no updates
                LocalizableMessageBox.Show(Localizer.Str("There are no program updates!"),
                                OseCaption);
            }
            catch (Program.RestartException)
            {
                // if it returns here without throwing an exception, it means there were no updates
                LocalizableMessageBox.Show(Localizer.Str("OneStory Editor needs to be restarted"),
                                OseCaption);

                _bRestarting = true;
                Close();
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                LocalizableMessageBox.Show(strMessage, OseCaption);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void programUpdatesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Checked =
                Settings.Default.AutoCheckForProgramUpdatesAtStartup;
        }

        private void automaticallyCheckAtStartupToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Default.AutoCheckForProgramUpdatesAtStartup =
                advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Checked;
            Settings.Default.Save();
        }

        private void enabledToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (advancedSaveTimeoutEnabledMenu.Checked)
            {
                //autosave timer goes off every 5 minutes.
                mySaveTimer.Tick += TimeToSave;
                mySaveTimer.Interval = CnIntervalBetweenAutoSaveReqs;
                mySaveTimer.Start();
            }
            else
            {
                mySaveTimer.Stop();
            }

            Settings.Default.AutoSaveTimeoutEnabled = advancedSaveTimeoutEnabledMenu.Checked;
            Settings.Default.Save();
        }

        private void advancedAutomaticallyLoadProjectMenu_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Default.AutoLoadLastProject = advancedAutomaticallyLoadProjectMenu.Checked;
            Settings.Default.Save();
        }

        private void sendReceiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert((StoryProject != null) &&
                (StoryProject.ProjSettings != null) &&
                (LoggedOnMember != null));
            // just pretend this is the same as a reload. (it'll take care of the rest)
            string strProjectName = StoryProject.ProjSettings.ProjectName;
            string strProjectPath = StoryProject.ProjSettings.ProjectFolder;
            DoReopen(strProjectPath, strProjectName);
        }

        private void DoReopen(string strProjectPath, string strProjectName)
        {
            try
            {
                OpenProject(strProjectPath, strProjectName);
            }
            catch (ProjectSettings.ProjectFileNotFoundException ex)
            {
                // the file doesn't exist anymore, so remove it from the recent used list
                int nIndex = Settings.Default.RecentProjects.IndexOf(strProjectName);
                if (nIndex != -1)
                {
                    Settings.Default.RecentProjects.RemoveAt(nIndex);
                    Settings.Default.RecentProjectPaths.RemoveAt(nIndex);
                    Settings.Default.Save();
                    LocalizableMessageBox.Show(ex.Message, OseCaption);
                }
            }
            catch (Program.RestartException)
            {
                // throw; if we do this, it seems it doesn't want to work (since it's
                //  on a handler, I guess). So see if we can just close
                Close();
            }
            catch (DuplicateStoryStateTransitionException)
            {
                // pass this one on so it triggers an email
                throw;
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        /// <summary>
        /// Insert the following story after the given verse
        /// </summary>
        /// <param name="verseStart">the verse after which the next story is to be inserted</param>
        public void JoinStory(VerseData verseStart)
        {
            Debug.Assert((TheCurrentStoriesSet != null) && (TheCurrentStory != null));

            var nIndexOfCurrentStory = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
            var nIndexOfNextStory = nIndexOfCurrentStory + 1;
            Debug.Assert(TheCurrentStoriesSet.Count > nIndexOfNextStory, "There must be a following story to join stories!");

            // make a copy of the story (so we get new guids)
            var theNextStory = new StoryData(TheCurrentStoriesSet[nIndexOfNextStory]);
            var nIndexVerseInsert = TheCurrentStory.Verses.IndexOf(verseStart) + 1;
            TheCurrentStory.Verses.InsertRange(nIndexVerseInsert, theNextStory.Verses);
            Modified = true;
        }

        public void SplitStory(VerseData verseStart)
        {
            Debug.Assert(TheCurrentStory != null);

            string strStoryName = null;
            int nIndexOfCurrentStory = -1;
            if (AddNewStoryGetIndex(ref nIndexOfCurrentStory, ref strStoryName, false))
            {
                Debug.Assert(nIndexOfCurrentStory != -1);
                int nIndexToInsert = Math.Min(nIndexOfCurrentStory + 1, TheCurrentStoriesSet.Count);

                // first clone the story... (so we get everything, including the state, etc)
                var theNewStory = new StoryData(TheCurrentStory) { Name = strStoryName };

                // then, delete the latter verses from current story and the earlier verses
                //  from the new one
                int nIndex = TheCurrentStory.Verses.IndexOf(verseStart);
                int nNumberToMove = TheCurrentStory.Verses.Count - nIndex;
                theNewStory.Verses.RemoveRange(0, nIndex);
                TheCurrentStory.Verses.RemoveRange(nIndex, nNumberToMove);
                InsertNewStoryAdjustComboBox(theNewStory, nIndexToInsert);
            }
        }

        public void CheckBiblePaneCursorPosition()
        {
            if (!_bAutoHide || !viewBibleMenu.Checked)
                return;

            Point locationNetBibleViewer = netBibleViewer.PointToScreen(netBibleViewer.Location);
            var rectangleNetBibleViewer = new Rectangle(locationNetBibleViewer, netBibleViewer.DisplayRectangle.Size);
            if (rectangleNetBibleViewer.Contains(MousePosition))
            {
                if (splitContainerUpDown.IsMinimized)
                    splitContainerUpDown.Restore();
            }
            else if (splitContainerUpDown.IsRestored && !netBibleViewer.checkBoxAutoHide.Checked)
                splitContainerUpDown.Minimize();
        }

        private void CheckBiblePaneCursorPositionMouseMove(object sender, MouseEventArgs e)
        {
            CheckBiblePaneCursorPosition();
        }

        private void storyAdaptItVernacularToNationalMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            if (((int)TheCurrentStory.ProjStage.ProjectStage)
                < (int)StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
            {
                MoveToNationalBtState();
            }

            string strStory = GetFullStoryContentsVernacular;
            GlossInAdaptIt(strStory, ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt);
        }

        public void MoveToNationalBtState()
        {
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ProjectFacilitator))
            {
                SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeNationalBT, false);
            }
            else
            {
                viewNationalLangMenu.Checked = true;
            }
        }

        private void storyAdaptItVernacularToEnglishMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            if (((int)TheCurrentStory.ProjStage.ProjectStage)
                < (int)StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
            {
                MoveToInternationalBtState();
            }

            string strStory = GetFullStoryContentsVernacular;
            GlossInAdaptIt(strStory, ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt);
        }

        public void MoveToInternationalBtState()
        {
            if (TeamMemberData.IsUser(LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ProjectFacilitator))
            {
                SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT, false);
            }
            else
            {
                viewEnglishBtMenu.Checked = true;
            }
        }

        private void storyAdaptItNationalToEnglishMenuItem_Click(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert((TheCurrentStory != null) && (TheCurrentStory.Verses.Count > 0));

            if (((int)TheCurrentStory.ProjStage.ProjectStage)
                < (int)StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
            {
                MoveToInternationalBtState();
            }

            string strStory = GetFullStoryContentsNationalBTText;
            GlossInAdaptIt(strStory, ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt);
        }

        private void linkLabelTasks_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTaskBarForm();
        }

        private void ShowTaskBarForm()
        {
            if (StoryProject == null)
                return;

            var dlg = new TaskBarForm(this, StoryProject, TheCurrentStory);
            dlg.ShowDialog();
        }

        private void synchronizeSharedAdaptItProjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SyncAiKb(StoryProject.ProjSettings.VernacularToNationalBt);
            SyncAiKb(StoryProject.ProjSettings.VernacularToInternationalBt);
            SyncAiKb(StoryProject.ProjSettings.NationalBtToInternationalBt);
        }

        private void SyncAiKb(ProjectSettings.AdaptItConfiguration aiconfig)
        {
            if ((aiconfig == null) ||
                (aiconfig.ProjectType != ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject))
                return;

            try
            {
                aiconfig.AlreadyCheckedForSync = false;
                ProjectSettings.LanguageInfo liSourceLang, liTargetLang;
                AdaptItGlossing.InitLookupAdapter(StoryProject.ProjSettings,
                                                  aiconfig.BtDirection,
                                                  LoggedOnMember,
                                                  out liSourceLang,
                                                  out liTargetLang);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }
        }

        private bool CheckForProperEditToken()
        {
            try
            {
                if (!IsInStoriesSet)
                    throw CantEditOldStoriesEx;

                LoggedOnMember.ThrowIfEditIsntAllowed(TheCurrentStory);
            }
            catch (Exception ex)
            {
                SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        private void addgeneralTestQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForProperEditToken())
                return;

            TheCurrentStory.Verses.FirstVerse.TestQuestions.AddTestQuestion();

            // just in case it wasn't showing
            viewGeneralTestingsQuestionMenu.Checked = true;
            ReInitVerseControls();
            Modified = true;
        }

        private bool SaveAndCloseProject()
        {
            // clean up any existing open projects
            if (!CheckForSaveDirtyFile())
                return false;

            // see if we should save the OS project meta data
            if ((StoryProject != null) &&
                (StoryProject.OsMetaData != null) &&
                (LoggedOnMember != null))
            {
                StoryProject.SaveProjectMetaData(LoggedOnMember);
            }

            CloseProjectFile();
            return true;
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAndCloseProject();
        }

        private void storyOverrideTasks_Click(object sender, EventArgs e)
        {
            Debug.Assert(_theCurrentStory != null);
            TeamMemberData.UserTypes eWhoEdits = _theCurrentStory.ProjStage.MemberTypeWithEditToken;
            if (TeamMemberData.IsUser(eWhoEdits,
                TeamMemberData.UserTypes.ProjectFacilitator))
            {
                Modified |= SetPfTasksForm.EditPfTasks(this, ref _theCurrentStory);
            }
            else if (TeamMemberData.IsUser(eWhoEdits,
                TeamMemberData.UserTypes.ConsultantInTraining))
            {
                Modified |= SetCitTasksForm.EditCitTasks(ref _theCurrentStory);
            }
        }

        private void projectNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.Assert((StoryProject != null) && (StoryProject.ProjSettings != null));
            InitProjectNotes(StoryProject.ProjSettings, LoggedOnMember.Name);
        }

        private LocDataEditorForm _localizationEditor;

        private void localizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_localizationEditor == null)
                _localizationEditor = new LocDataEditorForm(Localizer.Default,
                                                            new[]
                                                                {
                                                                    "OneStoryProjectEditor",
                                                                    "Chorus.UI.Sync.SyncDialog",
                                                                    "NetLoc"
                                                                });
            LocDataEditorForm.DelegateCallOnClose = OnCloseLocalizationDialog;
            _localizationEditor.Show();
        }

        private void OnCloseLocalizationDialog(bool bWasViaFileClose)
        {
            // for us, if the user does "File", "Close", then totally Dispose if it
            //  otherwise, just hide it
            OnLocalizationChange();
            if (bWasViaFileClose)
                _localizationEditor = null;
            else
                _localizationEditor.Hide();
        }

        // global scope variables that might need to be updated whenever the localization
        //  changes
        public static void OnLocalizationChangeStatic()
        {
            NetBibleViewer.OnLocalizationChangeStatic();
            ConsultNoteDataConverter.OnLocalizationChange();
        }

        public void OnLocalizationChange()
        {
            netBibleViewer.OnLocalizationChange(true);
            ConsultNoteDataConverter.OnLocalizationChange();
            if (UsingHtmlForStoryBtPane)
                htmlStoryBtControl.ResetContextMenu();

            Settings.Default.LastLocalizationId = Localizer.Default.LanguageId;
            Settings.Default.Save();
            ReloadStateTransitionFile();
            if ((TheCurrentStory != null) && (TheCurrentStory.ProjStage != null))
            {
                SetViewBasedOnProjectStage(TheCurrentStory.ProjStage.ProjectStage, true);
                InitAllPanes(); // just in case the language changed
            }
        }

        public static string OseCaption
        {
            get { return Localizer.Str("OneStory Project Editor"); }
        }

        private void advancedEmailMenu_Click(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Settings.Default.UseMapiPlus = (sender as ToolStripMenuItem).Checked;
                Settings.Default.Save();
            }
        }

        private bool LanguageInUse(ProjectSettings.LanguageInfo li)
        {
            return ((li != null) && li.HasData);
        }

        private void advancedImportHelper_Click(object sender, EventArgs e)
        {
            // chances are that the user wants all the fields visible
            var viewSettings = new VerseData.ViewSettings
                (
                StoryProject.ProjSettings,
                LanguageInUse(StoryProject.ProjSettings.Vernacular), // viewVernacularLangMenu.Checked,
                LanguageInUse(StoryProject.ProjSettings.NationalBT), // viewNationalLangMenu.Checked,
                LanguageInUse(StoryProject.ProjSettings.InternationalBT), // viewEnglishBtMenu.Checked,
                LanguageInUse(StoryProject.ProjSettings.FreeTranslation), // viewFreeTranslationMenu.Checked,
                true, // viewAnchorsMenu.Checked,
                true, // viewExegeticalHelps.Checked,
                true, // viewStoryTestingQuestionsMenu.Checked,
                true, // viewStoryTestingQuestionAnswersMenu.Checked,
                true, // viewRetellingsMenu.Checked,
                viewConsultantNotesMenu.Checked,
                viewCoachNotesMenu.Checked,
                true, // viewBibleMenu.Checked,
                true,
                viewHiddenVersesMenu.Checked,
                viewOnlyOpenConversationsMenu.Checked,
                viewGeneralTestingsQuestionMenu.Checked,
                true,   // use textareas
                CurrentFieldEditability(TheCurrentStory),
                null,
                null,
                null,
                null);
            SetViewSettings(viewSettings, viewUseSameSettingsForAllStoriesMenu.Checked);

            var dlg = new TextPaster(null);
            dlg.Show();
        }

        public string TriggerDropboxCopyStory(bool bCopyToDropbox, string strFileToCopy)
        {
            var strFilename = Localizer.Str("Story");
            return ShouldCopyFileToDropbox(strFilename, bCopyToDropbox, strFileToCopy);
        }

        public string TriggerDropboxCopyRetelling(bool bCopyToDropbox)
        {
            var strFilename = Localizer.Str("Retelling ") +
                              TheCurrentStory.CraftingInfo.TestersToCommentsRetellings.Count.ToString(CultureInfo.InvariantCulture);
            return ShouldCopyFileToDropbox(strFilename, bCopyToDropbox, null);
        }

        public string TriggerDropboxCopyAnswer(bool bCopyToDropbox)
        {
            var strFilename = Localizer.Str("Inference Test Answers ") +
                              TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers.Count.ToString(CultureInfo.InvariantCulture);
            return ShouldCopyFileToDropbox(strFilename, bCopyToDropbox, null);
        }

        private void InitStoryBtPaneControl(bool bUsingHtmlForStoryBtPane)
        {
            UsingHtmlForStoryBtPane = bUsingHtmlForStoryBtPane;
            if (bUsingHtmlForStoryBtPane)
            {
                flowLayoutPanelVerses.Visible = false;
                htmlStoryBtControl.Visible = true;
            }
            else
            {
                htmlStoryBtControl.Visible = false;
                flowLayoutPanelVerses.Visible = true;
            }
        }

        private void advancedUseOldStyleStoryBtPaneMenu_Click(object sender, EventArgs e)
        {
            // reset what we used to be using...
            if (UsingHtmlForStoryBtPane)
                flowLayoutPanelVerses.Controls.Clear();
            else
                htmlStoryBtControl.ResetDocument();

            InitStoryBtPaneControl(UsingHtmlForStoryBtPane);
            Settings.Default.UsingHtmlForStoryBtPane = UsingHtmlForStoryBtPane;
            Settings.Default.Save();

            // repaint with the new one
            InitAllPanes();
        }

        internal static string CstrAddNoteOnSelected
        {
            get { return Localizer.Str("&Add note on selected text"); }
        }

        internal static string CstrAddNoteToSelfOnSelected
        {
            get { return Localizer.Str("Add no&te to self on selected text"); }
        }

        internal static string CstrJumpToReference
        {
            get { return Localizer.Str("&Jump to Bible reference"); }
        }

        internal static string CstrConcordanceSearch
        {
            get { return Localizer.Str("Concordance &search"); }
        }

        internal static string CstrAddLnCNote
        {
            get { return Localizer.Str("Add &L && C note"); }
        }

        internal static string CstrCutSelected
        {
            get { return Localizer.Str("C&ut"); }
        }

        internal static string CstrCopySelected
        {
            get { return Localizer.Str("&Copy"); }
        }

        internal static string CstrCopyOriginalSelected
        {
            get { return Localizer.Str("Copy &original text (before transliteration)"); }
        }

        internal static string CstrPasteSelected
        {
            get { return Localizer.Str("&Paste"); }
        }

        internal static string CstrUndo
        {
            get { return Localizer.Str("U&ndo"); }
        }

        internal static string CstrAddAnswerBox
        {
            get { return Localizer.Str("Add ans&wer box"); }
        }

        internal static string CstrRemAnswerBox
        {
            get { return Localizer.Str("Remove ans&wer box"); }
        }

        internal static string CstrRemAnswerChangeUns
        {
            get { return Localizer.Str("Change UN&S"); }
        }

        internal static string CstrReorderWords
        {
            get { return Localizer.Str("&Reorder words"); }
        }

        internal static string CstrGlossTextToNational
        {
            get { return Localizer.Str("&Back-translate to national language"); }
        }

        internal static string CstrGlossTextToEnglish
        {
            get { return Localizer.Str("Back-translate to &English"); }
        }
        
        private void advancedCoachNotesToConsultantNotesPane_Click(object sender, EventArgs e)
        {
            TheCurrentStory.MoveCoachNotesToConsultantNotePane();
            Modified = true;
            InitAllPanes();
            LocalizableMessageBox.Show(
                Localizer.Str(
                    "Next you should click 'Project', 'Login' and Edit the member records for the two roles that are changing to change their role within the project (e.g. Change the Coach's record to make him/her the Independent Consultant) and then click 'Story', 'Story Information' and use the 'Change' link next to the team member roles that are changing (e.g. to change the member who is the Consultant assigned to the story). This needs to be done for every story"),
                OseCaption);
        }

        private void advancedConsultantNotesToCoachNotesPane_Click(object sender, EventArgs e)
        {
            TheCurrentStory.MoveConsultantNotesToCoachNotePane();
            Modified = true;
            InitAllPanes();
            LocalizableMessageBox.Show(
                Localizer.Str(
                    "Next you should click 'Project', 'Login' and Edit the member records for the two roles that are changing to change their role within the project (e.g. Change the Consultant's record to make him/her a Coach) and then click 'Story', 'Story Information' and use the 'Change' link next to the team member roles that are changing (e.g. to change the member who is the Consultant assigned to the story). This needs to be done for every story"),
                OseCaption);
        }

        private void advancedReassignNotesToProperMember_Click(object sender, EventArgs e)
        {
            if (!MemberIdInfo.Configured(TheCurrentStory.CraftingInfo.ProjectFacilitator) ||
                !MemberIdInfo.Configured(TheCurrentStory.CraftingInfo.Consultant))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Configure the member roles in the 'Story', 'Story Information' dialog first and then click this menu."),
                    OseCaption);
                return;
            }
            TheCurrentStory.ReassignRolesToConNoteComments();
        }

        private void StoryImportFromSayMoreClick(object sender, EventArgs e)
        {
            var cursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                DoSaymoreImport();
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                InitAllPanes();// just to make sure we aren't hiding something
            }
            finally
            {
                Cursor = cursor;
            }
        }

        private bool _bNagOnceSayMoreImport = true;
        private delegate StringTransfer StringTransferDelegate(LineData ld);
        private delegate LineData LineDataDelegate(VerseData vd);
        private void DoSaymoreImport()
        {
            CheckForSaveDirtyFileNoCleanup();
            var dlg = new SayMoreImportForm(TheCurrentStory, StoryProject.ProjSettings);
            if (dlg.ShowDialog() != DialogResult.OK) 
                return;

            string strUnsGuid = null;
            LineDataDelegate lineData;
            switch (dlg.SaymoreImportType)
            {
                case SayMoreImportForm.SaymoreImportTypes.NewStory:
                    lineData = v => v.StoryLine;
                    break;
                case SayMoreImportForm.SaymoreImportTypes.Retelling:
                    if (!AddRetellingTest(true))
                        return;
                    lineData = v => v.Retellings.Last();

                    // keep track of the guid of this UNS in case we have to add more empty retellings.
                    strUnsGuid = TheCurrentStory.Verses[0].Retellings.Last().MemberId;
                    break;
                default:
                    lineData = null;
                    break;
            }

            StringTransferDelegate stTranscription;
            switch (dlg.TranscriptionField)
            {
                case TextFields.NationalBt:
                    stTranscription = ld => ld.NationalBt;
                    break;
                case TextFields.InternationalBt:
                    stTranscription = ld => ld.InternationalBt;
                    break;
                case TextFields.FreeTranslation:
                    stTranscription = ld => ld.FreeTranslation;
                    break;
                default:
                    stTranscription = ld => ld.Vernacular;
                    break;
            }

            StoryStageLogic.ProjectStages eStageToGoTo = StoryStageLogic.ProjectStages.eUndefined;
            StringTransferDelegate stTranslation;
            switch (dlg.TranslationField)
            {
                case TextFields.NationalBt:
                    stTranslation = ld => ld.NationalBt;
                    eStageToGoTo = StoryStageLogic.ProjectStages.eProjFacTypeNationalBT;
                    break;
                case TextFields.InternationalBt:
                    stTranslation = ld => ld.InternationalBt;
                    eStageToGoTo = StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT;
                    break;
                case TextFields.FreeTranslation:
                    stTranslation = ld => ld.FreeTranslation;
                    eStageToGoTo = StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation;
                    break;
                default:
                    if (_bNagOnceSayMoreImport)
                    {
                        _bNagOnceSayMoreImport = false;
                        LocalizableMessageBox.Show(
                            Localizer.Str(
                                "Import from Saymore will import both the Saymore 'Transcription' data (usually into the OSE 'Story Language' fields) and the Saymore 'Translation' data (into one of the OSE 'back-translation' fields), but you don't have a 'back-translation' field enabled. To enable one, click 'Project', 'Settings' and check another box in the 'Story' column of the 'Languages' tab."),
                            OseCaption);
                        Debug.Assert(false, "wasn't expecting any other value for BT");
                    }
                    stTranslation = ld => ld.FreeTranslation; // gotta put it somewhere
                    break;
            }

            StoryData theStory;
            if (dlg.SaymoreImportType == SayMoreImportForm.SaymoreImportTypes.NewStory)
            {
                theStory = AddNewStoryAfter(dlg.StoryName, dlg.FullRecordingFileSpec, dlg.Crafter);
                if (theStory == null) // cancelled
                    return;
                theStory.Verses.RemoveAt(0);
            }
            else
            {
                theStory = TheCurrentStory;
                Debug.Assert(theStory != null); // shouldn't be possible to be null
                eStageToGoTo = (dlg.SaymoreImportType == SayMoreImportForm.SaymoreImportTypes.Retelling)
                                    ? StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1
                                    : StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1;
            }

            // for the answers, we have to do this a totally different way
            if (dlg.SaymoreImportType == SayMoreImportForm.SaymoreImportTypes.Answers)
            {
                SaymoreImportToAnswers(theStory, dlg.VernacularLines, dlg.BackTranslationLines, 
                                       stTranscription, stTranslation);
            }
            else
            {
                var nLen = Math.Max(dlg.VernacularLines.Count, dlg.BackTranslationLines.Count);

                var nLineIndex = 0;
                var bCreateNewStory = (dlg.SaymoreImportType == SayMoreImportForm.SaymoreImportTypes.NewStory);
                for (var i = 0; i < nLen; i++)
                {
                    var vernacular = GetSafeValue(dlg.VernacularLines, i);
                    var backTr = GetSafeValue(dlg.BackTranslationLines, i);
                    VerseData newVerse;
                    if (bCreateNewStory || (theStory.Verses.Count <= nLineIndex))
                    {
                        newVerse = GetNewVerse(strUnsGuid, theStory, bCreateNewStory);
                    }
                    else
                    {
                        // skip over hidden lines
                        while (!(newVerse = theStory.Verses[nLineIndex++]).IsVisible)
                        {
                            if (theStory.Verses.Count > nLineIndex)
                                continue;

                            newVerse = GetNewVerse(strUnsGuid, theStory, false);
                            break;
                        }
                    }

                    Debug.Assert(lineData != null, "lineData != null");
                    stTranscription(lineData(newVerse)).SetValue(vernacular);
                    stTranslation(lineData(newVerse)).SetValue(backTr);
                }
            }

            Modified = true;

            // set the state to whatever field we put the bt in (which enables the view)
            if (eStageToGoTo == StoryStageLogic.ProjectStages.eUndefined)
            {
                InitAllPanes();
                return;
            }

            SetNextStateAdvancedOverride(eStageToGoTo, false);
        }

        private void SaymoreImportToAnswers(StoryData theStory, 
                                            List<string> vernacularLines, List<string> backTranslationLines, 
                                            StringTransferDelegate stTranscription, StringTransferDelegate stTranslation)
        {
            // Start by adding Answer boxes for all the TQs using our normal way
            if (!AddInferenceTest())
                return;

            // find the TQs and start putting the lines we're importing into a new answer box
            var nIndex = 0;
            SaymoreImportAnswers(theStory.Verses.FirstVerse, vernacularLines, backTranslationLines, 
                                 stTranscription, stTranslation, ref nIndex);
            foreach (var aVerseData in theStory.Verses)
                SaymoreImportAnswers(aVerseData, vernacularLines, backTranslationLines,
                                     stTranscription, stTranslation, ref nIndex);
        }

        private void SaymoreImportAnswers(VerseData aVerse, List<string> vernacularLines, List<string> backTranslationLines, StringTransferDelegate stTranscription, StringTransferDelegate stTranslation, ref int nIndex)
        {
            foreach (var lineData in aVerse.TestQuestions.Select(testQuestion => testQuestion.Answers.Last()))
            {
                var vernacular = GetSafeValue(vernacularLines, nIndex);
                var backTr = GetSafeValue(backTranslationLines, nIndex);
                nIndex++;

                // now set the proper fields
                stTranscription(lineData).SetValue(vernacular);
                stTranslation(lineData).SetValue(backTr);
            }
        }

        private static VerseData GetNewVerse(string strUnsGuid, StoryData theStory, bool bCreateNewStory)
        {
            var newVerse = new VerseData();
            theStory.Verses.Add(newVerse);

            if (!bCreateNewStory)
                newVerse.Retellings.TryAddNewLine(strUnsGuid);
            return newVerse;
        }

        private static string GetSafeValue(IList<string> lst, int i)
        {
            return (lst.Count > i)
                       ? lst[i]
                       : null;
        }

        private void ButtonMoveToNextLineClick(object sender, EventArgs e)
        {
            var nLineIndex = (int)linkLabelVerseBT.Tag;
            nLineIndex++;
            while ((nLineIndex < TheCurrentStory.Verses.Count) && !TheCurrentStory.Verses[nLineIndex - 1].IsVisible)
                nLineIndex++;
            FocusOnVerse(nLineIndex, true, true);
        }

        void ButtonMoveToPrevLineClick(object sender, EventArgs e)
        {
            var nLineIndex = (int)linkLabelVerseBT.Tag;
            if (nLineIndex > 0)
                nLineIndex--;
            while ((nLineIndex > 0) && !TheCurrentStory.Verses[nLineIndex].IsVisible)
                nLineIndex--;
            FocusOnVerse(nLineIndex, true, true);
        }

        private void advancedSaveTimeoutAsSilentlyAsPossibleMenu_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Default.DoAutoSaveSilently = advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked;
            Settings.Default.Save();
        }

        private void StoryCopyToAnotherProjectMenuClick(object sender, EventArgs e)
        {
            // iterate thru the verses and copy them to the clipboard
            Debug.Assert(TheCurrentStory != null);
            var xmlOfStoryToCopy = StoryProject.GetXmlToCopyStory(TheCurrentStory);
            Clipboard.SetDataObject(xmlOfStoryToCopy.ToString());
        }

        private void StoryCopyFromAnotherProjectMenuClick(object sender, EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData == null)
                return;

            string strData;
            if (!iData.GetDataPresent(DataFormats.UnicodeText) ||
                ((strData = (string)iData.GetData(DataFormats.UnicodeText)) == null) ||
                !strData.Contains(StoryProjectData.CstrElementOseStoryToCopy))
                return;

            var theStoryToCopyPlusMembersXElement = XElement.Parse(strData);
            var theStoryToCopyXElement = theStoryToCopyPlusMembersXElement.Element(StoryData.CstrElementNameStory);

            // find all of the descendent attributes for 'memberId'
            if (theStoryToCopyXElement == null)
                return;

            var theStoryToCopyXmlNode = theStoryToCopyXElement.GetXmlNode();
            var theStoryToCopy = new StoryData(theStoryToCopyXmlNode.FirstChild, StoryProject.ProjSettings.ProjectFolder);
            theStoryToCopy = new StoryData(theStoryToCopy); // yes, we have to do this again, because the XmlNode ctor won't re-do the guids

            // remove references to participants we don't care about to minimize the # of memberId guid's we'd have to copy over
            theStoryToCopy.CraftingInfo.BackTranslator =
                theStoryToCopy.CraftingInfo.OutsideEnglishBackTranslator =
                theStoryToCopy.CraftingInfo.ProjectFacilitator = null;

            // before copying over the reference members, let's see what the user wants to copy: 
            var dlg = new ViewEnableForm(this, StoryProject.ProjSettings, theStoryToCopy, false);
            dlg.InitializeForQueryingFieldsToCopy();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            // get the list of stuff we're going to delete, so we can confirm it
            var mapToFieldsToDelete = new Dictionary<string, ResetValue>();
            var fieldsToDeleteChosen = dlg.ViewSettings;
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                mapToFieldsToDelete.Add(dlg.checkBoxLangVernacular.Text, DeleteVernacular);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField))
                mapToFieldsToDelete.Add(dlg.checkBoxLangNationalBT.Text, DeleteNationalBt);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField))
                mapToFieldsToDelete.Add(dlg.checkBoxLangInternationalBT.Text, DeleteInternationalBt);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                mapToFieldsToDelete.Add(dlg.checkBoxLangFreeTranslation.Text, DeleteFreeTranslation);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields))
                mapToFieldsToDelete.Add(dlg.checkBoxAnchors.Text, DeleteAnchors);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps))
                mapToFieldsToDelete.Add(dlg.checkBoxExegeticalNotes.Text, DeleteExegeticalHelps);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions))
                mapToFieldsToDelete.Add(dlg.checkBoxGeneralTestingQuestions.Text, DeleteStoryTestingQuestionsAndAnswers);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
                mapToFieldsToDelete.Add(dlg.checkBoxStoryTestingQuestions.Text, DeleteStoryTestingQuestionsAndAnswers);
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
                mapToFieldsToDelete.Add(dlg.checkBoxAnswers.Text, null);   // special case
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.RetellingFields))
                mapToFieldsToDelete.Add(dlg.checkBoxRetellings.Text, null);   // special case
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields))
            {
                theStoryToCopy.CraftingInfo.Consultant = null;      // so we can start with a new one in this project
                mapToFieldsToDelete.Add(dlg.checkBoxConsultantNotes.Text, DeleteConsultantNotes);
            }
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields))
            {
                theStoryToCopy.CraftingInfo.Coach = null;           // so we can start with a new one in this project
                mapToFieldsToDelete.Add(dlg.checkBoxCoachNotes.Text, DeleteCoachNotes);
            }
            if (!fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.HiddenStuff))
                mapToFieldsToDelete.Add(dlg.checkBoxShowHidden.Text, DeleteHiddenLines);

            // if none were chosen for deletion, then nothing to delete (duh)
            if (mapToFieldsToDelete.Count > 0)
                DeleteFieldsFromStory(theStoryToCopy, dlg, mapToFieldsToDelete);

            // now that we've removed anything the user didn't want to copy, let's see what members are still being referenced
            var theMembersInTheOtherProjectXElement = theStoryToCopyPlusMembersXElement.Element(TeamMembersData.CstrElementLabelMembers);
            var theMembersInTheOtherProjectXmlNode = theMembersInTheOtherProjectXElement.GetXmlNode();
            var theMembersInTheOtherProject = new TeamMembersData(theMembersInTheOtherProjectXmlNode);

#if !UsingOldMethodToDetermineMembersToCopy
            foreach (var aMember in theMembersInTheOtherProject.Values)
            {
                if (theStoryToCopy.DoesReferenceExist(aMember))
                {
                    var strMemberId = aMember.MemberGuid;
                    var strMembersName = aMember.Name;
                    TeamMemberData theMemberInThisProjectWithTheSameName;
                    if (!StoryProject.TeamMembers.TryGetValue(strMembersName, out theMemberInThisProjectWithTheSameName))
                    {
                        StoryProject.TeamMembers.Add(strMembersName, aMember);
                    }
                    else if (strMemberId != theMemberInThisProjectWithTheSameName.MemberGuid)
                    {
                        theStoryToCopy.ReplaceMemberId(strMemberId, theMemberInThisProjectWithTheSameName.MemberGuid);
                    }
                }
            }
#else
            // find all of the guids to members (so we can make sure they're in the target project also
            var elemsWithMemberIds = theStoryToCopyXElement.Descendants()
                .Where(elem => elem.Attribute("memberID") != null);

            foreach (var elemWithMemberId in elemsWithMemberIds)
            {
                Debug.Assert(elemWithMemberId.Attribute("memberID") != null, "elemWithMemberId.Attribute('memberID') != null");
                var strMemberId = elemWithMemberId.Attribute("memberID").Value;
                var strMembersName = theMembersInTheOtherProject.GetNameFromMemberId(strMemberId);
                TeamMemberData theMemberInThisProjectWithTheSameName;
                if (!StoryProject.TeamMembers.TryGetValue(strMembersName, out theMemberInThisProjectWithTheSameName))
                    StoryProject.TeamMembers.Add(strMembersName,
                                                 theMembersInTheOtherProject.GetMemberFromId(strMemberId));
                else if (strMemberId != theMemberInThisProjectWithTheSameName.MemberGuid)
                    elemWithMemberId.SetAttributeValue("memberID", theMemberInThisProjectWithTheSameName.MemberGuid);
            }
#endif

            var nIndexOfCurrentStory = TheCurrentStoriesSet.IndexOf(TheCurrentStory);
            if (TheCurrentStoriesSet.Any(s => s.Name == theStoryToCopy.Name))
            {
                string strStoryName = theStoryToCopy.Name;
                if (!AddNewStoryGetIndex(ref nIndexOfCurrentStory, ref strStoryName, false))
                    return;

                theStoryToCopy.Name = strStoryName;
            }

            nIndexOfCurrentStory = Math.Min(nIndexOfCurrentStory + 1, TheCurrentStoriesSet.Count);
            InsertNewStoryAdjustComboBox(theStoryToCopy, nIndexOfCurrentStory);

            var theProjectSettingsInTheOtherProjectXElement = theStoryToCopyPlusMembersXElement.Element(ProjectSettings.CstrElementLabelLanguages);
            var theProjectSettingsInTheOtherProjectXmlNode = theProjectSettingsInTheOtherProjectXElement.GetXmlNode().FirstChild;
            var theProjectSettingsInTheOtherProject = new ProjectSettings(theProjectSettingsInTheOtherProjectXmlNode, null);
            if ((theProjectSettingsInTheOtherProject.Vernacular.HasData ^ StoryProject.ProjSettings.Vernacular.HasData) ||
                (theProjectSettingsInTheOtherProject.NationalBT.HasData ^ StoryProject.ProjSettings.NationalBT.HasData) ||
                (theProjectSettingsInTheOtherProject.InternationalBT.HasData ^ StoryProject.ProjSettings.InternationalBT.HasData) ||
                (theProjectSettingsInTheOtherProject.FreeTranslation.HasData ^ StoryProject.ProjSettings.FreeTranslation.HasData) ||
                (theProjectSettingsInTheOtherProject.Vernacular.HasData && (theProjectSettingsInTheOtherProject.Vernacular.LangName != StoryProject.ProjSettings.Vernacular.LangName)) ||
                (theProjectSettingsInTheOtherProject.NationalBT.HasData && (theProjectSettingsInTheOtherProject.NationalBT.LangName != StoryProject.ProjSettings.NationalBT.LangName)) ||
                (theProjectSettingsInTheOtherProject.InternationalBT.HasData && (theProjectSettingsInTheOtherProject.InternationalBT.LangName != StoryProject.ProjSettings.InternationalBT.LangName)) ||
                (theProjectSettingsInTheOtherProject.FreeTranslation.HasData && (theProjectSettingsInTheOtherProject.FreeTranslation.LangName != StoryProject.ProjSettings.FreeTranslation.LangName)))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Warning: the source and target project have differences which can affect how the story is copied. For example, if the source project was configured with a 'National language back-translation', but this project isn't, then the 'National BT' text won't be visible until/unless you configure a 'National BT' field in the 'Project', 'Settings' dialog, 'Languages' tab. Or if one of the languages (e.g. 'Story language') is different between this project and the source project, that can cause display issues -- e.g. a different font might be used in this project for which the source project text won't display correctly..."),
                    OseCaption);
            }
        }

        private static void DeleteFieldsFromStory(StoryData theStoryToCopy, ViewEnableForm dlg, Dictionary<string, ResetValue> mapToFieldsToDelete)
        {
            foreach (var kvp in mapToFieldsToDelete)
            {
                var resetFunction = kvp.Value;

                // do special cases first
                if (kvp.Key == dlg.checkBoxRetellings.Text)
                {
                    for (var i = theStoryToCopy.CraftingInfo.TestersToCommentsRetellings.Count - 1; i >= 0; i--)
                        theStoryToCopy.DeleteRetellingTestResults(i, theStoryToCopy.CraftingInfo.TestersToCommentsRetellings[i].MemberId);
                }
                else if (kvp.Key == dlg.checkBoxAnswers.Text)
                {
                    for (var i = theStoryToCopy.CraftingInfo.TestersToCommentsTqAnswers.Count - 1; i >= 0; i--)
                        theStoryToCopy.DeleteAnswerTestResults(i, theStoryToCopy.CraftingInfo.TestersToCommentsTqAnswers[i].MemberId);
                }
                else if (kvp.Key == dlg.checkBoxGeneralTestingQuestions.Text)
                {
                    // then we *only* do the first verse
                    resetFunction(theStoryToCopy.Verses.FirstVerse);
                }
                else
                {
                    foreach (var aVerse in theStoryToCopy.Verses)
                    {
                        resetFunction(aVerse);
                    }
                }
            }
        }

        private void AdvancedSwapDataColumnsClick(object sender, EventArgs e)
        {
            if (!CheckForProperEditToken())
            {
                LocalizableMessageBox.Show(
                    Localizer.Str("The current story is not in your turn. If you were to swap fields while the story is in someone else's turn and they make changes, then your changes and theirs would conflict and one of them would be lost. So only attempt to swap columns for a story that is already in your turn (and don't use 'Advanced', 'Change Turn without checks...' unless you're sure the other person isn't going to make changes). You can click 'Panorama', 'Show' to see the list of all the stories in the set to see which ones are in your turn (highlighted)."),
                    OseCaption);
                return;
            }

            if (!SwapColumns())
                return;

            InitializeSwapColumnsAllStories();
        }

        private TextFields _lstColumn1,
                           _lstColumn2,
                           _lstFieldsToSwap = TextFields.StoryLine | TextFields.Retelling |
                                              TextFields.TestQuestion | TextFields.TestQuestionAnswer;
        private bool SwapColumns()
        {
            var dlg = new SwapColumnsForm(this)
                          {
                              Column1 = _lstColumn1,
                              Column2 = _lstColumn2,
                              FieldsToSwap = _lstFieldsToSwap
                          };

            var res = dlg.ShowDialog();
            if (res == DialogResult.Cancel)
            {
                _lstToSwap = null;
                return false;
            }
            else if (res == DialogResult.Ignore)    // = Skip
            {
                return true;
            }

            // else, 'ok', means swap the columns
            System.Diagnostics.Debug.Assert(res == DialogResult.OK);
            TheCurrentStory.SwapColumns(_lstColumn1 = dlg.Column1,
                                        _lstColumn2 = dlg.Column2, 
                                        _lstFieldsToSwap = dlg.FieldsToSwap);
            Modified = true;
            InitAllPanes();
            return true;
        }

        private List<string> _lstToSwap;

        private void InitializeSwapColumnsAllStories()
        {
            // check to see if there are any other stories in this same user's turn
            // keep track of the ones we can do and in the 'LoadStory' routine, bring up the dialog for the next story after it's loaded.
            _lstToSwap = TheCurrentStoriesSet.Where(s =>
            {
                return (s.Name == TheCurrentStory.Name)
                            ? false
                            : LoggedOnMember.IsEditAllowed(s);
            })
            .Select(s => s.Name)
            .ToList();

            CheckForNextStoryColumnSwap();
        }

        private void CheckForNextStoryColumnSwap()
        {
            if ((_lstToSwap == null) || (_lstToSwap.Count <= 0))
            {
                _lstToSwap = null;
                return;
            }

            var res = LocalizableMessageBox.Show(
                Localizer.Str(
                    "There are other stories in this project that are in your turn. Would you like to swap columns in those stories also?"),
                OseCaption,
                MessageBoxButtons.YesNoCancel);

            if (res != DialogResult.Yes)
            {
                _lstToSwap = null;
                return;
            }

            var nextStoryName = _lstToSwap.First();
            _lstToSwap.Remove(nextStoryName);
            JumpToStory(nextStoryName);
        }

        private void advancedOneStoryProjectMetaData_Click(object sender, EventArgs e)
        {
            if (LaunchOsMetaDataDialog(StoryProject))
                StoryProject.SaveProjectMetaData(LoggedOnMember);
        }

        public static bool LaunchOsMetaDataDialog(StoryProjectData storyProject)
        {

            var dlg = new OsMetaDataForm(storyProject.OsMetaData ?? storyProject.InitializeMetaData);
            return (dlg.ShowDialog() == DialogResult.OK);
        }

        private void viewRetellingsChooseWhichRetellingsItem_Click(object sender, EventArgs e)
        {
            ChooseWhichTestsToDisplay(TheCurrentStory.CraftingInfo.TestersToCommentsRetellings, Localizer.Str("Retelling"), viewRetellingsMenu);
        }

        private void ChooseWhichTestsToDisplay(TestInfo tests, string strLable, ToolStripMenuItem mainMenu)
        {
            var dlg = new ChooseWhichTestToDisplayForm(StoryProject, tests, strLable);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (dlg.ListOfTesteeIdsToShow.Count == 0)
                mainMenu.Checked = false;
            else
            {
                tests.ListOfMemberIdsToDisplay = dlg.ListOfTesteeIdsToShow;
                mainMenu.Checked = true;    // in case it wasn't checked
            }
            InitAllPanes();
        }

        private void viewAnswersChooseWhichAnswersItem_Click(object sender, EventArgs e)
        {
            ChooseWhichTestsToDisplay(TheCurrentStory.CraftingInfo.TestersToCommentsTqAnswers, Localizer.Str("Answer"), viewStoryTestingQuestionAnswersMenu);
        }

        private void advancedUseDialogToPreviewConNotes_Click(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Settings.Default.UsePreviewDialogWhenAddingConNotes = (sender as ToolStripMenuItem).Checked;
                Settings.Default.Save();
            }
        }

        private void tasksToolStripMenu_Click(object sender, EventArgs e)
        {
            ShowTaskBarForm();
        }

        private void panoramaFirstStoryMenu_Click(object sender, EventArgs e)
        {
            GoToFirstStory();
        }

        private void panoramaPreviousStoryMenu_Click(object sender, EventArgs e)
        {
            GoToPreviousStory();
        }

        private void editDeleteChooseFieldsMenu_Click(object sender, EventArgs e)
        {
            var dlg = new ViewEnableForm(this, StoryProject.ProjSettings, TheCurrentStory, false);
            dlg.InitializeForQueryingFieldsSelected();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // get the list of stuff we're going to delete, so we can confirm it
                var mapToFieldsToDelete = new Dictionary<string, ResetValue>();
                var fieldsToDeleteChosen = dlg.ViewSettings;
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                    mapToFieldsToDelete.Add(dlg.checkBoxLangVernacular.Text, DeleteVernacular);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField))
                    mapToFieldsToDelete.Add(dlg.checkBoxLangNationalBT.Text, DeleteNationalBt);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField))
                    mapToFieldsToDelete.Add(dlg.checkBoxLangInternationalBT.Text, DeleteInternationalBt);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                    mapToFieldsToDelete.Add(dlg.checkBoxLangFreeTranslation.Text, DeleteFreeTranslation);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields))
                    mapToFieldsToDelete.Add(dlg.checkBoxAnchors.Text, DeleteAnchors);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps))
                    mapToFieldsToDelete.Add(dlg.checkBoxExegeticalNotes.Text, DeleteExegeticalHelps);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions))
                    mapToFieldsToDelete.Add(dlg.checkBoxGeneralTestingQuestions.Text, DeleteStoryTestingQuestionsAndAnswers);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
                    mapToFieldsToDelete.Add(dlg.checkBoxStoryTestingQuestions.Text, DeleteStoryTestingQuestionsAndAnswers);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
                    mapToFieldsToDelete.Add(dlg.checkBoxAnswers.Text, null);   // special case
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.RetellingFields))
                    mapToFieldsToDelete.Add(dlg.checkBoxRetellings.Text, null);   // special case
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields))
                    mapToFieldsToDelete.Add(dlg.checkBoxConsultantNotes.Text, DeleteConsultantNotes);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields))
                    mapToFieldsToDelete.Add(dlg.checkBoxCoachNotes.Text, DeleteCoachNotes);
                if (fieldsToDeleteChosen.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.HiddenStuff))
                    mapToFieldsToDelete.Add(dlg.checkBoxShowHidden.Text, DeleteHiddenLines);

                if (mapToFieldsToDelete.Count == 0)
                    return;

                var strMessage = mapToFieldsToDelete.Keys.Aggregate(Localizer.Str("Are you sure you want to delete:"),
                                                                     (curr, next) => { return curr + Environment.NewLine + next; });
                if (LocalizableMessageBox.Show(strMessage, OseCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    DeleteFieldsFromStory(TheCurrentStory, dlg, mapToFieldsToDelete);

                    if (mapToFieldsToDelete.ContainsKey(dlg.checkBoxConsultantNotes.Text) ||
                        mapToFieldsToDelete.ContainsKey(dlg.checkBoxCoachNotes.Text))
                    {
                        InitAllPanes();
                    }
                    else
                    {
                        ReInitVerseControls();
                    }

                    Modified = true;
                }
            }
        }

        private void panoramaNextStoryMenu_Click(object sender, EventArgs e)
        {
            GoToNextStory();
        }

        private void panoramaLastStoryMenu_Click(object sender, EventArgs e)
        {
            GoToLastStory();
        }
    }

    public static class MyExtensions
    {
        public static XElement GetXElement(this XmlNode node)
        {
            var xDoc = new XDocument();
            using (var xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        public static XmlNode GetXmlNode(this XElement element)
        {
            using (var xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}