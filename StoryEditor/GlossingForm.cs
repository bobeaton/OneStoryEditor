using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters31;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public partial class GlossingForm : Form
    {
        public enum GlossType
        {
            eUndefined = 0,
            eVernacularToNational,  // use Vern -> Natl project
            eVernacularToEnglish,   // use Vern -> Engl project
            eNationalToEnglish      // use Natl -> Engl project (which can be shared by other clients)
        }

        protected static char[] achWordDelimiters = new[] { ' ' };
        private AdaptItEncConverter m_aEC = null;
        public List<string> TargetWords;
        public List<string> SourceWords;
        public List<string> StringsInBetween;

        public GlossingForm(ProjectSettings proj, string strSentence, GlossType eGlossType)
        {
            InitializeComponent();

            m_aEC = InitLookupAdapter(proj, eGlossType);

            ProjectSettings.LanguageInfo liSource;
            ProjectSettings.LanguageInfo liTarget;
            switch (eGlossType)
            {
                case GlossType.eVernacularToNational:
                    liSource = proj.Vernacular;
                    liTarget = proj.NationalBT;
                    break;

                case GlossType.eVernacularToEnglish:    // the glossing KB for the Vern to Natl project
                    liSource = proj.Vernacular;
                    liTarget = proj.InternationalBT;
                    break;

                case GlossType.eNationalToEnglish:
                    liSource = proj.NationalBT;
                    liTarget = proj.InternationalBT;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new ApplicationException("Wrong glossing type specified. Send to bob_eaton@sall.com for help");
            }

            // get the EncConverter to break apart the given sentence into bundles
            m_aEC.SplitAndConvert(strSentence, out SourceWords, out StringsInBetween, out TargetWords);
            if (SourceWords.Count == 0)
                throw new ApplicationException("No sentence to gloss!");

            if (liSource.IsRTL)
                flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;

            System.Diagnostics.Debug.Assert(SourceWords.Count == TargetWords.Count);
            for (int i = 0; i < SourceWords.Count; i++)
            {
                GlossingControl gc = new GlossingControl(this,
                    liSource, SourceWords[i],
                    liTarget, TargetWords[i],
                    StringsInBetween[i + 1]);

                // Bill Martin says that glossing KBs can't have Map greater than 1.
                if (eGlossType == GlossType.eVernacularToEnglish)
                    gc.DisableButton();

                flowLayoutPanel.Controls.Add(gc);
            }

            // disable the button on the last one
            ((GlossingControl)flowLayoutPanel.Controls[flowLayoutPanel.Controls.Count - 1]).DisableButton();
        }

        public string TargetSentence
        {
            get
            {
                System.Diagnostics.Debug.Assert(flowLayoutPanel.Controls.Count > 0);
                string strTargetSentence = ((GlossingControl)flowLayoutPanel.Controls[0]).TargetWord;

                for (int i = 1; i < flowLayoutPanel.Controls.Count; i++)
                {
                    GlossingControl aGC = (GlossingControl)flowLayoutPanel.Controls[i];
                    strTargetSentence += " " + aGC.TargetWord;
                }

                return strTargetSentence;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (GlossingControl aGC in flowLayoutPanel.Controls)
            {
                // means we have to add it to the kb. The source word will get trimmed, but not the target
                string strTargetWord = aGC.TargetWord.Trim(m_aEC.DelimitersReverse);
                TargetWords.Add(strTargetWord);
                m_aEC.AddEntryPair(aGC.SourceWord, strTargetWord);
            }

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
    }
}
