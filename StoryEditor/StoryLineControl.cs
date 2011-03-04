// rde: removing lable row to save pixels
// #define ShowLabelRow

using System;
using System.Windows.Forms;
using ECInterfaces;
using SilEncConverters40;

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

#if ShowLabelRow
            InsertRow(1);
#endif

            // remove the columns, because we're going to add them back as equal sizes.
            while (tableLayoutPanel.ColumnCount > 0)
                RemoveColumn(tableLayoutPanel.ColumnCount - 1);

            int nNumColumns = 0;
            CtrlTextBox ctrlTextBoxVernacular = null;
            if (aSE.viewVernacularLangFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
#if ShowLabelRow
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.Vernacular.LangName, nNumColumns);
#endif

                // if we're in the one of the states where the user is entering in the 
                //  national or international BT, then disable the Vernacular as a tab stop.
                bool bDisableTabStopVernacular =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                _aVerseData.StoryLine.Vernacular.Transliterator = ctrlVerse.TransliteratorVernacular;
                ctrlTextBoxVernacular = InitTextBox(ctrlVerse, LineData.CstrAttributeLangVernacular, 
                    _aVerseData.StoryLine.Vernacular, 
                    aSE.StoryProject.ProjSettings.Vernacular, bDisableTabStopVernacular,
                    nNumColumns, StoryEditor.TextFieldType.eVernacular);
                nNumColumns++;
            }

            CtrlTextBox ctrlTextBoxNationalBT = null;
            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);

#if ShowLabelRow
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.NationalBT.LangName, nNumColumns);
#endif

                // if we're in the one of the states where the user is entering in the 
                //  international BT, then disable the National BT as a tab stop.
                bool bDisableTabStopNationalBT =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                _aVerseData.StoryLine.NationalBt.Transliterator = ctrlVerse.TransliteratorNationalBT;
                ctrlTextBoxNationalBT = InitTextBox(ctrlVerse, LineData.CstrAttributeLangNationalBt, _aVerseData.StoryLine.NationalBt, 
                    aSE.StoryProject.ProjSettings.NationalBT, bDisableTabStopNationalBT,
                    nNumColumns, StoryEditor.TextFieldType.eNational);

                nNumColumns++;

                if (ctrlTextBoxVernacular != null)
                    ctrlTextBoxVernacular.NationalBtSibling = ctrlTextBoxNationalBT;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);

#if ShowLabelRow
                if (ctrlVerse.VerseNumber == 1)
                    InitLabel(aSE.StoryProject.ProjSettings.InternationalBT.LangName, nNumColumns);
#endif

                // if we're in the one of the states where the user is entering in the 
                //  international BT, then disable the National BT as a tab stop.
                bool bDisableTabStopInternationalBT =
                    (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeFreeTranslation);

                CtrlTextBox ctrlTextBoxEnglishBT = InitTextBox(ctrlVerse, 
                    LineData.CstrAttributeLangInternationalBt, _aVerseData.StoryLine.InternationalBt,
                    aSE.StoryProject.ProjSettings.InternationalBT, bDisableTabStopInternationalBT, nNumColumns,
                    StoryEditor.TextFieldType.eInternational);
                nNumColumns++;

                if (ctrlTextBoxVernacular != null)
                    ctrlTextBoxVernacular.EnglishBtSibling = ctrlTextBoxEnglishBT;

                if (ctrlTextBoxNationalBT != null)
                    ctrlTextBoxNationalBT.EnglishBtSibling = ctrlTextBoxEnglishBT;
            }

            if (aSE.viewFreeTranslationToolStripMenuItem.Checked)
            {
                InsertColumn(nNumColumns);

                InitTextBox(ctrlVerse, 
                    LineData.CstrAttributeLangFreeTranslation, _aVerseData.StoryLine.FreeTranslation,
                    aSE.StoryProject.ProjSettings.FreeTranslation, false, nNumColumns,
                    StoryEditor.TextFieldType.eFreeTranslation);
                nNumColumns++;
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public new bool Focus()
        {
            CtrlTextBox tbx = _aVerseData.StoryLine.ExistingTextBox;
            if (tbx != null)
                tbx.Focus();
            else
                return false;

            return true;
        }

        public void GetTextBoxValues(out string strVernacular, out string strNationalBT,
            out string strEnglishBT, out string strFreeTranslation)
        {
            _aVerseData.StoryLine.ExtractSelectedText(out strVernacular,
                                                      out strNationalBT, 
                                                      out strEnglishBT, 
                                                      out strFreeTranslation);
        }

#if ShowLabelRow
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
#endif

        protected CtrlTextBox InitTextBox(VerseControl ctrlVerse, string strTbName, StringTransfer strTbText, 
            ProjectSettings.LanguageInfo li, bool bDisableTabStop, int nLayoutColumn,
            StoryEditor.TextFieldType eFieldType)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + CstrSuffixTextBox), "otherwise, fix wrong assumption");
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, ctrlVerse, this,
                strTbText, li, li.LangCode, eFieldType, li.FontColor);
            tb.TabStop = !bDisableTabStop;
#if ShowLabelRow
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
#else
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 0);
#endif
            return tb;
        }
    }
}
