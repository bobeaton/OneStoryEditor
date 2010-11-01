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
    public partial class KeyTermsSearchForm : Form
    {
        readonly StoryEditor _theSE;
        readonly StoryProjectData _storyProject;
        readonly BiblicalTermsHTMLBuilder htmlBuilder;  // Class used to build references window text as html

        public KeyTermsSearchForm(StoryEditor theSE, LnCNote theLnCNote)
        {
            InitializeComponent();
            _theSE = theSE;
            _storyProject = _theSE.StoryProject;

            ProjectSettings projSettings = _theSE.StoryProject.ProjSettings;
            htmlBuilder = new BiblicalTermsHTMLBuilder(projSettings);

            var btl = BiblicalTermsList.GetBiblicalTerms(projSettings.ProjectFolder);
            List<Term> listTerms = theLnCNote.GetKeyTerms(btl);

            htmlBuilder.ReadVerseText(listTerms, _theSE.StoryProject, progressBarLoadingKeyTerms);
            BiblicalTermStatus dontcare;
            string strHtml = htmlBuilder.Build(_storyProject, progressBarLoadingKeyTerms, false, out dontcare);
            webBrowser.LoadDocument(strHtml);
        }
    }
}
