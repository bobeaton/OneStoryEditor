using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// one highlighted selection in a Story/BT textarea, as the page reports it (the 'getHighlights' reply in StoryBt.js).
    /// Replaces the HtmlElement spans C# used to read out of the DOM
    /// </summary>
    public sealed class HighlightedText
    {
        public HighlightedText(string strTextareaId, string strClassName, string strText)
        {
            TextareaId = strTextareaId;
            ClassName = strClassName;
            Text = strText;
        }

        public string TextareaId { get; }
        public string ClassName { get; }    // the span's class, e.g. "LangVernacular highlight"
        public string Text { get; }         // the span's innerText

        public static List<HighlightedText> FromReply(HtmlMessage reply)
        {
            var list = new List<HighlightedText>();
            if (reply?.Body["items"] is JArray items)
                list.AddRange(items.OfType<JObject>()
                                   .Select(item => new HighlightedText((string)item["textareaId"],
                                                                       (string)item["className"],
                                                                       (string)item["text"])));
            return list;
        }
    }
}
