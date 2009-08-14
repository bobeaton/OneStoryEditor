using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public abstract class ConsultNoteDataConverter
    {
        public int RoundNum = 0;
        
        public string MentorLabel = null;
        public string MentorComment = null;

        public string MenteeLabel = null;
        public string MenteeResponse = null;

        public Color CommentColor
        {
            get { return Color.Maroon; }
        }

        public Color ResponseColor
        {
            get { return Color.Blue; }
        }

        protected string InstanceElementName;
        protected string CommentElementName;
        protected string ResponseElementName;

        public bool HasData
        {
            get { return (!String.IsNullOrEmpty(MentorComment) || !String.IsNullOrEmpty(MenteeResponse)); }
        }

        public XElement GetXml
        {
            get
            {
                // must have guids if there's data
                XElement eleNote = new XElement(InstanceElementName, new XAttribute("round", RoundNum));
                if (!String.IsNullOrEmpty(MentorComment))
                    eleNote.Add(new XElement(CommentElementName, MentorComment));
                if (!String.IsNullOrEmpty(MenteeResponse))
                    eleNote.Add(new XElement(ResponseElementName, MenteeResponse));

                return eleNote;
            }
        }
    }

    public class ConsultantNoteData : ConsultNoteDataConverter
    {
        public ConsultantNoteData()
        {
            InstanceElementName = "ConsultantNote";
            CommentElementName = "ConsultantComment";
            ResponseElementName = "CrafterResponse";
        }
    }

    public class ConsultantNotesData : ConsultNotesDataConverter
    {
        public ConsultantNotesData()
        {
            CollectionElementName = "ConsultantNotes";
        }
    }

    public class CoachNoteData : ConsultNoteDataConverter
    {
        public CoachNoteData()
        {
            InstanceElementName = "CoachNote";
            CommentElementName = "CoachComment";
            ResponseElementName = "ConsultantResponse";
        }
    }

    public abstract class ConsultNotesDataConverter : List<ConsultNoteDataConverter>
    {
        protected string CollectionElementName = null;

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData);
                XElement elemCNDC = new XElement(CollectionElementName);
                foreach (ConsultNoteDataConverter aCNDC in this)
                    if (aCNDC.HasData)
                        elemCNDC.Add(aCNDC.GetXml);
                return elemCNDC;
            }
        }
    }

    public class CoachNotesData : ConsultNotesDataConverter
    {
        public CoachNotesData()
        {
            CollectionElementName = "CoachNotes";
        }
    }
}
