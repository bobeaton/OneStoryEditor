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
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBoxRole = new System.Windows.Forms.GroupBox();
            this.radioButtonJustViewing = new System.Windows.Forms.RadioButton();
            this.radioButtonCoach = new System.Windows.Forms.RadioButton();
            this.radioButtonIndependentConsultant = new System.Windows.Forms.RadioButton();
            this.radioButtonConsultantInTraining = new System.Windows.Forms.RadioButton();
            this.radioButtonUNS = new System.Windows.Forms.RadioButton();
            this.radioButtonStoryCrafter = new System.Windows.Forms.RadioButton();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.textBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.textBoxAltPhone = new System.Windows.Forms.TextBox();
            this.textBoxSkypeID = new System.Windows.Forms.TextBox();
            this.textBoxTeamViewer = new System.Windows.Forms.TextBox();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
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
            this.tabPageEditMember = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelMemberInformation = new System.Windows.Forms.TableLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelPhoneNumber = new System.Windows.Forms.Label();
            this.labelAltPhone = new System.Windows.Forms.Label();
            this.labelSkype = new System.Windows.Forms.Label();
            this.labelTeamViewerID = new System.Windows.Forms.Label();
            this.labelAddress = new System.Windows.Forms.Label();
            this.tabPageLanguageInfo = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelLanguageInformation = new System.Windows.Forms.TableLayoutPanel();
            this.labelLanguageName = new System.Windows.Forms.Label();
            this.labelStoryLanguage = new System.Windows.Forms.Label();
            this.labelNationalBTLanguage = new System.Windows.Forms.Label();
            this.labelEthnologueCode = new System.Windows.Forms.Label();
            this.labelFont = new System.Windows.Forms.Label();
            this.labelEnglishBT = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.groupBoxRole.SuspendLayout();
            this.tabControlProjectMetaData.SuspendLayout();
            this.tabPageMemberList.SuspendLayout();
            this.tableLayoutPanelTeamMembers.SuspendLayout();
            this.tabPageEditMember.SuspendLayout();
            this.tableLayoutPanelMemberInformation.SuspendLayout();
            this.tabPageLanguageInfo.SuspendLayout();
            this.tableLayoutPanelLanguageInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxName
            // 
            this.textBoxName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxName.Location = new System.Drawing.Point(92, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(350, 20);
            this.textBoxName.TabIndex = 0;
            this.toolTip.SetToolTip(this.textBoxName, "Enter your name here");
            this.textBoxName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // groupBoxRole
            // 
            this.groupBoxRole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRole.Controls.Add(this.radioButtonJustViewing);
            this.groupBoxRole.Controls.Add(this.radioButtonCoach);
            this.groupBoxRole.Controls.Add(this.radioButtonIndependentConsultant);
            this.groupBoxRole.Controls.Add(this.radioButtonConsultantInTraining);
            this.groupBoxRole.Controls.Add(this.radioButtonUNS);
            this.groupBoxRole.Controls.Add(this.radioButtonStoryCrafter);
            this.groupBoxRole.Location = new System.Drawing.Point(92, 29);
            this.groupBoxRole.Name = "groupBoxRole";
            this.groupBoxRole.Size = new System.Drawing.Size(350, 170);
            this.groupBoxRole.TabIndex = 3;
            this.groupBoxRole.TabStop = false;
            this.groupBoxRole.Text = "What role do you have in the team?";
            this.toolTip.SetToolTip(this.groupBoxRole, "This area is for choosing the role you play on the team");
            // 
            // radioButtonJustViewing
            // 
            this.radioButtonJustViewing.AutoSize = true;
            this.radioButtonJustViewing.Location = new System.Drawing.Point(101, 140);
            this.radioButtonJustViewing.Name = "radioButtonJustViewing";
            this.radioButtonJustViewing.Size = new System.Drawing.Size(85, 17);
            this.radioButtonJustViewing.TabIndex = 6;
            this.radioButtonJustViewing.TabStop = true;
            this.radioButtonJustViewing.Text = "&Just Looking";
            this.toolTip.SetToolTip(this.radioButtonJustViewing, "Choose if you just want to look at the project data (note: you will not be able t" +
                    "o make changes)");
            this.radioButtonJustViewing.UseVisualStyleBackColor = true;
            this.radioButtonJustViewing.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonCoach
            // 
            this.radioButtonCoach.AutoSize = true;
            this.radioButtonCoach.Location = new System.Drawing.Point(101, 116);
            this.radioButtonCoach.Name = "radioButtonCoach";
            this.radioButtonCoach.Size = new System.Drawing.Size(56, 17);
            this.radioButtonCoach.TabIndex = 5;
            this.radioButtonCoach.TabStop = true;
            this.radioButtonCoach.Text = "Coac&h";
            this.toolTip.SetToolTip(this.radioButtonCoach, "Choose if you are the coach for the project");
            this.radioButtonCoach.UseVisualStyleBackColor = true;
            this.radioButtonCoach.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonIndependentConsultant
            // 
            this.radioButtonIndependentConsultant.AutoSize = true;
            this.radioButtonIndependentConsultant.Location = new System.Drawing.Point(101, 92);
            this.radioButtonIndependentConsultant.Name = "radioButtonIndependentConsultant";
            this.radioButtonIndependentConsultant.Size = new System.Drawing.Size(138, 17);
            this.radioButtonIndependentConsultant.TabIndex = 4;
            this.radioButtonIndependentConsultant.TabStop = true;
            this.radioButtonIndependentConsultant.Text = "&Independent Consultant";
            this.toolTip.SetToolTip(this.radioButtonIndependentConsultant, "Choose if you are an independent consultant (i.e. one working without a coach)");
            this.radioButtonIndependentConsultant.UseVisualStyleBackColor = true;
            this.radioButtonIndependentConsultant.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonConsultantInTraining
            // 
            this.radioButtonConsultantInTraining.AutoSize = true;
            this.radioButtonConsultantInTraining.Location = new System.Drawing.Point(101, 68);
            this.radioButtonConsultantInTraining.Name = "radioButtonConsultantInTraining";
            this.radioButtonConsultantInTraining.Size = new System.Drawing.Size(123, 17);
            this.radioButtonConsultantInTraining.TabIndex = 3;
            this.radioButtonConsultantInTraining.TabStop = true;
            this.radioButtonConsultantInTraining.Text = "&Consultant-in-training";
            this.toolTip.SetToolTip(this.radioButtonConsultantInTraining, "Choose if you are a/the consultant-in-training for the project (i.e. one who has " +
                    "a coach)");
            this.radioButtonConsultantInTraining.UseVisualStyleBackColor = true;
            this.radioButtonConsultantInTraining.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonUNS
            // 
            this.radioButtonUNS.AutoSize = true;
            this.radioButtonUNS.Location = new System.Drawing.Point(101, 44);
            this.radioButtonUNS.Name = "radioButtonUNS";
            this.radioButtonUNS.Size = new System.Drawing.Size(126, 17);
            this.radioButtonUNS.TabIndex = 2;
            this.radioButtonUNS.Text = "&Testing Helper (UNS)";
            this.toolTip.SetToolTip(this.radioButtonUNS, resources.GetString("radioButtonUNS.ToolTip"));
            this.radioButtonUNS.UseVisualStyleBackColor = true;
            this.radioButtonUNS.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonStoryCrafter
            // 
            this.radioButtonStoryCrafter.AutoSize = true;
            this.radioButtonStoryCrafter.Checked = true;
            this.radioButtonStoryCrafter.Location = new System.Drawing.Point(101, 20);
            this.radioButtonStoryCrafter.Name = "radioButtonStoryCrafter";
            this.radioButtonStoryCrafter.Size = new System.Drawing.Size(83, 17);
            this.radioButtonStoryCrafter.TabIndex = 1;
            this.radioButtonStoryCrafter.TabStop = true;
            this.radioButtonStoryCrafter.Text = "&Story Crafter";
            this.toolTip.SetToolTip(this.radioButtonStoryCrafter, "Choose if you are one of the mother-tongue speakers involved in story crafting or" +
                    " conducting testing");
            this.radioButtonStoryCrafter.UseVisualStyleBackColor = true;
            this.radioButtonStoryCrafter.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEmail.Location = new System.Drawing.Point(92, 205);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(350, 20);
            this.textBoxEmail.TabIndex = 5;
            this.toolTip.SetToolTip(this.textBoxEmail, "Enter your email address if you have one");
            this.textBoxEmail.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(92, 231);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(350, 20);
            this.textBoxPhoneNumber.TabIndex = 7;
            this.toolTip.SetToolTip(this.textBoxPhoneNumber, "Enter the phone number (STD, long distance) by which you can be reached");
            this.textBoxPhoneNumber.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxAltPhone
            // 
            this.textBoxAltPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAltPhone.Location = new System.Drawing.Point(92, 257);
            this.textBoxAltPhone.Name = "textBoxAltPhone";
            this.textBoxAltPhone.Size = new System.Drawing.Size(350, 20);
            this.textBoxAltPhone.TabIndex = 9;
            this.toolTip.SetToolTip(this.textBoxAltPhone, "Enter an alternate phone number by which you can be reached");
            this.textBoxAltPhone.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxSkypeID
            // 
            this.textBoxSkypeID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSkypeID.Location = new System.Drawing.Point(92, 283);
            this.textBoxSkypeID.Name = "textBoxSkypeID";
            this.textBoxSkypeID.Size = new System.Drawing.Size(350, 20);
            this.textBoxSkypeID.TabIndex = 11;
            this.toolTip.SetToolTip(this.textBoxSkypeID, "Enter your Skype ID");
            this.textBoxSkypeID.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxTeamViewer
            // 
            this.textBoxTeamViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTeamViewer.Location = new System.Drawing.Point(92, 309);
            this.textBoxTeamViewer.Name = "textBoxTeamViewer";
            this.textBoxTeamViewer.Size = new System.Drawing.Size(350, 20);
            this.textBoxTeamViewer.TabIndex = 13;
            this.toolTip.SetToolTip(this.textBoxTeamViewer, "Enter your TeamViewer ID (e.g. 248 273 795)");
            this.textBoxTeamViewer.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAddress.Location = new System.Drawing.Point(92, 335);
            this.textBoxAddress.Multiline = true;
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(350, 74);
            this.textBoxAddress.TabIndex = 15;
            this.toolTip.SetToolTip(this.textBoxAddress, "Enter your mailing address");
            this.textBoxAddress.TextChanged += new System.EventHandler(this.textBox_TextChanged);
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
            this.textBoxVernacular.Location = new System.Drawing.Point(83, 89);
            this.textBoxVernacular.Name = "textBoxVernacular";
            this.textBoxVernacular.Size = new System.Drawing.Size(176, 20);
            this.textBoxVernacular.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxVernacular, "Enter the name of the language that the stories are going to be crafted into");
            // 
            // textBoxNationalBTLanguage
            // 
            this.textBoxNationalBTLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTLanguage.Location = new System.Drawing.Point(265, 89);
            this.textBoxNationalBTLanguage.Name = "textBoxNationalBTLanguage";
            this.textBoxNationalBTLanguage.Size = new System.Drawing.Size(177, 20);
            this.textBoxNationalBTLanguage.TabIndex = 2;
            this.toolTip.SetToolTip(this.textBoxNationalBTLanguage, "Enter the name of the language that the stories will be back-translated into by t" +
                    "he UNSs (i.e. typically, the National language)");
            // 
            // textBoxVernacularEthCode
            // 
            this.textBoxVernacularEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacularEthCode.Location = new System.Drawing.Point(83, 115);
            this.textBoxVernacularEthCode.Name = "textBoxVernacularEthCode";
            this.textBoxVernacularEthCode.Size = new System.Drawing.Size(176, 20);
            this.textBoxVernacularEthCode.TabIndex = 6;
            this.toolTip.SetToolTip(this.textBoxVernacularEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            // 
            // textBoxNationalBTEthCode
            // 
            this.textBoxNationalBTEthCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBTEthCode.Location = new System.Drawing.Point(265, 115);
            this.textBoxNationalBTEthCode.Name = "textBoxNationalBTEthCode";
            this.textBoxNationalBTEthCode.Size = new System.Drawing.Size(177, 20);
            this.textBoxNationalBTEthCode.TabIndex = 7;
            this.toolTip.SetToolTip(this.textBoxNationalBTEthCode, "Enter the 2-3 letter code for this language (e.g. English is \'en\', Hindi is \'hi\')" +
                    "");
            // 
            // buttonVernacularFont
            // 
            this.buttonVernacularFont.Location = new System.Drawing.Point(83, 141);
            this.buttonVernacularFont.Name = "buttonVernacularFont";
            this.buttonVernacularFont.Size = new System.Drawing.Size(75, 23);
            this.buttonVernacularFont.TabIndex = 9;
            this.buttonVernacularFont.Text = "&Choose Font";
            this.toolTip.SetToolTip(this.buttonVernacularFont, "Click here to choose the font, size, and color of the Story language text");
            this.buttonVernacularFont.UseVisualStyleBackColor = true;
            this.buttonVernacularFont.Click += new System.EventHandler(this.buttonVernacularFont_Click);
            // 
            // buttonNationalBTFont
            // 
            this.buttonNationalBTFont.Location = new System.Drawing.Point(265, 141);
            this.buttonNationalBTFont.Name = "buttonNationalBTFont";
            this.buttonNationalBTFont.Size = new System.Drawing.Size(75, 23);
            this.buttonNationalBTFont.TabIndex = 10;
            this.buttonNationalBTFont.Text = "Choose &Font";
            this.toolTip.SetToolTip(this.buttonNationalBTFont, "Click here to choose the font, size, and color of the National language back tran" +
                    "slation text");
            this.buttonNationalBTFont.UseVisualStyleBackColor = true;
            this.buttonNationalBTFont.Click += new System.EventHandler(this.buttonNationalBTFont_Click);
            // 
            // buttonInternationalBTFont
            // 
            this.buttonInternationalBTFont.Location = new System.Drawing.Point(265, 258);
            this.buttonInternationalBTFont.Name = "buttonInternationalBTFont";
            this.buttonInternationalBTFont.Size = new System.Drawing.Size(75, 23);
            this.buttonInternationalBTFont.TabIndex = 12;
            this.buttonInternationalBTFont.Text = "Choo&se Font";
            this.toolTip.SetToolTip(this.buttonInternationalBTFont, "Click here to choose the font, size, and color of the English language back trans" +
                    "lation text");
            this.buttonInternationalBTFont.UseVisualStyleBackColor = true;
            this.buttonInternationalBTFont.Click += new System.EventHandler(this.buttonInternationalBTFont_Click);
            // 
            // tabControlProjectMetaData
            // 
            this.tabControlProjectMetaData.Controls.Add(this.tabPageMemberList);
            this.tabControlProjectMetaData.Controls.Add(this.tabPageEditMember);
            this.tabControlProjectMetaData.Controls.Add(this.tabPageLanguageInfo);
            this.tabControlProjectMetaData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProjectMetaData.Location = new System.Drawing.Point(0, 0);
            this.tabControlProjectMetaData.Name = "tabControlProjectMetaData";
            this.tabControlProjectMetaData.SelectedIndex = 0;
            this.tabControlProjectMetaData.Size = new System.Drawing.Size(459, 444);
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
            // tabPageEditMember
            // 
            this.tabPageEditMember.Controls.Add(this.tableLayoutPanelMemberInformation);
            this.tabPageEditMember.Location = new System.Drawing.Point(4, 22);
            this.tabPageEditMember.Name = "tabPageEditMember";
            this.tabPageEditMember.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEditMember.Size = new System.Drawing.Size(451, 418);
            this.tabPageEditMember.TabIndex = 1;
            this.tabPageEditMember.Text = "Edit Member";
            this.tabPageEditMember.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMemberInformation
            // 
            this.tableLayoutPanelMemberInformation.ColumnCount = 2;
            this.tableLayoutPanelMemberInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMemberInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxName, 1, 0);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.groupBoxRole, 1, 1);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelEmail, 0, 2);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxEmail, 1, 2);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelPhoneNumber, 0, 3);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxPhoneNumber, 1, 3);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelAltPhone, 0, 4);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxAltPhone, 1, 4);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelSkype, 0, 5);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxSkypeID, 1, 5);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelTeamViewerID, 0, 6);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxTeamViewer, 1, 6);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelAddress, 0, 7);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxAddress, 1, 7);
            this.tableLayoutPanelMemberInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMemberInformation.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelMemberInformation.Name = "tableLayoutPanelMemberInformation";
            this.tableLayoutPanelMemberInformation.RowCount = 8;
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMemberInformation.Size = new System.Drawing.Size(445, 412);
            this.tableLayoutPanelMemberInformation.TabIndex = 1;
            // 
            // labelName
            // 
            this.labelName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(3, 6);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "&Name:";
            // 
            // labelEmail
            // 
            this.labelEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(3, 208);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(35, 13);
            this.labelEmail.TabIndex = 4;
            this.labelEmail.Text = "&Email:";
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(3, 234);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(41, 13);
            this.labelPhoneNumber.TabIndex = 6;
            this.labelPhoneNumber.Text = "&Phone:";
            // 
            // labelAltPhone
            // 
            this.labelAltPhone.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAltPhone.AutoSize = true;
            this.labelAltPhone.Location = new System.Drawing.Point(3, 260);
            this.labelAltPhone.Name = "labelAltPhone";
            this.labelAltPhone.Size = new System.Drawing.Size(56, 13);
            this.labelAltPhone.TabIndex = 8;
            this.labelAltPhone.Text = "Al&t Phone:";
            // 
            // labelSkype
            // 
            this.labelSkype.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSkype.AutoSize = true;
            this.labelSkype.Location = new System.Drawing.Point(3, 286);
            this.labelSkype.Name = "labelSkype";
            this.labelSkype.Size = new System.Drawing.Size(54, 13);
            this.labelSkype.TabIndex = 10;
            this.labelSkype.Text = "S&kype ID:";
            // 
            // labelTeamViewerID
            // 
            this.labelTeamViewerID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTeamViewerID.AutoSize = true;
            this.labelTeamViewerID.Location = new System.Drawing.Point(3, 312);
            this.labelTeamViewerID.Name = "labelTeamViewerID";
            this.labelTeamViewerID.Size = new System.Drawing.Size(83, 13);
            this.labelTeamViewerID.TabIndex = 12;
            this.labelTeamViewerID.Text = "&TeamViewer ID:";
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(3, 332);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(48, 13);
            this.labelAddress.TabIndex = 14;
            this.labelAddress.Text = "A&ddress:";
            this.labelAddress.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tabPageLanguageInfo
            // 
            this.tabPageLanguageInfo.Controls.Add(this.tableLayoutPanelLanguageInformation);
            this.tabPageLanguageInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageLanguageInfo.Name = "tabPageLanguageInfo";
            this.tabPageLanguageInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanguageInfo.Size = new System.Drawing.Size(451, 418);
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
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelEnglishBT, 1, 6);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.buttonInternationalBTFont, 2, 6);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.labelProjectName, 0, 0);
            this.tableLayoutPanelLanguageInformation.Controls.Add(this.textBoxProjectName, 1, 0);
            this.tableLayoutPanelLanguageInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLanguageInformation.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLanguageInformation.Name = "tableLayoutPanelLanguageInformation";
            this.tableLayoutPanelLanguageInformation.RowCount = 8;
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLanguageInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLanguageInformation.Size = new System.Drawing.Size(445, 412);
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
            this.labelStoryLanguage.Location = new System.Drawing.Point(83, 65);
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
            this.labelNationalBTLanguage.Location = new System.Drawing.Point(265, 65);
            this.labelNationalBTLanguage.Name = "labelNationalBTLanguage";
            this.labelNationalBTLanguage.Size = new System.Drawing.Size(177, 21);
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
            this.labelEnglishBT.Location = new System.Drawing.Point(174, 263);
            this.labelEnglishBT.Name = "labelEnglishBT";
            this.labelEnglishBT.Size = new System.Drawing.Size(85, 13);
            this.labelEnglishBT.TabIndex = 11;
            this.labelEnglishBT.Text = "English BT Font:";
            // 
            // fontDialog
            // 
            this.fontDialog.ShowColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.Personal;
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
            this.textBoxProjectName.Location = new System.Drawing.Point(83, 3);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(359, 20);
            this.textBoxProjectName.TabIndex = 16;
            // 
            // TeamMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(459, 444);
            this.Controls.Add(this.tabControlProjectMetaData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TeamMemberForm";
            this.Text = "Project Settings";
            this.groupBoxRole.ResumeLayout(false);
            this.groupBoxRole.PerformLayout();
            this.tabControlProjectMetaData.ResumeLayout(false);
            this.tabPageMemberList.ResumeLayout(false);
            this.tableLayoutPanelTeamMembers.ResumeLayout(false);
            this.tableLayoutPanelTeamMembers.PerformLayout();
            this.tabPageEditMember.ResumeLayout(false);
            this.tableLayoutPanelMemberInformation.ResumeLayout(false);
            this.tableLayoutPanelMemberInformation.PerformLayout();
            this.tabPageLanguageInfo.ResumeLayout(false);
            this.tableLayoutPanelLanguageInformation.ResumeLayout(false);
            this.tableLayoutPanelLanguageInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabControl tabControlProjectMetaData;
        private System.Windows.Forms.TabPage tabPageMemberList;
        private System.Windows.Forms.TabPage tabPageEditMember;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMemberInformation;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.GroupBox groupBoxRole;
        private System.Windows.Forms.RadioButton radioButtonJustViewing;
        private System.Windows.Forms.RadioButton radioButtonCoach;
        private System.Windows.Forms.RadioButton radioButtonIndependentConsultant;
        private System.Windows.Forms.RadioButton radioButtonConsultantInTraining;
        private System.Windows.Forms.RadioButton radioButtonUNS;
        private System.Windows.Forms.RadioButton radioButtonStoryCrafter;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.TextBox textBoxEmail;
        private System.Windows.Forms.Label labelPhoneNumber;
        private System.Windows.Forms.TextBox textBoxPhoneNumber;
        private System.Windows.Forms.Label labelAltPhone;
        private System.Windows.Forms.TextBox textBoxAltPhone;
        private System.Windows.Forms.Label labelSkype;
        private System.Windows.Forms.TextBox textBoxSkypeID;
        private System.Windows.Forms.Label labelTeamViewerID;
        private System.Windows.Forms.TextBox textBoxTeamViewer;
        private System.Windows.Forms.Label labelAddress;
        private System.Windows.Forms.TextBox textBoxAddress;
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
    }
}