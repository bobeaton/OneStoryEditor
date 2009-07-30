using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StoryEditor
{
    public partial class AnchorControl : UserControl
    {
        protected const string cstrFieldNameAnchor = "AnchorButton";
        protected const string cstrFieldNameExegeticalHelp = "ExegeticalHelp";

        protected int[] m_anRowIndexExegeticalHelps = null;

        public AnchorControl(VerseBtControl theParent)
        {
            InitializeComponent();
            Parent = theParent;
        }

        public void UpdateView(StoryEditor aSE, StoryProject.anchorsRow anAnchorsRow, int nWidth)
        {
            this.Width = this.tableLayoutPanelAnchor.Width = nWidth;

            // add the label and tool strip as a new row to the table layout panel
            // finally populate the buttons on that tool strip
            int nNumRows = 1;
            foreach (StoryProject.anchorRow anAnchorRow in anAnchorsRow.GetanchorRows())
            {
                InitAnchorButton(toolStripAnchors, anAnchorRow.jumpTarget, anAnchorRow.text);
                InitExegeticalHelpsRow(aSE, anAnchorRow, ref nNumRows);
            }
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
            if (m_anRowIndexExegeticalHelps == null)
            {
                StoryProject.exegeticalHelpRow[] aEHRows = anEHsRow.GetexegeticalHelpRows();
                System.Diagnostics.Debug.Assert(aEHRows != null);
                m_anRowIndexExegeticalHelps = new int[aEHRows.Length];
                for (int i = 0; i < aEHRows.Length; i++)
                {
                    StoryProject.exegeticalHelpRow aEHRow = aEHRows[i];
                    TextBox tb = new TextBox();
                    tb.Name = cstrFieldNameExegeticalHelp + i.ToString();
                    tb.Multiline = true;
                    tb.Dock = DockStyle.Fill;
                    tb.Text = aEHRow.quote;
                    tb.TextChanged += new EventHandler(textBox_TextChanged);

                    Size sz = tb.GetPreferredSize(new Size(tableLayoutPanelAnchor.Width, 1000));
                    if (sz.Height != tb.Size.Height)
                        tb.Size = sz;

                    // add the label and tool strip as a new row to the table layout panel
                    m_anRowIndexExegeticalHelps[i] = nNumRows++;
                    tableLayoutPanelAnchor.InsertRow(m_anRowIndexExegeticalHelps[i]);
                    tableLayoutPanelAnchor.Controls.Add(tb, 0, m_anRowIndexExegeticalHelps[i]);
                }
            }
        }

        protected void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();

            VerseBtControl myParent = (VerseBtControl)Parent;
            bool b = myParent.ResizeTextBoxToFitText(tb);

            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);
            
            if (b)
                myParent.AdjustHeight();
        }

        protected void InitExegeticalHelpsRow(StoryEditor aSE, StoryProject.anchorRow anAnchorRow, ref int nNumRows)
        {
            StoryProject.exegeticalHelpsRow[] anEHsRow = anAnchorRow.GetexegeticalHelpsRows();
            if ((anEHsRow != null) && (anEHsRow.Length > 0))
                InitExegeticalHelpRows(anEHsRow[0], ref nNumRows);
        }
    }
}
