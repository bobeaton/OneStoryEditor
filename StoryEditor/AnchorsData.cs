using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;   // for Regex
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class AnchorData
    {
        public string JumpTarget = null;
        public string ToolTipText = null;
        public ExegeticalHelpNotesData ExegeticalHelpNotes = null;
        public List<string> keyTerms = new List<string>();

        public AnchorData(NewDataSet.anchorRow theAnchorRow, NewDataSet projFile)
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

        public AnchorData(AnchorData rhs)
        {
            JumpTarget = rhs.JumpTarget;
            ToolTipText = rhs.ToolTipText;
            ExegeticalHelpNotes = new ExegeticalHelpNotesData(rhs.ExegeticalHelpNotes);
            foreach (string str in rhs.keyTerms)
                keyTerms.Add(str);
        }

        // match for things like "2Cor" and turn it into "2Co"
        protected Regex regexParsingAnchor = new Regex(@"^(.{3})[^ ]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
                
        public string AnchorAsVerseRef
        {
            get
            {
                // some things which work as anchors for Sword don't work for the Paratext
                //  key terms stuff as "VerseRef" objects
                string strVerseRef = regexParsingAnchor.Replace(JumpTarget, "$1");
                return strVerseRef;
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));
                XElement elemAnchor = new XElement("anchor", 
                    new XAttribute("jumpTarget", JumpTarget),
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
        public bool IsKeyTermChecked = false;
        public AnchorsData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
        {
            NewDataSet.anchorsRow[] theAnchorsRows = theVerseRow.GetanchorsRows();
            NewDataSet.anchorsRow theAnchorsRow;
            if (theAnchorsRows.Length == 0)
                theAnchorsRow = projFile.anchors.AddanchorsRow(false, theVerseRow);
            else
                theAnchorsRow = theAnchorsRows[0];

            if (!theAnchorsRow.IskeyTermCheckedNull())
                IsKeyTermChecked = theAnchorsRow.keyTermChecked;

            foreach (NewDataSet.anchorRow anAnchorRow in theAnchorsRow.GetanchorRows())
                Add(new AnchorData(anAnchorRow, projFile));
        }

        public AnchorsData(AnchorsData rhs)
        {
            IsKeyTermChecked = rhs.IsKeyTermChecked;
            foreach (AnchorData aAnchor in rhs)
                Add(new AnchorData(aAnchor));
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
            get { return (Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an AnchorsData without items");
                XElement elemAnchors = new XElement("anchors", new XAttribute("keyTermChecked", IsKeyTermChecked));
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }
    }
}
