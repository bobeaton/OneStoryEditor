using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Chorus.UI.Clone;
using Chorus.UI.Sync;
using Chorus.Utilities;
using Chorus.VcsDrivers;
using Chorus.VcsDrivers.Mercurial;
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

        public string HgRepoUrlHost;    // e.g. http://hg-private.languagedepot.org
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

        public ProjectSettings(string strProjectFolderDefaultIfNull, string strProjectName,
            bool bLookForHgUrlHost)
        {
            ProjectName = strProjectName;
            if (String.IsNullOrEmpty(strProjectFolderDefaultIfNull))
                _strProjectFolder = GetDefaultProjectPath(ProjectName);
            else
            {
                System.Diagnostics.Debug.Assert(strProjectFolderDefaultIfNull[strProjectFolderDefaultIfNull.Length - 1] != '\\');
                _strProjectFolder = strProjectFolderDefaultIfNull;
            }

            if (bLookForHgUrlHost)
            {
                HgRepoUrlHost = GetHgRepoUrlHostFromProjectFile();
            }
        }

        private string GetHgRepoUrlHostFromProjectFile()
        {
            if (!File.Exists(ProjectFilePath))
                return null;

            // this is kind of a hack, but it works for now. We're going to read in the 
            //  project (xml) file as though it were just a utf-8 text file and look for 
            //  the attribute at the beginning for the HgRepoUrl host (this is usually so 
            //  we can build the sync command (see SyncWithRepository) to update the file 
            //  before *actually* opening it.
            // it's usually something like:
            //  <StoryProject version="1.6" ProjectName="asdg" HgRepoUrlHost="http://hg-private.languagedepot.org" PanoramaFrontMatter=...
            string strProjectFileContents = File.ReadAllText(ProjectFilePath, Encoding.UTF8);
            const string strToSearchFor = StoryProjectData.CstrAttributeHgRepoUrlHost + "=\"";
            int nIndex = strProjectFileContents.IndexOf(strToSearchFor);
            if (nIndex >= 0)
            {
                // look past the attribute name...
                nIndex += strToSearchFor.Length;

                // get the index (or len) to the end of the attribute value and return
                //  that substring
                int nLength = strProjectFileContents.IndexOf('"', nIndex) - nIndex;
                return strProjectFileContents.Substring(nIndex, nLength);
            }

            // finally, worst case, we *might* have it in the settings file
            string strBaseUrl = null, strFullUri = Program.GetHgRepoFullUrl(ProjectName);
            if (!String.IsNullOrEmpty(strFullUri))
            {
                var uri = new Uri(strFullUri);
                string strDummy;
                StoryEditor.GetDetailsFromUri(uri, out strDummy, out strDummy, ref strBaseUrl);
            }
            return strBaseUrl;
        }

        public ProjectSettings(XmlNode node, string strProjectFolder)
        {
            XmlAttribute attr;
            ProjectName = ((attr = node.Attributes[StoryProjectData.CstrAttributeProjectName]) != null)
                              ? attr.Value
                              : null;

            _strProjectFolder = strProjectFolder;

            HgRepoUrlHost = ((attr = node.Attributes[StoryProjectData.CstrAttributeHgRepoUrlHost]) != null)
                                 ? attr.Value
                                 : null;

            UseDropbox = ((attr = node.Attributes[StoryProjectData.CstrAttributeUseDropbox]) != null) && (attr.Value == "true");
            DropboxStory = ((attr = node.Attributes[StoryProjectData.CstrAttributeDropboxStory]) != null) && (attr.Value == "true");
            DropboxRetelling = ((attr = node.Attributes[StoryProjectData.CstrAttributeDropboxRetellings]) != null) && (attr.Value == "true");
            DropboxAnswers = ((attr = node.Attributes[StoryProjectData.CstrAttributeDropboxAnswers]) != null) && (attr.Value == "true");

            Vernacular = new LanguageInfo(node.SelectSingleNode(XPathForLangInformation(LineData.CstrAttributeLangVernacular)));
            NationalBT = new LanguageInfo(node.SelectSingleNode(XPathForLangInformation(LineData.CstrAttributeLangNationalBt)));
            InternationalBT = new LanguageInfo(node.SelectSingleNode(XPathForLangInformation(LineData.CstrAttributeLangInternationalBt)));
            FreeTranslation = new LanguageInfo(node.SelectSingleNode(XPathForLangInformation(LineData.CstrAttributeLangFreeTranslation)));
        }

        private static string XPathForLangInformation(string strLangType)
        {
            return String.Format("{0}/{1}[@lang = '{2}']",
                                 CstrElementLabelLanguages,
                                 LanguageInfo.CstrElementLabelLanguageInfo,
                                 strLangType);
        }

        public void SerializeProjectSettings(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.StoryProject[0].ProjectName == ProjectName));

            NewDataSet.LanguagesRow theLangRow = InsureLanguagesRow(projFile);

            if (!theLangRow.IsUseRetellingVernacularNull())
                ShowRetellings.Vernacular = theLangRow.UseRetellingVernacular;

            if (!theLangRow.IsUseRetellingNationalBTNull())
                ShowRetellings.NationalBt = theLangRow.UseRetellingNationalBT;

            if (!theLangRow.IsUseRetellingInternationalBTNull())
                ShowRetellings.InternationalBt = theLangRow.UseRetellingInternationalBT;

            if (!theLangRow.IsUseTestQuestionVernacularNull())
                ShowTestQuestions.Vernacular = theLangRow.UseTestQuestionVernacular;

            if (!theLangRow.IsUseTestQuestionNationalBTNull())
                ShowTestQuestions.NationalBt = theLangRow.UseTestQuestionNationalBT;

            if (!theLangRow.IsUseTestQuestionInternationalBTNull())
                ShowTestQuestions.InternationalBt = theLangRow.UseTestQuestionInternationalBT;

            if (!theLangRow.IsUseAnswerVernacularNull())
                ShowAnswers.Vernacular = theLangRow.UseAnswerVernacular;

            if (!theLangRow.IsUseAnswerNationalBTNull())
                ShowAnswers.NationalBt = theLangRow.UseAnswerNationalBT;

            if (!theLangRow.IsUseAnswerInternationalBTNull())
                ShowAnswers.InternationalBt = theLangRow.UseAnswerInternationalBT;

            if (projFile.AdaptItConfigurations.Count == 1)
            {
                foreach (NewDataSet.AdaptItConfigurationRow aAiConfigRow in projFile.AdaptItConfigurations[0].GetAdaptItConfigurationRows())
                {
                    if (aAiConfigRow.BtDirection == AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt.ToString())
                    {
                        VernacularToNationalBt = new AdaptItConfiguration();
                        VernacularToNationalBt.SerializeFromProjectFile(aAiConfigRow);
                    }
                    if (aAiConfigRow.BtDirection == AdaptItConfiguration.AdaptItBtDirection.VernacularToInternationalBt.ToString())
                    {
                        VernacularToInternationalBt = new AdaptItConfiguration();
                        VernacularToInternationalBt.SerializeFromProjectFile(aAiConfigRow);
                    }
                    if (aAiConfigRow.BtDirection == AdaptItConfiguration.AdaptItBtDirection.NationalBtToInternationalBt.ToString())
                    {
                        NationalBtToInternationalBt = new AdaptItConfiguration();
                        NationalBtToInternationalBt.SerializeFromProjectFile(aAiConfigRow);
                    }
                }
            }

            bool bFoundInternationalBt = false, bFoundFreeTranslation = false;
            foreach (NewDataSet.LanguageInfoRow aLangRow in theLangRow.GetLanguageInfoRows())
            {
                if (aLangRow.lang == LineData.CstrAttributeLangVernacular)
                    Vernacular.Serialize(aLangRow);
                if (aLangRow.lang == LineData.CstrAttributeLangNationalBt)
                    NationalBT.Serialize(aLangRow);
                if (aLangRow.lang == LineData.CstrAttributeLangInternationalBt)
                {
                    bFoundInternationalBt = true;
                    InternationalBT.Serialize(aLangRow);
                }
                if (aLangRow.lang == LineData.CstrAttributeLangFreeTranslation)
                {
                    bFoundFreeTranslation = true;
                    FreeTranslation.Serialize(aLangRow);
                }
            }

            // the "international language" will appear to "have data" even when it shouldn't
            //  so clear out the default language name in this case:
            if (!bFoundInternationalBt)
            {
                InternationalBT.LangName = null;
                System.Diagnostics.Debug.Assert(!InternationalBT.HasData);
            }

            // the "international language" will appear to "have data" even when it shouldn't
            //  so clear out the default language name in this case:
            if (!bFoundFreeTranslation)
            {
                FreeTranslation.LangName = null;
                System.Diagnostics.Debug.Assert(!FreeTranslation.HasData);
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

            public void SerializeFromProjectFile(NewDataSet.AdaptItConfigurationRow aAiConfigRow)
            {
                ProjectType = (AdaptItProjectType)Enum.Parse(typeof(AdaptItProjectType), aAiConfigRow.ProjectType);
                BtDirection = (AdaptItBtDirection)Enum.Parse(typeof(AdaptItBtDirection), aAiConfigRow.BtDirection);
                ConverterName = aAiConfigRow.ConverterName;
                if (!aAiConfigRow.IsProjectFolderNameNull())
                    ProjectFolderName = aAiConfigRow.ProjectFolderName;

                if (!aAiConfigRow.IsRepoProjectNameNull())
                    RepoProjectName = aAiConfigRow.RepoProjectName;

                if (!aAiConfigRow.IsRepositoryServerNull())
                    RepositoryServer = aAiConfigRow.RepositoryServer;

                if (!aAiConfigRow.IsNetworkRepositoryPathNull())
                    NetworkRepositoryPath = aAiConfigRow.NetworkRepositoryPath;
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
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterName));
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
                System.Diagnostics.Debug.Assert(ProjectType == AdaptItProjectType.SharedAiProject);
                if (!AlreadyCheckedForSync
                    && !String.IsNullOrEmpty(strProjectFolder)
                    && !String.IsNullOrEmpty(RepoProjectName))
                {
                    if (!Program.AreAdaptItHgParametersSet(RepoProjectName))
                    {
                        Program.SetAdaptItHgParameters(strProjectFolder,
                                                       RepoProjectName,
                                                       RepositoryServer,
                                                       loggedOnMember.HgUsername,
                                                       loggedOnMember.HgPassword);
                    }

                    // if the folder doesn't exist or the repo doesn't exist...
                    if (!Directory.Exists(strProjectFolder) ||
                        !Directory.Exists(Path.Combine(strProjectFolder, ".hg")))
                    {
                        // offer to clone it
                        if (LocalizableMessageBox.Show(Localizer.Str("The shared Adapt It project for this field is not on the local computer. Please enter the necessary information in the next window to download it from the internet (i.e. the repository server, username and password). These should be in an email message you received previously"),
                                            StoryEditor.OseCaption,
                                            MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return;

                        if (!DoPossiblePull(strProjectFolder, loggedOnMember))
                            return;
                    }
                    else
                        Program.SyncWithAiRepository(strProjectFolder, RepoProjectName, true);

                    Program.SetAiProjectForSyncage(strProjectFolder, RepoProjectName);
                    AlreadyCheckedForSync = true;
                }
            }

            public bool DoPossiblePull(string strProjectFolder, TeamMemberData loggedOnMember)
            {
                string strHgUsername = null, strHgPassword = null;
                if (loggedOnMember != null)
                {
                    strHgUsername = loggedOnMember.HgUsername;
                    strHgPassword = loggedOnMember.HgPassword;
                }

                // the GetClone dialog is expecting that the parent folder exist (e.g.
                //  C:\Documents and Settings\Bob\My Documents\Adapt It Unicode Work)
                string strAiWorkFolder = Path.GetDirectoryName(strProjectFolder);
                Debug.Assert((strAiWorkFolder != null) && (strAiWorkFolder == AdaptItKBReader.AdaptItWorkFolder));
                if (!Directory.Exists(strAiWorkFolder))
                    Directory.CreateDirectory(strAiWorkFolder);

                string strAiProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                var strServerLabel = RepositoryServer;
                if (String.IsNullOrEmpty(strServerLabel))
                    strServerLabel = Properties.Resources.IDS_DefaultRepoServer;

                var model = new GetCloneFromInternetModel(strAiWorkFolder)
                {
                    ProjectId = RepoProjectName,
                    SelectedServerLabel = strServerLabel,
                    LocalFolderName = strAiProjectFolderName,
                    AccountName = strHgUsername,
                    Password = strHgPassword
                };

                using (var dlg = new GetCloneFromInternetDialog(model))
                {
                    if (DialogResult.Cancel == dlg.ShowDialog())
                        return false;

                    // we can save this information so we can use it automatically during the next restart
                    Program.SetAdaptItHgParameters(dlg.PathToNewlyClonedFolder,
                                                   RepoProjectName = model.ProjectId,
                                                   RepositoryServer = model.SelectedServerLabel,
                                                   model.AccountName,
                                                   model.Password);
                }

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

            public LanguageInfo(XmlNode node)
            {
                if (node == null)
                    return;

                XmlAttribute attr;
                LangType = ((attr = node.Attributes[CstrAttributeLang]) != null) ? attr.Value : null;
                LangName = ((attr = node.Attributes[CstrAttributeName]) != null) ? attr.Value : null;
                LangCode = ((attr = node.Attributes[CstrAttributeCode]) != null) ? attr.Value : null;
                DefaultFontName = ((attr = node.Attributes[CstrAttributeFontName]) != null) ? attr.Value : null;
                DefaultFontSize = ((attr = node.Attributes[CstrAttributeFontSize]) != null) ? Convert.ToSingle(attr.Value) : 12;
                FontToUse = new Font(DefaultFontName, DefaultFontSize);
                FontColor = ((attr = node.Attributes[CstrAttributeFontColor]) != null) ? Color.FromName(attr.Value) : Color.Black;
                FullStop = ((attr = node.Attributes[CstrAttributeSentenceFinalPunct]) != null) ? attr.Value : null;
                DefaultKeyboard = ((attr = node.Attributes[CstrAttributeKeyboard]) != null) ? attr.Value : null;
                DefaultRtl = ((attr = node.Attributes[CstrAttributeRTL]) != null) ? (attr.Value == "true") : false;
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
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(LangName));
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

            public void Serialize(NewDataSet.LanguageInfoRow aLangRow)
            {
                LangName = aLangRow.name;
                LangCode = aLangRow.code;
                DefaultFontName = aLangRow.FontName;
                DefaultFontSize = aLangRow.FontSize;
                FontToUse = new Font(aLangRow.FontName, aLangRow.FontSize);
                FontColor = Color.FromName(aLangRow.FontColor);
                FullStop = aLangRow.SentenceFinalPunct;
                DefaultRtl = (!aLangRow.IsRTLNull() && aLangRow.RTL);
                DefaultKeyboard =
                    (!aLangRow.IsKeyboardNull() && !String.IsNullOrEmpty(aLangRow.Keyboard))
                        ? aLangRow.Keyboard
                        : null;

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
                    strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
                System.Diagnostics.Debug.Assert(Vernacular.HasData || NationalBT.HasData || InternationalBT.HasData || FreeTranslation.HasData);

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
                System.Diagnostics.Debug.Assert(HasAdaptItConfigurationData);
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

        protected NewDataSet.LanguagesRow InsureLanguagesRow(NewDataSet projFile)
        {
            System.Diagnostics.Debug.Assert(projFile.StoryProject.Count == 1);
            if (projFile.Languages.Count == 0)
                return projFile.Languages.AddLanguagesRow(
                    ShowRetellings.Vernacular, ShowRetellings.NationalBt, ShowRetellings.InternationalBt,
                    ShowTestQuestions.Vernacular, ShowTestQuestions.NationalBt, ShowTestQuestions.InternationalBt,
                    ShowAnswers.Vernacular, ShowAnswers.NationalBt, ShowAnswers.InternationalBt,
                    projFile.StoryProject[0]);

            System.Diagnostics.Debug.Assert(projFile.Languages.Count == 1);
            return projFile.Languages[0];
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

        public void SyncWithRepository(string strUsername, string strPassword)
        {
            SyncWithRepository(ProjectFolder, ProjectName, HgRepoUrlHost, strUsername, strPassword,
                Program.LookupSharedNetworkPath(ProjectFolder));
        }

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test
        // or \\Bob-StudioXPS\Backup\Storying\snwmtn-test
        public static void SyncWithRepository(string strProjectFolder, string strProjectName, string strHgRepoUrlHost,
            string strUsername, string strPassword, string strSharedNetworkPath)
        {
            // the project folder name has come here bogus at times...
            if (!Directory.Exists(strProjectFolder))
                return;

            try
            {
                string strHgUrl = Program.FormHgUrl(strHgRepoUrlHost,
                                                    strUsername,
                                                    strPassword,
                                                    strProjectName);
                if (String.IsNullOrEmpty(strHgUrl))
                    Program.SyncWithRepository(strProjectFolder, true);
                else
                    TrySyncWithRepository(strProjectName, strProjectFolder, strHgUrl, strSharedNetworkPath);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private static void TrySyncWithRepository(string strProjectName,
            string strProjectFolder, string strRepoUrl, string strSharedNetworkPath)
        {
            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = Program.GetProjectFolderConfiguration(strProjectFolder);
            /*
            var nullProgress = new NullProgress();
            var repo = new HgRepository(strProjectFolder, nullProgress);
            if (!repo.GetCanConnectToRemote(strRepoUrl, nullProgress))
                if (Program.UserCancelledNotConnectToInternetWarning)
                {
                    return;
                }
            */
            // for when we launch the program, just do a quick & dirty send/receive, 
            //  but for closing (or if we have a network drive also), then we want to 
            //  be more informative
            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.Lazy, 
                SyncUIFeatures.NormalRecommended))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                if (!String.IsNullOrEmpty(strRepoUrl))
                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(Program.CstrInternetName, strRepoUrl));
                if (!String.IsNullOrEmpty(strSharedNetworkPath))
                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(Program.CstrNetworkDriveName, strSharedNetworkPath));

                dlg.Text = "Synchronizing OneStory Project: " + strProjectName;
                dlg.ShowDialog();
            }
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
