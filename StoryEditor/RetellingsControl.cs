using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class RetellingsControl : OneStoryProjectEditor.ResizableControl
    {
        protected const string cstrFieldNameRetellingLabel = "RetellingLabel";
        protected const string cstrFieldNameRetellingTextBox = "RetellingTextBox";

        public RetellingsControl(StoryProject.RetellingsRow aRetellingsRow)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, fix this assumption: RetellingsControl.cs.20");
            tableLayoutPanel.RemoveRow(1);  // remove the one default one we start with

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            StoryProject.RetellingRow[] aRetellingRows = aRetellingsRow.GetRetellingRows();
            System.Diagnostics.Debug.Assert(aRetellingRows.Length > 0);
            int nNumRows = 0;
            foreach (StoryProject.RetellingRow aRetellingRow in aRetellingRows)
            {
                InitRetellingRow(aRetellingRow, ref nNumRows);
            }

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        protected void InitRetellingRow(StoryProject.RetellingRow aRetellingRow, ref int nNumRows)
        {
            int nLayoutRow = nNumRows++;

            Label labelRetelling = new Label();
            labelRetelling.Anchor = System.Windows.Forms.AnchorStyles.Left;
            labelRetelling.AutoSize = true;
            labelRetelling.Name = cstrFieldNameRetellingLabel + nNumRows.ToString();
            labelRetelling.Text = String.Format("ret({0}):", nNumRows);

            TextBox tb = new TextBox();
            tb.Name = cstrFieldNameRetellingTextBox + nNumRows.ToString();
            tb.Multiline = true;
            tb.Dock = DockStyle.Fill;
            tb.Text = aRetellingRow.Retelling_text;
            tb.TextChanged += new EventHandler(textBox_TextChanged);

            // add the label and tool strip as a new row to the table layout panel
            InsertRow(nLayoutRow);
            tableLayoutPanel.Controls.Add(labelRetelling, 0, nLayoutRow);
            tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
        }
    }
}
