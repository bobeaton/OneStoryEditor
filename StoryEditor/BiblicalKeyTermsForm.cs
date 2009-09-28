using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NetLoc;
using Paratext;
using onlyconnect;

namespace OneStoryProjectEditor
{
    public partial class BiblicalKeyTermsForm : Form
    {
        protected const int CnColumnKeyTermLemma = 0;
        protected const int CnColumnStatus = 1;
        protected const int CnColumnEnglishGloss = 2;
        protected const int CnColumnRenderings = 3;

        protected List<Term> visibleTerms = new List<Term>();
        TermRenderingsList renderings;   // Rendering info for terms in target language
        TermLocalizations termLocalizations;   // Term info unique to UI language
        StoriesData _theStories;
        BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html
        string refererencesHtml;  // text loaded into html references browser
        private BiblicalTermsList _biblicalTerms;   // All Biblical terms
        string idToScrollTo = null;		// Set this to scroll to an elementid when loaded
        internal StoryEditor _theSE;

        public BiblicalKeyTermsForm(StoryEditor theSE)
        {
            _theSE = theSE;
            InitializeComponent();
            _biblicalTerms = BiblicalTermsList.GetBiblicalTerms();
            htmlBuilder = new BiblicalTermsHTMLBuilder(theSE.Stories.ProjSettings.Vernacular);
        }

        public void Show(AnchorsData theAnchors, StoriesData theStories)
        {
            Show();
            Cursor curCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                _theStories = theStories;

                List<string> lstRefs = new List<string>();
                foreach (AnchorData anAnchor in theAnchors)
                {
                    VerseRef verseRef = new VerseRef(anAnchor.AnchorAsVerseRef);
                    lstRefs.Add(verseRef.BBBCCCVVV());
                }

                visibleTerms.Clear();
                progressBarLoadingKeyTerms.Maximum = _biblicalTerms.Terms.Count;
                progressBarLoadingKeyTerms.Value = 0;
                foreach (Term term in _biblicalTerms.Terms)
                {
                    foreach (Verse aVerseReference in term.References)
                        if (lstRefs.Contains(aVerseReference.VerseRef.BBBCCCVVV()))
                        {
                            visibleTerms.Add(term);
                            break;
                        }
                    progressBarLoadingKeyTerms.Value++;
                }

                // indicate that we've checked the key terms here.
                theAnchors.IsKeyTermChecked = true;

                if (visibleTerms.Count == 0)
                {
                    MessageBox.Show(Localizer.Str("There are no Biblical Terms in this verse(s)."));
                    return;
                }

                renderings = TermRenderingsList.GetTermRenderings(theStories.ProjSettings.ProjectFolder, theStories.ProjSettings.Vernacular.LangCode);
                termLocalizations = TermLocalizations.Localizations;

                ColumnTermLemma.DefaultCellStyle.Font = new Font("Charis SIL", 12);
                ColumnStatus.DefaultCellStyle.Font = new Font("Wingdings", 11);
                ColumnRenderings.DefaultCellStyle.Font = theStories.ProjSettings.Vernacular.LangFont;
                ColumnRenderings.DefaultCellStyle.ForeColor = theStories.ProjSettings.Vernacular.FontColor;

                termIndexRequested = -1;
                LoadTermsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_KeyTermsProblem,
                    Environment.NewLine, ex.Message), Properties.Resources.IDS_Caption);
                return;
            }
            finally
            {
                Cursor = curCursor;
            }
        }

        private void dataGridViewKeyTerms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditRenderings();
        }

        private void dataGridViewKeyTerms_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= visibleTerms.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex > CnColumnRenderings))
                return;

            Term term = visibleTerms[e.RowIndex];

            e.Value = GetCellValue(e.ColumnIndex, term);
        }

        private readonly string checkMark = "\uf0fc";
        private readonly string checkMarkX = "\uf0fc" + "\uf0fb";

        internal object GetCellValue(int columnIndex, Term term)
        {
            TermRendering termRendering;
            switch (columnIndex)
            {
                case CnColumnKeyTermLemma:
                    return term.Transliteration;
                case CnColumnStatus:
                    {
                        termRendering = renderings.GetRendering(term.Id);
                        BiblicalTermStatus status = termRendering.Status;
                        
                        if (status == BiblicalTermStatus.AllFound)
                            return checkMark;    // Check mark
                        if (status == BiblicalTermStatus.AllFoundOrDenied)
                            return checkMarkX;    // Check mark + x
                        
                        return " ";
                    }

                case CnColumnEnglishGloss:
                    return term.Gloss;
                    /*
                case CnColumnNationalGloss:
                    {
                        Localization loc = termLocalizations.GetTermLocalization(term.Id);
                        return loc.Gloss;  // term.LocalGloss;
                    }
                    */
                case CnColumnRenderings:
                    termRendering = renderings.GetRendering(term.Id);
                    return termRendering.Renderings;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            return "";
        }

        private void dataGridViewKeyTerms_SelectionChanged(object sender, System.EventArgs e)
        {
            LoadReferencesDisplay(true);
        }

        private void webBrowser_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                webBrowser.DoDragDrop(SelectedText, DragDropEffects.Copy);
        }

        /// <summary>
        /// Respond to user clicks in references display.
        /// </summary>
        private void webBrowser_BeforeNavigate(object s, onlyconnect.BeforeNavigateEventArgs e)
        {
            string link = e.Target;

            // Deny that missing reference rendering is a problem
            if (link.StartsWith("userclick:deny"))
            {
                DenyReference(link.Substring(15), true);
                e.Cancel = true;
            }

            // Remove denial that missing reference is a problem
            else if (link.StartsWith("userclick:undeny"))
            {
                DenyReference(link.Substring(17), false);
                e.Cancel = true;
            }

            // Scroll the editor to this reference.
            else if (link.StartsWith("userclick:scripref"))
            {
                string thisRef = link.Substring(19).Replace('_', ' ');
                string strStoryName, strAnchor;
                int nLineNumber;
                ParseReference(thisRef, out strStoryName, out nLineNumber, out strAnchor);
                _theSE.NavigateTo(strStoryName, nLineNumber, strAnchor);
                e.Cancel = true;
            }
        }

        protected void ParseReference(string strReference, out string strStoryName, out int nLineNumber, out string strAnchor)
        {
            // format for reference is: "Story: '{0}', line: {1}, anchor: {2}"
            const string CstrStoryPortion = "Story: '";
            const string CstrLinePortion = "', line: ";
            const string CstrAnchorPortion = ", anchor: ";

            System.Diagnostics.Debug.Assert(strReference.IndexOf(CstrStoryPortion) == 0);
            int nIndexStoryName = CstrStoryPortion.Length;
            int nIndexLineNumber = strReference.IndexOf(CstrLinePortion, nIndexStoryName) + CstrLinePortion.Length;
            int nIndexAnchor = strReference.IndexOf(CstrAnchorPortion, nIndexLineNumber) + CstrAnchorPortion.Length;

            strStoryName = strReference.Substring(nIndexStoryName, nIndexLineNumber - nIndexStoryName - CstrLinePortion.Length);
            string strLineNumber = strReference.Substring(nIndexLineNumber, nIndexAnchor - nIndexLineNumber - CstrAnchorPortion.Length);
            nLineNumber = 0;
            try
            {
                nLineNumber = Convert.ToInt32(strLineNumber) - 1;
            }
            catch { }

            strAnchor = strReference.Substring(nIndexAnchor);
        }

        private void DenyReference(string reference, bool doDeny)
        {
            if (SelectedTerm == null)
                return;  // should not happen

            reference = reference.Replace("%20", " ");
            TermRendering termRendering = renderings.GetRendering(SelectedTerm.Id);

            if (doDeny)
            {
                if (!termRendering.Denials.Contains(reference))
                {
                    termRendering.Status = BiblicalTermStatus.AllFoundOrDenied;
                    termRendering.Denials.Add(reference);
                }
            }
            else  // remove denial
            {
                termRendering.Status = BiblicalTermStatus.SomeMissing;
                while (termRendering.Denials.Contains(reference))
                    termRendering.Denials.Remove(reference);
            }

            LoadReferencesDisplay(false);

            reference = reference.Replace(" ", "_");
            
            // e.g. projectxnr_Story: 'BibStory', line: 1, anchor: Gen 2:4
            idToScrollTo = String.Format("project{0}_{1}", renderings.ScrTextName, reference);

            NotifyRenderingsChanged();
        }

        private void webBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                webBrowser.Copy();
            }
        }

        private void webBrowser_ReadyStateChanged(object sender, ReadyStateChangedEventArgs e)
        {
            if (e.ReadyState == "complete")
            {
                if (idToScrollTo != null)
                {
                    IHTMLElement element = webBrowser.DocumentHTML.GetElementByID(idToScrollTo);
                    if (element != null)
                        element.scrollIntoView(false);
                    idToScrollTo = null;
                }
            }
        }

        private bool loadTermsListInProgress = false;
        private bool repeatLoadTermsList = false;

        /// <summary>
        /// Make sure LoadTermsList is not called rentrantly
        /// (this seemed to be happened when the user updated the search box
        /// in the toolbar)
        /// </summary>
        private void LoadTermsList()
        {
            if (loadTermsListInProgress)
            {
                repeatLoadTermsList = true;
                return;
            }

            loadTermsListInProgress = true;

            try
            {
                while (true)
                {
                    NonReentrantLoadTermsList();
                    if (!repeatLoadTermsList)
                        break;
                    repeatLoadTermsList = false;
                }
            }
            finally
            {
                loadTermsListInProgress = false;
                repeatLoadTermsList = false;
            }
        }

        /// <summary>
        /// Load the biblical terms listed found in the top half of the tool window
        /// </summary>
        private void NonReentrantLoadTermsList()
        {
            dataGridViewKeyTerms.Rows.Clear();

            if (dataGridViewKeyTerms.RowCount > 0)
                dataGridViewKeyTerms.FirstDisplayedScrollingRowIndex = 0;

            if (dataGridViewKeyTerms.RowCount != visibleTerms.Count)
                dataGridViewKeyTerms.RowCount = visibleTerms.Count;
            else
                LoadReferencesDisplay(true);

            dataGridViewKeyTerms.Invalidate();
        }

        private string SelectedText
        {
            get
            {
                if (webBrowser.HtmlDocument2 == null)
                    return "";

                IHTMLTxtRange txtRange = webBrowser.HtmlDocument2.GetSelection().createRange() as IHTMLTxtRange;

                if (txtRange == null || txtRange.text == null)
                    return "";

                return txtRange.text;
            }
        }

        private void dataGridViewKeyTerms_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                Point clientPoint = dataGridViewKeyTerms.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hti = dataGridViewKeyTerms.HitTest(clientPoint.X, clientPoint.Y);
                System.Diagnostics.Debug.WriteLine(String.Format("{0}", hti));
                if ((hti.RowIndex < 0) || (hti.RowIndex >= visibleTerms.Count))
                    e.Effect = DragDropEffects.None;
                else
                    e.Effect = DragDropEffects.Copy;
            }
        }

        private void dataGridViewKeyTerms_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                string strRendering = (string)e.Data.GetData(typeof(string));
                if (!String.IsNullOrEmpty(strRendering))
                    strRendering = strRendering.Trim();

                if (String.IsNullOrEmpty(strRendering))
                    return;

                Point clientPoint = dataGridViewKeyTerms.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hti = dataGridViewKeyTerms.HitTest(clientPoint.X, clientPoint.Y);
                if ((hti.RowIndex < 0) || (hti.RowIndex >= visibleTerms.Count)
                    || (hti.ColumnIndex < 0) || (hti.ColumnIndex > CnColumnRenderings))
                    return;

                Term term = visibleTerms[hti.RowIndex];

                TermRendering termRendering = renderings.GetRendering(term.Id);
                if (termRendering.RenderingsList.Contains(strRendering))
                {
                    MessageBox.Show(Localizer.Str("Rendering already present."));
                    return;
                }

                termRendering.RenderingsList.Add(strRendering);
                string strRenderings;
                if (String.IsNullOrEmpty(termRendering.Renderings))
                    strRenderings = strRendering;
                else
                    strRenderings = termRendering.Renderings + ", " + strRendering;

                termRendering.Renderings = strRenderings;
                renderings.RenderingsChanged = true;
                dataGridViewKeyTerms.UpdateCellValue(hti.ColumnIndex, hti.RowIndex);
                LoadReferencesDisplay(true);
            }
        }

        // Index of selected determine.
        // visibleTerms and DataGrid are parallel.
        // Only one term can be selected.
        // -1 if no term selected (usually this can only happen if there are no terms
        // which meet current filter criteria.
        // 
        // WHEN SETTING
        // If not items in grid do nothing.
        // If selection beyond end of grid, select last row in grid
        private int SelectedTermIndex
        {
            get
            {
                if (dataGridViewKeyTerms.SelectedRows.Count == 0)
                    return -1;

                DataGridViewRow row = dataGridViewKeyTerms.SelectedRows[0];
                return row.Index;
            }
            set
            {
                if (dataGridViewKeyTerms.RowCount == 0)
                    return;

                int index = value;
                if (index >= dataGridViewKeyTerms.RowCount)
                    index = dataGridViewKeyTerms.RowCount;

                dataGridViewKeyTerms.Rows[index].Selected = true;
                dataGridViewKeyTerms.FirstDisplayedScrollingRowIndex = index;
            }
        }

        protected Term SelectedTerm
        {
            get
            {
                if (dataGridViewKeyTerms.SelectedRows.Count > 0)
                {
                    DataGridViewRow theRow = dataGridViewKeyTerms.SelectedRows[0];
                    if (theRow.Index < visibleTerms.Count)
                        return visibleTerms[theRow.Index];
                }
                return null;
            }
        }

        private void EditRenderings()
		{
			if (SelectedTerm == null)
				return;

			TermRendering termRendering = renderings.GetRendering(SelectedTerm.Id);
			Localization termLocalization = termLocalizations.GetTermLocalization(SelectedTerm.Id);

			string currentRenderings = termRendering.Renderings;
			if (!String.IsNullOrEmpty(SelectedText))
			{
				if (!String.IsNullOrEmpty(currentRenderings))
					currentRenderings += ", ";
				currentRenderings += SelectedText;
			}

			EditRenderingsForm form = new EditRenderingsForm(
                _theStories.ProjSettings.Vernacular.LangFont,
				currentRenderings,
				termRendering,
                _theStories.ProjSettings.Vernacular.LangCode,
				termLocalization);

			if (form.ShowDialog() == DialogResult.OK)
			{
				LoadReferencesDisplay(false);
			    NotifyRenderingsChanged();
			}

			form.Dispose();
		}

        private int termIndexRequested = -1;
        private Term termRequested = null;

        // Prevent rentrant calls to Redraw the reference window caused by users
        // madly clicking on different terms.
        // This is hard to make happen on fast machines but easy on slower machines.
        private void LoadReferencesDisplay(bool forceReloadVerseText)
        {
            if (SelectedTermIndex == -1)
                return;

            if (termIndexRequested != -1)
            {
                termIndexRequested = SelectedTermIndex;
                termRequested = visibleTerms[termIndexRequested];
                return;
            }

            termIndexRequested = SelectedTermIndex;
            termRequested = visibleTerms[termIndexRequested];

            while (true)
            {
                Term termBeingLoaded = termRequested;
                LoadReferencesDisplayNonRentrant(termIndexRequested);

                // If no new requests have come in, we are done
                if (termBeingLoaded == termRequested)
                    break;
            }

            termIndexRequested = -1;
        }

        protected Term _lastTerm = null;
        
        /// <summary>
        /// Redraw the reference window.
        /// </summary>
        /// <param name="myTermIndex">index of term to laod in visibilTerms</param>
        private void LoadReferencesDisplayNonRentrant(int myTermIndex)
        {
            if ((myTermIndex == -1) || (visibleTerms.Count <= myTermIndex))
                return;

            progressBarLoadingKeyTerms.Visible = true;
            Term myTerm = visibleTerms[myTermIndex];

            List<VerseRef> vrefs = new List<VerseRef>(myTerm.VerseRefs());

            if (_lastTerm != myTerm)
            {
                htmlBuilder.ReadVerseText(myTerm, vrefs, _theStories, progressBarLoadingKeyTerms);
                _lastTerm = myTerm;
            }

            BiblicalTermStatus status;

            // Build HTML text for references display.
            refererencesHtml = htmlBuilder.Build(vrefs, myTerm.Id, 
                _theStories.ProjSettings.ProjectFolder, _theStories.ProjSettings.Vernacular.LangCode, progressBarLoadingKeyTerms, out status);

            TermRendering termRendering = renderings.GetRendering(myTerm.Id);
            if (termRendering.Status != status)  // If status has changed, updated it.
            {
                termRendering.Status = status;
                NotifyRenderingsChanged();
            }

            // Load reference html
            webBrowser.LoadDocument(refererencesHtml);
            progressBarLoadingKeyTerms.Visible = false;
        }

        void NotifyRenderingsChanged()
        {
            renderings.RenderingsChanged = true;
        }

        private void BiblicalKeyTermsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            DoClosingTasks();
            Hide();
        }

        public void DoClosingTasks()
        {
            renderings.PromptForSave(_theStories.ProjSettings.ProjectFolder);
        }
    }
}
