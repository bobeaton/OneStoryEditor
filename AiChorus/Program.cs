using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using AiChorus.Properties;
using Chorus.UI.Clone;
using ECInterfaces;
using OneStoryProjectEditor;
using SilEncConverters40;

namespace AiChorus
{
    static class Program
    {
        public const string CstrApplicationTypeOse = "OneStory Editor";
        public const string CstrApplicationTypeAi = "Adapt It";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if encryptingNewCredentials
            var specialDecryptionKey = File.ReadAllText("gdck.dat");
            var clientId = EncryptionClass.Encrypt(..., specialDecryptionKey);
            var clientSecret = EncryptionClass.Encrypt(..., specialDecryptionKey);
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
                    LaunchProgram("Chorus.exe", Settings.Default.LastProjectFolder);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private static void HarvestProjectData(string strPathToProjectFile)
        {
            LogMessage(String.Format("Harvesting data from the projects in the .cpc file: '{0}'", strPathToProjectFile));

            if (!File.Exists("gdck.dat"))
            {
                LogMessage($"Missing the keyfile, 'gdck.dat'!");
            }

            var chorusConfig = ChorusConfigurations.Load(strPathToProjectFile);
            foreach (var server in chorusConfig.ServerSettings)
                HarvestProjectData(server, Path.GetFileNameWithoutExtension(strPathToProjectFile));
        }

        private static void HarvestProjectData(ServerSetting serverSetting, string cpcFilename)
        {
            var mapProjectsToProjectData = new Dictionary<string, List<OseProjectData>>();

            foreach (var project in serverSetting.Projects.Where(p => p.ExcludeFromGoogleSheet != true))
            {
                LogMessage(String.Format("Harvesting data from the project: '{0}'", project.ProjectId));
                HarvestProjectData(project, serverSetting, mapProjectsToProjectData);
            }

#if !UsingCsvAndDrive
            var specialDecryptionKey = File.ReadAllText("gdck.dat");
            var clientId = EncryptionClass.Decrypt(Settings.Default.GoogleSheetsCredentialsClientId, specialDecryptionKey);
            var clientSecret = EncryptionClass.Decrypt(Settings.Default.GoogleSheetsCredentialsClientSecret, specialDecryptionKey);
            GoogleSheetHandler.UpdateGoogleSheet(mapProjectsToProjectData, serverSetting.GoogleSheetId,
                                                 clientId, clientSecret);
#else
            var googleDocUrl = serverSetting.GoogleDocUrl;
            var googleDocProjections = ConvertToList(serverSetting.GoogleDocProjections);

            var csvHeader = ProjectionData.GenerateHeader(googleDocProjections);
            var lstLines = new List<string> { csvHeader };
            foreach (var kvp in mapProjectsToStoryInfo)
            {
                foreach (var storyInfo in kvp.Value)
                {
                    var str = storyInfo.GenerateRow(googleDocProjections);
                    lstLines.Add(str);
                }
            }

            var csvFileSpec = Path.GetTempFileName();
            if (File.Exists(csvFileSpec))
                File.Delete(csvFileSpec);
            File.WriteAllLines(csvFileSpec, lstLines, Encoding.UTF8);

            UploadToGoogleDrive(csvFileSpec, Settings.Default.GoogleDriveClientId, Settings.Default.GoogleDriveClientSecret,
                                googleDocUrl, cpcFilename);
#endif
        }

        private static void HarvestProjectData(Project project, ServerSetting serverSetting,
                                               Dictionary<string, List<OseProjectData>> mapProjectsToProjectData)
        {
            if (project.ApplicationType != CstrApplicationTypeOse)
                return;

            if (!GetSyncApplicationHandler(project, serverSetting, project.ApplicationType, out ApplicationSyncHandler appHandler))
                return;

            ((OseSyncHandler)appHandler).HarvestProjectData(mapProjectsToProjectData);
        }

#if UsingCsvAndDrive
        private static string[] scopes = new string[] 
        {
            DriveService.Scope.Drive,
            DriveService.Scope.DriveFile
        };

        private static void UploadToGoogleDrive(string csvFileSpec, string clientId, string clientSecret, string googleDocUrl, string cpcFilename)
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            }, scopes, Environment.UserName, CancellationToken.None, new FileDataStore("OseProjectDataToken")).Result;

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "OseProjectData",
            });

            service.HttpClient.Timeout = TimeSpan.FromSeconds(100);

            var response = UploadFile(service, csvFileSpec, cpcFilename);
            LogMessage($"Harvesting data file uploaded to {response.Name}");
        }

        private static Google.Apis.Drive.v3.Data.File UploadFile(DriveService service, string csvFileSpec, string cpcFilename)
        {
            if (File.Exists(csvFileSpec))
            {
                var folderListRequest = service.Files.List();
                folderListRequest.Q = "mimeType = 'application/vnd.google-apps.folder'";    // and name = 'OseProjectData'
                // folderListRequest.Fields = "files(id,name)";
                folderListRequest.IncludeItemsFromAllDrives = true;
                folderListRequest.SupportsAllDrives = true;
                var folders = folderListRequest.Execute().Files;
                var folder = folders?.FirstOrDefault();
                if (folder == null)
                    throw new ApplicationException($"Unable to find the 'OseProjectData' folder with the {service.HttpClientInitializer} account");

                var body = new Google.Apis.Drive.v3.Data.File();
                body.Name = cpcFilename;
                body.Description = $"Story data harvested from projects in {cpcFilename}";
                body.MimeType = "text/csv";
                body.Parents = new List<string> { folder.Id };
                var byteArray = File.ReadAllBytes(csvFileSpec);
                var stream = new MemoryStream(byteArray);
                try
                {
                    var deleteExistingRequest = service.Files.List();
                    deleteExistingRequest.Q = $"'{folder.Id}' in parents";
                    var existingFiles = deleteExistingRequest.Execute().Files.Where(f => f.Name == cpcFilename).ToList();
                    if ((existingFiles != null) && existingFiles.Any())
                        existingFiles.ForEach(f => service.Files.Delete(f.Id).Execute());

                    FilesResource.CreateMediaUpload request = service.Files.Create(body, stream, "text/csv");
                    request.SupportsTeamDrives = true;
                    // You can bind event handler with progress changed event and response recieved(completed event)
                    request.ProgressChanged += Request_ProgressChanged;
                    request.ResponseReceived += Request_ResponseReceived;
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error Occured");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("The file does not exist.", "404");
                return null;
            }
        }

        private static void Request_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            var statusMsg = obj.Status + " " + obj.BytesSent;
            LogMessage($"Harvesting data file uploading status: {statusMsg}");
        }

        private static void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                var statusMsg = "File was uploaded sucessfully--" + obj.Id;
                LogMessage($"Harvesting data file uploaded status: {statusMsg}");
            }
        }

        /// <summary>
        /// e.g. name,stage,TransitionHistory.StateTransition.LoggedInMemberId,TransitionHistory.StateTransition.TransitionDateTime,CraftingInfo.ProjectFacilitator.memberID
        /// </summary>
        public class ProjectionData
        {
            public const string Delim = ",";    // or it could be '\t' if that helps

            private const string ProjectionNameAll = "AllAvailableFields";

            private const string ProjectionNameProjectName = "ProjectName";
            private const string ProjectionNameStoryLanguageName = "StoryLanguageName";
            private const string ProjectionNameStorySetName = "SetName";
            private const string ProjectionNameStoryName = "StoryName";
            private const string ProjectionNameStoryStage = "StoryStage";
            private const string ProjectionNameStoryPurpose = "Purpose";
            private const string ProjectionNameLastEditor = "LastEditor";
            private const string ProjectionNameCurrentEditor = "CurrentEditor";
            private const string ProjectionNameTimeInStage = "TimeInStage";
            private const string ProjectionNameProjectFacilitator = "ProjectFacilitator";
            private const string ProjectionNameConsultant = "Consultant";
            private const string ProjectionNameCoach = "Coach";
            private const string ProjectionNameCrafter = "StoryCrafter";
            private const string ProjectionNameNumberOfLines = "NumberOfLines";
            private const string ProjectionNameNumberOfTestQuestions = "NumberOfTestQuestions";
            private const string ProjectionNameNumberOfWords = "NumberOfWords";

            public string ProjectName { get; set; }             // required
            public string StoryLanguageName { get; set; }
            public string StorySetName { get; set; }            // required
            public string StoryName { get; set; }               // name
            public string StoryStage { get; set; }              // stage
            public string StoryPurpose { get; set; }            // CraftingInfo.StoryPurpose
            public string LastEditor { get; set; }              // TransitionHistory.StateTransition.LoggedInMemberId
            public string CurrentEditor { get; set; }           
            public string TimeInStage { get; set; }             // TransitionHistory.StateTransition.TransitionDateTime
            public string StoryProjectFacilitator { get; set; } // CraftingInfo.ProjectFacilitator.memberID
            public string StoryConsultant { get; set; }         // CraftingInfo.Consultant.memberID
            public string StoryCoach { get; set; }              // CraftingInfo.Coach.memberID
            public string StoryCrafter { get; set; }            // CraftingInfo.StoryCrafter.memberID
            public int NumberOfLines { get; set; }
            public int NumberOfTestQuestions { get; set; }
            public int NumberOfWords { get; set; }

            internal static string GenerateHeader(List<string> googleDocProjections)
            {
                if ((googleDocProjections.Count == 1) && (googleDocProjections.First() == ProjectionNameAll))
                    AddAllProjectionNames(googleDocProjections);

                var header = ProjectionNameProjectName;
                if (googleDocProjections.Contains(ProjectionNameStoryLanguageName))
                    header += Delim + ProjectionNameStoryLanguageName;
                if (googleDocProjections.Contains(ProjectionNameStorySetName))
                    header += Delim + ProjectionNameStorySetName;
                if (googleDocProjections.Contains(ProjectionNameStoryName))
                    header += Delim + ProjectionNameStoryName;
                if (googleDocProjections.Contains(ProjectionNameStoryStage))
                    header += Delim + ProjectionNameStoryStage;
                if (googleDocProjections.Contains(ProjectionNameStoryPurpose))
                    header += Delim + ProjectionNameStoryPurpose;
                if (googleDocProjections.Contains(ProjectionNameLastEditor))
                    header += Delim + ProjectionNameLastEditor;
                if (googleDocProjections.Contains(ProjectionNameCurrentEditor))
                    header += Delim + ProjectionNameCurrentEditor;
                if (googleDocProjections.Contains(ProjectionNameTimeInStage))
                    header += Delim + ProjectionNameTimeInStage;
                if (googleDocProjections.Contains(ProjectionNameProjectFacilitator))
                    header += Delim + ProjectionNameProjectFacilitator;
                if (googleDocProjections.Contains(ProjectionNameConsultant))
                    header += Delim + ProjectionNameConsultant;
                if (googleDocProjections.Contains(ProjectionNameCoach))
                    header += Delim + ProjectionNameCoach;
                if (googleDocProjections.Contains(ProjectionNameCrafter))
                    header += Delim + ProjectionNameCrafter;
                if (googleDocProjections.Contains(ProjectionNameNumberOfLines))
                    header += Delim + ProjectionNameNumberOfLines;
                if (googleDocProjections.Contains(ProjectionNameNumberOfTestQuestions))
                    header += Delim + ProjectionNameNumberOfTestQuestions;
                if (googleDocProjections.Contains(ProjectionNameNumberOfWords))
                    header += Delim + ProjectionNameNumberOfWords;
                return header;
            }

            private static void AddAllProjectionNames(List<string> googleDocProjections)
            {
                googleDocProjections.Clear();
                googleDocProjections.Add(ProjectionNameStoryLanguageName);
                googleDocProjections.Add(ProjectionNameStorySetName);
                googleDocProjections.Add(ProjectionNameStoryName);
                googleDocProjections.Add(ProjectionNameStoryStage);
                googleDocProjections.Add(ProjectionNameStoryPurpose);
                googleDocProjections.Add(ProjectionNameLastEditor);
                googleDocProjections.Add(ProjectionNameCurrentEditor);
                googleDocProjections.Add(ProjectionNameTimeInStage);
                googleDocProjections.Add(ProjectionNameProjectFacilitator);
                googleDocProjections.Add(ProjectionNameConsultant);
                googleDocProjections.Add(ProjectionNameCoach);
                googleDocProjections.Add(ProjectionNameCrafter);
                googleDocProjections.Add(ProjectionNameNumberOfLines);
                googleDocProjections.Add(ProjectionNameNumberOfTestQuestions);
                googleDocProjections.Add(ProjectionNameNumberOfWords);
            }

            internal string GenerateRow(List<string> googleDocProjections)
            {
                var row = Quote(ProjectName, addPrefixDelimiter: false);
                if (googleDocProjections.Contains(ProjectionNameStoryLanguageName))
                    row += Quote(StoryLanguageName);
                if (googleDocProjections.Contains(ProjectionNameStorySetName))
                    row += Quote(StorySetName);
                if (googleDocProjections.Contains(ProjectionNameStoryName))
                    row += Quote(StoryName);
                if (googleDocProjections.Contains(ProjectionNameStoryStage))
                    row += Quote(StoryStage);
                if (googleDocProjections.Contains(ProjectionNameStoryPurpose))
                    row += Quote(StoryPurpose);
                if (googleDocProjections.Contains(ProjectionNameLastEditor))
                    row += Quote(LastEditor);
                if (googleDocProjections.Contains(ProjectionNameCurrentEditor))
                    row += Quote(CurrentEditor);
                if (googleDocProjections.Contains(ProjectionNameTimeInStage))
                    row += Quote(TimeInStage);
                if (googleDocProjections.Contains(ProjectionNameProjectFacilitator))
                    row += Quote(StoryProjectFacilitator);
                if (googleDocProjections.Contains(ProjectionNameConsultant))
                    row += Quote(StoryConsultant);
                if (googleDocProjections.Contains(ProjectionNameCoach))
                    row += Quote(StoryCoach);
                if (googleDocProjections.Contains(ProjectionNameCrafter))
                    row += Quote(StoryCrafter);
                if (googleDocProjections.Contains(ProjectionNameNumberOfLines))
                    row += Quote(NumberOfLines.ToString());
                if (googleDocProjections.Contains(ProjectionNameNumberOfTestQuestions))
                    row += Quote(NumberOfTestQuestions.ToString());
                if (googleDocProjections.Contains(ProjectionNameNumberOfWords))
                    row += Quote(NumberOfWords.ToString());

                return row;
            }

            internal void SetCurrentEditorAndStageToFriendlyName(string currState)
            {
                CurrentEditor = EditorFromState(currState, out string roleName);
                StoryStage = roleName;
            }

            private string EditorFromState(string currState, out string roleName)
            {
                switch (currState)
                {
                    case "ProjFacTypeVernacular":
                    case "ProjFacTypeNationalBT":
                    case "ProjFacTypeInternationalBT":
                    case "ProjFacTypeFreeTranslation":
                    case "ProjFacAddAnchors":
                    case "ProjFacAddStoryQuestions":
                    case "ProjFacRevisesBeforeUnsTest":
                    case "ProjFacReviseBasedOnRound1Notes":
                    case "ProjFacOnlineReview1WithConsultant":
                    case "ProjFacReadyForTest1":
                    case "ProjFacEnterRetellingOfTest1":
                    case "ProjFacEnterAnswersToStoryQuestionsOfTest1":
                    case "ProjFacRevisesAfterUnsTest":
                        roleName = "Project Facilitator";
                        return StoryProjectFacilitator;

                    case "BackTranslatorTypeInternationalBT":
                    case "BackTranslatorTranslateConNotesBeforeUnsTest":
                    case "BackTranslatorTranslateConNotes":
                    case "BackTranslatorTypeInternationalBTTest1":
                    case "BackTranslatorTranslateConNotesAfterUnsTest":
                        roleName = "BackTranslator";
                        return roleName;

                    case "ConsultantCheckNonBiblicalStory":
                    case "ConsultantCheckStoryInfo":
                    case "ConsultantCheckAnchors":
                    case "ConsultantCheckStoryQuestions":
                    case "ConsultantCauseRevisionBeforeUnsTest":
                    case "ConsultantReviseRound1Notes":
                    case "ConsultantCheck2":
                    case "ConsultantCauseRevisionAfterUnsTest":
                    case "ConsultantFinalCheck":
                        roleName = "Consultant";
                        return StoryConsultant;

                    case "FirstPassMentorCheck1":
                    case "FirstPassMentorCheck2":
                        roleName = "FirstPassMentor";
                        return roleName;

                    case "CoachReviewRound1Notes":
                    case "CoachReviewRound2Notes":
                        roleName = "Coach";
                        return StoryCoach;

                    case "TeamComplete":
                    case "TeamFinalApproval":
                        roleName = "Any Team Member";
                        return roleName;

                    default:
                        roleName = "Unknown!";
                        return roleName;
                }
            }

            private string Quote(string str, bool addPrefixDelimiter = true)
            {
                return (String.IsNullOrEmpty(str))
                        ? String.Empty
                        : String.Format("{0}\"{1}\"",
                                       (addPrefixDelimiter) ? Delim : String.Empty,
                                       str.Replace(Delim, null).Replace("\"", null));
            }
        }


#endif
        private static List<string> ConvertToList(string googleDocProjections)
        {
            // e.g. name,stage,TransitionHistory.StateTransition.LoggedInMemberId,TransitionHistory.StateTransition.TransitionDateTime,CraftingInfo.ProjectFacilitator.memberID
            return googleDocProjections.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static void SyncChorusProjects(string strPathToProjectFile)
        {
            LogMessage(String.Format("Processing the file: '{0}'", strPathToProjectFile));
            var chorusConfig = ChorusConfigurations.Load(strPathToProjectFile);
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
                    appHandler.DoSilentSynchronize();
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, $"Not expecting {appHandler.ButtonLabel}. Should be either Clone or Send/Receive");
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

        /*
        internal static void DoClone()
        {
            var strAdaptItWorkFolder = AdaptItWorkFolder;
            if (!Directory.Exists(strAdaptItWorkFolder))
                Directory.CreateDirectory(strAdaptItWorkFolder);
#if DEBUG
            const string cstrHindiToUrdu = "Hindi to Urdu adaptations";
            var strHindiToUrduProjectFolder = Path.Combine(strAdaptItWorkFolder, cstrHindiToUrdu);
            if (Directory.Exists(strHindiToUrduProjectFolder))
                Directory.Delete(strHindiToUrduProjectFolder, true);
            var strProjectId = "aikb-hindi-urdu";
            var strUsername = "bobeaton";
            var strLocalFolderName = cstrHindiToUrdu;
            var strServerName = Resources.IDS_DefaultRepoServer;
            var strPassword = "helpmepld";
#endif
            CloneProject(strLocalFolderName, strServerName, strUsername, strAdaptItWorkFolder, strPassword, strProjectId);
        }
        */

        internal static bool CloneProject(ServerSetting serverSetting, Project project, string strProjectFolderRoot)
        {
            return CloneProject(project.FolderName, serverSetting.ServerName, serverSetting.Username,
                         strProjectFolderRoot, serverSetting.DecryptedPassword, project.ProjectId);
        }

        private static bool CloneProject(string strLocalFolderName, string strServerName, string strUsername,
                                         string strProjectFolderRoot, string strPassword, string strProjectId)
        {
            if (!Directory.Exists(strProjectFolderRoot))
                Directory.CreateDirectory(strProjectFolderRoot);

            var model = new GetCloneFromInternetModel(strProjectFolderRoot)
            {
                ProjectId = strProjectId,
                Username = strUsername,
                Password = strPassword,
                LocalFolderName = strLocalFolderName
            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.Cancel == dlg.ShowDialog())
                    return false;

                var strProjectFolder = dlg.PathToNewlyClonedFolder;
                Settings.Default.LastProjectFolder = strProjectFolder;
                Settings.Default.Save();
            }
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
            string strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            MessageBox.Show(strErrorMsg, Resources.AiChorusCaption);
        }

        public static void LaunchProgram(string strProgramPath, string strArguments)
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
                        Arguments = "\"" + strArguments + "\"",
                        WorkingDirectory = strWorkingDir
                    }
                };

                myProcess.Start();
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
