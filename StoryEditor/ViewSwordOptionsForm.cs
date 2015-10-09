#define UseSeedCo

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

        private FtpClient _ftp = null;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageSeedConnect)
            {
                AutoUpgrade swordDownloader = null;
                var cursor = Cursor;
                Cursor = Cursors.WaitCursor;
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
            }
            else
            {
                for (int i = 0; i < checkedListBoxSwordBibles.Items.Count; i++)
                    _lstBibleResources[i].Loaded = checkedListBoxSwordBibles.GetItemChecked(i);

                DialogResult = DialogResult.OK;
                CloseFtpConnection();
                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CloseFtpConnection();
        }

        private void CloseFtpConnection()
        {
            if (_ftp != null)
            {
                _ftp.Close();
                _ftp.Dispose();
                _ftp = null;
                System.Threading.Thread.Sleep(2000);
                Application.DoEvents(); // let it get a chance to complete the releasing of the Ftp connection
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

#if UseSeedCo
        private const string CstrPathSwordRemote = "/SWORD/";
#else
        private const string CstrPathSwordRemote = "/OseUpdates/SWORD/";
#endif
        private const string CstrPathModsD = "mods.d";
        private static Dictionary<string, SwordModuleData> _mapShortCodes2SwordData = null;

        private FtpClient FtpClient
        {
            get
            {
#if UseSeedCo
                const string host = "ftp.seedconnect.org";
                const string user = "Bob_Eaton";
                const string pass = "tsc2009";
#else
                const string host = "palaso.org";
                const string user = "onestory";
                const string pass = "yrotseno23";
#endif
                const int port = 21;

                if (_mapShortCodes2SwordData == null)
                    _mapShortCodes2SwordData = new Dictionary<string, SwordModuleData>();

                // create a new ftpclient object with the host and port number to use
#if UseSeedCo
                // set the security protocol to use - in this case we are instructing the FtpClient to use either
                // the SSL 3.0 protocol or the TLS 1.0 protocol depending on what the FTP server supports
                var ftp = new FtpClient(host, port, FtpSecurityProtocol.Tls1OrSsl3Explicit);
#else
                var ftp = new FtpClient(host, port);
#endif
                // register an event hook so that we can view and accept the security certificate that is given by the FTP server
                ftp.ValidateServerCertificate += FtpValidateServerCertificate;
                ftp.ConnectionClosed += FtpConnectionClosed;
                ftp.Open(user, pass);
                return ftp;
            }
        }

        private void FtpConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            // not sure if this occurs but we should definitely try to make sure we dispose of things properly.
            CloseFtpConnection();
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
            data.ModulesDataPath = CstrPathSwordRemote + strDataPath;

            const string cstrDirectionRtl = "Direction=RtoL";
            if (strContents.IndexOf(cstrDirectionRtl) != -1)
                data.DirectionRtl = true;
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
                try
                {
                    if (_ftp == null)
                        _ftp = FtpClient;
                    var files = _ftp.GetDirList(CstrPathSwordRemote + CstrPathModsD, true);
                    checkedListBoxDownloadable.Items.Clear();   // in case it's a repeat
                    _mapShortCodes2SwordData.Clear();
                    bool bAtLeastOneToInstall = false;
                    foreach (var file in files)
                    {
                        var strTempFilename = Path.GetTempFileName();
                        _ftp.GetFile(file.FullPath, strTempFilename, FileAction.Create);
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

                    if (!bAtLeastOneToInstall)
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
                finally
                {
                    // CloseFtpConnection();
                    Cursor = cursor;
                }
            }
        }

        private static string DownloadLabel
        {
            get { return Localizer.Str("&Download"); }
        }

        private static string CloseLabel
        {
            get { return Localizer.Str("&Close"); }
        }

        private bool IsAlreadyInstalled(SwordModuleData data)
        {
            if (_lstBibleResources.Any(p => p.Name == data.SwordShortCode))
            {
                var strLocalFilepath = Path.Combine(StoryProjectData.GetRunningFolder,
                                                    data.ModsDfile.FullPath.Substring(1).Replace('/', '\\'));
                
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
    }

    public class SwordModuleData
    {
        public string SwordShortCode { get; set; }
        public string SwordDescription { get; set; }
        public FtpItem ModsDfile { get; set; }
        public string ModsTempFilePath { get; set; }
        public string ModulesDataPath { get; set; }
        public bool DirectionRtl { get; set; }
    }
}
