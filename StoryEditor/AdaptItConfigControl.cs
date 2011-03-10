using System;
using System.IO;
using System.Windows.Forms;
using Chorus.UI.Clone;
using ECInterfaces;
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
        }

        private ProjectSettings.AdaptItConfiguration _adaptItConfiguration;
        public ProjectSettings.AdaptItConfiguration AdaptItConfiguration
        { 
            get
            {
                if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                    return null;

                if (_adaptItConfiguration == null)
                    _adaptItConfiguration = new ProjectSettings.AdaptItConfiguration();

                _adaptItConfiguration.BtDirection = BtDirection;    // from parent
                _adaptItConfiguration.ProjectType = AdaptItProjectType; // from user
                _adaptItConfiguration.ConverterName = AdaptItConverterName;
                _adaptItConfiguration.RepoProjectName = GetProjectNameOrDefault;
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
                    _strAdaptItProjectName = GetProjectNameOrDefault;
                    _strAdaptItRepositoryServer = Properties.Resources.IDS_DefaultRepoServer;
                }
            }
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

        private string _strAdaptItProjectName;
        private string _strAdaptItRepositoryServer;
        private string _strAdaptItNetworkRepositoryPath;

        private string GetProjectNameOrDefault
        {
            get
            {
                if (String.IsNullOrEmpty(_strAdaptItProjectName)
                    && !String.IsNullOrEmpty(SourceLanguageName)
                    && !String.IsNullOrEmpty(TargetLanguageName))
                {
                    _strAdaptItProjectName = String.Format(Properties.Resources.AdaptItProjectRepositoryFormat,
                                                           SourceLanguageName.ToLower(), 
                                                           TargetLanguageName.ToLower());
                }
                return _strAdaptItProjectName;
            }
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
            string strAiProjectFolder = AdaptItGlossing.AdaptItProjectFolder(SourceLanguageName, TargetLanguageName);
            if (Directory.Exists(strAiProjectFolder))
            {
                string strConverterSpec = AdaptItGlossing.AdaptItLookupFileSpec(SourceLanguageName, TargetLanguageName);
                if (File.Exists(strConverterSpec))
                {
                    string strConverterName = AdaptItGlossing.AdaptItLookupConverterName(SourceLanguageName, TargetLanguageName);
                    if (theECs.ContainsKey(strConverterName))
                    {
                        IEncConverter theEc = theECs[strConverterName];
                        if (theEc is AdaptItEncConverter)
                        {
                            AdaptItConverterName = theEc.Name;
                            return;
                        }
                    }
                }
            }

            // otherwise, let's see if the user wants us to create it or browse for it
            DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_QueryCreateAdaptItProject,
                                                             SourceLanguageName, TargetLanguageName),
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            
            // 'Yes' means the user is asking us to create the AI project
            if (res == DialogResult.Yes)
            {
                ProjectSettings.LanguageInfo liSource, liTarget;
                var theEc = AdaptItGlossing.InitLookupAdapter(Parent.ProjSettings, BtDirection,
                                                              Parent.LoggedInMember,
                                                              out liSource, out liTarget);
                AdaptItConverterName = theEc.Name;
                return;
            }
            
            // 'No' means browse for it
            if (res == DialogResult.No)
                buttonBrowse_Click(sender, e);
        }

        private void radioButtonShared_Click(object sender, EventArgs e)
        {
            buttonBrowse_Click(sender, e);
        }

        private EncConverters theECs = new EncConverters();

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
        }

        private void DoSharedAiProjectClick()
        {
            DialogResult res = MessageBox.Show(Properties.Resources.IDS_QueryIfAiProjectNeedsToBePulled,
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return;

            if (res == DialogResult.No)
            {
                string strProjectFolder;
                DoPushPull(out strProjectFolder);
                if (String.IsNullOrEmpty(strProjectFolder))
                    return;

                string strAiProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);
                string strConverterName = "Lookup in " + strAiProjectName;
                string strConverterSpec = Path.Combine(strProjectFolder,
                                                       strAiProjectName + ".xml");
                theECs.AddConversionMap(strConverterName, strConverterSpec,
                    ConvType.Unicode_to_from_Unicode, EncConverters.strTypeSILadaptit,
                    "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);

                IEncConverter theEc = theECs[strConverterName];
                AdaptItConverterName = theEc.Name;
            }
            else // the project *is* on this machine...
            {
                // first let's see if an AI Lookup transducer already exists with the
                //  proper name
                string strProjectFolder,
                       strConverterName = AdaptItGlossing.AdaptItLookupConverterName(SourceLanguageName,
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
                        res = MessageBox.Show(String.Format(Properties.Resources.IDS_QuerySharedAiProject,
                                                            strProjectFolder),
                                              OseResources.Properties.Resources.IDS_Caption,
                                              MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Cancel)
                            return;

                        if (res == DialogResult.No)
                            theEc = null;

                        // the 'yes' case falls through and skips the next if statement
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
                    }
                    else
                        return;
                }

                // now we know which local AI project it is and it's EncConverter, but now
                //  we need to possibly push the project.
                DoPushPull(out strProjectFolder);
            }
        }

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
                string strErrorMsg = String.Format(Properties.Resources.IDS_UnableToConfigureSharedAiProject,
                    Environment.NewLine, _strAdaptItProjectName,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""), ex.Message);
                MessageBox.Show(strErrorMsg, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            ResetSharedOnlyFields();
            AdaptItConverterName = null;
        }
    }
}
