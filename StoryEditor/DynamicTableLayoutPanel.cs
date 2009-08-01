using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Add the ability to remove a row of the table layout panel (which turns out to be 
    /// freekisly hard to do with the native control
    /// </summary>
    public class DynamicTableLayoutPanel : System.Windows.Forms.TableLayoutPanel
    {
        public void InsertRow(int nLayoutRowIndex)
        {
            int nRowIndex = this.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.AutoSize));
            this.RowCount++;

            // if we're inserting in the middle, then we have to move the following controls down
            if (nRowIndex != nLayoutRowIndex)
            {
                for (int row = this.RowCount - 1; row > nLayoutRowIndex; row--)
                    for (int col = 0; col < this.ColumnCount; col++)
                    {
                        Control ctrl = this.GetControlFromPosition(col, row - 1);
                        if (ctrl != null)
                        {
                            TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(col, row);
                            this.SetCellPosition(ctrl, cp);
                        }
                    }
            }
        }

        public void RemoveRow(int nLayoutRowIndex)
        {
            for (int col = 0; col < this.ColumnCount; col++)
            {
                Control ctrl = this.GetControlFromPosition(col, nLayoutRowIndex);
                if (ctrl != null)
                    this.Controls.Remove(ctrl);
            }

            for (int row = nLayoutRowIndex; row < this.RowCount; row++)
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    Control ctrl = this.GetControlFromPosition(col, row + 1);
                    if (ctrl != null)
                    {
                        TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(col, row);
                        this.SetCellPosition(ctrl, cp);
                    }
                }

            this.RowCount--;
            this.RowStyles.RemoveAt(RowCount);
        }

        public void InsertColumn(int nLayoutColumnIndex)
        {
            int nColumnIndex = this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100));
            ColumnCount++;

            // if we're inserting in the middle, then we have to move the following controls down
            if (nColumnIndex != nLayoutColumnIndex)
            {
                for (int col = this.ColumnCount - 1; col > nLayoutColumnIndex; col--)
                    for (int row = 0; row < this.RowCount; row++)
                    {
                        Control ctrl = this.GetControlFromPosition(col - 1, row);
                        if (ctrl != null)
                        {
                            TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(col, row);
                            this.SetCellPosition(ctrl, cp);
                        }
                    }
            }
        }

        public void RemoveColumn(int nLayoutColumnIndex)
        {
            for (int row = 0; row < this.RowCount; row++)
            {
                Control ctrl = this.GetControlFromPosition(nLayoutColumnIndex, row);
                if (ctrl != null)
                    this.Controls.Remove(ctrl);
            }

            for (int col = nLayoutColumnIndex; col < this.ColumnCount; col++)
                for (int row = 0; row < this.RowCount; row++)
                {
                    Control ctrl = this.GetControlFromPosition(col + 1, row);
                    if (ctrl != null)
                    {
                        TableLayoutPanelCellPosition cp = new TableLayoutPanelCellPosition(col, row);
                        this.SetCellPosition(ctrl, cp);
                    }
                }

            this.ColumnCount--;
            this.ColumnStyles.RemoveAt(ColumnCount);
        }

        protected float GetColumnWidthPercentage(int nNewColumnWidth)
        {
            return 100 / nNewColumnWidth;
        }

        public void DumpTable()
        {
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Control ctrl = this.GetControlFromPosition(j, i);
                    if (ctrl != null)
                        System.Diagnostics.Debug.WriteLine(String.Format("ctrl(row:{0},column:{1}).Name: {2}; Width: {3}; Height: {4}", i, j, ctrl.ToString(), ctrl.Width.ToString(), ctrl.Height.ToString()));
                    else
                        System.Diagnostics.Debug.WriteLine(String.Format("NO CTRL at: row:{0}, column:{1}", i, j));
                }
        }

        public int GetPreferredHeight()
        {
            int nHeight = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                int nRowHeight = 0;
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Control ctrl = this.GetControlFromPosition(j, i);
                    if (ctrl != null)
                        nRowHeight = Math.Max(nRowHeight, ctrl.Height);
                }
                nHeight += nRowHeight;
            }
            return nHeight;
        }
    }
}
