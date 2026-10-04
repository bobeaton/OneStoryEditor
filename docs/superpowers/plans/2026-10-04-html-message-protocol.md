# HTML Pane Message Protocol (Sub-project B) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Every HTML host in StoryEditor talks to its page only through `IHtmlHost`, a JSON message channel. There is no `window.external.X`, no C# DOM access and no `mshtml`. IE stays the only browser, and the behaviour users see doesn't change.

**Architecture:**
- `IeHtmlHost` wraps the IE `WebBrowser`. JS → C# goes through one COM method, `postMessage(json)`, and is delivered asynchronously with `BeginInvoke`. C# → JS goes through one function, `InvokeScript("oseReceive", json)`.
- `js/bridge.js` (transport) and `js/PaneCommon.js` (shared pane handlers) are inlined into the pages.
- The panes become `UserControl`s that hold a host.
- JS pushes events. C# asks for page state with a bounded `Request` that pumps messages while it waits, and sends commands with `Post`.
- Every save path flushes the panes first.

**Tech Stack:** C# / .NET Framework 4.8 WinForms (SDK-style, x86), the IE `WebBrowser` control in IE9 document mode (`FEATURE_BROWSER_EMULATION` 9999), Newtonsoft.Json 13 (already resolved), jQuery (already in the Story/BT page), NUnit 3 (`StoryEditor.Tests`, 199 tests at start).

**Spec:** `docs/superpowers/specs/2026-10-04-html-message-protocol-design.md`. Read it first, including **Refinements made while writing the plan** at the end.

## Global Constraints

- Branch `DecoupleWebBrowser`. Commit after each task. End each commit message with the `Co-Authored-By:` trailer of the model that wrote it.
- **No data or file-format change** and no version marker. The released exe must keep opening what we save.
- **No behaviour change users can see**, apart from the fixes the spec names:
  - the flush happens before the `Modified` check
  - `readOnly` is spelled correctly in StoryBtPs.js
  - the flush-failure prompt
  - text that didn't change no longer sets `Modified` (Task 4; needed so that a flush can't cause a spurious "save changes?" prompt)
- **Keep the existing element id formats** (`ta_…`, `tp_v_c_i`, `btn_v_c_i`, `lineTable_N`, `ln_N`, `anc_N`, `btnAnc_…`, `btnLn_N`). C# id parsing doesn't change.
- **Keep IE-specific values exactly as JS reports them today.** For example, `textareaMouseDown.button` is jQuery's `event.button` in the Story/BT pane and `window.event.button` in the note panes; C# keeps `nButton == 1` meaning left.
- After Task 7, **only `IeHtmlHost.cs` may contain** `HtmlElement`, `HtmlDocument`, `InvokeScript`, `InvokeMember`, `DomDocument`, `DocumentText`, `ObjectForScripting` or `mshtml`, and **only `js/bridge.js` may contain** `window.external`. The architecture guard test (Task 7) enforces this.
- Edit with the Write/Edit tools, never shell heredocs/sed/python. In this environment escapes have been corrupted and sandboxed range-deletes refused. After writing C# or JS, check that your added lines contain no unexpected non-ASCII (`grep -nP '[^\x00-\x7F]' <file>`), apart from text that was already there. Use Bash only for git, MSBuild, vstest and grep.
- Large files (`StoryEditor.cs`, `HtmlStoryBtControl.cs`, `HtmlConNoteControl.cs`, `VerseData.cs`, `ConsultNoteDataConverter.cs`, `StoryData.cs`, `Properties/Resources.resx`, `Properties/Resources.Designer.cs`): use Grep `-n`, and read only the regions you need.
- **Resources:**
  - `html/StoryBt.htm` and `js/StoryBt.js`, `StoryBtPs.js`, `ConNoteDomPrefix.js` are `ResXFileRef` resources. Editing the file is enough.
  - Inline resx strings (`HTML_Header`, `HTML_ButtonClass`, …) are edited in `Properties/Resources.resx`, where the value is XML-escaped (`&lt;` …).
  - When you remove a resx entry, also remove its property from `Properties/Resources.Designer.cs`.
  - New JS files are `EmbeddedResource`s read through `PageScripts`.
- **Build and test** (Git Bash). If `output\Debug` is locked because the user is debugging, add `"-p:OutDir=<scratch folder>/"` and run vstest on `<scratch folder>/StoryEditor.Tests.dll`:
  ```bash
  MSBUILD="/c/Program Files/Microsoft Visual Studio/18/Insiders/MSBuild/Current/Bin/MSBuild.exe"
  VSTEST="/c/Program Files/Microsoft Visual Studio/18/Insiders/Common7/IDE/Extensions/TestPlatform/vstest.console.exe"
  "$MSBUILD" "StoryEditor 2017.sln" -t:StoryEditor_Tests -restore -p:Configuration=Debug -p:Platform=x86 -v:m -nologo
  "$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86
  ```
  Run one fixture with `/Tests:IeHtmlHostTests`.
- **Tests must never block on UI.** Browser tests use an off-screen `Form` (`StartPosition = Manual`, `Location = (-32000, -32000)`, `ShowInTaskbar = false`) and pump with a timeout. No message boxes.
- **The app only works end to end again after Task 6.** Task 4 moves all pane C# over (it must, to compile), and Tasks 5 and 6 move the two kinds of pane page. Between them the solution builds and the tests pass, but the pane pages aren't fully wired. That's why each of those tasks ends with a browser test that loads the real page and drives it through the protocol.

## Review Focus

These are the inputs and conditions most likely to bite a user that no single unit test covers. Each line names the test that pins it:

1. **A flush that sends unchanged text sets `Modified`**, so closing an untouched project asks "save changes?" every time. Expected: unchanged text never sets `Modified`. Pinned by `PaneTextTests.IsSame` (Task 3). Task 4 makes both panes' `textChanged` handlers return before setting `Modified` when `PaneText.IsSame` is true; the reviewer checks that ordering in `SetFieldValue` and `TextareaOnKeyUp`.
2. **A message from the previous document arrives after `LoadHtml`.** For example, the old page's `ready` lands after the new `LoadHtml`, so posts go to a half-loaded page. Expected: a `ready` whose `doc` id isn't the current one is ignored. Pinned by `IeHtmlHostTests.ReadyFromAnOlderDocument_IsIgnored` (Task 1).
3. **A JS handler throws while C# waits on a `Request`.** Expected: an immediate reply with `error`, not a 2-second hang. Pinned by `IeHtmlHostTests.Request_HandlerThrows_RepliesWithErrorPromptly` (Task 1).
4. **Clicking a link in a pane navigates the page away** and leaves the pane blank. Expected: every `<a>` click in a pane page is cancelled; the known kinds are sent as messages. Pinned by `PaneCommonPageTests.LinkClick_SendsMessage_AndDoesNotNavigate` (Task 4) and `SmallHostPageTests.HoverLinks_LinkClick_SendsInnerHtml_AndDoesNotNavigate` (Task 2).
5. **Text with quotes, newlines, `<`, `&` and non-ASCII** must round-trip unchanged through JSON both ways. Pinned by `IeHtmlHostTests.Request_RoundTripsAwkwardText` (Task 1) and `StoryBtPageTests.SetText_ThenFlush_RoundTripsAwkwardText` (Task 6).

## File map

| File | Responsibility | Task |
|---|---|---|
| `StoryEditor/HtmlMessage.cs` (new) | One message: `Type` plus a JSON body, typed getters, parse and serialize | 1 |
| `StoryEditor/HtmlMessageDispatcher.cs` (new) | `type → handler` table: catches and logs exceptions, ignores unknown types | 1 |
| `StoryEditor/IHtmlHost.cs` (new) | The host interface and `HtmlHostOptions` | 1 |
| `StoryEditor/IeHtmlHost.cs` (new) | The IE implementation; the only file that touches `WebBrowser` | 1 |
| `StoryEditor/HtmlHostFactory.cs` (new) | `Create(options)`. C will add the setting here | 1 |
| `StoryEditor/PageScripts.cs` (new) | Reads the embedded JS files | 1 |
| `StoryEditor/js/bridge.js` (new) | Transport detection, `ose.send/on`, `oseReceive`, `ready`, `onerror`, doc id | 1 |
| `StoryEditor/js/HoverLinks.js`, `js/NetBible.js` (new) | Page scripts for `HtmlForm` and `NetBibleViewer` | 2 |
| `StoryEditor/LineLabelParser.cs`, `PaneText.cs`, `HighlightedText.cs`, `ReferringTextBuilder.cs` (new) | Pure helpers: the "Ln: N" label parser, the same-text check, selection records, and the referring-text builder for notes | 3 |
| `StoryEditor/js/PaneCommon.js` (new) | Handlers shared by both pane pages | 4 |
| `StoryEditor/HtmlVerseControl.cs` | Becomes a `UserControl` holding a host; shared handlers; selection commands | 4 |
| `StoryEditor/HtmlStoryBtControl.cs`, `HtmlConNoteControl.cs` | Pane C# on the dispatcher and host (no DOM) | 4 |
| `StoryEditor/TextPaster.cs` | Uses `TextareaRef` instead of `HtmlElement` | 4 |
| `StoryEditor/js/ConNoteDomPrefix.js`, note button builders, `HTML_Header` | Note-pane pages on the protocol | 5 |
| `StoryEditor/js/StoryBt.js`, `js/StoryBtPs.js`, anchor and line-option templates | Story/BT page on the protocol | 6 |
| `StoryEditor/PaneFlush.cs` (new), `StoryEditor.cs` | Flush policy (pure) and the four save entry points | 7 |
| `StoryEditor.Tests/ArchitectureGuardTests.cs` (new), `StoryEditor.csproj`, `OseComponents.wxs` | Guard test; `mshtml` and `OseResources` removed | 8 |
| `StoryEditor.Tests/FakeHtmlHost.cs`, `BrowserTestHelper.cs`, `PaneTestData.cs` (new) | Test doubles and helpers | 1, 5 |

---

### Task 1: Host core (`HtmlMessage`, dispatcher, `IHtmlHost`, `IeHtmlHost`, `bridge.js`)

**Files:**
- Create: `StoryEditor/HtmlMessage.cs`, `StoryEditor/HtmlMessageDispatcher.cs`, `StoryEditor/IHtmlHost.cs`, `StoryEditor/IeHtmlHost.cs`, `StoryEditor/HtmlHostFactory.cs`, `StoryEditor/PageScripts.cs`, `StoryEditor/js/bridge.js`
- Modify: `StoryEditor/StoryEditor.csproj` (embedded resource), `StoryEditor/Program.cs:303-310` (`AddIe9RegistryKey` → `IeHtmlHost.EnsureIe9Mode`)
- Test: `StoryEditor.Tests/HtmlMessageTests.cs`, `StoryEditor.Tests/HtmlMessageDispatcherTests.cs`, `StoryEditor.Tests/FakeHtmlHost.cs`, `StoryEditor.Tests/BrowserTestHelper.cs`, `StoryEditor.Tests/IeHtmlHostTests.cs`

**Interfaces:**
- Produces, used by every later task:
  - `HtmlMessage`:
    - `Create(string type, object payload = null)`, `TryParse(string json)`
    - `Type`, `Body` (`JObject`)
    - `GetString(name)`, `TryGetInt(name, out int)`, `GetBool(name, bool dflt = false)`
    - `Rid` (`int?`), `ReplyTo` (`int?`), `DocId` (`string`)
    - `ToJson()`, `WithRid(int)`
  - `HtmlMessageDispatcher`: `Register(string type, Action<HtmlMessage>)`, `bool Dispatch(HtmlMessage)`, `IReadOnlyCollection<string> RegisteredTypes`, `Action<string> ReportError` (set by panes to show errors on the status bar)
  - `IHtmlHost`: `Control Control`, `bool IsReady`, `string LoadedHtml`, `void LoadHtml(string)`, `void Post(string type, object payload = null)`, `HtmlMessage Request(string type, object payload, TimeSpan timeout)`, `event EventHandler<HtmlMessage> MessageReceived`, `event EventHandler DocumentReady`, `void ShowPrintPreview()`
  - `HtmlHostOptions { bool AllowBrowserContextMenu; bool AllowFileDrop; }`
  - `HtmlHostFactory.Create(HtmlHostOptions options = null)`
  - `HtmlHostDefaults.RequestTimeout` (2 s; panes and tests use it, never an IE-specific constant), `IeHtmlHost.CstrDocIdToken = "__OSE_DOC_ID__"`, `IeHtmlHost.EnsureIe9Mode(string exeName)`
  - `PageScripts.Bridge`, `PageScripts.Get(string fileName)`, `PageScripts.ScriptBlock(params string[] scripts)`
  - Tests: `FakeHtmlHost`, `BrowserTestHelper.PumpUntil(Func<bool>, int ms)`, `BrowserTestHelper.CreateHostInOffscreenForm(out Form)`

- [ ] **Step 1: Write the failing message and dispatcher tests**

`StoryEditor.Tests/HtmlMessageTests.cs`:

```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlMessageTests
    {
        [Test]
        public void Create_PutsTypeAndPayloadInBody()
        {
            var msg = HtmlMessage.Create("scrollTo", new { id = "ln_3", alignTop = true });
            Assert.That(msg.Type, Is.EqualTo("scrollTo"));
            Assert.That(msg.GetString("id"), Is.EqualTo("ln_3"));
            Assert.That(msg.GetBool("alignTop"), Is.True);
        }

        [Test]
        public void ToJson_ThenTryParse_RoundTripsAwkwardText()
        {
            const string strText = "a \"quote\" <b>&amp;</b>\r\nline2 üकि";
            var msg = HtmlMessage.TryParse(HtmlMessage.Create("x", new { text = strText }).ToJson());
            Assert.That(msg.GetString("text"), Is.EqualTo(strText));
        }

        [TestCase("not json")]
        [TestCase("[1,2]")]
        [TestCase("{\"notype\":1}")]
        [TestCase("{\"type\":5}")]
        [TestCase("")]
        [TestCase(null)]
        public void TryParse_Garbage_ReturnsNull(string json)
        {
            Assert.That(HtmlMessage.TryParse(json), Is.Null);
        }

        [Test]
        public void TryGetInt_AcceptsNumbersAndNumericStrings_RejectsOthers()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":3,\"b\":\"12\",\"c\":\"x\",\"d\":true}");
            Assert.That(msg.TryGetInt("a", out var a) && a == 3, Is.True);
            Assert.That(msg.TryGetInt("b", out var b) && b == 12, Is.True);
            Assert.That(msg.TryGetInt("c", out _), Is.False);
            Assert.That(msg.TryGetInt("d", out _), Is.False);
            Assert.That(msg.TryGetInt("missing", out _), Is.False);
        }

        [Test]
        public void GetBool_AcceptsBooleansAndTrueFalseStrings()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":true,\"b\":\"true\",\"c\":\"False\",\"d\":1}");
            Assert.That(msg.GetBool("a"), Is.True);
            Assert.That(msg.GetBool("b"), Is.True);
            Assert.That(msg.GetBool("c", true), Is.False);
            Assert.That(msg.GetBool("d"), Is.False, "numbers aren't booleans");
            Assert.That(msg.GetBool("missing", true), Is.True);
        }

        [Test]
        public void GetString_NonString_ReturnsNull()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":3}");
            Assert.That(msg.GetString("a"), Is.Null);
        }

        [Test]
        public void RidReplyToAndDocId_AreRead()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"reply\",\"re\":7,\"doc\":\"3\"}");
            Assert.That(msg.ReplyTo, Is.EqualTo(7));
            Assert.That(msg.DocId, Is.EqualTo("3"));
            Assert.That(HtmlMessage.Create("x").WithRid(9).Rid, Is.EqualTo(9));
        }
    }
}
```

`StoryEditor.Tests/HtmlMessageDispatcherTests.cs`:

```csharp
using System;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlMessageDispatcherTests
    {
        [Test]
        public void Dispatch_KnownType_CallsHandler()
        {
            var d = new HtmlMessageDispatcher();
            string strSeen = null;
            d.Register("focus", m => strSeen = m.GetString("id"));
            Assert.That(d.Dispatch(HtmlMessage.Create("focus", new { id = "ta_1" })), Is.True);
            Assert.That(strSeen, Is.EqualTo("ta_1"));
        }

        [Test]
        public void Dispatch_UnknownType_ReturnsFalse_DoesNotThrow()
        {
            var d = new HtmlMessageDispatcher();
            Assert.That(d.Dispatch(HtmlMessage.Create("nope")), Is.False);
        }

        [Test]
        public void Dispatch_HandlerThrows_IsCaughtAndReported()
        {
            var d = new HtmlMessageDispatcher();
            string strReported = null;
            d.ReportError = s => strReported = s;
            d.Register("boom", m => throw new InvalidOperationException("bad id"));
            Assert.DoesNotThrow(() => d.Dispatch(HtmlMessage.Create("boom")));
            Assert.That(strReported, Does.Contain("bad id"));
        }

        [Test]
        public void Register_SameTypeTwice_LastWins()
        {
            var d = new HtmlMessageDispatcher();
            var n = 0;
            d.Register("t", m => n = 1);
            d.Register("t", m => n = 2);
            d.Dispatch(HtmlMessage.Create("t"));
            Assert.That(n, Is.EqualTo(2));
            Assert.That(d.RegisteredTypes, Is.EquivalentTo(new[] { "t" }));
        }
    }
}
```

- [ ] **Step 2: Run them to verify they fail**

Run the build. Expected: it fails to compile because `HtmlMessage` and `HtmlMessageDispatcher` don't exist.

- [ ] **Step 3: Implement `HtmlMessage` and `HtmlMessageDispatcher`**

`StoryEditor/HtmlMessage.cs`:

```csharp
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// one message between an HTML page and its host (see js/bridge.js): a flat JSON object with a string 'type'
    /// </summary>
    public class HtmlMessage
    {
        public const string CstrTypeReply = "reply";
        public const string CstrTypeReady = "ready";
        public const string CstrTypeJsError = "jsError";
        public const string CstrTypeLog = "log";

        public string Type { get; }
        public JObject Body { get; }

        private HtmlMessage(string type, JObject body)
        {
            Type = type;
            Body = body;
        }

        public static HtmlMessage Create(string type, object payload = null)
        {
            var body = (payload == null) ? new JObject() : JObject.FromObject(payload);
            body["type"] = type;
            return new HtmlMessage(type, body);
        }

        // null if the string isn't a JSON object with a string 'type'
        public static HtmlMessage TryParse(string json)
        {
            if (String.IsNullOrEmpty(json))
                return null;
            try
            {
                var body = JObject.Parse(json);
                var tokType = body["type"];
                if ((tokType == null) || (tokType.Type != JTokenType.String))
                    return null;
                return new HtmlMessage((string)tokType, body);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public HtmlMessage WithRid(int nRid)
        {
            Body["rid"] = nRid;
            return this;
        }

        public int? Rid => TryGetInt("rid", out var n) ? n : (int?)null;
        public int? ReplyTo => TryGetInt("re", out var n) ? n : (int?)null;
        public string DocId => GetString("doc");

        public string GetString(string strName)
        {
            var tok = Body[strName];
            return ((tok != null) && (tok.Type == JTokenType.String)) ? (string)tok : null;
        }

        // element ids and 'name' attributes arrive as strings, so numeric strings count too
        public bool TryGetInt(string strName, out int nValue)
        {
            nValue = 0;
            var tok = Body[strName];
            if (tok == null)
                return false;
            if (tok.Type == JTokenType.Integer)
            {
                nValue = (int)tok;
                return true;
            }
            return (tok.Type == JTokenType.String) && Int32.TryParse((string)tok, out nValue);
        }

        public bool GetBool(string strName, bool bDefault = false)
        {
            var tok = Body[strName];
            if (tok == null)
                return bDefault;
            if (tok.Type == JTokenType.Boolean)
                return (bool)tok;
            if ((tok.Type == JTokenType.String) && Boolean.TryParse((string)tok, out var b))
                return b;
            return bDefault;
        }

        public string ToJson()
        {
            return Body.ToString(Formatting.None);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
```

`StoryEditor/HtmlMessageDispatcher.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// routes messages from a page to handlers by 'type'. One bad message must never take down the message loop,
    /// so unknown types are logged and handler exceptions are caught and reported
    /// </summary>
    public class HtmlMessageDispatcher
    {
        private readonly Dictionary<string, Action<HtmlMessage>> _handlers = new Dictionary<string, Action<HtmlMessage>>();

        // set by the owner (e.g. to put the error on the status bar); defaults to the debug log
        public Action<string> ReportError { get; set; }

        public IReadOnlyCollection<string> RegisteredTypes => _handlers.Keys;

        public void Register(string strType, Action<HtmlMessage> handler)
        {
            _handlers[strType] = handler;
        }

        public bool Dispatch(HtmlMessage msg)
        {
            if ((msg == null) || !_handlers.TryGetValue(msg.Type, out var handler))
            {
                Debug.WriteLine("HtmlMessageDispatcher: no handler for " + msg);
                return false;
            }

            try
            {
                handler(msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HtmlMessageDispatcher: '{msg.Type}' threw {ex}");
                (ReportError ?? (s => Debug.WriteLine(s)))(ex.Message);
            }
            return true;
        }
    }
}
```

- [ ] **Step 4: Run the tests to verify they pass**

Run the build and `/Tests:HtmlMessageTests,HtmlMessageDispatcherTests`. Expected: all pass.

- [ ] **Step 5: Write `IHtmlHost`, the factory, `PageScripts` and `bridge.js`**

`StoryEditor/IHtmlHost.cs`:

```csharp
using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// how C# talks to an HTML page: load it, post commands to it, ask it things (bounded), and hear from it.
    /// Nothing else may touch the browser (see spec: docs/superpowers/specs/2026-10-04-html-message-protocol-design.md)
    /// </summary>
    public interface IHtmlHost : IDisposable
    {
        Control Control { get; }                         // add this to the owning pane/form
        bool IsReady { get; }                            // the current document has sent 'ready'
        string LoadedHtml { get; }                       // the last string passed to LoadHtml
        void LoadHtml(string strHtml);                   // "" = blank page
        void Post(string strType, object payload = null); // C# -> JS, doesn't wait (queued until 'ready')
        HtmlMessage Request(string strType, object payload, TimeSpan timeout); // null if not ready or timed out
        event EventHandler<HtmlMessage> MessageReceived; // JS -> C#, raised on the UI thread
        event EventHandler DocumentReady;
        void ShowPrintPreview();
    }

    public class HtmlHostOptions
    {
        public bool AllowBrowserContextMenu { get; set; }   // IE's own right-click menu
        public bool AllowFileDrop { get; set; }             // dropping a file onto the browser navigates to it
    }

    public static class HtmlHostDefaults
    {
        // how long C# waits for a page to answer a Request
        public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(2);
    }
}
```

`StoryEditor/HtmlHostFactory.cs`:

```csharp
namespace OneStoryProjectEditor
{
    public static class HtmlHostFactory
    {
        // sub-project C adds the setting that chooses the WebView2 host here
        public static IHtmlHost Create(HtmlHostOptions options = null)
        {
            return new IeHtmlHost(options ?? new HtmlHostOptions());
        }
    }
}
```

`StoryEditor/PageScripts.cs`:

```csharp
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// the JS files embedded with LogicalName "OneStoryProjectEditor.js.&lt;file&gt;" (see StoryEditor.csproj)
    /// </summary>
    internal static class PageScripts
    {
        private const string CstrResourcePrefix = "OneStoryProjectEditor.js.";
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        public static string Bridge => Get("bridge.js");

        public static string Get(string strFileName)
        {
            return Cache.GetOrAdd(strFileName, name =>
            {
                using (var stream = typeof(PageScripts).Assembly.GetManifestResourceStream(CstrResourcePrefix + name))
                {
                    if (stream == null)
                        throw new InvalidOperationException("missing embedded script " + name);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            });
        }

        public static string ScriptBlock(params string[] astrScripts)
        {
            return "<script type=\"text/javascript\">" + Environment.NewLine +
                   String.Join(Environment.NewLine, astrScripts) + Environment.NewLine +
                   "</script>";
        }
    }
}
```

`StoryEditor/js/bridge.js`:

```js
// bridge.js: the only page script that knows which browser it is hosted in. Inlined first into every page.
//  ose.send(type, payload)  page -> C#
//  ose.on(type, handler)    C# -> page; when the message has a 'rid', the handler's return value is the reply
(function () {
    var docId = '__OSE_DOC_ID__';   // replaced by the host on load, so it can ignore an older document's messages
    var handlers = {};

    function post(str) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(str);
        else if (window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.ose)
            window.webkit.messageHandlers.ose.postMessage(str);
        else
            window.external.postMessage(str);
    }

    if (!window.JSON) {
        // IE below document mode 8 has no JSON, so say so the only way we can
        try {
            post('{"type":"jsError","message":"JSON is unavailable (documentMode ' + document.documentMode + ')"}');
        } catch (e) { }
    }

    function send(type, payload) {
        var msg = payload || {};
        msg.type = type;
        msg.doc = docId;
        try {
            post(JSON.stringify(msg));
        } catch (e) { }
    }

    function on(type, handler) {
        handlers[type] = handler;
    }

    window.oseReceive = function (json) {
        var msg, result = null, error = null;
        try {
            msg = JSON.parse(json);
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
            var reply = result || {};
            reply.re = msg.rid;
            if (error)
                reply.error = error;
            send('reply', reply);
        }
    };

    window.onerror = function (message, source, line) {
        send('jsError', { message: String(message), source: String(source || ''), line: line || 0 });
        return false;   // keep IE's own handling, as before
    };

    function ready() {
        send('ready', { docMode: document.documentMode || 0 });
    }
    if (window.addEventListener)
        window.addEventListener('load', ready, false);
    else
        window.attachEvent('onload', ready);

    window.ose = { send: send, on: on };
})();
```

In `StoryEditor/StoryEditor.csproj`, next to the existing `StoryProject.xsd` line (`:191`), add:

```xml
    <EmbeddedResource Include="js\bridge.js" LogicalName="OneStoryProjectEditor.js.bridge.js" />
```

- [ ] **Step 6: Write `IeHtmlHost`**

`StoryEditor/IeHtmlHost.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// IHtmlHost over the IE WebBrowser control. The only class allowed to touch WebBrowser/HtmlDocument.
    /// Messages from the page are delivered with BeginInvoke (never inside the window.external call) so that
    /// timing matches WebView2's asynchronous WebMessageReceived
    /// </summary>
    public sealed class IeHtmlHost : IHtmlHost
    {
        public const string CstrDocIdToken = "__OSE_DOC_ID__";

        private readonly WebBrowser _browser;
        private readonly List<HtmlMessage> _pendingPosts = new List<HtmlMessage>();
        private readonly HashSet<int> _awaitedRids = new HashSet<int>();
        private readonly Dictionary<int, HtmlMessage> _replies = new Dictionary<int, HtmlMessage>();
        private int _nDocId;
        private int _nLastRid;

        public IeHtmlHost(HtmlHostOptions options)
        {
            _browser = new WebBrowser
            {
                IsWebBrowserContextMenuEnabled = options.AllowBrowserContextMenu,
                AllowWebBrowserDrop = options.AllowFileDrop,
                Dock = DockStyle.Fill,
                ObjectForScripting = new ScriptingBridge(this)
            };
        }

        public Control Control => _browser;
        public bool IsReady { get; private set; }
        public string LoadedHtml { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;

        public void LoadHtml(string strHtml)
        {
            _nDocId++;
            IsReady = false;
            _pendingPosts.Clear();
            LoadedHtml = strHtml ?? String.Empty;
            _browser.DocumentText = LoadedHtml.Replace(CstrDocIdToken, _nDocId.ToString(CultureInfo.InvariantCulture));
        }

        public void Post(string strType, object payload = null)
        {
            var msg = HtmlMessage.Create(strType, payload);
            if (!IsReady)
                _pendingPosts.Add(msg);
            else
                Send(msg);
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            if (!IsReady)
                return null;

            var nRid = ++_nLastRid;
            _awaitedRids.Add(nRid);
            try
            {
                Send(HtmlMessage.Create(strType, payload).WithRid(nRid));
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
                        Debug.WriteLine($"IeHtmlHost: no reply to '{strType}' within {timeout.TotalMilliseconds} ms");
                        return null;
                    }
                    Application.DoEvents();     // replies arrive through BeginInvoke, so let the queue run
                    if (!_replies.ContainsKey(nRid))
                        Thread.Sleep(1);
                }
            }
            finally
            {
                _awaitedRids.Remove(nRid);
            }
        }

        public void ShowPrintPreview()
        {
            _browser.ShowPrintPreviewDialog();
        }

        public void Dispose()
        {
            _browser.Dispose();
        }

        private void Send(HtmlMessage msg)
        {
            var doc = _browser.Document;
            if (doc == null)
            {
                Debug.WriteLine("IeHtmlHost: no document; dropped " + msg);
                return;
            }
            doc.InvokeScript("oseReceive", new object[] { msg.ToJson() });
        }

        // called (on the UI thread) from inside the page's window.external.postMessage
        private void OnScriptMessage(string strJson)
        {
            var msg = HtmlMessage.TryParse(strJson);
            if (msg == null)
            {
                Debug.WriteLine("IeHtmlHost: unparseable message from page: " + strJson);
                return;
            }
            if (_browser.IsDisposed || !_browser.IsHandleCreated)
                return;
            _browser.BeginInvoke((Action)(() => Deliver(msg)));
        }

        private void Deliver(HtmlMessage msg)
        {
            switch (msg.Type)
            {
                case HtmlMessage.CstrTypeReply:
                    var nRe = msg.ReplyTo;
                    if (nRe.HasValue && _awaitedRids.Contains(nRe.Value))
                        _replies[nRe.Value] = msg;
                    return;     // late replies (after a timeout) are dropped

                case HtmlMessage.CstrTypeReady:
                    if (msg.DocId != _nDocId.ToString(CultureInfo.InvariantCulture))
                        return; // an older document finished loading after LoadHtml was called again
                    IsReady = true;
                    var aPending = _pendingPosts.ToArray();
                    _pendingPosts.Clear();
                    foreach (var pending in aPending)
                        Send(pending);
                    DocumentReady?.Invoke(this, EventArgs.Empty);
                    return;

                case HtmlMessage.CstrTypeJsError:
                    Debug.WriteLine($"IeHtmlHost: script error: {msg.GetString("message")} ({msg.GetString("source")}:{msg.Body["line"]})");
                    break;      // also passed on, so tests (and panes) can see it
            }
            MessageReceived?.Invoke(this, msg);
        }

        // IE reads FEATURE_BROWSER_EMULATION for the exe when the first WebBrowser is created in the process
        public static void EnsureIe9Mode(string strExeName)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
                key?.SetValue(strExeName, 9999, RegistryValueKind.DWord);
        }

        // what the page sees as window.external: exactly one method
        [ComVisible(true)]
        public sealed class ScriptingBridge
        {
            private readonly IeHtmlHost _host;

            internal ScriptingBridge(IeHtmlHost host)
            {
                _host = host;
            }

            // ReSharper disable once InconsistentNaming (JS calls it by this name)
            public void postMessage(string strJson)
            {
                _host.OnScriptMessage(strJson);
            }
        }
    }
}
```

In `StoryEditor/Program.cs`, replace the body of `AddIe9RegistryKey` (`:303-310`) with a call to the shared helper:

```csharp
        private static void AddIe9RegistryKey()
        {
            IeHtmlHost.EnsureIe9Mode("StoryEditor.exe");
        }
```

- [ ] **Step 7: Write the test helpers and the failing browser tests**

`StoryEditor.Tests/FakeHtmlHost.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// records what C# posts and requests; answers requests with OnRequest; lets a test raise page messages
    /// </summary>
    public class FakeHtmlHost : IHtmlHost
    {
        public readonly List<HtmlMessage> Posts = new List<HtmlMessage>();
        public readonly List<HtmlMessage> Requests = new List<HtmlMessage>();
        public Func<HtmlMessage, HtmlMessage> OnRequest { get; set; }

        public Control Control { get; } = new Panel();
        public bool IsReady { get; set; } = true;
        public string LoadedHtml { get; private set; }
        public int PrintPreviewCount { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;

        public void LoadHtml(string strHtml)
        {
            LoadedHtml = strHtml;
        }

        public void Post(string strType, object payload = null)
        {
            Posts.Add(HtmlMessage.Create(strType, payload));
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            var msg = HtmlMessage.Create(strType, payload);
            Requests.Add(msg);
            return IsReady ? OnRequest?.Invoke(msg) : null;
        }

        public void ShowPrintPreview()
        {
            PrintPreviewCount++;
        }

        public void Raise(string strType, object payload = null)
        {
            MessageReceived?.Invoke(this, HtmlMessage.Create(strType, payload));
        }

        public void RaiseReady()
        {
            IsReady = true;
            DocumentReady?.Invoke(this, EventArgs.Empty);
        }

        public static HtmlMessage Reply(object payload = null)
        {
            return HtmlMessage.Create(HtmlMessage.CstrTypeReply, payload);
        }

        public void Dispose()
        {
            Control.Dispose();
        }
    }
}
```

`StoryEditor.Tests/BrowserTestHelper.cs`:

```csharp
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace OneStoryProjectEditor.Tests
{
    internal static class BrowserTestHelper
    {
        private static bool _bEmulationSet;

        // the test runner isn't StoryEditor.exe, so put its own exe into IE9 mode (once, before the first browser)
        public static void EnsureIe9ModeForTestProcess()
        {
            if (_bEmulationSet)
                return;
            IeHtmlHost.EnsureIe9Mode(Process.GetCurrentProcess().ProcessName + ".exe");
            _bEmulationSet = true;
        }

        public static bool PumpUntil(Func<bool> condition, int nTimeoutMs = 10000)
        {
            var sw = Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > nTimeoutMs)
                    return false;
                Application.DoEvents();
                Thread.Sleep(5);
            }
            return true;
        }

        public static void Pump(int nMs)
        {
            PumpUntil(() => false, nMs);
        }

        public static IeHtmlHost CreateHostInOffscreenForm(out Form form)
        {
            EnsureIe9ModeForTestProcess();
            form = new Form
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-32000, -32000),
                Size = new Size(800, 600),
                ShowInTaskbar = false
            };
            var host = new IeHtmlHost(new HtmlHostOptions());
            form.Controls.Add(host.Control);
            form.Show();
            return host;
        }

        public static string Page(string strBodyHtml, params string[] astrScripts)
        {
            return "<html><head>" + PageScripts.ScriptBlock(astrScripts) + "</head><body>" + strBodyHtml + "</body></html>";
        }
    }
}
```

`StoryEditor.Tests/IeHtmlHostTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class IeHtmlHostTests
    {
        private const string CstrTestScript =
            "ose.on('echo', function (m) { return { text: m.text }; });" +
            "ose.on('sendBack', function (m) { ose.send('pong', { n: m.n }); });" +
            "ose.on('boom', function () { throw new Error('boom'); });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        private void LoadTestPage()
        {
            _host.LoadHtml(BrowserTestHelper.Page("hi", PageScripts.Bridge, CstrTestScript));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True, "page never sent 'ready'");
        }

        [Test]
        public void LoadHtml_RaisesDocumentReady()
        {
            var nReady = 0;
            _host.DocumentReady += (s, e) => nReady++;
            LoadTestPage();
            Assert.That(nReady, Is.EqualTo(1));
        }

        [Test]
        public void Request_RoundTripsAwkwardText()
        {
            LoadTestPage();
            const string strText = "a \"quote\" <b>&amp;</b>\r\nline2 \\ üकि";
            var reply = _host.Request("echo", new { text = strText }, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply, Is.Not.Null);
            Assert.That(reply.GetString("text"), Is.EqualTo(strText));
        }

        [Test]
        public void Post_DeliversPageMessageAsynchronously()
        {
            LoadTestPage();
            _host.Post("sendBack", new { n = 5 });
            Assert.That(_received.Exists(m => m.Type == "pong"), Is.False, "must not be delivered inside Post");
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "pong")), Is.True);
            Assert.That(_received.Find(m => m.Type == "pong").TryGetInt("n", out var n) && n == 5, Is.True);
        }

        [Test]
        public void Post_BeforeReady_IsSentOnceReady()
        {
            _host.LoadHtml(BrowserTestHelper.Page("hi", PageScripts.Bridge, CstrTestScript));
            _host.Post("sendBack", new { n = 1 });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "pong")), Is.True);
        }

        [Test]
        public void Request_HandlerThrows_RepliesWithErrorPromptly()
        {
            LoadTestPage();
            var sw = Stopwatch.StartNew();
            var reply = _host.Request("boom", null, TimeSpan.FromSeconds(5));
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(2000));
            Assert.That(reply, Is.Not.Null);
            Assert.That(reply.GetString("error"), Does.Contain("boom"));
            BrowserTestHelper.Pump(100);
            Assert.That(_received.Exists(m => m.Type == HtmlMessage.CstrTypeJsError), Is.True);
        }

        [Test]
        public void Request_NotReady_ReturnsNullImmediately()
        {
            Assert.That(_host.Request("echo", new { text = "x" }, TimeSpan.FromSeconds(5)), Is.Null);
        }

        [Test]
        public void Request_UnknownType_RepliesWithError()
        {
            LoadTestPage();
            var reply = _host.Request("noSuchThing", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply?.GetString("error"), Does.Contain("no handler"));
        }

        [Test]
        public void ReadyFromAnOlderDocument_IsIgnored()
        {
            // the first page delays its 'ready' until after the second LoadHtml; the host must stay not-ready for the
            //  second page until the second page's own 'ready'
            const string strSlowReady = "window.attachEvent('onload', function () { });";
            _host.LoadHtml(BrowserTestHelper.Page("first", PageScripts.Bridge, strSlowReady));
            _host.LoadHtml(BrowserTestHelper.Page("second", PageScripts.Bridge, CstrTestScript));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True);
            var reply = _host.Request("echo", new { text = "second" }, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply?.GetString("text"), Is.EqualTo("second"), "the ready host must be talking to the second page");
        }
    }
}
```

> `ReadyFromAnOlderDocument_IsIgnored` can't force the race deterministically. It checks the outcome: once ready, the host is talking to the second page. The doc-id check in `Deliver` is what makes this safe; review it directly.

- [ ] **Step 8: Run all the new tests and the full suite**

Run the build, then `/Tests:IeHtmlHostTests`. Then run the full suite. Expected: everything passes, 199 + the new tests.

- If `LoadHtml_RaisesDocumentReady` times out, the browser isn't in IE9 mode or the form has no handle. Check `document.documentMode` (it's in the `ready` payload) by logging `_received`.
- If a browser test hangs instead of failing, check that `PumpUntil` has a timeout everywhere.

- [ ] **Step 9: Commit**

```bash
git add StoryEditor/HtmlMessage.cs StoryEditor/HtmlMessageDispatcher.cs StoryEditor/IHtmlHost.cs StoryEditor/IeHtmlHost.cs StoryEditor/HtmlHostFactory.cs StoryEditor/PageScripts.cs StoryEditor/js/bridge.js StoryEditor/StoryEditor.csproj StoryEditor/Program.cs StoryEditor.Tests/HtmlMessageTests.cs StoryEditor.Tests/HtmlMessageDispatcherTests.cs StoryEditor.Tests/FakeHtmlHost.cs StoryEditor.Tests/BrowserTestHelper.cs StoryEditor.Tests/IeHtmlHostTests.cs
git commit -m "B1: IHtmlHost message channel over IE (HtmlMessage, dispatcher, IeHtmlHost, bridge.js)"
```

---
### Task 2: The small hosts (`MinimalHtmlForm`, `HtmlForm`, `NetBibleViewer`)

**Files:**
- Modify: `StoryEditor/js/bridge.js` (add the `ose.closest` and `ose.cancel` helpers and the `scrollTo` handler that every page uses)
- Create: `StoryEditor/js/HoverLinks.js`, `StoryEditor/js/NetBible.js`
- Modify: `StoryEditor/StoryEditor.csproj` (2 more embedded resources)
- Modify: `StoryEditor/NetBibleFootnoteTooltip.cs`, `StoryEditor/NetBibleFootnoteTooltip.designer.cs` (`MinimalHtmlForm` and its two subclasses)
- Modify: `StoryEditor/HtmlForm.cs`, `StoryEditor/HtmlForm.Designer.cs`
- Modify: `StoryEditor/NetBibleViewer.cs` (`:16`, `:44-82`, `:109-117`, `:371`, `:600-612`, `:720-731`, `:805-845`, `:913-916`), `StoryEditor/NetBibleViewer.Designer.cs` (`:35`, `:75-86`, `:109`, `:300`)
- Test: `StoryEditor.Tests/SmallHostPageTests.cs`

**Interfaces:**
- Consumes (Task 1): `IHtmlHost`, `HtmlHostFactory.Create(HtmlHostOptions)`, `HtmlMessageDispatcher`, `PageScripts`, `HtmlHostDefaults.RequestTimeout`, `BrowserTestHelper`
- Produces:
  - JS on every page: `ose.closest(el, predicate)` returns the nearest ancestor-or-self for which `predicate(el)` is true; `ose.cancel(e)` prevents the default action.
  - The `scrollTo {id, alignTop, focus}` command. When sent as a `Request`, it replies `{found}`.
  - Messages: `hoverRef {ref}`, `refMouseDown`, `refMouseOut {target, ref}`, `refMouseUp {target, ref}`.

- [ ] **Step 1: Write the failing page tests**

`StoryEditor.Tests/SmallHostPageTests.cs`:

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class SmallHostPageTests
    {
        // test-only driver: simulates the user's mouse on the first link/button
        private const string CstrDriver =
            "function oseFire(el, type) { var ev = document.createEvent('MouseEvents');" +
            "  ev.initMouseEvent(type, true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
            "  el.dispatchEvent(ev); }" +
            "ose.on('clickLink', function () { document.getElementsByTagName('a')[0].click(); });" +
            "ose.on('mouseButton', function (m) { oseFire(document.getElementsByTagName('button')[0], m.what); });" +
            "ose.on('echo', function (m) { return { text: m.text }; });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        private void Load(string strBody, string strPageScript)
        {
            _host.LoadHtml(BrowserTestHelper.Page(strBody, PageScripts.Bridge, PageScripts.Get(strPageScript), CstrDriver));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True);
        }

        private HtmlMessage WaitFor(string strType)
        {
            BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == strType), 3000);
            return _received.Find(m => m.Type == strType);
        }

        [Test]
        public void HoverLinks_LinkClick_SendsInnerHtml_AndDoesNotNavigate()
        {
            Load("<a href=\"x\">Gen 1:1</a><p id=\"Commentary0\">c</p>", "HoverLinks.js");
            _host.Post("clickLink");
            Assert.That(WaitFor("hoverRef")?.GetString("ref"), Is.EqualTo("Gen 1:1"));
            Assert.That(_host.Request("echo", new { text = "still here" }, HtmlHostDefaults.RequestTimeout)?.GetString("text"),
                        Is.EqualTo("still here"), "the click must not navigate the page away");
        }

        [Test]
        public void ScrollTo_AsRequest_ReportsWhetherFound()
        {
            Load("<p id=\"Commentary0\">c</p>", "HoverLinks.js");
            Assert.That(_host.Request("scrollTo", new { id = "Commentary0", alignTop = true }, HtmlHostDefaults.RequestTimeout)?.GetBool("found"), Is.True);
            Assert.That(_host.Request("scrollTo", new { id = "Commentary9", alignTop = true }, HtmlHostDefaults.RequestTimeout)?.GetBool("found", true), Is.False);
        }

        [Test]
        public void NetBible_LinkClick_SendsHrefAfterSixChars()
        {
            Load("<a href=\"sword:Gen 1:1\">1</a>", "NetBible.js");
            _host.Post("clickLink");
            Assert.That(WaitFor("hoverRef")?.GetString("ref"), Is.EqualTo("Gen 1:1"));
        }

        [TestCase("mousedown", "refMouseDown")]
        [TestCase("mouseup", "refMouseUp")]
        [TestCase("mouseout", "refMouseOut")]
        public void NetBible_ButtonMouse_SendsMessage(string strDomEvent, string strMessage)
        {
            Load("<button id=\"Gen 1:2\" value=\"Genesis 1:2\">2</button>", "NetBible.js");
            _host.Post("mouseButton", new { what = strDomEvent });
            var msg = WaitFor(strMessage);
            Assert.That(msg, Is.Not.Null);
            if (strMessage != "refMouseDown")
            {
                Assert.That(msg.GetString("target"), Is.EqualTo("Gen 1:2"));
                Assert.That(msg.GetString("ref"), Is.EqualTo("Genesis 1:2"));
            }
        }
    }
}
```

> The hrefs and ids above have the same shape as the real pages. Before you finalize the test values, check them against the HTML that `NetBibleViewer.DisplayVerses` builds (around `:540-600`). If the real href prefix isn't 6 characters, the original `substr(6)` still applies; keep it, and make the test match the real prefix.

- [ ] **Step 2: Run them to verify they fail**

Expected: they fail with "missing embedded script HoverLinks.js".

- [ ] **Step 3: Add the helpers and `scrollTo` to `bridge.js`, then write the two page scripts**

In `StoryEditor/js/bridge.js`, just before `function ready()`, add the following. These are page utilities every page needs, and none of them names a host:

```js
    // the nearest element (from el up) for which test(el) is true, or null
    function closest(el, test) {
        while (el && el.nodeType == 1) {
            if (test(el))
                return el;
            el = el.parentNode;
        }
        return null;
    }

    function cancel(e) {
        if (e.preventDefault)
            e.preventDefault();
        e.returnValue = false;
    }

    on('scrollTo', function (m) {
        var el = document.getElementById(m.id);
        if (!el)
            return { found: false };
        // later, so it runs after anything else waiting to scroll the document (what Application.DoEvents was for)
        setTimeout(function () {
            el.scrollIntoView(m.alignTop !== false);
            if (m.focus) {
                try { el.focus(); } catch (e) { }
            }
        }, 0);
        return { found: true };
    });
```

Change the export line to `window.ose = { send: send, on: on, closest: closest, cancel: cancel };`.

`StoryEditor/js/HoverLinks.js`:

```js
// HoverLinks.js: the commentary/info popup (HtmlForm). Clicking a link shows that reference in the Bible pane.
document.addEventListener('click', function (e) {
    var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
    if (!link)
        return;
    ose.cancel(e);
    ose.send('hoverRef', { ref: link.innerHTML });
}, false);
```

`StoryEditor/js/NetBible.js`:

```js
// NetBible.js: the Bible pane (NetBibleViewer). Footnote links show a tooltip; verse buttons can be clicked
//  (show commentary) or dragged out (to drop a reference on an anchor cell or a note).
(function () {
    function isButton(el) { return el.nodeName == 'BUTTON'; }

    document.addEventListener('click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
        if (!link)
            return;
        ose.cancel(e);
        ose.send('hoverRef', { ref: link.getAttribute('href').substr(6) });
    }, false);

    document.addEventListener('mousedown', function (e) {
        if (!ose.closest(e.target, isButton))
            return;
        ose.cancel(e);
        ose.send('refMouseDown');
    }, false);

    document.addEventListener('mouseup', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        ose.cancel(e);
        ose.send('refMouseUp', { target: btn.id, ref: btn.getAttribute('value') });
    }, false);

    document.addEventListener('mouseout', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        ose.send('refMouseOut', { target: btn.id, ref: btn.getAttribute('value') });
    }, false);
})();
```

In `StoryEditor.csproj`, add:

```xml
    <EmbeddedResource Include="js\HoverLinks.js" LogicalName="OneStoryProjectEditor.js.HoverLinks.js" />
    <EmbeddedResource Include="js\NetBible.js" LogicalName="OneStoryProjectEditor.js.NetBible.js" />
```

- [ ] **Step 4: Run the page tests to verify they pass**

Run `/Tests:SmallHostPageTests`. Expected: all pass.

- [ ] **Step 5: Move `MinimalHtmlForm` onto a host**

In `NetBibleFootnoteTooltip.designer.cs`:
- Remove every `this.webBrowser…` line: the `new`, the property block at `:34-45`, and `this.Controls.Add(this.webBrowser)` at `:54`.
- Remove the field declaration at `:75`.

In `NetBibleFootnoteTooltip.cs`:
- Add the field and set it up in the `MinimalHtmlForm` constructor. These are the same property values the designer had:

```csharp
    public partial class MinimalHtmlForm : Form
    {
        protected readonly IHtmlHost htmlHost;

        public MinimalHtmlForm()
        {
            InitializeComponent();

            htmlHost = HtmlHostFactory.Create();
            var ctrl = htmlHost.Control;
            ctrl.Margin = new Padding(6);
            ctrl.MaximumSize = new Size(1774, 1473);
            ctrl.MinimumSize = new Size(100, 29);
            ctrl.Name = "webBrowser";
            ctrl.TabIndex = 0;
            ctrl.PreviewKeyDown += webBrowser_PreviewKeyDown;
            Controls.Add(ctrl);
        }
```

- `MoveConNoteTooltip.SetDocumentText`: `webBrowser.DocumentText = aConNote.Html(...)` becomes `htmlHost.LoadHtml(aConNote.Html(...))`, with the same arguments.
- `NetBibleFootnoteTooltip`: `webBrowser.Focus()` (`:131`) becomes `htmlHost.Control.Focus()`, and `webBrowser.DocumentText = text;` (`:164`) becomes `htmlHost.LoadHtml(text);`. Keep the trailing comment.

- [ ] **Step 6: Move `HtmlForm` onto a host**

In `HtmlForm.Designer.cs`, remove the `webBrowser` `new`, its property block (`:38-47`), its `Controls.Add` (`:93`) and its field (`:104`).

In `HtmlForm.cs`:
- Remove `[System.Runtime.InteropServices.ComVisible(true)]`.
- Replace both script constants with one style constant:

```csharp
    public partial class HtmlForm : Form
    {
        private const string CstrStyle = "<style> body  { margin:1 } </style>";

        private readonly IHtmlHost _htmlHost;
        private readonly HtmlMessageDispatcher _dispatcher = new HtmlMessageDispatcher();
        private int _nIndexToScrollTo = 0;

        public HtmlForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            // IE's own context menu and file drop were on for this form before, so keep them
            _htmlHost = HtmlHostFactory.Create(new HtmlHostOptions { AllowBrowserContextMenu = true, AllowFileDrop = true });
            var ctrl = _htmlHost.Control;
            ctrl.Dock = DockStyle.None;
            ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ctrl.Location = new Point(0, 0);
            ctrl.MinimumSize = new Size(20, 20);
            ctrl.Name = "webBrowser";
            ctrl.Size = new Size(455, 364);
            ctrl.TabIndex = 0;
            Controls.Add(ctrl);

            _dispatcher.Register("hoverRef", m => ShowHoverOver(m.GetString("ref")));
            _htmlHost.MessageReceived += (s, m) => _dispatcher.Dispatch(m);
            _htmlHost.DocumentReady += (s, e) => UpdateButtonEnabledState(true);
        }
```

  - Check the designer's `Controls.Add` order. If the buttons were added after the browser, add `ctrl.SendToBack()` so z-order stays the same.
- `Show()`: delete `Application.DoEvents(); // to get the doc to load` and `UpdateButtonEnabledState(true);`. `DocumentReady` now does that.
- `ClientText`:

```csharp
        public string ClientText
        {
            get { return _htmlHost.LoadedHtml; }
            set
            {
                _htmlHost.LoadHtml(CstrStyle +
                                   PageScripts.ScriptBlock(PageScripts.Bridge, PageScripts.Get("HoverLinks.js")) +
                                   value);
            }
        }
```

- `ShowHoverOver` becomes `private` and loses its COM role. Its body doesn't change.
- `ScrollToElement`:

```csharp
        private bool ScrollToElement(int nElemName)
        {
            var reply = _htmlHost.Request("scrollTo",
                                          new { id = NetBibleViewer.CommentaryHeader + nElemName, alignTop = true },
                                          HtmlHostDefaults.RequestTimeout);
            return (reply != null) && reply.GetBool("found");
        }
```

- Add `using System.Drawing;` if it isn't there.

- [ ] **Step 7: Move `NetBibleViewer` onto a host**

In `NetBibleViewer.Designer.cs`, remove the `webBrowserNetBible` `new` (`:35`), its property block (`:75-86`, including the `DocumentCompleted` wiring), its `tableLayoutPanel.Controls.Add` (`:109`) and its field (`:300`).

In `NetBibleViewer.cs`:
- Remove `[System.Runtime.InteropServices.ComVisible(true)]` (`:16`).
- Replace `preDocumentDOMScript` and `postDocumentDOMScript` (`:44-82`) with:

```csharp
        protected const string preDocumentDOMScript = "<style> body { margin:0 } " + CstrReplaceWithStyle + " </style>";
```

- Add the fields and finish the constructor (`:109-117`):

```csharp
        private readonly IHtmlHost _htmlHost;
        private readonly HtmlMessageDispatcher _dispatcher = new HtmlMessageDispatcher();

        public NetBibleViewer()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            // these are the settings the designer had for webBrowserNetBible (file drop was left on)
            _htmlHost = HtmlHostFactory.Create(new HtmlHostOptions { AllowFileDrop = true });
            var ctrl = _htmlHost.Control;
            ctrl.ContextMenuStrip = contextMenuChangeFont;
            ctrl.MinimumSize = new Size(20, 20);
            ctrl.Name = "webBrowserNetBible";
            ctrl.TabIndex = 1;
            tableLayoutPanel.Controls.Add(ctrl, 0, 1);
            tableLayoutPanel.SetColumnSpan(ctrl, 2);

            _dispatcher.Register("hoverRef", m => ShowHoverOver(m.GetString("ref")));
            _dispatcher.Register("refMouseDown", m => OnMouseDown());
            _dispatcher.Register("refMouseOut", m => OnMouseOut(m.GetString("target"), m.GetString("ref")));
            _dispatcher.Register("refMouseUp", m => OnDoOnMouseUp(m.GetString("target"), m.GetString("ref")));
            _htmlHost.MessageReceived += (s, m) => _dispatcher.Dispatch(m);
            _htmlHost.DocumentReady += (s, e) => ScrollToElement();

            OnLocalizationChange(false);
            domainUpDownBookNames.ContextMenuStrip = contextMenuStripBibleBooks;
            checkBoxAutoHide.Checked = Properties.Settings.Default.AutoHideBiblePane;
        }
```

  If `tableLayoutPanel.Controls.Add` for the other controls happens inside a `SuspendLayout`/`ResumeLayout` pair in the designer, adding one more control after `InitializeComponent` is fine.
- Delete the comment block and `webBrowserNetBible.ObjectForScripting = this;` (`:364-371`).
- `:600-612`: build the page as

```csharp
                var strHtml = preDocumentDOMScript.Replace(CstrReplaceWithStyle, (bSpecifyFont)
                                                                                    ? GetTextStyle(strFontName, strFontSize)
                                                                                    : String.Empty)
                              + PageScripts.ScriptBlock(PageScripts.Bridge, PageScripts.Get("NetBible.js"))
                              + sb;
                _htmlHost.LoadHtml(strHtml);
```

- `ScrollToElement()` (`:720-731`):

```csharp
        private void ScrollToElement()
        {
            if (!String.IsNullOrEmpty(strIdToScrollTo))
                _htmlHost.Post("scrollTo", new { id = strIdToScrollTo, alignTop = true });
        }
```

- `OnMouseOut`: `webBrowserNetBible.DoDragDrop(...)` becomes `_htmlHost.Control.DoDragDrop(...)`.
- `OnMouseDown`, `OnMouseOut`, `OnDoOnMouseUp` and `ShowHoverOver` become `private`. They are only called through the dispatcher now.
- Delete `webBrowserNetBible_DocumentCompleted` (`:913-916`).
- Grep `NetBibleViewer.cs` for any other `webBrowserNetBible` use and route it through `_htmlHost` (`Control` for WinForms things, `LoadHtml`/`Post` for page things).

- [ ] **Step 8: Build, run the full suite, and grep**

Run the build and all tests. Expected: all pass.

```bash
grep -n "window.external\|ObjectForScripting\|DocumentText\|GetElementById\|webBrowser\b" StoryEditor/NetBibleViewer.cs StoryEditor/HtmlForm.cs StoryEditor/NetBibleFootnoteTooltip.cs
```

Expected: no output.

- [ ] **Step 9: Commit**

```bash
git add StoryEditor/js/bridge.js StoryEditor/js/HoverLinks.js StoryEditor/js/NetBible.js StoryEditor/StoryEditor.csproj StoryEditor/NetBibleFootnoteTooltip.cs StoryEditor/NetBibleFootnoteTooltip.designer.cs StoryEditor/HtmlForm.cs StoryEditor/HtmlForm.Designer.cs StoryEditor/NetBibleViewer.cs StoryEditor/NetBibleViewer.Designer.cs StoryEditor.Tests/SmallHostPageTests.cs
git commit -m "B2: NetBibleViewer, HtmlForm and the tooltip forms use IHtmlHost"
```

---
### Task 3: Pure helpers (`LineLabelParser`, `PaneText`, `HighlightedText`, `ReferringTextBuilder`)

These are the logic pieces Task 4 moves out of DOM-reading code. They're pure, so they get proper unit tests before anything depends on them.

**Files:**
- Create: `StoryEditor/LineLabelParser.cs`, `StoryEditor/PaneText.cs`, `StoryEditor/HighlightedText.cs`, `StoryEditor/ReferringTextBuilder.cs`
- Modify: `StoryEditor/HtmlStoryBtControl.cs` (`TryGetTextAreaId` changes from `protected static` to `internal static`; nothing else)
- Test: `StoryEditor.Tests/LineLabelParserTests.cs`, `StoryEditor.Tests/PaneTextTests.cs`, `StoryEditor.Tests/ReferringTextBuilderTests.cs`

**Interfaces:**
- Consumes: `HtmlMessage` (Task 1); the existing `VersesData.LinePrefix`, `VersesData.CstrZerothLineNameConNotes`, `VersesData.CstrZerothLineNameBtPane`, `VersesData.HiddenStringSpace`, `StoryEditor.CstrFirstVerse`, `StoryEditor.IsFirstCharsEqual`, `StoryData.NormalizeLineEndings`, `HtmlText.Encode`, `HtmlText.LineBreaksToBr`, `TextAreaIdentifier`
- Produces:
  - `LineLabelParser.TryParse(string strLabel, out string strLinkText, out int nLineIndex)`
  - `PaneText.IsSame(StringTransfer st, string strNewText)`, which compares after line-ending normalization, with null and "" equal
  - `HighlightedText` (`TextareaId`, `ClassName`, `Text`) and `HighlightedText.FromReply(HtmlMessage reply)`, which returns `List<HighlightedText>` and is never null
  - `ReferringTextBuilder.TryBuild(IEnumerable<HighlightedText> items, out string strReferringText)`, which returns false when an item's textarea id can't be parsed, and `ReferringTextBuilder.SpanHtml(HighlightedText)`

- [ ] **Step 1: Write the failing tests**

`StoryEditor.Tests/LineLabelParserTests.cs`:

```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class LineLabelParserTests
    {
        [Test]
        public void LineLabel_GivesLabelAndIndex()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "5", out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "5"));
            Assert.That(n, Is.EqualTo(5));
        }

        [Test]
        public void HiddenLineLabel_DropsTheHiddenSuffix()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "12" + VersesData.HiddenStringSpace, out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "12"));
            Assert.That(n, Is.EqualTo(12));
        }

        [Test]
        public void ConNoteZerothLine_GivesFirstVerseLabel()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.CstrZerothLineNameConNotes + " whatever", out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(StoryEditor.CstrFirstVerse));
            Assert.That(n, Is.EqualTo(0));
        }

        [Test]
        public void BtPaneZerothLine_GivesItsOwnName()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.CstrZerothLineNameBtPane, out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.CstrZerothLineNameBtPane));
            Assert.That(n, Is.EqualTo(0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("anchors")]
        public void Other_ReturnsFalse(string strLabel)
        {
            Assert.That(LineLabelParser.TryParse(strLabel, out _, out _), Is.False);
        }

        [Test]
        public void LineLabelWithoutNumber_ReturnsFalse_InsteadOfThrowing()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "x", out _, out _), Is.False);
        }
    }
}
```

`StoryEditor.Tests/PaneTextTests.cs`:

```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class PaneTextTests
    {
        [TestCase("abc", "abc", true)]
        [TestCase("a\nb", "a\r\nb", true)]     // the page sends \r\n; the model holds \n
        [TestCase(null, "", true)]
        [TestCase("", null, true)]
        [TestCase("abc", "abd", false)]
        [TestCase(null, "x", false)]
        public void IsSame(string strModel, string strFromPage, bool bExpected)
        {
            var st = new StringTransfer(strModel, StoryEditor.TextFields.Vernacular);
            Assert.That(PaneText.IsSame(st, strFromPage), Is.EqualTo(bExpected));
        }
    }
}
```

`StoryEditor.Tests/ReferringTextBuilderTests.cs`:

```csharp
using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ReferringTextBuilderTests
    {
        private static HighlightedText Item(string strId, string strClass, string strText)
        {
            return new HighlightedText(strId, strClass, strText);
        }

        [Test]
        public void SpanHtml_EncodesTextAndKeepsLineBreaks()
        {
            Assert.That(ReferringTextBuilder.SpanHtml(Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular highlight", "a<b> & c\r\nd")),
                        Is.EqualTo("<span class=\"LangVernacular highlight\">a&lt;b&gt; &amp; c<br>d</span>"));
        }

        [Test]
        public void OneItem_GivesFieldReferenceThenSpan_WithHighlightAndReadonlyRemoved()
        {
            var items = new List<HighlightedText> { Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular readonly highlight", "word") };
            Assert.That(ReferringTextBuilder.TryBuild(items, out var str), Is.True);
            Assert.That(str, Is.EqualTo("StoryLine : <span class=\"LangVernacular\">word</span>"));
        }

        [Test]
        public void TwoItems_AreJoinedWithVs()
        {
            // AddNote compares the previous *reference* name with the next *type* name, so they never match and every
            //  item starts a new " vs: " part. This characterizes that, so the change of mechanism doesn't change notes.
            var items = new List<HighlightedText>
            {
                Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular highlight", "one"),
                Item("ta_1_StoryLine_0_0_InternationalBt", "LangInternationalBt highlight", "two")
            };
            Assert.That(ReferringTextBuilder.TryBuild(items, out var str), Is.True);
            Assert.That(str, Is.EqualTo("StoryLine : <span class=\"LangVernacular\">one</span> vs: StoryLine : <span class=\"LangInternationalBt\">two</span>"));
        }

        [Test]
        public void NoItems_GivesNull_AndSucceeds()
        {
            Assert.That(ReferringTextBuilder.TryBuild(new List<HighlightedText>(), out var str), Is.True);
            Assert.That(str, Is.Null);
        }

        [Test]
        public void FromReply_ReadsItems_AndToleratesNull()
        {
            var reply = HtmlMessage.TryParse("{\"type\":\"reply\",\"items\":[{\"textareaId\":\"ta_1\",\"className\":\"c\",\"text\":\"t\"}]}");
            var list = HighlightedText.FromReply(reply);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].TextareaId, Is.EqualTo("ta_1"));
            Assert.That(list[0].ClassName, Is.EqualTo("c"));
            Assert.That(list[0].Text, Is.EqualTo("t"));
            Assert.That(HighlightedText.FromReply(null), Is.Empty);
        }
    }
}
```

> Don't add a test with a malformed textarea id. `TryGetTextAreaId` hits a `Debug.Assert` first, and in a Debug test run that shows a dialog.
>
> Check the expected `"StoryLine : "` against `TextAreaIdentifier.FieldReferenceName` (`TextAreaIdentifier.cs:40-53`): for a StoryLine field it is `FieldTypeName + " :"`. Also check `HtmlText.Encode` and `LineBreaksToBr` (`HtmlText.cs:19-48`) for the exact encoding of `<`, `>`, `&` and `\r\n`. If they differ, correct the test's expected string to match those functions; don't change the functions.

- [ ] **Step 2: Run them to verify they fail**

Expected: compile errors for the four missing types.

- [ ] **Step 3: Implement them**

`StoryEditor/LineLabelParser.cs`:

```csharp
using System;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// turns the text of a pane's top line-header cell (e.g. "Ln: 5", "Ln: 5 (Hidden)", "Gen Qs:", "Story: ...")
    /// into the line-number link's label and line index (was inline in HtmlVerseControl.OnScroll)
    /// </summary>
    internal static class LineLabelParser
    {
        public static bool TryParse(string strLabel, out string strLinkText, out int nLineIndex)
        {
            strLinkText = null;
            nLineIndex = 0;
            if (String.IsNullOrEmpty(strLabel))
                return false;

            if (StoryEditor.IsFirstCharsEqual(strLabel, VersesData.CstrZerothLineNameConNotes,
                                              VersesData.CstrZerothLineNameConNotes.Length))
            {
                strLinkText = StoryEditor.CstrFirstVerse;
                return true;
            }

            if (StoryEditor.IsFirstCharsEqual(strLabel, VersesData.CstrZerothLineNameBtPane,
                                              VersesData.CstrZerothLineNameBtPane.Length))
            {
                strLinkText = VersesData.CstrZerothLineNameBtPane;
                return true;
            }

            if (!StoryEditor.IsFirstCharsEqual(strLabel, VersesData.LinePrefix, VersesData.LinePrefix.Length))
                return false;

            // e.g. "Ln: 1" (or for the French localization: "Ln : 1") or "Ln: 1 (Hidden)"
            int nIndex;
            if ((nIndex = strLabel.IndexOf(VersesData.HiddenStringSpace, StringComparison.Ordinal)) != -1)
                strLabel = strLabel.Substring(0, nIndex);

            // the line number is the last bit after the last space (French has a space before the colon)
            nIndex = strLabel.LastIndexOf(' ');
            if ((nIndex == -1) || !Int32.TryParse(strLabel.Substring(nIndex + 1), out nLineIndex))
                return false;

            strLinkText = strLabel;
            return true;
        }
    }
}
```

`StoryEditor/PaneText.cs`:

```csharp
namespace OneStoryProjectEditor
{
    internal static class PaneText
    {
        // true when text from a pane is what the model already holds. Then nothing changed, so the project must not
        //  become Modified (a flush re-sends the focused box's text, which mustn't cause a "save changes?" prompt)
        public static bool IsSame(StringTransfer st, string strNewText)
        {
            var strNew = StoryData.NormalizeLineEndings(strNewText) ?? string.Empty;
            return strNew == (st.ToString() ?? string.Empty);
        }
    }
}
```

> Check that `StoryData.NormalizeLineEndings(null)` doesn't throw (`grep -n "static string NormalizeLineEndings" -A6 StoryEditor/StoryData.cs`). If it does, guard with `strNewText ?? string.Empty` before the call.

`StoryEditor/HighlightedText.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// one highlighted selection in a Story/BT textarea, as the page reports it (the 'getHighlights' reply in StoryBt.js).
    /// Replaces the HtmlElement spans C# used to read out of the DOM
    /// </summary>
    public sealed class HighlightedText
    {
        public HighlightedText(string strTextareaId, string strClassName, string strText)
        {
            TextareaId = strTextareaId;
            ClassName = strClassName;
            Text = strText;
        }

        public string TextareaId { get; }
        public string ClassName { get; }    // the span's class, e.g. "LangVernacular highlight"
        public string Text { get; }         // the span's innerText

        public static List<HighlightedText> FromReply(HtmlMessage reply)
        {
            var list = new List<HighlightedText>();
            if (reply?.Body["items"] is JArray items)
                list.AddRange(items.OfType<JObject>()
                                   .Select(item => new HighlightedText((string)item["textareaId"],
                                                                       (string)item["className"],
                                                                       (string)item["text"])));
            return list;
        }
    }
}
```

`StoryEditor/ReferringTextBuilder.cs`. This is ported line for line from `HtmlStoryBtControl.AddNote` (`:1520-1550`):

```csharp
using System;
using System.Collections.Generic;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// builds a new note's referring text from the highlighted selections ("Add note on selected text"),
    /// exactly as HtmlStoryBtControl.AddNote did from IE's span elements
    /// </summary>
    internal static class ReferringTextBuilder
    {
        // false if an item isn't in a recognisable textarea (AddNote gave up then, and still does)
        public static bool TryBuild(IEnumerable<HighlightedText> items, out string strReferringText)
        {
            var nLastSubItemIndex = -1;
            string strLastFieldReference = null;
            strReferringText = null;

            foreach (var item in items)
            {
                if (!HtmlStoryBtControl.TryGetTextAreaId(item.TextareaId, out var textAreaIdentifierParent))
                {
                    strReferringText = null;
                    return false;
                }

                // (this compares a reference name with a type name, so every item starts a new " vs: " part;
                //  kept as it was so notes come out the same)
                if (strLastFieldReference != textAreaIdentifierParent.FieldTypeName)
                {
                    if (!String.IsNullOrEmpty(strLastFieldReference))
                        strReferringText += " vs: ";

                    strLastFieldReference = textAreaIdentifierParent.FieldReferenceName;
                    strReferringText += strLastFieldReference;
                }
                else if (textAreaIdentifierParent.SubItemIndex != nLastSubItemIndex)
                {
                    if (nLastSubItemIndex != -1)
                        strReferringText += " &";
                    nLastSubItemIndex = textAreaIdentifierParent.SubItemIndex;
                }
                strReferringText += " " + SpanHtml(item);
            }

            // remove the highlight class so it isn't highlighted in the connote pane
            if (strReferringText != null)
            {
                strReferringText = strReferringText.Replace(" highlight", null);
                strReferringText = strReferringText.Replace(" readonly", null);
            }
            return true;
        }

        // what IE's span.OuterHtml gave (lower-case tag; NoteHtmlSanitizer keeps only span + Lang* classes anyway)
        public static string SpanHtml(HighlightedText item)
        {
            return "<span class=\"" + item.ClassName + "\">" +
                   HtmlText.LineBreaksToBr(HtmlText.Encode(item.Text ?? String.Empty)) +
                   "</span>";
        }
    }
}
```

In `HtmlStoryBtControl.cs`, change `protected static bool TryGetTextAreaId(` to `internal static bool TryGetTextAreaId(`.

- [ ] **Step 4: Run the tests to verify they pass**

Run `/Tests:LineLabelParserTests,PaneTextTests,ReferringTextBuilderTests`, then the full suite. Expected: all pass.

- [ ] **Step 5: Commit**

```bash
git add StoryEditor/LineLabelParser.cs StoryEditor/PaneText.cs StoryEditor/HighlightedText.cs StoryEditor/ReferringTextBuilder.cs StoryEditor/HtmlStoryBtControl.cs StoryEditor.Tests/LineLabelParserTests.cs StoryEditor.Tests/PaneTextTests.cs StoryEditor.Tests/ReferringTextBuilderTests.cs
git commit -m "B3: pure helpers for the panes (line label, same-text check, highlights, referring text)"
```

---

### Task 4: The panes' C# on `IHtmlHost`, plus `PaneCommon.js`

After this task, no pane C# touches the DOM. Every pane is a `UserControl` holding a host, and every message type the pages will send has a registered handler. The pane-specific page scripts are rewritten in Tasks 5 and 6, so after this task the pane pages are only partly wired. That's expected (Global Constraints).

**Files:**
- Create: `StoryEditor/js/PaneCommon.js`
- Modify: `StoryEditor/StoryEditor.csproj` (embedded resource)
- Modify: `StoryEditor/HtmlVerseControl.cs` (rewritten; full text below)
- Modify: `StoryEditor/HtmlStoryBtControl.cs`, `StoryEditor/HtmlStoryBtControl.Designer.cs:400`
- Modify: `StoryEditor/HtmlConNoteControl.cs`
- Modify: `StoryEditor/TextPaster.cs` (`:109-129`, `:150-153`, `:205-221`)
- Modify: `StoryEditor/StoryData.cs` (`AddHtmlHtmlDocOutside` `:496-505`; `ConNoteHtml`, `ConsultantNotesHtml`, `CoachNotesHtml` `:660-705`)
- Modify: `StoryEditor/html/StoryBt.htm`, `StoryEditor/js/StoryBt.js` (delete the document `keydown` block at the end, `:455-470`, which is now in PaneCommon.js; keep the `ctrl_down` keydown/keyup tracker above it)
- Modify: `StoryEditor/Properties/Resources.resx`: `HTML_Header` (remove `onKeyDown` and `onscroll` from `<body>`, keep `onmouseup` until Task 5); `HTML_LinkJumpLine`, `HTML_HttpLink`, `HTML_LinkJumpTargetBibleReference` (remove `onClick`)
- Modify: designer/caller files: `StoryEditor/StoryEditor.Designer.cs` (`:1770`, `:1772`, `:1852`, `:1896`), `StoryEditor/SwapColumnsForm.Designer.cs` (`:402`, `:405`, `:469`, `:472`), `StoryEditor/PrintViewer.Designer.cs` (`:82`, `:85`), `StoryEditor/PrintViewer.cs` (`:33`, `:39`, `:61`), `StoryEditor/PrintForm.cs:78`, `StoryEditor/LnCNotePrintForm.cs:15`, `StoryEditor/AddConNoteForm.cs:31`, `StoryEditor/StoryEditor.cs:2121-2124` (`TriggerSaveUpdates`)
- Test: `StoryEditor.Tests/PaneCommonPageTests.cs`, `StoryEditor.Tests/StoryBtPaneTests.cs`, `StoryEditor.Tests/ConNotePaneTests.cs`

**Interfaces:**
- Consumes: Task 1 (`IHtmlHost`, `HtmlMessage`, `HtmlMessageDispatcher`, `HtmlHostFactory`, `PageScripts`, `HtmlHostDefaults.RequestTimeout`), Task 3 (`LineLabelParser`, `PaneText`, `HighlightedText`, `ReferringTextBuilder`)
- Produces:
  - `HtmlVerseControl`:
    - `protected internal HtmlVerseControl(IHtmlHost host)` (null means use the factory)
    - `protected IHtmlHost Host`, `protected HtmlMessageDispatcher Dispatcher`
    - `public void LoadHtml(string)`, `public string LoadedHtml`, `public void ShowPrintPreview()`
    - `public bool FlushEdits(TimeSpan timeout)`
    - `public virtual void OnVerseLineJump(int)`, `protected virtual void OnRealign()`
    - `internal void SetTextareaText(string id, string text)`
    - `GetSelectedText`, `SetSelectedText`, `ClearSelection`, `ScrollToElement`, `ScrollToVerse`, `ResetDocument` and `ForgetWhereYouWere`, all with unchanged signatures
  - `HtmlStoryBtControl`:
    - `internal HtmlStoryBtControl(IHtmlHost host)`
    - `public List<HighlightedText> GetSelectedTexts(int nLineNumber)`
    - `StoryBtActions` constants: `Anchor = "anchor"`, `AnchorCell = "anchorCell"`, `LineOptions = "lineOptions"`
  - `HtmlConNoteControl`:
    - `protected internal HtmlConNoteControl(IHtmlHost host)`; subclasses have `public` parameterless and `internal (IHtmlHost)` constructors
    - `NoteActions` constants: `AddNote = "addNote"`, `AddNoteToSelf = "addNoteToSelf"`, `AddStickyNote = "addStickyNote"`, `ShowHideOpen = "showHideOpen"`, `Delete = "delete"`, `ConvertToMentoree = "convertToMentoree"`, `ConvertToMentor = "convertToMentor"`, `ConvertToMentorToSelf = "convertToMentorToSelf"`, `ConvertToMenteeToSelf = "convertToMenteeToSelf"`, `Approve = "approve"`, `EndConversation = "endConversation"`
  - `TextareaRef { Pane, Id, Text }`, and `TextPaster.TriggerPaste(bool, TextareaRef)`
  - The page protocol that `PaneCommon.js` implements. Tasks 5 and 6 rely on it:
    - **sends:** `bibRefJump {ref}`, `verseLineJump {index}`, `openUrl {url}`, `action {name, id, arg}`, `scriptureDropped {id}`, `save`, `reload`, `realign`, `scrolled {topId, topLabel, prevId, nextId}`, `textChanged {id, value, quiet}`
    - **handles:** `setText {id, text}`, `appendText {id, text, focus}`, `appendHtml {id, html}`, `setHtml {id, html}`, `selectRange {id, start, length}`, `clearSelection`, `getSelection → {selType, text}`, `replaceSelection {id, text} → {endPoint, ieHtml}`, `hasElement {id} → {found}`, `flush → {}` (sends `textChanged … quiet:true` for the last-focused textarea first)
    - **for page files:** `ose.lastTextareaId()`, `ose.replaceSelectionIn(id, text)`

- [ ] **Step 1: Write `PaneCommon.js` and its failing page tests**

`StoryEditor/js/PaneCommon.js`:

```js
// PaneCommon.js: what the Story/BT pane and the Consultant/Coach note panes have in common. Inlined after bridge.js
//  and before the pane's own script, which may replace any of these handlers with ose.on (the last one wins).
(function () {
    var lastTextareaId = null;

    function isTextarea(el) { return !!el && (el.nodeName == 'TEXTAREA'); }
    function textarea(id) { var el = id ? document.getElementById(id) : null; return isTextarea(el) ? el : null; }

    // focus doesn't bubble, but it can be captured
    document.addEventListener('focus', function (e) {
        if (isTextarea(e.target))
            lastTextareaId = e.target.id;
    }, true);

    // never let a click navigate the pane away; the links we make are sent to C# instead
    document.addEventListener('click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
        if (link) {
            ose.cancel(e);
            var href = link.getAttribute('href') || '';
            if (href == 'bibleViewer.setReference')
                ose.send('bibRefJump', { ref: link.getAttribute('name') });
            else if (href == 'conNote.jumpToLine')
                ose.send('verseLineJump', { index: link.getAttribute('name') });
            else if (/^https?:/i.test(href))
                ose.send('openUrl', { url: href });
            return;
        }
        var btn = ose.closest(e.target, function (el) { return !!el.getAttribute('data-action'); });
        if (btn) {
            ose.cancel(e);
            ose.send('action', { name: btn.getAttribute('data-action'), id: btn.id, arg: btn.getAttribute('data-arg') });
        }
    }, false);

    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && (e.keyCode == 83)) {           // Ctrl+S
            ose.cancel(e);
            ose.send('save');
        }
        else if (e.keyCode == 116) {                    // F5 (Ctrl+F5 also realigns the lines)
            ose.send(e.ctrlKey ? 'realign' : 'reload');
            try { window.event.keyCode = 0; } catch (ex) { }   // the only way to stop IE's own refresh
            ose.cancel(e);
        }
    }, false);

    // a reference dragged from the Bible pane onto an anchor cell or a note box
    function dropTarget(el) {
        return ose.closest(el, function (x) { return x.getAttribute('data-drop') == 'scripture'; });
    }
    document.addEventListener('dragover', function (e) {
        if (dropTarget(e.target))
            ose.cancel(e);
    }, false);
    document.addEventListener('drop', function (e) {
        var target = dropTarget(e.target);
        if (!target)
            return;
        ose.cancel(e);
        ose.send('scriptureDropped', { id: target.id });
    }, false);

    // which line is at the top, for the line-number link and for coming back to the same place after a reload
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
        var scrollTop = Math.max(document.documentElement.scrollTop, document.body.scrollTop);
        var cells = document.getElementsByTagName('td');
        var top = null, line = null;
        for (var i = 0; i < cells.length; i++) {
            var td = cells[i];
            if (!td.id)
                continue;
            if (topOf(td) > scrollTop + 1)
                break;
            top = td;
            if (td.id.indexOf('ln_') == 0)
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
    }, false);
    window.addEventListener('load', reportScroll, false);

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
        if (m.focus) {
            try { ta.focus(); } catch (e) { }
        }
    });

    ose.on('appendHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el)
            el.insertAdjacentHTML('beforeEnd', m.html);
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
        var range = ta.createTextRange();
        range.moveStart('character', m.start);
        range.moveEnd('character', -ta.value.length + m.start + m.length);
        range.select();
    });

    ose.on('clearSelection', function () {
        if (document.selection)
            document.selection.empty();
    });

    ose.on('getSelection', function () {
        if (!document.selection)
            return { selType: 'none', text: '' };
        var selType = String(document.selection.type).toLowerCase();
        return { selType: selType, text: (selType == 'text') ? document.selection.createRange().text : '' };
    });

    // replaces the selection (which must be in textarea 'id') with text; returns the new end point (0 = failed)
    function replaceSelectionIn(id, text) {
        var ta = textarea(id);
        if (!ta || !document.selection)
            return 0;
        var rangeSelection = document.selection.createRange();
        var rangeElement = rangeSelection.duplicate();
        rangeElement.moveToElementText(ta);
        var nEndPoint = 0;
        if (rangeElement.inRange(rangeSelection)) {
            rangeSelection.text = text;
            rangeSelection.select();
            while (rangeElement.compareEndPoints('StartToEnd', rangeSelection) < 0) {
                rangeElement.moveStart('character', 1);
                nEndPoint++;
            }
        }
        return nEndPoint;
    }

    ose.on('replaceSelection', function (m) {
        var nEndPoint = replaceSelectionIn(m.id, m.text);
        var ta = textarea(m.id);
        return { endPoint: nEndPoint, ieHtml: ta ? ta.innerHTML : null };
    });

    // send the last-focused box's text (it may have changed without a keyup, e.g. pasted with the mouse)
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

In `StoryEditor.csproj`, add:

```xml
    <EmbeddedResource Include="js\PaneCommon.js" LogicalName="OneStoryProjectEditor.js.PaneCommon.js" />
```

`StoryEditor.Tests/PaneCommonPageTests.cs`:

```csharp
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class PaneCommonPageTests
    {
        private const string CstrDriver =
            "ose.on('click', function (m) { document.getElementById(m.id).click(); });" +
            "ose.on('focusOn', function (m) { document.getElementById(m.id).focus(); });" +
            "ose.on('typeInto', function (m) { var ta = document.getElementById(m.id); ta.value = m.text; });" +
            "ose.on('innerHtml', function (m) { return { html: document.getElementById(m.id).innerHTML }; });" +
            "ose.on('fire', function (m) { var ev = document.createEvent('Event'); ev.initEvent(m.what, true, true);" +
            "  document.getElementById(m.id).dispatchEvent(ev); });" +
            "ose.on('echo', function (m) { return { text: m.text }; });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);

            var sb = new StringBuilder("<table>");
            for (var i = 0; i <= 40; i++)
                sb.AppendFormat("<tr style=\"height:60px\"><td id=\"ln_{0}\">{1}{0}</td><td id=\"anc_{0}\" data-drop=\"scripture\">a</td></tr>", i, VersesData.LinePrefix);
            sb.Append("</table>");
            sb.Append("<a id=\"l1\" href=\"conNote.jumpToLine\" name=\"3\">Ln 3</a>");
            sb.Append("<a id=\"l2\" href=\"bibleViewer.setReference\" name=\"Gen 1:1\">Gen 1:1</a>");
            sb.Append("<a id=\"l3\" href=\"http://example.com/x\">here</a>");
            sb.Append("<button id=\"btn_1_0_0\" data-action=\"delete\">Delete</button>");
            sb.Append("<button id=\"btn_1_0_1\" data-action=\"convertToMentoree\" data-arg=\"true\">Change</button>");
            sb.Append("<textarea id=\"ta_1_0\">abc</textarea><textarea id=\"ta_ro\" readonly>ro</textarea>");
            sb.Append("<p id=\"tp_1_0_0\">para</p>");

            _host.LoadHtml(BrowserTestHelper.Page(sb.ToString(), PageScripts.Bridge, PageScripts.Get("PaneCommon.js"), CstrDriver));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True);
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

        private bool StillThere()
        {
            return _host.Request("echo", new { text = "x" }, HtmlHostDefaults.RequestTimeout)?.GetString("text") == "x";
        }

        [TestCase("l1", "verseLineJump", "index", "3")]
        [TestCase("l2", "bibRefJump", "ref", "Gen 1:1")]
        [TestCase("l3", "openUrl", "url", "http://example.com/x")]
        public void LinkClick_SendsMessage_AndDoesNotNavigate(string strId, string strType, string strField, string strValue)
        {
            _host.Post("click", new { id = strId });
            Assert.That(WaitFor(strType)?.GetString(strField), Is.EqualTo(strValue));
            Assert.That(StillThere(), Is.True, "the click must not navigate the pane away");
        }

        [Test]
        public void ActionButton_SendsActionWithNameIdAndArg()
        {
            _host.Post("click", new { id = "btn_1_0_1" });
            var msg = WaitFor("action");
            Assert.That(msg.GetString("name"), Is.EqualTo("convertToMentoree"));
            Assert.That(msg.GetString("id"), Is.EqualTo("btn_1_0_1"));
            Assert.That(msg.GetBool("arg"), Is.True);
        }

        [Test]
        public void SetText_SendsTextChanged()
        {
            _host.Post("setText", new { id = "ta_1_0", text = "new text" });
            var msg = WaitFor("textChanged");
            Assert.That(msg.GetString("id"), Is.EqualTo("ta_1_0"));
            Assert.That(msg.GetString("value"), Is.EqualTo("new text"));
        }

        [Test]
        public void AppendText_SendsTheWholeNewValue()
        {
            _host.Post("appendText", new { id = "ta_1_0", text = " Gen 1:1", focus = true });
            Assert.That(WaitFor("textChanged")?.GetString("value"), Is.EqualTo("abc Gen 1:1"));
        }

        [Test]
        public void Flush_SendsLastFocusedTextQuietly_ThenReplies()
        {
            _host.Post("focusOn", new { id = "ta_1_0" });
            _host.Post("typeInto", new { id = "ta_1_0", text = "typed but no keyup" });
            BrowserTestHelper.Pump(100);
            var reply = _host.Request("flush", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply, Is.Not.Null);
            var msg = _received.Find(m => m.Type == "textChanged");
            Assert.That(msg, Is.Not.Null, "the textChanged must arrive before the reply");
            Assert.That(msg.GetString("value"), Is.EqualTo("typed but no keyup"));
            Assert.That(msg.GetBool("quiet"), Is.True);
        }

        [Test]
        public void Flush_ReadOnlyBox_SendsNothing()
        {
            _host.Post("focusOn", new { id = "ta_ro" });
            BrowserTestHelper.Pump(100);
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null);
            Assert.That(_received.Exists(m => m.Type == "textChanged"), Is.False);
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
        public void SetHtml_ReplacesContent()
        {
            _host.Post("setHtml", new { id = "tp_1_0_0", html = "<i>x</i>" });
            Assert.That(_host.Request("innerHtml", new { id = "tp_1_0_0" }, HtmlHostDefaults.RequestTimeout)?.GetString("html")?.ToLower(),
                        Is.EqualTo("<i>x</i>"));
        }

        [Test]
        public void HasElement_And_GetSelection_Answer()
        {
            Assert.That(_host.Request("hasElement", new { id = "ta_1_0" }, HtmlHostDefaults.RequestTimeout)?.GetBool("found"), Is.True);
            Assert.That(_host.Request("hasElement", new { id = "nope" }, HtmlHostDefaults.RequestTimeout)?.GetBool("found", true), Is.False);
            Assert.That(_host.Request("getSelection", null, HtmlHostDefaults.RequestTimeout)?.GetString("selType"), Is.Not.EqualTo("text"));
        }
    }
}
```

Run `/Tests:PaneCommonPageTests`. Expected: they fail with "missing embedded script PaneCommon.js" until the resource is added. Then they pass once the csproj line is in. This step adds JS only, so pass/fail depends only on PaneCommon.js and bridge.js.

> If `ScrollTo_ReportsTheTopLineAndItsNeighbours` is off by one row because of table borders and cell padding, don't loosen the JS. Instead, add `style="border-collapse:collapse"` and `cellpadding="0"` to the test table, so the row top equals the scroll position exactly the way `scrollIntoView(true)` leaves it.

- [ ] **Step 2: Rewrite `HtmlVerseControl.cs`**

Replace the whole file with the following. It keeps every public member SE and the forms use, with the same signatures:

```csharp
using System;
using System.Diagnostics;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// base of the HTML panes (Story/BT, Consultant Notes, Coach Notes). It holds an IHtmlHost (it used to *be* the
    /// IE WebBrowser) and talks to its page only through messages; see
    /// docs/superpowers/specs/2026-10-04-html-message-protocol-design.md
    /// </summary>
    public class HtmlVerseControl : UserControl
    {
        public const string CstrTextAreaPrefix = "ta";
        public const string CstrParagraphPrefix = "tp";
        public const string CstrButtonPrefix = "btn";

        public delegate void SetLineNumberLinkProc(string strText, int nLineIndex);
        internal SetLineNumberLinkProc SetLineNumberLink;

        public delegate void MakeLineNumberLinkVisibleProc();
        internal MakeLineNumberLinkVisibleProc MakeLineNumberLinkVisible;

        internal string StrIdToScrollTo;

        public StoryEditor TheSE { get; set; }
        public virtual StoryData StoryData { get; set; }

        protected readonly IHtmlHost Host;
        protected readonly HtmlMessageDispatcher Dispatcher = new HtmlMessageDispatcher();

        // what the page last reported about which line is at the top (the 'scrolled' message)
        private string _strTopRowId, _strPrevRowId, _strNextRowId;

        protected HtmlVerseControl()
            : this(null)
        {
        }

        protected internal HtmlVerseControl(IHtmlHost host)
        {
            Host = host ?? HtmlHostFactory.Create();
            Host.Control.Dock = DockStyle.Fill;
            Controls.Add(Host.Control);
            Host.MessageReceived += (sender, msg) => Dispatcher.Dispatch(msg);
            Host.DocumentReady += (sender, args) => OnDocumentReady();
            Dispatcher.ReportError = s => TheSE?.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), s));

            Dispatcher.Register("scrolled", OnScrolled);
            Dispatcher.Register("save", msg => TheSE?.SaveClicked());
            Dispatcher.Register("reload", msg => LoadDocument());
            Dispatcher.Register("realign", msg => OnRealign());
            Dispatcher.Register("bibRefJump", msg => OnBibRefJump(msg.GetString("ref")));
            Dispatcher.Register("openUrl", msg => OnUrlJump(msg.GetString("url")));
            Dispatcher.Register("verseLineJump", msg =>
            {
                if (msg.TryGetInt("index", out var nVerseIndex))
                    OnVerseLineJump(nVerseIndex);
            });
            Dispatcher.Register("textareaMouseDown", OnTextareaMouseDown);
            Dispatcher.Register(HtmlMessage.CstrTypeLog, msg => Debug.WriteLine(msg.GetString("text")));
            Dispatcher.Register(HtmlMessage.CstrTypeJsError, msg => { });   // the host has already logged it
        }

        public void LoadHtml(string strHtml)
        {
            Host.LoadHtml(strHtml);
        }

        public string LoadedHtml => Host.LoadedHtml;

        public void ShowPrintPreview()
        {
            Host.ShowPrintPreview();
        }

        public virtual void LoadDocument()
        {
            Debug.Assert(false);
        }

        // asks the page to send any edit it hasn't sent yet. True when it has (or when there's no page to ask)
        public bool FlushEdits(TimeSpan timeout)
        {
            if (!Host.IsReady)
                return true;
            var reply = Host.Request("flush", null, timeout);
            return (reply != null) && (reply.GetString("error") == null);
        }

        public virtual void OnVerseLineJump(int nVerseIndex)
        {
        }

        protected virtual void OnRealign()
        {
            LoadDocument();
        }

        private void OnUrlJump(string url)
        {
            // doing it this way allows us to launch the default browser defined rather than IE
            if (!String.IsNullOrEmpty(url))
                Process.Start(url);
        }

        private void OnBibRefJump(string strBibRef)
        {
            TheSE?.SetNetBibleVerse(strBibRef);
        }

        public virtual void ScrollToVerse(int nVerseIndex)
        {
            StrIdToScrollTo = VersesData.LineId(nVerseIndex);
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        private void OnScrolled(HtmlMessage msg)
        {
            _strTopRowId = msg.GetString("topId");
            _strPrevRowId = msg.GetString("prevId");
            _strNextRowId = msg.GetString("nextId");
            if ((SetLineNumberLink != null) &&
                LineLabelParser.TryParse(msg.GetString("topLabel"), out var strLinkText, out var nLineIndex))
            {
                SetLineNumberLink(strLinkText, nLineIndex);
            }
        }

        protected string GetTopRowId => _strTopRowId;
        protected string GetNextRowId => _strNextRowId ?? _strTopRowId;
        protected string GetPrevRowId => _strPrevRowId ?? _strTopRowId;

        private void OnDocumentReady()
        {
            if (!String.IsNullOrEmpty(StrIdToScrollTo))
                ScrollToElement(StrIdToScrollTo, true);
        }

        protected VerseData GetVerseData(int nLineIndex)
        {
            if (StoryData.Verses.Count <= (nLineIndex - 1))
                return null;
            return (nLineIndex == 0)
                       ? StoryData.Verses.FirstVerse
                       : StoryData.Verses[nLineIndex - 1];
        }

        public void ScrollToElement(String strElemName, bool bAlignWithTop)
        {
            Debug.Assert(!String.IsNullOrEmpty(strElemName));
            Host.Post("scrollTo", new { id = strElemName, alignTop = bAlignWithTop, focus = !bAlignWithTop });
        }

        public void ForgetWhereYouWere()
        {
            StrIdToScrollTo = null;
        }

        public void ResetDocument()
        {
            // reset so we don't jump to a soon-to-be-non-existant (or wrong context) place
            // update: if you *don't* want to jump there, then clear out StrIdToScrollTo manually. This needs
            //  to be here (e.g. for DoMove) which wants to go back to the same spot
            Host.LoadHtml(String.Empty);
        }

        protected static readonly char[] AchDelim = new[] { '_' };

        protected bool CheckForProperEditToken(out StoryEditor theSE)
        {
            theSE = TheSE;
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        Localizer.Str("Unable to edit the file! Restart the program and if it persists, contact bob_eaton@sall.com"));

                if (!theSE.IsInStoriesSet)
                    throw theSE.CantEditOldStoriesEx;

                theSE.LoggedOnMember.ThrowIfEditIsntAllowed(theSE.TheCurrentStory);
            }
            catch (Exception ex)
            {
                theSE?.SetStatusBar(String.Format(Localizer.Str("Error: {0}"), ex.Message));
                return false;
            }

            return true;
        }

        public virtual string GetSelectedText(StringTransfer stringTransfer)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            if (!IsTextareaElement(stringTransfer.HtmlElementId))
                return null;

            var reply = Host.Request("getSelection", new { id = stringTransfer.HtmlElementId }, HtmlHostDefaults.RequestTimeout);
            if (reply == null)
                return null;

            if (reply.GetString("selType") != "text")
            {
                LocalizableMessageBox.Show(Localizer.Str("Sorry, you can only modify editable text in consultant or coach notes!"),
                                           StoryEditor.OseCaption);
                return null;
            }
            return reply.GetString("text");
        }

        public bool IsParagraphElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrParagraphPrefix) == 0));
        }

        public bool IsTextareaElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrTextAreaPrefix) == 0));
        }

        public bool IsButtonElement(string strHtmlId)
        {
            return (!String.IsNullOrEmpty(strHtmlId) && (strHtmlId.IndexOf(CstrButtonPrefix) == 0));
        }

        public bool SetSelectedText(StringTransfer stringTransfer, string strNewValue, out int nNewEndPoint)
        {
            // this isn't allowed for paragraphs (it could be, but this is only currently called
            //  when we want to do 'replace', which isn't allowed for paragraphs (as opposed to textareas)
            Debug.Assert(IsTextareaElement(stringTransfer.HtmlElementId));
            nNewEndPoint = 0;   // 0 means it didn't work

            var reply = Host.Request("replaceSelection", new { id = stringTransfer.HtmlElementId, text = strNewValue },
                                     HtmlHostDefaults.RequestTimeout);
            if ((reply == null) || !reply.TryGetInt("endPoint", out nNewEndPoint) || (nNewEndPoint <= 0))
            {
                nNewEndPoint = 0;   // e.g. the selected portion wasn't in the element thought
                return false;
            }

            // now we have to update the string transfer with the new value
            var strIeHtml = reply.GetString("ieHtml");
            if (strIeHtml != null)
                stringTransfer.SetValue(HtmlText.FromIeHtmlText(strIeHtml));
            return true;
        }

        public void ClearSelection(StringTransfer stringTransfer)
        {
            Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (IsTextareaElement(stringTransfer.HtmlElementId))
            {
                Host.Post("clearSelection");
            }
            else if (IsParagraphElement(stringTransfer.HtmlElementId))
            {
                var strHtml = (stringTransfer is CommInstance)
                                  ? NoteHtmlSanitizer.ToReadOnlyHtml(stringTransfer.ToString())
                                  : HtmlText.ForParagraph(stringTransfer.ToString());
                Host.Post("setHtml", new { id = stringTransfer.HtmlElementId, html = strHtml });
            }
        }

        // TextPaster sets a textarea's text this way; the page then sends 'textChanged' like any other edit
        internal void SetTextareaText(string strId, string strText)
        {
            Host.Post("setText", new { id = strId, text = strText });
        }

        // button is the value JS reports (1 == left, as before)
        private void OnTextareaMouseDown(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            if ((StoryEditor.TextPaster == null) || (strId == null) || !msg.TryGetInt("button", out var nButton))
                return;

            StoryEditor.TextPaster.TriggerPaste(nButton == 1,
                                                new TextareaRef { Pane = this, Id = strId, Text = msg.GetString("value") ?? String.Empty });
        }
    }
}
```

> If `TheSE.SetStatusBar`, `SetNetBibleVerse` or `SaveClicked` aren't accessible from here (`internal`/`public`), they already were: the old file called them. Keep the access as it is.

- [ ] **Step 3: Update `TextPaster` to use `TextareaRef`**

In `TextPaster.cs`, add this class (next to `TextPaster` in the same file):

```csharp
    // a textarea in one of the HTML panes, as a paste/undo target (was an IE HtmlElement)
    internal class TextareaRef
    {
        public HtmlVerseControl Pane;
        public string Id;
        public string Text;     // what the textarea holds, as far as we know (updated when we set it)
    }
```

Then replace `SetTextareaText`, `SetElementText` and `GetTextareaText` (`:109-129`), the `TriggerPaste(bool, HtmlElement)` overload (`:150-153`) and the `UndoLast` branch (`:214-219`):

```csharp
        private void SetTextareaText(object tb, string str)
        {
            SetElementText((TextareaRef)tb, str);
        }

        internal static void SetElementText(TextareaRef textarea, string str)
        {
            textarea.Text = str;
            textarea.Pane.SetTextareaText(textarea.Id, str); // the page sends textChanged, which updates the data buffer
        }

        private string GetTextareaText(object tb)
        {
            return ((TextareaRef)tb).Text;
        }
```

```csharp
        internal void TriggerPaste(bool bLeftClicked, TextareaRef textarea)
        {
            TriggerPaste(bLeftClicked, textarea, SetTextareaText, GetTextareaText);
        }
```

```csharp
                else if (val.Item1 is TextareaRef textarea)
                    SetElementText(textarea, val.Item2);
```

Remove `using System.Windows.Forms` only if nothing else in the file needs it (it does, for `Form`), so most likely keep it.

- [ ] **Step 4: Move `HtmlStoryBtControl` onto the host**

Edit `HtmlStoryBtControl.cs` as follows. Use Grep `-n` to find each site; the line numbers are from the start of this task.

1. Remove `using System.Runtime.InteropServices;`, `using mshtml;` and `[ComVisible(true)]`.

2. Add the action names next to the class:

```csharp
    // data-mouseup values on the Story/BT page's buttons and cells (StoryBt.js turns them into 'action' messages)
    internal static class StoryBtActions
    {
        public const string Anchor = "anchor";          // an anchor button
        public const string AnchorCell = "anchorCell";  // the empty part of the anchor row
        public const string LineOptions = "lineOptions";
        public const string AnchorMenu = "anchorMenu";  // the action name sent for a right-click on either of the first two
    }
```

3. Replace the constructor (`:37-43`):

```csharp
        public HtmlStoryBtControl()
            : this(null)
        {
        }

        internal HtmlStoryBtControl(IHtmlHost host)
            : base(host)
        {
            InitializeComponent();
            ResetContextMenu();

            Dispatcher.Register("textChanged", OnTextChanged);
            Dispatcher.Register("focus", msg => TextareaOnFocus(msg.GetString("id")));
            Dispatcher.Register("blur", msg => Program.ActivateDefaultKeyboard());
            Dispatcher.Register("textareaMouseUp", msg => LastTextareaInFocusId = msg.GetString("id"));
            Dispatcher.Register("contextMenu", msg => ShowContextMenu(msg.GetString("id")));
            Dispatcher.Register("mouseMove", msg => TheSE?.CheckBiblePaneCursorPosition());
            Dispatcher.Register("scriptureDropped", msg => AddScriptureReference(msg.GetString("id")));
            Dispatcher.Register("action", OnAction);
        }

        private void OnAction(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            switch (msg.GetString("name"))
            {
                case StoryBtActions.AnchorMenu:
                    OnAnchorButton(strId);
                    break;
                case StoryBtActions.LineOptions:
                    OnLineOptionsButton(strId, msg.GetString("arg") == "right");
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("HtmlStoryBtControl: unknown action " + msg);
                    break;
            }
        }
```

4. `TriggerCtrlF5` (`:50-57`) becomes the realign override:

```csharp
        protected override void OnRealign()
        {
            TheSE.RealignLines();
            LoadDocument();     // (the page used to call LoadDocument itself after TriggerCtrlF5)
        }
```

5. `LoadDocument`: `DocumentText = strHtml;` becomes `LoadHtml(strHtml);`.

6. `OnVerseLineJump(int)` (`:151`) becomes `public override void OnVerseLineJump(int nVerseIndex)`.

7. Replace `GetSelectedTexts` and delete `TriggerOnBlur` (`:156-183`):

```csharp
        // the highlighted selections on this line (StoryBt.js first turns the current selection into one, as
        //  TriggerMyBlur always did)
        public List<HighlightedText> GetSelectedTexts(int nLineNumber)
        {
            var reply = Host.Request("getHighlights", new { tableId = VerseData.GetLineTableId(nLineNumber) },
                                     HtmlHostDefaults.RequestTimeout);
            return HighlightedText.FromReply(reply);
        }
```

8. Delete `_bIgnoringChanges` and `TriggerChangeUpdate` (`:246-268`), and both `GetHtmlElementById` overloads (`:385-410`). Delete `TextareaOnSelect` (dead) and `SelectFoundText` (dead; its JS never existed).

9. Replace `TextareaMouseUp`, `TextareaOnKeyUp`, `TextareaOnChange` and `SetFieldValue` (`:275-322`) with:

```csharp
        // textChanged: 'value' is a textarea's plain value (keyup, paste, set by C#); 'ieHtml' is IE's htmlText form
        //  (the onchange path in StoryBtPs.js); 'quiet' means it came from a flush (no error box for read-only boxes)
        private void OnTextChanged(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            if (strId == null)
                return;

            var bQuiet = msg.GetBool("quiet");
            var strIeHtml = msg.GetString("ieHtml");
            if ((strIeHtml == null) && !bQuiet)
            {
                LastTextareaInFocusId = strId;
                TheSE.LastKeyPressedTimeStamp = DateTime.Now;
            }

            var strText = (strIeHtml != null)
                              ? HtmlText.FromIeHtmlText(strIeHtml)
                              : (msg.GetString("value") ?? String.Empty);
            SetFieldValue(strId, strText, bQuiet);
        }

        private bool SetFieldValue(string strId, string strText, bool bQuiet)
        {
            var stringTransfer = GetStringTransfer(strId);
            if (stringTransfer == null)
                return false;

            // nothing changed (e.g. an arrow key, or a flush): don't mark the project modified
            if (PaneText.IsSame(stringTransfer, strText))
                return true;

            if (!CheckForProperEditToken(out var theSe))
                return false;

            if (bQuiet && stringTransfer.IsFieldReadonly(ViewSettings.FieldEditibility))
                return false;

            if (!CheckShowErrorOnFieldNotEditable(stringTransfer))
                return false;

            stringTransfer.SetValue(strText);

            // indicate that the document has changed
            theSe.Modified = true;

            // update the status bar (in case we previously put an error there
            var st = StoryStageLogic.stateTransitions[theSe.TheCurrentStory.ProjStage.ProjectStage];
            theSe.SetDefaultStatusBar(st.StageDisplayString);

            return true;
        }
```

  In `CheckShowErrorOnFieldNotEditable`, drop `&& !_bIgnoringChanges` from the condition.

10. `TextareaOnFocus` and `ShowContextMenu` stay as they are but become `private`. Delete `TextareaOnBlur`; the dispatcher calls `Program.ActivateDefaultKeyboard()` directly.

11. In `OnLineOptionsButton` (`:414-427`), delete the two lines that call `TriggerOnBlur(Document)` and its debug line. StoryBt.js now runs `TriggerMyBlur` before it sends the action. Make the method `private`. Make `OnAnchorButton` `private` too.

12. Replace the DOM part of `AddScriptureReference` (`:577-617`) so that it is:

```csharp
        private void AddScriptureReference(string strId)
        {
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return;

            int nLineIndex;
            if (!GetIndicesFromId(strId, out nLineIndex))
                return;

            var verseData = GetVerseData(nLineIndex);
            if (verseData == null)
                return;

            var strJumpTarget = TheSE.GetNetBibleScriptureReference;
            if (verseData.Anchors.Contains(strJumpTarget))
                return;

            var anchorNew = verseData.Anchors.AddAnchorData(strJumpTarget,
                                                            strJumpTarget);

            List<string> astrDontCare = null;
            var strButtonHtml = anchorNew.PresentationHtml(nLineIndex, null,
                                                           StoryData.PresentationType.Plain,
                                                           false,
                                                           ref astrDontCare);

            // the button's html already has its (localized) label in it
            Host.Post("appendHtml", new { id = strId, html = strButtonHtml });
            TheSE.Modified = true;
        }
```

13. In `MoveSelectedText` (`:708`), `GetSpanInnerText` (both overloads, `:1590-1607`), `AddNote` (`:1508-1555`) and `ClearSelectionSpans` (`:1561-1571`), change `IEnumerable<HtmlElement>` to `IEnumerable<HighlightedText>`:

```csharp
        private static string GetSpanInnerText(IEnumerable<HighlightedText> spans, string strId)
        {
            return spans.Where(s => (s.TextareaId == strId) && !String.IsNullOrEmpty(s.Text))
                        .Select(s => s.Text)
                        .FirstOrDefault();
        }
```

```csharp
        private void AddNote(bool bNoteToSelf)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(LastTextareaInFocusId));

            TextAreaIdentifier textAreaIdentifier;
            if (!TryGetTextAreaId(LastTextareaInFocusId, out textAreaIdentifier))
                return;

            var spans = GetSelectedTexts(textAreaIdentifier.LineIndex);
            if (!ReferringTextBuilder.TryBuild(spans, out var strReferringText))
                return;

            // if the user doesn't cancel, then clear out the spans/selected text (save a step for the next note)
            if (TheSE.SendNoteToCorrectPane(textAreaIdentifier.LineIndex, strReferringText, bNoteToSelf))
                ClearSelectionSpans(spans);
        }

        private void ClearSelectionSpans(IEnumerable<HighlightedText> spans)
        {
            foreach (var strTextareaId in spans.Select(s => s.TextareaId).Distinct())
                Host.Post("clearHighlight", new { id = strTextareaId });
        }
```

14. `onCutSelectedText` (`:1113-1132`): change `|| (this.Document == null)` to `|| !Host.IsReady`, and delete the `TriggerChangeUpdate();` line. `SetSelectedText` now updates the model from the page's reply.

15. `OnDoGlossing` (`:1384-1386`): replace the `HtmlElement siblingElement; if (!GetHtmlElementById(...)) return;` lines with:

```csharp
                var hasSibling = Host.Request("hasElement", new { id = siblingId }, HtmlHostDefaults.RequestTimeout);
                if ((hasSibling == null) || !hasSibling.GetBool("found"))
                    return;
```

16. Grep the file:

```bash
grep -n "Document\b\|HtmlElement\|HtmlDocument\|InvokeScript\|InvokeMember\|DocumentText\|ObjectForScripting\|IsWebBrowser" StoryEditor/HtmlStoryBtControl.cs StoryEditor/HtmlStoryBtControl.Designer.cs
```

  Fix each hit. `HtmlStoryBtControl.Designer.cs:400` `this.AllowWebBrowserDrop = false;` is deleted. Hits that are only in comments describing removed code (for example `// done by js TriggerOnBlur(Document);`) are deleted too.

- [ ] **Step 5: Move `HtmlConNoteControl` (and its two subclasses) onto the host**

Edit `HtmlConNoteControl.cs`:

1. Remove `using System.Runtime.InteropServices;`, `using mshtml;` and all three `[ComVisible(true)]`.

2. Add the action names (top of the namespace):

```csharp
    // data-action values on the note panes' buttons (VerseData/ConsultNoteDataConverter make them; PaneCommon.js
    //  sends them as 'action' messages)
    internal static class NoteActions
    {
        public const string AddNote = "addNote";
        public const string AddNoteToSelf = "addNoteToSelf";
        public const string AddStickyNote = "addStickyNote";
        public const string ShowHideOpen = "showHideOpen";
        public const string Delete = "delete";
        public const string ConvertToMentoree = "convertToMentoree";   // data-arg = needs approval (true/false)
        public const string ConvertToMentor = "convertToMentor";
        public const string ConvertToMentorToSelf = "convertToMentorToSelf";
        public const string ConvertToMenteeToSelf = "convertToMenteeToSelf";
        public const string Approve = "approve";
        public const string EndConversation = "endConversation";
    }
```

3. Replace the constructor:

```csharp
        protected HtmlConNoteControl()
            : this(null)
        {
        }

        protected internal HtmlConNoteControl(IHtmlHost host)
            : base(host)
        {
            InitializeComponent();

            Dispatcher.Register("textChanged", msg => TextareaOnKeyUp(msg.GetString("id"), msg.GetString("value") ?? String.Empty, msg.GetBool("quiet")));
            Dispatcher.Register("contextMenu", msg => ShowContextMenu());
            Dispatcher.Register("scriptureDropped", msg => CopyScriptureReference(msg.GetString("id")));
            Dispatcher.Register("action", OnAction);
        }

        private void OnAction(HtmlMessage msg)
        {
            var strId = msg.GetString("id");
            switch (msg.GetString("name"))
            {
                case NoteActions.AddNote:
                    if (Int32.TryParse(strId, out var nVerseIndex))     // this button's id is the bare line index
                        OnAddNote(nVerseIndex, null, false);
                    break;
                case NoteActions.AddNoteToSelf: OnAddNoteToSelf(strId); break;
                case NoteActions.AddStickyNote: OnAddStickyNote(strId); break;
                case NoteActions.ShowHideOpen: OnShowHideOpenConversations(strId); break;
                case NoteActions.Delete: OnClickDelete(strId); break;
                case NoteActions.ConvertToMentoree: OnConvertToMentoreeNote(strId, msg.GetBool("arg")); break;
                case NoteActions.ConvertToMentor: OnConvertToMentorNote(strId); break;
                case NoteActions.ConvertToMentorToSelf: OnConvertToMentorNoteToSelf(strId); break;
                case NoteActions.ConvertToMenteeToSelf: OnConvertToMentoreeNoteToSelf(strId); break;
                case NoteActions.Approve: OnApproveNote(strId); break;
                case NoteActions.EndConversation: OnClickEndConversation(strId); break;
                default:
                    System.Diagnostics.Debug.WriteLine("HtmlConNoteControl: unknown action " + msg);
                    break;
            }
        }

        // only the Consultant Notes pane has an Approve button
        protected virtual bool OnApproveNote(string strId)
        {
            return false;
        }
```

4. `OnClickDelete`:
   - Delete the `bRemovedLast` variable and the `if (bRemovedLast && RemoveHtmlNodeById(...) && RemoveHtmlNodeById(...)) return true;` block, together with its comment.
   - Leave `theCNsDC.Remove(theCNDC); LoadDocument(); return true;`.
   - Delete `RemoveHtmlNodeById`.

5. `OnClickHide`: delete the commented-out `/* ... */` block, which refers to `HtmlElement` and `Document`.

6. `OnClickEndConversation`:
   - Delete `System.Diagnostics.Debug.Assert(Document != null);`, the `HtmlElement elemButton = …` line and its assert, and both `elemButton.InnerText = …` lines. The reload at the end relabels the button.
   - Delete the whole `#else` branch of `#if !DontAlwaysDoLoadDoc`, keeping only the `#if` branch's code, and remove the `#if`/`#endif` lines.
   - Delete the `#if false` block near the end, which reads `elemButton.InnerText`.

7. Replace `CopyScriptureReference`:

```csharp
        private void CopyScriptureReference(string strId)
        {
            if (!GetIndicesFromId(strId, out int nVerseIndex, out int nConversationIndex, out int nDontCare))
                return;

            // the page appends it and sends textChanged, which puts it in the note like typing would
            Host.Post("appendText", new { id = strId, text = TheSE.GetNetBibleScriptureReference, focus = true });
        }
```

8. `ShowContextMenu()` becomes `private`.

9. `TextareaOnKeyUp` becomes `private bool TextareaOnKeyUp(string strId, string strText, bool bQuiet)`. Right after the `CheckForProperEditToken` block and the `aCI` lookup, add:

```csharp
            // nothing changed (e.g. an arrow key, or a flush): don't mark the project modified
            if (PaneText.IsSame(aCI, strText))
                return true;
```

   Change `theSE.LastKeyPressedTimeStamp = DateTime.Now;` to `if (!bQuiet) theSE.LastKeyPressedTimeStamp = DateTime.Now;`.

10. `SetSelection`:

```csharp
        public void SetSelection(StringTransfer stringTransfer,
            int nFoundIndex, int nLengthToSelect)
        {
            System.Diagnostics.Debug.Assert(stringTransfer.HasData && !String.IsNullOrEmpty(stringTransfer.HtmlElementId));
            if (IsTextareaElement(stringTransfer.HtmlElementId))
            {
                Host.Post("selectRange", new { id = stringTransfer.HtmlElementId, start = nFoundIndex, length = nLengthToSelect });
            }
            else if (IsParagraphElement(stringTransfer.HtmlElementId))
            {
                var str = NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight(stringTransfer.ToString(),
                                                                        nFoundIndex, nLengthToSelect,
                                                                        CstrParagraphHighlightBegin,
                                                                        CstrParagraphHighlightEnd);
                Host.Post("setHtml", new { id = stringTransfer.HtmlElementId, html = str });
            }
        }
```

11. Replace `ConNoteAddNote` (delete `regExReadLineNumber` too):

```csharp
        private void ConNoteAddNote(bool bNoteToSelf)
        {
            // the page finds the selection and the line it's on (getNoteSelection in ConNoteDomPrefix.js)
            var reply = Host.Request("getNoteSelection", null, HtmlHostDefaults.RequestTimeout);
            if ((reply == null) || !reply.TryGetInt("lineIndex", out var nLineNumber))
                return;

            var strHtml = reply.GetString("html");
            if (String.IsNullOrEmpty(strHtml))
                return;

            var strReferringText = String.Format("<p><i>{0}</i></p>", Localizer.Str("Re: ConNote:"));

            // add the selection to the referring text, but strip out any bits which look like table parts
            //  (they don't add so easily)
            strReferringText += regexStripTableBits.Replace(strHtml, "");
            TheSE.SendNoteToCorrectPane(nLineNumber, strReferringText, bNoteToSelf);
        }
```

12. `InitializeComponent`: delete `this.IsWebBrowserContextMenuEnabled = false;`.

13. Subclasses:
   - `DocumentText = strHtml;` becomes `LoadHtml(strHtml);` in both `LoadDocument`s.
   - `public void OnVerseLineJump(int)` becomes `public override void OnVerseLineJump(int)` in both.
   - `HtmlConsultantNotesControl.OnApproveNote` becomes `protected override bool OnApproveNote(string strId)`.
   - Add constructors to each subclass:

```csharp
        public HtmlConsultantNotesControl()
        {
        }

        internal HtmlConsultantNotesControl(IHtmlHost host)
            : base(host)
        {
        }
```

  (and the same for `HtmlCoachNotesControl`).

14. Grep:

```bash
grep -n "Document\b\|HtmlElement\|HtmlDocument\|InvokeScript\|DomDocument\|DocumentText\|ObjectForScripting\|IsWebBrowser\|IHTML\|HTMLDocument" StoryEditor/HtmlConNoteControl.cs
```

  Expected: no output, apart from `StoryData` property names that contain "Document". If grep matches those, refine the pattern.

- [ ] **Step 6: Update the page templates, the callers and the designers**

`StoryEditor/html/StoryBt.htm`: put the bridge and PaneCommon first, and drop the inline scroll handler. The file becomes:

```html
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
    <head>
        <title></title>
{6}
        <script type="text/javascript">
{0}
        </script>
        <script type="text/javascript">
{1}
{2}
        </script>
{3}
    </head>
    <body>
        <!--for debugging: <textarea id="osedebughtmlwindow"></textarea>-->
{4}
    </body>
<script type="text/javascript">
{5}
</script>
</html>
```

`StoryData.AddHtmlHtmlDocOutside` gets the 7th argument:

```csharp
            return String.Format(Properties.Resources.StoryBtHtml,
                                 Properties.Resources.jquery_min,
                                 Properties.Resources.StoryBtJs,
                                 GetPlaceHolders(projSettings),
                                 StylePrefix(projSettings, null, null),
                                 strHtmlInside,
                                 Properties.Resources.StoryBtPsJs,
                                 PageScripts.ScriptBlock(PageScripts.Bridge, PageScripts.Get("PaneCommon.js")));
```

In `StoryData.cs`, add next to these methods:

```csharp
        // the scripts the note panes' HTML_Header puts in its <script> block
        private static string ConNotePageScripts
        {
            get
            {
                return PageScripts.Bridge + Environment.NewLine +
                       PageScripts.Get("PaneCommon.js") + Environment.NewLine +
                       Properties.Resources.ConNoteDomPrefix;
            }
        }
```

In `ConNoteHtml`, `ConsultantNotesHtml` and `CoachNotesHtml`, replace `Properties.Resources.ConNoteDomPrefix,` with `ConNotePageScripts,`. Leave the `HTML_Script_AddTextareaMouseDown` argument alone until Task 5.

In `Properties/Resources.resx`:
- In `HTML_Header`, change `&lt;body onKeyDown="return OnKeyDown();" onscroll="window.external.OnScroll();" onmouseup="OnMouseUp();"&gt;` to `&lt;body onmouseup="OnMouseUp();"&gt;`.
- In `HTML_LinkJumpLine`, delete ` onClick="return OnVerseLineJump(this);"`.
- In `HTML_HttpLink`, delete ` onClick="return OnUrlJump(this);"`.
- In `HTML_LinkJumpTargetBibleReference`, delete ` onClick="return OnBibRefJump(this);"`.
- Update the matching doc comments in `Resources.Designer.cs` if they quote the old markup. They are comments only, but keeping them accurate avoids confusion.

In `js/StoryBt.js`, delete the trailing `$(document).keydown(function (e) { if (ctrl_down && (e.keyCode == s_key)) … });` block (Ctrl+S / F5, about `:455-470`). PaneCommon.js does this now. Keep the `ctrl_down` tracker block before it.

Callers:
- `StoryEditor.cs` `TriggerSaveUpdates`: `htmlStoryBtControl.TriggerChangeUpdate();` becomes `htmlStoryBtControl.FlushEdits(HtmlHostDefaults.RequestTimeout);`. Task 7 replaces this method.
- `PrintForm.cs:78` and `LnCNotePrintForm.cs:15`: `printViewer.webBrowser.DocumentText = X;` becomes `printViewer.webBrowser.LoadHtml(X);`.
- `PrintViewer.cs`: `webBrowser.DocumentText` becomes `webBrowser.LoadedHtml` (2 places, one inside `#if UseWordExportInGemBox`), and `webBrowser.ShowPrintPreviewDialog();` becomes `webBrowser.ShowPrintPreview();`.
- `AddConNoteForm.cs:31`: `pane.DocumentText = strHtmlNote;` becomes `pane.LoadHtml(strHtmlNote);`.

Designers: delete these lines. They are `WebBrowser` properties that a `UserControl` doesn't have, and the host sets the equivalent:
- `StoryEditor.Designer.cs`: `htmlStoryBtControl.AllowWebBrowserDrop`, `htmlStoryBtControl.IsWebBrowserContextMenuEnabled`, `htmlConsultantNotesControl.IsWebBrowserContextMenuEnabled`, `htmlCoachNotesControl.IsWebBrowserContextMenuEnabled`
- `SwapColumnsForm.Designer.cs`: the `AllowWebBrowserDrop` and `IsWebBrowserContextMenuEnabled` lines for `htmlStoryBtControlBefore` and `htmlStoryBtControlAfter`
- `PrintViewer.Designer.cs`: `webBrowser.AllowWebBrowserDrop`, `webBrowser.IsWebBrowserContextMenuEnabled`

Then build and fix whatever else the compiler names:

```bash
grep -rn "AllowWebBrowserDrop\|IsWebBrowserContextMenuEnabled\|WebBrowserShortcutsEnabled\|ScriptErrorsSuppressed" StoryEditor --include=*.cs | grep -v IeHtmlHost.cs
```

Expected: no output.

- [ ] **Step 7: Write the pane tests (fake host)**

`StoryEditor.Tests/StoryBtPaneTests.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class StoryBtPaneTests
    {
        private FakeHtmlHost _host;
        private HtmlStoryBtControl _pane;

        [SetUp]
        public void SetUp()
        {
            _host = new FakeHtmlHost();
            _pane = new HtmlStoryBtControl(_host);
        }

        [TearDown]
        public void TearDown()
        {
            _pane.Dispose();
        }

        [Test]
        public void HandlesEveryMessageTheStoryBtPageSends()
        {
            var aExpected = new[]
            {
                "scrolled", "save", "reload", "realign", "bibRefJump", "openUrl", "verseLineJump", "textareaMouseDown",
                "log", "jsError", "textChanged", "focus", "blur", "textareaMouseUp", "contextMenu", "mouseMove",
                "scriptureDropped", "action"
            };
            Assert.That(PaneDispatcher(_pane).RegisteredTypes, Is.SupersetOf(aExpected));
        }

        [Test]
        public void GetSelectedTexts_AsksForTheLinesTable_AndReadsTheReply()
        {
            _host.OnRequest = m => FakeHtmlHost.Reply(new
            {
                items = new[] { new { textareaId = "ta_2_StoryLine_0_0_Vernacular", className = "LangVernacular highlight", text = "w" } }
            });
            var list = _pane.GetSelectedTexts(2);
            Assert.That(_host.Requests.Single().Type, Is.EqualTo("getHighlights"));
            Assert.That(_host.Requests.Single().GetString("tableId"), Is.EqualTo(VerseData.GetLineTableId(2)));
            Assert.That(list.Single().Text, Is.EqualTo("w"));
        }

        [Test]
        public void GetSelectedTexts_NoReply_GivesEmptyList()
        {
            _host.OnRequest = m => null;
            Assert.That(_pane.GetSelectedTexts(1), Is.Empty);
        }

        [Test]
        public void ScrollToElement_PostsScrollTo_FocusingOnlyWhenNotAligningTop()
        {
            _pane.ScrollToElement("ln_3", true);
            _pane.ScrollToElement("ta_1", false);
            Assert.That(_host.Posts.Select(p => p.GetBool("focus")), Is.EqualTo(new[] { false, true }));
            Assert.That(_host.Posts.All(p => p.Type == "scrollTo"), Is.True);
        }

        [Test]
        public void DocumentReady_ScrollsToRememberedId()
        {
            _pane.StrIdToScrollTo = "ln_7";
            _host.RaiseReady();
            Assert.That(_host.Posts.Single().GetString("id"), Is.EqualTo("ln_7"));
        }

        [Test]
        public void Scrolled_UpdatesLineLinkAndTopRows()
        {
            string strText = null;
            var nLine = -1;
            _pane.SetLineNumberLink = (s, n) => { strText = s; nLine = n; };
            _host.Raise("scrolled", new { topId = "anc_4", topLabel = VersesData.LinePrefix + "4", prevId = "ln_3", nextId = "ln_5" });
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "4"));
            Assert.That(nLine, Is.EqualTo(4));
        }

        [Test]
        public void SetSelectedText_UpdatesModelFromReply()
        {
            var st = new StringTransfer("old", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_StoryLine_0_0_Vernacular" };
            _host.OnRequest = m => FakeHtmlHost.Reply(new { endPoint = 5, ieHtml = "a &amp; b" });
            Assert.That(_pane.SetSelectedText(st, "x", out var nEnd), Is.True);
            Assert.That(nEnd, Is.EqualTo(5));
            Assert.That(st.ToString(), Is.EqualTo("a & b"));
        }

        [Test]
        public void SetSelectedText_FailedReply_LeavesModel()
        {
            var st = new StringTransfer("old", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_StoryLine_0_0_Vernacular" };
            _host.OnRequest = m => FakeHtmlHost.Reply(new { endPoint = 0 });
            Assert.That(_pane.SetSelectedText(st, "x", out var nEnd), Is.False);
            Assert.That(nEnd, Is.EqualTo(0));
            Assert.That(st.ToString(), Is.EqualTo("old"));
        }

        [Test]
        public void ClearSelection_TextareaVsParagraph()
        {
            _pane.ClearSelection(new StringTransfer("t", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1" });
            _pane.ClearSelection(new StringTransfer("a<b", StoryEditor.TextFields.Vernacular) { HtmlElementId = "tp_1_0_0" });
            Assert.That(_host.Posts[0].Type, Is.EqualTo("clearSelection"));
            Assert.That(_host.Posts[1].Type, Is.EqualTo("setHtml"));
            Assert.That(_host.Posts[1].GetString("html"), Is.EqualTo(HtmlText.ForParagraph("a<b")));
        }

        [TestCase(true, null, true)]                // not ready: nothing to flush
        [TestCase(false, "ok", true)]
        [TestCase(false, null, false)]              // no reply (timed out)
        [TestCase(false, "error", false)]           // the page's flush handler threw
        public void FlushEdits(bool bNotReady, string strReply, bool bExpected)
        {
            _host.IsReady = !bNotReady;
            _host.OnRequest = m => (strReply == null) ? null
                                 : (strReply == "error") ? FakeHtmlHost.Reply(new { error = "boom" })
                                 : FakeHtmlHost.Reply();
            Assert.That(_pane.FlushEdits(HtmlHostDefaults.RequestTimeout), Is.EqualTo(bExpected));
        }

        internal static HtmlMessageDispatcher PaneDispatcher(HtmlVerseControl pane)
        {
            return (HtmlMessageDispatcher)typeof(HtmlVerseControl)
                .GetField("Dispatcher", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(pane);
        }
    }
}
```

`StoryEditor.Tests/ConNotePaneTests.cs`:

```csharp
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ConNotePaneTests
    {
        [Test]
        public void HandlesEveryMessageTheNotePagesSend()
        {
            using (var pane = new HtmlConsultantNotesControl(new FakeHtmlHost()))
            {
                var aExpected = new[]
                {
                    "scrolled", "save", "reload", "realign", "bibRefJump", "openUrl", "verseLineJump", "textareaMouseDown",
                    "log", "jsError", "textChanged", "contextMenu", "scriptureDropped", "action"
                };
                Assert.That(StoryBtPaneTests.PaneDispatcher(pane).RegisteredTypes, Is.SupersetOf(aExpected));
            }
        }

        [Test]
        public void SetSelection_TextareaPostsSelectRange()
        {
            var host = new FakeHtmlHost();
            using (var pane = new HtmlCoachNotesControl(host))
            {
                pane.SetSelection(new StringTransfer("hello", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_0" }, 1, 3);
                var post = host.Posts.Single();
                Assert.That(post.Type, Is.EqualTo("selectRange"));
                Assert.That(post.TryGetInt("start", out var nStart) && nStart == 1, Is.True);
                Assert.That(post.TryGetInt("length", out var nLen) && nLen == 3, Is.True);
            }
        }

        [Test]
        public void TextareaText_PostsSetText()
        {
            var host = new FakeHtmlHost();
            using (var pane = new HtmlCoachNotesControl(host))
            {
                pane.SetTextareaText("ta_1_0", "pasted");
                Assert.That(host.Posts.Single().GetString("text"), Is.EqualTo("pasted"));
            }
        }
    }
}
```

> If constructing a pane in a test throws, it's most likely `InitializeComponent` resource loading or a `Localizer` call. Read the exception, and fix it by giving the test what the real form gives (for example `ProjectSettings`). Don't add test-only branches to the pane. These tests need `[Apartment(ApartmentState.STA)]` because WinForms controls are created.

- [ ] **Step 8: Build and run everything**

Run the build and the full suite. Expected: all pass: the 199 originals plus Tasks 1–4.

```bash
grep -rn "window.external" StoryEditor/js StoryEditor/html StoryEditor/Properties/Resources.resx
```

Expected: hits only in `js/StoryBt.js`, `js/StoryBtPs.js`, `js/ConNoteDomPrefix.js`, the resx `HTML_Script_AddTextareaMouseDown`, `HTML_TableCellWidthDropAnchor`, and the button builders in `VerseData.cs`/`ConsultNoteDataConverter.cs` (grep `StoryEditor/*.cs` too). Tasks 5 and 6 remove all of these.

```bash
grep -rln "HtmlElement\b\|HtmlDocument\b\|InvokeScript\|InvokeMember\|DomDocument\|DocumentText\|ObjectForScripting\|using mshtml" StoryEditor --include=*.cs | grep -v "/obj/"
```

Expected: only `StoryEditor/IeHtmlHost.cs`.

- [ ] **Step 9: Commit**

```bash
git add -A StoryEditor StoryEditor.Tests
git status --short   # check that only the files listed for this task are staged
git commit -m "B4: panes hold an IHtmlHost; all pane C# talks to the page through messages; PaneCommon.js"
```

---
### Task 5: The note-pane pages (`ConNoteDomPrefix.js`, note buttons, templates)

**Files:**
- Modify: `StoryEditor/js/ConNoteDomPrefix.js` (rewritten; full text below)
- Modify: `StoryEditor/HtmlConNoteControl.cs` (add `NoteActions.ButtonHtml`)
- Modify: `StoryEditor/VerseData.cs:2051-2081` (note header buttons)
- Modify: `StoryEditor/ConsultNoteDataConverter.cs` (`:788`, `:912`, `:1080-1190`)
- Modify: `StoryEditor/StoryData.cs` (`ConNoteHtml`, `ConsultantNotesHtml`, `CoachNotesHtml`: drop the 4th format argument)
- Modify: `StoryEditor/Properties/Resources.resx` and `Properties/Resources.Designer.cs`:
  - `HTML_Header`: `<body>` without handlers; the `{3}` slot removed
  - `HTML_TextareaWithRefDoubleClick`: no inline handlers; add `data-drop` and `data-note`
  - delete `HTML_Script_AddTextareaMouseDown` and `HTML_ButtonClass`
- Create test helper: `StoryEditor.Tests/PaneTestData.cs`
- Test: `StoryEditor.Tests/ConNotePageTests.cs`

**Interfaces:**
- Consumes: Task 4 (`NoteActions`, the `PaneCommon.js` protocol, `HtmlConNoteControl` handlers), Task 1 test helpers
- Produces:
  - `NoteActions.ButtonHtml(string strId, string strClass, string strAction, string strLabel, string strArg = null)`
  - The page messages `textChanged {id, value}`, `textareaMouseDown {id, value, button}` and `contextMenu {}`
  - The command `getNoteSelection → {lineIndex, html} | {}`
  - `PaneTestData.LoadProject(string name)`, `PaneTestData.Stories(StoryProjectData)`. Task 6 reuses them.

- [ ] **Step 1: Write the test helper and the failing page test**

`StoryEditor.Tests/PaneTestData.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    internal static class PaneTestData
    {
        // a private copy of a TestData project, loaded the way the app loads it
        public static StoryProjectData LoadProject(string strName = "characterization-project")
        {
            var strFolder = Path.Combine(Path.GetTempPath(), "OseB_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(strFolder);
            var strPath = Path.Combine(strFolder, strName + ".onestory");
            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", strName + ".onestory"), strPath);
            var contents = ProjectFile.Load(strPath);
            return new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, strName));
        }

        public static IEnumerable<StoryData> Stories(StoryProjectData project)
        {
            return project.Values.Cast<StoriesData>().SelectMany(s => s.Cast<StoryData>());
        }
    }
}
```

`StoryEditor.Tests/ConNotePageTests.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class ConNotePageTests
    {
        private const string CstrDriver =
            "ose.on('clickFirst', function (m) { var els = document.querySelectorAll('[data-action=\"' + m.action + '\"]');" +
            "  if (els.length) els[0].click(); return { n: els.length }; });" +
            "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
            "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes;" +
            "    for (var j = 0; j < a.length; j++) if (a[j].specified && /^on/i.test(a[j].name)) n++; } return { n: n }; });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        // the Consultant Notes page of the first story, for the first team member who gets 'Add Note' buttons
        private void LoadConsultantNotesPage()
        {
            var project = PaneTestData.LoadProject();
            var story = PaneTestData.Stories(project).First(s => s.Verses.Count > 0);
            string strHtml = null;
            foreach (var member in project.TeamMembers.Values)
            {
                strHtml = story.ConsultantNotesHtml(null, project.ProjSettings, member, project.TeamMembers,
                                                    false, false, null, null);
                if (strHtml.Contains("data-action=\"" + NoteActions.AddNote + "\""))
                    break;
            }
            Assert.That(strHtml, Does.Contain("data-action=\"" + NoteActions.AddNote + "\""), "no member gets an Add Note button");
            _host.LoadHtml(strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True, "page never sent 'ready'");
        }

        [Test]
        public void Page_LoadsWithoutScriptErrors()
        {
            LoadConsultantNotesPage();
            BrowserTestHelper.Pump(300);
            Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
        }

        [Test]
        public void Page_HasNoInlineEventHandlers()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("countInline", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("n", out var n) && (n == 0), Is.True, "inline on* attributes left: " + n);
        }

        [Test]
        public void AddNoteButton_SendsActionWithLineIndexId()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("clickFirst", new { action = NoteActions.AddNote }, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("n", out var n) && n > 0, Is.True);
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "action")), Is.True);
            var msg = _received.First(m => m.Type == "action");
            Assert.That(msg.GetString("name"), Is.EqualTo(NoteActions.AddNote));
            Assert.That(msg.TryGetInt("id", out _), Is.True, "the Add Note button's id is the bare line index");
        }

        [Test]
        public void GetNoteSelection_NothingSelected_GivesNoLine()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("getNoteSelection", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply, Is.Not.Null);
            Assert.That(reply.TryGetInt("lineIndex", out _), Is.False);
        }

        [Test]
        public void Flush_Replies()
        {
            LoadConsultantNotesPage();
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null);
        }
    }
}
```

> `Page_HasNoInlineEventHandlers` checks that no `on*` attribute is left anywhere in the generated page. That proves no C#-built script remains in this page. If it reports a count, find the producer with `grep -rn "on[a-z]*=\\\\\"" StoryEditor/*.cs` and the resx, and convert it the same way.

Run `/Tests:ConNotePageTests`. Expected: `AddNoteButton…`, `Page_HasNoInlineEventHandlers` and `Page_LoadsWithoutScriptErrors` fail, because the buttons still carry `window.external` onclicks and the page still calls old functions.

- [ ] **Step 2: Add the button builder and convert the note buttons**

In `HtmlConNoteControl.cs`, inside `NoteActions`:

```csharp
        // a note-pane button; PaneCommon.js sends its data-action (and data-arg) when it's clicked
        public static string ButtonHtml(string strId, string strClass, string strAction, string strLabel, string strArg = null)
        {
            return String.Format("<button id=\"{0}\" class=\"{1}\" data-action=\"{2}\"{3}>{4}</button>",
                                 strId, strClass, strAction,
                                 (strArg == null) ? String.Empty : String.Format(" data-arg=\"{0}\"", strArg),
                                 strLabel);
        }
```

In `VerseData.cs` (`GetHeaderRow` for the note panes, `:2051-2081`), replace each `String.Format(Properties.Resources.HTML_ButtonClass, id, cls, "return window.external.X(...);", label)` with `NoteActions.ButtonHtml(id, cls, NoteActions.Y, label)`:

| old script string | `NoteActions` |
|---|---|
| `OnAddNote(this.id, null, false)` | `AddNote` |
| `OnAddNoteToSelf(this.id)` | `AddNoteToSelf` |
| `OnAddStickyNote(this.id)` | `AddStickyNote` |
| `OnShowHideOpenConversations(this.id)` | `ShowHideOpen` |

For example:

```csharp
                strHtmlButtons += NoteActions.ButtonHtml(nVerseIndex.ToString(),
                                                         StoryData.CstrLangLocalizationStyleClassName,
                                                         NoteActions.AddNote,
                                                         Localizer.Str("Add Note"));
```

(The old call passed `nVerseIndex`, an int, to `{0}`. Pass `nVerseIndex.ToString()`, which gives the same id text.)

In `ConsultNoteDataConverter.cs` (`:1080-1190`), do the same:

| old | new |
|---|---|
| `OnClickDelete(this.id)` | `NoteActions.ButtonHtml(…, NoteActions.Delete, StoryFrontMatterForm.CstrDeleteTest)` |
| commented-out `OnClickHide` block | delete the commented block |
| `strScriptCall = String.Format("…OnConvertToMentoreeNote(this.id, {0});", InitiatedByCit(...) ? "true" : "false")` | `strAction = NoteActions.ConvertToMentoree; strArg = InitiatedByCit(theTeamMembers) ? "true" : "false";` |
| `strScriptCall = "…OnConvertToMentorNote(this.id);"` | `strAction = NoteActions.ConvertToMentor; strArg = null;` |
| the `HTML_ButtonClass` call after that if/else | `NoteActions.ButtonHtml(ButtonId(…CnBtnIndexConvertToMentoreeNote), cls, strAction, Localizer.Str("Change to note"), strArg)` |
| `…OnConvertToMentorNoteToSelf(this.id);` | `strAction = NoteActions.ConvertToMentorToSelf;` |
| `…OnConvertToMentoreeNoteToSelf(this.id);` | `strAction = NoteActions.ConvertToMenteeToSelf;` (then `ButtonHtml(…, strAction, Localizer.Str("Change to note to self"))`) |
| `OnApproveNote(this.id)` | `NoteActions.Approve` |
| `GetEndOrOpenConversationButtonHtml`: `OnClickEndConversation(this.id)` | `NoteActions.EndConversation` |

Rename the local `string strScriptCall;` declarations to `string strAction; string strArg = null;` where both are needed.

At `:788` and `:912`, change `"<p ondblclick=\"OnDoubleClick(this)\">{0}</p>"` to `"<p data-note=\"ref\">{0}</p>"`.

- [ ] **Step 3: Update the templates**

In `Properties/Resources.resx`:
- `HTML_Header` value becomes (XML-escaped as the file does it):

  ```
  &lt;html&gt;
  {0}
  &lt;head&gt;
  &lt;script type="text/javascript"&gt;
  {1}
  &lt;/script&gt;
  &lt;/head&gt;
  &lt;body&gt;
  {2}
  &lt;/body&gt;
  &lt;/html&gt;
  ```

- `HTML_TextareaWithRefDoubleClick` value becomes `&lt;textarea id="{0}" data-note="edit" data-drop="scripture" class="{1}"&gt;{2}&lt;/textarea&gt;`.
- Delete the `HTML_Script_AddTextareaMouseDown` and `HTML_ButtonClass` `<data>` entries, and their properties in `Resources.Designer.cs`.

In `StoryData.cs`, the three note-page methods pass only 3 arguments to `HTML_Header`:

```csharp
            return String.Format(Properties.Resources.HTML_Header,
                                 StylePrefix(projSettings, strFontName, strFontSize),
                                 ConNotePageScripts,
                                 strHtml);
```

- [ ] **Step 4: Rewrite `ConNoteDomPrefix.js`**

Replace the whole file:

```js
// ConNoteDomPrefix.js: the Consultant/Coach note panes. Inlined after bridge.js and PaneCommon.js (which handle the
//  links, buttons, keys, drops, scrolling and the textarea commands).
function DisplayHtml(str) {
    if (window.oseDebug)
        ose.send('log', { text: str.replace(/\r\n/gm, "<nl>") });
}

function regexRemoveSpan(html) {
    return html.replace(/<span class=".*?">(.*?)<\/span>/gmi, "$1");
}

function ToNewLines(html) {
    return html.replace(/(<br>+)/gmi, "\r\n");
}

if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    }
}

// (unchanged from before, apart from being called by the dblclick listener below)
function OnDoubleClick(elem) {
```

…then copy the **body of the existing `OnDoubleClick`** unchanged, from its first line (`DisplayHtml("OnDoubleClick: start…`) to its closing brace. Then continue with:

```js
function transformText(e, type) {
    if (document.selection) {
        var rangeSelection = document.selection.createRange();
        var selectVal = rangeSelection.text;
        selectVal = selectVal.trim();
        if (selectVal) {
            if (!startsWith(selectVal, type) || !endsWith(selectVal, type)) {
                selectVal = type + selectVal + type + " ";
                rangeSelection.text = selectVal;
                rangeSelection.select();
            }
        }
    }
    try { window.event.keyCode = 0; } catch (ex) { }
    ose.cancel(e);
}

function startsWith(str, word) {
    return str.lastIndexOf(word, 0) === 0;
}
function endsWith(str, word) {
    return str.indexOf(word, str.length - word.length) !== -1;
}

// double-click selects a word in the note box or the referring text (data-note on both)
document.addEventListener('dblclick', function (e) {
    var el = ose.closest(e.target, function (x) { return !!x.getAttribute('data-note'); });
    if (el)
        OnDoubleClick(el);
}, false);

// Ctrl+B / Ctrl+I in the note box wrap the selection in $...$ / *...*
document.addEventListener('keydown', function (e) {
    if (!e.ctrlKey || !ose.closest(e.target, function (x) { return x.getAttribute('data-note') == 'edit'; }))
        return;
    if (e.keyCode == 66)
        transformText(e, "$");
    else if (e.keyCode == 73)
        transformText(e, "*");
}, false);

// right-click anywhere asks C# for the note context menu (window.event.button, as the old body onmouseup used)
document.onmouseup = function () {
    if (window.event.button == 2)
        ose.send('contextMenu', {});
};

// the note box's own events (this replaces the HTML_Script_AddTextareaMouseDown block; same event properties, so
//  the values sent are the same as before)
window.addEventListener('load', function () {
    var textareas = document.getElementsByTagName("textarea");
    for (var i = 0; i < textareas.length; i++) {
        textareas[i].onmousedown = function () { ose.send('textareaMouseDown', { id: this.id, value: this.value, button: window.event.button }); };
        textareas[i].onkeyup = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
        textareas[i].onselect = function () { this.focus(); };
        textareas[i].onchange = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
        textareas[i].onpaste = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
    }
}, false);

// the selection in a read-only part of the pane, and the line it's on, for "Add note on selected text"
ose.on('getNoteSelection', function () {
    if (!document.selection)
        return {};
    var range = document.selection.createRange();
    if (!range || !range.htmlText)
        return {};
    var re = /id="?tp_(\d+?)_/;
    var elem = range.parentElement();
    while (elem && !re.test(elem.innerHTML))
        elem = elem.parentElement;
    if (!elem)
        return {};
    return { lineIndex: parseInt(re.exec(elem.innerHTML)[1], 10), html: range.htmlText };
});
```

> - The old C# regex was `id=tp_(\d+?)_`, matching IE's habit of serializing `id=tp_1_0_0` without quotes. The JS accepts both forms, so it works whichever way IE9 mode serializes.
> - The old `dragover`/`drop` handlers aren't copied: PaneCommon.js handles `data-drop="scripture"`.
> - `textboxSetSelection` and `textboxSetSelectionTextReturnEndPosition` are now PaneCommon's `selectRange` and `replaceSelection`.
> - `OnKeyDown`, `OnBibRefJump`, `OnVerseLineJump`, `OnUrlJump` and `OnMouseUp` are replaced by PaneCommon's listeners and the `document.onmouseup` above.

- [ ] **Step 5: Run the page tests and the full suite**

Run `/Tests:ConNotePageTests`, then everything. Expected: all pass.

```bash
grep -n "window.external\|HTML_ButtonClass\|HTML_Script_AddTextareaMouseDown\|OnDoubleClick(this)" StoryEditor/js/ConNoteDomPrefix.js StoryEditor/VerseData.cs StoryEditor/ConsultNoteDataConverter.cs StoryEditor/StoryData.cs StoryEditor/Properties/Resources.resx StoryEditor/Properties/Resources.Designer.cs
```

Expected: no output.

- [ ] **Step 6: Commit**

```bash
git add StoryEditor/js/ConNoteDomPrefix.js StoryEditor/HtmlConNoteControl.cs StoryEditor/VerseData.cs StoryEditor/ConsultNoteDataConverter.cs StoryEditor/StoryData.cs StoryEditor/Properties/Resources.resx StoryEditor/Properties/Resources.Designer.cs StoryEditor.Tests/PaneTestData.cs StoryEditor.Tests/ConNotePageTests.cs
git commit -m "B5: note-pane pages on the protocol (data-action buttons, ConNoteDomPrefix.js, no inline handlers)"
```

---

### Task 6: The Story/BT page (`StoryBt.js`, `StoryBtPs.js`, anchor and line-option templates)

**Files:**
- Modify: `StoryEditor/js/StoryBt.js`, `StoryEditor/js/StoryBtPs.js`
- Modify: `StoryEditor/AnchorsData.cs:269-275` (`GetAnchorButtonHtml`), `StoryEditor/VerseData.cs:2007-2009` (line-options button)
- Modify: `StoryEditor/Properties/Resources.resx`: `HTML_ButtonToolTip`, `HTML_ButtonLineOptions`, `HTML_TableCellWidthDropAnchor`
- Test: `StoryEditor.Tests/StoryBtPageTests.cs`

**Interfaces:**
- Consumes: Task 4 (`StoryBtActions`, the `HtmlStoryBtControl` handlers, `PaneCommon.js`), Task 5 (`PaneTestData`)
- Produces: the Story/BT page side of the protocol:
  - **sends:** `focus {id}`, `blur {id}`, `contextMenu {id}`, `textareaMouseUp {id}`, `textareaMouseDown {id, value, button}`, `mouseMove` (at most every 100 ms), `textChanged {id, value}` (keyup), `textChanged {id, ieHtml, quiet}` (onchange/flush), `action {name: 'anchorMenu' | 'lineOptions', id, arg}`, `bibRefJump {ref}`
  - **handles:** `getHighlights {tableId} → {items:[{textareaId, className, text}]}`, `clearHighlight {id}`, `replaceSelection` (override), `flush` (override)

- [ ] **Step 1: Write the failing page tests**

`StoryEditor.Tests/StoryBtPageTests.cs`:

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
    public class StoryBtPageTests
    {
        private const string CstrDriver =
            // select the first n characters of a textarea the way a user would, then leave it (what .blur records)
            "ose.on('selectStart', function (m) { var ta = document.getElementById(m.id); ta.focus();" +
            "  var r = ta.createTextRange(); r.collapse(true); r.moveEnd('character', m.n); r.select();" +
            "  $(ta).triggerHandler('select'); window.oseConfig.idLastTextareaToBlur = ta.id; });" +
            "ose.on('focusOn', function (m) { document.getElementById(m.id).focus(); });" +
            "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
            "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes;" +
            "    for (var j = 0; j < a.length; j++) if (a[j].specified && /^on/i.test(a[j].name)) n++; } return { n: n }; });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();
        private string _strHtml;

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);

            var project = PaneTestData.LoadProject();
            var story = PaneTestData.Stories(project).First(s => s.Verses.Count > 0);
            var viewSettings = new VerseData.ViewSettings(project.ProjSettings,
                true, true, true, true,     // the four language columns
                true, true, true, true, true,   // anchors, exegetical notes, TQs, answers, retellings
                false, false, false,        // consultant notes, coach notes, bible viewer
                true, false, false, true,   // front matter, hidden, only open conversations, general TQs
                true,                       // use textareas (the editable pane)
                (StoryEditor.TextFields)~0, // everything editable
                null, null, null, null);
            _strHtml = story.PresentationHtml(viewSettings, project.ProjSettings, project.TeamMembers, null);
            _host.LoadHtml(_strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True, "page never sent 'ready'");
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        private string FirstStoryLineTextareaId(int nLine)
        {
            var match = Regex.Match(_strHtml, "id=\"(ta_" + nLine + "_StoryLine_0_0_[A-Za-z]+)\"");
            Assert.That(match.Success, Is.True, "no story line textarea on line " + nLine);
            return match.Groups[1].Value;
        }

        [Test]
        public void Page_LoadsWithoutScriptErrors()
        {
            BrowserTestHelper.Pump(300);
            Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
        }

        [Test]
        public void Page_HasNoInlineEventHandlers()
        {
            var reply = _host.Request("countInline", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("n", out var n) && (n == 0), Is.True, "inline on* attributes left: " + n);
        }

        [Test]
        public void GetHighlights_NothingSelected_IsEmpty()
        {
            var reply = _host.Request("getHighlights", new { tableId = VerseData.GetLineTableId(1) }, HtmlHostDefaults.RequestTimeout);
            Assert.That(HighlightedText.FromReply(reply), Is.Empty);
        }

        [Test]
        public void Selection_BecomesAHighlight_ThatCanBeCleared()
        {
            var strId = FirstStoryLineTextareaId(1);
            _host.Post("setText", new { id = strId, text = "abcdef ghi" });
            _host.Post("selectStart", new { id = strId, n = 3 });
            BrowserTestHelper.Pump(200);

            var items = HighlightedText.FromReply(_host.Request("getHighlights", new { tableId = VerseData.GetLineTableId(1) }, HtmlHostDefaults.RequestTimeout));
            Assert.That(items.Count, Is.EqualTo(1), "the selection must turn into one highlight");
            Assert.That(items[0].TextareaId, Is.EqualTo(strId));
            Assert.That(items[0].Text, Is.EqualTo("abc"));
            Assert.That(items[0].ClassName, Does.Contain("highlight"));
            Assert.That(ReferringTextBuilder.TryBuild(items, out var strReferring) && strReferring.Contains(">abc</span>"), Is.True);

            _host.Post("clearHighlight", new { id = strId });
            Assert.That(HighlightedText.FromReply(_host.Request("getHighlights", new { tableId = VerseData.GetLineTableId(1) }, HtmlHostDefaults.RequestTimeout)), Is.Empty);
        }

        [Test]
        public void SetText_ThenFlush_RoundTripsAwkwardText()
        {
            var strId = FirstStoryLineTextareaId(1);
            const string strText = "say \"hi\" <donkey bray> & more\r\nnext line ü";
            _host.Post("setText", new { id = strId, text = strText });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "textChanged")), Is.True);
            Assert.That(_received.First(m => m.Type == "textChanged").GetString("value"), Is.EqualTo(strText));

            _received.Clear();
            _host.Post("focusOn", new { id = strId });
            BrowserTestHelper.Pump(100);
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null);
            var msg = _received.FirstOrDefault(m => m.Type == "textChanged");
            Assert.That(msg, Is.Not.Null, "flush must send the focused box's text before replying");
            Assert.That(msg.GetBool("quiet"), Is.True);
            Assert.That(StoryData.NormalizeLineEndings(HtmlText.FromIeHtmlText(msg.GetString("ieHtml"))),
                        Is.EqualTo(StoryData.NormalizeLineEndings(strText)));
        }
    }
}
```

> - Check the 22 `ViewSettings` arguments against the constructor (`VerseData.cs:485-510`), and fix the order if this comment-labelled list is off.
> - If `focus()` doesn't move focus in the off-screen form, so `ose.lastTextareaId()` stays null and the flush sends nothing, show the form at `(0, 0)` with `Opacity = 0` in `BrowserTestHelper.CreateHostInOffscreenForm`, and call `form.Activate()`. Don't weaken the assertion.
> - If `Selection_BecomesAHighlight_ThatCanBeCleared` can't produce a selection programmatically in IE, mark only that test `[Explicit("IE won't select programmatically here; covered by the manual checklist")]`. Say so in the task report. Keep the rest.

Run `/Tests:StoryBtPageTests`. Expected: it fails, because the page still calls `window.external.*`, has inline handlers, and has no `getHighlights` handler.

- [ ] **Step 2: Update the Story/BT templates and their C# producers**

`Properties/Resources.resx`:
- `HTML_ButtonToolTip`: `&lt;button id="{0}" name="{1}" data-mouseup="{2}" title="{3}"&gt;{4}&lt;/button&gt;`
- `HTML_ButtonLineOptions`: `&lt;button id="{0}" data-mouseup="{1}" style="height:20px; width:20px;"&gt;{2}&lt;/button&gt;`
- `HTML_TableCellWidthDropAnchor`: `&lt;td id="{0}" width="{1}%" data-drop="scripture" data-mouseup="anchorCell"&gt;{2}&lt;/td&gt;` (`anchorCell` is `StoryBtActions.AnchorCell`)

`AnchorsData.GetAnchorButtonHtml`: the third argument `"return OnBibRefJump(this);"` becomes `StoryBtActions.Anchor`.
`VerseData.cs:2009`: `"return OnLineOptionsButton(this);"` becomes `StoryBtActions.LineOptions`.

- [ ] **Step 3: Edit `StoryBt.js`**

Make these edits. Everything else in the file (`removeSelection`, `CheckRemovePlaceHolder`, `removeSpan`, `regexRemoveSpan`, `ClearSelectionSpan`, `NewLinesToBRs`, `NewLinesToSpecialChars`, `ToNewLines`, `TriggerMyBlur`, the `.select`/`.focus`/`.dblclick` logic, `oseConfig`, the key-code variables and the `ctrl_down` tracker) stays exactly as it is.

1. Delete the global functions `OnBibRefJump(btn)`, `OnEmptyAnchorClick(id)`, `OnLineOptionsButton(btn)`, `OnVerseLineJump(link)` and `textboxSetSelectionTextReturnEndPosition(strId, strNewValue)`.

2. `DisplayHtml` becomes:

```js
function DisplayHtml(str) {
    // only when the debugging textarea (see StoryBt.htm) is in the page
    if ($('#osedebughtmlwindow').length)
        ose.send('log', { text: str.replace(/\r\n/gm, "<nl>") });
}
```

3. In `$(document).ready(...)`, replace the `window.external` calls:
   - `.blur`: `window.external.TextareaOnBlur(this.id);` becomes `ose.send('blur', { id: this.id });`
   - `.focus`: `window.external.TextareaOnFocus(this.id);` becomes `ose.send('focus', { id: this.id });`
   - `.mouseup`: `window.external.ShowContextMenu(this.id);` becomes `ose.send('contextMenu', { id: this.id });`, and `window.external.TextareaMouseUp(this.id);` becomes `ose.send('textareaMouseUp', { id: this.id });`
   - `.mousedown`: `window.external.OnTextareaMouseDown(this.id, this.value, event.button);` becomes `ose.send('textareaMouseDown', { id: this.id, value: this.value, button: event.button });`. Keep `event.button` exactly; see Global Constraints.
   - `.mousemove`: the handler becomes

     ```js
     }).mousemove(function () {
         // only the Bible pane's auto-hide listens, so a few times a second is plenty
         var now = new Date().getTime();
         if (now - window.oseConfig.lastMouseMove >= 100) {
             window.oseConfig.lastMouseMove = now;
             ose.send('mouseMove');
         }
     ```

     and add `lastMouseMove: 0` to `window.oseConfig`.
   - `.keyup`: `return window.external.TextareaOnKeyUp(this.id, this.value);` becomes `ose.send('textChanged', { id: this.id, value: this.value }); return true;`

4. After the `$(document).ready(...)` block, add the anchor/line-option listener and the Story/BT commands:

```js
// anchor buttons, the empty part of the anchor row, and the line-options buttons (data-mouseup in the templates)
document.addEventListener('mouseup', function (e) {
    var el = ose.closest(e.target, function (x) { return !!x.getAttribute('data-mouseup'); });
    if (!el)
        return;
    var what = el.getAttribute('data-mouseup');
    var bRight = (e.button == 2);
    if (what == 'lineOptions') {
        // capture the last textarea selected before it loses focus to do a context menu
        DisplayHtml("Calling TriggerMyBlur from lineOptions");
        TriggerMyBlur(true);
        ose.send('action', { name: 'lineOptions', id: el.id, arg: bRight ? 'right' : 'left' });
        ose.cancel(e);
    }
    else if (what == 'anchor') {
        if (bRight)
            ose.send('action', { name: 'anchorMenu', id: el.id });
        else
            ose.send('bibRefJump', { ref: el.getAttribute('name') });
        ose.cancel(e);  // and (being the nearest) the anchor cell's own right-click doesn't happen too
    }
    else if ((what == 'anchorCell') && bRight) {
        ose.send('action', { name: 'anchorMenu', id: el.id });
    }
}, false);

// the highlighted selections in one line's table, after turning the current selection into one (what C# did with
//  InvokeScript("TriggerMyBlur") and then reading the spans)
ose.on('getHighlights', function (m) {
    TriggerMyBlur();
    var items = [];
    var table = document.getElementById(m.tableId);
    if (table) {
        var spans = table.getElementsByTagName('span');
        for (var i = 0; i < spans.length; i++) {
            var ta = spans[i].parentNode;
            if (ta && (ta.nodeName == 'TEXTAREA'))
                items.push({ textareaId: ta.id, className: spans[i].className, text: spans[i].innerText });
        }
    }
    return { items: items };
});

ose.on('clearHighlight', function (m) {
    ClearSelectionSpan(m.id);
});

// like PaneCommon's, but first turns a highlight we made back into the selection
ose.on('replaceSelection', function (m) {
    var oTextbox = document.getElementById(m.id);
    if (oTextbox && oTextbox.selectedText) {
        var range = oTextbox.createTextRange();
        range.collapse(true);
        range.moveStart("character", oTextbox.selectionStart);
        range.moveEnd("character", oTextbox.selectionEnd - oTextbox.selectionStart);
        range.select();
    }
    var nEndPoint = ose.replaceSelectionIn(m.id, m.text);
    return { endPoint: nEndPoint, ieHtml: oTextbox ? oTextbox.innerHTML : null };
});
```

5. Check that the trailing `$(document).keydown(… s_key … f5_key …)` block is gone. Task 4 removed it, and PaneCommon.js handles those keys.

- [ ] **Step 4: Edit `StoryBtPs.js`**

Replace the `textareas[i].onchange = function () {...}` loop at the top with:

```js
// sends a textarea's text the way the onchange always did: IE's htmlText (which keeps the line breaks that
//  'value' loses once we've put <br>s and highlight spans in the box), minus spans, <br> -> \r\n
function oseSendChange(ta, bQuiet) {
    if (ta.readOnly)    // if edits aren't allowed, then our job is done here!
        return;

    // an empty box shows its language's name (with class 'hasPlaceholder'), but that isn't data
    if ($(ta).hasClass('hasPlaceholder')) {
        ose.send('textChanged', { id: ta.id, value: '', quiet: bQuiet });
        return;
    }

    var range = ta.createTextRange();
    ose.send('textChanged', { id: ta.id, ieHtml: ToNewLines(regexRemoveSpan(range.htmlText)), quiet: bQuiet });
}

var textareas = document.getElementsByTagName("textarea");
for (var i = 0; i < textareas.length; i++) {
    textareas[i].onchange = function () { oseSendChange(this, false); };
}

// the Story/BT flush sends the last-focused box the onchange way (replaces PaneCommon's)
ose.on('flush', function () {
    var ta = document.getElementById(ose.lastTextareaId() || '');
    if (ta && (ta.nodeName == 'TEXTAREA'))
        oseSendChange(ta, true);
    return {};
});
```

Keep the two placeholder blocks after it (`$('textarea').attr('placeholder', …)` and the `.each` that shows the language name) unchanged.

> `this.readonly` became `ta.readOnly` (the DOM property's real name). The old check never fired. C# still rejects read-only edits, and with `quiet` it now does so silently.

- [ ] **Step 5: Run the page tests, the full suite, and the window.external grep**

Run `/Tests:StoryBtPageTests`, then everything. Expected: all pass.

```bash
grep -rn "window.external" StoryEditor --include=*.js --include=*.cs --include=*.htm --include=*.resx | grep -v "/obj/\|/bin/"
```

Expected: only `StoryEditor/js/bridge.js`.

- [ ] **Step 6: Commit**

```bash
git add StoryEditor/js/StoryBt.js StoryEditor/js/StoryBtPs.js StoryEditor/AnchorsData.cs StoryEditor/VerseData.cs StoryEditor/Properties/Resources.resx StoryEditor.Tests/StoryBtPageTests.cs
git commit -m "B6: Story/BT page on the protocol (highlights via getHighlights, data-mouseup anchors and line options)"
```

---
### Task 7: Save flush (`PaneFlush`, the prompt, the four entry points)

**Files:**
- Create: `StoryEditor/PaneFlush.cs`
- Modify: `StoryEditor/StoryEditor.cs`:
  - the autosave tick (`:575-605`)
  - `CheckForSaveDirtyFileNoCleanup` (`:2043-2076`)
  - `SaveClicked` (`:2088-2116`)
  - `TriggerSaveUpdates` (`:2121-2124`, deleted)
  - `StoryEditor_FormClosing` (`:2981-3008`)
- Test: `StoryEditor.Tests/PaneFlushTests.cs`

**Interfaces:**
- Consumes: `HtmlVerseControl.FlushEdits(TimeSpan)` (Task 4), `HtmlConNoteControl.PaneLabel()`, `CustomMsgBox(frame, body, okText, retryText)` (OK, Retry and Cancel results)
- Produces:
  - `FlushChoice { Retry, SaveWithoutLatest, Cancel }` and `FlushOutcome { AllEditsCollected, SomeEditsMissing, Cancelled }`
  - `PaneFlush.Timeout` (3 s)
  - `PaneFlush.FlushAll(IEnumerable<KeyValuePair<string, Func<bool>>> panes)`, which returns the label of the first pane that didn't answer, or null
  - `PaneFlush.Run(Func<string> flushAll, Func<string, FlushChoice> askUser, bool bAutosave)`, which returns a `FlushOutcome`
  - `StoryEditor.FlushPendingEdits(bool bAutosave, bool bClosing)`, which returns a `FlushOutcome`

- [ ] **Step 1: Write the failing tests**

`StoryEditor.Tests/PaneFlushTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class PaneFlushTests
    {
        private static KeyValuePair<string, Func<bool>> Pane(string strLabel, Func<bool> flush)
        {
            return new KeyValuePair<string, Func<bool>>(strLabel, flush);
        }

        [Test]
        public void FlushAll_AllAnswer_GivesNull()
        {
            var nCalls = 0;
            var panes = new[] { Pane("Story", () => { nCalls++; return true; }), Pane("Coach", () => { nCalls++; return true; }) };
            Assert.That(PaneFlush.FlushAll(panes), Is.Null);
            Assert.That(nCalls, Is.EqualTo(2));
        }

        [Test]
        public void FlushAll_NamesTheFirstPaneThatDidNotAnswer()
        {
            var panes = new[] { Pane("Story", () => true), Pane("Consultant Notes", () => false), Pane("Coach", () => false) };
            Assert.That(PaneFlush.FlushAll(panes), Is.EqualTo("Consultant Notes"));
        }

        [Test]
        public void Run_AllCollected_NeverAsks()
        {
            var outcome = PaneFlush.Run(() => null, s => { Assert.Fail("must not ask"); return FlushChoice.Cancel; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.AllEditsCollected));
        }

        [Test]
        public void Run_RetryThenSuccess_IsAllCollected()
        {
            var aResults = new Queue<string>(new[] { "Story", null });
            var nAsked = 0;
            var outcome = PaneFlush.Run(() => aResults.Dequeue(), s => { nAsked++; return FlushChoice.Retry; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.AllEditsCollected));
            Assert.That(nAsked, Is.EqualTo(1));
        }

        [Test]
        public void Run_SaveWithout_IsSomeEditsMissing_AndPassesThePaneName()
        {
            string strAskedAbout = null;
            var outcome = PaneFlush.Run(() => "Coach Notes", s => { strAskedAbout = s; return FlushChoice.SaveWithoutLatest; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.SomeEditsMissing));
            Assert.That(strAskedAbout, Is.EqualTo("Coach Notes"));
        }

        [Test]
        public void Run_Cancel_IsCancelled()
        {
            Assert.That(PaneFlush.Run(() => "Story", s => FlushChoice.Cancel, false), Is.EqualTo(FlushOutcome.Cancelled));
        }

        [Test]
        public void Run_Autosave_NeverAsks_AndSkips()
        {
            var outcome = PaneFlush.Run(() => "Story", s => { Assert.Fail("autosave must not ask"); return FlushChoice.Retry; }, true);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.Cancelled));
        }
    }
}
```

- [ ] **Step 2: Run them to verify they fail**

Expected: compile errors (no `PaneFlush`).

- [ ] **Step 3: Implement `PaneFlush`**

`StoryEditor/PaneFlush.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace OneStoryProjectEditor
{
    internal enum FlushChoice
    {
        Retry,
        SaveWithoutLatest,
        Cancel
    }

    internal enum FlushOutcome
    {
        AllEditsCollected,  // save as normal
        SomeEditsMissing,   // the user chose to save without the latest typing in a pane that didn't answer
        Cancelled           // don't save (the user cancelled, or it's an autosave, which tries again next time)
    }

    /// <summary>
    /// before any save, every HTML pane is asked to send the edits it hasn't sent yet (see spec, Section 3). This is
    /// the policy for when a pane doesn't answer; StoryEditor supplies the panes and the prompt
    /// </summary>
    internal static class PaneFlush
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

        // the label of the first pane that didn't answer, or null when they all did
        public static string FlushAll(IEnumerable<KeyValuePair<string, Func<bool>>> panes)
        {
            foreach (var pane in panes)
                if (!pane.Value())
                    return pane.Key;
            return null;
        }

        public static FlushOutcome Run(Func<string> flushAll, Func<string, FlushChoice> askUser, bool bAutosave)
        {
            while (true)
            {
                var strPaneNotAnswering = flushAll();
                if (strPaneNotAnswering == null)
                    return FlushOutcome.AllEditsCollected;

                if (bAutosave)
                    return FlushOutcome.Cancelled;

                switch (askUser(strPaneNotAnswering))
                {
                    case FlushChoice.Retry:
                        continue;
                    case FlushChoice.SaveWithoutLatest:
                        return FlushOutcome.SomeEditsMissing;
                    default:
                        return FlushOutcome.Cancelled;
                }
            }
        }
    }
}
```

Run `/Tests:PaneFlushTests`. Expected: all pass.

- [ ] **Step 4: Wire it into `StoryEditor`**

In `StoryEditor.cs`, next to `SaveClicked`, add:

```csharp
        private bool _bInSave;  // e.g. Ctrl+S (or an autosave tick) arriving while a save is collecting edits

        // asks the HTML panes for any edit they haven't sent yet; see PaneFlush
        internal FlushOutcome FlushPendingEdits(bool bAutosave, bool bClosing)
        {
            var panes = new List<KeyValuePair<string, Func<bool>>>
            {
                new KeyValuePair<string, Func<bool>>(Localizer.Str("Story"), () => htmlStoryBtControl.FlushEdits(PaneFlush.Timeout)),
                new KeyValuePair<string, Func<bool>>(htmlConsultantNotesControl.PaneLabel(), () => htmlConsultantNotesControl.FlushEdits(PaneFlush.Timeout)),
                new KeyValuePair<string, Func<bool>>(htmlCoachNotesControl.PaneLabel(), () => htmlCoachNotesControl.FlushEdits(PaneFlush.Timeout))
            };
            return PaneFlush.Run(() => PaneFlush.FlushAll(panes),
                                 strPane => AskAboutUncollectedEdits(strPane, bClosing),
                                 bAutosave);
        }

        private static FlushChoice AskAboutUncollectedEdits(string strPane, bool bClosing)
        {
            var res = new CustomMsgBox(OseCaption,
                                       String.Format(Localizer.Str("The most recent typing in the {0} pane could not be collected (the pane is not responding)."),
                                                     strPane),
                                       Localizer.Str("Retry"),
                                       bClosing
                                           ? Localizer.Str("Save the rest and close")
                                           : Localizer.Str("Save without the latest typing"))
                .ShowDialog();
            switch (res)
            {
                case DialogResult.OK:
                    return FlushChoice.Retry;
                case DialogResult.Retry:
                    return FlushChoice.SaveWithoutLatest;
                default:
                    return FlushChoice.Cancel;
            }
        }
```

> Check `CustomMsgBox`: OK is the left button (here, Retry), Retry is the middle one (Save without), Cancel is the third. Check that closing it with the X gives `Cancel`, and that OK is the `AcceptButton` (the default). If OK isn't the default, set `AcceptButton = buttonOK` in `CustomMsgBox`'s constructor only if that doesn't change its other caller (`HtmlConNoteControl.OnClickDelete`). Otherwise leave it, and note it in the task report.

Replace `SaveClicked` (`:2088-2116`) and delete `TriggerSaveUpdates` (`:2119-2124`) along with its comment:

```csharp
        internal void SaveClicked()
        {
            if (_bInSave)
                return;
            _bInSave = true;
            try
            {
                // first, so an edit the panes hadn't sent yet counts for the Modified check below
                var outcome = FlushPendingEdits(false, false);
                if (outcome != FlushOutcome.Cancelled)
                    SaveAfterFlush(outcome);
            }
            finally
            {
                _bInSave = false;
            }
        }

        // for callers that have already flushed (and asked the user what they needed to)
        private void SaveAfterFlush(FlushOutcome outcome)
        {
            mySaveTimer.Stop();
            mySaveTimer.Interval = CnIntervalBetweenAutoSaveReqs;
            mySaveTimer.Start();

            if (!IsInStoriesSet || !Modified || (StoryProject == null) || (StoryProject.ProjSettings == null))
                return;

            string strFilename = StoryProject.ProjSettings.ProjectFilePath;

            bool bSaveThisSnapshotInRepo = (DateTime.Now - tmLastSync) > tsBackupTime;
            SaveFile(strFilename, bSaveThisSnapshotInRepo);

            // the user chose to save without the latest typing in a pane that didn't answer, so there's more to save
            //  once it does
            if (outcome == FlushOutcome.SomeEditsMissing)
                Modified = true;

            if (bSaveThisSnapshotInRepo)
            {
                try
                {
                    Program.SyncWithOneStoryRepository(StoryProject.ProjSettings.ProjectFolder, false);
                }
                catch (Exception ex)
                {
                    LocalizableMessageBox.Show(String.Format(ex.Message, strFilename),
                                    OseCaption);
                    return;
                }
                tmLastSync = DateTime.Now;
            }
        }
```

`CheckForSaveDirtyFileNoCleanup` (`:2043-2076`): insert the flush right after the "old stories / just looking" early return. Skip the "save changes?" question when the user has already chosen "Save without the latest typing":

```csharp
            var outcome = FlushPendingEdits(false, false);
            if (outcome == FlushOutcome.Cancelled)
                return false;   // (as if they'd clicked Cancel on 'save changes?')

            if (Modified)
            {
                // it's annoying that the keyboard doesn't deactivate so I can just type 'y' for "Yes"
                Program.ActivateDefaultKeyboard(); // ... do it manually

                if (!advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked && (outcome != FlushOutcome.SomeEditsMissing))
                {
                    var res = QuerySave();
                    if (res == DialogResult.Cancel)
                        return false;
                    if (res == DialogResult.No)
                    {
                        Modified = false;
                        return true;
                    }
                }

                SaveAfterFlush(outcome);
            }

            return true;
```

`StoryEditor_FormClosing` (`:2981-3008`), after `if (!IsInStoriesSet) return;`:

```csharp
            var outcome = FlushPendingEdits(false, true);
            if (outcome == FlushOutcome.Cancelled)
            {
                e.Cancel = true;
                return;
            }

            if (Modified)
            {
                if (!advancedSaveTimeoutAsSilentlyAsPossibleMenu.Checked && (outcome != FlushOutcome.SomeEditsMissing))
                {
                    DialogResult res = QuerySave();
                    if (res == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (res != DialogResult.Yes)
                    {
                        Modified = false;
                        return;
                    }
                }

                SaveAfterFlush(outcome);
            }
```

(leave the `#if UseAutoUpgrade` block after it as it is).

Autosave tick (`:575-605`):
- After `mySaveTimer.Stop();`, add

  ```csharp
              if (_bInSave)
              {
                  mySaveTimer.Start();
                  return;
              }

              // an autosave never asks about a pane that didn't answer; it tries again next time
              if (FlushPendingEdits(true, false) == FlushOutcome.Cancelled)
              {
                  mySaveTimer.Start();
                  return;
              }
  ```

- In the `if (res == DialogResult.Yes)` branch, replace `SaveClicked();` with `SaveAfterFlush(FlushOutcome.AllEditsCollected);`. It has just flushed.

Grep for leftovers:

```bash
grep -n "TriggerSaveUpdates\|TriggerChangeUpdate" StoryEditor/*.cs
```

Expected: no output.

- [ ] **Step 5: Build and run everything**

Expected: all tests pass.

- [ ] **Step 6: Commit**

```bash
git add StoryEditor/PaneFlush.cs StoryEditor/StoryEditor.cs StoryEditor.Tests/PaneFlushTests.cs
git commit -m "B7: every save path flushes the HTML panes first (Retry / save without / cancel when one doesn't answer)"
```

---

### Task 8: Cleanup and the architecture guard

**Files:**
- Delete: `OseResources/` (the whole folder; it's in no solution and nothing references it)
- Modify: `StoryEditor/StoryEditor.csproj` (remove the `Microsoft.mshtml` reference `:86-90` and the "mshtml PIA" mention in the comment at `:47-49`)
- Modify: `Installer/Setup OneStoryEditor/OseComponents.wxs` (remove the `Microsoft.mshtml.dll` component `:170-172` and any `ComponentRef Id="Microsoft.mshtml.dll"`)
- Test: `StoryEditor.Tests/ArchitectureGuardTests.cs`

**Interfaces:**
- Consumes: everything above
- Produces: the guard test that keeps sub-project B's rules from regressing

- [ ] **Step 1: Write the guard test**

`StoryEditor.Tests/ArchitectureGuardTests.cs`:

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// sub-project B's rules (spec: docs/superpowers/specs/2026-10-04-html-message-protocol-design.md): pages talk to
    /// C# only through js/bridge.js, and only IeHtmlHost touches the IE browser. Sub-project C relies on both
    /// </summary>
    [TestFixture]
    public class ArchitectureGuardTests
    {
        private static readonly Regex RegexBrowserApi = new Regex(
            @"\b(HtmlElement|HtmlDocument|InvokeScript|InvokeMember|DomDocument|DocumentText|ObjectForScripting|mshtml)\b",
            RegexOptions.Compiled);

        private static string StoryEditorSourceDir()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while ((dir != null) && !File.Exists(Path.Combine(dir.FullName, "StoryEditor 2017.sln")))
                dir = dir.Parent;
            Assert.That(dir, Is.Not.Null, "the repository root wasn't found above " + TestContext.CurrentContext.TestDirectory);
            return Path.Combine(dir.FullName, "StoryEditor");
        }

        private static IEnumerable<string> SourceFiles(params string[] astrExtensions)
        {
            return Directory.EnumerateFiles(StoryEditorSourceDir(), "*.*", SearchOption.AllDirectories)
                            .Where(f => !f.Contains(@"\obj\") && !f.Contains(@"\bin\"))
                            .Where(f => astrExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
        }

        [Test]
        public void OnlyTheBridgeScriptUsesWindowExternal()
        {
            var offenders = SourceFiles(".js", ".cs", ".htm", ".html", ".resx")
                .Where(f => !Path.GetFileName(f).Equals("bridge.js"))
                .Where(f => File.ReadAllText(f).Contains("window.external"))
                .Select(Path.GetFileName)
                .ToList();
            Assert.That(offenders, Is.Empty);
        }

        [Test]
        public void OnlyIeHtmlHostTouchesTheBrowser()
        {
            var offenders = SourceFiles(".cs")
                .Where(f => !Path.GetFileName(f).Equals("IeHtmlHost.cs"))
                .Where(f => RegexBrowserApi.IsMatch(File.ReadAllText(f)))
                .Select(f => Path.GetFileName(f) + ": " + RegexBrowserApi.Match(File.ReadAllText(f)).Value)
                .ToList();
            Assert.That(offenders, Is.Empty);
        }
    }
}
```

- [ ] **Step 2: Run it**

Run `/Tests:ArchitectureGuardTests`. Expected: both pass. If either fails, it names the file and the word:
- If the match is real code, convert it the way the earlier tasks did.
- If it's in a comment that describes removed code, delete the comment.
- If it's a legitimate unrelated use (for example a word inside an identifier that the `\b` boundaries still match), show it in the task report. Don't add an exclusion without saying why.

- [ ] **Step 3: Delete the dead code and the `mshtml` reference**

```bash
git rm -r -q OseResources
grep -rn "OseResources" --include=*.sln --include=*.csproj --include=*.wxs --include=*.props . | grep -v "/obj/"
```

Expected: no output from the grep.

- In `StoryEditor.csproj`, delete the `<Reference Include="Microsoft.mshtml, …">` element. In the comment above the references, remove the phrase ", or the machine-wide mshtml PIA" (keep the rest of the comment).
- In `Installer/Setup OneStoryEditor/OseComponents.wxs`, delete the `Microsoft.mshtml.dll` `<Component>` (`:170-172`). Then `grep -rn "Microsoft.mshtml" Installer` and delete any `<ComponentRef Id="Microsoft.mshtml.dll" />` it finds.

```bash
grep -rn "mshtml" StoryEditor --include=*.cs --include=*.csproj | grep -v "/obj/"
grep -rn "Microsoft.mshtml" Installer
```

Expected: no output.

- [ ] **Step 4: Clean build and full test run**

Delete `StoryEditor/obj` and `StoryEditor.Tests/obj` with Explorer or `git clean -ndX` (dry run first, and only for those two folders), so a stale reference can't hide. Then build and run all tests. Expected: everything passes. The total is 199 plus the tests added in Tasks 1–8.

- [ ] **Step 5: Commit**

```bash
git add -A StoryEditor/StoryEditor.csproj "Installer/Setup OneStoryEditor/OseComponents.wxs" StoryEditor.Tests/ArchitectureGuardTests.cs
git commit -m "B8: architecture guard test; drop the mshtml reference and the unused OseResources project"
```

- [ ] **Step 6: Hand over the manual smoke checklist**

Copy the **Manual smoke checklist** from the spec (Section 5) into the final report. Add the WinForms-key items from **Risks**: Ctrl+F/F3/Ctrl+H swallowed, Tab, Keyman switching, and the menu shortcuts while focus is in each pane. The user runs it against the released exe on IE. Automated tests can't check what users see in the real app.

---

## Self-review notes (for the executor)

- **Spec coverage:**

  | Spec | Task |
  |---|---|
  | Section 1, transport | 1 |
  | small hosts | 2 |
  | Section 2, pane restructure / state / table of replacements | 3, 4 |
  | note pages | 5 |
  | Story/BT page | 6 |
  | Section 3, flush | 7 |
  | Section 4, errors | 1 (dispatcher, host), 4 (status bar reporting) |
  | Section 5, tests | each task |
  | Decisions 2 (mouse-move throttle) | 6 |
  | Decisions 3–4 (dead code, mshtml) | 4, 8 |
  | Decision 5 (class names kept) | 4 |
  | Refinements | 2 (`scrollTo` request), 4 (`PaneCommon.js`, quiet flush, `removeElement` dropped), 2 (`HtmlForm` on `DocumentReady`) |

- **Deviations from the spec text, decided while planning:**
  - `bridge.js` also carries the `ose.closest` and `ose.cancel` helpers and the `scrollTo` handler, because every page, not only the panes, needs them. It is still the only file that names a host.
  - The note buttons are built by `NoteActions.ButtonHtml`, and the `HTML_ButtonClass` resx template is deleted. This is because one button needs a `data-arg` and the others don't.
  - "Unchanged text doesn't set `Modified`" is applied to every `textChanged`, not only the flush. Without it, an arrow key in a box would keep marking the project modified, and a flush would cause a "save changes?" prompt on every close.
