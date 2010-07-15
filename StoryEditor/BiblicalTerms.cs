using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using NetLoc;
using Paratext;
using Paratext.BiblicalTerms;

// This information regards Biblical terms.
// This information is project and UI language independant.


namespace OneStoryProjectEditor
{
	/// <summary>
	/// Terms found in the source language texts.
	/// Reinier created this. 
	/// This information comes from BiblicalTerms.xml
	/// </summary>
	public class BiblicalTermsList
	{
		private List<Term> terms;  // list in order found in file
		private Dictionary<string,int> idToIndexDictionary;
	    private string termsFileName;
	    private string caption = "";

        public static event EventHandler BiblicalTermsListChanged = null;

        private static void OnBiblicalTermsListChanged()
        {
            EventHandler handler = BiblicalTermsListChanged;
            if (handler != null)
                handler(null, null);
        }

        private BiblicalTermsList()
        {
            terms = new List<Term>();
        }

		[XmlElement("Term")]
		public List<Term> Terms
		{
			get { return terms; }
			set { terms = value; }
		}

	    private ScrVers versification;

	    public ScrVers Versification
	    {
	        get
	        {
                if (versification == ScrVers.Unknown)
                    versification = ScrVers.English;
	            return versification;
	        }
            set { versification = value; }
	    }

	    public static string GetCaptionFromFileName(string fileName)
	    {
            if (fileName.EndsWith("\\BiblicalTerms.xml"))
                return Localizer.Str("Major Biblical Terms");

            if (fileName.EndsWith("\\AllBiblicalTerms.xml"))
                return Localizer.Str("All Biblical Terms");

            if (fileName.EndsWith("\\MyBiblicalTerms.xml"))
                return Localizer.Str("My Biblical Terms");

            return Path.GetFileNameWithoutExtension(fileName);
	    }

	    private int containsCategories = -1;

        /// <summary>
        /// True if at least some fields have the Categories defined
        /// </summary>
        /// <returns></returns>
        public bool ContainsCategories()
        {
            if (containsCategories == -1)
            {
                containsCategories = 0;
                foreach (var term in terms)
                    if (term.CategoryIds.Count() > 0)
                    {
                        containsCategories = 1;
                        break;
                    }
            }

            return containsCategories > 0;
        }

        private int containsGlosses = -1;

        /// <summary>
        /// True if at least some fields have the Categories defined
        /// </summary>
        /// <returns></returns>
        public bool ContainsGlosses()
        {
            // The standard file BiblicalTerms.xml and the test version of this file have glosses present
            // in the BiblicalTermsZZ.xml files (where ZZ is the language code, "xx" for testing
            if (termsFileName.EndsWith("\\BiblicalTerms.xml") || termsFileName.EndsWith("\\TestBiblicalTerms.xml"))
                containsGlosses = 1;

            else if (containsGlosses == -1)
            {
                containsGlosses = 0;
                foreach (var term in terms)
                    if (!string.IsNullOrEmpty(term.LocalGloss))
                    {
                        containsGlosses = 1;
                        break;
                    }
            }

            return containsGlosses > 0;
        }

        /// <summary>
        /// Name list is known by when selected
        /// </summary>
        public string Caption
        {
            get
            {
                if (string.IsNullOrEmpty(caption))
                    caption = GetCaptionFromFileName(termsFileName);

                return caption;
            }
            set { caption = value; }
        }

        /// <summary>
        /// Only MyBiblcalTerms.xml is editable
        /// </summary>
	    public bool IsMyBiblicalTerms
	    {
            get { return termsFileName != null && termsFileName.EndsWith(("\\MyBiblicalTerms.xml")); }
	    }

        public List<Term> TermsInRange(VerseRef firstReference, VerseRef lastReference)
		{
			VerseRef first = firstReference.Clone();
		    VerseRef last = lastReference.Clone();

            first.ChangeVersification(ScrVers.Original);
            last.ChangeVersification(ScrVers.Original);

		    IEnumerable<Term> terms2;
            /*
            if (first.Book == last.Book && first.Chapter == last.Chapter)
                terms2 = TermsInChapter(first);
            else
            */
                terms2 = terms;

		    int firstBCV = int.Parse(first.BBBCCCVVV());
		    int lastBCV = int.Parse(last.BBBCCCVVV());

		    return terms2.Where(term => term.HasReferencesInRange(firstBCV, lastBCV)).ToList();
		}

        /* rde
		private MRUCache<string, List<Term>> chapterCache;

        /// <summary>
        /// All the terms that have at least one reference in the specified chapter, 
        /// cached for performance.
        /// </summary>
        /// <param name="vref"></param>
        /// <returns></returns>
		private IEnumerable<Term> TermsInChapter(VerseRef vref)
		{
            if (chapterCache == null)
                chapterCache = new MRUCache<string, List<Term>>(10, GetTermsInChapter);

		    return chapterCache.Get(vref.ToString());
		}
        */

        /// <summary>
        /// Return all the terms in the specified chapter
        /// </summary>
        /// <param name="chapterRef">e.g. "GEN 3:1"</param>
        /// <returns></returns>
        private List<Term> GetTermsInChapter(string chapterRef)
        {
            VerseRef vref = new VerseRef(chapterRef);
            vref.Verse = "1";
            int firstBCV = int.Parse(vref.BBBCCCVVV());
            vref.Verse = vref.LastVerse.ToString();
            int lastBCV = int.Parse(vref.BBBCCCVVV());

            return terms.Where(term => term.HasReferencesInRange(firstBCV, lastBCV)).ToList();
        }

		/// <summary>
		/// Get the term with the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Term Get(string id)
		{
			int termIndex;
			if (idToIndexDictionary.TryGetValue(id, out termIndex))
				return terms[termIndex];

			throw new Exception("Term not found: " + id);
		}

        /// <summary>
        /// Return term with this id if present, otherwise null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Term GetIfPresent(string id)
        {
            int termIndex;
            if (idToIndexDictionary.TryGetValue(id, out termIndex))
                return terms[termIndex];

            return null;
        }

        /// <summary>
        /// Add a term to the list. Enter it in lookup dictionary
        /// </summary>
        /// <param name="term"></param>
        public void Add(Term term)
        {
            if (GetIfPresent(term.Id) != null)
                throw new ArgumentException("Biblical term already in list" + term.Id);

            Terms.Add(term);
            idToIndexDictionary[term.Id] = Terms.Count - 1;
        }

        private static BiblicalTermsList biblicalTerms = null;  // Singleton instance of this class.
        private static string biblicalTermsPath = Properties.Settings.Default.BiblicalTermsPath;

        public static void SelectTermsList()
        {
            var dialog = new SelectTermsListForm(biblicalTermsPath);
            DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                dialog.Dispose();
                return;
            }

            ForceReloadOfTerms();
            biblicalTermsPath = dialog.FileName;
            Properties.Settings.Default.BiblicalTermsPath = biblicalTermsPath;
            Properties.Settings.Default.Save();
            OnBiblicalTermsListChanged();

            dialog.Dispose();
        }
        /* rde

        public static void SelectAllBiblicalTermsList()
        {
            ForceReloadOfTerms();
            biblicalTermsPath = Path.Combine(
                Path.Combine(Program.DataPath, "BiblicalTerms"),
                "AllBiblicalTerms.xml");

            Properties.Settings.Default.BiblicalTermsPath = biblicalTermsPath;

            OnBiblicalTermsListChanged();
        }
        */

        private static void ForceReloadOfTerms()
        {
            biblicalTerms = null;
            myBiblicalTermsList = null;
        }

        /// <summary>
		/// Return a list of all Biblical Terms.
		/// If this has already been read, use previously read list.
		/// List is found in "BiblicalTerms.xml" in Paratext executable directory.
		/// </summary>
		/// <returns></returns>
		public static BiblicalTermsList GetBiblicalTerms()
		{
			if (biblicalTerms != null)
				return biblicalTerms;

            if (biblicalTermsPath == "")
            {
                biblicalTermsPath = DefaultBiblicalTermsFileName;
            }

            if (biblicalTermsPath.EndsWith("\\MyBiblicalTerms.xml"))
                return GetMyBiblicalTermsList();

            biblicalTerms = GetBiblicalTermsLocal(biblicalTermsPath);
            if (biblicalTerms == null && biblicalTermsPath != DefaultBiblicalTermsFileName)
            {
                biblicalTermsPath = DefaultBiblicalTermsFileName;
                biblicalTerms = GetBiblicalTermsLocal(biblicalTermsPath);
            }

			CreateIndexDictionary(biblicalTerms);
            RemoveDuplicateReferences();

			return biblicalTerms;
		}

        // Assign an index to each term and create a map from id to index
        private static void CreateIndexDictionary(BiblicalTermsList btl)
	    {
	        int index = 0;
	        btl.idToIndexDictionary = new Dictionary<string, int>();

	        foreach (Term term in btl.Terms)
	        {
	            btl.idToIndexDictionary[term.Id] = index;
	            term.Index = index;
	            ++index;
	        }
	    }

        private static BiblicalTermsList myBiblicalTermsList = null;

	    /// <summary>
        /// Get terms list for My Paratext Project/MyBiblicalTerms.xml.
        /// Create an empty list if this does not exist yet.
        /// </summary>
        /// <returns></returns>
        public static BiblicalTermsList GetMyBiblicalTermsList()
        {
            if (myBiblicalTermsList != null)
                return myBiblicalTermsList;

            string fileName = Path.Combine(Path.Combine(StoryProjectData.GetRunningFolder, "BiblicalTerms"), "MyBiblicalTerms.xml");
            myBiblicalTermsList = GetBiblicalTermsLocal(fileName);

            if (myBiblicalTermsList != null)
            {
                CreateIndexDictionary(myBiblicalTermsList);
                return myBiblicalTermsList;
            }

            myBiblicalTermsList = new BiblicalTermsList();
            myBiblicalTermsList.termsFileName = fileName;
            CreateIndexDictionary(myBiblicalTermsList);

            return myBiblicalTermsList;
        }

        public static string DefaultBiblicalTermsFileFolder
        {
            get
            {
#if DEBUG
                return @"C:\src\StoryEditor\StoryEditor\BiblicalTerms";
#else
                return StoryProjectData.GetRunningFolder + @"\BiblicalTerms";
#endif
            }
        }

        public static string DefaultBiblicalTermsFileName
        {
            get
            {
                return DefaultBiblicalTermsFileFolder + @"\BiblicalTerms.xml";
            }
        }

	    private static BiblicalTermsList GetBiblicalTermsLocal(string path)
        {
            BiblicalTermsList btl = null;

            if (!File.Exists(path))
                return null;

            using (TextReader reader = new StreamReader(path))
            {
                try
                {
                    XmlSerializer xser = Utilities.Memento.CreateXmlSerializer(typeof (BiblicalTermsList));
                    btl = (BiblicalTermsList) xser.Deserialize(reader);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(path + ": " + exc.Message);
                    return null;
                }
            }

            btl.Terms.ForEach(term => term.Versification = btl.Versification );
            btl.termsFileName = path;
            return btl;
        }

		private static void RemoveDuplicateReferences()
		{
			foreach (Term term in biblicalTerms.Terms)
			{
				int i = 0;
				while (i < term.References.Count-1)
				{
					if (term.References[i].VerseText == term.References[i+1].VerseText)
						term.References.RemoveAt(i+1);
					else
						++i;
				}
			}
		}

        public void Save()
        {
            XmlSerializer xser = Utilities.Memento.CreateXmlSerializer(typeof(BiblicalTermsList));
            TextWriter writer = new StreamWriter(termsFileName);
            using (writer)
            {
                xser.Serialize(writer, this);
            }

            ForceReloadOfTerms();

            OnBiblicalTermsListChanged();
        }
	}

	/// <summary>
	/// A Biblical term. Contains no localizable data (see TermLocalization)
	/// </summary>
	public class Term : IComparable<Term>
	{
		private string id;
		//private string lemma;
		private string transliteration;
		private string semanticDomain;
		private string wordForms;
		private string p6Id;  // not currently used
		private string language;
		private List<string> categoryIds;
		private List<Verse> references;
		private string origID;
		private int index;

		public static readonly string[] AllCategoryIds = 
			{   "KT" /*Keyterm*/, 
				"PN" /*Proper Name*/,
				"FL" /*Flora*/, 
				"FA" /*Fauna*/, 
				"RE" /*Realia*/,
				"UD" /*User Defined*/ };

		/// <summary>
		/// Unique identifier for this term.
		/// </summary>
		[XmlAttribute("Id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// Id in original file from Reinier, this is mostly only for
		/// debugging purposes.
		/// </summary>
		public string OrigID
		{
			get { return origID; }
			set { origID = value; }
		}

		/// <summary>
		/// Lemma for term, this is also used for the Id for the term
		/// </summary>
		public string Lemma
		{
			get { return id; }
		}

		///// <summary>
		///// Transliteration of original Hebrew or Greek source language term
		///// </summary>
		public string Transliteration
		{
            // Return transliterated lemma if available, otherwise just lemma
		    get { return transliteration ?? Lemma; }
		    set { transliteration = value; }
		}

		[XmlElement("Domain")]
		public string SemanticDomain
		{
			get { return semanticDomain; }
			set { semanticDomain = value; }
		}

		[XmlElement("Language")]
		public string Language
		{
			get
			{
                const int hebrewLow = 0x590;
			    const int hebrewHigh = 0x5ff;
			    const int greekLow = 0x370;
			    const int greekHigh = 0x3ff;

                if (!string.IsNullOrEmpty(language))
                    return language;

                foreach (char cc in Lemma)
                    if (cc >= hebrewLow && cc <= hebrewHigh)
                        return "Hebrew";

                foreach (char cc in Lemma)
                    if (cc >= greekLow && cc <= greekHigh)
                        language = "Greek";

			    return "";
			}
			set { language = value; }
		}

		/// <summary>
		/// Return each part of the semicolon separated domains
		/// </summary>
		public IEnumerable<string> SemanticDomains
		{
			get
			{
				if (semanticDomain != null && semanticDomain.Length != 0)
				{
					string[] parts = SemanticDomain.Split(';');
					foreach (string part in parts)
						yield return part.Trim();
				}
			}
		}

		[XmlElement("Form")]
		public string WordForms
		{
			get { return wordForms; }
			set { wordForms = value; }
		}

		/// <summary>
		/// Related P6 kb2 file identifier, if any. Used for data migration. Not currently used.  On a
		/// </summary>
		public string P6Id
		{
			get { return p6Id; }
			set { p6Id = value; }
		}

		/// <summary>
		/// Comma separated list of categories from CategoryIds. Usually a term only has one category
		/// but occasionally it may have multiple categories.
		/// </summary>
		[XmlElement("Category")]
		public List<string> CategoryIds
		{
			get { return categoryIds; }
			set { categoryIds = value; }
		}

	    private string localGloss;

	    public string LocalGloss
	    {
            get { return localGloss; }
            set { localGloss = value; }
        }

		/// <summary>
		/// Get the gloss for the term in the current UI language.
		/// </summary>
		[XmlIgnore()]
		public string Gloss
		{
			get
			{
                if (!string.IsNullOrEmpty(LocalGloss))
                    return LocalGloss;

				return TermLocalizations.Localizations.GetTermLocalization(id).Gloss;
			}
		}
		
		/// <summary>
		/// Index of this term in original XML file.
		/// Used to resort terms into original order.
		/// </summary>
		[XmlIgnore()]
		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		/// <summary>
		/// Get the notes for the term in the current UI language.
		/// </summary>
		[XmlIgnore()]
		public string Notes
		{
			get
			{
				string notes = TermLocalizations.Localizations.GetTermLocalization(id).Notes;
				return notes.Trim();
			}
		}

        private ScrVers versification;

        [XmlIgnore]
        public ScrVers Versification
        {
            get
            {
                return versification;
            }
            set
            {
                References.ForEach(verse => verse.Versification = value);
                versification = value;
            }
        }

		/// <summary>
		/// All references to this term.
		/// </summary>
		public List<Verse> References
		{
			get { return references; }
			set { references = value; }
		}

		/// <summary>
		/// Sort terms based on their Id.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(Term other)
		{
			return Id.CompareTo(other.Id);
		}

		/// <summary>
		/// Enumerated the VerseRef's for this term.
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<VerseRef> VerseRefs()
		{
            if (References != null)
			    foreach (Verse verse in References)
			    {
				    yield return verse.VerseRef;
			    }
		}

	    private int[] bcvReferences;

		/// <summary>
		/// True if this term has at least one reference in the specified range
		/// </summary>
		/// <returns></returns>
		public bool HasReferencesInRange(int firstBCV, int lastBCV)
		{
            if (bcvReferences == null)
                bcvReferences = GetBcvReferences();

		    int i = 0;
            while (i + 20 < bcvReferences.Length && bcvReferences[i + 20] < firstBCV)
                i += 20;

            while (i < bcvReferences.Length && bcvReferences[i] < firstBCV)
                i += 1;

            if (i < bcvReferences.Length && bcvReferences[i] <= lastBCV)
                return true;

		    return false;
		}

        /// <summary>
        /// Get references for this term as an array on BBBCCCVVV integers
        /// </summary>
        /// <returns></returns>
        private int[] GetBcvReferences()
        {
            int[] bcvs = new int[references.Count];
            for (int i = 0; i < references.Count; ++i)
                bcvs[i] = int.Parse(references[i].VerseRef.BBBCCCVVV());
            return bcvs;
        }

        public void AddReference(VerseRef vref)
        {
            if (references == null)
                references = new List<Verse>();

            var verse = new Verse();
            verse.VerseText = vref.ToString();
            verse.Versification = Versification;
            references.Add(verse);
        }

        public void SortReferences()
        {
            references.Sort();
        }
    }

	/// <summary>
	/// A reference to a biblical term in a specific verse.
	/// </summary>
	public class Verse: IComparable<Verse>
	{
		private string verseText;

		/// <summary>
		/// Reference for biblical term. Formats allowed:
		/// 1) 400101022 = MAT 1:10 char 22.
		/// 2) GEN 3:11    (Gen 3:11 "orignal" versification)
		/// 3) GEN 3:11/4  (Gen 3:11 "English" versification)
		/// This is not used directly at his used in directly by the XML serialization code.
		/// </summary>
		[XmlText()]
		public string VerseText
		{
		    get { return verseText; }
		    set { verseText = value; }
		}

        [XmlIgnore]
	    public ScrVers Versification { get; set; }

	    [XmlIgnore]
		public VerseRef VerseRef
		{
			get
			{
			    VerseRef verseRef;

				string book = verseText.Substring(0, 3);

                // Allow items of form GEN 3:11
                if (Canon.BookIdToNumber(book) != 0)
                {
                    verseRef = new VerseRef(verseText);
                    verseRef.Versification = Versification;
                    return verseRef;
                }

				string chapter = verseText.Substring(3, 3);
				string verse = verseText.Substring(6, 3);

				int bookNum;
				if (!int.TryParse(book, out bookNum))
				{
					throw new Exception("Invalid book number in <Verse> entry: " + verseText);
				}

				book = Canon.BookNumberToId(bookNum);

				// Strip leading zeros.
				while (chapter.Length > 1 && chapter[0] == '0')
					chapter = chapter.Substring(1);

				while (verse.Length > 1 && verse[0] == '0')
					verse = verse.Substring(1);

				verseRef = new VerseRef(book + " " + chapter + ":" + verse);

				// Set versification to original
				verseRef.Versification = ScrVers.Original;

				return verseRef;
			}
		}

		/// <summary>
		/// Character offset for this reference.
		/// </summary>
		[XmlIgnore]
		public int Offset
		{
			get
			{
				string off = verseText.Substring(9, 2);
				int offset;
				if (!int.TryParse(off, out offset))
				{
					throw new Exception("Invalid offset in <Verse> entry: " + verseText);
				}

				return offset;
			}
		}

        public int CompareTo(Verse other)
        {
            return VerseRef.CompareTo(other.VerseRef);
        }
    }
}
