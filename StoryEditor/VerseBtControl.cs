using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StoryEditor
{
    public partial class VerseBtControl : UserControl
    {
        protected const string cstrSuffixTextBox = "TextBox";
        protected const string cstrSuffixLabel = "Label";

        protected const string cstrFieldNameVernacular = "Vernacular";
        protected const string cstrFieldNameNationalBT = "NationalBT";
        protected const string cstrFieldNameInternationalBT = "InternationalBT";
        protected const string cstrFieldNameAnchors = "Anchors";

        protected StoryProject.verseRow m_aVerse = null;
        protected int m_nRowIndexVernacular = -1, m_nRowIndexNationalBT = -1, m_nRowIndexInternationalBT = -1,
            m_nRowIndexAnchors = -1;

        public VerseBtControl(int nVerseNumber, StoryProject.verseRow aVerse)
        {
            InitializeComponent();

            m_aVerse = aVerse;
            this.labelReference.Text = String.Format("Verse: {0}", nVerseNumber);
        }

        public void UpdateView(StoryEditor aSE, int nWidth)
        {
            // set the width to the new width given by caller
            this.SuspendLayout();
            this.tableLayoutPanelVerse.SuspendLayout();

            tableLayoutPanelVerse.Width = this.Width = nWidth - this.Margin.Left - SystemInformation.VerticalScrollBarWidth;
            
            this.tableLayoutPanelVerse.ResumeLayout(false);
            this.tableLayoutPanelVerse.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

            AdjustHeight();
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected void InsertRow(int nLayoutRowIndex)
        {
            tableLayoutPanelVerse.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexNationalBT >= nLayoutRowIndex)
                m_nRowIndexNationalBT++;
            if (m_nRowIndexInternationalBT >= nLayoutRowIndex)
                m_nRowIndexInternationalBT++;
            if (m_nRowIndexAnchors >= nLayoutRowIndex)
                m_nRowIndexAnchors++;
        }

        protected void RemoveRow(int nLayoutRowIndex)
        {
            tableLayoutPanelVerse.RemoveRow(nLayoutRowIndex);
            if (m_nRowIndexNationalBT > nLayoutRowIndex)
                m_nRowIndexNationalBT--;
            if (m_nRowIndexInternationalBT > nLayoutRowIndex)
                m_nRowIndexInternationalBT--;
            if (m_nRowIndexAnchors > nLayoutRowIndex)
                m_nRowIndexAnchors--;
        }

        protected void InitAnchors(StoryEditor aSE, StoryProject.anchorsRow anAnchorsRow, int nLayoutRow, ref int nNumRows)
        {
            if (!tableLayoutPanelVerse.Controls.ContainsKey(cstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(this);
                anAnchorCtrl.UpdateView(aSE, anAnchorsRow, (int)tableLayoutPanelVerse.ColumnStyles[1].Width);

                InsertRow(nLayoutRow);
                tableLayoutPanelVerse.Controls.Add(labelAnchor, 0, nLayoutRow);
                tableLayoutPanelVerse.Controls.Add(anAnchorCtrl, 1, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanelVerse.GetControlFromPosition(1, nLayoutRow);
                System.Diagnostics.Debug.Assert((ctrl.Name == cstrFieldNameAnchors) && ((ToolStrip)ctrl).Items.Count == anAnchorsRow.GetanchorRows().Length);
            }
#endif
        }

        protected void InitTextBox(string strTbName, string strTbLabel, string strTbText, Font font, Color color, int nLayoutRow)
        {
            if (!tableLayoutPanelVerse.Controls.ContainsKey(strTbName + cstrSuffixLabel))
            {
                InsertRow(nLayoutRow);

                // add the column0 row label
                Label lbl = new Label();
                lbl.Name = strTbName + cstrSuffixLabel;
                lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
                lbl.AutoSize = true;
                lbl.Text = strTbLabel;
                this.tableLayoutPanelVerse.Controls.Add(lbl, 0, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanelVerse.GetControlFromPosition(0, nLayoutRow);
                System.Diagnostics.Debug.Assert((ctrl.Name == strTbName + cstrSuffixLabel));
            }
#endif
            if (!tableLayoutPanelVerse.Controls.ContainsKey(strTbName + cstrSuffixTextBox))
            {
                TextBox tb = new TextBox();
                tb.Name = strTbName + cstrSuffixTextBox;
                tb.Multiline = true;
                tb.Font = font;
                tb.ForeColor = color;
                tb.Dock = DockStyle.Fill;
                tb.Text = strTbText;
                tb.TextChanged += new EventHandler(textBox_TextChanged);
                this.tableLayoutPanelVerse.Controls.Add(tb, 1, nLayoutRow);
            }
#if DEBUG
            else
            {
                Control ctrl = tableLayoutPanelVerse.GetControlFromPosition(1, nLayoutRow);
                System.Diagnostics.Debug.Assert((ctrl.Name == strTbName + cstrSuffixTextBox));
            }
#endif
        }

        protected void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (ResizeTextBoxToFitText(tb))
                AdjustHeight();
        }

        internal bool ResizeTextBoxToFitText(TextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(tb.Width, 1000));
            bool bHeightChanged = (sz.Height != tb.Size.Height);
            if (bHeightChanged)
                tb.Size = sz;
            return bHeightChanged;
        }


        internal void AdjustHeight()
        {
            this.tableLayoutPanelVerse.SuspendLayout();
            this.SuspendLayout();

            /*
            // go thru all the controls and ...
            foreach (Control aCtrl in tableLayoutPanelVerse.Controls)
            {
                try
                {
                    // for all the text boxes, set them to the fixed width, but high as possible (so it will limit
                    //  at whatever is neede)
                    TextBox tb = (TextBox)aCtrl;
                    ResizeTextBoxToFitText(tb);
                }
                catch { } // skip any non-text boxes
            }
            */

            // do a similar thing with the layout panel (i.e. give it the same width and infinite height.
            Size sz = this.tableLayoutPanelVerse.GetPreferredSize(new Size(tableLayoutPanelVerse.Width, 1000));
            sz.Height += tableLayoutPanelVerse.Margin.Bottom;
            this.tableLayoutPanelVerse.Size = this.Size = sz;

            this.tableLayoutPanelVerse.ResumeLayout(false);
            this.tableLayoutPanelVerse.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
