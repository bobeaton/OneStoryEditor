﻿using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SetPfTasksForm : SetTasksForm
    {
        private const string CstrVernacularLangFields = "Edit story language";
        private const string CstrNationalBtLangFields = "Edit national/regional BT language";
        private const string CstrInternationalBtFields = "Edit English BT language";
        private const string CstrFreeTranslationFields = "Edit free translation";
        private const string CstrAnchors = "Add Anchors";
        private const string CstrRetellingTest1 = "Do 1 more retelling test";
        private const string CstrRetellingTest2 = "Do 2 more retelling tests";
        private const string CstrTestQuestion = "Add story testing questions";
        private const string CstrAnswers1 = "Do 1 more story question test";
        private const string CstrAnswers2 = "Do 2 more story question tests";

        private readonly ProjectSettings _projSettings;

        public SetPfTasksForm(ProjectSettings projSettings)
        {
            _projSettings = projSettings;
        }

        public SetPfTasksForm(ProjectSettings projSettings,
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings tasksRequired,
            bool bIsBiblicalStory)
        {
            InitializeComponent();

            if (projSettings.Vernacular.HasData)
            {
                SetCheckState(CstrVernacularLangFields, 
                    TasksPf.TaskSettings.VernacularLangFields, 
                    tasksAllowed, tasksRequired);
            }

            if (projSettings.NationalBT.HasData)
            {
                SetCheckState(CstrNationalBtLangFields,
                    TasksPf.TaskSettings.NationalBtLangFields,
                    tasksAllowed, tasksRequired);
            }

            if (projSettings.InternationalBT.HasData)
            {
                SetCheckState(CstrInternationalBtFields,
                    TasksPf.TaskSettings.InternationalBtFields,
                    tasksAllowed, tasksRequired);
            }

            if (projSettings.FreeTranslation.HasData)
            {
                SetCheckState(CstrFreeTranslationFields,
                    TasksPf.TaskSettings.FreeTranslationFields,
                    tasksAllowed, tasksRequired);
            }

            if (bIsBiblicalStory)
            {
                SetCheckState(CstrAnchors,
                    TasksPf.TaskSettings.Anchors,
                    tasksAllowed, tasksRequired);

                SetCheckState(CstrRetellingTest1,
                    TasksPf.TaskSettings.Retellings,
                    tasksAllowed, tasksRequired);

                SetCheckState(CstrRetellingTest2,
                    TasksPf.TaskSettings.Retellings2,
                    tasksAllowed, tasksRequired);

                SetCheckState(CstrTestQuestion,
                    TasksPf.TaskSettings.TestQuestions,
                    tasksAllowed, tasksRequired);

                SetCheckState(CstrAnswers1,
                    TasksPf.TaskSettings.Answers,
                    tasksAllowed, tasksRequired);

                SetCheckState(CstrAnswers2,
                    TasksPf.TaskSettings.Answers2,
                    tasksAllowed, tasksRequired);
            }

            _projSettings = projSettings;
        }

        public TasksPf.TaskSettings TasksAllowed
        {
            get { return ReadCheckedStates(checkedListBoxAllowedTasks); }
        }

        public TasksPf.TaskSettings TasksRequired
        {
            get { return ReadCheckedStates(checkedListBoxRequiredTasks); }
        }

        public static bool EditPfTasks(StoryEditor theSe, ref StoryData theStory)
        {
            var dlg = new SetPfTasksForm(theSe.StoryProject.ProjSettings,
                                         theStory.TasksAllowedPf, theStory.TasksRequiredPf,
                                         theStory.CraftingInfo.IsBiblicalStory)
            {
                Text = String.Format("Set tasks for the Project Facilitator ({0}) to do on story: {1}",
                                     theSe.StoryProject.GetMemberNameFromMemberGuid(
                                         theStory.CraftingInfo.ProjectFacilitator.MemberId),
                                     theStory.Name)
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            theStory.TasksAllowedPf = dlg.TasksAllowed;
            theStory.TasksRequiredPf = dlg.TasksRequired;
            return true;
        }

        private static TasksPf.TaskSettings ReadCheckedStates(CheckedListBox checkedListBox)
        {
            TasksPf.TaskSettings taskAllowed = TasksPf.TaskSettings.None;
            foreach (string strCheckedItem in checkedListBox.CheckedItems)
            {
                switch (strCheckedItem)
                {
                    case CstrVernacularLangFields:
                        taskAllowed |= TasksPf.TaskSettings.VernacularLangFields;
                        break;
                    case CstrNationalBtLangFields:
                        taskAllowed |= TasksPf.TaskSettings.NationalBtLangFields;
                        break;
                    case CstrInternationalBtFields:
                        taskAllowed |= TasksPf.TaskSettings.InternationalBtFields;
                        break;
                    case CstrFreeTranslationFields:
                        taskAllowed |= TasksPf.TaskSettings.FreeTranslationFields;
                        break;
                    case CstrAnchors:
                        taskAllowed |= TasksPf.TaskSettings.Anchors;
                        break;
                    case CstrRetellingTest1:
                        taskAllowed |= TasksPf.TaskSettings.Retellings;
                        break;
                    case CstrRetellingTest2:
                        taskAllowed |= TasksPf.TaskSettings.Retellings2;
                        break;
                    case CstrTestQuestion:
                        taskAllowed |= TasksPf.TaskSettings.TestQuestions;
                        break;
                    case CstrAnswers1:
                        taskAllowed |= TasksPf.TaskSettings.Answers;
                        break;
                    case CstrAnswers2:
                        taskAllowed |= TasksPf.TaskSettings.Answers2;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
            }
            return taskAllowed;
        }

        protected override bool CheckForRequirements()
        {
            if (!base.CheckForRequirements())
                return false;

            TasksPf.TaskSettings tasksRequired = TasksRequired;
            TasksPf.TaskSettings tasksAllowed = TasksAllowed;
            string strFieldToAllow = null, strFieldRequired;
            if (TasksPf.IsTaskOn(tasksRequired, TasksPf.TaskSettings.Retellings)
                && (TasksPf.IsTaskOn(tasksRequired, TasksPf.TaskSettings.Retellings2)))
            {
                WarnAboutIncompatibityInTestRequirement(Localizer.Str("retelling"));
                return false;
            }

            if (TasksPf.IsTaskOn(tasksRequired, TasksPf.TaskSettings.Answers)
                     && (TasksPf.IsTaskOn(tasksRequired, TasksPf.TaskSettings.Answers2)))
            {
                WarnAboutIncompatibityInTestRequirement(Localizer.Str("story question"));
                return false;
            }

            // another scenario is that if any retelling (or answers) are required, then
            //  make sure the PF has the permission to do what's required
            if ((CheckForRequirements(tasksRequired, tasksAllowed,
                                      TasksPf.TaskSettings.Retellings,
                                      _projSettings.ShowRetellings,
                                      ref strFieldToAllow) &&
                 !String.IsNullOrEmpty(strFieldRequired = CstrRetellingTest1)) ||
                (CheckForRequirements(tasksRequired, tasksAllowed,
                                      TasksPf.TaskSettings.Retellings2,
                                      _projSettings.ShowRetellings,
                                      ref strFieldToAllow) &&
                 !String.IsNullOrEmpty(strFieldRequired = CstrRetellingTest2)) ||
                (CheckForRequirements(tasksRequired, tasksAllowed,
                                      TasksPf.TaskSettings.TestQuestions,
                                      _projSettings.ShowTestQuestions,
                                      ref strFieldToAllow) &&
                 !String.IsNullOrEmpty(strFieldRequired = CstrTestQuestion)) ||
                (CheckForRequirements(tasksRequired, tasksAllowed,
                                      TasksPf.TaskSettings.Answers,
                                      _projSettings.ShowAnswers,
                                      ref strFieldToAllow) &&
                 !String.IsNullOrEmpty(strFieldRequired = CstrAnswers1)) ||
                (CheckForRequirements(tasksRequired, tasksAllowed,
                                      TasksPf.TaskSettings.Answers2,
                                      _projSettings.ShowAnswers,
                                      ref strFieldToAllow) &&
                 !String.IsNullOrEmpty(strFieldRequired = CstrAnswers2)))
            {
                MessageBox.Show(String.Format(Localizer.Str("If you require the project facilitator to '{0}', then you'll also need to allow the '{1}' task?"),
                                              strFieldRequired, strFieldToAllow),
                                StoryEditor.OseCaption);
                return false;
            }
            
            if (TasksPf.IsTaskOn(tasksRequired,
                                  TasksPf.TaskSettings.Answers | 
                                  TasksPf.TaskSettings.Answers2) &&
                 !TasksPf.IsTaskOn(tasksAllowed,
                                   TasksPf.TaskSettings.TestQuestions))
            {
                MessageBox.Show(String.Format(Localizer.Str("If you're going to have the project facilitator do story inference testing, then you probably need to allow them to '{0}' also"),
                                              CstrTestQuestion),
                                StoryEditor.OseCaption);
                return false;
            }

            return true;
        }

        private static void WarnAboutIncompatibityInTestRequirement(string strTestType)
        {
            MessageBox.Show(String.Format(Localizer.Str("Did you want the project facilitator to do 1 or 2 {0} tests?"),
                                          strTestType),
                            StoryEditor.OseCaption);
        }

        private bool CheckForRequirements(TasksPf.TaskSettings tasksRequired,
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings toCheck,
            ShowLanguageFields showLanguageFields, ref string strFieldToAllow)
        {
            if (TasksPf.IsTaskOn(tasksRequired, toCheck))
            {
                return ((CheckForRequirement(_projSettings.Vernacular.HasData,
                                             showLanguageFields.Vernacular, tasksAllowed,
                                             TasksPf.TaskSettings.VernacularLangFields) &&
                         !String.IsNullOrEmpty(strFieldToAllow = CstrVernacularLangFields)) ||
                        (CheckForRequirement(_projSettings.NationalBT.HasData,
                                             showLanguageFields.NationalBt, tasksAllowed,
                                             TasksPf.TaskSettings.NationalBtLangFields) &&
                         !String.IsNullOrEmpty(strFieldToAllow = CstrNationalBtLangFields)) ||
                        (CheckForRequirement(_projSettings.InternationalBT.HasData,
                                             showLanguageFields.InternationalBt, tasksAllowed,
                                             TasksPf.TaskSettings.InternationalBtFields) &&
                         !String.IsNullOrEmpty(strFieldToAllow = CstrInternationalBtFields)));
            }
            return false;
        }

        private static bool CheckForRequirement(bool bHasData, bool bShowLanguageField, 
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings taskToCheck)
        {
            return bHasData && bShowLanguageField && 
                   !TasksPf.IsTaskOn(tasksAllowed, taskToCheck);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!CheckForRequirements())
                return;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
