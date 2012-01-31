using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public partial class HtmlStoryBtControl : HtmlVerseControl
    {
        /* obsolete?
        public const string CstrFieldNameVernacular = "Vernacular";
        public const string CstrFieldNameNationalBt = "NationalBT";
        public const string CstrFieldNameInternationalBt = "InternationalBT";

        public const string CstrFieldNameStoryLine = "StoryLine";
        public const string CstrFieldNameAnchors = "Anchors";
        public const string CstrFieldNameExegeticalHelp = "ExegeticalHelp";
        public const string CstrFieldNameExegeticalHelpLabel = "ExegeticalHelpLabel";
        public const string CstrFieldNameRetellings = "Retellings";
        public const string CstrFieldNameTestQuestions = "TestQuestions";
        */
        public static DirectableEncConverter TransliteratorVernacular;
        public static DirectableEncConverter TransliteratorNationalBt;
        public static DirectableEncConverter TransliteratorInternationalBt;
        public static DirectableEncConverter TransliteratorFreeTranslation;

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
            if (ParentStory != null)
                strHtml = ParentStory.PresentationHtml(ViewSettings,
                                                       TheSE.StoryProject.ProjSettings,
                                                       TheSE.StoryProject.TeamMembers,
                                                       StoryData);
            else if (StoryData != null)
                strHtml = StoryData.PresentationHtml(ViewSettings,
                                                     TheSE.StoryProject.ProjSettings,
                                                     TheSE.StoryProject.TeamMembers,
                                                     null);

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

        public static DialogResult QueryAboutHidingVerseInstead()
        {
            return LocalizableMessageBox.Show(
                String.Format(Localizer.Str("This line isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around to know what it used to be.{0}{0}Click 'Yes' to hide the line or click 'No' to delete it?"),
                              Environment.NewLine),
                StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel);
        }

        public static bool UserConfirmDeletion
        {
            get
            {
                return (LocalizableMessageBox.Show(
                    Localizer.Str("Are you sure you want to delete this line (and all associated consultant notes, etc)?"),
                    StoryEditor.OseCaption,
                    MessageBoxButtons.YesNoCancel) == DialogResult.Yes);
            }
        }

        public void DeleteVerse(int nVerseIndex)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            VerseData verseData = Verse(nVerseIndex);
            if (verseData.HasData)
            {
                DialogResult res = QueryAboutHidingVerseInstead();

                if (res == DialogResult.Yes)
                {
                    theSE.VisiblizeVerse(verseData, false);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            if (UserConfirmDeletion)
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

        public new string GetSelectedText
        {
            get 
            {
                return GetSelectedText(GetStringTransferOfLastTextAreaInFocus);
            }
        }

        public StringTransfer GetStringTransferOfLastTextAreaInFocus
        {
            get
            {
                if (String.IsNullOrEmpty(LastTextareaInFocusId))
                    return null;

                int nVerseIndex, nItemIndex, nSubItemIndex;
                string strDataType, strLanguageColumn;
                if (!GetIndicesFromId(LastTextareaInFocusId, out nVerseIndex, out strDataType,
                                      out nItemIndex, out nSubItemIndex, out strLanguageColumn))
                    return null;


                return GetStringTransfer(nVerseIndex, strDataType,
                                         nItemIndex, nSubItemIndex, strLanguageColumn);
            }
        }

        public static string LastTextareaInFocusId { get; set; }

        public void TriggerChangeUpdate()
        {
            // we only update the StringTransfer for a textarea when the user leaves (onchange), 
            //  so when the user saves, sometimes, we need to trigger that call.
            if ((LastTextareaInFocusId == null) || (Document == null))
                return;

            HtmlDocument doc = Document;
            var elem = doc.GetElementById(LastTextareaInFocusId);
            if (elem == null)
                return;
            elem.InvokeMember("onchange");
        }

        public bool TextareaOnKeyUp(string strId, string strText)
        {
            // we'll get the value updates during OnChange, but in order to enable 
            //  the save menu, we have to set modified
            LastTextareaInFocusId = strId;
            TheSE.Modified = true;
            return true;
        }

        public bool TextareaOnChange(string strId, string strText)
        {
            var stringTransfer = GetStringTransfer(strId);
            if (stringTransfer == null) 
                return false;

            stringTransfer.SetValue(strText);

            // indicate that the document has changed
            TheSE.Modified = true;

            // update the status bar (in case we previously put an error there
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[TheSE.TheCurrentStory.ProjStage.ProjectStage];
            TheSE.SetDefaultStatusBar(st.StageDisplayString);

            return true;
        }

        private StringTransfer GetStringTransfer(string strId)
        {
            int nVerseIndex, nItemIndex, nSubItemIndex;
            string strDataType, strLanguageColumn;
            if (!GetIndicesFromId(strId, out nVerseIndex, out strDataType,
                                  out nItemIndex, out nSubItemIndex, out strLanguageColumn))
                return null;

            StringTransfer stringTransfer = GetStringTransfer(nVerseIndex, strDataType,
                                                              nItemIndex, nSubItemIndex, strLanguageColumn);
            return stringTransfer;
        }

        public bool TextareaOnBlur(string strId, string strText)
        {
            return false;
        }

        public bool TextareaOnFocus(string strId)
        {
            LastTextareaInFocusId = strId;
            return false;
        }

        public bool TextareaOnSelect(string strId, int nStartIndex, int nLength)
        {
            return false;
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
            var astr = strId.Split(AchDelim);
            return ((astr.Length == 2) &&
                    (astr[0] == VersesData.ButtonId(1).Split(AchDelim)[0]));
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
            var fieldType = (StoryEditor.TextFields) Enum.Parse(typeof (StoryEditor.TextFields), strDataType);
            LineData lineData;
            switch (fieldType)
            {
                case StoryEditor.TextFields.StoryLine:
                    lineData = verseData.StoryLine;
                    break;

                case StoryEditor.TextFields.ExegeticalNote:
                    return verseData.ExegeticalHelpNotes[nItemIndex];

                case StoryEditor.TextFields.Retelling:
                    lineData = verseData.Retellings[nItemIndex];
                    break;

                case StoryEditor.TextFields.TestQuestion:
                    lineData = verseData.TestQuestions[nItemIndex].TestQuestionLine;
                    break;

                case StoryEditor.TextFields.TestQuestionAnswer:
                    lineData = verseData.TestQuestions[nItemIndex].Answers[nSubItemIndex];
                    break;

                default:
                    return null;
            }

            System.Diagnostics.Debug.Assert(lineData != null);
            var languageColumn = (StoryEditor.TextFields)Enum.Parse(typeof (StoryEditor.TextFields), strLanguageColumn);
            switch (languageColumn)
            {
                case StoryEditor.TextFields.Vernacular:
                    return lineData.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return lineData.NationalBt;
                case StoryEditor.TextFields.InternationalBt:
                    return lineData.InternationalBt;
                case StoryEditor.TextFields.FreeTranslation:
                    return lineData.FreeTranslation;
            }

            return null;
        }

        protected static bool GetIndicesFromId(string strId,
            out int nLineIndex, out string strDataType, out int nItemIndex,
            out int nSubItemIndex, out string strLanguageColumn)
        {
            try
            {
                // for TextAreas:
                //  ta_<lineNum>_<dataType>_<itemNum>_<subItemNum>_<stylename>
                // where:
                //  lineNum (0-GTQ line, ln 1, etc)
                //  dataType (e.g. "Retelling", "StoryLine", etc)
                //  itemNum (e.g. "TQ *1*")
                //  subItemNum (e.g. "TQ 1.Ans *3*)
                //  langName (e.g. Vernacular, etc)
                string[] aVerseConversationIndices = strId.Split(AchDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices[0] == CstrTextAreaPrefix) &&
                                                 (aVerseConversationIndices.Length == 6))
                                                ||
                                                ((aVerseConversationIndices[0] == CstrButtonPrefix) &&
                                                 (aVerseConversationIndices.Length == 3)));

                nLineIndex = Convert.ToInt32(aVerseConversationIndices[1]);
                strDataType = aVerseConversationIndices[2];
                nItemIndex = Convert.ToInt32(aVerseConversationIndices[3]);
                nSubItemIndex = Convert.ToInt32(aVerseConversationIndices[4]);
                strLanguageColumn = aVerseConversationIndices[5];
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
                string[] aVerseConversationIndices = strId.Split(AchDelim);
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
