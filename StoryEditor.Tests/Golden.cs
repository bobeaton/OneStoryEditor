using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Golden-file comparison for what the XElement loader builds from the characterization fixtures. The golden
    /// files (TestData/Golden/*.txt) hold what the old DataSet row constructors produced for the same fixtures (the
    /// oracle tests that compared the two passed before the row constructors were deleted); to regenerate after a
    /// deliberate change, run the tests with the environment variable OSE_UPDATE_GOLDEN=1 (each regenerating test then
    /// reports Inconclusive, so a leftover variable is noticed) and review the diff.
    ///
    /// Two kinds of timestamp text are normalized so a golden file depends neither on the machine's time zone nor on the clock:
    ///
    ///  1. A fixture date with an explicit UTC offset ("2026-10-03T12:34:57+02:00") is loaded as the machine's LOCAL
    ///     clock reading and the loader then treats that reading as UTC when it calls ToLocalTime, so what GetXml
    ///     writes is (UTC instant + the machine's offset), written as a plain "...Z" string: a different value in every
    ///     time zone, and in some zone equal to a Z or zone-less literal in a fixture. What is zone-independent is the
    ///     SECONDS: no zone offset has a seconds part, so they survive every conversion (including zones with :30 and
    ///     :45 minute offsets). The fixtures therefore give offset-form dates seconds that no Z or zone-less fixture
    ///     date uses (:57 and :58). Any written timestamp ending in :57Z / :58Z is normalized to {OFFSET <source text>}.
    ///  2. A timestamp that defaults to DateTime.Now when the fixture has none (only story stageDateTimeStamp and a
    ///     comment's timeStamp do; Dump writes those as timeStamp=) becomes {NOW} when it is within two minutes of now,
    ///     unless it is exactly a literal that appears in a fixture file (so a fixture literal can never become {NOW}).
    ///     Residual risk, accepted: an offset-derived value is also within two minutes of now only when the clock is
    ///     near the fixture's own 2026-10-0x time of day on those dates.
    /// </summary>
    internal static class Golden
    {
        // seconds of the offset-form fixture dates -> the fixture text they came from
        private static readonly Dictionary<string, string> OffsetSources = new Dictionary<string, string>
        {
            { "57", "2026-10-03T12:34:57+02:00" },
            { "58", "2026-10-03T13:00:58+02:00" },
        };

        // attributes (as Dump and GetXml write them) that default to DateTime.Now when the source has no value
        private static readonly HashSet<string> DefaultsToNow = new HashSet<string> { "timeStamp", "stageDateTimeStamp" };

        private static readonly Regex RegexTimestampAttribute =
            new Regex("(\\w+)=\"(\\d{4}-\\d\\d-\\d\\dT[^\"]*)\"", RegexOptions.Compiled);

        private static readonly Regex RegexOffsetDerived =
            new Regex("^\\d{4}-\\d\\d-\\d\\dT\\d\\d:\\d\\d:(57|58)Z$", RegexOptions.Compiled);

        private static readonly Regex RegexFixtureLiteral =
            new Regex("=\"(\\d{4}-\\d\\d-\\d\\dT\\d\\d:\\d\\d:\\d\\d(?:\\.\\d+)?Z?)\"", RegexOptions.Compiled);

        private static HashSet<string> _fixtureLiterals;

        // the clock readings of every Z or zone-less timestamp in the fixture files, as GetXml would write them
        internal static HashSet<string> FixtureLiterals()
        {
            if (_fixtureLiterals != null)
                return _fixtureLiterals;

            var set = new HashSet<string>();
            var strDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            if (Directory.Exists(strDir))
                foreach (var strFile in Directory.GetFiles(strDir, "*.onestory"))
                    foreach (Match m in RegexFixtureLiteral.Matches(File.ReadAllText(strFile)))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(m.Groups[1].Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
                            set.Add(WrittenForm(dt));
                    }
            return _fixtureLiterals = set;
        }

        // how GetXml writes a Z or zone-less clock reading
        private static string WrittenForm(DateTime dt)
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
        }

        internal static string Normalize(string str, ISet<string> fixtureLiterals = null)
        {
            str = str.Replace("\r\n", "\n");
            var literals = fixtureLiterals ?? FixtureLiterals();

            return RegexTimestampAttribute.Replace(str, m =>
            {
                var strAttribute = m.Groups[1].Value;
                var strValue = m.Groups[2].Value;

                DateTime dt;
                if (DefaultsToNow.Contains(strAttribute) && !literals.Contains(strValue) &&
                    DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt) &&
                    (Math.Abs((dt.ToUniversalTime() - DateTime.UtcNow).TotalMinutes) < 2))
                    return strAttribute + "=\"{NOW}\"";

                var mOffset = RegexOffsetDerived.Match(strValue);
                if (mOffset.Success)
                    return strAttribute + "=\"{OFFSET " + OffsetSources[mOffset.Groups[1].Value] + "}\"";

                return m.Value;
            });
        }

        public static void Check(string strName, string strActual, [CallerFilePath] string strCallerPath = "")
        {
            var strNormalized = Normalize(strActual);
            var strFileName = strName + ".txt";

            if (Environment.GetEnvironmentVariable("OSE_UPDATE_GOLDEN") == "1")
            {
                var strDir = Path.Combine(Path.GetDirectoryName(strCallerPath), "TestData", "Golden");
                Directory.CreateDirectory(strDir);
                File.WriteAllText(Path.Combine(strDir, strFileName), strNormalized, new UTF8Encoding(false));
                Assert.Inconclusive("golden regenerated: " + strFileName);
            }

            var strPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Golden", strFileName);
            Assert.That(File.Exists(strPath), Is.True, "missing golden file " + strFileName);
            var strExpected = File.ReadAllText(strPath, Encoding.UTF8).Replace("\r\n", "\n");
            Assert.That(strNormalized, Is.EqualTo(strExpected), "golden file " + strFileName);
        }
    }

    /// <summary>Builds the text that goes in a golden file: one "key: value" line per fact.</summary>
    internal class Dump
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public Dump Line(string strKey, object value)
        {
            _sb.Append(strKey).Append(": ").Append(Format(value)).Append('\n');
            return this;
        }

        // a timestamp that defaults to DateTime.Now when the source has none (written as timeStamp=, which Golden may turn into {NOW})
        public Dump DateMayDefaultToNow(string strKey, DateTime dt)
        {
            _sb.Append(strKey).Append(": timeStamp=\"").Append(StoryData.ToUniversalTime(dt)).Append("\" kind=").Append(dt.Kind).Append('\n');
            return this;
        }

        public Dump Raw(string str)
        {
            _sb.Append(str).Append('\n');
            return this;
        }

        public Dump Xml(string strKey, System.Xml.Linq.XElement elem)
        {
            _sb.Append(strKey).Append(":\n").Append(elem.ToString()).Append('\n');
            return this;
        }

        private static string Format(object value)
        {
            if (value == null)
                return "<null>";
            if (value is string)
                return "[" + ((string)value).Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") + "]";
            if (value is float)
                return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            if (value is DateTime)
            {
                var dt = (DateTime)value;
                return "stamp=\"" + StoryData.ToUniversalTime(dt) + "\" kind=" + dt.Kind;
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
