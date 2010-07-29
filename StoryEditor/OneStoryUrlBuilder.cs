using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class OneStoryUrlBuilder
    {
        protected const string CstrOneStoryUrlHeader = "onestory://";
        protected const string CstrStoryId = "StoryId";
        protected const string CstrLineId = "LineId";
        protected const string CstrType = "Type";
        protected const string CstrTypeId = "TypeId";
        protected const string CstrTypeValue = "TypeValue";

        // OneStory URL syntax
        // onestory://<projectname>?StoryId=<storyguid>&LineId=<verseguid>&Type=<fieldtype>&TypeId=<typeguid>&TypeValue=<string>
        // where:
        //  projectname = e.g. snwmtn-test
        //  fieldtype = oneof{Vernacular, NationalBT, EnglishBT, Anchor, ...}
        //  num = which instances (e.g. the 1st retelling = 0)
        //  string = portion of the phrase/string that was selected
        // this method is for constructing a single FieldInfo item
        public static string Url(string strProjectName,
            string strStoryGuid, string strVerseGuid, FieldType eFieldType,
            string strItemId, string strPhrase)
        {
            string strUrl;
            if (!String.IsNullOrEmpty(strItemId))
                strUrl = String.Format("onestory://{0}?StoryId={1}&LineId={2}&Type={3}&TypeId={4}&TypeValue={5}",
                                       strProjectName, strStoryGuid, strVerseGuid,
                                       eFieldType, strItemId, strPhrase);
            else
                strUrl = String.Format("onestory://{0}?StoryId={1}&LineId={2}&Type={3}",
                                       strProjectName, strStoryGuid, strVerseGuid,
                                       eFieldType);

            var parse = System.Web.HttpUtility.ParseQueryString(strUrl);
            Debug.Assert(parse.AllKeys[0] == CstrOneStoryUrlHeader + strProjectName + "?" + CstrStoryId);
            Debug.Assert(parse.GetValues(CstrOneStoryUrlHeader + strProjectName + "?" + CstrStoryId)[0] == strStoryGuid);
            Debug.Assert(parse.AllKeys[1] == CstrLineId);
            Debug.Assert(parse.GetValues(CstrLineId)[0] == strVerseGuid);
            Debug.Assert(parse.AllKeys[2] == CstrType);
            Debug.Assert(parse.GetValues(CstrType)[0] == eFieldType.ToString());
            if (!String.IsNullOrEmpty(strItemId))
            {
                Debug.Assert(parse.AllKeys[3] == CstrTypeId);
                Debug.Assert(parse.GetValues(CstrTypeId)[0] == strItemId);
                Debug.Assert(parse.AllKeys[4] == CstrTypeValue);
                Debug.Assert(parse.GetValues(CstrTypeValue)[0] == strPhrase);
            }
            /*
#if DEBUG
            string strTestProjectName, strTestStoryGuid, strTestVerseGuid;
            List<FieldInfo> lstFields;
            ParseOneStoryUrl(strUrl, out strTestProjectName, out strTestStoryGuid,
                             out strTestVerseGuid, out lstFields);
            Debug.Assert(strProjectName == strTestProjectName);
            Debug.Assert(strStoryGuid == strTestStoryGuid);
            Debug.Assert(strVerseGuid == strTestVerseGuid);
            Debug.Assert(lstFields[0].FieldType == eFieldType);
            Debug.Assert(lstFields[0].ItemId == strItemId);
            Debug.Assert(lstFields[0].Phrase == strPhrase);
#endif
            */
            return strUrl;
        }
        /*
        public static bool ParseOneStoryUrl(string strUrl, out string strProjectName,
            out string strStoryGuid, out string strVerseGuid, out List<FieldInfo> lstFields)
        {
            try
            {
                var parse = System.Web.HttpUtility.ParseQueryString(strUrl);
                int nIndex = parse.AllKeys[0].IndexOf("?");
                strProjectName = parse.AllKeys[0].Substring(CstrOneStoryUrlHeader.Length,
                                                            nIndex - CstrOneStoryUrlHeader.Length);
                strStoryGuid = parse.GetValues(CstrOneStoryUrlHeader + strProjectName + "?" + CstrStoryId)[0];
                strVerseGuid = parse.GetValues(CstrLineId)[0];
                string[] astrTypes = parse.GetValues(CstrType);
                string[] astrTypeIds = parse.GetValues(CstrTypeId);
                string[] astrTypeVals = parse.GetValues(CstrTypeValue);

                Debug.Assert((astrTypes.Length == astrTypeIds.Length)
                    && (astrTypeIds.Length == astrTypeVals.Length));

                lstFields = new List<FieldInfo>(astrTypes.Length);
                for (int i = 0; i < astrTypes.Length; i++)
                {
                    lstFields.Add(new FieldInfo(GetFieldTypeFromString(astrTypes[i]), 
                        astrTypeIds[i], astrTypeVals[i]));
                }

                return true;
            }
            catch (Exception ex)
            {
                Palaso.Reporting.ErrorReport.ReportNonFatalException(
                    new ApplicationException(String.Format("Unable to parse URL: {0}", strUrl), ex));
            }

            strProjectName = null;
            strStoryGuid = null;
            strVerseGuid = null;
            lstFields = null;
            return false;
        }
        */

        // this method is for constructing multiple FieldInfo items
        public static string Url(string strProjectName,
            string strStoryGuid, string strVerseGuid, List<FieldInfo> lstFields)
        {
            string str = String.Format("onestory://{0}?StoryId={1}&LineId={2}",
                strProjectName, strStoryGuid, strVerseGuid);

            foreach (FieldInfo fieldInfo in lstFields)
            {
                str += String.Format("&Type={0}&TypeId={1}&TypeValue={2}",
                                     fieldInfo.FieldType, fieldInfo.ItemId, fieldInfo.Phrase);
            }

            return str;
        }

        public enum FieldType
        {
            eVernacularLangField,
            eNationalLangField,
            eEnglishBTField,
            eAnchorFields,
            eStoryQuestionFields,
            eStoryAnswerFields,
            eRetellingFields,
            eConsultantNoteFields,
            eCoachNotesFields
        }

        public class FieldInfo
        {
            public FieldType FieldType { get; set; }
            public string ItemId { get; set; }
            public string Phrase { get; set; }

            public FieldInfo(FieldType eType, string itemId, string str)
            {
                FieldType = eType;
                ItemId = itemId;
                Phrase = str;
            }
        }

        public static FieldType GetFieldTypeFromString(string str)
        {
            return (FieldType)Enum.Parse(typeof(FieldType), str);
        }
        /*
        public enum Domain
        {
            eConsultantNote,
            eCoachNote,
            eConflict,
            eKeytermDenial
        }

        public static Domain GetDomainFromString(string str)
        {
            return (Domain)Enum.Parse(typeof(Domain), str);
        }
        */
    }
}
