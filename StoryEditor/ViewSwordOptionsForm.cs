#define UsingCSwordRemoteManager
#define UseFluentFtp
#define UseOseServer

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetLoc;
using Sword;
#if UseFluentFtp
using FluentFTP;
#else
using Starksoft.Net.Ftp;
#endif
using devX;

namespace OneStoryProjectEditor
{
    public partial class ViewSwordOptionsForm : TopForm
    {
        protected const string CstrSwordLink = "www.crosswire.org/sword/modules/ModDisp.jsp?modType=Bibles";

#if UsingCSwordRemoteManager
        protected List<ModInfo> _lstBibleResources;
        protected List<Module> _lstBibleCommentaries;
#else
        protected List<NetBibleViewer.SwordResource> _lstBibleResources;
#endif
        protected int _nIndexOfNetBible = -1;
        protected bool _bInCtor;

        private ViewSwordOptionsForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

#if UsingCSwordRemoteManager
        public ViewSwordOptionsForm(ref List<ModInfo> lstBibleResources, List<Module> lstBibleCommentaries)
#else
        public ViewSwordOptionsForm(ref List<NetBibleViewer.SwordResource> lstBibleResources)
#endif
        {
            _lstBibleResources = lstBibleResources;
            _lstBibleCommentaries = lstBibleCommentaries;

            InitializeComponent();
            Localizer.Ctrl(this);

            _bInCtor = true;
            for (int i = 0; i < lstBibleResources.Count; i++)
            {
                var modInfo = lstBibleResources[i];
                string strName = modInfo.Name;
                string strDesc = modInfo.Description;
                if (modInfo.Name == "NET")
                    _nIndexOfNetBible = i;
                var item = GetDisplayString(strName, modInfo.Category, strDesc);
                checkedListBoxSwordBibles.Items.Add(item, modInfo.Delta == NetBibleViewer.ModInfoDeltaLoaded);
            }
            for (int i = 0; i < lstBibleCommentaries.Count; i++)
            {
                var module = lstBibleCommentaries[i];
                var item = GetDisplayString(module.Name, module.Category, module.Description);
                checkedListBoxSwordBibles.Items.Add(item, true);    // by definition
            }

            linkLabelLinkToSword.Links.Add(6, 4, CstrSwordLink);
            labelDownloadProgress.Text = "";
            _bInCtor = false;
        }

        private FtpClient _ftp = null;

        public static string SwordUpgradeCacheDir
        {
            get { return Path.Combine(Application.UserAppDataPath, "sword-upgrade-cache"); }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StopBackgroundWorker();

            if (tabControl.SelectedTab == tabPageSeedConnect)
            {
                var cursor = Cursor;
                Cursor = Cursors.WaitCursor;
#if UsingCSwordRemoteManager
                try
                {
                    // we used to use this to FTP download the files. Now we just want to create the manifest that makes OSE
                    //  think there are modules to install in the folder (w/ elevated privileges)
                    var swordManifestCreator = AutoUpgrade.CreateSwordDownloader(Program.UpgradeCacheDir);
                    swordManifestCreator.ApplicationBasePath = StoryProjectData.GetRunningFolder;

                    // create the path to the folder where we want to install them (the 'upgrade-cache', so the regular copy 
                    //  handler at startup can see them)
                    var upgradePath = Path.Combine(Program.UpgradeCacheDir, NetBibleViewer.SwordResourcePathSubfolderName);
                    using (Manager manager = new Manager(upgradePath))
                    {
                        foreach (var strItem in from int checkedIndex in checkedListBoxDownloadable.CheckedIndices
                                                select checkedListBoxDownloadable.Items[checkedIndex] as String)
                        {
                            ParseCheckBoxItem(strItem, out string moduleName);

                            var data = _mapShortCodes2SwordData[moduleName];
                            var installManager = CrossWireInstallManager;
                            if (installManager.RemoteInstallModule(manager, data.SwordRemoteSource, data.SwordShortCode))
                            {
                                data.ModsDfile = Path.Combine(Path.Combine(upgradePath, CstrPathModsD),
                                                              $"{data.SwordShortCode}.conf");
                                if (!GetInformation(data.ModsDfile, ref data))
                                    continue;

                                var pathToDataFiles = Path.Combine(Program.UpgradeCacheDir, data.ModulesDataPath);
                                data.SwordDataFiles = Directory.GetFiles(pathToDataFiles).ToList();

                                swordManifestCreator.AddModuleToManifest(data.ModsDfile, data.SwordDataFiles);
                                if (data.DirectionRtl && !Properties.Settings.Default.ListSwordModuleToRtl.Contains(data.SwordShortCode))
                                {
                                    Properties.Settings.Default.ListSwordModuleToRtl.Add(data.SwordShortCode);
                                    Properties.Settings.Default.Save();
                                }
                            }
                            else
                                throw new ApplicationException(Localizer.Str("Unable to download requested SWORD module(s)... Try again later."));
                        }
                    }
                    CrossWireInstallManager = null;

                    // once we're done pulling the files from remote, then see about calling AutoUpdate to do its copy of the files
                    if (swordManifestCreator.IsUpgradeAvailable())
                    {
                        swordManifestCreator.PrepareModuleForInstall();
                        LocalizableMessageBox.Show(
                            Localizer.Str("The new SWORD module(s) are downloaded and will be installed the next time OneStory Editor is launched."),
                            StoryEditor.OseCaption);

                        buttonOK.Enabled = false;
                        buttonCancel.Text = CloseLabel;
                    }
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                    return;
                }
                finally
                {
                    Cursor = cursor;
                }
#else
                AutoUpgrade swordDownloader = null;
                try
                {
                    if (_ftp == null)
                        _ftp = FtpClient;
                    swordDownloader = AutoUpgrade.CreateSwordDownloader(Program.IDS_OSEUpgradeServerSword);
                    swordDownloader.ApplicationBasePath = StoryProjectData.GetRunningFolder;
                    foreach (var strItem in from int checkedIndex in checkedListBoxDownloadable.CheckedIndices 
                                            select checkedListBoxDownloadable.Items[checkedIndex] as String)
                    {
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strItem) && (strItem.IndexOf(':') != -1));
                        var strShortCode = strItem.Substring(0, strItem.IndexOf(':'));
                        var data = _mapShortCodes2SwordData[strShortCode];
                        swordDownloader.AddModuleToManifest(_ftp, data.ModsDfile, data.ModulesDataPath);
                        if (data.DirectionRtl && !Properties.Settings.Default.ListSwordModuleToRtl.Contains(data.SwordShortCode))
                        {
                            Properties.Settings.Default.ListSwordModuleToRtl.Add(data.SwordShortCode);
                            Properties.Settings.Default.Save();
                        }
                    }

                    CloseFtpConnection();

                    // once we close *our* ftp connection, then see about calling AutoUpdate to do its copy of the files
                    if (swordDownloader.IsUpgradeAvailable())
                    {
                        swordDownloader.PrepareModuleForInstall();
                        LocalizableMessageBox.Show(
                            Localizer.Str("The new SWORD module(s) are downloaded and will be installed the next time OneStory Editor is launched."),
                            StoryEditor.OseCaption);

                        buttonOK.Enabled = false;
                        buttonCancel.Text = CloseLabel;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("Unable to read data from the transport connection: A non-blocking socket operation") == 0)
                        LocalizableMessageBox.Show(
                            Localizer.Str(
                                "The connection to the server isn't fully closed down from a previous attempt. Please wait a few seconds and try again."),
                            StoryEditor.OseCaption);
                    else
                        Program.ShowException(ex);
                    return;
                }
                finally
                {
                    // need to have closed the connection before doing the next bit
                    CloseFtpConnection();
                    Cursor = cursor;
                }
#endif
            }
            else
            {
                for (int i = 0; i < checkedListBoxSwordBibles.Items.Count; i++)
                {
                    var item = checkedListBoxSwordBibles.Items[i].ToString();
                    ParseCheckBoxItem(item, out string moduleName);

                    // if it were a commentary, then we don't really support uninstalling those and they aren't shown as active anyway.
                    var modInfo = _lstBibleResources.FirstOrDefault(r => r.Name == moduleName);
                    if (modInfo != null)
                        modInfo.Delta = (checkedListBoxSwordBibles.GetItemChecked(i)) ? NetBibleViewer.ModInfoDeltaLoaded : null;
                }

                DialogResult = DialogResult.OK;
#if !UsingCSwordRemoteManager
                CloseFtpConnection();
#endif
                Close();
            }
        }

        private void ParseCheckBoxItem(string strItem, out string moduleName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strItem) && (strItem.IndexOf(':') != -1));

            // this will be the modules "Name" field... but if it were a commentary, then it'll also have " (Commentaries)" attached to it
            var strShortCode = strItem.Substring(0, strItem.IndexOf(':'));
            if (strShortCode.Contains('('))
            {
                strShortCode = strShortCode.Substring(0, strShortCode.IndexOf('(') - 1);
            }
            moduleName = strShortCode;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
#if UsingCSwordRemoteManager
            StopBackgroundWorker();
#else
            CloseFtpConnection();
#endif
        }

        private void StopBackgroundWorker()
        {
            if (backgroundWorkerLoadDownloadListBox.IsBusy)

            {
                backgroundWorkerLoadDownloadListBox.CancelAsync();
                while (backgroundWorkerLoadDownloadListBox.IsBusy)
                    Application.DoEvents();
            }
        }

        private void CloseFtpConnection()
        {
            if (_ftp != null)
            {
#if UseFluentFtp
                _ftp.Disconnect();
#else
                _ftp.Close();
                _ftp.Dispose();
                System.Threading.Thread.Sleep(2000);
                Application.DoEvents(); // let it get a chance to complete the releasing of the Ftp connection
#endif
                _ftp = null;
            }
        }

        private void linkLabelLinkToSword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strSwordLink = (string)e.Link.LinkData;
            if (!String.IsNullOrEmpty(strSwordLink))
                System.Diagnostics.Process.Start(strSwordLink);
        }

        private void checkedListBoxSwordBibles_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((e.Index == _nIndexOfNetBible) 
                && (e.CurrentValue == CheckState.Unchecked) 
                && (e.NewValue == CheckState.Checked)
                && !_bInCtor)
            {
                var dlg = new HtmlForm
                {
                    Text = "NET Bible",
                    ClientText = Properties.Resources.IDS_NetBibleDonateMessage
                };

                dlg.ShowDialog();
            }
        }

#if UsingCSwordRemoteManager
#elif UseOseServer
        private const string CstrPathSwordRemote = "/SWORD/";
#elif UseSeedCo
        private const string CstrPathSwordRemote = "/SWORD/";
#elif UsePalaso
        private const string CstrPathSwordRemote = "/OseUpdates/SWORD/";
#endif
        private const string CstrPathModsD = "mods.d";
        private static Dictionary<string, SwordModuleData> _mapShortCodes2SwordData = new Dictionary<string, SwordModuleData>();

        private FtpClient FtpClient
        {
            get
            {
#if UseOseServer
                var host = Properties.Settings.Default.OseServerIpAddress;
                const string user = "OseProgram";
                const string pass = "OseAccess!2O";
#endif
#if !UseFluentFtp
                const int port = 21;
#endif

                // create a new ftpclient object with the host and port number to use
#if UseSeedCo
                // set the security protocol to use - in this case we are instructing the FtpClient to use either
                // the SSL 3.0 protocol or the TLS 1.0 protocol depending on what the FTP server supports
                var ftp = new FtpClient(host, port, FtpSecurityProtocol.Tls1OrSsl3Explicit);
#else
#if UseFluentFtp
                var ftp = new FtpClient(host)
                {
                    Credentials = new System.Net.NetworkCredential(user, pass),
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                };
                ftp.Connect();
#else
                var ftp = new FtpClient(host, port);
#endif
#if UseSeedCo || !UseFluentFtp
                // register an event hook so that we can view and accept the security certificate that is given by the FTP server
                ftp.ValidateServerCertificate += FtpValidateServerCertificate;
                ftp.ConnectionClosed += FtpConnectionClosed;
                ftp.Open(user, pass);
#endif
#endif
                return ftp;
            }
        }

#if !UseFluentFtp
        private static void FtpValidateServerCertificate(object sender, ValidateServerCertificateEventArgs e)
        {
            e.IsCertificateValid = true;
        }

        private void FtpConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            // not sure if this occurs but we should definitely try to make sure we dispose of things properly.
            CloseFtpConnection();
        }
#endif

        private static readonly Regex RegexModsReaderShortCode = new Regex(@"\[(.+?)\]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RegexModsReaderDesc = new Regex("Description=(.+?)[\n\r]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RegexModsReaderDataPath = new Regex(@"DataPath=\.\/(.+?)[\n\r]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static bool GetInformation(string strFilename, ref SwordModuleData data)
        {
            string strContents = File.ReadAllText(strFilename);
            var match = RegexModsReaderShortCode.Match(strContents);
            if (match.Groups.Count != 2)
                return false;
            data.SwordShortCode = match.Groups[1].Value;
            match = RegexModsReaderDesc.Match(strContents);
            if (match.Groups.Count != 2)
                return false;
            data.SwordDescription = match.Groups[1].Value;
            match = RegexModsReaderDataPath.Match(strContents);
            if (match.Groups.Count != 2)
                return false;
            var strDataPath = match.Groups[1].Value;
            if (strDataPath[strDataPath.Length - 1] != '/')
                strDataPath += '/';
#if UsingCSwordRemoteManager
            strDataPath = strDataPath.Replace('/','\\');
#endif
            data.ModulesDataPath = Path.Combine(NetBibleViewer.SwordResourcePathSubfolderName, strDataPath);

            const string cstrDirectionRtl = "Direction=RtoL";
            if (strContents.IndexOf(cstrDirectionRtl) != -1)
                data.DirectionRtl = true;
            return true;
        }

        private InstallManager _installManager;
        private InstallManager CrossWireInstallManager
        {
            get
            {
                if (_installManager == null)
                {
                    _installManager = new InstallManager(SwordUpgradeCacheDir);
                }
                return _installManager;
            }
            set
            {
                if ((_installManager != null) && (value == null))
                    _installManager.Dispose();
                _installManager = value;
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopBackgroundWorker();

            if (tabControl.SelectedTab == tabPageSeedConnect)
            {
                try
                {
                    comboCheckBoxListForSwordSources.Visible = true;
                    bool bAtLeastOneInstallable = false;
                    checkedListBoxDownloadable.Items.Clear();   // in case it's a repeat
                    _mapShortCodes2SwordData.Clear();
#if UsingCSwordRemoteManager
                    bAtLeastOneInstallable = true;  // this'll always be true
                    backgroundWorkerLoadDownloadListBox.RunWorkerAsync();
#else
                    if (_ftp == null)
                        _ftp = FtpClient;

#if UseFluentFtp
                    var files = _ftp.GetListing(CstrPathSwordRemote + CstrPathModsD, FtpListOption.AllFiles);
                    foreach (var file in files)
                    {
                        var strTempFilename = Path.GetTempFileName();
                        _ftp.DownloadFile(strTempFilename, file.FullName);
#else
                    var files = _ftp.GetDirList(CstrPathSwordRemote + CstrPathModsD, true);
                    foreach (var file in files)
                    {
                        var strTempFilename = Path.GetTempFileName();
                        _ftp.GetFile(file.FullPath, strTempFilename, FileAction.Create);
#endif
                        SwordModuleData data;
                        if (!GetInformation(strTempFilename, out data))
                            continue;
                        data.ModsDfile = file;  // add for later use

                        // don't bother to add it to the list box if it's already installed with the same
                        //  time/date stamp
                        if (!IsAlreadyInstalled(data))
                        {
                            checkedListBoxDownloadable.Items.Add(String.Format("{0}: {1}", data.SwordShortCode, data.SwordDescription), false);
                            _mapShortCodes2SwordData.Add(data.SwordShortCode, data);
                            bAtLeastOneToInstall = true;
                        }
                    }
#endif
                    if (!bAtLeastOneInstallable)
                    {
                        LocalizableMessageBox.Show(
                            Localizer.Str(
                                "There are no additional SWORD modules to download from the OneStory Editor SWORD server that you don't already have installed. Check the SWORD depot at the link below for more options."),
                            StoryEditor.OseCaption);
                    }
                    else
                    {
                        buttonOK.Text = DownloadLabel;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("Unable to read data from the transport connection: A non-blocking socket operation") == 0)
                        LocalizableMessageBox.Show(
                            Localizer.Str(
                                "The connection to the server isn't fully closed down from a previous attempt. Please wait a few seconds and try again."),
                            StoryEditor.OseCaption);
                    else
                        Program.ShowException(ex);
                }
            }
            else
                comboCheckBoxListForSwordSources.Visible = false;
        }

        private static string DownloadLabel
        {
            get { return Localizer.Str("&Download"); }
        }

        private static string CloseLabel
        {
            get { return Localizer.Str("&Close"); }
        }

#if UsingCSwordRemoteManager
        private bool IsAlreadyInstalled(ModInfo modInfo)
        {
            return  _lstBibleResources.Any(p => p.Name == modInfo.Name) ||
                    _lstBibleCommentaries.Any(c => c.Name == modInfo.Name);
        }

        private Dictionary<string, List<ModInfo>> _mapSwordSourceToModInfos = new Dictionary<string, List<ModInfo>>();

        private void backgroundWorkerLoadDownloadListBox_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var cursor = Cursor;
            backgroundWorkerLoadDownloadListBox.ReportProgress(2, Cursors.WaitCursor);  // turn on wait cursor
            try
            {
                var installManager = CrossWireInstallManager;
                installManager.SetUserDisclaimerConfirmed();
                installManager.SyncConfig();

                using (Manager manager = new Manager())
                {
                    foreach (var source in installManager.RemoteSources)
                    {
                        if (!_mapSwordSourceToModInfos.TryGetValue(source, out List<ModInfo> modInfos))
                        {
                            backgroundWorkerLoadDownloadListBox.ReportProgress(0, source);
                            installManager.RefreshRemoteSource(source);
                            modInfos = installManager.GetRemoteModInfoList(manager, source).ToList();
                            _mapSwordSourceToModInfos.Add(source, modInfos);
                        }

                        foreach (var modInfo in modInfos)
                        {
                            if (this.backgroundWorkerLoadDownloadListBox.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (!_mapShortCodes2SwordData.TryGetValue(modInfo.Name, out SwordModuleData swordModuleData))
                            {
                                swordModuleData = new SwordModuleData
                                {
                                    SwordRemoteSource = source,
                                    SwordShortCode = modInfo.Name,
                                    SwordDescription = modInfo.Description,
                                    SwordModInfo = modInfo
                                };

                                _mapShortCodes2SwordData.Add(swordModuleData.SwordShortCode, swordModuleData);
                            }

                            if (IsAlreadyInstalled(modInfo) ||  // already on the first tab of installed items (don't show in the download tab)
                                Properties.Settings.Default.SwordSourcesToExclude.Contains(source)) // don't add it to the checkboxlist if the user had previously unchecked its source
                                    continue;

                            AddToDownloadCheckBoxList(modInfo);
                        }
                    }
                    backgroundWorkerLoadDownloadListBox.ReportProgress(3, Localizer.Str("All accessible resources listed"));
                }
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
            finally
            {
                CrossWireInstallManager = null;
                backgroundWorkerLoadDownloadListBox.ReportProgress(2, cursor);  // restore/turn off wait cursor
            }
        }

        private bool AddToDownloadCheckBoxList(ModInfo modInfo)
        {
            string item = GetDisplayString(modInfo.Name, modInfo.Category, modInfo.Description);
            if (checkedListBoxDownloadable.Items.Contains(item))
                return false;

            if (backgroundWorkerLoadDownloadListBox.IsBusy)
                backgroundWorkerLoadDownloadListBox.ReportProgress(1, item);
            else
                checkedListBoxDownloadable.Items.Add(item, false);

            return true;
        }

        private void AddSourceToDropdownComboBox(string source)
        {
            var item = new CCBoxItem() { Name = source };
            var idx = comboCheckBoxListForSwordSources.Items.Add(item);
            var uncheckedState = Properties.Settings.Default.SwordSourcesToExclude.Contains(source);
            comboCheckBoxListForSwordSources.SetItemChecked(idx, !uncheckedState);
        }

        private static string GetDisplayString(string moduleName, string moduleCategory, string moduleDescription)
        {
            return (moduleCategory != NetBibleViewer.SwordResourceCategoryBiblicalText)
                            ? $"{moduleName} ({moduleCategory}): {moduleDescription}"
                            : $"{moduleName}: {moduleDescription}";
        }

        private void backgroundWorkerLoadDownloadListBox_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 2)
            {
                var cursor = e.UserState as Cursor;
                Cursor = cursor;
            }
            else
            {
                var data = e.UserState as string;
                if (e.ProgressPercentage == 0)
                {
                    AddSourceToDropdownComboBox(data);
                    labelDownloadProgress.Text = String.Format(Localizer.Str("Gathering resources available from '{0}'..."), data);
                }
                else if (e.ProgressPercentage == 1)
                {
                    checkedListBoxDownloadable.Items.Add(data, false);
                }
                else if (e.ProgressPercentage == 3)
                {
                    labelDownloadProgress.Text = data;
                }
                else 
                    System.Diagnostics.Debug.Fail("Did you add a new type?");
            }
        }
#else
        private bool IsAlreadyInstalled(SwordModuleData data)
        {
            if (_lstBibleResources.Any(p => p.Name == data.SwordShortCode))
            {
                var strLocalFilepath = Path.Combine(StoryProjectData.GetRunningFolder,
#if UseFluentFtp
                                                    data.ModsDfile.FullName.Substring(1).Replace('/', '\\'));
#else
                                                    data.ModsDfile.FullPath.Substring(1).Replace('/', '\\'));
#endif

                if (File.Exists(strLocalFilepath))
                {
                    var dtLocal = File.GetLastWriteTime(strLocalFilepath);
                    return (Math.Abs((dtLocal - data.ModsDfile.Modified).TotalMinutes) < 3600);
                }
                // else  it must mean it was in some other SWORD folder, so consider it not installed 
                //  (so that it's displayed in the download list
            }
            return false;
        }
#endif

        Dictionary<string, bool> _mapSwordSourceToShowState = new Dictionary<string, bool>();

        private void ccb_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var ccb = comboCheckBoxListForSwordSources.Items[e.Index] as CCBoxItem;
            var source = ccb.Name;
            if (e.NewValue == CheckState.Checked)
            {
                // do we need to add it back?
                labelDownloadProgress.Text = String.Format(Localizer.Str("Added resources available from '{0}'..."), source);
                if (_mapSwordSourceToModInfos.TryGetValue(source, out List<ModInfo> modInfos))
                {
                    foreach (var modInfo in modInfos)
                    {
                        AddToDownloadCheckBoxList(modInfo);
                    }
                    AddRemoveSwordSourceSetting(source, add: true);
                }
            }
            else
            {
                // remove these from the list box. First get the descending indices to remove
                labelDownloadProgress.Text = String.Format(Localizer.Str("Removed resources available from '{0}'..."), source);
                if (_mapSwordSourceToModInfos.TryGetValue(source, out List<ModInfo> modInfos))
                {
                    foreach (var modInfo in modInfos)
                    {
                        var checkListBoxItemValue = GetDisplayString(modInfo.Name, modInfo.Category, modInfo.Description);
                        var idx = checkedListBoxDownloadable.Items.IndexOf(checkListBoxItemValue);
                        if (idx >= 0)
                            checkedListBoxDownloadable.Items.RemoveAt(idx);
                    }
                }
                /*
                List<int> descendingIndiciesToRemove = new List<int>();
                for (var j = checkedListBoxDownloadable.Items.Count - 1; j >= 0; j--)
                {
                    var item = checkedListBoxDownloadable.Items[j] as String;
                    ParseCheckBoxItem(item, out string moduleName);
                    System.Diagnostics.Debug.Assert(_mapShortCodes2SwordData.ContainsKey(moduleName));
                    var modInfo = _mapShortCodes2SwordData[moduleName];
                    if (source == modInfo.SwordRemoteSource)
                        descendingIndiciesToRemove.Add(j);
                }

                descendingIndiciesToRemove.ForEach(idx => checkedListBoxDownloadable.Items.RemoveAt(idx));
                */
                checkedListBoxDownloadable.Refresh();
                AddRemoveSwordSourceSetting(source, add: false);
            }
        }

        private static void AddRemoveSwordSourceSetting(string source, bool add)
        {
            var inSettingsAlready = Properties.Settings.Default.SwordSourcesToExclude.Contains(source);
            if (add && !inSettingsAlready)
            {
                Properties.Settings.Default.SwordSourcesToExclude.Add(source);
            }
            else if (!add && inSettingsAlready)
            {
                Properties.Settings.Default.SwordSourcesToExclude.Remove(source);
            }
            else
                return;

            Properties.Settings.Default.Save();
        }
    }

    public class SwordModuleData
    {
        public string SwordShortCode { get; set; }
        public string SwordDescription { get; set; }

        public ModInfo SwordModInfo { get; set; }
        public string SwordRemoteSource { get; set; }
#if UsingCSwordRemoteManager
        public string ModsDfile { get; set; }   // path to .conf file
        public List<string> SwordDataFiles { get; set; }    // path to the data files (e.g. in ./modules/texts/ztext/azeri/)
#else
#if UseFluentFtp
        public FtpListItem ModsDfile { get; set; }
#else
        public FtpItem ModsDfile { get; set; }
#endif
#endif
        public string ModulesDataPath { get; set; }
        public bool DirectionRtl { get; set; }
    }
}
