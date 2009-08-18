using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public abstract class ConsultNoteDataConverter
    {
        public int RoundNum = 0;
        public bool Visible = true;

        public StringTransfer MentorComment = null;
        public StringTransfer MenteeResponse = null;

        public abstract string MentorLabel
        {
            get;
        }

        public abstract string MenteeLabel
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

        protected abstract string InstanceElementName
        {
            get;
        }

        protected abstract string CommentElementName
        {
            get;
        }

        protected abstract string ResponseElementName
        {
            get;
        }

        public abstract TeamMemberData.UserTypes MentorRequiredEditor
        {
            get;
        }

        public abstract TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get;
        }

        public bool HasData
        {
            get
            {
                System.Diagnostics.Debug.Assert((MentorComment != null) && (MenteeResponse != null));
                return (MentorComment.HasData || MenteeResponse.HasData);
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert((MentorComment != null) && (MenteeResponse != null) && HasData);

                XElement eleNote = new XElement(InstanceElementName, new XAttribute("round", RoundNum));
                if (!Visible)
                    eleNote.Add(new XAttribute("visible", "false"));

                if (MentorComment.HasData)
                    eleNote.Add(new XElement(CommentElementName, MentorComment));
                if (MenteeResponse.HasData)
                    eleNote.Add(new XElement(ResponseElementName, MenteeResponse));

                return eleNote;
            }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData(StoryProject.ConsultantNoteRow aCNRow)
        {
            RoundNum = aCNRow.round;
            if (!aCNRow.IsvisibleNull())
                Visible = aCNRow.visible;

            MentorComment = new StringTransfer((aCNRow.IsConsultantCommentNull()) ? null : aCNRow.ConsultantComment);
            MenteeResponse = new StringTransfer((aCNRow.IsCrafterResponseNull()) ? null : aCNRow.CrafterResponse);
        }

        public ConsultantNoteData(int nRound)
        {
            RoundNum = nRound;
            MentorComment = new StringTransfer(null);
            MenteeResponse = new StringTransfer(null);
        }

        public override string MentorLabel
        {
            get { return "con:"; }
        }

        public override string MenteeLabel
        {
            get { return "res:"; }
        }

        protected override string InstanceElementName
        {
            get { return "ConsultantNote"; }
        }

        protected override string CommentElementName
        {
            get { return "ConsultantComment"; }
        }

        protected override string ResponseElementName
        {
            get { return "CrafterResponse"; }
        }

        public override TeamMemberData.UserTypes MentorRequiredEditor
        {
            get { return TeamMemberData.UserTypes.eConsultantInTraining; }
        }

        public override TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get { return TeamMemberData.UserTypes.eCrafter; }
        }
    }

    public class ConsultantNotesData : ConsultNotesDataConverter
    {
        public ConsultantNotesData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            CollectionElementName = "ConsultantNotes";

            StoryProject.ConsultantNotesRow[] theConsultantNotesRows = theVerseRow.GetConsultantNotesRows();
            StoryProject.ConsultantNotesRow theConsultantNotesRow;
            if (theConsultantNotesRows.Length == 0)
                theConsultantNotesRow = projFile.ConsultantNotes.AddConsultantNotesRow(theVerseRow);
            else
                theConsultantNotesRow = theConsultantNotesRows[0];

            foreach (StoryProject.ConsultantNoteRow aConsultantNoteRow in theConsultantNotesRow.GetConsultantNoteRows())
                Add(new ConsultantNoteData(aConsultantNoteRow));
        }

        public override void AddEmpty(int nRound)
        {
            // always add closest to the verse label
            Insert(0, new ConsultantNoteData(nRound));
        }

        public ConsultantNotesData()
        {
            CollectionElementName = "ConsultantNotes";
        }
    }

    public class CoachNoteData : ConsultNoteDataConverter
    {
        public CoachNoteData(StoryProject.CoachNoteRow aCoNRow)
        {
            RoundNum = aCoNRow.round;
            if (!aCoNRow.IsvisibleNull())
                Visible = aCoNRow.visible;

            MentorComment = new StringTransfer((aCoNRow.IsCoachCommentNull()) ? null : aCoNRow.CoachComment);
            MenteeResponse = new StringTransfer((aCoNRow.IsConsultantResponseNull()) ? null : aCoNRow.ConsultantResponse);
        }

        public CoachNoteData(int nRound)
        {
            RoundNum = nRound;
            MentorComment = new StringTransfer(null);
            MenteeResponse = new StringTransfer(null);
        }

        public override string MentorLabel
        {
            get { return "co:"; }
        }

        public override string MenteeLabel
        {
            get { return "con:"; }
        }

        protected override string InstanceElementName
        {
            get { return "CoachNote"; }
        }

        protected override string CommentElementName
        {
            get { return "CoachComment"; }
        }

        protected override string ResponseElementName
        {
            get { return "ConsultantResponse"; }
        }

        public override TeamMemberData.UserTypes MentorRequiredEditor
        {
            get { return TeamMemberData.UserTypes.eCoach; }
        }

        public override TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get { return TeamMemberData.UserTypes.eConsultantInTraining; }
        }
    }

    public abstract class ConsultNotesDataConverter : List<ConsultNoteDataConverter>
    {
        protected string CollectionElementName = null;

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public abstract void AddEmpty(int nRound);

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemCNDC = new XElement(CollectionElementName);
                foreach (ConsultNoteDataConverter aCNDC in this)
                    if (aCNDC.HasData)
                        elemCNDC.Add(aCNDC.GetXml);
                return elemCNDC;
            }
        }
    }

    public class CoachNotesData : ConsultNotesDataConverter
    {
        public CoachNotesData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            CollectionElementName = "CoachNotes";

            StoryProject.CoachNotesRow[] theCoachNotesRows = theVerseRow.GetCoachNotesRows();
            StoryProject.CoachNotesRow theCoachNotesRow;
            if (theCoachNotesRows.Length == 0)
                theCoachNotesRow = projFile.CoachNotes.AddCoachNotesRow(theVerseRow);
            else
                theCoachNotesRow = theCoachNotesRows[0];

            foreach (StoryProject.CoachNoteRow aCoachNoteRow in theCoachNotesRow.GetCoachNoteRows())
                Add(new CoachNoteData(aCoachNoteRow));
        }

        public override void AddEmpty(int nRound)
        {
            // always add closest to the verse label
            Insert(0, new CoachNoteData(nRound));
        }

        public CoachNotesData()
        {
            CollectionElementName = "CoachNotes";
        }
    }
}
