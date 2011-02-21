using System;
using System.IO;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public class AdaptItGlossing
    {
        protected static string AdaptItGlossingLookupFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strSourceLangName, strTargetLangName),
                "Glossing.xml");
        }

        protected static string AdaptItProjectFolderName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0} to {1} adaptations", strSourceLangName, strTargetLangName);
        }

        protected static string AdaptationFileName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}.xml", AdaptItProjectFolderName(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItLookupFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strSourceLangName, strTargetLangName),
                AdaptationFileName(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItProjectFileSpec(string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strSourceLangName, strTargetLangName),
                "AI-ProjectConfiguration.aic");
        }

        protected static string AdaptItWorkFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Adapt It Unicode Work");
            }
        }

        protected static string AdaptItProjectFolder(string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItWorkFolder, AdaptItProjectFolderName(strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItProjectAdaptationsFolder(string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strSourceLangName, strTargetLangName),
                "Adaptations");
        }

        protected static string AdaptItLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations",
                strSourceLangName, strTargetLangName);
        }

        public static AdaptItEncConverter InitLookupAdapter(ProjectSettings proj, StoryEditor.GlossType eGlossType,
            out ProjectSettings.LanguageInfo liSourceLang, out ProjectSettings.LanguageInfo liTargetLang)
        {
            var aECs = new EncConverters();
            string strName;
            switch (eGlossType)
            {
                case StoryEditor.GlossType.eVernacularToNational:
                    System.Diagnostics.Debug.Assert(
                        !String.IsNullOrEmpty(proj.VernacularToNationalBtAdaptItConverterName));
                    strName = proj.VernacularToNationalBtAdaptItConverterName;
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.NationalBT;
                    break;

                case StoryEditor.GlossType.eVernacularToEnglish:    // the glossing KB for the Vern to Natl project
                    System.Diagnostics.Debug.Assert(
                        !String.IsNullOrEmpty(proj.VernacularToInternationalBtAdaptItConverterName));
                    strName = proj.VernacularToInternationalBtAdaptItConverterName;
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.InternationalBT; // this is still the national lg project (but the glossing KB)
                    break;

                case StoryEditor.GlossType.eNationalToEnglish:
                    System.Diagnostics.Debug.Assert(
                        !String.IsNullOrEmpty(proj.NationalBtToInternationalBtAdaptItConverterName));
                    strName = proj.NationalBtToInternationalBtAdaptItConverterName;
                    liSourceLang = proj.NationalBT;         // this is a whole nuther national to English project
                    liTargetLang = proj.InternationalBT;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new ApplicationException("Wrong glossing type specified. Send to bob_eaton@sall.com for help");
            }
            /*
            // just in case the project doesn't exist yet...
            WriteAdaptItProjectFiles(liSourceLang, liTargetLang, proj.InternationalBT); // move this to AIGuesserEC project when it's mature.

            // if we don't have the converter already in the repository.
            if (!aECs.ContainsKey(strName))
            {
                aECs.AddConversionMap(strName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                    EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);
            }
            */

            if (!aECs.ContainsKey(strName))
                throw new ApplicationException(String.Format(Properties.Resources.IDS_AdaptItConverterDoesntExist,
                                                             strName));

            IEncConverter aEC = aECs[strName];
            System.Diagnostics.Debug.Assert((aEC != null) && (aEC is AdaptItEncConverter));
            AdaptItEncConverter theLookupAdapter = (AdaptItEncConverter)aEC;

            // in order to get the converter to load the database, do a dummy Convert
            theLookupAdapter.Convert("nothing");
            return theLookupAdapter;
        }

        protected static void WriteAdaptItProjectFiles(ProjectSettings.LanguageInfo liSourceLang,
            ProjectSettings.LanguageInfo liTargetLang, ProjectSettings.LanguageInfo liNavigation)
        {
            // create folders...
            if (!Directory.Exists(AdaptItProjectAdaptationsFolder(liSourceLang.LangName, liTargetLang.LangName)))
                Directory.CreateDirectory(AdaptItProjectAdaptationsFolder(liSourceLang.LangName, liTargetLang.LangName));

            // create Project file
            if (!File.Exists(AdaptItProjectFileSpec(liSourceLang.LangName, liTargetLang.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIProjectFile;
                string strProjectFileContents = String.Format(strFormat,
                    liSourceLang.FontToUse.Name,
                    liTargetLang.FontToUse.Name,
                    liNavigation.FontToUse.Name,
                    liSourceLang.LangName,
                    liTargetLang.LangName,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    AIPunctuation(liSourceLang.FullStop),
                    AIPunctuation(liTargetLang.FullStop),
                    (liSourceLang.DoRtl) ? "1" : "0",
                    (liTargetLang.DoRtl) ? "1" : "0");
                File.WriteAllText(AdaptItProjectFileSpec(liSourceLang.LangName, liTargetLang.LangName), strProjectFileContents);
            }

            // create main KB
            if (!File.Exists(AdaptItLookupFileSpec(liSourceLang.LangName, liTargetLang.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSourceLang.LangName, liTargetLang.LangName);
                File.WriteAllText(AdaptItLookupFileSpec(liSourceLang.LangName, liTargetLang.LangName), strKBContents);
            }

            if (!File.Exists(AdaptItGlossingLookupFileSpec(liSourceLang.LangName, liTargetLang.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSourceLang.LangName, liTargetLang.LangName);
                File.WriteAllText(AdaptItGlossingLookupFileSpec(liSourceLang.LangName, liTargetLang.LangName), strKBContents);
            }
        }

        protected static string AIPunctuation(string strSentenceFinalPuncts)
        {
            const string CstrAdaptItPunct = "?.,;:\"!()<>{}[]“”‘’";
            string strAllPunctuation = CstrAdaptItPunct;
            foreach (char ch in strSentenceFinalPuncts)
                if (strAllPunctuation.IndexOf(ch) == -1)
                    strAllPunctuation += ch;
            return strAllPunctuation;
        }
    }
}
