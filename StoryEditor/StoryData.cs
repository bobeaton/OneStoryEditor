using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Xsl;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public class StoryData
    {
        public string Name;
        public string guid;
        public DateTime StageTimeStamp;
        public StoryStageLogic ProjStage;
        public CraftingInfoData CraftingInfo;
        public StoryStateTransitionHistory TransitionHistory;
        public VersesData Verses;
        public TasksPf.TaskSettings TasksAllowedPf;
        public TasksPf.TaskSettings TasksRequiredPf;
        public TasksCit.TaskSettings TasksAllowedCit;
        public TasksCit.TaskSettings TasksRequiredCit;
        public int CountRetellingsTests;
        public int CountTestingQuestionTests;

        public StoryData(string strStoryName, string strCrafterMemberGuid, 
            string strLoggedOnMemberGuid, bool bIsBiblicalStory, 
            StoryProjectData projectData)
        {
            Name = strStoryName;
            guid = Guid.NewGuid().ToString();
            StageTimeStamp = DateTime.Now;
            ProjStage = new StoryStageLogic(projectData.ProjSettings);
            CraftingInfo = new CraftingInfoData(strCrafterMemberGuid, strLoggedOnMemberGuid, bIsBiblicalStory);
            TransitionHistory = new StoryStateTransitionHistory();
            Verses = new VersesData();
            Verses.InsureFirstVerse();

            string strMemberName = projectData.TeamMembers.GetNameFromMemberId(strLoggedOnMemberGuid);
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(projectData.TeamMembers[strMemberName].MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));

            TeamMemberData thePf = projectData.TeamMembers[strMemberName];
            TasksAllowedPf = (TasksPf.TaskSettings)thePf.DefaultAllowed;
            TasksRequiredPf = (TasksPf.TaskSettings)thePf.DefaultRequired;
            TasksAllowedCit = TasksCit.DefaultAllowed;
            TasksRequiredCit = TasksCit.DefaultRequired;

            // set the attributes that say how many tests are required
            if (TasksPf.IsTaskOn(TasksRequiredPf, TasksPf.TaskSettings.Retellings))
                CountRetellingsTests = 1;
            if (TasksPf.IsTaskOn(TasksRequiredPf, TasksPf.TaskSettings.Retellings2))
                CountRetellingsTests = 2;
            if (TasksPf.IsTaskOn(TasksRequiredPf, TasksPf.TaskSettings.Answers))
                CountTestingQuestionTests = 1;
            if (TasksPf.IsTaskOn(TasksRequiredPf, TasksPf.TaskSettings.Answers2))
                CountTestingQuestionTests = 2;
        }

        public StoryData(XmlNode node, string strProjectFolder)
        {
            XmlAttribute attr;
            Name = ((attr = node.Attributes[CstrAttributeName]) != null) ? attr.Value : null;
            TasksAllowedPf = GetAttributeValue(node, CstrAttributeLabelTasksAllowedPf, TasksPf.DefaultAllowed);
            TasksRequiredPf = GetAttributeValue(node, CstrAttributeLabelTasksRequiredPf, TasksPf.DefaultRequired);
            TasksAllowedCit = GetAttributeValue(node, CstrAttributeLabelTasksAllowedCit, TasksCit.DefaultAllowed);
            TasksRequiredCit = GetAttributeValue(node, CstrAttributeLabelTasksRequiredCit, TasksCit.DefaultRequired);

            CountRetellingsTests = ((attr = node.Attributes[CstrAttributeLabelCountRetellingsTests]) != null)
                                       ? Convert.ToInt32(attr.Value)
                                       : 0;
            CountTestingQuestionTests = ((attr = node.Attributes[CstrAttributeLabelCountTestingQuestionTests]) != null)
                                       ? Convert.ToInt32(attr.Value)
                                       : 0;

            // the last param isn't really false, but this ctor is only called when that doesn't matter
            //  (during Chorus diff presentation)
            ProjStage = new StoryStageLogic(strProjectFolder, node.Attributes[CstrAttributeStage].Value);
            guid = node.Attributes[CstrAttributeGuid].Value;
            StageTimeStamp = DateTime.Parse(node.Attributes[CstrAttributeTimeStamp].Value);
            CraftingInfo = new CraftingInfoData(node.SelectSingleNode(CraftingInfoData.CstrElementLabelCraftingInfo));
            TransitionHistory = new StoryStateTransitionHistory(node.SelectSingleNode(StoryStateTransitionHistory.CstrElementLabelTransitionHistory));
            Verses = new VersesData(node.SelectSingleNode(VersesData.CstrElementLabelVerses));
        }

        public StoryData(NewDataSet.storyRow theStoryRow, NewDataSet projFile, string strProjectFolder)
        {
            Name = theStoryRow.name;
            TasksAllowedPf = (!theStoryRow.IsTasksAllowedPfNull())
                                 ? (TasksPf.TaskSettings)
                                   Enum.Parse(typeof (TasksPf.TaskSettings), theStoryRow.TasksAllowedPf)
                                 : TasksPf.DefaultAllowed;
            TasksRequiredPf = (!theStoryRow.IsTasksRequiredPfNull())
                                  ? (TasksPf.TaskSettings)
                                    Enum.Parse(typeof (TasksPf.TaskSettings), theStoryRow.TasksRequiredPf)
                                  : TasksPf.DefaultRequired;
            TasksAllowedCit = (!theStoryRow.IsTasksAllowedCitNull())
                                  ? (TasksCit.TaskSettings)
                                    Enum.Parse(typeof (TasksCit.TaskSettings), theStoryRow.TasksAllowedCit)
                                  : TasksCit.DefaultAllowed;
            TasksRequiredCit = (!theStoryRow.IsTasksRequiredCitNull())
                                   ? (TasksCit.TaskSettings)
                                     Enum.Parse(typeof (TasksCit.TaskSettings), theStoryRow.TasksRequiredCit)
                                   : TasksCit.DefaultRequired;
            CountRetellingsTests = (!theStoryRow.IsCountRetellingsTestsNull()) 
                                            ? theStoryRow.CountRetellingsTests 
                                            : 0;
            CountTestingQuestionTests = (!theStoryRow.IsCountTestingQuestionTestsNull())
                                            ? theStoryRow.CountTestingQuestionTests
                                            : 0;

            guid = theStoryRow.guid;
            StageTimeStamp = (theStoryRow.IsstageDateTimeStampNull()) ? DateTime.Now : theStoryRow.stageDateTimeStamp;
            ProjStage = new StoryStageLogic(strProjectFolder, theStoryRow.stage);
            CraftingInfo = new CraftingInfoData(theStoryRow);
            TransitionHistory = new StoryStateTransitionHistory(theStoryRow);
            Verses = new VersesData(theStoryRow, projFile);
        }

        public StoryData(StoryData rhs)
        {
            Name = rhs.Name;

            TasksAllowedPf = rhs.TasksAllowedPf;
            TasksRequiredPf = rhs.TasksRequiredPf;
            TasksAllowedCit = rhs.TasksAllowedCit;
            TasksRequiredCit = rhs.TasksRequiredCit;
            CountRetellingsTests = rhs.CountRetellingsTests;
            CountTestingQuestionTests = rhs.CountTestingQuestionTests;

            // the guid shouldn't be replicated
            guid = Guid.NewGuid().ToString();  // rhs.guid;

            StageTimeStamp = rhs.StageTimeStamp;
            ProjStage = new StoryStageLogic(rhs.ProjStage);
            CraftingInfo = new CraftingInfoData(rhs.CraftingInfo);
            TransitionHistory = new StoryStateTransitionHistory();  // start from scratch
            Verses = new VersesData(rhs.Verses);
        }

        protected const string CstrAttributeName = "name";
        protected const string CstrAttributeStage = "stage";
        protected const string CstrAttributeGuid = "guid";
        public const string CstrAttributeTimeStamp = "stageDateTimeStamp";
        public const string CstrAttributeLabelTasksAllowedPf = "TasksAllowedPf";
        public const string CstrAttributeLabelTasksRequiredPf = "TasksRequiredPf";
        public const string CstrAttributeLabelTasksAllowedCit = "TasksAllowedCit";
        public const string CstrAttributeLabelTasksRequiredCit = "TasksRequiredCit";
        public const string CstrAttributeLabelCountRetellingsTests = "CountRetellingsTests";
        public const string CstrAttributeLabelCountTestingQuestionTests = "CountTestingQuestionTests";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ProjStage.ProjectStage.ToString())
                                                && !String.IsNullOrEmpty(guid));

                var elemStory = new XElement("story",
                                             new XAttribute(CstrAttributeName, Name),
                                             new XAttribute(CstrAttributeStage, ProjStage.ToString()),
                                             new XAttribute(CstrAttributeLabelTasksAllowedPf,
                                                            TasksAllowedPf),
                                             new XAttribute(CstrAttributeLabelTasksRequiredPf,
                                                            TasksRequiredPf),
                                             new XAttribute(CstrAttributeLabelTasksAllowedCit,
                                                            TasksAllowedCit),
                                             new XAttribute(CstrAttributeLabelTasksRequiredCit,
                                                            TasksRequiredCit),
                                             new XAttribute(CstrAttributeLabelCountRetellingsTests,
                                                            CountRetellingsTests),
                                             new XAttribute(CstrAttributeLabelCountTestingQuestionTests,
                                                            CountTestingQuestionTests),
                                             new XAttribute(CstrAttributeGuid, guid),
                                             new XAttribute(CstrAttributeTimeStamp, StageTimeStamp.ToString("s")),
                                             CraftingInfo.GetXml);

                if (TransitionHistory.HasData)
                    elemStory.Add(TransitionHistory.GetXml);

                if (Verses.HasData)
                        elemStory.Add(Verses.GetXml);

                return elemStory;
            }
        }

        public void ChangeRetellingTestor(int nTestIndex, string strNewGuid, 
            ref TestInfo testInfoNew)
        {
            // first see if UNS being "added" is already there (i.e. it's just a change
            //  of index)
            if (ChangeTestor(CraftingInfo.TestorsToCommentsRetellings, 
                    strNewGuid, nTestIndex, ref testInfoNew))
            {
                Verses.ChangeRetellingTestorGuid(nTestIndex, strNewGuid);
            }
        }

        public void ChangeTqAnswersTestor(int nTestIndex, string strNewGuid,
            ref TestInfo testInfoNew)
        {
            // first see if UNS being "added" is already there (i.e. it's just a change
            //  of index)
            if (ChangeTestor(CraftingInfo.TestorsToCommentsTqAnswers,
                    strNewGuid, nTestIndex, ref testInfoNew))
            {
                Verses.ChangeTqAnswersTestorGuid(nTestIndex, strNewGuid);
            }
        }

        private static bool ChangeTestor(TestInfo testInfo, string strNewGuid, 
            int nTestIndex, ref TestInfo testInfoNew)
        {
            bool bRet = false;
            TestorInfo testorInfo;
            if (testInfo.Count > nTestIndex)
            {
                var oldTestorInfo = testInfo[nTestIndex];
                if (oldTestorInfo.TestorGuid != strNewGuid)
                {
                    testorInfo = new TestorInfo(strNewGuid, oldTestorInfo.TestComment);
                    bRet = true;
                }
                else
                {
                    testorInfo = oldTestorInfo;
                }
            }
            else
            {
                // default comment for both
                string strComment = String.Format(Properties.Resources.IDS_InferenceCommentFormat,
                                                  DateTime.Now.ToString("yyyy-MMM-dd"));
                testorInfo = new TestorInfo(strNewGuid, strComment);
            }

            testInfoNew.Insert(nTestIndex, testorInfo);
            return bRet;
        }

        /// <summary>
        /// Returns the number of (unhidden) lines in the story
        /// </summary>
        public int NumOfLines
        {
            get { return Verses.NumOfLines; }
        }

        public int NumOfTestQuestions
        {
            get
            {
                return Verses.Where(aVerse => aVerse.IsVisible).Sum(aVerse => aVerse.TestQuestions.Count);
            }
        }

        public string NumOfWords(ProjectSettings projSettings)
        {
            return Verses.NumOfWords(projSettings);
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            Verses.IndexSearch(findProperties, ref lstBoxesToSearch);
        }

        public void CheckForProjectFacilitator(StoryProjectData storyProjectData, TeamMemberData loggedOnMember)
        {
            if (CraftingInfo.ProjectFacilitatorMemberID != null)
                return;

            // this means that we've opened a file which didn't have the proj fac listed
            // if there's only one PF, then just put that one in. If there are multiple,
            //  then ask which one to use
            if ((storyProjectData.TeamMembers.CountOfProjectFacilitator == 1)
                && TeamMemberData.IsUser(loggedOnMember.MemberType,
                                         TeamMemberData.UserTypes.ProjectFacilitator))
            {
                CraftingInfo.ProjectFacilitatorMemberID = loggedOnMember.MemberGuid;
                return;
            }
#if !DataDllBuild
            // fall thru means either the logged in person isn't a PF or there are multiple,
            //  so, ask the user to tell which PF added this story
            var dlg = new MemberPicker(storyProjectData, TeamMemberData.UserTypes.ProjectFacilitator)
                          {
                              Text = Properties.Resources.IDS_ChooseProjFac
                          };
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            CraftingInfo.ProjectFacilitatorMemberID = dlg.SelectedMember.MemberGuid;
#endif
        }

        private static TasksPf.TaskSettings GetAttributeValue(XmlNode node, 
            string cstrAttributeLabel, TasksPf.TaskSettings defaultValue)
        {
            if (node.Attributes != null)
            {
                var attr = node.Attributes[cstrAttributeLabel];
                if (attr != null)
                    return (TasksPf.TaskSettings)Enum.Parse(typeof(TasksPf.TaskSettings), attr.Value);
            }
            return defaultValue;
        }

        private static TasksCit.TaskSettings GetAttributeValue(XmlNode node, 
            string cstrAttributeLabel, TasksCit.TaskSettings defaultValue)
        {
            if (node.Attributes != null)
            {
                var attr = node.Attributes[cstrAttributeLabel];
                if (attr != null)
                    return (TasksCit.TaskSettings)Enum.Parse(typeof(TasksCit.TaskSettings), attr.Value);
            }
            return defaultValue;
        }

        // remotely accessible one from Chorus
        public string GetPresentationHtmlForChorus(XmlNode nodeProjectFile, string strProjectPath, XmlNode parentStory, XmlNode childStory)
        {
            ProjectSettings projSettings = new ProjectSettings(nodeProjectFile, strProjectPath);
            TeamMembersData teamMembers = new TeamMembersData(nodeProjectFile);
            StoryData ParentStory = null, ChildStory = null;
            if (parentStory != null)
                ParentStory = new StoryData(parentStory, strProjectPath);
            if (childStory != null)
                ChildStory = new StoryData(childStory, strProjectPath);
            var viewSettings = new VerseData.ViewSettings(
                projSettings, 
                true,   // vernacular
                true,   // national bt
                true,   // english bt
                true,   // free translation
                true,   // anchors 
                true,   // exegetical/cultural notes
                true,   // testing questions
                true,   // answers
                true,   // retellings
                false,  // theSE.viewConsultantNoteFieldMenuItem.Checked,
                false,  // theSE.viewCoachNotesFieldMenuItem.Checked,
                false,  // theSE.viewNetBibleMenuItem.Checked
                true,   // story front matter
                false,  // hidden matter
                false,  // only open conversations
                true,   // General Testing questions
                null,
                null);

            string strHtml = null;
            if (ParentStory != null)
                strHtml = ParentStory.PresentationHtml(viewSettings,
                    projSettings, teamMembers, ChildStory);
            else if (ChildStory != null)
                strHtml = ChildStory.PresentationHtml(viewSettings,
                    projSettings, teamMembers, null);
            return strHtml;
        }

        public string PresentationHtml(VerseData.ViewSettings viewSettings, ProjectSettings projSettings, TeamMembersData teamMembers, StoryData child)
        {
            Rainbow.HtmlDiffEngine.Added.BeginTag = "<span style=\"text-decoration: underline; color: orange\">"; 
            string strHtml = PresentationHtmlWithoutHtmlDocOutside(viewSettings, projSettings, teamMembers, child);
            return AddHtmlHtmlDocOutside(strHtml, projSettings);
        }

        public static string AddHtmlHtmlDocOutside(string strHtmlInside, ProjectSettings projSettings)
        {
            return String.Format(OseResources.Properties.Resources.HTML_HeaderPresentation,
                                 StylePrefix(projSettings),
                                 OseResources.Properties.Resources.HTML_DOM_PrefixPresentation,
                                 strHtmlInside);
        }

        public string PresentationHtmlWithoutHtmlDocOutside(VerseData.ViewSettings viewSettings, 
            ProjectSettings projSettings, TeamMembersData teamMembers, StoryData child)
        {
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            bool bShowNationalBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField);
            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField);
            bool bShowFreeTranslation = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField);

            int nNumCols = 0;
            if (bShowVernacular) nNumCols++;
            if (bShowNationalBT) nNumCols++;
            if (bShowEnglishBT) nNumCols++;
            if (bShowFreeTranslation) nNumCols++;

            string strHtml = null;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryFrontMatter))
            {
                strHtml += PresentationHtmlRow("Story Name", Name, (child != null) ? child.Name : null);

                // for stage, it's a bit more complicated
                StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[ProjStage.ProjectStage];
                string strParentStage = st.StageDisplayString;
                string strChildStage = null;
                if (child != null)
                {
                    st = StoryStageLogic.stateTransitions[child.ProjStage.ProjectStage];
                    strChildStage = st.StageDisplayString;
                }
                strHtml += PresentationHtmlRow("Story Stage", strParentStage, strChildStage);

                strHtml += CraftingInfo.PresentationHtml(teamMembers, (child != null) ? child.CraftingInfo : null);
            }

            // make a sub-table out of all this
            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strHtml)));

            // occasionally, we just want to print the header stuff (without lines)
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBTLangField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.EnglishBTField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.RetellingFields)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                strHtml += Verses.PresentationHtml((child != null) ? child.CraftingInfo : CraftingInfo,
                                                   (child != null) ? child.Verses : null, nNumCols, viewSettings,
                                                   teamMembers.HasOutsideEnglishBTer);
            }
            else
            {
                strHtml += String.Format("<p>{0}</p>", Environment.NewLine);
            }

            return strHtml;
        }

        protected string PresentationHtmlRow(string strLabel, string strParentValue, string strChildValue)
        {
            string strName;
            if (String.IsNullOrEmpty(strChildValue))
                strName = strParentValue;
            else
                strName = Diff.HtmlDiff(strParentValue, strChildValue, true);

            return String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                                             strLabel),
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                             100,
                                                             String.Format(
                                                                 OseResources.Properties.Resources.HTML_ParagraphText,
                                                                 strLabel,
                                                                 CstrLangInternationalBtStyleClassName,
                                                                 strName))));
        }

        public const string CstrLangVernacularStyleClassName = "LangVernacular";
        public const string CstrLangNationalBtStyleClassName = "LangNationalBT";
        public const string CstrLangInternationalBtStyleClassName = "LangInternationalBT";
        public const string CstrLangFreeTranslationStyleClassName = "LangFreeTranslation";

        public static string StylePrefix(ProjectSettings projSettings)
        {
            string strLangStyles = null;
            if (projSettings.Vernacular.HasData)
                strLangStyles += projSettings.Vernacular.HtmlStyle(CstrLangVernacularStyleClassName);
            if (projSettings.NationalBT.HasData)
                strLangStyles += projSettings.NationalBT.HtmlStyle(CstrLangNationalBtStyleClassName);
            if (projSettings.InternationalBT.HasData)
                strLangStyles += projSettings.InternationalBT.HtmlStyle(CstrLangInternationalBtStyleClassName);
            if (projSettings.FreeTranslation.HasData)
                strLangStyles += projSettings.FreeTranslation.HtmlStyle(CstrLangFreeTranslationStyleClassName);

            return String.Format(OseResources.Properties.Resources.HTML_StyleDefinition,
                                 Properties.Settings.Default.ConNoteTableFontSize,
                                 Properties.Settings.Default.ConNoteButtonFontSize,
                                 strLangStyles);
        }

        public string ConsultantNotesHtml(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, ProjectSettings projSettings, 
            TeamMemberData LoggedOnMember,
            bool bViewHidden, bool bShowOnlyOpenConversations)
        {
            string strHtml = Verses.ConsultantNotesHtml(htmlConNoteCtrl, theStoryStage,
                LoggedOnMember, bViewHidden, bShowOnlyOpenConversations);
            return String.Format(OseResources.Properties.Resources.HTML_Header,
                StylePrefix(projSettings),
                OseResources.Properties.Resources.HTML_DOM_Prefix, 
                strHtml);
        }

        public string CoachNotesHtml(object htmlConNoteCtrl, 
            StoryStageLogic theStoryStage, ProjectSettings projSettings, 
            TeamMemberData LoggedOnMember,
            bool bViewHidden, bool bShowOnlyOpenConversations)
        {
            string strHtml = Verses.CoachNotesHtml(htmlConNoteCtrl, theStoryStage,
                LoggedOnMember, bViewHidden, bShowOnlyOpenConversations);
            return String.Format(OseResources.Properties.Resources.HTML_Header,
                StylePrefix(projSettings),
                OseResources.Properties.Resources.HTML_DOM_Prefix,
                strHtml);
        }

        public void ReplaceUns(string strOldMemberGuid, string strNewMemberGuid)
        {
            try
            {
                CraftingInfo.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
                Verses.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
            }
            catch (StoryProjectData.ReplaceMemberException ex)
            {
                ex.StoryName = Name;
                throw;
            }
        }

        public void ReplaceProjectFacilitator(string strOldMemberGuid, string strNewMemberGuid)
        {
            CraftingInfo.ReplaceProjectFacilitator(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            CraftingInfo.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
        }
    }

    public class StoryStateTransitionHistory : List<StoryStateTransition>
    {
        public StoryStateTransitionHistory()
        {
        }

        public StoryStateTransitionHistory(XmlNode node)
        {
            if (node == null)
                return;
            XmlNodeList list = node.SelectNodes(String.Format("{0}/{1}",
                CstrElementLabelTransitionHistory, StoryStateTransition.CstrElemLabelStateTransition));
            if (list != null)
                foreach (XmlNode nodeStateTransition in list)
                    Add(new StoryStateTransition(nodeStateTransition));
        }

        public StoryStateTransitionHistory(NewDataSet.storyRow theStoryRow)
        {
            NewDataSet.TransitionHistoryRow[] aTHRs = theStoryRow.GetTransitionHistoryRows();
            if (aTHRs.Length == 1)
            {
                NewDataSet.TransitionHistoryRow theTHR = aTHRs[0];
                foreach (NewDataSet.StateTransitionRow aSTR in theTHR.GetStateTransitionRows())
                    Add(new StoryStateTransition(aSTR));
            }
        }

        public void Add(string strMemberId, StoryStageLogic.ProjectStages fromState,
            StoryStageLogic.ProjectStages toState)
        {
            var user = System.Security.Principal.WindowsIdentity.GetCurrent();
            Add(new StoryStateTransition
                    {
                        LoggedInMemberId = strMemberId,
                        FromState = fromState,
                        ToState = toState,
                        TransitionDateTime = DateTime.Now,
                        WindowsUserName = (user != null) ? user.Name : null
                    });
        }

        public const string CstrElementLabelTransitionHistory = "TransitionHistory";

        public XElement GetXml
        {
            get
            {
                var elem = new XElement(CstrElementLabelTransitionHistory);
                foreach (var stateTransitionHistory in this)
                    elem.Add(stateTransitionHistory.GetXml);
                return elem;
            }
        }

        public bool HasData
        {
            get { return (Count > 0); }
        }
    }

    public class StoryStateTransition
    {
        public string LoggedInMemberId { get; set; }
        public StoryStageLogic.ProjectStages FromState { get; set; }
        public StoryStageLogic.ProjectStages ToState { get; set; }
        public DateTime TransitionDateTime { get; set; }
        public string WindowsUserName { get; set; } // beware, this one may be null

        public StoryStateTransition()
        {
        }

        public StoryStateTransition(NewDataSet.StateTransitionRow theSTR)
        {
            LoggedInMemberId = theSTR.LoggedInMemberId;
            FromState = StoryStageLogic.GetProjectStageFromString(theSTR.FromState);
            ToState = StoryStageLogic.GetProjectStageFromString(theSTR.ToState);
            TransitionDateTime = theSTR.TransitionDateTime;
            if (!theSTR.IsWindowsUserNameNull())
                WindowsUserName = theSTR.WindowsUserName;
        }

        public StoryStateTransition(XmlNode node)
        {
            LoggedInMemberId = node.Attributes[CstrAttrNameLoggedInMemberId].Value;
            XmlAttribute attr;
            WindowsUserName = ((attr = node.Attributes[CstrAttrNameWindowsUserName]) != null) ? attr.Value : null;
            FromState = StoryStageLogic.GetProjectStageFromString(node.Attributes[CstrAttrNameFromState].Value);
            ToState = StoryStageLogic.GetProjectStageFromString(node.Attributes[CstrAttrNameToState].Value);
            TransitionDateTime = DateTime.Parse(node.Attributes[CstrAttrNameTransitionDateTime].Value);
        }

        public const string CstrElemLabelStateTransition = "StateTransition";
        public const string CstrAttrNameLoggedInMemberId = "LoggedInMemberId";
        public const string CstrAttrNameWindowsUserName = "WindowsUserName";
        public const string CstrAttrNameFromState = "FromState";
        public const string CstrAttrNameToState = "ToState";
        public const string CstrAttrNameTransitionDateTime = "TransitionDateTime";

        public XElement GetXml
        {
            get
            {
                XElement elem = new XElement(CstrElemLabelStateTransition,
                                             new XAttribute(CstrAttrNameLoggedInMemberId, LoggedInMemberId),
                                             new XAttribute(CstrAttrNameFromState, FromState.ToString().Substring(1)),
                                             new XAttribute(CstrAttrNameToState, ToState.ToString().Substring(1)),
                                             new XAttribute(CstrAttrNameTransitionDateTime, TransitionDateTime.ToString("s")));
                
                // this one might be null
                if (!String.IsNullOrEmpty(WindowsUserName))
                    elem.Add(new XAttribute(CstrAttrNameWindowsUserName, WindowsUserName));

                return elem;
            }
        }
    }
    
    public class TestorInfo
    {
        public const string CstrAttributeMemberID = "memberID";

        public TestorInfo(string strTestorGuid, string strTestComment)
        {
            TestorGuid = strTestorGuid;
            TestComment = strTestComment;
        }

        public TestorInfo(TestorInfo rhs)
        {
            TestorGuid = rhs.TestorGuid;
            TestComment = rhs.TestComment;
        }

        public TestorInfo(XmlNode nodeTest)
        {
            System.Diagnostics.Debug.Assert(nodeTest != null);
            if (nodeTest.Attributes != null)
            {
                XmlAttribute attr;
                TestorGuid = ((attr = nodeTest.Attributes[CstrAttributeMemberID]) != null) ? attr.Value : null;
                TestComment = nodeTest.InnerText;
            }
        }

        public string TestorGuid { get; set; }
        public string TestComment { get; set; }
    }

    public class TestInfo : List<TestorInfo>
    {
        public TestInfo()
        {
            
        }

        public TestInfo(IEnumerable<TestorInfo> rhs)
        {
            foreach (TestorInfo rhsTi in rhs)
                Add(new TestorInfo(rhsTi));
        }

        public bool Contains(string strGuid)
        {
            foreach (var testorInfo in this)
                if (testorInfo.TestorGuid == strGuid)
                    return true;
            return false;
        }

        public int IndexOf(string strGuid)
        {
            for (int i = 0; i < Count; i++)
            {
                var testorInfo = this[i];
                if (testorInfo.TestorGuid == strGuid)
                    return i;
            }
            return -1;
        }

        public void Add(XmlNode node, string strCollectionElement, string strElementLabel)
        {
            XmlNodeList list = node.SelectNodes(String.Format("{0}/{1}",
                strCollectionElement, strElementLabel));
            if (list != null)
                foreach (XmlNode nodeTest in list)
                    Add(new TestorInfo(nodeTest));
        }

        public void WriteXml(string strCollectionLabel, string strElementLabel, XElement elemCraftingInfo)
        {
            var elemTestors = new XElement(strCollectionLabel);
            foreach (TestorInfo testorInfo in this)
                elemTestors.Add(new XElement(strElementLabel,
                    new XAttribute(TestorInfo.CstrAttributeMemberID, testorInfo.TestorGuid),
                    testorInfo.TestComment));
            elemCraftingInfo.Add(elemTestors);
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            int nIndex = IndexOf(strOldUnsGuid);
            if (nIndex != -1)
            {
                var testor = this[nIndex];
                testor.TestorGuid = strNewUnsGuid;
            }
        }
    }

    public class CraftingInfoData
    {
        public string StoryCrafterMemberID;
        public string ProjectFacilitatorMemberID;
        public string StoryPurpose;
        public string ResourcesUsed;
        public string MiscellaneousStoryInfo;
        public string BackTranslatorMemberID;
        public TestInfo TestorsToCommentsRetellings = new TestInfo();
        public TestInfo TestorsToCommentsTqAnswers = new TestInfo();
        public bool IsBiblicalStory = true;

        public CraftingInfoData(string strCrafterMemberGuid, string strLoggedOnMemberGuid, bool bIsBiblicalStory)
        {
            StoryCrafterMemberID = strCrafterMemberGuid;
            ProjectFacilitatorMemberID = strLoggedOnMemberGuid;
            IsBiblicalStory = bIsBiblicalStory;
        }

        public CraftingInfoData(XmlNode node)
        {
            XmlNode elem;
            StoryCrafterMemberID = ((elem = node.SelectSingleNode(String.Format("{0}[@{1}]", 
                                                                                CstrElementLabelStoryCrafter,
                                                                                CstrAttributeMemberID))) != null) 
                                                                                ? elem.Attributes[CstrAttributeMemberID].Value 
                                                                                : null;
            ProjectFacilitatorMemberID = ((elem = node.SelectSingleNode(String.Format("{0}[@{1}]",
                                                                                CstrElementLabelProjectFacilitator,
                                                                                CstrAttributeMemberID))) != null)
                                                                                ? elem.Attributes[CstrAttributeMemberID].Value
                                                                                : null;
            StoryPurpose = ((elem = node.SelectSingleNode(CstrElementLabelStoryPurpose)) != null) 
                ? elem.InnerText 
                : null;
            ResourcesUsed = ((elem = node.SelectSingleNode(CstrElementLabelResourcesUsed)) != null)
                ? elem.InnerText
                : null;
            MiscellaneousStoryInfo = ((elem = node.SelectSingleNode(CstrElementLabelMiscellaneousStoryInfo)) != null)
                ? elem.InnerText
                : null;
            BackTranslatorMemberID = ((elem = node.SelectSingleNode(String.Format("{0}[@{1}]",
                                                                                CstrElementLabelBackTranslator,
                                                                                CstrAttributeMemberID))) != null)
                                                                                ? elem.Attributes[CstrAttributeMemberID].Value
                                                                                : null;

            TestorsToCommentsRetellings.Add(node, CstrElementLabelTestsRetellings, CstrElementLabelTestRetelling);
            TestorsToCommentsTqAnswers.Add(node, CstrElementLabelTestsTqAnswers, CstrElementLabelTestTqAnswer);
        }

        public CraftingInfoData(NewDataSet.storyRow theStoryRow)
        {
            NewDataSet.CraftingInfoRow[] aCIRs = theStoryRow.GetCraftingInfoRows();
            if (aCIRs.Length == 1)
            {
                NewDataSet.CraftingInfoRow theCIR = aCIRs[0];
                if (!theCIR.IsNonBiblicalStoryNull())
                    IsBiblicalStory = !theCIR.NonBiblicalStory;

                NewDataSet.StoryCrafterRow[] aSCRs = theCIR.GetStoryCrafterRows();
                if (aSCRs.Length == 1)
                    StoryCrafterMemberID = aSCRs[0].memberID;
                else
                    throw new ApplicationException(OseResources.Properties.Resources.IDS_ProjectFileCorrupted);

                NewDataSet.ProjectFacilitatorRow[] aPFRs = theCIR.GetProjectFacilitatorRows();
                if (aPFRs.Length == 1)
                    ProjectFacilitatorMemberID = aPFRs[0].memberID;

                if (!theCIR.IsStoryPurposeNull())
                    StoryPurpose = theCIR.StoryPurpose;

                if (!theCIR.IsResourcesUsedNull())
                    ResourcesUsed = theCIR.ResourcesUsed;

                if (!theCIR.IsMiscellaneousStoryInfoNull())
                    MiscellaneousStoryInfo = theCIR.MiscellaneousStoryInfo;

                NewDataSet.BackTranslatorRow[] aBTRs = theCIR.GetBackTranslatorRows();
                if (aBTRs.Length == 1)
                    BackTranslatorMemberID = aBTRs[0].memberID;

                NewDataSet.TestsRetellingsRow[] aTsReRs = theCIR.GetTestsRetellingsRows();
                if (aTsReRs.Length == 1)
                {
                    NewDataSet.TestRetellingRow[] aTReRs = aTsReRs[0].GetTestRetellingRows();
                    foreach (NewDataSet.TestRetellingRow aTReR in aTReRs)
                        TestorsToCommentsRetellings.Add(new TestorInfo(aTReR.memberID,
                                                                       (aTReR.IsTestRetelling_textNull())
                                                                           ? null
                                                                           : aTReR.TestRetelling_text));
                }

                NewDataSet.TestsTqAnswersRow[] aTsAnRs = theCIR.GetTestsTqAnswersRows();
                if (aTsAnRs.Length == 1)
                {
                    NewDataSet.TestTqAnswerRow[] aTReRs = aTsAnRs[0].GetTestTqAnswerRows();
                    foreach (NewDataSet.TestTqAnswerRow aTReR in aTReRs)
                        TestorsToCommentsTqAnswers.Add(new TestorInfo(aTReR.memberID,
                                                                       (aTReR.IsTestTqAnswer_textNull())
                                                                           ? null
                                                                           : aTReR.TestTqAnswer_text));
                }
            }
            else
                throw new ApplicationException(OseResources.Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo);
        }

        public CraftingInfoData(CraftingInfoData rhs)
        {
            StoryCrafterMemberID = rhs.StoryCrafterMemberID;
            ProjectFacilitatorMemberID = rhs.ProjectFacilitatorMemberID;
            StoryPurpose = rhs.StoryPurpose;
            ResourcesUsed = rhs.ResourcesUsed;
            MiscellaneousStoryInfo = rhs.MiscellaneousStoryInfo;
            BackTranslatorMemberID = rhs.BackTranslatorMemberID;
            IsBiblicalStory = rhs.IsBiblicalStory;
            TestorsToCommentsRetellings = new TestInfo(rhs.TestorsToCommentsRetellings);
            TestorsToCommentsTqAnswers = new TestInfo(rhs.TestorsToCommentsTqAnswers);
        }

        public const string CstrElementLabelCraftingInfo = "CraftingInfo";
        public const string CstrElementLabelNonBiblicalStory = "NonBiblicalStory";
        public const string CstrElementLabelStoryCrafter = "StoryCrafter";
        public const string CstrElementLabelProjectFacilitator = "ProjectFacilitator";
        public const string CstrElementLabelStoryPurpose = "StoryPurpose";
        public const string CstrElementLabelResourcesUsed = "ResourcesUsed";
        public const string CstrElementLabelMiscellaneousStoryInfo = "MiscellaneousStoryInfo";
        public const string CstrElementLabelBackTranslator = "BackTranslator";
        public const string CstrAttributeMemberID = "memberID";

        public const string CstrElementLabelTestsRetellings = "TestsRetellings";
        public const string CstrElementLabelTestRetelling = "TestRetelling";
        public const string CstrElementLabelTestsTqAnswers = "TestsTqAnswers";
        public const string CstrElementLabelTestTqAnswer = "TestTqAnswer";

        public XElement GetXml
        {
            get
            {
                var elemCraftingInfo = new XElement(CstrElementLabelCraftingInfo,
                                                         new XAttribute(CstrElementLabelNonBiblicalStory, !IsBiblicalStory),
                                                         new XElement(CstrElementLabelStoryCrafter,
                                                                      new XAttribute(CstrAttributeMemberID, StoryCrafterMemberID)));

                if (!String.IsNullOrEmpty(ProjectFacilitatorMemberID))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelProjectFacilitator, 
                        new XAttribute(CstrAttributeMemberID, ProjectFacilitatorMemberID)));

                if (!String.IsNullOrEmpty(StoryPurpose))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelStoryPurpose, StoryPurpose));

                if (!String.IsNullOrEmpty(ResourcesUsed))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelResourcesUsed, ResourcesUsed));

                if (!String.IsNullOrEmpty(MiscellaneousStoryInfo))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelMiscellaneousStoryInfo, MiscellaneousStoryInfo));

                if (!String.IsNullOrEmpty(BackTranslatorMemberID))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelBackTranslator, 
                        new XAttribute(CstrAttributeMemberID, BackTranslatorMemberID)));

                if (TestorsToCommentsRetellings.Count > 0)
                {
                    TestorsToCommentsRetellings.WriteXml(CstrElementLabelTestsRetellings,
                                                         CstrElementLabelTestRetelling,
                                                         elemCraftingInfo);
                }

                if (TestorsToCommentsTqAnswers.Count > 0)
                {
                    TestorsToCommentsTqAnswers.WriteXml(CstrElementLabelTestsTqAnswers,
                                                        CstrElementLabelTestTqAnswer,
                                                        elemCraftingInfo);
                }

                return elemCraftingInfo;
            }
        }

        public string PresentationHtml(TeamMembersData teamMembers, CraftingInfoData child)
        {
            string strRow = null;
            strRow += PresentationHtmlRow(teamMembers, "Story Crafter", StoryCrafterMemberID, 
                (child != null) 
                ? child.StoryCrafterMemberID 
                : null);
            strRow += PresentationHtmlRow(teamMembers, "Project Facilitator", ProjectFacilitatorMemberID, 
                (child != null) 
                ? child.ProjectFacilitatorMemberID 
                : null);
            strRow += PresentationHtmlRow(null, "Story Purpose", StoryPurpose,
                (child != null)
                ? child.StoryPurpose
                : null);
            strRow += PresentationHtmlRow(null, "Resources Used", ResourcesUsed,
                (child != null)
                ? child.ResourcesUsed
                : null);
            strRow += PresentationHtmlRow(null, "Miscellaneous Comments", MiscellaneousStoryInfo,
                (child != null)
                ? child.MiscellaneousStoryInfo
                : null);
            strRow += PresentationHtmlRow(teamMembers, "Back Translator", BackTranslatorMemberID,
                (child != null)
                ? child.BackTranslatorMemberID
                : null);

            int nInsertCount = 0;
            int i = 1;
            string strLabelFormat = "Retelling Testor {0}";
            while (i <= TestorsToCommentsRetellings.Count)
            {
                int nTestNumber = i + nInsertCount;
                string strTestor = TestorsToCommentsRetellings[i - 1].TestorGuid;

                // get the child tests that match this one
                string strChildMatch = FindChildEquivalentRetellings(strTestor, child);

                // see if there were any child verses that weren't processed
                if (!String.IsNullOrEmpty(strChildMatch))
                {
                    bool bFoundOne = false;
                    // the <= in the test led us to an infinite loop
                    // for (int j = i; j <= child.TestorsToCommentsRetellings.IndexOf(strChildMatch); j++)
                    for (int j = i; j < child.TestorsToCommentsRetellings.IndexOf(strChildMatch); j++)
                    {
                        string strChildPassedBy = child.TestorsToCommentsRetellings[j - 1].TestorGuid;
                        strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildPassedBy, true);
                        bFoundOne = true;
                        nInsertCount++;
                    }

                    if (bFoundOne)
                        continue;
                }

                strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strTestor, false);

                // if there is a child, but we couldn't find the equivalent testor...
                if ((child != null) && String.IsNullOrEmpty(strChildMatch) && (child.TestorsToCommentsRetellings.Count >= i))
                {
                    // this means the original testor (which we just showed as deleted) 
                    //  was replaced by whatever is the same verse in the child collection
                    strChildMatch = child.TestorsToCommentsRetellings[i - 1].TestorGuid;
                    strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildMatch, true);
                }

                i++;    // do this here in case we redo one (from 'continue' above)
            }

            if (child != null)
                while (i <= child.TestorsToCommentsRetellings.Count)
                {
                    string strChildTestor = child.TestorsToCommentsRetellings[i - 1].TestorGuid;
                    int nTestNumber = i + nInsertCount;
                    strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildTestor, true);
                    i++;
                }

            strLabelFormat = "Story testing {0}";
            while (i <= TestorsToCommentsRetellings.Count)
            {
                int nTestNumber = i + nInsertCount;
                string strTestor = TestorsToCommentsRetellings[i - 1].TestorGuid;

                // get the child tests that match this one
                string strChildMatch = FindChildEquivalentAnswers(strTestor, child);

                // see if there were any child verses that weren't processed
                if (!String.IsNullOrEmpty(strChildMatch))
                {
                    bool bFoundOne = false;
                    // the <= in the test led us to an infinite loop
                    // for (int j = i; j <= child.TestorsToCommentsRetellings.IndexOf(strChildMatch); j++)
                    for (int j = i; j < child.TestorsToCommentsRetellings.IndexOf(strChildMatch); j++)
                    {
                        string strChildPassedBy = child.TestorsToCommentsRetellings[j - 1].TestorGuid;
                        strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildPassedBy, true);
                        bFoundOne = true;
                        nInsertCount++;
                    }

                    if (bFoundOne)
                        continue;
                }

                strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strTestor, false);

                // if there is a child, but we couldn't find the equivalent testor...
                if ((child != null) && String.IsNullOrEmpty(strChildMatch) && (child.TestorsToCommentsRetellings.Count >= i))
                {
                    // this means the original testor (which we just showed as deleted) 
                    //  was replaced by whatever is the same verse in the child collection
                    strChildMatch = child.TestorsToCommentsRetellings[i - 1].TestorGuid;
                    strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildMatch, true);
                }

                i++;    // do this here in case we redo one (from 'continue' above)
            }

            if (child != null)
                while (i <= child.TestorsToCommentsRetellings.Count)
                {
                    string strChildTestor = child.TestorsToCommentsRetellings[i - 1].TestorGuid;
                    int nTestNumber = i + nInsertCount;
                    strRow += PresentationHtmlRow(teamMembers, String.Format(strLabelFormat, nTestNumber), strChildTestor, true);
                    i++;
                }
            
            return strRow;
        }

        private string FindChildEquivalentRetellings(string strTestor, CraftingInfoData child)
        {
            if (child != null)
                if (child.TestorsToCommentsRetellings.Contains(strTestor))
                    return strTestor;
            return null;
        }

        private string FindChildEquivalentAnswers(string strTestor, CraftingInfoData child)
        {
            if (child != null)
                if (child.TestorsToCommentsTqAnswers.Contains(strTestor))
                    return strTestor;
            return null;
        }

        protected string PresentationHtmlRow(TeamMembersData teamMembers, string strLabel, string strMemberId, bool bFromChild)
        {
            string strName = null;
            if ((teamMembers != null) && !String.IsNullOrEmpty(strMemberId))
            {
                strName = teamMembers.GetNameFromMemberId(strMemberId);
                if (bFromChild)
                    strName = Diff.HtmlDiff(null, strName, true);
            }

            if (String.IsNullOrEmpty(strName))
                strName = "-";  // so it's not nothing (or the HTML shows without a cell frame)

            return String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                                             strLabel),
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                             100,
                                                             String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                                           strLabel,
                                                                           StoryData.
                                                                              CstrLangInternationalBtStyleClassName,
                                                                           strName))));
        }

        protected string PresentationHtmlRow(TeamMembersData teamMembers, string strLabel, string strParentMemberId, string strChildMemberId)
        {
            string strName;
            if (teamMembers != null)
            {
                string strParent = null;
                if (!String.IsNullOrEmpty(strParentMemberId))
                    strParent = teamMembers.GetNameFromMemberId(strParentMemberId);

                strName = (String.IsNullOrEmpty(strChildMemberId))
                    ? strParent
                    : Diff.HtmlDiff(strParent, teamMembers.GetNameFromMemberId(strChildMemberId), true);
            }
            else if (String.IsNullOrEmpty(strChildMemberId))
                strName = strParentMemberId;
            else
                strName = Diff.HtmlDiff(strParentMemberId, strChildMemberId, true);

            if (String.IsNullOrEmpty(strName))
                strName = "-";  // so it's not nothing (or the HTML shows without a cell frame)
            return String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                                             strLabel),
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                             100,
                                                             String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                                           strLabel,
                                                                           StoryData.
                                                                              CstrLangInternationalBtStyleClassName,
                                                                           strName))));
        }

        public void ReplaceUns(string strOldMemberGuid, string strNewMemberGuid)
        {
            // before we do *any* changing, we have to verify that this isn't going 
            //  to put us in a bad situation: the same UNS twice for the same story
            if (TestorsToCommentsRetellings.Contains(strNewMemberGuid)
                || TestorsToCommentsTqAnswers.Contains(strNewMemberGuid))
            {
                throw new StoryProjectData.ReplaceMemberException
                          {
                              MemberGuid = strNewMemberGuid,
                              Format = Properties.Resources.IDS_ReplaceUnsException
                          };
            }

            if (BackTranslatorMemberID == strOldMemberGuid)
                BackTranslatorMemberID = strNewMemberGuid;
            TestorsToCommentsRetellings.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
            TestorsToCommentsTqAnswers.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceProjectFacilitator(string strOldMemberGuid, string strNewMemberGuid)
        {
            if (ProjectFacilitatorMemberID == strOldMemberGuid)
                ProjectFacilitatorMemberID = strNewMemberGuid;
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            if (StoryCrafterMemberID == strOldMemberGuid)
                StoryCrafterMemberID = strNewMemberGuid;
        }
    }

    public class StoriesData : List<StoryData>
    {
        public string SetName;

        public StoriesData(string strSetName)
        {
            SetName = strSetName;
        }

        public StoriesData(NewDataSet.storiesRow theStoriesRow, NewDataSet projFile, string strProjectFolder)
        {
            SetName = theStoriesRow.SetName;

            // finally, if it's not new, then it might (should) have stories as well
            foreach (NewDataSet.storyRow aStoryRow in theStoriesRow.GetstoryRows())
                Add(new StoryData(aStoryRow, projFile, strProjectFolder));
        }

        public new bool Contains(StoryData theSD)
        {
            foreach (StoryData aSD in this)
                if (aSD.Name == theSD.Name)
                    return true;
            return false;
        }

        public const string CstrElementLabelStories = "stories";
        public const string CstrAttributeLabelSetName = "SetName";

        public XElement GetXml
        {
            get
            {
                var elemStories = new XElement(CstrElementLabelStories, 
                    new XAttribute(CstrAttributeLabelSetName, SetName));

                foreach (StoryData aSD in this)
                    elemStories.Add(aSD.GetXml);

                return elemStories;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StorySearchIndex alstBoxesToSearch)
        {
            foreach (StoryData aSD in this)
            {
                VerseData.StringTransferSearchIndex ssi = alstBoxesToSearch.GetNewStorySearchIndex(aSD.Name);
                aSD.IndexSearch(findProperties, ref ssi);
            }
        }

        public StoryData GetStoryFromName(string strStoryName)
        {
            foreach (StoryData aSD in this)
                if (strStoryName == aSD.Name)
                    return aSD;
            return null;
        }

        public void ReplaceUns(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storyData in this)
                storyData.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceProjectFacilitator(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storyData in this)
                storyData.ReplaceProjectFacilitator(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storyData in this)
                storyData.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
        }
    }

    public class StoryProjectData : Dictionary<string, StoriesData>
    {
        public TeamMembersData TeamMembers;
        public ProjectSettings ProjSettings;
        public LnCNotesData LnCNotes;
        public string PanoramaFrontMatter;
        public string XmlDataVersion = "1.5";

        /// <summary>
        /// This version of the constructor should *always* be followed by a call to InitializeProjectSettings()
        /// </summary>
        public StoryProjectData()
        {
            TeamMembers = new TeamMembersData();
            LnCNotes = new LnCNotesData();
            PanoramaFrontMatter = OseResources.Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            // start with to stories sets (the current one and the obsolete ones)
            Add(OseResources.Properties.Resources.IDS_MainStoriesSet, new StoriesData(OseResources.Properties.Resources.IDS_MainStoriesSet));
            Add(OseResources.Properties.Resources.IDS_ObsoleteStoriesSet, new StoriesData(OseResources.Properties.Resources.IDS_ObsoleteStoriesSet));
        }

        static bool bProjectConvertWarnedOnce = false;
                
        public StoryProjectData(NewDataSet projFile, ProjectSettings projSettings)
        {
            // this version comes with a project settings object
            ProjSettings = projSettings;

            // if the project file we opened doesn't have anything yet.. (shouldn't really happen)
            if (projFile.StoryProject.Count == 0)
                projFile.StoryProject.AddStoryProjectRow(XmlDataVersion, ProjSettings.ProjectName, OseResources.Properties.Resources.IDS_DefaultPanoramaFrontMatter);
            else
            {
                projFile.StoryProject[0].ProjectName = ProjSettings.ProjectName; // in case the user changed it.

                if (projFile.StoryProject[0].version.CompareTo("1.3") == 0)
                {
                    // see if the user wants us to upgrade this one
                    if (MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_QueryConvertProjectFile1_3to1_4,
                        ProjSettings.ProjectName), OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                        throw BackOutWithNoUI;

                    // convert the 1.3 file to 1.4 using xslt
                    bProjectConvertWarnedOnce = true;
                    ConvertProjectFile1_3_to_1_4(ProjSettings.ProjectFilePath);
                }

                else if (projFile.StoryProject[0].version.CompareTo("1.4") == 0)
                {
                    // see if the user wants us to upgrade this one
                    if (!bProjectConvertWarnedOnce)
                        if (MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_QueryConvertProjectFile1_3to1_4,
                            ProjSettings.ProjectName), OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            throw BackOutWithNoUI;

                    // convert the 1.3 file to 1.4 using xslt
                    bProjectConvertWarnedOnce = true;
                    ConvertProjectFile1_4_to_1_5(ProjSettings.ProjectFilePath);
                }

                else if (projFile.StoryProject[0].version.CompareTo(XmlDataVersion) > 0)
                {
                    MessageBox.Show(OseResources.Properties.Resources.IDS_GetNewVersion, OseResources.Properties.Resources.IDS_Caption);
                    throw BackOutWithNoUI;
                }
            }

            PanoramaFrontMatter = projFile.StoryProject[0].PanoramaFrontMatter;
            if (String.IsNullOrEmpty(PanoramaFrontMatter))
                PanoramaFrontMatter = OseResources.Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            if (projFile.stories.Count == 0)
            {
                projFile.stories.AddstoriesRow(OseResources.Properties.Resources.IDS_MainStoriesSet,
                    projFile.StoryProject[0]);
                projFile.stories.AddstoriesRow(OseResources.Properties.Resources.IDS_ObsoleteStoriesSet,
                    projFile.StoryProject[0]);
            }

            TeamMembers = new TeamMembersData(projFile);
            ProjSettings.SerializeProjectSettings(projFile);
            LnCNotes = new LnCNotesData(projFile);

            // finally, if it's not new, then it might (should) have stories as well
            foreach (NewDataSet.storiesRow aStoriesRow in projFile.StoryProject[0].GetstoriesRows())
                Add(aStoriesRow.SetName, new StoriesData(aStoriesRow, 
                                                         projFile,
                                                         ProjSettings.ProjectFolder));
        }

        private void ConvertProjectFile1_3_to_1_4(string strProjectFilePath)
        {
            // if the user had 1.3 and customized the state transition xml file, 
            //  then blow it away.
            string strProjectFolder = Path.GetDirectoryName(strProjectFilePath);
            string strStateTransitions = Path.Combine(strProjectFolder, StoryStageLogic.StateTransitions.CstrStateTransitionsXmlFilename);
            if (File.Exists(strStateTransitions))
            {
                if (MessageBox.Show(Properties.Resources.IDS_ConfirmDeleteStateTransitions,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    throw BackOutWithNoUI;

                File.Delete(strStateTransitions);
            }

            // get the xml (.onestory) file into a memory string so it can be the 
            //  input to the transformer
            string strProjectFile = File.ReadAllText(strProjectFilePath);
            var streamData = new MemoryStream(Encoding.UTF8.GetBytes(strProjectFile));

#if DEBUG
            string strXslt = File.ReadAllText(@"C:\src\StoryEditor\StoryEditor\Resources\1.3 to 1.4.xslt");
            System.Diagnostics.Debug.Assert(strXslt == Properties.Resources.project_1_3_to_1_4);
#else
            string strXslt = Properties.Resources.project_1_3_to_1_4;
#endif
            var streamXSLT = new MemoryStream(Encoding.UTF8.GetBytes(strXslt));
            var xelemProjectFileXml = TransformedXmlDataToSfm(streamXSLT, streamData);
            throw BackOut2Reopen(xelemProjectFileXml);
        }

        private void ConvertProjectFile1_4_to_1_5(string strProjectFilePath)
        {
            // get the xml (.onestory) file into a memory string so it can be the 
            //  input to the transformer
            string strProjectFile = File.ReadAllText(strProjectFilePath);
            var streamData = new MemoryStream(Encoding.UTF8.GetBytes(strProjectFile));

            string strXslt = Properties.Resources._1_4_to_1_5;
            var streamXSLT = new MemoryStream(Encoding.UTF8.GetBytes(strXslt));
            var xelemProjectFileXml = TransformedXmlDataToSfm(streamXSLT, streamData);
            throw BackOut2Reopen(xelemProjectFileXml);
        }

        protected XElement TransformedXmlDataToSfm(Stream streamXSLT, Stream streamData)
        {
            var myProcessor = new XslCompiledTransform();
            var xslReader = XmlReader.Create(streamXSLT);
            myProcessor.Load(xslReader);

            // rewind
            streamData.Seek(0, SeekOrigin.Begin);
            var reader = XmlReader.Create(streamData);
            /*
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    _xslt.Transform(section.CreateReader(), null, writer);
                    stream.Seek(0, SeekOrigin.Begin);
                    transformed = XElement.Load(stream);
                }
            }
            */
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    myProcessor.Transform(reader, null, writer);
                    stream.Seek(0, SeekOrigin.Begin);
                    XElement elem = XElement.Load(XmlReader.Create(stream));
                    return elem;
                }
            }
        }

        // if this is "new", then we won't have a project name yet, so query the user for it
        public bool InitializeProjectSettings(TeamMemberData loggedOnMember)
        {
            bool bRet = false;
#if !DataDllBuild
            NewProjectWizard dlg = new NewProjectWizard(this) 
            { 
                LoggedInMember = loggedOnMember,
                Text = "Edit Project Settings"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                throw BackOutWithNoUI;

            // otherwise, it has our new project settings
            ProjSettings = dlg.ProjSettings;
            bRet = dlg.Modified;
#endif
            return bRet;
        }

        // routines can use this exception to back out of creating a new project without UI 
        //  (presumably, because they've already done so--e.g. "are you sure you want to 
        //  overwrite this project + user Cancel)
        internal class BackOutWithNoUIException : ApplicationException
        {
        }

        internal static BackOutWithNoUIException BackOutWithNoUI
        {
            get { return new BackOutWithNoUIException(); }
        }

        internal class Backout2ReOpenException : ApplicationException
        {
            public XElement XmlProjectFile;
        }

        internal static Backout2ReOpenException BackOut2Reopen(XElement xElement)
        {
            return new Backout2ReOpenException { XmlProjectFile = xElement };
        }

        internal string GetMemberNameFromMemberGuid(string strMemberGuid)
        {
            string strMemberName = null;
            foreach (TeamMemberData aTMD in TeamMembers.Values)
            {
                if (aTMD.MemberGuid == strMemberGuid)
                {
                    strMemberName = aTMD.Name;
                    break;
                }
            }
            return strMemberName;
        }

#if !DataDllBuild
        /*
        internal static string QueryProjectName()
        {
            bool bDoItAgain;
            string strProjectName = null;
            do
            {
                bDoItAgain = false;
                strProjectName = Microsoft.VisualBasic.Interaction.InputBox(OseResources.Properties.Resources.IDS_EnterProjectName, OseResources.Properties.Resources.IDS_Caption, strProjectName, 300, 200);
                if (String.IsNullOrEmpty(strProjectName))
                    throw new ApplicationException(OseResources.Properties.Resources.IDS_UnableToCreateProjectWithoutName);

                // See if there's already a project with this name (which may be elsewhere)
                for (int i = 0; i < Properties.Settings.Default.RecentProjects.Count; i++)
                {
                    string strProject = Properties.Settings.Default.RecentProjects[i];
                    if (strProject == strProjectName)
                    {
                        string strProjectFolder = Properties.Settings.Default.RecentProjectPaths[i];
                        DialogResult res = MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_AboutToStrandProject, Environment.NewLine, strProjectName, strProjectFolder), OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Cancel)
                            throw BackOutWithNoUI;
                        if (res == DialogResult.No)
                            bDoItAgain = true;
                        break;
                    }
                }
            } while (bDoItAgain);
            return strProjectName;
        }
        */

        internal TeamMemberData GetLogin(ref bool bModified)
        {
            // look at the last person to log in and see if we ought to automatically log them in again
            //  (basically Crafters or others that are also the same role as last time)
            string strMemberName = null;
            TeamMemberData loggedOnMember = null;
            TeamMemberData.UserTypes eRole = TeamMemberData.UserTypes.Undefined;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
            {
                strMemberName = Properties.Settings.Default.LastMemberLogin;
                eRole = TeamMemberData.GetMemberType(Properties.Settings.Default.LastUserType);
                if (CanLoginMember(strMemberName, eRole))
                    loggedOnMember = TeamMembers[strMemberName];
            }

            // otherwise, fall thru and make them pick it.
            if (loggedOnMember == null)
                loggedOnMember = EditTeamMembers(strMemberName, eRole, true, 
                    ProjSettings, true, ref bModified);

            // if we have a logged on person, then initialize the overrides for that
            //  person (i.e. fonts, keyboards)
            if (loggedOnMember != null)
                ProjSettings.InitializeOverrides(loggedOnMember);

            return loggedOnMember;
        }

        // this can be used to determine whether a given member name and type are one
        //  of the ones in this project (for auto-login)
        public bool CanLoginMember(string strMemberName, TeamMemberData.UserTypes eRole)
        {
            if (TeamMembers.ContainsKey(strMemberName))
            {
                var aTMD = TeamMembers[strMemberName];
                if (TeamMemberData.IsUser(aTMD.MemberType, eRole))
                {
                    // kind of a kludge, but necessary for the state logic
                    //  If we're going to return true (meaning that we can auto-log this person in), then
                    //  if we have an English Back-translator person in the team, then we have to set the 
                    //  member with the edit token when we get to the EnglishBT state as that person
                    //  otherwise, it's a crafter
                    /*
                    StoryStageLogic.stateTransitions[StoryStageLogic.ProjectStages.eBackTranslatorTypeInternationalBT].MemberTypeWithEditToken =
                        (TeamMembers.HasOutsideEnglishBTer) ? TeamMemberData.UserTypes.eEnglishBacktranslator : TeamMemberData.UserTypes.eProjectFacilitator;
                    */
                    return true;
                }
            }
            return false;
        }

        // returns the logged in member
        internal TeamMemberData EditTeamMembers(string strMemberName, 
            TeamMemberData.UserTypes eRole, bool bUseLoginLabel, 
            ProjectSettings projSettings, bool bLoginRequired, ref bool bModified)
        {
            var dlg = new TeamMemberForm(TeamMembers, bUseLoginLabel, projSettings, this);
            if (!String.IsNullOrEmpty(strMemberName))
            {
                try
                {
                    // if we did find the "last member" in the list, but couldn't accept it without question
                    //  (e.g. because the role was different), then at least pre-select the member
                    dlg.SelectedMember = TeamMemberForm.GetListBoxItem(strMemberName, eRole);
                }
                catch { }    // might fail if the "last user" on this machine is opening this project file for the first time... just ignore
            }

            var res = dlg.ShowDialog();

            // if the user added a new member, then the proj file is 'dirty'
            if (dlg.Modified)
                bModified = true;

            if (res != DialogResult.OK)
            {
                if (bUseLoginLabel && bLoginRequired)
                    MessageBox.Show(OseResources.Properties.Resources.IDS_HaveToLogInToContinue, OseResources.Properties.Resources.IDS_Caption);

                throw BackOutWithNoUI;
            }

            return TeamMembers[dlg.SelectedMemberName];
        }
#endif

        // use of this version factors in both the settings in the project file
        public bool GetHgRepoUsernamePassword(string strProjectName, TeamMemberData loggedOnMember, 
            out string strUsername, out string strPassword, out string strHgUrlBase)
        {
            strPassword = strHgUrlBase = null;    // just in case we don't have anything for this.

#if !DataDllBuild
            string strRepoUrl, strDummy;
            if (Program.GetHgRepoParameters(strProjectName, out strUsername, out strRepoUrl, 
                out strDummy))
            {
                if (!String.IsNullOrEmpty(strRepoUrl))
                {
                    var uri = new Uri(strRepoUrl);
                    if (!String.IsNullOrEmpty(uri.UserInfo) && (uri.UserInfo.IndexOf(':') != -1))
                    {
                        string[] astrUserInfo = uri.UserInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        System.Diagnostics.Debug.Assert((astrUserInfo.Length == 2) && (astrUserInfo[0] == strUsername));
                        strUsername = astrUserInfo[0];
                        strPassword = astrUserInfo[1];
                    }
                    strHgUrlBase = uri.Scheme + "://" + uri.Host;
                }
            }
#else
            strUsername = null;
#endif

            // okay, the above is what we saved in the user file, but it's possible that that
            //  will be empty (e.g. if change the assembly number), so let's get it out of the
            //  project file as well (which will supercede the above information)
            if ((loggedOnMember != null)
                && (!String.IsNullOrEmpty(loggedOnMember.HgUsername))
                && (!String.IsNullOrEmpty(loggedOnMember.HgPassword)))
            {
                strUsername = loggedOnMember.HgUsername;
                strPassword = loggedOnMember.HgPassword;
            }

            return !String.IsNullOrEmpty(strHgUrlBase);
        }

        public bool IsASeparateEnglishBackTranslator
        {
            get
            {
                System.Diagnostics.Debug.Assert(TeamMembers != null);

                // the role "English Back-translator" only has meaning if there's another
                //  language involved.
                return TeamMembers.Values.Any(aTM => 
                    TeamMemberData.IsUser(aTM.MemberType, 
                    TeamMemberData.UserTypes.EnglishBackTranslator));
            }
        }

        public static string GetRunningFolder
        {
            get
            {
                string strCurrentFolder = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                return Path.GetDirectoryName(strCurrentFolder);
            }
        }

        public const string CstrAttributeProjectName = "ProjectName";

        public XElement GetXml
        {
            get
            {
                XElement elemStoryProject =
                    new XElement("StoryProject",
                        new XAttribute("version", XmlDataVersion),
                        new XAttribute(CstrAttributeProjectName, ProjSettings.ProjectName),
                        new XAttribute("PanoramaFrontMatter", PanoramaFrontMatter),
                        TeamMembers.GetXml,
                        ProjSettings.GetXml);
                
                if (ProjSettings.HasAdaptItConfigurationData)
                    elemStoryProject.Add(ProjSettings.AdaptItConfigXml);

                elemStoryProject.Add(LnCNotes.GetXml);

                foreach (StoriesData aSsD in Values)
                    elemStoryProject.Add(aSsD.GetXml);

                return elemStoryProject;
            }
        }

        public void ReplaceUns(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storiesData in Values)
                storiesData.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceProjectFacilitator(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storiesData in Values)
                storiesData.ReplaceProjectFacilitator(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storiesData in Values)
                storiesData.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
        }

        public class ReplaceMemberException : Exception
        {
            public string MemberGuid { get; set; }
            public string StoryName { get; set; }
            public string Format { get; set; }
        }
    }
}
