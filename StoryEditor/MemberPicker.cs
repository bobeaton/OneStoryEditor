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
        protected StoriesData _theStoryData = null;
        protected TeamMemberData _dataSelectedMember = null;
        protected StoryEditor.UserTypes _eWantedType = StoryEditor.UserTypes.eUndefined;

        public MemberPicker(StoriesData theStoryData, StoryEditor.UserTypes eWantedType)
        {
            _theStoryData = theStoryData;
            _eWantedType = eWantedType;

            InitializeComponent();

            InitializeListBox(eWantedType);
        }

        protected void InitializeListBox(StoryEditor.UserTypes eType)
        {
            listBoxUNSs.Items.Clear();
            foreach (TeamMemberData aTMD in _theStoryData.TeamMembers.Values)
                if (aTMD.MemberType == eType)
                    listBoxUNSs.Items.Add(aTMD.Name);
        }

        public TeamMemberData SelectedMember
        {
            get { return _dataSelectedMember; }
            set { _dataSelectedMember = value; }
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            TeamMemberData theMember = _theStoryData.EditTeamMembers(null);
            InitializeListBox(_eWantedType);
            listBoxUNSs.SelectedItem = theMember.Name;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string strSelectedMember = (string)listBoxUNSs.SelectedItem;
            SelectedMember = _theStoryData.TeamMembers[strSelectedMember];

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listBoxUNSs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(null, null);
        }
    }
}
