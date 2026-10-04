# Sub-project B: HTML pane message protocol (still on IE) — Design

Status: sections 1 and save-flush approved in conversation (2026-10-04); remaining sections written while the user was
away, awaiting review · Branch: `DecoupleWebBrowser` (after A, R, E)

## Decisions for the reviewer

These were decided without the user. Each has a default that the rest of the spec assumes:

1. **If the flush fails when the window is closing**, ask "The latest edits could not be collected from the editing
   pane, so the project was not saved. Close anyway and lose them?" (Yes closes, No cancels the close). Every other
   save path cancels the save and shows a message instead.
2. **Mouse-move messages are throttled to one per 100 ms.** They drive only the Bible pane's auto-hide.
3. **Dead code is deleted:**
   - `SelectFoundText` and the missing JS `paragraphSelect`
   - `TextareaOnSelect`
   - the `DEBUGBOB` block in `GetTopHtmlElementId`
   - the `OseResources` folder (it isn't in `StoryEditor 2017.sln`, and nothing references it)
4. **The `Microsoft.mshtml` reference is removed** from `StoryEditor.csproj`. It has a machine-specific HintPath into
   the PIA folder; once nothing uses it, removing it is a deployment win.
5. **Panes keep their class names** (`HtmlVerseControl`, `HtmlStoryBtControl`, `HtmlConNoteControl` and its
   subclasses) so the designer files and callers change as little as possible. Only their base class changes.

## Context and goal

StoryEditor shows its main panes as HTML in the IE `WebBrowser` control. The C# side and the page talk in two ways:

- **JS → C#:** about 36 `window.external.X(...)` calls. Their targets are public methods on `[ComVisible]` classes:
  the panes themselves, `NetBibleViewer` and `HtmlForm`.
- **C# → page:**
  - 6 `InvokeScript` targets (`TriggerMyBlur`, `ClearSelectionSpan`, `textboxSetSelection`,
    `textboxSetSelectionTextReturnEndPosition`, the missing `paragraphSelect`, and `InvokeMember("onchange")`)
  - about 20 places that touch the DOM directly: `HtmlElement`, `GetElementById`, `InnerText`/`InnerHtml`,
    `GetElementFromPoint`, `ScrollIntoView`, mshtml `IHTMLTxtRange` selection reads, and `HTMLDocumentClass` node
    removal

WebView2 (sub-project C) has none of these. It offers a string channel each way: `postMessage` from JS, and
`ExecuteScriptAsync` from C#. Both are asynchronous. WKWebView, WebKitGTK and the cross-platform wrappers offer the
same shape.

**Goal:** after B, every HTML host talks to its page only through one small interface over a JSON message
channel. JS owns the page state and pushes it to C#. When C# needs to know something *now*, it asks with a bounded
request. C# never touches the DOM. Sub-project C then only adds a `WebView2HtmlHost` and a setting to choose it.

**Success:**

- No `window.external` outside `js/bridge.js`.
- No `HtmlElement`, `HtmlDocument`, `InvokeScript`, `InvokeMember`, `DomDocument`, `DocumentText` or `mshtml` outside
  `IeHtmlHost.cs`.
- All 9 hosts work through `IHtmlHost`.
- No behaviour change visible to users, confirmed by the manual smoke checklist against the released exe.
- New NUnit tests cover the dispatchers, the flush and the pure helpers.
- An architecture guard test keeps the two rules above from regressing.

## Scope: 9 hosts

| Host | Today | After B |
|---|---|---|
| `HtmlStoryBtControl` (main Story/BT pane) | `WebBrowser` subclass, `ObjectForScripting = this` | `UserControl` holding an `IHtmlHost`; the full protocol |
| `HtmlConsultantNotesControl`, `HtmlCoachNotesControl` (via `HtmlConNoteControl`) | same | same |
| `HtmlDisplayForm` (revision history), `SwapColumnsForm` (×2), `PrintViewer` (used by `PrintForm` and `LnCNotePrintForm`) | reuse `HtmlStoryBtControl` | unchanged code; they get the new control. `PrintViewer` uses `IHtmlHost.ShowPrintPreview()` and reads the HTML it loaded from the host |
| `AddConNoteForm` | creates a ConNote pane with `Activator.CreateInstance` and sets `DocumentText` | calls `LoadHtml` |
| `NetBibleViewer` | its own `WebBrowser` with inline script; hover, mouse down/out/up (drag a reference), scroll to an element | `IeHtmlHost` + `js/NetBible.js` |
| `HtmlForm` (commentary / hover popup) | inline script `ShowHoverOver`; scroll to an element | `IeHtmlHost` + `js/HoverLinks.js` |
| `MinimalHtmlForm` (`MoveConNoteTooltip`, `NetBibleFootnoteTooltip`) | display only (`DocumentText`) | `IeHtmlHost.LoadHtml`, no messages |

Out of scope:

- WebView2 itself, the native `placeholder` attribute, and the CSS Custom Highlight API. These belong to C.
- Ctrl+F. It stays IE's built-in find, and WebView2 has its own.
- Changes to the data model or file format. There are none.

## Section 1: Transport and wire protocol (approved)

### `IHtmlHost`

```csharp
public interface IHtmlHost : IDisposable
{
    Control Control { get; }                         // added to the owning pane/form
    string LoadedHtml { get; }                       // last string passed to LoadHtml (PrintViewer save, HtmlForm getter)
    void LoadHtml(string html);                      // "" = blank page (replaces ResetDocument/OpenNew)
    void Post(string type, object payload = null);   // C# → JS, does not wait for a reply
    HtmlMessage Request(string type, object payload, TimeSpan timeout); // bounded round-trip, null on timeout
    event EventHandler<HtmlMessage> MessageReceived; // JS → C#, raised on the UI thread
    event EventHandler DocumentReady;                // JS sent "ready" (replaces DocumentCompleted)
    void ShowPrintPreview();
}
```

- `HtmlMessage` holds `Type` and the raw `JObject`. It has typed helpers: `GetString`, `GetInt`, `GetBool`.
- JSON goes through Newtonsoft.Json, which is already deployed (`ViewSwordOptionsForm` uses it).
- `HtmlHostFactory.Create()` returns an `IeHtmlHost`. Sub-project C adds the setting that picks WebView2.

### `IeHtmlHost`

- Wraps a `WebBrowser`:
  - `AllowNavigation = false`
  - `AllowWebBrowserDrop = false`
  - `IsWebBrowserContextMenuEnabled = false`
  - `ScriptErrorsSuppressed` left as it is today

  The designer files stop setting these on the panes.
- Its `ObjectForScripting` is a private `[ComVisible]` class with one method, `postMessage(string json)`.
- **Delivery is asynchronous, even on IE.** `postMessage` queues the parsed message with
  `Control.BeginInvoke` and returns at once. This copies WebView2's timing, so any code that relied on IE's
  synchronous re-entry fails now, on IE, where we can compare against the released exe.
- `Post` calls `Document.InvokeScript("oseReceive", json)`. Before the page is ready, posts are queued and sent
  when `ready` arrives. Posts made while no document exists are dropped and logged.
- `Request`:
  1. Adds `"rid": n` and posts the message.
  2. Pumps messages with `Application.DoEvents()` in a loop with a 1 ms sleep, until the matching
     `{type:"reply", re:n}` arrives or the timeout passes.
  3. Messages that arrive in the meantime are dispatched normally and in order. The reply is taken out of the
     queue and returned.

  Nested `Request` calls are allowed; each waits for its own `rid`. The same pumping wait works with WebView2,
  where replies arrive as `WebMessageReceived` on the UI thread.
- `LoadHtml` sets `DocumentText`. It is the only place in the app that does.

### Envelope

- Flat JSON: `{"type":"textChanged","id":"ta_3_StoryLine_0_0_Vernacular","value":"..."}`.
- No version field. The page and the C# always ship in the same exe.
- Element ids keep their current formats (`ta_…`, `tp_v_c_i`, `btn_v_c_i`, `lineTable_N`, `ln_N`, `anc_N`), so
  `TryGetTextAreaId`, `GetStringTransferEx` and their relatives don't change.

### `js/bridge.js` (new, inlined into every page)

- `ose.send(type, payload)`
- `ose.on(type, handler)`. The handler's return value becomes the reply when the incoming message has a `rid`.
- `window.oseReceive(json)`
- `window.onerror` → `ose.send("jsError", {message, source, line})`. C# logs it with `Debug.WriteLine`. This
  replaces the silent failure we get today.
- `ose.send("ready")` on `window.onload`.
- **Transport detection:** `window.chrome.webview.postMessage` (C) → `window.webkit.messageHandlers.ose` (later) →
  `window.external.postMessage` (IE). This is the only file that names a host.

### Buttons and inline handlers stop carrying script

- `HTML_ButtonClass` becomes `<button id="{0}" class="{1}" data-action="{2}">{3}</button>`. C# passes an action name
  (`addNote`, `delete`, `endConversation`, …) instead of a `return window.external.X(this.id…)` string. The 12 call
  sites are in `VerseData.cs:2054-2077` and `ConsultNoteDataConverter.cs:1083-1188`.
- One delegated `click` handler sends `{type:"action", name, id}` plus any `data-arg`. `OnConvertToMentoreeNote`'s
  bool becomes `data-arg="true"`.
- Inline attributes in the resx templates become handlers registered by the page's JS file:
  - `onscroll`, `onmouseup` and `onKeyDown` on `<body>`
  - `ondrop`, `ondragover` and `onmouseup` on the anchor `<td>`
  - `onClick` on the jump and bible-reference links and the http link
  - `onMouseUp` on the anchor and line-options buttons
  - `ondblclick` and `onKeyDown` on the note textarea
  - the whole `HTML_Script_AddTextareaMouseDown` block, which moves into `ConNoteDomPrefix.js`

  Links keep their `href` / `name` attributes, so the delegated handler tells them apart by
  `href="bibleViewer.setReference"`, `href="conNote.jumpToLine"` and `http(s)://`.
- `NoteHtmlSanitizer` already removes internal links from stored note HTML and `SetHyperlinks` rebuilds them from
  the templates, so stored data never carries the old `onClick` strings back in.

### Typed dispatch

- Each pane has a dispatcher: a `Dictionary<string, Action<HtmlMessage>>` filled in its constructor. Handlers parse
  ids and numbers explicitly.
- That replaces COM's quiet string→int and string→bool coercion in `OnVerseLineJump`, `OnAddNote` and
  `OnConvertToMentoreeNote`.
- An unknown type or a bad payload is logged and ignored. It is never thrown into the UI.
- Handler exceptions are caught at the dispatcher. They are logged and shown on the status bar the way
  `CheckForProperEditToken` errors are now, so one bad message can't take down the message loop.

## Section 2: Pane restructure and the state JS owns

### Holding a host instead of being a browser

- `HtmlVerseControl : WebBrowser` becomes `HtmlVerseControl : UserControl`. It creates its host in the constructor
  (`Host = HtmlHostFactory.Create()`), docks `Host.Control` to fill, and subscribes to `MessageReceived` and
  `DocumentReady`.
- Members of the `WebBrowser` base that callers use are replaced:

  | Old | New |
  |---|---|
  | `DocumentText = html` | `Host.LoadHtml(html)` |
  | `Document?.OpenNew(true)` (`ResetDocument`) | `Host.LoadHtml("")` |
  | `DocumentCompleted` | `Host.DocumentReady` |
  | `IsWebBrowserContextMenuEnabled` etc. in the designer files | set by `IeHtmlHost`; the designer lines are removed (StoryEditor, SwapColumnsForm, PrintViewer, NetBibleFootnoteTooltip, HtmlStoryBtControl/HtmlConNoteControl `InitializeComponent`) |
  | `PrintViewer.webBrowser.DocumentText` / `.ShowPrintPreviewDialog()` | `Host.LoadedHtml` / `Host.ShowPrintPreview()` |

- `ObjectForScripting = this` and `[ComVisible]` are removed from every pane and form.
- The `public` callback methods that JS called become private handlers registered with the dispatcher, except
  where other C# also calls them (for example `OnAddNote` from `StoryEditor.cs:1822`).

### The rule for state

1. **Events JS already knows about are pushed.** Focus, keyup and edits, scroll position, context-menu requests,
   button actions, link clicks, drops. C# reacts or caches.
2. **When C# needs current page state on demand, it asks with `Request`.** This keeps today's semantics. Each place
   that used to read the DOM synchronously makes one bounded request: the selection highlights, the selection for a
   note, find/replace, and the flush. The request timeout is 2 s.
3. **When C# changes the page, it posts a command** and doesn't wait: scroll, select a range, set a textarea's text,
   append HTML, set a paragraph's HTML, remove an element, clear a highlight.

### Story/BT selection highlighting (the must-keep feature)

The rendering stays exactly as it is today. When a textarea loses its selection, `TriggerMyBlur` rewrites the
textarea's HTML so the selection becomes `<span class="<lang> highlight">`. Highlights in several textareas build
up, and "Add note on selected text" collects them. **Only how C# reads them changes:**

- `GetSelectedTexts(line)` used to call `InvokeScript("TriggerMyBlur")` and then walk `lineTable_N` for `span`
  elements, returning `List<HtmlElement>`. It now calls `Request("getHighlights", {line})`. The JS handler:
  1. runs `TriggerMyBlur()`
  2. finds the spans under `lineTable_N`
  3. replies with `{items:[{textareaId, className, text}]}`

  `text` is the span's `innerText`, and `className` is the span's `className` attribute.
- The return type becomes `List<HighlightedText>`, where `HighlightedText` is an immutable record with
  `TextareaId`, `ClassName` and `Text`. Its consumers are rewritten one for one:
  - `GetSpanInnerText` filters on `TextareaId` instead of `span.Parent.Id`.
  - `AddNote` builds the referring text with a new pure function, `ReferringTextBuilder.Build(items, …)`. It
    produces `<span class="{ClassName}">{HtmlEncoded Text, \n→<br>}</span>` in place of `span.OuterHtml`, then
    strips ` highlight` and ` readonly` exactly as now. The stored text ends up as the same markup in lower case;
    `NoteHtmlSanitizer` keeps only `span` with `Lang*` classes in any case.
  - `MoveSelectedText`, `GetSelectedLanguageText`, `GetSelectedTextByTextareaIdentifier` and `onCutSelectedText`
    work on the records.
- `ClearSelectionSpans` → `Post("clearHighlight", {id})` for each distinct `TextareaId`.
- `OnLineOptionsButton`'s right-click path used to `InvokeScript("TriggerMyBlur")`. The JS now runs `TriggerMyBlur`
  itself before it sends the `action`. It already does this before calling C#, so the C# call was redundant.
- In C, the rendering moves to the CSS Custom Highlight API. The `getHighlights` reply format stays the same.

### Other C# DOM reads and writes, and what replaces them

| Today (file) | After B |
|---|---|
| `OnScroll` → `GetTopHtmlElementId("td")` (`GetElementFromPoint`, offset walk), parse the "Ln: N" `InnerText` (HtmlVerseControl.cs) | JS sends `scrolled {topId, topLabel, prevId, nextId}`, throttled with `setTimeout` to one per 50 ms (IE9 has no `requestAnimationFrame`). It mirrors the current logic exactly: first `document.elementFromPoint` at the window's top-left (C# used `GetElementFromPoint(doc.Window.Position)`; that element's `id` and `innerText` are reported as they are). If that returns nothing, it falls back to the last `td[id]` whose summed `offsetTop` is ≤ `scrollTop + 1`. C# caches it in `TopRowId`/`PrevRowId`/`NextRowId` and calls a new pure `LineLabelParser.TryParse(label, out text, out lineIndex)` (the current Zeroth/BtPane/"Ln: N (Hidden)" logic, localized prefixes passed in) → `SetLineNumberLink`. The page also sends `scrolled` once after `ready` |
| `GetTopRowId` / `GetNextRowId` / `GetPrevRowId` (existence checked with `GetElementById`) | read the cached values. JS fills in `nextId`/`prevId` only if those rows exist |
| `ScrollToElement(id, alignTop)` with `Application.DoEvents()` + `ScrollIntoView` + `Focus` | `Post("scrollTo", {id, alignTop, focus: !alignTop})`. JS defers with `setTimeout(0)`, which replaces the `DoEvents` hack. On `DocumentReady` the pane posts `scrollTo` for `StrIdToScrollTo` |
| `TriggerChangeUpdate` → `InvokeMember("onchange")` (HtmlStoryBtControl) | removed. The flush covers it (Section 3). `onCutSelectedText` now calls the flush |
| `SetSelectedText` → `InvokeScript("textboxSetSelectionTextReturnEndPosition")` + read `InnerHtml` (HtmlVerseControl; callers: SearchForm replace, SE paste, StoryBt cut) | `Request("replaceSelection", {id, text})` → reply `{endPoint, ieHtml}`. C# applies `HtmlText.FromIeHtmlText(ieHtml)` to the `StringTransfer` exactly as now. `endPoint == 0` still means failure |
| `GetSelectedText(StringTransfer)` via mshtml `selection.type` / `IHTMLTxtRange.text` (HtmlVerseControl; SearchForm) | `Request("getSelection", {id})` → `{type, text}`. The "only editable text" message stays in C# |
| `ClearSelection` (`selection.empty()`; or a paragraph's `InnerHtml` = sanitized HTML) | textarea: `Post("clearSelection")`. paragraph: `Post("setHtml", {id, html})`, where `html` is still made by `NoteHtmlSanitizer.ToReadOnlyHtml` / `HtmlText.ForParagraph` |
| `HtmlConNoteControl.SetSelection` (`textboxSetSelection`; or a paragraph's highlighted `InnerHtml`) | `Post("selectRange", {id, start, length})` / `Post("setHtml", {id, html})` |
| `ConNoteAddNote` reads the selection's `htmlText` and walks parent elements for `id=tp_(\d+)_` (HtmlConNoteControl) | `Request("getNoteSelection")`. JS does the same walk and replies `{lineIndex, html}`, or `{}` when nothing is selected. C# keeps `regexStripTableBits` and the "Re: ConNote:" prefix |
| `RemoveHtmlNodeById` via `HTMLDocumentClass` (OnClickDelete, last conversation) | `Post("removeElement", {id})` |
| `OnClickEndConversation` sets the button's `InnerText`, then `LoadDocument()` | drop the `InnerText` write; the reload replaces it anyway |
| `CopyScriptureReference` (`InnerText +=`, model update, `Focus`) | `Post("appendText", {id, text, focus:true})`. JS appends and sends `textChanged` as for any edit, so the model is updated by the normal path |
| `AddScriptureReference` (`CreateElement` + `AppendChild` of an anchor button) | `Post("appendHtml", {id, html})`. The button HTML is still made from `HTML_ButtonToolTip` in C# (now with `data-action`) |
| `TextPaster.SetElementText` (`InnerText =` + `InvokeMember("onchange")`); `GetTextareaText` (`InnerText`) | `Post("setText", {id, text})`. JS sets `value` and sends `textChanged`. The current text comes from the `textareaMouseDown` message, which already carries `value`. `TextPaster.TriggerPaste` takes `(HtmlVerseControl pane, string id, string currentText)` instead of an `HtmlElement` |
| `HtmlForm.ScrollToElement`, `NetBibleViewer.ScrollToElement` | `Post("scrollTo", …)` |

### Messages JS sends and their C# handlers

- **`textChanged {id, value}`** or **`{id, ieHtml}`**
  - `value` is the textarea's `.value`. It is sent on keyup (with the same Ctrl+C/A/F/H/S skip list), paste, cut and
    drop.
  - `ieHtml` is today's onchange form: `createTextRange().htmlText` with spans removed and `<br>`→`\r\n`. C# runs
    it through `FromIeHtmlText`, as now.
  - The two fields keep today's two paths byte-for-byte. Under WebView2 (C) only `value` will be sent.
  - The placeholder guard stays: a box showing the language name (`hasPlaceholder`) sends `value:""`.
  - The JS `readonly` check is corrected to `readOnly`. C# still rejects edits to read-only fields as before.
  - Story pane → `SetFieldValue`. ConNote pane → `FinalComment.SetValue`. Both also set `Modified`,
    `LastKeyPressedTimeStamp` and the status bar.
- **`focus {id}`** → `LastTextareaInFocusId` and keyboard activation.
- **`blur {id}`** → `Program.ActivateDefaultKeyboard()`.
- **`textareaMouseUp {id}`**
- **`textareaMouseDown {id, value, button}`** → TextPaster.
- **`contextMenu {id?}`** → shows the `ContextMenuStrip` at `Cursor.Position`. Story pane: textarea menu. ConNote
  pane: note menu.
- **`action {name, id, arg?}`** covers:
  - anchor buttons, line options, the empty anchor cell
  - ConNote buttons: addNote, addNoteToSelf, addStickyNote, showHideOpen, delete, convertToMentoree,
    convertToMentor, convertToMentorToSelf, convertToMentoreeToSelf, approve, endConversation
- **`bibRefJump {ref}`**
- **`verseLineJump {index}`**
- **`openUrl {url}`** → `Process.Start`.
- **`scriptureDropped {id}`** → Story pane `AddScriptureReference`; ConNote pane `CopyScriptureReference`.
- **`mouseMove`** (throttled) → `CheckBiblePaneCursorPosition`.
- **`save`** (Ctrl+S)
- **`reload`** (F5)
- **`realign`** (Ctrl+F5)
- **`scrolled`**
- **`ready`**
- **`jsError`**
- **`reply`**
- `LogMessage` becomes `log {text}`, sent only from `DisplayHtml`.

### Commands C# sends, which JS handles

`getHighlights`, `clearHighlight`, `getSelection`, `replaceSelection`, `getNoteSelection`, `clearSelection`,
`selectRange`, `setHtml`, `setText`, `appendText`, `appendHtml`, `removeElement`, `scrollTo`, `flush`.

### Page assembly

- `StoryBt.htm` gets a `{6}` slot for `bridge.js`, inlined before jQuery and `StoryBt.js`. Its `<body onscroll>` is
  removed.
- `HTML_Header` (ConNote) gets `bridge.js` inlined before `ConNoteDomPrefix.js`. Its `<body …>` handlers are
  removed. The `{3}` slot (`HTML_Script_AddTextareaMouseDown`) is removed and that code moves into
  `ConNoteDomPrefix.js`.
- `NetBibleViewer` and `HtmlForm` swap their inline `preDocumentDOMScript` / `postDocumentDOMScript` strings for
  `bridge.js` plus `js/NetBible.js` / `js/HoverLinks.js`, as embedded resources like the others. Their style
  strings stay.
- `StoryData.AddHtmlHtmlDocOutside` is also used by `PrintForm` and `LnCNotePrintForm`, so those pages get the
  bridge too. Their messages go to the `HtmlStoryBtControl` in `PrintViewer`, as today.

## Section 3: Save flush (approved)

- A new `bool StoryEditor.FlushPendingEdits()`:
  1. Sends `Request("flush", null, 3 s)` to each loaded pane: Story/BT, Consultant and Coach.
  2. The JS handler sends a final `textChanged` for the focused or last-focused textarea, using the onchange form in
     the Story pane, skipping read-only and placeholder boxes. Then it replies.
  3. Messages arrive in order, so by the time the reply comes back every edit has been applied.
  4. It returns false if any pane doesn't reply in time.
- **It's called first thing** in:
  - `SaveClicked()`, before the `Modified` check. This fixes the existing early return on an edit that hasn't been
    recorded yet.
  - `StoryEditor_FormClosing`
  - `CheckForSaveDirtyFileNoCleanup`
  - the autosave timer tick

  These all check `Modified` before they decide whether to ask or save.
- **On failure:**
  - Save: no write. The message says "The latest edits could not be collected from the editing pane, so the
    project was not saved. Please try again." `Modified` stays true.
  - Closing: Decision 1 above.
  - `CheckForSaveDirtyFileNoCleanup`: the same message, and it returns false, which cancels the new action, as
    Cancel does today.
- **Re-entrancy:** a `_bSaving` guard in `SaveClicked` drops a nested `save` (for example Ctrl+S delivered during the
  flush pump) and an autosave tick that fires during a save. `FlushPendingEdits` is re-entrant-safe because each
  `Request` waits only for its own `rid`.
- The coordination logic lives in a small testable class, `PaneFlush`. It takes `IEnumerable<IHtmlHost>` and a
  timeout and returns bool. `StoryEditor` stays thin.

## Section 4: Error handling

- Malformed JSON, an unknown `type`, or a missing or invalid field → `Debug.WriteLine` plus a counter. It is never
  shown to the user and never throws.
- A handler exception → caught by the dispatcher, logged, and shown on the status bar as `Error: {message}`, the
  same as `CheckForProperEditToken`.
- A `Request` timeout → `null` to the caller, and each call site keeps its current "nothing selected" path:
  - highlights: an empty list
  - getSelection: null
  - replaceSelection: `endPoint 0`, so "didn't work"
  - noteSelection: return
  - flush: Section 3
- `jsError` from `window.onerror` → logged with source and line. Today these surface as IE script-error dialogs
  when `ScriptErrorsSuppressed` is off, or nowhere at all.
- A `Post` before `ready` is queued; a `Post` with no document is dropped and logged.

## Section 5: Testing

**NUnit (no browser):**

- `HtmlMessage` parse and serialize: round-trips, missing fields, wrong types.
- `FakeHtmlHost`: records `Post`s and returns scripted `Request` replies. It's used for:
  - the dispatcher tables for each pane: every message type maps to its handler; unknown types and bad ids are
    ignored
  - `PaneFlush`:
    - all panes reply → true
    - one pane times out → false
    - a `textChanged` delivered before the reply is applied before `PaneFlush` returns
    - no write after a failure, checked through a fake save action
- `LineLabelParser`: zeroth line (ConNote and BT forms), "Ln: 5", "Ln : 5" (French), "Ln: 5 (Hidden)", garbage.
- `ReferringTextBuilder`: field-type separators (" vs: ", " &"), encoding, newline → `<br>`, `highlight`/`readonly`
  stripping. A characterization case reproduces a string captured from the old `OuterHtml` path.
- `TextPaster` with a fake pane: `setText` posted with the right id and text.
- **Architecture guard:** a test scans `StoryEditor/*.cs` and `StoryEditor/js/*.js`. It fails on `window.external`
  outside `bridge.js`, and on `HtmlElement`, `HtmlDocument`, `InvokeScript`, `InvokeMember`, `DomDocument`,
  `DocumentText`, `ObjectForScripting` or `mshtml` outside `IeHtmlHost.cs`.
- The existing 199 tests stay green. `HtmlDiffTests` and any golden HTML change only where the templates changed
  (`data-action` instead of `onClick`), and those expected values are updated on purpose.

**Manual smoke checklist** (on IE, compared side by side with the released exe):

- **Typing and saving:**
  - Type in every Story/BT column, then Ctrl+S immediately.
  - Type, then close the window at once (must prompt or save, and must not lose the edit).
  - Type in a ConNote, then save.
  - Autosave after 5 minutes.
  - Line breaks survive save and reload.
- **Selection:**
  - Highlight text in two columns of one line → right-click → Add note: the referring text shows both, and the
    highlights clear.
  - Copy / Cut / Paste selected (context menu and Edit menu).
  - Concordance on a selection.
  - L&C note lookup on a selection.
  - Move selected text to a new line.
- **Notes:**
  - Add a note on selected text in a ConNote read-only paragraph.
  - Every ConNote button: add, to-self, sticky, show/hide open, delete (also the last conversation), convert
    ×4, approve, end conversation.
- **Navigation:**
  - Scrolling updates the line-number link.
  - Jump-to-line links in both note panes.
  - Bible-reference links and anchor buttons (left: jump; right: menu).
  - http links open in the default browser.
  - F5 / Ctrl+F5.
  - Scroll position is kept across reloads.
- **Drag a reference:**
  - from the Bible pane onto an anchor cell (adds a button)
  - onto a ConNote textarea (appends the reference)
- **Keyboard switching** on focus and blur for each language column.
- **TextPaster** left and right click.
- **Find/Replace in the note panes** (SearchForm): find highlights, replace one, replace all.
- **Other hosts:**
  - Revision history view.
  - Swap-columns form (both panes scroll in step).
  - Print preview and save from PrintViewer.
  - Commentary HtmlForm hover links.
  - Footnote and move-note tooltips.
  - Bible pane auto-hide on mouse-move.

## Rollout and order of work

This section is for the implementation plan, which comes after this spec is approved. Each step builds and passes the
tests, so it can be committed on its own:

1. `IHtmlHost`, `HtmlMessage`, `IeHtmlHost`, `FakeHtmlHost`, `bridge.js` and their tests. Nothing uses them yet.
2. The display-only and small hosts: `MinimalHtmlForm`, `HtmlForm`, `NetBibleViewer`, `PrintViewer`'s load and
   print path.
3. `HtmlVerseControl` becomes a `UserControl` with a host; shared messages (scroll, save, reload, links, log); the
   designer files.
4. The ConNote panes: buttons as `data-action`, the template and JS moves, note selection, find/replace, removing
   elements, the drop.
5. The Story/BT pane: highlights through `getHighlights`, text changes, focus and keyboard, context menus, anchors,
   TextPaster, cut, paste and replace.
6. The save flush: `PaneFlush`, the four entry points, the guards.
7. Cleanup: dead code, `OseResources`, the `Microsoft.mshtml` reference, the architecture guard test, and a final
   grep.

There is no data or file-format change, and no version marker. Falling back to the released exe stays seamless.

## Risks

- **Focus and keys inside a `UserControl`.** The browser is one level deeper than before. Ctrl+F/F3/Ctrl+H are
  swallowed by hidden menu items (sub-project R), and Tab, the clipboard keys and the Keyman keyboards all need
  re-checking. These are on the smoke checklist.
- **`DoEvents` re-entrancy during `Request`.** The waits are short and bounded, and the code already uses `DoEvents`
  in the same places (`ScrollToElement`, `ClearFlowControls`). The save guard covers the dangerous case.
- **Drag from `NetBibleViewer`** relies on WinForms `DoDragDrop` reaching the HTML `ondrop` in another browser. B
  keeps that mechanism; C has to check it against WebView2.
- **Asynchronous `contextMenu`.** The menu now opens a few milliseconds later, at `Cursor.Position`. That isn't
  noticeable.
