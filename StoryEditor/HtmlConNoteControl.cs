#define AddNoteFromConNotes

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetLoc;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    // data-action values on the note panes' buttons (VerseData/ConsultNoteDataConverter make them; PaneCommon.js
    //  sends them as 'action' messages)
    internal static class NoteActions
    {
        public const string AddNote = "addNote";
        public const string AddNoteToSelf = "addNoteToSelf";
        public const string AddStickyNote = "addStickyNote";
        public const string ShowHideOpen = "showHideOpen";
        public const string Delete = "delete";
        public const string ConvertToMentoree = "convertToMentoree";   // data-arg = needs approval (true/false)
        public const string ConvertToMentor = "convertToMentor";
        public const string ConvertToMentorToSelf = "convertToMentorToSelf";
        public const string ConvertToMenteeToSelf = "convertToMenteeToSelf";
        public const string Approve = "approve";
        public const string EndConversation = "endConversation";
    }

    public abstract class HtmlConNoteControl : HtmlVerseControl
    {
        public abstract string PaneLabel();

        protected HtmlConNoteControl()
            : this(null)
        {
        }

        protected internal HtmlConNoteControl(IHtmlHost host)
            : base(host)
        {
            InitializeComponent();

            Dispatcher.Register("textChanged", msg => TextareaOnKeyUp(msg.GetString("id"), msg.GetString("value") ?? String.Empty, msg.GetBool("quiet")));
            Dispatcher.Register("contextMenu", msg => ShowContextMenu());
            Dispatcher.Register("scriptureDropped", msg => CopyScriptureReference(msg.GetString("id")));
            Dispatcher.Register("action", OnAction);
        }

        private void OnAction(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            switch (msg.GetString("name"))
            {
                case NoteActions.AddNote:
                    if (Int32.TryParse(strId, out var nVerseIndex))     // this button's id is the bare line index
                        OnAddNote(nVerseIndex, null, false);
                    break;
                case NoteActions.AddNoteToSelf: OnAddNoteToSelf(strId); break;
                case NoteActions.AddStickyNote: OnAddStickyNote(strId); break;
                case NoteActions.ShowHideOpen: OnShowHideOpenConversations(strId); break;
                case NoteActions.Delete: OnClickDelete(strId); break;
                case NoteActions.ConvertToMentoree: OnConvertToMentoreeNote(strId, msg.GetBool("arg")); break;
                case NoteActions.ConvertToMentor: OnConvertToMentorNote(strId); break;
                case NoteActions.ConvertToMentorToSelf: OnConvertToMentorNoteToSelf(strId); break;
                case NoteActions.ConvertToMenteeToSelf: OnConvertToMentoreeNoteToSelf(strId); break;
                case NoteActions.Approve: OnApproveNote(strId); break;
                case NoteActions.EndConversation: OnClickEndConversation(strId); break;
                default:
                    System.Diagnostics.Debug.WriteLine("HtmlConNoteControl: unknown action " + msg);
                    break;
            }
        }

        // only the Consultant Notes pane has an Approve button
        protected virtual bool OnApproveNote(string strId)
        {
            return false;
        }

        public override StoryData StoryData
        {
            set
            {
                System.Diagnostics.Debug.Assert((value == null) || (TheSE != null));
                base.StoryData = value;
                if (value == null)
                    return;

                // for ConNotes, we also have to do the 'insure extra box' thingy
                //  (there are actually one more verses than 'Count', but DataConverter(i)
                //  handles that for us)
                for (int i = 0; i <= StoryData.Verses.Count; i++)
                {
                    ConsultNotesDataConverter aCNsDC = DataConverter(i);
                    foreach (ConsultNoteDataConverter dc in aCNsDC)
                        aCNsDC.InsureExtraBox(dc, TheSE.TheCurrentStory, 
                                TheSE.LoggedOnMember, TheSE.StoryProject.TeamMembers);
                }
            }
        }

        public abstract ConsultNotesDataConverter DataConverter(int nVerseIndex);

        public ConsultNoteDataConverter DataConverter(int nVerseIndex, int nConversationIndex)
        {
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            System.Diagnostics.Debug.Assert(aCNsDC.Count > nConversationIndex);
            return aCNsDC[nConversationIndex];
        }

        public bool OnAddNote(int nVerseIndex, string strReferringText, bool bNoteToSelf)
        {
            var eNoteType = (bNoteToSelf)
                                ? ConsultNoteDataConverter.NoteType.NoteToSelf
                                : ConsultNoteDataConverter.NoteType.RegularNote;
            return CallDoAddNote(nVerseIndex, strReferringText, eNoteType);
        }

        // this form is called from VersesData.GetHeaderRow (html)
        public bool OnAddNoteToSelf(string strButtonId)
        {
            var astrId = strButtonId.Split('_');
            System.Diagnostics.Debug.Assert(astrId.Length == 2);
            var nVerseIndex = Convert.ToInt32(astrId[1]);
            return CallDoAddNote(nVerseIndex, null, ConsultNoteDataConverter.NoteType.NoteToSelf);
        }

        public bool OnAddStickyNote(string strButtonId)
        {
            return CallDoAddNote(0, null, ConsultNoteDataConverter.NoteType.StickyNote);
        }

        private bool CallDoAddNote(int nVerseIndex, string strReferringText, ConsultNoteDataConverter.NoteType eNoteType)
        {
            // StrIdToScrollTo = GetNextRowId;
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            ConsultNoteDataConverter aCNDC = DoAddNote(strReferringText, aCNsDC, nVerseIndex, eNoteType);
            
            // if we couldn't determine the top-most row, then just get the line row
            // if ((aCNDC != null) && String.IsNullOrEmpty(StrIdToScrollTo))
            if (aCNDC != null)
            {
                StrIdToScrollTo = ConsultNoteDataConverter.ButtonRowId(nVerseIndex, aCNsDC.IndexOf(aCNDC));
            }
            else
                return false;

            return true;
        }

        public bool OnShowHideOpenConversations(string strButtonId)
        {
            StrIdToScrollTo = GetTopRowId;
            string[] astrId = strButtonId.Split('_');
            System.Diagnostics.Debug.Assert(astrId.Length ==2);
            int nVerseIndex = Convert.ToInt32(astrId[1]);

            // toggle state of 'Show All' or 'Hide Closed' button
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            aCNsDC.ShowOpenConversations = !aCNsDC.ShowOpenConversations;
            
            // brute force (no need to repaint the button since the reload will do it for us
            LoadDocument();
            // don't think we need this anymore
            // Application.DoEvents();
            // ScrollToVerse(nVerseIndex);
            return true;
        }

        public bool OnClickDelete(string strId)
        {
            if (!GetDataConverters(strId, out int nVerseIndex, out int nConversationIndex,
                out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC))
                return false;

            if (theCNDC.HasData)
            {
#if !UseOlderMsgBox
                var res = new CustomMsgBox(Localizer.Str("Delete or Hide?"), "This conversation isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around for history. Click 'Delete' to delete the conversation or click 'Hide' to hide it?", "Delete", "Hide")
                               .ShowDialog();

                if (res == DialogResult.Retry)
                    return OnClickHide(strId);

                if (res == DialogResult.Cancel)
                    return true;
#else
                DialogResult res = LocalizableMessageBox.Show(Localizer.Str("This conversation isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around for history. Click 'Yes' to hide the conversation or click 'No' to delete it?"),
                    StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                    return OnClickHide(strId);

                if (res == DialogResult.Cancel)
                    return true;
#endif
            }

            StrIdToScrollTo = GetTopRowId;
            theCNsDC.Remove(theCNDC);
            LoadDocument();
            return true;
        }

        public bool OnClickHide(string strId)
        {
            if (!GetDataConverters(strId, out int nVerseIndex, out int nConversationIndex,
                out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC))
                return false;

            // if there's only one and it's empty, then just delete it
            if (!theCNDC.HasData)
                OnClickDelete(strId);

            StrIdToScrollTo = GetTopRowId;
            theCNDC.Visible = !theCNDC.Visible;

            // otherwise, we have to reload the document
            LoadDocument();
            return true;
        }

        public bool OnClickEndConversation(string strId)
        {
            if (!GetDataConverters(strId, out int nVerseIndex, out int nConversationIndex,
                out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC))
                return false;

            if (theCNDC.IsFinished)
            {
                theCNDC.IsFinished = false;

                // also if it were hidden, then make it unhidden
                theCNDC.Visible = true;
            }
            else
            {
                theCNDC.IsFinished = true;
            }

            StrIdToScrollTo = GetTopRowId;
            if (theCNDC.IsFinished)
            {
                if (!theCNDC.FinalComment.HasData)
                    theCNDC.Remove(theCNDC.FinalComment);
            }
            else
            {
                // just in case we need to have an open box now
                theCNsDC.InsureExtraBox(theCNDC, TheSE.TheCurrentStory,  
                    TheSE.LoggedOnMember, TheSE.StoryProject.TeamMembers);
            }

            if (String.IsNullOrEmpty(StrIdToScrollTo))
            {
                if (theCNDC.IsEditable(TheSE.LoggedOnMember, TheSE.StoryProject.TeamMembers,
                    TheSE.TheCurrentStory))
                    StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, nConversationIndex);
                else
                    StrIdToScrollTo = ConsultNoteDataConverter.TextareaReadonlyRowId(nVerseIndex, nConversationIndex,
                                                                                     theCNDC.Count - 1);
            }

            LoadDocument();
            return true;
        }

        /* doesn't seem to work... the 'value' member isn't updated until *after*
         * keyPress is executed. I could use event.keyCode to get the latest key
         * pressed, but that's not what I want. So have to use onKeyUp
        public bool TextareaOnKeyPress(string strId, string strText)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return false;

            int nVerseIndex, nConversationIndex;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex))
                return false;

            ConsultNoteDataConverter theCNDC = DataConverter(nVerseIndex, nConversationIndex);
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > 0));
            CommInstance aCI = theCNDC[theCNDC.Count - 1];
            System.Diagnostics.Debug.WriteLine(String.Format("Was: {0}, now: {1}",
                aCI, strText));
            aCI.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;
            return true;
        }
        */
        
        private void CopyScriptureReference(string strId)
        {
            if (!GetIndicesFromId(strId, out int nVerseIndex, out int nConversationIndex, out int nDontCare))
                return;

            // the page appends it and sends textChanged, which puts it in the note like typing would
            Host.Post("appendText", new { id = strId, text = TheSE.GetNetBibleScriptureReference, focus = true });
        }

        private void ShowContextMenu()
        {
            if (StoryEditor.TextPaster != null)
                return;
            contextMenu.Show(MousePosition);
        }

        private bool TextareaOnKeyUp(string strId, string strText, bool bQuiet)
        {
            if (!GetIndicesFromId(strId, out int nVerseIndex, out int nConversationIndex, out int nDontCare))
                return false;

            ConsultNotesDataConverter theCNsDC = DataConverter(nVerseIndex);
            if (!CheckForProperEditToken(theCNsDC, out StoryEditor theSE))
                return false;

            ConsultNoteDataConverter theCNDC = theCNsDC[nConversationIndex];
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > 0));

            CommInstance aCI = theCNDC.FinalComment;

            // nothing changed (e.g. an arrow key, or a flush): don't mark the project modified
            if (PaneText.IsSame(aCI, strText))
                return true;

            aCI.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;
            if (!bQuiet)
                theSE.LastKeyPressedTimeStamp = DateTime.Now;   // so we can delay the autosave while typing

            // update the status bar (in case we previously put an error there
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[theSE.TheCurrentStory.ProjStage.ProjectStage];
            theSE.SetDefaultStatusBar(st.StageDisplayString);

            return true;
        }

        protected bool GetDataConverters(string strId, out int nVerseIndex, out int nConversationIndex,
            out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC)
        {
            theCNsDC = null;
            theCNDC = null;

            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex, out int nDontCare))
                return false;

            theCNsDC = DataConverter(nVerseIndex);
            System.Diagnostics.Debug.Assert((theCNsDC != null) && (theCNsDC.Count > nConversationIndex));

            /* I shouldn't need this anymore, because if it's not allowed, then there shouldn't
             * be buttons visible to get here
            StoryEditor theSE;
            if (!CheckForProperEditToken(theCNsDC, out theSE))
                return false;
            */

            theCNDC = theCNsDC[nConversationIndex];

            // this always leads to the document being modified
            TheSE.Modified = true;
            
            return true;
        }

        protected bool CheckForProperEditToken(ConsultNotesDataConverter theCNsDC, 
            out StoryEditor theSE)
        {
            theSE = TheSE;  // (StoryEditor)FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        Localizer.Str("Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com"));

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

#if !JustMentorsCanAddNotesWhenNotTheirTurn
                string strPfMemberId = null;
                if (theSE.TheCurrentStory != null)
                    strPfMemberId = theSE.TheCurrentStory.CraftingInfo.ProjectFacilitator.MemberId;
                if (theCNsDC.HasAddNotePrivilege(theSE.LoggedOnMember, strPfMemberId))
                    return true;
#else
                if (((this is HtmlConsultantNotesControl) && (theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eConsultantInTraining))
                    || ((this is HtmlCoachNotesControl) && (theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eCoach)))
                {
                    return true;
                }
#endif

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.TheCurrentStory);
            }
            catch (Exception ex)
            {
                theSE?.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        public ConsultNoteDataConverter DoAddNote(string strReferringText, ConsultNotesDataConverter aCNsDC, 
                                                  int nVerseIndex, ConsultNoteDataConverter.NoteType eNoteType)
        {
            // the only function of the button here is to add a slot to type a con note
            if (!CheckForProperEditToken(aCNsDC, out StoryEditor theSE))
                return null;

            // if we're not given anything to put in the box, at least put in the logged
            //  in member's initials and re
            // (but not if we're pasting)
            string strNote = null;
            if ((theSE.LoggedOnMember != null) &&
                (StoryEditor.TextPaster == null) &&
                (eNoteType != ConsultNoteDataConverter.NoteType.StickyNote))
            {
                strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + StoryEditor.DateForConNote;
            }

            var cndc = aCNsDC.Add(theSE.TheCurrentStory, theSE.LoggedOnMember, theSE.StoryProject.TeamMembers,
                strReferringText, strNote, eNoteType);
            System.Diagnostics.Debug.Assert(cndc.Count == 1);

            // if there's referring text, then do it in a separate dialog so we can 'preview' the referring text
            if (!String.IsNullOrEmpty(strReferringText) && theSE.advancedUseDialogToPreviewConNotes.Checked)
            {
                var nConversationIndex = aCNsDC.IndexOf(cndc);
                cndc.DontShowButtonsOverride = true;
                strNote = StoryData.ConNoteHtml(this, theSE.StoryProject.ProjSettings, nVerseIndex,
                                                nConversationIndex, theSE.LoggedOnMember,
                                                theSE.StoryProject.TeamMembers, cndc);
                var dlg = new AddConNoteForm(GetType(), theSE, StoryData, strNote);
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    aCNsDC.Remove(cndc);
                    return null;
                }
                cndc.DontShowButtonsOverride = false;
            }

            // StrIdToScrollTo = GetTopRowId;
            LoadDocument();
            theSE.Modified = true;

            CheckUpdateMentorInfo(theSE);

            // return the conversation we just created
            return cndc;
        }

        protected abstract void CheckUpdateMentorInfo(StoryEditor theSe);

        private const string CstrParagraphHighlightBegin = "<span style=\"background-color:Blue; color: White\">";
        private ToolStripMenuItem menuAddNote;
        private ToolStripMenuItem menuAddNoteToSelf;
        private ToolStripMenuItem menuConNoteToFont;
        private const string CstrParagraphHighlightEnd = "</span>";

        public void SetSelection(StringTransfer stringTransfer,
            int nFoundIndex, int nLengthToSelect)
        {
            System.Diagnostics.Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (IsTextareaElement(stringTransfer.HtmlElementId))
            {
                Host.Post("selectRange", new { id = stringTransfer.HtmlElementId, start = nFoundIndex, length = nLengthToSelect });
            }
            else if (IsParagraphElement(stringTransfer.HtmlElementId))
            {
                var str = NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight(stringTransfer.ToString(),
                                                                        nFoundIndex, nLengthToSelect,
                                                                        CstrParagraphHighlightBegin,
                                                                        CstrParagraphHighlightEnd);
                Host.Post("setHtml", new { id = stringTransfer.HtmlElementId, html = str });
            }
        }

        protected bool GetIndicesFromId(string strId,
            out int nVerseIndex, out int nConversationIndex, out int nCommentIndex)
        {
            nCommentIndex = 0;
            try
            {
                string[] aVerseConversationIndices = strId.Split(AchDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices.Length == 3) ||
                                                 (aVerseConversationIndices.Length == 4))
                                                &&
                                                ((aVerseConversationIndices[0] == CstrTextAreaPrefix) ||
                                                 (aVerseConversationIndices[0] == CstrParagraphPrefix) ||
                                                 (aVerseConversationIndices[0] == CstrButtonPrefix)));

                nVerseIndex = Convert.ToInt32(aVerseConversationIndices[1]);
                nConversationIndex = Convert.ToInt32(aVerseConversationIndices[2]);
                if (aVerseConversationIndices.Length == 4)
                    nCommentIndex = Convert.ToInt32(aVerseConversationIndices[3]);
            }
            catch
            {
                nVerseIndex = 0;
                nConversationIndex = 0;
                return false;
            }
            return true;
        }

        public bool OnConvertToMentoreeNote(string strId, bool bNeedsApproval)
        {
            return SetDirectionTo(strId, bNeedsApproval, true);
        }

        public bool OnConvertToMentorNote(string strId)
        {
            // don't pass a value for 'NeedsApproval', so it'll be calculated
            return SetDirectionTo(strId, null, false);
        }

        public bool OnConvertToMentorNoteToSelf(string strId)
        {
            return SetDirectionTo(strId, false, true, true);
        }

        public bool OnConvertToMentoreeNoteToSelf(string strId)
        {
            return SetDirectionTo(strId, false, false, true);
        }

        protected bool SetDirectionTo(string strId, 
            bool? bNeedsApproval, bool bToMentorDirection, bool bToNoteToSelf = false)
        {
            if (!GetDataConverters(strId, out int nVerseIndex, out int nConversationIndex,
                                   out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC))
            {
                return false;
            }

            // means we should calculate whether it needs approval or not
            // only needs approval if this is the consultant notes pane and ...
            bNeedsApproval ??= (this is HtmlConsultantNotesControl) &&
                              ConsultantNoteData.CalculateWhetherNoteNeedsApproval(TheSE.LoggedOnMember, StoryData, ConsultNoteDataConverter.NoteType.RegularNote);

            theCNDC.FinalComment.Direction = (bToNoteToSelf)
                                                ? (bToMentorDirection)
                                                    ? theCNDC.MentorToSelfDirection
                                                    : theCNDC.MenteeToSelfDirection
                                                : (bToMentorDirection)
                                                     ? ((bool)bNeedsApproval)
                                                           ? ConsultNoteDataConverter.CommunicationDirections.
                                                                 eConsultantToProjFacNeedsApproval
                                                           : theCNDC.MentorDirection
                                                     : theCNDC.MenteeDirection;

            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
            return true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddNote = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddNoteToSelf = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConNoteToFont = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddNote,
            this.menuAddNoteToSelf,
            this.menuConNoteToFont});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(244, 48);
            // 
            // menuAddNote
            // 
            this.menuAddNote.Name = "menuAddNote";
            this.menuAddNote.Size = new System.Drawing.Size(243, 22);
            this.menuAddNote.Text = "Add note on selected text";
            this.menuAddNote.Click += new System.EventHandler(this.MenuAddNote_Click);
            // 
            // menuAddNoteToSelf
            // 
            this.menuAddNoteToSelf.Name = "menuAddNoteToSelf";
            this.menuAddNoteToSelf.Size = new System.Drawing.Size(243, 22);
            this.menuAddNoteToSelf.Text = "Add note to self on selected text";
            this.menuAddNoteToSelf.Click += new System.EventHandler(this.MenuAddNoteToSelf_Click);
            // 
            // menuConNoteToFont
            // 
            this.menuConNoteToFont.Name = "menuConNoteToFont";
            this.menuConNoteToFont.Size = new System.Drawing.Size(243, 22);
            this.menuConNoteToFont.Text = $"Change font used for {PaneLabel()} pane";
            this.menuConNoteToFont.Click += new System.EventHandler(this.ToolStripMenuItemConNoteChangeFont_Click);
            // 
            // HtmlConNoteControl
            // 
            // this is now down manually (see ShowContextMenu) so we can turn it off when TextPaster is active
            // this.ContextMenuStrip = this.contextMenu;
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void MenuAddNoteToSelf_Click(object sender, EventArgs e)
        {
            bool bNoteToSelf = true;
            ConNoteAddNote(bNoteToSelf);
        }

        public string SettingsKeyForFontToUse
        {
            get
            {
                return $"FontFor{PaneLabel()}Pane";
            }
        }

        private void ToolStripMenuItemConNoteChangeFont_Click(object sender, EventArgs e)
        {
            var settingKeyForFontToUse = SettingsKeyForFontToUse;

            // if we have this in the user config, then pre-select it for the Font dialog
            var fontDialog = new FontDialog();
            if (NetBibleViewer.ReadFontNameAndSizeFromUserConfig(settingKeyForFontToUse,
                out string strFontName, out string strFontSize))
            {
                if (!float.TryParse(strFontSize, out float fFontSize))
                    fFontSize = 12F;
                fontDialog = new FontDialog { Font = new System.Drawing.Font(strFontName, fFontSize) };
            }

            // query what the user wants
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            strFontName = String.Format("{0};{1}", fontDialog.Font.Name, fontDialog.Font.Size);
            if (!Program.MapSwordModuleToFont.ContainsKey(settingKeyForFontToUse))
            {
                Program.MapSwordModuleToFont.Add(settingKeyForFontToUse, strFontName);
            }
            else
            {
                Program.MapSwordModuleToFont[settingKeyForFontToUse] = strFontName;
            }

            // save the changes/additions
            Properties.Settings.Default.SwordModuleToFont = Program.DictionaryToArray(Program.MapSwordModuleToFont);
            Properties.Settings.Default.Save();

            if (String.IsNullOrEmpty(Tag as string))   // don't do the re-loading of the document if this is the AddConNote dialog approach
            {
                LoadDocument();
            }
            else
            {
                LocalizableMessageBox.Show(Localizer.Str("Reopen this dialog box to activate the new font selected."),
                    StoryEditor.OseCaption);
            }
        }

        private void MenuAddNote_Click(object sender, EventArgs args)
        {
            bool bNoteToSelf = false;
            ConNoteAddNote(bNoteToSelf);
        }

        readonly Regex regexStripTableBits = new("</?(TD|TR|FONT|TEXTAREA|TBODY|TABLE|BUTTON).*?>", RegexOptions.Compiled | RegexOptions.Singleline);

        private void ConNoteAddNote(bool bNoteToSelf)
        {
            // the page finds the selection and the line it's on (getNoteSelection in ConNoteDomPrefix.js)
            var reply = Host.Request("getNoteSelection", null, HtmlHostDefaults.RequestTimeout);
            if ((reply == null) || !reply.TryGetInt("lineIndex", out var nLineNumber))
                return;

            var strHtml = reply.GetString("html");
            if (String.IsNullOrEmpty(strHtml))
                return;

            var strReferringText = String.Format("<p><i>{0}</i></p>", Localizer.Str("Re: ConNote:"));

            // add the selection to the referring text, but strip out any bits which look like table parts
            //  (they don't add so easily)
            strReferringText += regexStripTableBits.Replace(strHtml, "");
            TheSE.SendNoteToCorrectPane(nLineNumber, strReferringText, bNoteToSelf);
        }

        private ContextMenuStrip contextMenu;
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class HtmlConsultantNotesControl : HtmlConNoteControl
    {
        public HtmlConsultantNotesControl()
        {
        }

        internal HtmlConsultantNotesControl(IHtmlHost host)
            : base(host)
        {
        }

        public override void LoadDocument()
        {
            NetBibleViewer.ReadFontNameAndSizeFromUserConfig(SettingsKeyForFontToUse, out string strFontName, out string strFontSize);

            var strHtml = StoryData.ConsultantNotesHtml(this,
                                                        TheSE.StoryProject.ProjSettings,
                                                        TheSE.LoggedOnMember,
                                                        TheSE.StoryProject.TeamMembers,
                                                        TheSE.viewHiddenVersesMenu.Checked,
                                                        TheSE.viewOnlyOpenConversationsMenu.Checked,
                                                        strFontName, strFontSize);
            LoadHtml(strHtml);
            MakeLineNumberLinkVisible?.Invoke();
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Consultant Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = GetVerseData(nVerseIndex);
            if (verse == null)
                return null;
            ConsultNotesDataConverter aCNsDC = verse.ConsultantNotes;
            return aCNsDC;
        }

        protected override void CheckUpdateMentorInfo(StoryEditor theSe)
        {
            theSe.CheckUpdateMentorInfoConsultant();
        }

        public override void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, false, true);
        }

        // this only applies to the Consultant Note pane
        protected override bool OnApproveNote(string strId)
        {
            return SetDirectionTo(strId, false, true);
        }
    }

    public class HtmlCoachNotesControl : HtmlConNoteControl
    {
        public HtmlCoachNotesControl()
        {
        }

        internal HtmlCoachNotesControl(IHtmlHost host)
            : base(host)
        {
        }

        public override void LoadDocument()
        {
            NetBibleViewer.ReadFontNameAndSizeFromUserConfig(SettingsKeyForFontToUse, out string strFontName, out string strFontSize);

            var strHtml = StoryData.CoachNotesHtml(this,
                                                   TheSE.StoryProject.ProjSettings,
                                                   TheSE.LoggedOnMember,
                                                   TheSE.StoryProject.TeamMembers,
                                                   TheSE.viewHiddenVersesMenu.Checked,
                                                   TheSE.viewOnlyOpenConversationsMenu.Checked,
                                                   strFontName, strFontSize);
            LoadHtml(strHtml);

            MakeLineNumberLinkVisible?.Invoke();
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Coach Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = GetVerseData(nVerseIndex);
            if (verse == null)
                return null;
            ConsultNotesDataConverter aCNsDC = verse.CoachNotes;
            return aCNsDC;
        }

        protected override void CheckUpdateMentorInfo(StoryEditor theSe)
        {
            theSe.CheckUpdateMentorInfoCoach();
        }

        public override void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, true, false);
        }
    }
}
