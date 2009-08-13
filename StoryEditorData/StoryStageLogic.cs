using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace OneStoryProjectEditor
{
    public class StoryStageLogic
    {
        protected ProjectStages _ProjectStage = ProjectStages.eUndefined;
        protected const string CstrDefaultProjectStage = "CrafterTypeNationalBT";
        internal static StateTransitions stateTransitions = new StateTransitions();

        public enum ProjectStages
        {
            eUndefined = 0,
            eCrafterTypeNationalBT,
            eCrafterTypeInternationalBT,
            eCrafterAddAnchors,
            eCrafterAddStoryQuestions,
            eConsultantCheckAnchors,
            eConsultantCheckStoryQuestions,
            eCoachReviewRound1Notes,
            eConsultantReviseRound1Notes,
            eCrafterReviseBasedOnRound1Notes,
            eCrafterOnlineReview1WithConsultant,
            eCrafterReadyForTest1,
            eCrafterEnterAnswersToStoryQuestionsOfTest1,
            eCrafterEnterRetellingOfTest1,
            eConsultantCheckAnchorsRound2,
            eConsultantCheckAnswersToTestingQuestionsRound2,
            eConsultantCheckRetellingRound2,
            eCoachReviewRound2Notes,
            eConsultantReviseRound2Notes,
            eCrafterReviseBasedOnRound2Notes,
            eCrafterOnlineReview2WithConsultant,
            eCrafterReadyForTest2,
            eCrafterEnterAnswersToStoryQuestionsOfTest2,
            eCrafterEnterRetellingOfTest2,
            eConsultantReviewTest2,
            eCoachReviewTest2Notes,
            eTeamComplete
        }

        public ProjectStages ProjectStage
        {
            get { return _ProjectStage; }
            set { _ProjectStage = value; }
        }

        public StoryStageLogic()
        {
            ProjectStage = ProjectStages.eCrafterTypeNationalBT;
        }

        public StoryStageLogic(string strProjectStage)
        {
            ProjectStage = GetProjectStageFromString(strProjectStage);
        }

        protected ProjectStages GetProjectStageFromString(string strProjectStageString)
        {
            System.Diagnostics.Debug.Assert(CmapStageStringToEnumType.ContainsKey(strProjectStageString));
            return CmapStageStringToEnumType[strProjectStageString];
        }

        /*
        public void SetViewBasedOnProjectStage(StoryEditor theSE, ProjectStages eStage, out string strStatusMessage, out string strTooltipMessage)
        {
            StageTransition st = stateTransitions[eStage];
            st.SetView(theSE, out strTooltipMessage);
            strStatusMessage = st.StageDisplayString;
        }
        */
        public override string ToString()
        {
            return _ProjectStage.ToString().Substring(1);
        }

        protected static Dictionary<string, StoryStageLogic.ProjectStages> CmapStageStringToEnumType = new Dictionary<string, StoryStageLogic.ProjectStages>() {
            { "CrafterTypeNationalBT", ProjectStages.eCrafterTypeNationalBT },
            { "CrafterTypeInternationalBT", ProjectStages.eCrafterTypeInternationalBT },
            { "CrafterAddAnchors", ProjectStages.eCrafterAddAnchors },
            { "CrafterAddStoryQuestions", ProjectStages.eCrafterAddStoryQuestions },
			{ "ConsultantCheckAnchors", ProjectStages.eConsultantCheckAnchors },
            { "ConsultantCheckStoryQuestions", ProjectStages.eConsultantCheckStoryQuestions },
            { "CoachReviewRound1Notes", ProjectStages.eCoachReviewRound1Notes },
            { "ConsultantReviseRound1Notes", ProjectStages.eConsultantReviseRound1Notes },
            { "CrafterReviseBasedOnRound1Notes", ProjectStages.eCrafterReviseBasedOnRound1Notes },
            { "CrafterOnlineReview1WithConsultant", ProjectStages.eCrafterOnlineReview1WithConsultant },
            { "CrafterReadyForTest1", ProjectStages.eCrafterReadyForTest1 },
            { "CrafterEnterAnswersToStoryQuestionsOfTest1", ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1 },
            { "CrafterEnterRetellingOfTest1", ProjectStages.eCrafterEnterRetellingOfTest1 },
            { "ConsultantCheckAnchorsRound2", ProjectStages.eConsultantCheckAnchorsRound2 },
            { "ConsultantCheckAnswersToTestingQuestionsRound2", ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2 },
            { "ConsultantCheckRetellingRound2", ProjectStages.eConsultantCheckRetellingRound2 },
            { "CoachReviewRound2Notes", ProjectStages.eCoachReviewRound2Notes },
            { "ConsultantReviseRound2Notes", ProjectStages.eConsultantReviseRound2Notes },
            { "CrafterReviseBasedOnRound2Notes", ProjectStages.eCrafterReviseBasedOnRound2Notes },
            { "CrafterOnlineReview2WithConsultant", ProjectStages.eCrafterOnlineReview2WithConsultant },
            { "CrafterReadyForTest2", ProjectStages.eCrafterReadyForTest2 },
            { "CrafterEnterAnswersToStoryQuestionsOfTest2", ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2 },
            { "CrafterEnterRetellingOfTest2", ProjectStages.eCrafterEnterRetellingOfTest2 },
            { "ConsultantReviewTest2", ProjectStages.eConsultantReviewTest2 },
            { "CoachReviewTest2Notes", ProjectStages.eCoachReviewTest2Notes },
            { "TeamComplete", ProjectStages.eTeamComplete }};

        public class StateTransitions : Dictionary<ProjectStages, StageTransition>
        {
            protected const string CstrStateTransitionsXmlFilename = "StageTransitions.xml";

            public StateTransitions()
            {
                InitStateTransitionsFromXml();
            }

            protected string PathToXmlFile
            {
                get
                {
                    // try the same folder as we're executing out of
                    string strCurrentFolder = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                    strCurrentFolder = Path.GetDirectoryName(strCurrentFolder);
                    string strFileToCheck = String.Format(@"{0}\{1}", strCurrentFolder, CstrStateTransitionsXmlFilename);
#if DEBUG
                    if (!File.Exists(strFileToCheck))
                        // on dev machines, this file is in the "..\..\src\EC\TECkit Mapping Editor" folder
                        strFileToCheck = @"C:\code\StoryEditor\StoryEditor\" + CstrStateTransitionsXmlFilename;
#endif
                    System.Diagnostics.Debug.Assert(File.Exists(strFileToCheck), String.Format("Can't find: {0}! You'll need to re-install or contact bob_eaton@sall.com", strFileToCheck));

                    return strFileToCheck;
                }
            }

            protected void GetXmlDocument(out XmlDocument doc, out XPathNavigator navigator, out XmlNamespaceManager manager)
            {
                doc = new XmlDocument();
                doc.Load(PathToXmlFile);
                navigator = doc.CreateNavigator();
                manager = new XmlNamespaceManager(navigator.NameTable);
            }

            protected void InitStateTransitionsFromXml()
            {
                System.Diagnostics.Debug.WriteLine("Starting InitStateTransitionsFromXml");
                /* e.g.
  <StageTransition stage="eCrafterTypeInternationalBT">
    <Transition MemberWithEditToken="eCrafter" NextState="eCrafterAddAnchors">
      <StageDisplayString>Crafter enters the English back-translation</StageDisplayString>
      <NextMemberTransitionMessage>If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.</NextMemberTransitionMessage>
      <StageInstructions></StageInstructions>
      <AllowableTransitions>
        <AllowableTransition>eCrafterTypeNationalBT</AllowableTransition>
        <AllowableTransition>eCrafterAddAnchors</AllowableTransition>
        <AllowableTransition>eCrafterAddStoryQuestions</AllowableTransition>
        <AllowableTransition>eConsultantCheckAnchors</AllowableTransition>
      </AllowableTransitions>
      <ViewSettings viewVernacularLangFieldMenuItem="false" viewNationalLangFieldMenuItem="true" viewEnglishBTFieldMenuItem="true" viewAnchorFieldMenuItem="false" viewStoryTestingQuestionFieldMenuItem="false" viewRetellingFieldMenuItem="false" viewConsultantNoteFieldMenuItem="false" viewCoachNotesFieldMenuItem="false" viewNetBibleMenuItem="false" />
    </Transition>
  </StageTransition>
                */
                try
                {
                    XmlDocument doc;
                    XPathNavigator navigator;
                    XmlNamespaceManager manager;
                    GetXmlDocument(out doc, out navigator, out manager);

                    XPathNodeIterator xpStageTransition = navigator.Select("/StageTransitions/StageTransition", manager);
                    while (xpStageTransition.MoveNext())
                    {
                        ProjectStages eThisStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpStageTransition.Current.GetAttribute("stage", navigator.NamespaceURI));

                        XPathNodeIterator xpTransition = xpStageTransition.Current.Select("Transition", manager);
                        if (xpTransition.MoveNext())
                        {
                            TeamMemberData.UserTypes eMemberType = (TeamMemberData.UserTypes)Enum.Parse(typeof(TeamMemberData.UserTypes), xpTransition.Current.GetAttribute("MemberWithEditToken", navigator.NamespaceURI));
                            ProjectStages eNextStage = (ProjectStages)Enum.Parse(typeof(ProjectStages), xpTransition.Current.GetAttribute("NextState", navigator.NamespaceURI));

                            XPathNodeIterator xpNextElement = xpTransition.Current.Select("StageDisplayString");
                            string strStageDisplayString = null;
                            if (xpNextElement.MoveNext())
                                strStageDisplayString = xpNextElement.Current.Value;

                            xpNextElement = xpTransition.Current.Select("NextMemberTransitionMessage");
                            string strNextMemberTransitionMessage = null;
                            if (xpNextElement.MoveNext())
                                strNextMemberTransitionMessage = xpNextElement.Current.Value;

                            xpNextElement = xpTransition.Current.Select("StageInstructions");
                            string strStageInstructions = null;
                            if (xpNextElement.MoveNext())
                                strStageInstructions = xpNextElement.Current.Value;

                            xpNextElement = xpTransition.Current.Select("AllowableTransitions/AllowableTransition");
                            List<ProjectStages> lstAllowableStages = new List<ProjectStages>();
                            while (xpNextElement.MoveNext())
                                lstAllowableStages.Add((ProjectStages)Enum.Parse(typeof(ProjectStages), xpNextElement.Current.Value));

                            xpNextElement = xpTransition.Current.Select("ViewSettings");
                            List<bool> lstViewStates = new List<bool>();
                            if (xpNextElement.MoveNext())
                            {
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewVernacularLangFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewNationalLangFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewEnglishBTFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewAnchorFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewStoryTestingQuestionFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewRetellingFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewConsultantNoteFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewCoachNotesFieldMenuItem", navigator.NamespaceURI) == "true");
                                lstViewStates.Add(xpNextElement.Current.GetAttribute("viewNetBibleMenuItem", navigator.NamespaceURI) == "true");
                            }

                            StageTransition st = new StageTransition(eThisStage, eNextStage, eMemberType, 
                                strStageDisplayString, strNextMemberTransitionMessage, strStageInstructions, 
                                lstAllowableStages, lstViewStates);

                            Add(eThisStage, st);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(String.Format("Unable to process the xml file containing the State Transitions (i.e. {0})... Reinstall.", CstrStateTransitionsXmlFilename), ex);
                }
                System.Diagnostics.Debug.WriteLine("Finished InitStateTransitionsFromXml");
            }
        }

        public class StageTransition
        {
            internal ProjectStages CurrentStage = ProjectStages.eUndefined;
            internal ProjectStages NextStage = ProjectStages.eUndefined;
            internal List<ProjectStages> AllowableTransitions = new List<ProjectStages>();
            internal TeamMemberData.UserTypes MemberTypeWithEditToken = TeamMemberData.UserTypes.eUndefined;
            protected List<bool> _abViewSettings = null;
            internal string StageDisplayString = null;
            protected string _strTerminalTransitionMessage = null;
            protected string _strInstructions = null;

            public StageTransition(
                ProjectStages thisStage,
                ProjectStages theNextStage,
                TeamMemberData.UserTypes eMemberTypeWithEditToken,
                string strDisplayString,
                string strTerminalTransitionMessage,
                string strInstructions,
                List<ProjectStages> lstAllowableStages, List<bool> abViewSettings)
            {
                CurrentStage = thisStage;
                NextStage = theNextStage;
                MemberTypeWithEditToken = eMemberTypeWithEditToken;
                StageDisplayString = strDisplayString;
                _strTerminalTransitionMessage = strTerminalTransitionMessage;
                _strInstructions = strInstructions;
                AllowableTransitions.AddRange(lstAllowableStages);
                _abViewSettings = abViewSettings;
                string strMethodName = thisStage.ToString().Substring(1);
            }

            public bool IsValidForTransition(ProjectStages eToStage, out string strTerminalTransitionMessage)
            {
                bool bAllowed = AllowableTransitions.Contains(eToStage);
                if (bAllowed && (eToStage == AllowableTransitions[AllowableTransitions.Count - 1]))
                    strTerminalTransitionMessage = _strTerminalTransitionMessage;
                else
                    strTerminalTransitionMessage = null;
                return bAllowed;
            }

            /*
            public bool CheckForValidEndOfState(StoryEditor theSE, ProjectSettings theProjSettings, StoryData theCurrentStory)
            {
                return IsReadyForTransition(theSE, theProjSettings, theCurrentStory);
            }

            public void SetView(StoryEditor theSE, out string strInstructions)
            {
                theSE.viewVernacularLangFieldMenuItem.Checked = _abViewSettings[0];
                theSE.viewNationalLangFieldMenuItem.Checked = _abViewSettings[1];
                theSE.viewEnglishBTFieldMenuItem.Checked = _abViewSettings[2];
                theSE.viewAnchorFieldMenuItem.Checked = _abViewSettings[3];
                theSE.viewStoryTestingQuestionFieldMenuItem.Checked = _abViewSettings[4];
                theSE.viewRetellingFieldMenuItem.Checked = _abViewSettings[5];
                theSE.viewConsultantNoteFieldMenuItem.Checked = _abViewSettings[6];
                theSE.viewCoachNotesFieldMenuItem.Checked = _abViewSettings[7];
                theSE.viewNetBibleMenuItem.Checked = _abViewSettings[8];
                strInstructions = _strInstructions;
            }
            */

            public XElement GetXml
            {
                get
                {
                    XElement elemAllowableTransition = new XElement("AllowableTransitions");
                    foreach (ProjectStages ps in AllowableTransitions)
                        elemAllowableTransition.Add(new XElement("AllowableTransition", ps));

                    XElement elem = new XElement("Transition", 
                        new XAttribute("MemberWithEditToken", MemberTypeWithEditToken),
                        new XAttribute("NextState", NextStage),
                        new XElement("StageDisplayString", StageDisplayString),
                        new XElement("NextMemberTransitionMessage", _strTerminalTransitionMessage),
                        new XElement("StageInstructions", _strInstructions),
                        elemAllowableTransition,
                        new XElement("ViewSettings",
                            new XAttribute("viewVernacularLangFieldMenuItem", _abViewSettings[0]),
                            new XAttribute("viewNationalLangFieldMenuItem", _abViewSettings[1]),
                            new XAttribute("viewEnglishBTFieldMenuItem", _abViewSettings[2]),
                            new XAttribute("viewAnchorFieldMenuItem", _abViewSettings[3]),
                            new XAttribute("viewStoryTestingQuestionFieldMenuItem", _abViewSettings[4]),
                            new XAttribute("viewRetellingFieldMenuItem", _abViewSettings[5]),
                            new XAttribute("viewConsultantNoteFieldMenuItem", _abViewSettings[6]),
                            new XAttribute("viewCoachNotesFieldMenuItem", _abViewSettings[7]),
                            new XAttribute("viewNetBibleMenuItem", _abViewSettings[8])));

                    return elem;
                }
            }
        }

        #region static StageTransition implementation
        /*
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
                XElement elem = new XElement("StageTransitions");
                foreach (KeyValuePair<ProjectStages, StageTransition> kvp in CmapAllowableStageTransitions)
                {
                    elem.Add(new XElement("StageTransition", new XAttribute("stage", kvp.Key),
                        kvp.Value.GetXml));
                }
                return elem;
            }
        }

        protected static Dictionary<ProjectStages, StageTransition> CmapAllowableStageTransitions = new Dictionary<ProjectStages, StageTransition>()
        {
            { 
                // read this as: from the "CrafterTypeNationalBT" state, you (the Crafter) can go to the
                //  CrafterTypeInternationalBT, CrafterAddAnchors, CrafterAddStoryQuestions, or ConsultantAddRound1Notes
                //  state (the latter of which is the terminal stage (beyond which the edit token goes to the next)
                ProjectStages.eCrafterTypeNationalBT, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter, 
                    "Crafter enters the National language back-translation",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeInternationalBT, 
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterTypeInternationalBT, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter, 
                    "Crafter enters the English back-translation",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterAddAnchors, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter, 
                    "Crafter adds biblical anchors",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterAddStoryQuestions, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter, 
                    "Crafter adds story testing questions",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eConsultantCheckAnchors
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnchors, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining, 
                    "Consultant checks the story anchors",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckStoryQuestions,
                        ProjectStages.eCoachReviewRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckStoryQuestions, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining, 
                    "Consultant checks the story testing questions",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterTypeNationalBT, 
                        ProjectStages.eCrafterTypeInternationalBT,
                        ProjectStages.eCrafterAddAnchors,
                        ProjectStages.eCrafterAddStoryQuestions,
                        ProjectStages.eConsultantCheckAnchors,
                        ProjectStages.eCoachReviewRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewRound1Notes, 
                new StageTransition(TeamMemberData.UserTypes.eCoach,
                    "Coach reviews round 1 consultant notes",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant returns it to you for the next round of review.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviseRound1Notes, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant revises round 1 notes based on Coach's feedback",
                    "If you change to this stage, then you won't be able to edit the story until after the crafter has returned it to you after the first (formal) UNS test.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReviseBasedOnRound1Notes, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter revises story based on Consultant's feedback",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterOnlineReview1WithConsultant, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter has 1st online review with consultant",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReadyForTest1, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter is ready for the 1st (formal) UNS test",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter enters answers to the test 1 story questions",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterRetellingOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterRetellingOfTest1, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter enters test 1 retelling back-translation",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eCrafterOnlineReview1WithConsultant,
                        ProjectStages.eCrafterReadyForTest1,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest1,
                        ProjectStages.eConsultantCheckAnchorsRound2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnchorsRound2, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant checks story anchors for round 2",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2,
                        ProjectStages.eConsultantCheckRetellingRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant checks test 1 answers to story testing questions",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnchorsRound2,
                        ProjectStages.eConsultantCheckRetellingRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantCheckRetellingRound2, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant checks test 1 retelling",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound1Notes,
                        ProjectStages.eConsultantCheckAnchorsRound2,
                        ProjectStages.eConsultantCheckAnswersToTestingQuestionsRound2,
                        ProjectStages.eCoachReviewRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewRound2Notes, 
                new StageTransition(TeamMemberData.UserTypes.eCoach,
                    "Coach reviews test 1 notes",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it for round 2.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviseRound2Notes, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant revises round 2 notes based on Coach's feedback",
                    "If you change to this stage, then you won't be able to edit the story until after the crafter has returned it for a final review.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReviseBasedOnRound2Notes, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter revises story based on round 2 notes",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterOnlineReview2WithConsultant, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter has 2nd online review with consultant",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterReadyForTest2, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter is ready for the second (formal) UNS test",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter enters test 2 answers to story testing questions",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterRetellingOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        false,  // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        false,  // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCrafterEnterRetellingOfTest2, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Crafter enters test 2 retelling back-translation",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCrafterOnlineReview2WithConsultant,
                        ProjectStages.eCrafterReadyForTest2,
                        ProjectStages.eCrafterEnterAnswersToStoryQuestionsOfTest2,
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        false,  // viewAnchorFieldMenuItem
                        false,  // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        false,  // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        false   // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eConsultantReviewTest2, 
                new StageTransition(TeamMemberData.UserTypes.eConsultantInTraining,
                    "Consultant reviews test 2 results",
                    "If you change to this stage, then you won't be able to edit the story until after the coach has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eCrafterReviseBasedOnRound2Notes,
                        ProjectStages.eCoachReviewTest2Notes
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        false,  // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eCoachReviewTest2Notes, 
                new StageTransition(TeamMemberData.UserTypes.eCoach,
                    "Coach reviews test 2 results",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has returned it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviseRound2Notes,
                        ProjectStages.eTeamComplete
                    },
                    new List<bool>
                    {
                        false,  // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            },
            { 
                ProjectStages.eTeamComplete, 
                new StageTransition(TeamMemberData.UserTypes.eCrafter,
                    "Story testing complete (waiting for panorama completion review)",
                    "If you change to this stage, then you won't be able to edit the story until after the consultant has reviewed it.",
                    "",
                    new List<ProjectStages>
                    {
                        ProjectStages.eConsultantReviewTest2
                    },
                    new List<bool>
                    {
                        true,   // viewVernacularLangFieldMenuItem
                        true,   // viewNationalLangFieldMenuItem
                        true,   // viewEnglishBTFieldMenuItem
                        true,   // viewAnchorFieldMenuItem
                        true,   // viewStoryTestingQuestionFieldMenuItem
                        true,   // viewRetellingFieldMenuItem
                        true,   // viewConsultantNoteFieldMenuItem
                        true,   // viewCoachNotesFieldMenuItem
                        true    // viewNetBibleMenuItem
                    } )
            }
        };
        */
        #endregion static StageTransition implementation
    }
}
