#define AddNoteFromConNotes

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mshtml;
using NetLoc;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract class HtmlConNoteControl : HtmlVerseControl
    {
        public abstract string PaneLabel();

        protected HtmlConNoteControl()
        {
            InitializeComponent();
#if AddNoteFromConNotes
            IsWebBrowserContextMenuEnabled = false;
#endif
            ObjectForScripting = this;
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
            aCNsDC.ShowOpenConversations = (aCNsDC.ShowOpenConversations) ? false : true;
            
            // brute force (no need to repaint the button since the reload will do it for us
            LoadDocument();
            // don't think we need this anymore
            // Application.DoEvents();
            // ScrollToVerse(nVerseIndex);
            return true;
        }

        public bool OnClickDelete(string strId)
        {
            int nVerseIndex, nConversationIndex;
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out nVerseIndex, out nConversationIndex, 
                out theCNsDC, out theCNDC))
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
            bool bRemovedLast = (theCNsDC.IndexOf(theCNDC) == (theCNsDC.Count - 1));
            theCNsDC.Remove(theCNDC);

            // remove the HTML elements for the row of buttons and the conversation table
            //  (but only if it was the last conversation. If it wasn't, then the other
            //  conversations will have out of sequence ids, so we'll just *have* to do
            //  LoadDoc
            if (bRemovedLast
                && 
                RemoveHtmlNodeById(ConsultNoteDataConverter.ButtonRowId(nVerseIndex, nConversationIndex))
                && 
                RemoveHtmlNodeById(ConsultNoteDataConverter.ConversationTableRowId(nVerseIndex, nConversationIndex)))
            {
                return true;
            }

            LoadDocument();
            return true;
        }

        public bool OnClickHide(string strId)
        {
            int nVerseIndex, nConversationIndex;
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out nVerseIndex, out nConversationIndex, 
                out theCNsDC, out theCNDC))
                return false;

            // if there's only one and it's empty, then just delete it
            if (!theCNDC.HasData)
                OnClickDelete(strId);

            StrIdToScrollTo = GetTopRowId;
            theCNDC.Visible = (theCNDC.Visible) ? false : true;

            /*
            if (TheSE.hiddenVersesToolStripMenuItem.Checked)
            {
                // then we just swap the text on the button
                if (Document != null)
                {
                    // repaint the button to be 'hide'
                    // since the strId might be from a Delete request (where the user
                    //  said "yes" to our request to hide it instead), we have to 
                    //  rebuild the ID for the Hide button, which is:
                    strId = ConsultNoteDataConverter.ButtonId(nVerseIndex,
                        nConversationIndex, ConsultNoteDataConverter.CnBtnIndexHide);
                    HtmlElement elemButtonHide = Document.GetElementById(strId);
                    if (elemButtonHide != null)
                    {
                        elemButtonHide.InnerText = (theCNDC.Visible)
                                                       ? ConsultNoteDataConverter.CstrButtonLabelHide
                                                       : ConsultNoteDataConverter.CstrButtonLabelUnhide;
                        return true;
                    }
                }
            }
            // otherwise remove the row of buttons and the embedded conversation table
            // if it's not invisible (but only if it's the last conversation... if it
            //  isn't the last one, then just hiding it won't work, because the subsequent
            //  conversations will have the wrong index (so we *must* do LoadDoc)
            else if (!theCNDC.Visible 
                    && (theCNsDC.Count == (nConversationIndex - 1))
                    && RemoveHtmlNodeById(ConsultNoteDataConverter.ButtonRowId(nVerseIndex, nConversationIndex))
                    && RemoveHtmlNodeById(ConsultNoteDataConverter.ConversationTableRowId(nVerseIndex, nConversationIndex)))
            {
                return true;
            }
            */

            // otherwise, we have to reload the document
            LoadDocument();
            return true;
        }

        protected bool RemoveHtmlNodeById(string strId)
        {
            if (Document != null)
            {
                HTMLDocumentClass htmldoc = (HTMLDocumentClass)Document.DomDocument;
                if (htmldoc != null)
                {
                    IHTMLDOMNode node = (IHTMLDOMNode)htmldoc.getElementById(strId);
                    if (node != null)
                    {
                        node.parentNode.removeChild(node);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool OnClickEndConversation(string strId)
        {
            int nVerseIndex, nConversationIndex;
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out nVerseIndex, out nConversationIndex, 
                out theCNsDC, out theCNDC))
                return false;

            System.Diagnostics.Debug.Assert(Document != null);
            HtmlElement elemButton = Document.GetElementById(strId);
            System.Diagnostics.Debug.Assert(elemButton != null);

            if (theCNDC.IsFinished)
            {
                theCNDC.IsFinished = false;
                elemButton.InnerText = ConsultNoteDataConverter.CstrButtonLabelConversationEnd;

                // also if it were hidden, then make it unhidden
                theCNDC.Visible = true;
            }
            else
            {
                theCNDC.IsFinished = true;
                elemButton.InnerText = ConsultNoteDataConverter.CstrButtonLabelConversationReopen;
            }

            StrIdToScrollTo = GetTopRowId;
            if (theCNDC.IsFinished)
            {
#if !DontAlwaysDoLoadDoc
                if (!theCNDC.FinalComment.HasData)
                    theCNDC.Remove(theCNDC.FinalComment);
#else
                if (Document != null)
                {
                    HtmlElement elem =
                        Document.GetElementById(ConsultNoteDataConverter.TextareaId(nVerseIndex, nConversationIndex));
                    if (elem != null)
                    {
                        if (String.IsNullOrEmpty(elem.InnerText))
                        {
                            theCNDC.RemoveAt(theCNDC.Count - 1);
                            if (RemoveHtmlNodeById(ConsultNoteDataConverter.TextareaRowId(nVerseIndex,
                                                                                          nConversationIndex)))
                                return true;
                        }
                    }
                }
#endif
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

#if false   // this doesn't really help (it makes it want to bring either the editable box (if reopening) or the
            // the now readonly final box (if closed) to either the top or bottom... jumps around too much
            if (elemButton.InnerText != ConsultNoteDataConverter.CstrButtonLabelConversationReopen)
            {
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, nConversationIndex);
            }
            else
            {
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaReadonlyRowId(nVerseIndex, nConversationIndex, theCNDC.Count - 1);
            }
#endif
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
        
        public void CopyScriptureReference(string strId)
        {
            int nVerseIndex, nConversationIndex, nDontCare;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex, out nDontCare))
                return;

            ConsultNoteDataConverter theCNDC = DataConverter(nVerseIndex, nConversationIndex);
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > 0));
            CommInstance aCI = theCNDC.FinalComment;

            if (Document != null)
            {
                HtmlDocument doc = Document;
                HtmlElement elem = doc.GetElementById(strId);
                if (elem != null)
                {
                    elem.InnerText += TheSE.GetNetBibleScriptureReference;
                    aCI.SetValue(elem.InnerText);
                    elem.Focus();
                }
            }
        }

        public void ShowContextMenu()
        {
            if (StoryEditor.TextPaster != null)
                return;
            contextMenu.Show(MousePosition);
        }

        public bool TextareaOnKeyUp(string strId, string strText)
        {
            int nVerseIndex, nConversationIndex, nDontCare;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex, out nDontCare))
                return false;

            ConsultNotesDataConverter theCNsDC = DataConverter(nVerseIndex);
            StoryEditor theSE;
            if (!CheckForProperEditToken(theCNsDC, out theSE))
                return false;

            ConsultNoteDataConverter theCNDC = theCNsDC[nConversationIndex];
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > 0));

            CommInstance aCI = theCNDC.FinalComment;
            aCI.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;
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

            int nDontCare;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex, out nDontCare))
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
                if (theSE != null)
                    theSE.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        public ConsultNoteDataConverter DoAddNote(string strReferringText, ConsultNotesDataConverter aCNsDC, 
                                                  int nVerseIndex, ConsultNoteDataConverter.NoteType eNoteType)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(aCNsDC, out theSE))
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

        public void DoFind(string strId)
        {
            if (TheSE.UsingHtmlForStoryBtPane)
                return;

            int nVerseIndex, nConversationIndex, nCommentIndex;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex, out nCommentIndex))
                return;

            ConsultNotesDataConverter theCNsDC = DataConverter(nVerseIndex);
            System.Diagnostics.Debug.Assert((theCNsDC != null) && (theCNsDC.Count > nConversationIndex));
            ConsultNoteDataConverter theCNDC = theCNsDC[nConversationIndex];
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > nCommentIndex));
            SearchForm.LastStringTransferSearched = theCNDC[nCommentIndex];
            TheSE.LaunchSearchForm();
        }

        private const string CstrParagraphHighlightBegin = "<span style=\"background-color:Blue; color: White\">";
        private ToolStripMenuItem menuAddNote;
        private ToolStripMenuItem menuAddNoteToSelf;
        private ToolStripMenuItem menuConNoteToFont;
        private const string CstrParagraphHighlightEnd = "</span>";

        public void SetSelection(StringTransfer stringTransfer,
            int nFoundIndex, int nLengthToSelect)
        {
            System.Diagnostics.Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                if (IsTextareaElement(stringTransfer.HtmlElementId))
                {
                    object[] oaParams = new object[] { stringTransfer.HtmlElementId, nFoundIndex, nLengthToSelect };
                    doc.InvokeScript("textboxSetSelection", oaParams);
                }
                else if (IsParagraphElement(stringTransfer.HtmlElementId))
                {
                    HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                    if (elem != null)
                    {
                        string str = stringTransfer.ToString();
                        str = str.Insert(nFoundIndex + nLengthToSelect, CstrParagraphHighlightEnd);
                        str = str.Insert(nFoundIndex, CstrParagraphHighlightBegin);
                        System.Diagnostics.Debug.WriteLine(str);
                        elem.InnerHtml = str;
                    }
                    else
                        System.Diagnostics.Debug.Assert(false, "unexpected element id in HTML");
                }
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
            int nVerseIndex, nConversationIndex;
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out nVerseIndex, out nConversationIndex,
                                   out theCNsDC, out theCNDC))
            {
                return false;
            }

            // means we should calculate whether it needs approval or not
            if (bNeedsApproval == null)
            {
                // only needs approval if this is the consultant notes pane and ...
                bNeedsApproval = (this is HtmlConsultantNotesControl) &&
                                  ConsultantNoteData.CalculateWhetherNoteNeedsApproval(TheSE.LoggedOnMember, StoryData, ConsultNoteDataConverter.NoteType.RegularNote);
            }

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
            this.menuAddNote.Click += new System.EventHandler(this.menuAddNote_Click);
            // 
            // menuAddNoteToSelf
            // 
            this.menuAddNoteToSelf.Name = "menuAddNoteToSelf";
            this.menuAddNoteToSelf.Size = new System.Drawing.Size(243, 22);
            this.menuAddNoteToSelf.Text = "Add note to self on selected text";
            this.menuAddNoteToSelf.Click += new System.EventHandler(this.menuAddNoteToSelf_Click);
            // 
            // menuConNoteToFont
            // 
            this.menuConNoteToFont.Name = "menuConNoteToFont";
            this.menuConNoteToFont.Size = new System.Drawing.Size(243, 22);
            this.menuConNoteToFont.Text = "Change font to conNote pane";
            this.menuConNoteToFont.Click += new System.EventHandler(this.toolStripMenuItemConNoteChangeFont_Click);
            // 
            // HtmlConNoteControl
            // 
            // this is now down manually (see ShowContextMenu) so we can turn it off when TextPaster is active
            // this.ContextMenuStrip = this.contextMenu;
            this.IsWebBrowserContextMenuEnabled = false;
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void menuAddNoteToSelf_Click(object sender, EventArgs e)
        {
            bool bNoteToSelf = true;
            ConNoteAddNote(bNoteToSelf);
        }

        private void toolStripMenuItemConNoteChangeFont_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Properties.Settings.Default.LastSwordModuleUsed: " + Properties.Settings.Default.LastSwordModuleUsed);

            // if we have this in the user config, then pre-select it for the Font dialog
            var fontDialog = new FontDialog();
            string strFontName, strFontSize;
            if (NetBibleViewer.ReadFontNameAndSizeFromUserConfig(Properties.Settings.Default.FontForConNotePane,
                out strFontName, out strFontSize))
            {
                float fFontSize;
                if (!float.TryParse(strFontSize, out fFontSize))
                    fFontSize = 12F;
                fontDialog = new FontDialog { Font = new System.Drawing.Font(strFontName, fFontSize) };
            }

            // query what the user wants
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            strFontName = String.Format("{0};{1}", fontDialog.Font.Name, fontDialog.Font.Size);
            if (!Program.MapSwordModuleToFont.ContainsKey(Properties.Settings.Default.FontForConNotePane))
            {
                Program.MapSwordModuleToFont.Add(Properties.Settings.Default.FontForConNotePane, strFontName);
            }
            else
            {
                Program.MapSwordModuleToFont[Properties.Settings.Default.FontForConNotePane] = strFontName;
            }

            // save the changes/additions
            Properties.Settings.Default.SwordModuleToFont = Program.DictionaryToArray(Program.MapSwordModuleToFont);
            Properties.Settings.Default.Save();
            //NetBibleViewer.TurnOnResource(Properties.Settings.Default.FontForConNotePane);
        }

        private static Regex regExReadLineNumber = new Regex(@"id=tp_(\d+?)_", RegexOptions.Compiled);

        private void menuAddNote_Click(object sender, EventArgs args)
        {
            bool bNoteToSelf = false;
            ConNoteAddNote(bNoteToSelf);
        }

        Regex regexStripTableBits = new Regex("</?(TD|TR|FONT|TEXTAREA|TBODY|TABLE|BUTTON).*?>", RegexOptions.Compiled | RegexOptions.Singleline);

        private void ConNoteAddNote(bool bNoteToSelf)
        {
            if (Document == null)
                return;

            var htmlDocument = Document.DomDocument as IHTMLDocument2;
            if (htmlDocument == null)
                return;

            var selection = htmlDocument.selection;
            var range = (IHTMLTxtRange) selection.createRange();
            if ((range == null) || String.IsNullOrEmpty(range.htmlText))
                return;

            System.Diagnostics.Debug.WriteLine(range.htmlText);
            var elem = range.parentElement();
            if (elem == null)
                return;

            while (!regExReadLineNumber.IsMatch(elem.innerHTML))
                elem = elem.parentElement;

            var strLineNumber = regExReadLineNumber.Match(elem.innerHTML).Groups[1].Value;
            var nLineNumber = Int32.Parse(strLineNumber);

            var children = elem.children as IHTMLElementCollection;
            if (children == null)
                return;

            var strReferringText = String.Format("<p><i>{0}</i></p>", Localizer.Str("Re: ConNote:"));

            // add the selection to the referring text, but strip out any bits which look like table parts
            //  (they don't add so easily)
            var strExtra = regexStripTableBits.Replace(range.htmlText, "");
            strReferringText += strExtra;
#if false
            var aMarkupService = (IMarkupServices)htmlDocument;
            IMarkupPointer aPointerBegin, aPointerEnd;
            aMarkupService.CreateMarkupPointer(out aPointerBegin);
            aMarkupService.CreateMarkupPointer(out aPointerEnd);
            aMarkupService.MovePointersToRange(range, aPointerBegin, aPointerEnd);
            int pResult;
            aPointerBegin.IsLeftOf(aPointerEnd, out pResult);
            while (pResult > 0)
            {
                IHTMLElement elemThis;
                aPointerBegin.CurrentScope(out elemThis);
                System.Diagnostics.Debug.WriteLine("{0} = {1}", elemThis.tagName, elemThis.outerHTML);
                strReferringText += elemThis.outerHTML;
                aPointerBegin.MoveAdjacentToElement(elemThis, _ELEMENT_ADJACENCY.ELEM_ADJ_AfterEnd);
                aPointerBegin.IsLeftOf(aPointerEnd, out pResult);
            }
#endif

            // var strReferringText = String.Format("<p><i>{0}</i></p><p>{1}</p>", Localizer.Str("Re: ConNote:"), range.htmlText);
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

    [ComVisible(true)]
    public class HtmlConsultantNotesControl : HtmlConNoteControl
    {
        public override void LoadDocument()
        {
            var strHtml = StoryData.ConsultantNotesHtml(this,
                                                        TheSE.StoryProject.ProjSettings,
                                                        TheSE.LoggedOnMember,
                                                        TheSE.StoryProject.TeamMembers,
                                                        TheSE.viewHiddenVersesMenu.Checked,
                                                        TheSE.viewOnlyOpenConversationsMenu.Checked);
            DocumentText = strHtml;
            MakeLineNumberLinkVisible?.Invoke();
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Consultant Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = GetVerseData(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.ConsultantNotes;
            return aCNsDC;
        }

        protected override void CheckUpdateMentorInfo(StoryEditor theSe)
        {
            theSe.CheckUpdateMentorInfoConsultant();
        }

        public void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, false, true);
        }

        // this only applies to the Consultant Note pane
        public bool OnApproveNote(string strId)
        {
            return SetDirectionTo(strId, false, true);
        }
    }

    [ComVisible(true)]
    public class HtmlCoachNotesControl : HtmlConNoteControl
    {
        public override void LoadDocument()
        {
            var strHtml = StoryData.CoachNotesHtml(this,
                                                   TheSE.StoryProject.ProjSettings,
                                                   TheSE.LoggedOnMember,
                                                   TheSE.StoryProject.TeamMembers,
                                                   TheSE.viewHiddenVersesMenu.Checked,
                                                   TheSE.viewOnlyOpenConversationsMenu.Checked);
            DocumentText = strHtml;

            MakeLineNumberLinkVisible?.Invoke();
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Coach Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = GetVerseData(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.CoachNotes;
            return aCNsDC;
        }

        protected override void CheckUpdateMentorInfo(StoryEditor theSe)
        {
            theSe.CheckUpdateMentorInfoCoach();
        }

        public void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, true, false);
        }
    }
}
