using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TaskBarControl : UserControl
    {
        private MentoreeRequirementsCheck _checker;
        private StoryEditor TheSe;
        private StoryData TheStory;
        
        public TaskBarControl()
        {
            InitializeComponent();
        }

        public void Initialize(StoryEditor theSe, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            TheSe = theSe;
            TheStory = theStory;

            // these buttons probably want to be shone for all users (i.e. so they
            //  can view the fields), but the labels will be adjusted by the different
            //  roles depending on permission
            ProjectSettings projSettings = theStoryProjectData.ProjSettings;
            buttonVernacular.Visible = projSettings.Vernacular.HasData;
            buttonNationalBt.Visible = projSettings.NationalBT.HasData;
            buttonInternationalBt.Visible = projSettings.InternationalBT.HasData;
            buttonFreeTranslation.Visible = projSettings.FreeTranslation.HasData;

            switch (TheSe.LoggedOnMember.MemberType)
            {
                case TeamMemberData.UserTypes.eProjectFacilitator:
                    SetProjectFaciliatorButtons(theStory, theStoryProjectData);
                    break;
                case TeamMemberData.UserTypes.eIndependentConsultant:
                    SetIndependentConsultantButtons();
                    break;
                case TeamMemberData.UserTypes.eConsultantInTraining:
                    SetConsultantInTrainingButtons();
                    break;
                case TeamMemberData.UserTypes.eCoach:
                    SetCoachButtons();
                    break;
            }
        }

        private void SetCoachButtons()
        {
            buttonMarkReadyForSfg.Visible =
                buttonSendToCIT.Visible =
                    TheSe.LoggedOnMember.IsEditAllowed(TheStory.ProjStage.MemberTypeWithEditToken);
        }

        private void SetConsultantInTrainingButtons()
        {
            buttonViewTasks.Visible = true;

            bool bEditAllowed = TheSe.LoggedOnMember.IsEditAllowed(TheStory.ProjStage.MemberTypeWithEditToken);

            buttonReturnToProjectFacilitator.Visible = (bEditAllowed &&
                                                        TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                                          TasksCit.TaskSettings.
                                                                              SendToProjectFacilitatorForRevision));
            buttonSendToCoach.Visible = (bEditAllowed && TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                                           TasksCit.TaskSettings.SendToCoachForReview));

            if (bEditAllowed)
                _checker = new ConsultantInTrainingRequirementsCheck(TheSe, TheStory);
        }

        private void SetIndependentConsultantButtons()
        {
            buttonReturnToProjectFacilitator.Visible =
                buttonMarkReadyForSfg.Visible =
                TheSe.LoggedOnMember.IsEditAllowed(TheStory.ProjStage.MemberTypeWithEditToken);
        }

        private void SetButtonsAndTooltip(Button btn, bool bEditAllowed, 
            TasksPf.TaskSettings taskToCheck, string strTooltipFormat, string strButtonText)
        {
            if (!bEditAllowed)
                toolTip.SetToolTip(btn, String.Format(Properties.Resources.IDS_PfNoEditToken,
                                                      strTooltipFormat,
                                                      TeamMemberData.GetMemberWithEditTokenAsDisplayString(
                                                          TheSe.StoryProject.TeamMembers,
                                                          TheSe.theCurrentStory.ProjStage.MemberTypeWithEditToken)));

            else if (!TasksPf.IsTaskOn(TheStory.TasksAllowedPf, taskToCheck))
                toolTip.SetToolTip(btn, String.Format(Properties.Resources.IDS_PfNotAllowedToModified,
                                                      strTooltipFormat));

            else
            {
                btn.Visible = true;
                if (!String.IsNullOrEmpty(strButtonText))
                    btn.Text = strButtonText;
            }
        }

        private void SetProjectFaciliatorButtons(StoryData theStory, StoryProjectData theStoryProjectData)
        {
            bool bEditAllowed = TheSe.LoggedOnMember.IsEditAllowed(theStory.ProjStage.MemberTypeWithEditToken);

            buttonAddStory.Visible = true;
            buttonViewTasks.Visible = true;

            SetButtonsAndTooltip(buttonVernacular,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.VernacularLangFields,
                                 "story language",
                                 Properties.Resources.IDS_PfButtonLabelVernacular);

            SetButtonsAndTooltip(buttonNationalBt,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.NationalBtLangFields,
                                 "national language bt",
                                 Properties.Resources.IDS_PfButtonLabelNationalBt);

            SetButtonsAndTooltip(buttonInternationalBt,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.InternationalBtFields,
                                 "English bt",
                                 Properties.Resources.IDS_PfButtonLabelInternationalBt);

            SetButtonsAndTooltip(buttonFreeTranslation,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.FreeTranslationFields,
                                 "free translation",
                                 Properties.Resources.IDS_PfButtonLabelFreeTranslation);

            SetButtonsAndTooltip(buttonAnchors,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.Anchors,
                                 "anchor",
                                 Properties.Resources.IDS_PfButtonLabelAnchors);

            const string cstrRetelling = "retelling";
            SetButtonsAndTooltip(buttonAddRetellingBoxes,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2,
                                 cstrRetelling,
                                 null);

            // then enable it whether there are any more tests to do
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2))
            {
                if (TheStory.CountRetellingsTests > 0)
                    toolTip.SetToolTip(buttonAddRetellingBoxes, String.Format(Properties.Resources.IDS_PfRequiredToDoXTests,
                        cstrRetelling,
                        TheStory.CountRetellingsTests));
                else
                {
                    toolTip.SetToolTip(buttonAddRetellingBoxes, String.Format(Properties.Resources.IDS_PfRequiredTestsDone,
                        cstrRetelling));
                    buttonAddRetellingBoxes.Enabled = false;
                }
            }

            SetButtonsAndTooltip(buttonViewTestQuestions,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.TestQuestions,
                                 "story testing question",
                                 Properties.Resources.IDS_PfButtonLabelTestQuestions);

            SetButtonsAndTooltip(buttonAddBoxesForAnswers,
                                 bEditAllowed,
                                 TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2,
                                 "answer",
                                 null);

            const string cstrStoryQuestionLabel = "story question";

            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2))
            {
                // then enable it whether there are any more tests to do
                if (TheStory.CountTestingQuestionTests > 0)
                    toolTip.SetToolTip(buttonAddBoxesForAnswers, String.Format(Properties.Resources.IDS_PfRequiredToDoXTests,
                        cstrStoryQuestionLabel,
                        TheStory.CountTestingQuestionTests));
                else
                {
                    toolTip.SetToolTip(buttonAddBoxesForAnswers, String.Format(Properties.Resources.IDS_PfRequiredTestsDone,
                        cstrStoryQuestionLabel));
                    buttonAddBoxesForAnswers.Enabled = false;
                }
            }

            // these also are added for short cuts
            buttonSendToConsultant.Visible = bEditAllowed;

            if (bEditAllowed)
                _checker = new ProjectFacilitatorRequirementsCheck(TheSe, theStoryProjectData, theStory);
        }

        private void buttonAddStory_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator);
            ParentForm.Close();
            TheSe.addNewStoryAfterToolStripMenuItem_Click(sender, e);
        }

        private void buttonVernacular_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeVernacular, false);
            }
            else
            {
                TheSe.viewVernacularLangFieldMenuItem.Checked = true;
            }
        }

        private void buttonNationalBt_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeNationalBT, false);
            }
            else
            {
                TheSe.viewNationalLangFieldMenuItem.Checked = true;
            }
        }

        private void buttonInternationalBt_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT, false);
            }
            else
            {
                TheSe.viewEnglishBTFieldMenuItem.Checked = true;
            }
        }

        private void buttonFreeTranslation_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation, false);
            }
            else
            {
                TheSe.viewFreeTranslationToolStripMenuItem.Checked = true;
            }
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
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator);
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
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator);
            ParentForm.Close();
            if (TheSe.AddInferenceTestBoxes())
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
            System.Diagnostics.Debug.Assert(_checker != null);
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator);
            
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
            var dlg = new SetPfTasksForm(TheSe.StoryProject.ProjSettings,
                TheStory.TasksAllowedPf, TheStory.TasksRequiredPf)
                          {
                              Text = String.Format("Set tasks for the Project Facilitator ({0}) to do on story: {1}",
                                                   TheSe.StoryProject.TeamMembers.GetNameFromMemberId(
                                                       TheStory.CraftingInfo.ProjectFacilitatorMemberID),
                                                   TheStory.Name)
                          };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            TheStory.TasksAllowedPf = dlg.TasksAllowed;
            TheStory.TasksRequiredPf = dlg.TasksRequired;

            // set the attributes that say how many tests are required
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings))
                TheStory.CountRetellingsTests = 1;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings2))
                TheStory.CountRetellingsTests = 2;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers))
                TheStory.CountTestingQuestionTests = 1;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers2))
                TheStory.CountTestingQuestionTests = 2;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest, true);
        }

        private void buttonMarkReadyForSfg_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eTeamComplete, true);
        }

        private void buttonSendToCoach_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(_checker != null);
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eConsultantInTraining);
            
            ParentForm.Close();
            if (!_checker.CheckIfRequirementsAreMet())
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eCoachReviewRound2Notes, true);
        }

        private void buttonSendToCIT_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eCoach);

            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCoachAnsweredCITsQuestions(TheSe, TheStory))
                return;

            // find out from the Coach what tasks they want to set in the story
            var dlg = new SetCitTasksForm(TheStory.TasksAllowedCit, 
                TheStory.TasksRequiredCit)
                          {
                              Text = String.Format("Set tasks for the CIT to do on story: {0}",
                                                   TheStory.Name)
                          };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            TheStory.TasksAllowedCit = dlg.TasksAllowed;
            TheStory.TasksRequiredCit = dlg.TasksRequired;
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eConsultantCauseRevisionAfterUnsTest, true);
        }

        private void buttonViewTasks_Click(object sender, EventArgs e)
        {
            SetTasksForm dlg;
            if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
                dlg = new SetPfTasksForm(TheSe.StoryProject.ProjSettings,
                                         TheStory.TasksAllowedPf, TheStory.TasksRequiredPf)
                          {Text = "Tasks for Project Facilitator"};
            else if (TheSe.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eConsultantInTraining)
                dlg = new SetCitTasksForm(TheStory.TasksAllowedCit, TheStory.TasksRequiredCit)
                          {Text = "Tasks for CIT"};
            else
            {
                System.Diagnostics.Debug.Assert(false);
                return;
            }
            dlg.Readonly = true;
            dlg.ShowDialog();
        }
    }
}
