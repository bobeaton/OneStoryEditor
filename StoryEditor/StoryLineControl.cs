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
        protected const string CstrFieldNameVernacular = "Vernacular";
        protected const string CstrFieldNameNationalBt = "NationalBT";
        protected const string CstrFieldNameInternationalBt = "InternationalBT";

        protected VerseData _aVerseData = null;
        protected int m_nColumnIndexVernacular = -1;
        protected int m_nColumnIndexNationalBT = -1;
        protected int m_nColumnIndexInternationalBT = -1;

        public StoryLineControl(StoryEditor aSE, VerseData aVerseData)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            _aVerseData = aVerseData;

            // clobber the base class table layout panel's configuration. We're 'column-oriented' instead
            // first add another row so that we have two rows (row(0)=label, row(1)=text)
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here: StoryLineControl.cs.30");
            InsertRow(1);

            // remove the columns, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 0)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            UpdateView(aSE);

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
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
                    InitLabel(aSE.Stories.ProjSettings.VernacularLangName, nNumColumns);
                    InitTextBox(CstrFieldNameVernacular, _aVerseData.VernacularText, aSE.Stories.ProjSettings.VernacularFont, aSE.Stories.ProjSettings.VernacularFontColor, nNumColumns);
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
                    InitLabel(aSE.Stories.ProjSettings.NationalBTLangName, nNumColumns);
                    InitTextBox(CstrFieldNameNationalBt, _aVerseData.NationalBTText, aSE.Stories.ProjSettings.NationalBTFont, aSE.Stories.ProjSettings.NationalBTFontColor, nNumColumns);
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
                    InitLabel(aSE.Stories.ProjSettings.InternationalBTLangName, nNumColumns);
                    InitTextBox(CstrFieldNameInternationalBt, _aVerseData.InternationalBTText, aSE.Stories.ProjSettings.InternationalBTFont, aSE.Stories.ProjSettings.InternationalBTFontColor, nNumColumns);
                    m_nColumnIndexInternationalBT = nNumColumns;
                }
                nNumColumns++;  // in either case, we have to bump the running count, because this control *is* there (whether new or old)
            }
            else if (m_nColumnIndexInternationalBT != -1)
            {
                RemoveColumn(m_nColumnIndexInternationalBT);
                m_nColumnIndexInternationalBT = -1;
            }
        }

        protected void InitLabel(string strStoryLineLableName, int nLayoutColumn)
        {
            // add the row0 column label
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strStoryLineLableName + CstrSuffixLabel), "otherwise, fix wrong assumption");
            Label lbl = new Label();
            lbl.Name = strStoryLineLableName + CstrSuffixLabel;
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lbl.AutoSize = true;
            lbl.Text = strStoryLineLableName;
            this.tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(string strTbName, StringTransfer strTbText, Font font, Color color, int nLayoutColumn)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + CstrSuffixTextBox), "otherwise, fix wrong assumption");
            TextBox tb = new TextBox();
            tb.Name = strTbName + CstrSuffixTextBox;
            tb.Multiline = true;
            tb.Font = font;
            tb.ForeColor = color;
            tb.Dock = DockStyle.Fill;
            strTbText.SetAssociation(tb);   // tb.Text = strTbText;
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
