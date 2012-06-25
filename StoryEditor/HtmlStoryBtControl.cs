using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public partial class HtmlStoryBtControl : HtmlVerseControl
    {
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
            _contextMenuTextarea = CreateContextMenuStrip();
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
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            theSe.AddNewVerse(nVerseIndex - 1, 1, false);
        }

        public void AddNewVerseAfter(int nVerseIndex)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            theSe.AddNewVerse(nVerseIndex - 1, 1, true);
        }

        public void HideVerse(int nVerseIndex)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            var verseData = GetVerseData(nVerseIndex);
            // toggle
            theSe.VisiblizeVerse(verseData, !(verseData.IsVisible));
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
                VerseData verseData = GetVerseData(nVerseIndex);
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

        public List<HtmlElement> GetSelectedTexts(int nLineNumber)
        {
            if (Document == null)
                return null;

            var doc = Document;
            
            // before we query for the spans, we have to trigger a 'blur'
            //  event (well, my 'fake' blur event) so the cell currently
            //  being edited will turn it's selection into a span also
            doc.InvokeScript("TriggerMyBlur");

            var strIdLn = VersesData.LineId(nLineNumber);
            HtmlElement elemParentLn = doc.GetElementById(strIdLn);
            if (elemParentLn == null)
                return null;

            // get the parent <table> below which we can query for the 'spans'
            var elemTableRoot = (elemParentLn.Parent != null) 
                                    ? elemParentLn.Parent.Parent
                                    : null;

            if (elemTableRoot == null)
                return null;

            var spans = elemTableRoot.GetElementsByTagName("span");
            var list = new List<HtmlElement>(spans.Count);
            list.AddRange(spans.Cast<object>().Cast<HtmlElement>());
            return list;
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
                return String.IsNullOrEmpty(LastTextareaInFocusId) ? null : GetStringTransfer(LastTextareaInFocusId);
            }
        }

        private static string _lastTextareaInFocusId;
        public static string LastTextareaInFocusId
        {
            get { return _lastTextareaInFocusId; }
            set
            {
                System.Diagnostics.Debug.WriteLine("setting LastTextareaInFocusId to " + value);
                _lastTextareaInFocusId = value;
            }
        }

        public void TriggerChangeUpdate()
        {
            // we only update the StringTransfer for a textarea when the user leaves (onchange), 
            //  so when the user saves, sometimes, we need to trigger that call.
            if (LastTextareaInFocusId == null)
                return;

            HtmlElement elem;
            if (!GetHtmlElementById(LastTextareaInFocusId, out elem))
                return;

            elem.InvokeMember("onchange");
        }

        public bool TextareaMouseUp(string strId)
        {
            LastTextareaInFocusId = strId;
            return true;
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
            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(strId, out textAreaIdentifier))
                return null;

            StringTransfer stringTransfer = GetStringTransfer(textAreaIdentifier);
            return stringTransfer;
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

        public bool GetHtmlElementById(string strId, out HtmlElement elem)
        {
            if (Document == null)
            {
                elem = null;
                return false;
            }

            var doc = Document;
            elem = doc.GetElementById(strId);
            return (elem != null);
        }

        public bool GetHtmlElementById(string strId, out HtmlDocument doc, out HtmlElement elem)
        {
            if (Document == null)
            {
                doc = null;
                elem = null;
                return false;
            }

            doc = Document;
            elem = doc.GetElementById(strId);
            return (elem != null);
        }

        public bool OnLineOptionsButton(string strId, bool bIsRightButton)
        {
            if (bIsRightButton)
            {
                TriggerChangeUpdate();
                contextMenuStripLineOptions.Show(MousePosition); 
                return false;
            }

            /*
            HtmlElement elem;
            if (!GetHtmlElementById(strId, out elem))
                return false;

            int nVerseIndex;
            var verseData = VerseDataFromLineOptionsButtonId(strId, out nVerseIndex);
            if (verseData == null)
                return false;
            var verseNetCtrl = TheSE.CreateVerseBtControl(null, verseData, nVerseIndex);
            var form = new VerseEditorForm(verseNetCtrl);
            form.ShowDialog();

            /*
            if (IsLineOptionsButton(strId))
                contextMenuStrip.Show(MousePosition);
            */
            return true;
        }

        private VerseData VerseDataFromLineOptionsButtonId(string strId, out int nVerseIndex)
        {
            var astr = strId.Split(AchDelim);
            if ((astr.Length == 2) && (astr[0] == VersesData.CstrButtonPrefixLineOptionsButton))
            {
                nVerseIndex = Convert.ToInt32(astr[1]);
                return GetVerseData(nVerseIndex);
            }
            nVerseIndex = -1;
            return null;
        }

        private StringTransfer GetStringTransfer(TextAreaIdentifier textAreaIdentifier)
        {
            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            var fieldType =
                (StoryEditor.TextFields) Enum.Parse(typeof (StoryEditor.TextFields), textAreaIdentifier.FieldType);
            LineData lineData;
            switch (fieldType)
            {
                case StoryEditor.TextFields.StoryLine:
                    lineData = verseData.StoryLine;
                    break;

                case StoryEditor.TextFields.ExegeticalNote:
                    return verseData.ExegeticalHelpNotes[textAreaIdentifier.ItemIndex];

                case StoryEditor.TextFields.Retelling:
                    lineData = verseData.Retellings[textAreaIdentifier.SubItemIndex];
                    break;

                case StoryEditor.TextFields.TestQuestion:
                    lineData = verseData.TestQuestions[textAreaIdentifier.ItemIndex].TestQuestionLine;
                    break;

                case StoryEditor.TextFields.TestQuestionAnswer:
                    lineData =
                        verseData.TestQuestions[textAreaIdentifier.ItemIndex].Answers[textAreaIdentifier.SubItemIndex];
                    break;

                default:
                    return null;
            }

            System.Diagnostics.Debug.Assert(lineData != null);
            var languageColumn =
                (StoryEditor.TextFields) Enum.Parse(typeof (StoryEditor.TextFields), textAreaIdentifier.LanguageColumn);
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

        protected static bool TryGetTextAreaId(string strId, out TextAreaIdentifier textAreaIdentifier)
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
                var aVerseConversationIndices = strId.Split(AchDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices[0] == CstrTextAreaPrefix) &&
                                                 (aVerseConversationIndices.Length == 6))
                                                ||
                                                ((aVerseConversationIndices[0] == CstrButtonPrefix) &&
                                                 (aVerseConversationIndices.Length == 3)));

                textAreaIdentifier = new TextAreaIdentifier
                                         {
                                             LineIndex = Convert.ToInt32(aVerseConversationIndices[1]),
                                             FieldType = aVerseConversationIndices[2],
                                             ItemIndex = Convert.ToInt32(aVerseConversationIndices[3]),
                                             SubItemIndex = Convert.ToInt32(aVerseConversationIndices[4]),
                                             LanguageColumn = aVerseConversationIndices[5]
                                         };
            }
            catch
            {
                textAreaIdentifier = null;
                return false;
            }
            return true;
        }

        public void AddScriptureReference(string strId)
        {
            int nLineIndex;
            if (!GetIndicesFromId(strId, out nLineIndex))
                return;

            HtmlElement elem;
            HtmlDocument doc;
            if (!GetHtmlElementById(strId, out doc, out elem))
                return;

            var verseData = GetVerseData(nLineIndex);
            var strJumpTarget = TheSE.GetNetBibleScriptureReference;
            if (verseData.Anchors.Contains(strJumpTarget))
                return;

            var anchorNew = verseData.Anchors.AddAnchorData(strJumpTarget,
                                                            strJumpTarget);
            
            List<string> astrDontCare = null;
            string str = anchorNew.PresentationHtml(null,
                                                    StoryData.PresentationType.Plain,
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

        private void MoveSelectedTextToANewLineToolStripMenuItemClick(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(LastTextareaInFocusId))
                return;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var nLineIndex = textAreaIdentifier.LineIndex;
            var verseData = GetVerseData(nLineIndex);
            verseData.AllowConNoteButtonsOverride();

            // make a copy and clear out the stuff that we'll have them manually move later
            var verseNew = new VerseData(verseData);
            verseNew.TestQuestions.Clear();
            verseNew.ConsultantNotes.Clear();
            verseNew.CoachNotes.Clear();

            var spans = GetSelectedTexts(nLineIndex);

            MoveSelectedText(spans, GetStoryLineId(nLineIndex, StoryEditor.TextFields.Vernacular.ToString()),
                             TheSE.StoryProject.ProjSettings.Vernacular.HasData,
                             verseData.StoryLine.Vernacular, verseNew.StoryLine.Vernacular);
            MoveSelectedText(spans, GetStoryLineId(nLineIndex, StoryEditor.TextFields.NationalBt.ToString()),
                             TheSE.StoryProject.ProjSettings.NationalBT.HasData,
                             verseData.StoryLine.NationalBt, verseNew.StoryLine.NationalBt);
            MoveSelectedText(spans, GetStoryLineId(nLineIndex, StoryEditor.TextFields.InternationalBt.ToString()),
                             TheSE.StoryProject.ProjSettings.InternationalBT.HasData,
                             verseData.StoryLine.InternationalBt, verseNew.StoryLine.InternationalBt);
            MoveSelectedText(spans, GetStoryLineId(nLineIndex, StoryEditor.TextFields.FreeTranslation.ToString()),
                             TheSE.StoryProject.ProjSettings.FreeTranslation.HasData,
                             verseData.StoryLine.FreeTranslation, verseNew.StoryLine.FreeTranslation);

            for (var i = 0; i < verseData.Retellings.Count; i++)
            {
                var lineDataFrom = verseData.Retellings[i];
                var lineDataTo = verseNew.Retellings[i];
                MoveSelectedText(spans, GetRetellingId(nLineIndex, i, StoryEditor.TextFields.Vernacular.ToString()),
                                 TheSE.StoryProject.ProjSettings.ShowRetellings.Vernacular,
                                 lineDataFrom.Vernacular, lineDataTo.Vernacular);
                MoveSelectedText(spans, GetRetellingId(nLineIndex, i, StoryEditor.TextFields.NationalBt.ToString()),
                                 TheSE.StoryProject.ProjSettings.ShowRetellings.NationalBt,
                                 lineDataFrom.NationalBt, lineDataTo.NationalBt);
                MoveSelectedText(spans, GetRetellingId(nLineIndex, i, StoryEditor.TextFields.InternationalBt.ToString()),
                                 TheSE.StoryProject.ProjSettings.ShowRetellings.InternationalBt,
                                 lineDataFrom.InternationalBt, lineDataTo.InternationalBt);
            }

            theSe.DoPasteVerse(nLineIndex, verseNew);

            var dlg = new CutItemPicker(verseData, verseNew, nLineIndex + 1, theSe);
            if (dlg.IsSomethingToMove)
                dlg.ShowDialog();

            theSe.InitAllPanes();
        }

        private void MoveSelectedText(IEnumerable<HtmlElement> spans, string strId, bool bFieldShowing,
            StringTransfer stFrom, StringTransfer stTo)
        {
            if (!bFieldShowing)
                return;

            var strSelectedText = GetSpanInnerText(spans, strId);
            string strOriginalText;
            if (!stFrom.TryGetSourceString(strSelectedText, out strOriginalText))
                return;

            stTo.SetValue(strOriginalText);
            stFrom.RemoveSubstring(strOriginalText);
        }

        private void MoveItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSt;
            if (!CheckForProperEditToken(out theSt) || (LastTextareaInFocusId == null))
                return;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            var dlg = new CutItemPicker(verseData, theSt.TheCurrentStory.Verses, theSt, false);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            theSt.Modified = true;
            theSt.InitAllPanes();
        }

        private void DeleteItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            var dlg = new CutItemPicker(verseData, theSe.TheCurrentStory.Verses, theSe, true)
            {
                Text = Localizer.Str("Choose the item(s) to delete and then click the Delete button")
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            theSe.Modified = true;
            theSe.InitAllPanes();
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

        private void DeleteTheWholeVerseToolStripMenuItemClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            if (verseData.HasData)
            {
                var res = QueryAboutHidingVerseInstead();

                if (res == DialogResult.Yes)
                {
                    theSe.VisiblizeVerse(verseData, false);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            if (UserConfirmDeletion)
            {
                theSe.DeleteVerse(verseData);
            }
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

        protected readonly char[] _achDelim = new[] { '_' };

        public void ShowContextMenu(string strId)
        {
            if (IsButtonElement(strId))
                ;
            else if (IsTextareaElement(strId))
            {
                LastTextareaInFocusId = strId;
                _contextMenuTextarea.Show(MousePosition);
            }
        }

        private ContextMenuStrip _contextMenuTextarea;
        private ContextMenuStrip CreateContextMenuStrip()
        {
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Items.Add(StoryEditor.CstrAddNoteOnSelected, null, OnAddNewNote);
            ctxMenu.Items.Add(StoryEditor.CstrAddNoteToSelfOnSelected, null, OnAddNoteToSelf);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToNational, null, OnGlossTextToNational);
            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToEnglish, null, OnGlossTextToEnglish);
            ctxMenu.Items.Add(StoryEditor.CstrConcordanceSearch, null, OnConcordanceSearch);
            /*
            ctxMenu.Items.Add(StoryEditor.CstrJumpToReference, null, onJumpToBibleRef);
            ctxMenu.Items.Add(StoryEditor.CstrConcordanceSearch, null, onConcordanceSearch);
            ctxMenu.Items.Add(StoryEditor.CstrAddLnCNote, null, OnAddLnCNote);
            ctxMenu.Items.Add(new ToolStripSeparator());
            if (StoryEditor.IsTestQuestionBox(strLabel))
            {
                ctxMenu.Items.Add(StoryEditor.CstrAddAnswerBox, null, onAddAnswerBox);
                ctxMenu.Items.Add(new ToolStripSeparator());
            }
            else if (StoryEditor.IsTqAnswerBox(strLabel))
            {
                ctxMenu.Items.Add(StoryEditor.CstrRemAnswerBox, null, onRemAnswerBox);
                ctxMenu.Items.Add(StoryEditor.CstrRemAnswerChangeUns, null, onChangeUns);
                ctxMenu.Items.Add(new ToolStripSeparator());
            }

            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToNational, null, onGlossTextToNational);
            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToEnglish, null, onGlossTextToEnglish);
            ctxMenu.Items.Add(StoryEditor.CstrReorderWords, null, onReorderWords);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(StoryEditor.CstrCutSelected, null, onCutSelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopySelected, null, onCopySelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopyOriginalSelected, null, onCopyOriginalText);
            ctxMenu.Items.Add(StoryEditor.CstrPasteSelected, null, onPasteSelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrUndo, null, onUndo);
            */
            ctxMenu.Opening += CtxMenuOpening;
            return ctxMenu;
        }

        void CtxMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var myStringTransfer = GetStringTransferOfLastTextAreaInFocus;
            var hasStringTransfer = (myStringTransfer != null);
            var nationalBtSiblingId = GetMyNationalBtSibling(LastTextareaInFocusId);
            var englishBtSibling = GetMyInternationalBtSibling(LastTextareaInFocusId);
            HtmlElement myElem;
            GetHtmlElementById(LastTextareaInFocusId, out myElem);

            // don't ask... I'm not sure why Items.ContainsKey isn't finding this...
            foreach (ToolStripItem x in _contextMenuTextarea.Items)
            {
                if (x.Text == StoryEditor.CstrCopyOriginalSelected)
                {
                    x.Enabled = (hasStringTransfer && (myStringTransfer.Transliterator != null));
                }
                else if (x.Text == StoryEditor.CstrReorderWords)
                {
                    x.Enabled = hasStringTransfer;
                }
                else if (x.Text == StoryEditor.CstrGlossTextToNational)
                {
                    x.Visible = ((nationalBtSiblingId != null) && (nationalBtSiblingId != LastTextareaInFocusId));
                }
                else if (x.Text == StoryEditor.CstrGlossTextToEnglish)
                {
                    x.Visible = ((englishBtSibling != null) && (englishBtSibling != LastTextareaInFocusId));
                }
                else if (x.Text == StoryEditor.CstrAddLnCNote)
                {
                    CheckForLnCNoteLookup((ToolStripMenuItem)x);
                }
                else if (x.Text == StoryEditor.CstrConcordanceSearch)
                {
                    x.Visible = (LastTextareaInFocusId.IndexOf(StoryEditor.TextFields.StoryLine.ToString(),
                                                               StringComparison.Ordinal) != -1);
                }
            }
        }

        private void OnConcordanceSearch(object sender, EventArgs e)
        {
            TheSE.concordanceToolStripMenuItem_Click(null, null);
        }

        private void CheckForLnCNoteLookup(ToolStripMenuItem x)
        {
            x.DropDownItems.Clear();
            var st = GetStringTransferOfLastTextAreaInFocus;
            var mapFoundString2LnCnote = TheSE.StoryProject.LnCNotes.FindHits(GetSelectedText(st), st.WhichField);
            foreach (var kvp in mapFoundString2LnCnote)
            {
                var tsi = x.DropDownItems.Add(kvp.Key, null, OnLookupLnCnote);
                tsi.Tag = kvp.Value;
                tsi.Font = Font;
            }
        }

        private void OnLookupLnCnote(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripItem;
            if (tsi == null)
                return;

            var note = (LnCNote)tsi.Tag;
            var dlg = new AddLnCNoteForm(TheSE, note) { Text = Localizer.Str("Edit L & C Note") };
            if ((dlg.ShowDialog() == DialogResult.OK) && (note != null))
                TheSE.Modified = true;
        }

        private void OnGlossTextToNational(object sender, EventArgs e)
        {
            OnDoGlossing(GetMyNationalBtSibling, TheSE.StoryProject.ProjSettings.NationalBT,
                         ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt);
        }

        private void OnGlossTextToEnglish(object sender, EventArgs e)
        {
            var st = GetStringTransferOfLastTextAreaInFocus;
            OnDoGlossing(GetMyInternationalBtSibling, TheSE.StoryProject.ProjSettings.InternationalBT,
                         ((st.WhichField & StoryEditor.TextFields.Vernacular) == StoryEditor.TextFields.Vernacular)
                             ? ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt
                             : ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt);
        }

        private delegate string GetSiblingId(string strId);

        private void OnDoGlossing(GetSiblingId mySiblingIdGetter, ProjectSettings.LanguageInfo liSibling,
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection adaptItBtDirection)
        {
            try
            {
                var siblingId = mySiblingIdGetter(LastTextareaInFocusId);
                if (siblingId == null)
                    return;

                HtmlElement siblingElement;
                if (!GetHtmlElementById(siblingId, out siblingElement))
                    return;

                var myStringTransfer = GetStringTransferOfLastTextAreaInFocus;
                if (!myStringTransfer.HasData)
                    return;

                var dlg = new GlossingForm(TheSE.StoryProject.ProjSettings,
                                           myStringTransfer.ToString(),
                                           adaptItBtDirection,
                                           TheSE.LoggedOnMember,
                                           TheSE.advancedUseWordBreaks.Checked,
                                           myStringTransfer.Transliterator);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    siblingElement.InnerText = dlg.TargetSentence;
                    siblingElement.InvokeMember("onchange"); // triggers the update to the data buffer

                    // but only update the source data if it wasn't being transliterated
                    if (myStringTransfer.Transliterator == null)
                    {
                        HtmlElement myElem;
                        if (GetHtmlElementById(LastTextareaInFocusId, out myElem))
                        {
                            myElem.InnerText = dlg.SourceSentence; // cause the user might have corrected some spelling
                            myElem.InvokeMember("onchange"); // triggers the update to the data buffer
                        }
                    }

                    TheSE.Modified = true;
                    if (dlg.DoReorder)
                    {
                        var siblingStringTransfer = GetStringTransfer(siblingId);
                        var dlgReorder = new ReorderWordsForm(siblingStringTransfer,
                                                              liSibling.FontToUse,
                                                              liSibling.FullStop);
                        if (dlgReorder.ShowDialog() == DialogResult.OK)
                        {
                            siblingElement.InnerText = dlgReorder.ReorderedText;
                            siblingElement.InvokeMember("onchange"); // triggers the update to the data buffer
                        }
                    }

                    siblingElement.Focus();
                }
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
        }

        private string GetMyVernacularSibling(string strId)
        {
            return GetMySibling(strId, StoryEditor.TextFields.Vernacular.ToString(),
                                TheSE.StoryProject.ProjSettings.Vernacular);
        }

        private string GetMyNationalBtSibling(string strId)
        {
            return GetMySibling(strId, StoryEditor.TextFields.NationalBt.ToString(),
                                TheSE.StoryProject.ProjSettings.NationalBT);
        }

        private string GetMyInternationalBtSibling(string strId)
        {
            return GetMySibling(strId, StoryEditor.TextFields.InternationalBt.ToString(),
                                TheSE.StoryProject.ProjSettings.InternationalBT);
        }

        private string GetMyFreeTranslationSibling(string strId)
        {
            return GetMySibling(strId, StoryEditor.TextFields.FreeTranslation.ToString(),
                                TheSE.StoryProject.ProjSettings.FreeTranslation);
        }

        private static string GetMySibling(string strId, string strSiblingName, ProjectSettings.LanguageInfo languageInfo)
        {
            if (String.IsNullOrEmpty(strId) || !languageInfo.HasData)
                return null;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(strId, out textAreaIdentifier))
                return null;

            // retask for the sibling language column
            textAreaIdentifier.LanguageColumn = strSiblingName;
            return textAreaIdentifier.HtmlIdentifier;
        }

        private static string GetStoryLineId(int nVerseIndex, string strFieldTypeName)
        {
            return StringTransfer.TextareaId(nVerseIndex,
                                             StoryEditor.TextFields.StoryLine.ToString(),
                                             0,
                                             0,
                                             strFieldTypeName);
        }

        private static string GetRetellingId(int nVerseIndex, int nSubItemIndex, string strFieldTypeName)
        {
            return StringTransfer.TextareaId(nVerseIndex,
                                             StoryEditor.TextFields.Retelling.ToString(),
                                             0,
                                             nSubItemIndex,
                                             strFieldTypeName);
        }

        private void OnAddNoteToSelf(object sender, EventArgs e)
        {
            AddNote(true);
        }

        private void OnAddNewNote(object sender, EventArgs e)
        {
            AddNote(false);
        }

        private void AddNote(bool bNoteToSelf)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(LastTextareaInFocusId));

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var nLastSubItemIndex = -1;
            string strLastFieldType = null,
                   strReferringText = null,
                   strNote = String.Format("{0}: ", StoryEditor.GetInitials(TheSE.LoggedOnMember.Name));

            foreach (var span in GetSelectedTexts(textAreaIdentifier.LineIndex))
            {
                var textarea = span.Parent;
                System.Diagnostics.Debug.Assert(textarea != null && textarea.TagName == "TEXTAREA");

                TextAreaIdentifier textAreaIdentifierParent;
                if (!TryGetTextAreaId(textarea.Id, out textAreaIdentifierParent))
                    return;

                // if this is a new type, then add it to the stream
                if (strLastFieldType != textAreaIdentifierParent.FieldType)
                {
                    if (!String.IsNullOrEmpty(strLastFieldType))
                        strReferringText += " vs: ";
                    strReferringText += textAreaIdentifierParent.FieldType + ":";
                    strLastFieldType = textAreaIdentifierParent.FieldType;
                }
                else if (textAreaIdentifierParent.SubItemIndex != nLastSubItemIndex)
                {
                    if (nLastSubItemIndex != -1)
                        strReferringText += " &";
                    nLastSubItemIndex = textAreaIdentifierParent.SubItemIndex;
                }
                strReferringText += " " + span.OuterHtml;
            }

            // remove the highlight class so it isn't highlighted in the connnote pane
            if (strReferringText != null)
                strReferringText = strReferringText.Replace(" highlight", null);

            TheSE.SendNoteToCorrectPane(textAreaIdentifier.LineIndex, strReferringText, strNote, bNoteToSelf);
        }

        internal void GetSelectedLanguageText(out string strVernacular, out string strNationalBt,
                                              out string strInternationalBt, out string strFreeTranslation)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(LastTextareaInFocusId));

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
            {
                strVernacular = strNationalBt = strInternationalBt = strFreeTranslation = null;
                return;
            }

            var spans = GetSelectedTexts(textAreaIdentifier.LineIndex);
            strVernacular = GetSpanInnerText(spans, GetMyVernacularSibling);
            strNationalBt = GetSpanInnerText(spans, GetMyNationalBtSibling);
            strInternationalBt = GetSpanInnerText(spans, GetMyInternationalBtSibling);
            strFreeTranslation = GetSpanInnerText(spans, GetMyFreeTranslationSibling);
        }

        private static string GetSpanInnerText(IEnumerable<HtmlElement> spans, GetSiblingId getterSiblingId)
        {
            return GetSpanInnerText(spans, getterSiblingId(LastTextareaInFocusId));
        }

        private static string GetSpanInnerText(IEnumerable<HtmlElement> spans, string strId)
        {
            return (from span in spans
                    where span.Parent.Id == strId
                    select span.InnerText).FirstOrDefault();
        }
    }
}
