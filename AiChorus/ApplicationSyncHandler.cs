using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Chorus.Model;
using Chorus.sync;
using Chorus.UI.Sync;

namespace AiChorus
{
    public abstract class ApplicationSyncHandler
    {
        public const string CstrOptionSendReceive = "Synchronize";
        public const string CstrOptionClone = "Download";
        public const string CstrOptionHidden = "Hidden";            // if we're hiding from syncing
        public const string CstrOptionOpenProject = "Open Project";

        public Project Project { get; set; }
        public ServerSetting ServerSetting { get; set; }

        public abstract string AppDataRoot { get; }

        protected ApplicationSyncHandler(Project project, ServerSetting serverSetting)
        {
            Project = project;
            ServerSetting = serverSetting;
        }

        public string ButtonLabel
        {
            get
            {
                return CheckForProjectFolder();
            }
        }

        protected string CheckForProjectFolder()
        {
            var strProjectFolderPath = Path.Combine(AppDataRoot, Project.FolderName);
            if (!Directory.Exists(strProjectFolderPath))
                return CstrOptionClone;

            // check for the '.hg' folder
            var strProjectFolderHgPath = Path.Combine(strProjectFolderPath, ".hg");
            return Directory.Exists(strProjectFolderHgPath) 
                        ? GetSynchronizeOrOpenProjectLabel 
                        : CstrOptionClone;
        }

        /// <summary>
        /// this normally gets just 'Synchronize', but some sub-classes might want to 
        /// override and also provide the 'Open Project' option
        /// </summary>
        public virtual string GetSynchronizeOrOpenProjectLabel
        {
            get { return CstrOptionSendReceive; }
        }

        public abstract void DoSynchronize();
        public void DoSilentSynchronize()
        {
            // for when we launch the program, just do a quick & dirty send/receive, 
            //  but for closing (or if we have a network drive also), then we want to 
            //  be more informative
            var strProjectFolder = Path.Combine(AppDataRoot, Project.FolderName);
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);
            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished, SyncUIFeatures.Minimal))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                dlg.Text = "Synchronizing Project: " + Project.FolderName;
                dlg.ShowDialog();
                if (!dlg.SyncResult.Succeeded)
                {
                    Program.LogMessage($"Silent sync failed for {strProjectFolder}. Reason: {dlg.SyncResult.ErrorEncountered}");
                }
            }
        }


        internal virtual bool DoClone()
        {
            return Program.CloneProject(ServerSetting, Project, AppDataRoot);
        }

        protected static string OneStoryEditorExePath
        {
            get
            {
                var strStoryEditorPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Debug.Assert(strStoryEditorPath != null, "strStoryEditorExePath != null");
                OseSyncHandler.OseRunningPath = strStoryEditorPath = Path.Combine(strStoryEditorPath, OseSyncHandler.CstrStoryEditorExe);
                return strStoryEditorPath;
            }
        }

        // must be implmented by sub-classes who return CstrOptionOpenProject for the label
        public virtual void DoProjectOpen()
        {
            throw new NotImplementedException();
        }

        public virtual ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            // until some future version of Chorus, we might need to programmatically trigger
            //  the update to the hgrc file... check if that's needed
            var hgrcPath = Path.Combine(Path.Combine(strProjectFolder, ".hg"), "hgrc");
            var hgrcLines = File.ReadAllLines(hgrcPath);
            var languageDepotPaths = hgrcLines.Where(l => l.Contains("languagedepot.org")).ToList();
            if (languageDepotPaths.Any())
            {
                var model = new ServerSettingsModel()
                {
                    Username = ServerSetting.Username,
                    Password = ServerSetting.DecryptedPassword
                };
                model.InitFromProjectPath(strProjectFolder);
                model.SaveSettings();
            }
            return new ProjectFolderConfiguration(strProjectFolder);
        }
    }
}
