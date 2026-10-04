using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Golden.Normalize must give the same text whatever the machine's time zone is. The zone can't be changed here, so
    /// these tests emulate the loader for several zones (TimeZoneInfo) and check what Normalize makes of the result.
    /// </summary>
    [TestFixture]
    public class GoldenNormalizeTests
    {
        // what GetXml writes for a fixture date with an explicit offset on a machine in zone tz: the loader reads the
        //  local clock reading, treats it as UTC when it calls ToLocalTime, and GetXml writes ToUniversalTime of that
        private static string WrittenInZone(string strWithOffset, TimeZoneInfo tz)
        {
            var utc = DateTimeOffset.Parse(strWithOffset, System.Globalization.CultureInfo.InvariantCulture).UtcDateTime;
            var localClockReading = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);                       // Kind Unspecified
            var asIfUtc = DateTime.SpecifyKind(localClockReading, DateTimeKind.Utc);
            var local = TimeZoneInfo.ConvertTimeFromUtc(asIfUtc, tz);                                // ToLocalTime in zone tz
            var written = TimeZoneInfo.ConvertTimeToUtc(local, tz);                                  // ToUniversalTime in zone tz
            return written.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static readonly HashSet<string> Literals = new HashSet<string>
        {
            "2026-10-03T12:34:56Z", "2026-10-03T14:00:00Z", "2026-10-03T00:00:00Z"
        };

        [TestCase("UTC+2", 2, 0)]
        [TestCase("UTC+3", 3, 0)]
        [TestCase("UTC+5:45", 5, 45)]
        [TestCase("UTC+5:30", 5, 30)]
        [TestCase("UTC-5", -5, 0)]
        [TestCase("UTC-9:30", -9, -30)]
        [TestCase("UTC", 0, 0)]
        public void OffsetDerivedValues_BecomeTheSameTokenInEveryZone(string strName, int nHours, int nMinutes)
        {
            var tz = TimeZoneInfo.CreateCustomTimeZone(strName, new TimeSpan(nHours, nMinutes, 0), strName, strName);
            var strA = WrittenInZone("2026-10-03T12:34:57+02:00", tz);
            var strB = WrittenInZone("2026-10-03T13:00:58+02:00", tz);

            var strText = "<a timeStamp=\"" + strA + "\" /><b TransitionDateTime=\"" + strB + "\" />" +
                          "<c timeStamp=\"2026-10-03T12:34:56Z\" /><d timeStamp=\"2026-10-03T14:00:00Z\" />";

            Assert.That(Golden.Normalize(strText, Literals),
                        Is.EqualTo("<a timeStamp=\"{OFFSET 2026-10-03T12:34:57+02:00}\" />" +
                                   "<b TransitionDateTime=\"{OFFSET 2026-10-03T13:00:58+02:00}\" />" +
                                   "<c timeStamp=\"2026-10-03T12:34:56Z\" /><d timeStamp=\"2026-10-03T14:00:00Z\" />"),
                        "in " + strName + " the derived values were " + strA + " and " + strB);
        }

        [Test]
        public void NowIsOnlyUsedForAttributesThatDefaultToNow_AndNeverForAFixtureLiteral()
        {
            var strNow = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture);

            Assert.That(Golden.Normalize("<a timeStamp=\"" + strNow + "\" />", Literals),
                        Is.EqualTo("<a timeStamp=\"{NOW}\" />"));
            Assert.That(Golden.Normalize("<a stageDateTimeStamp=\"" + strNow + "\" />", Literals),
                        Is.EqualTo("<a stageDateTimeStamp=\"{NOW}\" />"));

            // an attribute that has no default-to-now (a transition's date is required) is left alone
            var strTransition = "<a TransitionDateTime=\"" + strNow + "\" />";
            Assert.That(Golden.Normalize(strTransition, Literals), Is.EqualTo(strTransition));

            // a fixture literal that happens to be the current time is left alone
            var strLiteral = "<a timeStamp=\"" + strNow + "\" />";
            Assert.That(Golden.Normalize(strLiteral, new HashSet<string> { strNow }), Is.EqualTo(strLiteral));

            // an old value is left alone
            Assert.That(Golden.Normalize("<a timeStamp=\"2020-01-01T00:00:00Z\" />", new HashSet<string>()),
                        Is.EqualTo("<a timeStamp=\"2020-01-01T00:00:00Z\" />"));
        }
    }
}
