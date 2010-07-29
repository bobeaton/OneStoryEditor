using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract partial class HtmlVerseControl : WebBrowser
    {
        internal LinkLabel LineNumberLink;

        protected string StrIdToScrollTo;

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
            if ((Document != null) && (Document.Body != null))
            {
                HtmlDocument doc = Document;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("doc.Body.ScrollTop: {0}", doc.Body.ScrollTop));
                foreach (HtmlElement elemLn in doc.GetElementsByTagName("td"))
                    if (!String.IsNullOrEmpty(elemLn.Id))   // so we only get "ln: n" values
                        System.Diagnostics.Debug.WriteLine(String.Format("id: {0}, or: {1}, sr: {2}, st: {3}",
                            elemLn.Id, elemLn.OffsetRectangle,
                            elemLn.ScrollRectangle,
                            elemLn.ScrollTop));
#endif

                HtmlElement elemLnPrev = null;
                foreach (HtmlElement elemLn in doc.GetElementsByTagName("td"))
                    if (!String.IsNullOrEmpty(elemLn.Id))
                    {
                        if (elemLn.OffsetRectangle.Top < doc.Body.ScrollTop)
                            elemLnPrev = elemLn;
                        else
                            break;
                    }

                if (elemLnPrev != null)
                {
                    if (elemLnPrev.InnerText == VersesData.CstrZerothLineName)
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
                        if ((nIndex = strLineNumber.IndexOf(VersesData.CstrHiddenVerseSuffix)) != -1)
                            strLineNumber = strLineNumber.Substring(0, nIndex);
                        LineNumberLink.Tag = Convert.ToInt32(strLineNumber);
                    }
                }
            }
        }

        public abstract void LoadDocument();

        private void HtmlConNoteControl_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, false);
        }

        protected VerseData Verse(int nVerseIndex)
        {
            System.Diagnostics.Debug.Assert(StoryData.Verses.Count > (nVerseIndex - 1));
            return (nVerseIndex == 0) ? StoryData.Verses.FirstVerse : StoryData.Verses[nVerseIndex - 1];
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

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.theCurrentStory.ProjStage.MemberTypeWithEditToken);
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
