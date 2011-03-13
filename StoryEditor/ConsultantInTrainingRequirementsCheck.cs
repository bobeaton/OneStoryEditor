using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    class ConsultantInTrainingRequirementsCheck : MentoreeRequirementsCheck
    {
        public readonly bool DoSendToProjectFacilitator;
        public readonly bool DoSendToCoach;

        public ConsultantInTrainingRequirementsCheck(StoryEditor theSe,
            StoryData theStory)
            : base(theSe, theStory)
        {
            DoSendToProjectFacilitator = TasksCit.IsTaskOn(theStory.TasksRequiredCit,
                                                           TasksCit.TaskSettings.SendToProjectFacilitatorForRevision);
            DoSendToCoach = TasksCit.IsTaskOn(theStory.TasksRequiredCit,
                                              TasksCit.TaskSettings.SendToCoachForReview);
        }

        public override bool CheckIfRequirementsAreMet()
        {
            try
            {
                if (DoSendToProjectFacilitator)
                {
                    if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoSendToCoach)
                {
                    if (!CheckEndOfStateTransition.CheckThatCITRespondedToCoachQuestions(TheSe, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                return false;
            }

            return true;
        }
    }
}
