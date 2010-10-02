using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConcordanceForm : Form
    {
        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html

        public ConcordanceForm(StoryEditor theSE, 
            string strToSearchForVernacular, string strToSearchForNationalBT, string strToSearchForInternationalBT)
        {
            InitializeComponent();
            _theSE = theSE;
            _storyProject = theSE.StoryProject;

            InitSearchBoxes(_storyProject.ProjSettings.Vernacular, strToSearchForVernacular, 
                labelVernacular, textBoxWordsToSearchForVernacular);
            InitSearchBoxes(_storyProject.ProjSettings.NationalBT, strToSearchForNationalBT,
                labelNationalBT, textBoxWordsToSearchForNationalBT);
            InitSearchBoxes(_storyProject.ProjSettings.InternationalBT, strToSearchForInternationalBT,
                labelInternationalBT, textBoxWordsToSearchForInternationalBT);

            htmlBuilder = new BiblicalTermsHTMLBuilder(_storyProject.ProjSettings);
        }

        private void buttonBeginSearch_Click(object sender, EventArgs e)
        {
            htmlBuilder.SearchVerseText(_storyProject, progressBarLoadingKeyTerms, 
                _theSE.hiddenVersesToolStripMenuItem.Checked,   // if the user is *showing* hidden verses, then search in them
                textBoxWordsToSearchForVernacular.Text, 
                textBoxWordsToSearchForNationalBT.Text,
                textBoxWordsToSearchForInternationalBT.Text);

            BiblicalTermStatus dontcare;
            string strHtml = htmlBuilder.Build(_storyProject, progressBarLoadingKeyTerms, false, out dontcare);
            webBrowser.LoadDocument(strHtml);
            progressBarLoadingKeyTerms.Visible = false;
        }

        private int _nColumnIndex = 0;
        private void InitSearchBoxes(ProjectSettings.LanguageInfo li, string strToStartWith, Control lbl, Control tb)
        {
            _nColumnIndex++;
            if (li.HasData)
            {
                lbl.Visible = tb.Visible = true;
                lbl.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                         li.LangName);
                tb.Font = li.FontToUse;
                tb.ForeColor = li.FontColor;
                tb.Text = !String.IsNullOrEmpty(strToStartWith) ? strToStartWith.Trim() : null;
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
    }
}
