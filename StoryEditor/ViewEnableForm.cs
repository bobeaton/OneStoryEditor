using System;
using System.Windows.Forms;
using NetLoc;
using System.Linq;

namespace OneStoryProjectEditor
{
    public partial class ViewEnableForm : Form
    {
        private readonly ProjectSettings _projSettings;
        private readonly StoryData _theCurrentStory;

        private ViewEnableForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public ViewEnableForm(StoryEditor theSe, ProjectSettings projSettings,
            StoryData theCurrentStory, bool bUseForAllStories)
        {
            _projSettings = projSettings;
            _theCurrentStory = theCurrentStory;
            InitializeComponent();
            Localizer.Ctrl(this);

            Program.InitializeLangCheckBoxes(projSettings.Vernacular,
                                             checkBoxLangVernacular,
                                             checkBoxLangTransliterateVernacular,
                                             theSe.viewTransliterationVernacular,
                                             theSe.LoggedOnMember.TransliteratorVernacular);

            Program.InitializeLangCheckBoxes(projSettings.NationalBT,
                                             checkBoxLangNationalBT, 
                                             checkBoxLangTransliterateNationalBT,
                                             theSe.viewTransliterationNationalBT,
                                             theSe.LoggedOnMember.TransliteratorNationalBt);

            Program.InitializeLangCheckBoxes(projSettings.InternationalBT,
                                             checkBoxLangInternationalBT,
                                             checkBoxLangTransliterateInternationalBt,
                                             theSe.viewTransliterationInternationalBt,
                                             theSe.LoggedOnMember.TransliteratorInternationalBt);

            Program.InitializeLangCheckBoxes(projSettings.FreeTranslation,
                                             checkBoxLangFreeTranslation,
                                             checkBoxLangTransliterateFreeTranslation,
                                             theSe.viewTransliterationFreeTranslation,
                                             theSe.LoggedOnMember.TransliteratorFreeTranslation);

            checkBoxConsultantNotes.Enabled = (theCurrentStory != null);
            checkBoxCoachNotes.Enabled = (theCurrentStory != null) &&
                                         !theSe.LoggedOnMember.IsPfAndNotLsr;

            checkBoxUseForAllStories.Checked = bUseForAllStories;

            checkBoxShowHidden.Checked = theSe.viewHiddenVersesMenu.Checked;

            checkBoxOpenConNotesOnly.Checked = theSe.viewOnlyOpenConversationsMenu.Checked;
        }

        public void InitializeForQueryingFieldsSelected()
        {
            ViewSettings = new VerseData.ViewSettings((long)0);
            Text = Localizer.Str("Select the fields that you want to delete");
            checkBoxGeneralTestingQuestions.Text = Localizer.Str("General Testing Questions and Answers");
            checkBoxStoryTestingQuestions.Text = Localizer.Str("Story Testing Questions and Answers");
            checkBoxAnswers.Text = Localizer.Str("All Question (General and Story) Answers");
            checkBoxShowHidden.Text = Localizer.Str("Hidden Lines");
            checkBoxLangTransliterateVernacular.Visible =
                checkBoxLangTransliterateNationalBT.Visible =
                checkBoxLangTransliterateInternationalBt.Visible =
                checkBoxLangTransliterateFreeTranslation.Visible = false;
            checkBoxUseForAllStories.Visible = false;
            checkBoxOpenConNotesOnly.Visible = false;
            checkBoxBibleViewer.Visible = false;
            checkBoxShowHidden.Checked = false;
        }

        public void InitializeForQueryingFieldsToCopy()
        {
            Text = Localizer.Str("Select the fields that you want to copy");
            checkBoxGeneralTestingQuestions.Text = Localizer.Str("General Testing Questions");
            checkBoxStoryTestingQuestions.Text = Localizer.Str("Story Testing Questions");
            checkBoxAnswers.Text = Localizer.Str("All Question (General and Story) Answers");
            checkBoxShowHidden.Text = Localizer.Str("Hidden Lines");
            checkBoxLangTransliterateVernacular.Visible =
                checkBoxLangTransliterateNationalBT.Visible =
                checkBoxLangTransliterateInternationalBt.Visible =
                checkBoxLangTransliterateFreeTranslation.Visible = false;
            checkBoxUseForAllStories.Visible = false;
            checkBoxOpenConNotesOnly.Visible = false;
            checkBoxBibleViewer.Visible = false;
            checkBoxShowHidden.Checked = false;
            checkBoxCoachNotes.Enabled = true;      // allow them to edit this if they're copying from another project (so they can uncheck it if they don't want it)

            // pre-check the boxes for which data exists
            ViewSettings = new VerseData.ViewSettings(_projSettings,
                _theCurrentStory.Verses.Any(v => v.StoryLine.Vernacular.HasData),
                _theCurrentStory.Verses.Any(v => v.StoryLine.NationalBt.HasData),
                _theCurrentStory.Verses.Any(v => v.StoryLine.InternationalBt.HasData),
                _theCurrentStory.Verses.Any(v => v.StoryLine.FreeTranslation.HasData),
                _theCurrentStory.Verses.Any(v => v.Anchors.HasData),
                _theCurrentStory.Verses.Any(v => v.ExegeticalHelpNotes.HasData),
                _theCurrentStory.Verses.Any(v => v.TestQuestions.HasData),
                _theCurrentStory.Verses.Any(v => v.TestQuestions.Any(tq => tq.Answers.HasData)),
                _theCurrentStory.Verses.Any(v => v.Retellings.HasData),
                _theCurrentStory.Verses.Any(v => v.ConsultantNotes.HasData),
                _theCurrentStory.Verses.Any(v => v.CoachNotes.HasData),
                false,
                false,
                _theCurrentStory.Verses.Any(v => !v.IsVisible),
                false, 
                _theCurrentStory.Verses.FirstVerse.TestQuestions.HasData,
                true,
                0,
                null, null, null, null, false
                );
        }

        public bool UseForAllStories
        {
            get;
            set;
        }

        private VerseData.ViewSettings _viewSettings;
        public VerseData.ViewSettings ViewSettings
        {
            set
            {
                _viewSettings = value;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                    checkBoxLangVernacular.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularTransliterationField))
                    checkBoxLangTransliterateVernacular.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField))
                    checkBoxLangNationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtTransliterationField))
                    checkBoxLangTransliterateNationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField))
                    checkBoxLangInternationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtTransliterationField))
                    checkBoxLangTransliterateInternationalBt.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                    checkBoxLangFreeTranslation.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationTransliterationField))
                    checkBoxLangTransliterateFreeTranslation.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields))
                    checkBoxAnchors.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps))
                    checkBoxExegeticalNotes.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions))
                    checkBoxGeneralTestingQuestions.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions))
                    checkBoxStoryTestingQuestions.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
                    checkBoxAnswers.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.RetellingFields))
                    checkBoxRetellings.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ConsultantNoteFields))
                    checkBoxConsultantNotes.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.CoachNotesFields))
                    checkBoxCoachNotes.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.BibleViewer))
                    checkBoxBibleViewer.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.HiddenStuff))
                    checkBoxShowHidden.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.OpenConNotesOnly))
                    checkBoxOpenConNotesOnly.Checked = true;
            }

            get
            {
                _viewSettings.SetItemsToInsureOn(
                    _projSettings,
                    checkBoxLangVernacular.Checked,
                    checkBoxLangTransliterateVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangTransliterateNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked, 
                    checkBoxLangTransliterateInternationalBt.Checked,
                    checkBoxLangFreeTranslation.Checked,
                    checkBoxLangTransliterateFreeTranslation.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxExegeticalNotes.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    checkBoxConsultantNotes.Checked,
                    checkBoxCoachNotes.Checked,
                    checkBoxBibleViewer.Checked,
                    true,
                    checkBoxShowHidden.Checked,
                    checkBoxOpenConNotesOnly.Checked,
                    checkBoxGeneralTestingQuestions.Checked,
                    false,  // I think this is a don't-care value (for here)
                    true); 
                return _viewSettings;
            }
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            UseForAllStories = checkBoxUseForAllStories.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
