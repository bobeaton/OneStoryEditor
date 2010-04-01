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

        private void buttonBrowserForProjectFacilitator_Click(object sender, EventArgs e)
        {
            MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eProjectFacilitator);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxProjectFacilitator.Tag = dlg.SelectedMember;
                InitToolboxTextTip(dlg.SelectedMember, textBoxProjectFacilitator);
            }
        }

        private void buttonBrowseForStoryCrafter_Click(object sender, EventArgs e)
        {
            MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eCrafter);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxStoryCrafter.Tag = dlg.SelectedMember;
                InitToolboxTextTip(dlg.SelectedMember, textBoxStoryCrafter);
            }
        }

        protected TeamMemberData SelectedUnsMember()
        {
            MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eUNS);
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.SelectedMember;
            return null;
        }

        private void buttonBrowseUNSBackTranslator_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsBackTranslator.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsBackTranslator);
        }

        private void buttonBrowseUnsTest1_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest1.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest1);
        }

        private void buttonBrowseUnsTest2_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest2.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest2);
        }

        private void buttonBrowseUnsTest3_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest3.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest3);
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

            if (textBoxUnsTest1.Tag != null)
                AddOrInsertTestor(textBoxUnsTest1, 0);

            if (textBoxUnsTest2.Tag != null)
                AddOrInsertTestor(textBoxUnsTest2, 1);

            if (textBoxUnsTest3.Tag != null)
                AddOrInsertTestor(textBoxUnsTest3, 2);

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
