using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public class HtmlStoryBtControl : HtmlVerseControl
    {
        public const string CstrFieldNameVernacular = "Vernacular";
        public const string CstrFieldNameNationalBt = "NationalBT";
        public const string CstrFieldNameInternationalBt = "InternationalBT";

        public VerseData.ViewItemToInsureOn ViewItemsToInsureOn { get; set; }
        public TeamMembersData MembersData { get; set; }
        public TeamMemberData LoggedOnMember { get; set; }

        public HtmlStoryBtControl()
        {
            ObjectForScripting = this;
        }

        public override void LoadDocument()
        {
            string strHtml = StoryData.VersesHtml(TheSE.StoryProject.ProjSettings,
                                                  TheSE.hiddenVersesToolStripMenuItem.Checked,
                                                  MembersData, LoggedOnMember,
                                                  ViewItemsToInsureOn);
            DocumentText = strHtml;
            // LineNumberLink.Visible = true;
        }

        public bool TextareaOnKeyUp(string strId, string strText)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return false;

            int nVerseIndex;
            string strTextElement;
            if (!GetIndicesFromId(strId, out nVerseIndex, out strTextElement))
                return false;

            if (strTextElement == CstrFieldNameVernacular)
                StoryData.Verses[nVerseIndex].VernacularText.SetValue(strText);
            else if (strTextElement == CstrFieldNameNationalBt)
                StoryData.Verses[nVerseIndex].NationalBTText.SetValue(strText);
            else if (strTextElement == CstrFieldNameInternationalBt)
                StoryData.Verses[nVerseIndex].InternationalBTText.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;

            // update the status bar (in case we previously put an error there
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[theSE.theCurrentStory.ProjStage.ProjectStage];
            theSE.SetStatusBar(String.Format("{0}  Press F1 for instructions", st.StageDisplayString));

            return true;
        }

        protected bool GetIndicesFromId(string strId,
            out int nVerseIndex, out string strTextElement)
        {
            try
            {
                string[] aVerseConversationIndices = strId.Split(_achDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices.Length == 3) ||
                                                 (aVerseConversationIndices.Length == 4))
                                                &&
                                                ((aVerseConversationIndices[0] == "ta") ||
                                                 (aVerseConversationIndices[0] == "btn")));

                strTextElement = aVerseConversationIndices[0];
                nVerseIndex = Convert.ToInt32(aVerseConversationIndices[1]);
            }
            catch
            {
                nVerseIndex = 0;
                strTextElement = null;
                return false;
            }
            return true;
        }

        public void SelectFoundText(string strHtmlElementId,
            int nFoundIndex, int nLengthToSelect)
        {
            if (Document != null)
            {
                HtmlDocument doc = Document;
                object[] oaParams = new object[] { strHtmlElementId, nFoundIndex, nLengthToSelect };
                // doc.InvokeScript("textboxSelect", oaParams);
                doc.InvokeScript("paragraphSelect", oaParams);
            }
        }
    }
}
