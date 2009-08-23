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

        // more slots are needed if we only have one in the list OR if we have an even number and the mentee started the conversation
        protected bool MoreSlotsNeeded
        {
            get { return ((Count == 1) || ((Count % 2) == 0) && (this[0].Direction == MenteeDirection)); }
        }

        public string GetDirectionString(CommunicationDirections eDirection)
        {
            return eDirection.ToString().Substring(1);
        }

        public void MakeExtraSlots()
        {
            if ((Count % 2) == 1)
            {
                // make it the opposite of the one that's there
                CommunicationDirections cd = (this[0].Direction == MentorDirection)
                    ? MenteeDirection : MentorDirection;
                Add(new CommInstance(null, cd, null));
            }

            // if the conversation was started by the mentee, then they need to finish it
            if (((Count % 2) == 0) && (this[0].Direction == MenteeDirection))
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
            if (MoreSlotsNeeded)
                MakeExtraSlots();
        }

        public ConsultantNoteData(int nRound, CommunicationDirections eDirectionOfFirst)
        {
            RoundNum = nRound;
            Add(new CommInstance(null, eDirectionOfFirst, null));
            System.Diagnostics.Debug.Assert(MoreSlotsNeeded);
            MakeExtraSlots();
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
            if (MoreSlotsNeeded)
                MakeExtraSlots();
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eCoachToConsultant; }
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eConsultantToCoach; }
        }

        public CoachNoteData(int nRound, CommunicationDirections eDirectionOfFirst)
        {
            RoundNum = nRound;
            Add(new CommInstance(null, eDirectionOfFirst, null));
            System.Diagnostics.Debug.Assert(MoreSlotsNeeded);
            MakeExtraSlots();
        }

        public override string MentorLabel
        {
            get { return "coach:"; }
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

        public abstract ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound);

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

        public override ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound)
        {
            ConsultNoteDataConverter theNewCN = new ConsultantNoteData(nRound, ConsultNoteDataConverter.CommunicationDirections.eConsultantToCrafter);
            Insert(nIndex, theNewCN);
            return theNewCN;
        }

        public ConsultantNotesData()
        {
            CollectionElementName = "ConsultantNotes";
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

        public override ConsultNoteDataConverter InsertEmpty(int nIndex, int nRound)
        {
            // always add closest to the verse label
            ConsultNoteDataConverter theNewCN = new CoachNoteData(nRound, ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant);
            Insert(0, theNewCN);
            return theNewCN;
        }

        public CoachNotesData()
        {
            CollectionElementName = "CoachNotes";
        }
    }
}
