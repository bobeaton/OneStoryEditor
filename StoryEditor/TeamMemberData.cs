using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TeamMemberData
    {
        [Flags]
        public enum UserTypes
        {
            eUndefined = 0,
            eCrafter = 1,
            eEnglishBacktranslator = 2,
            eUNS = 4,
            eProjectFacilitator = 8,
            eFirstPassMentor = 16,
            eConsultantInTraining = 32,
            eIndependentConsultant = 64,
            eCoach = 128,
            eJustLooking = 256
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
        internal const string CstrIndependentConsultantDisplay = "Consultant";
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
        public string OverrideFontNameVernacular;
        public float OverrideFontSizeVernacular;
        public bool OverrideRtlVernacular;
        public string OverrideFontNameNationalBT;
        public float OverrideFontSizeNationalBT;
        public bool OverrideRtlNationalBT;
        public string OverrideFontNameInternationalBT;
        public float OverrideFontSizeInternationalBT;
        public bool OverrideRtlInternationalBT;
        public string HgUsername;
        public string HgPassword;
        public string TransliteratorVernacular;
        public bool TransliteratorDirectionForwardVernacular;
        public string TransliteratorNationalBT;
        public bool TransliteratorDirectionForwardNationalBT;

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

        protected string GetAttribute(XmlNode node, string strAttrName)
        {
            XmlAttribute attr = node.Attributes[strAttrName];
            if (attr != null)
                return attr.Value;
            return null;
        }

        public TeamMemberData(XmlNode node)
        {
            Name = GetAttribute(node, CstrAttributeNameName);
            MemberType = GetMemberType(GetAttribute(node, CstrAttributeNameMemberType));
            MemberGuid = GetAttribute(node, CstrAttributeNameMemberKey);

            // I could do the rest, but I know that when this version of the ctor is call 
            //  (e.g. history diffing/print preview), I'm only interested in the mapping 
            //  between MemberGuid and Name... so for now, I'm ignoring the rest
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

            if (!theMemberRow.IsOverrideFontNameVernacularNull())
                OverrideFontNameVernacular = theMemberRow.OverrideFontNameVernacular;

            if (!theMemberRow.IsOverrideFontSizeVernacularNull())
                OverrideFontSizeVernacular = theMemberRow.OverrideFontSizeVernacular;

            if (!theMemberRow.IsOverrideRtlVernacularNull())
                OverrideRtlVernacular = theMemberRow.OverrideRtlVernacular;

            if (!theMemberRow.IsOverrideFontNameNationalBTNull())
                OverrideFontNameNationalBT = theMemberRow.OverrideFontNameNationalBT;

            if (!theMemberRow.IsOverrideFontSizeNationalBTNull())
                OverrideFontSizeNationalBT = theMemberRow.OverrideFontSizeNationalBT;

            if (!theMemberRow.IsOverrideRtlNationalBTNull())
                OverrideRtlNationalBT = theMemberRow.OverrideRtlNationalBT;

            if (!theMemberRow.IsOverrideFontNameInternationalBTNull())
                OverrideFontNameInternationalBT = theMemberRow.OverrideFontNameInternationalBT;

            if (!theMemberRow.IsOverrideFontSizeInternationalBTNull())
                OverrideFontSizeInternationalBT = theMemberRow.OverrideFontSizeInternationalBT;

            if (!theMemberRow.IsOverrideRtlInternationalBTNull())
                OverrideRtlInternationalBT = theMemberRow.OverrideRtlInternationalBT;

            if (!theMemberRow.IsHgUsernameNull())
                HgUsername = theMemberRow.HgUsername;

            if (!theMemberRow.IsHgPasswordNull())
            {
                string strEncryptedHgPassword = theMemberRow.HgPassword;
                HgPassword = EncryptionClass.Decrypt(strEncryptedHgPassword);
            }

            if (!theMemberRow.IsTransliteratorVernacularNull())
                TransliteratorVernacular = theMemberRow.TransliteratorVernacular;
            if (!theMemberRow.IsTransliteratorDirectionForwardVernacularNull())
                TransliteratorDirectionForwardVernacular = theMemberRow.TransliteratorDirectionForwardVernacular;

            if (!theMemberRow.IsTransliteratorNationalBTNull())
                TransliteratorNationalBT = theMemberRow.TransliteratorNationalBT;
            if (!theMemberRow.IsTransliteratorDirectionForwardNationalBTNull())
                TransliteratorDirectionForwardNationalBT = theMemberRow.TransliteratorDirectionForwardNationalBT;
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
            if ((eMemberType & UserTypes.eIndependentConsultant) == UserTypes.eIndependentConsultant)
                return CstrIndependentConsultantDisplay;
            if ((eMemberType & UserTypes.eConsultantInTraining) == UserTypes.eConsultantInTraining)
                return CstrConsultantInTrainingDisplay;
            if ((eMemberType & UserTypes.eEnglishBacktranslator) == UserTypes.eEnglishBacktranslator)
                return CstrEnglishBackTranslatorDisplay;
            if ((eMemberType & UserTypes.eProjectFacilitator) == UserTypes.eProjectFacilitator)
                return CstrProjectFacilitatorDisplay;
            if ((eMemberType & UserTypes.eFirstPassMentor) == UserTypes.eFirstPassMentor)
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

        public const string CstrElementLabelMember = "Member";
        public const string CstrAttributeNameName = "name";
        public const string CstrAttributeNameMemberType = "memberType";
        public const string CstrAttributeNameEmail = "email";
        public const string CstrAttributeNameAltPhone = "altPhone";
        public const string CstrAttributeNamePhone = "phone";
        public const string CstrAttributeNameBioData = "bioData";
        public const string CstrAttributeNameSkypeID = "skypeID";
        public const string CstrAttributeNameTeamViewerID = "teamViewerID";
        public const string CstrAttributeNameOverrideVernacularKeyboard = "OverrideVernacularKeyboard";
        public const string CstrAttributeNameOverrideNationalBTKeyboard = "OverrideNationalBTKeyboard";
        public const string CstrAttributeNameOverrideInternationalBTKeyboard = "OverrideInternationalBTKeyboard";
        public const string CstrAttributeNameOverrideFontNameVernacular = "OverrideFontNameVernacular";
        public const string CstrAttributeNameOverrideFontSizeVernacular = "OverrideFontSizeVernacular";
        public const string CstrAttributeNameOverrideRtlVernacular = "OverrideRtlVernacular";
        public const string CstrAttributeNameOverrideFontNameNationalBT = "OverrideFontNameNationalBT";
        public const string CstrAttributeNameOverrideFontSizeNationalBT = "OverrideFontSizeNationalBT";
        public const string CstrAttributeNameOverrideRtlNationalBT = "OverrideRtlNationalBT";
        public const string CstrAttributeNameOverrideFontNameInternationalBT = "OverrideFontNameInternationalBT";
        public const string CstrAttributeNameOverrideFontSizeInternationalBT = "OverrideFontSizeInternationalBT";
        public const string CstrAttributeNameOverrideRtlInternationalBT = "OverrideRtlInternationalBT";
        public const string CstrAttributeNameHgUsername = "HgUsername";
        public const string CstrAttributeNameHgPassword = "HgPassword";
        public const string CstrAttributeNameTransliteratorVernacular = "TransliteratorVernacular";
        public const string CstrAttributeNameTransliteratorDirectionForwardVernacular = "TransliteratorDirectionForwardVernacular";
        public const string CstrAttributeNameTransliteratorNationalBT = "TransliteratorNationalBT";
        public const string CstrAttributeNameTransliteratorDirectionForwardNationalBT = "TransliteratorDirectionForwardNationalBT";
        public const string CstrAttributeNameMemberKey = "memberKey";

        public XElement GetXml
        {
            get
            {
                XElement eleMember = new XElement(CstrElementLabelMember,
                    new XAttribute(CstrAttributeNameName, Name),
                    new XAttribute(CstrAttributeNameMemberType, MemberTypeAsString));
                if (!String.IsNullOrEmpty(Email))
                    eleMember.Add(new XAttribute(CstrAttributeNameEmail, Email));
                if (!String.IsNullOrEmpty(AltPhone))
                    eleMember.Add(new XAttribute(CstrAttributeNameAltPhone, AltPhone));
                if (!String.IsNullOrEmpty(Phone))
                    eleMember.Add(new XAttribute(CstrAttributeNamePhone, Phone));
                if (!String.IsNullOrEmpty(BioData))
                    eleMember.Add(new XAttribute(CstrAttributeNameBioData, BioData));
                if (!String.IsNullOrEmpty(SkypeID))
                    eleMember.Add(new XAttribute(CstrAttributeNameSkypeID, SkypeID));
                if (!String.IsNullOrEmpty(TeamViewerID))
                    eleMember.Add(new XAttribute(CstrAttributeNameTeamViewerID, TeamViewerID));
                if (!String.IsNullOrEmpty(OverrideVernacularKeyboard))
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideVernacularKeyboard, OverrideVernacularKeyboard));
                if (!String.IsNullOrEmpty(OverrideNationalBTKeyboard))
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideNationalBTKeyboard, OverrideNationalBTKeyboard));
                if (!String.IsNullOrEmpty(OverrideInternationalBTKeyboard))
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideInternationalBTKeyboard, OverrideInternationalBTKeyboard));
                if (!String.IsNullOrEmpty(OverrideFontNameVernacular))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameOverrideFontNameVernacular, OverrideFontNameVernacular),
                        new XAttribute(CstrAttributeNameOverrideFontSizeVernacular, OverrideFontSizeVernacular));
                }
                if (OverrideRtlVernacular)
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideRtlVernacular, OverrideRtlVernacular));

                if (!String.IsNullOrEmpty(OverrideFontNameNationalBT))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameOverrideFontNameNationalBT, OverrideFontNameNationalBT),
                        new XAttribute(CstrAttributeNameOverrideFontSizeNationalBT, OverrideFontSizeNationalBT));
                }
                if (OverrideRtlNationalBT)
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideRtlNationalBT, OverrideRtlNationalBT));

                if (!String.IsNullOrEmpty(OverrideFontNameInternationalBT))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameOverrideFontNameInternationalBT, OverrideFontNameInternationalBT),
                        new XAttribute(CstrAttributeNameOverrideFontSizeInternationalBT, OverrideFontSizeInternationalBT));
                }
                if (OverrideRtlInternationalBT)
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideRtlInternationalBT, OverrideRtlInternationalBT));

                if (!String.IsNullOrEmpty(HgUsername))
                    eleMember.Add(new XAttribute(CstrAttributeNameHgUsername, HgUsername));
                if (!String.IsNullOrEmpty(HgPassword))
                {
                    string strEncryptedHgPassword = EncryptionClass.Encrypt(HgPassword);
                    System.Diagnostics.Debug.Assert(HgPassword == EncryptionClass.Decrypt(strEncryptedHgPassword));
                    eleMember.Add(new XAttribute(CstrAttributeNameHgPassword, strEncryptedHgPassword));
                }

                if (!String.IsNullOrEmpty(TransliteratorVernacular))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameTransliteratorVernacular, TransliteratorVernacular),
                        new XAttribute(CstrAttributeNameTransliteratorDirectionForwardVernacular,
                                       TransliteratorDirectionForwardVernacular));
                }
                if (!String.IsNullOrEmpty(TransliteratorNationalBT))
                {
                    eleMember.Add(new XAttribute(CstrAttributeNameTransliteratorNationalBT, TransliteratorNationalBT),
                                  new XAttribute(CstrAttributeNameTransliteratorDirectionForwardNationalBT,
                                                 TransliteratorDirectionForwardNationalBT));
                }

                eleMember.Add(new XAttribute(CstrAttributeNameMemberKey, MemberGuid));

                return eleMember;
            }
        }

        public void CheckIfThisAnAcceptableEditorForThisStory(StoryData theCurrentStory)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory != null);
            
            // technically speaking the 'just looking' isn't an acceptable editor, but it's handled elsewhere.
            if (MemberType == UserTypes.eJustLooking)
                return;

            UserTypes eMemberWithEditToken = theCurrentStory.ProjStage.MemberTypeWithEditToken;
            switch(eMemberWithEditToken)
            {
                case UserTypes.eIndependentConsultant:
                case UserTypes.eConsultantInTraining:
                    // this is a special case where both the CIT and Independant Consultant are acceptable
                    if ((MemberType != UserTypes.eConsultantInTraining)
                        && (MemberType != UserTypes.eIndependentConsultant))
                    {
                        MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_WhichUserEdits,
                            CstrIndependentConsultantDisplay), OseResources.Properties.Resources.IDS_Caption);
                    }
                    break;

                case UserTypes.eProjectFacilitator:
                    if (MemberType != UserTypes.eProjectFacilitator)
                    {
                        MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_WhichUserEdits,
                            CstrProjectFacilitatorDisplay), OseResources.Properties.Resources.IDS_Caption);
                    }
                    else if ((MemberGuid != theCurrentStory.CraftingInfo.ProjectFacilitatorMemberID)
                        && !String.IsNullOrEmpty(theCurrentStory.CraftingInfo.ProjectFacilitatorMemberID))
                    {
                        MessageBox.Show(OseResources.Properties.Resources.IDS_NotTheRightProjFac,
                                OseResources.Properties.Resources.IDS_Caption);
                    }
                    break;

                default:
                    if (MemberType != eMemberWithEditToken)
                    {
                        MessageBox.Show(GetWrongMemberTypeWarning(eMemberWithEditToken),
                                        OseResources.Properties.Resources.IDS_Caption);
                    }
                    break;
            }
        }

        public void ThrowIfEditIsntAllowed(UserTypes eMemberTypeWithEditToken)
        {
            if (!IsEditAllowed(eMemberTypeWithEditToken))
                throw WrongMemberTypeException(eMemberTypeWithEditToken);
        }

        public bool IsEditAllowed(UserTypes eMemberTypeWithEditToken)
        {
            switch (eMemberTypeWithEditToken)
            {
                case UserTypes.eConsultantInTraining:
                case UserTypes.eIndependentConsultant:
                    // special case where either role is allowed to edit
                    return ((MemberType == UserTypes.eConsultantInTraining)
                        || (MemberType == UserTypes.eIndependentConsultant));

                default:
                    return (MemberType == eMemberTypeWithEditToken);
            }
        }

        public static string GetWrongMemberTypeWarning(UserTypes eMemberTypeWithEditToken)
        {
            // rewrite the name so it applies for either type of consultant
            if (eMemberTypeWithEditToken == UserTypes.eConsultantInTraining)
                eMemberTypeWithEditToken = UserTypes.eIndependentConsultant;
            return String.Format(OseResources.Properties.Resources.IDS_WhichUserEdits,
                                 GetMemberTypeAsDisplayString(eMemberTypeWithEditToken));
        }

        public static ApplicationException WrongMemberTypeException(UserTypes eMemberTypeWithEditToken)
        {
            return new ApplicationException(GetWrongMemberTypeWarning(eMemberTypeWithEditToken));
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

        public TeamMembersData(XmlNode node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(CstrElementLabelMembers + '/' + TeamMemberData.CstrElementLabelMember);
            if (list == null)
                return;

            foreach (XmlNode nodeMember in list)
            {
                TeamMemberData aTM = new TeamMemberData(nodeMember);
                Add(aTM.Name, aTM);
            }
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

        public string GetNameFromMemberId(string memberID)
        {
            foreach (TeamMemberData aTeamMember in Values)
                if (aTeamMember.MemberGuid == memberID)
                    return aTeamMember.Name;
            return CstrBrowserMemberName;   // shouldn't really be able to happen, but return something
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

        public const string CstrElementLabelMembers = "Members";

        public XElement GetXml
        {
            get
            {
                var eleMembers = new XElement(CstrElementLabelMembers,
                    new XAttribute("HasOutsideEnglishBTer", HasOutsideEnglishBTer),
                    new XAttribute("HasFirstPassMentor", HasFirstPassMentor),
                    new XAttribute("HasIndependentConsultant", HasIndependentConsultant));

                foreach (TeamMemberData aMemberData in Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }

        public DialogResult ShowEditDialog(TeamMemberData theTeamMember)
        {
            string strName = theTeamMember.Name;
            System.Diagnostics.Debug.Assert(ContainsKey(strName));
 
#if !DataDllBuild
            var dlg = new EditMemberForm(theTeamMember);
            DialogResult res = dlg.UpdateMember();

            // in case the name was changed
            if ((res == DialogResult.OK) && (strName != theTeamMember.Name))
            {
                Remove(strName);
                Add(theTeamMember.Name, theTeamMember);
            }

            return res;
#else
            return DialogResult.OK;
#endif
        }
    }
}
