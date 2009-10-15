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

        public StoryLineControl(StoryEditor aSE, VerseData aVerseData)
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
                InitLabel(aSE.StoryProject.ProjSettings.Vernacular.LangName, nNumColumns);

                // if we're in the one of the states where the user is entering in the 
                //  national or international BT, then disable the Vernacular as a tab stop.
                bool bDisableTabStopVernacular =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeNationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                InitTextBox(CstrFieldNameVernacular, _aVerseData.VernacularText, 
                    aSE.StoryProject.ProjSettings.Vernacular, aSE.LoggedOnMember.OverrideVernacularKeyboard,
                    bDisableTabStopVernacular, nNumColumns);
                nNumColumns++;
            }

            if (aSE.viewNationalLangFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
                InitLabel(aSE.StoryProject.ProjSettings.NationalBT.LangName, nNumColumns);

                // if we're in the one of the states where the user is entering in the 
                //  international BT, then disable the National BT as a tab stop.
                bool bDisableTabStopNationalBT =
                    ((aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eProjFacTypeInternationalBT)
                    || (aSE.theCurrentStory.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT));

                InitTextBox(CstrFieldNameNationalBt, _aVerseData.NationalBTText, 
                    aSE.StoryProject.ProjSettings.NationalBT, aSE.LoggedOnMember.OverrideNationalBTKeyboard,
                    bDisableTabStopNationalBT, nNumColumns);

                nNumColumns++;
            }

            if (aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                InsertColumn(nNumColumns);
                InitLabel(aSE.StoryProject.ProjSettings.InternationalBT.LangName, nNumColumns);

                InitTextBox(CstrFieldNameInternationalBt, _aVerseData.InternationalBTText, 
                    aSE.StoryProject.ProjSettings.InternationalBT, null, false, nNumColumns);
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

        protected void InitTextBox(string strTbName, StringTransfer strTbText, 
            ProjectSettings.LanguageInfo li, string strOverrideKeyboard, bool bDisableTabStop, int nLayoutColumn)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(strTbName + CstrSuffixTextBox), "otherwise, fix wrong assumption");
            CtrlTextBox tb = new CtrlTextBox(strTbName + CstrSuffixTextBox, this, strTbText, li, strOverrideKeyboard);
            tb.TabStop = !bDisableTabStop;
            tableLayoutPanel.Controls.Add(tb, nLayoutColumn, 1);
        }
    }
}
