using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AiChorus
{
    public partial class PresentProjectsForm : Form
    {
        private const int CnColumnOptionsButton = 0;
        private const int CnColumnApplicationType = 1;
        private const int CnColumnProjectId = 2;
        private const int CnColumnFolderName = 3;
#if AllowServerToBeConfigured
        private const int CnColumnServerName = 4;
#else
        private const int CnColumnExcludeFromSync = 4;
        private const int CnColumnExcludeFromGoogleSheet = 5;
#endif

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

            var areDownloadableProjects = false;
            var areSyncableProjects = false;
            foreach (var serverSetting in chorusConfigs.ServerSettings)
            {
                foreach (var project in serverSetting.Projects)
                {
                    var theRow = AddProjectRow(project, serverSetting);

                    if (theRow != null)
                    {
                        areDownloadableProjects |= (ApplicationSyncHandler.CstrOptionClone == (string)theRow.Cells[CnColumnOptionsButton].Value);
                        areSyncableProjects |= (((ApplicationSyncHandler)theRow.Tag).GetSynchronizeOrOpenProjectLabel == (string)theRow.Cells[CnColumnOptionsButton].Value);
                    }
                }
            }

            if (!areDownloadableProjects)
            {
                toolStripMenuItemDownloadAll.Enabled = false;
                toolStripMenuItemDownloadAll.ToolTipText = "There are no projects in this project set that haven't been downloaded already.";
            }

            if (!areSyncableProjects)
            {
                toolStripMenuItemSynchronizeAll.Enabled = false;
                toolStripMenuItemSynchronizeAll.ToolTipText = "You must first download the projects before you can synchronize them.";
            }
        }

        private DataGridViewRow AddProjectRow(Project project, ServerSetting serverSetting)
        {
            // based on the type, check out whether the root folder has this project already in it
            ApplicationSyncHandler appHandler;
            var strApplicationName = project.ApplicationType;
            if (!Program.GetSyncApplicationHandler(project, serverSetting, strApplicationName, out appHandler)) 
                return null;

            string strButtonLabel = (project.ExcludeFromSyncing) 
                                        ? ApplicationSyncHandler.CstrOptionHidden
                                        : appHandler.ButtonLabel;

            var aos =
                new object[]
                    {strButtonLabel, strApplicationName, project.ProjectId, project.FolderName
#if AllowServerToBeConfigured
                    , serverSetting.ServerName
#else
                    , project.ExcludeFromSyncing
                    , project.ExcludeFromGoogleSheet
#endif
                    };
            var nRow = dataGridViewProjects.Rows.Add(aos);
            var row = dataGridViewProjects.Rows[nRow];
#if AllowServerToBeConfigured
            row.Cells[CnColumnServerName].ToolTipText =
                String.Format("Your username is '{0}' and password is '{1}'",
                              serverSetting.Username, serverSetting.Password);
#endif
            row.Tag = appHandler;
            return row;
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
                    // means the user is adding a new one (by clicking on the bottom row)
                    using (var dlg = new QueryApplicationTypeForm(_chorusConfigs))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            InitializeDataGrid(_chorusConfigs);
                            _modified = true;
                        }
                    }
                    return;
                }
                Debug.Assert(theRow.Tag is ApplicationSyncHandler);
                var appSyncHandler = theRow.Tag as ApplicationSyncHandler;
                Debug.Assert(appSyncHandler != null);
                var strOption = theRow.Cells[CnColumnOptionsButton].Value.ToString();
                switch (strOption)
                {
                    case ApplicationSyncHandler.CstrOptionClone:
                        if (appSyncHandler.DoClone())
                            InitializeDataGrid(_chorusConfigs);
                        break;
                    case ApplicationSyncHandler.CstrOptionSendReceive:
                        appSyncHandler.DoSynchronize(false);
                        break;
                    case ApplicationSyncHandler.CstrOptionOpenProject:
                        appSyncHandler.DoProjectOpen();
                        break;
                }
            }
        }

        private void dataGridViewProjects_MouseClick(object sender, MouseEventArgs e)
        {
            var hti = dataGridViewProjects.HitTest(e.X, e.Y);
            if ((hti.ColumnIndex < 0) || (hti.ColumnIndex >= dataGridViewProjects.Columns.Count) || 
                (hti.RowIndex < 0) || (hti.RowIndex >= dataGridViewProjects.Rows.Count))
                return;

            if (e.Button == MouseButtons.Right)
            {
                var theRow = dataGridViewProjects.Rows[hti.RowIndex];
                var theCell = theRow?.Cells[hti.ColumnIndex] as DataGridViewButtonCell;
                if (theCell == null)
                    return;

                contextMenu.Show(MousePosition);
            }
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            SaveProjectSet();
        }

        private bool SaveProjectSet()
        {
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_chorusConfigs.FileSpec);
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_chorusConfigs.FileSpec);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _chorusConfigs.SaveAsXml(saveFileDialog.FileName);
                return true;
            }
            return false;
        }

        private void toolStripMenuItemDownloadAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewProjects.Rows)
            {
                if (ApplicationSyncHandler.CstrOptionClone != (string)row.Cells[CnColumnOptionsButton].Value)
                    continue;

                try
                {
                    var appSyncHndlr = row.Tag as ApplicationSyncHandler;
                    if (!appSyncHndlr.DoClone())
                        break;
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
            }
            InitializeDataGrid(_chorusConfigs);
        }

        private void toolStripMenuItemSynchronizeAll_Click(object sender, EventArgs e)
        {
            try
            {
                Program.SychronizeProjectSet(_chorusConfigs);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private void toolStripMenuItemUpdateGoogleSheet_Click(object sender, EventArgs e)
        {
            try
            {
                Program.HarvestProjectSet(_chorusConfigs);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private Project ProjectFromRow(DataGridViewRow row)
        {
            var projectId = row.Cells[CnColumnProjectId].Value as String;
            var project = _chorusConfigs.ServerSettings.SelectMany(s => s.Projects).First(p => p.ProjectId == projectId);
            return project;
        }

        private void dataGridViewProjects_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewProjects.IsCurrentCellDirty)
            {
                dataGridViewProjects.CommitEdit(DataGridViewDataErrorContexts.Commit);
                var theRow = dataGridViewProjects.CurrentRow;
                var project = ProjectFromRow(theRow);
                project.ExcludeFromSyncing = (bool)theRow.Cells[CnColumnExcludeFromSync].Value;
                project.ExcludeFromGoogleSheet = (bool)theRow.Cells[CnColumnExcludeFromGoogleSheet].Value;

                // fix the button label if it was the ExcludeFromSyncing that changed
                if (!project.ExcludeFromSyncing && (ApplicationSyncHandler.CstrOptionHidden == (string)theRow.Cells[CnColumnOptionsButton].Value))
                {
                    theRow.Cells[CnColumnOptionsButton].Value = ((ApplicationSyncHandler)theRow.Tag).ButtonLabel;
                }
                else if (project.ExcludeFromSyncing && (ApplicationSyncHandler.CstrOptionHidden != (string)theRow.Cells[CnColumnOptionsButton].Value))
                {
                    theRow.Cells[CnColumnOptionsButton].Value = ApplicationSyncHandler.CstrOptionHidden;
                }
                _modified = true;
            }
        }

        private bool _modified = false;
        private void PresentProjectsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_modified)
            {
                if (!SaveProjectSet())
                {
                    e.Cancel = true;
                    _modified = false;  // so it can close the next time
                }
            }
        }

        private void toolStripMenuItemSynchronize_Click(object sender, EventArgs e)
        {
            try
            {
                var ptMousePosition = dataGridViewProjects.PointToClient(MousePosition);
                var hti = dataGridViewProjects.HitTest(ptMousePosition.X, ptMousePosition.Y);
                if ((hti.ColumnIndex < 0) || (hti.ColumnIndex >= dataGridViewProjects.Columns.Count) ||
                    (hti.RowIndex < 0) || (hti.RowIndex >= dataGridViewProjects.Rows.Count))
                    return;

                var theRow = dataGridViewProjects.Rows[hti.RowIndex];
                if ((theRow == null) || (theRow.Tag == null))
                    return;

                Debug.Assert(theRow.Tag is ApplicationSyncHandler);
                var appSyncHandler = theRow.Tag as ApplicationSyncHandler;
                Debug.Assert(appSyncHandler != null);
                appSyncHandler.DoSynchronize(true);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }
    }
}
