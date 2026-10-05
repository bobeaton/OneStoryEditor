using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class ModernStoryBtPageTests
    {
        // standards-mode layout checks (also used by ModernConNotePageTests)
        internal const string LayoutDriver =
            "ose.on('layout', function (m) { var t = document.getElementById(m.id), td = t.parentNode;" +
            "  while (td && td.nodeName !== 'TD') td = td.parentNode;" +
            "  return { fontSize: window.getComputedStyle(t).fontSize, className: t.className, mode: document.compatMode," +
            "    clientHeight: t.clientHeight, scrollHeight: t.scrollHeight, offsetWidth: t.offsetWidth," +
            "    cellWidth: td ? td.clientWidth : -1, cellPadding: td ? parseFloat(window.getComputedStyle(td).paddingLeft) + parseFloat(window.getComputedStyle(td).paddingRight) : 0 }; });" +
            "ose.on('resizeTo', function (m) { var t = document.getElementById(m.id); t.style.width = m.width; window.dispatchEvent(new Event('resize')); });";

        // the font size (in CSS px, as getComputedStyle gives it) a project language's style asks for
        internal static double ExpectedPx(ProjectSettings.LanguageInfo li) => li.FontToUse.SizeInPoints * 96.0 / 72.0;

        internal static double Px(string strCssPx) =>
            double.Parse(strCssPx.Replace("px", ""), System.Globalization.CultureInfo.InvariantCulture);

        private const string CstrDriver =
            // a user selecting characters [start, end) in a box and leaving it
            "ose.on('selectIn', function (m) { var t = document.getElementById(m.id); t.focus(); t.setSelectionRange(m.start, m.end);" +
            "  t.dispatchEvent(new MouseEvent('mouseup', { bubbles: true, button: 0 })); t.blur(); });" +
            "ose.on('setValue', function (m) { var t = document.getElementById(m.id); t.value = m.text; });" +
            "ose.on('typeIn', function (m) { var t = document.getElementById(m.id); t.focus(); t.value = t.value + m.text;" +
            "  t.dispatchEvent(new Event('input', { bubbles: true })); t.blur(); });" +
            "ose.on('leftClick', function (m) { var t = document.getElementById(m.id);" +
            "  t.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, button: 0 })); t.setSelectionRange(0, 0);" +
            "  t.dispatchEvent(new MouseEvent('mouseup', { bubbles: true, button: 0 })); });" +
            "ose.on('mirror', function (m) { var t = document.getElementById(m.id), mi = t.previousElementSibling;" +
            "  if (!mi || mi.className !== 'oseMirror') return { has: false };" +
            "  var a = t.getBoundingClientRect(), b = mi.getBoundingClientRect(), mk = mi.querySelector('mark');" +
            "  return { has: true, dx: Math.abs(a.left - b.left), dy: Math.abs(a.top - b.top), dw: Math.abs(a.width - b.width)," +
            "    dh: Math.abs(a.height - b.height), text: mi.textContent, mark: mk ? mk.textContent : null," +
            "    transparent: window.getComputedStyle(t).backgroundColor, mirrorBg: window.getComputedStyle(mi).backgroundColor," +
            "    mirrorVisibility: window.getComputedStyle(mi).visibility," +
            "    markVisibility: mk ? window.getComputedStyle(mk).visibility : null }; });" +
            "ose.on('rightClick', function (m) { var t = document.getElementById(m.id);" +
            "  t.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, button: 2 }));" +
            "  t.dispatchEvent(new MouseEvent('mouseup', { bubbles: true, button: 2 })); });" +
            "ose.on('focusBox', function (m) { document.getElementById(m.id).focus(); });" +
            "ose.on('blurBox', function (m) { document.getElementById(m.id).blur(); });" +
            "ose.on('tint', function (m) { document.getElementById(m.id).style.backgroundColor = 'rgb(240, 255, 255)'; });" +
            // make a box short and narrow (so long text wraps and it gets a vertical scrollbar), in a given direction
            "function oseScrollbar(el) { var cs = window.getComputedStyle(el);" +
            "  return el.offsetWidth - el.clientWidth - parseFloat(cs.borderLeftWidth) - parseFloat(cs.borderRightWidth); }" +
            // (boxes normally grow to fit and never scroll; this one is made to scroll)
            "ose.on('squeeze', function (m) { var t = document.getElementById(m.id); t.style.width = '120px'; t.style.height = '40px';" +
            "  t.style.overflow = 'auto'; t.style.direction = m.dir; return { scrollbar: oseScrollbar(t) }; });" +
            // a box's layout: its computed font size, whether its text all shows, and whether it fits its cell
            LayoutDriver +
            // how the mirror's text wraps compared to the box's: the width the text has, and the height it takes up
            "ose.on('wrap', function (m) { var t = document.getElementById(m.id), mi = t.previousElementSibling;" +
            "  function inner(el) { var cs = window.getComputedStyle(el); return el.clientWidth - parseFloat(cs.paddingLeft) - parseFloat(cs.paddingRight); }" +
            "  function left(el) { var r = el.getBoundingClientRect(), cs = window.getComputedStyle(el);" +
            "    return r.left + parseFloat(cs.borderLeftWidth) + parseFloat(cs.paddingLeft) + (cs.direction === 'rtl' ? el.offsetWidth - el.clientWidth - parseFloat(cs.borderLeftWidth) - parseFloat(cs.borderRightWidth) : 0); }" +
            "  return { boxWidth: inner(t), mirrorWidth: inner(mi), boxLeft: left(t), mirrorLeft: left(mi)," +
            "    boxHeight: t.scrollHeight, mirrorHeight: mi.scrollHeight, scrollbar: oseScrollbar(t) }; });" +
            "ose.on('box', function (m) { var t = document.getElementById(m.id); return { placeholder: t.placeholder, readOnly: t.readOnly }; });" +
            "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
            "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes;" +
            "    for (var j = 0; j < a.length; j++) if (/^on/i.test(a[j].name)) n++; } return { n: n }; });";

        private Form _form;
        private WebView2HtmlHost _host;
        private string _strHtml;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateWebView2HostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);
            _strHtml = PaneTestData.StoryBtPage(HtmlEngine.WebView2);
            _host.LoadHtml(_strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady, 20000), Is.True, "page never sent 'ready'");
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        private HtmlMessage Ask(string strType, object payload = null) => _host.Request(strType, payload, HtmlHostDefaults.RequestTimeout);

        private List<HighlightedText> Highlights(int nLine) =>
            HighlightedText.FromReply(Ask("getHighlights", new { tableId = VerseData.GetLineTableId(nLine) }));

        private string StoryLineBox(int nLine, int nNth = 0)
        {
            var matches = Regex.Matches(_strHtml, "id=\"(ta_" + nLine + "_StoryLine_0_0_[A-Za-z]+)\"");
            Assert.That(matches.Count, Is.GreaterThan(nNth), "not enough story line boxes on line " + nLine);
            return matches[nNth].Groups[1].Value;
        }

        [Test]
        public void Page_LoadsWithoutScriptErrors_AndHasNoInlineHandlers()
        {
            BrowserTestHelper.Pump(300);
            Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
            Assert.That(Ask("countInline").TryGetInt("n", out var n) && n == 0, Is.True);
        }

        [Test]
        public void Selection_BecomesAHighlight_WithIeShapedItems_AndClears()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
            _host.Post("selectIn", new { id = strId, start = 0, end = 3 });
            BrowserTestHelper.Pump(200);
            var items = Highlights(1);
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].TextareaId, Is.EqualTo(strId));
            Assert.That(items[0].Text, Is.EqualTo("abc"));
            Assert.That(items[0].ClassName, Does.EndWith(" highlight"));
            Assert.That(ReferringTextBuilder.TryBuild(items, out var strReferring) && strReferring.Contains(">abc</span>"), Is.True);

            _host.Post("clearHighlight", new { id = strId });
            Assert.That(Highlights(1), Is.Empty);
        }

        [Test]
        public void HighlightsInTwoBoxes_AreBothKept()
        {
            var strA = StoryLineBox(1, 0);
            var strB = StoryLineBox(1, 1);
            _host.Post("setValue", new { id = strA, text = "first box" });
            _host.Post("setValue", new { id = strB, text = "second box" });
            _host.Post("selectIn", new { id = strA, start = 0, end = 5 });
            _host.Post("selectIn", new { id = strB, start = 7, end = 10 });
            BrowserTestHelper.Pump(200);
            Assert.That(Highlights(1).Select(h => h.Text), Is.EquivalentTo(new[] { "first", "box" }));
        }

        [Test]
        public void Edit_ClearsTheBoxHighlight()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef" });
            _host.Post("selectIn", new { id = strId, start = 1, end = 4 });
            _host.Post("typeIn", new { id = strId, text = "X" });
            BrowserTestHelper.Pump(200);
            Assert.That(Highlights(1), Is.Empty, "a box's highlight must not survive an edit of that box");
        }

        // typing is how C# learns the project is modified (the Save menu item follows Modified)
        [Test]
        public void Typing_SendsTextChanged_WithTheBoxValue()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abc" });
            _host.Post("typeIn", new { id = strId, text = "X" });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Any(m => m.Type == "textChanged"), 3000), Is.True,
                "no textChanged; got: " + string.Join(", ", _received.Select(m => m.Type)));
            var msg = _received.Last(m => m.Type == "textChanged");
            Assert.That(msg.GetString("id"), Is.EqualTo(strId));
            Assert.That(msg.GetString("value"), Is.EqualTo("abcX"));
        }

        [Test]
        public void LeftClick_ClearsTheBoxHighlight()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef" });
            _host.Post("selectIn", new { id = strId, start = 1, end = 4 });
            _host.Post("leftClick", new { id = strId });
            BrowserTestHelper.Pump(200);
            Assert.That(Highlights(1), Is.Empty);
        }

        [Test]
        public void Mirror_LinesUpWithTheBox_AndMarksTheRange()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
            _host.Post("selectIn", new { id = strId, start = 2, end = 5 });
            BrowserTestHelper.Pump(200);
            var m = Ask("mirror", new { id = strId });
            Assert.That(m.GetBool("has"), Is.True, "a highlighted box gets a mirror");
            foreach (var strDelta in new[] { "dx", "dy", "dw", "dh" })
                Assert.That(m.Body[strDelta].ToObject<double>(), Is.LessThan(1.0), strDelta);
            Assert.That(m.GetString("text"), Is.EqualTo("abcdef ghi\n"));
            Assert.That(m.GetString("mark"), Is.EqualTo("cde"));
            Assert.That(m.GetString("transparent"), Is.EqualTo("rgba(0, 0, 0, 0)"));

            _host.Post("clearHighlight", new { id = strId });
            BrowserTestHelper.Pump(100);
            Assert.That(Ask("mirror", new { id = strId }).GetBool("has"), Is.False, "the mirror goes when the highlight does");
        }

        // the spike's finding: the mirror must leave room for the box's vertical scrollbar (on the left when rtl), or its
        //  text wraps at a different place from the box's and the highlight drifts off the selected words
        [TestCase("ltr")]
        [TestCase("rtl")]
        public void Mirror_WrapsLikeTheBox_WhenTheBoxHasAScrollbar(string strDir)
        {
            var strId = StoryLineBox(1);
            var r = Ask("squeeze", new { id = strId, dir = strDir });
            Assert.That(r.Body["scrollbar"].ToObject<double>(), Is.EqualTo(0.0), "no scrollbar before the text is long");
            var strText = string.Join(" ", Enumerable.Repeat("several words that wrap", 12));
            _host.Post("setValue", new { id = strId, text = strText });
            _host.Post("selectIn", new { id = strId, start = 100, end = 120 });
            BrowserTestHelper.Pump(200);

            var w = Ask("wrap", new { id = strId });
            Assert.That(w.Body["boxHeight"].ToObject<double>(), Is.GreaterThan(80.0), "the text must overflow the box");
            Assert.That(w.Body["scrollbar"].ToObject<double>(), Is.GreaterThan(0.0), "the box must now have a vertical scrollbar");
            Assert.That(w.Body["mirrorWidth"].ToObject<double>(), Is.EqualTo(w.Body["boxWidth"].ToObject<double>()).Within(0.5),
                        "the text gets the same width in the mirror as in the box");
            Assert.That(w.Body["mirrorLeft"].ToObject<double>(), Is.EqualTo(w.Body["boxLeft"].ToObject<double>()).Within(1.0),
                        "the text starts at the same place (the scrollbar is on the " + (strDir == "rtl" ? "left" : "right") + ")");
            Assert.That(w.Body["mirrorHeight"].ToObject<double>(), Is.EqualTo(w.Body["boxHeight"].ToObject<double>()).Within(1.0),
                        "the text wraps onto as many lines in the mirror as in the box");
        }

        // a highlight made in a box, then the box's value changed by a C# command (no input event)
        private void HighlightThen(string strCommand, object payload)
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
            _host.Post("selectIn", new { id = strId, start = 2, end = 5 });
            BrowserTestHelper.Pump(200);
            Assert.That(Ask("mirror", new { id = strId }).GetBool("has"), Is.True, "the selection must first become a highlight");
            if (strCommand == "replaceSelection")
                Assert.That(Ask(strCommand, payload).TryGetInt("endPoint", out var n) && (n == 4), Is.True, "replaceSelection reply");
            else
                _host.Post(strCommand, payload);
            BrowserTestHelper.Pump(100);
        }

        [TestCase("setText")]
        [TestCase("appendText")]
        [TestCase("replaceSelection")]
        [TestCase("setValue")]      // (a value change no command knows about: the stale-value guard)
        public void ScriptValueChange_DropsTheBoxHighlight_AndItsMirror(string strCommand)
        {
            var strId = StoryLineBox(1);
            HighlightThen(strCommand, new { id = strId, text = "XY" });
            Assert.That(Highlights(1), Is.Empty, "a highlight must not outlive a change to its box's value");
            Assert.That(Ask("mirror", new { id = strId }).GetBool("has"), Is.False, "nor may its mirror");
        }

        [Test]
        public void FocusedHighlightedBox_KeepsItsBackground_AndHidesOnlyTheMark()
        {
            var strId = StoryLineBox(1);
            _host.Post("tint", new { id = strId });     // e.g. a read-only (Azure) box
            _host.Post("setValue", new { id = strId, text = "abcdef ghi" });
            _host.Post("selectIn", new { id = strId, start = 2, end = 5 });
            BrowserTestHelper.Pump(200);
            var m = Ask("mirror", new { id = strId });
            Assert.That(m.GetString("markVisibility"), Is.EqualTo("visible"));

            _host.Post("focusBox", new { id = strId });
            BrowserTestHelper.Pump(100);
            m = Ask("mirror", new { id = strId });
            Assert.That(m.GetBool("has"), Is.True);
            Assert.That(m.GetString("mirrorVisibility"), Is.EqualTo("visible"), "the mirror (the box's background) stays while it has focus");
            Assert.That(m.GetString("mirrorBg"), Is.EqualTo("rgb(240, 255, 255)"), "the box's own background shows while it has focus");
            Assert.That(m.GetString("markVisibility"), Is.EqualTo("hidden"), "the browser shows its own selection while the box has focus");

            _host.Post("blurBox", new { id = strId });
            BrowserTestHelper.Pump(100);
            Assert.That(Ask("mirror", new { id = strId }).GetString("markVisibility"), Is.EqualTo("visible"));
        }

        [Test]
        public void LeftMouseDown_ReportsIeButtonValue()
        {
            var strId = StoryLineBox(1);
            _host.Post("leftClick", new { id = strId });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "textareaMouseDown"), 3000), Is.True);
            var msg = _received.First(m => m.Type == "textareaMouseDown");
            Assert.That(msg.GetString("id"), Is.EqualTo(strId));
            Assert.That(msg.TryGetInt("button", out var nButton) && (nButton == 1), Is.True, "button " + nButton);
        }

        [Test]
        public void RightClick_KeepsTheHighlight_AndAsksForTheContextMenu()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = "abcdef" });
            _host.Post("selectIn", new { id = strId, start = 1, end = 4 });
            _host.Post("rightClick", new { id = strId });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "contextMenu"), 3000), Is.True);
            Assert.That(_received.First(m => m.Type == "contextMenu").GetString("id"), Is.EqualTo(strId));
            Assert.That(_received.First(m => m.Type == "textareaMouseDown").TryGetInt("button", out var nButton) && (nButton == 2), Is.True);
            Assert.That(Highlights(1).Select(h => h.Text), Is.EqualTo(new[] { "bcd" }));
        }

        internal static ProjectSettings.LanguageInfo LanguageOf(ProjectSettings ps, string strClassName)
        {
            // (the boxes say "LangNationalBt", the style constants "LangNationalBT")
            var astrClasses = strClassName.Split(' ').Select(s => s.ToLowerInvariant()).ToArray();
            bool Has(string strClass) => astrClasses.Contains(strClass.ToLowerInvariant());
            if (Has(StoryData.CstrLangVernacularStyleClassName)) return ps.Vernacular;
            if (Has(StoryData.CstrLangNationalBtStyleClassName)) return ps.NationalBT;
            if (Has(StoryData.CstrLangInternationalBtStyleClassName)) return ps.InternationalBT;
            if (Has(StoryData.CstrLangFreeTranslationStyleClassName)) return ps.FreeTranslation;
            Assert.Fail("no language class in '" + strClassName + "'");
            return null;
        }


        // standards mode ignored "font-size: 12 pt" (the space), so every box showed Chromium's 13.33px default
        [Test]
        public void StoryBox_HasTheProjectsFontSize()
        {
            var project = PaneTestData.LoadProject();
            var nNotDefault = 0;
            for (var nNth = 0; nNth < 4; nNth++)
            {
                var r = Ask("layout", new { id = StoryLineBox(1, nNth) });
                Assert.That(r.GetString("mode"), Is.EqualTo("CSS1Compat"));
                var li = LanguageOf(project.ProjSettings, r.GetString("className"));
                if (System.Math.Abs(ExpectedPx(li) - 13.333) > 0.01)
                    nNotDefault++;
                Assert.That(Px(r.GetString("fontSize")), Is.EqualTo(ExpectedPx(li)).Within(0.1),
                            r.GetString("className") + " should be " + li.FontToUse.SizeInPoints + "pt");
            }
            Assert.That(nNotDefault, Is.GreaterThan(0), "(the check needs a size that isn't Chromium's default 13.33px)");
        }

        // IE grew a box to fit its text; standards mode doesn't, so lines were hidden
        [Test]
        public void StoryBox_GrowsToShowAllItsLines_AndShrinksAgain()
        {
            var strId = StoryLineBox(1);
            var nOneLine = Ask("layout", new { id = strId }).Body["clientHeight"].ToObject<double>();
            _host.Post("setText", new { id = strId, text = "one\ntwo\nthree\nfour\nfive\nsix" });
            BrowserTestHelper.Pump(200);
            var r = Ask("layout", new { id = strId });
            Assert.That(r.Body["clientHeight"].ToObject<double>(), Is.GreaterThanOrEqualTo(r.Body["scrollHeight"].ToObject<double>()),
                        "no hidden lines");
            Assert.That(r.Body["clientHeight"].ToObject<double>(), Is.GreaterThan(nOneLine * 2), "the box grew");

            _host.Post("setText", new { id = strId, text = "one" });
            BrowserTestHelper.Pump(200);
            Assert.That(Ask("layout", new { id = strId }).Body["clientHeight"].ToObject<double>(), Is.LessThan(nOneLine * 2), "and shrank");
        }

        [Test]
        public void TypedLines_GrowTheBox()
        {
            var strId = StoryLineBox(1);
            _host.Post("typeIn", new { id = strId, text = "\na\nb\nc\nd\ne\nf" });
            BrowserTestHelper.Pump(200);
            var r = Ask("layout", new { id = strId });
            Assert.That(r.Body["clientHeight"].ToObject<double>(), Is.GreaterThanOrEqualTo(r.Body["scrollHeight"].ToObject<double>()));
        }

        // width:100% plus the border and padding overflowed the cell (content-box)
        [Test]
        public void StoryBoxes_FitTheirCells()
        {
            for (var nNth = 0; nNth < 4; nNth++)
            {
                var r = Ask("layout", new { id = StoryLineBox(1, nNth) });
                Assert.That(r.Body["cellWidth"].ToObject<double>(), Is.GreaterThan(0));
                Assert.That(r.Body["offsetWidth"].ToObject<double>(),
                            Is.LessThanOrEqualTo(r.Body["cellWidth"].ToObject<double>() - r.Body["cellPadding"].ToObject<double>() + 0.5));
            }
        }

        // a box that grows (window resize, new text) moves the mirror with it
        [Test]
        public void Mirror_FollowsTheBox_WhenItIsResized()
        {
            var strId = StoryLineBox(1);
            _host.Post("setValue", new { id = strId, text = string.Join(" ", Enumerable.Repeat("words that wrap", 10)) });
            _host.Post("selectIn", new { id = strId, start = 2, end = 5 });
            BrowserTestHelper.Pump(200);
            _host.Post("resizeTo", new { id = strId, width = "150px" });
            BrowserTestHelper.Pump(300);
            var r = Ask("layout", new { id = strId });
            Assert.That(r.Body["clientHeight"].ToObject<double>(), Is.GreaterThanOrEqualTo(r.Body["scrollHeight"].ToObject<double>()));
            var m = Ask("mirror", new { id = strId });
            Assert.That(m.GetBool("has"), Is.True);
            foreach (var strDelta in new[] { "dx", "dy", "dw", "dh" })
                Assert.That(m.Body[strDelta].ToObject<double>(), Is.LessThan(1.0), strDelta);
        }

        [Test]
        public void Boxes_UseNativePlaceholders()
        {
            var r = Ask("box", new { id = StoryLineBox(1) });
            Assert.That(r.GetString("placeholder"), Is.Not.Empty, "the language name is the native placeholder");
        }

        [Test]
        public void SetText_ThenFlush_RoundTripsAwkwardText()
        {
            var strId = StoryLineBox(1);
            var strText = "say \"hi\" <donkey bray> & more\nnext line " + HtmlMessageTests.AwkwardNonAscii;
            _host.Post("setText", new { id = strId, text = strText });
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "textChanged")), Is.True);
            Assert.That(_received.First(m => m.Type == "textChanged").GetString("value"), Is.EqualTo(strText));
        }
    }
}
