using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class NewProjectWizard : Form
    {
        public NewProjectWizard()
        {
            InitializeComponent();

            tabControl.Controls.Remove(tabPageLanguage);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageProjectName)
            {
                
            }
            else if (tabControl.SelectedTab == tabPageLanguages)
            {
                /*
                if (checkBoxStoryLanguage.Checked)
                    tabControl.Controls.SetChildIndex();
                */
            }
            else if (tabControl.SelectedTab == tabPageMemberRoles)
            {
                
            }
        }

        protected TabPage StoryLanguagePage
        {
            get
            {
                return new TabPage().;
            }
        }
    }
}
