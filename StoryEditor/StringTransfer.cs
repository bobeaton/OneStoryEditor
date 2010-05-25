using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ECInterfaces;
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
                tb.Text = Transliterator.Convert(Value);
                tb.ReadOnly = true;
            }
            else
            {
                tb.Text = Value;
                tb.ReadOnly = false;
            }
            tb.Tag = this;
        }

        internal CtrlTextBox TextBox
        {
            get { return _tb; }
        }
#endif

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

        public void ExtractSelectedText(out string strSelectedText)
        {
            if (_tb != null)
            {
                strSelectedText = _tb.SelectedText;
                if (!String.IsNullOrEmpty(strSelectedText))
                    _tb.SelectedText = null;
            }
            else
                strSelectedText = null;
        }
    }
}
