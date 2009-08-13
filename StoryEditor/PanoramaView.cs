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

            foreach (StoryData aSD in _stories)
            {
                StoryStageLogic.StageTransition st = StoryStageLogic.stateTransitions[aSD.ProjStage.ProjectStage];
                object[] aObs = new object[] { aSD.StoryName, aSD.CraftingInfo.StoryPurpose, 
                    TeamMemberData.GetMemberTypeAsString(aSD.ProjStage.MemberTypeWithEditToken), 
                    st.StageDisplayString };
                int nRowIndex = dataGridViewPanorama.Rows.Add(aObs);
                DataGridViewRow aRow = dataGridViewPanorama.Rows[nRowIndex];
                aRow.Tag = st;
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedRows.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedRows.Count != 1)
                return;

            DataGridViewRow theSelectedRow = dataGridViewPanorama.SelectedRows[0];
            int nSelectedRowIndex = theSelectedRow.Index;
            if (nSelectedRowIndex > 0)
            {
                dataGridViewPanorama.Rows.RemoveAt(nSelectedRowIndex);
                dataGridViewPanorama.Rows.Insert(--nSelectedRowIndex, theSelectedRow);
                theSelectedRow.Selected = true;
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(dataGridViewPanorama.SelectedRows.Count < 2);   // 1 or 0
            if (dataGridViewPanorama.SelectedRows.Count != 1)
                return;

            DataGridViewRow theSelectedRow = dataGridViewPanorama.SelectedRows[0];
            int nSelectedRowIndex = theSelectedRow.Index;
            if (nSelectedRowIndex < dataGridViewPanorama.Rows.Count - 1)
            {
                dataGridViewPanorama.Rows.RemoveAt(nSelectedRowIndex);
                dataGridViewPanorama.Rows.Insert(++nSelectedRowIndex, theSelectedRow);
                theSelectedRow.Selected = true;
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
            StoryStageLogic.StageTransition theCurrentST = (StoryStageLogic.StageTransition)m_rowLast.Tag;
            if (theCurrentST == null)
                return;

            foreach (StoryStageLogic.ProjectStages eAllowableTransition in theCurrentST.AllowableTransitions)
            {
                StoryStageLogic.StageTransition aST = StoryStageLogic.stateTransitions[eAllowableTransition];
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

            StoryStageLogic.StageTransition theCurrentST = (StoryStageLogic.StageTransition)tsi.Tag;
            if (theCurrentST == null)
                return;

            m_rowLast.Cells[CnColumnStoryStage].Value = theCurrentST.StageDisplayString;

            string strStoryName = (string)theCurrentST.Cells[CnColumnStoryName].Value;
            foreach (StoryData aSD in _stories)
                if (aSD.StoryName == strStoryName)
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
