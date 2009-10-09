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
    public partial class MemberPicker : Form
    {
        protected StoryProjectData _theStoryProjectData;
        protected TeamMemberData _dataSelectedMember = null;
        protected TeamMemberData.UserTypes _eWantedType = TeamMemberData.UserTypes.eUndefined;

        public MemberPicker(StoryProjectData theStoryProjectData, TeamMemberData.UserTypes eWantedType)
        {
            _theStoryProjectData = theStoryProjectData;
            _eWantedType = eWantedType;

            InitializeComponent();

            InitializeListBox(eWantedType);
        }

        protected void InitializeListBox(TeamMemberData.UserTypes eType)
        {
            listBoxUNSs.Items.Clear();
            foreach (TeamMemberData aTMD in _theStoryProjectData.TeamMembers.Values)
                if (aTMD.MemberType == eType)
                    listBoxUNSs.Items.Add(aTMD.Name);

            if (listBoxUNSs.Items.Count > 0)
                listBoxUNSs.SelectedIndex = 0;
        }

        public TeamMemberData SelectedMember
        {
            get { return _dataSelectedMember; }
            set { _dataSelectedMember = value; }
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            try
            {
                TeamMemberData theMember = _theStoryProjectData.EditTeamMembers(null, "&Return");
                InitializeListBox(_eWantedType);
                listBoxUNSs.SelectedItem = theMember.Name;
            }
            catch (StoryEditor.BackOutWithNoUIException)
            {
                // sub-routine has taken care of the UI, just exit without doing anything
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string strSelectedMember = (string)listBoxUNSs.SelectedItem;
            SelectedMember = _theStoryProjectData.TeamMembers[strSelectedMember];

            DialogResult = DialogResult.OK;
            Close();
        }

        private void listBoxUNSs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(null, null);
        }

        private void listBoxUNSs_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (listBoxUNSs.SelectedIndex != -1);                
        }
    }
}
