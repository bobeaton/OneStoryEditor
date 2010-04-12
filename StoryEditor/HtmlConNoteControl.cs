using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [ComVisible(true)]
    public partial class HtmlConNoteControl : WebBrowser
    {
        protected string _strIdToScrollTo = null;

        public HtmlConNoteControl()
        {
            InitializeComponent();
            ObjectForScripting = this;
        }

        public VersesData Verses
        {
            get; set;
        }

        public bool IsConsultantNotes { get; set; }

        public VerseData Verse(int nVerseIndex)
        {
            System.Diagnostics.Debug.Assert(Verses.Count > (nVerseIndex - 1));
            return (nVerseIndex == 0) ? Verses.FirstVerse : Verses[nVerseIndex - 1];
        }

        public ConsultNotesDataConverter DataConverter(int nVerseIndex)
        {
            VerseData verse = Verse(nVerseIndex);
            ConsultNotesDataConverter aCNsDC;
            if (IsConsultantNotes)
                aCNsDC = verse.ConsultantNotes;
            else
                aCNsDC = verse.CoachNotes;
            return aCNsDC;
        }

        public ConsultNoteDataConverter DataConverter(int nVerseIndex, int nConversationIndex)
        {
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            System.Diagnostics.Debug.Assert(aCNsDC.Count > nConversationIndex);
            return aCNsDC[nConversationIndex];
        }

        private readonly char[] _achDelim = new[] { '_' };

        public bool OnAddNote(int nVerseIndex)
        {
            ConsultNotesDataConverter aCNsDC = DataConverter(nVerseIndex);
            ConsultNoteDataConverter aCNDC = DoAddNote(null, aCNsDC);
            if (aCNDC != null)
                _strIdToScrollTo = String.Format("ta_{0}_{1}", nVerseIndex, aCNsDC.IndexOf(aCNDC));
            return true;
        }

        public bool OnClickDelete(string strId)
        {
            return true;
        }

        public bool OnClickHide(string strId, string strLabel)
        {
            return true;
        }

        public bool OnClickEndConversation(string strId, string strLabel)
        {
            return true;
        }
        
        public bool TextareaOnKeyUp(string strId, string strText)
        {
            string[] aVerseConversationIndices = strId.Split(_achDelim);
            System.Diagnostics.Debug.Assert(aVerseConversationIndices.Length == 2);
            try
            {
                int nVerseIndex = Convert.ToInt32(aVerseConversationIndices[0]);
                int nConversationIndex = Convert.ToInt32(aVerseConversationIndices[1]);
                ConsultNoteDataConverter aCNDC = DataConverter(nVerseIndex, nConversationIndex);
                System.Diagnostics.Debug.Assert(aCNDC != null);
                CommInstance aCI = aCNDC[aCNDC.Count - 1];
                System.Diagnostics.Debug.WriteLine(String.Format("Was: {0}, now: {1}",
                    aCI, strText));
                aCI.SetValue(strText);
            }
            catch
            {
                return false;
            }
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

            theSE.ReInitConsultNotesPane(aCNsDC);

            // return the conversation we just created
            return cndc;
        }

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            theSE = (StoryEditor)FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        "Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com");

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

                if (!theSE.theCurrentStory.ProjStage.IsEditAllowed(theSE.LoggedOnMember))
                    throw theSE.theCurrentStory.ProjStage.WrongMemberTypeEx;
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message));
                return false;
            }

            return true;
        }
    }
}
