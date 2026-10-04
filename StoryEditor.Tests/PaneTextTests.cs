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
        }
    }
}
