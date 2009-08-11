using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ProjectSettingsForm : Form
    {
        protected ProjectSettings _theProjSettings = null;

        public ProjectSettingsForm(StoriesData theStoriesData, ProjectSettings theProjSettings)
        {
            _theProjSettings = theProjSettings;
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxProjectName.Text))
                MessageBox.Show("You have to select")
        }
    }
}
