using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class MultipleLineDataConverter : List<string>
    {
        public string LabelTextFormat = null;
        protected List<string> MemberIDs = new List<string>();
        protected string CollectionElementName = null;
        protected string InstanceElementName = null;

        public XElement GetXml
        {
            get
            {
                XElement elemRetellings = new XElement(CollectionElementName);
                for (int i = 0; i < this.Count; i++)
                    elemRetellings.Add(new XElement(InstanceElementName, new XAttribute("memberID", MemberIDs[i]), this[i]));
                return elemRetellings;
            }
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        public RetellingsData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            LabelTextFormat = "ret({0}):";
            CollectionElementName = "Retellings";
            InstanceElementName = "Retelling";

            StoryProject.RetellingsRow[] theRetellingsRows = theVerseRow.GetRetellingsRows();
            StoryProject.RetellingsRow theRetellingsRow;
            if (theRetellingsRows.Length == 0)
                theRetellingsRow = projFile.Retellings.AddRetellingsRow(theVerseRow);
            else
                theRetellingsRow = theRetellingsRows[0];

            foreach (StoryProject.RetellingRow aRetellingRow in theRetellingsRow.GetRetellingRows())
                Add(aRetellingRow.Retelling_text);
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        public AnswersData(StoryProject.TestQuestionRow theTestQuestionRow, StoryProject projFile)
        {
            LabelTextFormat = "ans({0}):";
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
                Add(anAnswerRow.answer_text);
                MemberIDs.Add(anAnswerRow.memberID);
            }
        }
    }
}
