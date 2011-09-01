using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class StoryFrontMatterForm : TopForm
    {
        private readonly StoryEditor _theSe;
        private readonly StoryProjectData _theStoryProjectData;
        private readonly StoryData _theCurrentStory;
        private readonly TestRows _rowsRetellings = new TestRows();
        private readonly TestRows _rowsAnswers = new TestRows();
        private TestRows _rowsToDelete = new TestRows();

        private StoryFrontMatterForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public StoryFrontMatterForm(StoryEditor theSE, StoryProjectData theStoryProjectData, StoryData theCurrentStory)
            : base(true)
        {
            _theSe = theSE;
            _theStoryProjectData = theStoryProjectData;
            _theCurrentStory = theCurrentStory;

            InitializeComponent();
            Localizer.Ctrl(this);

            textBoxStoryPurpose.Text = theCurrentStory.CraftingInfo.StoryPurpose;
            textBoxResourcesUsed.Text = theCurrentStory.CraftingInfo.ResourcesUsed;
            textBoxMiscNotes.Text = theCurrentStory.CraftingInfo.MiscellaneousStoryInfo;

            InitControls(theCurrentStory.CraftingInfo.ProjectFacilitator,
                         textBoxProjectFacilitator,
                         textBoxCommentProjectFacilitator,
                         linkLabelForProjectFacilitatorView,
                         linkLabelForProjectFacilitatorChange);

            InitControls(theCurrentStory.CraftingInfo.Consultant,
                         textBoxConsultant,
                         textBoxCommentConsultant,
                         linkLabelForConsultantView,
                         linkLabelForConsultantChange);

            // only show the coach if this is a manage with coaching situation
            InitToolboxTextTip(theCurrentStory.CraftingInfo.Coach,
                               labelCoach,
                               textBoxCoach,
                               textBoxCommentCoach,
                               linkLabelForCoachView,
                               linkLabelForCoachChange,
                               !theStoryProjectData.TeamMembers.HasIndependentConsultant ||
                               theCurrentStory.HasCoachNoteData);

            InitControls(theCurrentStory.CraftingInfo.StoryCrafter,
                         textBoxStoryCrafter,
                         textBoxCommentStoryCrafter,
                         linkLabelForStoryCrafterView,
                         linkLabelForStoryCrafterChange);

            InitControls(theCurrentStory.CraftingInfo.BackTranslator,
                         textBoxUnsBackTranslator,
                         textBoxCommentUnsBackTranslator,
                         linkLabelForUnsBterView,
                         linkLabelForUnsBterChange);

            // initialize a callback pointer for when mouses click up
            ControlRow.MyParent = this;

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            InitializeTestRows(theCurrentStory.CraftingInfo.TestersToCommentsRetellings,
                _rowsRetellings,
                Localizer.Str("Retelling Test &"));

            InitializeTestRows(theCurrentStory.CraftingInfo.TestersToCommentsTqAnswers,
                _rowsAnswers,
                Localizer.Str("Story Question Test &"));

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);

            Text = String.Format(Localizer.Str("Story Information for '{0}'"), theCurrentStory.Name);

            Modified = false;
            buttonOK.Enabled = false;
            buttonCancel.Text = Localizer.Str("Close");
        }

        private void InitializeTestRows(TestInfo testInfo, TestRows rows, 
            string cstrLabel)
        {
            for (int i = 1; i <= testInfo.Count; i++)
            {
                var row = new ControlRow(cstrLabel + i);
                var tester = testInfo[i - 1];
                InitTesterRow(row, tester);
                rows.Add(row);
            }
        }

        private void InitTesterRow(ControlRow row, MemberIdInfo tester)
        {
            // add the row of controls to the table layout panel
            int nRow = tableLayoutPanel.RowCount;
            tableLayoutPanel.Controls.Add(row.Label, 0, nRow);
            tableLayoutPanel.Controls.Add(row.TbxName, 1, nRow);
            tableLayoutPanel.Controls.Add(row.TbxComment, 2, nRow);
            tableLayoutPanel.Controls.Add(row.LinkLabelView, 3, nRow);
            tableLayoutPanel.Controls.Add(row.LinkLabelChange, 4, nRow);
            tableLayoutPanel.Controls.Add(row.LinkLabelDelete, 5, nRow);
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowCount++;
            
            // now initialize it with data
            var member = _theStoryProjectData.GetMemberFromId(tester.MemberId);
            if (member != null)
                InitToolboxTextTip(member, row.TbxName);
            row.TbxComment.Text = tester.MemberComment;
            toolTip.SetToolTip(row.LinkLabelView, StoryInformationTooltipLinkView);
            toolTip.SetToolTip(row.LinkLabelChange, StoryInformationTooltipLinkChange);
        }

        internal static string CstrDeleteTest
        {
            get { return Localizer.Str("Delete"); }
        }

        internal static string CstrUndeleteTest
        {
            get { return Localizer.Str("Undelete"); }
        }

        private void InitToolboxTextTip(MemberIdInfo memberInfo, Label lbl, 
            TextBox tbName, TextBox tbComment, LinkLabel linkView, LinkLabel linkChange, 
            bool bShowControls)
        {
            if (bShowControls)
                InitControls(memberInfo, tbName, tbComment, linkView, linkChange);
            else
                lbl.Visible = tbName.Visible = tbComment.Visible = linkView.Visible = 
                    linkChange.Visible = false;
        }

        private void InitControls(MemberIdInfo memberInfo, TextBox tbName, 
            TextBox tbComment, LinkLabel linkView, LinkLabel linkChange)
        {
            if (MemberIdInfo.Configured(memberInfo))
            {
                var member = _theStoryProjectData.GetMemberFromId(memberInfo.MemberId);
                if (member != null)
                    InitToolboxTextTip(member, tbName);
                tbComment.Text = memberInfo.MemberComment;
            }
            toolTip.SetToolTip(linkView, StoryInformationTooltipLinkView);
            toolTip.SetToolTip(linkChange, StoryInformationTooltipLinkChange);
        }

        private void InitToolboxTextTip(TeamMemberData tmd, TextBox tbName)
        {
            tbName.Text = tmd.Name;
            toolTip.SetToolTip(tbName, tmd.BioData);
        }

        private static string StoryInformationTooltipLinkView
        {
            get { return Localizer.Str("Click to edit the contact or bio data for this team member"); }
        }

        private static string StoryInformationTooltipLinkChange
        {
            get { return Localizer.Str("Click to choose another team member"); }
        }

        private void linkLabelViewForProjectFacilitator_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProjectFacilitatorHandler(false);
        }

        private void linkLabelChangeForProjectFacilitator_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProjectFacilitatorHandler(true);
        }

        private void ProjectFacilitatorHandler(bool bChange)
        {
            HandleMouseUp(bChange, textBoxProjectFacilitator,
                          TeamMemberData.UserTypes.ProjectFacilitator,
                          "Choose the Project Facilitator for this story");
        }

        private void linkLabelViewForConsultant_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConsultantHandler(false);
        }

        private void linkLabelChangeForConsultant_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConsultantHandler(true);
        }

        private void ConsultantHandler(bool bChange)
        {
            HandleMouseUp(bChange, textBoxConsultant,
                          TeamMemberData.UserTypes.ConsultantInTraining |
                          TeamMemberData.UserTypes.IndependentConsultant,
                          "Choose the Consultant/CIT for this story");
        }

        private void linkLabelViewForCoach_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CoachHandler(false);
        }

        private void linkLabelChangeForCoach_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CoachHandler(true);
        }

        private void CoachHandler(bool bChange)
        {
            HandleMouseUp(bChange, textBoxCoach,
                          TeamMemberData.UserTypes.Coach,
                          "Choose the Coach for this story");
        }

        private void linkLabelViewForStoryCrafter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StoryCrafterHandler(false);
        }

        private void linkLabelChangeForStoryCrafter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StoryCrafterHandler(true);
        }

        private void StoryCrafterHandler(bool bChange)
        {
            HandleMouseUp(bChange, textBoxStoryCrafter,
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
                MarkTouched();
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
            _theSe.Modified = true;
        }

        private void linkLabelViewForUnsBter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UnsBterHandler(false);
        }

        private void linkLabelChangeForUnsBter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UnsBterHandler(true);
        }

        private void UnsBterHandler(bool bChange)
        {
            HandleMouseUp(bChange, textBoxUnsBackTranslator,
                          TeamMemberData.UserTypes.UNS,
                          "Choose the back-translator for this story");
        }

        public void HandleUnsTestMouseUp(bool bChange, TextBox tb)
        {
            HandleMouseUp(bChange, tb,
                          TeamMemberData.UserTypes.UNS,
                          "Choose the UNS that took this test");
        }

        public void DeleteTestClicked(ControlRow row)
        {
            // if deleting it, show all the controls ith strikeout (and the link as 'undelete')
            var fontStyleNew = new FontStyle();
            if (_rowsToDelete.Contains(row))
            {
                fontStyleNew = new FontStyle();
                row.LinkLabelDelete.Text = CstrDeleteTest;
                _rowsToDelete.Remove(row);
            }
            else
            {
                fontStyleNew |= FontStyle.Strikeout;
                row.LinkLabelDelete.Text = CstrUndeleteTest;
                _rowsToDelete.Add(row);
            }

            row.Label.Font = new Font(row.Label.Font, fontStyleNew);
            row.TbxName.Font = new Font(row.TbxName.Font, fontStyleNew);
            row.TbxComment.Font = new Font(row.TbxComment.Font, fontStyleNew);
            row.LinkLabelView.Font = new Font(row.LinkLabelView.Font, fontStyleNew);
            row.LinkLabelChange.Font = new Font(row.LinkLabelChange.Font, fontStyleNew);
            row.TbxName.Font = new Font(row.TbxName.Font, fontStyleNew);

            MarkTouched();
        }

        public bool Modified;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxProjectFacilitator.Tag != null)
            {
                var thePF = (TeamMemberData)textBoxProjectFacilitator.Tag;
                _theCurrentStory.ReplaceProjectFacilitator(thePF.MemberGuid);
                Modified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.ProjectFacilitator,
                               textBoxCommentProjectFacilitator, ref Modified);

            if (textBoxConsultant.Tag != null)
            {
                var theCons = (TeamMemberData)textBoxConsultant.Tag;
                _theCurrentStory.ReplaceConsultant(theCons.MemberGuid);
                Modified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.Consultant,
                               textBoxCommentConsultant, ref Modified);

            if (textBoxCoach.Tag != null)
            {
                var theCch = (TeamMemberData)textBoxCoach.Tag;
                _theCurrentStory.ReplaceCoach(theCch.MemberGuid);
                Modified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.Coach,
                               textBoxCommentCoach, ref Modified);

            if (textBoxStoryCrafter.Tag != null)
            {
                var theSC = (TeamMemberData) textBoxStoryCrafter.Tag;
                MemberIdInfo.SetCreateIfEmpty(ref _theCurrentStory.CraftingInfo.StoryCrafter,
                                              theSC.MemberGuid, true);
                Modified = true;                
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.StoryCrafter,
                               textBoxCommentStoryCrafter, ref Modified);

            if (textBoxUnsBackTranslator.Tag != null)
            {
                var theBT = (TeamMemberData)textBoxUnsBackTranslator.Tag;
                MemberIdInfo.SetCreateIfEmpty(ref _theCurrentStory.CraftingInfo.BackTranslator,
                                              theBT.MemberGuid, true);
                Modified = true;
            }
            GetChangeToComment(_theCurrentStory.CraftingInfo.BackTranslator,
                               textBoxCommentUnsBackTranslator, ref Modified);

            if (_theCurrentStory.CraftingInfo.StoryPurpose != textBoxStoryPurpose.Text)
            {
                _theCurrentStory.CraftingInfo.StoryPurpose = textBoxStoryPurpose.Text;
                Modified = true;
            }

            if (_theCurrentStory.CraftingInfo.ResourcesUsed != textBoxResourcesUsed.Text)
            {
                _theCurrentStory.CraftingInfo.ResourcesUsed = textBoxResourcesUsed.Text;
                Modified = true;
            }

            if (_theCurrentStory.CraftingInfo.MiscellaneousStoryInfo != textBoxMiscNotes.Text)
            {
                _theCurrentStory.CraftingInfo.MiscellaneousStoryInfo = textBoxMiscNotes.Text;
                Modified = true;
            }

            PurgeDeletedTests(ref Modified);

            ProcessTestCommentChanges(_theCurrentStory.CraftingInfo.TestersToCommentsRetellings,
                _rowsRetellings, ref Modified);

            ProcessTestCommentChanges(_theCurrentStory.CraftingInfo.TestersToCommentsTqAnswers,
                _rowsAnswers, ref Modified);

            try
            {
                _rowsRetellings.CheckForDuplicateUns();
                _rowsAnswers.CheckForDuplicateUns();

                _theCurrentStory.CraftingInfo.TestersToCommentsRetellings =
                    _rowsRetellings.ChangeTesters(
                        _theCurrentStory.CraftingInfo.TestersToCommentsRetellings,
                        _theCurrentStory.ChangeRetellingTester,
                        _theCurrentStory.Verses.ChangeRetellingTesterGuid,
                        _theStoryProjectData.TeamMembers,
                        ref Modified);

                _theCurrentStory.CraftingInfo.TestersToCommentsTqAnswers =
                    _rowsAnswers.ChangeTesters(
                        _theCurrentStory.CraftingInfo.TestersToCommentsTqAnswers,
                        _theCurrentStory.ChangeTqAnswersTester,
                        _theCurrentStory.Verses.ChangeTqAnswersTesterGuid,
                        _theStoryProjectData.TeamMembers,
                        ref Modified);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }

            if (Modified)
                _theSe.Modified = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void PurgeDeletedTests(ref bool bModified)
        {
            foreach (var controlRow in _rowsToDelete)
            {
                var nIndex = _rowsRetellings.IndexOf(controlRow);
                if (nIndex != -1)
                {
                    if (_theSe.QueryForAndDeleteRetellingTest(nIndex))
                    {
                        _rowsRetellings.Remove(controlRow);
                        bModified = true;
                    }
                }
                else if ((nIndex = _rowsAnswers.IndexOf(controlRow)) != -1)
                {
                    if (_theSe.QueryForAndDeleteAnswerTest(nIndex))
                    {
                        _rowsAnswers.Remove(controlRow);
                        bModified = true;
                    }
                }
            }
        }

        private static void ProcessTestCommentChanges(TestInfo testInfo, 
            TestRows lstRows, ref bool bModified)
        {
            System.Diagnostics.Debug.Assert(testInfo.Count == lstRows.Count, "bad assumption after having deleted a test");
            for (int i = 0; i < lstRows.Count; i++)
            {
                var memberIdInfo = testInfo[i];
                var testRow = lstRows[i];
                GetChangeToComment(memberIdInfo, testRow.TbxComment, ref bModified);
            }
        }
        
        private static void GetChangeToComment(MemberIdInfo testerInfo, TextBox tb, 
            ref bool bModified)
        {
            if (MemberIdInfo.Configured(testerInfo) && 
                (tb.Text != testerInfo.MemberComment))
            {
                testerInfo.MemberComment = tb.Text;
                bModified = true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            MarkTouched();
        }

        public void MarkTouched()
        {
            Modified = true;
            buttonOK.Enabled = true;
            buttonCancel.Text = "Cancel";
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
                    throw new ApplicationException(StoryEditor.NewUnsMessage(aRow.TbxName.Text));
                lstUnsGuids.Add(aRow.TbxName.Text);
            }
        }

        public delegate void ChangeTesterMethod(TeamMembersData teamMembersData,
            int nIndex, string strNewGuid, ref TestInfo testInfoNew);
        public delegate void ChangeTesterGuid(string strOldGuid, string strNewGuid);

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
        public TestInfo ChangeTesters(TestInfo testInfo, ChangeTesterMethod changeMethod,
            ChangeTesterGuid changeTesterGuid, TeamMembersData teamMembersData, 
            ref bool bModified)
        {
            var testInfoNew = new TestInfo();
            for (int i = 0; i < Count; i++)
            {
                var aRow = this[i];
                ChangeTester(aRow.TbxName, i, testInfo, changeMethod, teamMembersData,
                             ref testInfoNew, ref bModified);
            }
            return AdjustForSwap(testInfoNew, teamMembersData, changeTesterGuid);
        }

        private static TestInfo AdjustForSwap(TestInfo testInfo, 
            TeamMembersData teamMembersData, ChangeTesterGuid changeTesterGuid)
        {
            foreach (var aTester in testInfo)
                if (teamMembersData.ContainsKey(aTester.MemberId))
                {
                    // this means we had temporarily used the member's *name* as their
                    //  guid (in the case where the person was already in the list, 
                    //  we couldn't just change the guid due to the 'swap' problem--
                    //  see StoryData.ChangeTester for details)
                    var member = teamMembersData[aTester.MemberId];
                    changeTesterGuid(aTester.MemberId, member.MemberGuid);
                    aTester.MemberId = member.MemberGuid;
                }
            return testInfo;
        }

        private static void ChangeTester(TextBox tb, int nIndex, TestInfo testInfo,
            ChangeTesterMethod changeMethod, TeamMembersData teamMembersData, 
            ref TestInfo testInfoNew, ref bool bModified)
        {
            TeamMemberData theNewUns;
            if (ChangeTester(tb, nIndex, testInfo, teamMembersData, out theNewUns, 
                ref bModified))
            {
                changeMethod(teamMembersData, nIndex, theNewUns.MemberGuid, 
                    ref testInfoNew);
            }
        }

        private static bool ChangeTester(TextBox tb, int nIndex, TestInfo testInfoCurrent,
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
                var testerInfo = testInfoCurrent[nIndex];
                theNewUns = teamMembersData.GetMemberFromId(testerInfo.MemberId);
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
        public LinkLabel LinkLabelView { get; set; }
        public LinkLabel LinkLabelChange { get; set; }
        public LinkLabel LinkLabelDelete { get; set; }

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
            TbxComment.TextChanged += textBox_TextChanged;
            LinkLabelView = new LinkLabel
                                {
                                    Anchor = AnchorStyles.Left,
                                    Text = Localizer.Str("Edit"),
                                    AutoSize = true
                                };
            LinkLabelView.Click += OnLinkClickView;
            LinkLabelChange = new LinkLabel
                                  {
                                      Anchor = AnchorStyles.Left,
                                      Text = Localizer.Str("Change"),
                                      AutoSize = true
                                  };
            LinkLabelChange.Click += OnLinkClickChange;
            LinkLabelDelete = new LinkLabel
                                  {
                                      Anchor = AnchorStyles.Left,
                                      Text = StoryFrontMatterForm.CstrDeleteTest,
                                      AutoSize = true
                                  };
            LinkLabelDelete.Click += OnLinkClickDelete;
        }

        private static void textBox_TextChanged(object sender, EventArgs e)
        {
            MyParent.MarkTouched();
        }

        private void OnLinkClickView(object sender, EventArgs e)
        {
            MyParent.HandleUnsTestMouseUp(false, TbxName);
        }

        private void OnLinkClickChange(object sender, EventArgs e)
        {
            MyParent.HandleUnsTestMouseUp(true, TbxName);
        }

        private void OnLinkClickDelete(object sender, EventArgs e)
        {
            MyParent.DeleteTestClicked(this);
        }
    }
}
