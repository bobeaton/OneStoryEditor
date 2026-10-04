# Symmetric XML Loading (Sub-project E) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Load `.onestory` project files with one XElement-based loader (a `FromXml` constructor per `*Data` class, mirroring its `GetXml`). It replaces the typed ADO.NET DataSet path and the buggy `XmlNode` path, with identical loaded results.

**Architecture:** Build the new loader **alongside** the old one, then prove they agree:
- the old loader (DataSet rows) is the **oracle** in every test
- per-class tests compare `old.GetXml` with `new.GetXml`
- a differential corpus test does the same over all real projects

Then switch every caller over, replace the save-time reload check with XSD validation plus a new-loader load, and delete the DataSet, the row constructors, the `XmlNode` constructors, the 1.3/1.4 XSLT upgrade, and the obsolete `Sfm2OneStory`/`StoryEditorData` projects.

**Tech Stack:** C# / .NET Framework 4.8 (SDK-style, x86), LINQ to XML, `System.Xml.Schema` (XSD validation), NUnit (`StoryEditor.Tests`, 72 tests at start).

**Spec:** `docs/superpowers/specs/2026-10-04-symmetric-xml-loading-design.md`

## Global Constraints

- Branch `DecoupleWebBrowser`. Commit after each task. A commit trailer naming the model that actually wrote the commit is fine.
- **File format unchanged.** `GetXml` output must not change, except for the `KeyTermIds` fix in Task 6. The OneStory Chorus merge plugin and the released exe's typed DataSet read these files.
- **Save sequence unchanged:** temp `.bad` → reload check → backup → replace, with every `RobustFile` call as it is in `StoryEditor.SaveXElement`/`SaveFile`.
- **Semantics come from the row constructors**, including singleton rules, null guards, `NormalizeLineEndings`, `.ToLocalTime()`, duplicate handling and corruption messages. Never copy semantics from the `XmlNode` constructors; they have known bugs (spec, Context).
- **`StoryData.GetPresentationHtmlForChorus(XmlNode …)` keeps its exact signature.** Chorus calls it by reflection.
- **The old loader must keep working until Task 7.** Tasks 1–5 only add code; Task 6 switches callers over; Task 7 deletes.
- **Tests must never show UI.** Some row paths call `LocalizableMessageBox`, for example duplicate member names in `TeamMembersData`. Tests that would hit one must use data that avoids it. If the corpus run would show a box for a file, skip that file with a logged reason (Task 5).
- Edit with the Write/Edit tools, never shell heredocs/sed/python. In this environment escapes have been corrupted and sandboxed range-deletes refused. After writing C#, check that your added lines contain no unexpected non-ASCII. Use Bash only for git, MSBuild, vstest and grep.
- Large files (`StoryData.cs`, `StoryEditor.cs`, `VerseData.cs`, `ConsultNoteDataConverter.cs`): use Grep `-n`, and read only the regions you need.
- Build and test (Git Bash). If `output\Debug` is locked because the user is debugging, add `"-p:OutDir=<scratch folder>/"` and run vstest on `<scratch folder>/StoryEditor.Tests.dll`:
  ```bash
  MSBUILD="/c/Program Files/Microsoft Visual Studio/18/Insiders/MSBuild/Current/Bin/MSBuild.exe"
  VSTEST="/c/Program Files/Microsoft Visual Studio/18/Insiders/Common7/IDE/Extensions/TestPlatform/vstest.console.exe"
  "$MSBUILD" "StoryEditor 2017.sln" -t:StoryEditor_Tests -restore -p:Configuration=Debug -p:Platform=x86 -v:m -nologo
  "$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86
  ```
- **Naming.** New constructors take `XElement` and mirror the row constructor's **parent-element convention**: a collection constructor receives the element the row constructor's row stood for. For example, `AnchorsData(XElement elemVerse)` mirrors `AnchorsData(VerseRow, NewDataSet)`. This avoids ambiguity with the existing `XmlNode` constructors (different parameter type). Element and attribute names come from the existing `Cstr…` constants used by each `GetXml`; use them, not literals.

## Review Focus

These are the inputs most likely to differ between the old and new loaders. Each is pinned by a test in the task named:

1. **Dates.** `story@stageDateTimeStamp`, `StateTransition@TransitionDateTime` and `Comment@timeStamp`, written with and without `Z` or an offset, must give the same `DateTime` value **and `Kind`** as the DataSet, because the code calls `.ToLocalTime()` on them (Task 1, `XmlReadCharacterizationTests`).
2. **Empty, whitespace-only and missing text elements** (`<Retelling ... />`, `<StoryLine lang="x"></StoryLine>`, `<Anchor jumpTarget="x"> </Anchor>`) must give the same null/""/value as the DataSet. For example, the Anchor tooltip falls back to `JumpTarget` only on null (Task 1).
3. **XSD defaults:** `Verse@visible` absent → true; `Verse@first` and `*Conversation@finished` absent → false; story count attributes absent → 0 (Task 1).
4. **Booleans** written as `True`/`1`/`true` must give what the DataSet gives (Task 1).
5. **Files whose top-level parts appear 0 or 2+ times** (`CraftingInfo`, `StoryCrafter`, `TransitionHistory`, `Anchors`, …) must behave as the row path does: throw the same corruption message, or take the first/ignore. Task 2/3 per-class tests, Task 5 corpus.

---

### Task 1: `XmlRead` helpers, characterized against the DataSet

**Files:**
- Create: `StoryEditor/XmlRead.cs`
- Create: `StoryEditor.Tests/XmlReadCharacterizationTests.cs`
- Create: `StoryEditor.Tests/TestData/characterization.onestory` (synthetic; see Step 1)

**Interfaces:**
- Produces (`public static class OneStoryProjectEditor.XmlRead`):
  - `string Attr(XElement e, string name)`: attribute value or null.
  - `string RequiredAttr(XElement e, string name)`: value; if missing, throws `ApplicationException` with the message `$"The project file is damaged: <{e.Name}> is missing the required attribute '{name}'."`.
  - `bool? Bool(XElement e, string name)`, `bool Bool(XElement e, string name, bool defaultValue)`
  - `int Int(XElement e, string name, int defaultValue)`, `float? Float(XElement e, string name)` (invariant culture)
  - `DateTime? Date(XElement e, string name)`
  - `string Text(XElement e)`: the element's text exactly as the DataSet's `*_text` / simpleContent column gives it (null vs "" per characterization).
  - `XElement First(XElement parent, string name)`: first child of that name, or null.
  - `IEnumerable<XElement> Children(XElement parent, string name)`: document order.

**Method.** The tests use the **DataSet as the oracle**: load the same file both ways and assert the helper's result equals the DataSet column's value (and `DateTime.Kind`). There are no hard-coded expectations. Implement the helpers until those tests pass.

- [ ] **Step 1: Create the characterization fixture** `StoryEditor.Tests/TestData/characterization.onestory`. Copy the structure of `minimal-1.8.onestory` (same folder) and add stories and verses that cover:
  - `story@stageDateTimeStamp` as `2026-10-03T12:34:56Z`, `2026-10-03T12:34:56`, `2026-10-03T12:34:56+02:00` (three stories)
  - `StateTransition@TransitionDateTime` and `ConsultantNote@timeStamp` in the same three forms
  - verses with `visible` absent / `"false"` / `"True"` / `"1"`, and `first` absent / `"true"`
  - a `ConsultantConversation` with `finished` absent and one with `finished="true"`
  - stories with `CountRetellingsTests` absent and `"2"`
  - `StoryLine` elements that are self-closing, empty `<StoryLine lang="Vernacular"></StoryLine>`, whitespace-only `<StoryLine lang="Vernacular">  </StoryLine>`, and text with leading/trailing spaces
  - an `Anchor` with no text, and one with whitespace text
  - `LanguageInfo@FontSize="12.5"`
  - `Members@HasOutsideEnglishBTer="True"`

  Use the XSD (`StoryEditor/StoryProject.xsd`) to keep every required attribute present. Add a test that loads it with `ProjectReader.ReadProjectFile` first, to prove it's valid.

- [ ] **Step 2: Write the oracle tests** (`XmlReadCharacterizationTests.cs`). Pattern, repeated per case:
```csharp
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class XmlReadCharacterizationTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization.onestory");

        private ProjectReader _ds;
        private XDocument _doc;

        [OneTimeSetUp]
        public void Load()
        {
            ProjectReader.ReadProjectFile(FixturePath, out _ds);
            _doc = XDocument.Load(FixturePath, LoadOptions.None);
        }

        [Test]
        public void StoryTimeStamps_MatchDataSet_ValueAndKind()
        {
            var rows = _ds.story.ToList();
            var elems = _doc.Descendants("story").ToList();
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].IsstageDateTimeStampNull()) { Assert.That(XmlRead.Date(elems[i], "stageDateTimeStamp"), Is.Null); continue; }
                var expected = rows[i].stageDateTimeStamp;
                var actual = XmlRead.Date(elems[i], "stageDateTimeStamp").Value;
                Assert.That(actual, Is.EqualTo(expected), $"story {i}");
                Assert.That(actual.Kind, Is.EqualTo(expected.Kind), $"story {i} Kind");
                Assert.That(actual.ToLocalTime(), Is.EqualTo(expected.ToLocalTime()), $"story {i} local");
            }
        }
        // … same pattern for: StateTransition@TransitionDateTime, Comment@timeStamp (ConsultantNote rows),
        //   Verse@visible/@first (incl. defaults), ConsultantConversation@finished, story Count* defaults,
        //   StoryLine_text (null vs "" vs whitespace preserved), Anchor_text, LanguageInfo@FontSize,
        //   Members@Has* booleans.
    }
}
```
  Write one test per case listed in Step 1. In each, compare the typed DataSet property (using `Is…Null()` where the row has it) with the helper's result. The typed property names are in `StoryEditor/StoryProject.Designer.cs`; Grep for e.g. `stageDateTimeStamp` to find them.

- [ ] **Step 3: Run the tests.** Expected: compile error, `XmlRead` doesn't exist.

- [ ] **Step 4: Implement `XmlRead`** to make every oracle test pass. Guidance (verify each against the tests; the tests are authoritative):
  - Booleans: the DataSet uses `XmlConvert.ToBoolean` semantics. If `"True"` turns out to be rejected by the DataSet, the test shows it; then match the DataSet, including throwing.
  - Dates: the DataSet uses `XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.…)` with the column's `DateTimeMode` (`UnspecifiedLocal`). Find the mode that reproduces both value and `Kind` for all three string forms.
  - Floats and ints: `XmlConvert.ToSingle` / `XmlConvert.ToInt32`, which are invariant.
  - Text: compare `XElement.Value` against the DataSet for self-closing, empty, whitespace-only and padded text, and pick the mapping the tests demand.

- [ ] **Step 5: Run the full suite.** Expected: all pass. **Commit:** `"Add XmlRead helpers characterized against the DataSet loader"`

---

### Task 2: `FromXml` for verse content (anchors, exegetical notes, test questions, retellings/answers, notes)

**Files:**
- Modify: `StoryEditor/AnchorsData.cs`, `StoryEditor/ExegeticalHelpNotesData.cs`, `StoryEditor/TestQuestionsData.cs`, `StoryEditor/MultipleLineDataConverter.cs`, `StoryEditor/ConsultNoteDataConverter.cs`
- Create: `StoryEditor.Tests/FromXmlVerseContentTests.cs`
- Modify (extend): `StoryEditor.Tests/TestData/characterization.onestory`. Ensure it contains:
  - anchors with and without `keyTermChecked`
  - exegetical helps, including a duplicate
  - test questions with answers
  - retellings
  - consultant and coach conversations with `visible`/`finished`, a ReferringToText comment, a StickyNote, and a mentor comment without `memberID`

**Interfaces:**
- Consumes: `XmlRead.*` (Task 1).
- Produces these new constructors. Each mirrors the row constructor listed beside it **line by line**: same fields set, same defaults, same normalization, same handling of missing parent containers. Where the row constructor *adds* an empty container row (e.g. `AddAnchorsRow`), just treat the container as absent.

| New | Mirrors |
|---|---|
| `AnchorData(XElement elemAnchor)` | `AnchorData(NewDataSet.AnchorRow)` (AnchorsData.cs:16) |
| `AnchorsData(XElement elemVerse)` | `AnchorsData(VerseRow, NewDataSet)` (:290) |
| `ExegeticalHelpNoteData(XElement elemExegeticalHelp)` | `(ExegeticalHelpRow)` (ExegeticalHelpNotesData.cs:15) |
| `ExegeticalHelpNotesData(XElement elemVerse)` | `(VerseRow, NewDataSet)` (:47) |
| `TestQuestionData(XElement elemTestQuestion)` | `(TestQuestionRow, NewDataSet)` (TestQuestionsData.cs:18) |
| `TestQuestionsData(XElement elemVerse)` | `(VerseRow, NewDataSet)` (:388) |
| `RetellingsData(XElement elemVerse)` | `(VerseRow, NewDataSet)` (MultipleLineDataConverter.cs:436) |
| `AnswersData(XElement elemTestQuestion)` | `(TestQuestionRow, NewDataSet)` (:520) |
| `ConsultantNoteData(XElement elemConversation)` | `(ConsultantConversationRow)` (ConsultNoteDataConverter.cs:1382) |
| `CoachNoteData(XElement elemConversation)` | `(CoachConversationRow)` (:1558) |
| `ConsultantNotesData(XElement elemVerse)` | `(VerseRow, NewDataSet)` (:1880) |
| `CoachNotesData(XElement elemVerse)` | `(VerseRow, NewDataSet)` (:1981) |

Shared base logic (e.g. in `ConsultNoteDataConverter`, `MultipleLineDataConverter`, `CommInstance`) may get a protected XElement-based initializer if the row constructors share one. Mirror whatever sharing the row path has.

**Worked example (the pattern for every class):**
```csharp
        // mirrors AnchorData(NewDataSet.AnchorRow)
        public AnchorData(XElement elemAnchor)
        {
            JumpTarget = XmlRead.RequiredAttr(elemAnchor, CstrAttributeJumpTarget);
            var strText = XmlRead.Text(elemAnchor);
            ToolTipText = (strText == null) ? JumpTarget : strText;
        }
```
```csharp
        // mirrors AnchorsData(NewDataSet.VerseRow, NewDataSet)
        public AnchorsData(XElement elemVerse)
        {
            var elemAnchors = XmlRead.First(elemVerse, CstrElementLabelAnchors);
            if (elemAnchors == null)
                return;

            IsKeyTermChecked = XmlRead.Bool(elemAnchors, "keyTermChecked", false);
            foreach (var elemAnchor in XmlRead.Children(elemAnchors, AnchorData.CstrElementLabelAnchor))
                Add(new AnchorData(elemAnchor));
        }
```
(Check that `IsKeyTermChecked`'s row-path default, used when the attribute is null, is `false`, as the row constructor shows.)

- [ ] **Step 1: Write the oracle tests** (`FromXmlVerseContentTests.cs`). For each class: load the fixture as a DataSet and as an `XDocument`; for every verse, build the object both ways and assert `old.GetXml.ToString() == new.GetXml.ToString()`. Where `GetXml` asserts `HasData`, compare only objects that have data, and also compare `Count`. Also assert directly on fields the XML doesn't show (e.g. ConsultNote `IsFinished`, `Visible`, `ReferringText`). Example:
```csharp
        [Test]
        public void AnchorsData_MatchesRowPath()
        {
            var verseRows = _ds.Verse.ToList();
            var verseElems = _doc.Descendants("Verse").ToList();
            Assert.That(verseElems.Count, Is.EqualTo(verseRows.Count));
            for (int i = 0; i < verseRows.Count; i++)
            {
                var oldAnchors = new AnchorsData(verseRows[i], _ds);
                var newAnchors = new AnchorsData(verseElems[i]);
                Assert.That(newAnchors.Count, Is.EqualTo(oldAnchors.Count), $"verse {i}");
                Assert.That(newAnchors.IsKeyTermChecked, Is.EqualTo(oldAnchors.IsKeyTermChecked), $"verse {i}");
                if (oldAnchors.HasData)
                    Assert.That(newAnchors.GetXml.ToString(), Is.EqualTo(oldAnchors.GetXml.ToString()), $"verse {i}");
            }
        }
```
  Note: the row constructors mutate the DataSet (add empty container rows). Load a **fresh** `ProjectReader` per test, so one test's mutations don't affect another.

- [ ] **Step 2: Build.** Expected: compile errors for the missing constructors.
- [ ] **Step 3: Implement the constructors** per the table, mirroring each row constructor.
- [ ] **Step 4: Run the tests** until all pass, then the full suite. **Commit:** `"Add XElement FromXml constructors for verse content (anchors, exegetical notes, TQs, retellings/answers, notes)"`

---

### Task 3: `FromXml` for verse, line and story level

**Files:**
- Modify: `StoryEditor/VerseData.cs`, `StoryEditor/StoryData.cs` (StoryData, StoryStateTransitionHistory, StoryStateTransition, CraftingInfoData, MemberIdInfo/TestInfo as used by CraftingInfoData)
- Create: `StoryEditor.Tests/FromXmlStoryTests.cs`
- Extend the fixture with:
  - a story with `NonBiblicalStory="true"`
  - CraftingInfo with every member kind (PF, Consultant, Coach, BT, OutsideEnglishBT) plus TestsRetellings/TestsTqAnswers including comments
  - a TransitionHistory with a duplicate transition
  - a verse with `first="true"`
  - a story with a duplicate guid of another story, and two stories with the same name

**Interfaces:**
- Consumes: Task 2's constructors and `XmlRead`.
- Produces:

| New | Mirrors |
|---|---|
| `VerseData(XElement elemVerse)` | `VerseData(VerseRow, NewDataSet)` (VerseData.cs:223) |
| `VersesData(XElement elemStory)` | `VersesData(storyRow, NewDataSet)` (VerseData.cs:1427), including `AdjustmentForFirstVerse` |
| `StoryStateTransition(XElement elemStateTransition)` | `(StateTransitionRow)` (StoryData.cs:1053) |
| `StoryStateTransitionHistory(XElement elemStory)` | `(storyRow)` (StoryData.cs:958), including silently dropping duplicates |
| `CraftingInfoData(XElement elemStory)` | `(storyRow)` (StoryData.cs:1406), including the exactly-1 checks for CraftingInfo/StoryCrafter that throw the existing `IDS_ProjectFileCorrupted…` messages, and the 0/2+ handling of the others |
| `StoryData(XElement elemStory, string strProjectFolder)` | `(storyRow, NewDataSet, string)` (StoryData.cs:125), including the `UniqueStoryGuids` duplicate-guid replacement |

- [ ] **Step 1: Oracle tests** (same pattern as Task 2) for `VersesData` (compare `GetXml` and `FirstVerse`), `StoryStateTransitionHistory`, `CraftingInfoData` (`GetXml` + `IsBiblicalStory`), and `StoryData` (whole-story `GetXml`). For the guid test, clear `ProjectReader.UniqueStoryGuids` before each pass, and assert both paths assign a **new** guid to the duplicate. Compare that the guids differ from the original; don't compare their values, which are random. Add a test that a story with 2 `CraftingInfo` elements throws the same exception type and message on both paths.
- [ ] **Step 2: Build** (expected compile errors) → **Step 3: implement** → **Step 4: tests pass, full suite.** **Commit:** `"Add XElement FromXml constructors for verses, lines, transitions, crafting info and stories"`

---

### Task 4: `FromXml` for project level + `ProjectFile` loader

**Files:**
- Modify: `StoryEditor/StoryData.cs` (StoriesData, StoryProjectData), `StoryEditor/TeamMemberData.cs`, `StoryEditor/ProjectSettings.cs` (SerializeProjectSettings, AdaptItConfiguration, LanguageInfo), `StoryEditor/LnCNotesData.cs`, `StoryEditor/LegacyTextRepair.cs`
- Create: `StoryEditor/ProjectFile.cs`
- Create: `StoryEditor.Tests/FromXmlProjectTests.cs`, `StoryEditor.Tests/ProjectFileTests.cs`
- Extend the fixture with:
  - Members with `Has*` flags and member attributes (including `OverrideFontSize*`, `TransliteratorDirectionForward*`)
  - an AdaptItConfiguration
  - two LnCNotes, one with renderings and `KeyTermIds`
  - the three story sets

**Interfaces:**
- Consumes: Tasks 1–3.
- Produces:

| New | Mirrors |
|---|---|
| `StoriesData(XElement elemStories, string strProjectFolder)` | `(storiesRow, NewDataSet, string)` (StoryData.cs:1782), including the duplicate-name renaming |
| `TeamMemberData(XElement elemMember)` | `(MemberRow)` (TeamMemberData.cs:286), every attribute, including decrypting `HgPassword` |
| `TeamMembersData(XElement elemStoryProject)` | `(NewDataSet)` (:877), including the duplicate-name handling (it shows a message box; tests must not hit it) |
| `ProjectSettings.SerializeProjectSettings(XElement elemStoryProject)` | the `(NewDataSet)` overload (ProjectSettings.cs:88) |
| `AdaptItConfiguration.SerializeFromProjectFile(XElement elemAdaptItConfiguration)` | `(AdaptItConfigurationRow)` (:206) |
| `LanguageInfo.Serialize(XElement elemLanguageInfo)` | `(LanguageInfoRow)` (:453) |
| `LnCNote(XElement elemLnCNote)` | `(LnCNoteRow)` (LnCNotesData.cs:268); read `KeyTermIds` exactly as the row path does (the `KeyTermId` fallback comes in Task 6) |
| `LnCNotesData(XElement elemStoryProject)` | `(NewDataSet)` (:19) |
| `StoryProjectData(XElement elemStoryProject, bool bIsPlainTextEncoded, ProjectSettings projSettings)` | `(NewDataSet, ProjectSettings)` (StoryData.cs:1993), every post-load fix-up (see below) |
| `LegacyTextRepair.ClearLanguageNamePlaceholders(XElement elemStoryProject)` | the DataSet overload: same tables and fields, reading `LanguageInfo` elements |
| `ProjectFile.Load(string strPath)` → `ProjectFileContents` | `ProjectReader.ReadProjectFile` + version gate (below) |

`ProjectFileContents`: `public XElement Root { get; }`, `public bool IsPlainTextEncoded { get; }`, `public DateTime LastWriteTime { get; }`.

`StoryProjectData(XElement, bool, ProjectSettings)` must reproduce, in the same order as the row constructor:
- overwriting `ProjectName` from settings
- Dropbox flags and PanoramaFrontMatter
- the missing-story-set additions
- `TeamMembersData`, `ProjectSettings.SerializeProjectSettings`, `LnCNotesData`, the `StoriesData` per set, `CheckForCommentMemberIds` for 1.5, and `LoadOsMetaData`
- `LegacyTextRepair.DecodePlainTextElements(elemStoryProject)` when `!bIsPlainTextEncoded`, and `ClearLanguageNamePlaceholders(elemStoryProject)` always, **both before** any child is built

Leave out the 1.3/1.4 branches and the newer-version check: both move to `ProjectFile.Load`. The row constructor's newer-version check shows a message box; `ProjectFile` throws instead, and `OpenProject`'s catch shows the message, so tests stay UI-free.

`ProjectFile.Load`:
- `XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore })` → `XDocument.Load(reader)`
- clears `ProjectReader.UniqueStoryGuids` (later Task 7 moves that list to `ProjectFile`; for now use the existing one)
- `IsPlainTextEncoded = LegacyTextRepair.IsMarkedPlain(root)`; `LastWriteTime = File.GetLastWriteTime(path)`
- if `version` is `"1.3"` or `"1.4"`: throw `ApplicationException` with: `"This project was saved by a very old version of OneStory Editor. Open and save it with OneStory Editor 4.x first."`
- if `version` is newer than `StoryProjectData.XmlDataVersion` ("1.6") and is not `"1.7"` or `"1.8"` (the row constructor's rule, StoryData.cs ~2039-2047): throw `ApplicationException` carrying exactly the existing localized "One of the team members is using a newer version of OSE…" text (`Localizer.Str(...)` with the same string)

- [ ] **Step 1: Oracle tests:**
  - `StoriesData` (`GetXml` per set)
  - `TeamMembersData`/`TeamMemberData` (`GetXml` + `Has*` flags)
  - `ProjectSettings` (`GetXml` of Languages + AdaptIt config + every `Use*`/`Show*` flag the row path reads)
  - `LnCNotesData` (`GetXml`)
  - `ClearLanguageNamePlaceholders` (XElement) giving the same result as the DataSet overload
  - `ProjectFileTests`:
    - a 1.4 file throws with the message
    - a 2.0 file throws the newer-version message
    - a DOCTYPE file loads
    - the marker is read
    - 1.6, 1.7 and 1.8 load

  `StoryProjectData` needs a `ProjectSettings` and does file IO (`LoadOsMetaData`). Create the settings the way `StoryEditor.OpenProject` does, pointing at a temp project folder created by the test (copy the fixture into `<temp>/<name>/<name>.onestory`). If that turns out to need UI or machine state, test the children instead, and record why in the report.
- [ ] **Step 2: Build** → **Step 3: implement** → **Step 4: tests pass, full suite.** **Commit:** `"Add XElement FromXml for project level and ProjectFile loader"`

---

### Task 5: Differential corpus test (old vs new) to zero differences

**Files:**
- Create: `StoryEditor.Tests/DifferentialCorpusTests.cs`
- Modify: whichever `FromXml` code the differences point at

**Interfaces:**
- Consumes: everything in Tasks 1–4.

- [ ] **Step 1: Write the `[Explicit]` test.** It uses the same corpus discovery as `CorpusTests` (`OSE_CORPUS_DIRS`, or `Documents\OneStory Editor Projects` + `C:\btmp\OSE Projects`). For each readable `*.onestory`:
  - **Old:** `ProjectReader.ReadProjectFile` → for each `storiesRow`: `new StoriesData(row, ds, folder).GetXml`; plus `new TeamMembersData(ds).GetXml`, `new LnCNotesData(ds).GetXml`, and the ProjectSettings XML via a settings object filled by `SerializeProjectSettings(ds)`. Use the same folder argument as Task 4.
  - **New:** `ProjectFile.Load` → the same objects from XElements.
  - Run `LegacyTextRepair` identically on both sides first: decode unless marked; placeholders always. Old side uses the DataSet overloads, new side the XElement ones.
  - Compare `ToString()` of each pair. On the first difference per file, log the file, the object, and the first differing line with ±3 lines of context, then continue to the next file. At the end, assert that the list of differing files is empty.
  - Count the files skipped because they're unreadable (expected: the zero-byte ones) or would show UI (log the reason), and print `files / compared / skipped / differing`.
- [ ] **Step 2: Run it.** For every difference, fix the **new** code to match the row path. Never change the row path or `GetXml`. Re-run until `differing = 0`.
- [ ] **Step 3: Commit:** `"Differential corpus test: new XElement loader matches the DataSet loader on all projects"`. Include the final summary line in the commit message body.

---

### Task 6: Switch over; save-time validation; version gate; `KeyTermIds` and `SaveDoc` fixes

**Files:**
- Modify: `StoryEditor/StoryEditor.cs` (OpenProject, GetOldStoryProjectData, SaveXElement, the `Backout2ReOpenException` handling, story/column paste), `StoryEditor/PanoramaView.cs` (PasteStoryCopy), `StoryEditor/HtmlDisplayForm.cs` (RevisionHistoryForm), `StoryEditor/StoryData.cs` (GetPresentationHtmlForChorus; remove the 1.3/1.4 conversion branches and methods), `StoryEditor/LnCNotesData.cs`, `OseCommon/OseXmlSerializer.cs`
- Create: `StoryEditor/ProjectFileValidator.cs`
- Create: `StoryEditor.Tests/SaveGuardTests.cs`, extend `ProjectFileTests.cs`

**Interfaces:**
- Consumes: `ProjectFile.Load`, `StoryProjectData(XElement, bool, ProjectSettings)`, `StoryData(XElement, string)` (Tasks 3–4).
- Produces: `ProjectFileValidator.Validate(string strPath)`. It throws `XmlSchemaValidationException` (or `ApplicationException` wrapping it) when the file doesn't satisfy `StoryProject.xsd`. The schema is loaded from the XSD **embedded as a resource**: add `<EmbeddedResource Include="StoryProject.xsd" LogicalName="OneStoryProjectEditor.StoryProject.xsd" />` to `StoryEditor.csproj`.

- [ ] **Step 1: Tests first:**
  - `SaveGuardTests`:
    - (a) a temp file missing `story@guid` fails `ProjectFileValidator.Validate`
    - (b) the fixture passes
    - (c) a file written by `StoryProjectData.GetXml` passes, **and** loads with `ProjectReader.ReadProjectFile` (the released exe's DataSet; still present until Task 7)
  - `ProjectFileTests`: an `LnCNote` with only the old `KeyTermId` attribute loads its key terms (fallback); `GetXml` now writes `KeyTermIds`.
  - A test that `OseXmlSerializer.SaveDoc` throws when the target can't be written (e.g. the target path is an existing directory).
  - **Parity of the former `XmlNode` users:** for each story in the fixture, `StoryData` built the way paste builds it (from the `XElement`, after `DecodeUnlessMarked`) has the same `GetXml` as the normal load. `GetPresentationHtmlForChorus` is called with the story's `XmlNode` (from an `XmlDocument` of the fixture) and returns non-empty HTML without throwing. This documents that the old `XmlNode`-path bugs are gone, e.g. a finished conversation stays finished and transition history isn't empty.
- [ ] **Step 2: Implement:**
  - **`OpenProject`:** `var contents = ProjectFile.Load(path)` → `new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, projSettings)`; `_dateTimeLastSaved = contents.LastWriteTime`. Keep the existing catch blocks and messages. Delete the `Backout2ReOpenException` catch and the 1.3/1.4 conversion code (`ConvertProjectFile1_3_to_1_4`, `ConvertProjectFile1_4_to_1_5`, `TransformedXmlDataToSfm` if unused, `Backout2ReOpenException`, and their XSLT resources in `Properties/Resources.resx`; check each with Grep before deleting).
  - **`SaveXElement`'s reload check:** replace `ProjectReader.ReadProjectFile(strTempFilename, out projFile)` with `ProjectFileValidator.Validate(strTempFilename); ProjectFile.Load(strTempFilename);`. Everything else in the method stays byte for byte.
  - **Paste** (StoryEditor story paste, PanoramaView.PasteStoryCopy): build `StoryData` from the `XElement` directly: `new StoryData(theStoryToCopyXElement, folder)`. Drop the `GetXmlNode()` round trip. Keep the `LegacyTextRepair.DecodeUnlessMarked` call and the second copy-constructor pass that regenerates guids.
  - **`RevisionHistoryForm`:** load revisions with `XDocument` and construct `StoryData` from the story `XElement`. Keep the marker-gated decode, using the XElement version.
  - **`GetPresentationHtmlForChorus(XmlNode …)`:** same signature. Convert internally with `XElement.Parse(node.OuterXml)`, decode, and construct from the XElement.
  - **`LnCNote`:** `GetXml` writes `KeyTermIds`; the XElement constructor reads `KeyTermIds`, else falls back to `KeyTermId`.
  - **`OseXmlSerializer.SaveDoc`:** make a persistent failure throw. Fix the unreachable guard so that, after the last attempt, it throws an `IOException` carrying the last error. Keep the read-only-clearing retry.
- [ ] **Step 3: Full suite green** (the differential test is `[Explicit]`). Re-run the differential corpus test once (it should still be 0, apart from `KeyTermIds`; adjust its comparison to normalize `KeyTermId`→`KeyTermIds` on the old side), and record the summary in the report.
- [ ] **Step 4: Commit:** `"Switch project loading, paste, revision history and Chorus view to the XElement loader; XSD-validated save check; fix KeyTermIds and SaveDoc"`

---

### Task 7: Delete the old loaders and obsolete projects

**Files:**
- Delete: `StoryEditor/StoryProject.Designer.cs` (**move** a copy to `StoryEditor.Tests/ReleasedExeDataSet/StoryProject.Designer.cs`; see Step 1), every DataSet-row constructor listed in Tasks 2–4 plus `ProjectSettings.InsureLanguagesRow`, every `XmlNode` constructor of the `*Data` classes (`StoryData(XmlNode,…)`, `CraftingInfoData(XmlNode)`, `VersesData(XmlNode)`, `VerseData(XmlNode)`, `LineData(XmlNode,…)`, `AnchorsData(XmlNode)`, `AnchorData(XmlNode)`, `ExegeticalHelpNotesData(XmlNode)`, `TestQuestion*(XmlNode)`, `MultipleLineDataConverter.InitFromXmlNode` and its callers, `ConsultNoteDataConverter(XmlNode)` and subclasses, `CommInstance(XmlNode)`, `StoryStateTransition*(XmlNode)`, `MemberIdInfo.CreateFromXmlNode`, `TestInfo.Add(XmlNode…)`, `TeamMember*(XmlNode)`, `ProjectSettings(XmlNode, …)`, `LanguageInfo(XmlNode)`), `ProjectReader`, the DataSet and `XmlNode` overloads in `LegacyTextRepair`, the `GetXmlNode()` extension if now unused, the directories `Sfm2OneStory/` and `StoryEditorData/`, and `DifferentialCorpusTests.cs`.
- Modify: `StoryEditor.Tests/CorpusTests.cs`, `LegacyTextRepairTests.cs`, `ProjectReaderMarkerTests.cs` (→ `ProjectFile`), and the Task 1–4 oracle tests (see Step 1).
- Keep: `StoryEditor/StoryProject.xsd` (now the embedded validation schema).

- [ ] **Step 1: The released-exe compatibility check moves into the tests.**
  - Move `StoryProject.Designer.cs` into `StoryEditor.Tests/ReleasedExeDataSet/`. Keep its namespace, or change it to `OneStoryProjectEditor.Tests.ReleasedExeDataSet` and fix the usings; it must compile inside the test project.
  - Port `SaveGuardTests` (c) to load written files with that test-only `NewDataSet.ReadXml`.
  - The oracle tests from Tasks 1–4 compared old row constructors with new; they can't survive the deletion of the row constructors. Convert each into a **golden** test: assert the new loader's `GetXml` equals the fixture's own element, normalized the way `GetXml` writes it. Where a test asserted a field (`IsFinished`, `FirstVerse`, `IsBiblicalStory`, guid replacement, duplicate renaming, corruption messages), keep the assertion with the value it had. Keep the `XmlRead` date, text, default and boolean tests as **hard-coded expectations**, using the values the DataSet produced in Task 1. Read them from the Task 1 test run or the Task 1 report.
- [ ] **Step 2: Delete** everything in the list. Build, and fix every compile error by switching to the XElement path. Never reintroduce a DataSet or `XmlNode` constructor. Then confirm:
  ```bash
  grep -rn "NewDataSet\|ProjectReader\|XmlNode node\|GetXmlNode\|Backout2ReOpen\|ConvertProjectFile1_" StoryEditor --include=*.cs
  ```
  Expected: no hits, except `GetPresentationHtmlForChorus(XmlNode …)` and its `XElement.Parse(node.OuterXml)` conversion.
- [ ] **Step 3: Full suite green.** Run the (ported) `CorpusTests` explicitly once and record its summary.
- [ ] **Step 4: Commit:** `"Remove the DataSet and XmlNode loaders, the 1.3/1.4 upgrade, and the obsolete Sfm2OneStory/StoryEditorData projects"`

- [ ] **Step 5: Manual verification (for the human, after the final review):**
  1. Open, edit, save and reopen several real projects (copies).
  2. Paste a story from another project.
  3. Revision history of a story.
  4. The Chorus change view (send/receive with a change).
  5. An L&C note with key terms survives save/reopen.
  6. Open the saved file in the released OSE: it loads.
  7. Make the project file read-only and save: a clear error appears and the original is untouched.
