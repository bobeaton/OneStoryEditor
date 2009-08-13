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
        internal ExegeticalHelpNotesData ExegeticalHelpNotes = null;

        public AnchorData(StoryProject.anchorRow theAnchorRow, StoryProject projFile)
        {
            JumpTarget = theAnchorRow.jumpTarget;
            if (!theAnchorRow.IstoolTipNull())
                ToolTipText = theAnchorRow.toolTip;

            ExegeticalHelpNotes = new ExegeticalHelpNotesData(theAnchorRow, projFile);
        }

        public AnchorData(string strJumpTarget, string strComment)
        {
            JumpTarget = strJumpTarget;
            ToolTipText = strComment;
            ExegeticalHelpNotes = new ExegeticalHelpNotesData();
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));
                XElement elemAnchor = new XElement(StoriesData.ns + "anchor", new XAttribute("jumpTarget", JumpTarget),
                    new XElement(StoriesData.ns + "toolTip", ToolTipText));
                if (ExegeticalHelpNotes.HasData)
                    elemAnchor.Add(ExegeticalHelpNotes.GetXml);
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

        public AnchorData AddAnchorData(string strJumpTarget)
        {
            AnchorData anAD = new AnchorData(strJumpTarget, strJumpTarget);
            this.Add(anAD);
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
                XElement elemAnchors = new XElement(StoriesData.ns + "anchors");
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }
    }
}
