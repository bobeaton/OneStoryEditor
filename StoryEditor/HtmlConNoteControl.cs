using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract class HtmlConNoteControl : HtmlVerseControl
    {
        internal TextBox HeaderTextBox;
        internal string HeaderText;

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
                        aCNsDC.InsureExtraBox(dc, TheSE.theCurrentStory.ProjStage, 
                                TheSE.LoggedOnMember.MemberType);
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

        public override void ScrollToVerse(int nVerseIndex)
        {
            StrIdToScrollTo = VersesData.LineId(nVerseIndex);
            base.ScrollToVerse(nVerseIndex);
        }

        public void OnScroll()
        {
            if ((Document != null) && (Document.Body != null))
            {
                HtmlDocument doc = Document;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("doc.Body.ScrollTop: {0}", doc.Body.ScrollTop));
                foreach (HtmlElement elemLn in doc.GetElementsByTagName("td"))
                    if (!String.IsNullOrEmpty(elemLn.Id))   // so we only get "ln: n" values
                        System.Diagnostics.Debug.WriteLine(String.Format("id: {0}, or: {1}, sr: {2}, st: {3}",
                            elemLn.Id, elemLn.OffsetRectangle,
                            elemLn.ScrollRectangle,
                            elemLn.ScrollTop));
#endif

                HtmlElement elemLnPrev = null;
                foreach (HtmlElement elemLn in doc.GetElementsByTagName("td"))
                    if (!String.IsNullOrEmpty(elemLn.Id))
                    {
                        if (elemLn.OffsetRectangle.Top < doc.Body.ScrollTop)
                            elemLnPrev = elemLn;
                        else
                            break;
                    }

                if (elemLnPrev != null)
                    HeaderTextBox.Text = String.Format("{0} ({1})",
                                                       HeaderText, elemLnPrev.InnerText);
            }
        }

        public bool OnAddNote(int nVerseIndex, string strNote)
        {
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            ConsultNoteDataConverter aCNDC = DoAddNote(strNote, aCNsDC);
            if (aCNDC != null)
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, aCNsDC.IndexOf(aCNDC));
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
                    Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                    return OnClickHide(strId);

                if (res == DialogResult.Cancel)
                    return true;
            }

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

            theCNDC.Visible = (theCNDC.Visible) ? false : true;

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

            if (theCNDC.IsFinished)
            {
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
            }
            else
            {
                // just in case we need to have an open box now
                theCNsDC.InsureExtraBox(theCNDC, TheSE.theCurrentStory.ProjStage, 
                    TheSE.LoggedOnMember.MemberType);
            }

            if (theCNDC.IsEditable(TheSE.theCurrentStory.ProjStage, theCNDC.Count - 1, TheSE.LoggedOnMember,
                theCNDC[theCNDC.Count - 1]))
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, nConversationIndex);
            else
                StrIdToScrollTo = ConsultNoteDataConverter.TextareaReadonlyRowId(nVerseIndex, nConversationIndex,
                                                                                 theCNDC.Count - 1);

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
        
        public bool OnBibRefJump(string strBibRef)
        {
            TheSE.SetNetBibleVerse(strBibRef);
            return true;
        }

        public void CopyScriptureReference(string strId)
        {
            int nVerseIndex, nConversationIndex;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex))
                return;

            ConsultNoteDataConverter theCNDC = DataConverter(nVerseIndex, nConversationIndex);
            System.Diagnostics.Debug.Assert((theCNDC != null) && (theCNDC.Count > 0));
            CommInstance aCI = theCNDC[theCNDC.Count - 1];

            if (Document != null)
            {
                HtmlDocument doc = Document;
                HtmlElement elem = doc.GetElementById(strId);
                if (elem != null)
                {
                    System.Diagnostics.Debug.Assert(elem.InnerText == aCI.ToString());
                    elem.InnerText += TheSE.GetNetBibleScriptureReference;
                    aCI.SetValue(elem.InnerText);
                    elem.Focus();
                }
            }
        }

        public bool TextareaOnKeyUp(string strId, string strText)
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
            aCI.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;

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

            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex))
                return false;

            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return false;

            theCNsDC = DataConverter(nVerseIndex);
            System.Diagnostics.Debug.Assert((theCNsDC != null) && (theCNsDC.Count > nConversationIndex));
            theCNDC = theCNsDC[nConversationIndex];

            // this always leads to the document being modified
            theSE.Modified = true;
            
            return true;
        }

        public ConsultNoteDataConverter DoAddNote(string strNote,
            ConsultNotesDataConverter aCNsDC)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return null;

            // if we're not given anything to put in the box, at least put in the logged
            //  in member's initials and re
            if (String.IsNullOrEmpty(strNote) && (theSE.LoggedOnMember != null))
                strNote = StoryEditor.GetInitials(theSE.LoggedOnMember.Name) + ": Re: ";

            // if the coach tries to add a note in the consultant's pane, that should fail.
            // (but it's okay for a project facilitator to add one if they have a question
            //  for the consultant)
            if (!aCNsDC.CheckAddNotePrivilege(theSE, theSE.LoggedOnMember.MemberType))
                return null;

            StoryStageLogic.ProjectStages eCurState = theSE.theCurrentStory.ProjStage.ProjectStage;
            int round = 1;
            if (eCurState > StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant)
            {
                round = 2;
                if (eCurState > StoryStageLogic.ProjectStages.eProjFacOnlineReview2WithConsultant)
                    round = 3;
            }

            ConsultNoteDataConverter cndc =
                aCNsDC.Add(round, theSE.theCurrentStory.ProjStage, 
                    theSE.LoggedOnMember.MemberType, strNote);
            System.Diagnostics.Debug.Assert(cndc.Count == 1);

            LoadDocument();
            theSE.Modified = true;

            // return the conversation we just created
            return cndc;
        }
    }

    [ComVisible(true)]
    public class HtmlConsultantNotesControl : HtmlConNoteControl
    {
        public override void LoadDocument()
        {
            string strHtml = StoryData.ConsultantNotesHtml(TheSE.theCurrentStory.ProjStage,
                                                           TheSE.StoryProject.ProjSettings,
                                                           TheSE.LoggedOnMember,
                                                           TheSE.hiddenVersesToolStripMenuItem.Checked);
            DocumentText = strHtml;
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
    }

    [ComVisible(true)]
    public class HtmlCoachNotesControl : HtmlConNoteControl
    {
        public override void LoadDocument()
        {
            string strHtml = StoryData.CoachNotesHtml(TheSE.theCurrentStory.ProjStage,
                                                      TheSE.StoryProject.ProjSettings,
                                                      TheSE.LoggedOnMember,
                                                      TheSE.hiddenVersesToolStripMenuItem.Checked);
            DocumentText = strHtml;
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
