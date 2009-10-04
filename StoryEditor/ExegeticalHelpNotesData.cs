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
    }
}
