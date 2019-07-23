// #define DoOneAtAtime

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ekm.oledb.data;
using Microsoft.Win32;
using OneStoryProjectEditor;

namespace UpdateAccessDbWithOsMetaData
{
    class Program
    {
        private const string CstrCommandLineUpdateDatabase = "/u";
        private const string CstrCommandLineCreateMetaDataFilesFromDatabase = "/c";

        private const string CstrSubfolderToExtractedOsMetaDataFileFormat = "MetaDataFilesFromDatabase";
        private const string CstrOsMetaDataFilename = "OsMetaData.xml";

        static void Main(string[] args)
        {
            try
            {
                string strPathToAccessDatabase;
                if ((args.Length != 2) ||
                    !((args[0] == CstrCommandLineUpdateDatabase) || 
                      (args[0] == CstrCommandLineCreateMetaDataFilesFromDatabase)) ||
                    !File.Exists((strPathToAccessDatabase = args[1])))
                {
                    DisplayUsage();
                    return;
                }

                if (args[0] == CstrCommandLineCreateMetaDataFilesFromDatabase)
                    ProcessOsMetaDataDatabase(strPathToAccessDatabase);
                else
                    UpdateDatabaseFromOsMetaDataFiles(strPathToAccessDatabase);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            LogMessage("Finished... Click any key to exit...");
            Console.ReadLine();
        }

        private static void UpdateDatabaseFromOsMetaDataFiles(string strPathToAccessDatabase)
        {
            // get all the OSE project folders that have OsMetaData files in them.
            var projectFolders = Directory.GetDirectories(OneStoryProjectFolderRoot).Where(fldr =>
            {
                var strFileName = Path.GetFileName(fldr);
                var strFileSpec = Path.Combine(fldr, strFileName + ".onestory");
                var strOsMetaDataFile = Path.Combine(fldr, CstrOsMetaDataFilename);
                return File.Exists(strFileSpec) && File.Exists(strOsMetaDataFile);
            }).ToList();

            if (projectFolders.Count == 0)
            {
                LogMessage(String.Format("There are no OSE projects that have a '{0}' file in them!?", CstrOsMetaDataFilename));
                return;
            }

            var db = Db.Open(strPathToAccessDatabase);
            foreach (var projectFolder in projectFolders)
            {
                try
                {
                    ProcessProject(projectFolder, db);
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }

#if DoOneAtAtime
                if (!_bDoRest)
                {
                    LogMessage("Finished... Click any key to exit...");
                    _bDoRest = (Console.ReadLine() == "a");
                }
#endif
            }
        }

#if DoOneAtAtime
        private static bool _bDoRest = false;
#endif

        private static void ProcessProject(string strProjectFolder, DatabaseContext db)
        {
            var dateTimeOfProjectFile = GetProjectFileDateTime(strProjectFolder);
            var strOsMetaDataFile = Path.Combine(strProjectFolder, CstrOsMetaDataFilename);
            var osMetaData = OsMetaDataModel.Load(strOsMetaDataFile);
            var osMetaDataModelRecord = osMetaData.OsProjects.First();
            var record = db.ExecuteSingle("select * from Projects where ose_proj_id = @oseProjectId",
                                           new Param("@oseProjectId", osMetaDataModelRecord.OseProjId, OleDbType.VarWChar));
            if (record == null)
            {
                LogMessage(String.Format("inserting new record for project, '{0}' from data in '{1}'", 
                                         osMetaDataModelRecord.OseProjId, strOsMetaDataFile));
                // record doesn't exist, so insert it
                db.ExecuteNonQuery("insert into Projects " +
                                           "(Project_Name, Language_Name, Ethnologue_Code, Continent, Country, Methodology, " +
                                           "Managing_Partner, Entity, Contact_Person, Contact_Person_Email, Priorities_Category, " +
                                           "Scripture_Status, Scrip_Status_Notes, Project_Facilitators, " +
                                            "PF_Category, PF_Affiliation, Notes, Status, Start_Date, " +
                                            "currently_using_OSE, ose_proj_id, ES_Consultant, ES_Coach, ES_stories_sent, " +
                                            "Number_SFGs, PS_Consultant, PS_Coach, " +
                                            "PS_stories_prelim_approv, Number_Final_Stories, Set_Finished_Date, " +
                                            "Uploaded_to_OSMedia, Uploaded_to_TWR360, Also_Online_At, Set_Copyrighted, DateLastChangeProjectFile) " +
                                   "values (@ProjectName, @LanguageName, @EthnologueCode, @Continent, @Country, @Methodology, " +
                                           "@ManagingPartner, @Entity, @ContactPerson, @ContactPersonEmail, @PrioritiesCategory, " +
                                           "@ScriptureStatus, @ScriptureStatusNotes, @ProjectFacilitators, @PfCategory, " +
                                           "@PfAffiliation, @Notes, @Status, @StartDate, " +
                                           "@IsCurrentlyUsingOse, @OseProjId, @EsConsultant, " +
                                           "@EsCoach, @EsStoriesSent, @NumberSfgs, " +
                                           "@PsConsultant, @PsCoach, @PsStoriesPrelimApprov, @NumInFinalApprov, " +
                                           "@SetFinishedDate, @IsUploadedToOsMedia, @IsUploadedToTWR360, @AlsoOnlineAt, @SetCopyrighted, @DateLastChangeProjectFile)", 
                                   GetParameterArray(osMetaDataModelRecord, dateTimeOfProjectFile)
                                );
            }
            else
            {
                LogMessage(String.Format("updating record for project, '{0}' from data in '{1}'",
                                         osMetaDataModelRecord.OseProjId, strOsMetaDataFile));
                db.ExecuteNonQuery("UPDATE [Projects] " +
                                   "SET [Project_Name] = @ProjectName, " +
                                       "[Language_Name] = @LanguageName, " +
                                       "[Ethnologue_Code] = @EthnologueCode, " +
                                       "[Continent] = @Continent, " +
                                       "[Country] = @Country, " +
                                       "[Methodology] = @Methodology, " +
                                       "[Managing_Partner] = @ManagingPartner, " +
                                       "[Entity] = @Entity, " +
                                       "[Contact_Person] = @ContactPerson, " + 
                                       "[Contact_Person_Email] = @ContactPersonEmail, " +
                                       "[Priorities_Category] = @PrioritiesCategory, " + 
                                       "[Scripture_Status] = @ScriptureStatus, " +
                                       "[Scrip_Status_Notes] = @ScriptureStatusNotes, " + 
                                       "[Project_Facilitators] = @ProjectFacilitators, " + 
                                       "[PF_Category] = @PfCategory, " +
                                       "[PF_Affiliation] = @PfAffiliation, " +
                                       "[Notes] = @Notes, " + 
                                       "[Status] = @Status, " +
                                       "[Start_Date] = @StartDate, " +
                                       "[currently_using_OSE] = @IsCurrentlyUsingOse, " +
                                       "[ose_proj_id] = @OseProjId, " +
                                       "[ES_Consultant] = @EsConsultant, " +
                                       "[ES_Coach] = @EsCoach, " +
                                       "[ES_stories_sent] = @EsStoriesSent, " +
                                       "[Number_SFGs] = @NumberSfgs, " +
                                       "[PS_Consultant] = @PsConsultant, " +
                                       "[PS_Coach] = @PsCoach, " +
                                       "[PS_stories_prelim_approv] = @PsStoriesPrelimApprov, " +
                                       "[Number_Final_Stories] = @NumInFinalApprov, " +
                                       "[Set_Finished_Date] = @SetFinishedDate, " +
                                       "[Uploaded_to_OSMedia] = @IsUploadedToOsMedia, " +
                                       "[Uploaded_to_TWR360] = @IsUploadedToTWR360, " +
                                       "[Also_Online_At] = @AlsoOnlineAt, " +
                                       "[Set_Copyrighted] = @SetCopyrighted, " +
                                       "[DateLastChangeProjectFile] = @DateLastChangeProjectFile " + 
                                    "WHERE [ose_proj_id] = @OseProjId;",
                                    GetParameterArray(osMetaDataModelRecord, dateTimeOfProjectFile)
                                   );
            }
        }

        private static DateTime GetProjectFileDateTime(string strProjectFolder)
        {
            // start by assuming that the project file name is the same as the project folder name
            // e.g. get lb1-hindi from "C:\Users\BEaton\Documents\OneStory Editor Projects\lb1-hindi"
            var filename = Path.GetFileName(strProjectFolder);
            var filespec = Path.Combine(strProjectFolder, $"{filename}.onestory");

            var newestDateTime = DateTime.MinValue;
            if (File.Exists(filespec))
            {
                newestDateTime = File.GetLastWriteTime(filespec);
            }
            else
            {
                // see if we can find a single *.onestory file
                var onestoryFiles = Directory.GetFiles(strProjectFolder, "*.onestory")
                                             .ToList();

                if (onestoryFiles.Any())
                {
                    newestDateTime = onestoryFiles.Select(fs => new FileInfo(fs).LastWriteTime)
                                                  .Max();
                }
            }
            return newestDateTime;
        }

        private static Param[] GetParameterArray(OsMetaDataModelRecord osMetaDataModelRecord, DateTime dateTimeOfProjectFile)
        {
            return new[]
            {
                new Param("@ProjectName", CheckForNull(osMetaDataModelRecord.ProjectName), OleDbType.VarWChar),
                new Param("@LanguageName", CheckForNull(osMetaDataModelRecord.LanguageName), OleDbType.VarWChar),
                new Param("@EthnologueCode", CheckForNull(osMetaDataModelRecord.EthnologueCode), OleDbType.VarWChar),
                new Param("@Continent", CheckForNull(osMetaDataModelRecord.Continent), OleDbType.VarWChar),
                new Param("@Country", CheckForNull(osMetaDataModelRecord.Country), OleDbType.VarWChar),
                new Param("@Methodology", CheckForNull(osMetaDataModelRecord.Methodology), OleDbType.VarWChar),
                new Param("@ManagingPartner", CheckForNull(osMetaDataModelRecord.ManagingPartner), OleDbType.VarWChar),
                new Param("@Entity", CheckForNull(osMetaDataModelRecord.Entity), OleDbType.VarWChar),
                new Param("@ContactPerson", CheckForNull(osMetaDataModelRecord.ContactPerson), OleDbType.VarWChar),
                new Param("@ContactPersonEmail", CheckForNull(osMetaDataModelRecord.ContactPersonEmail), OleDbType.VarWChar),
                new Param("@PrioritiesCategory", CheckForNull(osMetaDataModelRecord.PrioritiesCategory), OleDbType.VarWChar),
                new Param("@ScriptureStatus", CheckForNull(osMetaDataModelRecord.ScriptureStatus), OleDbType.VarWChar),
                new Param("@ScriptureStatusNotes", CheckForNull(osMetaDataModelRecord.ScriptureStatusNotes), OleDbType.VarWChar),
                new Param("@ProjectFacilitators", CheckForNull(osMetaDataModelRecord.ProjectFacilitators), OleDbType.VarWChar),
                new Param("@PfCategory", CheckForNull(osMetaDataModelRecord.PfCategory), OleDbType.VarWChar),
                new Param("@PfAffiliation", CheckForNull(osMetaDataModelRecord.PfAffiliation), OleDbType.VarWChar),
                new Param("@Notes", CheckForNull(osMetaDataModelRecord.Notes), OleDbType.VarWChar),
                new Param("@Status", CheckForNull(osMetaDataModelRecord.Status), OleDbType.VarWChar),
                new Param("@StartDate", CheckForNull(osMetaDataModelRecord.StartDate), OleDbType.DBDate),
                new Param("@IsCurrentlyUsingOse", osMetaDataModelRecord.IsCurrentlyUsingOse, OleDbType.Boolean),
                new Param("@OseProjId", osMetaDataModelRecord.OseProjId, OleDbType.VarWChar),   // not allowed to be null!
                new Param("@EsConsultant", CheckForNull(osMetaDataModelRecord.EsConsultant), OleDbType.VarWChar),
                new Param("@EsCoach", CheckForNull(osMetaDataModelRecord.EsCoach), OleDbType.VarWChar),
                new Param("@EsStoriesSent", osMetaDataModelRecord.EsStoriesSent, OleDbType.Integer),
                new Param("@NumberSfgs", osMetaDataModelRecord.NumberSfgs, OleDbType.Integer),
                new Param("@PsConsultant", CheckForNull(osMetaDataModelRecord.PsConsultant), OleDbType.VarWChar),
                new Param("@PsCoach", CheckForNull(osMetaDataModelRecord.PsCoach), OleDbType.VarWChar),
                new Param("@PsStoriesPrelimApprov", osMetaDataModelRecord.PsStoriesPrelimApprov, OleDbType.Integer),
                new Param("@NumInFinalApprov", osMetaDataModelRecord.NumInFinalApprov, OleDbType.Integer),
                new Param("@SetFinishedDate", CheckForNull(osMetaDataModelRecord.SetFinishedDate), OleDbType.DBDate),
                new Param("@IsUploadedToOsMedia", osMetaDataModelRecord.IsUploadedToOsMedia, OleDbType.Boolean),
                new Param("@IsUploadedToTWR360", osMetaDataModelRecord.IsUploadedToTWR360, OleDbType.Boolean),
                new Param("@AlsoOnlineAt", CheckForNull(osMetaDataModelRecord.AlsoOnlineAt), OleDbType.VarWChar),
                new Param("@SetCopyrighted", CheckForNull(osMetaDataModelRecord.SetCopyrighted), OleDbType.VarWChar),
                new Param("@DateLastChangeProjectFile", CheckForNull(dateTimeOfProjectFile), OleDbType.DBDate)
            };
        }

        private static object CheckForNull(DateTime dateTime)
        {
            return (dateTime == DateTime.MinValue) ? (object)DBNull.Value : dateTime;
        }

        private static object CheckForNull(string str)
        {
            return (String.IsNullOrEmpty(str)) ? (object)DBNull.Value : str;
        }

        private static void ProcessOsMetaDataDatabase(string strPathToAccessDatabase)
        {
            var strOutputFolder = OsMetaDataExtractionFolder;
            LogMessage(String.Format("Would you like to delete all the existing sub-folders of the folder:{0}{0}{1}{0}{0}(if so, press 'y' and then the Enter key)",
                                     Environment.NewLine, strOutputFolder));
            var key = Console.ReadLine();
            if (key != "y")
                return;

            DeleteContentsOfFolder(strOutputFolder);

            // connect to the Access database
            var db = Db.Open(strPathToAccessDatabase);

            // this selects all the records for which there is an OSE project id
            var records = db.ExecuteMany("select * from Projects where ose_proj_id is not null");
            foreach (var record in records)
            {
                OsMetaDataModelRecord recordMapped = Mapper.Map<OsMetaDataModelRecord, OsMetaDataModelMapping>(record);
                CreateOsMetaDataFileForProject(recordMapped);
            }
        }

        private static void DeleteContentsOfFolder(string strFolderSpec)
        {
            var dirs = Directory.GetDirectories(strFolderSpec).ToList();
            dirs.ForEach(fldr => Directory.Delete(fldr, true));
        }

        private static void CreateOsMetaDataFileForProject(OsMetaDataModelRecord record)
        {
            System.Diagnostics.Debug.Assert(record.OseProjId != null);

            // get the path to the file in the OSE project folder (to see if it already exists)
            var strProjectId = record.OseProjId.Trim();
            var strProjectFileSpec = Path.Combine(Path.Combine(OneStoryProjectFolderRoot, strProjectId),
                                                  CstrOsMetaDataFilename);

            // if the file already exists in the project folder, then don't bother
            if (File.Exists(strProjectFileSpec))
                return;

            // get the path to the file that we're going to write
            var strFileSpec = Path.Combine(Path.Combine(OsMetaDataExtractionFolder, strProjectId),
                                           CstrOsMetaDataFilename);

            var osMetaDataModel = new OsMetaDataModel
            {
                OsProjects = new List<OsMetaDataModelRecord> {record},
                PathToMetaDataFile = strFileSpec
            };

            osMetaDataModel.Save();
        }

        private static void DisplayUsage()
        {
            LogMessage(
                String.Format(@"Usage:{0}  UpdateAccessDbWithOsMetaData ""{2}"" | ""{3}"" <path to OS Metadata Access database (e.g. 'C:\Users\Bob\Dropbox\OSE CPCs\OneStory.mdb')>{0}{0}where ""/u"" means update the database from the meta data files (in the various{0}project folders){0}and ""/c"" means create meta data files from the database (in the folder:{0}'<My Documents>\OneStory Editor Projects\{1}' folder)",
                              Environment.NewLine,
                              CstrSubfolderToExtractedOsMetaDataFileFormat,
                              CstrCommandLineUpdateDatabase,
                              CstrCommandLineCreateMetaDataFilesFromDatabase));
        }

        public static void ProcessException(Exception ex)
        {
            var strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            LogMessage(strErrorMsg);
        }

        protected const string OneStoryHiveRoot = @"Software\SIL\OneStory";
        protected const string CstrRootDirKey = "RootDir";
        protected const string CstrProjectRootFolder = "OneStory Editor Projects";

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                string strDefaultProjectFolderRoot = null;
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    strDefaultProjectFolderRoot = (string)keyOneStoryHiveRoot.GetValue(CstrRootDirKey);

                if (String.IsNullOrEmpty(strDefaultProjectFolderRoot))
                    strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                return Path.Combine(strDefaultProjectFolderRoot, CstrProjectRootFolder);
            }
            set
            {
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot, true) ??
                                          Registry.CurrentUser.CreateSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    keyOneStoryHiveRoot.SetValue(CstrRootDirKey, value);
            }
        }

        public static string OsMetaDataExtractionFolder
        {
            get { return Path.Combine(OneStoryProjectFolderRoot, CstrSubfolderToExtractedOsMetaDataFileFormat); }
        }

        private static string _strLogFilepath;
        private static string LogPath
        {
            get
            {
                return _strLogFilepath ??
                       (_strLogFilepath = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                                                                                 "SIL"),
                                                                    "OneStory Editor",
                                                       "UpdateAccessDbWithOsMetaData.log")));
            }
        }

        private static void LogMessage(string strOutput)
        {
            Console.WriteLine(strOutput);
            var strLine = String.Format("{0}: {1}{2}", DateTime.Now, strOutput, Environment.NewLine);

            // for some reason, if a log file was originally created in a server folder, say by my computer,
            //  the server doesn't then like to allow a process run on the server itself to update that file.
            //  (I have no idea why). So a) let's do exception handling around the attempted write to the log
            //  file so that programs don't fail to run and b) let's try some fall back strategies: 1) try to
            //  make a backup so we start again and if that fails, 2) let's try a different filename, and if
            //  that fails, then 3) try to fallback to a known writable folder.
            try
            {
                File.AppendAllText(LogPath, strLine);
            }
            catch (Exception)
            {
            }
        }
    }
}
