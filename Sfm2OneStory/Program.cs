using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    class Program
    {
        const string CstrCaption = "OneStory Project Importer";

        // tags in the SFM document to look for (hope everyone was consistent!)
        const string CstrReasonLable = "reason this story is in the set:";
        const string CstrStoryCrafter = "Story crafter:";
        const string CstrBackTranslator = "Backtranslator to Hindi:";
        const string CstrTestor1 = "Testing 1:";
        const string CstrTestor2 = "Testing 2:";

        // details about the project we're trying to convert
        /*
        const string CstrBtFile = @"L:\Pahari\Storying\Dogri\DogriStoriesBT.txt";
        const string CstrRetFile = @"L:\Pahari\Storying\Dogri\DogriTestRetellings.txt";
        const string CstrConFile = @"L:\Pahari\Storying\Dogri\ProjectConNotes.txt";
        const string CstrCoaFile = @"L:\Pahari\Storying\Dogri\CoachingNotes.txt";
        const string CstrDefOutputFile = @"C:\Code\StoryEditor\StoryEditor\Dogri.onestory";
        const string CstrVernName = "Dogri";
        const string CstrVernCode = "doj";
        const string CstrVernCodeSfm = @"\doj";
        const string CstrVernFullStop = "।";
        const string CstrNatlName = "Hindi";
        const string CstrNatlCode = "hi";
        const string CstrNatlCodeSfm = @"\hnd";
        const string CstrNatlFullStop = "।";
        const string CstrIntlName = "English";
        const string CstrIntlCode = "en";
        */
        const string CstrBtFile = @"L:\Pahari\Storying\Dogri\DogriStoriesBT.txt";
        const string CstrRetFile = @"L:\Pahari\Storying\Dogri\DogriTestRetellings.txt";
        const string CstrConFile = @"L:\Pahari\Storying\Dogri\ProjectConNotes.txt";
        const string CstrCoaFile = @"L:\Pahari\Storying\Dogri\CoachingNotes.txt";
        const string CstrDefOutputFile = @"C:\Code\StoryEditor\StoryEditorData\Dogri.onestory";
        const string CstrVernName = "Dogri";
        const string CstrVernCode = "doj";
        const string CstrVernCodeSfm = @"\doj";
        const string CstrVernFullStop = "।";
        const string CstrNatlName = "Hindi";
        const string CstrNatlCode = "hi";
        const string CstrNatlCodeSfm = @"\hnd";
        const string CstrNatlFullStop = "।";
        const string CstrIntlName = "English";
        const string CstrIntlCode = "en";
        const string CstrConsultantsInitials = "BE";
        const string CstrCoachsInitials = "JP";

        private static readonly List<string> CastrConsultantMenteeSfms = new List<string>
                                                           {
                                                               @"\fac"
                                                           };
        private static readonly List<string> CastrCoachMenteeSfms = new List<string>
                                                           {
                                                               @"\con", // should only be one, but I wasn't consistent
                                                               @"\fac"
                                                           };

        // SFMs you might have which don't correspond to data in the new paradigm
        protected static List<string> lstSfmsToIgnore = new List<string>
        {
            @"\ash",
            @"\dt",
            @"\old",
            @"\ohnd"
        };

#if DEBUG
        [STAThreadAttribute]
#endif
        static void Main(string[] args)
        {
            string[] astrBt = File.ReadAllLines(CstrBtFile);
            CleanSfmFileContents(ref astrBt);
            string[] astrRet = File.ReadAllLines(CstrRetFile);
            CleanSfmFileContents(ref astrRet);
            string[] astrCon = File.ReadAllLines(CstrConFile);
            CleanSfmFileContents(ref astrCon);
            string[] astrCoa = File.ReadAllLines(CstrCoaFile);
            CleanSfmFileContents(ref astrCoa);
            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Browse for the existing version of the output file (so we can reuse the GUIDs)";
            StoriesData theStories;
            bool bUsingStoryProject;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StoryProject projFile = new StoryProject();
                projFile.ReadXml(dlg.FileName);
                ProjectSettings projSettings = new ProjectSettings(Path.GetDirectoryName(dlg.FileName), Path.GetFileNameWithoutExtension(dlg.FileName));

                theStories = new StoriesData(projFile, projSettings);
                
                // import the member ids so we keep their same guids
                foreach (TeamMemberData aTMD in theStories.TeamMembers.Values)
                    MemberGuid(theStories.TeamMembers, aTMD.Name, aTMD.MemberType);

                bUsingStoryProject = true;
            }
            else
            {
                ProjectSettings projSettings = new ProjectSettings(Path.GetDirectoryName(CstrDefOutputFile), Path.GetFileNameWithoutExtension(CstrDefOutputFile));
                theStories = new StoriesData(projSettings);
                bUsingStoryProject = false;
            }

            // skip down to the title of the first story (in each file)
            int nIndexBt = 0, nIndexRet = 0, nIndexCon = 0, nIndexCoa = 0;
            SkipTo(@"\t ", null, @"\t ", astrBt, ref nIndexBt);
            SkipTo(@"\t ", null, @"\t ", astrRet, ref nIndexRet);
            SkipTo(@"\t ", null, @"\t ", astrCon, ref nIndexCon);
            SkipTo(@"\t ", null, @"\t ", astrCoa, ref nIndexCoa);

            int nStoryNumber = 0;
            while (nIndexBt != -1)
            {
                System.Diagnostics.Debug.Assert(
                    (astrBt[nIndexBt] == astrRet[nIndexRet])
                    && (astrBt[nIndexBt] == astrCon[nIndexCon])
                    && (astrBt[nIndexBt] == astrCoa[nIndexCoa]), "Fixup problem in data files: one file has an extra record");

                string strStoryName = astrBt[nIndexBt].Substring(3);
                StoryData story;
                if (bUsingStoryProject)
                    story = theStories[nStoryNumber++];
                else
                    story = new StoryData(strStoryName, null);

                Console.WriteLine("Story: " + strStoryName);

                // first process the 'story front matter (reason, testors, etc)'
                string strTestorGuid;
                DoStoryFrontMatter(theStories, story, astrBt, ref nIndexBt, bUsingStoryProject, out strTestorGuid);

                int nVerseNumber = 0;
                while (nIndexBt < astrBt.Length)
                {
                    string strRef = astrBt[nIndexBt].Substring(4);
                    Console.WriteLine(" ln: " + strRef);

                    VerseData verse;
                    if (bUsingStoryProject)
                    {
                        System.Diagnostics.Debug.Assert(nVerseNumber < story.Verses.Count);
                        verse = story.Verses[nVerseNumber];

                        // clear out any data that doesn't have guids (so we don't add them again)
                        verse.Anchors.Clear();
                        verse.TestQuestions.Clear();    // these have IDs, but they're member IDs which should get picked up correctly
                        verse.Retellings.Clear();       // same here
                    }
                    else
                        verse = new VerseData();

                    if (DoVerseData(verse, astrBt, strTestorGuid, ref nIndexBt))
                    {
                        // now grab the retelling
                        if (SkipTo(@"\ln ", strRef, @"\c ", astrRet, ref nIndexRet))
                        {
                            DoRetellingData(astrRet, ref nIndexRet, verse, strTestorGuid);
                        }

                        // now grab the consultant notes
                        if (SkipTo(@"\ln ", strRef, @"\c ", astrCon, ref nIndexCon))
                        {
                            DoConsultData(astrCon, ref nIndexCon, bUsingStoryProject, verse.ConsultantNotes, @"\con", CstrConsultantsInitials,
                                CastrConsultantMenteeSfms, ConsultNoteDataConverter.CommunicationDirections.eConsultantToCrafter,
                                ConsultNoteDataConverter.CommunicationDirections.eCrafterToConsultant);
                        }

                        // now grab the coach notes
                        if (SkipTo(@"\ln ", strRef, @"\c ", astrCoa, ref nIndexCoa))
                        {
                            DoConsultData(astrCoa, ref nIndexCoa, bUsingStoryProject, verse.CoachNotes, @"\cch", CstrCoachsInitials,
                                CastrCoachMenteeSfms, ConsultNoteDataConverter.CommunicationDirections.eCoachToConsultant,
                                ConsultNoteDataConverter.CommunicationDirections.eConsultantToCoach);
                        }

                        System.Diagnostics.Debug.Assert(verse.HasData);
                        if (bUsingStoryProject)
                            nVerseNumber++;
                        else
                            story.Verses.Add(verse);
                    }

                    if ((nIndexBt >= astrBt.Length) || (astrBt[nIndexBt].Substring(0,3) == @"\c "))
                        break;
                }

                story.ProjStage.ProjectStage = StoryStageLogic.ProjectStages.eCoachReviewTest2Notes;
                if (!bUsingStoryProject)
                    theStories.Add(story);
                SkipTo(@"\t ", null, @"\t ", astrBt, ref nIndexBt);
                SkipTo(@"\t ", null, @"\t ", astrRet, ref nIndexRet);
                SkipTo(@"\t ", null, @"\t ", astrCon, ref nIndexCon);
                SkipTo(@"\t ", null, @"\t ", astrCoa, ref nIndexCoa);
            }

            theStories.ProjSettings.Vernacular.LangName = CstrVernName;
            theStories.ProjSettings.Vernacular.LangCode = CstrVernCode;
            theStories.ProjSettings.Vernacular.FullStop = CstrVernFullStop;
            theStories.ProjSettings.NationalBT.LangName = CstrNatlName;
            theStories.ProjSettings.NationalBT.LangCode = CstrNatlCode;
            theStories.ProjSettings.NationalBT.FullStop = CstrNatlFullStop;
            theStories.ProjSettings.InternationalBT.LangName = CstrIntlName;
            theStories.ProjSettings.InternationalBT.LangCode = CstrIntlCode;
            SaveXElement(theStories.GetXml, theStories.ProjSettings.ProjectFileName);
        }

        private static void DoStoryFrontMatter(StoriesData theStories, StoryData story, string[] astrBt, ref int nIndexBt, 
            bool bUsingStoryProject, out string strTestorGuid)
        {
            strTestorGuid = null;
            string strMarker, strData;
            ParseLine(astrBt[++nIndexBt], out strMarker, out strData);
            while (strMarker != @"\ln")
            {
                if (strMarker == @"\co")    // comment field
                {
                    if (BeginsWith(strData, CstrReasonLable))
                    {
                        string strStoryPurpose = strData.Substring(CstrReasonLable.Length).Trim();
                        if (String.IsNullOrEmpty(strStoryPurpose))
                            strStoryPurpose = null;
                        if (bUsingStoryProject)
                            System.Diagnostics.Debug.Assert(story.CraftingInfo.StoryPurpose == strStoryPurpose);
                        else
                            story.CraftingInfo.StoryPurpose = strStoryPurpose;
                    }
                    else if (BeginsWith(strData, CstrStoryCrafter))
                    {
                        string strCrafterName = strData.Substring(CstrStoryCrafter.Length).Trim();
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strCrafterName));
                        string strCrafterGuid = MemberGuid(theStories.TeamMembers, strCrafterName, TeamMemberData.UserTypes.eCrafter);

                        if (bUsingStoryProject)
                        {
                            if (String.IsNullOrEmpty(story.CraftingInfo.StoryCrafterMemberID) && !String.IsNullOrEmpty(strCrafterGuid))
                                story.CraftingInfo.StoryCrafterMemberID = strCrafterGuid;
                            System.Diagnostics.Debug.Assert(story.CraftingInfo.StoryCrafterMemberID == strCrafterGuid);
                        }
                        else
                            story.CraftingInfo.StoryCrafterMemberID = strCrafterGuid;
                    }
                    else if (BeginsWith(strData, CstrBackTranslator))
                    {
                        string strBackTranslatorName = strData.Substring(CstrBackTranslator.Length).Trim();
                        string strBackTranslatorGuid = null;
                        if (!String.IsNullOrEmpty(strBackTranslatorName))
                            strBackTranslatorGuid = MemberGuid(theStories.TeamMembers, strBackTranslatorName, TeamMemberData.UserTypes.eUNS);

                        if (bUsingStoryProject)
                            System.Diagnostics.Debug.Assert(story.CraftingInfo.BackTranslatorMemberID == strBackTranslatorGuid);
                        else
                            story.CraftingInfo.BackTranslatorMemberID = strBackTranslatorGuid;
                    }
                    else if (BeginsWith(strData, CstrTestor1))
                    {
                        string strTestor1 = strData.Substring(CstrTestor1.Length).Trim();
                        if (!String.IsNullOrEmpty(strTestor1))
                        {
                            strTestorGuid = MemberGuid(theStories.TeamMembers, strTestor1, TeamMemberData.UserTypes.eUNS);
                            if (bUsingStoryProject)
                                System.Diagnostics.Debug.Assert(story.CraftingInfo.Testors[1] == strTestorGuid);
                            else
                                story.CraftingInfo.Testors.Add(1, strTestorGuid);
                        }
                    }
                    else if (BeginsWith(strData, CstrTestor2))
                    {
                        string strTestor2 = strData.Substring(CstrTestor2.Length).Trim();
                        if (!String.IsNullOrEmpty(strTestor2))
                        {
                            strTestorGuid = MemberGuid(theStories.TeamMembers, strTestor2, TeamMemberData.UserTypes.eUNS);
                            if (bUsingStoryProject)
                                System.Diagnostics.Debug.Assert(story.CraftingInfo.Testors[2] == strTestorGuid);
                            else
                                story.CraftingInfo.Testors.Add(2, strTestorGuid);
                        }
                    }
                }
                else if (strMarker == @"\c")    // chapter field
                    break;

                ParseLine(astrBt[++nIndexBt], out strMarker, out strData);
            }
        }

        private static bool DoVerseData(VerseData verse, string[] astrBt, string strTestorGuid, ref int nIndexBt)
        {
            int nTQIndex = -1;
            string strMarker, strData;
            ParseLine(astrBt[++nIndexBt], out strMarker, out strData);

            // check to see if this verse is empty
            if (strMarker == @"\ln")
                return false;

            bool bVerseMarkerHasData = false;
            while ((strMarker != @"\ln") && (strMarker != @"\c"))   // until we hit the next line or the next chapter
            {
                if (strMarker == CstrVernCodeSfm)
                {
                    verse.VernacularText.SetValue(strData);
                    bVerseMarkerHasData = true;
                }
                else if (strMarker == CstrNatlCodeSfm)
                {
                    verse.NationalBTText.SetValue(strData);
                    bVerseMarkerHasData = true;
                }
                else if (strMarker == @"\bt")
                {
                    verse.InternationalBTText.SetValue(strData);
                    bVerseMarkerHasData = true;
                }
                else if (strMarker == @"\anc")
                {
                    Dictionary<string, string> map = GetListOfAnchors(strData);
                    foreach (KeyValuePair<string, string> kvp in map)
                        verse.Anchors.AddAnchorData(kvp.Key, kvp.Value);
                }
                else if (strMarker == @"\tsth")
                {
                    verse.TestQuestions.AddTestQuestion();
                    verse.TestQuestions[++nTQIndex].QuestionVernacular.SetValue(strData);
                }
                else if (strMarker == @"\tst")
                    verse.TestQuestions[nTQIndex].QuestionEnglish.SetValue(strData);
                else if (strMarker == @"\ans")
                {
                    if (nTQIndex >= 0)
                    {
                        System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strTestorGuid));
                        verse.TestQuestions[nTQIndex].Answers.AddNewLine(strTestorGuid).SetValue(strData);
                    }
                }
                else if (strMarker == @"\cn")
                {
                    System.Diagnostics.Debug.Assert(verse.Anchors.Count > 0);
                    verse.Anchors[0].ExegeticalHelpNotes.AddExegeticalHelpNote(strData);
                }
                else if (lstSfmsToIgnore.Contains(strMarker))
                {
                    Console.WriteLine("Found, but don't care about marker:" + strMarker);
                }
                else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                    System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                // do next line
                if (++nIndexBt < astrBt.Length)
                    ParseLine(astrBt[nIndexBt], out strMarker, out strData);
                else
                    break;
            }

            return bVerseMarkerHasData;
        }

        static void DoConsultData(string[] astrFile, ref int nIndexLine, bool bUsingStoryProject, 
            ConsultNotesDataConverter theCNsD, string strMentorSfm, string strMentorInitials, List<string> astrMenteeSfms, 
            ConsultNoteDataConverter.CommunicationDirections eMentorToMentee, ConsultNoteDataConverter.CommunicationDirections eMenteeToMentor)
        {
            if (++nIndexLine >= astrFile.Length)
                return;

            int nConvNumber = 0, nNoteNumber = 0;
            string strMarker, strData;
            ParseLine(astrFile[nIndexLine], out strMarker, out strData);
            if (strMarker == @"\ln")
                return;

            while ((strMarker != @"\ln") && (strMarker != @"\c"))
            {
                ConsultNoteDataConverter con = null;
                if (theCNsD.Count > nConvNumber)
                    con = theCNsD[nConvNumber];

                int nRound = 1;
                if (strMarker == strMentorSfm)
                {
                    try
                    {
                        char chRound = (strData.Substring(0, 2) == strMentorInitials) ? strData[2] : '1';
                        nRound = Convert.ToInt32(chRound.ToString());
                    }
                    catch { }

                    if (con == null)
                        con = theCNsD.AddEmpty(nRound);

                    if (con.Count > nNoteNumber)
                    {
                        CommInstance aCI = con[nNoteNumber];
                        System.Diagnostics.Debug.Assert((aCI.ToString() == strData) && (aCI.Direction == eMentorToMentee));
                    }
                    else
                        con.Add(new CommInstance(strData, eMentorToMentee, Guid.NewGuid().ToString()));
                    
                    nNoteNumber++;
                }
                else if (astrMenteeSfms.Contains(strMarker))
                {
                    // sometimes the CIT has a comment for the coach
                    if (con == null)
                        con = theCNsD.AddEmpty(nRound);

                    if (con.Count > nNoteNumber)
                    {
                        CommInstance aCI = con[nNoteNumber];
                        System.Diagnostics.Debug.Assert((aCI.ToString() == strData) && (aCI.Direction == eMenteeToMentor));
                    }
                    else
                        con.Add(new CommInstance(strData, eMenteeToMentor, Guid.NewGuid().ToString()));

                    // for now, we just do two per conversation (or maybe 3 if the mentee started the conversation)
                    if (++nNoteNumber >= 2)
                    {
                        nConvNumber++;
                        nNoteNumber = 0;
                    }
                }
                else if (lstSfmsToIgnore.Contains(strMarker))
                {
                    Console.WriteLine("Found, but don't care about marker:" + strMarker);
                }
                else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                    System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                if (++nIndexLine < astrFile.Length)
                    ParseLine(astrFile[nIndexLine], out strMarker, out strData);
                else
                    break;
            }
        }

        static void DoRetellingData(string[] astrRet, ref int nIndexRet, VerseData verse, string strTestorGuid)
        {
            string strMarker, strData;
            ParseLine(astrRet[++nIndexRet], out strMarker, out strData);
            while ((strMarker != @"\ln") && (strMarker != @"\c"))
            {
                if (strMarker == @"\ret")
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strTestorGuid));
                    verse.Retellings.AddNewLine(strTestorGuid).SetValue(strData);
                }
                else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                    System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                ParseLine(astrRet[++nIndexRet], out strMarker, out strData);
            }
        }

        static void ParseLine(string strLine, out string strMarker, out string strData)
        {
            int nSpace = strLine.IndexOf(' ');
            if (nSpace == -1)
                strMarker = strData = null;
            else
            {
                strMarker = strLine.Substring(0, nSpace);
                strData = strLine.Substring(nSpace + 1);
            }
        }

        static bool SkipTo(string strSfmCodeFind, string strValue, string strSfmCodeStopBy, string[] astr, ref int nIndex)
        {
            int nLen = astr.Length;
            for (int i = nIndex; i < nLen; i++)
            {
                string strLine = astr[i];
                if (String.IsNullOrEmpty(strLine.Trim()))
                    continue;

                string strMarker, strData;
                ParseLine(strLine, out strMarker, out strData);
                strMarker += ' ';
                if (strMarker == strSfmCodeFind)
                {
                    if (!String.IsNullOrEmpty(strValue))
                    {
                        if (strData.CompareTo(strValue) < 0)
                        {
                            continue;
                        }
                        else if (strData.CompareTo(strValue) > 0)
                        {
                            System.Diagnostics.Debug.Assert(false, String.Format("subsidary files are missing verse: {0} {1}", strSfmCodeFind, strValue));
                            nIndex = i;
                            return false;
                        }
                    }
                    nIndex = i;
                    return true;
                }

                if (strMarker == strSfmCodeStopBy)
                {
                    nIndex = i + 1; // add one so we don't stop there again
                    return false;
                }
            }

            System.Diagnostics.Debug.Assert(nIndex == astr.Length);
            nIndex = -1;
            return false;
        }

        // set up some regex helpers to pick apart the anchors
        protected static Regex SearchRegExErrorsBad2ndPunct = new Regex(@"\b([1-3a-zA-Z][a-zA-Z]{2})[ :\.](\d{1,3})[ \.](\d{1,3})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        protected static Regex SearchRegExErrorsBad1stPunct = new Regex(@"\b([1-3a-zA-Z][a-zA-Z]{2})[ :](\d{1,3})[ :\.](\d{1,3})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        protected static Regex SearchRegExHyphenRangeOfAnchors = new Regex(@"([1-3a-zA-Z][a-zA-Z]{2}\.\d{2,3}\:)(\d{2}) ?- ?(\d{2})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        protected static Regex SearchRegExComma2ndAnchor = new Regex(@"([1-3a-zA-Z][a-zA-Z]{2}\.\d{2,3}\:)(\d{2}), ?(\d{2})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        protected static Regex SearchRegExNormalAnchor = new Regex(@"[1-3a-zA-Z][a-zA-Z]{2}\.\d{2,3}\:\d{2}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        private static readonly char[] achAnchorDelimiters = new [] { ';' };

        static Dictionary<string, string> GetListOfAnchors(string strParagraph)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(strParagraph));

            string[] aStrSentences = strParagraph.Split(achAnchorDelimiters, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> mapJt2Comment = new Dictionary<string, string>();
            string strPrefix = null;
            foreach (string str in aStrSentences)
            {
                string strTrimmed = str.Trim();
                if (!String.IsNullOrEmpty(strTrimmed))
                {
                    // look for things like "gen 02:01" or "gen:02:01" "psl:119:141" "1jn:01:01"
                    //  if you find something like this, in VS, you can find them (to fix them) via RegEx search with:
                    //  Find What: "{[1-3:c]:c^2}[ \:]{(:d^2|:d^3)}[ \.\:]{(:d^2|:d^3)}"
                    //  Replace with: "\1\.\2:\3"
                    MatchCollection mc = SearchRegExErrorsBad1stPunct.Matches(strTrimmed);
                    if (mc.Count > 0)
                    {
                        bool bAdded = false;
                        foreach (Match match in mc)
                            bAdded |= FixupAnchorError(strTrimmed, "{0}.{1}", match, mapJt2Comment);
                        if (bAdded)
                            continue;
                    }

                    // look for things like "gen.02.01" or "gen.02 01" "psl:119 141" "1jn:01 01"
                    //  if you find something like this, in VS, you can find them (to fix them) via RegEx search with:
                    //  Find What: "{[1-3:c]:c^2}[ \.\:]{(:d^2|:d^3)}[ \.]{(:d^2|:d^3)}"
                    //  Replace with: "\1\:\2"
                    mc = SearchRegExErrorsBad2ndPunct.Matches(strTrimmed);
                    if (mc.Count > 0)
                    {
                        bool bAdded = false;
                        foreach (Match match in mc)
                            bAdded |= FixupAnchorError(strTrimmed, "{0}:{1}", match, mapJt2Comment);
                        if (bAdded)
                            continue;
                    }

                    // look for things like, "gen.03:21, 24". Turn this into two distinct anchors
                    mc = SearchRegExComma2ndAnchor.Matches(strTrimmed);
                    if (mc.Count > 0)
                    {
                        System.Diagnostics.Debug.Assert(mc.Count == 1); // otherwise, there's probably some assumption we're breaking
                        foreach (Match match in mc)
                        {
                            string str1 = match.Groups[1].ToString();   // should be "gen.03:"
                            string str2 = match.Groups[2].ToString();   // should be "21"
                            string str3 = match.Groups[3].ToString();   // should be "24"
                            mapJt2Comment.Add(str1 + str2, strTrimmed);
                            mapJt2Comment.Add(str1 + str3, strTrimmed);
                        }
                        continue;
                    }

                    // look for things like, "gen.03:21 - 24". Turn this into a range of anchors
                    mc = SearchRegExHyphenRangeOfAnchors.Matches(strTrimmed);
                    if (mc.Count > 0)
                    {
                        System.Diagnostics.Debug.Assert(mc.Count == 1); // otherwise, there's probably some assumption we're breaking
                        Match match = mc[0];
                        string str1 = match.Groups[1].ToString();   // should be "gen.03:"
                        string str2 = match.Groups[2].ToString();   // should be "21"
                        string str3 = match.Groups[3].ToString();   // should be "24"
                        int nStart = Convert.ToInt32(str2);
                        int nEnd = Convert.ToInt32(str3);
                        System.Diagnostics.Debug.Assert(nEnd > nStart);

                        for (int i = nStart; i <= nEnd; i++)
                        {
                            string strJT = String.Format("{0}{1:D2}", str1, i);
                            mapJt2Comment.Add(strJT, strTrimmed);
                        }
                        continue;
                    }

                    // otherwise, just look for normal things like, "gen.03:21".
                    mc = SearchRegExNormalAnchor.Matches(strTrimmed);
                    if (mc.Count > 0)
                    {
                        foreach (Match match in mc)
                        {
                            strPrefix += strTrimmed;
                            mapJt2Comment.Add(match.Value, strPrefix);
                            strPrefix = null;
                        }
                    }
                    else
                    {
                        // this means that there wasn't an anchor here, so just attach it with
                        //  the next one you find.
                        strPrefix += strTrimmed + ',';
                    }
                }
            }
            return mapJt2Comment;
        }

        static bool FixupAnchorError(string strTrimmed, string strRepairFormat, Match match, Dictionary<string, string> mapJt2Comment)
        {
            string str1 = match.Groups[1].ToString();   // should be "gen.03" or "gen"
            string str2 = match.Groups[2].ToString();   // should be "01"     or "03:01"

            string strRepaired = String.Format(strRepairFormat, str1, str2);

            // These are questionable to change automatically, so just throw an error and let the user decide
            string strMsg = String.Format("Found a possible bad anchor:{0}{0}{1}{0}{0} in the string,{0}{0}{2}{0}{0}Shall I change this to: \"{3}\"?)",
                Environment.NewLine, match, strTrimmed, strRepaired);

            if (MessageBox.Show(strMsg, CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                mapJt2Comment.Add(strRepaired, strTrimmed);
                return true;
            }

            return false;
        }

        static bool BeginsWith(string strData, string strBeginning)
        {
            return (strData.Substring(0, Math.Min(strBeginning.Length, strData.Length)) == strBeginning);
        }

        static string MemberGuid(TeamMembersData members, string strName, TeamMemberData.UserTypes eType)
        {
            if (String.IsNullOrEmpty(strName))
                return strName;
            if (!members.ContainsKey(strName))
                members.Add(strName, new TeamMemberData(strName, eType, "mem-" + Guid.NewGuid(), null, null, null, null, null, null));
            return members[strName].MemberGuid;
        }

        static void SaveXElement(XElement elem, string strFilename)
        {
            // create the root portions of the XML document and tack on the fragment we've been building
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("StoryProject",
                    elem));

            // save it with an extra extn.
            doc.Save(strFilename);
        }

        protected static void CleanSfmFileContents(ref string[] aStrFileContents)
        {
            int i = 0, nLastSfmIndex = 0, nRemovedLines = 0;
            while (i < aStrFileContents.Length - 1)
            {
                if (aStrFileContents[i + 1].Length > 0)
                {
                    char cFirstCharOfNextLine = aStrFileContents[i + 1][0];

                    if ((cFirstCharOfNextLine != '\\')
                     && (cFirstCharOfNextLine != '\r'))
                    {
                        // this means this is a redundant line break so move it to the 
                        //  preceding line (but make sure there's only one space)
                        string strLine = aStrFileContents[nLastSfmIndex];

                        // if the last character is *not* a space, then put one in.
                        if (strLine[strLine.Length - 1] != ' ')
                            aStrFileContents[nLastSfmIndex] += ' ';

                        // and add the next line.
                        aStrFileContents[nLastSfmIndex] += aStrFileContents[i + 1];

                        // null it out
                        aStrFileContents[i + 1] = null;

                        // keep track of new size
                        nRemovedLines++;
                    }
                    else
                        nLastSfmIndex = i + 1;
                }

                i++;
            }

            string[] aStrs = new string[aStrFileContents.Length - nRemovedLines];
            int j = 0;
            for (i = 0; i < aStrFileContents.Length; i++)
                if (aStrFileContents[i] != null)
                    aStrs[j++] = aStrFileContents[i];

            aStrFileContents = aStrs;
        }
    }
}
