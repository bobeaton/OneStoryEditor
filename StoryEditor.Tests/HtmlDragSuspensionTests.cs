using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlDragSuspensionTests
    {
        private int _nBefore;

        [SetUp]
        public void SetUp() { _nBefore = StoryEditor.SuspendSaveDialog; }

        [TearDown]
        public void TearDown() { StoryEditor.SuspendSaveDialog = _nBefore; }

        [Test]
        public void RepeatedBegin_CountsOnce_AndEndReleases()
        {
            var d = new HtmlDragSuspension();
            d.Begin();
            d.Begin();
            Assert.That(StoryEditor.SuspendSaveDialog, Is.EqualTo(_nBefore + 1));
            d.End();
            Assert.That(StoryEditor.SuspendSaveDialog, Is.EqualTo(_nBefore));
        }

        [Test]
        public void End_WithoutBegin_OrTwice_DoesNotUnderflow()
        {
            var d = new HtmlDragSuspension();
            d.End();
            d.Begin();
            d.End();
            d.End();
            Assert.That(StoryEditor.SuspendSaveDialog, Is.EqualTo(_nBefore));
            Assert.That(d.InProgress, Is.False);
        }
    }
}
