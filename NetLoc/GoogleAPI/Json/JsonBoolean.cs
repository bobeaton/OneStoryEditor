using System;
using System.Collections.Generic;
using System.Text;

namespace Gapi.Json
{
    public class JsonBoolean : JsonValue
    {
        bool _value;

        public bool Value
        {
            get { return _value; }
        }

        public JsonBoolean(bool value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
