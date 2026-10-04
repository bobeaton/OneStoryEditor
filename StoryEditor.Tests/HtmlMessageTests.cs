using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlMessageTests
    {
        // u-umlaut plus a Devanagari consonant and vowel sign; built from code points so no source-file escape or encoding can mangle it
        internal static readonly string AwkwardNonAscii = new string(new[] { (char)0x00FC, (char)0x0915, (char)0x093F });

        [Test]
        public void Create_PutsTypeAndPayloadInBody()
        {
            var msg = HtmlMessage.Create("scrollTo", new { id = "ln_3", alignTop = true });
            Assert.That(msg.Type, Is.EqualTo("scrollTo"));
            Assert.That(msg.GetString("id"), Is.EqualTo("ln_3"));
            Assert.That(msg.GetBool("alignTop"), Is.True);
        }

        [Test]
        public void ToJson_ThenTryParse_RoundTripsAwkwardText()
        {
            var strText = "a \"quote\" <b>&amp;</b>\r\nline2 " + AwkwardNonAscii;
            var msg = HtmlMessage.TryParse(HtmlMessage.Create("x", new { text = strText }).ToJson());
            Assert.That(msg.GetString("text"), Is.EqualTo(strText));
        }

        [TestCase("not json")]
        [TestCase("[1,2]")]
        [TestCase("{\"notype\":1}")]
        [TestCase("{\"type\":5}")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("{\"type\":\"t\"} {\"type\":\"u\"}")]     // trailing content (JObject.Parse rejected it too)
        public void TryParse_Garbage_ReturnsNull(string json)
        {
            Assert.That(HtmlMessage.TryParse(json), Is.Null);
        }

        // Json.NET would otherwise turn date-like strings into DateTime (and GetString would then give null)
        [TestCase("2021-03-04T12:00")]
        [TestCase("2021-03-04T12:00:00Z")]
        [TestCase("2021-03-04T12:00:00.123+02:00")]
        public void TryParse_DateLikeText_StaysAString(string strText)
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"text\":\"" + strText + "\"}");
            Assert.That(msg.GetString("text"), Is.EqualTo(strText));
        }

        [Test]
        public void TryGetInt_AcceptsNumbersAndNumericStrings_RejectsOthers()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":3,\"b\":\"12\",\"c\":\"x\",\"d\":true}");
            Assert.That(msg.TryGetInt("a", out var a) && a == 3, Is.True);
            Assert.That(msg.TryGetInt("b", out var b) && b == 12, Is.True);
            Assert.That(msg.TryGetInt("c", out _), Is.False);
            Assert.That(msg.TryGetInt("d", out _), Is.False);
            Assert.That(msg.TryGetInt("missing", out _), Is.False);
        }

        [Test]
        public void GetBool_AcceptsBooleansAndTrueFalseStrings()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":true,\"b\":\"true\",\"c\":\"False\",\"d\":1}");
            Assert.That(msg.GetBool("a"), Is.True);
            Assert.That(msg.GetBool("b"), Is.True);
            Assert.That(msg.GetBool("c", true), Is.False);
            Assert.That(msg.GetBool("d"), Is.False, "numbers aren't booleans");
            Assert.That(msg.GetBool("missing", true), Is.True);
        }

        [Test]
        public void GetString_NonString_ReturnsNull()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"t\",\"a\":3}");
            Assert.That(msg.GetString("a"), Is.Null);
        }

        [Test]
        public void RidReplyToAndDocId_AreRead()
        {
            var msg = HtmlMessage.TryParse("{\"type\":\"reply\",\"re\":7,\"doc\":\"3\"}");
            Assert.That(msg.ReplyTo, Is.EqualTo(7));
            Assert.That(msg.DocId, Is.EqualTo("3"));
            Assert.That(HtmlMessage.Create("x").WithRid(9).Rid, Is.EqualTo(9));
        }
    }
}
