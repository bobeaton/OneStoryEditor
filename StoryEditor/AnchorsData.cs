using System;
using System.Collections.Generic;
using System.Xml;
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
        public List<string> keyTerms = new List<string>();

        public AnchorData(NewDataSet.AnchorRow theAnchorRow, NewDataSet projFile)
        {
            JumpTarget = theAnchorRow.jumpTarget;
            ToolTipText = (theAnchorRow.IsAnchor_textNull()) 
                ? JumpTarget 
                : theAnchorRow.Anchor_text;
        }

        public AnchorData(XmlNode node)
        {
            XmlAttribute attr;
            JumpTarget = ((attr = node.Attributes[CstrAttributeJumpTarget]) != null) ? attr.Value : null;
            // ToolTipText = ((attr = node.Attributes[CstrAttributeToolTip]) != null) ? attr.Value : null;
            ToolTipText = node.InnerText;
        }

        public AnchorData(string strJumpTarget, string strComment)
        {
            JumpTarget = strJumpTarget;
            ToolTipText = strComment;
        }

        public AnchorData(AnchorData rhs)
        {
            JumpTarget = rhs.JumpTarget;
            ToolTipText = rhs.ToolTipText;
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
                        String.Format(OseResources.Properties.Resources.IDS_IllFormedJumpTarget, strBookCode));
                }
                
                return strBookCode + JumpTarget.Substring(nIndexSpace);
            }
        }

        public const string CstrElementLabelAnchor = "Anchor";
        public const string CstrAttributeJumpTarget = "jumpTarget";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));

                var elemAnchor = new XElement(CstrElementLabelAnchor,
                                              new XAttribute(CstrAttributeJumpTarget, JumpTarget));
                                              
                if (!String.IsNullOrEmpty(ToolTipText))
                    elemAnchor.Add(ToolTipText);

                return elemAnchor;
            }
        }

        public const string CstrTooltipIndicator = " *";

        public string Html
        {
            get 
            {
                string strButtonLabel = JumpTarget;
                if (!String.IsNullOrEmpty(ToolTipText)
                    && (JumpTarget != ToolTipText))
                    strButtonLabel += CstrTooltipIndicator; // give an indication that there's a tooltip

                return String.Format(OseResources.Properties.Resources.HTML_Button,
                                        JumpTarget,
                                        "return OnBibRefJump(this);",
                                        strButtonLabel);
            }
        }

        public string PresentationHtml(AnchorsData childAnchorsData, bool bPrintPreview,
            bool bProcessingTheChild, ref List<string> astrExegeticalHelpNotes)
        {
            string strButtonLabel = JumpTarget;
            if (childAnchorsData != null)
            {
                bool bFound = false;
                string strToolTipText = null;
                foreach (AnchorData anAnchor in childAnchorsData)
                    if (JumpTarget == anAnchor.JumpTarget)
                    {
                        bFound = true;
                        strToolTipText = anAnchor.ToolTipText;
                        childAnchorsData.Remove(anAnchor);   // so we know which ones we've "done"
                        break;
                    }

                // if we didn't find it, then
                if (!bFound)
                {
                    strButtonLabel = Diff.HtmlDiff(JumpTarget, null, true);   // show as deleted
                    if (ToolTipText != JumpTarget)
                    {
                        strToolTipText = Diff.HtmlDiff(ToolTipText, null, true);
                        strButtonLabel += CstrTooltipIndicator;
                        astrExegeticalHelpNotes.Add(strToolTipText);
                    }
                }
                else
                {
                    // otherwise, if we did find it, see if there's any difference to the tooltip
                    strToolTipText = Diff.HtmlDiff(ToolTipText, strToolTipText, true);
                    if (strToolTipText != JumpTarget)
                    {
                        strButtonLabel += CstrTooltipIndicator;
                        astrExegeticalHelpNotes.Add(strToolTipText);
                    }
                }
            }
            else if (!bPrintPreview && !bProcessingTheChild)
            {
                // this means a) we're processing with children (i.e. 2 way merge rather than just print
                //  preview), b) but we don't *have* a child (because the original if case failed), and
                //  c) but we're processing the parent record.
                // So this means we're processing the parent which has no child, thus: deletion:
                strButtonLabel = Diff.HtmlDiff(JumpTarget, null, true);
                if (ToolTipText != JumpTarget)
                {
                    strButtonLabel += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(ToolTipText, null, true));
                }
            }
            else if (!bPrintPreview)
            {
                // the only time we call this method without a value for childAnchorsData is when
                //  we're processing the new stuff (i.e. what's in the child)
                //  so show it with 'addition' markup
                strButtonLabel = Diff.HtmlDiff(null, JumpTarget, true);
                if (JumpTarget != ToolTipText)
                {
                    strButtonLabel += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(null, ToolTipText, true));
                }
            }
            else
            {
                // this means we're doing print preview, so just give the value without markup
                strButtonLabel = JumpTarget;
                if (JumpTarget != ToolTipText)
                {
                    strButtonLabel += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(ToolTipText);
                }
            }

            return String.Format(OseResources.Properties.Resources.HTML_Button,
                                    JumpTarget,
                                    "return OnBibRefJump(this);",
                                    strButtonLabel);
        }

        public string PresentationHtmlAsAddition(ref List<string> astrExegeticalHelpNotes)
        {
            string strButtonLabel = JumpTarget;
            // the only time we call this method without a value for childAnchorsData is when
            //  we're processing the new stuff (i.e. what's in the child)
            //  so show it with 'addition' markup
            strButtonLabel = Diff.HtmlDiff(null, JumpTarget, true);
            if (JumpTarget != ToolTipText)
            {
                strButtonLabel += CstrTooltipIndicator;
                astrExegeticalHelpNotes.Add(Diff.HtmlDiff(null, ToolTipText, true));
            }

            return String.Format(OseResources.Properties.Resources.HTML_Button,
                                    JumpTarget,
                                    "return OnBibRefJump(this);",
                                    strButtonLabel);
        }
    }

    public class AnchorsData : List<AnchorData>
    {
        public bool IsKeyTermChecked = false;
        public AnchorsData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
        {
            NewDataSet.AnchorsRow[] theAnchorsRows = theVerseRow.GetAnchorsRows();
            NewDataSet.AnchorsRow theAnchorsRow;
            if (theAnchorsRows.Length == 0)
                theAnchorsRow = projFile.Anchors.AddAnchorsRow(false, theVerseRow);
            else
                theAnchorsRow = theAnchorsRows[0];

            if (!theAnchorsRow.IskeyTermCheckedNull())
                IsKeyTermChecked = theAnchorsRow.keyTermChecked;

            foreach (NewDataSet.AnchorRow anAnchorRow in theAnchorsRow.GetAnchorRows())
                Add(new AnchorData(anAnchorRow, projFile));
        }

        public AnchorsData(XmlNode node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(AnchorData.CstrElementLabelAnchor);
            if (list == null)
                return;

            foreach (XmlNode nodeAnchor in list)
                Add(new AnchorData(nodeAnchor));
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

        public const string CstrElementLabelAnchors = "Anchors";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an AnchorsData without items");
                XElement elemAnchors = new XElement(CstrElementLabelAnchors, new XAttribute("keyTermChecked", IsKeyTermChecked));
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }

        /* don't thinks this is used (and it doesn't work)
        public string Html(int nVerseIndex, int nNumCols)
        {
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.Html;

            // make a cell out of the buttons
            string strHtmlCell = String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                               100, 
                                               strRow);

            // add combine with the 'anc:' header cell into a Table Row
            string strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                           String.Format("{0}{1}",
                                                         String.Format(OseResources.Properties.Resources.HTML_TableCell,
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
            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strHtml)));

            return strHtml;
        }
        */

        public string PresentationHtml(int nVerseIndex, int nNumCols, 
            AnchorsData childAnchorsData, 
            ExegeticalHelpNotesData theExegeticalHelpNotes,
            ExegeticalHelpNotesData theChildExegeticalHelpNotes,
            bool bPrintPreview)
        {
            List<string> astrExegeticalHelpNotes = new List<string>();
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.PresentationHtml(childAnchorsData, bPrintPreview, false, ref astrExegeticalHelpNotes);

            // get the ExegeticalHelp notes as well
            theExegeticalHelpNotes.PresentationHtml(theChildExegeticalHelpNotes, ref astrExegeticalHelpNotes);

            // now put the anchors that are in the child (as additions)
            if (childAnchorsData != null)
                foreach (AnchorData anchorData in childAnchorsData)
                    strRow += anchorData.PresentationHtmlAsAddition(ref astrExegeticalHelpNotes);

            if (theChildExegeticalHelpNotes != null)
                theChildExegeticalHelpNotes.PresentationHtml(null, ref astrExegeticalHelpNotes);

            return FinishPresentationHtml(nVerseIndex, nNumCols, strRow, astrExegeticalHelpNotes);
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols)
        {
            List<string> astrExegeticalHelpNotes = new List<string>();
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.PresentationHtmlAsAddition(ref astrExegeticalHelpNotes);

            return FinishPresentationHtml(nVerseIndex, nNumCols, strRow, astrExegeticalHelpNotes);
        }

        protected string FinishPresentationHtml(int nVerseIndex, int nNumCols, 
            string strRow, List<string> astrExegeticalHelpNotes)
        {
            // stop if there was nothing
            if (String.IsNullOrEmpty(strRow))
                return strRow;

            // make a cell out of the buttons
            string strHtmlCell = String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                               100,
                                               strRow);

            // add combine with the 'anc:' header cell into a Table Row
            string strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                           String.Format("{0}{1}",
                                                         String.Format(OseResources.Properties.Resources.HTML_TableCell,
                                                                       "anc:"),
                                                         strHtmlCell));

            // add exegetical comments as their own rows
            for (int i = 0; i < astrExegeticalHelpNotes.Count; i++)
            {
                string strExegeticalHelpNote = astrExegeticalHelpNotes[i];
                string strHtmlElementId = String.Format("paragraphExHelp{0}_{1}", nVerseIndex, i);
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                         String.Format("{0}{1}",
                                                       String.Format(OseResources.Properties.Resources.HTML_TableCell, "cn:"),
                                                       String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                                     100,
                                                                     String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                                                   strHtmlElementId,
                                                                                   StoryData.
                                                                                       CstrLangInternationalBtStyleClassName,
                                                                                   strExegeticalHelpNote))));
            }

            // make a sub-table out of all this
            strHtml = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strHtml)));

            return strHtml;
        }
    }
}
