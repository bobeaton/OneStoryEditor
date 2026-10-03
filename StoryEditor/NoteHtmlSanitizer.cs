using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Note text (ConsultantNote/CoachNote, incl. ReferringText) is legacy HTML: "create note" spans,
    /// typed <B>/<EM>, and sometimes whole chunks of the pane's own HTML (even a <SCRIPT>) that got
    /// captured over the years. It's sanitized only when it's rendered; the stored text is never changed.
    /// </summary>
    public static class NoteHtmlSanitizer
    {
        private static readonly string[] AllowedTagNames = { "span", "p", "br", "i", "b", "em", "strong", "u", "a" };

        // the language/field classes the app generates (seen in the stored "create note" spans)
        private static readonly string[] AllowedClassNames =
        {
            "LangVernacular", "LangNationalBt", "LangNationalBT", "LangInternationalBt", "LangInternationalBT",
            "LangFreeTranslation", "StoryLine", "Anchor", "ExegeticalNote", "Retelling", "TestQuestion",
            "TestQuestionAnswer", "LocalizationStyle"
        };

        // tags we recognize as HTML (allowed ones, or ones the sanitizer removes). Anything else that
        //  looks like a tag (e.g. "<RTL>", "<malti see note>") is something a user typed: show it as text
        private static readonly HashSet<string> KnownHtmlTagNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "span", "p", "br", "i", "b", "em", "strong", "u", "a",
            "div", "font", "script", "style", "textarea", "input", "button", "select", "option", "label", "form",
            "table", "thead", "tbody", "tfoot", "tr", "td", "th", "caption", "col", "colgroup",
            "img", "iframe", "object", "embed", "link", "meta", "html", "head", "body", "title", "base",
            "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li", "dl", "dt", "dd", "pre", "code", "blockquote",
            "sup", "sub", "small", "big", "center", "hr", "nobr", "strike", "del", "ins", "tt", "abbr", "cite", "q"
        };

        private static readonly HashSet<string> DropWithContentNodeNames = new HashSet<string> { "SCRIPT", "STYLE" };

        // links SetHyperlinks made (and which got captured into the note); it makes them again from the text
        private static readonly Regex RegexInternalLink = new Regex(
            @"<a\b[^>]*?\bhref\s*=\s*[""']?(?:bibleViewer\.setReference|conNote\.jumpToLine)[""']?[^>]*>(.*?)</a\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RegexTagLike = new Regex(@"<(/?)([A-Za-z][A-Za-z0-9:_-]*)([^<>]*)>", RegexOptions.Compiled);
        private static readonly Regex RegexHttpLink = new Regex(@"<a href=""(https?:[^""]*)"">", RegexOptions.Compiled);

        private const char HighlightBeginSentinel = '\uE000';
        private const char HighlightEndSentinel = '\uE001';

        private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();
        private static readonly object SanitizerLock = new object();

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer { KeepChildNodes = true };

            sanitizer.AllowedTags.Clear();
            foreach (var strTag in AllowedTagNames)
                sanitizer.AllowedTags.Add(strTag);

            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("href");

            sanitizer.AllowedClasses.Clear();
            foreach (var strClass in AllowedClassNames)
                sanitizer.AllowedClasses.Add(strClass);

            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");

            // KeepChildNodes would otherwise leave the script's code behind as text
            sanitizer.RemovingTag += (sender, e) =>
            {
                if (DropWithContentNodeNames.Contains(e.Tag.NodeName))
                    e.Tag.TextContent = String.Empty;
            };

            return sanitizer;
        }

        public static bool TrySanitize(string html, out string result)
        {
            result = html;
            if (String.IsNullOrEmpty(html))
                return true;

            try
            {
                var str = RegexInternalLink.Replace(html, "$1");
                str = RegexTagLike.Replace(str, m => KnownHtmlTagNames.Contains(m.Groups[2].Value)
                                                         ? m.Value
                                                         : HtmlText.Encode(m.Value));
                lock (SanitizerLock)
                    str = Sanitizer.Sanitize(str);
                result = RegexHttpLink.Replace(str, "<a href=\"$1\" onClick=\"return OnUrlJump(this);\">");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NoteHtmlSanitizer: showing note as plain text because: {ex.Message}");
                result = HtmlText.Encode(html);
                return false;
            }
        }

        public static string Sanitize(string html)
        {
            TrySanitize(html, out var result);
            return result;
        }

        // n.b. the HTML parser turns "\r\n" into "\n", so the line breaks are converted afterwards
        public static string ToReadOnlyHtml(string raw)
        {
            return HtmlText.LineBreaksToBr(Sanitize(raw));
        }

        // for search: highlight [nIndex, nIndex+nLength) of the raw note text
        public static string ToReadOnlyHtmlWithHighlight(string raw, int nIndex, int nLength,
            string strBegin, string strEnd)
        {
            if (String.IsNullOrEmpty(raw) || (nIndex < 0) || (nLength <= 0) || (nIndex + nLength > raw.Length))
                return ToReadOnlyHtml(raw);

            var strBeginSentinel = HighlightBeginSentinel.ToString();
            var strEndSentinel = HighlightEndSentinel.ToString();
            var strUnhighlighted = ToReadOnlyHtml(raw);
            var html = ToReadOnlyHtml(raw.Insert(nIndex + nLength, strEndSentinel)
                                         .Insert(nIndex, strBeginSentinel));

            // if the range overlapped markup, the sentinels change how it's parsed (or get dropped).
            //  Only highlight if, apart from the sentinels, it renders exactly as without them
            if ((html.IndexOf(HighlightBeginSentinel) < 0) || (html.IndexOf(HighlightEndSentinel) < 0) ||
                (html.Replace(strBeginSentinel, String.Empty).Replace(strEndSentinel, String.Empty) != strUnhighlighted))
                return strUnhighlighted;

            return html.Replace(strBeginSentinel, strBegin)
                       .Replace(strEndSentinel, strEnd);
        }
    }
}
