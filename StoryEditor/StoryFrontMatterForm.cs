using System;
using System.Collections.Generic;
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

            InitToolboxTextTip(theCurrentStory.CraftingInfo.ProjectFacilitator,
                               textBoxProjectFacilitator, 
                               textBoxCommentProjectFacilitator,
                               buttonBrowserForProjectFacilitator);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.Consultant,
                               textBoxConsultant,
                               textBoxCommentConsultant,
                               buttonBrowserForConsultant);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.Coach,
                               textBoxCoach,
                               textBoxCommentCoach,
                               buttonBrowserForCoach);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.StoryCrafter,
                               textBoxStoryCrafter, 
                               textBoxCommentStoryCrafter,
                               buttonBrowseForStoryCrafter);

            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            textBoxMiscNotes.Text = theCurrentStory.CraftingInfo.MiscellaneousStoryInfo;

            InitToolboxTextTip(theCurrentStory.CraftingInfo.BackTranslator,
                               textBoxUnsBackTranslator, 
                               textBoxCommentUnsBackTranslator,
                               buttonBrowseUNSBackTranslator);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsRetellings, 0,
                       labelUnsRetellingTest1, textBoxUnsRetellingTest1,
                       textBoxRetellingComment1, buttonBrowseUnsRetellingTest1);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsRetellings, 1,
                       labelUnsRetellingTest2, textBoxUnsRetellingTest2,
                       textBoxRetellingComment2, buttonBrowseUnsRetellingTest2);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsRetellings, 2,
                       labelUnsRetellingTest3, textBoxUnsRetellingTest3,
                       textBoxRetellingComment3, buttonBrowseUnsRetellingTest3);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsRetellings, 3,
                       labelUnsRetellingTest4, textBoxUnsRetellingTest4,
                       textBoxRetellingComment4, buttonBrowseUnsRetellingTest4);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers, 0,
                       labelInferenceTest1, textBoxUnsInferenceTest1,
                       textBoxInferenceComment1, buttonBrowseUnsInferenceTest1);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers, 1,
                       labelInferenceTest2, textBoxUnsInferenceTest2,
                       textBoxInferenceComment2, buttonBrowseUnsInferenceTest2);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers, 2,
                       labelInferenceTest3, textBoxUnsInferenceTest3,
                       textBoxInferenceComment3, buttonBrowseUnsInferenceTest3);

            InitTestor(theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers, 3,
                       labelInferenceTest4, textBoxUnsInferenceTest4,
                       textBoxInferenceComment4, buttonBrowseUnsInferenceTest4);

            Text = String.Format("Story Information for '{0}'", theCurrentStory.Name);
        }

        private void InitTestor(TestInfo tests, int nIndex, Label lbl, TextBox tb, 
            TextBox tbComment, Button btnBrowse)
        {
            if (tests.Count > nIndex)
            {
                var ti = tests[nIndex];
                InitToolboxTextTip(ti, tb, tbComment, btnBrowse);
            }
            else
                lbl.Visible = tb.Visible = tbComment.Visible = btnBrowse.Visible = false;
        }

        protected void InitToolboxTextTip(MemberIdInfo memberInfo, TextBox tbName, 
            TextBox tbComment, Button btnBrowse)
        {
            var member = _theStoryProjectData.GetMemberFromId(memberInfo.MemberId);
            if (member != null)
                InitToolboxTextTip(member, tbName);
            tbComment.Text = memberInfo.MemberComment;
            toolTip.SetToolTip(btnBrowse, Properties.Resources.IDS_StoryInformationBrowseButtonTooltip);
        }

        protected void InitToolboxTextTip(TeamMemberData tmd, TextBox tbName)
        {
            tbName.Text = tmd.Name;
            toolTip.SetToolTip(tbName, tmd.BioData);
        }

        private void buttonBrowserForProjectFacilitator_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxProjectFacilitator,
                          TeamMemberData.UserTypes.ProjectFacilitator,
                          "Choose the Project Facilitator for this story");
        }

        private void buttonBrowserForConsultant_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxConsultant,
                          TeamMemberData.UserTypes.ConsultantInTraining |
                          TeamMemberData.UserTypes.IndependentConsultant,
                          "Choose the Consultant/CIT for this story");
        }

        private void buttonBrowserForCoach_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxCoach,
                          TeamMemberData.UserTypes.Coach,
                          "Choose the Coach for this story");
        }

        private void buttonBrowseForStoryCrafter_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxStoryCrafter,
                TeamMemberData.UserTypes.Crafter,
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
            var dlg = new MemberPicker(_theStoryProjectData, TeamMemberData.UserTypes.UNS);
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.SelectedMember;
            return null;
        }

        private void buttonBrowseUNSBackTranslator_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsBackTranslator,
                TeamMemberData.UserTypes.UNS,
                "Choose the back-translator for this story");
        }

        private void HandleUnsTestMouseUp(TextBox tb, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, tb,
                          TeamMemberData.UserTypes.UNS,
                          "Choose the UNS that took this test");
        }

        private void buttonBrowseUnsRetellingTest1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsRetellingTest1, e);
        }

        private void buttonBrowseUnsRetellingTest2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsRetellingTest2, e);
        }

        private void buttonBrowseUnsRetellingTest3_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsRetellingTest3, e);
        }

        private void buttonBrowseUnsRetellingTest4_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsRetellingTest4, e);
        }

        private void buttonBrowseUnsInferenceTest1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsInferenceTest1, e);
        }

        private void buttonBrowseUnsInferenceTest2_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsInferenceTest2, e);
        }

        private void buttonBrowseUnsInferenceTest3_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsInferenceTest3, e);
        }

        private void buttonBrowseUnsInferenceTest4_MouseUp(object sender, MouseEventArgs e)
        {
            HandleUnsTestMouseUp(textBoxUnsInferenceTest4, e);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool bModified = false;

            if (textBoxProjectFacilitator.Tag != null)
            {
                var thePF = (TeamMemberData)textBoxProjectFacilitator.Tag;
                _theCurrentStory.ReplaceProjectFacilitator(thePF.MemberGuid);
                bModified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.ProjectFacilitator,
                               textBoxCommentProjectFacilitator, ref bModified);

            if (textBoxConsultant.Tag != null)
            {
                var theCons = (TeamMemberData)textBoxConsultant.Tag;
                _theCurrentStory.ReplaceConsultant(theCons.MemberGuid);
                bModified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.Consultant,
                               textBoxCommentConsultant, ref bModified);

            if (textBoxCoach.Tag != null)
            {
                var theCch = (TeamMemberData)textBoxCoach.Tag;
                _theCurrentStory.ReplaceCoach(theCch.MemberGuid);
                bModified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.Coach,
                               textBoxCommentCoach, ref bModified);

            if (textBoxStoryCrafter.Tag != null)
            {
                var theSC = (TeamMemberData) textBoxStoryCrafter.Tag;
                _theCurrentStory.CraftingInfo.StoryCrafter.MemberId = theSC.MemberGuid;
                bModified = true;                
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.StoryCrafter,
                               textBoxCommentStoryCrafter, ref bModified);

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

            if (_theCurrentStory.CraftingInfo.MiscellaneousStoryInfo != textBoxMiscNotes.Text)
            {
                _theCurrentStory.CraftingInfo.MiscellaneousStoryInfo = textBoxMiscNotes.Text;
                bModified = true;
            }

            if (textBoxUnsBackTranslator.Tag != null)
            {
                var theBT = (TeamMemberData)textBoxUnsBackTranslator.Tag;
                _theCurrentStory.CraftingInfo.BackTranslator.MemberId = theBT.MemberGuid;
                bModified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.BackTranslator,
                               textBoxCommentUnsBackTranslator, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment1, 0, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment2, 1, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment3, 2, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                textBoxRetellingComment4, 3, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment1, 0, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment2, 1, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment3, 2, ref bModified);

            GetChangeToComment(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                textBoxInferenceComment4, 3, ref bModified);

            // here's a scenario to avoid: the user is changing UNS#2 to be UNS#1 and
            //  UNS#2 to be someone else (or accidentally leaving USE#2 as both).
            // Because of the way the change is done (by re-writing the guids), we
            //  have to make sure we don't change them in the wrong order--i.e. the 
            //  "swap" problem (a->c, b->a, and c->b)
            // suppose: Before          After
            //          #1 = f4450160   #1 = 68b690f6
            //          #2 = 68b690f6   #2 = 214f9152
            // then 
            var lstUnsGuids = new List<string>();
            try
            {
                // this checks to make sure we don't have the same UNS for multiple tests
                CheckForDuplicate(textBoxUnsRetellingTest1, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsRetellingTest2, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsRetellingTest3, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsRetellingTest4, ref lstUnsGuids);
                lstUnsGuids.Clear();
                CheckForDuplicate(textBoxUnsInferenceTest1, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsInferenceTest2, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsInferenceTest3, ref lstUnsGuids);
                CheckForDuplicate(textBoxUnsInferenceTest4, ref lstUnsGuids);

                var testInfoRetellings = new TestInfo();
                ChangeRetellingTestor(textBoxUnsRetellingTest1, 0, ref testInfoRetellings, ref bModified);
                ChangeRetellingTestor(textBoxUnsRetellingTest2, 1, ref testInfoRetellings, ref bModified);
                ChangeRetellingTestor(textBoxUnsRetellingTest3, 2, ref testInfoRetellings, ref bModified);
                ChangeRetellingTestor(textBoxUnsRetellingTest4, 3, ref testInfoRetellings, ref bModified);

                var testInfoInferenceTests = new TestInfo();
                ChangeQuestionTestor(textBoxUnsInferenceTest1, 0, ref testInfoInferenceTests, ref bModified);
                ChangeQuestionTestor(textBoxUnsInferenceTest2, 1, ref testInfoInferenceTests, ref bModified);
                ChangeQuestionTestor(textBoxUnsInferenceTest3, 2, ref testInfoInferenceTests, ref bModified);
                ChangeQuestionTestor(textBoxUnsInferenceTest4, 3, ref testInfoInferenceTests, ref bModified);

                // now that we've checked and verified everything, then update the stored info
                _theCurrentStory.CraftingInfo.TestorsToCommentsRetellings = testInfoRetellings;
                _theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers = testInfoInferenceTests;
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }

            if (bModified)
                _theSE.Modified = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CheckForDuplicate(TextBox textBox, ref List<string> lstUnsGuids)
        {
            if (String.IsNullOrEmpty(textBox.Text))
                return;

            var theUns = _theStoryProjectData.TeamMembers[textBox.Text];
            if (lstUnsGuids.Contains(theUns.MemberGuid))
                throw new ApplicationException(String.Format(Properties.Resources.IDS_AddTestSameUNS,
                                                             _theStoryProjectData.GetMemberNameFromMemberGuid(
                                                                 theUns.MemberGuid)));
            lstUnsGuids.Add(theUns.MemberGuid);
        }

        private static void GetChangeToComment(TestInfo testInfo, TextBox tb, int nIndex,
            ref bool bModified)
        {
            if (testInfo.Count > nIndex)
            {
                var testorInfo = testInfo[nIndex];
                GetChangeToComment(testorInfo, tb, ref bModified);
            }
        }

        private static void GetChangeToComment(MemberIdInfo testorInfo, TextBox tb, 
            ref bool bModified)
        {
            if (tb.Text != testorInfo.MemberComment)
            {
                testorInfo.MemberComment = tb.Text;
                bModified = true;
            }
        }

        protected void ChangeRetellingTestor(TextBox tb, int nIndex,
            ref TestInfo testInfoNew, ref bool bModified)
        {
            TeamMemberData theUns;
            if (ChangeTestor(tb, nIndex,
                             _theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                             out theUns, ref bModified))
            {
                _theCurrentStory.ChangeRetellingTestor(nIndex, theUns.MemberGuid, ref testInfoNew);
            }
        }

        protected void ChangeQuestionTestor(TextBox tb, int nIndex,
            ref TestInfo testInfoNew, ref bool bModified)
        {
            TeamMemberData theUns;
            if (ChangeTestor(tb, nIndex,
                             _theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                             out theUns, ref bModified))
            {
                _theCurrentStory.ChangeTqAnswersTestor(nIndex, theUns.MemberGuid, ref testInfoNew);
            }
        }

        private bool ChangeTestor(TextBox tb, int nIndex, TestInfo testInfoCurrent,
            out TeamMemberData theUns, ref bool bModified)
        {
            if (tb.Tag != null)
            {
                theUns = (TeamMemberData)tb.Tag;
                bModified = true;
            }
            else if (testInfoCurrent.Count > nIndex)
            {
                var testorInfo = testInfoCurrent[nIndex];
                theUns = _theStoryProjectData.GetMemberFromId(testorInfo.MemberId);
            }
            else
            {
                theUns = null;
                return false;
            }

            return true;
        }
    }
}
