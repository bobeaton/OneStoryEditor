using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Detached stories (paste, revision history, Chorus html) must neither check nor register their guid in
    /// ProjectFile.UniqueStoryGuids. These tests deliberately leave the Trace listeners in place, so a
    /// Debug.Assert on a duplicate guid would fail the run.
    /// </summary>
    [TestFixture]
    public class DetachedStoryGuidTests
    {
        private static string TestDataDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");

        [Test]
        public void DetachedStory_BuiltTwice_NoAssertAndGuidsUnchanged()
        {
            var elemStory = XDocument.Load(Path.Combine(TestDataDir, "characterization-stories.onestory"))
                .Descendants("story").First();
            var strGuid = (string)elemStory.Attribute("guid");

            ProjectFile.UniqueStoryGuids.Clear();
            ProjectFile.UniqueStoryGuids.Add("already there");
            var first = new StoryData(new XElement(elemStory), TestDataDir, false);
            var second = new StoryData(new XElement(elemStory), TestDataDir, false);

            Assert.That(first.guid, Is.EqualTo(strGuid));
            Assert.That(second.guid, Is.EqualTo(strGuid));
            Assert.That(ProjectFile.UniqueStoryGuids, Is.EqualTo(new[] { "already there" }));
        }

        [Test]
        public void ProjectLoadPath_StillReplacesDuplicateGuids()
        {
            // (the project-load path tracks guids; the duplicate-guid assert is muted here as it is in FromXmlStoryTests)
            var saved = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(saved, 0);
            System.Diagnostics.Trace.Listeners.Clear();
            try
            {
                var elemStory = XDocument.Load(Path.Combine(TestDataDir, "characterization-stories.onestory"))
                    .Descendants("story").First();
                ProjectFile.UniqueStoryGuids.Clear();
                var first = new StoryData(new XElement(elemStory), TestDataDir);
                var second = new StoryData(new XElement(elemStory), TestDataDir);

                Assert.That(second.guid, Is.Not.EqualTo(first.guid));
                Assert.That(ProjectFile.UniqueStoryGuids, Does.Contain(first.guid).And.Contain(second.guid));
            }
            finally
            {
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.AddRange(saved);
            }
        }
    }
}
