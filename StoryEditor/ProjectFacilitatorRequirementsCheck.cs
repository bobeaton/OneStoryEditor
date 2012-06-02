using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

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

        public abstract bool CheckIfRequirementsAreMet(bool bGoingToMentor);
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

        public override bool CheckIfRequirementsAreMet(bool bGoingToMentor)
        {
            bool bTriggerRefresh = false;
            StoryProjectData theStoryProjectData = TheSe.StoryProject;
            try
            {
                if (DoVernacularLangFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFields.Vernacular,
                        StoryEditor.TextFields.Vernacular, ref bTriggerRefresh))  // by definition is the highest
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoNationalBtLangFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFields.NationalBt,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoInternationalBtFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFields.InternationalBt,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoFreeTranslationFields)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    if (!CheckForCompletion(TheSe, theStoryProjectData, TheStory,
                        StoryEditor.TextFields.FreeTranslation,
                        HighestLanguage, ref bTriggerRefresh))
                        throw StoryProjectData.BackOutWithNoUI;
                }

                if (DoAnchors)
                {
                    // for this one, make sure that every line of the story has something in the vernacular field
                    CheckForCompletionAnchors(TheSe, TheStory);
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
                if (bGoingToMentor)
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
                ThrowViewExceptionLackingTests(theStory.CountTestingQuestionTests,
                                   Localizer.Str("story question"),
                                   TheSe.viewStoryTestingQuestionAnswersMenu);
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
                        ThrowExceptionDataMissing(TheSe.viewStoryTestingQuestionAnswersMenu,
                                                  nVerseNumber, li.LangName,
                                                  Localizer.Str("Answer"));
                    }
                }
            }
        }

        private static void ThrowExceptionDataMissing(ToolStripMenuItem tsmi, int nVerseNumber, string strLangName,
            string strDataType)
        {
            throw new ExceptionWithViewToTurnOn(String.Format(Localizer.Str("The '{1}' language field of the {2} in line '{3}' is empty. Did you forget it?{0}{0}(if you don't mean to enter data in the '{1}' field, then click 'Project', 'Settings' and on the 'Languages' tab, uncheck the box for that language in the '{2}s' column)"),
                                                              Environment.NewLine,
                                                              strLangName,
                                                              strDataType,
                                                              nVerseNumber))
                      {
                          ViewToTurnOn = tsmi,
                          VerseToScrollTo = nVerseNumber
                      };
        }

        private static void ThrowViewExceptionLackingTests(int nCountOfTests, string strTestType, 
            ToolStripMenuItem tsmi)
        {
            throw new ExceptionWithViewToTurnOn(String.Format(Localizer.Str("The consultant is requiring you to do {0} more {1} test(s)"),
                                                              nCountOfTests, strTestType)) 
                                                              {ViewToTurnOn = tsmi};
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
                ThrowViewExceptionLackingTests(theStory.CountRetellingsTests,
                                   Localizer.Str("retelling"),
                                   theSe.viewRetellingsMenu);
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
                        ThrowExceptionDataMissing(theSe.viewRetellingsMenu,
                                                  nVerseNumber, li.LangName,
                                                  Localizer.Str("Retelling"));
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
                        ThrowExceptionDataMissing(theSe.viewStoryTestingQuestionsMenu,
                                                  nVerseNumber, li.LangName,
                                                  Localizer.Str("Testing Question"));
                    }
                }
            }

            return true;
        }

        private static void CheckForCompletionAnchors(StoryEditor theSe, 
            StoryData theStory)
        {
            // this can happen if the person has a story in this state, but then changes it to be a non-biblical story
            if (!theStory.CraftingInfo.IsBiblicalStory)
                return;

            // for each verse, make sure that there is at least one anchor.
            int nVerseNumber = 1;
            foreach (var aVerseData in theStory.Verses)
            {
                if (aVerseData.IsVisible)
                {
                    if (aVerseData.Anchors.Count == 0)
                    {
                        throw new ExceptionWithViewToTurnOn(NoAnchorMessage(nVerseNumber))
                                  {
                                      ViewToTurnOn = theSe.viewAnchorsMenu,
                                      VerseToScrollTo = nVerseNumber
                                  };
                    }
                }
                nVerseNumber++;
            }
        }

        public static string NoAnchorMessage(int nVerseNumber)
        {
            return String.Format(Localizer.Str("Line '{0}' doesn't have an anchor. Did you forget it?"),
                                 nVerseNumber);
        }

        private bool CheckForCompletion(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory,
            StoryEditor.TextFields fieldToCheck, StoryEditor.TextFields fieldHighest,
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
                if (!CheckFieldForCompletion(theSe, theStoryProjectData, theStory,
                                             fieldToCheck, fieldHighest))
                    return false;

                // make sure the PF has specified the UNS back-translator (this only 
                //  shows something if nothing is configured (but if the user keeps 
                //  cancelling the dialog, theoretically, it will come up multiple times)
                CheckEndOfStateTransition.QueryForUnsBackTranslator(TheSe, theStoryProjectData, TheStory);

            }
            return true;
        }

        private bool CheckFieldForCompletion(StoryEditor theSe, 
            StoryProjectData theStoryProjectData, StoryData theStory,
            StoryEditor.TextFields fieldToCheck, StoryEditor.TextFields fieldHighest)
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

                        var strError = String.Format(
                            Localizer.Str(
                                "The '{0}' field of line '{1}' is empty, but the '{2}' field of that same line is not. Did you forget to enter the back-translation?"),
                            liCheck.LangName, nVerseNumber, liHighest.LangName);
                        if (theSe.UsingHtmlForStoryBtPane)
                            CheckEndOfStateTransition.ShowErrorFocus(theSe, nVerseNumber, stCheck.WhichField, strError);
                        else
                            CheckEndOfStateTransition.ShowErrorFocus(theSe, stCheck.TextBox, strError);
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
            StoryEditor.TextFields fieldToCheck, StoryEditor.TextFields fieldHighest,
            ref bool bTriggerRefresh)
        {
            // if there are no verses... that can't be good
            if (theStory.Verses.Count < 2)
            {
                CheckEndOfStateTransition.ShowError(theSe,
                    Localizer.Str("Your story doesn't have multiple lines! Did you forget to do: 'Story', 'Split into lines'?"));
                return false;
            }

            if (StoryEditor.WillBeLossInVerse(theStory.Verses))
                return true;

            bool bRepeatAfterMe = false;
            var lstDontCheck = new List<int>();
            do
            {
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
                                DialogResult res = CheckEndOfStateTransition.QuerySplitIntoLines(li.LangName, nVerseNumber);

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
                                DialogResult res = LocalizableMessageBox.Show(
                                    String.Format(
                                        Localizer.Str(
                                            "The '{0}' field of line '{1}' is empty. Click 'Yes' to hide the line? Click 'No' to ignore and continue"),
                                        li.LangName, nVerseNumber),
                                    StoryEditor.OseCaption,
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
        private StoryEditor.TextFields HighestLanguage
        {
            get
            {
                if (DoVernacularLangFields)
                    return StoryEditor.TextFields.Vernacular;
                if (DoNationalBtLangFields)
                    return StoryEditor.TextFields.NationalBt;
                return DoInternationalBtFields
                    ? StoryEditor.TextFields.InternationalBt
                    : StoryEditor.TextFields.FreeTranslation;
            }
        }

        private StringTransfer GetStringTransfer(LineData theLineData,
            StoryEditor.TextFields highestLanguage)
        {
            switch (highestLanguage & StoryEditor.TextFields.Languages)
            {
                case StoryEditor.TextFields.Vernacular:
                    return theLineData.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return theLineData.NationalBt;
                case StoryEditor.TextFields.InternationalBt:
                    return theLineData.InternationalBt;
                case StoryEditor.TextFields.FreeTranslation:
                    return theLineData.FreeTranslation;
            }
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        private ProjectSettings.LanguageInfo GetLanguageInfo(
            ProjectSettings projSettings,
            StoryEditor.TextFields highestLanguage)
        {
            switch (highestLanguage & StoryEditor.TextFields.Languages)
            {
                case StoryEditor.TextFields.Vernacular:
                    return projSettings.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return projSettings.NationalBT;
                case StoryEditor.TextFields.InternationalBt:
                    return projSettings.InternationalBT;
                case StoryEditor.TextFields.FreeTranslation:
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
