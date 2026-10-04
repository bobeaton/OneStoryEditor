using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    internal static class PaneTestData
    {
        // a private copy of a TestData project, loaded the way the app loads it
        public static StoryProjectData LoadProject(string strName = "characterization-project")
        {
            var strFolder = Path.Combine(Path.GetTempPath(), "OseB_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(strFolder);
            var strPath = Path.Combine(strFolder, strName + ".onestory");
            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", strName + ".onestory"), strPath);
            var contents = ProjectFile.Load(strPath);
            return new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, strName));
        }

        // the Consultant Notes page of the first story, for the first team member who gets 'Add Note' buttons
        internal static string ConsultantNotesPage(HtmlEngine engine)
        {
            var project = LoadProject();
            var story = Stories(project).First(s => s.Verses.Count > 0);
            // the app always has a project facilitator by the time a note pane is built; this test project's stories don't
            var pf = project.TeamMembers.Values.First(m => m.MemberType == TeamMemberData.UserTypes.ProjectFacilitator);
            story.CraftingInfo.ProjectFacilitator = new MemberIdInfo(pf.MemberGuid, null);
            string strHtml = null;
            foreach (var member in project.TeamMembers.Values)
            {
                strHtml = story.ConsultantNotesHtml(null, project.ProjSettings, member, project.TeamMembers,
                                                    false, false, null, null, engine);
                if (strHtml.Contains("data-action=\"" + NoteActions.AddNote + "\""))
                    break;
            }
            Assert.That(strHtml, Does.Contain("data-action=\"" + NoteActions.AddNote + "\""), "no member gets an Add Note button");
            return strHtml;
        }

        // the editable Story/BT page of the first story, with the four language columns and most rows showing
        internal static string StoryBtPage(HtmlEngine engine)
        {
            var project = LoadProject();
            var story = Stories(project).First(s => s.Verses.Count > 0);
            var viewSettings = new VerseData.ViewSettings(project.ProjSettings,
                true, true, true, true,     // the four language columns
                true, true, true, true, true,   // anchors, exegetical notes, TQs, answers, retellings
                false, false, false,        // consultant notes, coach notes, bible viewer
                true, false, false, true,   // front matter, hidden, only open conversations, general TQs
                true,                       // use textareas (the editable pane)
                (StoryEditor.TextFields)~0, // everything editable
                null, null, null, null);
            return story.PresentationHtml(viewSettings, project.ProjSettings, project.TeamMembers, null, engine);
        }

        public static IEnumerable<StoryData> Stories(StoryProjectData project)
        {
            return project.Values.Cast<StoriesData>().SelectMany(s => s.Cast<StoryData>());
        }
    }
}
