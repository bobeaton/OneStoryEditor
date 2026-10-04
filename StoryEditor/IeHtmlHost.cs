using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
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
        private readonly HtmlMessageChannel _channel;
        private readonly System.Windows.Forms.Timer _loadWatchdog;
        private bool _bLoading;
        private bool _bLoadDeferred;
        private bool _bLoadCompleted;           // the in-flight load's DocumentCompleted has come
        private bool _bExpectStaleCompletion;   // a load ended by its 'ready' before its DocumentCompleted came
        private bool _bDisposed;
        private int _nLoadingDocId;             // the doc id of the load in flight

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
            _channel = new HtmlMessageChannel(SendJson, () => !_bDisposed && !_browser.IsDisposed, "IeHtmlHost");
            _channel.MessageReceived += (s, m) => MessageReceived?.Invoke(this, m);
            _channel.DocumentReady += (s, e) => DocumentReady?.Invoke(this, EventArgs.Empty);
            // generous: a big story's page can take seconds to load, and a deferred load mustn't start in the middle
            //  of one. (The load normally ends long before this, on DocumentCompleted or the page's 'ready'.)
            _loadWatchdog = new System.Windows.Forms.Timer { Interval = 15000 };
            _loadWatchdog.Tick += OnLoadWatchdog;
        }

        public Control Control => _browser;
        public bool IsReady => _channel.IsReady;
        public string LoadedHtml { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;
        public HtmlEngine Engine => HtmlEngine.Ie;
        public event EventHandler EngineChanged { add { } remove { } }   // never raised: IE is the last resort

        public void LoadHtml(string strHtml)
        {
            _channel.BeginDocument();
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
            _nLoadingDocId = _channel.DocId;
            _loadWatchdog.Stop();
            _loadWatchdog.Start();
            // the page renders in whatever document mode its own markup gives it (the pane pages: quirks mode, as they
            //  always have; their scripts, bridge.js included, work there)
            _browser.DocumentText = LoadedHtml.Replace(CstrDocIdToken, _channel.DocIdText);
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
            _channel.Post(HtmlMessage.Create(strType, payload));
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            // replies arrive through BeginInvoke, so let the queue run while waiting
            return _channel.Request(HtmlMessage.Create(strType, payload), timeout, Application.DoEvents);
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

        // (the channel only calls this while we're alive)
        private void SendJson(string strJson)
        {
            var doc = _browser.Document;
            if (doc == null)
            {
                Debug.WriteLine("IeHtmlHost: no document; dropped " + strJson);
                return;
            }
            doc.InvokeScript("oseReceive", new object[] { strJson });
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
            // a 'ready' for the load in flight ends that load, even if it's no longer the current document
            //  (this may start a deferred load, in which case the channel's doc id is that one's and this ready is ignored)
            if ((msg.Type == HtmlMessage.CstrTypeReady) && _bLoading &&
                (msg.DocId == _nLoadingDocId.ToString(CultureInfo.InvariantCulture)))
                EndLoadWithoutCompletion();
            _channel.Deliver(msg);
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
