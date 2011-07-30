using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public partial class GlossingControl : UserControl
    {
        protected GlossingForm _parent;
        public const string CstrAmbiguitySeparator = "¦";
        protected string _strTargetKeyboard, _strSourceKeyboard;

        public GlossingControl(GlossingForm parent, 
            ProjectSettings.LanguageInfo liSource, ref string strBeforeSource, string strSourceWord, string strSourceInBetween,
            ProjectSettings.LanguageInfo liTarget, ref string strBeforeTarget, string strTargetWord, string strTargetInBetween)
        {
            _parent = parent;
            InitializeComponent();

            _strTargetKeyboard = liTarget.Keyboard;
            _strSourceKeyboard = liSource.Keyboard;

            InBetweenBeforeSource = strBeforeSource;
            InBetweenBeforeTarget = strBeforeTarget;

            if (liSource.DoRtl)
                this.tableLayoutPanel.RightToLeft = RightToLeft.Yes;

            InBetweenAfterSource = ProcessInBetween(strSourceInBetween, ref strBeforeSource);
            if (!String.IsNullOrEmpty(InBetweenAfterSource))
                buttonJoin.Visible = false;

            InBetweenAfterTarget = ProcessInBetween(strTargetInBetween, ref strBeforeTarget);

            textBoxSourceWord.Font = liSource.FontToUse;
            textBoxSourceWord.ForeColor = liSource.FontColor;
            if (liSource.DoRtl)
                textBoxSourceWord.RightToLeft = RightToLeft.Yes;
            textBoxSourceWord.Text = strSourceWord;

            textBoxTargetWord.Font = liTarget.FontToUse;
            textBoxTargetWord.ForeColor = liTarget.FontColor;
            if (liTarget.DoRtl)
                textBoxTargetWord.RightToLeft = RightToLeft.Yes;

            /*
            if (strTargetInBetween != " ")
                strTargetWord += strTargetInBetween;
            */
            TargetWord = strTargetWord;

            Modified = false;   // reinitialize
        }

        public string InBetweenBeforeSource { get; set; }
        public string InBetweenAfterSource { get; set; }
        public string InBetweenBeforeTarget { get; set; }
        public string InBetweenAfterTarget { get; set; }

        /// <summary>
        /// Turn the white space and punctuation between words into strings that follow
        /// the current word, or precede the next word
        /// </summary>
        /// <param name="strInBetweenAfter">what comes after the current word (some of which might be for after this current word and some (past a space) is what comes before the next word</param>
        /// <param name="strBeforeNext">what should go before the current word</param>
        private static string ProcessInBetween(string strInBetweenAfter, ref string strBeforeNext)
        {
            if (String.IsNullOrEmpty(strInBetweenAfter))
                return null;

            string[] astr = strInBetweenAfter.Split(GlossingForm.achWordDelimiters, StringSplitOptions.RemoveEmptyEntries);
            if (astr.Length >= 2)
            {
                // e.g. /, "/
                strInBetweenAfter = astr[0];
                strBeforeNext = null;
                for (int i = 1; i < astr.Length; i++ )
                    strBeforeNext += astr[i];
            }
            else if (astr.Length == 1)
            {
                // either, e.g. / '/, but not / ' / (which we want to fix by stripping
                //  out the initial space as in the else case
                if (strInBetweenAfter[0] == ' ' && 
                    (strInBetweenAfter[strInBetweenAfter.Length - 1] != ' '))
                {
                    strBeforeNext = astr[0];
                    strInBetweenAfter = null;
                }
                else
                {
                    // or /' / or / ' /
                    strBeforeNext = null;
                    strInBetweenAfter = astr[0];
                }
            }
            else
            {
                // must have just been a space
                strBeforeNext = strInBetweenAfter = null;
            }

            return strInBetweenAfter;
        }

        public string SourceWord
        {
            get { return textBoxSourceWord.Text; }
            set
            {
                textBoxSourceWord.Text = value;
                ResizeTextBoxToFitText(textBoxSourceWord);
            }
        }

        protected static Regex FindMultipleAmbiguities = new Regex(@"%\d%", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        public string TargetWord
        {
            get { return textBoxTargetWord.Text; }
            set
            {
                string strValue = FindMultipleAmbiguities.Replace(value, "");
                string[] astrTokens = strValue.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                if (astrTokens.Length > 1)
                {
                    string strLine = astrTokens[0];
                    for (int i = 1; i < astrTokens.Length; i++)
                        strLine += CstrAmbiguitySeparator + astrTokens[i].Trim();
                    value = strLine;
                }
#if false
                MatchCollection mc = FindMultipleAmbiguities.Matches(value);
                if (mc.Count > 0)
                {
                    
                    try
                    {
                        string[] astrTokens = value.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);

                        // the zeroth token is the count of the first ambiguous word (in case
                        //  there are multiple -- e.g. after joining two ambiguous words).
                        int nIndex = -1;
                        string strLine = null;
                        do
                        {
                            int nNumOfAmbiguities = 0;
                            try
                            {
                                if (astrTokens[++nIndex] == " ")    // if in between two sets of ambiguous strings
                                    strLine += astrTokens[nIndex++];
                                nNumOfAmbiguities = Convert.ToInt32(astrTokens[nIndex]);
                            }
                            catch { }

                            strLine += astrTokens[++nIndex];
                            for (int i = 1; i < nNumOfAmbiguities; i++)
                                strLine += CstrAmbiguitySeparator + astrTokens[++nIndex];

                        } while (nIndex < (astrTokens.Length - 1));
                        value = strLine;
                    }
                    catch { }
                }
#endif

                textBoxTargetWord.Text = value;
                ResizeTextBoxToFitText(textBoxTargetWord);
            }
        }

        public void DisableButton()
        {
            buttonJoin.Visible = false;
        }

        public bool Modified { get; set; }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            AdjustWidth(tb);
            Modified = true;
        }

        protected void AdjustWidth(TextBox tb)
        {
            if (ResizeTextBoxToFitText(tb))
                AdjustWidthWithSuspendLayout();
        }

        protected static bool ResizeTextBoxToFitText(TextBox tb)
        {
            Size sz = tb.GetPreferredSize(new Size(0, 0));
            bool bWidthChanged = (sz.Width != tb.Width);
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

        private void textBoxTargetWord_Enter(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_strTargetKeyboard))
                KeyboardController.ActivateKeyboard(_strTargetKeyboard);

            if (SourceWord == TargetWord)
            {
                // this might be because there was no lookup found. In this case, let's
                //  throw up the context menu of "similar" words to make sure they might
                //  want to correct the source word.
                _parent.CheckForSimilarWords(this);
            }

            if (textBoxTargetWord.Text.IndexOf(CstrAmbiguitySeparator) != -1)
                contextMenuStripAmbiguityPicker.Show(textBoxTargetWord, textBoxTargetWord.Bounds.Location, ToolStripDropDownDirection.BelowRight);
        }

        void textBoxTargetWord_Leave(object sender, System.EventArgs e)
        {
            KeyboardController.DeactivateKeyboard();
        }

        void OnSelectAmbiguity(object sender, EventArgs e)
        {
            ToolStripMenuItem aTSMI = (ToolStripMenuItem)sender;
            textBoxTargetWord.Text = aTSMI.Text;
        }

        private void textBoxTargetWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("\t");
                if (buttonJoin.Visible)
                    SendKeys.Send("\t");
                e.Handled = true;
            }
        }

        public void ShowSimilarWordList(List<string> lstSimilarWords)
        {
            SimilarWords = lstSimilarWords;
            Point pt = textBoxSourceWord.Bounds.Location;
            pt.Offset(textBoxSourceWord.Width, 0);
            contextMenuStripForSplitting.Show(textBoxSourceWord,
                                              pt, ToolStripDropDownDirection.AboveRight);
        }
        
        private List<string> SimilarWords;
        private void contextMenuStripForSplitting_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            const int CnFixedItemsStripForSplitting = 4;
            while (contextMenuStripForSplitting.Items.Count > CnFixedItemsStripForSplitting)
                contextMenuStripForSplitting.Items.RemoveAt(contextMenuStripForSplitting.Items.Count - 1);

            if (String.IsNullOrEmpty(SourceWord))
                return;

            splitToolStripMenuItem.Enabled = (SourceWord.Split(GlossingForm.achWordDelimiters).Length > 1);

            // if we have some possible similar words, then add them to give the user 
            //  the chance to change the source word to one of these forms.
            if (SimilarWords != null)
            {
                foreach (string strSimilarWord in SimilarWords)
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strSimilarWord));
                    var aTsmi = new ToolStripMenuItem(strSimilarWord, null, OnChangeSourceWord) { Font = textBoxSourceWord.Font };
                    contextMenuStripForSplitting.Items.Add(aTsmi);
                }
            }
        }

        private void OnChangeSourceWord(object sender, EventArgs e)
        {
            var aTsmi = (ToolStripMenuItem)sender;
            _parent.Update(this, aTsmi.Text);
        }

        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _parent.SplitMeUp(this);
        }

        private void editTargetWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _parent.EditTargetWords(this);
        }

        private void contextMenuStripAmbiguityPicker_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (contextMenuStripAmbiguityPicker.Items.Count > 1)
                contextMenuStripAmbiguityPicker.Items.RemoveAt(0);
            if (String.IsNullOrEmpty(textBoxTargetWord.Text))
                return;

            // if we have multiple interpretations, then throw up a choice list
            if (textBoxTargetWord.Text.IndexOf(CstrAmbiguitySeparator) != -1)
            {
                string[] astrWords = textBoxTargetWord.Text.Split(CstrAmbiguitySeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < astrWords.Length; i++)
                {
                    string strWord = astrWords[i];
                    var aTSMI = new ToolStripMenuItem(strWord, null, OnSelectAmbiguity) { Font = textBoxTargetWord.Font };
                    contextMenuStripAmbiguityPicker.Items.Insert(i, aTSMI);
                }
            }
        }

        private void correctSpellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new CorrectSourceWordForm
                          {
                              Font = textBoxSourceWord.Font,
                              CorrectedWord = SourceWord
                          };

            if (!String.IsNullOrEmpty(_strSourceKeyboard))
                KeyboardController.ActivateKeyboard(_strSourceKeyboard);
            if (dlg.ShowDialog() == DialogResult.OK)
                _parent.Update(this, dlg.CorrectedWord);
            KeyboardController.DeactivateKeyboard();
        }

        private void retranslateSourceWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _parent.Update(this, SourceWord);
        }

        private void editTranslationDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _parent.EditKb(this);
        }
    }
}
