﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetLoc;
using Sword;
using devX;
using Newtonsoft.Json;

namespace OneStoryProjectEditor
{
    public partial class ViewSwordOptionsForm : TopForm
    {
        protected const string CstrSwordLink = "www.crosswire.org/sword/modules/ModDisp.jsp?modType=Bibles";

        protected List<ModInfo> _lstBibleResources;
        protected List<Module> _lstBibleCommentaries;
        protected int _nIndexOfNetBible = -1;
        protected bool _bInCtor;

        private ViewSwordOptionsForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public ViewSwordOptionsForm(ref List<ModInfo> lstBibleResources, List<Module> lstBibleCommentaries)
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

        public static string SwordUpgradeCacheDir
        {
            get { return Path.Combine(Application.UserAppDataPath, "sword-upgrade-cache"); }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StopBackgroundWorker();

            if (tabControl.SelectedTab == tabPageDownloadTree)
            {
                var cursor = Cursor;
                Cursor = Cursors.WaitCursor;
                try
                {
                    // we used to use this to FTP download the files. Now we just want to create the manifest that makes OSE
                    //  think there are modules to install in the folder (w/ elevated privileges)
                    var swordManifestCreator = AutoUpgrade.CreateSwordDownloader(Program.UpgradeCacheDir);
                    swordManifestCreator.ApplicationBasePath = StoryProjectData.GetRunningFolder;

                    // all the files in the SWORD resource have already been downloaded (when it was checked), so here we just need
                    //  to create the manifest for them
                    var resourcesToDownload = _mapSourceAndName2SwordData.Values.SelectMany(kvp => kvp.Values.Where(s => s.SwordDataFiles != null)).ToList();
                    foreach (var data in resourcesToDownload)
                    {
                        swordManifestCreator.AddModuleToManifest(data.ModsDfile, data.SwordDataFiles);

                        if (data.DirectionRtl && !Properties.Settings.Default.ListSwordModuleToRtl.Contains(data.SwordShortCode))
                        {
                            Properties.Settings.Default.ListSwordModuleToRtl.Add(data.SwordShortCode);
                            Properties.Settings.Default.Save();
                        }
                    }

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
            StopBackgroundWorker();
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

        private const string CstrPathModsD = "mods.d";

        // map of SWORD resource "sources" to a map of module names to module information
        private static Dictionary<string, Dictionary<string, SwordModuleData>> _mapSourceAndName2SwordData = new Dictionary<string, Dictionary<string, SwordModuleData>>();

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
            strDataPath = strDataPath.Replace('/','\\');
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

            if (tabControl.SelectedTab == tabPageDownloadTree)
            {
                try
                {
                    labelFilter.Visible = 
                        textBoxFilter.Visible = 
                        treeViewSourcesDownloadable.Visible = true;
                    bool bAtLeastOneInstallable = false;
                    treeViewSourcesDownloadable.Nodes.Clear();  // in case it's a repeat
                    _mapSourceAndName2SwordData.Clear();

                    bAtLeastOneInstallable = true;  // this'll always be true
                    backgroundWorkerLoadDownloadListBox.RunWorkerAsync();
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
            {
                labelFilter.Visible = 
                    textBoxFilter.Visible = 
                    treeViewSourcesDownloadable.Visible = false;
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

        private bool IsAlreadyInstalled(ModInfo modInfo)
        {
            return  _lstBibleResources.Any(p => p.Name == modInfo.Name) ||
                    _lstBibleCommentaries.Any(c => c.Name == modInfo.Name);
        }

        private void backgroundWorkerLoadDownloadListBox_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                var installManager = CrossWireInstallManager;
                installManager.SetUserDisclaimerConfirmed();
                installManager.SyncConfig();

                using (Manager manager = new Manager())
                {
                    // load the checkListBox first (so it happens quickly)
                    var sources = installManager.RemoteSources.OrderBy(s => Properties.Settings.Default.SwordSourcesToExclude.Contains(s));
                    foreach (var source in sources)
                    {
                        // add the sources as the first level leaf nodes in the treeview
                        var sourceTreeNode = new TreeNode(source) 
                        {
                            ToolTipText = $"Open this node to see the SWORD resources available from the {source} source",
                            Name = source,
                            Checked = !Properties.Settings.Default.SwordSourcesToExclude.Contains(source) 
                        };
                        backgroundWorkerLoadDownloadListBox.ReportProgress(0, sourceTreeNode);
                    }

                    // now go back thru it and (in the background), update their resources in case we want to add them later
                    foreach (var source in sources)
                    {
                        if (!_mapSourceAndName2SwordData.TryGetValue(source, out Dictionary<string, SwordModuleData> mapName2SwordData))
                        {
                            mapName2SwordData = new Dictionary<string, SwordModuleData>();
                            _mapSourceAndName2SwordData.Add(source, mapName2SwordData);

                            installManager.RefreshRemoteSource(source);
                        }

                        var modInfos = installManager.GetRemoteModInfoList(manager, source).ToList();

                        var count = modInfos.Count;
                        int updateLabelFrequency = GetUpdateFrequency(count);   // don't allow it to be 0
                        backgroundWorkerLoadDownloadListBox.ReportProgress(4, $"{source} (total: {count})");  // update the label

                        foreach (var modInfo in modInfos.OrderBy(m => m.Name))
                        {
                            if (this.backgroundWorkerLoadDownloadListBox.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            else
                                Application.DoEvents(); // so the UI doesn't seem locked out

                            if (!mapName2SwordData.TryGetValue(modInfo.Name, out SwordModuleData swordModuleData))
                            {
                                swordModuleData = new SwordModuleData
                                {
                                    SwordRemoteSource = source,
                                    SwordShortCode = modInfo.Name,
                                    SwordDescription = modInfo.Description,
                                    SwordModInfo = modInfo
                                };

                                mapName2SwordData.Add(swordModuleData.SwordShortCode, swordModuleData);
                            }

                            // add a '.' to the lable every so it shows progress
                            if (--updateLabelFrequency == 0)
                            {
                                backgroundWorkerLoadDownloadListBox.ReportProgress(5, ".");  // update the label
                                updateLabelFrequency = GetUpdateFrequency(count);
                            }

                            string filter;
                            if (Properties.Settings.Default.SwordSourcesToExclude.Contains(source) ||   // don't add it to the checkboxlist if the user had previously unchecked its source
                                (!String.IsNullOrEmpty(filter = textBoxFilter.Text) && modInfo.Name.Contains(filter)))
                            {
                                continue;
                            }

                            AddToDownloadCheckBoxList(modInfo, source);
                        }
                    }
                    backgroundWorkerLoadDownloadListBox.ReportProgress(3, Localizer.Str("All accessible resources listed..."));
                }
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
            finally
            {
                CrossWireInstallManager = null;
            }
        }

        private static int GetUpdateFrequency(int count)
        {
            return Math.Max(1, count / 20);
        }

        private bool IsFilterMatch(string filter, string inputText, bool caseSensitive)
        {
            return ((caseSensitive && inputText.Contains(filter)) ||
                    (!caseSensitive && inputText.ToLower().Contains(filter)));
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            var filter = textBoxFilter.Text;
            var anyUpperCase = filter.Any(char.IsUpper);

            var checkedSourceNodes = treeViewSourcesDownloadable.Nodes.Cast<TreeNode>().ToList().Where(tn => tn.Checked).ToList();

            // remove any resource nodes that don't contain the filter string in their 'Text' value (but only if the filter string is not null)
            if (!String.IsNullOrEmpty(filter))
            {
                foreach (var sourceNode in checkedSourceNodes)
                {
                    foreach (var resourceNode in sourceNode.Nodes.Cast<TreeNode>().ToList().Where(rn => !IsFilterMatch(filter, rn.Text, anyUpperCase)))
                    {
                        resourceNode.Remove();
                    }
                }
            }

            // but only add them if the source node is checked (if we're in the process of filling it, the source map may not be present yet)
            foreach (var sourceNode in checkedSourceNodes.Where(n => _mapSourceAndName2SwordData.ContainsKey(n.Name)).ToList())
            {
                foreach (var resource in _mapSourceAndName2SwordData[sourceNode.Name].Values.Where(r => String.IsNullOrEmpty(filter) ||
                                                                                                        IsFilterMatch(filter, 
                                                                                                                      GetDisplayString(r.SwordModInfo.Name, r.SwordModInfo.Category, r.SwordModInfo.Description),
                                                                                                                      anyUpperCase)).ToList())
                {
                    AddToDownloadCheckBoxList(resource.SwordModInfo, resource.SwordRemoteSource);
                }
            }
        }

        public class CheckBoxListItem
        {
            public string Source { get; set; }
            public string Text { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }

        private bool AddToDownloadCheckBoxList(ModInfo modInfo, string source)
        {
            var keyNode = treeViewSourcesDownloadable.Nodes.Find(source, false).First();

            // make sure we haven't already added it
            if (keyNode.Nodes.Find(modInfo.Name, false).Any())
                return false;

            var itemText = GetDisplayString(modInfo.Name, modInfo.Category, modInfo.Description);
            var isInstalled = IsAlreadyInstalled(modInfo);
            var item = new TreeNode(itemText) { Name = modInfo.Name, Checked = isInstalled, Tag = keyNode };  // add the source node as Tag, so we know which one to add it to

            if (backgroundWorkerLoadDownloadListBox.IsBusy)
                backgroundWorkerLoadDownloadListBox.ReportProgress(1, item);
            else
                keyNode.Nodes.Add(item);

            return true;
        }

        private static string GetDisplayString(string moduleName, string moduleCategory, string moduleDescription)
        {
            return (moduleCategory != NetBibleViewer.SwordResourceCategoryBiblicalText)
                            ? $"{moduleName} ({moduleCategory}): {moduleDescription}"
                            : $"{moduleName}: {moduleDescription}";
        }

        private void backgroundWorkerLoadDownloadListBox_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                var sourceTreeNode = e.UserState as TreeNode;
                treeViewSourcesDownloadable.Nodes.Add(sourceTreeNode);
            }
            else if (e.ProgressPercentage == 1)
            {
                var resourceTreeNode = e.UserState as TreeNode;
                var sourceTreeNode = resourceTreeNode.Tag as TreeNode;
                sourceTreeNode.Nodes.Add(resourceTreeNode);
            }
            else if (e.ProgressPercentage == 2)
            {
                var cursor = e.UserState as Cursor;
                Cursor = cursor;
            }
            else
            {
                var data = e.UserState as string;
                if (e.ProgressPercentage == 3)
                {
                    labelDownloadProgress.Text = data;
                }
                else if (e.ProgressPercentage == 4)
                {
                    labelDownloadProgress.Text = String.Format(Localizer.Str("Gathering resources available from {0}..."), data);
                }
                else if (e.ProgressPercentage == 5)
                {
                    labelDownloadProgress.Text += data;
                    labelDownloadProgress.Update();
                }
                else 
                    System.Diagnostics.Debug.Fail("Did you add a new type?");
            }
        }

        private void treeViewSourcesDownloadable_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var cursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                ProcessCheckChangeInTreeView(e);
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
            finally
            {
                Cursor = cursor;
            }
        }

        private void ProcessCheckChangeInTreeView(TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)  // means this is a resource node. Go ahead and download it now
            {
                var data = _mapSourceAndName2SwordData[e.Node.Parent.Name][e.Node.Name];
                if (e.Node.Checked)
                {
                    var upgradePath = Path.Combine(Program.UpgradeCacheDir, NetBibleViewer.SwordResourcePathSubfolderName);
                    Directory.CreateDirectory(upgradePath);
                    using (Manager manager = new Manager(upgradePath))
                    {
                        var installManager = CrossWireInstallManager;

                        if (installManager.RemoteInstallModule(manager, data.SwordRemoteSource, data.SwordShortCode))
                        {
                            data.ModsDfile = Path.Combine(Path.Combine(upgradePath, CstrPathModsD),
                                                          $"{data.SwordShortCode}.conf");
                            if (!GetInformation(data.ModsDfile, ref data))
                                return;

                            var pathToDataFiles = Path.Combine(Program.UpgradeCacheDir, data.ModulesDataPath);
                            SetFilePathSafe(data, pathToDataFiles);
                        }
                        else
                        {
                            LocalizableMessageBox.Show(
                                String.Format(Localizer.Str("Some error is preventing the '{0}' SWORD module from being downloaded from the {1} repository."),
                                              data.SwordShortCode, data.SwordRemoteSource),
                                StoryEditor.OseCaption);
                        }
                        CrossWireInstallManager = null;
                    }
                }
                else
                {
                    data.SwordDataFiles = null;
                    data.ModsDfile = null;
                    data.ModulesDataPath = null;
                }
            }
            else    // a source checkbox was checked (or unchecked)
            {
                var source = e.Node.Name;
                if (e.Node.Checked)
                {
                    // do we need to add it back?
                    var filter = textBoxFilter.Text;
                    var anyUpperCase = filter.Any(char.IsUpper);
                    foreach (var modInfo in _mapSourceAndName2SwordData[source].Values.Where(r => String.IsNullOrEmpty(filter) ||
                                                                                                  IsFilterMatch(filter,
                                                                                                                GetDisplayString(r.SwordModInfo.Name, r.SwordModInfo.Category, r.SwordModInfo.Description),
                                                                                                                anyUpperCase))
                                                                               .Select(s => s.SwordModInfo))
                    {
                        AddToDownloadCheckBoxList(modInfo, source);
                    }
                    AddRemoveSwordSourceSetting(source, exclude: false);
                }
                else
                {
                    var sourceNode = treeViewSourcesDownloadable.Nodes.Find(source, false).First();
                    sourceNode.Nodes.Clear();
                    AddRemoveSwordSourceSetting(source, exclude: true);
                }
            }
        }

        private static void SetFilePathSafe(SwordModuleData data, string pathToDataFiles)
        {
            // for some reason, a large number of *.conf files contain bad path to the data files (usually one sub-folder too much)
            // so make sure the folder exists, before you look for the files:
            while (!Directory.Exists(pathToDataFiles))
                pathToDataFiles = Path.GetDirectoryName(pathToDataFiles);

            data.SwordDataFiles = Directory.GetFiles(pathToDataFiles).ToList();
        }

        private static void AddRemoveSwordSourceSetting(string source, bool exclude)
        {
            var alreadyExcluded = Properties.Settings.Default.SwordSourcesToExclude.Contains(source);
            if (exclude && !alreadyExcluded)
            {
                Properties.Settings.Default.SwordSourcesToExclude.Add(source);
            }
            else if (!exclude && alreadyExcluded)
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
        public string ModsDfile { get; set; }   // path to .conf file
        public List<string> SwordDataFiles { get; set; }    // path to the data files (e.g. in ./modules/texts/ztext/azeri/)
        public string ModulesDataPath { get; set; }
        public bool DirectionRtl { get; set; }
    }
}
