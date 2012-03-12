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
            // commenting out the base class call (if you do have it, then you really need
            //  to call all of them in the setup project's custom actions)
            // base.Commit(savedState);
            // RunFixupProgram();
            InitializeEncConverter();
        }

        /// <summary>
        /// This method will run the EncConvertersAppDataMover40.exe program (during 
        /// installation when we have elevated privileges, because that's needed in
        /// order to add a registry key to the registry for machines that don't have
        /// SEC otherwise installed.
        /// </summary>
        private void InitializeEncConverter()
        {
            var theEcs = new SilEncConverters40.EncConverters();
        }

        private void RunFixupProgram()
        {
            string strRunPath = Path.Combine(StoryProjectData.GetRunningFolder, "FixupOneStoryFile.exe");
            if (File.Exists(strRunPath))
                StoryEditor.LaunchProgram(strRunPath, null);
        }
    }
}
