using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class AnchorData
    {
        public string JumpTarget = null;
        public string ToolTip = null;
        public ExegeticalHelpNotesData ExegeticalHelpNotes = null;

        public AnchorData(StoryProject.anchorRow theAnchorRow, StoryProject projFile)
        {
            JumpTarget = theAnchorRow.jumpTarget;
            if (!theAnchorRow.IstoolTipNull())
                ToolTip = theAnchorRow.toolTip;

            ExegeticalHelpNotes = new ExegeticalHelpNotesData(theAnchorRow, projFile);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "anchor", new XAttribute("jumpTarget", JumpTarget),
                    new XElement(StoryEditor.ns + "toolTip", ToolTip),
                    ExegeticalHelpNotes.GetXml);
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

        public XElement GetXml
        {
            get
            {
                XElement elemAnchors = new XElement(StoryEditor.ns + "anchors");
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }
    }
}
