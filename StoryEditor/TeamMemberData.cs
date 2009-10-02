using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TeamMemberData
    {
        public enum UserTypes
        {
            eUndefined = 0,
            eCrafter,
            eEnglishBacktranslator,
            eUNS,
            eProjectFacilitator,
            eConsultantInTraining,
            eCoach,
            eJustLooking
        }

        internal const string CstrCrafter = "Crafter";
        internal const string CstrEnglishBackTranslator = "EnglishBackTranslator";
        internal const string CstrUNS = "UNS";
        internal const string CstrProjectFacilitator = "ProjectFacilitator";
        internal const string CstrConsultantInTraining = "ConsultantInTraining";
        internal const string CstrCoach = "Coach";
        internal const string CstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        protected const string CstrEnglishBackTranslatorDisplay = "English Back Translator";
        protected const string CstrConsultantInTrainingDisplay = "Mentor";
        protected const string CstrProjectFacilitatorDisplay = "Project Facilitator";
        
        public string Name = null;
        public UserTypes MemberType = UserTypes.eUndefined;
        public string MemberGuid = null;
        public string Email = null;
        public string SkypeID = null;
        public string TeamViewerID = null;
        public string Phone = null;
        public string AltPhone = null;
        public string Address = null;

        public TeamMemberData(string strName, TeamMemberData.UserTypes eMemberType, string strMemberGuid, string strEmail, string strSkypeID, string strTeamViewerID, string strPhone, string strAltPhone, string strAddress)
        {
            Name = strName;
            MemberType = eMemberType;
            MemberGuid = strMemberGuid;
            Email = strEmail;
            SkypeID = strSkypeID;
            TeamViewerID = strTeamViewerID;
            Phone = strPhone;
            AltPhone = strAltPhone;
            Address = strAddress;
        }

        public TeamMemberData(StoryProject.MemberRow theMemberRow)
        {
            Name = theMemberRow.name;
            MemberType = GetMemberType(theMemberRow.memberType);
            MemberGuid = theMemberRow.memberKey;

            // now for the optional ones
            if (!theMemberRow.IsemailNull())
                Email = theMemberRow.email;

            if (!theMemberRow.IsskypeIDNull())
                SkypeID = theMemberRow.skypeID;

            if (!theMemberRow.IsteamViewerIDNull())
                TeamViewerID = theMemberRow.teamViewerID;

            if (!theMemberRow.IsphoneNull())
                Phone = theMemberRow.phone;

            if (!theMemberRow.IsaltPhoneNull())
                AltPhone = theMemberRow.altPhone;

            if (!theMemberRow.IsaddressNull())
                Address = theMemberRow.address;
        }

        public static TeamMemberData.UserTypes GetMemberType(string strMemberTypeString)
        {
            if (strMemberTypeString == CstrCrafter)
                return TeamMemberData.UserTypes.eCrafter;
            else if (strMemberTypeString == CstrEnglishBackTranslator)
                return TeamMemberData.UserTypes.eEnglishBacktranslator;
            else if (strMemberTypeString == CstrUNS)
                return TeamMemberData.UserTypes.eUNS;
            else if (strMemberTypeString == CstrProjectFacilitator)
                return TeamMemberData.UserTypes.eProjectFacilitator;
            else if (strMemberTypeString == CstrConsultantInTraining)
                return TeamMemberData.UserTypes.eConsultantInTraining;
            else if (strMemberTypeString == CstrCoach)
                return TeamMemberData.UserTypes.eCoach;
            else if (strMemberTypeString == CstrJustLooking)
                return TeamMemberData.UserTypes.eJustLooking;
            else
                return TeamMemberData.UserTypes.eUndefined;
        }

        public string MemberTypeAsString
        {
            get { return GetMemberTypeAsString(MemberType); }
        }

        public static string GetMemberTypeAsDisplayString(TeamMemberData.UserTypes eMemberType)
        {
            if (eMemberType == UserTypes.eConsultantInTraining)
                return CstrConsultantInTrainingDisplay;
            else if (eMemberType == UserTypes.eEnglishBacktranslator)
                return CstrEnglishBackTranslatorDisplay;
            else if (eMemberType == UserTypes.eProjectFacilitator)
                return CstrProjectFacilitatorDisplay;
            else
                return GetMemberTypeAsString(eMemberType);
        }

        public static string GetMemberTypeAsString(TeamMemberData.UserTypes eMemberType)
        {
            switch (eMemberType)
            {
                case TeamMemberData.UserTypes.eCrafter:
                    return CstrCrafter;
                case UserTypes.eEnglishBacktranslator:
                    return CstrEnglishBackTranslator;
                case TeamMemberData.UserTypes.eUNS:
                    return CstrUNS;
                case TeamMemberData.UserTypes.eProjectFacilitator:
                    return CstrProjectFacilitator;
                case TeamMemberData.UserTypes.eConsultantInTraining:
                    return CstrConsultantInTraining;
                case TeamMemberData.UserTypes.eCoach:
                    return CstrCoach;
                case TeamMemberData.UserTypes.eJustLooking:
                    return CstrJustLooking;
                default:
                    System.Diagnostics.Debug.Assert(false); // shouldn't get here
                    return null;
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement eleMember = new XElement("Member",
                    new XAttribute("name", this.Name),
                    new XAttribute("memberType", this.MemberTypeAsString));
                    if (!String.IsNullOrEmpty(this.Email))
                        eleMember.Add(new XAttribute("email", this.Email));
                    if (!String.IsNullOrEmpty(this.AltPhone))
                        eleMember.Add(new XAttribute("altPhone", this.AltPhone));
                    if (!String.IsNullOrEmpty(this.Phone))
                        eleMember.Add(new XAttribute("phone", this.Phone));
                    if (!String.IsNullOrEmpty(this.Address))
                        eleMember.Add(new XAttribute("address", this.Address));
                    if (!String.IsNullOrEmpty(this.SkypeID))
                        eleMember.Add(new XAttribute("skypeID", this.SkypeID));
                    if (!String.IsNullOrEmpty(this.TeamViewerID))
                        eleMember.Add(new XAttribute("teamViewerID", this.TeamViewerID));
                    eleMember.Add(new XAttribute("memberKey", this.MemberGuid));

                return eleMember;
            }
        }
    }

    public class TeamMembersData : Dictionary<string, TeamMemberData>
    {
        protected const string CstrBrowserMemberName = "Browser";

        public TeamMembersData()
        {
            TeamMemberData aTMD = new TeamMemberData(CstrBrowserMemberName, TeamMemberData.UserTypes.eJustLooking,
                Guid.NewGuid().ToString(), null, null, null, null, null, null);
            Add(CstrBrowserMemberName, aTMD);
        }

        public TeamMembersData(StoryProject projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.stories != null) && (projFile.stories.Count > 0));
            StoryProject.storiesRow theStoriesRow = projFile.stories[0];
            StoryProject.MembersRow[] aMembersRows = theStoriesRow.GetMembersRows();
            StoryProject.MembersRow theMembersRow;
            if (aMembersRows.Length == 0)
                theMembersRow = projFile.Members.AddMembersRow(theStoriesRow);
            else
                theMembersRow = aMembersRows[0];

            foreach (StoryProject.MemberRow aMemberRow in theMembersRow.GetMemberRows())
                Add(aMemberRow.name, new TeamMemberData(aMemberRow));
        }

        // this can be used to determine whether a given member name and type are one
        //  of the ones in this project (for auto-login)
        public bool CanLoginMember(string strMemberName, string strMemberType)
        {
            if (ContainsKey(strMemberName))
            {
                TeamMemberData aTMD = this[strMemberName];
                if (aTMD.MemberTypeAsString == strMemberType)
                {
                    // kind of a kludge, but necessary for the state logic
                    //  If we're going to return true (meaning that we can auto-log this person in), then
                    //  if we have an English Back-translator person in the team, then we have to set the 
                    //  member with the edit token when we get to the EnglishBT state as that person
                    //  otherwise, it's a crafter
                    StoryStageLogic.stateTransitions[StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT].MemberTypeWithEditToken =
                        (IsThereASeparateEnglishBackTranslator) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eCrafter;
                    return true;
                }
            }
            return false;
        }

        public bool IsThereASeparateEnglishBackTranslator
        {
            get
            {
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eEnglishBacktranslator)
                        return true;
                return false;
            }
        }

        public bool IsThereAProjectFacilitator
        {
            get
            {
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
                        return true;
                return false;
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement eleMembers = new XElement("Members");

                foreach (TeamMemberData aMemberData in this.Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }
    }
}
