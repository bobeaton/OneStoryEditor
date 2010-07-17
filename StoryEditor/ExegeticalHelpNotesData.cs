using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ExegeticalHelpNoteData
    {
        public StringTransfer ExegeticalHelpNote = null;

        public ExegeticalHelpNoteData(NewDataSet.exegeticalHelpRow theExHelpNoteRow)
        {
            ExegeticalHelpNote = new StringTransfer(theExHelpNoteRow.exegeticalHelp_Column);
        }

        public ExegeticalHelpNoteData(string strInitialText)
        {
            ExegeticalHelpNote = new StringTransfer(strInitialText);
        }

        public ExegeticalHelpNoteData(ExegeticalHelpNoteData rhs)
        {
            ExegeticalHelpNote = new StringTransfer(rhs.ExegeticalHelpNote.ToString());
        }

        public bool HasData
        {
            get { return (ExegeticalHelpNote.HasData); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(ExegeticalHelpNote.HasData, "Trying to serialize an ExegeticalHelpNoteData with no data");
                return new XElement("exegeticalHelp", ExegeticalHelpNote);
            }
        }
    }

    public class ExegeticalHelpNotesData : List<ExegeticalHelpNoteData>
    {
        public ExegeticalHelpNotesData(NewDataSet.anchorRow theAnchorRow, NewDataSet projFile)
        {
            NewDataSet.exegeticalHelpsRow[] theExHelpNotesRows = theAnchorRow.GetexegeticalHelpsRows();
            NewDataSet.exegeticalHelpsRow theExHelpNotesRow;
            if (theExHelpNotesRows.Length == 0)
                theExHelpNotesRow = projFile.exegeticalHelps.AddexegeticalHelpsRow(theAnchorRow);
            else
                theExHelpNotesRow = theExHelpNotesRows[0];

            foreach (NewDataSet.exegeticalHelpRow anExHelpNoteRow in theExHelpNotesRow.GetexegeticalHelpRows())
                Add(new ExegeticalHelpNoteData(anExHelpNoteRow));
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

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemExegeticalHelps = new XElement("exegeticalHelps");
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
                strHtml += String.Format(Properties.Resources.HTML_TableRow,
                                         String.Format("{0}{1}",
                                                       String.Format(Properties.Resources.HTML_TableCell, "cn:"),
                                                       String.Format(Properties.Resources.HTML_TableCellWidth,
                                                                     100,
                                                                     String.Format(Properties.Resources.HTML_TextareaWithRefDrop,
                                                                                   strHtmlElementId,
                                                                                   StoryData.
                                                                                       CstrLangInternationalBtStyleClassName,
                                                                                   anExHelpNoteData.ExegeticalHelpNote))));
            }
            return strHtml;
        }
    }
}
