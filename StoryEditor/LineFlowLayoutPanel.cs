﻿using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class LineFlowLayoutPanel : FlowLayoutPanel
    {
        public LineFlowLayoutPanel()
        {
            InitializeComponent();
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
                    ScrollControlIntoView(ctrlNext);
            }
        }

        protected Control LastControlIntoView;

        public new void ScrollControlIntoView(Control ctrl)
        {
            LastControlIntoView = ctrl;
            base.ScrollControlIntoView(ctrl);
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
