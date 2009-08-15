using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class MultipleLineDataConverter : List<StringTransfer>
    {
        public string LabelTextFormat = null;
        protected List<string> MemberIDs = new List<string>();
        protected string CollectionElementName = null;
        protected string InstanceElementName = null;

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        // add a new retelling (have to know the member ID of the UNS giving it)
        public StringTransfer AddNewLine(string strMemberID)
        {
            StringTransfer st = new StringTransfer(null);
            Add(st);
            MemberIDs.Add(strMemberID);
            return st;
        }

        public void RemoveLine(string strText)
        {
            for (int i = 0; i < this.Count; i++)
            {
                StringTransfer st = this[i];
                if (st.ToString() == strText)
                {
                    this.RemoveAt(i);
                    MemberIDs.RemoveAt(i);
                    break;
                }
            }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, String.Format("You have an empty collection of {0} that you're trying to serialize", CollectionElementName));
                XElement elem = new XElement(StoriesData.ns + CollectionElementName);
                for (int i = 0; i < this.Count; i++)
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(MemberIDs[i]));
                    if (this[i].HasData)
                        elem.Add(new XElement(StoriesData.ns + InstanceElementName, new XAttribute("memberID", MemberIDs[i]), this[i]));
                }
                return elem;
            }
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        public RetellingsData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            LabelTextFormat = "ret {0}:";
            CollectionElementName = "Retellings";
            InstanceElementName = "Retelling";

            StoryProject.RetellingsRow[] theRetellingsRows = theVerseRow.GetRetellingsRows();
            StoryProject.RetellingsRow theRetellingsRow;
            if (theRetellingsRows.Length == 0)
                theRetellingsRow = projFile.Retellings.AddRetellingsRow(theVerseRow);
            else
                theRetellingsRow = theRetellingsRows[0];

            foreach (StoryProject.RetellingRow aRetellingRow in theRetellingsRow.GetRetellingRows())
            {
                Add(new StringTransfer(aRetellingRow.Retelling_text));
                MemberIDs.Add(aRetellingRow.memberID);
            }
        }

        public RetellingsData()
        {
            LabelTextFormat = "ret {0}:";
            CollectionElementName = "Retellings";
            InstanceElementName = "Retelling";
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        public AnswersData(StoryProject.TestQuestionRow theTestQuestionRow, StoryProject projFile)
        {
            LabelTextFormat = "ans {0}:";
            CollectionElementName = "Answers";
            InstanceElementName = "answer";

            StoryProject.AnswersRow[] theAnswersRows = theTestQuestionRow.GetAnswersRows();
            StoryProject.AnswersRow theAnswersRow;
            if (theAnswersRows.Length == 0)
                theAnswersRow = projFile.Answers.AddAnswersRow(theTestQuestionRow);
            else
                theAnswersRow = theAnswersRows[0];

            foreach (StoryProject.answerRow anAnswerRow in theAnswersRow.GetanswerRows())
            {
                Add(new StringTransfer(anAnswerRow.answer_text));
                MemberIDs.Add(anAnswerRow.memberID);
            }
        }

        public AnswersData()
        {
        }
    }
}
