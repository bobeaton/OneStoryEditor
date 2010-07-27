using System;
using System.Collections.Generic;
using System.Drawing;
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

        protected long _ticksSinceShow;

        protected Dictionary<StoryStageLogic.ProjectStages, DataGridViewButtonCell> _mapStatesToButtons
            = new Dictionary<StoryStageLogic.ProjectStages, DataGridViewButtonCell>();

        public StageEditorForm(StoryProjectData storyProjectData, StoryData theCurrentStory, Point ptStatusBar)
        {
            InitializeComponent();

            _storyProjectData = storyProjectData;
            _theCurrentStory = theCurrentStory;

#if ShowViewStateWithDialog
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
#else
            ColumnEnglishBackTranslator.Visible = storyProjectData.TeamMembers.HasOutsideEnglishBTer;
            ColumnFirstPassMentor.Visible = storyProjectData.TeamMembers.HasFirstPassMentor;

            if (storyProjectData.TeamMembers.HasIndependentConsultant)
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrIndependentConsultantDisplay;
                ColumnCoach.Visible = false;
            }
            else
            {
                ColumnConsultantInTraining.HeaderText = TeamMemberData.CstrConsultantInTrainingDisplay;
                ColumnCoach.Visible = true;
            }
#endif
            InitGrid();

            // set the background of the special ones (i.e. the current one, and all valid next and prev states
            System.Diagnostics.Debug.Assert(StateTransitions.ContainsKey(theCurrentStory.ProjStage.ProjectStage));
            StoryStageLogic.StateTransition st = StateTransitions[theCurrentStory.ProjStage.ProjectStage];
            if (st != null)
            {
                // current state
                DataGridViewButtonCell dgbc;
                System.Diagnostics.Debug.Assert(_mapStatesToButtons.ContainsKey(st.CurrentStage));
                if (_mapStatesToButtons.TryGetValue(st.CurrentStage, out dgbc))
                {
                    dgbc.Value += " (current state)";
                    Font font = new Font(dataGridViewStates.Font, FontStyle.Bold);
                    dgbc.Style.Font = font;
                    dgbc.FlatStyle = FlatStyle.Popup;
                    dgbc.Style.BackColor = Color.Yellow;
                    dgbc.Tag = st;
                }

                // check allowable next state
                CheckAllowableTransitions(storyProjectData, theCurrentStory, st.AllowableForewardsTransitions, Color.LightGreen, true);

                // allowed previous states
                CheckAllowableTransitions(storyProjectData, theCurrentStory, st.AllowableBackwardsTransitions, Color.Red, false);
            }

            Height = dataGridViewStates.PreferredSize.Height;
            Location = new Point(ptStatusBar.X, ptStatusBar.Y - Height);
            _ticksSinceShow = DateTime.Now.Ticks;
        }

        protected void CheckAllowableTransitions(StoryProjectData storyProjectData, StoryData theCurrentStory, 
            StoryStageLogic.AllowableTransitions allowableTransitions, Color clr, bool bForwardTransition)
        {
            // allowed previous states
            foreach (StoryStageLogic.AllowableTransition aps in allowableTransitions)
            {
                // put the allowable transitions into the DropDown list
                if (aps.IsThisStateAllow(storyProjectData, theCurrentStory))
                {
                    System.Diagnostics.Debug.Assert(StateTransitions.ContainsKey(aps.ProjectStage));
                    StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[aps.ProjectStage];
                    System.Diagnostics.Debug.Assert(_mapStatesToButtons.ContainsKey(st.CurrentStage));
                    
                    DataGridViewButtonCell dgbc;
                    if (_mapStatesToButtons.TryGetValue(st.CurrentStage, out dgbc))
                    {
                        Font font = new Font(dataGridViewStates.Font, FontStyle.Bold);
                        dgbc.Style.Font = font;
                        dgbc.FlatStyle = FlatStyle.Popup;
                        dgbc.Style.BackColor = clr;
                        dgbc.Tag = st;

                        // for the forward transitions, we only need one to succeed (we don't really want
                        //  users to skip any)
                        if (bForwardTransition && !aps.AllowToSkip)
                            break;  // we only need one forward
                    }
                }
            }
        }

        protected StoryStageLogic.ProjectStages _nextState = StoryStageLogic.ProjectStages.eUndefined;
        public StoryStageLogic.ProjectStages NextState
        {
            get { return _nextState; }
            set { _nextState = value; }
        }

        protected void InitGrid()
        {
            // clear out the previous contents
            dataGridViewStates.Rows.Clear();

            // now populate the grid from the StateTransitions (whether default or specialized)
            foreach (StoryStageLogic.StateTransition stateTransition in StateTransitions.Values)
            {
                if (!stateTransition.IsThisStateAllow(_storyProjectData, _theCurrentStory))
                    continue;

                switch (stateTransition.MemberTypeWithEditToken)
                {
                    case TeamMemberData.UserTypes.eProjectFacilitator:
                        InitButton(stateTransition, ColumnProjectFacilitatorStages.Name);
                        break;
                    case TeamMemberData.UserTypes.eEnglishBacktranslator:
                        if (ColumnEnglishBackTranslator.Visible)
                        {
                            InitButton(stateTransition, ColumnEnglishBackTranslator.Name);
                        }
                        break;
                    case TeamMemberData.UserTypes.eFirstPassMentor:
                        if (ColumnFirstPassMentor.Visible)
                        {
                            InitButton(stateTransition, ColumnFirstPassMentor.Name);
                        }
                        break;
                    case TeamMemberData.UserTypes.eConsultantInTraining:
                        InitButton(stateTransition, ColumnConsultantInTraining.Name);
                        break;
                    case TeamMemberData.UserTypes.eIndependentConsultant:
                        InitButton(stateTransition, ColumnConsultantInTraining.Name);
                        break;
                    case TeamMemberData.UserTypes.eCoach:
                        if (ColumnCoach.Visible)
                        {
                            InitButton(stateTransition, ColumnCoach.Name);
                        }
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
            }
        }
        protected void InitButton(StoryStageLogic.StateTransition stateTransition,
            string strColumnName)
        {
            int nIndex = dataGridViewStates.Rows.Add();
            var dgb = dataGridViewStates.Rows[nIndex].Cells[strColumnName] as DataGridViewButtonCell;
            dgb.Value = stateTransition.StageDisplayString;
            dgb.ToolTipText = String.Format("{1}:{0}{2}",
                Environment.NewLine, stateTransition.StageDisplayString, stateTransition.StageInstructions);
            _mapStatesToButtons.Add(stateTransition.CurrentStage, dgb);
        }

        protected StoryStageLogic.StateTransitions StateTransitions
        {
            get
            {
                return StoryStageLogic.stateTransitions;
            }
        }

        const long m_ticksMinForClick = 3000000;

        private void dataGridViewStates_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // make sure we have something reasonable
            if (((e.ColumnIndex < 0) || (e.ColumnIndex >= dataGridViewStates.Columns.Count))
                || (e.RowIndex < 0) || (e.RowIndex >= dataGridViewStates.Rows.Count))
                return;

            // also make sure enough time has passed (because we seem to be getting this from the mouse
            //  up of having launched this dialog
            long lTickDiff = DateTime.Now.Ticks - _ticksSinceShow;
            System.Diagnostics.Debug.WriteLine(String.Format("Ticks since launch: {0}", lTickDiff));
            if (lTickDiff < m_ticksMinForClick)
                return;

            DataGridViewRow theRow = dataGridViewStates.Rows[e.RowIndex];
            DataGridViewButtonCell theCell = theRow.Cells[e.ColumnIndex] as DataGridViewButtonCell;
            if (theCell == null)
                return;

            // only those that were legitimate next or previous states will have their .Tag set
            StoryStageLogic.StateTransition st = theCell.Tag as StoryStageLogic.StateTransition;
            if (st != null)
            {
                NextState = st.CurrentStage;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void dataGridViewStates_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                NextState = StoryStageLogic.ProjectStages.eUndefined;
                Close();
            }
        }

#if ShowViewStateWithDialog
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
#endif
    }
}
