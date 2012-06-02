using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var strStoryEditorExePath = OneStoryEditorExePath;

            // if it is there, then get the path to the project folder root
            var theStoryEditor = Assembly.LoadFile(strStoryEditorExePath);
            if (theStoryEditor == null)
                return /* CstrCantOpenOse */;

            var typeOseProgram = theStoryEditor.GetType("OneStoryProjectEditor.Program");
            if (typeOseProgram == null)
                return /* CstrCantOpenOse */;

            var typeString = typeof (string);
            var aTypeParams = new Type[] { typeString, typeString, typeString, typeString, typeString, typeString, typeof(bool) };
            var methodSyncWithAiRepository = typeOseProgram.GetMethod("SyncWithAiRepository", aTypeParams);

            /*
            public static void SyncWithAiRepository(string strProjectFolder, string strProjectName, string strRepoUrl,
                                                    string strSharedNetworkUrl, string strHgUsername, string strHgPassword, 
                                                    bool bRewriteAdaptItKb)
            */

            var oParams = new object[]
                              {
                                  Path.Combine(AppDataRoot, Project.FolderName), 
                                  Project.ProjectId,
                                  "http://resumable.languagedepot.org",    // for now
                                  null,                                     // shared network path
                                  ServerSetting.Username, 
                                  ServerSetting.Password, 
                                  true
                              };
            methodSyncWithAiRepository.Invoke(theStoryEditor, oParams);
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
    }
}
