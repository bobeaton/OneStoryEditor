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
    public partial class EditMemberForm : Form
    {
        public EditMemberForm(TeamMemberData theMemberData)
        {
            InitializeComponent();

            if (theMemberData == null) 
                return;

            textBoxName.Text = theMemberData.Name;
            MemberType = theMemberData.MemberType;
            textBoxEmail.Text = theMemberData.Email;
            textBoxPhoneNumber.Text = theMemberData.Phone;
            textBoxAltPhone.Text = theMemberData.AltPhone;
            textBoxSkypeID.Text = theMemberData.SkypeID;
            textBoxTeamViewer.Text = theMemberData.TeamViewerID;
            textBoxAddress.Text = theMemberData.Address;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(MemberName))
            {
                MessageBox.Show(
                    "You have to enter at least a name and indicate your role (even if you're 'just looking')!",
                    StoryEditor.CstrCaption);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
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
                if (radioButtonUNS.Checked)
                    return StoryEditor.UserTypes.eUNS;
                if (radioButtonConsultantInTraining.Checked)
                    return StoryEditor.UserTypes.eConsultantInTraining;
                if (radioButtonCoach.Checked)
                    return StoryEditor.UserTypes.eCoach;
                if (radioButtonJustViewing.Checked)
                    return StoryEditor.UserTypes.eJustLooking;
                return StoryEditor.UserTypes.eUndefined;
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
    }
}
