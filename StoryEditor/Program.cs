using System;
using System.Collections.Generic;
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
                Application.Run(new StoryEditor(Properties.Resources.IDS_MainStoriesSet));
#if DEBUG
                if (MessageBox.Show("Do repository Sync?", Properties.Resources.IDS_Caption, MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
#endif
                foreach (string strProjectFolder in _astrProjectForSync)
                    SyncWithRepository(strProjectFolder);
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

        static void SyncWithRepository(string strProjectFolder)
        {
            using (var setup = new RepositorySetup("bobeaton"))
            {
                setup.ProjectFolderConfig.FolderPath = strProjectFolder;
                Application.EnableVisualStyles();
                setup.Repository.SetKnownRepositoryAddresses(new []
                {
                    RepositoryAddress.Create("language depot", "http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test"),
                });
                setup.Repository.SetDefaultSyncRepositoryAliases(new[] { "language depot" });
                using (var dlg = new SyncDialog(setup.ProjectFolderConfig,
                    SyncUIDialogBehaviors.Lazy, SyncUIFeatures.NormalRecommended))
                {
                    dlg.ShowDialog();
                }
            }
        }
    }
}
