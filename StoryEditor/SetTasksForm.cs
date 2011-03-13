using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SetTasksForm : Form
    {
        public SetTasksForm()
        {
            InitializeComponent();
        }

        protected void SetCheckState(string strItemText, TasksPf.TaskSettings task, 
            TasksPf.TaskSettings tasksAllowed, TasksPf.TaskSettings tasksRequired)
        {
            int nIndex = checkedListBoxAllowedTasks.Items.Add(strItemText);
            checkedListBoxAllowedTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksAllowed, task));
            nIndex = checkedListBoxRequiredTasks.Items.Add(strItemText);
            checkedListBoxRequiredTasks.SetItemChecked(nIndex, TasksPf.IsTaskOn(tasksRequired, task));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // make sure that if something is required that it's also allowed.
            foreach (int nIndex in checkedListBoxRequiredTasks.CheckedIndices)
            {
                if (!checkedListBoxAllowedTasks.GetItemChecked(nIndex))
                {
                    MessageBox.Show(String.Format(Properties.Resources.IDS_MustAllowToRequireTask,
                                                  checkedListBoxRequiredTasks.Items[nIndex]),
                                    OseResources.Properties.Resources.IDS_Caption);
                    return;
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
