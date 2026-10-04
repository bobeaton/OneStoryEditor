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
            if (_form == null)
                return;
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
