using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using OneStoryProjectEditor;

namespace AiChorus
{
    [XmlRoot]
    public class ChorusConfigurations
    {
        [XmlIgnore]
        public string FileSpec { get; set; }

        public  ChorusConfigurations()
        {
            ServerSettings = new List<ServerSetting>();
        }

        [XmlElement(ElementName = "ServerSetting")]
        public List<ServerSetting> ServerSettings { get; set; }        

        public void SaveAsXml(string strFilepath)
        {
            var x = new XmlSerializer(typeof(ChorusConfigurations));

            Program.MakeBackupOfOutputFile(strFilepath);
            var ws = new XmlWriterSettings
                            {
                                Indent = true
                            };
            var w = XmlWriter.Create(strFilepath, ws);

            // no need for the default namespaces that get added
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            // Serialize it out
            x.Serialize(w, this, ns);
            w.Close();
        }

        /// <summary>
        /// Load the 'Backorder by Item with SKU information' csv file.
        /// Assumed that the filename is INR0822D.csv and you pass in the path where it's located
        /// </summary>
        /// <param name="strFilePath">path to the Chorus Configurations xml file</param>
        /// <returns>the ChorusConfigurations with the loaded project data</returns>
        public static ChorusConfigurations Load(string strFilePath)
        {
            if (!String.IsNullOrEmpty(strFilePath) && File.Exists(strFilePath))
            {
                var serializer = new XmlSerializer(typeof(ChorusConfigurations));
                using (TextReader r = new StreamReader(strFilePath, Encoding.UTF8))
                {
                    var data = (ChorusConfigurations)serializer.Deserialize(r);
                    data.FileSpec = strFilePath;
                    return data;
                }
            }
            return null;
        }

        public bool TryGet(string serverName, out ServerSetting serverSetting)
        {
            foreach (var setting in ServerSettings.Where(setting => setting.ServerName == serverName))
            {
                serverSetting = setting;
                return true;
            }
            serverSetting = null;
            return false;
        }
    }

    public class ServerSetting
    {
        public ServerSetting()
        {
            Projects = new List<Project>();
        }

        [XmlAttribute]
        public string Username { get; set; }

        [XmlAttribute]
        public string Password { get; set; }

        [XmlAttribute]
        public bool IsPasswordEncrypted { get; set; }

        [XmlAttribute]
        public string ServerName { get; set; }

        /// <summary>
        /// This member gives the url to a google spreadsheet to be first emptied and then filled with the data requested (see GoogleDocProjections)
        /// for the projects in this *.cpc file. 
        /// </summary>
        [XmlAttribute]
        public string GoogleSheetUrl { get; set; }    // e.g. https://docs.google.com/spreadsheets/d/1hnKQ_qCbl_Pxp7pDPYfLHgiwVjoyjz991fM8TLf2TUE/edit?usp=sharing

        private Regex _regEx = new Regex("/spreadsheets/d/([a-zA-Z0-9-_]+)");

        [XmlIgnore]
        public string GoogleSheetId
        {
            get { return _regEx.Match(GoogleSheetUrl).Groups[1].Value; }
        }

        [XmlElement(ElementName = "Project")]
        public List<Project> Projects { get; set; }

        public bool TryGet(string projectId, out Project projectFound)
        {
            foreach (var project in Projects.Where(project => project.ProjectId == projectId))
            {
                projectFound = project;
                return true;
            }

            projectFound = null;
            return false;
        }

        public string DecryptedPassword
        {
            get
            {
                return (IsPasswordEncrypted)
                    ? EncryptionClass.Decrypt(Password)
                    : Password;
            }
        }
    }

    public class Project
    {
        [XmlAttribute]
        public string ApplicationType { get; set; }

        [XmlAttribute]
        public string ProjectId { get; set; }

        [XmlAttribute]
        public string FolderName { get; set; }

        /// <summary>
        /// Set this attr to 'true' to have the project not be sync'd on Nathan Payne's machine for
        /// daily updates
        /// </summary>
        [DefaultValue(false)]
        [XmlAttribute]
        public bool ExcludeFromSyncing { get; set; } = false;

        /// <summary>
        /// Set this attr to 'true' to have this project excluded from a configured GoogleSheet extract (see 
        /// GoogleSheetUrl above)
        /// </summary>
        [DefaultValue(false)]
        [XmlAttribute]
        public bool ExcludeFromGoogleSheet { get; set; } = false;
    }
}
