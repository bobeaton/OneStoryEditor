using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class TeamMemberForm : Form
    {
        protected ProjectSettings m_projSettings = null;
        protected TeamMembersData _dataTeamMembers = null;
        protected string m_strSelectedMember = null;

        Dictionary<string, TeamMemberData> m_mapNewMembersThisSession = new Dictionary<string, TeamMemberData>();

        public TeamMemberForm(TeamMembersData dataTeamMembers, ProjectSettings projSettings)
        {
            _dataTeamMembers = dataTeamMembers;
            m_projSettings = projSettings;

            InitializeComponent();

            foreach (TeamMemberData aMember in dataTeamMembers.Values)
            {
                listBoxTeamMembers.Items.Add(aMember.Name);
                listBoxMemberRoles.Items.Add(aMember.MemberTypeAsString);
            }

            if ((listBoxTeamMembers.Items.Count > 0) && !String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
                listBoxTeamMembers.SelectedItem = Properties.Settings.Default.LastMemberLogin;

            textBoxVernacular.Font = textBoxVernacularEthCode.Font = projSettings.VernacularFont;
            textBoxVernacular.ForeColor = textBoxVernacularEthCode.ForeColor = projSettings.VernacularFontColor;
            textBoxVernacular.Text = projSettings.VernacularLangName;
            textBoxVernacularEthCode.Text = projSettings.VernacularLangCode;

            textBoxNationalBTLanguage.Font = textBoxNationalBTEthCode.Font = projSettings.NationalBTFont;
            textBoxNationalBTLanguage.ForeColor = textBoxNationalBTEthCode.ForeColor = projSettings.NationalBTFontColor;
            textBoxNationalBTLanguage.Text = projSettings.NationalBTLangName;
            textBoxNationalBTEthCode.Text = projSettings.NationalBTLangCode;
        }

        public string SelectedMember
        {
            get { return m_strSelectedMember; }
            set
            {
                if (!listBoxTeamMembers.Items.Contains(value))
                    throw new ApplicationException(String.Format("Project File doesn't contain a member named '{0}'", value));
                listBoxTeamMembers.SelectedItem = m_strSelectedMember = value;
            }
        }

        public string MemberName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        public StoryEditor.UserTypes MemberType
        {
            get
            {
                if (radioButtonStoryCrafter.Checked)
                    return StoryEditor.UserTypes.eCrafter;
                else if (radioButtonUNS.Checked)
                    return StoryEditor.UserTypes.eUNS;
                else if (radioButtonConsultantInTraining.Checked)
                    return StoryEditor.UserTypes.eConsultantInTraining;
                else if (radioButtonIndependentConsultant.Checked)
                    return StoryEditor.UserTypes.eIndependentConsultant;
                else if (radioButtonCoach.Checked)
                    return StoryEditor.UserTypes.eCoach;
                else
                    return StoryEditor.UserTypes.eJustLooking;
            }
            set
            {
                switch (value)
                {
                    case StoryEditor.UserTypes.eCrafter:
                        radioButtonStoryCrafter.Checked = true;
                        break;
                    case StoryEditor.UserTypes.eUNS:
                        radioButtonUNS.Checked = true;
                        break;
                    case StoryEditor.UserTypes.eConsultantInTraining:
                        radioButtonConsultantInTraining.Checked = true;
                        break;
                    case StoryEditor.UserTypes.eIndependentConsultant:
                        radioButtonIndependentConsultant.Checked = true;
                        break;
                    case StoryEditor.UserTypes.eCoach:
                        radioButtonCoach.Checked = true;
                        break;
                    case StoryEditor.UserTypes.eJustLooking:
                        radioButtonJustViewing.Checked = true;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false); // should get here.
                        break;
                }
            }
        }

        public string Email
        {
            get { return (String.IsNullOrEmpty(textBoxEmail.Text) ? null : textBoxEmail.Text); }
            set { textBoxEmail.Text = value; }
        }

        public string Phone
        {
            get { return (String.IsNullOrEmpty(textBoxPhoneNumber.Text) ? null : textBoxPhoneNumber.Text); }
            set { textBoxPhoneNumber.Text = value; }
        }

        public string AltPhone
        {
            get { return (String.IsNullOrEmpty(textBoxAltPhone.Text) ? null : textBoxAltPhone.Text); }
            set { textBoxAltPhone.Text = value; }
        }

        public string SkypeID
        {
            get { return (String.IsNullOrEmpty(textBoxSkypeID.Text) ? null : textBoxSkypeID.Text); }
            set { textBoxSkypeID.Text = value; }
        }

        public string TeamViewerID
        {
            get { return (String.IsNullOrEmpty(textBoxTeamViewer.Text) ? null : textBoxTeamViewer.Text); }
            set { textBoxTeamViewer.Text = value; }
        }

        public string Address
        {
            get { return (String.IsNullOrEmpty(textBoxAddress.Text) ? null : textBoxAddress.Text); }
            set { textBoxAddress.Text = value; }
        }

        private void DoAccept()
        {
            try
            {
                FinishEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StoryEditor.CstrCaption);
            }
        }

        protected void FinishEdit()
        {
            if (String.IsNullOrEmpty(this.MemberName))
                throw new ApplicationException("You have to enter at least a name and indicate your role (even if you're 'just looking')!");

            TeamMemberData theNewMemberData;
            if (m_mapNewMembersThisSession.TryGetValue(this.MemberName, out theNewMemberData))
            {
                // must just be editing the already added member...
                System.Diagnostics.Debug.Assert(listBoxTeamMembers.Items.Contains(MemberName));

                theNewMemberData.MemberType = this.MemberType;
                theNewMemberData.Email = this.Email;
                theNewMemberData.AltPhone = this.AltPhone;
                theNewMemberData.Phone = this.Phone;
                theNewMemberData.Address = this.Address;
                theNewMemberData.SkypeID = this.SkypeID;
                theNewMemberData.TeamViewerID = this.TeamViewerID;

                // update the role listbox
                int nIndex = listBoxTeamMembers.Items.IndexOf(MemberName);
                listBoxMemberRoles.Items[nIndex] = theNewMemberData.MemberTypeAsString;
            }
            else if (listBoxTeamMembers.Items.Contains(this.MemberName))
            {
                throw new ApplicationException(String.Format("Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then go back to the 'Team Members' tab, select the member, and click the 'Edit Member' button", MemberName));
            }
            else
            {
                // add this new user to the proj file
                theNewMemberData = new TeamMemberData(this.MemberName,
                    this.MemberType, String.Format("mem-{0}", Guid.NewGuid().ToString()),
                    this.Email, this.SkypeID, this.TeamViewerID, this.Phone, this.AltPhone, 
                    this.Address);
                m_mapNewMembersThisSession.Add(this.MemberName, theNewMemberData);
                listBoxTeamMembers.Items.Add(this.MemberName);
                listBoxMemberRoles.Items.Add(theNewMemberData.MemberTypeAsString);
                listBoxTeamMembers.SelectedItem = this.MemberName;
            }

            // update the language information as well (in case that was changed also)
            m_projSettings.VernacularLangName = textBoxVernacular.Text;
            m_projSettings.VernacularLangCode = textBoxVernacularEthCode.Text;
            m_projSettings.VernacularFont = textBoxVernacular.Font;
            m_projSettings.VernacularFontColor = textBoxVernacular.ForeColor;

            m_projSettings.NationalBTLangName = textBoxNationalBTLanguage.Text;
            m_projSettings.NationalBTLangCode = textBoxNationalBTEthCode.Text;
            m_projSettings.NationalBTFont = textBoxNationalBTLanguage.Font;
            m_projSettings.NationalBTFontColor = textBoxNationalBTLanguage.ForeColor;

            // English was done by the font dialog handler

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
                buttonDeleteMember.Visible = m_mapNewMembersThisSession.ContainsKey(SelectedMember);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);

            // if the selected user is a UNS, this is probably a mistake.
            TeamMemberData theMember = _dataTeamMembers[SelectedMember];
            if (theMember.MemberType == StoryEditor.UserTypes.eUNS)
            {
                MessageBox.Show("You may have added a UNS in order to identify, for example, which UNS did the back translation or a particular test. However, you as the crafter should still be logged in to enter the UNS's comments. So select your *crafter* member name and click 'Login' again", StoryEditor.CstrCaption);
                return;
            }

            // indicate that this is the currently logged on user
            // _dataTeamMembers.LoggedOn = _dataTeamMembers[SelectedMember];

            Properties.Settings.Default.LastMemberLogin = SelectedMember;
            Properties.Settings.Default.LastUserType = _dataTeamMembers[SelectedMember].MemberTypeAsString;
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
            System.Diagnostics.Debug.Assert(_dataTeamMembers.ContainsKey(m_strSelectedMember));
            TeamMemberData theMemberData = _dataTeamMembers[m_strSelectedMember];

            this.MemberName = theMemberData.Name;
            this.MemberType = theMemberData.MemberType;
            this.Email = theMemberData.Email;
            this.AltPhone = theMemberData.AltPhone;
            this.Phone = theMemberData.Phone;
            this.Address = theMemberData.Address;
            this.SkypeID = theMemberData.SkypeID;
            this.TeamViewerID = theMemberData.TeamViewerID;

            // keep a hang on it so we don't try to, for example, give it a new guid
            if (!m_mapNewMembersThisSession.ContainsKey(MemberName))
                m_mapNewMembersThisSession.Add(MemberName, theMemberData);
        }

        private void buttonEditMember_Click(object sender, EventArgs e)
        {
            // act as if the user clicked the 'Edit Member Information' tab (which does what we want)
            tabControlProjectMetaData.SelectedTab = tabPageEditMember;
        }

        private void buttonDeleteMember_Click(object sender, EventArgs e)
        {
            // this is only enabled if we added the member this session
            System.Diagnostics.Debug.Assert(m_mapNewMembersThisSession.ContainsKey(SelectedMember) && _dataTeamMembers.ContainsKey(SelectedMember));

            _dataTeamMembers.Remove(SelectedMember);
            m_mapNewMembersThisSession.Remove(SelectedMember);
        }

        private void tabControlProjectMetaData_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageMemberList)
            {
                Console.WriteLine("tabPageMemberList Selected");

                // if the user made some changes and then is moving away from the tab, 
                //  then do an implicit Accept
                if (Modified && !String.IsNullOrEmpty(this.MemberName))
                    DoAccept();
            }
            else if (listBoxTeamMembers.SelectedIndex != -1)
            {
                Console.WriteLine(String.Format("tabPageEditMember Selected: editing: {0}", listBoxTeamMembers.SelectedItem));
                
                // behave just as if they'd clicked the Edit member button
                EditMemberInfo();
                textBoxName.Focus();
            }
            else
            {
                // adding a new member
                textBoxName.Focus();
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            Modified = true;
        }

        void radioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            Modified = true;
        }

        private void buttonVernacularFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textBoxVernacular.Font;
            fontDialog.Color = textBoxVernacular.ForeColor;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxVernacular.Font = textBoxVernacularEthCode.Font = fontDialog.Font;
                textBoxVernacular.ForeColor = textBoxVernacularEthCode.ForeColor = fontDialog.Color;
            }
        }

        private void buttonNationalBTFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textBoxNationalBTLanguage.Font;
            fontDialog.Color = textBoxNationalBTLanguage.ForeColor;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxNationalBTLanguage.Font = textBoxNationalBTEthCode.Font = fontDialog.Font;
                textBoxNationalBTLanguage.ForeColor = textBoxNationalBTEthCode.ForeColor = fontDialog.Color;
            }
        }

        private void buttonInternationalBTFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = m_projSettings.InternationalBTFont;
            fontDialog.Color = m_projSettings.InternationalBTFontColor;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                m_projSettings.InternationalBTFont = fontDialog.Font;
                m_projSettings.InternationalBTFontColor = fontDialog.Color;
            }
        }
    }
}
