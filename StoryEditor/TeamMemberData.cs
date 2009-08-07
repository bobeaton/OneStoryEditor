using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TeamMemberData
    {
        internal const string cstrCrafter = "Crafter";
        internal const string cstrUNS = "UNS";
        internal const string cstrConsultantInTraining = "Consultant-in-Training";
        internal const string cstrIndependentConsultant = "IndependentConsultant";
        internal const string cstrCoach = "Coach";
        internal const string cstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        public string Name = null;
        public StoryEditor.UserTypes MemberType = StoryEditor.UserTypes.eUndefined;
        public string MemberGuid = null;
        public string Email = null;
        public string SkypeID = null;
        public string TeamViewerID = null;
        public string Phone = null;
        public string AltPhone = null;
        public string Address = null;

        public TeamMemberData(string strName, StoryEditor.UserTypes eMemberType, string strMemberGuid, string strEmail, string strSkypeID, string strTeamViewerID, string strPhone, string strAltPhone, string strAddress)
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
            MemberType = GetUserType(theMemberRow.memberType);
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

        public static StoryEditor.UserTypes GetUserType(string strMemberTypeString)
        {
            if (strMemberTypeString == cstrCrafter)
                return StoryEditor.UserTypes.eCrafter;
            else if (strMemberTypeString == cstrUNS)
                return StoryEditor.UserTypes.eUNS;
            else if (strMemberTypeString == cstrConsultantInTraining)
                return StoryEditor.UserTypes.eConsultantInTraining;
            else if (strMemberTypeString == cstrIndependentConsultant)
                return StoryEditor.UserTypes.eIndependentConsultant;
            else if (strMemberTypeString == cstrCoach)
                return StoryEditor.UserTypes.eCoach;
            else if (strMemberTypeString == cstrJustLooking)
                return StoryEditor.UserTypes.eJustLooking;
            else
                return StoryEditor.UserTypes.eUndefined;
        }

        public string UserTypeAsString
        {
            get
            {
                switch (MemberType)
                {
                    case StoryEditor.UserTypes.eCrafter:
                        return cstrCrafter;
                    case StoryEditor.UserTypes.eUNS:
                        return cstrUNS;
                    case StoryEditor.UserTypes.eConsultantInTraining:
                        return cstrConsultantInTraining;
                    case StoryEditor.UserTypes.eIndependentConsultant:
                        return cstrIndependentConsultant;
                    case StoryEditor.UserTypes.eCoach:
                        return cstrCoach;
                    case StoryEditor.UserTypes.eJustLooking:
                        return cstrJustLooking;
                    default:
                        System.Diagnostics.Debug.Assert(false); // shouldn't get here
                        return null;
                }
            }
        }
    }

    public class TeamMembersData : Dictionary<string, TeamMemberData>
    {
        public TeamMemberData LoggedOn = null;

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
            if (this.ContainsKey(strMemberName))
            {
                TeamMemberData aTMD = this[strMemberName];
                if (aTMD.UserTypeAsString == strMemberType)
                {
                    LoggedOn = aTMD;
                    return true;
                }
            }
            return false;
        }
    }
}
