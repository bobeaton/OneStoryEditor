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
        public HtmlDisplayForm(StoryEditor theSE, StoryData storyData)
        {
            InitializeComponent();
            _webBrowser.TheSE = theSE;
            _webBrowser.StoryData = storyData;
            _webBrowser.LoadDocument();

            htmlCoachNotesControl1.TheSE = theSE;
            htmlCoachNotesControl1.StoryData = storyData;
            htmlCoachNotesControl1.LoadDocument();
        }
    }
}
