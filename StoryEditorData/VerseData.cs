using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class VerseData
    {
        public string guid = null;
        public string VernacularText = null;
        public string NationalBTText = null;
        public string InternationalBTText = null;
        public AnchorsData Anchors = null;
        public TestQuestionsData TestQuestions = null;
        public RetellingsData Retellings = null;
        public ConsultantNotesData ConsultantNotes = null;
        public CoachNotesData CoachNotes = null;

        public VerseData()
        {
            guid = Guid.NewGuid().ToString();
            VernacularText = null;
            NationalBTText = null;
            InternationalBTText = null;
            Anchors = new AnchorsData();
            TestQuestions = new TestQuestionsData();
            Retellings = new RetellingsData();
            ConsultantNotes = new ConsultantNotesData();
            CoachNotes = new CoachNotesData();
        }

        public bool HasData
        {
            get
            {
                return (!String.IsNullOrEmpty(VernacularText) || !String.IsNullOrEmpty(NationalBTText)
                    || !String.IsNullOrEmpty(InternationalBTText));
            }
        }

        public XElement GetXml
        {
            get
            {
                XElement elemVerse = new XElement("verse", new XAttribute("guid", guid));
                if (!String.IsNullOrEmpty(VernacularText))
                    elemVerse.Add(new XElement("Vernacular", VernacularText));
                if (!String.IsNullOrEmpty(NationalBTText))
                    elemVerse.Add(new XElement("NationalBT", NationalBTText));
                if (!String.IsNullOrEmpty(InternationalBTText))
                    elemVerse.Add(new XElement("InternationalBT", InternationalBTText));
                if (Anchors.HasData)
                    elemVerse.Add(Anchors.GetXml);
                if (TestQuestions.HasData)
                    elemVerse.Add(TestQuestions.GetXml);
                if (Retellings.HasData)
                    elemVerse.Add(Retellings.GetXml);
                if (ConsultantNotes.HasData)
                    elemVerse.Add(ConsultantNotes.GetXml);
                if (CoachNotes.HasData)
                    elemVerse.Add(CoachNotes.GetXml);
                return elemVerse;
            }
        }
    }

    public class VersesData : List<VerseData>
    {
        public VersesData()
        {
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemVerses = new XElement("verses");
                foreach (VerseData aVerseData in this)
                    elemVerses.Add(aVerseData.GetXml);
                return elemVerses;
            }
        }
    }
}
