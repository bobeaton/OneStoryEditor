using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OneStoryProjectEditor
{
    // class encapsulating one retelling or TQ answer possibly in multiple languages
    //  the List-ness of this is that there may be 3 StringTransfers for each of
    //  the StoryEditor.TextFieldType types
    public class LineData : List<StringTransfer>
    {
        public string MemberId { get; set; }

        public bool HasData
        {
            get
            {
                return this[StoryEditor.CnInternationalBt].HasData
                       || this[StoryEditor.CnNationalBt].HasData
                       || this[StoryEditor.CnVernacular].HasData;
            }
        }

        public LineData(LineData rhs)
        {
            MemberId = rhs.MemberId;
            for (var i = StoryEditor.CnInternationalBt;
                i <= StoryEditor.CnVernacular; 
                i++)
            {
                var stRhs = rhs[i];
                var st = new StringTransfer((stRhs.HasData) ? rhs[i].ToString() : null);
                Add(st);
            }
        }

        public LineData(string strMemberId)
        {
            MemberId = strMemberId;
            Add(new StringTransfer(""));
            Add(new StringTransfer(""));
            Add(new StringTransfer(""));
        }

        public CtrlTextBox ExistingTextBox
        {
            get
            {
                if (this[StoryEditor.CnInternationalBt].TextBox != null)
                    return this[StoryEditor.CnInternationalBt].TextBox;
                if (this[StoryEditor.CnNationalBt].TextBox != null)
                    return this[StoryEditor.CnNationalBt].TextBox;
                if (this[StoryEditor.CnVernacular].TextBox != null)
                    return this[StoryEditor.CnVernacular].TextBox;
                return null;
            }
        }
    }

    public abstract class MultipleLineDataConverter : List<LineData>
    {
        public abstract string CollectionElementName { get; }
        protected abstract string InstanceElementName { get; }
        public abstract string LabelTextFormat { get; }
        protected abstract VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu { get; }

        protected MultipleLineDataConverter(List<LineData> rhs)
        {
            foreach (LineData aLineData in rhs)
                Add(new LineData(aLineData));
        }

        protected MultipleLineDataConverter()
        {
        }

        public LineData TryGetValue(string strMemberId)
        {
            return this.FirstOrDefault(aLineData => aLineData.MemberId == strMemberId);
        }

        protected void InitFromXmlNode(XmlNode node, string strInstanceElementName)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes(strInstanceElementName);
            if (list == null)
                return;

            // e.g. 
            // <Retelling memberID="mem-34719c50-a00d-4910-846d-1c17b14ec973"></Retelling>
            // <Retelling lang="Vernacular" memberID="mem-34719c50-a00d-4910-846d-1c17b14ec973"></Retelling>
            foreach (XmlNode nodeFromList in list)
            {
                string strMemberId = nodeFromList.Attributes[CstrAttributeMemberID].Value;
                string strValue = nodeFromList.InnerText;
                XmlAttribute attrLangId = nodeFromList.Attributes[CstrAttributeLang];
                string strLangId = null;
                if (attrLangId != null)
                    strLangId = attrLangId.Value;

                AddLineDataValue(strMemberId, strLangId, strValue);
            }
        }

        protected void AddLineDataValue(string strMemberId, string strLangId, string strValue)
        {
            int nIndex = StoryEditor.CnInternationalBt;
            if (strLangId == CstrAttributeLangVernacular)
                nIndex = StoryEditor.CnVernacular;
            else if (strLangId == CstrAttributeLangNationalBT)
                nIndex = StoryEditor.CnNationalBt;

            LineData theLineData = TryGetValue(strMemberId);
            if (theLineData == null)
            {
                theLineData = new LineData(strMemberId);
                Add(theLineData);
            }
            theLineData[nIndex].SetValue(strValue);
        }

        public bool HasData
        {
            get { return (Count > 0); }
        }

        // add a new retelling (have to know the member ID of the UNS giving it)
        public LineData AddNewLine(string strMemberId)
        {
            LineData theLineData = TryGetValue(strMemberId);
            if (theLineData == null)
            {
                theLineData = new LineData(strMemberId);
                Add(theLineData);
            }
            return theLineData;
        }

        public const string CstrAttributeMemberID = "memberID";
        public const string CstrAttributeLang = "lang";
        public const string CstrAttributeLangVernacular = "Vernacular";
        public const string CstrAttributeLangNationalBT = "NationalBT";

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, String.Format("You have an empty collection of {0} that you're trying to serialize", CollectionElementName));
                XElement elem = new XElement(CollectionElementName);
                foreach (LineData aLineData in this)
                {
                    foreach (StringTransfer st in aLineData)
                    {
                        XElement elemLineData = new XElement(InstanceElementName);
                        elemLineData.Add(new XAttribute(CstrAttributeMemberID, aLineData.MemberId));

                        if (aLineData.IndexOf(st) == StoryEditor.CnVernacular)
                            elemLineData.Add(new XAttribute(CstrAttributeLang, CstrAttributeLangVernacular));
                        else if (aLineData.IndexOf(st) == StoryEditor.CnNationalBt)
                            elemLineData.Add(new XAttribute(CstrAttributeLang, CstrAttributeLangNationalBT));

                        elemLineData.Add(st);
                        elem.Add(elemLineData);
                    }
                }

                return elem;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            foreach (LineData aLineData in this)
            {
                if (findProperties.StoryLanguage)
                    lstBoxesToSearch.AddNewVerseString(aLineData[StoryEditor.CnVernacular], AssociatedViewMenu);
                if (findProperties.NationalBT)
                    lstBoxesToSearch.AddNewVerseString(aLineData[StoryEditor.CnNationalBt], AssociatedViewMenu);
                if (findProperties.EnglishBT)
                    lstBoxesToSearch.AddNewVerseString(aLineData[StoryEditor.CnInternationalBt], AssociatedViewMenu);
            }
        }

        public static string TextareaId(string strPrefix, int nVerseIndex, int nRetellingNum, string strFieldTypeName)
        {
            return String.Format("ta{0}_{1}_{2}_{3}", strPrefix, nVerseIndex, nRetellingNum, strFieldTypeName);
        }

        /*
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
        */

        public string Html(int nVerseIndex, int nNumTestQuestionCols, 
            bool bShowVernacular, bool bShowNationalBT, bool bShowEnglishBT)
        {
            int nAnswerCols = 0;
            if (bShowVernacular) nAnswerCols++;
            if (bShowNationalBT) nAnswerCols++;
            if (bShowEnglishBT) nAnswerCols++;

            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                        String.Format(LabelTextFormat, i + 1));

                LineData theLine = this[i];
                if (bShowVernacular)
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i, VerseData.CstrFieldNameVernacular),
                                                          StoryData.CstrLangVernacularStyleClassName,
                                                          theLine[StoryEditor.CnVernacular]));
                }

                if (bShowNationalBT)
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i, VerseData.CstrFieldNameNationalBt),
                                                          StoryData.CstrLangNationalBtStyleClassName,
                                                          theLine[StoryEditor.CnNationalBt]));
                }

                if (bShowEnglishBT)
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i, VerseData.CstrFieldNameInternationalBt),
                                                          StoryData.CstrLangInternationalBtStyleClassName,
                                                          theLine[StoryEditor.CnInternationalBt]));
                }

                strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                       strRow);
            }

            // make a sub-table out of all this
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nAnswerCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }
        public string PresentationHtml(int nVerseIndex, int nNumCols, List<string> astrTestors,
            MultipleLineDataConverter child, bool bPrintPreview, bool bProcessingTheChild,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT)
        {
            string strRow = null;
            int nTestNum = 0;
            for (int i = 0; i < Count; i++)
            {
                LineData theParentLineData = this[i];
                string strMemberId = theParentLineData.MemberId;
                nTestNum = astrTestors.IndexOf(strMemberId);

                bool bFound = false;
                LineData theChildLineData = null;

                if (child != null)
                {
                    theChildLineData = TryGetValue(strMemberId);
                    if (theChildLineData != null)
                    {
                        Remove(theChildLineData);
                        bFound = true;
                    }
                }

                string strVernacular, strNationalBT, strInternationalBT;
                // if we found it, it means there was a child version... 
                if (bFound)
                {
                    // so diff them
                    strVernacular = Diff.HtmlDiff(theParentLineData[StoryEditor.CnVernacular],
                                                  theChildLineData[StoryEditor.CnVernacular]);
                    strNationalBT = Diff.HtmlDiff(theParentLineData[StoryEditor.CnNationalBt],
                                                  theChildLineData[StoryEditor.CnNationalBt]);
                    strInternationalBT = Diff.HtmlDiff(theParentLineData[StoryEditor.CnInternationalBt],
                                                       theChildLineData[StoryEditor.CnInternationalBt]);
                }

                // but if there was a child and yet we didn't find it...
                // OR if there wasn't a child, but there should have been (because we're processing with a child)
                else if (((child != null) || !bPrintPreview) && !bProcessingTheChild)
                {
                    // it means that the parent was deleted.
                    strVernacular = Diff.HtmlDiff(theParentLineData[StoryEditor.CnVernacular], null);
                    strNationalBT = Diff.HtmlDiff(theParentLineData[StoryEditor.CnNationalBt], null);
                    strInternationalBT = Diff.HtmlDiff(theParentLineData[StoryEditor.CnInternationalBt], null);
                }

                // this means there is a child and we're processing it here as if it were the parent
                //  (so that implicitly means this is an addition)
                else if (bProcessingTheChild)
                {
                    strVernacular = Diff.HtmlDiff(null, theParentLineData[StoryEditor.CnVernacular]);
                    strNationalBT = Diff.HtmlDiff(null, theParentLineData[StoryEditor.CnNationalBt]);
                    strInternationalBT = Diff.HtmlDiff(null, theParentLineData[StoryEditor.CnInternationalBt]);
                }

                // otherwise, if there was no child (e.g. just doing a print preview of one version)...
                else
                {
                    // then the parent's value is the value
                    strVernacular = theParentLineData[StoryEditor.CnVernacular].ToString();
                    strNationalBT = theParentLineData[StoryEditor.CnNationalBt].ToString();
                    strInternationalBT = theParentLineData[StoryEditor.CnInternationalBt].ToString();
                }

                strRow += PresentationHtmlRow(nVerseIndex, nTestNum,
                    strVernacular, strNationalBT, strInternationalBT,
                    bShowVernacular, bShowNationalBT, bShowInternationalBT);
            }

            // finally, everything that is left in the child is new
            if ((child != null) && (child.Count > 0))
            {
                for (int j = 0; j < child.Count; j++)
                {
                    LineData theChildLineData = child[j];
                    string strMemberId = theChildLineData.MemberId;
                    nTestNum = astrTestors.IndexOf(strMemberId);
                    string strVernacular = Diff.HtmlDiff(null, theChildLineData[StoryEditor.CnVernacular]);
                    string strNationalBT = Diff.HtmlDiff(null, theChildLineData[StoryEditor.CnNationalBt]);
                    string strInternationalBT = Diff.HtmlDiff(null, theChildLineData[StoryEditor.CnInternationalBt]);
                    strRow += PresentationHtmlRow(nVerseIndex, nTestNum,
                        strVernacular, strNationalBT, strInternationalBT,
                        bShowVernacular, bShowNationalBT, bShowInternationalBT);
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


        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols, List<string> astrTestors,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT)
        {
            string strRow = null;
            int nTestNum = 0;
            for (int i = 0; i < Count; i++)
            {
                LineData theLineData = this[i];
                string strMemberId = theLineData.MemberId;
                nTestNum = astrTestors.IndexOf(strMemberId);
                string strVernacular = Diff.HtmlDiff(null, theLineData[StoryEditor.CnVernacular]);
                string strNationalBT = Diff.HtmlDiff(null, theLineData[StoryEditor.CnNationalBt]);
                string strEnglishBT = Diff.HtmlDiff(null, theLineData[StoryEditor.CnInternationalBt]);
                strRow += PresentationHtmlRow(nVerseIndex, nTestNum,
                    strVernacular, strNationalBT, strEnglishBT, 
                    bShowVernacular, bShowNationalBT, bShowInternationalBT);
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

        protected string PresentationHtmlRow(int nVerseIndex, int nTestNum,
            string strVernacular, string strNationalBT, string strInternationalBT,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT)
        {
            string strRow = String.Format(OseResources.Properties.Resources.HTML_TableCellNoWrap,
                                          String.Format(LabelTextFormat, nTestNum + 1));

            int nNumCols = 0;
            if (bShowVernacular) nNumCols++;
            if (bShowNationalBT) nNumCols++;
            if (bShowInternationalBT) nNumCols++;

            if (bShowVernacular)
            {
                if (String.IsNullOrEmpty(strVernacular))
                    strVernacular = "-";  // give it something so we don't have no cell there.
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100/nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(InstanceElementName, nVerseIndex, nTestNum,
                                                                 VerseData.CstrFieldNameVernacular),
                                                      StoryData.CstrLangVernacularStyleClassName,
                                                      strVernacular));
            }

            if (bShowNationalBT)
            {
                if (String.IsNullOrEmpty(strNationalBT))
                    strNationalBT = "-";  // give it something so we don't have no cell there.
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(InstanceElementName, nVerseIndex, nTestNum,
                                                                 VerseData.CstrFieldNameNationalBt),
                                                      StoryData.CstrLangNationalBtStyleClassName,
                                                      strNationalBT));
            }

            if (bShowInternationalBT)
            {
                if (String.IsNullOrEmpty(strInternationalBT))
                    strInternationalBT = "-";  // give it something so we don't have no cell there.
                strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop, 100 / nNumCols,
                                        String.Format(OseResources.Properties.Resources.HTML_ParagraphText,
                                                      TextareaId(InstanceElementName, nVerseIndex, nTestNum,
                                                                 VerseData.CstrFieldNameInternationalBt),
                                                      StoryData.CstrLangInternationalBtStyleClassName,
                                                      strInternationalBT));
            }

            // make a sub-table out of all this
            // TODO: I think nNumCols here is wrong. It should be the column count of the parent
            strRow = String.Format(OseResources.Properties.Resources.HTML_TableRow,
                                    String.Format(OseResources.Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(OseResources.Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
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
                string strLangId = (aRetellingRow.IslangNull()) ? null : aRetellingRow.lang;
                string strValue = (aRetellingRow.IsRetelling_textNull()) ? "" : aRetellingRow.Retelling_text;
                AddLineDataValue(aRetellingRow.memberID, strLangId, strValue);
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

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.RetellingFields; }
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
                string strLangId = (anAnswerRow.IslangNull()) ? null : anAnswerRow.lang;
                string strValue = (anAnswerRow.Isanswer_textNull()) ? "" : anAnswerRow.answer_text;
                AddLineDataValue(anAnswerRow.memberID, strLangId, strValue);
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

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers; }
        }
    }
}
