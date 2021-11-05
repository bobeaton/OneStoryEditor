using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetLoc;
using mshtml;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public class HtmlVerseControl : WebBrowser
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

        protected HtmlVerseControl()
        {
            DocumentCompleted += HtmlConNoteControl_DocumentCompleted;
        }

        public virtual void ScrollToVerse(int nVerseIndex)
        {
            StrIdToScrollTo = VersesData.LineId(nVerseIndex);
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        public void OnSaveDocument()
        {
            TheSE.SaveClicked();
        }

        public void OnScroll()
        {
            var elemLnPrev = GetTopHtmlElementId("td");
            if ((elemLnPrev == null) || (SetLineNumberLink == null))
                return;

            if (StoryEditor.IsFirstCharsEqual(elemLnPrev.InnerText,
                                              VersesData.CstrZerothLineNameConNotes,
                                              VersesData.CstrZerothLineNameConNotes.Length))
            {
                SetLineNumberLink(StoryEditor.CstrFirstVerse, 0);
            }
            else if (StoryEditor.IsFirstCharsEqual(elemLnPrev.InnerText,
                                                   VersesData.CstrZerothLineNameBtPane,
                                                   VersesData.CstrZerothLineNameBtPane.Length))
            {
                SetLineNumberLink(VersesData.CstrZerothLineNameBtPane, 0);
            }
            else if (StoryEditor.IsFirstCharsEqual(elemLnPrev.InnerText,
                                                   VersesData.LinePrefix,
                                                   VersesData.LinePrefix.Length))
            {
                // e.g.
                //  "Ln: 1" (or for the French localization: "Ln : 1")
                //  "Ln: 1 (Hidden)"
                var strLabel = elemLnPrev.InnerText;
                
                // if the 'Hidden' keyword is showing, then strip that off
                int nIndex;
                var bHidden = ((nIndex = strLabel.IndexOf(VersesData.HiddenStringSpace)) != -1);
                if (bHidden)
                    strLabel = strLabel.Substring(0, nIndex);

                // the verse number should be the last bit after the last space (French has a space before the colon)
                nIndex = strLabel.LastIndexOf(' ');
                if (nIndex == -1) 
                    return;

                var strLineNumber = strLabel.Substring(nIndex + 1);
                SetLineNumberLink(strLabel, Convert.ToInt32(strLineNumber));
            }
        }

        protected string GetTopRowId
        {
            get
            {
                var elem = GetTopHtmlElementId("td");
                return (elem != null) ? elem.Id : null;
            }
        }

        protected string GetNextRowId
        {
            get
            {
                var topRow = GetTopRowId;
                if (topRow != null)
                {
                    var astr = topRow.Split(AchDelim);
                    if ((astr.Length == 2) && (astr[0] == VersesData.CstrLinePrefix))
                    {
                        var nextRow = VersesData.LineId(Int32.Parse(astr[1]) + 1);
                        if ((Document != null) && Document.GetElementById(nextRow) != null)
                            topRow = nextRow;
                    }
                }
                return topRow;
            }
        }

        protected string GetPrevRowId
        {
            get
            {
                var topRow = GetTopRowId;
                if (topRow != null)
                {
                    var astr = topRow.Split(AchDelim);
                    if ((astr.Length == 2) && (astr[0] == VersesData.CstrLinePrefix))
                    {
                        var prevRow = VersesData.LineId(Int32.Parse(astr[1]) - 1);
                        if ((Document != null) && Document.GetElementById(prevRow) != null)
                            topRow = prevRow;
                    }
                }
                return topRow;
            }
        }

        private static void HtmlElementTotalScrollTop(HtmlElement elem, 
            ref int nTopOffset, ref int nTopScroll)
        {
            nTopOffset += elem.OffsetRectangle.Top;
            nTopScroll += elem.ScrollTop;
            if (elem.OffsetParent != null)
                HtmlElementTotalScrollTop(elem.OffsetParent, ref nTopOffset, ref nTopScroll);
        }

        private HtmlElement GetTopHtmlElementId(string strElementTagName)
        {
            HtmlElement elemLnPrev = null;
            HtmlDocument doc;
            int nScrollTop;
            if (((doc = Document) != null) &&
                ((elemLnPrev = doc.Body) != null) &&
                (nScrollTop = elemLnPrev.ScrollTop) >= 0)
            {
#if DEBUGBOB
                // in debug, dump out the position of all the rows that have IDs
                HtmlWindow window = doc.Window;
                var domwindow = (mshtml.IHTMLWindow3)window.DomWindow;
                var screenY = domwindow.screenTop;
                System.Diagnostics.Debug.WriteLine(String.Format("GetTopRow: doc.Body.ScrollTop: {0}, screenY: {1}",
                                                                 nScrollTop,
                                                                 screenY));

                foreach (var elemLn in
                    doc.GetElementsByTagName(strElementTagName).Cast<HtmlElement>().Where(elemLn =>
                            !String.IsNullOrEmpty(elemLn.Id)))
                {
                    int nTopOffset = 0, nTopScroll = 0;
                    HtmlElementTotalScrollTop(elemLn, ref nTopOffset, ref nTopScroll);
                    System.Diagnostics.Debug.WriteLine(String.Format("id: {0}, to: {1}, ts: {2}",
                                                                     elemLn.Id,
                                                                     nTopOffset,
                                                                     nTopScroll));
                }
#endif
                var docWindow = doc.Window;
                elemLnPrev = doc.GetElementFromPoint(new Point { X = docWindow.Position.X, Y = docWindow.Position.Y });
                
                // if we don't get anything back from this, then go back to the original of finding it from the next highest line
                if (elemLnPrev == null)
                {
                    // get all the 'row' elements that have 'ids' (these are the ones that we 
                    //  can scroll to if the need arises)
                    foreach (var elemLn in
                        doc.GetElementsByTagName(strElementTagName).Cast<HtmlElement>().
                            Where(elemLn => !String.IsNullOrEmpty(elemLn.Id)))
                    {
                        // the first time through, the lhs might be a small # and the rhs 0
                        //  so if st is 0, just pick the first one
                        int nTopOffset = 0, nTopScroll = 0;
                        HtmlElementTotalScrollTop(elemLn, ref nTopOffset, ref nTopScroll);
                        if (nTopOffset <= (nScrollTop + 1))
                            elemLnPrev = elemLn;
                        else
                            break;
                    }
                }
            }

            return elemLnPrev;
        }

        public virtual void LoadDocument()
        {
            Debug.Assert(false);
        }

        private void HtmlConNoteControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        protected VerseData GetVerseData(int nLineIndex)
        {
            Debug.Assert(StoryData.Verses.Count > (nLineIndex - 1));
            return (nLineIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nLineIndex - 1];
        }

        public void ScrollToElement(String strElemName, bool bAlignWithTop)
        {
            Debug.Assert(!String.IsNullOrEmpty(strElemName));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                HtmlElement elem = doc.GetElementById(strElemName);
                if (elem != null)
                {
                    // for some reason, there's an event waiting that wants to bring the doc to the top... so let that run first...
                    Application.DoEvents();
                    elem.ScrollIntoView(bAlignWithTop);
                    if (!bAlignWithTop)
                        elem.Focus();
                }
            }
        }

        public void ForgetWhereYouWere()
        {
            StrIdToScrollTo = null;
        }

        public void ResetDocument()
        {
            //reset so we don't jump to a soon-to-be-non-existant (or wrong context) place
            // update: if you *don't* want to jump there, then clear out StrIdToScrollTo manually. This needs
            //  to be here (e.g. for DoMove) which wants to go back to the same spot
            // StrIdToScrollTo = null;
            if (Document != null)
                Document.OpenNew(true);
        }

        public bool OnBibRefJump(string strBibRef)
        {
            TheSE.SetNetBibleVerse(strBibRef);
            return true;
        }

        protected static readonly char[] AchDelim = new[] { '_' };

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            theSE = TheSE;  // (StoryEditor)FindForm();
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
                if (theSE != null)
                    theSE.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        public virtual string GetSelectedText(StringTransfer stringTransfer)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            if (IsTextareaElement(stringTransfer.HtmlElementId) && (Document != null))
            {
                var doc = Document;
                var htmlDocument = doc.DomDocument as IHTMLDocument2;
                if (htmlDocument != null)
                {
                    var selection = htmlDocument.selection;
                    if (selection.type.ToLower() != "text")
                    {
                        LocalizableMessageBox.Show(Localizer.Str("Sorry, you can only modify editable text in consultant or coach notes!"),
                            StoryEditor.OseCaption);
                    }
                    else
                    {
                        var rangeSelection = selection.createRange() as IHTMLTxtRange;
                        if (rangeSelection != null)
                            return rangeSelection.text;
                        // else otherwise nothing selected, so just return
                    }
                }
            }

            return null;
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
            nNewEndPoint = 0;   // return of 0 means it didn't work.
            if (Document != null)
            {
                HtmlDocument doc = Document;
                IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                if (htmlDocument != null)
                {
                    object[] oaParams = new object[] { stringTransfer.HtmlElementId, strNewValue };
                    nNewEndPoint = (int)doc.InvokeScript("textboxSetSelectionTextReturnEndPosition", oaParams);

                    // zero return means it failed (e.g. the selected portion wasn't in the element thought)
                    if (nNewEndPoint > 0)
                    {
                        // now we have to update the string transfer with the new value
                        HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                        if (elem != null)
                            stringTransfer.SetValue(elem.InnerHtml);
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearSelection(StringTransfer stringTransfer)
        {
            Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                if (IsTextareaElement(stringTransfer.HtmlElementId))
                {
                    IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                    if (htmlDocument != null)
                    {
                        IHTMLSelectionObject selection = htmlDocument.selection;
                        selection.empty();
                    }
                }
                else if (IsParagraphElement(stringTransfer.HtmlElementId))
                {
                    HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                    if (elem != null)
                        elem.InnerHtml = stringTransfer.ToString();
                    else
                        Debug.Assert(false, "unexpected element id in HTML");
                }
            }
        }

        // nButton == 1 for left and 2 for right
        public void OnTextareaMouseDown(string strId, string strText, int nButton)
        {
            if ((StoryEditor.TextPaster != null) && (Document != null))
            {
                var elemTextArea = Document.GetElementById(strId);
                StoryEditor.TextPaster.TriggerPaste((nButton == 1), elemTextArea);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HtmlVerseControl
            // 
            this.AllowNavigation = false;
            this.AllowWebBrowserDrop = false;
            this.ResumeLayout(false);

        }
    }
}
