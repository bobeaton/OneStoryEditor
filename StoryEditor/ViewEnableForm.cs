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
        public ViewEnableForm(ProjectSettings projSettings, StoryData theCurrentStory, bool bUseForAllStories)
        {
            InitializeComponent();
            if (projSettings.Vernacular.HasData)
            {
                checkBoxLangVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                                            projSettings.Vernacular.LangName);
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

            checkBoxStoryTestingQuestions.Enabled = ((theCurrentStory != null)
                                                     && (((int) theCurrentStory.ProjStage.ProjectStage)
                                                         > (int) StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions));

            checkBoxAnswers.Enabled = 
                checkBoxConsultantNotes.Enabled =
                checkBoxCoachNotes.Enabled = (theCurrentStory != null);

            checkBoxUseForAllStories.Checked = bUseForAllStories;
        }

        public bool UseForAllStories
        {
            get;
            set;
        }

        public VerseData.ViewItemToInsureOn ItemsToInsureAreOn
        {
            set
            {
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eVernacularLangField))
                    checkBoxLangVernacular.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eNationalLangField))
                    checkBoxLangNationalBT.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eEnglishBTField))
                    checkBoxLangInternationalBT.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eAnchorFields))
                    checkBoxAnchors.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eStoryTestingQuestions))
                    checkBoxStoryTestingQuestions.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eStoryTestingQuestionAnswers))
                    checkBoxAnswers.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eRetellingFields))
                    checkBoxRetellings.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eConsultantNoteFields))
                    checkBoxConsultantNotes.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eCoachNotesFields))
                    checkBoxCoachNotes.Checked = true;
                if (VerseData.IsViewItemOn(value, VerseData.ViewItemToInsureOn.eBibleViewer))
                    checkBoxBibleViewer.Checked = true;
            }

            get
            {
                return VerseData.SetItemsToInsureOn(
                    checkBoxLangVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    checkBoxConsultantNotes.Checked,
                    checkBoxCoachNotes.Checked,
                    checkBoxBibleViewer.Checked,
                    true);
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
