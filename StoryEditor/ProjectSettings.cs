using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;                  // for RegistryKey

namespace OneStoryProjectEditor
{
    public class ProjectSettings
    {
        public string ProjectName = null;
        protected string _strProjectFolder = null;

        // default is to have all 3, but the user might disable one or the other bt languages
        public LanguageInfo Vernacular = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Maroon);
        public LanguageInfo NationalBT = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Green);
        public LanguageInfo InternationalBT = new LanguageInfo("English", "en", new Font("Times New Roman", 10), Color.Blue);

        public bool IsConfigured;
        public string HgRepoUrl = null;

        public ProjectSettings(string strProjectFolderDefaultIfNull, string strProjectName)
        {
            ProjectName = strProjectName;
            if (String.IsNullOrEmpty(strProjectFolderDefaultIfNull))
                _strProjectFolder = GetDefaultProjectPath(ProjectName);
            else
            {
                System.Diagnostics.Debug.Assert(strProjectFolderDefaultIfNull[strProjectFolderDefaultIfNull.Length-1] != '\\');
                _strProjectFolder = strProjectFolderDefaultIfNull;
            }
        }

        public void SerializeProjectSettings(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.StoryProject[0].ProjectName == ProjectName));

            NewDataSet.LanguagesRow theLangRow = InsureLanguagesRow(projFile);

            // if there is no vernacular row, we must add it (it's required)
            if (projFile.VernacularLang.Count == 1)
            {
                // otherwise, read in the details
                System.Diagnostics.Debug.Assert(projFile.VernacularLang.Count == 1);
                NewDataSet.VernacularLangRow theVernRow = projFile.VernacularLang[0];
                Vernacular.LangName = theVernRow.name;
                Vernacular.LangCode = theVernRow.code;
                Vernacular.DefaultFontName = theVernRow.FontName;
                Vernacular.DefaultFontSize = theVernRow.FontSize;
                Vernacular.FontToUse = new Font(theVernRow.FontName, theVernRow.FontSize);
                Vernacular.FontColor = Color.FromName(theVernRow.FontColor);
                Vernacular.FullStop = theVernRow.SentenceFinalPunct;
                Vernacular.DefaultRtl = (!theVernRow.IsRTLNull() && theVernRow.RTL);
                Vernacular.DefaultKeyboard = 
                    (!theVernRow.IsKeyboardNull() && !String.IsNullOrEmpty(theVernRow.Keyboard))
                        ? theVernRow.Keyboard
                        : null;
            }

            // the national language BT isn't strictly necessary...
            //  so only initialize if there's one in the file (shouldn't be more than 1)
            if (projFile.NationalBTLang.Count == 1)
            {
                System.Diagnostics.Debug.Assert(projFile.NationalBTLang.Count == 1);
                NewDataSet.NationalBTLangRow rowNatlRow = projFile.NationalBTLang[0];
                NationalBT.LangName = rowNatlRow.name;
                NationalBT.LangCode = rowNatlRow.code;
                NationalBT.DefaultFontName = rowNatlRow.FontName;
                NationalBT.DefaultFontSize = rowNatlRow.FontSize;
                NationalBT.FontToUse = new Font(rowNatlRow.FontName, rowNatlRow.FontSize);
                NationalBT.FontColor = Color.FromName(rowNatlRow.FontColor);
                NationalBT.FullStop = rowNatlRow.SentenceFinalPunct;
                NationalBT.DefaultRtl = (!rowNatlRow.IsRTLNull() && rowNatlRow.RTL);
                NationalBT.DefaultKeyboard =
                    (!rowNatlRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowNatlRow.Keyboard))
                        ? rowNatlRow.Keyboard
                        : null;
            }

            // the international language BT isn't strictly necessary... (e.g. if they're only doing
            //  national lang BTs) (but have to have one or the other; neither is not acceptable)
            //  so only initialize if there's one in the file (shouldn't be more than 1)
            if (projFile.InternationalBTLang.Count == 1)
            {
                System.Diagnostics.Debug.Assert(projFile.InternationalBTLang.Count == 1);
                NewDataSet.InternationalBTLangRow rowEngRow = projFile.InternationalBTLang[0];
                InternationalBT.LangName = rowEngRow.name;
                InternationalBT.LangCode = rowEngRow.code;
                InternationalBT.DefaultFontName = rowEngRow.FontName;
                InternationalBT.DefaultFontSize = rowEngRow.FontSize;
                InternationalBT.FontToUse = new Font(rowEngRow.FontName, rowEngRow.FontSize);
                InternationalBT.FontColor = Color.FromName(rowEngRow.FontColor);
                InternationalBT.FullStop = rowEngRow.SentenceFinalPunct;
                InternationalBT.DefaultRtl = (!rowEngRow.IsRTLNull() && rowEngRow.RTL);
                InternationalBT.DefaultKeyboard =
                    (!rowEngRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowEngRow.Keyboard))
                        ? rowEngRow.Keyboard
                        : null;
            }
            else
            {
                // the "international language" will appear to "have data" even when it shouldn't
                //  so clear out the default language name in this case:
                InternationalBT.LangName = null;
                System.Diagnostics.Debug.Assert(!InternationalBT.HasData);
            }

            // if we're setting this up from the file, then we're "configured"
            IsConfigured = true;
        }

        public class LanguageInfo
        {
            internal static string CstrSentenceFinalPunctuation = ".!?:";

            public string LangName;
            public string LangCode;
            public string DefaultFontName;
            public float DefaultFontSize;
            public Font FontToUse;
            public Color FontColor;
            public string FullStop = CstrSentenceFinalPunctuation;
            public string DefaultKeyboard;
            public string KeyboardOverride;
            public bool DefaultRtl; // this is the value that most of the team uses
            public bool InvertRtl;  // this indicates whether the default value should
                                    // be overridden (which means toggle) for a particular
                                    // user.

            public LanguageInfo(Font font, Color fontColor)
            {
                FontToUse = font;
                DefaultFontName = font.Name;
                DefaultFontSize = font.Size;
                FontColor = fontColor;
            }

            public LanguageInfo(string strLangName, string strLangCode, Font font, Color fontColor)
            {
                LangName = strLangName;
                LangCode = strLangCode;
                FontToUse = font;
                DefaultFontName = font.Name;
                DefaultFontSize = font.Size;
                FontColor = fontColor;
            }

            public string Keyboard
            {
                get
                {
                    return (String.IsNullOrEmpty(KeyboardOverride)) ? DefaultKeyboard : KeyboardOverride;
                }
            }

            public bool DoRtl
            {
                // we want to 'do RTL' if a) we're supposed to invert the default
                //  RTL flag (what most users are using) and the default is false OR 
                //  b) we're not supposed to invert (which means override) and the 
                //  default is true
                get { return ((InvertRtl && !DefaultRtl) || (!InvertRtl && DefaultRtl)); }
            }

            public bool HasData
            {
                get { return !String.IsNullOrEmpty(LangName); }
                set
                {
                    if (!value)
                        LangName = null;
                    else
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(LangName));
                }
            }

            public XElement GetXml(string strLangType)
            {
                XElement elemLang =
                    new XElement(strLangType,
                        new XAttribute("name", LangName),
                        new XAttribute("code", LangCode),
                        new XAttribute("FontName", DefaultFontName),
                        new XAttribute("FontSize", DefaultFontSize),
                        new XAttribute("FontColor", FontColor.Name));

                if (!String.IsNullOrEmpty(FullStop))
                    elemLang.Add(new XAttribute("SentenceFinalPunct", FullStop));

                // when saving, though, we only write out the default value (override
                //  values (if any) are saved by the member ID info)
                if (DefaultRtl)
                    elemLang.Add(new XAttribute("RTL", DefaultRtl));

                if (!String.IsNullOrEmpty(DefaultKeyboard))
                    elemLang.Add(new XAttribute("Keyboard", DefaultKeyboard));

                return elemLang;
            }

            public string HtmlStyle(string strLangCat)
            {
                string strHtmlStyle = String.Format(Properties.Resources.HTML_LangStyle,
                    strLangCat,
                    FontToUse.Name,
                    FontToUse.SizeInPoints,
                    VerseData.HtmlColor(FontColor),
                    (DoRtl) ? "rtl" : "ltr",
                    (DoRtl) ? "right" : "left");

                return strHtmlStyle;
            }
        }

        internal class ProjectFileNotFoundException : ApplicationException
        {
            internal ProjectFileNotFoundException(string strMessage)
                : base(strMessage)
            {
            }
        }

        public void ThrowIfProjectFileDoesntExists()
        {
            if (!File.Exists(ProjectFilePath))
                throw new ProjectFileNotFoundException(String.Format("Unable to find the file: '{0}'", ProjectFilePath));
        }

        public static string OneStoryFileName(string strProjectName)
        {
            return String.Format(@"{0}.onestory", strProjectName);
        }

        public string ProjectFilePath
        {
            get { return Path.Combine(ProjectFolder, OneStoryFileName(ProjectName)); }
        }

        public static string GetDefaultProjectPath(string strProjectName)
        {
            return Path.Combine(OneStoryProjectFolderRoot, strProjectName);
        }

        public static string GetDefaultProjectFilePath(string strProjectName)
        {
            return Path.Combine(GetDefaultProjectPath(strProjectName), 
                OneStoryFileName(strProjectName));
        }

        public string ProjectFolder
        {
            get { return _strProjectFolder; }
        }

        public static void InsureOneStoryProjectFolderRootExists()
        {
            // one of the first things this might do is try to get a project from the internet, in which case
            //  the OneStory folder should exist
            if (!Directory.Exists(OneStoryProjectFolderRoot))
                Directory.CreateDirectory(OneStoryProjectFolderRoot);
        }

        // if any of this changes, update FixupOneStoryFile::Program.cs
        protected const string OneStoryHiveRoot = @"Software\SIL\OneStory";
        protected const string CstrRootDirKey = "RootDir";

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                string strDefaultProjectFolderRoot = null;
                RegistryKey keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    strDefaultProjectFolderRoot = (string)keyOneStoryHiveRoot.GetValue(CstrRootDirKey);

                if (String.IsNullOrEmpty(strDefaultProjectFolderRoot))
                    strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string strPath = Path.Combine(strDefaultProjectFolderRoot,
                    Properties.Settings.Default.DefMyDocsSubfolder);

                return strPath;
            }
            set
            {
                RegistryKey keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot, true);
                if (keyOneStoryHiveRoot == null)
                    keyOneStoryHiveRoot = Registry.CurrentUser.CreateSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    keyOneStoryHiveRoot.SetValue(CstrRootDirKey, value);
            }
        }

        public XElement GetXml
        {
            get
            {
                // have to have one or the other languages
                System.Diagnostics.Debug.Assert(Vernacular.HasData || NationalBT.HasData || InternationalBT.HasData);

                XElement elem = new XElement("Languages");
                if (Vernacular.HasData)
                    elem.Add(Vernacular.GetXml("VernacularLang"));
                    
                if (NationalBT.HasData)
                    elem.Add(NationalBT.GetXml("NationalBTLang"));

                if (InternationalBT.HasData)
                    elem.Add(InternationalBT.GetXml("InternationalBTLang"));

                return elem;
            }
        }

        protected NewDataSet.LanguagesRow InsureLanguagesRow(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert(projFile.StoryProject.Count == 1);
            if (projFile.Languages.Count == 0)
                return projFile.Languages.AddLanguagesRow(projFile.StoryProject[0]);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.Languages.Count == 1);
                return projFile.Languages[0];
            }
        }

        public void InitializeOverrides(TeamMemberData loggedOnMember)
        {
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideFontNameVernacular))
                Vernacular.FontToUse =
                    new Font(loggedOnMember.OverrideFontNameVernacular, loggedOnMember.OverrideFontSizeVernacular);
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideFontNameNationalBT))
                NationalBT.FontToUse =
                    new Font(loggedOnMember.OverrideFontNameNationalBT, loggedOnMember.OverrideFontSizeNationalBT);
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideFontNameInternationalBT))
                InternationalBT.FontToUse =
                    new Font(loggedOnMember.OverrideFontNameInternationalBT, loggedOnMember.OverrideFontSizeInternationalBT);
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideVernacularKeyboard))
                Vernacular.KeyboardOverride = loggedOnMember.OverrideVernacularKeyboard;
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideNationalBTKeyboard))
                NationalBT.KeyboardOverride = loggedOnMember.OverrideNationalBTKeyboard;
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideInternationalBTKeyboard))
                InternationalBT.KeyboardOverride = loggedOnMember.OverrideInternationalBTKeyboard;
            Vernacular.InvertRtl = loggedOnMember.OverrideRtlVernacular;
            NationalBT.InvertRtl = loggedOnMember.OverrideRtlNationalBT;
            InternationalBT.InvertRtl = loggedOnMember.OverrideRtlInternationalBT;
        }
    }
}
