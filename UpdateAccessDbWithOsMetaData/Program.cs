using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ekm.oledb.data;
using OneStoryProjectEditor;

namespace UpdateAccessDbWithOsMetaData
{
    class Program
    {
        private const string CstrPathToExtractedOsMetaDataFileFormat = @"D:\temp\OS\AddToProjectFolders\{0}\OsMetaData.xml";

        static void Main(string[] args)
        {
            if (args.Length != 1)
                DisplayUsage();
            ProcessOsMetaDataDatabase(args[0]);
        }

        private static void ProcessOsMetaDataDatabase(string strPathToOsMetaDataAccessDatabase)
        {
            var db = Db.Open(strPathToOsMetaDataAccessDatabase);

            // this selects all the records for which there is an OSE project id
            var records = db.ExecuteMany("select * from Projects where ose_proj_id is not null");
            foreach (var record in records)
            {
                OsMetaDataModelRecord recordMapped = Mapper.Map<OsMetaDataModelRecord, OsMetaDataModelMapping>(record);
                System.Diagnostics.Debug.Assert(recordMapped.OseProjId != null);
                var strFileSpec = String.Format(CstrPathToExtractedOsMetaDataFileFormat, recordMapped.OseProjId);
                
                var osMetaDataModel = new OsMetaDataModel
                {
                    OsProjects = new List<OsMetaDataModelRecord> { recordMapped },
                    PathToMetaDataFile = strFileSpec
                };

                osMetaDataModel.Save();
            }
        }

        private static void DisplayUsage()
        {
            throw new ApplicationException(
                String.Format(
                    @"Usage:{0}  UpdateAccessDbWithOsMetaData <path to OS Metadata Access database (e.g. 'D:\temp\OS\OneStory.mdf')>",
                    Environment.NewLine));
        }
    }
}
