using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ProjectFileTests
    {
        private readonly List<string> _tempPaths = new List<string>();

        private static string TestDataDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");

        [TearDown]
        public void Cleanup()
        {
            foreach (var strPath in _tempPaths)
            {
                try
                {
                    File.Delete(strPath);
                }
                catch (Exception)
                {
                    // best effort
                }
            }
        }

        private string WriteTemp(string strRootAttributes, string strPrologExtra = "")
        {
            var strPath = Path.Combine(Path.GetTempPath(), "ose-pf-" + Guid.NewGuid() + ".onestory");
            File.WriteAllText(strPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + strPrologExtra +
                "<StoryProject " + strRootAttributes + " ProjectName=\"p\"><Members /></StoryProject>");
            _tempPaths.Add(strPath);
            return strPath;
        }

        [TestCase("1.3")]
        [TestCase("1.4")]
        public void VeryOldVersions_Throw(string strVersion)
        {
            var ex = Assert.Throws<ApplicationException>(() => ProjectFile.Load(WriteTemp("version=\"" + strVersion + "\"")));
            Assert.That(ex.Message, Is.EqualTo("This project was saved by a very old version of OneStory Editor. Open and save it with OneStory Editor 4.x first."));
        }

        [TestCase("2.0")]
        [TestCase("1.9")]
        [TestCase("3.1")]
        public void NewerVersions_Throw_WithTheNewerVersionMessage(string strVersion)
        {
            var ex = Assert.Throws<ApplicationException>(() => ProjectFile.Load(WriteTemp("version=\"" + strVersion + "\"")));
            Assert.That(ex.Message, Is.EqualTo("One of the team members is using a newer version of OSE to edit the file, which is not compatible with the version you are using. You might try, \"Advanced\", \"Program Updates\", \"Check now\" or \"Check now for next major update\" or you may have to go to the http://palaso.org/install/onestory website and download and install the new version of the program in the \"Setup OneStory Editor.zip\" file"));
        }

        [Test]
        public void MissingVersion_Throws()
        {
            var strPath = Path.Combine(Path.GetTempPath(), "ose-pf-" + Guid.NewGuid() + ".onestory");
            File.WriteAllText(strPath, "<StoryProject ProjectName=\"p\" />");
            _tempPaths.Add(strPath);
            var ex = Assert.Throws<ApplicationException>(() => ProjectFile.Load(strPath));
            Assert.That(ex.Message, Is.EqualTo("The project file is damaged: <StoryProject> is missing the required attribute 'version'."));
        }

        [Test]
        public void EmptyVersion_Loads()
        {
            Assert.That(ProjectFile.Load(WriteTemp("version=\"\"")).Root.Name.LocalName, Is.EqualTo("StoryProject"));
        }

        [Test]
        public void WrongRootName_Throws()
        {
            var strPath = Path.Combine(Path.GetTempPath(), "ose-pf-" + Guid.NewGuid() + ".onestory");
            File.WriteAllText(strPath, "<Other version=\"1.8\" />");
            _tempPaths.Add(strPath);
            var ex = Assert.Throws<ApplicationException>(() => ProjectFile.Load(strPath));
            Assert.That(ex.Message, Is.EqualTo("The project file is damaged: its root element is <Other> instead of <StoryProject>."));
        }

        [TestCase("1.5")]
        [TestCase("1.6")]
        [TestCase("1.7")]
        [TestCase("1.8")]
        public void SupportedVersions_Load(string strVersion)
        {
            var strPath = WriteTemp("version=\"" + strVersion + "\"");
            var contents = ProjectFile.Load(strPath);
            Assert.That(contents.Root.Name.LocalName, Is.EqualTo("StoryProject"));
            Assert.That((string)contents.Root.Attribute("version"), Is.EqualTo(strVersion));
            Assert.That(contents.IsPlainTextEncoded, Is.False);
            Assert.That(contents.LastWriteTime, Is.EqualTo(File.GetLastWriteTime(strPath)));
        }

        [Test]
        public void PlainTextMarker_IsRead()
        {
            Assert.That(ProjectFile.Load(WriteTemp("version=\"1.8\" TextEncoding=\"plain\"")).IsPlainTextEncoded, Is.True);
            Assert.That(ProjectFile.Load(WriteTemp("version=\"1.8\" TextEncoding=\"other\"")).IsPlainTextEncoded, Is.False);
            Assert.That(ProjectFile.Load(WriteTemp("version=\"1.8\"")).IsPlainTextEncoded, Is.False);
        }

        [Test]
        public void FileWithDoctype_Loads()
        {
            var strPath = WriteTemp("version=\"1.8\"", "<!DOCTYPE StoryProject [ <!ENTITY e \"x\"> ]>");
            var contents = ProjectFile.Load(strPath);
            Assert.That(contents.Root.Name.LocalName, Is.EqualTo("StoryProject"));
        }

        [Test]
        public void Load_ClearsTheUniqueStoryGuids()
        {
            ProjectReader.UniqueStoryGuids.Add("left over");
            ProjectFile.Load(Path.Combine(TestDataDir, "minimal-1.8.onestory"));
            Assert.That(ProjectReader.UniqueStoryGuids, Is.Empty);
        }

        [Test]
        public void Load_FixtureWithMarker_MatchesProjectReader()
        {
            var strPath = Path.Combine(TestDataDir, "minimal-1.8.onestory");
            ProjectReader.ReadProjectFile(strPath, out var ds);
            var contents = ProjectFile.Load(strPath);
            Assert.That(contents.IsPlainTextEncoded, Is.EqualTo(ds.IsPlainTextEncoded));
        }

        [Test]
        public void LnCNote_OldSingularKeyTermIdAttribute_StillLoads()
        {
            var note = new LnCNote(System.Xml.Linq.XElement.Parse(
                "<LnCNote guid=\"g1\" KeyTermId=\"KT1, KT2\">text</LnCNote>"));
            Assert.That(note.GetXml.Attribute("KeyTermIds")?.Value, Is.EqualTo("KT1, KT2"));
            Assert.That(note.GetXml.Attribute("KeyTermId"), Is.Null);
        }

        [Test]
        public void LnCNote_KeyTermIds_RoundTrips()
        {
            var note = new LnCNote(System.Xml.Linq.XElement.Parse(
                "<LnCNote guid=\"g1\" KeyTermIds=\"KT3\">text</LnCNote>"));
            var again = new LnCNote(note.GetXml);
            Assert.That(again.GetXml.ToString(), Is.EqualTo(note.GetXml.ToString()));
            Assert.That(again.GetXml.Attribute("KeyTermIds")?.Value, Is.EqualTo("KT3"));
        }

        [Test]
        public void MissingFile_Throws()
        {
            Assert.Throws<FileNotFoundException>(() => ProjectFile.Load(Path.Combine(Path.GetTempPath(), "ose-does-not-exist.onestory")));
        }
    }
}
