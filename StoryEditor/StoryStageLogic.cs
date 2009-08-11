using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class StoryStageLogic
    {
        protected StoryEditor _theSE = null; // so we can access the logged on user
        protected ProjectStages _ProjectStage = ProjectStages.eUndefined;
        protected const string CstrDefaultProjectStage = "CrafterTypeNationalBT";

        public enum ProjectStages
        {
            eUndefined = 0,
            eCrafterTypeNationalBT,
            eCrafterTypeInternationalBT,
            eCrafterAddAnchors,
            eCrafterAddStoryQuestions,
            eConsultantAddRound1Notes,
            eCoachReviewRound1Notes,
            eConsultantReviseRound1Notes,
            eCrafterReviseBasedOnRound1Notes,
            eCrafterOnlineReview1WithConsultant,
            eCrafterEnterRetellingBTTest1,
            eCrafterEnterStoryQuestionAnswersBTTest1,
            eConsultantAddRoundZNotes,
            eCoachReviewRoundZNotes,
            eConsultantReviseRoundZNotes,
            eCrafterReviseBasedOnRoundZNotes,
            eCrafterOnlineReviewZWithConsultant,
            eCrafterEnterRetellingBTTestZ,
            eCrafterEnterStoryQuestionAnswersBTTestZ,
            eTeamComplete
        }

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set { _ProjectStage = value; }
        }

        public StoryStageLogic(StoryEditor theSE)
        {
            ProjectStage = ProjectStages.eCrafterTypeNationalBT;
            _theSE = theSE;
        }

        public StoryStageLogic(string strProjectStage, StoryEditor theSE)
        {
            ProjectStage = GetProjectStageFromString(strProjectStage);
            _theSE = theSE;
        }

        protected StoryStageLogic.ProjectStages GetProjectStageFromString(string strProjectStageString)
        {
            System.Diagnostics.Debug.Assert(CmapStageStringToEnumType.ContainsKey(strProjectStageString));
            return CmapStageStringToEnumType[strProjectStageString];
        }

        protected TeamMemberData LoggedOnMember
        {
            get { return _theSE.LoggedOnMember; }
        }

        public bool IsEditAllowed
        {
            get
            {
                bool bEditAllowed = false;
                switch (LoggedOnMember.MemberType)
                {
                    case StoryEditor.UserTypes.eJustLooking:
                        string strErrorMessage = "Since you are logged in as \"Just Looking\", you are not allowed to make changes to any of the stories. Click \"Project\", \"Settings\" to login as a different team member.";
                        if (WhoHasTheEditToken != StoryEditor.UserTypes.eUndefined)
                            strErrorMessage += String.Format("{0}Right now, a {1} should be the only one editing the file", Environment.NewLine, TeamMemberData.GetMemberTypeAsString(WhoHasTheEditToken));
                        throw new ApplicationException(strErrorMessage);
                }

                return bEditAllowed;
            }
        }

        // this isn't 100% effective. Sometimes a particular stage can have a single (but varied) editors
        //  (e.g. the Online consult could either be the crafter or the consultant)
        public StoryEditor.UserTypes WhoHasTheEditToken
        {
            get
            {
                StoryEditor.UserTypes eType = StoryEditor.UserTypes.eUndefined;
                switch (_ProjectStage)
                {
                    case StoryStageLogic.ProjectStages.eCrafterTypeNationalBT:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;

                    case StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;

                    case StoryStageLogic.ProjectStages.eCrafterAddAnchors:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;

                    case StoryStageLogic.ProjectStages.eCrafterAddStoryQuestions:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;

                    case StoryStageLogic.ProjectStages.eConsultantAddRound1Notes:
                        eType = StoryEditor.UserTypes.eConsultantInTraining;
                        break;
                    case StoryStageLogic.ProjectStages.eCoachReviewRound1Notes:
                        eType = StoryEditor.UserTypes.eCoach;
                        break;
                    case StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes:
                        eType = StoryEditor.UserTypes.eConsultantInTraining;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRound1Notes:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterOnlineReview1WithConsultant:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTest1:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTest1:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eConsultantAddRoundZNotes:
                        eType = StoryEditor.UserTypes.eConsultantInTraining;
                        break;
                    case StoryStageLogic.ProjectStages.eCoachReviewRoundZNotes:
                        eType = StoryEditor.UserTypes.eCoach;
                        break;
                    case StoryStageLogic.ProjectStages.eConsultantReviseRoundZNotes:
                        eType = StoryEditor.UserTypes.eConsultantInTraining;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRoundZNotes:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterOnlineReviewZWithConsultant:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTestZ:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTestZ:
                        eType = StoryEditor.UserTypes.eCrafter;
                        break;
                    case StoryStageLogic.ProjectStages.eTeamComplete:
                    case StoryStageLogic.ProjectStages.eUndefined:
                    default:
                        break;
                };

                return eType;
            }
        }

        public static bool IsValidTransition(ProjectStages eCurrentStage, ProjectStages eToStage)
        {
            List<StoryStageLogic.ProjectStages> lstAllowable;
            if (CmapAllowableStageTransitions.TryGetValue(eCurrentStage, out lstAllowable))
                return lstAllowable.Contains(eToStage);

            return false;
        }

        /*
        public bool CheckIfProjectTransitionIsAllowed(ProjectStages eNextStage)
        {
            // whether a transition is allowed or not is based on what the current stage is and 
            //  who the user is.
            bool bTransitionAllowed = false;
            switch (LoggedOnMember.MemberType)
            {
                case StoryEditor.UserTypes.eJustLooking:    // if just looking, then any transition is allowed (but no edits are).
                    bTransitionAllowed = true;
                    break;
            }

            return bTransitionAllowed;
        }
        */

        public override string ToString()
        {
            return _ProjectStage.ToString().Substring(1);
        }

        protected Dictionary<string, StoryStageLogic.ProjectStages> CmapStageStringToEnumType = new Dictionary<string, StoryStageLogic.ProjectStages>() {
            { "CrafterTypeNationalBT", ProjectStages.eCrafterTypeNationalBT },
            { "CrafterTypeInternationalBT", ProjectStages.eCrafterTypeInternationalBT },
            { "CrafterAddAnchors", ProjectStages.eCrafterAddAnchors },
            { "CrafterAddStoryQuestions", ProjectStages.eCrafterAddStoryQuestions },
            { "ConsultantAddRound1Notes", ProjectStages.eConsultantAddRound1Notes },
            { "CoachReviewRound1Notes", ProjectStages.eCoachReviewRound1Notes },
            { "ConsultantReviseRound1Notes", ProjectStages.eConsultantReviseRound1Notes },
            { "CrafterReviseBasedOnRound1Notes", ProjectStages.eCrafterReviseBasedOnRound1Notes },
            { "CrafterOnlineReview1WithConsultant", ProjectStages.eCrafterOnlineReview1WithConsultant },
            { "CrafterEnterRetellingBTTest1", ProjectStages.eCrafterEnterRetellingBTTest1 },
            { "CrafterEnterStoryQuestionAnswersBTTest1", ProjectStages.eCrafterEnterStoryQuestionAnswersBTTest1 },
            { "ConsultantAddRoundZNotes", ProjectStages.eConsultantAddRoundZNotes },
            { "CoachReviewRoundZNotes", ProjectStages.eCoachReviewRoundZNotes },
            { "ConsultantReviseRoundZNotes", ProjectStages.eConsultantReviseRoundZNotes },
            { "CrafterReviseBasedOnRoundZNotes", ProjectStages.eCrafterReviseBasedOnRoundZNotes },
            { "CrafterOnlineReviewZWithConsultant", ProjectStages.eCrafterOnlineReviewZWithConsultant },
            { "CrafterEnterRetellingBTTestZ", ProjectStages.eCrafterEnterRetellingBTTestZ },
            { "CrafterEnterStoryQuestionAnswersBTTestZ", ProjectStages.eCrafterEnterStoryQuestionAnswersBTTestZ },
            { "TeamComplete", ProjectStages.eTeamComplete }};

        protected static Dictionary<StoryStageLogic.ProjectStages, List<StoryStageLogic.ProjectStages>> CmapAllowableStageTransitions = new Dictionary<ProjectStages, List<ProjectStages>>()
        {
            { ProjectStages.eCrafterTypeNationalBT, new List<ProjectStages> { ProjectStages.eCrafterTypeInternationalBT, ProjectStages.eCrafterAddAnchors } },
            { ProjectStages.eCrafterTypeInternationalBT, new List<ProjectStages> { ProjectStages.eCrafterTypeNationalBT, ProjectStages.eCrafterAddAnchors } }
        };

        internal static Dictionary<StoryStageLogic.ProjectStages, string> CmapStageToDisplayString = new Dictionary<StoryStageLogic.ProjectStages, string>()
        {
            { StoryStageLogic.ProjectStages.eCrafterTypeNationalBT, "Crafter enters the {0} back-translation" },
            { StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT, "Crafter enters the English back-translation" },
            { StoryStageLogic.ProjectStages.eCrafterAddAnchors, "Crafter adds biblical anchors" },
            { StoryStageLogic.ProjectStages.eCrafterAddStoryQuestions, "Crafter adds story testing questions" },
            { StoryStageLogic.ProjectStages.eConsultantAddRound1Notes, "Consultant adds round 1 exegetical notes" },
            { StoryStageLogic.ProjectStages.eCoachReviewRound1Notes, "Coach reviews round 1 notes" },
            { StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes, "Consultant revises round 1 notes based on Coach's feedback" },
            { StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRound1Notes, "Crafter revises story based on round 1 notes and enters round 1 responses" },
            { StoryStageLogic.ProjectStages.eCrafterOnlineReview1WithConsultant, "Crafter has 1st online review with consultant" },
            { StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTest1, "Crafter enters test 1 retelling back-translation" },
            { StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTest1, "Crafter enters test 1 answers to story questions" },
            { StoryStageLogic.ProjectStages.eConsultantAddRoundZNotes, "Consultant adds round 2 notes" },
            { StoryStageLogic.ProjectStages.eCoachReviewRoundZNotes, "Coach reviews round 2 notes" },
            { StoryStageLogic.ProjectStages.eConsultantReviseRoundZNotes, "Consultant revises round 2 notes based on Coach's feedback" },
            { StoryStageLogic.ProjectStages.eCrafterReviseBasedOnRoundZNotes, "Crafter revises story based on round 2 notes and enters round 2 responses" },
            { StoryStageLogic.ProjectStages.eCrafterOnlineReviewZWithConsultant, "Crafter has 2nd online review with consultant" },
            { StoryStageLogic.ProjectStages.eCrafterEnterRetellingBTTestZ, "Crafter enters test 2 retelling back-translation" },
            { StoryStageLogic.ProjectStages.eCrafterEnterStoryQuestionAnswersBTTestZ, "Crafter enters test 2 answers to story questions" },
            { StoryStageLogic.ProjectStages.eTeamComplete, "Story testing complete (waiting for panorama completion review)" }};
    }
}
