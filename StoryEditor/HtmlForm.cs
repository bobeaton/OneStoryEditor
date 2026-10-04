using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class HtmlForm : Form
    {
        private const string CstrStyle = "<style> body  { margin:1 } </style>";

        private readonly IHtmlHost _htmlHost;
        private readonly HtmlMessageDispatcher _dispatcher = new HtmlMessageDispatcher();
        private int _nIndexToScrollTo = 0;

        public HtmlForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            // IE's own context menu and file drop were on for this form before, so keep them
            _htmlHost = HtmlHostFactory.Create(new HtmlHostOptions { AllowBrowserContextMenu = true, AllowFileDrop = true });
            var ctrl = _htmlHost.Control;
            ctrl.Dock = DockStyle.None;
            ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ctrl.Location = new Point(0, 0);
            ctrl.MinimumSize = new Size(20, 20);
            ctrl.Name = "webBrowser";
            ctrl.Size = new Size(455, 364);
            ctrl.TabIndex = 0;
            Controls.Add(ctrl);
            ctrl.SendToBack();      // the designer added it after the buttons, i.e. behind them

            _dispatcher.Register("hoverRef", m => ShowHoverOver(m.GetString("ref")));
            _htmlHost.MessageReceived += (s, m) => _dispatcher.Dispatch(m);
            _htmlHost.DocumentReady += (s, e) => UpdateButtonEnabledState(true);
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
            get { return _htmlHost.LoadedHtml; }
            set
            {
                _htmlHost.LoadHtml(CstrStyle +
                                   PageScripts.ScriptBlock(PageScripts.Bridge, PageScripts.Get("HoverLinks.js")) +
                                   value);
            }
        }

        private void ShowHoverOver(string strBibRef)
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

        private bool ScrollToElement(int nElemName)
        {
            var reply = _htmlHost.Request("scrollTo",
                                          new { id = NetBibleViewer.CommentaryHeader + nElemName, alignTop = true },
                                          HtmlHostDefaults.RequestTimeout);
            return (reply != null) && reply.GetBool("found");
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
            while (!ScrollToElement(_nIndexToScrollTo) && (++_nIndexToScrollTo <= NumberOfResources - 1))
                ;

            UpdateButtonEnabledState(false);
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            _nIndexToScrollTo = Math.Max(--_nIndexToScrollTo, 0);
            while (!ScrollToElement(_nIndexToScrollTo) && (--_nIndexToScrollTo >= 0))
                ;

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
