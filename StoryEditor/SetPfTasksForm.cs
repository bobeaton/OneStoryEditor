using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

        public SetPfTasksForm()
        {
            
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
        }

        public TasksPf.TaskSettings TasksAllowed
        {
            get { return ReadCheckedStates(checkedListBoxAllowedTasks); }
        }

        public TasksPf.TaskSettings TasksRequired
        {
            get { return ReadCheckedStates(checkedListBoxRequiredTasks); }
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

        private void SetPfTasksForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // make sure they haven't requested both one and two tests
            TasksPf.TaskSettings taskRequired = TasksRequired;
            if (TasksPf.IsTaskOn(taskRequired, TasksPf.TaskSettings.Retellings)
                && (TasksPf.IsTaskOn(taskRequired, TasksPf.TaskSettings.Retellings2)))
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_CantHaveBoth1And2,
                                              "retelling"),
                                OseResources.Properties.Resources.IDS_Caption);
                e.Cancel = true;
            }
            else if (TasksPf.IsTaskOn(taskRequired, TasksPf.TaskSettings.Answers)
                && (TasksPf.IsTaskOn(taskRequired, TasksPf.TaskSettings.Answers2)))
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_CantHaveBoth1And2,
                                              "story question"),
                                OseResources.Properties.Resources.IDS_Caption);
                e.Cancel = true;
            }
        }
    }
}
