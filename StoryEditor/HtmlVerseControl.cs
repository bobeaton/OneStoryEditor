using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract partial class HtmlVerseControl : WebBrowser
    {
        protected string StrIdToScrollTo;

        public StoryEditor TheSE { get; set; }
        public virtual StoryData StoryData { get; set; }

        public virtual void ScrollToVerse(int nVerseIndex)
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        protected HtmlVerseControl()
        {
            InitializeComponent();
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
                    elem.Focus();
                }
            }
        }

        private readonly char[] _achDelim = new[] { '_' };

        protected bool GetIndicesFromId(string strId, 
            out int nVerseIndex, out int nConversationIndex)
        {
            try
            {
                string[] aVerseConversationIndices = strId.Split(_achDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices.Length == 3) ||
                                                 (aVerseConversationIndices.Length == 4))
                                                &&
                                                ((aVerseConversationIndices[0] == "ta") ||
                                                 (aVerseConversationIndices[0] == "btn")));

                nVerseIndex = Convert.ToInt32(aVerseConversationIndices[1]);
                nConversationIndex = Convert.ToInt32(aVerseConversationIndices[2]);
            }
            catch
            {
                nVerseIndex = 0;
                nConversationIndex = 0;
                return false;
            }
            return true;
        }

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

                if (!theSE.theCurrentStory.ProjStage.IsEditAllowed(theSE.LoggedOnMember))
                    throw theSE.theCurrentStory.ProjStage.WrongMemberTypeEx;
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
