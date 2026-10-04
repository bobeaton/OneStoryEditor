using System;
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
    /// deliberate change, run the tests with the environment variable OSE_UPDATE_GOLDEN=1 and review the diff.
    ///
    /// Two kinds of text are normalized so a golden file doesn't depend on the machine or the clock:
    ///  - a timestamp written within a few minutes of now (the loader's "no timestamp means now") becomes {NOW};
    ///  - a timestamp that came from a date with an explicit UTC offset in a fixture is, by the loader's rule,
    ///    converted to the machine's local time, so it is written as {OFFSET input} (the value is computed here from the
    ///    same rule, independently of the loader, which is what makes the file machine-independent).
    /// </summary>
    internal static class Golden
    {
        // the dates with an explicit offset that the fixtures contain
        private static readonly string[] OffsetInputs = { "2026-10-03T12:34:56+02:00", "2026-10-03T13:00:00+02:00" };

        private static readonly Regex RegexTimeStamp =
            new Regex("(\\w*(?:imeStamp|DateTime))=\"([^\"]+)\"", RegexOptions.Compiled);

        internal static string Normalize(string str)
        {
            str = str.Replace("\r\n", "\n");

            foreach (var strInput in OffsetInputs)
            {
                // a date with an explicit offset: the local clock reading (Kind Unspecified), which the loader then
                //  treats as UTC when it calls ToLocalTime
                var dtLocal = DateTime.SpecifyKind(
                    DateTimeOffset.Parse(strInput, CultureInfo.InvariantCulture).LocalDateTime,
                    DateTimeKind.Unspecified).ToLocalTime();
                var strWritten = StoryData.ToUniversalTime(dtLocal);
                str = str.Replace("\"" + strWritten + "\"", "\"{OFFSET " + strInput + "}\"");
            }

            return RegexTimeStamp.Replace(str, m =>
            {
                DateTime dt;
                if (DateTime.TryParse(m.Groups[2].Value, CultureInfo.InvariantCulture,
                                      DateTimeStyles.RoundtripKind, out dt) &&
                    (Math.Abs((dt.ToUniversalTime() - DateTime.UtcNow).TotalMinutes) < 10))
                    return m.Groups[1].Value + "=\"{NOW}\"";
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
                return;
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
                return "timeStamp=\"" + StoryData.ToUniversalTime(dt) + "\" kind=" + dt.Kind;
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
