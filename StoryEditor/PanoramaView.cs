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
        protected StoriesData _stories;
        protected bool _bInCtor = true;
        protected ProjectSettings.LanguageInfo MainLang { get; set; }
        protected Font _fontForDev = new Font("Arial Unicode MS", 11);
        protected TermRenderingsList renderings;
        TermLocalizations termLocalizations;

        protected const int CnColumnGloss = 0;
        protected const int CnColumnRenderings = 1;
        protected const int CnColumnNotes = 2;

        public PanoramaView(StoryProjectData storyProject)
            : base(true)
        {
            _storyProject = storyProject;
            InitializeComponent();
            
            // not really using key terms anymore
            tabControlSets.TabPages.Remove(tabPageKeyTerms);

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

        public new DialogResult ShowDialog()
        {
            if (Properties.Settings.Default.PanoramaViewDlgHeight != 0)
            {
                Bounds = new Rectangle(Properties.Settings.Default.PanoramaViewDlgLocation,
                    new Size(Properties.Settings.Default.PanoramaViewDlgWidth,
                        Properties.Settings.Default.PanoramaViewDlgHeight));
            }

            InitStoriesTab(tabPagePanorama);
            tabControlSets.SelectTab(tabPagePanorama);

            return base.ShowDialog();
        }

        public bool Modified = false;

        protected void InitGrid()
        {
            dataGridViewPanorama.Rows.Clear();
            if (_stories == null)
                return;

            foreach (StoryData aSD in _stories)
            {
                TimeSpan ts = DateTime.Now - aSD.StageTimeStamp;
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
                else if (strWhoHasEditToken == TeamMemberData.UserTypes.Coach.ToString())
                {
                    strMemberId = MemberIdInfo.SafeGetMemberId(aSD.CraftingInfo.Coach);
                }

                if (!String.IsNullOrEmpty(strMemberId))
                {
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
                DataGridViewRow aRow = dataGridViewPanorama.Rows[nRowIndex];
#if ShowingState
                aRow.Tag = st;
#endif
                aRow.Height = _fontForDev.Height + 4;
            }
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
                MessageBox.Show(Properties.Resources.IDS_CantEditPanoramaView,
                                OseResources.Properties.Resources.IDS_Caption);
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
                    if (_theStoryBeingEdited.Name != strCellValue)
                        _theStoryBeingEdited.Name = strCellValue;
                    else
                        return; // return unModified
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
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
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

        void buttonCopyToOldStories_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            if (tabControlSets.SelectedTab == tabPagePanorama)
            {
                // copy it to the 'old stories' set
                CopyStoryToOtherStoriesSet(OseResources.Properties.Resources.IDS_ObsoleteStoriesSet);
            }
            else if (tabControlSets.SelectedTab == tabPageObsolete)
            {
                // copy it back!
                CopyStoryToOtherStoriesSet(OseResources.Properties.Resources.IDS_MainStoriesSet);
            }
        }

        private void CopyStoryToOtherStoriesSet(string strDestSet)
        {
            System.Diagnostics.Debug.Assert((strDestSet == OseResources.Properties.Resources.IDS_ObsoleteStoriesSet)
                || (strDestSet == OseResources.Properties.Resources.IDS_MainStoriesSet));

            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex <= dataGridViewPanorama.Rows.Count - 1)
            {
                DataGridViewRow theRow = dataGridViewPanorama.Rows[nSelectedRowIndex];
                DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
                if (theNameCell.Value == null)
                    return; // shouldn't happen, but...

                var strName = theNameCell.Value as String;

                var theOrigSd = _stories.GetStoryFromName(strName);
                if (theOrigSd == null)
                    return;

                var theSD = new StoryData(theOrigSd);
                int n = 1;
                if (_storyProject[strDestSet].Contains(theSD))
                {
                    while (_storyProject[strDestSet].Contains(theSD))
                        theSD.Name = String.Format("{0}.{1}", strName, n++);
                    theSD.guid = Guid.NewGuid().ToString();
                }
                _storyProject[strDestSet].Add(theSD);
                Modified = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
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
                if (MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmDeleteStory,
                                                  strName),
                                    OseResources.Properties.Resources.IDS_Caption, 
                                    MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    return;

                _stories.Remove(theSd);
                InitGrid();
                if (nSelectedRowIndex >= dataGridViewPanorama.Rows.Count)
                    nSelectedRowIndex--;

                if ((nSelectedRowIndex >= 0) && (nSelectedRowIndex < dataGridViewPanorama.Rows.Count))
                    dataGridViewPanorama.Rows[nSelectedRowIndex].Selected = true;

                Modified = true;
            }
        }

        BiblicalTermsList _biblicalTerms;

        private void tabControlSets_Selected(object sender, TabControlEventArgs e)
        {
            TabPage tab = e.TabPage;
            if (tab != null)
            {
                if ((tab == tabPagePanorama) || (tab == tabPageObsolete))
                {
                    InitStoriesTab(tab);
                }
                else if (tab == tabPageKeyTerms)
                {
                    Cursor = Cursors.WaitCursor;
                    if (_biblicalTerms == null)
                    {
                        if (AnchorControl.m_dlgKeyTerms != null)
                        {
                            if (AnchorControl.m_dlgKeyTerms._biblicalTerms != null)
                                _biblicalTerms = AnchorControl.m_dlgKeyTerms._biblicalTerms;
                                
                            if (AnchorControl.m_dlgKeyTerms.renderings != null)
                                renderings = AnchorControl.m_dlgKeyTerms.renderings;

                            if (AnchorControl.m_dlgKeyTerms.termLocalizations != null)
                                termLocalizations = AnchorControl.m_dlgKeyTerms.termLocalizations;
                        }

                        dataGridViewKeyTerms.Columns[CnColumnRenderings].DefaultCellStyle.Font = MainLang.FontToUse;
                    }

                    if (_biblicalTerms == null)
                        _biblicalTerms = BiblicalTermsList.GetBiblicalTerms(_storyProject.ProjSettings.ProjectFolder);

                    if (renderings == null)
                        renderings = TermRenderingsList.GetTermRenderings(_storyProject.ProjSettings.ProjectFolder, 
                                                                          MainLang.LangCode);

                    if (termLocalizations == null)
                        termLocalizations = TermLocalizations.Localizations;

                    dataGridViewKeyTerms.Rows.Clear();
                    foreach (TermRendering tr in renderings.Renderings)
                    {
                        if (tr.RenderingsList.Count > 0)
                        {
                            Term term = _biblicalTerms.GetIfPresent(tr.Id);
                            if (term != null)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("Gloss: '{0}', Renderings: '{1}', Notes: '{2}'",
                                    term.Gloss, tr.Renderings, tr.Notes));

                                int nRow = dataGridViewKeyTerms.Rows.Add(new[] { term.Gloss, tr.Renderings, tr.Notes });
                                dataGridViewKeyTerms.Rows[nRow].Tag = tr.Id;
                                dataGridViewKeyTerms.Rows[nRow].Cells[CnColumnNotes].ToolTipText = "Click on the row header or double-click here to edit the renderings and/or notes for the selected key term";
                            }
                        }
                    }
                    Cursor = Cursors.Default;
                }
            }
        }

        private void InitStoriesTab(TabPage tab)
        {
            if (tab == tabPagePanorama)
            {
                _stories = _storyProject[OseResources.Properties.Resources.IDS_MainStoriesSet];
                toolTip.SetToolTip(buttonCopyToOldStories, Properties.Resources.IDS_PanoramaViewCopyToOldStories);
            }
            else if (tab == tabPageObsolete)
            {
                _stories = _storyProject[OseResources.Properties.Resources.IDS_ObsoleteStoriesSet];
                toolTip.SetToolTip(buttonCopyToOldStories, Properties.Resources.IDS_PanoramaViewCopyBackToStories);
            }
            InitParentTab(tab);
            InitGrid();
        }

        protected void InitParentTab(TabPage tab)
        {
            tableLayoutPanel.Parent = tab;
            // buttonCopyToOldStories.Enabled = (tab != tabPageObsolete);
        }

        private void richTextBoxPanoramaFrontMatter_TextChanged(object sender, EventArgs e)
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

        private void PanoramaView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (renderings != null)
                renderings.PromptForSave(_storyProject.ProjSettings.ProjectFolder);

            Properties.Settings.Default.PanoramaViewDlgLocation = Location;
            Properties.Settings.Default.PanoramaViewDlgHeight = Bounds.Height;
            Properties.Settings.Default.PanoramaViewDlgWidth = Bounds.Width;
            Properties.Settings.Default.Save();
        }

        private void dataGridViewPanorama_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || ((e.ColumnIndex < CnColumnStoryEditToken) || e.ColumnIndex > CnColumnNumOfWords))
                return;

            DataGridViewRow theRow = dataGridViewPanorama.Rows[e.RowIndex];
            DataGridViewCell theNameCell = theRow.Cells[CnColumnStoryName];
            if (theNameCell.Value == null)
                return; // shouldn't happen, but...

            var strName = theNameCell.Value as String;
            StoryData theSD = _stories.GetStoryFromName(strName);
            if (theSD == null)
                return;

            if (!theSD.TransitionHistory.HasData)
            {
                MessageBox.Show(Properties.Resources.IDS_NoTransitionHistory,
                                OseResources.Properties.Resources.IDS_Caption);
                return;
            }

            var dlg = new TransitionHistoryForm(theSD.TransitionHistory, _storyProject.TeamMembers);
            dlg.ShowDialog();
        }

        #region obsolete code
        /*
        protected DataGridViewRow m_rowLast = null;

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
