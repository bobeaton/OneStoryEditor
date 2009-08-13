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

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        // add a new retelling (have to know the member ID of the UNS giving it)
        public string AddNewLine(string strAnswer, string strMemberID)
        {
            string st = strAnswer;
            Add(st);
            MemberIDs.Add(strMemberID);
            return st;
        }

        public void RemoveLine(string strText)
        {
            for (int i = 0; i < this.Count; i++)
            {
                string st = this[i];
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
                XElement elem = new XElement(CollectionElementName);
                for (int i = 0; i < this.Count; i++)
                {
                    System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(MemberIDs[i]));
                    if (!String.IsNullOrEmpty(this[i]))
                        elem.Add(new XElement(InstanceElementName, new XAttribute("memberID", MemberIDs[i]), this[i]));
                }
                return elem;
            }
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        public RetellingsData()
        {
            LabelTextFormat = "ret({0}):";
            CollectionElementName = "Retellings";
            InstanceElementName = "Retelling";
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        public AnswersData()
        {
            LabelTextFormat = "ans({0}):";
            CollectionElementName = "Answers";
            InstanceElementName = "answer";
        }
    }
}
