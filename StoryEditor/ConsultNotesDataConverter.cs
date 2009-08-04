using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class ConsultNotesDataConverter
    {
        public abstract List<ConsultNoteDataConverter> ConsultNotes
        {
            get;
        }

        public abstract bool IsConsultNotes
        {
            get;
        }
    }

    public class ConsultantNotesData : ConsultNotesDataConverter
    {
        protected StoryProject.ConsultantNotesRow m_aCNsRow = null;

        public ConsultantNotesData(StoryProject.ConsultantNotesRow[] aCNsRows)
        {
            if ((aCNsRows != null) && (aCNsRows.Length > 0))
                m_aCNsRow = aCNsRows[0];
        }


        public override List<ConsultNoteDataConverter> ConsultNotes
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsConsultNotes);
                StoryProject.ConsultantNoteRow[] aCNRows = m_aCNsRow.GetConsultantNoteRows();
                System.Diagnostics.Debug.Assert(aCNRows.Length > 0);
                List<ConsultNoteDataConverter> lst = new List<ConsultNoteDataConverter>(aCNRows.Length);
                foreach (StoryProject.ConsultantNoteRow aCNRow in aCNRows)
                    lst.Add(new ConsultantNoteData(aCNRow));
                return lst;
            }
        }

        public override bool IsConsultNotes
        {
            get { return (m_aCNsRow != null); }
        }
    }

    public class CoachNotesData : ConsultNotesDataConverter
    {
        protected StoryProject.CoachNotesRow m_aCoNsRow = null;

        public CoachNotesData(StoryProject.CoachNotesRow[] aCoNsRows)
        {
            if ((aCoNsRows != null) && (aCoNsRows.Length > 0))
                m_aCoNsRow = aCoNsRows[0];
        }

        public override List<ConsultNoteDataConverter> ConsultNotes
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsConsultNotes);
                StoryProject.CoachNoteRow[] aCoNRows = m_aCoNsRow.GetCoachNoteRows();
                System.Diagnostics.Debug.Assert(aCoNRows.Length > 0);
                List<ConsultNoteDataConverter> lst = new List<ConsultNoteDataConverter>(aCoNRows.Length);
                foreach (StoryProject.CoachNoteRow aCoNRow in aCoNRows)
                    lst.Add(new CoachNoteData(aCoNRow));
                return lst;
            }
        }

        public override bool IsConsultNotes
        {
            get { return (m_aCoNsRow != null); }
        }
    }
}
