using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class PaneTextTests
    {
        [TestCase("abc", "abc", true)]
        [TestCase("a\nb", "a\r\nb", true)]     // the page sends \r\n; the model holds \n
        [TestCase(null, "", true)]
        [TestCase("", null, true)]
        [TestCase("abc", "abd", false)]
        [TestCase(null, "x", false)]
        public void IsSame(string strModel, string strFromPage, bool bExpected)
        {
            var st = new StringTransfer(strModel, StoryEditor.TextFields.Vernacular);
            Assert.That(PaneText.IsSame(st, strFromPage), Is.EqualTo(bExpected));
            Assert.That(PaneText.IsSame(st, strFromPage, false), Is.EqualTo(bExpected));
        }

        // a flush (quiet textChanged) ignores line breaks at either end: IE drops a leading one from the value and
        //  htmlText, and a trailing one from htmlText, of a box the user merely focused
        [TestCase("x\r\n", "x", false, false)]
        [TestCase("x\r\n", "x", true, true)]
        [TestCase("x\n", "x", true, true)]
        [TestCase("\r\nx", "x", true, true)]
        [TestCase("\r\n\r\nx\r\n\r\n", "x", true, true)]
        [TestCase("x", "x\r\n", true, true)]
        [TestCase("x", "\nx\n", true, true)]
        [TestCase("\r\n", "", true, true)]
        [TestCase(null, "\r\n", true, true)]
        [TestCase("abc", "abc", true, true)]
        [TestCase("a\r\nb\r\n", "a\r\nb", true, true)]
        [TestCase("a\r\nb", "ab", true, false)]         // a line break inside the text is still a change
        [TestCase("x\r\n", "y", true, false)]
        [TestCase("x ", "x", true, false)]              // only CR/LF are ignored, not other whitespace
        [TestCase("x\r\n", "x \r\n", true, false)]
        public void IsSame_IgnoringEdgeLineBreaks(string strModel, string strFromPage, bool bIgnoreEdgeLineBreaks, bool bExpected)
        {
            var st = new StringTransfer(strModel, StoryEditor.TextFields.Vernacular);
            Assert.That(PaneText.IsSame(st, strFromPage, bIgnoreEdgeLineBreaks), Is.EqualTo(bExpected));
        }
    }
}
