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
            htmlStoryBtControl.LoadDocument();
        }
    }
}
