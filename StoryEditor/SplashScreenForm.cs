using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            InitializeComponent();

            string strLocation = Assembly.GetExecutingAssembly().Location;
            if (!String.IsNullOrEmpty(strLocation))
            {
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo(strLocation);
                labelVersion.Text = String.Format("Version: {0}", fv.FileVersion);
            }
            labelVersion.Parent = pictureBox;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }
    }
}
