using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Chorus.UI.Review;
using Chorus.UI.Review.RevisionsInRepository;
using Chorus.Utilities;
using Chorus.VcsDrivers.Mercurial;

namespace OneStoryProjectEditor
{
    public partial class HtmlDisplayForm : Form
    {
        protected RevisionSelectedEvent _revisionSelectedEvent = new RevisionSelectedEvent();
        protected HgRepository _repository;
        protected RevisionInRepositoryModel _revisionInRepositoryModel;
        protected RevisionsInRepositoryView _revisionsInRepositoryView;

        public HtmlDisplayForm(StoryEditor theSE, StoryData storyData)
        {
            InitializeComponent();

            if (theSE.StoryProject.ProjSettings.Vernacular.HasData)
            {
                checkBoxLangVernacular.Text = String.Format(Properties.Resources.IDS_LanguageFields,
                                                            theSE.StoryProject.ProjSettings.Vernacular.LangName);
            }
            else
                checkBoxLangVernacular.Visible = false;

            if (theSE.StoryProject.ProjSettings.NationalBT.HasData)
            {
                checkBoxLangNationalBT.Text = String.Format(Properties.Resources.IDS_StoryLanguageField,
                                                            theSE.StoryProject.ProjSettings.NationalBT.LangName);
            }
            else
                checkBoxLangNationalBT.Visible = false;

            checkBoxLangInternationalBT.Visible = theSE.StoryProject.ProjSettings.InternationalBT.HasData;

            try
            {
                _revisionSelectedEvent.Subscribe(SetRevision);
                _repository = HgRepository.CreateOrLocate(theSE.StoryProject.ProjSettings.ProjectFolder, new NullProgress());
                _revisionInRepositoryModel = new RevisionInRepositoryModel(_repository, _revisionSelectedEvent);
                _revisionsInRepositoryView = new RevisionsInRepositoryView(_revisionInRepositoryModel);
                _revisionsInRepositoryView.Dock = DockStyle.Fill;
                tableLayoutPanelSettings.Controls.Add(_revisionsInRepositoryView, 0, 1);
                tableLayoutPanelSettings.SetColumnSpan(_revisionsInRepositoryView, 2);
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, OseResources.Properties.Resources.IDS_Caption);
            }
            
            htmlStoryBtControl.TheSE = theSE;
            htmlStoryBtControl.StoryData = storyData;
            htmlStoryBtControl.MembersData = theSE.StoryProject.TeamMembers;
            htmlStoryBtControl.LoggedOnMember = theSE.LoggedOnMember;
            // htmlStoryBtControl.LoadDocument();
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageDisplayChangeReport)
            {
                htmlStoryBtControl.ViewItemsToInsureOn = VerseData.SetItemsToInsureOn(
                    checkBoxLangVernacular.Checked,
                    checkBoxLangNationalBT.Checked,
                    checkBoxLangInternationalBT.Checked,
                    checkBoxAnchors.Checked,
                    checkBoxStoryTestingQuestions.Checked,
                    checkBoxRetellings.Checked,
                    false,  // theSE.viewConsultantNoteFieldMenuItem.Checked,
                    false,  // theSE.viewCoachNotesFieldMenuItem.Checked,
                    false); // theSE.viewNetBibleMenuItem.Checked

                htmlStoryBtControl.LoadDocument();
            }
        }

        private void SetRevision(Revision descriptor)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (descriptor != null)
            {
                FileInRevision firParent = new FileInRevision(descriptor.Number.LocalRevisionNumber,
                    htmlStoryBtControl.TheSE.StoryProject.ProjSettings.ProjectFilePath, FileInRevision.Action.Unknown);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(firParent.GetFileContents(_repository));
                string strXPath = String.Format("/StoryProject/stories[@SetName = 'Stories']/story[@name = '{0}']", 
                    htmlStoryBtControl.StoryData.Name);
                XmlNode parentStoryNode = doc.SelectSingleNode(strXPath);
                if (parentStoryNode != null)
                {
                    StoryData parentStory = new StoryData(parentStoryNode);
                    htmlStoryBtControl.ParentStory = parentStory;
                }
            }
            else
            {
                htmlStoryBtControl.ParentStory = null;
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
