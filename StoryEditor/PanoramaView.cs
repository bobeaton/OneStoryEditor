using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class PanoramaView : TopForm
    {
        protected const int CnColumnStoryName = 0;
        protected const int CnColumnStoryPurpose = 1;
        protected const int CnColumnStoryEditToken = 2;
#if ShowingState
        protected const int CnColumnStoryStage = 3;
        protected const int CnColumnStoryTimeInStage = 4;
        protected const int CnColumnNumOfLines = 5;
        protected const int CnColumnTestQuestions = 6;
        protected const int CnColumnNumOfWords = 7;
#else
        protected const int CnColumnStoryTimeInStage = 3;
        protected const int CnColumnNumOfLines = 4;
        protected const int CnColumnTestQuestions = 5;
        protected const int CnColumnNumOfWords = 6;
#endif
        protected StoryProjectData _storyProject;
        private readonly TeamMemberData _loggedOnMember;
        protected StoriesData _stories;
        protected bool _bInCtor = true;
        protected ProjectSettings.LanguageInfo MainLang { get; set; }
        protected Font _fontForDev = new Font("Arial Unicode MS", 11);
        // protected TermRenderingsList renderings;
        // TermLocalizations termLocalizations;

        protected const int CnColumnGloss = 0;
        protected const int CnColumnRenderings = 1;
        protected const int CnColumnNotes = 2;

        private PanoramaView()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public PanoramaView(StoryProjectData storyProject, TeamMemberData loggedOnMember)
        {
            _storyProject = storyProject;
            _loggedOnMember = loggedOnMember;
            InitializeComponent();
            Localizer.Ctrl(this);
            
            richTextBoxPanoramaFrontMatter.Rtf = storyProject.PanoramaFrontMatter;
            _bInCtor = false;   // prevent the _TextChanged during ctor
            dataGridViewPanorama.Columns[CnColumnStoryName].DefaultCellStyle.Font =
                dataGridViewPanorama.Columns[CnColumnStoryPurpose].DefaultCellStyle.Font = _fontForDev;

            if (_storyProject.ProjSettings.Vernacular.HasData)
                MainLang = _storyProject.ProjSettings.Vernacular;
            else if (_storyProject.ProjSettings.NationalBT.HasData)
                MainLang = _storyProject.ProjSettings.NationalBT;
            else
                MainLang = _storyProject.ProjSettings.InternationalBT;
        }

        public DialogResult ShowDialog(string storySetName)
        {
            if (Properties.Settings.Default.PanoramaViewDlgHeight != 0)
            {
                Bounds = new Rectangle(Properties.Settings.Default.PanoramaViewDlgLocation,
                    new Size(Properties.Settings.Default.PanoramaViewDlgWidth,
                        Properties.Settings.Default.PanoramaViewDlgHeight));
            }

            // can't do 'Any' on tabControlSets.Controls, so gather the Text values for each of the controls
            List<string> lstTabTextNames = new List<string>();
            foreach (var control in tabControlSets.Controls)
                lstTabTextNames.Add(((TabPage)control).Text);

            foreach (var storySet in _storyProject.Values)
            {
                if (!lstTabTextNames.Any(sn => sn == storySet.SetName))
                {
                    var addlTab = new TabPage(storySet.SetName)
                    {
                        Location = new Point(4, 22),
                        Name = $"tabPage{storySet.SetName}",
                        Padding = new Padding(3),
                        Size = new Size(854, 474),
                        TabIndex = tabControlSets.Controls.Count,
                        UseVisualStyleBackColor = true
                    };

                    tabControlSets.Controls.Add(addlTab);

                    // also have to add menu items to the contextMenuMove Item collection
                    var moveToThisSetMenu = new ToolStripMenuItem($"Move selected story to \'{storySet.SetName}\' tab")
                    {
                        Name = $"moveTo{storySet.SetName.Replace(" ", null)}Menu",
                        Size = new Size(326, 22)
                    };
                    moveToThisSetMenu.Click += new EventHandler(this.MoveToStoriesMenuClick);
                    this.contextMenuMove.Items.Add(moveToThisSetMenu);

                    var copyToThisSetMenu = new ToolStripMenuItem($"Copy selected story to \'{storySet.SetName}\' tab")
                    {
                        Name = $"copyTo{storySet.SetName.Replace(" ", null)}Menu",
                        Size = new Size(326, 22)
                    };
                    copyToThisSetMenu.Click += new EventHandler(this.CopyToStoriesMenuClick);
                    this.contextMenuMove.Items.Add(copyToThisSetMenu);
                }
            }

            var tab = InitStoriesTab(storySetName);
            tabControlSets.SelectTab(tab);

            return ShowDialog();
        }

        public string SelectedStorySetName; 
        public string JumpToStory;
        public bool Modified;

        protected void InitGrid()
        {
            dataGridViewPanorama.Rows.Clear();
            if (_stories == null)
                return;

            foreach (StoryData aSD in _stories)
            {
                DateTime dateTime;
                var nCount = aSD.TransitionHistory.Count;
                if (nCount == 0)
                    dateTime = aSD.StageTimeStamp;
                else
                {
                    var storyStateTransition = aSD.TransitionHistory[nCount - 1];
                    dateTime = storyStateTransition.TransitionDateTime;
                }

                var ts = DateTime.Now - dateTime;
                string strTimeInState = "";
                if (ts.Days > 0)
                    strTimeInState += String.Format("{0} days, ", ts.Days);
                if (ts.Hours > 0)
                    strTimeInState += String.Format("{0} hours, ", ts.Hours);
                strTimeInState += String.Format("{0} minutes", ts.Minutes);

                string strWhoHasEditToken =
                    GetMemberWithEditTokenAsDisplayString(_storyProject.TeamMembers,
                                                          aSD.ProjStage.MemberTypeWithEditToken);

                string strMemberId = null;
                if (strWhoHasEditToken == TeamMemberData.CstrProjectFacilitatorDisplay)
                {
                    strMemberId = MemberIdInfo.SafeGetMemberId(aSD.CraftingInfo.ProjectFacilitator);
                }
                else if ((strWhoHasEditToken == TeamMemberData.CstrConsultantInTrainingDisplay) ||
                    (strWhoHasEditToken == TeamMemberData.CstrIndependentConsultantDisplay))
                {
                    strMemberId = MemberIdInfo.SafeGetMemberId(aSD.CraftingInfo.Consultant);
                }
                else if (strWhoHasEditToken == TeamMemberData.CstrCoachDisplay)
                {
                    strMemberId = MemberIdInfo.SafeGetMemberId(aSD.CraftingInfo.Coach);
                }

                var bInLoggedInUsersTurn = false;
                if (!String.IsNullOrEmpty(strMemberId))
                {
                    // if we have a single person's turn who has the edit token and they are the current user, 
                    //  then highlight the story
                    bInLoggedInUsersTurn = ((_loggedOnMember != null) && (_loggedOnMember.MemberGuid == strMemberId));

                    // give a name and role if it's just a single one
                    strWhoHasEditToken = String.Format("{0} ({1})",
                                                       strWhoHasEditToken,
                                                       _storyProject.GetMemberNameFromMemberGuid(strMemberId));
                }

                if ((aSD.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete) ||
                    (aSD.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamFinalApproval))
                {
                    StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[aSD.ProjStage.ProjectStage];
                    strWhoHasEditToken = String.Format("{0}: {1}",
                                                       st.StageDisplayString.Substring("Story has ".Length), 
                                                       strWhoHasEditToken);
                }

                var aObs = new object[]
                {   
                    aSD.Name, 
                    aSD.CraftingInfo.StoryPurpose, 
                    strWhoHasEditToken, 
#if ShowingState
                    st.StageDisplayString, 
#endif
                    strTimeInState,
                    aSD.NumOfLines,
                    aSD.NumOfTestQuestions,
                    aSD.NumOfWords(_storyProject.ProjSettings)
                };
                int nRowIndex = dataGridViewPanorama.Rows.Add(aObs);
                var aRow = dataGridViewPanorama.Rows[nRowIndex];
#if ShowingState
                aRow.Tag = st;
#endif
                aRow.Height = _fontForDev.Height + 4;

                if (bInLoggedInUsersTurn)
                    aRow.DefaultCellStyle.BackColor = Color.Yellow;
            }
        }

        private static bool IsInLoggedInUsersTurn(DataGridViewBand theRow)
        {
            return (theRow.DefaultCellStyle.BackColor == Color.Yellow);
        }

        // override to handle the case were the project state says it's in the 
        //  CIT's state, but really, it's an independent consultant
        public static string GetMemberWithEditTokenAsDisplayString(TeamMembersData teamMembers,
            TeamMemberData.UserTypes eMemberType)
        {
            if ((eMemberType != TeamMemberData.UserTypes.AnyEditor) &&
                 TeamMemberData.IsUser(eMemberType,
                        TeamMemberData.UserTypes.IndependentConsultant |
                        TeamMemberData.UserTypes.ConsultantInTraining))
            {
                // this is a special case where both the CIT and Independant Consultant are acceptable
                return TeamMemberData.GetMemberTypeAsDisplayString(
                    teamMembers.HasIndependentConsultant
                        ? TeamMemberData.UserTypes.IndependentConsultant
                        : TeamMemberData.UserTypes.ConsultantInTraining);
            }

            if (eMemberType == TeamMemberData.UserTypes.AnyEditor)
                eMemberType &= (teamMembers.HasIndependentConsultant)
                                   ? ~(TeamMemberData.UserTypes.ConsultantInTraining |
                                       TeamMemberData.UserTypes.Coach |
                                       TeamMemberData.UserTypes.FirstPassMentor)
                                   : ~(TeamMemberData.UserTypes.IndependentConsultant |
                                       TeamMemberData.UserTypes.FirstPassMentor);

            // otherwise, let the other version do it
            return TeamMemberData.GetMemberTypeAsDisplayString(eMemberType);
        }

        private StoryData _theStoryBeingEdited;
        private void dataGridViewPanorama_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex > CnColumnStoryPurpose))
                return;

            DataGridViewRow theRow = dataGridViewPanorama.Rows[e.RowIndex];
            DataGridViewCell theCell = theRow.Cells[CnColumnStoryName];
            if (theCell.Value == null)
                return;

            _theStoryBeingEdited = _stories.GetStoryFromName(theCell.Value as string);
        }

        private void dataGridViewPanorama_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex > CnColumnStoryPurpose))
                return;

            if (_theStoryBeingEdited == null)
            {
                LocalizableMessageBox.Show(Properties.Resources.IDS_CantEditPanoramaView,
                                StoryEditor.OseCaption);
                return;
            }

            try
            {
                DataGridViewRow theRow = dataGridViewPanorama.Rows[e.RowIndex];
                DataGridViewCell theCell = theRow.Cells[e.ColumnIndex];
                if (theCell.Value == null)
                    return;

                string strCellValue = ((string)theCell.Value).Trim();
                if (String.IsNullOrEmpty(strCellValue))
                    return;

                if (e.ColumnIndex == CnColumnStoryName)
                {
                    // if nothing's changed, then nothing to do
                    if (_theStoryBeingEdited.Name == strCellValue)
                        return; // return unModified

                    // then make sure there isn't already a story by that name
                    if (_stories.Contains(strCellValue))
                    {
                        LocalizableMessageBox.Show(
                            String.Format(
                                Localizer.Str(
                                    "There's already a story by the name of '{0}'. Story names must be unique, so ignoring the requested change"),
                                strCellValue),
                            StoryEditor.OseCaption);
                        theCell.Value = _theStoryBeingEdited.Name;
                        return;
                    }
                    else
                        _theStoryBeingEdited.Name = strCellValue;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(e.ColumnIndex == CnColumnStoryPurpose);
                    if (_theStoryBeingEdited.CraftingInfo.StoryPurpose != strCellValue)
                        _theStoryBeingEdited.CraftingInfo.StoryPurpose = strCellValue;
                    else
                        return; // return unModified
                }

                Modified = true;
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
            finally
            {
                _theStoryBeingEdited = null;    // don't confuse different editing sessions
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            int nSelectedColumnIndex = dataGridViewPanorama.SelectedCells[0].ColumnIndex;
            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex > 0)
            {
                DataGridViewRow theRow = dataGridViewPanorama.Rows[nSelectedRowIndex];
                DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
                if (theNameCell.Value == null)
                    return; // shouldn't happen, but...

                var strName = theNameCell.Value as String;

                StoryData theSDToMove = _stories.GetStoryFromName(strName);
                if (theSDToMove == null)
                    return;

                int nStoryIndex = _stories.IndexOf(theSDToMove);
                
                // I've disabled the ability to sort the rows (because what would it mean
                //  to move up if the order isn't in the panorama order). So the index
                //  of the story in the _stories collection should be the same as the
                //  index of the row in the datagrid
                System.Diagnostics.Debug.Assert(nStoryIndex == nSelectedRowIndex);

                _stories.Remove(theSDToMove);
                _stories.Insert(--nStoryIndex, theSDToMove);
                
                // display index, which could be different from story index (e.g. if they
                //  sorted the columns differently.
                nSelectedRowIndex--;
                InitGrid();
                System.Diagnostics.Debug.Assert(nSelectedRowIndex < dataGridViewPanorama.Rows.Count);
                dataGridViewPanorama.Rows[nSelectedRowIndex].Cells[nSelectedColumnIndex].Selected = true;
                Modified = true;
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0 (or I'm not understanding this properly
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            int nSelectedColumnIndex = dataGridViewPanorama.SelectedCells[0].ColumnIndex;
            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex < dataGridViewPanorama.Rows.Count - 1)
            {
                DataGridViewRow theRow = dataGridViewPanorama.Rows[nSelectedRowIndex];
                DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
                if (theNameCell.Value == null)
                    return; // shouldn't happen, but...

                var strName = theNameCell.Value as String;

                StoryData theSDToMove = _stories.GetStoryFromName(strName);
                if (theSDToMove == null)
                    return;

                int nStoryIndex = _stories.IndexOf(theSDToMove);

                // I've disabled the ability to sort the rows (because what would it mean
                //  to move up if the order isn't in the panorama order). So the index
                //  of the story in the _stories collection should be the same as the
                //  index of the row in the datagrid
                System.Diagnostics.Debug.Assert(nStoryIndex == nSelectedRowIndex);

                _stories.Remove(theSDToMove);
                _stories.Insert(++nStoryIndex, theSDToMove);

                // display index, which could be different from story index (e.g. if they
                //  sorted the columns differently.
                nSelectedRowIndex++;
                InitGrid();
                System.Diagnostics.Debug.Assert(nSelectedRowIndex < dataGridViewPanorama.Rows.Count);
                dataGridViewPanorama.Rows[nSelectedRowIndex].Cells[nSelectedColumnIndex].Selected = true;
                Modified = true;
            }
        }

        private void ButtonCopyToOldStoriesClick(object sender, EventArgs e)
        {
            contextMenuMove.Show(Cursor.Position);
        }

        private void ContextMenuMoveOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var selectedTab = tabControlSets.SelectedTab;
            var selectedTabName = selectedTab.Text.Replace(" ", null);

            foreach (ToolStripMenuItem controlItem in contextMenuMove.Items)
            {
                // will be one of: 
                //  "moveTo{selectedTabName}Menu"
                //  "copyTo{selectedTabName}Menu"
                string menuName = controlItem.Name;
                if ((menuName == $"moveTo{selectedTabName}Menu") ||
                    (menuName == $"copyTo{selectedTabName}Menu"))
                {
                    controlItem.Enabled = false;
                }
                else
                {
                    controlItem.Enabled = true;
                }
            }
        }

        private static string CstrMoved
        {
            get { return Localizer.Str("moved"); }
        }

        private static string CstrCopied
        {
            get { return Localizer.Str("copied"); }
        }

        private void CopyToStoriesMenuClick(object sender, EventArgs e)
        {
            var tsmi = (ToolStripMenuItem)sender;
            var destTabText = tsmi.Text;
            var finalIndex = destTabText.LastIndexOf('\'');
            int startIndex = "Move selected story to \'".Length;
            var destTabSetName = destTabText.Substring(startIndex, finalIndex - startIndex);
            MoveCopyStoryToOtherStoriesSet(destTabSetName,
                                           false, true, true);
        }

        private void MoveToStoriesMenuClick(object sender, EventArgs e)
        {
            var tsmi = (ToolStripMenuItem)sender;
            var destTabText = tsmi.Text;
            var finalIndex = destTabText.LastIndexOf('\'');
            int startIndex = "Move selected story to \'".Length;
            var destTabSetName = destTabText.Substring(startIndex, finalIndex - startIndex);
            MoveCopyStoryToOtherStoriesSet(destTabSetName,
                                           true, true, true);
        }

        private void MoveCopyStoryToOtherStoriesSet(string strDestSet, bool bMove,
            bool bAdjustCraftingInfo, bool bIsBiblicalStory)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            System.Diagnostics.Debug.Assert(_storyProject.ContainsKey(strDestSet));

            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex > dataGridViewPanorama.Rows.Count - 1)
                return;

            DataGridViewRow theRow = dataGridViewPanorama.Rows[nSelectedRowIndex];
            DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
            if (theNameCell.Value == null)
                return; // shouldn't happen, but...

            var strName = theNameCell.Value as String;

            var theOrigSd = _stories.GetStoryFromName(strName);
            if (theOrigSd == null)
                return;

            var theSd = theOrigSd;
            // if moving, then we have to remove it out of the current list
            if (bMove)
                RemoveStoryFromCurrentList(nSelectedRowIndex, theSd);
            
            // if copying, then it needs its own guids
            theSd = new StoryData(theOrigSd);

            if (bAdjustCraftingInfo)
                theSd.CraftingInfo.IsBiblicalStory = bIsBiblicalStory;

            StoryEditor.InsertInOtherSetInsureUnique(_storyProject[strDestSet], theSd);

            LocalizableMessageBox.Show(String.Format(Localizer.Str("The story '{0}' has been {1} to the '{2}' list"),
                                                     theSd.Name,
                                                     (bMove) ? CstrMoved : CstrCopied,
                                                     strDestSet),
                                       StoryEditor.OseCaption);
            Modified = true;
        }

        private void ButtonDeleteClick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex <= dataGridViewPanorama.Rows.Count - 1)
            {
                DataGridViewRow theRow = dataGridViewPanorama.Rows[nSelectedRowIndex];
                DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
                if (theNameCell.Value == null)
                    return; // shouldn't happen, but...

                var strName = theNameCell.Value as String;

                StoryData theSd = _stories.GetStoryFromName(strName);
                if (theSd == null)
                    return;

                // make sure the user really wants to do this
                if (StoryEditor.QueryDeleteStory(strName))
                    return;

                RemoveStoryFromCurrentList(nSelectedRowIndex, theSd);

                // if it isn't already in the Old Set, then just move it there
                if (_stories.SetName != Properties.Resources.IDS_ObsoleteStoriesSet)
                {
                    // make a copy (so it has new guids) -- this is just in case, someone simultaneously
                    //  edits and so this isn't actually deleted which could result in two story's with the
                    //  same guids
                    theSd = new StoryData(theSd);
                    StoryEditor.InsertInOtherSetInsureUnique(_storyProject[Properties.Resources.IDS_ObsoleteStoriesSet],
                                                             theSd);
                }

                Modified = true;
            }
        }

        private void RemoveStoryFromCurrentList(int nSelectedRowIndex, StoryData theSd)
        {
            _stories.Remove(theSd);
            InitGrid();
            if (nSelectedRowIndex >= dataGridViewPanorama.Rows.Count)
                nSelectedRowIndex--;

            if ((nSelectedRowIndex >= 0) && (nSelectedRowIndex < dataGridViewPanorama.Rows.Count))
                dataGridViewPanorama.Rows[nSelectedRowIndex].Selected = true;
        }

        private void TabControlSetsSelected(object sender, TabControlEventArgs e)
        {
            TabPage tab = e.TabPage;
            if ((tab != null) && (tab != tabPageFrontMatter))
            {
                InitStoriesTab(tab.Text);
            }
        }

        private TabPage InitStoriesTab(string currentStoriesSetName)
        {
            System.Diagnostics.Debug.Assert(_storyProject.ContainsKey(currentStoriesSetName));
            SelectedStorySetName = currentStoriesSetName;
            _stories = _storyProject[currentStoriesSetName];

            TabPage tab = null;
            foreach (var control in tabControlSets.Controls)
            {
                TabPage tabControl = (TabPage)control;
                var tabText = tabControl.Text;
                if (tabText == currentStoriesSetName)
                    tab = tabControl;
            }

            System.Diagnostics.Debug.Assert(tab != null);
            InitParentTab(tab);
            InitGrid();
            return tab;
        }

        protected void InitParentTab(TabPage tab)
        {
            tableLayoutPanel.Parent = tab;
        }

        private void RichTextBoxPanoramaFrontMatterTextChanged(object sender, EventArgs e)
        {
            // skip this supposed change unless we're not in the ctor (the setting of 
            //  the Rtf value in the ctor is falsely triggering this call, but that's not
            //  a legitimate change)
            if (!_bInCtor)
            {
                _storyProject.PanoramaFrontMatter = richTextBoxPanoramaFrontMatter.Rtf;
                Modified = true;
            }
        }
        /*
        private void EditRenderings(DataGridViewRow theRow, string strId)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strId));

            TermRendering termRendering = renderings.GetRendering(strId);
            Localization termLocalization = termLocalizations.GetTermLocalization(strId);

            EditRenderingsForm form = new EditRenderingsForm(
                MainLang.FontToUse,
                termRendering.Renderings,
                termRendering,
                MainLang.LangCode,
                termLocalization);

            if (form.ShowDialog() == DialogResult.OK)
            {
                if ((termRendering.Renderings != theRow.Cells[CnColumnRenderings].Value.ToString())
                    || (termRendering.Notes != theRow.Cells[CnColumnNotes].Value.ToString()))
                {
                    renderings.RenderingsChanged = true;
                    theRow.Cells[CnColumnRenderings].Value = termRendering.Renderings;
                    theRow.Cells[CnColumnNotes].Value = termRendering.Notes;
                }
            }

            form.Dispose();
        }

        private void dataGridViewKeyTerms_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // make sure we have something reasonable
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewKeyTerms.Rows.Count))
                return;

            DataGridViewRow theRow = dataGridViewKeyTerms.Rows[e.RowIndex];
            string strId = theRow.Tag as string;
            EditRenderings(theRow, strId);
        }
        */

        private void PanoramaViewFormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (renderings != null)
                renderings.PromptForSave(_storyProject.ProjSettings.ProjectFolder);
            */
            Properties.Settings.Default.PanoramaViewDlgLocation = Location;
            Properties.Settings.Default.PanoramaViewDlgHeight = Bounds.Height;
            Properties.Settings.Default.PanoramaViewDlgWidth = Bounds.Width;
            Properties.Settings.Default.Save();
        }

        private void DataGridViewPanoramaCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || ((e.ColumnIndex < CnColumnStoryName) || e.ColumnIndex > CnColumnNumOfWords))
                return;

            var theRow = dataGridViewPanorama.Rows[e.RowIndex];
            var theNameCell = theRow.Cells[CnColumnStoryName];
            if (theNameCell.Value == null)
                return; // shouldn't happen, but...

            var strName = theNameCell.Value as String;
            var theSD = _stories.GetStoryFromName(strName);
            if (theSD == null)
                return;

            if (e.ColumnIndex == CnColumnStoryName)
            {
                JumpToStory = strName;
                Close();
                return;
            }

            if (!theSD.TransitionHistory.HasData)
            {
                StoryEditor.WarnNoTransitionHistory();
                return;
            }

            var dlg = new TransitionHistoryForm(theSD.TransitionHistory, _storyProject.TeamMembers);
            dlg.ShowDialog();
        }

        private void CopyGridToClipboard(IEnumerable dataGridViewRowCollection)
        {
            var strHtml = "<tr><th>Story Name</th><th>Purpose</th><th>Who Edits</th><th>Time in Turn</th><th># of Lines</th><th># of TQs</th><th># of Words</th></tr>";
            foreach (DataGridViewRow aRow in dataGridViewRowCollection)
            {
                var strRowHtml = aRow.Cells.Cast<DataGridViewCell>().Aggregate<DataGridViewCell, string>(null, (current, aCell) => current + String.Format("<td>{0}</td>", aCell.Value));
                strHtml += String.Format("<tr class='{1}'>{0}</tr>", 
                                         strRowHtml, 
                                         IsInLoggedInUsersTurn(aRow)
                                            ? "highlight"
                                            : "");
            }

            strHtml = String.Format(@"<table>{0}</table>", strHtml);

            HtmlFragment.CopyToClipboard(strHtml, 
                                         String.Format("OneStory Editor: {0}", _storyProject.ProjSettings.ProjectName),
                                         @"<style type=""text/css"">table, th, td { border-style: solid; border-width: 1px; } .highlight { background-color:yellow; }</style>",
                                         null);
        }

        private void buttonCopyToClipboard_Click(object sender, EventArgs e)
        {
            CopyGridToClipboard(dataGridViewPanorama.Rows);
        }

        #region obsolete code
        /*
        protected DataGridViewRow m_rowLast = null;
        
        void buttonCopyToOldStories_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            if (tabControlSets.SelectedTab == tabPagePanorama)
            {
                // copy it to the 'old stories' set
                CopyStoryToOtherStoriesSet(Properties.Resources.IDS_ObsoleteStoriesSet);
            }
            else if (tabControlSets.SelectedTab == tabPageOldStories)
            {
                // copy it back!
                CopyStoryToOtherStoriesSet(Properties.Resources.IDS_MainStoriesSet);
            }
        }

        private void contextMenuStripProjectStages_Opening(object sender, CancelEventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedRows.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedRows.Count != 1)
                return;

            contextMenuStripProjectStages.Items.Clear();
            m_rowLast = dataGridViewPanorama.SelectedRows[0];
            StoryStageLogic.StateTransition theCurrentST = (StoryStageLogic.StateTransition)m_rowLast.Tag;
            if (theCurrentST == null)
                return;

            foreach (StoryStageLogic.ProjectStages eAllowableTransition in theCurrentST.AllowableTransitions)
            {
                StoryStageLogic.StateTransition aST = StoryStageLogic.stateTransitions[eAllowableTransition];
                ToolStripItem tsi = contextMenuStripProjectStages.Items.Add(
                                    aST.StageDisplayString, null, OnSelectOtherState);
                tsi.Tag = aST;
                tsi.Enabled = theCurrentST.AllowableTransitions.Contains(eAllowableTransition);
            }
        }

        protected void OnSelectOtherState(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripItem);
            ToolStripItem tsi = (ToolStripItem)sender;

            StoryStageLogic.StateTransition theCurrentST = (StoryStageLogic.StateTransition)tsi.Tag;
            if (theCurrentST == null)
                return;

            m_rowLast.Cells[CnColumnStoryStage].Value = theCurrentST.StageDisplayString;

            string strStoryName = (string)theCurrentST.Cells[CnColumnStoryName].Value;
            foreach (StoryData aSD in _stories)
                if (aSD.Name == strStoryName)
                {
                    GetTransitionInfoForRow(theCurrentStageTransition.StageDisplayString, out eStage);
                    aSD.ProjStage.ProjectStage = eStage;
                    break;
                }
        }
        */
        #endregion obsolete code
    }
}
