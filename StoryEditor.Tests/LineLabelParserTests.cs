using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class LineLabelParserTests
    {
        [Test]
        public void LineLabel_GivesLabelAndIndex()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "5", out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "5"));
            Assert.That(n, Is.EqualTo(5));
        }

        [Test]
        public void HiddenLineLabel_DropsTheHiddenSuffix()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "12" + VersesData.HiddenStringSpace, out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.LinePrefix + "12"));
            Assert.That(n, Is.EqualTo(12));
        }

        [Test]
        public void ConNoteZerothLine_GivesFirstVerseLabel()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.CstrZerothLineNameConNotes + " whatever", out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(StoryEditor.CstrFirstVerse));
            Assert.That(n, Is.EqualTo(0));
        }

        [Test]
        public void BtPaneZerothLine_GivesItsOwnName()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.CstrZerothLineNameBtPane, out var strText, out var n), Is.True);
            Assert.That(strText, Is.EqualTo(VersesData.CstrZerothLineNameBtPane));
            Assert.That(n, Is.EqualTo(0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("anchors")]
        public void Other_ReturnsFalse(string strLabel)
        {
            Assert.That(LineLabelParser.TryParse(strLabel, out _, out _), Is.False);
        }

        [Test]
        public void LineLabelWithoutNumber_ReturnsFalse_InsteadOfThrowing()
        {
            Assert.That(LineLabelParser.TryParse(VersesData.LinePrefix + "x", out _, out _), Is.False);
        }
    }
}
