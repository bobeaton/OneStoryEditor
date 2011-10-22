using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class ConcordanceForm : TopForm
    {
        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html

        public string VernacularForm { get; set; }
        public string NationalForm { get; set; }
        public string InternationalForm { get; set; }
        public string FreeTranslationForm { get; set; }

        private ConcordanceForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public ConcordanceForm(StoryEditor theSE, string strToSearchForVernacular, 
            string strToSearchForNationalBT, string strToSearchForInternationalBT,
            string strToSearchForFreeTranslation)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

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
                labelFreeTranslation.Text = Localizer.Str("Free Translation");

            var strHelpString = Localizer.Str("Enter the word(s) you want to search for. You can search for more than one word at a time by separating them by commas. For all words beginning with \"xyz\" use \"xyz*\" ; ending with \"xyz\" use \"*xyz\"; containing \"xyz\" use \"*xyz*\". Multiple word search phrases are allowed, for example \"John the Baptist\" (enclose in double-quotes)");
            var strTooltip = Localizer.Str("Enter the word(s) to search. Press F1 for further instructions on how to enter data in this field.");

            SetHelpStrings(textBoxWordsToSearchForVernacular, strHelpString, strTooltip);
            SetHelpStrings(textBoxWordsToSearchForNationalBT, strHelpString, strTooltip);
            SetHelpStrings(textBoxWordsToSearchForInternationalBT, strHelpString, strTooltip);
            SetHelpStrings(textBoxWordsToSearchForFreeTranslation, strHelpString, strTooltip);

            htmlBuilder = new BiblicalTermsHTMLBuilder(_storyProject.ProjSettings);
            buttonBeginSearch_Click(null, null);
        }

        private void SetHelpStrings(Control ctrl, string strHelpString, string strTooltip)
        {
            helpProvider.SetHelpString(ctrl, strHelpString);
            toolTip.SetToolTip(ctrl, strTooltip);
        }

        private void buttonBeginSearch_Click(object sender, EventArgs e)
        {
            htmlBuilder.SearchVerseText(_storyProject,
                                        progressBarLoadingKeyTerms,
                                        _theSE.viewHiddenVersesMenu.Checked,
                                        // if the user is *showing* hidden verses, then search in them
                                        textBoxWordsToSearchForVernacular.Text,
                                        _theSE.viewTransliterationVernacular.Checked
                                            ? _theSE.LoggedOnMember.TransliteratorVernacular
                                            : null,
                                        textBoxWordsToSearchForNationalBT.Text,
                                        _theSE.viewTransliterationNationalBT.Checked
                                            ? _theSE.LoggedOnMember.TransliteratorNationalBt
                                            : null,
                                        textBoxWordsToSearchForInternationalBT.Text,
                                        _theSE.viewTransliterationInternationalBt.Checked
                                            ? _theSE.LoggedOnMember.TransliteratorInternationalBt
                                            : null,
                                        textBoxWordsToSearchForFreeTranslation.Text,
                                        _theSE.viewTransliterationFreeTranslation.Checked
                                            ? _theSE.LoggedOnMember.TransliteratorFreeTranslation
                                            : null);

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
                lbl.Text = String.Format(Localizer.Str("{0} &language fields"),
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
                string strStorySet, strStoryName, strAnchor;
                int nLineNumber;
                BiblicalTermsHTMLBuilder.ParseReference(thisRef, out strStorySet, out strStoryName, out nLineNumber, out strAnchor);
                _theSE.NavigateTo(strStorySet, strStoryName, nLineNumber, strAnchor);
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
