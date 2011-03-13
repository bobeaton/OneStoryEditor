using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public abstract class MentoreeRequirementsCheck
    {
        protected readonly StoryData TheStory;
        protected readonly StoryEditor TheSe;

        protected MentoreeRequirementsCheck(StoryEditor theSe, StoryData theStory)
        {
            TheSe = theSe;
            TheStory = theStory;
        }

        public abstract bool CheckIfRequirementsAreMet();
    }

    public class ProjectFacilitatorRequirementsCheck : MentoreeRequirementsCheck
    {
        public readonly bool DoVernacularLangFields;
        public readonly bool DoNationalBtLangFields;
        public readonly bool DoInternationalBtFields;
        public readonly bool DoFreeTranslationFields;
        public readonly bool DoAnchors;
        public readonly bool DoRetelling;
        public readonly bool DoTestQuestions;
        public readonly bool DoAnswers;

        public ProjectFacilitatorRequirementsCheck(StoryEditor theSe,
            StoryProjectData theStoryProjectData, StoryData theStory)
            :base(theSe, theStory)
        {
            ProjectSettings projSettings = TheSe.StoryProject.ProjSettings;

            DoVernacularLangFields = projSettings.Vernacular.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                         TasksPf.TaskSettings.
                                                                                             VernacularLangFields);
            DoNationalBtLangFields = projSettings.NationalBT.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                         TasksPf.TaskSettings.
                                                                                             NationalBtLangFields);
            DoInternationalBtFields = projSettings.InternationalBT.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                               TasksPf.TaskSettings.
                                                                                                   InternationalBtFields);
            DoFreeTranslationFields = projSettings.FreeTranslation.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                               TasksPf.TaskSettings.
                                                                                                   FreeTranslationFields);
            DoAnchors = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                      TasksPf.TaskSettings.Anchors);
            DoRetelling = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                      TasksPf.TaskSettings.Retellings);
            DoTestQuestions = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                      TasksPf.TaskSettings.TestQuestions);
            DoAnswers = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                      TasksPf.TaskSettings.Answers);
        }

        public override bool CheckIfRequirementsAreMet()
        {
            bool bTriggerRefresh = false;
            StoryProjectData theStoryProjectData = TheSe.StoryProject;
            try
            {
                if (DoVernacularLangFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFieldType.Vernacular,
                        StoryEditor.TextFieldType.Vernacular, ref bTriggerRefresh))  // by definition is the highest
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoNationalBtLangFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFieldType.NationalBt,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoInternationalBtFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFieldType.InternationalBt,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoFreeTranslationFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFieldType.FreeTranslation,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoAnchors)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletionAnchors(TheSe, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoRetelling)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletionRetelling(TheSe, theStoryProjectData, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoTestQuestions)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletionTestQuestions(TheSe, theStoryProjectData, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoAnswers)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletionAnswers(TheSe, theStoryProjectData, TheStory))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                // finally, they have to have responded to all of the CITs comments
                if (!CheckEndOfStateTransition.CheckThatPFRespondedToCITQuestions(TheSe, TheStory))
                    throw StoryProjectData.BackOutWithNoUI;
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                return false;
            }
            finally
            {
                if (bTriggerRefresh)
                    TheSe.refreshToolStripMenuItem_Click(null, null);
            }

            return true;
        }

        private bool CheckForCompletionAnswers(StoryEditor theSe, StoryProjectData theStoryProjectData, StoryData theStory)
        {
            int nVerseNumber = 1;   // this wants to be 1, because it deals with the VerseBT pane
            foreach (VerseData aVerseData in theStory.Verses)
            {
                if (!aVerseData.IsVisible || !aVerseData.TestQuestions.HasData)
                    continue;

                foreach (TestQuestionData aTQ in aVerseData.TestQuestions)
                {
                    // if there are no answers, then we have a problem
                    if (aTQ.Answers.Count == 0)
                    {
                        MessageBox.Show(Properties.Resources.IDS_CantHaveNoAnswers,
                                        OseResources.Properties.Resources.IDS_Caption);
                        return false;
                    }

                    // then make sure that all fields configured are filled
                    int nLastIndex = aTQ.Answers.Count - 1;
                    ProjectSettings.LanguageInfo li;
                    LineData theAnswerData = aTQ.Answers[nLastIndex];
                    ProjectSettings projSettings = theStoryProjectData.ProjSettings;
                    if (!HasProperData(projSettings.ShowAnswersVernacular, (li = projSettings.Vernacular), theAnswerData.Vernacular)
                        || !HasProperData(projSettings.ShowAnswersNationalBT, (li = projSettings.NationalBT), theAnswerData.NationalBt)
                        || !HasProperData(projSettings.ShowAnswersInternationalBT, (li = projSettings.InternationalBT), theAnswerData.InternationalBt))
                    {
                        MessageBox.Show(String.Format(Properties.Resources.IDS_DataMissing,
                                                      Environment.NewLine,
                                                      li.LangName,
                                                      "Answer",
                                                      nVerseNumber));
                        return false;
                    }
                    nVerseNumber++;
                }
            }
            return true;
        }

        private static bool CheckForCompletionRetelling(StoryEditor theSe, StoryProjectData theStoryProjectData, StoryData theStory)
        {
            // until I figure out how to do this properly, for now just check to make sure there
            //  *are* retellings.
            int nVerseNumber = 1;
            foreach (var aVerse in theStory.Verses)
            {
                if (!aVerse.IsVisible) 
                    continue;

                // if there are no retellings, then we have a problem
                if (aVerse.Retellings.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.IDS_CantHaveNoRetellings,
                                    OseResources.Properties.Resources.IDS_Caption);
                    return false;
                }

                // then make sure that all fields configured are filled
                int nLastIndex = aVerse.Retellings.Count - 1;
                ProjectSettings.LanguageInfo li;
                LineData theRetellingData = aVerse.Retellings[nLastIndex];
                ProjectSettings projSettings = theStoryProjectData.ProjSettings;
                if (!HasProperData(projSettings.ShowRetellingVernacular, (li = projSettings.Vernacular), theRetellingData.Vernacular)
                    || !HasProperData(projSettings.ShowRetellingNationalBT, (li = projSettings.NationalBT), theRetellingData.NationalBt)
                    || !HasProperData(projSettings.ShowRetellingInternationalBT, (li = projSettings.InternationalBT), theRetellingData.InternationalBt))
                {
                    MessageBox.Show(String.Format(Properties.Resources.IDS_DataMissing,
                                                  Environment.NewLine,
                                                  li.LangName,
                                                  "Retelling",
                                                  nVerseNumber));
                    return false;
                }
                nVerseNumber++;
            }

            return true;
        }

        private static bool HasProperData(bool isShowing, 
            ProjectSettings.LanguageInfo li, StringTransfer FieldData)
        {
            return !isShowing || FieldData.HasData;
        }

        private static bool CheckForCompletionTestQuestions(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory)
        {
            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theStory.CraftingInfo.IsBiblicalStory)
                return true;

            // there should be at least half as many questions as there are verses.
            if (!CheckEndOfStateTransition.CheckForCountOfTestingQuestions(theStory, theSe))
                return false;

            // make sure the TQs that are there are filled in for all requested languages
            int nVerseNumber = 1;
            foreach (var aVerse in theStory.Verses)
            {
                // skip the invisible ones and/or the ones with no testing questions
                if (!aVerse.IsVisible || !aVerse.TestQuestions.HasData)
                    continue;

                // then make sure that all fields configured are filled
                int nLastIndex = aVerse.TestQuestions.Count - 1;
                ProjectSettings.LanguageInfo li;
                LineData theTqData = aVerse.TestQuestions[nLastIndex].TestQuestionLine;
                ProjectSettings projSettings = theStoryProjectData.ProjSettings;
                if (!HasProperData(projSettings.ShowTestQuestionsVernacular, (li = projSettings.Vernacular), theTqData.Vernacular)
                    || !HasProperData(projSettings.ShowTestQuestionsNationalBT, (li = projSettings.NationalBT), theTqData.NationalBt)
                    || !HasProperData(projSettings.ShowTestQuestionsInternationalBT, (li = projSettings.InternationalBT), theTqData.InternationalBt))
                {
                    MessageBox.Show(String.Format(Properties.Resources.IDS_DataMissing,
                                                  Environment.NewLine,
                                                  li.LangName,
                                                  "Test Question",
                                                  nVerseNumber));
                    return false;
                }
                nVerseNumber++;
            }

            return true;
        }

        private static bool CheckForCompletionAnchors(StoryEditor theSe, 
            StoryData theStory)
        {
            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theStory.CraftingInfo.IsBiblicalStory)
                return true;

            // for each verse, make sure that there is at least one anchor.
            int nVerseNumber = 1;
            foreach (VerseData aVerseData in theStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    if (aVerseData.Anchors.Count == 0)
                    {
                        CheckEndOfStateTransition.ShowError(theSe,
                                  String.Format(Properties.Resources.IDS_NoAnchor,
                                                nVerseNumber));
                        theSe.FocusOnVerse(nVerseNumber, true, true);
                        return false;
                    }
                }
                nVerseNumber++;
            }

            return true;
        }

        private bool CheckForCompletion(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory, 
            StoryEditor.TextFieldType fieldToCheck, StoryEditor.TextFieldType fieldHighest,
            ref bool bTriggerRefresh)
        {
            if (!CheckStoryLinesForCompletion(theSe, theStoryProjectData, theStory,
                                              fieldToCheck, fieldHighest, ref bTriggerRefresh))
            {
                return false;
            }

            // if this isn't the highest field, then we also have to make sure that if 
            //  the highest field had something, that the field to check also does.
            if (fieldToCheck != fieldHighest)
            {
                return CheckFieldForCompletion(theSe, theStoryProjectData, theStory,
                                               fieldToCheck, fieldHighest);
            }
            return true;
        }

        private bool CheckFieldForCompletion(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory, 
            StoryEditor.TextFieldType fieldToCheck, StoryEditor.TextFieldType fieldHighest)
        {
            int nVerseNumber = 1; // this wants to be 1, because it's dealing with the
            foreach (var aVerseData in theStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    StringTransfer stCheck = GetStringTransfer(aVerseData.StoryLine, fieldToCheck);
                    StringTransfer stHighest = GetStringTransfer(aVerseData.StoryLine, fieldHighest);
                    if (stHighest.HasData && !stCheck.HasData)
                    {
                        ProjectSettings.LanguageInfo liCheck = GetLanguageInfo(theStoryProjectData.ProjSettings, fieldToCheck);
                        ProjectSettings.LanguageInfo liHighest = GetLanguageInfo(theStoryProjectData.ProjSettings, fieldHighest);

                        CheckEndOfStateTransition.ShowErrorFocus(theSe, stCheck.TextBox,
                                                                 String.Format(
                                                                     Properties.Resources.IDS_FieldCantBeEmpty,
                                                                     liCheck.LangName, nVerseNumber, liHighest.LangName));
                        return false;
                    }
                }

                nVerseNumber++;
            }
            return true;
        }

        /// <summary>
        /// This method basically checks to make sure that there aren't any multiple
        /// sentences/line or empty lines (and that there are more than 1 line).
        /// </summary>
        /// <param name="theSe"></param>
        /// <param name="theStoryProjectData"></param>
        /// <param name="theStory"></param>
        /// <param name="fieldToCheck"></param>
        /// <returns></returns>
        private bool CheckStoryLinesForCompletion(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory,
            StoryEditor.TextFieldType fieldToCheck, StoryEditor.TextFieldType fieldHighest,
            ref bool bTriggerRefresh)
        {
            bool bRepeatAfterMe = false;
            var lstDontCheck = new List<int>();
            do
            {
                // if there are no verses... that can't be good
                if (theStory.Verses.Count < 2)
                {
                    CheckEndOfStateTransition.ShowError(theSe, Properties.Resources.IDS_NoMultipleLines);
                    return false;
                }

                int nVerseNumber = 1; // this wants to be 1, because it's dealing with the
                // VerseBT pane, which starts at 1.
                foreach (VerseData aVerseData in theStory.Verses)
                {
                    if (aVerseData.IsVisible && !lstDontCheck.Contains(nVerseNumber))
                    {
                        List<string> lstSentences;
                        ProjectSettings.LanguageInfo li = GetLanguageInfo(theStoryProjectData.ProjSettings, fieldToCheck);
                        StringTransfer st = GetStringTransfer(aVerseData.StoryLine, fieldToCheck);
                        if (CheckEndOfStateTransition.GetListOfSentences(
                            st, li.FullStop, out lstSentences))
                        {
                            // see if the user wants us to split it into lines for them
                            //  (but only if they haven't already said 'no' for this line)
                            if (lstSentences.Count > 1)
                            {
                                DialogResult res = MessageBox.Show(
                                    String.Format(Properties.Resources.IDS_QuerySplitMultipleLines,
                                                  li.LangName, nVerseNumber),
                                    OseResources.Properties.Resources.IDS_Caption,
                                    MessageBoxButtons.YesNoCancel);

                                // cancel stops the process ('no' just means I don't care 
                                //  that there's multiple sentences/line)
                                if (res == DialogResult.Cancel)
                                    return false;

                                if (res == DialogResult.No)
                                    lstDontCheck.Add(nVerseNumber);

                                else if (res == DialogResult.Yes)
                                {
                                    int nNewVerses = lstSentences.Count;
                                    while (nNewVerses-- > 1)
                                    {
                                        string strSentence = lstSentences[nNewVerses];
                                        theStory.Verses.InsertVerse(nVerseNumber, strSentence, null, null, null);
                                    }

                                    aVerseData.StoryLine.Vernacular.SetValue(lstSentences[nNewVerses]);
                                    bTriggerRefresh = bRepeatAfterMe = true;
                                    break; // we have to exit the loop since we've modified the collection
                                }
                            }
                        }
                        else
                        {
                            // ... no data this row
                            if (!aVerseData.HasData)
                            {
                                // if the rest of the verse is empty, then don't even ask
                                theStory.Verses.Remove(aVerseData);
                                bTriggerRefresh = bRepeatAfterMe = true;
                                break; // we have to exit the loop since we've modified the collection
                            }

                            // when we're checking the highest field, we might want to 
                            //  ask about hiding empty lines. but when it's a subordinate
                            //  line we don't (that problem will get flagged during
                            //  the other check
                            if (fieldToCheck == fieldHighest)
                            {
                                DialogResult res = MessageBox.Show(
                                    String.Format(Properties.Resources.IDS_HideEmptyLine,
                                                  li.LangName, nVerseNumber),
                                    OseResources.Properties.Resources.IDS_Caption,
                                    MessageBoxButtons.YesNoCancel);

                                if (res == DialogResult.Cancel)
                                    return false;

                                if (res == DialogResult.No)
                                    lstDontCheck.Add(nVerseNumber);

                                else if (res == DialogResult.Yes)
                                {
                                    aVerseData.IsVisible = false;
                                    bTriggerRefresh = true;
                                }
                            }
                        }
                    }

                    nVerseNumber++;
                    bRepeatAfterMe = false; // if we get this far without a problem, then we haven't changed anything
                }
            } while (bRepeatAfterMe);

            return true;
        }

        // the highest language is based on which is required according to this
        //  implicational scale: 
        //  vernacular > nationalbt > internationalbt > free translation
        private StoryEditor.TextFieldType HighestLanguage
        {
            get
            {
                if (DoVernacularLangFields)
                    return StoryEditor.TextFieldType.Vernacular;
                if (DoNationalBtLangFields)
                    return StoryEditor.TextFieldType.NationalBt;
                return DoInternationalBtFields 
                    ? StoryEditor.TextFieldType.InternationalBt 
                    : StoryEditor.TextFieldType.FreeTranslation;
            }
        }

        private StringTransfer GetStringTransfer(LineData theLineData,
            StoryEditor.TextFieldType highestLanguage)
        {
            switch (highestLanguage)
            {
                case StoryEditor.TextFieldType.Vernacular:
                    return theLineData.Vernacular;
                case StoryEditor.TextFieldType.NationalBt:
                    return theLineData.NationalBt;
                case StoryEditor.TextFieldType.InternationalBt:
                    return theLineData.InternationalBt;
                case StoryEditor.TextFieldType.FreeTranslation:
                    return theLineData.FreeTranslation;
            }
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        private ProjectSettings.LanguageInfo GetLanguageInfo(
            ProjectSettings projSettings, 
            StoryEditor.TextFieldType highestLanguage)
        {
            switch (highestLanguage)
            {
                case StoryEditor.TextFieldType.Vernacular:
                    return projSettings.Vernacular;
                case StoryEditor.TextFieldType.NationalBt:
                    return projSettings.NationalBT;
                case StoryEditor.TextFieldType.InternationalBt:
                    return projSettings.InternationalBT;
                case StoryEditor.TextFieldType.FreeTranslation:
                    return projSettings.FreeTranslation;
            }
            System.Diagnostics.Debug.Assert(false);
            return null;
        }
    }
}
