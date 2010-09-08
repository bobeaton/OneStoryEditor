using System;
using System.IO;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public partial class NewProjectWizard : Form
    {
        protected const string CstrDefaultFontTooltip =
                    "Click here to choose the font, size, and color of the font to use for this language{0}Currently, Font: {1}, Size: {2}, {3}";
        protected const string CstrFinishButtonText = "&Finish";
        protected const string CstrNextButtonText = "&Next";

        protected StoryProjectData _storyProjectData;
        public TeamMemberData LoggedInMember;

        public NewProjectWizard(StoryProjectData storyProjectData)
        {
            InitializeComponent();
            _storyProjectData = storyProjectData;

            if ((_storyProjectData.ProjSettings != null) && 
                _storyProjectData.ProjSettings.IsConfigured)
            {
                ProjSettings = _storyProjectData.ProjSettings;
                ProjectName = ProjSettings.ProjectName;
                string strDummy;
                if (Program.GetHgRepoParameters(ProjectName, out strDummy, out strDummy, out strDummy))
                    checkBoxUseInternetRepo.Checked = true;
            }
            
            if (!checkBoxUseInternetRepo.Checked)
                tabControl.TabPages.Remove(tabPageInternetRepository);

            if ((_storyProjectData.ProjSettings == null) 
                || !_storyProjectData.ProjSettings.Vernacular.HasData)
                tabControl.TabPages.Remove(tabPageLanguageVernacular);
            else
                checkBoxStoryLanguage.Checked = true;

            if ((_storyProjectData.ProjSettings == null)
                || !_storyProjectData.ProjSettings.NationalBT.HasData)
                tabControl.TabPages.Remove(tabPageLanguageNationalBT);
            else
                checkBoxNationalBT.Checked = true;

            if ((_storyProjectData.ProjSettings != null)
                && !_storyProjectData.ProjSettings.InternationalBT.HasData)
            {
                checkBoxEnglishBT.Checked = false;
                tabControl.TabPages.Remove(tabPageLanguageEnglishBT);
            }
        }

        private void ProcessNext()
        {
            if (tabControl.SelectedTab == tabPageProjectName)
            {
                if (String.IsNullOrEmpty(ProjectName))
                    throw new UserException(Properties.Resources.IDS_UnableToCreateProjectWithoutName,
                        textBoxProjectName, tabPageProjectName);

                if (ProjSettings == null)
                {
                    // this means that we are doing "new" (as opposed to "edit" settings)
                    // first check if this means we have to overwrite a project
                    string strFilename = ProjectSettings.GetDefaultProjectFilePath(ProjectName);
                    if (File.Exists(strFilename))
                    {
                        DialogResult res =
                            MessageBox.Show(String.Format(Properties.Resources.IDS_OverwriteProject, ProjectName),
                                            OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);
                        if (res != DialogResult.Yes)
                            return;

                        RemoveProject(strFilename, ProjectName);
                    }

                    ProjSettings = new ProjectSettings((string)null, ProjectName);

                    // make sure the 'new' folder exists
                    Directory.CreateDirectory(ProjSettings.ProjectFolder);
                }
                else
                    ProjSettings.ProjectName = ProjectName;

                string strUsername, strRepoUrl, strPassword;
                if (_storyProjectData.GetHgRepoUsernamePassword(ProjectName, LoggedInMember,
                    out strUsername, out strPassword, out strRepoUrl))
                    UrlBase = strRepoUrl;
                else
                    UrlBase = Properties.Resources.IDS_DefaultRepoBasePath;

                // these *might* have been initialized even if the call to GetHg... fails
                HgUsername = strUsername;
                HgPassword = strPassword;

                InitLanguageControls(tabPageLanguageVernacular, ProjSettings.Vernacular);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideVernacularKeyboard)))
                    comboBoxKeyboardVernacular.SelectedItem = LoggedInMember.OverrideVernacularKeyboard;

                InitLanguageControls(tabPageLanguageNationalBT, ProjSettings.NationalBT);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideNationalBTKeyboard)))
                    comboBoxKeyboardVernacular.SelectedItem = LoggedInMember.OverrideNationalBTKeyboard;

                InitLanguageControls(tabPageLanguageEnglishBT, ProjSettings.InternationalBT);
                if ((LoggedInMember != null) && (!String.IsNullOrEmpty(LoggedInMember.OverrideInternationalBTKeyboard)))
                    comboBoxKeyboardVernacular.SelectedItem = LoggedInMember.OverrideInternationalBTKeyboard;

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
                
                Program.SetHgParameters(ProjSettings.ProjectFolder,
                    ProjSettings.ProjectName, Url, HgUsername);

                tabControl.SelectedIndex++;
            }
            else if (tabControl.SelectedTab == tabPageLanguages)
            {
                if (!checkBoxStoryLanguage.Checked
                    && !checkBoxNationalBT.Checked
                    && !checkBoxEnglishBT.Checked)
                {
                    throw new UserException(Properties.Resources.IDS_MustHaveAtLeastOneLanguage,
                        checkBoxEnglishBT, tabPageLanguages);
                }

                if (checkBoxStoryLanguage.Checked)
                {
                    if (String.IsNullOrEmpty(textBoxLanguageNameVernacular.Text))
                        textBoxLanguageNameVernacular.Text = (String.IsNullOrEmpty(ProjSettings.Vernacular.LangName))
                                                                 ? ProjectName
                                                                 : ProjSettings.Vernacular.LangName;
                }
                else
                    ProjSettings.Vernacular.HasData = false;

                if (!checkBoxNationalBT.Checked)
                    ProjSettings.NationalBT.HasData = false;

                if (checkBoxEnglishBT.Checked)
                {
                    if (String.IsNullOrEmpty(textBoxLanguageNameEnglishBT.Text)
                        && !ProjSettings.InternationalBT.HasData)
                        textBoxLanguageNameEnglishBT.Text = "English";
                }
                else
                    _storyProjectData.TeamMembers.HasOutsideEnglishBTer = 
                        ProjSettings.InternationalBT.HasData = false;

                // can't have an outside english bter, if we don't have an English BT or only an English BT
                //  (the PF has to put something in!)
                checkBoxOutsideEnglishBackTranslator.Enabled = (checkBoxEnglishBT.Checked 
                    && (checkBoxNationalBT.Checked || checkBoxStoryLanguage.Checked));

                checkBoxOutsideEnglishBackTranslator.Checked = _storyProjectData.TeamMembers.HasOutsideEnglishBTer;
                checkBoxFirstPassMentor.Checked = _storyProjectData.TeamMembers.HasFirstPassMentor;
                radioButtonIndependentConsultant.Checked = _storyProjectData.TeamMembers.HasIndependentConsultant;

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
            else if (tabControl.SelectedTab == tabPageMemberRoles)
            {
            }
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
            Properties.Settings.Default.RecentProjects.RemoveAt(nIndex);
            Properties.Settings.Default.RecentProjectPaths.RemoveAt(nIndex);
            Properties.Settings.Default.Save();
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
                comboBoxKeyboard.Items.Add(keyboard.Name);

            System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 4) is TextBox);
            TextBox tbSentFullStop = tlp.GetControlFromPosition(1, 4) as TextBox;

            tbSentFullStop.Font = languageInfo.FontToUse;
            tbSentFullStop.ForeColor = languageInfo.FontColor;
            tbSentFullStop.Text = languageInfo.FullStop;

            System.Diagnostics.Debug.Assert(tlp.GetControlFromPosition(1, 3) is Button);
            Button btnFont = tlp.GetControlFromPosition(1, 3) as Button;

            toolTip.SetToolTip(btnFont, String.Format(CstrDefaultFontTooltip,
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
        }

        public string HgUsername
        {
            get { return textBoxUsername.Text; }
            set { textBoxUsername.Text = value; }
        }

        protected string HgPassword
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
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
            }
            finally
            {
                _bEnableTabSelection = false;
                if (tabControl.SelectedIndex != (tabControl.TabPages.Count - 1))
                    buttonNext.Text = CstrNextButtonText;   // just in case we're going backwards from the last tab
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (buttonNext.Text == CstrFinishButtonText)
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
                tabControl.SelectedTab = ex.Tab;
                ex.Control.Focus();
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
            }
            finally
            {
                _bEnableTabSelection = false;
                if (tabControl.SelectedIndex == (tabControl.TabPages.Count - 1))
                    buttonNext.Text = CstrFinishButtonText;
            }
        }

        private void FinishEdit()
        {
            _storyProjectData.TeamMembers.HasOutsideEnglishBTer = checkBoxOutsideEnglishBackTranslator.Checked;
            _storyProjectData.TeamMembers.HasFirstPassMentor = checkBoxFirstPassMentor.Checked;
            _storyProjectData.TeamMembers.HasIndependentConsultant = radioButtonIndependentConsultant.Checked;

            if (!checkBoxUseInternetRepo.Checked)
                Program.ClearHgParameters(ProjectName);

            // this is now configured!
            ProjSettings.IsConfigured = true;
            DialogResult = DialogResult.OK;
            Close();
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
                    DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmOverride,
                        li.LangName, "keyboard", LoggedInMember.Name), OseResources.Properties.Resources.IDS_Caption,
                        MessageBoxButtons.YesNoCancel);

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
                    DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmOverride,
                        "Right-to-left", "value", LoggedInMember.Name), OseResources.Properties.Resources.IDS_Caption,
                        MessageBoxButtons.YesNoCancel);

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

            li.LangName = ThrowIfTextNullOrEmpty(textBoxLanguageName, "Language Name");
            li.LangCode = ThrowIfTextNullOrEmpty(textBoxEthCode, "Ethnologue Code");
            li.FullStop = ThrowIfTextNullOrEmpty(textBoxSentFullStop, "Sentence Final Punctuation");

            tabControl.SelectedIndex++;
        }

        protected string ThrowIfTextNullOrEmpty(TextBox tb, string strErrorMessage)
        {
            if (String.IsNullOrEmpty(tb.Text))
                throw new UserException(String.Format("You have to configure the {0} first", strErrorMessage),
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
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxStoryLanguage));
            if (checkBoxStoryLanguage.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageVernacular);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageVernacular);
            Modified = true;
        }

        private void checkBoxNationalBT_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxNationalBT));
            if (checkBoxNationalBT.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguageVernacular, tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageNationalBT);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageNationalBT);
            Modified = true;
        }

        private void checkBoxEnglishBT_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((sender is CheckBox) && (sender == checkBoxEnglishBT));
            if (checkBoxEnglishBT.Checked)
            {
                int nIndex = IndexAfter(new[] { tabPageLanguageNationalBT, tabPageLanguageVernacular, tabPageLanguages });
                tabControl.TabPages.Insert(nIndex, tabPageLanguageEnglishBT);
            }
            else
                tabControl.TabPages.Remove(tabPageLanguageEnglishBT);
            Modified = true;
        }

        private void checkBoxOutsideEnglishBackTranslator_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOutsideEnglishBackTranslator.Checked
                && !_storyProjectData.IsASeparateEnglishBackTranslator)
            {
                // if this user is saying that there's an external BTer, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.eEnglishBacktranslator)
                                       {
                                           Text = "Choose the member that will do English BTs"
                                       };
                if (dlg.ShowDialog() == DialogResult.OK)
                    return;

                checkBoxOutsideEnglishBackTranslator.Checked = false;
            }
            Modified = true;
        }

        private void checkBoxFirstPassMentor_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFirstPassMentor.Checked 
                && !_storyProjectData.TeamMembers.IsThereAFirstPassMentor)
            {
                // if this user is saying that there's a first pass mentor, but there doesn't
                //  appear to be one, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.eFirstPassMentor)
                {
                    Text = "Choose the member that is the first-pass mentor"
                };
                if (dlg.ShowDialog() == DialogResult.OK)
                    return;

                checkBoxFirstPassMentor.Checked = false;
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
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.eIndependentConsultant)
                {
                    Text = "Choose the member that is the independent consultant"
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
                UpdateUrlTextBox();
                Modified = true;
            }
            catch
            {
            }
        }

        protected void UpdateUrlTextBox()
        {
            string strUrlBase = UrlBase;
            var uri = new Uri(strUrlBase);
            if (!String.IsNullOrEmpty(textBoxUsername.Text))
            {
                textBoxPassword.Enabled = true;
                textBoxHgRepoUrl.Text = String.Format("{0}://{1}{2}@{3}/{4}",
                    uri.Scheme, HgUsername, 
                    (String.IsNullOrEmpty(HgPassword)) ? null : ':' + HgPassword, 
                    uri.Host, ProjectName);
            }
            else
            {
                textBoxPassword.Text = null;
                textBoxPassword.Enabled = false;
                textBoxHgRepoUrl.Text = String.Format("{0}://{1}/{2}",
                    uri.Scheme, uri.Host, ProjectName);
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
#if DEBUG
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

        // for users that
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!_bEnableTabSelection)
                e.Cancel = true;
        }

        protected void SetKeyboard(string strKeybaordToSet)
        {
#if !DEBUG
            if (!String.IsNullOrEmpty(strKeybaordToSet))
                KeyboardController.ActivateKeyboard(strKeybaordToSet);
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
                            && (fontDialog.Font.Name != li.DefaultFontName))
                        {
                            DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmOverride,
                                li.DefaultFontName, "font", LoggedInMember.Name), OseResources.Properties.Resources.IDS_Caption,
                                MessageBoxButtons.YesNoCancel);

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
                    MessageBox.Show("Since you just added this font, you have to restart the program for it to work", OseResources.Properties.Resources.IDS_Caption);
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

        private void textBoxSentFullStop_Leave(object sender, EventArgs e)
        {
#if !DEBUG
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
