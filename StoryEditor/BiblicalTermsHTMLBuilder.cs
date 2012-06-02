using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NetLoc;
using Paratext;
using System.Text.RegularExpressions;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
	/// <summary>
	/// Build the HTML text for the references display of the Biblical Terms tool.
	/// </summary>
	public class BiblicalTermsHTMLBuilder
	{
		List<string> scrTextNames;   // Texts being displayed.

		// Values in the template which change for each reference.
		Dictionary<string, string> referenceVariables;
		// Values in the template which are the same for each reference.
        List<Dictionary<string, string>> projectVariablesList;  // e.g. "Kangri" to Dictionary<"FontFace" to "Arial Unicode MS">

        Dictionary<string, List<string>> mapReferenceToVerseTextList; // e.g. "Spiritual World Verse 1" to List<"<vern>", "<natlBT>", "<EnglBT>">

		// Renderings for current term by project
		List<TermRendering> termRenderingsList;

		HTMLBuilder builder;

		readonly string template = @"
=a=
<html>
<style TYPE=""text/css"">
	table { 
		border-collapse: collapse;
		border: 1px solid #000000; 
	}
	td { 
		border: 1px solid #000000; 
	}
	.found {
		font-family: Wingdings;
		font-size: 10pt;
	}
	.Notes {
		font-family: Arial;
		font-size: 9pt;
		text-indent: 3em;
	}
=b=
	.{ProjectN} {
		font-family: ""{FontFace}"";
		font-size: {FontSize}pt;
		direction: {Direction};
		text-align: {Alignment};
	}
=c=
</style>
<body>
<table border=1 width=100%>
<thead>
	<tr>
=d=
		<td width={ColWidthPercent}%><b>{ScrTextName}</b>:
			<span id=""renderings"" class=""{ProjectN}"">{Renderings}</span>
		</td>
=e=
	</tr>
</thead>

<tbody style=display:block class=body>
=f=
	<tr class=error id=""{ReferenceAsId}"">
		<td colspan={ColCount}>
			<a href=""userclick:scripref,{ReferenceAsId}"">{Reference}</a>
		</td>
	</tr>
	<tr>
=g=
		<td valign=""top"">
			<p class=""{ProjectClass}"">{Text}
<span id=""{ProjectClass}_{ReferenceAsId}"">{StatusBMP}</span></p>
		</td>
=h=
	</tr>
=i=
</tbody>
</table>
=j=
</body>
</html>
";

		public BiblicalTermsHTMLBuilder(ProjectSettings projSettings)
		{
			builder = new HTMLBuilder(template);
            scrTextNames = new List<string>();
            projectVariablesList = new List<Dictionary<string, string>>();

            // Figure out how many side-by-side projects we're going to show (max is: vern, natlBt, english, freeT)
            
            if (projSettings.Vernacular.HasData)
                scrTextNames.Add(projSettings.Vernacular.LangCode);
            if (projSettings.NationalBT.HasData)
                scrTextNames.Add(projSettings.NationalBT.LangCode);
            if (projSettings.InternationalBT.HasData)
                scrTextNames.Add(projSettings.InternationalBT.LangCode);
            if (projSettings.FreeTranslation.HasData)
                scrTextNames.Add(projSettings.FreeTranslation.LangCode);

            // Create list of template variables which are the same for each reference
            int nProjectNum = 0;
            if (projSettings.Vernacular.HasData)
                projectVariablesList.Add(setupProjectVars(projSettings.Vernacular, ++nProjectNum, scrTextNames.Count));
            if (projSettings.NationalBT.HasData)
                projectVariablesList.Add(setupProjectVars(projSettings.NationalBT, ++nProjectNum, scrTextNames.Count));
            if (projSettings.InternationalBT.HasData)
                projectVariablesList.Add(setupProjectVars(projSettings.InternationalBT, ++nProjectNum, scrTextNames.Count));
            if (projSettings.FreeTranslation.HasData)
                projectVariablesList.Add(setupProjectVars(projSettings.FreeTranslation, ++nProjectNum, scrTextNames.Count));

            System.Diagnostics.Debug.Assert(nProjectNum == scrTextNames.Count);
        }


		/// <summary>
		/// Setup the embedded template fields related to a specific project.
		/// </summary>
        /// <param name="li">language information about the 'projectNum' project</param>
		/// <param name="projectNum"></param>
		/// <param name="projectCount"></param>
		/// <returns></returns>
        private Dictionary<string, string> setupProjectVars(ProjectSettings.LanguageInfo li, int projectNum, int projectCount)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();

            dict["ScrTextName"] = li.LangName;
            dict["ProjectN"] = "project" + projectNum.ToString();
            dict["FontFace"] = li.FontToUse.Name;
            dict["FontSize"] = li.FontToUse.Size.ToString();

            if (li.DoRtl)
			{
				dict["Direction"] = "rtl";
				dict["Alignment"] = "right";
			}
			else
			{
				dict["Direction"] = "ltr";
				dict["Alignment"] = "left";
			}

			int colWidthPercent = 100 / projectCount;
			dict["ColWidthPercent"] = colWidthPercent.ToString();
			dict["ColCount"] = projectCount.ToString();

			return dict;
		}

        /// <summary>
		/// Mark the renderings found in the text of a verse.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="renderingFound"></param>
		/// <returns></returns>
		private string MarkRenderings(int projectNum, string text, out bool renderingFound)
		{
			renderingFound = false;

            if (String.IsNullOrEmpty(text))
                return text;

            TermRendering termRendering = termRenderingsList[projectNum];

            foreach (string strRendering in termRendering.RenderingsList)
            {
                Regex regexRendering = RegexForMatch(strRendering);
                string strBoldedText = regexRendering.Replace(text, SearchForRendering);
                if (strBoldedText != text)
                {
                    text = strBoldedText;
                    renderingFound = true;
                }
			}

            return text;
		}

        static string SearchForRendering(Match m)
        {
            // Get the matched string.
            string strRendering = m.ToString();

            return String.Format("<b>{0}</b>", strRendering);
        }

        private static char chSpace = ' ';
        private static char chUnderscore = '_';
        private static char chDoubleQuote = '"';
        private static char chNeverUsed = '\u000f';

        public static string EncodeAsHtmlId(string str)
        {
            return str.Replace(chSpace, chUnderscore).Replace(chDoubleQuote, chNeverUsed);
        }

        public static string DecodeAsHtmlId(string str)
        {
            return str.Replace(chUnderscore, chSpace).Replace(chNeverUsed, chDoubleQuote);
        }

	    /// <summary>
		/// Setup the template variables related to a single verse of a single project
		/// </summary>
        private void setupReferenceVars(StoryProjectData theSPD, int projectNum, string strVerseReference,
			bool bShowStatusBmp, out bool renderingFound, out bool renderingDenied)
		{
			referenceVariables = new Dictionary<string, string>();

            referenceVariables["ProjectClass"] = "project" + (projectNum + 1);
            referenceVariables["Reference"] = strVerseReference;

            // html ids can't have spaces or double-quotes, so replace them in turn
            referenceVariables["ReferenceAsId"] = EncodeAsHtmlId(strVerseReference);

            renderingDenied = false;
            string text = MarkRenderings(projectNum, mapReferenceToVerseTextList[strVerseReference][projectNum],
                out renderingFound);

            TermRendering termRendering = termRenderingsList[0];
            if (!renderingFound)
            {
                string strOneStoryUrl = ConstructUrlFromReference(theSPD, strVerseReference);
                renderingDenied = termRendering.Denials.Contains(strOneStoryUrl);
            }
            referenceVariables["Text"] = text;

            /*
            if (bShowStatusBmp && projectNum == 0)
                referenceVariables["StatusBMP"] = RenderingStatus(renderingFound, renderingDenied, strVerseReference);
            else
            */
            referenceVariables["StatusBMP"] = "";
        }

        internal static string ConstructUrlFromReference(StoryProjectData theSPD, 
            string reference)
        {
            string strStorySet, strStoryName, strAnchor;
            int nLineNum;
            ParseReference(reference, out strStorySet, out strStoryName, out nLineNum, out strAnchor);

            StoryData theStory = theSPD[strStorySet].GetStoryFromName(strStoryName);
            System.Diagnostics.Debug.Assert((theStory != null) && ((nLineNum - 1) < theStory.Verses.Count));
            return OneStoryUrlBuilder.Url(
                                   theSPD.ProjSettings.ProjectName,
                                   strStorySet,
                                   theStory.guid,
                                   theStory.Verses[nLineNum - 1].guid,
                                   StoryEditor.TextFields.Anchor, strAnchor, strAnchor);
        }

        internal static void ParseReference(string strReference, 
            out string strStorySet, out string strStoryName, out int nLineNumber, 
            out string strAnchor)
        {
            // format for reference is: "Set: '{0}' Story: '{1}' line: {2} anchor: {3}"
            const string CstrStorySetPortion = "Set: '";
            const string CstrStoryPortion = "' Story: '";
            const string CstrLinePortion = "' line: ";
            const string CstrAnchorPortion = " anchor: ";

            System.Diagnostics.Debug.Assert((strReference.IndexOf(CstrStorySetPortion) == 0) &&
                                            (strReference.IndexOf(CstrStoryPortion) != -1));
            int nIndexOfStoryNameLabel = strReference.IndexOf(CstrStoryPortion);
            int nLengthStorySetName = nIndexOfStoryNameLabel - CstrStorySetPortion.Length;
            strStorySet = strReference.Substring(CstrStorySetPortion.Length, nLengthStorySetName);
            int nIndexStoryName = nIndexOfStoryNameLabel + CstrStoryPortion.Length;
            int nIndexLineNumber = strReference.IndexOf(CstrLinePortion, nIndexStoryName) + CstrLinePortion.Length;
            strStoryName = strReference.Substring(nIndexStoryName, nIndexLineNumber - nIndexStoryName - CstrLinePortion.Length);

            int nIndexOfAnchorPortion = strReference.IndexOf(CstrAnchorPortion, nIndexLineNumber);
            string strLineNumber;
            if (nIndexOfAnchorPortion != -1)
            {
                int nIndexAnchor = nIndexOfAnchorPortion + CstrAnchorPortion.Length;
                strLineNumber = strReference.Substring(nIndexLineNumber, nIndexAnchor - nIndexLineNumber - CstrAnchorPortion.Length);
                strAnchor = strReference.Substring(nIndexAnchor);
            }
            else
            {
                strLineNumber = strReference.Substring(nIndexLineNumber);
                strAnchor = null;
            }

            nLineNumber = 1;
            try
            {
                nLineNumber = Convert.ToInt32(strLineNumber);
            }
            catch { }
        }

        /*
        private static string RenderingStatus(bool renderingFound, bool renderingDenied, string reference)
		{
			if (renderingFound) 
				return " \u2713";   // check mark

			string image;
			string operation;

			if (renderingDenied)
			{
				image = "StatHasErrDenied.gif";
				operation = "undeny";
			}
			else
			{
				image = "StatHasErrors.gif";
				operation = "deny";
			}

            image = BiblicalTermsList.DefaultBiblicalTermsFileFolder + @"\" + image;

			string result = 
				"<a href=\"userclick:" +
				operation +
				"," +
				reference +
				"\"><img src=\"" +
				image +
				"\"></a>";

			return result;
		}
        */

        private static Regex RegexForMatch(string strRendering)
        {
            if (strRendering.Length < 1)
                return null;

            strRendering = strRendering.Trim().Trim(VersesData.achQuotes);

            // build the regular expression for searching for the rendering. Possible values are:
            //  *xyz if the word ends with "xyz"
            //  xyz* if it begins with "xyz"
            //  *xyz* if it contains "xyz"
            // So... for each *, insert ".*"
            char chFirst = strRendering[0];
            char chLast = strRendering[strRendering.Length - 1];

            // replace an initial "*" with the proper RegEx for anything at the 
            //  beginning of a word
            string strSearch = String.Format(@"\b{0}{1}",
                (chFirst == '*') ? ".*?" : chFirst.ToString(),
                strRendering.Substring(1, strRendering.Length - 1));

            // replace a final "*" with the proper RegEx for anything at the end
            //  of a word
            strSearch = String.Format(@"{0}{1}\b",
                                      strSearch.Substring(0, strSearch.Length - 1),
                                      (chLast == '*') ? @".*?" : chLast.ToString());

            return new Regex(strSearch, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        public static List<Regex> GetRegexs(TermRendering termRendering)
        {
            // get an array of RegEx searchers for each search string(s) in the list
            //  (may be none)
            if (termRendering.RenderingsList.Count == 0)
                return null;

            List<Regex> arrRegexs = new List<Regex>();
            foreach (string strSearchPattern in termRendering.RenderingsList)
            {
                Regex regex = RegexForMatch(strSearchPattern);
                if (regex != null)
                    arrRegexs.Add(regex);
            }

            return arrRegexs;
        }

        private TermRendering BuildFakeReferences(int nColumnIndex, string strSearchPatterns)
        {
            TermRendering termRendering = new TermRendering {Renderings = strSearchPatterns.Trim()};
            termRenderingsList.Add(termRendering);
            projectVariablesList[nColumnIndex]["Renderings"] = termRendering.Renderings;
            return termRendering;
        }

        public void SearchVerseText(StoryProjectData theSpd, 
            ProgressBar progressBarLoadingKeyTerms, 
            bool bSearchHidden, 
            string strSearchPatternsVernacular, 
            DirectableEncConverter decVernacular,
            string strSearchPatternsNationalBt, 
            DirectableEncConverter decNationalBt,
            string strSearchPatternsInternationalBt,
            DirectableEncConverter decInternationalBt,
            string strSearchPatternsFreeTranslation,
            DirectableEncConverter decFreeTranslation)
        {
            // to *have* them, is to show them.
            bool bShowVernacular = theSpd.ProjSettings.Vernacular.HasData;
            bool bShowNationalBT = theSpd.ProjSettings.NationalBT.HasData;
            bool bShowInternationalBT = theSpd.ProjSettings.InternationalBT.HasData;
            bool bShowFreeTranslation = theSpd.ProjSettings.FreeTranslation.HasData;

            mapReferenceToVerseTextList = new Dictionary<string, List<string>>();
            termRenderingsList = new List<TermRendering>();
            int nColumnIndex = 0;
            List<Regex> arrRegexVernacular = null;
            List<Regex> arrRegexNationalBT = null;
            List<Regex> arrRegexInternationalBT = null;
            List<Regex> arrRegexFreeTranslation = null;

            if (bShowVernacular)
                arrRegexVernacular = GetRegexs(BuildFakeReferences(nColumnIndex++, strSearchPatternsVernacular));

            if (bShowNationalBT)
                arrRegexNationalBT = GetRegexs(BuildFakeReferences(nColumnIndex++, strSearchPatternsNationalBt));

            if (bShowInternationalBT)
                arrRegexInternationalBT = GetRegexs(BuildFakeReferences(nColumnIndex++, strSearchPatternsInternationalBt));

            if (bShowFreeTranslation)
                arrRegexFreeTranslation = GetRegexs(BuildFakeReferences(nColumnIndex, strSearchPatternsFreeTranslation));

            // get the current stories only (not the obsolete ones)
            // StoriesData theStories = theSpd[Properties.Resources.IDS_MainStoriesSet];
            progressBarLoadingKeyTerms.Maximum = theSpd.Values.Sum(aStorySet => aStorySet.Count);
            progressBarLoadingKeyTerms.Value = 0;
            progressBarLoadingKeyTerms.Visible = true;  // in case we're repeating

            foreach (var aStorySet in theSpd.Values)
            {
                foreach (StoryData aStory in aStorySet)
                {
                    for (int nVerseNumber = 0; nVerseNumber < aStory.Verses.Count; nVerseNumber++)
                    {
                        string strVerseReference = String.Format("Set: '{0}' Story: '{1}' line: {2}",
                                                                 aStorySet.SetName,
                                                                 aStory.Name,
                                                                 nVerseNumber + 1);

                        VerseData aVerse = aStory.Verses[nVerseNumber];
                        if (!aVerse.IsVisible && !bSearchHidden)
                            continue;

                        // don't need to continue checking the others if we find a hit earlier
                        if (SearchForHit(bShowVernacular, arrRegexVernacular, aVerse.StoryLine.Vernacular, decVernacular)
                            || SearchForHit(bShowNationalBT, arrRegexNationalBT, aVerse.StoryLine.NationalBt, decNationalBt)
                            || SearchForHit(bShowInternationalBT, arrRegexInternationalBT, aVerse.StoryLine.InternationalBt, decInternationalBt)
                            || SearchForHit(bShowFreeTranslation, arrRegexFreeTranslation, aVerse.StoryLine.FreeTranslation, decFreeTranslation))
                        {
                            var astrVerseText = new List<string>(scrTextNames.Count);
                            if (bShowVernacular)
                                astrVerseText.Add(aVerse.StoryLine.Vernacular.GetValue(decVernacular));
                            if (bShowNationalBT)
                                astrVerseText.Add(aVerse.StoryLine.NationalBt.GetValue(decNationalBt));
                            if (bShowInternationalBT)
                                astrVerseText.Add(aVerse.StoryLine.InternationalBt.GetValue(decInternationalBt));
                            if (bShowFreeTranslation)
                                astrVerseText.Add(aVerse.StoryLine.FreeTranslation.GetValue(decFreeTranslation));
                            if (mapReferenceToVerseTextList.ContainsKey(strVerseReference))
                            {
                                LocalizableMessageBox.Show(
                                    String.Format(
                                        Localizer.Str(
                                            "Tell bob_eaton@sall.com that your project (i.e. '{0}') has two stories by the name of '{1}' in the '{2}' story set (which shouldn't be able to happen). Ignoring the subsequent one found... you may want to rename the stories to be unique in the Panorama, Show dialog, {2} tab)"),
                                        theSpd.ProjSettings.ProjectName,
                                        aStory.Name,
                                        aStorySet.SetName),
                                    StoryEditor.OseCaption);
                            }
                            else
                                mapReferenceToVerseTextList.Add(strVerseReference, astrVerseText);
                        }
                    }
                    progressBarLoadingKeyTerms.Value++;
                }
            }
        }

        private static bool SearchForHit(bool bCheck, IEnumerable<Regex> arrRegex, 
            StringTransfer st, DirectableEncConverter dec)
        {
            if (!bCheck || (st == null))
                return false;

            string strForMatch = st.GetValue(dec);
            return SearchForHit(arrRegex, strForMatch);
        }

        public static bool SearchForHit(IEnumerable<Regex> arrRegex, string strForMatch)
        {
            var bFoundMatch = false;
            if (!String.IsNullOrEmpty(strForMatch) && (arrRegex != null))
                bFoundMatch = arrRegex.Any(regex =>
                                           regex.IsMatch(strForMatch));
            return bFoundMatch;
        }
        /*
        /// <summary>
        /// Build by project by reference array of verse text for this term
        /// </summary>
        internal void ReadVerseText(List<Term> myTerms, StoryProjectData theSPD, ProgressBar progressBarLoadingKeyTerms)
        {
            var vrefs = new ArrayList();
            foreach (var myTerm in myTerms)
                foreach (var vref in myTerm.VerseRefs())
                    vrefs.Add(vref.BBBCCCVVVS());

            ProcessReferences(vrefs, theSPD, progressBarLoadingKeyTerms);
        }

        private void ProcessReferences(ArrayList vrefs, StoryProjectData theSPD, 
            ProgressBar progressBarLoadingKeyTerms)
	    {
	        mapReferenceToVerseTextList = new Dictionary<string, List<string>>();

	        // get the current stories only (not the obsolete ones)
	        StoriesData theStories = theSPD[Properties.Resources.IDS_MainStoriesSet];
	        progressBarLoadingKeyTerms.Maximum = theStories.Count;
	        progressBarLoadingKeyTerms.Value = 0;

	        for (int nStoryNumber = 0; nStoryNumber < theStories.Count; nStoryNumber++)
	        {
	            StoryData aStory = theStories[nStoryNumber];
	            for (int nVerseNumber = 0; nVerseNumber < aStory.Verses.Count; nVerseNumber++)
	            {
	                VerseData aVerse = aStory.Verses[nVerseNumber];
	                for (int nAnchorNumber = 0; nAnchorNumber < aVerse.Anchors.Count; nAnchorNumber++)
	                {
	                    AnchorData anAnchor = aVerse.Anchors[nAnchorNumber];
	                    VerseRef theVerseRef = new VerseRef(anAnchor.AnchorAsVerseRef);
	                    int nIndex = vrefs.BinarySearch(theVerseRef.BBBCCCVVVS());
	                    if (nIndex < 0) continue;

	                    string strVerseReference = String.Format("Story: '{0}' line: {1} anchor: {2}",
	                                                             aStory.Name, nVerseNumber + 1, anAnchor.JumpTarget);
                            
	                    List<string> astrVerseText = new List<string>(projectVariablesList.Count);
	                    if (theSPD.ProjSettings.Vernacular.HasData)
	                        astrVerseText.Add(aVerse.StoryLine.Vernacular.ToString());
	                    if (theSPD.ProjSettings.NationalBT.HasData)
                            astrVerseText.Add(aVerse.StoryLine.NationalBt.ToString());
	                    if (theSPD.ProjSettings.InternationalBT.HasData)
                            astrVerseText.Add(aVerse.StoryLine.InternationalBt.ToString());
                        if (theSPD.ProjSettings.FreeTranslation.HasData)
                            astrVerseText.Add(aVerse.StoryLine.FreeTranslation.ToString());

	                    // keep track of this verse and it's reference
	                    if (!mapReferenceToVerseTextList.ContainsKey(strVerseReference))
	                        mapReferenceToVerseTextList.Add(strVerseReference, astrVerseText);

	                    // we don't need to do any more anchors with this same line of the same story
	                    //  so set the anchor # to the number of anchors so the next outer for loop
	                    //  will think it's finished
	                    nAnchorNumber = aVerse.Anchors.Count;
	                    break;
	                }
	            }
	            progressBarLoadingKeyTerms.Value++;
	        }
	    }
        */

	    public static string FormattedNotes(TermRendering termRendering)
		{
			string notes = termRendering.Notes.Trim();
			string val = "";
			if (notes == "")
				return "";

			if (notes.Contains("\n"))
			{
				string[] parts = notes.Split('\n');
				val = " --- <span class=\"Notes\">" + parts[0] + "</span>";

				for (int i = 1; i < parts.Length; ++i)
					val += "\n<p class=\"Notes\">" + parts[i] + "</p>";
			}
			else
			{
				val = " --- <span class=\"Notes\">" + notes + "</span>";
			}

			return val;
		}

        /*
		public void BuildRenderings(string strProjectFolder, string termId)
		{
			termRenderingsList = new List<TermRendering>();
			int projectNum = 0;

            for (int i = 0; i < scrTextNames.Count; i++)
            {
                string name = scrTextNames[i];
                // until we have a "TermRenderingsList" xml file for this project, we can't use this feature
                // the BiblicalTermsEn.xml in the BiblicalTerms folder is *not* one of these and won't work.
                //string strPath = strProjectFolder;
                //if (i > 0)
                //    strPath = BiblicalTermsList.DefaultBiblicalTermsFileFolder;
                //
                TermRenderingsList termRenderings = TermRenderingsList.GetTermRenderings(strProjectFolder, name);
                TermRendering termRendering = termRenderings.GetRendering(termId);
                termRenderingsList.Add(termRendering);

                string val = termRendering.Renderings;
                val += FormattedNotes(termRendering);

                projectVariablesList[projectNum]["Renderings"] = val;

                ++projectNum;
            }
		}
        */

        /// <summary>
		/// Build the HTML text for a list of references.
		/// </summary>
		/// <param name="references">verses to show in list</param>
		/// <param name="termId">Biblical Term</param>
		/// <param name="status">summary status over all references</param>
		/// <returns>html for references</returns>
		public string Build(
            StoryProjectData theSPD,
            ProgressBar progressBarLoadingKeyTerms,
            bool bShowStatusBmp,
			out BiblicalTermStatus status)
		{
			builder.Clear();

			// Output the per project stylesheet info
			for (int i = 0; i < projectVariablesList.Count; ++i)
			{
				builder.SetDictionary(projectVariablesList[i]);
				if (i == 0)
			        builder.Output("a");
			    builder.Output("b");
			}

			builder.Output("c");

			// builder.Output the table header info
			for (int i = 0; i < projectVariablesList.Count; ++i)
			{
				builder.SetDictionary(projectVariablesList[i]);
			    builder.Output("d");
			}

			builder.Output("e");

			status = BiblicalTermStatus.AllFound;

			//For each reference output a cell for each project
            progressBarLoadingKeyTerms.Maximum = mapReferenceToVerseTextList.Keys.Count;
            progressBarLoadingKeyTerms.Value = 0;

            foreach (string strVerseReference in mapReferenceToVerseTextList.Keys)
			{
				for (int projectNum = 0; projectNum < projectVariablesList.Count; ++projectNum)
				{
					builder.SetDictionary(projectVariablesList[projectNum]);

				    bool renderingFound;
				    bool renderingDenied;
                    setupReferenceVars(theSPD, projectNum, strVerseReference, bShowStatusBmp,
                        out renderingFound, out renderingDenied);
				    builder.SetDictionary(referenceVariables);

				    // Decide whether to show this reference depending on whether
				    // a rendering was found/denied for the first project and the view settings.
					if (projectNum == 0)
					{
				        // status is cumulative over all references
				        status = UpdateStatus(status, renderingFound, renderingDenied);

				        builder.Output("f");
					}

				    builder.Output("g");
				}

				builder.Output("h");

                progressBarLoadingKeyTerms.Value++;
			}

			builder.Output("i");

			builder.Output("j");
			return builder.ToString();
		}

		private BiblicalTermStatus UpdateStatus(BiblicalTermStatus status, bool renderingFound,
			bool renderingDenied)
		{
			if (renderingFound)
				return status;

			if (renderingDenied)
			{
				if (status == BiblicalTermStatus.AllFound)
					return BiblicalTermStatus.AllFoundOrDenied;

				return status;
			}

			return BiblicalTermStatus.SomeMissing;
		}
        /*
		internal BiblicalTermStatus RecheckItem(
			Term term, 
			List<VerseRef> vrefs, 
			TermRendering termRendering)
		{
			BiblicalTermStatus status = BiblicalTermStatus.AllFound;
			termRenderingsList = new List<TermRendering>();
			termRenderingsList.Add(termRendering);

			foreach (VerseRef vref2 in vrefs)
			{
				VerseRef vref = new VerseRef(vref2);
				vref.Versification = ScrVers.Original;
				if (scrText.Versification != ScrVers.Original)
					vref.ChangeVersification(scrText.Versification);

				// In PSA count the book title (verse 0) as part of book 1
				// for searching purposes.
				string text = "";

				if (vref.VerseNum == 1 && vref.Book == "PSA")
				{
					vref.VerseNum = 0;
					text = parser.GetVerseTextSafe(vref, true);
					vref.VerseNum = 1;
					text += parser.GetVerseTextSafe(vref, true);
				}
				else
					text = parser.GetVerseTextSafe(vref, true);

				bool renderingFound;
				MarkRenderings(0, text, out renderingFound);

				if (!renderingFound)
				{
					bool renderingDenied = termRendering.Denials.Contains(vref);
					if (renderingDenied)
						status = BiblicalTermStatus.AllFoundOrDenied;
					else
						return BiblicalTermStatus.SomeMissing;
				}
			}

			return status;
		}
        */
	}

    /*
	/// <summary>
	/// Build the HTML text for the Biblical Terms list
	/// </summary>
	public class BiblicalTermsListHTMLBuilder
	{
		HTMLBuilder builder;

		readonly string template = @"
=a=
<html>
<Title>{Title}</Title>
<style TYPE=""text/css"">
	.Lemma {
		font-family: ""{LemmaFont}"";
		font-size: 10pt;
	}
	.Gloss {
		font-family: Arial;
		font-size: 10pt;
	}
	.Status {
		font-family: Wingdings;
		font-size: 10pt;
	}
	.Renderings {
		font-family: ""{FontFace}"";
		font-size: {FontSize}pt;
		direction: {Direction}
	}
	.ItemList {
		empty-cells: show;
		border-collapse: collapse;
	}
	.Notes {
		font-family: Arial;
		font-size: 9pt;
		text-indent: 3em;
	}
	.Header {
		font-weight: bold;
	}
</style>
<body>
<table class=""ItemList"" border=1 width=100%>
	<tr class=""header"">
		<td>Lemma</td>
		<td> </td>
		<td>Category</td>
		<td>Gloss</td>
		<td>Renderings</td>
	</tr>
=b=
	<tr>
		<td class=""Lemma"" valign=""top"">{Lemma}</td>
		<td class=""Status"" valign=""top"" align=""{Direction}"">{Status}</td>
		<td class=""Gloss"" valign=""top"">{Category}</td>
		<td class=""Gloss"" valign=""top"" align=""{Direction}"">{Gloss}</td>
		<td class=""Renderings"" valign=""top"" align=""{Direction}"">{Renderings}</td>
	</tr>
=c=
</table>
</body>
</html>
";

		//! localize table headers

		public BiblicalTermsListHTMLBuilder()
		{
		}

        /// <summary>
		/// Build the HTML text for a list of references.
		/// </summary>
		public string Build(ListView listView, bool transliterateLemmas)
		{
			builder = new HTMLBuilder(template);

			if (transliterateLemmas)
				builder.SetVar("LemmaFont", "Arial");
			else
				builder.SetVar("LemmaFont", "Doulos SIL");

			builder.SetVar("ScrTextName", scrText.Name);
			builder.SetVar("FontFace", scrText.DefaultFont);
			builder.SetVar("FontSize", scrText.DefaultFontSize.ToString());
			if (scrText.RightToLeft)
				builder.SetVar("Direction", "rtl");
			else
				builder.SetVar("Direction", "ltr");

			builder.Output("a");

			foreach (ListViewItem lvi in listView.Items)
			{
				builder.SetVar("Lemma", lvi.Text);

				if (lvi.SubItems[1].Text.Trim() != "")
					builder.SetVar("Status", "\u2713");
				else
					builder.SetVar("Status", " ");

				builder.SetVar("Category", lvi.SubItems[2].Text);
				builder.SetVar("Gloss", lvi.SubItems[3].Text);
				builder.SetVar("Renderings", lvi.SubItems[4].Text);

				builder.Output("b");
			}

			builder.Output("c");

			return builder.ToString();
		}

		/// <summary>
		/// Build the HTML text for a list of references.
		/// </summary>
		public string Build(
            BiblicalKeyTermsForm termsForm, 
			List<Term> terms, 
			bool transliterateLemmas,
			TermRenderingsList renderings,
            string title)
		{
			builder = new HTMLBuilder(template);

            if (title == "")
                builder.SetVar("Title", "Biblical Terms Renderings For " + scrText.Name);
            else
                builder.SetVar("Title", title);

			if (transliterateLemmas)
				builder.SetVar("LemmaFont", "Arial");
			else
				builder.SetVar("LemmaFont", "Doulos SIL");

			builder.SetVar("ScrTextName", scrText.Name);
			builder.SetVar("FontFace", scrText.DefaultFont);
			builder.SetVar("FontSize", scrText.DefaultFontSize.ToString());
			if (scrText.RightToLeft)
				builder.SetVar("Direction", "rtl");
			else
				builder.SetVar("Direction", "ltr");

			builder.Output("a");

			foreach (Term term in terms)
			{
				builder.SetVar("Lemma", termsForm.GetCellValue(0, term).ToString());

				if (termsForm.GetCellValue(1, term).ToString().Trim() != "")
					builder.SetVar("Status", "\u2713");
				else
					builder.SetVar("Status", " ");

				builder.SetVar("Category", termsForm.GetCellValue(2, term).ToString());
				builder.SetVar("Gloss", termsForm.GetCellValue(3, term).ToString());

				TermRendering termRendering = renderings.GetRendering(term.Id);

				string val = termsForm.GetCellValue(4, term).ToString();
				string notes = termRendering.Notes.Trim();
				val += BiblicalTermsHTMLBuilder.FormattedNotes(termRendering);
				builder.SetVar("Renderings", val);

				builder.Output("b");
			}

			builder.Output("c");

			return builder.ToString();
		}
    }
    */
}
