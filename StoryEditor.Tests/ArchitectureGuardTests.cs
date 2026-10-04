using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// sub-project B's rules (spec: docs/superpowers/specs/2026-10-04-html-message-protocol-design.md): pages talk to
    /// C# only through js/bridge.js, and only IeHtmlHost touches the IE browser. Sub-project C relies on both
    /// </summary>
    [TestFixture]
    public class ArchitectureGuardTests
    {
        private static readonly Regex RegexBrowserApi = new Regex(
            @"\b(HtmlElement|HtmlDocument|InvokeScript|InvokeMember|DomDocument|DocumentText|ObjectForScripting|mshtml)\b",
            RegexOptions.Compiled);

        // starts from where this file was compiled, so it also works when the tests were built into some other OutDir
        private static string StoryEditorSourceDir([CallerFilePath] string strThisFile = "")
        {
            var strStart = String.IsNullOrEmpty(strThisFile) ? null : Path.GetDirectoryName(strThisFile);
            var dir = new DirectoryInfo((strStart != null) && Directory.Exists(strStart)
                                            ? strStart
                                            : TestContext.CurrentContext.TestDirectory);
            while ((dir != null) && !File.Exists(Path.Combine(dir.FullName, "StoryEditor 2017.sln")))
                dir = dir.Parent;
            Assert.That(dir, Is.Not.Null, "the repository root wasn't found above " + TestContext.CurrentContext.TestDirectory);
            return Path.Combine(dir.FullName, "StoryEditor");
        }

        private static IEnumerable<string> SourceFiles(params string[] astrExtensions)
        {
            return Directory.EnumerateFiles(StoryEditorSourceDir(), "*.*", SearchOption.AllDirectories)
                            .Where(f => !f.Contains(@"\obj\") && !f.Contains(@"\bin\"))
                            .Where(f => astrExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
        }

        [Test]
        public void OnlyTheBridgeScriptUsesWindowExternal()
        {
            var offenders = SourceFiles(".js", ".cs", ".htm", ".html", ".resx")
                .Where(f => !Path.GetFileName(f).Equals("bridge.js"))
                .Where(f => File.ReadAllText(f).Contains("window.external"))
                .Select(Path.GetFileName)
                .ToList();
            Assert.That(offenders, Is.Empty);
        }

        [Test]
        public void OnlyIeHtmlHostTouchesTheBrowser()
        {
            var offenders = SourceFiles(".cs")
                .Where(f => !Path.GetFileName(f).Equals("IeHtmlHost.cs"))
                .Where(f => RegexBrowserApi.IsMatch(File.ReadAllText(f)))
                .Select(f => Path.GetFileName(f) + ": " + RegexBrowserApi.Match(File.ReadAllText(f)).Value)
                .ToList();
            Assert.That(offenders, Is.Empty);
        }
    }
}
