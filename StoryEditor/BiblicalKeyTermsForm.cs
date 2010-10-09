using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
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
        internal TermRenderingsList renderings;   // Rendering info for terms in target language
        internal TermLocalizations termLocalizations;   // Term info unique to UI language
        string refererencesHtml;  // text loaded into html references browser
        string idToScrollTo = null;		// Set this to scroll to an elementid when loaded
        protected StoryEditor _theSE;
        internal ProjectSettings.LanguageInfo MainLang;
        internal ProjectSettings _projSettings;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html
        internal BiblicalTermsList _biblicalTerms;   // All Biblical terms

        public BiblicalKeyTermsForm(StoryEditor theSE, ProjectSettings projSettings, ProjectSettings.LanguageInfo liMainLang)
        {
            _theSE = theSE;
            MainLang = liMainLang;
            _projSettings = projSettings;
            InitializeComponent();
            _biblicalTerms = BiblicalTermsList.GetBiblicalTerms(projSettings.ProjectFolder);
            htmlBuilder = new BiblicalTermsHTMLBuilder(projSettings);

            BiblicalTermsList.BiblicalTermsListChanged += BiblicalTermsList_BiblicalTermsListChanged;
        }

        void BiblicalTermsList_BiblicalTermsListChanged(object sender, EventArgs e)
        {
            GetBiblicalTerms();
            ShowPartTwo();
        }

        private void GetBiblicalTerms()
        {
            _biblicalTerms = BiblicalTermsList.GetBiblicalTerms(_projSettings.ProjectFolder);
        }

        private List<string> _lstRefs;

        public void Show(AnchorsData theAnchors, StoryProjectData theStoryProject)
        {
            if (Properties.Settings.Default.BiblicalTermsDlgHeight != 0)
            {
                Bounds = new Rectangle(Properties.Settings.Default.BiblicalTermsDlgLocation,
                    new Size(Properties.Settings.Default.BiblicalTermsDlgWidth,
                        Properties.Settings.Default.BiblicalTermsDlgHeight));
            }

            Show();
            Cursor curCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
#if DEBUG
                // Test the Sword names vs. the names used by Paratext
                foreach (string str in AnchorData.mapSwordToParatextBookCodes.Values)
                {
                    VerseRef verser = new VerseRef(str + " 1:1");
                }
                foreach (string str in AnchorData.mapSwordToParatextBookCodes.Keys)
                {
                    string strAnchor = str + " 1:1";
                    AnchorData anchorData = new AnchorData(strAnchor, strAnchor);
                    VerseRef verser = new VerseRef(anchorData.AnchorAsVerseRef);
                }
#endif

                _lstRefs = new List<string>();
                foreach (AnchorData anAnchor in theAnchors)
                {
                    VerseRef verseRef = new VerseRef(anAnchor.AnchorAsVerseRef);
                    _lstRefs.Add(verseRef.BBBCCCVVV());
                }

                ShowPartTwo();

                // indicate that we've checked the key terms here.
                theAnchors.IsKeyTermChecked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.IDS_KeyTermsProblem,
                    Environment.NewLine, ex.Message), OseResources.Properties.Resources.IDS_Caption);
                return;
            }
            finally
            {
                Cursor = curCursor;
            }
        }

        private void ShowPartTwo()
        {
            visibleTerms.Clear();
            progressBarLoadingKeyTerms.Visible = true;
            progressBarLoadingKeyTerms.Maximum = _biblicalTerms.Terms.Count;
            progressBarLoadingKeyTerms.Value = 0;
            foreach (Term term in _biblicalTerms.Terms)
            {
                if (toolStripButtonShowAllTerms.Checked)
                    visibleTerms.Add(term);
                else
                    foreach (Verse aVerseReference in term.References)
                        if (_lstRefs.Contains(aVerseReference.VerseRef.BBBCCCVVV()))
                        {
                            visibleTerms.Add(term);
                            break;
                        }
                progressBarLoadingKeyTerms.Value++;
            }

            Text = String.Format("Key Terms -- {0}", _biblicalTerms.Caption);
            termIndexRequested = -1;
            dataGridViewKeyTerms.Rows.Clear();  // just in case we exit next

            if (visibleTerms.Count == 0)
            {
                progressBarLoadingKeyTerms.Visible = false;
                MessageBox.Show(Localizer.Str("There are no Biblical Terms in this verse(s)."));
                return;
            }

            renderings = TermRenderingsList.GetTermRenderings(_projSettings.ProjectFolder, MainLang.LangCode);
            termLocalizations = TermLocalizations.Localizations;

            // TODO: temporary hack. If we are using the Hindi 'MyKeyTerms' db, then
            // change the 'English Gloss' column header to Hindi and change the font
            if (_biblicalTerms.IsMyBiblicalTerms)
            {
                ColumnGlossEnglish.HeaderText = "Gloss";
                ColumnGlossEnglish.DefaultCellStyle.Font = new Font("Arial Unicode MS", 12);
            }

            ColumnTermLemma.DefaultCellStyle.Font = new Font("Charis SIL", 12);
            ColumnStatus.DefaultCellStyle.Font = new Font("Wingdings", 11);
            ColumnRenderings.DefaultCellStyle.Font = MainLang.FontToUse;
            ColumnRenderings.DefaultCellStyle.ForeColor = MainLang.FontColor;

            LoadTermsList();
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
                string thisRef = BiblicalTermsHTMLBuilder.DecodeAsHtmlId(link.Substring(19));
                string strStoryName, strAnchor;
                int nLineNumber;
                BiblicalTermsHTMLBuilder.ParseReference(thisRef, out strStoryName, out nLineNumber, out strAnchor);
                _theSE.NavigateTo(strStoryName, nLineNumber, strAnchor);
                e.Cancel = true;
            }
        }

        private void DenyReference(string reference, bool doDeny)
        {
            if (SelectedTerm == null)
                return;  // should not happen

            reference = reference.Replace("%20", " ");
            TermRendering termRendering = renderings.GetRendering(SelectedTerm.Id);

            string strOneStoryDenialUrl = BiblicalTermsHTMLBuilder.ConstructUrlFromReference(_theSE.StoryProject, reference);
            if (doDeny)
            {
                if (!termRendering.Denials.Contains(strOneStoryDenialUrl))
                {
                    termRendering.Status = BiblicalTermStatus.AllFoundOrDenied;
                    termRendering.Denials.Add(strOneStoryDenialUrl);
                }
            }
            else  // remove denial
            {
                termRendering.Status = BiblicalTermStatus.SomeMissing;
                while (termRendering.Denials.Contains(strOneStoryDenialUrl))
                    termRendering.Denials.Remove(strOneStoryDenialUrl);
            }

            LoadReferencesDisplay(false);

            reference = reference.Replace(" ", "_");
            
            // e.g. projectxnr_Story: 'BibStory' line: 1 anchor: Gen 2:4
            idToScrollTo = "project1_" + reference;

            NotifyRenderingsChanged();
        }

        private void webBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                webBrowser.Copy();
            }

            System.Diagnostics.Debug.WriteLine(String.Format("e.KeyValue = {0}", e.KeyValue));
            // the "+" key will add the rendering
            if ((e.KeyValue == 107) || (e.KeyValue == 187))
            {
                AddRendering(SelectedText, SelectedTermIndex);
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
            /* now done as part of "SelectedTermIndex = i - 1;" below
            else
                LoadReferencesDisplay(true);
            */

            // go through the rows added one-by-one and a) change the height to match the 
            //  font for hte "main language" (the one in the left column, on which the renderings
            //  are based) and b) select those rows one-by-one so that it triggers an update
            //  of the 'status' display (e.g. if a new anchor was added and it becomes 
            //  "SomeMissing" it'd be nice to see that up front).
            for (int i = dataGridViewKeyTerms.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow aRow = dataGridViewKeyTerms.Rows[i];
                aRow.Height = MainLang.FontToUse.Height;
                SelectedTermIndex = i;
            }

            int nSplitterDistance = dataGridViewKeyTerms.ColumnHeadersHeight + toolStrip1.Height + 15;
            int nMaxToShow = Math.Min(5, dataGridViewKeyTerms.Rows.Count);
            nSplitterDistance += nMaxToShow * MainLang.FontToUse.Height;
            splitContainer1.SplitterDistance = nSplitterDistance;

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
                Point clientPoint = dataGridViewKeyTerms.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hti = dataGridViewKeyTerms.HitTest(clientPoint.X, clientPoint.Y);
                if ((hti.RowIndex < 0) || (hti.RowIndex >= visibleTerms.Count)
                    || (hti.ColumnIndex < 0) || (hti.ColumnIndex > CnColumnRenderings))
                    return;

                string strRendering = (string)e.Data.GetData(typeof(string));
                AddRendering(strRendering, hti.RowIndex);
            }
        }

        protected static Regex SearchForSpanID = new Regex("<SPAN id=\"(.*?)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        protected void AddRendering(string strRendering, int nRowIndex)
        {
            // try to keep track of where we were so we can go back there
            string strID = webBrowser.HtmlDocument2.GetActiveElement().innerHTML;
            if (!String.IsNullOrEmpty(strID))
            {
                MatchCollection mc = SearchForSpanID.Matches(strID);
                if (mc.Count > 0)
                    idToScrollTo = mc[0].Groups[1].Value;
            }

            if (!String.IsNullOrEmpty(strRendering))
                strRendering = strRendering.Trim();

            if (String.IsNullOrEmpty(strRendering) || (nRowIndex == -1))
                return;

            Term term = visibleTerms[nRowIndex];
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
            dataGridViewKeyTerms.UpdateCellValue(CnColumnRenderings, nRowIndex);
            LoadReferencesDisplay(true);
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
                MainLang.FontToUse,
				currentRenderings,
				termRendering,
                MainLang.LangCode,
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

            if (_lastTerm != myTerm)
            {
                htmlBuilder.ReadVerseText(myTerm, _theSE.StoryProject, progressBarLoadingKeyTerms);
                _lastTerm = myTerm;
            }

            BiblicalTermStatus status;

            // Build HTML text for references display.
            htmlBuilder.BuildRenderings(_projSettings.ProjectFolder, myTerm.Id);

            refererencesHtml = htmlBuilder.Build(_theSE.StoryProject, progressBarLoadingKeyTerms, true, out status);

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
            if (renderings != null)
                renderings.PromptForSave(_projSettings.ProjectFolder);

            Properties.Settings.Default.BiblicalTermsDlgLocation = Location;
            Properties.Settings.Default.BiblicalTermsDlgHeight = Bounds.Height;
            Properties.Settings.Default.BiblicalTermsDlgWidth = Bounds.Width;
            Properties.Settings.Default.Save();
        }

        private void toolStripButtonEditRenderings_Click(object sender, EventArgs e)
        {
            EditRenderings();
        }

        private void toolStripButtonAddKeyTerm_Click(object sender, EventArgs e)
        {
            EditKeyTerm(null);
        }

        private void EditKeyTerm(Term term)
        {
            // first see if we're using the 'my biblical terms' (in the project folder), which is the only list that is editable.
            if (!_biblicalTerms.IsMyBiblicalTerms)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_CreateMyBiblicalTermsQuery,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel)
                    return;

                string strListToCopyFrom = _biblicalTerms.Caption;
                res = MessageBox.Show(String.Format(Properties.Resources.IDS_VerifyOverwriteMyBiblicalTerms,
                    strListToCopyFrom), OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.AbortRetryIgnore);

                if (res == DialogResult.Abort)
                    return;

                if (res == DialogResult.Ignore)
                {
                    // clobber any existing 'My Biblical Terms'
                    string fileName = Path.Combine(_projSettings.ProjectFolder, "MyBiblicalTerms.xml");
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }

                // now get a new one (if needed) to merge with
                BiblicalTermsList mbt = BiblicalTermsList.GetMyBiblicalTermsList(_projSettings.ProjectFolder);

                foreach (Term termInList in _biblicalTerms.Terms)
                {
                    if (mbt.GetIfPresent(termInList.Id) == null)
                        mbt.Add(termInList);
                }

                mbt.Save();
                BiblicalTermsList.SelectMyBiblicalTermsList();
                System.Diagnostics.Debug.Assert(_biblicalTerms.IsMyBiblicalTerms);
            }
            
            System.Diagnostics.Debug.Assert(_biblicalTerms.FileNameTerms.Contains(_projSettings.ProjectFolder));

            // otherwise, if we now have an editable list...
            EditBiblicalTermForm form = new EditBiblicalTermForm(term, _projSettings.ProjectFolder);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadReferencesDisplay(false);
                NotifyRenderingsChanged();
            }

            form.Dispose();
        }

        private void toolStripButtonEditKeyTerm_Click(object sender, EventArgs e)
        {
            if (SelectedTerm == null)
                return;

            EditKeyTerm(SelectedTerm);
        }

        private void toolStripButtonSelectKeyTermsList_Click(object sender, EventArgs e)
        {
            BiblicalTermsList.SelectTermsList(_projSettings.ProjectFolder);
        }

        private void toolStripButtonAddRendering_Click(object sender, EventArgs e)
        {
            AddRendering(SelectedText, SelectedTermIndex);
        }

        private void toolStripButtonShowAllTerms_Click(object sender, EventArgs e)
        {
            BiblicalTermsList_BiblicalTermsListChanged(sender, e);
        }

        private void toolStripButtonDeleteKeyTerm_Click(object sender, EventArgs e)
        {
            if (!_biblicalTerms.IsMyBiblicalTerms)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_CanDeleteOnlyFromMyBiblicalTerms,
                    OseResources.Properties.Resources.IDS_Caption);
                return;
            }

            var term = SelectedTerm;
            if (term == null)
                return;

            if (MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmDeleteKeyTerm,
                Environment.NewLine, term), OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) 
                == DialogResult.Yes)
            {
                _biblicalTerms.Terms.Remove(term);
                _biblicalTerms.Save();

                LoadTermsList();
                LoadReferencesDisplay(false);
            }
        }
    }
}
