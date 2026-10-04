using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Parsing helpers for loading a project file from XElements. Each helper reproduces what the
    /// typed DataSet (NewDataSet) used to yield for the same attribute or element text; this is
    /// verified by XmlReadCharacterizationTests, which uses the DataSet as the oracle.
    /// </summary>
    public static class XmlRead
    {
        public static string Attr(XElement e, string name)
        {
            var attr = e.Attribute(name);
            return attr?.Value;
        }

        public static string RequiredAttr(XElement e, string name)
        {
            var strValue = Attr(e, name);
            if (strValue == null)
                throw new ApplicationException(
                    $"The project file is damaged: <{e.Name}> is missing the required attribute '{name}'.");
            return strValue;
        }

        public static bool? Bool(XElement e, string name)
        {
            var str = Attr(e, name);
            return (str == null) ? (bool?)null : ParseBool(str);
        }

        public static bool Bool(XElement e, string name, bool defaultValue)
        {
            return Bool(e, name) ?? defaultValue;
        }

        private static bool ParseBool(string str)
        {
            // the DataSet uses XmlConvert.ToBoolean, so "True" is rejected (FormatException), as here
            return XmlConvert.ToBoolean(str);
        }

        public static int Int(XElement e, string name, int defaultValue)
        {
            var str = Attr(e, name);
            return (str == null) ? defaultValue : XmlConvert.ToInt32(str);
        }

        public static float? Float(XElement e, string name)
        {
            var str = Attr(e, name);
            return (str == null) ? (float?)null : XmlConvert.ToSingle(str);
        }

        public static DateTime? Date(XElement e, string name)
        {
            var str = Attr(e, name);
            if (str == null)
                return null;

            // The DataSet yields Kind=Unspecified for every form: a trailing 'Z' keeps its clock reading,
            //  an explicit offset is converted to this machine's local time, and no zone is left as is.
            //  RoundtripKind gives exactly those readings (Utc / Local / Unspecified); only the Kind differs.
            var dt = XmlConvert.ToDateTime(str, XmlDateTimeSerializationMode.RoundtripKind);
            return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        }

        // put on an element whose value ClearLanguageNamePlaceholders emptied: the DataSet holds "" there (not null)
        internal sealed class ClearedPlaceholder
        {
            public static readonly ClearedPlaceholder Instance = new ClearedPlaceholder();
        }

        public static string Text(XElement e)
        {
            if (e.Annotation<ClearedPlaceholder>() != null)
                return String.Empty;

            // The DataSet yields null (never "") for a self-closing, empty or whitespace-only element
            //  and the exact text (padding included) otherwise.
            var str = e.Value;
            return (str.Trim(' ', '\t', '\r', '\n').Length == 0) ? null : str;
        }

        public static XElement First(XElement parent, string name)
        {
            return parent.Element(name);
        }

        public static IEnumerable<XElement> Children(XElement parent, string name)
        {
            return parent.Elements(name);
        }
    }
}
