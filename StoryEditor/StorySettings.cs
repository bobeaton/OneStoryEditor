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

        public StorySettings(StoryProject.storyRow theStoryRow, StoryProject projFile, LoggedOnMemberInfo logonInfo)
        {
            StoryName = theStoryRow.name;
            StoryGuid = theStoryRow.guid;
            ProjStage = new StoryStageLogic(theStoryRow.stage, logonInfo);
            CraftingInfo = new CraftingInfoData(theStoryRow, projFile, logonInfo);
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

        public CraftingInfoData(StoryProject.storyRow theStoryRow, StoryProject projFile, LoggedOnMemberInfo member)
        {
            StoryProject.CraftingInfoRow[] aCIRs = theStoryRow.GetCraftingInfoRows();
            if (aCIRs.Length == 1)
            {
                StoryProject.CraftingInfoRow theCIR = aCIRs[0];

                StoryProject.StoryCrafterRow[] aSCRs = theCIR.GetStoryCrafterRows();
                if (aSCRs.Length == 1)
                    StoryCrafterMemberID = aSCRs[0].memberID;

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
        }

        public XElement GetXml
        {
            get
            {
                XElement elemTestors = new XElement(StoryEditor.ns + "Tests");
                foreach (KeyValuePair<byte, string> kvp in Testors)
                    elemTestors.Add(new XElement(StoryEditor.ns + "Test", new XAttribute("number", kvp.Key), new XAttribute("memberID", kvp.Value)));

                return new XElement(StoryEditor.ns + "CraftingInfo",
                    new XElement(StoryEditor.ns + "StoryCrafter", new XAttribute("memberID", StoryCrafterMemberID)),
                    new XElement(StoryEditor.ns + "StoryPurpose", StoryPurpose),
                    new XElement(StoryEditor.ns + "BackTranslator", new XAttribute("memberID", BackTranslatorMemberID)),
                    elemTestors);
            }
        }
    }
}
