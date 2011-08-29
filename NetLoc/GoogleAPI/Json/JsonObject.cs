using System;
using System.Text;
using System.Collections.Generic;

namespace Gapi.Json
{
    public class JsonObject : JsonValue
    {
        Dictionary<string, JsonValue> _value;

        public JsonValue this[string key]
        {
            get
            {
                if (key == null)
                    throw new NullReferenceException("key");

                if (this.ContainsKey(key) == false)
                    throw new JsonException("Key does not exists: " + key);

                return (JsonValue)_value[key];
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _value.Keys;
            }
        }

        JsonObject()
        {
            _value = new Dictionary<string, JsonValue>();
        }

        public bool TryGetValue(string key, out JsonValue value)
        {
            value = null;

            if (key == null)
                throw new NullReferenceException("key");

            if (this.ContainsKey(key) == false)
                return false;

            value = this[key];

            return true;
        }

        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new NullReferenceException("key");

            return _value.ContainsKey(key);
        }

        void Add(string key, JsonValue value)
        {
            if (key == null)
                throw new NullReferenceException("key");

            if (ContainsKey(key) == true)
                throw new JsonException(string.Format("Key '{0}' already exists in JsonObject", key));

            _value.Add(key, value);
        }

        public static JsonObject Parse(string str)
        {
            if (str == null)
                return null;

            if (str.Trim().EndsWith("}") == false)
                throw new JsonParseException("Json string does not terminates with '}': " + str);

            int startPos = 0;
            return Parse(str, ref startPos);
        }

        internal static JsonObject Parse(string str, ref int position)
        {
            JsonString.EatSpaces(str, ref position);

            if (position >= str.Length)
                return null;

            if (str[position] != '{')
                throw new JsonParseException(str, position);

            JsonObject jsonObject = new JsonObject();

            // Read all the pairs
            bool continueReading = true;

            // Read starting '{'
            position++;
            while (continueReading == true)
            {
                JsonString.EatSpaces(str, ref position);
                if (str[position] != '}')
                {
                    // Read string
                    JsonString jsonString = JsonString.Parse(str, ref position);
                    string key = jsonString.Value;

                    // Read seperator ':'
                    JsonString.EatSpaces(str, ref position);
                    if (str[position] != ':')
                        throw new JsonParseException(str, position);
                    position++;

                    // Read value
                    JsonValue value = ParseValue(str, ref position);

                    jsonObject.Add(key, value);
                }

                JsonString.EatSpaces(str, ref position);
                if (str[position] == '}')
                    continueReading = false;
                else if (str[position] != ',')
                    throw new JsonParseException(str, position);

                // Skip "," between pair of items
                position++;
            }

            return jsonObject;
        }
    }
}
