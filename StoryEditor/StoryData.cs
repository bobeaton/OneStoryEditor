using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class StoryData
    {
        public string StoryName = null;
        public string StoryGuid = null;
        public StoryStageLogic ProjStage = null;
        public CraftingInfoData CraftingInfo = null;
        public VersesData Verses = null;

        internal StoryData(string strStoryName, StoryEditor theSE)
        {
            StoryName = strStoryName;
            StoryGuid = Guid.NewGuid().ToString();
            ProjStage = new StoryStageLogic(theSE);
            CraftingInfo = new CraftingInfoData(theSE.LoggedOnMember);
            Verses = new VersesData();
        }

        public StoryData(StoryProject.storyRow theStoryRow, StoryProject projFile, StoryEditor theSE)
        {
            StoryName = theStoryRow.name;
            StoryGuid = theStoryRow.guid;
            ProjStage = new StoryStageLogic(theStoryRow.stage, theSE);
            CraftingInfo = new CraftingInfoData(theStoryRow, projFile, theSE.LoggedOnMember);
            Verses = new VersesData(theStoryRow, projFile);
        }

        public bool IsReadyForTransition(StoryEditor theSE)
        {
            System.Diagnostics.Debug.Assert((theSE.Stories != null) && (theSE.Stories.ProjSettings != null));
            StoryStageLogic.StageTransition st = StoryStageLogic.stateTransitions[ProjStage.ProjectStage];
            bool bRet = st.CheckForValidEndOfState(theSE, theSE.Stories.ProjSettings, this);
            if (bRet)
                ProjStage.ProjectStage = st.NextStage;  // if we are ready, then go ahead and transition
            return bRet;
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(StoryName)
                    && !String.IsNullOrEmpty(ProjStage.ProjectStage.ToString())
                    && !String.IsNullOrEmpty(StoryGuid));

                XElement elemStory = new XElement(StoryEditor.ns + "story",
                        new XAttribute("name", StoryName),
                        new XAttribute("stage", ProjStage.ToString()),
                        new XAttribute("guid", StoryGuid),
                        CraftingInfo.GetXml);

                if (Verses.HasData)
                        elemStory.Add(Verses.GetXml);

                return elemStory;
            }
        }
    }

    public class StoriesData : List<StoryData>
    {
        internal TeamMembersData TeamMembers = null;
        internal ProjectSettings ProjSettings = null;

        public StoriesData(StoryEditor theSE)
        {
            // if this is "new", then we won't have a project name yet, so query the user for it
            string strProjectName = QueryProjectName();
            TeamMembers = new TeamMembersData();
            ProjSettings = new ProjectSettings(strProjectName);

            // the LoggedOnMemb might have been passed in from a previous file
            if (theSE.LoggedOnMember == null)
                theSE.LoggedOnMember = GetLogin();
        }

        public StoriesData(StoryProject projFile, StoryEditor theSE)
        {
            // if this is "new", then we won't have a project name yet, so query the user for it
            string strProjectName;
            if (projFile.stories.Count == 0)
            {
                strProjectName = QueryProjectName();
                projFile.stories.AddstoriesRow(strProjectName); // most of the following is expecting it to have a stories row
            }
            else
                strProjectName = projFile.stories[0].ProjectName;

            TeamMembers = new TeamMembersData(projFile);
            ProjSettings = new ProjectSettings(projFile, strProjectName);

            // the LoggedOnMemb might have been passed in from a previous file
            if (theSE.LoggedOnMember == null)
                theSE.LoggedOnMember = GetLogin();

            // finally, if it's not new, then it might (should) have stories as well
            foreach (StoryProject.storyRow aStoryRow in projFile.stories[0].GetstoryRows())
                Add(new StoryData(aStoryRow, projFile, theSE));
        }

        public StoryData AddStory(string strStoryName, StoryEditor theSE)
        {
            StoryData theNewStory = new StoryData(strStoryName, theSE);
            Add(theNewStory);
            return theNewStory;
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

        protected string QueryProjectName()
        {
            string strProjectName = Microsoft.VisualBasic.Interaction.InputBox(String.Format("You are creating a brand new OneStory project. Enter the name you want to give this project (e.g. the language name).{0}{0}(if you had intended to edit an existing project, cancel this dialog and use the 'File', 'Open' command)", Environment.NewLine), StoryEditor.CstrCaption, null, 300, 200);
            if (String.IsNullOrEmpty(strProjectName))
                throw new ApplicationException("Unable to create a project without a project name!");
            return strProjectName;
        }

        protected TeamMemberData GetLogin()
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
            return EditTeamMembers(strMemberName);
        }

        // returns the logged in member
        internal TeamMemberData EditTeamMembers(string strMemberName)
        {
            TeamMemberForm dlg = new TeamMemberForm(TeamMembers, ProjSettings);
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

            return TeamMembers[dlg.SelectedMember];
        }

        public XElement GetXml
        {
            get
            {
                XElement elemStories =
                    new XElement(StoryEditor.ns + "stories", new XAttribute("ProjectName", ProjSettings.ProjectName),
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

        public CraftingInfoData(TeamMemberData loggedOnMember)
        {
            StoryCrafterMemberID = loggedOnMember.MemberGuid;
        }

        public CraftingInfoData(StoryProject.storyRow theStoryRow, StoryProject projFile, TeamMemberData loggedOnMember)
        {
            StoryProject.CraftingInfoRow[] aCIRs = theStoryRow.GetCraftingInfoRows();
            if (aCIRs.Length == 1)
            {
                StoryProject.CraftingInfoRow theCIR = aCIRs[0];

                StoryProject.StoryCrafterRow[] aSCRs = theCIR.GetStoryCrafterRows();
                if (aSCRs.Length == 1)
                    StoryCrafterMemberID = aSCRs[0].memberID;
                else
                    StoryCrafterMemberID = loggedOnMember.MemberGuid;

                StoryPurpose = theCIR.StoryPurpose;
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
            {
                StoryCrafterMemberID = loggedOnMember.MemberGuid;
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement elemCraftingInfo = new XElement(StoryEditor.ns + "CraftingInfo",
                    new XElement(StoryEditor.ns + "StoryCrafter", new XAttribute("memberID", StoryCrafterMemberID)));

                if (!String.IsNullOrEmpty(StoryPurpose))
                    elemCraftingInfo.Add(new XElement(StoryEditor.ns + "StoryPurpose", StoryPurpose));

                if (!String.IsNullOrEmpty(ResourcesUsed))
                    elemCraftingInfo.Add(new XElement(StoryEditor.ns + "ResourcesUsed", ResourcesUsed));

                if (!String.IsNullOrEmpty(BackTranslatorMemberID))
                    elemCraftingInfo.Add(new XElement(StoryEditor.ns + "BackTranslator", new XAttribute("memberID", BackTranslatorMemberID)));

                if (Testors.Count > 0)
                {
                    XElement elemTestors = new XElement(StoryEditor.ns + "Tests");
                    foreach (KeyValuePair<byte, string> kvp in Testors)
                        elemTestors.Add(new XElement(StoryEditor.ns + "Test", new XAttribute("number", kvp.Key), new XAttribute("memberID", kvp.Value)));
                    elemCraftingInfo.Add(elemTestors);
                }

                return elemCraftingInfo;
            }
        }
    }
}
