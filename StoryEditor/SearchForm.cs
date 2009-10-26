using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class SearchForm : Form
    {
        protected StoryEditor TheSE;
        protected SearchLookInProperties FindProperties = new SearchLookInProperties();

        public SearchForm()
        {
            InitializeComponent();

            FindProperties.ReadFromConfig(this);
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            FindProperties.WriteToConfig(this);

            Hide();
        }

        public void Show(StoryEditor theSE, bool bShowFind)
        {
            TheSE = theSE;
            checkBoxLookInStoryLanguage.Visible = 
                TheSE.StoryProject.ProjSettings.Vernacular.HasData;
            checkBoxLookInNationalBT.Visible = 
                TheSE.StoryProject.ProjSettings.NationalBT.HasData;
            checkBoxLookInEnglishBT.Visible =
                TheSE.StoryProject.ProjSettings.InternationalBT.HasData;

            checkBoxEnableFind.Checked = bShowFind;
            checkBoxEnableReplace.Checked = !bShowFind;

            if (!bShowFind)
                RemReplaceControlsHeight();

            UpdateReplaceControls(!bShowFind);
            Show();
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            DoFindNext();
        }

        public void ResetSearchParameters()
        {
            BoxesToSearch.Clear();
            ResetSearchStartParameters();
        }

        public void ResetSearchStartParameters()
        {
            LastStoryIndex = LastCtxBoxIndex = LastCharIndex = -1;
            buttonReplace.Enabled = false;
        }

        public static StorySearchIndex BoxesToSearch = new StorySearchIndex();
        protected int LastStoryIndex = -1;
        protected int LastCtxBoxIndex = -1;
        protected int LastCharIndex = -1;

        protected void InitSearchList(ref StorySearchIndex alstBoxesToSearch,
            out int nStoryIndex, out int nCtxBoxIndex, out int nCharIndex)
        {
            StringTransferSearchIndex ssidx = null;
                    
            // if the index is empty, then load it...
            if (alstBoxesToSearch.Count == 0)
            {
                if (checkBoxAllStories.Checked)
                {
                    TheSE.TheCurrentStoriesSet.IndexSearch(FindProperties,
                        ref alstBoxesToSearch);
                }
                else
                {
                    ssidx = alstBoxesToSearch.GetNewStorySearchIndex(TheSE.theCurrentStory.Name);
                    TheSE.theCurrentStory.IndexSearch(FindProperties, ref ssidx);
                }
            }
            
            // this should be 'else' because if we requery the index, then this
            //  index is definitely bad
            else if ((LastStoryIndex != -1) && (LastCtxBoxIndex != -1) && (LastCharIndex != -1))
            {
                nStoryIndex = LastStoryIndex;
                nCtxBoxIndex = LastCtxBoxIndex;
                nCharIndex = LastCharIndex;
                return;
            }

            ssidx = ssidx ?? BoxesToSearch[TheSE.theCurrentStory.Name];

            // check to see if we have a starting place...
            if (CtrlTextBox._inTextBox != null)
                for (int i = 0; i < ssidx.Count; i++)
                {
                    // start with the last text box selected... find it
                    StringTransfer boxToSearch = ssidx[i].StringTransfer;
                    if (boxToSearch.TextBox == CtrlTextBox._inTextBox)
                    {
                        nStoryIndex = LastStoryIndex = BoxesToSearch.IndexOf(ssidx);
                        nCtxBoxIndex = LastCtxBoxIndex = i;
                        nCharIndex = LastCharIndex = 
                            CaptureNextStartingCharIndex(boxToSearch.TextBox);
                        return;
                    }
                }

            // otherwise, just start at the 0th verse of *this* story
            nStoryIndex = LastStoryIndex = BoxesToSearch.IndexOf(ssidx);
            nCtxBoxIndex = LastCtxBoxIndex = nCharIndex = LastCharIndex = 0;
        }

        protected int CaptureNextStartingCharIndex(CtrlTextBox ctb)
        {
            return ctb.SelectionStart +
                   ctb.SelectionLength;
        }

        protected static string UpdateComboBox(ComboBox cb)
        {
            string str = cb.Text;
            if (!String.IsNullOrEmpty(str)
                && ((cb.Items.Count == 0)
                    || (str != (string)cb.Items[0])))
                cb.Items.Insert(0, str);
            return str;
        }

        protected Regex GetRegex(string strSearchString)
        {
            if ((regex == null) || (regex.ToString() != strSearchString))
                return new Regex(strSearchString);
            return regex;
        }

        Regex regex = null;

        public void DoFindNext()
        {
            string strToSearchFor = UpdateComboBox(comboBoxFindWhat);
            if (FindProperties.UseRegex)
                regex = GetRegex(strToSearchFor);
            else
                regex = null;

            int nLastStoryIndex, nLastCtxBoxIndex, nLastCharIndex;
            InitSearchList(ref BoxesToSearch, out nLastStoryIndex, out nLastCtxBoxIndex, out nLastCharIndex);

            CtrlTextBox ctbStopWhereWeStarted = null;
            int nStoryIndex = nLastStoryIndex;
            while (nStoryIndex < BoxesToSearch.Count)
            {
                StringTransferSearchIndex stsi = BoxesToSearch[nStoryIndex];
                int nCtxBoxIndex = nLastCtxBoxIndex;
                for (; nCtxBoxIndex < stsi.Count;  nCtxBoxIndex++)
                {
                    StringTransfer stringTransfer = stsi[nCtxBoxIndex].StringTransfer;
                    if (!stringTransfer.HasData)
                        continue;

                    string strValue = stringTransfer.ToString();
                    
                    // check to see if we're wrapped around and are starting
                    //  back at the beginning
                    // note: that it's possible that this stringTransfer
                    //  does not have a TextBox (e.g. if it isn't visible)
                    CtrlTextBox ctrlTextBox = stringTransfer.TextBox;

                    // get the index *after* the selected text (our starting index
                    //  of the search)
                    int nStartIndex = 0;
                    if (ctrlTextBox != null)
                    {
                        // if the length of the text in the text box is not the same
                        //  length as the text in the StringTransfer, then this 
                        //  assumption is probably no good
                        System.Diagnostics.Debug.Assert(ctrlTextBox.TextLength == strValue.Length);

                        if (ctbStopWhereWeStarted == ctrlTextBox)
                        {
                            ShowNotFound();
                            return;
                        }

                        nStartIndex = nLastCharIndex;
                        if (nLastCharIndex != 0)
                        {
                            nLastCharIndex = 0; // only do that once
                            ctrlTextBox.Select(0,0);    // don't leave it selected
                        }
                    }

                    int nFoundIndex = -1, nLengthToSelect = 0;
                    if (regex != null)
                    {
                        Match match = regex.Match(strValue, nStartIndex);
                        if (match.Success)
                        {
                            nFoundIndex = match.Index;
                            nLengthToSelect = match.Length;
                        }
                    }
                    else
                    {
                        nFoundIndex = strValue.IndexOf(strToSearchFor, nStartIndex);
                        nLengthToSelect = strToSearchFor.Length;
                    }

                    if (nFoundIndex != -1)
                    {
                        // found a match!
                        VerseString vs = stsi[nCtxBoxIndex];
                        System.Diagnostics.Debug.Assert(vs.StringTransfer == stringTransfer);
                        TheSE.NavigateTo(stsi.StoryName,
                                         vs.VerseNumber,
                                         vs.ViewToInsureIsOn,
                                         stringTransfer);
                        
                        // The navigation process should make it visible as well.
                        System.Diagnostics.Debug.Assert(stringTransfer.TextBox != null);
                        stringTransfer.TextBox.Select(nFoundIndex, nLengthToSelect);
                        LastStoryIndex = nStoryIndex;
                        LastCtxBoxIndex = nCtxBoxIndex;
                        LastCharIndex = CaptureNextStartingCharIndex(stringTransfer.TextBox);
                        buttonReplace.Enabled = true;
                        return;
                    }
                }

                // if we've reached the end of the verses in *this* story...
                if (!FindProperties.SearchAll)
                {
                    // if we were already searching from the beginning...
                    if (nLastCtxBoxIndex == 0)
                    {
                        // ... then we couldn't find it
                        ShowNotFound();
                    }

                    // otherwise, see if the user wants to start over from 0
                    else if (MessageBox.Show(Properties.Resources.IDS_StartFromBeginning, 
                        Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        nLastCtxBoxIndex = 0;

                        // have it stop where we started
                        ctbStopWhereWeStarted = CtrlTextBox._inTextBox ?? stsi[0].StringTransfer.TextBox;
                    }
                    else
                        return; // user said *don't* start over
                }

                // otherwise, we're supposed to search the next story as well.
                else
                {
                    nLastStoryIndex++;
                    nLastCtxBoxIndex = 0;
                }
            }

            // if we reach here, it's because we were searching all the stories
            //  and we reached the end and couldn't find it
            ShowNotFound();
        }

        protected void ShowNotFound()
        {
            Console.Beep();
            if (!Visible)
                Show(TheSE);
        }

        public class SearchLookInProperties
        {
            public bool ViewLookInOptions { get; set; }
            public bool StoryLanguage { get; set; }
            public bool NationalBT { get; set; }
            public bool EnglishBT { get; set; }
            public bool ConsultantNotes { get; set; }
            public bool CoachNotes { get; set; }
            public bool Retellings { get; set; }
            public bool TestQnA { get; set; }
            public bool SearchAll { get; set; }
            public bool UseRegex { get; set; }

            public void ReadFromConfig(SearchForm form)
            {
                ViewLookInOptions = form.checkBoxLookInExpander.Checked = Properties.Settings.Default.LookInExpander;
                StoryLanguage = form.checkBoxLookInStoryLanguage.Checked = Properties.Settings.Default.LookInStoryLanguage;
                NationalBT = form.checkBoxLookInNationalBT.Checked = Properties.Settings.Default.LookInNationalBT;
                EnglishBT = form.checkBoxLookInEnglishBT.Checked = Properties.Settings.Default.LookInEnglishBT;
                ConsultantNotes = form.checkBoxLookInConsultantNotes.Checked = Properties.Settings.Default.LookInConsultantNotes;
                CoachNotes = form.checkBoxLookInCoachNotes.Checked = Properties.Settings.Default.LookInCoachNotes;
                Retellings = form.checkBoxLookInRetellings.Checked = Properties.Settings.Default.LookInRetellings;
                TestQnA = form.checkBoxLookInTestQnA.Checked = Properties.Settings.Default.LookInTestQnA;
                SearchAll = form.checkBoxAllStories.Checked = Properties.Settings.Default.LookInAllStories;
                UseRegex = form.checkBoxUseRegex.Checked = Properties.Settings.Default.UseRegEx;

                if (Properties.Settings.Default.RecentFindWhat != null)
                    foreach (var item in Properties.Settings.Default.RecentFindWhat)
                        form.comboBoxFindWhat.Items.Add(item);
                else
                    Properties.Settings.Default.RecentFindWhat = new StringCollection();

                if (Properties.Settings.Default.RecentReplaceWith != null)
                    foreach (var item in Properties.Settings.Default.RecentReplaceWith)
                        form.comboBoxReplaceWith.Items.Add(item);
                else
                    Properties.Settings.Default.RecentReplaceWith = new StringCollection();
            }

            public void WriteToConfig(SearchForm form)
            {
                Properties.Settings.Default.LookInExpander = form.checkBoxLookInExpander.Checked;
                Properties.Settings.Default.LookInStoryLanguage = form.checkBoxLookInStoryLanguage.Checked;
                Properties.Settings.Default.LookInNationalBT = form.checkBoxLookInNationalBT.Checked;
                Properties.Settings.Default.LookInEnglishBT = form.checkBoxLookInEnglishBT.Checked;
                Properties.Settings.Default.LookInConsultantNotes = form.checkBoxLookInConsultantNotes.Checked;
                Properties.Settings.Default.LookInCoachNotes = form.checkBoxLookInCoachNotes.Checked;
                Properties.Settings.Default.LookInRetellings = form.checkBoxLookInRetellings.Checked;
                Properties.Settings.Default.LookInTestQnA = form.checkBoxLookInTestQnA.Checked;
                Properties.Settings.Default.LookInAllStories = form.checkBoxAllStories.Checked;
                Properties.Settings.Default.UseRegEx = form.checkBoxUseRegex.Checked;
                
                // keep the 15 most recent find whats and replace withs
                Properties.Settings.Default.RecentFindWhat.Clear();
                for (int i = 0; i < Math.Min(15, form.comboBoxFindWhat.Items.Count); i++)
                    Properties.Settings.Default.RecentFindWhat.Add((string)form.comboBoxFindWhat.Items[i]);

                Properties.Settings.Default.RecentReplaceWith.Clear();
                for (int i = 0; i < Math.Min(15, form.comboBoxReplaceWith.Items.Count); i++)
                    Properties.Settings.Default.RecentReplaceWith.Add((string)form.comboBoxReplaceWith.Items[i]);
                    
                Properties.Settings.Default.Save();
            }
        }

        public class VerseString
        {
            public StringTransfer StringTransfer { get; set; }
            public int VerseNumber { get; set; }
            public VerseData.ViewItemToInsureOn ViewToInsureIsOn { get; set; }

            public VerseString(int nVerseNum, StringTransfer strStringTransfer,
                VerseData.ViewItemToInsureOn viewItemToInsureOn)
            {
                StringTransfer = strStringTransfer;
                VerseNumber = nVerseNum;
                ViewToInsureIsOn = viewItemToInsureOn;
            }
        }

        public class StringTransferSearchIndex : List<VerseString>
        {
            public string StoryName { get; set; }

            public StringTransferSearchIndex(string strStoryName)
            {
                StoryName = strStoryName;
            }

            public VerseString AddNewVerseString(int nVerseNum,
                StringTransfer strStringTransfer, 
                VerseData.ViewItemToInsureOn viewItemToInsureOn)
            {
                var vs = new VerseString(nVerseNum, strStringTransfer, viewItemToInsureOn);
                Add(vs);
                return vs;
            }
        }

        public class StorySearchIndex : List<StringTransferSearchIndex>
        {
            public StringTransferSearchIndex GetNewStorySearchIndex(string strStoryName)
            {
                var stsi = new StringTransferSearchIndex(strStoryName);
                Add(stsi);
                return stsi;
            }

            public StringTransferSearchIndex this[string strStoryName]
            {
                get
                {
                    foreach (StringTransferSearchIndex stsi in this)
                        if (stsi.StoryName == strStoryName)
                            return stsi;
                    
                    System.Diagnostics.Debug.Assert(false);
                    return null;
                }
            }
        }

        private bool ChangeCheckedState(CheckBox checkBox)
        {
            ResetSearchParameters();
            return checkBox.Checked;
        }

        private void checkBoxLookInStoryLanguage_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.StoryLanguage = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInNationalBT_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.NationalBT = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInEnglishBT_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.EnglishBT = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInConsultantNotes_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.ConsultantNotes = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInCoachNotes_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.CoachNotes = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInTestQnA_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.TestQnA = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxLookInRetellings_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.Retellings = ChangeCheckedState(sender as CheckBox);
        }

        private void checkBoxAllStories_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.SearchAll = ChangeCheckedState(sender as CheckBox);
            
            // if we switch from this story to all stories, then we have to reset the index
            ResetSearchParameters();
        }

        private void checkBoxUseRegex_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.UseRegex = ChangeCheckedState(sender as CheckBox);
            buttonFindRegExHelper.Visible = FindProperties.UseRegex;

            buttonReplaceRegExHelper.Visible = FindProperties.UseRegex &&
                                               checkBoxEnableReplace.Checked;

            if (FindProperties.UseRegex)
            {
                tableLayoutPanel.SetColumnSpan(comboBoxFindWhat, 3);
                tableLayoutPanel.SetColumnSpan(comboBoxReplaceWith, 3);
            }
            else
            {
                tableLayoutPanel.SetColumnSpan(comboBoxFindWhat, 4);
                tableLayoutPanel.SetColumnSpan(comboBoxReplaceWith, 4);
            }
        }

        private void buttonRegExHelper_Click(object sender, EventArgs e)
        {
            ToolStripDropDownDirection dir = ToolStripDropDownDirection.BelowRight;
            buttonFindRegExHelper.ContextMenuStrip.Show(PointToScreen(buttonFindRegExHelper.Location), dir);
        }

        private void buttonReplaceRegExHelper_Click(object sender, EventArgs e)
        {
            ToolStripDropDownDirection dir = ToolStripDropDownDirection.BelowRight;
            buttonReplaceRegExHelper.ContextMenuStrip.Show(PointToScreen(buttonReplaceRegExHelper.Location), dir);
        }

        private void contextMenuStripExprBuilder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string strItem = ((ToolStripItem)e.ClickedItem).Text;
            if (strItem != regularExpressionHelpToolStripMenuItem.Text)
            {
                int nIndex = strItem.IndexOf(' ');
                comboBoxFindWhat.Text += strItem.Substring(0, nIndex);
                comboBoxFindWhat.Focus();
            }
        }

        private void regularExpressionHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // launch the ICU help
            string strCommandLine = Properties.Resources.RegexHelpProgram;
            StoryEditor.LaunchProgram(strCommandLine, null);
        }
        
        protected void UpdateReplaceControls(bool bShowReplace)
        {
            comboBoxReplaceWith.Visible = labelReplaceWith.Visible =
                buttonReplace.Visible = bShowReplace;
            buttonReplaceRegExHelper.Visible = FindProperties.UseRegex && bShowReplace;

            if (bShowReplace)
            {
                AddReplaceControlsHeight();
            }
            else
            {
                RemReplaceControlsHeight();
            }
        }

        protected void AddReplaceControlsHeight()
        {
            Height += (comboBoxReplaceWith.Height + labelReplaceWith.Height);
        }

        protected  void RemReplaceControlsHeight()
        {
            Height -= (comboBoxReplaceWith.Height + labelReplaceWith.Height);
        }

        private void checkBoxEnableFind_Click(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            checkBoxEnableReplace.Checked = !cb.Checked;
            UpdateReplaceControls(!cb.Checked);
        }

        private void checkBoxEnableReplace_Click(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            checkBoxEnableFind.Checked = !cb.Checked;
            UpdateReplaceControls(cb.Checked);
        }

        private void checkBoxLookInExpander_CheckedChanged(object sender, EventArgs e)
        {
            FindProperties.ViewLookInOptions = ((CheckBox) sender).Checked;

            if (FindProperties.ViewLookInOptions)
            {
                checkBoxLookInExpander.Text = " —";
                flowLayoutPanelLookIn.Visible = true;
                Height += flowLayoutPanelLookIn.GetPreferredSize(new Size(0, 1000)).Height;
            }
            else
            {
                checkBoxLookInExpander.Text = " +";
                Height -= flowLayoutPanelLookIn.Height;
                flowLayoutPanelLookIn.Visible = false;
            }
        }

        private void contextMenuStripReplaceWithExprBuilder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string strItem = e.ClickedItem.Text;
            if (strItem != findWhatTextToolStripMenuItem.Text)
            {
                System.Diagnostics.Debug.Assert(strItem.Length > 0);
                comboBoxReplaceWith.Text += @"$" + strItem.Substring(strItem.Length - 1);
                comboBoxReplaceWith.Focus();
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            // a replace is just a replace currently selected text in current textbox
            //  followed by a find next.
            string strFindWhat = UpdateComboBox(comboBoxFindWhat);
            string strReplaceWith = UpdateComboBox(comboBoxReplaceWith);
            if (CtrlTextBox._inTextBox != null)
            {
                if (regex == null)
                {
                    if (CtrlTextBox._inTextBox.SelectedText == strFindWhat)
                    {
                        CtrlTextBox._inTextBox.SelectedText = strReplaceWith;
                        LastCharIndex = CaptureNextStartingCharIndex(CtrlTextBox._inTextBox);
                    }
                }
                else
                {
                    Match match = regex.Match(CtrlTextBox._inTextBox.SelectedText);
                    if (match.Success)
                    {
                        string strReplacedText =
                            regex.Replace(CtrlTextBox._inTextBox.SelectedText, strReplaceWith);
                        CtrlTextBox._inTextBox.SelectedText = strReplacedText;
                        LastCharIndex = CaptureNextStartingCharIndex(CtrlTextBox._inTextBox);
                    }
                }
            }

            DoFindNext();
        }

        private void comboBoxFindWhat_TextChanged(object sender, EventArgs e)
        {
            string strFindWhat = UpdateComboBox(comboBoxFindWhat);
            if (CtrlTextBox._inTextBox != null)
            {
                if (FindProperties.UseRegex)
                {
                    regex = GetRegex(strFindWhat);
                    Match match = regex.Match(CtrlTextBox._inTextBox.SelectedText);
                    buttonReplace.Enabled = match.Success;
                }
                else
                {
                    buttonReplace.Enabled = (CtrlTextBox._inTextBox.SelectedText == strFindWhat);
                }
            }
        }
    }
}
