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
        private NewProjectWizard _parent;
        private StoryEditor.GlossType _eGlossType;

        public AdaptItConfigControl(NewProjectWizard parent, StoryEditor.GlossType eGlossType)
        {
            InitializeComponent();
            _parent = parent;
            _eGlossType = eGlossType;
        }

        public string AdaptItConverterName
        { 
            get { return textBoxProjectPath.Text; } 
            set { textBoxProjectPath.Text = value; }
        }

        public override string Text
        {
            get
            {
                return textBoxProjectPath.Text;
            }
            set
            {
                textBoxProjectPath.Text = value;
            }
        }

        public string Label
        {
            get { return labelBT.Text; }
            set { labelBT.Text = value; }
        }

        public string SourceLanguageName { get; set; }
        public string TargetLanguageName { get; set; }

        public enum AdaptItProjectType
        {
            None,
            LocalAiProjectOnly,
            SharedAiProject
        }

        public AdaptItProjectType AdaptItProject
        {
            get
            {
                if (radioButtonLocal.Checked)
                    return AdaptItProjectType.LocalAiProjectOnly;
                if (radioButtonShared.Checked)
                    return AdaptItProjectType.SharedAiProject;
                return AdaptItProjectType.None;
            }
            set
            {
                if (AdaptItProjectType.LocalAiProjectOnly == value)
                    radioButtonLocal.Checked = true;
                else if (AdaptItProjectType.SharedAiProject == value)
                    radioButtonShared.Checked = true;
                else 
                    radioButtonNone.Checked = true;
            }
        }

        public new bool Visible
        {
            get { return _parent.flowLayoutPanelAdaptItControls.Contains(this); }
            set 
            {
                if (!value && Visible)
                    _parent.flowLayoutPanelAdaptItControls.Controls.Remove(this);
                else if (value && !Visible)
                    _parent.flowLayoutPanelAdaptItControls.Controls.Add(this);
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
                AdaptItEncConverter theEc = AdaptItGlossing.InitLookupAdapter(_parent.ProjSettings, _eGlossType,
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
            if (AdaptItProject == AdaptItProjectType.None)
                return;

            if (AdaptItProject == AdaptItProjectType.LocalAiProjectOnly)
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

            if (AdaptItProject == AdaptItProjectType.SharedAiProject)
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
            if (_parent.LoggedInMember != null)
            {
                strHgUsername = _parent.LoggedInMember.HgUsername;
                strHgPassword = _parent.LoggedInMember.HgPassword;
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
    }
}
