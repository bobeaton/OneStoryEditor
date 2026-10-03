using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlTextTests
    {
        [Test]
        public void Encode_EncodesOnlyHtmlSpecialCharacters()
        {
            Assert.That(HtmlText.Encode("a & b <donkey bray> \"q\" é ਪੰ"),
                        Is.EqualTo("a &amp; b &lt;donkey bray&gt; &quot;q&quot; é ਪੰ"));
        }

        [Test]
        public void Encode_NullAndEmptyPassThrough()
        {
            Assert.That(HtmlText.Encode((string)null), Is.Null);
            Assert.That(HtmlText.Encode(""), Is.EqualTo(""));
        }

        [Test]
        public void Encode_Char()
        {
            Assert.That(HtmlText.Encode('<'), Is.EqualTo("&lt;"));
            Assert.That(HtmlText.Encode('x'), Is.EqualTo("x"));
        }

        [Test]
        public void ForParagraph_EncodesAndConvertsLineBreaks()
        {
            Assert.That(HtmlText.ForParagraph("a & b\r\nc\nd"), Is.EqualTo("a &amp; b<br />c<br />d"));
        }

        [Test]
        public void LineBreaksToBr_LeavesMarkupAlone()
        {
            Assert.That(HtmlText.LineBreaksToBr("<b>x</b>\r\ny"), Is.EqualTo("<b>x</b><br />y"));
        }

        [Test]
        public void FromIeHtmlText_StripsHighlightSpansAndDecodes()
        {
            const string ieHtmlText = "[B&amp;B] <SPAN class=\"LangVernacular StoryLine highlight\">idop</SPAN><BR>baris &lt;2&gt;";
            Assert.That(HtmlText.FromIeHtmlText(ieHtmlText), Is.EqualTo("[B&B] idop\r\nbaris <2>"));
        }

        [Test]
        public void FromIeHtmlText_UnquotedSpanAttributes()
        {
            Assert.That(HtmlText.FromIeHtmlText("a <SPAN class=highlight>b</SPAN>"), Is.EqualTo("a b"));
        }

        [Test]
        public void FromIeHtmlText_KeepsLiteralAngleBracketText()
        {
            // raw "<x>" can appear when TriggerMyBlur rebuilt the textarea from its (decoded) value;
            //  only our own <span>/<br> markup may be removed
            Assert.That(HtmlText.FromIeHtmlText("a <x> &lt;y&gt;"), Is.EqualTo("a <x> <y>"));
        }

        [Test]
        public void FromIeHtmlText_Nbsp()
        {
            Assert.That(HtmlText.FromIeHtmlText("a&nbsp;b"), Is.EqualTo("a b"));
        }
    }
}
