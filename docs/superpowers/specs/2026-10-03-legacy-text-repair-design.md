# Sub-project A: Legacy Text Repair — Design

Status: approved (rev 2: marker attribute instead of version bump) · Branch: `DecoupleWebBrowser` · Date: 2026-10-03

## Context

OneStory Editor's HTML panes (`HtmlStoryBtControl`, `HtmlConNoteControl`, both deriving from
`HtmlVerseControl : WebBrowser`) are tightly coupled to the IE WebBrowser control. The overall
effort is to move them to WebView2 (and later possibly cross-platform) in incremental,
individually shippable steps:

| | Sub-project | Still IE? |
|---|---|---|
| **A** | **Legacy text repair** (this document) | yes |
| R | Remove the old .NET text-box Story/BT view (`advancedUseOldStyleStoryBtPaneMenu`, `CtrlTextBox`, `VerseBtControl` & co., `UsingHtmlDisplayForConNotes`) | yes |
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

It is called from the `StoryProjectData(NewDataSet, ProjectSettings)` constructor, before any
`StoriesData` is built, **only when the file lacks the plain-text marker** (see "Plain-text
marker" below).

The same decode is applied to plain-text values arriving through the copy-story / copy-column
paste paths (`OseStoryToCopy`, `OseColumnToCopy`; `XElement.Parse` in PanoramaView and the column
paste in StoryEditor) when the pasted root element lacks the marker.

### Plain-text marker (replaces a version bump)

- The new exe writes the attribute **`TextEncoding="plain"`** on the `StoryProject` root (and on
  `OseStoryToCopy` / `OseColumnToCopy` clipboard roots). **`version` is not changed**; the
  existing 1.6/1.7/1.8 logic stays as is.
- Because neither the released exe's nor this exe's typed `NewDataSet` knows the attribute,
  `ReadXml` ignores it. The new exe reads it separately: `ProjectReader.ReadProjectFile` reads the
  root element's attributes with an `XmlReader` (first element only) and exposes
  `ProjectReader.IsPlainTextEncoded`.
- The released exe never writes the marker (its `GetXml` doesn't know it). So if anyone saves with
  the released exe, the marker disappears and the next load in the new exe decodes again, cleaning
  up anything the released exe re-encoded via `htmlText`.
- Result: **mixed teams work, nobody is locked out, and users can go back to the released exe with
  no manual steps.**

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
- **`StoryProjectData`:** call `DecodePlainTextFields` when `!IsPlainTextEncoded`; `GetXml`,
  `GetXmlToCopyStory` and `GetXmlToCopyColumn` add `TextEncoding="plain"`. Version handling is
  unchanged.
- **`ProjectReader`:** add `IsPlainTextEncoded` (read from the root element as described above).

### Out of scope

- Any JS change, the pane/host protocol (B) or WebView2 (C).
- Changes to how the file is read or written (E).
- The old .NET text-box view (removed in R); A ignores any implications for it.
- Repairing pasted `<OseStoryToCopy>` blobs in StoryLines, or the zero-byte
  `or-mankidia (2).onestory`. These are reported only.

## Edge cases and decisions

- **Notes are never rewritten in storage**; sanitizing happens only when rendering. A too-strict
  allowlist loses nothing and can be loosened later. The editable latest-comment box shows stored
  text as today.
- **Decode runs whenever the marker is missing.** Repeated decoding (one level each time) only
  affects text where a user deliberately typed an entity such as `&lt;`; this is negligible in the
  data.
- **Released exe on cleaned data:** decoded text containing `<…>` is inserted raw into the
  released exe's read-only/print views and is hidden there. That is already true today for
  `<donkey bray>`; most newly affected text is the junk `<OseStoryToCopy>` pastes. Accepted.
- **Chorus merges:** if the merged root keeps the marker but a released-exe user edited a field,
  that field may keep one `&amp;` until the next marker-less save. Accepted as a small risk.
- **Ctrl+B / Ctrl+I in the editable note box** insert plain-text markers `$…$` / `*…*`
  (`transformText` in `ConNoteDomPrefix.js`); `SetHyperlinks` turns them into `<b>`/`<em>` only
  when rendering read-only. A keeps this: the editable box holds plain text, and the read-only
  order is sanitize → line breaks to `<br />` → `SetHyperlinks`, so the app-generated
  `<b>`/`<em>`/links are never sanitized away. (In C, `transformText` is rewritten with
  `selectionStart`/`selectionEnd`/`setRangeText`; same keys and markers.)
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
- **Marker:**
  - a file without the marker is decoded; a file with it is not
  - saving writes `TextEncoding="plain"` and leaves `version` unchanged
  - `ProjectReader.ReadProjectFile` loads a marked file without error (the same typed DataSet the
    released exe uses, so this also shows the released exe ignores the attribute)
  - copy-story/column XML: with marker not decoded, without marker decoded

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
- **Released exe round trip:** open a marked file in the currently released OSE, edit a field
  containing `&`, save; confirm it opens, the marker is gone, and the new exe cleans the field on
  the next load.

## Notes for sub-project C (recorded here so they aren't lost)

- The IE/WebView2 choice is a **setting in one exe** (plus a command-line override), like the old
  .NET-view toggle. Loading, rendering and saving are shared, so switching has no effect on the data.

## Notes for sub-project E (recorded here so they aren't lost)

- **Goal:** replace `ReadXml` into the typed `NewDataSet` (`StoryProject.xsd` + the 24k-line
  `StoryProject.Designer.cs`) with `FromXml(XElement)` on each `*Data` class, mirroring the
  existing `GetXml`.
- **Keep the file format exactly as it is**, for the Chorus merge plugin and older versions
  (including the `TextEncoding` marker from A, which then becomes a normal attribute).
- **Keep all `RobustFile` usage** and the save sequence (write temp → reload check → backup →
  replace). Teams have had files clobbered (e.g. the zero-byte file above), and `RobustFile`
  reduces that.
- **First step:** a round-trip corpus test (load + save each sample file and compare with the
  current writer's output), built on A's test project.
- `LegacyTextRepair.DecodePlainTextFields` moves from `NewDataSet` to the `FromXml` path.
