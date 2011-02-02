using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ECInterfaces;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class VerseBtControl : VerseControl
    {
        protected const string CstrFieldNameStoryLine = "StoryLine";
        protected const string CstrFieldNameAnchors = "Anchors";
        protected const string CstrFieldNameRetellings = "Retellings";
        protected const string CstrFieldNameTestQuestions = "TestQuestions";

        internal VerseData _verseData = null;

        public DirectableEncConverter TransliteratorVernacular;
        public DirectableEncConverter TransliteratorNationalBT;

        public VerseBtControl(StoryEditor theSE, LineFlowLayoutPanel parentFlowLayoutPanel,
            VerseData dataVerse, int nVerseNumber)
            : base(theSE.theCurrentStory.ProjStage, nVerseNumber, theSE, 
                parentFlowLayoutPanel)
        {
            _verseData = dataVerse;
            InitializeComponent();

            tableLayoutPanel.Controls.Add(labelReference, 0, 0);
            tableLayoutPanel.Controls.Add(buttonDragDropHandle, 1, 0);
            labelReference.Text = CstrVerseName + VerseNumber;

            if (theSE.viewTransliterationVernacular.Checked
                && !String.IsNullOrEmpty(theSE.LoggedOnMember.TransliteratorVernacular))
            {
                if (TransliteratorVernacular == null)
                {
                    TransliteratorVernacular = new DirectableEncConverter(theSE.LoggedOnMember.TransliteratorVernacular,
                                                                          theSE.LoggedOnMember.
                                                                              TransliteratorDirectionForwardVernacular,
                                                                          NormalizeFlags.None);
                }
            }
            else
                TransliteratorVernacular = null;  // in case it was set from before

            if (theSE.viewTransliterationNationalBT.Checked
                && !String.IsNullOrEmpty(theSE.LoggedOnMember.TransliteratorNationalBT))
            {
                if (TransliteratorNationalBT == null)
                {
                    TransliteratorNationalBT = new DirectableEncConverter(theSE.LoggedOnMember.TransliteratorNationalBT,
                                                                          theSE.LoggedOnMember.
                                                                              TransliteratorDirectionForwardNationalBT,
                                                                          NormalizeFlags.None);
                }
            }
            else
                TransliteratorNationalBT = null;

            InitControls(theSE);
        }

        protected void InitControls(StoryEditor theSE)
        {
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            int nNumRows = 1;
            // if the user is requesting one of the story lines (vernacular, nationalBT, or English), then...
            if (theSE.viewVernacularLangFieldMenuItem.Checked 
                || theSE.viewNationalLangFieldMenuItem.Checked 
                || theSE.viewEnglishBTFieldMenuItem.Checked
                || theSE.viewFreeTranslationToolStripMenuItem.Checked)
            {
                // ask that control to do the Update View
                InitStoryLine(theSE, _verseData, nNumRows);
                nNumRows++;
            }

            if (theSE.viewAnchorFieldMenuItem.Checked)
            {
                AnchorsData anAnchorsData = _verseData.Anchors;
                if (anAnchorsData != null)
                {
                    InitAnchors(anAnchorsData, nNumRows, TheSE.StoryProject.ProjSettings.InternationalBT);
                    nNumRows++;
                }
            }

            if (theSE.viewRetellingFieldMenuItem.Checked)
            {
                if (_verseData.Retellings.Count > 0)
                {
                    InitRetellings(_verseData.Retellings, nNumRows, 
                        theSE.theCurrentStory.CraftingInfo.Testors,
                        TheSE.StoryProject.ProjSettings);
                    nNumRows++;
                }
            }

            if (theSE.viewStoryTestingQuestionMenuItem.Checked || theSE.viewStoryTestingQuestionAnswerMenuItem.Checked)
            {
                if (_verseData.TestQuestions.Count > 0)
                {
                    InitTestingQuestions(theSE, _verseData.TestQuestions, nNumRows);
                    nNumRows++;
                }
            }

            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected void ClearControls()
        {
            while (tableLayoutPanel.RowCount > 1)
                RemoveRow(tableLayoutPanel.RowCount - 1);
        }

        public new bool Focus()
        {
            var slc = tableLayoutPanel.Controls[CstrFieldNameStoryLine] as StoryLineControl;
            if (slc != null) return slc.Focus();
            return false;
        }

        protected void InitStoryLine(StoryEditor theSE, VerseData aVerseData, int nLayoutRow)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine));
            StoryLineControl aStoryLineCtrl = new StoryLineControl(theSE, this, aVerseData);
            aStoryLineCtrl.Name = CstrFieldNameStoryLine;
            aStoryLineCtrl.ParentControl = this;

            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(aStoryLineCtrl, 2);
            tableLayoutPanel.Controls.Add(aStoryLineCtrl, 0, nLayoutRow);
        }

        protected void InitAnchors(AnchorsData anAnchorsData, int nLayoutRow, ProjectSettings.LanguageInfo li)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameAnchors));
            AnchorControl anAnchorCtrl = new AnchorControl(this, StageLogic, anAnchorsData, li);
            anAnchorCtrl.Name = CstrFieldNameAnchors;
            anAnchorCtrl.ParentControl = this;
            
            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(anAnchorCtrl, 2);
            tableLayoutPanel.Controls.Add(anAnchorCtrl, 0, nLayoutRow);
        }

        protected void InitRetellings(RetellingsData aRetellingsData, int nLayoutRow,
            List<string> astrTestors, ProjectSettings projSettings)
        {
            System.Diagnostics.Debug.Assert(!tableLayoutPanel.Controls.ContainsKey(CstrFieldNameRetellings));
            MultiLineControl aRetellingsCtrl = new MultiLineControl(this, StageLogic,
                aRetellingsData, projSettings, astrTestors, 
                projSettings.ShowRetellingVernacular,
                projSettings.ShowRetellingNationalBT,
                projSettings.ShowRetellingInternationalBT);

            aRetellingsCtrl.Name = CstrFieldNameRetellings;
            aRetellingsCtrl.ParentControl = this;

            InsertRow(nLayoutRow);
            tableLayoutPanel.SetColumnSpan(aRetellingsCtrl, 2);
            tableLayoutPanel.Controls.Add(aRetellingsCtrl, 0, nLayoutRow);
        }

        protected void InitTestingQuestions(StoryEditor theSE, TestQuestionsData aTQsData, int nLayoutRow)
        {
            for (int i = 0; i < aTQsData.Count; i++)
                InitTestQuestion(theSE, i, aTQsData[i], nLayoutRow, (i==0));
        }

        protected void InitTestQuestion(StoryEditor theSE, int i, TestQuestionData aTQData, int nLayoutRow, bool bShowHeader)
        {
            TestingQuestionControl aTestingQuestionCtrl = new TestingQuestionControl(theSE, this, aTQData, bShowHeader);
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
            {
                // the only function of the button here is to add a slot to type a con note
                StoryEditor theSE;
                if (!CheckForProperEditToken(out theSE))
                    return;

                buttonDragDropHandle.DoDragDrop(_verseData, DragDropEffects.Move);
            }
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
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            _verseData.TestQuestions.AddTestQuestion();
            if (!theSE.viewStoryTestingQuestionMenuItem.Checked)
                theSE.viewStoryTestingQuestionMenuItem.Checked = true;
            else
                UpdateViewOfThisVerse(theSE);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // if this is a Biblical story, we have to add a few menu items
            StoryEditor theSE = (StoryEditor)FindForm();
            if (theSE.theCurrentStory.CraftingInfo.IsBiblicalStory)
            {
                contextMenuStrip.Items.Insert(3, menuAddTestQuestion);
                /* adding answer and retelling spots is now done during end-of-state processing for ProjFacReadyForTest1
                contextMenuStrip.Items.Insert(3, addTestQuestionAnswerToolStripMenuItem);
                contextMenuStrip.Items.Insert(4, addRetellingToolStripMenuItem);
                */
            }

            // for answers, we have to attach them to the correct question
            /* adding answer spots is now done during end-of-state processing for ProjFacReadyForTest1
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
            */

            // add all the test questions to a drop down menu to allow removing them
            /*
            removeToolStripMenuItem.DropDown.Items.Clear();
            if (theSE.viewRetellingFieldMenuItem.Checked)
                AddRemoveRetellingSubmenus(_verseData.Retellings);
            if (theSE.viewStoryTestingQuestionMenuItem.Checked)
                AddRemoveTestQuestionsAndAnswersSubmenus(_verseData.TestQuestions);
            if (theSE.viewConsultantNoteFieldMenuItem.Checked)
                AddRemoveConNoteSubmenus("Consultant Notes",
                    _verseData.ConsultantNotes, remConNote_Click);
            if (theSE.viewCoachNotesFieldMenuItem.Checked)
                AddRemoveConNoteSubmenus("Coach Notes",
                    _verseData.CoachNotes, remCoachNote_Click);

            removeToolStripMenuItem.Enabled = (removeToolStripMenuItem.DropDown.Items.Count > 0);
            pasteTestingQuestionToolStripMenuItem.Enabled = (_myTQClipboard != null);
            pasteConsultantNoteToolStripMenuItem.Enabled = (_myConNoteClipboard != null);
            pasteCoachNoteToolStripMenuItem.Enabled = (_myCoachNoteClipboard != null);
            */

            if (_verseData.IsVisible)
            {
                hideVerseToolStripMenuItem.Text = "&Hide verse";
            }
            else
            {
                hideVerseToolStripMenuItem.Text = "&Unhide verse";
            }

            moveSelectedTextToANewLineToolStripMenuItem.Enabled =
                tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine);

            moveItemsToolStripMenuItem.Enabled = (_verseData.TestQuestions.HasData ||
                                                  _verseData.ConsultantNotes.HasData ||
                                                  _verseData.CoachNotes.HasData);
        }

        private void moveItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            var dlg = new CutItemPicker(_verseData, theSE.theCurrentStory.Verses, theSE);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                theSE.Modified = true;
                theSE.InitAllPanes();
            }
        }

        /* can't think of a good reason to allow them to remove a retelling (at least not this way)
        protected void AddRemoveConNoteSubmenus(string strSubmenu,
            ConsultNotesDataConverter theConNotes, EventHandler remConNoteClick)
        {
            if (theConNotes.Count == 0)
                return;

            ToolStripMenuItem tsm = AddHeadSubmenu(strSubmenu);
            int nIndex = 0;
            foreach (ConsultNoteDataConverter aConNote in theConNotes)
                AddRemConNoteSubmenu(tsm, strSubmenu, remConNoteClick, 
                    aConNote, nIndex++);
        }

        protected void AddRemoveTestQuestionsAndAnswersSubmenus(TestQuestionsData theTQs)
        {
            if (theTQs.Count == 0)
                return;

            ToolStripMenuItem tsm = AddHeadSubmenu("Testing Question");
            int nIndex = 0;
            foreach (TestQuestionData aTQ in theTQs)
                AddRemTQSubmenu(tsm, aTQ, nIndex++);
        }

        protected void AddRemTQSubmenu(ToolStripMenuItem tsm, TestQuestionData theTQ, int nIndex)
        {
            ToolStripMenuItem tsmSub = new ToolStripMenuItem();
            string strPrimary = (theTQ.QuestionVernacular.HasData) ? theTQ.QuestionVernacular.ToString() :
                (theTQ.QuestionNationalBT.HasData) ? theTQ.QuestionNationalBT.ToString() : theTQ.QuestionInternationalBT.ToString();
            string strSecondary = (theTQ.QuestionInternationalBT.HasData) ? theTQ.QuestionInternationalBT.ToString() :
                (theTQ.QuestionNationalBT.HasData) ? theTQ.QuestionNationalBT.ToString() : theTQ.QuestionVernacular.ToString();
            tsmSub.Name = strSecondary;
            tsmSub.Text = strPrimary;
            tsmSub.ToolTipText = strSecondary;
            tsmSub.Tag = theTQ;
            tsmSub.Click += remTQ_Click;
            tsm.DropDown.Items.Add(tsmSub);
        }

        protected void AddRemConNoteSubmenu(ToolStripMenuItem tsm, 
            string strSubmenu, EventHandler remConNoteClick,
            ConsultNoteDataConverter theConsultNote, int nIndex)
        {
            var tsmSub = new ToolStripMenuItem();
            string str = theConsultNote[0].ToString();
            tsmSub.Name = strSubmenu.Replace(" ", null) + nIndex;
            int len = str.IndexOf(Environment.NewLine);
            if (len == -1)
                len = Math.Min(50, str.Length);
            tsmSub.Text = str.Substring(0, len);
            // tsmSub.ToolTipText = str;
            tsmSub.Tag = theConsultNote;
            tsmSub.Click += remConNoteClick;
            tsm.DropDown.Items.Add(tsmSub);
        }

        protected static ConsultNoteDataConverter _myConNoteClipboard;

        void remConNote_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            var tsm = (ToolStripMenuItem)sender;
            var theConNote = (ConsultNoteDataConverter)tsm.Tag;
            _myConNoteClipboard = theConNote;
            _verseData.ConsultantNotes.Remove(theConNote);
            theSE.htmlConsultantNotesControl.LoadDocument();
            Application.DoEvents();
            theSE.htmlConsultantNotesControl.ScrollToVerse(VerseNumber);
            theSE.Modified = true;
        }

        protected static ConsultNoteDataConverter _myCoachNoteClipboard;

        void remCoachNote_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            var tsm = (ToolStripMenuItem)sender;
            var theConNote = (ConsultNoteDataConverter)tsm.Tag;
            _myCoachNoteClipboard = theConNote;
            _verseData.CoachNotes.Remove(theConNote);
            theSE.htmlCoachNotesControl.LoadDocument();
            Application.DoEvents();
            theSE.htmlCoachNotesControl.ScrollToVerse(VerseNumber);
            theSE.Modified = true;
        }

        protected static TestQuestionData _myTQClipboard;

        void remTQ_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            TestQuestionData theTQD = (TestQuestionData)tsm.Tag;
            _myTQClipboard = theTQD;
            _verseData.TestQuestions.Remove(theTQD);
            UpdateViewOfThisVerse(theSE);
        }
        
        protected void AddRemoveRetellingSubmenus(RetellingsData theRD)
        {
            ToolStripMenuItem tsm = AddHeadSubmenu("Retelling(s)");

            int nRetellingNum = 1;
            foreach (StringTransfer rd in theRD)
            {
                string strText = rd.ToString();
                if (String.IsNullOrEmpty(strText))
                    strText = String.Format("<no retelling #{0}>", nRetellingNum);
                nRetellingNum++;

                AddSubmenu(tsm, strText, theRD, remLine_Click);
            }
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
            UpdateViewOfThisVerse();
        }
        */

        internal void UpdateViewOfThisVerse(StoryEditor theSE)
        {
            System.Diagnostics.Debug.Assert(theSE != null);
            ClearControls();
            InitControls(theSE);
            UpdateHeight(Width);
            tableLayoutPanel.PerformLayout();
            PerformLayout();
            theSE.Modified = true;
        }

        /*
        protected ToolStripMenuItem AddHeadSubmenu(string strHeading, EventHandler tsm_Click)
        {
            ToolStripMenuItem tsm = new ToolStripMenuItem();
            tsm.Name = strHeading;
            tsm.Text = strHeading;
            tsm.Click += tsm_Click;
            removeToolStripMenuItem.DropDown.Items.Add(tsm);
            return tsm;
        }

        protected const string CstrAddAnswerPrefix = "For the question: ";
        protected void AddAnswerSubmenu(string strText, int nIndex)
        {
            ToolStripMenuItem tsm = new ToolStripMenuItem
                                        {
                                            Name = strText,
                                            Size = new System.Drawing.Size(202, 22),
                                            Text = CstrAddAnswerPrefix + strText,
                                            Tag = nIndex
                                        };
            tsm.Click += addTestQuestionAnswerToolStripMenuItem_Click;
            addTestQuestionAnswerToolStripMenuItem.DropDown.Items.Add(tsm);
        }

        private void addTestQuestionAnswerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;

            //have to query for the UNS that this test is from (if we don't already have it).
            System.Diagnostics.Debug.Assert((theSE != null) && (theSE.theCurrentStory != null) && (theSE.theCurrentStory.CraftingInfo != null));
            System.Diagnostics.Debug.Assert(theSE.theCurrentStory.CraftingInfo.Testors.Count == _verseData.TestQuestions.Count);
            System.Diagnostics.Debug.Assert((tsm.Tag != null) && (tsm.Tag is int) && (((int)tsm.Tag) < _verseData.TestQuestions.Count));
            TestQuestionData tqd = _verseData.TestQuestions[(int)tsm.Tag];
            System.Diagnostics.Debug.Assert(CstrAddAnswerPrefix + tqd.QuestionVernacular.ToString() == tsm.Text);
            byte nNewIndex = (byte)tqd.Answers.Count;
            while (((nNewIndex >= theSE.theCurrentStory.CraftingInfo.Testors.Count) || String.IsNullOrEmpty(theSE.theCurrentStory.CraftingInfo.Testors[nNewIndex])))
            {
                MemberPicker dlg = new MemberPicker(theSE.StoryProject, TeamMemberData.UserTypes.eUNS);
                dlg.Text = "Choose the UNS that gave these answers";
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return;

                tqd.Answers.AddNewLine(dlg.SelectedMember.MemberGuid);
                break;
            }

            UpdateViewOfThisVerse();
        }

        private void addRetellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
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
            */
        /*
  }

        private void pasteTestingQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(_myTQClipboard != null);
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            _verseData.TestQuestions.Add(_myTQClipboard);
            UpdateViewOfThisVerse(theSE);
            theSE.Modified = true;
        }

        private void pasteConsultantNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            PasteConNote(_myConNoteClipboard, _verseData.ConsultantNotes,
                theSE.htmlConsultantNotesControl);
            theSE.Modified = true;
        }

        private void pasteCoachNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            PasteConNote(_myCoachNoteClipboard, _verseData.CoachNotes,
                theSE.htmlCoachNotesControl);
            theSE.Modified = true;
        }

        private void PasteConNote(ConsultNoteDataConverter theCNDC, 
            ICollection<ConsultNoteDataConverter> theCNsDC, HtmlConNoteControl ctrlHtml)
        {
            System.Diagnostics.Debug.Assert(theCNDC != null);
            theCNsDC.Add(theCNDC);
            ctrlHtml.LoadDocument();
            Application.DoEvents();
            ctrlHtml.ScrollToVerse(VerseNumber);
        }
  */

        private void deleteTheWholeVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            if (_verseData.HasData)
            {
                DialogResult res = MessageBox.Show(
                    Properties.Resources.IDS_VerseNotEmptyHideQuery,
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    theSE.VisiblizeVerse(_verseData, false);
                    return;
                }
                
                if (res == DialogResult.Cancel)
                    return;
            }

            if (MessageBox.Show(
                Properties.Resources.IDS_DeleteVerseQuery, 
                OseResources.Properties.Resources.IDS_Caption, 
                MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                theSE.DeleteVerse(_verseData);
            }
        }

        private void addANewVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.AddNewVerse(VerseNumber - 1, 1, false);
        }

        private void addNewVersesAfterMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            int nNumNewVerses = Convert.ToInt32(tsmi.Text);

            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.AddNewVerse(VerseNumber - 1, nNumNewVerses, true);
        }

        private void moveSelectedTextToANewLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            MoveSelectedTextToANewLineToolStripMenuItem(theSE);
        }

        private void MoveSelectedTextToANewLineToolStripMenuItem(StoryEditor theSE)
        {
            System.Diagnostics.Debug.Assert(tableLayoutPanel.Controls.ContainsKey(CstrFieldNameStoryLine));
            StoryLineControl slc = tableLayoutPanel.Controls[CstrFieldNameStoryLine] as StoryLineControl;
            string strVernacular, strNationalBT, strEnglishBT, strFreeTranslation;
            slc.GetTextBoxValues(out strVernacular, out strNationalBT, out strEnglishBT,
                out strFreeTranslation);
            
            // all this verse to have it's buttons shown (so the editor can delete now
            //  obsolete comments)
            _verseData.AllowConNoteButtonsOverride();

            // make a copy and clear out the stuff that we'll have them manually move later
            VerseData verseNew = new VerseData(_verseData);
            verseNew.TestQuestions.Clear();
            verseNew.ConsultantNotes.Clear();
            verseNew.CoachNotes.Clear();

            // copyVerseToClipboardToolStripMenuItem_Click(null, null);
            verseNew.VernacularText.SetValue(strVernacular);
            verseNew.NationalBTText.SetValue(strNationalBT);
            verseNew.InternationalBTText.SetValue(strEnglishBT);
            verseNew.FreeTranslationText.SetValue(strFreeTranslation);

            theSE.DoPasteVerse(VerseNumber, verseNew);
            // VerseData verseDest = PasteVerseToIndex(theSE, VerseNumber);
            var dlg = new CutItemPicker(_verseData, verseNew, VerseNumber + 1, theSE);
            if (dlg.IsSomethingToMove)
                dlg.ShowDialog();
            theSE.InitAllPanes();
        }

        private void addNewVersesBeforeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            int nNumNewVerses = Convert.ToInt32(tsmi.Text);

            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.AddNewVerse(VerseNumber - 1, nNumNewVerses, false);
        }

        // since you can't put something on the clipboard that isn't 'serializable', until I 
        //  make that change, use an internal 'clipboard' to do copying
        protected static VerseData _myClipboard = null;

        private void copyVerseToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Copies the verse to the clipboard.
                // Clipboard.SetDataObject(_verseData);
                // make a copy so that if the user makes changes after the copy, we won't be
                //  referring to the same object.
                _myClipboard = new VerseData(_verseData);
            }
            catch   // ignore errors
            {
            }
        }

        protected VerseData PasteVerseToIndex(StoryEditor theSE, int nInsertionIndex)
        {
            if (_myClipboard != null)
            {
                var theNewVerse = new VerseData(_myClipboard);
                theNewVerse.AllowConNoteButtonsOverride();
                // make another copy, so that the guid is changed
                theSE.DoPasteVerse(nInsertionIndex, theNewVerse);
                return theNewVerse;
            }
            return null;
        }
        private void pasteVerseFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            PasteVerseToIndex(theSE, VerseNumber - 1);
            theSE.InitAllPanes();
        }

        private void pasteVerseFromClipboardAfterThisOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // the only function of the button here is to add a slot to type a con note
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            PasteVerseToIndex(theSE, VerseNumber);
            theSE.InitAllPanes();
        }

        private void hideVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.VisiblizeVerse(_verseData, 
                (_verseData.IsVisible) ? false : true   // toggle
                );
        }

        private void splitStoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoryEditor theSE;
            if (!CheckForProperEditToken(out theSE))
                return;

            theSE.SplitStory(_verseData);
        }
    }
}
