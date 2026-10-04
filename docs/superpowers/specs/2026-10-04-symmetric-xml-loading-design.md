# Sub-project E: Symmetric XML loading — Design

Status: approved in conversation · Branch: `DecoupleWebBrowser` (after A and R)

## Context and goal

`.onestory` project files are **saved** by hand-written LINQ to XML (`GetXml` on each `*Data` class),
but **loaded** in two other ways:

1. **Typed ADO.NET DataSet** (`ProjectReader : NewDataSet`, `ReadXml`): generated from
   `StoryProject.xsd` into the 24k-line `StoryProject.Designer.cs`. The `*Data` constructors read typed
   `DataRow`s (~1,000 lines across 14 files). This is used to open projects and for the save-time reload
   check.
2. **XmlNode constructors** on most `*Data` classes. Used by paste-story/column, revision history
   (`RevisionHistoryForm`) and the Chorus change presenter (`StoryData.GetPresentationHtmlForChorus`,
   called by reflection). This path has known bugs:
   - `ConsultNoteDataConverter`'s `IsFinished` is read from `visible`.
   - The state-transition history is always empty (wrong relative XPath).
   - `NonBiblicalStory` and `keyTermChecked` are ignored; the Anchor tooltip has no `JumpTarget` fallback.
   - ReferringText comments land in the comment list.
   - Null references on a missing `memberID`, `stage`, `guid` or `stageDateTimeStamp`.
   - Booleans only accept `"true"`.
   - Dates and floats are parsed with the current culture.
   - Team members' `Has*` flags are skipped, and a duplicate member name throws.

**Goal:** one loader. Each `*Data` class reads its own `XElement` with a `FromXml` that mirrors its
`GetXml`. This replaces both the DataSet path and the XmlNode path, so both move off the
DataSet/`XmlNode` world ahead of .NET 8 (sub-project D).

**Success:**
- For every project in the corpus, loading with the new loader and saving produces exactly what loading
  with the old loader and saving produces (differential corpus test).
- Files we write remain loadable by the released exe.
- The save sequence and its protections are unchanged.

## Binding constraints

- **File format unchanged.** It is still read by the OneStory Chorus merge plugin and by the released
  exe's typed DataSet. The only intended format change is the `KeyTermIds` fix below.
- **Save sequence unchanged:** write temp (`.bad`) → reload check → backup → replace, with all
  `RobustFile` calls as they are.
- **Reload check keeps its guarantees.** The temp file is **validated against `StoryProject.xsd`**
  (this replaces the DataSet's required-attribute/constraint checks, so the released exe can still read
  it) and then **loaded with the new loader**.
- **`GetPresentationHtmlForChorus` keeps its exact signature** (Chorus calls it by reflection).
- **Kept behaviours** (re-implemented on XElement, same semantics):
  - duplicate story names renamed `name.1`, `name.2`, …
  - duplicate story guids replaced (`UniqueStoryGuids`)
  - duplicate state transitions silently dropped on load
  - `VersesData.AdjustmentForFirstVerse`
  - missing story sets added (Main/Obsolete; NonBib when there are exactly 2)
  - `ProjectName` overwritten from settings
  - `CheckForCommentMemberIds` for version 1.5
  - `LoadOsMetaData`
  - `CraftingInfo`/`StoryCrafter` count checks with their existing corruption messages
  - `LegacyTextRepair` entity decode (gated by the marker) and language-placeholder clearing (always)
  - `NormalizeLineEndings` everywhere it is applied today
- **Version handling:** read `StoryProject@version` before anything else.
  - **1.3 / 1.4 are refused** with a message (user decision): "This project was saved by a very old
    version of OneStory Editor. Open and save it with OneStory Editor 4.x first."
  - The XSLT conversions (`ConvertProjectFile1_3_to_1_4`, `ConvertProjectFile1_4_to_1_5`, their
    resources and `Backout2ReOpenException` handling) are deleted.
  - Newer-than-supported versions are refused with the existing message (1.7 and 1.8 accepted).

## Components

### 1. `ProjectFile` (new, replaces `ProjectReader`)
`ProjectFile.Load(string path) → ProjectFileContents` (root `XElement`, `IsPlainTextEncoded`, last-write
time). It:
- reads with `XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }`; the XML parser normalizes
  CRLF exactly as before
- does the version gate
- reads the `TextEncoding` marker from the same document (no second file read)
- clears `UniqueStoryGuids`

Errors propagate as today: `OpenProject`'s and `SaveFile`'s existing catch blocks show them.

### 2. `XmlRead` helpers (new, static)
Each one replicates the DataSet's implicit behaviour, and each is pinned by characterization tests run
against the old loader:
- **booleans:** `xs:boolean` semantics (`true`/`false`/`1`/`0`, case as the DataSet accepted)
- **numbers:** invariant-culture float/int
- **dates:** `xs:dateTime` → the same `DateTime` value and `Kind` the DataSet produced, so the existing
  `.ToLocalTime()` calls behave identically, with and without `Z`/offset
- **text:** empty or whitespace-only text element → the same null/"" result as the DataSet
- **defaults:** the 7 XSD defaults (`Verse@first`=false, `Verse@visible`=true, `*Conversation@finished`=false,
  `story@CountRetellingsTests`/`CountTestingQuestionTests`=0)
- **required attributes** (`use="required"` in the XSD): missing → an `ApplicationException` naming the
  element and attribute. Today `ReadXml` throws a constraint exception there, and `OpenProject` shows
  it, so opening still fails with a message, now a clearer one.

### 3. `FromXml` per class
A constructor or static factory next to each `GetXml`, reading exactly what `GetXml` writes:
- **Project level:** `StoryProjectData`, `StoriesData`, `TeamMembersData`/`TeamMemberData` (all
  attributes the row path read, including the decrypted `HgPassword`), `ProjectSettings`
  (Languages + `Show*`/`Use*` flags, `LanguageInfo`, `AdaptItConfiguration`), `LnCNotesData`/`LnCNote`.
- **Story level:** `StoryData`, `CraftingInfoData` (+`MemberIdInfo`, `TestInfo`),
  `StoryStateTransitionHistory`/`StoryStateTransition`, `VersesData`/`VerseData`/`LineData`.
- **Verse content:** `AnchorsData`/`AnchorData`, `ExegeticalHelpNotesData`/`…NoteData`,
  `TestQuestionsData`/`TestQuestionData`, `RetellingsData`/`AnswersData`, `ConsultantNotesData`/
  `CoachNotesData`/`ConsultNoteDataConverter`/`CommInstance`.

The semantics are the row path's (including singleton rules and null guards), not the buggy XmlNode
path's.

### 4. `LegacyTextRepair`
Keep only the XElement versions (`DecodePlainTextElements`, `ClearLanguageNamePlaceholders` re-targeted
to XElement + `LanguageInfo` elements). Delete the `DataSet` and `XmlNode` overloads.

### 5. Callers switched to the new loader
- `StoryEditor.OpenProject` and `GetOldStoryProjectData` (renamed to fit)
- the save reload check (`SaveXElement`): XSD validation + `ProjectFile` load
- paste story/column (`StoryEditor`, `PanoramaView`): drop the `GetXmlNode()` round trip
- `RevisionHistoryForm` (`HtmlDisplayForm.cs`): XElement instead of `XmlDocument`
- `StoryData.GetPresentationHtmlForChorus`: converts the incoming `XmlNode` to `XElement` internally;
  signature unchanged

### 6. Fixes included
- **LnCNote `KeyTermIds`:** `GetXml` writes `KeyTermId` while the XSD and loader use `KeyTermIds`, so
  key-term links are lost on every save. Write `KeyTermIds`; read `KeyTermIds`, falling back to
  `KeyTermId` (repairs files saved by the buggy writer).
- **`OseXmlSerializer.SaveDoc`:** its final `if (attempt > 2) throw` can never fire (`maxAttempt = 2`),
  so persistent write errors are swallowed. Make a persistent failure throw, so `SaveFile`'s catch
  reports it and the original file is never replaced.

## Deleted

- `StoryProject.Designer.cs` (generated DataSet), `ProjectReader`, every `DataRow` constructor, every
  `XmlNode` constructor, the XSLT 1.3/1.4 conversion code and its resources.
- The obsolete, out-of-solution projects `Sfm2OneStory` and `StoryEditorData`. Both depend on the
  DataSet and are stale (user: obsolete).

**Kept:** `StoryProject.xsd` (now the save-time validation schema). **Test-only:** a copy of the
generated DataSet in `StoryEditor.Tests` so a test can keep proving the released exe can read files we
write.

## Testing

1. **Characterization tests (first, against the old loader):**
   - dates with and without `Z`/offset; empty/whitespace text elements; `True`/`1` booleans
   - a missing optional attribute; each XSD default; a DOCTYPE
   - duplicate story names and guids
   - `CraftingInfo` count errors
   - The new `XmlRead` helpers and `FromXml` must give identical results.
2. **Unit tests per `FromXml`** using the minimal synthetic fixture (extended as needed: notes with
   `finished`, ReferringText, transitions, anchors with `keyTermChecked`, a non-biblical story,
   LnCNotes, AdaptIt config).
3. **Differential corpus test** (`[Explicit]`; `OSE_CORPUS_DIRS` or the two local folders):
   - for every project, `old.GetXml().ToString() == new.GetXml().ToString()`
   - this is required to reach zero differences on all readable files before switch-over
   - the `KeyTermIds` change is accounted for explicitly; any other difference fails with file,
     element path and both values
   - the test is deleted together with the old loader after switch-over, keeping its last passing
     result in the commit message
4. **XmlNode-path parity:** a story loaded via paste / revision history / Chorus equals the normal load
   (this documents the fixed XmlNode bugs).
5. **Save-guard tests:**
   - a temp file missing a required attribute fails the reload check
   - a valid written file passes, and loads in the test-only copy of the released exe's DataSet
6. **Version gate tests:** 1.3/1.4 refused with the message; 1.6/1.7/1.8 load; 2.0 refused.
7. **Manual (user):**
   - open, edit, save and reopen a few real projects
   - paste a story from another project
   - revision history
   - Chorus change view
   - an L&C note with key terms survives save/reopen

## Order

1. Characterization tests + `XmlRead` helpers.
2. `FromXml` bottom-up (verse content → verse/line → story/crafting info/history → story sets →
   team/settings/LnCNotes → root).
3. Differential corpus test to zero differences.
4. Switch over all callers and the save check; version gate; `SaveDoc` fix; `KeyTermIds` fix.
5. Delete the old loader, the XmlNode constructors, the XSLT conversion, the generated DataSet,
   `Sfm2OneStory` and `StoryEditorData`.

## Out of scope

- Any file-format change beyond `KeyTermIds`.
- Changing `GetXml` output, and performance work.
- `FixupOneStoryFile` and `AiChorus` (they read files with `XDocument` already).
