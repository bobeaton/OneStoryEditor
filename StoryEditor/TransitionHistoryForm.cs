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
    public partial class TransitionHistoryForm : TopForm
    {
        public TransitionHistoryForm(IEnumerable<StoryStateTransition> theHistory, TeamMembersData teamMembersData)
            : base(true)
        {
            InitializeComponent();

            foreach (StoryStateTransition stateTransition in theHistory)
            {
                string strOSUser = teamMembersData.GetNameFromMemberId(stateTransition.LoggedInMemberId);
                var aObs = new object[]
                                    {
                                        stateTransition.TransitionDateTime,
                                        stateTransition.WindowsUserName,
                                        strOSUser,
                                        StoryStageLogic.stateTransitions[stateTransition.FromState].StageDisplayString,
                                        StoryStageLogic.stateTransitions[stateTransition.ToState].StageDisplayString
                                    };
                dataGridView.Rows.Add(aObs);
            }
        }
    }
}
