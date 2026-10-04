# Sub-project C: WebView2 panes — Design

Status: approved in conversation (2026-10-04) · Branch: `DecoupleWebBrowser` (after A, R, E, B)

## Context and goal

Sub-project B put every HTML host behind `IHtmlHost`, a JSON message channel. `IeHtmlHost`, the IE `WebBrowser`, is the
only implementation. During B we found that every real page runs in **IE quirks mode** (documentMode 5), and the
page scripts depend on it:
- `document.selection` and `createTextRange`
- highlight spans inside a textarea
- expando `selectionStart`
- legacy `event.button`
- events attached with `attachEvent`

**Goal.** Add WebView2 (Chromium, the Evergreen runtime) as a second engine in the same exe:
- WebView2 is the default wherever it works.
- The app falls back to IE automatically when WebView2 isn't available or fails.
- A setting lets users force IE.

Nothing users rely on is lost. In particular:
- multi-box selection highlighting feeding "Add note on selected text"
- line breaks persisting
- drag-and-drop of Bible references
- the keyboard shortcuts

**Success:**
- On a machine with the WebView2 runtime, every pane and popup works in WebView2. The smoke checklist passes, and the
  layout is checked against IE.
- On a machine without the runtime, or where WebView2 fails, the app runs on IE with a one-line notice, never a blank
  pane.
- The IE path's scripts and behaviour are unchanged.
- No data or file-format change, so falling back to the released exe stays seamless.

## Decisions (made with the user)

1. **Default engine:** WebView2 wherever the runtime works, with automatic fallback to IE. A user setting, *Use the IE
   browser panes*, forces IE. Changing the setting takes effect after a restart.
2. **Runtime delivery:** the Evergreen runtime. The installer carries Microsoft's online bootstrapper (about 2 MB) and
   runs it only when the runtime is missing, ignoring failures. Offline machines finish installing and use IE until the
   runtime appears.
3. **Page side:** two script sets, chosen by engine. IE keeps today's quirks-mode scripts byte-for-byte. WebView2 gets a
   modern set in `js/modern/`. Both speak B's message protocol unchanged, apart from one reply field (see Section 2).
4. **Highlight in WebView2:** the mirror ("backdrop") technique. A layer behind each transparent textarea shows the
   highlighted range. Story fields stay textareas.

Rejected:
- **One modern script set with IE moved to standards mode:** it reopens what Task 6b fixed, and would mean rewriting and
  retesting the fallback.
- **Editable `div`s instead of textareas:** it changes paste, line breaks, IME and Keyman input, and value mapping for
  every user at once.
- **Bundling the runtime (Fixed Version):** 150–250 MB extra, and we would own its security updates.

## Section 1: Host, engine choice, fallback, installer

### `WebView2HtmlHost : IHtmlHost` (new; the only file that touches WebView2 types)

- **Control and environment.**
  - Uses the WinForms `Microsoft.Web.WebView2.WinForms.WebView2` control from the `Microsoft.Web.WebView2` NuGet SDK.
  - Creates its environment with `CoreWebView2Environment.CreateAsync(null, userDataFolder)`, where the data folder is
    `%LOCALAPPDATA%\OneStory Editor\WebView2`. The default, next to the exe, is read-only under Program Files.
  - Uses the x86 `WebView2Loader.dll`, because StoryEditor is x86.
- **Settings:**
  - default context menus off
  - dev tools off except in Debug builds
  - status bar off
  - zoom on Ctrl+wheel on
  - external drops allowed (Section 3 confirms what the drag needs)
- **Page delivery.**
  - `LoadHtml(html)` navigates to `https://ose.pane/page.html?d=<docId>`.
  - A `WebResourceRequested` filter on `https://ose.pane/*` serves `LoadedHtml`, with the doc-id token substituted,
    from memory as `text/html; charset=utf-8`. So there is no 2 MB `NavigateToString` limit and no temp files.
  - `NavigationStarting` cancels any other URL.
  - `NewWindowRequested` is handled and does nothing.
- **Messages.**
  - Page → C#: `WebMessageReceived` (`TryGetWebMessageAsString`), raised on the UI thread. It goes through the same
    `HtmlMessage` parse and `Deliver` logic as `IeHtmlHost`:
    - `reply` is matched by rid
    - `ready` is gated on the current doc id
    - everything else goes to `MessageReceived`
  - C# → page: `PostWebMessageAsString(json)`.
  - The shared delivery logic moves into a small internal base class or helper that both hosts use, so the two hosts
    can't drift apart.
- **Asynchronous start-up.**
  - The control starts asynchronously. `LoadHtml`, `Post` and `Request` made before it has started are handled the same
    way as before `ready`: loads are deferred, posts are queued, requests return null.
  - `Request` pumps with the same bounded wait as `IeHtmlHost`, which the spike confirms works.
- **Keys.**
  - The page handles F5, Ctrl+F5 and Ctrl+S, as with IE.
  - `CoreWebView2Controller.AcceleratorKeyPressed` lets through Ctrl+F (find), clipboard keys, and text-editing keys.
  - It marks as handled (blocks): reload (F5 and Ctrl+R, after the page has seen them), print (Ctrl+P), Ctrl+N/T/O/W,
    Alt+Left/Right, Ctrl+U, F12 in Release, and other browser-level commands.
- **Print.** `ShowPrintPreview()` → `CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser)`.
- **Engine.** `IHtmlHost` gains `HtmlEngine Engine { get; }` (`Ie` | `WebView2`).

### Engine choice (`HtmlHostFactory`)

WebView2 is used when all of these hold:
- the setting *Use the IE browser panes* is off (the default)
- `CoreWebView2Environment.GetAvailableBrowserVersionString()` returns a version (no exception)
- the persisted flag `WebView2FailedLastRun` is false

Otherwise the factory uses IE, and the status bar shows the reason once per session, for example "Using IE panes:
WebView2 runtime not installed".

The setting is a checkable item on the Advanced menu, captioned "(takes effect after restart)". It is stored in
`Properties.Settings`.

### Automatic fallback (`FallbackHtmlHost`)

- The factory returns a `FallbackHtmlHost`. Its `Control` is a container `Panel` holding the inner host's control, and
  it forwards everything to the inner host.
- If the WebView2 inner host fails to start (environment or controller creation throws), or its browser process fails
  afterwards (`ProcessFailed`), the wrapper:
  1. sets `WebView2FailedLastRun = true` (persisted)
  2. disposes the WebView2 host
  3. creates an `IeHtmlHost` in the same container
  4. raises `EngineChanged`
- Owners rebuild their page with the IE scripts on `EngineChanged`:
  - `HtmlVerseControl` calls `LoadDocument()`.
  - `NetBibleViewer`, `HtmlForm` and `MinimalHtmlForm` re-run their loaders.
- The status bar notes the switch once.
- `WebView2FailedLastRun` is cleared when the user turns the IE setting off again, or when a WebView2 session starts
  successfully after the runtime version changes.

### Installer (WiX)

- New components:
  - `Microsoft.Web.WebView2.Core.dll`
  - `Microsoft.Web.WebView2.WinForms.dll`
  - the x86 `WebView2Loader.dll`, in the layout the SDK expects next to the exe
- `MicrosoftEdgeWebview2Setup.exe` is included. A deferred custom action runs it `/silent /install` only when neither
  runtime registry key exists (the per-machine and per-user `EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}`
  values). Its result is ignored.

## Section 2: Modern page scripts (WebView2 only)

### Picking scripts

- `PageScripts.For(HtmlEngine)` returns the engine's script set.
- Page builders take the engine from their host:
  - `StoryData.AddHtmlHtmlDocOutside`/`PresentationHtml`
  - `ConNoteHtml`/`ConsultantNotesHtml`/`CoachNotesHtml`
  - the NetBibleViewer and HtmlForm page builders
  - PrintForm and LnCNotePrintForm, via the PrintViewer pane's engine
- Modern pages start with `<!DOCTYPE html>` (standards mode).
- **Layout check:** quirks and standards mode differ in table font inheritance, box sizing and image/table spacing.
  Every pane gets a side-by-side visual comparison with IE, and any differences are fixed in the modern page's CSS
  only. `StylePrefix` gains an engine-specific addition if needed.
- jQuery isn't used by the modern scripts.

### `modern/bridge.js`

- Same `ose` API: `send`, `on`, `listen` (a thin wrapper around `addEventListener`), `cancel`, `closest`, and the
  `scrollTo` command.
- Sends with `chrome.webview.postMessage(JSON.stringify(msg))`.
- Receives through `chrome.webview.addEventListener('message', e => oseReceive(e.data))`.
- Native JSON.
- The same doc-id token, `ready`, `jsError` (`window.onerror`) and reply rules.

### `modern/PaneCommon.js`

- **Same as the IE version:**
  - the link, button and drop delegation
  - Ctrl+S / F5 / Ctrl+F5
  - `scrolled`
  - `setText`, `appendText`, `appendHtml`, `setHtml`, `hasElement`
- **Selection commands:**
  - `getSelection` returns `{selType: 'text', text}` when the focused textarea has a non-empty selection, and
    `{selType: 'none'}` otherwise.
  - `replaceSelection {id, text}` uses `setRangeText(text, start, end, 'end')` and replies `{endPoint: <selectionEnd>,
    value: ta.value}`.
  - `selectRange {id, start, length}` focuses the box and calls `setSelectionRange`.
  - `clearSelection` collapses the textarea selection and calls `window.getSelection().removeAllRanges()`.
- **Edits.**
  - `textChanged {id, value}` is sent on `input` and on keyup (with the same Ctrl+C/A/F/H/S skip list).
  - `flush` sends `textChanged {id, value, quiet: true}` for the last-focused textarea that isn't read-only.
  - The edge-line-break rule for quiet messages in C# still applies.
- **Placeholders.** The native `placeholder` attribute carries the language name. None of the IE fake-value logic is in
  the modern set.
- **`textareaMouseDown.button`.** Converted to the values C# expects: left = 1, middle = 4, right = 2.

### `modern/StoryBt.js` — highlights (the must-keep feature)

- **State.** One highlight per textarea: `{start, end, text}`, keyed by textarea id.
- **Recording.** A non-empty selection made in a box (mouseup, keyup with Shift, double-click, `select`) records that
  box's highlight and replaces any earlier one in the same box. Highlights in other boxes are kept.
- **Clearing.** A plain left-click (no selection) or any edit in a box clears that box's highlight, matching today's
  `removeSelection` on left mousedown and keyup. `clearHighlight {id}` clears it from C#.
- **Drawing (mirror).**
  - A highlighted box gets a mirror `div` inserted just before it in a relative wrapper. The mirror is absolutely
    positioned to the box's rectangle.
  - It copies the box's computed font, padding, border widths, line height, letter and word spacing, text direction,
    width, `white-space: pre-wrap` and `overflow-wrap`.
  - Its content is the box's text, HTML-escaped, with the range wrapped in `<mark class="<box class> highlight">`.
  - The box's background becomes transparent while a highlight exists. The mirror follows the box's scroll, and is
    re-laid out on `input`, resize and window resize.
  - While the box has focus, the browser's native selection is shown and the mark is hidden.
- **`getHighlights {tableId}`.**
  1. Records the focused box's current selection first, as IE's `TriggerMyBlur` did.
  2. Replies `{items: [{textareaId, className: box.className + ' highlight', text}]}` for each highlighted box inside the
     table, in document order.

  `HighlightedText`, `ReferringTextBuilder` and `AddNote` are unchanged, so note text comes out the same as with IE.
- **Double-click.** The browser's word selection, trimmed of a trailing space as the old code did.
- **Anchors and line options.** Delegated through `data-mouseup`, as in IE (W3C `button == 2` is right-click).
- **Other events:** `focus`, `blur`, `contextMenu {id}` (from `contextmenu`, with the default prevented),
  `textareaMouseUp`, and `mouseMove` (throttled to 100 ms).

### `modern/ConNote.js`

- **`getNoteSelection`.**
  - Uses `window.getSelection()`. When the range isn't collapsed, it walks up from the range's common ancestor to the
    conversation's table row and reads the line index from the first `[id^="tp_"]` id inside it (`tp_<line>_…`).
  - Replies `{lineIndex, html}`, where `html` is the range's cloned contents serialised.
  - Replies `{}` otherwise.
- **Ctrl+B / Ctrl+I.** Wrap the textarea selection with `$…$` / `*…*` using `setRangeText`, with today's
  starts-with/ends-with check.
- **Right-click.** `contextmenu` sends `contextMenu {}`.
- **Note box.** `textChanged` on `input`/keyup; `textareaMouseDown` with the converted button value; double-click on
  `[data-note]` uses the browser's word selection.

### `modern/NetBible.js`, `modern/HoverLinks.js`

- The same messages (`hoverRef`, `refMouseDown`/`refMouseUp`/`refMouseOut`).
- **Bible-reference drag (the spike decides which):**
  - **Preferred:** verse buttons are `draggable="true"` and set `text/plain` to the reference on `dragstart`. A drop on a
    pane's `[data-drop="scripture"]` element works through HTML5 drag-and-drop between the two WebView2 controls. The
    target sends `scriptureDropped {id}`, and C# uses its current Bible reference as now. `refMouseOut` no longer
    starts a WinForms drag in WebView2.
  - **Fallback:** keep `refMouseOut` → WinForms `DoDragDrop`, but with `new DataObject(DataFormats.UnicodeText, ref)`
    in place of the control object, so Chromium accepts the drop.

### The only C# protocol change

`HtmlVerseControl.SetSelectedText` accepts a `value` field in the `replaceSelection` reply, used as is. When the reply
has `ieHtml` instead, it goes through `FromIeHtmlText` as now. Everything else (`textChanged` with `value`,
`getHighlights` items, `getNoteSelection`, actions) is already engine-neutral.

## Section 3: Spike, testing, rollout

### Spike (task 1; throwaway, in the scratchpad)

Each question is answered in the plan's spike report:
1. Does a pumping `Request` receive `WebMessageReceived` replies, the same pattern IE uses?
2. Does serving the page from memory through `WebResourceRequested` load a page larger than 2 MB, with UTF-8 intact?
3. Drag:
   - Does an HTML5 drag between two WebView2 controls fire `drop` on the target?
   - Does a WinForms `DoDragDrop` with `UnicodeText` reach a WebView2 page's `drop`?
4. Do an unwritable user-data folder and a missing runtime raise exceptions we can catch, so the fallback can work?
5. Mirror highlight: does it line up with the textarea text across wrapping, scrolling, mixed scripts (for example
   Devanagari) and RTL?

If an answer is "no", the plan uses the fallback named above, and this spec gets an amendment.

### Automated tests

- The browser page fixtures (bridge, PaneCommon, small hosts, note page, Story/BT page) become **engine-parameterised**
  and run against `IeHtmlHost` and `WebView2HtmlHost`. WebView2 cases report Inconclusive when the runtime is missing.
- The modern Story/BT selection test checks that a programmatic selection becomes a highlight, clears, and gives the
  same `getHighlights` items as IE. A mirror layout test checks the mirror's text lines up with the textarea text.
- `FallbackHtmlHost` unit tests with fake inner hosts:
  - start-up failure → swap to IE, `EngineChanged`, page replayed, failure flag recorded
  - process failure after start → the same
- `HtmlHostFactory` decision tests: setting, runtime present or absent, and the failed-last-run flag.
- `SetSelectedText` handles both `value` and `ieHtml` replies.
- **Guard test, extended:**
  - Microsoft.Web.WebView2 types only in `WebView2HtmlHost.cs`.
  - `chrome.webview` only in `js/modern/bridge.js`.
  - `window.external` only in `js/bridge.js`.
  - No `document.selection`, `createTextRange`, `attachEvent`, `fireEvent` or `window.event` anywhere in `js/modern/`.

### Manual checks

- B's smoke checklist, run on **both** engines.
- A side-by-side layout comparison of every pane and popup (WebView2 vs IE).
- Fallback: rename the WebView2 runtime folder or point the data folder at an unwritable path, then confirm the status
  bar notice and that the IE panes work.
- The *Use the IE browser panes* setting round trip.
- The installer on a machine without the runtime, both online (runtime installs) and offline (install completes; app
  uses IE).

### Rollout

- WebView2 is the default where it works, with automatic fallback to IE and the IE setting to switch back.
- The IE scripts are untouched.
- No data or file-format change.

### Task order (for the plan)

1. Spike
2. `WebView2HtmlHost`, the shared delivery helper, `FallbackHtmlHost`, the factory, the setting
3. Modern bridge and PaneCommon, plus engine-parameterised test fixtures
4. Modern NetBible and HoverLinks pages and the Bible-reference drag
5. Modern note pages
6. Modern Story/BT page with the mirror highlight
7. Installer (SDK DLLs and bootstrapper custom action)
8. Guard tests and cleanup

## Out of scope

- Removing IE or its scripts (a later sub-project, once WebView2 has proven itself).
- .NET 8 (sub-project D) and cross-platform UI.
- The revision history performance issue (`HtmlDisplayForm` `GetAllRevisions` blocking the UI). Noted separately.
- Localizing the new strings. This happens in the normal localization pass before release.

## Risks

- **Standards-mode layout differences.** Covered by the visual comparison; fixes stay in the modern CSS.
- **Mirror highlight fidelity** with complex scripts and Keyman. The spike checks it first; the mirror copies all
  computed text metrics.
- **Drag-and-drop between the Bible pane and the panes.** The spike picks HTML5 drag or a WinForms text `DataObject`.
- **Focus, Tab and keyboard (Keyman) across the WinForms/WebView2 process boundary.** On the manual checklist.
  `MoveFocusRequested` is handled if Tab gets stuck.
- **`Request` pumping while WebView2 runs in another process.** The spike checks it; the existing timeouts and the
  flush's Retry prompt are the safety net.
