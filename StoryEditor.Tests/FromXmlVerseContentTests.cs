using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// The typed DataSet row constructors are the oracle: every test loads characterization.onestory both as a
    /// DataSet and as an XDocument, builds each verse-content object both ways and requires identical results.
    /// A fresh ProjectReader per test, because the row constructors add empty container rows to the DataSet.
    /// </summary>
    [TestFixture]
    public class FromXmlVerseContentTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization.onestory");

        private ProjectReader _ds;
        private XDocument _doc;
        private List<NewDataSet.VerseRow> _verseRows;
        private List<XElement> _verseElems;

        [SetUp]
        public void Load()
        {
            ProjectReader.ReadProjectFile(FixturePath, out _ds);
            _doc = XDocument.Load(FixturePath, LoadOptions.None);
            _verseRows = _ds.Verse.ToList();
            _verseElems = _doc.Descendants("Verse").ToList();
            Assert.That(_verseElems.Count, Is.EqualTo(_verseRows.Count));
        }

        [Test]
        public void AnchorsData_MatchesRowPath()
        {
            var nWithData = 0;
            var bSawKeyTermChecked = false;
            var bSawKeyTermUnchecked = false;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldAnchors = new AnchorsData(_verseRows[i], _ds);
                var newAnchors = new AnchorsData(_verseElems[i]);
                Assert.That(newAnchors.Count, Is.EqualTo(oldAnchors.Count), $"verse {i}");
                Assert.That(newAnchors.IsKeyTermChecked, Is.EqualTo(oldAnchors.IsKeyTermChecked), $"verse {i}");
                for (int j = 0; j < oldAnchors.Count; j++)
                {
                    Assert.That(newAnchors[j].JumpTarget, Is.EqualTo(oldAnchors[j].JumpTarget), $"verse {i} anchor {j}");
                    Assert.That(newAnchors[j].ToolTipText, Is.EqualTo(oldAnchors[j].ToolTipText), $"verse {i} anchor {j}");
                }
                if (!oldAnchors.HasData)
                    continue;

                nWithData++;
                bSawKeyTermChecked |= oldAnchors.IsKeyTermChecked;
                bSawKeyTermUnchecked |= !oldAnchors.IsKeyTermChecked;
                Assert.That(newAnchors.GetXml.ToString(), Is.EqualTo(oldAnchors.GetXml.ToString()), $"verse {i}");
            }
            Assert.That(nWithData, Is.GreaterThanOrEqualTo(3));
            Assert.That(bSawKeyTermChecked && bSawKeyTermUnchecked, Is.True, "fixture should cover keyTermChecked both ways");
        }

        [Test]
        public void AnchorData_EmptyTextFallsBackToJumpTarget()
        {
            var elem = XElement.Parse("<Anchor jumpTarget=\"GEN 1:2\">   </Anchor>");
            var anchor = new AnchorData(elem);
            Assert.That(anchor.JumpTarget, Is.EqualTo("GEN 1:2"));
            Assert.That(anchor.ToolTipText, Is.EqualTo("GEN 1:2"));
        }

        [Test]
        public void AnchorData_MissingJumpTarget_Throws()
        {
            Assert.Throws<ApplicationException>(() => new AnchorData(XElement.Parse("<Anchor>x</Anchor>")));
        }

        [Test]
        public void ExegeticalHelpNotesData_MatchesRowPath_IncludingDuplicates()
        {
            var nWithData = 0;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldNotes = new ExegeticalHelpNotesData(_verseRows[i], _ds);
                var newNotes = new ExegeticalHelpNotesData(_verseElems[i]);
                Assert.That(newNotes.Count, Is.EqualTo(oldNotes.Count), $"verse {i}");
                for (int j = 0; j < oldNotes.Count; j++)
                {
                    Assert.That(newNotes[j].HasData, Is.EqualTo(oldNotes[j].HasData), $"verse {i} note {j}");
                    Assert.That(newNotes[j].ToString(), Is.EqualTo(oldNotes[j].ToString()), $"verse {i} note {j}");
                    Assert.That(newNotes[j].Value, Is.EqualTo(oldNotes[j].Value), $"verse {i} note {j} raw");
                }
                if (!oldNotes.HasData)
                    continue;

                nWithData++;
                Assert.That(newNotes.GetXml.ToString(), Is.EqualTo(oldNotes.GetXml.ToString()), $"verse {i}");
            }
            Assert.That(nWithData, Is.GreaterThanOrEqualTo(1));

            // the duplicate and the empty note are kept by the loader (GetXml is what drops the duplicate)
            Assert.That(new ExegeticalHelpNotesData(_verseElems[5]).Count, Is.EqualTo(4));
        }

        [Test]
        public void TestQuestionsData_MatchesRowPath()
        {
            var nWithData = 0;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldTqs = new TestQuestionsData(_verseRows[i], _ds);
                var newTqs = new TestQuestionsData(_verseElems[i]);
                Assert.That(newTqs.Count, Is.EqualTo(oldTqs.Count), $"verse {i}");
                for (int j = 0; j < oldTqs.Count; j++)
                {
                    var oldTq = oldTqs[j];
                    var newTq = newTqs[j];
                    Assert.That(newTq.guid, Is.EqualTo(oldTq.guid), $"verse {i} tq {j}");
                    Assert.That(newTq.IsVisible, Is.EqualTo(oldTq.IsVisible), $"verse {i} tq {j}");
                    Assert.That(newTq.HasData, Is.EqualTo(oldTq.HasData), $"verse {i} tq {j}");
                    AssertSameLine(oldTq.TestQuestionLine, newTq.TestQuestionLine, $"verse {i} tq {j}");
                    AssertSameMultipleLines(oldTq.Answers, newTq.Answers, $"verse {i} tq {j} answers");
                    if (oldTq.HasData)
                        Assert.That(newTq.GetXml.ToString(), Is.EqualTo(oldTq.GetXml.ToString()), $"verse {i} tq {j}");
                }
                if (!oldTqs.HasData)
                    continue;

                nWithData++;
                Assert.That(newTqs.GetXml.ToString(), Is.EqualTo(oldTqs.GetXml.ToString()), $"verse {i}");
            }
            Assert.That(nWithData, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void RetellingsData_MatchesRowPath()
        {
            var nWithData = 0;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldRetellings = new RetellingsData(_verseRows[i], _ds);
                var newRetellings = new RetellingsData(_verseElems[i]);
                AssertSameMultipleLines(oldRetellings, newRetellings, $"verse {i}");
                if (oldRetellings.HasData)
                    nWithData++;
            }
            Assert.That(nWithData, Is.EqualTo(1));
        }

        [Test]
        public void AnswersData_MatchesRowPath()
        {
            var oldRows = _ds.TestQuestion.ToList();
            var newElems = _doc.Descendants("TestQuestion").ToList();
            Assert.That(newElems.Count, Is.EqualTo(oldRows.Count));
            Assert.That(oldRows.Count, Is.EqualTo(2));
            var nWithData = 0;
            for (int i = 0; i < oldRows.Count; i++)
            {
                var oldAnswers = new AnswersData(oldRows[i], _ds);
                var newAnswers = new AnswersData(newElems[i]);
                AssertSameMultipleLines(oldAnswers, newAnswers, $"tq {i}");
                if (oldAnswers.HasData)
                    nWithData++;
            }
            Assert.That(nWithData, Is.EqualTo(1));
        }

        [Test]
        public void ConsultantNotesData_MatchesRowPath()
        {
            var nWithData = 0;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldNotes = new ConsultantNotesData(_verseRows[i], _ds);
                var newNotes = new ConsultantNotesData(_verseElems[i]);
                AssertSameConversations(oldNotes, newNotes, $"verse {i}");
                if (oldNotes.Count > 0)
                    nWithData++;
            }
            Assert.That(nWithData, Is.EqualTo(3));
        }

        [Test]
        public void CoachNotesData_MatchesRowPath()
        {
            var nWithData = 0;
            for (int i = 0; i < _verseRows.Count; i++)
            {
                var oldNotes = new CoachNotesData(_verseRows[i], _ds);
                var newNotes = new CoachNotesData(_verseElems[i]);
                AssertSameConversations(oldNotes, newNotes, $"verse {i}");
                if (oldNotes.Count > 0)
                    nWithData++;
            }
            Assert.That(nWithData, Is.EqualTo(1));
        }

        [Test]
        public void ConsultantNoteData_FixtureFlagsAndSpecialComments()
        {
            // verse 5 of the fixture is the "Story Content" verse that has every kind of conversation
            var notes = new ConsultantNotesData(_verseElems[5]);
            Assert.That(notes.Count, Is.EqualTo(3));

            Assert.That(notes[0].Visible, Is.False);
            Assert.That(notes[0].IsFinished, Is.True);
            Assert.That(notes[0].ReferringText, Is.Not.Null);
            Assert.That(notes[0].ReferringText.ToString(), Is.EqualTo("referring text"));
            Assert.That(notes[0].ReferringText.Direction, Is.EqualTo(ConsultNoteDataConverter.CommunicationDirections.eReferringToText));
            Assert.That(notes[0].Count, Is.EqualTo(2), "the ReferringToText comment is not in the list");

            Assert.That(notes[1].Visible, Is.True);
            Assert.That(notes[1].IsFinished, Is.False);
            Assert.That(notes[1][0].Direction, Is.EqualTo(ConsultNoteDataConverter.CommunicationDirections.eStickyNote));

            Assert.That(notes[2].Visible, Is.True, "visible absent defaults to true");
            Assert.That(notes[2].IsFinished, Is.False, "finished absent defaults to false");
            Assert.That(notes[2][0].MemberId, Is.Null, "a mentor comment without memberID");
            Assert.That(notes[2][1].TimeStamp, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromMinutes(1)), "no timeStamp means now");

            var coach = new CoachNotesData(_verseElems[5]);
            Assert.That(coach.Count, Is.EqualTo(2));
            Assert.That(coach[0].IsFinished, Is.True);
            Assert.That(coach[0].Visible, Is.False);
            Assert.That(coach[0].ReferringText.ToString(), Is.EqualTo("coach referring"));
            Assert.That(coach[0][0].MemberId, Is.Null);
            Assert.That(coach[1][0].Direction, Is.EqualTo(ConsultNoteDataConverter.CommunicationDirections.eStickyNote));
        }

        [Test]
        public void ConsultantNoteData_EmptyNote_ThrowsOnBothPaths()
        {
            var doc = XDocument.Load(FixturePath, LoadOptions.None);
            var elemNote = doc.Descendants("ConsultantNote").First();
            elemNote.Value = "";
            var strTemp = Path.Combine(Path.GetTempPath(), "ose-emptynote-" + Guid.NewGuid() + ".onestory");
            try
            {
                doc.Save(strTemp);
                // the DataSet refuses the whole file (non-null constraint on the note text), so the row
                //  constructor never even sees such a note
                ProjectReader ds;
                Assert.That(() => ProjectReader.ReadProjectFile(strTemp, out ds), Throws.Exception);

                var elemConversation = doc.Descendants("ConsultantConversation").First();
                Assert.That(() => new ConsultantNoteData(elemConversation), Throws.TypeOf<ApplicationException>());
            }
            finally
            {
                File.Delete(strTemp);
            }
        }

        [Test]
        public void TestQuestionData_MissingVisible_ThrowsOnBothPaths()
        {
            var doc = XDocument.Load(FixturePath, LoadOptions.None);
            var elemTq = doc.Descendants("TestQuestion").First();
            elemTq.Attribute("visible").Remove();
            var strTemp = Path.Combine(Path.GetTempPath(), "ose-novisible-" + Guid.NewGuid() + ".onestory");
            try
            {
                doc.Save(strTemp);
                ProjectReader ds;
                ProjectReader.ReadProjectFile(strTemp, out ds);
                var row = ds.TestQuestion.First();
                Assert.That(() => new TestQuestionData(row, ds), Throws.Exception, "the row path throws for an absent visible");
                Assert.That(() => new TestQuestionData(elemTq), Throws.TypeOf<ApplicationException>());
            }
            finally
            {
                File.Delete(strTemp);
            }
        }

        private static void AssertSameLine(LineData expected, LineData actual, string what)
        {
            Assert.That(actual.Vernacular.Value, Is.EqualTo(expected.Vernacular.Value), what + " vern");
            Assert.That(actual.NationalBt.Value, Is.EqualTo(expected.NationalBt.Value), what + " nat");
            Assert.That(actual.InternationalBt.Value, Is.EqualTo(expected.InternationalBt.Value), what + " intl");
            Assert.That(actual.FreeTranslation.Value, Is.EqualTo(expected.FreeTranslation.Value), what + " free");
        }

        private static void AssertSameMultipleLines(MultipleLineDataConverter expected, MultipleLineDataConverter actual, string what)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), what);
            for (int j = 0; j < expected.Count; j++)
            {
                Assert.That(actual[j].MemberId, Is.EqualTo(expected[j].MemberId), $"{what} line {j}");
                AssertSameLine(expected[j], actual[j], $"{what} line {j}");
            }
            if (expected.HasData)
                Assert.That(actual.GetXml.ToString(), Is.EqualTo(expected.GetXml.ToString()), what);
        }

        private static void AssertSameConversations(ConsultNotesDataConverter expected, ConsultNotesDataConverter actual, string what)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), what);
            for (int j = 0; j < expected.Count; j++)
            {
                var oldConv = expected[j];
                var newConv = actual[j];
                var strWhat = $"{what} conversation {j}";
                Assert.That(newConv.guid, Is.EqualTo(oldConv.guid), strWhat);
                Assert.That(newConv.Visible, Is.EqualTo(oldConv.Visible), strWhat + " Visible");
                Assert.That(newConv.IsFinished, Is.EqualTo(oldConv.IsFinished), strWhat + " IsFinished");
                Assert.That(newConv.AllowButtonsOverride, Is.EqualTo(oldConv.AllowButtonsOverride), strWhat);
                Assert.That(newConv.DontShowButtonsOverride, Is.EqualTo(oldConv.DontShowButtonsOverride), strWhat);
                Assert.That(newConv.Count, Is.EqualTo(oldConv.Count), strWhat);
                AssertSameComment(oldConv.ReferringText, newConv.ReferringText, strWhat + " ReferringText");
                for (int k = 0; k < oldConv.Count; k++)
                    AssertSameComment(oldConv[k], newConv[k], $"{strWhat} comment {k}");
                if (oldConv.Count > 0)
                    Assert.That(newConv.GetXml.ToString(), Is.EqualTo(oldConv.GetXml.ToString()), strWhat);
            }
            if (expected.Count > 0)
                Assert.That(actual.GetXml.ToString(), Is.EqualTo(expected.GetXml.ToString()), what);
        }

        private static void AssertSameComment(CommInstance expected, CommInstance actual, string what)
        {
            if (expected == null)
            {
                Assert.That(actual, Is.Null, what);
                return;
            }
            Assert.That(actual, Is.Not.Null, what);
            Assert.That(actual.Direction, Is.EqualTo(expected.Direction), what);
            Assert.That(actual.Guid, Is.EqualTo(expected.Guid), what);
            Assert.That(actual.MemberId, Is.EqualTo(expected.MemberId), what);
            Assert.That(actual.Value, Is.EqualTo(expected.Value), what);
            Assert.That(actual.WhichField, Is.EqualTo(expected.WhichField), what);
            Assert.That(actual.TimeStamp.Kind, Is.EqualTo(expected.TimeStamp.Kind), what + " Kind");
            // no-timeStamp comments are stamped with "now" on each path, so allow a little slack
            Assert.That(actual.TimeStamp, Is.EqualTo(expected.TimeStamp).Within(TimeSpan.FromSeconds(30)), what + " TimeStamp");
        }
    }
}
