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
                _strProjectFolder = Path.Combine(OneStoryProjectFolderRoot, ProjectName);
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
                Vernacular.LangFont = new Font(theVernRow.FontName, theVernRow.FontSize);
                
                // save what was in the actual file so we don't overwrite when the font isn't present
                if (Vernacular.LangFont.Name != theVernRow.FontName)
                    Vernacular.FontName = theVernRow.FontName;

                Vernacular.FontColor = Color.FromName(theVernRow.FontColor);
                Vernacular.FullStop = theVernRow.SentenceFinalPunct;
                Vernacular.IsRTL = (!theVernRow.IsRTLNull() && theVernRow.RTL);
                Vernacular.DefaultKeyboard = (!theVernRow.IsKeyboardNull() && !String.IsNullOrEmpty(theVernRow.Keyboard))
                    ? theVernRow.Keyboard : null;
            }

            // the national language BT isn't strictly necessary...
            //  so only initialize if there's one in the file (shouldn't be more than 1)
            if (projFile.NationalBTLang.Count == 1)
            {
                System.Diagnostics.Debug.Assert(projFile.NationalBTLang.Count == 1);
                NewDataSet.NationalBTLangRow rowNatlRow = projFile.NationalBTLang[0];
                NationalBT.LangName = rowNatlRow.name;
                NationalBT.LangCode = rowNatlRow.code;
                NationalBT.LangFont = new Font(rowNatlRow.FontName, rowNatlRow.FontSize);

                // save what was in the actual file so we don't overwrite when the font isn't present
                if (NationalBT.LangFont.Name != rowNatlRow.FontName)
                    NationalBT.FontName = rowNatlRow.FontName;

                NationalBT.FontColor = Color.FromName(rowNatlRow.FontColor);
                NationalBT.FullStop = rowNatlRow.SentenceFinalPunct;
                NationalBT.IsRTL = (!rowNatlRow.IsRTLNull() && rowNatlRow.RTL);
                NationalBT.DefaultKeyboard = (!rowNatlRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowNatlRow.Keyboard))
                    ? rowNatlRow.Keyboard : null;
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
                InternationalBT.LangFont = new Font(rowEngRow.FontName, rowEngRow.FontSize);

                // save what was in the actual file so we don't overwrite when the font isn't present
                if (InternationalBT.LangFont.Name != rowEngRow.FontName)
                    InternationalBT.FontName = rowEngRow.FontName;

                InternationalBT.FontColor = Color.FromName(rowEngRow.FontColor);
                InternationalBT.FullStop = rowEngRow.SentenceFinalPunct;
                InternationalBT.IsRTL = (!rowEngRow.IsRTLNull() && rowEngRow.RTL);
                InternationalBT.DefaultKeyboard = (!rowEngRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowEngRow.Keyboard))
                    ? rowEngRow.Keyboard : null;
            }

            // if we're setting this up from the file, then we're "configured"
            IsConfigured = true;
        }

        public class LanguageInfo
        {
            internal static string CstrSentenceFinalPunctuation = ".!?:";

            public string LangName = null;
            public string LangCode = null;
            public string FontName = null;
            public Font LangFont;
            public Color FontColor;
            public string FullStop = CstrSentenceFinalPunctuation;
            public string DefaultKeyboard = null;
            public bool IsRTL = false;

            public LanguageInfo(Font font, Color fontColor)
            {
                LangFont = font;
                FontColor = fontColor;
            }

            public LanguageInfo(string strLangName, string strLangCode, Font font, Color fontColor)
            {
                LangName = strLangName;
                LangCode = strLangCode;
                LangFont = font;
                FontColor = fontColor;
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
                // if the font wasn't present (as evidenced by a non-null value for FontName), then
                //  don't overwrite the xml file's value
                string strFontName = (String.IsNullOrEmpty(FontName)) ? LangFont.Name : FontName;
                XElement elemLang =
                    new XElement(strLangType,
                        new XAttribute("name", LangName),
                        new XAttribute("code", LangCode),
                        new XAttribute("FontName", strFontName),
                        new XAttribute("FontSize", LangFont.Size),
                        new XAttribute("FontColor", FontColor.Name));

                if (!String.IsNullOrEmpty(FullStop))
                    elemLang.Add(new XAttribute("SentenceFinalPunct", FullStop));

                if (IsRTL)
                    elemLang.Add(new XAttribute("RTL", IsRTL));

                if (!String.IsNullOrEmpty(DefaultKeyboard))
                    elemLang.Add(new XAttribute("Keyboard", DefaultKeyboard));

                return elemLang;
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

        public static string GetDefaultProjectFilePath(string strProjectName)
        {
            return Path.Combine(OneStoryProjectFolderRoot, 
                Path.Combine(strProjectName, OneStoryFileName(strProjectName)));
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
                // have to have one or the other BT language
                System.Diagnostics.Debug.Assert(NationalBT.HasData || InternationalBT.HasData || (Vernacular.HasData && (Vernacular.LangName == "English")));

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
    }
}
