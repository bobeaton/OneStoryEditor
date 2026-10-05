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

        // how every modern (WebView2) page starts (PageScriptSet.ApplyDoctype); no IE page does
        internal const string CstrModernDoctype = "<!DOCTYPE html>\r\n";

        public static bool IsModernPage(string strHtml)
        {
            return (strHtml != null) && strHtml.StartsWith(CstrModernDoctype, StringComparison.Ordinal);
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

        // standards mode, unlike IE's quirks mode, doesn't grow a textarea to fit its text (overflow:visible), and adds
        //  the border and padding to width:100% (so a box overflowed its cell). The script half of the growing is
        //  modern/PaneCommon.js's autoSize (for runtimes without field-sizing)
        internal const string CstrModernStyle =
            "<style type=\"text/css\">\r\n" +
            "textarea { box-sizing: border-box; overflow: hidden; resize: none; field-sizing: content; }\r\n" +
            "</style>\r\n";

        // IE: the page as it is (quirks mode, as the pages have always run). WebView2: standards mode, with the
        //  modern style block last in the head (so it wins over the page's own textarea rule)
        public string ApplyDoctype(string strHtml)
        {
            if (IsIe)
                return strHtml;
            var strPage = RegexLeadingDoctype.Replace(strHtml, String.Empty);
            var nHeadEnd = strPage.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            strPage = (nHeadEnd >= 0)
                          ? strPage.Insert(nHeadEnd, CstrModernStyle)
                          : CstrModernStyle + strPage;
            return PageScripts.CstrModernDoctype + strPage;
        }

        // a length written "12 pt" (a space before the unit) or "10,5 pt" (a localized decimal comma)
        private static readonly System.Text.RegularExpressions.Regex RegexSpacedUnit =
            new System.Text.RegularExpressions.Regex(@"(?<int>\d+)(?:[.,](?<frac>\d+))?\s+(?<unit>pt|px|em)(?=\s*[;}""])");

        // a margin with no unit ("margin:1"), other than 0
        private static readonly System.Text.RegularExpressions.Regex RegexUnitlessMargin =
            new System.Text.RegularExpressions.Regex(@"(?<prop>\bmargin(?:-(?:top|right|bottom|left))?\s*:\s*)(?<num>\d*[1-9]\d*(?:\.\d+)?)(?=\s*[;}])");

        // IE: the CSS as it is (quirks mode reads "12 pt" and "margin:1" as the pages have always meant them).
        //  WebView2: standards mode ignores those, so they become "12pt" and "margin:1px"
        public string Css(string strCss)
        {
            if (IsIe || String.IsNullOrEmpty(strCss))
                return strCss;
            strCss = RegexSpacedUnit.Replace(strCss, m => m.Groups["int"].Value +
                                                          (m.Groups["frac"].Success ? "." + m.Groups["frac"].Value : String.Empty) +
                                                          m.Groups["unit"].Value);
            return RegexUnitlessMargin.Replace(strCss, m => m.Groups["prop"].Value + m.Groups["num"].Value + "px");
        }
    }
}
