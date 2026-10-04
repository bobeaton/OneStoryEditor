using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// one message between an HTML page and its host (see js/bridge.js): a flat JSON object with a string 'type'
    /// </summary>
    public class HtmlMessage
    {
        public const string CstrTypeReply = "reply";
        public const string CstrTypeReady = "ready";
        public const string CstrTypeJsError = "jsError";
        public const string CstrTypeLog = "log";

        public string Type { get; }
        public JObject Body { get; }

        private HtmlMessage(string type, JObject body)
        {
            Type = type;
            Body = body;
        }

        public static HtmlMessage Create(string type, object payload = null)
        {
            var body = (payload == null) ? new JObject() : JObject.FromObject(payload);
            body["type"] = type;
            return new HtmlMessage(type, body);
        }

        // null if the string isn't a JSON object with a string 'type'
        public static HtmlMessage TryParse(string json)
        {
            if (String.IsNullOrEmpty(json))
                return null;
            try
            {
                JObject body;
                // DateParseHandling.None: text that looks like a date (e.g. "2021-03-04T12:00") stays a string
                using (var reader = new JsonTextReader(new StringReader(json)) { DateParseHandling = DateParseHandling.None })
                {
                    body = JObject.Load(reader);
                    while (reader.Read())
                    {
                        if (reader.TokenType != JsonToken.Comment)
                            return null;    // something after the object (JObject.Parse rejects that too)
                    }
                }
                var tokType = body["type"];
                if ((tokType == null) || (tokType.Type != JTokenType.String))
                    return null;
                return new HtmlMessage((string)tokType, body);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public HtmlMessage WithRid(int nRid)
        {
            Body["rid"] = nRid;
            return this;
        }

        public int? Rid => TryGetInt("rid", out var n) ? n : (int?)null;
        public int? ReplyTo => TryGetInt("re", out var n) ? n : (int?)null;
        public string DocId => GetString("doc");

        public string GetString(string strName)
        {
            var tok = Body[strName];
            return ((tok != null) && (tok.Type == JTokenType.String)) ? (string)tok : null;
        }

        // element ids and 'name' attributes arrive as strings, so numeric strings count too
        public bool TryGetInt(string strName, out int nValue)
        {
            nValue = 0;
            var tok = Body[strName];
            if (tok == null)
                return false;
            if (tok.Type == JTokenType.Integer)
            {
                nValue = (int)tok;
                return true;
            }
            return (tok.Type == JTokenType.String) && Int32.TryParse((string)tok, out nValue);
        }

        public bool GetBool(string strName, bool bDefault = false)
        {
            var tok = Body[strName];
            if (tok == null)
                return bDefault;
            if (tok.Type == JTokenType.Boolean)
                return (bool)tok;
            if ((tok.Type == JTokenType.String) && Boolean.TryParse((string)tok, out var b))
                return b;
            return bDefault;
        }

        public string ToJson()
        {
            return Body.ToString(Formatting.None);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
