using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StoryFrontMatterForm : Form
    {
        protected StoryEditor _theSE = null;
        protected StoriesData _theStoriesData = null;
        protected StoryData _theCurrentStory = null;

        public StoryFrontMatterForm(StoryEditor theSE, StoriesData theStoriesData, StoryData theCurrentStory)
        {
            _theSE = theSE;
            _theStoriesData = theStoriesData;
            _theCurrentStory = theCurrentStory;

            InitializeComponent();
            
            textBoxStoryCrafter.Text =
                theStoriesData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.StoryCrafterMemberID);
            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            textBoxUnsBackTranslator.Text =
                theStoriesData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.BackTranslatorMemberID);
            if (theCurrentStory.CraftingInfo.Testors.Count > 0)
                textBoxUnsTest1.Text = theStoriesData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.Testors[1]);
            if (theCurrentStory.CraftingInfo.Testors.Count > 1)
                textBoxUnsTest2.Text = theStoriesData.GetMemberNameFromMemberGuid(theCurrentStory.CraftingInfo.Testors[2]);

            Text = String.Format("Story Information for '{0}'", theCurrentStory.Name);
        }

        private void buttonBrowseForStoryCrafter_Click(object sender, EventArgs e)
        {
            MemberPicker dlg = new MemberPicker(_theStoriesData, TeamMemberData.UserTypes.eCrafter);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxStoryCrafter.Tag = dlg.SelectedMember;
                textBoxStoryCrafter.Text = dlg.SelectedMember.Name;
            }
        }

        protected TeamMemberData SelectedUnsMember()
        {
            MemberPicker dlg = new MemberPicker(_theStoriesData, TeamMemberData.UserTypes.eUNS);
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
            {
                TeamMemberData theUns = (TeamMemberData) textBoxUnsTest1.Tag;
                if (_theCurrentStory.CraftingInfo.Testors.ContainsKey(1))
                    _theCurrentStory.CraftingInfo.Testors[1] = theUns.MemberGuid;
                else 
                    _theCurrentStory.CraftingInfo.Testors.Add(1, theUns.MemberGuid);
                _theSE.Modified = true;
            }

            if (textBoxUnsTest2.Tag != null)
            {
                TeamMemberData theUns = (TeamMemberData) textBoxUnsTest2.Tag;
                if (_theCurrentStory.CraftingInfo.Testors.ContainsKey(2))
                    _theCurrentStory.CraftingInfo.Testors[2] = theUns.MemberGuid;
                else
                    _theCurrentStory.CraftingInfo.Testors.Add(2, theUns.MemberGuid);
                _theSE.Modified = true;
            }
            
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
