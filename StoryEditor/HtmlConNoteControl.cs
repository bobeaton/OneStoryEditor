using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

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
                        aCNsDC.InsureExtraBox(dc, TheSE.theCurrentStory, 
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

        public bool OnAddNote(int nVerseIndex, string strNote)
        {
            return CallDoAddNote(nVerseIndex, strNote, false);
        }

        public bool OnAddNoteToSelf(string strButtonId, string strNote)
        {
            string[] astrId = strButtonId.Split('_');
            System.Diagnostics.Debug.Assert(astrId.Length == 2);
            int nVerseIndex = Convert.ToInt32(astrId[1]);

            return CallDoAddNote(nVerseIndex, strNote, true);
        }

        private bool CallDoAddNote(int nVerseIndex, string strNote, bool bNoteToSelf)
        {
            StrIdToScrollTo = GetTopRowId;
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            ConsultNoteDataConverter aCNDC = DoAddNote(strNote, aCNsDC, bNoteToSelf);
            
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
                DialogResult res = MessageBox.Show(
                    Properties.Resources.IDS_NoteNotEmptyHideQuery,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

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
                theCNsDC.InsureExtraBox(theCNDC, TheSE.theCurrentStory,  
                    TheSE.LoggedOnMember, TheSE.StoryProject.TeamMembers);
            }

            if (String.IsNullOrEmpty(StrIdToScrollTo))
            {
                if (theCNDC.IsEditable(TheSE.LoggedOnMember, TheSE.StoryProject.TeamMembers,
                    TheSE.theCurrentStory))
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
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[theSE.theCurrentStory.ProjStage.ProjectStage];
            theSE.SetStatusBar(String.Format("{0}  Press F1 for instructions", st.StageDisplayString));

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
                        "Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com");

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

#if !JustMentorsCanAddNotesWhenNotTheirTurn
                string strPfMemberId = null;
                if (theSE.theCurrentStory != null)
                    strPfMemberId = theSE.theCurrentStory.CraftingInfo.ProjectFacilitator.MemberId;
                if (theCNsDC.HasAddNotePrivilege(theSE.LoggedOnMember, strPfMemberId))
                    return true;
#else
                if (((this is HtmlConsultantNotesControl) && (theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eConsultantInTraining))
                    || ((this is HtmlCoachNotesControl) && (theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eCoach)))
                {
                    return true;
                }
#endif

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.theCurrentStory);
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message));
                return false;
            }

            return true;
        }

        public ConsultNoteDataConverter DoAddNote(string strNote,
            ConsultNotesDataConverter aCNsDC, bool bNoteToSelf)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(aCNsDC, out theSE))
                return null;

            // if we're not given anything to put in the box, at least put in the logged
            //  in member's initials and re
            if (String.IsNullOrEmpty(strNote) && (theSE.LoggedOnMember != null))
                strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + ": Re: ";

            /* I think this is already done in CheckForProperEditToken now (via HasAddNotePrivilege)
            // if the coach tries to add a note in the consultant's pane, that should fail.
            // (but it's okay for a project facilitator to add one if they have a question
            //  for the consultant)
            if (!aCNsDC.CheckAddNotePrivilege(theSE.SetStatusBar, theSE.LoggedOnMember.MemberType))
                return null;
            */

            StrIdToScrollTo = GetTopRowId;
            ConsultNoteDataConverter cndc =
                aCNsDC.Add(theSE.theCurrentStory, theSE.LoggedOnMember, 
                theSE.StoryProject.TeamMembers, strNote, bNoteToSelf);
            System.Diagnostics.Debug.Assert(cndc.Count == 1);
            LoadDocument();
            theSE.Modified = true;

            // return the conversation we just created
            return cndc;
        }

        public void DoFind(string strId)
        {
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

        public bool IsParagraphElement(string strHtmlId)
        {
            return strHtmlId.Contains(ConsultNoteDataConverter.CstrParagraphPrefix);
        }

        public bool IsTextareaElement(string strHtmlId)
        {
            return strHtmlId.Contains(ConsultNoteDataConverter.CstrTextAreaPrefix);
        }

        public string GetSelectedText(StringTransfer stringTransfer)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            System.Diagnostics.Debug.Assert(IsTextareaElement(stringTransfer.HtmlElementId));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                if (htmlDocument != null)
                {
                    IHTMLSelectionObject selection = htmlDocument.selection;
                    if (selection.type.ToLower() != "text")
                    {
                        MessageBox.Show(Properties.Resources.IDS_CanOnlyChangeConNoteTextareas,
                            OseResources.Properties.Resources.IDS_Caption);
                    }
                    else
                    {
                        IHTMLTxtRange rangeSelection = selection.createRange() as IHTMLTxtRange;
                        if (rangeSelection != null)
                            return rangeSelection.text;
                        // else otherwise nothing selected, so just return
                    }
                }
            }

            return null;
        }

        public bool SetSelectedText(StringTransfer stringTransfer, string strNewValue, out int nNewEndPoint)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            System.Diagnostics.Debug.Assert(IsTextareaElement(stringTransfer.HtmlElementId));
            nNewEndPoint = 0;   // return of 0 means it didn't work.
            if (Document != null)
            {
                HtmlDocument doc = Document;
                IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                if (htmlDocument != null)
                {
                    object[] oaParams = new object[] { stringTransfer.HtmlElementId, strNewValue };
                    nNewEndPoint = (int)doc.InvokeScript("textboxSetSelectionTextReturnEndPosition", oaParams);
                    
                    // zero return means it failed (e.g. the selected portion wasn't in the element thought)
                    if (nNewEndPoint > 0)
                    {
                        // now we have to update the string transfer with the new value
                        HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                        if (elem != null)
                            stringTransfer.SetValue(elem.InnerHtml);
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearSelection(StringTransfer stringTransfer)
        {
            System.Diagnostics.Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (Document != null)
            {
                HtmlDocument doc = Document;
                if (IsTextareaElement(stringTransfer.HtmlElementId))
                {
                    IHTMLDocument2 htmlDocument = doc.DomDocument as IHTMLDocument2;
                    if (htmlDocument != null)
                    {
                        IHTMLSelectionObject selection = htmlDocument.selection;
                        selection.empty();
                    }
                }
                else if (IsParagraphElement(stringTransfer.HtmlElementId))
                {
                    HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                    if (elem != null)
                        elem.InnerHtml = stringTransfer.ToString();
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
                string[] aVerseConversationIndices = strId.Split(_achDelim);
                System.Diagnostics.Debug.Assert(((aVerseConversationIndices.Length == 3) ||
                                                 (aVerseConversationIndices.Length == 4))
                                                &&
                                                ((aVerseConversationIndices[0] == ConsultNoteDataConverter.CstrTextAreaPrefix) ||
                                                 (aVerseConversationIndices[0] == ConsultNoteDataConverter.CstrParagraphPrefix) ||
                                                 (aVerseConversationIndices[0] == ConsultNoteDataConverter.CstrButtonPrefix)));

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

        public void ResetDocument()
        {
            //reset so we don't jump to a soon-to-be-non-existant (or wrong context) place
            StrIdToScrollTo = null;
            if (Document != null)
                Document.OpenNew(true);
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
                                                        TheSE.hiddenVersesToolStripMenuItem.Checked,
                                                        TheSE.viewOnlyOpenConversationsMenu.Checked);
            DocumentText = strHtml;
            LineNumberLink.Visible = true;
        }

        public override string PaneLabel()
        {
            return "Consultant Notes";
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.ConsultantNotes;
            return aCNsDC;
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
                                                   TheSE.hiddenVersesToolStripMenuItem.Checked,
                                                   TheSE.viewOnlyOpenConversationsMenu.Checked);
            DocumentText = strHtml;
            LineNumberLink.Visible = true;
        }

        public override string PaneLabel()
        {
            return "Coach Notes";
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.CoachNotes;
            return aCNsDC;
        }

        public void OnVerseLineJump(int nVerseIndex)
        {
            TheSE.FocusOnVerse(nVerseIndex, true, false);
        }
    }
}
