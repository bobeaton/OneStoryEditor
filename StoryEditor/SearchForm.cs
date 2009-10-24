using System;
using System.Collections.Generic;
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

        public void Show(StoryEditor theSE)
        {
            TheSE = theSE;
            checkBoxLookInStoryLanguage.Visible = 
                TheSE.StoryProject.ProjSettings.Vernacular.HasData;
            checkBoxLookInNationalBT.Visible = 
                TheSE.StoryProject.ProjSettings.NationalBT.HasData;
            checkBoxLookInEnglishBT.Visible =
                TheSE.StoryProject.ProjSettings.InternationalBT.HasData;

            labelStatus.Text = String.Format("Click 'Find Next' to search in {0}",
                    (!checkBoxAllStories.Checked) ? "this story" : "all stories");

            Show();
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            DoFindNext();
        }

        public void ResetSearchParameters()
        {
            BoxesToSearch.Clear();
            LastStoryIndex = LastCtxBoxIndex = LastCharIndex = -1;
        }

        public static StorySearchIndex BoxesToSearch = new StorySearchIndex();
        protected int LastStoryIndex = -1;
        protected int LastCtxBoxIndex = -1;
        protected int LastCharIndex = -1;

        protected bool InitSearchList(ref StorySearchIndex alstBoxesToSearch,
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
            else if ((LastStoryIndex != -1) && (LastCtxBoxIndex != -1))
            {
                nStoryIndex = LastStoryIndex;
                nCtxBoxIndex = LastCtxBoxIndex;
                nCharIndex = LastCharIndex;
                return true;
            }

            ssidx = ssidx ?? BoxesToSearch[TheSE.theCurrentStory.Name];

            // check to see if we have a starting place. If not, then just 
            //  start at the 0th verse of *this* story
            if (CtrlTextBox._inTextBox == null)
            {
                nStoryIndex = LastStoryIndex = BoxesToSearch.IndexOf(ssidx);
                nCtxBoxIndex = LastCtxBoxIndex = nCharIndex = LastCharIndex = 0;
                return true;
            }

            // otherwise, we start with the last text box selected... find it
            for (int i = 0; i < ssidx.Count; i++)
            {
                StringTransfer boxToSearch = ssidx[i].StringTransfer;
                if (boxToSearch.TextBox == CtrlTextBox._inTextBox)
                {
                    nStoryIndex = LastStoryIndex = BoxesToSearch.IndexOf(ssidx);
                    nCtxBoxIndex = LastCtxBoxIndex = i;
                    nCharIndex = LastCharIndex = 
                        CaptureNextStartingCharIndex(boxToSearch.TextBox);
                    return true;
                }
            }

            // the for loop shouldn't fail or I don't understand this properly
            System.Diagnostics.Debug.Assert(false);
            nStoryIndex = nCtxBoxIndex = nCharIndex = -1;
            return false;
        }

        protected int CaptureNextStartingCharIndex(CtrlTextBox ctb)
        {
            return ctb.SelectionStart +
                   ctb.SelectionLength;
        }

        public void DoFindNext()
        {
            string strToSearchFor = comboBoxFindWhat.Text;
            if ((comboBoxFindWhat.Items.Count == 0) || (strToSearchFor != (string)comboBoxFindWhat.Items[0]))
                comboBoxFindWhat.Items.Insert(0, strToSearchFor);
            int nLastStoryIndex, nLastCtxBoxIndex, nLastCharIndex;
            if (!InitSearchList(ref BoxesToSearch, out nLastStoryIndex,
                out nLastCtxBoxIndex, out nLastCharIndex))
            {
                Console.Beep(); // not found
                return;
            }

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

                    int nFoundIndex = strValue.IndexOf(strToSearchFor, nStartIndex);
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
                        stringTransfer.TextBox.Select(nFoundIndex, strToSearchFor.Length);
                        LastStoryIndex = nStoryIndex;
                        LastCtxBoxIndex = nCtxBoxIndex;
                        LastCharIndex = CaptureNextStartingCharIndex(stringTransfer.TextBox);
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
            labelStatus.Text = "Not Found!";
        }

        public class SearchLookInProperties
        {
            public bool StoryLanguage { get; set; }
            public bool NationalBT { get; set; }
            public bool EnglishBT { get; set; }
            public bool ConsultantNotes { get; set; }
            public bool CoachNotes { get; set; }
            public bool Retellings { get; set; }
            public bool TestQnA { get; set; }
            public bool SearchAll { get; set; }

            public void ReadFromConfig(SearchForm form)
            {
                StoryLanguage = form.checkBoxLookInStoryLanguage.Checked = Properties.Settings.Default.LookInStoryLanguage;
                NationalBT = form.checkBoxLookInNationalBT.Checked = Properties.Settings.Default.LookInNationalBT;
                EnglishBT = form.checkBoxLookInEnglishBT.Checked = Properties.Settings.Default.LookInEnglishBT;
                ConsultantNotes = form.checkBoxLookInConsultantNotes.Checked = Properties.Settings.Default.LookInConsultantNotes;
                CoachNotes = form.checkBoxLookInCoachNotes.Checked = Properties.Settings.Default.LookInCoachNotes;
                Retellings = form.checkBoxLookInRetellings.Checked = Properties.Settings.Default.LookInRetellings;
                TestQnA = form.checkBoxLookInTestQnA.Checked = Properties.Settings.Default.LookInTestQnA;
                SearchAll = form.checkBoxAllStories.Checked = Properties.Settings.Default.LookInAllStories;
            }

            public void WriteToConfig(SearchForm form)
            {
                Properties.Settings.Default.LookInStoryLanguage = form.checkBoxLookInStoryLanguage.Checked;
                Properties.Settings.Default.LookInNationalBT = form.checkBoxLookInNationalBT.Checked;
                Properties.Settings.Default.LookInEnglishBT = form.checkBoxLookInEnglishBT.Checked;
                Properties.Settings.Default.LookInConsultantNotes = form.checkBoxLookInConsultantNotes.Checked;
                Properties.Settings.Default.LookInCoachNotes = form.checkBoxLookInCoachNotes.Checked;
                Properties.Settings.Default.LookInRetellings = form.checkBoxLookInRetellings.Checked;
                Properties.Settings.Default.LookInTestQnA = form.checkBoxLookInTestQnA.Checked;
                Properties.Settings.Default.LookInAllStories = form.checkBoxAllStories.Checked;
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
    }
}
