using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class FallbackHtmlHostTests
    {
        private FakeFailingHtmlHost _primary;
        private FakeHtmlHost _fallback;
        private FallbackHtmlHost _host;
        private Exception _reported;

        [SetUp]
        public void SetUp()
        {
            _primary = new FakeFailingHtmlHost { Engine = HtmlEngine.WebView2 };
            _fallback = new FakeHtmlHost { Engine = HtmlEngine.Ie };
            _reported = null;
            _host = new FallbackHtmlHost(_primary, () => _fallback, ex => _reported = ex);
        }

        [TearDown]
        public void TearDown()
        {
            _host.Dispose();
        }

        [Test]
        public void ForwardsToPrimary_UntilItFails()
        {
            Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.WebView2));
            _host.LoadHtml("<p>a</p>");
            _host.Post("x");
            Assert.That(_primary.LoadedHtml, Is.EqualTo("<p>a</p>"));
            Assert.That(_primary.Posts.Count, Is.EqualTo(1));
            Assert.That(_host.Control.Controls.Contains(_primary.Control), Is.True);
        }

        [Test]
        public void PrimaryFails_SwapsToFallback_ReplaysPage_RaisesEngineChanged()
        {
            var nChanged = 0;
            _host.EngineChanged += (s, e) => nChanged++;
            _host.LoadHtml("<p>a</p>");
            _primary.Fail();
            Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
            Assert.That(_fallback.LoadedHtml, Is.EqualTo("<p>a</p>"), "the last page is replayed in the fallback");
            Assert.That(nChanged, Is.EqualTo(1));
            Assert.That(_reported, Is.Not.Null);
            Assert.That(_host.Control.Controls.Contains(_fallback.Control), Is.True);
            Assert.That(_host.Control.Controls.Contains(_primary.Control), Is.False);
            Assert.That(_primary.DisposeCount, Is.EqualTo(1));
        }

        [Test]
        public void MessagesAndReady_AreForwarded_FromWhicheverHostIsCurrent()
        {
            var types = new List<string>();
            var nReady = 0;
            _host.MessageReceived += (s, m) => types.Add(m.Type);
            _host.DocumentReady += (s, e) => nReady++;
            _primary.Raise("a");
            _primary.Fail();
            _fallback.Raise("b");
            _fallback.RaiseReady();
            _primary.Raise("c");    // the old host is detached
            Assert.That(types, Is.EqualTo(new[] { "a", "b" }));
            Assert.That(nReady, Is.EqualTo(1));
        }

        [Test]
        public void SecondFailure_IsIgnored()
        {
            _primary.Fail();
            Assert.DoesNotThrow(() => _primary.Fail());
            Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
        }

        [Test]
        public void RequestDuringSwap_ReturnsNull()
        {
            // the primary fails while a Request is waiting on it (e.g. a flush): that Request gets no answer
            _primary.OnRequest = m => { _primary.Fail(); return null; };
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Null);
            Assert.That(_host.Engine, Is.EqualTo(HtmlEngine.Ie));
            _fallback.OnRequest = m => FakeHtmlHost.Reply();
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null, "later requests go to the fallback");
        }
    }
}
