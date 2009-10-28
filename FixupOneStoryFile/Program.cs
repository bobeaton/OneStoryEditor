using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Win32;

namespace FixupOneStoryFile
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lstOneStoryFiles = GetOneStoryFiles(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OneStory"));
            if ((args.Length > 0) && File.Exists(args[0]))
                lstOneStoryFiles.Add(args[0]);

            if (lstOneStoryFiles.Count == 0)
                return;

            foreach (string strFilename in lstOneStoryFiles)
                FixupOneStoryFile(strFilename);
        }

        static void FixupOneStoryFile(string strFilename)
        {
            try
            {
                XDocument doc = XDocument.Load(strFilename);
                bool bFound = false;

                FixupAddressToBioData(doc, ref bFound);
                FixupCrafterToProjFac(doc, ref bFound);
                FixupCrafterToConNoteDirection(doc, ref bFound);
                FixupProjectName(doc, ref bFound);
                FixupEmbedStories(doc, ref bFound);
                FixupGuidAdditions(doc, ref bFound);

                if (bFound)
                    doc.Save(strFilename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void FixupGuidAdditions(XDocument doc, ref bool bFound)
        {
            foreach (XElement elem in doc.XPathSelectElements("//ConsultantNotes/ConsultantConversation[not(@guid)]"))
            {
                elem.Add(new XAttribute("guid", Guid.NewGuid().ToString()));
                bFound = true;
            }

            foreach (XElement elem in doc.XPathSelectElements("//CoachNotes/CoachConversation[not(@guid)]"))
            {
                elem.Add(new XAttribute("guid", Guid.NewGuid().ToString()));
                bFound = true;
            }

            foreach (XElement elem in doc.XPathSelectElements("//TestQuestions/TestQuestion[not(@guid)]"))
            {
                elem.Add(new XAttribute("guid", Guid.NewGuid().ToString()));
                bFound = true;
            }
        }

        static void FixupEmbedStories(XDocument doc, ref bool bFound)
        {
            XElement elemStoryProject = doc.XPathSelectElement("/StoryProject");

            // extra the "Languages" component out of the 'stories' element and put it in the parent element
            XElement elemLanguages = doc.XPathSelectElement("/StoryProject/stories/Languages");
            if (elemLanguages != null)
            {
                bFound = true;
                elemStoryProject.AddFirst(elemLanguages);
                elemLanguages.Remove();
            }

            // same with Members
            XElement elemMembers = doc.XPathSelectElement("/StoryProject/stories/Members");
            if (elemMembers != null)
            {
                bFound = true;
                elemStoryProject.AddFirst(elemMembers);
                elemMembers.Remove();
            }

            // add an "Old Stories" named 'stories' element if it doesn't exist
            XElement elemOldStories = doc.XPathSelectElement("/StoryProject/stories[@SetName='Old Stories']");
            if (elemOldStories == null)
            {
                bFound = true;
                elemStoryProject.Add(new XElement("stories", new XAttribute("SetName", "Old Stories")));
            }
        }

        static void FixupProjectName(XDocument doc, ref bool bFound)
        {
            XElement elem = doc.XPathSelectElement("/StoryProject");
            XElement elemStories = doc.XPathSelectElement("/StoryProject/stories");
            XAttribute attrProjectName = elemStories.Attribute("ProjectName");
            if (attrProjectName != null)
            {
                bFound = true;
                string strProjectName = attrProjectName.Value;
                attrProjectName.Remove();
                if (elem.Attribute("ProjectName") == null)
                    elem.SetAttributeValue("ProjectName", strProjectName);
            }

            if (elem.Attribute("PanoramaFrontMatter") == null)
            {
                bFound = true;
                elem.SetAttributeValue("PanoramaFrontMatter", "");
            }
            
            XAttribute attrSetName = elemStories.Attribute("SetName");
            if (attrSetName == null)
            {
                bFound = true;
                elemStories.SetAttributeValue("SetName", "Stories");
            }
        }

        static void FixupCrafterToConNoteDirection(XDocument doc, ref bool bFound)
        {
            // first fixup any Members with an 'address' to have a 'bioData' instead
            foreach (XElement elem in doc.XPathSelectElements("/StoryProject/stories/story/verses/verse/ConsultantNotes/ConsultantConversation/ConsultantNote[@Direction]"))
            {
                string value = elem.Attribute("Direction").Value;
                string newValue = value.Replace("Crafter", "ProjFac");
                if (value != newValue)
                {
                    bFound = true;
                    elem.Attribute("Direction").Value = newValue;
                }
            }
        }

        static void FixupCrafterToProjFac(XDocument doc, ref bool bFound)
        {
            // first fixup any Members with an 'address' to have a 'bioData' instead
            foreach (XElement elem in doc.XPathSelectElements("/StoryProject/stories/story[@stage]"))
            {
                string value = elem.Attribute("stage").Value;
                string newValue = value.Replace("Crafter", "ProjFac");
                if (value != newValue)
                {
                    bFound = true;
                    elem.Attribute("stage").Value = newValue;
                }
            }
        }

        static void FixupAddressToBioData(XDocument doc, ref bool bFound)
        {
            // first fixup any Members with an 'address' to have a 'bioData' instead
            foreach (XElement elem in doc.XPathSelectElements("/StoryProject/stories/Members/Member[@address]"))
            {
                bFound = true;
                string value = elem.Attribute("address").Value;
                elem.Attribute("address").Remove();
                elem.SetAttributeValue("bioData", value);
            }
        }

        protected const string OneStoryHiveRoot = @"Software\SIL\OneStory";
        protected const string CstrRootDirKey = "RootDir";

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                string strProjectFolderName = "OneStory";
                string strDefaultProjectFolderRoot = null;
                RegistryKey keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                {
                    strProjectFolderName = "OneStory Editor Projects";
                    strDefaultProjectFolderRoot = (string) keyOneStoryHiveRoot.GetValue(CstrRootDirKey);
                }

                if (String.IsNullOrEmpty(strDefaultProjectFolderRoot))
                    strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string strPath = Path.Combine(strDefaultProjectFolderRoot,
                    strProjectFolderName);

                return strPath;
            }
        }

        private static List<string> GetOneStoryFiles(DirectoryInfo info)
        {
            DirectoryInfo[] dis = info.GetDirectories();
            List<string> lstOneStoryFiles = new List<string>();
            foreach (DirectoryInfo di in dis)
            {
                FileInfo[] fis = di.GetFiles("*.onestory");
                foreach (FileInfo fi in fis)
                    lstOneStoryFiles.Add(fi.FullName);
            }
            return lstOneStoryFiles;
        }
    }
}
