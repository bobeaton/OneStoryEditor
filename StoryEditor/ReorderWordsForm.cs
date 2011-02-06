using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class ReorderWordsForm : Form
    {
        [DllImport("TECkit_Compiler_x86", SetLastError = true)]
        static extern unsafe byte* TECkit_GetTECkitName(UInt32 usv);

        internal static unsafe string GetUnicodeName(char ch)
        {
            UInt32 usv = ch;
            byte* pszUnicodeName = TECkit_GetTECkitName(usv);
            byte[] baUnicodeName = ECNormalizeData.ByteStarToByteArr(pszUnicodeName);
            string strUnicodeName = Encoding.ASCII.GetString(baUnicodeName);
            return strUnicodeName;
        }

        private const string CstrSpace = "<Space>";
        private Font _font;
        public ReorderWordsForm(CtrlTextBox ctrlTextBox)
        {
            InitializeComponent();
            _font = ctrlTextBox.Font;
            char[] achToIgnore = VersesData.GetSplitChars(ctrlTextBox._strFullStop);
            string[] astrWords = ctrlTextBox.MyStringTransfer.GetWords(achToIgnore);
                        
            foreach (string strWord in astrWords)
            {
                var btnWord = new Button
                                  {
                                      Text = strWord,
                                      Font = _font,
                                      AutoSize = true
                                  };
                btnWord.Click += BtnWordClick;
                btnWord.PreviewKeyDown += BtnPreviewKeyDown;
                flowLayoutPanelWords.Controls.Add(btnWord);
            }

            foreach (char c in Properties.Settings.Default.LeftEdgeQuotes)
            {
                Button btn = AddPunctuationButton(c.ToString(), BtnPunctuationClickLeftEdge);
                toolTip.SetToolTip(btn, GetUnicodeName(c));
            }

            string strRightEdgePunctuation = ctrlTextBox._strFullStop + Properties.Settings.Default.RightEdgeQuotes;
            foreach (char c in strRightEdgePunctuation)
            {
                Button btn = AddPunctuationButton(c.ToString(), BtnPunctuationClickRightEdge);
                toolTip.SetToolTip(btn, GetUnicodeName(c));
            }

            foreach (char c in Properties.Settings.Default.EitherEdgeQuotes)
            {
                Button btn = AddPunctuationButton(c.ToString(), BtnPunctuationClickEitherEdge);
                toolTip.SetToolTip(btn, GetUnicodeName(c));
            }

            textBoxReorderedText.Font = _font;
        }

        private Button AddPunctuationButton(string strPunctuation, EventHandler btnPunctuationClick)
        {
            var btnPunctuation = new Button
                                     {
                                         Text = strPunctuation,
                                         Font = _font,
                                         AutoSize = true
                                     };
            btnPunctuation.Click += btnPunctuationClick;
            btnPunctuation.PreviewKeyDown += BtnPreviewKeyDown;
            flowLayoutPanelPunctuation.Controls.Add(btnPunctuation);
            return btnPunctuation;
        }

        void BtnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
                buttonUndoLast_Click(null, null);
        }

        private void BtnPunctuationClickLeftEdge(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null)
            {
                textBoxReorderedText.Text += ' ' + btnWord.Text;
                strPrefix = "";
            }
        }

        private void BtnPunctuationClickRightEdge(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null)
                textBoxReorderedText.Text += btnWord.Text;
        }

        private void BtnPunctuationClickEitherEdge(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null)
                textBoxReorderedText.Text += btnWord.Text;
        }

        private string strPrefix = "";

        protected void BtnWordClick(object sender, EventArgs e)
        {
            var btnWord = sender as Button;
            if (btnWord != null)
            {
                textBoxReorderedText.Text += strPrefix + btnWord.Text;
                btnWord.ForeColor = SystemColors.ControlDark;
                strPrefix = " ";
            }
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
            string strLabelToDelete = strReorderedText.Substring(nIndex + 1);
            if (nIndex != -1)
                textBoxReorderedText.Text = strReorderedText.Substring(0, nIndex);
            foreach (Button btn in flowLayoutPanelWords.Controls)
            {
                if (btn.Text == strLabelToDelete)
                {
                    btn.ForeColor = SystemColors.ControlText;
                    break;
                }
            }
        }
    }
}
