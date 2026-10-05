using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// IHtmlHost over WebView2 (Chromium; the Evergreen runtime). The only class allowed to touch WebView2 types.
    /// Pages are served from memory at https://ose.pane/page.html?d=docId (no NavigateToString 2 MB limit) and every
    /// navigation the host didn't start is cancelled. Message bookkeeping is HtmlMessageChannel, shared with IeHtmlHost
    /// </summary>
    public sealed class WebView2HtmlHost : IHtmlHost, IFailingHtmlHost
    {
        private const string CstrPageOrigin = "https://ose.pane/";
        private const string CstrPagePath = "page.html";
        public static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(20);

        public static string DefaultUserDataFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneStory Editor", "WebView2");

        // null when no WebView2 runtime is installed (it throws WebView2RuntimeNotFoundException then) or it can't be queried
        public static string RuntimeVersion
        {
            get
            {
                try
                {
                    return CoreWebView2Environment.GetAvailableBrowserVersionString();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private readonly WebView2 _view;
        private readonly HtmlMessageChannel _channel;
        private readonly string _strUserDataFolder;
        private readonly bool _bAllowContextMenu;
        private readonly Timer _startWatchdog;
        private CoreWebView2Environment _env;
        private bool _bStarted;
        private bool _bStarting;
        private bool _bFailed;
        private bool _bDisposed;
        private string _strExpectedUrl;     // the one navigation we allow next (null once it has started)
        private string _strServedHtml;      // LoadedHtml with the doc id filled in

        public WebView2HtmlHost(HtmlHostOptions options)
        {
            _strUserDataFolder = options?.UserDataFolder ?? DefaultUserDataFolder;
            _bAllowContextMenu = options?.AllowBrowserContextMenu ?? false;
            _view = new WebView2 { Dock = DockStyle.Fill, DefaultBackgroundColor = Color.White };
            _channel = new HtmlMessageChannel(SendJson, () => !_bDisposed && !_bFailed && _bStarted && !_view.IsDisposed,
                                              "WebView2HtmlHost");
            _channel.MessageReceived += (s, m) => MessageReceived?.Invoke(this, m);
            _channel.DocumentReady += (s, e) => DocumentReady?.Invoke(this, EventArgs.Empty);
            _startWatchdog = new Timer { Interval = (int)StartTimeout.TotalMilliseconds };
            _startWatchdog.Tick += (s, e) => Fail(new TimeoutException("WebView2 did not start within " + StartTimeout.TotalSeconds + " s"));
            _view.CoreWebView2InitializationCompleted += OnInitializationCompleted;
            _view.HandleCreated += (s, e) => Start();
        }

        public Control Control => _view;
        public HtmlEngine Engine => HtmlEngine.WebView2;
        public bool IsReady => _channel.IsReady;
        public string LoadedHtml { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;
        public event EventHandler EngineChanged { add { } remove { } }
        public event EventHandler<Exception> Failed;

        private async void Start()
        {
            if (_bStarting || _bStarted || _bDisposed)
                return;
            _bStarting = true;
            _startWatchdog.Start();
            try
            {
                try
                {
                    Directory.CreateDirectory(_strUserDataFolder);
                }
                catch (Exception ex)
                {
                    // (best effort: WebView2 itself reports a folder it can't use)
                    Debug.WriteLine("WebView2HtmlHost: couldn't create " + _strUserDataFolder + ": " + ex.Message);
                }
                _env = await CoreWebView2Environment.CreateAsync(null, _strUserDataFolder);
                if (_bDisposed || _bFailed)
                    return;
                await _view.EnsureCoreWebView2Async(_env);
                if (_bDisposed || _bFailed)
                    return;
                Configure(_view.CoreWebView2);
                _startWatchdog.Stop();
                _bStarted = true;
                if (LoadedHtml != null)
                    Navigate();
            }
            catch (Exception ex)
            {
                // the spike saw WebView2RuntimeNotFoundException from CreateAsync and COMException 0x80080005 from
                //  EnsureCoreWebView2Async (unusable data folder); treat anything as a failed start
                Fail(ex);
            }
        }

        private void OnInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
                Fail(e.InitializationException ?? new InvalidOperationException("WebView2 initialization failed"));
        }

        private void Configure(CoreWebView2 core)
        {
            var settings = core.Settings;
            settings.AreDefaultContextMenusEnabled = _bAllowContextMenu;    // (HtmlForm wants right-click Copy)
#if DEBUG
            settings.AreDevToolsEnabled = true;
#else
            settings.AreDevToolsEnabled = false;
#endif
            settings.IsStatusBarEnabled = false;
            settings.IsZoomControlEnabled = true;
            core.AddWebResourceRequestedFilter(CstrPageOrigin + "*", CoreWebView2WebResourceContext.All);
            core.WebResourceRequested += OnWebResourceRequested;
            core.NavigationStarting += OnNavigationStarting;
            core.NewWindowRequested += (s, e) => e.Handled = true;
            core.WebMessageReceived += OnWebMessageReceived;
            core.ProcessFailed += OnProcessFailed;
        }

        private void OnProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e)
        {
            // only the browser or the page's renderer gone is fatal; WebView2 restarts helper processes (GPU, utility,
            //  ...) itself, and an unresponsive renderer may recover
            if ((e.ProcessFailedKind == CoreWebView2ProcessFailedKind.BrowserProcessExited) ||
                (e.ProcessFailedKind == CoreWebView2ProcessFailedKind.RenderProcessExited))
                Fail(new InvalidOperationException("WebView2 process failed: " + e.ProcessFailedKind));
            else
                Debug.WriteLine("WebView2HtmlHost: process failure (not fatal): " + e.ProcessFailedKind);
        }

        public void LoadHtml(string strHtml)
        {
            _channel.BeginDocument();
            LoadedHtml = strHtml ?? String.Empty;
            if (_bStarted)
                Navigate();
            // (otherwise Start() navigates once it has finished)
        }

        private void Navigate()
        {
            if (_bDisposed || _bFailed)
                return;
            _strServedHtml = LoadedHtml.Replace(IeHtmlHost.CstrDocIdToken, _channel.DocIdText);
            _strExpectedUrl = CstrPageOrigin + CstrPagePath + "?d=" + _channel.DocIdText;
            _view.CoreWebView2.Navigate(_strExpectedUrl);
        }

        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // only the navigation we started, and only once: no back/forward to an older page, no reload of this one,
            //  no links or script navigation away
            if ((_strExpectedUrl == null) || (e.Uri != _strExpectedUrl))
            {
                Debug.WriteLine("WebView2HtmlHost: cancelled navigation to " + e.Uri);
                e.Cancel = true;
                return;
            }
            _strExpectedUrl = null;
        }

        private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            // only the current document's page; anything else (the favicon.ico the browser asks for on every load, or the
            //  page of a navigation a later LoadHtml superseded: it mustn't get the newer document) is a 404
            var uri = new Uri(e.Request.Uri);
            if (!String.Equals(uri.AbsolutePath, "/" + CstrPagePath, StringComparison.OrdinalIgnoreCase) ||
                (uri.Query != "?d=" + _channel.DocIdText))
            {
                e.Response = _env.CreateWebResourceResponse(null, 404, "Not Found", "");
                return;
            }
            var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(_strServedHtml ?? String.Empty));
            e.Response = _env.CreateWebResourceResponse(stream, 200, "OK", "Content-Type: text/html; charset=utf-8");
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string strJson;
            try
            {
                strJson = e.TryGetWebMessageAsString();
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("WebView2HtmlHost: non-string message from page");
                return;
            }
            var msg = HtmlMessage.TryParse(strJson);
            if (msg == null)
            {
                Debug.WriteLine("WebView2HtmlHost: unparseable message from page: " + strJson);
                return;
            }
            // not inline: while a WebMessageReceived handler runs, WebView2 raises no further WebMessageReceived (even
            //  when pumped), so a Request made by a MessageReceived handler would always time out (spike, Q6)
            if (_bDisposed || _view.IsDisposed || !_view.IsHandleCreated)
                return;
            _view.BeginInvoke((Action)(() =>
            {
                if (!_bDisposed)
                    _channel.Deliver(msg);
            }));
        }

        public void Post(string strType, object payload = null)
        {
            _channel.Post(HtmlMessage.Create(strType, payload));
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            return _channel.Request(HtmlMessage.Create(strType, payload), timeout, Application.DoEvents);
        }

        // (the channel only calls this while we're started and alive)
        private void SendJson(string strJson)
        {
            _view.CoreWebView2?.PostWebMessageAsString(strJson);
        }

        public void ShowPrintPreview()
        {
            if (_bStarted && !_bDisposed && !_bFailed && !_view.IsDisposed)
                _view.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        private void Fail(Exception ex)
        {
            if (_bFailed || _bDisposed)
                return;
            _bFailed = true;
            _startWatchdog.Stop();
            Debug.WriteLine("WebView2HtmlHost: failed: " + ex);
            LogFailure(ex);

            // raised later, not from inside the WebView2 event (or await continuation) that found the failure: the
            //  subscriber (FallbackHtmlHost) disposes this host, and with it the control that is raising the event
            if (_view.IsHandleCreated && !_view.IsDisposed)
                _view.BeginInvoke((Action)(() =>
                {
                    if (!_bDisposed)
                        Failed?.Invoke(this, ex);
                }));
            else
                Failed?.Invoke(this, ex);
        }

        // where the full story of a failure goes (the status bar only has room for the message): beside the user data
        //  folder, i.e. %LOCALAPPDATA%\OneStory Editor\WebView2-failures.log for the app
        public string FailureLogPath =>
            Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_strUserDataFolder)) ?? String.Empty, "WebView2-failures.log");

        // best effort: never let logging a failure cause another
        private void LogFailure(Exception ex)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("==== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine("runtime: " + (RuntimeVersion ?? "(none)"));
                var asmCore = typeof(CoreWebView2Environment).Assembly;
                sb.AppendLine("Core.dll: " + asmCore.GetName().Version + " (file " +
                              FileVersionInfo.GetVersionInfo(asmCore.Location).FileVersion + ") " + asmCore.Location);
                var strLoader = Path.Combine(Path.GetDirectoryName(asmCore.Location) ?? String.Empty, "WebView2Loader.dll");
                sb.AppendLine("WebView2Loader.dll: " + (File.Exists(strLoader)
                                                            ? FileVersionInfo.GetVersionInfo(strLoader).FileVersion
                                                            : "not beside Core.dll"));
                sb.AppendLine("process: " + (Environment.Is64BitProcess ? "64" : "32") + "-bit; thread " +
                              System.Threading.Thread.CurrentThread.ManagedThreadId + " (" +
                              System.Threading.Thread.CurrentThread.GetApartmentState() + "); on the control's thread: " +
                              (!_view.IsHandleCreated || !_view.InvokeRequired) + "; context: " +
                              (System.Threading.SynchronizationContext.Current?.GetType().Name ?? "none"));
                sb.AppendLine("state: starting=" + _bStarting + " started=" + _bStarted + " handle=" + _view.IsHandleCreated +
                              " visible=" + _view.Visible);
                sb.AppendLine("pane: " + OwnerChain());
                sb.AppendLine(ex.ToString());
                var strPath = FailureLogPath;
                Directory.CreateDirectory(Path.GetDirectoryName(strPath));
                if (File.Exists(strPath) && (new FileInfo(strPath).Length > 1024 * 1024))
                    File.Delete(strPath);   // (keep it small: only the recent failures matter)
                File.AppendAllText(strPath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception exLog)
            {
                Debug.WriteLine("WebView2HtmlHost: couldn't log the failure: " + exLog.Message);
            }
        }

        // e.g. "WebView2 < Panel < HtmlStoryBtControl 'htmlStoryBtControl' < ... < StoryEditor"
        private string OwnerChain()
        {
            var sb = new StringBuilder();
            for (Control c = _view; c != null; c = c.Parent)
            {
                if (sb.Length > 0)
                    sb.Append(" < ");
                sb.Append(c.GetType().Name);
                if (!String.IsNullOrEmpty(c.Name))
                    sb.Append(" '" + c.Name + "'");
            }
            return sb.ToString();
        }

        // safe to call more than once (the owning pane disposes us, and WinForms disposes the control with it)
        public void Dispose()
        {
            if (_bDisposed)
                return;
            _bDisposed = true;
            _startWatchdog.Stop();
            _startWatchdog.Dispose();
            _view.Dispose();
        }
    }
}
