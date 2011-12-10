using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class LnCNotePrintForm : Form
    {
        public LnCNotePrintForm(StoryEditor theSe)
        {
            InitializeComponent();
            printViewer.webBrowser.TheSE = theSe;
            printViewer.saveFileDialog.FileName = String.Format("{0} LnCNotes.html",
                                                                theSe.StoryProject.ProjSettings.ProjectName);
            var strHtmlInner = theSe.StoryProject.LnCNotes.PresentationHtml;
            printViewer.webBrowser.DocumentText = StoryData.AddHtmlHtmlDocOutside(strHtmlInner,
                                                                                  theSe.StoryProject.ProjSettings);
        }
    }
}
