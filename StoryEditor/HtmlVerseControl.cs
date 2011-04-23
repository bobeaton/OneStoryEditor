using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract partial class HtmlVerseControl : WebBrowser
    {
        internal LinkLabel LineNumberLink;

        internal string StrIdToScrollTo;

        public StoryEditor TheSE { get; set; }
        public virtual StoryData StoryData { get; set; }

        protected HtmlVerseControl()
        {
            InitializeComponent();
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
                LineNumberLink.Text = "Story (Ln: 0)";
                LineNumberLink.Tag = 0;
            }
            else
            {
                LineNumberLink.Text = elemLnPrev.InnerText;
                int nIndex = elemLnPrev.InnerText.IndexOf(' ');
                System.Diagnostics.Debug.Assert(nIndex != -1);
                string strLineNumber = elemLnPrev.InnerText.Substring(nIndex + 1);
                if ((nIndex = strLineNumber.IndexOf(OseResources.Properties.Resources.IDS_HiddenLabel)) != -1)
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
#if DEBUG
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

        public abstract void LoadDocument();

        private void HtmlConNoteControl_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        protected VerseData Verse(int nVerseIndex)
        {
            System.Diagnostics.Debug.Assert(StoryData.Verses.Count > (nVerseIndex - 1));
            return (nVerseIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nVerseIndex - 1];
        }

        private void ScrollToElement(String strElemName, bool bAlignWithTop)
        {
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

        public bool OnBibRefJump(string strBibRef)
        {
            TheSE.SetNetBibleVerse(strBibRef);
            return true;
        }

        protected readonly char[] _achDelim = new[] { '_' };

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            theSE = TheSE;  // (StoryEditor)FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        "Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com");

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.theCurrentStory);
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message));
                return false;
            }

            return true;
        }
    }
}
