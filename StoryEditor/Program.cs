using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StoryEditor
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

            bool bNeedToSave = false;
            if (bNeedToSave)
                Properties.Settings.Default.Save();

            StoryEditor.UserTypes eType = (StoryEditor.UserTypes)Properties.Settings.Default.UserType;
            Application.Run(new StoryEditor(eType));
        }

        public static bool Modified = false;
    }
}
