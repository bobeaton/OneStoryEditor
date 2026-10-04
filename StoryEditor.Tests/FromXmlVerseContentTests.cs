using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using OneStoryProjectEditor.Tests.ReleasedExeDataSet;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Golden tests for the verse-content XElement constructors: each test loads characterization.onestory as an
    /// XDocument, builds each verse-content object and compares what it holds (every field the old row-constructor
    /// oracle tests compared, and the object's GetXml) with a golden file (see Golden). Where the old test also
    /// asserted a value, the assertion is kept.
    /// </summary>
    [TestFixture]
    public class FromXmlVerseContentTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization.onestory");

        private XDocument _doc;
        private List<XElement> _verseElems;

        [SetUp]
        public void Load()
        {
            _doc = XDocument.Load(FixturePath, LoadOptions.None);
            _verseElems = _doc.Descendants("Verse").ToList();
            Assert.That(_verseElems.Count, Is.EqualTo(8));
        }

        [Test]
        public void AnchorsData_Golden()
        {
            var dump = new Dump();
            var nWithData = 0;
            var bSawKeyTermChecked = false;
            var bSawKeyTermUnchecked = false;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var anchors = new AnchorsData(_verseElems[i]);
                dump.Raw($"== verse {i}").Line("Count", anchors.Count).Line("IsKeyTermChecked", anchors.IsKeyTermChecked)
                    .Line("HasData", anchors.HasData);
                for (int j = 0; j < anchors.Count; j++)
                    dump.Line($"anchor {j} JumpTarget", anchors[j].JumpTarget).Line($"anchor {j} ToolTipText", anchors[j].ToolTipText);
                if (!anchors.HasData)
                    continue;

                nWithData++;
                bSawKeyTermChecked |= anchors.IsKeyTermChecked;
                bSawKeyTermUnchecked |= !anchors.IsKeyTermChecked;
                dump.Xml("GetXml", anchors.GetXml);
            }
            Golden.Check("verse-anchors", dump.ToString());
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
        public void ExegeticalHelpNotesData_Golden_IncludingDuplicates()
        {
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var notes = new ExegeticalHelpNotesData(_verseElems[i]);
                dump.Raw($"== verse {i}").Line("Count", notes.Count).Line("HasData", notes.HasData);
                for (int j = 0; j < notes.Count; j++)
                    dump.Line($"note {j} HasData", notes[j].HasData).Line($"note {j} ToString", notes[j].ToString())
                        .Line($"note {j} Value", notes[j].Value);
                if (!notes.HasData)
                    continue;

                nWithData++;
                dump.Xml("GetXml", notes.GetXml);
            }
            Golden.Check("verse-exegetical-helps", dump.ToString());
            Assert.That(nWithData, Is.GreaterThanOrEqualTo(1));

            // the duplicate and the empty note are kept by the loader (GetXml is what drops the duplicate)
            Assert.That(new ExegeticalHelpNotesData(_verseElems[5]).Count, Is.EqualTo(4));
        }

        private static void DumpLine(Dump dump, string strWhat, LineData line)
        {
            dump.Line(strWhat + " vern", line.Vernacular.Value).Line(strWhat + " nat", line.NationalBt.Value)
                .Line(strWhat + " intl", line.InternationalBt.Value).Line(strWhat + " free", line.FreeTranslation.Value);
        }

        private static void DumpMultipleLines(Dump dump, string strWhat, MultipleLineDataConverter lines)
        {
            dump.Line(strWhat + " Count", lines.Count).Line(strWhat + " HasData", lines.HasData);
            for (int j = 0; j < lines.Count; j++)
            {
                dump.Line($"{strWhat} line {j} MemberId", lines[j].MemberId);
                DumpLine(dump, $"{strWhat} line {j}", lines[j]);
            }
            if (lines.HasData)
                dump.Xml(strWhat + " GetXml", lines.GetXml);
        }

        [Test]
        public void TestQuestionsData_Golden()
        {
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var tqs = new TestQuestionsData(_verseElems[i]);
                dump.Raw($"== verse {i}").Line("Count", tqs.Count).Line("HasData", tqs.HasData);
                for (int j = 0; j < tqs.Count; j++)
                {
                    var tq = tqs[j];
                    var strWhat = $"tq {j}";
                    dump.Line(strWhat + " guid", tq.guid).Line(strWhat + " IsVisible", tq.IsVisible)
                        .Line(strWhat + " HasData", tq.HasData);
                    DumpLine(dump, strWhat, tq.TestQuestionLine);
                    DumpMultipleLines(dump, strWhat + " answers", tq.Answers);
                    if (tq.HasData)
                        dump.Xml(strWhat + " GetXml", tq.GetXml);
                }
                if (!tqs.HasData)
                    continue;

                nWithData++;
                dump.Xml("GetXml", tqs.GetXml);
            }
            Golden.Check("verse-test-questions", dump.ToString());
            Assert.That(nWithData, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void RetellingsData_Golden()
        {
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var retellings = new RetellingsData(_verseElems[i]);
                DumpMultipleLines(dump.Raw($"== verse {i}"), "retellings", retellings);
                if (retellings.HasData)
                    nWithData++;
            }
            Golden.Check("verse-retellings", dump.ToString());
            Assert.That(nWithData, Is.EqualTo(1));
        }

        [Test]
        public void AnswersData_Golden()
        {
            var elems = _doc.Descendants("TestQuestion").ToList();
            Assert.That(elems.Count, Is.EqualTo(2));
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < elems.Count; i++)
            {
                var answers = new AnswersData(elems[i]);
                DumpMultipleLines(dump.Raw($"== tq {i}"), "answers", answers);
                if (answers.HasData)
                    nWithData++;
            }
            Golden.Check("verse-answers", dump.ToString());
            Assert.That(nWithData, Is.EqualTo(1));
        }

        [Test]
        public void ConsultantNotesData_Golden()
        {
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var notes = new ConsultantNotesData(_verseElems[i]);
                DumpConversations(dump.Raw($"== verse {i}"), notes);
                if (notes.Count > 0)
                    nWithData++;
            }
            Golden.Check("verse-consultant-notes", dump.ToString());
            Assert.That(nWithData, Is.EqualTo(3));
        }

        [Test]
        public void CoachNotesData_Golden()
        {
            var dump = new Dump();
            var nWithData = 0;
            for (int i = 0; i < _verseElems.Count; i++)
            {
                var notes = new CoachNotesData(_verseElems[i]);
                DumpConversations(dump.Raw($"== verse {i}"), notes);
                if (notes.Count > 0)
                    nWithData++;
            }
            Golden.Check("verse-coach-notes", dump.ToString());
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
        public void ConsultantNoteData_EmptyNote_Throws()
        {
            var doc = XDocument.Load(FixturePath, LoadOptions.None);
            var elemNote = doc.Descendants("ConsultantNote").First();
            elemNote.Value = "";
            var strTemp = Path.Combine(Path.GetTempPath(), "ose-emptynote-" + Guid.NewGuid() + ".onestory");
            try
            {
                doc.Save(strTemp);
                // the released exe's typed DataSet refuses the whole file (non-null constraint on the note text)
                Assert.That(() => new NewDataSet().ReadXml(strTemp), Throws.Exception);

                var elemConversation = doc.Descendants("ConsultantConversation").First();
                Assert.That(() => new ConsultantNoteData(elemConversation), Throws.TypeOf<ApplicationException>());
            }
            finally
            {
                File.Delete(strTemp);
            }
        }

        [Test]
        public void TestQuestionData_MissingVisible_Throws()
        {
            var doc = XDocument.Load(FixturePath, LoadOptions.None);
            var elemTq = doc.Descendants("TestQuestion").First();
            elemTq.Attribute("visible").Remove();
            var strTemp = Path.Combine(Path.GetTempPath(), "ose-novisible-" + Guid.NewGuid() + ".onestory");
            try
            {
                doc.Save(strTemp);
                // the released exe's typed DataSet loads the file, but its typed 'visible' getter throws when it's absent
                var ds = new NewDataSet();
                ds.ReadXml(strTemp);
                var row = ds.TestQuestion.First();
                Assert.That(() => row.visible, Throws.Exception);
                Assert.That(() => new TestQuestionData(elemTq), Throws.TypeOf<ApplicationException>());
            }
            finally
            {
                File.Delete(strTemp);
            }
        }

        private static void DumpConversations(Dump dump, ConsultNotesDataConverter conversations)
        {
            dump.Line("Count", conversations.Count);
            for (int j = 0; j < conversations.Count; j++)
            {
                var conv = conversations[j];
                var strWhat = $"conversation {j}";
                dump.Line(strWhat + " guid", conv.guid).Line(strWhat + " Visible", conv.Visible)
                    .Line(strWhat + " IsFinished", conv.IsFinished)
                    .Line(strWhat + " AllowButtonsOverride", conv.AllowButtonsOverride)
                    .Line(strWhat + " DontShowButtonsOverride", conv.DontShowButtonsOverride)
                    .Line(strWhat + " Count", conv.Count);
                DumpComment(dump, strWhat + " ReferringText", conv.ReferringText);
                for (int k = 0; k < conv.Count; k++)
                    DumpComment(dump, $"{strWhat} comment {k}", conv[k]);
                if (conv.Count > 0)
                    dump.Xml(strWhat + " GetXml", conv.GetXml);
            }
            if (conversations.Count > 0)
                dump.Xml("GetXml", conversations.GetXml);
        }

        private static void DumpComment(Dump dump, string strWhat, CommInstance comment)
        {
            if (comment == null)
            {
                dump.Line(strWhat, null);
                return;
            }
            dump.Line(strWhat + " Direction", comment.Direction).Line(strWhat + " Guid", comment.Guid)
                .Line(strWhat + " MemberId", comment.MemberId).Line(strWhat + " Value", comment.Value)
                .Line(strWhat + " WhichField", comment.WhichField).Line(strWhat + " TimeStamp", comment.TimeStamp);
        }
    }
}
