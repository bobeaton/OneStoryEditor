using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using NetLoc;
using SilEncConverters40;
using SIL.Keyboarding;

namespace OneStoryProjectEditor
{
    // data-mouseup values on the Story/BT page's buttons and cells (StoryBt.js turns them into 'action' messages)
    internal static class StoryBtActions
    {
        public const string Anchor = "anchor";          // an anchor button
        public const string AnchorCell = "anchorCell";  // the empty part of the anchor row
        public const string LineOptions = "lineOptions";
        public const string AnchorMenu = "anchorMenu";  // the action name sent for a right-click on either of the first two
    }

    public partial class HtmlStoryBtControl : HtmlVerseControl
    {
        public static DirectableEncConverter TransliteratorVernacular;
        public static DirectableEncConverter TransliteratorNationalBt;
        public static DirectableEncConverter TransliteratorInternationalBt;
        public static DirectableEncConverter TransliteratorFreeTranslation;

        public const string CstrNullAnchor = "No Anchor";

        public static string CstrMenuLabelHide
        {
            get { return Localizer.Str("&Hide line"); }
        }

        public static string CstrMenuLabelUnhide
        {
            get { return Localizer.Str("&Unhide line"); }
        }

        public VerseData.ViewSettings ViewSettings { get; set; }
        public StoryData ParentStory { get; set; }

        public HtmlStoryBtControl()
            : this(null)
        {
        }

        internal HtmlStoryBtControl(IHtmlHost host)
            : base(host)
        {
            InitializeComponent();
            ResetContextMenu();

            Dispatcher.Register("textChanged", OnTextChanged);
            Dispatcher.Register("focus", msg => TextareaOnFocus(msg.GetString("id")));
            Dispatcher.Register("blur", msg => Program.ActivateDefaultKeyboard());
            Dispatcher.Register("textareaMouseUp", msg => LastTextareaInFocusId = msg.GetString("id"));
            Dispatcher.Register("contextMenu", msg => ShowContextMenu(msg.GetString("id")));
            Dispatcher.Register("mouseMove", msg => TheSE?.CheckBiblePaneCursorPosition());
            Dispatcher.Register("scriptureDropped", msg => AddScriptureReference(msg.GetString("id")));
            Dispatcher.Register("action", OnAction);
        }

        private void OnAction(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            switch (msg.GetString("name"))
            {
                case StoryBtActions.AnchorMenu:
                    OnAnchorButton(strId);
                    break;
                case StoryBtActions.LineOptions:
                    OnLineOptionsButton(strId, msg.GetString("arg") == "right");
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("HtmlStoryBtControl: unknown action " + msg);
                    break;
            }
        }

        public void ResetContextMenu()
        {
            _contextMenuTextarea = CreateContextMenuStrip();
        }

        protected override void OnRealign()
        {
            TheSE.RealignLines();
            LoadDocument();     // (the page used to call LoadDocument itself after TriggerCtrlF5)
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

            LoadHtml(strHtml);
        }

        /*
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
                var theNewVerse = new VerseData(_myClipboard);
                theNewVerse.AllowConNoteButtonsOverride();
                // make another copy, so that the guid is changed
                theSE.DoPasteVerse(nInsertionIndex, theNewVerse);
            }
        }
        */

        public override void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, true, true);
        }

        // the highlighted selections on this line (StoryBt.js first turns the current selection into one, as
        //  TriggerMyBlur always did)
        public List<HighlightedText> GetSelectedTexts(int nLineNumber)
        {
            var reply = Host.Request("getHighlights", new { tableId = VerseData.GetLineTableId(nLineNumber) },
                                     HtmlHostDefaults.RequestTimeout);
            return HighlightedText.FromReply(reply);
        }

        public new string GetSelectedText
        {
            get 
            {
                if (String.IsNullOrEmpty(LastTextareaInFocusId))
                    return null;

                TextAreaIdentifier textAreaIdentifier;
                if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                    return null;

                StoryEditor.TextFields whichLanguage;
                return GetSelectedTextByTextareaIdentifier(textAreaIdentifier, out whichLanguage);
            }
        }

        public string GetSelectedTextByTextareaIdentifier(TextAreaIdentifier textAreaIdentifier, out StoryEditor.TextFields whichLanguage)
        {
            GetSiblingId pSiblingId;
            var spans = GetSelectedTexts(textAreaIdentifier.LineIndex);
            whichLanguage = textAreaIdentifier.LanguageColumn;
            switch (whichLanguage)
            {
                case StoryEditor.TextFields.Vernacular:
                    pSiblingId = GetMyVernacularSibling;
                    break;
                case StoryEditor.TextFields.NationalBt:
                    pSiblingId = GetMyNationalBtSibling;
                    break;
                case StoryEditor.TextFields.InternationalBt:
                    pSiblingId = GetMyInternationalBtSibling;
                    break;
                case StoryEditor.TextFields.FreeTranslation:
                    pSiblingId = GetMyFreeTranslationSibling;
                    break;
                default:
                    System.Diagnostics.Debug.Fail("wasn't expecting this case");
                    return null;
            }
            return GetSpanInnerText(spans, pSiblingId);
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

        public void OnMouseMove()
        {
            TheSE.CheckBiblePaneCursorPosition();
        }

        // textChanged: 'value' is a textarea's plain value (keyup, paste, set by C#); 'ieHtml' is IE's htmlText form
        //  (the onchange path in StoryBtPs.js); 'quiet' means it came from a flush (no error box for read-only boxes)
        private void OnTextChanged(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            if (strId == null)
                return;

            var bQuiet = msg.GetBool("quiet");
            var strIeHtml = msg.GetString("ieHtml");
            if ((strIeHtml == null) && !bQuiet)
            {
                LastTextareaInFocusId = strId;
                TheSE.LastKeyPressedTimeStamp = DateTime.Now;
            }

            var strText = (strIeHtml != null)
                              ? HtmlText.FromIeHtmlText(strIeHtml)
                              : (msg.GetString("value") ?? String.Empty);
            SetFieldValue(strId, strText, bQuiet);
        }

        private bool SetFieldValue(string strId, string strText, bool bQuiet)
        {
            var stringTransfer = GetStringTransfer(strId);
            if (stringTransfer == null)
                return false;

            // nothing changed (e.g. an arrow key, or a flush): don't mark the project modified
            if (PaneText.IsSame(stringTransfer, strText))
                return true;

            if (!CheckForProperEditToken(out var theSe))
                return false;

            if (bQuiet && stringTransfer.IsFieldReadonly(ViewSettings.FieldEditibility))
                return false;

            if (!CheckShowErrorOnFieldNotEditable(stringTransfer))
                return false;

            stringTransfer.SetValue(strText);

            // indicate that the document has changed
            theSe.Modified = true;

            // update the status bar (in case we previously put an error there
            var st = StoryStageLogic.stateTransitions[theSe.TheCurrentStory.ProjStage.ProjectStage];
            theSe.SetDefaultStatusBar(st.StageDisplayString);

            return true;
        }

        public bool CheckShowErrorOnFieldNotEditable(StringTransfer stringTransfer)
        {
            // this will fail if the field is readonly which would be if the consultant hadn't allowed it or if
            //  a transliterator were turned on. Either way, this should catch it.
            if (stringTransfer.IsFieldReadonly(ViewSettings.FieldEditibility))
            {
                LocalizableMessageBox.Show(
                    String.Format(
                        Localizer.Str(
                            "You can't edit this field right now... either the consultant hasn't given you permission to edit the '{0}' language fields or perhaps there is a transliterator turned on"),
                        stringTransfer.WhichField & StoryEditor.TextFields.Languages),
                    StoryEditor.OseCaption);
                return false;
            }
            return true;
        }

        private bool CheckForTaskPermission(ProjectSettings.LanguageInfo li, StoryEditor.TextFields typeField,
            StoryEditor.TextFields eType, bool isTaskOn)
        {
            if (typeField == eType)
                return (!li.HasData || isTaskOn);
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

        private bool TextareaOnFocus(string strId)
        {
            LastTextareaInFocusId = strId;
            TextAreaIdentifier textAreaIdentifier;
            if (TryGetTextAreaId(strId, out textAreaIdentifier))
            {
                var strKeyboardName = textAreaIdentifier.GetLanguageInfo(TheSE.StoryProject.ProjSettings).Keyboard;
                if (!String.IsNullOrEmpty(strKeyboardName))
                    // Keyboard.Controller.SetKeyboard(strKeyboardName);
                    Keyboard.Controller.GetKeyboard(strKeyboardName).Activate();
            }
            return false;
        }

        private string _lastLineOptionsButtonClicked;

        private bool OnLineOptionsButton(string strId, bool bIsRightButton)
        {
            if (bIsRightButton)
            {
                _lastLineOptionsButtonClicked = strId;
                contextMenuStripLineOptions.Show(MousePosition); 
                return false;
            }

            return true;
        }

        private VerseData VerseDataFromLineOptionsButtonId(string strId, out int nLineIndex)
        {
            var astr = strId.Split(AchDelim);
            if ((astr.Length == 2) && (astr[0] == VersesData.CstrButtonPrefixLineOptionsButton))
            {
                nLineIndex = Convert.ToInt32(astr[1]);
                return GetVerseData(nLineIndex);
            }
            nLineIndex = -1;
            return null;
        }

        private string _lastAnchorButtonClicked;
        private bool OnAnchorButton(string strButtonId)
        {
            _lastAnchorButtonClicked = strButtonId;
            contextMenuStripAnchorOptions.Show(MousePosition);
            return true;
        }

        private VerseData VerseDataFromAnchorButtonId(string strId, out int nLineIndex)
        {
            // if there is no button, this comes in as anc_3 (for line 3 anchor bar), but otherwise, it might be ???
            var astr = strId.Split(AchDelim);
            if ((astr[0] == AnchorData.CstrButtonPrefixAnchorButton) || (astr[0] == AnchorData.CstrButtonPrefixAnchorBar))
            {
                nLineIndex = Convert.ToInt32(astr[1]);
                return GetVerseData(nLineIndex);
            }

            nLineIndex = -1;
            return null;
        }

        private StringTransfer GetStringTransfer(TextAreaIdentifier textAreaIdentifier)
        {
            // this might fail if the screen doesn't match what's in the internal buffer (e.g. a
            //  ExegeticalHelpNote doesn't exist in verseData.ExegeticalHelpNotes so [] throws an exception)
            // Not really sure how this can happen, but it does
            try
            {
                return GetStringTransferEx(textAreaIdentifier);
            }
            catch (Exception)
            {
                return null;
            }
        }
    
        private StringTransfer GetStringTransferEx(TextAreaIdentifier textAreaIdentifier)
        {
            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            if (verseData == null)
                return null;

            LineData lineData;
            StringTransfer stField = null;
            switch (textAreaIdentifier.FieldType)
            {
                case StoryEditor.TextFields.StoryLine:
                    lineData = verseData.StoryLine;
                    break;

                case StoryEditor.TextFields.ExegeticalNote:
                    stField = verseData.ExegeticalHelpNotes[textAreaIdentifier.ItemIndex];
                    
                    // since we're returning right away, we have to do this now.
                    stField.HtmlElementId = textAreaIdentifier.HtmlIdentifier;
                    return stField;

                case StoryEditor.TextFields.Retelling:
                    lineData = verseData.Retellings[textAreaIdentifier.SubItemIndex];
                    break;

                case StoryEditor.TextFields.TestQuestion:
                    lineData = verseData.TestQuestions[textAreaIdentifier.ItemIndex].TestQuestionLine;
                    break;

                case StoryEditor.TextFields.TestQuestionAnswer:
                    // the sub-index seems to reflect the test number; not necessarily the offset into "Answers".
                    lineData = TheSE.GetTqAnswerData(
                        verseData.TestQuestions[textAreaIdentifier.ItemIndex].Answers,
                        (textAreaIdentifier.SubItemIndex + 1).ToString());
                    break;

                default:
                    return null;
            }

            System.Diagnostics.Debug.Assert(lineData != null);
            var languageColumn =
                (StoryEditor.TextFields) Enum.Parse(typeof (StoryEditor.TextFields), textAreaIdentifier.LanguageColumnName);
            switch (languageColumn)
            {
                case StoryEditor.TextFields.Vernacular:
                    stField = lineData.Vernacular;
                    break;
                case StoryEditor.TextFields.NationalBt:
                    stField = lineData.NationalBt;
                    break;
                case StoryEditor.TextFields.InternationalBt:
                    stField = lineData.InternationalBt;
                    break;
                case StoryEditor.TextFields.FreeTranslation:
                    stField = lineData.FreeTranslation;
                    break;
            }

            if (stField != null)
                stField.HtmlElementId = textAreaIdentifier.HtmlIdentifier;
            return stField;
        }

        internal static bool TryGetTextAreaId(string strId, out TextAreaIdentifier textAreaIdentifier)
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
                                             FieldTypeName = aVerseConversationIndices[2],
                                             ItemIndex = Convert.ToInt32(aVerseConversationIndices[3]),
                                             SubItemIndex = Convert.ToInt32(aVerseConversationIndices[4]),
                                             LanguageColumnName = aVerseConversationIndices[5]
                                         };
            }
            catch
            {
                textAreaIdentifier = null;
                return false;
            }
            return true;
        }

        private void AddScriptureReference(string strId)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            int nLineIndex;
            if (!GetIndicesFromId(strId, out nLineIndex))
                return;

            var verseData = GetVerseData(nLineIndex);
            if (verseData == null)
                return;

            var strJumpTarget = TheSE.GetNetBibleScriptureReference;
            if (verseData.Anchors.Contains(strJumpTarget))
                return;

            var anchorNew = verseData.Anchors.AddAnchorData(strJumpTarget,
                                                            strJumpTarget);

            List<string> astrDontCare = null;
            var strButtonHtml = anchorNew.PresentationHtml(nLineIndex, null,
                                                           StoryData.PresentationType.Plain,
                                                           false,
                                                           ref astrDontCare);

            // the button's html already has its (localized) label in it
            Host.Post("appendHtml", new { id = strId, html = strButtonHtml });
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
                System.Diagnostics.Debug.Assert((aVerseConversationIndices[0] == AnchorData.CstrButtonPrefixAnchorBar) &&
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

        private void MoveSelectedTextToANewLineToolStripMenuItemClick(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);
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

            ReloadAllWindows();
        }

        private void MoveSelectedText(IEnumerable<HighlightedText> spans, string strId, bool bFieldShowing,
            StringTransfer stFrom, StringTransfer stTo)
        {
            if (!bFieldShowing)
                return;

            var strSelectedText = GetSpanInnerText(spans, strId);
            string strOriginalText;
            if (String.IsNullOrEmpty(strSelectedText) ||
                !stFrom.TryGetSourceString(strSelectedText, out strOriginalText))
                return;

            stTo.SetValue(strOriginalText);
            stFrom.RemoveSubstring(strOriginalText);
        }

        private void MoveItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);
            var dlg = new CutItemPicker(verseData, theSe.TheCurrentStory.Verses, theSe, false);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            ReloadAllWindows();
        }

        private void ReloadAllWindows()
        {
            TheSE.Modified = true;
            var lastTop = GetTopRowId;
            TheSE.InitAllPanes();
            StrIdToScrollTo = lastTop;  // because InitAllPanes will have clobbered it
        }

        private void DeleteItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            var dlg = new CutItemPicker(verseData, theSe.TheCurrentStory.Verses, theSe, true)
            {
                Text = Localizer.Str("Choose the item(s) to delete and then click the Delete button")
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            ReloadAllWindows();
        }

#if UseOlderMsgBox
        public static DialogResult QueryAboutHidingVerseInstead()
        {
            return LocalizableMessageBox.Show(
                String.Format(Localizer.Str("This line isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around to know what it used to be.{0}{0}Click 'Yes' to hide the line or click 'No' to delete it?"),
                              Environment.NewLine),
                StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel);
        }
#endif

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

        private void MenuAddTestQuestionClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            var isGeneralQuestionsLine = (nLineIndex == 0);
            if (isGeneralQuestionsLine &&
                TeamMemberData.IsUser(theSe.LoggedOnMember.MemberType, TeamMemberData.UserTypes.ProjectFacilitator) && 
                !TasksPf.IsTaskOn(theSe.TheCurrentStory.TasksAllowedPf, TasksPf.TaskSettings.TestQuestions))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str("The consultant has not allowed you to enter testing questions at this time"),
                    StoryEditor.OseCaption);
                return;
            }

            verseData.TestQuestions.AddTestQuestion();
            theSe.Modified = true;
            if (isGeneralQuestionsLine && !theSe.viewGeneralTestingsQuestionMenu.Checked)
                theSe.viewGeneralTestingsQuestionMenu.Checked = true;
            if (!isGeneralQuestionsLine && !theSe.viewStoryTestingQuestionsMenu.Checked)
                theSe.viewStoryTestingQuestionsMenu.Checked = true;
            else
            {
                StrIdToScrollTo = GetTopRowId;
                LoadDocument();
            }
        }

        private void addExegeticalCulturalNoteBelowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            verseData.ExegeticalHelpNotes.AddExegeticalHelpNote("");
            theSe.Modified = true;

            if (!theSe.viewExegeticalHelps.Checked)
                theSe.viewExegeticalHelps.Checked = true;
            else
            {
                StrIdToScrollTo = GetTopRowId;
                LoadDocument();
            }
        }

        private void addNewVersesBeforeMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            var tsmi = (ToolStripMenuItem)sender;
            int nNumNewVerses = Convert.ToInt32(tsmi.Text);

            theSe.AddNewVerse(nLineIndex - 1, nNumNewVerses, false);
        }

        private void addANewVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            theSe.AddNewVerse(nLineIndex - 1, 1, false);
        }

        private void addNewVersesAfterMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            var tsmi = (ToolStripMenuItem)sender;
            var nNumNewVerses = Convert.ToInt32(tsmi.Text);

            theSe.AddNewVerse(nLineIndex - 1, nNumNewVerses, true);
        }

        private void hideVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            theSe.VisiblizeVerse(verseData,
                !(verseData.IsVisible)   // toggle
                );
        }

        private void DeleteTheWholeVerseToolStripMenuItemClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            if (verseData.HasData)
            {
#if !UseOlderMsgBox
                var res = new CustomMsgBox(Localizer.Str("Delete or Hide?"), "This line isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around for history. Click 'Delete' to delete the line or click 'Hide' to hide it?", "Delete", "Hide")
                               .ShowDialog();

                if (res == DialogResult.Retry)
#else
                var res = QueryAboutHidingVerseInstead();

                if (res == DialogResult.Yes)
#endif
                {
                    theSe.VisiblizeVerse(verseData, false);
                    return;
                }

                if (res == DialogResult.Cancel)
                    return;
            }

            if (UserConfirmDeletion)
            {
                StrIdToScrollTo = GetTopRowId;
                theSe.DeleteVerse(verseData);
            }
        }

        protected static VerseData _myClipboard = null;
        protected VerseData PasteVerseToIndex(StoryEditor theSe, int nInsertionIndex)
        {
            if (_myClipboard != null)
            {
                var theNewVerse = new VerseData(_myClipboard);
                theNewVerse.AllowConNoteButtonsOverride();
                // make another copy, so that the guid is changed
                theSe.DoPasteVerse(nInsertionIndex, theNewVerse);
                return theNewVerse;
            }
            return null;
        }

        private void pasteVerseFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            PasteVerseToIndex(theSe, Math.Max(0, nLineIndex - 1));
            theSe.InitAllPanes();
        }

        private void pasteVerseFromClipboardAfterThisOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            PasteVerseToIndex(theSe, nLineIndex);
            theSe.InitAllPanes();
        }

        private void copyVerseToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            _myClipboard = new VerseData(verseData);
        }

        private void splitStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            theSe.SplitStory(verseData);
        }

        private void joinStoryToolStripMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            theSe.JoinStory(verseData);
            theSe.InitAllPanes();
            LocalizableMessageBox.Show(
                String.Format(
                    "The following story has been copied and inserted into this story starting at line number: '{0}'. If you no longer need the following story, you can delete it by moving to that story and then choosing 'Story', 'Delete story' from the menu",
                    ++nLineIndex), StoryEditor.OseCaption);
        }

        private void MoveLineUp(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            StrIdToScrollTo = GetPrevRowId; // get the *previous* row for going up
            theSe.DoMove(nLineIndex - 2, verseData);
        }

        private void MoveLineDown(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);

            StrIdToScrollTo = GetNextRowId; // get the *next* row for going down
            theSe.DoMove(nLineIndex + 1, verseData);
        }

        protected readonly char[] _achDelim = new[] { '_' };

        private void ShowContextMenu(string strId)
        {
            if (StoryEditor.TextPaster != null)
                return;

            if (IsButtonElement(strId))
            {
                ;
            }
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
            ctxMenu.Items.Add(StoryEditor.CstrReorderWords, null, onReorderWords);
            ctxMenu.Items.Add(StoryEditor.CstrConcordanceSearch, null, OnConcordanceSearch);
            ctxMenu.Items.Add(StoryEditor.CstrAddLnCNote, null, OnAddLnCNote);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(StoryEditor.CstrAddAnswerBox, null, onAddAnswerBox);
            ctxMenu.Items.Add(StoryEditor.CstrRemAnswerBox, null, onRemAnswerBox);
            ctxMenu.Items.Add(StoryEditor.CstrRemAnswerChangeUns, null, onChangeUns);

            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(StoryEditor.CstrCutSelected, null, onCutSelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopySelected, null, onCopySelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopyOriginalSelected, null, onCopyOriginalText);
            ctxMenu.Items.Add(StoryEditor.CstrPasteSelected, null, onPasteSelectedText);
            // ctxMenu.Items.Add(StoryEditor.CstrUndo, null, onUndo);

            ctxMenu.Opening += CtxMenuOpening;
            return ctxMenu;
        }

        private void onPasteSelectedText(object sender, EventArgs e)
        {
            TheSE.pasteToolStripMenuItem_Click(null, null);
        }

        private void onCopyOriginalText(object sender, EventArgs e)
        {
            TextAreaIdentifier textAreaIdentifier;
            if (String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var st = GetStringTransfer(textAreaIdentifier);
            if (st == null)
                return;

            Clipboard.SetText(st.ToString(), TextDataFormat.UnicodeText);
        }

        private void onCopySelectedText(object sender, EventArgs e)
        {
            TheSE.editCopySelectionToolStripMenuItem_Click(null, null);
        }

        private void onCutSelectedText(object sender, EventArgs e)
        {
            StoryEditor theSe;
            TextAreaIdentifier textAreaIdentifier;
            if (!CheckForProperEditToken(out theSe) ||
                String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var st = GetStringTransfer(textAreaIdentifier);
            if ((st == null) || !CheckShowErrorOnFieldNotEditable(st) || !Host.IsReady)
                return;

            int nNewEndPoint;
            var selectedText = GetSelectedText;
            Clipboard.SetDataObject(selectedText);
            SetSelectedText(st, String.Empty, out nNewEndPoint);
            theSe.Modified = true;
        }

        private void onReorderWords(object sender, EventArgs e)
        {
            StoryEditor theSe;
            TextAreaIdentifier textAreaIdentifier;
            if (!CheckForProperEditToken(out theSe) ||
                String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var st = GetStringTransfer(textAreaIdentifier);
            if (st == null)
                return;

            var li = textAreaIdentifier.GetLanguageInfo(TheSE.StoryProject.ProjSettings);
            var dlg = new ReorderWordsForm(st, li.FontToUse, li.FullStop);
            if (dlg.ShowDialog() == DialogResult.OK)
                st.SetValue(dlg.ReorderedText);

            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
        }

        private void onChangeUns(object sender, EventArgs e)
        {
            StoryEditor theSe;
            TextAreaIdentifier textAreaIdentifier;
            if (!CheckForProperEditToken(out theSe) ||
                String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            if (verseData == null)
                return;

            System.Diagnostics.Debug.Assert(textAreaIdentifier.ItemIndex < verseData.TestQuestions.Count);
            var testQuestionData = verseData.TestQuestions[textAreaIdentifier.ItemIndex];
            var answers = testQuestionData.Answers;

            var answerToChange = theSe.GetTqAnswerData(answers, (textAreaIdentifier.SubItemIndex + 1).ToString());

            theSe.ChangeAnswerBoxUns(testQuestionData, answers, answerToChange);

            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
        }

        private void onAddAnswerBox(object sender, EventArgs e)
        {
            StoryEditor theSe;
            TextAreaIdentifier textAreaIdentifier;
            if (!CheckForProperEditToken(out theSe) || 
                String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            if (verseData == null)
                return;

            var testQuestionData = verseData.TestQuestions[textAreaIdentifier.ItemIndex];
            LineMemberData theNewAnswer;
            if (!theSe.AddSingleTestResult(testQuestionData, out theNewAnswer))
                return;

            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
        }

        private void onRemAnswerBox(object sender, EventArgs e)
        {
            StoryEditor theSe;
            TextAreaIdentifier textAreaIdentifier;
            if (!CheckForProperEditToken(out theSe) ||
                String.IsNullOrEmpty(LastTextareaInFocusId) ||
                !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var verseData = GetVerseData(textAreaIdentifier.LineIndex);
            if (verseData == null)
                return;

            System.Diagnostics.Debug.Assert(textAreaIdentifier.ItemIndex < verseData.TestQuestions.Count);
            var testQuestionData = verseData.TestQuestions[textAreaIdentifier.ItemIndex];
            var answers = testQuestionData.Answers;

            var answerToRemove = theSe.GetTqAnswerData(answers, (textAreaIdentifier.SubItemIndex + 1).ToString());
            answers.Remove(answerToRemove);

            theSe.Modified = true;
            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
        }

        private void OnAddLnCNote(object sender, EventArgs e)
        {
            TheSE.AddLnCNote();
        }

        void CtxMenuOpening(object sender, CancelEventArgs e)
        {
            var myStringTransfer = GetStringTransferOfLastTextAreaInFocus;
            var hasStringTransfer = (myStringTransfer != null);

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
                    var nationalBtSiblingId = GetMyNationalBtSibling(LastTextareaInFocusId);
                    x.Visible = ((nationalBtSiblingId != null) && (nationalBtSiblingId != LastTextareaInFocusId));
                }
                else if (x.Text == StoryEditor.CstrGlossTextToEnglish)
                {
                    var englishBtSibling = GetMyInternationalBtSibling(LastTextareaInFocusId);
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
                else if (x.Text == StoryEditor.CstrAddAnswerBox)
                {
                    x.Visible = ShouldTqPopupsBeVisible;
                }
                else if ((x.Text == StoryEditor.CstrRemAnswerBox) || (x.Text == StoryEditor.CstrRemAnswerChangeUns))
                {
                    x.Visible = ShouldAnsPopupsBeVisible;
                }
                else if ((x.Text == StoryEditor.CstrCutSelected) ||
                         (x.Text == StoryEditor.CstrCopySelected) ||
                         (x.Text == StoryEditor.CstrCopyOriginalSelected))
                {
                    x.Enabled = !String.IsNullOrEmpty(GetSelectedText);
                }
                else if (x.Text == StoryEditor.CstrPasteSelected)
                {
                    x.Enabled = (hasStringTransfer && !myStringTransfer.IsFieldReadonly(ViewSettings.FieldEditibility));
                }
            }
        }

        protected bool ShouldTqPopupsBeVisible
        {
            get
            {
                TextAreaIdentifier textAreaIdentifier;
                if (String.IsNullOrEmpty(LastTextareaInFocusId) || !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                    return false;

                return (textAreaIdentifier.FieldType == StoryEditor.TextFields.TestQuestion);
            }
        }

        protected bool ShouldAnsPopupsBeVisible
        {
            get
            {
                TextAreaIdentifier textAreaIdentifier;
                if (String.IsNullOrEmpty(LastTextareaInFocusId) || !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                    return false;

                return (textAreaIdentifier.FieldType == StoryEditor.TextFields.TestQuestionAnswer);
            }
        }

        private void OnConcordanceSearch(object sender, EventArgs e)
        {
            TheSE.concordanceToolStripMenuItem_Click(null, null);
        }

        private void CheckForLnCNoteLookup(ToolStripMenuItem x)
        {
            x.DropDownItems.Clear();

            if (String.IsNullOrEmpty(LastTextareaInFocusId))
                return;

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            StoryEditor.TextFields whichLanguage;
            var selText = GetSelectedTextByTextareaIdentifier(textAreaIdentifier, out whichLanguage);
            if (String.IsNullOrEmpty(selText))
                return;

            var mapFoundString2LnCnote = TheSE.StoryProject.LnCNotes.FindHits(selText, whichLanguage);
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
            if (st == null)
                return;

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

                var hasSibling = Host.Request("hasElement", new { id = siblingId }, HtmlHostDefaults.RequestTimeout);
                if ((hasSibling == null) || !hasSibling.GetBool("found"))
                    return;

                var myStringTransfer = GetStringTransferOfLastTextAreaInFocus;
                if ((myStringTransfer == null) || !myStringTransfer.HasData)
                    return;

                var dlg = new GlossingForm(TheSE.StoryProject.ProjSettings,
                                           myStringTransfer.ToString(),
                                           adaptItBtDirection,
                                           TheSE.LoggedOnMember,
                                           TheSE.advancedUseWordBreaks.Checked,
                                           myStringTransfer.Transliterator);

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                StoryEditor theSe;
                if (!CheckForProperEditToken(out theSe))
                    return;

                var siblingStringTransfer = GetStringTransfer(siblingId);
                if (siblingStringTransfer == null)
                    return;

                if (siblingStringTransfer.ToString() != dlg.TargetSentence)
                    siblingStringTransfer.SetValue(dlg.TargetSentence);

                // check whether we need to update the source as well (user may have changed it)...
                //  but only update the source data if it wasn't being transliterated
                if (myStringTransfer.Transliterator == null)
                {
                    if (myStringTransfer.ToString() != dlg.SourceSentence)
                        myStringTransfer.SetValue(dlg.SourceSentence);
                }

                TheSE.Modified = true;
                if (dlg.DoReorder)
                {
                    var dlgReorder = new ReorderWordsForm(siblingStringTransfer,
                                                          liSibling.FontToUse,
                                                          liSibling.FullStop);
                    if (dlgReorder.ShowDialog() == DialogResult.OK)
                        siblingStringTransfer.SetValue(dlgReorder.ReorderedText);
                }

                StrIdToScrollTo = GetTopRowId;
                LoadDocument();
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
            textAreaIdentifier.LanguageColumnName = strSiblingName;
            return textAreaIdentifier.HtmlIdentifier;
        }

        private static string GetStoryLineId(int nVerseIndex, string strFieldTypeName)
        {
            StoryEditor.LocalizedEnum<StoryEditor.TextFields> field = StoryEditor.TextFields.StoryLine;
            return StringTransfer.TextareaId(nVerseIndex,
                                             field.ToString(),
                                             0,
                                             0,
                                             strFieldTypeName);
        }

        private static string GetRetellingId(int nVerseIndex, int nSubItemIndex, string strFieldTypeName)
        {
            StoryEditor.LocalizedEnum<StoryEditor.TextFields> field = StoryEditor.TextFields.Retelling;
            return StringTransfer.TextareaId(nVerseIndex,
                                             field.ToString(),
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

            var spans = GetSelectedTexts(textAreaIdentifier.LineIndex);
            if (!ReferringTextBuilder.TryBuild(spans, out var strReferringText))
                return;

            // if the user doesn't cancel, then clear out the spans/selected text (save a step for the next note)
            if (TheSE.SendNoteToCorrectPane(textAreaIdentifier.LineIndex, strReferringText, bNoteToSelf))
                ClearSelectionSpans(spans);
        }

        private void ClearSelectionSpans(IEnumerable<HighlightedText> spans)
        {
            foreach (var strTextareaId in spans.Select(s => s.TextareaId).Distinct())
                Host.Post("clearHighlight", new { id = strTextareaId });
        }

        internal void GetSelectedLanguageText(out string strVernacular, out string strNationalBt,
                                              out string strInternationalBt, out string strFreeTranslation)
        {
            TextAreaIdentifier textAreaIdentifier;
            if (String.IsNullOrEmpty(LastTextareaInFocusId) || !TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
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

        private static string GetSpanInnerText(IEnumerable<HighlightedText> spans, GetSiblingId getterSiblingId)
        {
            return GetSpanInnerText(spans, getterSiblingId(LastTextareaInFocusId));
        }

        private static string GetSpanInnerText(IEnumerable<HighlightedText> spans, string strId)
        {
            return spans.Where(s => (s.TextareaId == strId) && !String.IsNullOrEmpty(s.Text))
                        .Select(s => s.Text)
                        .FirstOrDefault();
        }

        private bool TryGetAnchorData(string strAnchorButtonId, out int nLineIndex, out AnchorData anchor)
        {
            anchor = null;
            var verseData = VerseDataFromAnchorButtonId(strAnchorButtonId, out nLineIndex);
            return ((verseData != null) && TryGetAnchorData(verseData, nLineIndex, out anchor));
        }

        private bool TryGetAnchorData(VerseData verseData, int nLineIndex, out AnchorData anchor)
        {
            anchor = verseData.Anchors
                .FirstOrDefault(anc => AnchorData.ButtonId(nLineIndex, anc.JumpTarget) == _lastAnchorButtonClicked);
            return (anchor != null);
        }

        private void DeleteAnchorToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_lastAnchorButtonClicked))
            {
                StoryEditor theSe;
                if (!CheckForProperEditToken(out theSe))
                    return;

                int nLineIndex;
                var verseData = VerseDataFromAnchorButtonId(_lastAnchorButtonClicked, out nLineIndex);
                if (verseData == null)
                    return;

                AnchorData anchor;
                if (!TryGetAnchorData(verseData, nLineIndex, out anchor))
                    return;

                verseData.Anchors.Remove(anchor);

                StrIdToScrollTo = GetTopRowId;
                LoadDocument();

                _lastAnchorButtonClicked = null;

                // indicate that we've changed something so that we don't exit without offering
                //  to save.
                theSe.Modified = true;
            }
            else
                LocalizableMessageBox.Show("Right-click on one of the buttons to choose which one to delete", StoryEditor.OseCaption);
        }

        private void AddCommentToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_lastAnchorButtonClicked))
            {
                StoryEditor theSe;
                if (!CheckForProperEditToken(out theSe))
                    return;

                int nLineIndex;
                AnchorData anchor;
                if (!TryGetAnchorData(_lastAnchorButtonClicked, out nLineIndex, out anchor))
                    return;

                var dlg = new AnchorAddCommentForm(NetBibleViewer.CheckForLocalization(anchor.JumpTarget), anchor.ToolTipText);
                var res = dlg.ShowDialog();
                if ((res == DialogResult.OK) || (res == DialogResult.Yes))
                {
                    anchor.ToolTipText = dlg.CommentText;
                    theSe.Modified = true;
                    StrIdToScrollTo = GetTopRowId;
                    LoadDocument();
                }
            }
            else
                LocalizableMessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to", StoryEditor.OseCaption);
        }

        private void AddConsultantCoachNoteOnThisAnchorToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_lastAnchorButtonClicked))
            {
                StoryEditor theSe;
                if (!CheckForProperEditToken(out theSe))
                    return;
                System.Diagnostics.Debug.Assert(theSe.LoggedOnMember != null);

                int nLineIndex;
                AnchorData anchor;
                if (!TryGetAnchorData(_lastAnchorButtonClicked, out nLineIndex, out anchor))
                    return;

                StrIdToScrollTo = GetTopRowId;

                var strReferringText = AnchorsData.AnchorLabel + " ";
                strReferringText += anchor.JumpTarget;

                if (anchor.JumpTarget != anchor.ToolTipText)
                    strReferringText += String.Format(" ({0})", anchor.ToolTipText);

                theSe.SendNoteToCorrectPane(nLineIndex, strReferringText, false);
            }
            else
                LocalizableMessageBox.Show("Right-click on one of the buttons to choose which one to add the comment to", StoryEditor.OseCaption);
        }

        private void InsertNullAnchorToolStripMenuItemClick(object sender, EventArgs e)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            int nLineIndex;
            var verseData = VerseDataFromAnchorButtonId(_lastAnchorButtonClicked, out nLineIndex);
            if (verseData == null)
                return;

            verseData.Anchors.AddAnchorData(CstrNullAnchor, CstrNullAnchor);

            // indicate that we've changed something so that we don't exit without offering
            //  to save.
            theSe.Modified = true;
            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
        }

        private void ContextMenuStripAnchorOptionsOpening(object sender, CancelEventArgs e)
        {
            int nLineIndex;
            var verseData = VerseDataFromAnchorButtonId(_lastAnchorButtonClicked, out nLineIndex);
            if (verseData == null)
                return;

            insertNullAnchorToolStripMenuItem.Visible = (verseData.Anchors.Count == 0);

            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || (theSe.LoggedOnMember == null))
                return;

            addConsultantCoachNoteOnThisAnchorToolStripMenuItem.Visible =
                TeamMemberData.IsUser(theSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.AnyEditor);
        }

        private void contextMenuStripLineOptions_Opening(object sender, CancelEventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe) || String.IsNullOrEmpty(_lastLineOptionsButtonClicked))
                return;

            int nLineIndex;
            var verseData = VerseDataFromLineOptionsButtonId(_lastLineOptionsButtonClicked, out nLineIndex);
            moveLineUp.Enabled = (nLineIndex > 1);
            moveLineDown.Enabled = (nLineIndex < theSe.TheCurrentStory.Verses.Count);
            hideVerseToolStripMenuItem.Text = (verseData.IsVisible) ? CstrMenuLabelHide : CstrMenuLabelUnhide;

            // the join story menu should be enabled as long as there's a following story
            var nIndexOfCurrentStory = theSe.TheCurrentStoriesSet.IndexOf(theSe.TheCurrentStory);
            joinStoryToolStripMenuItem.Enabled = (theSe.TheCurrentStoriesSet.Count > nIndexOfCurrentStory + 1);
        }
    }
}
