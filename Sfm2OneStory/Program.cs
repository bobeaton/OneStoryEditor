using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.IO;
using OneStoryProjectEditor;

namespace OneStoryProjectEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] astrBt = File.ReadAllLines(@"L:\Pahari\Storying\Kangri\KangriStoriesBT.txt");
            string[] astrRet = File.ReadAllLines(@"L:\Pahari\Storying\Kangri\KangriTestRetellings.txt");
            string[] astrCon = File.ReadAllLines(@"L:\Pahari\Storying\Kangri\ProjectConNotes.txt");
            string[] astrCoa = File.ReadAllLines(@"L:\Pahari\Storying\Kangri\CoachingNotes.txt");

            StoriesData theStories = new StoriesData();
            int nIndexBt = 0, nIndexRet = 0, nIndexCon = 0, nIndexCoa = 0;
            SkipTo(@"\t ", @"\t ", astrBt, ref nIndexBt);
            SkipTo(@"\t ", @"\t ", astrRet, ref nIndexRet);
            SkipTo(@"\t ", @"\t ", astrCon, ref nIndexCon);
            SkipTo(@"\t ", @"\t ", astrCoa, ref nIndexCoa);

            while (nIndexBt != -1)
            {
                string strStoryName = astrBt[nIndexBt].Substring(3);
                StoryData story = new StoryData(strStoryName, "mem-99CDE60E-453A-4947-8627-83B33223EF0B");
                Console.WriteLine("Story: " + strStoryName);
                if (SkipTo(@"\ln ", @"\t ", astrBt, ref nIndexBt))
                {
                    string strRef = astrBt[nIndexBt].Substring(4);
                    Console.WriteLine(" ln: " + strRef);
                    int nTQIndex = -1;
                    VerseData verse = new VerseData();
                    string strMarker, strData;
                    ParseLine(astrBt[++nIndexBt], out strMarker, out strData);
                    while ((strMarker != @"\ln") && (strMarker != @"\c"))
                    {
                        if (strMarker == @"\xnr")
                            verse.VernacularText = strData;
                        else if (strMarker == @"\hnd")
                            verse.NationalBTText = strData;
                        else if (strMarker == @"\bt")
                            verse.InternationalBTText = strData;
                        else if (strMarker == @"\anc")
                            verse.Anchors.AddAnchorData(strData);
                        else if (strMarker == @"\tsth")
                        {
                            verse.TestQuestions.AddTestQuestion();
                            verse.TestQuestions[++nTQIndex].QuestionVernacular = strData;
                        }
                        else if (strMarker == @"\tst")
                            verse.TestQuestions[nTQIndex].QuestionEnglish = strData;
                        else if (strMarker == @"\ans")
                        {
                            if (nTQIndex >= 0)
                                verse.TestQuestions[nTQIndex].Answers.AddNewLine(strData, "mem-FCAC1BB2-827F-47b8-B1BD-60245BB15392");
                        }
                        else if (strMarker == @"\cn")
                        {
                            System.Diagnostics.Debug.Assert(verse.Anchors.Count > 0);
                            verse.Anchors[0].ExegeticalHelpNotes.AddExegeticalHelpNote(strData);
                        }
                        else if ((strMarker == @"\ash") || (strMarker == @"\dt"))
                            Console.WriteLine("Found:" + strMarker);
                        else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                            System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));
                        
                        // do next line
                        ParseLine(astrBt[++nIndexBt], out strMarker, out strData);
                    }

                    // now grab the retelling
                    if (SkipTo(@"\ln " + strRef, @"\t ", astrRet, ref nIndexRet))
                    {
                        ParseLine(astrRet[++nIndexRet], out strMarker, out strData);
                        while ((strMarker != @"\ln") && (strMarker != @"\c"))
                        {
                            if (strMarker == @"\ret")
                                verse.Retellings.AddNewLine(strData, "mem-FCAC1BB2-827F-47b8-B1BD-60245BB15392");
                            else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                                System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                            ParseLine(astrRet[++nIndexRet], out strMarker, out strData);
                        }                    
                    }

                    // now grab the consultant notes
                    if (SkipTo(@"\ln " + strRef, @"\t ", astrCon, ref nIndexCon))
                    {
                        ParseLine(astrCon[++nIndexCon], out strMarker, out strData);
                        while ((strMarker != @"\ln") && (strMarker != @"\c"))
                        {
                            if (strMarker == @"\con")
                            {
                                ConsultNoteDataConverter con = new ConsultantNoteData();
                                con.MentorComment = strData;
                                con.MentorGuid = "mem-8B45075E-6434-4755-81AE-E1EEB9134D5B";
                                int nRound = 1;
                                try
                                {
                                    char chRound = (strData.Substring(0, 2) == "BE") ? strData[2] : '1';
                                    nRound = Convert.ToInt32(chRound.ToString());
                                }
                                catch { }
                                con.RoundNum = nRound;
                                verse.ConsultantNotes.Add(con);
                            }
                            else if (strMarker == @"\fac")
                            {
                                System.Diagnostics.Debug.Assert(verse.ConsultantNotes.Count > 0);
                                ConsultNoteDataConverter con = verse.ConsultantNotes[verse.ConsultantNotes.Count - 1];
                                con.MenteeResponse = strData;
                                con.MenteeGuid = "mem-99CDE60E-453A-4947-8627-83B33223EF0B";
                            }
                            else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                                System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                            ParseLine(astrCon[++nIndexCon], out strMarker, out strData);
                        }
                    }

                    // now grab the coach notes
                    if (SkipTo(@"\ln " + strRef, @"\t ", astrCoa, ref nIndexCoa))
                    {
                        ParseLine(astrCoa[++nIndexCoa], out strMarker, out strData);
                        while ((strMarker != @"\ln") && (strMarker != @"\c"))
                        {
                            if (strMarker == @"\cch")
                            {
                                ConsultNoteDataConverter coa = new CoachNoteData();
                                verse.CoachNotes.Add(coa);
                                coa.MentorComment = strData;
                                coa.MentorGuid = "mem-7064CC55-73C4-48bf-B2DC-9989A5ED9D1B";
                                int nRound = 1;
                                try
                                {
                                    char chRound = (strData.Substring(0, 2) == "JP") ? strData[2] : '1';
                                    nRound = Convert.ToInt32(chRound);
                                }
                                catch { }
                                coa.RoundNum = nRound;
                            }
                            else if ((strMarker == @"\con") || (strMarker == @"\fac"))
                            {
                                if (verse.CoachNotes.Count == 0)
                                {
                                    ConsultNoteDataConverter coa2 = new CoachNoteData();
                                    verse.CoachNotes.Add(coa2);
                                }
                                System.Diagnostics.Debug.Assert(verse.CoachNotes.Count > 0);
                                ConsultNoteDataConverter coa = verse.CoachNotes[verse.CoachNotes.Count - 1];
                                coa.MenteeResponse = strData;
                                coa.MenteeGuid = "mem-8B45075E-6434-4755-81AE-E1EEB9134D5B";
                            }
                            else if (strMarker == @"\dt")
                                Console.WriteLine("Found:" + strMarker);
                            else if (!String.IsNullOrEmpty(strMarker) && !String.IsNullOrEmpty(strData))
                                System.Diagnostics.Debug.Assert(false, String.Format("not handling the '{0}' marker", strMarker));

                            ParseLine(astrCoa[++nIndexCoa], out strMarker, out strData);
                        }
                    }

                    story.Verses.Add(verse);
                }
                theStories.Add(story);
            }

            SaveXElement(theStories.GetXml, @"C:\Code\StoryEditor\StoryEditor\Kangri.onestory");
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

        static bool SkipTo(string strSfmCodeFind, string strSfmCodeStopBy, string[] astr, ref int nIndex)
        {
            int nLen = astr.Length;
            for (int i = nIndex + 1; i < nLen; i++)
            {
                string strLine = astr[i];
                if (String.IsNullOrEmpty(strLine.Trim()))
                    continue;
                if (strLine.Substring(0, Math.Min(strLine.Length, strSfmCodeFind.Length)) == strSfmCodeFind)
                {
                    nIndex = i;
                    return true;
                }
                else if (strLine.Substring(0, Math.Min(strLine.Length, strSfmCodeStopBy.Length)) == strSfmCodeStopBy)
                {
                    nIndex = i;
                    return false;
                }
            }

            nIndex = -1;
            return false;
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
    }
}
