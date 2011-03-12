using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TaskBarControl : UserControl
    {
        private SendToCoachRequirementsCheck _checker;
        private StoryEditor TheSe;
        private StoryData TheStory;
        private StoryProjectData TheStoryProjectData;
        
        public TaskBarControl()
        {
            InitializeComponent();
        }

        public void Initialize(StoryEditor theSe, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            TheSe = theSe;
            TheStory = theStory;
            TheStoryProjectData = theStoryProjectData;
            switch (TheSe.LoggedOnMember.MemberType)
            {
                case TeamMemberData.UserTypes.eProjectFacilitator:
                    SetProjectFaciliatorButtons(theStory, theStoryProjectData);
                    break;
                case TeamMemberData.UserTypes.eIndependentConsultant:
                    SetIndependentConsultantButtons();
                    break;
                case TeamMemberData.UserTypes.eConsultantInTraining:
                    SetIndependentConsultantButtons();
                    break;
            }
        }

        private void SetIndependentConsultantButtons()
        {
            buttonReturnToProjectFacilitator.Visible = 
                buttonMarkReadyForSfg.Visible = true;
        }

        private void SetProjectFaciliatorButtons(StoryData theStory, StoryProjectData theStoryProjectData)
        {
            buttonAddStory.Visible = true;
            buttonVernacular.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf,
                                                        TasksPf.TaskSettings.VernacularLangFields);
            buttonNationalBt.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf,
                                                        TasksPf.TaskSettings.NationalBtLangFields);
            buttonInternationalBt.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf,
                                                             TasksPf.TaskSettings.InternationalBtFields);
            buttonFreeTranslation.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf,
                                                             TasksPf.TaskSettings.FreeTranslationFields);
            // buttonAnchors.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.Anchors);
            buttonAddRetellingBoxes.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.Retellings);
            // buttonViewTestQuestions.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.TestQuestions);
            buttonAddBoxesForAnswers.Visible = TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.Answers);

            // these also are added for short cuts
            buttonStoryInformation.Visible = true;
            buttonViewPanorama.Visible = true;
            buttonSendToConsultant.Visible = true;

            _checker = new SendToCoachRequirementsCheck(TheSe, theStoryProjectData, theStory);
        }

        private void buttonAddStory_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.addNewStoryAfterToolStripMenuItem_Click(sender, e);
        }

        private void buttonVernacular_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeVernacular, false);
        }

        private void buttonNationalBt_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeNationalBT, false);
        }

        private void buttonInternationalBt_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT, false);
        }

        private void buttonFreeTranslation_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation, false);
        }

        private void buttonAnchors_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacAddAnchors, false);
            }
            else
            {
                TheSe.viewAnchorFieldMenuItem.Checked = 
                    TheSe.viewNetBibleMenuItem.Checked = true;
            }
        }

        private void buttonRetellings_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1, false);
            }
            else
            {
                TheSe.viewRetellingFieldMenuItem.Checked = true;
            }
        }

        private void buttonAddRetellingBoxes_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.editAddTestResultsToolStripMenuItem_Click(sender, e);
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1, false);
        }

        private void buttonTestQuestions_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacAddStoryQuestions, false);
            }
            else
            {
                TheSe.viewStoryTestingQuestionMenuItem.Checked = true;
            }
        }

        private void buttonTestQuestionAnswers_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1, false);
            }
            else
            {
                TheSe.viewStoryTestingQuestionMenuItem.Checked = 
                    TheSe.viewStoryTestingQuestionAnswerMenuItem.Checked = true;
            }
        }
        
        private void buttonAddBoxesForAnswers_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.editAddInferenceTestResultsToolStripMenuItem_Click(sender, e);
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1, false);
        }

        private void buttonStoryInformation_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.QueryStoryPurpose();
        }

        private void buttonViewPanorama_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            TheSe.toolStripMenuItemShowPanorama_Click(sender, e);
        }

        private void buttonSendToConsultant_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (!_checker.CheckIfRequirementsAreMet())
                return;

            if (MessageBox.Show(Properties.Resources.IDS_TerminalTransitionMessage,
                                OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eConsultantCheck2, true);
        }

        private void buttonReturnToProjectFacilitator_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                return;

            // find out from the consultant what tasks they want to set in the story
            var dlg = new SetTasksForm(TheStoryProjectData.ProjSettings,
                TheStory.TasksAllowedPf, TheStory.TasksRequiredPf);

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            TheStory.TasksAllowedPf = dlg.TasksAllowed;
            TheStory.TasksRequiredPf = dlg.TasksRequired;
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest, true);
        }

        private void buttonMarkReadyForSfg_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eTeamComplete, true);
        }
    }
}
