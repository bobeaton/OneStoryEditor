using System;
using System.Text;

namespace Gapi.Json
{
    public class JsonException : Exception
    {
        public JsonException(string message)
            : base(message)
        {
        }        
    }

    public class JsonParseException : JsonException
    {
        public JsonParseException(string message)
            : base(message)
        {
        }

        public JsonParseException(string input, int position)
            : this(string.Format("Unexpected character '{0}' at position {1}, input: '{2}'", input[position], position, input))
        {
        }
    }
}
