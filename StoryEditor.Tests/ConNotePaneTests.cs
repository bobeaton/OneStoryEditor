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

        // a flush of a note box the user merely focused can lose the note's leading/trailing line breaks. That mustn't
        //  change the note, and the IsSame check must come before the edit-token check: with no StoryEditor here the
        //  token check fails, so TextareaOnKeyUp returning true (note unchanged) shows it was never reached
        [Test]
        public void QuietTextChanged_DifferingOnlyByEdgeLineBreaks_NeverReachesTheEditTokenCheck()
        {
            // the story loader can Debug.Assert (a duplicate story guid; the stage-transition loader's missing strings);
            //  with the default listener that kills the test host, so mute asserts as FromXmlStoryTests does
            var savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
            var host = new FakeHtmlHost();
            try
            {
                QuietTextChanged_DifferingOnlyByEdgeLineBreaks(host);
            }
            finally
            {
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.AddRange(savedListeners);
            }
        }

        private static void QuietTextChanged_DifferingOnlyByEdgeLineBreaks(FakeHtmlHost host)
        {
            using (var pane = new HtmlConsultantNotesControl(host))
            {
                var strTestData = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
                var elemStory = System.Xml.Linq.XDocument.Load(System.IO.Path.Combine(strTestData, "characterization-stories.onestory"))
                    .Descendants("story").First(e => e.Descendants("ConsultantConversation").Any());
                var story = new StoryData(elemStory, strTestData);
                var nVerse = Enumerable.Range(0, story.Verses.Count + 1).First(i => VerseOf(story, i).ConsultantNotes.Count > 0);
                var aCI = VerseOf(story, nVerse).ConsultantNotes[0].FinalComment;
                aCI.SetValue("note\r\n");
                var strStored = aCI.ToString();
                Assert.That(strStored, Does.EndWith("\n"), "the model must keep the trailing line break for this test to mean anything");
                // the StoryData setter wants a StoryEditor (for the 'extra box'); the base property is all this needs
                typeof(HtmlVerseControl)
                    .GetField("<StoryData>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(pane, story);
                var strId = "ta_" + nVerse + "_0";

                host.Raise("textChanged", new { id = strId, value = "note", quiet = true });
                Assert.That(aCI.ToString(), Is.EqualTo(strStored));

                Assert.That(InvokeTextareaOnKeyUp(pane, strId, "note", true), Is.True, "quiet unchanged text reached the edit-token check");
                Assert.That(InvokeTextareaOnKeyUp(pane, strId, "\r\nnote", true), Is.True);
                Assert.That(aCI.ToString(), Is.EqualTo(strStored));

                // a keystroke, or different text: goes on to the edit-token check (which fails here)
                Assert.That(InvokeTextareaOnKeyUp(pane, strId, "note", false), Is.False);
                Assert.That(InvokeTextareaOnKeyUp(pane, strId, "other", true), Is.False);
                Assert.That(aCI.ToString(), Is.EqualTo(strStored));
            }
        }

        private static VerseData VerseOf(StoryData story, int nVerseIndex)
        {
            return (nVerseIndex == 0) ? story.Verses.FirstVerse : story.Verses[nVerseIndex - 1];
        }

        private static bool InvokeTextareaOnKeyUp(HtmlConNoteControl pane, string strId, string strText, bool bQuiet)
        {
            return (bool)typeof(HtmlConNoteControl)
                .GetMethod("TextareaOnKeyUp", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(pane, new object[] { strId, strText, bQuiet });
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
