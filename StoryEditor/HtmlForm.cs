using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class HtmlForm : Form
    {
        public HtmlForm()
        {
            InitializeComponent();
        }

        public string ClientText
        {
            get { return webBrowser.DocumentText; }
            set { webBrowser.DocumentText = value; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
