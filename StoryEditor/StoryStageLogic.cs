using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Windows.Forms;
using OseResources;

namespace OneStoryProjectEditor
{
    public class StoryStageLogic
    {
        protected ProjectStages _ProjectStage = ProjectStages.eUndefined;
        internal static StateTransitions stateTransitions;

        public enum ProjectStages
        {
            eUndefined = 0,
            eProjFacTypeVernacular,
            eProjFacTypeNationalBT,
            eProjFacTypeInternationalBT,
            eProjFacAddAnchors,
            eProjFacAddStoryQuestions,
            eProjFacRevisesBeforeUnsTest,
            eBackTranslatorTypeInternationalBT,
            eBackTranslatorTranslateConNotesBeforeUnsTest,
            eConsultantCheckNonBiblicalStory,
            eFirstPassMentorCheck1,
            eConsultantCheckStoryInfo,
            eConsultantCheckAnchors,
            eConsultantCheckStoryQuestions,
            eConsultantCauseRevisionBeforeUnsTest,
            eCoachReviewRound1Notes,
            eConsultantReviseRound1Notes,
            eBackTranslatorTranslateConNotes,
            eProjFacReviseBasedOnRound1Notes,
            eProjFacOnlineReview1WithConsultant,
            eProjFacReadyForTest1,
            eProjFacEnterRetellingOfTest1,
            eProjFacEnterAnswersToStoryQuestionsOfTest1,
            eBackTranslatorTypeInternationalBTTest1,
            eFirstPassMentorCheck2,
            eConsultantCheck2,
            eCoachReviewRound2Notes,
            eTeamComplete
        }

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set { _ProjectStage = value; }
        }

        public StoryStageLogic(ProjectSettings projSettings, bool bHasIndependentConsultant)
        {
            System.Diagnostics.Debug.Assert(projSettings.Vernacular.HasData 
                || projSettings.NationalBT.HasData
                || projSettings.InternationalBT.HasData);

            // the first state is either eProjFacTypeVernacular or eProjFacTypeNationalBT or eProjFacTypeInternationalBT
            //  (can't have a separate EngBTr if there's no Vernacular or National BT)
            ProjectStage = (projSettings.Vernacular.HasData) ? ProjectStages.eProjFacTypeVernacular : 
                (projSettings.NationalBT.HasData) ? ProjectStages.eProjFacTypeNationalBT : ProjectStages.eProjFacTypeInternationalBT;

            if ((stateTransitions == null)
             || (stateTransitions.RewroteCitAsIndependentConsultant != bHasIndependentConsultant))
                stateTransitions = new StateTransitions(bHasIndependentConsultant);
        }

        public StoryStageLogic(string strProjectStage, bool bHasIndependentConsultant)
        {
            ProjectStage = GetProjectStageFromString(strProjectStage);
            if ((stateTransitions == null) 
             || (stateTransitions.RewroteCitAsIndependentConsultant != bHasIndependentConsultant))
                stateTransitions = new StateTransitions(bHasIndependentConsultant);
        }

        public StoryStageLogic(StoryStageLogic rhs)
        {
            ProjectStage = rhs.ProjectStage;
            System.Diagnostics.Debug.Assert(stateTransitions != null);
        }

        // these states (on the lhs) were purged in 1.4 in favor of those on the rhs.
        static readonly Dictionary<string, string> mapStageNameFixups = new Dictionary<string, string>
        {
            { "ConsultantReviseRound2Notes", "ConsultantReviseRound1Notes" },
            { "BackTranslatorTranslateConNotes2", "BackTranslatorTranslateConNotes" },
            { "ProjFacReviseBasedOnRound2Notes", "ProjFacReviseBasedOnRound1Notes" },
            { "ProjFacOnlineReview2WithConsultant", "ProjFacOnlineReview1WithConsultant" },
            { "ProjFacReadyForTest2", "ProjFacReadyForTest1" },
            { "ProjFacEnterRetellingOfTest2", "ProjFacEnterRetellingOfTest1" },
            { "ProjFacEnterAnswersToStoryQuestionsOfTest2", "ProjFacEnterAnswersToStoryQuestionsOfTest1" },
            { "BackTranslatorTypeInternationalBTTest2", "BackTranslatorTypeInternationalBTTest1" },
            { "FirstPassMentorReviewTest2", "FirstPassMentorCheck2" },
            { "ConsultantReviewTest2", "ConsultantCheck2" },
            { "CoachReviewTest2Notes", "CoachReviewRound2Notes" }
        };

        protected ProjectStages GetProjectStageFromString(string strProjectStageString)
        {
            try
            {
                return CmapStageStringToEnumType[strProjectStageString];
            }
            catch
            {
                strProjectStageString = mapStageNameFixups[strProjectStageString];
                return CmapStageStringToEnumType[strProjectStageString];
            }
        }

        public bool IsChangeOfStateAllowed(TeamMemberData loggedOnMember)
        {
            bool bRet = ((loggedOnMember.MemberType == TeamMemberData.UserTypes.eJustLooking)
                || (loggedOnMember.MemberType == MemberTypeWithEditToken));

            if (!bRet)
                MessageBox.Show(
                    String.Format(OseResources.Properties.Resources.IDS_WhichUserEdits,
                                  TeamMemberData.GetMemberTypeAsDisplayString(MemberTypeWithEditToken)),
                    OseResources.Properties.Resources.IDS_Caption);

            return bRet;
        }

        public bool IsEditAllowed(TeamMemberData.UserTypes loggedOnMemberType)
        {
            return (MemberTypeWithEditToken == loggedOnMemberType);
        }

        public ApplicationException WrongMemberTypeEx
        {
            get {
                return
                    new ApplicationException(String.Format(OseResources.Properties.Resources.IDS_WhichUserEdits,
                                                           TeamMemberData.GetMemberTypeAsDisplayString(
                                                               MemberTypeWithEditToken))); }
        }

        // this isn't 100% effective. Sometimes a particular stage can have a single (but varied) editors
        //  (e.g. the Online consult could either be the ProjFac or the consultant)
        public TeamMemberData.UserTypes MemberTypeWithEditToken
        {
            get
            {
                StateTransition st = stateTransitions[ProjectStage];
                return st.MemberTypeWithEditToken;
            }
        }

        public bool IsTerminalTransition(ProjectStages eNextStage)
        {
            StateTransition stThis = stateTransitions[ProjectStage];
            StateTransition stNext = stateTransitions[eNextStage];
            return (stThis.MemberTypeWithEditToken != stNext.MemberTypeWithEditToken);
        }

        public override string ToString()
        {
            return _ProjectStage.ToString().Substring(1);
        }

        protected static Dictionary<string, ProjectStages> CmapStageStringToEnumType = new Dictionary<string, ProjectStages>() {
            { "ProjFacTypeVernacular", ProjectStages.eProjFacTypeVernacular },
            { "ProjFacTypeNationalBT", ProjectStages.eProjFacTypeNationalBT },
            { "ProjFacTypeInternationalBT", ProjectStages.eProjFacTypeInternationalBT },
            { "ProjFacAddAnchors", ProjectStages.eProjFacAddAnchors },
            { "ProjFacAddStoryQuestions", ProjectStages.eProjFacAddStoryQuestions },
            { "ProjFacRevisesBeforeUnsTest", ProjectStages.eProjFacRevisesBeforeUnsTest },
            { "BackTranslatorTypeInternationalBT", ProjectStages.eBackTranslatorTypeInternationalBT },
            { "BackTranslatorTranslateConNotesBeforeUnsTest", ProjectStages.eBackTranslatorTranslateConNotesBeforeUnsTest },
            { "ConsultantCheckNonBiblicalStory", ProjectStages.eConsultantCheckNonBiblicalStory },
            { "FirstPassMentorCheck1", ProjectStages.eFirstPassMentorCheck1 },
			{ "ConsultantCheckStoryInfo", ProjectStages.eConsultantCheckStoryInfo },
            { "ConsultantCheckAnchors", ProjectStages.eConsultantCheckAnchors },
            { "ConsultantCheckStoryQuestions", ProjectStages.eConsultantCheckStoryQuestions },
            { "ConsultantCauseRevisionBeforeUnsTest", ProjectStages.eConsultantCauseRevisionBeforeUnsTest },
            { "CoachReviewRound1Notes", ProjectStages.eCoachReviewRound1Notes },
            { "ConsultantReviseRound1Notes", ProjectStages.eConsultantReviseRound1Notes },
            { "BackTranslatorTranslateConNotes", ProjectStages.eBackTranslatorTranslateConNotes },
            { "ProjFacReviseBasedOnRound1Notes", ProjectStages.eProjFacReviseBasedOnRound1Notes },
            { "ProjFacOnlineReview1WithConsultant", ProjectStages.eProjFacOnlineReview1WithConsultant },
            { "ProjFacReadyForTest1", ProjectStages.eProjFacReadyForTest1 },
            { "ProjFacEnterRetellingOfTest1", ProjectStages.eProjFacEnterRetellingOfTest1 },
            { "ProjFacEnterAnswersToStoryQuestionsOfTest1", ProjectStages.eProjFacEnterAnswersToStoryQuestionsOfTest1 },
            { "BackTranslatorTypeInternationalBTTest1", ProjectStages.eBackTranslatorTypeInternationalBTTest1 },
            { "FirstPassMentorCheck2", ProjectStages.eFirstPassMentorCheck2 },
            { "ConsultantCheck2", ProjectStages.eConsultantCheck2 },
            { "CoachReviewRound2Notes", ProjectStages.eCoachReviewRound2Notes },
            { "TeamComplete", ProjectStages.eTeamComplete }};

        public class StateTransitions : Dictionary<ProjectStages, StateTransition>
        {
            protected const string CstrStateTransitionsXmlFilename = "StageTransitions.xml";
            public bool RewroteCitAsIndependentConsultant;

            public StateTransitions(bool bHasIndependentConsultant)
            {
                RewroteCitAsIndependentConsultant = bHasIndependentConsultant;
                InitStateTransitionsFromXml(bHasIndependentConsultant);
            }

            protected string PathToXmlFile
            {
                get
                {
                    // try the same folder as we're executing out of
                    string strFileToCheck = Path.Combine(StoryProjectData.GetRunningFolder, 
                        CstrStateTransitionsXmlFilename);
#if DEBUG
                    if (!File.Exists(strFileToCheck))
                        // on dev machines, this file is in the "..\..\src\EC\TECkit Mapping Editor" folder
                        strFileToCheck = @"C:\src\StoryEditor\StoryEditor\" + CstrStateTransitionsXmlFilename;
#endif
                    System.Diagnostics.Debug.Assert(File.Exists(strFileToCheck), String.Format("Can't find: {0}! You'll need to re-install or contact bob_eaton@sall.com", strFileToCheck));

                    return strFileToCheck;
                }
            }
            
            /* rde: this code works just fine, it's just that we don't ever write this file anymore
            public void SaveStates(string strFilename)
            {
                // create the root portions of the XML document and tack on the fragment we've been building
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    GetXml);

                // save it with an extra extn.
                doc.Save(strFilename);
            }

            protected XElement GetXml
            {
                get
                {
                    XElement elem = new XElement("ProjectStates");
                    foreach (KeyValuePair<ProjectStages, StateTransition> kvp in this)
                    {
                        elem.Add(kvp.Value.GetXml);
                    }
                    return elem;
                }
            }
            */
            
            // these are for reading the file
            protected void GetXmlDocument(out XmlDocument doc, out XPathNavigator navigator, out XmlNamespaceManager manager)
            {
                doc = new XmlDocument();
                doc.Load(PathToXmlFile);
                navigator = doc.CreateNavigator();
                manager = new XmlNamespaceManager(navigator.NameTable);
            }

            protected void InitStateTransitionsFromXml(bool bHasIndependentConsultant)
            {
                try
                {
                    XmlDocument doc;
                    XPathNavigator navigator;
                    XmlNamespaceManager manager;
                    GetXmlDocument(out doc, out navigator, out manager);

                    XPathNodeIterator xpStageTransition = navigator.Select("/ProjectStates/StateTransition", manager);
                    while (xpStageTransition.MoveNext())
                    {
                        ProjectStages eThisStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpStageTransition.Current.GetAttribute("stage", navigator.NamespaceURI));
                        TeamMemberData.UserTypes eMemberType = (TeamMemberData.UserTypes)Enum.Parse(typeof(TeamMemberData.UserTypes), xpStageTransition.Current.GetAttribute("MemberTypeWithEditToken", navigator.NamespaceURI));
                        if (eMemberType == TeamMemberData.UserTypes.eConsultantInTraining
                            && bHasIndependentConsultant)
                            eMemberType = TeamMemberData.UserTypes.eIndependentConsultant;

                        bool bRequiresUsingVernacular =
                            (xpStageTransition.Current.GetAttribute("RequiresUsingVernacular", navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresUsingNationalBT =
                            (xpStageTransition.Current.GetAttribute("RequiresUsingNationalBT", navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresUsingEnglishBT =
                            (xpStageTransition.Current.GetAttribute("RequiresUsingEnglishBT", navigator.NamespaceURI) ==
                             "true");

                        string strRequiresUsingOtherEnglishBTer = xpStageTransition.Current.GetAttribute("RequiresUsingOtherEnglishBTer", 
                            navigator.NamespaceURI);
                        bool bHasUsingOtherEnglishBTer = !String.IsNullOrEmpty(strRequiresUsingOtherEnglishBTer);
                        bool bRequiresUsingOtherEnglishBTer = (strRequiresUsingOtherEnglishBTer == "true");
                        
                        bool bRequiresFirstPassMentor =
                            (xpStageTransition.Current.GetAttribute("RequiresFirstPassMentor", navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresBiblicalStory =
                            (xpStageTransition.Current.GetAttribute("RequiresBiblicalStory", navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresNonBiblicalStory =
                            (xpStageTransition.Current.GetAttribute("RequiresNonBiblicalStory", navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresManageWithCoaching = 
                            (xpStageTransition.Current.GetAttribute("RequiresManageWithCoaching", navigator.NamespaceURI) == 
                             "true");

                        XPathNodeIterator xpNextElement = xpStageTransition.Current.Select("StageDisplayString");
                        string strStageDisplayString = null;
                        if (xpNextElement.MoveNext())
                            strStageDisplayString = xpNextElement.Current.Value;

                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strStageDisplayString));

                        xpNextElement = xpStageTransition.Current.Select("StageInstructions");
                        string strStageInstructions = null;
                        if (xpNextElement.MoveNext())
                            strStageInstructions = xpNextElement.Current.Value;

                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strStageInstructions));

                        xpNextElement = xpStageTransition.Current.Select("AllowableForewardsTransitions/AllowableTransition");
                        List<AllowableTransition> lstAllowableForewardsStages = new List<AllowableTransition>();
                        while (xpNextElement.MoveNext())
                        {
                            AllowableTransition aps = new AllowableTransition
                            {
                                ProjectStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpNextElement.Current.Value),

                                // forward transitions have an 'AllowToSkip' attribute, which allows the user to possibly
                                //  jump forward two states (e.g. PF can jump over 'Project Facilitator has optional online 
                                //  review with consultant' if they have nothing to say.
                                AllowToSkip = (xpNextElement.Current.GetAttribute("AllowToSkip", navigator.NamespaceURI) == "true")
                            };
                            lstAllowableForewardsStages.Add(aps);
                        }

                        xpNextElement = xpStageTransition.Current.Select("AllowableBackwardsTransitions/AllowableTransition");
                        List<AllowableTransition> lstAllowableBackwardsStages = new List<AllowableTransition>();
                        while (xpNextElement.MoveNext())
                        {
                            AllowableTransition aps = new AllowableTransition
                            {
                                ProjectStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpNextElement.Current.Value)
                            };
                            lstAllowableBackwardsStages.Add(aps);
                        }

                        StateTransition st = new StateTransition(eThisStage, lstAllowableForewardsStages, lstAllowableBackwardsStages)
                            {
                                MemberTypeWithEditToken = eMemberType,
                                StageDisplayString = strStageDisplayString,
                                StageInstructions = strStageInstructions,
                                RequiresUsingVernacular = bRequiresUsingVernacular,
                                RequiresUsingNationalBT = bRequiresUsingNationalBT,
                                RequiresUsingEnglishBT = bRequiresUsingEnglishBT,
                                HasUsingOtherEnglishBTer = bHasUsingOtherEnglishBTer,
                                RequiresUsingOtherEnglishBTer = bRequiresUsingOtherEnglishBTer,
                                RequiresFirstPassMentor = bRequiresFirstPassMentor,
                                RequiresBiblicalStory = bRequiresBiblicalStory,
                                RequiresNonBiblicalStory = bRequiresNonBiblicalStory,
                                RequiresManageWithCoaching = bRequiresManageWithCoaching
                            };

                        xpNextElement = xpStageTransition.Current.Select("ViewSettings");
                        if (xpNextElement.MoveNext())
                        {
                            st.IsVernacularVisible = (xpNextElement.Current.GetAttribute("viewVernacularLangField", navigator.NamespaceURI) == "true");
                            st.IsNationalBTVisible = (xpNextElement.Current.GetAttribute("viewNationalLangField", navigator.NamespaceURI) == "true");
                            st.IsEnglishBTVisible = (xpNextElement.Current.GetAttribute("viewEnglishBTField", navigator.NamespaceURI) == "true");
                            st.IsAnchorVisible = (xpNextElement.Current.GetAttribute("viewAnchorField", navigator.NamespaceURI) == "true");
                            st.IsStoryTestingQuestionVisible = (xpNextElement.Current.GetAttribute("viewStoryTestingQuestions", navigator.NamespaceURI) == "true");
                            st.IsStoryTestingQuestionAnswersVisible = (xpNextElement.Current.GetAttribute("viewStoryTestingAnswers", navigator.NamespaceURI) == "true");
                            st.IsRetellingVisible = (xpNextElement.Current.GetAttribute("viewRetellingField", navigator.NamespaceURI) == "true");
                            st.IsConsultantNotesVisible = (xpNextElement.Current.GetAttribute("viewConsultantNoteField", navigator.NamespaceURI) == "true");
                            st.IsCoachNotesVisible = (xpNextElement.Current.GetAttribute("viewCoachNotesField", navigator.NamespaceURI) == "true");
                            st.IsNetBibleVisible = (xpNextElement.Current.GetAttribute("viewNetBible", navigator.NamespaceURI) == "true");
                        }

                        Add(eThisStage, st);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(String.Format("Unable to process the xml file containing the State Transitions (i.e. {0})... Reinstall.", CstrStateTransitionsXmlFilename), ex);
                }
            }
        }

        public class AllowableTransitions : List<AllowableTransition>
        {
            private string ElementLabel { get; set; }

            public AllowableTransitions(string strElementName)
            {
                ElementLabel = strElementName;
            }

            public XElement GetXml
            {
                get
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ElementLabel));
                    XElement elemAllowableTransition = new XElement(ElementLabel);
                    foreach (AllowableTransition allowed in this)
                    {
                        elemAllowableTransition.Add(allowed.GetXml);
                    }
                    return elemAllowableTransition;
                }
            }
        }
        public class AllowableTransition
        {
            public ProjectStages ProjectStage { get; set; }
            public bool AllowToSkip { get; set; }

            public bool IsThisStateAllow(StoryProjectData storyProjectData, StoryData theCurrentStory)
            {
                StateTransition st = stateTransitions[ProjectStage];
                return st.IsThisStateAllow(storyProjectData, theCurrentStory);
            }

            public XElement GetXml
            {
                get
                {
                    XElement elem = new XElement("AllowableTransition", ProjectStage);
                    if (AllowToSkip)
                        elem.Add(new XAttribute("AllowToSkip", AllowToSkip));
                    return elem;
                }
            }
        }

        public class StateTransition
        {
            internal ProjectStages CurrentStage = ProjectStages.eUndefined;
            internal AllowableTransitions AllowableForewardsTransitions = new AllowableTransitions("AllowableForewardsTransitions");
            internal AllowableTransitions AllowableBackwardsTransitions = new AllowableTransitions("AllowableBackwardsTransitions");
            internal TeamMemberData.UserTypes MemberTypeWithEditToken = TeamMemberData.UserTypes.eUndefined;
            internal string StageDisplayString;
            internal string StageInstructions;
#if !DataDllBuild
            public CheckEndOfStateTransition.CheckForValidEndOfState IsReadyForTransition;
#endif

            internal bool IsVernacularVisible { get; set; }
            internal bool IsNationalBTVisible { get; set; }
            internal bool IsEnglishBTVisible { get; set; }
            internal bool IsAnchorVisible { get; set; }
            internal bool IsStoryTestingQuestionVisible { get; set; }
            internal bool IsStoryTestingQuestionAnswersVisible { get; set; }
            internal bool IsRetellingVisible { get; set; }
            internal bool IsConsultantNotesVisible { get; set; }
            internal bool IsCoachNotesVisible { get; set; }
            internal bool IsNetBibleVisible { get; set; }

            internal string PreviousStateButtonLabel { get; set; }
            internal ProjectStages PreviousButtonState { get; set; }
            internal string NextStateButtonLabel { get; set; }

            internal bool RequiresUsingVernacular { get; set; }
            internal bool RequiresUsingNationalBT { get; set; }
            internal bool RequiresUsingEnglishBT { get; set; }
            internal bool HasUsingOtherEnglishBTer { get; set; }
            internal bool RequiresUsingOtherEnglishBTer { get; set; }
            internal bool RequiresFirstPassMentor { get; set; }
            internal bool RequiresBiblicalStory { get; set; }
            internal bool RequiresNonBiblicalStory { get; set; }
            internal bool RequiresManageWithCoaching { get; set; }

            public StateTransition
                (
                ProjectStages thisStage,
                IEnumerable<AllowableTransition> lstAllowableForewardsStages,
                IEnumerable<AllowableTransition> lstAllowableBackwardsStages
                )
            {
                CurrentStage = thisStage;
                AllowableForewardsTransitions.AddRange(lstAllowableForewardsStages);
                AllowableBackwardsTransitions.AddRange(lstAllowableBackwardsStages);
                string strMethodName = thisStage.ToString().Substring(1);

#if !DataDllBuild
                IsReadyForTransition = (CheckEndOfStateTransition.CheckForValidEndOfState)Delegate.CreateDelegate(
                    typeof(CheckEndOfStateTransition.CheckForValidEndOfState), 
                    typeof(CheckEndOfStateTransition), strMethodName);
#endif
            }

            public bool IsTransitionValid(ProjectStages eToStage)
            {
                foreach (AllowableTransition aps in AllowableBackwardsTransitions)
                    if (aps.ProjectStage == eToStage)
                        return true;
                return false;
            }

            public bool IsThisStateAllow(StoryProjectData storyProjectData, StoryData theCurrentStory)
            {
                return ((!RequiresUsingVernacular || storyProjectData.ProjSettings.Vernacular.HasData)
                        && (!RequiresUsingNationalBT || storyProjectData.ProjSettings.NationalBT.HasData)
                        && (!RequiresUsingEnglishBT || storyProjectData.ProjSettings.InternationalBT.HasData)
                        && (!RequiresBiblicalStory || theCurrentStory.CraftingInfo.IsBiblicalStory)
                        && (!RequiresNonBiblicalStory || !theCurrentStory.CraftingInfo.IsBiblicalStory)
                        && (!RequiresFirstPassMentor || storyProjectData.TeamMembers.HasOutsideEnglishBTer)
                        && (!HasUsingOtherEnglishBTer
                            || (RequiresUsingOtherEnglishBTer ==
                                storyProjectData.TeamMembers.HasOutsideEnglishBTer))
                        && (!RequiresManageWithCoaching || !storyProjectData.TeamMembers.HasIndependentConsultant)
                        );
            }

#if !DataDllBuild
            public void SetView(StoryEditor theSE)
            {
                // the vern should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an English story project)
                theSE.viewVernacularLangFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.Vernacular.HasData
                    && (IsVernacularVisible 
                        || (!theSE.StoryProject.ProjSettings.NationalBT.HasData 
                        && !theSE.StoryProject.ProjSettings.InternationalBT.HasData)));

                // the National BT should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an EnglishBT-only project)
                theSE.viewNationalLangFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.NationalBT.HasData
                    && (IsNationalBTVisible
                        || (!theSE.StoryProject.ProjSettings.Vernacular.HasData
                        && !theSE.StoryProject.ProjSettings.InternationalBT.HasData)));

                // the English BT should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an EnglishBT-only project)
                theSE.viewEnglishBTFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.InternationalBT.HasData
                    &&  (IsEnglishBTVisible 
                        || (!theSE.StoryProject.ProjSettings.Vernacular.HasData 
                        && !theSE.StoryProject.ProjSettings.NationalBT.HasData)));

                theSE.viewAnchorFieldMenuItem.Checked = IsAnchorVisible;
                theSE.viewStoryTestingQuestionMenuItem.Checked = IsStoryTestingQuestionVisible;
                theSE.viewStoryTestingQuestionAnswerMenuItem.Checked = IsStoryTestingQuestionAnswersVisible;
                theSE.viewRetellingFieldMenuItem.Checked = IsRetellingVisible;
                theSE.viewConsultantNoteFieldMenuItem.Checked = IsConsultantNotesVisible;
                theSE.viewCoachNotesFieldMenuItem.Checked = IsCoachNotesVisible;
                theSE.viewNetBibleMenuItem.Checked = IsNetBibleVisible;
            }
#endif

            public XElement GetXml
            {
                get
                {
                    XElement elem = new XElement("StateTransition", 
                        new XAttribute("stage", CurrentStage),
                        new XAttribute("MemberTypeWithEditToken", MemberTypeWithEditToken));

                    if (RequiresUsingVernacular)
                        elem.Add(new XAttribute("RequiresUsingVernacular", RequiresUsingVernacular));

                    if (RequiresUsingNationalBT)
                        elem.Add(new XAttribute("RequiresUsingNationalBT", RequiresUsingNationalBT));

                    if (RequiresUsingEnglishBT)
                        elem.Add(new XAttribute("RequiresUsingEnglishBT", RequiresUsingEnglishBT));

                    if (RequiresBiblicalStory)
                        elem.Add(new XAttribute("RequiresBiblicalStory", RequiresBiblicalStory));

                    if (RequiresNonBiblicalStory)
                        elem.Add(new XAttribute("RequiresNonBiblicalStory", RequiresNonBiblicalStory));

                    if (RequiresManageWithCoaching)
                        elem.Add(new XAttribute("RequiresManageWithCoaching", RequiresManageWithCoaching));

                    elem.Add(new XElement("StageDisplayString", StageDisplayString),
                             new XElement("StageInstructions", StageInstructions),
                             AllowableForewardsTransitions.GetXml,
                             AllowableBackwardsTransitions.GetXml,
                             new XElement("ViewSettings",
                                new XAttribute("viewVernacularLangFieldMenuItem", IsVernacularVisible),
                                new XAttribute("viewNationalLangFieldMenuItem", IsNationalBTVisible),
                                new XAttribute("viewEnglishBTFieldMenuItem", IsEnglishBTVisible),
                                new XAttribute("viewAnchorFieldMenuItem", IsAnchorVisible),
                                new XAttribute("viewStoryTestingQuestions", IsStoryTestingQuestionVisible),
                                new XAttribute("viewStoryTestingAnswers", IsStoryTestingQuestionAnswersVisible),
                                new XAttribute("viewRetellingFieldMenuItem", IsRetellingVisible),
                                new XAttribute("viewConsultantNoteFieldMenuItem", IsConsultantNotesVisible),
                                new XAttribute("viewCoachNotesFieldMenuItem", IsCoachNotesVisible),
                                new XAttribute("viewNetBibleMenuItem", IsNetBibleVisible)));

                    return elem;
                }
            }
        }
    }
}
