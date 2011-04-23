#define NotRelaxTqCountRequirement

using System;
using System.Linq;
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
        /// <param name="theStoryProjectData">Needed for things like punctuation full stops for the various languages, etc.</param>
        /// <param name="theCurrentStory">The data of the story that we're to check</param>
        /// <param name="eProposedNextState">The state we're going to (so we can do specialized checking that everything is ok)</param>
        /// <returns></returns>
        public delegate bool CheckForValidEndOfState(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState);

        public static bool ProjFacTypeVernacular(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeVernacular' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then gets rid of the Vernacular (so just allow it)
            if (!theStoryProjectData.ProjSettings.Vernacular.HasData)
                return true;

            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                // if there are no verses, then just quit (before we get into an infinite loop)
                if (theCurrentStory.Verses.Count == 0)
                {
                    ShowError(theSE, "Error: No lines in the story!");
                    return false;
                }

                int nVerseNumber = 1;   // this wants to be 1, because it's dealing with the
                                        // VerseBT pane, which starts at 1.
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    if (aVerseData.IsVisible)
                    {
                        string strSentenceFinalPunct = theStoryProjectData.ProjSettings.Vernacular.FullStop;
                        List<string> lstSentences;
                        if (GetListOfSentences(aVerseData.StoryLine.Vernacular, strSentenceFinalPunct, out lstSentences))
                        {
                            if (lstSentences.Count > 1)
                            {
                                DialogResult res = MessageBox.Show(
                                    String.Format(Properties.Resources.IDS_QuerySplitMultipleLines,
                                                  theStoryProjectData.ProjSettings.Vernacular.LangName,
                                            nVerseNumber), OseResources.Properties.Resources.IDS_Caption,
                                    MessageBoxButtons.YesNoCancel);
                                if (res == DialogResult.Cancel)
                                    return false;

                                if (res == DialogResult.Yes)
                                {
                                int nNewVerses = lstSentences.Count;
                                while (nNewVerses-- > 1)
                                {
                                    string strSentence = lstSentences[nNewVerses];
                                    theCurrentStory.Verses.InsertVerse(nVerseNumber, strSentence, null, null, null);
                                }

                                aVerseData.StoryLine.Vernacular.SetValue(lstSentences[nNewVerses]);
                                bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }
                        }
                        }
                        else
                        {
                            if (!aVerseData.HasData)
                            {
                                theCurrentStory.Verses.Remove(aVerseData);
                                bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }
                        }
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
                MessageBox.Show(String.Format("In the following window, type in the purpose of the story (why you have it in your panorama) and list the resources you used to craft the story", Environment.NewLine), OseResources.Properties.Resources.IDS_Caption);
                theSE.QueryStoryPurpose();
            }

#if CheckProposedNextState
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
                    eProposedNextState = (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                        ? StoryStageLogic.ProjectStages.eProjFacAddAnchors : StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT;
                }
                else
                {
                    eProposedNextState = theStoryProjectData.TeamMembers.HasOutsideEnglishBTer
                        ? StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT : StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT;
                }
            }
#endif
            if (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddAnchors)
                theSE._bAutoHide = false;

            return true;
        }

        public static bool ProjFacTypeNationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeNationalBT' work is finished: Name: {0}", theCurrentStory.Name));
            
            // this can happen if the person has a story in this state, but then gets rid of the NationalBT (so just allow it)
            if (!theStoryProjectData.ProjSettings.NationalBT.HasData)
                return true;

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert(eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeVernacular);
                return true;
            }

            // see if the user would like us to break the sentences up
            bool bRepeatAfterMe = false;
            do
            {
                // if there are no verses, then just quit (before we get into an infinite loop)
                if (theCurrentStory.Verses.Count == 0)
                {
                    ShowError(theSE, "Error: No lines in the story!");
                    return false;
                }

                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    if (aVerseData.IsVisible)
                    {
                        string strSentenceFinalPunct = theStoryProjectData.ProjSettings.NationalBT.FullStop;
                        List<string> lstSentences;
                        if (!GetListOfSentences(aVerseData.StoryLine.NationalBt, strSentenceFinalPunct, out lstSentences))
                        {
                            // if there's nothing in this verse, then just get rid of it.
                            if (!aVerseData.HasData)
                            {
                                theCurrentStory.Verses.Remove(aVerseData);
                                bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }

                            if (aVerseData.StoryLine.Vernacular.HasData)
                            {
                                ShowErrorFocus(theSE, aVerseData.StoryLine.NationalBt.TextBox,
                                               String.Format(
                                                   "Error: Line {0} is missing a back-translation. Did you forget it?",
                                                   nVerseNumber));
                                return false;
                            }
                        }

                        else if (lstSentences.Count > 1)
                        {
                            MessageBox.Show(String.Format(Properties.Resources.IDS_UseStoryCollapse,
                                                          nVerseNumber,
                                                          theStoryProjectData.ProjSettings.NationalBT.LangName),
                                            OseResources.Properties.Resources.IDS_Caption);
                            return false;
                        }
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);

            // we need to know who (which UNS) did the BT (but only if we don't have a free tr,
            //  which otherwise is assumed to be the one that the UNS did). Might as well only
            //  ask if we're done with BT'ing, so also not if there's an International BT.
            if (!theStoryProjectData.ProjSettings.FreeTranslation.HasData
                && !theStoryProjectData.ProjSettings.InternationalBT.HasData)
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

#if CheckProposedNextState
            if (theCurrentStory.CraftingInfo.IsBiblicalStory)
            {
                if (theStoryProjectData.ProjSettings.InternationalBT.HasData
                    && !theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
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
                if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                    eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
                else if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                    eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
                else
                    System.Diagnostics.Debug.Assert(eProposedNextState == 
                        StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT);
            }
#endif

            if (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddAnchors)
                theSE._bAutoHide = false;

            return true;
        }

        public static bool ProjFacTypeInternationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then gets rid of the EnglishBT (so just allow it)
            if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                return true;
            
            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeVernacular)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT));
                return true;
            }

#if CheckForOneSentencePerLine
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                // if there are no verses, then just quit (before we get into an infinite loop)
                if (theCurrentStory.Verses.Count == 0)
                {
                    ShowError(theSE, "Error: No lines in the story!");
                    return false;
                }

                // now go thru the English BT parts and make sure that there's only one sentence/verse.
                // make sure that each verse has only one sentence
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    if (aVerseData.IsVisible)
                    {
                        string strSentenceFinalPunct = theStoryProjectData.ProjSettings.InternationalBT.FullStop;
                        List<string> lstSentences;
                        if ((!GetListOfSentences(aVerseData.StoryLine.InternationalBt, strSentenceFinalPunct, out lstSentences))
                            || (lstSentences.Count == 0))
                        {
                            // if there's nothing in this verse, then just get rid of it.
                            if (!aVerseData.HasData)
                            {
                                theCurrentStory.Verses.Remove(aVerseData);
                                bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }


                            // if there's data in either the story box or the natl bt box...
                            if (aVerseData.StoryLine.Vernacular.HasData || aVerseData.StoryLine.NationalBt.HasData)
                            {
                                // then there ought to be some in the English BT box as well.
                                // light it up and let the user know they need to do something!
                                ShowErrorFocus(theSE, aVerseData.StoryLine.InternationalBt.TextBox,
                                               String.Format(
                                                   "Error: Line {0} doesn't have any English back-translation in it. Did you forget it?",
                                                   nVerseNumber));
                                return false;
                            }
                            return false;
                        }

                        if (lstSentences.Count > 1)
                        {
                            MessageBox.Show(String.Format(Properties.Resources.IDS_UseStoryCollapse,
                                                          nVerseNumber,
                                                          theStoryProjectData.ProjSettings.InternationalBT.LangName),
                                            OseResources.Properties.Resources.IDS_Caption);
                            return false;
                        }
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);
#endif

            // if there's not a free translation and we haven't already figured it out, then
            //  query for the UNS who did the BT
            if (!theStoryProjectData.ProjSettings.FreeTranslation.HasData
                && (theCurrentStory.CraftingInfo.BackTranslator == null))
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

#if CheckProposedNextState
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eProjFacAddAnchors);
#endif

            if (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddAnchors)
                theSE._bAutoHide = false;

            return true;
        }

        public static bool ProjFacTypeFreeTranslation(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacTypeFreeTranslation' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then gets rid of the EnglishBT (so just allow it)
            if (!theStoryProjectData.ProjSettings.FreeTranslation.HasData)
                return true;

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeVernacular)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT));
                return true;
            }

#if CheckForOneSentencePerLine
            // make sure that each verse has only one sentence
            bool bRepeatAfterMe = false;
            do
            {
                // if there are no verses, then just quit (before we get into an infinite loop)
                if (theCurrentStory.Verses.Count == 0)
                {
                    ShowError(theSE, "Error: No lines in the story!");
                    return false;
                }

                // now go thru the Free Translation parts and make sure that there's only one sentence/verse.
                // make sure that each verse has only one sentence
                int nVerseNumber = 1;
                foreach (VerseData aVerseData in theCurrentStory.Verses)
                {
                    if (aVerseData.IsVisible)
                    {
                        string strSentenceFinalPunct = theStoryProjectData.ProjSettings.FreeTranslation.FullStop;
                        List<string> lstSentences;
                        if ((!GetListOfSentences(aVerseData.StoryLine.FreeTranslation, strSentenceFinalPunct, out lstSentences))
                            || (lstSentences.Count == 0))
                        {
                            // if there's nothing in this verse, then just get rid of it.
                            if (!aVerseData.HasData)
                            {
                                theCurrentStory.Verses.Remove(aVerseData);
                                bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }


                            // if there's data in either the story box or the natl bt box...
                            if (aVerseData.StoryLine.Vernacular.HasData 
                                || aVerseData.StoryLine.NationalBt.HasData 
                                || aVerseData.StoryLine.InternationalBt.HasData)
                            {
                                // then there ought to be some in the Free Translation box as well.
                                // light it up and let the user know they need to do something!
                                ShowErrorFocus(theSE, aVerseData.StoryLine.FreeTranslation.TextBox,
                                               String.Format(
                                                   "Error: Line {0} doesn't have any Free translation in it. Did you forget it?",
                                                   nVerseNumber));
                                return false;
                            }
                            return false;
                        }

                        if (lstSentences.Count > 1)
                        {
                            MessageBox.Show(String.Format(Properties.Resources.IDS_UseStoryCollapse,
                                                          nVerseNumber,
                                                          theStoryProjectData.ProjSettings.FreeTranslation.LangName),
                                            OseResources.Properties.Resources.IDS_Caption);
                            return false;
                        }
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);
#endif

            // if we haven't already figured it out, then query for the UNS who did the BT
            if (theCurrentStory.CraftingInfo.BackTranslator == null)
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

            if (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddAnchors)
                theSE._bAutoHide = false;

            return true;
        }

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
                if (((nIndex + 1) < strParagraph.Length) && (strParagraph.IndexOfAny(VersesData.achQuotes, nIndex + 1, 1) != -1))
                    nIndex++;

                string strLine = strParagraph.Substring(nStartIndex, nIndex - nStartIndex + 1).Trim();
                strLine = strLine.Replace("\r\n", " ");
                while (strLine.IndexOf("  ", StringComparison.Ordinal) != -1)
                    strLine = strLine.Replace("  ", " ");
                
                if ((strLine.Length == 1) && (strLine.IndexOfAny(achSplitOn) == 0))
                {
                    // this means it was just a 2nd punctuation mark
                    if (lstSentences.Count > 0)
                        lstSentences[lstSentences.Count - 1] += strLine;
                }

                else if (!String.IsNullOrEmpty(strLine))
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

        public static void ShowErrorFocus(StoryEditor theSE, CtrlTextBox tb, string strStatusMessage)
        {
            if (tb != null)
            {
                tb.Focus();
                tb.SelectAll();
            }
            ShowError(theSE, strStatusMessage);
        }

        public static void ShowErrorFocus(StoryEditor theSE, 
            HtmlConNoteControl paneConNote, int nVerseNumber, string strStatusMessage)
        {
            paneConNote.ScrollToVerse(nVerseNumber);
            ShowError(theSE, strStatusMessage);
        }

        public static void ShowError(StoryEditor theSE, string strStatusMessage)
        {
            theSE.SetStatusBar(strStatusMessage);
            Console.Beep();
            MessageBox.Show(strStatusMessage, OseResources.Properties.Resources.IDS_Caption);
        }

        public static bool ProjFacAddAnchors(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacAddAnchors' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                return true;
            
            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeVernacular)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT) 
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation));
                return true;
            }

            // for each verse, make sure that there is at least one anchor.
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    if (aVerseData.Anchors.Count == 0)
                    {
                        ShowError(theSE,
                                  String.Format("Error: Line {0} doesn't have an anchor. Did you forget it?",
                                                nVerseNumber));
                        theSE.FocusOnVerse(nVerseNumber, true, true);
                        return false;
                    }

                    /*
                    if (aVerseData.Anchors.IsKeyTermChecked)
                        bHasAnyKeyTermBeenChecked = true;
                    */
                }
                nVerseNumber++;
            }

            /* Not doing 'key terms' anymore. using L&C Notes instead
            if (!bHasAnyKeyTermBeenChecked)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_CheckOnKeyTerms,
                                               OseResources.Properties.Resources.IDS_Caption, 
                                               MessageBoxButtons.RetryCancel);
                if (res == DialogResult.Cancel)
                    return false;
            }
            */
#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions);
#endif

            return true;
        }

        public static bool ProjFacAddStoryQuestions(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacAddStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                return true;

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeVernacular)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddAnchors));
                return true;
            }

            // there should be at least half as many questions as there are verses.
            if (!CheckForCountOfTestingQuestions(theCurrentStory, theSE))
                return false;

            // before going to the CIT, let's make sure that if the CIT had made 
            //  a comment, that the PF answered it. (this only occurs if the CIT
            //  had earlier checked the story and gone backwards)
            if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                return false;

            // for the Payne approach, they want the first two UNS checks *before* the
            //  consultant sees it, so check if we should be skipping all the way to the 
            //  "after round 1 consultant checking do the UNS test" state
            // only do this if the PF was using the Next State button (not if they selected the
            //  state manually)
            if (!theSE._bByNextStateButton)
                return true;

            DialogResult res = MessageBox.Show(Properties.Resources.IDS_CheckForSkipToUnsCheck,
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return false;

            if (res == DialogResult.Yes)
            {
                // add the retelling lines to the verses for test n
                if (!QueryPrepareForRetellingBoxes(theSE))
                    return false;

                eProposedNextState = StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1;
            }

#if CheckProposedNextState
            else if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck1;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                                                StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif

            return true;
        }

        public static bool CheckForCountOfTestingQuestions(StoryData theCurrentStory, StoryEditor theSE)
        {
            int nNumOfVerses = 0, nNumOfTQs = 0;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
                if (aVerseData.IsVisible)
                {
                    nNumOfVerses++;
                    nNumOfTQs += aVerseData.TestQuestions.Count;
                }

            if (nNumOfTQs < ((nNumOfVerses + 1) / 2))
            {
                int nNumLacking = ((nNumOfVerses + 1) / 2) - nNumOfTQs;
#if !NotRelaxTqCountRequirement
                DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_WarnAboutNotEnoughTqs,
                                                                 nNumLacking),
                                                   OseResources.Properties.Resources.IDS_Caption,
                                                   MessageBoxButtons.YesNoCancel);
                
                if (res != DialogResult.Yes)
#else
                ShowError(theSE,
                          String.Format(Properties.Resources.IDS_NotEnoughTqs, nNumLacking));
#endif
                    return false;
            }
            return true;
        }

        public static bool ProjFacRevisesBeforeUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacRevisesBeforeUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                return true;

            // there should be at least half as many questions as there are verses.
            if (!CheckForCountOfTestingQuestions(theCurrentStory, theSE))
                return false;

            // before going to the CIT, let's make sure that if the CIT had made 
            //  a comment, that the PF answered it. (this only occurs if the CIT
            //  had earlier checked the story and gone backwards)
            if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck1;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                                                StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif

            return true;
        }

        public static bool BackTranslatorTypeInternationalBT(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTypeInternationalBT' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then gets rid of the EnglishBT (so just allow it)
            if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                return true;
            
            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));
                return true;
            }

            // if there are no verses, then just quit (before we get into an infinite loop)
            if (theCurrentStory.Verses.Count == 0)
            {
                ShowError(theSE, "Error: No lines in the story!");
                return false;
            }

            // now go thru the English BT parts and make sure that there's only one sentence/verse.
            // make sure that each verse has only one sentence
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    string strSentenceFinalPunct = theStoryProjectData.ProjSettings.InternationalBT.FullStop;
                    List<string> lstSentences;
                    if ((!GetListOfSentences(aVerseData.StoryLine.InternationalBt, strSentenceFinalPunct, out lstSentences))
                        || (lstSentences.Count == 0))
                    {
                        // light it up and let the user know they need to do something!
                        ShowErrorFocus(theSE, aVerseData.StoryLine.InternationalBt.TextBox,
                                       String.Format(
                                           "Error: Line {0} doesn't have any English back-translation in it. Did you forget it?",
                                           nVerseNumber));
                        return false;
                    }

                    if (lstSentences.Count > 1)
                    {
                        // light it up and let the user know they need to do something!
                        ShowErrorFocus(theSE, aVerseData.StoryLine.InternationalBt.TextBox,
                                       String.Format(
                                           "Error: Line {0} has multiple sentences in English, but only 1 in {1}. Adjust the English to match the {1}",
                                           nVerseNumber, theStoryProjectData.ProjSettings.NationalBT.LangName));
                        return false;
                    }
                }

                nVerseNumber++;
            }

            // if we haven't already figured it out, then query for the UNS who did the BT
            if (theCurrentStory.CraftingInfo.BackTranslator == null)
                QueryForUnsBackTranslator(theSE, theStoryProjectData, theCurrentStory);

#if CheckProposedNextState
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                eProposedNextState = StoryStageLogic.ProjectStages.eConsultantCheckNonBiblicalStory;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck1;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif
            
            return true;
        }

        public static bool BackTranslatorTranslateConNotesBeforeUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTranslateConNotesBeforeUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));
                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif

            return true;
        }

        public static void QueryForUnsBackTranslator(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
        {
            while (!MemberIdInfo.Configured(theCurrentStory.CraftingInfo.BackTranslator))
            {
                var dlg = new MemberPicker(theStoryProjectData, TeamMemberData.UserTypes.UNS)
                              {
                                  Text = "Choose the UNS that did this back-translation"
                              };
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                MemberIdInfo.SetCreateIfEmpty(ref theCurrentStory.CraftingInfo.BackTranslator,
                                              dlg.SelectedMember.MemberGuid, true);
            }
        }

        public static bool ConsultantCheckNonBiblicalStory(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckNonBiblicalStory' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool FirstPassMentorCheck1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'FirstPassMentorCheck1' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));
                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif
            
            return true;
        }

        public static bool ConsultantCheckStoryInfo(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryInfo' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck1)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));

                // if it's going to the PF, then...
                if (eProposedNextState != StoryStageLogic.ProjectStages.eFirstPassMentorCheck1)
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eConsultantCheckAnchors);
#endif

            return true;
        }

        public static bool ConsultantCheckAnchors(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckAnchors' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck1)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest))
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eConsultantCheckStoryQuestions);
#endif

            return true;
        }

        public static bool ConsultantCheckStoryQuestions(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheckStoryQuestions' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckAnchors)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck1)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest))
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            if (eProposedNextState == StoryStageLogic.ProjectStages.eCoachReviewRound1Notes)
            {
                // before going to the Coach, let's make sure that if the coach had made 
                //  a comment, that the CIT answered it. (this only occurs if the coach
                //  had earlier checked the story and gone backwards)
                if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                    return false;
            }
            else if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes))
            {
                if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                    return false;
            }

#if CheckProposedNextState
            // if we have an independent consultant, then the next state is back to the PF
            if (theStoryProjectData.TeamMembers.HasIndependentConsultant 
             && theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eIndependentConsultant)
            {
                if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                    eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes;
                else
                    eProposedNextState = StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes;
            }
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eCoachReviewRound1Notes);
#endif

            return true;
        }

        public static bool ConsultantCauseRevisionBeforeUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCauseRevisionBeforeUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesBeforeUnsTest))
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // before going to the Coach, let's make sure that if the coach had made 
            //  a comment, that the CIT answered it. (this only occurs if the coach
            //  had earlier checked the story and gone backwards)
            if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eCoachReviewRound1Notes);
#endif

            return true;
        }

        public static bool CheckThatCoachAnsweredCITsQuestions(StoryEditor theSE, 
            StoryData theCurrentStory)
        {
            int nVerseNumber = 0;   // this wants to be 0, because ConNotes have a 0th verse
            if (!CheckThatCoachAnsweredInVerse(theCurrentStory, theSE, 
                theCurrentStory.Verses.FirstVerse, ref nVerseNumber))
                return false;

            nVerseNumber++;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (!CheckThatCoachAnsweredInVerse(theCurrentStory, theSE, 
                    aVerseData, ref nVerseNumber))
                    return false;
                nVerseNumber++;
            }
            return true;
        }

        private static bool CheckThatConsultantAnsweredInVerse(StoryData theCurrentStory,
            StoryEditor theSE, VerseData theVerseData, ref int nVerseNumber)
        {
            return CheckThatMentorAnsweredInVerse(theVerseData, theSE,
                theSE.htmlConsultantNotesControl, theCurrentStory, theVerseData.ConsultantNotes,
                VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields, ref nVerseNumber);
        }

        private static bool CheckThatCoachAnsweredInVerse(StoryData theCurrentStory,
            StoryEditor theSE, VerseData theVerseData, ref int nVerseNumber)
        {
            return CheckThatMentorAnsweredInVerse(theVerseData, theSE,
                theSE.htmlCoachNotesControl, theCurrentStory, theVerseData.CoachNotes, 
                VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields, ref nVerseNumber);
        }

        private static bool CheckThatMentorAnsweredInVerse(VerseData theVerseData,
            StoryEditor theSE, HtmlConNoteControl htmlControl, StoryData theCurrentStory,
            ConsultNotesDataConverter dataConNotes, 
            VerseData.ViewSettings.ItemToInsureOn itemToInsureOn, ref int nVerseNumber)
        {
            if (theVerseData.IsVisible)
                return CheckThatMentorAnsweredMenteesQuestions(theSE,
                                                               htmlControl,
                                                               dataConNotes,
                                                               theCurrentStory,
                                                               itemToInsureOn,
                                                               ref nVerseNumber);
            return true;
        }

        public static bool CheckThatCITAnsweredPFsQuestions(StoryEditor theSE, StoryData theCurrentStory)
        {
            int nVerseNumber = 0;
            if (!CheckThatConsultantAnsweredInVerse(theCurrentStory, theSE, 
                theCurrentStory.Verses.FirstVerse, ref nVerseNumber))
                return false;

            nVerseNumber++;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (!CheckThatConsultantAnsweredInVerse(theCurrentStory, theSE,
                    aVerseData, ref nVerseNumber))
                    return false;

                nVerseNumber++;
            }

            return true;
        }

        public static bool CheckThatCITRespondedToCoachQuestions(StoryEditor theSE, StoryData theCurrentStory)
        {
            int nVerseNumber = 0;
            if (theCurrentStory.Verses.FirstVerse.IsVisible)
                if (!CheckThatMenteeAnsweredMentorsQuestions(theSE,
                        theSE.htmlCoachNotesControl, theCurrentStory.Verses.FirstVerse.CoachNotes, 
                        theCurrentStory, VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields, 
                        ref nVerseNumber))
                    return false;

            nVerseNumber++;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.IsVisible)
                    if (!CheckThatMenteeAnsweredMentorsQuestions(theSE, theSE.htmlCoachNotesControl,
                        aVerseData.CoachNotes, theCurrentStory, 
                        VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields, ref nVerseNumber))
                        return false;
                nVerseNumber++;
            }

            return true;
        }

        public static bool CheckThatPFRespondedToCITQuestions(StoryEditor theSE, StoryData theCurrentStory)
        {
            int nVerseNumber = 0;
            if (theCurrentStory.Verses.FirstVerse.IsVisible)
                if (!CheckThatMenteeAnsweredMentorsQuestions(theSE, theSE.htmlConsultantNotesControl,
                    theCurrentStory.Verses.FirstVerse.ConsultantNotes, theCurrentStory,
                    VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields, ref nVerseNumber))
                    return false;

            nVerseNumber++;
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.IsVisible)
                    if (!CheckThatMenteeAnsweredMentorsQuestions(theSE, theSE.htmlConsultantNotesControl,
                        aVerseData.ConsultantNotes, theCurrentStory,
                    VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields, ref nVerseNumber))
                        return false;

                nVerseNumber++;
            }
            return true;
        }

        static bool CheckThatMentorAnsweredMenteesQuestions(StoryEditor theSE,
            HtmlConNoteControl paneConNote, IEnumerable<ConsultNoteDataConverter> aCNsDC, 
            StoryData theStory, VerseData.ViewSettings.ItemToInsureOn ePane,
            ref int nVerseNumber)
        {
            foreach (var aConNote in aCNsDC)
            {
                var theLastCi = aConNote.FinalComment;
                if (!aConNote.Visible ||
                    aConNote.IsFinished ||
                    aConNote.IsFromMentee(theLastCi) ||
                    theLastCi.HasData)
                    continue;

                var viewSettings = new VerseData.ViewSettings(ePane);
                theSE.NavigateTo(theStory.Name,
                                 viewSettings,
                                 false,
                                 null);

                TeamMemberData theCommentor = theLastCi.Commentor(theSE.StoryProject.TeamMembers);
                string strErrorMsg = String.Format(
                    Properties.Resources.IDS_DidntAnswerQuestion,
                    nVerseNumber,
                    paneConNote.PaneLabel(),
                    aConNote.MentorLabel(theCommentor, theStory.CraftingInfo.ProjectFacilitator.MemberId));
                ShowErrorFocus(theSE, paneConNote, nVerseNumber, strErrorMsg);
                return false;
            }

            return true;
        }

        static bool CheckThatMenteeAnsweredMentorsQuestions(StoryEditor theSE, 
            HtmlConNoteControl paneConNote, IEnumerable<ConsultNoteDataConverter> aCNsDC, 
            StoryData theStory, VerseData.ViewSettings.ItemToInsureOn ePane,
            ref int nVerseNumber)
        {
            foreach (var aConNote in aCNsDC)
            {
                var theLastCi = aConNote.FinalComment;
                if (!aConNote.Visible || 
                    aConNote.IsFinished ||
                    aConNote.IsFromMentor(theLastCi) ||
                    theLastCi.HasData) 
                    continue;

                var viewSettings = new VerseData.ViewSettings(ePane);
                theSE.NavigateTo(theStory.Name,
                                 viewSettings,
                                 false,
                                 null);

                TeamMemberData theCommentor = theLastCi.Commentor(theSE.StoryProject.TeamMembers);
                string strErrorMsg = String.Format(
                    Properties.Resources.IDS_DidntAnswerQuestion,
                    nVerseNumber,
                    paneConNote.PaneLabel(),
                    aConNote.MenteeLabel(theCommentor, theStory.CraftingInfo.ProjectFacilitator.MemberId));

                ShowErrorFocus(theSE, paneConNote, nVerseNumber, strErrorMsg);
                return false;
            }

            return true;
        }

        public static bool CoachReviewRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                // if it's going backwards...
                System.Diagnostics.Debug.Assert(eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCauseRevisionBeforeUnsTest);
                if (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCauseRevisionBeforeUnsTest)
                {
                    // make sure that if the CIT asked a question that the Coach responded to it.
                    if (!CheckThatCoachAnsweredCITsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // going forward... same thing
            // before handing it back to the consultant, let's make sure that if the consultant had initiated
            //  a conversation, that the coach answered it.
            if (!CheckThatCoachAnsweredCITsQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes);
#endif

            return true;
        }

        public static bool ConsultantReviseRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert(eProposedNextState == StoryStageLogic.ProjectStages.eCoachReviewRound1Notes);
                if (eProposedNextState == StoryStageLogic.ProjectStages.eCoachReviewRound1Notes)
                {
                    // make sure that if the CIT answered the Coach's comments.
                    if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // and if the ProjectFac asked a question that the CIT responded to it.
            if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes);
#endif

            return true;
        }

        public static bool BackTranslatorTranslateConNotes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTranslateConNotes' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo));

                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes);
#endif

            return true;
        }

        public static bool ProjFacReviseBasedOnRound1Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReviseBasedOnRound1Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo))
                {
                    // make sure that if the PF answered the CIT 's comments.
                    if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest1)
                || (eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck2)
                || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheck2))
            {
                // make sure that if the PF answered the CIT 's comments.
                if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                    return false;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant);
#endif

            return true;
        }

        public static bool ProjFacOnlineReview1WithConsultant(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacOnlineReview1WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo))
                {
                    // make sure that if the PF answered the CIT 's comments.
                    if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // let's make sure that if the CIT had made a comment, that the ProjFac answered it.
            if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacReadyForTest1);
#endif

            return true;
        }

        public static bool ProjFacReadyForTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReadyForTest1' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions));

                return true;
            }

            // add the retelling lines to the verses for test n
            if (!QueryPrepareForRetellingBoxes(theSE)) 
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState == 
                StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1);
#endif

            return true;
        }

        private static bool QueryPrepareForRetellingBoxes(StoryEditor theSE)
        {
            DialogResult res = MessageBox.Show(Properties.Resources.IDS_AddRetellingTestQuery,
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return false;

            if (res == DialogResult.Yes)
                theSE.AddRetellingTest();

            return true;
        }

        public static bool CheckAnswersAnswered(StoryEditor theSE, StoryData theCurrentStory)
        {
            int nVerseNumber = 1;   // this wants to be 1, because it deals with the VerseBT pane
            foreach (VerseData aVerseData in theCurrentStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    foreach (TestQuestionData aTQ in aVerseData.TestQuestions)
                        foreach (LineData aLineData in aTQ.Answers)
                            if (!aLineData.HasData)
                            {
                                ShowErrorFocus(theSE, aLineData.ExistingTextBox,
                                               String.Format(
                                                   "Error: Line {0} is missing an answer to a testing question. Did you forget it?",
                                                   nVerseNumber));
                                return false;
                            }
                }
                nVerseNumber++;
            }
            return true;
        }

        public static bool ProjFacEnterRetellingOfTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterRetellingOfTest1' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReadyForTest1)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes));

                return true;
            }

            // add the answer lines to the verses for test n
            DialogResult res = MessageBox.Show(Properties.Resources.IDS_AddInferenceTestQuery,
                                               OseResources.Properties.Resources.IDS_Caption,
                                               MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return false;

            if (res == DialogResult.Yes)
                theSE.AddInferenceTest();

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState == 
                StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1);
#endif

            return true;
        }

        public static bool ProjFacEnterAnswersToStoryQuestionsOfTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterAnswersToStoryQuestionsOfTest1' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReadyForTest1)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacOnlineReview1WithConsultant)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes));

                // make sure they have some answer written into each question (if going on to the next test)
                if (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1)
                {
                    if (!CheckAnswersAnswered(theSE, theCurrentStory))
                        return false;
                    theSE.AddRetellingTest();
                }

                return true;
            }

            // make sure they have some answer written into each question
            if (!CheckAnswersAnswered(theSE, theCurrentStory))
                return false;

            // if the user used the 'Select State' dialog, then we don't need to check whether
            //  they want to enter another test.
            if (!theSE._bByNextStateButton)
                return true;

            // if they've only done one test, then ask them to do another (but don't force them)
            DialogResult res =
                MessageBox.Show(
                    Properties.Resources.IDS_AddAnotherTestQuery,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

            if (res == DialogResult.Cancel)
                return false;

            if (res == DialogResult.Yes)
            {
                theSE.AddRetellingTest();

                // this will go:
                //  ProjFacEnterRetellingOfTest1 (to get answer from UNS_1), then
                //  ProjFacEnterAnswersToStoryQuestionsOfTest1 (to get retelling from UNS_1), then (while Testor.Count < 2) back to:
                //  ProjFacEnterRetellingOfTest1, etc.
                eProposedNextState = StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1;
            }
#if CheckProposedNextState
            else if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest1;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheck2);
#endif

            return true;
        }

        public static bool ProjFacRevisesAfterUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacRevisesAfterUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theCurrentStory.CraftingInfo.IsBiblicalStory)
                return true;

            // there should be at least half as many questions as there are verses.
            if (!CheckForCountOfTestingQuestions(theCurrentStory, theSE))
                return false;

            // before going to the CIT, let's make sure that if the CIT had made 
            //  a comment, that the PF answered it. (this only occurs if the CIT
            //  had earlier checked the story and gone backwards)
            if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck1;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                                                StoryStageLogic.ProjectStages.eConsultantCheckStoryInfo);
#endif

            return true;
        }

        public static bool BackTranslatorTypeInternationalBTTest1(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTypeInternationalBTTest1' work is finished: Name: {0}", theCurrentStory.Name));

            // this can happen if the person has a story in this state, but then gets rid of the EnglishBT (so just allow it)
            if (!theStoryProjectData.ProjSettings.InternationalBT.HasData)
                return true;
            
            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert(eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest);
                return true;
            }

            // if there are no verses, then just quit (before we get into an infinite loop)
            if (theCurrentStory.Verses.Count == 0)
            {
                ShowError(theSE, "Error: No lines in the story!");
                return false;
            }

#if CheckProposedNextState
            if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheck2);
#endif
            
            return true;
        }

        public static bool BackTranslatorTranslateConNotesAfterUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTranslateConNotesAfterUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards, we don't have anything to check
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest));
                return true;
            }

#if CheckProposedNextState
            if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorCheck2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantCheck2);
#endif

            return true;
        }

        public static bool FirstPassMentorCheck2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'FirstPassMentorCheck2' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest)
                    ||  (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes));

                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(theStoryProjectData.TeamMembers.IsThereAFirstPassMentor
                && (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCheck2));
#endif
            
            return true;
        }

        public static bool ConsultantCheck2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCheck2' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck2)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesAfterUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes));

                // if it's going to the PF, then...
                if (eProposedNextState != StoryStageLogic.ProjectStages.eFirstPassMentorCheck2)
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // and if the ProjectFac asked a question that the CIT responded to it (so the 
            //  coach can check the responses)
            if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            // if we have an independent consultant, then the next state is TeamComplete
            if (theStoryProjectData.TeamMembers.HasIndependentConsultant
             && theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eIndependentConsultant)
            {
                eProposedNextState = StoryStageLogic.ProjectStages.eTeamComplete;
            }
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eCoachReviewRound2Notes);
#endif

            return true;
        }

        public static bool ConsultantCauseRevisionAfterUnsTest(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantCauseRevisionAfterUnsTest' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck2)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest1)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest));

                if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest1)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest))
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

            // before going to the Coach, let's make sure that if the coach had made 
            //  a comment, that the CIT answered it. (this only occurs if the coach
            //  had earlier checked the story and gone backwards)
            if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eCoachReviewRound2Notes);
#endif

            return true;
        }


        public static bool CoachReviewRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviseRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantCauseRevisionAfterUnsTest));

                // if it's going backwards...
                // make sure that if the CIT asked a question that the Coach responded to it.
                if (!CheckThatCoachAnsweredCITsQuestions(theSE, theCurrentStory))
                    return false;

                return true;
            }

            // before finalizing it, make sure that if the consultant had initiated
            //  a conversation, that the coach answered it.
            if (!CheckThatCoachAnsweredCITsQuestions(theSE, theCurrentStory))
                return false;

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState == 
                StoryStageLogic.ProjectStages.eTeamComplete);
#endif

            return true;
        }

        public static bool ConsultantFinalCheck(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantFinalCheck' work is finished: Name: {0}", theCurrentStory.Name));

            // if going backwards...
            if (IsGoingBackwards(theCurrentStory, eProposedNextState))
            {
                System.Diagnostics.Debug.Assert((eProposedNextState == StoryStageLogic.ProjectStages.eFirstPassMentorCheck2)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eCoachReviewRound2Notes));

                // if it's going to the PF, then...
                if ((eProposedNextState == StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes)
                    || (eProposedNextState == StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound1Notes))
                {
                    // make sure that if the ProjectFac asked a question that the CIT responded to it.
                    if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                        return false;
                }

                return true;
            }

#if CheckProposedNextState
            System.Diagnostics.Debug.Assert(eProposedNextState == 
                StoryStageLogic.ProjectStages.eTeamComplete);
#endif

            // before finalizing it, make sure that if the consultant had initiated
            //  a conversation, that the coach answered it.
            if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                return false;

            return true;
        }

        /*
        public static bool ConsultantReviseRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviseRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it back to the Project Facilitator, let's make sure that if the coach had made 
            //  a comment, that the CIT answered it.
            if (!CheckThatCITRespondedToCoachQuestions(theSE, theCurrentStory))
                return false;

            // and if the ProjectFac asked a question that the CIT responded to it.
            if (!CheckThatCITAnsweredPFsQuestions(theSE, theCurrentStory))
                return false;

            if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotes2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound2Notes);

            return true;
        }

        public static bool BackTranslatorTranslateConNotes2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTranslateConNotes2' work is finished: Name: {0}", theCurrentStory.Name));

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacReviseBasedOnRound2Notes);

            return true;
        }

        public static bool ProjFacReviseBasedOnRound2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReviseBasedOnRound2Notes' work is finished: Name: {0}", theCurrentStory.Name));

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacOnlineReview2WithConsultant);

            return true;
        }

        public static bool ProjFacOnlineReview2WithConsultant(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacOnlineReview2WithConsultant' work is finished: Name: {0}", theCurrentStory.Name));
            
            // let's make sure that if the CIT had made a comment, that the ProjFac answered it.
            if (!CheckThatPFRespondedToCITQuestions(theSE, theCurrentStory))
                return false;

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacReadyForTest2);

            return true;
        }

        public static bool ProjFacReadyForTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacReadyForTest2' work is finished: Name: {0}", theCurrentStory.Name));

            // add the story question answer lines and retelling lines to the verses for test n
            DialogResult res = MessageBox.Show(OseResources.Properties.Resources.IDS_AddTestQuery, OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return false;

            if (res == DialogResult.Yes)
                theSE.AddTest();

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest2);

            return true;
        }

        public static bool ProjFacEnterRetellingOfTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterRetellingOfTest2' work is finished: Name: {0}", theCurrentStory.Name));

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest2);

            return true;
        }

        public static bool ProjFacEnterAnswersToStoryQuestionsOfTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ProjFacEnterAnswersToStoryQuestionsOfTest2' work is finished: Name: {0}", theCurrentStory.Name));

            // make sure they have some answer written into each question
            if (!CheckAnswersAnswered(theSE, theCurrentStory))
                return false;

            // see if they want to enter results for the next UNS test
            DialogResult res = MessageBox.Show("Click 'Yes' to create the boxes for entering the next UNS's answers to the testing questions", OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Yes)
                theSE.AddTest();
            else if (res == DialogResult.Cancel)
                return false;
            else if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                eProposedNextState = StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest2;
            else if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorReviewTest2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantReviewTest2);

            return true;
        }

        public static bool BackTranslatorTypeInternationalBTTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'BackTranslatorTypeInternationalBTTest2' work is finished: Name: {0}", theCurrentStory.Name));

            // if there are no verses, then just quit (before we get into an infinite loop)
            if (theCurrentStory.Verses.Count == 0)
            {
                ShowError(theSE, "Error: No lines in the story!");
                return false;
            }

            if (theStoryProjectData.TeamMembers.HasFirstPassMentor)
                eProposedNextState = StoryStageLogic.ProjectStages.eFirstPassMentorReviewTest2;
            else
                System.Diagnostics.Debug.Assert(eProposedNextState ==
                    StoryStageLogic.ProjectStages.eConsultantReviewTest2);

            return true;
        }

        public static bool FirstPassMentorReviewTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'FirstPassMentorReviewTest2' work is finished: Name: {0}", theCurrentStory.Name));

            System.Diagnostics.Debug.Assert(theStoryProjectData.TeamMembers.IsThereAFirstPassMentor
                && (eProposedNextState == StoryStageLogic.ProjectStages.eConsultantReviewTest2));

            return true;
        }

        public static bool ConsultantReviewTest2(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'ConsultantReviewTest2' work is finished: Name: {0}", theCurrentStory.Name));

            System.Diagnostics.Debug.Assert(eProposedNextState == StoryStageLogic.ProjectStages.eCoachReviewTest2Notes);

            // if we have an independent consultant, then we're done
            if (theStoryProjectData.TeamMembers.HasIndependentConsultant
             && theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eIndependentConsultant)
            {
                eProposedNextState = StoryStageLogic.ProjectStages.eTeamComplete;
            }

            return true;
        }

        public static bool CoachReviewTest2Notes(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'CoachReviewTest2Notes' work is finished: Name: {0}", theCurrentStory.Name));

            // before handing it back to the consultant, let's make sure that if the consultant had initiated
            //  a conversation, that the coach answered it.
            if (!CheckThatCoachAnsweredCITsQuestions(theSE, theCurrentStory))
                return false;

            System.Diagnostics.Debug.Assert(eProposedNextState ==
                StoryStageLogic.ProjectStages.eTeamComplete);

            return true;
        }
        */
        public static bool TeamComplete(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamComplete' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        public static bool TeamFinalApproval(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory, ref StoryStageLogic.ProjectStages eProposedNextState)
        {
            Console.WriteLine(String.Format("Checking if stage 'TeamFinalApproval' work is finished: Name: {0}", theCurrentStory.Name));
            return true;
        }

        /// <summary>
        /// Indicates whether the new proposed state is a backwards transition or not
        /// </summary>
        /// <param name="theCurrentStory"></param>
        /// <param name="eProposedNextState"></param>
        /// <returns></returns>
        protected static bool IsGoingBackwards(StoryData theCurrentStory, StoryStageLogic.ProjectStages eProposedNextState)
        {
            return ((int)eProposedNextState < (int)theCurrentStory.ProjStage.ProjectStage);
        }
    }
}
