using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            eIndependentConsultant,
            eCoach,
            eJustLooking
        }

        internal const string CstrCrafter = "Crafter";
        internal const string CstrEnglishBackTranslator = "EnglishBackTranslator";
        internal const string CstrUNS = "UNS";
        internal const string CstrProjectFacilitator = "ProjectFacilitator";
        internal const string CstrFirstPassMentor = "FirstPassMentor";
        internal const string CstrConsultantInTraining = "ConsultantInTraining";
        internal const string CstrIndependentConsultant = "IndependentConsultant";
        internal const string CstrCoach = "Coach";
        internal const string CstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        protected const string CstrEnglishBackTranslatorDisplay = "English Back Translator";
        protected const string CstrFirstPassMentorDisplay = "First Pass Mentor";
        internal const string CstrConsultantInTrainingDisplay = "Consultant in Training";
        internal const string CstrIndependentConsultantDisplay = "Independent Consultant";
        protected const string CstrProjectFacilitatorDisplay = "Project Facilitator";
        
        public string Name;
        public UserTypes MemberType = UserTypes.eUndefined;
        public string MemberGuid;
        public string Email;
        public string SkypeID;
        public string TeamViewerID;
        public string Phone;
        public string AltPhone;
        public string BioData;
        public string OverrideVernacularKeyboard;
        public string OverrideNationalBTKeyboard;
        public string OverrideInternationalBTKeyboard;
        public string HgUsername;
        public string HgPassword;

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

            if (!theMemberRow.IsOverrideVernacularKeyboardNull())
                OverrideVernacularKeyboard = theMemberRow.OverrideVernacularKeyboard;

            if (!theMemberRow.IsOverrideNationalBTKeyboardNull())
                OverrideNationalBTKeyboard = theMemberRow.OverrideNationalBTKeyboard;

            if (!theMemberRow.IsOverrideInternationalBTKeyboardNull())
                OverrideInternationalBTKeyboard = theMemberRow.OverrideInternationalBTKeyboard;
            
            if (!theMemberRow.IsHgUsernameNull())
                HgUsername = theMemberRow.HgUsername;

            if (!theMemberRow.IsHgPasswordNull())
            {
                string strEncryptedHgPassword = theMemberRow.HgPassword;
                HgPassword = EncryptionClass.Decrypt(strEncryptedHgPassword);
            }
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
            if (strMemberTypeString == CstrIndependentConsultant)
                return UserTypes.eIndependentConsultant;
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
            if (eMemberType == UserTypes.eIndependentConsultant)
                return CstrIndependentConsultantDisplay;
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
                case UserTypes.eIndependentConsultant:
                    return CstrIndependentConsultant;
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
                if (!String.IsNullOrEmpty(OverrideVernacularKeyboard))
                    eleMember.Add(new XAttribute("OverrideVernacularKeyboard", OverrideVernacularKeyboard));
                if (!String.IsNullOrEmpty(OverrideNationalBTKeyboard))
                    eleMember.Add(new XAttribute("OverrideNationalBTKeyboard", OverrideNationalBTKeyboard));
                    if (!String.IsNullOrEmpty(OverrideInternationalBTKeyboard))
                        eleMember.Add(new XAttribute("OverrideInternationalBTKeyboard", OverrideInternationalBTKeyboard));
                if (!String.IsNullOrEmpty(HgUsername))
                    eleMember.Add(new XAttribute("HgUsername", HgUsername));
                if (!String.IsNullOrEmpty(HgPassword))
                {
                    string strEncryptedHgPassword = EncryptionClass.Encrypt(HgPassword);
                    System.Diagnostics.Debug.Assert(HgPassword == EncryptionClass.Decrypt(strEncryptedHgPassword));
                    eleMember.Add(new XAttribute("HgPassword", strEncryptedHgPassword));
                }

                eleMember.Add(new XAttribute("memberKey", MemberGuid));

                return eleMember;
            }
        }
    }

    public class TeamMembersData : Dictionary<string, TeamMemberData>
    {
        protected const string CstrBrowserMemberName = "Browser";

        public bool HasOutsideEnglishBTer;
        public bool HasFirstPassMentor;
        public bool HasIndependentConsultant;

        public TeamMembersData()
        {
            var aTMD = new TeamMemberData(CstrBrowserMemberName, TeamMemberData.UserTypes.eJustLooking,
                "mem-" + Guid.NewGuid(), null, null, null, null, null, null);
            Add(CstrBrowserMemberName, aTMD);
        }

        public TeamMembersData(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.StoryProject != null) && (projFile.StoryProject.Count > 0));
            NewDataSet.StoryProjectRow theStoryProjectRow = projFile.StoryProject[0];
            NewDataSet.MembersRow[] aMembersRows = theStoryProjectRow.GetMembersRows();
            NewDataSet.MembersRow theMembersRow;
            if (aMembersRows.Length == 0)
                theMembersRow = projFile.Members.AddMembersRow(false, false, false, theStoryProjectRow);
            else
                theMembersRow = aMembersRows[0];

            foreach (NewDataSet.MemberRow aMemberRow in theMembersRow.GetMemberRows())
                Add(aMemberRow.name, new TeamMemberData(aMemberRow));

            // if the 'Has...' attributes are new, then get these values from the old method
            if (theMembersRow.IsHasOutsideEnglishBTerNull())
            {
                HasOutsideEnglishBTer = IsThereAnOutsideEnglishBTer;
            }
            else
                HasOutsideEnglishBTer = theMembersRow.HasOutsideEnglishBTer;

            if (theMembersRow.IsHasFirstPassMentorNull())
                HasFirstPassMentor = IsThereAFirstPassMentor;
            else
                HasFirstPassMentor = theMembersRow.HasFirstPassMentor;

            if (theMembersRow.IsHasIndependentConsultantNull())
                HasIndependentConsultant = IsThereAnIndependentConsultant;
            else
                HasIndependentConsultant = theMembersRow.HasIndependentConsultant;
        }

        // should use the StoryProjectData version if outside user
        protected bool IsThereAnOutsideEnglishBTer
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

        public bool IsThereAnIndependentConsultant
        {
            get
            {
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eIndependentConsultant)
                        return true;
                return false;
            }
        }

        public int CountOfProjectFacilitator
        {
            get
            {
                int nCount = 0;
                foreach (TeamMemberData aTM in Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
                        nCount++;
                return nCount;
            }
        }

        public XElement GetXml
        {
            get
            {
                var eleMembers = new XElement("Members",
                    new XAttribute("HasOutsideEnglishBTer", HasOutsideEnglishBTer),
                    new XAttribute("HasFirstPassMentor", HasFirstPassMentor),
                    new XAttribute("HasIndependentConsultant", HasIndependentConsultant));

                foreach (TeamMemberData aMemberData in Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }
    }
}
