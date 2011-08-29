using System;
using System.Text;

namespace Gapi.Json
{
    public class JsonNumber : JsonValue 
    {
        double _value;

        public int IntValue
        {
            get { return (int)_value; }
        }

        public double DoubleValue
        {
            get { return _value; }
        }

        public JsonNumber(double value)
        {
            _value = value;
        }

        public static bool IsNumberPart(char ch)
        {
            return (char.IsDigit(ch) || (ch == '-') || (ch == '+')
                || (ch == '.') || (ch == 'e') || (ch == 'E'));
        }

        internal static JsonNumber Parse(string str, ref int position)
        {
            double value;

            JsonString.EatSpaces(str, ref position);

            int startPos = position;
            while (IsNumberPart(str[position]))
                position++;

            value = double.Parse(str.Substring(startPos, position - startPos));

            return new JsonNumber(value);
        }

        public override string ToString()
        {
            return string.Format("{0}", this.DoubleValue);
        }
    }
}
