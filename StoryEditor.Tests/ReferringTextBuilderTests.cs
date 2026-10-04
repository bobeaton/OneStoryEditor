using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ReferringTextBuilderTests
    {
        private static HighlightedText Item(string strId, string strClass, string strText)
        {
            return new HighlightedText(strId, strClass, strText);
        }

        [Test]
        public void SpanHtml_EncodesTextAndKeepsLineBreaks()
        {
            Assert.That(ReferringTextBuilder.SpanHtml(Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular highlight", "a<b> & c\r\nd")),
                        Is.EqualTo("<span class=\"LangVernacular highlight\">a&lt;b&gt; &amp; c<br />d</span>"));
        }

        [Test]
        public void OneItem_GivesFieldReferenceThenSpan_WithHighlightAndReadonlyRemoved()
        {
            var items = new List<HighlightedText> { Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular readonly highlight", "word") };
            Assert.That(ReferringTextBuilder.TryBuild(items, out var str), Is.True);
            Assert.That(str, Is.EqualTo("StoryLine : <span class=\"LangVernacular\">word</span>"));
        }

        [Test]
        public void TwoItems_AreJoinedWithVs()
        {
            // AddNote compares the previous *reference* name with the next *type* name, so they never match and every
            //  item starts a new " vs: " part. This characterizes that, so the change of mechanism doesn't change notes.
            var items = new List<HighlightedText>
            {
                Item("ta_1_StoryLine_0_0_Vernacular", "LangVernacular highlight", "one"),
                Item("ta_1_StoryLine_0_0_InternationalBt", "LangInternationalBt highlight", "two")
            };
            Assert.That(ReferringTextBuilder.TryBuild(items, out var str), Is.True);
            Assert.That(str, Is.EqualTo("StoryLine : <span class=\"LangVernacular\">one</span> vs: StoryLine : <span class=\"LangInternationalBt\">two</span>"));
        }

        [Test]
        public void NoItems_GivesNull_AndSucceeds()
        {
            Assert.That(ReferringTextBuilder.TryBuild(new List<HighlightedText>(), out var str), Is.True);
            Assert.That(str, Is.Null);
        }

        [Test]
        public void FromReply_ReadsItems_AndToleratesNull()
        {
            var reply = HtmlMessage.TryParse("{\"type\":\"reply\",\"items\":[{\"textareaId\":\"ta_1\",\"className\":\"c\",\"text\":\"t\"}]}");
            var list = HighlightedText.FromReply(reply);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].TextareaId, Is.EqualTo("ta_1"));
            Assert.That(list[0].ClassName, Is.EqualTo("c"));
            Assert.That(list[0].Text, Is.EqualTo("t"));
            Assert.That(HighlightedText.FromReply(null), Is.Empty);
        }
    }
}
