using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
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
            /*
            string strHtml = StoryData.VersesHtml(TheSE.StoryProject.ProjSettings,
                                                  TheSE.hiddenVersesToolStripMenuItem.Checked,
                                                  MembersData, LoggedOnMember,
                                                  ViewItemsToInsureOn);
            string strXml = @"<story name=""Jesus invited to dinner"" stage=""ConsultantCheckStoryInfo"" guid=""0e2e37a9-de08-4a4d-ad09-d3d99b44306a"" stageDateTimeStamp=""2009-11-13T18:12:43.0863848"">
      <CraftingInfo NonBiblicalStory=""false"">
        <StoryCrafter memberID=""mem-4f3fd25c-827a-4ce8-ab07-7286bd1980e1"" />
        <StoryPurpose>purpose</StoryPurpose>
        <ResourcesUsed>resources</ResourcesUsed>
        <BackTranslator memberID=""mem-73324703-2f5c-42d3-b28e-13d0f77694be"" />
      </CraftingInfo>
      <verses>
        <verse guid=""d26d41b5-472b-45b2-8db3-79290d3ff478"">
          <Vernacular>किसे शहुकार माह़णू दे दुईं कर्जदार थी।</Vernacular>
          <NationalBT>किसी अमीर मानू के दो ही कर्ज़दार।</NationalBT>
          <InternationalBT>A rich fellow had only two debtor.</InternationalBT>
          <anchors keyTermChecked=""false"">
            <anchor jumpTarget=""Luke 7:41"">
              <toolTip>Luke 7:42</toolTip>
            </anchor>
          </anchors>
        </verse>
        <verse guid=""e627fd0d-5d38-4331-98d1-f51ae8493fef"">
          <Vernacular>उना कल इन्‍नी हिम्‍मत नेई नेई सी के ओह़ अपने मालिक दा कर्जा दे सकह़न।</Vernacular>
          <NationalBT>उनके पोस इतनी हिम्‍मत नही थी कि नही थी वे अपने मालिक का कर्ज़ा दे सकें।</NationalBT>
          <InternationalBT>they did not have enough enough courage that they could pay their master.</InternationalBT>
          <anchors keyTermChecked=""false"">
            <anchor jumpTarget=""Luke 7:42"">
              <toolTip>Luke 7:43</toolTip>
            </anchor>
          </anchors>
          <TestQuestions>
            <TestQuestion visible=""true"" guid=""d2644a8f-4c7d-48c7-9f91-3bb9c63af5d4"">
              <TQInternationalBT>Why didn't they have courage?</TQInternationalBT>
            </TestQuestion>
          </TestQuestions>
        </verse>
      </verses>
    </story>";
            var dom = new XmlDocument();
            dom.LoadXml(strXml);
            StoryData sd = new StoryData(dom.FirstChild);
            */
            string strHtml = StoryData.PresentationHtml(TheSE.StoryProject.ProjSettings, TheSE.StoryProject.TeamMembers, null);

            DocumentText = strHtml;
            // LineNumberLink.Visible = true;
        }

        public void InsertNewVerseBefore(int nVerseIndex)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.AddNewVerse(nVerseIndex - 1, 1, false);
        }

        public void AddNewVerseAfter(int nVerseIndex)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.AddNewVerse(nVerseIndex - 1, 1, true);
        }

        public void HideVerse(int nVerseIndex)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            VerseData verseData = Verse(nVerseIndex);
            theSE.VisiblizeVerse(verseData,
                (verseData.IsVisible) ? false : true   // toggle
                );
        }

        public void DeleteVerse(int nVerseIndex)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            VerseData verseData = Verse(nVerseIndex);
            if (verseData.HasData)
            {
                DialogResult res = MessageBox.Show(
                    Properties.Resources.IDS_VerseNotEmptyHideQuery,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    theSE.VisiblizeVerse(verseData, false);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            if (MessageBox.Show(
                Properties.Resources.IDS_DeleteVerseQuery,
                OseResources.Properties.Resources.IDS_Caption,
                MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                theSE.DeleteVerse(verseData);
            }
        }

        protected static VerseData _myClipboard = null;
        public void CopyVerse(int nVerseIndex)
        {
            try
            {
                // Copies the verse to the clipboard.
                // Clipboard.SetDataObject(_verseData);
                // make a copy so that if the user makes changes after the copy, we won't be
                //  referring to the same object.
                VerseData verseData = Verse(nVerseIndex);
                _myClipboard = new VerseData(verseData);
            }
            catch   // ignore errors
            {
            }
        }

        public void PasteVerseBefore(int nVerseIndex)
        {
            PasteVerseToIndex(nVerseIndex - 1);
        }

        public void PasteVerseAfter(int nVerseIndex)
        {
            PasteVerseToIndex(nVerseIndex);
        }

        protected void PasteVerseToIndex(int nInsertionIndex)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            if (_myClipboard != null)
            {
                VerseData theNewVerse = new VerseData(_myClipboard);
                theNewVerse.AllowConNoteButtonsOverride();
                // make another copy, so that the guid is changed
                theSE.DoPasteVerse(nInsertionIndex, theNewVerse);
            }
        }

        public void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, true, true);
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
