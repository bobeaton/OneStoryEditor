using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using ECInterfaces;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public class TeamMemberData
    {
        [Flags]
        public enum UserTypes
        {
            Undefined = 0,
            Crafter = 1,
            EnglishBackTranslator = 2,
            UNS = 4,
            ProjectFacilitator = 8,
            FirstPassMentor = 16,   // aka. language specialty review
            ConsultantInTraining = 32,
            IndependentConsultant = 64,
            Coach = 128,
            JustLooking = 256,
            AnyEditor = ProjectFacilitator | 
                            FirstPassMentor |   // LSR
                            ConsultantInTraining |
                            IndependentConsultant |
                            Coach
        }

        public static bool IsUser(UserTypes eTypes, UserTypes eType)
        {
            return ((eTypes & eType) != UserTypes.Undefined);
        }

        // this is used enough places to justify making a short cut
        public bool IsPfAndNotLsr
        {
            get
            {
                return (MemberType & (UserTypes.ProjectFacilitator | UserTypes.FirstPassMentor)) ==
                       UserTypes.ProjectFacilitator;
            }
        }

        // means exactly and only 'any editor' (for TeamComplete and Final states)
        public bool IsAnyEditor
        {
            get { return ((MemberType & UserTypes.AnyEditor) == UserTypes.AnyEditor); }
        }

        /*
        internal const string CstrCrafter = "Crafter";
        internal const string CstrEnglishBackTranslator = "EnglishBackTranslator";
        internal const string CstrUNS = "UNS";
        internal const string CstrProjectFacilitator = "ProjectFacilitator";
        internal const string CstrFirstPassMentor = "FirstPassMentor";
        internal const string CstrConsultantInTraining = "ConsultantInTraining";
        internal const string CstrIndependentConsultant = "IndependentConsultant";
        internal const string CstrCoach = "Coach";
        internal const string CstrJustLooking = "JustLooking"; // gives full access, but no change privileges
        */

        public static string CstrEnglishBackTranslatorDisplay
        {
            get { return Localizer.Str("English Back Translator"); }
        }

        protected static string CstrFirstPassMentorDisplay
        {
            get { return Localizer.Str("Language Specialty Reviewer"); }
        }

        internal static string CstrConsultantInTrainingDisplay
        {
            get { return Localizer.Str("Consultant in Training"); }
        }

        internal static string CstrIndependentConsultantDisplay
        {
            get { return Localizer.Str("Consultant"); }
        }

        internal static string CstrProjectFacilitatorDisplay
        {
            get { return Localizer.Str("Project Facilitator"); }
        }

        private static string CstrCrafterDisplay
        {
            get { return Localizer.Str("Crafter"); }
        }

        private static string CstrUnsDisplay
        {
            get { return Localizer.Str("UNS"); }
        }

        public static string CstrCoachDisplay
        {
            get { return Localizer.Str("Coach"); }
        }

        private static string CstrJustLookingDisplay
        {
            get { return Localizer.Str("Just Looking"); }
        }

        public string Name;
        public UserTypes MemberType = UserTypes.Undefined;
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
        public string OverrideFreeTranslationKeyboard;
        public string OverrideFontNameVernacular;
        public float OverrideFontSizeVernacular;
        public bool OverrideRtlVernacular;
        public string OverrideFontNameNationalBT;
        public float OverrideFontSizeNationalBT;
        public bool OverrideRtlNationalBT;
        public string OverrideFontNameInternationalBT;
        public float OverrideFontSizeInternationalBT;
        public bool OverrideRtlInternationalBT;
        public string OverrideFontNameFreeTranslation;
        public float OverrideFontSizeFreeTranslation;
        public bool OverrideRtlFreeTranslation;
        public string HgUsername;
        public string HgPassword;
        public long DefaultAllowed;
        public long DefaultRequired;

        private string TransliteratorNameVernacular;
        private bool TransliteratorDirectionForwardVernacular;
        private string TransliteratorNameNationalBT;
        private bool TransliteratorDirectionForwardNationalBT;
        private string TransliteratorNameInternationalBt;
        private bool TransliteratorDirectionForwardInternationalBt;
        private string TransliteratorNameFreeTranslation;
        private bool TransliteratorDirectionForwardFreeTranslation;

        private DirectableEncConverter _transliteratorVernacular;
        public DirectableEncConverter TransliteratorVernacular
        {
            get
            {
                return _transliteratorVernacular ??
                       (_transliteratorVernacular = GetTransliterator(TransliteratorNameVernacular,
                                                                   TransliteratorDirectionForwardVernacular));
            }
            set
            {
                SetTransliterator(value,
                                  ref _transliteratorVernacular,
                                  ref TransliteratorNameVernacular,
                                  ref TransliteratorDirectionForwardVernacular);
            }
        }

        private DirectableEncConverter _transliteratorNationalBt;
        public DirectableEncConverter TransliteratorNationalBt
        {
            get
            {
                return _transliteratorNationalBt ??
                       (_transliteratorNationalBt = GetTransliterator(TransliteratorNameNationalBT,
                                                                      TransliteratorDirectionForwardNationalBT));
            }
            set
            {
                SetTransliterator(value,
                                  ref _transliteratorNationalBt,
                                  ref TransliteratorNameNationalBT,
                                  ref TransliteratorDirectionForwardNationalBT);
            }
        }

        private DirectableEncConverter _transliteratorInternationalBt;
        public DirectableEncConverter TransliteratorInternationalBt
        {
            get
            {
                return _transliteratorInternationalBt ??
                       (_transliteratorInternationalBt = GetTransliterator(TransliteratorNameInternationalBt,
                                                                        TransliteratorDirectionForwardInternationalBt));
            }
            set
            {
                SetTransliterator(value,
                                  ref _transliteratorInternationalBt,
                                  ref TransliteratorNameInternationalBt,
                                  ref TransliteratorDirectionForwardInternationalBt);
            }
        }

        private DirectableEncConverter _transliteratorFreeTranslation;
        public DirectableEncConverter TransliteratorFreeTranslation
        {
            get
            {
                return _transliteratorFreeTranslation ??
                       (_transliteratorFreeTranslation = GetTransliterator(TransliteratorNameFreeTranslation,
                                                                        TransliteratorDirectionForwardFreeTranslation));
            }
            set
            {
                SetTransliterator(value,
                                  ref _transliteratorFreeTranslation,
                                  ref TransliteratorNameFreeTranslation,
                                  ref TransliteratorDirectionForwardFreeTranslation);
            }
        }

        private static DirectableEncConverter GetTransliterator(string strName, bool bForward)
        {
            if (!String.IsNullOrEmpty(strName))
                return new DirectableEncConverter(strName,
                                                  bForward,
                                                  NormalizeFlags.None);
            return null;
        }

        private static void SetTransliterator(DirectableEncConverter theNewValue,
            ref DirectableEncConverter theEc, ref string strName, 
            ref bool bDirectionForward)
        {
            theEc = theNewValue;
            if (theNewValue != null)
            {
                strName = theNewValue.Name;
                bDirectionForward = theNewValue.GetEncConverter.DirectionForward;
            }
            else
            {
                strName = null;
            }
        }

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

            if (!theMemberRow.IsOverrideFreeTranslationKeyboardNull())
                OverrideFreeTranslationKeyboard = theMemberRow.OverrideFreeTranslationKeyboard;

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

            if (!theMemberRow.IsOverrideFontNameFreeTranslationNull())
                OverrideFontNameFreeTranslation = theMemberRow.OverrideFontNameFreeTranslation;

            if (!theMemberRow.IsOverrideFontSizeFreeTranslationNull())
                OverrideFontSizeFreeTranslation = theMemberRow.OverrideFontSizeFreeTranslation;

            if (!theMemberRow.IsOverrideRtlFreeTranslationNull())
                OverrideRtlFreeTranslation = theMemberRow.OverrideRtlFreeTranslation;

            if (!theMemberRow.IsHgUsernameNull())
                HgUsername = theMemberRow.HgUsername;

            if (!theMemberRow.IsHgPasswordNull())
            {
                string strEncryptedHgPassword = theMemberRow.HgPassword;
                HgPassword = EncryptionClass.Decrypt(strEncryptedHgPassword);
            }

            // transliterator information for the Vernacular field
            if (!theMemberRow.IsTransliteratorVernacularNull())
                TransliteratorNameVernacular = theMemberRow.TransliteratorVernacular;
            if (!theMemberRow.IsTransliteratorDirectionForwardVernacularNull())
                TransliteratorDirectionForwardVernacular = theMemberRow.TransliteratorDirectionForwardVernacular;

            // transliterator information for the National BT
            if (!theMemberRow.IsTransliteratorNationalBTNull())
                TransliteratorNameNationalBT = theMemberRow.TransliteratorNationalBT;
            if (!theMemberRow.IsTransliteratorDirectionForwardNationalBTNull())
                TransliteratorDirectionForwardNationalBT = theMemberRow.TransliteratorDirectionForwardNationalBT;

            // transliterator information for the International BT
            if (!theMemberRow.IsTransliteratorInternationalBTNull())
                TransliteratorNameInternationalBt = theMemberRow.TransliteratorInternationalBT;
            if (!theMemberRow.IsTransliteratorDirectionForwardInternationalBTNull())
                TransliteratorDirectionForwardInternationalBt = theMemberRow.TransliteratorDirectionForwardInternationalBT;

            // transliterator information for the Free Translation
            if (!theMemberRow.IsTransliteratorFreeTranslationNull())
                TransliteratorNameFreeTranslation = theMemberRow.TransliteratorFreeTranslation;
            if (!theMemberRow.IsTransliteratorDirectionForwardFreeTranslationNull())
                TransliteratorDirectionForwardFreeTranslation = theMemberRow.TransliteratorDirectionForwardFreeTranslation;

            if (IsUser(MemberType, UserTypes.ProjectFacilitator))
            {
                DefaultAllowed =
                    (long)
                    (TasksPf.TaskSettings) Enum.Parse(typeof (TasksPf.TaskSettings), theMemberRow.DefaultTasksAllowed);
                DefaultRequired =
                    (long)
                    (TasksPf.TaskSettings) Enum.Parse(typeof (TasksPf.TaskSettings), theMemberRow.DefaultTasksRequired);
            }
            
            else if (IsUser(MemberType, UserTypes.ConsultantInTraining))
            {
                DefaultAllowed =
                    (long)
                    (TasksCit.TaskSettings) Enum.Parse(typeof (TasksCit.TaskSettings), theMemberRow.DefaultTasksAllowed);
                DefaultRequired =
                    (long)
                    (TasksCit.TaskSettings)
                    Enum.Parse(typeof (TasksCit.TaskSettings), theMemberRow.DefaultTasksRequired);
            }
        }

        public static UserTypes GetMemberType(string strMemberTypeString)
        {
            if (String.IsNullOrEmpty(strMemberTypeString))
                return UserTypes.Undefined;

            return (UserTypes) Enum.Parse(typeof (UserTypes), strMemberTypeString);
            /*
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
            */
        }

        public static UserTypes GetMemberTypeFromDisplayString(string strMemberTypeDisplayString)
        {
            // strMemberTypeDisplayString could be: "Project Facilitator, Crafter, UNS"
            UserTypes eRoles = UserTypes.Undefined;
            var castrDelimiter = new [] {", "};
            string[] astrRoles = strMemberTypeDisplayString.Split(castrDelimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var strRole in astrRoles)
            {
                if (strRole == CstrEnglishBackTranslatorDisplay)
                    eRoles |= UserTypes.EnglishBackTranslator;
                else if (strRole == CstrFirstPassMentorDisplay)
                    eRoles |= UserTypes.FirstPassMentor;
                else if (strRole == CstrConsultantInTrainingDisplay)
                    eRoles |= UserTypes.ConsultantInTraining;
                else if (strRole == CstrIndependentConsultantDisplay)
                    eRoles |= UserTypes.IndependentConsultant;
                else if (strRole == CstrProjectFacilitatorDisplay)
                    eRoles |= UserTypes.ProjectFacilitator;
                else if (strRole == CstrJustLookingDisplay)
                    eRoles |= UserTypes.JustLooking;
                else if (strRole == CstrCrafterDisplay)
                    eRoles |= UserTypes.Crafter;
                else if (strRole == CstrUnsDisplay)
                    eRoles |= UserTypes.UNS;
                else if (strRole == CstrCoachDisplay)
                    eRoles |= UserTypes.Coach;
                else
                {
                    System.Diagnostics.Debug.Assert(false); // shouldn't happen
                    eRoles |= GetMemberType(strRole);
                }
            }

            return eRoles;
        }

        public static string GetMemberTypeAsDisplayString(UserTypes eMemberType)
        {
            var lst = new List<string>();
            if (IsUser(eMemberType, UserTypes.IndependentConsultant))
                lst.Add(CstrIndependentConsultantDisplay);
            if (IsUser(eMemberType, UserTypes.ConsultantInTraining))
                lst.Add(CstrConsultantInTrainingDisplay);
            if (IsUser(eMemberType, UserTypes.EnglishBackTranslator))
                lst.Add(CstrEnglishBackTranslatorDisplay);
            if (IsUser(eMemberType, UserTypes.ProjectFacilitator))
                lst.Add(CstrProjectFacilitatorDisplay);
            if (IsUser(eMemberType, UserTypes.FirstPassMentor))
                lst.Add(CstrFirstPassMentorDisplay);
            if (IsUser(eMemberType, UserTypes.JustLooking))
                lst.Add(CstrJustLookingDisplay);
            if (IsUser(eMemberType, UserTypes.Crafter))
                lst.Add(CstrCrafterDisplay);
            if (IsUser(eMemberType, UserTypes.UNS))
                lst.Add(CstrUnsDisplay);
            if (IsUser(eMemberType, UserTypes.Coach))
                lst.Add(CstrCoachDisplay);

            if (lst.Count == 0)
                return null;

            string strDisplayString = lst[0];
            for (int i = 1; i < lst.Count; i++)
                strDisplayString += ", " + lst[i];
            return strDisplayString;
        }

        public static string GetMemberTypeAsString(UserTypes eMemberType)
        {
            return eMemberType.ToString();
            /*
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
            */
        }

        public static bool GetHgParameters(TeamMemberData loggedOnMember, 
            out string strUsername, out string strPassword)
        {
            if (loggedOnMember == null)
            {
                strUsername = strPassword = null;
                return false;
            }

            strUsername = loggedOnMember.HgUsername;
            strPassword = loggedOnMember.HgPassword;
            return true;
        }

        public static void SetHgParameters(TeamMemberData loggedOnMember,
            string strUsername, string strPassword)
        {
            if (loggedOnMember != null)
            {
                loggedOnMember.HgUsername = strUsername;
                loggedOnMember.HgPassword = strPassword;
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
        public const string CstrAttributeNameOverrideFreeTranslationKeyboard = "OverrideFreeTranslationKeyboard";
        public const string CstrAttributeNameOverrideFontNameVernacular = "OverrideFontNameVernacular";
        public const string CstrAttributeNameOverrideFontSizeVernacular = "OverrideFontSizeVernacular";
        public const string CstrAttributeNameOverrideRtlVernacular = "OverrideRtlVernacular";
        public const string CstrAttributeNameOverrideFontNameNationalBT = "OverrideFontNameNationalBT";
        public const string CstrAttributeNameOverrideFontSizeNationalBT = "OverrideFontSizeNationalBT";
        public const string CstrAttributeNameOverrideRtlNationalBT = "OverrideRtlNationalBT";
        public const string CstrAttributeNameOverrideFontNameInternationalBT = "OverrideFontNameInternationalBT";
        public const string CstrAttributeNameOverrideFontSizeInternationalBT = "OverrideFontSizeInternationalBT";
        public const string CstrAttributeNameOverrideRtlInternationalBT = "OverrideRtlInternationalBT";
        public const string CstrAttributeNameOverrideFontNameFreeTranslation = "OverrideFontNameFreeTranslation";
        public const string CstrAttributeNameOverrideFontSizeFreeTranslation = "OverrideFontSizeFreeTranslation";
        public const string CstrAttributeNameOverrideRtlFreeTranslation = "OverrideRtlFreeTranslation";
        public const string CstrAttributeNameHgUsername = "HgUsername";
        public const string CstrAttributeNameHgPassword = "HgPassword";
        public const string CstrAttributeNameTransliteratorVernacular = "TransliteratorVernacular";
        public const string CstrAttributeNameTransliteratorDirectionForwardVernacular = "TransliteratorDirectionForwardVernacular";
        public const string CstrAttributeNameTransliteratorNationalBT = "TransliteratorNationalBT";
        public const string CstrAttributeNameTransliteratorDirectionForwardNationalBT = "TransliteratorDirectionForwardNationalBT";
        public const string CstrAttributeNameTransliteratorInternationalBT = "TransliteratorInternationalBT";
        public const string CstrAttributeNameTransliteratorDirectionForwardInternationalBT = "TransliteratorDirectionForwardInternationalBT";
        public const string CstrAttributeNameTransliteratorFreeTranslation = "TransliteratorFreeTranslation";
        public const string CstrAttributeNameTransliteratorDirectionForwardFreeTranslation = "TransliteratorDirectionForwardFreeTranslation";
        public const string CstrAttributeNameMemberKey = "memberKey";

        public const string CstrAttributeLabelDefaultTasksAllowed = "DefaultTasksAllowed";
        public const string CstrAttributeLabelDefaultTasksRequired = "DefaultTasksRequired";

        public XElement GetXml
        {
            get
            {
                var eleMember = new XElement(CstrElementLabelMember,
                    new XAttribute(CstrAttributeNameName, Name),
                    new XAttribute(CstrAttributeNameMemberType, GetMemberTypeAsString(MemberType)));
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
                if (!String.IsNullOrEmpty(OverrideFreeTranslationKeyboard))
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideFreeTranslationKeyboard, OverrideFreeTranslationKeyboard));
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

                if (!String.IsNullOrEmpty(OverrideFontNameFreeTranslation))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameOverrideFontNameFreeTranslation, OverrideFontNameFreeTranslation),
                        new XAttribute(CstrAttributeNameOverrideFontSizeFreeTranslation, OverrideFontSizeFreeTranslation));
                }
                if (OverrideRtlFreeTranslation)
                    eleMember.Add(new XAttribute(CstrAttributeNameOverrideRtlFreeTranslation, OverrideRtlFreeTranslation));

                if (!String.IsNullOrEmpty(HgUsername))
                    eleMember.Add(new XAttribute(CstrAttributeNameHgUsername, HgUsername));
                if (!String.IsNullOrEmpty(HgPassword))
                {
                    string strEncryptedHgPassword = EncryptionClass.Encrypt(HgPassword);
                    System.Diagnostics.Debug.Assert(HgPassword == EncryptionClass.Decrypt(strEncryptedHgPassword));
                    eleMember.Add(new XAttribute(CstrAttributeNameHgPassword, strEncryptedHgPassword));
                }

                if (!String.IsNullOrEmpty(TransliteratorNameVernacular))
                {
                    eleMember.Add(
                        new XAttribute(CstrAttributeNameTransliteratorVernacular, TransliteratorNameVernacular),
                        new XAttribute(CstrAttributeNameTransliteratorDirectionForwardVernacular,
                                       TransliteratorDirectionForwardVernacular));
                }

                if (!String.IsNullOrEmpty(TransliteratorNameNationalBT))
                {
                    eleMember.Add(new XAttribute(CstrAttributeNameTransliteratorNationalBT, TransliteratorNameNationalBT),
                                  new XAttribute(CstrAttributeNameTransliteratorDirectionForwardNationalBT,
                                                 TransliteratorDirectionForwardNationalBT));
                }

                if (!String.IsNullOrEmpty(TransliteratorNameInternationalBt))
                {
                    eleMember.Add(new XAttribute(CstrAttributeNameTransliteratorInternationalBT, TransliteratorNameInternationalBt),
                                  new XAttribute(CstrAttributeNameTransliteratorDirectionForwardInternationalBT,
                                                 TransliteratorDirectionForwardInternationalBt));
                }

                if (!String.IsNullOrEmpty(TransliteratorNameFreeTranslation))
                {
                    eleMember.Add(new XAttribute(CstrAttributeNameTransliteratorFreeTranslation, TransliteratorNameFreeTranslation),
                                  new XAttribute(CstrAttributeNameTransliteratorDirectionForwardFreeTranslation,
                                                 TransliteratorDirectionForwardFreeTranslation));
                }

                eleMember.Add(new XAttribute(CstrAttributeNameMemberKey, MemberGuid));

                if (IsUser(MemberType, UserTypes.ProjectFacilitator))
                {
                    eleMember.Add(new XAttribute(CstrAttributeLabelDefaultTasksAllowed, (TasksPf.TaskSettings)DefaultAllowed),
                                  new XAttribute(CstrAttributeLabelDefaultTasksRequired, (TasksPf.TaskSettings)DefaultRequired));
                }
                else if (IsUser(MemberType, UserTypes.ConsultantInTraining))
                {
                    eleMember.Add(new XAttribute(CstrAttributeLabelDefaultTasksAllowed, (TasksCit.TaskSettings)DefaultAllowed),
                                  new XAttribute(CstrAttributeLabelDefaultTasksRequired, (TasksCit.TaskSettings)DefaultRequired));
                }

                return eleMember;
            }
        }

        public void CheckIfThisAnAcceptableEditorForThisStory(StoryData theCurrentStory)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory != null);
            
            // technically speaking the 'just looking' isn't an acceptable editor, but it's handled elsewhere.
            if (IsUser(MemberType, UserTypes.JustLooking))
                return;

            UserTypes eMemberWithEditToken = theCurrentStory.ProjStage.MemberTypeWithEditToken;
            if (IsUser(eMemberWithEditToken, 
                UserTypes.IndependentConsultant | 
                UserTypes.ConsultantInTraining))
            {
                // this is a special case where both the CIT and Independant Consultant 
                //  are acceptable. So show an error if it isn't one of those two.
                if (!IsUser(MemberType, 
                            UserTypes.ConsultantInTraining | 
                            UserTypes.IndependentConsultant))
                {
                    LocalizableMessageBox.Show(String.Format(LoginAsProperUserMessage,
                                                  CstrIndependentConsultantDisplay),
                                    StoryEditor.OseCaption);
                }
            }

            else if (IsUser(eMemberWithEditToken, UserTypes.ProjectFacilitator))
            {
                if (!IsUser(MemberType, UserTypes.ProjectFacilitator))
                {
                    LocalizableMessageBox.Show(String.Format(LoginAsProperUserMessage,
                                                  CstrProjectFacilitatorDisplay),
                                    StoryEditor.OseCaption);
                }
                else if (MemberGuid != theCurrentStory.CraftingInfo.ProjectFacilitator.MemberId)
                {
                    LocalizableMessageBox.Show(NotTheRightProjFacMessage,
                                    StoryEditor.OseCaption);
                }
            }

            else if (!IsUser(MemberType, eMemberWithEditToken))
            {
                LocalizableMessageBox.Show(GetWrongMemberTypeWarning(eMemberWithEditToken),
                                StoryEditor.OseCaption);
            }
        }

        public static string NotTheRightProjFacMessage
        {
            get
            {
                return
                    Localizer.Str(
                        "You will not be able to make any changes to this story, because you are not the Project Facilitator assigned to it. If you are the new Project Facilitator for the story, then click 'Story', 'Story Information' and click the 'Change' link to the right of the line that says 'Project Facilitator' to choose your name to be assigned to it");
            }
        }

        private static string LoginAsProperUserMessage
        {
            get
            {
                return
                    Localizer.Str(
                        "Right now, only a '{0}' can make changes in the Story pane. If you're a {0}, click 'Project', 'Login' to login");
            }
        }

        public void ThrowIfEditIsntAllowed(StoryData theCurrentStory)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory != null);

            if (!IsEditAllowed(theCurrentStory))
                throw WrongMemberTypeException(theCurrentStory.ProjStage.MemberTypeWithEditToken);
        }

        public bool IsEditAllowed(StoryData theStory)
        {
            return IsEditAllowed(theStory.ProjStage.MemberTypeWithEditToken,
                                 theStory.CraftingInfo.ProjectFacilitator.MemberId);
        }

        private bool IsEditAllowed(UserTypes eMemberTypeWithEditToken, 
                                   string strTheStoryPfId)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strTheStoryPfId));

            if (eMemberTypeWithEditToken == UserTypes.AnyEditor)
                return true;

            // if it's a PF, then it has to be the *right* PF
            if (IsUser(eMemberTypeWithEditToken, UserTypes.ProjectFacilitator) &&
                (MemberGuid != strTheStoryPfId))
                return false;

            return (IsUser(MemberType, eMemberTypeWithEditToken));
        }

        public static string GetWrongMemberTypeWarning(UserTypes eMemberTypeWithEditToken)
        {
            if (IsUser(eMemberTypeWithEditToken, UserTypes.ProjectFacilitator))
                return NotTheRightProjFacMessage;

            // rewrite the name so it applies for either type of consultant
            if (IsUser(eMemberTypeWithEditToken, UserTypes.ConsultantInTraining))
                eMemberTypeWithEditToken = UserTypes.IndependentConsultant;

            return String.Format(LoginAsProperUserMessage,
                                 GetMemberTypeAsDisplayString(eMemberTypeWithEditToken));
        }

        public static ApplicationException WrongMemberTypeException(UserTypes eMemberTypeWithEditToken)
        {
            return new ApplicationException(GetWrongMemberTypeWarning(eMemberTypeWithEditToken));
        }

        public void MergeWith(TeamMemberData otherMember)
        {
            if (String.IsNullOrEmpty(Email))
                Email = otherMember.Email;
            if (String.IsNullOrEmpty(SkypeID))
                SkypeID = otherMember.SkypeID;
            if (String.IsNullOrEmpty(TeamViewerID))
                TeamViewerID = otherMember.TeamViewerID;
            if (String.IsNullOrEmpty(Phone))
                Phone = otherMember.Phone;
            if (String.IsNullOrEmpty(AltPhone))
                AltPhone = otherMember.AltPhone;
            if (String.IsNullOrEmpty(BioData))
                BioData = otherMember.BioData;
        }
    }

    public class TeamMembersData : Dictionary<string, TeamMemberData>
    {
        protected const string CstrBrowserMemberName = "Browser";

        public bool HasOutsideEnglishBTer;
        public bool HasLanguageSpecialtyReviewer;
        public bool HasIndependentConsultant;

        public TeamMembersData()
        {
            var aTMD = new TeamMemberData(CstrBrowserMemberName, TeamMemberData.UserTypes.JustLooking,
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
                if (ContainsKey(aMemberRow.name))  // Throw away any duplicates
                {
                    LocalizableMessageBox.Show(String.Format(Localizer.Str("It appears that some other team member has added a duplicate member with the name '{0}', but only one is allowed! The latter one will be discarded"),
                                                  aMemberRow.name), StoryEditor.OseCaption);
                }
                else
                    Add(aMemberRow.name, new TeamMemberData(aMemberRow));

            // if the 'Has...' attributes are new, then get these values from the old method
            if (theMembersRow.IsHasOutsideEnglishBTerNull())
            {
                HasOutsideEnglishBTer = IsThereAnOutsideEnglishBTer;
            }
            else
                HasOutsideEnglishBTer = theMembersRow.HasOutsideEnglishBTer;

            HasLanguageSpecialtyReviewer = (theMembersRow.IsHasFirstPassMentorNull())
                                               ? false
                                               : theMembersRow.HasFirstPassMentor;

            if (theMembersRow.IsHasIndependentConsultantNull())
                HasIndependentConsultant = IsThereAnIndependentConsultant;
            else
                HasIndependentConsultant = theMembersRow.HasIndependentConsultant;
        }

        public string GetNameFromMemberId(string memberId)
        {
            foreach (var aTeamMember in Values.Where(aTeamMember => 
                aTeamMember.MemberGuid == memberId))
            {
                return aTeamMember.Name;
            }
            return CstrBrowserMemberName;   // shouldn't really be able to happen, but return something
        }

        public TeamMemberData GetMemberFromId(string strMemberId)
        {
            string strName = GetNameFromMemberId(strMemberId);
            return !String.IsNullOrEmpty(strName) ? this[strName] : null;
        }

        // should use the StoryProjectData version if outside user
        protected bool IsThereAnOutsideEnglishBTer
        {
            get
            {
                return Values.Any(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.EnglishBackTranslator));
            }
        }

        public bool IsThereACoach
        {
            get
            {
                return Values.Any(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.Coach));
            }
        }

        public bool IsThereAFirstPassMentor
        {
            get
            {
                return Values.Any(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.FirstPassMentor));
            }
        }

        public bool IsThereAnIndependentConsultant
        {
            get
            {
                return Values.Any(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.IndependentConsultant));
            }
        }

        public int CountOfProjectFacilitator
        {
            get
            {
                return Values.Count(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.ProjectFacilitator));
            }
        }

        public string MemberIdOfOneAndOnlyMemberType(TeamMemberData.UserTypes eMemberType)
        {
            if (Values.Count(aTm =>
                             TeamMemberData.IsUser(aTm.MemberType, eMemberType)) == 1)
            {
                return (from aTm in Values
                        where TeamMemberData.IsUser(aTm.MemberType, eMemberType)
                        select aTm.MemberGuid).FirstOrDefault();
            }
            return null;
        }

        public const string CstrElementLabelMembers = "Members";

        public XElement GetXml
        {
            get
            {
                var eleMembers = new XElement(CstrElementLabelMembers,
                    new XAttribute("HasOutsideEnglishBTer", HasOutsideEnglishBTer),
                    new XAttribute("HasFirstPassMentor", HasLanguageSpecialtyReviewer),
                    new XAttribute("HasIndependentConsultant", HasIndependentConsultant));

                foreach (var aMemberData in Values)
                    eleMembers.Add(aMemberData.GetXml);

                return eleMembers;
            }
        }

        public DialogResult ShowEditDialog(TeamMemberData theTeamMember,
            ProjectSettings projSettings)
        {
            string strName = theTeamMember.Name;
            System.Diagnostics.Debug.Assert(ContainsKey(strName));
 
#if !DataDllBuild
            var dlg = new EditMemberForm(theTeamMember, projSettings, false);
            DialogResult res = dlg.UpdateMember();

            // in case the name was changed
            if ((res == DialogResult.Yes) && (strName != theTeamMember.Name))
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
