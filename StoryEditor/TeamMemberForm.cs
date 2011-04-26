using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public partial class TeamMemberForm : TopForm
    {
        /*
        protected const string CstrDefaultFontTooltipVernacular =
            "Click here to choose the font, size, and color of the Story language text{0}Currently, Font: {1}, Color: {2}, RTL: {3}";
        protected const string CstrDefaultFontTooltipNationalBT =
            "Click here to choose the font, size, and color of the National language back-translation text{0}Currently, Font: {1}, Color: {2}, RTL: {3}";
        protected const string CstrDefaultFontTooltipInternationalBT =
            "Click here to choose the font, size, and color of the English language back-translation text{0}Currently, Font: {1}, Color: {2}, RTL: {3}";
        */
        private const string CstrDefaultOKLabel = "&Login";
        private const string CstrReturnLabel = "&Return";

        private readonly TeamMembersData _dataTeamMembers;
        private readonly ProjectSettings _theProjSettings;
        protected StoryProjectData _theStoryProjectData;
        private string m_strSelectedMemberName;

        Dictionary<string, TeamMemberData> m_mapNewMembersThisSession = new Dictionary<string, TeamMemberData>();

        public bool Modified;

        public TeamMemberForm(TeamMembersData dataTeamMembers, bool bUseLoginLabel,
            ProjectSettings theProjSettings, StoryProjectData theStoryProjectData)
            : base(true)
        {
            _dataTeamMembers = dataTeamMembers;
            _theProjSettings = theProjSettings;
            _theStoryProjectData = theStoryProjectData;

            InitializeComponent();
            
            foreach (TeamMemberData aMember in _dataTeamMembers.Values)
                listBoxTeamMembers.Items.Add(GetListBoxItem(aMember));

            if ((listBoxTeamMembers.Items.Count > 0) && !String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
                listBoxTeamMembers.SelectedItem = Properties.Settings.Default.LastMemberLogin;

            if (!bUseLoginLabel)
            {
                buttonOK.Text = CstrReturnLabel;
                toolTip.SetToolTip(buttonOK, "Click to return to the previous window");
            }
        }

        private static string GetListBoxItem(TeamMemberData theTeamMember)
        {
            return GetListBoxItem(theTeamMember.Name, theTeamMember.MemberType);
        }

        public static string GetListBoxItem(string strName, TeamMemberData.UserTypes eMemberRole)
        {
            return String.Format("{0} ({1})",
                                 strName,
                                 TeamMemberData.GetMemberTypeAsDisplayString(eMemberRole));
        }

        private static void ParseListBoxItem(string strItem, 
            out string strName, out TeamMemberData.UserTypes eMemberRole)
        {
            int nIndex = strItem.LastIndexOf(" (");
            strName = strItem.Substring(0, nIndex);
            string strRole = strItem.Substring(nIndex + 2, strItem.Length - nIndex - 3);
            eMemberRole = TeamMemberData.GetMemberTypeFromDisplayString(strRole);
        }

        public string SelectedMemberName
        {
            get { return m_strSelectedMemberName; }
        }

        public string SelectedMember
        {
            set
            {
                listBoxTeamMembers.SelectedItem = value;
            }
        }
        
        /*
                private bool DoAccept()
                {
                    try
                    {
                        FinishEdit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,  OseResources.Properties.Resources.IDS_Caption);
                        return false;
                    }

                    return true;
                }

                protected string ThrowIfNullOrEmpty(string strValue, string strErrorMessage)
                {
                    if (String.IsNullOrEmpty(strValue))
                        throw new ApplicationException(String.Format("You have to configure the {0} first", strErrorMessage));
                    return strValue;
                }

                protected void FinishEdit()
                {
                    // the only time you can *not* have a BT language is if the vernacular is "English"
                    if ((!checkBoxVernacular.Checked || (textBoxVernacular.Text != "English")) && !checkBoxEnglishBT.Checked && !checkBoxNationalLangBT.Checked)
                        throw new ApplicationException("You must have at least a back-translation language! Either a national language back-translation or English. Check one of the boxes that begins 'Project will use a *** BT?'");

                    // update the language information as well (in case that was changed also)
                    if (checkBoxVernacular.Checked)
                    {
                        // if there is a default keyboard (from before) and the user has chosen another one, then see if they mean to 
                        //  change it for everyone or just themselves (then we can make sure that they are who we think we are)
                        string strKeyboard = (string)comboBoxKeyboardVernacular.SelectedItem;
                        if (_tmdLastMember != null)
                        {
                            if (!String.IsNullOrEmpty(_projSettings.Vernacular.DefaultKeyboard)
                                && (strKeyboard != _projSettings.Vernacular.DefaultKeyboard))
                            {
                                DialogResult res = MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_ConfirmKeyboardOverride,
                                    _projSettings.Vernacular.LangName, _tmdLastMember.Name), OseResources.Properties.Resources.IDS_Caption,
                                    MessageBoxButtons.YesNoCancel);

                                if (res == DialogResult.Yes)
                                    _tmdLastMember.OverrideVernacularKeyboard = strKeyboard;
                                else if (res == DialogResult.No)
                                {
                                    _projSettings.Vernacular.DefaultKeyboard = strKeyboard;
                                    _tmdLastMember.OverrideVernacularKeyboard = null;   // if there was an override, it should go away
                                }
                                else
                                    return;
                            }
                            else
                            {
                                _projSettings.Vernacular.DefaultKeyboard = strKeyboard;
                                _tmdLastMember.OverrideVernacularKeyboard = null;   // if there was an override, it should go away
                            }
                        }
                        else
                            _projSettings.Vernacular.DefaultKeyboard = strKeyboard;

                        _projSettings.Vernacular.LangName = ThrowIfNullOrEmpty(textBoxVernacular.Text, "Story language name");
                        _projSettings.Vernacular.LangCode = ThrowIfNullOrEmpty(textBoxVernacularEthCode.Text, "Story language Ethn. code");
                        _projSettings.Vernacular.FullStop = ThrowIfNullOrEmpty(textBoxVernSentFullStop.Text, "Story language sentence final punctuation");
                        _projSettings.Vernacular.IsRTL = checkBoxVernacularRTL.Checked;
                    }
                    else
                        _projSettings.Vernacular.HasData = false;

                    if (checkBoxNationalLangBT.Checked)
                    {
                        // if there is a default keyboard (from before) and the user has chosen another one, then see if they mean to 
                        //  change it for everyone or just themselves (then we can make sure that they are who we think we are)
                        string strKeyboard = (string)comboBoxKeyboardNationalBT.SelectedItem;
                        if (_tmdLastMember != null)
                        {
                            if(!String.IsNullOrEmpty(_projSettings.NationalBT.DefaultKeyboard)
                                && (strKeyboard != _projSettings.NationalBT.DefaultKeyboard))
                            {
                                DialogResult res = MessageBox.Show(String.Format(OseResources.Properties.Resources.IDS_ConfirmKeyboardOverride,
                                    _projSettings.Vernacular.LangName, _tmdLastMember.Name), OseResources.Properties.Resources.IDS_Caption,
                                    MessageBoxButtons.YesNoCancel);

                                if (res == DialogResult.Yes)
                                    _tmdLastMember.OverrideNationalBTKeyboard = strKeyboard;
                                else if (res == DialogResult.No)
                                {
                                    _projSettings.NationalBT.DefaultKeyboard = strKeyboard;
                                    _tmdLastMember.OverrideNationalBTKeyboard = null;
                                }
                                else
                                    return;
                            }
                            else
                            {
                                _projSettings.NationalBT.DefaultKeyboard = strKeyboard;
                                _tmdLastMember.OverrideNationalBTKeyboard = null;
                            }
                        }
                        else
                            _projSettings.NationalBT.DefaultKeyboard = strKeyboard;

                        _projSettings.NationalBT.LangName = ThrowIfNullOrEmpty(textBoxNationalBTLanguage.Text, "National BT language name");
                        _projSettings.NationalBT.LangCode = ThrowIfNullOrEmpty(textBoxNationalBTEthCode.Text, "National BT language Ethn. code");
                        _projSettings.NationalBT.FullStop = ThrowIfNullOrEmpty(textBoxNationalBTSentFullStop.Text, "National BT language sentence final punctuation");
                        _projSettings.NationalBT.IsRTL = checkBoxNationalRTL.Checked;
                    }
                    else
                        _projSettings.NationalBT.HasData = false;

                    if (!checkBoxEnglishBT.Checked)
                        // don't set via 'Checked' because HasData should only be set if false
                        _projSettings.InternationalBT.HasData = false; 

                    // English was done by the font dialog handler

                    Modified = false;
                }
                */
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void listBoxTeamMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bOneSelected = (listBoxTeamMembers.SelectedIndex != -1);
            buttonEditMember.Enabled = buttonOK.Enabled = bOneSelected;

            // the delete button is only enabled during the current session (just to prevent them
            //  from removing a team member that has other references in the project).
            if (bOneSelected)
            {
                TeamMemberData.UserTypes eUserType;
                ParseListBoxItem((string) listBoxTeamMembers.SelectedItem,
                                 out m_strSelectedMemberName, out eUserType);
                buttonDeleteMember.Visible = m_mapNewMembersThisSession.ContainsKey(m_strSelectedMemberName);

                if (_dataTeamMembers.ContainsKey(m_strSelectedMemberName))
                {
                    var theMember = _dataTeamMembers[m_strSelectedMemberName];
                    buttonMergeUns.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                    TeamMemberData.UserTypes.UNS));
                    buttonMergeCrafter.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                        TeamMemberData.UserTypes.Crafter));
                    buttonMergeProjectFacilitators.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                                    TeamMemberData.UserTypes.
                                                                                        ProjectFacilitator));
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
            if (listBoxTeamMembers.SelectedIndex == -1)
                return;

            // if the selected user is a UNS, this is probably a mistake.
            TeamMemberData theMember = _dataTeamMembers[m_strSelectedMemberName];
            var eAllowedLoginRoleFilter = (theMember.MemberType &
                                           (TeamMemberData.UserTypes.ProjectFacilitator |
                                            TeamMemberData.UserTypes.ConsultantInTraining |
                                            TeamMemberData.UserTypes.IndependentConsultant |
                                            TeamMemberData.UserTypes.Coach |
                                            TeamMemberData.UserTypes.EnglishBackTranslator |
                                            TeamMemberData.UserTypes.FirstPassMentor |
                                            TeamMemberData.UserTypes.JustLooking));

            if ((buttonOK.Text == CstrDefaultOKLabel)
                && (eAllowedLoginRoleFilter == TeamMemberData.UserTypes.Undefined))
            {
                MessageBox.Show(Properties.Resources.IDS_LoginAsProjectFacilitator,
                                OseResources.Properties.Resources.IDS_Caption);
                return;
            }

            // when the button label is "OK", it means we're adding a UNS
            if (buttonOK.Text == CstrDefaultOKLabel)
            {
                Properties.Settings.Default.LastMemberLogin = SelectedMemberName;
                Properties.Settings.Default.LastUserType = eAllowedLoginRoleFilter.ToString();
                Properties.Settings.Default.Save();
            }

            // _projSettings.IsConfigured = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            // unselect any member and set the target tab (see 
            //  tabControlProjectMetaData_Selected for what happens)
            listBoxTeamMembers.SelectedIndex = -1;

            var dlg = new EditMemberForm(null, _theProjSettings, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string strItem = GetListBoxItem(dlg.MemberName, dlg.MemberType);
                if (listBoxTeamMembers.Items.Contains(strItem) ||
                    _dataTeamMembers.ContainsKey(dlg.MemberName))
                {
                    MessageBox.Show(String.Format("Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then select the name in the listbox and click the 'Edit Member' button", dlg.MemberName));
                    return;
                }
                
                Modified = true;

                TeamMemberData theNewMemberData;
                if (m_mapNewMembersThisSession.TryGetValue(dlg.MemberName, out theNewMemberData))
                {
                    // I don't see how this could happen... this must have been from back when
                    //  you could edit and add in a similar way. Now *Add* means *add a new one*
                    //  and they can't exist in this map...
                    System.Diagnostics.Debug.Assert(false);

                    // must just be editing the already added member...
                    System.Diagnostics.Debug.Assert(listBoxTeamMembers.Items.Contains(strItem));

                    theNewMemberData.MemberType = dlg.MemberType;
                    theNewMemberData.Email = dlg.Email;
                    theNewMemberData.AltPhone = dlg.AltPhone;
                    theNewMemberData.Phone = dlg.Phone;
                    theNewMemberData.BioData = dlg.BioData;
                    theNewMemberData.SkypeID = dlg.SkypeID;
                    theNewMemberData.TeamViewerID = dlg.TeamViewerID;

                    // update the role listbox
                    int nIndex = listBoxTeamMembers.Items.IndexOf(strItem);
                    // listBoxMemberRoles.Items[nIndex] = TeamMemberData.GetMemberTypeAsDisplayString(theNewMemberData.MemberType);
                }
                else
                {
                    // add this new user to the proj file
                    theNewMemberData = new TeamMemberData(dlg.MemberName,
                        dlg.MemberType, String.Format("mem-{0}", Guid.NewGuid()),
                        dlg.Email, dlg.SkypeID, dlg.TeamViewerID, dlg.Phone, dlg.AltPhone,
                        dlg.BioData)
                           {
                               DefaultAllowed = dlg.DefaultAllowed, 
                               DefaultRequired = dlg.DefaultRequired
                           };

                    _dataTeamMembers.Add(dlg.MemberName, theNewMemberData);
                    m_mapNewMembersThisSession.Add(dlg.MemberName, theNewMemberData);
                    listBoxTeamMembers.Items.Add(strItem);
                    // listBoxMemberRoles.Items.Insert(nIndex, TeamMemberData.GetMemberTypeAsDisplayString(theNewMemberData.MemberType));
                    listBoxTeamMembers.SelectedItem = strItem;
                }
            }
        }

        private void buttonEditMember_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
            int nIndex = listBoxTeamMembers.SelectedIndex;

            TeamMemberData.UserTypes eMemberRole;
            ParseListBoxItem((string) listBoxTeamMembers.SelectedItem,
                             out m_strSelectedMemberName, out eMemberRole);

            System.Diagnostics.Debug.Assert(_dataTeamMembers.ContainsKey(m_strSelectedMemberName));
            TeamMemberData theMemberData = _dataTeamMembers[m_strSelectedMemberName];
            var dlg = new EditMemberForm(theMemberData, _theProjSettings, true);
            if (dlg.ShowDialog() != DialogResult.OK) 
                return;

            Modified = true;

            // if the name of the edited item has been changed and the new name is already 
            //  in use, then don't change the name
            if ((dlg.MemberName != m_strSelectedMemberName) 
                && _dataTeamMembers.ContainsKey(dlg.MemberName))
            {
                MessageBox.Show(String.Format("Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then select the name in the listbox and click the 'Edit Member' button.", dlg.MemberName));
            }
            else
                theMemberData.Name = dlg.MemberName;

            theMemberData.MemberType = dlg.MemberType;
            theMemberData.Email = dlg.Email;
            theMemberData.AltPhone = dlg.AltPhone;
            theMemberData.Phone = dlg.Phone;
            theMemberData.BioData = dlg.BioData;
            theMemberData.SkypeID = dlg.SkypeID;
            theMemberData.TeamViewerID = dlg.TeamViewerID;
            theMemberData.DefaultAllowed = dlg.DefaultAllowed;
            theMemberData.DefaultRequired = dlg.DefaultRequired;

            // update the role listbox
            // listBoxMemberRoles.Items[nIndex] = TeamMemberData.GetMemberTypeAsDisplayString(theMemberData.MemberType);
            if (theMemberData.Name != m_strSelectedMemberName)
            {
                _dataTeamMembers.Remove(m_strSelectedMemberName);
                m_strSelectedMemberName = theMemberData.Name;
                _dataTeamMembers.Add(m_strSelectedMemberName, theMemberData);
            }

            listBoxTeamMembers.Items[nIndex] = GetListBoxItem(theMemberData);

            // keep a hang on it so we don't try to, for example, give it a new guid
            if (!m_mapNewMembersThisSession.ContainsKey(dlg.MemberName))
                m_mapNewMembersThisSession.Add(dlg.MemberName, theMemberData);
        }

        private void buttonDeleteMember_Click(object sender, EventArgs e)
        {
            // this is only enabled if we added the member this session
            System.Diagnostics.Debug.Assert(m_mapNewMembersThisSession.ContainsKey(SelectedMemberName) && _dataTeamMembers.ContainsKey(SelectedMemberName));

            _dataTeamMembers.Remove(SelectedMemberName);
            m_mapNewMembersThisSession.Remove(SelectedMemberName);
        }
        /*
        private void tabControlProjectMetaData_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageMemberList)
            {
                Console.WriteLine("tabPageMemberList Selected");

                // if the user made some changes and then is moving away from the tab, 
                //  then do an implicit Accept
                if (Modified)
                    if (!DoAccept())
                        tabControlProjectMetaData.SelectedTab = tabPageLanguageInfo;
            }
        }

        void textBox_TextChanged(object sender, System.EventArgs e)
        {
            Modified = true;
        }

        private void buttonVernacularFont_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = _projSettings.Vernacular.LangFont;
                fontDialog.Color = _projSettings.Vernacular.FontColor;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    _projSettings.Vernacular.LangFont = fontDialog.Font;
                    _projSettings.Vernacular.FontName = null;  // clear it out so we use the value in LangFont
                    _projSettings.Vernacular.FontColor = fontDialog.Color;
                    textBoxVernSentFullStop.Font = fontDialog.Font;
                    textBoxVernSentFullStop.ForeColor = fontDialog.Color;
                    Modified = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Only TrueType fonts are supported. This is not a TrueType font.")
                    MessageBox.Show("Since you just added this font, you have to restart the program for it to work", OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void buttonNationalBTFont_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = _projSettings.NationalBT.LangFont;
                fontDialog.Color = _projSettings.NationalBT.FontColor;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    _projSettings.NationalBT.LangFont = fontDialog.Font;
                    _projSettings.NationalBT.FontName = null;  // clear it out so we use the value in LangFont
                    _projSettings.NationalBT.FontColor = fontDialog.Color;
                    textBoxNationalBTSentFullStop.Font = fontDialog.Font;
                    textBoxNationalBTSentFullStop.ForeColor = fontDialog.Color;
                    Modified = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Only TrueType fonts are supported. This is not a TrueType font.")
                    MessageBox.Show("Since you just added this font, you have to restart the program for it to work", OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void buttonInternationalBTFont_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = _projSettings.InternationalBT.LangFont;
                fontDialog.Color = _projSettings.InternationalBT.FontColor;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    _projSettings.InternationalBT.LangFont = fontDialog.Font;
                    _projSettings.InternationalBT.FontName = null; // clear it out so we use the value in LangFont
                    _projSettings.InternationalBT.FontColor = fontDialog.Color;
                    Modified = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Only TrueType fonts are supported. This is not a TrueType font.")
                    MessageBox.Show("Since you just added this font, you have to restart the program for it to work", OseResources.Properties.Resources.IDS_Caption);
            }
        }
        */
        private void listBoxTeamMembers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(sender, e);
        }

        /*
        private void listBoxMemberRoles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(sender, e);
        }
        */

        private void buttonMergeUns_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.UNS, _theStoryProjectData.ReplaceUns);
        }

        private void buttonMergeProjectFacilitators_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.ProjectFacilitator, _theStoryProjectData.ReplaceProjectFacilitator);
        }

        private void buttonMergeCrafter_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.Crafter, _theStoryProjectData.ReplaceCrafter);
        }

        private delegate void ReplaceMemberDelegate(string strOldUnsGuid, string strNewUnsGuid);

        private void ReplaceMember(TeamMemberData.UserTypes eRole, ReplaceMemberDelegate replaceMemberDelegate)
        {
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
            int nIndex = listBoxTeamMembers.SelectedIndex;

            TeamMemberData.UserTypes eMemberRole;
            ParseListBoxItem((string) listBoxTeamMembers.SelectedItem,
                             out m_strSelectedMemberName, out eMemberRole);

            System.Diagnostics.Debug.Assert(_dataTeamMembers.ContainsKey(m_strSelectedMemberName));
            TeamMemberData theMemberData = _dataTeamMembers[m_strSelectedMemberName];

            // query the UNS to merge into this UNS record
            var dlg = new MemberPicker(_theStoryProjectData, eRole)
                          {
                              Text =
                                  String.Format("Choose the {0} to merge into the record for '{1}'",
                                                TeamMemberData.GetMemberTypeAsDisplayString(eRole),
                                                theMemberData.Name),
                              ItemToBlock = theMemberData.Name
                          };

            DialogResult res = dlg.ShowDialog();
            if (res != DialogResult.OK)
                return;

            string strOldMemberGuid = dlg.SelectedMember.MemberGuid;
            TeamMemberData.UserTypes eOrigRoles = dlg.SelectedMember.MemberType;
            try
            {
                replaceMemberDelegate(strOldMemberGuid, theMemberData.MemberGuid);
                theMemberData.MergeWith(dlg.SelectedMember);
                Modified = true;

                dlg.SelectedMember.MemberType &= ~eRole;
                if (dlg.SelectedMember.MemberType != TeamMemberData.UserTypes.Undefined)
                {
                    res = MessageBox.Show(String.Format(Properties.Resources.IDS_QueryMergeMultipleRoles,
                                                        _dataTeamMembers.GetNameFromMemberId(strOldMemberGuid),
                                                        TeamMemberData.GetMemberTypeAsDisplayString(
                                                            dlg.SelectedMember.MemberType),
                                                        _dataTeamMembers.GetNameFromMemberId(theMemberData.MemberGuid)),
                                          OseResources.Properties.Resources.IDS_Caption,
                                          MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                        return;

                    MergeOtherRoles(dlg.SelectedMember.MemberType,
                                    strOldMemberGuid, 
                                    theMemberData.MemberGuid);

                    // get the index for the member we're about to add new roles to 
                    //  (since we have to update his role list)
                    nIndex = listBoxTeamMembers.FindString(GetListBoxItem(theMemberData));

                    // now add those roles just in case they aren't already
                    theMemberData.MemberType |= dlg.SelectedMember.MemberType;

                    if (nIndex != -1)
                        listBoxTeamMembers.Items[nIndex] = GetListBoxItem(theMemberData);
                    else
                        System.Diagnostics.Debug.Assert(false);
                }

                res = MessageBox.Show(String.Format(Properties.Resources.IDS_ConfirmDeleteMember,
                                                    dlg.SelectedMember.Name,
                                                    theMemberData.Name),
                                      OseResources.Properties.Resources.IDS_Caption,
                                      MessageBoxButtons.YesNoCancel);

                if (res != DialogResult.Yes)
                    return;

                string strNameToDelete = _dataTeamMembers.GetNameFromMemberId(strOldMemberGuid);
                _dataTeamMembers.Remove(strNameToDelete);

                nIndex = listBoxTeamMembers.FindString(GetListBoxItem(strNameToDelete, eOrigRoles));
                if (nIndex != -1)
                    listBoxTeamMembers.Items.RemoveAt(nIndex);
                else
                    System.Diagnostics.Debug.Assert(false);
            }
            catch (StoryProjectData.ReplaceMemberException ex)
            {
                var strErrorMsg = String.Format(Properties.Resources.IDS_CantChangeMember,
                                                _dataTeamMembers.GetNameFromMemberId(
                                                    strOldMemberGuid),
                                                _dataTeamMembers.GetNameFromMemberId(
                                                    theMemberData.MemberGuid),
                                                ex.StoryName,
                                                String.Format(ex.Format,
                                                              _dataTeamMembers.GetNameFromMemberId(
                                                                  ex.MemberGuid)));
                MessageBox.Show(strErrorMsg, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void MergeOtherRoles(TeamMemberData.UserTypes eRoles, string strOldMemberGuid, string strNewMemberGuid)
        {
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.ProjectFacilitator))
                _theStoryProjectData.ReplaceProjectFacilitator(strOldMemberGuid, strNewMemberGuid);
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.Crafter))
                _theStoryProjectData.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.UNS))
                _theStoryProjectData.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }

        /*
        private void textBoxVernacular_Leave(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxVernacular.Text, textBoxVernacularEthCode);
        }

        private void textBoxNationalBTLanguage_Leave(object sender, EventArgs e)
        {
            ProposeEthnologueCode(textBoxNationalBTLanguage.Text, textBoxNationalBTEthCode);
        }

        string _strLangCodesFile = null;

        protected void ProposeEthnologueCode(string strLanguageName, TextBox tbLanguageCode)
        {
            if (!String.IsNullOrEmpty(tbLanguageCode.Text))  // only have to suggest if we don't have a value
                return;

            if (_strLangCodesFile == null)
                _strLangCodesFile = File.ReadAllText(PathToLangCodesFile);

            int nIndex = _strLangCodesFile.IndexOf(String.Format("\t{0}{1}", strLanguageName, Environment.NewLine));
            const int CnOffset = 8;
            if (nIndex >= CnOffset)
            {
                nIndex -= CnOffset;    // back up to the beginning of the line;
                int nLength = 1 // for the tab
                    + strLanguageName.Length + CnOffset;
                string strEntry = _strLangCodesFile.Substring(nIndex, nLength);
                
                // now, grab off just the code, which goes from the beginning of the line to the first tab.
                nIndex = strEntry.IndexOf('\t');
                string strCode = strEntry.Substring(0, nIndex);
                tbLanguageCode.Text = strCode;
            }
        }

        protected const string CstrLangCodesFilename = "LanguageCodes.tab";

        protected string PathToLangCodesFile
        {
            get
            {
                // try the same folder as we're executing out of
                string strCurrentFolder = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                strCurrentFolder = Path.GetDirectoryName(strCurrentFolder);
                string strFileToCheck = Path.Combine(strCurrentFolder, CstrLangCodesFilename);
#if DEBUG
                if (!File.Exists(strFileToCheck))
                    // on dev machines, this file is in the "..\..\src\EC\TECkit Mapping Editor" folder
                    strFileToCheck = @"C:\src\StoryEditor\StoryEditor\" + CstrLangCodesFilename;
#endif
                System.Diagnostics.Debug.Assert(File.Exists(strFileToCheck), String.Format("Can't find: {0}! You'll need to re-install or contact bob_eaton@sall.com", strFileToCheck));

                return strFileToCheck;
            }
        }

        private void textBoxVernSentFullStop_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardVernacular.SelectedItem);
        }

        private void textBoxNationalBTSentFullStop_Enter(object sender, EventArgs e)
        {
            SetKeyboard((string)comboBoxKeyboardNationalBT.SelectedItem);
        }

        protected void SetKeyboard(string strKeybaordToSet)
        {
            if (!String.IsNullOrEmpty(strKeybaordToSet))
                KeyboardController.ActivateKeyboard(strKeybaordToSet);
        }

        private void textBoxSentFullStop_Leave(object sender, EventArgs e)
        {
            KeyboardController.DeactivateKeyboard();
        }

        private void checkBoxVernacular_CheckedChanged(object sender, EventArgs e)
        {
            textBoxVernacular.Enabled =
                textBoxVernacularEthCode.Enabled =
                comboBoxKeyboardVernacular.Enabled =
                textBoxVernSentFullStop.Enabled =
                buttonVernacularFont.Enabled =
                checkBoxVernacularRTL.Enabled = checkBoxVernacular.Checked;
        }

        private void checkBoxNationalLangBT_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNationalBTLanguage.Enabled =
                textBoxNationalBTEthCode.Enabled =
                comboBoxKeyboardNationalBT.Enabled =
                textBoxNationalBTSentFullStop.Enabled =
                buttonNationalBTFont.Enabled =
                checkBoxNationalRTL.Enabled = checkBoxNationalLangBT.Checked;
        }

        private void checkBoxEnglishBT_CheckedChanged(object sender, EventArgs e)
        {
            buttonInternationalBTFont.Enabled = checkBoxEnglishBT.Checked;
        }

        private void textBoxVernacular_TextChanged(object sender, EventArgs e)
        {
            Modified = true;

            // if the user retypes the language name, then clobber the eth code as well
            //  (so we'll guess again)
            textBoxVernacularEthCode.Text = null;
        }

        private void textBoxNationalBTLanguage_TextChanged(object sender, EventArgs e)
        {
            Modified = true;

            // if the user retypes the language name, then clobber the eth code as well
            //  (so we'll guess again)
            textBoxNationalBTEthCode.Text = null;
        }

        private void comboBoxKeyboard_DragDrop(object sender, DragEventArgs e)
        {
            // initialize the keyboard combo list (in case the user just added it while
            //  we were open
            System.Diagnostics.Debug.Assert(sender is ComboBox);
            ComboBox cb = (ComboBox)sender;
            cb.Items.Clear();
            foreach (KeyboardController.KeyboardDescriptor keyboard in
                KeyboardController.GetAvailableKeyboards(KeyboardController.Engines.All))
            {
                cb.Items.Add(keyboard.Name);
            }
        }

        private void buttonStateConfig_Click(object sender, EventArgs e)
        {
            var dlg = new StageEditorForm(_storyProjectData);
            dlg.ShowDialog();
        }
        */
    }
}
