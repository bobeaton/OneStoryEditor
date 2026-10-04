using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class LegacyTextRepairTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minimal-1.8.onestory");

        [TestCase("[B&amp;B]", "[B&B]")]
        [TestCase("snake &amp; lady", "snake & lady")]
        [TestCase("kata&nbsp;Tuhan", "kata\u00A0Tuhan")]
        [TestCase("&lt;OseStoryToCopy&gt;", "<OseStoryToCopy>")]
        [TestCase("&quot;q&quot; &#39;s&#39; &#x41;", "\"q\" 's' A")]
        [TestCase("<donkey bray>", "<donkey bray>")]
        [TestCase("B&B;", "B&B;")]                 // not an entity IE produces: leave it
        [TestCase("a & b", "a & b")]
        public void DecodeIeEntities(string input, string expected)
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities(input), Is.EqualTo(expected));
        }

        [Test]
        public void ClearLanguageNamePlaceholders_ClearsValuesThatAreJustTheColumnsLanguageName()
        {
            // the fixture's Vernacular language is called "Testish"
            var doc = XDocument.Load(FixturePath);
            doc.Descendants("StoryLine").Single().Value = "Testish";
            doc.Descendants("TestQuestionLine").Single().Value = " Testish ";
            doc.Descendants("ConsultantNote").Single().Value = "Testish";
            var strTemp = Path.Combine(Path.GetTempPath(), "ose-placeholder-" + Guid.NewGuid() + ".onestory");
            try
            {
                doc.Save(strTemp);
                ProjectReader projFile;
                ProjectReader.ReadProjectFile(strTemp, out projFile);

                var nCleared = LegacyTextRepair.ClearLanguageNamePlaceholders(projFile);

                Assert.That(nCleared, Is.EqualTo(2));
                Assert.That(projFile.Tables["StoryLine"].Rows[0]["StoryLine_text"], Is.EqualTo(String.Empty));
                Assert.That(projFile.Tables["TestQuestionLine"].Rows[0]["TestQuestionLine_text"], Is.EqualTo(String.Empty));
                Assert.That(projFile.Tables["ConsultantNote"].Rows[0]["ConsultantNote_text"], Is.EqualTo("Testish"));   // notes aren't textareas with placeholders
            }
            finally
            {
                File.Delete(strTemp);
            }
        }

        [Test]
        public void ClearLanguageNamePlaceholders_LeavesOtherTextAlone()
        {
            ProjectReader projFile;
            ProjectReader.ReadProjectFile(FixturePath, out projFile);

            Assert.That(LegacyTextRepair.ClearLanguageNamePlaceholders(projFile), Is.EqualTo(0));
            Assert.That((string)projFile.Tables["StoryLine"].Rows[0]["StoryLine_text"], Does.StartWith("dengan"));
        }

        [Test]
        public void DecodeIeEntities_OneLevelOnly()
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities("&amp;amp;"), Is.EqualTo("&amp;"));
        }

        [Test]
        public void DecodePlainTextFields_DecodesStoryFieldsButNotNotes()
        {
            ProjectReader projFile;
            ProjectReader.ReadProjectFile(FixturePath, out projFile);

            var nChanged = LegacyTextRepair.DecodePlainTextFields(projFile);

            Assert.That(nChanged, Is.EqualTo(2));
            Assert.That(projFile.Tables["StoryLine"].Rows[0]["StoryLine_text"],
                        Is.EqualTo("dengan [B&B] kata\u00A0Tuhan <donkey bray> B&B;"));
            Assert.That(projFile.Tables["TestQuestionLine"].Rows[0]["TestQuestionLine_text"],
                        Is.EqualTo("snake & lady?"));
            Assert.That(projFile.Tables["ConsultantNote"].Rows[0]["ConsultantNote_text"],
                        Is.EqualTo("ok <B>only</B> [B&amp;B]"));   // notes are HTML: untouched
        }

        [Test]
        public void DecodePlainTextElements_DecodesClipboardXml()
        {
            var root = XElement.Parse(
                "<OseStoryToCopy><story><Verses><Verse>" +
                "<StoryLine lang=\"Vernacular\">[B&amp;amp;B]</StoryLine>" +
                "<ConsultantNotes><ConsultantConversation><ConsultantNote>[B&amp;amp;B]</ConsultantNote></ConsultantConversation></ConsultantNotes>" +
                "</Verse></Verses></story></OseStoryToCopy>");

            var nChanged = LegacyTextRepair.DecodePlainTextElements(root);

            Assert.That(nChanged, Is.EqualTo(1));
            Assert.That(root.Descendants("StoryLine").Single().Value, Is.EqualTo("[B&B]"));
            Assert.That(root.Descendants("StoryLine").Single().Attribute("lang").Value, Is.EqualTo("Vernacular"));
            Assert.That(root.Descendants("ConsultantNote").Single().Value, Is.EqualTo("[B&amp;B]"));
        }

        [Test]
        public void DecodeUnlessMarked_UnmarkedIsDecoded()
        {
            var root = XElement.Parse("<OseColumnToCopy><StoryLine lang=\"Vernacular\">a &amp;amp; b</StoryLine></OseColumnToCopy>");
            Assert.That(LegacyTextRepair.DecodeUnlessMarked(root), Is.True);
            Assert.That(root.Element("StoryLine").Value, Is.EqualTo("a & b"));
        }

        [Test]
        public void DecodeUnlessMarked_MarkedIsLeftAlone()
        {
            var root = XElement.Parse("<OseColumnToCopy TextEncoding=\"plain\"><StoryLine lang=\"Vernacular\">a &amp;amp; b</StoryLine></OseColumnToCopy>");
            Assert.That(LegacyTextRepair.DecodeUnlessMarked(root), Is.False);
            Assert.That(root.Element("StoryLine").Value, Is.EqualTo("a &amp; b"));
        }

        [Test]
        public void MarkAsPlain_SetsAttribute()
        {
            var root = new XElement("StoryProject", new XAttribute("version", "1.8"));
            Assert.That(LegacyTextRepair.IsMarkedPlain(root), Is.False);
            LegacyTextRepair.MarkAsPlain(root);
            Assert.That(LegacyTextRepair.IsMarkedPlain(root), Is.True);
            Assert.That(root.Attribute("version").Value, Is.EqualTo("1.8"));
        }

        [Test]
        public void IsFileMarkedPlain()
        {
            var strMarked = Path.Combine(Path.GetTempPath(), "ose-marked-" + Guid.NewGuid() + ".onestory");
            try
            {
                var doc = XDocument.Load(FixturePath);
                LegacyTextRepair.MarkAsPlain(doc.Root);
                doc.Save(strMarked);

                Assert.That(LegacyTextRepair.IsFileMarkedPlain(FixturePath), Is.False);
                Assert.That(LegacyTextRepair.IsFileMarkedPlain(strMarked), Is.True);
            }
            finally
            {
                File.Delete(strMarked);
            }
        }

        [Test]
        public void IsFileMarkedPlain_NeverThrows()
        {
            var strDoctype = Path.Combine(Path.GetTempPath(), "ose-doctype-" + Guid.NewGuid() + ".onestory");
            try
            {
                File.WriteAllText(strDoctype,
                    "<?xml version=\"1.0\"?><!DOCTYPE StoryProject [<!ENTITY x \"y\">]><StoryProject TextEncoding=\"plain\" />");
                Assert.That(LegacyTextRepair.IsFileMarkedPlain(strDoctype), Is.False);
            }
            finally
            {
                File.Delete(strDoctype);
            }

            Assert.That(LegacyTextRepair.IsFileMarkedPlain(Path.Combine(Path.GetTempPath(), "no-such-" + Guid.NewGuid() + ".onestory")),
                        Is.False);
        }

        [TestCase("&#65;", "A")]
        [TestCase("&#x41;", "A")]
        [TestCase("&#0;", "&#0;")]
        [TestCase("&#8;", "&#8;")]
        [TestCase("&#xD800;", "&#xD800;")]
        [TestCase("&#xFFFE;", "&#xFFFE;")]
        [TestCase("&#9;", "\t")]
        [TestCase("&#1114112;", "&#1114112;")]
        public void DecodeIeEntities_NumericEntitiesMustBeValidXmlChars(string input, string expected)
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities(input), Is.EqualTo(expected));
        }

        [Test]
        public void DecodePlainTextElements_XmlNode()
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml("<story><Verses><Verse><StoryLine lang=\"V\">[B&amp;amp;B]</StoryLine>" +
                        "<ConsultantNote>[B&amp;amp;B]</ConsultantNote>" +
                        "<LnCNote VernacularRendering=\"a &amp;amp; b\">x &amp;amp; y</LnCNote></Verse></Verses></story>");

            var node = LegacyTextRepair.DecodePlainTextElements(doc.DocumentElement);

            Assert.That(node, Is.SameAs(doc.DocumentElement));
            Assert.That(doc.SelectSingleNode("//StoryLine").InnerText, Is.EqualTo("[B&B]"));
            Assert.That(doc.SelectSingleNode("//ConsultantNote").InnerText, Is.EqualTo("[B&amp;B]"));
            Assert.That(doc.SelectSingleNode("//LnCNote").InnerText, Is.EqualTo("x & y"));
            Assert.That(doc.SelectSingleNode("//LnCNote").Attributes["VernacularRendering"].Value, Is.EqualTo("a & b"));
            Assert.That(LegacyTextRepair.DecodePlainTextElements((System.Xml.XmlNode)null), Is.Null);
        }

        [Test]
        public void ContainsIeEntity()
        {
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&amp;B]"), Is.True);
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&B] B&B;"), Is.False);
        }
    }
}
