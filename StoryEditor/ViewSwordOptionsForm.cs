using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetLoc;
using Starksoft.Net.Ftp;
using devX;

namespace OneStoryProjectEditor
{
    public partial class ViewSwordOptionsForm : TopForm
    {
        protected const string CstrSwordLink = "www.crosswire.org/sword/modules/ModDisp.jsp?modType=Bibles";

        protected List<NetBibleViewer.SwordResource> _lstBibleResources;
        protected int _nIndexOfNetBible = -1;
        protected bool _bInCtor;

        private ViewSwordOptionsForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public ViewSwordOptionsForm(ref List<NetBibleViewer.SwordResource> lstBibleResources)
            : base(true)
        {
            _lstBibleResources = lstBibleResources;

            InitializeComponent();
            Localizer.Ctrl(this);

            _bInCtor = true;
            for (int i = 0; i < lstBibleResources.Count; i++)
            {
                NetBibleViewer.SwordResource aSR = lstBibleResources[i];
                string strName = aSR.Name;
                string strDesc = aSR.Description;
                if (aSR.Name == "NET")
                    _nIndexOfNetBible = i;
                checkedListBoxSwordBibles.Items.Add(String.Format("{0}: {1}", strName, strDesc), aSR.Loaded);
            }

            linkLabelLinkToSword.Links.Add(6, 4, CstrSwordLink);
            _bInCtor = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageSeedConnect)
            {
                FtpClient ftp = null;
                try
                {
                    ftp = FtpClient;
                    var swordDownloader = AutoUpgrade.CreateSwordDownloader();
                    swordDownloader.ApplicationBasePath = StoryProjectData.GetRunningFolder;
                    foreach (int checkedIndex in checkedListBoxDownloadable.CheckedIndices)
                    {
                        var strItem = checkedListBoxDownloadable.Items[checkedIndex] as String;
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strItem) && (strItem.IndexOf(':') != -1));
                        var strShortCode = strItem.Substring(0, strItem.IndexOf(':'));
                        var data = _mapShortCodes2SwordData[strShortCode];
                        swordDownloader.DownloadModule(ftp, data.ModsFilename,
                                                       data.ModsTempFilePath,
                                                       data.ModulesDataPath);
                    }
                    if (swordDownloader.IsUpgradeAvailable())
                        swordDownloader.StartModuleInstall();
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
                finally
                {
                    if (ftp != null)
                    {
                        ftp.Close();
                        ftp.Dispose();
                    }
                }
            }
            else
            {
                for (int i = 0; i < checkedListBoxSwordBibles.Items.Count; i++)
                    _lstBibleResources[i].Loaded = checkedListBoxSwordBibles.GetItemChecked(i);
            }

            DialogResult = DialogResult.OK;
            Close();
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

        private const string CstrPathSwordRemote = "/SWORD/";
        private const string CstrPathModsD = "mods.d";
        private static Dictionary<string, SwordModuleData> _mapShortCodes2SwordData = null;

        private static FtpClient FtpClient
        {
            get
            {
                const string host = "ftp.seedconnect.org";
                const int port = 21;
                const string user = "Bob_Eaton";
                const string pass = "tsc2009";

                if (_mapShortCodes2SwordData == null)
                    _mapShortCodes2SwordData = new Dictionary<string, SwordModuleData>();

                // create a new ftpclient object with the host and port number to use
                // set the security protocol to use - in this case we are instructing the FtpClient to use either
                // the SSL 3.0 protocol or the TLS 1.0 protocol depending on what the FTP server supports
                var ftp = new FtpClient(host, port, FtpSecurityProtocol.Tls1OrSsl3Explicit);

                // register an event hook so that we can view and accept the security certificate that is given by the FTP server
                ftp.ValidateServerCertificate += FtpValidateServerCertificate;

                ftp.Open(user, pass);
                return ftp;
            }
        }

        private static void DownloadModule(FtpClient ftp, string strSwordPathLocal, string strModsName,
            string strModsTempName, string strRemoteDataFolder)
        {
            // copy the downloaded (temporary) mods file to the proper path
            var strPathToModsFolder = Path.Combine(strSwordPathLocal, CstrPathModsD);
            if (!Directory.Exists(strPathToModsFolder))
                Directory.CreateDirectory(strPathToModsFolder);

            var strPathToMods = Path.Combine(strPathToModsFolder, strModsName);
            File.Copy(strModsTempName, strPathToMods, true);

            // create the local path for the data files
            var strLocalPath = GetLocalPath(strSwordPathLocal, strRemoteDataFolder);

            // Get the files to it
            ftp.GetFiles(CstrPathSwordRemote + strRemoteDataFolder, strLocalPath);
        }

        private static readonly char[] _achPathDelim = new[] { '/' };

        private static string GetLocalPath(string strSwordPath, string strModulePath)
        {
            string[] astr = strModulePath.Split(_achPathDelim, StringSplitOptions.RemoveEmptyEntries);
            return astr.Aggregate(strSwordPath, Path.Combine);
        }

        private static readonly Regex RegexModsReaderShortCode = new Regex(@"\[(.+?)\]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RegexModsReaderDesc = new Regex("Description=(.+?)[\n\r]", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RegexModsReaderDataPath = new Regex(@"DataPath=\.\/(.+?)[\n\r]", RegexOptions.Compiled | RegexOptions.Singleline);

        private static bool GetInformation(string strFilename, out SwordModuleData data)
        {
            data = new SwordModuleData {ModsTempFilePath = strFilename};
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
            data.ModulesDataPath = strDataPath;
            return true;
        }

        private static void FtpValidateServerCertificate(object sender, ValidateServerCertificateEventArgs e)
        {
            e.IsCertificateValid = true;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageSeedConnect)
            {
                var cursor = Cursor;
                Cursor = Cursors.WaitCursor;
                FtpClient ftp = null;
                try
                {
                    ftp = FtpClient;
                    var files = ftp.GetDirList(CstrPathSwordRemote + CstrPathModsD);
                    checkedListBoxDownloadable.Items.Clear();   // in case it's a repeat
                    _mapShortCodes2SwordData.Clear();
                    foreach (var file in files)
                    {
                        var strTempFilename = Path.GetTempFileName();
                        ftp.GetFile(file.FullPath, strTempFilename, FileAction.Create);
                        SwordModuleData data;
                        if (!GetInformation(strTempFilename, out data))
                            continue;
                        data.ModsFilename = file.Name;  // add for later use
                        checkedListBoxDownloadable.Items.Add(String.Format("{0}: {1}", data.SwordShortCode, data.SwordDescription), false);
                        _mapShortCodes2SwordData.Add(data.SwordShortCode, data);
                    }
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
                finally
                {
                    if (ftp != null)
                    {
                        ftp.Close();
                        ftp.Dispose();
                    }
                    Cursor = cursor;
                }
            }
        }
    }

    public class SwordModuleData
    {
        public string SwordShortCode { get; set; }
        public string SwordDescription { get; set; }
        public string ModsFilename { get; set; }
        public string ModsTempFilePath { get; set; }
        public string ModulesDataPath { get; set; }
    }
}
