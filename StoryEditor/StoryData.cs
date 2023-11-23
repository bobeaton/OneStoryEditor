using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Xsl;
using NetLoc;
using System.Collections.Specialized;

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
        public bool JustAdded;

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

            var thePf = projectData.GetMemberFromId(strLoggedOnMemberGuid);
            System.Diagnostics.Debug.Assert(TeamMemberData.IsUser(thePf.MemberType,
                                                                  TeamMemberData.UserTypes.ProjectFacilitator));

            TasksAllowedPf = (TasksPf.TaskSettings)thePf.DefaultAllowed;
            TasksRequiredPf = (TasksPf.TaskSettings)thePf.DefaultRequired;
            TasksAllowedCit = TasksCit.DefaultAllowed;
            TasksRequiredCit = TasksCit.DefaultRequired;

            if (bIsBiblicalStory)
            {
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
            else
            {
                // can't require Anchors, Retellings, etc in a non-biblical story.
                TasksRequiredPf &= ~(TasksPf.TaskSettings.Anchors |
                                     TasksPf.TaskSettings.Answers |
                                     TasksPf.TaskSettings.Answers2 |
                                     TasksPf.TaskSettings.Retellings |
                                     TasksPf.TaskSettings.Retellings2 |
                                     TasksPf.TaskSettings.TestQuestions);
            }
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
            StageTimeStamp = DateTime.Parse(node.Attributes[CstrAttributeTimeStamp].Value).ToLocalTime();
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

            // the guid is supposed to be unique, but there are some cases where the merger might actually
            //  leave two stories with the same guid. If that happens, then just start over with a new
            //  guid (we'll lose historical differencing, but this is the lesser of 2 evils).
            guid = theStoryRow.guid;
            if (ProjectReader.UniqueStoryGuids.Contains(guid))
            {
                Debug.Assert(false, String.Format("Duplicate unique identifier for story '{1}'{0}{0}{2}",
                                                  Environment.NewLine,
                                                  Name,
                                                  guid));
                guid = Guid.NewGuid().ToString();
            }

            ProjectReader.UniqueStoryGuids.Add(guid);

            StageTimeStamp = (theStoryRow.IsstageDateTimeStampNull())
                                 ? DateTime.Now
                                 : theStoryRow.stageDateTimeStamp.ToLocalTime();
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

            // the guid should be replicated
            guid = Guid.NewGuid().ToString();  // rhs.guid;

            StageTimeStamp = rhs.StageTimeStamp;
            ProjStage = new StoryStageLogic(rhs.ProjStage);
            CraftingInfo = new CraftingInfoData(rhs.CraftingInfo);
            TransitionHistory = new StoryStateTransitionHistory(rhs.TransitionHistory);
            Verses = new VersesData(rhs.Verses);
        }

        public const string CstrElementNameStory = "story";
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

                var elemStory = new XElement(CstrElementNameStory,
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
                                             new XAttribute(CstrAttributeTimeStamp, ToUniversalTime(StageTimeStamp)),
                                             CraftingInfo.GetXml);

                if (TransitionHistory.HasData)
                    elemStory.Add(TransitionHistory.GetXml);

                if (Verses.HasData)
                        elemStory.Add(Verses.GetXml);

                return elemStory;
            }
        }

        public static string ToUniversalTime(DateTime dateTime)
        {
            // weird: "s" gives "yyyy-mm-ddThh:mm:ss"
            //        "u" gives "yyyy-mm-dd hh:mm:ssZ"
            //  but the standard is:
            //                  "yyyy-mm-ddThh:mm:ssZ"
            var strUtc = dateTime.ToUniversalTime().ToString("u");
            return strUtc.Replace(' ', 'T');
        }

        public void ChangeRetellingTester(TeamMembersData teamMembersData, int nTestIndex, 
            string strNewGuid, ref TestInfo testInfoNew)
        {
            // first see if UNS being 'added' is already there (i.e. it's just a change
            //  of index)
            string strOldGuid = null;
            if (ChangeTester(teamMembersData, CraftingInfo.TestersToCommentsRetellings,
                    nTestIndex, ref testInfoNew, ref strOldGuid, ref strNewGuid))
            {
                Verses.ChangeRetellingTesterGuid(strOldGuid, strNewGuid);
            }
        }

        public void ChangeTqAnswersTester(TeamMembersData teamMembersData, int nTestIndex, 
            string strNewGuid, ref TestInfo testInfoNew)
        {
            // first see if UNS being 'added' is already there (i.e. it's just a change
            //  of index)
            string strOldGuid = null;
            if (ChangeTester(teamMembersData, CraftingInfo.TestersToCommentsTqAnswers,
                    nTestIndex, ref testInfoNew, ref strOldGuid, ref strNewGuid))
            {
                Verses.ChangeTqAnswersTesterGuid(strOldGuid, strNewGuid);
            }
        }

        private static bool ChangeTester(TeamMembersData teamMembersData, 
            TestInfo testInfo, int nTestIndex, ref TestInfo testInfoNew, 
            ref string strOldGuid, ref string strNewGuid)
        {
            bool bRet = false;
            MemberIdInfo memberIdInfo;
            if (testInfo.Count > nTestIndex)
            {
                var oldTesterInfo = testInfo[nTestIndex];
                if (oldTesterInfo.MemberId != strNewGuid)
                {
                    strOldGuid = oldTesterInfo.MemberId;
                    
                    // if the new guid is already in the list (i.e. the person is being
                    //  moved to another index), then we have to do the 'swap' algorithm 
                    //  (i.e. a->c, b->a, c->b) so we don't end up having (if temporarily) 
                    //  two sets of results for one UNS, since we can't distinguish 
                    //  between them to split them up again. So 'c' will be the member's 
                    //  *name* rather than guid and after we're done, we'll check for the 
                    //  existance of names as guids and swap them to guids if so
                    if (testInfo.Contains(strNewGuid))
                        strNewGuid = teamMembersData.GetNameFromMemberId(strNewGuid);

                    memberIdInfo = new MemberIdInfo(strNewGuid, oldTesterInfo.MemberComment);
                    bRet = true;
                }
                else
                {
                    memberIdInfo = oldTesterInfo;
                }
            }
            else
            {
                Debug.Assert(false); // can this happen?
                // default comment for both
                string strComment = String.Format(StoryEditor.InferenceTestCommentFormat,
                                                  DateTime.Now.ToString("yyyy-MMM-dd"));
                memberIdInfo = new MemberIdInfo(strNewGuid, strComment);
            }

            testInfoNew.Insert(nTestIndex, memberIdInfo);
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

        public bool AreUnapprovedConsultantNotes
        {
            get { return Verses.AreUnapprovedConsultantNotes; }
        }

        public bool AreUnrespondedToCoachNoteComments
        {
            get { return Verses.AreUnrespondedToCoachNoteComments; }
        }

        public bool HasCoachNoteData
        {
            get { return Verses.HasCoachNoteData; }
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

        public string CheckForMember(StoryProjectData storyProjectData, 
            TeamMemberData.UserTypes eMemberType, ref MemberIdInfo member)
        {
            if (!MemberIdInfo.Configured(member))
            {
                var str = QueryForMember(storyProjectData, eMemberType,
                                         String.Format(
                                             Localizer.Str("Choose the {0} working on the story '{1}'"),
                                             TeamMemberData.GetMemberTypeAsDisplayString(eMemberType),
                                             Name));
                if (!String.IsNullOrEmpty(str))
                    MemberIdInfo.SetCreateIfEmpty(ref member, str, true);
            }

            return member?.MemberId;
        }

        public bool CheckForConNotesParticipants(StoryProjectData storyProjectData, 
            ref bool bModified)
        {
            bool bNeedPf, bNeedCons, bNeedCoach;
            Verses.CheckConNoteMemberIds(out bNeedPf, out bNeedCons, out bNeedCoach);
            if (bNeedPf || !MemberIdInfo.Configured(CraftingInfo.ProjectFacilitator))
            {
                var str = CheckForMember(storyProjectData,
                                         TeamMemberData.UserTypes.ProjectFacilitator,
                                         ref CraftingInfo.ProjectFacilitator);
                if (String.IsNullOrEmpty(str))
                    return false;
                bModified = true;
            }

            // the Consultant isn't strictly necessary until there are Consultant Notes
            //  so avoid the 'barage of questions' when a PF adds a new story.
            if (bNeedCons) // || !MemberIdInfo.Configured(CraftingInfo.Consultant))
            {
                var str = CheckForMember(storyProjectData,
                                         TeamMemberData.UserTypes.IndependentConsultant |
                                         TeamMemberData.UserTypes.ConsultantInTraining,
                                         ref CraftingInfo.Consultant);
                if (String.IsNullOrEmpty(str))
                    return false;
                bModified = true;
            }

            // for the coach, we only need this if it's a 'manage with coaching' situation
            // but again, this isn't strictly necessary until there are Coach Notes
            //  so avoid the 'barage of questions' when a PF adds a new story.
            if (bNeedCoach) /* || 
                (!MemberIdInfo.Configured(CraftingInfo.Coach) &&
                    !storyProjectData.TeamMembers.HasIndependentConsultant)) */
            {
                var str = CheckForMember(storyProjectData,
                                         TeamMemberData.UserTypes.Coach,
                                         ref CraftingInfo.Coach);
                if (String.IsNullOrEmpty(str))
                    return false;
                bModified = true;
            }
            
            if (bNeedPf || bNeedCons || bNeedCoach)
            {
                SetCommentMemberId();
            }

            return true;
        }

        private static string QueryForMember(StoryProjectData storyProjectData, 
            TeamMemberData.UserTypes eMemberToCheck, string strPickerTitle)
        {
            string strMemberId = storyProjectData.TeamMembers.MemberIdOfOneAndOnlyMemberType(eMemberToCheck);
            if (!String.IsNullOrEmpty(strMemberId))
                return strMemberId;

            // fall thru means that there isn't just one of them, so ask the user 
            //  to tell which one it is
            var dlg = new MemberPicker(storyProjectData,
                                       eMemberToCheck)
                          {
                              Text = strPickerTitle
                          };

            return (dlg.ShowDialog() == DialogResult.OK)
                       ? dlg.SelectedMember.MemberGuid
                       : null;
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
            var projSettings = new ProjectSettings(nodeProjectFile, strProjectPath);
            var teamMembers = new TeamMembersData(nodeProjectFile);
            StoryData ParentStory = null;
            StoryData ChildStory = null;
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
                false,  // don't use textareas
                StoryEditor.TextFields.Undefined,
                null,   // TransliteratorVernacular
                null,   // TransliteratorNationalBt
                null,   // TransliteratorInternationalBt
                null);  // TransliteratorFreeTranslation

            string strHtml = null;
            if (ParentStory != null)
                strHtml = ParentStory.PresentationHtml(viewSettings,
                    projSettings, teamMembers, ChildStory);
            else if (ChildStory != null)
                strHtml = ChildStory.PresentationHtml(viewSettings,
                    projSettings, teamMembers, null);
            return strHtml;
        }

        public string PresentationHtml(VerseData.ViewSettings viewSettings, 
            ProjectSettings projSettings, 
            TeamMembersData teamMembers, 
            StoryData child)
        {
            Rainbow.HtmlDiffEngine.Added.BeginTag = "<span style=\"text-decoration: underline; color: orange\">";
            string strHtml = PresentationHtmlWithoutHtmlDocOutside(viewSettings,
                                                                   projSettings,
                                                                   teamMembers,
                                                                   child,
                                                                   (child != null)
                                                                       ? PresentationType.Differencing
                                                                       : PresentationType.Plain);
            return AddHtmlHtmlDocOutside(strHtml, projSettings);
        }

        public static string AddHtmlHtmlDocOutside(string strHtmlInside, ProjectSettings projSettings)
        {
            return String.Format(Properties.Resources.StoryBtHtml,
                                 Properties.Resources.jquery_min,
                                 Properties.Resources.StoryBtJs,
                                 GetPlaceHolders(projSettings),
                                 StylePrefix(projSettings, null, null),
                                 strHtmlInside,
                                 Properties.Resources.StoryBtPsJs);
        }

        private static string GetPlaceHolders(ProjectSettings projSettings)
        {
            return String.Format(Properties.Resources.StoryBtPlaceholders,
                                 projSettings.Vernacular.LangName,
                                 projSettings.NationalBT.LangName,
                                 projSettings.InternationalBT.LangName,
                                 projSettings.FreeTranslation.LangName);
        }

        public enum PresentationType    
        {
            Plain,          // for the Story BT pane
            Printing,       // for the printing dialog
            Differencing    // for the historical differencing dialog
        }

        public string PresentationHtmlWithoutHtmlDocOutside(
            VerseData.ViewSettings viewSettings, 
            ProjectSettings projSettings, 
            TeamMembersData teamMembers, 
            StoryData child,
            PresentationType presentationType)
        {
            bool bShowVernacular = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField);
            bool bShowNationalBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField);
            bool bShowEnglishBT = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField);
            bool bShowFreeTranslation = viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField);

            int nNumCols = 0;
            if (bShowVernacular) nNumCols++;
            if (bShowNationalBT) nNumCols++;
            if (bShowEnglishBT) nNumCols++;
            if (bShowFreeTranslation) nNumCols++;

            // it's not helpful to have nothing...
            if (nNumCols == 0)
                nNumCols = 1;

            // always gotta have at least the title and stage
            var bIsPrinting = (child == null);
            var strHtml = CraftingInfoData.PresentationHtmlRow(Localizer.Str("Story Name"), Name,
                                                               (bIsPrinting)
                                                                   ? null
                                                                   : child.Name,
                                                               bIsPrinting);

            // for stage, it's a bit more complicated
            StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[ProjStage.ProjectStage];
            string strParentStage = st.StageDisplayString;
            string strChildStage = null;
            if (child != null)
            {
                st = StoryStageLogic.stateTransitions[child.ProjStage.ProjectStage];
                strChildStage = st.StageDisplayString;
            }
            strHtml += CraftingInfoData.PresentationHtmlRow(Localizer.Str("Story Turn"),
                                                            strParentStage,
                                                            strChildStage,
                                                            bIsPrinting);

            // if requested, include the remaining 'front matter'
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryFrontMatter))
            {
                strHtml += CraftingInfo.PresentationHtml(teamMembers, (child != null) ? child.CraftingInfo : null);
            }

            // make a sub-table out of all this
            strHtml = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strHtml)));

            // occasionally, we just want to print the header stuff (without lines)
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.VernacularLangField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.NationalBtLangField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.InternationalBtField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.FreeTranslationField)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.AnchorFields)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.ExegeticalHelps)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.RetellingFields)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestions)
                || viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers))
            {
                if (child != null)
                    strHtml += Verses.PresentationHtmlWithDiffs(child.CraftingInfo, child.Verses, nNumCols, viewSettings,
                                    teamMembers.HasOutsideEnglishBTer, presentationType, teamMembers);
                else
                    strHtml += Verses.PresentationHtml(CraftingInfo,
                                                       nNumCols,
                                                       viewSettings,
                                                       teamMembers.HasOutsideEnglishBTer, presentationType, 
                                                       teamMembers);
            }
            else
            {
                strHtml += String.Format("<p>{0}</p>", Environment.NewLine);
            }

            return strHtml;
        }

        public const string CstrLangVernacularStyleClassName = "LangVernacular";
        public const string CstrLangNationalBtStyleClassName = "LangNationalBT";
        public const string CstrLangInternationalBtStyleClassName = "LangInternationalBT";
        public const string CstrLangFreeTranslationStyleClassName = "LangFreeTranslation";
        public const string CstrLangLocalizationStyleClassName = "LocalizationStyle";
        public const string CstrLangLocalizationEdgeStyleClassName = "LocalizationEdgeStyle";
        public const string CstrLangTextAreaStyleClassName = "TextAreaStyle";

        public static string StylePrefix(ProjectSettings projSettings, string strFontName, string strFontSize)
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
            if ((Localizer.Default != null) &&
                (Localizer.Default.LocLanguage != null) &&
                (Localizer.Default.LocLanguage.Font != null))
            {
                strLangStyles += String.Format(Properties.Resources.HTML_LangStyleCenter,
                                               CstrLangLocalizationStyleClassName,
                                               Localizer.Default.LocLanguage.Font.Name,
                                               Localizer.Default.LocLanguage.Font.SizeInPoints);
                strLangStyles += projSettings.Localization.HtmlStyle(CstrLangLocalizationEdgeStyleClassName);
            }

            if (!String.IsNullOrEmpty(strFontName))
            {
                strLangStyles += String.Format(Properties.Resources.HTML_LangStyleCenter,
                                               CstrLangTextAreaStyleClassName,
                                               strFontName,
                                               strFontSize);
            }
            else
            {
                // otherwise, just use the English style for ConNotes
                strLangStyles += projSettings.InternationalBT.HtmlStyle(CstrLangTextAreaStyleClassName);
            }

            return String.Format(Properties.Resources.HTML_StyleDefinition,
                                 Properties.Settings.Default.ConNoteTableFontSize,
                                 Properties.Settings.Default.ConNoteButtonFontSize,
                                 strLangStyles,
                                 VerseData.HtmlColor(Properties.Settings.Default.RetellingVernacularColor),
                                 VerseData.HtmlColor(Properties.Settings.Default.TQVernacularColor),
                                 VerseData.HtmlColor(Properties.Settings.Default.AnswersVernacularColor));
        }

        public string ConNoteHtml(HtmlConNoteControl htmlConNoteCtrl, ProjectSettings projSettings, int nVerseIndex, 
            int nConversationIndex, TeamMemberData loggedOnMember, TeamMembersData teamMembers, 
            ConsultNoteDataConverter theCnDc)
        {
            string strHtml = theCnDc.Html(htmlConNoteCtrl, teamMembers, loggedOnMember, this,
                                          nVerseIndex,
                                          nConversationIndex);

            string strFontName, strFontSize;
            NetBibleViewer.ReadFontNameAndSizeFromUserConfig(htmlConNoteCtrl.SettingsKeyForFontToUse, out strFontName, out strFontSize);

            return String.Format(Properties.Resources.HTML_Header,
                                 StylePrefix(projSettings, strFontName, strFontSize),
                                 Properties.Resources.ConNoteDomPrefix,
                                 strHtml,
                                 Properties.Resources.HTML_Script_AddTextareaMouseDown);
        
        }

        public string ConsultantNotesHtml(object htmlConNoteCtrl, 
            ProjectSettings projSettings, TeamMemberData LoggedOnMember, 
            TeamMembersData teamMembers, bool bViewHidden, bool bShowOnlyOpenConversations, string strFontName, string strFontSize)
        {
            string strHtml = Verses.ConsultantNotesHtml(htmlConNoteCtrl, LoggedOnMember, 
                teamMembers, this, bViewHidden, bShowOnlyOpenConversations);

            return String.Format(Properties.Resources.HTML_Header,
                                 StylePrefix(projSettings, strFontName, strFontSize),
                                 Properties.Resources.ConNoteDomPrefix,
                                 strHtml,
                                 Properties.Resources.HTML_Script_AddTextareaMouseDown);
        }

        public string CoachNotesHtml(object htmlConNoteCtrl, 
            ProjectSettings projSettings, TeamMemberData LoggedOnMember, 
            TeamMembersData teamMembers, bool bViewHidden, bool bShowOnlyOpenConversations, string strFontName, string strFontSize)
        {
            string strHtml = Verses.CoachNotesHtml(htmlConNoteCtrl, LoggedOnMember, 
                teamMembers, this, bViewHidden, bShowOnlyOpenConversations);

            return String.Format(Properties.Resources.HTML_Header,
                                 StylePrefix(projSettings, strFontName, strFontSize),
                                 Properties.Resources.ConNoteDomPrefix,
                                 strHtml,
                                 Properties.Resources.HTML_Script_AddTextareaMouseDown);
        }

        /// <summary>
        /// this is for when you don't know what role the person has, but just want to replace it any possible place...
        /// </summary>
        /// <param name="strOldMemberGuid"></param>
        /// <param name="strNewMemberGuid"></param>
        public void ReplaceMemberId(string strOldMemberGuid, string strNewMemberGuid)
        {
            ReplaceUns(strOldMemberGuid, strNewMemberGuid);
            if (MemberIdInfo.SafeGetMemberId(CraftingInfo.ProjectFacilitator) == strOldMemberGuid)
                ReplaceProjectFacilitator(strNewMemberGuid);

            ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
            ReplaceConsultant(strOldMemberGuid, strNewMemberGuid);
            if (MemberIdInfo.SafeGetMemberId(CraftingInfo.Coach) == strOldMemberGuid)
                ReplaceCoach(strNewMemberGuid);
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

        public void ReplaceProjectFacilitator(string strNewMemberGuid)
        {
            string strOldMemberGuid = 
                CraftingInfo.ReplaceProjectFacilitator(strNewMemberGuid);

            // also have to update any pfrc comments in the ConsultantNotes pane
            Verses.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceConsultant(string strOldMemberGuid, string strNewMemberGuid)
        {
            if (MemberIdInfo.SafeGetMemberId(CraftingInfo.Consultant) == strOldMemberGuid)
                CraftingInfo.ReplaceConsultant(strNewMemberGuid);

            // also have to update any comments in the ConNotes panes
            Verses.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);

            // Also have to update any State Transition logins (since this person is soon to disappear)
        }

        public void ReplaceCoach(string strNewMemberGuid)
        {
            string strOldMemberGuid =
                CraftingInfo.ReplaceCoach(strNewMemberGuid);

            // also have to update any comments in the ConNotes panes
            Verses.UpdateCommentMemberId(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            CraftingInfo.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
        }

        public void SetCommentMemberId()
        {
            Verses.SetCommentMemberId(
                MemberIdInfo.SafeGetMemberId(CraftingInfo.ProjectFacilitator),
                MemberIdInfo.SafeGetMemberId(CraftingInfo.Consultant),
                MemberIdInfo.SafeGetMemberId(CraftingInfo.Coach));
        }

        public bool DoesReferenceTqUns(string strMemberId)
        {
            return Verses.DoesReferenceTqUns(strMemberId);
        }

        public void DeleteRetellingTestResults(int nTestNum, string strUnsGuid)
        {
            foreach (var aVerseData in Verses)
                aVerseData.RemoveRetelling(strUnsGuid);

            // and remove the entry from the 'database' of tests
            CraftingInfo.TestersToCommentsRetellings.RemoveAt(nTestNum);
        }

        public void DeleteAnswerTestResults(int nTestNum, string strUnsGuid)
        {
            // remove the answers to general questions in ln 0
            Verses.FirstVerse.RemoveTestQuestionAnswer(strUnsGuid);

            // then from the rest of the verses
            foreach (var aVerseData in Verses)
                aVerseData.RemoveTestQuestionAnswer(strUnsGuid);

            // and remove the entry from the 'database' of tests
            CraftingInfo.TestersToCommentsTqAnswers.RemoveAt(nTestNum);
        }

        public void MoveCoachNotesToConsultantNotePane()
        {
            Verses.MoveCoachNotesToConsultantNotePane();
        }

        internal void MoveConsultantNotesToCoachNotePane()
        {
            Verses.MoveConsultantNotesToCoachNotePane();
        }

        public void ReassignRolesToConNoteComments()
        {
            // for now, it seems the problem is only in the Consultant Notes pane
            //  (otherwise, add the coach as well)
            Verses.ReassignRolesToConNoteComments(CraftingInfo.ProjectFacilitator,
                                                  CraftingInfo.Consultant);
        }

        public void SwapColumns(StoryEditor.TextFields column1, StoryEditor.TextFields column2, StoryEditor.TextFields fieldsToSwap)
        {
            Verses.SwapColumns(column1, column2, fieldsToSwap);
        }

        internal bool DoesReferenceExist(TeamMemberData theTeamMember)
        {
            var eUserTypes = theTeamMember.MemberType;
            var strMemberGuid = theTeamMember.MemberGuid;

            // don't use switch, since any particular user can have multiple roles
            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.UNS))
            {
                if (CraftingInfo.DoesReferenceUns(strMemberGuid) ||
                    Verses.DoesReferenceUns(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.Crafter))
            {
                if (CraftingInfo.DoesReferenceCrafter(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.ProjectFacilitator))
            {
                if (CraftingInfo.DoesReferenceProjectFacilitator(strMemberGuid) ||
                    Verses.DoesReferenceMemberInConNote(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes,
                TeamMemberData.UserTypes.ConsultantInTraining | 
                TeamMemberData.UserTypes.IndependentConsultant))
            {
                if (CraftingInfo.DoesReferenceConsultant(strMemberGuid) ||
                    Verses.DoesReferenceMemberInConNote(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.Coach))
            {
                if (CraftingInfo.DoesReferenceCoach(strMemberGuid) ||
                    Verses.DoesReferenceMemberInConNote(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.FirstPassMentor))
            {
                if (Verses.DoesReferenceMemberInConNote(strMemberGuid))
                    return true;
            }

            if (TeamMemberData.IsUser(eUserTypes, TeamMemberData.UserTypes.EnglishBackTranslator))
            {
                if (CraftingInfo.DoesReferenceEnglishBackTranslator(strMemberGuid))
                    return true;
            }

            return false;
        }
    }

    public class StoryStateTransitionHistory : List<StoryStateTransition>
    {
        public StoryStateTransitionHistory()
        {
        }

        public StoryStateTransitionHistory(StoryStateTransitionHistory rhs)
        {
            foreach (var state in rhs)
                Add(new StoryStateTransition(state));
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

        private bool _bSuspendBigThrow;
        public StoryStateTransitionHistory(NewDataSet.storyRow theStoryRow)
        {
            NewDataSet.TransitionHistoryRow[] aTHRs = theStoryRow.GetTransitionHistoryRows();
            if (aTHRs.Length == 1)
            {
                NewDataSet.TransitionHistoryRow theTHR = aTHRs[0];
                
                // don't throw in this case, because whatever the cause is, is already done
                _bSuspendBigThrow = true;
                foreach (NewDataSet.StateTransitionRow aSTR in theTHR.GetStateTransitionRows())
                    Add(new StoryStateTransition(aSTR));
                _bSuspendBigThrow = false;
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

        private new void Add(StoryStateTransition transition)
        {
            if (this.Any(t => (t.TransitionDateTime == transition.TransitionDateTime) &&
                              (t.WindowsUserName == transition.WindowsUserName) &&
                              (t.LoggedInMemberId == transition.LoggedInMemberId) &&
                              (t.FromState == transition.FromState) &&
                              (t.ToState == transition.ToState)))
            {
                // don't bother adding duplicates (this shouldn't be able to happen, but it is consistently on
                //  Vijay's (HindiMp team) computer.
                // In the case where we're just *loading* the file, just throw away duplicates (the problem has already
                //  happened so no point is bothering the user at this point)
                if (!_bSuspendBigThrow)
                    throw new DuplicateStoryStateTransitionException("Please click the button to send an email to Bob Eaton (bob_eaton@sall.com) to debug the 'multiple identical StoryStateTransition' error." + transition.GetXml);
                return;
            }
            base.Add(transition);
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

    public class DuplicateStoryStateTransitionException : ApplicationException
    {
        public DuplicateStoryStateTransitionException(string strMessage) : base(strMessage)
        {
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

        public StoryStateTransition(StoryStateTransition rhs)
        {
            LoggedInMemberId = rhs.LoggedInMemberId;
            FromState = rhs.FromState;
            ToState = rhs.ToState;
            TransitionDateTime = rhs.TransitionDateTime;
            WindowsUserName = rhs.WindowsUserName;
        }

        public StoryStateTransition(NewDataSet.StateTransitionRow theSTR)
        {
            LoggedInMemberId = theSTR.LoggedInMemberId;
            FromState = StoryStageLogic.GetProjectStageFromString(theSTR.FromState);
            ToState = StoryStageLogic.GetProjectStageFromString(theSTR.ToState);
            TransitionDateTime = theSTR.TransitionDateTime.ToLocalTime();
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
            TransitionDateTime = DateTime.Parse(node.Attributes[CstrAttrNameTransitionDateTime].Value).ToLocalTime();
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
                var elem = new XElement(CstrElemLabelStateTransition,
                                        new XAttribute(CstrAttrNameLoggedInMemberId, LoggedInMemberId),
                                        new XAttribute(CstrAttrNameFromState, FromState.ToString().Substring(1)),
                                        new XAttribute(CstrAttrNameToState, ToState.ToString().Substring(1)),
                                        new XAttribute(CstrAttrNameTransitionDateTime,
                                                       StoryData.ToUniversalTime(TransitionDateTime)));
                
                // this one might be null
                if (!String.IsNullOrEmpty(WindowsUserName))
                    elem.Add(new XAttribute(CstrAttrNameWindowsUserName, WindowsUserName));

                return elem;
            }
        }
    }
    
    public class MemberIdInfo
    {
        public const string CstrAttributeMemberID = "memberID";

        public string MemberId { get; set; }
        public string MemberComment { get; set; }

        public MemberIdInfo(string strTesterGuid, string strTestComment)
        {
            MemberId = strTesterGuid;
            MemberComment = strTestComment;
        }

        public MemberIdInfo(MemberIdInfo rhs)
        {
            MemberId = rhs.MemberId;
            MemberComment = rhs.MemberComment;
        }

        public static MemberIdInfo CreateFromXmlNode(XmlNode node)
        {
            XmlAttribute attr;
            if ((node == null) ||
                (node.Attributes == null) ||
                ((attr = node.Attributes[CstrAttributeMemberID]) == null))
                return null;

            return new MemberIdInfo(attr.Value, node.InnerText);
        }

        public void WriteXml(string strElementLabel, XElement elem)
        {
            elem.Add(new XElement(strElementLabel,
                                  new XAttribute(CstrAttributeMemberID, MemberId),
                                  MemberComment ?? String.Empty));  // merger now puts full closing tags, so do that here too, so we don't get false diffs
        }

        public bool IsConfigured
        {
            get { return !String.IsNullOrEmpty(MemberId); }
        }

        public static bool Configured(MemberIdInfo memberIdInfo)
        {
            return ((memberIdInfo != null) && memberIdInfo.IsConfigured);
        }

        public static string SafeGetMemberId(MemberIdInfo member)
        {
            return Configured(member) ? member.MemberId : null;
        }

        /// <summary>
        /// checks if the given memberIdInfo is configured or not and if not creates it.
        /// Then it checks whether it's MemberId member has been set and if not (or if
        /// it's being 'forced') sets it. NOTE: if bForceSet is false and the MemberId
        /// is already set, then no change is made.
        /// </summary>
        /// <param name="memberIdInfo"></param>
        /// <param name="strMemberId"></param>
        /// <param name="bForceSet"></param>
        public static void SetCreateIfEmpty(ref MemberIdInfo memberIdInfo, 
            string strMemberId, bool bForceSet)
        {
            if (!Configured(memberIdInfo))
                memberIdInfo = new MemberIdInfo(strMemberId, null);
            else if (!memberIdInfo.IsConfigured || bForceSet)
                memberIdInfo.MemberId = strMemberId;
        }

        public static string PresentationHtmlRow(TeamMembersData teamMembers, 
            string strLabel, MemberIdInfo parent, MemberIdInfo child, bool bIsPrinting)
        {
            Debug.Assert(teamMembers != null);
            if (!Configured(parent) && !Configured(child))
                return null;    // not configured yet

            string strName = null;
            string strComment = null;
            if (Configured(parent))
            {
                Debug.Assert(!String.IsNullOrEmpty(parent.MemberId));
                string strParentName = teamMembers.GetNameFromMemberId(parent.MemberId);
                if (!Configured(child))
                {
                    if (bIsPrinting)
                    {
                        // means there never was a child to compare with
                        strName = strParentName;
                        strComment = parent.MemberComment;
                    }
                    else
                    {
                        // there's no child, because it was deleted
                        strName = Diff.HtmlDiff(strParentName, null, true);
                        strComment = Diff.HtmlDiff(parent.MemberComment, null, true);
                    }
                }
                else
                {
                    // there was a child and we have to compare against it
                    strName = Diff.HtmlDiff(strParentName,
                                            teamMembers.GetNameFromMemberId(child.MemberId), true);
                    strComment = Diff.HtmlDiff(parent.MemberComment, child.MemberComment, true);
                }
            }
            else if (child.IsConfigured)
            {
                // no parent, but child present means it was added.
                strName = Diff.HtmlDiff(null, teamMembers.GetNameFromMemberId(child.MemberId), true);
                strComment = Diff.HtmlDiff(null, child.MemberComment, true);
            }

            return String.IsNullOrEmpty(strComment)
                       ? PresentationHtml(strLabel, strName)
                       : PresentationHtml(strLabel, strName, strComment);
        }

        public static string PresentationHtml(string strLabel, string strName, string strComment)
        {
            if (String.IsNullOrEmpty(strName))
                strName = "-";  // so it's not nothing (or the HTML shows without a cell frame)

            return String.Format(Properties.Resources.HTML_TableRow,
                                 String.Format(Properties.Resources.HTML_TableCellNoWrap,
                                               strLabel) +
                                 String.Format(Properties.Resources.HTML_TableCellWidth,
                                               40,
                                               String.Format(
                                                   Properties.Resources.HTML_ParagraphTextId,
                                                   strLabel,
                                                   StoryData.CstrLangInternationalBtStyleClassName,
                                                   strName)) +
                                 String.Format(Properties.Resources.HTML_TableCellWidth,
                                               60,
                                               String.Format(
                                                   Properties.Resources.HTML_ParagraphTextId,
                                                   strLabel + "_Comment",
                                                   StoryData.CstrLangInternationalBtStyleClassName,
                                                   strComment)));
        }

        public static string PresentationHtml(string strLabel, string strName)
        {
            if (String.IsNullOrEmpty(strName))
                strName = "-";  // so it's not nothing (or the HTML shows without a cell frame)

            return String.Format(Properties.Resources.HTML_TableRow,
                                 String.Format(Properties.Resources.HTML_TableCellNoWrap,
                                               strLabel) +
                                 String.Format(
                                     Properties.Resources.HTML_TableCellWithSpanAndWidth,
                                     100,
                                     2,
                                     String.Format(Properties.Resources.HTML_ParagraphTextId,
                                                   strLabel,
                                                   StoryData.
                                                       CstrLangInternationalBtStyleClassName,
                                                   strName)));
        }
    }

    public class TestInfo : List<MemberIdInfo>
    {
        public List<string> ListOfMemberIdsToDisplay { get; set; }

        public TestInfo()
        {
            
        }

        public TestInfo(IEnumerable<MemberIdInfo> rhs)
        {
            foreach (var rhsMi in rhs)
                Add(new MemberIdInfo(rhsMi));
        }

        public bool Contains(string strGuid)
        {
            return this.Any(testerInfo => testerInfo.MemberId == strGuid);
        }

        public int IndexOf(string strGuid)
        {
            for (int i = 0; i < Count; i++)
            {
                var testerInfo = this[i];
                if (testerInfo.MemberId == strGuid)
                    return i;
            }
            return -1;
        }

        public void Add(XmlNode node, string strCollectionElement, string strElementLabel)
        {
            XmlNodeList list = node.SelectNodes(String.Format("{0}/{1}",
                strCollectionElement, strElementLabel));
            if (list != null)
                foreach (var memberIdInfo in
                    list.Cast<XmlNode>().Select(nodeTest => 
                        MemberIdInfo.CreateFromXmlNode(nodeTest)).Where(memberIdInfo => 
                            memberIdInfo != null))
                {
                    Add(memberIdInfo);
                }
        }

        public void WriteXml(string strCollectionLabel, string strElementLabel, XElement elemCraftingInfo)
        {
            var elemTesters = new XElement(strCollectionLabel);
            foreach (MemberIdInfo testerInfo in this)
                testerInfo.WriteXml(strElementLabel, elemTesters);
            elemCraftingInfo.Add(elemTesters);
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            int nIndex = IndexOf(strOldUnsGuid);
            if (nIndex != -1)
            {
                var tester = this[nIndex];
                tester.MemberId = strNewUnsGuid;
            }
        }

        public string PresentationHtml(TeamMembersData teamMembers, string strLabelFormat, 
            TestInfo child)
        {
            string strRow = null;
            int i = 1;
            for (; i <= Count; i++)
            {
                var parentTester = this[i - 1];

                // get the child tests that match this one (since we now allow users to
                //  reassign UNSs to a particular test, the only thing that makes sense
                //  is a child of the same test number (i.e. same index; not memberId)
                // var childMatch = FindChildEquivalentTesterInChild(parentTester, child);
                var childMatch = ((child != null) && (child.Count >= i))
                                     ? child[i - 1]
                                     : null;

                strRow += MemberIdInfo.PresentationHtmlRow(teamMembers,
                                                           String.Format(strLabelFormat, i), 
                                                           parentTester, childMatch,
                                                           (child == null));
            }

            if (child != null)
                while (i <= child.Count)
                {
                    var childTester = child[i - 1];
                    strRow += MemberIdInfo.PresentationHtmlRow(teamMembers,
                        String.Format(strLabelFormat, i++), null, childTester, true);
                }

            return strRow;
        }
    }

    public class CraftingInfoData
    {
        public MemberIdInfo ProjectFacilitator;
        public MemberIdInfo Consultant;
        public MemberIdInfo Coach;
        public MemberIdInfo StoryCrafter;
        public MemberIdInfo BackTranslator;
        public MemberIdInfo OutsideEnglishBackTranslator;
        public string StoryPurpose;
        public string ResourcesUsed;
        public string MiscellaneousStoryInfo;
        public TestInfo TestersToCommentsRetellings = new TestInfo();
        public TestInfo TestersToCommentsTqAnswers = new TestInfo();
        public bool IsBiblicalStory = true;

        public CraftingInfoData(string strCrafterMemberGuid, string strLoggedOnMemberGuid, bool bIsBiblicalStory)
        {
            StoryCrafter = new MemberIdInfo(strCrafterMemberGuid, null);
            ProjectFacilitator = new MemberIdInfo(strLoggedOnMemberGuid, null);
            IsBiblicalStory = bIsBiblicalStory;
        }

        public CraftingInfoData(XmlNode node)
        {
            StoryCrafter = MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelStoryCrafter));
            ProjectFacilitator = MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelProjectFacilitator));
            Consultant = MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelConsultant));
            Coach = MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelCoach));
            BackTranslator = MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelBackTranslator));
            OutsideEnglishBackTranslator =
                MemberIdInfo.CreateFromXmlNode(node.SelectSingleNode(CstrElementLabelOutsideEnglishBackTranslator));

            XmlNode elem;
            StoryPurpose = ((elem = node.SelectSingleNode(CstrElementLabelStoryPurpose)) != null) 
                ? elem.InnerText 
                : null;
            ResourcesUsed = ((elem = node.SelectSingleNode(CstrElementLabelResourcesUsed)) != null)
                ? elem.InnerText
                : null;
            MiscellaneousStoryInfo = ((elem = node.SelectSingleNode(CstrElementLabelMiscellaneousStoryInfo)) != null)
                ? elem.InnerText
                : null;
            TestersToCommentsRetellings.Add(node, CstrElementLabelTestsRetellings, CstrElementLabelTestRetelling);
            TestersToCommentsTqAnswers.Add(node, CstrElementLabelTestsTqAnswers, CstrElementLabelTestTqAnswer);
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
                {
                    var theRow = aSCRs[0];
                    StoryCrafter = new MemberIdInfo(theRow.memberID,
                                                    (theRow.IsStoryCrafter_textNull())
                                                        ? null
                                                        : theRow.StoryCrafter_text);
                }
                else
                    throw new ApplicationException(Properties.Resources.IDS_ProjectFileCorrupted);

                NewDataSet.ProjectFacilitatorRow[] thePfRows = theCIR.GetProjectFacilitatorRows();
                if (thePfRows.Length == 1)
                {
                    var theRow = thePfRows[0];
                    ProjectFacilitator = new MemberIdInfo(theRow.memberID,
                                                          (theRow.IsProjectFacilitator_textNull())
                                                              ? null
                                                              : theRow.ProjectFacilitator_text);
                }

                NewDataSet.ConsultantRow[] theCoRows = theCIR.GetConsultantRows();
                if (theCoRows.Length == 1)
                {
                    var theRow = theCoRows[0];
                    Consultant = new MemberIdInfo(theRow.memberID,
                                                  (theRow.IsConsultant_textNull())
                                                      ? null
                                                      : theRow.Consultant_text);
                }

                NewDataSet.CoachRow[] theCchRows = theCIR.GetCoachRows();
                if (theCchRows.Length == 1)
                {
                    var theRow = theCchRows[0];
                    Coach = new MemberIdInfo(theRow.memberID,
                                                          (theRow.IsCoach_textNull())
                                                              ? null
                                                              : theRow.Coach_text);
                }

                NewDataSet.BackTranslatorRow[] aBTRs = theCIR.GetBackTranslatorRows();
                if (aBTRs.Length == 1)
                {
                    var theRow = aBTRs[0];
                    BackTranslator = new MemberIdInfo(theRow.memberID,
                                                      (theRow.IsBackTranslator_textNull())
                                                          ? null
                                                          : theRow.BackTranslator_text);
                }

                NewDataSet.OutsideEnglishBackTranslatorRow[] aOEBTers = theCIR.GetOutsideEnglishBackTranslatorRows();
                if (aOEBTers.Length == 1)
                {
                    var theRow = aOEBTers[0];
                    OutsideEnglishBackTranslator = new MemberIdInfo(theRow.memberID,
                                                      (theRow.IsOutsideEnglishBackTranslator_textNull())
                                                          ? null
                                                          : theRow.OutsideEnglishBackTranslator_text);
                }

                if (!theCIR.IsStoryPurposeNull())
                    StoryPurpose = theCIR.StoryPurpose;

                if (!theCIR.IsResourcesUsedNull())
                    ResourcesUsed = theCIR.ResourcesUsed;

                if (!theCIR.IsMiscellaneousStoryInfoNull())
                    MiscellaneousStoryInfo = theCIR.MiscellaneousStoryInfo;

                NewDataSet.TestsRetellingsRow[] aTsReRs = theCIR.GetTestsRetellingsRows();
                if (aTsReRs.Length == 1)
                {
                    NewDataSet.TestRetellingRow[] aTReRs = aTsReRs[0].GetTestRetellingRows();
                    foreach (NewDataSet.TestRetellingRow aTReR in aTReRs)
                        TestersToCommentsRetellings.Add(new MemberIdInfo(aTReR.memberID,
                                                                       (aTReR.IsTestRetelling_textNull())
                                                                           ? null
                                                                           : aTReR.TestRetelling_text));
                }

                NewDataSet.TestsTqAnswersRow[] aTsAnRs = theCIR.GetTestsTqAnswersRows();
                if (aTsAnRs.Length == 1)
                {
                    NewDataSet.TestTqAnswerRow[] aTReRs = aTsAnRs[0].GetTestTqAnswerRows();
                    foreach (NewDataSet.TestTqAnswerRow aTReR in aTReRs)
                        TestersToCommentsTqAnswers.Add(new MemberIdInfo(aTReR.memberID,
                                                                       (aTReR.IsTestTqAnswer_textNull())
                                                                           ? null
                                                                           : aTReR.TestTqAnswer_text));
                }
            }
            else
                throw new ApplicationException(Properties.Resources.IDS_ProjectFileCorruptedNoCraftingInfo);
        }

        public CraftingInfoData(CraftingInfoData rhs)
        {
            if (MemberIdInfo.Configured(rhs.StoryCrafter))
                StoryCrafter = new MemberIdInfo(rhs.StoryCrafter);

            if (MemberIdInfo.Configured(rhs.ProjectFacilitator))
                ProjectFacilitator = new MemberIdInfo(rhs.ProjectFacilitator);

            if (MemberIdInfo.Configured(rhs.Consultant))
                Consultant = new MemberIdInfo(rhs.Consultant);

            if (MemberIdInfo.Configured(rhs.Coach))
                Coach = new MemberIdInfo(rhs.Coach);

            if (MemberIdInfo.Configured(rhs.BackTranslator))
                BackTranslator = new MemberIdInfo(rhs.BackTranslator);

            if (MemberIdInfo.Configured(rhs.OutsideEnglishBackTranslator))
                OutsideEnglishBackTranslator = new MemberIdInfo(rhs.OutsideEnglishBackTranslator);

            StoryPurpose = rhs.StoryPurpose;
            ResourcesUsed = rhs.ResourcesUsed;
            MiscellaneousStoryInfo = rhs.MiscellaneousStoryInfo;
            IsBiblicalStory = rhs.IsBiblicalStory;
            TestersToCommentsRetellings = new TestInfo(rhs.TestersToCommentsRetellings);
            TestersToCommentsTqAnswers = new TestInfo(rhs.TestersToCommentsTqAnswers);
        }

        public const string CstrElementLabelCraftingInfo = "CraftingInfo";
        public const string CstrElementLabelNonBiblicalStory = "NonBiblicalStory";
        public const string CstrElementLabelStoryCrafter = "StoryCrafter";
        public const string CstrElementLabelProjectFacilitator = "ProjectFacilitator";
        public const string CstrElementLabelConsultant = "Consultant";
        public const string CstrElementLabelCoach = "Coach";
        public const string CstrElementLabelStoryPurpose = "StoryPurpose";
        public const string CstrElementLabelResourcesUsed = "ResourcesUsed";
        public const string CstrElementLabelMiscellaneousStoryInfo = "MiscellaneousStoryInfo";
        public const string CstrElementLabelBackTranslator = "BackTranslator";
        public const string CstrElementLabelOutsideEnglishBackTranslator = "OutsideEnglishBackTranslator";
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
                                                         new XAttribute(CstrElementLabelNonBiblicalStory, !IsBiblicalStory));

                System.Diagnostics.Debug.Assert(MemberIdInfo.Configured(StoryCrafter));
                StoryCrafter.WriteXml(CstrElementLabelStoryCrafter, elemCraftingInfo);

                if (MemberIdInfo.Configured(ProjectFacilitator))
                    ProjectFacilitator.WriteXml(CstrElementLabelProjectFacilitator,
                                                elemCraftingInfo);

                if (MemberIdInfo.Configured(Consultant))
                    Consultant.WriteXml(CstrElementLabelConsultant,
                                        elemCraftingInfo);

                if (MemberIdInfo.Configured(Coach))
                    Coach.WriteXml(CstrElementLabelCoach,
                                   elemCraftingInfo);

                if (MemberIdInfo.Configured(BackTranslator))
                    BackTranslator.WriteXml(CstrElementLabelBackTranslator,
                                            elemCraftingInfo);

                if (MemberIdInfo.Configured(OutsideEnglishBackTranslator))
                    OutsideEnglishBackTranslator.WriteXml(CstrElementLabelOutsideEnglishBackTranslator,
                                                          elemCraftingInfo);

                if (!String.IsNullOrEmpty(StoryPurpose))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelStoryPurpose, StoryPurpose));

                if (!String.IsNullOrEmpty(ResourcesUsed))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelResourcesUsed, ResourcesUsed));

                if (!String.IsNullOrEmpty(MiscellaneousStoryInfo))
                    elemCraftingInfo.Add(new XElement(CstrElementLabelMiscellaneousStoryInfo, MiscellaneousStoryInfo));

                if (TestersToCommentsRetellings.Count > 0)
                {
                    TestersToCommentsRetellings.WriteXml(CstrElementLabelTestsRetellings,
                                                         CstrElementLabelTestRetelling,
                                                         elemCraftingInfo);
                }

                if (TestersToCommentsTqAnswers.Count > 0)
                {
                    TestersToCommentsTqAnswers.WriteXml(CstrElementLabelTestsTqAnswers,
                                                        CstrElementLabelTestTqAnswer,
                                                        elemCraftingInfo);
                }

                return elemCraftingInfo;
            }
        }

        public string PresentationHtml(TeamMembersData teamMembers, CraftingInfoData child)
        {
            string strRow = null;
            bool bIsPrinting = (child == null); // child doesn't exist means printing
            strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("Story Crafter"), StoryCrafter,
                                                       (bIsPrinting)
                                                           ? null
                                                           : child.StoryCrafter,
                                                       bIsPrinting);
            strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("Project Facilitator"), ProjectFacilitator,
                                                       (bIsPrinting)
                                                           ? null
                                                           : child.ProjectFacilitator,
                                                       bIsPrinting);
            strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("Consultant"), Consultant,
                                                       (bIsPrinting)
                                                           ? null
                                                           : child.Consultant,
                                                       bIsPrinting);
            if (Coach != null)
                strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("Coach"), Coach,
                                                           (bIsPrinting)
                                                               ? null
                                                               : child.Coach,
                                                           bIsPrinting);
            strRow += PresentationHtmlRow(Localizer.Str("Story Purpose"), StoryPurpose,
                                          (bIsPrinting)
                                              ? null
                                              : child.StoryPurpose,
                                          bIsPrinting);
            strRow += PresentationHtmlRow(Localizer.Str("Resources Used"), ResourcesUsed,
                                          (bIsPrinting)
                                              ? null
                                              : child.ResourcesUsed,
                                          bIsPrinting);
            strRow += PresentationHtmlRow(Localizer.Str("Miscellaneous Comments"), MiscellaneousStoryInfo,
                                          (bIsPrinting)
                                              ? null
                                              : child.MiscellaneousStoryInfo,
                                          bIsPrinting);
            strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("Back Translator"), BackTranslator,
                                                       (bIsPrinting)
                                                           ? null
                                                           : child.BackTranslator,
                                                       bIsPrinting);
            strRow += MemberIdInfo.PresentationHtmlRow(teamMembers, Localizer.Str("English Back-Translator"), OutsideEnglishBackTranslator,
                                                       (bIsPrinting)
                                                           ? null
                                                           : child.OutsideEnglishBackTranslator,
                                                       bIsPrinting);
            
            strRow += TestersToCommentsRetellings.PresentationHtml(teamMembers,
                                                                   Localizer.Str("Retelling tester {0}"),
                                                                   ((child != null) &&
                                                                    (child.TestersToCommentsRetellings != null)
                                                                        ? child.TestersToCommentsRetellings
                                                                        : null));

            strRow += TestersToCommentsTqAnswers.PresentationHtml(teamMembers,
                                                                   Localizer.Str("Story question tester {0}"),
                                                                   ((child != null) &&
                                                                    (child.TestersToCommentsTqAnswers != null)
                                                                        ? child.TestersToCommentsTqAnswers
                                                                        : null));

            return strRow;
        }


        public static string PresentationHtmlRow(string strLabel, 
            string strParentComment, string strChildComment, bool bIsPrinting)
        {
            string strName = (bIsPrinting)
                                 ? strParentComment
                                 : Diff.HtmlDiff(strParentComment, strChildComment, true);
            return MemberIdInfo.PresentationHtml(strLabel, strName);
        }

        public void ReplaceUns(string strOldMemberGuid, string strNewMemberGuid)
        {
            // before we do *any* changing, we have to verify that this isn't going 
            //  to put us in a bad situation: the same UNS twice for the same story
            if (TestersToCommentsRetellings.Contains(strNewMemberGuid)
                || TestersToCommentsTqAnswers.Contains(strNewMemberGuid))
            {
                throw new StoryProjectData.ReplaceMemberException
                          {
                              MemberGuid = strNewMemberGuid,
                              Format = Localizer.Str("the UNS '{0}' is already used. That would result in the same UNS being used for two different tests. To correct this, go to that story and click 'Story', 'Story Information' and change the test associated with '{0}' to someone else and try to merge them again")
                          };
            }

            if (MemberIdInfo.SafeGetMemberId(BackTranslator) == strOldMemberGuid)
                MemberIdInfo.SetCreateIfEmpty(ref BackTranslator, strNewMemberGuid, true);
            TestersToCommentsRetellings.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
            TestersToCommentsTqAnswers.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            if (MemberIdInfo.SafeGetMemberId(StoryCrafter) == strOldMemberGuid)
                MemberIdInfo.SetCreateIfEmpty(ref StoryCrafter, strNewMemberGuid, true);
        }

        public string ReplaceProjectFacilitator(string strNewMemberGuid)
        {
            string strOldId = MemberIdInfo.SafeGetMemberId(ProjectFacilitator);
            MemberIdInfo.SetCreateIfEmpty(ref ProjectFacilitator, strNewMemberGuid, true);
            return strOldId;
        }

        public string ReplaceConsultant(string strNewMemberGuid)
        {
            string strOldId = MemberIdInfo.SafeGetMemberId(Consultant);
            MemberIdInfo.SetCreateIfEmpty(ref Consultant, strNewMemberGuid, true);
            return strOldId;
        }

        public string ReplaceCoach(string strNewMemberGuid)
        {
            string strOldId = MemberIdInfo.SafeGetMemberId(Coach);
            MemberIdInfo.SetCreateIfEmpty(ref Coach, strNewMemberGuid, true);
            return strOldId;
        }

        public bool DoesReferenceUns(string strMemberGuid)
        {
            return TestersToCommentsRetellings.Contains(strMemberGuid) ||
                   TestersToCommentsTqAnswers.Contains(strMemberGuid) ||
                   (MemberIdInfo.SafeGetMemberId(BackTranslator) == strMemberGuid);
        }

        public bool DoesReferenceCrafter(string strMemberGuid)
        {
            return (MemberIdInfo.SafeGetMemberId(StoryCrafter) == strMemberGuid);
        }

        public bool DoesReferenceProjectFacilitator(string strMemberGuid)
        {
            return (MemberIdInfo.SafeGetMemberId(ProjectFacilitator) == strMemberGuid);
        }

        public bool DoesReferenceConsultant(string strMemberGuid)
        {
            return (MemberIdInfo.SafeGetMemberId(Consultant) == strMemberGuid);
        }

        public bool DoesReferenceCoach(string strMemberGuid)
        {
            return (MemberIdInfo.SafeGetMemberId(Coach) == strMemberGuid);
        }

        public bool DoesReferenceEnglishBackTranslator(string strMemberGuid)
        {
            return (MemberIdInfo.SafeGetMemberId(OutsideEnglishBackTranslator) == strMemberGuid);
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
            foreach (var aStoryRow in theStoriesRow.GetstoryRows())
            {
                // create the story object
                var theNewStory = new StoryData(aStoryRow, projFile, strProjectFolder);

                // make sure it doesn't have a name the same as an existing one... (it could come in via the merge)
                var n = 1;
                var strName = theNewStory.Name;
                while (Contains(theNewStory))
                    theNewStory.Name = String.Format("{0}.{1}", strName, n++);

                Add(theNewStory);
            }
        }

        public new bool Contains(StoryData theSD)
        {
            return this.Any(aSd => aSd.Name == theSD.Name);
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
            foreach (var storyData in
                this.Where(storyData => 
                    MemberIdInfo.SafeGetMemberId(storyData.CraftingInfo.ProjectFacilitator) == strOldMemberGuid))
            {
                storyData.ReplaceProjectFacilitator(strNewMemberGuid);
            }
        }

        public void ReplaceCrafter(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storyData in this)
                storyData.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceConsultant(string strOldMemberGuid, string strNewMemberGuid)
        {
#if false
            foreach (var storyData in
                this.Where(storyData =>
                    MemberIdInfo.SafeGetMemberId(storyData.CraftingInfo.Consultant) == strOldMemberGuid))
#else
            // the reason the above didn't work is this sequence of events: 
            //  a) Consultant A makes comments and changes state multiple times.
            //  b) Consultant B takes ownership of the story (which doesn't change any of the above settings)
            //  c) Consultant B then tries to merge Consultant A into B, but since this wasn't Consultant A's story anymore
            //      the above logic would cause us to skip this one.
            // Solution: when merging, just check every story for any remnents of Consultant A and change it to B.
            foreach (var storyData in this)
#endif
            {
                storyData.ReplaceConsultant(strOldMemberGuid, strNewMemberGuid);
            }
        }

        public void ReplaceCoach(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storyData in
                this.Where(storyData =>
                    MemberIdInfo.SafeGetMemberId(storyData.CraftingInfo.Coach) == strOldMemberGuid))
            {
                storyData.ReplaceCoach(strNewMemberGuid);
            }
        }

        public bool DoesReferenceExist(TeamMemberData theTeamMember)
        {
            return this.Any(data => data.DoesReferenceExist(theTeamMember));
        }

        public void SetCommentMemberId(string strConsultant, string strCoach)
        {
            foreach (var storyData in this)
            {
                if (!String.IsNullOrEmpty(strConsultant))
                {
                    MemberIdInfo.SetCreateIfEmpty(ref storyData.CraftingInfo.Consultant,
                                                  strConsultant, 
                                                  false);
                }

                if (!String.IsNullOrEmpty(strCoach))
                {
                    MemberIdInfo.SetCreateIfEmpty(ref storyData.CraftingInfo.Coach,
                                                  strCoach,
                                                  false);
                }

                storyData.SetCommentMemberId();
            }
        }

        public bool Contains(string strStoryName)
        {
            return this.Any(aStory => aStory.Name == strStoryName);
        }
    }

    public class StoryProjectData : OrderedDictionary   // <string, StoriesData>
    {
        public TeamMembersData TeamMembers;
        public ProjectSettings ProjSettings;
        public LnCNotesData LnCNotes;
        public OsMetaDataModel OsMetaData;
        public string PanoramaFrontMatter;
        public string XmlDataVersion = "1.6";
        private const string CxmlDataVersionReferringText = "1.7";
        private const string CxmlDataVersionStickyNote = "1.8";

        private const string CxmlOsMetaDataVersion = "1.0";

        // added in support of acquiring OsMetaData via OSE
        private const string CstrOsMetaDataFilename = "OsMetaData.xml";
        public const string CstrOsMetaDataStatusExploratory = "3. Exploratory Stage";
        public const string CstrOsMetaDataStatusProduction = "4. Production Stage";
        private const string CstrOsMetaDataStatusSetComplete = "5. Set Complete";

        /// <summary>
        /// This version of the constructor should *always* be followed by a call to InitializeProjectSettings()
        /// </summary>
        public StoryProjectData()
        {
            TeamMembers = new TeamMembersData();
            LnCNotes = new LnCNotesData();
            PanoramaFrontMatter = Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            // start with to stories sets (the current one and the obsolete ones)
            Add(Properties.Resources.IDS_MainStoriesSet, new StoriesData(Properties.Resources.IDS_MainStoriesSet));
            Add(Properties.Resources.IDS_NonBibStoriesSet, new StoriesData(Properties.Resources.IDS_NonBibStoriesSet));
            Add(Properties.Resources.IDS_ObsoleteStoriesSet, new StoriesData(Properties.Resources.IDS_ObsoleteStoriesSet));
        }

        public new StoriesData this[int key]
        {
            get { return (StoriesData)base[key]; }
        }

        public new StoriesData this[object key]
        {
            get { return (StoriesData)base[key]; }
        }

        public new List<StoriesData> Values
        {
            get
            {
                var list = new List<StoriesData>();
                foreach (StoriesData item in base.Values)
                    list.Add(item);
                return list;
            }
        }

        public bool ContainsKey(string key)
        {
            foreach (string _key in Keys)
                if (_key == key)
                    return true;
            return false;
        }

        private static bool _bProjectConvertWarnedOnce;
        private static bool _bStopNaggingPf;

        public StoryProjectData(NewDataSet projFile, ProjectSettings projSettings)
        {
            // this version comes with a project settings object
            ProjSettings = projSettings;

            // if the project file we opened doesn't have anything yet.. (shouldn't really happen)
            if (projFile.StoryProject.Count == 0)
            {
                projFile.StoryProject.AddStoryProjectRow(XmlDataVersion,
                                                         ProjSettings.ProjectName,
                                                         Properties.Resources.
                                                             IDS_DefaultPanoramaFrontMatter,
                                                         ProjSettings.UseDropbox,
                                                         ProjSettings.DropboxStory,
                                                         ProjSettings.DropboxRetelling,
                                                         ProjSettings.DropboxAnswers);
            }
            else
            {
                projFile.StoryProject[0].ProjectName = ProjSettings.ProjectName; // in case the user changed it.

                if (projFile.StoryProject[0].version.CompareTo("1.3") == 0)
                {
                    // see if the user wants us to upgrade this one
                    if (LocalizableMessageBox.Show(String.Format(Properties.Resources.IDS_QueryConvertProjectFile1_3to1_4,
                        ProjSettings.ProjectName), StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                        throw BackOutWithNoUI;

                    // convert the 1.3 file to 1.4 using xslt
                    _bProjectConvertWarnedOnce = true;
                    ConvertProjectFile1_3_to_1_4(ProjSettings.ProjectFilePath);
                }

                else if (projFile.StoryProject[0].version.CompareTo("1.4") == 0)
                {
                    // see if the user wants us to upgrade this one
                    if (!_bProjectConvertWarnedOnce)
                        if (LocalizableMessageBox.Show(String.Format(Properties.Resources.IDS_QueryConvertProjectFile1_3to1_4,
                            ProjSettings.ProjectName), StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            throw BackOutWithNoUI;

                    // convert the 1.3 file to 1.4 using xslt
                    _bProjectConvertWarnedOnce = true;
                    ConvertProjectFile1_4_to_1_5(ProjSettings.ProjectFilePath);
                }

                else if (projFile.StoryProject[0].version.CompareTo(XmlDataVersion) > 0)
                {
                    if ((projFile.StoryProject[0].version != CxmlDataVersionReferringText) &&
                        (projFile.StoryProject[0].version != CxmlDataVersionStickyNote))
                    {
                        LocalizableMessageBox.Show(Localizer.Str("One of the team members is using a newer version of OSE to edit the file, which is not compatible with the version you are using. You might try, \"Advanced\", \"Program Updates\", \"Check now\" or \"Check now for next major update\" or you may have to go to the http://palaso.org/install/onestory website and download and install the new version of the program in the \"Setup OneStory Editor.zip\" file"), StoryEditor.OseCaption);
                        throw BackOutWithNoUI;
                    }
                }
            }

            PanoramaFrontMatter = projFile.StoryProject[0].PanoramaFrontMatter;
            if (String.IsNullOrEmpty(PanoramaFrontMatter))
                PanoramaFrontMatter = Properties.Resources.IDS_DefaultPanoramaFrontMatter;

            ProjSettings.UseDropbox = !projFile.StoryProject[0].IsUseDropboxNull() &&
                                      projFile.StoryProject[0].UseDropbox;

            ProjSettings.DropboxStory = !projFile.StoryProject[0].IsDropboxStoryNull() &&
                                        projFile.StoryProject[0].DropboxStory;

            ProjSettings.DropboxRetelling = !projFile.StoryProject[0].IsDropboxRetellingsNull() &&
                                            projFile.StoryProject[0].DropboxRetellings;

            ProjSettings.DropboxAnswers = !projFile.StoryProject[0].IsDropboxAnswersNull() &&
                                          projFile.StoryProject[0].DropboxAnswers;

            if (projFile.stories.Count == 0)
            {
                projFile.stories.AddstoriesRow(Properties.Resources.IDS_MainStoriesSet,
                    projFile.StoryProject[0]);
                projFile.stories.AddstoriesRow(Properties.Resources.IDS_ObsoleteStoriesSet,
                    projFile.StoryProject[0]);
            }
            // new 'non-biblical' stories set added in 2.4 
            // UPDATE (2/11/20): unless it's already there -- see trio-mina rev 91
            else if ((projFile.stories.Count == 2) &&
                     !projFile.StoryProject[0].GetstoriesRows().Any(s => s.SetName == Properties.Resources.IDS_NonBibStoriesSet))
            {
                projFile.stories.AddstoriesRow(Properties.Resources.IDS_NonBibStoriesSet, projFile.StoryProject[0]);
            }
            TeamMembers = new TeamMembersData(projFile);
            ProjSettings.SerializeProjectSettings(projFile);
            LnCNotes = new LnCNotesData(projFile);

            // finally, if it's not new, then it might (should) have stories as well
            foreach (NewDataSet.storiesRow aStoriesRow in projFile.StoryProject[0].GetstoriesRows())
                Add(aStoriesRow.SetName, new StoriesData(aStoriesRow,
                                                         projFile,
                                                         ProjSettings.ProjectFolder));

            if (projFile.StoryProject[0].version.CompareTo("1.5") == 0)
                CheckForCommentMemberIds();

            OsMetaData = LoadOsMetaData();
        }

        public void SaveProjectMetaData(TeamMemberData loggedOnMember)
        {
            System.Diagnostics.Debug.Assert(OsMetaData != null);
            var storySet = this[Properties.Resources.IDS_MainStoriesSet];
            var record = OsMetaData.OsProjects.First();

            switch (record.Status)
            {
                case CstrOsMetaDataStatusExploratory:
                    record.EsConsultant = GetListOfMemberAtRole(storySet, TeamMembers,
                        (story) => story.CraftingInfo.Consultant);
                    record.EsCoach = GetListOfMemberAtRole(storySet, TeamMembers,
                        (story) => story.CraftingInfo.Coach);

                    record.EsStoriesSent =
                        storySet.Count(
                            story =>
                                story.TransitionHistory.Any(
                                    th =>
                                        StoryStageLogic.WhoseTurn(th.ToState) ==
                                        TeamMemberData.UserTypes.ConsultantInTraining));
                    break;

                case CstrOsMetaDataStatusProduction:
                    record.PsConsultant = GetListOfMemberAtRole(storySet, TeamMembers,
                        (story) => story.CraftingInfo.Consultant);
                    record.PsCoach = GetListOfMemberAtRole(storySet, TeamMembers,
                        (story) => story.CraftingInfo.Coach);
                    record.PsStoriesPrelimApprov =
                        storySet.Count(story => story.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamComplete);
                    break;

            }

            record.ProjectFacilitators = GetListOfMemberAtRole(storySet, TeamMembers,
                        (story) => story.CraftingInfo.ProjectFacilitator);

            record.NumInFinalApprov =
                storySet.Count(story => story.ProjStage.ProjectStage == StoryStageLogic.ProjectStages.eTeamFinalApproval);

            // do various notifications after 4 weeks
            var timeDiff = new TimeSpan(28, 0, 0, 0);

            // check to see if we should nag the PF about the number of SFGs
            if (loggedOnMember.IsPfAndNotLsr &&
                !_bStopNaggingPf &&
                ((record.Status == CstrOsMetaDataStatusProduction) ||
                 (record.Status == CstrOsMetaDataStatusSetComplete)) &&
                ((record.LastQueriedPfNumOfSfgs == DateTime.MinValue) ||
                 ((DateTime.Now - record.LastQueriedPfNumOfSfgs) > timeDiff)))
            {
                _bStopNaggingPf = true;
                QueryPfForNumberOfSfgs(record);
            }

            // check to see if we should remind the consultant/coach to check the meta data
            if ((record.EsStoriesSent >= 8) &&
                !_bStopNaggingPf &&
                TeamMemberData.IsUser(loggedOnMember.MemberType, TeamMemberData.UserTypes.AnyMentor) &&
                ((record.LastQueriedConsCheckMetaData == DateTime.MinValue) ||
                 ((DateTime.Now - record.LastQueriedConsCheckMetaData) > timeDiff)))
            {
                _bStopNaggingPf = true;
                if (QueryConsultantToCheckMetaData() &&
                    StoryEditor.LaunchOsMetaDataDialog(this))
                {
                    record.LastQueriedConsCheckMetaData = DateTime.Now;
                }

            }

            OsMetaData.Save();

            // get rid of the ChorusNotes file -- we don't want or need it telling us there was a conflict
            var strChorusNotesFileSpec = PathToMetaDataFile + ".ChorusNotes";
            if (File.Exists(strChorusNotesFileSpec))
                File.Delete(strChorusNotesFileSpec);
        }

        private static bool QueryConsultantToCheckMetaData()
        {
            return LocalizableMessageBox.Show(
                Localizer.Str("Would you like to check the OneStory meta data to see if any changes are necessary?"),
                StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes;
        }

        private static void QueryPfForNumberOfSfgs(OsMetaDataModelRecord record)
        {
            var strValue = LocalizableMessageBox.InputBox(
                Localizer.Str("How many Story Fellowship Groups are currently active in this project?"),
                StoryEditor.OseCaption, "");

            if (String.IsNullOrEmpty(strValue))
                return;

            int nValue;
            if (!Int32.TryParse(strValue, out nValue))
                return;

            record.NumberSfgs = nValue;
            record.LastQueriedPfNumOfSfgs = DateTime.Now;
        }

        private OsMetaDataModel LoadOsMetaData()
        {
            var strPathToMetaDataFile = PathToMetaDataFile;
            OsMetaDataModel data = null;
            if (File.Exists(strPathToMetaDataFile))
            {
                data = OsMetaDataModel.Load(strPathToMetaDataFile);

                if (data.Version != CxmlOsMetaDataVersion)
                {
                    LocalizableMessageBox.Show(Localizer.Str("One of the team members is using a newer version of OSE to edit the OS Meta Data file, which is not compatible with the version you are using. You might try, \"Advanced\", \"Program Updates\", \"Check now\" or \"Check now for next major update\" or you may have to go to the http://palaso.org/install/onestory website and download and install the new version of the program in the \"Setup OneStory Editor.zip\" file"), StoryEditor.OseCaption);
                    throw BackOutWithNoUI;
                }
            }

            return data;
        }

        private string PathToMetaDataFile
        {
            get { return Path.Combine(ProjSettings.ProjectFolder, CstrOsMetaDataFilename); }
        }

        public OsMetaDataModel InitializeMetaData
        {
            get
            {
                System.Diagnostics.Debug.Assert(OsMetaData == null);
                OsMetaData = new OsMetaDataModel { PathToMetaDataFile = PathToMetaDataFile };
                var record = new OsMetaDataModelRecord();
                OsMetaData.OsProjects.Add(record);

                record.ProjectName = ProjSettings.ProjectName;
                if (ProjSettings.Vernacular.HasData)
                {
                    record.LanguageName = ProjSettings.Vernacular.LangName;
                    record.EthnologueCode = ProjSettings.Vernacular.LangCode;
                }

                var storySet = this[Properties.Resources.IDS_MainStoriesSet];
                record.ProjectFacilitators = GetListOfMemberAtRole(storySet, TeamMembers,
                    (story) => story.CraftingInfo.ProjectFacilitator);

                var dtMinOfProject = storySet.Select(story => story.TransitionHistory
                                                                   .Min(th => th.TransitionDateTime))
                                             .FirstOrDefault();
                record.StartDate = dtMinOfProject;

                record.IsCurrentlyUsingOse = true;

                record.OseProjId = ProjSettings.ProjectName;

                record.LastQueriedPfNumOfSfgs = record.LastQueriedConsCheckMetaData = DateTime.MinValue;
                return OsMetaData;
            }
        }

        private delegate MemberIdInfo GetMemberAtRole(StoryData story);
        private static string GetListOfMemberAtRole(IEnumerable<StoryData> storySet, TeamMembersData teamMembers,
            GetMemberAtRole pGetMemberAtRole)
        {
            // aggregate the distinct names of the people in the stories in the storySet with
            //  a given role (e.g. Consultant, PF, etc)
            var strMembersWithRole = storySet.Where(story => pGetMemberAtRole(story) != null)
                                             .Select(story =>
                teamMembers.GetNameFromMemberId(pGetMemberAtRole(story).MemberId))
                .Distinct()
                .Aggregate<string, string>(null,
                    (current, next) => current + (next + ", "));

            return (String.IsNullOrEmpty(strMembersWithRole))
                        ? null
                        : strMembersWithRole.Substring(0, strMembersWithRole.Length - 2);
        }

        private void CheckForCommentMemberIds()
        {
            // add 'memberID' attributes to all connote conversation comments, but only
            //  if there's only one consultant (otherwise, we'll have to do this 
            //  interactively as a story is attempted to be edited).
            string strConsultant =
                TeamMembers.MemberIdOfOneAndOnlyMemberType(TeamMemberData.UserTypes.ConsultantInTraining |
                                                           TeamMemberData.UserTypes.IndependentConsultant);

            string strCoach =
                TeamMembers.MemberIdOfOneAndOnlyMemberType(TeamMemberData.UserTypes.Coach);

            if (!String.IsNullOrEmpty(strConsultant) || !String.IsNullOrEmpty(strCoach))
                foreach (var storiesData in Values)
                    storiesData.SetCommentMemberId(strConsultant, strCoach);
        }

        private void ConvertProjectFile1_3_to_1_4(string strProjectFilePath)
        {
            // if the user had 1.3 and customized the state transition xml file, 
            //  then blow it away.
            string strProjectFolder = Path.GetDirectoryName(strProjectFilePath);
            if (StoryStageLogic.StateTransitions.DoesStateTransitionFileOverrideExist(strProjectFolder))
            {
                if (LocalizableMessageBox.Show(Properties.Resources.IDS_ConfirmDeleteStateTransitions,
                    StoryEditor.OseCaption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    throw BackOutWithNoUI;

                StoryStageLogic.StateTransitions.DeleteStateTransitionFileOverride(strProjectFolder);
            }

            // get the xml (.onestory) file into a memory string so it can be the 
            //  input to the transformer
            string strProjectFile = File.ReadAllText(strProjectFilePath);
            var streamData = new MemoryStream(Encoding.UTF8.GetBytes(strProjectFile));

#if false // DEBUG
            string strXslt = File.ReadAllText(@"D:\src\StoryEditor\StoryEditor\Resources\1.3 to 1.4.xslt");
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

        // if this is 'new', then we won't have a project name yet, so query the user for it
        public bool InitializeProjectSettings(TeamMemberData loggedOnMember)
        {
#if !DataDllBuild
            var dlg = new NewProjectWizard(this)
            {
                LoggedInMember = loggedOnMember,
                Text = Localizer.Str("Edit Project Settings")
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                throw BackOutWithNoUI;

            // otherwise, it has our new project settings
            ProjSettings = dlg.ProjSettings;
#endif
            return true;    // if we get here, there are settings
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
            return TeamMembers.GetNameFromMemberId(strMemberGuid);
        }

        internal TeamMemberData GetMemberFromId(string strMemberId)
        {
            return TeamMembers.GetMemberFromId(strMemberId);
        }

#if !DataDllBuild
        internal TeamMemberData GetLogin(ref bool bModified)
        {
            // look at the last person to log in and see if we ought to automatically log them in again
            //  (basically Crafters or others that are also the same role as last time)
            string strMemberName = null;
            TeamMemberData loggedOnMember = null;
            var eRole = TeamMemberData.UserTypes.Undefined;
            if (Program.MapProjectNameToLastMemberLogin.ContainsKey(ProjSettings.ProjectName))
            {
                strMemberName = Program.MapProjectNameToLastMemberLogin[ProjSettings.ProjectName];
                eRole = TeamMemberData.GetMemberType(Program.MapProjectNameToLastUserType[ProjSettings.ProjectName]);
                if (CanLoginMember(strMemberName, eRole))
                    loggedOnMember = TeamMembers[strMemberName];
            }
            else if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
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
                    // if we did find the 'last member' in the list, but couldn't accept it without question
                    //  (e.g. because the role was different), then at least pre-select the member
                    dlg.SelectedMember = TeamMemberForm.GetListBoxItem(strMemberName, eRole);
                }
                catch { }    // might fail if the 'last user' on this machine is opening this project file for the first time... just ignore
            }

            var res = dlg.ShowDialog();

            // if the user added a new member, then the proj file is 'dirty'
            if (dlg.Modified)
                bModified = true;

            // if they didn't click 'OK' (they might not if they're selecting a collaborator), then
            //  just backout... BUT not if they actually selected a member of the correct type 
            //  (in which case, let it fall thru)
            if ((res != DialogResult.OK) && (String.IsNullOrEmpty(dlg.SelectedMemberName)
                                            || !TeamMembers.ContainsKey(dlg.SelectedMemberName)
                                            || !TeamMemberData.IsUser(TeamMembers[dlg.SelectedMemberName].MemberType, eRole)))
            {
                if (bUseLoginLabel && bLoginRequired)
                    LocalizableMessageBox.Show(
                        Localizer.Str(
                            "You have to log in to continue. Click 'Project', 'Login' and choose or add your name"),
                        StoryEditor.OseCaption);

                throw BackOutWithNoUI;
            }

            return TeamMembers[dlg.SelectedMemberName];
        }
#endif

#if UseUrlsWithChorus
        // use of this version factors in both the settings in the project file
        public bool GetHgRepoUsernamePassword(string strProjectName, 
            TeamMemberData loggedOnMember, out string strUsername, out string strPassword)
        {
            strPassword = null;    // just in case we don't have anything for this.

#if !DataDllBuild
            string strRepoUrl, strDummy;
            if (Program.GetHgRepoParameters(strProjectName, out strUsername, out strRepoUrl, 
                out strDummy))
            {
                if (!String.IsNullOrEmpty(strRepoUrl))
                {
                    var uri = new Uri(strRepoUrl);
                    StoryEditor.GetDetailsFromUri(uri, out strUsername, out strPassword,
                        ref strHgUrlBase);
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
                TeamMemberData.GetHgParameters(loggedOnMember,
                                               out strUsername, out strPassword);
            }

            return !String.IsNullOrEmpty(strHgUrlBase);
        }
#endif

        public bool IsASeparateEnglishBackTranslator
        {
            get
            {
                System.Diagnostics.Debug.Assert(TeamMembers != null);

                // the role 'English Back-translator' only has meaning if there's another
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

        public const string CstrElementStoryProjectRoot = "StoryProject";
        public const string CstrAttributeVersion = "version";

        public const string CstrAttributeProjectName = "ProjectName";
        public const string CstrAttributeUseDropbox = "UseDropbox";
        public const string CstrAttributeDropboxStory = "DropboxStory";
        public const string CstrAttributeDropboxRetellings = "DropboxRetellings";
        public const string CstrAttributeDropboxAnswers = "DropboxAnswers";

        public const string CstrElementOseStoryToCopy = "OseStoryToCopy";
        public const string CstrElementOseColumnToCopy = "OseColumnToCopy";

        public XElement GetXml
        {
            get
            {
                var elemStoryProject =
                    new XElement(CstrElementStoryProjectRoot,
                                 new XAttribute(CstrAttributeVersion, XmlDataVersion),
                                 new XAttribute(CstrAttributeProjectName, ProjSettings.ProjectName));

                if (ProjSettings.UseDropbox)
                {
                    elemStoryProject.Add(new XAttribute(CstrAttributeUseDropbox, true));

                    if (ProjSettings.DropboxStory)
                        elemStoryProject.Add(new XAttribute(CstrAttributeDropboxStory,
                                                            ProjSettings.DropboxStory));

                    if (ProjSettings.DropboxRetelling)
                        elemStoryProject.Add(new XAttribute(CstrAttributeDropboxRetellings,
                                                            ProjSettings.DropboxRetelling));

                    if (ProjSettings.DropboxAnswers)
                        elemStoryProject.Add(new XAttribute(CstrAttributeDropboxAnswers,
                                                            ProjSettings.DropboxAnswers));
                }

                elemStoryProject.Add(new XAttribute("PanoramaFrontMatter", PanoramaFrontMatter),
                                     TeamMembers.GetXml,
                                     ProjSettings.GetXml);
                
                if (ProjSettings.HasAdaptItConfigurationData)
                    elemStoryProject.Add(ProjSettings.AdaptItConfigXml);

                elemStoryProject.Add(LnCNotes.GetXml);

                foreach (var aSsD in Values)
                    elemStoryProject.Add(aSsD.GetXml);

                // if there are any "Sticky Notes"...  
                if (!SetNextVersionIfNeeded(ConsultNoteDataConverter.CommunicationDirections.eStickyNote, 
                                         elemStoryProject, CxmlDataVersionStickyNote))
                {
                    // otherwise if there are any "Referring text" Direction entries...  
                    SetNextVersionIfNeeded(ConsultNoteDataConverter.CommunicationDirections.eReferringToText,
                                        elemStoryProject, CxmlDataVersionReferringText);
                }

                return elemStoryProject;
            }
        }

        // retrieve an xml fragment so we can copy a story to another project... 
        //  just need the members (so we can add them to the new project, if used in this story) and the story itself
        public XElement GetXmlToCopyStory(StoryData theStoryToCopy)
        {
            return new XElement(CstrElementOseStoryToCopy,
                                TeamMembers.GetXml,
                                ProjSettings.GetXml,
                                theStoryToCopy.GetXml);
        }

        // retrieve an xml fragment so we can copy a story to another project... 
        //  just need the members (so we can add them to the new project, if used in this story) and the story itself
        public XElement GetXmlToCopyColumn(VersesData theColumnToCopy, SilEncConverters40.DirectableEncConverter transliterator, Func<LineData, StringTransfer> getFieldFunc)
        {
            var elem = new XElement(CstrElementOseColumnToCopy);
            if (transliterator != null)
                elem.Add(new XAttribute("Transliterator", transliterator.Name));

            foreach (var verse in theColumnToCopy)
            {
                var field = getFieldFunc(verse.StoryLine);
                verse.StoryLine.AddXmlField(elem, "StoryLine", field, field.GetLanguageType, transliterator);
            }

            return elem;
        }

        private static bool SetNextVersionIfNeeded(ConsultNoteDataConverter.CommunicationDirections eNewType, XElement elemStoryProject, string strNewVersion)
        {
            var strSearch =
                ConsultNoteDataConverter.GetCommDirectionStringFromEnum(eNewType);
            if (elemStoryProject.Descendants(ConsultantNoteData.CstrSubElementName)
                    .Any(e => e.Attribute(CommInstance.CstrAttributeLabelDirection).Value == strSearch) ||
                elemStoryProject.Descendants(CoachNoteData.CstrSubElementName)
                    .Any(e => e.Attribute(CommInstance.CstrAttributeLabelDirection).Value == strSearch))
            {
                // set the xml version to 1.7 so it forces all team members to use a version that supports this
                var versionAttribute = elemStoryProject.Attribute(CstrAttributeVersion);
                versionAttribute.Value = strNewVersion;
                return true;
            }
            return false;
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

        public void ReplaceConsultant(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storiesData in Values)
                storiesData.ReplaceConsultant(strOldMemberGuid, strNewMemberGuid);
        }

        public void ReplaceCoach(string strOldMemberGuid, string strNewMemberGuid)
        {
            foreach (var storiesData in Values)
                storiesData.ReplaceCoach(strOldMemberGuid, strNewMemberGuid);
        }

        public bool DoesReferenceExist(TeamMemberData theTeamMember)
        {
            return Values.Any(value => value.DoesReferenceExist(theTeamMember));
        }

        public class ReplaceMemberException : Exception
        {
            public string MemberGuid { get; set; }
            public string StoryName { get; set; }
            public string Format { get; set; }
        }
    }

    public class ProjectReader : NewDataSet
    {
        public static List<string> UniqueStoryGuids = new List<string>();

        public static DateTime ReadProjectFile(string strProjectFilePath, out ProjectReader projectReader)
        {
            try
            {
                UniqueStoryGuids.Clear();
                projectReader = new ProjectReader();
                projectReader.ReadXml(strProjectFilePath);
                return File.GetLastWriteTime(strProjectFilePath);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        var exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                MessageBox.Show(errorMessage);
            }
            projectReader = null;
            return DateTime.MinValue;
        }
    }
}
