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
        protected const string cstrSuffixTextBox = "TextBox";
        protected const string cstrSuffixLabel = "Label";

        protected const string cstrFieldNameVernacular = "Vernacular";
        protected const string cstrFieldNameNationalBT = "NationalBT";
        protected const string cstrFieldNameInternationalBT = "InternationalBT";
        protected const string cstrFieldNameAnchors = "Anchors";

        protected StoryProject.verseRow m_aVerse = null;   // TODO: change this isn't a class that can do linq writes
        protected int m_nRowIndexVernacular = -1;
        protected int m_nRowIndexNationalBT = -1;
        protected int m_nRowIndexInternationalBT = -1;
        protected int m_nRowIndexAnchors = -1;

        public VerseBtControl(StoryEditor aSE, StoryProject.verseRow aVerse, int nVerseNumber)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = String.Format("Verse: {0}", nVerseNumber);
            this.tableLayoutPanel.SetColumnSpan(this.labelReference, 2);
            this.tableLayoutPanel.Controls.Add(this.labelReference);

            m_aVerse = aVerse;
            UpdateView(aSE);

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public override void UpdateView(StoryEditor aSE)
        {
            int nNumRows = 1;
            if (aSE.viewVernacularLangFieldMenuItem.Checked)
            {
                m_nRowIndexVernacular = nNumRows++;
                InitTextBox(cstrFieldNameVernacular, m_aVerse.GetVernacularRows()[0].lang, m_aVerse.GetVernacularRows()[0].Vernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, m_nRowIndexVernacular);
            }
            else if (m_nRowIndexVernacular != -1)
            {
                RemoveRow(m_nRowIndexVernacular);
                m_nRowIndexVernacular = -1;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                m_nRowIndexNationalBT = nNumRows++;
                InitTextBox(cstrFieldNameNationalBT, m_aVerse.GetNationalBTRows()[0].lang, m_aVerse.GetNationalBTRows()[0].NationalBT_text, aSE.NationalBTFont, aSE.NationalBTFontColor, m_nRowIndexNationalBT);
            }
            else if (m_nRowIndexNationalBT != -1)
            {
                RemoveRow(m_nRowIndexNationalBT);
                m_nRowIndexNationalBT = -1;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                m_nRowIndexInternationalBT = nNumRows++;
                InitTextBox(cstrFieldNameInternationalBT, m_aVerse.GetInternationalBTRows()[0].lang, m_aVerse.GetInternationalBTRows()[0].InternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, m_nRowIndexInternationalBT);
            }
            else if (m_nRowIndexInternationalBT != -1)
            {
                RemoveRow(m_nRowIndexInternationalBT);
                m_nRowIndexInternationalBT = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                StoryProject.anchorsRow[] anAnchorsRow = m_aVerse.GetanchorsRows();
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
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexNationalBT >= nLayoutRowIndex)
                m_nRowIndexNationalBT++;
            if (m_nRowIndexInternationalBT >= nLayoutRowIndex)
                m_nRowIndexInternationalBT++;
            if (m_nRowIndexAnchors >= nLayoutRowIndex)
                m_nRowIndexAnchors++;
        }

        protected override void RemoveRow(int nLayoutRowIndex)
        {
            base.RemoveRow(nLayoutRowIndex);
            if (m_nRowIndexNationalBT > nLayoutRowIndex)
                m_nRowIndexNationalBT--;
            if (m_nRowIndexInternationalBT > nLayoutRowIndex)
                m_nRowIndexInternationalBT--;
            if (m_nRowIndexAnchors > nLayoutRowIndex)
                m_nRowIndexAnchors--;
        }

        protected void InitAnchors(StoryEditor aSE, StoryProject.anchorsRow anAnchorsRow, int nLayoutRow, ref int nNumRows)
        {
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(this, anAnchorsRow);
                anAnchorCtrl.Name = cstrFieldNameAnchors;
                
                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
                tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanel.GetControlFromPosition(1, nLayoutRow);
                System.Diagnostics.Debug.Assert(ctrl.Name == cstrFieldNameAnchors);
            }
#endif
        }

        protected void InitTextBox(string strTbName, string strTbLabel, string strTbText, Font font, Color color, int nLayoutRow)
        {
            if (!tableLayoutPanel.Controls.ContainsKey(strTbName + cstrSuffixLabel))
            {
                InsertRow(nLayoutRow);

                // add the column0 row label
                Label lbl = new Label();
                lbl.Name = strTbName + cstrSuffixLabel;
                lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
                lbl.AutoSize = true;
                lbl.Text = strTbLabel;
                this.tableLayoutPanel.Controls.Add(lbl, 0, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanel.GetControlFromPosition(0, nLayoutRow);
                System.Diagnostics.Debug.Assert((ctrl.Name == strTbName + cstrSuffixLabel));
            }
#endif
            if (!tableLayoutPanel.Controls.ContainsKey(strTbName + cstrSuffixTextBox))
            {
                TextBox tb = new TextBox();
                tb.Name = strTbName + cstrSuffixTextBox;
                tb.Multiline = true;
                tb.Font = font;
                tb.ForeColor = color;
                tb.Dock = DockStyle.Fill;
                tb.Text = strTbText;
                tb.TextChanged += new EventHandler(textBox_TextChanged);
                this.tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanel.GetControlFromPosition(1, nLayoutRow);
                System.Diagnostics.Debug.Assert((ctrl.Name == strTbName + cstrSuffixTextBox));
            }
#endif
        }
    }
}
