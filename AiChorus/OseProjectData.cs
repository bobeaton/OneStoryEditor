using System;
using System.Linq;
using System.Collections.Generic;

namespace AiChorus
{
    public class OseProjectData : Dictionary<string, string>
    {
        /// <summary>
        /// Id of the project (required)
        /// </summary>
        public const string KeyProjectName = "ProjectName";

        /// <summary>
        /// from //LanguageInfo[@lang = 'Vernacular']/@name
        /// </summary>
        public const string KeyStoryLanguageName = "StoryLanguageName";

        /// <summary>
        /// from //stories/@SetName
        /// </summary>
        public const string KeyStorySetName = "StorySetName";

        /// <summary>
        /// from //story/@name
        /// </summary>
        public const string KeyStoryName = "StoryName";

        /// <summary>
        /// from //story/CraftingInfo.StoryPurpose/text()
        /// </summary>
        public const string KeyStoryPurpose = "StoryPurpose";

        /// <summary>
        /// from //Member[@memberKey = //story//StateTransition[-1].LoggedInMemberId]/@name
        /// </summary>
        public const string KeyLastEditor = "LastEditor";

        /// <summary>
        /// either StoryProjectFacilitator, StoryConsultant, or StoryCoach depending on whose state it's in 
        /// based on //story/@stage
        /// </summary>
        public const string KeyCurrentEditor = "CurrentEditor";

        /// <summary>
        /// based on //story//StateTransition[-1]/@stageDateTimeStamp
        /// </summary>
        public const string KeyTimeInStage = "TimeInStage";

        /// <summary>
        /// from //Member[@memberKey = //story/CraftingInfo.ProjectFacilitator.memberID]/@name
        /// </summary>
        public const string KeyStoryProjectFacilitator = "ProjectFacilitator";

        /// <summary>
        /// from //Member[@memberKey = //story/CraftingInfo.Consultant.memberID]/@name
        /// </summary>
        public const string KeyStoryConsultant = "Consultant";

        /// <summary>
        /// from //Member[@memberKey = //story/CraftingInfo.Coach.memberID]/@name
        /// </summary>
        public const string KeyStoryCoach = "Coach";

        /// <summary>
        /// from //Member[@memberKey = //story/CraftingInfo.StoryCrafter.memberID]/@name
        /// </summary>
        public const string KeyStoryCrafter = "Crafter";

        /// <summary>
        /// from //story//Verse[not @first and @visible]/Count
        /// </summary>
        public const string KeyNumberOfLines = "NumberOfLines";

        /// <summary>
        /// from //story//Verse[not @first and @visible]//TestQuestion/Count
        /// </summary>
        public const string KeyNumberOfTestQuestions = "NumberOfTestQuestions";

        /// <summary>
        /// based on the sum of the # of words between spaces in:
        ///  //story//Verse[not @first and @visible]/StoryLine[@lang = 'Vernacular']/text()
        /// </summary>
        public const string KeyNumberOfWords = "NumberOfWords";

        public string KeyToString()
        {
            return (this.Any())
                        ? String.Join(",", Keys)
                        : $"{KeyProjectName},{KeyStoryLanguageName},{KeyStorySetName},{KeyStoryName},{KeyStoryPurpose},{KeyLastEditor},{KeyCurrentEditor},{KeyTimeInStage},{KeyStoryProjectFacilitator},{KeyStoryConsultant},{KeyStoryCoach},{KeyStoryCrafter},{KeyNumberOfLines},{KeyNumberOfTestQuestions},{KeyNumberOfWords}";
        }

        public string EditorFromState(string currState)
        {
            string memberName;
            switch (currState)
            {
                case "ProjFacTypeVernacular":
                case "ProjFacTypeNationalBT":
                case "ProjFacTypeInternationalBT":
                case "ProjFacTypeFreeTranslation":
                case "ProjFacAddAnchors":
                case "ProjFacAddStoryQuestions":
                case "ProjFacRevisesBeforeUnsTest":
                case "ProjFacReviseBasedOnRound1Notes":
                case "ProjFacOnlineReview1WithConsultant":
                case "ProjFacReadyForTest1":
                case "ProjFacEnterRetellingOfTest1":
                case "ProjFacEnterAnswersToStoryQuestionsOfTest1":
                case "ProjFacRevisesAfterUnsTest":
                    this.TryGetValue(KeyStoryProjectFacilitator, out memberName);
                    return $"{memberName} (Project Facilitator)";

                case "BackTranslatorTypeInternationalBT":
                case "BackTranslatorTranslateConNotesBeforeUnsTest":
                case "BackTranslatorTranslateConNotes":
                case "BackTranslatorTypeInternationalBTTest1":
                case "BackTranslatorTranslateConNotesAfterUnsTest":
                    return "BackTranslator";

                case "ConsultantCheckNonBiblicalStory":
                case "ConsultantCheckStoryInfo":
                case "ConsultantCheckAnchors":
                case "ConsultantCheckStoryQuestions":
                case "ConsultantCauseRevisionBeforeUnsTest":
                case "ConsultantReviseRound1Notes":
                case "ConsultantCheck2":
                case "ConsultantCauseRevisionAfterUnsTest":
                case "ConsultantFinalCheck":
                    this.TryGetValue(KeyStoryConsultant, out memberName);
                    return $"{memberName} (Consultant)";

                case "FirstPassMentorCheck1":
                case "FirstPassMentorCheck2":
                    return "FirstPassMentor";

                case "CoachReviewRound1Notes":
                case "CoachReviewRound2Notes":
                    this.TryGetValue(KeyStoryCoach, out memberName);
                    return $"{memberName} (Coach)";

                case "TeamComplete":
                case "TeamFinalApproval":
                    return "Any Editor";

                default:
                    return "Unknown!";
            }
        }
    }
}
