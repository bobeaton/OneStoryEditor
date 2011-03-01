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

        private int _nIndexToScrollTo = 0;
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
            Application.DoEvents(); // to get the doc to load
            UpdateButtonEnabledState(true);
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

        private void ScrollToElement(int nElemName)
        {
            if (webBrowser.Document != null)
            {
                HtmlDocument doc = webBrowser.Document;
                HtmlElement elem = doc.GetElementById(Properties.Resources.IDS_CommentaryHeader + nElemName.ToString());
                if (elem != null)
                    elem.ScrollIntoView(true);
            }
        }

        private int _nNumResources = 0;
        public int NumberOfResources
        {
            get { return _nNumResources; }
            set 
            {
                _nNumResources = value;
                buttonNext.Visible = buttonPrev.Visible = (_nNumResources > 1);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            _nIndexToScrollTo = Math.Min(++_nIndexToScrollTo, NumberOfResources - 1);
            ScrollToElement(_nIndexToScrollTo);

            UpdateButtonEnabledState(false);
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            _nIndexToScrollTo = Math.Max(--_nIndexToScrollTo, 0);
            ScrollToElement(_nIndexToScrollTo);

            UpdateButtonEnabledState(false);
        }

        private void UpdateButtonEnabledState(bool bLoadFromSettings)
        {
            if (bLoadFromSettings)
            {
                _nIndexToScrollTo = Properties.Settings.Default.CommentaryLastIndex;
                ScrollToElement(_nIndexToScrollTo);
            }
            else
            {
                Properties.Settings.Default.CommentaryLastIndex = _nIndexToScrollTo;
                Properties.Settings.Default.Save();
            }
            buttonNext.Enabled = (_nIndexToScrollTo < (NumberOfResources - 1));
            buttonPrev.Enabled = (_nIndexToScrollTo > 0);
        }
    }
}
