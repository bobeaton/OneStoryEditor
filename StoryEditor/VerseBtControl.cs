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
        protected int m_nWidth = 0;

        public VerseBtControl(StoryEditor aSE, StoryProject.verseRow aVerse, int nVerseNumber)
        {
            InitializeComponent();

            this.labelReference.Text = String.Format("Verse: {0}", nVerseNumber);

            InitTextBox("Vernacular", aVerse.GetVernacularRows()[0].lang, aVerse.GetVernacularRows()[0].Vernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, 1);
            InitTextBox("NationalBT", aVerse.GetNationalBTRows()[0].lang, aVerse.GetNationalBTRows()[0].NationalBT_text, aSE.NationalBTFont, aSE.NationalBTFontColor, 2);
            InitTextBox("InternationalBT", aVerse.GetInternationalBTRows()[0].lang, aVerse.GetInternationalBTRows()[0].InternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, 3);

            StoryProject.anchorsRow[] anAnchorsRow = aVerse.GetanchorsRows();
            if (anAnchorsRow != null)
            {
                System.Diagnostics.Debug.Assert(anAnchorsRow.Length > 0);
                foreach (StoryProject.anchorRow anAnchorRow in anAnchorsRow[0].GetanchorRows())
                {
                    InitAnchorButton(anAnchorRow.jumpTarget, anAnchorRow.text);
                }

                toolStripAnchors.Items.Add(toolStripButtonNewAnchor);
            }
        }

        protected void InitAnchorButton(string strJumpTarget, string strComment)
        {
            ToolStripButton aButton = new ToolStripButton();
            aButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            aButton.Name = "toolStripButtonNewAnchor";
            aButton.AutoSize = true;
            aButton.Text = strJumpTarget;
            aButton.ToolTipText = strComment;
            toolStripAnchors.Items.Add(aButton);
        }

        protected void InitTextBox(string strTbName, string strTbLabel, string strTbText, Font font, Color color, int nLayoutRow)
        {
            Label lbl = new Label();
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lbl.AutoSize = true;
            lbl.Name = "label" + strTbName;
            lbl.Text = strTbLabel;
            this.tableLayoutPanelVerse.Controls.Add(lbl, 0, nLayoutRow);

            TextBox tb = new TextBox();
            tb.Name = "textBox" + strTbName;
            tb.Multiline = true;
            tb.Font = font;
            tb.ForeColor = color;
            tb.Dock = DockStyle.Fill;
            tb.Text = strTbText;
            tb.TextChanged += new EventHandler(textBox_TextChanged);
            this.tableLayoutPanelVerse.Controls.Add(tb, 1, nLayoutRow);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (ResizeTextBoxToFitText(tb))
                SetWidth(m_nWidth);
        }

        public void SetWidth(int nWidth)
        {
            m_nWidth = nWidth;
            SetSize(m_nWidth);
        }

        protected bool ResizeTextBoxToFitText(TextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(tb.Width, 1000));
            bool bHeightChanged = (sz.Height != tb.Size.Height);
            if (bHeightChanged)
                tb.Size = sz;
            return bHeightChanged;
        }

        protected void SetSize(int nWidth)
        {
            this.tableLayoutPanelVerse.SuspendLayout();
            this.SuspendLayout();

            // set the width to the new width given by caller
            this.tableLayoutPanelVerse.Width = nWidth - this.Margin.Left - SystemInformation.VerticalScrollBarWidth;
            
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
                catch { /* skip any non-text boxes */ }
            }

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
