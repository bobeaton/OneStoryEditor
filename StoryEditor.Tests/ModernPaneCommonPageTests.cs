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
