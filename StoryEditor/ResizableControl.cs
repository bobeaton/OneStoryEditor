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
        internal StoryStageLogic StageLogic = null;

        protected const string CstrSuffixTextBox = "TextBox";
        protected const string CstrSuffixLabel = "Label";

        protected delegate void ReheightAllControlsDelegate();

        protected ResizableControl()
        {
        }

        public ResizableControl(StoryStageLogic storyStageLogic)
        {
            StageLogic = storyStageLogic;
            InitializeComponent();
        }

        /// <summary>
        /// Begins the UpdateView process by suspending the layout and fixing this control's width
        /// Subclasses will call this at the start of their implementation of UpdateView
        /// </summary>
        /// <param name="nWidth">This parameter is used to fix the width of the control (the parent knows what the width is)</param>
        public void UpdateHeight(int nWidth)
        {
            if (Width != nWidth)
            {
                tableLayoutPanel.SuspendLayout();
                SuspendLayout();

                Width = nWidth;

                // first resume and perform the layout. This causes all, for example, Dock=Fill TextBox's 
                // to have their width's set
                tableLayoutPanel.ResumeLayout(false);
                tableLayoutPanel.PerformLayout();
                ResumeLayout(false);
                PerformLayout();
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
            tableLayoutPanel.InsertColumn(nLayoutColumnIndex, new ColumnStyle(SizeType.Percent, 100));
        }

        protected virtual void RemoveRow(int nLayoutRowIndex)
        {
            tableLayoutPanel.RemoveRow(nLayoutRowIndex);
        }

        protected virtual void RemoveColumn(int nLayoutColumnIndex)
        {
            tableLayoutPanel.RemoveColumn(nLayoutColumnIndex);
        }

        public void textBox_TextChanged(object sender, EventArgs e)
        {
            CtrlTextBox tb = (CtrlTextBox)sender;
            AdjustTextBoxHeight(tb);

            // the 'Tag' of each text box contains a delegate to set the data item it is associated with
            System.Diagnostics.Debug.Assert((tb.Tag != null) && (tb.Tag is StringTransfer.SetValueDelegate));
            StringTransfer.SetValueDelegate SetAssocDataValue = (StringTransfer.SetValueDelegate)tb.Tag;
            SetAssocDataValue(tb.Text);
        }

        protected void AdjustTextBoxHeight(CtrlTextBox tb)
        {
            if (ResizeTextBoxToFitText(tb))
                AdjustHeightWithSuspendLayout(null);
        }

        protected virtual void ReheightAllControls()
        {
            //  Resize all of the Text boxes to fit and tell all sub-controls of this same type to resize 
            //  themselves also.
            foreach (Control aCtrl in tableLayoutPanel.Controls)
            {
                Type type = aCtrl.GetType();
                if (type == typeof(CtrlTextBox))
                    ResizeTextBoxToFitText((CtrlTextBox)aCtrl);
                else if ((aCtrl is ResizableControl) || (aCtrl is VerseControl))
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
            SuspendLayout();
            tableLayoutPanel.SuspendLayout();

            bool bBeingCalledFromUpdateHeight = (myDelegate != null);
            if (bBeingCalledFromUpdateHeight)
                myDelegate();

            AdjustHeight();

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
            Height = nTableLayoutPanel;
        }

        protected static bool ResizeTextBoxToFitText(CtrlTextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(tb.Width, 1000));
            bool bHeightChanged = (sz.Height != tb.Size.Height);
            if (bHeightChanged)
                tb.Height = sz.Height;
            return bHeightChanged;
        }

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            if (this is VerseControl)
                theSE = (this as VerseControl).TheSE;
            else
                theSE = (StoryEditor)FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        "Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com");

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

                if (!theSE.theCurrentStory.ProjStage.IsEditAllowed(theSE.LoggedOnMember))
                    throw theSE.theCurrentStory.ProjStage.WrongMemberTypeEx;
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message));
                return false;
            }

            return true;
        }

        public void VisiblizeLast()
        {
            if (tableLayoutPanel.Controls.Count > 0)
                ScrollControlIntoView(tableLayoutPanel.Controls[Controls.Count - 1]);
        }
    }

    public class VerseControl : ResizableControl
    {
        protected const string CstrVerseName = "ln: ";
        internal int VerseNumber = -1;
        internal StoryEditor TheSE = null;

        private VerseControl()
        {
        }

        public VerseControl(StoryStageLogic storyStageLogic, int nVerseNumber, StoryEditor theSE)
            : base(storyStageLogic)
        {
            VerseNumber = nVerseNumber;
            TheSE = theSE;
        }
    }
}
