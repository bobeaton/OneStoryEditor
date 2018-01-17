using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SetCitTasksForm : SetTasksForm
    {
        public static string CstrSendToCoach
        {
            get { return Localizer.Str("Set to coach's turn"); }
        }

        private static string CstrSendToProjectFacilitator
        {
            get { return Localizer.Str("Set to project facilitator's turn"); }
        }
        
        private SetCitTasksForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public SetCitTasksForm(TasksCit.TaskSettings tasksAllowed, 
            TasksCit.TaskSettings tasksRequired)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

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
                Text = String.Format(Localizer.Str("Set tasks for the CIT to do on story: {0}"),
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
            var taskAllowed = TasksCit.TaskSettings.None;
            foreach (string strCheckedItem in checkedListBox.CheckedItems)
            {
                if (strCheckedItem == CstrSendToCoach)
                {
                    taskAllowed |= TasksCit.TaskSettings.SendToCoachForReview;
                }
                else if (strCheckedItem == CstrSendToProjectFacilitator)
                {
                    taskAllowed |= TasksCit.TaskSettings.SendToProjectFacilitatorForRevision;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false);
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

            // make sure that only one of these is a required task
            if (TasksCit.IsTaskOn(TasksRequired, TasksCit.TaskSettings.SendToCoachForReview) && TasksCit.IsTaskOn(TasksRequired, TasksCit.TaskSettings.SendToProjectFacilitatorForRevision))
            {
                LocalizableMessageBox.Show(Localizer.Str("Only one of these should be a required task. Please uncheck one of them and click 'OK' again."),
                                           StoryEditor.OseCaption);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
