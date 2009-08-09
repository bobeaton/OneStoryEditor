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
        
        public string MentorGuid = null;
        public string MentorLabel = null;
        public StringTransfer MentorComment = null;

        public string MenteeGuid = null;
        public string MenteeLabel = null;
        public StringTransfer MenteeResponse = null;

        public Color CommentColor
        {
            get { return Color.Maroon; }
        }

        public Color ResponseColor
        {
            get { return Color.Blue; }
        }

        protected string InstanceElementName;
        protected string CommentElementName;
        protected string ResponseElementName;

        public bool HasData
        {
            get { return (MentorComment.HasData || MenteeResponse.HasData); }
        }

        public XElement GetXml
        {
            get
            {
                // must have guids if there's data
                System.Diagnostics.Debug.Assert(MentorComment.HasData && !String.IsNullOrEmpty(MentorGuid));
                System.Diagnostics.Debug.Assert(MenteeResponse.HasData && !String.IsNullOrEmpty(MenteeGuid));

                XElement eleNote = new XElement(StoryEditor.ns + InstanceElementName, new XAttribute("round", RoundNum));
                if (MentorComment.HasData)
                    eleNote.Add(new XElement(StoryEditor.ns + CommentElementName, new XAttribute("memberID", MentorGuid), MentorComment));
                if (MenteeResponse.HasData)
                    eleNote.Add(new XElement(StoryEditor.ns + ResponseElementName, new XAttribute("memberID", MenteeGuid), MenteeResponse));

                return eleNote;
            }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData(StoryProject.ConsultantNoteRow aCNRow)
        {
            RoundNum = aCNRow.round;
            InstanceElementName = "ConsultantNote";
            CommentElementName = "ConsultantComment";
            ResponseElementName = "CrafterResponse";

            StoryProject.ConsultantCommentRow[] aCCRows = aCNRow.GetConsultantCommentRows();
            if (aCCRows.Length == 1)
            {
                StoryProject.ConsultantCommentRow theCCRow = aCCRows[0];
                MentorGuid = theCCRow.memberID;
                MentorLabel = "con:";
                MentorComment = new StringTransfer(theCCRow.ConsultantComment_text);
            }

            StoryProject.CrafterResponseRow[] aCRRows = aCNRow.GetCrafterResponseRows();
            if (aCRRows.Length == 1)
            {
                StoryProject.CrafterResponseRow theCRRow = aCRRows[0];
                MenteeGuid = theCRRow.memberID;
                MenteeLabel = "res:";
                MenteeResponse = new StringTransfer(theCRRow.CrafterResponse_text);
            }
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
            InstanceElementName = "CoachNote";
            CommentElementName = "CoachComment";
            ResponseElementName = "ConsultantResponse";

            StoryProject.CoachCommentRow[] aCoCRows = aCoNRow.GetCoachCommentRows();
            if (aCoCRows.Length == 1)
            {
                StoryProject.CoachCommentRow theCoCRow = aCoCRows[0];
                MentorGuid = theCoCRow.memberID;
                MentorLabel = "co:";
                MentorComment = new StringTransfer(theCoCRow.CoachComment_text);
            }

            StoryProject.ConsultantResponseRow[] aCRRows = aCoNRow.GetConsultantResponseRows();
            if (aCRRows.Length == 1)
            {
                StoryProject.ConsultantResponseRow theCRRow = aCRRows[0];
                MenteeGuid = theCRRow.memberID;
                MenteeLabel = "con:";
                MenteeResponse = new StringTransfer(theCRRow.ConsultantResponse_text);
            }
        }
    }

    public abstract class ConsultNotesDataConverter : List<ConsultNoteDataConverter>
    {
        protected string CollectionElementName = null;

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemCNDC = new XElement(StoryEditor.ns + CollectionElementName);
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

        public CoachNotesData()
        {
            CollectionElementName = "CoachNotes";
        }
    }
}
