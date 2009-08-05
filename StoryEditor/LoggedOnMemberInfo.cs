using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class LoggedOnMemberInfo
    {
        public LoggedOnMemberInfo(string strMemberName, string strMemberGuid, StoryEditor.UserTypes eType)
        {
            MemberName = strMemberName;
            MemberGuid = strMemberGuid;
            Type = eType;
        }

        public string MemberName;
        public string MemberGuid;
        public StoryEditor.UserTypes Type;
    }
}
