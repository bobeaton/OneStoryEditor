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
    public partial class SetTasksForm : Form
    {
        private const string CstrVernacularLangFields = "Vernacular language fields";
        private const string CstrNationalBtLangFields = "National/Regional BT language fields";
        private const string CstrInternationalBtFields = "English BT language fields";
        private const string CstrFreeTranslationFields = "Free translation fields";
        private const string CstrAnchors = "Anchors";
        private const string CstrRetellings = "Retelling fields";
        private const string CstrTestQuestions = "Story testing questions";
        private const string CstrAnswers = "Answers to story testing questions";

        public SetTasksForm(ProjectSettings projSettings, 
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings tasksRequired)
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

            SetCheckState(CstrAnchors,
                TasksPf.TaskSettings.Anchors,
                tasksAllowed, tasksRequired);

            SetCheckState(CstrRetellings,
                TasksPf.TaskSettings.Retellings,
                tasksAllowed, tasksRequired);

            SetCheckState(CstrTestQuestions,
                TasksPf.TaskSettings.TestQuestions,
                tasksAllowed, tasksRequired);

            SetCheckState(CstrAnswers,
                TasksPf.TaskSettings.Answers,
                tasksAllowed, tasksRequired);
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
                    case CstrRetellings:
                        taskAllowed |= TasksPf.TaskSettings.Retellings;
                        break;
                    case CstrTestQuestions:
                        taskAllowed |= TasksPf.TaskSettings.TestQuestions;
                        break;
                    case CstrAnswers:
                        taskAllowed |= TasksPf.TaskSettings.Answers;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
            }
            return taskAllowed;
        }

        private void SetCheckState(string strItemText, TasksPf.TaskSettings task, 
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings tasksRequired)
        {
            int nIndex = checkedListBoxAllowedTasks.Items.Add(strItemText);
            checkedListBoxAllowedTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksAllowed, task));
            nIndex = checkedListBoxRequiredTasks.Items.Add(strItemText);
            checkedListBoxRequiredTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksRequired, task));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
