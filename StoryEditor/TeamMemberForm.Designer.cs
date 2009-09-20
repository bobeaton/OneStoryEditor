namespace OneStoryProjectEditor
{
    partial class TeamMemberForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamMemberForm));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.listBoxTeamMembers = new System.Windows.Forms.ListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAddNewMember = new System.Windows.Forms.Button();
            this.buttonEditMember = new System.Windows.Forms.Button();
            this.buttonDeleteMember = new System.Windows.Forms.Button();
            this.listBoxMemberRoles = new System.Windows.Forms.ListBox();
            this.textBoxVernacular = new System.Windows.Forms.TextBox();
            this.textBoxNationalBTLanguage = new System.Windows.Forms.TextBox();
            this.textBoxVernacularEthCode = new System.Windows.Forms.TextBox();
            this.textBoxNationalBTEthCode = new System.Windows.Forms.TextBox();
            this.textBoxVernSentFullStop = new System.Windows.Forms.TextBox();
            this.textBoxNationalBTSentFullStop = new System.Windows.Forms.TextBox();
            this.checkBoxNationalLangBT = new System.Windows.Forms.CheckBox();
            this.checkBoxEnglishBT = new System.Windows.Forms.CheckBox();
            this.buttonVernacularFont = new System.Windows.Forms.Button();
            this.buttonNationalBTFont = new System.Windows.Forms.Button();
            this.tabControlProjectMetaData = new System.Windows.Forms.TabControl();
            this.tabPageMemberList = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelTeamMembers = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMemberNames = new System.Windows.Forms.TextBox();
            this.textBoxMemberRoles = new System.Windows.Forms.TextBox();
            this.tabPageLanguageInfo = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelLanguageInformation = new System.Windows.Forms.TableLayoutPanel();
            this.labelLanguageName = new System.Windows.Forms.Label();
            this.labelStoryLanguage = new System.Windows.Forms.Label();
            this.labelNationalBTLanguage = new System.Windows.Forms.Label();
            this.labelEthnologueCode = new System.Windows.Forms.Label();
            this.buttonInternationalBTFont = new System.Windows.Forms.Button();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.labelReturnToTeamMemberTabInstructions = new System.Windows.Forms.Label();
            this.labelSentenceTerm = new System.Windows.Forms.Label();
            this.labelFont = new System.Windows.Forms.Label();
            this.checkBoxVernacularRTL = new System.Windows.Forms.CheckBox();
            this.checkBoxNationalRTL = new System.Windows.Forms.CheckBox();
            this.labelKeyboard = new System.Windows.Forms.Label();
            this.comboBoxKeyboardVernacular = new System.Windows.Forms.ComboBox();
            this.comboBoxKeyboardNationalBT = new System.Windows.Forms.ComboBox();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControlProjectMetaData.SuspendLayout();
            this.tabPageMemberList.SuspendLayout();
            this.tableLayoutPanelTeamMembers.SuspendLayout();
            this.tabPageLanguageInfo.SuspendLayout();
            this.tableLayoutPanelLanguageInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxTeamMembers
            // 
            this.listBoxTeamMembers.ColumnWidth = 151;
            this.listBoxTeamMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTeamMembers.FormattingEnabled = true;
            this.listBoxTeamMembers.Location = new System.Drawing.Point(3, 38);
            this.listBoxTeamMembers.MultiColumn = true;
            this.listBoxTeamMembers.Name = "listBoxTeamMembers";
            this.tableLayoutPanelTeamMembers.SetRowSpan(this.listBoxTeamMembers, 3);
            this.listBoxTeamMembers.Size = new System.Drawing.Size(199, 342);
            this.listBoxTeamMembers.TabIndex = 0;
            this.toolTip.SetToolTip(this.listBoxTeamMembers, "This list shows all the members of the team");
            this.listBoxTeamMembers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxTeamMembers_MouseDoubleClick);
            this.listBoxTeamMembers.SelectedIndexChanged += new System.EventHandler(this.listBoxTeamMembers_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(229, 386);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "&Login";
            this.toolTip.SetToolTip(this.buttonOK, "Click to login as the selected member");
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(310, 386);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.toolTip.SetToolTip(this.buttonCancel, "Click to cancel this dialog");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAddNewMember
            // 
            this.buttonAddNewMember.Location = new System.Drawing.Point(412, 38);
            this.buttonAddNewMember.Name = "buttonAddNewMember";
            this.buttonAddNewMember.Size = new System.Drawing.Size(111, 30);
            this.buttonAddNewMember.TabIndex = 1;
            this.buttonAddNewMember.Text = "&Add New Member";
            this.toolTip.SetToolTip(this.buttonAddNewMember, "Click to Add a new team member");
            this.buttonAddNewMember.UseVisualStyleBackColor = true;
            this.buttonAddNewMember.Click += new System.EventHandler(this.buttonAddNewMember_Click);
            // 
            // buttonEditMember
            // 
            this.buttonEditMember.Enabled = false;
            this.buttonEditMember.Location = new System.Drawing.Point(412, 74);
            this.buttonEditMember.Name = "buttonEditMember";
            this.buttonEditMember.Size = new System.Drawing.Size(111, 30);
            this.buttonEditMember.TabIndex = 2;
            this.buttonEditMember.Text = "&Edit Member";
            this.toolTip.SetToolTip(this.buttonEditMember, "Click to edit the selected member\'s profile");
            this.buttonEditMember.UseVisualStyleBackColor = true;
            this.buttonEditMember.Click += new System.EventHandler(this.buttonEditMember_Click);
            // 
            // buttonDeleteMember
            // 
            this.buttonDeleteMember.Location = new System.Drawing.Point(412, 110);
            this.buttonDeleteMember.Name = "buttonDeleteMember";
            this.buttonDeleteMember.Size = new System.Drawing.Size(111, 30);
            this.buttonDeleteMember.TabIndex = 3;
            this.buttonDeleteMember.Text = "&Delete Member";
            this.toolTip.SetToolTip(this.buttonDeleteMember, "Click to delete the selected member (only works for members added this session)");
            this.buttonDeleteMember.UseVisualStyleBackColor = true;
            this.buttonDeleteMember.Visible = false;
            this.buttonDeleteMember.Click += new System.EventHandler(this.buttonDeleteMember_Click);
            // 
            // listBoxMemberRoles
            // 
            this.tableLayoutPanelTeamMembers.SetColumnSpan(this.listBoxMemberRoles, 2);
            this.listBoxMemberRoles.ColumnWidth = 151;
            this.listBoxMemberRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxMemberRoles.FormattingEnabled = true;
            this.listBoxMemberRoles.Location = new System.Drawing.Point(208, 38);
            this.listBoxMemberRoles.MultiColumn = true;
            this.listBoxMemberRoles.Name = "listBoxMemberRoles";
            this.tableLayoutPanelTeamMembers.SetRowSpan(this.listBoxMemberRoles, 3);
            this.listBoxMemberRoles.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxMemberRoles.Size = new System.Drawing.Size(198, 342);
            this.listBoxMemberRoles.TabIndex = 7;
            this.toolTip.SetToolTip(this.listBoxMemberRoles, "This list shows all the member\'s role on the team");
            this.listBoxMemberRoles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxMemberRoles_MouseDoubleClick);
            // 
            // textBoxVernacular
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxVernacular, 2);
            this.textBoxVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacular.Location = new System.Drawing.Point(104, 110);
            this.textBoxVernacular.Name = "textBoxVernacular";
            this.textBoxVernacular.Size = new System.Drawing.Size(207, 20);
            this.textBoxVernacular.TabIndex = 0;
            this.toolTip.SetToolTip(this.textBoxVernacular, "Enter the name of the language that the stories are going to be crafted into");
            this.textBoxVernacular.TextChanged += new System.EventHandler(this.textBoxVernacular_TextChanged);
            this.textBoxVernacular.Leave += new System.EventHandler(this.textBoxVernacular_Leave);
            // 
            // textBoxNationalBTLanguage
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxNationalBTLanguage, 2);
            this.textBoxNationalBTLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTLanguage.Location = new System.Drawing.Point(317, 110);
            this.textBoxNationalBTLanguage.Name = "textBoxNationalBTLanguage";
            this.textBoxNationalBTLanguage.Size = new System.Drawing.Size(208, 20);
            this.textBoxNationalBTLanguage.TabIndex = 7;
            this.toolTip.SetToolTip(this.textBoxNationalBTLanguage, "Enter the name of the language that the stories will be back-translated into by t" +
                    "he UNSs (i.e. typically, the National language)");
            this.textBoxNationalBTLanguage.TextChanged += new System.EventHandler(this.textBoxNationalBTLanguage_TextChanged);
            this.textBoxNationalBTLanguage.Leave += new System.EventHandler(this.textBoxNationalBTLanguage_Leave);
            // 
            // textBoxVernacularEthCode
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxVernacularEthCode, 2);
            this.textBoxVernacularEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacularEthCode.Location = new System.Drawing.Point(104, 136);
            this.textBoxVernacularEthCode.Name = "textBoxVernacularEthCode";
            this.textBoxVernacularEthCode.Size = new System.Drawing.Size(207, 20);
            this.textBoxVernacularEthCode.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxVernacularEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            this.textBoxVernacularEthCode.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxNationalBTEthCode
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxNationalBTEthCode, 2);
            this.textBoxNationalBTEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTEthCode.Location = new System.Drawing.Point(317, 136);
            this.textBoxNationalBTEthCode.Name = "textBoxNationalBTEthCode";
            this.textBoxNationalBTEthCode.Size = new System.Drawing.Size(208, 20);
            this.textBoxNationalBTEthCode.TabIndex = 8;
            this.toolTip.SetToolTip(this.textBoxNationalBTEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            // 
            // textBoxVernSentFullStop
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxVernSentFullStop, 2);
            this.textBoxVernSentFullStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernSentFullStop.Location = new System.Drawing.Point(104, 189);
            this.textBoxVernSentFullStop.Name = "textBoxVernSentFullStop";
            this.textBoxVernSentFullStop.Size = new System.Drawing.Size(207, 20);
            this.textBoxVernSentFullStop.TabIndex = 3;
            this.toolTip.SetToolTip(this.textBoxVernSentFullStop, resources.GetString("textBoxVernSentFullStop.ToolTip"));
            this.textBoxVernSentFullStop.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxVernSentFullStop.Leave += new System.EventHandler(this.textBoxSentFullStop_Leave);
            this.textBoxVernSentFullStop.Enter += new System.EventHandler(this.textBoxVernSentFullStop_Enter);
            // 
            // textBoxNationalBTSentFullStop
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxNationalBTSentFullStop, 2);
            this.textBoxNationalBTSentFullStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTSentFullStop.Location = new System.Drawing.Point(317, 189);
            this.textBoxNationalBTSentFullStop.Name = "textBoxNationalBTSentFullStop";
            this.textBoxNationalBTSentFullStop.Size = new System.Drawing.Size(208, 20);
            this.textBoxNationalBTSentFullStop.TabIndex = 10;
            this.toolTip.SetToolTip(this.textBoxNationalBTSentFullStop, resources.GetString("textBoxNationalBTSentFullStop.ToolTip"));
            this.textBoxNationalBTSentFullStop.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxNationalBTSentFullStop.Leave += new System.EventHandler(this.textBoxSentFullStop_Leave);
            this.textBoxNationalBTSentFullStop.Enter += new System.EventHandler(this.textBoxNationalBTSentFullStop_Enter);
            // 
            // checkBoxNationalLangBT
            // 
            this.checkBoxNationalLangBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxNationalLangBT.AutoSize = true;
            this.checkBoxNationalLangBT.Checked = true;
            this.checkBoxNationalLangBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.checkBoxNationalLangBT, 2);
            this.checkBoxNationalLangBT.Location = new System.Drawing.Point(317, 66);
            this.checkBoxNationalLangBT.Name = "checkBoxNationalLangBT";
            this.checkBoxNationalLangBT.Size = new System.Drawing.Size(170, 17);
            this.checkBoxNationalLangBT.TabIndex = 6;
            this.checkBoxNationalLangBT.Text = "Project will use a National BT?";
            this.toolTip.SetToolTip(this.checkBoxNationalLangBT, "Check this box to configure the project to use a national langauage (e.g. Hindi) " +
                    "as the primary back-translation for checking");
            this.checkBoxNationalLangBT.UseVisualStyleBackColor = true;
            this.checkBoxNationalLangBT.CheckedChanged += new System.EventHandler(this.checkBoxNationalLangBT_CheckedChanged);
            // 
            // checkBoxEnglishBT
            // 
            this.checkBoxEnglishBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxEnglishBT.AutoSize = true;
            this.checkBoxEnglishBT.Checked = true;
            this.checkBoxEnglishBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.checkBoxEnglishBT, 2);
            this.checkBoxEnglishBT.Location = new System.Drawing.Point(317, 281);
            this.checkBoxEnglishBT.Name = "checkBoxEnglishBT";
            this.checkBoxEnglishBT.Size = new System.Drawing.Size(171, 17);
            this.checkBoxEnglishBT.TabIndex = 13;
            this.checkBoxEnglishBT.Text = "Project will use an English BT?";
            this.toolTip.SetToolTip(this.checkBoxEnglishBT, "Check this box to configure the project to use English as a secondary back-transl" +
                    "ation for checking (primarily for the benefit of the consultant and coach)");
            this.checkBoxEnglishBT.UseVisualStyleBackColor = true;
            this.checkBoxEnglishBT.CheckedChanged += new System.EventHandler(this.checkBoxEnglishBT_CheckedChanged);
            // 
            // buttonVernacularFont
            // 
            this.buttonVernacularFont.Location = new System.Drawing.Point(104, 215);
            this.buttonVernacularFont.Name = "buttonVernacularFont";
            this.buttonVernacularFont.Size = new System.Drawing.Size(85, 23);
            this.buttonVernacularFont.TabIndex = 4;
            this.buttonVernacularFont.Text = "&Choose Font";
            this.buttonVernacularFont.UseVisualStyleBackColor = true;
            this.buttonVernacularFont.Click += new System.EventHandler(this.buttonVernacularFont_Click);
            // 
            // buttonNationalBTFont
            // 
            this.buttonNationalBTFont.Location = new System.Drawing.Point(317, 215);
            this.buttonNationalBTFont.Name = "buttonNationalBTFont";
            this.buttonNationalBTFont.Size = new System.Drawing.Size(85, 23);
            this.buttonNationalBTFont.TabIndex = 11;
            this.buttonNationalBTFont.Text = "Choose &Font";
            this.buttonNationalBTFont.UseVisualStyleBackColor = true;
            this.buttonNationalBTFont.Click += new System.EventHandler(this.buttonNationalBTFont_Click);
            // 
            // tabControlProjectMetaData
            // 
            this.tabControlProjectMetaData.Controls.Add(this.tabPageMemberList);
            this.tabControlProjectMetaData.Controls.Add(this.tabPageLanguageInfo);
            this.tabControlProjectMetaData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProjectMetaData.Location = new System.Drawing.Point(0, 0);
            this.tabControlProjectMetaData.Name = "tabControlProjectMetaData";
            this.tabControlProjectMetaData.SelectedIndex = 0;
            this.tabControlProjectMetaData.Size = new System.Drawing.Size(542, 444);
            this.tabControlProjectMetaData.TabIndex = 1;
            this.tabControlProjectMetaData.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlProjectMetaData_Selected);
            // 
            // tabPageMemberList
            // 
            this.tabPageMemberList.Controls.Add(this.tableLayoutPanelTeamMembers);
            this.tabPageMemberList.Location = new System.Drawing.Point(4, 22);
            this.tabPageMemberList.Name = "tabPageMemberList";
            this.tabPageMemberList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMemberList.Size = new System.Drawing.Size(534, 418);
            this.tabPageMemberList.TabIndex = 0;
            this.tabPageMemberList.Text = "Team Members";
            this.tabPageMemberList.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelTeamMembers
            // 
            this.tableLayoutPanelTeamMembers.ColumnCount = 4;
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonCancel, 2, 4);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonAddNewMember, 4, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonEditMember, 4, 2);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonDeleteMember, 4, 3);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.listBoxTeamMembers, 0, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.listBoxMemberRoles, 1, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.textBoxMemberNames, 0, 0);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.textBoxMemberRoles, 1, 0);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonOK, 1, 4);
            this.tableLayoutPanelTeamMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTeamMembers.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelTeamMembers.Name = "tableLayoutPanelTeamMembers";
            this.tableLayoutPanelTeamMembers.RowCount = 5;
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.Size = new System.Drawing.Size(528, 412);
            this.tableLayoutPanelTeamMembers.TabIndex = 0;
            // 
            // textBoxMemberNames
            // 
            this.textBoxMemberNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberNames.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberNames.Location = new System.Drawing.Point(3, 3);
            this.textBoxMemberNames.Name = "textBoxMemberNames";
            this.textBoxMemberNames.ReadOnly = true;
            this.textBoxMemberNames.Size = new System.Drawing.Size(199, 29);
            this.textBoxMemberNames.TabIndex = 8;
            this.textBoxMemberNames.Text = "Team Members";
            this.textBoxMemberNames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMemberRoles
            // 
            this.tableLayoutPanelTeamMembers.SetColumnSpan(this.textBoxMemberRoles, 2);
            this.textBoxMemberRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberRoles.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberRoles.Location = new System.Drawing.Point(208, 3);
            this.textBoxMemberRoles.Name = "textBoxMemberRoles";
            this.textBoxMemberRoles.ReadOnly = true;
            this.textBoxMemberRoles.Size = new System.Drawing.Size(198, 29);
            this.textBoxMemberRoles.TabIndex = 9;
            this.textBoxMemberRoles.Text = "Roles";
            this.textBoxMemberRoles.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabPageLanguageInfo
            // 
            this.tabPageLanguageInfo.Controls.Add(this.tableLayoutPanelLanguageInformation);
            this.tabPageLanguageInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageLanguageInfo.Name = "tabPageLanguageInfo";
            this.tabPageLanguageInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanguageInfo.Size = new System.Drawing.Size(534, 418);
            this.tabPageLanguageInfo.TabIndex = 2;
            this.tabPageLanguageInfo.Text = "Project/Language Settings";
            this.tabPageLanguageInfo.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelLanguageInformation
            // 
            this.tableLayoutPanelLanguageInformation.ColumnCount = 5;
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelLanguageName, 0, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernacular, 1, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTLanguage, 3, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelStoryLanguage, 1, 2);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelNationalBTLanguage, 3, 2);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelEthnologueCode, 0, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernacularEthCode, 1, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTEthCode, 3, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonInternationalBTFont, 3, 9);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelProjectName, 0, 0);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxProjectName, 1, 0);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelReturnToTeamMemberTabInstructions, 1, 10);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelSentenceTerm, 0, 6);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernSentFullStop, 1, 6);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTSentFullStop, 3, 6);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonVernacularFont, 1, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonNationalBTFont, 3, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelFont, 0, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.checkBoxVernacularRTL, 2, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.checkBoxNationalRTL, 4, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelKeyboard, 0, 5);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.comboBoxKeyboardVernacular, 1, 5);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.comboBoxKeyboardNationalBT, 3, 5);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.checkBoxNationalLangBT, 3, 1);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.checkBoxEnglishBT, 3, 8);
            this.tableLayoutPanelLanguageInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLanguageInformation.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLanguageInformation.Name = "tableLayoutPanelLanguageInformation";
            this.tableLayoutPanelLanguageInformation.RowCount = 11;
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLanguageInformation.Size = new System.Drawing.Size(528, 412);
            this.tableLayoutPanelLanguageInformation.TabIndex = 0;
            // 
            // labelLanguageName
            // 
            this.labelLanguageName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelLanguageName.AutoSize = true;
            this.labelLanguageName.Location = new System.Drawing.Point(3, 113);
            this.labelLanguageName.Name = "labelLanguageName";
            this.labelLanguageName.Size = new System.Drawing.Size(38, 13);
            this.labelLanguageName.TabIndex = 3;
            this.labelLanguageName.Text = "&Name:";
            // 
            // labelStoryLanguage
            // 
            this.labelStoryLanguage.AutoSize = true;
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.labelStoryLanguage, 2);
            this.labelStoryLanguage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStoryLanguage.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStoryLanguage.Location = new System.Drawing.Point(104, 86);
            this.labelStoryLanguage.Name = "labelStoryLanguage";
            this.labelStoryLanguage.Size = new System.Drawing.Size(207, 21);
            this.labelStoryLanguage.TabIndex = 2;
            this.labelStoryLanguage.Text = "Story Language";
            this.labelStoryLanguage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelNationalBTLanguage
            // 
            this.labelNationalBTLanguage.AutoSize = true;
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.labelNationalBTLanguage, 2);
            this.labelNationalBTLanguage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelNationalBTLanguage.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNationalBTLanguage.Location = new System.Drawing.Point(317, 86);
            this.labelNationalBTLanguage.Name = "labelNationalBTLanguage";
            this.labelNationalBTLanguage.Size = new System.Drawing.Size(208, 21);
            this.labelNationalBTLanguage.TabIndex = 6;
            this.labelNationalBTLanguage.Text = "National BT Language";
            this.labelNationalBTLanguage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelEthnologueCode
            // 
            this.labelEthnologueCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEthnologueCode.AutoSize = true;
            this.labelEthnologueCode.Location = new System.Drawing.Point(3, 139);
            this.labelEthnologueCode.Name = "labelEthnologueCode";
            this.labelEthnologueCode.Size = new System.Drawing.Size(62, 13);
            this.labelEthnologueCode.TabIndex = 5;
            this.labelEthnologueCode.Text = "&Ethn. code:";
            // 
            // buttonInternationalBTFont
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.buttonInternationalBTFont, 2);
            this.buttonInternationalBTFont.Location = new System.Drawing.Point(317, 304);
            this.buttonInternationalBTFont.Name = "buttonInternationalBTFont";
            this.buttonInternationalBTFont.Size = new System.Drawing.Size(108, 23);
            this.buttonInternationalBTFont.TabIndex = 14;
            this.buttonInternationalBTFont.Text = "&English BT Font";
            this.buttonInternationalBTFont.UseVisualStyleBackColor = true;
            this.buttonInternationalBTFont.Click += new System.EventHandler(this.buttonInternationalBTFont_Click);
            // 
            // labelProjectName
            // 
            this.labelProjectName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelProjectName.AutoSize = true;
            this.labelProjectName.Location = new System.Drawing.Point(3, 6);
            this.labelProjectName.Name = "labelProjectName";
            this.labelProjectName.Size = new System.Drawing.Size(74, 13);
            this.labelProjectName.TabIndex = 0;
            this.labelProjectName.Text = "&Project Name:";
            // 
            // textBoxProjectName
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxProjectName, 4);
            this.textBoxProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxProjectName.Location = new System.Drawing.Point(104, 3);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.ReadOnly = true;
            this.textBoxProjectName.Size = new System.Drawing.Size(421, 20);
            this.textBoxProjectName.TabIndex = 1;
            this.textBoxProjectName.TabStop = false;
            this.textBoxProjectName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelReturnToTeamMemberTabInstructions
            // 
            this.labelReturnToTeamMemberTabInstructions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.labelReturnToTeamMemberTabInstructions, 4);
            this.labelReturnToTeamMemberTabInstructions.Location = new System.Drawing.Point(104, 355);
            this.labelReturnToTeamMemberTabInstructions.Name = "labelReturnToTeamMemberTabInstructions";
            this.labelReturnToTeamMemberTabInstructions.Size = new System.Drawing.Size(298, 31);
            this.labelReturnToTeamMemberTabInstructions.TabIndex = 19;
            this.labelReturnToTeamMemberTabInstructions.Text = "When you\'re finished entering this information, click on the \'Team Members\' tab a" +
                "nd choose your name to Login";
            // 
            // labelSentenceTerm
            // 
            this.labelSentenceTerm.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSentenceTerm.AutoSize = true;
            this.labelSentenceTerm.Location = new System.Drawing.Point(3, 192);
            this.labelSentenceTerm.Name = "labelSentenceTerm";
            this.labelSentenceTerm.Size = new System.Drawing.Size(95, 13);
            this.labelSentenceTerm.TabIndex = 17;
            this.labelSentenceTerm.Text = "Sentence full stop:";
            // 
            // labelFont
            // 
            this.labelFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(3, 220);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(31, 13);
            this.labelFont.TabIndex = 8;
            this.labelFont.Text = "Font:";
            // 
            // checkBoxVernacularRTL
            // 
            this.checkBoxVernacularRTL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxVernacularRTL.AutoSize = true;
            this.checkBoxVernacularRTL.Location = new System.Drawing.Point(195, 218);
            this.checkBoxVernacularRTL.Name = "checkBoxVernacularRTL";
            this.checkBoxVernacularRTL.Size = new System.Drawing.Size(80, 17);
            this.checkBoxVernacularRTL.TabIndex = 5;
            this.checkBoxVernacularRTL.Text = "Right to left";
            this.toolTip.SetToolTip(this.checkBoxVernacularRTL, "Check this if your language reads from right-to-left (e.g. Arabic)");
            this.checkBoxVernacularRTL.UseVisualStyleBackColor = true;
            // 
            // checkBoxNationalRTL
            // 
            this.checkBoxNationalRTL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxNationalRTL.AutoSize = true;
            this.checkBoxNationalRTL.Location = new System.Drawing.Point(408, 218);
            this.checkBoxNationalRTL.Name = "checkBoxNationalRTL";
            this.checkBoxNationalRTL.Size = new System.Drawing.Size(80, 17);
            this.checkBoxNationalRTL.TabIndex = 12;
            this.checkBoxNationalRTL.Text = "Right to left";
            this.toolTip.SetToolTip(this.checkBoxNationalRTL, "Check this if your language reads from right-to-left (e.g. Arabic)");
            this.checkBoxNationalRTL.UseVisualStyleBackColor = true;
            // 
            // labelKeyboard
            // 
            this.labelKeyboard.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelKeyboard.AutoSize = true;
            this.labelKeyboard.Location = new System.Drawing.Point(3, 166);
            this.labelKeyboard.Name = "labelKeyboard";
            this.labelKeyboard.Size = new System.Drawing.Size(55, 13);
            this.labelKeyboard.TabIndex = 20;
            this.labelKeyboard.Text = "&Keyboard:";
            // 
            // comboBoxKeyboardVernacular
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.comboBoxKeyboardVernacular, 2);
            this.comboBoxKeyboardVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxKeyboardVernacular.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKeyboardVernacular.FormattingEnabled = true;
            this.comboBoxKeyboardVernacular.ItemHeight = 13;
            this.comboBoxKeyboardVernacular.Location = new System.Drawing.Point(104, 162);
            this.comboBoxKeyboardVernacular.Name = "comboBoxKeyboardVernacular";
            this.comboBoxKeyboardVernacular.Size = new System.Drawing.Size(207, 21);
            this.comboBoxKeyboardVernacular.TabIndex = 2;
            this.comboBoxKeyboardVernacular.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBoxKeyboard_DragDrop);
            // 
            // comboBoxKeyboardNationalBT
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.comboBoxKeyboardNationalBT, 2);
            this.comboBoxKeyboardNationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxKeyboardNationalBT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKeyboardNationalBT.FormattingEnabled = true;
            this.comboBoxKeyboardNationalBT.ItemHeight = 13;
            this.comboBoxKeyboardNationalBT.Location = new System.Drawing.Point(317, 162);
            this.comboBoxKeyboardNationalBT.Name = "comboBoxKeyboardNationalBT";
            this.comboBoxKeyboardNationalBT.Size = new System.Drawing.Size(208, 21);
            this.comboBoxKeyboardNationalBT.TabIndex = 9;
            this.comboBoxKeyboardNationalBT.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBoxKeyboard_DragDrop);
            // 
            // fontDialog
            // 
            this.fontDialog.ShowColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.Personal;
            // 
            // TeamMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(542, 444);
            this.Controls.Add(this.tabControlProjectMetaData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TeamMemberForm";
            this.Text = "Project Settings";
            this.tabControlProjectMetaData.ResumeLayout(false);
            this.tabPageMemberList.ResumeLayout(false);
            this.tableLayoutPanelTeamMembers.ResumeLayout(false);
            this.tableLayoutPanelTeamMembers.PerformLayout();
            this.tabPageLanguageInfo.ResumeLayout(false);
            this.tableLayoutPanelLanguageInformation.ResumeLayout(false);
            this.tableLayoutPanelLanguageInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabControl tabControlProjectMetaData;
        private System.Windows.Forms.TabPage tabPageMemberList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTeamMembers;
        private System.Windows.Forms.ListBox listBoxTeamMembers;
        private System.Windows.Forms.Button buttonAddNewMember;
        private System.Windows.Forms.Button buttonEditMember;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonDeleteMember;
        private System.Windows.Forms.ListBox listBoxMemberRoles;
        private System.Windows.Forms.TextBox textBoxMemberNames;
        private System.Windows.Forms.TextBox textBoxMemberRoles;
        private System.Windows.Forms.TabPage tabPageLanguageInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLanguageInformation;
        private System.Windows.Forms.Label labelLanguageName;
        private System.Windows.Forms.TextBox textBoxVernacular;
        private System.Windows.Forms.TextBox textBoxNationalBTLanguage;
        private System.Windows.Forms.Label labelStoryLanguage;
        private System.Windows.Forms.Label labelNationalBTLanguage;
        private System.Windows.Forms.Label labelEthnologueCode;
        private System.Windows.Forms.TextBox textBoxVernacularEthCode;
        private System.Windows.Forms.TextBox textBoxNationalBTEthCode;
        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button buttonVernacularFont;
        private System.Windows.Forms.Button buttonNationalBTFont;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label labelProjectName;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Label labelSentenceTerm;
        private System.Windows.Forms.TextBox textBoxVernSentFullStop;
        private System.Windows.Forms.TextBox textBoxNationalBTSentFullStop;
        private System.Windows.Forms.Label labelReturnToTeamMemberTabInstructions;
        private System.Windows.Forms.Button buttonInternationalBTFont;
        private System.Windows.Forms.CheckBox checkBoxVernacularRTL;
        private System.Windows.Forms.CheckBox checkBoxNationalRTL;
        private System.Windows.Forms.Label labelKeyboard;
        private System.Windows.Forms.ComboBox comboBoxKeyboardVernacular;
        private System.Windows.Forms.ComboBox comboBoxKeyboardNationalBT;
        private System.Windows.Forms.CheckBox checkBoxNationalLangBT;
        private System.Windows.Forms.CheckBox checkBoxEnglishBT;
    }
}