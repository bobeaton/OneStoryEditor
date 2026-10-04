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

        public static IEnumerable<StoryData> Stories(StoryProjectData project)
        {
            return project.Values.Cast<StoriesData>().SelectMany(s => s.Cast<StoryData>());
        }
    }
}
