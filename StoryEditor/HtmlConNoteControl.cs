using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public abstract class HtmlConNoteControl : HtmlVerseControl
    {
        protected HtmlConNoteControl()
        {
            ObjectForScripting = this;
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
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            StrIdToScrollTo = ConsultNoteDataConverter.TextareaId(nVerseIndex, aCNsDC.Count - 1);
            base.ScrollToVerse(nVerseIndex);
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
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out theCNsDC, out theCNDC))
                return false;

            if (theCNDC.HasData)
            {
                DialogResult res = MessageBox.Show(
                    Properties.Resources.IDS_NoteNotEmptyHideQuery,
                    Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    theCNDC.Visible = false;
                    LoadDocument();
                    return true;
                }

                if (res == DialogResult.Cancel)
                    return true;
            }

            theCNsDC.Remove(theCNDC);
            LoadDocument();
            return true;
        }

        public bool OnClickHide(string strId)
        {
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out theCNsDC, out theCNDC))
                return false;

            theCNDC.Visible = (theCNDC.Visible) ? false : true;

            LoadDocument();

            return true;
        }

        public bool OnClickEndConversation(string strId)
        {
            ConsultNotesDataConverter theCNsDC;
            ConsultNoteDataConverter theCNDC;
            if (!GetDataConverters(strId, out theCNsDC, out theCNDC))
                return false;

            if (theCNDC.IsFinished)
                theCNDC.IsFinished = false;
            else
                theCNDC.IsFinished = true;

            LoadDocument();
            return true;
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
            System.Diagnostics.Debug.WriteLine(String.Format("Was: {0}, now: {1}",
                aCI, strText));
            aCI.SetValue(strText);

            // indicate that the document has changed
            theSE.Modified = true;

            return true;
        }

        protected bool GetDataConverters(string strId,
            out ConsultNotesDataConverter theCNsDC, out ConsultNoteDataConverter theCNDC)
        {
            theCNsDC = null;
            theCNDC = null;

            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return false;

            int nVerseIndex, nConversationIndex;
            if (!GetIndicesFromId(strId, out nVerseIndex, out nConversationIndex))
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
                aCNsDC.Add(round, theSE.LoggedOnMember.MemberType, strNote);
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
            string strHtml = StoryData.ConsultantNotesHtml(TheSE.LoggedOnMember.MemberType,
                                                           TheSE.hiddenVersesToolStripMenuItem.Checked);
            DocumentText = strHtml;
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.ConsultantNotes;
            return aCNsDC;
        }
    }

    [ComVisible(true)]
    public class HtmlCoachNotesControl : HtmlConNoteControl
    {
        public override void LoadDocument()
        {
            string strHtml = StoryData.CoachNotesHtml(TheSE.LoggedOnMember.MemberType,
                                                      TheSE.hiddenVersesToolStripMenuItem.Checked);
            DocumentText = strHtml;
        }

        public override ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
            ConsultNotesDataConverter aCNsDC = verse.CoachNotes;
            return aCNsDC;
        }
    }
}
