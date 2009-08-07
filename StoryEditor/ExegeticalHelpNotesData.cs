using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ExegeticalHelpNoteData
    {
        public string ExegeticalHelpNote = null;

        public ExegeticalHelpNoteData(StoryProject.exegeticalHelpRow theExHelpNoteRow)
        {
            ExegeticalHelpNote = theExHelpNoteRow.exegeticalHelp_Column;
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
