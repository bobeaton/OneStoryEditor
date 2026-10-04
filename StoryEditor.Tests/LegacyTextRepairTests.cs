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

        // (the no-break space is written as an escape so it can't be mistaken for an ordinary space)
        private const string Nbsp = "\x00A0";

        [TestCase("[B&amp;B]", "[B&B]")]
        [TestCase("snake &amp; lady", "snake & lady")]
        [TestCase("kata&nbsp;Tuhan", "kata" + Nbsp + "Tuhan")]
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
            var root = XDocument.Load(FixturePath).Root;
            root.Descendants("StoryLine").Single().Value = "Testish";
            root.Descendants("TestQuestionLine").Single().Value = " Testish ";
            root.Descendants("ConsultantNote").Single().Value = "Testish";

            var nCleared = LegacyTextRepair.ClearLanguageNamePlaceholders(root);

            Assert.That(nCleared, Is.EqualTo(2));
            Assert.That(XmlRead.Text(root.Descendants("StoryLine").Single()), Is.EqualTo(String.Empty));
            Assert.That(XmlRead.Text(root.Descendants("TestQuestionLine").Single()), Is.EqualTo(String.Empty));
            Assert.That(XmlRead.Text(root.Descendants("ConsultantNote").Single()), Is.EqualTo("Testish"));   // notes aren't textareas with placeholders
        }

        [Test]
        public void ClearLanguageNamePlaceholders_Element_ReadsAsEmptyText_AndWritesAnEmptyElementPair()
        {
            var root = XElement.Parse(
                "<StoryProject><Languages><LanguageInfo lang=\"Vernacular\" name=\"Testish\" /></Languages>" +
                "<Retelling lang=\"Vernacular\" memberID=\"m1\">Testish</Retelling></StoryProject>");
            var elemRetelling = root.Element("Retelling");

            Assert.That(LegacyTextRepair.ClearLanguageNamePlaceholders(root), Is.EqualTo(1));
            Assert.That(XmlRead.Text(elemRetelling), Is.EqualTo(String.Empty));   // the annotation: "" and not null

            // the object rebuilt from it writes <Retelling ...></Retelling>, not <Retelling ... />
            var line = new LineMemberData("m1", StoryEditor.TextFields.Retelling);
            line.SetValue("Vernacular", XmlRead.Text(elemRetelling));
            var elemOut = new XElement("Retellings");
            line.AddXml(elemOut, "Retelling");
            var strVernacular = elemOut.Elements("Retelling").First().ToString(SaveOptions.DisableFormatting);
            Assert.That(strVernacular, Does.EndWith("></Retelling>"));
            Assert.That(strVernacular, Does.Contain("lang=\"Vernacular\""));
        }

        [Test]
        public void ClearLanguageNamePlaceholders_LeavesOtherTextAlone()
        {
            var root = ProjectFile.Load(FixturePath).Root;

            Assert.That(LegacyTextRepair.ClearLanguageNamePlaceholders(root), Is.EqualTo(0));
            Assert.That(XmlRead.Text(root.Descendants("StoryLine").Single()), Does.StartWith("dengan"));
        }

        [Test]
        public void DecodePlainTextElements_DecodesStoryFieldsButNotNotes()
        {
            var root = ProjectFile.Load(FixturePath).Root;

            var nChanged = LegacyTextRepair.DecodePlainTextElements(root);

            Assert.That(nChanged, Is.EqualTo(2));
            Assert.That(XmlRead.Text(root.Descendants("StoryLine").Single()),
                        Is.EqualTo("dengan [B&B] kata" + Nbsp + "Tuhan <donkey bray> B&B;"));
            Assert.That(XmlRead.Text(root.Descendants("TestQuestionLine").Single()),
                        Is.EqualTo("snake & lady?"));
            Assert.That(XmlRead.Text(root.Descendants("ConsultantNote").Single()),
                        Is.EqualTo("ok <B>only</B> [B&amp;B]"));   // notes are HTML: untouched
        }

        [Test]
        public void DecodePlainTextElements_DecodesLnCNoteTextAndRenderingAttributes()
        {
            var root = XElement.Parse(
                "<story><Verses><Verse><StoryLine lang=\"V\">[B&amp;amp;B]</StoryLine>" +
                "<ConsultantNote>[B&amp;amp;B]</ConsultantNote>" +
                "<LnCNote VernacularRendering=\"a &amp;amp; b\">x &amp;amp; y</LnCNote></Verse></Verses></story>");

            Assert.That(LegacyTextRepair.DecodePlainTextElements(root), Is.EqualTo(3));

            Assert.That(root.Descendants("StoryLine").Single().Value, Is.EqualTo("[B&B]"));
            Assert.That(root.Descendants("ConsultantNote").Single().Value, Is.EqualTo("[B&amp;B]"));
            Assert.That(root.Descendants("LnCNote").Single().Value, Is.EqualTo("x & y"));
            Assert.That((string)root.Descendants("LnCNote").Single().Attribute("VernacularRendering"), Is.EqualTo("a & b"));
        }

        [Test]
        public void DecodeIeEntities_OneLevelOnly()
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities("&amp;amp;"), Is.EqualTo("&amp;"));
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
        public void ContainsIeEntity()
        {
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&amp;B]"), Is.True);
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&B] B&B;"), Is.False);
        }
    }
}
