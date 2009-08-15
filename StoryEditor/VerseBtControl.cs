#define RemoveLater

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class VerseBtControl : ResizableControl
    {
        internal const string CstrVerseName = "line: ";
        protected const string CstrFieldNameStoryLine = "StoryLine";
        protected const string CstrFieldNameAnchors = "Anchors";
        protected const string CstrFieldNameRetellings = "Retellings";
        protected const string CstrFieldNameTestQuestions = "TestQuestions";

        internal VerseData _verseData = null;
        protected string _strUnsMemberId = null;
        protected int _VerseNumber = -1;

        public string Guid = null;

        public VerseBtControl(StoryEditor aSE, VerseData dataVerse, int nVerseNumber)
            : base(aSE.theCurrentStory.ProjStage)
        {
            _verseData = dataVerse;
            Guid = _verseData.guid;
            InitializeComponent();
            VerseNumber = nVerseNumber;

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            tableLayoutPanel.Controls.Add(labelReference, 0, 0);
            tableLayoutPanel.Controls.Add(buttonDragDropHandle, 1, 0);

            int nNumRows = 1;
            // if the user is requesting one of the story lines (vernacular, nationalBT, or English), then...
            if (aSE.viewVernacularLangFieldMenuItem.Checked || aSE.viewNationalLangFieldMenuItem.Checked || aSE.viewEnglishBTFieldMenuItem.Checked)
            {
                // ask that control to do the Update View
                InitStoryLine(aSE, _verseData, nNumRows);
                nNumRows++;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                AnchorsData anAnchorsData = _verseData.Anchors;
                if (anAnchorsData != null)
                {
                    InitAnchors(anAnchorsData, nNumRows);
                    nNumRows++;
                }
            }

            if (aSE.viewRetellingFieldMenuItem.Checked)
            {
                if (_verseData.Retellings.Count > 0)
                {
                    InitRetellings(_verseData.Retellings, nNumRows);
                    nNumRows++;
                }
            }

            if (aSE.viewStoryTestingQuestionFieldMenuItem.Checked)
            {
                if (_verseData.TestQuestions.Count > 0)
                {
                    InitTestingQuestions(aSE, _verseData.TestQuestions, nNumRows);
                    nNumRows++;
                }
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        internal int VerseNumber
        {
            get { return _VerseNumber; }
            set
            {
                _VerseNumber = value;
                labelReference.Text = CstrVerseName + _VerseNumber;
            }
        }

        protected void InitStoryLine(StoryEditor aSE, VerseData aVerseData, int nLayoutRow)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine));
            StoryLineControl aStoryLineCtrl = new StoryLineControl(aSE, aVerseData);
            aStoryLineCtrl.Name = CstrFieldNameStoryLine;
            aStoryLineCtrl.ParentControl = this;

            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(aStoryLineCtrl, 2);
            tableLayoutPanel.Controls.Add(aStoryLineCtrl, 0, nLayoutRow);
        }

        protected void InitAnchors(AnchorsData anAnchorsData, int nLayoutRow)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameAnchors));
            AnchorControl anAnchorCtrl = new AnchorControl(StageLogic, anAnchorsData);
            anAnchorCtrl.Name = CstrFieldNameAnchors;
            anAnchorCtrl.ParentControl = this;
            
            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
            tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
        }

        protected void InitRetellings(RetellingsData aRetellingsData, int nLayoutRow)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameRetellings));
            MultiLineControl aRetellingsCtrl = new MultiLineControl(StageLogic, aRetellingsData);
            aRetellingsCtrl.Name = CstrFieldNameRetellings;
            aRetellingsCtrl.ParentControl = this;

            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(aRetellingsCtrl, 2);
            tableLayoutPanel.Controls.Add(aRetellingsCtrl, 0, nLayoutRow);
        }

        protected void InitTestingQuestions(StoryEditor aSE, TestQuestionsData aTQsData, int nLayoutRow)
        {
            for (int i = 0; i < aTQsData.Count; i++)
                InitTestQuestion(aSE, i, aTQsData[i], nLayoutRow);
        }

        protected void InitTestQuestion(StoryEditor aSE, int i, TestQuestionData aTQData, int nLayoutRow)
        {
            int nTQNumber = i + 1;
            TestingQuestionControl aTestingQuestionCtrl = new TestingQuestionControl(aSE, aTQData);
            aTestingQuestionCtrl.ParentControl = this;
            aTestingQuestionCtrl.Name = CstrFieldNameTestQuestions + nLayoutRow.ToString();

            int nRowIndex = nLayoutRow + i;
            InsertRow(nRowIndex);
            tableLayoutPanel.SetColumnSpan(aTestingQuestionCtrl, 2);
            tableLayoutPanel.Controls.Add(aTestingQuestionCtrl, 0, nRowIndex);
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                buttonDragDropHandle.DoDragDrop(_verseData, DragDropEffects.Move);
        }

        void buttonDragDropHandle_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            Form form = FindForm();
            System.Diagnostics.Debug.Assert(form is StoryEditor);
            if (form is StoryEditor)
            {
                StoryEditor theSE = (StoryEditor)form;

                // this code causes the vertical scroll bar to move if the user is dragging the mouse beyond
                //  the boundary of the flowLayout panel that these verse controls are sitting it.
                System.Drawing.Point pt = theSE.flowLayoutPanelVerses.PointToClient(MousePosition);
                if (theSE.flowLayoutPanelVerses.Bounds.Height < (pt.Y + 10))    // close to the bottom edge...
                    theSE.flowLayoutPanelVerses.VerticalScroll.Value += 10;     // bump the scroll bar down
                else if ((pt.Y < 10) && theSE.flowLayoutPanelVerses.VerticalScroll.Value > 0)   // close to the top edge, while the scroll bar position is non-zero
                    theSE.flowLayoutPanelVerses.VerticalScroll.Value -= Math.Min(10, theSE.flowLayoutPanelVerses.VerticalScroll.Value);

                if (e.Action != DragAction.Continue)
                    theSE.DimDropTargetButtons();
                else
                    theSE.LightUpDropTargetButtons(this);
            }
        }

        private void menuAddTestQuestion_Click(object sender, EventArgs e)
        {
            _verseData.TestQuestions.AddTestQuestion();

            // this is kind of sledge-hammer-y... but it works
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitVerseControls();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            splitStoryIntoVersesToolStripMenuItem.Enabled = _verseData.NationalBTText.HasData;

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
            tsmSub.Click += remTQ_Click;
            tsm.DropDown.Items.Add(tsmSub);
        }

        void remTQ_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem) sender;
            TestQuestionData theTQD = (TestQuestionData)tsm.Tag;
            _verseData.TestQuestions.Remove(theTQD);

            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitVerseControls();
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
            tsmSub.Click += theEH;
            tsm.DropDown.Items.Add(tsmSub);
        }

        private void remLine_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            MultipleLineDataConverter theObj = (MultipleLineDataConverter)tsm.Tag;
            theObj.RemoveLine(tsm.Text);

            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.ReInitVerseControls();
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
            tsm.Click += addTestQuestionAnswerToolStripMenuItem_Click;
            addTestQuestionAnswerToolStripMenuItem.DropDown.Items.Add(tsm);
        }

        private void addTestQuestionAnswerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();

            // gotta query for the UNS
            if (String.IsNullOrEmpty(_strUnsMemberId))
            {
                MemberPicker dlg = new MemberPicker(theSE.Stories, TeamMemberData.UserTypes.eUNS);
                if (dlg.ShowDialog() == DialogResult.OK)
                    _strUnsMemberId = dlg.SelectedMember.MemberGuid;
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

            theSE.ReInitVerseControls();
        }

        private void addRetellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();

            // gotta query for the UNS
            if (String.IsNullOrEmpty(_strUnsMemberId))
            {
                System.Diagnostics.Debug.Assert(theSE.Stories != null);
                MemberPicker dlg = new MemberPicker(theSE.Stories, TeamMemberData.UserTypes.eUNS);
                if (dlg.ShowDialog() == DialogResult.OK)
                    _strUnsMemberId = dlg.SelectedMember.MemberGuid;
            }

            if (String.IsNullOrEmpty(_strUnsMemberId))
                return;

            _verseData.Retellings.AddNewLine(_strUnsMemberId);

            // this is kind of sledge-hammer-y... but it works
            theSE.ReInitVerseControls();
        }

        private void deleteTheWholeVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this verse (and all associated consultant notes, etc)?", StoryEditor.CstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                StoryEditor theSE = (StoryEditor)FindForm();
                theSE.DeleteVerse(_verseData);
            }
        }

        private void addANewVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.AddNewVerse(this, 1, false);
        }

        private void addNewVersesAfterMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            int nNumNewVerses = Convert.ToInt32(tsmi.Text);

            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.AddNewVerse(this, nNumNewVerses, true);
        }

        private void addNewVersesBeforeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            int nNumNewVerses = Convert.ToInt32(tsmi.Text);

            StoryEditor theSE = (StoryEditor)FindForm();
            theSE.AddNewVerse(this, nNumNewVerses, false);
        }

        private void splitStoryIntoVersesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(_verseData.NationalBTText.HasData);

            StoryEditor theSE = (StoryEditor)FindForm();
            string strFullStop;
            if (String.IsNullOrEmpty(theSE.Stories.ProjSettings.NationalBT.FullStop))
            {
                strFullStop = Microsoft.VisualBasic.Interaction.InputBox("Enter the character in this language that ends a sentence (e.g. '.' for English or '।' for Hindi)", StoryEditor.CstrCaption, "।", Screen.PrimaryScreen.WorkingArea.Right / 2, Screen.PrimaryScreen.WorkingArea.Bottom / 2);
                if (String.IsNullOrEmpty(strFullStop))
                    return;
            }
            else
                strFullStop = theSE.Stories.ProjSettings.NationalBT.FullStop;

            List<string> lstSentences = CheckEndOfStateTransition.GetListOfSentences(_verseData.NationalBTText, strFullStop);
            System.Diagnostics.Debug.Assert((lstSentences != null) && (lstSentences.Count > 0));
            int nNewVerses = lstSentences.Count;
            while (nNewVerses-- > 1)
            {
                string strSentence = lstSentences[nNewVerses] + strFullStop;
                theSE.AddNewVerse(VerseNumber, strSentence);
            }
            _verseData.NationalBTText.SetValue(lstSentences[nNewVerses] + strFullStop);

            theSE.InitAllPanes();
        }
    }
}
