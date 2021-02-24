using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Chorus.UI.Clone;
using Chorus.UI.Misc;

namespace AiChorus
{
    public partial class QueryApplicationTypeForm : Form
    {
        private readonly ServerSettingsControl _serverSettingsControl;
        private readonly TargetFolderControl _targetFolderControl;
        private readonly ChorusConfigurations _chorusConfigs;
        private Project _project = new Project();
        public QueryApplicationTypeForm(ChorusConfigurations chorusConfigs)
        {
            InitializeComponent();

            _chorusConfigs = chorusConfigs;
            var serverSettings = _chorusConfigs.ServerSettings.FirstOrDefault();

            var model = new GetCloneFromInternetModel(null)
            {
                Username = serverSettings?.Username,
                Password = serverSettings?.DecryptedPassword,
                HasLoggedIn = true
            };

            _serverSettingsControl = new ServerSettingsControl()
                                         {
                                             TabIndex = 2, 
                                             Anchor = (AnchorStyles.Top | AnchorStyles.Left),
                                             Model = model
                                         };
            tableLayoutPanel.Controls.Add(_serverSettingsControl, 1, 0);
            tableLayoutPanel.SetColumnSpan(_serverSettingsControl, 2);

            _targetFolderControl = new TargetFolderControl(model)
                                       {
                                           TabIndex = 3, 
                                           Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                       };

            tableLayoutPanel.Controls.Add(_targetFolderControl, 2, 0);
            tableLayoutPanel.SetColumnSpan(_targetFolderControl, 2);

            _targetFolderControl._downloadButton.Visible = 
                _serverSettingsControl.Visible =
                _targetFolderControl.Visible = false;
        }

        public string SelectedApplicationType
        { 
            get
            {
                return (radioButtonOse.Checked)
                           ? Program.CstrApplicationTypeOse
                           : Program.CstrApplicationTypeAi;
            } 
        }

        public GetCloneFromInternetModel ServerSettingsModel
        {
            get
            {
                return (GetCloneFromInternetModel)_serverSettingsControl.Model;
            }
        }

        public ApplicationSyncHandler ApplicationToUse { get; set; }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void QueryApplicationTypeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return; // don't care

            ServerSetting serverSetting;
            GetProjectAndServerSettings(out serverSetting);
            if (serverSetting.Projects.Any(p => p.ProjectId == _project.ProjectId))
            {
                MessageBox.Show(String.Format("Can't have another project with the id '{0}'", _project.ProjectId),
                                Properties.Resources.AiChorusCaption);
                e.Cancel = true;
                return;
            }

            serverSetting.Projects.Add(_project);
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RadioButtonOseCheckedChanged(object sender, EventArgs e)
        {
            SetApplicationToUse(typeof(OseSyncHandler));
        }

        private void radioButtonAdaptIt_CheckedChanged(object sender, EventArgs e)
        {
            _targetFolderControl.Visible = true;
            SetApplicationToUse(typeof(AdaptItSyncHandler));
        }

        private void SetApplicationToUse(Type appToUse)
        {
            _serverSettingsControl.Visible = true;
            ServerSetting serverSetting;
            GetProjectAndServerSettings(out serverSetting);
            var aObjects = new object[] {_project, serverSetting};
            ApplicationToUse = (ApplicationSyncHandler)Activator.CreateInstance(appToUse, aObjects);
            var strRootFolder = ApplicationToUse.AppDataRoot;
            if (!Directory.Exists(strRootFolder))
                Directory.CreateDirectory(strRootFolder);
            ServerSettingsModel.ParentDirectoryToPutCloneIn = strRootFolder;
        }

        private void GetProjectAndServerSettings(out ServerSetting serverSetting)
        {
            var model = ServerSettingsModel;
            if (!_chorusConfigs.TryGet(model.CustomUrl, out serverSetting))
            {
                serverSetting = new ServerSetting();
                _chorusConfigs.ServerSettings.Add(serverSetting);
            }

            serverSetting.Password = model.Password;
            serverSetting.ServerName = model.AliasName;
            serverSetting.Username = model.Username;

            _project.ApplicationType = SelectedApplicationType;
            _project.FolderName = model.LocalFolderName;
            _project.ProjectId = model.ProjectId;
        }
    }
}
