using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public partial class GlossingForm : TopForm
    {
        internal static char[] achWordDelimiters = new[] { ' ' };
        private AdaptItEncConverter m_theEC = null;
        public List<string> SourceWords;
        public List<string> TargetWords;
        public List<string> SourceStringsInBetween;
        public List<string> TargetStringsInBetween;
        protected ProjectSettings.LanguageInfo liSourceLang, liTargetLang;

        public GlossingForm(ProjectSettings projSettings, string strSentence, 
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection)
            : base(true)
        {
            InitializeComponent();
            m_theEC = AdaptItGlossing.InitLookupAdapter(projSettings, eBtDirection, out liSourceLang, out liTargetLang);

            // get the EncConverter to break apart the given sentence into bundles
            m_theEC.SplitAndConvert(strSentence, out SourceWords, out SourceStringsInBetween, 
                out TargetWords, out TargetStringsInBetween);
            if (SourceWords.Count == 0)
                throw new ApplicationException("No sentence to gloss!");

            if (liSourceLang.DoRtl)
                flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;

            System.Diagnostics.Debug.Assert(SourceWords.Count == TargetWords.Count);
            for (int i = 0; i < SourceWords.Count; i++)
            {
                var gc = new GlossingControl(this,
                    liSourceLang, SourceWords[i], SourceStringsInBetween[i + 1],
                    liTargetLang, TargetWords[i], TargetStringsInBetween[i + 1]);

                flowLayoutPanel.Controls.Add(gc);
            }

            // disable the button on the last one
            ((GlossingControl)flowLayoutPanel.Controls[flowLayoutPanel.Controls.Count - 1]).DisableButton();
        }

        public string TargetSentence { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (GlossingControl aGC in flowLayoutPanel.Controls)
            {
                // means we have to add it to the kb. But only if the user chose
                //  all the ambiguities
                if (aGC.TargetWord.IndexOf(GlossingControl.CstrAmbiguitySeparator) == -1)
                    m_theEC.AddEntryPair(aGC.SourceWord, aGC.TargetWord);
            }

            System.Diagnostics.Debug.Assert(flowLayoutPanel.Controls.Count > 0);
            string strTargetSentence = ((GlossingControl)flowLayoutPanel.Controls[0]).TargetWord;

            for (int i = 1; i < flowLayoutPanel.Controls.Count; i++)
            {
                GlossingControl aGC = (GlossingControl)flowLayoutPanel.Controls[i];
                strTargetSentence += " " + aGC.TargetWord;
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
            string strConvertedTarget = SafeConvert(theNextGC.SourceWord);
            if (strConvertedTarget != theNextGC.SourceWord)
                strTargetPhrase = strConvertedTarget;   // means it was converted
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

            for (int i = 1; i < astrSourceWords.Length; i++)
            {
                string strSourceWord = astrSourceWords[i];
                string strTargetWord = SafeConvert(strSourceWord);
                var gc = new GlossingControl(this,
                    liSourceLang, strSourceWord, "",
                    liTargetLang, strTargetWord, "");

                flowLayoutPanel.Controls.Add(gc);
                flowLayoutPanel.Controls.SetChildIndex(gc, ++nIndex);
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
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
            }
        }
    }
}
