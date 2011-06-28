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
        public DirectableEncConverter TransliteratorVernacular;
        public DirectableEncConverter TransliteratorNationalBT;

        public PrintForm(StoryEditor theSE)
        {
            _theSE = theSE;
            InitializeComponent();
            htmlStoryBt.TheSE = theSE;  // so it can do anchor jumps

            foreach (StoryData aStory in _theSE.TheCurrentStoriesSet)
                checkedListBoxStories.Items.Add(aStory.Name, true);

            if (_theSE.StoryProject.ProjSettings.Vernacular.HasData)
            {
                checkBoxLangVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                                            _theSE.StoryProject.ProjSettings.Vernacular.LangName);
                if (theSE.viewTransliterationVernacular.Checked
                   && !String.IsNullOrEmpty(theSE.LoggedOnMember.TransliteratorVernacular))
                {
                    checkBoxLangTransliterateVernacular.Visible =
                        checkBoxLangTransliterateVernacular.Checked = true;
                    TransliteratorVernacular = new DirectableEncConverter(theSE.LoggedOnMember.TransliteratorVernacular,
                                                                          theSE.LoggedOnMember.
                                                                              TransliteratorDirectionForwardVernacular,
                                                                          NormalizeFlags.None);
                }
            }
            else
                checkBoxLangVernacular.Checked = checkBoxLangVernacular.Visible = false;

            if (_theSE.StoryProject.ProjSettings.NationalBT.HasData)
            {
                checkBoxLangNationalBT.Text = String.Format(Properties.Resources.IDS_StoryLanguageField,
                                                            _theSE.StoryProject.ProjSettings.NationalBT.LangName);
                if (theSE.viewTransliterationNationalBT.Checked
                   && !String.IsNullOrEmpty(theSE.LoggedOnMember.TransliteratorNationalBT))
                {
                    checkBoxLangTransliterateNationalBT.Visible =
                        checkBoxLangTransliterateNationalBT.Checked = true;
                    TransliteratorNationalBT = new DirectableEncConverter(theSE.LoggedOnMember.TransliteratorNationalBT,
                                                                          theSE.LoggedOnMember.
                                                                              TransliteratorDirectionForwardNationalBT,
                                                                          NormalizeFlags.None);
                }
            }
            else
                checkBoxLangNationalBT.Checked = checkBoxLangNationalBT.Visible = false;

            checkBoxLangInternationalBT.Checked =
                checkBoxLangInternationalBT.Visible =
                _theSE.StoryProject.ProjSettings.InternationalBT.HasData;

            checkBoxLangFreeTranslation.Checked =
                checkBoxLangFreeTranslation.Visible =
                _theSE.StoryProject.ProjSettings.FreeTranslation.HasData;

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
                        ? TransliteratorVernacular
                        : null,
                    (checkBoxLangTransliterateNationalBT.Checked)
                        ? TransliteratorNationalBT
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
                           VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
            SetViewSetting(checkBoxLangInternationalBT, viewSettings,
                           VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);
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
                           VerseData.ViewSettings.ItemToInsureOn.NationalBTTransliterationField);
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
