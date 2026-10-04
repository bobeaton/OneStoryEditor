using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// The typed DataSet row constructors are the oracle for the verse/line/transition/crafting/story XElement
    /// constructors. A fresh ProjectReader per test, because the row constructors add empty container rows.
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

        private ProjectReader _ds;
        private XDocument _doc;
        private List<NewDataSet.storyRow> _storyRows;
        private List<XElement> _storyElems;

        private System.Diagnostics.TraceListener[] _savedListeners;

        [SetUp]
        public void Load()
        {
            // The row constructors Debug.Assert(false) on a duplicate story guid (and the stage-transition loader
            //  asserts on missing strings); with the default listener that kills the test host, so mute asserts.
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
            Load(StoriesFixturePath);
        }

        [TearDown]
        public void RestoreListeners()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(_savedListeners);
        }

        private void Load(string strPath)
        {
            ProjectReader.ReadProjectFile(strPath, out _ds);
            _doc = XDocument.Load(strPath, LoadOptions.None);
            _storyRows = _ds.story.ToList();
            _storyElems = _doc.Descendants("story").ToList();
            Assert.That(_storyElems.Count, Is.EqualTo(_storyRows.Count));
        }

        // When the story has no leading first="true" verse, InsureFirstVerse makes a new VerseData with a random guid
        //  on both paths; give the new path's the old path's so the XML can be compared.
        private static void AlignGeneratedFirstGuid(XElement elemStory, VersesData oldVerses, VersesData newVerses)
        {
            var firstElem = elemStory.Element("Verses")?.Elements("Verse").FirstOrDefault();
            var bHasFirst = firstElem != null && (string)firstElem.Attribute("first") == "true";
            if (!bHasFirst)
                newVerses.FirstVerse.guid = oldVerses.FirstVerse.guid;
        }

        // ----- verses -----

        [Test]
        public void VersesData_MatchesRowPath()
        {
            var nFirstFlagged = 0;
            var nWithVerses = 0;
            for (int i = 0; i < _storyRows.Count; i++)
            {
                var oldVerses = new VersesData(_storyRows[i], _ds);
                var newVerses = new VersesData(_storyElems[i]);
                AlignGeneratedFirstGuid(_storyElems[i], oldVerses, newVerses);
                Assert.That(newVerses.Count, Is.EqualTo(oldVerses.Count), $"story {i}");
                Assert.That(newVerses.FirstVerse.IsFirstVerse, Is.EqualTo(oldVerses.FirstVerse.IsFirstVerse), $"story {i}");
                Assert.That(newVerses.FirstVerse.guid, Is.EqualTo(oldVerses.FirstVerse.guid), $"story {i} first guid");
                Assert.That(newVerses.FirstVerse.GetXml.ToString(), Is.EqualTo(oldVerses.FirstVerse.GetXml.ToString()), $"story {i} first");
                if (_storyElems[i].Descendants("Verse").Any(v => (string)v.Attribute("first") == "true"))
                    nFirstFlagged++;
                for (int j = 0; j < oldVerses.Count; j++)
                {
                    Assert.That(newVerses[j].guid, Is.EqualTo(oldVerses[j].guid), $"story {i} verse {j}");
                    Assert.That(newVerses[j].IsFirstVerse, Is.False, $"story {i} verse {j}");
                    Assert.That(newVerses[j].IsVisible, Is.EqualTo(oldVerses[j].IsVisible), $"story {i} verse {j}");
                    Assert.That(newVerses[j].GetXml.ToString(), Is.EqualTo(oldVerses[j].GetXml.ToString()), $"story {i} verse {j}");
                }
                if (oldVerses.HasData)
                {
                    nWithVerses++;
                    Assert.That(newVerses.GetXml.ToString(), Is.EqualTo(oldVerses.GetXml.ToString()), $"story {i}");
                }
            }
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
            var oldVerses = new VersesData(_storyRows[1], _ds);
            var newVerses = new VersesData(_storyElems[1]);
            Assert.That(newVerses.Count, Is.EqualTo(0));
            Assert.That(oldVerses.Count, Is.EqualTo(0));
            Assert.That(newVerses.FirstVerse, Is.Not.Null);
            Assert.That(newVerses.FirstVerse.IsFirstVerse, Is.True);
            Assert.That(newVerses.FirstVerse.IsFirstVerse, Is.EqualTo(oldVerses.FirstVerse.IsFirstVerse));
        }

        [Test]
        public void VerseData_MatchesRowPath_ForEveryFixtureVerse()
        {
            // the earlier fixture has visible="1"/"false", first="true", padded and empty lines etc.
            ProjectReader.ReadProjectFile(ContentFixturePath, out var ds);
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var rows = ds.Verse.ToList();
            var elems = doc.Descendants("Verse").ToList();
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
            {
                var oldVerse = new VerseData(rows[i], ds);
                var newVerse = new VerseData(elems[i]);
                Assert.That(newVerse.guid, Is.EqualTo(oldVerse.guid), $"verse {i}");
                Assert.That(newVerse.IsFirstVerse, Is.EqualTo(oldVerse.IsFirstVerse), $"verse {i}");
                Assert.That(newVerse.IsVisible, Is.EqualTo(oldVerse.IsVisible), $"verse {i}");
                Assert.That(newVerse.GetXml.ToString(), Is.EqualTo(oldVerse.GetXml.ToString()), $"verse {i}");
            }
        }

        // ----- transitions -----

        [Test]
        public void StoryStateTransitionHistory_MatchesRowPath_DroppingDuplicates()
        {
            for (int i = 0; i < _storyRows.Count; i++)
            {
                var oldHistory = new StoryStateTransitionHistory(_storyRows[i]);
                var newHistory = new StoryStateTransitionHistory(_storyElems[i]);
                Assert.That(newHistory.Count, Is.EqualTo(oldHistory.Count), $"story {i}");
                for (int j = 0; j < oldHistory.Count; j++)
                {
                    Assert.That(newHistory[j].LoggedInMemberId, Is.EqualTo(oldHistory[j].LoggedInMemberId), $"story {i} t{j}");
                    Assert.That(newHistory[j].WindowsUserName, Is.EqualTo(oldHistory[j].WindowsUserName), $"story {i} t{j}");
                    Assert.That(newHistory[j].FromState, Is.EqualTo(oldHistory[j].FromState), $"story {i} t{j}");
                    Assert.That(newHistory[j].ToState, Is.EqualTo(oldHistory[j].ToState), $"story {i} t{j}");
                    Assert.That(newHistory[j].TransitionDateTime, Is.EqualTo(oldHistory[j].TransitionDateTime), $"story {i} t{j}");
                    Assert.That(newHistory[j].TransitionDateTime.Kind, Is.EqualTo(oldHistory[j].TransitionDateTime.Kind), $"story {i} t{j} kind");
                }
                Assert.That(newHistory.HasData, Is.EqualTo(oldHistory.HasData), $"story {i}");
                if (oldHistory.HasData)
                    Assert.That(newHistory.GetXml.ToString(), Is.EqualTo(oldHistory.GetXml.ToString()), $"story {i}");
            }

            // 4 elements, one an exact duplicate, so 3 survive
            Assert.That(new StoryStateTransitionHistory(_storyElems[0]).Count, Is.EqualTo(3));
        }

        [Test]
        public void StoryStateTransition_MatchesRowPath_ForEveryFixtureTransition()
        {
            ProjectReader.ReadProjectFile(ContentFixturePath, out var ds);
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var rows = ds.StateTransition.ToList();
            var elems = doc.Descendants("StateTransition").ToList();
            Assert.That(rows.Count, Is.GreaterThanOrEqualTo(3));
            for (int i = 0; i < rows.Count; i++)
            {
                var oldT = new StoryStateTransition(rows[i]);
                var newT = new StoryStateTransition(elems[i]);
                Assert.That(newT.GetXml.ToString(), Is.EqualTo(oldT.GetXml.ToString()), $"transition {i}");
                Assert.That(newT.TransitionDateTime, Is.EqualTo(oldT.TransitionDateTime), $"transition {i}");
                Assert.That(newT.TransitionDateTime.Kind, Is.EqualTo(oldT.TransitionDateTime.Kind), $"transition {i} kind");
            }
        }

        // ----- crafting info -----

        [Test]
        public void CraftingInfoData_MatchesRowPath()
        {
            var bSawNonBiblical = false;
            for (int i = 0; i < _storyRows.Count; i++)
            {
                var oldCi = new CraftingInfoData(_storyRows[i]);
                var newCi = new CraftingInfoData(_storyElems[i]);
                Assert.That(newCi.IsBiblicalStory, Is.EqualTo(oldCi.IsBiblicalStory), $"story {i}");
                bSawNonBiblical |= !oldCi.IsBiblicalStory;
                Assert.That(newCi.GetXml.ToString(), Is.EqualTo(oldCi.GetXml.ToString()), $"story {i}");
                AssertMember(newCi.StoryCrafter, oldCi.StoryCrafter, $"story {i} crafter");
                AssertMember(newCi.ProjectFacilitator, oldCi.ProjectFacilitator, $"story {i} pf");
                AssertMember(newCi.Consultant, oldCi.Consultant, $"story {i} consultant");
                AssertMember(newCi.Coach, oldCi.Coach, $"story {i} coach");
                AssertMember(newCi.BackTranslator, oldCi.BackTranslator, $"story {i} bt");
                AssertMember(newCi.OutsideEnglishBackTranslator, oldCi.OutsideEnglishBackTranslator, $"story {i} oebt");
                Assert.That(newCi.StoryPurpose, Is.EqualTo(oldCi.StoryPurpose), $"story {i} purpose");
                Assert.That(newCi.ResourcesUsed, Is.EqualTo(oldCi.ResourcesUsed), $"story {i} resources");
                Assert.That(newCi.MiscellaneousStoryInfo, Is.EqualTo(oldCi.MiscellaneousStoryInfo), $"story {i} misc");
                AssertTesters(newCi.TestersToCommentsRetellings, oldCi.TestersToCommentsRetellings, $"story {i} retellings");
                AssertTesters(newCi.TestersToCommentsTqAnswers, oldCi.TestersToCommentsTqAnswers, $"story {i} tq answers");
            }
            Assert.That(bSawNonBiblical, Is.True);
        }

        private static void AssertMember(MemberIdInfo actual, MemberIdInfo expected, string strMsg)
        {
            Assert.That(actual == null, Is.EqualTo(expected == null), strMsg);
            if (expected == null)
                return;
            Assert.That(actual.MemberId, Is.EqualTo(expected.MemberId), strMsg);
            Assert.That(actual.MemberComment, Is.EqualTo(expected.MemberComment), strMsg);
        }

        private static void AssertTesters(TestInfo actual, TestInfo expected, string strMsg)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), strMsg);
            for (int i = 0; i < expected.Count; i++)
                AssertMember(actual[i], expected[i], $"{strMsg} [{i}]");
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
        public void CraftingInfoData_MissingStoryCrafter_ThrowsSameAsRowPath()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\" /></story>";
            AssertBothPathsThrowSame(strStory, Properties.Resources.IDS_ProjectFileCorrupted);
        }

        [Test]
        public void CraftingInfoData_TwoStoryCrafters_ThrowsSameAsRowPath()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\">" +
                                    "<StoryCrafter memberID=\"m1\" /><StoryCrafter memberID=\"m1\" />" +
                                    "</CraftingInfo></story>";
            AssertBothPathsThrowSame(strStory, Properties.Resources.IDS_ProjectFileCorrupted);
        }

        [Test]
        public void CraftingInfoData_TwoCraftingInfoElements_ThrowsSameAsRowPath()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "</story>";
            AssertBothPathsThrowSame(strStory, Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo);
        }

        [Test]
        public void CraftingInfoData_TwoOptionalMembers_AreIgnoredLikeRowPath()
        {
            const string strStory = "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" />" +
                                    "<Consultant memberID=\"m2\">a</Consultant><Consultant memberID=\"m3\">b</Consultant>" +
                                    "<TestsRetellings><TestRetelling memberID=\"m2\">a</TestRetelling></TestsRetellings>" +
                                    "<TestsRetellings><TestRetelling memberID=\"m3\">b</TestRetelling></TestsRetellings>" +
                                    "</CraftingInfo></story>";
            using (var tmp = new TempProject(strStory))
            {
                ProjectReader.ReadProjectFile(tmp.Path, out var ds);
                var oldCi = new CraftingInfoData(ds.story.Single());
                var newCi = new CraftingInfoData(XDocument.Load(tmp.Path).Descendants("story").Single());
                Assert.That(newCi.Consultant, Is.Null);
                Assert.That(newCi.Consultant == null, Is.EqualTo(oldCi.Consultant == null));
                Assert.That(newCi.TestersToCommentsRetellings.Count, Is.EqualTo(oldCi.TestersToCommentsRetellings.Count));
                Assert.That(newCi.GetXml.ToString(), Is.EqualTo(oldCi.GetXml.ToString()));
            }
        }

        // The DataSet may reject the file in ReadXml or in the row constructor; either way the row path throws
        //  and the XElement path must throw the same message (when the row path's exception is the loader's own).
        private static void AssertBothPathsThrowSame(string strStoryXml, string strExpectedMessage)
        {
            using (var tmp = new TempProject(strStoryXml))
            {
                Exception exRow = null;
                try
                {
                    ProjectReader.ReadProjectFile(tmp.Path, out var ds);
                    new StoryData(ds.story.Single(), ds, TestDataDir);
                }
                catch (Exception ex)
                {
                    exRow = ex;
                }

                Assert.That(exRow, Is.Not.Null, "row path should throw");

                var elemStory = XDocument.Load(tmp.Path).Descendants("story").Single();
                var exNew = Assert.Throws<ApplicationException>(() => new StoryData(elemStory, TestDataDir));
                Assert.That(exNew.Message, Is.EqualTo(strExpectedMessage));
                if (exRow is ApplicationException)
                {
                    Assert.That(exNew.GetType(), Is.EqualTo(exRow.GetType()));
                    Assert.That(exNew.Message, Is.EqualTo(exRow.Message));
                }
                TestContext.WriteLine($"row path threw {exRow.GetType().Name}: {exRow.Message}");
            }
        }

        private sealed class TempProject : IDisposable
        {
            public string Path { get; }

            public TempProject(string strStoryXml)
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ose-story-" + Guid.NewGuid() + ".onestory");
                File.WriteAllText(Path,
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<StoryProject version=\"1.8\" ProjectName=\"t\">" +
                    "<Members HasOutsideEnglishBTer=\"false\" HasFirstPassMentor=\"false\" HasIndependentConsultant=\"false\"><Member name=\"c\" memberType=\"Crafter\" memberKey=\"m1\" /></Members>" +
                    "<Languages /><LnCNotes /><stories SetName=\"Stories\">" + strStoryXml + "</stories></StoryProject>");
            }

            public void Dispose()
            {
                try { File.Delete(Path); } catch { /* best effort */ }
            }
        }

        // ----- stories -----

        [Test]
        public void StoryData_MatchesRowPath()
        {
            for (int i = 0; i < _storyRows.Count; i++)
            {
                ProjectReader.UniqueStoryGuids.Clear();
                // the duplicate-guid story is covered separately (it needs the earlier story registered first)
                var oldStories = _storyRows.Take(i + 1).Select(r => new StoryData(r, _ds, TestDataDir)).ToList();
                ProjectReader.UniqueStoryGuids.Clear();
                var newStories = _storyElems.Take(i + 1).Select(e => new StoryData(e, TestDataDir)).ToList();
                var oldStory = oldStories.Last();
                var newStory = newStories.Last();

                Assert.That(newStory.Name, Is.EqualTo(oldStory.Name), $"story {i}");
                Assert.That(newStory.TasksAllowedPf, Is.EqualTo(oldStory.TasksAllowedPf), $"story {i}");
                Assert.That(newStory.TasksRequiredPf, Is.EqualTo(oldStory.TasksRequiredPf), $"story {i}");
                Assert.That(newStory.TasksAllowedCit, Is.EqualTo(oldStory.TasksAllowedCit), $"story {i}");
                Assert.That(newStory.TasksRequiredCit, Is.EqualTo(oldStory.TasksRequiredCit), $"story {i}");
                Assert.That(newStory.CountRetellingsTests, Is.EqualTo(oldStory.CountRetellingsTests), $"story {i}");
                Assert.That(newStory.CountTestingQuestionTests, Is.EqualTo(oldStory.CountTestingQuestionTests), $"story {i}");
                Assert.That(newStory.CraftingInfo.IsBiblicalStory, Is.EqualTo(oldStory.CraftingInfo.IsBiblicalStory), $"story {i}");

                if (_storyElems[i].Attribute("stageDateTimeStamp") == null)
                {
                    // no stamp: both paths use DateTime.Now
                    Assert.That((newStory.StageTimeStamp - oldStory.StageTimeStamp).Duration(), Is.LessThan(TimeSpan.FromSeconds(30)));
                    newStory.StageTimeStamp = oldStory.StageTimeStamp;
                }
                else
                {
                    Assert.That(newStory.StageTimeStamp, Is.EqualTo(oldStory.StageTimeStamp), $"story {i}");
                    Assert.That(newStory.StageTimeStamp.Kind, Is.EqualTo(oldStory.StageTimeStamp.Kind), $"story {i}");
                }

                AlignGeneratedFirstGuid(_storyElems[i], oldStory.Verses, newStory.Verses);
                if (i == 2)
                    continue;   // duplicate guid: the replacement guid is random (see the guid tests)

                Assert.That(newStory.guid, Is.EqualTo(oldStory.guid), $"story {i}");
                Assert.That(newStory.GetXml.ToString(), Is.EqualTo(oldStory.GetXml.ToString()), $"story {i}");
            }
        }

        [Test]
        public void StoryData_DuplicateGuid_BothPathsAssignNewGuidToTheDuplicate()
        {
            const string strOriginal = "00000000-0000-0000-0000-000000000101";

            ProjectReader.UniqueStoryGuids.Clear();
            var oldStories = _storyRows.Select(r => new StoryData(r, _ds, TestDataDir)).ToList();
            ProjectReader.UniqueStoryGuids.Clear();
            var newStories = _storyElems.Select(e => new StoryData(e, TestDataDir)).ToList();

            foreach (var stories in new[] { oldStories, newStories })
            {
                Assert.That(stories[0].guid, Is.EqualTo(strOriginal));
                Assert.That(stories[2].guid, Is.Not.EqualTo(strOriginal));
                Assert.That(Guid.TryParse(stories[2].guid, out _), Is.True);
                Assert.That(stories.Select(s => s.guid).Distinct().Count(), Is.EqualTo(stories.Count));
            }
            Assert.That(ProjectReader.UniqueStoryGuids, Does.Contain(newStories[2].guid));
        }

        [Test]
        public void StoryData_SameNameTwice_BothKept()
        {
            ProjectReader.UniqueStoryGuids.Clear();
            var oldStories = _storyRows.Select(r => new StoryData(r, _ds, TestDataDir)).ToList();
            ProjectReader.UniqueStoryGuids.Clear();
            var newStories = _storyElems.Select(e => new StoryData(e, TestDataDir)).ToList();
            Assert.That(newStories.Count(s => s.Name == "Story Minimal"), Is.EqualTo(2));
            Assert.That(newStories.Count(s => s.Name == "Story Minimal"), Is.EqualTo(oldStories.Count(s => s.Name == "Story Minimal")));
            Assert.That(newStories[1].guid, Is.Not.EqualTo(newStories[3].guid));
        }

        [Test]
        public void StoryData_ContentFixture_MatchesRowPath()
        {
            ProjectReader.ReadProjectFile(ContentFixturePath, out var ds);
            var doc = XDocument.Load(ContentFixturePath, LoadOptions.None);
            var rows = ds.story.ToList();
            var elems = doc.Descendants("story").ToList();
            ProjectReader.UniqueStoryGuids.Clear();
            var oldStories = rows.Select(r => new StoryData(r, ds, TestDataDir)).ToList();
            ProjectReader.UniqueStoryGuids.Clear();
            var newStories = elems.Select(e => new StoryData(e, TestDataDir)).ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                if (elems[i].Attribute("stageDateTimeStamp") == null)
                    newStories[i].StageTimeStamp = oldStories[i].StageTimeStamp;
                AlignGeneratedFirstGuid(elems[i], oldStories[i].Verses, newStories[i].Verses);
                Assert.That(newStories[i].GetXml.ToString(), Is.EqualTo(oldStories[i].GetXml.ToString()), $"story {i}");
            }
        }
    }
}
