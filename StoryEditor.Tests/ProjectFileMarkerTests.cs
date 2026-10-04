using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using OneStoryProjectEditor.Tests.ReleasedExeDataSet;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ProjectFileMarkerTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minimal-1.8.onestory");

        [Test]
        public void UnmarkedFile_IsNotPlainTextEncoded()
        {
            Assert.That(ProjectFile.Load(FixturePath).IsPlainTextEncoded, Is.False);
        }

        [Test]
        public void MarkedFile_LoadsAndIsPlainTextEncoded()
        {
            var strMarked = Path.Combine(Path.GetTempPath(), "ose-marked-" + Guid.NewGuid() + ".onestory");
            try
            {
                var doc = XDocument.Load(FixturePath);
                LegacyTextRepair.MarkAsPlain(doc.Root);
                doc.Save(strMarked);

                var contents = ProjectFile.Load(strMarked);
                Assert.That(contents.IsPlainTextEncoded, Is.True);
                Assert.That((string)contents.Root.Attribute("version"), Is.EqualTo("1.8"));

                // the released exe's typed DataSet (a test-only copy) must ignore the new attribute: loading without an
                //  exception shows that older versions will read the marked file
                var ds = new NewDataSet();
                ds.ReadXml(strMarked);
                Assert.That(ds.StoryProject[0].version, Is.EqualTo("1.8"));
            }
            finally
            {
                File.Delete(strMarked);
            }
        }

        [Test]
        public void DoctypeInTheFile_IsIgnoredNotResolved()
        {
            var strDoctype = Path.Combine(Path.GetTempPath(), "ose-doctype-" + Guid.NewGuid() + ".onestory");
            try
            {
                var strText = File.ReadAllText(FixturePath).Replace("<StoryProject", "<!DOCTYPE StoryProject [<!ENTITY x \"y\">]><StoryProject");
                File.WriteAllText(strDoctype, strText);
                Assert.That(ProjectFile.Load(strDoctype).IsPlainTextEncoded, Is.False);
            }
            finally
            {
                File.Delete(strDoctype);
            }
        }
    }
}
