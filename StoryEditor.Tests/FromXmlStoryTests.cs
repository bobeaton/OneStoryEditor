using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Golden tests for the verse/line/transition/crafting/story XElement constructors (see Golden): each test builds
    /// the objects from the fixture elements and compares what they hold (every field the old row-constructor oracle
    /// tests compared, and GetXml) with a golden file. Assertions on values are kept as they were.
    /// The stories live in their own fixture (characterization-stories.onestory) so the counts that the
    /// earlier characterization tests assert about characterization.onestory stay as they are.
    /// </summary>
    [TestFixture]
    public class FromXmlStoryTests
    {
        private static string TestDataDir =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");

        private static string StoriesFixturePath => Path.Combine(TestDataDir, "characterization-stories.onestory");
        private static string ContentFixturePath => Path.Combine(TestDataDir, "characterization.onestory");

        private XDocument _doc;
        private List<XElement> _storyElems;

        private System.Diagnostics.TraceListener[] _savedListeners;

        [SetUp]
        public void Load()
        {
            // The loader Debug.Assert(false)s on a duplicate story guid (and the stage-transition loader
            //  asserts on missing strings); with the default listener that kills the test host, so mute asserts.
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();

            _doc = XDocument.Load(StoriesFixturePath, LoadOptions.None);
            _storyElems = _doc.Descendants("story").ToList();
            Assert.That(_storyElems.Count, Is.EqualTo(4));
        }

        [TearDown]
        public void RestoreListeners()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(_savedListeners);
        }

        private const string GeneratedFirstGuid = "00000000-0000-0000-0000-generated-first";

        // When the story has no leading first="true" verse, InsureFirstVerse makes a new VerseData with a random guid;
        //  give it a fixed one so the XML can be compared with a golden file.
        private static void FixGeneratedFirstGuid(XElement elemStory, VersesData verses)
        {
            var firstElem = elemStory.Element("Verses")?.Elements("Verse").FirstOrDefault();
            var bHasFirst = firstElem != null && (string)firstElem.Attribute("first") == "true";
            if (!bHasFirst)
                verses.FirstVerse.guid = GeneratedFirstGuid;
        }

        // ----- verses -----

        [Test]
        public void VersesData_Golden()
        {
            var dump = new Dump();
            var nFirstFlagged = 0;
            var nWithVerses = 0;
            for (int i = 0; i < _storyElems.Count; i++)
            {
                var verses = new VersesData(_storyElems[i]);
                FixGeneratedFirstGuid(_storyElems[i], verses);
                dump.Raw($"== story {i}").Line("Count", verses.Count)
                    .Line("FirstVerse IsFirstVerse", verses.FirstVerse.IsFirstVerse)
                    .Line("FirstVerse guid", verses.FirstVerse.guid)
                    .Xml("FirstVerse GetXml", verses.FirstVerse.GetXml);
                if (_storyElems[i].Descendants("Verse").Any(v => (string)v.Attribute("first") == "true"))
                    nFirstFlagged++;
                for (int j = 0; j < verses.Count; j++)
                {
                    Assert.That(verses[j].IsFirstVerse, Is.False, $"story {i} verse {j}");
                    dump.Line($"verse {j} guid", verses[j].guid).Line($"verse {j} IsVisible", verses[j].IsVisible)
                        .Xml($"verse {j} GetXml", verses[j].GetXml);
                }
                dump.Line("HasData", verses.HasData);
                if (verses.HasData)
                {
                    nWithVerses++;
                    dump.Xml("GetXml", verses.GetXml);
                }
            }
            Golden.Check("story-verses", dump.ToString());
            Assert.That(nFirstFlagged, Is.GreaterThanOrEqualTo(1));
            Assert.That(nWithVerses, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void VersesData_FirstFlaggedVerseBecomesFirstVerse_AndStrayFlagsAreCleared()
        {
            var verses = new VersesData(_storyElems[0]);
            Assert.That(verses.FirstVerse.guid, Is.EqualTo("00000000-0000-0000-0000-000000000110"));
            Assert.That(verses.Count, Is.EqualTo(2));
            Assert.That(verses.All(v => !v.IsFirstVerse), Is.True);
            Assert.That(verses[1].IsVisible, Is.False);
        }

        [Test]
        public void VersesData_NoVersesElement_GetsNewFirstVerse()
        {
            var newVerses = new VersesData(_storyElems[1]);
            Assert.That(newVerses.Count, Is.EqualTo(0));
            Assert.That(newVerses.FirstVerse, Is.Not.Null);
            Assert.That(newVerses.FirstVerse.IsFirstVerse, Is.True);
        }

        [Test]
        public void VerseData_Golden_ForEveryFixtureVerse()
        {
            // the earlier fixture has visible="1"/"false", first="true", padded and empty lines etc.
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var elems = doc.Descendants("Verse").ToList();
            Assert.That(elems.Count, Is.EqualTo(8));
            var dump = new Dump();
            for (int i = 0; i < elems.Count; i++)
            {
                var verse = new VerseData(elems[i]);
                dump.Raw($"== verse {i}").Line("guid", verse.guid).Line("IsFirstVerse", verse.IsFirstVerse)
                    .Line("IsVisible", verse.IsVisible).Xml("GetXml", verse.GetXml);
            }
            Golden.Check("content-verses", dump.ToString());
        }

        // ----- transitions -----

        [Test]
        public void StoryStateTransitionHistory_Golden_DroppingDuplicates()
        {
            var dump = new Dump();
            for (int i = 0; i < _storyElems.Count; i++)
            {
                var history = new StoryStateTransitionHistory(_storyElems[i]);
                dump.Raw($"== story {i}").Line("Count", history.Count).Line("HasData", history.HasData);
                for (int j = 0; j < history.Count; j++)
                    dump.Line($"t{j} LoggedInMemberId", history[j].LoggedInMemberId)
                        .Line($"t{j} WindowsUserName", history[j].WindowsUserName)
                        .Line($"t{j} FromState", history[j].FromState).Line($"t{j} ToState", history[j].ToState)
                        .Line($"t{j} TransitionDateTime", history[j].TransitionDateTime);
                if (history.HasData)
                    dump.Xml("GetXml", history.GetXml);
            }
            Golden.Check("story-transition-histories", dump.ToString());

            // 4 elements, one an exact duplicate, so 3 survive
            Assert.That(new StoryStateTransitionHistory(_storyElems[0]).Count, Is.EqualTo(3));
        }

        [Test]
        public void StoryStateTransition_Golden_ForEveryFixtureTransition()
        {
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var elems = doc.Descendants("StateTransition").ToList();
            Assert.That(elems.Count, Is.EqualTo(3));
            var dump = new Dump();
            for (int i = 0; i < elems.Count; i++)
            {
                var t = new StoryStateTransition(elems[i]);
                dump.Raw($"== transition {i}").Line("TransitionDateTime", t.TransitionDateTime).Xml("GetXml", t.GetXml);
            }
            Golden.Check("content-transitions", dump.ToString());
        }

        // ----- crafting info -----

        private static void DumpMember(Dump dump, string strWhat, MemberIdInfo member)
        {
            if (member == null)
            {
                dump.Line(strWhat, null);
                return;
            }
            dump.Line(strWhat + " MemberId", member.MemberId).Line(strWhat + " MemberComment", member.MemberComment);
        }

        private static void DumpTesters(Dump dump, string strWhat, TestInfo testers)
        {
            dump.Line(strWhat + " Count", testers.Count);
            for (int i = 0; i < testers.Count; i++)
                DumpMember(dump, $"{strWhat} [{i}]", testers[i]);
        }

        private static void DumpCraftingInfo(Dump dump, CraftingInfoData ci)
        {
            dump.Line("IsBiblicalStory", ci.IsBiblicalStory);
            DumpMember(dump, "crafter", ci.StoryCrafter);
            DumpMember(dump, "pf", ci.ProjectFacilitator);
            DumpMember(dump, "consultant", ci.Consultant);
            DumpMember(dump, "coach", ci.Coach);
            DumpMember(dump, "bt", ci.BackTranslator);
            DumpMember(dump, "oebt", ci.OutsideEnglishBackTranslator);
            dump.Line("StoryPurpose", ci.StoryPurpose).Line("ResourcesUsed", ci.ResourcesUsed)
                .Line("MiscellaneousStoryInfo", ci.MiscellaneousStoryInfo);
            DumpTesters(dump, "retellings", ci.TestersToCommentsRetellings);
            DumpTesters(dump, "tq answers", ci.TestersToCommentsTqAnswers);
            dump.Xml("GetXml", ci.GetXml);
        }

        [Test]
        public void CraftingInfoData_Golden()
        {
            var bSawNonBiblical = false;
            var dump = new Dump();
            for (int i = 0; i < _storyElems.Count; i++)
            {
                var ci = new CraftingInfoData(_storyElems[i]);
                bSawNonBiblical |= !ci.IsBiblicalStory;
                DumpCraftingInfo(dump.Raw($"== story {i}"), ci);
            }
            Golden.Check("story-crafting-infos", dump.ToString());
            Assert.That(bSawNonBiblical, Is.True);
        }

        [Test]
        public void CraftingInfoData_FullStory_HasEveryMemberKindAndNormalizedText()
        {
            var ci = new CraftingInfoData(_storyElems[0]);
            Assert.That(ci.IsBiblicalStory, Is.False);
            Assert.That(ci.StoryCrafter.MemberComment, Is.EqualTo("crafter comment"));
            Assert.That(ci.ProjectFacilitator, Is.Not.Null);
            Assert.That(ci.Consultant.MemberComment, Is.EqualTo("consultant\r\ncomment"));
            Assert.That(ci.Coach, Is.Not.Null);
            Assert.That(ci.Coach.MemberComment, Is.Null);   // whitespace-only text is null in the DataSet
            Assert.That(ci.BackTranslator, Is.Not.Null);
            Assert.That(ci.OutsideEnglishBackTranslator, Is.Not.Null);
            Assert.That(ci.StoryPurpose, Is.EqualTo("purpose line 1\r\npurpose line 2"));
            Assert.That(ci.ResourcesUsed, Is.EqualTo("resources\r\nalready crlf"));
            Assert.That(ci.TestersToCommentsRetellings.Count, Is.EqualTo(2));
            Assert.That(ci.TestersToCommentsTqAnswers.Count, Is.EqualTo(2));
        }

        [Test]
        public void CraftingInfoData_MissingCraftingInfo_ThrowsNoCraftingInfoMessage()
        {
            var ex = Assert.Throws<ApplicationException>(() =>
                new CraftingInfoData(XElement.Parse("<story name=\"s\" />")));
            Assert.That(ex.Message, Is.EqualTo(Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo));
        }

        [Test]
        public void CraftingInfoData_MissingStoryCrafter_Throws()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\" /></story>";
            AssertStoryThrows(strStory, Properties.Resources.IDS_ProjectFileCorrupted);
        }

        [Test]
        public void CraftingInfoData_TwoStoryCrafters_Throws()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\">" +
                                    "<StoryCrafter memberID=\"m1\" /><StoryCrafter memberID=\"m1\" />" +
                                    "</CraftingInfo></story>";
            AssertStoryThrows(strStory, Properties.Resources.IDS_ProjectFileCorrupted);
        }

        [Test]
        public void CraftingInfoData_TwoCraftingInfoElements_Throws()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "</story>";
            AssertStoryThrows(strStory, Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo);
        }

        [Test]
        public void CraftingInfoData_TwoOptionalMembers_AreIgnored()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" />" +
                                    "<Consultant memberID=\"m2\">a</Consultant><Consultant memberID=\"m3\">b</Consultant>" +
                                    "<TestsRetellings><TestRetelling memberID=\"m2\">a</TestRetelling></TestsRetellings>" +
                                    "<TestsRetellings><TestRetelling memberID=\"m3\">b</TestRetelling></TestsRetellings>" +
                                    "</CraftingInfo></story>";
            var ci = new CraftingInfoData(XElement.Parse(strStory));
            Assert.That(ci.Consultant, Is.Null);
            Assert.That(ci.TestersToCommentsRetellings.Count, Is.EqualTo(0));
            Golden.Check("crafting-two-optional-members", ci.GetXml.ToString());
        }

        // the loader's own message for a damaged story
        private static void AssertStoryThrows(string strStoryXml, string strExpectedMessage)
        {
            var elemStory = XElement.Parse(strStoryXml);
            var ex = Assert.Throws<ApplicationException>(() => new StoryData(elemStory, TestDataDir));
            Assert.That(ex.Message, Is.EqualTo(strExpectedMessage));
        }

        // ----- stories -----

        [Test]
        public void StoryData_Golden()
        {
            var dump = new Dump();
            for (int i = 0; i < _storyElems.Count; i++)
            {
                ProjectFile.UniqueStoryGuids.Clear();
                // the duplicate-guid story needs the earlier story registered first
                var newStories = _storyElems.Take(i + 1).Select(e => new StoryData(e, TestDataDir)).ToList();
                var newStory = newStories.Last();

                dump.Raw($"== story {i}").Line("Name", newStory.Name).Line("TasksAllowedPf", newStory.TasksAllowedPf)
                    .Line("TasksRequiredPf", newStory.TasksRequiredPf).Line("TasksAllowedCit", newStory.TasksAllowedCit)
                    .Line("TasksRequiredCit", newStory.TasksRequiredCit)
                    .Line("CountRetellingsTests", newStory.CountRetellingsTests)
                    .Line("CountTestingQuestionTests", newStory.CountTestingQuestionTests)
                    .Line("IsBiblicalStory", newStory.CraftingInfo.IsBiblicalStory);

                if (_storyElems[i].Attribute("stageDateTimeStamp") == null)
                    // no stamp: the loader uses DateTime.Now
                    Assert.That((newStory.StageTimeStamp - DateTime.Now).Duration(), Is.LessThan(TimeSpan.FromSeconds(30)));
                dump.Line("StageTimeStamp", newStory.StageTimeStamp);

                FixGeneratedFirstGuid(_storyElems[i], newStory.Verses);
                if (i == 2)
                {
                    // duplicate guid: the replacement guid is random (see the guid tests)
                    Assert.That(newStory.guid, Is.Not.EqualTo((string)_storyElems[i].Attribute("guid")));
                    newStory.guid = "{REPLACED}";
                }
                dump.Line("guid", newStory.guid).Xml("GetXml", newStory.GetXml);
            }
            Golden.Check("story-stories", dump.ToString());
        }

        [Test]
        public void StoryData_DuplicateGuid_AssignsNewGuidToTheDuplicate()
        {
            const string strOriginal = "00000000-0000-0000-0000-000000000101";

            ProjectFile.UniqueStoryGuids.Clear();
            var newStories = _storyElems.Select(e => new StoryData(e, TestDataDir)).ToList();

            Assert.That(newStories[0].guid, Is.EqualTo(strOriginal));
            Assert.That(newStories[2].guid, Is.Not.EqualTo(strOriginal));
            Assert.That(Guid.TryParse(newStories[2].guid, out _), Is.True);
            Assert.That(newStories.Select(s => s.guid).Distinct().Count(), Is.EqualTo(newStories.Count));
            Assert.That(ProjectFile.UniqueStoryGuids, Does.Contain(newStories[2].guid));
        }

        [Test]
        public void StoryData_SameNameTwice_BothKept()
        {
            ProjectFile.UniqueStoryGuids.Clear();
            var newStories = _storyElems.Select(e => new StoryData(e, TestDataDir)).ToList();
            Assert.That(newStories.Count(s => s.Name == "Story Minimal"), Is.EqualTo(2));
            Assert.That(newStories[1].guid, Is.Not.EqualTo(newStories[3].guid));
        }

        [Test]
        public void StoryData_ContentFixture_Golden()
        {
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var elems = doc.Descendants("story").ToList();
            Assert.That(elems.Count, Is.EqualTo(5));
            ProjectFile.UniqueStoryGuids.Clear();
            var newStories = elems.Select(e => new StoryData(e, TestDataDir)).ToList();
            var dump = new Dump();
            for (int i = 0; i < elems.Count; i++)
            {
                FixGeneratedFirstGuid(elems[i], newStories[i].Verses);
                dump.Raw($"== story {i}").Xml("GetXml", newStories[i].GetXml);
            }
            Golden.Check("content-stories", dump.ToString());
        }
    }
}
