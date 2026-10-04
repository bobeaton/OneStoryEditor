using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// the JS files embedded with LogicalName "OneStoryProjectEditor.js.&lt;file&gt;" (see StoryEditor.csproj)
    /// </summary>
    internal static class PageScripts
    {
        private const string CstrResourcePrefix = "OneStoryProjectEditor.js.";
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        public static string Bridge => Get("bridge.js");

        public static PageScriptSet For(HtmlEngine engine)
        {
            return new PageScriptSet(engine);
        }

        public static string Get(string strFileName)
        {
            return Cache.GetOrAdd(strFileName, name =>
            {
                using (var stream = typeof(PageScripts).Assembly.GetManifestResourceStream(CstrResourcePrefix + name))
                {
                    if (stream == null)
                        throw new InvalidOperationException("missing embedded script " + name);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            });
        }

        public static string ScriptBlock(params string[] astrScripts)
        {
            return "<script type=\"text/javascript\">" + Environment.NewLine +
                   String.Join(Environment.NewLine, astrScripts) + Environment.NewLine +
                   "</script>";
        }
    }

    /// <summary>
    /// the page scripts for one engine. IE: today's quirks-mode scripts, untouched. WebView2: js/modern/*, standards
    /// mode. Both speak the same message protocol (docs/superpowers/specs/2026-10-04-webview2-host-design.md)
    /// </summary>
    internal sealed class PageScriptSet
    {
        private static readonly System.Text.RegularExpressions.Regex RegexLeadingDoctype =
            new System.Text.RegularExpressions.Regex(@"^\s*<!DOCTYPE[^>]*>\s*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public PageScriptSet(HtmlEngine engine)
        {
            Engine = engine;
        }

        public HtmlEngine Engine { get; }
        private bool IsIe => Engine == HtmlEngine.Ie;

        public string Bridge => IsIe ? PageScripts.Get("bridge.js") : PageScripts.Get("modern.bridge.js");
        public string PaneCommon => IsIe ? PageScripts.Get("PaneCommon.js") : PageScripts.Get("modern.PaneCommon.js");
        public string StoryBt => IsIe ? Properties.Resources.StoryBtJs : PageScripts.Get("modern.StoryBt.js");
        public string StoryBtEnd => IsIe ? Properties.Resources.StoryBtPsJs : String.Empty;   // (modern: all in StoryBt)
        public string ConNote => IsIe ? Properties.Resources.ConNoteDomPrefix : PageScripts.Get("modern.ConNote.js");
        public string NetBible => IsIe ? PageScripts.Get("NetBible.js") : PageScripts.Get("modern.NetBible.js");
        public string HoverLinks => IsIe ? PageScripts.Get("HoverLinks.js") : PageScripts.Get("modern.HoverLinks.js");
        public string JQuery => IsIe ? Properties.Resources.jquery_min : String.Empty;

        // IE: the page as it is (quirks mode, as the pages have always run). WebView2: standards mode
        public string ApplyDoctype(string strHtml)
        {
            if (IsIe)
                return strHtml;
            return "<!DOCTYPE html>\r\n" + RegexLeadingDoctype.Replace(strHtml, String.Empty);
        }
    }
}
