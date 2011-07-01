using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ECInterfaces;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class PrintForm : Form
    {
        private StoryEditor _theSE;

        public PrintForm(StoryEditor theSE)
        {
            _theSE = theSE;
            InitializeComponent();
            htmlStoryBt.TheSE = theSE;  // so it can do anchor jumps

            foreach (StoryData aStory in _theSE.TheCurrentStoriesSet)
                checkedListBoxStories.Items.Add(aStory.Name, true);

            Program.InitializeLangCheckBoxes(theSE.StoryProject.ProjSettings.Vernacular,
                                             checkBoxLangVernacular,
                                             checkBoxLangTransliterateVernacular,
                                             theSE.viewTransliterationVernacular,
                                             theSE.LoggedOnMember.TransliteratorVernacular);


            Program.InitializeLangCheckBoxes(_theSE.StoryProject.ProjSettings.NationalBT,
                                             checkBoxLangNationalBT,
                                             checkBoxLangTransliterateNationalBT,
                                             theSE.viewTransliterationNationalBT,
                                             theSE.LoggedOnMember.TransliteratorNationalBt);

            Program.InitializeLangCheckBoxes(_theSE.StoryProject.ProjSettings.InternationalBT,
                                             checkBoxLangInternationalBT,
                                             checkBoxLangTransliterateInternationalBt,
                                             theSE.viewTransliterationInternationalBt,
                                             theSE.LoggedOnMember.TransliteratorInternationalBt);

            Program.InitializeLangCheckBoxes(_theSE.StoryProject.ProjSettings.FreeTranslation,
                                             checkBoxLangFreeTranslation,
                                             checkBoxLangTransliterateFreeTranslation,
                                             theSE.viewTransliterationFreeTranslation,
                                             theSE.LoggedOnMember.TransliteratorFreeTranslation);

            checkBoxShowHidden.Checked = _theSE.hiddenVersesToolStripMenuItem.Checked;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage != tabPagePrintPreview)
                return;

            string strHtml = null;
            foreach (var aCheckedStoryName in checkedListBoxStories.CheckedItems)
            {
                StoryData aStory = _theSE.TheCurrentStoriesSet.GetStoryFromName(aCheckedStoryName.ToString());
                if (aStory != null)
                    strHtml += aStory.PresentationHtmlWithoutHtmlDocOutside(ViewSettings, _theSE.StoryProject.ProjSettings,
                                                                            _theSE.StoryProject.TeamMembers, null);
            }

            htmlStoryBt.DocumentText = StoryData.AddHtmlHtmlDocOutside(strHtml, _theSE.StoryProject.ProjSettings);
        }

        private VerseData.ViewSettings ViewSettings
        {
            get
            {
                if ((_theSE == null)
                    || (_theSE.StoryProject == null)
                    || (_theSE.StoryProject.ProjSettings == null))
                    return null;

                var viewSettings = new VerseData.ViewSettings(
                    _theSE.StoryProject.ProjSettings,
                    checkBoxLangVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxLangFreeTranslation.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxExegeticalHelpNote.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    false, // _theSE.viewConsultantNoteFieldMenuItem.Checked,
                    false, // _theSE.viewCoachNotesFieldMenuItem.Checked,
                    false, // _theSE.viewNetBibleMenuItem.Checked
                    checkBoxFrontMatter.Checked,
                    checkBoxShowHidden.Checked,
                    false, // show only open conversations (doesn't apply)
                    checkBoxGeneralTestingQuestions.Checked,
                    (checkBoxLangTransliterateVernacular.Checked)
                        ? _theSE.LoggedOnMember.TransliteratorVernacular
                        : null,
                    (checkBoxLangTransliterateNationalBT.Checked)
                        ? _theSE.LoggedOnMember.TransliteratorNationalBt
                        : null,
                    (checkBoxLangTransliterateInternationalBt.Checked)
                        ? _theSE.LoggedOnMember.TransliteratorInternationalBt
                        : null,
                    (checkBoxLangTransliterateFreeTranslation.Checked)
                        ? _theSE.LoggedOnMember.TransliteratorFreeTranslation
                        : null);
                return viewSettings;
            }
        }

        private void SetViewSettings(long lSettings)
        {
            var viewSettings = new VerseData.ViewSettings(lSettings);
            SetViewSetting(checkBoxLangVernacular, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            SetViewSetting(checkBoxLangNationalBT, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField);
            SetViewSetting(checkBoxLangInternationalBT, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.InternationalBtField);
            SetViewSetting(checkBoxLangFreeTranslation, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField);
            SetViewSetting(checkBoxAnchors, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.AnchorFields);
            SetViewSetting(checkBoxExegeticalHelpNote, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps);
            SetViewSetting(checkBoxGeneralTestingQuestions, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.GeneralTestQuestions);
            SetViewSetting(checkBoxStoryTestingQuestions, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions);
            SetViewSetting(checkBoxAnswers, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers);
            SetViewSetting(checkBoxRetellings, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.RetellingFields);
            SetViewSetting(checkBoxFrontMatter, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.StoryFrontMatter);
            SetViewSetting(checkBoxShowHidden, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.HiddenStuff);
            SetViewSetting(checkBoxLangTransliterateVernacular, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.VernacularTransliterationField);
            SetViewSetting(checkBoxLangTransliterateNationalBT, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.NationalBtTransliterationField);
            SetViewSetting(checkBoxLangTransliterateInternationalBt, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.InternationalBtTransliterationField);
            SetViewSetting(checkBoxLangTransliterateFreeTranslation, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.FreeTranslationTransliterationField);
        }

        // set the checkbox state *if* the checkbox is even visible
        private static void SetViewSetting(CheckBox cb, VerseData.ViewSettings viewSettings,
            VerseData.ViewSettings.ItemToInsureOn eItem)
        {
            if (cb.Visible)
                cb.Checked = viewSettings.IsViewItemOn(eItem);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            htmlStoryBt.ShowPrintPreviewDialog();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBoxSelectAll_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;

            for (int i = 0; i < checkedListBoxStories.Items.Count; i++)
            {
                checkedListBoxStories.SetItemCheckState(i, cb.CheckState);
            }

            checkBoxSelectAll.Text = (cb.CheckState == CheckState.Checked)
                ? "&Deselect All" 
                : "&Select All";
        }

        private void checkBoxSelectAllFields_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;

            if (cb.CheckState == CheckState.Indeterminate)
            {
                SetViewSettings(Properties.Settings.Default.LastPrintSettings);
                checkBoxSelectAllFields.Text = "&Last Settings";
            }
            else
            {
                bool bIsChecked = (cb.CheckState == CheckState.Checked);
                checkBoxLangVernacular.Checked =
                    checkBoxLangNationalBT.Checked =
                    checkBoxLangInternationalBT.Checked =
                    checkBoxAnchors.Checked =
                    checkBoxExegeticalHelpNote.Checked = 
                    checkBoxGeneralTestingQuestions.Checked = 
                    checkBoxStoryTestingQuestions.Checked =
                    checkBoxAnswers.Checked =
                    checkBoxRetellings.Checked =
                    checkBoxFrontMatter.Checked = bIsChecked;

                checkBoxSelectAllFields.Text = (bIsChecked)
                    ? "&Deselect All"
                    : "&Select All";
            }
        }

        private void PrintForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var vd = ViewSettings;
            if (vd == null)
                return;

            Properties.Settings.Default.LastPrintSettings = ViewSettings.LongValue;
            Properties.Settings.Default.Save();
        }

        private void buttonSaveHtml_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = _theSE.StoryProject.ProjSettings.ProjectName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string strDocumentText = htmlStoryBt.DocumentText;
                File.WriteAllText(saveFileDialog.FileName, strDocumentText, Encoding.UTF8);
            }
        }
    }
}
