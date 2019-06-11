#define AllowByPassOfPreliminaryApprovalStage

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class TaskBarControl : UserControl
    {
        private MentoreeRequirementsCheck _checker;
        private StoryEditor TheSe;
        private StoryData TheStory;
        private StoryProjectData TheStoryProjectData;

        public TaskBarControl()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public void Initialize(StoryEditor theSe, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            TheSe = theSe;
            TheStory = theStory;
            TheStoryProjectData = theStoryProjectData;

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

            // this was in the PF tasks, but anyone might be wanting to do this....
            var projSettings = theStoryProjectData.ProjSettings;
            if (projSettings.UseDropbox)
            {
                buttonCopyRecordingToDropbox.Visible = true;
                dropboxStory.Visible = projSettings.DropboxStory;
                dropboxRetelling.Visible = projSettings.DropboxRetelling;
                dropboxTqAnswers.Visible = projSettings.DropboxAnswers;
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

#if AllowByPassOfPreliminaryApprovalStage
            // allow either one... (requested by Irene and I think Nathan at one point)
            buttonMarkPreliminaryApproval.Visible =
                buttonMarkFinalApproval.Visible = true;
#else
            if (TheStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete)
                buttonMarkFinalApproval.Visible = true;
            else
                buttonMarkPreliminaryApproval.Visible = true;
#endif

            buttonSendToCIT.Visible = 
                buttonReturnToProjectFacilitator.Visible = true;    // add sending to PF (in case of workshop)
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
                CheckPfEbtrSetButtonTooltip(buttonSendToEnglishBter, true, bAreUnapprovedNotes);
            }
            else
            {
                CheckPfEbtrSetButtonTooltip(buttonReturnToProjectFacilitator,
                                            TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                              TasksCit.TaskSettings.
                                                                  SendToProjectFacilitatorForRevision),
                                            bAreUnapprovedNotes);
            }

            buttonSendToCoach.Visible = (TasksCit.IsTaskOn(TheStory.TasksAllowedCit,
                                                           TasksCit.TaskSettings.SendToCoachForReview));

            buttonMarkPreliminaryApproval.Visible =
                buttonMarkFinalApproval.Visible = !TasksCit.IsTaskOn(TheStory.TasksRequiredCit, TasksCit.TaskSettings.SendToCoachForReview);

            _checker = new ConsultantInTrainingRequirementsCheck(TheSe, TheStory);
        }

        private void CheckPfEbtrSetButtonTooltip(Button btn, bool bVisible, 
            bool bAreUnapprovedNotes)
        {
            btn.Visible = bVisible;
            if (bAreUnapprovedNotes)
                toolTip.SetToolTip(btn,
                                   Localizer.Str(
                                       "There are unapproved comments! Set to Coach's turn instead to get them approved or they won't be visible to the Project Facilitator"));
        }

        private void SetIndependentConsultantButtons()
        {
            buttonViewTasksPf.Visible = true;
            
            
            if (!TheSe.LoggedOnMember.IsEditAllowed(TheStory))
                return;

#if AllowByPassOfPreliminaryApprovalStage
            // allow either one... (requested by Irene and I think Nathan at one point)
            buttonMarkPreliminaryApproval.Visible =
                buttonMarkFinalApproval.Visible = true;
            buttonSendToCoach.Visible = true;
#else
            if (TheStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete)
                buttonMarkFinalApproval.Visible = true;
            else
                buttonMarkPreliminaryApproval.Visible = true;
#endif

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

            else if (!TasksPf.IsTaskOn(TheStory.TasksAllowedPf, taskToCheck))
                toolTip.SetToolTip(btn, String.Format(Localizer.Str("You can view the {0} fields, but the consultant hasn't given you permission to make changes to them"),
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
                                 Localizer.Str("story language"),
                                 Localizer.Str("Enter Story Language (&Vernacular)"));

            SetButtonsAndTooltip(buttonNationalBt,
                                 bEditAllowed,
                                 projSettings.NationalBT.HasData,
                                 TasksPf.TaskSettings.NationalBtLangFields,
                                 Localizer.Str("national language BT"),
                                 Localizer.Str("Enter &National/Regional BT"));

            SetButtonsAndTooltip(buttonInternationalBt,
                                 bEditAllowed,
                                 projSettings.InternationalBT.HasData,
                                 TasksPf.TaskSettings.InternationalBtFields,
                                 Localizer.Str("English BT"),
                                 Localizer.Str("Enter &English BT"));

            SetButtonsAndTooltip(buttonFreeTranslation,
                                 bEditAllowed,
                                 projSettings.FreeTranslation.HasData,
                                 TasksPf.TaskSettings.FreeTranslationFields,
                                 Localizer.Str("free translation"),
                                 Localizer.Str("Enter &Free Translation (UNS BT)"));

            SetButtonsAndTooltip(buttonAnchors,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Anchors,
                                 Localizer.Str("anchor"),
                                 Localizer.Str("Enter &Anchors"));

            SetButtonsAndTooltip(buttonAddRetellingBoxes,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2,
                                 LocalizeRetelling,
                                 null);

            // then enable it whether there are any more tests to do
            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2))
            {
                if ((TheStory.CountRetellingsTests > 0) ||
                    (TasksPf.IsTaskOn(TheStory.TasksAllowedPf, TasksPf.TaskSettings.Retellings | TasksPf.TaskSettings.Retellings2)))
                    toolTip.SetToolTip(buttonAddRetellingBoxes, TooltipRequiredTasksToDo(LocalizeRetelling,
                                                                                         TheStory.CountRetellingsTests));
                else
                {
                    toolTip.SetToolTip(buttonAddRetellingBoxes, TooltipRequiredTasksDone(LocalizeRetelling));
                    buttonAddRetellingBoxes.Enabled = false;
                }
            }

            SetButtonsAndTooltip(buttonViewTestQuestions,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.TestQuestions,
                                 Localizer.Str("story testing question"),
                                 Localizer.Str("Enter &Testing Questions"));
            
            SetButtonsAndTooltip(buttonAddBoxesForAnswers,
                                 bEditAllowed,
                                 true,
                                 TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2,
                                 Localizer.Str("answer"),
                                 null);

            if (TasksPf.IsTaskOn(TheStory.TasksRequiredPf, TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2))
            {
                // then enable it whether there are any more tests to do
                if ((TheStory.CountTestingQuestionTests > 0) ||
                    (TasksPf.IsTaskOn(TheStory.TasksAllowedPf, TasksPf.TaskSettings.Answers | TasksPf.TaskSettings.Answers2)))
                    toolTip.SetToolTip(buttonAddBoxesForAnswers, TooltipRequiredTasksToDo(LocalizeStoryQuestion,
                                                                                          TheStory.
                                                                                              CountTestingQuestionTests));
                else
                {
                    toolTip.SetToolTip(buttonAddBoxesForAnswers, TooltipRequiredTasksDone(LocalizeStoryQuestion));
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

        private static string TooltipRequiredTasksToDo(string strTestType, int nTestCount)
        {
            return String.Format(Localizer.Str("The consultant is requiring you to show results for {1} {0} test(s)"),
                                 strTestType, nTestCount);
        }

        private static string TooltipRequiredTasksDone(string strTestType)
        {
            return String.Format(Localizer.Str("You've added boxes for all of the required {0} tests"),
                                 strTestType);
        }

        private static string LocalizeRetelling
        {
            get { return Localizer.Str("retelling"); }
        }

        private static string LocalizeStoryQuestion
        {
            get { return Localizer.Str("story question"); }
        }

        private void buttonAddStory_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.AddNewStoryAfter(null, null, null);
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
                TheSe.viewVernacularLangMenu.Checked = true;
            }
        }

        private void buttonNationalBt_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.MoveToNationalBtState();
        }

        private void buttonInternationalBt_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.MoveToInternationalBtState();
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
                TheSe.viewFreeTranslationMenu.Checked = true;
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
                TheSe.viewAnchorsMenu.Checked = 
                    TheSe.viewExegeticalHelps.Checked =     // this kind of goes with anchors
                    TheSe.viewBibleMenu.Checked = true;
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
                TheSe.viewRetellingsMenu.Checked = true;
            }
        }

        private void buttonAddRetellingBoxes_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.AddRetellingAndPaint();
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
                TheSe.viewStoryTestingQuestionsMenu.Checked = true;
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
                TheSe.viewStoryTestingQuestionsMenu.Checked = 
                    TheSe.viewStoryTestingQuestionAnswersMenu.Checked = true;
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

            // if the consultant isn't configured yet (e.g. a new story), but there's 
            //  only one potential consultant, then go ahead and pre-load it... or (s)he
            //  won't get an email from below
            if (!MemberIdInfo.Configured(TheStory.CraftingInfo.Consultant))
            {
                string strMentor = TheSe.StoryProject.TeamMembers.MemberIdOfOneAndOnlyMemberType(
                                                                    TeamMemberData.UserTypes.ConsultantInTraining |
                                                                    TeamMemberData.UserTypes.IndependentConsultant);
                if (!String.IsNullOrEmpty(strMentor))
                {
                    MemberIdInfo.SetCreateIfEmpty(ref TheStory.CraftingInfo.Consultant, strMentor, false);
                }
                else
                {
                    // UPDATE: it's not good to allow the consultant go unconfigured. Then we can't send an email and 
                    //  we can't set default tasks for CITs... So... if it isn't configured, then query for who it is
                    var str = TheStory.CheckForMember(TheStoryProjectData,
                                                      TeamMemberData.UserTypes.IndependentConsultant |
                                                      TeamMemberData.UserTypes.ConsultantInTraining,
                                                      ref TheStory.CraftingInfo.Consultant);
                    if (String.IsNullOrEmpty(str))
                        return;
                }
            }

            // if this is a 'manage with coaching' situation, then reset the 
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
            string strDetails = String.Format(Localizer.Str("‘{0}’ has set the story ‘{1}’ from project '{2}' to your turn"),
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
                strMessageBody += String.Format("{0}{0}Most recent (open) Story line comments:{1}",
                                                Environment.NewLine,
                                                strFinalComments);

            try
            {
                Program.TrySendEmail(member.Email, strSubjectLine, strMessageBody);
                LocalizableMessageBox.Show(
                    String.Format(
                        Localizer.Str(
                            "An automated message has been put into your email's Outbox to inform {0} that it is now his/her turn to work on the story. When you're finished, you should click 'Project', 'Send/Receive' to synchronize your changes to the Internet repository (or thumbdrive) and then do a Send/Receive of your email as well, so the other person gets the message"),
                        member.Name), StoryEditor.OseCaption);
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

            if (!StoryEditor.QueryTerminalTransition)
                return false;

            return true;
        }

        private void buttonReturnToProjectFacilitator_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(
                TheSe.LoggedOnMember.MemberType,
                TeamMemberData.UserTypes.EnglishBackTranslator |
                TeamMemberData.UserTypes.ConsultantInTraining |
                TeamMemberData.UserTypes.IndependentConsultant |
                TeamMemberData.UserTypes.Coach) &&
                                            (ParentForm != null));

            ParentForm.Close();

            if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType,
                                      TeamMemberData.UserTypes.ConsultantInTraining |
                                      TeamMemberData.UserTypes.IndependentConsultant))
            {
                // if it's coming from the CIT/IC
                if (!CheckIfReadyToReturnToPf())
                    return;
            }
            else if (TeamMemberData.IsUser(TheSe.LoggedOnMember.MemberType, TeamMemberData.UserTypes.Coach))
            {
                if (!QueryForProjectFacilitatorTasks())
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
                LocalizableMessageBox.Show(String.Format(Localizer.Str("The coach is requiring you to '{0}'"),
                                              SetCitTasksForm.CstrSendToCoach),
                                StoryEditor.OseCaption);
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
                DialogResult res = LocalizableMessageBox.Show(Localizer.Str("There are one or more questions in the Coach Note pane which haven't been responded to by a Coach. Click 'Yes' to ignore them and continue changing to the project facilitator's turn or click 'No' to cancel so you can go back and respond to them"),
                                                   StoryEditor.OseCaption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res != DialogResult.Yes)
                {
                    if (res == DialogResult.No)
                    {
                        TheSe.viewCoachNotesMenu.Checked = true;
                        LocalizableMessageBox.Show(Properties.Resources.IDS_LoginAsCoach,
                                        StoryEditor.OseCaption);
                    }
                    return false;
                }
            }

            return QueryForProjectFacilitatorTasks();
        }

        private bool QueryForProjectFacilitatorTasks()
        {
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
                DialogResult res = LocalizableMessageBox.Show(Localizer.Str("There are one or more comments in the Consultant Notes pane by the CIT (or LSR) that haven't been approved. Click 'Yes' to ignore them and continue on, or click 'No' to cancel so you can go back and approve/get them approved"),
                                                   StoryEditor.OseCaption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res != DialogResult.Yes)
                {
                    if (res == DialogResult.No)
                        TheSe.viewConsultantNotesMenu.Checked = true;
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

            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                TheStory.CraftingInfo.ProjectFacilitator,
                FinalConNoteComments(TheStory.Verses.FirstVerse.ConsultantNotes));
        }

        private void buttonMarkFinalApproval_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ParentForm != null);
            ParentForm.Close();
            TheSe.SetNextStateAdvancedOverride(StoryStageLogic.ProjectStages.eTeamFinalApproval, true);
            
            SendEmail(TheSe.StoryProject, TheStory, TheSe.LoggedOnMember,
                      TheStory.CraftingInfo.ProjectFacilitator,
                      FinalConNoteComments(TheStory.Verses.FirstVerse.ConsultantNotes));
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

            // if the consultant isn't configured yet (e.g. a new story), but there's 
            //  only one potential consultant, then go ahead and pre-load it... or (s)he
            //  won't get an email from below
            if (!MemberIdInfo.Configured(TheStory.CraftingInfo.Coach))
            {
                var strMentor = TheSe.StoryProject.TeamMembers.MemberIdOfOneAndOnlyMemberType(TeamMemberData.UserTypes.Coach);
                if (!String.IsNullOrEmpty(strMentor))
                {
                    MemberIdInfo.SetCreateIfEmpty(ref TheStory.CraftingInfo.Coach, strMentor, false);
                }
                else
                {
                    // otherwise, force the user to define which Coach is the correct one for this story
                    var str = TheStory.CheckForMember(TheStoryProjectData,
                                                      TeamMemberData.UserTypes.Coach,
                                                      ref TheStory.CraftingInfo.Coach);
                    if (String.IsNullOrEmpty(str))
                        return;
                }
            }

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
                              Text = Localizer.Str("Tasks for Project Facilitator"),
                              Readonly = true
                          };

            dlg.ShowDialog();
        }

        private void buttonViewTasksCit_Click(object sender, EventArgs e)
        {
            var dlg = new SetCitTasksForm(TheStory.TasksAllowedCit,
                                          TheStory.TasksRequiredCit)
                          {
                              Text = Localizer.Str("Tasks for CIT"),
                              Readonly = true
                          };

            dlg.ShowDialog();
        }

        private void ButtonCopyRecordingToDropboxClick(object sender, EventArgs e)
        {
            contextMenuDropbox.Show(MousePosition);
        }

        private void DropboxStoryClick(object sender, EventArgs e)
        {
            TheSe.TriggerDropboxCopyStory(true, null);
        }

        private void DropboxRetellingClick(object sender, EventArgs e)
        {
            TheSe.TriggerDropboxCopyRetelling(true);
        }

        private void DropboxTqAnswersClick(object sender, EventArgs e)
        {
            TheSe.TriggerDropboxCopyAnswer(true);
        }
    }
}
