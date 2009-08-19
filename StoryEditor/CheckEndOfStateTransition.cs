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
        public delegate bool CheckForValidEndOfState(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory);

        public static bool CrafterTypeNationalBT(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeNationalBT' work is finished: Name: {0}", theCurrentStory.StoryName));
            
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strFullStop = theStories.ProjSettings.NationalBT.FullStop;
                    List<string> lstSentences = GetListOfSentences(aVerseData.NationalBTText, strFullStop);
                    if ((lstSentences == null) || (lstSentences.Count == 0))
                    {
                        theCurrentStory.Verses.Remove(aVerseData);
                        bRepeatAfterMe = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }
                    
                    if (lstSentences.Count > 1)
                    {
                        if (MessageBox.Show(String.Format("Verse number '{0}' has multiple sentences. Click Yes to have them separated into their own verses.", nVerseNumber),  StoriesData.CstrCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return false;

                        int nNewVerses = lstSentences.Count;
                        while (nNewVerses-- > 1)
                        {
                            string strSentence = lstSentences[nNewVerses] + strFullStop;
                            theCurrentStory.Verses.InsertVerse(nVerseNumber, strSentence); 
                        }

                        aVerseData.NationalBTText.SetValue(lstSentences[nNewVerses] + strFullStop);
                        bRepeatAfterMe = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);

            // finally, we need to know who (which UNS) did the bt.
            while(String.IsNullOrEmpty(theCurrentStory.CraftingInfo.BackTranslatorMemberID))
            {
                MessageBox.Show("In the following window, click on the button to select which UNS did this back-translation",  StoriesData.CstrCaption);
                StoryFrontMatterForm dlg = new StoryFrontMatterForm(theStories, theCurrentStory);
                dlg.Text = String.Format("Choose the UNS that did the {0} language back-translation", theStories.ProjSettings.NationalBT.LangName);
                dlg.ShowDialog();
            } 
            return true;
        }

        public static List<string> GetListOfSentences(StringTransfer stParagraph, string strFullStop)
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

        public static bool CrafterTypeInternationalBT(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.StoryName));
            
            // first do everything that we would have done in the first step (since the user might have changed things around.
            if (!CrafterTypeNationalBT(theSE, theStories, theCurrentStory))
                return false;

            // just in case there were changes
            theSE.InitAllPanes();

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                string strFullStop = theStories.ProjSettings.InternationalBT.FullStop;
                List<string> lstSentences = GetListOfSentences(aVerseData.InternationalBTText, strFullStop);
                if ((lstSentences == null) || (lstSentences.Count == 0))
                {
                    // light it up and let the user know they need to do something!
                    ShowErrorFocus(theSE, aVerseData.InternationalBTText.TextBox, 
                        String.Format("Error: Verse {0} doesn't have any English back-translation in it. Did you forget it?", nVerseNumber));
                    return false;
                }

                if (lstSentences.Count > 1)
                {
                    // light it up and let the user know they need to do something!
                    aVerseData.InternationalBTText.TextBox.Focus();
                    Console.Beep();
                    theSE.SetStatusBar(String.Format("Error: Verse {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}", nVerseNumber, theStories.ProjSettings.NationalBT.LangName), null);
                    return false;
                }

                nVerseNumber++;
            }
            return true;
        }

        protected static void ShowErrorFocus(StoryEditor theSE, CtrlTextBox tb, string strStatusMessage)
        {
            Console.Beep();
            tb.Focus();
            tb.SelectAll();
            theSE.SetStatusBar(strStatusMessage, null);
        }

        public static bool CrafterAddAnchors(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddAnchors' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterAddStoryQuestions(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddStoryQuestions' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchors(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchors' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryQuestions' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound1Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview1WithConsultant(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview1WithConsultant' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest1' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnchorsRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchorsRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckAnswersToTestingQuestionsRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnswersToTestingQuestionsRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantCheckRetellingRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckRetellingRound2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviseRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReviseBasedOnRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterOnlineReview2WithConsultant(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview2WithConsultant' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterReadyForTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool ConsultantReviewTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviewTest2' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool CoachReviewTest2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewTest2Notes' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }

        public static bool TeamComplete(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamComplete' work is finished: Name: {0}", theCurrentStory.StoryName));
            return true;
        }
    }
}
