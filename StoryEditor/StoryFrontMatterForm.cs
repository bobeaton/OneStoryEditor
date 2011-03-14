using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StoryFrontMatterForm : TopForm
    {
        protected StoryEditor _theSE;
        protected StoryProjectData _theStoryProjectData;
        protected StoryData _theCurrentStory;

        public StoryFrontMatterForm(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
            : base(true)
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

            if (theCurrentStory.CraftingInfo.TestorsToCommentsRetellings.Count > 0)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsRetellings[0];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsRetellingTest1);
                textBoxRetellingComment1.Text = ti.TestComment;
                buttonBrowseUnsRetellingTest1.Enabled = true;
            }
            else
                textBoxRetellingComment1.Enabled = buttonBrowseUnsRetellingTest1.Enabled = false;

            if (theCurrentStory.CraftingInfo.TestorsToCommentsRetellings.Count > 1)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsRetellings[1];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsRetellingTest2);
                textBoxRetellingComment2.Text = ti.TestComment;
                buttonBrowseUnsRetellingTest2.Enabled = true;
            }
            else
                textBoxRetellingComment2.Enabled = buttonBrowseUnsRetellingTest2.Enabled = false;

            if (theCurrentStory.CraftingInfo.TestorsToCommentsRetellings.Count > 2)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsRetellings[2];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsRetellingTest3);
                textBoxRetellingComment3.Text = ti.TestComment;
                buttonBrowseUnsRetellingTest3.Enabled = true;
            }
            else
                textBoxRetellingComment3.Enabled = buttonBrowseUnsRetellingTest3.Enabled = false;

            if (theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers.Count > 0)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers[0];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsInferenceTest1);
                textBoxInferenceComment1.Text = ti.TestComment;
                buttonBrowseUnsInferenceTest1.Enabled = true;
            }
            else
                textBoxInferenceComment1.Enabled = buttonBrowseUnsInferenceTest1.Enabled = false;

            if (theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers.Count > 1)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers[1];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsInferenceTest2);
                textBoxInferenceComment2.Text = ti.TestComment;
                buttonBrowseUnsInferenceTest2.Enabled = true;
            }
            else
                textBoxInferenceComment2.Enabled = buttonBrowseUnsInferenceTest2.Enabled = false;

            if (theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers.Count > 2)
            {
                var ti = theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers[2];
                InitToolboxTextTip(ti.TestorGuid, textBoxUnsInferenceTest3);
                textBoxInferenceComment3.Text = ti.TestComment;
                buttonBrowseUnsInferenceTest3.Enabled = true;
            }
            else
                textBoxInferenceComment3.Enabled = buttonBrowseUnsInferenceTest3.Enabled = false;

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
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxProjectFacilitator,
                TeamMemberData.UserTypes.eProjectFacilitator, 
                "Choose the Project Facilitator for this story");
        }

        private void buttonBrowseForStoryCrafter_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxStoryCrafter,
                TeamMemberData.UserTypes.eCrafter,
                "Choose the crafter for this story");
        }

        protected void HandleMouseUp(bool bRightButton, TextBox textBox,
            TeamMemberData.UserTypes eType, string strPickerTitleDescription)
        {
            TeamMemberData theTeamMember;
            if (bRightButton)
            {
                var dlg = new MemberPicker(_theStoryProjectData, eType)
                {
                    Text = strPickerTitleDescription
                };
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

                if (_theStoryProjectData.TeamMembers.ShowEditDialog(theTeamMember, _theStoryProjectData.ProjSettings) != DialogResult.Yes)
                    return;
            }

            textBox.Tag = theTeamMember;
            InitToolboxTextTip(theTeamMember, textBox);
            _theSE.Modified = true;
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
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsBackTranslator,
                TeamMemberData.UserTypes.eUNS,
                "Choose the back-translator for this story");

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsBackTranslator.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsBackTranslator);
#endif
        }

        private void buttonBrowseUnsRetellingTest1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsRetellingTest1, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest1.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest1);
#endif
        }

        private void buttonBrowseUnsRetellingTest2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsRetellingTest2, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest2.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest2);
#endif
        }

        private void buttonBrowseUnsRetellingTest3_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsRetellingTest3, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");

#if false   // obsolete (I think)
            TeamMemberData aUns = SelectedUnsMember();
            textBoxUnsTest3.Tag = aUns;
            if (aUns != null)
                InitToolboxTextTip(aUns, textBoxUnsTest3);
#endif
        }

        private void buttonBrowseUnsInferenceTest1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsInferenceTest1, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");
        }

        private void buttonBrowseUnsInferenceTest2_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsInferenceTest2, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");
        }

        private void buttonBrowseUnsInferenceTest3_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsInferenceTest3, TeamMemberData.UserTypes.eUNS,
                "Choose the UNS that took this test");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool bModified = false;

            if (textBoxProjectFacilitator.Tag != null)
            {
                TeamMemberData thePF = (TeamMemberData)textBoxProjectFacilitator.Tag;
                _theCurrentStory.CraftingInfo.ProjectFacilitatorMemberID = thePF.MemberGuid;
                bModified = true;
            }

            if (textBoxStoryCrafter.Tag != null)
            {
                TeamMemberData theSC = (TeamMemberData) textBoxStoryCrafter.Tag;
                _theCurrentStory.CraftingInfo.StoryCrafterMemberID = theSC.MemberGuid;
                bModified = true;                
            }

            if (_theCurrentStory.CraftingInfo.StoryPurpose != textBoxStoryPurpose.Text)
            {
                _theCurrentStory.CraftingInfo.StoryPurpose = textBoxStoryPurpose.Text;
                bModified = true;
            }

            if (_theCurrentStory.CraftingInfo.ResourcesUsed != textBoxResourcesUsed.Text)
            {
                _theCurrentStory.CraftingInfo.ResourcesUsed = textBoxResourcesUsed.Text;
                bModified = true;
            }

            if (textBoxUnsBackTranslator.Tag != null)
            {
                TeamMemberData theBT = (TeamMemberData)textBoxUnsBackTranslator.Tag;
                _theCurrentStory.CraftingInfo.BackTranslatorMemberID = theBT.MemberGuid;
                bModified = true;
            }

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment1, 0, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment2, 1, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment3, 2, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment1, 0, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment2, 1, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment3, 2, ref bModified);

            if (textBoxUnsRetellingTest1.Tag != null)
                ChangeRetellingTestor(textBoxUnsRetellingTest1, 0, ref bModified);

            if (textBoxUnsRetellingTest2.Tag != null)
                ChangeRetellingTestor(textBoxUnsRetellingTest2, 1, ref bModified);

            if (textBoxUnsRetellingTest3.Tag != null)
                ChangeRetellingTestor(textBoxUnsRetellingTest3, 2, ref bModified);

            if (textBoxUnsInferenceTest1.Tag != null)
                ChangeQuestionTestor(textBoxUnsInferenceTest1, 0, ref bModified);

            if (textBoxUnsInferenceTest2.Tag != null)
                ChangeQuestionTestor(textBoxUnsInferenceTest2, 1, ref bModified);

            if (textBoxUnsInferenceTest3.Tag != null)
                ChangeQuestionTestor(textBoxUnsInferenceTest3, 2, ref bModified);

            if (bModified)
                _theSE.Modified = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private static void GetChangeToComment(TestInfo testInfo, TextBox tb, int nIndex,
            ref bool bModified)
        {
            if (testInfo.Count > nIndex)
            {
                if (tb.Text != testInfo[nIndex].TestComment)
                {
                    testInfo[nIndex].TestComment = tb.Text;
                    bModified = true;
                }
            }
        }

        protected void ChangeRetellingTestor(TextBox tb, int nIndex, ref bool bModified)
        {
            var theUns = (TeamMemberData)tb.Tag;
            _theCurrentStory.ChangeRetellingTestor(nIndex, theUns.MemberGuid);
            bModified = true;
        }

        protected void ChangeQuestionTestor(TextBox tb, int nIndex, ref bool bModified)
        {
            var theUns = (TeamMemberData)tb.Tag;
            _theCurrentStory.ChangeTqAnswersTestor(nIndex, theUns.MemberGuid);
            bModified = true;
        }
    }
}
