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
    public partial class StateTransitionHistoryGraphForm : Form
    {
        private StateTransitionHistoryGraphForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public StateTransitionHistoryGraphForm(IEnumerable<StoryStateTransition> theHistory, TeamMembersData teamMembersData)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            TimeSpan tsPf = new TimeSpan(), tsCit = new TimeSpan(), tsCoach = new TimeSpan(), tsEbtr = new TimeSpan();
            DateTime dtLstPf = DateTime.MinValue,
                     dtLstCit = DateTime.MinValue,
                     dtLstCoach = DateTime.MinValue,
                     dtLstEbtr = DateTime.MinValue;
            var memberAccumulating = TeamMemberData.UserTypes.Undefined;

            foreach (var stateTransition in theHistory)
            {
                var memberOfTransitionFrom = StoryStageLogic.WhoseTurn(stateTransition.FromState);
                var memberOfTransitionTo = StoryStageLogic.WhoseTurn(stateTransition.ToState);

                if (!GetMemberAccumulating(memberOfTransitionTo, memberOfTransitionFrom, ref memberAccumulating))
                    continue;

                bool bDoItAgain;

                do
                {
                    bDoItAgain = false;
                    switch (memberAccumulating)
                    {
                        case TeamMemberData.UserTypes.ProjectFacilitator:
                            tsPf = UpdateTimeSpan(memberAccumulating, stateTransition,
                                                  memberOfTransitionTo, tsPf, ref dtLstPf);
                            break;
                        case TeamMemberData.UserTypes.EnglishBackTranslator:
                            tsEbtr = UpdateTimeSpan(memberAccumulating, stateTransition,
                                                    memberOfTransitionTo, tsEbtr, ref dtLstEbtr);
                            break;
                        case TeamMemberData.UserTypes.ConsultantInTraining:
                            tsCit = UpdateTimeSpan(memberAccumulating, stateTransition,
                                                   memberOfTransitionTo, tsCit, ref dtLstCit);
                            break;
                        case TeamMemberData.UserTypes.Coach:
                            tsCoach = UpdateTimeSpan(memberAccumulating, stateTransition,
                                                     memberOfTransitionTo, tsCoach, ref dtLstCoach);
                            break;
                        default:
                            break;
                    }

                    if (memberAccumulating != memberOfTransitionTo)
                    {
                        bDoItAgain = true;
                        memberAccumulating = memberOfTransitionTo;
                    }

                } while (bDoItAgain);
            }


            var yValues = new List<double> { tsPf.TotalDays, tsCit.TotalDays };
            var xNames = new List<string>
                             {
                                 TeamMemberData.CstrProjectFacilitatorDisplay,
                                 TeamMemberData.CstrIndependentConsultantDisplay
                             };

            if (tsEbtr.TotalDays > 0)
            {
                yValues.Add(tsEbtr.TotalDays);
                xNames.Add(TeamMemberData.CstrEnglishBackTranslatorDisplay);
            }

            if (tsCoach.TotalDays > 0)
            {
                yValues.Add(tsCoach.TotalDays);
                xNames.Add(TeamMemberData.CstrCoachDisplay);
            }

            chartTurnTiming.Series[0].AxisLabel = Localizer.Str("Days in turn");
            chartTurnTiming.Series[0].Points.DataBindXY(xNames, yValues);
        }

        private static TimeSpan UpdateTimeSpan(TeamMemberData.UserTypes userType,
                                               StoryStateTransition stateTransition,
                                               TeamMemberData.UserTypes memberOfTransitionTo,
                                               TimeSpan timeSpan,
                                               ref DateTime dtTransitionStart)
        {
            if (dtTransitionStart == DateTime.MinValue)
                dtTransitionStart = stateTransition.TransitionDateTime;

            else if (memberOfTransitionTo != userType)
            {
                timeSpan += stateTransition.TransitionDateTime - dtTransitionStart;
                dtTransitionStart = DateTime.MinValue;  // reset for next time
            }

            return timeSpan;
        }

        private static bool GetMemberAccumulating(TeamMemberData.UserTypes memberOfTransitionTo, TeamMemberData.UserTypes memberOfTransitionFrom,
                                                  ref TeamMemberData.UserTypes memberAccumulating)
        {
            if (memberAccumulating == TeamMemberData.UserTypes.Undefined)
            {
                if (memberOfTransitionFrom == TeamMemberData.UserTypes.Undefined)
                {
                    if (memberOfTransitionTo == TeamMemberData.UserTypes.Undefined)
                        return false;

                    memberAccumulating = memberOfTransitionTo;
                }
                else
                    memberAccumulating = memberOfTransitionFrom;
            }
            return true;
        }
    }
}
