using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SelectStateBasicForm : Form
    {
        private StoryProjectData _storyProjectData;
        private StoryData _theCurrentStory;

        protected StoryStageLogic.ProjectStages _nextState = StoryStageLogic.ProjectStages.eUndefined;
        public StoryStageLogic.ProjectStages NextState
        {
            get { return _nextState; }
            set { _nextState = value; }
        }

        public bool ViewStateChanged { get; set; }
        
        public SelectStateBasicForm(StoryProjectData storyProjectData, StoryData theCurrentStory)
        {
            InitializeComponent();
            _storyProjectData = storyProjectData;
            _theCurrentStory = theCurrentStory;

            Font fontDisplay = new Font(Properties.Settings.Default.StateTransitionFontName,
                                        Properties.Settings.Default.StateTransitionFontSize);
            labelClickTo.Font = fontDisplay;
            InitButtons(theCurrentStory.ProjStage.ProjectStage, fontDisplay);
        }

        protected void InitButtons(StoryStageLogic.ProjectStages eCurrentState, Font fontDisplay)
        {
            StoryStageLogic.StateTransition stateTransition = StateTransitions[eCurrentState];

            CheckAllowableTransitions(_storyProjectData, _theCurrentStory,
                                      stateTransition.AllowableForewardsTransitions,
                                      fontDisplay,
                                      true);

            CheckAllowableTransitions(_storyProjectData, _theCurrentStory,
                                      stateTransition.AllowableBackwardsTransitions,
                                      fontDisplay,
                                      false);

            int nWidth = 0;
            int nHeight = 0;
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                nWidth = Math.Max(nWidth, ctrl.Width);
                nHeight += ctrl.Height + ctrl.Margin.Vertical;
            }

            int nPadding = 26;
            nWidth += nPadding;
            nHeight += (nPadding*2) + labelClickTo.Height + buttonAdvanced.Height;
            ClientSize = new Size(nWidth, nHeight);
        }

        protected void CheckAllowableTransitions(StoryProjectData storyProjectData, 
            StoryData theCurrentStory, StoryStageLogic.AllowableTransitions allowableTransitions,
            Font fontDisplay, bool bForwardTransition)
        {
            // allowed transition states (could be previous or forward)
            foreach (StoryStageLogic.AllowableTransition aps in allowableTransitions)
            {
                // see if this transition is allowed from our current situation
                if (aps.IsThisTransitionAllow(storyProjectData, theCurrentStory))
                {
                    Debug.Assert(StateTransitions.ContainsKey(aps.ProjectStage));
                    StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[aps.ProjectStage];

                    InitButton(st.TransitionDisplayString, st.StageDisplayString, aps.ProjectStage, fontDisplay);

                    // for the forward transitions, we only need one to succeed (we don't really 
                    //  want users to skip any)
                    if (bForwardTransition && !aps.AllowToSkip)
                        break;  // we only need one forward
                }
            }
        }

        protected void InitButton(string strDisplayString, string strStageDescription, StoryStageLogic.ProjectStages eNextState, Font fontDisplay)
        {
            var btn = new Button
            {
                Name = strDisplayString.Replace(' ', '_'),
                AutoSize = true,
                TabIndex = flowLayoutPanel1.Controls.Count,
                Text = strDisplayString,
                UseVisualStyleBackColor = true,
                Tag = eNextState,
                Font = fontDisplay
            };
            btn.Click += clickUserSelectedState;
            flowLayoutPanel1.Controls.Add(btn);
        }

        void clickUserSelectedState(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            var eStateChosen = (StoryStageLogic.ProjectStages) btn.Tag;
            if (_theCurrentStory.ProjStage.ProjectStage != eStateChosen)
            {
                NextState = eStateChosen;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected StoryStageLogic.StateTransitions StateTransitions
        {
            get
            {
                return StoryStageLogic.stateTransitions;
            }
        }

        private void buttonAdvanced_Click(object sender, EventArgs e)
        {
            var dlg = new StageEditorForm(_storyProjectData, _theCurrentStory, new Point(20, 20), false);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Debug.Assert(dlg.NextState != StoryStageLogic.ProjectStages.eUndefined);
                if (_theCurrentStory.ProjStage.ProjectStage != dlg.NextState)
                {
                    NextState = dlg.NextState;
                    ViewStateChanged = dlg.ViewStateChanged;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void labelClickTo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    Font fontDisplay = fontDialog.Font;
                    labelClickTo.Font = fontDisplay;
                    foreach (Control control in flowLayoutPanel1.Controls)
                        control.Font = fontDisplay;
                    Properties.Settings.Default.StateTransitionFontName = fontDisplay.Name;
                    Properties.Settings.Default.StateTransitionFontSize = fontDisplay.Size;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
