using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SetCitTasksForm : SetTasksForm
    {
        public const string CstrSendToCoach = "Set to coach's turn";
        private const string CstrSendToProjectFacilitator = "Set to project facilitator's turn";

        public SetCitTasksForm(TasksCit.TaskSettings tasksAllowed, 
            TasksCit.TaskSettings tasksRequired)
        {
            InitializeComponent();

            SetCheckState(CstrSendToCoach, 
                TasksCit.TaskSettings.SendToCoachForReview, 
                tasksAllowed, tasksRequired);

            SetCheckState(CstrSendToProjectFacilitator,
                TasksCit.TaskSettings.SendToProjectFacilitatorForRevision,
                tasksAllowed, tasksRequired);
        }

        public TasksCit.TaskSettings TasksAllowed
        {
            get { return ReadCheckedStates(checkedListBoxAllowedTasks); }
        }

        public TasksCit.TaskSettings TasksRequired
        {
            get { return ReadCheckedStates(checkedListBoxRequiredTasks); }
        }

        public static bool EditCitTasks(ref StoryData theStory)
        {
            var dlg = new SetCitTasksForm(theStory.TasksAllowedCit,
                                          theStory.TasksRequiredCit)
            {
                Text = String.Format("Set tasks for the CIT to do on story: {0}",
                                     theStory.Name)
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            theStory.TasksAllowedCit = dlg.TasksAllowed;
            theStory.TasksRequiredCit = dlg.TasksRequired;
            return true;
        }

        private static TasksCit.TaskSettings ReadCheckedStates(CheckedListBox checkedListBox)
        {
            TasksCit.TaskSettings taskAllowed = TasksCit.TaskSettings.None;
            foreach (string strCheckedItem in checkedListBox.CheckedItems)
            {
                switch (strCheckedItem)
                {
                    case CstrSendToCoach:
                        taskAllowed |= TasksCit.TaskSettings.SendToCoachForReview;
                        break;
                    case CstrSendToProjectFacilitator:
                        taskAllowed |= TasksCit.TaskSettings.SendToProjectFacilitatorForRevision;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
            }
            return taskAllowed;
        }

        private void SetCheckState(string strItemText, TasksCit.TaskSettings task, 
            TasksCit.TaskSettings tasksAllowed, TasksCit.TaskSettings tasksRequired)
        {
            int nIndex = checkedListBoxAllowedTasks.Items.Add(strItemText);
            checkedListBoxAllowedTasks.SetItemChecked(nIndex, TasksCit.IsTaskOn(tasksAllowed, task));
            nIndex = checkedListBoxRequiredTasks.Items.Add(strItemText);
            checkedListBoxRequiredTasks.SetItemChecked(nIndex, TasksCit.IsTaskOn(tasksRequired, task));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!base.CheckForRequirements())
                return;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
