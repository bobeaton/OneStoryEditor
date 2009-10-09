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
            
            textBoxStoryCrafter.Text =
                theStoryProjectData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.StoryCrafterMemberID);
            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            textBoxUnsBackTranslator.Text =
                theStoryProjectData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.BackTranslatorMemberID);
            if (theCurrentStory.CraftingInfo.Testors.Count > 0)
            {
                textBoxUnsTest1.Text = theStoryProjectData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.Testors[0]);
                buttonBrowseUnsTest2.Enabled = true;
            }
            else
                buttonBrowseUnsTest2.Enabled = false;

            if (theCurrentStory.CraftingInfo.Testors.Count > 1)
            {
                textBoxUnsTest2.Text = theStoryProjectData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.Testors[1]);
                buttonBrowseUnsTest3.Enabled = true;
            }
            else
                buttonBrowseUnsTest3.Enabled = false;

            if (theCurrentStory.CraftingInfo.Testors.Count > 2)
                textBoxUnsTest3.Text = theStoryProjectData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.Testors[2]);

            Text = String.Format("Story Information for '{0}'", theCurrentStory.Name);
        }

        private void buttonBrowseForStoryCrafter_Click(object sender, EventArgs e)
        {
            MemberPicker dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.eCrafter);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxStoryCrafter.Tag = dlg.SelectedMember;
                textBoxStoryCrafter.Text = dlg.SelectedMember.Name;
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
                textBoxUnsBackTranslator.Text = aUns.Name;
        }

        private void buttonBrowseUnsTest1_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest1.Tag = aUns;
            if (aUns != null)
                textBoxUnsTest1.Text = aUns.Name;
        }

        private void buttonBrowseUnsTest2_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest2.Tag = aUns;
            if (aUns != null)
                textBoxUnsTest2.Text = aUns.Name;
        }

        private void buttonBrowseUnsTest3_Click(object sender, EventArgs e)
        {
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest3.Tag = aUns;
            if (aUns != null)
                textBoxUnsTest3.Text = aUns.Name;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
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
            if (_theCurrentStory.CraftingInfo.Testors.Count < nIndex)
                _theCurrentStory.CraftingInfo.Testors.Add(theUns.MemberGuid);
            else
                _theCurrentStory.CraftingInfo.Testors[nIndex] = theUns.MemberGuid;
            _theSE.Modified = true;
        }
    }
}
