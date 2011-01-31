using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConcordanceForm : Form
    {
        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html

        public string VernacularForm { get; set; }
        public string NationalForm { get; set; }
        public string InternationalForm { get; set; }
        public string FreeTranslationForm { get; set; }

        public ConcordanceForm(StoryEditor theSE, string strToSearchForVernacular, 
            string strToSearchForNationalBT, string strToSearchForInternationalBT,
            string strToSearchForFreeTranslation)
        {
            InitializeComponent();
            _theSE = theSE;
            _storyProject = theSE.StoryProject;
            VernacularForm = TrimIfNeeded(strToSearchForVernacular);
            NationalForm = TrimIfNeeded(strToSearchForNationalBT);
            InternationalForm = TrimIfNeeded(strToSearchForInternationalBT);
            FreeTranslationForm = TrimIfNeeded(strToSearchForFreeTranslation);

            InitSearchBoxes(_storyProject.ProjSettings.Vernacular, VernacularForm,
                labelVernacular, textBoxWordsToSearchForVernacular);
            InitSearchBoxes(_storyProject.ProjSettings.NationalBT, NationalForm,
                labelNationalBT, textBoxWordsToSearchForNationalBT);
            InitSearchBoxes(_storyProject.ProjSettings.InternationalBT, InternationalForm,
                labelInternationalBT, textBoxWordsToSearchForInternationalBT);
            InitSearchBoxes(_storyProject.ProjSettings.FreeTranslation, FreeTranslationForm,
                            labelFreeTranslation, textBoxWordsToSearchForFreeTranslation);

            if (labelFreeTranslation.Visible)
                labelFreeTranslation.Text = "Free Translation";

            htmlBuilder = new BiblicalTermsHTMLBuilder(_storyProject.ProjSettings);
            buttonBeginSearch_Click(null, null);
        }

        private void buttonBeginSearch_Click(object sender, EventArgs e)
        {
            htmlBuilder.SearchVerseText(_storyProject, progressBarLoadingKeyTerms, 
                _theSE.hiddenVersesToolStripMenuItem.Checked,   // if the user is *showing* hidden verses, then search in them
                textBoxWordsToSearchForVernacular.Text, 
                textBoxWordsToSearchForNationalBT.Text,
                textBoxWordsToSearchForInternationalBT.Text,
                textBoxWordsToSearchForFreeTranslation.Text);

            BiblicalTermStatus dontcare;
            string strHtml = htmlBuilder.Build(_storyProject, progressBarLoadingKeyTerms, false, out dontcare);
            webBrowser.LoadDocument(strHtml);
            progressBarLoadingKeyTerms.Visible = false;
        }

        private string TrimIfNeeded(string str)
        {
            return !String.IsNullOrEmpty(str) ? str.Trim() : null;
        }

        private int _nColumnIndex = 0;
        private void InitSearchBoxes(ProjectSettings.LanguageInfo li, string strToStartWith, Control lbl, Control tb)
        {
            _nColumnIndex++;
            tb.Text = strToStartWith;   // this wants to happen even if we're not going to show it
            if (li.HasData)
            {
                lbl.Visible = tb.Visible = true;
                lbl.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                         li.LangName);
                tb.Font = li.FontToUse;
                tb.ForeColor = li.FontColor;
            }
            else
            {
                // change it so that this column is invisible and the others fill the gaps
                lbl.Visible = tb.Visible = false;
                tableLayoutPanel.ColumnStyles[_nColumnIndex] = new ColumnStyle(SizeType.Absolute, 0);
            }
        }

        private void webBrowser_BeforeNavigate(object s, onlyconnect.BeforeNavigateEventArgs e)
        {
            string link = e.Target;

            // Scroll the editor to this reference.
            if (link.StartsWith("userclick:scripref"))
            {
                string thisRef = BiblicalTermsHTMLBuilder.DecodeAsHtmlId(link.Substring(19));
                string strStoryName, strAnchor;
                int nLineNumber;
                BiblicalTermsHTMLBuilder.ParseReference(thisRef, out strStoryName, out nLineNumber, out strAnchor);
                _theSE.NavigateTo(strStoryName, nLineNumber, strAnchor);
                e.Cancel = true;
            }
        }

        private void webBrowser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0003)    // reverse engineered
                webBrowser.Copy();
        }

        private void ConcordanceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save the current values in case a caller (e.g. L&C Notes) wants to update
            //  from what they ended on searching for
            VernacularForm = textBoxWordsToSearchForVernacular.Text;
            NationalForm = textBoxWordsToSearchForNationalBT.Text;
            InternationalForm = textBoxWordsToSearchForInternationalBT.Text;
            FreeTranslationForm = textBoxWordsToSearchForFreeTranslation.Text;
        }
    }
}
