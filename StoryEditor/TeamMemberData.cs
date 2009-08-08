using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TeamMemberData
    {
        internal const string cstrCrafter = "Crafter";
        internal const string cstrUNS = "UNS";
        internal const string cstrConsultantInTraining = "ConsultantInTraining";
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

        public static StoryEditor.UserTypes GetMemberType(string strMemberTypeString)
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

        public string MemberTypeAsString
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

        public XElement GetXml
        {
            get
            {
                XElement eleMember = new XElement(StoryEditor.ns + "Member",
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
        // protected TeamMemberData LoggedOn = null;

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
                return (aTMD.MemberTypeAsString == strMemberType);
            }
            return false;
        }

        public XElement GetXml
        {
            get
            {
                XElement eleMembers = new XElement(StoryEditor.ns + "Members");

                foreach (TeamMemberData aMemberData in this.Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }
    }
}
