using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Older versions round-tripped story text through IE's htmlText, which left HTML entities in
    /// the data (e.g. "[B&amp;B]" where the user typed "[B&B]"). Files saved by this version carry
    /// TextEncoding="plain" on the root element and are already clean. Versions that don't know
    /// about the attribute drop it when they save, so their files get cleaned again on the next load.
    /// Note text (ConsultantNote/CoachNote) is HTML and is never touched here.
    /// </summary>
    public static class LegacyTextRepair
    {
        public const string CstrAttributeTextEncoding = "TextEncoding";
        public const string CstrTextEncodingPlain = "plain";

        // just the entities IE's htmlText produced (so the user's own "B&B;" is left alone)
        private static readonly Regex RegexIeEntity =
            new Regex(@"&(?:amp|lt|gt|quot|nbsp|#[0-9]{1,7}|#[xX][0-9A-Fa-f]{1,6});", RegexOptions.Compiled);

        // typed NewDataSet table -> its plain-text columns
        private static readonly Dictionary<string, string[]> PlainTextColumns = new Dictionary<string, string[]>
        {
            { "StoryLine", new[] { "StoryLine_text" } },
            { "Retelling", new[] { "Retelling_text" } },
            { "Answer", new[] { "Answer_text" } },
            { "TestQuestionLine", new[] { "TestQuestionLine_text" } },
            { "ExegeticalHelp", new[] { "ExegeticalHelp_Column" } },
            { "LnCNote", new[] { "LnCNote_text", "VernacularRendering", "NationalBTRendering", "InternationalBTRendering" } },
            { "CraftingInfo", new[] { "StoryPurpose", "ResourcesUsed", "MiscellaneousStoryInfo" } },
            { "TestRetelling", new[] { "TestRetelling_text" } },
            { "TestTqAnswer", new[] { "TestTqAnswer_text" } },
        };

        // the same fields as XML elements (for the copy-story/copy-column clipboard XML)
        private static readonly HashSet<string> PlainTextElementNames = new HashSet<string>
        {
            "StoryLine", "Retelling", "Answer", "TestQuestionLine", "ExegeticalHelp", "LnCNote",
            "StoryPurpose", "ResourcesUsed", "MiscellaneousStoryInfo", "TestRetelling", "TestTqAnswer"
        };

        private static readonly string[] PlainTextAttributeNamesOfLnCNote =
        {
            "VernacularRendering", "NationalBTRendering", "InternationalBTRendering"
        };

        public static string DecodeIeEntities(string str)
        {
            if (String.IsNullOrEmpty(str) || (str.IndexOf('&') < 0))
                return str;
            return RegexIeEntity.Replace(str, m => DecodeEntity(m.Value));
        }

        // a numeric entity that isn't a valid XML character (e.g. "&#0;", "&#xD800;") is left as it is, because
        //  we couldn't save the decoded character in the file
        private static string DecodeEntity(string strEntity)
        {
            if (strEntity[1] == '#')
            {
                var bHex = (strEntity[2] == 'x') || (strEntity[2] == 'X');
                var strDigits = strEntity.Substring(bHex ? 3 : 2, strEntity.Length - (bHex ? 4 : 3));
                int nCodePoint;
                if (!Int32.TryParse(strDigits,
                                    bHex ? NumberStyles.AllowHexSpecifier : NumberStyles.None,
                                    CultureInfo.InvariantCulture, out nCodePoint) ||
                    !IsValidXmlCodePoint(nCodePoint))
                    return strEntity;
            }
            return WebUtility.HtmlDecode(strEntity);
        }

        private static bool IsValidXmlCodePoint(int nCodePoint)
        {
            if ((nCodePoint >= 0x10000) && (nCodePoint <= 0x10FFFF))
                return true;    // supplementary plane
            return (nCodePoint <= 0xFFFF) && XmlConvert.IsXmlChar((char)nCodePoint);
        }

        public static bool ContainsIeEntity(string str)
        {
            return !String.IsNullOrEmpty(str) && RegexIeEntity.IsMatch(str);
        }

        // the textareas that show their language's name (grayed) when empty
        private static readonly string[] TablesWithPlaceholders = { "StoryLine", "Retelling", "Answer", "TestQuestionLine" };

        // StoryBt.js fakes the (IE9-unsupported) placeholder by putting the language name into an empty
        //  textarea, and older versions saved that as if the user had typed it. So a value that is exactly
        //  its column's language name is really an empty field.
        public static int ClearLanguageNamePlaceholders(DataSet ds)
        {
            var tableLanguages = ds.Tables["LanguageInfo"];
            if (tableLanguages == null)
                return 0;

            var mapLanguageNames = new Dictionary<string, string>();
            foreach (DataRow row in tableLanguages.Rows)
            {
                var strLang = row["lang"] as string;
                var strName = (row["name"] as string)?.Trim();
                if (!String.IsNullOrEmpty(strLang) && !String.IsNullOrEmpty(strName))
                    mapLanguageNames[strLang] = strName;
            }

            int nCleared = 0;
            foreach (var strTable in TablesWithPlaceholders)
            {
                var table = ds.Tables[strTable];
                var columnText = table?.Columns[strTable + "_text"];
                var columnLang = table?.Columns["lang"];
                if ((columnText == null) || (columnLang == null))
                    continue;

                foreach (DataRow row in table.Rows)
                {
                    var strText = row[columnText] as string;
                    if (String.IsNullOrEmpty(strText) ||
                        !mapLanguageNames.TryGetValue(row[columnLang] as string ?? String.Empty, out var strName) ||
                        (strText.Trim() != strName))
                        continue;

                    row[columnText] = String.Empty;
                    nCleared++;
                }
            }
            return nCleared;
        }

        public static int DecodePlainTextFields(DataSet ds)
        {
            int nChanged = 0;
            foreach (var kvp in PlainTextColumns)
            {
                var table = ds.Tables[kvp.Key];
                if (table == null)
                    continue;

                foreach (var strColumnName in kvp.Value)
                {
                    var column = table.Columns[strColumnName];
                    if ((column == null) || (column.DataType != typeof(string)))
                        continue;

                    foreach (DataRow row in table.Rows)
                    {
                        if (row.IsNull(column))
                            continue;

                        var str = (string)row[column];
                        var strDecoded = DecodeIeEntities(str);
                        if (strDecoded == str)
                            continue;

                        row[column] = strDecoded;
                        nChanged++;
                    }
                }
            }
            return nChanged;
        }

        public static int DecodePlainTextElements(XElement root)
        {
            int nChanged = 0;
            foreach (var elem in root.DescendantsAndSelf())
            {
                var strName = elem.Name.LocalName;
                if (!PlainTextElementNames.Contains(strName))
                    continue;

                if (!elem.HasElements)
                {
                    var str = elem.Value;
                    var strDecoded = DecodeIeEntities(str);
                    if (strDecoded != str)
                    {
                        elem.Value = strDecoded;
                        nChanged++;
                    }
                }

                if (strName != "LnCNote")
                    continue;

                foreach (var strAttributeName in PlainTextAttributeNamesOfLnCNote)
                {
                    var attr = elem.Attribute(strAttributeName);
                    if (attr == null)
                        continue;
                    var strDecoded = DecodeIeEntities(attr.Value);
                    if (strDecoded == attr.Value)
                        continue;
                    attr.Value = strDecoded;
                    nChanged++;
                }
            }
            return nChanged;
        }

        // same as above for an XmlNode (e.g. a story out of an old revision or one Chorus gave us); decodes it
        //  in place and returns it
        public static XmlNode DecodePlainTextElements(XmlNode node)
        {
            if (node == null)
                return null;

            var elements = new List<XmlElement>();
            if (node is XmlElement)
                elements.Add((XmlElement)node);
            foreach (XmlElement elem in node.SelectNodes(".//*"))
                elements.Add(elem);

            foreach (var elem in elements)
            {
                var strName = elem.LocalName;
                if (!PlainTextElementNames.Contains(strName))
                    continue;

                var bHasChildElements = false;
                foreach (XmlNode child in elem.ChildNodes)
                    if (child.NodeType == XmlNodeType.Element)
                        bHasChildElements = true;

                if (!bHasChildElements)
                {
                    var str = elem.InnerText;
                    var strDecoded = DecodeIeEntities(str);
                    if (strDecoded != str)
                        elem.InnerText = strDecoded;
                }

                if (strName != "LnCNote")
                    continue;

                foreach (var strAttributeName in PlainTextAttributeNamesOfLnCNote)
                {
                    var attr = elem.GetAttributeNode(strAttributeName);
                    if (attr == null)
                        continue;
                    var strDecoded = DecodeIeEntities(attr.Value);
                    if (strDecoded != attr.Value)
                        attr.Value = strDecoded;
                }
            }
            return node;
        }

        public static bool IsMarkedPlain(XmlNode root)
        {
            var elem = root as XmlElement;
            return (elem != null) && (elem.GetAttribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain);
        }

        public static bool IsMarkedPlain(XElement root)
        {
            return (string)root.Attribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain;
        }

        public static void MarkAsPlain(XElement root)
        {
            root.SetAttributeValue(CstrAttributeTextEncoding, CstrTextEncodingPlain);
        }

        public static bool DecodeUnlessMarked(XElement root)
        {
            if (IsMarkedPlain(root))
                return false;
            DecodePlainTextElements(root);
            return true;
        }

        // the typed NewDataSet doesn't know this attribute (neither does the one in older versions,
        //  which is why they can still read our files), so read it straight from the root element
        public static bool IsFileMarkedPlain(string strFilePath)
        {
            // this must never stop a file from loading (DataSet.ReadXml accepts some things that XmlReader doesn't,
            //  e.g. a DOCTYPE): if we can't tell, say it isn't marked, which means it gets decoded (the safe direction)
            try
            {
                using (var reader = XmlReader.Create(strFilePath))
                {
                    reader.MoveToContent();
                    return reader.GetAttribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
