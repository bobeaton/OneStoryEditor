using System;
using System.Windows.Forms;
using NetLoc;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public partial class AddLnCNoteForm : TopForm
    {
        public LnCNote TheLnCNote;

        readonly StoryProjectData _storyProject;
#if UnhookParatextBiblicalTerms
        BiblicalTermsList _btl;
#endif
        private AddLnCNoteForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public AddLnCNoteForm(StoryEditor theSE, LnCNote theLnC)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            TheLnCNote = theLnC;
            _storyProject = theSE.StoryProject;
#if UnhookParatextBiblicalTerms
            _btl = BiblicalTermsList.GetBiblicalTerms(_storyProject.ProjSettings.ProjectFolder);
#endif
            if (theLnC == null)
                return;

            InitSearchBoxes(true, _storyProject.ProjSettings.Vernacular, theLnC.VernacularRendering,
                labelVernacular, textBoxVernacular);
            InitSearchBoxes(false, _storyProject.ProjSettings.NationalBT, theLnC.NationalBtRendering,
                labelNationalBT, textBoxNationalBT);
            InitSearchBoxes(true, _storyProject.ProjSettings.InternationalBT, theLnC.InternationalBtRendering,
                labelInternationalBT, textBoxInternationalBT);

            textBoxNotes.Text = theLnC.Notes;

            var strHelpString = Localizer.Str("Enter the word(s) you want to search for. You can search for more than one word at a time by separating them by commas. For all words beginning with \"xyz\" use \"xyz*\" ; ending with \"xyz\" use \"*xyz\"; containing \"xyz\" use \"*xyz*\". Multiple word search phrases are allowed, for example \"John the Baptist\" (enclose in double-quotes)");
            var strTooltip = Localizer.Str("Enter the different spellings of this word separated by commas. Press F1 for further instructions on how to enter data in this field.");

            SetHelpStrings(textBoxVernacular, strHelpString, strTooltip);
            SetHelpStrings(textBoxNationalBT, strHelpString, strTooltip);
            SetHelpStrings(textBoxInternationalBT, strHelpString, strTooltip);

#if UnhookParatextBiblicalTerms
            textBoxKeyTerms.Text = theLnC.GetKeyTermsGlosses(_btl);
#endif
        }

        public AddLnCNoteForm(StoryEditor theSE, string strToSearchForVernacular, 
            string strToSearchForNationalBT, string strToSearchForInternationalBT)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            _storyProject = theSE.StoryProject;
            TheLnCNote = new LnCNote();

            InitSearchBoxes(true, _storyProject.ProjSettings.Vernacular, strToSearchForVernacular,
                labelVernacular, textBoxVernacular);
            InitSearchBoxes(false, _storyProject.ProjSettings.NationalBT, strToSearchForNationalBT,
                labelNationalBT, textBoxNationalBT);
            InitSearchBoxes(true, _storyProject.ProjSettings.InternationalBT, strToSearchForInternationalBT,
                labelInternationalBT, textBoxInternationalBT);
        }

        private void SetHelpStrings(Control ctrl, string strHelpString, string strTooltip)
        {
            helpProvider.SetHelpString(ctrl, strHelpString);
            toolTip.SetToolTip(ctrl, strTooltip);
        }

        private int _nRowIndex = 0;
        private void InitSearchBoxes(bool bDoForSure, ProjectSettings.LanguageInfo li, 
            string strToStartWith, Control lbl, Control tb)
        {
            if (bDoForSure || li.HasData)
            {
                lbl.Visible = tb.Visible = true;
                tb.Font = li.FontToUse;
                tb.ForeColor = li.FontColor;
                tb.Text = !String.IsNullOrEmpty(strToStartWith) ? strToStartWith.Trim() : null;
                if (li.HasData)
                {
                    lbl.Text = li.LangName;
                    tb.Tag = li;
                }
            }
            else
            {
                // change it so that this column is invisible and the others fill the gaps
                lbl.Visible = tb.Visible = false;
                tableLayoutPanel.RowStyles[_nRowIndex] = new RowStyle(SizeType.Absolute, 0);
            }
            _nRowIndex++;            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxInternationalBT.Text))
            {
                LocalizableMessageBox.Show(Localizer.Str("You must enter the gloss"),
                                StoryEditor.OseCaption);
                return;
            }

            if (String.IsNullOrEmpty(textBoxVernacular.Text))
            {
                LocalizableMessageBox.Show(Localizer.Str("You must enter the word in the story language"),
                                StoryEditor.OseCaption);
                return;
            }

            TheLnCNote.VernacularRendering = textBoxVernacular.Text;
            TheLnCNote.NationalBtRendering = textBoxNationalBT.Text;
            TheLnCNote.InternationalBtRendering = textBoxInternationalBT.Text;
            TheLnCNote.Notes = textBoxNotes.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null)
                return;
            var li = tb.Tag as ProjectSettings.LanguageInfo;
            if ((li != null) && (!String.IsNullOrEmpty(li.Keyboard)))
                KeyboardController.ActivateKeyboard(li.Keyboard);
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            KeyboardController.DeactivateKeyboard();
        }
        /*
        private void buttonKeyTermSelect_Click(object sender, EventArgs e)
        {
            if (_btl == null)
                _btl = BiblicalTermsList.GetBiblicalTerms(_storyProject.ProjSettings.ProjectFolder);

            var dlg = new SelectKeyTerms(_btl, TheLnCNote.GetKeyTerms(_btl));
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TheLnCNote.SetKeyTerms(dlg.SelectedTerms);
                textBoxKeyTerms.Text = TheLnCNote.GetKeyTermsGlosses(_btl);
            }
        }
        */
    }
}
