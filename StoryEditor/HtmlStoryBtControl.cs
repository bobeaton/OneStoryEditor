using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public partial class HtmlStoryBtControl : HtmlVerseControl
    {
        public const string CstrFieldNameVernacular = "Vernacular";
        public const string CstrFieldNameNationalBt = "NationalBT";
        public const string CstrFieldNameInternationalBt = "InternationalBT";

        public VerseData.ViewSettings ViewSettings { get; set; }
        public StoryData ParentStory { get; set; }

        public HtmlStoryBtControl()
        {
            InitializeComponent();
            IsWebBrowserContextMenuEnabled = false;
            ObjectForScripting = this;
        }

        public override void LoadDocument()
        {
            string strHtml = null;
            StoryEditor theSe;
            bool bUseTextAreas = CheckForProperEditToken(out theSe);
            if (ParentStory != null)
                strHtml = ParentStory.PresentationHtml(ViewSettings,
                                                       TheSE.StoryProject.ProjSettings,
                                                       TheSE.StoryProject.TeamMembers,
                                                       StoryData,
                                                       bUseTextAreas);
            else if (StoryData != null)
                strHtml = StoryData.PresentationHtml(ViewSettings,
                                                     TheSE.StoryProject.ProjSettings,
                                                     TheSE.StoryProject.TeamMembers,
                                                     null,
                                                     bUseTextAreas);

            DocumentText = strHtml;
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
                DialogResult res = VerseBtControl.QueryAboutHidingVerseInstead();

                if (res == DialogResult.Yes)
                {
                    theSE.VisiblizeVerse(verseData, false);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            if (VerseBtControl.UserConfirmDeletion)
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
            int nVerseIndex, nItemIndex, nSubItemIndex;
            string strDataType, strLanguageColumn;
            if (!GetIndicesFromId(strId, out nVerseIndex, out strDataType,
                out nItemIndex, out nSubItemIndex, out strLanguageColumn))
                return false;

            StringTransfer elem = GetStringTransfer(nVerseIndex, strDataType,
                nItemIndex, nSubItemIndex, strLanguageColumn);

            if (elem == null)
                return false;

            elem.SetValue(strText);

            // indicate that the document has changed
            TheSE.Modified = true;

            // update the status bar (in case we previously put an error there
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[TheSE.TheCurrentStory.ProjStage.ProjectStage];
            TheSE.SetDefaultStatusBar(st.StageDisplayString);

            return true;
        }

        public bool OnLineOptionsButton(string strId)
        {
            if (Document == null)
                return false;

            HtmlDocument doc = Document;
            var elem = doc.GetElementById(strId);
            if (elem == null)
                return false;

            if (IsLineOptionsButton(strId))
                contextMenuStrip.Show(MousePosition);

            return true;
        }

        private bool IsLineOptionsButton(string strId)
        {
            var astr = strId.Split(_achDelim);
            return ((astr.Length == 2) &&
                    (astr[0] == VersesData.ButtonId(1).Split(_achDelim)[0]));
        }

        /*
                public void OnDoOnMouseUp(string strId)
                {
                    if (Document == null)
                        return;

                    HtmlDocument doc = Document;
                    var elem = doc.GetElementById(strId);
                    if (elem == null)
                        return;

                    if (IsLineOptionsButton(strId))
                        contextMenuStrip.Show(MousePosition);
                    else
                        MessageBox.Show(elem.InnerText, "Clicked on...");
                }
                */

        private VerseData GetVerseData(int nLineIndex)
        {
            return (nLineIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nLineIndex - 1];
        }

        private StringTransfer GetStringTransfer(int nLineIndex, 
            string strDataType, int nItemIndex, int nSubItemIndex, 
            string strLanguageColumn)
        {
            var verseData = GetVerseData(nLineIndex);
            LineData lineData;
            switch(strDataType)
            {
                case VerseBtControl.CstrFieldNameStoryLine:
                    lineData = verseData.StoryLine;
                    break;

                case VerseBtControl.CstrFieldNameExegeticalHelp:
                    return verseData.ExegeticalHelpNotes[nItemIndex];

                case RetellingsData.CstrElementLableRetelling:
                    lineData = verseData.Retellings[nItemIndex];
                    break;

                case VerseBtControl.CstrFieldNameTestQuestions:
                    lineData = verseData.TestQuestions[nItemIndex].TestQuestionLine;
                    break;

                case AnswersData.CstrElementLableAnswer:
                    lineData = verseData.TestQuestions[nItemIndex].Answers[nSubItemIndex];
                    break;

                default:
                    return null;
            }

            System.Diagnostics.Debug.Assert(lineData != null);
            switch(strLanguageColumn)
            {
                case StoryData.CstrLangVernacularStyleClassName:
                    return lineData.Vernacular;
                case StoryData.CstrLangNationalBtStyleClassName:
                    return lineData.NationalBt;
                case StoryData.CstrLangInternationalBtStyleClassName:
                    return lineData.InternationalBt;
                case StoryData.CstrLangFreeTranslationStyleClassName:
                    return lineData.FreeTranslation;
            }

            return null;
        }

        protected bool GetIndicesFromId(string strId,
            out int nLineIndex, out string strDataType, out int nItemIndex,
            out int nSubItemIndex, out string strLanguageColumn)
        {
            try
            {
                // for TextAreas:
                //  ta_<lineNum>_<dataType>_<itemNum>_<stylename>
                // where:
                //  lineNum (0-GTQ line, ln 1, etc)
                //  dataType (e.g. "Retelling", "StoryLine", etc)
                //  itemNum (e.g. "ret *1*")
                //  styleName (e.g. tells how to render it; font, etc)
                string[] aVerseConversationIndices = strId.Split(_achDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices[0] == "ta") &&
                                                 (aVerseConversationIndices.Length == 5))
                                                ||
                                                ((aVerseConversationIndices[0] == "btn") &&
                                                 (aVerseConversationIndices.Length == 3)));

                nLineIndex = Convert.ToInt32(aVerseConversationIndices[1]);
                strDataType = aVerseConversationIndices[2];
                nItemIndex = Convert.ToInt32(aVerseConversationIndices[3]);
                nSubItemIndex = 0;
                strLanguageColumn = aVerseConversationIndices[4];
            }
            catch
            {
                nLineIndex = nItemIndex = nSubItemIndex = 0;
                strDataType = strLanguageColumn = null;
                return false;
            }
            return true;
        }

        public void AddScriptureReference(string strId)
        {
            int nLineIndex;
            if (!GetIndicesFromId(strId, out nLineIndex))
                return;

            if (Document == null) 
                return;

            HtmlDocument doc = Document;
            HtmlElement elem = doc.GetElementById(strId);
            if (elem == null) 
                return;

            var verseData = GetVerseData(nLineIndex);
            var strJumpTarget = TheSE.GetNetBibleScriptureReference;
            if (verseData.Anchors.Contains(strJumpTarget))
                return;

            var anchorNew = verseData.Anchors.AddAnchorData(strJumpTarget,
                                                            strJumpTarget);
            
            List<string> astrDontCare = null;
            string str = anchorNew.PresentationHtml(null,
                                                    true,
                                                    false,
                                                    ref astrDontCare);
            
            // create a new button element out of this string of html
            var elemNew = doc.CreateElement(str);
            if (elemNew == null) 
                return;

            // don't know why, but you have to explicitly set the inner text
            elemNew.InnerText = anchorNew.JumpTarget;
            elem.AppendChild(elemNew);
            TheSE.Modified = true;
        }

        protected bool GetIndicesFromId(string strId, out int nLineIndex)
        {
            try
            {
                // for AnchorIds:
                //  anc_<lineNum>
                // where:
                //  lineNum (0-GTQ line, ln 1, etc)
                string[] aVerseConversationIndices = strId.Split(_achDelim);
                System.Diagnostics.Debug.Assert((aVerseConversationIndices[0] == "anc") &&
                                                (aVerseConversationIndices.Length == 2));

                nLineIndex = Convert.ToInt32(aVerseConversationIndices[1]);
            }
            catch
            {
                nLineIndex = 0;
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

        private void moveSelectedTextToANewLineToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moveItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuAddTestQuestion_Click(object sender, EventArgs e)
        {

        }

        private void addExegeticalCulturalNoteBelowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addNewVersesBeforeMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addANewVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addNewVersesAfterMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hideVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteTheWholeVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteVerseFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteVerseFromClipboardAfterThisOneToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void copyVerseToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void splitStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
