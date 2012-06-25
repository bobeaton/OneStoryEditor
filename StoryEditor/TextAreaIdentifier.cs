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
    }
}
