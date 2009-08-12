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
                object[] aObs = new object[4] { aSD.StoryName, aSD.CraftingInfo.StoryPurpose, 
                    TeamMemberData.GetMemberTypeAsString(aSD.ProjStage.MemberTypeWithEditToken), 
                    st.StageDisplayString };
                dataGridViewPanorama.Rows.Add(aObs);
            }
        }

        private void contextMenuStripProjectStages_Opening(object sender, CancelEventArgs e)
        {
            if (m_rowLastClicked == null)
                return;

            contextMenuStripProjectStages.Items.Clear();
            string strStoryStageDisplayString = (string)m_rowLastClicked.Cells[CnColumnStoryStage].Value;
            StoryStageLogic.ProjectStages eStage;
            StoryStageLogic.StageTransition theCurrentStageTransition =
                GetTransitionInfoForRow(strStoryStageDisplayString, out eStage);

            if (theCurrentStageTransition == null)
                return;

            foreach (KeyValuePair<StoryStageLogic.ProjectStages, StoryStageLogic.StageTransition> kvp
                in StoryStageLogic.stateTransitions)
            {
                ToolStripItem tsi = contextMenuStripProjectStages.Items.Add(
                    kvp.Value.StageDisplayString,
                    null,
                    new EventHandler(OnSelectNextState));
                tsi.Enabled = theCurrentStageTransition.AllowableTransitions.Contains(kvp.Key);
            }
        }

        protected void OnSelectNextState(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripItem);
            ToolStripItem tsi = (ToolStripItem)sender;

            StoryStageLogic.ProjectStages eStage;
            StoryStageLogic.StageTransition theCurrentStageTransition =
                GetTransitionInfoForRow(tsi.Text, out eStage);

            if (theCurrentStageTransition == null)
                return;

            m_rowLastClicked.Cells[CnColumnStoryStage].Value = theCurrentStageTransition.StageDisplayString;

            string strStoryName = (string)m_rowLastClicked.Cells[CnColumnStoryName].Value;
            foreach (StoryData aSD in _stories)
                if (aSD.StoryName == strStoryName)
                {
                    GetTransitionInfoForRow(theCurrentStageTransition.StageDisplayString, out eStage);
                    aSD.ProjStage.ProjectStage = eStage;
                    break;
                }
        }

        protected StoryStageLogic.StageTransition GetTransitionInfoForRow(string strStoryStageDisplayString,
            out StoryStageLogic.ProjectStages eStage)
        {
            foreach (KeyValuePair<StoryStageLogic.ProjectStages, StoryStageLogic.StageTransition> kvp
                in StoryStageLogic.stateTransitions)
                if (kvp.Value.StageDisplayString == strStoryStageDisplayString)
                {
                    eStage = kvp.Key;
                    return kvp.Value;
                }

            eStage = StoryStageLogic.ProjectStages.eUndefined;
            return null;
        }

        DataGridViewRow m_rowLastClicked = null;
        /*
        private void dataGridViewPanorama_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || (e.ColumnIndex < 1) || (e.ColumnIndex >= dataGridViewPanorama.Columns.Count))
                return;

            m_rowLastClicked = dataGridViewPanorama.Rows[e.RowIndex];
        }
        */
        private void dataGridViewPanorama_SelectionChanged(object sender, EventArgs e)
        {
            // should only be one...
            if (dataGridViewPanorama.SelectedRows.Count > 0)
                m_rowLastClicked = dataGridViewPanorama.SelectedRows[0];
        }
    }
}
