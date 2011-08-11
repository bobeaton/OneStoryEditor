using System;
using System.Collections.Generic;
using System.Linq;
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

            // if it's an empty project, then at least have the task for adding a story.
            if (TheStory == null)
            {
                if ((TheSe.LoggedOnMember != null)
                    && TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                             TeamMemberData.UserTypes.ProjectFacilitator))
                    buttonAddStory.Visible = true;
                return;
            }

            // getting rid of all the 'view' ones, since those are "tasks"
            //  buttonStoryInformation.Visible = true;

            if (TheSe.LoggedOnMember == null)
                return;

            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                                      TeamMemberData.UserTypes.ProjectFacilitator))
            {
                SetProjectFaciliatorButtons(theStory, theStoryProjectData);
            }
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                           TeamMemberData.UserTypes.EnglishBackTranslator))
            {
                SetEnglishBackTranslatorButtons();
            }
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                                           TeamMemberData.UserTypes.IndependentConsultant))
            {
                SetIndependentConsultantButtons();
            }
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                                           TeamMemberData.UserTypes.ConsultantInTraining))
            {
                SetConsultantInTrainingButtons();
            }
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                                           TeamMemberData.UserTypes.Coach))
            {
                SetCoachButtons();
            }
        }

        private void SetEnglishBackTranslatorButtons()
        {
            buttonViewTasksPf.Visible = true;

            bool bEditAllowed = TheSe.LoggedOnMember.IsEditAllowed(TheStory);

            buttonReturnToProjectFacilitator.Visible =
                buttonSendToConsultant.Visible = bEditAllowed;

            _checker = new EnglishBterRequirementsCheck(TheSe, TheStory);
        }

        private void SetCoachButtons()
        {
            buttonViewTasksPf.Visible = true;

            // make the view CIT tasks visible only if we're in a manage with coaching
            //  situation.
            buttonViewTasksCit.Visible = !TheSe.StoryProject.TeamMembers.HasIndependentConsultant;

            if (!TheSe.LoggedOnMember.IsEditAllowed(TheStory))
                return;

            if (TheStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete)
                buttonMarkFinalApproval.Visible = true;
            else
                buttonMarkPreliminaryApproval.Visible = true;

            buttonSendToCIT.Visible = true;
        }

        private void SetConsultantInTrainingButtons()
        {
            buttonViewTasksPf.Visible = true;
            buttonViewTasksCit.Visible = true;

            if (!TheSe.LoggedOnMember.IsEditAllowed(TheStory))
                return;

            bool bAreUnapprovedNotes = TheStory.AreUnapprovedConsultantNotes;
            if (TheSe.StoryProject.TeamMembers.HasOutsideEnglishBTer)
            {
                buttonSendToEnglishBter.Visible = true;
                if (bAreUnapprovedNotes)
                    toolTip.SetToolTip(buttonSendToEnglishBter, 
                        Properties.Resources.IDS_ToolTipAboutUnapprovedComments);
            }
            else
            {
                buttonReturnToProjectFacilitator.Visible = (TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                                              TasksCit.TaskSettings.
                                                                                  SendToProjectFacilitatorForRevision));
                if (bAreUnapprovedNotes)
                    toolTip.SetToolTip(buttonReturnToProjectFacilitator,
                        Properties.Resources.IDS_ToolTipAboutUnapprovedComments);
            }

            buttonSendToCoach.Visible = (TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                           TasksCit.TaskSettings.SendToCoachForReview));

            _checker = new ConsultantInTrainingRequirementsCheck(TheSe, TheStory);
        }

        private void SetIndependentConsultantButtons()
        {
            buttonViewTasksPf.Visible = true;
            
            if (!TheSe.LoggedOnMember.IsEditAllowed(TheStory))
                return;

            if (TheStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete)
                buttonMarkFinalApproval.Visible = true;
            else
                buttonMarkPreliminaryApproval.Visible = true;

            if (TheSe.StoryProject.TeamMembers.HasOutsideEnglishBTer)
                buttonSendToEnglishBter.Visible = true;
            else
                buttonReturnToProjectFacilitator.Visible = true;
        }

        private void SetButtonsAndTooltip(Button btn, bool bEditAllowed, bool bDefaultVisibility,
            TasksPf.TaskSettings taskToCheck, string strTooltipFormat, string strButtonText)
        {
            if (!bEditAllowed)
                btn.Visible = false;
                /* people didn't want to see 'View' anything as a 'task'
                toolTip.SetToolTip(btn, String.Format(Properties.Resources.IDS_PfNoEditToken,
                                                      strTooltipFormat,
                                                      TeamMemberData.GetMemberWithEditTokenAsDisplayString(
                                                          TheSe.StoryProject.TeamMembers,
                                                          TheSe.theCurrentStory.ProjStage.MemberTypeWithEditToken)));
                */

            else if (!TasksPf.IsTaskOn(TheStory.TasksAllowedPf, taskToCheck))
                toolTip.SetToolTip(btn, String.Format(Properties.Resources.IDS_PfNotAllowedToModified,
                                                      strTooltipFormat));

            else
            {
                btn.Visible = bDefaultVisibility;
                if (!String.IsNullOrEmpty(strButtonText))
                    btn.Text = strButtonText;
            }
        }

        private void SetProjectFaciliatorButtons(StoryData theStory,
            StoryProjectData theStoryProjectData)
        {
            bool bEditAllowed = TheSe.LoggedOnMember.IsEditAllowed(theStory);

            buttonAddStory.Visible = true;
            buttonViewTasksPf.Visible = true;

            ProjectSettings projSettings = theStoryProjectData.ProjSettings;
            buttonVernacular.Visible = bEditAllowed && projSettings.Vernacular.HasData;
            buttonNationalBt.Visible = bEditAllowed && projSettings.NationalBT.HasData;
            buttonInternationalBt.Visible = bEditAllowed && projSettings.InternationalBT.HasData;
            buttonFreeTranslation.Visible = bEditAllowed && projSettings.FreeTranslation.HasData;

            if (TheStory.CraftingInfo.IsBiblicalStory)
            {
                buttonAnchors.Visible = bEditAllowed;
                // buttonViewRetellings.Visible = bEditAllowed;
                buttonViewTestQuestions.Visible = bEditAllowed;
                // buttonViewTestQuestionAnswers.Visible = bEditAllowed;
            }

            SetButtonsAndTooltip(buttonVernacular,
                                 bEditAllowed,
                                 projSettings.Vernacular.HasData,
                                 TasksPf.TaskSettings.VernacularLangFields,
                                 "story language",
                                 Properties.Resources.IDS_PfButtonLabelVernacular);

            SetButtonsAndTooltip(buttonNationalBt,
                                 bEditAllowed,
                                 projSettings.NationalBT.HasData,
                                 TasksPf.TaskSettings.NationalBtLangFields,
                                 "national language bt",
                                 Properties.Resources.IDS_PfButtonLabelNationalBt);

            SetButtonsAndTooltip(buttonInternationalBt,
                                 bEditAllowed,
                                 projSettings.InternationalBT.HasData,
                                 TasksPf.TaskSettings.InternationalBtFields,
                                 "English bt",
                                 Properties.Resources.IDS_PfButtonLabelInternationalBt);

            SetButtonsAndTooltip(buttonFreeTranslation,
                                 bEditAllowed,
                                 projSettings.FreeTranslation.HasData,
                                 TasksPf.TaskSettings.FreeTranslationFields,
                                 "free translation",
                                 Properties.Resources.IDS_PfButtonLabelFreeTranslation);

            SetButtonsAndTooltip(buttonAnchors,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Anchors,
                                 "anchor",
                                 Properties.Resources.IDS_PfButtonLabelAnchors);

            const string cstrRetelling = "retelling";
            SetButtonsAndTooltip(buttonAddRetellingBoxes,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2,
                                 cstrRetelling,
                                 null);

            // then enable it whether there are any more tests to do
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2))
            {
                if ((TheStory.CountRetellingsTests > 0) || 
                    (TasksPf.IsTaskOn(TheStory.TasksAllowedPf, TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2)))
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
                                 true,
                                 TasksPf.TaskSettings.TestQuestions,
                                 "story testing question",
                                 Properties.Resources.IDS_PfButtonLabelTestQuestions);
            
            SetButtonsAndTooltip(buttonAddBoxesForAnswers,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2,
                                 "answer",
                                 null);

            const string cstrStoryQuestionLabel = "story question";

            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2))
            {
                // then enable it whether there are any more tests to do
                if ((TheStory.CountTestingQuestionTests > 0) ||
                    (TasksPf.IsTaskOn(TheStory.TasksAllowedPf, TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2)))
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

            if (!bEditAllowed)
                return;

            if (theStoryProjectData.TeamMembers.HasOutsideEnglishBTer)
                buttonSendToEnglishBter.Visible = true;
            else
                buttonSendToConsultant.Visible = true;

            _checker = new ProjectFacilitatorRequirementsCheck(TheSe, theStory);
        }

        private void buttonAddStory_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.addNewStoryAfterToolStripMenuItem_Click(sender, e);
        }

        private void buttonVernacular_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.ProjectFacilitator))
            {
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacAddAnchors, false);
            }
            else
            {
                TheSe.viewAnchorFieldMenuItem.Checked = 
                    TheSe.viewExegeticalHelps.Checked =     // this kind of goes with anchors
                    TheSe.viewNetBibleMenuItem.Checked = true;
            }
        }

        private void buttonRetellings_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.editAddTestResultsToolStripMenuItem_Click(sender, e);
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterRetellingOfTest1, false);
        }

        private void buttonTestQuestions_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, 
                TeamMemberData.UserTypes.ProjectFacilitator))
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
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (TheSe.AddInferenceTestBoxes())
                TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1, false);
        }

        private void buttonStoryInformation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.QueryStoryPurpose();
        }

        private void buttonViewPanorama_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.toolStripMenuItemShowPanorama_Click(sender, e);
        }

        private void buttonSendToEnglishBter_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();

            // we go to the English Bter either going forwards from the PF...
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ProjectFacilitator))
            {
                if (!CheckForPfDone())
                    return;

                TheSe.SetNextStateAdvancedOverride(
                    StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBTTest1, true);
            }
            // ... or coming backwards from the CIT/IC
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                           TeamMemberData.UserTypes.IndependentConsultant | 
                                           TeamMemberData.UserTypes.ConsultantInTraining))
            {
                if (!CheckIfReadyToReturnToPf())
                    return;

                TheSe.SetNextStateAdvancedOverride(
                    TheStory.CraftingInfo.IsBiblicalStory
                        ? StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesAfterUnsTest
                        : StoryStageLogic.ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest, true);
            }

            // TODO: Add OutsideEnglishBackTranslator to the StoryFrontMatterForm
            //  and initialize it when an OutsideEnglishBackTranslator does something
            //  to the story.
            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.OutsideEnglishBackTranslator,
                FinalConNoteComments(TheStory.Verses.FirstVerse.ConsultantNotes));
        }

        private void buttonSendToConsultant_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(
                TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.EnglishBackTranslator |
                TeamMemberData.UserTypes.ProjectFacilitator)
                                            && (ParentForm != null)
                                            && (_checker != null));
            
            // we go to the consultant forward either from the English BTer...
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.EnglishBackTranslator))
            {
                ParentForm.Close();
            
                if (!_checker.CheckIfRequirementsAreMet(true))
                    return;

            }

            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                           TeamMemberData.UserTypes.ProjectFacilitator))
            {
                if (!CheckForPfDone())
                    return;
            }

            // if this is a "manage with coaching" situation, then reset the 
            //  'Set to Coach's turn' requirement. [That requirement will have been so
            //  removed that the CIT could send it to the PF in the first place, and the 
            //  nature of a CIT is that he always must send to the coach anyway]
            if (!TheSe.StoryProject.TeamMembers.HasIndependentConsultant &&
                MemberIdInfo.Configured(TheStory.CraftingInfo.Consultant))
            {
                // but it must be based on the assigned CITs default requirement
                var theCit = TheSe.StoryProject.TeamMembers.GetMemberFromId(TheStory.CraftingInfo.Consultant.MemberId);
                if (TasksCit.IsTaskOn((TasksCit.TaskSettings)theCit.DefaultRequired, 
                                       TasksCit.TaskSettings.SendToCoachForReview))
                    TheStory.TasksRequiredCit |= 
                        TasksCit.TaskSettings.SendToCoachForReview;
            }

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eConsultantCheck2, true);

            // Send the consultant for this story an email
            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.Consultant,
                FinalConNoteComments(TheStory.Verses.FirstVerse.ConsultantNotes));
        }

        private static void SendEmail(StoryProjectData theProject, StoryData theStory, 
            TeamMemberData loggedOnMember, MemberIdInfo recipient, string strFinalComments)
        {
            if (!MemberIdInfo.Configured(recipient))
                return;

            var member = theProject.GetMemberFromId(recipient.MemberId);
            if ((member == null) || String.IsNullOrEmpty(member.Email))
                return;

            // e.g. ‘Bob Eaton’ has set the story ‘01 creation’ to your turn
            string strDetails = String.Format(Properties.Resources.IDS_EmailDetails,
                                              loggedOnMember.Name, 
                                              theStory.Name,
                                              theProject.ProjSettings.ProjectName);

            // e.g. [OSE says]: <strDetails>
            string strSubjectLine = String.Format(Properties.Resources.IDS_EmailSubjectLine,
                                                  strDetails);

            /*
             * This is an automated message from OSE v 2.2.0.7:
             * 
             * <strDetails>
             */

            string strMessageBody = String.Format(Properties.Resources.IDS_EmailMessageBody,
                                                  StoryEditor.GetOseVersion(),
                                                  strDetails);

            if (!String.IsNullOrEmpty(strFinalComments))
                strMessageBody += String.Format(Properties.Resources.IDS_EmailLastComments,
                                                Environment.NewLine,
                                                strFinalComments);

            try
            {
                Program.SendEmail(member.Email, strSubjectLine, strMessageBody);
                MessageBox.Show(String.Format(Properties.Resources.IDS_InformToDoSendReceive,
                                              member.Name));
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private bool CheckForPfDone()
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            System.Diagnostics.Debug.Assert(_checker != null);
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));

            ParentForm.Close();
            if (!_checker.CheckIfRequirementsAreMet(true))
                return false;

            if (MessageBox.Show(Properties.Resources.IDS_TerminalTransitionMessage,
                                OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return false;

            return true;
        }

        private void buttonReturnToProjectFacilitator_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(
                TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.EnglishBackTranslator |
                TeamMemberData.UserTypes.ConsultantInTraining |
                TeamMemberData.UserTypes.IndependentConsultant)
                                            && (ParentForm != null));

            ParentForm.Close();

            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ConsultantInTraining |
                                      TeamMemberData.UserTypes.IndependentConsultant))
            {
                // if it's coming from the CIT/IC
                if (!CheckIfReadyToReturnToPf())
                    return;
            }

            // this applies regardless of who it's coming from
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eProjFacRevisesAfterUnsTest, true);

            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.ProjectFacilitator,
                FinalConNoteComments(TheStory.Verses.FirstVerse.ConsultantNotes));
        }

        private bool CheckIfReadyToReturnToPf()
        {
            // first have to satisfy the requirement to send to coach
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ConsultantInTraining) &&
                TasksCit.IsTaskOn(TheStory.TasksRequiredCit,
                                  TasksCit.TaskSettings.SendToCoachForReview))
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_MustPassToCoach,
                                              SetCitTasksForm.CstrSendToCoach),
                                OseResources.Properties.Resources.IDS_Caption);
                return false;
            }

            if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                return false;

            // this applies to both CITs and ICs
            if (!CheckForUnapprovedComments())
                return false;

            // if we don't have an explicit coach state, then the IC is the one who
            //  probably needs to deal with Coach Notes, so warn him/her if there
            //  are unresponded to comments.
            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.IndependentConsultant) &&
                TheSe.StoryProject.TeamMembers.HasIndependentConsultant &&
                TheStory.AreUnrespondedToCoachNoteComments)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_WarnAboutUnrespondedToComments,
                                                   OseResources.Properties.Resources.IDS_Caption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res != DialogResult.Yes)
                {
                    if (res == DialogResult.No)
                    {
                        TheSe.viewCoachNotesFieldMenuItem.Checked = true;
                        MessageBox.Show(Properties.Resources.IDS_LoginAsCoach,
                                        OseResources.Properties.Resources.IDS_Caption);
                    }
                    return false;
                }
            }

            // find out from the consultant what tasks they want to set in the story
            if (!SetPfTasksForm.EditPfTasks(TheSe, ref TheStory))
                return false;

            // set the attributes that say how many tests are required
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings))
                TheStory.CountRetellingsTests = 1;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings2))
                TheStory.CountRetellingsTests = 2;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers))
                TheStory.CountTestingQuestionTests = 1;
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers2))
                TheStory.CountTestingQuestionTests = 2;

            return true;
        }

        private bool CheckForUnapprovedComments()
        {
            if (TheStory.AreUnapprovedConsultantNotes)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_WarnAboutUnapprovedComments,
                                                   OseResources.Properties.Resources.IDS_Caption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res != DialogResult.Yes)
                {
                    if (res == DialogResult.No)
                        TheSe.viewConsultantNoteFieldMenuItem.Checked = true;
                    return false;
                }
            }
            return true;
        }

        private void buttonMarkPreliminaryApproval_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCITAnsweredPFsQuestions(TheSe, TheStory))
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eTeamComplete, true);
        }

        private void buttonMarkFinalApproval_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eTeamFinalApproval, true);
        }

        private void buttonSendToCoach_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            System.Diagnostics.Debug.Assert(_checker != null);
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ConsultantInTraining));
            
            ParentForm.Close();
            if (!_checker.CheckIfRequirementsAreMet(true))
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eCoachReviewRound2Notes, true);

            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.Coach,
                FinalConNoteComments(TheStory.Verses.FirstVerse.CoachNotes));
        }

        private string FinalConNoteComments(IEnumerable<ConsultNoteDataConverter> theStoryLine)
        {
            /*
            string strFinalComments = null;
            foreach (var theCndc in theStoryLine)
            {
                if (!theCndc.IsFinished && !theCndc.IsNoteToSelf && !theCndc.NoteNeedsApproval)
                {
                    var theLastComment = theCndc.FinalComment;
                    if (TheSe.LoggedOnMember == theLastComment.Commentor(TheSe.StoryProject.TeamMembers))
                        strFinalComments += String.Format("{0}{0}{1}",
                                                          Environment.NewLine,
                                                          theLastComment);
                }
            }
            return strFinalComments;
            */
            return theStoryLine.Where(theCndc =>
                                        !theCndc.IsFinished &&
                                        !theCndc.IsNoteToSelf &&
                                        !theCndc.NoteNeedsApproval)
                    .Select(theCndc => theCndc.FinalComment)
                    .Where(theLastComment =>
                        TheSe.LoggedOnMember == theLastComment.Commentor(TheSe.StoryProject.TeamMembers))
                        .Aggregate<CommInstance, string>(null, (current, theLastComment) =>
                            current + String.Format("{0}{0}{1}", Environment.NewLine, theLastComment));
        }

        private void buttonSendToCIT_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.Coach));

            ParentForm.Close();
            if (!CheckEndOfStateTransition.CheckThatCoachAnsweredCITsQuestions(TheSe, TheStory))
                return;

            // this applies to both CITs and ICs
            if (!CheckForUnapprovedComments())
                return;

            // find out from the Coach what tasks they want to set in the story
            if (!SetCitTasksForm.EditCitTasks(ref TheStory))
                return;

            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eConsultantCauseRevisionAfterUnsTest, true);

            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.Consultant,
                FinalConNoteComments(TheStory.Verses.FirstVerse.CoachNotes));
        }

        private void buttonViewTasksPf_Click(object sender, EventArgs e)
        {
            var dlg = new SetPfTasksForm(TheSe.StoryProject.ProjSettings,
                                         TheStory.TasksAllowedPf,
                                         TheStory.TasksRequiredPf,
                                         TheStory.CraftingInfo.IsBiblicalStory)
                          {
                              Text = "Tasks for Project Facilitator",
                              Readonly = true
                          };

            dlg.ShowDialog();
        }

        private void buttonViewTasksCit_Click(object sender, EventArgs e)
        {
            var dlg = new SetCitTasksForm(TheStory.TasksAllowedCit,
                                          TheStory.TasksRequiredCit)
                          {
                              Text = "Tasks for CIT",
                              Readonly = true
                          };

            dlg.ShowDialog();
        }
    }
}
