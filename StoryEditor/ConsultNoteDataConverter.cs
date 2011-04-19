using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Text;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public class CommInstance : StringTransfer
    {
        public ConsultNoteDataConverter.CommunicationDirections Direction;
        public string Guid;
        public DateTime TimeStamp;
        public string MemberID;

        public CommInstance(string strValue, 
            ConsultNoteDataConverter.CommunicationDirections direction, 
            string strGuid, string strMemberId, DateTime timeStamp)
            : base(strValue)
        {
            Direction = direction;
            Guid = strGuid ?? System.Guid.NewGuid().ToString();
            MemberID = strMemberId;
            TimeStamp = timeStamp;
        }

        public CommInstance(CommInstance rhs)
            : base(rhs.ToString())
        {
            Direction = rhs.Direction;

            // the guid shouldn't be replicated
            Guid = System.Guid.NewGuid().ToString();  // rhs.Guid;
            MemberID = rhs.MemberID;
            TimeStamp = rhs.TimeStamp;
        }

        public TeamMemberData.UserTypes InitiatorType
        {
            get
            {
                if (Direction == ConsultNoteDataConverter.CommunicationDirections.eProjFacToConsultant)
                    return TeamMemberData.UserTypes.ProjectFacilitator;

                if ((Direction == ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant)
                    || (Direction == ConsultNoteDataConverter.CommunicationDirections.eCoachToCoach))
                    return TeamMemberData.UserTypes.Coach;

                return (TeamMemberData.UserTypes.ConsultantInTraining |
                        TeamMemberData.UserTypes.IndependentConsultant |
                        TeamMemberData.UserTypes.FirstPassMentor);
            }
        }

        public const string CstrAttributeLabelDirection = "Direction";
        public const string CstrAttributeLabelGuid = "guid";
        public const string CstrAttributeLabelMemberId = "memberID";
        public const string CstrAttributeLabelTimeStamp = "timeStamp";

        public XElement GetXml(string strSubElementName)
        {
            var elem = new XElement(strSubElementName,
                                new XAttribute(CstrAttributeLabelDirection, GetDirectionString),
                                new XAttribute(CstrAttributeLabelGuid, Guid));

            if (!String.IsNullOrEmpty(MemberID))
                elem.Add(new XAttribute(CstrAttributeLabelMemberId, MemberID));

            elem.Add(new XAttribute(CstrAttributeLabelTimeStamp, TimeStamp.ToString("s")),
                     this.ToString());
            
            return elem;
        }

        public string GetDirectionString
        {
            get { return Direction.ToString().Substring(1); }
        }

        public TeamMemberData Commentor(TeamMembersData teamMembersData)
        {
            return teamMembersData.GetMemberFromId(MemberID);
        }
    }

    public abstract class ConsultNoteDataConverter : List<CommInstance>
    {
        public string guid;
        public bool Visible = true;
        public bool IsFinished;
        public bool AllowButtonsOverride;

        public enum CommunicationDirections
        {
            eConsultantToProjFac,
            eProjFacToConsultant,
            eConsultantToCoach,
            eCoachToConsultant,
            eConsultantToConsultant,    // consultant's note to self
            eCoachToCoach,              // coach's note to self
            eConsultantToProjFacNeedsApproval   // for LRC or CIT notes to PF (IC or Coach must approve before they become visible to PF)
        }

        protected ConsultNoteDataConverter()
        {
            guid = Guid.NewGuid().ToString();
            AllowButtonsOverride = true;    // so the person can delete in case it was a mistake
        }

        protected ConsultNoteDataConverter(string strGuid,
            bool bVisible, bool bIsFinished)
        {
            guid = strGuid;
            Visible = bVisible;
            IsFinished = bIsFinished;
        }

        protected ConsultNoteDataConverter(ConsultNoteDataConverter rhs)
        {
            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();   // rhs.guid;
            Visible = rhs.Visible;
            IsFinished = rhs.IsFinished;

            foreach (CommInstance aCI in rhs)
                Add(new CommInstance(aCI));
        }

        protected static Dictionary<string, CommunicationDirections> CmapDirectionStringToEnumType = new Dictionary<string, CommunicationDirections>()
        {
            { "ConsultantToProjFac", CommunicationDirections.eConsultantToProjFac },
            { "ProjFacToConsultant", CommunicationDirections.eProjFacToConsultant },
            { "ConsultantToCoach", CommunicationDirections.eConsultantToCoach },
            { "CoachToConsultant", CommunicationDirections.eCoachToConsultant },
            { "ConsultantToConsultant", CommunicationDirections.eConsultantToConsultant },
            { "CoachToCoach", CommunicationDirections.eCoachToCoach },
            { "ConsultantToProjFacNeedsApproval", CommunicationDirections.eConsultantToProjFacNeedsApproval }
        };

        protected CommunicationDirections GetDirectionFromString(string strDirectionString)
        {
            System.Diagnostics.Debug.Assert(CmapDirectionStringToEnumType.ContainsKey(strDirectionString));
            return CmapDirectionStringToEnumType[strDirectionString];
        }

        public void InsureExtraBox(StoryData theStory, TeamMemberData loggedOnMember,
            TeamMembersData theTeamMembers, string strValue)
        {
            // in case the user re-logs in, we might have extra boxes here. So remove any null ones before
            //  "insuring" the one(s) we need
            if (Count > 1)
                while (!FinalComment.HasData)
                    RemoveAt(Count - 1);

            // don't bother, though, if the user has ended the conversation
            if (IsFinished)// || !HasAddNotePrivilege loggedOnMember.IsEditAllowed(theStory))
                /*
                (!LoggedOnMentorHasResponsePrivilege(loggedOnMember, theStory) &&
                 !LoggedOnMentoreeHasResponsePrivilege(loggedOnMember, theStory)) &&
                 !loggedOnMember.IsEditAllowed(theStory)) 
                */
            {
                return;
            }

            if (MentorHasRespondPrivilege(loggedOnMember, theTeamMembers, theStory))
                Add(CreateMentorNote(loggedOnMember, theStory, strValue));
            else if (MentoreeHasRespondPrivilege(loggedOnMember, theStory))
                Add(new CommInstance(strValue, MenteeDirection, null, loggedOnMember.MemberGuid, DateTime.Now));
        }

        protected virtual CommInstance CreateMentorNote(TeamMemberData loggedOnMember,
            StoryData theStory, string strValue)
        {
            return new CommInstance(strValue, MentorDirection, null, loggedOnMember.MemberGuid, DateTime.Now);
        }

        protected virtual bool LoggedOnMentoreeHasResponsePrivilege(TeamMemberData loggedOnMember, 
            StoryData theStory)
        {
            return IsMenteeLoggedOn(loggedOnMember)
                   && loggedOnMember.IsEditAllowed(theStory);   // needed to prevent cons response box to a PF initiated comment while story is in PF state
        }

        protected virtual bool LoggedOnMentorHasResponsePrivilege(TeamMemberData loggedOnMember,
            TeamMembersData theTeamMembers, StoryData theStory)
        {
            return IsMentorLoggedOn(loggedOnMember)
                   && loggedOnMember.IsEditAllowed(theStory);   // needed to prevent cons response box to a PF initiated comment while story is in PF state
        }

        protected bool MentorHasRespondPrivilege(TeamMemberData loggedOnMember,
            TeamMembersData theTeamMembers, StoryData theStory)
        {
            // can add a box if: 
            //  a)  the mentor is logged on and is initiating a conversation or 
            //  b)  the last comment is from the mentoree and the logged on mentor
            //      has privilege to respond
            if (Count == 0)
                return IsMentorLoggedOn(loggedOnMember);
            
            // otherwise, the last comment has to be from the mentoree AND
            // either the conversation is just beginning or the same mentor has to have
            //  initiated the conversation AND
            // the logged on mentor must have the privilege to make a response.
            return (IsFromMentee(FinalComment) &&
                    ((Count < 2) || 
                        InitiatedConversation(loggedOnMember) || 
                        (InitiatedByMentoree && InitialFollowupBy(loggedOnMember))) &&
                    LoggedOnMentorHasResponsePrivilege(loggedOnMember, theTeamMembers, theStory));
        }

        protected bool MentoreeHasRespondPrivilege(TeamMemberData loggedOnMember,
            StoryData theStory)
        {
            // can add a box if: 
            //  a)  the mentoree is logged on and is initiating a conversation or 
            //  b)  the last comment is from the mentor and the logged on mentoree
            //      has privilege to respond
            if (Count == 0)
                return IsMenteeLoggedOn(loggedOnMember);

            return (IsFromMentor(FinalComment) &&
                    ((Count < 2) || 
                        InitiatedConversation(loggedOnMember) || 
                        (InitiatedByMentor && InitialFollowupBy(loggedOnMember))) &&
                    !NoteNeedsApproval &&
                    !IsNoteToSelf && 
                    LoggedOnMentoreeHasResponsePrivilege(loggedOnMember, theStory));
        }
        /*
        // do this here, because we need to sub-class it to allow for FirstPassMentor working as well in addition to CIT
        public virtual void ThrowIfWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            if (IsWrongEditor(eLoggedOnMember, eRequiredEditor))
                throw new ApplicationException(String.Format("Only a '{0}' can edit this field", TeamMemberData.GetMemberTypeAsDisplayString(eRequiredEditor)));
        }
        */
        protected bool IsMentorLoggedOn(TeamMemberData loggedOnMember)
        {
            return (TeamMemberData.IsUser(loggedOnMember.MemberType,
                                          MentorRequiredEditor));
        }

        protected bool IsMenteeLoggedOn(TeamMemberData loggedOnMember)
        {
            return (TeamMemberData.IsUser(loggedOnMember.MemberType,
                                          MenteeRequiredEditor));
        }

        protected bool IsWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            return !TeamMemberData.IsUser(eLoggedOnMember, eRequiredEditor);
        }

        protected CommInstance InitialComment
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);
                return this[0];
            }
        }

        public CommInstance FinalComment
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);
                return this[Count - 1];
            }
        }

        private bool InitialFollowupBy(TeamMemberData loggedOnMember)
        {
            System.Diagnostics.Debug.Assert(Count > 1);
            var theInitialFollowup = this[1];
            return (loggedOnMember.MemberGuid == theInitialFollowup.MemberID);
        }

        protected virtual bool InitiatedConversation(TeamMemberData loggedOnMember)
        {
            var theInitialComment = InitialComment;
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(theInitialComment.MemberID));
            return (loggedOnMember.MemberGuid == theInitialComment.MemberID);
        }

        protected TeamMemberData InitiatedConversation(TeamMembersData teamMembersData)
        {
            return InitialComment.Commentor(teamMembersData);
        }

        protected bool InitiatedByMentor
        {
            get
            {
                var aCi = InitialComment;
                return IsFromMentor(aCi);
            }
        }

        protected bool InitiatedByMentoree
        {
            get
            {
                var aCi = InitialComment;
                return IsFromMentee(aCi);
            }
        }

        public abstract CommunicationDirections MenteeDirection { get; }
        public abstract CommunicationDirections MentorDirection { get; }
        public abstract CommunicationDirections MentorToSelfDirection { get; }

        /// <summary>
        /// This returns whether the initiator of this comment is to be interpreted
        /// as a language specialty reviewer (so we can use a different label and/or
        /// color for those comments. It would be an LSR either if the member type
        /// doesn't include PF or if the PF of the story is a different PF.
        /// </summary>
        /// <param name="theMember">the TeamMemberData of the initiator of this comment</param>
        /// <param name="theStoryPfMemberId">the member id of the project facilitator for the story</param>
        /// <returns></returns>
        public static bool IsMemberAnLsrThatIsntAlsoTheStoryPf(TeamMemberData theMember,
            string theStoryPfMemberId)
        {
            return ((theMember != null)
                    && TeamMemberData.IsUser(theMember.MemberType,
                                             TeamMemberData.UserTypes.FirstPassMentor)
                    && (theMember.MemberGuid != theStoryPfMemberId));
        }

        public abstract string MentorLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId);

        public abstract string MenteeLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId);

        public Color CommentColor(TeamMemberData theInitiator,
            string theStoryPfMemberId)
        {
            return /* IsMemberAnLsrThatIsntAlsoTheStoryPf(theMember, theStoryPfMemberId)
                       ? Color.AntiqueWhite
                       : */ Color.AliceBlue;
        }

        public Color ResponseColor(TeamMemberData theInitiator,
            string theStoryPfMemberId)
        {
            return /* IsMemberAnLsrThatIsntAlsoTheStoryPf(theMember, theStoryPfMemberId)
                       ? Color.Coral
                       : */ Color.Cornsilk; 
        }

        public Color NoteToSelfColor
        {
            get { return Color.DodgerBlue; }
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

        protected abstract VerseData.ViewSettings.ItemToInsureOn AssociatedPane { get; }

        public bool HasData
        {
            get
            {
                if (Count > 0)
                    foreach (CommInstance aCI in this)
                        if (aCI.HasData)
                            return true;
                return false;
            }
        }

        public const string CstrAttributeLabelGuid = "guid";
        public const string CstrAttributeLabelVisible = "visible";
        public const string CstrAttributeLabelFinished = "finished";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);

                var eleNote = new XElement(InstanceElementName,
                    new XAttribute(CstrAttributeLabelGuid, guid));

                if (!Visible)
                    eleNote.Add(new XAttribute(CstrAttributeLabelVisible, false));

                if (IsFinished)
                    eleNote.Add(new XAttribute(CstrAttributeLabelFinished, true));

                foreach (CommInstance aCI in this)
                    if (aCI.IsSavable)
                        eleNote.Add(aCI.GetXml(SubElementName));

                return eleNote;
            }
        }

        public const string CstrTextAreaPrefix = "ta";
        public const string CstrParagraphPrefix = "tp";
        public const string CstrButtonPrefix = "btn";

        public static string TextareaId(int nVerseIndex, int nConversationIndex)
        {
            return String.Format("{0}_{1}_{2}", CstrTextAreaPrefix, nVerseIndex, nConversationIndex);
        }

        public static string TextParagraphId(int nVerseIndex, int nConversationIndex, int nCommentIndex)
        {
            return String.Format("{0}_{1}_{2}_{3}", CstrParagraphPrefix, nVerseIndex, nConversationIndex, nCommentIndex);
        }

        public static string TextareaRowId(int nVerseIndex, int nConversationIndex)
        {
            return String.Format("tarow_{0}_{1}", nVerseIndex, nConversationIndex);
        }

        public static string TextareaReadonlyRowId(int nVerseIndex, int nConversationIndex, int nCommentIndex)
        {
            return String.Format("tarorow_{0}_{1}_{2}", nVerseIndex, nConversationIndex, nCommentIndex);
        }

        public static string ButtonId(int nVerseIndex, int nConversationIndex, int nBtnIndex)
        {
            return String.Format("{0}_{1}_{2}_{3}", CstrButtonPrefix, nVerseIndex, nConversationIndex, nBtnIndex);
        }

        public const int CnBtnIndexDelete = 0;
        public const int CnBtnIndexHide = 1;
        public const int CnBtnIndexEndConversation = 2;
        public const int CnBtnIndexConvertToMentoreeNote = 3;
        public const int CnBtnIndexApproveNote = 4;

        public static string ButtonRowId(int nVerseIndex, int nConversationIndex)
        {
            return String.Format("btnrow_{0}_{1}", nVerseIndex, nConversationIndex);
        }

        public static string ConversationTableRowId(int nVerseIndex, int nConversationIndex)
        {
            return String.Format("convtblrow_{0}_{1}", nVerseIndex, nConversationIndex);
        }

        public const string CstrButtonLabelHide = "Hide";
        public const string CstrButtonLabelUnhide = "Unhide";
        public const string CstrButtonLabelConversationReopen = "Reopen conversation";
        public const string CstrButtonLabelConversationEnd = "End conversation";

        public bool IsEditable(TeamMemberData loggedOnMember, 
            TeamMembersData theTeamMembers, StoryData theStory)
        {
            var theFinalComment = FinalComment;
            return !IsFinished &&
                   (MentoreeAcceptable(loggedOnMember, theStory, theFinalComment) |
                    MentorAcceptable(loggedOnMember, theStory, theTeamMembers, theFinalComment));
        }

        /*
        public bool IsEditable(StoryStageLogic theStoryStage, int i, 
            TeamMemberData loggedOnMember, string theStoryPfMemberId, CommInstance aCi)
        {
            return (i == (Count - 1))
                   && !IsFinished
                   && ((IsMentorLoggedOn(loggedOnMember) && IsFromMentor(aCi))
                    || (IsMenteeLoggedOn(loggedOnMember) && IsFromMentee(aCi))
                    || loggedOnMember.IsEditAllowed(theStoryStage.MemberTypeWithEditToken)
                       && ((IsFromMentor(aCi) && !IsWrongEditor(loggedOnMember.MemberType, MentorRequiredEditor))
                           || (!IsFromMentor(aCi) && !IsWrongEditor(loggedOnMember.MemberType, MenteeRequiredEditor))));
        }
        */

        public bool HasAllMentoreeMemberIdData
        {
            get 
            {
                return this.All(aCi => 
                    !IsFromMentee(aCi) || 
                    !String.IsNullOrEmpty(aCi.MemberID)); 
            }
        }

        public bool HasAllMentorMemberIdData
        {
            get
            {
                return this.All(aCi =>
                    !IsFromMentor(aCi) || 
                    !String.IsNullOrEmpty(aCi.MemberID));
            }
        }

        public bool IsNoteToSelf
        {
            get
            {
                return ((Count == 1) &&
                        IsMyNoteToSelf(InitialComment));
            }
        }

        protected bool IsMyNoteToSelf(CommInstance aCi)
        {
            return (aCi.Direction == MentorToSelfDirection);
        }

        protected bool IsMyNoteToSelf(CommInstance aCi, TeamMemberData loggedOnMember)
        {
            return (IsMyNoteToSelf(aCi) &&
                    (aCi.MemberID == loggedOnMember.MemberGuid));
        }

        public bool IsFromMentor(CommInstance aCI)
        {
            // it is from the mentor if if it has the mentor's direction or is a note to self
            return ((aCI.Direction == MentorDirection) ||
                    IsMyNoteToSelf(aCI) ||
                    CommentNeedsApproval(aCI));
        }

        public bool IsFromMentee(CommInstance aCI)
        {
            // it is from the mentor if if it has the mentor's direction or is a note to self
            return (aCI.Direction == MenteeDirection);
        }

        readonly static Regex RegexBibRef = new Regex(@"\b(([a-zA-Z]{3,4}|[1-3][a-zA-Z]{2,5}) \d{1,3}:\d{1,3})\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly static Regex RegexLineRef = new Regex(@"\b((Ln|ln|line) ([1-9][0-9]?))\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly static Regex RegexItalics = new Regex(@"\*\b(.+?)\b\*", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly static Regex RegexHttpRef = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        public string Html(object htmlConNoteCtrl, 
            TeamMembersData theTeamMembers, TeamMemberData loggedOnMember, 
            StoryData theStory, int nVerseIndex, int nConversationIndex)
        {
            // don't show anything if
            //  a) there's nothing to show
            //  b) it's someone's note to self and the logged on member is not "self"
            //  c) it's a note in need of approval, but neither the person who initiated
            //      the note, nor the one with the ability to approve it, nor one with
            //      the ability to view such notes is logged on.
            if ((Count == 0) ||
                    (IsNoteToSelf && !InitiatedConversation(loggedOnMember)) ||
                    (NoteNeedsApproval &&
                        !InitiatedConversation(loggedOnMember) &&
                        !HasNoteApprovalAuthority(loggedOnMember, theTeamMembers) &&
                        !HasApprovalNeedingNoteViewingAuthority(loggedOnMember)))
            {
                return null;
            }

            string strHtml = HtmlGetButtonRow(loggedOnMember,
                                              theStory,
                                              theTeamMembers,
                                              nVerseIndex,
                                              nConversationIndex);

            string strColor, strHtmlTable = null;
            for (int i = 0; i < Count; i++)
            {
                CommInstance aCI = this[i];

                TeamMemberData theCommentor = aCI.Commentor(theTeamMembers);

                string strRow = null, theStoryPfMemberId = theStory.CraftingInfo.ProjectFacilitator.MemberId;
                Color clrRow;
                if (IsMyNoteToSelf(aCI))
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            MentorLabel(theCommentor,
                                                        theStoryPfMemberId));
                    clrRow = NoteToSelfColor;
                }
                else if (IsFromMentor(aCI))
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            MentorLabel(theCommentor,
                                                        theStoryPfMemberId));
                    clrRow = CommentColor(theCommentor, theStoryPfMemberId);
                }
                else
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            MenteeLabel(theCommentor,
                                                        theStoryPfMemberId));
                    clrRow = ResponseColor(theCommentor, theStoryPfMemberId);
                }

                strColor = VerseData.HtmlColor(clrRow);

                // there are two factors in deciding whether a conversation may be edited.
                //  1) the right person is logged in and
                //  2) the last comment box is for that person
                // Re: 1) the conversation is editable if either the conversation initiator 
                //  is logged in or a mentoree. For the ConsultantNote pane, this means the
                //  specific project facilitator associated with the story. For the Coach
                //  note pane, this could be any mentoree (PF, LSR, or CIT).
                // Re: 2) that depends on whether the ...
                string strHtmlElementId;
                if ((i == (Count - 1)) && 
                    IsEditable(loggedOnMember, theTeamMembers, theStory))
                {
                    strHtmlElementId = TextareaId(nVerseIndex, nConversationIndex);
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellForTextArea, "#FF0000",
                                            String.Format(OseResources.Properties.Resources.HTML_TextareaWithRefDrop,
                                                          strHtmlElementId,
                                                          StoryData.CstrLangInternationalBtStyleClassName,
                                                          aCI));

                    strHtmlTable += String.Format(OseResources.Properties.Resources.HTML_TableRowIdColor,
                                                  TextareaRowId(nVerseIndex, nConversationIndex),
                                                  strColor,
                                                  strRow);
                }
                else
                {
                    strHtmlElementId = TextParagraphId(nVerseIndex, nConversationIndex, i);
                    string strHyperlinkedText = null;
                    if (aCI.HasData)
                    {
                        strHyperlinkedText = aCI.ToString().Replace("\r\n", "<br />");   // regexParagraph.Replace(aCI.ToString(), ParagraphFound);
                        strHyperlinkedText = RegexBibRef.Replace(strHyperlinkedText, BibleReferenceFound);
                        strHyperlinkedText = RegexLineRef.Replace(strHyperlinkedText, LineReferenceFound);
                        strHyperlinkedText = RegexItalics.Replace(strHyperlinkedText, EmphasizedTextFound);
                        strHyperlinkedText = RegexHttpRef.Replace(strHyperlinkedText, HttpReferenceFound);
                    }

                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidth, 100,
                                            String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                          strHtmlElementId,
                                                          StoryData.CstrLangInternationalBtStyleClassName,
                                                          strHyperlinkedText));

                    strHtmlTable += String.Format(OseResources.Properties.Resources.HTML_TableRowIdColor,
                                                  TextareaReadonlyRowId(nVerseIndex, nConversationIndex, i),
                                                  strColor,
                                                  strRow);
                }

                // keep track of the element id so we can use it during 'Search/Replace' operations
                aCI.HtmlElementId = strHtmlElementId;
                aCI.HtmlConNoteCtrl = htmlConNoteCtrl;
            }

            string strEmbeddedTable = String.Format(OseResources.Properties.Resources.HTML_Table,
                                                    strHtmlTable);
            if (Visible)
                strColor = "#CCFFAA";
            else
                strColor = "#F0E68C";
            strHtml += String.Format(OseResources.Properties.Resources.HTML_TableRowIdColor,
                                     ConversationTableRowId(nVerseIndex, nConversationIndex),
                                     strColor,
                                     String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, 5,
                                                   strEmbeddedTable));

            return strHtml;
        }

        // the logged on person makes an exceptable editor of the final box, as a mentor if...
        private bool MentoreeAcceptable(TeamMemberData loggedOnMember, StoryData theStory,
            CommInstance aCiLast)
        {
            if (Count == 1)
                return InitiatedConversation(loggedOnMember);

            return LoggedOnMentoreeHasResponsePrivilege(loggedOnMember, theStory) &&
                   (InitiatedConversation(loggedOnMember) || InitiatedByMentor) &&
                   IsFromMentee(aCiLast);
        }

        // the logged on person makes an exceptable editor of the final box, as a mentor if...
        private bool MentorAcceptable(TeamMemberData loggedOnMember, StoryData theStory,
            TeamMembersData theTeamMembers, CommInstance aCiLast)
        {
            if (Count == 1)
                return InitiatedConversation(loggedOnMember);

            // the mentor is actually logged on
            // and that specific person actually initiated the conversation or it was 
            //  initiated by the mentoree
            // and the last comment visible is from a mentor
            return LoggedOnMentorHasResponsePrivilege(loggedOnMember, theTeamMembers, theStory) &&
                   (InitiatedConversation(loggedOnMember) || 
                        (InitiatedByMentoree && InitialFollowupBy(loggedOnMember))) &&
                   IsFromMentor(aCiLast);
        }

        // certain members won't get a regular turn (e.g. an LSR or a Coach when the 
        //  project is not specified as a manage with coaching situation). In these cases,
        //  just allow them to have their 'End', 'Hide', etc, buttons.
        private static bool OtherwiseDoesntHaveaTurn(TeamMemberData loggedOnMember, 
            StoryData theStory, TeamMembersData theTeamMembers)
        {
            return IsMemberAnLsrThatIsntAlsoTheStoryPf(loggedOnMember, theStory.CraftingInfo.ProjectFacilitator.MemberId) ||
                   CoachWithoutaTurn(loggedOnMember, theTeamMembers);
        }

        protected static bool CoachWithoutaTurn(TeamMemberData loggedOnMember, TeamMembersData theTeamMembers)
        {
            return theTeamMembers.HasIndependentConsultant &&
                   TeamMemberData.IsUser(loggedOnMember.MemberType,
                                         TeamMemberData.UserTypes.Coach);
        }

        private string HtmlGetButtonRow(TeamMemberData loggedOnMember, StoryData theStory,
            TeamMembersData theTeamMembers, int nVerseIndex, int nConversationIndex)
        {
            // only the initiator of a conversation gets the buttons to delete, hide or
            //  end conversation. But they can only do that when it's their turn.
            //  However, for an LSR, there's never an explicit turn, so allow them to do
            //  it anytime.
            string strRow = null;
            if (InitiatedConversation(loggedOnMember) && 
                (loggedOnMember.IsEditAllowed(theStory) ||
                    OtherwiseDoesntHaveaTurn(loggedOnMember, theStory, theTeamMembers) ||
                    AllowButtonsOverride))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_Button,
                                        ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexDelete),
                                        "return window.external.OnClickDelete(this.id);",
                                        "Delete");

                strRow += String.Format(OseResources.Properties.Resources.HTML_Button,
                                        ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexHide),
                                        "return window.external.OnClickHide(this.id);",
                                        (Visible) ? CstrButtonLabelHide : CstrButtonLabelUnhide);

                strRow += String.Format(OseResources.Properties.Resources.HTML_Button,
                                        ButtonId(nVerseIndex, nConversationIndex,
                                                 CnBtnIndexEndConversation),
                                        "return window.external.OnClickEndConversation(this.id);",
                                        (IsFinished)
                                            ? CstrButtonLabelConversationReopen
                                            : CstrButtonLabelConversationEnd);
            }

            // allow the person who created a "note to self" to convert it to a note to 
            //  the mentoree
            if (IsNoteToLoggedOnMemberSelf(loggedOnMember))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_Button,
                                        ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexConvertToMentoreeNote),
                                        String.Format("return window.external.OnConvertToMentoreeNote(this.id, {0});",
                                        (InitiatedByCit(theTeamMembers)) ? "true" : "false"),
                                        "Change to mentoree note");
            }

            // add a button if the logged on person has the authority to approve the note
            if (NoteNeedsApproval && !IsFinished)
            {
                if (HasNoteApprovalAuthority(loggedOnMember, theTeamMembers) && 
                    (loggedOnMember.IsEditAllowed(theStory) || 
                        CoachWithoutaTurn(loggedOnMember, theTeamMembers)))
                    strRow += String.Format(OseResources.Properties.Resources.HTML_Button,
                                            ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexApproveNote),
                                            "return window.external.OnApproveNote(this.id);",
                                            "Approve Note");
                else
                    strRow += "(Awaiting approval)";
            }

            if (!Visible)
                strRow += "(Hidden)";

            strRow = String.Format(OseResources.Properties.Resources.HTML_TableCell, strRow);

            // color changes if hidden
            string strColor = "#FFFFFF";    // default white background
            if (!Visible)
                strColor = "#F0E68C";

            return String.Format(OseResources.Properties.Resources.HTML_TableRowIdColor,
                                 ButtonRowId(nVerseIndex, nConversationIndex),
                                 strColor,
                                 strRow);
        }

        protected bool InitiatedByCit(TeamMembersData teamMembersData)
        {
            var theInitiator = InitiatedConversation(teamMembersData);
            return TeamMemberData.IsUser(theInitiator.MemberType,
                                         TeamMemberData.UserTypes.ConsultantInTraining |
                                         TeamMemberData.UserTypes.FirstPassMentor);
        }

        private static bool HasNoteApprovalAuthority(TeamMemberData loggedOnMember,
            TeamMembersData theTeamMembers)
        {
            return TeamMemberData.IsUser(loggedOnMember.MemberType,
                                         !theTeamMembers.HasIndependentConsultant
                                             ? TeamMemberData.UserTypes.Coach
                                             : TeamMemberData.UserTypes.Coach |
                                               TeamMemberData.UserTypes.IndependentConsultant);
        }

        private static bool HasApprovalNeedingNoteViewingAuthority(TeamMemberData loggedOnMember)
        {
            return TeamMemberData.IsUser(loggedOnMember.MemberType,
                                         TeamMemberData.UserTypes.FirstPassMentor |
                                         TeamMemberData.UserTypes.ConsultantInTraining);
        }

        public bool CommentNeedsApproval(CommInstance theCi)
        {
            return (theCi.Direction == CommunicationDirections.eConsultantToProjFacNeedsApproval);
        }

        public bool NoteNeedsApproval
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);
                return ((Count > 0)
                        && (FinalComment.Direction == CommunicationDirections.eConsultantToProjFacNeedsApproval));
            }
        }

        private bool IsNoteToLoggedOnMemberSelf(TeamMemberData loggedOnMember)
        {
            System.Diagnostics.Debug.Assert(Count > 0);
            return IsMyNoteToSelf(InitialComment, loggedOnMember);
        }

        static string BibleReferenceFound(Match m)
        {
            // Get the matched string.
            string str = String.Format(OseResources.Properties.Resources.HTML_LinkJumpTargetBibleReference, 
                m);
            return str;
        }

        static string LineReferenceFound(Match m)
        {
            // Get the matched string.
            string str = String.Format(OseResources.Properties.Resources.HTML_LinkJumpLine,
                                       m.Groups[3], m);
            return str;
        }

        static string HttpReferenceFound(Match m)
        {
            // Get the matched string.
            string str = String.Format(OseResources.Properties.Resources.HTML_HttpLink,
                                       m.Groups[1]);
            return str;
        }

        static string EmphasizedTextFound(Match m)
        {
            string str = String.Format(OseResources.Properties.Resources.HTML_EmphasizedText,
                m.Groups[1]);
            return str;
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (CommInstance aCI in this)
                lstBoxesToSearch.AddNewVerseString(aCI, AssociatedPane);
        }

        public void SetCommentMemberId(string strMentoree, string strMentor)
        {
            foreach (var aCi in this.Where(aCi => String.IsNullOrEmpty(aCi.MemberID)))
            {
                aCi.MemberID = (IsFromMentee(aCi))
                                   ? strMentoree
                                   : strMentor;
            }
        }

        public void UpdateCommentMemberId(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var aComment in this.Where(aComment => 
                aComment.MemberID == strOldMemberGuid))
            {
                aComment.MemberID = strNewMemberGuid;
            }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData(NewDataSet.ConsultantConversationRow aConRow)
            : base (aConRow.guid, 
            (aConRow.IsvisibleNull()) ? true : aConRow.visible,
            (aConRow.IsfinishedNull()) ? false : aConRow.finished)
        {
            NewDataSet.ConsultantNoteRow[] theNoteRows = aConRow.GetConsultantNoteRows();
            foreach (NewDataSet.ConsultantNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.ConsultantNote_text,
                                     GetDirectionFromString(aNoteRow.Direction),
                                     aNoteRow.guid,
                                     (aNoteRow.IsmemberIDNull())
                                         ? null
                                         : aNoteRow.memberID,
                                     (aNoteRow.IstimeStampNull())
                                         ? DateTime.Now
                                         : aNoteRow.timeStamp));

            // make sure that there are at least two (we can't save them if they're empty)
            System.Diagnostics.Debug.Assert(Count != 0, "It looks like you have an empty Consultant Note field that shouldn't be there. For now, you can just 'Ignore' this error (but perhaps let bob_eaton@sall.com know)");
        }

        public ConsultantNoteData(StoryData theStory,
            TeamMemberData loggedOnMember, TeamMembersData theTeamMembers, string strValue)
        {
            InsureExtraBox(theStory, loggedOnMember, theTeamMembers, strValue);
        }

        public ConsultantNoteData(ConsultNoteDataConverter rhs)
            : base(rhs)
        {
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eConsultantToProjFac; }
        }

        public override CommunicationDirections MentorToSelfDirection
        {
            get { return CommunicationDirections.eConsultantToConsultant; }
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eProjFacToConsultant; }
        }

        public override string MentorLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId)
        {
            return TeamMemberData.IsUser(theCommentor.MemberType,
                                         TeamMemberData.UserTypes.ConsultantInTraining)
                       ? "cit:"
                       : IsMemberAnLsrThatIsntAlsoTheStoryPf(theCommentor, theStoryPfMemberId)
                             ? "lsr:" // language specialty reviewer
                             : "con:";
        }

        public override string MenteeLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId)
        {
            return IsMemberAnLsrThatIsntAlsoTheStoryPf(theCommentor, theStoryPfMemberId)
                       ? "lsr:" // language specialty reviewer
                       : "prf:";
        }

        protected override string InstanceElementName
        {
            get { return "ConsultantConversation"; }
        }

        protected override string SubElementName
        {
            get { return "ConsultantNote"; }
        }

        /*
        protected override bool IsWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            // if it's the *mentor* that we're supposed to be checking for... (this will get called for the mentee check
            //  as well, but the special case is only for FirstPassMentor; not ProjFac)
            if (eRequiredEditor == MentorRequiredEditor)
            {
                // ... and if the logged in member isn't that mentor, nor the First Pass 
                //  Mentor, nor an independent consultant... or an English BTr
                if (TeamMemberData.IsUser(eLoggedOnMember, 
                    eRequiredEditor | 
                    TeamMemberData.UserTypes.FirstPassMentor | 
                    TeamMemberData.UserTypes.IndependentConsultant | 
                    TeamMemberData.UserTypes.EnglishBackTranslator))
                {
                    return false;
                }
            }
            
            // otherwise, let the base class handle it
            return base.IsWrongEditor(eLoggedOnMember, eRequiredEditor);
        }
        
        protected override bool CanDoConversationButtons(TeamMemberData loggedOnMember, CommInstance theCommInstance)
        {
            TeamMemberData.UserTypes eRequiredEditor = theCommInstance.InitiatorType;
            TeamMemberData.UserTypes eLoggedOnMember = loggedOnMember.MemberType;

            // if it's the *mentor* that we're supposed to be checking for... (this will get called for the mentee check
            //  as well, but the special case is only for FirstPassMentor; not ProjFac)
            if (eRequiredEditor == MentorRequiredEditor)
            {
                // ... and if the logged in member isn't that mentor, nor the First Pass 
                //  Mentor, nor an independent consultant... or an English BTr
                if (TeamMemberData.IsUser(eLoggedOnMember,
                                          eRequiredEditor |
                                          TeamMemberData.UserTypes.FirstPassMentor |
                                          TeamMemberData.UserTypes.IndependentConsultant))
                {
                    return true;
                }
            }

            // otherwise, let the base class handle it
            // I really want this to be base.IsWrongEditorbase (we don't want to call back to
            //  *our* version of IsWro...)
            return !base.IsWrongEditor(eLoggedOnMember, eRequiredEditor) || AllowButtonsOverride;
        }

        // override to allow for FirstPassMentor working as well in addition to CIT
        //  (or an independent consultant)
        public override void ThrowIfWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            // if it's the *mentor* that we're supposed to be checking for... (this will get called for the mentee check
            //  as well, but the special case is only for FirstPassMentor; not ProjFac)
            if (IsWrongEditor(eRequiredEditor, eRequiredEditor))
            {
                // then throw that it's the wrong editor
                throw new ApplicationException(String.Format("Only a '{0}', '{1}', or a '{2}' can edit this field",
                                                             TeamMemberData.GetMemberTypeAsDisplayString(
                                                                 eRequiredEditor),
                                                             TeamMemberData.GetMemberTypeAsDisplayString(
                                                                 TeamMemberData.UserTypes.eFirstPassMentor),
                                                             TeamMemberData.GetMemberTypeAsDisplayString(
                                                                 TeamMemberData.UserTypes.eIndependentConsultant)));
            }
        }
        */
        public override TeamMemberData.UserTypes MentorRequiredEditor
        {
            get
            {
                return TeamMemberData.UserTypes.ConsultantInTraining |
                       TeamMemberData.UserTypes.FirstPassMentor |
                       TeamMemberData.UserTypes.IndependentConsultant;
            }
        }

        public override TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get { return TeamMemberData.UserTypes.ProjectFacilitator; }
        }

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedPane
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields; }
        }

        protected override CommInstance CreateMentorNote(TeamMemberData loggedOnMember, 
            StoryData theStory, string strValue)
        {
            // override in order to add a comment that needs to be approved
            // a comment needs approval in two cases:
            //   1) The member is a CIT and (s)he is required to send the story back to
            //      the coach anyway (so, to allow the CIT to *not* have to get approval 
            //      for some minor comment (e.g. "add this TQ and send it to the PF for 
            //      a test"), then the coach must uncheck the 'Set to Coach's turn' 
            //      requirement).
            //   2) The member is an LSR. Such comments always need to be approved.
            bool bNeedsApproval =
                (TeamMemberData.IsUser(loggedOnMember.MemberType,
                                       TeamMemberData.UserTypes.ConsultantInTraining) &&
                 TasksCit.IsTaskOn(theStory.TasksRequiredCit,
                                   TasksCit.TaskSettings.SendToCoachForReview)) ||
                TeamMemberData.IsUser(loggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.FirstPassMentor);
            return (bNeedsApproval)
                       ? new CommInstance(strValue, CommunicationDirections.eConsultantToProjFacNeedsApproval, null,
                                          loggedOnMember.MemberGuid, DateTime.Now)
                       : base.CreateMentorNote(loggedOnMember, theStory, strValue);
        }

        protected override bool LoggedOnMentorHasResponsePrivilege(TeamMemberData loggedOnMember,
            TeamMembersData theTeamMembers, StoryData theStory)
        {
            return base.LoggedOnMentorHasResponsePrivilege(loggedOnMember, theTeamMembers, theStory) ||
                   IsMemberAnLsrThatIsntAlsoTheStoryPf(loggedOnMember, theStory.CraftingInfo.ProjectFacilitator.MemberId);
        }
    }

    public class CoachNoteData : ConsultNoteDataConverter
    {
        public CoachNoteData(NewDataSet.CoachConversationRow aCoaCRow)
            : base (aCoaCRow.guid, 
            (aCoaCRow.IsvisibleNull()) ? true : aCoaCRow.visible,
            (aCoaCRow.IsfinishedNull()) ? false : aCoaCRow.finished)
        {
            NewDataSet.CoachNoteRow[] theNoteRows = aCoaCRow.GetCoachNoteRows();
            foreach (NewDataSet.CoachNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.CoachNote_text,
                                     GetDirectionFromString(aNoteRow.Direction),
                                     aNoteRow.guid,
                                     (aNoteRow.IsmemberIDNull())
                                         ? null
                                         : aNoteRow.memberID,
                                     (aNoteRow.IstimeStampNull())
                                         ? DateTime.Now
                                         : aNoteRow.timeStamp));
        }

        public CoachNoteData(StoryData theStory,
            TeamMemberData loggedOnMember, TeamMembersData theTeamMembers, string strValue)
        {
            InsureExtraBox(theStory, loggedOnMember, theTeamMembers, strValue);
        }

        public CoachNoteData(ConsultNoteDataConverter rhs)
            : base(rhs)
        {
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eCoachToConsultant; }
        }

        public override CommunicationDirections MentorToSelfDirection
        {
            get { return CommunicationDirections.eCoachToCoach; }
        }

        protected override bool LoggedOnMentoreeHasResponsePrivilege(TeamMemberData loggedOnMember, StoryData theStory)
        {
            return base.LoggedOnMentoreeHasResponsePrivilege(loggedOnMember, theStory) |
                   IsMemberAnLsrThatIsntAlsoTheStoryPf(loggedOnMember,
                                 theStory.CraftingInfo.ProjectFacilitator.MemberId);
        }

        protected override bool LoggedOnMentorHasResponsePrivilege(TeamMemberData loggedOnMember, 
            TeamMembersData theTeamMembers, StoryData theStory)
        {
            // if there's a coach, but he won't otherwise get a turn (since the project
            //  is not otherwise configured to be a manage with coaching situation)
            return base.LoggedOnMentorHasResponsePrivilege(loggedOnMember, theTeamMembers, theStory) ||
                   CoachWithoutaTurn(loggedOnMember, theTeamMembers);
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eConsultantToCoach; }
        }

        public override string MentorLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId)
        {
            return "cch:";
        }

        public override string MenteeLabel(TeamMemberData theCommentor,
            string theStoryPfMemberId)
        {
            return TeamMemberData.IsUser(theCommentor.MemberType,
                                         TeamMemberData.UserTypes.ConsultantInTraining)
                       ? "cit:"
                       : IsMemberAnLsrThatIsntAlsoTheStoryPf(theCommentor, theStoryPfMemberId)
                             ? "lsr:" // language specialty reviewer
                             : "con:";
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
            get { return TeamMemberData.UserTypes.Coach; }
        }

        public override TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get
            {
                return TeamMemberData.UserTypes.ConsultantInTraining |
                       TeamMemberData.UserTypes.FirstPassMentor;
            }
        }

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedPane
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields; }
        }
    }

    public abstract class ConsultNotesDataConverter : List<ConsultNoteDataConverter>
    {
        public bool ShowOpenConversations;

        protected string CollectionElementName;

        protected ConsultNotesDataConverter(string strCollectionElementName)
        {
            CollectionElementName = strCollectionElementName;
        }

        public bool HasData
        {
            get { return (Count > 0); }
        }

        public bool HasAllMentoreeMemberIdData
        {
            get { return this.All(aCndc => aCndc.HasAllMentoreeMemberIdData); }
        }

        public bool HasAllMentorMemberIdData
        {
            get { return this.All(aCndc => aCndc.HasAllMentorMemberIdData); }
        }

        public void InsureExtraBox(ConsultNoteDataConverter aCNDC,
            StoryData theStory, TeamMemberData LoggedOnMemberType, TeamMembersData theTeamMembers)
        {
            aCNDC.InsureExtraBox(theStory, LoggedOnMemberType, theTeamMembers, null);
        }

        /*
        public delegate void UpdateStatusBar(string strStatus);
        // if the coach tries to add a note in the consultant's pane, that should fail.
        // (but it's okay for a project facilitator to add one if they have a question
        //  for the consultant)
        public bool CheckAddNotePrivilege(UpdateStatusBar pUpdateStatusBar, 
            TeamMemberData.UserTypes eLoggedOnMember)
        {
            if (!HasAddNotePrivilege(eLoggedOnMember))
            {
                pUpdateStatusBar("Error: " + String.Format("You must be logged in as a '{0}' or a '{1}' to add a note here",
                    TeamMemberData.GetMemberTypeAsDisplayString(MentorType),
                    TeamMemberData.GetMemberTypeAsDisplayString(MenteeType)));
                return false;
            }
            return true;
        }
        */

        public virtual bool HasAddNotePrivilege(TeamMemberData loggedOnMember, 
            string strThePfMemberId)
        {
            return (TeamMemberData.IsUser(loggedOnMember.MemberType, MentorType)) ||
                   (TeamMemberData.IsUser(loggedOnMember.MemberType, MenteeType));
        }

        public bool HasAddNoteToSelfPrivilege(TeamMemberData.UserTypes eLoggedOnMember)
        {
            return TeamMemberData.IsUser(MentorType, eLoggedOnMember);
        }

        public abstract ConsultNoteDataConverter Add(StoryData theStory,
            TeamMemberData LoggedOnMember, TeamMembersData theTeamMembers, string strValue);
        protected abstract TeamMemberData.UserTypes MentorType { get; }
        protected abstract TeamMemberData.UserTypes MenteeType { get; }

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

        public string Html(object htmlConNoteCtrl, TeamMemberData LoggedOnMember, 
            TeamMembersData teamMembers, StoryData theStory, bool bViewHidden, 
            bool bVerseVisible, bool bShowOnlyOpenConversations, int nVerseIndex)
        {
            string strHtml = null;
            for (int i = 0; i < Count; i++)
            {
                ConsultNoteDataConverter aCNDC = this[i];
                if ((aCNDC.Visible || bViewHidden) 
                    && (!bShowOnlyOpenConversations 
                        || !aCNDC.IsFinished 
                        || ShowOpenConversations))
                    strHtml += aCNDC.Html(htmlConNoteCtrl, teamMembers, LoggedOnMember,
                        theStory, nVerseIndex, i);
            }

            // color changes if hidden
            string strColor;
            if (bVerseVisible)
                strColor = "#FFFFFF";
            else
                strColor = "#F0E68C";   // khakhi

            int nSpan = 3;
            if (bShowOnlyOpenConversations)
                nSpan++;

            return String.Format(OseResources.Properties.Resources.HTML_TableRowColor, strColor,
                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nSpan,
                        String.Format(OseResources.Properties.Resources.HTML_TableNoBorder, strHtml)));
        }

        public void AllowButtonsOverride()
        {
            foreach (ConsultNoteDataConverter aCNDC in this)
                aCNDC.AllowButtonsOverride = true;
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (ConsultNoteDataConverter aCNDC in this)
                aCNDC.IndexSearch(findProperties, lstBoxesToSearch);
        }

        public void SetCommentMemberId(string strMentoree, string strMentor)
        {
            foreach (var aCn in this)
                aCn.SetCommentMemberId(strMentoree, strMentor);
        }
    }

    public class ConsultantNotesData : ConsultNotesDataConverter
    {
        protected const string CstrCollectionElementName = "ConsultantNotes";

        public ConsultantNotesData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
            : base(CstrCollectionElementName)
        {
            NewDataSet.ConsultantNotesRow[] theConsultantNotesRows = theVerseRow.GetConsultantNotesRows();
            NewDataSet.ConsultantNotesRow theConsultantNotesRow;
            if (theConsultantNotesRows.Length == 0)
                theConsultantNotesRow = projFile.ConsultantNotes.AddConsultantNotesRow(theVerseRow);
            else
                theConsultantNotesRow = theConsultantNotesRows[0];

            foreach (NewDataSet.ConsultantConversationRow aConsultantConversationRow in theConsultantNotesRow.GetConsultantConversationRows())
                Add(new ConsultantNoteData(aConsultantConversationRow));
        }

        public ConsultantNotesData(IEnumerable<ConsultNoteDataConverter> rhs)
            : base(CstrCollectionElementName)
        {
            foreach (var aCND in rhs)
                Add(new ConsultantNoteData(aCND));
        }

        public ConsultantNotesData()
            : base(CstrCollectionElementName)
        {
        }

        public override bool HasAddNotePrivilege(TeamMemberData loggedOnMember,
            string strThePfMemberId)
        {
            // for this one, we also have to have the right PF
            return (TeamMemberData.IsUser(loggedOnMember.MemberType, MentorType) ||
                    (loggedOnMember.MemberGuid == strThePfMemberId));
        }

        public override ConsultNoteDataConverter Add(StoryData theStory,
            TeamMemberData LoggedOnMember, TeamMembersData theTeamMembers, string strValue)
        {
            ConsultNoteDataConverter theNewCN = new ConsultantNoteData(theStory, 
                LoggedOnMember, theTeamMembers, strValue);
            Add(theNewCN);
            return theNewCN;
        }
        /*
        // if the coach tries to add a note in the consultant's pane, that should fail.
        // (but it's okay for a project facilitator to add one if they have a question
        //  for the consultant).
        // The base class version is overridden here, because for the Consultant Pane, the 'First Pass Mentor' is
        //  also a valid note adder
        public override bool CheckAddNotePrivilege(StoryEditor theSE, TeamMemberData.UserTypes eLoggedOnMember)
        {
            if (!HasAddNotePrivilege(eLoggedOnMember))
            {
                theSE.SetStatusBar("Error: " + String.Format("You must be logged in as a '{0}', a '{1}', a '{2}', or a '{3}' to add a note here",
                    TeamMemberData.GetMemberTypeAsDisplayString(MentorType),
                    TeamMemberData.GetMemberTypeAsDisplayString(TeamMemberData.UserTypes.eFirstPassMentor),
                    TeamMemberData.GetMemberTypeAsDisplayString(TeamMemberData.UserTypes.eIndependentConsultant),
                    TeamMemberData.GetMemberTypeAsDisplayString(MenteeType)));
                return false;
            }
            return true;
        }

        public override bool HasAddNotePrivilege(TeamMemberData.UserTypes eLoggedOnMember)
        {
            return ((eLoggedOnMember == MentorType)
                    ||
                    (eLoggedOnMember == TeamMemberData.UserTypes.eFirstPassMentor)
                    || 
                    (eLoggedOnMember == TeamMemberData.UserTypes.eIndependentConsultant)
                    || 
                    (eLoggedOnMember == MenteeType));
        }
        */

        protected override TeamMemberData.UserTypes MentorType
        {
            get
            {
                // the 'mentor' for this class can be any of the following
                return (TeamMemberData.UserTypes.IndependentConsultant
                        | TeamMemberData.UserTypes.ConsultantInTraining
                        | TeamMemberData.UserTypes.FirstPassMentor);
            }
        }

        protected override TeamMemberData.UserTypes MenteeType
        {
            get { return TeamMemberData.UserTypes.ProjectFacilitator; }
        }

        public bool AreUnapprovedComments
        {
            get
            {
                return this.Any(aCndc => aCndc.Visible &&
                                         !aCndc.IsFinished &&
                                         aCndc.NoteNeedsApproval);
            }
        }

        public void UpdateCommentMemberId(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var aCndc in this)
                aCndc.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
        }
    }

    public class CoachNotesData : ConsultNotesDataConverter
    {
        protected const string CstrCollectionElementName = "CoachNotes";

        public CoachNotesData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
            : base(CstrCollectionElementName)
        {
            NewDataSet.CoachNotesRow[] theCoachNotesRows = theVerseRow.GetCoachNotesRows();
            NewDataSet.CoachNotesRow theCoachNotesRow;
            if (theCoachNotesRows.Length == 0)
                theCoachNotesRow = projFile.CoachNotes.AddCoachNotesRow(theVerseRow);
            else
                theCoachNotesRow = theCoachNotesRows[0];

            foreach (NewDataSet.CoachConversationRow aCoachConversationRow in theCoachNotesRow.GetCoachConversationRows())
                Add(new CoachNoteData(aCoachConversationRow));
        }

        public CoachNotesData(IEnumerable<ConsultNoteDataConverter> rhs)
            : base(CstrCollectionElementName)
        {
            foreach (var aCND in rhs)
                Add(new CoachNoteData(aCND));
        }

        public CoachNotesData()
            : base(CstrCollectionElementName)
        {
        }

        public override ConsultNoteDataConverter Add(StoryData theStory,
            TeamMemberData LoggedOnMember, TeamMembersData theTeamMembers, string strValue)
        {
            // always add closest to the verse label
            ConsultNoteDataConverter theNewCN = new CoachNoteData(theStory, 
                LoggedOnMember, theTeamMembers, strValue);
            Add(theNewCN);
            return theNewCN;
        }

        protected override TeamMemberData.UserTypes MentorType
        {
            get { return TeamMemberData.UserTypes.Coach; }
        }

        protected override TeamMemberData.UserTypes MenteeType
        {
            get
            {
                // the mentee type for this class can be any of the following
                return (TeamMemberData.UserTypes.ConsultantInTraining |
                        TeamMemberData.UserTypes.FirstPassMentor);
            }
        }

        public bool AreUnrespondedToCoachNoteComments
        {
            get
            {
                return this.Any(aCndc =>
                                aCndc.Visible &&
                                !aCndc.IsFinished &&
                                aCndc.IsFromMentee(aCndc.FinalComment));
            }
        }
    }
}
