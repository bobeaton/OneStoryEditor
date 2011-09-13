using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class ViewEnableForm : Form
    {
        private readonly ProjectSettings _projSettings;

        private ViewEnableForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public ViewEnableForm(StoryEditor theSe, ProjectSettings projSettings,
            StoryData theCurrentStory, bool bUseForAllStories)
        {
            _projSettings = projSettings;
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
                    checkBoxGeneralTestingQuestions.Checked);
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
