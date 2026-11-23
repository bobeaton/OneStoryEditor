# Task 9 report

## Implemented
- `StoryEditor.Tests/ArchitectureGuardTests.cs`: added `OnlyWebView2HtmlHostTouchesWebView2`, `OnlyTheBridgesUseChromeWebview` (excludes `js\bridge.js` and `js\modern\bridge.js` by full-path match; scans only the StoryEditor source dir), `ModernScriptsUseNoIeOnlyApis`. `OnlyTheBridgeScriptUsesWindowExternal` now excludes only `js\bridge.js` (modern bridge not excluded).
- Spec: added "Refinements made while planning and implementing" section (brief's list plus the controller's additions).
- Commit 5ac3e20 "C9: guard tests for the WebView2 boundaries; spec refinements" (+ Co-Authored-By Claude Sonnet 5.5).

## Guard results
All four guards passed first time; no offenders, no exclusions added beyond the two bridge files.

## Clean build and tests
Deleted only StoryEditor/obj and StoryEditor.Tests/obj (dry-run first). Rebuilt into the scratch OutDir. Full run: 434 total, 434 passed (431 baseline + 3 new guards). WebView2 cases ran.
Note: the runner printed one "failure: COMException ... 0x80080005" line during UnusableUserDataFolder_RaisesFailed; that is the expected logged start-up failure, and the test passed.

## Manual checklist (run on both engines)
Spike (unlocked desktop; harness in scratchpad wv2spike\bin\Debug, rebuild from task-1 report if gone):
- Run `Wv2Spike.exe q3a`, `q3b`, `q6`; read log-q3a.txt / log-q3b.txt / the q6 log. If q3a shows no `drop`, switch to the WinForms DataObject fallback.
- Q6: record F5, Ctrl+R, Ctrl+P, Ctrl+F, Alt+Left separately.

Every pane and popup, side by side WebView2 vs IE: layout, fonts, table spacing. Note any difference as a CSS fix for the modern pages.

B's smoke checklist on WebView2:
- typing and Ctrl+S; close right after typing; autosave
- line breaks survive save and reload
- highlight in two columns, then Add note; highlights clear
- Find/Replace in the note panes, then Add note (stale highlight regression)
- Copy / Cut / Paste selected
- concordance; L&C note lookup; move selected text to a new line
- every note button
- Bible references: links and anchors (left click jumps; right click opens the menu)
- drag a verse from the Bible pane onto an anchor cell AND onto a note box (real drag in WebView2)
- F5 and Ctrl+F5; Ctrl+F (find bar opens); Keyman switching; Tab
- TextPaster
- find/replace in the note panes
- revision history; swap columns; print preview and save HTML
- the commentary popup; the tooltips

Fallback:
- Point the WebView2 data folder at a read-only path, or rename the runtime folder. Start: status bar says IE panes are in use, and the panes work.
- With WebView2 running, kill `msedgewebview2.exe`. Panes reload in IE with a status note.
- Restart: starts in IE (failure remembered). Toggle *Use the IE browser panes* off and restart: tries WebView2 again.

Installer:
- Check the installed `Microsoft.Web.WebView2.Core.dll` is version 1.0.4191.47.
- Machine/VM without the runtime, online: runtime gets installed; app uses WebView2.
- Offline: install completes; app uses IE.

## Concerns
- Q3 drag and Q6 keys remain unverified until the spike is run on an unlocked desktop.
- Line-ending warnings (LF to CRLF) from git on the two edited files; harmless.

## Fix round 1
- RegexIeOnlyJs extended (detachEvent, createControlRange, setCapture, currentStyle, document.all, clipboardData, showModalDialog, onpropertychange, srcElement, cancelBubble, returnValue, bare `event.`, window.event / window["event"], word boundaries). Grep of js/modern showed no genuine offenders or false positives.
- ModernScriptsUseNoIeOnlyApis also scans .htm/.html under js\modern (none exist today; commented) and asserts bridge.js exists and more than one modern file was found.
- New TheGuardsHaveSomethingToScan asserts both bridges and WebView2HtmlHost.cs exist.
- IsBridge now matches the path tail \js\bridge.js / \js\modern\bridge.js, case-insensitive.
- Spec drag bullet now says provisional Q3 choice, with the DataObject fallback if q3a shows no drop.
- Full suite: 435/435 passed.
