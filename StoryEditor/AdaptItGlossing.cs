using System;
using System.Collections.Generic;
using System.IO;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public class AdaptItGlossing
    {
        /// <summary>
        /// returns something like 
        /// </summary>
        /// <param name="strProjectFolderName"></param>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        protected static string AdaptItGlossingLookupFileSpec(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                "Glossing.xml");
        }

        protected static string AdaptItProjectFolderName(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return String.IsNullOrEmpty(strProjectFolderName)
                       ? String.Format(@"{0} to {1} adaptations", strSourceLangName, strTargetLangName)
                       : strProjectFolderName;
        }

        protected static string AdaptationFileName(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}.xml",
                                 AdaptItProjectFolderName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        public static string AdaptItLookupFileSpec(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                AdaptationFileName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItProjectFileSpec(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                "AI-ProjectConfiguration.aic");
        }

        /// <summary>
        /// returns something like: <My Documents>\Adapt It Unicode Work
        /// which is the root folder in the user's my documents folder for all adapt it projects
        /// </summary>
        public static string AdaptItWorkFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Adapt It Unicode Work");
            }
        }

        /// <summary>
        /// returns something like: <My Documents>\Adapt It Unicode Work\Bundelkhandi to English adaptations
        /// which is the project folder for the Adapt It project
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItProjectFolder(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItWorkFolder,
                                AdaptItProjectFolderName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        /// <summary>
        /// returns something like, <My Documents>\Adapt It Unicode Work\Bundelkhandi to English adaptations\Adaptations
        /// which contains the adaptations done in Adapt It (only used if the user has called to AI to
        /// do glossing; not in the OSE glossing tool)
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItProjectAdaptationsFolder(string strProjectFolderName, 
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                "Adaptations");
        }

        /// <summary>
        /// returns something like: "Lookup in {0} to {1} adaptations", which is the EncConverter friendly name for the project
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations",
                strSourceLangName, strTargetLangName);
        }

        private static Dictionary<string, AdaptItEncConverter> mapNameToTheEc;

        public static AdaptItEncConverter InitLookupAdapter(ProjectSettings proj, 
            ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection,
            TeamMemberData loggedOnMember, out ProjectSettings.LanguageInfo liSourceLang, 
            out ProjectSettings.LanguageInfo liTargetLang)
        {
            string strName = null, strProjectFolder = null;
            ProjectSettings.AdaptItConfiguration adaptItConfiguration = null;
            switch (eBtDirection)
            {
                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt:
                    if ((proj.VernacularToNationalBt != null) && !String.IsNullOrEmpty(proj.VernacularToNationalBt.ConverterName))
                    {
                        strName = proj.VernacularToNationalBt.ConverterName;
                        strProjectFolder = proj.VernacularToNationalBt.ProjectFolderName;
                        adaptItConfiguration = proj.VernacularToNationalBt;
                    }
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.NationalBT;
                    break;

                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt:    // the glossing KB for the Vern to Natl project
                    if ((proj.VernacularToInternationalBt != null) && !String.IsNullOrEmpty(proj.VernacularToInternationalBt.ConverterName))
                    {
                        strName = proj.VernacularToInternationalBt.ConverterName;
                        strProjectFolder = proj.VernacularToInternationalBt.ProjectFolderName;
                        adaptItConfiguration = proj.VernacularToInternationalBt;
                    }
                    liSourceLang = proj.Vernacular;
                    liTargetLang = proj.InternationalBT; // this is still the national lg project (but the glossing KB)
                    break;

                case ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt:
                    if ((proj.NationalBtToInternationalBt != null) && !String.IsNullOrEmpty(proj.NationalBtToInternationalBt.ConverterName))
                    {
                        strName = proj.NationalBtToInternationalBt.ConverterName;
                        strProjectFolder = proj.NationalBtToInternationalBt.ProjectFolderName;
                        adaptItConfiguration = proj.NationalBtToInternationalBt;
                    }
                    liSourceLang = proj.NationalBT;         // this is a whole nuther national to English project
                    liTargetLang = proj.InternationalBT;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new ApplicationException("Wrong glossing type specified. Send to bob_eaton@sall.com for help");
            }

            // if there wasn't one configured, then just use the default
            if (String.IsNullOrEmpty(strName))
                strName = AdaptItLookupConverterName(liSourceLang.LangName, liTargetLang.LangName);

            if (mapNameToTheEc == null)
                mapNameToTheEc = new Dictionary<string, AdaptItEncConverter>();

            string strConverterSpec = AdaptItLookupFileSpec(strProjectFolder, liSourceLang.LangName, liTargetLang.LangName);
            if ((adaptItConfiguration != null) 
                && (adaptItConfiguration.ProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject))
            {
                strProjectFolder = Path.GetDirectoryName(strConverterSpec);
                adaptItConfiguration.CheckForSync(strProjectFolder, loggedOnMember);
            }
            else
            {
                // just in case the project doesn't exist yet and nothing's been 
                //  configured (i.e. else case)
                strProjectFolder = AdaptItProjectFolderName(strProjectFolder, liSourceLang.LangName,
                                                            liTargetLang.LangName);
                WriteAdaptItProjectFiles(strProjectFolder, liSourceLang, liTargetLang, proj.InternationalBT); // move this to AIGuesserEC project when it's mature.
            }

            AdaptItEncConverter theEc;
            if (!mapNameToTheEc.TryGetValue(strName, out theEc))
            {
                // if we don't have the converter already in the repository.
                var aECs = new EncConverters();
                if (!aECs.ContainsKey(strName) && File.Exists(strConverterSpec))
                {
                    aECs.AddConversionMap(strName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                        EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);
                }

                if (!aECs.ContainsKey(strName))
                    throw new ApplicationException(String.Format(Properties.Resources.IDS_AdaptItConverterDoesntExist,
                                                                 strName));

                IEncConverter aEC = aECs[strName];
                System.Diagnostics.Debug.Assert((aEC != null) && (aEC is AdaptItEncConverter));
                theEc = (AdaptItEncConverter)aEC;

                // in order to get the converter to load the database, do a dummy Convert
                theEc.Convert("nothing");
                mapNameToTheEc.Add(strName, theEc);
            }

            return theEc;
        }

        public static string GetAiProjectFolderFromConverterIdentifier(string strConverterIdentifier)
        {
            // the converter identifier can now have two parts separated by a ';'
            int nIndex = strConverterIdentifier.IndexOf(';');
            if (nIndex != -1)
                strConverterIdentifier = strConverterIdentifier.Substring(0, nIndex);
            return Path.GetDirectoryName(strConverterIdentifier);
        }

        public static string GetAiProjectFolderNameFromConverterIdentifier(string strConverterIdentifier)
        {
            return Path.GetFileNameWithoutExtension(
                GetAiProjectFolderFromConverterIdentifier(strConverterIdentifier));
        }

        protected static void WriteAdaptItProjectFiles(string strProjectFolderName, 
            ProjectSettings.LanguageInfo liSourceLang,
            ProjectSettings.LanguageInfo liTargetLang, 
            ProjectSettings.LanguageInfo liNavigation)
        {
            // create folders...
            if (!Directory.Exists(AdaptItProjectAdaptationsFolder(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName)))
                Directory.CreateDirectory(AdaptItProjectAdaptationsFolder(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName));

            // create Project file
            if (!File.Exists(AdaptItProjectFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName)))
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
                File.WriteAllText(AdaptItProjectFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName), strProjectFileContents);
            }

            // create main KB
            if (!File.Exists(AdaptItLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSourceLang.LangName, liTargetLang.LangName);
                File.WriteAllText(AdaptItLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName), strKBContents);
            }

            if (!File.Exists(AdaptItGlossingLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName)))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKBContents = String.Format(strFormat, liSourceLang.LangName, liTargetLang.LangName);
                File.WriteAllText(AdaptItGlossingLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName), strKBContents);
            }
        }

        const string CstrAdaptItPunct = "?.,;:\"'!()<>{}[]“”‘’…-";
            
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
