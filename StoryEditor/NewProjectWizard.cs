using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Keyboarding;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class NewProjectWizard : TopForm
    {
        protected StoryProjectData _storyProjectData;
        public TeamMemberData LoggedInMember;

        private bool _bStartedWithVernacular;
        private bool _bStartedWithNationalBt;
        private bool _bStartedWithInternationalBt;
        private bool _bStartedWithFreeTranslation;

        private NewProjectWizard()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public NewProjectWizard(StoryProjectData storyProjectData)
            : base(true)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            _storyProjectData = storyProjectData;

            if ((_storyProjectData.ProjSettings != null) && 
                _storyProjectData.ProjSettings.IsConfigured)
            {
                ProjSettings = _storyProjectData.ProjSettings;
                ProjectName = ProjSettings.ProjectName;
                string strDummy;
                if (!String.IsNullOrEmpty(ProjSettings.HgRepoUrlHost) ||
                    Program.GetHgRepoParameters(ProjectName, out strDummy, out strDummy, out strDummy))
                    checkBoxUseInternetRepo.Checked = true;
            }
            
            if (!checkBoxUseInternetRepo.Checked)
                tabControl.TabPages.Remove(tabPageInternetRepository);

            if ((_storyProjectData.ProjSettings == null)
                || !_storyProjectData.ProjSettings.Vernacular.HasData)
                tabControl.TabPages.Remove(tabPageLanguageVernacular);
            else
            {
                _bStartedWithVernacular = checkBoxLanguageVernacular.Checked = true;
                checkBoxRetellingsVernacular.Checked = _storyProjectData.ProjSettings.ShowRetellings.Vernacular;
                checkBoxTestQuestionsVernacular.Checked = _storyProjectData.ProjSettings.ShowTestQuestions.Vernacular;
                checkBoxAnswersVernacular.Checked = _storyProjectData.ProjSettings.ShowAnswers.Vernacular;
            }

            if ((_storyProjectData.ProjSettings == null)
                || !_storyProjectData.ProjSettings.NationalBT.HasData)
                tabControl.TabPages.Remove(tabPageLanguageNationalBT);
            else
            {
                _bStartedWithNationalBt = checkBoxLanguageNationalBT.Checked = true;
                checkBoxRetellingsNationalBT.Checked = _storyProjectData.ProjSettings.ShowRetellings.NationalBt;
                checkBoxTestQuestionsNationalBT.Checked = _storyProjectData.ProjSettings.ShowTestQuestions.NationalBt;
                checkBoxAnswersNationalBT.Checked = _storyProjectData.ProjSettings.ShowAnswers.NationalBt;
            }

            if ((_storyProjectData.ProjSettings != null)
                && !_storyProjectData.ProjSettings.InternationalBT.HasData)
            {
                checkBoxLanguageInternationalBT.Checked = false;
                tabControl.TabPages.Remove(tabPageLanguageEnglishBT);
            }
            else if (_storyProjectData.ProjSettings != null)
            {
                _bStartedWithInternationalBt
                // checkBoxLanguageInternationalBT.Checked
                    = true;
                checkBoxRetellingsInternationalBT.Checked = _storyProjectData.ProjSettings.ShowRetellings.InternationalBt;
                checkBoxTestQuestionsInternationalBT.Checked =
                    _storyProjectData.ProjSettings.ShowTestQuestions.InternationalBt;
                checkBoxAnswersInternationalBT.Checked = _storyProjectData.ProjSettings.ShowAnswers.InternationalBt;
            }

            if ((_storyProjectData.ProjSettings == null)
                || !_storyProjectData.ProjSettings.FreeTranslation.HasData)
                tabControl.TabPages.Remove(tabPageLanguageFreeTranslation);
            else
            {
                _bStartedWithFreeTranslation = 
                    checkBoxLanguageFreeTranslation.Checked = true;
            }

            UpdateTabPageAIBT();
        }

        private void ProcessNext()
        {
            if (tabControl.SelectedTab == tabPageProjectName)
            {
                if (String.IsNullOrEmpty(ProjectName))
                    throw new UserException(Localizer.Str("Unable to create a project without a project name!"),
                        textBoxProjectName, tabPageProjectName);

                if (ProjSettings == null)
                {
                    // this means that we are doing "new" (as opposed to "edit" settings)
                    // first check if this means we have to overwrite a project
                    string strFilename = ProjectSettings.GetDefaultProjectFilePath(ProjectName);
                    if (File.Exists(strFilename))
                    {
                        DialogResult res = StoryEditor.QueryOverwriteProject(ProjectName);
                        if (res != DialogResult.Yes)
                            return;

                        RemoveProject(strFilename, ProjectName);
                    }

                    ProjSettings = new ProjectSettings((string)null, ProjectName, false);

                    // make sure the 'new' folder exists
                    Directory.CreateDirectory(ProjSettings.ProjectFolder);
                }
                else
                    ProjSettings.ProjectName = ProjectName;

                string strUsername, strRepoUrl, strPassword;
                UrlBase = _storyProjectData.GetHgRepoUsernamePassword(ProjectName, LoggedInMember,
                                                                      out strUsername, 
                                                                      out strPassword, 
                                                                      out strRepoUrl)
                              ? strRepoUrl
                              : Program.LookupRepoUrlHost(Properties.Resources.IDS_DefaultRepoServer);

                // these *might* have been initialized even if the call to GetHg... fails
                HgUsername = strUsername;
                HgPassword = strPassword;

                InitLanguageControls(tabPageLanguageVernacular, ProjSettings.Vernacular);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideVernacularKeyboard)))
                    comboBoxKeyboardVernacular.SelectedItem = LoggedInMember.OverrideVernacularKeyboard;

                InitLanguageControls(tabPageLanguageNationalBT, ProjSettings.NationalBT);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideNationalBTKeyboard)))
                    comboBoxKeyboardNationalBT.SelectedItem = LoggedInMember.OverrideNationalBTKeyboard;

                InitLanguageControls(tabPageLanguageEnglishBT, ProjSettings.InternationalBT);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideInternationalBTKeyboard)))
                    comboBoxKeyboardEnglishBT.SelectedItem = LoggedInMember.OverrideInternationalBTKeyboard;

                InitLanguageControls(tabPageLanguageFreeTranslation, ProjSettings.FreeTranslation);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideFreeTranslationKeyboard)))
                    comboBoxKeyboardFreeTranslation.SelectedItem = LoggedInMember.OverrideFreeTranslationKeyboard;

                tabControl.SelectedIndex++;
            }
            else if (tabControl.SelectedTab == tabPageInternetRepository)
            {
                // do we need to check whether it's available?
                if ((LoggedInMember != null)
                    && (!String.IsNullOrEmpty(HgUsername))
                    && (!String.IsNullOrEmpty(HgPassword)))
                {
                    LoggedInMember.HgUsername = HgUsername;
                    LoggedInMember.HgPassword = HgPassword;
                }

                ProjSettings.HgRepoUrlHost = UrlBase;
                Program.SetHgParameters(ProjSettings.ProjectFolder,
                    ProjSettings.ProjectName, Url, HgUsername);

                tabControl.SelectedIndex++;
            }
            else if (tabControl.SelectedTab == tabPageLanguages)
            {
                if (!checkBoxLanguageVernacular.Checked
                    && !checkBoxLanguageNationalBT.Checked
                    && !checkBoxLanguageInternationalBT.Checked
                    && !checkBoxLanguageFreeTranslation.Checked)
                {
                    throw new UserException(Localizer.Str("A project must have at least one language selected"),
                        checkBoxLanguageInternationalBT, tabPageLanguages);
                }

                // if we're editing the settings and we didn't start with Vern, but now have it...
                SetDefaultAllowedForNewField(_bStartedWithVernacular,
                                             checkBoxLanguageVernacular, 
                                             TasksPf.TaskSettings.VernacularLangFields);

                if (!checkBoxLanguageVernacular.Checked)
                {
                    System.Diagnostics.Debug.Assert(!checkBoxRetellingsVernacular.Checked
                                                    && !checkBoxTestQuestionsVernacular.Checked
                                                    && !checkBoxAnswersVernacular.Checked);
                    ProjSettings.Vernacular.HasData = false;
                }
                else
                {
                    if (String.IsNullOrEmpty(textBoxLanguageNameVernacular.Text))
                        textBoxLanguageNameVernacular.Text = (!String.IsNullOrEmpty(ProjSettings.Vernacular.LangName))
                                                                 ? ProjSettings.Vernacular.LangName 
                                                                 : null;
                }

                SetDefaultAllowedForNewField(_bStartedWithNationalBt,
                                             checkBoxLanguageNationalBT,
                                             TasksPf.TaskSettings.NationalBtLangFields);

                if (!checkBoxLanguageNationalBT.Checked)
                {
                    System.Diagnostics.Debug.Assert(!checkBoxRetellingsNationalBT.Checked
                                                    && !checkBoxTestQuestionsNationalBT.Checked
                                                    && !checkBoxAnswersNationalBT.Checked);
                    ProjSettings.NationalBT.HasData = false;
                }

                SetDefaultAllowedForNewField(_bStartedWithInternationalBt,
                                             checkBoxLanguageInternationalBT,
                                             TasksPf.TaskSettings.InternationalBtFields);

                if (checkBoxLanguageInternationalBT.Checked)
                {
                    if (String.IsNullOrEmpty(textBoxLanguageNameEnglishBT.Text)
                        && !ProjSettings.InternationalBT.HasData)
                        textBoxLanguageNameEnglishBT.Text = ProjectSettings.DefInternationalLanguageName;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(!checkBoxRetellingsInternationalBT.Checked
                                                    && !checkBoxTestQuestionsInternationalBT.Checked
                                                    && !checkBoxAnswersInternationalBT.Checked);
                    _storyProjectData.TeamMembers.HasOutsideEnglishBTer =
                        ProjSettings.InternationalBT.HasData = false;
                }

                SetDefaultAllowedForNewField(_bStartedWithFreeTranslation,
                                             checkBoxLanguageFreeTranslation,
                                             TasksPf.TaskSettings.FreeTranslationFields);

                if (checkBoxLanguageFreeTranslation.Checked)
                {
                    if (String.IsNullOrEmpty(textBoxLanguageNameFreeTranslation.Text)
                        && !ProjSettings.FreeTranslation.HasData)
                        textBoxLanguageNameFreeTranslation.Text = ProjectSettings.DefInternationalLanguageName;
                    
                    // make them different...
                    if (textBoxLanguageNameEnglishBT.Text == ProjectSettings.DefInternationalLanguageName)
                        textBoxLanguageNameFreeTranslation.Text = ProjectSettings.DefInternationalLanguageName + 
                            Localizer.Str(" FT");
                }
                else
                    ProjSettings.FreeTranslation.HasData = false;

                // can't have an outside english bter, if we don't have an English BT or only an English BT
                //  (the PF has to put something in!)
                checkBoxOutsideEnglishBackTranslator.Enabled = (
                                                   (checkBoxLanguageInternationalBT.Checked ||
                                                    checkBoxLanguageFreeTranslation.Checked)
                                                   &&
                                                   (checkBoxLanguageNationalBT.Checked ||
                                                    checkBoxLanguageVernacular.Checked));

                checkBoxOutsideEnglishBackTranslator.Checked = _storyProjectData.TeamMembers.HasOutsideEnglishBTer;
                radioButtonIndependentConsultant.Checked = _storyProjectData.TeamMembers.HasIndependentConsultant;

                // now deal with the AdaptIt BT helpers (doing this here, because by now LoggedOnMember
                //  will be initialized)
                SetAdaptItControlVisibility(checkBoxLanguageVernacular,
                                            checkBoxLanguageNationalBT,
                                            labelAdaptItVernacularToNationalBt,
                                            adaptItConfigCtrlVernacularToNationalBt,
                                            0);

                SetAdaptItControlVisibility(checkBoxLanguageVernacular,
                                            checkBoxLanguageInternationalBT,
                                            labelAdaptItVernacularToInternationalBt,
                                            adaptItConfigCtrlVernacularToInternationalBt,
                                            1);

                SetAdaptItControlVisibility(checkBoxLanguageNationalBT,
                                            checkBoxLanguageInternationalBT,
                                            labelAdaptItNationalBtToInternationalBt,
                                            adaptItConfigCtrlNationalBtToInternationalBt,
                                            2);

                tabControl.SelectedIndex++;
            }
            else if (tabControl.SelectedTab == tabPageLanguageVernacular)
            {
                bool bRtfOverride = false;
                string strKeyboardOverride = null;
                ProcessLanguageTab(comboBoxKeyboardVernacular, ProjSettings.Vernacular, checkBoxIsRTLVernacular,
                    textBoxLanguageNameVernacular, textBoxEthCodeVernacular, textBoxSentFullStopVernacular,
                    ref strKeyboardOverride, ref bRtfOverride);
                if (LoggedInMember != null)
                {
                    LoggedInMember.OverrideVernacularKeyboard = strKeyboardOverride;
                    LoggedInMember.OverrideRtlVernacular = bRtfOverride;
                }
            }
            else if (tabControl.SelectedTab == tabPageLanguageNationalBT)
            {
                bool bRtfOverride = false;
                string strKeyboardOverride = null;
                ProcessLanguageTab(comboBoxKeyboardNationalBT, ProjSettings.NationalBT, checkBoxIsRTLNationalBT,
                    textBoxLanguageNameNationalBT, textBoxEthCodeNationalBT, textBoxSentFullStopNationalBT,
                    ref strKeyboardOverride, ref bRtfOverride);
                if (LoggedInMember != null)
                {
                    LoggedInMember.OverrideNationalBTKeyboard = strKeyboardOverride;
                    LoggedInMember.OverrideRtlNationalBT = bRtfOverride;
                }
            }
            else if (tabControl.SelectedTab == tabPageLanguageEnglishBT)
            {
                bool bRtfOverride = false;
                string strKeyboardOverride = null;
                ProcessLanguageTab(comboBoxKeyboardEnglishBT, ProjSettings.InternationalBT, checkBoxIsRTLEnglishBT,
                    textBoxLanguageNameEnglishBT, textBoxEthCodeEnglishBT, textBoxSentFullStopEnglishBT,
                    ref strKeyboardOverride, ref bRtfOverride);
                if (LoggedInMember != null)
                {
                    LoggedInMember.OverrideInternationalBTKeyboard = strKeyboardOverride;
                    LoggedInMember.OverrideRtlInternationalBT = bRtfOverride;
                }
            }
            else if (tabControl.SelectedTab == tabPageLanguageFreeTranslation)
            {
                bool bRtfOverride = false;
                string strKeyboardOverride = null;
                ProcessLanguageTab(comboBoxKeyboardFreeTranslation, ProjSettings.FreeTranslation, checkBoxIsRTLFreeTranslation,
                    textBoxLanguageNameFreeTranslation, textBoxEthCodeFreeTranslation, textBoxSentFullStopFreeTranslation,
                    ref strKeyboardOverride, ref bRtfOverride);
                if (LoggedInMember != null)
                {
                    LoggedInMember.OverrideFreeTranslationKeyboard = strKeyboardOverride;
                    LoggedInMember.OverrideRtlFreeTranslation = bRtfOverride;
                }
            }
            else if (tabControl.SelectedTab == tabPageMemberRoles)
            {
                if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToNationalBt.Name))
                {
                    ConfigureAdaptItConfig(adaptItConfigCtrlVernacularToNationalBt,
                                           labelAdaptItVernacularToNationalBt,
                                           ProjSettings.VernacularToNationalBt,
                                           ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt,
                                           ProjSettings.Vernacular.LangName,
                                           ProjSettings.NationalBT.LangName);
                }
                if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToInternationalBt.Name))
                {
                    ConfigureAdaptItConfig(adaptItConfigCtrlVernacularToInternationalBt,
                                           labelAdaptItVernacularToInternationalBt,
                                           ProjSettings.VernacularToInternationalBt,
                                           ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt,
                                           ProjSettings.Vernacular.LangName,
                                           ProjSettings.InternationalBT.LangName);
                }
                if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlNationalBtToInternationalBt.Name))
                {
                    ConfigureAdaptItConfig(adaptItConfigCtrlNationalBtToInternationalBt,
                                           labelAdaptItNationalBtToInternationalBt,
                                           ProjSettings.NationalBtToInternationalBt,
                                           ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt,
                                           ProjSettings.NationalBT.LangName,
                                           ProjSettings.InternationalBT.LangName);
                }
                tabControl.SelectedIndex++;
            }
            else if (tabControl.SelectedTab == tabPageAIBT)
            {
            }
        }

        private void SetDefaultAllowedForNewField(bool bStartedWith, 
            CheckBox checkBox, TasksPf.TaskSettings newTaskAllowed)
        {
            if (ProjSettings.IsConfigured
                && !bStartedWith
                && checkBox.Checked)
            {
                // then at least turn on the default
                foreach (var aTeamMember in
                    _storyProjectData.TeamMembers.Values.Where(aTeamMember => TeamMemberData.IsUser(aTeamMember.MemberType, TeamMemberData.UserTypes.ProjectFacilitator)))
                {
                    aTeamMember.DefaultAllowed |= (long)(uint)newTaskAllowed;
                }
            }
        }

        private void SetAdaptItControlVisibility(CheckBox cbSource, CheckBox cbTarget,
            Label label, AdaptItConfigControl ctrlAdaptItConfig, int nRowNum)
        {
            if (cbSource.Checked && cbTarget.Checked)
            {
                if (!tlpAdaptItConfiguration.Controls.ContainsKey(label.Name))
                {
                    tlpAdaptItConfiguration.Controls.Add(label, 0, nRowNum);
                    tlpAdaptItConfiguration.Controls.Add(ctrlAdaptItConfig, 1, nRowNum);
                }
            }
            else if (tlpAdaptItConfiguration.Controls.ContainsKey(label.Name))
            {
                tlpAdaptItConfiguration.Controls.Remove(label);
                tlpAdaptItConfiguration.Controls.Remove(ctrlAdaptItConfig);
            }
        }

        private void ConfigureAdaptItConfig(AdaptItConfigControl ctrl, Label label,
            ProjectSettings.AdaptItConfiguration aiProjectConfig, 
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection,
            string strSourceName, string strTargetName)
        {
            ctrl.Parent = this;
            ctrl.BtDirection = eBtDirection;
            ctrl.SourceLanguageName = strSourceName;
            ctrl.TargetLanguageName = strTargetName;
            ctrl.AdaptItConfiguration = aiProjectConfig;
            label.Text =
                String.Format(Localizer.Str("{0} to {1} adaptation:"),
                              strSourceName,
                              strTargetName);
        }

        internal static void RemoveProject(string strProjectFilename, string strProjectName)
        {
            string strProjectFolder = Path.GetDirectoryName(strProjectFilename);
            System.Diagnostics.Debug.Assert(strProjectFolder == ProjectSettings.GetDefaultProjectPath(strProjectName));

            // they want to delete it (so remove all traces of it, so we don't leave around a file which 
            //  is no longer being referenced, which they might one day mistake for the current version)
            Directory.Delete(strProjectFolder, true);

            // remove the existing references in the Recent lists too
            int nIndex = Properties.Settings.Default.RecentProjects.IndexOf(strProjectName);
            if (nIndex != -1)
            {
                Properties.Settings.Default.RecentProjects.RemoveAt(nIndex);
                Properties.Settings.Default.RecentProjectPaths.RemoveAt(nIndex);
                Properties.Settings.Default.Save();
            }
        }

        private void InitLanguageControls(Control tabPage, ProjectSettings.LanguageInfo languageInfo)
        {
            System.Diagnostics.Debug.Assert(tabPage.Controls[0] is TableLayoutPanel);
            TableLayoutPanel tlp = tabPage.Controls[0] as TableLayoutPanel;
            System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 2) is ComboBox);
            ComboBox comboBoxKeyboard = tlp.GetControlFromPosition(1, 2) as ComboBox;

            // initialize the keyboard combo list
            foreach (KeyboardController.KeyboardDescriptor keyboard in
                KeyboardController.GetAvailableKeyboards(KeyboardController.Engines.All))
                comboBoxKeyboard.Items.Add(keyboard.ShortName);

            System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 4) is TextBox);
            TextBox tbSentFullStop = tlp.GetControlFromPosition(1, 4) as TextBox;

            tbSentFullStop.Font = languageInfo.FontToUse;
            tbSentFullStop.ForeColor = languageInfo.FontColor;
            tbSentFullStop.Text = languageInfo.FullStop;

            System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 3) is Button);
            Button btnFont = tlp.GetControlFromPosition(1, 3) as Button;

            toolTip.SetToolTip(btnFont,
                               String.Format(
                                   Localizer.Str(
                                       "Click here to choose the font, size, and color of the font to use for this language{0}Currently, Font: {1}, Size: {2}, {3}"),
                                   Environment.NewLine,
                                   languageInfo.DefaultFontName,
                                   languageInfo.DefaultFontSize,
                                   languageInfo.FontColor));

            if (languageInfo.HasData)
            {
                System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 0) is TextBox);
                TextBox textBoxLanguageName = tlp.GetControlFromPosition(1, 0) as TextBox;
                textBoxLanguageName.Text = languageInfo.LangName;

                System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 1) is TextBox);
                TextBox textBoxEthCode = tlp.GetControlFromPosition(1, 1) as TextBox;
                textBoxEthCode.Text = languageInfo.LangCode;

                comboBoxKeyboard.SelectedItem = languageInfo.DefaultKeyboard;

                System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(2, 3) is CheckBox);
                CheckBox checkBoxIsRTL = tlp.GetControlFromPosition(2, 3) as CheckBox;
                checkBoxIsRTL.Checked = languageInfo.DefaultRtl;
            }
        }

        public string ProjectName
        {
            get
            {
                return textBoxProjectName.Text;
            }
            set
            {
                textBoxProjectName.Text = value;
            }
        }

        public ProjectSettings ProjSettings;

        public string UrlBase
        {
            get
            {
                return textBoxHgRepoUrlBase.Text;
            }
            set
            {
                textBoxHgRepoUrlBase.Text = value;
            }
        }

        public string Url
        {
            get { return textBoxHgRepoUrl.Text; }
            set { textBoxHgRepoUrl.Text = value; }
        }

        public string HgUsername
        {
            get { return textBoxUsername.Text; }
            set { textBoxUsername.Text = value; }
        }

        public string HgPassword
        {
            get { return textBoxPassword.Text; }
            set { textBoxPassword.Text = value; }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                _bEnableTabSelection = true;
                if (tabControl.SelectedIndex > 0)
                    tabControl.SelectedIndex--;
            }
            catch (UserException ex)
            {
                tabControl.SelectedTab = ex.Tab;
                ex.Control.Focus();
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
            finally
            {
                _bEnableTabSelection = false;
                if (tabControl.SelectedIndex != (tabControl.TabPages.Count - 1))
                    buttonNext.Text = Localizer.Str("&Next");
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            var strFinishButtonText = Localizer.Str("&Finish");
            try
            {
                if (buttonNext.Text == strFinishButtonText)
                {
                    FinishEdit();
                }
                else
                {
                    _bEnableTabSelection = true;
                    ProcessNext();
                }
            }
            catch (UserException ex)
            {
                if (ex.Tab != null)
                    tabControl.SelectedTab = ex.Tab;
                if (ex.Control != null)
                    ex.Control.Focus();
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
            finally
            {
                _bEnableTabSelection = false;
                if (tabControl.SelectedIndex == (tabControl.TabPages.Count - 1))
                    buttonNext.Text = strFinishButtonText;
            }
        }

        private void FinishEdit()
        {
            VerifyAiConfiguration();
            _storyProjectData.TeamMembers.HasOutsideEnglishBTer = checkBoxOutsideEnglishBackTranslator.Checked;
            _storyProjectData.TeamMembers.HasIndependentConsultant = radioButtonIndependentConsultant.Checked;
            ProjSettings.ShowRetellings.Vernacular = checkBoxRetellingsVernacular.Checked;
            ProjSettings.ShowRetellings.NationalBt = checkBoxRetellingsNationalBT.Checked;
            ProjSettings.ShowRetellings.InternationalBt = checkBoxRetellingsInternationalBT.Checked;
            ProjSettings.ShowTestQuestions.Vernacular = checkBoxTestQuestionsVernacular.Checked;
            ProjSettings.ShowTestQuestions.NationalBt = checkBoxTestQuestionsNationalBT.Checked;
            ProjSettings.ShowTestQuestions.InternationalBt = checkBoxTestQuestionsInternationalBT.Checked;
            ProjSettings.ShowAnswers.Vernacular = checkBoxAnswersVernacular.Checked;
            ProjSettings.ShowAnswers.NationalBt = checkBoxAnswersNationalBT.Checked;
            ProjSettings.ShowAnswers.InternationalBt = checkBoxAnswersInternationalBT.Checked;

            ProjSettings.VernacularToNationalBt =
                (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToNationalBt.Name))
                    ? adaptItConfigCtrlVernacularToNationalBt.AdaptItConfiguration
                    : null;
            ProjSettings.VernacularToInternationalBt =
                (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToInternationalBt.Name))
                    ? adaptItConfigCtrlVernacularToInternationalBt.AdaptItConfiguration
                    : null;
            ProjSettings.NationalBtToInternationalBt =
                (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlNationalBtToInternationalBt.Name))
                    ? adaptItConfigCtrlNationalBtToInternationalBt.AdaptItConfiguration
                    : null;

            if (!checkBoxUseInternetRepo.Checked)
                Program.ClearHgParameters(ProjectName);

            // this is now configured!
            ProjSettings.IsConfigured = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void VerifyAiConfiguration()
        {
            // if there is an AI project configured, then it can't have a null ConverterName
            if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToNationalBt.Name))
            {
                ThrowIfAiConfigIsBad(adaptItConfigCtrlVernacularToNationalBt,
                                     labelAdaptItVernacularToNationalBt.Text);
            }
            if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlVernacularToInternationalBt.Name))
            {
                ThrowIfAiConfigIsBad(adaptItConfigCtrlVernacularToInternationalBt,
                                     labelAdaptItVernacularToInternationalBt.Text);
            }
            if (tlpAdaptItConfiguration.Controls.ContainsKey(adaptItConfigCtrlNationalBtToInternationalBt.Name))
            {
                ThrowIfAiConfigIsBad(adaptItConfigCtrlNationalBtToInternationalBt,
                                     labelAdaptItNationalBtToInternationalBt.Text);
            }
        }

        private void ThrowIfAiConfigIsBad(AdaptItConfigControl adaptItConfigControl, string strWhichLanguages)
        {
            System.Diagnostics.Debug.Assert(adaptItConfigControl != null);
            var conf = adaptItConfigControl.AdaptItConfiguration;
            
            // this if condition is to see if we should even check whether it's bad
            if ((conf != null) &&
                (adaptItConfigControl.AdaptItConfiguration.ProjectType !=
                 ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None))
            {
                // we could check other things, but for now, at least make sure that 
                //  there's a converter spec, since it particularly doesn't like to work 
                //  without one
                if (String.IsNullOrEmpty(conf.ConverterName))
                    throw new UserException(
                        String.Format(
                            Localizer.Str(
                                "There's something missing in the Adapt It configuration for {0} (e.g. no Converter name in the text box above when the type set to 'Local...' or 'Shared Adapt It project'). Try to configure it again or set it to 'None'"),
                            strWhichLanguages),
                        null, null);
            }
        }

        public bool Modified;

        protected bool _bEnableTabSelection;

        protected void ProcessLanguageTab(ComboBox cb, ProjectSettings.LanguageInfo li, 
            CheckBox cbRtl, TextBox textBoxLanguageName, TextBox textBoxEthCode, 
            TextBox textBoxSentFullStop, ref string strKeyboardOverride, ref bool bRtfOverride)
        {
            // if there is a default keyboard (from before) and the user has chosen another 
            //  one, then see if they mean to change it for everyone or just themselves 
            //  (then we can make sure that they are who we think we are)
            string strKeyboard = (string)cb.SelectedItem;
            if (LoggedInMember != null)
            {
                if (!String.IsNullOrEmpty(li.DefaultKeyboard)
                    && (strKeyboard != li.DefaultKeyboard))
                {
                    var res = QueryOverride(li.LangName, Localizer.Str("keyboard"));
                    if (res == DialogResult.Yes)
                        strKeyboardOverride = strKeyboard;
                    else if (res == DialogResult.No)
                    {
                        li.DefaultKeyboard = strKeyboard;
                        strKeyboardOverride = null;   // if there was an override, it should go away
                    }
                    else
                        return;
                }
                else
                {
                    li.DefaultKeyboard = strKeyboard;
                    strKeyboardOverride = null;   // if there was an override, it should go away
                }

                if (li.DefaultRtl != cbRtl.Checked)
                {
                    DialogResult res = QueryOverride(Localizer.Str("Right-to-left"), Localizer.Str("value"));
                    if (res == DialogResult.Yes)
                        bRtfOverride = true;
                    else if (res == DialogResult.No)
                    {
                        li.DefaultRtl = cbRtl.Checked;
                    }
                    else
                        return;
                }
                else
                {
                    li.DefaultRtl = cbRtl.Checked;
                }
            }
            else
                li.DefaultRtl = cbRtl.Checked;

            li.LangName = ThrowIfTextNullOrEmpty(textBoxLanguageName, Localizer.Str("Language Name"));
            li.LangCode = ThrowIfTextNullOrEmpty(textBoxEthCode, Localizer.Str("Ethnologue Code"));
            li.FullStop = ThrowIfTextNullOrEmpty(textBoxSentFullStop, Localizer.Str("Sentence Final Punctuation"));

            tabControl.SelectedIndex++;
        }

        private DialogResult QueryOverride(string strProperty, string strValue)
        {
            return LocalizableMessageBox.Show(String.Format(Localizer.Str("Click 'Yes' to override the '{0}' {1} for yourself ('{2}') only. Click 'No' to make this change apply to every member of the team (if you all are switching to a new {1})"),
                                                 strProperty, strValue, LoggedInMember.Name),
                                   StoryEditor.OseCaption,
                                   MessageBoxButtons.YesNoCancel);
        }

        protected string ThrowIfTextNullOrEmpty(TextBox tb, string strErrorMessage)
        {
            if (String.IsNullOrEmpty(tb.Text))
                throw new UserException(String.Format(Localizer.Str("You have to configure the {0} first"),
                                                      strErrorMessage),
                                        tb, tabControl.SelectedTab);
            return tb.Text;
        }

        protected int IndexAfter(TabPage[] atabs)
        {
            foreach (TabPage tab in atabs)
                if (tabControl.TabPages[tab.Name] != null)
                    return tabControl.TabPages.IndexOf(tab) + 1;

            // shouldn't fall thru
            System.Diagnostics.Debug.Assert(false);
            return 1;
        }

        private void checkBoxUseInternetRepo_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxUseInternetRepo));
            if (checkBoxUseInternetRepo.Checked)
                tabControl.TabPages.Insert(1, tabPageInternetRepository);
            else
                tabControl.TabPages.Remove(tabPageInternetRepository);
            Modified = true;
        }

        private void checkBoxStoryLanguage_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxLanguageVernacular));
            if (checkBoxLanguageVernacular.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageVernacular);
                checkBoxRetellingsVernacular.Checked =
                    checkBoxAnswersVernacular.Checked = false;
                checkBoxTestQuestionsVernacular.Checked = true;
            }
            else
            {
                tabControl.TabPages.Remove(tabPageLanguageVernacular);
                checkBoxRetellingsVernacular.Checked =
                    checkBoxTestQuestionsVernacular.Checked =
                    checkBoxAnswersVernacular.Checked = false;
            }

            UpdateTabPageAIBT();

            checkBoxRetellingsVernacular.Enabled =
                checkBoxTestQuestionsVernacular.Enabled =
                checkBoxAnswersVernacular.Enabled = checkBoxLanguageVernacular.Checked;

            Modified = true;
        }

        private void checkBoxNationalBT_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxLanguageNationalBT));
            if (checkBoxLanguageNationalBT.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguageVernacular, tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageNationalBT);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageNationalBT);

            UpdateTabPageAIBT();

            checkBoxRetellingsNationalBT.Checked = 
                checkBoxTestQuestionsNationalBT.Checked = 
                checkBoxAnswersNationalBT.Checked = false;

            checkBoxRetellingsNationalBT.Enabled =
                checkBoxTestQuestionsNationalBT.Enabled =
                checkBoxAnswersNationalBT.Enabled = checkBoxLanguageNationalBT.Checked;

            Modified = true;
        }

        private void checkBoxEnglishBT_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxLanguageInternationalBT));
            if (checkBoxLanguageInternationalBT.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguageNationalBT, tabPageLanguageVernacular, tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageEnglishBT);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageEnglishBT);

            UpdateTabPageAIBT();

            checkBoxRetellingsInternationalBT.Checked =
                checkBoxTestQuestionsInternationalBT.Checked =
                checkBoxAnswersInternationalBT.Checked = checkBoxLanguageInternationalBT.Checked;

            checkBoxRetellingsInternationalBT.Enabled =
                checkBoxTestQuestionsInternationalBT.Enabled =
                checkBoxAnswersInternationalBT.Enabled = checkBoxLanguageInternationalBT.Checked;

            Modified = true;
        }

        private void UpdateTabPageAIBT()
        {
            // if we don't have either the vernacular or the national bt, then we can't do
            //  any adaptIt BT'ing
            if ((checkBoxLanguageVernacular.Checked
                && (checkBoxLanguageNationalBT.Checked
                    || checkBoxLanguageInternationalBT.Checked))
                || (checkBoxLanguageNationalBT.Checked && checkBoxLanguageInternationalBT.Checked))
            {
                if (!tabControl.TabPages.Contains(tabPageAIBT))
                    tabControl.Controls.Add(tabPageAIBT);
            }
            else // means we can remove it
                tabControl.TabPages.Remove(tabPageAIBT);
        }

        private void checkBoxFreeTranslation_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxLanguageFreeTranslation));
            if (checkBoxLanguageFreeTranslation.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguageEnglishBT, tabPageLanguageNationalBT, tabPageLanguageVernacular, tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageFreeTranslation);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageFreeTranslation);
            Modified = true;
        }

        private void checkBoxOutsideEnglishBackTranslator_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOutsideEnglishBackTranslator.Checked
                && !_storyProjectData.IsASeparateEnglishBackTranslator)
            {
                // if this user is saying that there's an external BTer, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.EnglishBackTranslator)
                                       {
                                           Text = Localizer.Str("Choose the member that will do English BTs")
                                       };
                if (dlg.ShowDialog() == DialogResult.OK)
                    return;

                checkBoxOutsideEnglishBackTranslator.Checked = false;
            }
            Modified = true;
        }

        private void radioButtonIndependentConsultant_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonIndependentConsultant.Checked
                && !_storyProjectData.TeamMembers.IsThereAnIndependentConsultant)
            {
                // if this user is saying that there's a first pass mentor, but there doesn't
                //  appear to be one, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.IndependentConsultant)
                {
                    Text = Localizer.Str("Choose the member that is the independent consultant")
                };
                if (dlg.ShowDialog() == DialogResult.OK)
                    return;

                radioButtonIndependentConsultant.Checked = false;
            }
            Modified = true;
        }

        private void textBoxHgRepo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Url = Program.FormHgUrl(UrlBase, HgUsername, HgPassword, ProjectName);
                Modified = true;
            }
            catch
            {
            }
        }

        private void textBoxHgRepoUrl_MouseClick(object sender, MouseEventArgs e)
        {
            textBoxHgRepoUrl.SelectAll();
            textBoxHgRepoUrl.Copy();
        }

        string _strLangCodesFile;

        protected void ProposeEthnologueCode(string strLanguageName, TextBox tbLanguageCode)
        {
            if (_strLangCodesFile == null)
                _strLangCodesFile = File.ReadAllText(PathToLangCodesFile);

            int nIndex = _strLangCodesFile.IndexOf(String.Format("\t{0}{1}", strLanguageName, Environment.NewLine));
            const int CnOffset = 8;
            if (nIndex >= CnOffset)
            {
                nIndex -= CnOffset;    // back up to the beginning of the line;
                int nLength = 1 /* for the tab */ + strLanguageName.Length + CnOffset;
                string strEntry = _strLangCodesFile.Substring(nIndex, nLength);

                // now, grab off just the code, which goes from the beginning of the line to the first tab.
                nIndex = strEntry.IndexOf('\t');
                string strCode = strEntry.Substring(0, nIndex);
                tbLanguageCode.Text = strCode;
            }
            Modified = true;
        }

        protected const string CstrLangCodesFilename = "LanguageCodes.tab";

        protected string PathToLangCodesFile
        {
            get
            {
                // try the same folder as we're executing out of
                string strCurrentFolder = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                strCurrentFolder = Path.GetDirectoryName(strCurrentFolder);
                string strFileToCheck = Path.Combine(strCurrentFolder, CstrLangCodesFilename);
#if DEBUGBOB
                if (!File.Exists(strFileToCheck))
                    // on dev machines, this file is in the "..\..\src\EC\TECkit Mapping Editor" folder
                    strFileToCheck = @"C:\src\StoryEditor\StoryEditor\" + CstrLangCodesFilename;
#endif
                System.Diagnostics.Debug.Assert(File.Exists(strFileToCheck), String.Format("Can't find: {0}! You'll need to re-install or contact bob_eaton@sall.com", strFileToCheck));

                return strFileToCheck;
            }
        }

        private void textBoxLanguageNameVernacular_TextChanged(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxLanguageNameVernacular.Text, textBoxEthCodeVernacular);
        }

        private void textBoxLanguageNameNationalBT_TextChanged(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxLanguageNameNationalBT.Text, textBoxEthCodeNationalBT);
        }

        private void textBoxLanguageNameEnglishBT_TextChanged(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxLanguageNameEnglishBT.Text, textBoxEthCodeEnglishBT);
        }

        private void textBoxLanguageNameFreeTranslation_TextChanged(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxLanguageNameFreeTranslation.Text, textBoxEthCodeFreeTranslation);
        }

        // for users that
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!_bEnableTabSelection)
                e.Cancel = true;
        }

        protected void SetKeyboard(string strKeyboardToSet)
        {
#if !DEBUGBOB
            if (!String.IsNullOrEmpty(strKeyboardToSet))
                KeyboardController.ActivateKeyboard(strKeyboardToSet);
#endif
        }

        private void textBoxSentFullStopVernacular_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardVernacular.SelectedItem);
        }

        private void textBoxSentFullStopNationalBT_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardNationalBT.SelectedItem);
        }

        private void textBoxSentFullStopEnglishBT_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardEnglishBT.SelectedItem);
        }

        private void textBoxSentFullStopFreeTranslation_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardFreeTranslation.SelectedItem);
        }

        protected void DoFontDialog(ProjectSettings.LanguageInfo li, TextBox tb,
            out string strOverrideFont, out float fOverrideFontSize)
        {
            strOverrideFont = null;
            fOverrideFontSize = 0;
            try
            {
                fontDialog.Font = li.FontToUse;
                fontDialog.Color = li.FontColor;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    li.FontToUse = fontDialog.Font;
                    if (LoggedInMember != null)
                    {
                        if (!String.IsNullOrEmpty(li.DefaultFontName)
                            && ((fontDialog.Font.Name != li.DefaultFontName) ||
                            fontDialog.Font.Size != li.DefaultFontSize))
                        {
                            DialogResult res = QueryOverride(li.DefaultFontName, Localizer.Str("font"));
                            if (res == DialogResult.Yes)
                            {
                                strOverrideFont = fontDialog.Font.Name;
                                fOverrideFontSize = fontDialog.Font.Size;
                            }
                            else if (res == DialogResult.No)
                            {
                                li.DefaultFontName = fontDialog.Font.Name;
                                li.DefaultFontSize = fontDialog.Font.Size;
                            }
                            else
                                return;
                        }
                        else
                        {
                            li.DefaultFontName = fontDialog.Font.Name;
                            li.DefaultFontSize = fontDialog.Font.Size;
                        }
                    }
                    else
                    {
                        li.DefaultFontName = fontDialog.Font.Name;
                        li.DefaultFontSize = fontDialog.Font.Size;
                    }

                    li.FontColor = fontDialog.Color;
                    tb.Font = fontDialog.Font;
                    tb.ForeColor = fontDialog.Color;
                    Modified = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Only TrueType fonts are supported. This is not a TrueType font.")
                    LocalizableMessageBox.Show(
                        Localizer.Str("Since you just added this font, you have to restart the program for it to work"),
                        StoryEditor.OseCaption);
            }
        }

        private void buttonFontVernacular_Click(object sender, EventArgs e)
        {
            string strOverrideFont;
            float fOverrideFontSize;
            DoFontDialog(ProjSettings.Vernacular, textBoxSentFullStopVernacular, 
                out strOverrideFont, out fOverrideFontSize);
            if (LoggedInMember != null)
            {
                LoggedInMember.OverrideFontNameVernacular = strOverrideFont;
                LoggedInMember.OverrideFontSizeVernacular = fOverrideFontSize;
            }
        }

        private void buttonFontNationalBT_Click(object sender, EventArgs e)
        {
            string strOverrideFont;
            float fOverrideFontSize;
            DoFontDialog(ProjSettings.NationalBT, textBoxSentFullStopNationalBT,
                out strOverrideFont, out fOverrideFontSize);
            if (LoggedInMember != null)
            {
                LoggedInMember.OverrideFontNameNationalBT = strOverrideFont;
                LoggedInMember.OverrideFontSizeNationalBT = fOverrideFontSize;
            }
        }

        private void buttonFontEnglishBT_Click(object sender, EventArgs e)
        {
            string strOverrideFont;
            float fOverrideFontSize;
            DoFontDialog(ProjSettings.InternationalBT, textBoxSentFullStopEnglishBT,
                out strOverrideFont, out fOverrideFontSize);
            if (LoggedInMember != null)
            {
                LoggedInMember.OverrideFontNameInternationalBT = strOverrideFont;
                LoggedInMember.OverrideFontSizeInternationalBT = fOverrideFontSize;
            }
        }

        private void buttonFontFreeTranslation_Click(object sender, EventArgs e)
        {
            string strOverrideFont;
            float fOverrideFontSize;
            DoFontDialog(ProjSettings.FreeTranslation, textBoxSentFullStopFreeTranslation,
                out strOverrideFont, out fOverrideFontSize);
            if (LoggedInMember != null)
            {
                LoggedInMember.OverrideFontNameFreeTranslation = strOverrideFont;
                LoggedInMember.OverrideFontSizeFreeTranslation = fOverrideFontSize;
            }
        }

        private void textBoxSentFullStop_Leave(object sender, EventArgs e)
        {
#if !DEBUGBOB
            KeyboardController.DeactivateKeyboard();
#endif
        }

        private void comboBoxKeyboard_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Modified = true;
        }
    }

    public class UserException : ApplicationException
    {
        public Control Control { get; set; }
        public TabPage Tab { get; set; }

        public UserException(string strError, Control ctrl, TabPage tab)
            : base(strError)
        {
            Control = ctrl;
            Tab = tab;
        }
    }
}
