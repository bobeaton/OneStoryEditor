using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SelectKeyTerms : Form
    {
        private Dictionary<string, Term> _mapTerms = new Dictionary<string, Term>();

        public SelectKeyTerms(BiblicalTermsList btl, List<Term> listSelected)
        {
            InitializeComponent();

            foreach (Term term in btl.Terms)
            {
                string strItem = String.Format("{0}: ({1})", term.Gloss, term.Transliteration);
                try
                {
                    _mapTerms.Add(strItem, term);
                    int nIndex = checkedListBoxKeyTerms.Items.Add(strItem);
                    checkedListBoxKeyTerms.SetItemChecked(nIndex, listSelected.Contains(term));
                }
                catch { }   // ignore duplicates
            }
        }

        private List<Term> _selectedTerms = new List<Term>();
        public List<Term> SelectedTerms
        {
            get { return _selectedTerms; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (string strItem in checkedListBoxKeyTerms.CheckedItems)
            {
                Term term;
                if (_mapTerms.TryGetValue(strItem, out term))
                    _selectedTerms.Add(term);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
