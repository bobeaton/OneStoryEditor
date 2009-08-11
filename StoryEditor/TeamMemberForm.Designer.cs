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
            this.buttonVernacularFont = new System.Windows.Forms.Button();
            this.buttonNationalBTFont = new System.Windows.Forms.Button();
            this.buttonInternationalBTFont = new System.Windows.Forms.Button();
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
            this.labelFont = new System.Windows.Forms.Label();
            this.labelEnglishBT = new System.Windows.Forms.Label();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.labelSentenceTerm = new System.Windows.Forms.Label();
            this.textBoxVernSentFullStop = new System.Windows.Forms.TextBox();
            this.textBoxNationalBTSentFullStop = new System.Windows.Forms.TextBox();
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
            this.listBoxTeamMembers.Size = new System.Drawing.Size(158, 342);
            this.listBoxTeamMembers.TabIndex = 0;
            this.toolTip.SetToolTip(this.listBoxTeamMembers, "This list shows all the members of the team");
            this.listBoxTeamMembers.SelectedIndexChanged += new System.EventHandler(this.listBoxTeamMembers_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(168, 386);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&Login";
            this.toolTip.SetToolTip(this.buttonOK, "Click to login as the selected member");
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(249, 386);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.toolTip.SetToolTip(this.buttonCancel, "Click to cancel this dialog");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAddNewMember
            // 
            this.buttonAddNewMember.Location = new System.Drawing.Point(331, 38);
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
            this.buttonEditMember.Location = new System.Drawing.Point(331, 74);
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
            this.buttonDeleteMember.Location = new System.Drawing.Point(331, 110);
            this.buttonDeleteMember.Name = "buttonDeleteMember";
            this.buttonDeleteMember.Size = new System.Drawing.Size(111, 30);
            this.buttonDeleteMember.TabIndex = 5;
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
            this.listBoxMemberRoles.Location = new System.Drawing.Point(167, 38);
            this.listBoxMemberRoles.MultiColumn = true;
            this.listBoxMemberRoles.Name = "listBoxMemberRoles";
            this.tableLayoutPanelTeamMembers.SetRowSpan(this.listBoxMemberRoles, 3);
            this.listBoxMemberRoles.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxMemberRoles.Size = new System.Drawing.Size(158, 342);
            this.listBoxMemberRoles.TabIndex = 7;
            this.toolTip.SetToolTip(this.listBoxMemberRoles, "This list shows all the member\'s role on the team");
            // 
            // textBoxVernacular
            // 
            this.textBoxVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacular.Location = new System.Drawing.Point(104, 89);
            this.textBoxVernacular.Name = "textBoxVernacular";
            this.textBoxVernacular.Size = new System.Drawing.Size(176, 20);
            this.textBoxVernacular.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxVernacular, "Enter the name of the language that the stories are going to be crafted into");
            // 
            // textBoxNationalBTLanguage
            // 
            this.textBoxNationalBTLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTLanguage.Location = new System.Drawing.Point(286, 89);
            this.textBoxNationalBTLanguage.Name = "textBoxNationalBTLanguage";
            this.textBoxNationalBTLanguage.Size = new System.Drawing.Size(176, 20);
            this.textBoxNationalBTLanguage.TabIndex = 2;
            this.toolTip.SetToolTip(this.textBoxNationalBTLanguage, "Enter the name of the language that the stories will be back-translated into by t" +
                    "he UNSs (i.e. typically, the National language)");
            // 
            // textBoxVernacularEthCode
            // 
            this.textBoxVernacularEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacularEthCode.Location = new System.Drawing.Point(104, 115);
            this.textBoxVernacularEthCode.Name = "textBoxVernacularEthCode";
            this.textBoxVernacularEthCode.Size = new System.Drawing.Size(176, 20);
            this.textBoxVernacularEthCode.TabIndex = 6;
            this.toolTip.SetToolTip(this.textBoxVernacularEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            // 
            // textBoxNationalBTEthCode
            // 
            this.textBoxNationalBTEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTEthCode.Location = new System.Drawing.Point(286, 115);
            this.textBoxNationalBTEthCode.Name = "textBoxNationalBTEthCode";
            this.textBoxNationalBTEthCode.Size = new System.Drawing.Size(176, 20);
            this.textBoxNationalBTEthCode.TabIndex = 7;
            this.toolTip.SetToolTip(this.textBoxNationalBTEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            // 
            // buttonVernacularFont
            // 
            this.buttonVernacularFont.Location = new System.Drawing.Point(104, 141);
            this.buttonVernacularFont.Name = "buttonVernacularFont";
            this.buttonVernacularFont.Size = new System.Drawing.Size(75, 23);
            this.buttonVernacularFont.TabIndex = 9;
            this.buttonVernacularFont.Text = "&Choose Font";
            this.buttonVernacularFont.UseVisualStyleBackColor = true;
            this.buttonVernacularFont.Click += new System.EventHandler(this.buttonVernacularFont_Click);
            // 
            // buttonNationalBTFont
            // 
            this.buttonNationalBTFont.Location = new System.Drawing.Point(286, 141);
            this.buttonNationalBTFont.Name = "buttonNationalBTFont";
            this.buttonNationalBTFont.Size = new System.Drawing.Size(75, 23);
            this.buttonNationalBTFont.TabIndex = 10;
            this.buttonNationalBTFont.Text = "Choose &Font";
            this.buttonNationalBTFont.UseVisualStyleBackColor = true;
            this.buttonNationalBTFont.Click += new System.EventHandler(this.buttonNationalBTFont_Click);
            // 
            // buttonInternationalBTFont
            // 
            this.buttonInternationalBTFont.Location = new System.Drawing.Point(286, 284);
            this.buttonInternationalBTFont.Name = "buttonInternationalBTFont";
            this.buttonInternationalBTFont.Size = new System.Drawing.Size(75, 23);
            this.buttonInternationalBTFont.TabIndex = 12;
            this.buttonInternationalBTFont.Text = "Choo&se Font";
            this.buttonInternationalBTFont.UseVisualStyleBackColor = true;
            this.buttonInternationalBTFont.Click += new System.EventHandler(this.buttonInternationalBTFont_Click);
            // 
            // tabControlProjectMetaData
            // 
            this.tabControlProjectMetaData.Controls.Add(this.tabPageMemberList);
            this.tabControlProjectMetaData.Controls.Add(this.tabPageLanguageInfo);
            this.tabControlProjectMetaData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProjectMetaData.Location = new System.Drawing.Point(0, 0);
            this.tabControlProjectMetaData.Name = "tabControlProjectMetaData";
            this.tabControlProjectMetaData.SelectedIndex = 0;
            this.tabControlProjectMetaData.Size = new System.Drawing.Size(479, 444);
            this.tabControlProjectMetaData.TabIndex = 1;
            this.tabControlProjectMetaData.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlProjectMetaData_Selected);
            // 
            // tabPageMemberList
            // 
            this.tabPageMemberList.Controls.Add(this.tableLayoutPanelTeamMembers);
            this.tabPageMemberList.Location = new System.Drawing.Point(4, 22);
            this.tabPageMemberList.Name = "tabPageMemberList";
            this.tabPageMemberList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMemberList.Size = new System.Drawing.Size(451, 418);
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
            this.tableLayoutPanelTeamMembers.Size = new System.Drawing.Size(445, 412);
            this.tableLayoutPanelTeamMembers.TabIndex = 0;
            // 
            // textBoxMemberNames
            // 
            this.textBoxMemberNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberNames.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberNames.Location = new System.Drawing.Point(3, 3);
            this.textBoxMemberNames.Name = "textBoxMemberNames";
            this.textBoxMemberNames.ReadOnly = true;
            this.textBoxMemberNames.Size = new System.Drawing.Size(158, 29);
            this.textBoxMemberNames.TabIndex = 8;
            this.textBoxMemberNames.Text = "Team Members";
            this.textBoxMemberNames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMemberRoles
            // 
            this.tableLayoutPanelTeamMembers.SetColumnSpan(this.textBoxMemberRoles, 2);
            this.textBoxMemberRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberRoles.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberRoles.Location = new System.Drawing.Point(167, 3);
            this.textBoxMemberRoles.Name = "textBoxMemberRoles";
            this.textBoxMemberRoles.ReadOnly = true;
            this.textBoxMemberRoles.Size = new System.Drawing.Size(158, 29);
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
            this.tabPageLanguageInfo.Size = new System.Drawing.Size(471, 418);
            this.tabPageLanguageInfo.TabIndex = 2;
            this.tabPageLanguageInfo.Text = "Project/Language Settings";
            this.tabPageLanguageInfo.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelLanguageInformation
            // 
            this.tableLayoutPanelLanguageInformation.ColumnCount = 3;
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLanguageInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelLanguageName, 0, 2);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernacular, 1, 2);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTLanguage, 2, 2);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelStoryLanguage, 1, 1);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelNationalBTLanguage, 2, 1);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelEthnologueCode, 0, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernacularEthCode, 1, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTEthCode, 2, 3);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelFont, 0, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonVernacularFont, 1, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonNationalBTFont, 2, 4);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelEnglishBT, 1, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonInternationalBTFont, 2, 7);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelProjectName, 0, 0);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxProjectName, 1, 0);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelSentenceTerm, 0, 5);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxVernSentFullStop, 1, 5);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxNationalBTSentFullStop, 2, 5);
            this.tableLayoutPanelLanguageInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLanguageInformation.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLanguageInformation.Name = "tableLayoutPanelLanguageInformation";
            this.tableLayoutPanelLanguageInformation.RowCount = 9;
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLanguageInformation.Size = new System.Drawing.Size(465, 412);
            this.tableLayoutPanelLanguageInformation.TabIndex = 0;
            // 
            // labelLanguageName
            // 
            this.labelLanguageName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelLanguageName.AutoSize = true;
            this.labelLanguageName.Location = new System.Drawing.Point(3, 92);
            this.labelLanguageName.Name = "labelLanguageName";
            this.labelLanguageName.Size = new System.Drawing.Size(38, 13);
            this.labelLanguageName.TabIndex = 0;
            this.labelLanguageName.Text = "&Name:";
            // 
            // labelStoryLanguage
            // 
            this.labelStoryLanguage.AutoSize = true;
            this.labelStoryLanguage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStoryLanguage.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStoryLanguage.Location = new System.Drawing.Point(104, 65);
            this.labelStoryLanguage.Name = "labelStoryLanguage";
            this.labelStoryLanguage.Size = new System.Drawing.Size(176, 21);
            this.labelStoryLanguage.TabIndex = 3;
            this.labelStoryLanguage.Text = "Story Language";
            this.labelStoryLanguage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelNationalBTLanguage
            // 
            this.labelNationalBTLanguage.AutoSize = true;
            this.labelNationalBTLanguage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelNationalBTLanguage.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNationalBTLanguage.Location = new System.Drawing.Point(286, 65);
            this.labelNationalBTLanguage.Name = "labelNationalBTLanguage";
            this.labelNationalBTLanguage.Size = new System.Drawing.Size(176, 21);
            this.labelNationalBTLanguage.TabIndex = 4;
            this.labelNationalBTLanguage.Text = "National BT Language";
            this.labelNationalBTLanguage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelEthnologueCode
            // 
            this.labelEthnologueCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEthnologueCode.AutoSize = true;
            this.labelEthnologueCode.Location = new System.Drawing.Point(3, 118);
            this.labelEthnologueCode.Name = "labelEthnologueCode";
            this.labelEthnologueCode.Size = new System.Drawing.Size(62, 13);
            this.labelEthnologueCode.TabIndex = 5;
            this.labelEthnologueCode.Text = "&Ethn. code:";
            // 
            // labelFont
            // 
            this.labelFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(3, 146);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(31, 13);
            this.labelFont.TabIndex = 8;
            this.labelFont.Text = "Font:";
            // 
            // labelEnglishBT
            // 
            this.labelEnglishBT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelEnglishBT.AutoSize = true;
            this.labelEnglishBT.Location = new System.Drawing.Point(195, 289);
            this.labelEnglishBT.Name = "labelEnglishBT";
            this.labelEnglishBT.Size = new System.Drawing.Size(85, 13);
            this.labelEnglishBT.TabIndex = 11;
            this.labelEnglishBT.Text = "English BT Font:";
            // 
            // labelProjectName
            // 
            this.labelProjectName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelProjectName.AutoSize = true;
            this.labelProjectName.Location = new System.Drawing.Point(3, 6);
            this.labelProjectName.Name = "labelProjectName";
            this.labelProjectName.Size = new System.Drawing.Size(74, 13);
            this.labelProjectName.TabIndex = 15;
            this.labelProjectName.Text = "&Project Name:";
            // 
            // textBoxProjectName
            // 
            this.tableLayoutPanelLanguageInformation.SetColumnSpan(this.textBoxProjectName, 2);
            this.textBoxProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxProjectName.Location = new System.Drawing.Point(104, 3);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(358, 20);
            this.textBoxProjectName.TabIndex = 16;
            // 
            // fontDialog
            // 
            this.fontDialog.ShowColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.Personal;
            // 
            // labelSentenceTerm
            // 
            this.labelSentenceTerm.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSentenceTerm.AutoSize = true;
            this.labelSentenceTerm.Location = new System.Drawing.Point(3, 173);
            this.labelSentenceTerm.Name = "labelSentenceTerm";
            this.labelSentenceTerm.Size = new System.Drawing.Size(95, 13);
            this.labelSentenceTerm.TabIndex = 17;
            this.labelSentenceTerm.Text = "Sentence full stop:";
            // 
            // textBoxVernSentFullStop
            // 
            this.textBoxVernSentFullStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernSentFullStop.Location = new System.Drawing.Point(104, 170);
            this.textBoxVernSentFullStop.Name = "textBoxVernSentFullStop";
            this.textBoxVernSentFullStop.Size = new System.Drawing.Size(176, 20);
            this.textBoxVernSentFullStop.TabIndex = 18;
            this.toolTip.SetToolTip(this.textBoxVernSentFullStop, "Enter the punctional character used in this language to end a sentence (e.g. Engl" +
                    "ish is \'.\', Hindi is \'।\')");
            // 
            // textBoxNationalBTSentFullStop
            // 
            this.textBoxNationalBTSentFullStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTSentFullStop.Location = new System.Drawing.Point(286, 170);
            this.textBoxNationalBTSentFullStop.Name = "textBoxNationalBTSentFullStop";
            this.textBoxNationalBTSentFullStop.Size = new System.Drawing.Size(176, 20);
            this.textBoxNationalBTSentFullStop.TabIndex = 18;
            this.toolTip.SetToolTip(this.textBoxNationalBTSentFullStop, "Enter the punctional character used in this language to end a sentence (e.g. Engl" +
                    "ish is \'.\', Hindi is \'।\')");
            // 
            // TeamMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(479, 444);
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
        private System.Windows.Forms.Label labelEnglishBT;
        private System.Windows.Forms.Button buttonInternationalBTFont;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label labelProjectName;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Label labelSentenceTerm;
        private System.Windows.Forms.TextBox textBoxVernSentFullStop;
        private System.Windows.Forms.TextBox textBoxNationalBTSentFullStop;
    }
}