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
        private AdaptItEncConverter m_theEC;
        public List<string> SourceWords;
        public List<string> TargetWords;
        public List<string> SourceStringsInBetween;
        public List<string> TargetStringsInBetween;
        protected ProjectSettings.LanguageInfo liSourceLang, liTargetLang;

        private GlossingForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public GlossingForm(ProjectSettings projSettings, string strSentence, 
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection,
            TeamMemberData loggedOnMember)
            : base(true)
        {
            InitializeComponent();
            Localizer.Ctrl(this);
            try
            {
                m_theEC = AdaptItGlossing.InitLookupAdapter(projSettings, eBtDirection, loggedOnMember,
                    out liSourceLang, out liTargetLang);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
                return;
            }

            // get the EncConverter to break apart the given sentence into bundles
            SourceSentence = strSentence;
            m_theEC.SplitAndConvert(strSentence, out SourceWords, out SourceStringsInBetween, 
                out TargetWords, out TargetStringsInBetween);
            if (SourceWords.Count == 0)
                throw new ApplicationException(Localizer.Str("No sentence to gloss!"));

            if (liSourceLang.DoRtl)
                flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;

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
                    m_theEC.AddEntryPair(strSourceWord, strTargetWord, false);
                }
                m_theEC.AddEntryPairSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Localizer.Str("adding {0}->{1} gave the error: {2}"),
                                              strSourceWord,
                                              strTargetWord,
                                              ex.Message),
                                StoryEditor.OseCaption);
                return;
            }

            System.Diagnostics.Debug.Assert(flowLayoutPanel.Controls.Count > 0);
            var gc = (GlossingControl) flowLayoutPanel.Controls[0];
            string strTargetSentence = gc.InBetweenBeforeTarget + 
                gc.TargetWord + gc.InBetweenAfterTarget;

            for (int i = 1; i < flowLayoutPanel.Controls.Count; i++)
            {
                gc = (GlossingControl)flowLayoutPanel.Controls[i];
                strTargetSentence += " " + gc.InBetweenBeforeTarget + 
                    gc.TargetWord + gc.InBetweenAfterTarget;
            }

            TargetSentence = strTargetSentence;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void MergeWithNext(GlossingControl control)
        {
            int nIndex = flowLayoutPanel.Controls.IndexOf(control);
            // add the contents of this one to the next one
            GlossingControl theNextGC = (GlossingControl)flowLayoutPanel.Controls[nIndex + 1];
            theNextGC.SourceWord = String.Format("{0} {1}", control.SourceWord, theNextGC.SourceWord);

            // as a general rule, the target form would just be the concatenation of the two
            //  target forms. 
            string strTargetPhrase = String.Format("{0} {1}", control.TargetWord, theNextGC.TargetWord);

            // But by combining it, we should at least see if this would result
            //  in a new form if it were converted. So let's check that.
            // and use that in any case if there were ambiguities in the original
            //  (concatenated) target forms
            string strConvertedTarget = SafeConvert(theNextGC.SourceWord);
            if ((strConvertedTarget != theNextGC.SourceWord) ||
                (strTargetPhrase.IndexOf(GlossingControl.CstrAmbiguitySeparator) != -1))
            {
                strTargetPhrase = strConvertedTarget; // means it was converted or had ambiguities
            }
            theNextGC.TargetWord = strTargetPhrase;

            flowLayoutPanel.Controls.Remove(control);
            theNextGC.Focus();
        }

        protected string SafeConvert(string strInput)
        {
            try
            {
                return m_theEC.Convert(strInput);
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
                if (m_theEC.EditTargetWords(glossingControl.SourceWord))
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
                List<string> lstSimilarWords = m_theEC.GetSimilarWords(glossingControl.SourceWord);
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
            SourceSentence = SourceSentence.Replace(theGc.SourceWord, strNewSourceWord);
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
                string strNewSourceWord = m_theEC.EditKnowledgeBase(glossingControl.SourceWord);
                if (!String.IsNullOrEmpty(strNewSourceWord))
                    Update(glossingControl, strNewSourceWord);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }
    }
}
