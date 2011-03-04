using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ExegeticalHelpNoteData : StringTransfer
    {
        public ExegeticalHelpNoteData(NewDataSet.ExegeticalHelpRow theExHelpNoteRow)
            : base((theExHelpNoteRow.IsExegeticalHelp_ColumnNull())
            ? null
            : theExHelpNoteRow.ExegeticalHelp_Column)
        {
        }

        public ExegeticalHelpNoteData(string strInitialText)
            : base(strInitialText)
        {
        }

        public ExegeticalHelpNoteData(ExegeticalHelpNoteData rhs)
            : base(rhs.ToString())
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

        public static string TextareaId(int nVerseIndex, int nAnchorNumber, int nCommentNumber, string strTextElementName)
        {
            return String.Format("ta{0}_{1}_{2}_{3}", strTextElementName, nVerseIndex, 
                nAnchorNumber, nCommentNumber);
        }

        public string Html(int nVerseIndex, int nAnchorNumber)
        {
            string strHtml = null;
            for (int i = 0; i < Count; i++)
            {
                ExegeticalHelpNoteData anExHelpNoteData = this[i];
                string strHtmlElementId = TextareaId(nVerseIndex, nAnchorNumber, i, CstrFieldNameExegeticalHelp);
                strHtml += String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                         String.Format("{0}{1}",
                                                       String.Format(OseResources.Properties.Resources.HTML_TableCell, "cn:"),
                                                       String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                                     100,
                                                                     String.Format(OseResources.Properties.Resources.HTML_TextareaWithRefDrop,
                                                                                   strHtmlElementId,
                                                                                   StoryData.
                                                                                       CstrLangInternationalBtStyleClassName,
                                                                                   anExHelpNoteData))));
            }
            return strHtml;
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
    }
}
