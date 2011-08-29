using System;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SetTasksForm : Form
    {
        public SetTasksForm()
        {
            InitializeComponent();
        }

        public bool Readonly
        {
            get { return !checkedListBoxAllowedTasks.Enabled; /* for e.g */ }
            set
            {
                checkedListBoxAllowedTasks.Enabled =
                    checkedListBoxRequiredTasks.Enabled = 
                    buttonOK.Enabled = !value;
            }
        }

        protected void SetCheckState(string strItemText, TasksPf.TaskSettings task, 
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings tasksRequired)
        {
            int nIndex = checkedListBoxAllowedTasks.Items.Add(strItemText);
            checkedListBoxAllowedTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksAllowed, task));
            nIndex = checkedListBoxRequiredTasks.Items.Add(strItemText);
            checkedListBoxRequiredTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksRequired, task));
        }

        protected virtual bool CheckForRequirements()
        {
            // make sure that if something is required that it's also allowed.
            foreach (int nIndex in
                checkedListBoxRequiredTasks.CheckedIndices.Cast<int>().Where(nIndex => !checkedListBoxAllowedTasks.GetItemChecked(nIndex)))
            {
                MessageBox.Show(String.Format(Localizer.Str("You have to allow a task in order to require it. Please check the box in the Allowed Tasks list for '{0}'"),
                                              checkedListBoxRequiredTasks.Items[nIndex]),
                                StoryEditor.OseCaption);
                return false;
            }

            return true;
        }
    }
}
