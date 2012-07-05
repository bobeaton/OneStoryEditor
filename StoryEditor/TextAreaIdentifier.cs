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
                var strFormat = FieldType;
                if (FieldType == StoryEditor.TextFields.Retelling.ToString())
                    strFormat = RetellingsData.RetellingLabelFormat;
                else if (FieldType == StoryEditor.TextFields.TestQuestionAnswer.ToString())
                    strFormat = AnswersData.AnswersLabelFormat;
                else
                    return strFormat + " :";
                return String.Format(strFormat, SubItemIndex + 1);
            }
        }

        public string GetKeyboardName(ProjectSettings projSettings)
        {
            StoryEditor.TextFields langType;
            if (Enum.TryParse(LanguageColumn, out langType))
            switch (langType)
            {
                case StoryEditor.TextFields.Vernacular:
                    return projSettings.Vernacular.Keyboard;
                case StoryEditor.TextFields.NationalBt:
                    return projSettings.NationalBT.Keyboard;
                case StoryEditor.TextFields.InternationalBt:
                    return projSettings.InternationalBT.Keyboard;
                case StoryEditor.TextFields.FreeTranslation:
                    return projSettings.FreeTranslation.Keyboard;
            }
            return null;
        }
    }
}
