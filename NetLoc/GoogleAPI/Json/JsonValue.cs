using System;
using System.Text;

namespace Gapi.Json
{
    public class JsonValue
    {
        protected static void EatSpaces(string str, ref int position)
        {

            while ((position < str.Length) &&
                    ((str[position] == ' ') || (str[position] == '\n'))
                  )
                position++;
        }

        protected static JsonValue ParseValue(string str, ref int position)
        {
            JsonString.EatSpaces(str, ref position);

            char ch = str[position];

            // JsonString
            if (ch == '\"')
                return JsonString.Parse(str, ref position);
            // JsonObject
            else if (ch == '{')
                return JsonObject.Parse(str, ref position);
            // JsonArray
            else if (ch == '[')
                return JsonArray.Parse(str, ref position);
            // JsonNumber
            else if (JsonNumber.IsNumberPart(ch))
                return JsonNumber.Parse(str, ref position);
            // 'null'
            else if ((str.Length > position + 4) &&
                (str.Substring(position, 4).Equals("null", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 4;
                return null;
            }
            // 'true'
            else if ((str.Length > position + 4) &&
                (str.Substring(position, 4).Equals("true", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 4;
                return new JsonBoolean(true);
            }
            // 'false'
            else if ((str.Length > position + 5) &&
                (str.Substring(position, 5).Equals("false", StringComparison.InvariantCultureIgnoreCase)))
            {
                position += 5;
                return new JsonBoolean(false);
            }
            else
                throw new JsonParseException(str, position);
        }
    }
}
