using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StoryLineControl : OneStoryProjectEditor.ResizableControl
    {
        protected const string cstrFieldNameVernacular = "Vernacular";
        protected const string cstrFieldNameNationalBT = "NationalBT";
        protected const string cstrFieldNameInternationalBT = "InternationalBT";

        protected StoryProject.verseRow m_aVerseRow = null;
        protected int m_nColumnIndexVernacular = -1;
        protected int m_nColumnIndexNationalBT = -1;
        protected int m_nColumnIndexInternationalBT = -1;

        public StoryLineControl(StoryEditor aSE, StoryProject.verseRow aVerseRow)
        {
            InitializeComponent();

            m_aVerseRow = aVerseRow;

            // clobber the base class table layout panel's configuration. We're 'column-oriented' instead
            // first add another row so that we have two rows (row(0)=label, row(1)=text)
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here");
            InsertRow(1);

            // remove the columns, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 0)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            UpdateView(aSE);
        }

        public override void  UpdateView(StoryEditor aSE)
        {
            int nNumColumns = 0;
            if (aSE.viewVernacularLangFieldMenuItem.Checked)
            {
                // if it isn't already initialized
                if (m_nColumnIndexVernacular == -1)
                {
                    m_nColumnIndexVernacular = nNumColumns++;
                    InsertColumn(m_nColumnIndexVernacular);
                    InitLabel(m_aVerseRow.GetVernacularRows()[0].lang, m_nColumnIndexVernacular);
                    InitTextBox(cstrFieldNameVernacular, m_aVerseRow.GetVernacularRows()[0].Vernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, m_nColumnIndexVernacular);
                }
            }
            else if (m_nColumnIndexVernacular != -1)
            {
                RemoveColumn(m_nColumnIndexVernacular);
                m_nColumnIndexVernacular = -1;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                // if it isn't already initialized
                if (m_nColumnIndexNationalBT == -1)
                {
                    m_nColumnIndexNationalBT = nNumColumns++;
                    InsertColumn(m_nColumnIndexNationalBT);
                    InitLabel(m_aVerseRow.GetNationalBTRows()[0].lang, m_nColumnIndexNationalBT);
                    InitTextBox(cstrFieldNameNationalBT, m_aVerseRow.GetNationalBTRows()[0].NationalBT_text, aSE.NationalBTFont, aSE.NationalBTFontColor, m_nColumnIndexNationalBT);
                }
            }
            else if (m_nColumnIndexNationalBT != -1)
            {
                RemoveColumn(m_nColumnIndexNationalBT);
                m_nColumnIndexNationalBT = -1;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // if it isn't already initialized
                if (m_nColumnIndexInternationalBT == -1)
                {
                    m_nColumnIndexInternationalBT = nNumColumns++;
                    InsertColumn(m_nColumnIndexInternationalBT);
                    InitLabel(m_aVerseRow.GetInternationalBTRows()[0].lang, m_nColumnIndexInternationalBT);
                    InitTextBox(cstrFieldNameInternationalBT, m_aVerseRow.GetInternationalBTRows()[0].InternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, m_nColumnIndexInternationalBT);
                }
            }
            else if (m_nColumnIndexInternationalBT != -1)
            {
                RemoveColumn(m_nColumnIndexInternationalBT);
                m_nColumnIndexInternationalBT = -1;
            }

#if DEBUG
            tableLayoutPanel.DumpTable();
#endif
        }

        protected void InitLabel(string strStoryLineLableName, int nLayoutColumn)
        {
            // add the row0 column label
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strStoryLineLableName + cstrSuffixLabel), "otherwise, fix wrong assumption");
            Label lbl = new Label();
            lbl.Name = strStoryLineLableName + cstrSuffixLabel;
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lbl.AutoSize = true;
            lbl.Text = strStoryLineLableName;
            this.tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(string strTbName, string strTbText, Font font, Color color, int nLayoutColumn)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + cstrSuffixTextBox), "otherwise, fix wrong assumption");
            TextBox tb = new TextBox();
            tb.Name = strTbName + cstrSuffixTextBox;
            tb.Multiline = true;
            tb.Font = font;
            tb.ForeColor = color;
            tb.Dock = DockStyle.Fill;
            tb.Text = strTbText;
            tb.TextChanged += new EventHandler(textBox_TextChanged);
            this.tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertColumn(int nLayoutColumnIndex)
        {
            base.InsertColumn(nLayoutColumnIndex);
            if (m_nColumnIndexNationalBT > nLayoutColumnIndex)
                m_nColumnIndexNationalBT++;
            if (m_nColumnIndexInternationalBT > nLayoutColumnIndex)
                m_nColumnIndexInternationalBT++;
        }

        protected override void RemoveColumn(int nLayoutColumnIndex)
        {
            base.RemoveColumn(nLayoutColumnIndex);
            if (m_nColumnIndexNationalBT > nLayoutColumnIndex)
                m_nColumnIndexNationalBT--;
            if (m_nColumnIndexInternationalBT > nLayoutColumnIndex)
                m_nColumnIndexInternationalBT--;
        }
    }
}
