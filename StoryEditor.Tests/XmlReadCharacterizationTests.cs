using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using OneStoryProjectEditor.Tests.ReleasedExeDataSet;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// The typed DataSet (the test-only copy of the released exe's, ReleasedExeDataSet) is the oracle: every test loads characterization.onestory both
    /// ways and requires the XmlRead helper to give what the DataSet column gives.
    /// </summary>
    [TestFixture]
    public class XmlReadCharacterizationTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization.onestory");

        private NewDataSet _ds;
        private XDocument _doc;

        [OneTimeSetUp]
        public void Load()
        {
            _ds = new NewDataSet();
            _ds.ReadXml(FixturePath);
            _doc = XDocument.Load(FixturePath, LoadOptions.None);
        }

        // null when the DataSet column is DBNull (the DataSet does NOT apply XSD defaults on read)
        private static T? Raw<T>(DataRow row, string column) where T : struct
        {
            return (row[column] == DBNull.Value) ? (T?)null : (T)row[column];
        }

        private static string RawString(DataRow row, string column)
        {
            var o = row[column];
            return (o == DBNull.Value) ? null : (string)o;
        }

        private static void AssertDate(DataRow row, string column, XElement elem, string attr, string what)
        {
            var actual = XmlRead.Date(elem, attr);
            if (row[column] == DBNull.Value)
            {
                Assert.That(actual, Is.Null, what);
                return;
            }
            var expected = (DateTime)row[column];
            Assert.That(actual.HasValue, Is.True, what);
            Assert.That(actual.Value, Is.EqualTo(expected), what);
            Assert.That(actual.Value.Kind, Is.EqualTo(expected.Kind), what + " Kind");
            Assert.That(actual.Value.ToUniversalTime(), Is.EqualTo(expected.ToUniversalTime()), what + " utc");
            Assert.That(actual.Value.ToString("o"), Is.EqualTo(expected.ToString("o")), what + " round-trip");
        }

        [Test]
        public void FixtureLoadsInDataSet()
        {
            Assert.That(_ds, Is.Not.Null);
            Assert.That(_ds.story.Count, Is.EqualTo(5));
        }

        [Test]
        public void StoryTimeStamps_MatchDataSet_ValueAndKind()
        {
            var rows = _ds.story.ToList();
            var elems = _doc.Descendants("story").ToList();
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
                AssertDate(rows[i], "stageDateTimeStamp", elems[i], "stageDateTimeStamp", $"story {i}");
        }

        [Test]
        public void StateTransitionDates_MatchDataSet_ValueAndKind()
        {
            var rows = _ds.StateTransition.ToList();
            var elems = _doc.Descendants("StateTransition").ToList();
            Assert.That(rows.Count, Is.EqualTo(3));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
                AssertDate(rows[i], "TransitionDateTime", elems[i], "TransitionDateTime", $"transition {i}");
        }

        [Test]
        public void ConsultantNoteTimeStamps_MatchDataSet_ValueAndKind()
        {
            var rows = _ds.ConsultantNote.ToList();
            var elems = _doc.Descendants("ConsultantNote").ToList();
            Assert.That(rows.Count, Is.EqualTo(11));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
                AssertDate(rows[i], "timeStamp", elems[i], "timeStamp", $"note {i}");
        }

        [Test]
        public void ConsultantNoteText_MatchesDataSet()
        {
            var rows = _ds.ConsultantNote.ToList();
            var elems = _doc.Descendants("ConsultantNote").ToList();
            for (int i = 0; i < rows.Count; i++)
                Assert.That(XmlRead.Text(elems[i]), Is.EqualTo(RawString(rows[i], "ConsultantNote_text")), $"note {i}");
        }

        [Test]
        public void VerseVisible_MatchesDataSet_IncludingDefault()
        {
            var rows = _ds.Verse.ToList();
            var elems = _doc.Descendants("Verse").ToList();
            Assert.That(rows.Count, Is.EqualTo(8));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
            {
                Assert.That(XmlRead.Bool(elems[i], "visible"), Is.EqualTo(Raw<bool>(rows[i], "visible")), $"verse {i}");
                var raw = XmlRead.Bool(elems[i], "visible");
                Assert.That(XmlRead.Bool(elems[i], "visible", true), Is.EqualTo(Raw<bool>(rows[i], "visible") ?? true), $"verse {i} default");
            }
        }

        [Test]
        public void VerseFirst_MatchesDataSet_IncludingDefault()
        {
            var rows = _ds.Verse.ToList();
            var elems = _doc.Descendants("Verse").ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                Assert.That(XmlRead.Bool(elems[i], "first"), Is.EqualTo(Raw<bool>(rows[i], "first")), $"verse {i}");
                Assert.That(XmlRead.Bool(elems[i], "first", false), Is.EqualTo(Raw<bool>(rows[i], "first") ?? false), $"verse {i} default");
            }
        }

        [Test]
        public void ConversationFinished_MatchesDataSet_IncludingDefault()
        {
            var rows = _ds.ConsultantConversation.ToList();
            var elems = _doc.Descendants("ConsultantConversation").ToList();
            Assert.That(rows.Count, Is.EqualTo(5));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
            {
                Assert.That(XmlRead.Bool(elems[i], "finished"), Is.EqualTo(Raw<bool>(rows[i], "finished")), $"conv {i}");
                Assert.That(XmlRead.Bool(elems[i], "finished", false), Is.EqualTo(Raw<bool>(rows[i], "finished") ?? false), $"conv {i} default");
            }
        }

        [Test]
        public void StoryCounts_MatchDataSet_IncludingDefault()
        {
            var rows = _ds.story.ToList();
            var elems = _doc.Descendants("story").ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                Assert.That(XmlRead.Int(elems[i], "CountRetellingsTests", 77), Is.EqualTo(Raw<int>(rows[i], "CountRetellingsTests") ?? 77), $"story {i}");
                Assert.That(XmlRead.Int(elems[i], "CountTestingQuestionTests", 0), Is.EqualTo(Raw<int>(rows[i], "CountTestingQuestionTests") ?? 0), $"story {i} tq");
            }
        }

        [Test]
        public void StoryLineText_MatchesDataSet_NullEmptyWhitespacePadded()
        {
            var rows = _ds.StoryLine.ToList();
            var elems = _doc.Descendants("StoryLine").ToList();
            Assert.That(rows.Count, Is.EqualTo(8));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
                Assert.That(XmlRead.Text(elems[i]), Is.EqualTo(RawString(rows[i], "StoryLine_text")), $"storyline {i}");
        }

        [Test]
        public void AnchorText_MatchesDataSet_NullEmptyWhitespace()
        {
            var rows = _ds.Anchor.ToList();
            var elems = _doc.Descendants("Anchor").ToList();
            Assert.That(rows.Count, Is.EqualTo(6));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
                Assert.That(XmlRead.Text(elems[i]), Is.EqualTo(RawString(rows[i], "Anchor_text")), $"anchor {i}");
        }

        [Test]
        public void LanguageInfoFontSize_MatchesDataSet()
        {
            var row = _ds.LanguageInfo.Single();
            var elem = _doc.Descendants("LanguageInfo").Single();
            Assert.That(XmlRead.Float(elem, "FontSize"), Is.EqualTo(Raw<float>(row, "FontSize")));
            Assert.That(XmlRead.Float(elem, "FontSize"), Is.EqualTo(12.5f));
            Assert.That(XmlRead.Float(elem, "Missing"), Is.Null);
        }

        [Test]
        public void MembersBooleans_MatchDataSet()
        {
            var row = _ds.Members.Single();
            var elem = _doc.Descendants("Members").Single();
            Assert.That(XmlRead.Bool(elem, "HasOutsideEnglishBTer"), Is.EqualTo(Raw<bool>(row, "HasOutsideEnglishBTer")));
            Assert.That(XmlRead.Bool(elem, "HasOutsideEnglishBTer"), Is.True);
            Assert.That(XmlRead.Bool(elem, "HasFirstPassMentor"), Is.EqualTo(Raw<bool>(row, "HasFirstPassMentor")));
            Assert.That(XmlRead.Bool(elem, "HasIndependentConsultant"), Is.EqualTo(Raw<bool>(row, "HasIndependentConsultant")));
        }

        [Test]
        public void VerseVisibleForms_AreAllParsedLikeTheDataSet()
        {
            // forms present in the fixture: absent, "false", "true", "1"
            var forms = _doc.Descendants("Verse").Select(v => (string)v.Attribute("visible")).ToList();
            Assert.That(forms, Is.EquivalentTo(new string[] { null, "false", "true", "1", null, null, null, null }));
        }

        [Test]
        public void RequiredAttr_MissingThrowsApplicationExceptionWithMessage()
        {
            var elem = XElement.Parse("<Verse />");
            var ex = Assert.Throws<ApplicationException>(() => XmlRead.RequiredAttr(elem, "guid"));
            Assert.That(ex.Message, Is.EqualTo("The project file is damaged: <Verse> is missing the required attribute 'guid'."));
            Assert.That(XmlRead.RequiredAttr(XElement.Parse("<Verse guid=\"g\" />"), "guid"), Is.EqualTo("g"));
        }

        [Test]
        public void FirstAndChildren_ReturnDocumentOrder()
        {
            var parent = XElement.Parse("<P><A n=\"1\"/><B/><A n=\"2\"/></P>");
            Assert.That(XmlRead.First(parent, "A").Attribute("n").Value, Is.EqualTo("1"));
            Assert.That(XmlRead.First(parent, "Z"), Is.Null);
            Assert.That(XmlRead.Children(parent, "A").Select(a => a.Attribute("n").Value), Is.EqualTo(new[] { "1", "2" }));
        }

        // ---- attribute-form variants: load a tiny file with one attribute value set to each form ----

        private const string VariantTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
            "<StoryProject version=\"1.8\" ProjectName=\"v\">" +
            "<Members><Member name=\"M\" memberType=\"Crafter\" memberKey=\"mem-1\" /></Members>" +
            "<Languages><LanguageInfo lang=\"Vernacular\" name=\"T\" code=\"t\" FontName=\"Arial\" FontSize=\"12\" FontColor=\"Maroon\" SentenceFinalPunct=\".\" /></Languages>" +
            "<stories SetName=\"Stories\"><story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\" @STORY@>" +
            "<CraftingInfo><StoryCrafter memberID=\"mem-1\" /></CraftingInfo>" +
            "<Verses><Verse guid=\"v1\" @VERSE@><StoryLine lang=\"Vernacular\">@TEXT@</StoryLine></Verse></Verses>" +
            "</story></stories></StoryProject>";

        private static string Esc(string s)
        {
            return s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;");
        }

        private static NewDataSet LoadVariant(string storyAttrs, string verseAttrs, string text, out XElement elemStory, out XElement elemVerse, out XElement elemLine)
        {
            var xml = VariantTemplate.Replace("@STORY@", storyAttrs).Replace("@VERSE@", verseAttrs).Replace("@TEXT@", text);
            var path = Path.Combine(Path.GetTempPath(), "ose-variant-" + Guid.NewGuid() + ".onestory");
            File.WriteAllText(path, xml, new System.Text.UTF8Encoding(false));
            try
            {
                var ds = new NewDataSet();
                ds.ReadXml(path);
                var doc = XDocument.Load(path, LoadOptions.None);
                elemStory = doc.Descendants("story").Single();
                elemVerse = doc.Descendants("Verse").Single();
                elemLine = doc.Descendants("StoryLine").Single();
                return ds;
            }
            finally
            {
                File.Delete(path);
            }
        }

        // compares "the DataSet's outcome" (value or exception type) with the helper's outcome
        private static void AssertSameOutcome<T>(Func<T> dataSet, Func<T> helper, string what)
        {
            T expected = default(T), actual = default(T);
            Exception exExpected = null, exActual = null;
            try { expected = dataSet(); } catch (Exception ex) { exExpected = ex; }
            try { actual = helper(); } catch (Exception ex) { exActual = ex; }
            if (exExpected != null)
            {
                Assert.That(exActual, Is.Not.Null, what + ": DataSet threw " + exExpected.GetType().Name + " but helper did not");
                Assert.That(exActual.GetType(), Is.EqualTo(exExpected.GetType()), what);
            }
            else
            {
                Assert.That(exActual, Is.Null, what + ": helper threw " + exActual);
                Assert.That(actual, Is.EqualTo(expected), what);
            }
        }

        private static readonly string[] BoolForms = { "true", "false", "1", "0", "True", "False", "TRUE", " true ", "", "yes" };

        [Test]
        public void BooleanForms_MatchDataSet_IncludingRejections()
        {
            foreach (var form in BoolForms)
            {
                AssertSameOutcome(
                    () =>
                    {
                        XElement s, v, l;
                        var ds = LoadVariant("", "visible=\"" + Esc(form) + "\"", "x", out s, out v, out l);
                        return (bool)ds.Verse.Single()["visible"];
                    },
                    () =>
                    {
                        var v = XElement.Parse("<Verse visible=\"" + Esc(form) + "\" />");
                        return XmlRead.Bool(v, "visible", true);
                    },
                    "bool form [" + form + "]");
            }
        }

        private static readonly string[] DateForms =
        {
            "2026-10-03T12:34:56Z", "2026-10-03T12:34:56", "2026-10-03T12:34:56+02:00", "2026-10-03T12:34:56-05:00",
            "2026-10-03T12:34:56.1234567Z", "2026-10-03T12:34:56.123", "2026-10-03", "2026-10-03Z", " 2026-10-03T12:34:56Z ",
            "not a date", ""
        };

        [Test]
        public void DateForms_MatchDataSet_ValueKindAndRejections()
        {
            foreach (var form in DateForms)
            {
                AssertSameOutcome(
                    () =>
                    {
                        XElement s, v, l;
                        var ds = LoadVariant("stageDateTimeStamp=\"" + Esc(form) + "\"", "", "x", out s, out v, out l);
                        var d = (DateTime)ds.story.Single()["stageDateTimeStamp"];
                        return d.ToString("o") + " " + d.Kind;
                    },
                    () =>
                    {
                        var e = XElement.Parse("<story stageDateTimeStamp=\"" + Esc(form) + "\" />");
                        var d = XmlRead.Date(e, "stageDateTimeStamp").Value;
                        return d.ToString("o") + " " + d.Kind;
                    },
                    "date form [" + form + "]");
            }
        }

        // what the DataSet gave for each form (observed in Task 1), as literal expectations. An explicit offset is
        //  converted to the machine's local time, so that expectation is computed rather than hard-coded.
        [TestCase("2026-10-03T12:34:56Z", "2026-10-03T12:34:56.0000000")]
        [TestCase("2026-10-03T12:34:56", "2026-10-03T12:34:56.0000000")]
        [TestCase("2026-10-03T12:34:56.1234567Z", "2026-10-03T12:34:56.1234567")]
        [TestCase("2026-10-03T12:34:56.123", "2026-10-03T12:34:56.1230000")]
        [TestCase("2026-10-03", "2026-10-03T00:00:00.0000000")]
        [TestCase("2026-10-03Z", "2026-10-03T00:00:00.0000000")]
        [TestCase(" 2026-10-03T12:34:56Z ", "2026-10-03T12:34:56.0000000")]
        public void Date_HardCodedExpectations_ZoneLessAndZ(string strInput, string strExpectedRoundTrip)
        {
            var d = XmlRead.Date(XElement.Parse("<story stageDateTimeStamp=\"" + strInput + "\" />"), "stageDateTimeStamp").Value;
            Assert.That(d.ToString("o"), Is.EqualTo(strExpectedRoundTrip));
            Assert.That(d.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        [TestCase("2026-10-03T12:34:56+02:00")]
        [TestCase("2026-10-03T12:34:56-05:00")]
        public void Date_HardCodedExpectations_ExplicitOffsetBecomesLocalTime(string strInput)
        {
            var expected = DateTime.SpecifyKind(
                DateTimeOffset.Parse(strInput, System.Globalization.CultureInfo.InvariantCulture).LocalDateTime,
                DateTimeKind.Unspecified);
            var d = XmlRead.Date(XElement.Parse("<story stageDateTimeStamp=\"" + strInput + "\" />"), "stageDateTimeStamp").Value;
            Assert.That(d, Is.EqualTo(expected));
            Assert.That(d.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        [TestCase("not a date")]
        [TestCase("")]
        public void Date_HardCodedExpectations_BadValueThrowsFormatException(string strInput)
        {
            Assert.Throws<FormatException>(() =>
                XmlRead.Date(XElement.Parse("<story stageDateTimeStamp=\"" + strInput + "\" />"), "stageDateTimeStamp"));
        }

        private static readonly string[] IntForms = { "0", "2", "-3", "+4", " 5 ", "2.0", "abc", "" };

        [Test]
        public void IntForms_MatchDataSet_IncludingRejections()
        {
            foreach (var form in IntForms)
            {
                AssertSameOutcome(
                    () =>
                    {
                        XElement s, v, l;
                        var ds = LoadVariant("CountRetellingsTests=\"" + Esc(form) + "\"", "", "x", out s, out v, out l);
                        return (int)ds.story.Single()["CountRetellingsTests"];
                    },
                    () => XmlRead.Int(XElement.Parse("<story CountRetellingsTests=\"" + Esc(form) + "\" />"), "CountRetellingsTests", 0),
                    "int form [" + form + "]");
            }
        }

        [Test]
        public void TextForms_MatchDataSet_IncludingEntitiesAndNewlines()
        {
            var forms = new[] { "plain", "  padded  ", "   ", "a&amp;b &lt;x&gt;", "line1&#10;line2", "line1&#13;&#10;line2", "tab&#9;x", "&#xE9;&#x4E2D;", "&#160;", " x&#10;" };
            foreach (var form in forms)
            {
                XElement s, v, l;
                var ds = LoadVariant("", "", form, out s, out v, out l);
                Assert.That(XmlRead.Text(l), Is.EqualTo(RawString(ds.StoryLine.Single(), "StoryLine_text")), "text form [" + form + "]");
            }
        }
    }
}
