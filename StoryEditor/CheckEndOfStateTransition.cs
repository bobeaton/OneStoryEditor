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
        public delegate bool CheckForValidEndOfState(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState);

        public static bool CrafterTypeVernacular(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeVernacular' work is finished: Name: {0}", theCurrentStory.Name));
            
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strSentenceFinalPunct = theStories.ProjSettings.Vernacular.FullStop;
                    List<string> lstSentences;
                    if (!GetListOfSentences(aVerseData.VernacularText, strSentenceFinalPunct, out lstSentences))
                    {
                        if (!aVerseData.HasData)
                        {
                            theCurrentStory.Verses.Remove(aVerseData);
                            bRepeatAfterMe = true;
                            break;  // we have to exit the loop since we've modified the collection
                        }
                    }
                    else if (lstSentences.Count > 1)
                    {
                        if (MessageBox.Show(String.Format("Verse number '{0}' has multiple sentences. Click 'Yes' to have them separated into their own verses.", nVerseNumber),  Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return false;

                        int nNewVerses = lstSentences.Count;
                        while (nNewVerses-- > 1)
                        {
                            string strSentence = lstSentences[nNewVerses];
                            theCurrentStory.Verses.InsertVerse(nVerseNumber, strSentence, null, null); 
                        }

                        aVerseData.VernacularText.SetValue(lstSentences[nNewVerses]);
                        bRepeatAfterMe = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);

            // if it's a biblical story, we need to know the purpose of the story and the resources used.
            if (theCurrentStory.CraftingInfo.IsBiblicalStory &&
                (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.StoryPurpose)
                || String.IsNullOrEmpty(theCurrentStory.CraftingInfo.ResourcesUsed)))
            {
                MessageBox.Show(String.Format("In the following window, type in the purpose of the story (why you have it in your panorama) and list the resources you used to craft the story", Environment.NewLine), Properties.Resources.IDS_Caption);
                theSE.QueryStoryPurpose();
            }

            // finally, if the user isn't doing a national bt, then we need to modify the next state
            if (!theStories.ProjSettings.NationalBT.HasData)
            {
                // otherwise, there has to be an international bt field... unless the vern is English
                if (!theStories.ProjSettings.InternationalBT.HasData)
                {
                    System.Diagnostics.Debug.Assert(theStories.ProjSettings.Vernacular.LangName == "English");
                    eProposedNextState = (theCurrentStory.CraftingInfo.IsBiblicalStory) 
                        ? StoryStageLogic.ProjectStages.eCrafterAddAnchors : StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
                }

                // normally, we'd go to the "crafter enters English BT" state next, but 
                //  if there's a separate English BTr on the team, then we go do anchors 
                //  first (and then Story Qs so that he/she can do the English BT for 
                //  those afterwards as well).
                else if (theCurrentStory.CraftingInfo.IsBiblicalStory)
                {
                    eProposedNextState = (theStories.TeamMembers.IsThereASeparateEnglishBackTranslator) 
                        ? StoryStageLogic.ProjectStages.eCrafterAddAnchors : StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT;
                }
                else
                {
                    eProposedNextState = theStories.TeamMembers.IsThereASeparateEnglishBackTranslator 
                        ? StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT : StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT;
                }
            }

            return true;
        }

        public static bool CrafterTypeNationalBT(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStories.ProjSettings.NationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeNationalBT' work is finished: Name: {0}", theCurrentStory.Name));
            
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strSentenceFinalPunct = theStories.ProjSettings.NationalBT.FullStop;
                    List<string> lstSentences;
                    if (!GetListOfSentences(aVerseData.NationalBTText, strSentenceFinalPunct, out lstSentences))
                    {
                        if (!aVerseData.HasData)
                        {
                            theCurrentStory.Verses.Remove(aVerseData);
                            bRepeatAfterMe = true;
                            break;  // we have to exit the loop since we've modified the collection
                        }
                        
                        if (aVerseData.VernacularText.HasData)
                        {
                            ShowErrorFocus(theSE, aVerseData.NationalBTText.TextBox, String.Format("Error: Verse {0} is missing a back-translation. Did you forget it?", nVerseNumber));
                            return false;
                        }
                    }
                    
                    else if (lstSentences.Count > 1)
                    {
                        if (MessageBox.Show(String.Format("Verse number '{0}' has multiple sentences. Click Yes to have them separated into their own verses.", nVerseNumber),  Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return false;

                        int nNewVerses = lstSentences.Count;
                        while (nNewVerses-- > 1)
                        {
                            string strSentence = lstSentences[nNewVerses];
                            theCurrentStory.Verses.InsertVerse(nVerseNumber, null, strSentence, null); 
                        }

                        aVerseData.NationalBTText.SetValue(lstSentences[nNewVerses]);
                        bRepeatAfterMe = true;
                        break;  // we have to exit the loop since we've modified the collection
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);

            // we need to know who (which UNS) did the BT.
            QueryForUnsBackTranslator(theSE, theStories, theCurrentStory);

            if (theCurrentStory.CraftingInfo.IsBiblicalStory)
            {
                if (theStories.ProjSettings.InternationalBT.HasData
                    && !theStories.TeamMembers.IsThereASeparateEnglishBackTranslator)
                    System.Diagnostics.Debug.Assert(eProposedNextState == 
                        StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT);
                else
                    eProposedNextState = StoryStageLogic.ProjectStages.eCrafterAddAnchors;
            }
            else
            {
                // normally, we'd go to doing anchors next, but if this isn't a biblical story, then
                //  no anchors and we skip right ot the English BT
                // but only if there is an English BT... if not, then we're done
                if (theStories.TeamMembers.IsThereASeparateEnglishBackTranslator)
                    eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
                else if (!theStories.ProjSettings.InternationalBT.HasData)
                    eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
                else
                    System.Diagnostics.Debug.Assert(eProposedNextState == 
                        StoryStageLogic.ProjectStages.eCrafterTypeInternationalBT);
            }

            return true;
        }

        public static bool CrafterTypeInternationalBT(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStories.ProjSettings.InternationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'CrafterTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                string strSentenceFinalPunct = theStories.ProjSettings.InternationalBT.FullStop;
                List<string> lstSentences;
                if ((!GetListOfSentences(aVerseData.InternationalBTText, strSentenceFinalPunct, out lstSentences))
                    || (lstSentences.Count == 0))
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
                    theSE.SetStatusBar(String.Format("Error: Verse {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}", nVerseNumber, theStories.ProjSettings.NationalBT.LangName));
                    return false;
                }

                nVerseNumber++;
            }

            // if there's only an English BT, then we need to know who (which UNS) did the BT.
            if (!theStories.ProjSettings.NationalBT.HasData)
                QueryForUnsBackTranslator(theSE, theStories, theCurrentStory);

            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eCrafterAddAnchors);
            
            return true;
        }

        protected static char[] achQuotes = new char[] { '"', '\'', '\u2018', '\u2019', '\u201B', 
            '\u201C', '\u201d', '\u201E', '\u201F' };
        protected static string CstrSentenceFinalPunctuation = "!?:\n";   // standard ones

        public static bool GetListOfSentences(StringTransfer stParagraph, string strSentenceFinalPunct, out List<string> lstSentences)
        {
            lstSentences = new List<string>();
            
            string strParagraph = stParagraph.ToString();
            if (strParagraph == null)
                return false;

            // split it up based on the sentence final punctuation: (the standard list, plus the list from
            //  the project passed in)
            char[] achSplitOn = (CstrSentenceFinalPunctuation + strSentenceFinalPunct).ToCharArray();
            int nStartIndex = 0, nIndex;
            while ((nStartIndex < strParagraph.Length) && (nIndex = strParagraph.IndexOfAny(achSplitOn, nStartIndex)) != -1)
            {
                if (((nIndex + 1) < strParagraph.Length) && (strParagraph.IndexOfAny(achQuotes, nIndex + 1, 1) != -1))
                    nIndex++;

                string strLine = strParagraph.Substring(nStartIndex, nIndex - nStartIndex + 1).Trim();
                if (!String.IsNullOrEmpty(strLine))
                    lstSentences.Add(strLine);

                nStartIndex = nIndex + 1;
            }

            // if we didn't find any final punctuation...
            if (lstSentences.Count == 0)
            {
                // it may just be that the user forgot it.
                if (!String.IsNullOrEmpty(strParagraph))
                {
                    // this means the user forgot the full stop, so help him/her out and add one.
                    stParagraph.SetValue(strParagraph.Trim() + strSentenceFinalPunct[0]);
                    lstSentences.Add(strParagraph);
                }
                else
                    return false;
            }

            // otherwise, there may have been multiple sentences, but the last one didn't have
            //  a full stop
            else if (nStartIndex < strParagraph.Length)
            {
                string strLine = strParagraph.Substring(nStartIndex).Trim();
                if (!String.IsNullOrEmpty(strLine))
                {
                    // this means the user forgot the full stop, so help him/her out and add one.
                    lstSentences.Add(strLine + strSentenceFinalPunct[0]);
                }
            }

            return (lstSentences.Count > 0);
        }

        protected static void ShowErrorFocus(StoryEditor theSE, CtrlTextBox tb, string strStatusMessage)
        {
            tb.Focus();
            tb.SelectAll();
            ShowError(theSE, strStatusMessage);
        }

        protected static void ShowError(StoryEditor theSE, string strStatusMessage)
        {
            theSE.SetStatusBar(strStatusMessage);
            Console.Beep();
        }

        public static bool CrafterAddAnchors(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddAnchors' work is finished: Name: {0}", theCurrentStory.Name));

            // for each verse, make sure that there is at least one anchor.
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.Anchors.Count == 0)
                {
                    ShowError(theSE, String.Format("Error: Verse {0} doesn't have an anchor. Did you forget it?", nVerseNumber));
                    theSE.FocusOnVerse(nVerseNumber - 1);
                    return false;
                }
                nVerseNumber++;
            }

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eCrafterCheckKeyTerms);

            return true;
        }

        public static bool CrafterCheckKeyTerms(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'CrafterCheckKeyTerms' work is finished: Name: {0}", theCurrentStory.Name));

            // for each verse, make sure that each anchor has had it's key terms checked.
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.Anchors.Count == 0)
                {
                    ShowError(theSE, String.Format("Error: Verse {0} doesn't have an anchor. Did you forget it?", nVerseNumber));
                    theSE.FocusOnVerse(nVerseNumber - 1);
                    return false;
                }

                if (!aVerseData.Anchors.IsKeyTermChecked)
                {
                    ShowError(theSE, String.Format("Verse {0} needs to have its key terms checked. right-click on the anchor bar and choose doesn't 'Edit Key Terms'", nVerseNumber));
                    theSE.FocusOnVerse(nVerseNumber - 1);
                    return false;
                }

                nVerseNumber++;
            }

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eCrafterAddStoryQuestions);

            return true;
        }
        
        public static bool CrafterAddStoryQuestions(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'CrafterAddStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

            // there should be at least half as many questions as there are verses.
            int nNumOfVerses = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
                nNumOfVerses += aVerseData.TestQuestions.Count;

            if (nNumOfVerses < (theCurrentStory.Verses.Count / 2))
            {
                int nNumLacking = (theCurrentStory.Verses.Count / 2) - nNumOfVerses;
                ShowError(theSE,
                    String.Format("Error: You should have at least half as many Story Testing Questions as verses in the story. Please add at least {0} more testing question(s). (right-click on the 'verse options' button and choose 'Add a story testing question')", nNumLacking));
                return false;
            }

            if (!theStories.TeamMembers.IsThereASeparateEnglishBackTranslator)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckAnchors;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT);

            return true;
        }

        public static bool BackTranslatorTypeInternationalBT(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStories.ProjSettings.InternationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                string strSentenceFinalPunct = theStories.ProjSettings.InternationalBT.FullStop;
                List<string> lstSentences;
                if ((!GetListOfSentences(aVerseData.InternationalBTText, strSentenceFinalPunct, out lstSentences))
                    || (lstSentences.Count == 0))
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
                    theSE.SetStatusBar(String.Format("Error: Verse {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}", nVerseNumber, theStories.ProjSettings.NationalBT.LangName));
                    return false;
                }

                nVerseNumber++;
            }

            // if there's only an English BT, then we need to know who (which UNS) did the BT.
            if (!theStories.ProjSettings.NationalBT.HasData)
                QueryForUnsBackTranslator(theSE, theStories, theCurrentStory);

            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheckAnchors);
            
            return true;
        }

        protected static void QueryForUnsBackTranslator(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory)
        {
            while (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.BackTranslatorMemberID))
            {
                if (theCurrentStory.CraftingInfo.IsBiblicalStory)
                {
                    MessageBox.Show("In the following window, click on the browse button to select the 'UNS Back-translator' and add or choose the UNS that did this back-translation.", Properties.Resources.IDS_Caption);
                    StoryFrontMatterForm dlg = new StoryFrontMatterForm(theSE, theStories, theCurrentStory);
                    dlg.Text = "Choose the UNS that did the back-translation";
                    dlg.ShowDialog();
                }
                else
                {
                    MemberPicker dlg = new MemberPicker(theStories, TeamMemberData.UserTypes.eUNS);
                    dlg.Text = "Choose the UNS that did this back-translation";
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;

                    theCurrentStory.CraftingInfo.BackTranslatorMemberID = dlg.SelectedMember.MemberGuid;
                }
            }
        }

        public static bool ConsultantCheckNonBiblicalStory(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckNonBiblicalStory' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnchors(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchors' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it over to the coach, let's make sure that if the crafter had initiated
            //  a conversation, that the consultant answered it.
            int nVerseNumber = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.ConsultantNotes)
                    if ((aConNote.Count > 0) && (aConNote[0].Direction == ConsultNoteDataConverter.CommunicationDirections.eCrafterToConsultant))
                        if ((aConNote.Count == 1) || !aConNote[1].HasData)
                        {
                            ShowErrorFocus(theSE, aConNote[1].TextBox,
                                String.Format("Error: in verse {0}, the Crafter asked a question, which you didn't respond to. Did you forget it?", nVerseNumber));
                            return false;
                        }
                nVerseNumber++;
            }
            return true;
        }

        public static bool CoachReviewRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it back to the consultant, let's make sure that if the consultant had initiated
            //  a conversation, that the coach answered it.
            int nVerseNumber = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.CoachNotes)
                    if ((aConNote.Count > 0) && (aConNote[0].Direction == ConsultNoteDataConverter.CommunicationDirections.eConsultantToCoach))
                        if ((aConNote.Count == 1) || !aConNote[1].HasData)
                        {
                            ShowErrorFocus(theSE, aConNote[1].TextBox,
                                String.Format("Error: in verse {0}, the consultant-in-training asked a question, which you didn't respond to. Did you forget it?", nVerseNumber));
                            return false;
                        }
                nVerseNumber++;
            }
            return true;
        }

        public static bool ConsultantReviseRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it back to the crafter, let's make sure that if the coach had made 
            //  a comment, that the CIT answered it.
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.CoachNotes)
                {
                    int nIndex = aConNote.Count;
                    CommInstance theLastCI = aConNote[nIndex - 1];
                    System.Diagnostics.Debug.Assert(theLastCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eConsultantToCoach);
                    if (!theLastCI.HasData)
                    {
                        ShowErrorFocus(theSE, aConNote[1].TextBox,
                            String.Format("Error: in line {0}, the coach made a comment, which you didn't respond to. Did you forget it?", nVerseNumber));
                        return false;
                    }
                }
                nVerseNumber++;
            }
            return true;
        }

        public static bool CrafterReviseBasedOnRound1Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // let's make sure that if the CIT had made a comment, that the Crafter answered it.
            int nVerseNumber = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.ConsultantNotes)
                {
                    int nIndex = aConNote.Count;
                    CommInstance theLastCI = aConNote[nIndex];
                    System.Diagnostics.Debug.Assert(theLastCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eCrafterToConsultant);
                    if (!theLastCI.HasData)
                    {
                        ShowErrorFocus(theSE, aConNote[1].TextBox,
                            String.Format("Error: in verse {0}, the consultant made a comment, which you didn't respond to. Did you forget it?", nVerseNumber));
                        return false;
                    }
                }
                nVerseNumber++;
            }
            return true;
        }

        public static bool CrafterOnlineReview1WithConsultant(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview1WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterReadyForTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest1(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnchorsRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchorsRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnswersToTestingQuestionsRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnswersToTestingQuestionsRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckRetellingRound2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckRetellingRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CoachReviewRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantReviseRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterReviseBasedOnRound2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReviseBasedOnRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterOnlineReview2WithConsultant(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterOnlineReview2WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterReadyForTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterReadyForTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterEnterAnswersToStoryQuestionsOfTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterAnswersToStoryQuestionsOfTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CrafterEnterRetellingOfTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CrafterEnterRetellingOfTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantReviewTest2(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviewTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CoachReviewTest2Notes(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewTest2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool TeamComplete(StoryEditor theSE, StoriesData theStories, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamComplete' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }
    }
}
