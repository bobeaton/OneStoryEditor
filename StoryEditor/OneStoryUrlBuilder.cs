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
        protected const string CstrStorySet = "StorySet";
        protected const string CstrStoryId = "StoryId";
        protected const string CstrLineId = "LineId";
        protected const string CstrType = "Type";
        protected const string CstrTypeId = "TypeId";
        protected const string CstrTypeValue = "TypeValue";

        public static string UrlProjectNote(string strProjectName)
        {
            return String.Format("{0}project?id={1}&label=Project%20Note", CstrOneStoryUrlHeader, strProjectName);
        }

        // OneStory URL syntax
        // onestory://<projectname>?StorySet=<storysetname>&StoryId=<storyguid>&LineId=<verseguid>&Type=<fieldtype>&TypeId=<typeguid>&TypeValue=<string>
        // where:
        //  projectname = e.g. snwmtn-test
        //  storysetname = e.g. "Stories" (for the main biblical stories)
        //  fieldtype = oneof{Vernacular, NationalBT, EnglishBT, Anchor, ...}
        //  num = which instances (e.g. the 1st retelling = 0)
        //  string = portion of the phrase/string that was selected
        // this method is for constructing a single FieldInfo item
        public static string Url(string strProjectName, string strStorySet,
            string strStoryGuid, string strVerseGuid, FieldType eFieldType,
            string strItemId, string strPhrase)
        {
            string strUrl;
            if (!String.IsNullOrEmpty(strItemId))
                strUrl = String.Format("onestory://{0}?StorySet={1}&StoryId={2}&LineId={3}&Type={4}&TypeId={5}&TypeValue={6}",
                                       strProjectName, strStorySet, strStoryGuid, strVerseGuid,
                                       eFieldType, strItemId, strPhrase);
            else
                strUrl = String.Format("onestory://{0}?StorySet={1}&StoryId={2}&LineId={3}&Type={4}",
                                       strProjectName, strStorySet, strStoryGuid, strVerseGuid,
                                       eFieldType);

            var parse = System.Web.HttpUtility.ParseQueryString(strUrl);
            Debug.Assert(parse.AllKeys[0] == CstrOneStoryUrlHeader + strProjectName + "?" + CstrStorySet);
            Debug.Assert(parse.GetValues(CstrOneStoryUrlHeader + strProjectName + "?" + CstrStorySet)[0] == strStorySet);
            Debug.Assert(parse.AllKeys[1] == CstrStoryId);
            Debug.Assert(parse.GetValues(CstrStoryId)[0] == strStoryGuid);
            Debug.Assert(parse.AllKeys[2] == CstrLineId);
            Debug.Assert(parse.GetValues(CstrLineId)[0] == strVerseGuid);
            Debug.Assert(parse.AllKeys[3] == CstrType);
            Debug.Assert(parse.GetValues(CstrType)[0] == eFieldType.ToString());
            if (!String.IsNullOrEmpty(strItemId))
            {
                Debug.Assert(parse.AllKeys[4] == CstrTypeId);
                Debug.Assert(parse.GetValues(CstrTypeId)[0] == strItemId);
                Debug.Assert(parse.AllKeys[5] == CstrTypeValue);
                Debug.Assert(parse.GetValues(CstrTypeValue)[0] == strPhrase);
            }

            return strUrl;
        }

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
