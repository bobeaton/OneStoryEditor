using System;
using System.Diagnostics;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// base of the HTML panes (Story/BT, Consultant Notes, Coach Notes). It holds an IHtmlHost (it used to *be* the
    /// IE WebBrowser) and talks to its page only through messages; see
    /// docs/superpowers/specs/2026-10-04-html-message-protocol-design.md
    /// </summary>
    public class HtmlVerseControl : UserControl
    {
        public const string CstrTextAreaPrefix = "ta";
        public const string CstrParagraphPrefix = "tp";
        public const string CstrButtonPrefix = "btn";

        public delegate void SetLineNumberLinkProc(string strText, int nLineIndex);
        internal SetLineNumberLinkProc SetLineNumberLink;

        public delegate void MakeLineNumberLinkVisibleProc();
        internal MakeLineNumberLinkVisibleProc MakeLineNumberLinkVisible;

        internal string StrIdToScrollTo;

        public StoryEditor TheSE { get; set; }
        public virtual StoryData StoryData { get; set; }

        protected readonly IHtmlHost Host;
        protected readonly HtmlMessageDispatcher Dispatcher = new HtmlMessageDispatcher();

        // what the page last reported about which line is at the top (the 'scrolled' message)
        private string _strTopRowId, _strPrevRowId, _strNextRowId;

        protected HtmlVerseControl()
            : this(null)
        {
        }

        protected internal HtmlVerseControl(IHtmlHost host)
        {
            Host = host ?? HtmlHostFactory.Create();
            Host.Control.Dock = DockStyle.Fill;
            Controls.Add(Host.Control);
            Host.MessageReceived += (sender, msg) => Dispatcher.Dispatch(msg);
            Host.DocumentReady += (sender, args) => OnDocumentReady();
            Dispatcher.ReportError = s => TheSE?.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), s));

            Dispatcher.Register("scrolled", OnScrolled);
            Dispatcher.Register("save", msg => TheSE?.SaveClicked());
            Dispatcher.Register("reload", msg => LoadDocument());
            Dispatcher.Register("realign", msg => OnRealign());
            Dispatcher.Register("bibRefJump", msg => OnBibRefJump(msg.GetString("ref")));
            Dispatcher.Register("openUrl", msg => OnUrlJump(msg.GetString("url")));
            Dispatcher.Register("verseLineJump", msg =>
            {
                if (msg.TryGetInt("index", out var nVerseIndex))
                    OnVerseLineJump(nVerseIndex);
            });
            Dispatcher.Register("textareaMouseDown", OnTextareaMouseDown);
            Dispatcher.Register(HtmlMessage.CstrTypeLog, msg => Debug.WriteLine(msg.GetString("text")));
            Dispatcher.Register(HtmlMessage.CstrTypeJsError, msg => { });   // the host has already logged it
        }

        public void LoadHtml(string strHtml)
        {
            Host.LoadHtml(strHtml);
        }

        public string LoadedHtml => Host.LoadedHtml;

        public void ShowPrintPreview()
        {
            Host.ShowPrintPreview();
        }

        public virtual void LoadDocument()
        {
            Debug.Assert(false);
        }

        // asks the page to send any edit it hasn't sent yet. True when it has (or when there's no page to ask)
        public bool FlushEdits(TimeSpan timeout)
        {
            if (!Host.IsReady)
                return true;
            var reply = Host.Request("flush", null, timeout);
            return (reply != null) && (reply.GetString("error") == null);
        }

        public virtual void OnVerseLineJump(int nVerseIndex)
        {
        }

        protected virtual void OnRealign()
        {
            LoadDocument();
        }

        private void OnUrlJump(string url)
        {
            // doing it this way allows us to launch the default browser defined rather than IE
            if (!String.IsNullOrEmpty(url))
                Process.Start(url);
        }

        private void OnBibRefJump(string strBibRef)
        {
            TheSE?.SetNetBibleVerse(strBibRef);
        }

        public virtual void ScrollToVerse(int nVerseIndex)
        {
            StrIdToScrollTo = VersesData.LineId(nVerseIndex);
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        private void OnScrolled(HtmlMessage msg)
        {
            _strTopRowId = msg.GetString("topId");
            _strPrevRowId = msg.GetString("prevId");
            _strNextRowId = msg.GetString("nextId");
            if ((SetLineNumberLink != null) &&
                LineLabelParser.TryParse(msg.GetString("topLabel"), out var strLinkText, out var nLineIndex))
            {
                SetLineNumberLink(strLinkText, nLineIndex);
            }
        }

        protected string GetTopRowId => _strTopRowId;
        protected string GetNextRowId => _strNextRowId ?? _strTopRowId;
        protected string GetPrevRowId => _strPrevRowId ?? _strTopRowId;

        private void OnDocumentReady()
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        protected VerseData GetVerseData(int nLineIndex)
        {
            if (StoryData.Verses.Count <= (nLineIndex - 1))
                return null;
            return (nLineIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nLineIndex - 1];
        }

        public void ScrollToElement(String strElemName, bool bAlignWithTop)
        {
            Debug.Assert(!String.IsNullOrEmpty(strElemName));
            Host.Post("scrollTo", new { id = strElemName, alignTop = bAlignWithTop, focus = !bAlignWithTop });
        }

        public void ForgetWhereYouWere()
        {
            StrIdToScrollTo = null;
        }

        public void ResetDocument()
        {
            // reset so we don't jump to a soon-to-be-non-existant (or wrong context) place
            // update: if you *don't* want to jump there, then clear out StrIdToScrollTo manually. This needs
            //  to be here (e.g. for DoMove) which wants to go back to the same spot
            Host.LoadHtml(String.Empty);
        }

        protected static readonly char[] AchDelim = new[] { '_' };

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            theSE = TheSE;
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        Localizer.Str("Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com"));

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.TheCurrentStory);
            }
            catch (Exception ex)
            {
                theSE?.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        public virtual string GetSelectedText(StringTransfer stringTransfer)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            if (!IsTextareaElement(stringTransfer.HtmlElementId))
                return null;

            var reply = Host.Request("getSelection", new { id = stringTransfer.HtmlElementId }, HtmlHostDefaults.RequestTimeout);
            if (reply == null)
                return null;

            if (reply.GetString("selType") != "text")
            {
                LocalizableMessageBox.Show(Localizer.Str("Sorry, you can only modify editable text in consultant or coach notes!"),
                                           StoryEditor.OseCaption);
                return null;
            }
            return reply.GetString("text");
        }

        public bool IsParagraphElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrParagraphPrefix) == 0));
        }

        public bool IsTextareaElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrTextAreaPrefix) == 0));
        }

        public bool IsButtonElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrButtonPrefix) == 0));
        }

        public bool SetSelectedText(StringTransfer stringTransfer, string strNewValue, out int nNewEndPoint)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            Debug.Assert(IsTextareaElement(stringTransfer.HtmlElementId));
            nNewEndPoint = 0;   // 0 means it didn't work

            var reply = Host.Request("replaceSelection", new { id = stringTransfer.HtmlElementId, text = strNewValue },
                                     HtmlHostDefaults.RequestTimeout);
            if ((reply == null) || !reply.TryGetInt("endPoint", out nNewEndPoint) || (nNewEndPoint <= 0))
            {
                nNewEndPoint = 0;   // e.g. the selected portion wasn't in the element thought
                return false;
            }

            // now we have to update the string transfer with the new value
            var strIeHtml = reply.GetString("ieHtml");
            if (strIeHtml != null)
                stringTransfer.SetValue(HtmlText.FromIeHtmlText(strIeHtml));
            return true;
        }

        public void ClearSelection(StringTransfer stringTransfer)
        {
            Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (IsTextareaElement(stringTransfer.HtmlElementId))
            {
                Host.Post("clearSelection");
            }
            else if (IsParagraphElement(stringTransfer.HtmlElementId))
            {
                var strHtml = (stringTransfer is CommInstance)
                                  ? NoteHtmlSanitizer.ToReadOnlyHtml(stringTransfer.ToString())
                                  : HtmlText.ForParagraph(stringTransfer.ToString());
                Host.Post("setHtml", new { id = stringTransfer.HtmlElementId, html = strHtml });
            }
        }

        // TextPaster sets a textarea's text this way; the page then sends 'textChanged' like any other edit
        internal void SetTextareaText(string strId, string strText)
        {
            Host.Post("setText", new { id = strId, text = strText });
        }

        // button is the value JS reports (1 == left, as before)
        private void OnTextareaMouseDown(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            if ((StoryEditor.TextPaster == null) || (strId == null) || !msg.TryGetInt("button", out var nButton))
                return;

            StoryEditor.TextPaster.TriggerPaste(nButton == 1,
                                                new TextareaRef { Pane = this, Id = strId, Text = msg.GetString("value") ?? String.Empty });
        }
    }
}
