using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetLoc;
using mshtml;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public class HtmlVerseControl : WebBrowser
    {
        public const string CstrTextAreaPrefix = "ta";
        public const string CstrParagraphPrefix = "tp";
        public const string CstrButtonPrefix = "btn";

        internal LinkLabel LineNumberLink;

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

        public void OnScroll()
        {
            var elemLnPrev = GetTopHtmlElementId("td");
            if (elemLnPrev == null)
                return;

            if (StoryEditor.IsFirstCharsEqual(elemLnPrev.InnerText,
                                              VersesData.CstrZerothLineNameConNotes,
                                              VersesData.CstrZerothLineNameConNotes.Length))
            {
                LineNumberLink.Text = StoryEditor.CstrFirstVerse;
                LineNumberLink.Tag = 0;
            }
            else
            {
                LineNumberLink.Text = elemLnPrev.InnerText;
                int nIndex = elemLnPrev.InnerText.IndexOf(' ');
                if (nIndex == -1) 
                    return;
                string strLineNumber = elemLnPrev.InnerText.Substring(nIndex + 1);
                if ((nIndex = strLineNumber.IndexOf(VersesData.HiddenStringSpace)) != -1)
                    strLineNumber = strLineNumber.Substring(0, nIndex);
                LineNumberLink.Tag = Convert.ToInt32(strLineNumber);
            }
        }

        protected string GetTopRowId
        {
            get
            {
                var elem = GetTopHtmlElementId("tr");
                return (elem != null) ? elem.Id : null;
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

        protected VerseData Verse(int nVerseIndex)
        {
            Debug.Assert(StoryData.Verses.Count > (nVerseIndex - 1));
            return (nVerseIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nVerseIndex - 1];
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
                    elem.ScrollIntoView(bAlignWithTop);
                    if (!bAlignWithTop)
                        elem.Focus();
                }
            }
        }

        public void ResetDocument()
        {
            //reset so we don't jump to a soon-to-be-non-existant (or wrong context) place
            StrIdToScrollTo = null;
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

        public string GetSelectedText(StringTransfer stringTransfer)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            Debug.Assert(IsTextareaElement(stringTransfer.HtmlElementId));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                if (htmlDocument != null)
                {
                    IHTMLSelectionObject selection = htmlDocument.selection;
                    if (selection.type.ToLower() != "text")
                    {
                        LocalizableMessageBox.Show(Localizer.Str("Sorry, you can only modify editable text in consultant or coach notes!"),
                            StoryEditor.OseCaption);
                    }
                    else
                    {
                        IHTMLTxtRange rangeSelection = selection.createRange() as IHTMLTxtRange;
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
            return (strHtmlId.IndexOf(CstrParagraphPrefix) == 0);
        }

        public bool IsTextareaElement(string strHtmlId)
        {
            return (strHtmlId.IndexOf(CstrTextAreaPrefix) == 0);
        }

        public bool IsButtonElement(string strHtmlId)
        {
            return (strHtmlId.IndexOf(CstrButtonPrefix) == 0);
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
    }
}
