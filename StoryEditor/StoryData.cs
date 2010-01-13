using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class StoryData
    {
        public string Name;
        public string guid;
        public DateTime StageTimeStamp;
        public StoryStageLogic ProjStage;
        public CraftingInfoData CraftingInfo;
        public VersesData Verses;

        public StoryData(string strStoryName, string strCrafterMemberGuid, string strLoggedOnMemberGuid, bool bIsBiblicalStory, ProjectSettings projSettings)
        {
            Name = strStoryName;
            guid = Guid.NewGuid().ToString();
            StageTimeStamp = DateTime.Now;
            ProjStage = new StoryStageLogic(projSettings);
            CraftingInfo = new CraftingInfoData(strCrafterMemberGuid, strLoggedOnMemberGuid, bIsBiblicalStory);
            Verses = new VersesData();
            Verses.InsureFirstVerse();
        }

        public StoryData(NewDataSet.storyRow theStoryRow, NewDataSet projFile)
        {
            Name = theStoryRow.name;
            guid = theStoryRow.guid;
            StageTimeStamp = (theStoryRow.IsstageDateTimeStampNull()) ? DateTime.Now : theStoryRow.stageDateTimeStamp;
            ProjStage = new StoryStageLogic(theStoryRow.stage);
            CraftingInfo = new CraftingInfoData(theStoryRow);
            Verses = new VersesData(theStoryRow, projFile);
        }

        public StoryData(StoryData rhs)
        {
            Name = rhs.Name;

            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();  // rhs.guid;

            StageTimeStamp = rhs.StageTimeStamp;
            ProjStage = new StoryStageLogic(rhs.ProjStage);
            CraftingInfo = new CraftingInfoData(rhs.CraftingInfo);
            Verses = new VersesData(rhs.Verses);
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(Name)
                    && !String.IsNullOrEmpty(ProjStage.ProjectStage.ToString())
                    && !String.IsNullOrEmpty(guid));

                XElement elemStory = new XElement("story",
                        new XAttribute("name", Name),
                        new XAttribute("stage", ProjStage.ToString()),
                        new XAttribute("guid", guid),
                        new XAttribute("stageDateTimeStamp", StageTimeStamp),
                        CraftingInfo.GetXml);

                if (Verses.HasData)
                        elemStory.Add(Verses.GetXml);

                return elemStory;
            }
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties, 
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            Verses.IndexSearch(findProperties, ref lstBoxesToSearch);
        }

        public void CheckForProjectFacilitator(StoryProjectData storyProjectData, TeamMemberData loggedOnMember)
        {
            if (CraftingInfo.ProjectFacilitatorMemberID == null)
            {
                // this means that we've opened a file which didn't have the proj fac listed
                // if there's only one PF, then just put that one in. If there are multiple,
                //  then ask which one to use
                if ((storyProjectData.TeamMembers.CountOfProjectFacilitator == 1)
                    && (loggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator))
                {
                    CraftingInfo.ProjectFacilitatorMemberID = loggedOnMember.MemberGuid;
                    return;
                }

                // fall thru means either the logged in person isn't a PF or there are multiple,
                //  so, ask the user to tell which PF added this story
                var dlg = new MemberPicker(storyProjectData, TeamMemberData.UserTypes.eProjectFacilitator)
                                       {
                                           Text = "Choose the Project Facilitator that entered this story"
                                       };
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                CraftingInfo.ProjectFacilitatorMemberID = dlg.SelectedMember.MemberGuid;
            }
        }
    }

    public class CraftingInfoData
    {
        public string StoryCrafterMemberID;
        public string ProjectFacilitatorMemberID;
        public string StoryPurpose;
        public string ResourcesUsed;
        public string BackTranslatorMemberID;
        public List<string> Testors = new List<string>();
        public bool IsBiblicalStory = true;

        public CraftingInfoData(string strCrafterMemberGuid, string strLoggedOnMemberGuid, bool bIsBiblicalStory)
        {
            StoryCrafterMemberID = strCrafterMemberGuid;
            ProjectFacilitatorMemberID = strLoggedOnMemberGuid;
            IsBiblicalStory = bIsBiblicalStory;
        }

        public CraftingInfoData(NewDataSet.storyRow theStoryRow)
        {
            NewDataSet.CraftingInfoRow[] aCIRs = theStoryRow.GetCraftingInfoRows();
            if (aCIRs.Length == 1)
            {
                NewDataSet.CraftingInfoRow theCIR = aCIRs[0];
                if (!theCIR.IsNonBiblicalStoryNull())
                    IsBiblicalStory = !theCIR.NonBiblicalStory;

                NewDataSet.StoryCrafterRow[] aSCRs = theCIR.GetStoryCrafterRows();
                if (aSCRs.Length == 1)
                    StoryCrafterMemberID = aSCRs[0].memberID;
                else
                    throw new ApplicationException(Properties.Resources.IDS_ProjectFileCorrupted);

                NewDataSet.ProjectFacilitatorRow[] aPFRs = theCIR.GetProjectFacilitatorRows();
                if (aPFRs.Length == 1)
                    ProjectFacilitatorMemberID = aPFRs[0].memberID;

                if (!theCIR.IsStoryPurposeNull())
                    StoryPurpose = theCIR.StoryPurpose;

                if (!theCIR.IsResourcesUsedNull())
                    ResourcesUsed = theCIR.ResourcesUsed;

                NewDataSet.BackTranslatorRow[] aBTRs = theCIR.GetBackTranslatorRows();
                if (aBTRs.Length == 1)
                    BackTranslatorMemberID = aBTRs[0].memberID;

                NewDataSet.TestsRow[] aTsRs = theCIR.GetTestsRows();
                if (aTsRs.Length == 1)
                {
                    foreach (NewDataSet.TestRow aTR in aTsRs[0].GetTestRows())
                        Testors.Add(aTR.memberID);
                }
            }
            else
                throw new ApplicationException(Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo);
        }

        public CraftingInfoData(CraftingInfoData rhs)
        {
            StoryCrafterMemberID = rhs.StoryCrafterMemberID;
            ProjectFacilitatorMemberID = rhs.ProjectFacilitatorMemberID;
            StoryPurpose = rhs.StoryPurpose;
            ResourcesUsed = rhs.ResourcesUsed;
            BackTranslatorMemberID = rhs.BackTranslatorMemberID;
            IsBiblicalStory = rhs.IsBiblicalStory;
            foreach (string strUnsGuid in rhs.Testors)
                Testors.Add(strUnsGuid);
        }

        public XElement GetXml
        {
            get
            {
                var elemCraftingInfo = new XElement("CraftingInfo",
                                                         new XAttribute("NonBiblicalStory", !IsBiblicalStory),
                                                         new XElement("StoryCrafter",
                                                                      new XAttribute("memberID", StoryCrafterMemberID)));

                if (!String.IsNullOrEmpty(ProjectFacilitatorMemberID))
                    elemCraftingInfo.Add(new XElement("ProjectFacilitator", new XAttribute("memberID", ProjectFacilitatorMemberID)));

                if (!String.IsNullOrEmpty(StoryPurpose))
                    elemCraftingInfo.Add(new XElement("StoryPurpose", StoryPurpose));

                if (!String.IsNullOrEmpty(ResourcesUsed))
                    elemCraftingInfo.Add(new XElement("ResourcesUsed", ResourcesUsed));

                if (!String.IsNullOrEmpty(BackTranslatorMemberID))
                    elemCraftingInfo.Add(new XElement("BackTranslator", new XAttribute("memberID", BackTranslatorMemberID)));

                if (Testors.Count > 0)
                {
                    XElement elemTestors = new XElement("Tests");
                    foreach (string strUnsGuid in Testors)
                        elemTestors.Add(new XElement("Test", new XAttribute("memberID", strUnsGuid)));
                    elemCraftingInfo.Add(elemTestors);
                }

                return elemCraftingInfo;
            }
        }
    }

    public class StoriesData : List<StoryData>
    {
        public string SetName;

        public StoriesData(string strSetName)
        {
            SetName = strSetName;
        }

        public StoriesData(NewDataSet.storiesRow theStoriesRow, NewDataSet projFile)
        {
            SetName = theStoriesRow.SetName;

            // finally, if it's not new, then it might (should) have stories as well
            foreach (NewDataSet.storyRow aStoryRow in theStoriesRow.GetstoryRows())
                Add(new StoryData(aStoryRow, projFile));
        }

        public new bool Contains(StoryData theSD)
        {
            foreach (StoryData aSD in this)
                if (aSD.Name == theSD.Name)
                    return true;
            return false;
        }

        public XElement GetXml
        {
            get
            {
                XElement elemStories = new XElement("stories", new XAttribute("SetName", SetName));

                foreach (StoryData aSD in this)
                    elemStories.Add(aSD.GetXml);

                return elemStories;
            }
        }

        public void IndexSearch(SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StorySearchIndex alstBoxesToSearch)
        {
            foreach (StoryData aSD in this)
            {
                SearchForm.StringTransferSearchIndex ssi = alstBoxesToSearch.GetNewStorySearchIndex(aSD.Name);
                aSD.IndexSearch(findProperties, ref ssi);
            }
        }

        public StoryData GetStoryFromName(string strStoryName)
        {
            foreach (StoryData aSD in this)
                if (strStoryName == aSD.Name)
                    return aSD;
            return null;
        }
    }

    public class StoryProjectData : Dictionary<string, StoriesData>
    {
        public TeamMembersData TeamMembers;
        public ProjectSettings ProjSettings;
        public string PanoramaFrontMatter;
        public string XmlDataVersion = "1.1";

        /// <summary>
        /// This version of the constructor should *always* be followed by a call to InitializeProjectSettings()
        /// </summary>
        public StoryProjectData()
        {
            TeamMembers = new TeamMembersData();
            PanoramaFrontMatter = Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            // start with to stories sets (the current one and the obsolete ones)
            Add(Properties.Resources.IDS_MainStoriesSet, new StoriesData(Properties.Resources.IDS_MainStoriesSet));
            Add(Properties.Resources.IDS_ObsoleteStoriesSet, new StoriesData(Properties.Resources.IDS_ObsoleteStoriesSet));
        }

        public StoryProjectData(NewDataSet projFile, ProjectSettings projSettings)
        {
            // this version comes with a project settings object
            ProjSettings = projSettings;

            // if the project file we opened doesn't have anything yet.. (shouldn't really happen)
            if (projFile.StoryProject.Count == 0)
                projFile.StoryProject.AddStoryProjectRow(XmlDataVersion, ProjSettings.ProjectName, Properties.Resources.IDS_DefaultPanoramaFrontMatter);
            else
            {
                projFile.StoryProject[0].ProjectName = ProjSettings.ProjectName; // in case the user changed it.
                if (projFile.StoryProject[0].version.CompareTo(XmlDataVersion) > 0)
                    throw new ApplicationException(Properties.Resources.IDS_GetNewVersion);
            }

            PanoramaFrontMatter = projFile.StoryProject[0].PanoramaFrontMatter;
            if (String.IsNullOrEmpty(PanoramaFrontMatter))
                PanoramaFrontMatter = Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            if (projFile.stories.Count == 0)
            {
                projFile.stories.AddstoriesRow(Properties.Resources.IDS_MainStoriesSet, projFile.StoryProject[0]);
                projFile.stories.AddstoriesRow(Properties.Resources.IDS_ObsoleteStoriesSet, projFile.StoryProject[0]);
            }

            TeamMembers = new TeamMembersData(projFile);
            ProjSettings.SerializeProjectSettings(projFile);

            // finally, if it's not new, then it might (should) have stories as well
            foreach (NewDataSet.storiesRow aStoriesRow in projFile.StoryProject[0].GetstoriesRows())
                Add(aStoriesRow.SetName, new StoriesData(aStoriesRow, projFile));
        }

        // if this is "new", then we won't have a project name yet, so query the user for it
        public bool InitializeProjectSettings(TeamMemberData loggedOnMember)
        {
            NewProjectWizard dlg = new NewProjectWizard(this) 
            { 
                LoggedInMember = loggedOnMember,
                Text = "Edit Project Settings"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                throw StoryEditor.BackOutWithNoUI;

            // otherwise, it has our new project settings
            ProjSettings = dlg.ProjSettings;
            return dlg.Modified;
        }

        internal string GetMemberNameFromMemberGuid(string strMemberGuid)
        {
            string strMemberName = null;
            foreach (TeamMemberData aTMD in TeamMembers.Values)
            {
                if (aTMD.MemberGuid == strMemberGuid)
                {
                    strMemberName = aTMD.Name;
                    break;
                }
            }
            return strMemberName;
        }

#if !DataDllBuild
        /*
        internal static string QueryProjectName()
        {
            bool bDoItAgain;
            string strProjectName = null;
            do
            {
                bDoItAgain = false;
                strProjectName = Microsoft.VisualBasic.Interaction.InputBox(Properties.Resources.IDS_EnterProjectName, Properties.Resources.IDS_Caption, strProjectName, 300, 200);
                if (String.IsNullOrEmpty(strProjectName))
                    throw new ApplicationException(Properties.Resources.IDS_UnableToCreateProjectWithoutName);

                // See if there's already a project with this name (which may be elsewhere)
                for (int i = 0; i < Properties.Settings.Default.RecentProjects.Count; i++)
                {
                    string strProject = Properties.Settings.Default.RecentProjects[i];
                    if (strProject == strProjectName)
                    {
                        string strProjectFolder = Properties.Settings.Default.RecentProjectPaths[i];
                        DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_AboutToStrandProject, Environment.NewLine, strProjectName, strProjectFolder), Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Cancel)
                            throw StoryEditor.BackOutWithNoUI;
                        if (res == DialogResult.No)
                            bDoItAgain = true;
                        break;
                    }
                }
            } while (bDoItAgain);
            return strProjectName;
        }
        */

        internal TeamMemberData GetLogin(ref bool bModified)
        {
            // look at the last person to log in and see if we ought to automatically log them in again
            //  (basically Crafters or others that are also the same role as last time)
            string strMemberName = null;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
            {
                strMemberName = Properties.Settings.Default.LastMemberLogin;
                string strMemberTypeString = Properties.Settings.Default.LastUserType;
                if (CanLoginMember(strMemberName, strMemberTypeString))
                    return TeamMembers[strMemberName];
            }

            // otherwise, fall thru and make them pick it.
            bModified = true;
            return EditTeamMembers(strMemberName, TeamMemberForm.CstrDefaultOKLabel);
        }

        // this can be used to determine whether a given member name and type are one
        //  of the ones in this project (for auto-login)
        public bool CanLoginMember(string strMemberName, string strMemberType)
        {
            if (TeamMembers.ContainsKey(strMemberName))
            {
                TeamMemberData aTMD = TeamMembers[strMemberName];
                if (aTMD.MemberTypeAsString == strMemberType)
                {
                    // kind of a kludge, but necessary for the state logic
                    //  If we're going to return true (meaning that we can auto-log this person in), then
                    //  if we have an English Back-translator person in the team, then we have to set the 
                    //  member with the edit token when we get to the EnglishBT state as that person
                    //  otherwise, it's a crafter
                    StoryStageLogic.stateTransitions[StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT].MemberTypeWithEditToken =
                        (TeamMembers.HasOutsideEnglishBTer) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eProjectFacilitator;
                    return true;
                }
            }
            return false;
        }

        // returns the logged in member
        internal TeamMemberData EditTeamMembers(string strMemberName, string strOKLabel)
        {
            TeamMemberForm dlg = new TeamMemberForm(TeamMembers, strOKLabel);
            if (!String.IsNullOrEmpty(strMemberName))
            {
                try
                {
                    // if we did find the "last member" in the list, but couldn't accept it without question
                    //  (e.g. because the role was different), then at least pre-select the member
                    dlg.SelectedMember = strMemberName;
                }
                catch { }    // might fail if the "last user" on this machine is opening this project file for the first time... just ignore
            }

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(Properties.Resources.IDS_HaveToLogInToContinue, Properties.Resources.IDS_Caption);
                throw StoryEditor.BackOutWithNoUI;
            }

            // kind of a kludge, but necessary for the state logic
            //  If we have an English Back-translator person in the team, then we have to set the 
            //  member with the edit token when we get to the EnglishBT state as that person
            //  otherwise, it's a crafter
            StoryStageLogic.stateTransitions[StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT].MemberTypeWithEditToken =
                (TeamMembers.HasOutsideEnglishBTer) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eProjectFacilitator;

            return TeamMembers[dlg.SelectedMember];
        }
#endif

        // use of this version factors in both the settings in the project file
        public bool GetHgRepoUsernamePassword(string strProjectName, TeamMemberData loggedOnMember, 
            out string strUsername, out string strPassword, out string strHgUrlBase)
        {
            strPassword = strHgUrlBase = null;    // just in case we don't have anything for this.

            string strRepoUrl, strDummy;
            if (Program.GetHgRepoParameters(strProjectName, out strUsername, out strRepoUrl, 
                out strDummy))
            {
                if (!String.IsNullOrEmpty(strRepoUrl))
                {
                    var uri = new Uri(strRepoUrl);
                    if (!String.IsNullOrEmpty(uri.UserInfo) && (uri.UserInfo.IndexOf(':') != -1))
                    {
                        string[] astrUserInfo = uri.UserInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        System.Diagnostics.Debug.Assert((astrUserInfo.Length == 2) && (astrUserInfo[0] == strUsername));
                        strUsername = astrUserInfo[0];
                        strPassword = astrUserInfo[1];
                    }
                    strHgUrlBase = uri.Scheme + "://" + uri.Host;
                }
            }

            // okay, the above is what we saved in the user file, but it's possible that that
            //  will be empty (e.g. if change the assembly number), so let's get it out of the
            //  project file as well (which will supercede the above information)
            if ((loggedOnMember != null)
                && (!String.IsNullOrEmpty(loggedOnMember.HgUsername))
                && (!String.IsNullOrEmpty(loggedOnMember.HgPassword)))
            {
                strUsername = loggedOnMember.HgUsername;
                strPassword = loggedOnMember.HgPassword;
            }

            return !String.IsNullOrEmpty(strHgUrlBase);
        }

        public bool IsASeparateEnglishBackTranslator
        {
            get
            {
                System.Diagnostics.Debug.Assert(TeamMembers != null);

                // the role "English Back-translator" only has meaning if there's another
                //  language involved.
                foreach (TeamMemberData aTM in TeamMembers.Values)
                    if (aTM.MemberType == TeamMemberData.UserTypes.eEnglishBacktranslator)
                        return true;
                return false;
            }
        }

        public static string GetRunningFolder
        {
            get
            {
                string strCurrentFolder = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                return Path.GetDirectoryName(strCurrentFolder);
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement elemStoryProject =
                    new XElement("StoryProject",
                        new XAttribute("version", XmlDataVersion),
                        new XAttribute("ProjectName", ProjSettings.ProjectName),
                        new XAttribute("PanoramaFrontMatter", PanoramaFrontMatter),
                        TeamMembers.GetXml,
                        ProjSettings.GetXml);

                foreach (StoriesData aSsD in Values)
                    elemStoryProject.Add(aSsD.GetXml);

                return elemStoryProject;
            }
        }
    }
}
