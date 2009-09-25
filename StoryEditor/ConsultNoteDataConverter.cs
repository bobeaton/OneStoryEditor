using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public class CommInstance : StringTransfer
    {
        public ConsultNoteDataConverter.CommunicationDirections Direction;
        public string Guid;

        public CommInstance(string strValue, ConsultNoteDataConverter.CommunicationDirections direction, string strGuid)
            : base(strValue)
        {
            Direction = direction;
            Guid = strGuid ?? System.Guid.NewGuid().ToString();
        }
    }

    public abstract class ConsultNoteDataConverter : List<CommInstance>
    {
        public enum CommunicationDirections
        {
            eConsultantToCrafter,
            eCrafterToConsultant,
            eConsultantToCoach,
            eCoachToConsultant
        }

        protected static Dictionary<string, CommunicationDirections> CmapDirectionStringToEnumType = new Dictionary<string, CommunicationDirections>()
        {
            { "ConsultantToCrafter", CommunicationDirections.eConsultantToCrafter },
            { "CrafterToConsultant", CommunicationDirections.eCrafterToConsultant },
            { "ConsultantToCoach", CommunicationDirections.eConsultantToCoach },
            { "CoachToConsultant", CommunicationDirections.eCoachToConsultant }
        };

        protected CommunicationDirections GetDirectionFromString(string strDirectionString)
        {
            System.Diagnostics.Debug.Assert(CmapDirectionStringToEnumType.ContainsKey(strDirectionString));
            return CmapDirectionStringToEnumType[strDirectionString];
        }

        public string GetDirectionString(CommunicationDirections eDirection)
        {
            return eDirection.ToString().Substring(1);
        }

        public void InsureExtraBox(bool bIsMentorLoggedIn)
        {
            // in case the user re-logs in, we might have extra boxes here. So remove any null ones before
            //  "insuring" the one(s) we need
            if (Count > 1)
                while (!this[Count - 1].HasData)
                    RemoveAt(Count - 1);

            if (bIsMentorLoggedIn && ((Count == 0) || (this[Count - 1].Direction != MentorDirection)))
                Add(new CommInstance(null, MentorDirection, null));
            else if (!bIsMentorLoggedIn && (this[Count - 1].Direction != MenteeDirection))
                Add(new CommInstance(null, MenteeDirection, null));
        }

        public int RoundNum = 0;
        public bool Visible = true;

        public abstract CommunicationDirections MenteeDirection { get; }
        public abstract CommunicationDirections MentorDirection { get; }

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

        protected abstract string InstanceElementName   // *Conversation
        {
            get;
        }

        protected abstract string SubElementName   // *Note
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
                return (Count > 0);
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);

                XElement eleNote = new XElement(InstanceElementName, new XAttribute("round", RoundNum));
                if (!Visible)
                    eleNote.Add(new XAttribute("visible", "false"));

                foreach (CommInstance aCI in this)
                    if (aCI.HasData)
                        eleNote.Add(new XElement(SubElementName, 
                            new XAttribute("Direction", GetDirectionString(aCI.Direction)),
                            new XAttribute("guid", aCI.Guid),
                            aCI.ToString()));

                return eleNote;
            }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData(StoryProject.ConsultantConversationRow aConRow)
        {
            RoundNum = aConRow.round;
            if (!aConRow.IsvisibleNull())
                Visible = aConRow.visible;

            StoryProject.ConsultantNoteRow[] theNoteRows = aConRow.GetConsultantNoteRows();
            foreach (StoryProject.ConsultantNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.ConsultantNote_text, GetDirectionFromString(aNoteRow.Direction), aNoteRow.guid));

            // make sure that there are at least two (we can't save them if they're empty)
            System.Diagnostics.Debug.Assert(Count != 0);
        }

        public ConsultantNoteData(int nRound, bool bIsMentorLoggedIn)
        {
            RoundNum = nRound;
            InsureExtraBox(bIsMentorLoggedIn);
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eConsultantToCrafter; }
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eCrafterToConsultant; }
        }

        public override string MentorLabel
        {
            get { return "cons:"; }
        }

        public override string MenteeLabel
        {
            get { return "crftr:"; }
        }

        protected override string InstanceElementName
        {
            get { return "ConsultantConversation"; }
        }

        protected override string SubElementName
        {
            get { return "ConsultantNote"; }
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

    public class CoachNoteData : ConsultNoteDataConverter
    {
        public CoachNoteData(StoryProject.CoachConversationRow aCoaCRow)
        {
            RoundNum = aCoaCRow.round;
            if (!aCoaCRow.IsvisibleNull())
                Visible = aCoaCRow.visible;

            StoryProject.CoachNoteRow[] theNoteRows = aCoaCRow.GetCoachNoteRows();
            foreach (StoryProject.CoachNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.CoachNote_text, GetDirectionFromString(aNoteRow.Direction), aNoteRow.guid));

            // make sure that there are at least two (we can't save them if they're empty)
            System.Diagnostics.Debug.Assert(Count != 0);
        }

        public CoachNoteData(int nRound, bool bIsMentorLoggedIn)
        {
            RoundNum = nRound;
            InsureExtraBox(bIsMentorLoggedIn);
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eCoachToConsultant; }
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eConsultantToCoach; }
        }

        public override string MentorLabel
        {
            get { return "coch:"; }
        }

        public override string MenteeLabel
        {
            get { return "cons:"; }
        }

        protected override string InstanceElementName
        {
            get { return "CoachConversation"; }
        }

        protected override string SubElementName
        {
            get { return "CoachNote"; }
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

        public abstract ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound, bool bIsMentorLoggedIn);
        public abstract TeamMemberData.UserTypes MentorType { get; }

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

            foreach (StoryProject.ConsultantConversationRow aConsultantConversationRow in theConsultantNotesRow.GetConsultantConversationRows())
                Add(new ConsultantNoteData(aConsultantConversationRow));
        }

        public ConsultantNotesData()
        {
            CollectionElementName = "ConsultantNotes";
        }

        public override ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound, bool bIsMentorLoggedIn)
        {
            ConsultNoteDataConverter theNewCN = new ConsultantNoteData(nRound, bIsMentorLoggedIn);
            Insert(nIndex, theNewCN);
            return theNewCN;
        }

        internal static TeamMemberData.UserTypes myMentorType = TeamMemberData.UserTypes.eConsultantInTraining;
        public override TeamMemberData.UserTypes MentorType
        {
            get { return myMentorType; }
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

            foreach (StoryProject.CoachConversationRow aCoachConversationRow in theCoachNotesRow.GetCoachConversationRows())
                Add(new CoachNoteData(aCoachConversationRow));
        }

        public CoachNotesData()
        {
            CollectionElementName = "CoachNotes";
        }

        public override ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound, bool bIsMentorLoggedIn)
        {
            // always add closest to the verse label
            ConsultNoteDataConverter theNewCN = new CoachNoteData(nRound, bIsMentorLoggedIn);
            Insert(0, theNewCN);
            return theNewCN;
        }

        internal static TeamMemberData.UserTypes myMentorType = TeamMemberData.UserTypes.eCoach;
        public override TeamMemberData.UserTypes MentorType
        {
            get { return myMentorType; }
        }
    }
}
