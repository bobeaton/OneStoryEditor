#define TurnOffLabels
using System;
using System.Windows.Forms;
using ECInterfaces;
using SilEncConverters31;

namespace OneStoryProjectEditor
{
    public partial class StoryLineControl : OneStoryProjectEditor.ResizableControl
    {
        protected VerseData _aVerseData = null;

        public StoryLineControl(StoryEditor aSE, VerseBtControl ctrlVerse, VerseData aVerseData)
            : base(aSE.theCurrentStory.ProjStage)
        {
            InitializeComponent();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            _aVerseData = aVerseData;

            // clobber the base class table layout panel's configuration. We're 'column-oriented' instead
            // first add another row so that we have two rows (row(0)=label, row(1)=text)
            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, adjust assumption here: StoryLineControl.cs.30");
            InsertRow(1);

            // remove the columns, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 0)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            int nNumColumns = 0;
            if (aSE.viewVernacularLangFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.Vernacular.LangName, nNumColumns);

                // if we're in the one of the states where the user is entering in the 
                //  national or international BT, then disable the Vernacular as a tab stop.
                bool bDisableTabStopVernacular =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                _aVerseData.VernacularText.Transliterator = ctrlVerse.TransliteratorVernacular;
                InitTextBox(ctrlVerse, VerseData.CstrFieldNameVernacular, _aVerseData.VernacularText, 
                    aSE.StoryProject.ProjSettings.Vernacular, bDisableTabStopVernacular, 
                    nNumColumns);
                nNumColumns++;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.NationalBT.LangName, nNumColumns);

                // if we're in the one of the states where the user is entering in the 
                //  international BT, then disable the National BT as a tab stop.
                bool bDisableTabStopNationalBT =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                _aVerseData.NationalBTText.Transliterator = ctrlVerse.TransliteratorNationalBT;
                InitTextBox(ctrlVerse, VerseData.CstrFieldNameNationalBt, _aVerseData.NationalBTText, 
                    aSE.StoryProject.ProjSettings.NationalBT, bDisableTabStopNationalBT, 
                    nNumColumns);

                nNumColumns++;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.InternationalBT.LangName, nNumColumns);

                InitTextBox(ctrlVerse, VerseData.CstrFieldNameInternationalBt, _aVerseData.InternationalBTText, 
                    aSE.StoryProject.ProjSettings.InternationalBT, false, nNumColumns);
                nNumColumns++;
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public bool Focus(StoryEditor theSE)
        {
            Focus();
            if (theSE.viewVernacularLangFieldMenuItem.Checked)
            {
                System.Diagnostics.Debug.Assert((_aVerseData.VernacularText.TextBox != null) && _aVerseData.VernacularText.TextBox.Visible);
                _aVerseData.VernacularText.TextBox.Focus();
            }
            else if (theSE.viewNationalLangFieldMenuItem.Checked)
            {
                System.Diagnostics.Debug.Assert((_aVerseData.NationalBTText.TextBox != null) && _aVerseData.NationalBTText.TextBox.Visible);
                _aVerseData.NationalBTText.TextBox.Focus();
            }
            else if (theSE.viewEnglishBTFieldMenuItem.Checked)
            {
                System.Diagnostics.Debug.Assert((_aVerseData.InternationalBTText.TextBox != null) && _aVerseData.InternationalBTText.TextBox.Visible);
                _aVerseData.InternationalBTText.TextBox.Focus();
            }
            else
                return false;

            return true;
        }

        public void GetTextBoxValues(out string strVernacular, out string strNationalBT,
            out string strEnglishBT)
        {
            _aVerseData.VernacularText.ExtractSelectedText(out strVernacular);
            _aVerseData.NationalBTText.ExtractSelectedText(out strNationalBT);
            _aVerseData.InternationalBTText.ExtractSelectedText(out strEnglishBT);
        }

        protected void InitLabel(string strStoryLineLableName, int nLayoutColumn)
        {
            // add the row0 column label
            string strCtrlName = strStoryLineLableName + CstrSuffixLabel + nLayoutColumn.ToString();
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strCtrlName), "otherwise, fix wrong assumption");
            Label lbl = new Label
                            {
                                Name = strCtrlName,
                                Anchor = AnchorStyles.Top,
                                AutoSize = true,
                                Text = strStoryLineLableName
                            };
            tableLayoutPanel.Controls.Add(lbl, nLayoutColumn, 0);
        }

        protected void InitTextBox(VerseControl ctrlVerse, string strTbName, StringTransfer strTbText, 
            ProjectSettings.LanguageInfo li, bool bDisableTabStop, int nLayoutColumn)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + CstrSuffixTextBox), "otherwise, fix wrong assumption");
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, ctrlVerse, this, 
                strTbText, li, li.LangCode);
            tb.TabStop = !bDisableTabStop;
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
