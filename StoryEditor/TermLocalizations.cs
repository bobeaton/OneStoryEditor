using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using NetLoc;
using Paratext;

// This is localization information for Biblical terms.
// It is project independent but UI language dependentant.

namespace OneStoryProjectEditor
{
	/// <summary>
	/// Localization information for all Biblical terms for a single language.
	/// Created by: Reinier will create the initial file in English.
	///      Localizers will use Notepad to create additional versions of the file.
	///      (Actually it would be better to us an XML editor but is there a free one
	///       available?)
	///		</summary>
	[XmlRoot("BiblicalTermsLocalizations")]
	public class TermLocalizations
	{
		private string language;
		private List<Localization> terms = new List<Localization>();
		private List<Localization> categories = new List<Localization>();
		private List<Localization> domains = new List<Localization>();

		private Dictionary<string, Localization> termsDict = null;
		private Dictionary<string, string> categoriesDict = null;
		private Dictionary<string, string> domainsDict = null;

		private static TermLocalizations termLocalizations;
		private static string localizationLanguageId = "";

		public static TermLocalizations Localizations
		{
			get
			{
				if (Localizer.Default.LanguageId == localizationLanguageId)
					return termLocalizations;

                // TODO localizer languages are stored as "es" or "en", not "Es" or "En". Switch to all lowercase
				localizationLanguageId = Localizer.Default.LanguageId;
				termLocalizations = GetTermLocalizations(Localizer.Default.LanguageId);
				if (termLocalizations != null)
					return termLocalizations;

				termLocalizations = GetTermLocalizations("En");
				return termLocalizations;
			}
		}


		private static TermLocalizations GetTermLocalizations(string language)
		{
			TermLocalizations termLocalizations;

			//ScrText scr = new ScrText();
			string fileName = "";

			try
			{
                fileName = BiblicalTermsList.DefaultBiblicalTermsFileFolder + @"\BiblicalTerms" + language + ".xml";
				System.IO.TextReader reader = new StreamReader(fileName);

				using (reader)
				{
					XmlSerializer xser = Utilities.Memento.CreateXmlSerializer(typeof(TermLocalizations));
					termLocalizations = (TermLocalizations)xser.Deserialize(reader);
				}
			}
			catch (Exception exc)
			{
				// Fall back to English
				if (language != "En")
					return GetTermLocalizations("En");

				MessageBox.Show(exc.Message + ": " + fileName);
				termLocalizations = null;
			}

			return termLocalizations;
		}

		public string Language
		{
			get { return language; }
			set { language = value; }
		}

		public Localization GetTermLocalization(string id)
		{
			if (termsDict == null)
			{
				termsDict = new Dictionary<string, Localization>();
				foreach (Localization loc in terms)
				{
					termsDict[loc.Id] = loc;
				}
				terms = null;   // free memory
			}

			Localization locOut;
			if (!termsDict.TryGetValue(id, out locOut))
			{
				locOut = new Localization();
				locOut.Id = id;
				//locOut.Gloss = id + ": gloss missing";
			    locOut.Gloss = "";
				locOut.Notes = "";
			}

			return locOut;
		}

		public string GetCategoryLocalization(string id)
		{
			if (categoriesDict == null)
			{
				categoriesDict = new Dictionary<string, string>();
				foreach (Localization loc in categories)
				{
					categoriesDict[loc.Id] = loc.Gloss;
				}
			}

			string glossText;
			if (!categoriesDict.TryGetValue(id, out glossText))
			{
				glossText = id;
			}

			return glossText;
		}

		public string GetDomainLocalization(string id)
		{
			if (domainsDict == null)
			{
				domainsDict = new Dictionary<string, string>();
				foreach (Localization loc in domains)
				{
					domainsDict[loc.Id] = loc.Gloss;
				}
			}

			string glossText;
			if (!domainsDict.TryGetValue(id, out glossText))
			{
				glossText = id;
			}

			return glossText;
		}

		/// <summary>
		/// Localization info for each term.
		/// </summary>
		public List<Localization> Terms
		{
			get { return terms; }
			set { terms = value; }
		}

		/// <summary>
		/// Map CategoryId do localized category name.
		/// </summary>
		public List<Localization> Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		/// <summary>
		/// Map domain id to localized domain name.
		/// </summary>
		public List<Localization> Domains
		{
			get { return domains; }
			set { domains = value; }
		}

		public void Save(string languageId)
		{
            string fileName = BiblicalTermsList.DefaultBiblicalTermsFileFolder + @"\BiblicalTerms" + languageId + ".xml";

			XmlSerializer xser = Utilities.Memento.CreateXmlSerializer(typeof(TermLocalizations));
			System.IO.TextWriter writer = new StreamWriter(fileName);
			using (writer)
			{
				xser.Serialize(writer, this);
			}

			MessageBox.Show(fileName + ": Localization file written.");
		}
	}

	/// <summary>
	/// Localized information for Biblical term components
	/// </summary>
	// I'd like to change this to TermLocalization but everytime I try it messes
	// the XML deserialization even if I try to edit the .xml file to match this
	// change, sigh.
	public class Localization : IComparable<Localization>
	{
		/// <summary>
		/// Id to match BTTerm
		/// </summary>
		private string gloss;
		private string notes;

		[XmlAttribute("Id")]
		public string Id;

		/// <summary>
		/// Short (30 chars?) gloss identifying this component
		/// </summary>
		[XmlAttribute("Gloss")]
		public string Gloss
		{
			get { return gloss; }
			set { gloss = value; }
		}

		/// <summary>
		/// Consultant notes for this term.
		/// can contain application specific links to resource such as Fauna Handbook.
		/// </summary>
		[XmlText()]
		public string Notes
		{
			get {
				if (notes == null)
					return "";

				return notes; 
			}
			set {
				notes = value.Trim();
				if (notes == "-")  // ignore notes entered as just a -
					notes = "";
			}
		}

		/// <summary>
		/// Return in notes in short lines suitable for a popup
		/// </summary>
		[XmlIgnore()]
		public string NotesPopup
		{
			get {
				string text = Notes;

				// Make sure note lines are 60 chars or less
				if (text.Length > 60)
				{
					string[] parts = text.Split();
					int len = 0;
					text = "";

					foreach (string part in parts)
					{
						if (part.Length + len > 60)
						{
							text += "\n" + part;
							len = part.Length;
						}
						else
						{
							if (text != "")
								text += " ";
							text += part;
							len += part.Length + 1;
						}
					}
				}

				return text;
			}
		}

		public override string ToString()
		{
			return Gloss;
		}

        public int CompareTo(Localization other)
        {
            return Gloss.CompareTo(other.Gloss);
        }
    }
}
