#define AiChorusInOseFolder

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;

namespace AiChorus
{
    public class OseSyncHandler : ApplicationSyncHandler
    {
        public const string CstrStoryEditorExe = "StoryEditor.exe";
        private const string CstrCantOpenOse = "Can't Open OneStory Editor";

        private Assembly _theStoryEditor;
#if !AiChorusInOseFolder
        private MethodInfo _methodSyncWithRepository;
#endif

        public OseSyncHandler(Project project, ServerSetting serverSetting)
            : base(project, serverSetting)
        {
            
        }

        public static string OseRunningPath;
        private static string _appDataRoot;
        public override string AppDataRoot
        {
            get 
            {
                if (_appDataRoot == null)
                {
#if AiChorusInOseFolder
                    var strStoryEditorPath = OneStoryEditorExePath;
#else
                    // first see if OSE is installed
                    // check in Program Files\SIL\OneStory Editor 2.0
                    string strStoryEditorExePath =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                     Path.Combine("SIL", Path.Combine("OneStory Editor 2.0", "StoryEditor.exe")));

                    if (!File.Exists(strStoryEditorExePath))
                    {
                        MessageBox.Show("Unable to find OneStory Editor! Is it installed?", Properties.Resources.AiChorusCaption);
                        return "Can't Find OneStory Editor";
                    }
#endif

                    // if it is there, then get the path to the project folder root
                    _theStoryEditor = Assembly.LoadFile(strStoryEditorPath);
                    if (_theStoryEditor == null)
                        return CstrCantOpenOse;

                    var typeOseProjectSettings = _theStoryEditor.GetType("OneStoryProjectEditor.ProjectSettings");
                    if (typeOseProjectSettings == null)
                        return CstrCantOpenOse;

                    var typeString = typeof (string);
#if !AiChorusInOseFolder
                    var aTypeParams = new Type[] { typeString, typeString, typeString, typeString, typeString, typeString };
                    _methodSyncWithRepository = typeOseProjectSettings.GetMethod("SyncWithRepository", aTypeParams);
#endif
                    var propOneStoryProjectFolderRoot = typeOseProjectSettings.GetProperty("OneStoryProjectFolderRoot");
                    _appDataRoot = (string)propOneStoryProjectFolderRoot.GetValue(typeOseProjectSettings, null);
                }

                return _appDataRoot;
            }
        }

        private static readonly List<string> _lstProjectsJustCloned = new List<string>();
        protected override string GetSynchronizeOrOpenProjectLable
        {
            get
            {
                return (_lstProjectsJustCloned.Contains(Project.FolderName))
                           ? CstrOptionOpenProject
                           : base.GetSynchronizeOrOpenProjectLable;
            }
        }

        public override void DoSynchronize()
        {
#if AiChorusInOseFolder
            var strCommandLine   = Path.Combine(Path.Combine(AppDataRoot, Project.FolderName),
                                              Project.ProjectId + ".onestory");
            Program.LaunchProgram(OseRunningPath, strCommandLine);
#else
            /* string strProjectFolder, string strProjectName, string strHgRepoUrlHost,
            string strUsername, string strPassword*/
            var oParams = new object[]
                              {
                                  Path.Combine(AppDataRoot, Project.FolderName), 
                                  Project.ProjectId,
                                  "http://resumable.languagedepot.org",    // for now
                                  ServerSetting.Username, 
                                  ServerSetting.Password, 
                                  null                                      // shared network path
                              };
            _methodSyncWithRepository.Invoke(_theStoryEditor, oParams);
#endif
        }

        public override void DoSilentSynchronize()
        {
            // for when we launch the program, just do a quick & dirty send/receive, 
            //  but for closing (or if we have a network drive also), then we want to 
            //  be more informative
            var strProjectFolder = Path.Combine(AppDataRoot, Project.FolderName);
            var projectConfig = GetProjectFolderConfiguration(strProjectFolder);
            using (var dlg = new SyncDialog(projectConfig, SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished, SyncUIFeatures.Minimal))
            {
                dlg.UseTargetsAsSpecifiedInSyncOptions = true;
                dlg.Text = "Synchronizing OneStory Project: " + Project.FolderName;
                dlg.ShowDialog();
            }

        }

        public static ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            var projectConfig = new ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            return projectConfig;
        }

        internal override bool DoClone()
        {
            if (base.DoClone())
            {
                _lstProjectsJustCloned.Add(Project.FolderName);
                return true;
            }
            return false;
        }

        public override void DoProjectOpen()
        {
            // for us, this is the same as Synchronize
            DoSynchronize();
        }
    }
}
