using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Chorus.UI.Clone;
using Microsoft.Win32;
using NetLoc;
using SilEncConverters40;

// for RegistryKey

namespace OneStoryProjectEditor
{
    public class ProjectSettings
    {
        public string ProjectName;
        protected string _strProjectFolder;

        public bool UseDropbox;
        public bool DropboxStory;
        public bool DropboxRetelling;
        public bool DropboxAnswers;

        // default is to have all 3, but the user might disable one or the other bt languages
        public LanguageInfo Vernacular = new LanguageInfo(LineData.CstrAttributeLangVernacular, new Font("Arial Unicode MS", 12), Color.Maroon);
        public LanguageInfo NationalBT = new LanguageInfo(LineData.CstrAttributeLangNationalBt, new Font("Arial Unicode MS", 12), Color.Green);
        public LanguageInfo InternationalBT = new LanguageInfo(LineData.CstrAttributeLangInternationalBt, DefInternationalLanguageName, "en", new Font("Times New Roman", 10), Color.Blue);
        public LanguageInfo FreeTranslation = new LanguageInfo(LineData.CstrAttributeLangFreeTranslation, DefInternationalLanguageName, "en", new Font("Times New Roman", 10), Color.ForestGreen);
        public LanguageInfo Localization = new LanguageInfo(LineData.CstrAttributeLangLocalization, new Font("Microsoft Sans Serif", 9), Color.Blue);

        public static string DefInternationalLanguageName
        {
            get { return Localizer.Str("English"); }
        }

        public AdaptItConfiguration VernacularToNationalBt;
        public AdaptItConfiguration VernacularToInternationalBt;
        public AdaptItConfiguration NationalBtToInternationalBt;

        public bool IsConfigured;
        public ShowLanguageFields ShowRetellings = new ShowLanguageFields();
        public ShowLanguageFields ShowTestQuestions = new ShowLanguageFields();
        public ShowLanguageFields ShowAnswers = new ShowLanguageFields();

        public ProjectSettings(string strProjectFolderDefaultIfNull, string strProjectName)
        {
            ProjectName = strProjectName;
            if (String.IsNullOrEmpty(strProjectFolderDefaultIfNull))
                _strProjectFolder = GetDefaultProjectPath(ProjectName);
            else
            {
                Debug.Assert(strProjectFolderDefaultIfNull[strProjectFolderDefaultIfNull.Length - 1] != '\\');
                _strProjectFolder = strProjectFolderDefaultIfNull;
            }
        }

        // lenient reader for a project element that came from somewhere else (e.g., Chorus or the story copied
        //  from another project): anything missing is simply left at its default
        public ProjectSettings(XElement elemStoryProject, string strProjectFolder)
        {
            ProjectName = (string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeProjectName);

            _strProjectFolder = strProjectFolder;

            UseDropbox = (string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeUseDropbox) == "true";
            DropboxStory = (string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeDropboxStory) == "true";
            DropboxRetelling = (string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeDropboxRetellings) == "true";
            DropboxAnswers = (string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeDropboxAnswers) == "true";

            Vernacular = new LanguageInfo(FindLanguageInfo(elemStoryProject, LineData.CstrAttributeLangVernacular));
            NationalBT = new LanguageInfo(FindLanguageInfo(elemStoryProject, LineData.CstrAttributeLangNationalBt));
            InternationalBT = new LanguageInfo(FindLanguageInfo(elemStoryProject, LineData.CstrAttributeLangInternationalBt));
            FreeTranslation = new LanguageInfo(FindLanguageInfo(elemStoryProject, LineData.CstrAttributeLangFreeTranslation));
        }

        private static XElement FindLanguageInfo(XElement elemStoryProject, string strLangType)
        {
            return elemStoryProject.Elements(CstrElementLabelLanguages)
                                   .Elements(LanguageInfo.CstrElementLabelLanguageInfo)
                                   .FirstOrDefault(e => (string)e.Attribute(LanguageInfo.CstrAttributeLang) == strLangType);
        }

        // elemStoryProject is the root element. An absent <Languages> leaves the current Show* values as they are.
        public void SerializeProjectSettings(XElement elemStoryProject)
        {
            Debug.Assert((elemStoryProject != null) &&
                         ((string)elemStoryProject.Attribute(StoryProjectData.CstrAttributeProjectName) == ProjectName));

            var elemLanguages = XmlRead.First(elemStoryProject, CstrElementLabelLanguages);
            if (elemLanguages != null)
            {
                bool? bValue;
                if ((bValue = XmlRead.Bool(elemLanguages, "UseRetellingVernacular")).HasValue)
                    ShowRetellings.Vernacular = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseRetellingNationalBT")).HasValue)
                    ShowRetellings.NationalBt = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseRetellingInternationalBT")).HasValue)
                    ShowRetellings.InternationalBt = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseTestQuestionVernacular")).HasValue)
                    ShowTestQuestions.Vernacular = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseTestQuestionNationalBT")).HasValue)
                    ShowTestQuestions.NationalBt = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseTestQuestionInternationalBT")).HasValue)
                    ShowTestQuestions.InternationalBt = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseAnswerVernacular")).HasValue)
                    ShowAnswers.Vernacular = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseAnswerNationalBT")).HasValue)
                    ShowAnswers.NationalBt = bValue.Value;

                if ((bValue = XmlRead.Bool(elemLanguages, "UseAnswerInternationalBT")).HasValue)
                    ShowAnswers.InternationalBt = bValue.Value;
            }

            // the configurations are only read when there is exactly one <AdaptItConfigurations>
            var elemsAiConfigurations = XmlRead.Children(elemStoryProject, "AdaptItConfigurations").ToList();
            if (elemsAiConfigurations.Count == 1)
            {
                foreach (var elemAiConfig in XmlRead.Children(elemsAiConfigurations[0], CstrElementLabelAdaptItConfiguration))
                {
                    var strBtDirection = XmlRead.RequiredAttr(elemAiConfig, CstrAttributeLabelBtDirection);
                    if (strBtDirection == AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt.ToString())
                    {
                        VernacularToNationalBt = new AdaptItConfiguration();
                        VernacularToNationalBt.SerializeFromProjectFile(elemAiConfig);
                    }
                    if (strBtDirection == AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt.ToString())
                    {
                        VernacularToInternationalBt = new AdaptItConfiguration();
                        VernacularToInternationalBt.SerializeFromProjectFile(elemAiConfig);
                    }
                    if (strBtDirection == AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt.ToString())
                    {
                        NationalBtToInternationalBt = new AdaptItConfiguration();
                        NationalBtToInternationalBt.SerializeFromProjectFile(elemAiConfig);
                    }
                }
            }

            bool bFoundInternationalBt = false, bFoundFreeTranslation = false;
            if (elemLanguages != null)
            {
                foreach (var elemLang in XmlRead.Children(elemLanguages, LanguageInfo.CstrElementLabelLanguageInfo))
                {
                    var strLang = XmlRead.RequiredAttr(elemLang, LanguageInfo.CstrAttributeLang);
                    if (strLang == LineData.CstrAttributeLangVernacular)
                        Vernacular.Serialize(elemLang);
                    if (strLang == LineData.CstrAttributeLangNationalBt)
                        NationalBT.Serialize(elemLang);
                    if (strLang == LineData.CstrAttributeLangInternationalBt)
                    {
                        bFoundInternationalBt = true;
                        InternationalBT.Serialize(elemLang);
                    }
                    if (strLang == LineData.CstrAttributeLangFreeTranslation)
                    {
                        bFoundFreeTranslation = true;
                        FreeTranslation.Serialize(elemLang);
                    }
                }
            }

            // the "international language" will appear to "have data" even when it shouldn't
            //  so clear out the default language name in this case:
            if (!bFoundInternationalBt)
            {
                InternationalBT.LangName = null;
                Debug.Assert(!InternationalBT.HasData);
            }

            // the "international language" will appear to "have data" even when it shouldn't
            //  so clear out the default language name in this case:
            if (!bFoundFreeTranslation)
            {
                FreeTranslation.LangName = null;
                Debug.Assert(!FreeTranslation.HasData);
            }

            // if we're setting this up from the file, then we're "configured"
            IsConfigured = true;
        }

        public const string CstrAttributeLabelProjectType = "ProjectType";
        public const string CstrAttributeLabelBtDirection = "BtDirection";
        public const string CstrAttributeLabelConverterName = "ConverterName";
        public const string CstrAttributeLabelProjectFolderName = "ProjectFolderName";
        public const string CstrAttributeLabelRepoProjectName = "RepoProjectName";
        public const string CstrAttributeLabelRepositoryServer = "RepositoryServer";
        public const string CstrAttributeLabelNetworkRepositoryPath = "NetworkRepositoryPath";

        public class AdaptItConfiguration
        {
            public enum AdaptItProjectType
            {
                None,
                LocalAiProjectOnly,
                SharedAiProject
            }

            public enum AdaptItBtDirection
            {
                VernacularToNationalBt,
                VernacularToInternationalBt,
                NationalBtToInternationalBt
            }

            public void SerializeFromProjectFile(XElement elemAdaptItConfiguration)
            {
                ProjectType = (AdaptItProjectType)Enum.Parse(typeof(AdaptItProjectType),
                    XmlRead.RequiredAttr(elemAdaptItConfiguration, CstrAttributeLabelProjectType));
                BtDirection = (AdaptItBtDirection)Enum.Parse(typeof(AdaptItBtDirection),
                    XmlRead.RequiredAttr(elemAdaptItConfiguration, CstrAttributeLabelBtDirection));
                ConverterName = XmlRead.RequiredAttr(elemAdaptItConfiguration, CstrAttributeLabelConverterName);

                var str = XmlRead.Attr(elemAdaptItConfiguration, CstrAttributeLabelProjectFolderName);
                if (str != null)
                    ProjectFolderName = str;

                str = XmlRead.Attr(elemAdaptItConfiguration, CstrAttributeLabelRepoProjectName);
                if (str != null)
                    RepoProjectName = str;

                str = XmlRead.Attr(elemAdaptItConfiguration, CstrAttributeLabelRepositoryServer);
                if (str != null)
                    RepositoryServer = str;

                str = XmlRead.Attr(elemAdaptItConfiguration, CstrAttributeLabelNetworkRepositoryPath);
                if (str != null)
                    NetworkRepositoryPath = str;
            }

            public AdaptItProjectType ProjectType { get; set; }
            public AdaptItBtDirection BtDirection { get; set; }
            public string ConverterName { get; set; }
            public string ProjectFolderName { get; set; }
            public string RepoProjectName { get; set; }
            public string RepositoryServer { get; set; }
            public string NetworkRepositoryPath { get; set; }
            public bool HasData
            {
                get { return (ProjectType != AdaptItProjectType.None); }
            }

            public XElement GetXml
            {
                get
                {
                    Debug.Assert(!String.IsNullOrEmpty(ConverterName));
                    var elem = new XElement(CstrElementLabelAdaptItConfiguration,
                                            new XAttribute(CstrAttributeLabelBtDirection, BtDirection.ToString()),
                                            new XAttribute(CstrAttributeLabelProjectType, ProjectType.ToString()),
                                            new XAttribute(CstrAttributeLabelConverterName, ConverterName));

                    if (!String.IsNullOrEmpty(ProjectFolderName))
                        elem.Add(new XAttribute(CstrAttributeLabelProjectFolderName, ProjectFolderName));

                    if (!String.IsNullOrEmpty(RepoProjectName))
                        elem.Add(new XAttribute(CstrAttributeLabelRepoProjectName, RepoProjectName));

                    if (!String.IsNullOrEmpty(RepositoryServer))
                        elem.Add(new XAttribute(CstrAttributeLabelRepositoryServer, RepositoryServer));

                    if (!String.IsNullOrEmpty(NetworkRepositoryPath))
                        elem.Add(new XAttribute(CstrAttributeLabelNetworkRepositoryPath, NetworkRepositoryPath));

                    return elem;
                }
            }

            public bool AlreadyCheckedForSync;
            public void CheckForSync(string strProjectFolder, TeamMemberData loggedOnMember)
            {
                Debug.Assert(ProjectType == AdaptItProjectType.SharedAiProject);
                if (!AlreadyCheckedForSync
                    && !String.IsNullOrEmpty(strProjectFolder)
                    && !String.IsNullOrEmpty(RepoProjectName))
                {
                    // if the folder doesn't exist or the repo doesn't exist...
                    if (!Directory.Exists(strProjectFolder) ||
                        !Directory.Exists(Program.PathToHgRepoFolder(strProjectFolder)))
                    {
                        // offer to clone it
                        if (LocalizableMessageBox.Show(Localizer.Str("The shared Adapt It project for this field is not on the local computer. Please enter the necessary information in the next window to download it from the internet (i.e. username, password, etc). These should be in an email message you received previously or contact your consultant."),
                                            StoryEditor.OseCaption,
                                            MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return;

                        if (!DoPossiblePull(strProjectFolder, loggedOnMember))
                            return;
                    }
                    else
                        Program.SyncWithAiRepository(strProjectFolder, RepoProjectName, true, true);

                    Program.SetAiProjectForSyncage(strProjectFolder, RepoProjectName);
                    AlreadyCheckedForSync = true;
                }
            }

            public bool DoPossiblePull(string strProjectFolder, TeamMemberData loggedOnMember)
            {
                string strHgUsername = null, strHgPassword = null;
                loggedOnMember?.GetHgParameters(out strHgUsername, out strHgPassword);

                // the GetClone dialog is expecting that the parent folder exist (e.g.
                //  C:\Documents and Settings\Bob\My Documents\Adapt It Unicode Work)
                string strAiWorkFolder = Path.GetDirectoryName(strProjectFolder);
                string strAiProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                var url = RepositoryServer;
                if (String.IsNullOrEmpty(url))
                    url = Properties.Resources.IDS_DefaultRepoUrl;

                Program.CloneRepository(projectName: RepoProjectName, parentDirToPutCloneIn: strAiWorkFolder,
                                        localFolder: strAiProjectFolderName,
                                        username: strHgUsername,
                                        password: strHgPassword,
                                        customUrl: url);

                return true;
            }
        }

        public class LanguageInfo
        {
            internal static string CstrSentenceFinalPunctuation = ".!?:";

            public string LangType; // oneof: Vernacular, NationalBt, InternationalBt, or FreeTranslation
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

            public LanguageInfo(string strLangType, Font font, Color fontColor)
            {
                LangType = strLangType;
                FontToUse = font;
                DefaultFontName = font.Name;
                DefaultFontSize = font.Size;
                FontColor = fontColor;
            }

            // lenient reader (see ProjectSettings(XElement, string)); a null element gives an empty LanguageInfo
            public LanguageInfo(XElement elemLanguageInfo)
            {
                if (elemLanguageInfo == null)
                    return;

                LangType = (string)elemLanguageInfo.Attribute(CstrAttributeLang);
                LangName = (string)elemLanguageInfo.Attribute(CstrAttributeName);
                LangCode = (string)elemLanguageInfo.Attribute(CstrAttributeCode);
                // a missing FontName gets the same default font the project settings start with (Font can't take null)
                DefaultFontName = (string)elemLanguageInfo.Attribute(CstrAttributeFontName) ?? "Arial Unicode MS";
                DefaultFontSize = XmlRead.Float(elemLanguageInfo, CstrAttributeFontSize) ?? 12;
                FontToUse = new Font(DefaultFontName, DefaultFontSize);
                var strFontColor = (string)elemLanguageInfo.Attribute(CstrAttributeFontColor);
                FontColor = (strFontColor != null) ? Color.FromName(strFontColor) : Color.Black;
                FullStop = (string)elemLanguageInfo.Attribute(CstrAttributeSentenceFinalPunct);
                DefaultKeyboard = (string)elemLanguageInfo.Attribute(CstrAttributeKeyboard);
                DefaultRtl = (string)elemLanguageInfo.Attribute(CstrAttributeRTL) == "true";
            }

            public LanguageInfo(string strLangType, string strLangName, string strLangCode, Font font, Color fontColor)
            {
                LangType = strLangType;
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
                        Debug.Assert(!String.IsNullOrEmpty(LangName));
                }
            }

            public const string CstrElementLabelLanguageInfo = "LanguageInfo";

            public const string CstrAttributeLang = "lang";
            public const string CstrAttributeName = "name";
            public const string CstrAttributeCode = "code";
            public const string CstrAttributeFontName = "FontName";
            public const string CstrAttributeFontSize = "FontSize";
            public const string CstrAttributeFontColor = "FontColor";
            public const string CstrAttributeSentenceFinalPunct = "SentenceFinalPunct";
            public const string CstrAttributeRTL = "RTL";
            public const string CstrAttributeKeyboard = "Keyboard";

            public XElement GetXml
            {
                get
                {
                    XElement elemLang =
                        new XElement(CstrElementLabelLanguageInfo,
                            new XAttribute(CstrAttributeLang, LangType),
                            new XAttribute(CstrAttributeName, LangName),
                            new XAttribute(CstrAttributeCode, LangCode),
                            new XAttribute(CstrAttributeFontName, DefaultFontName),
                            new XAttribute(CstrAttributeFontSize, DefaultFontSize),
                            new XAttribute(CstrAttributeFontColor, FontColor.Name));

                    if (!String.IsNullOrEmpty(FullStop))
                        elemLang.Add(new XAttribute(CstrAttributeSentenceFinalPunct, FullStop));

                    // when saving, though, we only write out the default value (override
                    //  values (if any) are saved by the member ID info)
                    if (DefaultRtl)
                        elemLang.Add(new XAttribute(CstrAttributeRTL, DefaultRtl));

                    if (!String.IsNullOrEmpty(DefaultKeyboard))
                        elemLang.Add(new XAttribute(CstrAttributeKeyboard, DefaultKeyboard));

                    return elemLang;
                }
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

            public void Serialize(XElement elemLanguageInfo)
            {
                LangName = XmlRead.RequiredAttr(elemLanguageInfo, CstrAttributeName);
                LangCode = XmlRead.RequiredAttr(elemLanguageInfo, CstrAttributeCode);
                DefaultFontName = XmlRead.RequiredAttr(elemLanguageInfo, CstrAttributeFontName);
                DefaultFontSize = XmlRead.Float(elemLanguageInfo, CstrAttributeFontSize) ??
                                  throw new ApplicationException(
                                      $"The project file is damaged: <{elemLanguageInfo.Name}> is missing the required attribute '{CstrAttributeFontSize}'.");
                FontToUse = new Font(DefaultFontName, DefaultFontSize);
                FontColor = Color.FromName(XmlRead.RequiredAttr(elemLanguageInfo, CstrAttributeFontColor));
                FullStop = XmlRead.RequiredAttr(elemLanguageInfo, CstrAttributeSentenceFinalPunct);
                DefaultRtl = XmlRead.Bool(elemLanguageInfo, CstrAttributeRTL, false);
                var strKeyboard = XmlRead.Attr(elemLanguageInfo, CstrAttributeKeyboard);
                DefaultKeyboard = !String.IsNullOrEmpty(strKeyboard) ? strKeyboard : null;
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
        protected const string CstrDropBoxRoot = "Dropbox Root";

        public static string DropboxFolderRoot
        {
            get
            {
                string strDropboxRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                                     "Dropbox");
                if (Directory.Exists(strDropboxRoot))
                    return strDropboxRoot;

                strDropboxRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                              "Dropbox");
                if (Directory.Exists(strDropboxRoot))
                    return strDropboxRoot;

                // else check in the person's registry for it
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    return (string)keyOneStoryHiveRoot.GetValue(CstrDropBoxRoot);
                return null;
            }
            set
            {
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot, true) ??
                                          Registry.CurrentUser.CreateSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    keyOneStoryHiveRoot.SetValue(CstrDropBoxRoot, value);
            }
        }

        public static string SayMoreFolderRoot
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "SayMore");

            }
        }

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                string strDefaultProjectFolderRoot = null;
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    strDefaultProjectFolderRoot = (string)keyOneStoryHiveRoot.GetValue(CstrRootDirKey);

                if (String.IsNullOrEmpty(strDefaultProjectFolderRoot))
                {
                    // doesn't work on a Mac w/ Parallels:
                    // strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    // but this should:
                    var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    strDefaultProjectFolderRoot = Path.Combine(userProfile,
                                                               Properties.Resources.DefMyDocsFolder);
                }

                var strPath = Path.Combine(strDefaultProjectFolderRoot,
                                           Properties.Resources.DefMyDocsSubfolder);

                return strPath;
            }
            set
            {
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot, true) ??
                                          Registry.CurrentUser.CreateSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    keyOneStoryHiveRoot.SetValue(CstrRootDirKey, value);
            }
        }

        public const string CstrElementLabelLanguages = "Languages";

        public const string CstrAttributeLabelUseRetellingVernacular = "UseRetellingVernacular";
        public const string CstrAttributeLabelUseRetellingNationalBT = "UseRetellingNationalBT";
        public const string CstrAttributeLabelUseRetellingInternationalBT = "UseRetellingInternationalBT";
        public const string CstrAttributeLabelUseTestQuestionVernacular = "UseTestQuestionVernacular";
        public const string CstrAttributeLabelUseTestQuestionNationalBT = "UseTestQuestionNationalBT";
        public const string CstrAttributeLabelUseTestQuestionInternationalBT = "UseTestQuestionInternationalBT";
        public const string CstrAttributeLabelUseAnswerVernacular = "UseAnswerVernacular";
        public const string CstrAttributeLabelUseAnswerNationalBT = "UseAnswerNationalBT";
        public const string CstrAttributeLabelUseAnswerInternationalBT = "UseAnswerInternationalBT";

        public const string CstrElementLabelAdaptItConfigurations = "AdaptItConfigurations";
        public const string CstrElementLabelAdaptItConfiguration = "AdaptItConfiguration";

        public XElement GetXml
        {
            get
            {
                // have to have one or the other languages
                Debug.Assert(Vernacular.HasData || NationalBT.HasData || InternationalBT.HasData || FreeTranslation.HasData);

                var elem = new XElement(CstrElementLabelLanguages,
                    new XAttribute(CstrAttributeLabelUseRetellingVernacular, ShowRetellings.Vernacular),
                    new XAttribute(CstrAttributeLabelUseRetellingNationalBT, ShowRetellings.NationalBt),
                    new XAttribute(CstrAttributeLabelUseRetellingInternationalBT, ShowRetellings.InternationalBt),
                    new XAttribute(CstrAttributeLabelUseTestQuestionVernacular, ShowTestQuestions.Vernacular),
                    new XAttribute(CstrAttributeLabelUseTestQuestionNationalBT, ShowTestQuestions.NationalBt),
                    new XAttribute(CstrAttributeLabelUseTestQuestionInternationalBT, ShowTestQuestions.InternationalBt),
                    new XAttribute(CstrAttributeLabelUseAnswerVernacular, ShowAnswers.Vernacular),
                    new XAttribute(CstrAttributeLabelUseAnswerNationalBT, ShowAnswers.NationalBt),
                    new XAttribute(CstrAttributeLabelUseAnswerInternationalBT, ShowAnswers.InternationalBt));

                if (Vernacular.HasData)
                    elem.Add(Vernacular.GetXml);

                if (NationalBT.HasData)
                    elem.Add(NationalBT.GetXml);

                if (InternationalBT.HasData)
                    elem.Add(InternationalBT.GetXml);

                if (FreeTranslation.HasData)
                    elem.Add(FreeTranslation.GetXml);

                return elem;
            }
        }

        public bool HasAdaptItConfigurationData
        {
            get
            {
                return (((VernacularToNationalBt != null) && VernacularToNationalBt.HasData)
                        || ((VernacularToInternationalBt != null) && VernacularToInternationalBt.HasData)
                        || ((NationalBtToInternationalBt != null) && NationalBtToInternationalBt.HasData));
            }
        }

        public XElement AdaptItConfigXml
        {
            get
            {
                Debug.Assert(HasAdaptItConfigurationData);
                var elem = new XElement(CstrElementLabelAdaptItConfigurations);

                if ((VernacularToNationalBt != null) && VernacularToNationalBt.HasData)
                    elem.Add(VernacularToNationalBt.GetXml);

                if ((VernacularToInternationalBt != null) && VernacularToInternationalBt.HasData)
                    elem.Add(VernacularToInternationalBt.GetXml);

                if ((NationalBtToInternationalBt != null) && NationalBtToInternationalBt.HasData)
                    elem.Add(NationalBtToInternationalBt.GetXml);

                return elem;
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
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideFontNameFreeTranslation))
                FreeTranslation.FontToUse =
                    new Font(loggedOnMember.OverrideFontNameFreeTranslation, loggedOnMember.OverrideFontSizeFreeTranslation);
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideVernacularKeyboard))
                Vernacular.KeyboardOverride = loggedOnMember.OverrideVernacularKeyboard;
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideNationalBTKeyboard))
                NationalBT.KeyboardOverride = loggedOnMember.OverrideNationalBTKeyboard;
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideInternationalBTKeyboard))
                InternationalBT.KeyboardOverride = loggedOnMember.OverrideInternationalBTKeyboard;
            if (!String.IsNullOrEmpty(loggedOnMember.OverrideFreeTranslationKeyboard))
                FreeTranslation.KeyboardOverride = loggedOnMember.OverrideFreeTranslationKeyboard;
            Vernacular.InvertRtl = loggedOnMember.OverrideRtlVernacular;
            NationalBT.InvertRtl = loggedOnMember.OverrideRtlNationalBT;
            InternationalBT.InvertRtl = loggedOnMember.OverrideRtlInternationalBT;
            FreeTranslation.InvertRtl = loggedOnMember.OverrideRtlFreeTranslation;
        }
    }

    public class ShowLanguageFields
    {
        public ShowLanguageFields()
        {
            InternationalBt = true; // by default
        }
        public bool Vernacular { get; set; }
        public bool NationalBt { get; set; }
        public bool InternationalBt { get; set; }
        public bool Configured
        {
            get { return Vernacular || NationalBt || InternationalBt; }
        }
    }

    public class TasksPf
    {
        [Flags]
        public enum TaskSettings
        {
            NotSet = 0,
            VernacularLangFields = 1,
            NationalBtLangFields = 2,
            InternationalBtFields = 4,
            FreeTranslationFields = 8,
            Anchors = 16,
            Retellings = 32,
            TestQuestions = 64,
            Answers = 128,
            Retellings2 = 256,
            Answers2 = 512,
            None = 1024
        }

        public static StoryEditor.TextFields FilterTextFields(StoryEditor.TextFields fieldsToFilter, TaskSettings pfAllowedTasks)
        {
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.Vernacular, 
                            pfAllowedTasks, TaskSettings.VernacularLangFields);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.NationalBt,
                            pfAllowedTasks, TaskSettings.NationalBtLangFields);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.InternationalBt,
                            pfAllowedTasks, TaskSettings.InternationalBtFields);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.FreeTranslation,
                            pfAllowedTasks, TaskSettings.FreeTranslationFields);

            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.Anchor,
                            pfAllowedTasks, TaskSettings.Anchors);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.Retelling,
                            pfAllowedTasks, TaskSettings.Retellings | TaskSettings.Retellings2);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.TestQuestion,
                            pfAllowedTasks, TaskSettings.TestQuestions);
            ResetValueIfOff(ref fieldsToFilter, StoryEditor.TextFields.TestQuestionAnswer,
                            pfAllowedTasks, TaskSettings.Answers | TaskSettings.Answers2);
            return fieldsToFilter;
        }

        private static void ResetValueIfOff(ref StoryEditor.TextFields fieldsToFilter, 
                                            StoryEditor.TextFields fieldToTurnOff,
                                            TaskSettings pfAllowedTasks,
                                            TaskSettings taskToCheck)
        {
            if ((pfAllowedTasks & taskToCheck) == TaskSettings.NotSet)
                fieldsToFilter &= ~fieldToTurnOff;
        }

        public static bool IsTaskOn(TaskSettings value, TaskSettings flagToTest)
        {
            return ((value & flagToTest) != TaskSettings.NotSet);
        }

        public static TaskSettings DefaultAllowed
        {
            get
            {
                return TaskSettings.VernacularLangFields |
                       TaskSettings.NationalBtLangFields |
                       TaskSettings.InternationalBtFields |
                       TaskSettings.FreeTranslationFields |
                       TaskSettings.Anchors |
                       TaskSettings.Retellings |
                       TaskSettings.TestQuestions |
                       TaskSettings.Answers;
            }
        }

        public static TaskSettings DefaultRequired
        {
            get
            {
                return TaskSettings.Anchors;
            }
        }
    }

    public class TasksCit
    {
        [Flags]
        public enum TaskSettings
        {
            None = 0,
            SendToProjectFacilitatorForRevision = 1,
            SendToCoachForReview = 2
        }

        public static bool IsTaskOn(TaskSettings value, TaskSettings flagToTest)
        {
            return ((value & flagToTest) != TaskSettings.None);
        }

        public static TaskSettings DefaultAllowed
        {
            get
            {
                return TaskSettings.SendToProjectFacilitatorForRevision |
                       TaskSettings.SendToCoachForReview;
            }
        }

        public static TaskSettings DefaultRequired
        {
            get
            {
                return TaskSettings.None;
            }
        }
    }
}
