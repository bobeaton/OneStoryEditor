using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Checks that a .onestory file satisfies StoryProject.xsd (embedded as a resource). Used at save time
    /// to make sure that what we just wrote is something the typed DataSet (the released exe, Chorus) can read.
    ///
    /// StoryProject.xsd is the schema the typed DataSet was generated from. It is stricter than the files we have
    /// always written (and that the DataSet reads happily): it fixes an element order that GetXml doesn't follow
    /// (e.g. BackTranslator before StoryPurpose), enumerations that the real values don't fit (memberType holds
    /// combined flags such as "Crafter, ProjectFacilitator"), IDs that repeat or dangle, and it doesn't know the
    /// attributes added since (HgRepoUrlHost, TextEncoding, ...). Enforcing it as written would refuse to save
    /// almost every real project, so the schema is relaxed on load (see Relax) to what is worth guarding: required
    /// attributes (e.g. story@guid), attribute types, and which elements may appear where.
    /// </summary>
    public static class ProjectFileValidator
    {
        public const string CstrSchemaResourceName = "OneStoryProjectEditor.StoryProject.xsd";

        private static readonly XNamespace Xs = XmlSchema.Namespace;

        private static XmlSchemaSet _schemas;

        private static XmlSchemaSet GetSchemas()
        {
            if (_schemas != null)
                return _schemas;

            XDocument xsd;
            using (var stream = typeof(ProjectFileValidator).Assembly.GetManifestResourceStream(CstrSchemaResourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("The embedded resource " + CstrSchemaResourceName + " is missing");
                xsd = XDocument.Load(stream);
            }
            Relax(xsd);

            var schemas = new XmlSchemaSet();
            using (var reader = xsd.CreateReader())
                schemas.Add(null, reader);
            schemas.Compile();
            _schemas = schemas;
            return schemas;
        }

        // internal so a test can look at what is (and isn't) enforced
        internal static void Relax(XDocument xsd)
        {
            // identity constraints (keys on guids and names) and enumerations
            xsd.Descendants().Where(e => e.Name == Xs + "key" || e.Name == Xs + "keyref" || e.Name == Xs + "unique")
               .ToList().ForEach(e => e.Remove());
            xsd.Descendants(Xs + "enumeration").ToList().ForEach(e => e.Remove());

            // ID/IDREF values repeat or dangle in real files
            foreach (var attr in xsd.Descendants(Xs + "attribute").Select(a => a.Attribute("type")).Where(t => t != null).ToList())
            {
                var strType = attr.Value;
                if (strType == "xs:ID" || strType == "xs:IDREF")
                    attr.Value = "xs:string";
            }

            // element order and counts: "any of these, any number of times, in any order"
            foreach (var sequence in xsd.Descendants(Xs + "sequence").ToList())
            {
                sequence.Name = Xs + "choice";
                sequence.SetAttributeValue("minOccurs", "0");
                sequence.SetAttributeValue("maxOccurs", "unbounded");
            }

            // attributes the schema doesn't know (written by newer versions)
            foreach (var complexType in xsd.Descendants(Xs + "complexType").ToList())
            {
                var extension = complexType.Element(Xs + "simpleContent")?.Element(Xs + "extension");
                var target = extension ?? complexType;
                if (target.Element(Xs + "anyAttribute") == null)
                    target.Add(new XElement(Xs + "anyAttribute", new XAttribute("processContents", "skip")));
            }
        }

        /// <summary>
        /// Throws XmlSchemaValidationException (or XmlException if the file isn't well-formed XML)
        /// if the file doesn't satisfy the (relaxed) StoryProject.xsd.
        /// </summary>
        public static void Validate(string strPath)
        {
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = GetSchemas(),
                DtdProcessing = DtdProcessing.Ignore
            };

            // with no event handler, validation errors are thrown as XmlSchemaValidationException
            using (var reader = XmlReader.Create(strPath, settings))
            {
                while (reader.Read())
                {
                }
            }
        }
    }
}
