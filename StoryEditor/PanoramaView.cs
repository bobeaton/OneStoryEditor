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
        protected const int CnColumnStoryStage = 3;
        protected const int CnColumnStoryTimeInStage = 4;
        protected const int CnColumnNumOfLines = 5;
        protected const int CnColumnNumOfWords = 6;

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
                    _storyProject.TeamMembers.GetMemberTypeAsDisplayString(aSD.ProjStage.MemberTypeWithEditToken);

                if (strWhoHasEditToken == TeamMemberData.CstrProjectFacilitatorDisplay)
                {
                    strWhoHasEditToken = String.Format("{0} ({1})",
                                                       strWhoHasEditToken,
                                                       _storyProject.TeamMembers.GetNameFromMemberId(
                                                           aSD.CraftingInfo.ProjectFacilitatorMemberID));
                }

                StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[aSD.ProjStage.ProjectStage];
                object[] aObs = new object[]
                {   
                    aSD.Name, 
                    aSD.CraftingInfo.StoryPurpose, 
                    strWhoHasEditToken, 
                    st.StageDisplayString, 
                    strTimeInState,
                    aSD.NumOfLines,
                    aSD.NumOfWords(_storyProject.ProjSettings)
                };
                int nRowIndex = dataGridViewPanorama.Rows.Add(aObs);
                DataGridViewRow aRow = dataGridViewPanorama.Rows[nRowIndex];
                aRow.Tag = st;
                aRow.Height = _fontForDev.Height + 4;
            }
        }

        private void dataGridViewPanorama_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex > CnColumnStoryPurpose))
                return;

            DataGridViewRow theRow = dataGridViewPanorama.Rows[e.RowIndex];
            DataGridViewCell theCell = theRow.Cells[e.ColumnIndex];
            if (theCell.Value == null)
                return;

            string strCellValue = ((string)theCell.Value).Trim();
            if (String.IsNullOrEmpty(strCellValue))
                return;

            System.Diagnostics.Debug.Assert(theRow.Index < _stories.Count);
            StoryData theSD = _stories[theRow.Index];

            if (e.ColumnIndex == CnColumnStoryName)
            {
                if (theSD.Name != strCellValue)
                    theSD.Name = strCellValue;
                else
                    return; // return unModified
            }
            else
            {
                System.Diagnostics.Debug.Assert(e.ColumnIndex == CnColumnStoryPurpose);
                if (theSD.CraftingInfo.StoryPurpose != strCellValue)
                    theSD.CraftingInfo.StoryPurpose = strCellValue;
                else
                    return; // return unModified
            }

            Modified = true;
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
                StoryData theSDToMove = _stories[nSelectedRowIndex];
                _stories.RemoveAt(nSelectedRowIndex);
                _stories.Insert(--nSelectedRowIndex, theSDToMove);
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
                StoryData theSDToMove = _stories[nSelectedRowIndex];
                _stories.RemoveAt(nSelectedRowIndex);
                _stories.Insert(++nSelectedRowIndex, theSDToMove);
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
                StoryData theSD = new StoryData(_stories[nSelectedRowIndex]);
                int n = 1;
                if (_storyProject[strDestSet].Contains(theSD))
                {
                    string strName = theSD.Name;
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
                // make sure the user really wants to do this
                if (MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmDeleteStory,
                    dataGridViewPanorama.Rows[nSelectedRowIndex].Cells[CnColumnStoryName].Value), 
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel)
                    != DialogResult.Yes)
                    return;

                _stories.RemoveAt(nSelectedRowIndex);
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
            StoryData theSD = _stories[theRow.Index];

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
