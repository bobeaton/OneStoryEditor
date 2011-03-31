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
    public partial class MemberPicker : TopForm
    {
        protected StoryProjectData _theStoryProjectData;
        protected TeamMemberData _dataSelectedMember = null;
        protected TeamMemberData.UserTypes _eWantedType = TeamMemberData.UserTypes.eUndefined;

        public MemberPicker(StoryProjectData theStoryProjectData, TeamMemberData.UserTypes eWantedType)
            : base(true)
        {
            _theStoryProjectData = theStoryProjectData;
            _eWantedType = eWantedType;

            InitializeComponent();
        }

        public new DialogResult ShowDialog()
        {
            InitializeListBox(_eWantedType);
            return base.ShowDialog();
        }

        public string ItemToBlock { get; set; }

        protected void InitializeListBox(TeamMemberData.UserTypes eType)
        {
            listBoxUNSs.Items.Clear();
            foreach (TeamMemberData aTMD in
                _theStoryProjectData.TeamMembers.Values.Where(aTMD => (aTMD.MemberType == eType) && (aTMD.Name != ItemToBlock)))
            {
                listBoxUNSs.Items.Add(aTMD.Name);
            }

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
                bool bModified = false;
                TeamMemberData theMember = _theStoryProjectData.EditTeamMembers(null, false, _theStoryProjectData.ProjSettings, ref bModified);
                InitializeListBox(_eWantedType);
                listBoxUNSs.SelectedItem = theMember.Name;
            }
            catch (StoryProjectData.BackOutWithNoUIException)
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
