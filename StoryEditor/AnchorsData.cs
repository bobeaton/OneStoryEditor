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

        internal static readonly Dictionary<string, string> mapSwordToParatextBookCodes = new Dictionary<string, string>()
                                                                     {
{ "Gen", "Gen" },
{ "Exod", "Exo" },
{ "Lev", "Lev" },
{ "Num", "Num" },
{ "Deut", "Deu" },
{ "Josh", "Jos" },
{ "Judg", "Jdg" },
{ "Ruth", "Rut" },
{ "1Sam", "1Sa" },
{ "2Sam", "2Sa" },
{ "1Kgs", "1Ki" },
{ "2Kgs", "2Ki" },
{ "1Chr", "1Ch" },
{ "2Chr", "2Ch" },
{ "Ezra", "Ezr" },
{ "Neh", "Neh" },
{ "Esth", "Est" },
{ "Job", "Job" },
{ "Ps", "Psa" },
{ "Prov", "Pro" },
{ "Eccl", "Ecc" },
{ "Song", "Sng" },
{ "Isa", "Isa" },
{ "Jer", "Jer" },
{ "Lam", "Lam" },
{ "Ezek", "Ezk" },
{ "Dan", "Dan" },
{ "Hos", "Hos" },
{ "Joel", "Jol" },
{ "Amos", "Amo" },
{ "Obad", "Oba" },
{ "Jonah", "Jon" },
{ "Mic", "Mic" },
{ "Nah", "Nam" },
{ "Hab", "Hab" },
{ "Zeph", "Zep" },
{ "Hag", "Hag" },
{ "Zech", "Zec" },
{ "Mal", "Mal" },
{ "Matt", "Mat" },
{ "Mark", "Mrk" },
{ "Luke", "Luk" },
{ "John", "Jhn" },
{ "Acts", "Act" },
{ "Rom", "Rom" },
{ "1Cor", "1Co" },
{ "2Cor", "2Co" },
{ "Gal", "Gal" },
{ "Eph", "Eph" },
{ "Phil", "Php" },
{ "Col", "Col" },
{ "1Thess", "1Th" },
{ "2Thess", "2Th" },
{ "1Tim", "1Ti" },
{ "2Tim", "2Ti" },
{ "Titus", "Tit" },
{ "Phlm", "Phm" },
{ "Heb", "Heb" },
{ "Jas", "Jas" },
{ "1Pet", "1Pe" },
{ "2Pet", "2Pe" },
{ "1John", "1Jn" },
{ "2John", "2Jn" },
{ "3John", "3Jn" },
{ "Jude", "Jud" },
{ "Rev",  "Rev" }
                                                                     };
        public string AnchorAsVerseRef
        {
            get
            {
                int nIndexSpace = JumpTarget.IndexOf(' ');
                string strBookCode = JumpTarget.Substring(0, nIndexSpace);
                try
                {
                    strBookCode = mapSwordToParatextBookCodes[strBookCode];
                }
                catch (Exception)
                {
                    throw new ApplicationException(
                        String.Format(Properties.Resources.IDS_IllFormedJumpTarget, strBookCode));
                }
                
                return strBookCode + JumpTarget.Substring(nIndexSpace);
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));
                XElement elemAnchor = new XElement("anchor", 
                    new XAttribute("jumpTarget", JumpTarget));

                if (!String.IsNullOrEmpty(ToolTipText))
                    elemAnchor.Add(new XElement("toolTip", ToolTipText));

                if (ExegeticalHelpNotes.HasData)
                    elemAnchor.Add(ExegeticalHelpNotes.GetXml);

                /* not using anymore
                if (keyTerms.Count > 0)
                {
                    XElement elemKeyTerms = new XElement("keyTerms");
                    foreach (string strKeyTerm in keyTerms)
                        elemKeyTerms.Add(new XElement("keyTerm", strKeyTerm));
                    elemAnchor.Add(elemKeyTerms);
                }
                */

                return elemAnchor;
            }
        }

        protected const string CstrTooltipIndicator = " *";

        public string Html
        {
            get 
            {
                string strButtonLabel = JumpTarget;
                if (!String.IsNullOrEmpty(ToolTipText)
                    && (JumpTarget != ToolTipText))
                    strButtonLabel += CstrTooltipIndicator; // give an indication that there's a tooltip

                return String.Format(Properties.Resources.HTML_Button,
                                        JumpTarget,
                                        "return OnBibRefJump(this);",
                                        strButtonLabel);
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

        public string Html(int nVerseIndex, int nNumCols)
        {
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.Html;

            // make a cell out of the buttons
            string strHtmlCell = String.Format(Properties.Resources.HTML_TableCellWidth,
                                               100, 
                                               strRow);

            // add combine with the 'anc:' header cell into a Table Row
            string strHtml = String.Format(Properties.Resources.HTML_TableRow,
                                           String.Format("{0}{1}",
                                                         String.Format(Properties.Resources.HTML_TableCell,
                                                                       "anc:"),
                                                         strHtmlCell));

            // add exegetical comments as their own rows
            for (int i = 0; i < Count; i++)
            {
                AnchorData anchorData = this[i];
                if (anchorData.ExegeticalHelpNotes.Count > 0)
                    strHtml += anchorData.ExegeticalHelpNotes.Html(nVerseIndex, i);
            }

            // make a sub-table out of all this
            strHtml = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_TableNoBorder,
                                                                strHtml)));

            return strHtml;
        }
    }
}
