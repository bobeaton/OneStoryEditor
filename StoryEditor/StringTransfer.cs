using System;
using OneStoryProjectEditor.Properties;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    // this class is used for all the string data in our *Data classes, so that the 
    //  controls they're associated with will be able to automatically update themselves
    public class StringTransfer
    {
        protected string Value;
#if UsingHtmlDisplayForStoryBt
        public StoryEditor.TextFields WhichField;

        public string FormatLanguageColumnHtml(int nVerseIndex, 
            int nItemNum, 
            int nNumCols,
            string strValue,
            bool bUseTextAreas)
        {
            if (!bUseTextAreas && String.IsNullOrEmpty(strValue))
                strValue = "-";  // just so there's something there (or the cell doesn't show)

            string strHtmlElement;
            if (bUseTextAreas)
            {
                strHtmlElement = String.Format(Resources.HTML_Textarea,
                                               GetHtmlElementId(nVerseIndex, nItemNum, bUseTextAreas),
                                               GetStyleClassName,
                                               GetLanguageType,
                                               strValue);
            }
            else
            {
                strHtmlElement = String.Format(Resources.HTML_ParagraphText,
                                               GetStyleClassName,
                                               strValue);
            }

            return String.Format(Resources.HTML_TableCellWidthAlignTop,
                                 100/nNumCols,
                                 strHtmlElement);
        }

        private string GetHtmlElementId(int nVerseIndex, int nItemNum, bool bUseTextAreas)
        {
            var strLanguageType = GetLanguageType;
            var strId = (bUseTextAreas)
                            ? TextareaId(nVerseIndex,
                                         GetFieldType,
                                         nItemNum,
                                         strLanguageType)
                            : TextParagraphId(nVerseIndex,
                                              GetFieldType,
                                              nItemNum,
                                              strLanguageType);
            return strId;
        }

        private string GetStyleClassName
        {
            get
            {
                System.Diagnostics.Debug.Assert(WhichField != StoryEditor.TextFields.Undefined);
                return "Lang" + GetLanguageType;
            }
        }

        private string GetFieldType
        {
            get
            {
                System.Diagnostics.Debug.Assert(WhichField != StoryEditor.TextFields.Undefined);
                return (WhichField & StoryEditor.TextFields.Fields).ToString();
            }
        }

        private string GetLanguageType
        {
            get
            {
                System.Diagnostics.Debug.Assert(WhichField != StoryEditor.TextFields.Undefined);
                return (WhichField & StoryEditor.TextFields.Languages).ToString();
            }
        }

        /// <summary>
        /// Gets a unique id for a text area. a combination of...
        /// </summary>
        /// <param name="nVerseIndex">indicates verse number (0-based)</param>
        /// <param name="strPrefix">indicates the data--e.g. StoryLine vs. Retelling, etc</param>
        /// <param name="nItemNum">indicates a sub-item number for certain types (e.g. ret *2*)</param>
        /// <param name="strFieldTypeName">indicates the language of the field (e.g. vernacular)</param>
        /// <returns></returns>
        public static string TextareaId(int nVerseIndex, string strPrefix, int nItemNum, string strFieldTypeName)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}", HtmlVerseControl.CstrTextAreaPrefix,
                                 nVerseIndex, strPrefix, nItemNum, strFieldTypeName);
        }

        public static string TextParagraphId(int nVerseIndex, string strPrefix, int nItemNum, string strFieldTypeName)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}", HtmlVerseControl.CstrParagraphPrefix,
                                 nVerseIndex, strPrefix, nItemNum, strFieldTypeName);
        }

#else
#if !DataDllBuild
        protected CtrlTextBox _tb = null;
#endif
#endif
        public DirectableEncConverter Transliterator { get; set; }

        public StringTransfer(string strValue, StoryEditor.TextFields eWhichField)
        {
            WhichField = eWhichField;
            SetValue(strValue);
        }

        public void SetValue(string strValue)
        {
            Value = strValue;
        }

        public string GetValue(DirectableEncConverter transliterator)
        {
            string str = Value;
            if (transliterator != null)
                try
                {
                    str = transliterator.Convert(Value);
                }
                catch { }

            return str;
        }
#if !UsingHtmlDisplayForStoryBt
#if !DataDllBuild
        public void SetAssociation(CtrlTextBox tb)
        {
            _tb = tb;
            if (Transliterator != null)
            {
                try
                {
                    tb.Text = Transliterator.Convert(Value);
                }
                catch
                {
                    tb.Text = Value;
                }
                tb.ReadOnly = true;
            }
            else
            {
                tb.Text = Value;
                tb.ReadOnly = false;
            }
            tb.Tag = this;
        }

        // the TextBox is for the BT pane where this string is associated with
        internal CtrlTextBox TextBox
        {
            get { return _tb; }
        }
#endif
#endif

        // if this string is associated with the ConNotes pane, then keep track
        //  of the element ID it's associated with and the pane, so we can use
        //  it during 'find'
        public string HtmlElementId;
        public object HtmlPane;

        // make it a little non-obvious how to get the string out so we can benefit from compiler-time errors
        public override string ToString()
        {
            return Value;
        }

        public bool HasData
        {
            get { return !String.IsNullOrEmpty(Value); }
        }

        public bool IsNull
        {
            get { return (Value == null); }
        }

        public bool IsSavable
        {
            get { return HasData && !String.IsNullOrEmpty(Value.Trim()); }
        }

        public int NumOfWords(char[] achToIgnore)
        {
            if (!HasData)
                return 0;

            string[] astr = GetWords(achToIgnore);
            return astr.Length;
        }

        public string[] GetWords(char[] achToIgnore)
        {
            return Value.Split(achToIgnore, StringSplitOptions.RemoveEmptyEntries);
        }

        public void ExtractSelectedText(out string strSelectedText)
        {
#if !UsingHtmlDisplayForStoryBt
#if !DataDllBuild
            if (_tb != null)
            {
                strSelectedText = _tb.SelectedText;
                if (!String.IsNullOrEmpty(strSelectedText))
                {
                    _tb.SelectedText = null;
                    strSelectedText = strSelectedText.Trim();
                }
            }
            else
#endif
#endif
                strSelectedText = null;
        }
    }
}
