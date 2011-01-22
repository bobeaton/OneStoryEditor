using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public partial class GlossingForm : Form
    {
        protected static char[] achWordDelimiters = new[] { ' ' };
        private AdaptItEncConverter m_theEC = null;
        public List<string> TargetWords;
        public List<string> SourceWords;
        public List<string> StringsInBetween;
        private Font _fontTarget;

        public GlossingForm(ProjectSettings projSettings, string strSentence, 
            StoryEditor.GlossType eGlossType)
        {
            InitializeComponent();
            ProjectSettings.LanguageInfo liSourceLang, liTargetLang;
            m_theEC = AdaptItGlossing.InitLookupAdapter(projSettings, eGlossType, out liSourceLang, out liTargetLang);

            // get the EncConverter to break apart the given sentence into bundles
            m_theEC.SplitAndConvert(strSentence, out SourceWords, out StringsInBetween, out TargetWords);
            if (SourceWords.Count == 0)
                throw new ApplicationException("No sentence to gloss!");

            if (liSourceLang.DoRtl)
                flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;

            _fontTarget = liTargetLang.FontToUse;

            System.Diagnostics.Debug.Assert(SourceWords.Count == TargetWords.Count);
            for (int i = 0; i < SourceWords.Count; i++)
            {
                var gc = new GlossingControl(this,
                    liSourceLang, SourceWords[i],
                    liTargetLang, TargetWords[i],
                    StringsInBetween[i + 1]);

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
                // means we have to add it to the kb. The source word will get trimmed, but not the target
                string strTargetWord = aGC.TargetWord.Trim(m_theEC.DelimitersReverse);
                TargetWords.Add(strTargetWord);
                m_theEC.AddEntryPair(aGC.SourceWord, strTargetWord);
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
            for (int i = 0; i < flowLayoutPanel.Controls.Count; i++ )
            {
                GlossingControl aGC = (GlossingControl)flowLayoutPanel.Controls[i];
                if (aGC == control)
                {
                    // add the contents of this one to the next one
                    GlossingControl theNextGC = (GlossingControl)flowLayoutPanel.Controls[i + 1];
                    theNextGC.SourceWord = String.Format("{0} {1}", control.SourceWord, theNextGC.SourceWord);
                    theNextGC.TargetWord = String.Format("{0} {1}", control.TargetWord, theNextGC.TargetWord);
                    flowLayoutPanel.Controls.Remove(aGC);
                    theNextGC.Focus();
                    break;
                }
            }
        }

        public bool DoReorder { get; set; }

        private void buttonReorder_Click(object sender, EventArgs e)
        {
            DoReorder = true;
            buttonOK_Click(sender, e);
        }
    }
}
