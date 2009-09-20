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
    public partial class PanoramaView : Form
    {
        protected const int CnColumnStoryName = 0;
        protected const int CnColumnStoryPurpose = 1;
        protected const int CnColumnStoryEditToken = 2;
        protected const int CnColumnStoryStage = 3;

        protected StoriesData _stories = null;

        public PanoramaView(StoriesData stories)
        {
            _stories = stories;
            InitializeComponent();
            InitGrid();
        }

        public bool Modified = false;

        protected void InitGrid()
        {
            dataGridViewPanorama.Rows.Clear();
            foreach (StoryData aSD in _stories)
            {
                StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[aSD.ProjStage.ProjectStage];
                object[] aObs = new object[] { aSD.Name, aSD.CraftingInfo.StoryPurpose, 
                    TeamMemberData.GetMemberTypeAsDisplayString(aSD.ProjStage.MemberTypeWithEditToken), 
                    st.StageDisplayString };
                int nRowIndex = dataGridViewPanorama.Rows.Add(aObs);
                DataGridViewRow aRow = dataGridViewPanorama.Rows[nRowIndex];
                aRow.Tag = st;
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedCells.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedCells.Count != 1)
                return;

            int nSelectedRowIndex = dataGridViewPanorama.SelectedCells[0].RowIndex;
            if (nSelectedRowIndex < dataGridViewPanorama.Rows.Count - 1)
            {
                StoryData theSDToDelete = _stories[nSelectedRowIndex];
                _stories.RemoveAt(nSelectedRowIndex);
                InitGrid();
                if (nSelectedRowIndex >= dataGridViewPanorama.Rows.Count)
                    nSelectedRowIndex--;

                dataGridViewPanorama.Rows[nSelectedRowIndex].Selected = true;
                Modified = true;
            }
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
