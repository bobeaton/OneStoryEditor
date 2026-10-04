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

        // In an IE9-mode page document.selection.createRange() reports the whole textarea (even right after
        //  setSelectionRange(0, 3) or a TextRange.select() of 3 characters), so the page can't be driven to a partial
        //  selection here. See the Task 6 report.
        [Test, Explicit("IE won't select programmatically here; covered by the manual checklist")]
        public void Selection_BecomesAHighlight_ThatCanBeCleared()
        {
            var strId = FirstStoryLineTextareaId(1);
            _host.Post("focusOn", new { id = strId });  // like a user: focusing an empty box clears its language-name placeholder
            BrowserTestHelper.Pump(100);
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
            _host.Post("focusOn", new { id = strId });  // like a user: focusing an empty box clears its language-name placeholder
            BrowserTestHelper.Pump(100);
            _host.Post("setText", new { id = strId, text = strText });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "textChanged")), Is.True);
            // IE9 mode's textarea.value reports line breaks as \n, whatever we set; C# normalizes line endings itself
            Assert.That(StoryData.NormalizeLineEndings(_received.First(m => m.Type == "textChanged").GetString("value")),
                        Is.EqualTo(StoryData.NormalizeLineEndings(strText)));

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
