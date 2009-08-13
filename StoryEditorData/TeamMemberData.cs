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
            eUNS,
            eConsultantInTraining,
            eCoach,
            eJustLooking
        }

        internal const string CstrCrafter = "Crafter";
        internal const string CstrUNS = "UNS";
        internal const string CstrConsultantInTraining = "ConsultantInTraining";
        internal const string CstrCoach = "Coach";
        internal const string CstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        public string Name = null;
        public UserTypes MemberType = UserTypes.eUndefined;
        public string MemberGuid = null;
        public string Email = null;
        public string SkypeID = null;
        public string TeamViewerID = null;
        public string Phone = null;
        public string AltPhone = null;
        public string Address = null;

        public TeamMemberData(string strName, TeamMemberData.UserTypes eMemberType)
        {
            Name = strName;
            MemberType = eMemberType;
            MemberGuid = String.Format("mem-{0}", Guid.NewGuid());
        }

        public static TeamMemberData.UserTypes GetMemberType(string strMemberTypeString)
        {
            if (strMemberTypeString == CstrCrafter)
                return TeamMemberData.UserTypes.eCrafter;
            else if (strMemberTypeString == CstrUNS)
                return TeamMemberData.UserTypes.eUNS;
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

        public static string GetMemberTypeAsString(TeamMemberData.UserTypes eMemberType)
        {
            switch (eMemberType)
            {
                case TeamMemberData.UserTypes.eCrafter:
                    return CstrCrafter;
                case TeamMemberData.UserTypes.eUNS:
                    return CstrUNS;
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
        public TeamMembersData()
        {
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
                XElement eleMembers = new XElement("Members");

                foreach (TeamMemberData aMemberData in this.Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }
    }
}
