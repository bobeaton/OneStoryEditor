using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlMessageChannelTests
    {
        private List<string> _sent;
        private HtmlMessageChannel _channel;
        private List<HtmlMessage> _raised;
        private int _nReady;

        [SetUp]
        public void SetUp()
        {
            _sent = new List<string>();
            _raised = new List<HtmlMessage>();
            _nReady = 0;
            _channel = new HtmlMessageChannel(json => _sent.Add(json), () => true, "test");
            _channel.MessageReceived += (s, m) => _raised.Add(m);
            _channel.DocumentReady += (s, e) => _nReady++;
        }

        private static HtmlMessage Msg(string json) => HtmlMessage.TryParse(json);

        [Test]
        public void Post_BeforeReady_IsQueued_ThenSentOnReady()
        {
            var nDoc = _channel.BeginDocument();
            _channel.Post(HtmlMessage.Create("x"));
            Assert.That(_sent, Is.Empty);
            _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
            Assert.That(_channel.IsReady, Is.True);
            Assert.That(_sent.Count, Is.EqualTo(1));
            Assert.That(_nReady, Is.EqualTo(1));
        }

        [Test]
        public void ReadyFromOlderDocument_IsIgnored()
        {
            var nOld = _channel.BeginDocument();
            _channel.BeginDocument();
            _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nOld + "\"}"));
            Assert.That(_channel.IsReady, Is.False);
            Assert.That(_nReady, Is.EqualTo(0));
        }

        [Test]
        public void BeginDocument_DropsQueuedPosts()
        {
            _channel.BeginDocument();
            _channel.Post(HtmlMessage.Create("x"));
            var nDoc = _channel.BeginDocument();
            _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
            Assert.That(_sent, Is.Empty);
        }

        [Test]
        public void Request_NotReady_ReturnsNull()
        {
            Assert.That(_channel.Request(HtmlMessage.Create("q"), TimeSpan.FromMilliseconds(50), () => { }), Is.Null);
        }

        [Test]
        public void Request_ReturnsItsOwnReply_AndDropsLateOnes()
        {
            var nDoc = _channel.BeginDocument();
            _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
            var reply = _channel.Request(HtmlMessage.Create("q"), TimeSpan.FromSeconds(1), () =>
            {
                // the "pump": the page answers the request that was just sent
                var sent = HtmlMessage.TryParse(_sent[_sent.Count - 1]);
                _channel.Deliver(Msg("{\"type\":\"reply\",\"re\":" + sent.Rid + ",\"v\":7}"));
            });
            Assert.That(reply.TryGetInt("v", out var n) && n == 7, Is.True);
            _channel.Deliver(Msg("{\"type\":\"reply\",\"re\":999}"));   // nobody waits for it
            Assert.That(_raised.Exists(m => m.Type == "reply"), Is.False, "replies are never raised");
        }

        [Test]
        public void Request_TimesOut_ReturnsNull()
        {
            var nDoc = _channel.BeginDocument();
            _channel.Deliver(Msg("{\"type\":\"ready\",\"doc\":\"" + nDoc + "\"}"));
            Assert.That(_channel.Request(HtmlMessage.Create("q"), TimeSpan.FromMilliseconds(30), () => { }), Is.Null);
        }

        [Test]
        public void OtherMessages_AreRaised_IncludingJsError()
        {
            _channel.Deliver(Msg("{\"type\":\"textChanged\",\"id\":\"a\"}"));
            _channel.Deliver(Msg("{\"type\":\"jsError\",\"message\":\"m\"}"));
            Assert.That(_raised.ConvertAll(m => m.Type), Is.EqualTo(new[] { "textChanged", "jsError" }));
        }
    }
}
