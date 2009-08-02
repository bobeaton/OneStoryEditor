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
    public partial class AnchorControl : ResizableControl
    {
        protected const string cstrFieldNameAnchor = "AnchorButton";
        protected const string cstrFieldNameExegeticalHelp = "ExegeticalHelp";

        public AnchorControl(StoryProject.anchorsRow anAnchorsRow)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.tableLayoutPanel.Controls.Add(this.labelAnchor, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.toolStripAnchors, 1, 0);

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            int nNumRows = 1;
            foreach (StoryProject.anchorRow anAnchorRow in anAnchorsRow.GetanchorRows())
            {
                InitAnchorButton(toolStripAnchors, anAnchorRow.jumpTarget, anAnchorRow.text);
                InitExegeticalHelpsRow(anAnchorRow, ref nNumRows);
            }

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
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

                // add the label and tool strip as a new row to the table layout panel
                int nLayoutRow = nNumRows++;
                InsertRow(nLayoutRow);
                tableLayoutPanel.Controls.Add(labelExegeticalHelp, 0, nLayoutRow);
                tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
            }
        }

        protected void InitExegeticalHelpsRow(StoryProject.anchorRow anAnchorRow, ref int nNumRows)
        {
            StoryProject.exegeticalHelpsRow[] anEHsRow = anAnchorRow.GetexegeticalHelpsRows();
            if ((anEHsRow != null) && (anEHsRow.Length > 0))
                InitExegeticalHelpRows(anEHsRow[0], ref nNumRows);
        }

        private void toolStripAnchors_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                NetBibleViewer theNetBibleViewer = (NetBibleViewer)e.Data.GetData(typeof(NetBibleViewer));
                foreach (ToolStripButton btn in toolStripAnchors.Items)
                    if (btn.Text == theNetBibleViewer.ScriptureReference)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                e.Effect = DragDropEffects.Link;
            }
        }

        private void toolStripAnchors_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                NetBibleViewer theNetBibleViewer = (NetBibleViewer)e.Data.GetData(typeof(NetBibleViewer));
                this.InitAnchorButton(toolStripAnchors, theNetBibleViewer.ScriptureReference, theNetBibleViewer.ScriptureReference);
            }
        }
    }
}
