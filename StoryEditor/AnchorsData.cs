﻿using System;
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
                return new XElement(StoryEditor.ns + "anchor", new XAttribute("jumpTarget", JumpTarget),
                    new XElement(StoryEditor.ns + "toolTip", ToolTipText),
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

        public AnchorData AddAnchorData(string strJumpTarget)
        {
            AnchorData anAD = new AnchorData(strJumpTarget, strJumpTarget);
            this.Add(anAD);
            return anAD;
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
