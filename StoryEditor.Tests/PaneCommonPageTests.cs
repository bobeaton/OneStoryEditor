using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    // in both document modes: standards (IE9) and quirks (no doctype, as the real pane pages are)
    [TestFixture(false), TestFixture(true), Apartment(ApartmentState.STA), Category("Browser")]
    public class PaneCommonPageTests
    {
        private const string CstrDriver =
            BrowserTestHelper.CstrFireEventScript +
            "ose.on('click', function (m) { document.getElementById(m.id).click(); });" +
            "ose.on('focusOn', function (m) { document.getElementById(m.id).focus(); });" +
            "ose.on('typeInto', function (m) { var ta = document.getElementById(m.id); ta.value = m.text; });" +
            "ose.on('innerHtml', function (m) { return { html: document.getElementById(m.id).innerHTML }; });" +
            "ose.on('fire', function (m) { oseFire(document.getElementById(m.id), m.what); });" +
            "ose.on('mode', function () { return { mode: document.documentMode }; });" +
            "ose.on('echo', function (m) { return { text: m.text }; });";

        private readonly bool _bQuirks;

        public PaneCommonPageTests(bool bQuirks)
        {
            _bQuirks = bQuirks;
        }

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

            var astrScripts = new[] { PageScripts.Bridge, PageScripts.Get("PaneCommon.js"), CstrDriver };
            _host.LoadHtml(_bQuirks ? BrowserTestHelper.QuirksPage(sb.ToString(), astrScripts) : BrowserTestHelper.Page(sb.ToString(), astrScripts));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True);
        }

        [Test]
        public void Page_RunsInTheModeUnderTest()
        {
            var reply = _host.Request("mode", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("mode", out var nMode), Is.True);
            Assert.That(_bQuirks ? (nMode == 5) : (nMode >= 9), Is.True, "documentMode " + nMode);
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
            if (_bQuirks)
                Assert.Ignore("in quirks mode a drop made with fireEvent doesn't bubble to the document listener (IE documents real drag events as bubbling); covered by the manual checklist");
            _host.Post("fire", new { id = "anc_2", what = "drop" });
            Assert.That(WaitFor("scriptureDropped")?.GetString("id"), Is.EqualTo("anc_2"),
                        string.Join("; ", _received.FindAll(m => m.Type == HtmlMessage.CstrTypeJsError).ConvertAll(m => m.GetString("message"))));
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
