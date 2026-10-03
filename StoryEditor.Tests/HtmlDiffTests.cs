using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlDiffTests
    {
        [SetUp]
        public void SetTags()
        {
            // make the markup predictable (StoryData sets these to styled spans at runtime)
            Rainbow.HtmlDiffEngine.Added.BeginTag = "<ins>";
            Rainbow.HtmlDiffEngine.Added.EndTag = "</ins>";
            Rainbow.HtmlDiffEngine.CommentOff.BeginTag = "<del>";
            Rainbow.HtmlDiffEngine.CommentOff.EndTag = "</del>";
        }

        [Test]
        public void Addition_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff(null, "<donkey bray> & co", false),
                        Is.EqualTo("<ins>&lt;donkey bray&gt; &amp; co</ins>"));
        }

        [Test]
        public void Deletion_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("[B&B]", null, false), Is.EqualTo("<del>[B&amp;B]</del>"));
        }

        [Test]
        public void NoChange_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("a < b", "a < b", false), Is.EqualTo("a &lt; b"));
        }

        [Test]
        public void KeepIntact_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("x&y", "x<y", true), Is.EqualTo("<del>x&amp;y</del><ins>x&lt;y</ins>"));
        }

        [Test]
        public void CharacterDiff_EncodesEveryPiece()
        {
            var result = Diff.HtmlDiff("a & b", "a < b", false);
            Assert.That(result, Does.Contain("&amp;"));
            Assert.That(result, Does.Contain("&lt;"));
            Assert.That(result.Replace("<ins>", "").Replace("</ins>", "").Replace("<del>", "").Replace("</del>", ""),
                        Does.Not.Contain("<").And.Not.Contain(" & "));
        }
    }
}
