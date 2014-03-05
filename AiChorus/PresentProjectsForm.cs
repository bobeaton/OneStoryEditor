using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Chorus.UI.Clone;
using Chorus.UI.Misc;

namespace AiChorus
{
    public partial class PresentProjectsForm : Form
    {
        private const int CnColumnOptionsButton = 0;
        private const int CnColumnApplicationType = 1;
        private const int CnColumnProjectId = 2;
        private const int CnColumnFolderName = 3;
        private const int CnColumnServerName = 4;

        private ChorusConfigurations _chorusConfigs;

        public PresentProjectsForm(ChorusConfigurations chorusConfigs)
        {
            _chorusConfigs = chorusConfigs;
            InitializeComponent();

            InitializeDataGrid(chorusConfigs);
        }

        private void InitializeDataGrid(ChorusConfigurations chorusConfigs)
        {
            dataGridViewProjects.Rows.Clear();
            foreach (var serverSetting in chorusConfigs.ServerSettings)
            {
                foreach (var project in serverSetting.Projects)
                    AddProjectRow(project, serverSetting);
            }
        }

        private void AddProjectRow(Project project, ServerSetting serverSetting)
        {
            // based on the type, check out whether the root folder has this project already in it
            ApplicationSyncHandler appHandler;
            var strApplicationName = project.ApplicationType;
            if (!Program.GetSyncApplicationHandler(project, serverSetting, strApplicationName, out appHandler)) 
                return;
            string strButtonLabel = appHandler.ButtonLabel;

            var aos =
                new object[]
                    {strButtonLabel, strApplicationName, project.ProjectId, project.FolderName, serverSetting.ServerName};
            var nRow = dataGridViewProjects.Rows.Add(aos);
            var row = dataGridViewProjects.Rows[nRow];
            row.Cells[CnColumnServerName].ToolTipText =
                String.Format("Your username is '{0}' and password is '{1}'",
                              serverSetting.Username, serverSetting.Password);
            row.Tag = appHandler;
        }

        private void OptionsButtonClicked(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewProjects.Rows.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex > CnColumnOptionsButton))
                return;

            var theRow = dataGridViewProjects.Rows[e.RowIndex];
            if (e.ColumnIndex == CnColumnOptionsButton)
            {
                if (theRow.Tag == null)
                {
                    // means add a new one (for the project managers)
                    var serverSettings = _chorusConfigs.ServerSettings.FirstOrDefault();
                    string serverName = null, hgUsername = null, hgPassword = null;
                    if (serverSettings != null)
                    {
                        serverName = serverSettings.ServerName;
                        hgUsername = serverSettings.Username;
                        hgPassword = serverSettings.DecryptedPassword;
                    }

                    if (String.IsNullOrEmpty(serverName))
                        serverName = Properties.Resources.IDS_DefaultRepoServer;

                    var model = new GetCloneFromInternetModel
                                    {
                                        SelectedServerLabel = serverName,
                                        AccountName = hgUsername,
                                        Password = hgPassword
                                    };
                    using (var dlg = new QueryApplicationTypeForm(_chorusConfigs, model))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            InitializeDataGrid(_chorusConfigs);
                        }
                    }
                    return;
                }
                System.Diagnostics.Debug.Assert(theRow.Tag is ApplicationSyncHandler);
                var appSyncHandler = theRow.Tag as ApplicationSyncHandler;
                Debug.Assert(appSyncHandler != null);
                var strOption = theRow.Cells[CnColumnOptionsButton].Value.ToString();
                switch (strOption)
                {
                    case ApplicationSyncHandler.CstrOptionClone:
                        appSyncHandler.DoClone();
                        InitializeDataGrid(_chorusConfigs);
                        break;
                    case ApplicationSyncHandler.CstrOptionSendReceive:
                        appSyncHandler.DoSynchronize();
                        break;
                    case ApplicationSyncHandler.CstrOptionOpenProject:
                        appSyncHandler.DoProjectOpen();
                        break;
                }
            } 
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _chorusConfigs.SaveAsXml(saveFileDialog.FileName);
            }
        }
    }
}
