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
            ProjectStage = GetProjectStage(strProjectStage);
            _theSE = theSE;
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

        public static ProjectStages GetProjectStage(string strProjectStageString)
        {
            if (strProjectStageString == "CrafterTypeNationalBT")
                return ProjectStages.eCrafterTypeNationalBT;
            else if (strProjectStageString == "CrafterTypeInternationalBT")
                return ProjectStages.eCrafterTypeInternationalBT;
            else if (strProjectStageString == "CrafterAddAnchors")
                return ProjectStages.eCrafterAddAnchors;
            else if (strProjectStageString == "CrafterAddStoryQuestions")
                return ProjectStages.eCrafterAddStoryQuestions;
            else if (strProjectStageString == "ConsultantAddRound1Notes")
                return ProjectStages.eConsultantAddRound1Notes;
            else if (strProjectStageString == "CoachReviewRound1Notes")
                return ProjectStages.eCoachReviewRound1Notes;
            else if (strProjectStageString == "ConsultantReviseRound1Notes")
                return ProjectStages.eConsultantReviseRound1Notes;
            else if (strProjectStageString == "CrafterReviseBasedOnRound1Notes")
                return ProjectStages.eCrafterReviseBasedOnRound1Notes;
            else if (strProjectStageString == "CrafterOnlineReview1WithConsultant")
                return ProjectStages.eCrafterOnlineReview1WithConsultant;
            else if (strProjectStageString == "CrafterEnterRetellingBTTest1")
                return ProjectStages.eCrafterEnterRetellingBTTest1;
            else if (strProjectStageString == "CrafterEnterStoryQuestionAnswersBTTest1")
                return ProjectStages.eCrafterEnterStoryQuestionAnswersBTTest1;
            else if (strProjectStageString == "ConsultantAddRoundZNotes")
                return ProjectStages.eConsultantAddRoundZNotes;
            else if (strProjectStageString == "CoachReviewRoundZNotes")
                return ProjectStages.eCoachReviewRoundZNotes;
            else if (strProjectStageString == "ConsultantReviseRoundZNotes")
                return ProjectStages.eConsultantReviseRoundZNotes;
            else if (strProjectStageString == "CrafterReviseBasedOnRoundZNotes")
                return ProjectStages.eCrafterReviseBasedOnRoundZNotes;
            else if (strProjectStageString == "CrafterOnlineReviewZWithConsultant")
                return ProjectStages.eCrafterOnlineReviewZWithConsultant;
            else if (strProjectStageString == "CrafterEnterRetellingBTTestZ")
                return ProjectStages.eCrafterEnterRetellingBTTestZ;
            else if (strProjectStageString == "CrafterEnterStoryQuestionAnswersBTTestZ")
                return ProjectStages.eCrafterEnterStoryQuestionAnswersBTTestZ;
            else if (strProjectStageString == "TeamComplete")
                return ProjectStages.eTeamComplete;
            else
                return ProjectStages.eUndefined;  // this version of the app doesn't know about this value
        }

        public override string ToString()
        {
            return _ProjectStage.ToString().Substring(1);
        }
    }
}
