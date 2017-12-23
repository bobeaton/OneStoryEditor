using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    public class AnchorData
    {
        public string JumpTarget = null;
        public string ToolTipText = null;
        public List<string> keyTerms = new List<string>();

        public AnchorData(NewDataSet.AnchorRow theAnchorRow)
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
                        String.Format(Properties.Resources.IDS_IllFormedJumpTarget, strBookCode));
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

        /*
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
        */

        public string PresentationHtml(int nLineIndex, AnchorsData childAnchorsData,
            StoryData.PresentationType presentationType, bool bProcessingTheChild, 
            ref List<string> astrExegeticalHelpNotes)
        {
            string strButtonLabelLocalized = NetBibleViewer.CheckForLocalization(JumpTarget);
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
                    strButtonLabelLocalized = Diff.HtmlDiff(strButtonLabelLocalized, null, true);   // show as deleted
                    if (ToolTipText != JumpTarget)
                    {
                        strToolTipText = Diff.HtmlDiff(ToolTipText, null, true);
                        strButtonLabelLocalized += CstrTooltipIndicator;
                        astrExegeticalHelpNotes.Add(strToolTipText);
                    }
                }
                else
                {
                    // otherwise, if we did find it, see if there's any difference to the tooltip
                    strToolTipText = Diff.HtmlDiff(ToolTipText, strToolTipText, true);
                    if (strToolTipText != JumpTarget)
                    {
                        strButtonLabelLocalized += CstrTooltipIndicator;
                        astrExegeticalHelpNotes.Add(strToolTipText);
                    }
                }
            }
            else if ((presentationType == StoryData.PresentationType.Differencing) && !bProcessingTheChild)
            {
                // this means a) we're processing with children (i.e. 2 way merge rather than just print
                //  preview), b) but we don't *have* a child (because the original if case failed), and
                //  c) but we're processing the parent record.
                // So this means we're processing the parent which has no child, thus: deletion:
                strButtonLabelLocalized = Diff.HtmlDiff(strButtonLabelLocalized, null, true);
                if (ToolTipText != JumpTarget)
                {
                    strButtonLabelLocalized += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(ToolTipText, null, true));
                }
            }
            else if (presentationType == StoryData.PresentationType.Differencing)
            {
                // the only time we call this method without a value for childAnchorsData is when
                //  we're processing the new stuff (i.e. what's in the child)
                //  so show it with 'addition' markup
                strButtonLabelLocalized = Diff.HtmlDiff(null, strButtonLabelLocalized, true);
                if (JumpTarget != ToolTipText)
                {
                    strButtonLabelLocalized += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(null, ToolTipText, true));
                }
            }
            else
            {
                // this means we're doing print preview, so just give the value without markup
                if (JumpTarget != ToolTipText)
                {
                    strButtonLabelLocalized += CstrTooltipIndicator;
                    astrExegeticalHelpNotes.Add(ToolTipText);
                }
            }

            return GetAnchorButtonHtml(nLineIndex, strButtonLabelLocalized);
        }

        public string PresentationHtmlAsAddition(int nLineIndex, ref List<string> astrExegeticalHelpNotes)
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

            return GetAnchorButtonHtml(nLineIndex, strButtonLabel);
        }

        public const string CstrButtonPrefixAnchorBar = "anc";
        public const string CstrButtonPrefixAnchorButton = "btnAnc";
        public static string ButtonId(int nLineIndex, string strJumpTarget)
        {
            return String.Format("{0}_{1}_{2}", CstrButtonPrefixAnchorButton, nLineIndex,
                                 strJumpTarget.Replace(" ", null));
        }

        private string GetAnchorButtonHtml(int nLineIndex, string strButtonLabel)
        {
            return String.Format(Properties.Resources.HTML_ButtonToolTip,
                                 ButtonId(nLineIndex, JumpTarget),
                                 JumpTarget,
                                 "return OnBibRefJump(this);",
                                 System.Web.HttpUtility.HtmlEncode(ToolTipText),
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
                Add(new AnchorData(anAnchorRow));
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
            foreach (var anAnchorData in this.Where(anAnchorData => 
                anAnchorData.JumpTarget == strJumpTarget))
            {
                return anAnchorData;
            }
            var anAD = new AnchorData(strJumpTarget, strComment);
            Add(anAD);
            return anAD;
        }

        public bool Contains(string strJumpTarget)
        {
            return this.Any(anAnchorData => 
                anAnchorData.JumpTarget == strJumpTarget);
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

        public string PresentationHtml(int nVerseIndex,
            AnchorsData childAnchorsData, StoryData.PresentationType presentationType, 
            ref List<string> astrExegeticalHelpNotes)
        {
            string strHtmlCell = GetAnchorButtonsCell(nVerseIndex,
                                                      childAnchorsData,
                                                      presentationType,
                                                      ref astrExegeticalHelpNotes);

            return FinishPresentationHtml(nVerseIndex, strHtmlCell);
        }

        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols,
            ref List<string> astrExegeticalHelpNotes)
        {
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.PresentationHtmlAsAddition(nVerseIndex, ref astrExegeticalHelpNotes);

            // make a cell out of the buttons
            string strHtmlCell = AddCellHtml(nVerseIndex, strRow);

            return FinishPresentationHtml(nVerseIndex, strHtmlCell);
        }

        public string GetAnchorButtonsCell(int nVerseIndex,
            AnchorsData childAnchorsData, StoryData.PresentationType presentationType, 
            ref List<string> astrExegeticalHelpNotes)
        {
            string strRow = null;
            foreach (AnchorData anchorData in this)
                strRow += anchorData.PresentationHtml(nVerseIndex, childAnchorsData, presentationType, false, ref astrExegeticalHelpNotes);

            // now put the anchors that are in the child (as additions)
            if (childAnchorsData != null)
                foreach (AnchorData anchorData in childAnchorsData)
                    strRow += anchorData.PresentationHtmlAsAddition(nVerseIndex, ref astrExegeticalHelpNotes);

            // make a cell out of the buttons
            return AddCellHtml(nVerseIndex, strRow);
        }

        protected string FinishPresentationHtml(int nVerseIndex, string strHtmlCell)
        {
            // add combine with the 'anc:' header cell into a Table Row
            return String.Format(Properties.Resources.HTML_TableRow,
                                 String.Format("{0}{1}",
                                               String.Format(Properties.Resources.HTML_TableCell,
                                                             AnchorLabel),
                                               strHtmlCell));
        }

        private string AddCellHtml(int nVerseIndex, string strRow)
        {
            return String.Format(Properties.Resources.HTML_TableCellWidthDropAnchor,
                                 AnchorId(nVerseIndex),
                                 100,
                                 strRow);
        }

        public static string AnchorId(int nVerseIndex)
        {
            return String.Format("anc_{0}", nVerseIndex);
        }

        public static string AnchorLabel
        {
            get { return Localizer.Str("anc:"); }
        }

        public void RemoveAll()
        {
            RemoveAll(a => true);
        }
    }
}
