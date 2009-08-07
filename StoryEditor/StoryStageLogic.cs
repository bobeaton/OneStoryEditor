using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class StoryStageLogic
    {
        public LoggedOnMemberInfo MemberInfo;
        public string ProjectStageString = null;
        public string StoryName = null;
        public string StoryGuid = null;

        protected ProjectStages _ProjectStage = ProjectStages.eUndefined;

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

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set
            {
                _ProjectStage = value;
                // for now...
                ProjectStageString = value.ToString().Substring(1); // clip off the initial 'e'
            }
        }

        public StoryStageLogic(string strProjectStage, LoggedOnMemberInfo memberInfo, string strStoryName, string strStoryGuid)
        {
            ProjectStageString = strProjectStage;
            ProjectStage = GetProjectStage(strProjectStage);
            StoryName = strStoryName;
            StoryGuid = strStoryGuid;
            MemberInfo = memberInfo;
        }

        public bool CheckIfProjectTransitionIsAllowed(ProjectStages eNextStage)
        {
            // whether a transition is allowed or not is based on what the current stage is and 
            //  who the user is.
            bool bTransitionAllowed = false;
            switch (MemberInfo.Type)
            {
                case StoryEditor.UserTypes.eJustLooking:    // if just looking, then any transition is allowed (but no edits are).
                    bTransitionAllowed = true;
                    break;
            }

            return bTransitionAllowed;
        }
    }
}
