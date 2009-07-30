using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace StoryEditor
{
    /// <summary>
    /// Add the ability to remove a row of the table layout panel (which turns out to be 
    /// freekisly hard to do with the native control
    /// </summary>
    class DynamicTableLayoutPanel : System.Windows.Forms.TableLayoutPanel
    {
        public void InsertRow(int nLayoutRowIndex)
        {
            int nRowIndex = this.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.AutoSize));
            this.RowCount++;

            // if we're inserting in the middle, then we have to move the following controls down
            if (nRowIndex != nLayoutRowIndex)
            {
                for (int i = this.RowCount - 1; i > nLayoutRowIndex; i--)
                    for (int j = 0; j < this.ColumnCount; j++)
                    {
                        Control ctrl = this.GetControlFromPosition(j, i - 1);
                        if (ctrl != null)
                        {
                            TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(j, i);
                            this.SetCellPosition(ctrl, cp);
                        }
                    }
            }
        }

        public void RemoveRow(int nLayoutRowIndex)
        {
            for (int i = 0; i < this.ColumnCount; i++)
            {
                Control ctrl = this.GetControlFromPosition(i, nLayoutRowIndex);
                if (ctrl != null)
                    this.Controls.Remove(ctrl);
            }

            for (int i = nLayoutRowIndex; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Control ctrl = this.GetControlFromPosition(j, i + 1);
                    if (ctrl != null)
                    {
                        TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(j, i);
                        this.SetCellPosition(ctrl, cp);
                    }
                }

            this.RowCount--;
            this.RowStyles.RemoveAt(RowCount);
        }

        protected void DumpTable()
        {
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Control ctrl = this.GetControlFromPosition(j, i);
                    if (ctrl != null)
                        System.Diagnostics.Debug.WriteLine(String.Format("ctrl(row:{0},column:{1}).Name: {2}", i, j, ctrl.ToString()));
                    else
                        System.Diagnostics.Debug.WriteLine(String.Format("NO CTRL at: row:{0}, column:{1}", i, j));
                }
        }
    }
}
