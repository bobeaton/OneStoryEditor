using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class MultipleLineDataConverter : List<StringTransfer>
    {
        public List<string> MemberIDs = new List<string>();
        protected abstract string CollectionElementName { get; }
        protected abstract string InstanceElementName { get; }
        public abstract string LabelTextFormat { get; }
        protected abstract VerseData.ViewItemToInsureOn AssociatedViewMenu { get; }

        protected MultipleLineDataConverter(MultipleLineDataConverter rhs)
        {
            foreach (string str in rhs.MemberIDs)
                MemberIDs.Add(str);

            foreach (StringTransfer aST in rhs)
                Add(new StringTransfer(aST.ToString()));
        }

        protected MultipleLineDataConverter()
        {
        }

        public bool HasData
        {
            get { return (Count > 0); }
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
            for (int i = 0; i < Count; i++)
            {
                StringTransfer st = this[i];
                if (st.ToString() == strText)
                {
                    RemoveAt(i);
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
                XElement elem = new XElement(CollectionElementName);
                for (int i = 0; i < this.Count; i++)
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(MemberIDs[i]));
                    // for these instances of StringTransfers, we want to store them even if they are empty
                    //  so that the boxes will persist across save and restarts
                    // if (this[i].HasData)
                        elem.Add(new XElement(InstanceElementName, new XAttribute("memberID", MemberIDs[i]), this[i]));
                }
                return elem;
            }
        }

        public void IndexSearch(int nVerseNum, SearchForm.SearchLookInProperties findProperties,
            ref SearchForm.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (StringTransfer line in this)
                lstBoxesToSearch.AddNewVerseString(nVerseNum, line, AssociatedViewMenu);
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        public RetellingsData(NewDataSet.verseRow theVerseRow, NewDataSet projFile)
        {
            NewDataSet.RetellingsRow[] theRetellingsRows = theVerseRow.GetRetellingsRows();
            NewDataSet.RetellingsRow theRetellingsRow;
            if (theRetellingsRows.Length == 0)
                theRetellingsRow = projFile.Retellings.AddRetellingsRow(theVerseRow);
            else
                theRetellingsRow = theRetellingsRows[0];

            foreach (NewDataSet.RetellingRow aRetellingRow in theRetellingsRow.GetRetellingRows())
            {
                Add(new StringTransfer((aRetellingRow.IsRetelling_textNull()) ? "" : aRetellingRow.Retelling_text));
                MemberIDs.Add(aRetellingRow.memberID);
            }
        }

        public RetellingsData(MultipleLineDataConverter rhs)
            : base(rhs)
        {
        }

        public RetellingsData()
        {
        }

        protected override string CollectionElementName
        {
            get { return "Retellings"; }
        }

        protected override string InstanceElementName
        {
            get { return "Retelling"; }
        }

        public override string LabelTextFormat
        {
            get { return "ret {0}:"; }
        }

        protected override VerseData.ViewItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewItemToInsureOn.eRetellingFields; }
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        public AnswersData(NewDataSet.TestQuestionRow theTestQuestionRow, NewDataSet projFile)
        {
            NewDataSet.AnswersRow[] theAnswersRows = theTestQuestionRow.GetAnswersRows();
            NewDataSet.AnswersRow theAnswersRow;
            if (theAnswersRows.Length == 0)
                theAnswersRow = projFile.Answers.AddAnswersRow(theTestQuestionRow);
            else
                theAnswersRow = theAnswersRows[0];

            foreach (NewDataSet.answerRow anAnswerRow in theAnswersRow.GetanswerRows())
            {
                Add(new StringTransfer((anAnswerRow.Isanswer_textNull()) ? "" : anAnswerRow.answer_text));
                MemberIDs.Add(anAnswerRow.memberID);
            }
        }

        public AnswersData(MultipleLineDataConverter rhs)
            : base(rhs)
        {
        }

        public AnswersData()
        {
        }

        protected override string CollectionElementName
        {
            get { return "Answers"; }
        }

        protected override string InstanceElementName
        {
            get { return "answer"; }
        }

        public override string LabelTextFormat
        {
            get { return "ans {0}:"; }
        }

        protected override VerseData.ViewItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewItemToInsureOn.eStoryTestingQuestionFields; }
        }
    }
}
