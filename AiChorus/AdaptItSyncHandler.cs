using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AiChorus.Properties;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;
using Chorus.sync;
using SilEncConverters40;

namespace AiChorus
{
    public class AdaptItSyncHandler : ApplicationSyncHandler
    {
        public AdaptItSyncHandler(Project project, ServerSetting serverSetting)
            : base(project, serverSetting)
        {
        }

        public override string AppDataRoot
        {
            get { return Program.AdaptItWorkFolder; }
        }

        public override void DoSynchronize()
        {
            var strProjectFolder = Path.Combine(AppDataRoot, Project.FolderName);
            var strProjectName = Project.ProjectId;

#if UseUrlsWithChorus
            var uri = new Uri("http://resumable.languagedepot.org");
            var strPassword = ServerSetting.DecryptedPassword;
            var strRepoUrl = String.Format("{0}://{1}{2}@{3}/{4}",
                                           uri.Scheme, ServerSetting.Username,
                                           (String.IsNullOrEmpty(strPassword))
                                               ? null
                                               : ':' + strPassword,
                                           uri.Host, strProjectName);
#endif
            SyncWithAiRepo(strProjectFolder, strProjectName);
        }

        // taken wholesale from OSE (so we don't need to depend on OSE--before we just called OSE to do it)
        private void SyncWithAiRepo(string strProjectFolder, string strProjectName)
        {
            // AdaptIt creates the xml file in a different way than we'd like (it
            //  triggers a whole file change set). So before we attempt to merge, let's
            //  resave the file using the same approach that Chorus will use if a merge
            //  is done so it'll always show differences only.
            string strKbFilename = Path.Combine(strProjectFolder,
                                                Path.GetFileNameWithoutExtension(strProjectFolder) + ".xml");
            if (File.Exists(strKbFilename))
            {
                try
                {
                    var strKbBackupFilename = strKbFilename + ".bak";
                    File.Copy(strKbFilename, strKbBackupFilename, true);
                    var doc = XDocument.Load(strKbBackupFilename);
                    File.Delete(strKbFilename);
                    doc.Save(strKbFilename);
                }
                catch
                {
                }
            }

            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);

            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.Lazy, SyncUIFeatures.NormalRecommended))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
#if UseUrlsWithChorus
                // shouldn't need to do this, bkz we wouldn't be here if it wasn't already cloned
                if (!String.IsNullOrEmpty(strRepoUrl))
                    dlg.SyncOptions.RepositorySourcesToTry.Add(RepositoryAddress.Create(CstrInternetName, strRepoUrl));
#endif
                dlg.Text = Resources.SynchronizingAdaptItDialogTitle + strProjectName;
                dlg.ShowDialog();
            }
        }

        protected override string GetSynchronizeOrOpenProjectLable
        {
            get
            {
                return (_lstJustClonedProjects.Any(aEc => Path.GetFileNameWithoutExtension(aEc.ConverterIdentifier) == Project.FolderName))
                            ? CstrOptionOpenProject
                            : base.GetSynchronizeOrOpenProjectLable;
            }
        }

        private static List<AdaptItEncConverter> _lstJustClonedProjects = new List<AdaptItEncConverter>();
        internal override bool DoClone()
        {
            if (!base.DoClone())
                return false;
            var theAiEc = Program.InitializeLookupConverter(Path.Combine(AppDataRoot, Project.FolderName));
            if (theAiEc != null)
                _lstJustClonedProjects.Add(theAiEc);
            return true;
        }

        public override void DoProjectOpen()
        {
            var theAiEc =
                _lstJustClonedProjects.FirstOrDefault(
                    aEc => Path.GetFileNameWithoutExtension(aEc.ConverterIdentifier) == Project.FolderName);
            Program.DisplayKnowledgeBase(theAiEc);
        }

        public override ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            var projectConfig = base.GetProjectFolderConfiguration(strProjectFolder);

            // initalize the files that we want the repo to keep track of            
            projectConfig.IncludePatterns.Add("*.xml"); // AI KB
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            projectConfig.IncludePatterns.Add("*.aic");
            projectConfig.IncludePatterns.Add("*.cct"); // possible normalization spellfixer files
            return projectConfig;
        }
    }
}
