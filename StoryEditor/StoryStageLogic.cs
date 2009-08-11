using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class StoryStageLogic
    {
        protected StoryEditor _theSE = null; // so we can access the logged on user
        protected ProjectStages _ProjectStage = ProjectStages.eUndefined;
        protected const string CstrDefaultProjectStage = "CrafterTypeNationalBT";

        public enum ProjectStages
        {
            eUndefined = 0,
            eCrafterTypeNationalBT,
            eCrafterTypeInternationalBT,
            eCrafterAddAnchors,
            eCrafterAddStoryQuestions,
            eConsultantCheckAnchors,
            eConsultantCheckStoryQuestions,
            eCoachReviewRound1Notes,
            eConsultantReviseRound1Notes,
            eCrafterReviseBasedOnRound1Notes,
            eCrafterOnlineReview1WithConsultant,
            eCrafterReadyForTest1,
            eCrafterEnterAnswersToStoryQuestionsOfTest1,
            eCrafterEnterRetellingOfTest1,
            eConsultantCheckAnchorsRound2,
            eConsultantCheckAnswersToTestingQuestionsRound2,
            eConsultantCheckRetellingRound2,
            eCoachReviewRound2Notes,
            eConsultantReviseRound2Notes,
            eCrafterReviseBasedOnRound2Notes,
            eCrafterOnlineReview2WithConsultant,
            eCrafterReadyForTest2,
            eCrafterEnterAnswersToStoryQuestionsOfTest2,
            eCrafterEnterRetellingOfTest2,
            eConsultantReviewTest2,
            eCoachReviewTest2Notes,
            eTeamComplete
        }

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set { _ProjectStage = value; }
        }

        public StoryStageLogic(StoryEditor theSE)
        {
            ProjectStage = ProjectStages.eCrafterTypeNationalBT;
            _theSE = theSE;
        }

        public StoryStageLogic(string strProjectStage, StoryEditor theSE)
        {
            ProjectStage = GetProjectStageFromString(strProjectStage);
            _theSE = theSE;
        }

        protected StoryStageLogic.ProjectStages GetProjectStageFromString(string strProjectStageString)
        {
            System.Diagnostics.Debug.Assert(CmapStageStringToEnumType.ContainsKey(strProjectStageString));
            return CmapStageStringToEnumType[strProjectStageString];
        }

        protected TeamMemberData LoggedOnMember
        {
            get { return _theSE.LoggedOnMember; }
        }

        public bool IsEditAllowed
        {
            get
            {
                if (MemberTypeWithEditToken == LoggedOnMember.MemberType)
                    return true;

                throw new ApplicationException(String.Format("Right now, only a '{0}' should be editing the story", TeamMemberData.GetMemberTypeAsString(MemberTypeWithEditToken)));
            }
        }

        // this isn't 100% effective. Sometimes a particular stage can have a single (but varied) editors
        //  (e.g. the Online consult could either be the crafter or the consultant)
        public StoryEditor.UserTypes MemberTypeWithEditToken
        {
            get
            {
                StageTransition st = CmapAllowableStageTransitions[ProjectStage];
                return st.MemberTypeWithEditToken;
            }
        }

        public static bool IsValidTransition(ProjectStages eCurrentStage, ProjectStages eToStage, out string strTerminalStageMessage)
        {
            StageTransition st = CmapAllowableStageTransitions[eCurrentStage];
            return st.IsValidTransition(eToStage, out strTerminalStageMessage);
        }

        public void SetViewBasedOnProjectStage(StoryEditor theSE, ProjectStages eStage, out string strStatusMessage, out string strTooltipMessage)
        {
            StageTransition st = CmapAllowableStageTransitions[eStage];
            st.SetView(theSE, out strStatusMessage, out strTooltipMessage);
        }

        public override string ToString()
        {
            return _ProjectStage.ToString().Substring(1);
        }

        protected static Dictionary<string, StoryStageLogic.ProjectStages> CmapStageStringToEnumType = new Dictionary<string, StoryStageLogic.ProjectStages>() {
            { "CrafterTypeNationalBT", ProjectStages.eCrafterTypeNationalBT },
            { "CrafterTypeInternationalBT", ProjectStages.eCrafterTypeInternationalBT },
            { "CrafterAddAnchors", ProjectStages.eCrafterAddAnchors },
            { "CrafterAddStoryQuestions", ProjectStages.eCrafterAddStoryQuestions },
			{ "ConsultantCheckAnchors", ProjectStages.eConsultantCheckAnchors },
            { "ConsultantCheckStoryQuestions", ProjectStages.eConsultantCheckStoryQuestions },
            { "CoachReviewRound1Notes", ProjectStages.eCoachReviewRound1Notes },
            { "ConsultantReviseRound1Notes", ProjectStages.eConsultantReviseRound1Notes },
            { "CrafterReviseBasedOnRound1Notes", ProjectStages.eCrafterReviseBasedOnRound1Notes },
            { "CrafterOnlineReview1WithConsultant", ProjectStages.eCrafterOnlineReview1WithConsultant },
            { "CrafterReadyForTest1", ProjectStages.eCrafterReadyForTest1 },
            { "CrafterEnterAnswersToStoryQuestionsOfTest1", ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1 },
            { "CrafterEnterRetellingOfTest1", ProjectStages.eCrafterEnterRetellingOfTest1 },
            { "ConsultantCheckAnchorsRound2", ProjectStages.eConsultantCheckAnchorsRound2 },
            { "ConsultantCheckAnswersToTestingQuestionsRound2", ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2 },
            { "ConsultantCheckRetellingRound2", ProjectStages.eConsultantCheckRetellingRound2 },
            { "CoachReviewRound2Notes", ProjectStages.eCoachReviewRound2Notes },
            { "ConsultantReviseRound2Notes", ProjectStages.eConsultantReviseRound2Notes },
            { "CrafterReviseBasedOnRound2Notes", ProjectStages.eCrafterReviseBasedOnRound2Notes },
            { "CrafterOnlineReview2WithConsultant", ProjectStages.eCrafterOnlineReview2WithConsultant },
            { "CrafterReadyForTest2", ProjectStages.eCrafterReadyForTest2 },
            { "CrafterEnterAnswersToStoryQuestionsOfTest2", ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2 },
            { "CrafterEnterRetellingOfTest2", ProjectStages.eCrafterEnterRetellingOfTest2 },
            { "ConsultantReviewTest2", ProjectStages.eConsultantReviewTest2 },
            { "CoachReviewTest2Notes", ProjectStages.eCoachReviewTest2Notes },
            { "TeamComplete", ProjectStages.eTeamComplete }};

        protected static Dictionary<ProjectStages, StageTransition> CmapAllowableStageTransitions = new Dictionary<ProjectStages, StageTransition>()
        {
            { 
                // read this as: from the "CrafterTypeNationalBT" state, you (the Crafter) can go to the
                //  CrafterTypeInternationalBT, CrafterAddAnchors, CrafterAddStoryQuestions, or ConsultantAddRound1Notes
                //  state (the latter of which is the terminal stage (beyond which the edit token goes to the next)
                ProjectStages.eCrafterTypeNationalBT, 
                new StageTransition(StoryEditor.UserTypes.eCrafter, 
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    Properties.Settings.Default.HelpCrafterTypeNationalBT,
                    Properties.Settings.Default.HelpTooltipCrafterTypeNationalBT,
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeInternationalBT, 
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterTypeInternationalBT, 
                new StageTransition(StoryEditor.UserTypes.eCrafter, 
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterAddAnchors, 
                new StageTransition(StoryEditor.UserTypes.eCrafter, 
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterAddStoryQuestions, 
                new StageTransition(StoryEditor.UserTypes.eCrafter, 
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnchors, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining, 
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckStoryQuestions,
                        ProjectStages.eCoachReviewRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckStoryQuestions, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining, 
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors,
                        ProjectStages.eCoachReviewRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewRound1Notes, 
                new StageTransition(StoryEditor.UserTypes.eCoach,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant returns it to you for the next round of review.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviseRound1Notes, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the crafter has returned it to you after the first (formal) UNS test.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReviseBasedOnRound1Notes, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterOnlineReview1WithConsultant, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReadyForTest1, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterRetellingOfTest1, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnchorsRound2, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2,
                        ProjectStages.eConsultantCheckRetellingRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnchorsRound2,
                        ProjectStages.eConsultantCheckRetellingRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckRetellingRound2, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnchorsRound2,
                        ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewRound2Notes, 
                new StageTransition(StoryEditor.UserTypes.eCoach,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it for round 2.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviseRound2Notes, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the crafter has returned it for a final review.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReviseBasedOnRound2Notes, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterOnlineReview2WithConsultant, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReadyForTest2, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterRetellingOfTest2, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviewTest2, 
                new StageTransition(StoryEditor.UserTypes.eConsultantInTraining,
                    "If you change to this stage, then you won't be able to edit the story until after the coach has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCoachReviewTest2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewTest2Notes, 
                new StageTransition(StoryEditor.UserTypes.eCoach,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound2Notes,
                        ProjectStages.eTeamComplete
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eTeamComplete, 
                new StageTransition(StoryEditor.UserTypes.eCrafter,
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        true,   // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            }
        };

        internal static Dictionary<ProjectStages, string> CmapStageToDisplayString = new Dictionary<ProjectStages, string>()
        {
            { ProjectStages.eCrafterTypeNationalBT, "Crafter enters the National language back-translation" },
            { ProjectStages.eCrafterTypeInternationalBT, "Crafter enters the English back-translation" },
            { ProjectStages.eCrafterAddAnchors, "Crafter adds biblical anchors" },
            { ProjectStages.eCrafterAddStoryQuestions, "Crafter adds story testing questions" },
            { ProjectStages.eConsultantCheckAnchors, "Consultant checks the story anchors" },
            { ProjectStages.eConsultantCheckStoryQuestions, "Consultant checks the story testing questions" },
            { ProjectStages.eCoachReviewRound1Notes, "Coach reviews round 1 consultant notes" },
            { ProjectStages.eConsultantReviseRound1Notes, "Consultant revises round 1 notes based on Coach's feedback" },
            { ProjectStages.eCrafterReviseBasedOnRound1Notes, "Crafter revises story based on Consultant's feedback" },
            { ProjectStages.eCrafterOnlineReview1WithConsultant, "Crafter has 1st online review with consultant" },
            { ProjectStages.eCrafterReadyForTest1, "Crafter is ready for the first (formal) UNS test" },
            { ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1, "Crafter enters answers to the test 1 story questions" },
            { ProjectStages.eCrafterEnterRetellingOfTest1, "Crafter enters test 1 retelling back-translation" },
            { ProjectStages.eConsultantCheckAnchorsRound2, "Consultant checks story anchors for round 2" },
            { ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2, "Consultant checks test 1 answers to story testing questions" },
            { ProjectStages.eConsultantCheckRetellingRound2, "Consultant checks test 1 retelling" },
            { ProjectStages.eCoachReviewRound2Notes, "Coach reviews test 1 notes" },
            { ProjectStages.eConsultantReviseRound2Notes, "Consultant revises round 2 notes based on Coach's feedback" },
            { ProjectStages.eCrafterReviseBasedOnRound2Notes, "Crafter revises story based on round 2 notes" },
            { ProjectStages.eCrafterOnlineReview2WithConsultant, "Crafter has 2nd online review with consultant" },
            { ProjectStages.eCrafterReadyForTest2, "Crafter is ready for the second (formal) UNS test" },
            { ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2, "Crafter enters test 2 answers to story testing questions" },
            { ProjectStages.eCrafterEnterRetellingOfTest2, "Crafter enters test 2 retelling back-translation" },
            { ProjectStages.eConsultantReviewTest2, "Consultant reviews test 2 results" },
            { ProjectStages.eCoachReviewTest2Notes, "Coach reviews test 2 results" },
            { ProjectStages.eTeamComplete, "Story testing complete (waiting for panorama completion review)" }};

        public class StageTransition : List<ProjectStages>
        {
            protected StoryEditor.UserTypes _eEditingMember = StoryEditor.UserTypes.eUndefined;
            protected List<bool> _abViewSettings = null;
            protected string _strTerminalTransitionMessage = null;
            protected string _strStatusMessage = null;
            protected string _strTooltipMessage = null;

            public StageTransition(StoryEditor.UserTypes eEditingMember, 
                string strTerminalTransitionMessage, 
                string strStatusMessage,
                string strTooltipMessage,
                List<ProjectStages> eAllowableStages, List<bool> abViewSettings)
            {
                _eEditingMember = eEditingMember;
                _strTerminalTransitionMessage = strTerminalTransitionMessage;
                _strStatusMessage = strStatusMessage;
                _strTooltipMessage = strTooltipMessage;
                AddRange(eAllowableStages);
                _abViewSettings = abViewSettings;
            }

            public bool IsValidTransition(ProjectStages eToStage, out string strTerminalTransitionMessage)
            {
                bool bAllowed = Contains(eToStage);
                if (bAllowed && (eToStage == this[Count - 1]))
                    strTerminalTransitionMessage = _strTerminalTransitionMessage;
                else
                    strTerminalTransitionMessage = null;
                return bAllowed;
            }

            public StoryEditor.UserTypes MemberTypeWithEditToken
            {
                get { return _eEditingMember; }   
            }

            public void SetView(StoryEditor theSE, out string strStatusMessage, out string strTooltipMessage)
            {
                theSE.viewVernacularLangFieldMenuItem.Checked = _abViewSettings[0];
                theSE.viewNationalLangFieldMenuItem.Checked = _abViewSettings[1];
                theSE.viewEnglishBTFieldMenuItem.Checked = _abViewSettings[2];
                theSE.viewAnchorFieldMenuItem.Checked = _abViewSettings[3];
                theSE.viewStoryTestingQuestionFieldMenuItem.Checked = _abViewSettings[4];
                theSE.viewRetellingFieldMenuItem.Checked = _abViewSettings[5];
                theSE.viewConsultantNoteFieldMenuItem.Checked = _abViewSettings[6];
                theSE.viewCoachNotesFieldMenuItem.Checked = _abViewSettings[7];
                theSE.viewNetBibleMenuItem.Checked = _abViewSettings[8];
                strStatusMessage = _strStatusMessage;
                strTooltipMessage = _strTooltipMessage;
            }
        }
    }
}
