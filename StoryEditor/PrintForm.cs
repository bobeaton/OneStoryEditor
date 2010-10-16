using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ECInterfaces;
using SilEncConverters31;

namespace OneStoryProjectEditor
{
    public partial class PrintForm : Form
    {
        private StoryEditor _theSE;
        public DirectableEncConverter TransliteratorVernacular;
        public DirectableEncConverter TransliteratorNationalBT;

        public PrintForm(StoryEditor theSE)
        {
            InitializeComponent();
            _theSE = theSE;
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

            checkBoxShowHidden.Checked = _theSE.hiddenVersesToolStripMenuItem.Checked;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPagePrintPreview)
            {
                VerseData.ViewSettings viewSettings = new VerseData.ViewSettings(
                    checkBoxLangVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    false, // _theSE.viewConsultantNoteFieldMenuItem.Checked,
                    false, // _theSE.viewCoachNotesFieldMenuItem.Checked,
                    false, // _theSE.viewNetBibleMenuItem.Checked
                    checkBoxFrontMatter.Checked,
                    checkBoxShowHidden.Checked,
                    (checkBoxLangTransliterateVernacular.Checked)
                        ? TransliteratorVernacular
                        : null,
                    (checkBoxLangTransliterateNationalBT.Checked)
                        ? TransliteratorNationalBT
                        : null);

                string strHtml = null;
                foreach (var aCheckedStoryName in checkedListBoxStories.CheckedItems)
                {
                    StoryData aStory = _theSE.TheCurrentStoriesSet.GetStoryFromName(aCheckedStoryName.ToString());
                    if (aStory != null)
                        strHtml += aStory.PresentationHtmlWithoutHtmlDocOutside(viewSettings, _theSE.StoryProject.ProjSettings,
                            _theSE.StoryProject.TeamMembers, null);
                }

                htmlStoryBt.DocumentText = StoryData.AddHtmlHtmlDocOutside(strHtml, _theSE.StoryProject.ProjSettings);
            }
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

            checkBoxLangVernacular.Checked = 
                checkBoxLangNationalBT.Checked = 
                checkBoxLangInternationalBT.Checked = 
                checkBoxAnchors.Checked = 
                checkBoxStoryTestingQuestions.Checked = 
                checkBoxAnswers.Checked =
                checkBoxRetellings.Checked =
                checkBoxFrontMatter.Checked = (cb.CheckState == CheckState.Checked);

            checkBoxSelectAllFields.Text = (cb.CheckState == CheckState.Checked) 
                ? "&Deselect All" 
                : "&Select All";
        }
    }
}
