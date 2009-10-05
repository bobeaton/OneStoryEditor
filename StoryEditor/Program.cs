using System;
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
                SyncWithRepository();
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage,  Properties.Resources.IDS_Caption);
            }
        }

        static void SyncWithRepository()
        {
            var setup = new RepositorySetup("bobeaton");
            {
                Application.EnableVisualStyles();
                setup.Repository.SetKnownRepositoryAddresses(new RepositoryAddress[]
                {
                    RepositoryAddress.Create("language depot", "http://bobeaton:helpmepld@hg-private.languagedepot.org"),
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
