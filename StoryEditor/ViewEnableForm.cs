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
        public ViewEnableForm(StoryEditor theSE, ProjectSettings projSettings, StoryData theCurrentStory, bool bUseForAllStories)
        {
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

                checkBoxLangNationalBT.Enabled = ((theCurrentStory != null)
                                                  && (((int) theCurrentStory.ProjStage.ProjectStage)
                                                      >= (int) StoryStageLogic.ProjectStages.eProjFacTypeNationalBT));
                checkBoxLangTransliterateNationalBT.Visible = (theSE.viewTransliterationNationalBT.Checked
                                                               &&
                                                               !String.IsNullOrEmpty(
                                                                    theSE.LoggedOnMember.TransliteratorNationalBT));
                checkBoxLangTransliterateNationalBT.Checked = theSE.viewTransliterationNationalBT.Checked;
            }
            else
                checkBoxLangNationalBT.Visible = false;

            if (projSettings.InternationalBT.HasData)
            {
                checkBoxLangInternationalBT.Visible = true;
                checkBoxLangInternationalBT.Enabled = ((theCurrentStory != null)
                                                       && (((int) theCurrentStory.ProjStage.ProjectStage)
                                                           >=
                                                           (int)
                                                           StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT));
            }
            else
                checkBoxLangInternationalBT.Visible = false;

            checkBoxAnchors.Enabled = ((theCurrentStory != null)
                                       && (((int) theCurrentStory.ProjStage.ProjectStage)
                                           >= (int) StoryStageLogic.ProjectStages.eProjFacAddAnchors));

            checkBoxStoryTestingQuestions.Enabled =
                checkBoxRetellings.Enabled = 
                checkBoxAnswers.Enabled = ((theCurrentStory != null)
                                           && (((int) theCurrentStory.ProjStage.ProjectStage)
                                               > (int) StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions));

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
                if (_viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields))
                    checkBoxAnchors.Checked = true;
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
                    checkBoxLangVernacular.Checked,
                    checkBoxLangTransliterateVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangTransliterateNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    checkBoxConsultantNotes.Checked,
                    checkBoxCoachNotes.Checked,
                    checkBoxBibleViewer.Checked,
                    true,
                    checkBoxShowHidden.Checked,
                    checkBoxOpenConNotesOnly.Checked);
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
