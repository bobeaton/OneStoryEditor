using System;
using SilEncConverters31;

namespace OneStoryProjectEditor
{
    // this class is used for all the string data in our *Data classes, so that the 
    //  controls they're associated with will be able to automatically update themselves
    public class StringTransfer
    {
        protected string Value;
#if !DataDllBuild
        protected CtrlTextBox _tb = null;
#endif

        public DirectableEncConverter Transliterator { get; set; }

        public StringTransfer(string strValue)
        {
            SetValue(strValue);
        }

        public void SetValue(string strValue)
        {
            Value = strValue;
        }

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

        // if this string is associated with the ConNotes pane, then keep track
        //  of the element ID it's associated with and the pane, so we can use
        //  it during 'find'
        public string HtmlElementId;
        public object HtmlConNoteCtrl;

        // make it a little non-obvious how to get the string out so we can benefit from compiler-time errors
        public override string ToString()
        {
            System.Diagnostics.Debug.WriteLine(HasData);
            return Value;
        }

        public bool HasData
        {
            get { return !String.IsNullOrEmpty(Value); }
        }

        public int NumOfWords(char[] achToIgnore)
        {
            if (!HasData)
                return 0;

            string[] astr = Value.Split(achToIgnore, StringSplitOptions.RemoveEmptyEntries);
            return astr.Length;
        }

        public void ExtractSelectedText(out string strSelectedText)
        {
#if !DataDllBuild
            if (_tb != null)
            {
                strSelectedText = _tb.SelectedText;
                if (!String.IsNullOrEmpty(strSelectedText))
                    _tb.SelectedText = null;
            }
            else
#endif
                strSelectedText = null;
        }
    }
}
