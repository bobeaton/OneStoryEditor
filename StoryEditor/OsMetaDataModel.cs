using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace OneStoryProjectEditor
{
    [XmlRoot]
    public class OsMetaDataModel
    {
        public const string CstrCurrentVersion = "1.0";

        public OsMetaDataModel()
        {
            Version = CstrCurrentVersion;
            OsProjects = new List<OsMetaDataModelRecord>();
        }

        [XmlAttribute]
        public string Version { get; set; }

        public List<OsMetaDataModelRecord> OsProjects { get; set; }

        [XmlIgnore]
        public string PathToMetaDataFile { get; set; }

        public static OsMetaDataModel Load(string strXmlFilePath)
        {
            var serializer = new XmlSerializer(typeof(OsMetaDataModel));
            var readFileStream = new FileStream(strXmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var data = (OsMetaDataModel)serializer.Deserialize(readFileStream);
            readFileStream.Close();
            data.PathToMetaDataFile = strXmlFilePath;
            return data;
        }

        public void Save()
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(PathToMetaDataFile));
            Save(PathToMetaDataFile);
        }

        public void Save(string strFilepath)
        {
            MakeBackupOfOutputFile(strFilepath);

            var directoryName = Path.GetDirectoryName(strFilepath);
            Debug.Assert(directoryName != null, "directoryName != null");
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            var x = new XmlSerializer(typeof(OsMetaDataModel));
            var sw = new StreamWriter(strFilepath);

            // no need for the default namespaces that get added
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            // Serialize it out
            x.Serialize(sw, this, ns);
            sw.Close();
        }

        /// <summary>
        /// Check to see if the output file exists and if so, make a backup and then delete it
        /// </summary>
        /// <param name="strFilepathOutput">file path to the output to back up</param>
        public static void MakeBackupOfOutputFile(string strFilepathOutput)
        {
            // first make sure the output directory exists
            System.Diagnostics.Debug.Assert(strFilepathOutput != null, "strFilepathOutput == null");

            // then check to see if we have to make a copy (to keep the attributes, which 'rename' doesn't do)
            //  as a backup file.
            if (!File.Exists(strFilepathOutput))
                return;

            // it exists, so make a backup
            var strBackupFilename = strFilepathOutput + ".bak";
            File.Delete(strBackupFilename); // just in case there was already a backup
            File.Copy(strFilepathOutput, strBackupFilename);
            File.Delete(strFilepathOutput);
        }
    }

    public class OsMetaDataModelRecord
    {
        public OsMetaDataModelRecord()
        {
            LastQueriedPfNumOfSfgs = LastQueriedConsCheckMetaData = DateTime.MinValue;
        }

        public int Id { get; set; }                         // ID
        public string ProjectName { get; set; }             // Project_Name
        public string LanguageName { get; set; }            // Language_Name
        public string EthnologueCode { get; set; }          // Ethnologue_Code
        public string Continent { get; set; }               // Continent
        public string Country { get; set; }                 // Country
        public string Methodology { get; set; }             // Methodology
        public string ManagingPartner { get; set; }         // Managing_Partner
        public string Entity { get; set; }                  // Entity
        public string ContactPerson { get; set; }           // Contact_Person
        public string ContactPersonEmail { get; set; }      // Contact_Person_Email
        public string PrioritiesCategory { get; set; }      // Priorities_Category
        public string ScriptureStatus { get; set; }         // Scripture_Status
        public string ScriptureStatusNotes { get; set; }    // Scrip_Status_Notes
        public string ProjectFacilitators { get; set; }     // Project_Facilitators
        public string PfCategory { get; set; }              // PF_Category
        public string PfAffiliation { get; set; }           // PF_Affiliation
        public string Notes { get; set; }                   // Notes
        public string Status { get; set; }                  // Status
        public DateTime StartDate { get; set; }             // Start_Date
        public bool IsCurrentlyUsingOse { get; set; }       // currently_using_OSE
        public string OseProjId { get; set; }               // ose_proj_id
        public string EsConsultant { get; set; }            // ES_Consultant
        public string EsCoach { get; set; }                 // ES_Coach
        public int EsStoriesSent { get; set; }              // ES_stories_sent
        public int NumberSfgs { get; set; }                 // Number_SFGs
        public string PsConsultant { get; set; }            // PS_Consultant
        public string PsCoach { get; set; }                 // PS_Coach
        public int PsStoriesPrelimApprov { get; set; }      // PS_stories_prelim_approv
        public int NumInFinalApprov { get; set; }           // Number_Final_Stories
        public DateTime SetFinishedDate { get; set; }       // Set_Finished_Date
        public bool IsUploadedToOsMedia { get; set; }       // Uploaded_to_OSMedia
        public bool IsUploadedToTWR360 { get; set; }       // Uploaded_to_TWR360
        public string AlsoOnlineAt { get; set; }          // Set_Copyrighted
        public string SetCopyrighted { get; set; }          // Set_Copyrighted

        // these are kept so we can do calculation at a particular time, but are not
        //  part of the Access database
        public DateTime LastQueriedPfNumOfSfgs { get; set; }  
        public DateTime LastQueriedConsCheckMetaData { get; set; }

        public void SetValues(OsMetaDataModelRecord rhs)
        {
            ProjectName = rhs.ProjectName;
            LanguageName = rhs.LanguageName;
            EthnologueCode = rhs.EthnologueCode;
            Continent = rhs.Continent;
            Country = rhs.Country;
            Methodology = rhs.Methodology;
            ManagingPartner = rhs.ManagingPartner;
            Entity = rhs.Entity;
            ContactPerson = rhs.ContactPerson;
            ContactPersonEmail = rhs.ContactPersonEmail;
            PrioritiesCategory = rhs.PrioritiesCategory;
            ScriptureStatus = rhs.ScriptureStatus;
            ScriptureStatusNotes = rhs.ScriptureStatusNotes;
            ProjectFacilitators = rhs.ProjectFacilitators;
            PfCategory = rhs.PfCategory;
            PfAffiliation = rhs.PfAffiliation;
            Notes = rhs.Notes;
            Status = rhs.Status;
            StartDate = rhs.StartDate;
            IsCurrentlyUsingOse = rhs.IsCurrentlyUsingOse;
            OseProjId = rhs.OseProjId;
            EsConsultant = rhs.EsConsultant;
            EsCoach = rhs.EsCoach;
            EsStoriesSent = rhs.EsStoriesSent;
            NumberSfgs = rhs.NumberSfgs;
            PsConsultant = rhs.PsConsultant;
            PsCoach = rhs.PsCoach;
            PsStoriesPrelimApprov = rhs.PsStoriesPrelimApprov;
            NumInFinalApprov = rhs.NumInFinalApprov;
            SetFinishedDate = rhs.SetFinishedDate;
            IsUploadedToOsMedia = rhs.IsUploadedToOsMedia;
            IsUploadedToTWR360 = rhs.IsUploadedToTWR360;
            AlsoOnlineAt = rhs.AlsoOnlineAt;
            SetCopyrighted = rhs.SetCopyrighted;
        }
    }
}
