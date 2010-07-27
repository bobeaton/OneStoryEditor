using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public abstract class MultipleLineDataConverter : List<StringTransfer>
    {
        public List<string> MemberIDs = new List<string>();
        public abstract string CollectionElementName { get; }
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

        protected void InitFromXmlNode(XmlNode node, string strInstanceElementName)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(strInstanceElementName);
            if (list == null)
                return;

            foreach (XmlNode nodeList in list)
            {
                Add(new StringTransfer(nodeList.InnerText));
                MemberIDs.Add(nodeList.Attributes[CstrAttributeMemberID].Value);
            }
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

        public const string CstrAttributeMemberID = "memberID";

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
                    elem.Add(new XElement(InstanceElementName, new XAttribute(CstrAttributeMemberID, MemberIDs[i]), this[i]));
                }
                return elem;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (StringTransfer line in this)
                lstBoxesToSearch.AddNewVerseString(line, AssociatedViewMenu);
        }

        public static string TextareaId(string strPrefix, int nVerseIndex, int nRetellingNum)
        {
            return String.Format("ta{0}_{1}_{2}", strPrefix, nVerseIndex, nRetellingNum);
        }

        public string Html(int nVerseIndex, int nNumCols)
        {
            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                        String.Format("{0}{1}",
                                                      String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                                                    String.Format(LabelTextFormat, i + 1)),
                                                      String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                                    100,
                                                                    String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                                                  TextareaId(InstanceElementName, nVerseIndex, i),
                                                                                  StoryData.
                                                                                      CstrLangInternationalBtStyleClassName,
                                                                                  this[i]))));
            }

            // make a sub-table out of all this
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public string PresentationHtml(int nVerseIndex, int nNumCols, List<string> astrTestors,
            MultipleLineDataConverter child, bool bProcessingWithChild)
        {
            string strRow = null;
            int nTestNum = 0;
            for (int i = 0; i < Count; i++)
            {
                string strMemberId = MemberIDs[i];
                nTestNum = astrTestors.IndexOf(strMemberId);
                StringTransfer stParent = this[i];
                bool bFound = false;
                StringTransfer stChild = null;
                if (child != null)
                    for (int j = 0; j < child.Count; j++)
                    {
                        string strChildMemberID = child.MemberIDs[j];
                        if (strMemberId == strChildMemberID)
                        {
                            stChild = child[j];
                            child.RemoveAt(j);
                            child.MemberIDs.RemoveAt(j);
                            bFound = true;
                            break;
                        }
                    }

                string str;
                // if we found it, it means there was a child version... 
                if (bFound)
                    str = Diff.HtmlDiff(stParent, stChild); // so diff them

                // but if there was a child and yet we didn't find it...
                else if (child != null)
                    str = Diff.HtmlDiff(stParent, null);    // then the parent was deleted.

                // otherwise, if there was no child (e.g. just doing a print preview of one version)...
                else if (bProcessingWithChild)
                    str = Diff.HtmlDiff(null, stParent);
                
                else
                    str = stParent.ToString();  // then the parent's value is the value

                strRow += PresentationHtmlRow(nVerseIndex, nTestNum, str);
            }

            // finally, everything that is left in the child is new
            if ((child != null) && (child.Count > 0))
            {
                for (int j = 0; j < child.Count; j++)
                {
                    string strMemberId = child.MemberIDs[j];
                    nTestNum = astrTestors.IndexOf(strMemberId);
                    string str = Diff.HtmlDiff(null, child[j]);
                    strRow += PresentationHtmlRow(nVerseIndex, nTestNum, str);
                }
            }

            if (!String.IsNullOrEmpty(strRow))
            {
                // make a sub-table out of all this
                strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                        String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                      String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                    strRow)));
            }
            return strRow;
        }

        protected string PresentationHtmlRow(int nVerseIndex, int nTestNum, string str)
        {
            if (String.IsNullOrEmpty(str))
                str = "-";  // give it something so we don't have no cell there.
            return String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                 String.Format("{0}{1}",
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                                             String.Format(LabelTextFormat, nTestNum + 1)),
                                               String.Format(OseResources.Properties.Resources.HTML_TableCellWidth,
                                                             100,
                                                             String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                                           TextareaId(InstanceElementName, nVerseIndex, nTestNum),
                                                                           StoryData.
                                                                              CstrLangInternationalBtStyleClassName,
                                                                           str))));
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

        public RetellingsData(XmlNode node)
        {
            InitFromXmlNode(node, InstanceElementName);
        }

        public RetellingsData()
        {
        }

        public const string CstrElementLableRetellings = "Retellings";

        public override sealed string CollectionElementName
        {
            get { return CstrElementLableRetellings; }
        }

        protected override sealed string InstanceElementName
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

        public AnswersData(XmlNode node)
        {
            InitFromXmlNode(node, InstanceElementName);
        }

        public AnswersData(MultipleLineDataConverter rhs)
            : base(rhs)
        {
        }

        public AnswersData()
        {
        }

        public const string CstrElementLableAnswers = "Answers";

        public override sealed string CollectionElementName
        {
            get { return CstrElementLableAnswers; }
        }

        protected override sealed string InstanceElementName
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
