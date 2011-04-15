using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool DoInternationalBtFields;
        public readonly bool DoFreeTranslationFields;
        public readonly bool DoAnchors;
        public readonly bool DoRetelling;
        public readonly bool DoTestQuestions;
        public readonly bool DoAnswers;

        public ProjectFacilitatorRequirementsCheck(StoryEditor theSe, StoryData theStory)
            :base(theSe, theStory)
        {
            ProjectSettings projSettings = TheSe.StoryProject.ProjSettings;

            DoVernacularLangFields = projSettings.Vernacular.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                         TasksPf.TaskSettings.
                                                                                             VernacularLangFields);
            DoNationalBtLangFields = projSettings.NationalBT.HasData && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                                                         TasksPf.TaskSettings.
                                                                                             NationalBtLangFields);
            DoInternationalBtFields = projSettings.InternationalBT.HasData
                                      && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                          TasksPf.TaskSettings.
                                                              InternationalBtFields)
                                      && !TheSe.StoryProject.TeamMembers.HasOutsideEnglishBTer;

            DoFreeTranslationFields = projSettings.FreeTranslation.HasData
                                      && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                          TasksPf.TaskSettings.
                                                              FreeTranslationFields);

            DoAnchors = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                         TasksPf.TaskSettings.Anchors);

            DoRetelling = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                           TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2);

            DoTestQuestions = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                               TasksPf.TaskSettings.TestQuestions);

            DoAnswers = TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                         TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2);
        }

        public override bool CheckIfRequirementsAreMet()
        {
            bool bTriggerRefresh = false;
            StoryProjectData theStoryProjectData = TheSe.StoryProject;
            try
            {
                // make sure the PF has specified the UNS back-translator
                CheckEndOfStateTransition.QueryForUnsBackTranslator(TheSe, theStoryProjectData, TheStory);

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
                    CheckForCompletionRetelling(TheSe, theStoryProjectData, TheStory);
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
                    CheckForCompletionAnswers(TheSe, theStoryProjectData, TheStory);
                }

                // finally, they have to have responded to all of the CITs comments
                if (!CheckEndOfStateTransition.CheckThatPFRespondedToCITQuestions(TheSe, TheStory))
                    throw StoryProjectData.BackOutWithNoUI;
            }
            catch (StoryProjectData.BackOutWithNoUIException)
            {
                return false;
            }
            catch (ExceptionWithViewToTurnOn ex)
            {
                ex.ViewToTurnOn.Checked = true; // turn on any needed views
                TheSe.FocusOnVerse(ex.VerseToScrollTo, false, false);
                Program.ShowException(ex);
                return false;
            }
            finally
            {
                if (bTriggerRefresh)
                    TheSe.refreshToolStripMenuItem_Click(null, null);
            }

            return true;
        }

        private class ExceptionWithViewToTurnOn : ApplicationException
        {
            public ToolStripMenuItem ViewToTurnOn { get; set; }
            public int VerseToScrollTo { get; set; }

            public ExceptionWithViewToTurnOn(string strError)
                : base(strError)
            {
            }
        }

        private static void CheckForCompletionAnswers(StoryEditor TheSe, 
            StoryProjectData theStoryProjectData, StoryData theStory)
        {
            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theStory.CraftingInfo.IsBiblicalStory)
                return;

            // the first/easiest check is that the count of question tests should be 0
            if (theStory.CountTestingQuestionTests > 0)
            {
                throw new ExceptionWithViewToTurnOn(String.Format(Properties.Resources.IDS_DoXMoreTqTests,
                                                                  theStory.CountTestingQuestionTests,
                                                                  "story question"))
                          {ViewToTurnOn = TheSe.viewStoryTestingQuestionAnswerMenuItem};
            }

            for (int nVerseNumber = 1; nVerseNumber <= theStory.Verses.Count; nVerseNumber++)
            {
                var aVerseData = theStory.Verses[nVerseNumber - 1];
                if (!aVerseData.IsVisible || !aVerseData.TestQuestions.HasData)
                    continue;

                foreach (var theAnswerData in aVerseData.TestQuestions.SelectMany(aTq => 
                    aTq.Answers))
                {
                    ProjectSettings.LanguageInfo li;
                    var projSettings = theStoryProjectData.ProjSettings;
                    if (!HasProperData(projSettings.ShowAnswers.Vernacular, (li = projSettings.Vernacular), theAnswerData.Vernacular)
                        || !HasProperData(projSettings.ShowAnswers.NationalBt, (li = projSettings.NationalBT), theAnswerData.NationalBt)
                        || !HasProperData(projSettings.ShowAnswers.InternationalBt, (li = projSettings.InternationalBT), theAnswerData.InternationalBt))
                    {
                        throw new ExceptionWithViewToTurnOn(String.Format(Properties.Resources.IDS_DataMissing,
                                                                          Environment.NewLine,
                                                                          li.LangName,
                                                                          "Answer",
                                                                          nVerseNumber))
                                  {
                                      ViewToTurnOn = TheSe.viewStoryTestingQuestionAnswerMenuItem,
                                      VerseToScrollTo = nVerseNumber
                                  };
                    }
                }
            }
        }

        private static void CheckForCompletionRetelling(StoryEditor theSe, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theStory.CraftingInfo.IsBiblicalStory)
                return;

            // the first/easiest check is that the count of retelling tests should be 0
            if (theStory.CountRetellingsTests > 0)
            {
                throw new ExceptionWithViewToTurnOn(String.Format(Properties.Resources.IDS_DoXMoreTqTests,
                                                                  theStory.CountRetellingsTests,
                                                                  "retelling"))
                          {ViewToTurnOn = theSe.viewRetellingFieldMenuItem};
            }

            // make sure the TQs that are there are filled in for all requested languages
            for (int nVerseNumber = 1; nVerseNumber <= theStory.Verses.Count; nVerseNumber++)
            {
                var aVerse = theStory.Verses[nVerseNumber - 1];

                if (!aVerse.IsVisible) 
                    continue;

                // but at least make sure that all fields configured are filled
                foreach (var theRetellingData in aVerse.Retellings)
                {
                    ProjectSettings.LanguageInfo li;
                    ProjectSettings projSettings = theStoryProjectData.ProjSettings;
                    if (!HasProperData(projSettings.ShowRetellings.Vernacular, (li = projSettings.Vernacular), theRetellingData.Vernacular)
                        || !HasProperData(projSettings.ShowRetellings.NationalBt, (li = projSettings.NationalBT), theRetellingData.NationalBt)
                        || !HasProperData(projSettings.ShowRetellings.InternationalBt, (li = projSettings.InternationalBT), theRetellingData.InternationalBt))
                    {
                        throw new ExceptionWithViewToTurnOn(String.Format(Properties.Resources.IDS_DataMissing,
                                                                          Environment.NewLine,
                                                                          li.LangName,
                                                                          "Retelling",
                                                                          nVerseNumber))
                                  {
                                      ViewToTurnOn = theSe.viewRetellingFieldMenuItem,
                                      VerseToScrollTo = nVerseNumber
                                  };
                    }
                }
            }
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
            for (int nVerseNumber = 1; nVerseNumber <= theStory.Verses.Count; nVerseNumber++)
            {
                var aVerse = theStory.Verses[nVerseNumber - 1];
                
                // skip the invisible ones and/or the ones with no testing questions
                if (!aVerse.IsVisible || !aVerse.TestQuestions.HasData)
                    continue;

                foreach (TestQuestionData t in aVerse.TestQuestions)
                {
                    // then make sure that all fields configured are filled
                    ProjectSettings.LanguageInfo li;
                    LineData theTqData = t.TestQuestionLine;
                    ProjectSettings projSettings = theStoryProjectData.ProjSettings;
                    if (!HasProperData(projSettings.ShowTestQuestions.Vernacular, (li = projSettings.Vernacular),
                                       theTqData.Vernacular)
                        ||
                        !HasProperData(projSettings.ShowTestQuestions.NationalBt, (li = projSettings.NationalBT),
                                       theTqData.NationalBt)
                        ||
                        !HasProperData(projSettings.ShowTestQuestions.InternationalBt, (li = projSettings.InternationalBT),
                                       theTqData.InternationalBt))
                    {
                        throw new ExceptionWithViewToTurnOn(String.Format(Properties.Resources.IDS_DataMissing,
                                                                          Environment.NewLine,
                                                                          li.LangName,
                                                                          "Test Question",
                                                                          nVerseNumber))
                                  {
                                      ViewToTurnOn = theSe.viewStoryTestingQuestionMenuItem,
                                      VerseToScrollTo = nVerseNumber
                                  };
                    }
                }
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
            if (StoryEditor.WillBeLossInVerse(theStory.Verses))
                return true;

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

    public class EnglishBterRequirementsCheck : ProjectFacilitatorRequirementsCheck
    {
        public EnglishBterRequirementsCheck(StoryEditor theSe, StoryData theStory)
            :base(theSe, theStory)
        {
            System.Diagnostics.Debug.Assert(TheSe.StoryProject.TeamMembers.HasOutsideEnglishBTer);

            ProjectSettings projSettings = TheSe.StoryProject.ProjSettings;

            DoInternationalBtFields = projSettings.InternationalBT.HasData
                                      && TasksPf.IsTaskOn(theStory.TasksRequiredPf,
                                                          TasksPf.TaskSettings.
                                                              InternationalBtFields);
        }
    }
}
