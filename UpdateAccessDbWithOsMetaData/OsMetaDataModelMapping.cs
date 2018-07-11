using ekm.oledb.data;

namespace UpdateAccessDbWithOsMetaData
{
    public class OsMetaDataModelMapping : ObjectMapping
    {
        public OsMetaDataModelMapping()
        {
            // value in the Access database, field name in OSE
            Map("ID", "Id");
            Map("Project_Name", "ProjectName");
            Map("Language_Name", "LanguageName");
            Map("Ethnologue_Code", "EthnologueCode");
            Map("Continent", "Continent");
            Map("Country", "Country");
            Map("Methodology", "Methodology");
            Map("Managing_Partner", "ManagingPartner");
            Map("Entity", "Entity");
            Map("Contact_Person", "ContactPerson");
            Map("Contact_Person_Email", "ContactPersonEmail");
            Map("Priorities_Category", "PrioritiesCategory");
            Map("Scripture_Status", "ScriptureStatus");
            Map("Scrip_Status_Notes", "ScriptureStatusNotes");
            Map("Project_Facilitators", "ProjectFacilitators");
            Map("PF_Category", "PfCategory");
            Map("PF_Affiliation", "PfAffiliation");
            Map("Notes", "Notes");
            Map("Status", "Status");
            Map("Start_Date", "StartDate");
            Map("LCA_Workshop", "LcaWorkshop");
            Map("LCA_Coach", "LcaCoach");
            Map("SC_workshop", "ScWorkshop");
            Map("currently_using_OSE", "IsCurrentlyUsingOse");
            Map("ose_proj_id", "OseProjId");
            Map("ES_Consultant", "EsConsultant");
            Map("ES_Coach", "EsCoach");
            Map("ES_stories_sent", "EsStoriesSent");
            Map("Process_Check", "ProcessCheck");
            Map("Multiplication_workshop", "MultiplicationWorkshop");
            Map("Number_SFGs", "NumberSfgs");
            Map("PS_Consultant", "PsConsultant");
            Map("PS_Coach", "PsCoach");
            Map("PS_stories_prelim_approv", "PsStoriesPrelimApprov");
            Map("LSR", "Lsr");
            Map("Number_Final_Stories", "NumInFinalApprov");
            Map("Set_Finished_Date", "SetFinishedDate");
            Map("Uploaded_to_OSMedia", "IsUploadedToOsMedia");
            Map("Set_Copyrighted", "SetCopyrighted");
            Map("DateLastChangeProjectFile", "DateLastChangeProjectFile");
        }
    }
}
