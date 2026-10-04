using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Old loader (typed DataSet + row constructors) versus new loader (XElement constructors) on every real
    /// project file on this machine. The row path is the oracle; every difference is a bug in the new code.
    /// Explicit only: set OSE_CORPUS_DIRS to a ';'-separated list of folders, or it uses the defaults.
    /// </summary>
    [TestFixture, Explicit("reads local project files")]
    public class DifferentialCorpusTests
    {
        private System.Diagnostics.TraceListener[] _savedListeners;

        [SetUp]
        public void SilenceTrace()
        {
            // real projects contain duplicate story guids; the mirrored Debug.Assert(false) would kill the test host
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
        }

        [TearDown]
        public void RestoreTrace()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(_savedListeners);
        }

        private static IEnumerable<string> CorpusFiles()
        {
            var strDirs = Environment.GetEnvironmentVariable("OSE_CORPUS_DIRS")
                          ?? (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OneStory Editor Projects")
                              + ";" + @"C:\btmp\OSE Projects");
            return strDirs.Split(';')
                          .Where(Directory.Exists)
                          .SelectMany(d => Directory.EnumerateFiles(d, "*.onestory", SearchOption.AllDirectories));
        }

        // the row path shows a message box for a duplicate member name (only the first <Members> counts there)
        private static string FindDuplicateMemberName(XElement root)
        {
            var elemMembers = root.Elements("Members").FirstOrDefault();
            if (elemMembers == null)
                return null;
            var setNames = new HashSet<string>();
            foreach (var elemMember in elemMembers.Elements("Member"))
            {
                var strName = (string)elemMember.Attribute("name");
                if (strName != null && !setNames.Add(strName))
                    return strName;
            }
            return null;
        }

        private static string FirstDifference(string strOld, string strNew)
        {
            var linesOld = strOld.Split('\n');
            var linesNew = strNew.Split('\n');
            var n = Math.Min(linesOld.Length, linesNew.Length);
            var i = 0;
            while (i < n && linesOld[i] == linesNew[i])
                i++;
            var from = Math.Max(0, i - 3);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"first differing line {i + 1} (old {linesOld.Length} lines, new {linesNew.Length} lines)");
            sb.AppendLine("--- old");
            for (int j = from; j <= Math.Min(linesOld.Length - 1, i + 3); j++)
                sb.AppendLine((j == i ? ">> " : "   ") + Trim(linesOld[j]));
            sb.AppendLine("--- new");
            for (int j = from; j <= Math.Min(linesNew.Length - 1, i + 3); j++)
                sb.AppendLine((j == i ? ">> " : "   ") + Trim(linesNew[j]));
            return sb.ToString();
        }

        private static string Trim(string str)
        {
            str = str.TrimEnd('\r');
            return str.Length > 400 ? str.Substring(0, 400) + "..." : str;
        }

        [Test]
        public void NewLoaderMatchesOldLoader_OnEveryProject()
        {
            int nFiles = 0, nCompared = 0, nSkipped = 0;
            long nChars = 0;
            int nBothThrew = 0, nMarkedPlain = 0, nDecodePath = 0;
            var lstDiffering = new List<string>();

            foreach (var strFile in CorpusFiles())
            {
                nFiles++;
                var strFolder = Path.GetDirectoryName(strFile);
                var strName = Path.GetFileNameWithoutExtension(strFile);

                // ----- old -----
                ProjectReader ds;
                try
                {
                    ProjectReader.ReadProjectFile(strFile, out ds);
                    if (ds == null)
                        throw new ApplicationException("ReadProjectFile returned null");
                }
                catch (Exception ex)
                {
                    nSkipped++;
                    TestContext.WriteLine($"SKIP (old loader can't read it) {strFile}: {ex.GetType().Name}: {ex.Message}");
                    try
                    {
                        ProjectFile.Load(strFile);
                        lstDiffering.Add(strFile);
                        TestContext.WriteLine($"DIFF {strFile}: new loader read a file the old loader could not");
                    }
                    catch (Exception exNew)
                    {
                        TestContext.WriteLine($"  (new loader also throws {exNew.GetType().Name}: {exNew.Message})");
                    }
                    continue;
                }

                // ----- new -----
                ProjectFileContents contents;
                try
                {
                    contents = ProjectFile.Load(strFile);
                }
                catch (Exception ex)
                {
                    nCompared++;
                    lstDiffering.Add(strFile);
                    TestContext.WriteLine($"DIFF {strFile}: new loader threw {ex.GetType().Name}: {ex.Message} (old loader read it)");
                    continue;
                }

                var strDuplicate = FindDuplicateMemberName(contents.Root);
                if (strDuplicate != null)
                {
                    nSkipped++;
                    TestContext.WriteLine($"SKIP (duplicate member name '{strDuplicate}' would show a message box) {strFile}");
                    continue;
                }

                nCompared++;
                if (contents.IsPlainTextEncoded)
                    nMarkedPlain++;
                else
                    nDecodePath++;
                try
                {
                    // same repairs on both sides: decode unless marked, placeholders always
                    if (!ds.IsPlainTextEncoded)
                        LegacyTextRepair.DecodePlainTextFields(ds);
                    LegacyTextRepair.ClearLanguageNamePlaceholders(ds);
                    if (!contents.IsPlainTextEncoded)
                        LegacyTextRepair.DecodePlainTextElements(contents.Root);
                    LegacyTextRepair.ClearLanguageNamePlaceholders(contents.Root);

                    string strFirstDiff = null;
                    Action<string, Func<string>, Func<string>> compare = (strObject, fOld, fNew) =>
                    {
                        if (strFirstDiff != null)
                            return;
                        string strOld, strNew;
                        bool bOldThrew = false, bNewThrew = false;
                        try { strOld = fOld(); }
                        catch (Exception ex) { bOldThrew = true; strOld = "EXCEPTION " + ex.GetType().Name + ": " + ex.Message; }
                        try { strNew = fNew(); }
                        catch (Exception ex) { bNewThrew = true; strNew = "EXCEPTION " + ex.GetType().Name + ": " + ex.Message; }
                        if (bOldThrew && bNewThrew)
                        {
                            nBothThrew++;
                            TestContext.WriteLine($"BOTH THREW {strFile} {strObject}: {strOld}");
                        }
                        nChars += strOld.Length;
                        if (strOld != strNew)
                            strFirstDiff = $"{strObject}: " + FirstDifference(strOld, strNew);
                    };

                    compare("TeamMembersData", () => new TeamMembersData(ds).GetXml.ToString(),
                            () => new TeamMembersData(contents.Root).GetXml.ToString());
                    compare("LnCNotesData", () => new LnCNotesData(ds).GetXml.ToString(),
                            () => new LnCNotesData(contents.Root).GetXml.ToString());
                    compare("ProjectSettings",
                            () =>
                            {
                                var s = new ProjectSettings(strFolder, strName);
                                s.SerializeProjectSettings(ds);
                                return s.GetXml.ToString() + (s.HasAdaptItConfigurationData ? s.AdaptItConfigXml.ToString() : "");
                            },
                            () =>
                            {
                                var s = new ProjectSettings(strFolder, strName);
                                s.SerializeProjectSettings(contents.Root);
                                return s.GetXml.ToString() + (s.HasAdaptItConfigurationData ? s.AdaptItConfigXml.ToString() : "");
                            });

                    var rows = ds.stories.ToList();
                    var elems = contents.Root.Elements("stories").ToList();
                    if (rows.Count != elems.Count)
                        compare("stories count", () => rows.Count.ToString(), () => elems.Count.ToString());
                    for (int i = 0; i < Math.Min(rows.Count, elems.Count); i++)
                    {
                        var row = rows[i];
                        var elem = elems[i];
                        compare($"StoriesData[{i}] '{row.SetName}'",
                                () => { ProjectReader.UniqueStoryGuids.Clear(); return new StoriesData(row, ds, strFolder).GetXml.ToString(); },
                                () => { ProjectReader.UniqueStoryGuids.Clear(); return new StoriesData(elem, strFolder).GetXml.ToString(); });
                    }

                    if (strFirstDiff != null)
                    {
                        lstDiffering.Add(strFile);
                        TestContext.WriteLine($"DIFF {strFile}\n{strFirstDiff}");
                    }
                }
                catch (Exception ex)
                {
                    lstDiffering.Add(strFile);
                    TestContext.WriteLine($"DIFF {strFile}: harness exception {ex}");
                }
            }

            TestContext.WriteLine($"SUMMARY files={nFiles} compared={nCompared} skipped={nSkipped} differing={lstDiffering.Count} charsCompared={nChars} bothThrew={nBothThrew} markedPlain={nMarkedPlain} decodePath={nDecodePath}");
            Assert.That(nFiles, Is.GreaterThan(0), "no corpus files found");
            Assert.That(nCompared, Is.GreaterThan(0), "nothing was compared");
            Assert.That(lstDiffering, Is.Empty);
        }
    }
}
