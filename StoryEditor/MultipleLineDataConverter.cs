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
    public class LineMemberData : LineData
    {
        public string MemberId { get; set; }

        public LineMemberData(LineMemberData rhs)
            : base(rhs)
        {
            MemberId = rhs.MemberId;
        }

        public LineMemberData(string strMemberId)
        {
            MemberId = strMemberId;
        }

        public const string CstrAttributeMemberID = "memberID";

        public override void AddXml(XElement elem, string strFieldName)
        {
            if (Vernacular.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangVernacular),
                    new XAttribute(CstrAttributeMemberID, MemberId),
                    Vernacular));
            if (NationalBt.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangNationalBt),
                    new XAttribute(CstrAttributeMemberID, MemberId),
                    NationalBt));
            if (InternationalBt.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangInternationalBt),
                    new XAttribute(CstrAttributeMemberID, MemberId),
                    InternationalBt));
            if (FreeTranslation.HasData)
                elem.Add(new XElement(strFieldName,
                    new XAttribute(CstrAttributeLang, CstrAttributeLangFreeTranslation),
                    new XAttribute(CstrAttributeMemberID, MemberId),
                    FreeTranslation));
        }

        public override void IndexSearch(VerseData.SearchLookInProperties findProperties,
            VerseData.ViewSettings.ItemToInsureOn itemToInsureOn,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (Vernacular.HasData && findProperties.StoryLanguage)
                lstBoxesToSearch.AddNewVerseString(Vernacular, itemToInsureOn);
            if (NationalBt.HasData && findProperties.NationalBT)
                lstBoxesToSearch.AddNewVerseString(NationalBt, itemToInsureOn);
            if (InternationalBt.HasData && findProperties.EnglishBT)
                lstBoxesToSearch.AddNewVerseString(InternationalBt, itemToInsureOn);
            if (FreeTranslation.HasData && findProperties.FreeTranslation)
                lstBoxesToSearch.AddNewVerseString(FreeTranslation, itemToInsureOn);
        }
    }

    public abstract class MultipleLineDataConverter : List<LineMemberData>
    {
        public abstract string CollectionElementName { get; }
        protected abstract string InstanceElementName { get; }
        public abstract string LabelTextFormat { get; }
        protected abstract VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu { get; }
        protected abstract bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties);

        protected MultipleLineDataConverter(IEnumerable<LineMemberData> rhs)
        {
            foreach (LineMemberData aLineData in rhs)
                Add(new LineMemberData(aLineData));
        }

        protected MultipleLineDataConverter()
        {
        }

        public LineMemberData TryGetValue(string strMemberId)
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
            // <Retelling lang="Vernacular" memberID="mem-34719c50-a00d-4910-846d-1c17b14ec973"></Retelling>
            foreach (XmlNode nodeFromList in list)
            {
                if (nodeFromList.Attributes != null)
                {
                    string strLangId = nodeFromList.Attributes[LineData.CstrAttributeLang].Value;
                    string strMemberId = nodeFromList.Attributes[LineMemberData.CstrAttributeMemberID].Value;
                    string strValue = nodeFromList.InnerText;
                    AddLineDataValue(strMemberId, strLangId, strValue);
                }
            }
        }

        protected void AddLineDataValue(string strMemberId, string strLangId, string strValue)
        {
            LineMemberData theLineData = TryAddNewLine(strMemberId);
            theLineData.SetValue(strLangId, strValue);
        }

        public bool HasData
        {
            get { return (Count > 0); }
        }

        // add a new retelling (have to know the member ID of the UNS giving it)
        public LineMemberData TryAddNewLine(string strMemberId)
        {
            LineMemberData theLineData = TryGetValue(strMemberId);
            if (theLineData == null)
            {
                theLineData = new LineMemberData(strMemberId);
                Add(theLineData);
            }
            return theLineData;
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, String.Format("You have an empty collection of {0} that you're trying to serialize", CollectionElementName));
                XElement elem = new XElement(CollectionElementName);
                foreach (LineMemberData aLineData in this)
                    aLineData.AddXml(elem, InstanceElementName);

                return elem;
            }
        }

        public void IndexSearch(VerseData.SearchLookInProperties findProperties,
            ref VerseData.StringTransferSearchIndex lstBoxesToSearch)
        {
            if (IsLookInPropertySet(findProperties))
                foreach (LineMemberData aLineData in this)
                    aLineData.IndexSearch(findProperties, AssociatedViewMenu, ref lstBoxesToSearch);
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
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop,
                                            100/nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i,
                                                                     LineData.CstrAttributeLangVernacular),
                                                          StoryData.CstrLangVernacularStyleClassName,
                                                          theLine.Vernacular));
                }

                if (bShowNationalBT)
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop,
                                            100/nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i,
                                                                     LineData.CstrAttributeLangNationalBt),
                                                          StoryData.CstrLangNationalBtStyleClassName,
                                                          theLine.NationalBt));
                }

                if (bShowEnglishBT)
                {
                    strRow += String.Format(OseResources.Properties.Resources.HTML_TableCellWidthAlignTop,
                                            100/nNumTestQuestionCols,
                                            String.Format(OseResources.Properties.Resources.HTML_Textarea,
                                                          TextareaId(InstanceElementName, nVerseIndex, i,
                                                                     LineData.CstrAttributeLangInternationalBt),
                                                          StoryData.CstrLangInternationalBtStyleClassName,
                                                          theLine.InternationalBt));
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
                LineMemberData theParentLineData = this[i];
                string strMemberId = theParentLineData.MemberId;
                nTestNum = astrTestors.IndexOf(strMemberId);

                bool bFound = false;
                LineMemberData theChildLineData = null;

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
                    strVernacular = Diff.HtmlDiff(theParentLineData.Vernacular,
                                                  theChildLineData.Vernacular);
                    strNationalBT = Diff.HtmlDiff(theParentLineData.NationalBt,
                                                  theChildLineData.NationalBt);
                    strInternationalBT = Diff.HtmlDiff(theParentLineData.InternationalBt,
                                                       theChildLineData.InternationalBt);
                }

                // but if there was a child and yet we didn't find it...
                // OR if there wasn't a child, but there should have been (because we're processing with a child)
                else if (((child != null) || !bPrintPreview) && !bProcessingTheChild)
                {
                    // it means that the parent was deleted.
                    strVernacular = Diff.HtmlDiff(theParentLineData.Vernacular, null);
                    strNationalBT = Diff.HtmlDiff(theParentLineData.NationalBt, null);
                    strInternationalBT = Diff.HtmlDiff(theParentLineData.InternationalBt, null);
                }

                // this means there is a child and we're processing it here as if it were the parent
                //  (so that implicitly means this is an addition)
                else if (bProcessingTheChild)
                {
                    strVernacular = Diff.HtmlDiff(null, theParentLineData.Vernacular);
                    strNationalBT = Diff.HtmlDiff(null, theParentLineData.NationalBt);
                    strInternationalBT = Diff.HtmlDiff(null, theParentLineData.InternationalBt);
                }

                // otherwise, if there was no child (e.g. just doing a print preview of one version)...
                else
                {
                    // then the parent's value is the value
                    strVernacular = theParentLineData.Vernacular.ToString();
                    strNationalBT = theParentLineData.NationalBt.ToString();
                    strInternationalBT = theParentLineData.InternationalBt.ToString();
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
                    LineMemberData theChildLineData = child[j];
                    string strMemberId = theChildLineData.MemberId;
                    nTestNum = astrTestors.IndexOf(strMemberId);
                    string strVernacular = Diff.HtmlDiff(null, theChildLineData.Vernacular);
                    string strNationalBT = Diff.HtmlDiff(null, theChildLineData.NationalBt);
                    string strInternationalBT = Diff.HtmlDiff(null, theChildLineData.InternationalBt);
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
                LineMemberData theLineData = this[i];
                string strMemberId = theLineData.MemberId;
                nTestNum = astrTestors.IndexOf(strMemberId);
                string strVernacular = Diff.HtmlDiff(null, theLineData.Vernacular);
                string strNationalBT = Diff.HtmlDiff(null, theLineData.NationalBt);
                string strEnglishBT = Diff.HtmlDiff(null, theLineData.InternationalBt);
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
                                                                 LineData.CstrAttributeLangVernacular),
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
                                                                 LineData.CstrAttributeLangNationalBt),
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
                                                                 LineData.CstrAttributeLangInternationalBt),
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
        public RetellingsData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
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

        protected override bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties)
        {
            return findProperties.Retellings;
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

            foreach (NewDataSet.AnswerRow anAnswerRow in theAnswersRow.GetAnswerRows())
            {
                string strLangId = (anAnswerRow.IslangNull()) ? null : anAnswerRow.lang;
                string strValue = (anAnswerRow.IsAnswer_textNull()) ? "" : anAnswerRow.Answer_text;
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

        protected override bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties)
        {
            return findProperties.TestAs;
        }
    }
}
