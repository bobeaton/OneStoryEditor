using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class MultipleLineDataConverter
    {
        public abstract string LabelTextFormat
        {
            get;
        }

        public abstract List<string> RowData
        {
            get;
        }

        public abstract int Length
        {
            get;
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        protected StoryProject.RetellingsRow m_aRetellingsRow = null;

        public RetellingsData(StoryProject.RetellingsRow aRetellingsRow)
        {
            m_aRetellingsRow = aRetellingsRow;
        }

        public override string LabelTextFormat
        {
            get { return "ret({0}):"; }
        }

        public override int Length
        {
            get { return m_aRetellingsRow.GetRetellingRows().Length; }
        }

        public override List<string> RowData
        {
            get
            {
                List<string> lst = new List<string>(Length);
                foreach (StoryProject.RetellingRow aRTRow in m_aRetellingsRow.GetRetellingRows())
                    lst.Add(aRTRow.Retelling_text);
                return lst;
            }
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        protected StoryProject.AnswersRow m_anAnswerRow = null;

        public AnswersData(StoryProject.AnswersRow anAnswerRow)
        {
            m_anAnswerRow = anAnswerRow;
        }

        public override string LabelTextFormat
        {
            get { return "ans({0}):"; }
        }

        public override int Length
        {
            get { return m_anAnswerRow.GetanswerRows().Length; }
        }

        public override List<string> RowData
        {
            get
            {
                List<string> lst = new List<string>(Length);
                foreach (StoryProject.answerRow anAnswerRow in m_anAnswerRow.GetanswerRows())
                    lst.Add(anAnswerRow.answer_text);
                return lst;
            }
        }
    }
}
