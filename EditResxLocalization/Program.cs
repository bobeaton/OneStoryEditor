using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace EditResxLocalization
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

            var strPathToLocData = Path.Combine(Application.UserAppDataPath,
                                                "LocData");
            Localizer.Default = new Localizer(strPathToLocData,
                                              Properties.Settings.Default.LastLocalizationId);

            Application.Run(new FormEditResx());

            Properties.Settings.Default.LastLocalizationId = Localizer.Default.LanguageId;
            Properties.Settings.Default.Save();
        }

        public static void ShowException(Exception ex)
        {
            var strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            MessageBox.Show(strErrorMsg, Properties.Resources.IDS_Caption);
        }
    }
}
