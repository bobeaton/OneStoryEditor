using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using OseCommon;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// The save-time check (XSD validation, then the new loader) and the logic that the paste, revision-history and
    /// Chorus callers now share with the project loader (they build StoryData from an XElement).
    /// </summary>
    [TestFixture]
    public class SaveGuardTests
    {
        private const string ProjectName = "characterization-stories";

        private static string TestDataDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");

        private static string StoriesFixturePath => Path.Combine(TestDataDir, ProjectName + ".onestory");

        private static string MinimalFixturePath => Path.Combine(TestDataDir, "minimal-1.8.onestory");

        private readonly List<string> _tempPaths = new List<string>();
        private System.Diagnostics.TraceListener[] _savedListeners;

        [SetUp]
        public void MuteAsserts()
        {
            // duplicate story guids / missing strings Debug.Assert(false); with the default listener that kills the test host
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
        }

        [TearDown]
        public void Cleanup()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(_savedListeners);
            foreach (var strPath in _tempPaths)
            {
                try
                {
                    if (Directory.Exists(strPath))
                        Directory.Delete(strPath, true);
                    else
                        File.Delete(strPath);
                }
                catch (Exception)
                {
                    // best effort
                }
            }
        }

        private string TempPath(string strExtension)
        {
            var strPath = Path.Combine(Path.GetTempPath(), "ose-guard-" + Guid.NewGuid() + strExtension);
            _tempPaths.Add(strPath);
            return strPath;
        }

        private string MakeProjectFolder()
        {
            var strFolder = TempPath("");
            Directory.CreateDirectory(strFolder);
            // (the loader requires PanoramaFrontMatter, which this fixture doesn't have)
            File.WriteAllText(Path.Combine(strFolder, ProjectName + ".onestory"),
                File.ReadAllText(StoriesFixturePath).Replace("ProjectName=\"" + ProjectName + "\"",
                                                             "ProjectName=\"" + ProjectName + "\" PanoramaFrontMatter=\"pfm\""));
            return strFolder;
        }

        // ----- the save guard -----

        [Test]
        public void Fixture_PassesValidation()
        {
            // (the characterization fixtures put elements in an order the schema doesn't allow; this one is in order)
            Assert.DoesNotThrow(() => ProjectFileValidator.Validate(MinimalFixturePath));
        }

        [Test]
        public void StoryWithoutGuid_FailsValidation()
        {
            var strXml = File.ReadAllText(MinimalFixturePath);
            var strNoGuid = new Regex("(<story [^>]*?) guid=\"[^\"]*\"").Replace(strXml, "$1", 1);
            Assert.That(strNoGuid, Is.Not.EqualTo(strXml), "test setup: no guid was removed");
            var strPath = TempPath(".onestory.bad");
            File.WriteAllText(strPath, strNoGuid);

            Assert.Throws<XmlSchemaValidationException>(() => ProjectFileValidator.Validate(strPath));
        }

        [Test]
        public void BadDatatype_FailsValidation_EvenWithTheRelaxedSchema()
        {
            var strXml = File.ReadAllText(MinimalFixturePath);
            var strBad = new Regex("stageDateTimeStamp=\"[^\"]*\"").Replace(strXml, "stageDateTimeStamp=\"not-a-date\"", 1);
            Assert.That(strBad, Is.Not.EqualTo(strXml), "test setup: no timestamp replaced");
            var strPath = TempPath(".onestory.bad");
            File.WriteAllText(strPath, strBad);

            Assert.Throws<XmlSchemaValidationException>(() => ProjectFileValidator.Validate(strPath));
        }

        [Test]
        public void MalformedXml_FailsValidation()
        {
            var strPath = TempPath(".onestory.bad");
            File.WriteAllText(strPath, "<StoryProject version=\"1.8\" ProjectName=\"p\"><Members>");
            Assert.Throws<XmlException>(() => ProjectFileValidator.Validate(strPath));
        }

        [Test]
        public void FileWrittenByGetXml_PassesValidation_AndLoadsWithProjectReader()
        {
            var strFolder = MakeProjectFolder();
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");
            var contents = ProjectFile.Load(strPath);
            var project = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, ProjectName));

            // what SaveXElement does (it round-trips through XDocument.Parse and saves under a .bad extension)
            var strSaved = TempPath(".onestory.bad");
            OseXmlSerializer.SaveDoc(strSaved, XDocument.Parse(project.GetXml.ToString()), ex => { });

            Assert.DoesNotThrow(() => ProjectFileValidator.Validate(strSaved));
            Assert.DoesNotThrow(() => ProjectFile.Load(strSaved));

            // the released exe's typed DataSet must read it too
            ProjectReader ds = null;
            Assert.DoesNotThrow(() => ProjectReader.ReadProjectFile(strSaved, out ds));
            Assert.That(ds, Is.Not.Null);
            Assert.That(ds.story.Count, Is.GreaterThan(0));
        }

        // ----- SaveDoc -----

        [Test]
        public void SaveDoc_Throws_WhenTargetCannotBeWritten()
        {
            var strDirectory = TempPath("");
            Directory.CreateDirectory(strDirectory);    // a directory where the file should go
            var lstErrors = new List<Exception>();

            var ex = Assert.Throws<IOException>(() =>
                OseXmlSerializer.SaveDoc(strDirectory, new XDocument(new XElement("a")), lstErrors.Add));

            Assert.That(ex.InnerException, Is.Not.Null);
            Assert.That(lstErrors.Count, Is.GreaterThanOrEqualTo(2), "the handler is told about each failed attempt");
        }

        [Test]
        public void SaveDoc_Succeeds_AndClearsReadOnlyOnRetry()
        {
            var strPath = TempPath(".bad");
            File.WriteAllText(strPath, "old");
            File.SetAttributes(strPath, FileAttributes.ReadOnly);

            OseXmlSerializer.SaveDoc(strPath, new XDocument(new XElement("a")), ex => { });

            Assert.That(File.ReadAllText(strPath), Does.Contain("<a"));
            Assert.That(new FileInfo(strPath).IsReadOnly, Is.False);
        }

        // ----- the callers that used XmlNode now build from the XElement -----

        // an item without a guid in the file gets a random one on every load (the fixture's own guids start with 00000000)
        private static string StripGeneratedGuids(string strXml)
        {
            // (and the story's own guid: a duplicate of an earlier story's guid is replaced by a random one on the normal load)
            // and its name (the project load makes a repeated name unique, e.g. "Story Minimal.1")
            strXml = new Regex("<story name=\"[^\"]*\"").Replace(strXml, "<story name=\"*\"", 1);
            strXml = new Regex("(<story [^>]*?)guid=\"[^\"]*\"").Replace(strXml, "$1guid=\"*\"", 1);
            return Regex.Replace(strXml, "guid=\"(?!00000000)[^\"]*\"", "guid=\"*\"");
        }

        [Test]
        public void StoryBuiltTheWayPasteBuildsIt_MatchesTheNormalLoad()
        {
            var strFolder = MakeProjectFolder();
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");
            var contents = ProjectFile.Load(strPath);
            var project = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, ProjectName));

            // (in file order; story names repeat across the sets, and so does a guid)
            var normal = new List<string>();
            foreach (var setName in project.Keys.Cast<string>())
                foreach (StoryData sd in project[setName])
                    normal.Add(sd.GetXml.ToString());
            Assert.That(normal.Count, Is.GreaterThan(0));

            // a fresh read, wrapped the way the clipboard wraps a story. (Read with ProjectFile.Load, so whitespace-only
            //  values are kept as the project load keeps them; the clipboard's XElement.Parse drops those, which is
            //  how paste has always behaved and isn't what's being compared here.)
            var elemStories = ProjectFile.Load(strPath).Root.Descendants(StoryData.CstrElementNameStory).ToList();
            Assert.That(elemStories.Count, Is.EqualTo(normal.Count));
            for (int i = 0; i < elemStories.Count; i++)
            {
                var elemStory = elemStories[i];
                var wrapper = new XElement(StoryProjectData.CstrElementOseStoryToCopy, new XElement(elemStory));
                LegacyTextRepair.DecodeUnlessMarked(wrapper);
                // (the project load also clears values that are only a language's name; paste doesn't)
                LegacyTextRepair.ClearLanguageNamePlaceholders(wrapper);
                ProjectReader.UniqueStoryGuids.Clear();     // (the project load registered this guid; a story seen twice gets a new one)
                var pasted = new StoryData(wrapper.Element(StoryData.CstrElementNameStory), strFolder);
                var strName = (string)elemStory.Attribute("name");
                Assert.That(StripGeneratedGuids(pasted.GetXml.ToString()), Is.EqualTo(StripGeneratedGuids(normal[i])), "story " + strName);

                // the second pass (regenerates the guids) keeps the content
                var copy = new StoryData(pasted);
                Assert.That(copy.guid, Is.Not.EqualTo(pasted.guid));
                Assert.That(copy.Verses.Count, Is.EqualTo(pasted.Verses.Count));
            }
        }

        [Test]
        public void StoryBuiltTheWayChorusBuildsIt_KeepsFinishedConversationsAndTransitionHistory()
        {
            // the old XmlNode constructors lost both of these
            var doc = new XmlDocument();
            doc.Load(Path.Combine(TestDataDir, "characterization.onestory"));
            var strFolder = TempPath("");
            var nodes = doc.SelectNodes("/StoryProject/stories/story").Cast<XmlNode>().ToList();
            Assert.That(nodes.Count, Is.GreaterThan(0));

            int nFinished = 0, nWithHistory = 0;
            foreach (var node in nodes)
            {
                var elem = XElement.Parse(node.OuterXml);   // what GetPresentationHtmlForChorus does
                LegacyTextRepair.DecodePlainTextElements(elem);
                ProjectReader.UniqueStoryGuids.Clear();
                var story = new StoryData(elem, strFolder);
                var strXml = story.GetXml.ToString();
                if (elem.Descendants().Any(e => (string)e.Attribute("finished") == "true"))
                {
                    nFinished++;
                    Assert.That(strXml, Does.Contain("finished=\"true\""), "finished conversation lost in " + story.Name);
                }
                if (elem.Element("TransitionHistory") != null && elem.Element("TransitionHistory").HasElements)
                {
                    nWithHistory++;
                    Assert.That(story.TransitionHistory.HasData, Is.True, "history empty in " + story.Name);
                    Assert.That(strXml, Does.Contain("<StateTransition "));
                }
            }
            Assert.That(nFinished, Is.GreaterThan(0), "fixture has no finished conversation");
            Assert.That(nWithHistory, Is.GreaterThan(0), "fixture has no transition history");
        }

        [Test]
        public void GetPresentationHtmlForChorus_WithXmlNodes_ReturnsHtml()
        {
            // this fixture has all of the languages configured (the stories-only one doesn't)
            const string strName = "characterization-project";
            var strFolder = TempPath("");
            Directory.CreateDirectory(strFolder);
            var strPath = Path.Combine(strFolder, strName + ".onestory");
            File.Copy(Path.Combine(TestDataDir, strName + ".onestory"), strPath);
            var contents = ProjectFile.Load(strPath);
            var project = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, strName));
            var anyStory = project.Values.Cast<StoriesData>().SelectMany(s => s.Cast<StoryData>()).First();

            var doc = new XmlDocument();
            doc.Load(strPath);
            var nodes = doc.SelectNodes("/StoryProject/stories/story").Cast<XmlNode>().ToList();
            Assert.That(nodes.Count, Is.GreaterThan(0));
            foreach (var node in nodes)
            {
                var strBefore = node.OuterXml;
                var strHtml = anyStory.GetPresentationHtmlForChorus(doc.DocumentElement, strFolder, node, node);
                Assert.That(strHtml, Is.Not.Null.And.Not.Empty);
                Assert.That(node.OuterXml, Is.EqualTo(strBefore), "Chorus's node must not be modified");
            }
        }
    }
}
