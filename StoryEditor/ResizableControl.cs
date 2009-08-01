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
        protected internal ResizableControl ParentControl = null;

        protected const string cstrSuffixTextBox = "TextBox";
        protected const string cstrSuffixLabel = "Label";

        protected delegate void ReheightAllControlsDelegate();

        public ResizableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// UpdateView is implemented by subclasses to turn on and off controls in the TableLayoutPanel
        /// based on view menu settings in the parent frame (aSE)
        /// </summary>
        /// <param name="aSE">This parameter is used to give access to the main window (for things like, what view items are enabled, etc)</param>
        /// <param name="nWidth">This parameter is used to fix the width of the control (the parent knows what the width is)</param>
        public virtual void UpdateView(StoryEditor aSE)
        {
            // only subclasses know what to do here...
        }

        /// <summary>
        /// Begins the UpdateView process by suspending the layout and fixing this control's width
        /// Subclasses will call this at the start of their implementation of UpdateView
        /// </summary>
        /// <param name="nWidth">This parameter is used to fix the width of the control (the parent knows what the width is)</param>
        public void UpdateHeight(int nWidth)
        {
            if (this.Width != nWidth)
            {
                this.tableLayoutPanel.SuspendLayout();
                this.SuspendLayout();

                this.Width = nWidth;

                // first resume and perform the layout. This causes all, for example, Dock=Fill TextBox's 
                // to have their width's set
                this.tableLayoutPanel.ResumeLayout(false);
                this.tableLayoutPanel.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }

            // now reheight all the controls
            ReheightAllControlsDelegate myDelegate = new ReheightAllControlsDelegate(ReheightAllControls);
            AdjustHeightWithSuspendLayout(myDelegate);
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected virtual void InsertRow(int nLayoutRowIndex)
        {
            tableLayoutPanel.InsertRow(nLayoutRowIndex);
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected virtual void InsertColumn(int nLayoutColumnIndex)
        {
            tableLayoutPanel.InsertColumn(nLayoutColumnIndex);
        }

        protected virtual void RemoveRow(int nLayoutRowIndex)
        {
            tableLayoutPanel.RemoveRow(nLayoutRowIndex);
        }

        protected virtual void RemoveColumn(int nLayoutColumnIndex)
        {
            tableLayoutPanel.RemoveColumn(nLayoutColumnIndex);
        }

        protected void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (ResizeTextBoxToFitText(tb))
                AdjustHeightWithSuspendLayout(null);
        }

        protected void ReheightAllControls()
        {
            //  Resize all of the Text boxes to fit and tell all sub-controls of this same type to resize 
            //  themselves also.
            foreach (Control aCtrl in tableLayoutPanel.Controls)
            {
                if (aCtrl.GetType() == typeof(TextBox))
                    ResizeTextBoxToFitText((TextBox)aCtrl);
                else if (aCtrl is ResizableControl)
                    ((ResizableControl)aCtrl).UpdateHeight(tableLayoutPanel.Width - tableLayoutPanel.Margin.Horizontal);
            }
        }

        /// <summary>
        /// This method is called from two directions: it's called (via UpdateView) when the user has 
        /// changed the width of the panel that this control is in (after UpdateView re-width's 'this'),
        /// which means that all the controls embedded in this control must be re-height'd as well.
        /// In this case, it is appropriate to pass the delegate for the "ReheightAllControls" so that
        /// that method can be called just prior to calling Adjust Height.
        /// The other direction this is called is when the user has forced an increase in a Text box
        /// that is necessatiating the parent control(s) (possibly all the way up the chain) to reheight
        /// themselves as well. In this case, we don't need to resize all the controls in any given
        /// form, but again, all the way up the chain.
        /// </summary>
        /// <param name="myDelegate"></param>
        protected void AdjustHeightWithSuspendLayout(ReheightAllControlsDelegate myDelegate)
        {
            this.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();

            bool bBeingCalledFromUpdateHeight = (myDelegate != null);
            if (bBeingCalledFromUpdateHeight)
                myDelegate();

            AdjustHeight();

            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            // if we have a parent (e.g. the sub-controls like AnchorControl do) AND
            //  we aren't being called from UpdateView above (where the Parent will 
            //  be done in it's own course), then call the parent to update the height
            //  (this is basically so that if we resize the Anchor because the user
            //  entered an exegetical comment, then the parent Verse control needs
            //  to resize also).
            if ((ParentControl != null) && (!bBeingCalledFromUpdateHeight))
                ParentControl.AdjustHeightWithSuspendLayout(null);
        }

        protected void AdjustHeight()
        {
            // do a similar thing with the layout panel (i.e. give it the same width and infinite height.
            // for some reason GetPreferredSize doesn't give the actual right size... so I'll write my own
            int nTableLayoutPanel = tableLayoutPanel.GetPreferredHeight();
            this.Height = nTableLayoutPanel;
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
