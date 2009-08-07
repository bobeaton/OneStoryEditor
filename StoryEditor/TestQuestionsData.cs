using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TestQuestionData
    {
        public bool IsVisible = true;
        public string QuestionVernacular = null;
        public string QuestionEnglish = null;
        public AnswersData Answers = null;

        public TestQuestionData(StoryProject.TestQuestionRow theTestQuestionRow, StoryProject projFile)
        {
            IsVisible = theTestQuestionRow.visible;

            if (!theTestQuestionRow.IsTQVernacularNull())
                QuestionVernacular = theTestQuestionRow.TQVernacular;
            if (!theTestQuestionRow.IsTQInternationalBTNull())
                QuestionEnglish = theTestQuestionRow.TQInternationalBT;

            Answers = new AnswersData(theTestQuestionRow, projFile);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "TestQuestion", new XAttribute("visible", IsVisible),
                    new XElement(StoryEditor.ns + "TQVernacular", QuestionVernacular),
                    new XElement(StoryEditor.ns + "TQInternationalBT", QuestionEnglish),
                    Answers.GetXml);
            }
        }
    }

    public class TestQuestionsData : List<TestQuestionData>
    {
        public TestQuestionsData(StoryProject.verseRow theVerseRow, StoryProject projFile)
        {
            StoryProject.TestQuestionsRow[] theTestQuestionsRows = theVerseRow.GetTestQuestionsRows();
            StoryProject.TestQuestionsRow theTestQuestionsRow;
            if (theTestQuestionsRows.Length == 0)
                theTestQuestionsRow = projFile.TestQuestions.AddTestQuestionsRow(theVerseRow);
            else
                theTestQuestionsRow = theTestQuestionsRows[0];

            foreach (StoryProject.TestQuestionRow aTestingQuestionRow in theTestQuestionsRow.GetTestQuestionRows())
                Add(new TestQuestionData(aTestingQuestionRow, projFile));
        }

        public XElement GetXml
        {
            get
            {
                XElement elemTestQuestions = new XElement(StoryEditor.ns + "TestQuestions");
                foreach (TestQuestionData aTestQuestionData in this)
                    elemTestQuestions.Add(aTestQuestionData.GetXml);
                return elemTestQuestions;
            }
        }
    }
}
