using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Chorus.UI.Sync;
using Chorus.Utilities;
using Chorus.VcsDrivers;
using Chorus.VcsDrivers.Mercurial;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.RecentProjects == null)
                Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.RecentProjectPaths == null)
                Properties.Settings.Default.RecentProjectPaths = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.ProjectNameToHgUrl == null)
                Properties.Settings.Default.ProjectNameToHgUrl = new System.Collections.Specialized.StringCollection();
            _mapProjectNameToHgUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUrl);

            if (Properties.Settings.Default.ProjectNameToHgUsername == null)
                Properties.Settings.Default.ProjectNameToHgUsername = new System.Collections.Specialized.StringCollection();
            _mapProjectNameToHgUsername = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUsername);

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

            try
            {
                // one of the first things this might do is try to get a project from the internet, in which case
                //  the OneStory folder should exist
                if (!Directory.Exists(ProjectSettings.OneStoryProjectFolderRoot))
                    Directory.CreateDirectory(ProjectSettings.OneStoryProjectFolderRoot);

                bool bPretendOpening = false;
                if ((args.Length > 0) && (args[0] == "/sync_all"))
                {
                    foreach (string strProjectName in _mapProjectNameToHgUrl.Keys)
                    {
                        string strProjectPath = Path.Combine(
                            ProjectSettings.OneStoryProjectFolderRoot, 
                            strProjectName);
                        _astrProjectForSync.Add(strProjectPath);
                    }
                    bPretendOpening = true; // this triggers minimal Sync UI
                }
                else
                    Application.Run(new StoryEditor(Properties.Resources.IDS_MainStoriesSet));

                foreach (string strProjectFolder in _astrProjectForSync)
                {
                    string strOneStoryFileSpec = String.Format(@"{0}\{1}.onestory",
                        strProjectFolder, Path.GetFileNameWithoutExtension(strProjectFolder));
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
                MessageBox.Show(strMessage, Properties.Resources.IDS_Caption);
            }
        }

        private static void SetupErrorHandling()
        {
            Logger.Init();
            ErrorReport.EmailAddress = "bob_eaton@sall.com";//TODO Change this address
            ErrorReport.AddStandardProperties();
            ExceptionHandler.Init();
        }

        static List<string> _astrProjectForSync = new List<string>();
        static Dictionary<string, string> _mapProjectNameToHgUrl;
        static Dictionary<string, string> _mapProjectNameToHgUsername;

        public static void SetHgParameters(string strProjectFolder, string strProjectName, string strUrl, string strUsername)
        {
            System.Diagnostics.Debug.Assert((_mapProjectNameToHgUrl != null) && (_mapProjectNameToHgUsername != null));
            _mapProjectNameToHgUrl[strProjectName] = strUrl;
            _mapProjectNameToHgUsername[strProjectName] = strUsername;
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgUrl);
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();

            var repo = new HgRepository(strProjectFolder, new NullProgress());

            var address = RepositoryAddress.Create("Internet", strUrl);
            var addresses = repo.GetRepositoryPathsInHgrc();
            foreach (var addr in addresses)
                if (addr.URI == address.URI)
                    return;

            var lstAddrs = new List<RepositoryAddress>(addresses);
            lstAddrs.Add(address);
            repo.SetKnownRepositoryAddresses(lstAddrs);
        }

        public static bool ShouldTrySync(string strProjectFolder)
        {
            return ((strProjectFolder.Length > ProjectSettings.OneStoryProjectFolderRoot.Length)
                    && (strProjectFolder.Substring(0, ProjectSettings.OneStoryProjectFolderRoot.Length) ==
                     ProjectSettings.OneStoryProjectFolderRoot));
        }

        public static void SetProjectForSyncage(string strProjectFolder)
        {
            // add it to the list to be sync'd, but only if it is in the OneStory data folder
            if (!_astrProjectForSync.Contains(strProjectFolder)
                && ShouldTrySync(strProjectFolder))
            {
                _astrProjectForSync.Add(strProjectFolder);
            }
        }

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test

        public static void SyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // if there's no repo yet, then create one (even if we aren't going
            //  to ultimately push with an internet repo, we still want one locally)
            var projectConfig = new Chorus.sync.ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them

            string strHgUsername, strRepoUrl;
            if (QueryHgRepoParameters(strProjectFolder, strProjectName, out strHgUsername, out strRepoUrl))
            {
                // for when we launch the program, just do a quick & dirty send/receive, but for
                //  closing, we can be more informative
                SyncUIDialogBehaviors suidb;
                SyncUIFeatures suif;
                if (bIsOpening)
                {
                    suidb = SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished;
                    suif = SyncUIFeatures.Minimal;

                    var nullProgress = new NullProgress();
                    var repo = new HgRepository(strProjectFolder, nullProgress);
                    if (!repo.GetCanConnectToRemote(strRepoUrl, nullProgress))
                        if (MessageBox.Show(Properties.Resources.IDS_ConnectToInternet, 
                            Properties.Resources.IDS_Caption, MessageBoxButtons.OKCancel) == 
                            DialogResult.Cancel)
                            return;
                }
                else
                {
                    suidb = SyncUIDialogBehaviors.Lazy;
                    suif = SyncUIFeatures.NormalRecommended;
                }

                using (var dlg = new SyncDialog(projectConfig, suidb, suif))
                {
                    dlg.SyncOptions.DoSendToOthers = true;
                    dlg.SyncOptions.DoPullFromOthers = true;
                    dlg.SyncOptions.DoMergeWithOthers = true;
                    dlg.UseTargetsAsSpecifiedInSyncOptions = true;

                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create("Internet",strRepoUrl));
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

        private static bool QueryHgRepoParameters(string strProjectFolder, string strProjectName, out string strUsername, out string strRepoUrl)
        {
            string strHgUrl = (_mapProjectNameToHgUrl.ContainsKey(strProjectName))
                ? _mapProjectNameToHgUrl[strProjectName] : null;
            string strHgUsername = (_mapProjectNameToHgUsername.ContainsKey(strProjectName))
                ? _mapProjectNameToHgUsername[strProjectName] : null;
            if (String.IsNullOrEmpty(strHgUrl))
            {
                HgRepoForm dlg = new HgRepoForm
                                        {
                                            ProjectName = strProjectName,
                                            UrlBase = "http://hg-private.languagedepot.org",
                                            Username = strHgUsername
                                        };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    strRepoUrl = dlg.Url;
                    strUsername = dlg.Username;
                    SetHgParameters(strProjectFolder, strProjectName, strRepoUrl, strUsername);
                    return true;
                }
            }
            else
            {
                strRepoUrl = strHgUrl;
                strUsername = strHgUsername;
                return true;
            }

            strUsername = strRepoUrl = null;
            return false;
        }

        public static Dictionary<string, string> ArrayToDictionary(System.Collections.Specialized.StringCollection data)
        { 
            var map = new Dictionary<string, string>(); 
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1]);
            }
            
            return map; 
        }

        public static System.Collections.Specialized.StringCollection DictionaryToArray(Dictionary<string, string> map)
        {
            var lst = new System.Collections.Specialized.StringCollection();
            foreach (KeyValuePair<string,string> kvp in map)
            {
                lst.Add(kvp.Key);
                lst.Add(kvp.Value);
            }

            return lst;
        }
    }
}
