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
