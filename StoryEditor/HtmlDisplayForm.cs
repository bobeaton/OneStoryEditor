using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            htmlStoryBtControl.ViewItemsToInsureOn = VerseData.SetItemsToInsureOn(
                    theSE.viewVernacularLangFieldMenuItem.Checked,
                    theSE.viewNationalLangFieldMenuItem.Checked,
                    theSE.viewEnglishBTFieldMenuItem.Checked,
                    theSE.viewAnchorFieldMenuItem.Checked,
                    theSE.viewStoryTestingQuestionFieldMenuItem.Checked,
                    theSE.viewRetellingFieldMenuItem.Checked,
                    theSE.viewConsultantNoteFieldMenuItem.Checked,
                    theSE.viewCoachNotesFieldMenuItem.Checked,
                    theSE.viewNetBibleMenuItem.Checked);

            htmlStoryBtControl.MembersData = theSE.StoryProject.TeamMembers;
            htmlStoryBtControl.LoggedOnMember = theSE.LoggedOnMember;
            // htmlStoryBtControl.LoadDocument();
        }
    }
}
