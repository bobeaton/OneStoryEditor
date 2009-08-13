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

        public StoryData(string strStoryName, string loggedOnMemberGuid)
        {
            StoryName = strStoryName;
            StoryGuid = Guid.NewGuid().ToString();
            ProjStage = new StoryStageLogic();
            CraftingInfo = new CraftingInfoData(loggedOnMemberGuid);
            Verses = new VersesData();
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(StoryName)
                    && !String.IsNullOrEmpty(ProjStage.ProjectStage.ToString())
                    && !String.IsNullOrEmpty(StoryGuid));

                XElement elemStory = new XElement("story",
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
        internal ProjectSettings ProjSettings = null;

        public StoriesData()
        {
            ProjSettings = new ProjectSettings("Kangri");
        }

        public XElement GetXml
        {
            get
            {
                XElement elemStories =
                    new XElement("stories", new XAttribute("ProjectName", ProjSettings.ProjectName));

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

        public CraftingInfoData(string loggedOnMemberGuid)
        {
            StoryCrafterMemberID = loggedOnMemberGuid;
        }

        public XElement GetXml
        {
            get
            {
                XElement elemCraftingInfo = new XElement("CraftingInfo",
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
