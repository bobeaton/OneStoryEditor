using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Chorus.Utilities;
using Chorus.merge.xml.generic;

namespace AdaptIt_ChorusPlugin
{
    class AdaptItContextGenerator : IGenerateContextDescriptor, IGenerateHtmlContext
    {
        private string _strType;
        private string _strKeyName;

        public AdaptItContextGenerator(string strType, string strKeyName)
        {
            _strType = strType;
            _strKeyName = strKeyName;
        }

        public ContextDescriptor GenerateContextDescriptor(string mergeElement, string filePath)
        {
            var doc = XDocument.Parse(mergeElement);
            Debug.Assert((doc.Root != null) && (String.IsNullOrEmpty(_strKeyName) || doc.Root.HasAttributes));
            var attr = (!String.IsNullOrEmpty(_strKeyName)) ? doc.Root.Attribute(_strKeyName) : null;

            return new ContextDescriptor(_strType, GetUrl(Path.GetFileNameWithoutExtension(filePath),
                                                          _strType,
                                                          (attr != null) ? attr.Value : null));
        }

        private readonly char[] _achEndOfElement = new[] { '>', ' ' };

        public string HtmlContext(XmlNode mergeElement)
        {
            if (mergeElement is XmlText)
                return mergeElement.Value;

            var strXml = mergeElement.OuterXml;

            // if this element has children (i mean besides a text field)...
            if (mergeElement.HasChildNodes && (mergeElement.FirstChild.LocalName != "#text"))
            {
                var nIndex = strXml.IndexOf('>');
                var strPrefixElement = strXml.Substring(0, nIndex + 1);

                nIndex = strXml.LastIndexOf('<');
                var strSuffixElement = strXml.Substring(nIndex);

                // we can have thousands of children... so just do 10 and leave it at that
                var nMaxToShow = 10;
                string strChildren = null;
                foreach (XmlNode child in mergeElement.ChildNodes)
                {
                    var nIndexName = child.OuterXml.IndexOfAny(_achEndOfElement);
                    var strName = child.OuterXml.Substring(1, nIndexName - 1);
                    strChildren += Environment.NewLine + "  <" + strName;

                    if ((child.Attributes != null) && (child.Attributes.Count > 0))
                    {
                        nIndex = child.OuterXml.IndexOf('>', nIndexName);
                        if (child.OuterXml[nIndex - 1] == '/')
                            nIndex--;
                        strChildren += child.OuterXml.Substring(nIndexName, nIndex - nIndexName);
                    }

                    strChildren += ">...</" + strName + ">";

                    if (--nMaxToShow < 0)
                    {
                        strChildren += Environment.NewLine + "  ...";
                        break;
                    }
                }

                strXml = String.Format("{1}{2}{0}{3}",
                                       Environment.NewLine, strPrefixElement, strChildren, strSuffixElement);
            }
            try
            {
                return XmlUtilities.GetXmlForShowingInHtml(strXml);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("exception: ", ex.Message);
                return UrlHelper.GetEscapedUrl(strXml);
            }
        }

        public string HtmlContextStyles(XmlNode mergeElement)
        {
            return "div.alternative {margin-left:  0.25in} div.ws {margin-left:  0.25in} div.property {margin-left:  0.25in} div.checksum {margin-left:  0.25in}";
        }

        // e.g. 
        //  adaptit://Greek%20to%20English%20adaptations?type=KB
        //  adaptit://Greek%20to%20English%20adaptations?type=MAP&key=1
        //  adaptit://Greek%20to%20English%20adaptations?type=TU&key=κν
        //  adaptit://Greek%20to%20English%20adaptations?type=RS&key=σδγαδγ
        public static string GetUrl(string strFolderName, string strType, string strKey)
        {
            var str = String.Format("adaptit://{0}?type={1}",
                                    Uri.EscapeDataString(strFolderName), strType);

            if (!String.IsNullOrEmpty(strKey))
                str += String.Format("&key={0}", Uri.EscapeDataString(strKey));

            return str;
        }
    }
}
