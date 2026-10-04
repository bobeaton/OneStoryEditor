using System;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlMessageDispatcherTests
    {
        [Test]
        public void Dispatch_KnownType_CallsHandler()
        {
            var d = new HtmlMessageDispatcher();
            string strSeen = null;
            d.Register("focus", m => strSeen = m.GetString("id"));
            Assert.That(d.Dispatch(HtmlMessage.Create("focus", new { id = "ta_1" })), Is.True);
            Assert.That(strSeen, Is.EqualTo("ta_1"));
        }

        [Test]
        public void Dispatch_UnknownType_ReturnsFalse_DoesNotThrow()
        {
            var d = new HtmlMessageDispatcher();
            Assert.That(d.Dispatch(HtmlMessage.Create("nope")), Is.False);
        }

        [Test]
        public void Dispatch_HandlerThrows_IsCaughtAndReported()
        {
            var d = new HtmlMessageDispatcher();
            string strReported = null;
            d.ReportError = s => strReported = s;
            d.Register("boom", m => throw new InvalidOperationException("bad id"));
            Assert.DoesNotThrow(() => d.Dispatch(HtmlMessage.Create("boom")));
            Assert.That(strReported, Does.Contain("bad id"));
        }

        [Test]
        public void Register_SameTypeTwice_LastWins()
        {
            var d = new HtmlMessageDispatcher();
            var n = 0;
            d.Register("t", m => n = 1);
            d.Register("t", m => n = 2);
            d.Dispatch(HtmlMessage.Create("t"));
            Assert.That(n, Is.EqualTo(2));
            Assert.That(d.RegisteredTypes, Is.EquivalentTo(new[] { "t" }));
        }
    }
}
