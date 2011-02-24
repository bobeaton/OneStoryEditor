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

        public new bool Visible
        {
            get { return Parent.tlpAdaptItConfiguration.Controls.Contains(this); }
        }

        public string SourceLanguageName { get; set; }
        public string TargetLanguageName { get; set; }

        public AdaptItConfigControl()
        {
            InitializeComponent();
        }

        private ProjectSettings.AdaptItConfiguration _adaptItConfiguration;
        public ProjectSettings.AdaptItConfiguration AdaptItConfiguration
        { 
            get
            {
                if (AdaptItProject == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                    return null;

                if (_adaptItConfiguration == null)
                    _adaptItConfiguration = new ProjectSettings.AdaptItConfiguration();

                _adaptItConfiguration.BtDirection = BtDirection;    // from parent
                _adaptItConfiguration.ProjectType = AdaptItProject; // from user
                _adaptItConfiguration.ConverterName = AdaptItConverterName;
                _adaptItConfiguration.RepositoryUrl = _strAdaptItRepositoryUrl;
                return _adaptItConfiguration;
            }
            set
            {
                _adaptItConfiguration = value;
                if (_adaptItConfiguration != null)
                {
                    AdaptItConverterName = _adaptItConfiguration.ConverterName;
                    _strAdaptItRepositoryUrl = _adaptItConfiguration.RepositoryUrl;
                    AdaptItProject = _adaptItConfiguration.ProjectType;
                    System.Diagnostics.Debug.Assert(_adaptItConfiguration.BtDirection == BtDirection);
                }
                else
                {
                    textBoxProjectPath.Clear();
                    AdaptItProject = ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None;
                    AdaptItConverterName = _strAdaptItRepositoryUrl = null;
                }
            }
        }

        private string AdaptItConverterName
        {
            get { return textBoxProjectPath.Text; }
            set { textBoxProjectPath.Text = value; }
        }

        private string _strAdaptItRepositoryUrl;

        private ProjectSettings.AdaptItConfiguration.AdaptItProjectType AdaptItProject
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
            // first let's see if an AI Lookup transducer already exists with the
            //  proper name
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

            // otherwise, let's see if the user wants us to create it or browse for it
            DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_QueryCreateAdaptItProject,
                                                             SourceLanguageName, TargetLanguageName),
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            
            // 'Yes' means the user is asking us to create the AI project
            if (res == DialogResult.Yes)
            {
                ProjectSettings.LanguageInfo liSource, liTarget;
                AdaptItEncConverter theEc = AdaptItGlossing.InitLookupAdapter(Parent.ProjSettings, BtDirection,
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
            if (AdaptItProject == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                return;

            if (AdaptItProject == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly)
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

            if (AdaptItProject == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject)
            {
                folderBrowserDialog.SelectedPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                 AdaptItAutoConfigDialog.CstrAdaptItWorkingDirUnicode);
                folderBrowserDialog.ShowDialog();

                string strAdaptItProjectFolder = folderBrowserDialog.SelectedPath;
                string strDotHgFolder = Path.Combine(strAdaptItProjectFolder, ".hg");
                if (!Directory.Exists(strDotHgFolder))
                {
                    // this means we haven't downloaded it yet, so go ahead and do a "from the internet"
                    if (!CreateRepository(ref strAdaptItProjectFolder))
                        return;
                }

                if (!Directory.Exists(strDotHgFolder))
                    return;

                string strProjectName = Path.GetFileNameWithoutExtension(strAdaptItProjectFolder);
                string strConverterName = "Lookup in " + strProjectName;
                if (!theECs.ContainsKey(strConverterName))
                {
                    string strConverterSpec = Path.Combine(strAdaptItProjectFolder,
                                                           strProjectName + ".xml");
                    theECs.AddConversionMap(strConverterName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                        EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);
                }

                IEncConverter theEC = theECs[strConverterName];
                AdaptItConverterName = theEC.Name;
            }
        }

        private bool CreateRepository(ref string strProjectFolder)
        {
            string strHgUsername = null, strHgPassword = null;
            if (Parent.LoggedInMember != null)
            {
                strHgUsername = Parent.LoggedInMember.HgUsername;
                strHgPassword = Parent.LoggedInMember.HgPassword;
            }

            var model = new GetCloneFromInternetModel(strProjectFolder)
                            {
                                AccountName = strHgUsername,
                                Password = strHgPassword,
                                ProjectId = String.Format(Properties.Settings.Default.AdaptItProjectRepositoryFormat,
                                                          SourceLanguageName, TargetLanguageName),
                                SelectedServerLabel = Properties.Settings.Default.AdaptItDefaultServerLabel,
                                LocalFolderName = String.Format(Properties.Resources.IDS_AdaptItProjectFolderFormat,
                                                                SourceLanguageName, TargetLanguageName)
                            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    strProjectFolder = dlg.PathToNewProject;
                    return true;
                }
            }
            return false;
        }

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            AdaptItConverterName = null;
        }
    }
}
