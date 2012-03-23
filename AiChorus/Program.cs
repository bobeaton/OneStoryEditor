using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using AiChorus.Properties;
using Chorus.UI.Clone;
using ECInterfaces;
using SilEncConverters40;

namespace AiChorus
{
    static class Program
    {
        public const string CstrApplicationTypeOse = "OneStory Editor";
        public const string CstrApplicationTypeAi = "Adapt It";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }

            if (args.Length == 0)
            {
                MessageBox.Show(Resources.UsageString, Resources.AiChorusCaption);
                return;
            }

            try
            {
                if (args[0] == "/f")
                    ProcessChorusConfigFile((args.Length == 2) ? args[1] : null);
                else if ((args[0] == "/c") || String.IsNullOrEmpty(Settings.Default.LastProjectFolder))
                    ;   // DoClone();
                else if (args[0] == "/e")
                    DoEdit();
                else
                    LaunchProgram("Chorus.exe", Settings.Default.LastProjectFolder);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private static int ProjectCount(ChorusConfigurations chorusConfigurations)
        {
            return chorusConfigurations.ServerSettings.Sum(serverSetting => serverSetting.Projects.Count);
        }

        private static void ProcessChorusConfigFile(string strFilePath)
        {
            ChorusConfigurations chorusConfig;
            if (!String.IsNullOrEmpty(strFilePath))
                chorusConfig = ChorusConfigurations.Load(strFilePath);
            else
                chorusConfig = new ChorusConfigurations();

            if (chorusConfig == null)
            {
                MessageBox.Show("Invalid file format", Resources.AiChorusCaption);
            }
            else
            {
                PresentProjects(chorusConfig);
            }
        }

        private static void PresentProjects(ChorusConfigurations chorusConfig)
        {
            var projView = new PresentProjectsForm(chorusConfig);
            if (projView.ShowDialog() == DialogResult.OK)
                ;
        }

        private static void DoEdit()
        {
            var theEc =
                DirectableEncConverter.EncConverters.AutoSelectWithTitle(ConvType.Unicode_to_from_Unicode,
                                                                         "Choose the 'Lookup in ...' item whose Knowledge base you want to edit and click 'OK'");
            
            if (theEc == null)
                return;

            if (!(theEc is AdaptItEncConverter))
            {
                if (MessageBox.Show(Resources.MustUseAiLookupConverterToEdit,
                                Resources.AiChorusCaption,
                                MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    DoEdit();
                return;
            }

            try
            {
                var theAiEc = (AdaptItEncConverter)theEc;
                var strData = GrabDataPoint(theAiEc.ConverterIdentifier);
                strData = theAiEc.EditKnowledgeBase(strData);
                theAiEc.Configurator.DisplayTestPage(DirectableEncConverter.EncConverters,
                                                     theAiEc.Name,
                                                     theAiEc.ConverterIdentifier,
                                                     ConvType.Unicode_to_from_Unicode,
                                                     strData);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }
        /*
        internal static void DoClone()
        {
            var strAdaptItWorkFolder = AdaptItWorkFolder;
            if (!Directory.Exists(strAdaptItWorkFolder))
                Directory.CreateDirectory(strAdaptItWorkFolder);
#if DEBUG
            const string cstrHindiToUrdu = "Hindi to Urdu adaptations";
            var strHindiToUrduProjectFolder = Path.Combine(strAdaptItWorkFolder, cstrHindiToUrdu);
            if (Directory.Exists(strHindiToUrduProjectFolder))
                Directory.Delete(strHindiToUrduProjectFolder, true);
            var strProjectId = "aikb-hindi-urdu";
            var strAccountName = "bobeaton";
            var strLocalFolderName = cstrHindiToUrdu;
            var strServerName = Resources.IDS_DefaultRepoServer;
            var strPassword = "helpmepld";
#endif
            CloneProject(strLocalFolderName, strServerName, strAccountName, strAdaptItWorkFolder, strPassword, strProjectId);
        }
        */
        internal static bool CloneProject(ServerSetting serverSetting, Project project, string strProjectFolderRoot)
        {
            return CloneProject(project.FolderName, serverSetting.ServerName, serverSetting.Username,
                         strProjectFolderRoot, serverSetting.Password, project.ProjectId);
        }

        private static bool CloneProject(string strLocalFolderName, string strServerName, string strAccountName,
                                         string strProjectFolderRoot, string strPassword, string strProjectId)
        {
            if (!Directory.Exists(strProjectFolderRoot))
                Directory.CreateDirectory(strProjectFolderRoot);

            var model = new GetCloneFromInternetModel(strProjectFolderRoot)
                            {
                                ProjectId = strProjectId,
                                AccountName = strAccountName,
                                Password = strPassword,
                                LocalFolderName = strLocalFolderName,
                                SelectedServerLabel = strServerName
                            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.Cancel == dlg.ShowDialog())
                    return false;

                var strProjectFolder = dlg.PathToNewProject;
                Settings.Default.LastProjectFolder = strProjectFolder;
                Settings.Default.Save();
            }
            return true;
        }

        public static void InitializeLookupConverter(string strProjectFolder)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);

            // in case AI isn't installed yet, it really doesn't like not having an Adaptations sub-folder
            var strAdaptationsFolder = Path.Combine(strProjectFolder, "Adaptations");
            if (!Directory.Exists(strAdaptationsFolder))
                Directory.CreateDirectory(strAdaptationsFolder);

            var strFriendlyName = "Lookup in " + strProjectName;
            var strConverterSpec = Path.Combine(strProjectFolder, strProjectName + ".xml");
            var aEcs = new EncConverters(true);
            aEcs.AddConversionMap(strFriendlyName, strConverterSpec, ConvType.Unicode_to_from_Unicode,
                                  "SIL.AdaptItKB", "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);

            // we can save this information so we can use it automatically during the next restart
            var aEc = aEcs[strFriendlyName];

            var strData = GrabDataPoint(strConverterSpec);
            aEc.Configurator.DisplayTestPage(aEcs, strFriendlyName, strConverterSpec, ConvType.Unicode_to_from_Unicode, strData);
        }

        // kind of a brute force approach, but it's easy. 
        //  This will return the first source word in the KB as a data point
        private static string GrabDataPoint(string strConverterSpec)
        {
            var doc = XDocument.Load(strConverterSpec);
            var strData = doc.Descendants("TU").First().Attributes("k").First().Value;
            return strData;
        }

        public static string AdaptItWorkFolder
        {
            get
            {
                var strAdaptItWorkFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                        "Adapt It Unicode Work");
                return strAdaptItWorkFolder;
            }
        }

        public static void ShowException(Exception ex)
        {
            string strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            MessageBox.Show(strErrorMsg, Resources.AiChorusCaption);
        }

        public static void LaunchProgram(string strProgramPath, string strArguments)
        {
            try
            {
                var strWorkingDir = Path.GetDirectoryName(strProgramPath);
                var strFilename = Path.GetFileName(strProgramPath);
                var myProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = strFilename,
                        Arguments = "\"" + strArguments + "\"",
                        WorkingDirectory = strWorkingDir
                    }
                };

                myProcess.Start();
            }
            catch { }    // we tried...
        }

        /// <summary>
        /// Check to see if the output file exists and if so, make a backup and then delete it
        /// </summary>
        /// <param name="strFilepath">file path to the output to back up</param>
        public static void MakeBackupOfOutputFile(string strFilepath)
        {
            // first make sure the output directory exists
            Debug.Assert(strFilepath != null, "strXmlFilepathToValidate != null");

            string strParentFolder = Path.GetDirectoryName(strFilepath);
            if (!Directory.Exists(strParentFolder))
                Directory.CreateDirectory(strParentFolder);

            // then check to see if we have to make a copy (to keep the attributes, which 'rename' doesn't do)
            //  as a backup file.
            if (!File.Exists(strFilepath))
                return;

            // it exists, so make a backup
            var strBackupFilename = strFilepath + ".bak";
            File.Delete(strBackupFilename); // just in case there was already a backup
            File.Copy(strFilepath, strBackupFilename);
            File.Delete(strFilepath);
        }

        public static Dictionary<string, string> ArrayToDictionary(StringCollection data)
        {
            var map = new Dictionary<string, string>();
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1]);
            }

            return map;
        }
    }
}
