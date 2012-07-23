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

        public string FieldTypeName { get; set; }
        public string LanguageColumnName { get; set; }

        public string HtmlIdentifier
        {
            get
            {
                return StringTransfer.TextareaId(LineIndex, FieldTypeName, ItemIndex, SubItemIndex,
                                                 LanguageColumnName);
            }
        }

        public StoryEditor.TextFields FieldType
        {
            get { return (StoryEditor.TextFields) Enum.Parse(typeof (StoryEditor.TextFields), FieldTypeName); }
        }

        public StoryEditor.TextFields LanguageColumn
        {
            get { return (StoryEditor.TextFields) Enum.Parse(typeof(StoryEditor.TextFields), LanguageColumnName); }
        }

        public string FieldReferenceName
        {
            get
            {
                var strFormat = FieldTypeName;
                if (FieldTypeName == StoryEditor.TextFields.Retelling.ToString())
                    strFormat = RetellingsData.RetellingLabelFormat;
                else if (FieldTypeName == StoryEditor.TextFields.TestQuestionAnswer.ToString())
                    strFormat = AnswersData.AnswersLabelFormat;
                else
                    return strFormat + " :";
                return String.Format(strFormat, SubItemIndex + 1);
            }
        }

        public ProjectSettings.LanguageInfo GetLanguageInfo(ProjectSettings projSettings)
        {
            switch (LanguageColumn)
            {
                case StoryEditor.TextFields.Vernacular:
                    return projSettings.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return projSettings.NationalBT;
                case StoryEditor.TextFields.InternationalBt:
                    return projSettings.InternationalBT;
                case StoryEditor.TextFields.FreeTranslation:
                    return projSettings.FreeTranslation;
                default:
                    System.Diagnostics.Debug.Fail("wasn't expecting this!");
                    return projSettings.InternationalBT;
            }
        }
        /*
        public string GetKeyboardName(ProjectSettings projSettings)
        {
            switch (LanguageColumn)
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
        */
    }
}
