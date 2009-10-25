using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;


namespace OneStoryProjectEditor
{
    [RunInstaller(true)]
    public partial class InstallerClass : Installer
    {
        public InstallerClass()
        {
            InitializeComponent();
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);
            RunFixupProgram();
        }

        private void RunFixupProgram()
        {
            string strRunPath = Path.Combine(StoryProjectData.GetRunningFolder, "FixupOneStoryFile.exe");
            if (File.Exists(strRunPath))
                StoryEditor.LaunchProgram(strRunPath, null);
        }
    }
}
