using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using AiChorus.Properties;
using ECInterfaces;
using Microsoft.Win32;
using OneStoryProjectEditor;
using SilEncConverters40;

namespace AiChorus
{
    static class Program
    {
        public const string CstrStoryEditorExe = "StoryEditor.exe";
        public const string CstrApplicationTypeOse = "OneStory Editor";
        public const string CstrApplicationTypeAi = "Adapt It";
        private const string KeyFileName = "gdck.dat";

        private static string KeyFileSpec
        {
            get
            {
                var dropboxPath = Path.Combine(Path.Combine(DropboxFolderRoot, "OSE CPCs"), KeyFileName);
                return (File.Exists(dropboxPath))
                        ? dropboxPath
                        : KeyFileName;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if encryptingNewCredentials
            var specialEncryptionKey = File.ReadAllText(KeyFileName);
            var clientId = EncryptionClass.Encrypt(..., specialEncryptionKey);
            var clientSecret = EncryptionClass.Encrypt(..., specialEncryptionKey);
#endif

            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }

            if (args.Length == 0)
            {
                MessageBox.Show(Resources.UsageString, Resources.AiChorusCaption);
                return;
            }

            try
            {
                if (args[0] == "/f")
                    ProcessChorusConfigFile((args.Length == 2) ? args[1] : null);
                else if (args[0] == "/e")
                    DoEdit();
                else if ((args[0] == "/s") && (args.Length == 2) && (File.Exists(args[1])))
                    SyncChorusProjects(args[1]);
                else if ((args[0] == "/h") && (args.Length == 2) && (File.Exists(args[1])))
                    HarvestProjectData(args[1]);
                else
                    LaunchProgram("Chorus.exe", $"\"{Settings.Default.LastProjectFolder}\"");
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private static void HarvestProjectData(string strPathToProjectFile)
        {
            LogMessage(String.Format("Harvesting data from the projects in the .cpc file: '{0}'", strPathToProjectFile));

            if (!File.Exists(KeyFileName))
            {
                LogMessage($"Missing the keyfile, '{KeyFileName}'!");
            }

            var chorusConfig = ChorusConfigurations.Load(strPathToProjectFile);
            HarvestProjectSet(chorusConfig);
        }

        public static void HarvestProjectSet(ChorusConfigurations chorusConfig)
        {
            foreach (var server in chorusConfig.ServerSettings)
                HarvestProjectData(server);
        }

        private const string OneStoryHiveRoot = @"Software\SIL\OneStory";
        private const string CstrDropBoxRoot = "Dropbox Root";

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

        private static void HarvestProjectData(ServerSetting serverSetting)
        {
            var mapProjectsToProjectData = new Dictionary<string, List<OseProjectData>>();

            // we have a flag that will excluding a project from being harvested and we only harvest OSE projects
            foreach (var project in serverSetting.Projects.Where(p => !p.ExcludeFromGoogleSheet && (p.ApplicationType == CstrApplicationTypeOse)))
            {
                LogMessage(String.Format("Harvesting data from the project: '{0}'", project.ProjectId));
                HarvestProjectData(project, serverSetting, mapProjectsToProjectData);
            }

            var specialDecryptionKey = File.ReadAllText(KeyFileSpec);
            var clientId = EncryptionClass.Decrypt(Settings.Default.GoogleSheetsCredentialsClientId, specialDecryptionKey);
            var clientSecret = EncryptionClass.Decrypt(Settings.Default.GoogleSheetsCredentialsClientSecret, specialDecryptionKey);
            GoogleSheetHandler.UpdateGoogleSheet(mapProjectsToProjectData, serverSetting.GoogleSheetId,
                                                 clientId, clientSecret);
        }

        private static void HarvestProjectData(Project project, ServerSetting serverSetting,
                                               Dictionary<string, List<OseProjectData>> mapProjectsToProjectData)
        {
            if (!GetSyncApplicationHandler(project, serverSetting, project.ApplicationType, out ApplicationSyncHandler appHandler))
                return;

            ((OseSyncHandler)appHandler).HarvestProjectData(mapProjectsToProjectData);
        }

        private static List<string> ConvertToList(string googleDocProjections)
        {
            // e.g. name,stage,TransitionHistory.StateTransition.LoggedInMemberId,TransitionHistory.StateTransition.TransitionDateTime,CraftingInfo.ProjectFacilitator.memberID
            return googleDocProjections.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static void SyncChorusProjects(string strPathToProjectFile)
        {
            LogMessage(String.Format("Processing the file: '{0}'", strPathToProjectFile));
            var chorusConfig = ChorusConfigurations.Load(strPathToProjectFile);
            SychronizeProjectSet(chorusConfig);
        }

        public static void SychronizeProjectSet(ChorusConfigurations chorusConfig)
        {
            foreach (var server in chorusConfig.ServerSettings)
                SyncServer(server);
        }

        private static void SyncServer(ServerSetting serverSetting)
        {
            foreach (var project in serverSetting.Projects.Where(p => p.ExcludeFromSyncing != true))
            {
                LogMessage(String.Format("Processing the project: '{0}'", project.ProjectId));
                SyncProject(project, serverSetting);
            }
        }

        private static string _strLogFilepath;
        private static string LogPath
        {
            get
            {
                return _strLogFilepath ??
                       (_strLogFilepath = Path.Combine(Application.UserAppDataPath, "ChorusSync.log"));
            }
        }

        public static void LogMessage(string strOutput)
        {
            Console.WriteLine(strOutput);
            var strLine = String.Format("{0}: {1}{2}", DateTime.Now, strOutput, Environment.NewLine);

            // for some reason, if a log file was originally created in a server folder, say by my computer,
            //  the server doesn't then like to allow a process run on the server itself to update that file.
            //  (I have no idea why). So a) let's do exception handling around the attempted write to the log
            //  file so that programs don't fail to run and b) let's try some fall back strategies: 1) try to
            //  make a backup so we start again and if that fails, 2) let's try a different filename, and if
            //  that fails, then 3) try to fallback to a known writable folder.
            try
            {
                File.AppendAllText(LogPath, strLine);
            }
            catch (Exception)
            {
            }
        }

        private static void SyncProject(Project project, ServerSetting serverSetting)
        {
            ApplicationSyncHandler appHandler;
            if (!GetSyncApplicationHandler(project, serverSetting, project.ApplicationType, out appHandler))
                return;

            switch (appHandler.ButtonLabel)
            {
                case ApplicationSyncHandler.CstrOptionClone:
                    appHandler.DoClone();
                    break;
                case ApplicationSyncHandler.CstrOptionSendReceive:
                case ApplicationSyncHandler.CstrOptionOpenProject:
                    appHandler.DoSilentSynchronize();
                    break;
                default:
                    Debug.Assert(false, $"Not expecting {appHandler.ButtonLabel}. Should be either Clone or Synchronize or Open Project");
                    break;
            }
        }

        public static bool GetSyncApplicationHandler(Project project, ServerSetting serverSetting, string strApplicationName,
            out ApplicationSyncHandler appHandler)
        {
            switch (strApplicationName)
            {
                case CstrApplicationTypeOse:
                    appHandler = new OseSyncHandler(project, serverSetting);
                    break;

                case CstrApplicationTypeAi:
                    appHandler = new AdaptItSyncHandler(project, serverSetting);
                    break;

                default:
                    MessageBox.Show(String.Format("Sorry, I'm not familiar with the type '{0}", project.ApplicationType),
                        Resources.AiChorusCaption);
                    appHandler = null;
                    return false;
            }
            return true;
        }

        private static int ProjectCount(ChorusConfigurations chorusConfigurations)
        {
            return chorusConfigurations.ServerSettings.Sum(serverSetting => serverSetting.Projects.Count);
        }

        private static void ProcessChorusConfigFile(string strFilePath)
        {
            var chorusConfig = !String.IsNullOrEmpty(strFilePath)
                                    ? ChorusConfigurations.Load(strFilePath)
                                    : new ChorusConfigurations();

            if (chorusConfig == null)
            {
                MessageBox.Show("Invalid file format", Resources.AiChorusCaption);
            }
            else
            {
                PresentProjects(chorusConfig);
            }
        }

        private static void PresentProjects(ChorusConfigurations chorusConfig)
        {
            var projView = new PresentProjectsForm(chorusConfig);
            projView.ShowDialog();
        }

        private static void DoEdit()
        {
            var theEc =
                DirectableEncConverter.EncConverters.AutoSelectWithTitle(ConvType.Unicode_to_from_Unicode,
                                                                         "Choose the 'Lookup in ...' item whose Knowledge base you want to edit and click 'OK'");

            if (theEc == null)
                return;

            if (!(theEc is AdaptItEncConverter))
            {
                if (MessageBox.Show(Resources.MustUseAiLookupConverterToEdit,
                                Resources.AiChorusCaption,
                                MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    DoEdit();
                return;
            }

            try
            {
                var theAiEc = (AdaptItEncConverter)theEc;
                var strData = DisplayKnowledgeBase(theAiEc);
                theAiEc.Configurator.DisplayTestPage(DirectableEncConverter.EncConverters,
                                                     theAiEc.Name,
                                                     theAiEc.ConverterIdentifier,
                                                     ConvType.Unicode_to_from_Unicode,
                                                     strData);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        public static string DisplayKnowledgeBase(AdaptItEncConverter theAiEc)
        {
            if (theAiEc != null)
            {
                var strData = GrabDataPoint(theAiEc.ConverterIdentifier);
                return theAiEc.EditKnowledgeBase(strData);
            }
            return "";
        }

        internal static bool CloneProject(ServerSetting serverSetting, Project project, string strProjectFolderRoot)
        {
            return CloneProject(project.FolderName, serverSetting.Username, strProjectFolderRoot, serverSetting.DecryptedPassword, project.ProjectId);
        }

        public static string OneStoryEditorExePath
        {
            get
            {
                var strStoryEditorPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Debug.Assert(strStoryEditorPath != null, "strStoryEditorExePath != null");
                return Path.Combine(strStoryEditorPath, CstrStoryEditorExe);
            }
        }


        private static bool CloneProject(string strLocalFolderName, string strUsername,
                                         string strProjectFolderRoot, string strPassword, string strProjectId)
        {
            // in the latest Chorus, they remove the username and password from the hgrc file and store it in a settings file that
            //  is based on the exe that clones the project. If 'AIChorus' does the cloning, then the credentials won't be available
            //  to StoryEditor.exe... so just call StoryEditor.exe here to do the cloning and then it'll be in the correct settings
            var args = $"/clone {strProjectId} \"{strProjectFolderRoot}\" \"{strLocalFolderName}\" {strUsername} \"{strPassword}\"";
            LaunchProgram(OneStoryEditorExePath, args, true);
            return true;
        }

        public static AdaptItEncConverter InitializeLookupConverter(string strProjectFolder)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // in case AI isn't installed yet, it really doesn't like not having an Adaptations sub-folder
            var strAdaptationsFolder = Path.Combine(strProjectFolder, "Adaptations");
            if (!Directory.Exists(strAdaptationsFolder))
                Directory.CreateDirectory(strAdaptationsFolder);

            var strFriendlyName = "Lookup in " + strProjectName;
            var strConverterSpec = Path.Combine(strProjectFolder, strProjectName + ".xml");
            var aEcs = new EncConverters(true);
            aEcs.AddConversionMap(strFriendlyName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                                  "SIL.AdaptItKB", "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);

            // we can save this information so we can use it automatically during the next restart
            var aEc = aEcs[strFriendlyName];

            var strData = GrabDataPoint(strConverterSpec);
            aEc.Configurator.DisplayTestPage(aEcs, strFriendlyName, strConverterSpec, ConvType.Unicode_to_from_Unicode, strData);
            return aEc as AdaptItEncConverter;
        }

        // kind of a brute force approach, but it's easy. 
        //  This will return the first source word in the KB as a data point
        private static string GrabDataPoint(string strConverterSpec)
        {
            var doc = XDocument.Load(strConverterSpec);
            var strData = doc.Descendants("TU").First().Attributes("k").First().Value;
            return strData;
        }

        public static string AdaptItWorkFolder
        {
            get
            {
                var strAdaptItWorkFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                        "Adapt It Unicode Work");
                return strAdaptItWorkFolder;
            }
        }

        public static void ShowException(Exception ex)
        {
            var strErrorMsg = FromException(ex);
            MessageBox.Show(strErrorMsg, Resources.AiChorusCaption);
        }

        public static string FromException(Exception ex)
        {
            string strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            return strErrorMsg;
        }

        public static void LaunchProgram(string strProgramPath, string strArguments, bool wait = false)
        {
            try
            {
                var strWorkingDir = Path.GetDirectoryName(strProgramPath);
                var strFilename = Path.GetFileName(strProgramPath);
                var myProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = strProgramPath,
                        Arguments = strArguments,
                        WorkingDirectory = strWorkingDir
                    }
                };

                myProcess.Start();
                if (wait)
                    myProcess.WaitForExit();
            }
            catch { }    // we tried...
        }

        /// <summary>
        /// Check to see if the output file exists and if so, make a backup and then delete it
        /// </summary>
        /// <param name="strFilepath">file path to the output to back up</param>
        public static void MakeBackupOfOutputFile(string strFilepath)
        {
            // first make sure the output directory exists
            Debug.Assert(strFilepath != null, "strXmlFilepathToValidate != null");

            string strParentFolder = Path.GetDirectoryName(strFilepath);
            if (!Directory.Exists(strParentFolder))
                Directory.CreateDirectory(strParentFolder);

            // then check to see if we have to make a copy (to keep the attributes, which 'rename' doesn't do)
            //  as a backup file.
            if (!File.Exists(strFilepath))
                return;

            // it exists, so make a backup
            var strBackupFilename = strFilepath + ".bak";
            File.Delete(strBackupFilename); // just in case there was already a backup
            File.Copy(strFilepath, strBackupFilename);
            File.Delete(strFilepath);
        }

        public static Dictionary<string, string> ArrayToDictionary(StringCollection data)
        {
            var map = new Dictionary<string, string>();
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1]);
            }

            return map;
        }
    }
}
