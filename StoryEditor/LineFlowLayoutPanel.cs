using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class LineFlowLayoutPanel : FlowLayoutPanel
    {
        public LinkLabel LineNumberLink { get; set; }

        public LineFlowLayoutPanel()
        {
            InitializeComponent();
        }

        public virtual VerseControl GetControlAtVerseIndex(int nVerseIndex)
        {
            // this used to just be 
            // return Controls[nVerseIndex] 
            //  (or Controls[((nVerseIndex - 1)*2) + 1] for VerseBT controls)
            // but some verses may be hidden and so it's not so straightforward
            // start at that index (going backwards) and make sure the verse index matches
            for (int i = Math.Min(nVerseIndex, Controls.Count - 1); i >= 0; i--)
            {
                Control ctrl = Controls[i];
                System.Diagnostics.Debug.Assert(ctrl is VerseControl);
                VerseControl ctrlVerse = ctrl as VerseControl;
                if (ctrlVerse.VerseNumber == nVerseIndex)
                    return ctrlVerse;
            }

            return null;
        }

        public virtual void Clear()
        {
            SuspendLayout();

            while (Controls.Count > 0)
            {
                Control ctrl = Controls[0];
                ctrl.Dispose();
            }

            Controls.Clear();

            ResumeLayout(false);
        }

        public void DoMouseWheel(MouseEventArgs e)
        {
            // for some reason, the scroll wheel doesn't result in the _Scroll
            //  event being called...
            VerseControl ctrlLastNegative = VerseControlJustAboveDisplayRectange;
            SetLineNumberLink(ctrlLastNegative);

            base.OnMouseWheel(e);
        }

        private void LineFlowLayoutPanel_Scroll(object sender, ScrollEventArgs e)
        {
            // if we get a vertical scroll event, and the values are different (i.e. 
            //  it's not at the first/last position) and if it's a large change (i.e.
            //  click in the middle of the scroll bar), then...
            if ((e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                && (e.NewValue != e.OldValue)
                && ((e.Type == ScrollEventType.LargeDecrement)
                    || (e.Type == ScrollEventType.LargeIncrement)))
            {
                bool bScrollDown = (e.NewValue > e.OldValue);

                Control ctrlNext;
                if (bScrollDown)
                {
                    ctrlNext = NextControlDown;
                }
                else
                {
                    ctrlNext = NextControlUp;
                }

                if (ctrlNext != null)
                {
                    VerseBtControl ctrlLastNegative = ctrlNext as VerseBtControl;
                    ScrollIntoView(ctrlLastNegative, true);
                    // SetLineNumberLink(ctrlLastNegative);
                }
            }
            
            // small or otherwise scrolling...
            else 
            {
                VerseBtControl ctrlLastNegative = VerseControlJustAboveDisplayRectange;

                // only do this next part if we're *not* thumb tracking (it's too annoying)
                if ((e.Type == ScrollEventType.SmallIncrement)
                    || (e.Type == ScrollEventType.SmallDecrement))
                {
                    // if the number is going to be changed, then let's *not* remember the last
                    //  control box that was clicked in (if the autosave timer expires and we've
                    //  been scrolling down the page, then when the save is done, it jumps back
                    //  up to the last one clicked in)
                    if (ctrlLastNegative.VerseNumber != (int) LineNumberLink.Tag)
                    {
                        ScrollIntoView(ctrlLastNegative, true);
                        return;
                    }
                }

                SetLineNumberLink(ctrlLastNegative);
            }
        }

        private VerseBtControl VerseControlJustAboveDisplayRectange
        {
            get
            {
                VerseBtControl ctrlLastNegative = null;
                foreach (Control control in Controls)
                {
                    if (control is VerseBtControl)
                    {
                        if (control.Bounds.Y <= 0)
                            ctrlLastNegative = control as VerseBtControl;
                        else
                            break;
                    }
                }
                return ctrlLastNegative;
            }
        }

        private void SetLineNumberLink(VerseControl ctrl)
        {
            if (ctrl != null)
            {
                LineNumberLink.Text = String.Format("Ln: {0}",
                                                    ctrl.VerseNumber);
                LineNumberLink.Tag = ctrl.VerseNumber;
            }
        }

        public Control LastControlIntoView { get; set; }

        public void ScrollIntoView(VerseBtControl ctrl, bool bSetFocusToo)
        {
            LastControlIntoView = ctrl;
            ScrollControlIntoView(ctrl);

            // technically, it may not have been moved much, so the given control
            //  may *not* be the one where the link label should point to
            SetLineNumberLink(VerseControlJustAboveDisplayRectange);

            if (bSetFocusToo)
                ctrl.Focus();
        }

        protected Control NextControlUp
        {
            get
            {
                if (LastControlIntoView == null)
                    LastControlIntoView = InitialControl;

                return NextControlUpFrom(LastControlIntoView);
            }
        }

        protected Control NextControlDown
        {
            get
            {
                if (LastControlIntoView == null)
                    LastControlIntoView = InitialControl;

                return NextControlDownFrom(LastControlIntoView);
            }
        }

        protected virtual Control InitialControl
        {
            get
            {
                if (Controls.Count > 0)
                {
                    System.Diagnostics.Debug.Assert(Controls[0] is VerseControl);
                    return Controls[0];
                }
                return null;
            }
        }

        protected virtual Control NextControlUpFrom(Control ctrlFrom)
        {
            if (ctrlFrom != null)
            {
                int nIndex = Controls.IndexOf(ctrlFrom);
                if (nIndex > 0)
                {
                    System.Diagnostics.Debug.Assert(Controls[nIndex - 1] is VerseControl);
                    return Controls[nIndex - 1];
                }
            }
            return null;
        }

        protected virtual Control NextControlDownFrom(Control ctrlFrom)
        {
            if (ctrlFrom != null)
            {
                int nIndex = Controls.IndexOf(ctrlFrom);
                if ((nIndex + 1) < Controls.Count)
                {
                    System.Diagnostics.Debug.Assert(Controls[nIndex + 1] is VerseControl);
                    return Controls[nIndex + 1];
                }
            }
            return null;
        }
    }

    public class VerseBtLineFlowLayoutPanel : LineFlowLayoutPanel
    {
        public override VerseControl GetControlAtVerseIndex(int nVerseIndex)
        {
            // this used to just be 
            // return Controls[((nVerseIndex - 1)*2) + 1]
            // but some verses may be hidden and so it's not so straightforward
            // start at that index (going backwards) and make sure the verse index matches
            for (int i = Math.Min((((nVerseIndex - 1) * 2) + 1), Controls.Count - 2); i >= 0; i -= 2)
            {
                Control ctrl = Controls[i];
                System.Diagnostics.Debug.Assert(ctrl is VerseControl);
                VerseControl ctrlVerse = ctrl as VerseControl;
                if (ctrlVerse.VerseNumber == nVerseIndex)
                    return ctrlVerse;
            }

            return null;
        }

        protected override Control InitialControl
        {
            get
            {
                if (Controls.Count > 2)
                {
                    System.Diagnostics.Debug.Assert(Controls[1] is VerseControl);
                    return Controls[1];
                }
                return null;
            }
        }

        protected override Control NextControlUpFrom(Control ctrlFrom)
        {
            if (ctrlFrom != null)
            {
                int nIndex = Controls.IndexOf(ctrlFrom);
                if (nIndex > 2)
                {
                    System.Diagnostics.Debug.Assert(Controls[nIndex - 2] is VerseControl);
                    return Controls[nIndex - 2];
                }
            }
            return null;
        }

        protected override Control NextControlDownFrom(Control ctrlFrom)
        {
            if (ctrlFrom != null)
            {
                int nIndex = Controls.IndexOf(ctrlFrom);
                if ((nIndex + 2) < Controls.Count)
                {
                    System.Diagnostics.Debug.Assert(Controls[nIndex + 2] is VerseControl);
                    return Controls[nIndex + 2];
                }
            }
            return null;
        }
    }
}
