using System;
using System.Collections.Generic;
using System.Data;
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

        [Test]
        public void EveryProjectLoadsDecodesAndSanitizes()
        {
            int nFiles = 0, nUnreadable = 0, nDecoded = 0, nNotes = 0, nFallbacks = 0;
            var lstProblems = new List<string>();

            foreach (var strFile in CorpusFiles())
            {
                nFiles++;
                ProjectReader projFile;
                try
                {
                    ProjectReader.ReadProjectFile(strFile, out projFile);
                }
                catch (Exception ex)
                {
                    nUnreadable++;  // e.g. the known all-zero-bytes file
                    TestContext.WriteLine($"UNREADABLE {strFile}: {ex.Message}");
                    continue;
                }

                nDecoded += LegacyTextRepair.DecodePlainTextFields(projFile);

                foreach (var strTable in new[] { "StoryLine", "Retelling", "Answer", "TestQuestionLine" })
                {
                    var table = projFile.Tables[strTable];
                    if (table == null)
                        continue;
                    foreach (DataRow row in table.Rows)
                    {
                        var str = row[strTable + "_text"] as string;
                        if (LegacyTextRepair.ContainsIeEntity(str))
                            lstProblems.Add($"{strFile} {strTable}: entity remains: {str}");
                    }
                }

                foreach (var strTable in new[] { "ConsultantNote", "CoachNote" })
                {
                    var table = projFile.Tables[strTable];
                    if (table == null)
                        continue;
                    foreach (DataRow row in table.Rows)
                    {
                        var str = row[strTable + "_text"] as string;
                        if (String.IsNullOrEmpty(str))
                            continue;
                        nNotes++;
                        string strResult;
                        if (!NoteHtmlSanitizer.TrySanitize(str, out strResult))
                            nFallbacks++;
                        if (strResult.IndexOf("<script", StringComparison.OrdinalIgnoreCase) >= 0)
                            lstProblems.Add($"{strFile} {strTable}: script survived");
                    }
                }
            }

            TestContext.WriteLine($"files={nFiles} unreadable={nUnreadable} decodedValues={nDecoded} notes={nNotes} sanitizerFallbacks={nFallbacks}");
            Assert.That(nFiles, Is.GreaterThan(0), "no corpus files found");
            Assert.That(lstProblems, Is.Empty);
            Assert.That(nFallbacks, Is.EqualTo(0));
        }
    }
}
