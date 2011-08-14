using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace EditResxLocalization
{
    public class EditableLocalization
    {
        [XmlAttribute("version")]
        public string XmlVersion
        {
            get { return _xmlVersion; }
            set
            {
                if (value.CompareTo(_xmlVersion) > 0)
                    throw new ApplicationException(Properties.Resources.IDS_NewVersion);
            }
        }

        public Languages Languages { get; set; }

        public List<Entry> Entries { get; set; }

        #region EditableLocalization_NonAttributesElements
        private string _xmlVersion = "1.0";

        [XmlIgnore]
        public string Path2Elf;

        // used by XmlSerializer.Serialize
        public EditableLocalization() { }

        public EditableLocalization(bool bInitialize)
        {
            Languages = new Languages
                            {
                                new Language
                                    {
                                        Id = "en", 
                                        Name = "English", 
                                        SendEmailOnUpdate = new List<string>
                                                                {
                                                                    "bob_eaton@sall.com"
                                                                }
                                    }
                            };
            Entries = new List<Entry>();
        }

        public static EditableLocalization GetEditableLocalizations(string strPath2Elf)
        {
            if (!String.IsNullOrEmpty(strPath2Elf) && File.Exists(strPath2Elf))
            {
                var serializer = new XmlSerializer(typeof(EditableLocalization));
                try
                {
                    var strFileContents = File.ReadAllText(strPath2Elf);
                    var stringReader = new StringReader(strFileContents);
                    var elf = (EditableLocalization)serializer.Deserialize(stringReader);
                    elf.Path2Elf = strPath2Elf;
                    return elf;
                }
                catch (ApplicationException ex)
                {
                    Program.ShowException(ex);
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
            }

            return new EditableLocalization(true);
        }

        public static EditableLocalization CreateEditableLocalizationsFromString(string strElfAsString)
        {
            if (!String.IsNullOrEmpty(strElfAsString))
            {
                var serializer = new XmlSerializer(typeof(EditableLocalization));
                try
                {
                    return (EditableLocalization)serializer.Deserialize(new StringReader(strElfAsString));
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
            }

            return new EditableLocalization(true);
        }

        public void SaveEditableLocalizations()
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(Path2Elf));
            SaveEditableLocalizationFile(Path2Elf);
        }

        private void SaveEditableLocalizationFile(string strPath2Elf)
        {
            var serializer = new XmlSerializer(typeof (EditableLocalization));
            var xmlWriter = new XmlTextWriter(strPath2Elf, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(xmlWriter, this, ns);
            xmlWriter.Close();
        }

        public override string ToString()
        {
            var serializer = new XmlSerializer(typeof(EditableLocalization));
            var strBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(strBuilder, settings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(writer, this, ns);
            }
            return strBuilder.ToString();
        }

        public string MergeXmlFragment(string strXmlFragment)
        {
            // get the xml fragment into a memory stream so it can be the 
            //  input to the transformer
            var streamData = new MemoryStream(Encoding.UTF8.GetBytes(strXmlFragment));

            // write the formatted XSLT to another memory stream.
            var strXslt = Properties.Resources.elfMerge;
            string strPath = Path2Elf;
            if (String.IsNullOrEmpty(strPath) || !File.Exists(strPath))
            {
                strPath = Path.GetTempFileName();
                SaveEditableLocalizationFile(strPath);
            }
            strXslt = strXslt.Replace("{0}", strPath);
            var streamXslt = new MemoryStream(Encoding.UTF8.GetBytes(strXslt));

            return TransformXml(streamXslt, streamData);
        }

        protected string TransformXml(Stream streamXslt, Stream streamData)
        {
            var myProcessor = new XslCompiledTransform();
            var xslReader = XmlReader.Create(streamXslt, new XmlReaderSettings { ProhibitDtd = false });
            var xsltSettings = new XsltSettings { EnableDocumentFunction = true };
            myProcessor.Load(xslReader, xsltSettings, null);

            // rewind
            streamData.Seek(0, SeekOrigin.Begin);

            var reader = XmlReader.Create(streamData);
            var strBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var writer = XmlWriter.Create(strBuilder, settings);
            myProcessor.Transform(reader, writer);

            return strBuilder.ToString();
        }
        #endregion EditableLocalization_NonAttributesElements
    }

    public class Entry
    {
        [XmlAttribute("Id")]
        public string Id;

        [XmlAttribute("obsolete")]
        public string obsolete { get; set; }

        [XmlAttribute("hidden")]
        public string hidden { get; set; }

        [XmlElement("Value")]
        public Values Values { get; set; }

        #region Entry_NonAttributesElements
        [XmlIgnore]
        public bool Obsolete
        {
            get { return (obsolete == "true"); }
            set { obsolete = (value) ? "true" : null; }
        }

        [XmlIgnore]
        public bool Hidden
        {
            get { return (hidden == "true"); }
            set { hidden = (value) ? "true" : null; }
        }
        #endregion Entry_NonAttributesElements
    }

    public class Values : List<Value>
    {
        public Value this[string strId]
        {
            get { return this.FirstOrDefault(value => value.Lang == strId); }
        }
    }

    public class Value
    {
        [XmlAttribute("lang")]
        public string Lang;

        [XmlAttribute("needsUpdating")]
        public string needsUpdating { get; set; }

        public string Text { get; set; }

        public string PreviousText { get; set; }

        #region Value_NonAttributesElements
        [XmlIgnore]
        public bool NeedsUpdating
        {
            get { return (needsUpdating == "true"); }
            set { needsUpdating = (value) ? "true" : null; }
        }
        #endregion Value_NonAttributesElements
    }

    public class Languages : List<Language>
    {
        public Language this[string strId]
        {
            get { return this.FirstOrDefault(lang => lang.Id == strId); }
        }
    }

    public class Language
    {
        [XmlAttribute("Id")] 
        public string Id;

        [XmlAttribute("name")]
        public string Name;

        [XmlElementAttribute(ElementName = "SendEmailOnUpdate")]
        public List<string> SendEmailOnUpdate { get; set; }
    }
}