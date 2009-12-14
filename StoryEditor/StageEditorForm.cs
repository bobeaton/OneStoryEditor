using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StageEditorForm : Form
    {
        protected const string CstrEditState = "Edit State";
        protected const string CstrDeleteState = "Delete State";
        protected const string CstrAddState = "Add New State";

        protected StoryProjectData _storyProjectData;
        public StageEditorForm(StoryProjectData storyProjectData)
        {
            InitializeComponent();
            _storyProjectData = storyProjectData;

            if (!storyProjectData.TeamMembers.HasOutsideEnglishBTer)
                ColumnEnglishBackTranslator.Visible = false;

            if (!storyProjectData.TeamMembers.HasFirstPassMentor)
                ColumnFirstPassMentor.Visible = false;

            if (storyProjectData.TeamMembers.HasIndependentConsultant)
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrIndependentConsultantDisplay;
                ColumnCoach.Visible = false;
            }

            InitGrid();
        }

        protected void InitGrid()
        {
            // clear out the previous contents
            dataGridViewStates.Rows.Clear();

            // now populate the grid from the StateTransitions (whether default or specialized)
            foreach (StoryStageLogic.StateTransition stateTransition in StateTransitions.Values)
            {
                switch (stateTransition.MemberTypeWithEditToken)
                {
                    case TeamMemberData.UserTypes.eProjectFacilitator:
                        InitComboBox(stateTransition, ColumnProjectFacilitatorStages.Name);
                        break;
                    case TeamMemberData.UserTypes.eFirstPassMentor:
                        if (ColumnFirstPassMentor.Visible)
                        {
                            InitComboBox(stateTransition, ColumnFirstPassMentor.Name);
                        }
                        break;
                    case TeamMemberData.UserTypes.eConsultantInTraining:
                        InitComboBox(stateTransition, ColumnConsultantInTraining.Name);
                        break;
                    case TeamMemberData.UserTypes.eCoach:
                        if (ColumnCoach.Visible)
                        {
                            InitComboBox(stateTransition, ColumnCoach.Name);
                        }
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
            }
        }
        protected void InitComboBox(StoryStageLogic.StateTransition stateTransition,
            string strColumnName)
        {
            int nIndex = dataGridViewStates.Rows.Add();
            var dgcb = (DataGridViewComboBoxCell)
                dataGridViewStates.Rows[nIndex].Cells[strColumnName];
            dgcb.Items.Add(stateTransition.StageDisplayString);
            dgcb.Items.Add(CstrEditState);
            dgcb.Items.Add(CstrDeleteState);
            dgcb.Items.Add(CstrAddState);
            dgcb.Value = stateTransition.StageDisplayString;
        }

        protected StoryStageLogic.StateTransitions StateTransitions
        {
            get
            {
                return StoryStageLogic.stateTransitions;
            }
        }

        private void checkBoxOutsideEnglishBackTranslator_CheckedChanged(object sender, System.EventArgs e)
        {
            ColumnEnglishBackTranslator.Visible = checkBoxOutsideEnglishBackTranslator.Checked;

            if (checkBoxOutsideEnglishBackTranslator.Checked 
                && !_storyProjectData.IsASeparateEnglishBackTranslator)
            {
                // if this user is saying that there's an external BTer, but there doesn't
                //  appear to be one, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.eEnglishBacktranslator)
                                       {
                                           Text = "Choose the member that will do English BTs"
                                       };
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // noop
            }

            InitGrid();
        }

        private void checkBoxFirstPassMentor_CheckedChanged(object sender, System.EventArgs e)
        {
            ColumnFirstPassMentor.Visible = checkBoxFirstPassMentor.Checked;

            if (checkBoxFirstPassMentor.Checked
                && !_storyProjectData.TeamMembers.IsThereAFirstPassMentor)
            {
                // if this user is saying that there's a first pass mentor, but there doesn't
                //  appear to be one, then query for it.
                var dlg = new MemberPicker(_storyProjectData, TeamMemberData.UserTypes.eFirstPassMentor)
                {
                    Text = "Choose the member that is the first-pass mentor"
                };
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // noop
            }

            InitGrid();
        }

        private void radioButtonManageWithCoaching_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonManageWithCoaching.Checked)
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrConsultantInTrainingDisplay;
                ColumnCoach.Visible = true;
            }
            else
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrIndependentConsultantDisplay;
                ColumnCoach.Visible = false;
            }

            InitGrid();
        }

        private void dataGridViewStates_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewStates_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewStates_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
