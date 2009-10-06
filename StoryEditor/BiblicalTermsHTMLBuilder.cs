using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Paratext;
using System.Text.RegularExpressions;

namespace OneStoryProjectEditor
{
	/// <summary>
	/// Build the HTML text for the references display of the Biblical Terms tool.
	/// </summary>
	public class BiblicalTermsHTMLBuilder
	{
		// Values in the template which change for each reference.
		Dictionary<string, string> referenceVariables;
		// Values in the template which are the same for each reference.
		Dictionary<string, string> projectVariables;

        Dictionary<string, string> mapReferenceToVerseText; // e.g. "Spiritual World Verse 1" to "this is the text of that verse"

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
		<td><b>{ScrTextName}</b>:
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
			<p class=""{ProjectN}"">{Text}
<span id=""{ProjectN}_{Reference}"">{StatusBMP}</span></p>
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

		public BiblicalTermsHTMLBuilder(ProjectSettings.LanguageInfo li)
		{
			builder = new HTMLBuilder(template);

            // Create list of template variables which are the same for each reference
            projectVariables = setupProjectVars(li);
        }


		/// <summary>
		/// Setup the embedded template fields related to a specific project.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, string> setupProjectVars(ProjectSettings.LanguageInfo li)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();

            dict["ProjectN"] = "project" + li.LangCode;
            dict["ScrTextName"] = li.LangName;
            dict["FontFace"] = li.LangFont.Name;
            dict["FontSize"] = li.LangFont.Size.ToString();

            if (li.IsRTL)
			{
				dict["Direction"] = "rtl";
				dict["Alignment"] = "right";
			}
			else
			{
				dict["Direction"] = "ltr";
				dict["Alignment"] = "left";
			}

			dict["ColWidthPercent"] = 100.ToString();
			dict["ColCount"] = 1.ToString();

			return dict;
		}

        protected static Regex SearchForRenderings = new Regex(@"\b([1-3a-zA-Z][a-zA-Z]{2})[ :\.](\d{1,3})[ \.](\d{1,3})", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        
        /// <summary>
		/// Mark the renderings found in the text of a verse.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="renderingFound"></param>
		/// <returns></returns>
		private string MarkRenderings(string text, out bool renderingFound)
		{
			renderingFound = false;

            TermRendering termRendering = termRenderingsList[0];

            foreach (string strRendering in termRendering.RenderingsList)
            {
                if (strRendering.Length < 1)
                    continue;
                // build the regular expression for searching for the rendering. Possible values are:
                //  *xyz if the word ends with "xyz"
                //  xyz* if it begins with "xyz"
                //  *xyz* if it contains "xyz"
                // So... for each *, insert ".*"
                char chFirst = strRendering[0];
                char chLast = strRendering[strRendering.Length - 1];
                string strSearch = String.Format("{0}{1}{2}",
                    (chFirst == '*') ? @"\b.*?" : chFirst.ToString(),
                    strRendering.Substring(1, strRendering.Length - 2),
                    (chLast == '*') ? @".*?\b" : chLast.ToString());

                Regex regexRendering = new Regex(strSearch, RegexOptions.CultureInvariant | RegexOptions.Singleline);
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

		/// <summary>
		/// Setup the template variables related to a single verse of a single project
		/// </summary>
		private void setupReferenceVars(string strVerseReference,
			out bool renderingFound, out bool renderingDenied)
		{
			referenceVariables = new Dictionary<string, string>();
            referenceVariables["Reference"] = strVerseReference;
            referenceVariables["ReferenceAsId"] = strVerseReference.Replace(' ', '_');

            string text = MarkRenderings(mapReferenceToVerseText[strVerseReference], out renderingFound);
            referenceVariables["Text"] = text;

            renderingDenied = false;
            TermRendering termRendering = termRenderingsList[0];
            if (!renderingFound)
                renderingDenied = termRendering.Denials.Contains(strVerseReference);

            referenceVariables["StatusBMP"] = RenderingStatus(renderingFound, renderingDenied, strVerseReference);
		}

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

		/// <summary>
		/// Build by project by reference array of verse text for this term
		/// </summary>
        internal void ReadVerseText(Term myTerm, List<VerseRef> references, StoryProjectData theSPD, ProgressBar progressBarLoadingKeyTerms)
        {
            mapReferenceToVerseText = new Dictionary<string, string>();

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
                        foreach (VerseRef aVerseRef in references)
                            if (theVerseRef.BBBCCCVVVS() == aVerseRef.BBBCCCVVVS())
                            {
                                string strVerseReference = String.Format("Story: '{0}', line: {1}, anchor: {2}",
                                    aStory.Name, nVerseNumber + 1, anAnchor.JumpTarget);
                                string strVerseText;
                                if (theSPD.ProjSettings.Vernacular.HasData)
                                    strVerseText = aVerse.VernacularText.ToString();
                                else if (theSPD.ProjSettings.NationalBT.HasData)
                                    strVerseText = aVerse.NationalBTText.ToString();
                                else
                                    strVerseText = aVerse.InternationalBTText.ToString();

                                // keep track of this verse and it's reference
                                mapReferenceToVerseText[strVerseReference] = strVerseText;
                            }
                        // ??? verseText[i] = HTMLBuilder.EscapeSpecialCharacters(verseText[i]);
                    }
                }
                progressBarLoadingKeyTerms.Value++;
            }
		}

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

		private void BuildRenderings(string strProjectFolder, string strLangCode, string termId)
		{
			termRenderingsList = new List<TermRendering>();

            TermRenderingsList termRenderings = TermRenderingsList.GetTermRenderings(strProjectFolder, strLangCode);
			TermRendering termRendering = termRenderings.GetRendering(termId);
			termRenderingsList.Add(termRendering);

			string val = termRendering.Renderings;
			val += FormattedNotes(termRendering);

            projectVariables["Renderings"] = val;
		}

		/// <summary>
		/// Build the HTML text for a list of references.
		/// </summary>
		/// <param name="references">verses to show in list</param>
		/// <param name="termId">Biblical Term</param>
		/// <param name="status">summary status over all references</param>
		/// <returns>html for references</returns>
		public string Build(
			List<VerseRef> references, 
			string termId,
            string strProjectFolder, 
            string strLangCode,
            ProgressBar progressBarLoadingKeyTerms,
			out BiblicalTermStatus status)
		{
			//BuildVerseText(references);
            BuildRenderings(strProjectFolder, strLangCode, termId);

			builder.Clear();

			// Output the per project stylesheet info
			builder.SetDictionary(projectVariables);
			builder.Output("a");
			builder.Output("b");
			builder.Output("c");

			// builder.Output the table header info
			builder.SetDictionary(projectVariables);
			builder.Output("d");

			builder.Output("e");

			status = BiblicalTermStatus.AllFound;

			//For each reference output a cell for each project
            progressBarLoadingKeyTerms.Maximum = mapReferenceToVerseText.Keys.Count;
            progressBarLoadingKeyTerms.Value = 0;

            foreach (string strVerseReference in mapReferenceToVerseText.Keys)
			{
                builder.SetDictionary(projectVariables);

				bool renderingFound;
				bool renderingDenied;
                setupReferenceVars(strVerseReference, out renderingFound, out renderingDenied);
				builder.SetDictionary(referenceVariables);

				// Decide whether to show this reference depending on whether
				// a rendering was found/denied for the first project and the view settings.
				// status is cumulative over all references
				status = UpdateStatus(status, renderingFound, renderingDenied);

				builder.Output("f");

				builder.Output("g");

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
