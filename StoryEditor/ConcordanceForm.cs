using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConcordanceForm : Form
    {
        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;
        Dictionary<string, List<string>> _mapReferenceToVerseTextList;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html

        public ConcordanceForm(StoryEditor theSE, TextBoxBase ctxBox)
        {
            InitializeComponent();
            _theSE = theSE;
            _storyProject = theSE.StoryProject;

            if (_storyProject.ProjSettings.Vernacular.HasData)
            {
                radioButtonSearchVernacular.Visible = true;
                radioButtonSearchVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields, 
                                                                 _storyProject.ProjSettings.Vernacular.LangName);
            }
            else
                radioButtonSearchVernacular.Visible = false;

            if (_storyProject.ProjSettings.NationalBT.HasData)
            {
                radioButtonSearchNationalBT.Visible = true;
                radioButtonSearchNationalBT.Text = String.Format(Properties.Resources.IDS_StoryLanguageField,
                                                                 _storyProject.ProjSettings.NationalBT.LangName);
            }
            else
                radioButtonSearchNationalBT.Visible = false;
            
            // if we only *have* EnglishBT, then don't need to bother showing anything
            flowLayoutPanelLanguageChoice.Visible = 
                labelLanguageToSearchIn.Visible =
                radioButtonSearchInternationalBT.Visible = (_storyProject.ProjSettings.Vernacular.HasData
                || _storyProject.ProjSettings.NationalBT.HasData);

            ProjectSettings.LanguageInfo liToUse;
            if (ctxBox != null)
            {
                textBoxWordsToSearchFor.Text = ctxBox.SelectedText.Trim();

                // try to figure out which language we're using
                if ((liToUse = _storyProject.ProjSettings.Vernacular).HasData
                    && (ctxBox.Font == liToUse.FontToUse)
                    && (ctxBox.ForeColor == liToUse.FontColor))
                {
                    radioButtonSearchVernacular.Checked = true;
                }
                else if ((liToUse = _storyProject.ProjSettings.NationalBT).HasData
                    &&  (ctxBox.Font == liToUse.FontToUse)
                    && (ctxBox.ForeColor == liToUse.FontColor))
                {
                    radioButtonSearchNationalBT.Checked = true;
                }
                else
                {
                    radioButtonSearchInternationalBT.Checked = true;
                }
            }
            else
            {
                radioButtonSearchInternationalBT.Checked = true;
            }

            htmlBuilder = new BiblicalTermsHTMLBuilder(_storyProject.ProjSettings);
        }

        private void buttonBeginSearch_Click(object sender, EventArgs e)
        {
            VerseData.SearchLookInProperties FindProperties = new VerseData.SearchLookInProperties();
            if (radioButtonSearchInternationalBT.Checked)
                FindProperties.EnglishBT = true;
            else if (radioButtonSearchNationalBT.Checked)
                FindProperties.NationalBT = true;
            else if (radioButtonSearchVernacular.Checked)
                FindProperties.StoryLanguage = true;

            htmlBuilder.SearchVerseText(textBoxWordsToSearchFor.Text, _storyProject, 
                FindProperties, progressBarLoadingKeyTerms);

            BiblicalTermStatus dontcare;
            string strHtml = htmlBuilder.Build(_storyProject, progressBarLoadingKeyTerms, false, out dontcare);
            webBrowser.LoadDocument(strHtml);
            progressBarLoadingKeyTerms.Visible = false;
        }

        private void InitSearchBoxes(ProjectSettings.LanguageInfo li, string strToStartWith)
        {
            textBoxWordsToSearchFor.Font = li.FontToUse;
            textBoxWordsToSearchFor.ForeColor = li.FontColor;
            textBoxWordsToSearchFor.Text = strToStartWith;
        }

        private void radioButtonSearchVernacular_CheckedChanged(object sender, EventArgs e)
        {
            InitSearchBoxes(_storyProject.ProjSettings.Vernacular, textBoxWordsToSearchFor.Text);
        }

        private void radioButtonSearchNationalBT_CheckedChanged(object sender, EventArgs e)
        {
            InitSearchBoxes(_storyProject.ProjSettings.NationalBT, textBoxWordsToSearchFor.Text);
        }

        private void radioButtonSearchInternationalBT_CheckedChanged(object sender, EventArgs e)
        {
            InitSearchBoxes(_storyProject.ProjSettings.InternationalBT, textBoxWordsToSearchFor.Text);
        }

        private void webBrowser_BeforeNavigate(object s, onlyconnect.BeforeNavigateEventArgs e)
        {
            string link = e.Target;

            // Scroll the editor to this reference.
            if (link.StartsWith("userclick:scripref"))
            {
                string thisRef = link.Substring(19).Replace('_', ' ');
                string strStoryName, strAnchor;
                int nLineNumber;
                BiblicalTermsHTMLBuilder.ParseReference(thisRef, out strStoryName, out nLineNumber, out strAnchor);
                _theSE.NavigateTo(strStoryName, nLineNumber, strAnchor);
                e.Cancel = true;
            }
        }
    }
}
