using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ECInterfaces;                 // for IEncConverter
using SilEncConverters40;           // for AdaptItEncConverter

namespace OneStoryProjectEditor
{
    public class AdaptItGlossing
    {
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
                strName = AdaptItKBReader.AdaptItLookupConverterName(liSourceLang.LangName, liTargetLang.LangName);

            if (mapNameToTheEc == null)
                mapNameToTheEc = new Dictionary<string, AdaptItEncConverter>();

            string strConverterSpec = AdaptItKBReader.AdaptItLookupFileSpec(strProjectFolder, liSourceLang.LangName, liTargetLang.LangName);
            if ((adaptItConfiguration != null) 
                && (adaptItConfiguration.ProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject))
            {
                strProjectFolder = Path.GetDirectoryName(strConverterSpec);
                adaptItConfiguration.CheckForSync(strProjectFolder, loggedOnMember);

                // sometimes the sync doesn't also create the Adaptation folder
                AdaptItKBReader.EnsureAiFoldersExist(strProjectFolder, liSourceLang.LangName, liTargetLang.LangName);
            }
            else
            {
                // just in case the project doesn't exist yet and nothing's been 
                //  configured (i.e. else case)
                WriteAdaptItProjectFiles(liSourceLang, liTargetLang, proj.InternationalBT);
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

        private static AdaptItKBReader.LanguageInfo FromOseLanguageInfo(ProjectSettings.LanguageInfo li)
        {
            return new AdaptItKBReader.LanguageInfo
                       {
                           FontName = li.FontToUse.Name,
                           LangName = li.LangName,
                           Punctuation = AddSentenceFinalPunctuation(li.FullStop),
                           RightToLeft = (li.DoRtl) ? RightToLeft.Yes : RightToLeft.No
                       };
        }

        protected static void WriteAdaptItProjectFiles(ProjectSettings.LanguageInfo liSourceLang,
                                                       ProjectSettings.LanguageInfo liTargetLang, 
                                                       ProjectSettings.LanguageInfo liNavigation)
        {
            // call Ai assembly to do it (but have to convert the LangInfo types
            var aiLiSource = FromOseLanguageInfo(liSourceLang);
            var aiLiTarget = FromOseLanguageInfo(liTargetLang);
            var aiLiNavigation = FromOseLanguageInfo(liNavigation);

            // can't allow puncuation to be different lengths
            var nSourceLen = aiLiSource.Punctuation.Length;
            var nTargetLen = aiLiTarget.Punctuation.Length;
            if (nSourceLen != nTargetLen)
            {
                if (nSourceLen > nTargetLen)
                    aiLiTarget.Punctuation += aiLiSource.Punctuation.Substring(nTargetLen);
                else
                    aiLiSource.Punctuation += aiLiTarget.Punctuation.Substring(nSourceLen);
                System.Diagnostics.Debug.Assert(aiLiTarget.Punctuation.Length == aiLiSource.Punctuation.Length);
            }
            AdaptItKBReader.WriteAdaptItProjectFiles(aiLiSource, aiLiTarget, aiLiNavigation);
        }

        private static string AddSentenceFinalPunctuation(string strSentenceFinalPuncts)
        {
            var strMyPunct = strSentenceFinalPuncts
                                .Where(ch => AdaptItKBReader.CstrAdaptItPunct.IndexOf(ch) == -1)
                                .Aggregate<char, string>(null, (current, ch) => current + ch);
            return AdaptItKBReader.CstrAdaptItPunct + strMyPunct;
        }
    }
}
