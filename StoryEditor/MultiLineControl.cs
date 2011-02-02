using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class MultiLineControl : ResizableControl
    {
        public MultiLineControl(VerseBtControl ctrlVerse, StoryStageLogic storyStageLogic, 
            MultipleLineDataConverter aMLDC, ProjectSettings projSettings, List<string> astrTestors,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT)
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
                LineData aLineData = aMLDC[nNumRows];
                string strUnsGui = aLineData.MemberId;
                System.Diagnostics.Debug.Assert(astrTestors.Contains(strUnsGui));
                int nTest = astrTestors.IndexOf(strUnsGui) + 1;
                InitRow(aMLDC.LabelTextFormat, nTest, nNumRows);

                int nNumColumns = 1;
                CtrlTextBox ctrlTextBoxVernacular = null;
                if (bShowVernacular)
                {
                    InsertColumn(nNumColumns);
                    aLineData[StoryEditor.CnVernacular].Transliterator = ctrlVerse.TransliteratorVernacular;
                    ctrlTextBoxVernacular = InitTextBox(ctrlVerse, VerseData.CstrFieldNameVernacular,
                                                        aLineData[StoryEditor.CnVernacular], projSettings.Vernacular,
                                                        nNumColumns, nNumRows, 
                                                        StoryEditor.TextFieldType.eVernacular);
                    nNumColumns++;
                }

                CtrlTextBox ctrlTextBoxNationalBT = null;
                if (bShowNationalBT)
                {
                    InsertColumn(nNumColumns);
                    aLineData[StoryEditor.CnNationalBt].Transliterator = ctrlVerse.TransliteratorNationalBT;
                    ctrlTextBoxNationalBT = InitTextBox(ctrlVerse, VerseData.CstrFieldNameNationalBt,
                                                        aLineData[StoryEditor.CnNationalBt], projSettings.NationalBT,
                                                        nNumColumns, nNumRows, 
                                                        StoryEditor.TextFieldType.eNational);
                    nNumColumns++;

                    if (ctrlTextBoxVernacular != null)
                        ctrlTextBoxVernacular.NationalBtSibling = ctrlTextBoxNationalBT;
                }

                if (bShowInternationalBT)
                {
                    InsertColumn(nNumColumns);
                    CtrlTextBox ctrlTextBoxEnglishBT = InitTextBox(ctrlVerse, VerseData.CstrFieldNameInternationalBt,
                                                        aLineData[StoryEditor.CnInternationalBt], projSettings.InternationalBT,
                                                        nNumColumns, nNumRows, 
                                                        StoryEditor.TextFieldType.eInternational);
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
            StringTransfer strTbText, ProjectSettings.LanguageInfo li, int nLayoutColumn, 
            int nLayoutRow, StoryEditor.TextFieldType eFieldType)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + CstrSuffixTextBox), "otherwise, fix wrong assumption");
            var tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, ctrlVerse, this,
                strTbText, li, li.LangCode, eFieldType);
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, nLayoutRow);
            return tb;
        }

        protected void InitRow(string strLabelTextFormat, int nTest, int nNumRows)
        {
            Label label = new Label
                              {
                                  Anchor = AnchorStyles.Right,
                                  AutoSize = true,
                                  Name = strLabelTextFormat + nTest,
                                  Text = String.Format(strLabelTextFormat, nTest)
                              };

            // add the label as a new row to the table layout panel
            InsertRow(nNumRows);
            tableLayoutPanel.Controls.Add(label, 0, nNumRows);
        }
    }
}
