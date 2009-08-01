#define RemoveLater

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class VerseBtControl : ResizableControl
    {
        protected const string cstrFieldNameStoryLine = "StoryLine";
        protected const string cstrFieldNameAnchors = "Anchors";

        protected StoryProject.verseRow m_aVerseRow = null;   // TODO: change this isn't a class that can do linq writes

        protected int m_nRowIndexAnchors = -1;
        protected int m_nRowIndexStoryLine = -1;

        public VerseBtControl(StoryEditor aSE, StoryProject.verseRow aVerseRow, int nVerseNumber)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = String.Format("Verse: {0}", nVerseNumber);
            this.tableLayoutPanel.SetColumnSpan(this.labelReference, 2);
            this.tableLayoutPanel.Controls.Add(this.labelReference);

            m_aVerseRow = aVerseRow;
            UpdateView(aSE);

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public override void UpdateView(StoryEditor aSE)
        {
            int nNumRows = 1;
            // if the user is requesting one of the story lines (vernacular, nationalBT, or English), then...
            if (aSE.viewVernacularLangFieldMenuItem.Checked || aSE.viewNationalLangFieldMenuItem.Checked || aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((m_nRowIndexStoryLine == -1) || (m_nRowIndexStoryLine == nNumRows), "fix bad assumption (VerseBtControl.cs.49): bob_eaton@sall.com");

                // ask that control to do the Update View
                InitStoryLine(aSE, m_aVerseRow, nNumRows);
                m_nRowIndexStoryLine = nNumRows++;
            }
            else if (m_nRowIndexStoryLine != -1)
            {
                RemoveRow(m_nRowIndexStoryLine);
                m_nRowIndexStoryLine = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((m_nRowIndexAnchors == -1) || (m_nRowIndexAnchors == nNumRows), "fix bad assumption (VerseBtControl.cs.64): bob_eaton@sall.com");

                StoryProject.anchorsRow[] anAnchorsRow = m_aVerseRow.GetanchorsRows();
                System.Diagnostics.Debug.Assert(anAnchorsRow != null);
                if (anAnchorsRow != null)
                {
                    System.Diagnostics.Debug.Assert(anAnchorsRow.Length > 0);
                    InitAnchors(aSE, anAnchorsRow[0], nNumRows);
                    m_nRowIndexAnchors = nNumRows++;
                }
            }
            else if (m_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexAnchors);
                m_nRowIndexAnchors = -1;
            }
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors >= nLayoutRowIndex)
                m_nRowIndexAnchors++;
        }

        protected override void RemoveRow(int nLayoutRowIndex)
        {
            base.RemoveRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors > nLayoutRowIndex)
                m_nRowIndexAnchors--;
        }

        protected void InitStoryLine(StoryEditor aSE, StoryProject.verseRow aVerseRow, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike with the Anchor control, which is all on or all off)
            System.Diagnostics.Debug.Assert((m_nRowIndexStoryLine != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameStoryLine));
            if (tableLayoutPanel.Controls.ContainsKey(cstrFieldNameStoryLine))
            {
                StoryLineControl aStoryLineCtrl = (StoryLineControl)tableLayoutPanel.Controls[cstrFieldNameStoryLine];
                aStoryLineCtrl.UpdateView(aSE);
            }
            else
            {
                StoryLineControl aStoryLineCtrl = new StoryLineControl(aSE, aVerseRow);
                aStoryLineCtrl.Name = cstrFieldNameStoryLine;
                aStoryLineCtrl.ParentControl = this;

                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aStoryLineCtrl, 2);
                tableLayoutPanel.Controls.Add(aStoryLineCtrl, 0, nLayoutRow);
            }
        }

        protected void InitAnchors(StoryEditor aSE, StoryProject.anchorsRow anAnchorsRow, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike here with the Anchor control, which is all on or all off)
            System.Diagnostics.Debug.Assert((m_nRowIndexAnchors != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors));
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(anAnchorsRow);
                anAnchorCtrl.Name = cstrFieldNameAnchors;
                anAnchorCtrl.ParentControl = this;

                
                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
                tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
            }
        }
    }
}
