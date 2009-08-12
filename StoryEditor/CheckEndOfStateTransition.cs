using System;

namespace OneStoryProjectEditor
{
    public class CheckEndOfStateTransition
    {
        public delegate bool CheckForValidEndOfState(StoryData theStoryData);

        public static bool CrafterTypeNationalBT(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterTypeNationalBT: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterTypeInternationalBT(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterTypeInternationalBT: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterAddAnchors(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterAddAnchors: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterAddStoryQuestions(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterAddStoryQuestions: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchors(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantCheckAnchors: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantCheckStoryQuestions: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CoachReviewRound1Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CoachReviewRound1Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound1Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantReviseRound1Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound1Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterReviseBasedOnRound1Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview1WithConsultant(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterOnlineReview1WithConsultant: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest1(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterReadyForTest1: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest1(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterEnterAnswersToStoryQuestionsOfTest1: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest1(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterEnterRetellingOfTest1: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchorsRound2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantCheckAnchorsRound2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnswersToTestingQuestionsRound2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantCheckAnswersToTestingQuestionsRound2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantCheckRetellingRound2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantCheckRetellingRound2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CoachReviewRound2Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CoachReviewRound2Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound2Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantReviseRound2Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound2Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterReviseBasedOnRound2Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview2WithConsultant(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterOnlineReview2WithConsultant: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterReadyForTest2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterEnterAnswersToStoryQuestionsOfTest2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CrafterEnterRetellingOfTest2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool ConsultantReviewTest2(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("ConsultantReviewTest2: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool CoachReviewTest2Notes(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("CoachReviewTest2Notes: Name: {0}", theStoryData.StoryName));
            return true;
        }

        public static bool TeamComplete(StoryData theStoryData)
        {
            Console.WriteLine(String.Format("TeamComplete: Name: {0}", theStoryData.StoryName));
            return true;
        }
    }
}
