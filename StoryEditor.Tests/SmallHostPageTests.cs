using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace OneStoryProjectEditor.Tests
{
    // in both document modes: standards (IE9) and quirks (no doctype, as the real NetBibleViewer and HtmlForm pages are)
    [TestFixture(false), TestFixture(true), Apartment(ApartmentState.STA), Category("Browser")]
    public class SmallHostPageTests
    {
        // test-only driver: simulates the user's mouse on the first link/button
        private const string CstrDriver =
            "function oseFire(el, type) { if (!document.createEvent) { el.fireEvent('on' + type); return; }" +
            "  var ev = document.createEvent('MouseEvents');" +
            "  ev.initMouseEvent(type, true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
            "  el.dispatchEvent(ev); }" +
            "ose.on('clickLink', function () { document.getElementsByTagName('a')[0].click(); });" +
            "ose.on('mouseButton', function (m) { oseFire(document.getElementsByTagName('button')[0], m.what); });" +
            "ose.on('echo', function (m) { return { text: m.text }; });";

        private readonly bool _bQuirks;

        public SmallHostPageTests(bool bQuirks)
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
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        private void Load(string strBody, string strPageScript)
        {
            var astrScripts = new[] { PageScripts.Bridge, PageScripts.Get(strPageScript), CstrDriver };
            _host.LoadHtml(_bQuirks ? BrowserTestHelper.QuirksPage(strBody, astrScripts) : BrowserTestHelper.Page(strBody, astrScripts));
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
            // Sword's footnote links look like this; "sword:" is the six characters the page strips
            Load("<a href=\"sword:passagestudy.jsp?action=showNote&amp;type=n&amp;value=1&amp;module=NET&amp;passage=Gen+1%3A1\">1</a>", "NetBible.js");
            _host.Post("clickLink");
            Assert.That(WaitFor("hoverRef")?.GetString("ref"),
                        Is.EqualTo("passagestudy.jsp?action=showNote&type=n&value=1&module=NET&passage=Gen+1%3A1"));
        }

        [TestCase("mousedown", "refMouseDown")]
        [TestCase("mouseup", "refMouseUp")]
        [TestCase("mouseout", "refMouseOut")]
        public void NetBible_ButtonMouse_SendsMessage(string strDomEvent, string strMessage)
        {
            // a real verse button as DisplayVerses builds it (CstrHtmlButtonCell): id "Gen 1:2", the localized label as
            //  its content, and no value attribute
            Load("<table><tr><td dir='ltr'><button id='Gen 1:2' type=\"button\">Genesis 1:2</button></td></tr></table>", "NetBible.js");
            _host.Post("mouseButton", new { what = strDomEvent });
            var msg = WaitFor(strMessage);
            Assert.That(msg, Is.Not.Null);
            if (strMessage != "refMouseDown")
            {
                Assert.That(msg.GetString("target"), Is.EqualTo("Gen 1:2"));
                // the reference is getAttribute('value'), as it always was: in the real page's quirks mode that is the
                //  button's content (in IE9 mode it would be null, since the button has no value attribute)
                Assert.That(msg.GetString("ref"), _bQuirks ? (IResolveConstraint)Is.EqualTo("Genesis 1:2") : Is.Null);
            }
        }
    }
}
