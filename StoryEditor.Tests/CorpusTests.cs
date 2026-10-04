using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Runs over real project files on this machine (not in the repo). Explicit only:
    ///   set OSE_CORPUS_DIRS to a ';'-separated list of folders, or it uses the defaults below.
    /// </summary>
    [TestFixture, Explicit("reads local project files")]
    public class CorpusTests
    {
        private static IEnumerable<string> CorpusFiles()
        {
            var strDirs = Environment.GetEnvironmentVariable("OSE_CORPUS_DIRS")
                          ?? (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OneStory Editor Projects")
                              + ";" + @"C:\btmp\OSE Projects");
            return strDirs.Split(';')
                          .Where(Directory.Exists)
                          .SelectMany(d => Directory.EnumerateFiles(d, "*.onestory", SearchOption.AllDirectories));
        }

        private static bool IsPastedCopyXml(string str) =>
            !String.IsNullOrEmpty(str) &&
            ((str.IndexOf("<" + StoryProjectData.CstrElementOseStoryToCopy, StringComparison.Ordinal) >= 0) ||
             (str.IndexOf("<" + StoryProjectData.CstrElementOseColumnToCopy, StringComparison.Ordinal) >= 0));

        [Test]
        public void EveryProjectLoadsDecodesAndSanitizes()
        {
            int nFiles = 0, nUnreadable = 0, nDecoded = 0, nNotes = 0, nFallbacks = 0, nPastedBlobs = 0;
            var lstProblems = new List<string>();

            foreach (var strFile in CorpusFiles())
            {
                nFiles++;
                ProjectFileContents contents;
                try
                {
                    contents = ProjectFile.Load(strFile);
                }
                catch (Exception ex)
                {
                    nUnreadable++;  // e.g. the known all-zero-bytes file
                    TestContext.WriteLine($"UNREADABLE {strFile}: {ex.Message}");
                    continue;
                }

                var root = contents.Root;
                nDecoded += LegacyTextRepair.DecodePlainTextElements(root);

                foreach (var strElement in new[] { "StoryLine", "Retelling", "Answer", "TestQuestionLine" })
                {
                    foreach (var elem in root.Descendants(strElement))
                    {
                        var str = XmlRead.Text(elem);
                        // a whole pasted OseStoryToCopy/OseColumnToCopy document (known junk) legitimately contains "&amp;" etc.
                        if (IsPastedCopyXml(str))
                            nPastedBlobs++;
                        else if (LegacyTextRepair.ContainsIeEntity(str))
                            lstProblems.Add($"{strFile} {strElement}: entity remains: {str}");
                    }
                }

                foreach (var strElement in new[] { "ConsultantNote", "CoachNote" })
                {
                    foreach (var elem in root.Descendants(strElement))
                    {
                        var str = XmlRead.Text(elem);
                        if (String.IsNullOrEmpty(str))
                            continue;
                        nNotes++;
                        string strResult;
                        if (!NoteHtmlSanitizer.TrySanitize(str, out strResult))
                            nFallbacks++;
                        if (strResult.IndexOf("<script", StringComparison.OrdinalIgnoreCase) >= 0)
                            lstProblems.Add($"{strFile} {strElement}: script survived");
                    }
                }
            }

            TestContext.WriteLine($"files={nFiles} unreadable={nUnreadable} decodedValues={nDecoded} notes={nNotes} sanitizerFallbacks={nFallbacks} pastedBlobsSkipped={nPastedBlobs}");
            Assert.That(nFiles, Is.GreaterThan(0), "no corpus files found");
            Assert.That(lstProblems, Is.Empty);
            Assert.That(nFallbacks, Is.EqualTo(0));
        }
    }
}
