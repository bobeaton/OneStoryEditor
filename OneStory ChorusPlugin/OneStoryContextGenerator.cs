using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Chorus.Utilities;
using Chorus.merge.xml.generic;

namespace OneStory_ChorusPlugin
{
    class OneStoryContextGenerator : IGenerateContextDescriptor, IGenerateHtmlContext
    {
        private string _strType;
        private string _strKeyName;

        public OneStoryContextGenerator(string strType, string strKeyName)
        {
            _strType = strType;
            _strKeyName = strKeyName;
        }

        public OneStoryContextGenerator(string strType)
          : this(strType, null)
        {
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

        private readonly char[] _achEndOfElement = new[] {'>', ' '};

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

        protected const string CstrOneStoryUrlHeader = "onestory://";
        protected const string CstrStorySet = "StorySet";
        protected const string CstrStoryId = "StoryId";
        protected const string CstrLineId = "LineId";
        protected const string CstrType = "Type";
        protected const string CstrTypeId = "TypeId";
        protected const string CstrTypeValue = "TypeValue";

        // OneStory URL syntax
        // onestory://<projectname>?StorySet=<storysetname>&StoryId=<storyguid>&LineId=<verseguid>&Type=<fieldtype>&TypeId=<typeguid>&TypeValue=<string>
        // where:
        //  projectname = e.g. snwmtn-test
        //  storysetname = e.g. "Stories" (for the main biblical stories)
        //  fieldtype = oneof{Vernacular, NationalBT, EnglishBT, Anchor, ...}
        //  num = which instances (e.g. the 1st retelling = 0)
        //  string = portion of the phrase/string that was selected
        // this method is for constructing a single FieldInfo item
        // onestory://<projectname>?type=<elemType>(&key=<guid>)
        // e.g. 
        //  onestory://lb1-hindi?type=StoryProject
        //  onestory://lb1-hindi?type=Story&key=King%20David
        //  onestory://lb1-hindi?type=StorySet&key=Stories
        //  onestory://lb1-hindi?type=Line&key=630f6ddf-2709-4321-a7e0-eca623faced6
        public static string GetUrl(string strProjectName, string strType, string strKey)
        {
            var str = String.Format("onestory://{0}?type={1}",
                                    strProjectName, strType);

            if (!String.IsNullOrEmpty(strKey))
                str += String.Format("&key={0}", Uri.EscapeDataString(strKey));
            
            return str;
        }

        [Flags]
        public enum UrlFields
        {
            Undefined = 0,

            // each field will be one of these 4
            Vernacular = 1,
            NationalBt = 2,
            InternationalBt = 4,
            FreeTranslation = 8,

            // and one of these
            StoryLine = 16,
            Anchor = 32,
            ExegeticalNote = 64,
            Retelling = 128,
            TestQuestion = 256,
            TestQuestionAnswer = 512,
            ConsultantNote = 1024,
            CoachNote = 2048,

            // meta data
            Member = 8192,                  // @memberKey = <guid>
            Language = 16384,               // @lang = oneof { Vernacular, NationalBt, InternationalBt, FreeTranslation }
            AdaptItConfiguration = 32768,   // @BtDirection = oneof { VernacularToNationalBt, VernacularToInternationalBt, NationalBtToInternationalBt}
            LnCNote = 65536,                // @guid = <guid>
            StorySet = 131072,              // @SetName = oneof { Stories, Non-Biblical Stories, Old Stories }
            Story = 262144,                 // @guid = <guid>
            Line = 524288                   // @guid = <guid>
        }
    }
}
