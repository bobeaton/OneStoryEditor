using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// what both browser hosts share: document ids, posts queued until the page is 'ready', matching replies to
    /// requests, and the bounded wait for a reply. The host supplies how to send JSON to the page, whether it's still
    /// alive, and (per request) how to pump the message loop while waiting
    /// </summary>
    internal sealed class HtmlMessageChannel
    {
        private readonly Action<string> _sendJson;
        private readonly Func<bool> _isAlive;
        private readonly string _strHostName;
        private readonly List<HtmlMessage> _pendingPosts = new List<HtmlMessage>();
        private readonly HashSet<int> _awaitedRids = new HashSet<int>();
        private readonly Dictionary<int, HtmlMessage> _replies = new Dictionary<int, HtmlMessage>();
        private int _nLastRid;

        public HtmlMessageChannel(Action<string> sendJson, Func<bool> isAlive, string strHostName)
        {
            _sendJson = sendJson;
            _isAlive = isAlive;
            _strHostName = strHostName;
        }

        public int DocId { get; private set; }
        public string DocIdText => DocId.ToString(CultureInfo.InvariantCulture);
        public bool IsReady { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;

        // a new document is about to load: returns its id (what the page's bridge.js will put in every message)
        public int BeginDocument()
        {
            DocId++;
            IsReady = false;
            _pendingPosts.Clear();
            return DocId;
        }

        public void Post(HtmlMessage msg)
        {
            if (!IsReady)
                _pendingPosts.Add(msg);
            else
                Send(msg);
        }

        // pumpOnce: let the host's message loop run once (replies arrive through it)
        public HtmlMessage Request(HtmlMessage msg, TimeSpan timeout, Action pumpOnce)
        {
            if (!IsReady || !_isAlive())
                return null;

            var nRid = ++_nLastRid;
            _awaitedRids.Add(nRid);
            try
            {
                Send(msg.WithRid(nRid));
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (_replies.TryGetValue(nRid, out var reply))
                    {
                        _replies.Remove(nRid);
                        return reply;
                    }
                    if (sw.Elapsed >= timeout)
                    {
                        Debug.WriteLine($"{_strHostName}: no reply to '{msg.Type}' within {timeout.TotalMilliseconds} ms");
                        return null;
                    }
                    pumpOnce();
                    if (!_isAlive())
                    {
                        Debug.WriteLine($"{_strHostName}: disposed while waiting for a reply to '{msg.Type}'");
                        return null;
                    }
                    if (!_replies.ContainsKey(nRid))
                        Thread.Sleep(1);
                }
            }
            finally
            {
                _awaitedRids.Remove(nRid);
            }
        }

        // returns true when msg was the current document's 'ready' (so the host can end its load)
        public bool Deliver(HtmlMessage msg)
        {
            switch (msg.Type)
            {
                case HtmlMessage.CstrTypeReply:
                    var nRe = msg.ReplyTo;
                    if (nRe.HasValue && _awaitedRids.Contains(nRe.Value))
                        _replies[nRe.Value] = msg;
                    return false;   // late replies (after a timeout) are dropped

                case HtmlMessage.CstrTypeReady:
                    if (msg.DocId != DocIdText)
                        return false; // an older document finished loading after another load began
                    IsReady = true;
                    var aPending = _pendingPosts.ToArray();
                    _pendingPosts.Clear();
                    foreach (var pending in aPending)
                        Send(pending);
                    DocumentReady?.Invoke(this, EventArgs.Empty);
                    return true;

                case HtmlMessage.CstrTypeJsError:
                    Debug.WriteLine($"{_strHostName}: script error: {msg.GetString("message")} ({msg.GetString("source")}:{msg.Body["line"]})");
                    break;  // also passed on, so tests (and panes) can see it
            }
            MessageReceived?.Invoke(this, msg);
            return false;
        }

        private void Send(HtmlMessage msg)
        {
            if (_isAlive())
                _sendJson(msg.ToJson());
        }
    }
}
