using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using NetLoc;
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public partial class GlossingForm : TopForm
    {
        internal static char[] achWordDelimiters = new[] { ' ' };
        private readonly AdaptItEncConverter _theEc;
        private static BreakIterator _theWordBreaker;
        public List<string> SourceWords;
        public List<string> TargetWords;
        public List<string> SourceStringsInBetween;
        public List<string> TargetStringsInBetween;
        protected ProjectSettings.LanguageInfo liSourceLang, liTargetLang;
        private bool _bUseWordBreakIterator;

        private GlossingForm(bool bUseWordBreakIterator)
        {
            _bUseWordBreakIterator = bUseWordBreakIterator;
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public GlossingForm(ProjectSettings projSettings, string strSentence, 
                            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection,
                            TeamMemberData loggedOnMember, bool bUseWordBreakIterator,
                            DirectableEncConverter ecSourceWordTransliterator)
        {
            _bUseWordBreakIterator = bUseWordBreakIterator;
            InitializeComponent();
            Localizer.Ctrl(this);
            try
            {
                _theEc = AdaptItGlossing.InitLookupAdapter(projSettings, eBtDirection, loggedOnMember,
                                                           out liSourceLang, out liTargetLang);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }


            if (_bUseWordBreakIterator)
            {
                try
                {
                    if (_theWordBreaker == null)
                        _theWordBreaker = new BreakIterator();

                    strSentence = _theWordBreaker.AddWordBreaks(strSentence);
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
            }

            // get the EncConverter to break apart the given sentence into bundles
            SourceSentence = strSentence;
            _theEc.SplitAndConvert(strSentence, out SourceWords, out SourceStringsInBetween, 
                out TargetWords, out TargetStringsInBetween);
            if (SourceWords.Count == 0)
                throw new ApplicationException(Localizer.Str("No sentence to gloss!"));

            if (liSourceLang.DoRtl)
                flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;

            // initialize the transliterator
            GlossingControl.SourceWordTransliterator = ecSourceWordTransliterator;

            System.Diagnostics.Debug.Assert(SourceWords.Count == TargetWords.Count);
            string strBeforeSource = SourceStringsInBetween[0];
            string strBeforeTarget = TargetStringsInBetween[0];
            for (int i = 0; i < SourceWords.Count; i++)
            {
                var gc = new GlossingControl(this,
                    liSourceLang, ref strBeforeSource, SourceWords[i], SourceStringsInBetween[i + 1],
                    liTargetLang, ref strBeforeTarget, TargetWords[i], TargetStringsInBetween[i + 1]);

                flowLayoutPanel.Controls.Add(gc);
            }

            // disable the button on the last one
            ((GlossingControl)flowLayoutPanel.Controls[flowLayoutPanel.Controls.Count - 1]).DisableButton();
        }

        public new DialogResult ShowDialog()
        {
            if (Properties.Settings.Default.GlossingFormHeight != 0)
            {
                Bounds = new Rectangle(Properties.Settings.Default.GlossingFormLocation,
                                       new Size(Properties.Settings.Default.GlossingFormWidth,
                                                Properties.Settings.Default.GlossingFormHeight));
            }

            return base.ShowDialog();
        }

        public string SourceSentence { get; set; }
        public string TargetSentence { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string strSourceWord = null, strTargetWord = null;
            try
            {
                foreach (GlossingControl aGc in
                    flowLayoutPanel.Controls.Cast<GlossingControl>().Where(aGC => (aGC.TargetWord.IndexOf(GlossingControl.CstrAmbiguitySeparator) == -1) && aGC.Modified))
                {
                    strSourceWord = aGc.SourceWord;
                    strTargetWord = aGc.TargetWord;
                    _theEc.AddEntryPair(strSourceWord, strTargetWord, false);
                }
                _theEc.AddEntryPairSave();
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(String.Format(Localizer.Str("adding {0}->{1} gave the error: {2}"),
                                              strSourceWord,
                                              strTargetWord,
                                              ex.Message),
                                StoryEditor.OseCaption);
                return;
            }

            System.Diagnostics.Debug.Assert(flowLayoutPanel.Controls.Count > 0);

            TargetSentence = GetFullSentence(gc => gc.TargetWord);
            SourceSentence = GetFullSentence(gc => gc.SourceWord);

            DialogResult = DialogResult.OK;
            Close();
        }

        private delegate string GetWordValue(GlossingControl gc);

        private string GetFullSentence(GetWordValue pValue)
        {
            var gc = (GlossingControl) flowLayoutPanel.Controls[0];
            var strTargetSentence = gc.InBetweenBeforeTarget +
                                       pValue(gc) + gc.InBetweenAfterTarget;

            for (var i = 1; i < flowLayoutPanel.Controls.Count; i++)
            {
                gc = (GlossingControl) flowLayoutPanel.Controls[i];
                strTargetSentence += " " + gc.InBetweenBeforeTarget +
                                     pValue(gc) + gc.InBetweenAfterTarget;
            }
            return strTargetSentence;
        }

        public void MergeWithNext(GlossingControl control)
        {
            int nIndex = flowLayoutPanel.Controls.IndexOf(control);
            // add the contents of this one to the next one
            var theNextGc = (GlossingControl)flowLayoutPanel.Controls[nIndex + 1];
            var newSourceWord = String.Format("{0}{1}{2}",
                                              control.SourceWord,
                                              (_bUseWordBreakIterator) ? "" : " ",    // if word breaking, on join, we don't want the space
                                              theNextGc.SourceWord);

            if (_bUseWordBreakIterator)
                Update(theNextGc, newSourceWord);    // this function also updates the SourceSentence field, without which we won't re-write the StoryBtPane with this now missing space combined word
            else
                theNextGc.SourceWord = newSourceWord;

            // as a general rule, the target form would just be the concatenation of the two
            //  target forms. 
            string strTargetPhrase = String.Format("{0} {1}", control.TargetWord, theNextGc.TargetWord);

            // But by combining it, we should at least see if this would result
            //  in a new form if it were converted. So let's check that.
            // and use that in any case if there were ambiguities in the original
            //  (concatenated) target forms
            string strConvertedTarget = SafeConvert(theNextGc.SourceWord);
            if ((strConvertedTarget != theNextGc.SourceWord) ||
                (strTargetPhrase.IndexOf(GlossingControl.CstrAmbiguitySeparator) != -1))
            {
                strTargetPhrase = strConvertedTarget; // means it was converted or had ambiguities
            }
            theNextGc.TargetWord = strTargetPhrase;

            flowLayoutPanel.Controls.Remove(control);
            theNextGc.Focus();
        }

        protected string SafeConvert(string strInput)
        {
            try
            {
                return _theEc.Convert(strInput);
            }
            catch
            {
            }
            return strInput;
        }

        public bool DoReorder { get; set; }

        private void buttonReorder_Click(object sender, EventArgs e)
        {
            DoReorder = true;
            buttonOK_Click(sender, e);
        }

        public void SplitMeUp(GlossingControl control)
        {
            System.Diagnostics.Debug.Assert(control.SourceWord.Split(achWordDelimiters).Length > 1);
            int nIndex = flowLayoutPanel.Controls.IndexOf(control);

            string[] astrSourceWords = control.SourceWord.Split(achWordDelimiters, StringSplitOptions.RemoveEmptyEntries);
            control.SourceWord = astrSourceWords[0];

            // since splitting can have unpredictable side effects (e.g. two source words
            //  becoming a single word, just to name one), go ahead and reconvert the
            //  split source words again.
            control.TargetWord = SafeConvert(control.SourceWord);

            GlossingControl ctrlNew = null;
            for (int i = 1; i < astrSourceWords.Length; i++)
            {
                string strSourceWord = astrSourceWords[i];
                string strTargetWord = SafeConvert(strSourceWord);
                string strDummy = null;
                ctrlNew = new GlossingControl(this,
                    liSourceLang, ref strDummy, strSourceWord, "",
                    liTargetLang, ref strDummy, strTargetWord, "");

                flowLayoutPanel.Controls.Add(ctrlNew);
                flowLayoutPanel.Controls.SetChildIndex(ctrlNew, ++nIndex);
            }

            if (ctrlNew != null)
            {
                ctrlNew.InBetweenAfterSource = control.InBetweenAfterSource;
                control.InBetweenAfterSource = null;
                ctrlNew.InBetweenAfterTarget = control.InBetweenAfterTarget;
                control.InBetweenAfterTarget = null;
            }
        }

        public void EditTargetWords(GlossingControl glossingControl)
        {
            try
            {
                if (_theEc.EditTargetWords(glossingControl.SourceWord))
                    glossingControl.TargetWord = SafeConvert(glossingControl.SourceWord);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        public void CheckForSimilarWords(GlossingControl glossingControl)
        {
            try
            {
                List<string> lstSimilarWords = _theEc.GetSimilarWords(glossingControl.SourceWord);
                if (lstSimilarWords != null)
                    glossingControl.ShowSimilarWordList(lstSimilarWords);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        public void Update(GlossingControl theGc, string strNewSourceWord)
        {
            // UPDATE: this was causing 'man' => to 'mant' also change 'woman' to 'womant'... So just do this during 'onOk'
            // SourceSentence = SourceSentence.Replace(theGc.SourceWord, strNewSourceWord);
            theGc.SourceWord = strNewSourceWord;
            string strNewTarget = SafeConvert(theGc.SourceWord);
            theGc.TargetWord = strNewTarget;
            if (strNewTarget == theGc.SourceWord)
                CheckForSimilarWords(theGc);
        }

        public void EditKb(GlossingControl glossingControl)
        {
            try
            {
                string strNewSourceWord = _theEc.EditKnowledgeBase(glossingControl.SourceWord);
                if (!String.IsNullOrEmpty(strNewSourceWord))
                    Update(glossingControl, strNewSourceWord);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private void GlossingFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.GlossingFormLocation = Location;
            Properties.Settings.Default.GlossingFormHeight = Bounds.Height;
            Properties.Settings.Default.GlossingFormWidth = Bounds.Width;
            Properties.Settings.Default.Save();
        }
    }
}
