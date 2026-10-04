using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// IHtmlHost over the IE WebBrowser control. The only class allowed to touch WebBrowser/HtmlDocument.
    /// Messages from the page are delivered with BeginInvoke (never inside the page's call into C#) so that
    /// timing matches WebView2's asynchronous WebMessageReceived
    /// </summary>
    public sealed class IeHtmlHost : IHtmlHost
    {
        public const string CstrDocIdToken = "__OSE_DOC_ID__";

        private readonly WebBrowser _browser;
        private readonly List<HtmlMessage> _pendingPosts = new List<HtmlMessage>();
        private readonly HashSet<int> _awaitedRids = new HashSet<int>();
        private readonly Dictionary<int, HtmlMessage> _replies = new Dictionary<int, HtmlMessage>();
        private readonly System.Windows.Forms.Timer _loadWatchdog;
        private bool _bLoading;
        private bool _bLoadDeferred;
        private bool _bLoadCompleted;           // the in-flight load's DocumentCompleted has come
        private bool _bExpectStaleCompletion;   // a load ended by its 'ready' before its DocumentCompleted came
        private bool _bDisposed;
        private int _nDocId;
        private int _nLoadingDocId;             // the doc id of the load in flight
        private int _nLastRid;

        public IeHtmlHost(HtmlHostOptions options)
        {
            _browser = new WebBrowser
            {
                IsWebBrowserContextMenuEnabled = options.AllowBrowserContextMenu,
                AllowWebBrowserDrop = options.AllowFileDrop,
                Dock = DockStyle.Fill,
                ObjectForScripting = new ScriptingBridge(this)
            };
            _browser.DocumentCompleted += OnDocumentCompleted;
            // generous: a big story's page can take seconds to load, and a deferred load mustn't start in the middle
            //  of one. (The load normally ends long before this, on DocumentCompleted or the page's 'ready'.)
            _loadWatchdog = new System.Windows.Forms.Timer { Interval = 15000 };
            _loadWatchdog.Tick += OnLoadWatchdog;
        }

        public Control Control => _browser;
        public bool IsReady { get; private set; }
        public string LoadedHtml { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;

        public void LoadHtml(string strHtml)
        {
            _nDocId++;
            IsReady = false;
            _pendingPosts.Clear();
            LoadedHtml = strHtml ?? String.Empty;

            // WebBrowser.DocumentText navigates to about:blank and then streams the text in; a second DocumentText set
            //  while the first is in flight ends with a blank page (the late about:blank completion wipes the stream),
            //  even though the page already said 'ready'. So only one load is ever in flight; the latest waits its turn
            if (_bLoading)
            {
                _bLoadDeferred = true;
                return;
            }
            StartLoad();
        }

        private void StartLoad()
        {
            if (_bDisposed || _browser.IsDisposed)
                return;
            _bLoading = true;
            _bLoadDeferred = false;
            _bLoadCompleted = false;
            _nLoadingDocId = _nDocId;
            _loadWatchdog.Stop();
            _loadWatchdog.Start();
            // the page renders in whatever document mode its own markup gives it (the pane pages: quirks mode, as they
            //  always have; their scripts, bridge.js included, work there)
            _browser.DocumentText = LoadedHtml.Replace(CstrDocIdToken, _nDocId.ToString(CultureInfo.InvariantCulture));
        }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // framed pages raise this per frame; only the top-level completion ends the load
            if ((_browser.ReadyState != WebBrowserReadyState.Complete) ||
                ((e.Url != null) && (_browser.Url != null) && (e.Url.AbsoluteUri != _browser.Url.AbsoluteUri)))
                return;

            // the completion of a load that its 'ready' already ended; it says nothing about the load now in flight
            if (_bExpectStaleCompletion)
            {
                _bExpectStaleCompletion = false;
                return;
            }
            _bLoadCompleted = true;
            EndLoad();
        }

        // the page in flight said 'ready' (its window.onload ran), or the watchdog gave up on it: end the load even
        //  though DocumentCompleted hasn't come (or never will). If it comes later, it isn't the next load's
        private void EndLoadWithoutCompletion()
        {
            if (!_bLoading)
                return;
            if (!_bLoadCompleted)
                _bExpectStaleCompletion = true;
            EndLoad();
        }

        // if the navigation is cancelled or fails, DocumentCompleted never comes; don't let that block every later load
        private void OnLoadWatchdog(object sender, EventArgs e)
        {
            if (_bDisposed || _browser.IsDisposed)
                return;
            Debug.WriteLine("IeHtmlHost: load did not complete in time; carrying on");
            EndLoadWithoutCompletion();
        }

        private void EndLoad()
        {
            if (_bDisposed || _browser.IsDisposed)
                return;
            _loadWatchdog.Stop();
            _bLoading = false;
            if (_bLoadDeferred)
                StartLoad();
        }

        public void Post(string strType, object payload = null)
        {
            var msg = HtmlMessage.Create(strType, payload);
            if (!IsReady)
                _pendingPosts.Add(msg);
            else
                Send(msg);
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            if (!IsReady || _bDisposed || _browser.IsDisposed)
                return null;

            var nRid = ++_nLastRid;
            _awaitedRids.Add(nRid);
            try
            {
                Send(HtmlMessage.Create(strType, payload).WithRid(nRid));
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
                        Debug.WriteLine($"IeHtmlHost: no reply to '{strType}' within {timeout.TotalMilliseconds} ms");
                        return null;
                    }
                    Application.DoEvents();     // replies arrive through BeginInvoke, so let the queue run
                    if (_bDisposed || _browser.IsDisposed)
                    {
                        Debug.WriteLine($"IeHtmlHost: disposed while waiting for a reply to '{strType}'");
                        return null;            // (the pane closed during the pump: no reply can come now)
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

        public void ShowPrintPreview()
        {
            _browser.ShowPrintPreviewDialog();
        }

        // safe to call more than once (the owning pane disposes us, and WinForms disposes the browser control with it)
        public void Dispose()
        {
            if (_bDisposed)
                return;
            _bDisposed = true;
            _loadWatchdog.Stop();
            _loadWatchdog.Dispose();
            _browser.Dispose();
        }

        private void Send(HtmlMessage msg)
        {
            if (_bDisposed || _browser.IsDisposed)
                return;
            var doc = _browser.Document;
            if (doc == null)
            {
                Debug.WriteLine("IeHtmlHost: no document; dropped " + msg);
                return;
            }
            doc.InvokeScript("oseReceive", new object[] { msg.ToJson() });
        }

        // called (on the UI thread) from inside the page's postMessage call (see bridge.js)
        private void OnScriptMessage(string strJson)
        {
            var msg = HtmlMessage.TryParse(strJson);
            if (msg == null)
            {
                Debug.WriteLine("IeHtmlHost: unparseable message from page: " + strJson);
                return;
            }
            if (_browser.IsDisposed || !_browser.IsHandleCreated)
                return;
            _browser.BeginInvoke((Action)(() => Deliver(msg)));
        }

        private void Deliver(HtmlMessage msg)
        {
            switch (msg.Type)
            {
                case HtmlMessage.CstrTypeReply:
                    var nRe = msg.ReplyTo;
                    if (nRe.HasValue && _awaitedRids.Contains(nRe.Value))
                        _replies[nRe.Value] = msg;
                    return;     // late replies (after a timeout) are dropped

                case HtmlMessage.CstrTypeReady:
                    // (this may start a deferred load, in which case _nDocId is that one's and this ready is ignored)
                    if (_bLoading && (msg.DocId == _nLoadingDocId.ToString(CultureInfo.InvariantCulture)))
                        EndLoadWithoutCompletion();
                    if (msg.DocId != _nDocId.ToString(CultureInfo.InvariantCulture))
                        return; // an older document finished loading after LoadHtml was called again
                    IsReady = true;
                    var aPending = _pendingPosts.ToArray();
                    _pendingPosts.Clear();
                    foreach (var pending in aPending)
                        Send(pending);
                    DocumentReady?.Invoke(this, EventArgs.Empty);
                    return;

                case HtmlMessage.CstrTypeJsError:
                    Debug.WriteLine($"IeHtmlHost: script error: {msg.GetString("message")} ({msg.GetString("source")}:{msg.Body["line"]})");
                    break;      // also passed on, so tests (and panes) can see it
            }
            MessageReceived?.Invoke(this, msg);
        }

        // IE reads FEATURE_BROWSER_EMULATION for the exe when the first WebBrowser is created in the process
        public static void EnsureIe9Mode(string strExeName)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
                key?.SetValue(strExeName, 9999, RegistryValueKind.DWord);
        }

        // what the page sees as its external object: exactly one method
        [ComVisible(true)]
        public sealed class ScriptingBridge
        {
            private readonly IeHtmlHost _host;

            internal ScriptingBridge(IeHtmlHost host)
            {
                _host = host;
            }

            // ReSharper disable once InconsistentNaming (JS calls it by this name)
            public void postMessage(string strJson)
            {
                _host.OnScriptMessage(strJson);
            }
        }
    }
}
