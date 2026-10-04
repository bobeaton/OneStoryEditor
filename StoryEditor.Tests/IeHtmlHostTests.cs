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
        public void Dispose_Twice_DoesNotThrow_AndLaterLoadsAreNoOps()
        {
            _host.Dispose();
            Assert.DoesNotThrow(() => _host.Dispose());
            Assert.DoesNotThrow(() => _host.LoadHtml("<html><body>late</body></html>"));
            Assert.DoesNotThrow(() => _host.Post("anything"));
            Assert.That(_host.IsReady, Is.False);
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
            var strText = "a \"quote\" <b>&amp;</b>\r\nline2 \\ " + HtmlMessageTests.AwkwardNonAscii;
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

        private const string CstrModeScript =
            "ose.on('mode', function () { return { mode: document.documentMode, hasJson: !!window.JSON }; });" +
            "ose.on('echoObj', function (m) { return { o: m.o }; });";

        // the pane pages have no doctype (or one IE treats as none), so they run in quirks mode, as they always have:
        //  no JSON and no addEventListener there, and the host doesn't change the mode
        [TestCase("<html><head>{0}</head><body>x</body></html>")]
        [TestCase("{0}<p>x</p>")]
        public void PageWithoutDoctype_StaysInQuirksMode_AndRoundTripsRequests(string strFormat)
        {
            _host.LoadHtml(string.Format(strFormat, PageScripts.ScriptBlock(PageScripts.Bridge, CstrTestScript, CstrModeScript)));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True, "page never sent 'ready'");
            var mode = _host.Request("mode", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(mode.TryGetInt("mode", out var nMode) && nMode < 8, Is.True, "documentMode " + nMode);
            Assert.That(mode.GetBool("hasJson", true), Is.False, "this test is about bridge.js's own JSON");

            // quotes, backslash, line breaks, a tab and another control character, the line separators that
            //  end a line in script source, and non-ASCII
            var strText = "a \"quote\" <b>&amp;</b>\r\nline2 \\ \t" + (char)1 + (char)0x2028 + (char)0x2029 + HtmlMessageTests.AwkwardNonAscii;
            Assert.That(_host.Request("echo", new { text = strText }, HtmlHostDefaults.RequestTimeout)?.GetString("text"), Is.EqualTo(strText));

            var reply = _host.Request("echoObj", new { o = new { a = new object[] { 1, "two", null, false }, b = true, c = (string)null, d = 1.5, e = -3 } },
                                      HtmlHostDefaults.RequestTimeout);
            Assert.That(reply?.Body["o"]?.ToString(Newtonsoft.Json.Formatting.None),
                        Is.EqualTo("{\"a\":[1,\"two\",null,false],\"b\":true,\"c\":null,\"d\":1.5,\"e\":-3}"));

            var error = _host.Request("boom", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(error?.GetString("error"), Does.Contain("boom"));
        }

        [Test]
        public void LoadedHtml_IsWhatTheCallerPassed()
        {
            var strHtml = "<html><head>" + PageScripts.ScriptBlock(PageScripts.Bridge) + "</head></html>";
            _host.LoadHtml(strHtml);
            Assert.That(_host.LoadedHtml, Is.EqualTo(strHtml));
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
