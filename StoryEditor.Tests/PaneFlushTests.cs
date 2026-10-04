using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class PaneFlushTests
    {
        private static KeyValuePair<string, Func<bool>> Pane(string strLabel, Func<bool> flush)
        {
            return new KeyValuePair<string, Func<bool>>(strLabel, flush);
        }

        [Test]
        public void FlushAll_AllAnswer_GivesNull()
        {
            var nCalls = 0;
            var panes = new[] { Pane("Story", () => { nCalls++; return true; }), Pane("Coach", () => { nCalls++; return true; }) };
            Assert.That(PaneFlush.FlushAll(panes), Is.Null);
            Assert.That(nCalls, Is.EqualTo(2));
        }

        [Test]
        public void FlushAll_NamesTheFirstPaneThatDidNotAnswer()
        {
            var panes = new[] { Pane("Story", () => true), Pane("Consultant Notes", () => false), Pane("Coach", () => false) };
            Assert.That(PaneFlush.FlushAll(panes), Is.EqualTo("Consultant Notes"));
        }

        [Test]
        public void Run_AllCollected_NeverAsks()
        {
            var outcome = PaneFlush.Run(() => null, s => { Assert.Fail("must not ask"); return FlushChoice.Cancel; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.AllEditsCollected));
        }

        [Test]
        public void Run_RetryThenSuccess_IsAllCollected()
        {
            var aResults = new Queue<string>(new[] { "Story", null });
            var nAsked = 0;
            var outcome = PaneFlush.Run(() => aResults.Dequeue(), s => { nAsked++; return FlushChoice.Retry; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.AllEditsCollected));
            Assert.That(nAsked, Is.EqualTo(1));
        }

        [Test]
        public void Run_SaveWithout_IsSomeEditsMissing_AndPassesThePaneName()
        {
            string strAskedAbout = null;
            var outcome = PaneFlush.Run(() => "Coach Notes", s => { strAskedAbout = s; return FlushChoice.SaveWithoutLatest; }, false);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.SomeEditsMissing));
            Assert.That(strAskedAbout, Is.EqualTo("Coach Notes"));
        }

        [Test]
        public void Run_Cancel_IsCancelled()
        {
            Assert.That(PaneFlush.Run(() => "Story", s => FlushChoice.Cancel, false), Is.EqualTo(FlushOutcome.Cancelled));
        }

        [Test]
        public void Run_Autosave_NeverAsks_AndSkips()
        {
            var outcome = PaneFlush.Run(() => "Story", s => { Assert.Fail("autosave must not ask"); return FlushChoice.Retry; }, true);
            Assert.That(outcome, Is.EqualTo(FlushOutcome.Cancelled));
        }
    }
}
