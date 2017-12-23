using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SwapColumnsForm : Form
    {
        private SwapColumnsForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public SwapColumnsForm(StoryEditor theSe)
        {
            InitializeComponent();

            var cntOfVisibleLanguages = 0;
            var projSettings = theSe.StoryProject.ProjSettings;
            SetFieldVisibility(radioButtonVernacularTranscription1, radioButtonVernacularTranscription2, projSettings.Vernacular.HasData, ref cntOfVisibleLanguages);
            SetFieldVisibility(radioButtonNationalBtTranscription1, radioButtonNationalBtTranscription2, projSettings.NationalBT.HasData, ref cntOfVisibleLanguages);
            SetFieldVisibility(radioButtonInternationalBtTranscription1, radioButtonInternationalBtTranscription2, projSettings.InternationalBT.HasData, ref cntOfVisibleLanguages);
            SetFieldVisibility(radioButtonFreeTrTranscription1, radioButtonFreeTrTranscription2, projSettings.FreeTranslation.HasData, ref cntOfVisibleLanguages);

            if (cntOfVisibleLanguages < 2)
            {
                LocalizableMessageBox.Show(Localizer.Str("You can't swap columns because this project only has 1 column (language) configured. See 'Project', 'Settings', 'Languages' tab to enable other languages."),
                                           StoryEditor.OseCaption);
            }

            // get just one verse to demonstrate what's going to happen
            var tempStory = new StoryData(theSe.TheCurrentStory);

            // adjust the view settings so that at least the configured languages are visible
            htmlStoryBtControlBefore.TheSE = htmlStoryBtControlAfter.TheSE = theSe;
            htmlStoryBtControlBefore.StoryData = tempStory;
            htmlStoryBtControlBefore.ViewSettings = htmlStoryBtControlAfter.ViewSettings = GetProjectViewSettings(projSettings, theSe);
            htmlStoryBtControlBefore.LoadDocument();
            htmlStoryBtControlBefore.ScrollToVerse(1);

            htmlStoryBtControlAfter.SetLineNumberLink += OnScrollAfter;
            htmlStoryBtControlBefore.SetLineNumberLink = (text, index) => { _nCurrIndexBefore = index; };
        }

        private int _nCurrIndexBefore, _nCurrIndexAfter;
        public void OnScrollAfter(string strText, int nLineIndex)
        {
            // whenever the user scrolls the lower one, have it automatically scroll the upper one
            _nCurrIndexAfter = nLineIndex;
            htmlStoryBtControlBefore.ScrollToVerse(_nCurrIndexAfter);
        }

        private static void SetFieldVisibility(RadioButton radioButtonBefore, RadioButton radioButtonAfter, bool isConfigured, ref int cntOfVisibleLanguages)
        {
            radioButtonBefore.Visible = radioButtonAfter.Visible = isConfigured;

            if (!isConfigured)
                return;

            if (cntOfVisibleLanguages++ == 0)
                radioButtonBefore.Checked = radioButtonAfter.Checked = true;
        }

        private void UpdateAfterDisplay(object sender, EventArgs e)
        {
            var column1 = Column1;
            var column2 = Column2;
            var fieldsToSwap = FieldsToSwap;

            if ((column1 == column2) || (fieldsToSwap == StoryEditor.TextFields.Undefined))
            {
                htmlStoryBtControlAfter.ResetDocument();
                Application.DoEvents(); // give them time to actually empty the webcontrols
                htmlStoryBtControlAfter.ResetDocument();
                SetButtons(false);
            }
            else
            {
                if (column1 == StoryEditor.TextFields.FreeTranslation)
                {
                    checkBoxRetellings.Checked = checkBoxTestQuestions.Checked = checkBoxTestQuestionAnswers.Checked = 
                        checkBoxRetellings.Enabled = checkBoxTestQuestions.Enabled = checkBoxTestQuestionAnswers.Enabled = false;
                }
                else
                {
                    checkBoxRetellings.Enabled = checkBoxTestQuestions.Enabled = checkBoxTestQuestionAnswers.Enabled = true;
                }

                var storyAfterSwap = new StoryData(htmlStoryBtControlBefore.StoryData);
                storyAfterSwap.SwapColumns(column1, column2, fieldsToSwap);
                htmlStoryBtControlAfter.StoryData = storyAfterSwap;
                htmlStoryBtControlAfter.LoadDocument();
                htmlStoryBtControlAfter.ScrollToVerse(1);
                SetButtons(true);
            }
        }

        private void SetButtons(bool value)
        {
            buttonMoveToNextLineAfter.Enabled =
                buttonMoveToPrevLineAfter.Enabled =
                buttonOK.Enabled = value;
        }

        public StoryEditor.TextFields Column1
        {
            get
            {
                if (radioButtonVernacularTranscription1.Checked )
                    return StoryEditor.TextFields.Vernacular;
                if (radioButtonNationalBtTranscription1.Checked)
                    return StoryEditor.TextFields.NationalBt;
                if (radioButtonInternationalBtTranscription1.Checked)
                    return StoryEditor.TextFields.InternationalBt;
                if (radioButtonFreeTrTranscription1.Checked)
                    return StoryEditor.TextFields.FreeTranslation;
                System.Diagnostics.Debug.Fail("shouldn't be able to not have one of these checked!?");
                return StoryEditor.TextFields.Undefined;
            }

            set
            {
                switch (value)
                {
                    case StoryEditor.TextFields.Vernacular:
                        radioButtonVernacularTranscription1.Checked = true;
                        break;
                    case StoryEditor.TextFields.NationalBt:
                        radioButtonNationalBtTranscription1.Checked = true;
                        break;
                    case StoryEditor.TextFields.InternationalBt:
                        radioButtonInternationalBtTranscription1.Checked = true;
                        break;
                    case StoryEditor.TextFields.FreeTranslation:
                        radioButtonFreeTrTranscription1.Checked = true;
                        break;
                }
            }
        }

        public StoryEditor.TextFields Column2
        {
            get
            {
                if (radioButtonVernacularTranscription2.Checked)
                    return StoryEditor.TextFields.Vernacular;
                if (radioButtonNationalBtTranscription2.Checked)
                    return StoryEditor.TextFields.NationalBt;
                if (radioButtonInternationalBtTranscription2.Checked)
                    return StoryEditor.TextFields.InternationalBt;
                if (radioButtonFreeTrTranscription2.Checked)
                    return StoryEditor.TextFields.FreeTranslation;
                System.Diagnostics.Debug.Fail("shouldn't be able to not have one of these checked!?");
                return StoryEditor.TextFields.Undefined;
            }

            set
            {
                switch (value)
                {
                    case StoryEditor.TextFields.Vernacular:
                        radioButtonVernacularTranscription2.Checked = true;
                        break;
                    case StoryEditor.TextFields.NationalBt:
                        radioButtonNationalBtTranscription2.Checked = true;
                        break;
                    case StoryEditor.TextFields.InternationalBt:
                        radioButtonInternationalBtTranscription2.Checked = true;
                        break;
                    case StoryEditor.TextFields.FreeTranslation:
                        radioButtonFreeTrTranscription2.Checked = true;
                        break;
                }
            }
        }

        public StoryEditor.TextFields FieldsToSwap
        {
            get
            {
                var fields = StoryEditor.TextFields.Undefined;
                if (checkBoxStoryLines.Checked)
                    fields |= StoryEditor.TextFields.StoryLine;
                if (checkBoxRetellings.Checked)
                    fields |= StoryEditor.TextFields.Retelling;
                if (checkBoxTestQuestions.Checked)
                    fields |= StoryEditor.TextFields.TestQuestion;
                if (checkBoxTestQuestionAnswers.Checked)
                    fields |= StoryEditor.TextFields.TestQuestionAnswer;
                return fields;
            }

            set
            {
                if (StoryEditor.IsFieldSet(value, StoryEditor.TextFields.StoryLine))
                    checkBoxStoryLines.Checked = true;
                if (StoryEditor.IsFieldSet(value, StoryEditor.TextFields.Retelling))
                    checkBoxRetellings.Checked = true;
                if (StoryEditor.IsFieldSet(value, StoryEditor.TextFields.TestQuestion))
                    checkBoxTestQuestions.Checked = true;
                if (StoryEditor.IsFieldSet(value, StoryEditor.TextFields.TestQuestionAnswer))
                    checkBoxTestQuestionAnswers.Checked = true;
            }
        }

        private void ResetCheckBoxFields()
        {
            checkBoxStoryLines.Checked =
                checkBoxRetellings.Checked =
                checkBoxTestQuestions.Checked =
                checkBoxTestQuestionAnswers.Checked = true;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            var column1 = Column1;
            var column2 = Column2;
            if (((column1 & StoryEditor.TextFields.Languages) == StoryEditor.TextFields.Undefined) ||
                ((column2 & StoryEditor.TextFields.Languages) == StoryEditor.TextFields.Undefined) ||
                (column1 == column2))
            {
                return;
            }

            if ((StoryEditor.IsFieldSet(column1, StoryEditor.TextFields.FreeTranslation) ||
                 StoryEditor.IsFieldSet(column2, StoryEditor.TextFields.FreeTranslation)) &&
                ((FieldsToSwap & ~StoryEditor.TextFields.StoryLine) != StoryEditor.TextFields.Undefined))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Only the 'Story Lines' fields contain a box for 'Free Translation' data. So when swapping the 'Free Translation' field, only the 'Story Lines' field can be checked for swapping."),
                    StoryEditor.OseCaption);

                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        protected VerseData.ViewSettings GetProjectViewSettings(ProjectSettings projSettings, StoryEditor theSe)
        {
            return new VerseData.ViewSettings
                (
                projSettings,
                projSettings.Vernacular.HasData,
                projSettings.NationalBT.HasData,
                projSettings.InternationalBT.HasData,
                projSettings.FreeTranslation.HasData,
                false, // viewAnchorsMenu.Checked,
                false, // viewExegeticalHelps.Checked,
                true, // viewStoryTestingQuestionsMenu.Checked,
                true, // viewStoryTestingQuestionAnswersMenu.Checked,
                true, // viewRetellingsMenu.Checked,
                false, // viewConsultantNotesMenu.Checked,
                false, // viewCoachNotesMenu.Checked,
                false, // viewBibleMenu.Checked,
                false,
                true, // viewHiddenVersesMenu.Checked,
                false, // viewOnlyOpenConversationsMenu.Checked,
                theSe.viewGeneralTestingsQuestionMenu.Checked,
                true, // use textareas
                theSe.CurrentFieldEditability(theSe.TheCurrentStory),
                HtmlStoryBtControl.TransliteratorVernacular,
                HtmlStoryBtControl.TransliteratorNationalBt,
                HtmlStoryBtControl.TransliteratorInternationalBt,
                HtmlStoryBtControl.TransliteratorFreeTranslation
                );
        }

        private void ButtonMoveToNextLineBeforeClick(object sender, EventArgs e)
        {
            if (_nCurrIndexBefore < htmlStoryBtControlBefore.StoryData.Verses.Count)
                _nCurrIndexBefore++;
            htmlStoryBtControlBefore.ScrollToVerse(_nCurrIndexBefore);
        }

        private void ButtonMoveToPrevLineBeforeClick(object sender, EventArgs e)
        {
            if (_nCurrIndexBefore > 1)
                _nCurrIndexBefore--;
            htmlStoryBtControlBefore.ScrollToVerse(_nCurrIndexBefore);
        }

        private void ButtonMoveToNextLineAfterClick(object sender, EventArgs e)
        {
            if (_nCurrIndexAfter < htmlStoryBtControlAfter.StoryData.Verses.Count)
                _nCurrIndexAfter++;
            htmlStoryBtControlAfter.ScrollToVerse(_nCurrIndexAfter);
        }

        private void ButtonMoveToPrevLineAfterClick(object sender, EventArgs e)
        {
            if (_nCurrIndexAfter > 1)
                _nCurrIndexAfter--;
            htmlStoryBtControlAfter.ScrollToVerse(_nCurrIndexAfter);
        }
    }
}
