using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Chorus.UI.Clone;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class AiRepoSelectionForm : TopForm
    {
        public new NewProjectWizard Parent;

        public AiRepoSelectionForm()
        {
            InitializeComponent();
            foreach (string strServerLabel in Program._mapServerToUrl.Keys)
                comboBoxServer.Items.Add(strServerLabel);
        }

        public string InternetAddress
        {
            get
            {
                return (checkBoxInternet.Checked  && (comboBoxServer.SelectedItem != null))
                    ? comboBoxServer.SelectedItem.ToString() 
                    : null;
            }
            set
            {
                comboBoxServer.SelectedItem = value;
                checkBoxInternet.Checked = (comboBoxServer.SelectedItem != null);
            }
        }

        public string ProjectName
        {
            get { return textBoxProjectName.Text; }
            set { textBoxProjectName.Text = value; }
        }

        public string ProjectFolder { get; set; }
        public string SourceLanguageName { get; set; }
        public string TargetLanguageName { get; set; }

        public string NetworkAddress
        {
            get
            {
                return checkBoxNetwork.Checked ? textBoxNetwork.Text : null;
            }
            set
            {
                textBoxNetwork.Text = value;
                checkBoxNetwork.Checked = !String.IsNullOrEmpty(value);
            }
        }

        public const string SharedFolderSuffix = "AdaptItSharedProjects";

        public static string GetFullNetworkAddress(string strNetworkAddress, string strProjectName)
        {
            if (String.IsNullOrEmpty(strNetworkAddress) || String.IsNullOrEmpty(strProjectName))
                return null;

            return Path.Combine(strNetworkAddress,
                                Path.Combine(SharedFolderSuffix,
                                             strProjectName));
        }

        public static string GetFullInternetAddress(string strServer, string strProjectName)
        {
            string strInternetAddress;
            if (String.IsNullOrEmpty(strProjectName) 
                || String.IsNullOrEmpty(strServer)
                || String.IsNullOrEmpty(strInternetAddress = Program.LookupRepoUrl(strServer)))
                return null;

            return String.Format("{0}/{1}", strInternetAddress, strProjectName);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxNetwork_TextChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void buttonBrowseNetwork_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog
                          {
                              Description = Properties.Resources.IDS_BrowseForNetworkFolder
                          };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                NetworkAddress = dlg.SelectedPath;
                checkBoxNetwork.Checked = true;
            }
        }

        private void buttonPushToInternet_Click(object sender, EventArgs e)
        {
            string strAiWorkFolder;
            string strProjectFolderName;
            string strHgUsername;
            string strHgPassword;
            if (!GetAiRepoSettings(out strAiWorkFolder, 
                out strProjectFolderName, out strHgUsername, out strHgPassword))
                return;

            ProjectFolder = Path.Combine(strAiWorkFolder, strProjectFolderName);
            Program.SetAdaptItHgParameters(ProjectFolder, ProjectName, InternetAddress, strHgUsername, strHgPassword);
            Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
        }

        private void buttonPullFromInternet_Click(object sender, EventArgs e)
        {
            string strAiWorkFolder;
            string strProjectFolderName;
            string strHgUsername;
            string strHgPassword;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName, 
                out strHgUsername, out strHgPassword))
                return;

            var model = new GetCloneFromInternetModel(strAiWorkFolder)
            {
                AccountName = strHgUsername,
                Password = strHgPassword,
                ProjectId = ProjectName,
                SelectedServerLabel = comboBoxServer.SelectedItem.ToString(),
                LocalFolderName = strProjectFolderName
            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    ProjectFolder = dlg.PathToNewProject;
                    ProjectName = model.ProjectId;

                    // here (with pull) is one of the few places we actually query the user
                    //  for a username/password. Currently, the code assumes that they will
                    //  be the same as the project account, so make sure that's the case
                    if ((Parent != null) && (Parent.LoggedInMember != null))
                    {
                        if ((!String.IsNullOrEmpty(Parent.LoggedInMember.HgUsername)
                            && (Parent.LoggedInMember.HgUsername != model.AccountName))
                            || (!String.IsNullOrEmpty(Parent.LoggedInMember.HgPassword)
                            && (Parent.LoggedInMember.HgPassword != model.Password)))
                        {
                            // means the user entered a different account/password than what's
                            //  being used by the project file
                            throw new ApplicationException(
                                "It isn't currently supported for your username and password to be different from the username/password for the project account. Contact bob_eaton@sall.com to correct this");
                        }

                        // in the case that the project isn't being used on the internet, but
                        //  the AdaptIt project is, then set the username/password for it.
                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgUsername))
                            Parent.LoggedInMember.HgUsername = model.AccountName;

                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgPassword))
                            Parent.LoggedInMember.HgPassword = model.Password;
                    }

                    Program.SetAdaptItHgParameters(ProjectFolder, ProjectName,
                        dlg.ThreadSafeUrl, model.AccountName, model.Password);
                    Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
                }
            }
        }

        private bool GetAiRepoSettings(out string strAiWorkFolder, 
            out string strProjectFolderName, out string strHgUsername, out string strHgPassword)
        {
            strHgPassword = null;
            strHgUsername = null;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return false;

            if (Parent != null)
            {
                // but override by the current configuration
                if (!String.IsNullOrEmpty(Parent.HgUsername))
                    strHgUsername = Parent.HgUsername;

                if (!String.IsNullOrEmpty(Parent.HgPassword))
                    strHgPassword = Parent.HgPassword;
            }

            return true;
        }

        private bool GetAiRepoSettings(out string strAiWorkFolder, out string strProjectFolderName)
        {
            // e.g. <My Documents>\Adapt It Unicode Work
            strAiWorkFolder = AdaptItGlossing.AdaptItWorkFolder;

            // e.g. "Kangri to Hindi adaptations"
            strProjectFolderName = String.Format(Properties.Resources.IDS_AdaptItProjectFolderFormat,
                                                 SourceLanguageName, TargetLanguageName);

            return true;
        }

        /*
        private static bool ExtractActualLanguageNames(string strProjectName, 
            out string strSourceLanguage, out string strTargetLanguage)
        {
            strSourceLanguage = null;
            strTargetLanguage = null;

            // strProjectName is e.g. aikb-{0}-{1}. We need to return {0} and {1}
            const string strTo = "_To_";
            int nIndex = strProjectName.IndexOf(strTo);
            if (nIndex == -1)
                return false;

            int nPrefixLen = "AdaptItKb_".Length;
            strSourceLanguage = strProjectName.Substring(nPrefixLen, nIndex - nPrefixLen);
            strTargetLanguage = strProjectName.Substring(nIndex + strTo.Length);
            return true;
        }
        */

        private void checkBoxInternet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void checkBoxNetwork_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void buttonPushToNetwork_Click(object sender, EventArgs e)
        {
            string strAiWorkFolder;         // e.g. C:\Users\Bob\Documents\Adapt It Unicode Work
            string strProjectFolderName;    // e.g. Kangri to Hindi adaptations
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return;

            ProjectFolder = Path.Combine(strAiWorkFolder, strProjectFolderName);
            Program.SetAdaptItHgParametersNetworkDrive(ProjectFolder, ProjectName, NetworkAddress);
            Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
        }

        private Regex FilterProjectName = new Regex("[A-Z]");

        private void textBoxProjectName_TextChanged(object sender, EventArgs e)
        {
            Match match = FilterProjectName.Match(textBoxProjectName.Text);
            if (match.Success)
            {
                textBoxProjectName.SelectionStart = match.Index;
                textBoxProjectName.SelectionLength = match.Length;

                toolTip.Show(
                    "Enter the project name of the repository on the internet (e.g. aikb-hindi-english). Length between 2 and 20 characters. Only lower case letters (a-z), numbers and dashes are allowed AND the repository must have already been created by the repository administrator (bob_eaton@sall.com)",
                    textBoxProjectName);
            } 
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            buttonPushToNetwork.Enabled = checkBoxNetwork.Checked;
            buttonPullFromInternet.Enabled = buttonPushToInternet.Enabled = checkBoxInternet.Checked;
            labelNetworkPath.Text = GetFullNetworkAddress(NetworkAddress, ProjectName);
            labelFullInternetUrl.Text = GetFullInternetAddress(InternetAddress, ProjectName);
        }

        private void comboBoxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }
    }
}
