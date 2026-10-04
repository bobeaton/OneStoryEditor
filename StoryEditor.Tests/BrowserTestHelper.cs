using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace OneStoryProjectEditor.Tests
{
    internal static class BrowserTestHelper
    {
        private static bool _bEmulationSet;

        // the test runner isn't StoryEditor.exe, so put its own exe into IE9 mode (once, before the first browser)
        public static void EnsureIe9ModeForTestProcess()
        {
            if (_bEmulationSet)
                return;
            IeHtmlHost.EnsureIe9Mode(Process.GetCurrentProcess().ProcessName + ".exe");
            _bEmulationSet = true;
        }

        public static bool PumpUntil(Func<bool> condition, int nTimeoutMs = 10000)
        {
            var sw = Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > nTimeoutMs)
                    return false;
                Application.DoEvents();
                Thread.Sleep(5);
            }
            return true;
        }

        public static void Pump(int nMs)
        {
            PumpUntil(() => false, nMs);
        }

        public static IeHtmlHost CreateHostInOffscreenForm(out Form form)
        {
            EnsureIe9ModeForTestProcess();
            form = new Form
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-32000, -32000),
                Size = new Size(800, 600),
                ShowInTaskbar = false
            };
            var host = new IeHtmlHost(new HtmlHostOptions());
            form.Controls.Add(host.Control);
            form.Show();
            return host;
        }

        // a standards-mode (IE9) page
        public static string Page(string strBodyHtml, params string[] astrScripts)
        {
            return "<!DOCTYPE html>" + QuirksPage(strBodyHtml, astrScripts);
        }

        // without a doctype IE renders in quirks mode (documentMode 5), as the real pane pages do
        public static string QuirksPage(string strBodyHtml, params string[] astrScripts)
        {
            return "<html><head>" + PageScripts.ScriptBlock(astrScripts) + "</head><body>" + strBodyHtml + "</body></html>";
        }

        // test-driver JS: fires a DOM event of the given type at el in either document mode
        public const string CstrFireEventScript =
            "function oseFire(el, type) { if (document.createEvent) { var ev = document.createEvent('Event'); ev.initEvent(type, true, true);" +
            "  el.dispatchEvent(ev); } else el.fireEvent('on' + type); }";
    }
}
