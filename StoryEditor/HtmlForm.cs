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
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class HtmlForm : Form
    {
        protected const string preDocumentDOMScript = "<style> body  { margin:1 } </style>" +
            "<script>" +
            "function OpenHoverWindow(link)" +
            "{" +
            "  window.external.ShowHoverOver(link.innerHTML);" +
            "  return false;" +
            "}" +
            "</script>";

        protected const string postDocumentDOMScript = "<script>" +
            "var links = document.getElementsByTagName(\"a\");" +
            "for (var i=0; i < links.length; i++)" +
            "{" +
            "  links[i].onclick = function(){return OpenHoverWindow(this);};" +
            "}" +
            "</script>";

        public HtmlForm()
        {
            InitializeComponent();
            webBrowser.ObjectForScripting = this;
        }

        public new void Show()
        {
            if (Properties.Settings.Default.CommentaryDialogHeight != 0)
            {
                Bounds = new Rectangle(Properties.Settings.Default.CommentaryDialogLocation,
                    new Size(Properties.Settings.Default.CommentaryDialogWidth,
                        Properties.Settings.Default.CommentaryDialogHeight));
            }
            
            base.Show();
        }

        public StoryEditor TheSE;
        
        public string ClientText
        {
            get { return webBrowser.DocumentText; }
            set
            {
                string str = preDocumentDOMScript + value + postDocumentDOMScript;
                webBrowser.DocumentText = str;
            }
        }

        public void ShowHoverOver(string strBibRef)
        {
            if ((TheSE != null) && !String.IsNullOrEmpty(strBibRef))
                TheSE.SetNetBibleVerse(strBibRef);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void HtmlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CommentaryDialogLocation = Location;
            Properties.Settings.Default.CommentaryDialogHeight = Bounds.Height;
            Properties.Settings.Default.CommentaryDialogWidth = Bounds.Width;
            Properties.Settings.Default.Save();
        }
    }
}
