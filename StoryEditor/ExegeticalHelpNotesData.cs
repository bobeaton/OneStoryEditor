using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ExegeticalHelpNoteData
    {
        public StringTransfer ExegeticalHelpNote = null;

        public ExegeticalHelpNoteData(StoryProject.exegeticalHelpRow theExHelpNoteRow)
        {
            ExegeticalHelpNote = new StringTransfer(theExHelpNoteRow.exegeticalHelp_Column);
        }

        public ExegeticalHelpNoteData(string strInitialText)
        {
            ExegeticalHelpNote = new StringTransfer(strInitialText);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "exegeticalHelp", ExegeticalHelpNote);
            }
        }
    }

    public class ExegeticalHelpNotesData : List<ExegeticalHelpNoteData>
    {
        public ExegeticalHelpNotesData(StoryProject.anchorRow theAnchorRow, StoryProject projFile)
        {
            StoryProject.exegeticalHelpsRow[] theExHelpNotesRows = theAnchorRow.GetexegeticalHelpsRows();
            StoryProject.exegeticalHelpsRow theExHelpNotesRow;
            if (theExHelpNotesRows.Length == 0)
                theExHelpNotesRow = projFile.exegeticalHelps.AddexegeticalHelpsRow(theAnchorRow);
            else
                theExHelpNotesRow = theExHelpNotesRows[0];

            foreach (StoryProject.exegeticalHelpRow anExHelpNoteRow in theExHelpNotesRow.GetexegeticalHelpRows())
                Add(new ExegeticalHelpNoteData(anExHelpNoteRow));
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

        public XElement GetXml
        {
            get
            {
                XElement elemExegeticalHelps = new XElement(StoryEditor.ns + "exegeticalHelps");
                foreach (ExegeticalHelpNoteData aExHelpData in this)
                    elemExegeticalHelps.Add(aExHelpData.GetXml);
                return elemExegeticalHelps;
            }
        }
    }
}
