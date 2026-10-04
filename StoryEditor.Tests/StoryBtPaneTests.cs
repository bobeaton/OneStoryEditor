using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class StoryBtPaneTests
    {
        private FakeHtmlHost _host;
        private HtmlStoryBtControl _pane;

        [SetUp]
        public void SetUp()
        {
            _host = new FakeHtmlHost();
            _pane = new HtmlStoryBtControl(_host);
        }

        [TearDown]
        public void TearDown()
        {
            _pane.Dispose();
        }

        [Test]
        public void HandlesEveryMessageTheStoryBtPageSends()
        {
            var aExpected = new[]
            {
                "scrolled", "save", "reload", "realign", "bibRefJump", "openUrl", "verseLineJump", "textareaMouseDown",
                "log", "jsError", "textChanged", "focus", "blur", "textareaMouseUp", "contextMenu", "mouseMove",
                "scriptureDropped", "action"
            };
            Assert.That(PaneDispatcher(_pane).RegisteredTypes, Is.SupersetOf(aExpected));
        }

        [Test]
        public void GetSelectedTexts_AsksForTheLinesTable_AndReadsTheReply()
        {
            _host.OnRequest = m => FakeHtmlHost.Reply(new
            {
                items = new[] { new { textareaId = "ta_2_StoryLine_0_0_Vernacular", className = "LangVernacular highlight", text = "w" } }
            });
            var list = _pane.GetSelectedTexts(2);
            Assert.That(_host.Requests.Single().Type, Is.EqualTo("getHighlights"));
            Assert.That(_host.Requests.Single().GetString("tableId"), Is.EqualTo(VerseData.GetLineTableId(2)));
            Assert.That(list.Single().Text, Is.EqualTo("w"));
        }

        [Test]
        public void GetSelectedTexts_NoReply_GivesEmptyList()
        {
            _host.OnRequest = m => null;
            Assert.That(_pane.GetSelectedTexts(1), Is.Empty);
        }

        [Test]
        public void ScrollToElement_PostsScrollTo_FocusingOnlyWhenNotAligningTop()
        {
            _pane.ScrollToElement("ln_3", true);
            _pane.ScrollToElement("ta_1", false);
            Assert.That(_host.Posts.Select(p => p.GetBool("focus")), Is.EqualTo(new[] { false, true }));
            Assert.That(_host.Posts.All(p => p.Type == "scrollTo"), Is.True);
        }

        [Test]
        public void DocumentReady_ScrollsToRememberedId()
        {
            _pane.StrIdToScrollTo = "ln_7";
            _host.RaiseReady();
            Assert.That(_host.Posts.Single().GetString("id"), Is.EqualTo("ln_7"));
        }

        [Test]
        public void Scrolled_UpdatesLineLinkAndTopRows()
        {
            string strText = null;
            var nLine = -1;
            _pane.SetLineNumberLink = (s, n) => { strText = s; nLine = n; };
            _host.Raise("scrolled", new { topId = "anc_4", topLabel = VersesData.LinePrefix + "4", prevId = "ln_3", nextId = "ln_5" });
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "4"));
            Assert.That(nLine, Is.EqualTo(4));
        }

        [Test]
        public void SetSelectedText_UpdatesModelFromReply()
        {
            var st = new StringTransfer("old", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_StoryLine_0_0_Vernacular" };
            _host.OnRequest = m => FakeHtmlHost.Reply(new { endPoint = 5, ieHtml = "a &amp; b" });
            Assert.That(_pane.SetSelectedText(st, "x", out var nEnd), Is.True);
            Assert.That(nEnd, Is.EqualTo(5));
            Assert.That(st.ToString(), Is.EqualTo("a & b"));
        }

        [Test]
        public void SetSelectedText_FailedReply_LeavesModel()
        {
            var st = new StringTransfer("old", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_StoryLine_0_0_Vernacular" };
            _host.OnRequest = m => FakeHtmlHost.Reply(new { endPoint = 0 });
            Assert.That(_pane.SetSelectedText(st, "x", out var nEnd), Is.False);
            Assert.That(nEnd, Is.EqualTo(0));
            Assert.That(st.ToString(), Is.EqualTo("old"));
        }

        [Test]
        public void ClearSelection_TextareaVsParagraph()
        {
            _pane.ClearSelection(new StringTransfer("t", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1" });
            _pane.ClearSelection(new StringTransfer("a<b", StoryEditor.TextFields.Vernacular) { HtmlElementId = "tp_1_0_0" });
            Assert.That(_host.Posts[0].Type, Is.EqualTo("clearSelection"));
            Assert.That(_host.Posts[1].Type, Is.EqualTo("setHtml"));
            Assert.That(_host.Posts[1].GetString("html"), Is.EqualTo(HtmlText.ForParagraph("a<b")));
        }

        [TestCase(true, null, true)]                // not ready: nothing to flush
        [TestCase(false, "ok", true)]
        [TestCase(false, null, false)]              // no reply (timed out)
        [TestCase(false, "error", false)]           // the page's flush handler threw
        public void FlushEdits(bool bNotReady, string strReply, bool bExpected)
        {
            _host.IsReady = !bNotReady;
            _host.OnRequest = m => (strReply == null) ? null
                                 : (strReply == "error") ? FakeHtmlHost.Reply(new { error = "boom" })
                                 : FakeHtmlHost.Reply();
            Assert.That(_pane.FlushEdits(HtmlHostDefaults.RequestTimeout), Is.EqualTo(bExpected));
        }

        internal static HtmlMessageDispatcher PaneDispatcher(HtmlVerseControl pane)
        {
            return (HtmlMessageDispatcher)typeof(HtmlVerseControl)
                .GetField("Dispatcher", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(pane);
        }
    }
}
