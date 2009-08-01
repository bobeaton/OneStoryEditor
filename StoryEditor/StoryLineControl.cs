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
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here: StoryLineControl.cs.30");
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
                // if we've already initialized the control, then it must have this project column index (i.e. nNumColumns)
                System.Diagnostics.Debug.Assert((m_nColumnIndexVernacular == -1) || (m_nColumnIndexVernacular == nNumColumns), "fix bad assumption (StoryLineControl.cs.46): bob_eaton@sall.com");

                // if it isn't already initialized
                if (m_nColumnIndexVernacular == -1)
                {
                    InsertColumn(nNumColumns);
                    InitLabel(m_aVerseRow.GetVernacularRows()[0].lang, nNumColumns);
                    InitTextBox(cstrFieldNameVernacular, m_aVerseRow.GetVernacularRows()[0].Vernacular_text, aSE.VernacularFont, aSE.VernacularFontColor, nNumColumns);
                    m_nColumnIndexVernacular = nNumColumns;
                }
                nNumColumns++;  // in either case, we have to bump the running count, because this control *is* there (whether new or old)
            }
            else if (m_nColumnIndexVernacular != -1)
            {
                RemoveColumn(m_nColumnIndexVernacular);
                m_nColumnIndexVernacular = -1;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project column index (i.e. nNumColumns)
                System.Diagnostics.Debug.Assert((m_nColumnIndexNationalBT == -1) || (m_nColumnIndexNationalBT == nNumColumns), "fix bad assumption (StoryLineControl.cs.66): bob_eaton@sall.com");

                // if it isn't already initialized
                if (m_nColumnIndexNationalBT == -1)
                {
                    InsertColumn(nNumColumns);
                    InitLabel(m_aVerseRow.GetNationalBTRows()[0].lang, nNumColumns);
                    InitTextBox(cstrFieldNameNationalBT, m_aVerseRow.GetNationalBTRows()[0].NationalBT_text, aSE.NationalBTFont, aSE.NationalBTFontColor, nNumColumns);
                    m_nColumnIndexNationalBT = nNumColumns;
                }
                nNumColumns++;  // in either case, we have to bump the running count, because this control *is* there (whether new or old)
            }
            else if (m_nColumnIndexNationalBT != -1)
            {
                RemoveColumn(m_nColumnIndexNationalBT);
                m_nColumnIndexNationalBT = -1;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project column index (i.e. nNumColumns)
                System.Diagnostics.Debug.Assert((m_nColumnIndexInternationalBT == -1) || (m_nColumnIndexInternationalBT == nNumColumns), "fix bad assumption (StoryLineControl.cs.86): bob_eaton@sall.com");

                // if it isn't already initialized
                if (m_nColumnIndexInternationalBT == -1)
                {
                    InsertColumn(nNumColumns);
                    InitLabel(m_aVerseRow.GetInternationalBTRows()[0].lang, nNumColumns);
                    InitTextBox(cstrFieldNameInternationalBT, m_aVerseRow.GetInternationalBTRows()[0].InternationalBT_text, aSE.InternationalBTFont, aSE.InternationalBTFontColor, nNumColumns);
                    m_nColumnIndexInternationalBT = nNumColumns;
                }
                nNumColumns++;  // in either case, we have to bump the running count, because this control *is* there (whether new or old)
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
            if (m_nColumnIndexNationalBT >= nLayoutColumnIndex)
                m_nColumnIndexNationalBT++;
            if (m_nColumnIndexInternationalBT >= nLayoutColumnIndex)
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
