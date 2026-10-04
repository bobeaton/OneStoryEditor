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
            "ose.on('injectBox', function () { var t = document.createElement('textarea'); t.id = 'injected_box'; t.setAttribute('data-note', 'edit');" +
            "  document.body.appendChild(t); var p = document.createElement('p'); p.id = 'tp_7_0_0'; p.textContent = 'some note words'; document.body.appendChild(p); return { id: t.id }; });" +
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

        // the test project has no notes, so these exercise the same handlers on injected elements
        [Test]
        public void InjectedNoteBox_CtrlB_MouseDown_AndParagraphSelection_Work()
        {
            var strId = Ask("injectBox").GetString("id");
            Assert.That(Ask("ctrlB", new { id = strId }).GetString("value"), Is.EqualTo("say $hello$  now"));
            Assert.That(WaitFor("textChanged")?.GetString("value"), Is.EqualTo("say $hello$  now"));
            BrowserTestHelper.Pump(300);
            Assert.That(_received.Count(m => m.Type == "textChanged"), Is.EqualTo(1), "one message per edit");
            _host.Post("leftDown", new { id = strId });
            Assert.That(WaitFor("textareaMouseDown").TryGetInt("button", out var n) && n == 1, Is.True);
            Assert.That(Ask("selectFirstNoteParagraph").GetString("id"), Is.EqualTo("tp_7_0_0"));
            var reply = Ask("getNoteSelection");
            Assert.That(reply.TryGetInt("lineIndex", out var nLine) && nLine == 7, Is.True);
            Assert.That(reply.GetString("html"), Is.EqualTo("some note words"));
        }

        [Test]
        public void RightClick_AsksForTheContextMenu()
        {
            _host.Post("rightClick");
            Assert.That(WaitFor("contextMenu"), Is.Not.Null);
        }
    }
}
