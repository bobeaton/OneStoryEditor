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
    /// The typed DataSet row constructors are the oracle for the project-level XElement constructors: each test
    /// loads a project file both as a DataSet (ProjectReader) and as an XDocument and requires identical results.
    /// A fresh ProjectReader per test, because the row constructors add empty container rows to the DataSet.
    /// Fixtures never contain duplicate member names (the row path shows a message box for those).
    /// </summary>
    [TestFixture]
    public class FromXmlProjectTests
    {
        private const string ProjectName = "characterization-project";

        private static string FixturePath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "characterization-project.onestory");

        private ProjectReader _ds;
        private XElement _root;
        private System.Diagnostics.TraceListener[] _savedListeners;
        private readonly List<string> _tempPaths = new List<string>();

        [SetUp]
        public void Load()
        {
            // the row constructors Debug.Assert in a few places; with the default listener that kills the test host
            _savedListeners = new System.Diagnostics.TraceListener[System.Diagnostics.Trace.Listeners.Count];
            System.Diagnostics.Trace.Listeners.CopyTo(_savedListeners, 0);
            System.Diagnostics.Trace.Listeners.Clear();
            LoadBoth(FixturePath);
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

        private void LoadBoth(string strPath)
        {
            ProjectReader.ReadProjectFile(strPath, out _ds);
            _root = XDocument.Load(strPath, LoadOptions.None).Root;
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

        // every instance field (public or private) of a simple type, so nothing the row path sets is missed
        private static void AssertSameFields(object actual, object expected, string strMsg)
        {
            var nCompared = 0;
            foreach (var field in expected.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var type = field.FieldType;
                if (!(type == typeof(string) || type == typeof(float) || type == typeof(bool) || type == typeof(long) || type.IsEnum))
                    continue;
                Assert.That(field.GetValue(actual), Is.EqualTo(field.GetValue(expected)), $"{strMsg}: {field.Name}");
                nCompared++;
            }
            Assert.That(nCompared, Is.GreaterThan(30), strMsg);
        }

        [Test]
        public void TeamMemberData_MatchesRowPath_ForEveryMember()
        {
            var rows = _ds.Member.ToList();
            var elems = _root.Descendants("Member").ToList();
            Assert.That(rows.Count, Is.EqualTo(5));
            Assert.That(elems.Count, Is.EqualTo(rows.Count));
            for (int i = 0; i < rows.Count; i++)
            {
                var oldMember = new TeamMemberData(rows[i]);
                var newMember = new TeamMemberData(elems[i]);
                AssertSameFields(newMember, oldMember, $"member {i}");
                Assert.That(newMember.GetXml.ToString(), Is.EqualTo(oldMember.GetXml.ToString()), $"member {i}");
            }

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

        [Test]
        public void TeamMembersData_MatchesRowPath()
        {
            var oldMembers = new TeamMembersData(_ds);
            var newMembers = new TeamMembersData(_root);
            Assert.That(newMembers.Count, Is.EqualTo(5));
            Assert.That(newMembers.Keys, Is.EqualTo(oldMembers.Keys));
            Assert.That(newMembers.HasOutsideEnglishBTer, Is.EqualTo(oldMembers.HasOutsideEnglishBTer));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.EqualTo(oldMembers.HasLanguageSpecialtyReviewer));
            Assert.That(newMembers.HasIndependentConsultant, Is.EqualTo(oldMembers.HasIndependentConsultant));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.True);
            Assert.That(newMembers.GetXml.ToString(), Is.EqualTo(oldMembers.GetXml.ToString()));
        }

        [TestCase("<Members>" + CrafterMember + "<Member name=\"e\" memberType=\"EnglishBackTranslator\" memberKey=\"m2\" /><Member name=\"i\" memberType=\"IndependentConsultant\" memberKey=\"m3\" /><Member name=\"f\" memberType=\"FirstPassMentor\" memberKey=\"m4\" /></Members>", true, false, true)]
        [TestCase("<Members>" + CrafterMember + "</Members>", false, false, false)]
        [TestCase("<Members HasOutsideEnglishBTer=\"false\" HasIndependentConsultant=\"true\">" + CrafterMember + "<Member name=\"e\" memberType=\"EnglishBackTranslator\" memberKey=\"m2\" /></Members>", false, false, true)]
        [TestCase("", false, false, false)]    // no <Members> at all: the row path's added row has all three flags false
        public void TeamMembersData_HasFlags_AbsentOrExplicit_MatchRowPath(string strMembersXml, bool bExpectOebt, bool bExpectLsr, bool bExpectIc)
        {
            var strPath = WriteTemp(strMembersXml + "<Languages /><LnCNotes /><stories SetName=\"Stories\" />");
            LoadBoth(strPath);
            var oldMembers = new TeamMembersData(_ds);
            var newMembers = new TeamMembersData(_root);
            Assert.That(newMembers.HasOutsideEnglishBTer, Is.EqualTo(bExpectOebt));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.EqualTo(bExpectLsr));
            Assert.That(newMembers.HasIndependentConsultant, Is.EqualTo(bExpectIc));
            Assert.That(newMembers.HasOutsideEnglishBTer, Is.EqualTo(oldMembers.HasOutsideEnglishBTer));
            Assert.That(newMembers.HasLanguageSpecialtyReviewer, Is.EqualTo(oldMembers.HasLanguageSpecialtyReviewer));
            Assert.That(newMembers.HasIndependentConsultant, Is.EqualTo(oldMembers.HasIndependentConsultant));
            Assert.That(newMembers.Keys, Is.EqualTo(oldMembers.Keys));
            Assert.That(newMembers.GetXml.ToString(), Is.EqualTo(oldMembers.GetXml.ToString()));
        }

        // ----- project settings -----

        private static void AssertSameLanguage(ProjectSettings.LanguageInfo actual, ProjectSettings.LanguageInfo expected, string strMsg)
        {
            Assert.That(actual.LangType, Is.EqualTo(expected.LangType), strMsg);
            Assert.That(actual.LangName, Is.EqualTo(expected.LangName), strMsg);
            Assert.That(actual.LangCode, Is.EqualTo(expected.LangCode), strMsg);
            Assert.That(actual.DefaultFontName, Is.EqualTo(expected.DefaultFontName), strMsg);
            Assert.That(actual.DefaultFontSize, Is.EqualTo(expected.DefaultFontSize), strMsg);
            Assert.That(actual.FontToUse.Name, Is.EqualTo(expected.FontToUse.Name), strMsg);
            Assert.That(actual.FontToUse.Size, Is.EqualTo(expected.FontToUse.Size), strMsg);
            Assert.That(actual.FontColor.Name, Is.EqualTo(expected.FontColor.Name), strMsg);
            Assert.That(actual.FullStop, Is.EqualTo(expected.FullStop), strMsg);
            Assert.That(actual.DefaultKeyboard, Is.EqualTo(expected.DefaultKeyboard), strMsg);
            Assert.That(actual.KeyboardOverride, Is.EqualTo(expected.KeyboardOverride), strMsg);
            Assert.That(actual.DefaultRtl, Is.EqualTo(expected.DefaultRtl), strMsg);
            Assert.That(actual.InvertRtl, Is.EqualTo(expected.InvertRtl), strMsg);
            Assert.That(actual.HasData, Is.EqualTo(expected.HasData), strMsg);
        }

        private static void AssertSameAdaptIt(ProjectSettings.AdaptItConfiguration actual, ProjectSettings.AdaptItConfiguration expected, string strMsg)
        {
            Assert.That(actual == null, Is.EqualTo(expected == null), strMsg);
            if (expected == null)
                return;
            Assert.That(actual.ProjectType, Is.EqualTo(expected.ProjectType), strMsg);
            Assert.That(actual.BtDirection, Is.EqualTo(expected.BtDirection), strMsg);
            Assert.That(actual.ConverterName, Is.EqualTo(expected.ConverterName), strMsg);
            Assert.That(actual.ProjectFolderName, Is.EqualTo(expected.ProjectFolderName), strMsg);
            Assert.That(actual.RepoProjectName, Is.EqualTo(expected.RepoProjectName), strMsg);
            Assert.That(actual.RepositoryServer, Is.EqualTo(expected.RepositoryServer), strMsg);
            Assert.That(actual.NetworkRepositoryPath, Is.EqualTo(expected.NetworkRepositoryPath), strMsg);
            Assert.That(actual.GetXml.ToString(), Is.EqualTo(expected.GetXml.ToString()), strMsg);
        }

        private static void AssertSameShow(ShowLanguageFields actual, ShowLanguageFields expected, string strMsg)
        {
            Assert.That(actual.Vernacular, Is.EqualTo(expected.Vernacular), strMsg);
            Assert.That(actual.NationalBt, Is.EqualTo(expected.NationalBt), strMsg);
            Assert.That(actual.InternationalBt, Is.EqualTo(expected.InternationalBt), strMsg);
        }

        private static void AssertSameSettings(ProjectSettings actual, ProjectSettings expected)
        {
            AssertSameLanguage(actual.Vernacular, expected.Vernacular, "Vernacular");
            AssertSameLanguage(actual.NationalBT, expected.NationalBT, "NationalBT");
            AssertSameLanguage(actual.InternationalBT, expected.InternationalBT, "InternationalBT");
            AssertSameLanguage(actual.FreeTranslation, expected.FreeTranslation, "FreeTranslation");
            AssertSameShow(actual.ShowRetellings, expected.ShowRetellings, "ShowRetellings");
            AssertSameShow(actual.ShowTestQuestions, expected.ShowTestQuestions, "ShowTestQuestions");
            AssertSameShow(actual.ShowAnswers, expected.ShowAnswers, "ShowAnswers");
            AssertSameAdaptIt(actual.VernacularToNationalBt, expected.VernacularToNationalBt, "VernacularToNationalBt");
            AssertSameAdaptIt(actual.VernacularToInternationalBt, expected.VernacularToInternationalBt, "VernacularToInternationalBt");
            AssertSameAdaptIt(actual.NationalBtToInternationalBt, expected.NationalBtToInternationalBt, "NationalBtToInternationalBt");
            Assert.That(actual.IsConfigured, Is.EqualTo(expected.IsConfigured));
            Assert.That(actual.GetXml.ToString(), Is.EqualTo(expected.GetXml.ToString()));
            Assert.That(actual.HasAdaptItConfigurationData, Is.EqualTo(expected.HasAdaptItConfigurationData));
            if (expected.HasAdaptItConfigurationData)
                Assert.That(actual.AdaptItConfigXml.ToString(), Is.EqualTo(expected.AdaptItConfigXml.ToString()));
        }

        private void AssertSettingsMatchRowPath()
        {
            var oldSettings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            var newSettings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            oldSettings.SerializeProjectSettings(_ds);
            newSettings.SerializeProjectSettings(_root);
            AssertSameSettings(newSettings, oldSettings);
        }

        [Test]
        public void ProjectSettings_FullFixture_MatchesRowPath()
        {
            AssertSettingsMatchRowPath();

            var settings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            settings.SerializeProjectSettings(_root);
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
        public void ProjectSettings_NoLanguagesNoAdaptIt_MatchesRowPath()
        {
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members><LnCNotes /><stories SetName=\"Stories\" />"));
            AssertSettingsMatchRowPath();
        }

        [Test]
        public void ProjectSettings_NoInternationalOrFreeTranslation_ClearsTheirDefaultNames_LikeRowPath()
        {
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members>" +
                               "<Languages UseRetellingInternationalBT=\"false\">" +
                               "<LanguageInfo lang=\"Vernacular\" name=\"V\" code=\"v\" FontName=\"Arial\" FontSize=\"9\" FontColor=\"Red\" SentenceFinalPunct=\".\" />" +
                               "</Languages><LnCNotes /><stories SetName=\"Stories\" />"));
            AssertSettingsMatchRowPath();

            var settings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            settings.SerializeProjectSettings(_root);
            Assert.That(settings.InternationalBT.HasData, Is.False);
            Assert.That(settings.FreeTranslation.HasData, Is.False);
            Assert.That(settings.ShowRetellings.InternationalBt, Is.False);
        }

        [Test]
        public void ProjectSettings_TwoAdaptItConfigurationsElements_ReadsNeither_LikeRowPath()
        {
            const string strConfig = "<AdaptItConfiguration ProjectType=\"LocalAiProjectOnly\" BtDirection=\"VernacularToNationalBt\" ConverterName=\"c\" />";
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members><Languages />" +
                               "<AdaptItConfigurations>" + strConfig + "</AdaptItConfigurations>" +
                               "<AdaptItConfigurations>" + strConfig + "</AdaptItConfigurations>" +
                               "<LnCNotes /><stories SetName=\"Stories\" />"));
            AssertSettingsMatchRowPath();
        }

        [Test]
        public void AdaptItConfiguration_And_LanguageInfo_MatchRowPath_Individually()
        {
            var aiRows = _ds.AdaptItConfiguration.ToList();
            var aiElems = _root.Descendants("AdaptItConfiguration").ToList();
            Assert.That(aiElems.Count, Is.EqualTo(3));
            Assert.That(aiRows.Count, Is.EqualTo(3));
            for (int i = 0; i < aiRows.Count; i++)
            {
                var oldAi = new ProjectSettings.AdaptItConfiguration();
                var newAi = new ProjectSettings.AdaptItConfiguration();
                oldAi.SerializeFromProjectFile(aiRows[i]);
                newAi.SerializeFromProjectFile(aiElems[i]);
                AssertSameAdaptIt(newAi, oldAi, $"config {i}");
            }

            var langRows = _ds.LanguageInfo.ToList();
            var langElems = _root.Descendants("LanguageInfo").ToList();
            Assert.That(langElems.Count, Is.EqualTo(4));
            Assert.That(langRows.Count, Is.EqualTo(4));
            var settings = new ProjectSettings(Path.GetTempPath().TrimEnd('\\'), ProjectName);
            for (int i = 0; i < langRows.Count; i++)
            {
                var oldLang = new ProjectSettings.LanguageInfo(settings.Vernacular.LangType, new System.Drawing.Font("Arial", 12), System.Drawing.Color.Black);
                var newLang = new ProjectSettings.LanguageInfo(settings.Vernacular.LangType, new System.Drawing.Font("Arial", 12), System.Drawing.Color.Black);
                oldLang.Serialize(langRows[i]);
                newLang.Serialize(langElems[i]);
                AssertSameLanguage(newLang, oldLang, $"language {i}");
            }
        }

        // ----- L&C notes -----

        [Test]
        public void LnCNotesData_MatchesRowPath()
        {
            var oldNotes = new LnCNotesData(_ds);
            var newNotes = new LnCNotesData(_root);
            Assert.That(newNotes.Count, Is.EqualTo(2));
            Assert.That(newNotes.Count, Is.EqualTo(oldNotes.Count));
            Assert.That(newNotes.GetXml.ToString(), Is.EqualTo(oldNotes.GetXml.ToString()));
            for (int i = 0; i < oldNotes.Count; i++)
            {
                Assert.That(newNotes[i].Notes, Is.EqualTo(oldNotes[i].Notes), $"note {i}");
                Assert.That(newNotes[i].VernacularRendering, Is.EqualTo(oldNotes[i].VernacularRendering), $"note {i}");
                Assert.That(newNotes[i].NationalBtRendering, Is.EqualTo(oldNotes[i].NationalBtRendering), $"note {i}");
                Assert.That(newNotes[i].InternationalBtRendering, Is.EqualTo(oldNotes[i].InternationalBtRendering), $"note {i}");
            }

            // the row path reads KeyTermIds (GetXml writes the singular KeyTermId, which is why this is the thing to check)
            Assert.That((string)newNotes.GetXml.Elements("LnCNote").First().Attribute("KeyTermId"), Is.EqualTo("KT1, KT2"));
            Assert.That((string)oldNotes.GetXml.Elements("LnCNote").First().Attribute("KeyTermId"), Is.EqualTo("KT1, KT2"));
            Assert.That(newNotes[0].Notes, Is.EqualTo("note with renderings\r\nsecond line"));
            Assert.That(newNotes[0].VernacularRendering, Is.EqualTo("verb\r\nrendering"));
        }

        [Test]
        public void LnCNotesData_NoLnCNotesElement_IsEmpty_LikeRowPath()
        {
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members>"));
            var oldNotes = new LnCNotesData(_ds);
            var newNotes = new LnCNotesData(_root);
            Assert.That(newNotes.Count, Is.EqualTo(0));
            Assert.That(newNotes.GetXml.ToString(), Is.EqualTo(oldNotes.GetXml.ToString()));
        }

        [Test]
        public void LnCNote_WithoutAnyText_HasEmptyNotes_LikeRowPath()
        {
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members><LnCNotes><LnCNote guid=\"g1\" VernacularRendering=\"v\" /></LnCNotes>"));
            var oldNotes = new LnCNotesData(_ds);
            var newNotes = new LnCNotesData(_root);
            Assert.That(newNotes.Count, Is.EqualTo(1));
            Assert.That(newNotes[0].Notes, Is.EqualTo(oldNotes[0].Notes));
            Assert.That(newNotes.GetXml.ToString(), Is.EqualTo(oldNotes.GetXml.ToString()));
        }

        // ----- story sets -----

        [Test]
        public void StoriesData_MatchesRowPath_ForEachOfTheThreeSets()
        {
            var strProjectFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            var rows = _ds.stories.ToList();
            var elems = _root.Elements("stories").ToList();
            Assert.That(elems.Count, Is.EqualTo(3));
            Assert.That(rows.Count, Is.EqualTo(3));
            var expectedCounts = new[] { 2, 1, 1 };
            for (int i = 0; i < rows.Count; i++)
            {
                ProjectReader.UniqueStoryGuids.Clear();
                var oldStories = new StoriesData(rows[i], _ds, strProjectFolder);
                ProjectReader.UniqueStoryGuids.Clear();
                var newStories = new StoriesData(elems[i], strProjectFolder);
                Assert.That(newStories.SetName, Is.EqualTo(oldStories.SetName), $"set {i}");
                Assert.That(newStories.Count, Is.EqualTo(expectedCounts[i]), $"set {i}");
                Assert.That(newStories.Count, Is.EqualTo(oldStories.Count), $"set {i}");
                Assert.That(newStories.GetXml.ToString(), Is.EqualTo(oldStories.GetXml.ToString()), $"set {i}");
            }
            Assert.That(rows.Select(r => r.SetName), Is.EqualTo(new[] { "Stories", "Non-Biblical Stories", "Old Stories" }));
        }

        [Test]
        public void StoriesData_DuplicateStoryNames_AreRenamedLikeRowPath()
        {
            const string strStory = "<story name=\"Same\" stage=\"ProjFacTypeVernacular\" guid=\"{0}\" stageDateTimeStamp=\"2026-10-03T12:34:56Z\">" +
                                    "<CraftingInfo NonBiblicalStory=\"false\"><StoryCrafter memberID=\"m1\" /></CraftingInfo>" +
                                    "<Verses><Verse guid=\"v{0}\" first=\"true\" /></Verses></story>";
            LoadBoth(WriteTemp("<Members>" + CrafterMember + "</Members><Languages /><LnCNotes />" +
                               "<stories SetName=\"Stories\">" +
                               string.Format(strStory, "g1") + string.Format(strStory, "g2") + string.Format(strStory, "g3") +
                               "</stories>"));
            var strProjectFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            ProjectReader.UniqueStoryGuids.Clear();
            var oldStories = new StoriesData(_ds.stories.Single(), _ds, strProjectFolder);
            ProjectReader.UniqueStoryGuids.Clear();
            var newStories = new StoriesData(_root.Element("stories"), strProjectFolder);
            Assert.That(newStories.Select(s => s.Name), Is.EqualTo(new[] { "Same", "Same.1", "Same.2" }));
            Assert.That(newStories.Select(s => s.Name), Is.EqualTo(oldStories.Select(s => s.Name)));
            Assert.That(newStories.GetXml.ToString(), Is.EqualTo(oldStories.GetXml.ToString()));
        }

        // ----- ClearLanguageNamePlaceholders -----

        [Test]
        public void ClearLanguageNamePlaceholders_XElement_MatchesDataSetOverload()
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
            LoadBoth(WriteTemp(strProject));

            var nOld = LegacyTextRepair.ClearLanguageNamePlaceholders(_ds);
            var nNew = LegacyTextRepair.ClearLanguageNamePlaceholders(_root);
            Assert.That(nNew, Is.EqualTo(nOld));
            // Vernacular and National lines, Vernacular and International retellings, the TQ line and the National answer
            Assert.That(nNew, Is.EqualTo(6));

            AssertSameTexts(_ds.StoryLine.Select(r => r.IsStoryLine_textNull() ? null : r.StoryLine_text), _root.Descendants("StoryLine"));
            AssertSameTexts(_ds.Retelling.Select(r => r.IsRetelling_textNull() ? null : r.Retelling_text), _root.Descendants("Retelling"));
            AssertSameTexts(_ds.TestQuestionLine.Select(r => r.IsTestQuestionLine_textNull() ? null : r.TestQuestionLine_text), _root.Descendants("TestQuestionLine"));
            AssertSameTexts(_ds.Answer.Select(r => r.IsAnswer_textNull() ? null : r.Answer_text), _root.Descendants("Answer"));
            // not placeholders, whatever the text
            Assert.That(_root.Descendants("ConsultantNote").Single().Value, Is.EqualTo("Testish"));
            Assert.That(_root.Descendants("StoryLine").Select(e => e.Value),
                Is.EqualTo(new[] { "", "", "Testish", "English" }));
        }

        // the DataSet leaves "" where it cleared a value; the element has no text at all then (which reads as null)
        private static void AssertSameTexts(IEnumerable<string> dataSetValues, IEnumerable<XElement> elems)
        {
            var aOld = dataSetValues.ToList();
            var aNew = elems.Select(XmlRead.Text).ToList();
            Assert.That(aNew.Count, Is.EqualTo(aOld.Count));
            for (int i = 0; i < aOld.Count; i++)
                Assert.That(string.IsNullOrEmpty(aNew[i]) ? null : aNew[i],
                            Is.EqualTo(string.IsNullOrEmpty(aOld[i]) ? null : aOld[i]), $"value {i}");
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

        private static void AssertSameProjectData(StoryProjectData actual, StoryProjectData expected)
        {
            Assert.That(actual.Keys.Cast<string>(), Is.EqualTo(expected.Keys.Cast<string>()));
            Assert.That(actual.PanoramaFrontMatter, Is.EqualTo(expected.PanoramaFrontMatter));
            Assert.That(actual.ProjSettings.ProjectName, Is.EqualTo(expected.ProjSettings.ProjectName));
            Assert.That(actual.ProjSettings.UseDropbox, Is.EqualTo(expected.ProjSettings.UseDropbox));
            Assert.That(actual.ProjSettings.DropboxStory, Is.EqualTo(expected.ProjSettings.DropboxStory));
            Assert.That(actual.ProjSettings.DropboxRetelling, Is.EqualTo(expected.ProjSettings.DropboxRetelling));
            Assert.That(actual.ProjSettings.DropboxAnswers, Is.EqualTo(expected.ProjSettings.DropboxAnswers));
            Assert.That(actual.OsMetaData == null, Is.EqualTo(expected.OsMetaData == null));
            AssertSameSettings(actual.ProjSettings, expected.ProjSettings);
            Assert.That(actual.GetXml.ToString(), Is.EqualTo(expected.GetXml.ToString()));
        }

        private void AssertProjectDataMatchesRowPath(string strXml, out StoryProjectData newProject)
        {
            var strFolder = MakeProjectFolder(strXml);
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");

            ProjectReader.ReadProjectFile(strPath, out var ds);
            var oldProject = new StoryProjectData(ds, new ProjectSettings(strFolder, ProjectName));

            var contents = ProjectFile.Load(strPath);
            newProject = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, ProjectName));

            AssertSameProjectData(newProject, oldProject);
        }

        [Test]
        public void StoryProjectData_FullFixture_MatchesRowPath()
        {
            AssertProjectDataMatchesRowPath(File.ReadAllText(FixturePath), out var project);

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
        public void StoryProjectData_OverwritesProjectNameFromSettings_LikeRowPath()
        {
            var strFolder = MakeProjectFolder(File.ReadAllText(FixturePath));
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");
            ProjectReader.ReadProjectFile(strPath, out var ds);
            var oldProject = new StoryProjectData(ds, new ProjectSettings(strFolder, "renamed"));
            var contents = ProjectFile.Load(strPath);
            var newProject = new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, "renamed"));
            Assert.That(newProject.GetXml.Attribute("ProjectName").Value, Is.EqualTo("renamed"));
            Assert.That(newProject.GetXml.ToString(), Is.EqualTo(oldProject.GetXml.ToString()));
        }

        [Test]
        public void StoryProjectData_MarkedPlain_IsNotDecoded_LikeRowPath()
        {
            var strXml = File.ReadAllText(FixturePath).Replace("<StoryProject version=\"1.8\"",
                "<StoryProject TextEncoding=\"plain\" version=\"1.8\"");
            AssertProjectDataMatchesRowPath(strXml, out var project);
            Assert.That(project.GetXml.ToString(), Does.Contain("first &amp;amp; line"));
        }

        [TestCase(0, new[] { "Stories", "Old Stories" })]      // none: two sets are added
        [TestCase(1, new[] { "Stories" })]                      // one: nothing added
        public void StoryProjectData_StorySetAdditions_MatchRowPath(int nSetsToKeep, string[] expectedKeys)
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            var sets = doc.Root.Elements("stories").ToList();
            for (int i = nSetsToKeep; i < sets.Count; i++)
                sets[i].Remove();
            AssertProjectDataMatchesRowPath(doc.ToString(), out var project);
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(expectedKeys));
        }

        [Test]
        public void StoryProjectData_TwoSetsWithoutNonBiblical_AddsTheNonBiblicalSet_LikeRowPath()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Elements("stories").Single(s => (string)s.Attribute("SetName") == "Non-Biblical Stories").Remove();
            AssertProjectDataMatchesRowPath(doc.ToString(), out var project);
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(new[] { "Stories", "Old Stories", "Non-Biblical Stories" }));
        }

        [Test]
        public void StoryProjectData_TwoSetsIncludingNonBiblical_AddsNothing_LikeRowPath()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Elements("stories").Single(s => (string)s.Attribute("SetName") == "Old Stories").Remove();
            AssertProjectDataMatchesRowPath(doc.ToString(), out var project);
            Assert.That(project.Keys.Cast<string>(), Is.EqualTo(new[] { "Stories", "Non-Biblical Stories" }));
        }

        [Test]
        public void StoryProjectData_NoDropboxAttributesAndEmptyFrontMatter_UseDefaults_LikeRowPath()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            foreach (var strName in new[] { "UseDropbox", "DropboxStory", "DropboxRetellings", "DropboxAnswers" })
                doc.Root.Attribute(strName).Remove();
            doc.Root.SetAttributeValue("PanoramaFrontMatter", "");
            AssertProjectDataMatchesRowPath(doc.ToString(), out var project);
            Assert.That(project.ProjSettings.UseDropbox, Is.False);
            Assert.That(project.PanoramaFrontMatter, Is.EqualTo(Properties.Resources.IDS_DefaultPanoramaFrontMatter));
        }

        [Test]
        public void StoryProjectData_MissingPanoramaFrontMatter_BothPathsThrow()
        {
            var doc = XDocument.Parse(File.ReadAllText(FixturePath));
            doc.Root.Attribute("PanoramaFrontMatter").Remove();
            var strFolder = MakeProjectFolder(doc.ToString());
            var strPath = Path.Combine(strFolder, ProjectName + ".onestory");

            Exception exOld = null;
            try
            {
                ProjectReader.ReadProjectFile(strPath, out var ds);
                new StoryProjectData(ds, new ProjectSettings(strFolder, ProjectName));
            }
            catch (Exception ex)
            {
                exOld = ex;
            }
            TestContext.WriteLine($"row path threw {exOld?.GetType().Name}: {exOld?.Message}");
            Assert.That(exOld, Is.Not.Null);

            var contents = ProjectFile.Load(strPath);
            Assert.Throws<ApplicationException>(() =>
                new StoryProjectData(contents.Root, contents.IsPlainTextEncoded, new ProjectSettings(strFolder, ProjectName)));
        }
    }
}
