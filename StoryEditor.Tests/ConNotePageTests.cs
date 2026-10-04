using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture, Apartment(ApartmentState.STA), Category("Browser")]
    public class ConNotePageTests
    {
        private const string CstrDriver =
            // (quirks-mode safe: the page has no querySelectorAll)
            "ose.on('clickFirst', function (m) { var els = [], all = document.getElementsByTagName('*');" +
            "  for (var i = 0; i < all.length; i++) if (all[i].nodeType == 1 && all[i].getAttribute('data-action') == m.action) els.push(all[i]);" +
            "  if (els.length) els[0].click(); return { n: els.length }; });" +
            "ose.on('mode', function () { return { mode: document.documentMode }; });" +
            "ose.on('countInline', function () { var n = 0, all = document.getElementsByTagName('*');" +
            "  for (var i = 0; i < all.length; i++) { var a = all[i].attributes; if (all[i].nodeType != 1 || !a) continue;" +
            "    for (var j = 0; j < a.length; j++) if (a[j].specified && /^on/i.test(a[j].name)) n++; } return { n: n }; });";

        private Form _form;
        private IeHtmlHost _host;
        private readonly List<HtmlMessage> _received = new List<HtmlMessage>();

        [SetUp]
        public void SetUp()
        {
            _received.Clear();
            _host = BrowserTestHelper.CreateHostInOffscreenForm(out _form);
            _host.MessageReceived += (s, m) => _received.Add(m);
        }

        [TearDown]
        public void TearDown()
        {
            _form.Close();
            _form.Dispose();
        }

        // the Consultant Notes page of the first story, for the first team member who gets 'Add Note' buttons
        private void LoadConsultantNotesPage()
        {
            var project = PaneTestData.LoadProject();
            var story = PaneTestData.Stories(project).First(s => s.Verses.Count > 0);
            // the app always has a project facilitator by the time a note pane is built; this test project's stories don't
            var pf = project.TeamMembers.Values.First(m => m.MemberType == TeamMemberData.UserTypes.ProjectFacilitator);
            story.CraftingInfo.ProjectFacilitator = new MemberIdInfo(pf.MemberGuid, null);
            string strHtml = null;
            foreach (var member in project.TeamMembers.Values)
            {
                strHtml = story.ConsultantNotesHtml(null, project.ProjSettings, member, project.TeamMembers,
                                                    false, false, null, null);
                if (strHtml.Contains("data-action=\"" + NoteActions.AddNote + "\""))
                    break;
            }
            Assert.That(strHtml, Does.Contain("data-action=\"" + NoteActions.AddNote + "\""), "no member gets an Add Note button");
            _host.LoadHtml(strHtml.Replace("</head>", PageScripts.ScriptBlock(CstrDriver) + "</head>"));
            Assert.That(BrowserTestHelper.PumpUntil(() => _host.IsReady), Is.True, "page never sent 'ready'");
        }

        [Test]
        public void Page_LoadsWithoutScriptErrors()
        {
            LoadConsultantNotesPage();
            BrowserTestHelper.Pump(300);
            Assert.That(_received.Where(m => m.Type == HtmlMessage.CstrTypeJsError).Select(m => m.GetString("message")), Is.Empty);
        }

        // the shipped app has always shown this page in quirks mode; the selection code depends on its text ranges
        [Test]
        public void Page_RunsInQuirksMode()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("mode", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("mode", out var nMode) && (nMode == 5), Is.True, "documentMode " + nMode);
        }

        [Test]
        public void Page_HasNoInlineEventHandlers()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("countInline", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("n", out var n) && (n == 0), Is.True, "inline on* attributes left: " + n);
        }

        [Test]
        public void AddNoteButton_SendsActionWithLineIndexId()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("clickFirst", new { action = NoteActions.AddNote }, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply.TryGetInt("n", out var n) && n > 0, Is.True);
            Assert.That(BrowserTestHelper.PumpUntil(() => _received.Exists(m => m.Type == "action")), Is.True);
            var msg = _received.First(m => m.Type == "action");
            Assert.That(msg.GetString("name"), Is.EqualTo(NoteActions.AddNote));
            Assert.That(msg.TryGetInt("id", out _), Is.True, "the Add Note button's id is the bare line index");
        }

        [Test]
        public void GetNoteSelection_NothingSelected_GivesNoLine()
        {
            LoadConsultantNotesPage();
            var reply = _host.Request("getNoteSelection", null, HtmlHostDefaults.RequestTimeout);
            Assert.That(reply, Is.Not.Null);
            Assert.That(reply.TryGetInt("lineIndex", out _), Is.False);
        }

        [Test]
        public void Flush_Replies()
        {
            LoadConsultantNotesPage();
            Assert.That(_host.Request("flush", null, HtmlHostDefaults.RequestTimeout), Is.Not.Null);
        }
    }
}
