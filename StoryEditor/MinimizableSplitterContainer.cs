using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class MinimizableSplitterContainer : SplitContainer
    {
        private const int CnNetBibleHiddenHeight = 30;
        private int _originalSplitterDistance;

        public void Minimize()
        {
            int nNewSplitterDistance = MinimizedSplitterDistance;
            if (nNewSplitterDistance != SplitterDistance)
            {
                _originalSplitterDistance = SplitterDistance;
                SplitterDistance = nNewSplitterDistance;
            }
        }

        public void Restore()
        {
            SplitterDistance = _originalSplitterDistance;
        }

        public bool IsMinimized
        {
            get { return (SplitterDistance == MinimizedSplitterDistance); }
        }

        public bool IsRestored
        {
            get { return (SplitterDistance == _originalSplitterDistance); }
        }

        protected int MinimizedSplitterDistance
        {
            get { return DisplayRectangle.Height - CnNetBibleHiddenHeight; }
        }
    }
}
