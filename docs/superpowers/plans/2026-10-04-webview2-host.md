# WebView2 Panes (Sub-project C) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** StoryEditor's HTML panes and popups run in WebView2 (Chromium, Evergreen runtime) by default, fall back to the
existing IE host automatically, and keep every user-visible behaviour (highlights → "Add note on selected text", line
breaks, drag-and-drop, keys).

**Architecture:**
- A new `WebView2HtmlHost` implements B's `IHtmlHost`. Message bookkeeping moves out of `IeHtmlHost` into a shared
  `HtmlMessageChannel`, so the two hosts behave identically.
- `HtmlHostFactory` decides which engine to use. Whenever it picks WebView2, it wraps the host in a `FallbackHtmlHost`
  that swaps to IE if WebView2 fails.
- Page builders pick a script set per engine. IE keeps today's scripts untouched; WebView2 gets new standards-mode
  scripts in `js/modern/` that speak the same protocol. The Story/BT highlight in WebView2 is drawn with a mirror layer
  behind a transparent textarea.

**Tech Stack:**
- C# / .NET Framework 4.8 WinForms (SDK-style, x86)
- Microsoft.Web.WebView2 NuGet SDK (WinForms control, Evergreen runtime)
- the existing IE host
- Newtonsoft.Json
- NUnit 3: `StoryEditor.Tests`, 348 tests at start, with browser fixtures run in an off-screen form
- WiX 6 (`Installer/Setup OneStoryEditor`)

**Spec:** `docs/superpowers/specs/2026-10-04-webview2-host-design.md`. Read it in full. B's spec
(`docs/superpowers/specs/2026-10-04-html-message-protocol-design.md`) defines the message protocol, which is unchanged.

## Global Constraints

- **Branch and commits.** Branch `DecoupleWebBrowser`. Commit after each task. End each commit with a
  `Co-Authored-By:` trailer naming the model that wrote it.
- **The IE path is untouched.** Don't change `js/bridge.js`, `js/PaneCommon.js`, `js/StoryBt.js`, `js/StoryBtPs.js`,
  `js/ConNoteDomPrefix.js`, `js/NetBible.js`, `js/HoverLinks.js`, or `IeHtmlHost`'s behaviour. The one exception is
  moving shared bookkeeping into `HtmlMessageChannel` in Task 2; the IE host tests must stay green.
- **No data or file-format change.**
- **The C# message protocol is unchanged, except one addition.** `HtmlVerseControl.SetSelectedText` accepts a `value`
  field in the `replaceSelection` reply, used as is, as well as today's `ieHtml` field.
- **Default engine:** WebView2 where its runtime works, with automatic fallback to IE. The setting *Use the IE browser
  panes* (`Properties.Settings.Default.UseIeBrowserPanes`, default `false`) forces IE after a restart.
- **WebView2 user-data folder:** `%LOCALAPPDATA%\OneStory Editor\WebView2` (tests use a temp folder through
  `HtmlHostOptions.UserDataFolder`).
- **Page address:** pages are served from memory at `https://ose.pane/page.html?d=<docId>`; any other navigation is
  cancelled.
- **Where things may appear** (the guard test enforces this in Task 9):
  - `Microsoft.Web.WebView2` types only in `StoryEditor/WebView2HtmlHost.cs`
  - `chrome.webview` only in `js/modern/bridge.js`
  - `window.external` only in `js/bridge.js`
  - no `document.selection`, `createTextRange`, `attachEvent`, `fireEvent` or `window.event` in `js/modern/`
- **`HtmlHostFactory.UseWebView2WhenAvailable` stays `false` until Task 7's last step** flips it on. Before then the app
  always uses IE, because the modern pages aren't complete.
- **Editing rules.**
  - Edit with the Write/Edit tools, never shell heredocs/sed/python. In this environment `\uXXXX` escapes have been
    turned into literal characters by the tools, so build any non-ASCII test strings from code points (see
    `HtmlMessageTests.AwkwardNonAscii`).
  - After writing C# or JS, check your added lines for unexpected non-ASCII.
  - Use Bash only for git, MSBuild, vstest and grep.
  - Large files (`StoryEditor.cs`, `StoryData.cs`, `HtmlStoryBtControl.cs`, `HtmlConNoteControl.cs`,
    `NetBibleViewer.cs`, `Resources.resx`, `Resources.Designer.cs`, the `.wxs` files): Grep `-n`, then read only the
    regions you need.
- **Build and test** (Git Bash). Always build into the scratch OutDir, so the user's Visual Studio doesn't lock files:
  ```bash
  S="C:/Users/pete_/AppData/Local/Temp/claude/c--Users-pete--source-repos-OneStoryEditor/2363c84b-2c3f-4dd6-936b-1ad00b798e87/scratchpad/build"
  MSBUILD="/c/Program Files/Microsoft Visual Studio/18/Insiders/MSBuild/Current/Bin/MSBuild.exe"
  VSTEST="/c/Program Files/Microsoft Visual Studio/18/Insiders/Common7/IDE/Extensions/TestPlatform/vstest.console.exe"
  "$MSBUILD" "StoryEditor 2017.sln" -t:StoryEditor_Tests -restore -p:Configuration=Debug -p:Platform=x86 -v:m -nologo "-p:OutDir=$S/"
  "$VSTEST" "$S/StoryEditor.Tests.dll" /Platform:x86            # add /Tests:<Fixture> for one fixture
  ```
- **Tests never block on UI.** Browser tests use an off-screen form and pump with a timeout. WebView2 tests call
  `Assert.Inconclusive` when the runtime is missing (`WebView2HtmlHost.RuntimeVersion == null`).

## Review Focus

These are the inputs most likely to bite a user that no single unit test covers. Each line names the test that pins it:

1. **WebView2 never finishes starting.** It hangs without throwing, which leaves blank panes forever. Expected: a
   start-up watchdog treats it as a failure, and `FallbackHtmlHost` swaps to IE. Pinned by
   `WebView2HtmlHostTests.UnusableUserDataFolder_RaisesFailed` and
   `FallbackHtmlHostTests.PrimaryFails_SwapsToFallback_ReplaysPage_RaisesEngineChanged` (Task 2 and Task 3).
2. **Browser back/forward, a script navigation, or a link reaches an older page or an outside URL.** The pane would
   then show a stale story. Expected: the host cancels every navigation it didn't start. Pinned by
   `WebView2HtmlHostTests.NavigationNotStartedByHost_IsCancelled` (Task 2).
3. **Non-ASCII story text served through `WebResourceRequested` comes out garbled.** This includes Devanagari,
   combining marks, and characters outside the Basic Multilingual Plane. Pinned by
   `WebView2HtmlHostTests.PageText_RoundTripsNonAscii` (Task 2).
4. **A box's text is edited after it was highlighted,** so "Add note" would quote text that's no longer there.
   Expected: any edit clears that box's highlight. Pinned by `ModernStoryBtPageTests.Edit_ClearsTheBoxHighlight`
   (Task 7).
5. **The engine swaps in the middle of a session while a `Request` is waiting** (for example a flush). Expected: the
   request returns null (the flush's Retry prompt covers it), and the page is replayed in IE. Pinned by
   `FallbackHtmlHostTests.RequestDuringSwap_ReturnsNull` (Task 3).

## File map

| File | Responsibility | Task |
|---|---|---|
| `StoryEditor/HtmlMessageChannel.cs` (new) | Doc ids, pending posts, rid/reply matching, the `ready` gate, the bounded `Request` wait. Shared by both hosts | 2 |
| `StoryEditor/IHtmlHost.cs` | Adds `HtmlEngine`, `IHtmlHost.Engine`, `IHtmlHost.EngineChanged`, `HtmlHostOptions.UserDataFolder` | 2 |
| `StoryEditor/IeHtmlHost.cs` | Uses `HtmlMessageChannel`; `Engine => Ie` | 2 |
| `StoryEditor/WebView2HtmlHost.cs` (new) | The WebView2 host; the only file that touches WebView2 types | 2 |
| `StoryEditor/FallbackHtmlHost.cs` (new) | Wrapper that swaps a failed WebView2 host for IE | 3 |
| `StoryEditor/HtmlHostFactory.cs` | Engine decision, failure memory, once-per-session notice | 3 |
| `StoryEditor/Properties/Settings.settings`, `Settings.Designer.cs` | `UseIeBrowserPanes`, `WebView2FailedRuntimeVersion` | 3 |
| `StoryEditor/StoryEditor.cs` | Advanced-menu item; status-bar notice | 3 |
| `StoryEditor/PageScripts.cs` | `PageScripts.For(engine)` returns a `PageScriptSet`; `ApplyDoctype` | 4 |
| `StoryEditor/StoryData.cs`, `NetBibleViewer.cs`, `HtmlForm.cs`, `PrintForm.cs`, `LnCNotePrintForm.cs`, panes | Engine-aware page building; rebuild on `EngineChanged` | 3, 4 |
| `StoryEditor/js/modern/bridge.js`, `PaneCommon.js` (new) | Modern transport and shared pane handlers | 4 |
| `StoryEditor/js/modern/NetBible.js`, `HoverLinks.js` (new) | Modern Bible-pane and popup pages; HTML5 drag | 5 |
| `StoryEditor/js/modern/ConNote.js` (new) | Modern note pages | 6 |
| `StoryEditor/js/modern/StoryBt.js` (new) | Modern Story/BT page with the mirror highlight | 7 |
| `Installer/Setup OneStoryEditor/OseComponents.wxs`, `Product.wxs` | WebView2 SDK DLLs, bootstrapper custom action, Newtonsoft check | 8 |
| `StoryEditor.Tests/ArchitectureGuardTests.cs` | Extended rules | 9 |

---

### Task 1: Spike (throwaway): answer the spec's five questions, plus keys

**Files:**
- Create (scratchpad only, never committed): `<scratchpad>/wv2spike/` containing a small WinForms test harness
  (`Wv2Spike.csproj`, SDK-style, net48, x86, with a `PackageReference` to `Microsoft.Web.WebView2`, latest stable)
- Modify: `docs/superpowers/specs/2026-10-04-webview2-host-design.md` (append a "Spike findings" section)

**Interfaces:**
- Produces: written answers that Tasks 2, 5 and 7 rely on. If an answer is "no", it records the fallback chosen and
  amends the spec section it affects.

- [ ] **Step 1: Build the harness.**

  One form, two WebView2 controls side by side. Each is created with
  `CoreWebView2Environment.CreateAsync(null, <temp folder>)` and `EnsureCoreWebView2Async(env)`.

  Page 1 is served by a `WebResourceRequested` handler for `https://ose.pane/*` (`AddWebResourceRequestedFilter`). It
  contains a script that `chrome.webview.postMessage`s replies to `message` events.

- [ ] **Step 2: Q1, message timing.**

  From a button click, post a request with `PostWebMessageAsString`. Then loop on `Application.DoEvents()` plus
  `Thread.Sleep(1)` until the page's reply arrives through `WebMessageReceived`, with a 2 s limit. Record yes/no and the
  typical latency.

- [ ] **Step 3: Q2, large pages.** Serve a 3 MB page with Devanagari text in it (build it from code points: U+0915
  U+093F U+0924 U+093E U+092C), and have the page post back `document.body.innerText.length` and a substring. Record
  whether it loads and whether the text matches.

- [ ] **Step 4: Q3, drag.**
  - (a) In page 1, a `draggable` button sets `text/plain` on `dragstart`. Page 2 has a `div` that cancels `dragover`
    and posts on `drop`. Drag with a real mouse, using `SetCursorPos` plus `mouse_event`, the way B's drag probe did.
    Record whether page 2 gets `drop`.
  - (b) Start a WinForms `DoDragDrop(new DataObject(DataFormats.UnicodeText, "Gen 1:1"), DragDropEffects.Copy |
    DragDropEffects.Link)` from a button on the form, and drag it onto page 2. Record whether page 2 gets `drop`.

- [ ] **Step 5: Q4, start-up failure.**
  - (a) Use a *file* path as the user-data folder.
  - (b) Set the `WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` environment variable to an empty temp directory, which simulates a
    missing runtime.

  For each, record what `CreateAsync` / `EnsureCoreWebView2Async` throws, or whether `CoreWebView2InitializationCompleted`
  reports `IsSuccess == false`, or whether nothing happens. If nothing happens, the watchdog in Task 2 is essential;
  record how long you waited.

  Also record what `CoreWebView2Environment.GetAvailableBrowserVersionString()` returns or throws in case (b).

- [ ] **Step 6: Q5, mirror highlight.** Put a 300 px wide textarea in a page, with the mirror technique from spec
  Section 2. Copy `js/modern/StoryBt.js`'s `drawMirror` from Task 7 of this plan, or write the equivalent.
  - Fill it with three wrapped lines of mixed English and Devanagari, then a line of Arabic (`dir="rtl"`).
  - Highlight a range that spans a line wrap.
  - Screenshot it (`CoreWebView2.CapturePreviewAsync`) to `<scratchpad>/wv2spike/mirror.png`, and look at it.
  - Record whether the mark lines up with the textarea text, including after scrolling the textarea.

- [ ] **Step 7: Q6, keys.**
  - With browser accelerator keys left enabled, and a page `keydown` listener that calls `preventDefault()` for F5,
    Ctrl+R and Ctrl+P, use `SendKeys` (or `keybd_event`) to send F5, Ctrl+R, Ctrl+P, Ctrl+F and Alt+Left.
  - Record which reach the page, which the page can stop, whether Ctrl+F opens the find bar, and whether Alt+Left
    raises `NavigationStarting` (so the host can cancel it).
  - Also check that a `WebMessageReceived` handler can run a nested `DoEvents` loop. A Request made while handling a
    message must not deadlock.

- [ ] **Step 8: Record the findings.**

  Append to the spec:

  ```markdown
  ## Spike findings (Task 1 of the plan)
  ```

  Give one bullet per question: the answer, the evidence (numbers, exception types, screenshot observations), and the
  consequence.

  Then decide, in the spec text itself:
  - **Q3:** which drag mechanism (HTML5 if (a) works, otherwise the WinForms text `DataObject` if (b) works). If neither
    works, mark Task 5's drag step "blocked" and stop for the controller.
  - **Q4:** the exact failure signal(s) Task 2 must catch.
  - **Q6:** whether page-level `preventDefault` plus the navigation guard is enough. That is this plan's design, which
    replaces the spec's `AcceleratorKeyPressed` filtering, because the WinForms control doesn't expose the controller
    event. If Ctrl+F doesn't open find, say so; it becomes a manual-check item.

- [ ] **Step 9: Commit the spec amendment only** (the harness stays in the scratchpad).

  ```bash
  git add docs/superpowers/specs/2026-10-04-webview2-host-design.md
  git commit -m "C1: spike findings for the WebView2 host"
  ```

---

### Task 2: `HtmlMessageChannel`, engine on `IHtmlHost`, `WebView2HtmlHost`

**Files:**
- Create: `StoryEditor/HtmlMessageChannel.cs`, `StoryEditor/WebView2HtmlHost.cs`
- Modify: `StoryEditor/IHtmlHost.cs`, `StoryEditor/IeHtmlHost.cs`, `StoryEditor/StoryEditor.csproj` (the
  `Microsoft.Web.WebView2` PackageReference), `StoryEditor.Tests/FakeHtmlHost.cs` (`Engine`, `EngineChanged`),
  `StoryEditor.Tests/BrowserTestHelper.cs`
- Test: `StoryEditor.Tests/HtmlMessageChannelTests.cs`, `StoryEditor.Tests/WebView2HtmlHostTests.cs`

**Interfaces:**
- Consumes: B's `HtmlMessage`, `IHtmlHost`, `IeHtmlHost.CstrDocIdToken`; Task 1's Q1, Q2 and Q4 findings.
- Produces:
  - `public enum HtmlEngine { Ie, WebView2 }`
  - `IHtmlHost.Engine` (`HtmlEngine`)
  - `IHtmlHost.EngineChanged` (`event EventHandler`; only `FallbackHtmlHost` raises it)
  - `HtmlHostOptions.UserDataFolder` (`string`; null means the default)
  - `internal sealed class HtmlMessageChannel` (members below)
  - `public sealed class WebView2HtmlHost : IHtmlHost, IFailingHtmlHost`
  - `public interface IFailingHtmlHost { event EventHandler<Exception> Failed; }`
  - `WebView2HtmlHost.RuntimeVersion` (`static string`; null if no runtime)
  - `WebView2HtmlHost.DefaultUserDataFolder` (`static string`)
  - `WebView2HtmlHost.StartTimeout` (20 s)

- [ ] **Step 1: Add the package and the interface members.**

  In `StoryEditor/StoryEditor.csproj`, next to the other PackageReferences, add:

  ```xml
  <PackageReference Include="Microsoft.Web.WebView2" Version="<latest stable 1.0.x on nuget.org; must be >= 1.0.1518.46>" />
  ```

  Write the exact version you used in the task report.

  After building, check that `$S/WebView2Loader.dll` (or `$S/runtimes/win-x86/native/WebView2Loader.dll`) and
  `$S/Microsoft.Web.WebView2.Core.dll` / `$S/Microsoft.Web.WebView2.WinForms.dll` are in the output. If the loader
  lands only under `runtimes\win-x86\native`, add
  `<WebView2LoaderPreference>Static</WebView2LoaderPreference>` to the csproj PropertyGroup and record that.

  In `IHtmlHost.cs`:

  ```csharp
      public enum HtmlEngine
      {
          Ie,
          WebView2
      }
  ```

  Add to `IHtmlHost`:

  ```csharp
          HtmlEngine Engine { get; }                       // which script set its pages need (see PageScripts.For)
          event EventHandler EngineChanged;                // the host switched engines (FallbackHtmlHost): rebuild the page
  ```

  Add to `HtmlHostOptions`:

  ```csharp
          public string UserDataFolder { get; set; }          // WebView2 only; null = WebView2HtmlHost.DefaultUserDataFolder
  ```

  Add to `IHtmlHost.cs`:

  ```csharp
      // a host that can fail after it's been created (WebView2: start-up or browser-process failure)
      public interface IFailingHtmlHost
      {
          event EventHandler<Exception> Failed;
      }
  ```

  In `FakeHtmlHost.cs`, add `public HtmlEngine Engine { get; set; } = HtmlEngine.Ie;`,
  `public event EventHandler EngineChanged;` and `public void RaiseEngineChanged() => EngineChanged?.Invoke(this, EventArgs.Empty);`.

- [ ] **Step 2: Write the failing channel tests.**

  `StoryEditor.Tests/HtmlMessageChannelTests.cs`. They pin the bookkeeping `IeHtmlHost` does today, so moving it can't
  change behaviour:

  ```csharp
  using System;
  using System.Collections.Generic;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture]
      public class HtmlMessageChannelTests
      {
          private List<string> _sent;
          private HtmlMessageChannel _channel;
          private List<HtmlMessage> _raised;
          private int _nReady;

          [SetUp]
          public void SetUp()
          {
              _sent = new List<string>();
              _raised = new List<HtmlMessage>();
              _nReady = 0;
              _channel = new HtmlMessageChannel(json => _sent.Add(json), () => true, "test");
              _channel.MessageReceived += (s, m) => _raised.Add(m);
              _channel.DocumentReady += (s, e) => _nReady++;
          }

          private static HtmlMessage Msg(string json) => HtmlMessage.TryParse(json);

          [Test]
          public void Post_BeforeReady_IsQueued_ThenSentOnReady()
          {
              var nDoc = _channel.BeginDocument();
              _channel.Post(HtmlMessage.Create("x"));
              Assert.That(_sent, Is.Empty);
              _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
              Assert.That(_channel.IsReady, Is.True);
              Assert.That(_sent.Count, Is.EqualTo(1));
              Assert.That(_nReady, Is.EqualTo(1));
          }

          [Test]
          public void ReadyFromOlderDocument_IsIgnored()
          {
              var nOld = _channel.BeginDocument();
              _channel.BeginDocument();
              _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nOld + "\"}"));
              Assert.That(_channel.IsReady, Is.False);
              Assert.That(_nReady, Is.EqualTo(0));
          }

          [Test]
          public void BeginDocument_DropsQueuedPosts()
          {
              _channel.BeginDocument();
              _channel.Post(HtmlMessage.Create("x"));
              var nDoc = _channel.BeginDocument();
              _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
              Assert.That(_sent, Is.Empty);
          }

          [Test]
          public void Request_NotReady_ReturnsNull()
          {
              Assert.That(_channel.Request(HtmlMessage.Create("q"), TimeSpan.FromMilliseconds(50), () => { }), Is.Null);
          }

          [Test]
          public void Request_ReturnsItsOwnReply_AndDropsLateOnes()
          {
              var nDoc = _channel.BeginDocument();
              _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
              var reply = _channel.Request(HtmlMessage.Create("q"), TimeSpan.FromSeconds(1), () =>
              {
                  // the "pump": the page answers the request that was just sent
                  var sent = HtmlMessage.TryParse(_sent[_sent.Count - 1]);
                  _channel.Deliver(Msg("{\"type\":\"reply\",\"re\":" + sent.Rid + ",\"v\":7}"));
              });
              Assert.That(reply.TryGetInt("v", out var n) && n == 7, Is.True);
              _channel.Deliver(Msg("{\"type\":\"reply\",\"re\":999}"));   // nobody waits for it
              Assert.That(_raised.Exists(m => m.Type == "reply"), Is.False, "replies are never raised");
          }

          [Test]
          public void Request_TimesOut_ReturnsNull()
          {
              var nDoc = _channel.BeginDocument();
              _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
              Assert.That(_channel.Request(HtmlMessage.Create("q"), TimeSpan.FromMilliseconds(30), () => { }), Is.Null);
          }

          [Test]
          public void OtherMessages_AreRaised_IncludingJsError()
          {
              _channel.Deliver(Msg("{\"type\":\"textChanged\",\"id\":\"a\"}"));
              _channel.Deliver(Msg("{\"type\":\"jsError\",\"message\":\"m\"}"));
              Assert.That(_raised.ConvertAll(m => m.Type), Is.EqualTo(new[] { "textChanged", "jsError" }));
          }
      }
  }
  ```

- [ ] **Step 3: Run them.** Expected: compile failure, because `HtmlMessageChannel` doesn't exist yet.

- [ ] **Step 4: Write `HtmlMessageChannel` and move `IeHtmlHost` onto it.**

  `StoryEditor/HtmlMessageChannel.cs`. This is `IeHtmlHost`'s bookkeeping, moved verbatim in behaviour:

  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Globalization;
  using System.Threading;

  namespace OneStoryProjectEditor
  {
      /// <summary>
      /// what both browser hosts share: document ids, posts queued until the page is 'ready', matching replies to
      /// requests, and the bounded wait for a reply. The host supplies how to send JSON to the page, whether it's still
      /// alive, and (per request) how to pump the message loop while waiting
      /// </summary>
      internal sealed class HtmlMessageChannel
      {
          private readonly Action<string> _sendJson;
          private readonly Func<bool> _isAlive;
          private readonly string _strHostName;
          private readonly List<HtmlMessage> _pendingPosts = new List<HtmlMessage>();
          private readonly HashSet<int> _awaitedRids = new HashSet<int>();
          private readonly Dictionary<int, HtmlMessage> _replies = new Dictionary<int, HtmlMessage>();
          private int _nLastRid;

          public HtmlMessageChannel(Action<string> sendJson, Func<bool> isAlive, string strHostName)
          {
              _sendJson = sendJson;
              _isAlive = isAlive;
              _strHostName = strHostName;
          }

          public int DocId { get; private set; }
          public string DocIdText => DocId.ToString(CultureInfo.InvariantCulture);
          public bool IsReady { get; private set; }
          public event EventHandler<HtmlMessage> MessageReceived;
          public event EventHandler DocumentReady;

          // a new document is about to load: returns its id (what the page's bridge.js will put in every message)
          public int BeginDocument()
          {
              DocId++;
              IsReady = false;
              _pendingPosts.Clear();
              return DocId;
          }

          public void Post(HtmlMessage msg)
          {
              if (!IsReady)
                  _pendingPosts.Add(msg);
              else
                  Send(msg);
          }

          // pumpOnce: let the host's message loop run once (replies arrive through it)
          public HtmlMessage Request(HtmlMessage msg, TimeSpan timeout, Action pumpOnce)
          {
              if (!IsReady || !_isAlive())
                  return null;

              var nRid = ++_nLastRid;
              _awaitedRids.Add(nRid);
              try
              {
                  Send(msg.WithRid(nRid));
                  var sw = Stopwatch.StartNew();
                  while (true)
                  {
                      if (_replies.TryGetValue(nRid, out var reply))
                      {
                          _replies.Remove(nRid);
                          return reply;
                      }
                      if (sw.Elapsed >= timeout)
                      {
                          Debug.WriteLine($"{_strHostName}: no reply to '{msg.Type}' within {timeout.TotalMilliseconds} ms");
                          return null;
                      }
                      pumpOnce();
                      if (!_isAlive())
                      {
                          Debug.WriteLine($"{_strHostName}: disposed while waiting for a reply to '{msg.Type}'");
                          return null;
                      }
                      if (!_replies.ContainsKey(nRid))
                          Thread.Sleep(1);
                  }
              }
              finally
              {
                  _awaitedRids.Remove(nRid);
              }
          }

          // returns true when msg was the current document's 'ready' (so the host can end its load)
          public bool Deliver(HtmlMessage msg)
          {
              switch (msg.Type)
              {
                  case HtmlMessage.CstrTypeReply:
                      var nRe = msg.ReplyTo;
                      if (nRe.HasValue && _awaitedRids.Contains(nRe.Value))
                          _replies[nRe.Value] = msg;
                      return false;   // late replies (after a timeout) are dropped

                  case HtmlMessage.CstrTypeReady:
                      if (msg.DocId != DocIdText)
                          return false; // an older document finished loading after another load began
                      IsReady = true;
                      var aPending = _pendingPosts.ToArray();
                      _pendingPosts.Clear();
                      foreach (var pending in aPending)
                          Send(pending);
                      DocumentReady?.Invoke(this, EventArgs.Empty);
                      return true;

                  case HtmlMessage.CstrTypeJsError:
                      Debug.WriteLine($"{_strHostName}: script error: {msg.GetString("message")} ({msg.GetString("source")}:{msg.Body["line"]})");
                      break;  // also passed on, so tests (and panes) can see it
              }
              MessageReceived?.Invoke(this, msg);
              return false;
          }

          private void Send(HtmlMessage msg)
          {
              if (_isAlive())
                  _sendJson(msg.ToJson());
          }
      }
  }
  ```

  Then refactor `IeHtmlHost` to use it:
  - The constructor creates `_channel = new HtmlMessageChannel(SendJson, () => !_bDisposed && !_browser.IsDisposed, "IeHtmlHost")`
    and forwards `_channel.MessageReceived` / `_channel.DocumentReady` to its own events.
  - `IsReady => _channel.IsReady`.
  - `LoadHtml` calls `_channel.BeginDocument()` in place of `_nDocId++; IsReady = false; _pendingPosts.Clear();`, and
    uses `_channel.DocIdText` where it formatted `_nDocId`.
  - `Post(type, payload)` → `_channel.Post(HtmlMessage.Create(type, payload))`.
  - `Request(type, payload, timeout)` → `_channel.Request(HtmlMessage.Create(type, payload), timeout, Application.DoEvents)`.
  - `SendJson(json)` holds the old `Send` body: the `Document` null check, then `InvokeScript("oseReceive", …)`.
  - `Deliver(msg)` becomes:

    ```csharp
            private void Deliver(HtmlMessage msg)
            {
                // a 'ready' for the load in flight ends that load, even if it's no longer the current document
                if ((msg.Type == HtmlMessage.CstrTypeReady) && _bLoading &&
                    (msg.DocId == _nLoadingDocId.ToString(CultureInfo.InvariantCulture)))
                    EndLoadWithoutCompletion();
                _channel.Deliver(msg);
            }
    ```

  - `_nLoadingDocId` is set from `_channel.DocId` in `StartLoad`.
  - Add `public HtmlEngine Engine => HtmlEngine.Ie;` and `public event EventHandler EngineChanged { add { } remove { } }`
    (never raised).
  - Remove the now-unused fields (`_pendingPosts`, `_awaitedRids`, `_replies`, `_nLastRid`, `_nDocId`).

- [ ] **Step 5: Run the channel tests and the IE host tests.** Run `/Tests:HtmlMessageChannelTests,IeHtmlHostTests`,
  then the full suite. Expected: everything that passed before still passes, plus the new channel tests.

- [ ] **Step 6: Write the failing WebView2 host tests.**

  In `BrowserTestHelper.cs`, add:

  ```csharp
          public static readonly string WebView2TestDataFolder =
              System.IO.Path.Combine(System.IO.Path.GetTempPath(), "OseWebView2Tests");

          public static WebView2HtmlHost CreateWebView2HostInOffscreenForm(out Form form, string strUserDataFolder = null)
          {
              if (WebView2HtmlHost.RuntimeVersion == null)
                  Assert.Inconclusive("the WebView2 runtime isn't installed on this machine");
              form = new Form
              {
                  StartPosition = FormStartPosition.Manual,
                  Location = new Point(-32000, -32000),
                  Size = new Size(800, 600),
                  ShowInTaskbar = false
              };
              var host = new WebView2HtmlHost(new HtmlHostOptions { UserDataFolder = strUserDataFolder ?? WebView2TestDataFolder });
              form.Controls.Add(host.Control);
              form.Show();
              return host;
          }

          public static string ModernPage(string strBodyHtml, params string[] astrScripts)
          {
              return "<!DOCTYPE html><html><head><meta charset=\"utf-8\">" + PageScripts.ScriptBlock(astrScripts) +
                     "</head><body>" + strBodyHtml + "</body></html>";
          }
  ```

  If the spike found that WebView2 doesn't start or render in a window at (-32000, -32000), use `Location = (0, 0)`
  with `Opacity = 0` here, and say so in the report.

  `StoryEditor.Tests/WebView2HtmlHostTests.cs`. The page script is a minimal bridge stand-in, so this task doesn't
  depend on Task 4's `modern/bridge.js`:

  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
      public class WebView2HtmlHostTests
      {
          // just enough of a bridge: ready on load; echo / sendBack / boom / navigate / text handlers
          private const string CstrMiniBridge =
              "var docId='" + IeHtmlHost.CstrDocIdToken + "';" +
              "function send(t,p){p=p||{};p.type=t;p.doc=docId;chrome.webview.postMessage(JSON.stringify(p));}" +
              "chrome.webview.addEventListener('message',function(e){var m=JSON.parse(e.data),r={};" +
              " if(m.type==='echo')r={text:m.text};" +
              " else if(m.type==='sendBack')send('pong',{n:m.n});" +
              " else if(m.type==='bodyText')r={text:document.body.innerText};" +
              " else if(m.type==='go')location.href=m.url;" +
              " else if(m.type==='back')history.back();" +
              " if(m.rid!==undefined){r.re=m.rid;send('reply',r);}});" +
              "window.addEventListener('load',function(){send('ready',{});});";

          private Form _form;
          private WebView2HtmlHost _host;
          private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

          [SetUp]
          public void SetUp()
          {
              _received.Clear();
              _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
              _host.MessageReceived += (s, m) => _received.Add(m);
          }

          [TearDown]
          public void TearDown()
          {
              _form?.Close();
              _form?.Dispose();
          }

          private void Load(string strBody)
          {
              _host.LoadHtml(BrowserTestHelper.ModernPage(strBody, CstrMiniBridge));
              Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True, "page never sent 'ready'");
          }

          [Test]
          public void Engine_IsWebView2()
          {
              Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.WebView2));
          }

          [Test]
          public void LoadHtml_BeforeStarted_StillLoads_AndRaisesDocumentReady()
          {
              var nReady = 0;
              _host.DocumentReady += (s, e) => nReady++;
              Load("hi");     // LoadHtml runs before the asynchronous start-up has finished
              Assert.That(nReady, Is.EqualTo(1));
          }

          [Test]
          public void Request_RoundTripsAwkwardText()
          {
              Load("hi");
              var strText = "a \"quote\" <b>&amp;</b>\r\nline2 \\ " + HtmlMessageTests.AwkwardNonAscii;
              Assert.That(_host.Request("echo", new { text = strText }, HtmlHostDefaults.RequestTimeout)?.GetString("text"),
                          Is.EqualTo(strText));
          }

          [Test]
          public void Post_DeliversPageMessageAsynchronously()
          {
              Load("hi");
              _host.Post("sendBack", new { n = 5 });
              Assert.That(_received.Exists(m => m.Type == "pong"), Is.False);
              Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "pong")), Is.True);
          }

          [Test]
          public void PageText_RoundTripsNonAscii()
          {
              // served through WebResourceRequested: must be UTF-8 end to end
              var strText = HtmlMessageTests.AwkwardNonAscii;
              Load("<p>" + strText + "</p>");
              Assert.That(_host.Request("bodyText", null, HtmlHostDefaults.RequestTimeout)?.GetString("text")?.Trim(),
                          Is.EqualTo(strText));
          }

          [Test]
          public void LargePage_Loads()
          {
              var sb = new StringBuilder();
              while (sb.Length < 3 * 1024 * 1024)
                  sb.Append("<p>line of story text ").Append(sb.Length).Append("</p>");
              Load(sb.ToString());
              Assert.That(_host.Request("echo", new { text = "ok" }, HtmlHostDefaults.RequestTimeout)?.GetString("text"), Is.EqualTo("ok"));
          }

          [Test]
          public void NavigationNotStartedByHost_IsCancelled()
          {
              Load("first");
              Load("second");
              _host.Post("back");
              _host.Post("go", new { url = "https://example.com/" });
              BrowserTestHelper.Pump(1500);
              Assert.That(_host.Request("bodyText", null, HtmlHostDefaults.RequestTimeout)?.GetString("text")?.Trim(),
                          Is.EqualTo("second"), "the page must still be the one the host loaded last");
          }

          [Test]
          public void LoadedHtml_IsWhatTheCallerPassed()
          {
              var strHtml = BrowserTestHelper.ModernPage("x", CstrMiniBridge);
              _host.LoadHtml(strHtml);
              Assert.That(_host.LoadedHtml, Is.EqualTo(strHtml));
          }

          [Test]
          public void UnusableUserDataFolder_RaisesFailed()
          {
              _form.Close();
              var strFile = Path.Combine(Path.GetTempPath(), "OseWv2NotAFolder.txt");
              File.WriteAllText(strFile, "x");
              Exception failure = null;
              var host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form, strFile);
              host.Failed += (s, ex) => failure = ex;
              host.LoadHtml(BrowserTestHelper.ModernPage("x", CstrMiniBridge));
              Assert.That(BrowserTestHelper.PumpUntil(() => failure != null, (int)WebView2HtmlHost.StartTimeout.TotalMilliseconds + 5000), Is.True,
                          "a host that can't start must say so (exception or watchdog)");
          }

          [Test]
          public void Dispose_Twice_DoesNotThrow_AndRequestsReturnNull()
          {
              Load("hi");
              _host.Dispose();
              Assert.DoesNotThrow(() => _host.Dispose());
              Assert.That(_host.Request("echo", new { text = "x" }, HtmlHostDefaults.RequestTimeout), Is.Null);
          }
      }
  }
  ```

  `HtmlMessageTests.AwkwardNonAscii` exists from B. If it's `private`, make it `internal static`.

- [ ] **Step 7: Write `WebView2HtmlHost`.**

  `StoryEditor/WebView2HtmlHost.cs`. Adapt the failure handling to Task 1's Q4 findings, keeping the watchdog either way:

  ```csharp
  using System;
  using System.Diagnostics;
  using System.Drawing;
  using System.IO;
  using System.Text;
  using System.Windows.Forms;
  using Microsoft.Web.WebView2.Core;
  using Microsoft.Web.WebView2.WinForms;

  namespace OneStoryProjectEditor
  {
      /// <summary>
      /// IHtmlHost over WebView2 (Chromium; the Evergreen runtime). The only class allowed to touch WebView2 types.
      /// Pages are served from memory at https://ose.pane/page.html?d=docId (no NavigateToString 2 MB limit) and every
      /// navigation the host didn't start is cancelled. Message bookkeeping is HtmlMessageChannel, shared with IeHtmlHost
      /// </summary>
      public sealed class WebView2HtmlHost : IHtmlHost, IFailingHtmlHost
      {
          private const string CstrPageOrigin = "https://ose.pane/";
          public static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(20);

          public static string DefaultUserDataFolder =>
              Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneStory Editor", "WebView2");

          // null when no WebView2 runtime is installed (or it can't be queried)
          public static string RuntimeVersion
          {
              get
              {
                  try
                  {
                      return CoreWebView2Environment.GetAvailableBrowserVersionString();
                  }
                  catch (Exception)
                  {
                      return null;
                  }
              }
          }

          private readonly WebView2 _view;
          private readonly HtmlMessageChannel _channel;
          private readonly string _strUserDataFolder;
          private readonly Timer _startWatchdog;
          private CoreWebView2Environment _env;
          private bool _bStarted;
          private bool _bStarting;
          private bool _bFailed;
          private bool _bDisposed;
          private string _strExpectedUrl;     // the one navigation we allow next
          private string _strServedHtml;      // LoadedHtml with the doc id filled in

          public WebView2HtmlHost(HtmlHostOptions options)
          {
              _strUserDataFolder = options?.UserDataFolder ?? DefaultUserDataFolder;
              _view = new WebView2 { Dock = DockStyle.Fill, DefaultBackgroundColor = Color.White };
              _channel = new HtmlMessageChannel(SendJson, () => !_bDisposed && !_bFailed && _bStarted, "WebView2HtmlHost");
              _channel.MessageReceived += (s, m) => MessageReceived?.Invoke(this, m);
              _channel.DocumentReady += (s, e) => DocumentReady?.Invoke(this, EventArgs.Empty);
              _startWatchdog = new Timer { Interval = (int)StartTimeout.TotalMilliseconds };
              _startWatchdog.Tick += (s, e) => Fail(new TimeoutException("WebView2 did not start within " + StartTimeout.TotalSeconds + " s"));
              _view.HandleCreated += (s, e) => Start();
          }

          public Control Control => _view;
          public HtmlEngine Engine => HtmlEngine.WebView2;
          public bool IsReady => _channel.IsReady;
          public string LoadedHtml { get; private set; }
          public event EventHandler<HtmlMessage> MessageReceived;
          public event EventHandler DocumentReady;
          public event EventHandler EngineChanged { add { } remove { } }
          public event EventHandler<Exception> Failed;

          private async void Start()
          {
              if (_bStarting || _bStarted || _bDisposed)
                  return;
              _bStarting = true;
              _startWatchdog.Start();
              try
              {
                  Directory.CreateDirectory(_strUserDataFolder);
                  _env = await CoreWebView2Environment.CreateAsync(null, _strUserDataFolder);
                  await _view.EnsureCoreWebView2Async(_env);
                  if (_bDisposed || _bFailed)
                      return;
                  Configure(_view.CoreWebView2);
                  _startWatchdog.Stop();
                  _bStarted = true;
                  if (LoadedHtml != null)
                      Navigate();
              }
              catch (Exception ex)
              {
                  Fail(ex);
              }
          }

          private void Configure(CoreWebView2 core)
          {
              var settings = core.Settings;
              settings.AreDefaultContextMenusEnabled = false;
  #if DEBUG
              settings.AreDevToolsEnabled = true;
  #else
              settings.AreDevToolsEnabled = false;
  #endif
              settings.IsStatusBarEnabled = false;
              settings.IsZoomControlEnabled = true;
              core.AddWebResourceRequestedFilter(CstrPageOrigin + "*", CoreWebView2WebResourceContext.All);
              core.WebResourceRequested += OnWebResourceRequested;
              core.NavigationStarting += OnNavigationStarting;
              core.NewWindowRequested += (s, e) => e.Handled = true;
              core.WebMessageReceived += OnWebMessageReceived;
              core.ProcessFailed += (s, e) => Fail(new InvalidOperationException("WebView2 browser process failed: " + e.ProcessFailedKind));
          }

          public void LoadHtml(string strHtml)
          {
              _channel.BeginDocument();
              LoadedHtml = strHtml ?? String.Empty;
              if (_bStarted)
                  Navigate();
              // (otherwise Start() navigates once it has finished)
          }

          private void Navigate()
          {
              if (_bDisposed || _bFailed)
                  return;
              _strServedHtml = LoadedHtml.Replace(IeHtmlHost.CstrDocIdToken, _channel.DocIdText);
              _strExpectedUrl = CstrPageOrigin + "page.html?d=" + _channel.DocIdText;
              _view.CoreWebView2.Navigate(_strExpectedUrl);
          }

          private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
          {
              // only the navigation we started: no back/forward to an older page, no links or script navigation away
              if (e.Uri != _strExpectedUrl)
              {
                  Debug.WriteLine("WebView2HtmlHost: cancelled navigation to " + e.Uri);
                  e.Cancel = true;
              }
          }

          private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
          {
              if (!e.Request.Uri.StartsWith(CstrPageOrigin + "page.html", StringComparison.OrdinalIgnoreCase))
              {
                  e.Response = _env.CreateWebResourceResponse(null, 404, "Not Found", "");
                  return;
              }
              var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(_strServedHtml ?? String.Empty));
              e.Response = _env.CreateWebResourceResponse(stream, 200, "OK", "Content-Type: text/html; charset=utf-8");
          }

          private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
          {
              string strJson;
              try
              {
                  strJson = e.TryGetWebMessageAsString();
              }
              catch (ArgumentException)
              {
                  Debug.WriteLine("WebView2HtmlHost: non-string message from page");
                  return;
              }
              var msg = HtmlMessage.TryParse(strJson);
              if (msg == null)
              {
                  Debug.WriteLine("WebView2HtmlHost: unparseable message from page: " + strJson);
                  return;
              }
              _channel.Deliver(msg);   // (already on the UI thread, already after the page's call returned)
          }

          public void Post(string strType, object payload = null)
          {
              _channel.Post(HtmlMessage.Create(strType, payload));
          }

          public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
          {
              return _channel.Request(HtmlMessage.Create(strType, payload), timeout, Application.DoEvents);
          }

          private void SendJson(string strJson)
          {
              _view.CoreWebView2?.PostWebMessageAsString(strJson);
          }

          public void ShowPrintPreview()
          {
              if (_bStarted && !_bDisposed)
                  _view.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
          }

          private void Fail(Exception ex)
          {
              if (_bFailed || _bDisposed)
                  return;
              _bFailed = true;
              _startWatchdog.Stop();
              Debug.WriteLine("WebView2HtmlHost: failed: " + ex);
              Failed?.Invoke(this, ex);
          }

          public void Dispose()
          {
              if (_bDisposed)
                  return;
              _bDisposed = true;
              _startWatchdog.Stop();
              _startWatchdog.Dispose();
              _view.Dispose();
          }
      }
  }
  ```

  Notes:
  - `Timer` here is `System.Windows.Forms.Timer`; qualify it if `System.Threading` is imported.
  - Navigating to `_strExpectedUrl` raises `NavigationStarting` with exactly that URI, so it is allowed through.
  - If Task 1 found that `Uri` comes back normalised (for example with a trailing slash or different casing), compare
    with `Uri.Compare` / `OrdinalIgnoreCase` and record it.

- [ ] **Step 8: Run the WebView2 tests and the full suite.**

  Run `/Tests:WebView2HtmlHostTests`, then everything. Expected: all pass on this machine (Windows 11 has the runtime).

  If `UnusableUserDataFolder_RaisesFailed` can't make WebView2 fail that way, use the failure Task 1 found would work
  (for example the `WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` trick, set and restored inside the test). Never weaken the
  assertion.

- [ ] **Step 9: Commit.**

  ```bash
  git add StoryEditor/HtmlMessageChannel.cs StoryEditor/WebView2HtmlHost.cs StoryEditor/IHtmlHost.cs StoryEditor/IeHtmlHost.cs StoryEditor/StoryEditor.csproj StoryEditor.Tests/FakeHtmlHost.cs StoryEditor.Tests/BrowserTestHelper.cs StoryEditor.Tests/HtmlMessageChannelTests.cs StoryEditor.Tests/WebView2HtmlHostTests.cs StoryEditor.Tests/HtmlMessageTests.cs
  git commit -m "C2: shared HtmlMessageChannel; WebView2HtmlHost (pages from memory, navigation guard, start watchdog)"
  ```

---
### Task 3: `FallbackHtmlHost`, engine choice, setting, notice, rebuild on engine change

**Files:**
- Create: `StoryEditor/FallbackHtmlHost.cs`
- Modify:
  - `StoryEditor/HtmlHostFactory.cs`
  - `StoryEditor/Properties/Settings.settings` and `Settings.Designer.cs` (2 settings)
  - `StoryEditor/StoryEditor.cs` (constructor at `:256`: menu item and notice)
  - `StoryEditor/HtmlVerseControl.cs` (the `EngineChanged` rebuild; `public HtmlEngine Engine`)
  - `StoryEditor/NetBibleViewer.cs` and `StoryEditor/HtmlForm.cs` (rebuild their pages on `EngineChanged`)
- Test: `StoryEditor.Tests/FallbackHtmlHostTests.cs`, `StoryEditor.Tests/HtmlHostFactoryTests.cs`;
  `StoryEditor.Tests/FakeHtmlHost.cs` (add `FakeFailingHtmlHost`)

**Interfaces:**
- Consumes (Task 2): `IHtmlHost.Engine` / `EngineChanged`, `IFailingHtmlHost.Failed`, `WebView2HtmlHost`, `IeHtmlHost`,
  `WebView2HtmlHost.RuntimeVersion`.
- Produces:
  - `public sealed class FallbackHtmlHost : IHtmlHost` with constructor
    `(IHtmlHost primary, Func<IHtmlHost> createFallback, Action<Exception> onFallback)`.
  - `HtmlHostFactory`:
    - `internal static bool UseWebView2WhenAvailable` (false until Task 7)
    - `internal static string WhyNotWebView2(bool bUseIeSetting, string strRuntimeVersion, string strFailedRuntimeVersion)`
    - `public static event EventHandler<string> Notice`
    - `public static string LastNotice`
  - Settings `UseIeBrowserPanes` (bool, default False) and `WebView2FailedRuntimeVersion` (string, default empty).
  - `HtmlVerseControl.Engine` (`public HtmlEngine`).

- [ ] **Step 1: Write the failing tests.**

  Add to `StoryEditor.Tests/FakeHtmlHost.cs`:

  ```csharp
      public class FakeFailingHtmlHost : FakeHtmlHost, IFailingHtmlHost
      {
          public event EventHandler<Exception> Failed;

          public void Fail(string strWhy = "simulated")
          {
              Failed?.Invoke(this, new InvalidOperationException(strWhy));
          }
      }
  ```

  `StoryEditor.Tests/FallbackHtmlHostTests.cs`:

  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA)]
      public class FallbackHtmlHostTests
      {
          private FakeFailingHtmlHost _primary;
          private FakeHtmlHost _fallback;
          private FallbackHtmlHost _host;
          private Exception _reported;

          [SetUp]
          public void SetUp()
          {
              _primary = new FakeFailingHtmlHost { Engine = HtmlEngine.WebView2 };
              _fallback = new FakeHtmlHost { Engine = HtmlEngine.Ie };
              _reported = null;
              _host = new FallbackHtmlHost(_primary, () => _fallback, ex => _reported = ex);
          }

          [TearDown]
          public void TearDown()
          {
              _host.Dispose();
          }

          [Test]
          public void ForwardsToPrimary_UntilItFails()
          {
              Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.WebView2));
              _host.LoadHtml("<p>a</p>");
              _host.Post("x");
              Assert.That(_primary.LoadedHtml, Is.EqualTo("<p>a</p>"));
              Assert.That(_primary.Posts.Count, Is.EqualTo(1));
              Assert.That(_host.Control.Controls.Contains(_primary.Control), Is.True);
          }

          [Test]
          public void PrimaryFails_SwapsToFallback_ReplaysPage_RaisesEngineChanged()
          {
              var nChanged = 0;
              _host.EngineChanged += (s, e) => nChanged++;
              _host.LoadHtml("<p>a</p>");
              _primary.Fail();
              Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
              Assert.That(_fallback.LoadedHtml, Is.EqualTo("<p>a</p>"), "the last page is replayed in the fallback");
              Assert.That(nChanged, Is.EqualTo(1));
              Assert.That(_reported, Is.Not.Null);
              Assert.That(_host.Control.Controls.Contains(_fallback.Control), Is.True);
              Assert.That(_host.Control.Controls.Contains(_primary.Control), Is.False);
              Assert.That(_primary.DisposeCount, Is.EqualTo(1));
          }

          [Test]
          public void MessagesAndReady_AreForwarded_FromWhicheverHostIsCurrent()
          {
              var types = new List<string>();
              var nReady = 0;
              _host.MessageReceived += (s, m) => types.Add(m.Type);
              _host.DocumentReady += (s, e) => nReady++;
              _primary.Raise("a");
              _primary.Fail();
              _fallback.Raise("b");
              _fallback.RaiseReady();
              _primary.Raise("c");    // the old host is detached
              Assert.That(types, Is.EqualTo(new[] { "a", "b" }));
              Assert.That(nReady, Is.EqualTo(1));
          }

          [Test]
          public void SecondFailure_IsIgnored()
          {
              _primary.Fail();
              Assert.DoesNotThrow(() => _primary.Fail());
              Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
          }

          [Test]
          public void RequestDuringSwap_ReturnsNull()
          {
              // the primary fails while a Request is waiting on it (e.g. a flush): that Request gets no answer
              _primary.OnRequest = m => { _primary.Fail(); return null; };
              Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Null);
              Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
              _fallback.OnRequest = m => FakeHtmlHost.Reply();
              Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null, "later requests go to the fallback");
          }
      }
  }
  ```

  `StoryEditor.Tests/HtmlHostFactoryTests.cs`:

  ```csharp
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture]
      public class HtmlHostFactoryTests
      {
          [Test]
          public void UsesWebView2_WhenRuntimePresent_SettingOff_NoFailure()
          {
              Assert.That(HtmlHostFactory.WhyNotWebView2(false, "130.0.2849.80", ""), Is.Null);
          }

          [Test]
          public void Setting_ForcesIe()
          {
              Assert.That(HtmlHostFactory.WhyNotWebView2(true, "130.0.2849.80", ""), Does.Contain("setting"));
          }

          [TestCase(null)]
          [TestCase("")]
          public void NoRuntime_UsesIe(string strRuntime)
          {
              Assert.That(HtmlHostFactory.WhyNotWebView2(false, strRuntime, ""), Does.Contain("isn't installed"));
          }

          [Test]
          public void FailedLastTime_OnThisRuntime_UsesIe()
          {
              Assert.That(HtmlHostFactory.WhyNotWebView2(false, "130.0.2849.80", "130.0.2849.80"), Does.Contain("failed"));
          }

          [Test]
          public void FailedOnAnOlderRuntime_TriesWebView2Again()
          {
              Assert.That(HtmlHostFactory.WhyNotWebView2(false, "131.0.1.1", "130.0.2849.80"), Is.Null);
          }
      }
  }
  ```

- [ ] **Step 2: Run them.** Expected: compile failure; the types and members don't exist yet.

- [ ] **Step 3: Write `FallbackHtmlHost`.**

  `StoryEditor/FallbackHtmlHost.cs`:

  ```csharp
  using System;
  using System.Windows.Forms;

  namespace OneStoryProjectEditor
  {
      /// <summary>
      /// the host HtmlHostFactory hands out when it chooses WebView2: it forwards to the WebView2 host, and if that fails
      /// (start-up or browser process), swaps an IE host into the same container, replays the last page there and raises
      /// EngineChanged so the owner rebuilds the page with the IE scripts. The user never sees a blank pane
      /// </summary>
      public sealed class FallbackHtmlHost : IHtmlHost
      {
          private readonly Panel _container = new Panel { Dock = DockStyle.Fill };
          private readonly Func<IHtmlHost> _createFallback;
          private readonly Action<Exception> _onFallback;
          private IHtmlHost _inner;
          private bool _bSwapped;
          private bool _bDisposed;

          public FallbackHtmlHost(IHtmlHost primary, Func<IHtmlHost> createFallback, Action<Exception> onFallback)
          {
              _createFallback = createFallback;
              _onFallback = onFallback;
              Attach(primary);
              if (primary is IFailingHtmlHost failing)
                  failing.Failed += OnPrimaryFailed;
          }

          public Control Control => _container;
          public HtmlEngine Engine => _inner.Engine;
          public bool IsReady => _inner.IsReady;
          public string LoadedHtml => _inner.LoadedHtml;
          public event EventHandler<HtmlMessage> MessageReceived;
          public event EventHandler DocumentReady;
          public event EventHandler EngineChanged;

          public void LoadHtml(string strHtml) => _inner.LoadHtml(strHtml);
          public void Post(string strType, object payload = null) => _inner.Post(strType, payload);
          public HtmlMessage Request(string strType, object payload, TimeSpan timeout) => _inner.Request(strType, payload, timeout);
          public void ShowPrintPreview() => _inner.ShowPrintPreview();

          private void Attach(IHtmlHost inner)
          {
              _inner = inner;
              inner.Control.Dock = DockStyle.Fill;
              _container.Controls.Add(inner.Control);
              inner.MessageReceived += OnInnerMessage;
              inner.DocumentReady += OnInnerReady;
          }

          private void Detach(IHtmlHost inner)
          {
              inner.MessageReceived -= OnInnerMessage;
              inner.DocumentReady -= OnInnerReady;
              _container.Controls.Remove(inner.Control);
          }

          private void OnInnerMessage(object sender, HtmlMessage msg) => MessageReceived?.Invoke(this, msg);
          private void OnInnerReady(object sender, EventArgs e) => DocumentReady?.Invoke(this, EventArgs.Empty);

          private void OnPrimaryFailed(object sender, Exception ex)
          {
              if (_bSwapped || _bDisposed)
                  return;
              _bSwapped = true;

              var failed = _inner;
              var strLastPage = failed.LoadedHtml;
              if (failed is IFailingHtmlHost failing)
                  failing.Failed -= OnPrimaryFailed;
              Detach(failed);
              failed.Dispose();

              Attach(_createFallback());
              if (strLastPage != null)
                  _inner.LoadHtml(strLastPage);     // (the owner rebuilds it with the IE scripts on EngineChanged)

              _onFallback?.Invoke(ex);
              EngineChanged?.Invoke(this, EventArgs.Empty);
          }

          public void Dispose()
          {
              if (_bDisposed)
                  return;
              _bDisposed = true;
              _inner.Dispose();
              _container.Dispose();
          }
      }
  }
  ```

  > If the WebView2 host raises `Failed` from inside one of its own WebView2 event handlers, disposing it right there
  > could be unsafe. If Task 1 or a test shows that, defer `failed.Dispose()` with `_container.BeginInvoke` when
  > `_container.IsHandleCreated`, and record it. The fake-host tests above call `Fail()` directly.

- [ ] **Step 4: Add the settings.**

  In `Properties/Settings.settings`, next to `AutoSaveTimeoutEnabled`:

  ```xml
      <Setting Name="UseIeBrowserPanes" Type="System.Boolean" Scope="User">
        <Value Profile="(Default)">False</Value>
      </Setting>
      <Setting Name="WebView2FailedRuntimeVersion" Type="System.String" Scope="User">
        <Value Profile="(Default)" />
      </Setting>
  ```

  In `Properties/Settings.Designer.cs`, in the same style as `AutoSaveTimeoutEnabled`:

  ```csharp
          [global::System.Configuration.UserScopedSettingAttribute()]
          [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
          [global::System.Configuration.DefaultSettingValueAttribute("False")]
          public bool UseIeBrowserPanes {
              get {
                  return ((bool)(this["UseIeBrowserPanes"]));
              }
              set {
                  this["UseIeBrowserPanes"] = value;
              }
          }

          [global::System.Configuration.UserScopedSettingAttribute()]
          [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
          [global::System.Configuration.DefaultSettingValueAttribute("")]
          public string WebView2FailedRuntimeVersion {
              get {
                  return ((string)(this["WebView2FailedRuntimeVersion"]));
              }
              set {
                  this["WebView2FailedRuntimeVersion"] = value;
              }
          }
  ```

  If `StoryEditor/app.config` lists user settings under `<userSettings>`, add both there too, with the same defaults.

- [ ] **Step 5: Write the factory.**

  Replace `StoryEditor/HtmlHostFactory.cs`:

  ```csharp
  using System;
  using OneStoryProjectEditor.Properties;

  namespace OneStoryProjectEditor
  {
      /// <summary>
      /// chooses the browser engine for every HTML host: WebView2 where its runtime works (wrapped so it falls back to
      /// IE if it fails), otherwise IE. See docs/superpowers/specs/2026-10-04-webview2-host-design.md
      /// </summary>
      public static class HtmlHostFactory
      {
          // off until every page has its modern (WebView2) scripts; the last step of the plan's Task 7 turns it on
          internal static bool UseWebView2WhenAvailable = false;

          private static bool _bNoticeGiven;

          // once per session: why IE is being used, or that WebView2 failed and IE took over
          public static event EventHandler<string> Notice;
          public static string LastNotice { get; private set; }

          public static IHtmlHost Create(HtmlHostOptions options = null)
          {
              options = options ?? new HtmlHostOptions();
              if (!UseWebView2WhenAvailable)
                  return new IeHtmlHost(options);

              var strWhyNot = WhyNotWebView2(Settings.Default.UseIeBrowserPanes, WebView2HtmlHost.RuntimeVersion,
                                             Settings.Default.WebView2FailedRuntimeVersion);
              if (strWhyNot != null)
              {
                  GiveNotice("Using IE browser panes: " + strWhyNot);
                  return new IeHtmlHost(options);
              }

              return new FallbackHtmlHost(new WebView2HtmlHost(options), () => new IeHtmlHost(options), OnWebView2Failed);
          }

          // null = use WebView2; otherwise the reason not to (for the status bar)
          internal static string WhyNotWebView2(bool bUseIeSetting, string strRuntimeVersion, string strFailedRuntimeVersion)
          {
              if (bUseIeSetting)
                  return "the 'Use the IE browser panes' setting is on";
              if (String.IsNullOrEmpty(strRuntimeVersion))
                  return "the WebView2 runtime isn't installed";
              if (strFailedRuntimeVersion == strRuntimeVersion)
                  return "WebView2 failed to start last time";
              return null;    // (a failure on an older runtime doesn't count: an update may have fixed it)
          }

          private static void OnWebView2Failed(Exception ex)
          {
              Settings.Default.WebView2FailedRuntimeVersion = WebView2HtmlHost.RuntimeVersion ?? String.Empty;
              Settings.Default.Save();
              GiveNotice("WebView2 stopped working (" + ex.Message + "), so IE browser panes are being used");
          }

          private static void GiveNotice(string strNotice)
          {
              if (_bNoticeGiven)
                  return;
              _bNoticeGiven = true;
              LastNotice = strNotice;
              Notice?.Invoke(null, strNotice);
          }
      }
  }
  ```

- [ ] **Step 6: Run the new tests.** Run `/Tests:FallbackHtmlHostTests,HtmlHostFactoryTests`. Expected: all pass.

- [ ] **Step 7: Wire up the owners, the menu and the notice.**

  `HtmlVerseControl.cs`:
  - Add `public HtmlEngine Engine => Host.Engine;`.
  - In the constructor, after the other `Host` subscriptions:

  ```csharp
              // the host fell back to IE: rebuild the page with the IE scripts (a pane with no story yet has nothing to
              //  rebuild; the host has already replayed its last page)
              Host.EngineChanged += (sender, args) =>
              {
                  if ((StoryData != null) && (TheSE != null))
                      LoadDocument();
              };
  ```

  `NetBibleViewer.cs`, in the constructor after `_htmlHost` is created:

  ```csharp
              _htmlHost.EngineChanged += (s, e) =>
              {
                  m_nBook = m_nChapter = 0;    // force DisplayVerses to rebuild the page (now with the IE scripts)
                  DisplayVerses();
              };
  ```

  `HtmlForm.cs`:
  - Keep the caller's text in a field, `private string _strClientText;`, assigned in the `ClientText` setter before
    `LoadHtml`.
  - In the constructor, add `_htmlHost.EngineChanged += (s, e) => { if (_strClientText != null) ClientText = _strClientText; };`.
  - `ClientText`'s getter keeps returning `_htmlHost.LoadedHtml`.

  `MinimalHtmlForm` (`NetBibleFootnoteTooltip.cs`) needs nothing: it has no scripts, and the replayed page is complete.

  `StoryEditor.cs`, in the constructor (`:256`), after `InitializeComponent()` and the other menu set-up:

  ```csharp
              var menuUseIePanes = new ToolStripMenuItem(Localizer.Str("Use the &IE browser panes (takes effect after restart)"))
              {
                  CheckOnClick = true,
                  Checked = Properties.Settings.Default.UseIeBrowserPanes
              };
              menuUseIePanes.CheckedChanged += (s, e) =>
              {
                  Properties.Settings.Default.UseIeBrowserPanes = menuUseIePanes.Checked;
                  if (!menuUseIePanes.Checked)
                      Properties.Settings.Default.WebView2FailedRuntimeVersion = String.Empty;  // try WebView2 again
                  Properties.Settings.Default.Save();
              };
              advancedToolStripMenu.DropDownItems.Add(menuUseIePanes);

              // the panes were created by InitializeComponent, so a notice may already be waiting
              if (HtmlHostFactory.LastNotice != null)
                  SetStatusBar(HtmlHostFactory.LastNotice);
              HtmlHostFactory.Notice += (s, strNotice) => SetStatusBar(strNotice);
  ```

  If something later in the constructor overwrites the status bar (for example the default stage text), move the
  `LastNotice` display to just after that point, and say where in the report.

- [ ] **Step 8: Build and run the full suite.** Expected: all pass. The app still always uses IE, because
  `UseWebView2WhenAvailable` is false.

- [ ] **Step 9: Commit.**

  ```bash
  git add StoryEditor/FallbackHtmlHost.cs StoryEditor/HtmlHostFactory.cs StoryEditor/Properties/Settings.settings StoryEditor/Properties/Settings.Designer.cs StoryEditor/StoryEditor.cs StoryEditor/HtmlVerseControl.cs StoryEditor/NetBibleViewer.cs StoryEditor/HtmlForm.cs StoryEditor.Tests/FakeHtmlHost.cs StoryEditor.Tests/FallbackHtmlHostTests.cs StoryEditor.Tests/HtmlHostFactoryTests.cs
  git commit -m "C3: FallbackHtmlHost, engine choice and IE setting, status notice, rebuild on engine change"
  ```

  (Also add `StoryEditor/app.config` if Step 4 changed it.)

---

### Task 4: Engine-aware pages, `modern/bridge.js`, `modern/PaneCommon.js`

**Files:**
- Modify:
  - `StoryEditor/PageScripts.cs` (`PageScriptSet`, `For`)
  - `StoryEditor/StoryData.cs` (`PresentationHtml`, `AddHtmlHtmlDocOutside`, `ConNotePageScripts`, `ConNoteHtml`,
    `ConsultantNotesHtml`, `CoachNotesHtml`, `GetPresentationHtmlForChorus`)
  - the callers:
    - `HtmlStoryBtControl.cs:96,101`
    - `HtmlConNoteControl.cs:449,740,797`
    - `PrintForm.cs:78`, `LnCNotePrintForm.cs:15`
    - `NetBibleViewer.cs:582`, `HtmlForm.cs:65`
    - any test that calls these builders
  - `StoryEditor/HtmlVerseControl.cs` (`SetSelectedText` accepts `value`)
  - `StoryEditor/StoryEditor.csproj` (embedded resources)
- Create: `StoryEditor/js/modern/bridge.js`, `StoryEditor/js/modern/PaneCommon.js`
- Test: `StoryEditor.Tests/PageScriptsTests.cs`, `StoryEditor.Tests/ModernPaneCommonPageTests.cs`;
  `StoryEditor.Tests/StoryBtPaneTests.cs` (one new test)

**Interfaces:**
- Consumes: `HtmlEngine` and `WebView2HtmlHost` (Task 2); `BrowserTestHelper.CreateWebView2HostInOffscreenForm` and
  `ModernPage` (Task 2).
- Produces:
  - `PageScripts.For(HtmlEngine)` returns `PageScriptSet`, which has `Engine`, lazy string properties `Bridge`,
    `PaneCommon`, `StoryBt`, `StoryBtEnd`, `ConNote`, `NetBible`, `HoverLinks` and `JQuery`, and `ApplyDoctype(string html)`.
  - Builders take an `HtmlEngine engine` parameter (exact signatures below).
  - The modern scripts expose `window.ose = { send, on, closest, cancel, listen, lastTextareaId, replaceSelectionIn }`,
    the same names as the IE scripts.
  - Embedded resource names: `OneStoryProjectEditor.js.modern.<file>`, read with `PageScripts.Get("modern.<file>")`.

- [ ] **Step 1: Write the failing tests.**

  `StoryEditor.Tests/PageScriptsTests.cs`:

  ```csharp
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture]
      public class PageScriptsTests
      {
          [Test]
          public void Ie_IsTodaysScripts()
          {
              var ie = PageScripts.For(HtmlEngine.Ie);
              Assert.That(ie.Bridge, Does.Contain("window.external"));
              Assert.That(ie.PaneCommon, Is.EqualTo(PageScripts.Get("PaneCommon.js")));
              Assert.That(ie.StoryBt, Is.EqualTo(Properties.Resources.StoryBtJs));
              Assert.That(ie.JQuery, Is.EqualTo(Properties.Resources.jquery_min));
              const string strHtml = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n<html></html>";
              Assert.That(ie.ApplyDoctype(strHtml), Is.EqualTo(strHtml), "IE pages are left exactly as they are");
          }

          [Test]
          public void WebView2_IsTheModernScripts_InStandardsMode()
          {
              var modern = PageScripts.For(HtmlEngine.WebView2);
              Assert.That(modern.Bridge, Does.Contain("chrome.webview"));
              Assert.That(modern.Bridge, Does.Not.Contain("window.external"));
              Assert.That(modern.JQuery, Is.Empty);
              Assert.That(modern.StoryBtEnd, Is.Empty);
              Assert.That(modern.ApplyDoctype("<!DOCTYPE HTML PUBLIC \"x\">\r\n<html></html>"), Is.EqualTo("<!DOCTYPE html>\r\n<html></html>"));
              Assert.That(modern.ApplyDoctype("<html></html>"), Is.EqualTo("<!DOCTYPE html>\r\n<html></html>"));
          }

          [Test]
          public void IePresentationPage_IsUnchangedByTheEngineParameter()
          {
              var project = PaneTestData.LoadProject();
              var html = StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.Ie);
              Assert.That(html, Does.StartWith("<!DOCTYPE HTML PUBLIC"));
              Assert.That(html, Does.Contain("window.external"));      // the IE bridge
              Assert.That(html, Does.Not.Contain("chrome.webview"));
          }

          [Test]
          public void WebView2PresentationPage_HasModernScripts_AndNoJQuery()
          {
              var project = PaneTestData.LoadProject();
              var html = StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.WebView2);
              Assert.That(html, Does.StartWith("<!DOCTYPE html>"));
              Assert.That(html, Does.Contain("chrome.webview"));
              Assert.That(html, Does.Not.Contain("window.external"));
              Assert.That(html, Does.Not.Contain("jQuery"));
          }
      }
  }
  ```

  > `WebView2PresentationPage_HasModernScripts_AndNoJQuery` needs `modern.StoryBt.js` to exist. Task 7 creates it, so
  > in this task create `StoryEditor/js/modern/StoryBt.js` and `ConNote.js`, `NetBible.js` and `HoverLinks.js` as
  > one-line placeholder files, each containing only `// replaced in Task 5/6/7 (modern pages)`, and embed them, so the
  > builders and this test work. Tasks 5–7 replace their contents.

  `StoryEditor.Tests/ModernPaneCommonPageTests.cs` is a WebView2 port of `PaneCommonPageTests`. The same assertions,
  with a standard-DOM driver, plus the modern-only commands:

  ```csharp
  using System.Collections.Generic;
  using System.Text;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
      public class ModernPaneCommonPageTests
      {
          private const string CstrDriver =
              "ose.on('click', function (m) { document.getElementById(m.id).click(); });" +
              "ose.on('focusOn', function (m) { document.getElementById(m.id).focus(); });" +
              "ose.on('typeInto', function (m) { var ta = document.getElementById(m.id); ta.value = m.text; });" +
              "ose.on('typeWithInput', function (m) { var ta = document.getElementById(m.id); ta.value = m.text;" +
              "  ta.dispatchEvent(new Event('input', { bubbles: true })); });" +
              "ose.on('selectIn', function (m) { var ta = document.getElementById(m.id); ta.focus(); ta.setSelectionRange(m.start, m.end); });" +
              "ose.on('valueOf', function (m) { return { value: document.getElementById(m.id).value }; });" +
              "ose.on('innerHtml', function (m) { return { html: document.getElementById(m.id).innerHTML }; });" +
              "ose.on('fire', function (m) { document.getElementById(m.id).dispatchEvent(new Event(m.what, { bubbles: true, cancelable: true })); });" +
              "ose.on('echo', function (m) { return { text: m.text }; });";

          private Form _form;
          private WebView2HtmlHost _host;
          private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

          [SetUp]
          public void SetUp()
          {
              _received.Clear();
              _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
              _host.MessageReceived += (s, m) => _received.Add(m);

              var sb = new StringBuilder("<table style=\"border-collapse:collapse\" cellpadding=\"0\">");
              for (var i = 0; i <= 40; i++)
                  sb.AppendFormat("<tr style=\"height:60px\"><td id=\"ln_{0}\">{1}{0}</td><td id=\"anc_{0}\" data-drop=\"scripture\">a</td></tr>", i, VersesData.LinePrefix);
              sb.Append("</table>");
              sb.Append("<a id=\"l1\" href=\"conNote.jumpToLine\" name=\"3\">Ln 3</a>");
              sb.Append("<a id=\"l2\" href=\"bibleViewer.setReference\" name=\"Gen 1:1\">Gen 1:1</a>");
              sb.Append("<a id=\"l3\" href=\"http://example.com/x\">here</a>");
              sb.Append("<button id=\"btn_1_0_1\" data-action=\"convertToMentoree\" data-arg=\"true\">Change</button>");
              sb.Append("<textarea id=\"ta_1_0\">abcdef</textarea><textarea id=\"ta_ro\" readonly>ro</textarea>");
              sb.Append("<p id=\"tp_1_0_0\">para</p>");

              var scripts = PageScripts.For(HtmlEngine.WebView2);
              _host.LoadHtml(BrowserTestHelper.ModernPage(sb.ToString(), scripts.Bridge, scripts.PaneCommon, CstrDriver));
              Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True);
          }

          [TearDown]
          public void TearDown()
          {
              _form.Close();
              _form.Dispose();
          }

          private HtmlMessage WaitFor(string strType, int nMs = 3000)
          {
              BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == strType), nMs);
              return _received.Find(m => m.Type == strType);
          }

          private HtmlMessage Ask(string strType, object payload = null)
          {
              return _host.Request(strType, payload, HtmlHostDefaults.RequestTimeout);
          }

          [TestCase("l1", "verseLineJump", "index", "3")]
          [TestCase("l2", "bibRefJump", "ref", "Gen 1:1")]
          [TestCase("l3", "openUrl", "url", "http://example.com/x")]
          public void LinkClick_SendsMessage_AndDoesNotNavigate(string strId, string strType, string strField, string strValue)
          {
              _host.Post("click", new { id = strId });
              Assert.That(WaitFor(strType)?.GetString(strField), Is.EqualTo(strValue));
              Assert.That(Ask("echo", new { text = "x" })?.GetString("text"), Is.EqualTo("x"));
          }

          [Test]
          public void ActionButton_SendsActionWithNameIdAndArg()
          {
              _host.Post("click", new { id = "btn_1_0_1" });
              var msg = WaitFor("action");
              Assert.That(msg.GetString("name"), Is.EqualTo("convertToMentoree"));
              Assert.That(msg.GetBool("arg"), Is.True);
          }

          [Test]
          public void InputEvent_SendsTextChanged_WithValue()
          {
              _host.Post("typeWithInput", new { id = "ta_1_0", text = "typed" });
              var msg = WaitFor("textChanged");
              Assert.That(msg.GetString("value"), Is.EqualTo("typed"));
              Assert.That(msg.GetString("ieHtml"), Is.Null);
          }

          [Test]
          public void SetText_SendsTextChanged()
          {
              _host.Post("setText", new { id = "ta_1_0", text = "new text" });
              Assert.That(WaitFor("textChanged")?.GetString("value"), Is.EqualTo("new text"));
          }

          [Test]
          public void Flush_SendsLastFocusedTextQuietly_ThenReplies()
          {
              _host.Post("focusOn", new { id = "ta_1_0" });
              _host.Post("typeInto", new { id = "ta_1_0", text = "no input event" });
              BrowserTestHelper.Pump(200);
              Assert.That(Ask("flush"), Is.Not.Null);
              var msg = _received.Find(m => m.Type == "textChanged");
              Assert.That(msg?.GetString("value"), Is.EqualTo("no input event"));
              Assert.That(msg.GetBool("quiet"), Is.True);
          }

          [Test]
          public void Flush_ReadOnlyBox_SendsNothing()
          {
              _host.Post("focusOn", new { id = "ta_ro" });
              BrowserTestHelper.Pump(200);
              Assert.That(Ask("flush"), Is.Not.Null);
              Assert.That(_received.Exists(m => m.Type == "textChanged"), Is.False);
          }

          [Test]
          public void GetSelection_And_ReplaceSelection_UseTheTextareaSelection()
          {
              _host.Post("selectIn", new { id = "ta_1_0", start = 1, end = 3 });
              BrowserTestHelper.Pump(200);
              var sel = Ask("getSelection", new { id = "ta_1_0" });
              Assert.That(sel.GetString("selType"), Is.EqualTo("text"));
              Assert.That(sel.GetString("text"), Is.EqualTo("bc"));
              var reply = Ask("replaceSelection", new { id = "ta_1_0", text = "XYZ" });
              Assert.That(reply.GetString("value"), Is.EqualTo("aXYZdef"));
              Assert.That(reply.TryGetInt("endPoint", out var n) && n == 4, Is.True);
              Assert.That(reply.GetString("ieHtml"), Is.Null);
          }

          [Test]
          public void SelectRange_SelectsInTheTextarea()
          {
              _host.Post("selectRange", new { id = "ta_1_0", start = 2, length = 3 });
              BrowserTestHelper.Pump(200);
              Assert.That(Ask("getSelection", new { id = "ta_1_0" })?.GetString("text"), Is.EqualTo("cde"));
          }

          [Test]
          public void ScrollTo_ReportsTheTopLineAndItsNeighbours()
          {
              _received.Clear();
              _host.Post("scrollTo", new { id = "ln_20", alignTop = true });
              Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => (m.Type == "scrolled") && (m.GetString("topLabel") ?? "").EndsWith("20"))), Is.True);
              var msg = _received.FindLast(m => m.Type == "scrolled");
              Assert.That(msg.GetString("prevId"), Is.EqualTo("ln_19"));
              Assert.That(msg.GetString("nextId"), Is.EqualTo("ln_21"));
          }

          [Test]
          public void Drop_OnDropTarget_SendsScriptureDropped()
          {
              _host.Post("fire", new { id = "anc_2", what = "drop" });
              Assert.That(WaitFor("scriptureDropped")?.GetString("id"), Is.EqualTo("anc_2"));
          }

          [Test]
          public void SetHtml_And_HasElement()
          {
              _host.Post("setHtml", new { id = "tp_1_0_0", html = "<i>x</i>" });
              Assert.That(Ask("innerHtml", new { id = "tp_1_0_0" })?.GetString("html"), Is.EqualTo("<i>x</i>"));
              Assert.That(Ask("hasElement", new { id = "ta_1_0" })?.GetBool("found"), Is.True);
              Assert.That(Ask("hasElement", new { id = "nope" })?.GetBool("found", true), Is.False);
          }

          [Test]
          public void Page_RunsInStandardsMode_WithoutScriptErrors()
          {
              BrowserTestHelper.Pump(300);
              Assert.That(_received.FindAll(m => m.Type == HtmlMessage.CstrTypeJsError).ConvertAll(m => m.GetString("message")), Is.Empty);
          }
      }
  }
  ```

  Add to `StoryBtPaneTests.cs`:

  ```csharp
          [Test]
          public void SetSelectedText_AcceptsAModernValueReply()
          {
              var st = new StringTransfer("old", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_StoryLine_0_0_Vernacular" };
              _host.OnRequest = m => FakeHtmlHost.Reply(new { endPoint = 3, value = "a &amp; b" });
              Assert.That(_pane.SetSelectedText(st, "x", out var nEnd), Is.True);
              Assert.That(st.ToString(), Is.EqualTo("a &amp; b"), "a 'value' is the box's plain text: used as is, not decoded");
          }
  ```

- [ ] **Step 2: Run them.** Expected: compile failure, because `PageScripts.For` doesn't exist and the builders have no
  engine parameter.

- [ ] **Step 3: `PageScripts.For` and `PageScriptSet`.**

  Add to `PageScripts.cs`:

  ```csharp
          public static PageScriptSet For(HtmlEngine engine)
          {
              return new PageScriptSet(engine);
          }
  ```

  Add a new class in the same file:

  ```csharp
      /// <summary>
      /// the page scripts for one engine. IE: today's quirks-mode scripts, untouched. WebView2: js/modern/*, standards
      /// mode. Both speak the same message protocol (docs/superpowers/specs/2026-10-04-webview2-host-design.md)
      /// </summary>
      internal sealed class PageScriptSet
      {
          private static readonly System.Text.RegularExpressions.Regex RegexLeadingDoctype =
              new System.Text.RegularExpressions.Regex(@"^\s*<!DOCTYPE[^>]*>\s*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

          public PageScriptSet(HtmlEngine engine)
          {
              Engine = engine;
          }

          public HtmlEngine Engine { get; }
          private bool IsIe => Engine == HtmlEngine.Ie;

          public string Bridge => IsIe ? PageScripts.Get("bridge.js") : PageScripts.Get("modern.bridge.js");
          public string PaneCommon => IsIe ? PageScripts.Get("PaneCommon.js") : PageScripts.Get("modern.PaneCommon.js");
          public string StoryBt => IsIe ? Properties.Resources.StoryBtJs : PageScripts.Get("modern.StoryBt.js");
          public string StoryBtEnd => IsIe ? Properties.Resources.StoryBtPsJs : String.Empty;   // (modern: all in StoryBt)
          public string ConNote => IsIe ? Properties.Resources.ConNoteDomPrefix : PageScripts.Get("modern.ConNote.js");
          public string NetBible => IsIe ? PageScripts.Get("NetBible.js") : PageScripts.Get("modern.NetBible.js");
          public string HoverLinks => IsIe ? PageScripts.Get("HoverLinks.js") : PageScripts.Get("modern.HoverLinks.js");
          public string JQuery => IsIe ? Properties.Resources.jquery_min : String.Empty;

          // IE: the page as it is (quirks mode, as the pages have always run). WebView2: standards mode
          public string ApplyDoctype(string strHtml)
          {
              if (IsIe)
                  return strHtml;
              return "<!DOCTYPE html>\r\n" + RegexLeadingDoctype.Replace(strHtml, String.Empty);
          }
      }
  ```

  In `StoryEditor.csproj`, add (keeping the existing `js\…` lines):

  ```xml
      <EmbeddedResource Include="js\modern\bridge.js" LogicalName="OneStoryProjectEditor.js.modern.bridge.js" />
      <EmbeddedResource Include="js\modern\PaneCommon.js" LogicalName="OneStoryProjectEditor.js.modern.PaneCommon.js" />
      <EmbeddedResource Include="js\modern\StoryBt.js" LogicalName="OneStoryProjectEditor.js.modern.StoryBt.js" />
      <EmbeddedResource Include="js\modern\ConNote.js" LogicalName="OneStoryProjectEditor.js.modern.ConNote.js" />
      <EmbeddedResource Include="js\modern\NetBible.js" LogicalName="OneStoryProjectEditor.js.modern.NetBible.js" />
      <EmbeddedResource Include="js\modern\HoverLinks.js" LogicalName="OneStoryProjectEditor.js.modern.HoverLinks.js" />
  ```

  (The csproj already copies `js\**` to the output; that's harmless.)

- [ ] **Step 4: Make the builders engine-aware.**

  `StoryData.cs`:

  ```csharp
          public string PresentationHtml(VerseData.ViewSettings viewSettings,
              ProjectSettings projSettings,
              TeamMembersData teamMembers,
              StoryData child,
              HtmlEngine engine)
          {
              // (body as before, ending with:)
              return AddHtmlHtmlDocOutside(strHtml, projSettings, engine);
          }

          public static string AddHtmlHtmlDocOutside(string strHtmlInside, ProjectSettings projSettings, HtmlEngine engine)
          {
              var scripts = PageScripts.For(engine);
              return scripts.ApplyDoctype(String.Format(Properties.Resources.StoryBtHtml,
                                   scripts.JQuery,
                                   scripts.StoryBt,
                                   GetPlaceHolders(projSettings),
                                   StylePrefix(projSettings, null, null),
                                   strHtmlInside,
                                   scripts.StoryBtEnd,
                                   PageScripts.ScriptBlock(scripts.Bridge, scripts.PaneCommon)));
          }

          // the scripts the note panes' HTML_Header puts in its <script> block
          private static string ConNotePageScripts(HtmlEngine engine)
          {
              var scripts = PageScripts.For(engine);
              return scripts.Bridge + Environment.NewLine + scripts.PaneCommon + Environment.NewLine + scripts.ConNote;
          }
  ```

  - `ConNoteHtml(…)`, `ConsultantNotesHtml(…)` and `CoachNotesHtml(…)` each get a last parameter
    `HtmlEngine engine`. Each passes `ConNotePageScripts(engine)` to `HTML_Header` and wraps its `String.Format(...)`
    in `PageScripts.For(engine).ApplyDoctype(...)`.
  - `GetPresentationHtmlForChorus` passes `HtmlEngine.Ie` (Chorus shows it in its own IE view).

  Callers pass their host's engine:
  - `HtmlStoryBtControl.LoadDocument`: `…PresentationHtml(…, Engine)`.
  - `HtmlConNoteControl` (both subclasses' `LoadDocument`, and `DoAddNote`'s `ConNoteHtml`): pass `Engine`.
    `DoAddNote`'s page goes into an `AddConNoteForm` whose own pane may differ, but both come from the same factory in
    one session, so the engines match.
  - `PrintForm.cs:78` and `LnCNotePrintForm.cs:15`: `StoryData.AddHtmlHtmlDocOutside(…, printViewer.webBrowser.Engine)`.
  - `NetBibleViewer.cs:582`: `PageScripts.ScriptBlock(PageScripts.For(_htmlHost.Engine).Bridge, PageScripts.For(_htmlHost.Engine).NetBible)`,
    and wrap the whole page in `PageScripts.For(_htmlHost.Engine).ApplyDoctype(...)`.
  - `HtmlForm.cs:65`: the same, with `HoverLinks`.
  - Tests that call these builders (`ConNotePageTests`, `StoryBtPageTests`, `PageScriptsTests`, `SaveGuardTests` and any
    others the compiler names): pass `HtmlEngine.Ie`, so the IE fixtures are unchanged.

- [ ] **Step 5: `SetSelectedText` accepts `value`.**

  In `HtmlVerseControl.SetSelectedText`, replace the `ieHtml` block with:

  ```csharp
              // now we have to update the string transfer with the new value: the modern page sends the box's plain
              //  'value'; the IE page sends its htmlText ('ieHtml'), which needs decoding
              var strValue = reply.GetString("value");
              var strIeHtml = reply.GetString("ieHtml");
              if (strValue != null)
                  stringTransfer.SetValue(strValue);
              else if (strIeHtml != null)
                  stringTransfer.SetValue(HtmlText.FromIeHtmlText(strIeHtml));
  ```

- [ ] **Step 6: Write `modern/bridge.js`.**

  `StoryEditor/js/modern/bridge.js`:

  ```js
  // modern/bridge.js: the WebView2 (Chromium) bridge -- the only modern script that names its host. Inlined first into
  //  every page when the host is WebView2. Same API as js/bridge.js (the IE one):
  //  ose.send(type, payload)  page -> C#
  //  ose.on(type, handler)    C# -> page; when the message has a 'rid', the handler's return value is the reply
  (function () {
      'use strict';
      var docId = '__OSE_DOC_ID__';   // replaced by the host on load, so it can ignore an older document's messages
      var handlers = {};

      function send(type, payload) {
          var msg = {}, k;
          payload = payload || {};
          for (k in payload)
              if (Object.prototype.hasOwnProperty.call(payload, k))
                  msg[k] = payload[k];
          msg.type = type;
          msg.doc = docId;
          try {
              window.chrome.webview.postMessage(JSON.stringify(msg));
          } catch (e) { }
      }

      function on(type, handler) {
          handlers[type] = handler;
      }

      function receive(str) {
          var msg, result = null, error = null;
          try {
              msg = JSON.parse(str);
          } catch (e) {
              send('jsError', { message: 'unparseable message from host: ' + e.message });
              return;
          }
          try {
              var handler = handlers[msg.type];
              if (handler)
                  result = handler(msg);
              else
                  error = 'no handler for ' + msg.type;
          } catch (e) {
              error = msg.type + ': ' + (e.message || String(e));
          }
          if (error)
              send('jsError', { message: error });
          if (msg.rid !== undefined) {
              var reply = (result && typeof result === 'object') ? result : {};
              reply.re = msg.rid;
              if (error)
                  reply.error = error;
              send('reply', reply);
          }
      }

      window.chrome.webview.addEventListener('message', function (e) { receive(e.data); });

      window.addEventListener('error', function (e) {
          send('jsError', { message: String(e.message), source: String(e.filename || ''), line: e.lineno || 0 });
      });

      // the nearest element (from el up) for which test(el) is true, or null
      function closest(el, test) {
          while (el && el.nodeType === 1) {
              if (test(el))
                  return el;
              el = el.parentNode;
          }
          return null;
      }

      function cancel(e) {
          e.preventDefault();
      }

      function listen(target, type, handler, useCapture) {
          target.addEventListener(type, handler, !!useCapture);
      }

      on('scrollTo', function (m) {
          var el = document.getElementById(m.id);
          if (!el)
              return { found: false };
          setTimeout(function () {
              el.scrollIntoView(m.alignTop !== false);
              if (m.focus) {
                  try { el.focus(); } catch (e) { }
              }
          }, 0);
          return { found: true };
      });

      window.addEventListener('load', function () {
          send('ready', { docMode: 0 });
      });

      window.ose = { send: send, on: on, closest: closest, cancel: cancel, listen: listen };
  })();
  ```

- [ ] **Step 7: Write `modern/PaneCommon.js`.**

  `StoryEditor/js/modern/PaneCommon.js`:

  ```js
  // modern/PaneCommon.js: what the Story/BT and note pages share, on WebView2. Same messages and commands as
  //  js/PaneCommon.js (the IE one), with standard DOM APIs. Inlined after modern/bridge.js and before the page's own
  //  script, which may replace a handler with ose.on (the last registration wins).
  (function () {
      'use strict';
      var lastTextareaId = null;

      function isTextarea(el) { return !!el && el.nodeName === 'TEXTAREA'; }
      function textarea(id) { var el = id ? document.getElementById(id) : null; return isTextarea(el) ? el : null; }

      document.addEventListener('focusin', function (e) {
          if (isTextarea(e.target))
              lastTextareaId = e.target.id;
      });

      // edits (typing, paste, cut, drop) -- the box's plain value
      document.addEventListener('input', function (e) {
          var ta = e.target;
          if (isTextarea(ta) && !ta.readOnly)
              ose.send('textChanged', { id: ta.id, value: ta.value });
      });

      // never let a click navigate the pane away; the links we make are sent to C# instead
      document.addEventListener('click', function (e) {
          var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
          if (link) {
              e.preventDefault();
              var href = link.getAttribute('href') || '';
              if (href === 'bibleViewer.setReference')
                  ose.send('bibRefJump', { ref: link.getAttribute('name') });
              else if (href === 'conNote.jumpToLine')
                  ose.send('verseLineJump', { index: link.getAttribute('name') });
              else if (/^https?:/i.test(href))
                  ose.send('openUrl', { url: href });
              return;
          }
          var btn = ose.closest(e.target, function (el) { return !!el.getAttribute('data-action'); });
          if (btn) {
              e.preventDefault();
              ose.send('action', { name: btn.getAttribute('data-action'), id: btn.id, arg: btn.getAttribute('data-arg') });
          }
      });

      document.addEventListener('keydown', function (e) {
          if (e.ctrlKey && (e.keyCode === 83)) {          // Ctrl+S
              e.preventDefault();
              ose.send('save');
          }
          else if (e.keyCode === 116) {                   // F5 (Ctrl+F5 also realigns the lines)
              e.preventDefault();
              ose.send(e.ctrlKey ? 'realign' : 'reload');
          }
          else if (e.ctrlKey && ((e.keyCode === 82) || (e.keyCode === 80))) {
              e.preventDefault();                         // the browser's own reload (Ctrl+R) and print (Ctrl+P)
          }
      });

      // a reference dragged from the Bible pane onto an anchor cell or a note box
      function dropTarget(el) {
          return ose.closest(el, function (x) { return x.getAttribute('data-drop') === 'scripture'; });
      }
      document.addEventListener('dragover', function (e) {
          if (dropTarget(e.target)) {
              e.preventDefault();
              if (e.dataTransfer)
                  e.dataTransfer.dropEffect = 'copy';
          }
      });
      document.addEventListener('drop', function (e) {
          var target = dropTarget(e.target);
          if (!target)
              return;
          e.preventDefault();
          ose.send('scriptureDropped', { id: target.id });
      });

      // which line is at the top (same rule as the IE page)
      function topOf(el) {
          var y = 0;
          while (el) {
              y += el.offsetTop;
              el = el.offsetParent;
          }
          return y;
      }
      var scrollTimer = null;
      function reportScroll() {
          scrollTimer = null;
          var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
          var cells = document.getElementsByTagName('td');
          var top = null, line = null;
          for (var i = 0; i < cells.length; i++) {
              var td = cells[i];
              if (!td.id)
                  continue;
              if (topOf(td) > scrollTop + 1)
                  break;
              top = td;
              if (td.id.indexOf('ln_') === 0)
                  line = td;
          }
          var msg = { topId: top ? top.id : null, topLabel: line ? line.innerText : null, prevId: null, nextId: null };
          if (line) {
              var n = parseInt(line.id.substr(3), 10);
              if (document.getElementById('ln_' + (n - 1)))
                  msg.prevId = 'ln_' + (n - 1);
              if (document.getElementById('ln_' + (n + 1)))
                  msg.nextId = 'ln_' + (n + 1);
          }
          ose.send('scrolled', msg);
      }
      window.addEventListener('scroll', function () {
          if (!scrollTimer)
              scrollTimer = setTimeout(reportScroll, 50);
      });
      window.addEventListener('load', reportScroll);

      // commands from C#
      ose.on('setText', function (m) {
          var ta = textarea(m.id);
          if (!ta)
              return;
          ta.value = m.text;
          ose.send('textChanged', { id: ta.id, value: ta.value });
      });

      ose.on('appendText', function (m) {
          var ta = textarea(m.id);
          if (!ta)
              return;
          ta.value += m.text;
          ose.send('textChanged', { id: ta.id, value: ta.value });
          if (m.focus)
              ta.focus();
      });

      ose.on('appendHtml', function (m) {
          var el = document.getElementById(m.id);
          if (el)
              el.insertAdjacentHTML('beforeend', m.html);
      });

      ose.on('setHtml', function (m) {
          var el = document.getElementById(m.id);
          if (el)
              el.innerHTML = m.html;
      });

      ose.on('hasElement', function (m) {
          return { found: !!document.getElementById(m.id) };
      });

      ose.on('selectRange', function (m) {
          var ta = textarea(m.id);
          if (!ta)
              return;
          ta.focus();
          ta.setSelectionRange(m.start, m.start + m.length);
      });

      ose.on('clearSelection', function () {
          var ta = textarea(lastTextareaId);
          if (ta)
              ta.setSelectionRange(ta.selectionEnd, ta.selectionEnd);
          var sel = window.getSelection();
          if (sel)
              sel.removeAllRanges();
      });

      // the selection C# asks about is in the box it names (or the last one focused); a box keeps its selection when
      //  focus moves to the app (e.g. the Find dialog)
      ose.on('getSelection', function (m) {
          var ta = textarea(m && m.id) || textarea(lastTextareaId);
          if (ta && (ta.selectionEnd > ta.selectionStart))
              return { selType: 'text', text: ta.value.substring(ta.selectionStart, ta.selectionEnd) };
          var sel = window.getSelection();
          if (sel && !sel.isCollapsed)
              return { selType: 'text', text: sel.toString() };
          return { selType: 'none', text: '' };
      });

      // replaces box 'id''s selection with text; returns the new end point (0 = failed) and the box's whole value
      function replaceSelectionIn(id, text) {
          var ta = textarea(id);
          if (!ta)
              return 0;
          ta.setRangeText(text, ta.selectionStart, ta.selectionEnd, 'end');
          return ta.selectionEnd;
      }

      ose.on('replaceSelection', function (m) {
          var nEndPoint = replaceSelectionIn(m.id, m.text);
          var ta = textarea(m.id);
          return { endPoint: nEndPoint, value: ta ? ta.value : null };
      });

      // send the last-focused box's text (it may have changed without an input event, e.g. set by script)
      ose.on('flush', function () {
          var ta = textarea(lastTextareaId);
          if (ta && !ta.readOnly)
              ose.send('textChanged', { id: ta.id, value: ta.value, quiet: true });
          return {};
      });

      ose.lastTextareaId = function () { return lastTextareaId; };
      ose.replaceSelectionIn = replaceSelectionIn;
  })();
  ```

- [ ] **Step 8: Run the tests.** Run `/Tests:PageScriptsTests,ModernPaneCommonPageTests,StoryBtPaneTests`, then the full
  suite. Expected: all pass. The IE fixtures are unchanged and pass `HtmlEngine.Ie`.

- [ ] **Step 9: Commit.**

  ```bash
  git add StoryEditor/PageScripts.cs StoryEditor/StoryData.cs StoryEditor/HtmlStoryBtControl.cs StoryEditor/HtmlConNoteControl.cs StoryEditor/PrintForm.cs StoryEditor/LnCNotePrintForm.cs StoryEditor/NetBibleViewer.cs StoryEditor/HtmlForm.cs StoryEditor/HtmlVerseControl.cs StoryEditor/StoryEditor.csproj StoryEditor/js/modern StoryEditor.Tests
  git status --short     # only this task's files
  git commit -m "C4: engine-aware page building; modern bridge and PaneCommon for WebView2"
  ```

---

### Task 5: Modern Bible-pane and popup pages, and the Bible-reference drag

**Files:**
- Modify: `StoryEditor/js/modern/NetBible.js`, `StoryEditor/js/modern/HoverLinks.js` (replacing the placeholders),
  `StoryEditor/NetBibleViewer.cs` (the `refDragStart`/`refDragEnd` handlers)
- Test: `StoryEditor.Tests/ModernSmallHostPageTests.cs`

**Interfaces:**
- Consumes: Task 4's modern bridge and `PageScripts.For`; Task 1's Q3 decision.
- Produces:
  - New page messages (WebView2 only): `refDragStart {target, ref}` and `refDragEnd`.
  - `NetBibleViewer` handles them: `refDragStart` sets `JumpTarget`/`ScriptureReference` (what
    `StoryEditor.GetNetBibleScriptureReference` returns to the drop target) and increments
    `StoryEditor.SuspendSaveDialog`; `refDragEnd` decrements it.

- [ ] **Step 1: Write the failing tests.**

  `StoryEditor.Tests/ModernSmallHostPageTests.cs`:

  ```csharp
  using System.Collections.Generic;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
      public class ModernSmallHostPageTests
      {
          private const string CstrDriver =
              "ose.on('clickLink', function () { document.getElementsByTagName('a')[0].click(); });" +
              "ose.on('mouseButton', function (m) { document.getElementsByTagName('button')[0].dispatchEvent(" +
              "  new MouseEvent(m.what, { bubbles: true, cancelable: true, button: 0 })); });" +
              "ose.on('dragStart', function () { var dt = new DataTransfer(); document.getElementsByTagName('button')[0]" +
              "  .dispatchEvent(new DragEvent('dragstart', { bubbles: true, cancelable: true, dataTransfer: dt }));" +
              "  return { data: dt.getData('text/plain'), allowed: dt.effectAllowed }; });" +
              "ose.on('dragEnd', function () { document.getElementsByTagName('button')[0].dispatchEvent(new DragEvent('dragend', { bubbles: true })); });" +
              "ose.on('isDraggable', function () { return { d: document.getElementsByTagName('button')[0].draggable }; });" +
              "ose.on('echo', function (m) { return { text: m.text }; });";

          private Form _form;
          private WebView2HtmlHost _host;
          private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

          [SetUp]
          public void SetUp()
          {
              _received.Clear();
              _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
              _host.MessageReceived += (s, m) => _received.Add(m);
          }

          [TearDown]
          public void TearDown()
          {
              _form.Close();
              _form.Dispose();
          }

          private void Load(string strBody, string strScript)
          {
              _host.LoadHtml(BrowserTestHelper.ModernPage(strBody, PageScripts.For(HtmlEngine.WebView2).Bridge, strScript, CstrDriver));
              Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True);
          }

          private HtmlMessage WaitFor(string strType)
          {
              BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == strType), 3000);
              return _received.Find(m => m.Type == strType);
          }

          [Test]
          public void HoverLinks_LinkClick_SendsInnerHtml_AndDoesNotNavigate()
          {
              Load("<a href=\"x\">Gen 1:1</a>", PageScripts.For(HtmlEngine.WebView2).HoverLinks);
              _host.Post("clickLink");
              Assert.That(WaitFor("hoverRef")?.GetString("ref"), Is.EqualTo("Gen 1:1"));
              Assert.That(_host.Request("echo", new { text = "still" }, HtmlHostDefaults.RequestTimeout)?.GetString("text"), Is.EqualTo("still"));
          }

          [Test]
          public void NetBible_LinkClick_SendsHrefAfterSixChars()
          {
              Load("<a href=\"sword:passagestudy.jsp?x\">1</a>", PageScripts.For(HtmlEngine.WebView2).NetBible);
              _host.Post("clickLink");
              Assert.That(WaitFor("hoverRef")?.GetString("ref"), Is.EqualTo("passagestudy.jsp?x"));
          }

          [TestCase("mousedown", "refMouseDown")]
          [TestCase("mouseup", "refMouseUp")]
          public void NetBible_ButtonMouse_SendsMessage(string strDomEvent, string strMessage)
          {
              // (the real verse buttons have no value attribute: the reference is the button's text)
              Load("<button id=\"Gen 1:2\">Genesis 1:2</button>", PageScripts.For(HtmlEngine.WebView2).NetBible);
              _host.Post("mouseButton", new { what = strDomEvent });
              var msg = WaitFor(strMessage);
              Assert.That(msg, Is.Not.Null);
              if (strMessage == "refMouseUp")
              {
                  Assert.That(msg.GetString("target"), Is.EqualTo("Gen 1:2"));
                  Assert.That(msg.GetString("ref"), Is.EqualTo("Genesis 1:2"));
              }
          }

          [Test]
          public void NetBible_VerseButtons_AreDraggable_AndDragStartTellsCSharp()
          {
              Load("<button id=\"Gen 1:2\">Genesis 1:2</button>", PageScripts.For(HtmlEngine.WebView2).NetBible);
              Assert.That(_host.Request("isDraggable", null, HtmlHostDefaults.RequestTimeout)?.GetBool("d"), Is.True);
              var reply = _host.Request("dragStart", null, HtmlHostDefaults.RequestTimeout);
              Assert.That(reply.GetString("data"), Is.EqualTo("Genesis 1:2"));
              var msg = WaitFor("refDragStart");
              Assert.That(msg.GetString("target"), Is.EqualTo("Gen 1:2"));
              Assert.That(msg.GetString("ref"), Is.EqualTo("Genesis 1:2"));
              _host.Post("dragEnd");
              Assert.That(WaitFor("refDragEnd"), Is.Not.Null);
              Assert.That(_received.Exists(m => m.Type == "refMouseOut"), Is.False, "WebView2 never starts a WinForms drag");
          }
      }
  }
  ```

  > **If Task 1's Q3 chose the WinForms text `DataObject` instead of the HTML5 drag:**
  > - Replace `NetBible_VerseButtons_AreDraggable_AndDragStartTellsCSharp` with a test that a `mouseout` on a button
  >   sends `refMouseOut {target, ref}`.
  > - Use the alternative `modern/NetBible.js` and C# in Step 3's note.

- [ ] **Step 2: Run them.** Expected: failures, because the modern scripts are still placeholders.

- [ ] **Step 3: Write the scripts and the C# handlers.**

  `StoryEditor/js/modern/HoverLinks.js`:

  ```js
  // modern/HoverLinks.js: the commentary/info popup (HtmlForm) on WebView2. Clicking a link shows that reference in the
  //  Bible pane.
  document.addEventListener('click', function (e) {
      var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
      if (!link)
          return;
      e.preventDefault();
      ose.send('hoverRef', { ref: link.innerHTML });
  });
  ```

  `StoryEditor/js/modern/NetBible.js`:

  ```js
  // modern/NetBible.js: the Bible pane (NetBibleViewer) on WebView2. Footnote links show a tooltip; verse buttons can be
  //  clicked (show commentary) or dragged (HTML5 drag) onto an anchor cell or a note box in another pane.
  (function () {
      'use strict';
      function isButton(el) { return el.nodeName === 'BUTTON'; }
      // the real verse buttons have no value attribute; IE (quirks) reported their text as the value, so do the same
      function refOf(btn) { return btn.getAttribute('value') || btn.textContent; }

      document.addEventListener('click', function (e) {
          var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
          if (!link)
              return;
          e.preventDefault();
          ose.send('hoverRef', { ref: link.getAttribute('href').substr(6) });
      });

      document.addEventListener('mousedown', function (e) {
          if (!ose.closest(e.target, isButton))
              return;
          ose.send('refMouseDown');
      });

      document.addEventListener('mouseup', function (e) {
          var btn = ose.closest(e.target, isButton);
          if (!btn)
              return;
          ose.send('refMouseUp', { target: btn.id, ref: refOf(btn) });
      });

      document.addEventListener('dragstart', function (e) {
          var btn = ose.closest(e.target, isButton);
          if (!btn)
              return;
          e.dataTransfer.setData('text/plain', refOf(btn));
          e.dataTransfer.effectAllowed = 'copy';
          ose.send('refDragStart', { target: btn.id, ref: refOf(btn) });
      });

      document.addEventListener('dragend', function (e) {
          if (ose.closest(e.target, isButton))
              ose.send('refDragEnd');
      });

      window.addEventListener('DOMContentLoaded', function () {
          var buttons = document.getElementsByTagName('button');
          for (var i = 0; i < buttons.length; i++)
              buttons[i].draggable = true;
      });
  })();
  ```

  > The IE page cancels `mousedown` on buttons (`return false`). The modern page doesn't, because cancelling
  > `mousedown` would stop the HTML5 drag from starting.

  `NetBibleViewer.cs`, in the constructor next to the other `_dispatcher.Register` calls:

  ```csharp
              // WebView2: the verse buttons are dragged with HTML5 drag-and-drop (no WinForms DoDragDrop); the drop target
              //  asks StoryEditor.GetNetBibleScriptureReference, i.e. JumpTarget, as it always has
              _dispatcher.Register("refDragStart", m =>
              {
                  JumpTarget = m.GetString("target");
                  ScriptureReference = m.GetString("ref");
                  StoryEditor.SuspendSaveDialog++;
                  _bHtmlDragInProgress = true;
              });
              _dispatcher.Register("refDragEnd", m =>
              {
                  if (_bHtmlDragInProgress)
                  {
                      _bHtmlDragInProgress = false;
                      StoryEditor.SuspendSaveDialog--;
                  }
              });
  ```

  Add the field `private bool _bHtmlDragInProgress;`. If `JumpTarget`'s setter does more than store the value, read it
  first and keep the same order the `OnMouseOut` path uses.

  > **Alternative if Task 1's Q3 found HTML5 drag doesn't work between WebView2 controls, but a WinForms `UnicodeText`
  > drag does:**
  > - `modern/NetBible.js` drops the `dragstart`/`dragend` handlers and the `draggable` loop, and sends
  >   `refMouseOut {target, ref: refOf(btn)}` on `mouseout` of a button, as the IE page does.
  > - In `NetBibleViewer.OnMouseOut`, when `_htmlHost.Engine == HtmlEngine.WebView2`, call
  >   `_htmlHost.Control.DoDragDrop(new DataObject(DataFormats.UnicodeText, strScriptureReference ?? strJumpTarget), DragDropEffects.Copy | DragDropEffects.Link)`
  >   in place of `DoDragDrop(this, …)`. Leave the IE branch exactly as it is.
  > - Don't register `refDragStart`/`refDragEnd`.

- [ ] **Step 4: Run the tests.** Run `/Tests:ModernSmallHostPageTests,SmallHostPageTests`, then the full suite.
  Expected: all pass. The IE `SmallHostPageTests` are unchanged.

- [ ] **Step 5: Real-drag check, if the spike harness is still in the scratchpad.**

  Re-run its Q3 drag between two WebView2 controls, loading this task's real `modern/NetBible.js` (with the modern
  bridge) as the source page and `modern/PaneCommon.js` with a `data-drop` cell as the target. Record in the report that
  the target received `scriptureDropped`.

- [ ] **Step 6: Commit.**

  ```bash
  git add StoryEditor/js/modern/NetBible.js StoryEditor/js/modern/HoverLinks.js StoryEditor/NetBibleViewer.cs StoryEditor.Tests/ModernSmallHostPageTests.cs
  git commit -m "C5: modern Bible-pane and popup pages; HTML5 drag of verse references"
  ```

---
### Task 6: Modern note pages (`modern/ConNote.js`)

**Files:**
- Modify: `StoryEditor/js/modern/ConNote.js` (replacing the placeholder)
- Modify (tests): `StoryEditor.Tests/PaneTestData.cs` (a shared page builder), `StoryEditor.Tests/ConNotePageTests.cs`
  (uses it)
- Test: `StoryEditor.Tests/ModernConNotePageTests.cs`

**Interfaces:**
- Consumes: Task 4 (modern bridge and PaneCommon; `ConsultantNotesHtml(…, HtmlEngine engine)`); B's `NoteActions`.
- Produces:
  - Modern note page messages, the same as the IE page: `textareaMouseDown {id, value, button}` (button converted to
    the IE values), `contextMenu {}`, and `textChanged` (from PaneCommon's `input`).
  - The command `getNoteSelection → {lineIndex, html} | {}`.
  - `PaneTestData.ConsultantNotesPage(HtmlEngine engine)` returns `string`.

- [ ] **Step 1: Share the page builder.**

  Move the code in `ConNotePageTests` that builds the Consultant Notes page into
  `internal static string PaneTestData.ConsultantNotesPage(HtmlEngine engine)`, unchanged except that it passes `engine`
  to `ConsultantNotesHtml`. That is: load the project, pick the story, set `CraftingInfo.ProjectFacilitator`, and loop
  over team members until the page has an Add Note button.

  `ConNotePageTests` then calls `PaneTestData.ConsultantNotesPage(HtmlEngine.Ie)`.

  Run `/Tests:ConNotePageTests`. Expected: it still passes.

- [ ] **Step 2: Write the failing modern tests.**

  `StoryEditor.Tests/ModernConNotePageTests.cs`:

  ```csharp
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
      public class ModernConNotePageTests
      {
          private const string CstrDriver =
              "ose.on('clickFirst', function (m) { var els = document.querySelectorAll('[data-action=\"' + m.action + '\"]');" +
              "  if (els.length) els[0].click(); return { n: els.length }; });" +
              "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
              "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes;" +
              "    for (var j = 0; j < a.length; j++) if (/^on/i.test(a[j].name)) n++; } return { n: n }; });" +
              // select the text of the first read-only note paragraph (id tp_<line>_<conv>_<i>)
              "ose.on('selectFirstNoteParagraph', function () { var p = document.querySelector('[id^=\"tp_\"]');" +
              "  if (!p) return { id: null }; var r = document.createRange(); r.selectNodeContents(p);" +
              "  var s = window.getSelection(); s.removeAllRanges(); s.addRange(r); return { id: p.id }; });" +
              "ose.on('noteBox', function () { var t = document.querySelector('[data-note=\"edit\"]'); return { id: t ? t.id : null }; });" +
              "ose.on('ctrlB', function (m) { var t = document.getElementById(m.id); t.focus(); t.value = 'say hello now'; t.setSelectionRange(4, 9);" +
              "  t.dispatchEvent(new KeyboardEvent('keydown', { bubbles: true, cancelable: true, ctrlKey: true, keyCode: 66 })); return { value: t.value }; });" +
              "ose.on('leftDown', function (m) { document.getElementById(m.id).dispatchEvent(new MouseEvent('mousedown', { bubbles: true, button: 0 })); });" +
              "ose.on('rightClick', function () { document.body.dispatchEvent(new MouseEvent('contextmenu', { bubbles: true, cancelable: true, button: 2 })); });";

          private Form _form;
          private WebView2HtmlHost _host;
          private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

          [SetUp]
          public void SetUp()
          {
              _received.Clear();
              _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
              _host.MessageReceived += (s, m) => _received.Add(m);
              var strHtml = PaneTestData.ConsultantNotesPage(HtmlEngine.WebView2);
              _host.LoadHtml(strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
              Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True, "page never sent 'ready'");
          }

          [TearDown]
          public void TearDown()
          {
              _form.Close();
              _form.Dispose();
          }

          private HtmlMessage Ask(string strType, object payload = null) => _host.Request(strType, payload, HtmlHostDefaults.RequestTimeout);

          private HtmlMessage WaitFor(string strType)
          {
              BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == strType), 3000);
              return _received.Find(m => m.Type == strType);
          }

          [Test]
          public void Page_LoadsWithoutScriptErrors_AndHasNoInlineHandlers()
          {
              BrowserTestHelper.Pump(300);
              Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
              Assert.That(Ask("countInline").TryGetInt("n", out var n) && n == 0, Is.True);
          }

          [Test]
          public void AddNoteButton_SendsAction()
          {
              Assert.That(Ask("clickFirst", new { action = NoteActions.AddNote }).TryGetInt("n", out var n) && n > 0, Is.True);
              Assert.That(WaitFor("action")?.GetString("name"), Is.EqualTo(NoteActions.AddNote));
          }

          [Test]
          public void GetNoteSelection_NothingSelected_GivesNoLine()
          {
              Assert.That(Ask("getNoteSelection").TryGetInt("lineIndex", out _), Is.False);
          }

          [Test]
          public void GetNoteSelection_InANoteParagraph_GivesItsLineAndHtml()
          {
              var strId = Ask("selectFirstNoteParagraph").GetString("id");
              if (strId == null)
                  Assert.Inconclusive("the test project's notes page has no read-only note paragraph");
              var reply = Ask("getNoteSelection");
              Assert.That(reply.TryGetInt("lineIndex", out var nLine), Is.True);
              Assert.That(nLine, Is.EqualTo(int.Parse(strId.Split('_')[1])));
              Assert.That(reply.GetString("html"), Is.Not.Empty);
          }

          [Test]
          public void CtrlB_WrapsTheSelectionInDollars_AndSendsTheText()
          {
              var strId = Ask("noteBox").GetString("id");
              if (strId == null)
                  Assert.Inconclusive("the test project's notes page has no editable note box");
              Assert.That(Ask("ctrlB", new { id = strId }).GetString("value"), Is.EqualTo("say $hello$  now"));
              Assert.That(WaitFor("textChanged")?.GetString("value"), Is.EqualTo("say $hello$  now"));
          }

          [Test]
          public void NoteBox_LeftMouseDown_ReportsIeButtonValue()
          {
              var strId = Ask("noteBox").GetString("id");
              if (strId == null)
                  Assert.Inconclusive("the test project's notes page has no editable note box");
              _host.Post("leftDown", new { id = strId });
              Assert.That(WaitFor("textareaMouseDown").TryGetInt("button", out var n) && n == 1, Is.True, "left = 1, as C# expects");
          }

          [Test]
          public void RightClick_AsksForTheContextMenu()
          {
              _host.Post("rightClick");
              Assert.That(WaitFor("contextMenu"), Is.Not.Null);
          }
      }
  }
  ```

  > `"say $hello$  now"` follows today's rule: the selection "hello" is replaced by `$hello$` plus a trailing space,
  > which leaves two spaces before "now". Check that against `ConNoteDomPrefix.js`'s `transformText` and adjust the
  > expectation only if the IE code differs.

- [ ] **Step 3: Run them.** Expected: failures, because `ConNote.js` is still a placeholder.

- [ ] **Step 4: Write `modern/ConNote.js`.**

  ```js
  // modern/ConNote.js: the Consultant/Coach note pages on WebView2. Same messages as js/ConNoteDomPrefix.js (the IE one).
  //  Inlined after modern/bridge.js and modern/PaneCommon.js (links, buttons, keys, drops, scrolling, edits).
  (function () {
      'use strict';
      function legacyButton(b) { return (b === 0) ? 1 : (b === 2) ? 2 : 4; }    // the values C# (and IE) use
      function noteBox(el) { return ose.closest(el, function (x) { return x.getAttribute('data-note') === 'edit'; }); }

      // Ctrl+B / Ctrl+I in the note box wrap the selection in $...$ / *...*
      document.addEventListener('keydown', function (e) {
          var ta = noteBox(e.target);
          if (!ta || !e.ctrlKey)
              return;
          var mark = (e.keyCode === 66) ? '$' : (e.keyCode === 73) ? '*' : null;
          if (!mark)
              return;
          e.preventDefault();
          var sel = ta.value.substring(ta.selectionStart, ta.selectionEnd).trim();
          if (!sel)
              return;
          var bWrapped = (sel.lastIndexOf(mark, 0) === 0) && (sel.indexOf(mark, sel.length - mark.length) !== -1);
          if (!bWrapped) {
              ta.setRangeText(mark + sel + mark + ' ', ta.selectionStart, ta.selectionEnd, 'end');
              ta.dispatchEvent(new Event('input', { bubbles: true }));    // so PaneCommon sends the new text
          }
      });

      document.addEventListener('mousedown', function (e) {
          var ta = e.target;
          if (ta && ta.nodeName === 'TEXTAREA')
              ose.send('textareaMouseDown', { id: ta.id, value: ta.value, button: legacyButton(e.button) });
      });

      // right-click anywhere asks C# for the note context menu
      document.addEventListener('contextmenu', function (e) {
          e.preventDefault();
          ose.send('contextMenu', {});
      });

      // the selection in a read-only part of the pane, and the line it's on, for "Add note on selected text"
      ose.on('getNoteSelection', function () {
          var sel = window.getSelection();
          if (!sel || sel.isCollapsed || !sel.rangeCount)
              return {};
          var range = sel.getRangeAt(0);
          var el = range.commonAncestorContainer;
          if (el.nodeType !== 1)
              el = el.parentNode;
          var re = /^tp_(\d+)_/;
          for (var x = el; x && x.nodeType === 1; x = x.parentNode) {
              var match = re.exec(x.id || '');
              if (!match) {
                  var inner = x.querySelector('[id^="tp_"]');
                  match = inner ? re.exec(inner.id) : null;
              }
              if (match) {
                  var div = document.createElement('div');
                  div.appendChild(range.cloneContents());
                  return { lineIndex: parseInt(match[1], 10), html: div.innerHTML };
              }
          }
          return {};
      });
  })();
  ```

  > The IE page's word-select double-click workaround isn't needed: Chromium selects words properly. Double-clicks on
  > `[data-note]` use the browser default.

- [ ] **Step 5: Run the tests.** Run `/Tests:ModernConNotePageTests,ConNotePageTests`, then the full suite. Expected:
  all pass.

- [ ] **Step 6: Commit.**

  ```bash
  git add StoryEditor/js/modern/ConNote.js StoryEditor.Tests/PaneTestData.cs StoryEditor.Tests/ConNotePageTests.cs StoryEditor.Tests/ModernConNotePageTests.cs
  git commit -m "C6: modern note pages for WebView2"
  ```

---

### Task 7: Modern Story/BT page with the mirror highlight; turn WebView2 on

**Files:**
- Modify: `StoryEditor/js/modern/StoryBt.js` (replacing the placeholder), `StoryEditor/HtmlHostFactory.cs`
  (`UseWebView2WhenAvailable = true`)
- Modify (tests): `StoryEditor.Tests/PaneTestData.cs` (a shared Story/BT page builder), `StoryEditor.Tests/StoryBtPageTests.cs`
  (uses it)
- Test: `StoryEditor.Tests/ModernStoryBtPageTests.cs`

**Interfaces:**
- Consumes:
  - Task 4: `AddHtmlHtmlDocOutside(…, engine)`, the modern bridge and PaneCommon (`ose.lastTextareaId()`).
  - B: `HighlightedText.FromReply`, `ReferringTextBuilder.TryBuild`, `StoryBtActions`.
- Produces:
  - Modern Story/BT page messages, the same as the IE page: `focus`, `blur`, `contextMenu {id}`, `textareaMouseUp`,
    `textareaMouseDown {id, value, button}` (IE values), `mouseMove`, `action anchorMenu/lineOptions`, `bibRefJump`.
  - Commands: `getHighlights {tableId} → {items:[{textareaId, className, text}]}` and `clearHighlight {id}`.
  - `PaneTestData.StoryBtPage(HtmlEngine engine)` returns `string`.

- [ ] **Step 1: Share the page builder.**

  Move the code in `StoryBtPageTests.SetUp` that builds the page (project, story, `ViewSettings`, `PresentationHtml`)
  into `internal static string PaneTestData.StoryBtPage(HtmlEngine engine)`, passing `engine` through.
  `StoryBtPageTests` calls it with `HtmlEngine.Ie`.

  Run `/Tests:StoryBtPageTests`. Expected: it still passes.

- [ ] **Step 2: Write the failing modern tests.**

  `StoryEditor.Tests/ModernStoryBtPageTests.cs`:

  ```csharp
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;

  namespace OneStoryProjectEditor.Tests
  {
      [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
      public class ModernStoryBtPageTests
      {
          private const string CstrDriver =
              // a user selecting characters [start, end) in a box and leaving it
              "ose.on('selectIn', function (m) { var t = document.getElementById(m.id); t.focus(); t.setSelectionRange(m.start, m.end);" +
              "  t.dispatchEvent(new MouseEvent('mouseup', { bubbles: true, button: 0 })); t.blur(); });" +
              "ose.on('setValue', function (m) { var t = document.getElementById(m.id); t.value = m.text; });" +
              "ose.on('typeIn', function (m) { var t = document.getElementById(m.id); t.focus(); t.value = t.value + m.text;" +
              "  t.dispatchEvent(new Event('input', { bubbles: true })); t.blur(); });" +
              "ose.on('leftClick', function (m) { var t = document.getElementById(m.id);" +
              "  t.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, button: 0 })); t.setSelectionRange(0, 0);" +
              "  t.dispatchEvent(new MouseEvent('mouseup', { bubbles: true, button: 0 })); });" +
              "ose.on('mirror', function (m) { var t = document.getElementById(m.id), mi = t.previousElementSibling;" +
              "  if (!mi || mi.className !== 'oseMirror') return { has: false };" +
              "  var a = t.getBoundingClientRect(), b = mi.getBoundingClientRect(), mk = mi.querySelector('mark');" +
              "  return { has: true, dx: Math.abs(a.left - b.left), dy: Math.abs(a.top - b.top), dw: Math.abs(a.width - b.width)," +
              "    dh: Math.abs(a.height - b.height), text: mi.textContent, mark: mk ? mk.textContent : null," +
              "    transparent: window.getComputedStyle(t).backgroundColor }; });" +
              "ose.on('box', function (m) { var t = document.getElementById(m.id); return { placeholder: t.placeholder, readOnly: t.readOnly }; });" +
              "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
              "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes;" +
              "    for (var j = 0; j < a.length; j++) if (/^on/i.test(a[j].name)) n++; } return { n: n }; });";

          private Form _form;
          private WebView2HtmlHost _host;
          private string _strHtml;
          private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

          [SetUp]
          public void SetUp()
          {
              _received.Clear();
              _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
              _host.MessageReceived += (s, m) => _received.Add(m);
              _strHtml = PaneTestData.StoryBtPage(HtmlEngine.WebView2);
              _host.LoadHtml(_strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
              Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True, "page never sent 'ready'");
          }

          [TearDown]
          public void TearDown()
          {
              _form.Close();
              _form.Dispose();
          }

          private HtmlMessage Ask(string strType, object payload = null) => _host.Request(strType, payload, HtmlHostDefaults.RequestTimeout);

          private List<HighlightedText> Highlights(int nLine) =>
              HighlightedText.FromReply(Ask("getHighlights", new { tableId = VerseData.GetLineTableId(nLine) }));

          private string StoryLineBox(int nLine, int nNth = 0)
          {
              var matches = Regex.Matches(_strHtml, "id=\"(ta_" + nLine + "_StoryLine_0_0_[A-Za-z]+)\"");
              Assert.That(matches.Count, Is.GreaterThan(nNth), "not enough story line boxes on line " + nLine);
              return matches[nNth].Groups[1].Value;
          }

          [Test]
          public void Page_LoadsWithoutScriptErrors_AndHasNoInlineHandlers()
          {
              BrowserTestHelper.Pump(300);
              Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
              Assert.That(Ask("countInline").TryGetInt("n", out var n) && n == 0, Is.True);
          }

          [Test]
          public void Selection_BecomesAHighlight_WithIeShapedItems_AndClears()
          {
              var strId = StoryLineBox(1);
              _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
              _host.Post("selectIn", new { id = strId, start = 0, end = 3 });
              BrowserTestHelper.Pump(200);
              var items = Highlights(1);
              Assert.That(items.Count, Is.EqualTo(1));
              Assert.That(items[0].TextareaId, Is.EqualTo(strId));
              Assert.That(items[0].Text, Is.EqualTo("abc"));
              Assert.That(items[0].ClassName, Does.EndWith(" highlight"));
              Assert.That(ReferringTextBuilder.TryBuild(items, out var strReferring) && strReferring.Contains(">abc</span>"), Is.True);

              _host.Post("clearHighlight", new { id = strId });
              Assert.That(Highlights(1), Is.Empty);
          }

          [Test]
          public void HighlightsInTwoBoxes_AreBothKept()
          {
              var strA = StoryLineBox(1, 0);
              var strB = StoryLineBox(1, 1);
              _host.Post("setValue", new { id = strA, text = "first box" });
              _host.Post("setValue", new { id = strB, text = "second box" });
              _host.Post("selectIn", new { id = strA, start = 0, end = 5 });
              _host.Post("selectIn", new { id = strB, start = 7, end = 10 });
              BrowserTestHelper.Pump(200);
              Assert.That(Highlights(1).Select(h => h.Text), Is.EquivalentTo(new[] { "first", "box" }));
          }

          [Test]
          public void Edit_ClearsTheBoxHighlight()
          {
              var strId = StoryLineBox(1);
              _host.Post("setValue", new { id = strId, text = "abcdef" });
              _host.Post("selectIn", new { id = strId, start = 1, end = 4 });
              _host.Post("typeIn", new { id = strId, text = "X" });
              BrowserTestHelper.Pump(200);
              Assert.That(Highlights(1), Is.Empty, "a box's highlight must not survive an edit of that box");
          }

          [Test]
          public void LeftClick_ClearsTheBoxHighlight()
          {
              var strId = StoryLineBox(1);
              _host.Post("setValue", new { id = strId, text = "abcdef" });
              _host.Post("selectIn", new { id = strId, start = 1, end = 4 });
              _host.Post("leftClick", new { id = strId });
              BrowserTestHelper.Pump(200);
              Assert.That(Highlights(1), Is.Empty);
          }

          [Test]
          public void Mirror_LinesUpWithTheBox_AndMarksTheRange()
          {
              var strId = StoryLineBox(1);
              _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
              _host.Post("selectIn", new { id = strId, start = 2, end = 5 });
              BrowserTestHelper.Pump(200);
              var m = Ask("mirror", new { id = strId });
              Assert.That(m.GetBool("has"), Is.True, "a highlighted box gets a mirror");
              foreach (var strDelta in new[] { "dx", "dy", "dw", "dh" })
                  Assert.That(m.Body[strDelta].ToObject<double>(), Is.LessThan(1.0), strDelta);
              Assert.That(m.GetString("text"), Is.EqualTo("abcdef ghi\n"));
              Assert.That(m.GetString("mark"), Is.EqualTo("cde"));
              Assert.That(m.GetString("transparent"), Is.EqualTo("rgba(0, 0, 0, 0)"));

              _host.Post("clearHighlight", new { id = strId });
              BrowserTestHelper.Pump(100);
              Assert.That(Ask("mirror", new { id = strId }).GetBool("has"), Is.False, "the mirror goes when the highlight does");
          }

          [Test]
          public void Boxes_UseNativePlaceholders()
          {
              var r = Ask("box", new { id = StoryLineBox(1) });
              Assert.That(r.GetString("placeholder"), Is.Not.Empty, "the language name is the native placeholder");
          }

          [Test]
          public void SetText_ThenFlush_RoundTripsAwkwardText()
          {
              var strId = StoryLineBox(1);
              var strText = "say \"hi\" <donkey bray> & more\nnext line " + HtmlMessageTests.AwkwardNonAscii;
              _host.Post("setText", new { id = strId, text = strText });
              Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "textChanged")), Is.True);
              Assert.That(_received.First(m => m.Type == "textChanged").GetString("value"), Is.EqualTo(strText));
          }
      }
  }
  ```

- [ ] **Step 3: Run them.** Expected: failures, because `StoryBt.js` is still a placeholder.

- [ ] **Step 4: Write `modern/StoryBt.js`.**

  ```js
  // modern/StoryBt.js: the Story/BT page on WebView2. A selection made in a box becomes that box's highlight (one per
  //  box; highlights in several boxes add up) for "Add note on selected text". Chromium can't draw inside a textarea, so a
  //  highlighted box gets a mirror layer behind it (same text metrics; the range in a <mark>) and goes transparent.
  //  Same messages as js/StoryBt.js + StoryBtPs.js (the IE ones). Inlined after modern/bridge.js and modern/PaneCommon.js.
  (function () {
      'use strict';
      var highlights = {};      // textarea id -> { start, end }
      var mirrors = {};         // textarea id -> its mirror div
      var lastMouseMove = 0;
      var COPIED = ['fontFamily', 'fontSize', 'fontWeight', 'fontStyle', 'fontVariant', 'lineHeight', 'letterSpacing',
                    'wordSpacing', 'textAlign', 'textIndent', 'textTransform', 'direction', 'tabSize', 'boxSizing',
                    'paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft',
                    'borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth'];

      function isTextarea(el) { return !!el && el.nodeName === 'TEXTAREA'; }
      function legacyButton(b) { return (b === 0) ? 1 : (b === 2) ? 2 : 4; }    // the values C# (and IE) use
      function esc(s) { return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); }

      function placeholderFor(ta) {
          var c = ta.classList;
          if (c.contains('LangVernacular')) return VernacularLanguageName();
          if (c.contains('LangNationalBt')) return NationalBtLanguageName();
          if (c.contains('LangInternationalBt')) return InternationalBtLanguageName();
          if (c.contains('LangFreeTranslation')) return FreeTranslationLanguageName();
          return '';
      }

      // a box's current (non-empty) selection becomes its highlight
      function record(ta) {
          if (ta.selectionEnd > ta.selectionStart) {
              highlights[ta.id] = { start: ta.selectionStart, end: ta.selectionEnd };
              drawMirror(ta);
          }
      }

      function clear(ta) {
          if (highlights[ta.id]) {
              delete highlights[ta.id];
              drawMirror(ta);
          }
      }

      function drawMirror(ta) {
          var h = highlights[ta.id], m = mirrors[ta.id];
          if (!h) {
              if (m) {
                  m.parentNode.removeChild(m);
                  delete mirrors[ta.id];
                  ta.style.backgroundColor = ta.getAttribute('data-ose-bg') || '';
              }
              return;
          }
          var cs = window.getComputedStyle(ta);
          if (!m) {
              m = document.createElement('div');
              m.className = 'oseMirror';
              m.setAttribute('aria-hidden', 'true');
              m.style.backgroundColor = cs.backgroundColor;          // (taken before the box goes transparent)
              ta.setAttribute('data-ose-bg', ta.style.backgroundColor || '');
              ta.parentNode.insertBefore(m, ta);
              mirrors[ta.id] = m;
              if (window.getComputedStyle(ta.parentNode).position === 'static')
                  ta.parentNode.style.position = 'relative';         // so the mirror and the box share a frame
              ta.style.position = 'relative';
              ta.style.zIndex = '1';
              ta.style.backgroundColor = 'transparent';
          }
          for (var i = 0; i < COPIED.length; i++)
              m.style[COPIED[i]] = cs[COPIED[i]];
          m.style.position = 'absolute';
          m.style.zIndex = '0';
          m.style.margin = '0';
          m.style.borderStyle = 'solid';
          m.style.borderColor = 'transparent';
          m.style.color = 'transparent';
          m.style.whiteSpace = 'pre-wrap';
          m.style.overflowWrap = 'break-word';
          m.style.overflowX = 'hidden';
          m.style.overflowY = (ta.scrollHeight > ta.clientHeight) ? 'scroll' : 'hidden';   // same wrapping width as the box
          m.style.pointerEvents = 'none';
          m.style.left = ta.offsetLeft + 'px';
          m.style.top = ta.offsetTop + 'px';
          m.style.width = ta.offsetWidth + 'px';
          m.style.height = ta.offsetHeight + 'px';
          var v = ta.value;
          m.innerHTML = esc(v.substring(0, h.start)) + '<mark class="' + esc(ta.className) + ' highlight">' +
                        esc(v.substring(h.start, h.end)) + '</mark>' + esc(v.substring(h.end)) + '\n';
          m.scrollTop = ta.scrollTop;
          m.scrollLeft = ta.scrollLeft;
          // while the box has focus, the browser shows its own selection
          m.style.visibility = (document.activeElement === ta) ? 'hidden' : 'visible';
      }

      function redrawAll() {
          for (var id in mirrors) {
              var ta = document.getElementById(id);
              if (ta)
                  drawMirror(ta);
          }
      }

      document.addEventListener('DOMContentLoaded', function () {
          var style = document.createElement('style');
          style.textContent = '.oseMirror mark { color: transparent !important; }';
          document.head.appendChild(style);
          var boxes = document.getElementsByTagName('textarea');
          for (var i = 0; i < boxes.length; i++) {
              var ta = boxes[i];
              if (ta.classList.contains('readonly'))
                  ta.readOnly = true;
              ta.placeholder = placeholderFor(ta);
              ta.addEventListener('scroll', function (e) {
                  var mi = mirrors[e.target.id];
                  if (mi) {
                      mi.scrollTop = e.target.scrollTop;
                      mi.scrollLeft = e.target.scrollLeft;
                  }
              });
          }
      });
      window.addEventListener('resize', redrawAll);

      document.addEventListener('focusin', function (e) {
          if (!isTextarea(e.target))
              return;
          drawMirror(e.target);
          ose.send('focus', { id: e.target.id });
      });

      document.addEventListener('focusout', function (e) {
          if (!isTextarea(e.target))
              return;
          record(e.target);       // a selection made and then left stays as the box's highlight
          drawMirror(e.target);
          ose.send('blur', { id: e.target.id });
      });

      document.addEventListener('mousedown', function (e) {
          var ta = e.target;
          if (!isTextarea(ta))
              return;
          if (e.button === 0)
              clear(ta);          // as before: a left-click in a box drops its highlight (a new drag makes a new one)
          ose.send('textareaMouseDown', { id: ta.id, value: ta.value, button: legacyButton(e.button) });
      });

      document.addEventListener('mouseup', function (e) {
          var ta = e.target;
          if (isTextarea(ta)) {
              record(ta);
              if (e.button === 2)
                  ose.send('contextMenu', { id: ta.id });
              ose.send('textareaMouseUp', { id: ta.id });
              return;
          }
          // anchor buttons, the empty part of the anchor row, and the line-options buttons (data-mouseup)
          var el = ose.closest(e.target, function (x) { return !!x.getAttribute('data-mouseup'); });
          if (!el)
              return;
          var what = el.getAttribute('data-mouseup');
          var bRight = (e.button === 2);
          if (what === 'lineOptions') {
              var last = document.getElementById(ose.lastTextareaId() || '');
              if (isTextarea(last))
                  record(last);   // (what TriggerMyBlur did before a line-options menu)
              ose.send('action', { name: 'lineOptions', id: el.id, arg: bRight ? 'right' : 'left' });
              e.preventDefault();
          }
          else if (what === 'anchor') {
              if (bRight)
                  ose.send('action', { name: 'anchorMenu', id: el.id });
              else
                  ose.send('bibRefJump', { ref: el.getAttribute('name') });
              e.preventDefault();
          }
          else if ((what === 'anchorCell') && bRight) {
              ose.send('action', { name: 'anchorMenu', id: el.id });
          }
      });

      document.addEventListener('keyup', function (e) {
          var ta = e.target;
          if (!isTextarea(ta))
              return;
          if (ta.selectionEnd > ta.selectionStart)
              record(ta);         // e.g. Shift+arrows
          else
              clear(ta);
      });

      document.addEventListener('input', function (e) {
          if (isTextarea(e.target))
              clear(e.target);    // the highlighted text may have changed (PaneCommon sends the new value)
      });

      document.addEventListener('dblclick', function (e) {
          var ta = e.target;
          if (!isTextarea(ta))
              return;
          // as before: no trailing space on a double-clicked word
          while ((ta.selectionEnd > ta.selectionStart) && (ta.value.charAt(ta.selectionEnd - 1) === ' '))
              ta.setSelectionRange(ta.selectionStart, ta.selectionEnd - 1);
          record(ta);
      });

      document.addEventListener('contextmenu', function (e) {
          if (isTextarea(e.target))
              e.preventDefault();
      });

      document.addEventListener('mousemove', function (e) {
          if (!isTextarea(e.target))
              return;
          var now = Date.now();
          if (now - lastMouseMove >= 100) {     // only the Bible pane's auto-hide listens
              lastMouseMove = now;
              ose.send('mouseMove');
          }
      });

      // the highlights in one line's table (after making the box being worked in's selection its highlight, as
      //  TriggerMyBlur did) -- the same items as the IE page, so the note's referring text comes out the same
      ose.on('getHighlights', function (m) {
          var last = document.getElementById(ose.lastTextareaId() || '');
          if (isTextarea(last))
              record(last);
          var items = [];
          var table = document.getElementById(m.tableId);
          if (table) {
              var boxes = table.getElementsByTagName('textarea');
              for (var i = 0; i < boxes.length; i++) {
                  var h = highlights[boxes[i].id];
                  if (h)
                      items.push({ textareaId: boxes[i].id, className: boxes[i].className + ' highlight',
                                   text: boxes[i].value.substring(h.start, h.end) });
              }
          }
          return { items: items };
      });

      ose.on('clearHighlight', function (m) {
          var ta = document.getElementById(m.id);
          if (isTextarea(ta))
              clear(ta);
      });
  })();
  ```

  > Check the textarea class names against `StringTransfer.FormatLanguageColumnHtml` and `js/StoryBtPs.js`'s
  > `hasClass('LangVernacular')` etc. They must match exactly.

- [ ] **Step 5: Run the tests.** Run `/Tests:ModernStoryBtPageTests,StoryBtPageTests`, then the full suite. Expected:
  all pass.

- [ ] **Step 6: Turn WebView2 on.**

  In `HtmlHostFactory.cs`, set `internal static bool UseWebView2WhenAvailable = true;` and update its comment to "on: every
  page has its modern scripts (plan Task 7)".

  Run the full suite again. Some tests construct a real pane or form with its parameterless constructor (so the factory
  picks the host). If one now fails because it got a WebView2 host where it expected IE, give that test an explicit host,
  or set `HtmlHostFactory.UseWebView2WhenAvailable = false` in that fixture's `SetUp` and restore it in `TearDown`.
  List each one in the report. Don't change product code for it.

- [ ] **Step 7: Commit.**

  ```bash
  git add StoryEditor/js/modern/StoryBt.js StoryEditor/HtmlHostFactory.cs StoryEditor.Tests/PaneTestData.cs StoryEditor.Tests/StoryBtPageTests.cs StoryEditor.Tests/ModernStoryBtPageTests.cs
  git commit -m "C7: modern Story/BT page with mirror highlights; WebView2 on by default (IE fallback)"
  ```

---

### Task 8: Installer: WebView2 SDK DLLs, runtime bootstrapper, Newtonsoft check

**Files:**
- Modify: `Installer/Setup OneStoryEditor/OseComponents.wxs`, `Installer/Setup OneStoryEditor/Product.wxs`
- Create: `Installer/ExternalFiles/MicrosoftEdgeWebview2Setup.exe` (downloaded; see Step 2), `Installer/ExternalFiles/README.md`

**Interfaces:**
- Consumes: the output layout from Task 2 Step 1 (where `WebView2Loader.dll` lands).
- Produces: an MSI that installs the 3 SDK DLLs, and runs the Evergreen bootstrapper silently when no runtime is
  installed, ignoring its result.

- [ ] **Step 1: Add the SDK DLLs.**

  In `OseComponents.wxs`, inside `ComponentGroup Id="ProductComponents"`, in the style of the existing
  `HtmlSanitizer`/`Newtonsoft` components (`git show 3964d7f -- "Installer/Setup OneStoryEditor/OseComponents.wxs"` shows
  how A added DLLs). Use new GUIDs, from PowerShell `[guid]::NewGuid()`:

  ```xml
        <Component Id="Microsoft.Web.WebView2.Core.dll" Guid="{NEW-GUID-1}">
          <File Id="Microsoft.Web.WebView2.Core.dll" Name="Microsoft.Web.WebView2.Core.dll" Vital="yes" KeyPath="yes" Source="..\..\output\release\Microsoft.Web.WebView2.Core.dll" />
        </Component>
        <Component Id="Microsoft.Web.WebView2.WinForms.dll" Guid="{NEW-GUID-2}">
          <File Id="Microsoft.Web.WebView2.WinForms.dll" Name="Microsoft.Web.WebView2.WinForms.dll" Vital="yes" KeyPath="yes" Source="..\..\output\release\Microsoft.Web.WebView2.WinForms.dll" />
        </Component>
        <Component Id="WebView2Loader.dll" Guid="{NEW-GUID-3}">
          <File Id="WebView2Loader.dll" Name="WebView2Loader.dll" Vital="yes" KeyPath="yes" Source="..\..\output\release\WebView2Loader.dll" />
        </Component>
  ```

  - Replace `{NEW-GUID-n}` with real GUIDs.
  - If Task 2 found the loader under `runtimes\win-x86\native\` in the output (and didn't set
    `WebView2LoaderPreference=Static`), put that component in a matching `Directory` under `INSTALLFOLDER`.
  - With `Static`, the loader is linked into `Microsoft.Web.WebView2.Core.dll` and there's no separate file; in that
    case leave out the third component.

- [ ] **Step 2: Add the bootstrapper.**

  Download Microsoft's Evergreen bootstrapper (`https://go.microsoft.com/fwlink/p/?LinkId=2124703`) to
  `Installer/ExternalFiles/MicrosoftEdgeWebview2Setup.exe`. Microsoft allows redistributing it with an app.

  Add `Installer/ExternalFiles/README.md` saying where it came from, the date, and that it's the online bootstrapper:
  it needs internet access and installs the shared, auto-updating WebView2 runtime.

  If the download is blocked in this environment, report BLOCKED for this step with the URL. The user can fetch it.

  In `Product.wxs`, inside `<Package>`:

  ```xml
      <!-- the shared (Evergreen) WebView2 runtime: per-machine (32-bit registry view) or per-user. 'pv' of 0.0.0.0 means
           not installed -->
      <Property Id="WEBVIEW2_MACHINE">
        <RegistrySearch Id="WebView2Machine" Root="HKLM" Key="SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" Name="pv" Type="raw" />
      </Property>
      <Property Id="WEBVIEW2_USER">
        <RegistrySearch Id="WebView2User" Root="HKCU" Key="Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" Name="pv" Type="raw" />
      </Property>

      <Binary Id="WebView2Bootstrapper" SourceFile="..\ExternalFiles\MicrosoftEdgeWebview2Setup.exe" />
      <!-- installs the runtime when it's missing; failure (e.g. offline) is ignored: the app then uses IE panes -->
      <CustomAction Id="InstallWebView2Runtime" BinaryRef="WebView2Bootstrapper" ExeCommand="/silent /install"
                    Execute="deferred" Impersonate="no" Return="ignore" />
  ```

  Extend the existing `<InstallExecuteSequence>`:

  ```xml
      <InstallExecuteSequence>
        <WriteEnvironmentStrings />
        <Custom Action="InstallWebView2Runtime" Before="InstallFinalize"
                Condition="NOT REMOVE AND (NOT WEBVIEW2_MACHINE OR WEBVIEW2_MACHINE = &quot;0.0.0.0&quot;) AND (NOT WEBVIEW2_USER OR WEBVIEW2_USER = &quot;0.0.0.0&quot;)" />
      </InstallExecuteSequence>
  ```

  If WiX 6 rejects any attribute here, use the WiX 6 equivalent it names in the error, and record it in the report.

- [ ] **Step 3: Check that Newtonsoft is installed.**

  Sub-project B made every pane depend on `Newtonsoft.Json.dll` (`HtmlMessage`), but its component in
  `OseComponents.wxs` (`:303-306`) is commented out. It may come from a merge module instead (Chorus or Encoding
  Converters).

  1. Build the installer:
     `"$MSBUILD" "Installer/Setup OneStoryEditor/Setup OneStoryEditor.wixproj" -p:Configuration=Release -v:m -nologo`.
     That needs a Release build of the solution in `output\release` first:
     `"$MSBUILD" "StoryEditor 2017.sln" -p:Configuration=Release -p:Platform=x86 -v:m -nologo`.
  2. List the MSI's files with PowerShell:

     ```powershell
     $i = New-Object -ComObject WindowsInstaller.Installer
     $db = $i.GetType().InvokeMember('OpenDatabase','InvokeMethod',$null,$i,@('<path to the .msi>',0))
     $v = $db.GetType().InvokeMember('OpenView','InvokeMethod',$null,$db,@("SELECT FileName FROM File"))
     $v.GetType().InvokeMember('Execute','InvokeMethod',$null,$v,$null)
     while ($r = $v.GetType().InvokeMember('Fetch','InvokeMethod',$null,$v,$null)) { $r.GetType().InvokeMember('StringData','GetProperty',$null,$r,1) }
     ```

     Merge-module files show up in the merged MSI's File table too.
  3. If no `Newtonsoft.Json.dll` (file names can look like `xxxxx|Newtonsoft.Json.dll`) is in the list, uncomment its
     component.
  4. Check the WebView2 DLLs from Step 1 are listed.
  5. If the installer can't be built in this environment (for example the Release solution build fails for reasons
     unrelated to this task), say so. Report the Newtonsoft question as open, with what you found in the `.wxs` and
     merge modules.

- [ ] **Step 4: Commit.**

  ```bash
  git add "Installer/Setup OneStoryEditor/OseComponents.wxs" "Installer/Setup OneStoryEditor/Product.wxs" Installer/ExternalFiles
  git commit -m "C8: installer ships the WebView2 SDK and runs the runtime bootstrapper when needed; Newtonsoft check"
  ```

---

### Task 9: Guard tests, cleanup, manual checklist

**Files:**
- Modify: `StoryEditor.Tests/ArchitectureGuardTests.cs`,
  `docs/superpowers/specs/2026-10-04-webview2-host-design.md` (a "Refinements made while planning and implementing"
  section)
- Report: the manual checklist goes in the task report, not the repo.

**Interfaces:**
- Consumes: everything above.
- Produces: the guard rules that keep C's boundaries.

- [ ] **Step 1: Extend the guard tests.**

  Add to `ArchitectureGuardTests` (it already has the source-dir and file helpers from B):

  ```csharp
          private static readonly Regex RegexWebView2Api = new Regex(@"\bMicrosoft\.Web\.WebView2\b|\bCoreWebView2\w*", RegexOptions.Compiled);
          private static readonly Regex RegexIeOnlyJs = new Regex(@"document\.selection|createTextRange|attachEvent|fireEvent|window\.event\b", RegexOptions.Compiled);

          [Test]
          public void OnlyWebView2HtmlHostTouchesWebView2()
          {
              var offenders = SourceFiles(".cs")
                  .Where(f => !Path.GetFileName(f).Equals("WebView2HtmlHost.cs"))
                  .Where(f => RegexWebView2Api.IsMatch(File.ReadAllText(f)))
                  .Select(Path.GetFileName)
                  .ToList();
              Assert.That(offenders, Is.Empty);
          }

          [Test]
          public void OnlyTheModernBridgeUsesChromeWebview()
          {
              var offenders = SourceFiles(".js", ".cs", ".htm", ".html", ".resx")
                  .Where(f => !(Path.GetFileName(f).Equals("bridge.js") && f.Contains(@"\modern\")))
                  .Where(f => File.ReadAllText(f).Contains("chrome.webview"))
                  .Select(Path.GetFileName)
                  .ToList();
              Assert.That(offenders, Is.Empty);
          }

          [Test]
          public void ModernScriptsUseNoIeOnlyApis()
          {
              var offenders = SourceFiles(".js")
                  .Where(f => f.Contains(@"\js\modern\"))
                  .Where(f => RegexIeOnlyJs.IsMatch(File.ReadAllText(f)))
                  .Select(f => Path.GetFileName(f) + ": " + RegexIeOnlyJs.Match(File.ReadAllText(f)).Value)
                  .ToList();
              Assert.That(offenders, Is.Empty);
          }
  ```

  Also adjust the existing `OnlyTheBridgeScriptUsesWindowExternal` so it excludes only `js\bridge.js`, not
  `js\modern\bridge.js`. The modern bridge must not contain `window.external`; check the condition is a full-path match.

  Run `/Tests:ArchitectureGuardTests`. Fix any offender the way earlier tasks did: move real code behind the boundary,
  or reword comments. Don't add exclusions.

- [ ] **Step 2: Record the refinements in the spec.** Add a section listing what changed from the spec text while
  planning and implementing:
  - Keys use page-level `preventDefault` plus the navigation guard, not `AcceleratorKeyPressed` (the WinForms control
    doesn't expose the controller event; Task 1 Q6).
  - The Bible-reference drag uses the mechanism Task 1 Q3 chose (HTML5 with `refDragStart`/`refDragEnd`, or the
    WinForms text `DataObject`).
  - The modern page tests are separate `Modern*` fixtures, not engine parameters on the IE fixtures. The IE fixtures
    keep their quirks-mode drivers.
  - `HtmlHostFactory.UseWebView2WhenAvailable` gated the switch-over until the modern pages were complete.
  - Whatever Task 8 found about Newtonsoft in the installer.

- [ ] **Step 3: Full clean build and test.**

  Delete `StoryEditor/obj` and `StoryEditor.Tests/obj`: dry-run `git clean -ndX StoryEditor/obj StoryEditor.Tests/obj`
  first, never the whole repo. Then build and run everything. Expected: all pass (WebView2 cases run on this machine).

- [ ] **Step 4: Write the manual checklist into the task report.** It's for the user, run on both engines:
  - **Every pane and popup, side by side WebView2 vs IE:** layout, fonts, table spacing. Standards mode can differ;
    note any difference as a CSS fix for the modern pages.
  - **B's smoke checklist on WebView2:**
    - typing and Ctrl+S; close right after typing; autosave
    - line breaks survive save and reload
    - highlight in two columns, then Add note; the highlights clear
    - Copy / Cut / Paste selected
    - concordance; L&C note lookup; move selected text to a new line
    - every note button
    - Bible references: links and anchors (left click jumps; right click opens the menu)
    - **drag a verse from the Bible pane onto an anchor cell and onto a note box**
    - F5 and Ctrl+F5; Ctrl+F (find); Keyman switching; Tab
    - TextPaster
    - find/replace in the note panes
    - revision history; swap columns; print preview and save HTML
    - the commentary popup; the tooltips
  - **Fallback:**
    - Point the WebView2 data folder at a read-only path, or rename the runtime folder. Start: the status bar says IE
      panes are in use, and the panes work.
    - With WebView2 running, kill `msedgewebview2.exe` in Task Manager. The panes reload in IE, with a status note.
    - Restart: it starts in IE (the failure is remembered). Toggle *Use the IE browser panes* off and restart: it tries
      WebView2 again.
  - **Installer:**
    - On a machine (or VM) without the runtime, online: the runtime gets installed, and the app uses WebView2.
    - Offline: the install completes, and the app uses IE.

- [ ] **Step 5: Commit.**

  ```bash
  git add StoryEditor.Tests/ArchitectureGuardTests.cs docs/superpowers/specs/2026-10-04-webview2-host-design.md
  git commit -m "C9: guard tests for the WebView2 boundaries; spec refinements"
  ```

---

## Self-review notes (for the executor)

- **Spec coverage:**

  | Spec | Task |
  |---|---|
  | Section 1 host | 2 |
  | Engine choice, fallback, setting | 3 |
  | Installer | 8 |
  | Section 2 page selection, bridge, PaneCommon | 4 |
  | NetBible/HoverLinks and the drag | 5 |
  | ConNote | 6 |
  | StoryBt and the mirror | 7 |
  | The C# `value` change | 4 |
  | Section 3 spike | 1 |
  | Tests | each task |
  | Guard | 9 |
  | Manual checks | 9 |
  | Rollout (default on) | 7, Step 6 |

- **Deviations from the spec text, decided while planning:**
  - Keys use page-level `preventDefault` plus the navigation guard, in place of `AcceleratorKeyPressed` (Task 1 Q6
    confirms it).
  - The modern tests are separate fixtures.
  - The `UseWebView2WhenAvailable` gate.

  Task 9 Step 2 records all three in the spec.
