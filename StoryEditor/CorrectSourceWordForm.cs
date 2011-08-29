using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class CorrectSourceWordForm : Form
    {
        public CorrectSourceWordForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public string CorrectedWord
        {
            get { return textBoxCorrectedWord.Text.Trim(); }
            set { textBoxCorrectedWord.Text = value.Trim(); }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxCorrectedWord_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = !String.IsNullOrEmpty(CorrectedWord);
        }
    }
}
