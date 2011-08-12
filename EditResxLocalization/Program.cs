using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            Application.Run(new FormEditResx());
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
