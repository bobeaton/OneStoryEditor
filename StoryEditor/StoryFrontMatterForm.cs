using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StoryFrontMatterForm : Form
    {
        protected StoryEditor _theSE;
        protected StoryProjectData _theStoryProjectData;
        protected StoryData _theCurrentStory;

        public StoryFrontMatterForm(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
        {
            _theSE = theSE;
            _theStoryProjectData = theStoryProjectData;
            _theCurrentStory = theCurrentStory;

            InitializeComponent();

            InitToolboxTextTip(theCurrentStory.CraftingInfo.ProjectFacilitatorMemberID,
                textBoxProjectFacilitator);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.StoryCrafterMemberID,
                textBoxStoryCrafter);

            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            
            InitToolboxTextTip(theCurrentStory.CraftingInfo.BackTranslatorMemberID,
                textBoxUnsBackTranslator);

            if (theCurrentStory.CraftingInfo.Testors.Count > 0)
            {
                InitToolboxTextTip(theCurrentStory.CraftingInfo.Testors[0],
                    textBoxUnsTest1);

                buttonBrowseUnsTest2.Enabled = true;
            }
            else
                buttonBrowseUnsTest2.Enabled = false;

            if (theCurrentStory.CraftingInfo.Testors.Count > 1)
            {
                InitToolboxTextTip(theCurrentStory.CraftingInfo.Testors[1],
                    textBoxUnsTest2);

                buttonBrowseUnsTest3.Enabled = true;
            }
            else
                buttonBrowseUnsTest3.Enabled = false;

            if (theCurrentStory.CraftingInfo.Testors.Count > 2)
                InitToolboxTextTip(theCurrentStory.CraftingInfo.Testors[2],
                    textBoxUnsTest3);

            Text = String.Format("Story Information for '{0}'", theCurrentStory.Name);
        }

        protected void InitToolboxTextTip(string strGuid, TextBox tb)
        {
            string strName = _theStoryProjectData.GetMemberNameFromMemberGuid(strGuid);
            if (!String.IsNullOrEmpty(strName))
                InitToolboxTextTip(_theStoryProjectData.TeamMembers[strName], tb);
        }

        protected void InitToolboxTextTip(TeamMemberData tmd, TextBox tb)
        {
            tb.Text = tmd.Name;
            toolTip.SetToolTip(tb, tmd.BioData);
        }

        private void buttonBrowserForProjectFacilitator_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxProjectFacilitator);
        }

        private void buttonBrowseForStoryCrafter_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxStoryCrafter);
        }

        protected void HandleMouseUp(bool bRightButton, TextBox textBox)
        {
            TeamMemberData theTeamMember = null;
            if (bRightButton)
            {
                MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eCrafter);
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;
                theTeamMember = dlg.SelectedMember;
            }
            else
            {
                string strName = textBox.Text;
                if (String.IsNullOrEmpty(strName))
                    return;

                theTeamMember = _theStoryProjectData.TeamMembers[strName];
                if (theTeamMember == null)
                    return;

                if (_theStoryProjectData.TeamMembers.ShowEditDialog(theTeamMember) != DialogResult.OK)
                    return;
            }

            textBox.Tag = theTeamMember;
            InitToolboxTextTip(theTeamMember, textBox);
        }

        protected TeamMemberData SelectedUnsMember()
        {
            MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eUNS);
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.SelectedMember;
            return null;
        }

        private void buttonBrowseUNSBackTranslator_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsBackTranslator);

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsBackTranslator.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsBackTranslator);
#endif
        }

        private void buttonBrowseUnsTest1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(false, textBoxUnsTest1);

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest1.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest1);
#endif
        }

        private void buttonBrowseUnsTest2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseUp(false, textBoxUnsTest2);

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest2.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest2);
#endif
        }

        private void buttonBrowseUnsTest3_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseUp(false, textBoxUnsTest3);

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest3.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest3);
#endif
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxProjectFacilitator.Tag != null)
            {
                TeamMemberData thePF = (TeamMemberData)textBoxProjectFacilitator.Tag;
                _theCurrentStory.CraftingInfo.ProjectFacilitatorMemberID = thePF.MemberGuid;
                _theSE.Modified = true;
            }

            if (textBoxStoryCrafter.Tag != null)
            {
                TeamMemberData theSC = (TeamMemberData) textBoxStoryCrafter.Tag;
                _theCurrentStory.CraftingInfo.StoryCrafterMemberID = theSC.MemberGuid;
                _theSE.Modified = true;                
            }

            if (_theCurrentStory.CraftingInfo.StoryPurpose != textBoxStoryPurpose.Text)
            {
                _theCurrentStory.CraftingInfo.StoryPurpose = textBoxStoryPurpose.Text;
                _theSE.Modified = true;
            }

            if (_theCurrentStory.CraftingInfo.ResourcesUsed != textBoxResourcesUsed.Text)
            {
                _theCurrentStory.CraftingInfo.ResourcesUsed = textBoxResourcesUsed.Text;
                _theSE.Modified = true;
            }

            if (textBoxUnsBackTranslator.Tag != null)
            {
                TeamMemberData theBT = (TeamMemberData)textBoxUnsBackTranslator.Tag;
                _theCurrentStory.CraftingInfo.BackTranslatorMemberID = theBT.MemberGuid;
                _theSE.Modified = true;
            }

            /* we don't allow changes to UNSs
            if (textBoxUnsTest1.Tag != null)
                AddOrInsertTestor(textBoxUnsTest1, 0);

            if (textBoxUnsTest2.Tag != null)
                AddOrInsertTestor(textBoxUnsTest2, 1);

            if (textBoxUnsTest3.Tag != null)
                AddOrInsertTestor(textBoxUnsTest3, 2);
            */

            DialogResult = DialogResult.OK;
            Close();
        }

        protected void AddOrInsertTestor(TextBox tb, int nIndex)
        {
            TeamMemberData theUns = (TeamMemberData)tb.Tag;
            if (_theCurrentStory.CraftingInfo.Testors.Count <= nIndex)
                _theCurrentStory.CraftingInfo.Testors.Add(theUns.MemberGuid);
            else
            {
                // but might means that they've changed the UNS, which means we have
                //  to update all the test values in the verses.

                _theCurrentStory.CraftingInfo.Testors[nIndex] = theUns.MemberGuid;
                System.Diagnostics.Debug.Assert(false);
            }
            _theSE.Modified = true;
        }
    }
}
