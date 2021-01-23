#define AiChorusInOseFolder

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Chorus.sync;

namespace AiChorus
{
    public class OseSyncHandler : ApplicationSyncHandler
    {
        public const string CstrStoryEditorExe = "StoryEditor.exe";
        private const string CstrCantOpenOse = "Can't Open OneStory Editor";

        private Assembly _theStoryEditor;
#if !AiChorusInOseFolder
        private MethodInfo _methodSyncWithRepository;
#endif

        public OseSyncHandler(Project project, ServerSetting serverSetting)
            : base(project, serverSetting)
        {
            
        }

        public static string OseRunningPath;
        private static string _appDataRoot;
        public override string AppDataRoot
        {
            get 
            {
                if (_appDataRoot == null)
                {
#if AiChorusInOseFolder
                    var strStoryEditorPath = OneStoryEditorExePath;
#else
                    // first see if OSE is installed
                    // check in Program Files\SIL\OneStory Editor 2.0
                    string strStoryEditorExePath =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                     Path.Combine("SIL", Path.Combine("OneStory Editor 2.0", "StoryEditor.exe")));

                    if (!File.Exists(strStoryEditorExePath))
                    {
                        MessageBox.Show("Unable to find OneStory Editor! Is it installed?", Properties.Resources.AiChorusCaption);
                        return "Can't Find OneStory Editor";
                    }
#endif

                    // if it is there, then get the path to the project folder root
                    _theStoryEditor = Assembly.LoadFile(strStoryEditorPath);
                    if (_theStoryEditor == null)
                        return CstrCantOpenOse;

                    var typeOseProjectSettings = _theStoryEditor.GetType("OneStoryProjectEditor.ProjectSettings");
                    if (typeOseProjectSettings == null)
                        return CstrCantOpenOse;

                    var typeString = typeof (string);
#if !AiChorusInOseFolder
                    var aTypeParams = new Type[] { typeString, typeString, typeString, typeString, typeString, typeString };
                    _methodSyncWithRepository = typeOseProjectSettings.GetMethod("SyncWithRepository", aTypeParams);
#endif
                    var propOneStoryProjectFolderRoot = typeOseProjectSettings.GetProperty("OneStoryProjectFolderRoot");
                    _appDataRoot = (string)propOneStoryProjectFolderRoot.GetValue(typeOseProjectSettings, null);
                }

                return _appDataRoot;
            }
        }

        private static readonly List<string> _lstProjectsJustCloned = new List<string>();
        protected override string GetSynchronizeOrOpenProjectLable
        {
            get
            {
                return (_lstProjectsJustCloned.Contains(Project.FolderName))
                           ? CstrOptionOpenProject
                           : base.GetSynchronizeOrOpenProjectLable;
            }
        }

        public override void DoSynchronize()
        {
#if AiChorusInOseFolder
            Program.LaunchProgram(OseRunningPath, ProjectFileSpec);
#else
            /* string strProjectFolder, string strProjectName, string strHgRepoUrlHost,
            string strUsername, string strPassword*/
            var oParams = new object[]
                              {
                                  Path.Combine(AppDataRoot, Project.FolderName), 
                                  Project.ProjectId,
                                  "http://resumable.languagedepot.org",    // for now
                                  ServerSetting.Username, 
                                  ServerSetting.Password, 
                                  null                                      // shared network path
                              };
            _methodSyncWithRepository.Invoke(_theStoryEditor, oParams);
#endif
        }

        private static List<string> _lstStorySetsToIgnore = new List<string> { "Non-Biblical Stories", "Old Stories" };

        internal void HarvestProjectData(Dictionary<string, List<OseProjectData>> mapProjectsToProjectData)
        {
            // get the path to the project file
            var projectFileSpec = ProjectFileSpec;
            if (!File.Exists(projectFileSpec))
                throw new ApplicationException($"can't find project file '{projectFileSpec}'. Is it being sync'd on this machine?");

            var doc = XDocument.Load(projectFileSpec);
            var storySets = doc.Root.Descendants("stories").Where(ss => !_lstStorySetsToIgnore.Contains(ss.Attribute("SetName").Value))
                                    .Descendants("story");

            var storyInfos = new List<OseProjectData>();
            foreach (var story in storySets)
            {
                var lastStateTransition = story.Descendants("StateTransition")?.LastOrDefault();
                var craftingInfo = story.Descendants("CraftingInfo").FirstOrDefault();
                var memberInfo = doc.Root.Descendants("Member");
                XAttribute visible;
                var verses = story.Descendants("Verse").Where(v => ((v.Attribute("first") == null) &&
                                                                   (((visible = v.Attribute("visible")) == null) || (visible.Value == "true"))))
                                                       .ToList();

                var vernacularLanguageInfo = doc.Root.Descendants("LanguageInfo").FirstOrDefault(l => l.Attribute("lang").Value == "Vernacular");

                var storyInfo = new OseProjectData
                {
                    { OseProjectData.KeyProjectName, Project.ProjectId },
                    { OseProjectData.KeyStoryLanguageName, vernacularLanguageInfo?.Attribute("name").Value },
                    { OseProjectData.KeyStorySetName, story.Parent.Attribute("SetName").Value },
                    { OseProjectData.KeyStoryName, story.Attribute("name").Value },
                    { OseProjectData.KeyStoryPurpose, craftingInfo?.Descendants("StoryPurpose")?.FirstOrDefault()?.Value },
                    { OseProjectData.KeyLastEditor, MemberLookup(memberInfo, lastStateTransition?.Attribute("LoggedInMemberId")?.Value, showRoleAlso: true) },
                    { OseProjectData.KeyTimeInStage, CalculateTimeInStage(lastStateTransition, story) },
                    { OseProjectData.KeyStoryProjectFacilitator, MemberLookup(memberInfo, craftingInfo?.Descendants("ProjectFacilitator")?.FirstOrDefault()?.Attribute("memberID")?.Value) },
                    { OseProjectData.KeyStoryConsultant, MemberLookup(memberInfo, craftingInfo?.Descendants("Consultant")?.FirstOrDefault()?.Attribute("memberID")?.Value) },
                    { OseProjectData.KeyStoryCoach, MemberLookup(memberInfo, craftingInfo?.Descendants("Coach")?.FirstOrDefault()?.Attribute("memberID")?.Value) },
                    { OseProjectData.KeyStoryCrafter, MemberLookup(memberInfo, craftingInfo?.Descendants("StoryCrafter")?.FirstOrDefault()?.Attribute("memberID")?.Value) },
                    { OseProjectData.KeyNumberOfLines, verses.Count.ToString() },
                    { OseProjectData.KeyNumberOfTestQuestions, verses.Sum(v => v.Descendants("TestQuestion").Count()).ToString() },
                    { OseProjectData.KeyNumberOfWords, CalculateWordCount(verses, vernacularLanguageInfo).ToString() },
                };

                storyInfo.Add(OseProjectData.KeyCurrentEditor, storyInfo.EditorFromState(story.Attribute("stage").Value));

                Console.WriteLine($"In project {Project.ProjectId}, found story info: {storyInfo}");
                storyInfos.Add(storyInfo);
            }

            mapProjectsToProjectData.Add(Project.ProjectId, storyInfos);
        }

        private int CalculateWordCount(List<XElement> verses, XElement vernacularLanguageInfo)
        {
            var storyLines = verses.Select(v => v.Descendants("StoryLine").FirstOrDefault(l => l.Attribute("lang").Value == "Vernacular")?.Value)
                                   .ToList();
            
            var charsToIgnore = (vernacularLanguageInfo?.Attribute("SentenceFinalPunct")?.Value + " ‘‛“‟’”„,\"\'").ToCharArray();

            var wordCount = storyLines.Where(l => !String.IsNullOrWhiteSpace(l))
                                      .Sum(l => l.Split(charsToIgnore, StringSplitOptions.RemoveEmptyEntries).Length);
            return wordCount;
        }

        private string CalculateTimeInStage(XElement lastStateTransition, XElement story)
        {
            var dateTimeLastChange =  DateTime.Parse(lastStateTransition?.Attribute("TransitionDateTime")?.Value ??
                                                     story.Attribute("stageDateTimeStamp")?.Value ?? 
                                                     DateTime.MinValue.ToString());

            var ts = DateTime.Now - dateTimeLastChange;
            string strTimeInState = "";
            if (ts.Days > 0)
                strTimeInState += String.Format("{0} days, ", ts.Days);
            if (ts.Hours > 0)
                strTimeInState += String.Format("{0} hours, ", ts.Hours);
            strTimeInState += String.Format("{0} minutes", ts.Minutes);
            return strTimeInState;
        }

        private string MemberLookup(IEnumerable<XElement> memberInfo, string value, bool showRoleAlso = false)
        {
            return MemberLookup(memberInfo?.FirstOrDefault(m => m.Attribute("memberKey").Value == value), showRoleAlso);
        }

        private string MemberLookup(XElement xElement, bool showRoleAlso)
        {
            var str = $"{xElement?.Attribute("name")?.Value}";
            if (showRoleAlso)
                str += $" ({xElement?.Attribute("memberType")?.Value})";
            return str;
        }

        private string ProjectFileSpec
        {
            get
            {
                return Path.Combine(Path.Combine(AppDataRoot, Project.FolderName),
                                    Project.ProjectId + ".onestory");
            }
        }

        public override ProjectFolderConfiguration GetProjectFolderConfiguration(string strProjectFolder)
        {
            var projectConfig = new ProjectFolderConfiguration(strProjectFolder);
            projectConfig.IncludePatterns.Add("*.onestory");
            projectConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
            projectConfig.IncludePatterns.Add("*.bad"); // if we write a bad file, commit that as well
            projectConfig.IncludePatterns.Add("*.conflict"); // include the conflicts file as well so we can fix them
            projectConfig.IncludePatterns.Add("*.ChorusNotes"); // the new conflict file
            return projectConfig;
        }

        internal override bool DoClone()
        {
            if (base.DoClone())
            {
                _lstProjectsJustCloned.Add(Project.FolderName);
                return true;
            }
            return false;
        }

        public override void DoProjectOpen()
        {
            // for us, this is the same as Synchronize
            DoSynchronize();
        }
    }
}
