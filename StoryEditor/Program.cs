#define UseOseServer

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Chorus.sync;
using Chorus.VcsDrivers.Mercurial;
using Microsoft.Win32;
using SIL.Progress;
using devX;
using MAPIEx;
using NetLoc;
using SIL.Email;
using SIL.Reporting;
using SilEncConverters40;
using SIL.Keyboarding;
using SIL.Windows.Forms.Reporting;
using System.Linq;
using Chorus.Model;
using Chorus.UI.Sync;
using Chorus.UI.Clone;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OneStoryProjectEditor
{
    static class Program
    {
        private const string ProjectTypeOneStory = "OneStory";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetupErrorHandling();

            try
            {
                var splashScreen = new SplashScreenForm();
                splashScreen.Show();
                Application.DoEvents();

                // do auto-upgrade handling
                InitializeLocalSettingsCollections(true);

                // make sure we have HG (or we can't really do much)
                HgSanityCheck();

                bool bNeedToSave = false;
                System.Diagnostics.Debug.Assert(Properties.Settings.Default.RecentProjects.Count ==
                                                Properties.Settings.Default.RecentProjectPaths.Count);
                if (Properties.Settings.Default.RecentProjects.Count !=
                    Properties.Settings.Default.RecentProjectPaths.Count)
                {
                    Properties.Settings.Default.RecentProjects.Clear();
                    Properties.Settings.Default.RecentProjectPaths.Clear();
                    bNeedToSave = true;
                }

                if (bNeedToSave)
                    Properties.Settings.Default.Save();

                ProjectSettings.InsureOneStoryProjectFolderRootExists();

                // add registry key to force IE9 (which supports double-click selects word
                AddIe9RegistryKey();

                string strFilePathToOpen = null;
                try
                {
                    if (args.Length > 0)
                    {
                        if (args[0] == "/sync_all")
                        {
                            var projectPathsWorkedOn = MapProjectNameToLastStoriesSetWorkedOn.Keys
                                                    .Select(projId => Path.Combine(ProjectSettings.OneStoryProjectFolderRoot, projId))
                                                    .ToList();
                            _astrProjectForSync.AddRange(projectPathsWorkedOn);
                            SyncBeforeClose(true);
                            return;
                        }
                        else if (args[0] == "/sync")
                        {
                            var projectFolder = args[1];
                            var projectType = args[2];
                            var isOpening = (args[3] == bool.TrueString);
                            if (projectType == ProjectTypeOneStory)
                                SyncWithOneStoryRepository(projectFolder, isOpening);
                            else
                            {
                                var projectName = args[4];
                                SyncWithAiRepository(projectFolder, projectName, isOpening, true);
                            }
                            return;
                        }
                        else if (args[0] == "/clone")
                        {
                            var projectName = args[1];
                            var parentDirToPutCloneIn = args[2];
                            var localFolder = args[3];
                            var username = args[4];
                            var password = args[5];
                            CloneRepository(projectName, parentDirToPutCloneIn, localFolder,
                                            username, password);
                            return;
                        }
                        else
                        {
                            strFilePathToOpen = args[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
                finally
                {
                    splashScreen.Close();
                }

                // if we sent some new locdata items in a recent program update, then copy them to the
                //  loc data folder
                var strPathToLocData = Path.Combine(ProjectSettings.OneStoryProjectFolderRoot,
                    "LocData");

                if (!Directory.Exists(strPathToLocData))
                    Directory.CreateDirectory(strPathToLocData);

                var strRunningLocData = Path.Combine(StoryProjectData.GetRunningFolder,
                    "LocData");
                if (Directory.Exists(strRunningLocData))
                {
                    var astrLocDataFiles = Directory.GetFiles(strRunningLocData, "*.xml");
                    foreach (var file in astrLocDataFiles)
                    {
                        var filename = Path.GetFileName(file);
                        var targetFilename = Path.Combine(strPathToLocData, filename);
                        if (!File.Exists(targetFilename) ||
                            (File.GetLastWriteTime(file) > File.GetLastWriteTime(targetFilename)))
                        {
                            File.Copy(file, targetFilename, true);
                        }
                    }
                }

                Localizer.Default = new Localizer(strPathToLocData,
                    Properties.Settings.Default.LastLocalizationId);
                Localizer.LocalizerStrUseStack = false;
                if (Properties.Settings.Default.LastLocalizationId != "en")
                    StoryEditor.OnLocalizationChangeStatic();

                // after we've loaded the localization (in case the msg has to be localized), 
                //  check for a ready install before doing anything
                if (AutoUpgrade.IsUpgradeReadyToInstall() &&
                    (LocalizableMessageBox.Show(
                        Localizer.Str("There is an update available. Would you like to install it now?"),
                        StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes))
                {
                    AutoUpgrade.LaunchUpgrade();
                    return;
                }

#if !TurnOnKeyboarding
                SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
#endif
                Application.Run(new StoryEditor(strFilePathToOpen));
            }
            catch (RestartException)
            {
                return;
            }
            catch (DuplicateStoryStateTransitionException)
            {
                // clobber this so it doesn't automatically try to load next time
                Properties.Settings.Default.LastProject = null;
                Properties.Settings.Default.Save();
                // pass this one on so it triggers an email
                throw;
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                LocalizableMessageBox.Show(strMessage, StoryEditor.OseCaption);

                // clobber this so it doesn't automatically try to load next time
                Properties.Settings.Default.LastProject = null;
                Properties.Settings.Default.Save();
            }
            finally
            {
#if !TurnOnKeyboarding
                SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
#endif
            }
        }

        public static (GetCloneFromInternetModel model, GetCloneFromInternetDialog dlg)
                        CloneRepository(string projectName, string parentDirToPutCloneIn, string localFolder, 
                                        string username, string password, string customUrl = null)
        {
            if (!Directory.Exists(parentDirToPutCloneIn))
                Directory.CreateDirectory(parentDirToPutCloneIn);

            // projectName may come in from AiChorus, but otherwise, will be null, but show the user something
            //  so they know they have to enter something in the custom URL and in the local folder name
            const string InstructionsForEnteringProjectId = "ENTER PROJECT ID HERE";
            var strProjectName = projectName ?? InstructionsForEnteringProjectId;
            var model = new GetCloneFromInternetModel(parentDirToPutCloneIn)
            {
                ProjectId = projectName,
                IsCustomUrl = true, // all OSE projects are custom
                CustomUrl = customUrl ?? $"{Properties.Resources.IDS_DefaultRepoUrl}{strProjectName}",
                LocalFolderName = localFolder ?? InstructionsForEnteringProjectId,
                Username = username,
                Password = password,
                // default, and setting it explicitly makes it stand out, which some users might want to argue with
                // Bandwidth = ServerSettingsModel.Bandwidths[0],  // this mean 'low' bandwidth, cuz if we say 'high', Chorus defaults to hg-public, which doesn't work for us
            };

            var dlg = new GetCloneFromInternetDialog(model);
            dlg.ShowDialog();
            return (model, dlg);
        }

        public static string UpgradeCacheDir
        {
            get { return Path.Combine(Application.UserAppDataPath, "upgrade-cache"); }
        }

        public static void ActivateDefaultKeyboard()
        {
            try
            {
                Keyboard.Controller.ActivateDefaultKeyboard();
            }
            catch
            {
                // on come computers, this happens all the time and I don't know why... just ignore errors
            }
        }

        private static void AddIe9RegistryKey()
        {
            var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
            if (key == null)
                return;

            key.SetValue("StoryEditor.exe", 9999, RegistryValueKind.DWord);
        }

        public static void SyncBeforeClose(bool bPretendOpening)
        {
            foreach (string strProjectFolder in _astrProjectForSync)
                SyncWithOneStoryRepository(strProjectFolder, bPretendOpening);

            if (_mapAiProjectsToSync != null)
                foreach (KeyValuePair<string, string> kvp in _mapAiProjectsToSync)
                    SyncWithAiRepository(kvp.Value, kvp.Key, bPretendOpening, true);
        }

        internal class RestartException : Exception
        {
        }

#if UseOseServer
        public static string IDS_OSEUpgradeServer = $"ftp://OseProgram:OseAccess!2O@{Properties.Settings.Default.OseServerIpAddress}/OSE4.2/StoryEditor.exe.manifest.xml";
        public static string IDS_OSEUpgradeServerNextMajorUpgrade = $"ftp://OseProgram:OseAccess!2O@{Properties.Settings.Default.OseServerIpAddress}/OSE4.3/StoryEditor.exe.manifest.xml";
        public static string IDS_OSEUpgradeServerSword = $"ftp://OseProgram:OseAccess!2O@{Properties.Settings.Default.OseServerIpAddress}";
        public static string IDS_OSEUpgradeServerTest = $"ftp://OseProgram:OseAccess!2O@{Properties.Settings.Default.OseServerIpAddress}/Test/testmanifest.xml";
#elif UseSeedCo
        public const string IDS_OSEUpgradeServer = "ftp://Bob_Eaton:tsc2009@ftp.seedconnect.org/OSE3.2/StoryEditor.exe.manifest.xml";
        public const string IDS_OSEUpgradeServerNextMajorUpgrade = "ftp://Bob_Eaton:tsc2009@ftp.seedconnect.org/OSE3.3/StoryEditor.exe.manifest.xml";
        public const string IDS_OSEUpgradeServerSword = "ftp://Bob_Eaton:tsc2009@ftp.seedconnect.org";
        public const string IDS_OSEUpgradeServerTest = "ftp://Bob_Eaton:tsc2009@ftp.seedconnect.org/Test/testmanifest.xml";
#elif UsePalasoFtp
        public const string IDS_OSEUpgradeServer = "ftp://onestory:yrotseno23@palaso.org/OseUpdates/OSE3.2/StoryEditor.exe.manifest.xml";
        public const string IDS_OSEUpgradeServerNextMajorUpgrade = "ftp://onestory:yrotseno23@palaso.org/OseUpdates/OSE3.3/StoryEditor.exe.manifest.xml";
        public const string IDS_OSEUpgradeServerSword = "ftp://onestory:yrotseno23@palaso.org";
        public const string IDS_OSEUpgradeServerTest = "ftp://onestory:yrotseno23@palaso.org/OseUpdates/Test/testmanifest.xml";
#endif

        internal static void CheckForProgramUpdate(AutoUpgrade autoUpgrade, string strManifestAddress)
        {
            if ((autoUpgrade == null) || (autoUpgrade.SourcePath != strManifestAddress))
            {
                autoUpgrade = AutoUpgrade.Create(strManifestAddress, true);
                if (!autoUpgrade.IsUpgradeAvailable())
                    return;
            }

            autoUpgrade.StartUpgradeStub();
            throw new RestartException();
        }

        public static void DisplayNewUpdatesMessage()
        {
            LocalizableMessageBox.Show(
                Localizer.Str(
                    "There are new program updates available, which will be installed when the program next launches."),
                StoryEditor.OseCaption);
        }

        private static void HgSanityCheck()
        {
            Environment.CurrentDirectory = StoryProjectData.GetRunningFolder;
            var msg = HgRepository.GetEnvironmentReadinessMessage("en");
            if (!string.IsNullOrEmpty(msg))
                throw new ApplicationException(msg);
        }

        public static void InitializeLocalSettingsCollections(bool bDoUpgrade)
        {
            // see if this is perhaps an upgrade from a previous version (i.e. when we change from 1.3.5.* to 1.4.0.*
            //  then we lose the user settings file which is stored in: 
            //  C:\Documents and Settings\Bob\Local Settings\Application Data\SIL\StoryEditor...\1.3.5.0\user.config
            if (bDoUpgrade && Properties.Settings.Default.UpgradeSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
            }

            if (Properties.Settings.Default.RecentProjects == null)
                Properties.Settings.Default.RecentProjects = new StringCollection();
            if (Properties.Settings.Default.RecentProjectPaths == null)
                Properties.Settings.Default.RecentProjectPaths = new StringCollection();
            if (Properties.Settings.Default.SwordModulesUsed == null)
                Properties.Settings.Default.SwordModulesUsed = new StringCollection();
            if (Properties.Settings.Default.SwordSourcesToExclude == null)
                Properties.Settings.Default.SwordSourcesToExclude = new StringCollection();

            // regardless of what happens, we *have* to have the Net bible (the only one guaranteed
            //  to be here)
            if (!Properties.Settings.Default.SwordModulesUsed.Contains(NetBibleViewer.CstrNetModuleName))
                Properties.Settings.Default.SwordModulesUsed.Add(NetBibleViewer.CstrNetModuleName);

            if (Properties.Settings.Default.RecentFindWhat == null)
                Properties.Settings.Default.RecentFindWhat = new StringCollection();
            if (Properties.Settings.Default.RecentReplaceWith == null)
                Properties.Settings.Default.RecentReplaceWith = new StringCollection();
            if (Properties.Settings.Default.ListSwordModuleToRtl == null)
                Properties.Settings.Default.ListSwordModuleToRtl = new StringCollection();

#if UseUrlsWithChorus
            if (Properties.Settings.Default.ProjectNameToHgUrl == null)
                Properties.Settings.Default.ProjectNameToHgUrl = new StringCollection();
            _mapProjectNameToHgHttpUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUrl);

            if (Properties.Settings.Default.ProjectNameToAiHgUrl == null)
                Properties.Settings.Default.ProjectNameToAiHgUrl = new StringCollection();
            _mapProjectNameToAiHgHttpUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToAiHgUrl);

            if (Properties.Settings.Default.ProjectNameToHgUsername == null)
                Properties.Settings.Default.ProjectNameToHgUsername = new StringCollection();
            _mapProjectNameToHgUsername = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUsername);

            if (Properties.Settings.Default.ProjectNameToHgNetworkUrl == null)
                Properties.Settings.Default.ProjectNameToHgNetworkUrl = new StringCollection();
            _mapProjectNameToHgNetworkUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgNetworkUrl);

            if (Properties.Settings.Default.ProjectNameToAiHgNetworkUrl == null)
                Properties.Settings.Default.ProjectNameToAiHgNetworkUrl = new StringCollection();
            _mapProjectNameToAiHgNetworkUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToAiHgNetworkUrl);

            MapServerToUrlHost = ArrayToDictionary(Properties.Settings.Default.AdaptItDefaultServerLabels);
#endif

            if (Properties.Settings.Default.ProjectNameToLastStoryWorkedOn == null)
                Properties.Settings.Default.ProjectNameToLastStoryWorkedOn = new StringCollection();
            MapProjectNameToLastStoryWorkedOn = ArrayToDictionary(Properties.Settings.Default.ProjectNameToLastStoryWorkedOn);

            if (Properties.Settings.Default.ProjectNameToLastStoriesSetWorkedOn == null)
                Properties.Settings.Default.ProjectNameToLastStoriesSetWorkedOn = new StringCollection();
            MapProjectNameToLastStoriesSetWorkedOn = ArrayToDictionary(Properties.Settings.Default.ProjectNameToLastStoriesSetWorkedOn);

            if (Properties.Settings.Default.ProjectNameToLastMemberLogin == null)
                Properties.Settings.Default.ProjectNameToLastMemberLogin = new StringCollection();
            MapProjectNameToLastMemberLogin = ArrayToDictionary(Properties.Settings.Default.ProjectNameToLastMemberLogin);

            if (Properties.Settings.Default.ProjectNameToLastUserType == null)
                Properties.Settings.Default.ProjectNameToLastUserType = new StringCollection();
            MapProjectNameToLastUserType = ArrayToDictionary(Properties.Settings.Default.ProjectNameToLastUserType);

            MapSwordModuleToFont = ArrayToDictionary(Properties.Settings.Default.SwordModuleToFont);
            MapSwordModuleToEncryption = ArrayToDictionary(Properties.Settings.Default.SwordModuleToUnlockKey);
        }

        private static void SetupErrorHandling()
        {
            Logger.Init();
            ErrorReport.EmailAddress = "bob_eaton@sall.com";//TODO Change this address
            ErrorReport.AddStandardProperties();
            ExceptionHandler.Init(new WinFormsExceptionHandler());
        }

        public static void TrySendEmail(string strEmailAddress, string strSubjectLine, 
            string strBodyText)
        {
            try
            {
                SendEmail(strEmailAddress, strSubjectLine, strBodyText);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private static void SendEmail(string strEmailAddress, string strSubjectLine, string strBodyText)
        {
            if (Properties.Settings.Default.EmailUseSendGrid)
            {
                var apiKey = EncryptionClass.Decrypt(Properties.Settings.Default.EncryptedSendGridApiKey);
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("OsMetaData@gmail.com", "OneStory Editor");
                var to = new EmailAddress(strEmailAddress, "OSE User");
                var msg = MailHelper.CreateSingleEmail(from, to, strSubjectLine, strBodyText, String.Empty);
                var response = client.SendEmailAsync(msg).Result;   //.ConfigureAwait(false).GetAwaiter().GetResult();
                Logger.WriteMinorEvent($"Just sent email (subj line: {strSubjectLine}) to {strEmailAddress} w/ response: {response?.StatusCode}");

                if (response?.IsSuccessStatusCode ?? false)
                    return;
            }

            if (Properties.Settings.Default.UseMapiPlus)
            {
                if (!NetMAPI.Init())
                    return;

                var mapi = new NetMAPI();
                if (mapi.Login())
                {
                    var strSenderEmail = new StringBuilder(NetMAPI.DefaultBufferSize);
                    mapi.GetProfileEmail(strSenderEmail);
                    if (mapi.OpenMessageStore())
                    {
                        if (mapi.OpenOutbox())
                        {
                            var message = new MAPIMessage();
                            if (message.Create(mapi, MAPIMessage.Importance.IMPORTANCE_NORMAL))
                            {
                                // No need to add Sender Details as per updated API.
                                //message.SetSender(StoryEditor.OseCaption, strSenderEmail.ToString());
                                message.SetSubject(strSubjectLine);
                                message.SetBody(strBodyText);
                                message.AddRecipient(strEmailAddress);
                                message.Send();
                            }
                        }
                    }
                    mapi.Logout();
                }
                NetMAPI.Term();
            }
            else
            {
                var emailProvider = EmailProviderFactory.PreferredEmailProvider();
                var emailMessage = emailProvider.CreateMessage();
                emailMessage.To.Add(strEmailAddress);
                emailMessage.Subject = strSubjectLine;
                emailMessage.Body = strBodyText;
                emailMessage.Send(emailProvider);
            }
        }

        static List<string> _astrProjectForSync = new List<string>();
        static Dictionary<string, string> _mapAiProjectsToSync;
#if UseUrlsWithChorus
        static Dictionary<string, string> _mapProjectNameToHgHttpUrl;
        static Dictionary<string, string> _mapProjectNameToAiHgHttpUrl;
        public static Dictionary<string, string> MapServerToUrlHost;
        static Dictionary<string, string> _mapProjectNameToHgUsername;
        static Dictionary<string, string> _mapProjectNameToHgNetworkUrl;
        static Dictionary<string, string> _mapProjectNameToAiHgNetworkUrl;
#endif

        public static Dictionary<string, string> MapProjectNameToLastStoriesSetWorkedOn;
        public static Dictionary<string, string> MapProjectNameToLastStoryWorkedOn;
        public static Dictionary<string, string> MapProjectNameToLastMemberLogin;
        public static Dictionary<string, string> MapProjectNameToLastUserType;

        public static Dictionary<string, string> MapSwordModuleToFont;
        public static Dictionary<string, string> MapSwordModuleToEncryption;
        
        public const string CstrChorusPathForInternetRepoName = "languageforge.org";
        public const string CstrNetworkDriveName = "Network Drive";

        private static EventWaitHandle EventForProjectName;

        public static void InsureSingleInstanceOfProgramName(string strProjectName)
        {
            if (EventForProjectName != null)
                ResetSingleInstanceLock();

            bool bCreatedNew;
            // don't localize the event name (we don't want to allow it if one
            //  is using a localization and the other isn't)
            string strEventName = String.Format("OneStory Editor -- {0}",
                                                strProjectName);
            EventForProjectName = new EventWaitHandle(false,
                EventResetMode.ManualReset, strEventName, out bCreatedNew);
            if (!bCreatedNew)
                throw new RestartException();
        }

        public static void ResetSingleInstanceLock()
        {
            if (EventForProjectName != null)
                EventForProjectName.Close();
            EventForProjectName = null;
        }

#if UseUrlsWithChorus
        public static void ClearHgParameters(string strProjectName)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToHgUsername != null);
            _mapProjectNameToHgHttpUrl.Remove(strProjectName);
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgHttpUrl);
            _mapProjectNameToHgUsername.Remove(strProjectName);
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();
        }

        public static void SetHgParameters(string strProjectFolder, string strProjectName, string strUrl, string strUsername)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToHgUsername != null);
            _mapProjectNameToHgHttpUrl[strProjectName] = strUrl;
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgHttpUrl);
            _mapProjectNameToHgUsername[strProjectName] = strUsername;
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();

            // CleanupHgrc(strProjectFolder, strUrl);
        }

        // this method gets rid of anything in the hgrc file that isn't what we want
        private static void CleanupHgrc(string strProjectFolder, string strUrl)
        {
            try
            {
                var repo = HgRepository.CreateOrUseExisting(strProjectFolder, new NullProgress());

                var address = RepositoryAddress.Create(CstrInternetName, strUrl);
                var addresses = repo.GetRepositoryPathsInHgrc();
                var addressesToRemain = new List<RepositoryAddress>();
                foreach (var addr in addresses)
                    if ((addr.UserName == address.UserName) &&
                        (addr.Name == address.Name) &&
                        (addr.Password != address.Password))
                    {
                        // means the person changed the password, so clobber the entry
                        //  so it gets added below
                        continue;
                    }
                    else if (addr.Name != CstrInternetName)
                        // chorus puts in a 'LanguageDepot', which we don't want
                        continue;
                    else if ((addr.Name == address.Name) &&
                             (addr.URI != address.URI))
                        // change from e.g. private.languagedepot.org to resumable
                        continue;
                    else
                        addressesToRemain.Add(addr);

                addressesToRemain.Add(address);
                repo.SetKnownRepositoryAddresses(addressesToRemain);
                // TODO: we might want to do repo.SetIsOneDefaultSyncAddresses( address, true);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }
#endif

        public static void ShowException(Exception ex)
        {
            string strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            LocalizableMessageBox.Show(strErrorMsg, StoryEditor.OseCaption);
        }

#if UseUrlsWithChorus
        public static void SetHgParametersNetworkDrive(string strProjectFolder, string strProjectName, string strUrl)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToHgNetworkUrl != null);
            _mapProjectNameToHgNetworkUrl[strProjectName] = strUrl;
            Properties.Settings.Default.ProjectNameToHgNetworkUrl = DictionaryToArray(_mapProjectNameToHgNetworkUrl);
            Properties.Settings.Default.Save();

            try
            {
                var repo = HgRepository.CreateOrUseExisting(strProjectFolder, new NullProgress());
                var address = RepositoryAddress.Create(CstrNetworkDriveName, strUrl);
                var addresses = repo.GetRepositoryPathsInHgrc();
                foreach (var addr in addresses)
                    if (addr.URI == address.URI)
                        return;

                var lstAddrs = new List<RepositoryAddress>(addresses);
                lstAddrs.Add(address);
                repo.SetKnownRepositoryAddresses(lstAddrs);
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                LocalizableMessageBox.Show(strMessage, StoryEditor.OseCaption);
            }
        }

        public static string LookupRepoUrlHost(string strServerName)
        {
            string strUrl;
            return MapServerToUrlHost.TryGetValue(strServerName, out strUrl)
                       ? strUrl
                       : null;
        }

        public static string LookupSharedNetworkPath(string strProjectFolder)
        {
            string strSharedNetworkPath;
            return _mapProjectNameToHgNetworkUrl.TryGetValue(strProjectFolder, out strSharedNetworkPath) ? strSharedNetworkPath : null;
        }

        public static bool AreAdaptItHgParametersSet(string strProjectFolder)
        {
            return _mapProjectNameToAiHgHttpUrl.ContainsKey(strProjectFolder);
        }

        public static void SetAdaptItHgParameters(string strProjectFolder, string strProjectName,
            string strServerName, string strHgUsername, string strHgPassword)
        {
            // for the AI project, the Url we saved didn't have account info, so add it now.
            string strRepoUrl = AiRepoSelectionForm.GetFullInternetAddress(strServerName, strProjectName);
            if (String.IsNullOrEmpty(strRepoUrl))
                return;

            var uri = new Uri(strRepoUrl);
            strRepoUrl = String.Format("{0}://{1}{2}@{3}/{4}",
                                       uri.Scheme, strHgUsername,
                                       (String.IsNullOrEmpty(strHgPassword)) ? null : ':' + strHgPassword,
                                       uri.Host, strProjectName);

            SetAdaptItHgParameters(strProjectFolder, strProjectName, strRepoUrl);
        }

        public static void SetAdaptItHgParameters(string strProjectFolder, string strProjectName
            , string strRepoUrl
        )
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToAiHgHttpUrl != null);
            _mapProjectNameToAiHgHttpUrl[strProjectName] = strRepoUrl;
            Properties.Settings.Default.ProjectNameToAiHgUrl = DictionaryToArray(_mapProjectNameToAiHgHttpUrl);
            Properties.Settings.Default.Save();

            try
            {
                var repo = HgRepository.CreateOrUseExisting(strProjectFolder, new NullProgress());

                var address = RepositoryAddress.Create(CstrInternetName, strRepoUrl);   // newChorus: use https://resumable.languageforge.org
                var addresses = repo.GetRepositoryPathsInHgrc();
                foreach (var addr in addresses)
                    if (addr.URI == address.URI)
                        return;

                var lstAddrs = new List<RepositoryAddress>(addresses);
                lstAddrs.Add(address);
                repo.SetKnownRepositoryAddresses(lstAddrs);

                // newChorus: ServerSettingsModel w/ username password => InitFromPath => SaveSettings;
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                LocalizableMessageBox.Show(strMessage, StoryEditor.OseCaption);
            }
        }

        public static void SetAdaptItHgParametersNetworkDrive(string strProjectFolder, 
            string strProjectName, string strUrl)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToAiHgNetworkUrl != null);
            strUrl = AiRepoSelectionForm.GetFullNetworkAddress(strUrl, strProjectName);

            _mapProjectNameToAiHgNetworkUrl[strProjectName] = strUrl;
            Properties.Settings.Default.ProjectNameToAiHgNetworkUrl = DictionaryToArray(_mapProjectNameToAiHgNetworkUrl);
            Properties.Settings.Default.Save();

            try
            {
                var repo = HgRepository.CreateOrUseExisting(strProjectFolder, new NullProgress());
                var address = RepositoryAddress.Create(CstrNetworkDriveName, strUrl);
                var addresses = repo.GetRepositoryPathsInHgrc();
                foreach (var addr in addresses)
                    if (addr.URI == address.URI)
                        return;

                var lstAddrs = new List<RepositoryAddress>(addresses);
                lstAddrs.Add(address);
                repo.SetKnownRepositoryAddresses(lstAddrs);
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                LocalizableMessageBox.Show(strMessage, StoryEditor.OseCaption);
            }
        }

        // this is too dangerous. if an MTT moves the file or accidentally sets the 
        //  the root to something else, we stop backing up...
        /*
        public static bool ShouldTrySync(string strProjectFolder)
        {
            return ((strProjectFolder.Length > ProjectSettings.OneStoryProjectFolderRoot.Length)
                    && (strProjectFolder.Substring(0, ProjectSettings.OneStoryProjectFolderRoot.Length) 
                        == ProjectSettings.OneStoryProjectFolderRoot));
        }
        */
#endif
        public static void SetProjectForSyncage(string strProjectFolder)
        {
            // add it to the list to be sync'd, but only if it is in the OneStory data folder
            if (!_astrProjectForSync.Contains(strProjectFolder))
                _astrProjectForSync.Add(strProjectFolder);
        }

        public static void SetAiProjectForSyncage(string strProjectFolder, string strProjectName)
        {
            if (_mapAiProjectsToSync == null)
                _mapAiProjectsToSync = new Dictionary<string, string>();

            // add it to the list to be sync'd
            if (!_mapAiProjectsToSync.ContainsKey(strProjectName))
                _mapAiProjectsToSync.Add(strProjectName, strProjectFolder);
        }

        delegate ProjectFolderConfiguration ProjectFolderConfigurationFunc(string strFolderPath);

        public static void SyncWithOneStoryRepository(string strProjectFolder, bool bIsOpening)
        {
            var strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);
            TrySyncWithRepository(strProjectFolder, strProjectName, bIsOpening, ProjectTypeOneStory, GetProjectFolderConfiguration);
        }

        public static void SyncWithAiRepository(string strProjectFolder, string strProjectName, bool bIsOpening, bool bRewriteAdaptItKb)
        {
            if (bRewriteAdaptItKb)
            {
                // AdaptIt creates the xml file in a different way than we'd like (it
                //  triggers a whole file change set). So before we attempt to merge, let's
                //  resave the file using the same approach that Chorus will use if a merge
                //  is done so it'll always show differences only.
                string strKbFilename = Path.Combine(strProjectFolder,
                                                    Path.GetFileNameWithoutExtension(strProjectFolder) + ".xml");
                if (File.Exists(strKbFilename))
                {
                    try
                    {
                        string strKbBackupFilename = strKbFilename + ".bak";
                        File.Copy(strKbFilename, strKbBackupFilename, true);
                        XDocument doc = XDocument.Load(strKbBackupFilename);
                        File.Delete(strKbFilename);
                        doc.Save(strKbFilename);
                    }
                    catch
                    {
                    }
                }
            }

            TrySyncWithRepository(strProjectFolder, strProjectName, bIsOpening, "Adapt It", GetAiProjectFolderConfiguration);
        }

        private static void TrySyncWithRepository(string strProjectFolder, string strProjectName, bool bIsOpening, string projectType,
                                                  ProjectFolderConfigurationFunc projectFolderConfigurationFunc)
        {
            // the project folder name has come here bogus at times...
            if (String.IsNullOrEmpty(strProjectFolder) || String.IsNullOrEmpty(strProjectName))
                return;

            if (!Directory.Exists(strProjectFolder))
                Directory.CreateDirectory(strProjectFolder);

            try
            {
                // if there's no repo yet, then create one (even if we aren't going
                //  to ultimately push with an internet repo, we still want one locally)
                var hgFolderPath = PathToHgRepoFolder(strProjectFolder);

                if (!Directory.Exists(hgFolderPath))
                {
                    HgRepository.CreateOrUseExisting(strProjectFolder, new NullProgress());
                }

                var projectConfig = projectFolderConfigurationFunc(strProjectFolder);

                if (!bIsOpening)
                {
                    // even if the user doesn't want to go to the internet, we
                    //  at least want to back up locally (when the user closes)
                    using (var dlg = new SyncDialog(projectConfig,
                                                    SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
                                                    SyncUIFeatures.Minimal))
                    {
                        dlg.Text = $"{projectType} Automatic Backup";
                        dlg.SyncOptions.DoMergeWithOthers = false;
                        dlg.SyncOptions.DoPullFromOthers = false;
                        dlg.SyncOptions.DoSendToOthers = false;
                        dlg.ShowDialog();
                        if (!dlg.SyncResult.Succeeded)
                        {
                            throw new ApplicationException($"Silent sync failed for {strProjectFolder}. Reason:", dlg.SyncResult.ErrorEncountered);
                        }
                    }
                    return;
                }

                var strRepoUrl = InternetRepoUrl(strProjectFolder, out NullProgress nullProgress, out HgRepository repo, out List<Chorus.VcsDrivers.RepositoryAddress> paths);

                if (String.IsNullOrEmpty(strRepoUrl))
                {
                    throw new ApplicationException(String.Format(Localizer.Str("The repository config file:{0}'{1}\\.hg\\hgrc'{0}{0}doesn't contain a path to the server to use for this project. It should contain something like:{0}{0}[paths]{0}hg-private.languageforge.org = https://hg-private.languageforge.org/{2}{0}Click 'Project', 'Settings' and see if you can configure it with the Internet Repository 'Configure' button"),
                                                                 Environment.NewLine, strProjectFolder, strProjectName));
                }
                else if (!repo.GetCanConnectToRemote(strRepoUrl, nullProgress))
                {
                    if (UserCancelledNotConnectToInternetWarning)
                    {
                        strRepoUrl = null;
                        if (paths.FirstOrDefault(path => path.Name == CstrNetworkDriveName) == null)
                            return;
                    }
                }

                // for when we launch the program, just do a quick & dirty send/receive, 
                //  but for closing (or if we have a network drive also), then we want to 
                //  be more informative
                using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.Lazy, SyncUIFeatures.NormalRecommended))
                {
                    dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                    dlg.Text = $"Synchronizing {projectType} Project: " + strProjectName;
                    dlg.ShowDialog();
                    if (!dlg.SyncResult.Succeeded && (dlg.SyncResult.ErrorEncountered != null))
                    {
                        throw new ApplicationException($"Sync failed for {strProjectFolder}. Reason:", dlg.SyncResult.ErrorEncountered);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        public static bool HasInternetRepo(string strProjectFolder)
        {
            var strRepoUrl = InternetRepoUrl(strProjectFolder, out NullProgress nullProgress, out HgRepository repo, out List<Chorus.VcsDrivers.RepositoryAddress> paths);
            return strRepoUrl != null;
        }

        public static string InternetRepoUrl(string strProjectFolder, out NullProgress nullProgress, out HgRepository repo, out List<Chorus.VcsDrivers.RepositoryAddress> paths)
        {
            nullProgress = new NullProgress();
            repo = new HgRepository(strProjectFolder, nullProgress);
            paths = repo.GetRepositoryPathsInHgrc().ToList();
            var pathToInternet = paths.FirstOrDefault(path => path.Name.ToLower().Contains(CstrChorusPathForInternetRepoName)) ??
paths.FirstOrDefault();    // just in case someone is using their own repo, let's give it a try

            return pathToInternet?.URI;
        }

        public static bool UserCancelledNotConnectToInternetWarning
        {
            get
            {
                return (LocalizableMessageBox.Show(Localizer.Str("You should connect to the internet now so we can download the latest version of the file (in case some of your team made changes)"),
                                        StoryEditor.OseCaption,
                                        MessageBoxButtons.OKCancel) ==
                        DialogResult.Cancel);
            }
        }

        public static bool HaveCalledAdaptIt;

#if UseUrlsWithChorus
        public static void SyncWithAiRepository(string strProjectFolder, string strProjectName, bool bIsOpening)
        {
            // the project folder name has come here bogus at times...
            if (String.IsNullOrEmpty(strProjectFolder))
                return;

            if (!Directory.Exists(strProjectFolder))
                Directory.CreateDirectory(strProjectFolder);

            //string strRepoUrl, strSharedNetworkUrl;
            //if (GetAiHgRepoParameters(strProjectName, out strRepoUrl, out strSharedNetworkUrl))
            if (bIsOpening)
            {
                SyncWithAiRepo(strProjectFolder, strProjectName, strRepoUrl, strSharedNetworkUrl, HaveCalledAdaptIt);
            }
            else if (!bIsOpening)
            {
                // even if the user doesn't want to go to the internet, we
                //  at least want to back up locally (when the user closes)
                var projectConfig = GetAiProjectFolderConfiguration(strProjectFolder);
                using (var dlg = new SyncDialog(projectConfig,
                    SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
                    SyncUIFeatures.Minimal))
                {
                    dlg.Text = "Adapt It Automatic Backup";
                    dlg.SyncOptions.DoMergeWithOthers = false;
                    dlg.SyncOptions.DoPullFromOthers = false;
                    dlg.SyncOptions.DoSendToOthers = false;
                    dlg.ShowDialog();
                }
            }
        }

        public static void SyncWithAiRepository(string strProjectFolder, string strProjectName, string strRepoUrl,
                                                string strSharedNetworkUrl, string strHgUsername, string strHgPassword, 
                                                bool bRewriteAdaptItKb)
        {
            strRepoUrl = FormHgUrl(strRepoUrl, strHgUsername, strHgPassword, strProjectName);
            // CleanupHgrc(strProjectFolder, strRepoUrl);
            SyncWithAiRepo(strProjectFolder, strProjectName, strRepoUrl, strSharedNetworkUrl, bRewriteAdaptItKb);
        }

        private static void SyncWithAiRepo(string strProjectFolder, string strProjectName, string strRepoUrl,
                                          string strSharedNetworkUrl, bool bRewriteAdaptItKb)
        {
            if (bRewriteAdaptItKb)
            {
                // AdaptIt creates the xml file in a different way than we'd like (it
                //  triggers a whole file change set). So before we attempt to merge, let's
                //  resave the file using the same approach that Chorus will use if a merge
                //  is done so it'll always show differences only.
                string strKbFilename = Path.Combine(strProjectFolder,
                                                    Path.GetFileNameWithoutExtension(strProjectFolder) + ".xml");
                if (File.Exists(strKbFilename))
                {
                    try
                    {
                        string strKbBackupFilename = strKbFilename + ".bak";
                        File.Copy(strKbFilename, strKbBackupFilename, true);
                        XDocument doc = XDocument.Load(strKbBackupFilename);
                        File.Delete(strKbFilename);
                        doc.Save(strKbFilename);
                    }
                    catch
                    {
                    }
                }
            }

            var projectConfig = GetAiProjectFolderConfiguration(strProjectFolder);

            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.Lazy, SyncUIFeatures.NormalRecommended))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                if (!String.IsNullOrEmpty(strRepoUrl))
                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrInternetName, strRepoUrl));
                if (!String.IsNullOrEmpty(strSharedNetworkUrl))
                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrNetworkDriveName,
                                                                                        strSharedNetworkUrl));

                dlg.Text = "Synchronizing Adapt It Project: " + strProjectName;
                dlg.ShowDialog();
            }
        }
#endif

        private static ProjectFolderConfiguration GetAiProjectFolderConfiguration(string strProjectFolder)
        {
            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = GetProjectFolderConfigurationInternal(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.xml"); // AI KB
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            projectConfig.IncludePatterns.Add("*.aic");
            projectConfig.IncludePatterns.Add("*.cct"); // possible normalization spellfixer files
            projectConfig.ExcludePatterns.Add("*.bak"); // created as a result of the sorting xslt process
            return projectConfig;
        }

#if UseUrlsWithChorus
        public static bool QueryHgRepoParameters(string strProjectFolder,
            string strProjectName, TeamMemberData loggedOnMember)
        {
            string strUsername = null, strPassword = null;
            loggedOnMember?.GetHgParameters(out strUsername, out strPassword);

            var dlg = new HgRepoForm
            {
                ProjectName = strProjectName,
                UrlBase = LookupRepoUrlHost(Properties.Resources.IDS_DefaultRepoServer),
                Username = strUsername,
                Password = strPassword
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TeamMemberData.SetHgParameters(loggedOnMember, dlg.Username, dlg.Password);
                SetHgParameters(strProjectFolder, strProjectName, dlg.Url, dlg.Username);
                return true;
            }
            return false;
        }

        public static bool GetHgRepoParameters(string strProjectName, 
            out string strUsername, out string strRepoUrl, out string strSharedNetworkUrl)
        {
            string strHgUrl = GetHgRepoFullUrl(strProjectName);
            string strHgUsername = (_mapProjectNameToHgUsername.ContainsKey(strProjectName))
                ? _mapProjectNameToHgUsername[strProjectName] : null;

            if (_mapProjectNameToHgNetworkUrl.ContainsKey(strProjectName))
                strSharedNetworkUrl = _mapProjectNameToHgNetworkUrl[strProjectName];
            else
                strSharedNetworkUrl = null;

            if (!String.IsNullOrEmpty(strHgUrl) || !String.IsNullOrEmpty(strSharedNetworkUrl))
            {
                strRepoUrl = strHgUrl;
                strUsername = strHgUsername;
                return true;
            }

            strUsername = strRepoUrl = null;
            return false;
        }

        public static string GetHgRepoFullUrl(string strProjectName)
        {
            return (_mapProjectNameToHgHttpUrl.ContainsKey(strProjectName))
                       ? _mapProjectNameToHgHttpUrl[strProjectName]
                       : null;
        }

        public static bool GetAiHgRepoParameters(string strProjectName,
            out string strRepoUrl, out string strSharedNetworkUrl)
        {
            strRepoUrl = (_mapProjectNameToAiHgHttpUrl.ContainsKey(strProjectName))
                ? _mapProjectNameToAiHgHttpUrl[strProjectName] : null;

            if (_mapProjectNameToAiHgNetworkUrl.ContainsKey(strProjectName))
                strSharedNetworkUrl = _mapProjectNameToAiHgNetworkUrl[strProjectName];
            else
                strSharedNetworkUrl = null;

            return !String.IsNullOrEmpty(strRepoUrl) || !String.IsNullOrEmpty(strSharedNetworkUrl);
        }
#endif

        public static Dictionary<string, string> ArrayToDictionary(StringCollection data)
        { 
            var map = new Dictionary<string, string>(); 
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1]);
            }
            
            return map; 
        }

        public static StringCollection DictionaryToArray(Dictionary<string, string> map)
        {
            var lst = new StringCollection();
            foreach (KeyValuePair<string,string> kvp in map)
            {
                lst.Add(kvp.Key);
                lst.Add(kvp.Value);
            }

            return lst;
        }

#if UseUrlsWithChorus
        public static void BackupInRepo(string strProjectFolder)
        {
            // the project folder name has come here bogus at times...
            if (!Directory.Exists(strProjectFolder))
                return;

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);

            // even if the user doesn't want to go to the internet, we
            //  at least want to back up locally (when the user closes)
            using (var dlg = new SyncDialog(projectConfig,
                SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
                SyncUIFeatures.Minimal))
            {
                dlg.Text = "OneStory Automatic Backup... Please wait";
                dlg.SyncOptions.DoMergeWithOthers = false;
                dlg.SyncOptions.DoPullFromOthers = false;
                dlg.SyncOptions.DoSendToOthers = false;
                dlg.ShowDialog();
            }
        }
#endif

        private static ProjectFolderConfiguration GetProjectFolderConfigurationInternal(string strProjectFolder)
        {
            // until some future version of Chorus, we might need to programmatically trigger
            //  the update to the hgrc file... check if that's needed
            var hgrcPath = Path.Combine(PathToHgRepoFolder(strProjectFolder), "hgrc");
            var hgrcLines = File.ReadAllLines(hgrcPath);
            var languageDepotPaths = hgrcLines.Where(l => l.Contains("languagedepot.org")).ToList();
            foreach (var url in languageDepotPaths.Select(p => p.Substring(p.LastIndexOf("http")).Trim()))
            {
                GetUserInfoFromUrl(url, out string strUsername, out string strPassword);

                var model = new ServerSettingsModel()
                {
                    Username = strUsername,
                    Password = strPassword
                };
                model.InitFromProjectPath(strProjectFolder);
                model.SaveSettings();
            }
            return new ProjectFolderConfiguration(strProjectFolder);
        }

        private static void GetUserInfoFromUrl(string url, out string strUsername, out string strPassword)
        {
            var uri = new Uri(url);
            strUsername = strPassword = null;
            if (!String.IsNullOrEmpty(uri.UserInfo) && (uri.UserInfo.IndexOf(':') != -1))
            {
                var astrUserInfo = uri.UserInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                strUsername = astrUserInfo[0];
                strPassword = astrUserInfo[1];
            }
        }

        public static string PathToHgRepoFolder(string strProjectFolder)
        {
            return Path.Combine(strProjectFolder, ".hg");
        }

        public static ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            var projectConfig = GetProjectFolderConfigurationInternal(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            return projectConfig;
        }

#if UseUrlsWithChorus
        public static string FormHgUrl(string strUrlBase, string strUsername,
            string strHgPassword, string strProjectName)
        {
            if (String.IsNullOrEmpty(strUrlBase))
                return null;

            var uri = new Uri(strUrlBase);
            string strHgRepoUrl;
            if (!String.IsNullOrEmpty(strUsername))
            {
                strHgRepoUrl = String.Format("{0}://{1}{2}@{3}/{4}",
                                             uri.Scheme, strUsername,
                                             (String.IsNullOrEmpty(strHgPassword))
                                                 ? null
                                                 : ':' + strHgPassword,
                                             uri.Host,
                                             strProjectName);
            }
            else
            {
                strHgRepoUrl = String.Format("{0}://{1}/{2}",
                                             uri.Scheme,
                                             uri.Host,
                                             strProjectName);
            }
            return strHgRepoUrl;
        }

        public static void SyncWithRepositoryThumbdrive(string strProjectFolder)
        {
            if (String.IsNullOrEmpty(strProjectFolder) || !Directory.Exists(strProjectFolder))
                return;

            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);

            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.Lazy, SyncUIFeatures.NormalRecommended))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                dlg.Text = "Synchronizing OneStory Project: " + strProjectName;
                dlg.ShowDialog();
            }
        }
#endif

        public static void InitializeLangCheckBoxes(ProjectSettings.LanguageInfo li,
            CheckBox cbLang, CheckBox cbLangTransliterator, ToolStripMenuItem tsmi,
            DirectableEncConverter dec)
        {
            if (li.HasData)
            {
                cbLang.Text = String.Format(Localizer.Str("{0} &language fields"),
                                            li.LangName);
                if (dec != null)
                {
                    cbLangTransliterator.Visible = true;
                    cbLangTransliterator.Checked = tsmi.Checked;
                }
                else
                    cbLangTransliterator.Visible =
                        cbLangTransliterator.Checked = false;
            }
            else
                cbLang.Visible = cbLang.Checked = false;            
        }
    }
}
