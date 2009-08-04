using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public abstract class ConsultNoteDataConverter
    {
        public abstract int RoundNum
        {
            get;
        }

        public abstract string MentorLabel
        {
            get;
        }

        public abstract string MentorComment
        {
            get;
        }

        public abstract string MenteeLabel
        {
            get;
        }

        public abstract string MenteeResponse
        {
            get;
        }

        public Color CommentColor
        {
            get { return Color.Maroon; }
        }

        public Color ResponseColor
        {
            get { return Color.Blue; }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        protected StoryProject.ConsultantNoteRow m_aCNRow = null;

        public ConsultantNoteData(StoryProject.ConsultantNoteRow aCNRow)
        {
            m_aCNRow = aCNRow;
        }

        public override int RoundNum
        {
            get { return (int)m_aCNRow.round; }
        }

        public override string MentorLabel
        {
            get { return "con:"; }
        }

        public override string MentorComment
        {
            get 
            {
                StoryProject.ConsultantCommentRow[] aCCRows = m_aCNRow.GetConsultantCommentRows();
                System.Diagnostics.Debug.Assert(aCCRows.Length > 0);
                StoryProject.ConsultantCommentRow aCCRow = aCCRows[0];
                return aCCRow.ConsultantComment_text;
            }
        }

        public override string MenteeLabel
        {
            get { return "res:"; }
        }

        public override string MenteeResponse
        {
            get
            {
                StoryProject.CrafterResponseRow[] aCRRows = m_aCNRow.GetCrafterResponseRows();
                System.Diagnostics.Debug.Assert(aCRRows.Length > 0);
                StoryProject.CrafterResponseRow aCRRow = aCRRows[0];
                return aCRRow.CrafterResponse_text;
            }
        }
    }

    public class CoachNoteData : ConsultNoteDataConverter
    {
        protected StoryProject.CoachNoteRow m_aCoNRow = null;

        public CoachNoteData(StoryProject.CoachNoteRow aCoNRow)
        {
            m_aCoNRow = aCoNRow;
        }

        public override int RoundNum
        {
            get { return m_aCoNRow.round; }
        }

        public override string MentorLabel
        {
            get { return "co:"; }
        }

        public override string MentorComment
        {
            get
            {
                StoryProject.CoachCommentRow[] aCoCRows = m_aCoNRow.GetCoachCommentRows();
                System.Diagnostics.Debug.Assert(aCoCRows.Length > 0);
                StoryProject.CoachCommentRow aCoCRow = aCoCRows[0];
                return aCoCRow.CoachComment_text;
            }
        }

        public override string MenteeLabel
        {
            get { return "con:"; }
        }

        public override string MenteeResponse
        {
            get
            {
                StoryProject.ConsultantResponseRow[] aCRRows = m_aCoNRow.GetConsultantResponseRows();
                System.Diagnostics.Debug.Assert(aCRRows.Length > 0);
                StoryProject.ConsultantResponseRow aCRRow = aCRRows[0];
                return aCRRow.ConsultantResponse_text;
            }
        }
    }
}
