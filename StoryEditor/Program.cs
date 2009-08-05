using System;
using System.Collections.Generic;
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

            if (Properties.Settings.Default.RecentFiles == null)
                Properties.Settings.Default.RecentFiles = new System.Collections.Specialized.StringCollection();

            bool bNeedToSave = false;
            if (bNeedToSave)
                Properties.Settings.Default.Save();

            try
            {
                Application.Run(new StoryEditor());
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message), StoryEditor.cstrCaption);
            }
        }
    }
}
