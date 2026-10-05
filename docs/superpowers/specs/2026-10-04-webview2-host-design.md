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
    `%LOCALAPPDATA%\SIL\OneStory Editor\WebView2` (failures are logged beside it, in `WebView2-failures.log`). The default, next to the exe, is read-only under Program Files.
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
  - *(Amended by the spike, see "Spike findings", Q6.)* `WebView2HtmlHost` hands each received message to the shared
    channel with `BeginInvoke`, as `IeHtmlHost` does, rather than inline from the `WebMessageReceived` handler. While a
    `WebMessageReceived` handler is running, WebView2 raises no further `WebMessageReceived` for any control, even when
    the code pumps with `DoEvents`. So a `Request` made inside that handler always times out.
  - C# → page: `PostWebMessageAsString(json)`.
  - The shared delivery logic moves into a small internal base class or helper that both hosts use, so the two hosts
    can't drift apart.
- **Asynchronous start-up.**
  - The control starts asynchronously. `LoadHtml`, `Post` and `Request` made before it has started are handled the same
    way as before `ready`: loads are deferred, posts are queued, requests return null.
  - `Request` pumps with the same bounded wait as `IeHtmlHost`, which the spike confirms works when the
    `Request` is made from outside a `WebMessageReceived` handler (see "Messages" above and the spike findings, Q6).
- **Keys.**
  - The page handles F5, Ctrl+F5 and Ctrl+S, as with IE.
  - *(Amended by the spike, see "Spike findings", Q6; provisional until `Wv2Spike.exe q6` runs on an unlocked desktop.)*
    `AcceleratorKeyPressed` filtering is **not** used, because the WinForms `WebView2` control doesn't expose the
    `CoreWebView2Controller` event. Instead:
    - Browser accelerator keys stay enabled, so Ctrl+F (find), clipboard keys and text-editing keys work.
    - The modern page's `keydown` handler calls `preventDefault()` for F5, Ctrl+R and Ctrl+P, after handling its own
      F5/Ctrl+F5.
    - The `NavigationStarting` guard cancels any reload or back/forward navigation that still gets through, such as
      Alt+Left/Right. This is verified for programmatic and page-initiated back/forward and reload.
    - Whether Ctrl+F opens the find bar is a manual-check item.
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
- **Drag-and-drop between the Bible pane and the panes.** See the spike findings, Q3. Provisional
  choice: HTML5 drag, with a WinForms text `DataObject` as the fallback.
- **Focus, Tab and keyboard (Keyman) across the WinForms/WebView2 process boundary.** On the manual checklist.
  `MoveFocusRequested` is handled if Tab gets stuck.
- **`Request` pumping while WebView2 runs in another process.** The spike checks it; the existing timeouts and the
  flush's Retry prompt are the safety net.

## Spike findings (Task 1 of the plan)

Setup:
- SDK `Microsoft.Web.WebView2` 1.0.4258.31, the latest stable release on 2026-10-04.
- Evergreen runtime 154.0.4258.53, Windows 11, x86 WinForms harness.
- The x86 build places `Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll` and the x86
  `WebView2Loader.dll` next to the exe.
- The throwaway harness is in the session scratchpad:
  `C:\Users\pete_\AppData\Local\Temp\claude\c--Users-pete--source-repos-OneStoryEditor\2363c84b-2c3f-4dd6-936b-1ad00b798e87\scratchpad\wv2spike\`
  (`Wv2Spike.exe` in `bin\Debug`). That folder belongs to one session. If it is gone, rebuild the harness from the
  description in `.superpowers/sdd/2026-10-04-webview2-host/task-1-report.md`: an SDK-style net48 x86 WinForms exe with
  two `WebView2` controls, pages served from `https://ose.pane/*`, and one mode per question.
- The real-input runs (Q3, and the key part of Q6) could not run: the Windows session was disconnected, and injected
  mouse and keyboard input reaches no window in that state. Those answers are **provisional** until someone runs them
  on an unlocked desktop.

- **Q1: a pumping `Request` gets `WebMessageReceived` replies. Yes.**
  - Evidence: from a button `Click` handler, 50 of 50 requests got their reply and none timed out.
  - With `Application.DoEvents(); Thread.Sleep(1)`, the median round trip was 32 ms (min 14, max 35). With `Sleep(0)` the
    median was 0.2 ms. So the round trip itself is under a millisecond, and the 32 ms is `Sleep(1)`'s timer granularity.
  - Consequence: `Request` keeps the IE pattern. Pumping with `Sleep(0)` for the first few milliseconds is an optional
    speed-up.
- **Q2: a page over 2 MB served from memory, with UTF-8 intact. Yes.**
  - Evidence: 200,000 copies of U+0915 U+093F U+0924 U+093E U+092C, 3.2 MB of UTF-8, served through
    `WebResourceRequested`. The page loaded about 130 ms after navigation.
  - `innerText.length` was 1,199,999, as expected, and substrings at the start, middle and end matched exactly.
  - `innerText` on that one giant paragraph took 15–27 s, because of layout and shaping. Split into paragraphs, it took
    25 ms, and the page loaded in 1.2 s. Tests shouldn't read `innerText` of huge single-block pages.
  - **Consequence:** the `WebResourceRequested` handler serves only `page.html` and returns **404 for any other request**.
    The browser asks for `https://ose.pane/favicon.ico` on every load.
- **Q3: drag. Provisional, not verified (the desktop was locked).**
  - **Decision:** the mechanism is HTML5 drag-and-drop between the WebView2 controls.
    - Verse buttons are `draggable="true"` and set `text/plain` to the reference on `dragstart`.
    - The page reports `refDragStart` / `refDragEnd`.
    - The target's `[data-drop="scripture"]` element sends `scriptureDropped {id}` on `drop`.
  - **Documented fallback:** `refMouseOut` → WinForms
    `DoDragDrop(new DataObject(DataFormats.UnicodeText, ref), Copy | Link)`, as in Section 2.
  - To confirm, run `Wv2Spike.exe q3a` (HTML5) and `Wv2Spike.exe q3b` (WinForms text `DataObject`) from the harness
    folder above on an unlocked desktop, then read `log-q3a.txt` / `log-q3b.txt`.
  - If q3a shows no `drop`, Task 5 switches to the fallback.
- **Q4: start-up failures are catchable, but one case hangs silently.**
  - **Missing runtime** (`WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` set to an empty folder, in the harness process only):
    `GetAvailableBrowserVersionString()` *throws* `WebView2RuntimeNotFoundException`; it does not return null.
    `CreateAsync` throws the same exception within 4 ms.
  - **User-data folder is a file, or is unwritable** (Program Files, non-admin): `CreateAsync` *succeeds*.
    `EnsureCoreWebView2Async` throws `COMException` 0x80080005 (`CO_E_SERVER_EXEC_FAILURE`) after about 150 ms.
    `CoreWebView2InitializationCompleted` fires with `IsSuccess == false` and the same exception.
  - **In 1 of 7 bad-folder runs nothing happened for 30 s**: no exception and no event. Disposing the control then gave
    `COMException` 0x80004004 (`E_ABORT`).
  - A normal start takes about 300 ms, but 2 of about 14 runs took about 6.5 s.
  - **Decision (Task 2):**
    - The factory's probe treats an exception from `GetAvailableBrowserVersionString()` as "runtime not installed".
    - `WebView2HtmlHost` treats as start-up failure: **any `Exception`** from `CreateAsync` or `EnsureCoreWebView2Async`;
      `CoreWebView2InitializationCompleted` with `IsSuccess == false`; and **a 20 s start watchdog** expiring.
    - Each of these leads to the IE fallback and sets `WebView2FailedLastRun`.
- **Q5: the mirror highlight lines up. Yes.**
  - Setup: a 300 px textarea with mixed English and Devanagari over three wrapped lines, then Arabic, scrollable. A
    second `dir="rtl"` box with Arabic. Each box has a highlight that spans a line wrap.
  - Evidence:
    - The textarea and mirror `scrollHeight`s are equal (146/146 and 66/66, and still equal at 200 % zoom).
    - The mirror's `scrollTop` follows the box.
    - An overlay (mirror text red under 55 % blue textarea text) blends to clean purple, with no fringes before or after
      scrolling.
    - The native selection of the same range covers exactly the area of the `<mark>`, in LTR and RTL, on both sides of
      the wrap.
  - **Consequence:** the mirror must reserve the textarea's scrollbar width (`offsetWidth - clientWidth - borders`).
    Add it to the mirror's right padding, or to its **left padding in RTL**, where Chromium puts the scrollbar.
  - Keep the "trailing newline → extra space" rule, so the scroll heights stay equal.
- **Q6: keys provisional; navigation guard and nested pumping verified.**
  - **Navigation guard (verified).** With `NavigationStarting` cancelling, the guard caught every case below, and the
    page stayed put with no new `ready`:
    - `CoreWebView2.GoBack()` and the page's `history.back()` (`NavigationKind.BackOrForward`)
    - the page's `location.reload()` and the host's `Reload()` (`Reload`)

    A cancelled navigation can still raise a `WebResourceRequested` for the page. That is harmless.
  - **Keys (provisional, not verified).** The design keeps it simple:
    - The page's `keydown` handler calls `preventDefault()` for F5, Ctrl+R and Ctrl+P.
    - The `NavigationStarting` guard is the backstop for reload and Alt+Left/Right.
    - This replaces the `AcceleratorKeyPressed` filtering in Section 1, because the WinForms control doesn't expose the
      controller event.

    To confirm key delivery, run `Wv2Spike.exe q6` on an unlocked desktop. Record each key on its own: F5, Ctrl+R,
    Ctrl+P, Ctrl+F and Alt+Left. For each, note whether the page saw it, whether `preventDefault` stopped it, and
    whether it raised `NavigationStarting` or `beforeprint`. **Whether Ctrl+F opens the find bar is a manual-check
    item.**
  - **Request inside `WebMessageReceived` (verified): it times out.**
    - A `Request` made inside a `WebMessageReceived` handler timed out after 2 s, and so did one to the other control.
    - The replies arrived as soon as the handler returned.
    - The same `Request` deferred with `BeginInvoke` got its reply in 13.5 ms.
    - **Decision:** Section 1's "Messages" is amended. `WebView2HtmlHost` hands each received message to the shared
      channel with `BeginInvoke`.
    - No `Request` is made from inside any `CoreWebView2` event handler.

## Refinements made while planning and implementing

What changed from the spec text above.

**Spike status**
- Q3 (drag) and the key part of Q6 are provisional, because the desktop was locked during the spike. Someone must run
  `Wv2Spike.exe q3a`, `q3b` and `q6` on an unlocked desktop. The harness is in the session scratchpad; rebuild it from
  `.superpowers/sdd/2026-10-04-webview2-host/task-1-report.md` if it is gone.

**Keys, drag, tests, gate**
- Keys use page-level `preventDefault` plus the navigation guard, not `AcceleratorKeyPressed` (the WinForms control
  doesn't expose the controller event; Task 1 Q6).
- The Bible-reference drag uses HTML5 drag-and-drop (`refDragStart` / `refDragEnd`, `scriptureDropped` on drop), the
  provisional mechanism Task 1 Q3 chose (the real-input run is still pending). If the `q3a` run shows no `drop`, switch
  to the fallback: `refMouseOut` and a WinForms text `DataObject`. `HtmlDragSuspension` keeps
  `SuspendSaveDialog` balanced for HTML5 drags.
- The modern page tests are separate `Modern*` fixtures, not engine parameters on the IE fixtures. The IE fixtures keep
  their quirks-mode drivers.
- `HtmlHostFactory.UseWebView2WhenAvailable` gated the switch-over until the modern pages were complete; it is now on.

**`WebView2HtmlHost`**
- Received messages are delivered through `BeginInvoke` (a `Request` inside `WebMessageReceived` times out).
- The navigation guard allows the expected URL once; a reload is cancelled.
- Pages are served only for the current `?d=` document id; any other request gets a 404.
- `Failed` is raised asynchronously.
- `ProcessFailed` is fatal only for `BrowserProcessExited` and `RenderProcessExited`.

**Installer**
- The WebView2 SDK DLLs come from the Encoding Converters `EC_40_MM` merge module (1.0.4191.47). StoryEditor's
  `PackageReference` is pinned to that version.
- `HtmlHostFactory` falls back to IE if the WebView2 assembly can't load.
- Newtonsoft.Json is supplied by the `EcTranslatorsMM` merge module (verified in the MSI).

**Modern `StoryBt`**
- The mirror reserves the scrollbar width (on the right; on the left for RTL).
- `clearHighlight` also collapses the box selection.
- Script value changes (`setText`, `appendText`, `replaceSelection`) drop the box highlight.
- Highlights store the box value and are dropped if it changed.

**Guard rules (Task 9)**
- `Microsoft.Web.WebView2` types only in `WebView2HtmlHost.cs`.
- `chrome.webview` only in `js/bridge.js` (B's host-detection branch, untouched) and `js/modern/bridge.js`.
- `window.external` only in `js/bridge.js`.
- No IE-only DOM APIs in `js/modern/`.
