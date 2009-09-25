using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class AnchorData
    {
        public string JumpTarget = null;
        public string ToolTipText = null;
        public ExegeticalHelpNotesData ExegeticalHelpNotes = null;
        public List<string> keyTerms = new List<string>();
        public AnchorData(StoryProject.anchorRow theAnchorRow, StoryProject projFile)
        {
            JumpTarget = theAnchorRow.jumpTarget;
            if (!theAnchorRow.IstoolTipNull())
                ToolTipText = theAnchorRow.toolTip;

            ExegeticalHelpNotes = new ExegeticalHelpNotesData(theAnchorRow, projFile);

            StoryProject.keyTermsRow[] thekeyTermsRows = theAnchorRow.GetkeyTermsRows();
            if (thekeyTermsRows.Length > 0)
            {
                StoryProject.keyTermsRow thekeyTermsRow = thekeyTermsRows[0];
                foreach (StoryProject.keyTermRow aKeyTermRow in thekeyTermsRow.GetkeyTermRows())
                    keyTerms.Add(aKeyTermRow.keyTerm_Column);
            }
        }

        public AnchorData(string strJumpTarget, string strComment)
        {
            JumpTarget = strJumpTarget;
            ToolTipText = strComment;
            ExegeticalHelpNotes = new ExegeticalHelpNotesData();
        }

        public void AddKeyTerm(string strKeyTerm)
        {
            if (!keyTerms.Contains(strKeyTerm))
                keyTerms.Add(strKeyTerm);
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));
                XElement elemAnchor = new XElement("anchor", new XAttribute("jumpTarget", JumpTarget),
                    new XElement("toolTip", ToolTipText));
                if (ExegeticalHelpNotes.HasData)
                    elemAnchor.Add(ExegeticalHelpNotes.GetXml);

                if (keyTerms.Count > 0)
                {
                    XElement elemKeyTerms = new XElement("keyTerms");
                    foreach (string strKeyTerm in keyTerms)
                        elemKeyTerms.Add(new XElement("keyTerm", strKeyTerm));
                    elemAnchor.Add(elemKeyTerms);
                }
                
                return elemAnchor;
            }
        }
    }

    public class AnchorsData : List<AnchorData>
    {
        public AnchorsData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            StoryProject.anchorsRow[] theAnchorsRows = theVerseRow.GetanchorsRows();
            StoryProject.anchorsRow theAnchorsRow;
            if (theAnchorsRows.Length == 0)
                theAnchorsRow = projFile.anchors.AddanchorsRow(theVerseRow);
            else
                theAnchorsRow = theAnchorsRows[0];

            foreach (StoryProject.anchorRow anAnchorRow in theAnchorsRow.GetanchorRows())
                Add(new AnchorData(anAnchorRow, projFile));
        }

        public AnchorsData()
        {
        }

        public AnchorData AddAnchorData(string strJumpTarget, string strComment)
        {
            AnchorData anAD = new AnchorData(strJumpTarget, strComment);
            Add(anAD);
            return anAD;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an AnchorsData without items");
                XElement elemAnchors = new XElement("anchors");
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }
    }
}
