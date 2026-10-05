using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// the host HtmlHostFactory hands out when it chooses WebView2: it forwards to the WebView2 host, and if that fails
    /// (start-up or browser process), swaps an IE host into the same container and raises EngineChanged so the owner
    /// rebuilds the page with the IE scripts. If the owner doesn't, the last page is replayed in IE -- unless it was a
    /// modern (WebView2) page, which IE can't run: then the pane is left blank rather than broken
    /// </summary>
    public sealed class FallbackHtmlHost : IHtmlHost
    {
        private readonly FocusForwardingPanel _container = new FocusForwardingPanel { Dock = DockStyle.Fill };
        private readonly Func<IHtmlHost> _createFallback;
        private readonly Action<Exception> _onFallback;
        private IHtmlHost _inner;
        private bool _bSwapped;
        private bool _bDisposed;
        private int _nLoads;            // LoadHtml calls so far (so the swap can tell whether the owner rebuilt its page)

        public FallbackHtmlHost(IHtmlHost primary, Func<IHtmlHost> createFallback, Action<Exception> onFallback)
        {
            _createFallback = createFallback;
            _onFallback = onFallback;
            Attach(primary);
            if (primary is IFailingHtmlHost failing)
                failing.Failed += OnPrimaryFailed;
        }

        public Control Control => _container;
        public HtmlEngine Engine => _inner.Engine;
        public bool IsReady => _inner.IsReady;
        public string LoadedHtml => _inner.LoadedHtml;
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;
        public event EventHandler EngineChanged;

        public void LoadHtml(string strHtml)
        {
            _nLoads++;
            _inner.LoadHtml(strHtml);
        }

        public void Post(string strType, object payload = null) => _inner.Post(strType, payload);
        public HtmlMessage Request(string strType, object payload, TimeSpan timeout) => _inner.Request(strType, payload, timeout);
        public void ShowPrintPreview() => _inner.ShowPrintPreview();

        private void Attach(IHtmlHost inner)
        {
            _inner = inner;
            inner.Control.Dock = DockStyle.Fill;
            _container.Controls.Add(inner.Control);
            inner.MessageReceived += OnInnerMessage;
            inner.DocumentReady += OnInnerReady;
        }

        private void Detach(IHtmlHost inner)
        {
            inner.MessageReceived -= OnInnerMessage;
            inner.DocumentReady -= OnInnerReady;
            _container.Controls.Remove(inner.Control);
        }

        private void OnInnerMessage(object sender, HtmlMessage msg) => MessageReceived?.Invoke(this, msg);
        private void OnInnerReady(object sender, EventArgs e) => DocumentReady?.Invoke(this, EventArgs.Empty);

        private void OnPrimaryFailed(object sender, Exception ex)
        {
            if (_bSwapped || _bDisposed)
                return;
            _bSwapped = true;

            var failed = _inner;
            var strLastPage = failed.LoadedHtml;
            if (failed is IFailingHtmlHost failing)
                failing.Failed -= OnPrimaryFailed;

            // the fallback first: if it can't be made, the failed host stays attached (it ignores loads and posts and
            //  answers no requests, as before 'ready'), so the owner is never left holding a disposed host
            IHtmlHost fallback;
            try
            {
                fallback = _createFallback();
            }
            catch (Exception exCreate)
            {
                Debug.WriteLine("FallbackHtmlHost: couldn't create the fallback host: " + exCreate);
                ReportFallback(ex);
                return;
            }

            Detach(failed);
            Attach(fallback);
            try
            {
                failed.Dispose();     // (WebView2HtmlHost raises Failed asynchronously, so disposing here is safe)
            }
            catch (Exception exDispose)
            {
                Debug.WriteLine("FallbackHtmlHost: disposing the failed host threw: " + exDispose);
            }

            // the owner rebuilds its page for the new engine; if it doesn't, replay the last page -- but not a modern
            //  one: its scripts would only throw errors in IE (so a blank page it is)
            var nLoadsBefore = _nLoads;
            try
            {
                EngineChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exOwner)
            {
                Debug.WriteLine("FallbackHtmlHost: an EngineChanged handler threw: " + exOwner);
            }
            if ((_nLoads == nLoadsBefore) && (strLastPage != null) && !_bDisposed)
            {
                if (PageScripts.IsModernPage(strLastPage))
                {
                    Debug.WriteLine("FallbackHtmlHost: the owner didn't rebuild its WebView2 page for IE; showing a blank page");
                    _inner.LoadHtml(String.Empty);
                }
                else
                    _inner.LoadHtml(strLastPage);
            }

            ReportFallback(ex);
        }

        // (persists the failure and gives the notice; nothing it throws, e.g. from Settings.Save, may break the swap)
        private void ReportFallback(Exception ex)
        {
            try
            {
                _onFallback?.Invoke(ex);
            }
            catch (Exception exReport)
            {
                Debug.WriteLine("FallbackHtmlHost: reporting the fallback threw: " + exReport);
            }
        }

        public void Dispose()
        {
            if (_bDisposed)
                return;
            _bDisposed = true;
            _inner.Dispose();
            _container.Dispose();
        }

        // the container gets the focus when an owner calls Control.Focus(); it passes it on to the browser inside
        private sealed class FocusForwardingPanel : Panel
        {
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                if (Controls.Count > 0)
                    Controls[0].Focus();
            }
        }
    }
}
