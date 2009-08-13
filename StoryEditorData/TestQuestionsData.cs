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

        public TestQuestionData()
        {
            QuestionVernacular = null;
            QuestionEnglish = null;
            Answers = new AnswersData();
        }

        public bool HasData
        {
            get { return (!String.IsNullOrEmpty(QuestionVernacular) || !String.IsNullOrEmpty(QuestionEnglish) 
                || Answers.HasData); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(QuestionVernacular)
                    || !String.IsNullOrEmpty(QuestionEnglish)
                    || Answers.HasData, "you have an empty TestQuestionData");

                XElement eleTQ = new XElement("TestQuestion", new XAttribute("visible", IsVisible));
                
                if (!String.IsNullOrEmpty(QuestionVernacular))
                    eleTQ.Add(new XElement("TQVernacular", QuestionVernacular));
                if (!String.IsNullOrEmpty(QuestionEnglish))
                    eleTQ.Add(new XElement("TQInternationalBT", QuestionEnglish));
                if (Answers.HasData)
                    eleTQ.Add(Answers.GetXml);

                return eleTQ;
            }
        }
    }

    public class TestQuestionsData : List<TestQuestionData>
    {
        public TestQuestionsData()
        {
        }

        public TestQuestionData AddTestQuestion()
        {
            TestQuestionData theTQ = new TestQuestionData();
            this.Add(theTQ);
            return theTQ;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an empty TestQuestionsData");
                XElement elemTestQuestions = new XElement("TestQuestions");
                foreach (TestQuestionData aTestQuestionData in this)
                    if (aTestQuestionData.HasData)
                        elemTestQuestions.Add(aTestQuestionData.GetXml);
                return elemTestQuestions;
            }
        }
    }
}
