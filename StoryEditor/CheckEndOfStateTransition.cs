using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace OneStoryProjectEditor
{
    public class CheckEndOfStateTransition
    {
        /// <summary>
        /// Delegate definition for routines that determine whether the project stage goals are complete
        /// </summary>
        /// <param name="theSE">Needed to be able to do things like show who's logged on and add verse controls</param>
        /// <param name="theProjSettings">Needed for things like punctuation full stops for the various languages, etc.</param>
        /// <param name="theCurrentStory">The data of the story that we're to check</param>
        /// <returns></returns>
        public delegate bool CheckForValidEndOfState(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory);

        public static bool CrafterTypeNationalBT(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeNationalBT' work is finished: Name: {0}", theCurrentStory.StoryName));
            
            // make sure that each verse has only one sentence
            bool bUpdateView = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strFullStop = theProjSettings.NationalBT.FullStop;
                    List<string> lstSentences = GetListOfSentences(aVerseData.NationalBTText, strFullStop);
                    if ((lstSentences == null) || (lstSentences.Count == 0))
                    {
                        theSE.DeleteVerse(aVerseData);
                        bUpdateView = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }
                    else if (lstSentences.Count > 1)
                    {
                        if (MessageBox.Show(String.Format("Verse number '{0}' has multiple sentences. Click Yes to have them separated into their own verses.", nVerseNumber), StoryEditor.CstrCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return false;

                        int nNewVerses = lstSentences.Count;
                        while (nNewVerses-- > 1)
                        {
                            string strSentence = lstSentences[nNewVerses] + strFullStop;
                            theSE.AddNewVerse(nVerseNumber, strSentence);
                        }

                        aVerseData.NationalBTText.SetValue(lstSentences[nNewVerses]);
                        bUpdateView = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }

                    nVerseNumber++;

                    if (bUpdateView)
                    {
                        theSE.InitVerseControls();
                        bUpdateView = false;
                    }
                }
            } while (bUpdateView);

            return true;
        }

        protected static List<string> GetListOfSentences(StringTransfer stParagraph, string strFullStop)
        {
            string strParagraph = stParagraph.ToString();
            if (strParagraph == null)
                return null;

            string[] aStrSentences = strParagraph.Split(strFullStop.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<string> lstStrRet = new List<string>();
            foreach (string str in aStrSentences)
            {
                string strTrimmed = str.Trim();
                if (!String.IsNullOrEmpty(strTrimmed))
                    lstStrRet.Add(strTrimmed);
            }
            return lstStrRet;
        }

        public static bool CrafterTypeInternationalBT(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterAddAnchors(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddAnchors' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterAddStoryQuestions(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddStoryQuestions' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchors(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchors' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryQuestions' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewRound1Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound1Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound1Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview1WithConsultant(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview1WithConsultant' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest1(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest1(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest1(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchorsRound2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchorsRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnswersToTestingQuestionsRound2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnswersToTestingQuestionsRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckRetellingRound2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckRetellingRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewRound2Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound2Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound2Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview2WithConsultant(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview2WithConsultant' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviewTest2(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviewTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewTest2Notes(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewTest2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool TeamComplete(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamComplete' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }
    }
}
