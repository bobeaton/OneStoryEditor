using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class AnchorControl : UserControl
    {
        protected const string cstrFieldNameAnchor = "AnchorButton";
        protected const string cstrFieldNameExegeticalHelp = "ExegeticalHelp";

#if DEBUG
        protected List<TextBox> m_lstTb = new List<TextBox>();
#endif

        public AnchorControl(StoryProject.anchorsRow anAnchorsRow)
        {
            InitializeComponent();

            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            int nNumRows = 1;
            foreach (StoryProject.anchorRow anAnchorRow in anAnchorsRow.GetanchorRows())
            {
                InitAnchorButton(toolStripAnchors, anAnchorRow.jumpTarget, anAnchorRow.text);
                InitExegeticalHelpsRow(anAnchorRow, ref nNumRows);
            }

            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void UpdateView(int nWidth)
        {
            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();

            foreach (Control aCtrl in tableLayoutPanelAnchor.Controls)
            {
                try
                {
                    // for all the text boxes, set them to the fixed width, but high as possible (so it will limit
                    //  at whatever is neede)
                    TextBox tb = (TextBox)aCtrl;
                    VerseBtControl.ResizeTextBoxToFitText(tb);
                }
                catch { } // skip any non-text boxes
            }

            AdjustHeight();

            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected void AdjustHeight()
        {
            // do a similar thing with the layout panel (i.e. give it the same width and infinite height.
            // for some reason GetPreferredSize doesn't give the actual right size... so I'll write my own
            // Size sz = this.tableLayoutPanelVerse.GetPreferredSize(new Size(tableLayoutPanelVerse.Width, 1000));
            this.tableLayoutPanelAnchor.Height = tableLayoutPanelAnchor.GetPreferredHeight();
            this.Height = tableLayoutPanelAnchor.Height + tableLayoutPanelAnchor.Margin.Top + tableLayoutPanelAnchor.Margin.Bottom;
        }

        protected void InitAnchorButton(ToolStrip ts, string strJumpTarget, string strComment)
        {
            ToolStripButton aButton = new ToolStripButton();
            aButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            aButton.Name = cstrFieldNameAnchor + strJumpTarget;
            aButton.AutoSize = true;
            aButton.Text = strJumpTarget;
            aButton.ToolTipText = strComment;
            ts.Items.Add(aButton);
        }

        protected void InitExegeticalHelpRows(StoryProject.exegeticalHelpsRow anEHsRow, ref int nNumRows)
        {
            StoryProject.exegeticalHelpRow[] aEHRows = anEHsRow.GetexegeticalHelpRows();
            System.Diagnostics.Debug.Assert(aEHRows != null);
            for (int i = 0; i < aEHRows.Length; i++)
            {
                StoryProject.exegeticalHelpRow aEHRow = aEHRows[i];
                TextBox tb = new TextBox();
                tb.Name = cstrFieldNameExegeticalHelp + i.ToString();
                tb.Multiline = true;
                tb.Dock = DockStyle.Fill;
                tb.Text = aEHRow.quote;
                tb.TextChanged += new EventHandler(textBox_TextChanged);
                m_lstTb.Add(tb);

                // add the label and tool strip as a new row to the table layout panel
                int nLayoutRow = nNumRows++;
                tableLayoutPanelAnchor.InsertRow(nLayoutRow);
                tableLayoutPanelAnchor.Controls.Add(labelExegeticalHelp, 0, nLayoutRow);
                tableLayoutPanelAnchor.Controls.Add(tb, 1, nLayoutRow);
            }
        }

        protected void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();

            VerseBtControl myParent = (VerseBtControl)Parent;
            bool b = VerseBtControl.ResizeTextBoxToFitText(tb);

            if (b)
            {
                this.tableLayoutPanelAnchor.SuspendLayout();
                this.SuspendLayout();

                myParent.AdjustHeight();

                this.tableLayoutPanelAnchor.ResumeLayout(false);
                this.tableLayoutPanelAnchor.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        protected void InitExegeticalHelpsRow(StoryProject.anchorRow anAnchorRow, ref int nNumRows)
        {
            StoryProject.exegeticalHelpsRow[] anEHsRow = anAnchorRow.GetexegeticalHelpsRows();
            if ((anEHsRow != null) && (anEHsRow.Length > 0))
                InitExegeticalHelpRows(anEHsRow[0], ref nNumRows);
        }
    }
}
