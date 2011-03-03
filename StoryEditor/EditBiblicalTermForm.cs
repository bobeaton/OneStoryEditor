using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NetLoc;
using Paratext;

namespace OneStoryProjectEditor
{
    public partial class EditBiblicalTermForm : TopForm
    {
        // True if adding new term, false if editing existing term
        protected bool AddingTerm = true;

        Font greekFont = new Font("Galatia SIL", 10);
        Font hebrewFont = new Font("Ezra SIL", 10);

        private BiblicalTermsList biblicalTermsList;
        private Term biblicalTerm;

        public EditBiblicalTermForm(Term term, string strProjectFolder)
            : base(true)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            biblicalTermsList = BiblicalTermsList.GetMyBiblicalTermsList(strProjectFolder);

            if (term != null)
            {
                Lemma = term.Id;
                Gloss = term.LocalGloss ?? term.Gloss;
                Transliteration = term.Transliteration;
                CategoryIds = term.CategoryIds;
                References = term.VerseRefs().ToList();
                AddingTerm = false;
            }
            else
                AddingTerm = true;
        }

        private EditBiblicalTermForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public string Lemma
        {
            get { return txtLemma.Text; }
            set
            {
                txtLemma.Text = value;
                SetLemmaFont();
            }
        }

        public List<string> CategoryIds { get; set; }

        public string Transliteration { get; set; }

        private void SetLemmaFont()
        {
            Term term = new Term() { Id = txtLemma.Text };
            if (term.Language == "Hebrew")
                txtLemma.Font = hebrewFont;
            else if (term.Language == "Greek")
                txtLemma.Font = greekFont;
        }

        public string Gloss
        {
            get { return txtGloss.Text; }
            set { txtGloss.Text = value; }
        }

        public List<VerseRef> References
        {
            set
            {
                List<VerseRef> vrefs = value.Select(vref => vref.Clone()).ToList();
                try
                {
                    vrefs.ForEach(vref => vref.ChangeVersification(biblicalTermsList.Versification));
                }
                catch { }

                vrefs.Sort();
                string[] references = vrefs.Select(vref => vref.ToString()).ToArray();
                txtReferences.Text = string.Join("\r\n", references);
            }
        }

        protected override void OnShown(System.EventArgs e)
        {
            if (!AddingTerm)
            {
                biblicalTerm = biblicalTermsList.Get(Lemma);
                if (biblicalTerm == null)
                    throw new Exception("Program error: term not found");
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Lemma = Lemma.Trim();

            if (Lemma == "")
            {
                MessageBox.Show(Localizer.Str("Lemma field cannot be blank."));
                return;
            }

            bool ok;

            if (AddingTerm)
                ok = AddTerm();
            else
                ok =UpdateTerm();

            if (!ok)
                return;

            DialogResult = DialogResult.OK;
        }

        private bool UpdateTerm()
        {
            // If lemma has changed, don't allow the new lemma to already exist
            if (biblicalTerm.Id != Lemma)
            {
                if (WarnIfLemmaExists())
                    return false;
            }

            if (!SetReferences(biblicalTerm))
                return false;

            biblicalTerm.Id = Lemma;
            biblicalTerm.LocalGloss = Gloss;
            biblicalTerm.CategoryIds = CategoryIds;
            biblicalTerm.Transliteration = Transliteration;

            biblicalTermsList.Terms.Sort();
            biblicalTermsList.Save();

            return true;
        }

        private bool WarnIfLemmaExists()
        {
            foreach (Term term in biblicalTermsList.Terms)
                if (term.Id == Lemma)
                {
                    MessageBox.Show(Localizer.Str("Cannot add Lemma, it is already in the list."));
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Add term.
        /// </summary>
        /// <returns>true if successfully added</returns>
        private bool AddTerm()
        {
            if (WarnIfLemmaExists())
                return false;

            var newTerm = new Term() {Id = Lemma, LocalGloss = Gloss};

            if (!SetReferences(newTerm))
                return false;

            biblicalTermsList.Terms.Add(newTerm);
            biblicalTermsList.Terms.Sort();
            biblicalTermsList.Save();

            return true;
        }

        /// <summary>
        /// Set references for this term from txtReferences field
        /// </summary>
        /// <param name="term"></param>
        /// <returns>true if successful</returns>
        private bool SetReferences(Term term)
        {
            string[] lines = txtReferences.Text.Split('\n');
            var vrefs = new List<VerseRef>();

            foreach (string line in lines)
            {
                string line1 = line.Trim();
                if (line1 == "" || line1.StartsWith("#"))
                    continue;

                try
                {
                    vrefs.Add(new VerseRef(line1));
                }
                catch (Exception)
                {
                    MessageBox.Show(Localizer.Str("Invalid verse reference") + ": " + line1);
                    return false;
                }
            }

            term.References = new List<Verse>();
            vrefs.ForEach(vref => term.AddReference(vref));
            term.SortReferences();

            return true;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtLemma_TextChanged(object sender, EventArgs e)
        {
            SetLemmaFont();
        }
    }
}
