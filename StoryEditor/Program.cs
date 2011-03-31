using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.Utilities;
using Chorus.VcsDrivers;
using Chorus.VcsDrivers.Mercurial;
using devX;
using Palaso.Reporting;

namespace OneStoryProjectEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            SetupErrorHandling();

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var splashScreen = new SplashScreenForm();
                splashScreen.Show();
                Application.DoEvents();

                // do auto-upgrade handling
                InitializeLocalSettingsCollections(true);

                // make sure we have HG (or we can't really do much)
                HgSanityCheck();

                bool bNeedToSave = false;
                System.Diagnostics.Debug.Assert(Properties.Settings.Default.RecentProjects.Count == Properties.Settings.Default.RecentProjectPaths.Count);
                if (Properties.Settings.Default.RecentProjects.Count != Properties.Settings.Default.RecentProjectPaths.Count)
                {
                    Properties.Settings.Default.RecentProjects.Clear();
                    Properties.Settings.Default.RecentProjectPaths.Clear();
                    bNeedToSave = true;
                }

                if (bNeedToSave)
                    Properties.Settings.Default.Save();

                ProjectSettings.InsureOneStoryProjectFolderRootExists();

                if ((args.Length > 0) && (args[0] == "/sync_all"))
                {
                    foreach (string strProjectName in _mapProjectNameToHgHttpUrl.Keys)
                    {
                        string strProjectPath = Path.Combine(
                            ProjectSettings.OneStoryProjectFolderRoot,
                            strProjectName);
                        _astrProjectForSync.Add(strProjectPath);
                    }
                    SyncBeforeClose(true);
                }
                else
                {
                    splashScreen.Close();
                    string strFilePathToOpen = null;
                    if (args.Length > 0)
                    {
                        strFilePathToOpen = args[0];
                    }
                    Application.Run(new StoryEditor(OseResources.Properties.Resources.IDS_MainStoriesSet, strFilePathToOpen));
                }
            }
            catch (RestartException)
            {
                return;
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        public static void SyncBeforeClose(bool bPretendOpening)
        {
            foreach (string strProjectFolder in _astrProjectForSync)
                SyncWithRepository(strProjectFolder, bPretendOpening);

            if (_mapAiProjectsToSync != null)
                foreach (KeyValuePair<string, string> kvp in _mapAiProjectsToSync)
                    SyncWithAiRepository(kvp.Value, kvp.Key, bPretendOpening);
        }

        internal class RestartException : Exception
        {
        }

        internal static void CheckForProgramUpdate(bool bThrowErrors)
        {
            string strManifestAddress = Properties.Resources.IDS_OSEUpgradeServer;
            /*
            strManifestAddress = @"ftp://Bob_Eaton:tsc2009@ftp.seedconnect.org/Testing/StoryEditor.exe.manifest.xml";
            strManifestAddress = 
                @"\\StudioXPS-1340\src\StoryEditor\OneStory Releases\OSE1.4.0\StoryEditor.exe.manifest.xml";
            */
            AutoUpgrade autoUpgrade = AutoUpgrade.Create(strManifestAddress, bThrowErrors);
            if (autoUpgrade.IsUpgradeAvailable(false))
            {
                // if this is the automatic check at startup, then at 
                //  least confirm this is what the user wants to do.
                if (!bThrowErrors && (MessageBox.Show(Properties.Resources.IDS_ConfirmAutoUpgrade, 
                        OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes))
                {
                    return;
                }

                autoUpgrade.StartUpgradeStub();
                throw new RestartException();
            }
        }

        private static void HgSanityCheck()
        {
            var msg = HgRepository.GetEnvironmentReadinessMessage("en");
            if (!string.IsNullOrEmpty(msg))
                throw new ApplicationException("It looks like you don't have TortoiseHg installed. Please install that first before trying to use the OneStory Editor (if you did install it, perhaps you need to reboot)");
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
            if (Properties.Settings.Default.RecentFindWhat == null)
                Properties.Settings.Default.RecentFindWhat = new StringCollection();
            if (Properties.Settings.Default.RecentReplaceWith == null)
                Properties.Settings.Default.RecentReplaceWith = new StringCollection();
            
            if (Properties.Settings.Default.ProjectNameToHgUrl == null)
                Properties.Settings.Default.ProjectNameToHgUrl = new StringCollection();
            _mapProjectNameToHgHttpUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUrl);

            if (Properties.Settings.Default.ProjectNameToHgUsername == null)
                Properties.Settings.Default.ProjectNameToHgUsername = new StringCollection();
            _mapProjectNameToHgUsername = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUsername);

            if (Properties.Settings.Default.ProjectNameToHgNetworkUrl == null)
                Properties.Settings.Default.ProjectNameToHgNetworkUrl = new StringCollection();
            _mapProjectNameToHgNetworkUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgNetworkUrl);

            if (Properties.Settings.Default.ProjectNameToAiHgUrl == null)
                Properties.Settings.Default.ProjectNameToAiHgUrl = new StringCollection();
            _mapProjectNameToAiHgHttpUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToAiHgUrl);

            if (Properties.Settings.Default.ProjectNameToAiHgNetworkUrl == null)
                Properties.Settings.Default.ProjectNameToAiHgNetworkUrl = new StringCollection();
            _mapProjectNameToAiHgNetworkUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToAiHgNetworkUrl);

            _mapServerToUrl = ArrayToDictionary(Properties.Settings.Default.AdaptItDefaultServerLabels);
            _mapSwordModuleToFont = ArrayToDictionary(Properties.Settings.Default.SwordModuleToFont);
        }

        private static void SetupErrorHandling()
        {
            Logger.Init();
            ErrorReport.EmailAddress = "bob_eaton@sall.com";//TODO Change this address
            ErrorReport.AddStandardProperties();
            ExceptionHandler.Init();
        }

        static List<string> _astrProjectForSync = new List<string>();
        static Dictionary<string, string> _mapAiProjectsToSync;
        static Dictionary<string, string> _mapProjectNameToHgHttpUrl;
        static Dictionary<string, string> _mapProjectNameToHgUsername;
        static Dictionary<string, string> _mapProjectNameToHgNetworkUrl;
        static Dictionary<string, string> _mapProjectNameToAiHgHttpUrl;
        static Dictionary<string, string> _mapProjectNameToAiHgNetworkUrl;
        public static Dictionary<string, string> _mapServerToUrl;
        public static Dictionary<string, string> _mapSwordModuleToFont;
        
        private const string CstrInternetName = "Internet";
        private const string CstrNetworkDriveName = "Network Drive";

        private static EventWaitHandle EventForProjectName;

        public static void InsureSingleInstanceOfProgramName(StoryEditor theSe)
        {
            if (EventForProjectName != null)
                ResetSingleInstanceLock();

            bool bCreatedNew;
            string strEventName = theSe.GetFrameTitle();
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

        public static void ClearHgParameters(string strProjectName)
        {
            System.Diagnostics.Debug.Assert((_mapProjectNameToHgHttpUrl != null) && (_mapProjectNameToHgUsername != null));
            _mapProjectNameToHgHttpUrl.Remove(strProjectName);
            _mapProjectNameToHgUsername.Remove(strProjectName);
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgHttpUrl);
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();
        }

        public static void SetHgParameters(string strProjectFolder, string strProjectName, string strUrl, string strUsername)
        {
            System.Diagnostics.Debug.Assert((_mapProjectNameToHgHttpUrl != null) && (_mapProjectNameToHgUsername != null));
            _mapProjectNameToHgHttpUrl[strProjectName] = strUrl;
            _mapProjectNameToHgUsername[strProjectName] = strUsername;
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgHttpUrl);
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();

            // var repo = new HgRepository(strProjectFolder, new NullProgress());
            try
            {
                var repo = HgRepository.CreateOrLocate(strProjectFolder, new NullProgress());

                var address = RepositoryAddress.Create(CstrInternetName, strUrl);
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
                ShowException(ex);
            }
        }

        public static void ShowException(Exception ex)
        {
            string strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            MessageBox.Show(strErrorMsg, OseResources.Properties.Resources.IDS_Caption);
        }

        public static void SetHgParametersNetworkDrive(string strProjectFolder, string strProjectName, string strUrl)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToHgNetworkUrl != null);
            _mapProjectNameToHgNetworkUrl[strProjectName] = strUrl;
            Properties.Settings.Default.ProjectNameToHgNetworkUrl = DictionaryToArray(_mapProjectNameToHgNetworkUrl);
            Properties.Settings.Default.Save();

            try
            {
                var repo = HgRepository.CreateOrLocate(strProjectFolder, new NullProgress());
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
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        public static string LookupRepoUrl(string strServerName)
        {
            string strUrl;
            return _mapServerToUrl.TryGetValue(strServerName, out strUrl) ? strUrl : null;
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

        public static void SetAdaptItHgParameters(string strProjectFolder, 
            string strProjectName, string strRepoUrl)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToAiHgHttpUrl != null);
            _mapProjectNameToAiHgHttpUrl[strProjectName] = strRepoUrl;
            Properties.Settings.Default.ProjectNameToAiHgUrl = DictionaryToArray(_mapProjectNameToAiHgHttpUrl);
            Properties.Settings.Default.Save();

            try
            {
                var repo = HgRepository.CreateOrLocate(strProjectFolder, new NullProgress());

                var address = RepositoryAddress.Create(CstrInternetName, strRepoUrl);
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
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
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
                var repo = HgRepository.CreateOrLocate(strProjectFolder, new NullProgress());
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
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
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

            // add it to the list to be sync'd, but only if it is in the OneStory data folder
            if (!_mapAiProjectsToSync.ContainsKey(strProjectName))
                _mapAiProjectsToSync.Add(strProjectName, strProjectFolder);
        }

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test
        // or \\Bob-StudioXPS\Backup\Storying\snwmtn-test
        public static void SyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            // the project folder name has come here bogus at times...
            if (!Directory.Exists(strProjectFolder))
                return;

            try
            {
                TrySyncWithRepository(strProjectFolder, bIsOpening);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private static void TrySyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);

            string strHgUsername, strRepoUrl, strSharedNetworkUrl;
            if (GetHgRepoParameters(strProjectName, out strHgUsername, out strRepoUrl, out strSharedNetworkUrl))
            {
                if (!String.IsNullOrEmpty(strRepoUrl))
                {
                    var nullProgress = new NullProgress();
                    var repo = new HgRepository(strProjectFolder, nullProgress);
                    if (!repo.GetCanConnectToRemote(strRepoUrl, nullProgress))
                        if (MessageBox.Show(Properties.Resources.IDS_ConnectToInternet,
                                            OseResources.Properties.Resources.IDS_Caption,
                                            MessageBoxButtons.OKCancel) ==
                            DialogResult.Cancel)
                        {
                            strRepoUrl = null;
                            if (String.IsNullOrEmpty(strSharedNetworkUrl))
                                return;
                        }
                }

                // for when we launch the program, just do a quick & dirty send/receive, 
                //  but for closing (or if we have a network drive also), then we want to 
                //  be more informative
                SyncUIDialogBehaviors suidb = SyncUIDialogBehaviors.Lazy;
                SyncUIFeatures suif = SyncUIFeatures.NormalRecommended;
                using (var dlg = new SyncDialog(projectConfig, suidb, suif))
                {
                    dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                    if (!String.IsNullOrEmpty(strRepoUrl))
                        dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrInternetName, strRepoUrl));
                    if (!String.IsNullOrEmpty(strSharedNetworkUrl))
                        dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrNetworkDriveName, strSharedNetworkUrl));

                    dlg.Text = "Synchronizing OneStory Project: " + strProjectName;
                    dlg.ShowDialog();
                }
            }
            else if (!bIsOpening)
            {
                // even if the user doesn't want to go to the internet, we
                //  at least want to back up locally (when the user closes)
                using (var dlg = new SyncDialog(projectConfig,
                                                SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
                                                SyncUIFeatures.Minimal))
                {
                    dlg.Text = "OneStory Automatic Backup";
                    dlg.SyncOptions.DoMergeWithOthers = false;
                    dlg.SyncOptions.DoPullFromOthers = false;
                    dlg.SyncOptions.DoSendToOthers = false;
                    dlg.ShowDialog();
                }
            }
        }

        public static bool HaveCalledAdaptIt;

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/aikb-{0}-{1}
        // or \\Bob-StudioXPS\Backup\Storying\aikb-{0}-{1}
        public static void SyncWithAiRepository(string strProjectFolder, string strProjectName, 
            bool bIsOpening)
        {
            // the project folder name has come here bogus at times...
            if (String.IsNullOrEmpty(strProjectFolder))
                return;

            if (!Directory.Exists(strProjectFolder))
                Directory.CreateDirectory(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = new Chorus.sync.ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.xml"); // AI KB
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            projectConfig.IncludePatterns.Add("*.aic");
            projectConfig.IncludePatterns.Add("*.cct"); // possible normalization spellfixer files

            string strRepoUrl, strSharedNetworkUrl;
            if (GetAiHgRepoParameters(strProjectName, out strRepoUrl, out strSharedNetworkUrl))
            {
                try
                {
                    if (!String.IsNullOrEmpty(strSharedNetworkUrl) && !Directory.Exists(strSharedNetworkUrl))
                        Directory.CreateDirectory(strSharedNetworkUrl);
                }
                catch { }

                if (!String.IsNullOrEmpty(strRepoUrl))
                {
                    var nullProgress = new NullProgress();
                    var repo = new HgRepository(strProjectFolder, nullProgress);
                    if (!repo.GetCanConnectToRemote(strRepoUrl, nullProgress))
                        if (MessageBox.Show(Properties.Resources.IDS_ConnectToInternet,
                                             OseResources.Properties.Resources.IDS_Caption,
                                             MessageBoxButtons.OKCancel) ==
                             DialogResult.Cancel)
                        {
                            strRepoUrl = null;
                            if (String.IsNullOrEmpty(strSharedNetworkUrl))
                                return;
                        }
                }

                if (HaveCalledAdaptIt)
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
                        catch { }
                    }
                }

                SyncUIDialogBehaviors suidb = SyncUIDialogBehaviors.Lazy;
                SyncUIFeatures suif = SyncUIFeatures.NormalRecommended;
                using (var dlg = new SyncDialog(projectConfig, suidb, suif))
                {
                    dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                    if (!String.IsNullOrEmpty(strRepoUrl))
                        dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrInternetName, strRepoUrl));
                    if (!String.IsNullOrEmpty(strSharedNetworkUrl))
                        dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrNetworkDriveName, strSharedNetworkUrl));

                    dlg.Text = "Synchronizing Adapt It Project: " + strProjectName;
                    dlg.ShowDialog();
                }
            }
            else if (!bIsOpening)
            {
                // even if the user doesn't want to go to the internet, we
                //  at least want to back up locally (when the user closes)
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

        public static bool QueryHgRepoParameters(string strProjectFolder, string strProjectName)
        {
            HgRepoForm dlg = new HgRepoForm
            {
                ProjectName = strProjectName,
                UrlBase = "http://hg-private.languagedepot.org",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetHgParameters(strProjectFolder, strProjectName, dlg.Url, dlg.Username);
                return true;
            }
            return false;
        }

        public static bool GetHgRepoParameters(string strProjectName, 
            out string strUsername, out string strRepoUrl, out string strSharedNetworkUrl)
        {
            string strHgUrl = (_mapProjectNameToHgHttpUrl.ContainsKey(strProjectName))
                ? _mapProjectNameToHgHttpUrl[strProjectName] : null;
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

        private static ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            var projectConfig = new ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            return projectConfig;
        }

        public static void SyncWithRepositoryThumbdrive(string strProjectFolder)
        {
            if (String.IsNullOrEmpty(strProjectFolder))
                return;

            if (!Directory.Exists(strProjectFolder))
                Directory.CreateDirectory(strProjectFolder);

            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);

            SyncUIDialogBehaviors suidb = SyncUIDialogBehaviors.Lazy;
            SyncUIFeatures suif = SyncUIFeatures.NormalRecommended;
            using (var dlg = new SyncDialog(projectConfig, suidb, suif))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                dlg.Text = "Synchronizing OneStory Project: " + strProjectName;
                dlg.ShowDialog();
            }
        }
    }
}
