using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// What was read from a .onestory file: the root element (to build the StoryProjectData from),
    /// whether it was saved by a version that keeps plain text (see LegacyTextRepair) and its write time.
    /// </summary>
    public class ProjectFileContents
    {
        public XElement Root { get; }
        public bool IsPlainTextEncoded { get; }
        public DateTime LastWriteTime { get; }

        public ProjectFileContents(XElement root, bool bIsPlainTextEncoded, DateTime lastWriteTime)
        {
            Root = root;
            IsPlainTextEncoded = bIsPlainTextEncoded;
            LastWriteTime = lastWriteTime;
        }
    }

    /// <summary>
    /// Loads a .onestory file as XML (replacing ProjectReader/the typed DataSet), refusing files that
    /// this version can't read. It throws instead of showing a message box; the caller reports the message.
    /// </summary>
    public static class ProjectFile
    {
        public const string CstrVersionTooOldMessage =
            "This project was saved by a very old version of OneStory Editor. Open and save it with OneStory Editor 4.x first.";

        public static ProjectFileContents Load(string strPath)
        {
            XDocument doc;
            // DataSet.ReadXml accepts a DOCTYPE; ignore it here too (but never resolve anything)
            using (var reader = XmlReader.Create(strPath, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                doc = XDocument.Load(reader);

            ProjectReader.UniqueStoryGuids.Clear();

            var root = doc.Root;
            if (root.Name.LocalName != StoryProjectData.CstrElementStoryProjectRoot)
                throw new ApplicationException(
                    $"The project file is damaged: its root element is <{root.Name}> instead of <{StoryProjectData.CstrElementStoryProjectRoot}>.");

            var strVersion = XmlRead.RequiredAttr(root, StoryProjectData.CstrAttributeVersion);
            if (strVersion == "1.3" || strVersion == "1.4")
                throw new ApplicationException(CstrVersionTooOldMessage);

            if (StoryProjectData.IsNewerThanSupported(strVersion))
                throw new ApplicationException(Localizer.Str("One of the team members is using a newer version of OSE to edit the file, which is not compatible with the version you are using. You might try, \"Advanced\", \"Program Updates\", \"Check now\" or \"Check now for next major update\" or you may have to go to the http://palaso.org/install/onestory website and download and install the new version of the program in the \"Setup OneStory Editor.zip\" file"));

            return new ProjectFileContents(root,
                                           LegacyTextRepair.IsMarkedPlain(root),
                                           File.GetLastWriteTime(strPath));
        }
    }
}
