using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StoryFrontMatterForm : TopForm
    {
        protected StoryEditor _theSE;
        protected StoryProjectData _theStoryProjectData;
        protected StoryData _theCurrentStory;
        private readonly TestRows rowsRetellings = new TestRows();
        private readonly TestRows rowsAnswers = new TestRows();

        public StoryFrontMatterForm(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
            : base(true)
        {
            _theSE = theSE;
            _theStoryProjectData = theStoryProjectData;
            _theCurrentStory = theCurrentStory;

            InitializeComponent();

            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            textBoxMiscNotes.Text = theCurrentStory.CraftingInfo.MiscellaneousStoryInfo;

            InitToolboxTextTip(theCurrentStory.CraftingInfo.ProjectFacilitator,
                               labelProjectFacilitator,
                               textBoxProjectFacilitator,
                               textBoxCommentProjectFacilitator,
                               buttonBrowserForProjectFacilitator);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.Consultant,
                               labelConsultant,
                               textBoxConsultant,
                               textBoxCommentConsultant,
                               buttonBrowserForConsultant);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.Coach,
                               labelCoach,
                               textBoxCoach,
                               textBoxCommentCoach,
                               buttonBrowserForCoach);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.StoryCrafter,
                               labelStoryCrafter,
                               textBoxStoryCrafter,
                               textBoxCommentStoryCrafter,
                               buttonBrowseForStoryCrafter);

            InitToolboxTextTip(theCurrentStory.CraftingInfo.BackTranslator,
                               labelBackTranslator,
                               textBoxUnsBackTranslator,
                               textBoxCommentUnsBackTranslator,
                               buttonBrowseUNSBackTranslator);

            // initialize a callback pointer for when mouses click up
            ControlRow.MyParent = this;

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            InitializeTestRows(theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                rowsRetellings,
                "Retelling Test &");

            InitializeTestRows(theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                rowsAnswers,
                "Story Question Test &");

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);

            Text = String.Format("Story Information for '{0}'", theCurrentStory.Name);
        }

        private void InitializeTestRows(TestInfo testInfo, TestRows rows, 
            string cstrLabel)
        {
            for (int i = 1; i <= testInfo.Count; i++)
            {
                var row = new ControlRow(cstrLabel + i);
                var testor = testInfo[i - 1];
                InitTestorRow(row, testor);
                rows.Add(row);
            }
        }

        private void InitTestorRow(ControlRow row, MemberIdInfo testor)
        {
            // add the row of controls to the table layout panel
            int nRow = tableLayoutPanel.RowCount;
            tableLayoutPanel.Controls.Add(row.Label, 0, nRow);
            tableLayoutPanel.Controls.Add(row.TbxName, 1, nRow);
            tableLayoutPanel.Controls.Add(row.TbxComment, 2, nRow);
            tableLayoutPanel.Controls.Add(row.BtnBrowse, 3, nRow);
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowCount++;
            
            // now initialize it with data
            var member = _theStoryProjectData.GetMemberFromId(testor.MemberId);
            if (member != null)
                InitToolboxTextTip(member, row.TbxName);
            row.TbxComment.Text = testor.MemberComment;
            toolTip.SetToolTip(row.BtnBrowse, Properties.Resources.IDS_StoryInformationBrowseButtonTooltip);
        }

        private void InitToolboxTextTip(MemberIdInfo memberInfo, Label lbl, 
            TextBox tbName, TextBox tbComment, Button btnBrowse)
        {
            if (MemberIdInfo.Configured(memberInfo))
            {
                var member = _theStoryProjectData.GetMemberFromId(memberInfo.MemberId);
                if (member != null)
                    InitToolboxTextTip(member, tbName);
                tbComment.Text = memberInfo.MemberComment;
                toolTip.SetToolTip(btnBrowse, Properties.Resources.IDS_StoryInformationBrowseButtonTooltip);
            }
            else
                lbl.Visible = tbName.Visible = tbComment.Visible = btnBrowse.Visible = false;
        }

        private void InitToolboxTextTip(TeamMemberData tmd, TextBox tbName)
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

        private void buttonBrowseUNSBackTranslator_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, textBoxUnsBackTranslator,
                TeamMemberData.UserTypes.UNS,
                "Choose the back-translator for this story");
        }

        public void HandleUnsTestMouseUp(TextBox tb, MouseEventArgs e)
        {
            HandleMouseUp(e.Button == MouseButtons.Right, tb,
                          TeamMemberData.UserTypes.UNS,
                          "Choose the UNS that took this test");
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

            ProcessTestCommentChanges(_theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                rowsRetellings, ref bModified);

            ProcessTestCommentChanges(_theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                rowsAnswers, ref bModified);

            try
            {
                rowsRetellings.CheckForDuplicateUns();
                rowsAnswers.CheckForDuplicateUns();

                _theCurrentStory.CraftingInfo.TestorsToCommentsRetellings =
                    rowsRetellings.ChangeTestors(
                        _theCurrentStory.CraftingInfo.TestorsToCommentsRetellings,
                        _theCurrentStory.ChangeRetellingTestor,
                        _theCurrentStory.Verses.ChangeRetellingTestorGuid,
                        _theStoryProjectData.TeamMembers,
                        ref bModified);

                _theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers =
                    rowsAnswers.ChangeTestors(
                        _theCurrentStory.CraftingInfo.TestorsToCommentsTqAnswers,
                        _theCurrentStory.ChangeTqAnswersTestor,
                        _theCurrentStory.Verses.ChangeTqAnswersTestorGuid,
                        _theStoryProjectData.TeamMembers,
                        ref bModified);
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

        private static void ProcessTestCommentChanges(TestInfo testInfo, 
            TestRows lstRows, ref bool bModified)
        {
            for (int i = 0; i < lstRows.Count; i++)
            {
                var memberIdInfo = testInfo[i];
                var testRow = lstRows[i];
                GetChangeToComment(memberIdInfo, testRow.TbxComment, ref bModified);
            }
        }
        
        private static void GetChangeToComment(MemberIdInfo testorInfo, TextBox tb, 
            ref bool bModified)
        {
            if (MemberIdInfo.Configured(testorInfo) && 
                (tb.Text != testorInfo.MemberComment))
            {
                testorInfo.MemberComment = tb.Text;
                bModified = true;
            }
        }
    }

    public class TestRows : List<ControlRow>
    {
        public void CheckForDuplicateUns()
        {
            var lstUnsGuids = new List<string>();
            foreach (var aRow in this.Where(aRow => 
                !String.IsNullOrEmpty(aRow.TbxName.Text)))
            {
                if (lstUnsGuids.Contains(aRow.TbxName.Text))
                    throw new ApplicationException(String.Format(Properties.Resources.IDS_AddTestSameUNS,
                                                                 aRow.TbxName.Text));
                lstUnsGuids.Add(aRow.TbxName.Text);
            }
        }

        public delegate void ChangeTestorMethod(TeamMembersData teamMembersData,
            int nIndex, string strNewGuid, ref TestInfo testInfoNew);
        public delegate void ChangeTestorGuid(string strOldGuid, string strNewGuid);

        // here's a scenario to avoid: the user is changing UNS#2 to be UNS#1 and
        //  UNS#2 to be someone else (or accidentally leaving USE#2 as both).
        // Because of the way the change is done (by re-writing the guids), we
        //  have to make sure we don't change them in the wrong order--i.e. the 
        //  "swap" problem (a->c, b->a, and c->b)
        // suppose: Before          After
        //          #1 = f4450160   #1 = 68b690f6
        //          #2 = 68b690f6   #2 = 214f9152
        // or worse
        //          #2 = 68b690f6   #2 = f4450160
        // so if the person is already there in another test, we'll use his *name* as
        //  the guid temporarily and then afterwards will "Adjust" those back to the guid
        public TestInfo ChangeTestors(TestInfo testInfo, ChangeTestorMethod changeMethod,
            ChangeTestorGuid changeTestorGuid, TeamMembersData teamMembersData, 
            ref bool bModified)
        {
            var testInfoNew = new TestInfo();
            for (int i = 0; i < Count; i++)
            {
                var aRow = this[i];
                ChangeTestor(aRow.TbxName, i, testInfo, changeMethod, teamMembersData,
                             ref testInfoNew, ref bModified);
            }
            return AdjustForSwap(testInfoNew, teamMembersData, changeTestorGuid);
        }

        private static TestInfo AdjustForSwap(TestInfo testInfo, 
            TeamMembersData teamMembersData, ChangeTestorGuid changeTestorGuid)
        {
            foreach (var aTestor in testInfo)
                if (teamMembersData.ContainsKey(aTestor.MemberId))
                {
                    // this means we had temporarily used the member's *name* as their
                    //  guid (in the case where the person was already in the list, 
                    //  we couldn't just change the guid due to the 'swap' problem--
                    //  see StoryData.ChangeTestor for details)
                    var member = teamMembersData[aTestor.MemberId];
                    changeTestorGuid(aTestor.MemberId, member.MemberGuid);
                    aTestor.MemberId = member.MemberGuid;
                }
            return testInfo;
        }

        private static void ChangeTestor(TextBox tb, int nIndex, TestInfo testInfo,
            ChangeTestorMethod changeMethod, TeamMembersData teamMembersData, 
            ref TestInfo testInfoNew, ref bool bModified)
        {
            TeamMemberData theNewUns;
            if (ChangeTestor(tb, nIndex, testInfo, teamMembersData, out theNewUns, 
                ref bModified))
            {
                changeMethod(teamMembersData, nIndex, theNewUns.MemberGuid, 
                    ref testInfoNew);
            }
        }

        private static bool ChangeTestor(TextBox tb, int nIndex, TestInfo testInfoCurrent,
            TeamMembersData teamMembersData, out TeamMemberData theNewUns, 
            ref bool bModified)
        {
            if (tb.Tag != null)
            {
                theNewUns = (TeamMemberData)tb.Tag;
                bModified = true;
            }
            else if (testInfoCurrent.Count > nIndex)
            {
                var testorInfo = testInfoCurrent[nIndex];
                theNewUns = teamMembersData.GetMemberFromId(testorInfo.MemberId);
            }
            else
            {
                theNewUns = null;
                return false;
            }

            return true;
        }
    }

    public class ControlRow
    {
        public static StoryFrontMatterForm MyParent;

        public Label Label { get; set; }
        public TextBox TbxName { get; set; }
        public TextBox TbxComment { get; set; }
        public Button BtnBrowse { get; set; }

        public ControlRow(string strLabel)
        {
            Label = new Label
                        {
                            Anchor = AnchorStyles.Left,
                            AutoSize = true,
                            Text = strLabel + ":"
                        };
            TbxName = new TextBox
                          {
                              AutoSize = true,
                              Dock = DockStyle.Fill,
                              ReadOnly = true
                          };
            TbxComment = new TextBox
                             {
                                 AutoSize = true,
                                 Dock = DockStyle.Fill
                             };
            BtnBrowse = new Button
                            {
                                Text = "...",
                                Size = new Size(24, 23),
                                UseVisualStyleBackColor = true
                            };
            BtnBrowse.MouseUp += OnBtnBrowseMouseUp;
        }

        private void OnBtnBrowseMouseUp(object sender, MouseEventArgs e)
        {
            MyParent.HandleUnsTestMouseUp(TbxName, e);
        }
    }
}
