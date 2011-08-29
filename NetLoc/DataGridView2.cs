using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetLoc
{
    /// <summary>
    /// DataGridView with accessibility disabled, as there is a .NET
    /// problem that causes menu opening to take up to 10 seconds
    /// when there is a large DataGridView on the form.
    /// 
    /// Just subclassing alone now seems to work, hopefully without causing 
    /// the errors that occurred when null was returned for the AccessibleObject
    /// </summary>
    public class DataGridView2 : DataGridView
    {
        //protected override AccessibleObject CreateAccessibilityInstance()
        //{
        //      // return null;
        //    Trace.TraceInformation("Creating DataGridView2 accessibility object");
        //    return new DataGridView2AccessibleObject();
        //}
    }
    //public class DataGridView2AccessibleObject : AccessibleObject
    //{
    //}
}