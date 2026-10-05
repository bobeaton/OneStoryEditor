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
            "ose.on('rightClick', function () { document.body.dispatchEvent(new MouseEvent('contextmenu', { bubbles: true, cancelable: true, button: 2 })); });" +
            // a note box in a 292px cell, as the notes table has them
            "ose.on('injectCellBox', function () { var tb = document.createElement('table'); tb.innerHTML = '<tr><td style=\"width:292px\">" +
            "<textarea id=\"cell_box\" data-note=\"edit\" class=\"" + StoryData.CstrLangVernacularStyleClassName + "\"></textarea></td></tr>';" +
            "  document.body.appendChild(tb); return { id: 'cell_box' }; });" +
            ModernStoryBtPageTests.LayoutDriver;

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

        // the standards-mode layout faults: font size ignored ("10 pt"), the box not growing, the box overflowing its cell
        [Test]
        public void NoteBox_HasTheProjectFont_ShowsAllItsLines_AndFitsItsCell()
        {
            var strId = Ask("injectCellBox").GetString("id");
            _host.Post("setText", new { id = strId, text = "line one\nline two\nline three\nline four\nline five" });
            BrowserTestHelper.Pump(200);
            var r = Ask("layout", new { id = strId });
            Assert.That(r.GetString("mode"), Is.EqualTo("CSS1Compat"));
            var project = PaneTestData.LoadProject();
            var dExpected = ModernStoryBtPageTests.ExpectedPx(project.ProjSettings.Vernacular);
            Assert.That(dExpected, Is.Not.EqualTo(13.333).Within(0.01), "(the check needs a size that isn't Chromium's default)");
            Assert.That(ModernStoryBtPageTests.Px(r.GetString("fontSize")), Is.EqualTo(dExpected).Within(0.1),
                        "a box with the vernacular class gets the vernacular style's size");
            Assert.That(r.Body["clientHeight"].ToObject<double>(), Is.GreaterThanOrEqualTo(r.Body["scrollHeight"].ToObject<double>()), "no hidden lines");
            Assert.That(r.Body["offsetWidth"].ToObject<double>(),
                        Is.LessThanOrEqualTo(r.Body["cellWidth"].ToObject<double>() - r.Body["cellPadding"].ToObject<double>() + 0.5), "fits its cell");
        }

        [Test]
        public void RightClick_AsksForTheContextMenu()
        {
            _host.Post("rightClick");
            Assert.That(WaitFor("contextMenu"), Is.Not.Null);
        }
    }
}
