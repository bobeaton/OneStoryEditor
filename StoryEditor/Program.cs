using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;
using LibChorus.Tests;

namespace OneStoryProjectEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.RecentProjects == null)
                Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.RecentProjectPaths == null)
                Properties.Settings.Default.RecentProjectPaths = new System.Collections.Specialized.StringCollection();

            bool bNeedToSave = false;
            System.Diagnostics.Debug.Assert(Properties.Settings.Default.RecentProjects.Count == Properties.Settings.Default.RecentProjectPaths.Count);
            if (Properties.Settings.Default.RecentProjects.Count != Properties.Settings.Default.RecentProjectPaths.Count)
            {
                Properties.Settings.Default.RecentProjects.Clear();
                Properties.Settings.Default.RecentProjectPaths.Clear();
                bNeedToSave = true;
            }

            if (bNeedToSave)
                Properties.Settings.Default.Save();

            try
            {
                // one of the first things this might do is try to get a project from the internet, in which case
                //  the OneStory folder should exist
                if (!Directory.Exists(ProjectSettings.OneStoryProjectFolderRoot))
                    Directory.CreateDirectory(ProjectSettings.OneStoryProjectFolderRoot);

                Application.Run(new StoryEditor(Properties.Resources.IDS_MainStoriesSet));
#if DEBUG
                if (_astrProjectForSync.Count > 0)
                    if (MessageBox.Show("Do repository Sync?", Properties.Resources.IDS_Caption, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
#endif
                foreach (string strProjectFolder in _astrProjectForSync)
                    SyncWithRepository(strProjectFolder, false);
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, Properties.Resources.IDS_Caption);
            }
        }

        static List<string> _astrProjectForSync = new List<string>();

        public static void SetProjectForSyncage(string strProjectFolder)
        {
            if (!_astrProjectForSync.Contains(strProjectFolder))
                _astrProjectForSync.Add(strProjectFolder);
        }

        // string strRepoPath = String.Format("http://bobeaton:helpmepld@hg-private.languagedepot.org/{0}", strProjectFolderName);

        public static void SyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);
            string strRepoUrl;
            if (!QueryHgRepoParameters(strProjectName, out strRepoUrl))
                return;

            using (var setup = new RepositorySetup(Properties.Settings.Default.HgRepoUsername))
            {
                setup.ProjectFolderConfig.FolderPath = strProjectFolder;
                setup.ProjectFolderConfig.IncludePatterns.Add("*.onestory");
                setup.ProjectFolderConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
                Application.EnableVisualStyles();
                
                setup.Repository.SetKnownRepositoryAddresses(new []
                {
                    RepositoryAddress.Create("language depot", strRepoUrl),
                });

                setup.Repository.SetDefaultSyncRepositoryAliases(new[] { "language depot" });
                using (var dlg = new SyncDialog(setup.ProjectFolderConfig,
                    (bIsOpening) ? SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished : SyncUIDialogBehaviors.Lazy, 
                    (bIsOpening) ? SyncUIFeatures.Minimal : SyncUIFeatures.NormalRecommended))
                {
                    dlg.ShowDialog();
                }
            }
        }

        private static bool QueryHgRepoParameters(string strProjectName, out string strRepoUrl)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.HgRepoUrl))
            {
                HgRepoForm dlg = new HgRepoForm
                                        {
                                            ProjectName = strProjectName,
                                            UrlBase = "http://hg-private.languagedepot.org",
                                            Username = Properties.Settings.Default.HgRepoUsername,
                                            Password = Properties.Settings.Default.HgRepoPassword
                                        };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.HgRepoUrl = strRepoUrl = dlg.Url;
                    Properties.Settings.Default.HgRepoUsername = dlg.Username;
                    Properties.Settings.Default.HgRepoPassword = dlg.Password;
                    Properties.Settings.Default.Save();
                    return true;
                }
            }
            else
            {
                strRepoUrl = Properties.Settings.Default.HgRepoUrl;
                return true;
            }

            strRepoUrl = null;
            return false;
        }
    }
}
