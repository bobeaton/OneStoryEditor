using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// the host HtmlHostFactory hands out when it chooses WebView2: it forwards to the WebView2 host, and if that fails
    /// (start-up or browser process), swaps an IE host into the same container, replays the last page there and raises
    /// EngineChanged so the owner rebuilds the page with the IE scripts. The user never sees a blank pane
    /// </summary>
    public sealed class FallbackHtmlHost : IHtmlHost
    {
        private readonly Panel _container = new Panel { Dock = DockStyle.Fill };
        private readonly Func<IHtmlHost> _createFallback;
        private readonly Action<Exception> _onFallback;
        private IHtmlHost _inner;
        private bool _bSwapped;
        private bool _bDisposed;

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

        public void LoadHtml(string strHtml) => _inner.LoadHtml(strHtml);
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
            Detach(failed);
            failed.Dispose();     // (WebView2HtmlHost raises Failed asynchronously, so disposing here is safe)

            Attach(_createFallback());
            if (strLastPage != null)
                _inner.LoadHtml(strLastPage);     // (the owner rebuilds it with the IE scripts on EngineChanged)

            _onFallback?.Invoke(ex);
            EngineChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_bDisposed)
                return;
            _bDisposed = true;
            _inner.Dispose();
            _container.Dispose();
        }
    }
}
