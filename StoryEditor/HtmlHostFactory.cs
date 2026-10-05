using System;
using System.Runtime.CompilerServices;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// chooses the browser engine for every HTML host: WebView2 where its runtime works (wrapped so it falls back to
    /// IE if it fails), otherwise IE. See docs/superpowers/specs/2026-10-04-webview2-host-design.md
    /// </summary>
    public static class HtmlHostFactory
    {
        // on: every page has its modern scripts (plan Task 7)
        internal static bool UseWebView2WhenAvailable = true;

        private static bool _bNoticeGiven;

        // once per session: why IE is being used, or that WebView2 failed and IE took over
        public static event EventHandler<string> Notice;
        public static string LastNotice { get; private set; }

        public static IHtmlHost Create(HtmlHostOptions options = null)
        {
            options = options ?? new HtmlHostOptions();
            if (!UseWebView2WhenAvailable)
                return new IeHtmlHost(options);

            // n.b. this method must not mention WebView2HtmlHost: the JIT loads the WebView2 assembly when it compiles a
            // method that does, and a missing or mismatched assembly has to be caught below, not crash here
            IHtmlHost host = null;
            try
            {
                host = WebView2Creator(options);
            }
            catch (Exception ex) when (ex is System.IO.FileNotFoundException || ex is System.IO.FileLoadException ||
                                       ex is TypeLoadException || ex is BadImageFormatException)
            {
                GiveNotice("Using IE browser panes: the WebView2 components could not be loaded (" + ex.Message + ")");
            }
            return host ?? new IeHtmlHost(options);
        }

        // test seam: replaced by a creator that throws, to check the fallback
        internal static Func<HtmlHostOptions, IHtmlHost> WebView2Creator = CreateWebView2;

        // null = use IE (the notice has been given)
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IHtmlHost CreateWebView2(HtmlHostOptions options)
        {
            var strWhyNot = WhyNotWebView2(Settings.Default.UseIeBrowserPanes, WebView2HtmlHost.RuntimeVersion,
                                           Settings.Default.WebView2FailedRuntimeVersion);
            if (strWhyNot != null)
            {
                GiveNotice("Using IE browser panes: " + strWhyNot);
                return null;
            }

            return new FallbackHtmlHost(new WebView2HtmlHost(options), () => new IeHtmlHost(options), OnWebView2Failed);
        }

        internal static void ResetNoticeForTests()
        {
            _bNoticeGiven = false;
            LastNotice = null;
        }

        // null = use WebView2; otherwise the reason not to (for the status bar)
        internal static string WhyNotWebView2(bool bUseIeSetting, string strRuntimeVersion, string strFailedRuntimeVersion)
        {
            if (bUseIeSetting)
                return "the 'Use the IE browser panes' setting is on";
            if (String.IsNullOrEmpty(strRuntimeVersion))
                return "the WebView2 runtime isn't installed";
            if (strFailedRuntimeVersion == strRuntimeVersion)
                return "WebView2 failed to start last time";
            return null;    // (a failure on an older runtime doesn't count: an update may have fixed it)
        }

        private static void OnWebView2Failed(Exception ex)
        {
            Settings.Default.WebView2FailedRuntimeVersion = WebView2HtmlHost.RuntimeVersion ?? String.Empty;
            Settings.Default.Save();
            GiveNotice("WebView2 stopped working (" + ex.Message + "), so IE browser panes are being used");
        }

        private static void GiveNotice(string strNotice)
        {
            if (_bNoticeGiven)
                return;
            _bNoticeGiven = true;
            LastNotice = strNotice;
            Notice?.Invoke(null, strNotice);
        }
    }
}
