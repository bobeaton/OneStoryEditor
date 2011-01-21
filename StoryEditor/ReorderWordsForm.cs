using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ReorderWordsForm : Form
    {
        public ReorderWordsForm(IEnumerable<string> astrWords, char[] achPunctuation, Font font)
        {
            InitializeComponent();
            foreach (string strWord in astrWords)
            {
                var btnWord = new Button
                                  {
                                      Text = strWord,
                                      Font = font,
                                      AutoSize = true
                                  };
                btnWord.Click += BtnWordClick;
                btnWord.PreviewKeyDown += BtnPreviewKeyDown;
                flowLayoutPanelWords.Controls.Add(btnWord);
            }

            foreach (char c in achPunctuation)
            {
                if (VersesData.CstrWhiteSpace.IndexOf(c) != -1)
                    continue;

                var btnPunctuation = new Button
                                         {
                                             Text = c.ToString(),
                                             Font = font,
                                             AutoSize = true
                                         };
                btnPunctuation.Click += BtnPunctuationClick;
                btnPunctuation.PreviewKeyDown += BtnPreviewKeyDown;
                flowLayoutPanelPunctuation.Controls.Add(btnPunctuation);
            }
        }

        void BtnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
                buttonUndoLast_Click(null, null);
        }

        private void BtnPunctuationClick(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null)
                textBoxReorderedText.Text += btnWord.Text + ' ';
        }

        protected void BtnWordClick(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null) 
                textBoxReorderedText.Text += ' ' + btnWord.Text;
        }

        public string ReorderedText { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ReorderedText = textBoxReorderedText.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonUndoLast_Click(object sender, EventArgs e)
        {
            string strReorderedText = textBoxReorderedText.Text;
            int nIndex = strReorderedText.LastIndexOf(' ');
            if (nIndex != -1)
                textBoxReorderedText.Text = strReorderedText.Substring(0, nIndex);
        }
    }
}
