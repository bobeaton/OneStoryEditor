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
    public partial class CutItemPicker : Form
    {
        private const string CstrNodeTestingQuestions = "TestingQuestions";
        private const string CstrNodeConsultantNotes = "ConsultantNotes";
        private const string CstrNodeCoachNotes = "CoachNotes";

        private VerseData _verseSource;
        private VerseData _verseDest;
        private VersesData _theVerses;

        public CutItemPicker(VerseData verseSource, VersesData theVerses)
        {
            InitializeComponent();
            _verseSource = verseSource;
            _theVerses = theVerses;

            InitializeFromVerse(verseSource);

            // now list the verses in the current story
            string strLine = "Story (Ln 0)";
            AddLineButton(strLine, null, 0, theVerses.FirstVerse);
            for (int i = 1; i <= theVerses.Count; i++)
            {
                VerseData aVerseData = theVerses[i - 1];
                if (aVerseData == _verseSource)
                    continue;

                strLine = "Ln: " + i;
                string strToolTip = StringForTooltip(aVerseData);
                AddLineButton(strLine, strToolTip, i, aVerseData);
            }
        }

        public CutItemPicker(VerseData verseSource, VerseData verseDest, int nIndex)
        {
            InitializeComponent();
            _verseSource = verseSource;
            _verseDest = verseDest;

            InitializeFromVerse(verseSource);

            // now list the verses in the current story
            string strLine = "Ln: " + nIndex;
            string strToolTip = StringForTooltip(verseDest);
            AddLineButton(strLine, strToolTip, nIndex, verseDest);
        }

        private void InitializeFromVerse(VerseData verseSource)
        {
            TreeNode nodeItems = treeViewItems.Nodes[CstrNodeTestingQuestions];
            if (verseSource.TestQuestions.HasData)
            {
                foreach (TestQuestionData aTQ in verseSource.TestQuestions)
                {
                    string strPrimary = (aTQ.QuestionVernacular.HasData)
                                            ? aTQ.QuestionVernacular.ToString()
                                            : (aTQ.QuestionNationalBT.HasData)
                                                  ? aTQ.QuestionNationalBT.ToString()
                                                  : aTQ.QuestionInternationalBT.ToString();
                    string strSecondary = (aTQ.QuestionInternationalBT.HasData)
                                              ? aTQ.QuestionInternationalBT.ToString()
                                              : (aTQ.QuestionNationalBT.HasData)
                                                    ? aTQ.QuestionNationalBT.ToString()
                                                    : aTQ.QuestionVernacular.ToString();
                    TreeNode theTQnode = nodeItems.Nodes.Add(strPrimary);
                    theTQnode.ToolTipText = strSecondary;
                    theTQnode.Checked = true;
                    theTQnode.Tag = aTQ;
                }
                nodeItems.Checked = true;
            }
            else
                nodeItems.Remove();

            nodeItems = treeViewItems.Nodes[CstrNodeConsultantNotes];
            AddConNoteNodes(verseSource.ConsultantNotes, nodeItems);
            
            nodeItems = treeViewItems.Nodes[CstrNodeCoachNotes];
            AddConNoteNodes(verseSource.CoachNotes, nodeItems);

            treeViewItems.ExpandAll();
        }

        private static string StringForTooltip(VerseData theVerse)
        {
            string strTooltip = null;
            if (theVerse.VernacularText.HasData)
                strTooltip += theVerse.VernacularText + Environment.NewLine;
            if (theVerse.NationalBTText.HasData)
                strTooltip += theVerse.NationalBTText + Environment.NewLine;
            if (theVerse.InternationalBTText.HasData)
                strTooltip += theVerse.InternationalBTText;
            return strTooltip;
        }

        private void AddLineButton(string strLine, string strToolTip, int nIndex, 
            VerseData theVerse)
        {
            var btnLine = new Button
                              {
                                  Name = "LineButton" + nIndex,
                                  Size = new Size(75, 23),
                                  TabIndex = nIndex,
                                  Text = strLine,
                                  UseVisualStyleBackColor = true,
                                  Tag = theVerse
                              };

            btnLine.Click += btnLine_Click;
            toolTip.SetToolTip(btnLine, strToolTip);
            flowLayoutPanelLines.Controls.Add(btnLine);
        }

        void btnLine_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) 
                return;

            var verseDest = btn.Tag as VerseData;
            if (verseDest == null)
                return;

            var nodeItems = treeViewItems.Nodes[CstrNodeTestingQuestions];
            if (nodeItems != null)
                foreach (var aTQ in
                    from TreeNode node in nodeItems.Nodes 
                    where node.Checked 
                    select node.Tag as TestQuestionData)
                {
                    _verseSource.TestQuestions.Remove(aTQ);
                    verseDest.TestQuestions.Add(aTQ);
                }

            nodeItems = treeViewItems.Nodes[CstrNodeConsultantNotes];
            if (nodeItems != null)
                foreach (var aConNote in 
                    from TreeNode node in nodeItems.Nodes
                    where node.Checked
                    select node.Tag as ConsultNoteDataConverter)
                {
                    _verseSource.ConsultantNotes.Remove(aConNote);
                    verseDest.ConsultantNotes.Add(aConNote);
                }

            nodeItems = treeViewItems.Nodes[CstrNodeCoachNotes];
            if (nodeItems != null)
                foreach (var aConNote in
                    from TreeNode node in nodeItems.Nodes
                    where node.Checked
                    select node.Tag as ConsultNoteDataConverter)
                {
                    _verseSource.CoachNotes.Remove(aConNote);
                    verseDest.CoachNotes.Add(aConNote);
                }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static void AddConNoteNodes(IEnumerable<ConsultNoteDataConverter> theConNotes, TreeNode nodeItems)
        {
            if (theConNotes.Count() > 0)
            {
                foreach (ConsultNoteDataConverter aConNote in theConNotes)
                {
                    string str = aConNote[0].ToString();
                    int len = str.IndexOf(Environment.NewLine);
                    if (len == -1)
                        len = Math.Min(50, str.Length);
                    string strPrimary = str.Substring(0, len);
                    TreeNode theConNoteNode = nodeItems.Nodes.Add(strPrimary);
                    theConNoteNode.Checked = true;
                    theConNoteNode.Tag = aConNote;
                }
                nodeItems.Checked = true;
            }
            else
                nodeItems.Remove();
        }

        readonly protected DateTime m_dtStarted = DateTime.Now;
        readonly TimeSpan m_timeMinStartup = new TimeSpan(0, 0, 1);
        private void treeViewItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // prevent the false click that occurs when the user chooses a menu item
            if ((DateTime.Now - m_dtStarted) < m_timeMinStartup)
                return;

            if ((e.Node == treeViewItems.Nodes[CstrNodeTestingQuestions])
                || (e.Node == treeViewItems.Nodes[CstrNodeConsultantNotes])
                || (e.Node == treeViewItems.Nodes[CstrNodeCoachNotes]))
            {
                SetChecks(e.Node);
            }
            else if (e.Node.Checked)
            {
                CheckForAllCheck(e.Node);
            }
            else
                e.Node.Parent.Checked = false;
        }

        private static void CheckForAllCheck(TreeNode nodeLeaf)
        {
            var parent = nodeLeaf.Parent;
            if (parent.Nodes.Cast<TreeNode>().Any(node => !node.Checked))
                return;
            parent.Checked = true;
        }

        private static void SetChecks(TreeNode nodeParent)
        {
            foreach (TreeNode child in nodeParent.Nodes)
                child.Checked = nodeParent.Checked;
        }
    }
}
