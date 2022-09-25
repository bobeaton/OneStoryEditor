using CircularProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SplashScreenBusyTransliterator : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
                int nLeftRect,
                int nTopRect,
                int nRightRect,
                int nBottomRect,
                int nWidthEllipse,
                int nHeightEllipse
            );

        public SplashScreenBusyTransliterator(int count)
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            circularProgressBar.Maximum = count;
            circularProgressBar.Value = 0;
        }

        public void BumpProgress()
        {
            circularProgressBar.Value += 1;
        }

        internal void Disable()
        {
            Close();
        }
    }
}
