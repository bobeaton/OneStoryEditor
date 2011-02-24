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

        public static string AdaptItLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations",
                strSourceLangName, strTargetLangName);
        }

        public static AdaptItEncConverter InitLookupAdapter(ProjectSettings proj, 
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection,
            out ProjectSettings.LanguageInfo liSourceLang, out ProjectSettings.LanguageInfo liTargetLang)
        {
            var aECs = new EncConverters();
            string strName;
            switch (eBtDirection)
            {
                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt:
                    strName = (proj.VernacularToNationalBt != null) ? proj.VernacularToNationalBt.ConverterName : null;
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.NationalBT;
                    break;

                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt:    // the glossing KB for the Vern to Natl project
                    strName = (proj.VernacularToInternationalBt != null) ? proj.VernacularToInternationalBt.ConverterName : null;
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.InternationalBT; // this is still the national lg project (but the glossing KB)
                    break;

                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt:
                    strName = (proj.NationalBtToInternationalBt != null) ? proj.NationalBtToInternationalBt.ConverterName : null;
                    liSourceLang = proj.NationalBT;         // this is a whole nuther national to English project
                    liTargetLang = proj.InternationalBT;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new ApplicationException("Wrong glossing type specified. Send to bob_eaton@sall.com for help");
            }

            // just in case the project doesn't exist yet...
            WriteAdaptItProjectFiles(liSourceLang, liTargetLang, proj.InternationalBT); // move this to AIGuesserEC project when it's mature.

            // if there wasn't one configured, then just use the default
            if (String.IsNullOrEmpty(strName))
                strName = AdaptItLookupConverterName(liSourceLang.LangName, liTargetLang.LangName);

            // if we don't have the converter already in the repository.
            if (!aECs.ContainsKey(strName))
            {
                string strConverterSpec = AdaptItLookupFileSpec(proj.Vernacular.LangName, proj.NationalBT.LangName); 
                aECs.AddConversionMap(strName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                    EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);
            }

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
                string strSourcePunct, strTargetPunct;
                AiPunctuation(liSourceLang.FullStop, liTargetLang.FullStop, 
                    out strSourcePunct, out strTargetPunct);
                string strFormat = Properties.Settings.Default.DefaultAIProjectFile;
                string strProjectFileContents = String.Format(strFormat,
                    liSourceLang.FontToUse.Name,
                    liTargetLang.FontToUse.Name,
                    liNavigation.FontToUse.Name,
                    liSourceLang.LangName,
                    liTargetLang.LangName,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    strSourcePunct,
                    strTargetPunct,
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

        const string CstrAdaptItPunct = "?.,;:\"!()<>{}[]“”‘’";
            
        private static void AiPunctuation(string strSourceSentenceFinalPuncts, string strTargetSentenceFinalPuncts, out string strSourcePunct, out string strTargetPunct)
        {
            strSourcePunct = strTargetPunct = null;
            int nLen = Math.Min(strSourceSentenceFinalPuncts.Length, strTargetSentenceFinalPuncts.Length);
            while(nLen-- > 0)
            {
                char chSrc = strSourceSentenceFinalPuncts[nLen];
                char chTgt = strTargetSentenceFinalPuncts[nLen];
                if ((CstrAdaptItPunct.IndexOf(chSrc) == -1)
                    || (CstrAdaptItPunct.IndexOf(chTgt) == -1))
                {
                    strSourcePunct += chSrc;
                    strTargetPunct += chTgt;
                }
            }

            strSourcePunct = CstrAdaptItPunct + strSourcePunct;
            strTargetPunct = CstrAdaptItPunct + strTargetPunct;
        }
    }
}
