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
#if RemoveLater
        protected const string cstrFieldNameStoryLine = "StoryLine";
        protected const string cstrFieldNameAnchors = "Anchors";
#else
        protected const string cstrFieldNameVernacular = "Vernacular";
        protected const string cstrFieldNameNationalBT = "NationalBT";
        protected const string cstrFieldNameInternationalBT = "InternationalBT";
#endif

        protected StoryProject.verseRow m_aVerseRow = null;   // TODO: change this isn't a class that can do linq writes

#if RemoveLater
        protected int m_nRowIndexAnchors = -1;
        protected int m_nRowIndexStoryLine = -1;
#else
        protected int m_nRowIndexVernacular = -1;
        protected int m_nRowIndexNationalBT = -1;
        protected int m_nRowIndexInternationalBT = -1;
#endif

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
#if RemoveLater
            int nNumRows = 1;
            // if the user is requesting one of the story lines (vernacular, nationalBT, or English), then...
            if (aSE.viewVernacularLangFieldMenuItem.Checked || aSE.viewNationalLangFieldMenuItem.Checked || aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // ask that control to do the Update View
                m_nRowIndexStoryLine = nNumRows++;
                InitStoryLine(aSE, m_aVerseRow, m_nRowIndexStoryLine);
            }
            else if (m_nRowIndexStoryLine != -1)
            {
                RemoveRow(m_nRowIndexStoryLine);
                m_nRowIndexStoryLine = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                StoryProject.anchorsRow[] anAnchorsRow = m_aVerseRow.GetanchorsRows();
                System.Diagnostics.Debug.Assert(anAnchorsRow != null);
                if (anAnchorsRow != null)
                {
                    m_nRowIndexAnchors = nNumRows++;
                    System.Diagnostics.Debug.Assert(anAnchorsRow.Length > 0);
                    InitAnchors(aSE, anAnchorsRow[0], m_nRowIndexAnchors);
                }
            }
            else if (m_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexAnchors);
                m_nRowIndexAnchors = -1;
            }
#else
            int nNumRows = 1;
            if (aSE.viewVernacularLangFieldMenuItem.Checked)
            {
                m_nRowIndexVernacular = nNumRows++;
                InitTextBox(cstrFieldNameVernacular, m_aVerseRow.GetVernacularRows()[0].lang, m_aVerseRow.GetVernacularRows()[0].Vernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, m_nRowIndexVernacular);
            }
            else if (m_nRowIndexVernacular != -1)
            {
                RemoveRow(m_nRowIndexVernacular);
                m_nRowIndexVernacular = -1;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                m_nRowIndexNationalBT = nNumRows++;
                InitTextBox(cstrFieldNameNationalBT, m_aVerseRow.GetNationalBTRows()[0].lang, m_aVerseRow.GetNationalBTRows()[0].NationalBT_text, aSE.NationalBTFont, aSE.NationalBTFontColor, m_nRowIndexNationalBT);
            }
            else if (m_nRowIndexNationalBT != -1)
            {
                RemoveRow(m_nRowIndexNationalBT);
                m_nRowIndexNationalBT = -1;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                m_nRowIndexInternationalBT = nNumRows++;
                InitTextBox(cstrFieldNameInternationalBT, m_aVerseRow.GetInternationalBTRows()[0].lang, m_aVerseRow.GetInternationalBTRows()[0].InternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, m_nRowIndexInternationalBT);
            }
            else if (m_nRowIndexInternationalBT != -1)
            {
                RemoveRow(m_nRowIndexInternationalBT);
                m_nRowIndexInternationalBT = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                StoryProject.anchorsRow[] anAnchorsRow = m_aVerseRow.GetanchorsRows();
                System.Diagnostics.Debug.Assert(anAnchorsRow != null);
                if (anAnchorsRow != null)
                {
                    m_nRowIndexAnchors = nNumRows++;
                    System.Diagnostics.Debug.Assert(anAnchorsRow.Length > 0);
                    InitAnchors(aSE, anAnchorsRow[0], m_nRowIndexAnchors, ref nNumRows);
                }
            }
            else if (m_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexAnchors);
                m_nRowIndexAnchors = -1;
            }
#endif
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors > nLayoutRowIndex)
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
            if (tableLayoutPanel.Controls.ContainsKey(cstrFieldNameStoryLine))
            {
                StoryLineControl aStoryLineCtrl = (StoryLineControl)tableLayoutPanel.Controls[cstrFieldNameStoryLine];
                aStoryLineCtrl.UpdateView(aSE);
            }
            else
            {
                StoryLineControl aStoryLineCtrl = new StoryLineControl(aSE, aVerseRow);
                aStoryLineCtrl.Name = cstrFieldNameStoryLine;

                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aStoryLineCtrl, 2);
                tableLayoutPanel.Controls.Add(aStoryLineCtrl, 0, nLayoutRow);
            }
        }

        protected void InitAnchors(StoryEditor aSE, StoryProject.anchorsRow anAnchorsRow, int nLayoutRow)
        {
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(this, anAnchorsRow);
                anAnchorCtrl.Name = cstrFieldNameAnchors;
                
                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
                tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
            }
        }
    }
}
