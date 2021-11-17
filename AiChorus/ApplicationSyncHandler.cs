using System;
using System.IO;
using Chorus.sync;

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

        public abstract void DoSynchronize(bool bIsOpening);
        public void DoSilentSynchronize()
        {
            // in the latest Chorus, they remove the username and password from the hgrc file and store it in a settings file that
            //  is based on the exe that clones the project. So we rely on StoryEditor.exe to do the cloning and then it'll be in 
            //  the correct settings file. But that also means when we want to synchronize, we'll have to have StoryEditor do it also.
            // "Silent" just means 'not opening' (so false for the isOpening parameter)
            var strProjectFolder = Path.Combine(AppDataRoot, Project.FolderName);
            var args = $"/sync \"{strProjectFolder}\" \"\" false {Project.ProjectId}";
            Program.LaunchProgram(Program.OneStoryEditorExePath, args, true);
        }

        protected delegate ProjectFolderConfiguration ProjectFolderConfigurationFunc(string strFolderPath);

        protected void TrySyncWithRepository(string strProjectFolder, string strProjectName, bool bIsOpening, string projectType)
        {
            // the project folder name has come here bogus at times...
            if (String.IsNullOrEmpty(strProjectFolder) || String.IsNullOrEmpty(strProjectName))
                return;

            // in the latest Chorus, they remove the username and password from the hgrc file and store it in a settings file that
            //  is based on the exe that clones the project. So we rely on StoryEditor.exe to do the cloning and then it'll be in 
            //  the correct settings file. But that also means when we want to synchronize, we'll have to have StoryEditor do it also.
            var args = $"/sync \"{strProjectFolder}\" \"{projectType}\" {bIsOpening} {strProjectName}";
            Program.LaunchProgram(Program.OneStoryEditorExePath, args, true);
        }

        internal virtual bool DoClone()
        {
            return Program.CloneProject(ServerSetting, Project, AppDataRoot);
        }

        // must be implmented by sub-classes who return CstrOptionOpenProject for the label
        public virtual void DoProjectOpen()
        {
            throw new NotImplementedException();
        }
    }
}
