using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// Golden tests for the project-level XElement constructors (see Golden): each test loads a project file as an
    /// XDocument, builds the object and compares what it holds (every field the old row-constructor oracle tests
    /// compared, and GetXml) with a golden file. Assertions on values are kept as they were.
    /// Fixtures never contain duplicate member names (the loader shows a message box for those).
    /// </summary>
    [TestFixture]
    public class FromXmlProjectTests
    {
        private const string ProjectName = "characterization-project";

        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization-project.onestory");

        private XElement _root;
        private System.Diagnostics.TraceListener[] _savedListeners;
        private readonly List<string> _tempPaths = new List<string>();

        [SetUp]
        public void Load()
        {
            // the loader Debug.Asserts in a few places; with the default listener that kills the test host
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
            _root = LoadRoot(FixturePath);
        }

        [TearDown]
        public void Cleanup()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(_savedListeners);
            foreach (var strPath in _tempPaths)
            {
                try
                {
                    if (Directory.Exists(strPath))
                        Directory.Delete(strPath, true);
                    else
                        File.Delete(strPath);
                }
                catch (Exception)
                {
                    // best effort
                }
            }
        }

        private static XElement LoadRoot(string strPath)
        {
            return XDocument.Load(strPath, LoadOptions.None).Root;
        }

        private string WriteTemp(string strRootChildrenXml, string strRootAttributes = "version=\"1.8\" ProjectName=\"" + ProjectName + "\" PanoramaFrontMatter=\"pfm\"")
        {
            var strPath = Path.Combine(Path.GetTempPath(), "ose-project-" + Guid.NewGuid() + ".onestory");
            File.WriteAllText(strPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<StoryProject " + strRootAttributes + ">" + strRootChildrenXml + "</StoryProject>");
            _tempPaths.Add(strPath);
            return strPath;
        }

        private const string CrafterMember =
            "<Member name=\"c\" memberType=\"Crafter\" memberKey=\"m1\" />";

        // ----- members -----

        // every instance field (public or private) of a simple type, so nothing the loader sets is missed
        private static int DumpFields(Dump dump, object obj, string strMsg)
        {
            var nDumped = 0;
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var type = field.FieldType;
                if (!(type == typeof(string) || type == typeof(float) || type == typeof(bool) || type == typeof(long) || type.IsEnum))
                    continue;
                dump.Line($"{strMsg}: {field.Name}", field.GetValue(obj));
                nDumped++;
            }
            return nDumped;
        }

        [Test]
        public void TeamMemberData_Golden_ForEveryMember()
        {
            var elems = _root.Descendants("Member").ToList();
            Assert.That(elems.Count, Is.EqualTo(5));
            var dump = new Dump();
            for (int i = 0; i < elems.Count; i++)
            {
                var member = new TeamMemberData(elems[i]);
                Assert.That(DumpFields(dump.Raw($"== member {i}"), member, $"member {i}"), Is.GreaterThan(30));
                dump.Xml("GetXml", member.GetXml);
            }
            Golden.Check("project-members", dump.ToString());

            var full = new TeamMemberData(elems[0]);
            Assert.That(full.HgPassword, Is.EqualTo("s3cret!"));   // decrypted
            Assert.That(full.OverrideFontSizeVernacular, Is.EqualTo(14.5f));
            Assert.That(full.OverrideRtlInternationalBT, Is.True);      // "1"
            Assert.That(full.BioData, Is.EqualTo("bio line 1\nbio line 2"));   // member text is not normalized
            Assert.That(new TeamMemberData(elems[1]).DefaultAllowed, Is.Not.EqualTo(0L));
            Assert.That(new TeamMemberData(elems[2]).DefaultAllowed, Is.Not.EqualTo(0L));
            Assert.That(new TeamMemberData(elems[3]).MemberType, Is.EqualTo(TeamMemberData.UserTypes.Coach));
            Assert.That(new TeamMemberData(elems[4]).OverrideFontSizeVernacular, Is.EqualTo(12f));
        }

        private static void DumpMembers(Dump dump, TeamMembersData members)
        {
            dump.Line("Count", members.Count).Line("Keys", string.Join("|", members.Keys.Cast<string>()))
                .Line("HasOutsideEnglishBTer", members.HasOutsideEnglishBTer)
                .Line("HasLanguageSpecialtyReviewer", members.HasLanguageSpecialtyReviewer)
                .Line("HasIndependentConsultant", members.HasIndependentConsultant)
                .Xml("GetXml", members.GetXml);
        }

        [Test]
        public void TeamMembersData_Golden()
        {
            var newMembers = new TeamMembersData(_root);
            Assert.That(newMembers.Count, Is.EqualTo(5));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.True);
            var dump = new Dump();
            DumpMembers(dump, newMembers);
            Golden.Check("project-team-members", dump.ToString());
        }

        [TestCase(0, "<Members>" + CrafterMember + "<Member name=\"e\" memberType=\"EnglishBackTranslator\" memberKey=\"m2\" /><Member name=\"i\" memberType=\"IndependentConsultant\" memberKey=\"m3\" /><Member name=\"f\" memberType=\"FirstPassMentor\" memberKey=\"m4\" /></Members>", true, false, true)]
        [TestCase(1, "<Members>" + CrafterMember + "</Members>", false, false, false)]
        [TestCase(2, "<Members HasOutsideEnglishBTer=\"false\" HasIndependentConsultant=\"true\">" + CrafterMember + "<Member name=\"e\" memberType=\"EnglishBackTranslator\" memberKey=\"m2\" /></Members>", false, false, true)]
        [TestCase(3, "", false, false, false)]    // no <Members> at all: all three flags are false
        public void TeamMembersData_HasFlags_AbsentOrExplicit(int nCase, string strMembersXml, bool bExpectOebt, bool bExpectLsr, bool bExpectIc)
        {
            var strPath = WriteTemp(strMembersXml + "<Languages /><LnCNotes /><stories SetName=\"Stories\" />");
            var newMembers = new TeamMembersData(LoadRoot(strPath));
            Assert.That(newMembers.HasOutsideEnglishBTer, Is.EqualTo(bExpectOebt));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.EqualTo(bExpectLsr));
            Assert.That(newMembers.HasIndependentConsultant, Is.EqualTo(bExpectIc));
            var dump = new Dump();
            DumpMembers(dump, newMembers);
            Golden.Check("project-team-members-flags-" + nCase, dump.ToString());
        }

        // ----- project settings -----

        private static void DumpLanguage(Dump dump, string strMsg, ProjectSettings.LanguageInfo lang)
        {
            dump.Line(strMsg + " LangType", lang.LangType).Line(strMsg + " LangName", lang.LangName)
                .Line(strMsg + " LangCode", lang.LangCode).Line(strMsg + " DefaultFontName", lang.DefaultFontName)
                .Line(strMsg + " DefaultFontSize", lang.DefaultFontSize).Line(strMsg + " FontToUse.Name", lang.FontToUse.Name)
                .Line(strMsg + " FontToUse.Size", lang.FontToUse.Size).Line(strMsg + " FontColor.Name", lang.FontColor.Name)
                .Line(strMsg + " FullStop", lang.FullStop).Line(strMsg + " DefaultKeyboard", lang.DefaultKeyboard)
                .Line(strMsg + " KeyboardOverride", lang.KeyboardOverride).Line(strMsg + " DefaultRtl", lang.DefaultRtl)
                .Line(strMsg + " InvertRtl", lang.InvertRtl).Line(strMsg + " HasData", lang.HasData);
        }

        private static void DumpAdaptIt(Dump dump, string strMsg, ProjectSettings.AdaptItConfiguration config)
        {
            if (config == null)
            {
                dump.Line(strMsg, null);
                return;
            }
            dump.Line(strMsg + " ProjectType", config.ProjectType).Line(strMsg + " BtDirection", config.BtDirection)
                .Line(strMsg + " ConverterName", config.ConverterName).Line(strMsg + " ProjectFolderName", config.ProjectFolderName)
                .Line(strMsg + " RepoProjectName", config.RepoProjectName).Line(strMsg + " RepositoryServer", config.RepositoryServer)
                .Line(strMsg + " NetworkRepositoryPath", config.NetworkRepositoryPath).Xml(strMsg + " GetXml", config.GetXml);
        }

        private static void DumpShow(Dump dump, string strMsg, ShowLanguageFields show)
        {
            dump.Line(strMsg + " Vernacular", show.Vernacular).Line(strMsg + " NationalBt", show.NationalBt)
                .Line(strMsg + " InternationalBt", show.InternationalBt);
        }

        private static void DumpSettings(Dump dump, ProjectSettings settings)
        {
            DumpLanguage(dump, "Vernacular", settings.Vernacular);
            DumpLanguage(dump, "NationalBT", settings.NationalBT);
            DumpLanguage(dump, "InternationalBT", settings.InternationalBT);
            DumpLanguage(dump, "FreeTranslation", settings.FreeTranslation);
            DumpShow(dump, "ShowRetellings", settings.ShowRetellings);
            DumpShow(dump, "ShowTestQuestions", settings.ShowTestQuestions);
            DumpShow(dump, "ShowAnswers", settings.ShowAnswers);
            DumpAdaptIt(dump, "VernacularToNationalBt", settings.VernacularToNationalBt);
            DumpAdaptIt(dump, "VernacularToInternationalBt", settings.VernacularToInternationalBt);
            DumpAdaptIt(dump, "NationalBtToInternationalBt", settings.NationalBtToInternationalBt);
            dump.Line("IsConfigured", settings.IsConfigured).Xml("GetXml", settings.GetXml)
                .Line("HasAdaptItConfigurationData", settings.HasAdaptItConfigurationData);
            if (settings.HasAdaptItConfigurationData)
                dump.Xml("AdaptItConfigXml", settings.AdaptItConfigXml);
        }

        private ProjectSettings SettingsFromRoot(XElement root)
        {
            var settings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            settings.SerializeProjectSettings(root);
            return settings;
        }

        private void AssertSettingsGolden(string strName, XElement root)
        {
            var dump = new Dump();
            DumpSettings(dump, SettingsFromRoot(root));
            Golden.Check(strName, dump.ToString());
        }

        [Test]
        public void ProjectSettings_FullFixture_Golden()
        {
            AssertSettingsGolden("project-settings-full", _root);

            var settings = SettingsFromRoot(_root);
            Assert.That(settings.IsConfigured, Is.True);
            Assert.That(settings.ShowRetellings.Vernacular, Is.True);
            Assert.That(settings.ShowRetellings.NationalBt, Is.False);
            Assert.That(settings.ShowTestQuestions.NationalBt, Is.True);
            Assert.That(settings.ShowAnswers.Vernacular, Is.False);     // absent attribute: default kept
            Assert.That(settings.ShowAnswers.InternationalBt, Is.True);
            Assert.That(settings.Vernacular.DefaultRtl, Is.True);
            Assert.That(settings.Vernacular.DefaultKeyboard, Is.EqualTo("Keyman Vern default"));
            Assert.That(settings.NationalBT.DefaultKeyboard, Is.Null);      // Keyboard="" is null
            Assert.That(settings.NationalBT.FullStop, Is.EqualTo("."));
            Assert.That(settings.VernacularToNationalBt.ProjectType, Is.EqualTo(ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly));
            Assert.That(settings.VernacularToInternationalBt.NetworkRepositoryPath, Is.EqualTo("\\\\server\\share\\Tst-Eng"));
            Assert.That(settings.NationalBtToInternationalBt.HasData, Is.False);
            Assert.That(settings.FreeTranslation.LangName, Is.EqualTo("Free English"));
        }

        [Test]
        public void ProjectSettings_NoLanguagesNoAdaptIt_Golden()
        {
            AssertSettingsGolden("project-settings-none",
                LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members><LnCNotes /><stories SetName=\"Stories\" />")));
        }

        [Test]
        public void ProjectSettings_NoInternationalOrFreeTranslation_ClearsTheirDefaultNames()
        {
            var root = LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members>" +
                                          "<Languages UseRetellingInternationalBT=\"false\">" +
                                          "<LanguageInfo lang=\"Vernacular\" name=\"V\" code=\"v\" FontName=\"Arial\" FontSize=\"9\" FontColor=\"Red\" SentenceFinalPunct=\".\" />" +
                                          "</Languages><LnCNotes /><stories SetName=\"Stories\" />"));
            AssertSettingsGolden("project-settings-vernacular-only", root);

            var settings = SettingsFromRoot(root);
            Assert.That(settings.InternationalBT.HasData, Is.False);
            Assert.That(settings.FreeTranslation.HasData, Is.False);
            Assert.That(settings.ShowRetellings.InternationalBt, Is.False);
        }

        [Test]
        public void ProjectSettings_TwoAdaptItConfigurationsElements_ReadsNeither()
        {
            const string strConfig = "<AdaptItConfiguration ProjectType=\"LocalAiProjectOnly\" BtDirection=\"VernacularToNationalBt\" ConverterName=\"c\" />";
            var root = LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members><Languages />" +
                                          "<AdaptItConfigurations>" + strConfig + "</AdaptItConfigurations>" +
                                          "<AdaptItConfigurations>" + strConfig + "</AdaptItConfigurations>" +
                                          "<LnCNotes /><stories SetName=\"Stories\" />"));
            AssertSettingsGolden("project-settings-two-adaptit", root);
            Assert.That(SettingsFromRoot(root).VernacularToNationalBt, Is.Null);
        }

        [Test]
        public void AdaptItConfiguration_And_LanguageInfo_Golden_Individually()
        {
            var aiElems = _root.Descendants("AdaptItConfiguration").ToList();
            Assert.That(aiElems.Count, Is.EqualTo(3));
            var dump = new Dump();
            for (int i = 0; i < aiElems.Count; i++)
            {
                var newAi = new ProjectSettings.AdaptItConfiguration();
                newAi.SerializeFromProjectFile(aiElems[i]);
                DumpAdaptIt(dump, $"config {i}", newAi);
            }

            var langElems = _root.Descendants("LanguageInfo").ToList();
            Assert.That(langElems.Count, Is.EqualTo(4));
            var settings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            for (int i = 0; i < langElems.Count; i++)
            {
                var newLang = new ProjectSettings.LanguageInfo(settings.Vernacular.LangType, new System.Drawing.Font("Arial", 12), System.Drawing.Color.Black);
                newLang.Serialize(langElems[i]);
                DumpLanguage(dump, $"language {i}", newLang);
            }
            Golden.Check("project-adaptit-and-languages", dump.ToString());
        }

        [Test]
        public void ProjectSettings_FromAnotherProjectsElement_ReadsLanguagesLeniently()
        {
            // the lenient constructor used for a project element that came from Chorus or another project
            var settings = new ProjectSettings(_root, null);
            Assert.That(settings.ProjectName, Is.EqualTo(ProjectName));
            Assert.That(settings.UseDropbox, Is.True);
            Assert.That(settings.DropboxRetelling, Is.False);
            Assert.That(settings.Vernacular.LangType, Is.EqualTo("Vernacular"));
            Assert.That(settings.Vernacular.HasData, Is.True);
            Assert.That(settings.FreeTranslation.LangName, Is.EqualTo("Free English"));

            var settingsNone = new ProjectSettings(new XElement("StoryProject"), null);
            Assert.That(settingsNone.Vernacular.HasData, Is.False);
            Assert.That(settingsNone.ProjectName, Is.Null);
        }

        // ----- L&C notes -----

        [Test]
        public void LnCNotesData_Golden()
        {
            var newNotes = new LnCNotesData(_root);
            Assert.That(newNotes.Count, Is.EqualTo(2));
            var dump = new Dump();
            for (int i = 0; i < newNotes.Count; i++)
                dump.Line($"note {i} Notes", newNotes[i].Notes).Line($"note {i} VernacularRendering", newNotes[i].VernacularRendering)
                    .Line($"note {i} NationalBtRendering", newNotes[i].NationalBtRendering)
                    .Line($"note {i} InternationalBtRendering", newNotes[i].InternationalBtRendering);
            dump.Xml("GetXml", newNotes.GetXml);
            Golden.Check("project-lnc-notes", dump.ToString());

            // GetXml writes KeyTermIds (it used to write the singular KeyTermId)
            Assert.That((string)newNotes.GetXml.Elements("LnCNote").First().Attribute("KeyTermIds"), Is.EqualTo("KT1, KT2"));
            Assert.That(newNotes[0].Notes, Is.EqualTo("note with renderings\r\nsecond line"));
            Assert.That(newNotes[0].VernacularRendering, Is.EqualTo("verb\r\nrendering"));
        }

        [Test]
        public void LnCNotesData_NoLnCNotesElement_IsEmpty()
        {
            var newNotes = new LnCNotesData(LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members>")));
            Assert.That(newNotes.Count, Is.EqualTo(0));
            Assert.That(newNotes.GetXml.ToString(), Is.EqualTo("<LnCNotes />"));
        }

        [Test]
        public void LnCNote_WithoutAnyText_HasEmptyNotes()
        {
            var newNotes = new LnCNotesData(LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members><LnCNotes><LnCNote guid=\"g1\" VernacularRendering=\"v\" /></LnCNotes>")));
            Assert.That(newNotes.Count, Is.EqualTo(1));
            Assert.That(newNotes[0].Notes, Is.EqualTo(String.Empty));
            Golden.Check("project-lnc-note-no-text", newNotes.GetXml.ToString());
        }

        // ----- story sets -----

        [Test]
        public void StoriesData_Golden_ForEachOfTheThreeSets()
        {
            var strProjectFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            var elems = _root.Elements("stories").ToList();
            Assert.That(elems.Count, Is.EqualTo(3));
            var expectedCounts = new[] { 2, 1, 1 };
            var dump = new Dump();
            for (int i = 0; i < elems.Count; i++)
            {
                ProjectFile.UniqueStoryGuids.Clear();
                var newStories = new StoriesData(elems[i], strProjectFolder);
                Assert.That(newStories.Count, Is.EqualTo(expectedCounts[i]), $"set {i}");
                dump.Raw($"== set {i}").Line("SetName", newStories.SetName).Line("Count", newStories.Count)
                    .Xml("GetXml", newStories.GetXml);
            }
            Golden.Check("project-story-sets", dump.ToString());
            Assert.That(elems.Select(e => (string)e.Attribute("SetName")), Is.EqualTo(new[] { "Stories", "Non-Biblical Stories", "Old Stories" }));
        }

        [Test]
        public void StoriesData_DuplicateStoryNames_AreRenamed()
        {
            const string strStory = "<story name=\"Same\" stage=\"ProjFacTypeVernacular\" guid=\"{0}\" stageDateTimeStamp=\"2026-10-03T12:34:56Z\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "<Verses><Verse guid=\"v{0}\" first=\"true\" /></Verses></story>";
            var root = LoadRoot(WriteTemp("<Members>" + CrafterMember + "</Members><Languages /><LnCNotes />" +
                                          "<stories SetName=\"Stories\">" +
                                          string.Format(strStory, "g1") + string.Format(strStory, "g2") + string.Format(strStory, "g3") +
                                          "</stories>"));
            var strProjectFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            ProjectFile.UniqueStoryGuids.Clear();
            var newStories = new StoriesData(root.Element("stories"), strProjectFolder);
            Assert.That(newStories.Select(s => s.Name), Is.EqualTo(new[] { "Same", "Same.1", "Same.2" }));
            Golden.Check("project-story-sets-duplicate-names", newStories.GetXml.ToString());
        }

        // ----- ClearLanguageNamePlaceholders -----

        [Test]
        public void ClearLanguageNamePlaceholders_XElement_ClearsTheNamedPlaceholders()
        {
            var strProject =
                "<Members>" + CrafterMember + "</Members>" +
                "<Languages>" +
                "<LanguageInfo lang=\"Vernacular\" name=\"Testish\" code=\"t\" FontName=\"Arial\" FontSize=\"9\" FontColor=\"Red\" SentenceFinalPunct=\".\" />" +
                "<LanguageInfo lang=\"NationalBt\" name=\"  Nationalese  \" code=\"n\" FontName=\"Arial\" FontSize=\"9\" FontColor=\"Red\" SentenceFinalPunct=\".\" />" +
                "<LanguageInfo lang=\"InternationalBt\" name=\"English\" code=\"e\" FontName=\"Arial\" FontSize=\"9\" FontColor=\"Red\" SentenceFinalPunct=\".\" />" +
                "</Languages><LnCNotes /><stories SetName=\"Stories\">" +
                "<story name=\"S\" stage=\"ProjFacTypeVernacular\" guid=\"g1\"><CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                "<Verses><Verse guid=\"v1\" first=\"true\">" +
                "<StoryLine lang=\"Vernacular\">Testish</StoryLine>" +
                "<StoryLine lang=\"NationalBt\"> Nationalese </StoryLine>" +
                "<StoryLine lang=\"InternationalBt\">Testish</StoryLine>" +           // another language's name: kept
                "<StoryLine lang=\"FreeTranslation\">English</StoryLine>" +             // no such language: kept
                "<Retellings><Retelling lang=\"Vernacular\" memberID=\"m1\">Testish</Retelling><Retelling lang=\"InternationalBt\" memberID=\"m1\">English</Retelling><Retelling lang=\"NationalBt\" memberID=\"m1\" /></Retellings>" +
                "<TestQuestions><TestQuestion visible=\"true\" guid=\"tq1\"><TestQuestionLine lang=\"Vernacular\">Testish</TestQuestionLine><TestQuestionLine lang=\"Vernacular\">Testish and more</TestQuestionLine>" +
                "<Answers><Answer lang=\"NationalBt\" memberID=\"m1\">Nationalese</Answer><Answer lang=\"Vernacular\" memberID=\"m1\">   </Answer></Answers></TestQuestion></TestQuestions>" +
                "<ConsultantNotes><ConsultantConversation guid=\"cc1\"><ConsultantNote Direction=\"ConsultantToProjFac\" guid=\"cn1\" memberID=\"m1\">Testish</ConsultantNote></ConsultantConversation></ConsultantNotes>" +
                "</Verse></Verses></story></stories>";
            var root = LoadRoot(WriteTemp(strProject));

            var nNew = LegacyTextRepair.ClearLanguageNamePlaceholders(root);
            // Vernacular and National lines, Vernacular and International retellings, the TQ line and the National answer
            Assert.That(nNew, Is.EqualTo(6));

            // what the DataSet held after clearing: "" where it cleared a value; the element has no text at all then
            Assert.That(root.Descendants("StoryLine").Select(e => e.Value), Is.EqualTo(new[] { "", "", "Testish", "English" }));
            Assert.That(root.Descendants("Retelling").Select(e => e.Value), Is.EqualTo(new[] { "", "", "" }));
            Assert.That(root.Descendants("TestQuestionLine").Select(e => e.Value), Is.EqualTo(new[] { "", "Testish and more" }));
            Assert.That(root.Descendants("Answer").Select(e => e.Value), Is.EqualTo(new[] { "", "" }));   // (the parser drops whitespace-only text)
            Assert.That(root.Descendants("StoryLine").Select(e => XmlRead.Text(e)), Is.EqualTo(new[] { "", "", "Testish", "English" }));
            Assert.That(root.Descendants("Answer").Select(e => XmlRead.Text(e)), Is.EqualTo(new[] { "", null }));
            // not placeholders, whatever the text
            Assert.That(root.Descendants("ConsultantNote").Single().Value, Is.EqualTo("Testish"));
        }

        // ----- StoryProjectData -----

        // The ProjectSettings is built the way StoryEditor.OpenProject does (the folder and the name); that needs no UI
        //  or machine state. A temp project folder holds a copy of the file (LoadOsMetaData looks for OsMetaData.xml there).
        private string MakeProjectFolder(string strFixtureXml)
        {
            var strFolder = Path.Combine(Path.GetTempPath(), "ose-proj-" + Guid.NewGuid());
            Directory.CreateDirectory(strFolder);
            _tempPaths.Add(strFolder);
            File.WriteAllText(Path.Combine(strFolder, ProjectName + ".onestory"), strFixtureXml);
            return strFolder;
        }

        private static void DumpProjectData(Dump dump, StoryProjectData project)
        {
            dump.Line("Keys", string.Join("|", project.Keys.Cast<string>())).Line("PanoramaFrontMatter", project.PanoramaFrontMatter)
                .Line("ProjectName", project.ProjSettings.ProjectName).Line("UseDropbox", project.ProjSettings.UseDropbox)
                .Line("DropboxStory", project.ProjSettings.DropboxStory).Line("DropboxRetelling", project.ProjSettings.DropboxRetelling)
                .Line("DropboxAnswers", project.ProjSettings.DropboxAnswers).Line("OsMetaData is null", project.OsMetaData == null);
            DumpSettings(dump, project.ProjSettings);
            dump.Xml("GetXml", project.GetXml);
        }

        private StoryProjectData BuildProject(string strXml, string strProjectName, string strGoldenName)
        {
            var strFolder = MakeProjectFolder(strXml);
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");
            var contents = ProjectFile.Load(strPath);
            var project = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, strProjectName));
            var dump = new Dump();
            DumpProjectData(dump, project);
            Golden.Check(strGoldenName, dump.ToString());
            return project;
        }

        [Test]
        public void StoryProjectData_FullFixture_Golden()
        {
            var project = BuildProject(File.ReadAllText(FixturePath), ProjectName, "project-data-full");

            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(new[] { "Stories", "Non-Biblical Stories", "Old Stories" }));
            Assert.That(project.PanoramaFrontMatter, Is.EqualTo("Front matter text"));
            Assert.That(project.ProjSettings.UseDropbox, Is.True);
            Assert.That(project.ProjSettings.DropboxStory, Is.True);
            Assert.That(project.ProjSettings.DropboxRetelling, Is.False);
            Assert.That(project.ProjSettings.DropboxAnswers, Is.True);
            Assert.That(project.TeamMembers.Count, Is.EqualTo(5));
            Assert.That(project.LnCNotes.Count, Is.EqualTo(2));
            Assert.That(project.ProjSettings.IsConfigured, Is.True);
            Assert.That(project.OsMetaData, Is.Null);
            Assert.That(project["Stories"].Count, Is.EqualTo(2));

            // the file isn't marked as plain text: entities are decoded (before any child is built), and a value that
            //  is only its language's name is cleared
            var strXml = project.GetXml.ToString();
            Assert.That(strXml, Does.Contain("first &amp; line"));
            Assert.That(strXml, Does.Not.Contain("Testish</StoryLine>"));
        }

        [Test]
        public void StoryProjectData_OverwritesProjectNameFromSettings()
        {
            var newProject = BuildProject(File.ReadAllText(FixturePath), "renamed", "project-data-renamed");
            Assert.That(newProject.GetXml.Attribute("ProjectName").Value, Is.EqualTo("renamed"));
        }

        [Test]
        public void StoryProjectData_MarkedPlain_IsNotDecoded()
        {
            var strXml = File.ReadAllText(FixturePath).Replace("<StoryProject version=\"1.8\"",
                "<StoryProject TextEncoding=\"plain\" version=\"1.8\"");
            var project = BuildProject(strXml, ProjectName, "project-data-marked-plain");
            Assert.That(project.GetXml.ToString(), Does.Contain("first &amp;amp; line"));
        }

        [TestCase(0, new[] { "Stories", "Old Stories" })]      // none: two sets are added
        [TestCase(1, new[] { "Stories" })]                      // one: nothing added
        public void StoryProjectData_StorySetAdditions(int nSetsToKeep, string[] expectedKeys)
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            var sets = doc.Root.Elements("stories").ToList();
            for (int i = nSetsToKeep; i < sets.Count; i++)
                sets[i].Remove();
            var project = BuildProject(doc.ToString(), ProjectName, "project-data-sets-kept-" + nSetsToKeep);
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(expectedKeys));
        }

        [Test]
        public void StoryProjectData_TwoSetsWithoutNonBiblical_AddsTheNonBiblicalSet()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Elements("stories").Single(s => (string)s.Attribute("SetName") == "Non-Biblical Stories").Remove();
            var project = BuildProject(doc.ToString(), ProjectName, "project-data-two-sets-add-nonbiblical");
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(new[] { "Stories", "Old Stories", "Non-Biblical Stories" }));
        }

        [Test]
        public void StoryProjectData_TwoSetsIncludingNonBiblical_AddsNothing()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Elements("stories").Single(s => (string)s.Attribute("SetName") == "Old Stories").Remove();
            var project = BuildProject(doc.ToString(), ProjectName, "project-data-two-sets-with-nonbiblical");
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(new[] { "Stories", "Non-Biblical Stories" }));
        }

        [Test]
        public void StoryProjectData_NoDropboxAttributesAndEmptyFrontMatter_UseDefaults()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            foreach (var strName in new[] { "UseDropbox", "DropboxStory", "DropboxRetellings", "DropboxAnswers" })
                doc.Root.Attribute(strName).Remove();
            doc.Root.SetAttributeValue("PanoramaFrontMatter", "");
            var project = BuildProject(doc.ToString(), ProjectName, "project-data-defaults");
            Assert.That(project.ProjSettings.UseDropbox, Is.False);
            Assert.That(project.PanoramaFrontMatter, Is.EqualTo(Properties.Resources.IDS_DefaultPanoramaFrontMatter));
        }

        [Test]
        public void StoryProjectData_MissingPanoramaFrontMatter_Throws()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Attribute("PanoramaFrontMatter").Remove();
            var strFolder = MakeProjectFolder(doc.ToString());
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");

            var contents = ProjectFile.Load(strPath);
            Assert.Throws<ApplicationException>(() =>
                new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, ProjectName)));
        }
    }
}
