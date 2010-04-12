using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class HtmlDisplayForm : Form
    {
        public HtmlDisplayForm(VersesData Verses, string strDocumentText)
        {
            InitializeComponent();
            _webBrowser.IsConsultantNotes = true;
            _webBrowser.Verses = Verses;
            _webBrowser.DocumentText = strDocumentText;
        }
    }
}
