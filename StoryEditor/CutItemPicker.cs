using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class CutItemPicker : TopForm
    {
        private const string CstrNodeTestingQuestions = "TestingQuestions";
        // private const string CstrNodeAnchors = "Anchors";
        private const string CstrNodeCulturalNotes = "CulturalNotes";
        private const string CstrNodeConsultantNotes = "ConsultantNotes";
        private const string CstrNodeCoachNotes = "CoachNotes";

        private VerseData _verseSource;

        public CutItemPicker(VerseData verseSource, VersesData theVerses, 
            StoryEditor theSE, bool bDeleteOnly)
            : base(true)
        {
            InitializeComponent();
            _verseSource = verseSource;
            TheSE = theSE;

            InitializeFromVerse(verseSource);

            if (bDeleteOnly)
            {
                // add the 'delete' button
                AddLineButton("Delete", null, 0, null);

                // start them all out initially as unchecked
                foreach (TreeNode node in treeViewItems.Nodes)
                {
                    node.Checked = false;
                    foreach (TreeNode treeNode in node.Nodes)
                        treeNode.Checked = false;
                }
            }
            else
            {
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
        }

        public CutItemPicker(VerseData verseSource, VerseData verseDest, int nIndex, 
            StoryEditor theSE)
        {
            InitializeComponent();
            _verseSource = verseSource;
            TheSE = theSE;

            InitializeFromVerse(verseSource);

            // now list the verses in the current story
            string strLine = "Ln: " + nIndex;
            string strToolTip = StringForTooltip(verseDest);
            AddLineButton(strLine, strToolTip, nIndex, verseDest);
        }

        public bool IsSomethingToMove { get; set; }

        private void InitializeFromVerse(VerseData verseSource)
        {
            TreeNode nodeItems = treeViewItems.Nodes[CstrNodeTestingQuestions];
            AddTestQuestionNodes(verseSource.TestQuestions, nodeItems);

#if AllowMovingAnchors
            // what does it mean to "move an anchor"... not very useful
            nodeItems = treeViewItems.Nodes[CstrNodeAnchors];
            AddAnchorNodes(verseSource.Anchors, nodeItems);
#endif

            nodeItems = treeViewItems.Nodes[CstrNodeCulturalNotes];
            AddExegeticalHelpNodes(verseSource.ExegeticalHelpNotes, nodeItems);

            nodeItems = treeViewItems.Nodes[CstrNodeConsultantNotes];
            AddConNoteNodes(verseSource.ConsultantNotes, nodeItems);
            
            nodeItems = treeViewItems.Nodes[CstrNodeCoachNotes];
            AddConNoteNodes(verseSource.CoachNotes, nodeItems);

            treeViewItems.ExpandAll();
        }

#if AllowMovingAnchors
        private void AddAnchorNodes(AnchorsData theAnchors, TreeNode nodeItems)
        {
            if (theAnchors.HasData)
            {
                foreach (AnchorData anAnchor in theAnchors)
                {
                    string strPrimary = anAnchor.JumpTarget;
                    string strSecondary = anAnchor.ToolTipText;
                    TreeNode theAnchorNode = nodeItems.Nodes.Add(strPrimary);
                    theAnchorNode.ToolTipText = strSecondary;
                    theAnchorNode.Checked = true;
                    theAnchorNode.Tag = anAnchor;
                }
                IsSomethingToMove = nodeItems.Checked = true;
            }
            else
                nodeItems.Remove();
        }
#endif

        private void AddExegeticalHelpNodes(ExegeticalHelpNotesData theExegHelps, TreeNode nodeItems)
        {
            if (theExegHelps.HasData)
            {
                foreach (ExegeticalHelpNoteData anExegHelp in theExegHelps)
                {
                    string strPrimary = anExegHelp.ToString();
                    TreeNode theExegHelpNode = nodeItems.Nodes.Add(strPrimary);
                    theExegHelpNode.Checked = true;
                    theExegHelpNode.Tag = anExegHelp;
                }
                IsSomethingToMove = nodeItems.Checked = true;
            }
            else
                nodeItems.Remove();
        }

        private void AddTestQuestionNodes(TestQuestionsData theTestQuestions, 
            TreeNode nodeItems)
        {
            if (theTestQuestions.HasData)
            {
                foreach (TestQuestionData aTQ in theTestQuestions)
                {
                    string strPrimary = (aTQ.TestQuestionLine.Vernacular.HasData)
                                            ? aTQ.TestQuestionLine.Vernacular.ToString()
                                            : (aTQ.TestQuestionLine.NationalBt.HasData)
                                                  ? aTQ.TestQuestionLine.NationalBt.ToString()
                                                  : aTQ.TestQuestionLine.InternationalBt.ToString();
                    string strSecondary = (aTQ.TestQuestionLine.InternationalBt.HasData)
                                              ? aTQ.TestQuestionLine.InternationalBt.ToString()
                                              : (aTQ.TestQuestionLine.NationalBt.HasData)
                                                    ? aTQ.TestQuestionLine.NationalBt.ToString()
                                                    : aTQ.TestQuestionLine.Vernacular.ToString();
                    TreeNode theTQnode = nodeItems.Nodes.Add(strPrimary);
                    theTQnode.ToolTipText = strSecondary;
                    theTQnode.Checked = true;
                    theTQnode.Tag = aTQ;
                }
                IsSomethingToMove = nodeItems.Checked = true;
            }
            else
                nodeItems.Remove();
        }

        private static string StringForTooltip(VerseData theVerse)
        {
            string strTooltip = null;
            if (theVerse.StoryLine.Vernacular.HasData)
                strTooltip += theVerse.StoryLine.Vernacular + Environment.NewLine;
            if (theVerse.StoryLine.NationalBt.HasData)
                strTooltip += theVerse.StoryLine.NationalBt + Environment.NewLine;
            if (theVerse.StoryLine.InternationalBt.HasData)
                strTooltip += theVerse.StoryLine.InternationalBt;
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
            {
                // means delete... confirm
                if (MessageBox.Show(Properties.Resources.IDS_ConfirmDeleteItems, 
                    OseResources.Properties.Resources.IDS_Caption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    return;
            }

            // can't do line zero with TQ and other BT pane fields
            bool bLineZero = (btn.TabIndex == 0);
            var nodeItems = treeViewItems.Nodes[CstrNodeTestingQuestions];
            if ((!bLineZero || (verseDest == null)) && (nodeItems != null))
                foreach (var aTQ in
                    from TreeNode node in nodeItems.Nodes 
                    where node.Checked 
                    select node.Tag as TestQuestionData)
                {
                    _verseSource.TestQuestions.Remove(aTQ);
                    if (verseDest != null)  // otherwise, it's just delete
                        verseDest.TestQuestions.Add(aTQ);
                }

#if AllowMovingAnchors
            nodeItems = treeViewItems.Nodes[CstrNodeAnchors];
            if (nodeItems != null)
                foreach (var anAnchor in
                    from TreeNode node in nodeItems.Nodes
                    where node.Checked
                    select node.Tag as AnchorData)
                {
                    _verseSource.Anchors.Remove(anAnchor);
                    if (verseDest != null)  // otherwise, it's just delete
                        verseDest.Anchors.Add(anAnchor);
                }
#endif

            nodeItems = treeViewItems.Nodes[CstrNodeCulturalNotes];
            if ((!bLineZero || (verseDest == null)) && (nodeItems != null))
                foreach (var anExegHelpNote in
                    from TreeNode node in nodeItems.Nodes
                    where node.Checked
                    select node.Tag as ExegeticalHelpNoteData)
                {
                    _verseSource.ExegeticalHelpNotes.Remove(anExegHelpNote);
                    if (verseDest != null)  // otherwise, it's just delete
                        verseDest.ExegeticalHelpNotes.Add(anExegHelpNote);
                }

            nodeItems = treeViewItems.Nodes[CstrNodeConsultantNotes];
            if (nodeItems != null)
                foreach (var aConNote in 
                    from TreeNode node in nodeItems.Nodes
                    where node.Checked
                    select node.Tag as ConsultNoteDataConverter)
                {
                    _verseSource.ConsultantNotes.Remove(aConNote);
                    if (verseDest != null)  // otherwise, it's just delete
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
                    if (verseDest != null)  // otherwise, it's just delete
                        verseDest.CoachNotes.Add(aConNote);
                }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void AddConNoteNodes(IEnumerable<ConsultNoteDataConverter> theConNotes, TreeNode nodeItems)
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
                IsSomethingToMove = nodeItems.Checked = true;
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

            if (e.Button == MouseButtons.Right)
            {
                var node = e.Node;
                if (node.Tag is ConsultNoteDataConverter)
                {
                    var aConNote = node.Tag as ConsultNoteDataConverter;
                    if (_toolTip == null)
                        _toolTip = new MoveConNoteTooltip();

                    _toolTip.SetDocumentText(aConNote, TheSE, Cursor.Position);
                    _toolTip.Show();
                }
            }
            else if ((e.Node == treeViewItems.Nodes[CstrNodeTestingQuestions])
                || (e.Node == treeViewItems.Nodes[CstrNodeCulturalNotes])
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
            if (parent == null)
                return;

            if (parent.Nodes.Cast<TreeNode>().Any(node => !node.Checked))
                return;
            parent.Checked = true;
        }

        private static void SetChecks(TreeNode nodeParent)
        {
            foreach (TreeNode child in nodeParent.Nodes)
                child.Checked = nodeParent.Checked;
        }

        private StoryEditor TheSE;
        private MoveConNoteTooltip _toolTip;
    }

    class MyTreeView : TreeView
    {
        private const int WM_LBUTTONDBLCLK = 0x203;

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
                return;

            base.DefWndProc(ref m);
        }
    }
}
