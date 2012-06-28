using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class TextAreaIdentifier
    {
        public int LineIndex { get; set; }
        public int ItemIndex { get; set; }
        public int SubItemIndex { get; set; }

        public string FieldType { get; set; }
        public string LanguageColumn { get; set; }

        public string HtmlIdentifier
        {
            get
            {
                return StringTransfer.TextareaId(LineIndex, FieldType, ItemIndex, SubItemIndex,
                                                 LanguageColumn);
            }
        }

        public string FieldReference
        {
            get
            {
                var str = FieldType;
                if ((FieldType == StoryEditor.TextFields.Retelling.ToString()) ||
                    (FieldType == StoryEditor.TextFields.TestQuestionAnswer.ToString()))
                    str = String.Format("{0} {1}", FieldType.Substring(0, 3).ToLower(), SubItemIndex + 1);
                return str + ":";
            }
        }
    }
}
