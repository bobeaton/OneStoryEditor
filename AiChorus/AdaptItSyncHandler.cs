using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public override void DoSynchronize(bool bIsOpening)
        {
            var strProjectFolder = Path.Combine(AppDataRoot, Project.FolderName);
            TrySyncWithRepository(strProjectFolder, Project.ProjectId, bIsOpening, "Adapt It");
        }

        public override string GetSynchronizeOrOpenProjectLabel
        {
            get
            {
                return (_lstJustClonedProjects.Any(aEc => Path.GetFileNameWithoutExtension(aEc.ConverterIdentifier) == Project.FolderName))
                            ? CstrOptionOpenProject
                            : base.GetSynchronizeOrOpenProjectLabel;
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
