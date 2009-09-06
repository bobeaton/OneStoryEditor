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
            eVernacularToEnglish,   // use Vern -> Natl project (glossing KB)
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

        public static AdaptItEncConverter InitLookupAdapter(ProjectSettings proj, GlossType eGlossType)
        {
            EncConverters aECs = new EncConverters();
            string strName, strConverterSpec;
            ProjectSettings.LanguageInfo liSource, liTarget;
            switch (eGlossType)
            {
                case GlossType.eVernacularToNational:
                    strName = AdaptItLookupConverterName(proj.Vernacular.LangName, proj.NationalBT.LangName);
                    strConverterSpec = AdaptItLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName);
                    liSource = proj.Vernacular;
                    liTarget = proj.NationalBT;
                    break;

                case GlossType.eVernacularToEnglish:    // the glossing KB for the Vern to Natl project
                    strName = AdaptItGlossingLookupConverterName(proj.Vernacular.LangName, proj.NationalBT.LangName);
                    strConverterSpec = AdaptItGlossingLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName);
                    liSource = proj.Vernacular;
                    liTarget = proj.NationalBT; // this is still the national lg project (but the glossing KB)
                    break;

                case GlossType.eNationalToEnglish:
                    strName = AdaptItLookupConverterName(proj.NationalBT.LangName, proj.InternationalBT.LangName);
                    strConverterSpec = AdaptItLookupFileSpec(proj.NationalBT.LangName, proj.InternationalBT.LangName);
                    liSource = proj.NationalBT;         // this is a whole nuther national to English project
                    liTarget = proj.InternationalBT;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new ApplicationException("Wrong glossing type specified. Send to bob_eaton@sall.com for help");
            }

            // just in case the project doesn't exist yet...
            WriteAdaptItProjectFiles(liSource, liTarget, proj.InternationalBT); // move this to AIGuesserEC project when it's mature.

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

        protected static void WriteAdaptItProjectFiles(ProjectSettings.LanguageInfo liSource,
            ProjectSettings.LanguageInfo liTarget, ProjectSettings.LanguageInfo liNavigation)
        {
            // create folders...
            if (!Directory.Exists(AdaptItProjectAdaptationsFolder(liSource.LangName, liTarget.LangName)))
                Directory.CreateDirectory(AdaptItProjectAdaptationsFolder(liSource.LangName, liTarget.LangName));

            // create Project file
            if (!File.Exists(AdaptItProjectFileSpec(liSource.LangName, liTarget.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIProjectFile;
                string strProjectFileContents = String.Format(strFormat,
                    liSource.LangFont.Name,
                    liTarget.LangFont.Name,
                    liNavigation.LangFont.Name,
                    liSource.LangName,
                    liTarget.LangName,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "{}",
                    liSource.FullStop,
                    liTarget.FullStop,
                    (liSource.IsRTL) ? "1" : "0",
                    (liTarget.IsRTL) ? "1" : "0");
                File.WriteAllText(AdaptItProjectFileSpec(liSource.LangName, liTarget.LangName), strProjectFileContents);
            }

            // create main KB
            if (!File.Exists(AdaptItLookupFileSpec(liSource.LangName, liTarget.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSource.LangName, liTarget.LangName);
                File.WriteAllText(AdaptItLookupFileSpec(liSource.LangName, liTarget.LangName), strKBContents);
            }

            if (!File.Exists(AdaptItGlossingLookupFileSpec(liSource.LangName, liTarget.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSource.LangName, liTarget.LangName);
                File.WriteAllText(AdaptItGlossingLookupFileSpec(liSource.LangName, liTarget.LangName), strKBContents);
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
