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
    public partial class CorrectSourceWordForm : Form
    {
        public CorrectSourceWordForm()
        {
            InitializeComponent();
        }

        public string CorrectedWord
        {
            get { return textBoxCorrectedWord.Text; }
            set { textBoxCorrectedWord.Text = value; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
