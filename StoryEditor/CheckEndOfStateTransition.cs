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
        public delegate bool CheckForValidEndOfState(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState);

        public static bool ProjFacTypeVernacular(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeVernacular' work is finished: Name: {0}", theCurrentStory.Name));
            
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strSentenceFinalPunct = theStoryProjectData.ProjSettings.Vernacular.FullStop;
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
            if (!theStoryProjectData.ProjSettings.NationalBT.HasData)
            {
                // otherwise, there has to be an international bt field... unless the vern is English
                if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                {
                    System.Diagnostics.Debug.Assert(theStoryProjectData.ProjSettings.Vernacular.LangName == "English");
                    eProposedNextState = (theCurrentStory.CraftingInfo.IsBiblicalStory) 
                        ? StoryStageLogic.ProjectStages.eProjFacAddAnchors : StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
                }

                // normally, we'd go to the "Project Facilitator enters English BT" state next, but 
                //  if there's a separate English BTr on the team, then we go do anchors 
                //  first (and then Story Qs so that he/she can do the English BT for 
                //  those afterwards as well).
                else if (theCurrentStory.CraftingInfo.IsBiblicalStory)
                {
                    eProposedNextState = (theStoryProjectData.TeamMembers.IsThereASeparateEnglishBackTranslator) 
                        ? StoryStageLogic.ProjectStages.eProjFacAddAnchors : StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT;
                }
                else
                {
                    eProposedNextState = theStoryProjectData.TeamMembers.IsThereASeparateEnglishBackTranslator 
                        ? StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT : StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT;
                }
            }

            return true;
        }

        public static bool ProjFacTypeNationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStoryProjectData.ProjSettings.NationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeNationalBT' work is finished: Name: {0}", theCurrentStory.Name));
            
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    string strSentenceFinalPunct = theStoryProjectData.ProjSettings.NationalBT.FullStop;
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
            QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

            if (theCurrentStory.CraftingInfo.IsBiblicalStory)
            {
                if (theStoryProjectData.ProjSettings.InternationalBT.HasData
                    && !theStoryProjectData.TeamMembers.IsThereASeparateEnglishBackTranslator)
                    System.Diagnostics.Debug.Assert(eProposedNextState == 
                        StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT);
                else
                    eProposedNextState = StoryStageLogic.ProjectStages.eProjFacAddAnchors;
            }
            else
            {
                // normally, we'd go to doing anchors next, but if this isn't a biblical story, then
                //  no anchors and we skip right ot the English BT
                // but only if there is an English BT... if not, then we're done
                if (theStoryProjectData.TeamMembers.IsThereASeparateEnglishBackTranslator)
                    eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
                else if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                    eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
                else
                    System.Diagnostics.Debug.Assert(eProposedNextState == 
                        StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT);
            }

            return true;
        }

        public static bool ProjFacTypeInternationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStoryProjectData.ProjSettings.InternationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                string strSentenceFinalPunct = theStoryProjectData.ProjSettings.InternationalBT.FullStop;
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
                    theSE.SetStatusBar(String.Format("Error: Verse {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}", nVerseNumber, theStoryProjectData.ProjSettings.NationalBT.LangName));
                    return false;
                }

                nVerseNumber++;
            }

            // if there's only an English BT, then we need to know who (which UNS) did the BT.
            if (!theStoryProjectData.ProjSettings.NationalBT.HasData)
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eProjFacAddAnchors);
            
            return true;
        }

        protected static char[] achQuotes = new char[] { '"', '\'', '\u2018', '\u2019', '\u201B', 
            '\u201C', '\u201d', '\u201E', '\u201F' };

        public static bool GetListOfSentences(StringTransfer stParagraph, string strSentenceFinalPunct, out List<string> lstSentences)
        {
            lstSentences = new List<string>();
            
            string strParagraph = stParagraph.ToString();
            if (strParagraph == null)
                return false;

            // split it up based on the sentence final punctuation: (the standard list, plus the list from
            //  the project passed in)
            char[] achSplitOn = strSentenceFinalPunct.ToCharArray();
            int nStartIndex = 0, nIndex;
            while ((nStartIndex < strParagraph.Length) && (nIndex = strParagraph.IndexOfAny(achSplitOn, nStartIndex)) != -1)
            {
                if (((nIndex + 1) < strParagraph.Length) && (strParagraph.IndexOfAny(achQuotes, nIndex + 1, 1) != -1))
                    nIndex++;

                string strLine = strParagraph.Substring(nStartIndex, nIndex - nStartIndex + 1).Trim();
                strLine = strLine.Replace("\r\n", " ");
                while (strLine.IndexOf("  ") != -1)
                    strLine = strLine.Replace("  ", " ");
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

        public static bool ProjFacAddAnchors(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'ProjFacAddAnchors' work is finished: Name: {0}", theCurrentStory.Name));

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
                StoryStageLogic.ProjectStages.eProjFacCheckKeyTerms);

            return true;
        }

        public static bool ProjFacCheckKeyTerms(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'ProjFacCheckKeyTerms' work is finished: Name: {0}", theCurrentStory.Name));

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
                StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions);

            return true;
        }
        
        public static bool ProjFacAddStoryQuestions(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theCurrentStory.CraftingInfo.IsBiblicalStory);
            Console.WriteLine(String.Format("Checking if stage 'ProjFacAddStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

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

            if (theStoryProjectData.TeamMembers.IsThereASeparateEnglishBackTranslator)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);

            return true;
        }

        public static bool BackTranslatorTypeInternationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            System.Diagnostics.Debug.Assert(theStoryProjectData.ProjSettings.InternationalBT.HasData);
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                string strSentenceFinalPunct = theStoryProjectData.ProjSettings.InternationalBT.FullStop;
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
                    theSE.SetStatusBar(String.Format("Error: Verse {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}", nVerseNumber, theStoryProjectData.ProjSettings.NationalBT.LangName));
                    return false;
                }

                nVerseNumber++;
            }

            // if there's only an English BT, then we need to know who (which UNS) did the BT.
            if (!theStoryProjectData.ProjSettings.NationalBT.HasData)
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
            
            return true;
        }

        protected static void QueryForUnsBackTranslator(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
        {
            while (String.IsNullOrEmpty(theCurrentStory.CraftingInfo.BackTranslatorMemberID))
            {
                if (theCurrentStory.CraftingInfo.IsBiblicalStory)
                {
                    MessageBox.Show("In the following window, click on the browse button to select the 'UNS Back-translator' and add or choose the UNS that did this back-translation.", Properties.Resources.IDS_Caption);
                    StoryFrontMatterForm dlg = new StoryFrontMatterForm(theSE, theStoryProjectData, theCurrentStory);
                    dlg.Text = "Choose the UNS that did the back-translation";
                    dlg.ShowDialog();
                }
                else
                {
                    MemberPicker dlg = new MemberPicker(theStoryProjectData, TeamMemberData.UserTypes.eUNS);
                    dlg.Text = "Choose the UNS that did this back-translation";
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;

                    theCurrentStory.CraftingInfo.BackTranslatorMemberID = dlg.SelectedMember.MemberGuid;
                }
            }
        }

        public static bool ConsultantCheckNonBiblicalStory(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckNonBiblicalStory' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckStoryInfo(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryInfo' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnchors(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchors' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it over to the coach, let's make sure that if the Project Facilitator had initiated
            //  a conversation, that the consultant answered it.
            int nVerseNumber = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.ConsultantNotes)
                    if ((aConNote.Count > 0) && (aConNote[0].Direction == ConsultNoteDataConverter.CommunicationDirections.eProjFacToConsultant))
                        if ((aConNote.Count == 1) || !aConNote[1].HasData)
                        {
                            ShowErrorFocus(theSE, aConNote[1].TextBox,
                                String.Format("Error: in verse {0}, the ProjFac asked a question, which you didn't respond to. Did you forget it?", nVerseNumber));
                            return false;
                        }
                nVerseNumber++;
            }
            return true;
        }

        public static bool CoachReviewRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
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

        public static bool ConsultantReviseRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it back to the Project Facilitator, let's make sure that if the coach had made 
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

        public static bool ProjFacReviseBasedOnRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReviseBasedOnRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // let's make sure that if the CIT had made a comment, that the ProjFac answered it.
            int nVerseNumber = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                foreach (ConsultNoteDataConverter aConNote in aVerseData.ConsultantNotes)
                {
                    int nIndex = aConNote.Count;
                    CommInstance theLastCI = aConNote[nIndex];
                    System.Diagnostics.Debug.Assert(theLastCI.Direction == ConsultNoteDataConverter.CommunicationDirections.eProjFacToConsultant);
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

        public static bool ProjFacOnlineReview1WithConsultant(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacOnlineReview1WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacReadyForTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReadyForTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacEnterAnswersToStoryQuestionsOfTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterAnswersToStoryQuestionsOfTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacEnterRetellingOfTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterRetellingOfTest1' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnchorsRound2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchorsRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckAnswersToTestingQuestionsRound2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnswersToTestingQuestionsRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantCheckRetellingRound2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckRetellingRound2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CoachReviewRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantReviseRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacReviseBasedOnRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReviseBasedOnRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacOnlineReview2WithConsultant(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacOnlineReview2WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacReadyForTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReadyForTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacEnterAnswersToStoryQuestionsOfTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterAnswersToStoryQuestionsOfTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ProjFacEnterRetellingOfTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterRetellingOfTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool ConsultantReviewTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviewTest2' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool CoachReviewTest2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewTest2Notes' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool TeamComplete(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamComplete' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }
    }
}
