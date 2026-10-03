# Legacy Text Repair (Sub-project A) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Keep plain text in the model for every non-note field, sanitize legacy note HTML when it is rendered, and send all text through a few encoding helpers. The IE panes must look and behave as they do today, and WebView2 (sub-project C) inherits clean data.

**Architecture:** Three small static helpers in the `StoryEditor` project:
- `HtmlText`: encode/decode at the HTML boundary.
- `LegacyTextRepair`: decodes IE-introduced entities on load, and owns the `TextEncoding="plain"` marker.
- `NoteHtmlSanitizer`: allowlist sanitizing for note HTML, using the HtmlSanitizer NuGet package.

They are wired into project load, save, clipboard paste, HTML generation and the IE input routes. A new NUnit test project covers the helpers and a real-file load.

**Tech Stack:** C# / .NET Framework 4.8 (SDK-style csproj, x86), WinForms + IE WebBrowser, typed ADO.NET `NewDataSet`, LINQ to XML, Ganss.Xss **HtmlSanitizer 9.2.1039**, **NUnit 3.14.0**, NUnit3TestAdapter 5.2.0, Microsoft.NET.Test.Sdk 18.10.1.

**Spec:** `docs/superpowers/specs/2026-10-03-legacy-text-repair-design.md` (rev 3)

## Global Constraints

- Branch: `DecoupleWebBrowser`. Commit after every task. Commit messages end with
  `Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>`.
- **Do not change `StoryProject@version`** or the existing 1.6/1.7/1.8 version logic. The marker is the attribute `TextEncoding="plain"`.
- **Do not change** `RobustFile` usage or the save sequence (temp file → reload check → backup → replace) in `StoryEditor.SaveXElement`.
- **No JS changes** in this sub-project.
- Note text (ConsultantNote/CoachNote, including ReferringText) is **never rewritten in storage**; it is sanitized only when rendered.
- Ignore the old .NET text-box view (`CtrlTextBox`, `advancedUseOldStyleStoryBtPaneMenu`, …); it is removed in sub-project R.
- Namespace for new production code: `OneStoryProjectEditor`. New files go directly in `StoryEditor\` (the project is flat).
- Match surrounding style: `String.IsNullOrEmpty`, `Cstr…` constants, `//` comments in the codebase's voice.
- Build and test commands (Git Bash; adjust the VS path if your install differs):
  ```bash
  MSBUILD="/c/Program Files/Microsoft Visual Studio/18/Insiders/MSBuild/Current/Bin/MSBuild.exe"
  VSTEST="/c/Program Files/Microsoft Visual Studio/18/Insiders/Common7/IDE/Extensions/TestPlatform/vstest.console.exe"
  # build StoryEditor + tests (the solution is needed: building the csproj alone fails on $(SolutionDir))
  "$MSBUILD" "StoryEditor 2017.sln" -t:StoryEditor_Tests -restore -p:Configuration=Debug -p:Platform=x86 -v:m -nologo
  # run tests (optionally add /TestCaseFilter:"FullyQualifiedName~ClassName")
  "$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86
  ```
  If the build prints a different output path for `StoryEditor.Tests ->`, use that path.

## Review Focus

These inputs are implied by the spec but easy to miss. Each has a test in the task named:

1. **Line breaks in read-only notes after sanitizing.** The HTML parser turns `\r\n` into `\n`, so a multi-line comment must still show its breaks (`<br />`). → Task 4, `ToReadOnlyHtml_KeepsLineBreaks`.
2. **Revision-history / differencing view with `&` or `<x>` in story text.** The text must show literally and the insert/delete markup must stay intact. → Task 5, `HtmlDiffTests`.
3. **A story box with a highlight span plus literal `<x>` text, sent back through IE `onchange`.** `<x>` must survive; only our `<span>`/`<br>` are stripped. → Task 1, `FromIeHtmlText_KeepsLiteralAngleBracketText`.
4. **Pasting a story/column copied from the released exe (no marker) versus from this exe (marked).** The first is decoded, the second is not decoded twice. → Task 2, `DecodeUnlessMarked_*`.
5. **Search highlight in a read-only note where the found range overlaps markup.** No sentinel characters may leak into the page; it falls back to unhighlighted. → Task 4, `ToReadOnlyHtmlWithHighlight_*`.

---

### Task 1: Test project + `HtmlText` helpers

**Files:**
- Create: `StoryEditor.Tests/StoryEditor.Tests.csproj`
- Create: `StoryEditor.Tests/HtmlTextTests.cs`
- Create: `StoryEditor/HtmlText.cs`
- Modify: `StoryEditor 2017.sln` (add the project)

**Interfaces:**
- Produces (all `public static` on `OneStoryProjectEditor.HtmlText`):
  - `string Encode(string str)`: encodes only `& < > "`; null/empty pass through.
  - `string Encode(char ch)`: same, for one char.
  - `string LineBreaksToBr(string html)`: `\r\n`, `\n` or `\r` → `<br />`.
  - `string ForParagraph(string str)` = `LineBreaksToBr(Encode(str))`.
  - `string FromIeHtmlText(string str)`: `<br>` → `\r\n`, remove `<span…>`/`</span>`, then `WebUtility.HtmlDecode`.

- [ ] **Step 1: Create the test project**

`StoryEditor.Tests/StoryEditor.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <Platforms>x86</Platforms>
    <PlatformTarget>x86</PlatformTarget>
    <LangVersion>latest</LangVersion>
    <RootNamespace>OneStoryProjectEditor.Tests</RootNamespace>
    <AssemblyName>StoryEditor.Tests</AssemblyName>
    <IsPackable>false</IsPackable>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.10.1" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\StoryEditor\StoryEditor.csproj" />
  </ItemGroup>
  <ItemGroup>
    <Reference Include="System.Data" />
    <Reference Include="System.Xml" />
    <Reference Include="System.Xml.Linq" />
  </ItemGroup>
  <ItemGroup>
    <None Update="TestData\**\*" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>
</Project>
```

Add it to the solution:
```bash
dotnet sln "StoryEditor 2017.sln" add StoryEditor.Tests/StoryEditor.Tests.csproj
```
Open `StoryEditor 2017.sln` and check the new project's lines in `GlobalSection(ProjectConfigurationPlatforms)`. They must map to `x86`, not `Any CPU`. If they don't, edit them to read (with the project's GUID):
```
		{GUID}.Debug|x86.ActiveCfg = Debug|x86
		{GUID}.Debug|x86.Build.0 = Debug|x86
		{GUID}.Release|x86.ActiveCfg = Release|x86
		{GUID}.Release|x86.Build.0 = Release|x86
```

- [ ] **Step 2: Write the failing tests**

`StoryEditor.Tests/HtmlTextTests.cs`:
```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlTextTests
    {
        [Test]
        public void Encode_EncodesOnlyHtmlSpecialCharacters()
        {
            Assert.That(HtmlText.Encode("a & b <donkey bray> \"q\" é ਪੰ"),
                        Is.EqualTo("a &amp; b &lt;donkey bray&gt; &quot;q&quot; é ਪੰ"));
        }

        [Test]
        public void Encode_NullAndEmptyPassThrough()
        {
            Assert.That(HtmlText.Encode((string)null), Is.Null);
            Assert.That(HtmlText.Encode(""), Is.EqualTo(""));
        }

        [Test]
        public void Encode_Char()
        {
            Assert.That(HtmlText.Encode('<'), Is.EqualTo("&lt;"));
            Assert.That(HtmlText.Encode('x'), Is.EqualTo("x"));
        }

        [Test]
        public void ForParagraph_EncodesAndConvertsLineBreaks()
        {
            Assert.That(HtmlText.ForParagraph("a & b\r\nc\nd"), Is.EqualTo("a &amp; b<br />c<br />d"));
        }

        [Test]
        public void LineBreaksToBr_LeavesMarkupAlone()
        {
            Assert.That(HtmlText.LineBreaksToBr("<b>x</b>\r\ny"), Is.EqualTo("<b>x</b><br />y"));
        }

        [Test]
        public void FromIeHtmlText_StripsHighlightSpansAndDecodes()
        {
            const string ieHtmlText = "[B&amp;B] <SPAN class=\"LangVernacular StoryLine highlight\">idop</SPAN><BR>baris &lt;2&gt;";
            Assert.That(HtmlText.FromIeHtmlText(ieHtmlText), Is.EqualTo("[B&B] idop\r\nbaris <2>"));
        }

        [Test]
        public void FromIeHtmlText_UnquotedSpanAttributes()
        {
            Assert.That(HtmlText.FromIeHtmlText("a <SPAN class=highlight>b</SPAN>"), Is.EqualTo("a b"));
        }

        [Test]
        public void FromIeHtmlText_KeepsLiteralAngleBracketText()
        {
            // raw "<x>" can appear when TriggerMyBlur rebuilt the textarea from its (decoded) value;
            //  only our own <span>/<br> markup may be removed
            Assert.That(HtmlText.FromIeHtmlText("a <x> &lt;y&gt;"), Is.EqualTo("a <x> <y>"));
        }

        [Test]
        public void FromIeHtmlText_Nbsp()
        {
            Assert.That(HtmlText.FromIeHtmlText("a&nbsp;b"), Is.EqualTo("a\u00A0b"));
        }
    }
}
```

- [ ] **Step 3: Run the build to verify it fails**

Run the build command from Global Constraints.
Expected: compile error `The name 'HtmlText' does not exist in the current context`.

- [ ] **Step 4: Implement `HtmlText`**

`StoryEditor/HtmlText.cs`:
```csharp
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// The model holds plain text; this is where it gets turned into HTML (and, for what comes
    /// back from IE's htmlText/innerHTML, turned back into plain text).
    /// </summary>
    public static class HtmlText
    {
        private static readonly Regex RegexLineBreak = new Regex(@"\r\n|\n|\r", RegexOptions.Compiled);
        private static readonly Regex RegexIeBr = new Regex(@"<br\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexIeSpan = new Regex(@"</?span\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // only the characters that matter to HTML (non-ASCII text is left as is)
        public static string Encode(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder(str.Length + 16);
            foreach (var ch in str)
                sb.Append(Encode(ch));
            return sb.ToString();
        }

        public static string Encode(char ch)
        {
            switch (ch)
            {
                case '&': return "&amp;";
                case '<': return "&lt;";
                case '>': return "&gt;";
                case '"': return "&quot;";
                default: return ch.ToString();
            }
        }

        // for text that's already HTML (e.g. the output of Diff.HtmlDiff)
        public static string LineBreaksToBr(string html)
        {
            return String.IsNullOrEmpty(html)
                       ? html
                       : RegexLineBreak.Replace(html, "<br />");
        }

        public static string ForParagraph(string str)
        {
            return LineBreaksToBr(Encode(str));
        }

        // IE's htmlText (textarea 'onchange') and innerHTML come back HTML-encoded and may contain
        //  the <span> (selection highlight) and <br> that StoryBt.js put there
        public static string FromIeHtmlText(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = RegexIeBr.Replace(str, "\r\n");
            str = RegexIeSpan.Replace(str, String.Empty);
            return WebUtility.HtmlDecode(str);
        }
    }
}
```

- [ ] **Step 5: Build and run the tests**

Run the build, then `"$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86 /TestCaseFilter:"FullyQualifiedName~HtmlTextTests"`.
Expected: 9 passed.

- [ ] **Step 6: Commit**

```bash
git add StoryEditor.Tests/StoryEditor.Tests.csproj StoryEditor.Tests/HtmlTextTests.cs StoryEditor/HtmlText.cs "StoryEditor 2017.sln"
git commit -m "Add StoryEditor.Tests (NUnit) and HtmlText encode/decode helpers

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 2: `LegacyTextRepair` (entity decode + marker helpers)

**Files:**
- Create: `StoryEditor/LegacyTextRepair.cs`
- Create: `StoryEditor.Tests/LegacyTextRepairTests.cs`
- Create: `StoryEditor.Tests/TestData/minimal-1.8.onestory`

**Interfaces:**
- Consumes: none.
- Produces (all `public static` on `OneStoryProjectEditor.LegacyTextRepair`):
  - `const string CstrAttributeTextEncoding = "TextEncoding"`, `const string CstrTextEncodingPlain = "plain"`
  - `string DecodeIeEntities(string str)`: decodes only `&amp; &lt; &gt; &quot; &nbsp; &#N; &#xH;`.
  - `bool ContainsIeEntity(string str)`
  - `int DecodePlainTextFields(DataSet ds)`: returns the number of values changed.
  - `int DecodePlainTextElements(XElement root)`: returns the number of values changed.
  - `bool IsMarkedPlain(XElement root)`, `void MarkAsPlain(XElement root)`
  - `bool DecodeUnlessMarked(XElement root)`: true if it decoded.
  - `bool IsFileMarkedPlain(string strFilePath)`: reads only the root element's attributes.

- [ ] **Step 1: Add the test fixture**

`StoryEditor.Tests/TestData/minimal-1.8.onestory` is synthetic; do not use real team data. It was verified to load with `ProjectReader.ReadProjectFile`.
```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<StoryProject version="1.8" ProjectName="minimal">
  <Members HasOutsideEnglishBTer="false" HasFirstPassMentor="false" HasIndependentConsultant="false">
    <Member name="Test Crafter" memberType="Crafter" memberKey="mem-00000000-0000-0000-0000-000000000001" />
  </Members>
  <Languages>
    <LanguageInfo lang="Vernacular" name="Testish" code="tst" FontName="Arial" FontSize="12" FontColor="Maroon" SentenceFinalPunct=".!?:" />
  </Languages>
  <LnCNotes />
  <stories SetName="Stories">
    <story name="Minimal story" stage="ProjFacTypeVernacular" guid="00000000-0000-0000-0000-000000000010" stageDateTimeStamp="2026-10-03T00:00:00Z">
      <CraftingInfo NonBiblicalStory="false">
        <StoryCrafter memberID="mem-00000000-0000-0000-0000-000000000001" />
      </CraftingInfo>
      <Verses>
        <Verse guid="00000000-0000-0000-0000-000000000020">
          <StoryLine lang="Vernacular">dengan [B&amp;amp;B] kata&amp;nbsp;Tuhan &lt;donkey bray&gt; B&amp;B;</StoryLine>
          <TestQuestions>
            <TestQuestion visible="true" guid="00000000-0000-0000-0000-000000000030">
              <TestQuestionLine lang="Vernacular">snake &amp;amp; lady?</TestQuestionLine>
            </TestQuestion>
          </TestQuestions>
          <ConsultantNotes>
            <ConsultantConversation guid="00000000-0000-0000-0000-000000000040">
              <ConsultantNote Direction="ConsultantToProjFac" guid="00000000-0000-0000-0000-000000000041" memberID="mem-00000000-0000-0000-0000-000000000001" timeStamp="2026-10-03T00:00:00Z">ok &lt;B&gt;only&lt;/B&gt; [B&amp;amp;B]</ConsultantNote>
            </ConsultantConversation>
          </ConsultantNotes>
        </Verse>
      </Verses>
    </story>
  </stories>
</StoryProject>
```
After XML parsing, the StoryLine value is `dengan [B&amp;B] kata&nbsp;Tuhan <donkey bray> B&B;`. That is exactly what IE's `htmlText` left in real files.

- [ ] **Step 2: Write the failing tests**

`StoryEditor.Tests/LegacyTextRepairTests.cs`:
```csharp
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class LegacyTextRepairTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minimal-1.8.onestory");

        [TestCase("[B&amp;B]", "[B&B]")]
        [TestCase("snake &amp; lady", "snake & lady")]
        [TestCase("kata&nbsp;Tuhan", "kata\u00A0Tuhan")]
        [TestCase("&lt;OseStoryToCopy&gt;", "<OseStoryToCopy>")]
        [TestCase("&quot;q&quot; &#39;s&#39; &#x41;", "\"q\" 's' A")]
        [TestCase("<donkey bray>", "<donkey bray>")]
        [TestCase("B&B;", "B&B;")]                 // not an entity IE produces: leave it
        [TestCase("a & b", "a & b")]
        public void DecodeIeEntities(string input, string expected)
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities(input), Is.EqualTo(expected));
        }

        [Test]
        public void DecodeIeEntities_OneLevelOnly()
        {
            Assert.That(LegacyTextRepair.DecodeIeEntities("&amp;amp;"), Is.EqualTo("&amp;"));
        }

        [Test]
        public void DecodePlainTextFields_DecodesStoryFieldsButNotNotes()
        {
            ProjectReader projFile;
            ProjectReader.ReadProjectFile(FixturePath, out projFile);

            var nChanged = LegacyTextRepair.DecodePlainTextFields(projFile);

            Assert.That(nChanged, Is.EqualTo(2));
            Assert.That(projFile.Tables["StoryLine"].Rows[0]["StoryLine_text"],
                        Is.EqualTo("dengan [B&B] kata\u00A0Tuhan <donkey bray> B&B;"));
            Assert.That(projFile.Tables["TestQuestionLine"].Rows[0]["TestQuestionLine_text"],
                        Is.EqualTo("snake & lady?"));
            Assert.That(projFile.Tables["ConsultantNote"].Rows[0]["ConsultantNote_text"],
                        Is.EqualTo("ok <B>only</B> [B&amp;B]"));   // notes are HTML: untouched
        }

        [Test]
        public void DecodePlainTextElements_DecodesClipboardXml()
        {
            var root = XElement.Parse(
                "<OseStoryToCopy><story><Verses><Verse>" +
                "<StoryLine lang=\"Vernacular\">[B&amp;amp;B]</StoryLine>" +
                "<ConsultantNotes><ConsultantConversation><ConsultantNote>[B&amp;amp;B]</ConsultantNote></ConsultantConversation></ConsultantNotes>" +
                "</Verse></Verses></story></OseStoryToCopy>");

            var nChanged = LegacyTextRepair.DecodePlainTextElements(root);

            Assert.That(nChanged, Is.EqualTo(1));
            Assert.That(root.Descendants("StoryLine").Single().Value, Is.EqualTo("[B&B]"));
            Assert.That(root.Descendants("StoryLine").Single().Attribute("lang").Value, Is.EqualTo("Vernacular"));
            Assert.That(root.Descendants("ConsultantNote").Single().Value, Is.EqualTo("[B&amp;B]"));
        }

        [Test]
        public void DecodeUnlessMarked_UnmarkedIsDecoded()
        {
            var root = XElement.Parse("<OseColumnToCopy><StoryLine lang=\"Vernacular\">a &amp;amp; b</StoryLine></OseColumnToCopy>");
            Assert.That(LegacyTextRepair.DecodeUnlessMarked(root), Is.True);
            Assert.That(root.Element("StoryLine").Value, Is.EqualTo("a & b"));
        }

        [Test]
        public void DecodeUnlessMarked_MarkedIsLeftAlone()
        {
            var root = XElement.Parse("<OseColumnToCopy TextEncoding=\"plain\"><StoryLine lang=\"Vernacular\">a &amp;amp; b</StoryLine></OseColumnToCopy>");
            Assert.That(LegacyTextRepair.DecodeUnlessMarked(root), Is.False);
            Assert.That(root.Element("StoryLine").Value, Is.EqualTo("a &amp; b"));
        }

        [Test]
        public void MarkAsPlain_SetsAttribute()
        {
            var root = new XElement("StoryProject", new XAttribute("version", "1.8"));
            Assert.That(LegacyTextRepair.IsMarkedPlain(root), Is.False);
            LegacyTextRepair.MarkAsPlain(root);
            Assert.That(LegacyTextRepair.IsMarkedPlain(root), Is.True);
            Assert.That(root.Attribute("version").Value, Is.EqualTo("1.8"));
        }

        [Test]
        public void IsFileMarkedPlain()
        {
            var strMarked = Path.Combine(Path.GetTempPath(), "ose-marked-" + Guid.NewGuid() + ".onestory");
            try
            {
                var doc = XDocument.Load(FixturePath);
                LegacyTextRepair.MarkAsPlain(doc.Root);
                doc.Save(strMarked);

                Assert.That(LegacyTextRepair.IsFileMarkedPlain(FixturePath), Is.False);
                Assert.That(LegacyTextRepair.IsFileMarkedPlain(strMarked), Is.True);
            }
            finally
            {
                File.Delete(strMarked);
            }
        }

        [Test]
        public void ContainsIeEntity()
        {
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&amp;B]"), Is.True);
            Assert.That(LegacyTextRepair.ContainsIeEntity("[B&B] B&B;"), Is.False);
        }
    }
}
```
- [ ] **Step 3: Run the build to verify it fails**

Expected: compile error `The name 'LegacyTextRepair' does not exist in the current context`.

- [ ] **Step 4: Implement `LegacyTextRepair`**

`StoryEditor/LegacyTextRepair.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Older versions round-tripped story text through IE's htmlText, which left HTML entities in
    /// the data (e.g. "[B&amp;B]" where the user typed "[B&B]"). Files saved by this version carry
    /// TextEncoding="plain" on the root element and are already clean. Versions that don't know
    /// about the attribute drop it when they save, so their files get cleaned again on the next load.
    /// Note text (ConsultantNote/CoachNote) is HTML and is never touched here.
    /// </summary>
    public static class LegacyTextRepair
    {
        public const string CstrAttributeTextEncoding = "TextEncoding";
        public const string CstrTextEncodingPlain = "plain";

        // just the entities IE's htmlText produced (so the user's own "B&B;" is left alone)
        private static readonly Regex RegexIeEntity =
            new Regex(@"&(?:amp|lt|gt|quot|nbsp|#[0-9]{1,7}|#[xX][0-9A-Fa-f]{1,6});", RegexOptions.Compiled);

        // typed NewDataSet table -> its plain-text columns
        private static readonly Dictionary<string, string[]> PlainTextColumns = new Dictionary<string, string[]>
        {
            { "StoryLine", new[] { "StoryLine_text" } },
            { "Retelling", new[] { "Retelling_text" } },
            { "Answer", new[] { "Answer_text" } },
            { "TestQuestionLine", new[] { "TestQuestionLine_text" } },
            { "ExegeticalHelp", new[] { "ExegeticalHelp_Column" } },
            { "LnCNote", new[] { "LnCNote_text", "VernacularRendering", "NationalBTRendering", "InternationalBTRendering" } },
            { "CraftingInfo", new[] { "StoryPurpose", "ResourcesUsed", "MiscellaneousStoryInfo" } },
            { "TestRetelling", new[] { "TestRetelling_text" } },
            { "TestTqAnswer", new[] { "TestTqAnswer_text" } },
        };

        // the same fields as XML elements (for the copy-story/copy-column clipboard XML)
        private static readonly HashSet<string> PlainTextElementNames = new HashSet<string>
        {
            "StoryLine", "Retelling", "Answer", "TestQuestionLine", "ExegeticalHelp", "LnCNote",
            "StoryPurpose", "ResourcesUsed", "MiscellaneousStoryInfo", "TestRetelling", "TestTqAnswer"
        };

        private static readonly string[] PlainTextAttributeNamesOfLnCNote =
        {
            "VernacularRendering", "NationalBTRendering", "InternationalBTRendering"
        };

        public static string DecodeIeEntities(string str)
        {
            if (String.IsNullOrEmpty(str) || (str.IndexOf('&') < 0))
                return str;
            return RegexIeEntity.Replace(str, m => WebUtility.HtmlDecode(m.Value));
        }

        public static bool ContainsIeEntity(string str)
        {
            return !String.IsNullOrEmpty(str) && RegexIeEntity.IsMatch(str);
        }

        public static int DecodePlainTextFields(DataSet ds)
        {
            int nChanged = 0;
            foreach (var kvp in PlainTextColumns)
            {
                var table = ds.Tables[kvp.Key];
                if (table == null)
                    continue;

                foreach (var strColumnName in kvp.Value)
                {
                    var column = table.Columns[strColumnName];
                    if ((column == null) || (column.DataType != typeof(string)))
                        continue;

                    foreach (DataRow row in table.Rows)
                    {
                        if (row.IsNull(column))
                            continue;

                        var str = (string)row[column];
                        var strDecoded = DecodeIeEntities(str);
                        if (strDecoded == str)
                            continue;

                        row[column] = strDecoded;
                        nChanged++;
                    }
                }
            }
            return nChanged;
        }

        public static int DecodePlainTextElements(XElement root)
        {
            int nChanged = 0;
            foreach (var elem in root.DescendantsAndSelf())
            {
                var strName = elem.Name.LocalName;
                if (!PlainTextElementNames.Contains(strName))
                    continue;

                if (!elem.HasElements)
                {
                    var str = elem.Value;
                    var strDecoded = DecodeIeEntities(str);
                    if (strDecoded != str)
                    {
                        elem.Value = strDecoded;
                        nChanged++;
                    }
                }

                if (strName != "LnCNote")
                    continue;

                foreach (var strAttributeName in PlainTextAttributeNamesOfLnCNote)
                {
                    var attr = elem.Attribute(strAttributeName);
                    if (attr == null)
                        continue;
                    var strDecoded = DecodeIeEntities(attr.Value);
                    if (strDecoded == attr.Value)
                        continue;
                    attr.Value = strDecoded;
                    nChanged++;
                }
            }
            return nChanged;
        }

        public static bool IsMarkedPlain(XElement root)
        {
            return (string)root.Attribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain;
        }

        public static void MarkAsPlain(XElement root)
        {
            root.SetAttributeValue(CstrAttributeTextEncoding, CstrTextEncodingPlain);
        }

        public static bool DecodeUnlessMarked(XElement root)
        {
            if (IsMarkedPlain(root))
                return false;
            DecodePlainTextElements(root);
            return true;
        }

        // the typed NewDataSet doesn't know this attribute (neither does the one in older versions,
        //  which is why they can still read our files), so read it straight from the root element
        public static bool IsFileMarkedPlain(string strFilePath)
        {
            using (var reader = XmlReader.Create(strFilePath))
            {
                reader.MoveToContent();
                return reader.GetAttribute(CstrAttributeTextEncoding) == CstrTextEncodingPlain;
            }
        }
    }
}
```

- [ ] **Step 5: Build and run the tests**

Filter `FullyQualifiedName~LegacyTextRepairTests`. Expected: all 16 pass (8 `DecodeIeEntities` cases + 8 others).

- [ ] **Step 6: Commit**

```bash
git add StoryEditor/LegacyTextRepair.cs StoryEditor.Tests/LegacyTextRepairTests.cs StoryEditor.Tests/TestData/minimal-1.8.onestory
git commit -m "Add LegacyTextRepair: decode IE-introduced entities in plain-text fields

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 3: Wire the marker into load, save and clipboard paste

**Files:**
- Modify: `StoryEditor/StoryData.cs`: `ProjectReader.ReadProjectFile` (~:2733), `StoryProjectData(NewDataSet, ProjectSettings)` constructor (~:1987-2051), `GetXml` (~:2591), `GetXmlToCopyStory` (~:2643), `GetXmlToCopyColumn` (~:2653)
- Modify: `StoryEditor/PanoramaView.cs` `PasteStoryCopy` (~:1320)
- Modify: `StoryEditor/StoryEditor.cs`: column paste (~:3986), story paste (~:7957)
- Test: `StoryEditor.Tests/ProjectReaderMarkerTests.cs`

**Interfaces:**
- Consumes: `LegacyTextRepair.IsFileMarkedPlain`, `DecodePlainTextFields`, `MarkAsPlain`, `DecodeUnlessMarked` (Task 2).
- Produces: `public bool ProjectReader.IsPlainTextEncoded { get; private set; }`.

- [ ] **Step 1: Write the failing test**

`StoryEditor.Tests/ProjectReaderMarkerTests.cs`:
```csharp
using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class ProjectReaderMarkerTests
    {
        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minimal-1.8.onestory");

        [Test]
        public void UnmarkedFile_IsNotPlainTextEncoded()
        {
            ProjectReader projFile;
            ProjectReader.ReadProjectFile(FixturePath, out projFile);
            Assert.That(projFile.IsPlainTextEncoded, Is.False);
        }

        [Test]
        public void MarkedFile_LoadsAndIsPlainTextEncoded()
        {
            // this typed DataSet is the same one the released exe uses, so loading without an
            //  exception also shows that older versions will ignore the new attribute
            var strMarked = Path.Combine(Path.GetTempPath(), "ose-marked-" + Guid.NewGuid() + ".onestory");
            try
            {
                var doc = XDocument.Load(FixturePath);
                LegacyTextRepair.MarkAsPlain(doc.Root);
                doc.Save(strMarked);

                ProjectReader projFile;
                ProjectReader.ReadProjectFile(strMarked, out projFile);

                Assert.That(projFile.IsPlainTextEncoded, Is.True);
                Assert.That(projFile.StoryProject[0].version, Is.EqualTo("1.8"));
            }
            finally
            {
                File.Delete(strMarked);
            }
        }
    }
}
```

- [ ] **Step 2: Run the build to verify it fails**

Expected: compile error `'ProjectReader' does not contain a definition for 'IsPlainTextEncoded'`.

- [ ] **Step 3: Implement**

In `StoryEditor/StoryData.cs`, class `ProjectReader`, add the property and set it after `ReadXml`:
```csharp
    public class ProjectReader : NewDataSet
    {
        public static List<string> UniqueStoryGuids = new List<string>();

        // true if the file was saved by a version that keeps plain text (see LegacyTextRepair)
        public bool IsPlainTextEncoded { get; private set; }

        public static DateTime ReadProjectFile(string strProjectFilePath, out ProjectReader projectReader)
        {
            try
            {
                UniqueStoryGuids.Clear();
                projectReader = new ProjectReader();
                projectReader.ReadXml(strProjectFilePath);
                projectReader.IsPlainTextEncoded = LegacyTextRepair.IsFileMarkedPlain(strProjectFilePath);
                return File.GetLastWriteTime(strProjectFilePath);
            }
```

In the `StoryProjectData(NewDataSet projFile, ProjectSettings projSettings)` constructor, insert this right after the closing brace of the `if (projFile.StoryProject.Count == 0) { … } else { … }` block, before `PanoramaFrontMatter = projFile.StoryProject[0].PanoramaFrontMatter;`:
```csharp
            // files not saved by a version that keeps plain text may have HTML entities in the
            //  story text that IE's htmlText put there (e.g. "[B&amp;B]")
            if (!((projFile as ProjectReader)?.IsPlainTextEncoded ?? false))
                LegacyTextRepair.DecodePlainTextFields(projFile);
```

In `StoryProjectData.GetXml`, right after `elemStoryProject` is created:
```csharp
                var elemStoryProject =
                    new XElement(CstrElementStoryProjectRoot,
                                 new XAttribute(CstrAttributeVersion, XmlDataVersion),
                                 new XAttribute(CstrAttributeProjectName, ProjSettings.ProjectName));
                LegacyTextRepair.MarkAsPlain(elemStoryProject);
```

`GetXmlToCopyStory`:
```csharp
        public XElement GetXmlToCopyStory(StoryData theStoryToCopy)
        {
            var elem = new XElement(CstrElementOseStoryToCopy,
                                    TeamMembers.GetXml,
                                    ProjSettings.GetXml,
                                    theStoryToCopy.GetXml);
            LegacyTextRepair.MarkAsPlain(elem);
            return elem;
        }
```

`GetXmlToCopyColumn`, right after `var elem = new XElement(CstrElementOseColumnToCopy);`:
```csharp
            LegacyTextRepair.MarkAsPlain(elem);
```

`StoryEditor/PanoramaView.cs` `PasteStoryCopy`, after `var theStoryToCopyPlusMembersXElement = XElement.Parse(strData);`:
```csharp
            LegacyTextRepair.DecodeUnlessMarked(theStoryToCopyPlusMembersXElement);
```

`StoryEditor/StoryEditor.cs`:
- story paste (~:7957), after `var theStoryToCopyPlusMembersXElement = XElement.Parse(strData);`:
  ```csharp
              LegacyTextRepair.DecodeUnlessMarked(theStoryToCopyPlusMembersXElement);
  ```
- column paste (~:3986), after `var theColumnToCopy = XElement.Parse(strData);`:
  ```csharp
              LegacyTextRepair.DecodeUnlessMarked(theColumnToCopy);
  ```

- [ ] **Step 4: Build and run all tests**

Expected: all tests pass, including the 2 new ones.

- [ ] **Step 5: Commit**

```bash
git add StoryEditor/StoryData.cs StoryEditor/PanoramaView.cs StoryEditor/StoryEditor.cs StoryEditor.Tests/ProjectReaderMarkerTests.cs
git commit -m "Decode legacy entities on load/paste unless TextEncoding=\"plain\"; write the marker on save/copy

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 4: `NoteHtmlSanitizer`

**Files:**
- Modify: `StoryEditor/StoryEditor.csproj` (add `<PackageReference Include="HtmlSanitizer" Version="9.2.1039" />` to the `PackageReference` ItemGroup, keeping it alphabetical)
- Create: `StoryEditor/NoteHtmlSanitizer.cs`
- Test: `StoryEditor.Tests/NoteHtmlSanitizerTests.cs`

**Interfaces:**
- Consumes: `HtmlText.Encode`, `HtmlText.LineBreaksToBr` (Task 1).
- Produces (`public static` on `OneStoryProjectEditor.NoteHtmlSanitizer`):
  - `string Sanitize(string html)`: never throws; falls back to `HtmlText.Encode(html)`.
  - `bool TrySanitize(string html, out string result)`
  - `string ToReadOnlyHtml(string raw)` = `LineBreaksToBr(Sanitize(raw))`
  - `string ToReadOnlyHtmlWithHighlight(string raw, int nIndex, int nLength, string strBegin, string strEnd)`

- [ ] **Step 1: Write the failing tests**

`StoryEditor.Tests/NoteHtmlSanitizerTests.cs` uses real examples from the data scan:
```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class NoteHtmlSanitizerTests
    {
        [Test]
        public void KeepsCreateNoteSpans()
        {
            const string note = "Cerita : <SPAN class=\"LangVernacular StoryLine\">idop</SPAN> vs: Cerita : <SPAN class=\"LangNationalBt StoryLine\">hidup</SPAN>";
            Assert.That(NoteHtmlSanitizer.Sanitize(note),
                        Is.EqualTo("Cerita : <span class=\"LangVernacular StoryLine\">idop</span> vs: Cerita : <span class=\"LangNationalBt StoryLine\">hidup</span>"));
        }

        [Test]
        public void KeepsSimpleFormatting()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<p><i>Ttg: Catatan Kons:</i></p>save <B>only</B> <EM>so</EM>"),
                        Is.EqualTo("<p><i>Ttg: Catatan Kons:</i></p>save <b>only</b> <em>so</em>"));
        }

        [Test]
        public void RemovesScriptWithItsContent()
        {
            const string note = "Ln: 11 Add Note\n<SCRIPT type=text/javascript>\n  var textareas = document.getElementsByTagName(\"textarea\");\n  for (var i = 0; i < textareas.length; i++) { textareas[i].onkeyup = function () { return window.external.TextareaOnKeyUp(this.id, this.value); }; }\n</SCRIPT>\nafter";
            var result = NoteHtmlSanitizer.Sanitize(note);
            Assert.That(result, Does.Not.Contain("script").IgnoreCase);
            Assert.That(result, Does.Not.Contain("window.external"));
            Assert.That(result, Does.Contain("after"));
        }

        [Test]
        public void UnwrapsDivAndDropsEventHandlersAndIds()
        {
            const string note = "<DIV id=tp_1_0_0 class=TextAreaStyle>\n<P ondblclick=OnDoubleClick(this) id=tp_1_0_1 class=LangInternationalBT>ok</P></DIV>";
            var result = NoteHtmlSanitizer.Sanitize(note);
            Assert.That(result, Does.Not.Contain("div").IgnoreCase);
            Assert.That(result, Does.Not.Contain("ondblclick").IgnoreCase);
            Assert.That(result, Does.Not.Contain("id=").IgnoreCase);
            Assert.That(result, Does.Contain("<p class=\"LangInternationalBT\">ok</p>"));
        }

        [Test]
        public void DropsUnknownClasses()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<span class=\"LangVernacular highlight readonly\">x</span>"),
                        Is.EqualTo("<span class=\"LangVernacular\">x</span>"));
        }

        [Test]
        public void UnwrapsStoredInternalLinksToTheirText()
        {
            const string note = "masukkan <A onclick=\"return OnBibRefJump(this);\" \nhref=\"bibleViewer.setReference\" name=\"Luk 3:23\">Luk 3:23</A> sebagai DA. ke <A \nonclick=\"return OnVerseLineJump(this);\" class=LocalizationStyle \nhref=\"conNote.jumpToLine\" name=4>baris 4</A>:";
            Assert.That(NoteHtmlSanitizer.Sanitize(note), Is.EqualTo("masukkan Luk 3:23 sebagai DA. ke baris 4:"));
        }

        [Test]
        public void HttpLinksGetOnUrlJump()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<a href=\"https://example.org/a?b=1&c=2\" onclick=\"evil()\">here</a>"),
                        Is.EqualTo("<a href=\"https://example.org/a?b=1&amp;c=2\" onClick=\"return OnUrlJump(this);\">here</a>"));
        }

        [TestCase("JH: Re: <RTL>  Your retellings", "JH: Re: &lt;RTL&gt;  Your retellings")]
        [TestCase("parmatma said <malti  see cont note>that", "parmatma said &lt;malti  see cont note&gt;that")]
        [TestCase("use a <space> rather than a <dot>.", "use a &lt;space&gt; rather than a &lt;dot&gt;.")]
        public void ShowsUserPseudoTagsAsText(string note, string expected)
        {
            Assert.That(NoteHtmlSanitizer.Sanitize(note), Is.EqualTo(expected));
        }

        [Test]
        public void KeepsBoldItalicMarkersAndEntities()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("$bold$ *it* [B&amp;B] a&nbsp;b"),
                        Is.EqualTo("$bold$ *it* [B&amp;B] a&nbsp;b"));
        }

        [Test]
        public void ToReadOnlyHtml_KeepsLineBreaks()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml("line 1\r\nline <B>2</B>\r\n\r\nline 3"),
                        Is.EqualTo("line 1<br />line <b>2</b><br /><br />line 3"));
        }

        [Test]
        public void ToReadOnlyHtml_NullAndEmpty()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml(null), Is.Null);
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml(""), Is.EqualTo(""));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_HighlightsPlainRange()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("find the word here", 9, 4, "<mark>", "</mark>"),
                        Is.EqualTo("find the <mark>word</mark> here"));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_RangeInsideMarkupFallsBackWithoutSentinels()
        {
            // the found range is inside the tag "<B>" itself (e.g. a search for "B>"), which the
            //  sanitizer rewrites; no private-use sentinel characters may leak into the page
            var result = NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("x <B>y</B>", 3, 2, "<mark>", "</mark>");
            Assert.That(result, Does.Not.Contain("\uE000").And.Not.Contain("\uE001"));
            Assert.That(result, Does.Contain("<b>y</b>"));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_OutOfRangeIsIgnored()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("abc", 2, 5, "<mark>", "</mark>"),
                        Is.EqualTo("abc"));
        }
    }
}
```

- [ ] **Step 2: Run the build to verify it fails**

Expected: compile error `The name 'NoteHtmlSanitizer' does not exist in the current context`.

- [ ] **Step 3: Add the package and implement**

Add to `StoryEditor/StoryEditor.csproj` (in the ItemGroup with the other `PackageReference`s, alphabetically after `FluentFTP`):
```xml
    <PackageReference Include="HtmlSanitizer" Version="9.2.1039" />
```

`StoryEditor/NoteHtmlSanitizer.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// Note text (ConsultantNote/CoachNote, incl. ReferringText) is legacy HTML: "create note" spans,
    /// typed <B>/<EM>, and sometimes whole chunks of the pane's own HTML (even a <SCRIPT>) that got
    /// captured over the years. It's sanitized only when it's rendered; the stored text is never changed.
    /// </summary>
    public static class NoteHtmlSanitizer
    {
        private static readonly string[] AllowedTagNames = { "span", "p", "br", "i", "b", "em", "strong", "u", "a" };

        // the language/field classes the app generates (seen in the stored "create note" spans)
        private static readonly string[] AllowedClassNames =
        {
            "LangVernacular", "LangNationalBt", "LangNationalBT", "LangInternationalBt", "LangInternationalBT",
            "LangFreeTranslation", "StoryLine", "Anchor", "ExegeticalNote", "Retelling", "TestQuestion",
            "TestQuestionAnswer", "LocalizationStyle"
        };

        // tags we recognize as HTML (allowed ones, or ones the sanitizer removes). Anything else that
        //  looks like a tag (e.g. "<RTL>", "<malti see note>") is something a user typed: show it as text
        private static readonly HashSet<string> KnownHtmlTagNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "span", "p", "br", "i", "b", "em", "strong", "u", "a",
            "div", "font", "script", "style", "textarea", "input", "button", "select", "option", "label", "form",
            "table", "thead", "tbody", "tfoot", "tr", "td", "th", "caption", "col", "colgroup",
            "img", "iframe", "object", "embed", "link", "meta", "html", "head", "body", "title", "base",
            "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li", "dl", "dt", "dd", "pre", "code", "blockquote",
            "sup", "sub", "small", "big", "center", "hr", "nobr", "strike", "del", "ins", "tt", "abbr", "cite", "q"
        };

        private static readonly HashSet<string> DropWithContentNodeNames = new HashSet<string> { "SCRIPT", "STYLE" };

        // links SetHyperlinks made (and which got captured into the note); it makes them again from the text
        private static readonly Regex RegexInternalLink = new Regex(
            @"<a\b[^>]*?\bhref\s*=\s*[""']?(?:bibleViewer\.setReference|conNote\.jumpToLine)[""']?[^>]*>(.*?)</a\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RegexTagLike = new Regex(@"<(/?)([A-Za-z][A-Za-z0-9:_-]*)([^<>]*)>", RegexOptions.Compiled);
        private static readonly Regex RegexHttpLink = new Regex(@"<a href=""(https?:[^""]*)"">", RegexOptions.Compiled);

        private const char HighlightBeginSentinel = '\uE000';
        private const char HighlightEndSentinel = '\uE001';

        private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();
        private static readonly object SanitizerLock = new object();

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer { KeepChildNodes = true };

            sanitizer.AllowedTags.Clear();
            foreach (var strTag in AllowedTagNames)
                sanitizer.AllowedTags.Add(strTag);

            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("href");

            sanitizer.AllowedClasses.Clear();
            foreach (var strClass in AllowedClassNames)
                sanitizer.AllowedClasses.Add(strClass);

            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");

            // KeepChildNodes would otherwise leave the script's code behind as text
            sanitizer.RemovingTag += (sender, e) =>
            {
                if (DropWithContentNodeNames.Contains(e.Tag.NodeName))
                    e.Tag.TextContent = String.Empty;
            };

            return sanitizer;
        }

        public static bool TrySanitize(string html, out string result)
        {
            result = html;
            if (String.IsNullOrEmpty(html))
                return true;

            try
            {
                var str = RegexInternalLink.Replace(html, "$1");
                str = RegexTagLike.Replace(str, m => KnownHtmlTagNames.Contains(m.Groups[2].Value)
                                                         ? m.Value
                                                         : HtmlText.Encode(m.Value));
                lock (SanitizerLock)
                    str = Sanitizer.Sanitize(str);
                result = RegexHttpLink.Replace(str, "<a href=\"$1\" onClick=\"return OnUrlJump(this);\">");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NoteHtmlSanitizer: showing note as plain text because: {ex.Message}");
                result = HtmlText.Encode(html);
                return false;
            }
        }

        public static string Sanitize(string html)
        {
            TrySanitize(html, out var result);
            return result;
        }

        // n.b. the HTML parser turns "\r\n" into "\n", so the line breaks are converted afterwards
        public static string ToReadOnlyHtml(string raw)
        {
            return HtmlText.LineBreaksToBr(Sanitize(raw));
        }

        // for search: highlight [nIndex, nIndex+nLength) of the raw note text
        public static string ToReadOnlyHtmlWithHighlight(string raw, int nIndex, int nLength,
            string strBegin, string strEnd)
        {
            if (String.IsNullOrEmpty(raw) || (nIndex < 0) || (nLength <= 0) || (nIndex + nLength > raw.Length))
                return ToReadOnlyHtml(raw);

            var strBeginSentinel = HighlightBeginSentinel.ToString();
            var strEndSentinel = HighlightEndSentinel.ToString();
            var strUnhighlighted = ToReadOnlyHtml(raw);
            var html = ToReadOnlyHtml(raw.Insert(nIndex + nLength, strEndSentinel)
                                         .Insert(nIndex, strBeginSentinel));

            // if the range overlapped markup, the sentinels change how it's parsed (or get dropped).
            //  Only highlight if, apart from the sentinels, it renders exactly as without them
            if ((html.IndexOf(HighlightBeginSentinel) < 0) || (html.IndexOf(HighlightEndSentinel) < 0) ||
                (html.Replace(strBeginSentinel, String.Empty).Replace(strEndSentinel, String.Empty) != strUnhighlighted))
                return strUnhighlighted;

            return html.Replace(strBeginSentinel, strBegin)
                       .Replace(strEndSentinel, strEnd);
        }
    }
}
```

- [ ] **Step 4: Build and run the tests**

Filter `FullyQualifiedName~NoteHtmlSanitizerTests`. Expected: all pass (16 tests, counting the 3 `ShowsUserPseudoTagsAsText` cases).

- [ ] **Step 5: Commit**

```bash
git add StoryEditor/StoryEditor.csproj StoryEditor/NoteHtmlSanitizer.cs StoryEditor.Tests/NoteHtmlSanitizerTests.cs
git commit -m "Add NoteHtmlSanitizer (HtmlSanitizer allowlist) for rendering legacy note HTML

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 5: Encode story-field text when generating HTML

**Files:**
- Modify: `StoryEditor/Diff.cs` `HtmlDiff(string strOrig, string strNew, bool bKeepStringsIntact)` (~:260-320)
- Modify: `StoryEditor/StringTransfer.cs` `FormatLanguageColumnHtml` (~:33-62)
- Modify: `StoryEditor/VerseData.cs` `TryStoryLineStringDiff` (~:761), `GetHtmlCell` (~:1148)
- Modify: `StoryEditor/TestQuestionsData.cs` (~:220, :236, :252)
- Modify: `StoryEditor/ExegeticalHelpNotesData.cs` (~:176)
- Modify: `StoryEditor/AnchorsData.cs` (~:246, print-preview branch)
- Modify: `StoryEditor/LnCNotesData.cs` `FormatLanguageColumn` (~:188)
- Test: `StoryEditor.Tests/HtmlDiffTests.cs`

**Interfaces:**
- Consumes: `HtmlText.Encode(string)`, `HtmlText.Encode(char)`, `HtmlText.LineBreaksToBr`, `HtmlText.ForParagraph` (Task 1).
- Produces:
  - `Diff.HtmlDiff(...)` now returns **safe HTML**: text encoded, insert/delete markup unchanged.
  - **Contract:** `StringTransfer.FormatLanguageColumnHtml(..., string strValue, ...)` takes **HTML**. Callers with plain text must `HtmlText.Encode` it first.

- [ ] **Step 1: Write the failing tests**

`StoryEditor.Tests/HtmlDiffTests.cs`:
```csharp
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlDiffTests
    {
        [SetUp]
        public void SetTags()
        {
            // make the markup predictable (StoryData sets these to styled spans at runtime)
            Rainbow.HtmlDiffEngine.Added.BeginTag = "<ins>";
            Rainbow.HtmlDiffEngine.Added.EndTag = "</ins>";
            Rainbow.HtmlDiffEngine.CommentOff.BeginTag = "<del>";
            Rainbow.HtmlDiffEngine.CommentOff.EndTag = "</del>";
        }

        [Test]
        public void Addition_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff(null, "<donkey bray> & co", false),
                        Is.EqualTo("<ins>&lt;donkey bray&gt; &amp; co</ins>"));
        }

        [Test]
        public void Deletion_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("[B&B]", null, false), Is.EqualTo("<del>[B&amp;B]</del>"));
        }

        [Test]
        public void NoChange_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("a < b", "a < b", false), Is.EqualTo("a &lt; b"));
        }

        [Test]
        public void KeepIntact_IsEncoded()
        {
            Assert.That(Diff.HtmlDiff("x&y", "x<y", true), Is.EqualTo("<del>x&amp;y</del><ins>x&lt;y</ins>"));
        }

        [Test]
        public void CharacterDiff_EncodesEveryPiece()
        {
            var result = Diff.HtmlDiff("a & b", "a < b", false);
            Assert.That(result, Does.Contain("&amp;"));
            Assert.That(result, Does.Contain("&lt;"));
            Assert.That(result.Replace("<ins>", "").Replace("</ins>", "").Replace("<del>", "").Replace("</del>", ""),
                        Does.Not.Contain("<").And.Not.Contain(" & "));
        }
    }
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Build and run with the filter `FullyQualifiedName~HtmlDiffTests`. Expected: FAIL. For example, `Addition_IsEncoded` gets `<ins><donkey bray> & co</ins>`.

- [ ] **Step 3: Encode inside `Diff.HtmlDiff`**

In `StoryEditor/Diff.cs`, `HtmlDiff(string strOrig, string strNew, bool bKeepStringsIntact)`, wrap every piece of text it emits with `HtmlText.Encode`. Only the text changes; the tags do not. The resulting branches:
```csharp
            if (String.IsNullOrEmpty(strOrig) && !String.IsNullOrEmpty(strNew))
                strResult = Rainbow.HtmlDiffEngine.Added.BeginTag + HtmlText.Encode(strNew) + Rainbow.HtmlDiffEngine.Added.EndTag;   // addition

            else if (!String.IsNullOrEmpty(strOrig) && String.IsNullOrEmpty(strNew))
                strResult = Rainbow.HtmlDiffEngine.CommentOff.BeginTag + HtmlText.Encode(strOrig) + Rainbow.HtmlDiffEngine.CommentOff.EndTag;  // deletion

            else if ((String.IsNullOrEmpty(strOrig) && String.IsNullOrEmpty(strNew)) || (strOrig == strNew))
                strResult = HtmlText.Encode(strOrig);    // then there's no change
            else if (bKeepStringsIntact)
            {
                strResult = Rainbow.HtmlDiffEngine.CommentOff.BeginTag + HtmlText.Encode(strOrig) + Rainbow.HtmlDiffEngine.CommentOff.EndTag + Rainbow.HtmlDiffEngine.Added.BeginTag + HtmlText.Encode(strNew) + Rainbow.HtmlDiffEngine.Added.EndTag;
            }
```
In the `#else` (character diff) branch, change the four char appends:
```csharp
                        strResult += HtmlText.Encode(strNew[pos++]);          // unchanged chars (both loops)
                            strResult += HtmlText.Encode(strOrig[it.StartA + m]);   // deleted chars
                            strResult += HtmlText.Encode(strNew[pos++]);      // inserted chars
```
Leave the `#if (UsingRainBow)` branch alone; it isn't compiled. Update the method's `<returns>` doc comment to say "safe HTML (the text is encoded)".

- [ ] **Step 4: Run the HtmlDiff tests**

Expected: 5 passed.

- [ ] **Step 5: Change the `FormatLanguageColumnHtml` contract and encode at plain-text callers**

`StoryEditor/StringTransfer.cs`, in `FormatLanguageColumnHtml` (the 6-parameter overload), add a comment above it and change the paragraph branch:
```csharp
        // strValue is HTML: callers either pass Diff.HtmlDiff output (already safe HTML) or
        //  HtmlText.Encode(plain text)
        public string FormatLanguageColumnHtml(int nVerseIndex, 
```
```csharp
            else
            {
                strHtmlElement = String.Format(Resources.HTML_ParagraphText,
                                               GetStyleClassName(viewSettings.FieldEditibility),
                                               HtmlText.LineBreaksToBr(strValue));
            }
```
The textarea branch stays as is, with `strValue` already encoded.

`StoryEditor/VerseData.cs`:
- `TryStoryLineStringDiff`, the no-diff branch:
  ```csharp
                  strValue = HtmlText.Encode(stringTransfer.GetValue(transliterator));
  ```
- `GetHtmlCell`:
  ```csharp
              var str = HtmlText.Encode(GetStoryLineString(transliterator, stringTransfer));
  ```

`StoryEditor/TestQuestionsData.cs`, the three non-diff branches (~:220, :236, :252):
```csharp
                        : HtmlText.Encode(TestQuestionLine.Vernacular.GetValue(transliterator));
```
```csharp
                        : HtmlText.Encode(TestQuestionLine.NationalBt.GetValue(transliterator));
```
```csharp
                        : HtmlText.Encode(TestQuestionLine.InternationalBt.GetValue(transliterator));
```

`StoryEditor/ExegeticalHelpNotesData.cs` (~:176), the textarea branch:
```csharp
                                                                        HtmlText.Encode(anExHelpNote.ToString()),
```

`StoryEditor/AnchorsData.cs`, print-preview branch (`// this means we're doing print preview, so just give the value without markup`):
```csharp
                    astrExegeticalHelpNotes.Add(HtmlText.Encode(ToolTipText));
```

`StoryEditor/LnCNotesData.cs` `FormatLanguageColumn`:
```csharp
            return String.Format(Properties.Resources.HTML_TableCell,
                                 String.Format(Properties.Resources.HTML_ParagraphText,
                                               strLangStyleClassName,
                                               HtmlText.ForParagraph(strValue)));
```

Leave `StoryData.PresentationHtml(...)` (member names and comments) and the dead code `TestQuestionData.Html(...)` (its only caller is commented out) unchanged.

- [ ] **Step 6: Build and run all tests**

Expected: all pass.

- [ ] **Step 7: Commit**

```bash
git add StoryEditor/Diff.cs StoryEditor/StringTransfer.cs StoryEditor/VerseData.cs StoryEditor/TestQuestionsData.cs StoryEditor/ExegeticalHelpNotesData.cs StoryEditor/AnchorsData.cs StoryEditor/LnCNotesData.cs StoryEditor.Tests/HtmlDiffTests.cs
git commit -m "Encode story text when generating HTML (HtmlDiff output is now safe HTML)

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 6: Render notes through the sanitizer

**Files:**
- Modify: `StoryEditor/ConsultNoteDataConverter.cs`: ReferringText (~:794, ~:917), read-only comments (~:809, ~:951), editable textarea (~:935-938)
- Modify: `StoryEditor/HtmlConNoteControl.cs` `SetSelection` (~:537-564)
- Modify: `StoryEditor/HtmlVerseControl.cs` `ClearSelection` (~:368-390)

**Interfaces:**
- Consumes: `NoteHtmlSanitizer.ToReadOnlyHtml`, `NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight` (Task 4); `HtmlText.Encode`, `HtmlText.ForParagraph` (Task 1).
- Produces: nothing new.

These are UI paths with no unit-test seam; the logic they call is covered by Task 4. Verification is the build plus the manual checks in Task 8.

- [ ] **Step 1: ReferringText (two identical lines)**

In `ConsultNoteDataConverter.cs`, replace both occurrences (use replace-all):
```csharp
                    strReferringHtml = String.Format("<p ondblclick=\"OnDoubleClick(this)\">{0}</p>", ReferringText);
```
with:
```csharp
                    strReferringHtml = String.Format("<p ondblclick=\"OnDoubleClick(this)\">{0}</p>",
                                                     NoteHtmlSanitizer.ToReadOnlyHtml(ReferringText.ToString()));
```

- [ ] **Step 2: Read-only comments (two identical lines)**

Replace both occurrences of:
```csharp
                    strHyperlinkedText += String.Format(Resources.HTML_Paragraph, aCI.ToString().Replace("\r\n", "<br />"));
```
with:
```csharp
                    strHyperlinkedText += String.Format(Resources.HTML_Paragraph, NoteHtmlSanitizer.ToReadOnlyHtml(aCI.ToString()));
```
`SetHyperlinks` still runs on the result right afterwards, so the Ctrl+B/Ctrl+I markers (`$…$`, `*…*`), Bible and line references, and URLs become `<b>`/`<em>`/links after sanitizing, as before.

- [ ] **Step 3: Editable latest-comment textarea**

In the `Html(object htmlConNoteCtrl, …)` builder (~:935):
```csharp
                                            String.Format(Resources.HTML_TextareaWithRefDoubleClick,
                                                          strHtmlElementId,
                                                          StoryData.CstrLangTextAreaStyleClassName,
                                                          HtmlText.Encode(aCI.ToString())));
```

- [ ] **Step 4: Search highlight in a read-only note paragraph**

`HtmlConNoteControl.SetSelection`, paragraph branch:
```csharp
                    HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                    if (elem != null)
                    {
                        var str = NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight(stringTransfer.ToString(),
                                                                                nFoundIndex, nLengthToSelect,
                                                                                CstrParagraphHighlightBegin,
                                                                                CstrParagraphHighlightEnd);
                        System.Diagnostics.Debug.WriteLine(str);
                        elem.InnerHtml = str;
                    }
```

- [ ] **Step 5: Restoring a paragraph after search**

`HtmlVerseControl.ClearSelection`, paragraph branch:
```csharp
                    HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                    if (elem != null)
                        elem.InnerHtml = (stringTransfer is CommInstance)
                                             ? NoteHtmlSanitizer.ToReadOnlyHtml(stringTransfer.ToString())
                                             : HtmlText.ForParagraph(stringTransfer.ToString());
                    else
                        Debug.Assert(false, "unexpected element id in HTML");
```

- [ ] **Step 6: Build and run all tests**

Expected: build succeeds and all tests pass.

- [ ] **Step 7: Commit**

```bash
git add StoryEditor/ConsultNoteDataConverter.cs StoryEditor/HtmlConNoteControl.cs StoryEditor/HtmlVerseControl.cs
git commit -m "Render notes through NoteHtmlSanitizer; encode the editable note textarea

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 7: Decode the IE input routes

**Files:**
- Modify: `StoryEditor/HtmlStoryBtControl.cs` `TextareaOnKeyUp` (~:269-278), `TextareaOnChange` (~:280-304)
- Modify: `StoryEditor/HtmlVerseControl.cs` `SetSelectedText` (~:360)

**Interfaces:**
- Consumes: `HtmlText.FromIeHtmlText` (Task 1).
- Produces: `private bool SetFieldValue(string strId, string strText)` in `HtmlStoryBtControl`.

There are two routes into the Story/BT model:
- **keyup** sends `this.value`, which is plain text.
- **onchange** (`js/StoryBtPs.js:20`) sends `ToNewLines(regexRemoveSpan(range.htmlText))`, which is HTML-encoded.

Only onchange is decoded. Separately, `SetSelectedText` (search and replace) reads `InnerHtml`, which is also encoded. The routes that use `InnerText` (`TextPaster`, `CopyScriptureReference`) are already plain.

- [ ] **Step 1: Split `TextareaOnChange`**

In `HtmlStoryBtControl.cs`:
```csharp
        public bool TextareaOnKeyUp(string strId, string strText)
        {
            // we'll get the value updates during OnChange, but in order to enable 
            //  the save menu, we have to set modified
            System.Diagnostics.Debug.WriteLine($"TextareaOnKeyUp: strId: {strId}, strText: {strText}");
            LastTextareaInFocusId = strId;
            TheSE.LastKeyPressedTimeStamp = DateTime.Now;
            SetFieldValue(strId, strText);  // keyup sends the textarea's 'value', which is plain text
            return true;
        }

        // called from StoryBtPs.js's onchange with text that came from IE's htmlText (i.e. HTML-encoded)
        public bool TextareaOnChange(string strId, string strText)
        {
            return SetFieldValue(strId, HtmlText.FromIeHtmlText(strText));
        }

        private bool SetFieldValue(string strId, string strText)
        {
            System.Diagnostics.Debug.WriteLine($"SetFieldValue: strText: {strText}");
            StoryEditor theSe;
            if (!CheckForProperEditToken(out theSe))
                return false;
```
The rest of the old `TextareaOnChange` body (from `var stringTransfer = GetStringTransfer(strId);` to `return true;`) becomes the rest of `SetFieldValue`, unchanged.

- [ ] **Step 2: `SetSelectedText` reads `InnerHtml`**

`HtmlVerseControl.SetSelectedText`:
```csharp
                        HtmlElement elem = doc.GetElementById(stringTransfer.HtmlElementId);
                        if (elem != null)
                            stringTransfer.SetValue(HtmlText.FromIeHtmlText(elem.InnerHtml));
```

- [ ] **Step 3: Build and run all tests**

Expected: build succeeds and all tests pass.

- [ ] **Step 4: Commit**

```bash
git add StoryEditor/HtmlStoryBtControl.cs StoryEditor/HtmlVerseControl.cs
git commit -m "Decode IE htmlText/innerHTML on the onchange and replace input routes

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

---

### Task 8: Corpus test + manual verification

**Files:**
- Create: `StoryEditor.Tests/CorpusTests.cs`

**Interfaces:**
- Consumes: `ProjectReader.ReadProjectFile`, `LegacyTextRepair.DecodePlainTextFields`, `LegacyTextRepair.ContainsIeEntity`, `NoteHtmlSanitizer.TrySanitize`.

- [ ] **Step 1: Write the corpus test**

`StoryEditor.Tests/CorpusTests.cs`:
```csharp
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
```

- [ ] **Step 2: Run it**

```bash
"$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86 /TestCaseFilter:"FullyQualifiedName~CorpusTests" /logger:"console;verbosity=detailed"
```
(vstest runs `[Explicit]` tests when they are selected by a filter.)
Expected: PASS. The summary line should show `unreadable=2` (the two zero-byte `or-mankidia (2).onestory` copies) and `sanitizerFallbacks=0`.

- [ ] **Step 3: Commit**

```bash
git add StoryEditor.Tests/CorpusTests.cs
git commit -m "Add explicit corpus test over local project files

Co-Authored-By: Claude Opus 5.5 (1M context) <noreply@anthropic.com>"
```

- [ ] **Step 4: Manual verification in the IE build (report the results; don't just tick)**

Build the solution (Debug|x86) and run `output\Debug\StoryEditor.exe`. **Use copies of the projects**: saving adds the marker and decodes text.

1. **indo-mualank** (heavy notes, links, ReferringText):
   - notes look as before
   - Bible-reference and line links in notes work
   - "create note" spans keep their language styling
   - no script runs
2. **lcc-ghiara** (Urdu `&nbsp;`) and **af-batwa** (`<donkey bray>`): story text shows correctly and edits correctly; `<donkey bray>` is visible in print preview.
3. **lb1-marathi:** consultant notes now show `<RTL>` as text.
4. **"Create note" across two textareas:** select a word in the vernacular box and one in a retelling box, right-click → add note. The highlight and the styled note look the same as before.
5. **Type `&` and `<x>`** in a story box, then select part of the text in another box so a highlight span is created. Save, reopen: the text is identical. Use **Search/Replace** on that box: the text is still correct.
6. **Ctrl+B / Ctrl+I** in the editable note box still insert `$…$` / `*…*`, which render bold/italic once the note is read-only.
7. **Revision history / print preview** of a story containing `&` shows it correctly, with diff colouring intact.
8. **Save, then open the `.onestory` in a text editor:** the root has `TextEncoding="plain"` and an unchanged `version`.
9. **Released exe round trip:** open that saved copy in the currently released OSE. It must open without complaint. Edit a field containing `&`, save, and confirm the marker is gone. Reopen in this build: the field is clean.
10. **Copy a story** (Panorama, Ctrl+C) from a project saved by the released exe, and paste it into a project in this build: `[B&B]`-style text arrives decoded.

---

## Self-review notes

- **Spec coverage:**
  - `LegacyTextRepair` → T2
  - marker + clipboard → T3
  - `HtmlText` → T1
  - `NoteHtmlSanitizer` → T4
  - existing-code changes → T5 (story fields, `FormatLanguageColumnHtml` contract), T6 (notes, search), T7 (input routes)
  - testing → T1-T5, T8
  - out-of-scope items untouched; `RobustFile`/save sequence untouched (Global Constraints)
- **Refinements beyond rev 2 of the spec:** `FormatLanguageColumnHtml` takes HTML; internal links are unwrapped; pseudo-tags are shown as text. These are recorded in the spec (rev 3).
