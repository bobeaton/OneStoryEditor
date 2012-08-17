using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using NetLoc;

namespace OneStoryProjectEditor
{
    public class ExegeticalHelpNoteData : StringTransfer
    {
        private const StoryEditor.TextFields CMyField =
            StoryEditor.TextFields.ExegeticalNote | StoryEditor.TextFields.InternationalBt;

        public ExegeticalHelpNoteData(NewDataSet.ExegeticalHelpRow theExHelpNoteRow)
            : base((theExHelpNoteRow.IsExegeticalHelp_ColumnNull())
                        ? null
                        : theExHelpNoteRow.ExegeticalHelp_Column, CMyField)
        {
            
        }

        public ExegeticalHelpNoteData(string strInitialText)
            : base(strInitialText, CMyField)
        {
        }

        public ExegeticalHelpNoteData(ExegeticalHelpNoteData rhs)
            : base(rhs.ToString(), CMyField)
        {
        }

        public const string CstrElementNameExegeticalHelp = "ExegeticalHelp";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "Trying to serialize an ExegeticalHelpNoteData with no data");
                return new XElement(CstrElementNameExegeticalHelp, ToString());
            }
        }
    }

    public class ExegeticalHelpNotesData : List<ExegeticalHelpNoteData>
    {
        public ExegeticalHelpNotesData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
        {
            NewDataSet.ExegeticalHelpsRow[] theExHelpNotesRows = theVerseRow.GetExegeticalHelpsRows();
            NewDataSet.ExegeticalHelpsRow theExHelpNotesRow;
            if (theExHelpNotesRows.Length == 0)
                theExHelpNotesRow = projFile.ExegeticalHelps.AddExegeticalHelpsRow(theVerseRow);
            else
                theExHelpNotesRow = theExHelpNotesRows[0];

            foreach (NewDataSet.ExegeticalHelpRow anExHelpNoteRow in theExHelpNotesRow.GetExegeticalHelpRows())
                Add(new ExegeticalHelpNoteData(anExHelpNoteRow));
        }

        public ExegeticalHelpNotesData(XmlNode node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(ExegeticalHelpNoteData.CstrElementNameExegeticalHelp);
            if (list == null)
                return;

            foreach (XmlNode nodeExegeticalNote in list)
                Add(new ExegeticalHelpNoteData(nodeExegeticalNote.InnerText));
        }

        public ExegeticalHelpNotesData(ExegeticalHelpNotesData rhs)
        {
            foreach (ExegeticalHelpNoteData aEHN in rhs)
                Add(new ExegeticalHelpNoteData(aEHN));
        }

        public ExegeticalHelpNotesData()
        {
        }

        public ExegeticalHelpNoteData AddExegeticalHelpNote(string strInitialText)
        {
            ExegeticalHelpNoteData theEHN = new ExegeticalHelpNoteData(strInitialText);
            this.Add(theEHN);
            return theEHN;
        }

        public bool HasData
        {
            get { return (Count > 0); }
        }

        public const string CstrElementLabelExegeticalHelps = "ExegeticalHelps";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemExegeticalHelps = new XElement(CstrElementLabelExegeticalHelps);
                foreach (ExegeticalHelpNoteData aExHelpData in this)
                    if (aExHelpData.HasData)
                        elemExegeticalHelps.Add(aExHelpData.GetXml);
                return elemExegeticalHelps;
            }
        }

        protected const string CstrFieldNameExegeticalHelp = "ExegeticalHelp";

        public static string CstrCnLable
        {
            get { return Localizer.Str("cn:"); }
        }

        public void PresentationHtml(ExegeticalHelpNotesData child, ref List<string> astrExegeticalHelpNotes)
        {
            // processing parent...
            foreach (ExegeticalHelpNoteData anExHelpNoteData in this)
            {
                bool bFound = false;
                if (child != null)
                    foreach (ExegeticalHelpNoteData anChildExHelpNoteData in child)
                        if (anExHelpNoteData.ToString() == anChildExHelpNoteData.ToString())
                        {
                            child.Remove(anChildExHelpNoteData);
                            bFound = true;
                            break;
                        }

                // if there was a child, but we didn't find it...
                if ((child != null) && !bFound)
                    // means the parent was deleted in the child version
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(anExHelpNoteData, null));

                // otherwise, the parent's version is the only one
                else
                    astrExegeticalHelpNotes.Add(anExHelpNoteData.ToString());
            }

            if (child != null)
                // what's left in the child are those that were added...
                foreach (ExegeticalHelpNoteData anChildExHelpNoteData in child)
                    astrExegeticalHelpNotes.Add(Diff.HtmlDiff(null, anChildExHelpNoteData));
        }

        public string FinishPresentationHtml(string strAnchorHtml, int nVerseIndex,
            int nNumCols, List<string> astrExegeticalHelpNotes, VerseData.ViewSettings viewSettings)
        {
            string strHtml = strAnchorHtml;
            if (viewSettings.IsViewItemOn(VerseData.ViewSettings.ItemToInsureOn.UseTextAreas))
            {
                // this means that in the Story BT pane, so add the exe notes directly from
                //  their StringTransfers
                for (int i = 0; i < Count; i++)
                {
                    var anExHelpNote = this[i];
                    var strNote = anExHelpNote.FormatLanguageColumnHtml(nVerseIndex,
                                                                        i,
                                                                        1,
                                                                        anExHelpNote.ToString(),
                                                                        viewSettings);
                    strHtml += String.Format(Properties.Resources.HTML_TableRow,
                                             String.Format("{0}{1}",
                                                           String.Format(Properties.Resources.HTML_TableCell, CstrCnLable),
                                                           strNote));
                }
            }
            else
            {
                // add exegetical comments as their own rows
                foreach (var strExegeticalHelpNote in astrExegeticalHelpNotes)
                {
                    var strValue = (!String.IsNullOrEmpty(strExegeticalHelpNote))
                    ? strExegeticalHelpNote
                    : "-";  // just so there's something there (or the cell doesn't show)

                    strHtml += String.Format(Properties.Resources.HTML_TableCellWidthAlignTop, 100/nNumCols,
                                             String.Format(Properties.Resources.HTML_ParagraphText,
                                                           StoryData.CstrLangInternationalBtStyleClassName,
                                                           strValue));
                }
            }

            // make a sub-table out of all this
            return String.Format(Properties.Resources.HTML_TableRow,
                                 String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                               String.Format(Properties.Resources.HTML_Table,
                                                             strHtml)));
        }
    }
}
