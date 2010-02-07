using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class LineFlowLayoutPanel : FlowLayoutPanel
    {
        public LineFlowLayoutPanel()
        {
            InitializeComponent();
        }

        public void DoMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        private void LineFlowLayoutPanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (e.NewValue == e.OldValue)
                    return;

                bool bScrollDown = (e.NewValue > e.OldValue);

                int nControlCount = 1;
                if (    (e.Type == ScrollEventType.LargeDecrement)
                    || (e.Type == ScrollEventType.LargeIncrement))
                {
                    nControlCount = 3;
                }

                Control ctrlNext = null;
                for (; nControlCount-- > 0; )
                {
                    Control ctrlTemp;
                    if (bScrollDown)
                    {
                        ctrlTemp = NextControlDown;
                    }
                    else
                    {
                        ctrlTemp = NextControlUp;
                    }

                    if (ctrlTemp != null)
                        ctrlNext = LastControlIntoView = ctrlTemp;
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
