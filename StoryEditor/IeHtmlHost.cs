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
    /// Messages from the page are delivered with BeginInvoke (never inside the window.external call) so that
    /// timing matches WebView2's asynchronous WebMessageReceived
    /// </summary>
    public sealed class IeHtmlHost : IHtmlHost
    {
        public const string CstrDocIdToken = "__OSE_DOC_ID__";

        private readonly WebBrowser _browser;
        private readonly List<HtmlMessage> _pendingPosts = new List<HtmlMessage>();
        private readonly HashSet<int> _awaitedRids = new HashSet<int>();
        private readonly Dictionary<int, HtmlMessage> _replies = new Dictionary<int, HtmlMessage>();
        private bool _bLoading;
        private bool _bLoadDeferred;
        private int _nDocId;
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
            _bLoading = true;
            _bLoadDeferred = false;
            _browser.DocumentText = LoadedHtml.Replace(CstrDocIdToken, _nDocId.ToString(CultureInfo.InvariantCulture));
        }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
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
            if (!IsReady)
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

        public void Dispose()
        {
            _browser.Dispose();
        }

        private void Send(HtmlMessage msg)
        {
            var doc = _browser.Document;
            if (doc == null)
            {
                Debug.WriteLine("IeHtmlHost: no document; dropped " + msg);
                return;
            }
            doc.InvokeScript("oseReceive", new object[] { msg.ToJson() });
        }

        // called (on the UI thread) from inside the page's window.external.postMessage
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

        // what the page sees as window.external: exactly one method
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
