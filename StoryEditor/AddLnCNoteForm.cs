using System;
using System.Drawing;
using System.Windows.Forms;
using NetLoc;
using SIL.Keyboarding;

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

        /// <summary>
        /// For use when editing the L&C note
        /// </summary>
        /// <param name="theSE"></param>
        /// <param name="theLnC"></param>
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

            CtorCommon(theLnC);
        }

        /// <summary>
        /// For use when adding a new one based on selected strings
        /// </summary>
        /// <param name="theSE"></param>
        /// <param name="strToSearchForVernacular"></param>
        /// <param name="strToSearchForNationalBT"></param>
        /// <param name="strToSearchForInternationalBT"></param>
        public AddLnCNoteForm(StoryEditor theSE, string strToSearchForVernacular,
            string strToSearchForNationalBT, string strToSearchForInternationalBT)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            _storyProject = theSE.StoryProject;
            TheLnCNote = new LnCNote
            {
                VernacularRendering = strToSearchForVernacular,
                NationalBtRendering = strToSearchForNationalBT,
                InternationalBtRendering = strToSearchForInternationalBT,
            };

            CtorCommon(TheLnCNote);
        }

        private TextBox GlossField { get; set; }

        private void CtorCommon(LnCNote theLnC)
        {
            InitSearchBoxes(true,
                            _storyProject.ProjSettings.Vernacular,
                            theLnC.VernacularRendering,
                            labelVernacular,
                            textBoxVernacular);

            // the problem we have is that the L&C Notes (grid) window shows
            //  two columns for a) the Story language rendering and b) the gloss, 
            //  so it's good for us to require both. But for the latter, which 
            //  field do we use? If there's no EnglishBT field (cf. the 
            //  Indonesian situation), then...
            //  Here's what I'm thinking (where 'X' indicates the language is
            //  configured in the project and '-' means it's not):
            //
            //      Scenario:   1   2       3       4       5       6
            //  Story           X   X       X       [c]     [c]     [c]
            //  NationalBt      -   X[b]    X/-     X[b]    X       
            //  InternationalBt [a] -       X[b]            X[b]    X[b]
            //
            // where: 
            //  'a' indicates that even though the project doesn't use IBT, it
            //      will be used for the 'meaning' field
            //  'b' the lowest BT language (i.e. NBT > EBT) will serve for the gloss field
            //  'c' even though the project doesn't use Story language, we still 
            //      need it to represent the rendering of the L&C term
            bool bMeaningFieldRequired = false;
            if (!_storyProject.ProjSettings.NationalBT.HasData)
            {
                if (!_storyProject.ProjSettings.InternationalBT.HasData)
                {
                    // just a story field, so now *require* the International field
                    //  as the meaning/gloss field
                    bMeaningFieldRequired = true;
                    labelInternationalBT.Text = Localizer.Str("Gloss");
                }
                GlossField = textBoxInternationalBT;
            }
            else // is a NationalBt
            {
                GlossField = (_storyProject.ProjSettings.InternationalBT.HasData)
                                 ? textBoxInternationalBT
                                 : textBoxNationalBT;
            }

            // so now GlossField tells us which field must be non-null (for the 
            //  'Gloss' field in L&C Notes tab) and bMeaningFieldRequired tells
            //  us whether scenario 1 is applying in which we need to force
            //  the IBT field to show
            InitSearchBoxes(false,
                            _storyProject.ProjSettings.NationalBT,
                            theLnC.NationalBtRendering,
                            labelNationalBT,
                            textBoxNationalBT);

            // we *have* to have a 2nd field for the 'gloss' column in the 
            //  L&C notes grid window, so if there's no National or International
            //  BT fields, then use this field for the 'gloss'
            InitSearchBoxes(bMeaningFieldRequired,
                            _storyProject.ProjSettings.InternationalBT,
                            theLnC.InternationalBtRendering,
                            labelInternationalBT,
                            textBoxInternationalBT);

            var lnCNotesNoteFontName = Properties.Settings.Default.LnCNotesNoteFontName;
            var lnCNotesNoteFontSize = Properties.Settings.Default.LnCNotesNoteFontSize;
            textBoxNotes.Font = new Font(lnCNotesNoteFontName,
                                         lnCNotesNoteFontSize);
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

        private void SetHelpStrings(Control ctrl, string strHelpString, 
            string strTooltip)
        {
            helpProvider.SetHelpString(ctrl, strHelpString);
            toolTip.SetToolTip(ctrl, strTooltip);
        }

        private int _nRowIndex;
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
            if (String.IsNullOrEmpty(GlossField.Text))
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
                // Keyboard.Controller.SetKeyboard(li.Keyboard);
                Keyboard.Controller.GetKeyboard(li.Keyboard).Activate();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            Program.ActivateDefaultKeyboard();
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
