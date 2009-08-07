#define RemoveLater

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class VerseBtControl : ResizableControl
    {
        internal const string cstrVerseName = "Verse: ";
        protected const string cstrFieldNameStoryLine = "StoryLine";
        protected const string cstrFieldNameAnchors = "Anchors";
        protected const string cstrFieldNameRetellings = "Retellings";
        protected const string cstrFieldNameTestQuestionsLabel = "TestQuestionsLabel";
        protected const string cstrTestQuestionsLabelFormat = "tst({0}):";
        protected const string cstrFieldNameTestQuestions = "TestQuestions";

        public VerseData VerseData = null;

        internal int VerseNumber = -1;
        protected int m_nRowIndexStoryLine = -1;
        protected int m_nRowIndexAnchors = -1;
        protected int m_nRowIndexRetelling = -1;
        protected int m_nRowIndexTestingQuestionGroup = -1;
        protected List<TestingQuestionControl> m_lstTestQuestionControls = null;

        public string Guid = null;

        public VerseBtControl(StoryEditor aSE, VerseData VerseData, int nVerseNumber)
        {
            Guid = VerseData.guid;
            VerseNumber = nVerseNumber;
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = cstrVerseName + nVerseNumber.ToString();
            this.tableLayoutPanel.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonDragDropHandle, 1, 0);

            VerseData = VerseData;
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
                System.Diagnostics.Debug.Assert((m_nRowIndexStoryLine == -1) || (m_nRowIndexStoryLine == nNumRows), "fix bad assumption (VerseBtControl.cs.49): bob_eaton@sall.com");

                // ask that control to do the Update View
                InitStoryLine(aSE, VerseData, nNumRows);
                m_nRowIndexStoryLine = nNumRows++;
            }
            else if (m_nRowIndexStoryLine != -1)
            {
                RemoveRow(m_nRowIndexStoryLine);
                m_nRowIndexStoryLine = -1;
            }

            if (aSE.viewAnchorFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((m_nRowIndexAnchors == -1) || (m_nRowIndexAnchors == nNumRows), "fix bad assumption (VerseBtControl.cs.64): bob_eaton@sall.com");

                AnchorsData anAnchorsData = VerseData.Anchors;
                if (anAnchorsData != null)
                {
                    InitAnchors(anAnchorsData, nNumRows);
                    m_nRowIndexAnchors = nNumRows++;
                }
            }
            else if (m_nRowIndexAnchors != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexAnchors);
                m_nRowIndexAnchors = -1;
            }

            if (aSE.viewRetellingFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert((m_nRowIndexRetelling == -1) || (m_nRowIndexRetelling == nNumRows), "fix bad assumption (VerseBtControl.cs.92): bob_eaton@sall.com");

                if (VerseData.Retellings.Count > 0)
                {
                    InitRetellings(VerseData.Retellings, nNumRows);
                    m_nRowIndexRetelling = nNumRows++;
                }
            }
            else if (m_nRowIndexRetelling != -1)
            {
                // now get rid of the anchor row
                RemoveRow(m_nRowIndexRetelling);
                m_nRowIndexRetelling = -1;
            }

            if (aSE.viewStoryTestingQuestionFieldMenuItem.Checked)
            {
                // if we've already initialized the control, then it must have this project row index (i.e. nNumRows)
                System.Diagnostics.Debug.Assert(
                    ((m_lstTestQuestionControls == null) && (m_nRowIndexTestingQuestionGroup == -1))
                    || ((m_lstTestQuestionControls != null) && (m_nRowIndexTestingQuestionGroup == nNumRows)), "fix bad assumption (VerseBtControl.cs.111): bob_eaton@sall.com");

                if (VerseData.TestQuestions.Count > 0)
                {
                    InitTestingQuestions(aSE, VerseData.TestQuestions, nNumRows);
                    m_nRowIndexTestingQuestionGroup = nNumRows++;
                }
            }
            else if (m_nRowIndexTestingQuestionGroup != -1)
            {
                // now get rid of the anchor row
                foreach (TestingQuestionControl aTQC in m_lstTestQuestionControls)
                {
                    int nRowIndex = tableLayoutPanel.GetRow(aTQC);
                    RemoveRow(nRowIndex);
                }
                m_nRowIndexTestingQuestionGroup = -1;
                m_lstTestQuestionControls = null;
            }
        }

        // if we insert or remove a row, we have to adjust the following indices
        protected override void InsertRow(int nLayoutRowIndex)
        {
            base.InsertRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors >= nLayoutRowIndex)
                m_nRowIndexAnchors++;
            if (m_nRowIndexRetelling >= nLayoutRowIndex)
                m_nRowIndexRetelling++;
            if (m_nRowIndexTestingQuestionGroup >= nLayoutRowIndex)
                m_nRowIndexTestingQuestionGroup++;
        }

        protected override void RemoveRow(int nLayoutRowIndex)
        {
            base.RemoveRow(nLayoutRowIndex);
            if (m_nRowIndexAnchors > nLayoutRowIndex)
                m_nRowIndexAnchors--;
            if (m_nRowIndexRetelling > nLayoutRowIndex)
                m_nRowIndexRetelling--;
            if (m_nRowIndexTestingQuestionGroup > nLayoutRowIndex)
                m_nRowIndexTestingQuestionGroup--;
        }

        protected void InitStoryLine(StoryEditor aSE, VerseData aVerseData, int nLayoutRow)
        {
            // since some of the view parameters (e.g. show Vernacular) are actually controlled within
            //  the StoryLine control, if we get the call to UpdateView, we have to pass it on to it
            //  to handle (unlike with the Anchor control, which is all on or all off)
            System.Diagnostics.Debug.Assert((m_nRowIndexStoryLine != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameStoryLine));
            if (tableLayoutPanel.Controls.ContainsKey(cstrFieldNameStoryLine))
            {
                StoryLineControl aStoryLineCtrl = (StoryLineControl)tableLayoutPanel.Controls[cstrFieldNameStoryLine];
                aStoryLineCtrl.UpdateView(aSE);
            }
            else
            {
                StoryLineControl aStoryLineCtrl = new StoryLineControl(aSE, aVerseData);
                aStoryLineCtrl.Name = cstrFieldNameStoryLine;
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
            System.Diagnostics.Debug.Assert((m_nRowIndexAnchors != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors));
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameAnchors))
            {
                AnchorControl anAnchorCtrl = new AnchorControl(anAnchorsData);
                anAnchorCtrl.Name = cstrFieldNameAnchors;
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
            System.Diagnostics.Debug.Assert((m_nRowIndexRetelling != -1) || !tableLayoutPanel.Controls.ContainsKey(cstrFieldNameRetellings));
            if (!tableLayoutPanel.Controls.ContainsKey(cstrFieldNameRetellings))
            {
                MultiLineControl aRetellingsCtrl = new MultiLineControl(aRetellingsData);
                aRetellingsCtrl.Name = cstrFieldNameRetellings;
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
            if (m_nRowIndexTestingQuestionGroup == -1)
            {
                if (aTQsData.Count > 0)
                {
                    m_lstTestQuestionControls = new List<TestingQuestionControl>(aTQsData.Count);
                    for (int i = 0; i < aTQsData.Count; i++)
                    {
                        int nTQNumber = i + 1;
                        Label label = new Label();
                        label.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
                        label.AutoSize = true;
                        label.Name = cstrFieldNameTestQuestionsLabel + nTQNumber.ToString();
                        label.Text = String.Format(cstrTestQuestionsLabelFormat, nTQNumber);

                        TestingQuestionControl aTestingQuestionCtrl = new TestingQuestionControl(aSE, aTQsData[i]);
                        aTestingQuestionCtrl.ParentControl = this;
                        aTestingQuestionCtrl.Name = cstrFieldNameTestQuestions + nLayoutRow.ToString();

                        int nRowIndex = nLayoutRow + i;
                        InsertRow(nRowIndex);
                        tableLayoutPanel.Controls.Add(label, 0, nRowIndex);
                        tableLayoutPanel.Controls.Add(aTestingQuestionCtrl, 1, nRowIndex);
                        m_lstTestQuestionControls.Add(aTestingQuestionCtrl);
                    }
                }
            }
        }

        void buttonDragDropHandle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
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
    }
}
