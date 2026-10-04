using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ConNotePaneTests
    {
        [Test]
        public void HandlesEveryMessageTheNotePagesSend()
        {
            using (var pane = new HtmlConsultantNotesControl(new FakeHtmlHost()))
            {
                var aExpected = new[]
                {
                    "scrolled", "save", "reload", "realign", "bibRefJump", "openUrl", "verseLineJump", "textareaMouseDown",
                    "log", "jsError", "textChanged", "contextMenu", "scriptureDropped", "action"
                };
                Assert.That(StoryBtPaneTests.PaneDispatcher(pane).RegisteredTypes, Is.SupersetOf(aExpected));
            }
        }

        [Test]
        public void DisposingANotePane_DisposesItsHost()
        {
            var host = new FakeHtmlHost();
            new HtmlConsultantNotesControl(host).Dispose();
            Assert.That(host.DisposeCount, Is.EqualTo(1));
        }

        [Test]
        public void DisposingAStoryBtPane_DisposesItsHost()
        {
            var host = new FakeHtmlHost();
            new HtmlStoryBtControl(host).Dispose();
            Assert.That(host.DisposeCount, Is.EqualTo(1));
        }

        [Test]
        public void SetSelection_TextareaPostsSelectRange()
        {
            var host = new FakeHtmlHost();
            using (var pane = new HtmlCoachNotesControl(host))
            {
                pane.SetSelection(new StringTransfer("hello", StoryEditor.TextFields.Vernacular) { HtmlElementId = "ta_1_0" }, 1, 3);
                var post = host.Posts.Single();
                Assert.That(post.Type, Is.EqualTo("selectRange"));
                Assert.That(post.TryGetInt("start", out var nStart) && nStart == 1, Is.True);
                Assert.That(post.TryGetInt("length", out var nLen) && nLen == 3, Is.True);
            }
        }

        [Test]
        public void TextareaText_PostsSetText()
        {
            var host = new FakeHtmlHost();
            using (var pane = new HtmlCoachNotesControl(host))
            {
                pane.SetTextareaText("ta_1_0", "pasted");
                Assert.That(host.Posts.Single().GetString("text"), Is.EqualTo("pasted"));
            }
        }
    }
}
