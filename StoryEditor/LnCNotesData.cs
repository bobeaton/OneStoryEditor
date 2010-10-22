using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    public class LnCNotesData : List<LnCNote>
    {
        public LnCNotesData()
        {
        }

        public LnCNotesData(NewDataSet projFile)
        {
            NewDataSet.StoryProjectRow theStoryProjectRow = projFile.StoryProject[0];
            NewDataSet.LnCNotesRow[] theLnCNotesRows = theStoryProjectRow.GetLnCNotesRows();
            NewDataSet.LnCNotesRow theLnCNotesRow;
            if (theLnCNotesRows.Length == 0)
                theLnCNotesRow = projFile.LnCNotes.AddLnCNotesRow(theStoryProjectRow);
            else
                theLnCNotesRow = theLnCNotesRows[0];

            foreach (NewDataSet.LnCNoteRow aLnCNoteRow in theLnCNotesRow.GetLnCNoteRows())
                Add(new LnCNote(aLnCNoteRow));
        }

        public XElement GetXml
        {
            get 
            {
                var elem = new XElement("LnCNotes");

                foreach (var aLnCNote in this)
                    elem.Add(aLnCNote.GetXml);

                return elem;
            }
        }
    }

    public class LnCNote
    {
        protected string guid { get; set; }
        public string VernacularRendering { get; set; }
        public string NationalBtRendering { get; set; }
        public string InternationalBtRendering { get; set; }
        public string KeyTermId { get; set; }
        public string Notes { get; set; }

        public XElement GetXml
        {
            get
            {
                var elem = new XElement("LnCNote", Notes,
                    new XAttribute("guid", guid));

                if (!String.IsNullOrEmpty(VernacularRendering))
                    elem.Add(new XAttribute("VernacularRendering", VernacularRendering));

                if (!String.IsNullOrEmpty(NationalBtRendering))
                    elem.Add(new XAttribute("NationalBTRendering", NationalBtRendering));

                if (!String.IsNullOrEmpty(InternationalBtRendering))
                    elem.Add(new XAttribute("InternationalBTRendering", InternationalBtRendering));

                if (!String.IsNullOrEmpty(KeyTermId))
                    elem.Add(new XAttribute("KeyTermId", KeyTermId));

                return elem;
            }
        }

        public LnCNote()
        {
            guid = Guid.NewGuid().ToString();
            Notes = String.Empty; // xml doesn't like null for this
        }

        public LnCNote(NewDataSet.LnCNoteRow theLnCNoteRow)
        {
            guid = theLnCNoteRow.guid;  // the only thing absolutely required

            Notes = theLnCNoteRow.LnCNote_text;
            if (!theLnCNoteRow.IsVernacularRenderingNull())
                VernacularRendering = theLnCNoteRow.VernacularRendering;
            if (!theLnCNoteRow.IsNationalBTRenderingNull())
                NationalBtRendering = theLnCNoteRow.NationalBTRendering;
            if (!theLnCNoteRow.IsInternationalBTRenderingNull())
                InternationalBtRendering = theLnCNoteRow.InternationalBTRendering;
            if (!theLnCNoteRow.IsKeyTermIdNull())
                KeyTermId = theLnCNoteRow.KeyTermId;
        }
    }
}
