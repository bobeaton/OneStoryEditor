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
            if (Directory.Exists(strProjectFolderPath))
            {
                // check for the '.hg' folder
                var strProjectFolderHgPath = Path.Combine(strProjectFolderPath, ".hg");
                if (Directory.Exists(strProjectFolderHgPath))
                {
                    // means it's already been cloned, so just allow sync'ing
                    return CstrOptionSendReceive;
                }
            }

            return CstrOptionClone;
        }

        public abstract void DoSynchronize();

        internal virtual void DoClone()
        {
            Program.CloneProject(ServerSetting, Project, AppDataRoot);
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
    }
}
