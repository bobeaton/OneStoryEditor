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
    public partial class UnsPicker : Form
    {
        protected StoriesData _theStoryData = null;
        protected string _strSelectedUnsGuid = null;

        public UnsPicker(StoriesData theStoryData)
        {
            _theStoryData = theStoryData;

            InitializeComponent();

            InitializeListBox();
        }

        protected void InitializeListBox()
        {
            listBoxUNSs.Items.Clear();
            foreach (TeamMemberData aTMD in _theStoryData.TeamMembers.Values)
                if (aTMD.MemberType == StoryEditor.UserTypes.eUNS)
                    listBoxUNSs.Items.Add(aTMD.Name);
        }

        public string SelectedUnsGuid
        {
            get { return _strSelectedUnsGuid; }
            set { _strSelectedUnsGuid = value; }
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            TeamMemberData theUNS = _theStoryData.EditTeamMembers(null);
            InitializeListBox();
            listBoxUNSs.SelectedItem = theUNS.Name;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string strSelectedMember = (string)listBoxUNSs.SelectedItem;
            TeamMemberData theUNS = _theStoryData.TeamMembers[strSelectedMember];
            SelectedUnsGuid = theUNS.MemberGuid;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listBoxUNSs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(null, null);
        }
    }
}
