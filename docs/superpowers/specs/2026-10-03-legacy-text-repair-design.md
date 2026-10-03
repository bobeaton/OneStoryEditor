# Sub-project A: Legacy Text Repair — Design

Status: draft for review · Branch: `DecoupleWebBrowser` · Date: 2026-10-03

## Context

OneStory Editor's HTML panes (`HtmlStoryBtControl`, `HtmlConNoteControl`, both deriving from
`HtmlVerseControl : WebBrowser`) are tightly coupled to the IE WebBrowser control. The overall
effort is to move them to WebView2 (and later possibly cross-platform) in incremental,
individually shippable steps:

| | Sub-project | Still IE? |
|---|---|---|
| **A** | **Legacy text repair** (this document) | yes |
| E | Symmetric LINQ-to-XML serialization (replace the typed `NewDataSet`) | yes |
| B | Host abstraction + JSON message protocol, implemented over IE first | yes |
| C | WebView2 host + modern JS (selection/highlight rewrite) behind a setting | switchable |
| D | .NET 8 / cross-platform | later |

A comes first because WebView2 (C) renders text strictly to HTML standards, while the current data
and rendering rely on IE quirks. A makes the data and the HTML generation correct *while still on
IE*, so the user-visible behaviour does not change and C inherits clean data.

## Findings that motivate A

From a scan of ~195 real project files (`Documents\OneStory Editor Projects`, `C:\btmp\OSE Projects`):

1. **Double-encoded entities in plain-text fields.** About 290 StoryLines and a few
   Retellings, Answers and TestQuestionLines contain literal text like `[B&amp;B]` where the
   user typed `[B&B]`. There are also `&nbsp;` entities in Urdu (lcc-ghiara) and Indonesian
   test questions. The cause is a pair of IE quirks that cancel out:
   - **Writing:** values are inserted into the HTML *unencoded*
     (`String.Format(Resources.HTML_Textarea, …, strValue)` in
     `StringTransfer.FormatLanguageColumnHtml`).
   - **Reading back:** values return through the textarea `onchange` handler, which uses
     `createTextRange().htmlText` (`js/StoryBtPs.js`). That value is *already HTML-encoded*.
   
   The old .NET text-box view stores raw `&`, so the data is mixed.
2. **Note fields (ConsultantNote/CoachNote, including ReferringText, which is stored as a
   ConsultantNote with direction `eReferringToText`) contain real HTML** that must keep
   rendering:
   - "create note" spans: `<SPAN class="LangVernacular StoryLine">…</SPAN>`
   - app prefixes such as `<p><i>Ttg: Catatan Kons:</i></p>`
   - hand-typed `<B>`, `<EM>`, `&nbsp;`
   - captured pane HTML (`<A onclick="return OnBibRefJump(this);" …>`, `<DIV id=… >`)
   - in `Malayalam.onestory`, a full `<SCRIPT>` block containing `window.external` code
   
   These are rendered unsanitized today.
3. **Plain-text fields contain angle-bracket annotations** (`<donkey bray>`, `<malti see note>`,
   `<RTL>`). IE treats these as unknown tags and hides them wherever text is rendered as HTML.
4. **Line breaks are already handled.** `StringTransfer.SetValue` → `StoryData.NormalizeLineEndings`
   turns `\n` into `\r\n` in memory, and files on disk are `\n`-only (XML parsing normalizes).
   A does not change this.

## Goal

- Every non-note field holds **plain text** in the model.
- Note fields hold **legacy HTML** that is **sanitized at render time**.
- All text goes into HTML through one small set of helpers, and the IE panes look and behave as
  they do today.

## Components

### 1. `LegacyTextRepair` (static, new file)

`DecodePlainTextFields(NewDataSet ds)` decodes `&amp; &lt; &gt; &quot; &#39; &nbsp; &#NNN; &#xHH;`
(one level only; `&nbsp;` → U+00A0) in these plain-text columns:

- **Story:** StoryLine, Retelling, Answer, TestQuestionLine and ExegeticalHelp text
- **LnCNote:** text and its renderings
- **CraftingInfo:** StoryPurpose, ResourcesUsed, MiscellaneousStoryInfo
- **Tests:** TestRetelling and TestTqAnswer member comments

It does **not** touch ConsultantNote/CoachNote text (HTML), `PanoramaFrontMatter` (RTF),
names, guids or other attribute data.

It is called from the `StoryProjectData(NewDataSet, ProjectSettings)` constructor **only when the
file's `StoryProject@version` is below `"1.9"`**, before any `StoriesData` is built.

The same decode is applied to plain-text values arriving through the copy-story / copy-column
paste paths (`OseStoryToCopy`, `OseColumnToCopy`; `XElement.Parse` in PanoramaView and the column
paste in StoryEditor) when the source has no version, or a version below 1.9.

### 2. `HtmlText` (static helpers, new file)

| Method | Behaviour | Replaces |
|---|---|---|
| `ForTextarea(string)` | HtmlEncode | raw value in `HTML_Textarea`, `HTML_TextareaWithRefDoubleClick` |
| `ForParagraph(string)` | HtmlEncode, then `\r\n` → `<br />` | raw value in `HTML_ParagraphText`; `InnerHtml = stringTransfer.ToString()` in `HtmlVerseControl` search restore |
| `FromIeHtmlText(string)` | strip highlight spans, `<br>` → `\r\n`, HtmlDecode | (new) applied to the IE `onchange` route |

### 3. `NoteHtmlSanitizer` (new file; wraps Ganss.Xss **HtmlSanitizer** NuGet, netstandard2.0)

Allowlist:
- **tags:** `span p br i b em strong u a`
- **`class` values:** only the language/field classes the app generates: `Lang*`, `StoryLine`,
  `Retelling`, `TestQuestion`, `Answer`, `ExegeticalNote`, `FreeTranslation` and the other
  `StoryEditor.TextFields` names, plus `LocalizationStyle`
- **`a`:** `href`, `name`, `class`. The `onclick` for the known internal `href`s
  (`bibleViewer.setReference` → `OnBibRefJump`, `conNote.jumpToLine` → `OnVerseLineJump`) is
  **re-added by C#** after sanitizing, so those links keep working in IE. Http(s) links keep
  their existing `OnUrlJump` treatment.
- **removed with content:** `script`, `style`
- **removed, content kept:** `div`, `font` and any other tag not on the list
- **attributes dropped:** all `on*`, `id`, `style`

`Sanitize(string html)` returns sanitized HTML. If the sanitizer throws, it falls back to
`HtmlText.ForParagraph(raw)`, which is safe but shows the tags as text, and logs the failure.

### 4. Changes to existing code

- **`StringTransfer.FormatLanguageColumnHtml`:** `HtmlText.ForTextarea` / `HtmlText.ForParagraph`.
- **`ConsultNoteDataConverter`** (both `Html` builders and the read-only builder near :794/:917/:809/:951):
  - ReferringText and read-only comments → `NoteHtmlSanitizer.Sanitize`, then `\r\n` → `<br />`, then `SetHyperlinks`
  - the editable latest-comment textarea → `HtmlText.ForTextarea`
- **`HtmlStoryBtControl`:** split the two routes into `TextareaOnChange`.
  - `TextareaOnKeyUp` (sends plain `this.value`) calls a new private `SetFieldValue(id, text)`.
  - The public `TextareaOnChange` (called from the `StoryBtPs.js` onchange with `htmlText`-derived
    text) calls `SetFieldValue(id, HtmlText.FromIeHtmlText(text))`.
  - No JS changes.
- **`HtmlVerseControl`** search-restore (`InnerHtml = …`): `HtmlText.ForParagraph` (or the
  sanitizer for note paragraphs).
- **`StoryProjectData`:**
  - `XmlDataVersion` becomes `"1.9"` and every save writes `"1.9"`. The conditional 1.7/1.8 bump
    (`SetNextVersionIfNeeded`) is removed.
  - The load check accepts 1.7, 1.8 and 1.9 and refuses anything newer with the existing
    "newer version" message.
  - Consequence: once a team member saves with this version and syncs, teammates on older
    versions get the existing "please update" message.

### Out of scope

- Any JS change, the pane/host protocol (B) or WebView2 (C).
- Changes to how the file is read or written (E).
- Repairing pasted `<OseStoryToCopy>` blobs in StoryLines, or the zero-byte
  `or-mankidia (2).onestory`. These are reported only.

## Edge cases and decisions

- **Notes are never rewritten in storage**; sanitizing happens only when rendering. A too-strict
  allowlist loses nothing and can be loosened later. The editable latest-comment box shows stored
  text as today.
- **Decode applies only to files below 1.9.** In a Chorus merge between a 1.8 and a 1.9 user,
  the merged file is expected to carry `version="1.9"`, which forces the 1.8 user to upgrade
  before reopening. A field the 1.8 user edited just before upgrading may keep one `&amp;`.
  This is accepted as a small risk. **Verify** how the OneStory Chorus plugin resolves the
  root `version` attribute in a conflict (test item below).
- **Decoding one level only:** a user who deliberately typed `&amp;` in a pre-1.9 file sees `&`.
  This is negligible in the data.
- **Search** now matches what users see (searching for `&` finds `[B&B]`).
- **`TextPaster`** (sets `InnerText`, then fires `onchange` → `htmlText`) is covered by the
  `TextareaOnChange` decode.
- **Saving is unchanged**, including `RobustFile` usage and the reload-after-save check.

## Testing

**New project `StoryEditor.Tests`** (net48, NUnit 3, added to `StoryEditor 2017.sln`).

**Unit tests**, using real samples from the scan:
- **`LegacyTextRepair`:**
  - `[B&amp;B]` → `[B&B]`; `snake &amp; lady` → `snake & lady`
  - `&nbsp;` → U+00A0
  - `<donkey bray>` unchanged; `B&B;` (not a known entity) unchanged
  - note tables untouched
- **`HtmlText`:**
  - `ForTextarea`/`ForParagraph` encode `<`, `&` and line breaks correctly
  - `FromIeHtmlText` on an IE-style `htmlText` with `<SPAN class="… highlight">` and `<BR>`
    returns the plain text with `\r\n`
- **`NoteHtmlSanitizer`:**
  - keeps `<SPAN class="LangVernacular StoryLine">idop</SPAN>` and `<B>only</B>`
  - removes the `<SCRIPT>` block and all `onclick`s
  - unwraps `<DIV id=tp_1_0_0 class=TextAreaStyle>`
  - bibref and line-jump links come back with the correct `onclick`
  - drops unknown classes
- **Version gate:** a 1.8 file is decoded and saves as 1.9; a 1.9 file is not decoded; a 2.0
  file is refused.

**Corpus test** (`[Explicit]`, reads the local sample folders):
- every project loads
- no IE-pattern entities remain in plain-text fields
- every note sanitizes without throwing
- a summary of what the sanitizer removed is reported

**Manual, in the IE build:**
- **indo-mualank** (heavy notes, links, ReferringText): notes render as before; links work;
  no script runs.
- **lcc-ghiara** (Urdu `&nbsp;`) and **af-batwa** (`<donkey bray>`): text displays and edits
  correctly; `<donkey bray>` is now visible in read-only/print views.
- **"create note" across two textareas:** highlight and the resulting styled note are unchanged.
- **Type `&` and `<x>`** in a story box, save, reopen: the text is identical.
- **Chorus merge** of a 1.8-edited and a 1.9-saved copy of the same project: the resulting
  version and behaviour on the 1.8 side.

## Notes for sub-project E (recorded here so they aren't lost)

- **Goal:** replace `ReadXml` into the typed `NewDataSet` (`StoryProject.xsd` + the 24k-line
  `StoryProject.Designer.cs`) with `FromXml(XElement)` on each `*Data` class, mirroring the
  existing `GetXml`.
- **Keep the file format exactly as it is**, for the Chorus merge plugin and older versions.
- **Keep all `RobustFile` usage** and the save sequence (write temp → reload check → backup →
  replace). Teams have had files clobbered (e.g. the zero-byte file above), and `RobustFile`
  reduces that.
- **First step:** a round-trip corpus test (load + save each sample file and compare with the
  current writer's output), built on A's test project.
- `LegacyTextRepair.DecodePlainTextFields` moves from `NewDataSet` to the `FromXml` path.
