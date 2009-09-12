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
        public string Name = null;
        public string guid = null;
        public StoryStageLogic ProjStage = null;
        public CraftingInfoData CraftingInfo = null;
        public VersesData Verses = null;

        public StoryData(string strStoryName, string strLoggedOnMemberGuid, bool bIsBiblicalStory)
        {
            Name = strStoryName;
            guid = Guid.NewGuid().ToString();
            ProjStage = new StoryStageLogic();
            CraftingInfo = new CraftingInfoData(strLoggedOnMemberGuid, bIsBiblicalStory);
            Verses = new VersesData();
        }

        public StoryData(StoryProject.storyRow theStoryRow, StoryProject projFile)
        {
            Name = theStoryRow.name;
            guid = theStoryRow.guid;
            ProjStage = new StoryStageLogic(theStoryRow.stage);
            CraftingInfo = new CraftingInfoData(theStoryRow, projFile);
            Verses = new VersesData(theStoryRow, projFile);
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
                        CraftingInfo.GetXml);

                if (Verses.HasData)
                        elemStory.Add(Verses.GetXml);

                return elemStory;
            }
        }
    }

    public class StoriesData : List<StoryData>
    {
        public const string CstrCaption = "OneStory Project Editor";
        public TeamMembersData TeamMembers = null;
        public ProjectSettings ProjSettings = null;

        public StoriesData(ProjectSettings projSettings)
        {
            // if this is "new", then we won't have a project name yet, so query the user for it
            if (projSettings == null)
            {
#if !DataDllBuild
                string strProjectName = QueryProjectName();
                ProjSettings = new ProjectSettings(null, strProjectName);
#endif
            }
            else
                ProjSettings = projSettings;

            TeamMembers = new TeamMembersData();

        }

        public StoriesData(StoryProject projFile, ProjectSettings projSettings)
        {
            // this version comes with a project settings object
            ProjSettings = projSettings;

            // if the project file we opened doesn't have anything yet.. (shouldn't really happen)
            if (projFile.stories.Count == 0)
                projFile.stories.AddstoriesRow(ProjSettings.ProjectName);
            else
                projFile.stories[0].ProjectName = ProjSettings.ProjectName; // in case the user changed it.

            TeamMembers = new TeamMembersData(projFile);
            ProjSettings.SerializeProjectSettings(projFile);

            // finally, if it's not new, then it might (should) have stories as well
            foreach (StoryProject.storyRow aStoryRow in projFile.stories[0].GetstoryRows())
                Add(new StoryData(aStoryRow, projFile));
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
        internal static string QueryProjectName()
        {
            bool bDoItAgain;
            string strProjectName = null;
            do
            {
                bDoItAgain = false;
                strProjectName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name you want to give this project (e.g. the language name).", CstrCaption, strProjectName, 300, 200);
                if (String.IsNullOrEmpty(strProjectName))
                    throw new ApplicationException("Unable to create a project without a project name!");

                // See if there's already a project with this name (which may be elsewhere)
                for (int i = 0; i < Properties.Settings.Default.RecentProjects.Count; i++)
                {
                    string strProject = Properties.Settings.Default.RecentProjects[i];
                    if (strProject == strProjectName)
                    {
                        string strProjectFolder = Properties.Settings.Default.RecentProjectPaths[i];
                        DialogResult res = MessageBox.Show(String.Format("You already have a project with the name '{1}' that is in another location. If you create this new project with the same name, then you won't be able to access the earlier project that is located in the '{2}' folder.{0}{0}Do you want to continue creating the new project and lose the reference to the earlier project (it won't be deleted if you do)?", Environment.NewLine, strProjectName, strProjectFolder), CstrCaption, MessageBoxButtons.YesNoCancel);
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

        internal TeamMemberData GetLogin()
        {
            // look at the last person to log in and see if we ought to automatically log them in again
            //  (basically Crafters or others that are also the same role as last time)
            string strMemberName = null;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
            {
                strMemberName = Properties.Settings.Default.LastMemberLogin;
                string strMemberTypeString = Properties.Settings.Default.LastUserType;
                if (TeamMembers.CanLoginMember(strMemberName, strMemberTypeString))
                    return TeamMembers[strMemberName];
            }

            // otherwise, fall thru and make them pick it.
            return EditTeamMembers(strMemberName, TeamMemberForm.CstrDefaultOKLabel);
        }

        // returns the logged in member
        internal TeamMemberData EditTeamMembers(string strMemberName, string strOKLabel)
        {
            TeamMemberForm dlg = new TeamMemberForm(TeamMembers, ProjSettings, strOKLabel);
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
                throw new ApplicationException("You have to log in in order to continue");

            // kind of a kludge, but necessary for the state logic
            //  If we have an English Back-translator person in the team, then we have to set the 
            //  member with the edit token when we get to the EnglishBT state as that person
            //  otherwise, it's a crafter
            StoryStageLogic.stateTransitions[StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT].MemberTypeWithEditToken =
                (TeamMembers.IsThereASeparateEnglishBackTranslator) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eCrafter;

            return TeamMembers[dlg.SelectedMember];
        }
#endif
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
                XElement elemStories =
                    new XElement("stories", new XAttribute("ProjectName", ProjSettings.ProjectName),
                        TeamMembers.GetXml,
                        ProjSettings.GetXml);

                foreach (StoryData aSD in this)
                    elemStories.Add(aSD.GetXml);

                return elemStories;
            }
        }
    }

    public class CraftingInfoData
    {
        public string StoryCrafterMemberID = null;
        public string StoryPurpose = null;
        public string ResourcesUsed = null;
        public string BackTranslatorMemberID = null;
        public Dictionary<byte, string> Testors = new Dictionary<byte, string>();
        public bool IsBiblicalStory = true;

        public CraftingInfoData(string strLoggedOnMemberGuid, bool bIsBiblicalStory)
        {
            StoryCrafterMemberID = strLoggedOnMemberGuid;
            IsBiblicalStory = bIsBiblicalStory;
        }

        public CraftingInfoData(StoryProject.storyRow theStoryRow, StoryProject projFile)
        {
            StoryProject.CraftingInfoRow[] aCIRs = theStoryRow.GetCraftingInfoRows();
            if (aCIRs.Length == 1)
            {
                StoryProject.CraftingInfoRow theCIR = aCIRs[0];
                if (!theCIR.IsNonBiblicalStoryNull())
                    IsBiblicalStory = !theCIR.NonBiblicalStory;

                StoryProject.StoryCrafterRow[] aSCRs = theCIR.GetStoryCrafterRows();
                if (aSCRs.Length == 1)
                    StoryCrafterMemberID = aSCRs[0].memberID;
                else
                    throw new ApplicationException("The project file is corrupted. No 'StoryCrafterMemberID' record found. Send to bob_eaton@sall.com for help.");

                if (!theCIR.IsStoryPurposeNull())
                    StoryPurpose = theCIR.StoryPurpose;

                if (!theCIR.IsResourcesUsedNull())
                    ResourcesUsed = theCIR.ResourcesUsed;

                StoryProject.BackTranslatorRow[] aBTRs = theCIR.GetBackTranslatorRows();
                if (aBTRs.Length == 1)
                    BackTranslatorMemberID = aBTRs[0].memberID;

                StoryProject.TestsRow[] aTsRs = theCIR.GetTestsRows();
                if (aTsRs.Length == 1)
                {
                    foreach (StoryProject.TestRow aTR in aTsRs[0].GetTestRows())
                        Testors.Add(aTR.number, aTR.memberID);
                }
            }
            else
                throw new ApplicationException("The project file is corrupted. No 'CraftingInfo' record found. Send to bob_eaton@sall.com for help.");
        }

        public XElement GetXml
        {
            get
            {
                XElement elemCraftingInfo = new XElement("CraftingInfo",
                    new XAttribute("NonBiblicalStory", !IsBiblicalStory),
                    new XElement("StoryCrafter", new XAttribute("memberID", StoryCrafterMemberID)));

                if (!String.IsNullOrEmpty(StoryPurpose))
                    elemCraftingInfo.Add(new XElement("StoryPurpose", StoryPurpose));

                if (!String.IsNullOrEmpty(ResourcesUsed))
                    elemCraftingInfo.Add(new XElement("ResourcesUsed", ResourcesUsed));

                if (!String.IsNullOrEmpty(BackTranslatorMemberID))
                    elemCraftingInfo.Add(new XElement("BackTranslator", new XAttribute("memberID", BackTranslatorMemberID)));

                if (Testors.Count > 0)
                {
                    XElement elemTestors = new XElement("Tests");
                    foreach (KeyValuePair<byte, string> kvp in Testors)
                        elemTestors.Add(new XElement("Test", new XAttribute("number", kvp.Key), new XAttribute("memberID", kvp.Value)));
                    elemCraftingInfo.Add(elemTestors);
                }

                return elemCraftingInfo;
            }
        }
    }
}
