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
        // just enough of a bridge: ready on load; echo / sendBack / bodyText / go / back / reload handlers
        private const string CstrMiniBridge =
            "var docId='" + IeHtmlHost.CstrDocIdToken + "';" +
            "function send(t,p){p=p||{};p.type=t;p.doc=docId;chrome.webview.postMessage(JSON.stringify(p));}" +
            "chrome.webview.addEventListener('message',function(e){var m=JSON.parse(e.data),r={};" +
            " if(m.type==='echo')r={text:m.text};" +
            " else if(m.type==='sendBack')send('pong',{n:m.n});" +
            " else if(m.type==='bodyText')r={text:document.body.innerText};" +
            " else if(m.type==='go')location.href=m.url;" +
            " else if(m.type==='back')history.back();" +
            " else if(m.type==='reload')location.reload();" +
            " else if(m.type==='fetch')fetch(m.url).then(function(x){send('fetched',{url:m.url,status:x.status});}," +
            "  function(){send('fetched',{url:m.url,status:-1});});" +
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
        public void Request_FromInsideAPageMessageHandler_GetsItsReply()
        {
            // the spike found WebView2 raises no WebMessageReceived while one's handler is running, so the host must
            //  hand messages on outside that handler (BeginInvoke), or this Request would always time out
            Load("hi");
            HtmlMessage reply = null;
            var bHandled = false;
            _host.MessageReceived += (s, m) =>
            {
                if (m.Type != "pong")
                    return;
                reply = _host.Request("echo", new { text = "nested" }, HtmlHostDefaults.RequestTimeout);
                bHandled = true;
            };
            _host.Post("sendBack", new { n = 1 });
            Assert.That(BrowserTestHelper.PumpUntil(() => bHandled), Is.True, "the page's message never arrived");
            Assert.That(reply?.GetString("text"), Is.EqualTo("nested"));
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
        public void Reload_IsCancelled()
        {
            // a reload asks for the very URL the host loaded last, so the guard must allow each URL only once
            Load("only");
            var nReady = 0;
            _host.DocumentReady += (s, e) => nReady++;
            _host.Post("reload");
            BrowserTestHelper.Pump(1500);
            Assert.That(nReady, Is.EqualTo(0), "a reload would send 'ready' again");
            Assert.That(_host.Request("bodyText", null, HtmlHostDefaults.RequestTimeout)?.GetString("text")?.Trim(),
                        Is.EqualTo("only"));
        }

        [Test]
        public void LoadedHtml_IsWhatTheCallerPassed()
        {
            var strHtml = BrowserTestHelper.ModernPage("x", CstrMiniBridge);
            _host.LoadHtml(strHtml);
            Assert.That(_host.LoadedHtml, Is.EqualTo(strHtml));
        }

        [Test]
        public void SupersededDocument_IsNotServed()
        {
            // a navigation a later LoadHtml superseded must never be given the newer document: only ?d=<current> is served
            Load("first");
            var nOld = int.Parse(_host.Request("echo", new { text = "" }, HtmlHostDefaults.RequestTimeout).DocId);   // (the mini bridge stamps its doc id)
            Load("second");
            Assert.That(FetchStatus("https://ose.pane/page.html?d=" + nOld), Is.EqualTo(404));
            Assert.That(FetchStatus("https://ose.pane/page.html?d=" + (nOld + 1)), Is.EqualTo(200));
            Assert.That(FetchStatus("https://ose.pane/favicon.ico"), Is.EqualTo(404));
        }

        private int FetchStatus(string strUrl)
        {
            _received.Clear();
            _host.Post("fetch", new { url = strUrl });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "fetched")), Is.True, "no answer for " + strUrl);
            Assert.That(_received.Find(m => m.Type == "fetched").TryGetInt("status", out var nStatus), Is.True);
            return nStatus;
        }

        [Test]
        public void LoadHtmlTwice_BeforeStarted_TheSecondPageBecomesReady()
        {
            _host.LoadHtml(BrowserTestHelper.ModernPage("first", CstrMiniBridge));
            _host.LoadHtml(BrowserTestHelper.ModernPage("second", CstrMiniBridge));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True, "page never sent 'ready'");
            Assert.That(_host.Request("echo", new { text = "ok" }, HtmlHostDefaults.RequestTimeout)?.GetString("text"), Is.EqualTo("ok"));
            Assert.That(_host.Request("bodyText", null, HtmlHostDefaults.RequestTimeout)?.GetString("text")?.Trim(),
                        Is.EqualTo("second"));
        }

        [Test]
        public void Dispose_DuringStartUp_DoesNotThrow_OrRaiseFailed()
        {
            Exception failure = null;
            _host.Failed += (s, ex) => failure = ex;
            _host.Dispose();    // (the asynchronous start-up began when the form was shown)
            Assert.DoesNotThrow(() => BrowserTestHelper.Pump(2000));
            Assert.That(failure, Is.Null);
        }

        [Test]
        public void ControlDisposedFirst_RequestReturnsAtOnce_AndPostDoesNotThrow()
        {
            // WinForms disposes the control when its form closes, possibly before the owner disposes the host
            Load("hi");
            _host.Control.Dispose();
            var sw = Stopwatch.StartNew();
            Assert.That(_host.Request("echo", new { text = "x" }, TimeSpan.FromSeconds(5)), Is.Null);
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1000));
            Assert.DoesNotThrow(() => _host.Post("sendBack", new { n = 1 }));
            Assert.DoesNotThrow(() => _host.ShowPrintPreview());
        }

        [Test]
        public void UnusableUserDataFolder_RaisesFailed()
        {
            _form.Close();
            var strFile = Path.Combine(Path.GetTempPath(), "OseWv2NotAFolder.txt");
            File.WriteAllText(strFile, "x");
            WebView2HtmlHost host = null;
            try
            {
                Exception failure = null;
                host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form, strFile);
                host.Failed += (s, ex) => failure = ex;
                host.LoadHtml(BrowserTestHelper.ModernPage("x", CstrMiniBridge));
                Assert.That(BrowserTestHelper.PumpUntil(() => failure != null, (int)WebView2HtmlHost.StartTimeout.TotalMilliseconds + 5000), Is.True,
                            "a host that can't start must say so (exception or watchdog)");
                TestContext.WriteLine("failure: " + failure.GetType().Name + ": " + failure.Message);
            }
            finally
            {
                host?.Dispose();
                File.Delete(strFile);
            }
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
