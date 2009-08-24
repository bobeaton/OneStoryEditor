using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters30;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public partial class GlossingForm : Form
    {
        protected static char[] achWordDelimiters = new[] { ' ' };
        private AdaptItEncConverter m_aEC = null;
        public List<string> TargetWords;
        public List<string> SourceWords;
        public List<string> StringsInBetween;

        public GlossingForm(ProjectSettings proj, string strSentence, bool bVernacularToNational)
        {
            InitializeComponent();

            m_aEC = InitLookupAdapter(proj, bVernacularToNational);

            m_aEC.SplitAndConvert(strSentence, out SourceWords, out StringsInBetween, out TargetWords);

            if (SourceWords.Count == 0)
                throw new ApplicationException("No sentence to gloss!");

            string strSourceFullStop, strTargetFullStop;
            Font fontSource, fontTarget;
            Color colorSource, colorTarget;
            if (bVernacularToNational)
            {
                strSourceFullStop = proj.Vernacular.FullStop;
                strTargetFullStop = proj.NationalBT.FullStop;
                fontSource = proj.Vernacular.Font;
                fontTarget = proj.NationalBT.Font;
                colorSource = proj.Vernacular.FontColor;
                colorTarget = proj.NationalBT.FontColor;
            }
            else
            {
                strSourceFullStop = proj.Vernacular.FullStop;
                strTargetFullStop = proj.InternationalBT.FullStop;
                fontSource = proj.Vernacular.Font;
                fontTarget = proj.InternationalBT.Font;
                colorSource = proj.Vernacular.FontColor;
                colorTarget = proj.InternationalBT.FontColor;
            }

            System.Diagnostics.Debug.Assert(SourceWords.Count == TargetWords.Count);
            for (int i = 0; i < SourceWords.Count; i++)
            {
                GlossingControl gc = new GlossingControl(this, fontSource, colorSource, SourceWords[i],
                    fontTarget, colorTarget, TargetWords[i],
                     StringsInBetween[i + 1], strSourceFullStop, strTargetFullStop);

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

        protected static string AdaptItGlossingLookupFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}\Glossing.xml",
                AdaptItProjectFolder(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItLookupFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}\{1} to {2} adaptations.xml",
                AdaptItProjectFolder(strSourceLangName, strTargetLangName), strSourceLangName, strTargetLangName);
        }

        protected static string AdaptItProjectFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}\AI-ProjectConfiguration.aic",
                AdaptItProjectFolder(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItWorkFolder
        {
            get
            {
                return String.Format(@"{0}\Adapt It Unicode Work",
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
        }

        protected static string AdaptItProjectFolder(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}\{1} to {2} adaptations",
                AdaptItWorkFolder, strSourceLangName, strTargetLangName);
        }

        protected static string AdaptItProjectAdaptationsFolder(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}\Adaptations",
                AdaptItProjectFolder(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItGlossingLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations (Glossing Knowledge Base)",
                strSourceLangName, strTargetLangName);
        }

        protected static string AdaptItLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations",
                strSourceLangName, strTargetLangName);
        }

        protected AdaptItEncConverter InitLookupAdapter(ProjectSettings proj, bool bVernacularToNational)
        {
            string strSourceLangName = proj.Vernacular.LangName;
            string strTargetLangName = proj.NationalBT.LangName;

            EncConverters aECs = new EncConverters();
            string strName, strConverterSpec;
            if (bVernacularToNational)
            {
                strName = AdaptItLookupConverterName(strSourceLangName, strTargetLangName);
                strConverterSpec = AdaptItLookupFileSpec(strSourceLangName, strTargetLangName);
            }
            else
            {
                strName = AdaptItGlossingLookupConverterName(strSourceLangName, strTargetLangName);
                strConverterSpec = AdaptItGlossingLookupFileSpec(strSourceLangName, strTargetLangName);
            }

            // just in case the project doesn't exist yet...
            WriteAdaptItProjectFiles(proj);

            // if we don't have the converter already in the repository.
            if (!aECs.ContainsKey(strName))
            {
                aECs.AddConversionMap(strName, strConverterSpec, ConvType.Unicode_to_from_Unicode, 
                    EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);
            }

            IEncConverter aEC = aECs[strName];
            System.Diagnostics.Debug.Assert((aEC != null) && (aEC is AdaptItEncConverter));
            AdaptItEncConverter theLookupAdapter = (AdaptItEncConverter)aEC;
            
            // in order to get the converter to load the database, do a dummy Convert
            theLookupAdapter.Convert("nothing");
            return theLookupAdapter;
        }

        protected void WriteAdaptItProjectFiles(ProjectSettings proj)
        {
            // create folders...
            if (!Directory.Exists(AdaptItProjectAdaptationsFolder(proj.Vernacular.LangName, proj.NationalBT.LangName)))
                Directory.CreateDirectory(AdaptItProjectAdaptationsFolder(proj.Vernacular.LangName, proj.NationalBT.LangName));

            // create Project file
            if (!File.Exists(AdaptItProjectFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIProjectFile;
                string strProjectFileContents = String.Format(strFormat,
                    proj.Vernacular.Font.Name,
                    proj.NationalBT.Font.Name,
                    proj.InternationalBT.Font.Name,
                    proj.Vernacular.LangName,
                    proj.NationalBT.LangName,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "{}",
                    proj.Vernacular.FullStop,
                    proj.NationalBT.FullStop,
                    (proj.Vernacular.IsRTL) ? "1" : "0",
                    (proj.NationalBT.IsRTL) ? "1" : "0");
                File.WriteAllText(AdaptItProjectFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName), strProjectFileContents);
            }

            // create main KB
            if (!File.Exists(AdaptItLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, proj.Vernacular.LangName, proj.NationalBT.LangName);
                File.WriteAllText(AdaptItLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName), strKBContents);
            }

            if (!File.Exists(AdaptItGlossingLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, proj.Vernacular.LangName, proj.NationalBT.LangName);
                File.WriteAllText(AdaptItGlossingLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName), strKBContents);
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
                    break;
                }
            }
        }
    }
}
