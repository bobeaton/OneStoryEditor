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
            eFirstPassMentor,
            eConsultantInTraining,
            eCoach,
            eJustLooking
        }

        internal const string CstrCrafter = "Crafter";
        internal const string CstrEnglishBackTranslator = "EnglishBackTranslator";
        internal const string CstrUNS = "UNS";
        internal const string CstrProjectFacilitator = "ProjectFacilitator";
        internal const string CstrFirstPassMentor = "FirstPassMentor";
        internal const string CstrConsultantInTraining = "ConsultantInTraining";
        internal const string CstrCoach = "Coach";
        internal const string CstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        protected const string CstrEnglishBackTranslatorDisplay = "English Back Translator";
        protected const string CstrFirstPassMentorDisplay = "First Pass Mentor";
        protected const string CstrConsultantInTrainingDisplay = "Consultant in Training";
        protected const string CstrProjectFacilitatorDisplay = "Project Facilitator";
        
        public string Name = null;
        public UserTypes MemberType = UserTypes.eUndefined;
        public string MemberGuid = null;
        public string Email = null;
        public string SkypeID = null;
        public string TeamViewerID = null;
        public string Phone = null;
        public string AltPhone = null;
        public string BioData = null;

        public TeamMemberData(string strName, UserTypes eMemberType, string strMemberGuid, string strEmail, string strSkypeID, string strTeamViewerID, string strPhone, string strAltPhone, string strBioData)
        {
            Name = strName;
            MemberType = eMemberType;
            MemberGuid = strMemberGuid;
            Email = strEmail;
            SkypeID = strSkypeID;
            TeamViewerID = strTeamViewerID;
            Phone = strPhone;
            AltPhone = strAltPhone;
            BioData = strBioData;
        }

        public TeamMemberData(NewDataSet.MemberRow theMemberRow)
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

            if (!theMemberRow.IsbioDataNull())
                BioData = theMemberRow.bioData;
        }

        public static UserTypes GetMemberType(string strMemberTypeString)
        {
            if (strMemberTypeString == CstrCrafter)
                return UserTypes.eCrafter;
            if (strMemberTypeString == CstrEnglishBackTranslator)
                return UserTypes.eEnglishBacktranslator;
            if (strMemberTypeString == CstrUNS)
                return UserTypes.eUNS;
            if (strMemberTypeString == CstrProjectFacilitator)
                return UserTypes.eProjectFacilitator;
            if (strMemberTypeString == CstrFirstPassMentor)
                return UserTypes.eFirstPassMentor;
            if (strMemberTypeString == CstrConsultantInTraining)
                return UserTypes.eConsultantInTraining;
            if (strMemberTypeString == CstrCoach)
                return UserTypes.eCoach;
            if (strMemberTypeString == CstrJustLooking)
                return UserTypes.eJustLooking;
            return UserTypes.eUndefined;
        }

        public string MemberTypeAsString
        {
            get { return GetMemberTypeAsString(MemberType); }
        }

        public static string GetMemberTypeAsDisplayString(UserTypes eMemberType)
        {
            if (eMemberType == UserTypes.eConsultantInTraining)
                return CstrConsultantInTrainingDisplay;
            if (eMemberType == UserTypes.eEnglishBacktranslator)
                return CstrEnglishBackTranslatorDisplay;
            if (eMemberType == UserTypes.eProjectFacilitator)
                return CstrProjectFacilitatorDisplay;
            if (eMemberType == UserTypes.eFirstPassMentor)
                return CstrFirstPassMentorDisplay;
            return GetMemberTypeAsString(eMemberType);
        }

        public static string GetMemberTypeAsString(UserTypes eMemberType)
        {
            switch (eMemberType)
            {
                case UserTypes.eCrafter:
                    return CstrCrafter;
                case UserTypes.eEnglishBacktranslator:
                    return CstrEnglishBackTranslator;
                case UserTypes.eUNS:
                    return CstrUNS;
                case UserTypes.eProjectFacilitator:
                    return CstrProjectFacilitator;
                case UserTypes.eFirstPassMentor:
                    return CstrFirstPassMentor;
                case UserTypes.eConsultantInTraining:
                    return CstrConsultantInTraining;
                case UserTypes.eCoach:
                    return CstrCoach;
                case UserTypes.eJustLooking:
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
                    new XAttribute("name", Name),
                    new XAttribute("memberType", MemberTypeAsString));
                    if (!String.IsNullOrEmpty(Email))
                        eleMember.Add(new XAttribute("email", Email));
                    if (!String.IsNullOrEmpty(AltPhone))
                        eleMember.Add(new XAttribute("altPhone", AltPhone));
                    if (!String.IsNullOrEmpty(Phone))
                        eleMember.Add(new XAttribute("phone", Phone));
                    if (!String.IsNullOrEmpty(BioData))
                        eleMember.Add(new XAttribute("bioData", BioData));
                    if (!String.IsNullOrEmpty(SkypeID))
                        eleMember.Add(new XAttribute("skypeID", SkypeID));
                    if (!String.IsNullOrEmpty(TeamViewerID))
                        eleMember.Add(new XAttribute("teamViewerID", TeamViewerID));
                    eleMember.Add(new XAttribute("memberKey", MemberGuid));

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

        public TeamMembersData(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.StoryProject != null) && (projFile.StoryProject.Count > 0));
            NewDataSet.StoryProjectRow theStoryProjectRow = projFile.StoryProject[0];
            NewDataSet.MembersRow[] aMembersRows = theStoryProjectRow.GetMembersRows();
            NewDataSet.MembersRow theMembersRow;
            if (aMembersRows.Length == 0)
                theMembersRow = projFile.Members.AddMembersRow(theStoryProjectRow);
            else
                theMembersRow = aMembersRows[0];

            foreach (NewDataSet.MemberRow aMemberRow in theMembersRow.GetMemberRows())
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
                        (IsThereASeparateEnglishBackTranslator) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eProjectFacilitator;
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

        public bool IsThereACoach
        {
            get
            {
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eCoach)
                        return true;
                return false;
            }
        }

        public bool IsThereAFirstPassMentor
        {
            get
            {
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eFirstPassMentor)
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
