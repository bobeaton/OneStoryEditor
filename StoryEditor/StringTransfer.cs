using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    // this class is used for all the string data in our *Data classes, so that the 
    //  controls they're associated with will be able to automatically update themselves
    public class StringTransfer
    {
        public delegate void SetValueDelegate(string strValue);

        protected SetValueDelegate theSetValueDelegate = null;
        protected string Value = null;

        public StringTransfer(string strValue)
        {
            Value = strValue;
            theSetValueDelegate = new SetValueDelegate(SetValue);
        }

        public void SetValue(string strValue)
        {
            Value = strValue;
        }

        public void SetAssociation(Control tb)
        {
            tb.Text = Value;
            tb.Tag = theSetValueDelegate;
        }

        // make it a little non-obvious how to get the string out so we can benefit from compiler-time errors
        public override string ToString()
        {
            return Value;
        }

        public bool HasData
        {
            get { return (Value != null); }
        }
    }
}
