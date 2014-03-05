using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AiChorus
{
    public abstract class ApplicationSyncHandler
    {
        public const string CstrOptionSendReceive = "Synchronize";
        public const string CstrOptionClone = "Download";
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
                        ? GetSynchronizeOrOpenProjectLable 
                        : CstrOptionClone;
        }

        /// <summary>
        /// this normally gets just 'Synchronize', but some sub-classes might want to 
        /// override and also provide the 'Open Project' option
        /// </summary>
        protected virtual string GetSynchronizeOrOpenProjectLable
        {
            get { return CstrOptionSendReceive; }
        }

        public abstract void DoSynchronize();
        public abstract void DoSilentSynchronize();

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
    }
}
