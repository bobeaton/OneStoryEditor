using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;
using NetLoc;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract class HtmlConNoteControl : HtmlVerseControl
    {
        public abstract string PaneLabel();

        protected HtmlConNoteControl()
        {
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

        public bool OnAddNote(int nVerseIndex, string strReferringText, string strNote, bool bNoteToSelf)
        {
            return CallDoAddNote(nVerseIndex, strReferringText, strNote, bNoteToSelf);
        }

        /// <summary>
        /// soon to be obsolete once we switch the StoryBt pane over to HTML
        /// </summary>
        /// <param name="nVerseIndex"></param>
        /// <param name="strNote"></param>
        /// <param name="bNoteToSelf"></param>
        /// <returns></returns>
        public bool OnAddNote(int nVerseIndex, string strNote, bool bNoteToSelf)
        {
            return CallDoAddNote(nVerseIndex, null, strNote, bNoteToSelf);
        }

        public bool OnAddNoteToSelf(string strButtonId, string strReferringText, string strNote)
        {
            string[] astrId = strButtonId.Split('_');
            System.Diagnostics.Debug.Assert(astrId.Length == 2);
            int nVerseIndex = Convert.ToInt32(astrId[1]);

            return CallDoAddNote(nVerseIndex, strReferringText, strNote, true);
        }

        private bool CallDoAddNote(int nVerseIndex, string strReferringText, string strNote, bool bNoteToSelf)
        {
            StrIdToScrollTo = GetTopRowId;
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            ConsultNoteDataConverter aCNDC = DoAddNote(strReferringText, strNote, aCNsDC, nVerseIndex, bNoteToSelf);
            
            // if we couldn't determine the top-most row, then just get the line row
            if ((aCNDC != null) && String.IsNullOrEmpty(StrIdToScrollTo))
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, aCNsDC.IndexOf(aCNDC));
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
                DialogResult res = LocalizableMessageBox.Show(Localizer.Str("This conversation isn't empty! Instead of deleting it, it would be better to just hide it so it will be left around for history. Click 'Yes' to hide the conversation or click 'No' to delete it?"),
                    StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                    return OnClickHide(strId);

                if (res == DialogResult.Cancel)
                    return true;
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

        public ConsultNoteDataConverter DoAddNote(string strReferringText, string strNote,
            ConsultNotesDataConverter aCNsDC, int nVerseIndex, bool bNoteToSelf)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(aCNsDC, out theSE))
                return null;

            // if we're not given anything to put in the box, at least put in the logged
            //  in member's initials and re
            // (but not if we're pasting)
            if (String.IsNullOrEmpty(strNote) && 
                (theSE.LoggedOnMember != null) &&
                (StoryEditor.TextPaster == null))
                strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + StoryEditor.StrRegarding;

            StrIdToScrollTo = GetTopRowId;

            ConsultNoteDataConverter cndc =
                aCNsDC.Add(theSE.TheCurrentStory, theSE.LoggedOnMember,
                theSE.StoryProject.TeamMembers, strReferringText, strNote, bNoteToSelf);
            System.Diagnostics.Debug.Assert(cndc.Count == 1);

            cndc.DontShowButtonsOverride = true;
            strNote = StoryData.ConNoteHtml(this, theSE.StoryProject.ProjSettings, nVerseIndex,
                                            theSE.LoggedOnMember,
                                            theSE.StoryProject.TeamMembers, cndc);
            var dlg = new AddConNoteForm(GetType(), theSE, StoryData, strNote);
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                aCNsDC.Remove(cndc);
                return null;
            }
            cndc.DontShowButtonsOverride = false;
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
            return SetDirectionTo(strId, false, false);
        }

        protected bool SetDirectionTo(string strId, 
            bool bNeedsApproval, bool bToMentorDirection)
        {
            int nVerseIndex, nConversationIndex;
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out nVerseIndex, out nConversationIndex,
                                   out theCNsDC, out theCNDC))
                return false;

            theCNDC.FinalComment.Direction = (bToMentorDirection)
                                                 ? (bNeedsApproval)
                                                       ? ConsultNoteDataConverter.CommunicationDirections.
                                                             eConsultantToProjFacNeedsApproval
                                                       : theCNDC.MentorDirection
                                                 : theCNDC.MenteeDirection;
            StrIdToScrollTo = GetTopRowId;
            LoadDocument();
            return true;
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
            LineNumberLink.Visible = true;
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Consultant Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
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
            LineNumberLink.Visible = true;
        }

        public override string PaneLabel()
        {
            return Localizer.Str("Coach Notes");
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
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
