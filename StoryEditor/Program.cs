using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
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
                // do auto-upgrade handling
                InitializeLocalSettingsCollections(true);
                
                // make sure we have HG (or we can't really do much)
                HgSanityCheck();
#if !DEBUG
                AutoUpgrade autoUpgrade = AutoUpgrade.Create(Properties.Resources.IDS_OSEUpgradeServer);
                if (autoUpgrade.IsUpgradeAvailable(true))
                    return;
#endif

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                
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

                bool bPretendOpening = false;
                if ((args.Length > 0) && (args[0] == "/sync_all"))
                {
                    foreach (string strProjectName in _mapProjectNameToHgHttpUrl.Keys)
                    {
                        string strProjectPath = Path.Combine(
                            ProjectSettings.OneStoryProjectFolderRoot, 
                            strProjectName);
                        _astrProjectForSync.Add(strProjectPath);
                    }
                    bPretendOpening = true; // this triggers minimal Sync UI
                }
                else
                    Application.Run(new StoryEditor(OseResources.Properties.Resources.IDS_MainStoriesSet));

                foreach (string strProjectFolder in _astrProjectForSync)
                {
                    string strOneStoryFileSpec = Path.Combine(strProjectFolder, 
                        ProjectSettings.OneStoryFileName(Path.GetFileNameWithoutExtension(strProjectFolder)));

                    try
                    {
                        System.Diagnostics.Debug.Assert(File.Exists(strOneStoryFileSpec));
                        SyncWithRepository(strProjectFolder, bPretendOpening);
                    }
                    catch (Exception ex)
                    {
                        string strMessage = String.Format("Error occurred trying to Send/Receive to the Internet:{0}{0}{1}", Environment.NewLine, ex.Message);
                        if (ex.InnerException != null)
                            strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                        strMessage += String.Format("{0}Please click the 'Send Email' button to get support help", 
                            Environment.NewLine, strOneStoryFileSpec);
                        ErrorReport.ReportNonFatalException(new Exception(strMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private static void HgSanityCheck()
        {
            var msg = HgRepository.GetEnvironmentReadinessMessage("en");
            if (!string.IsNullOrEmpty(msg))
                throw new ApplicationException("It looks like you don't have TortoiseHg installed. Please install that first before trying to use the OneStory Editor");
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
        }

        private static void SetupErrorHandling()
        {
            Logger.Init();
            ErrorReport.EmailAddress = "bob_eaton@sall.com";//TODO Change this address
            ErrorReport.AddStandardProperties();
            ExceptionHandler.Init();
        }

        static List<string> _astrProjectForSync = new List<string>();
        static Dictionary<string, string> _mapProjectNameToHgHttpUrl;
        static Dictionary<string, string> _mapProjectNameToHgUsername;
        static Dictionary<string, string> _mapProjectNameToHgNetworkUrl;

        private const string CstrInternetName = "Internet";
        private const string CstrNetworkDriveName = "Network Drive";

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
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        public static void SetHgParametersNetworkDrive(string strProjectFolder, string strProjectName, string strUrl)
        {
            System.Diagnostics.Debug.Assert(_mapProjectNameToHgNetworkUrl != null);
            _mapProjectNameToHgNetworkUrl[strProjectName] = strUrl;
            Properties.Settings.Default.ProjectNameToHgNetworkUrl = DictionaryToArray(_mapProjectNameToHgNetworkUrl);
            Properties.Settings.Default.Save();

            var repo = new HgRepository(strProjectFolder, new NullProgress());

            var address = RepositoryAddress.Create(CstrNetworkDriveName, strUrl);
            var addresses = repo.GetRepositoryPathsInHgrc();
            foreach (var addr in addresses)
                if (addr.URI == address.URI)
                    return;

            var lstAddrs = new List<RepositoryAddress>(addresses);
            lstAddrs.Add(address);
            repo.SetKnownRepositoryAddresses(lstAddrs);
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

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test
        // or \\Bob-StudioXPS\Backup\Storying\snwmtn-test
        public static void SyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            // the project folder name has come here bogus at times...
            if (!Directory.Exists(strProjectFolder))
                return;

            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = new Chorus.sync.ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file

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
                
                /*
                if (bIsOpening && String.IsNullOrEmpty(strSharedNetworkUrl))
                {
                    suidb = SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished;
                    suif = SyncUIFeatures.Minimal;
                }
                else
                {
                    suidb = SyncUIDialogBehaviors.Lazy;
                    suif = SyncUIFeatures.NormalRecommended;
                }
                */

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

            if (!String.IsNullOrEmpty(strHgUrl))
            {
                strRepoUrl = strHgUrl;
                strUsername = strHgUsername;
                return true;
            }

            strUsername = strRepoUrl = null;
            return false;
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
            var projectConfig = new Chorus.sync.ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file

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
    }
}
