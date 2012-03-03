using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class GeckoTestForm : Form
    {
        private readonly StoryEditor _theSe;
        private readonly StoryData _storyData;

        public GeckoTestForm(StoryEditor theSe, StoryData storyData)
        {
            _theSe = theSe;
            _storyData = storyData;
            InitializeComponent();
            geckoWebBrowser.NavigateFinishedNotifier.NavigateFinished += (sender, e) =>
            {
                var strHtml = _storyData.ConsultantNotesHtml(this,
                                                            _theSe.StoryProject.ProjSettings,
                                                            _theSe.LoggedOnMember,
                                                            _theSe.StoryProject.TeamMembers,
                                                            _theSe.viewHiddenVersesMenu.Checked,
                                                            _theSe.viewOnlyOpenConversationsMenu.Checked);
                geckoWebBrowser.Document.Body.InnerHtml = strHtml;
            };

            // geckoWebBrowser.DocumentCompleted += (s, e) => LoadDocument();
        }

        public new void Show()
        {
            base.Show();
            geckoWebBrowser.Navigate(@"C:\src\StoryEditor\StoryEditor\Resources\ose.html");
        }

        private void LoadDocument()
        {
            var strHtml = _storyData.ConsultantNotesHtml(this,
                                                        _theSe.StoryProject.ProjSettings,
                                                        _theSe.LoggedOnMember,
                                                        _theSe.StoryProject.TeamMembers,
                                                        _theSe.viewHiddenVersesMenu.Checked,
                                                        _theSe.viewOnlyOpenConversationsMenu.Checked);
            geckoWebBrowser.Document.Body.InnerHtml = strHtml;
        }
    }
}
