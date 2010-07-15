using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StageEditorForm : Form
    {
        protected const string CstrEditState = "Edit State";
        protected const string CstrDeleteState = "Delete State";
        protected const string CstrAddState = "Add New State";

        protected StoryProjectData _storyProjectData;
        protected StoryData _theCurrentStory;
        public StageEditorForm(StoryProjectData storyProjectData, StoryData theCurrentStory)
        {
            InitializeComponent();
            _storyProjectData = storyProjectData;
            _theCurrentStory = theCurrentStory;

            if (!storyProjectData.TeamMembers.HasOutsideEnglishBTer)
                ColumnEnglishBackTranslator.Visible = false;
            else
                checkBoxOutsideEnglishBackTranslator.Checked = true;

            if (!storyProjectData.TeamMembers.HasFirstPassMentor)
                ColumnFirstPassMentor.Visible = false;
            else
                checkBoxFirstPassMentor.Checked = true;

            if (storyProjectData.TeamMembers.HasIndependentConsultant)
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrIndependentConsultantDisplay;
                ColumnCoach.Visible = false;
                radioButtonIndependentConsultant.Checked = true;
            }
            else
                radioButtonManageWithCoaching.Checked = true;

            InitGrid();
        }

        protected void InitGrid()
        {
            // clear out the previous contents
            dataGridViewStates.Rows.Clear();

            // now populate the grid from the StateTransitions (whether default or specialized)
            foreach (StoryStageLogic.StateTransition stateTransition in StateTransitions.Values)
            {
                if ((stateTransition.RequiresBiblicalStory && !_theCurrentStory.CraftingInfo.IsBiblicalStory)
                    || (stateTransition.RequiresNonBiblicalStory && _theCurrentStory.CraftingInfo.IsBiblicalStory))
                    continue;

                switch (stateTransition.MemberTypeWithEditToken)
                {
                    case TeamMemberData.UserTypes.eProjectFacilitator:
                        InitComboBox(stateTransition, ColumnProjectFacilitatorStages.Name);
                        break;
                    case TeamMemberData.UserTypes.eEnglishBacktranslator:
                        if (ColumnEnglishBackTranslator.Visible)
                        {
                            InitComboBox(stateTransition, ColumnEnglishBackTranslator.Name);
                        }
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
                    case TeamMemberData.UserTypes.eIndependentConsultant:
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
                {
                    checkBoxOutsideEnglishBackTranslator.Checked = false;
                    return;
                }

                // noop
            }

            ColumnEnglishBackTranslator.Visible = checkBoxOutsideEnglishBackTranslator.Checked;
            InitGrid();
        }

        private void checkBoxFirstPassMentor_CheckedChanged(object sender, System.EventArgs e)
        {
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
                {
                    checkBoxFirstPassMentor.Checked = false;
                    return;
                }

                // noop
            }

            ColumnFirstPassMentor.Visible = checkBoxFirstPassMentor.Checked;
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
