using System;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public partial class AddLnCNoteForm : Form
    {
        public LnCNote TheLnCNote;

        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;

        public AddLnCNoteForm(StoryEditor theSE, LnCNote theLnC)
        {
            InitializeComponent();
            TheLnCNote = theLnC;
            _theSE = theSE;
            _storyProject = theSE.StoryProject;

            if (theLnC == null)
                return;

            InitSearchBoxes(true, _storyProject.ProjSettings.Vernacular, theLnC.VernacularRendering,
                labelVernacular, textBoxVernacular);
            InitSearchBoxes(false, _storyProject.ProjSettings.NationalBT, theLnC.NationalBtRendering,
                labelNationalBT, textBoxNationalBT);
            InitSearchBoxes(true, _storyProject.ProjSettings.InternationalBT, theLnC.InternationalBtRendering,
                labelInternationalBT, textBoxInternationalBT);

            textBoxNotes.Text = theLnC.Notes;
        }

        public AddLnCNoteForm(StoryEditor theSE, string strToSearchForVernacular, 
            string strToSearchForNationalBT, string strToSearchForInternationalBT)
        {
            InitializeComponent();

            _theSE = theSE;
            _storyProject = theSE.StoryProject;

            InitSearchBoxes(true, _storyProject.ProjSettings.Vernacular, strToSearchForVernacular,
                labelVernacular, textBoxVernacular);
            InitSearchBoxes(false, _storyProject.ProjSettings.NationalBT, strToSearchForNationalBT,
                labelNationalBT, textBoxNationalBT);
            InitSearchBoxes(true, _storyProject.ProjSettings.InternationalBT, strToSearchForInternationalBT,
                labelInternationalBT, textBoxInternationalBT);
        }

        private int _nColumnIndex = 0;
        private void InitSearchBoxes(bool bDoForSure, ProjectSettings.LanguageInfo li, 
            string strToStartWith, Control lbl, Control tb)
        {
            _nColumnIndex++;
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
                tableLayoutPanel.ColumnStyles[_nColumnIndex] = new ColumnStyle(SizeType.Absolute, 0);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxInternationalBT.Text))
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_MustHave,
                                              "gloss"),
                                OseResources.Properties.Resources.IDS_Caption);
                return;
            }

            if (String.IsNullOrEmpty(textBoxVernacular.Text))
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_MustHave,
                                              "word in the story language"),
                                OseResources.Properties.Resources.IDS_Caption);
                return;
            }

            if (TheLnCNote == null)
                TheLnCNote = new LnCNote();

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
    }
}
