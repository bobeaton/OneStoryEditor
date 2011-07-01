using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class MultiLineControl : ResizableControl
    {
        public MultiLineControl(VerseBtControl ctrlVerse, StoryStageLogic storyStageLogic, 
            MultipleLineDataConverter aMLDC, ProjectSettings projSettings,
            TestInfo lstTestInfo, string strLabelSuffix,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT,
            Color clrVernacular, Color clrNationalBt, Color clrInternationalBt)
            : base(storyStageLogic)
        {
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, fix this assumption: RetellingsControl.cs.20");
            tableLayoutPanel.RemoveRow(1);  // remove the one default one we start with (we add them back later)

            // remove the columns, because we're going to add them back as equal sizes.
            // But leave 1 for the label
            while (tableLayoutPanel.ColumnCount > 1)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            // finally populate the buttons on that tool strip
            System.Diagnostics.Debug.Assert(aMLDC.Count > 0);
            for (int nNumRows = 0; nNumRows < aMLDC.Count; nNumRows++)
            {
                LineMemberData aLineData = aMLDC[nNumRows];
                string strUnsGui = aLineData.MemberId;
                System.Diagnostics.Debug.Assert(lstTestInfo.Contains(strUnsGui));
                int nTest = lstTestInfo.IndexOf(strUnsGui) + 1;
                string strLabelRow = String.Format(aMLDC.LabelTextFormat, nTest);
                InitRow(strLabelRow, nNumRows);

                int nNumColumns = 1;
                CtrlTextBox ctrlTextBoxVernacular = null;
                if (bShowVernacular)
                {
                    if (nNumRows == 0)
                        InsertColumn(nNumColumns);
                    aLineData.Vernacular.Transliterator = VerseBtControl.TransliteratorVernacular;
                    ctrlTextBoxVernacular = InitTextBox(ctrlVerse, LineData.CstrAttributeLangVernacular,
                                                        aLineData.Vernacular, strLabelRow + strLabelSuffix, 
                                                        projSettings.Vernacular,
                                                        nNumColumns, nNumRows, 
                                                        StoryEditor.TextFieldType.Vernacular,
                                                        clrVernacular);
                    nNumColumns++;
                }

                CtrlTextBox ctrlTextBoxNationalBT = null;
                if (bShowNationalBT)
                {
                    if (nNumRows == 0)
                        InsertColumn(nNumColumns);
                    aLineData.NationalBt.Transliterator = VerseBtControl.TransliteratorNationalBt;
                    ctrlTextBoxNationalBT = InitTextBox(ctrlVerse, LineData.CstrAttributeLangNationalBt,
                                                        aLineData.NationalBt, strLabelRow + strLabelSuffix, 
                                                        projSettings.NationalBT,
                                                        nNumColumns, nNumRows, 
                                                        StoryEditor.TextFieldType.NationalBt,
                                                        clrNationalBt);
                    nNumColumns++;

                    if (ctrlTextBoxVernacular != null)
                        ctrlTextBoxVernacular.NationalBtSibling = ctrlTextBoxNationalBT;
                }

                if (bShowInternationalBT)
                {
                    if (nNumRows == 0)
                        InsertColumn(nNumColumns);
                    aLineData.InternationalBt.Transliterator = VerseBtControl.TransliteratorInternationalBt;
                    CtrlTextBox ctrlTextBoxEnglishBT = InitTextBox(ctrlVerse, LineData.CstrAttributeLangInternationalBt,
                                                                   aLineData.InternationalBt,
                                                                   strLabelRow + strLabelSuffix,
                                                                   projSettings.InternationalBT,
                                                                   nNumColumns, nNumRows,
                                                                   StoryEditor.TextFieldType.InternationalBt,
                                                                   clrInternationalBt);
                    nNumColumns++;

                    if (ctrlTextBoxVernacular != null)
                        ctrlTextBoxVernacular.EnglishBtSibling = ctrlTextBoxEnglishBT;

                    if (ctrlTextBoxNationalBT != null)
                        ctrlTextBoxNationalBT.EnglishBtSibling = ctrlTextBoxEnglishBT;
                }
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected CtrlTextBox InitTextBox(VerseControl ctrlVerse, string strTbName,
            StringTransfer strTbText, string strLabel, ProjectSettings.LanguageInfo li, int nLayoutColumn,
            int nLayoutRow, StoryEditor.TextFieldType eFieldType, Color clrFont)
        {
            string strTextBoxName = strTbName + CstrSuffixTextBox + nLayoutRow + nLayoutColumn;
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTextBoxName), "otherwise, fix wrong assumption");
            var tb = new CtrlTextBox(strTextBoxName, ctrlVerse, this,
                strTbText, li, strLabel, eFieldType, clrFont);
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, nLayoutRow);
            return tb;
        }

        protected void InitRow(string strLabelText, int nNumRows)
        {
            var label = new Label
                              {
                                  Anchor = AnchorStyles.Right,
                                  AutoSize = true,
                                  Name = strLabelText,
                                  Text = strLabelText
                              };

            // add the label as a new row to the table layout panel
            InsertRow(nNumRows);
            tableLayoutPanel.Controls.Add(label, 0, nNumRows);
        }
    }
}
