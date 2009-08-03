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
        internal const string cstrVerseName = "Verse: ";
        protected const string cstrFieldNameStoryLine = "StoryLine";
        protected const string cstrFieldNameAnchors = "Anchors";
        protected const string cstrFieldNameRetellings = "Retellings";

        protected StoryProject.verseRow m_aVerseRow = null;   // TODO: change this isn't a class that can do linq writes

        internal int VerseNumber = -1;
        protected int m_nRowIndexStoryLine = -1;
        protected int m_nRowIndexAnchors = -1;
        protected int m_nRowIndexRetelling = -1;

        public VerseBtControl(StoryEditor aSE, StoryProject.verseRow aVerseRow, int nVerseNumber)
        {
            VerseNumber = nVerseNumber;
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = cstrVerseName + nVerseNumber.ToString();
            this.tableLayoutPanel.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonDragDropHandle, 1, 0);

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
                    InitAnchors(anAnchorsRow[0], nNumRows);
                    m_nRowIndexAnchors = nNumRows++;
                }
            }
            else if (m_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexAnchors);
                m_nRowIndexAnchors = -1;
            }

            if (aSE.viewRetellingFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((m_nRowIndexRetelling == -1) || (m_nRowIndexRetelling == nNumRows), "fix bad assumption (VerseBtControl.cs.64): bob_eaton@sall.com");

                StoryProject.RetellingsRow[] aRetellingsRows = m_aVerseRow.GetRetellingsRows();
                if ((aRetellingsRows != null) && (aRetellingsRows.Length > 0))
                {
                    InitRetellings(aRetellingsRows[0], nNumRows);
                    m_nRowIndexRetelling = nNumRows++;
                }
            }
            else if (m_nRowIndexRetelling != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexRetelling);
                m_nRowIndexRetelling = -1;
            }
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors >= nLayoutRowIndex)
                m_nRowIndexAnchors++;
            if (m_nRowIndexRetelling >= nLayoutRowIndex)
                m_nRowIndexRetelling++;
        }

        protected override void RemoveRow(int nLayoutRowIndex)
        {
            base.RemoveRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors > nLayoutRowIndex)
                m_nRowIndexAnchors--;
            if (m_nRowIndexRetelling > nLayoutRowIndex)
                m_nRowIndexRetelling--;
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

        protected void InitAnchors(StoryProject.anchorsRow anAnchorsRow, int nLayoutRow)
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

        protected void InitRetellings(StoryProject.RetellingsRow aRetellingsRow, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike here with the Retellings control, which is all on or all off)
            System.Diagnostics.Debug.Assert((m_nRowIndexRetelling != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameRetellings));
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameRetellings))
            {
                RetellingsControl aRetellingsCtrl = new RetellingsControl(aRetellingsRow);
                aRetellingsCtrl.Name = cstrFieldNameRetellings;
                aRetellingsCtrl.ParentControl = this;

                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aRetellingsCtrl, 2);
                tableLayoutPanel.Controls.Add(aRetellingsCtrl, 0, nLayoutRow);
            }
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            buttonDragDropHandle.DoDragDrop(this, DragDropEffects.Move | DragDropEffects.Copy);
        }

        void buttonDragDropHandle_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            Console.WriteLine(String.Format("QueryContinueDrag: Action: {0}", e.Action.ToString()));

            Form form = FindForm();
            System.Diagnostics.Debug.Assert(form is StoryEditor);
            if (form is StoryEditor)
            {
                StoryEditor aSE = (StoryEditor)form;
                if (e.Action != DragAction.Continue)
                    aSE.DimDropTargetButtons();
                else
                    aSE.LightUpDropTargetButtons();
            }
        }
    }
}
