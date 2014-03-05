using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OneStoryProjectEditor;

namespace AiChorus
{
    [XmlRoot]
    public class ChorusConfigurations
    {
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
    }
}
