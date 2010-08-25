using System;
using System.Collections.Generic;
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

        public CommInstance(string strValue, 
            ConsultNoteDataConverter.CommunicationDirections direction, 
            string strGuid, DateTime timeStamp)
            : base(strValue)
        {
            Direction = direction;
            Guid = strGuid ?? System.Guid.NewGuid().ToString();
            TimeStamp = timeStamp;
        }

        public CommInstance(CommInstance rhs)
            : base(rhs.ToString())
        {
            Direction = rhs.Direction;

            // the guid shouldn't be replicated
            Guid = System.Guid.NewGuid().ToString();  // rhs.Guid;
            TimeStamp = rhs.TimeStamp;
        }

        public TeamMemberData.UserTypes Initiator
        {
            get
            {
                switch (Direction)
                {
                    case ConsultNoteDataConverter.CommunicationDirections.eProjFacToConsultant:
                        return TeamMemberData.UserTypes.eProjectFacilitator;

                    case ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant:
                        return TeamMemberData.UserTypes.eCoach;

                    default:
                        return TeamMemberData.UserTypes.eConsultantInTraining;
                }
            }
        }
    }

    public abstract class ConsultNoteDataConverter : List<CommInstance>
    {
        public int RoundNum;
        public string guid;
        public bool Visible = true;
        public bool IsFinished;
        public bool AllowButtonsOverride;

        public enum CommunicationDirections
        {
            eConsultantToProjFac,
            eProjFacToConsultant,
            eConsultantToCoach,
            eCoachToConsultant
        }

        protected ConsultNoteDataConverter(int nRoundNum)
        {
            RoundNum = nRoundNum;
            guid = Guid.NewGuid().ToString();
        }

        protected ConsultNoteDataConverter(int nRoundNum, string strGuid, 
            bool bVisible, bool bIsFinished)
        {
            RoundNum = nRoundNum;
            guid = strGuid;
            Visible = bVisible;
            IsFinished = bIsFinished;
        }

        protected ConsultNoteDataConverter(ConsultNoteDataConverter rhs)
        {
            RoundNum = rhs.RoundNum;

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

        public void InsureExtraBox(StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember, 
            TeamMemberData.UserTypes eMentorType, TeamMemberData.UserTypes eMenteeType,
            string strValue)
        {
            // in case the user re-logs in, we might have extra boxes here. So remove any null ones before
            //  "insuring" the one(s) we need
            if (Count > 1)
                while (!this[Count - 1].HasData)
                    RemoveAt(Count - 1);

            // don't bother, though, if the user has ended the conversation
            if (IsFinished || !LoggedOnMember.IsEditAllowed(theStoryStage.MemberTypeWithEditToken))
                return;

            TeamMemberData.UserTypes eLoggedOnMember = LoggedOnMember.MemberType;
            if (((eLoggedOnMember & eMentorType) == eLoggedOnMember) && ((Count == 0) || (this[Count - 1].Direction == MenteeDirection)))
                Add(new CommInstance(strValue, MentorDirection, null, DateTime.Now));
            else if (((eLoggedOnMember & eMenteeType) == eLoggedOnMember) && ((Count == 0) || (this[Count - 1].Direction == MentorDirection)))
                Add(new CommInstance(strValue, MenteeDirection, null, DateTime.Now));
        }
        /*
        // do this here, because we need to sub-class it to allow for FirstPassMentor working as well in addition to CIT
        public virtual void ThrowIfWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            if (IsWrongEditor(eLoggedOnMember, eRequiredEditor))
                throw new ApplicationException(String.Format("Only a '{0}' can edit this field", TeamMemberData.GetMemberTypeAsDisplayString(eRequiredEditor)));
        }
        */
        protected virtual bool IsWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            return (eLoggedOnMember != eRequiredEditor);
        }

        protected virtual bool CanDoConversationButtons(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            return !IsWrongEditor(eLoggedOnMember, eRequiredEditor) || AllowButtonsOverride;
        }

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
            get { return Color.AliceBlue; }
        }

        public Color ResponseColor
        {
            get { return Color.Cornsilk; /* LightCyan */ }
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

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(Count > 0);

                XElement eleNote = new XElement(InstanceElementName, 
                    new XAttribute("round", RoundNum),
                    new XAttribute("guid", guid));

                if (!Visible)
                    eleNote.Add(new XAttribute("visible", false));

                if (IsFinished)
                    eleNote.Add(new XAttribute("finished", true));

                foreach (CommInstance aCI in this)
                    if (aCI.HasData)
                        eleNote.Add(new XElement(SubElementName,
                            new XAttribute("Direction", GetDirectionString(aCI.Direction)),
                            new XAttribute("guid", aCI.Guid),
                            new XAttribute("timeStamp", aCI.TimeStamp),
                            aCI.ToString()));

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
        internal const string CstrRoundLabel = "Round: ";

        public bool IsEditable(StoryStageLogic theStoryStage, int i, 
            TeamMemberData LoggedOnMember, CommInstance aCI)
        {
            return (i == (Count - 1))
                   && !IsFinished
                   && LoggedOnMember.IsEditAllowed(theStoryStage.MemberTypeWithEditToken)
                   && ((IsFromMentor(aCI) && !IsWrongEditor(LoggedOnMember.MemberType, MentorRequiredEditor))
                       || (!IsFromMentor(aCI) && !IsWrongEditor(LoggedOnMember.MemberType, MenteeRequiredEditor)));
        }

        public bool IsFromMentor(CommInstance aCI)
        {
            return (aCI.Direction == MentorDirection);
        }

        readonly Regex regexBibRef = new Regex(@"\b(([a-zA-Z]{3,4}|[1-3][a-zA-Z]{2,5}) \d{1,3}:\d{1,3})\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly Regex regexLineRef = new Regex(@"\b((Ln|ln|line) ([1-9][0-9]?))\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly Regex regexItalics = new Regex(@"\*\b(.+?)\b\*", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        readonly Regex regexHttpRef = new Regex(@"\b(http://.*?) \b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        public string Html(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember, 
            int nVerseIndex, int nConversationIndex)
        {
            System.Diagnostics.Debug.Assert(Count > 0);
            if (Count == 0)
                return null;

            // r1: "Round: n"; "button"
            // r2-n: "Label:"; "value in textbox"
            string strRow = String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpanAndWidth, 100, 2,
                String.Format("{0}{1}", CstrRoundLabel, RoundNum));

            // only the initiator of a conversation gets the buttons to delete, hide or
            //  end conversation.
            CommInstance aCInitiator = this[0];
            if (CanDoConversationButtons(LoggedOnMember.MemberType, aCInitiator.Initiator))
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                        String.Format(OseResources.Properties.Resources.HTML_Button,
                                                      ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexDelete),
                                                      "return window.external.OnClickDelete(this.id);",
                                                      "Delete"));

                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                        String.Format(OseResources.Properties.Resources.HTML_Button,
                                                      ButtonId(nVerseIndex, nConversationIndex, CnBtnIndexHide),
                                                      "return window.external.OnClickHide(this.id);",
                                                      (Visible) ? CstrButtonLabelHide : CstrButtonLabelUnhide));

                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                        String.Format(OseResources.Properties.Resources.HTML_Button,
                                                      ButtonId(nVerseIndex, nConversationIndex,
                                                               CnBtnIndexEndConversation),
                                                      "return window.external.OnClickEndConversation(this.id);",
                                                      (IsFinished)
                                                          ? CstrButtonLabelConversationReopen
                                                          : CstrButtonLabelConversationEnd));
            }

            string strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRowId,
                ButtonRowId(nVerseIndex, nConversationIndex),
                strRow);

            string strHtmlTable = null;
            for (int i = 0; i < Count; i++)
            {
                CommInstance aCI = this[i];

                strRow = null;
                Color clrRow;
                if (IsFromMentor(aCI))
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            MentorLabel);
                    clrRow = CommentColor;
                }
                else
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                            MenteeLabel);
                    clrRow = ResponseColor;
                }

                string strColor = VerseData.HtmlColor(clrRow);

                // only the last one is editable and then only if the right person is 
                //  logged in
                string strHtmlElementId;
                if (IsEditable(theStoryStage, i, LoggedOnMember, aCI))
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
                    string strHyperlinkedText = aCI.ToString().Replace("\r\n", "<br />");   // regexParagraph.Replace(aCI.ToString(), ParagraphFound);
                    strHyperlinkedText = regexBibRef.Replace(strHyperlinkedText, BibleReferenceFound);
                    strHyperlinkedText = regexLineRef.Replace(strHyperlinkedText, LineReferenceFound);
                    strHyperlinkedText = regexItalics.Replace(strHyperlinkedText, EmphasizedTextFound);
                    strHyperlinkedText = regexHttpRef.Replace(strHyperlinkedText, HttpReferenceFound);

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

            strHtml += String.Format(OseResources.Properties.Resources.HTML_TableRowIdColor,
                                     ConversationTableRowId(nVerseIndex, nConversationIndex),
                                     "#CCFFAA",
                                     String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, 5,
                                                   strEmbeddedTable));

            return strHtml;
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
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData(NewDataSet.ConsultantConversationRow aConRow)
            : base (aConRow.round, aConRow.guid, 
            (aConRow.IsvisibleNull()) ? true : aConRow.visible,
            (aConRow.IsfinishedNull()) ? false : aConRow.finished)
        {
            NewDataSet.ConsultantNoteRow[] theNoteRows = aConRow.GetConsultantNoteRows();
            foreach (NewDataSet.ConsultantNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.ConsultantNote_text, 
                    GetDirectionFromString(aNoteRow.Direction),
                    aNoteRow.guid, (aNoteRow.IstimeStampNull()) ? 
                        DateTime.Now : aNoteRow.timeStamp));

            // make sure that there are at least two (we can't save them if they're empty)
            System.Diagnostics.Debug.Assert(Count != 0, "It looks like you have an empty Consultant Note field that shouldn't be there. For now, you can just 'Ignore' this error (but perhaps let bob_eaton@sall.com know)");
        }

        public ConsultantNoteData(int nRound, StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember,
            TeamMemberData.UserTypes eMentorType, TeamMemberData.UserTypes eMenteeType,
            string strValue)
            : base(nRound)
        {
            InsureExtraBox(theStoryStage, LoggedOnMember, eMentorType, eMenteeType, strValue);
        }

        public ConsultantNoteData(ConsultNoteDataConverter rhs)
            : base(rhs)
        {
        }

        public override CommunicationDirections MentorDirection
        {
            get { return CommunicationDirections.eConsultantToProjFac; }
        }

        public override CommunicationDirections MenteeDirection
        {
            get { return CommunicationDirections.eProjFacToConsultant; }
        }

        public override string MentorLabel
        {
            get { return "cons:"; }
        }

        public override string MenteeLabel
        {
            get { return "prfc:"; }
        }

        protected override string InstanceElementName
        {
            get { return "ConsultantConversation"; }
        }

        protected override string SubElementName
        {
            get { return "ConsultantNote"; }
        }

        protected override bool IsWrongEditor(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            // if it's the *mentor* that we're supposed to be checking for... (this will get called for the mentee check
            //  as well, but the special case is only for FirstPassMentor; not ProjFac)
            if (eRequiredEditor == MentorRequiredEditor)
            {
                // ... and if the logged in member isn't that mentor, nor the First Pass 
                //  Mentor, nor an independent consultant... or an English BTr
                if ((eLoggedOnMember == eRequiredEditor)
                    || 
                    (eLoggedOnMember == TeamMemberData.UserTypes.eFirstPassMentor)
                    || 
                    (eLoggedOnMember == TeamMemberData.UserTypes.eIndependentConsultant)
                    ||
                    (eLoggedOnMember == TeamMemberData.UserTypes.eEnglishBacktranslator)
                    )
                {
                    return false;
                }
            }
            
            // otherwise, let the base class handle it
            return base.IsWrongEditor(eLoggedOnMember, eRequiredEditor);
        }

        protected override bool CanDoConversationButtons(TeamMemberData.UserTypes eLoggedOnMember, TeamMemberData.UserTypes eRequiredEditor)
        {
            // if it's the *mentor* that we're supposed to be checking for... (this will get called for the mentee check
            //  as well, but the special case is only for FirstPassMentor; not ProjFac)
            if (eRequiredEditor == MentorRequiredEditor)
            {
                // ... and if the logged in member isn't that mentor, nor the First Pass 
                //  Mentor, nor an independent consultant... or an English BTr
                if ((eLoggedOnMember == eRequiredEditor)
                    ||
                    (eLoggedOnMember == TeamMemberData.UserTypes.eFirstPassMentor)
                    ||
                    (eLoggedOnMember == TeamMemberData.UserTypes.eIndependentConsultant))
                {
                    return true;
                }
            }

            // otherwise, let the base class handle it
            // I really want this to be base.IsWrongEditorbase (we don't want to call back to
            //  *our* version of IsWro...)
            return !base.IsWrongEditor(eLoggedOnMember, eRequiredEditor) || AllowButtonsOverride;
        }
        /*
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
            get { return TeamMemberData.UserTypes.eConsultantInTraining; }
        }

        public override TeamMemberData.UserTypes MenteeRequiredEditor
        {
            get { return TeamMemberData.UserTypes.eProjectFacilitator; }
        }

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedPane
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields; }
        }
    }

    public class CoachNoteData : ConsultNoteDataConverter
    {
        public CoachNoteData(NewDataSet.CoachConversationRow aCoaCRow)
            : base (aCoaCRow.round, aCoaCRow.guid, 
            (aCoaCRow.IsvisibleNull()) ? true : aCoaCRow.visible,
            (aCoaCRow.IsfinishedNull()) ? false : aCoaCRow.finished)
        {
            NewDataSet.CoachNoteRow[] theNoteRows = aCoaCRow.GetCoachNoteRows();
            foreach (NewDataSet.CoachNoteRow aNoteRow in theNoteRows)
                Add(new CommInstance(aNoteRow.CoachNote_text, 
                    GetDirectionFromString(aNoteRow.Direction), 
                    aNoteRow.guid,
                    (aNoteRow.IstimeStampNull()) ? DateTime.Now :
                    aNoteRow.timeStamp));
        }

        public CoachNoteData(int nRound, StoryStageLogic theStoryStage, 
            TeamMemberData LoggedOnMember,
            TeamMemberData.UserTypes eMentorType, TeamMemberData.UserTypes eMenteeType,
            string strValue)
            : base (nRound)
        {
            InsureExtraBox(theStoryStage, LoggedOnMember, eMentorType, eMenteeType, strValue);
        }

        public CoachNoteData(ConsultNoteDataConverter rhs)
            : base(rhs)
        {
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

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedPane
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields; }
        }
    }

    public abstract class ConsultNotesDataConverter : List<ConsultNoteDataConverter>
    {
        protected string CollectionElementName = null;

        protected ConsultNotesDataConverter(string strCollectionElementName)
        {
            CollectionElementName = strCollectionElementName;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public void InsureExtraBox(ConsultNoteDataConverter aCNDC, 
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMemberType)
        {
            aCNDC.InsureExtraBox(theStoryStage, LoggedOnMemberType, MentorType, MenteeType, null);
        }

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

        public bool HasAddNotePrivilege(TeamMemberData.UserTypes eLoggedOnMember)
        {
            return ((eLoggedOnMember & MentorType) == eLoggedOnMember) ||
                   ((eLoggedOnMember & MenteeType) == eLoggedOnMember);
        }

        public abstract ConsultNoteDataConverter Add(int nRound,
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, 
            string strValue);
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

        public string Html(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, 
            bool bViewHidden, bool bVerseVisible, int nVerseIndex)
        {
            string strHtml = null;
            for (int i = 0; i < Count; i++)
            {
                ConsultNoteDataConverter aCNDC = this[i];
                if (aCNDC.Visible || bViewHidden)
                    strHtml += aCNDC.Html(htmlConNoteCtrl, theStoryStage, LoggedOnMember, nVerseIndex, i);
            }

            // color changes if hidden
            string strColor;
            if (bVerseVisible)
                strColor = "#FFFFFF";
            else
                strColor = "#F0E68C";

            return String.Format(OseResources.Properties.Resources.HTML_TableRowColor, strColor,
                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, 2,
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
    }

    public class ConsultantNotesData : ConsultNotesDataConverter
    {
        protected const string CstrCollectionElementName = "ConsultantNotes";

        public ConsultantNotesData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
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

        public override ConsultNoteDataConverter Add(int nRound,
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, 
            string strValue)
        {
            /*
            TeamMemberData.UserTypes eMentorType = MentorType;
            if (eLoggedOnMember == TeamMemberData.UserTypes.eFirstPassMentor)
                eMentorType = TeamMemberData.UserTypes.eFirstPassMentor;
            else if (eLoggedOnMember == TeamMemberData.UserTypes.eIndependentConsultant)
                eMentorType = TeamMemberData.UserTypes.eIndependentConsultant;
            */
            ConsultNoteDataConverter theNewCN = new ConsultantNoteData(nRound,
                theStoryStage, LoggedOnMember, MentorType, MenteeType, strValue);
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
                return (TeamMemberData.UserTypes.eConsultantInTraining
                        | TeamMemberData.UserTypes.eFirstPassMentor
                        | TeamMemberData.UserTypes.eIndependentConsultant);
            }
        }

        protected override TeamMemberData.UserTypes MenteeType
        {
            get { return TeamMemberData.UserTypes.eProjectFacilitator; }
        }
    }

    public class CoachNotesData : ConsultNotesDataConverter
    {
        protected const string CstrCollectionElementName = "CoachNotes";

        public CoachNotesData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
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

        public override ConsultNoteDataConverter Add(int nRound,
            StoryStageLogic theStoryStage, TeamMemberData LoggedOnMember, 
            string strValue)
        {
            // always add closest to the verse label
            ConsultNoteDataConverter theNewCN = new CoachNoteData(nRound, 
                theStoryStage, LoggedOnMember, MentorType, MenteeType, strValue);
            Add(theNewCN);
            return theNewCN;
        }

        protected override TeamMemberData.UserTypes MentorType
        {
            get { return TeamMemberData.UserTypes.eCoach; }
        }

        protected override TeamMemberData.UserTypes MenteeType
        {
            get
            {
                // the mentee type for this class can be any of the following
                return (TeamMemberData.UserTypes.eConsultantInTraining
                        | TeamMemberData.UserTypes.eFirstPassMentor
                        | TeamMemberData.UserTypes.eIndependentConsultant);
            }
        }
    }
}
