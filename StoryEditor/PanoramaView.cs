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
        protected const int CnColumnStoryStage = 2;
        protected const int CnColumnStoryEditToken = 3;

        protected DateTime m_dtStarted = DateTime.Now;
        TimeSpan m_timeMinStartup = new TimeSpan(0, 0, 3);

        protected StoriesData _stories = null;

        public PanoramaView(StoriesData stories)
        {
            _stories = stories;
            InitializeComponent();

            foreach (StoryData aSD in _stories)
            {
                object[] aObs = new object[4] { aSD.StoryName, aSD.CraftingInfo.StoryPurpose, null, TeamMemberData.GetMemberTypeAsString(aSD.ProjStage.WhoHasTheEditToken) };
                int nIndex = dataGridViewPanorama.Rows.Add(aObs);
                DataGridViewCell theStageCell = dataGridViewPanorama.Rows[nIndex].Cells[CnColumnStoryStage];
                System.Diagnostics.Debug.Assert(theStageCell is DataGridViewComboBoxCell);
                theStageCell.Value = 2;
            }
        }

        private void contextMenuStripProjectStages_Opening(object sender, CancelEventArgs e)
        {
            string strStoryStageDisplayString = (string)m_rowLastClicked.Cells[CnColumnStoryStage].Value;
            StoryStageLogic.ProjectStages theCurrentStage = StoryStageLogic.ProjectStages.eUndefined;
            foreach (KeyValuePair<StoryStageLogic.ProjectStages, string> kvp in StoryStageLogic.CmapStageToDisplayString)
                if (kvp.Value == strStoryStageDisplayString)
                {
                    theCurrentStage = kvp.Key;
                    break;
                }

            foreach (KeyValuePair<StoryStageLogic.ProjectStages, string> kvp in StoryStageLogic.CmapStageToDisplayString)
            {
                ToolStripItem tsi = contextMenuStripProjectStages.Items.Add(kvp.Value);
                tsi.Enabled = StoryStageLogic.IsValidTransition(theCurrentStage, kvp.Key);
            }
        }

        DataGridViewRow m_rowLastClicked = null;

        private void dataGridViewPanorama_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // prevent the false click that occurs when the user chooses a menu item
            if ((DateTime.Now - m_dtStarted) < m_timeMinStartup)
                return;

            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewPanorama.Rows.Count)
                || (e.ColumnIndex < 1) || (e.ColumnIndex >= dataGridViewPanorama.Columns.Count))
                return;

            m_rowLastClicked = dataGridViewPanorama.Rows[e.RowIndex];
        }
    }
}
