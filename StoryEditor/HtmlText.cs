using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// The model holds plain text; this is where it gets turned into HTML (and, for what comes
    /// back from IE's htmlText/innerHTML, turned back into plain text).
    /// </summary>
    public static class HtmlText
    {
        private static readonly Regex RegexLineBreak = new Regex(@"\r\n|\n|\r", RegexOptions.Compiled);
        private static readonly Regex RegexIeBr = new Regex(@"<br\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexIeSpan = new Regex(@"</?span\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // only the characters that matter to HTML (non-ASCII text is left as is)
        public static string Encode(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder(str.Length + 16);
            foreach (var ch in str)
                sb.Append(Encode(ch));
            return sb.ToString();
        }

        public static string Encode(char ch)
        {
            switch (ch)
            {
                case '&': return "&amp;";
                case '<': return "&lt;";
                case '>': return "&gt;";
                case '"': return "&quot;";
                default: return ch.ToString();
            }
        }

        // for text that's already HTML (e.g. the output of Diff.HtmlDiff)
        public static string LineBreaksToBr(string html)
        {
            return String.IsNullOrEmpty(html)
                       ? html
                       : RegexLineBreak.Replace(html, "<br />");
        }

        public static string ForParagraph(string str)
        {
            return LineBreaksToBr(Encode(str));
        }

        // IE's htmlText (textarea 'onchange') and innerHTML come back HTML-encoded and may contain
        //  the <span> (selection highlight) and <br> that StoryBt.js put there
        public static string FromIeHtmlText(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = RegexIeBr.Replace(str, "\r\n");
            str = RegexIeSpan.Replace(str, String.Empty);
            return WebUtility.HtmlDecode(str);
        }
    }
}
