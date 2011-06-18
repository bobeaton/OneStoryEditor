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
            eProjFacTypeFreeTranslation,
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
            eProjFacRevisesAfterUnsTest,
            eBackTranslatorTypeInternationalBTTest1,
            eBackTranslatorTranslateConNotesAfterUnsTest,
            eFirstPassMentorCheck2,
            eConsultantCheck2,
            eConsultantCauseRevisionAfterUnsTest,
            eCoachReviewRound2Notes,
            eConsultantFinalCheck,
            eTeamComplete,  // i.e. eTeamPreliminaryApproval
            eTeamFinalApproval
        }

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set { _ProjectStage = value; }
        }

        public StoryStageLogic(ProjectSettings projSettings)
        {
            System.Diagnostics.Debug.Assert(projSettings.Vernacular.HasData 
                || projSettings.NationalBT.HasData
                || projSettings.InternationalBT.HasData
                || projSettings.FreeTranslation.HasData);

            // the first state is either eProjFacTypeVernacular or eProjFacTypeNationalBT or eProjFacTypeInternationalBT
            //  (can't have a separate EngBTr if there's no Vernacular or National BT)
            ProjectStage = (projSettings.Vernacular.HasData)
                               ? ProjectStages.eProjFacTypeVernacular
                               : (projSettings.NationalBT.HasData)
                                     ? ProjectStages.eProjFacTypeNationalBT
                                     : (projSettings.InternationalBT.HasData)
                                           ? ProjectStages.eProjFacTypeInternationalBT
                                           : ProjectStages.eProjFacTypeFreeTranslation;

            if (stateTransitions == null)
                stateTransitions = new StateTransitions(projSettings.ProjectFolder);
        }

        public StoryStageLogic(string strProjectFolder, string strProjectStage)
        {
            ProjectStage = GetProjectStageFromString(strProjectStage);
            if (stateTransitions == null) 
                stateTransitions = new StateTransitions(strProjectFolder);
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

        public static ProjectStages GetProjectStageFromString(string strProjectStageString)
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

        // this isn't 100% effective. Sometimes a particular stage can have a single (but varied) editors
        //  (e.g. the Online consult could either be the ProjFac or the consultant). But this is mostly
        //  used in the TeamsMemberData class which knows of these details.
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
            { "ProjFacTypeFreeTranslation", ProjectStages.eProjFacTypeFreeTranslation },
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
            { "ProjFacRevisesAfterUnsTest", ProjectStages.eProjFacRevisesAfterUnsTest },
            { "BackTranslatorTypeInternationalBTTest1", ProjectStages.eBackTranslatorTypeInternationalBTTest1 },
            { "BackTranslatorTranslateConNotesAfterUnsTest", ProjectStages.eBackTranslatorTranslateConNotesAfterUnsTest },
            { "FirstPassMentorCheck2", ProjectStages.eFirstPassMentorCheck2 },
            { "ConsultantCheck2", ProjectStages.eConsultantCheck2 },
            { "ConsultantCauseRevisionAfterUnsTest", ProjectStages.eConsultantCauseRevisionAfterUnsTest },
            { "CoachReviewRound2Notes", ProjectStages.eCoachReviewRound2Notes },
            { "ConsultantFinalCheck", ProjectStages.eConsultantFinalCheck },
            { "TeamComplete", ProjectStages.eTeamComplete },
            { "TeamFinalApproval", ProjectStages.eTeamFinalApproval }};

        public class StateTransitions : Dictionary<ProjectStages, StateTransition>
        {
            public const string CstrStateTransitionsXmlFilename = "StageTransitions.xml";
            protected string _strProjectFolder;

            public StateTransitions(string strProjectFolder)
            {
                _strProjectFolder = strProjectFolder;
                InitStateTransitionsFromXml(_strProjectFolder);
            }

            protected string DefaultPathToXmlFile
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
            
            public void SaveCustomStateTransitionsXmlFile()
            {
                // create the root portions of the XML document and tack on the fragment we've been building
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    GetXml);

                // save it with an extra extn.
                string strCustomFile = Path.Combine(_strProjectFolder, CstrStateTransitionsXmlFilename);;
                doc.Save(strCustomFile);
            }

            protected string XmlDataVersion = "2.0";

            protected XElement GetXml
            {
                get
                {
                    var elem = new XElement("ProjectStates",
                        new XAttribute("version", XmlDataVersion));

                    foreach (KeyValuePair<ProjectStages, StateTransition> kvp in this)
                    {
                        elem.Add(kvp.Value.GetXml);
                    }
                    return elem;
                }
            }
            
            // these are for reading the file
            protected void GetXmlDocument(string strProjectFolder, 
                out XmlDocument doc, out XPathNavigator navigator, out XmlNamespaceManager manager)
            {
                doc = new XmlDocument();

                // first see if we have a customized version of the StateTransitions.xml in the project folder
                string strCustomFile = Path.Combine(strProjectFolder, CstrStateTransitionsXmlFilename);
                if (File.Exists(strCustomFile))
                    doc.Load(strCustomFile);
                else
                    doc.Load(DefaultPathToXmlFile);

                navigator = doc.CreateNavigator();
                manager = new XmlNamespaceManager(navigator.NameTable);
            }

            protected void InitStateTransitionsFromXml(string strProjectFolder)
            {
                try
                {
                    XmlDocument doc;
                    XPathNavigator navigator;
                    XmlNamespaceManager manager;
                    GetXmlDocument(strProjectFolder, out doc, out navigator, out manager);

#if DoUpdateTo20
                    // first see if we need to upgrade from the previous format (which
                    //  didn't have the FreeTranslation state, nor the viewFreeTranslation
                    //  bool.
                    var varProjectStateVersion = navigator.SelectSingleNode(String.Format("/{0}/@{1}",
                                                                         StateTransition.CstrElementLabelProjectStates, StateTransition.CstrAttributeLabelVersion));

                    if (varProjectStateVersion == null)
                        FixupTo2_0(doc, navigator, manager);
                    else if (varProjectStateVersion.ToString().CompareTo(XmlDataVersion) > 0)
                        throw new ApplicationException(OseResources.Properties.Resources.IDS_GetNewVersion);
#endif

                    XPathNodeIterator xpStageTransition = navigator.Select(String.Format("/{0}/{1}", 
                        StateTransition.CstrElementLabelProjectStates, StateTransition.CstrElementLabelStateTransition), manager);

                    while (xpStageTransition.MoveNext())
                    {
                        ProjectStages eThisStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelStage, navigator.NamespaceURI));
                        string strMemberType =
                            xpStageTransition.Current.GetAttribute(
                                StateTransition.CstrAttributeLabelMemberTypeWithEditToken, navigator.NamespaceURI);
                        TeamMemberData.UserTypes eMemberType = (TeamMemberData.UserTypes)Enum.Parse(typeof(TeamMemberData.UserTypes), strMemberType);
                        
                        bool bRequiresUsingVernacular =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresUsingVernacular, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresUsingNationalBT =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresUsingNationalBT, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresUsingEnglishBT =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresUsingEnglishBT, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresUsingFreeTranslation =
                            (xpStageTransition.Current.GetAttribute(
                                StateTransition.CstrAttributeLabelRequiresUsingFreeTranslation, navigator.NamespaceURI) ==
                             "true");

                        string strRequiresUsingOtherEnglishBTer = xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLableRequiresUsingOtherEnglishBTer, 
                            navigator.NamespaceURI);
                        bool bHasUsingOtherEnglishBTer = !String.IsNullOrEmpty(strRequiresUsingOtherEnglishBTer);
                        bool bRequiresUsingOtherEnglishBTer = (strRequiresUsingOtherEnglishBTer == "true");
                        
                        bool bRequiresFirstPassMentor =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLableRequiresFirstPassMentor, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresBiblicalStory =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresBiblicalStory, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresNonBiblicalStory =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresNonBiblicalStory, navigator.NamespaceURI) ==
                             "true");
                        bool bRequiresManageWithCoaching = 
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelRequiresManageWithCoaching, navigator.NamespaceURI) == 
                             "true");
                        bool bDontShowTilPast =
                            (xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelDontShowTilPast, navigator.NamespaceURI) ==
                             "true");

                        XPathNodeIterator xpNextElement = xpStageTransition.Current.Select(StateTransition.CstrElementLabelStageDisplayString);
                        string strStageDisplayString = null;
                        if (xpNextElement.MoveNext())
                            strStageDisplayString = xpNextElement.Current.Value;
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strStageDisplayString));

                        xpNextElement = xpStageTransition.Current.Select(StateTransition.CstrElementLabelTransitionDisplayString);
                        string strTransitionDisplayString = null;
                        if (xpNextElement.MoveNext())
                            strTransitionDisplayString = xpNextElement.Current.Value;
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strTransitionDisplayString));

                        xpNextElement = xpStageTransition.Current.Select(StateTransition.CstrElementLabelStageInstructions);
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

                        StateTransition st = new StateTransition(eThisStage, lstAllowableForewardsStages,
                                                                 lstAllowableBackwardsStages)
                                                 {
                                                     MemberTypeWithEditToken = eMemberType,
                                                     StageDisplayString = strStageDisplayString,
                                                     TransitionDisplayString = strTransitionDisplayString,
                                                     StageInstructions = strStageInstructions,
                                                     RequiresUsingVernacular = bRequiresUsingVernacular,
                                                     RequiresUsingNationalBT = bRequiresUsingNationalBT,
                                                     RequiresUsingEnglishBT = bRequiresUsingEnglishBT,
                                                     HasUsingOtherEnglishBTer = bHasUsingOtherEnglishBTer,
                                                     RequiresUsingFreeTranslation = bRequiresUsingFreeTranslation,
                                                     RequiresUsingOtherEnglishBTer = bRequiresUsingOtherEnglishBTer,
                                                     RequiresFirstPassMentor = bRequiresFirstPassMentor,
                                                     RequiresBiblicalStory = bRequiresBiblicalStory,
                                                     RequiresNonBiblicalStory = bRequiresNonBiblicalStory,
                                                     RequiresManageWithCoaching = bRequiresManageWithCoaching,
                                                     DontShowTilPast = bDontShowTilPast
                                                 };

                        xpNextElement = xpStageTransition.Current.Select(StateTransition.CstrElementLabelViewSettings);
                        if (xpNextElement.MoveNext())
                        {
                            st.IsVernacularVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewVernacularLangField, navigator.NamespaceURI) == "true");
                            st.IsNationalBTVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewNationalLangField, navigator.NamespaceURI) == "true");
                            st.IsEnglishBTVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewEnglishBTField, navigator.NamespaceURI) == "true");
                            st.IsFreeTranslationVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewFreeTranslationField, navigator.NamespaceURI) == "true");
                            st.IsAnchorVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewAnchorField, navigator.NamespaceURI) == "true");
                            st.IsStoryTestingQuestionVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewStoryTestingQuestions, navigator.NamespaceURI) == "true");
                            st.IsStoryTestingQuestionAnswersVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewStoryTestingAnswers, navigator.NamespaceURI) == "true");
                            st.IsRetellingVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewRetellingField, navigator.NamespaceURI) == "true");
                            st.IsConsultantNotesVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewConsultantNoteField, navigator.NamespaceURI) == "true");
                            st.IsCoachNotesVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewCoachNotesField, navigator.NamespaceURI) == "true");
                            st.IsNetBibleVisible = (xpNextElement.Current.GetAttribute(StateTransition.CstrAttributeLabelViewNetBible, navigator.NamespaceURI) == "true");
                        }

                        Add(eThisStage, st);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        String.Format(Properties.Resources.IDS_UnableToOpenStageTransitionsXml,
                                      CstrStateTransitionsXmlFilename,
                                      strProjectFolder), ex);
                }
            }

#if DoUpdateTo20
            private void FixupTo2_0(XmlDocument doc, XPathNavigator navigator, XmlNamespaceManager manager)
            {
                navigator.MoveToChild(StateTransition.CstrElementLabelProjectStates, navigator.NamespaceURI);
                navigator.CreateAttribute(navigator.Prefix, "version", navigator.NamespaceURI, XmlDataVersion);

                XPathNodeIterator xpStageTransition = navigator.Select(String.Format("{0}",
                    StateTransition.CstrElementLabelStateTransition), manager);

                while (xpStageTransition.MoveNext())
                {
                    ProjectStages eThisStage =
                        (ProjectStages)
                        Enum.Parse(typeof (ProjectStages),
                                   xpStageTransition.Current.GetAttribute(StateTransition.CstrAttributeLabelStage,
                                                                          navigator.NamespaceURI));

                    // add the updated bit just before AddAnchors
                    if (eThisStage == ProjectStages.eProjFacAddAnchors)
                        xpStageTransition.Current.InsertBefore(Properties.Resources.StageTransition20Update);
                }
            }
#endif
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
                    if (this.Count == 0)
                        return null;

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

            /// <summary>
            /// This method can be used to indicate whether this supposed alloweable transition is
            /// actually allowed given our current project configuration. e.g. the state eProjFacTypeInternationalBT
            /// is generally allowed after the state eProjFacTypeVernacular, but not if we have an 'outside English
            /// back-translator'
            /// </summary>
            /// <param name="storyProjectData"></param>
            /// <param name="theCurrentStory"></param>
            /// <returns></returns>
            public bool IsThisTransitionAllow(StoryProjectData storyProjectData, StoryData theCurrentStory)
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
            internal TeamMemberData.UserTypes MemberTypeWithEditToken = TeamMemberData.UserTypes.Undefined;
            internal string StageDisplayString;
            internal string TransitionDisplayString;
            private string _strStageInstructions;
            public string StageInstructions
            {
                get
                {
                    return _strStageInstructions;
                }
                set
                {
#if LetHelpDoSizing
                    // strip out the single instances of \r\n
                    _strStageInstructions = value.Replace("\r\n\r\n", "\u001f") // replace double-instances (which we want to keep)
                        .Replace("\r\n", null)  // strip out single instances
                        .Replace("\u001f", "\r\n\r\n"); // replace the double-instances
#else
                    _strStageInstructions = value;
#endif
                }
            }
#if !DataDllBuild
            public CheckEndOfStateTransition.CheckForValidEndOfState IsReadyForTransition;
#endif

            internal bool IsVernacularVisible { get; set; }
            internal bool IsNationalBTVisible { get; set; }
            internal bool IsEnglishBTVisible { get; set; }
            internal bool IsFreeTranslationVisible { get; set; }
            internal bool IsAnchorVisible { get; set; }
            internal bool IsStoryTestingQuestionVisible { get; set; }
            internal bool IsStoryTestingQuestionAnswersVisible { get; set; }
            internal bool IsRetellingVisible { get; set; }
            internal bool IsConsultantNotesVisible { get; set; }
            internal bool IsCoachNotesVisible { get; set; }
            internal bool IsNetBibleVisible { get; set; }

            internal ProjectStages DefaultNextState(StoryProjectData storyProjectData,
                StoryData theCurrentStory)
            {
                // allowed transition states (could be previous or forward)
                foreach (AllowableTransition aps in AllowableForewardsTransitions)
                {
                    // see if this transition is allowed from our current situation
                    if (aps.IsThisTransitionAllow(storyProjectData, theCurrentStory))
                        return aps.ProjectStage;
                }
                return theCurrentStory.ProjStage.ProjectStage;
            }

            internal bool RequiresUsingVernacular { get; set; }
            internal bool RequiresUsingNationalBT { get; set; }
            internal bool RequiresUsingEnglishBT { get; set; }
            internal bool HasUsingOtherEnglishBTer { get; set; }
            internal bool RequiresUsingFreeTranslation { get; set; }
            internal bool RequiresUsingOtherEnglishBTer { get; set; }
            internal bool RequiresFirstPassMentor { get; set; }
            internal bool RequiresBiblicalStory { get; set; }
            internal bool RequiresNonBiblicalStory { get; set; }
            internal bool RequiresManageWithCoaching { get; set; }
            internal bool DontShowTilPast { get; set; }

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

            /// <summary>
            /// This method indicates whether the CurrentState is legitimate (i.e. to show in the 
            /// Select State form) based on the current configuration. e.g. the state eCoachReviewRound2Notes
            /// is only allowed if current configuration is that there's no IndependentConsultant.
            /// </summary>
            /// <param name="storyProjectData">project configuration information</param>
            /// <param name="theCurrentStory">current story configuration (e.g. is biblical story or not)</param>
            /// <returns></returns>
            public bool IsThisStateAllow(StoryProjectData storyProjectData, StoryData theCurrentStory)
            {
                return ((!RequiresUsingVernacular || storyProjectData.ProjSettings.Vernacular.HasData)
                        && (!RequiresUsingNationalBT || storyProjectData.ProjSettings.NationalBT.HasData)
                        && (!RequiresUsingEnglishBT || storyProjectData.ProjSettings.InternationalBT.HasData)
                        && (!RequiresUsingFreeTranslation || storyProjectData.ProjSettings.FreeTranslation.HasData)
                        && (!RequiresBiblicalStory || theCurrentStory.CraftingInfo.IsBiblicalStory)
                        && (!RequiresNonBiblicalStory || !theCurrentStory.CraftingInfo.IsBiblicalStory)
                        && (!HasUsingOtherEnglishBTer
                            || (RequiresUsingOtherEnglishBTer ==
                                storyProjectData.TeamMembers.HasOutsideEnglishBTer))
                        && (!RequiresManageWithCoaching || !storyProjectData.TeamMembers.HasIndependentConsultant)
                        && (!DontShowTilPast || ((int)theCurrentStory.ProjStage.ProjectStage >= (int)CurrentStage))
                        );
            }

#if !DataDllBuild
            /// <summary>
            /// This method will set the view menu items to on or off depending on the defaults for the given state
            /// </summary>
            /// <param name="theSE"></param>
            public void SetView(StoryEditor theSE)
            {
                // the vern should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an English story project)
                theSE.viewVernacularLangFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.Vernacular.HasData
                    && (IsVernacularVisible 
                        || (!theSE.StoryProject.ProjSettings.NationalBT.HasData
                        && !theSE.StoryProject.ProjSettings.InternationalBT.HasData
                        && !theSE.StoryProject.ProjSettings.FreeTranslation.HasData)));

                // the National BT should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an EnglishBT-only project)
                theSE.viewNationalLangFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.NationalBT.HasData
                    && (IsNationalBTVisible
                        || (!theSE.StoryProject.ProjSettings.Vernacular.HasData
                        && !theSE.StoryProject.ProjSettings.InternationalBT.HasData
                        && !theSE.StoryProject.ProjSettings.FreeTranslation.HasData)));

                // the English BT should be visible if it is configured and either it's supposed to be visible (based on the
                //  state information) OR it's the only one in existance--i.e. an EnglishBT-only project)
                theSE.viewEnglishBTFieldMenuItem.Checked = (theSE.StoryProject.ProjSettings.InternationalBT.HasData
                    &&  (IsEnglishBTVisible 
                        || (!theSE.StoryProject.ProjSettings.Vernacular.HasData
                        && !theSE.StoryProject.ProjSettings.NationalBT.HasData
                        && !theSE.StoryProject.ProjSettings.FreeTranslation.HasData)));

                // the Free translation should be visible if it is configured and either it's supposed to be visible 
                //  (based on the state information) OR it's the only one in existance--i.e. a Free Translation-only 
                //  project)
                theSE.viewFreeTranslationToolStripMenuItem.Checked = (theSE.StoryProject.ProjSettings.FreeTranslation.HasData
                    && (IsFreeTranslationVisible
                        || (!theSE.StoryProject.ProjSettings.Vernacular.HasData
                        && !theSE.StoryProject.ProjSettings.NationalBT.HasData
                        && !theSE.StoryProject.ProjSettings.InternationalBT.HasData)));

                // the rest never want to be on for a non-biblical story
                bool bBiblicalStory = ((theSE.TheCurrentStory == null) ||
                                       theSE.TheCurrentStory.CraftingInfo.IsBiblicalStory);

                theSE.viewAnchorFieldMenuItem.Checked = 
                    IsAnchorVisible && bBiblicalStory;

                theSE.viewExegeticalHelps.Checked = // this kindof goes with anchors
                    IsAnchorVisible;    // but doesn't require a biblical story

                theSE.viewStoryTestingQuestionMenuItem.Checked = IsStoryTestingQuestionVisible && bBiblicalStory;
                theSE.viewStoryTestingQuestionAnswerMenuItem.Checked = IsStoryTestingQuestionAnswersVisible && bBiblicalStory;
                theSE.viewRetellingFieldMenuItem.Checked = IsRetellingVisible && bBiblicalStory;
                theSE.viewConsultantNoteFieldMenuItem.Checked = IsConsultantNotesVisible;
                
                // disable the coach window also if the proj fac is logged in.
                //  (here we want to use "!=" for PF, because if he's also a LSR, then
                //  we want to be able to see Coach note pane).
                theSE.viewCoachNotesFieldMenuItem.Checked = IsCoachNotesVisible
                                                            && (theSE.LoggedOnMember != null)
                                                            &&
                                                            (!TeamMemberData.IsUser(theSE.LoggedOnMember.MemberType,
                                                                                    TeamMemberData.UserTypes.
                                                                                        ProjectFacilitator) ||
                                                             TeamMemberData.IsUser(theSE.LoggedOnMember.MemberType,
                                                                                   TeamMemberData.UserTypes.
                                                                                       FirstPassMentor));

                theSE.viewNetBibleMenuItem.Checked = IsNetBibleVisible && bBiblicalStory;
            }
#endif
            public const string CstrElementLabelProjectStates = "ProjectStates";
            public const string CstrAttributeLabelVersion = "version";
            public const string CstrElementLabelStateTransition = "StateTransition";
            public const string CstrAttributeLabelStage = "stage";
            public const string CstrAttributeLabelMemberTypeWithEditToken = "MemberTypeWithEditToken";
            public const string CstrAttributeLabelRequiresUsingVernacular = "RequiresUsingVernacular";
            public const string CstrAttributeLabelRequiresUsingNationalBT = "RequiresUsingNationalBT";
            public const string CstrAttributeLabelRequiresUsingEnglishBT = "RequiresUsingEnglishBT";
            public const string CstrAttributeLableRequiresUsingOtherEnglishBTer = "RequiresUsingOtherEnglishBTer";
            public const string CstrAttributeLabelRequiresUsingFreeTranslation = "RequiresUsingFreeTranslation";
            public const string CstrAttributeLableRequiresFirstPassMentor = "RequiresFirstPassMentor";
            public const string CstrAttributeLabelRequiresBiblicalStory = "RequiresBiblicalStory";
            public const string CstrAttributeLabelRequiresNonBiblicalStory = "RequiresNonBiblicalStory";
            public const string CstrAttributeLabelRequiresManageWithCoaching = "RequiresManageWithCoaching";
            public const string CstrAttributeLabelDontShowTilPast = "DontShowTilPast";
            public const string CstrElementLabelStageDisplayString = "StageDisplayString";
            public const string CstrElementLabelTransitionDisplayString = "TransitionDisplayString";
            public const string CstrElementLabelStageInstructions = "StageInstructions";
            public const string CstrElementLabelViewSettings = "ViewSettings";
            public const string CstrAttributeLabelViewVernacularLangField = "viewVernacularLangField";
            public const string CstrAttributeLabelViewNationalLangField = "viewNationalLangField";
            public const string CstrAttributeLabelViewEnglishBTField = "viewEnglishBTField";
            public const string CstrAttributeLabelViewFreeTranslationField = "viewFreeTranslationField";
            public const string CstrAttributeLabelViewAnchorField = "viewAnchorField";
            public const string CstrAttributeLabelViewStoryTestingQuestions = "viewStoryTestingQuestions";
            public const string CstrAttributeLabelViewStoryTestingAnswers = "viewStoryTestingAnswers";
            public const string CstrAttributeLabelViewRetellingField = "viewRetellingField";
            public const string CstrAttributeLabelViewConsultantNoteField = "viewConsultantNoteField";
            public const string CstrAttributeLabelViewCoachNotesField = "viewCoachNotesField";
            public const string CstrAttributeLabelViewNetBible = "viewNetBible";

            public XElement GetXml
            {
                get
                {
                    XElement elem = new XElement(CstrElementLabelStateTransition,
                        new XAttribute(CstrAttributeLabelStage, CurrentStage),
                        new XAttribute(CstrAttributeLabelMemberTypeWithEditToken, MemberTypeWithEditToken));

                    if (RequiresUsingVernacular)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresUsingVernacular, RequiresUsingVernacular));

                    if (RequiresUsingNationalBT)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresUsingNationalBT, RequiresUsingNationalBT));

                    if (RequiresUsingEnglishBT)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresUsingEnglishBT, RequiresUsingEnglishBT));

                    if (HasUsingOtherEnglishBTer)
                        elem.Add(new XAttribute(CstrAttributeLableRequiresUsingOtherEnglishBTer, RequiresUsingOtherEnglishBTer));

                    if (RequiresUsingFreeTranslation)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresUsingFreeTranslation, RequiresUsingFreeTranslation));

                    if (RequiresFirstPassMentor)
                        elem.Add(new XAttribute(CstrAttributeLableRequiresFirstPassMentor, RequiresFirstPassMentor));

                    if (RequiresBiblicalStory)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresBiblicalStory, RequiresBiblicalStory));

                    if (RequiresNonBiblicalStory)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresNonBiblicalStory, RequiresNonBiblicalStory));

                    if (RequiresManageWithCoaching)
                        elem.Add(new XAttribute(CstrAttributeLabelRequiresManageWithCoaching, RequiresManageWithCoaching));

                    if (DontShowTilPast)
                        elem.Add(new XAttribute(CstrAttributeLabelDontShowTilPast, DontShowTilPast));

                    elem.Add(new XElement(CstrElementLabelStageDisplayString, StageDisplayString),
                             new XElement(CstrElementLabelTransitionDisplayString, TransitionDisplayString),
                             new XElement(CstrElementLabelStageInstructions, StageInstructions),
                             AllowableForewardsTransitions.GetXml,
                             AllowableBackwardsTransitions.GetXml,
                             new XElement(CstrElementLabelViewSettings,
                                new XAttribute(CstrAttributeLabelViewVernacularLangField, IsVernacularVisible),
                                new XAttribute(CstrAttributeLabelViewNationalLangField, IsNationalBTVisible),
                                new XAttribute(CstrAttributeLabelViewEnglishBTField, IsEnglishBTVisible),
                                new XAttribute(CstrAttributeLabelViewFreeTranslationField, IsFreeTranslationVisible),
                                new XAttribute(CstrAttributeLabelViewAnchorField, IsAnchorVisible),
                                new XAttribute(CstrAttributeLabelViewStoryTestingQuestions, IsStoryTestingQuestionVisible),
                                new XAttribute(CstrAttributeLabelViewStoryTestingAnswers, IsStoryTestingQuestionAnswersVisible),
                                new XAttribute(CstrAttributeLabelViewRetellingField, IsRetellingVisible),
                                new XAttribute(CstrAttributeLabelViewConsultantNoteField, IsConsultantNotesVisible),
                                new XAttribute(CstrAttributeLabelViewCoachNotesField, IsCoachNotesVisible),
                                new XAttribute(CstrAttributeLabelViewNetBible, IsNetBibleVisible)));

                    return elem;
                }
            }
        }
    }
}
