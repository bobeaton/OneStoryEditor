using System;
using System.IO;
using System.Windows.Forms;
using Chorus.UI.Clone;
using ECInterfaces;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class AdaptItConfigControl : UserControl
    {
        public new NewProjectWizard Parent;
        public ProjectSettings.AdaptItConfiguration.AdaptItBtDirection BtDirection;

        private string _strSourceLanguageName;
        public string SourceLanguageName
        {
            get
            {
                return _strSourceLanguageName;
            }
            set
            {
                _strSourceLanguageName = value;
                _strAdaptItProjectName = null;
            }
        }

        private string _strTargetLanguageName;
        public string TargetLanguageName 
        {
            get { return _strTargetLanguageName; }
            set
            {
                _strTargetLanguageName = value;
                _strAdaptItProjectName = null;
            }
        }

        public AdaptItConfigControl()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        private ProjectSettings.AdaptItConfiguration _adaptItConfiguration;
        public ProjectSettings.AdaptItConfiguration AdaptItConfiguration
        { 
            get
            {
                var eType = AdaptItProjectType;
                if (eType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                    return null;

                if (_adaptItConfiguration == null)
                    _adaptItConfiguration = new ProjectSettings.AdaptItConfiguration();

                _adaptItConfiguration.BtDirection = BtDirection;    // from parent
                _adaptItConfiguration.ProjectType = AdaptItProjectType; // from user
                _adaptItConfiguration.ConverterName = AdaptItConverterName;
                _adaptItConfiguration.ProjectFolderName = _strAdaptItProjectFolderName;
                _adaptItConfiguration.RepoProjectName = GetProjectNameOrDefault(eType);
                _adaptItConfiguration.RepositoryServer = _strAdaptItRepositoryServer;
                _adaptItConfiguration.NetworkRepositoryPath = _strAdaptItNetworkRepositoryPath;
                return _adaptItConfiguration;
            }
            set
            {
                _adaptItConfiguration = value;
                if (_adaptItConfiguration != null)
                {
                    AdaptItConverterName = _adaptItConfiguration.ConverterName;
                    _strAdaptItProjectFolderName = _adaptItConfiguration.ProjectFolderName;
                    _strAdaptItProjectName = _adaptItConfiguration.RepoProjectName;
                    _strAdaptItRepositoryServer = _adaptItConfiguration.RepositoryServer;
                    _strAdaptItNetworkRepositoryPath = _adaptItConfiguration.NetworkRepositoryPath;
                    AdaptItProjectType = _adaptItConfiguration.ProjectType;
                    System.Diagnostics.Debug.Assert(_adaptItConfiguration.BtDirection == BtDirection);
                }
                else
                {
                    textBoxProjectPath.Clear();
                    AdaptItProjectType = ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None;
                    AdaptItConverterName = _strAdaptItNetworkRepositoryPath = null;
                    InitSharedOnlyFieldDefaults();
                }
            }
        }

        private void InitSharedOnlyFieldDefaults()
        {
            _strAdaptItProjectName = GetProjectNameOrDefault(AdaptItProjectType);
            _strAdaptItRepositoryServer = Properties.Resources.IDS_DefaultRepoServer;
        }

        private void ResetSharedOnlyFields()
        {
            _strAdaptItProjectName = 
                _strAdaptItRepositoryServer = 
                _strAdaptItNetworkRepositoryPath = null;
        }

        private string AdaptItConverterName
        {
            get { return textBoxProjectPath.Text; }
            set { textBoxProjectPath.Text = value; }
        }

        private string _strAdaptItProjectFolderName;
        private string _strAdaptItProjectName;
        private string _strAdaptItRepositoryServer;
        private string _strAdaptItNetworkRepositoryPath;

        private string GetProjectNameOrDefault(ProjectSettings.AdaptItConfiguration.AdaptItProjectType eType)
        {
        /*
            if (String.IsNullOrEmpty(_strAdaptItProjectName)
                && !String.IsNullOrEmpty(SourceLanguageName)
                && !String.IsNullOrEmpty(TargetLanguageName)
                && (eType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject))
            {
                _strAdaptItProjectName = String.Format(Properties.Resources.AdaptItProjectRepositoryFormat,
                                                       SourceLanguageName.ToLower(), 
                                                       TargetLanguageName.ToLower());
            }
        */
            return _strAdaptItProjectName;
        }

        private ProjectSettings.AdaptItConfiguration.AdaptItProjectType AdaptItProjectType
        {
            get
            {
                if (radioButtonLocal.Checked)
                    return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly;
                if (radioButtonShared.Checked)
                    return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject;
                return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None;
            }
            set
            {
                if (ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly == value)
                    radioButtonLocal.Checked = true;
                else if (ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject == value)
                    radioButtonShared.Checked = true;
                else 
                    radioButtonNone.Checked = true;
            }
        }

        private void radioButtonLocal_Click(object sender, EventArgs e)
        {
            ResetSharedOnlyFields();
            // first let's see if an AI Lookup transducer already exists with the
            //  proper name
            string strAiProjectFolder = AdaptItKBReader.AdaptItProjectFolder(null, SourceLanguageName, TargetLanguageName);
            if (Directory.Exists(strAiProjectFolder))
            {
                string strConverterSpec = AdaptItKBReader.AdaptItLookupFileSpec(null, SourceLanguageName, TargetLanguageName);
                if (File.Exists(strConverterSpec))
                {
                    string strConverterName = AdaptItKBReader.AdaptItLookupConverterName(SourceLanguageName, TargetLanguageName);
                    if (theECs.ContainsKey(strConverterName))
                    {
                        IEncConverter theEc = theECs[strConverterName];
                        if (theEc is AdaptItEncConverter)
                        {
                            AdaptItConverterName = theEc.Name;
                            _strAdaptItProjectFolderName =
                                Path.GetFileNameWithoutExtension(
                                    AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
                            Parent.Modified = true;
                            return;
                        }
                    }
                }
            }

            // otherwise, let's see if the user wants us to create it or browse for it
            DialogResult res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Click 'Yes' to create an Adapt It project to use for back-translation from '{0}' to '{1}'. Click 'No' to browse for an existing Adapt It project instead"),
                                                             SourceLanguageName, TargetLanguageName),
                                               StoryEditor.OseCaption,
                                               MessageBoxButtons.YesNoCancel);
            
            // 'Yes' means the user is asking us to create the AI project
            if (res == DialogResult.Yes)
            {
                try
                {
                    ProjectSettings.LanguageInfo liSource, liTarget;
                    var theEc = AdaptItGlossing.InitLookupAdapter(Parent.ProjSettings, BtDirection,
                                                                  Parent.LoggedInMember,
                                                                  out liSource, out liTarget);
                    AdaptItConverterName = theEc.Name;
                    _strAdaptItProjectFolderName =
                        Path.GetFileNameWithoutExtension(
                            AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
                return;
            }
            
            // 'No' means browse for it
            if (res == DialogResult.No)
                buttonBrowse_Click(sender, e);

            Parent.Modified = true;
        }

        private void radioButtonShared_Click(object sender, EventArgs e)
        {
            buttonBrowse_Click(sender, e);
        }

        private EncConverters _theECs;
        private EncConverters theECs
        {
            get
            {
                if (_theECs == null)
                    _theECs = new EncConverters();
                return _theECs;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                return;

            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly)
            {
                string strConverterName = null;
                IEncConverter theEC = new AdaptItEncConverter();
                if (theECs.AutoConfigureEx(theEC,
                                           ConvType.Unicode_to_from_Unicode,
                                           ref strConverterName,
                                           "UNICODE", "UNICODE"))
                {
                    AdaptItConverterName = theEC.Name;
                }
            }

            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject)
            {
                DoSharedAiProjectClick();
            }

            Parent.Modified = true;
        }

        private void DoSharedAiProjectClick()
        {
            DialogResult res = LocalizableMessageBox.Show(Localizer.Str("Is the shared Adapt It project on your computer now? (click 'Yes' to browse for it; click 'No' to enter the repository information for it)"),
                                               StoryEditor.OseCaption,
                                               MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return;

            if (res == DialogResult.No)
            {
                string strProjectFolder;
                DoPull(out strProjectFolder);
                if (String.IsNullOrEmpty(strProjectFolder))
                    return;

                _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                string strConverterName = "Lookup in " + _strAdaptItProjectFolderName;
                string strConverterSpec = Path.Combine(strProjectFolder,
                                                       _strAdaptItProjectFolderName + ".xml");
                theECs.AddConversionMap(strConverterName, strConverterSpec,
                    ConvType.Unicode_to_from_Unicode, EncConverters.strTypeSILadaptit,
                    "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);

                IEncConverter theEc = theECs[strConverterName];
                if (theEc == null)
                {
                    LocalizableMessageBox.Show(Localizer.Str(@"Unable to create an AdaptIt glossing converter for the project! Are you missing the file <InstallDir>\EC\Plugins\AI 4.0.0.0 Plugin Details.xml"),
                                               StoryEditor.OseCaption,
                                               MessageBoxButtons.OKCancel);
                    return;
                }
                AdaptItConverterName = theEc.Name;
            }
            else // the project *is* on this machine...
            {
                // first let's see if an AI Lookup transducer already exists with the
                //  proper name
                string strProjectFolder,
                       strConverterName = AdaptItKBReader.AdaptItLookupConverterName(SourceLanguageName,
                                                                                     TargetLanguageName);
                IEncConverter theEc = null;
                if (theECs.ContainsKey(strConverterName))
                {
                    theEc = theECs[strConverterName];
                    if (theEc is AdaptItEncConverter)
                    {
                        AdaptItConverterName = theEc.Name;
                        strProjectFolder =
                            AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier);
                        res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Is the shared Adapt It project in the '{0}' folder?"),
                                                            strProjectFolder),
                                              StoryEditor.OseCaption,
                                              MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Cancel)
                            return;

                        if (res == DialogResult.No)
                            theEc = null;

                        // the 'yes' case falls through and skips the next if statement
                        _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                    }
                }

                if (theEc == null)
                {
                    // this means we don't know which one it was, so query for which project 
                    //  the user wants to share
                    theEc = new AdaptItEncConverter();
                    if (theECs.AutoConfigureEx(theEc,
                                               ConvType.Unicode_to_from_Unicode,
                                               ref strConverterName,
                                               "UNICODE", "UNICODE"))
                    {
                        AdaptItConverterName = theEc.Name;
                        _strAdaptItProjectFolderName =
                            Path.GetFileNameWithoutExtension(
                                AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
                    }
                    else
                        return;
                }

                // in case the user has just created it via Local and switches to Shared, we
                //  need to re setup the project name, etc...
                if (String.IsNullOrEmpty(_strAdaptItProjectName) || 
                    String.IsNullOrEmpty(_strAdaptItRepositoryServer))
                    InitSharedOnlyFieldDefaults();

                // now we know which local AI project it is and it's EncConverter, but now
                //  we need to possibly push the project.
                DoPush(out strProjectFolder);
                if (!String.IsNullOrEmpty(strProjectFolder))
                    _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
            }
        }

#if !UseUrlsWithChorus
        private void DoPull(out string strProjectFolder)
        {
            strProjectFolder = null;
            string strAiWorkFolder;
            string strProjectFolderName;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return;

            if (!Directory.Exists(strAiWorkFolder))
                Directory.CreateDirectory(strAiWorkFolder);

            var model = new GetCloneFromInternetModel(strAiWorkFolder)
            {
                Username = Parent?.LoggedInMember?.HgUsername,
                Password = Parent?.LoggedInMember?.HgPassword,
                LocalFolderName = strProjectFolderName
            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    strProjectFolder = dlg.PathToNewlyClonedFolder;

                    // here (with pull) is one of the few places we actually query the user
                    //  for a username/password. Currently, the code assumes that they will
                    //  be the same as the project account, so make sure that's the case
                    if ((Parent != null) && (Parent.LoggedInMember != null))
                    {
                        // in the case that the project isn't being used on the internet, but
                        //  the AdaptIt project is, then set the username/password for it.
                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgUsername))
                            Parent.LoggedInMember.HgUsername = model.Username;

                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgPassword))
                            Parent.LoggedInMember.HgPassword = model.Password;
                    }

                    Program.SetAiProjectForSyncage(strProjectFolder, model.ProjectId);
                }
            }
        }

        private bool GetAiRepoSettings(out string strAiWorkFolder, out string strProjectFolderName)
        {
            // e.g. <My Documents>\Adapt It Unicode Work
            strAiWorkFolder = AdaptItKBReader.AdaptItWorkFolder;

            // e.g. "Kangri to Hindi adaptations"
            strProjectFolderName = String.Format(Properties.Resources.IDS_AdaptItProjectFolderFormat,
                                                 SourceLanguageName, TargetLanguageName);

            return true;
        }

        private void DoPush(out string strProjectFolder)
        {
            strProjectFolder = null;
            string strAiWorkFolder, strProjectFolderName;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return;

            var strValue = LocalizableMessageBox.InputBox(Localizer.Str("Enter the AdaptIt knowledge base project name (e.g. aikb-hindi-english)"),
                                StoryEditor.OseCaption, "aikb-");

            Program.SyncWithAiRepository(strAiWorkFolder, strValue, true, true);
        }
#else
        private void DoPushPull(out string strProjectFolder)
        {
            strProjectFolder = null;
            var dlg = new AiRepoSelectionForm
                          {
                              SourceLanguageName = SourceLanguageName,
                              TargetLanguageName = TargetLanguageName,
                              InternetAddress = _strAdaptItRepositoryServer,
                              NetworkAddress = _strAdaptItNetworkRepositoryPath,
                              ProjectName = _strAdaptItProjectName,
                              Parent = Parent
                          };

            try
            {
                // this dialog takes care of push and pull
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    strProjectFolder = dlg.ProjectFolder;
                    _strAdaptItProjectName = dlg.ProjectName;
                    _strAdaptItRepositoryServer = dlg.InternetAddress;
                    _strAdaptItNetworkRepositoryPath = dlg.NetworkAddress;
                }
            }
            catch (Exception ex)
            {
                string strErrorMsg = String.Format(Localizer.Str("Unable to Send/Receive the AdaptIt project '{1}' from the requested server{0}{2}{0}{3}"),
                    Environment.NewLine, _strAdaptItProjectName,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""), ex.Message);
                LocalizableMessageBox.Show(strErrorMsg, StoryEditor.OseCaption);
            }
        }
#endif

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            ResetSharedOnlyFields();
            AdaptItConverterName = null;
            Parent.Modified = true;
        }
    }
}
