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
    public partial class ChooseWhichTestToDisplayForm : Form
    {
        private readonly StoryProjectData _theStoryProjectData;

        private readonly TestRows _rows = new TestRows();
        private List<CheckBox> _checkBoxes = new List<CheckBox>();

        private ChooseWhichTestToDisplayForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testInfo">e.g. theCurrentStory.CraftingInfo.TestersToCommentsRetellings</param>
        /// <param name="strLabel">e.g. Localizer.Str("Retelling Test &")</param>
        public ChooseWhichTestToDisplayForm(StoryProjectData theStoryProjectData, TestInfo testInfo, string strLabel)
        {
            _theStoryProjectData = theStoryProjectData;

            InitializeComponent();
            Localizer.Ctrl(this);

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            if (testInfo.Count == 0)
            {
                Text = String.Format(Localizer.Str("Oops, this story has no {0}s"), strLabel);
            }
            else
            {
                InitializeTestRows(testInfo, _rows, strLabel);

                Text = String.Format(Localizer.Str("Choose which tests to view for '{0}'"), strLabel);
            }

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        private void InitializeTestRows(TestInfo testInfo, TestRows rows, string strLabel)
        {
            for (var i = 0; i < testInfo.Count; i++)
            {
                var j = i + 1;
                var row = new ControlRow(String.Format("{0} &{1}", strLabel, j));
                var tester = testInfo[i];
                var checkBox = InitTesterRow(row, tester);
                if ((testInfo.ListOfMemberIdsToDisplay != null) && (testInfo.ListOfMemberIdsToDisplay.Count > 0) &&
                    testInfo.ListOfMemberIdsToDisplay.Contains(tester.MemberId))
                {
                    checkBox.Checked = true;
                }

                rows.Add(row);
            }
        }

        private CheckBox InitTesterRow(ControlRow row, MemberIdInfo tester)
        {
            // add the row of controls to the table layout panel
            int nRow = tableLayoutPanel.RowCount;
            tableLayoutPanel.Controls.Add(row.Label, 0, nRow);

            var checkBox = new CheckBox { Tag = tester.MemberId };  // keep track of the member ID, so we can recover that later
            _checkBoxes.Add(checkBox);
            tableLayoutPanel.Controls.Add(checkBox, 1, nRow);

            tableLayoutPanel.Controls.Add(row.TbxName, 2, nRow);
            tableLayoutPanel.Controls.Add(row.TbxComment, 3, nRow);
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowCount++;

            // now initialize it with data
            var member = _theStoryProjectData.GetMemberFromId(tester.MemberId);
            if (member != null)
                row.TbxName.Text = member.Name;
            row.TbxComment.Text = tester.MemberComment;

            return checkBox;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _listOfTesteeIdsToShow.Clear();
            foreach (var checkBox in _checkBoxes.Where(checkBox => checkBox.Checked))
            {
                _listOfTesteeIdsToShow.Add((string)checkBox.Tag);   //  _checkBoxes.IndexOf(checkBox));
            }

            DialogResult = DialogResult.OK;
        }

        private List<string> _listOfTesteeIdsToShow = new List<string>();
        public List<string> ListOfTesteeIdsToShow
        {
            get { return _listOfTesteeIdsToShow; }
        }
    }
}
