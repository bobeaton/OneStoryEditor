#define RemoveLater

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class VerseBtControl : ResizableControl
    {
        internal const string CstrVerseName = "Verse: ";
        protected const string CstrFieldNameStoryLine = "StoryLine";
        protected const string CstrFieldNameAnchors = "Anchors";
        protected const string CstrFieldNameRetellings = "Retellings";
        protected const string CstrFieldNameTestQuestionsLabel = "TestQuestionsLabel";
        protected const string CstrTestQuestionsLabelFormat = "tst({0}):";
        protected const string CstrFieldNameTestQuestions = "TestQuestions";

        internal VerseData _verseData = null;
        protected string _strUnsMemberId = null;

        internal int VerseNumber = -1;
        protected int _nRowIndexStoryLine = -1;
        protected int _nRowIndexAnchors = -1;
        protected int _nRowIndexRetelling = -1;
        protected int _nRowIndexTestingQuestionGroup = -1;
        protected List<TestingQuestionControl> _lstTestQuestionControls = null;

        public string Guid = null;

        public VerseBtControl(StoryEditor aSE, VerseData dataVerse, int nVerseNumber)
        {
            _verseData = dataVerse;
            Guid = _verseData.guid;
            VerseNumber = nVerseNumber;
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = CstrVerseName + nVerseNumber.ToString();
            this.tableLayoutPanel.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonDragDropHandle, 1, 0);

            UpdateView(aSE);

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public override void UpdateView(StoryEditor aSE)
        {
            int nNumRows = 1;
            // if the user is requesting one of the story lines (vernacular, nationalBT, or English), then...
            if (aSE.viewVernacularLangFieldMenuItem.Checked || aSE.viewNationalLangFieldMenuItem.Checked || aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((_nRowIndexStoryLine == -1) || (_nRowIndexStoryLine == nNumRows), "fix bad assumption (VerseBtControl.cs.49): bob_eaton@sall.com");

                // ask that control to do the Update View
                InitStoryLine(aSE, _verseData, nNumRows);
                _nRowIndexStoryLine = nNumRows++;
            }
            else if (_nRowIndexStoryLine != -1)
            {
                RemoveRow(_nRowIndexStoryLine);
                _nRowIndexStoryLine = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((_nRowIndexAnchors == -1) || (_nRowIndexAnchors == nNumRows), "fix bad assumption (VerseBtControl.cs.64): bob_eaton@sall.com");

                AnchorsData anAnchorsData = _verseData.Anchors;
                if (anAnchorsData != null)
                {
                    InitAnchors(anAnchorsData, nNumRows);
                    _nRowIndexAnchors = nNumRows++;
                }
            }
            else if (_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(_nRowIndexAnchors);
                _nRowIndexAnchors = -1;
            }

            if (aSE.viewRetellingFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((_nRowIndexRetelling == -1) || (_nRowIndexRetelling == nNumRows), "fix bad assumption (VerseBtControl.cs.92): bob_eaton@sall.com");

                if (_verseData.Retellings.Count > 0)
                {
                    InitRetellings(_verseData.Retellings, nNumRows);
                    _nRowIndexRetelling = nNumRows++;
                }
            }
            else if (_nRowIndexRetelling != -1)
            {
                // now get rid of the anchor row
                RemoveRow(_nRowIndexRetelling);
                _nRowIndexRetelling = -1;
            }

            if (aSE.viewStoryTestingQuestionFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert(
                    ((_lstTestQuestionControls == null) && (_nRowIndexTestingQuestionGroup == -1))
                    || ((_lstTestQuestionControls != null) && (_nRowIndexTestingQuestionGroup == nNumRows)), "fix bad assumption (VerseBtControl.cs.111): bob_eaton@sall.com");

                if (_verseData.TestQuestions.Count > 0)
                {
                    InitTestingQuestions(aSE, _verseData.TestQuestions, nNumRows);
                    _nRowIndexTestingQuestionGroup = nNumRows++;
                }
            }
            else if (_nRowIndexTestingQuestionGroup != -1)
            {
                // now get rid of the anchor row
                foreach (TestingQuestionControl aTQC in _lstTestQuestionControls)
                {
                    int nRowIndex = tableLayoutPanel.GetRow(aTQC);
                    RemoveRow(nRowIndex);
                }
                _nRowIndexTestingQuestionGroup = -1;
                _lstTestQuestionControls = null;
            }
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (_nRowIndexAnchors >= nLayoutRowIndex)
                _nRowIndexAnchors++;
            if (_nRowIndexRetelling >= nLayoutRowIndex)
                _nRowIndexRetelling++;
            if (_nRowIndexTestingQuestionGroup >= nLayoutRowIndex)
                _nRowIndexTestingQuestionGroup++;
        }

        protected override void RemoveRow(int nLayoutRowIndex)
        {
            base.RemoveRow(nLayoutRowIndex);
            if (_nRowIndexAnchors > nLayoutRowIndex)
                _nRowIndexAnchors--;
            if (_nRowIndexRetelling > nLayoutRowIndex)
                _nRowIndexRetelling--;
            if (_nRowIndexTestingQuestionGroup > nLayoutRowIndex)
                _nRowIndexTestingQuestionGroup--;
        }

        protected void InitStoryLine(StoryEditor aSE, VerseData aVerseData, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike with the Anchor control, which is all on or all off)
            System.Diagnostics.Debug.Assert((_nRowIndexStoryLine != -1) || !tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine));
            if (tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine))
            {
                StoryLineControl aStoryLineCtrl = (StoryLineControl)tableLayoutPanel.Controls[CstrFieldNameStoryLine];
                aStoryLineCtrl.UpdateView(aSE);
            }
            else
            {
                StoryLineControl aStoryLineCtrl = new StoryLineControl(aSE, aVerseData);
                aStoryLineCtrl.Name = CstrFieldNameStoryLine;
                aStoryLineCtrl.ParentControl = this;

                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aStoryLineCtrl, 2);
                tableLayoutPanel.Controls.Add(aStoryLineCtrl, 0, nLayoutRow);
            }
        }

        protected void InitAnchors(AnchorsData anAnchorsData, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike here with the Anchor control, which is all on or all off)
            System.Diagnostics.Debug.Assert((_nRowIndexAnchors != -1) || !tableLayoutPanel.Controls.ContainsKey(CstrFieldNameAnchors));
            if (!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(anAnchorsData);
                anAnchorCtrl.Name = CstrFieldNameAnchors;
                anAnchorCtrl.ParentControl = this;
                
                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
                tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
            }
        }

        protected void InitRetellings(RetellingsData aRetellingsData, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike here with the Retellings control, which is all on or all off)
            System.Diagnostics.Debug.Assert((_nRowIndexRetelling != -1) || !tableLayoutPanel.Controls.ContainsKey(CstrFieldNameRetellings));
            if (!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameRetellings))
            {
                MultiLineControl aRetellingsCtrl = new MultiLineControl(aRetellingsData);
                aRetellingsCtrl.Name = CstrFieldNameRetellings;
                aRetellingsCtrl.ParentControl = this;

                InsertRow(nLayoutRow);
                tableLayoutPanel.SetColumnSpan(aRetellingsCtrl, 2);
                tableLayoutPanel.Controls.Add(aRetellingsCtrl, 0, nLayoutRow);
            }
        }

        protected void InitTestingQuestions(StoryEditor aSE, TestQuestionsData aTQsData, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike here with the Anchor control, which is all on or all off)
            if (_nRowIndexTestingQuestionGroup == -1)
            {
                if (aTQsData.Count > 0)
                {
                    _lstTestQuestionControls = new List<TestingQuestionControl>(aTQsData.Count);
                    for (int i = 0; i < aTQsData.Count; i++)
                        InitTestQuestion(aSE, i, aTQsData[i], nLayoutRow);
                }
            }
        }

        protected void InitTestQuestion(StoryEditor aSE, int i, TestQuestionData aTQData, int nLayoutRow)
        {
            int nTQNumber = i + 1;
            Label label = new Label();
            label.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            label.AutoSize = true;
            label.Name = CstrFieldNameTestQuestionsLabel + nTQNumber.ToString();
            label.Text = String.Format(CstrTestQuestionsLabelFormat, nTQNumber);

            TestingQuestionControl aTestingQuestionCtrl = new TestingQuestionControl(aSE, aTQData);
            aTestingQuestionCtrl.ParentControl = this;
            aTestingQuestionCtrl.Name = CstrFieldNameTestQuestions + nLayoutRow.ToString();

            int nRowIndex = nLayoutRow + i;
            InsertRow(nRowIndex);
            tableLayoutPanel.Controls.Add(label, 0, nRowIndex);
            tableLayoutPanel.Controls.Add(aTestingQuestionCtrl, 1, nRowIndex);
            _lstTestQuestionControls.Add(aTestingQuestionCtrl);
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                buttonDragDropHandle.DoDragDrop(this, DragDropEffects.Move | DragDropEffects.Copy);
        }

        void buttonDragDropHandle_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            Form form = FindForm();
            System.Diagnostics.Debug.Assert(form is StoryEditor);
            if (form is StoryEditor)
            {
                StoryEditor aSE = (StoryEditor)form;
                if (e.Action != DragAction.Continue)
                    aSE.DimDropTargetButtons();
                else
                    aSE.LightUpDropTargetButtons();
            }
        }

        private void menuAddTestQuestion_Click(object sender, EventArgs e)
        {
            _verseData.TestQuestions.AddTestQuestion();

            // this is kind of sledge-hammer-y... but it works
            StoryEditor theSE = (StoryEditor)FindForm();
            if (theSE.viewStoryTestingQuestionFieldMenuItem.Checked)
                theSE.viewStoryTestingQuestionFieldMenuItem.Checked = false;
            theSE.viewStoryTestingQuestionFieldMenuItem.Checked = true;
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // for answers, we have to attach them to the correct question
            int nTestQuestionCount = _verseData.TestQuestions.Count;
            if (nTestQuestionCount > 1)
            {
                addTestQuestionAnswerToolStripMenuItem.DropDown.Items.Clear();
                int nIndex = 0;
                foreach (TestQuestionData aTQD in _verseData.TestQuestions)
                    AddAnswerSubmenu(aTQD.QuestionVernacular.ToString(), nIndex++);
            }
            else if (nTestQuestionCount == 0)
                addTestQuestionAnswerToolStripMenuItem.Enabled = false;

            // add all the test questions, retellings, and answers to a drop down menu to remove them
            removeToolStripMenuItem.DropDown.Items.Clear();
            StoryEditor theSE = (StoryEditor)FindForm();
            if (theSE.viewRetellingFieldMenuItem.Checked)
                AddRemoveRetellingSubmenus(_verseData.Retellings);
            if (theSE.viewStoryTestingQuestionFieldMenuItem.Checked)
                AddRemoveTestQuestionsAndAnswersSubmenus(_verseData.TestQuestions);
        }

        protected void AddRemoveTestQuestionsAndAnswersSubmenus(TestQuestionsData theTQs)
        {
            ToolStripMenuItem tsm = AddHeadSubmenu("Testing Question(s)");
            int nIndex = 0;
            foreach (TestQuestionData aTQ in theTQs)
                AddRemTQSubmenu(tsm, aTQ, nIndex++);
        }

        protected void AddRemTQSubmenu(ToolStripMenuItem tsm, TestQuestionData theTQ, int nIndex)
        {
            ToolStripMenuItem tsmSub = new ToolStripMenuItem();
            tsmSub.Name = theTQ.QuestionEnglish.ToString();
            tsmSub.Text = theTQ.QuestionVernacular.ToString();
            tsmSub.ToolTipText = theTQ.QuestionEnglish.ToString();
            tsmSub.Tag = theTQ;
            tsmSub.Click += new EventHandler(remTQ_Click);
            tsm.DropDown.Items.Add(tsmSub);
        }

        void remTQ_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem) sender;
            TestQuestionData theTQD = (TestQuestionData)tsm.Tag;
            _verseData.TestQuestions.Remove(theTQD);

            StoryEditor theSE = (StoryEditor)FindForm();
            if (theSE.viewStoryTestingQuestionFieldMenuItem.Checked)
                theSE.viewStoryTestingQuestionFieldMenuItem.Checked = false;
            theSE.viewStoryTestingQuestionFieldMenuItem.Checked = true;
        }

        protected void AddRemoveRetellingSubmenus(RetellingsData theRD)
        {
            ToolStripMenuItem tsm = AddHeadSubmenu("Retelling(s)");
            foreach (StringTransfer rd in theRD)
                AddSubmenu(tsm, rd.ToString(), theRD, remLine_Click);
        }

        protected void AddSubmenu(ToolStripMenuItem tsm, string strText, MultipleLineDataConverter theObj, EventHandler theEH)
        {
            ToolStripMenuItem tsmSub = new ToolStripMenuItem();
            tsmSub.Name = strText;
            tsmSub.Text = strText;
            tsmSub.Tag = theObj;
            tsmSub.Click += new EventHandler(theEH);
            tsm.DropDown.Items.Add(tsmSub);
        }

        private void remLine_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            MultipleLineDataConverter theObj = (MultipleLineDataConverter)tsm.Tag;
            theObj.RemoveLine(tsm.Text);

            StoryEditor theSE = (StoryEditor)FindForm();
            if (theSE.viewRetellingFieldMenuItem.Checked)
                theSE.viewRetellingFieldMenuItem.Checked = false;
            theSE.viewRetellingFieldMenuItem.Checked = true;
        }

        protected const string CstrAddAnswerPrefix = "For the question: ";
        protected ToolStripMenuItem AddHeadSubmenu(string strHeading)
        {
            ToolStripMenuItem tsm = new ToolStripMenuItem();
            tsm.Name = strHeading;
            tsm.Text = strHeading;
            removeToolStripMenuItem.DropDown.Items.Add(tsm);
            return tsm;
        }

        protected void AddAnswerSubmenu(string strText, int nIndex)
        {
            ToolStripMenuItem tsm = new ToolStripMenuItem();
            tsm.Name = strText;
            tsm.Size = new System.Drawing.Size(202, 22);
            tsm.Text = CstrAddAnswerPrefix + strText;
            tsm.Tag = nIndex;
            tsm.Click += new System.EventHandler(addTestQuestionAnswerToolStripMenuItem_Click);
            addTestQuestionAnswerToolStripMenuItem.DropDown.Items.Add(tsm);
        }

        private void addTestQuestionAnswerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();

            // gotta query for the UNS
            if (String.IsNullOrEmpty(_strUnsMemberId))
            {
                UnsPicker dlg = new UnsPicker(theSE.Stories);
                if (dlg.ShowDialog() == DialogResult.OK)
                    _strUnsMemberId = dlg.SelectedUnsGuid;
            }

            if (String.IsNullOrEmpty(_strUnsMemberId))
                return;

            ToolStripMenuItem theTSM = (ToolStripMenuItem) sender;
            int nIndex;
            if (theTSM.Tag == null)
                nIndex = 0;
            else
                nIndex = (int)theTSM.Tag;

            System.Diagnostics.Debug.Assert(nIndex < _verseData.TestQuestions.Count);

            TestQuestionData theTQD = _verseData.TestQuestions[nIndex];
            theTQD.Answers.AddNewLine(_strUnsMemberId);

            if (theSE.viewStoryTestingQuestionFieldMenuItem.Checked)
                theSE.viewStoryTestingQuestionFieldMenuItem.Checked = false;
            theSE.viewStoryTestingQuestionFieldMenuItem.Checked = true;
        }

        private void addRetellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();

            // gotta query for the UNS
            if (String.IsNullOrEmpty(_strUnsMemberId))
            {
                System.Diagnostics.Debug.Assert(theSE.Stories != null);
                UnsPicker dlg = new UnsPicker(theSE.Stories);
                if (dlg.ShowDialog() == DialogResult.OK)
                    _strUnsMemberId = dlg.SelectedUnsGuid;
            }

            if (String.IsNullOrEmpty(_strUnsMemberId))
                return;

            _verseData.Retellings.AddNewLine(_strUnsMemberId);

            // this is kind of sledge-hammer-y... but it works
            if (theSE.viewRetellingFieldMenuItem.Checked)
                theSE.viewRetellingFieldMenuItem.Checked = false;
            theSE.viewRetellingFieldMenuItem.Checked = true;
        }

        private void deleteTheWholeVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this verse (and all associated consultant notes, etc)?", StoryEditor.CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                StoryEditor theSE = (StoryEditor)FindForm();
                theSE.DeleteVerse(this);
            }
        }
    }
}
