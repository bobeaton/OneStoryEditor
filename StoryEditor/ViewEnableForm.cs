using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ViewEnableForm : Form
    {
        private ProjectSettings _projSettings;
        public ViewEnableForm(StoryEditor theSE, ProjectSettings projSettings, StoryData theCurrentStory, bool bUseForAllStories)
        {
            _projSettings = projSettings;
            InitializeComponent();

            if (projSettings.Vernacular.HasData)
            {
                checkBoxLangVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                                            projSettings.Vernacular.LangName);
                checkBoxLangTransliterateVernacular.Visible = (theSE.viewTransliterationVernacular.Checked
                                                               &&
                                                               !String.IsNullOrEmpty(
                                                                    theSE.LoggedOnMember.TransliteratorVernacular));
                checkBoxLangTransliterateVernacular.Checked = theSE.viewTransliterationVernacular.Checked;
            }
            else
                checkBoxLangVernacular.Visible = false;

            if (projSettings.NationalBT.HasData)
            {
                checkBoxLangNationalBT.Text = String.Format(Properties.Resources.IDS_StoryLanguageField,
                                                            projSettings.NationalBT.LangName);

                checkBoxLangTransliterateNationalBT.Visible = (theSE.viewTransliterationNationalBT.Checked
                                                               &&
                                                               !String.IsNullOrEmpty(
                                                                    theSE.LoggedOnMember.TransliteratorNationalBT));
                checkBoxLangTransliterateNationalBT.Checked = theSE.viewTransliterationNationalBT.Checked;
            }
            else
                checkBoxLangNationalBT.Visible = false;

            if (projSettings.InternationalBT.HasData)
                checkBoxLangInternationalBT.Visible = true;
            else
                checkBoxLangInternationalBT.Visible = false;

            if (projSettings.FreeTranslation.HasData)
                checkBoxLangFreeTranslation.Visible = true;
            else
                checkBoxLangFreeTranslation.Visible = false;

            checkBoxConsultantNotes.Enabled =
                checkBoxCoachNotes.Enabled = (theCurrentStory != null);

            checkBoxUseForAllStories.Checked = bUseForAllStories;

            checkBoxShowHidden.Checked = theSE.hiddenVersesToolStripMenuItem.Checked;

            checkBoxOpenConNotesOnly.Checked = theSE.viewOnlyOpenConversationsMenu.Checked;
        }

        public bool UseForAllStories
        {
            get;
            set;
        }

        protected VerseData.ViewSettings _viewSettings;

        public VerseData.ViewSettings ViewSettings
        {
            set
            {
                _viewSettings = value;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField))
                    checkBoxLangVernacular.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularTransliterationField))
                    checkBoxLangTransliterateVernacular.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField))
                    checkBoxLangNationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTTransliterationField))
                    checkBoxLangTransliterateNationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField))
                    checkBoxLangInternationalBT.Checked = true;
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField))
                    checkBoxLangFreeTranslation.Checked = true;
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
                    checkBoxLangFreeTranslation.Checked,
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

        private void buttonOK_Click(object sender, EventArgs e)
        {
            UseForAllStories = checkBoxUseForAllStories.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
