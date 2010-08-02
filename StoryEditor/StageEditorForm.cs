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
        protected List<DataGridViewButtonCell> _lstOfAllowedTransitions = new List<DataGridViewButtonCell>();

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
                }

                // check allowable next state
                CheckAllowableTransitions(storyProjectData, theCurrentStory, st.AllowableForewardsTransitions, Color.LightGreen, true);

                // allowed previous states
                CheckAllowableTransitions(storyProjectData, theCurrentStory, st.AllowableBackwardsTransitions, Color.Red, false);
            }

            // locate the window near the cursor...
            // but make sure it doesn't go off the edge
            Point ptTooltip = Cursor.Position;
            Rectangle rectScreen = Screen.GetBounds(ptTooltip);
            if (Width > rectScreen.Width - ptStatusBar.X)
            {
                if (Width < rectScreen.Width)
                    ptStatusBar.X = rectScreen.Width - Width;   // move it more left to fit
                else
                    ptStatusBar.X = 0;  // make it go all the way to the left
                Width = Math.Min(Width, rectScreen.Width);
            }

            Height = Math.Min(dataGridViewStates.PreferredSize.Height 
                + statusStrip.Height 
                + SystemInformation.HorizontalScrollBarHeight,
                rectScreen.Height - ptStatusBar.Y);
            Location = ptStatusBar;
            _ticksSinceShow = DateTime.Now.Ticks;
        }

        protected void CheckAllowableTransitions(StoryProjectData storyProjectData, StoryData theCurrentStory, 
            StoryStageLogic.AllowableTransitions allowableTransitions, Color clr, bool bForwardTransition)
        {
            // allowed transition states (could be previous or forward)
            foreach (StoryStageLogic.AllowableTransition aps in allowableTransitions)
            {
                // see if this transition is allowed from our current situation
                if (aps.IsThisTransitionAllow(storyProjectData, theCurrentStory))
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
                        _lstOfAllowedTransitions.Add(dgbc);

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

        public bool ViewStateChanged { get; set; }

        protected void InitGrid()
        {
            // clear out the previous contents
            dataGridViewStates.Rows.Clear();

            // now populate the grid from the StateTransitions (whether default or specialized)
            foreach (StoryStageLogic.StateTransition stateTransition in StateTransitions.Values)
            {
                // see if this state is allowed (whether it's a valid transition or not) given
                //  our current configuration
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
                        // could be an independent consultant as well (because that invalid case of
                        //  the state being for a CIT while having no "manage with coaching" in
                        //  the project settings was already excluded by the call to IsThisStateAllow above)
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
            var dgbc = dataGridViewStates.Rows[nIndex].Cells[strColumnName] as DataGridViewButtonCell;
            InitCellFromStateTransition(dgbc, stateTransition);
            _mapStatesToButtons.Add(stateTransition.CurrentStage, dgbc);
        }

        protected void InitCellFromStateTransition(DataGridViewButtonCell theCell, StoryStageLogic.StateTransition stateTransition)
        {
            theCell.Value = stateTransition.StageDisplayString;
            theCell.ToolTipText = stateTransition.StageDisplayString;
            theCell.Tag = stateTransition;
        }

        protected StoryStageLogic.StateTransitions StateTransitions
        {
            get
            {
                return StoryStageLogic.stateTransitions;
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

        const long m_ticksMinForClick = 3000000;

        private void dataGridViewStates_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
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

            if (e.Button == MouseButtons.Left)
            {
                // only those that were legitimate next or previous states will be in the _lstOfAllowedTransitions
                if (_lstOfAllowedTransitions.Contains(theCell))
                {
                    StoryStageLogic.StateTransition st = theCell.Tag as StoryStageLogic.StateTransition;
                    if (st != null)
                    {
                        NextState = st.CurrentStage;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // right-click means edit the state information
                StoryStageLogic.StateTransition st = theCell.Tag as StoryStageLogic.StateTransition;
                if (st != null)
                {
                    StateEditorForm dlg = new StateEditorForm(st);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        ViewStateChanged |= dlg.ViewStateChanged;
                        InitCellFromStateTransition(theCell, st);
                        StoryStageLogic.stateTransitions.SaveCustomStateTransitionsXmlFile();
                    }
                }
            }
        }

        private void dataGridViewStates_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            // make sure we have something reasonable
            if (((e.ColumnIndex < 0) || (e.ColumnIndex >= dataGridViewStates.Columns.Count))
                || (e.RowIndex < 0) || (e.RowIndex >= dataGridViewStates.Rows.Count))
                return;

            DataGridViewRow theRow = dataGridViewStates.Rows[e.RowIndex];
            DataGridViewButtonCell theCell = theRow.Cells[e.ColumnIndex] as DataGridViewButtonCell;
            if (theCell == null)
                return;

            // update the status bar with the state and F1 instructions
            StoryStageLogic.StateTransition st = theCell.Tag as StoryStageLogic.StateTransition;
            if (st != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("dataGridViewStates_CellMouseEnter: cell: {0}.{1}: {2}",
                    theCell.ColumnIndex, theCell.RowIndex, st.StageDisplayString));

                toolStripStatusLabel.Text = String.Format(Properties.Resources.IDS_PressF1ForInstructions,
                    st.StageDisplayString);
            }
        }

        private void dataGridViewStates_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Point ptMousePosition = dataGridViewStates.PointToClient(hlpevent.MousePos);
            DataGridView.HitTestInfo hti = dataGridViewStates.HitTest(ptMousePosition.X, ptMousePosition.Y);

            // make sure we have something reasonable
            if (((hti.ColumnIndex < 0) || (hti.ColumnIndex >= dataGridViewStates.Columns.Count))
                || (hti.RowIndex < 0) || (hti.RowIndex >= dataGridViewStates.Rows.Count))
                return;

            DataGridViewRow theRow = dataGridViewStates.Rows[hti.RowIndex];
            DataGridViewButtonCell theCell = theRow.Cells[hti.ColumnIndex] as DataGridViewButtonCell;
            if (theCell == null)
                return;

            // update the status bar with the state and F1 instructions
            StoryStageLogic.StateTransition st = theCell.Tag as StoryStageLogic.StateTransition;
            if (st != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("dataGridViewStates_HelpRequested: cell: {0}.{1}: {2}",
                    theCell.ColumnIndex, theCell.RowIndex, st.StageDisplayString));

                helpProvider.ResetShowHelp(dataGridViewStates);
                Help.ShowPopup(dataGridViewStates, String.Format("{1}{0}{2}{0}{3}", Environment.NewLine,
                    st.StageDisplayString, st.StageInstructions, Properties.Resources.IDS_EscapeToDismiss), 
                    hlpevent.MousePos);
                hlpevent.Handled = true;
            }
        }
    }
}
