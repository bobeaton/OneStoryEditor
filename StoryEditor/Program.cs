using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message),  Properties.Resources.IDS_Caption);
            }
        }
    }
}
