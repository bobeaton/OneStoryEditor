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
    public partial class TeamMemberForm : Form
    {
        internal const string cstrCrafter = "Crafter";
        internal const string cstrUNS = "UNS";
        internal const string cstrConsultantInTraining = "Consultant-in-Training";
        internal const string cstrIndependentConsultant = "IndependentConsultant";
        internal const string cstrCoach = "Coach";
        internal const string cstrJustLooking = "JustLooking"; // gives full access, but no change privileges

        protected StoryProject m_projFile = null;
        protected string m_strSelectedMember = null;

        Dictionary<string, StoryProject.MemberRow> m_mapNewMembersThisSession = new Dictionary<string, StoryProject.MemberRow>();

        public TeamMemberForm(StoryProject projFile)
        {
            m_projFile = projFile;
            InitializeComponent();

            foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
            {
                listBoxTeamMembers.Items.Add(aMemberRow.name);
                listBoxMemberRoles.Items.Add(aMemberRow.memberType);
            }

            if ((listBoxTeamMembers.Items.Count > 0) && !String.IsNullOrEmpty(Properties.Settings.Default.LastMember))
                listBoxTeamMembers.SelectedItem = Properties.Settings.Default.LastMember;
        }

        public string SelectedMember
        {
            get { return m_strSelectedMember; }
            set
            {
                System.Diagnostics.Debug.Assert(listBoxTeamMembers.Items.Contains(value));
                listBoxTeamMembers.SelectedItem = m_strSelectedMember = value;
            }
        }

        public string MemberName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        public string MemberTypeString
        {
            get
            {
                if (radioButtonStoryCrafter.Checked)
                    return cstrCrafter;
                else if (radioButtonUNS.Checked)
                    return cstrUNS;
                else if (radioButtonConsultantInTraining.Checked)
                    return cstrConsultantInTraining;
                else if (radioButtonIndependentConsultant.Checked)
                    return cstrIndependentConsultant;
                else if (radioButtonCoach.Checked)
                    return cstrCoach;
                else
                    return cstrJustLooking;
            }
            set
            {
                if (value == cstrCrafter)
                    radioButtonStoryCrafter.Checked = true;
                else if (value == cstrUNS)
                    radioButtonUNS.Checked = true;
                else if (value == cstrConsultantInTraining)
                    radioButtonConsultantInTraining.Checked = true;
                else if (value == cstrIndependentConsultant)
                    radioButtonIndependentConsultant.Checked = true;
                else if (value == cstrCoach)
                    radioButtonCoach.Checked = true;
                else // if (value == cstrJustLooking)
                    radioButtonJustViewing.Checked = true;
            }
        }

        public StoryEditor.UserTypes UserType
        {
            get { return GetUserType(MemberTypeString); }
        }

        public static StoryEditor.UserTypes GetUserType(string strMemberTypeString)
        {
            if (strMemberTypeString == cstrCrafter)
                return StoryEditor.UserTypes.eCrafter;
            else if (strMemberTypeString == cstrUNS)
                return StoryEditor.UserTypes.eUNS;
            else if (strMemberTypeString == cstrConsultantInTraining)
                return StoryEditor.UserTypes.eConsultantInTraining;
            else if (strMemberTypeString == cstrIndependentConsultant)
                return StoryEditor.UserTypes.eIndependentConsultant;
            else if (strMemberTypeString == cstrCoach)
                return StoryEditor.UserTypes.eCoach;
            else if (strMemberTypeString == cstrJustLooking)
                return StoryEditor.UserTypes.eJustLooking;
            else
                return StoryEditor.UserTypes.eUndefined;
        }

        public string Email
        {
            get { return textBoxEmail.Text; }
            set { textBoxEmail.Text = value; }
        }

        public string Phone
        {
            get { return textBoxPhoneNumber.Text; }
            set { textBoxPhoneNumber.Text = value; }
        }

        public string AltPhone
        {
            get { return textBoxAltPhone.Text; }
            set { textBoxAltPhone.Text = value; }
        }

        public string SkypeID
        {
            get { return textBoxSkypeID.Text; }
            set { textBoxSkypeID.Text = value; }
        }

        public string TeamViewerID
        {
            get { return textBoxTeamViewer.Text; }
            set { textBoxTeamViewer.Text = value; }
        }

        public string Address
        {
            get { return textBoxAddress.Text; }
            set { textBoxAddress.Text = value; }
        }

        private void buttonEditOK_Click(object sender, EventArgs e)
        {
            try
            {
                FinishEdit();

                Properties.Settings.Default.LastMember = SelectedMember;
                Properties.Settings.Default.Save();

                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StoryEditor.cstrCaption);
            }
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            try
            {
                FinishEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StoryEditor.cstrCaption);
            }
        }

        protected void FinishEdit()
        {
            if (String.IsNullOrEmpty(this.MemberName))
                throw new ApplicationException("You have to enter at least a name and indicate your role (even if you're 'just looking')!");

            StoryProject.MemberRow theNewMember;
            if (m_mapNewMembersThisSession.TryGetValue(this.MemberName, out theNewMember))
            {
                // must just be editing the already added member...
                System.Diagnostics.Debug.Assert(listBoxTeamMembers.Items.Contains(MemberName));

                theNewMember.memberType = this.MemberTypeString;
                theNewMember.email = this.Email;
                theNewMember.altPhone = this.AltPhone;
                theNewMember.phone = this.Phone;
                theNewMember.address = this.Address;
                theNewMember.skypeID = this.SkypeID;
                theNewMember.teamViewerID = this.TeamViewerID;
            }
            else if (listBoxTeamMembers.Items.Contains(this.MemberName))
            {
                throw new ApplicationException(String.Format("Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then go back to the 'Team Members' tab, select the member, and click the 'Edit Member' button", MemberName));
            }
            else
            {
                // add this new user to the proj file
                theNewMember = m_projFile.Member.AddMemberRow(this.MemberName,
                    this.MemberTypeString, this.Email, this.AltPhone, this.Phone, this.Address, 
                    this.SkypeID, this.TeamViewerID, String.Format("mem-{0}", Guid.NewGuid().ToString()), 
                    m_projFile.Members[0]);
                m_mapNewMembersThisSession.Add(this.MemberName, theNewMember);
                listBoxTeamMembers.Items.Add(this.MemberName);
                listBoxTeamMembers.SelectedItem = this.MemberName;
            }

            Modified = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listBoxTeamMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bOneSelected = (listBoxTeamMembers.SelectedIndex != -1);
            buttonEditMember.Enabled = buttonOK.Enabled = bOneSelected;

            // the delete button is only enabled during the current session (just to prevent them
            //  from removing a team member that has other references in the project).
            if (bOneSelected)
            {
                m_strSelectedMember = (string)listBoxTeamMembers.SelectedItem;
                buttonDeleteMember.Enabled = m_mapNewMembersThisSession.ContainsKey(SelectedMember);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);

            Properties.Settings.Default.LastMember = SelectedMember;
            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            // unselect any member and set the target tab (see 
            //  tabControlProjectMetaData_Selected for what happens)
            listBoxTeamMembers.SelectedIndex = -1;
            tabControlProjectMetaData.SelectedTab = tabPageEditMember;
        }

        protected bool Modified = false;

        protected void EditMemberInfo()
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);

            m_strSelectedMember = (string)listBoxTeamMembers.SelectedItem;
            StoryProject.MemberRow theMemberRow = null;
            foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                if (SelectedMember == aMemberRow.name)
                {
                    theMemberRow = aMemberRow;
                    break;
                }

            System.Diagnostics.Debug.Assert(theMemberRow != null);
            this.MemberName = theMemberRow.name;
            this.MemberTypeString = theMemberRow.memberType;
            this.Email = (theMemberRow.IsemailNull()) ? null : theMemberRow.email;
            this.AltPhone = (theMemberRow.IsaltPhoneNull()) ? null : theMemberRow.altPhone;
            this.Phone = (theMemberRow.IsphoneNull()) ? null : theMemberRow.phone;
            this.Address = (theMemberRow.IsaddressNull()) ? null : theMemberRow.address;
            this.SkypeID = (theMemberRow.IsskypeIDNull()) ? null : theMemberRow.skypeID;
            this.TeamViewerID = (theMemberRow.IsteamViewerIDNull()) ? null : theMemberRow.teamViewerID;

            // keep a hang on it so we don't try to, for example, give it a new guid
            if (!m_mapNewMembersThisSession.ContainsKey(MemberName))
                m_mapNewMembersThisSession.Add(MemberName, theMemberRow);
        }

        private void buttonEditMember_Click(object sender, EventArgs e)
        {
            // act as if the user clicked the 'Edit Member Information' tab (which does what we want)
            tabControlProjectMetaData.SelectedTab = tabPageEditMember;
        }

        private void buttonDeleteMember_Click(object sender, EventArgs e)
        {
            // this is only enabled if we added the member this session
            System.Diagnostics.Debug.Assert(m_mapNewMembersThisSession.ContainsKey(SelectedMember));

            StoryProject.MemberRow theMember;
            if (m_mapNewMembersThisSession.TryGetValue(SelectedMember, out theMember))
            {
                foreach (StoryProject.MemberRow aMemberRow in m_projFile.Member)
                    if (theMember.name == aMemberRow.name)
                    {
                        m_projFile.Member.RemoveMemberRow(theMember);
                        break;
                    }
            }

            m_mapNewMembersThisSession.Remove(SelectedMember);
        }

        private void tabControlProjectMetaData_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageMemberList)
            {
                Console.WriteLine("tabPageMemberList Selected");

                // if the user made some changes and then is moving away from the tab, 
                //  then do an implicit Accept
                if (Modified && String.IsNullOrEmpty(this.MemberName))
                    buttonAccept_Click(null, null);
            }
            else
            {
                Console.WriteLine("tabPageEditMember Selected");
                System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
                
                // behave just as if they'd clicked the Edit member button
                EditMemberInfo();
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            Modified = true;

            buttonEditOK.Enabled = buttonAccept.Enabled = !String.IsNullOrEmpty(textBoxName.Text);
        }

        private void tabControlProjectMetaData_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (listBoxTeamMembers.SelectedIndex == -1)
            {
                MessageBox.Show("You have to select a member before you can choose the 'Edit Member Information' tab", StoryEditor.cstrCaption);
                e.Cancel = true;
            }
        }
    }
}
