using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class GlossingControl : UserControl
    {
        protected GlossingForm _parent;
        protected string strAmbiguitySeparator = "¦";

        public GlossingControl(GlossingForm parent, Font fontSourceWord, Color colorSourceWord, string strSourceWord,
            Font fontTargetWord, Color colorTargetWord, string strTargetWord,
            string strInBetween, string strSourceFullStop, string strTargetFullStop)
        {
            _parent = parent;
            InitializeComponent();

            string strFollowingSource = strInBetween.Trim();
            if (!String.IsNullOrEmpty(strFollowingSource))
                buttonJoin.Visible = false;

            textBoxSourceWord.Font = fontSourceWord;
            textBoxSourceWord.ForeColor = colorSourceWord;
            textBoxSourceWord.Text = strSourceWord + strFollowingSource;

            textBoxTargetWord.Font = fontTargetWord;
            textBoxTargetWord.ForeColor = colorTargetWord;
            MatchCollection mc = FindMultipleAmbiguities.Matches(strTargetWord);
            if (mc.Count > 0)
            {
                string strAmbiguityList = mc[0].Groups[1].ToString();
                strTargetWord = strAmbiguityList.Replace("%", strAmbiguitySeparator);
            }

            textBoxTargetWord.Text = strTargetWord + strFollowingSource.Replace(strSourceFullStop, strTargetFullStop);
        }

        public string SourceWord
        {
            get { return textBoxSourceWord.Text; }
            set { textBoxSourceWord.Text = value; }
        }

        public string TargetWord
        {
            get { return textBoxTargetWord.Text; }
            set { textBoxTargetWord.Text = value; }
        }

        public void DisableButton()
        {
            buttonJoin.Visible = false;
        }

        private void textBoxSourceWord_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            AdjustWidth(tb);
        }

        private void textBoxTargetWord_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            AdjustWidth(tb);
        }

        protected void AdjustWidth(TextBox tb)
        {
            if (ResizeTextBoxToFitText(tb))
                AdjustWidthWithSuspendLayout();
        }

        protected static bool ResizeTextBoxToFitText(TextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(1000, tb.Height));
            bool bWidthChanged = (sz.Width != tb.Size.Width);
            if (bWidthChanged)
                tb.Width = sz.Width;
            return bWidthChanged;
        }

        protected void AdjustWidthWithSuspendLayout()
        {
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            // do a similar thing with the layout panel (i.e. give it the same width and infinite height.
            // for some reason GetPreferredSize doesn't give the actual right size... so I'll write my own
            Width = tableLayoutPanel.GetPreferredWidth();
            Height = tableLayoutPanel.GetPreferredHeight();

            tableLayoutPanel.ResumeLayout(true);
            ResumeLayout(true);
        }

        private void buttonJoin_Click(object sender, EventArgs e)
        {
            _parent.MergeWithNext(this);
        }

        protected static Regex FindMultipleAmbiguities = new Regex(@"%\d%(.*)%", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private void textBoxTargetWord_Enter(object sender, EventArgs e)
        {
            contextMenuStripAmbiguityPicker.Items.Clear();
            if (String.IsNullOrEmpty(textBoxTargetWord.Text))
                return;

            // if we have multiple interpretations, then throw up a choice list
            if (textBoxTargetWord.Text.IndexOf(strAmbiguitySeparator) != -1)
            {
                string[] astrWords = textBoxTargetWord.Text.Split(strAmbiguitySeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string strWord in astrWords)
                {
                    ToolStripMenuItem aTSMI = new ToolStripMenuItem(strWord, null, OnSelectAmbiguity);
                    aTSMI.Font = textBoxTargetWord.Font;
                    contextMenuStripAmbiguityPicker.Items.Add(aTSMI);
                }

                contextMenuStripAmbiguityPicker.Show(textBoxTargetWord, textBoxTargetWord.Bounds.Location, ToolStripDropDownDirection.BelowRight);
            }
        }

        void OnSelectAmbiguity(object sender, EventArgs e)
        {
            ToolStripMenuItem aTSMI = (ToolStripMenuItem)sender;
            textBoxTargetWord.Text = aTSMI.Text;
        }
    }
}
