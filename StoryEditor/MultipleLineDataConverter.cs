using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    // class encapsulating one retelling or TQ answer possibly in multiple languages
    //  the List-ness of this is that there may be 3 StringTransfers for each of
    //  the StoryEditor.TextFields types
    public class LineMemberData : LineData
    {
        public string MemberId { get; set; }

        public LineMemberData(LineMemberData rhs, StoryEditor.TextFields whichField)
            : base(rhs, whichField)
        {
            MemberId = rhs.MemberId;
        }

        public LineMemberData(string strMemberId, StoryEditor.TextFields whichField)
            : base(whichField)
        {
            MemberId = strMemberId;
        }

        public const string CstrAttributeMemberID = "memberID";

        public override void AddXml(XElement elem, string strFieldName)
        {
            // Can't not write these! The only thing we could do is decide on the basis
            //  of whether there project settings say to save in retelling or not, so 
            //  until we decide to pass that information around, just write them all
            // if (!Vernacular.IsNull)
            elem.Add(new XElement(strFieldName,
                                  new XAttribute(CstrAttributeLang, CstrAttributeLangVernacular),
                                  new XAttribute(CstrAttributeMemberID, MemberId),
                                  Vernacular.IsNull
                                      ? null
                                      : Vernacular.ToString()));
            // if (!NationalBt.IsNull)
            elem.Add(new XElement(strFieldName,
                                  new XAttribute(CstrAttributeLang, CstrAttributeLangNationalBt),
                                  new XAttribute(CstrAttributeMemberID, MemberId),
                                  NationalBt.IsNull
                                      ? null
                                      : NationalBt.ToString()));
            // if (!InternationalBt.IsNull)
            elem.Add(new XElement(strFieldName,
                                  new XAttribute(CstrAttributeLang, CstrAttributeLangInternationalBt),
                                  new XAttribute(CstrAttributeMemberID, MemberId),
                                  InternationalBt.IsNull
                                      ? null
                                      : InternationalBt.ToString()));
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
        }
    }

    public abstract class MultipleLineDataConverter : List<LineMemberData>
    {
        public abstract string CollectionElementName { get; }
        protected abstract string InstanceElementName { get; }
        public abstract string LabelTextFormat { get; }
        protected abstract VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu { get; }
        protected abstract bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties);
        protected abstract StoryEditor.TextFields WhichField { get; }

        protected MultipleLineDataConverter(IEnumerable<LineMemberData> rhs)
        {
            foreach (LineMemberData aLineData in rhs)
                Add(new LineMemberData(aLineData, WhichField));
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
                theLineData = new LineMemberData(strMemberId, WhichField);
                Add(theLineData);
            }
            return theLineData;
        }

        public XElement GetXml
        {
            get
            {
                var elem = new XElement(CollectionElementName);
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

        public string PresentationHtml(int nVerseIndex, int nNumCols, int nParentNum,
            TestInfo astrTesters,
            MultipleLineDataConverter child,
            StoryData.PresentationType presentationType, 
            bool bProcessingTheChild,
            bool bShowVernacular, 
            bool bShowNationalBT, 
            bool bShowInternationalBT,
            VerseData.ViewSettings viewSettings,
            TeamMembersData teamMembersData)
        {
            string strRow = null;
            int nTestNum;
            for (int i = 0; i < Count; i++)
            {
                // only display this retelling, if it isn't being excluded by a list of indices to display
                var theParentLineData = this[i];
                var strMemberId = theParentLineData.MemberId;
                if ((astrTesters.ListOfMemberIdsToDisplay != null) 
                    && (astrTesters.ListOfMemberIdsToDisplay.Count > 0)
                    && !astrTesters.ListOfMemberIdsToDisplay.Contains(strMemberId))
                    continue;

                nTestNum = astrTesters.IndexOf(strMemberId);

                // check to see if this testee was there... if not, then add it (it must have been lost in a merge or something)
                if (nTestNum == -1)
                {
                    astrTesters.Add(new MemberIdInfo(strMemberId, Localizer.Str("the main entry for this test was remove, but has been added back by OSE since there's still data for it. If you don't want it, you can try deleting it again (beware a Send/Receive merge might bring it back if someone else edited it)")));
                    nTestNum = astrTesters.IndexOf(strMemberId);
                }

                var strUnsName = teamMembersData.GetNameFromMemberId(strMemberId);

                bool bFound = false;
                LineMemberData theChildLineData = null;

                if (child != null)
                {
                    theChildLineData = child.TryGetValue(strMemberId);
                    if (theChildLineData != null)
                    {
                        child.Remove(theChildLineData);
                        bFound = true;
                    }
                }

                string strVernacular, strNationalBt, strInternationalBt;
                // if we found it, it means there was a child version... 
                if (bFound)
                {
                    // so diff them
                    strVernacular = Diff.HtmlDiff(viewSettings.TransliteratorVernacular,
                                                  theParentLineData.Vernacular,
                                                  theChildLineData.Vernacular);
                    strNationalBt = Diff.HtmlDiff(viewSettings.TransliteratorNationalBT,
                                                  theParentLineData.NationalBt,
                                                  theChildLineData.NationalBt);
                    strInternationalBt = Diff.HtmlDiff(viewSettings.TransliteratorInternationalBt,
                                                       theParentLineData.InternationalBt,
                                                       theChildLineData.InternationalBt);
                }

                // but if there was a child and yet we didn't find it...
                // OR if there wasn't a child, but there should have been (because we're processing with a child)
                else if (((child != null) || (presentationType == StoryData.PresentationType.Differencing)) 
                    && !bProcessingTheChild)
                {
                    // it means that the parent was deleted.
                    strVernacular = Diff.HtmlDiff(viewSettings.TransliteratorVernacular,
                                                  theParentLineData.Vernacular, 
                                                  null);
                    strNationalBt = Diff.HtmlDiff(viewSettings.TransliteratorNationalBT,
                                                  theParentLineData.NationalBt, 
                                                  null);
                    strInternationalBt = Diff.HtmlDiff(viewSettings.TransliteratorInternationalBt,
                                                       theParentLineData.InternationalBt,
                                                       null);
                }

                // this means there is a child and we're processing it here as if it were the parent
                //  (so that implicitly means this is an addition)
                else if (bProcessingTheChild)
                {
                    strVernacular = Diff.HtmlDiff(viewSettings.TransliteratorVernacular,
                                                  null, 
                                                  theParentLineData.Vernacular);
                    strNationalBt = Diff.HtmlDiff(viewSettings.TransliteratorNationalBT,
                                                  null,
                                                  theParentLineData.NationalBt);
                    strInternationalBt = Diff.HtmlDiff(viewSettings.TransliteratorInternationalBt,
                                                       null,
                                                       theParentLineData.InternationalBt);
                }

                // otherwise, if there was no child (e.g. just doing a print preview of one version)...
                else
                {
                    // then the parent's value is the value
                    strVernacular = theParentLineData.Vernacular.GetValue(viewSettings.TransliteratorVernacular);
                    strNationalBt = theParentLineData.NationalBt.GetValue(viewSettings.TransliteratorNationalBT);
                    strInternationalBt = theParentLineData.InternationalBt.GetValue(viewSettings.TransliteratorInternationalBt);
                }

                strRow += PresentationHtmlRow(nVerseIndex, nParentNum, nTestNum, strUnsName,
                    strVernacular, strNationalBt, strInternationalBt,
                    bShowVernacular, bShowNationalBT, bShowInternationalBT,
                    theParentLineData, viewSettings);
            }

            // finally, everything that is left in the child is new
            if ((child != null) && (child.Count > 0))
            {
                for (int j = 0; j < child.Count; j++)
                {
                    var theChildLineData = child[j];
                    var strMemberId = theChildLineData.MemberId;
                    nTestNum = astrTesters.IndexOf(strMemberId);
                    var strUnsName = teamMembersData.GetNameFromMemberId(strMemberId);

                    string strVernacular = Diff.HtmlDiff(viewSettings.TransliteratorVernacular,
                                                         null, 
                                                         theChildLineData.Vernacular);
                    string strNationalBT = Diff.HtmlDiff(viewSettings.TransliteratorNationalBT,
                                                         null, 
                                                         theChildLineData.NationalBt);
                    string strInternationalBT = Diff.HtmlDiff(viewSettings.TransliteratorInternationalBt,
                                                              null,
                                                              theChildLineData.InternationalBt);
                    strRow += PresentationHtmlRow(nVerseIndex, nParentNum, nTestNum, strUnsName,
                                                  strVernacular, strNationalBT, strInternationalBT,
                                                  bShowVernacular, bShowNationalBT, bShowInternationalBT,
                                                  theChildLineData, viewSettings);
                }
            }

            if (!String.IsNullOrEmpty(strRow))
            {
                // make a sub-table out of all this
                strRow = String.Format(Properties.Resources.HTML_TableRow,
                                        String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                      String.Format(Properties.Resources.HTML_Table,
                                                                    strRow)));
            }
            return strRow;
        }


        public string PresentationHtmlAsAddition(int nVerseIndex, int nNumCols, int nParentNum, TestInfo astrTesters,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT,
            VerseData.ViewSettings viewSettings, TeamMembersData teamMembersData)
        {
            string strRow = null;
            for (int i = 0; i < Count; i++)
            {
                var theLineData = this[i];
                var strMemberId = theLineData.MemberId;
                var nTestNum = astrTesters.IndexOf(strMemberId);
                var strUnsName = teamMembersData.GetNameFromMemberId(strMemberId);

                string strVernacular = Diff.HtmlDiff(viewSettings.TransliteratorVernacular,
                                                     null,
                                                     theLineData.Vernacular);
                string strNationalBT = Diff.HtmlDiff(viewSettings.TransliteratorNationalBT,
                                                     null,
                                                     theLineData.NationalBt);
                string strEnglishBT = Diff.HtmlDiff(viewSettings.TransliteratorInternationalBt,
                                                    null,
                                                    theLineData.InternationalBt);
                strRow += PresentationHtmlRow(nVerseIndex, nParentNum, nTestNum, strUnsName,
                                              strVernacular, strNationalBT, strEnglishBT,
                                              bShowVernacular, bShowNationalBT, bShowInternationalBT,
                                              theLineData, viewSettings);
            }

            if (!String.IsNullOrEmpty(strRow))
            {
                // make a sub-table out of all this
                strRow = String.Format(Properties.Resources.HTML_TableRow,
                                        String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                      String.Format(Properties.Resources.HTML_Table,
                                                                    strRow)));
            }
            return strRow;
        }

        protected string PresentationHtmlRow(int nVerseIndex, int nItemNum, int nSubItemNum, string strUnsName,
            string strVernacular, string strNationalBT, string strInternationalBT,
            bool bShowVernacular, bool bShowNationalBT, bool bShowInternationalBT,
            LineMemberData theLineOfData, VerseData.ViewSettings viewSettings)
        {
            // was: HTML_TableCell (after being HTML_TableCellNoWrap, but I can't figure out why
            //  I took that off)
            string strRow = String.Format(Properties.Resources.HTML_TableCellNoWrapWithToolTip,
                                          strUnsName,
                                          String.Format(LabelTextFormat, nSubItemNum + 1));

            int nNumCols = 0;
            if (bShowVernacular) nNumCols++;
            if (bShowNationalBT) nNumCols++;
            if (bShowInternationalBT) nNumCols++;

            if (bShowVernacular)
            {
                strRow += theLineOfData.Vernacular.FormatLanguageColumnHtml(nVerseIndex,
                                                                            nItemNum,
                                                                            nSubItemNum,
                                                                            nNumCols,
                                                                            strVernacular,
                                                                            viewSettings);
            }

            if (bShowNationalBT)
            {
                strRow += theLineOfData.NationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                            nItemNum,
                                                                            nSubItemNum,
                                                                            nNumCols,
                                                                            strNationalBT,
                                                                            viewSettings);
            }

            if (bShowInternationalBT)
            {
                strRow += theLineOfData.InternationalBt.FormatLanguageColumnHtml(nVerseIndex,
                                                                                 nItemNum,
                                                                                 nSubItemNum,
                                                                                 nNumCols,
                                                                                 strInternationalBT,
                                                                                 viewSettings);
            }

            // make a sub-table out of all this
            // TODO: I think nNumCols here is wrong. It should be the column count of the parent
            strRow = String.Format(Properties.Resources.HTML_TableRow,
                                    String.Format(Properties.Resources.HTML_TableCellWithSpan, nNumCols,
                                                  String.Format(Properties.Resources.HTML_Table,
                                                                strRow)));
            return strRow;
        }

        public void ReplaceUns(string strOldUnsGuid, string strNewUnsGuid)
        {
            // shouldn't already have the new one (or we'll get duplicates, which can't
            //  be rectified)
            System.Diagnostics.Debug.Assert(TryGetValue(strNewUnsGuid) == null);

            var theLine = TryGetValue(strOldUnsGuid);
            if ((theLine != null) && (theLine.MemberId == strOldUnsGuid))
                theLine.MemberId = strNewUnsGuid;
        }

        public bool DoesReferenceUns(string strMemberId)
        {
            return (TryGetValue(strMemberId) != null);
        }

        public void RemoveTestResult(string strUnsGuid)
        {
            // even the verse itself may be newer and only have a single retelling (compared
            //  with multiple retellings for verses that we're present from draft 1)
            var theLineData = TryGetValue(strUnsGuid);
            if (theLineData != null)
                Remove(theLineData);
        }
    }

    public class RetellingsData : MultipleLineDataConverter
    {
        public RetellingsData(NewDataSet.VerseRow theVerseRow, NewDataSet projFile)
        {
            var theRetellingsRows = theVerseRow.GetRetellingsRows();
            var theRetellingsRow = (theRetellingsRows.Length == 0)
                                       ? projFile.Retellings.AddRetellingsRow(theVerseRow)
                                       : theRetellingsRows[0];

            foreach (var aRetellingRow in theRetellingsRow.GetRetellingRows())
            {
                var strLangId = (aRetellingRow.IslangNull())
                                    ? null
                                    : aRetellingRow.lang;
                var strValue = (aRetellingRow.IsRetelling_textNull())
                                   ? null
                                   : aRetellingRow.Retelling_text;
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
        public const string CstrElementLableRetelling = "Retelling";
        
        public static string RetellingLabelFormat
        {
            get { return Localizer.Str("ret {0}:"); }
        }

        public override sealed string CollectionElementName
        {
            get { return CstrElementLableRetellings; }
        }

        protected override sealed string InstanceElementName
        {
            get { return CstrElementLableRetelling; }
        }

        public override string LabelTextFormat
        {
            get { return RetellingLabelFormat; }
        }

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.RetellingFields; }
        }

        protected override bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties)
        {
            return findProperties.Retellings;
        }

        protected override StoryEditor.TextFields WhichField
        {
            get { return StoryEditor.TextFields.Retelling; }
        }

        public void SwapColumns(StoryEditor.TextFields column1, StoryEditor.TextFields column2)
        {
            ForEach(r => r.SwapColumns(column1, column2));
        }
    }

    public class AnswersData : MultipleLineDataConverter
    {
        public AnswersData(NewDataSet.TestQuestionRow theTestQuestionRow, NewDataSet projFile)
        {
            var theAnswersRows = theTestQuestionRow.GetAnswersRows();
            var theAnswersRow = (theAnswersRows.Length == 0)
                                    ? projFile.Answers.AddAnswersRow(theTestQuestionRow)
                                    : theAnswersRows[0];

            foreach (var anAnswerRow in theAnswersRow.GetAnswerRows())
            {
                var strLangId = (anAnswerRow.IslangNull())
                                    ? null
                                    : anAnswerRow.lang;
                var strValue = (anAnswerRow.IsAnswer_textNull())
                                   ? null
                                   : anAnswerRow.Answer_text;
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
        public const string CstrElementLableAnswer = "Answer";
        
        public static string AnswersLabelFormat
        {
            get { return Localizer.Str("ans {0}:"); }
        }

        public override sealed string CollectionElementName
        {
            get { return CstrElementLableAnswers; }
        }

        protected override sealed string InstanceElementName
        {
            get { return CstrElementLableAnswer; }
        }

        public override string LabelTextFormat
        {
            get { return AnswersLabelFormat; }
        }

        protected override VerseData.ViewSettings.ItemToInsureOn AssociatedViewMenu
        {
            get { return VerseData.ViewSettings.ItemToInsureOn.StoryTestingQuestionAnswers; }
        }

        protected override bool IsLookInPropertySet(VerseData.SearchLookInProperties findProperties)
        {
            return findProperties.TestAs;
        }

        protected override StoryEditor.TextFields WhichField
        {
            get { return StoryEditor.TextFields.TestQuestionAnswer; }
        }

        public void SwapColumns(StoryEditor.TextFields column1, StoryEditor.TextFields column2)
        {
            ForEach(a => a.SwapColumns(column1, column2));
        }
    }
}
