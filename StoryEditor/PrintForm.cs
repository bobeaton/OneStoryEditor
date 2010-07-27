using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class PrintForm : Form
    {
        private StoryEditor _theSE;

        public PrintForm(StoryEditor theSE)
        {
            InitializeComponent();
            _theSE = theSE;

            foreach (StoryData aStory in _theSE.TheCurrentStoriesSet)
                checkedListBoxStories.Items.Add(aStory.Name, true);

            if (_theSE.StoryProject.ProjSettings.Vernacular.HasData)
            {
                checkBoxLangVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                                            _theSE.StoryProject.ProjSettings.Vernacular.LangName);
            }
            else
                checkBoxLangVernacular.Checked = checkBoxLangVernacular.Visible = false;

            if (_theSE.StoryProject.ProjSettings.NationalBT.HasData)
            {
                checkBoxLangNationalBT.Text = String.Format(Properties.Resources.IDS_StoryLanguageField,
                                                            _theSE.StoryProject.ProjSettings.NationalBT.LangName);
            }
            else
                checkBoxLangNationalBT.Checked = checkBoxLangNationalBT.Visible = false;

            checkBoxLangInternationalBT.Checked =
                checkBoxLangInternationalBT.Visible =
                _theSE.StoryProject.ProjSettings.InternationalBT.HasData;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPagePrintPreview)
            {
                VerseData.ViewItemToInsureOn viewSettings = VerseData.SetItemsToInsureOn(
                    checkBoxLangVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxAnswers.Checked,
                    checkBoxRetellings.Checked,
                    false,  // _theSE.viewConsultantNoteFieldMenuItem.Checked,
                    false,  // _theSE.viewCoachNotesFieldMenuItem.Checked,
                    false,  // _theSE.viewNetBibleMenuItem.Checked
                    checkBoxFrontMatter.Checked);

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
    }
}
