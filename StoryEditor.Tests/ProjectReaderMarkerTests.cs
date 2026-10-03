using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ProjectReaderMarkerTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minimal-1.8.onestory");

        [Test]
        public void UnmarkedFile_IsNotPlainTextEncoded()
        {
            ProjectReader projFile;
            ProjectReader.ReadProjectFile(FixturePath, out projFile);
            Assert.That(projFile.IsPlainTextEncoded, Is.False);
        }

        [Test]
        public void MarkedFile_LoadsAndIsPlainTextEncoded()
        {
            // this typed DataSet is the same one the released exe uses, so loading without an
            //  exception also shows that older versions will ignore the new attribute
            var strMarked = Path.Combine(Path.GetTempPath(), "ose-marked-" + Guid.NewGuid() + ".onestory");
            try
            {
                var doc = XDocument.Load(FixturePath);
                LegacyTextRepair.MarkAsPlain(doc.Root);
                doc.Save(strMarked);

                ProjectReader projFile;
                ProjectReader.ReadProjectFile(strMarked, out projFile);

                Assert.That(projFile.IsPlainTextEncoded, Is.True);
                Assert.That(projFile.StoryProject[0].version, Is.EqualTo("1.8"));
            }
            finally
            {
                File.Delete(strMarked);
            }
        }
    }
}
