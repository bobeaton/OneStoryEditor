using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class TransitionHistoryForm : TopForm
    {
        private IEnumerable<StoryStateTransition> _theHistory;
        private TeamMembersData _teamMembersData;

        private TransitionHistoryForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public TransitionHistoryForm(IEnumerable<StoryStateTransition> theHistory, TeamMembersData teamMembersData)
            : base(true)
        {
            _theHistory = theHistory;
            _teamMembersData = teamMembersData;

            InitializeComponent();
            Localizer.Ctrl(this);

            foreach (var stateTransition in theHistory)
            {
                var strOsUser = teamMembersData.GetNameFromMemberId(stateTransition.LoggedInMemberId);
                var aObs = new object[]
                                    {
                                        stateTransition.TransitionDateTime,
                                        stateTransition.WindowsUserName,
                                        strOsUser,
                                        StoryStageLogic.stateTransitions[stateTransition.FromState].StageDisplayString,
                                        StoryStageLogic.stateTransitions[stateTransition.ToState].StageDisplayString
                                    };
                dataGridView.Rows.Add(aObs);
                showGraphicalView.Enabled = true;
            }
        }

        private void ShowGraphicalViewClick(object sender, EventArgs e)
        {
            var dlg = new StateTransitionHistoryGraphForm(_theHistory, _teamMembersData);
            dlg.ShowDialog();
        }
    }
}
