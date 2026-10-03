using System;
using System.Collections.Generic;
using System.Data;
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
            return RegexIeEntity.Replace(str, m => WebUtility.HtmlDecode(m.Value));
        }

        public static bool ContainsIeEntity(string str)
        {
            return !String.IsNullOrEmpty(str) && RegexIeEntity.IsMatch(str);
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
            using (var reader = XmlReader.Create(strFilePath))
            {
                reader.MoveToContent();
                return reader.GetAttribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain;
            }
        }
    }
}
