using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ResizableControl : UserControl
    {
        public ResizableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// UpdateView is implemented by subclasses to add controls to the TableLayoutPanel
        /// </summary>
        /// <param name="aSE">This parameter is used to give access to the main window (for things like, what view items are enabled, etc)</param>
        /// <param name="nWidth">This parameter is used to fix the width of the control (the parent knows what the width is)</param>
        public virtual void UpdateView(StoryEditor aSE, int nWidth)
        {
            UpdateViewInit(nWidth);
            UpdateViewFini(aSE, nWidth);
        }

        /// <summary>
        /// Begins the UpdateView process by suspending the layout and fixing this control's width
        /// Subclasses will call this at the start of their implementation of UpdateView
        /// </summary>
        /// <param name="nWidth">This parameter is used to fix the width of the control (the parent knows what the width is)</param>
        protected void UpdateViewInit(int nWidth)
        {
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.Width = nWidth;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateViewFini(StoryEditor aSE, int nWidth)
        {
            // first resume and perform the layout. This causes all, for example, Dock=Fill TextBox's to have their width's set
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            // now suspend the layout again because we're going to get their preferred height (based on 
            //  fully displaying the text in them and adjust them accordingly).
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            //  Resize all of the Text boxes to fit and tell all sub-controls of this same type to resize themselves also.
            foreach (Control aCtrl in tableLayoutPanel.Controls)
            {
                if (aCtrl.GetType() == typeof(TextBox))
                    ResizeTextBoxToFitText((TextBox)aCtrl);
                else if (aCtrl.GetType() == typeof(ResizableControl))
                    ((ResizableControl)aCtrl).UpdateView(aSE, tableLayoutPanel.Width - tableLayoutPanel.Margin.Horizontal);
            }

            AdjustHeight();

            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected virtual void InsertRow(int nLayoutRowIndex)
        {
            tableLayoutPanel.InsertRow(nLayoutRowIndex);
        }

        protected virtual void RemoveRow(int nLayoutRowIndex)
        {
            tableLayoutPanel.RemoveRow(nLayoutRowIndex);
        }

        protected void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (ResizeTextBoxToFitText(tb))
            {
                this.SuspendLayout();
                this.tableLayoutPanel.SuspendLayout();

                AdjustHeight();

                if ((Parent != null) && (Parent.GetType() == typeof(ResizableControl)))
                    ((ResizableControl)Parent).AdjustHeight();

                this.tableLayoutPanel.ResumeLayout(false);
                this.tableLayoutPanel.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        protected void AdjustHeight()
        {
            // do a similar thing with the layout panel (i.e. give it the same width and infinite height.
            // for some reason GetPreferredSize doesn't give the actual right size... so I'll write my own
            // Size sz = this.tableLayoutPanelVerse.GetPreferredSize(new Size(tableLayoutPanelVerse.Width, 1000));
            this.tableLayoutPanel.Height = tableLayoutPanel.GetPreferredHeight();
            this.Height = tableLayoutPanel.Height + tableLayoutPanel.Margin.Vertical;
        }

        protected static bool ResizeTextBoxToFitText(TextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(tb.Width, 1000));
            bool bHeightChanged = (sz.Height != tb.Size.Height);
            if (bHeightChanged)
                tb.Height = sz.Height;
            return bHeightChanged;
        }
    }
}
