using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class StorySettings
    {
        public string StoryName = null;
        public string StoryGuid = null;
        public StoryStageLogic ProjStage = null;
        public CraftingInfoData CraftingInfo = null;

        public StorySettings(StoryProject.storyRow theStoryRow, StoryProject projFile, TeamMemberData loggedOnMember)
        {
            StoryName = theStoryRow.name;
            StoryGuid = theStoryRow.guid;
            ProjStage = new StoryStageLogic(theStoryRow.stage, loggedOnMember);
            CraftingInfo = new CraftingInfoData(theStoryRow, projFile, loggedOnMember);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "story",
                        new XAttribute("name", StoryName),
                        new XAttribute("stage", ProjStage.ProjectStageString),
                        new XAttribute("guid", StoryGuid),
                        CraftingInfo.GetXml);
            }
        }
    }

    public class CraftingInfoData
    {
        public string StoryCrafterMemberID = null;
        public string StoryPurpose = null;
        public string BackTranslatorMemberID = null;
        public Dictionary<byte, string> Testors = new Dictionary<byte, string>();

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
