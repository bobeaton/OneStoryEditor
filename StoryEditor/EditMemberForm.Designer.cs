namespace OneStoryProjectEditor
{
    partial class EditMemberForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMemberForm));
            this.tableLayoutPanelMemberInformation = new System.Windows.Forms.TableLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBoxRole = new System.Windows.Forms.GroupBox();
            this.checkBoxLangSpecialtyReviewer = new System.Windows.Forms.CheckBox();
            this.checkBoxUns = new System.Windows.Forms.CheckBox();
            this.checkBoxCrafter = new System.Windows.Forms.CheckBox();
            this.checkBoxProjectFacilitator = new System.Windows.Forms.CheckBox();
            this.buttonSetDefaultTasks = new System.Windows.Forms.Button();
            this.radioButtonIndependentConsultant = new System.Windows.Forms.RadioButton();
            this.radioButtonEnglishBackTranslator = new System.Windows.Forms.RadioButton();
            this.radioButtonJustViewing = new System.Windows.Forms.RadioButton();
            this.radioButtonCoach = new System.Windows.Forms.RadioButton();
            this.radioButtonConsultantInTraining = new System.Windows.Forms.RadioButton();
            this.labelEmail = new System.Windows.Forms.Label();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.labelPhoneNumber = new System.Windows.Forms.Label();
            this.textBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.labelAltPhone = new System.Windows.Forms.Label();
            this.textBoxAltPhone = new System.Windows.Forms.TextBox();
            this.labelSkype = new System.Windows.Forms.Label();
            this.textBoxSkypeID = new System.Windows.Forms.TextBox();
            this.labelTeamViewerID = new System.Windows.Forms.Label();
            this.textBoxTeamViewer = new System.Windows.Forms.TextBox();
            this.labelBioData = new System.Windows.Forms.Label();
            this.textBoxBioData = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanelMemberInformation.SuspendLayout();
            this.groupBoxRole.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMemberInformation
            // 
            this.tableLayoutPanelMemberInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.tableLayoutPanelMemberInformation.Controls.Add(this.labelBioData, 0, 7);
            this.tableLayoutPanelMemberInformation.Controls.Add(this.textBoxBioData, 1, 7);
            this.tableLayoutPanelMemberInformation.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanelMemberInformation.Name = "tableLayoutPanelMemberInformation";
            this.tableLayoutPanelMemberInformation.RowCount = 8;
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMemberInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMemberInformation.Size = new System.Drawing.Size(620, 493);
            this.tableLayoutPanelMemberInformation.TabIndex = 2;
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
            // textBoxName
            // 
            this.textBoxName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxName, "The name of the member. Must be unique. If you are fulfilling multiple roles, you" +
        " can make a name like \"John as consultant-in-training\"");
            this.textBoxName.Location = new System.Drawing.Point(92, 3);
            this.textBoxName.Name = "textBoxName";
            this.helpProvider.SetShowHelp(this.textBoxName, true);
            this.textBoxName.Size = new System.Drawing.Size(525, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // groupBoxRole
            // 
            this.groupBoxRole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRole.Controls.Add(this.checkBoxLangSpecialtyReviewer);
            this.groupBoxRole.Controls.Add(this.checkBoxUns);
            this.groupBoxRole.Controls.Add(this.checkBoxCrafter);
            this.groupBoxRole.Controls.Add(this.checkBoxProjectFacilitator);
            this.groupBoxRole.Controls.Add(this.buttonSetDefaultTasks);
            this.groupBoxRole.Controls.Add(this.radioButtonIndependentConsultant);
            this.groupBoxRole.Controls.Add(this.radioButtonEnglishBackTranslator);
            this.groupBoxRole.Controls.Add(this.radioButtonJustViewing);
            this.groupBoxRole.Controls.Add(this.radioButtonCoach);
            this.groupBoxRole.Controls.Add(this.radioButtonConsultantInTraining);
            this.groupBoxRole.Location = new System.Drawing.Point(92, 29);
            this.groupBoxRole.Name = "groupBoxRole";
            this.groupBoxRole.Size = new System.Drawing.Size(525, 147);
            this.groupBoxRole.TabIndex = 2;
            this.groupBoxRole.TabStop = false;
            this.groupBoxRole.Text = "What role do you have in the team?";
            // 
            // checkBoxLangSpecialtyReviewer
            // 
            this.checkBoxLangSpecialtyReviewer.AutoSize = true;
            this.checkBoxLangSpecialtyReviewer.Location = new System.Drawing.Point(17, 88);
            this.checkBoxLangSpecialtyReviewer.Name = "checkBoxLangSpecialtyReviewer";
            this.checkBoxLangSpecialtyReviewer.Size = new System.Drawing.Size(168, 17);
            this.checkBoxLangSpecialtyReviewer.TabIndex = 13;
            this.checkBoxLangSpecialtyReviewer.Text = "&Language Specialty Reviewer";
            this.checkBoxLangSpecialtyReviewer.UseVisualStyleBackColor = true;
            this.checkBoxLangSpecialtyReviewer.CheckedChanged += new System.EventHandler(this.checkBoxRole_CheckedChanged);
            // 
            // checkBoxUns
            // 
            this.checkBoxUns.AutoSize = true;
            this.checkBoxUns.Location = new System.Drawing.Point(17, 65);
            this.checkBoxUns.Name = "checkBoxUns";
            this.checkBoxUns.Size = new System.Drawing.Size(127, 17);
            this.checkBoxUns.TabIndex = 12;
            this.checkBoxUns.Text = "&Testing Helper (UNS)";
            this.checkBoxUns.UseVisualStyleBackColor = true;
            this.checkBoxUns.CheckedChanged += new System.EventHandler(this.checkBoxRole_CheckedChanged);
            // 
            // checkBoxCrafter
            // 
            this.checkBoxCrafter.AutoSize = true;
            this.checkBoxCrafter.Location = new System.Drawing.Point(18, 42);
            this.checkBoxCrafter.Name = "checkBoxCrafter";
            this.checkBoxCrafter.Size = new System.Drawing.Size(84, 17);
            this.checkBoxCrafter.TabIndex = 11;
            this.checkBoxCrafter.Text = "&Story Crafter";
            this.checkBoxCrafter.UseVisualStyleBackColor = true;
            this.checkBoxCrafter.CheckedChanged += new System.EventHandler(this.checkBoxRole_CheckedChanged);
            // 
            // checkBoxProjectFacilitator
            // 
            this.checkBoxProjectFacilitator.AutoSize = true;
            this.checkBoxProjectFacilitator.Location = new System.Drawing.Point(18, 19);
            this.checkBoxProjectFacilitator.Name = "checkBoxProjectFacilitator";
            this.checkBoxProjectFacilitator.Size = new System.Drawing.Size(107, 17);
            this.checkBoxProjectFacilitator.TabIndex = 10;
            this.checkBoxProjectFacilitator.Text = "&Project Facilitator";
            this.checkBoxProjectFacilitator.UseVisualStyleBackColor = true;
            this.checkBoxProjectFacilitator.CheckedChanged += new System.EventHandler(this.checkBoxRole_CheckedChanged);
            // 
            // buttonSetDefaultTasks
            // 
            this.buttonSetDefaultTasks.Location = new System.Drawing.Point(37, 111);
            this.buttonSetDefaultTasks.Name = "buttonSetDefaultTasks";
            this.buttonSetDefaultTasks.Size = new System.Drawing.Size(107, 23);
            this.buttonSetDefaultTasks.TabIndex = 9;
            this.buttonSetDefaultTasks.Text = "Set &Default Tasks";
            this.buttonSetDefaultTasks.UseVisualStyleBackColor = true;
            this.buttonSetDefaultTasks.Visible = false;
            this.buttonSetDefaultTasks.Click += new System.EventHandler(this.buttonSetDefaultTasks_Click);
            // 
            // radioButtonIndependentConsultant
            // 
            this.radioButtonIndependentConsultant.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonIndependentConsultant, "The member who provides exegetical feedback to the Project Facilitator on their s" +
        "tories. An independent consultant works without a \"Coach\" (c.f. “Consultant-in-T" +
        "raining”, who is mentored by a coach.");
            this.radioButtonIndependentConsultant.Location = new System.Drawing.Point(251, 65);
            this.radioButtonIndependentConsultant.Name = "radioButtonIndependentConsultant";
            this.helpProvider.SetShowHelp(this.radioButtonIndependentConsultant, true);
            this.radioButtonIndependentConsultant.Size = new System.Drawing.Size(138, 17);
            this.radioButtonIndependentConsultant.TabIndex = 6;
            this.radioButtonIndependentConsultant.Text = "&Independent Consultant";
            this.radioButtonIndependentConsultant.UseVisualStyleBackColor = true;
            this.radioButtonIndependentConsultant.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonEnglishBackTranslator
            // 
            this.radioButtonEnglishBackTranslator.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonEnglishBackTranslator, resources.GetString("radioButtonEnglishBackTranslator.HelpString"));
            this.radioButtonEnglishBackTranslator.Location = new System.Drawing.Point(251, 19);
            this.radioButtonEnglishBackTranslator.Name = "radioButtonEnglishBackTranslator";
            this.helpProvider.SetShowHelp(this.radioButtonEnglishBackTranslator, true);
            this.radioButtonEnglishBackTranslator.Size = new System.Drawing.Size(171, 17);
            this.radioButtonEnglishBackTranslator.TabIndex = 3;
            this.radioButtonEnglishBackTranslator.Text = "&Outside English back-translator";
            this.radioButtonEnglishBackTranslator.UseVisualStyleBackColor = true;
            this.radioButtonEnglishBackTranslator.Click += new System.EventHandler(this.radioButtonEnglishBackTranslator_Click);
            // 
            // radioButtonJustViewing
            // 
            this.radioButtonJustViewing.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonJustViewing, "A \"just looking\" member can view the stories in the file, but may not make editin" +
        "g changes. This is useful if you want to browse the stories but not accidentally" +
        " change something.");
            this.radioButtonJustViewing.Location = new System.Drawing.Point(251, 111);
            this.radioButtonJustViewing.Name = "radioButtonJustViewing";
            this.helpProvider.SetShowHelp(this.radioButtonJustViewing, true);
            this.radioButtonJustViewing.Size = new System.Drawing.Size(85, 17);
            this.radioButtonJustViewing.TabIndex = 8;
            this.radioButtonJustViewing.Text = "&Just Looking";
            this.radioButtonJustViewing.UseVisualStyleBackColor = true;
            this.radioButtonJustViewing.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonCoach
            // 
            this.radioButtonCoach.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonCoach, "The member (usually only one/team) that coaches (mentors) a consultant-in-trainin" +
        "g.");
            this.radioButtonCoach.Location = new System.Drawing.Point(251, 88);
            this.radioButtonCoach.Name = "radioButtonCoach";
            this.helpProvider.SetShowHelp(this.radioButtonCoach, true);
            this.radioButtonCoach.Size = new System.Drawing.Size(56, 17);
            this.radioButtonCoach.TabIndex = 7;
            this.radioButtonCoach.Text = "Coac&h";
            this.radioButtonCoach.UseVisualStyleBackColor = true;
            this.radioButtonCoach.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonConsultantInTraining
            // 
            this.radioButtonConsultantInTraining.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonConsultantInTraining, resources.GetString("radioButtonConsultantInTraining.HelpString"));
            this.radioButtonConsultantInTraining.Location = new System.Drawing.Point(251, 42);
            this.radioButtonConsultantInTraining.Name = "radioButtonConsultantInTraining";
            this.helpProvider.SetShowHelp(this.radioButtonConsultantInTraining, true);
            this.radioButtonConsultantInTraining.Size = new System.Drawing.Size(127, 17);
            this.radioButtonConsultantInTraining.TabIndex = 5;
            this.radioButtonConsultantInTraining.Text = "&Consultant in Training";
            this.radioButtonConsultantInTraining.UseVisualStyleBackColor = true;
            this.radioButtonConsultantInTraining.CheckedChanged += new System.EventHandler(this.radioButtonConsultantInTraining_CheckedChanged);
            // 
            // labelEmail
            // 
            this.labelEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(3, 185);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(35, 13);
            this.labelEmail.TabIndex = 4;
            this.labelEmail.Text = "&Email:";
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxEmail, "Email address by which you can be contacted.");
            this.textBoxEmail.Location = new System.Drawing.Point(92, 182);
            this.textBoxEmail.Name = "textBoxEmail";
            this.helpProvider.SetShowHelp(this.textBoxEmail, true);
            this.textBoxEmail.Size = new System.Drawing.Size(525, 20);
            this.textBoxEmail.TabIndex = 5;
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(3, 211);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(41, 13);
            this.labelPhoneNumber.TabIndex = 6;
            this.labelPhoneNumber.Text = "&Phone:";
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxPhoneNumber, "Phone number by which you can be contacted.");
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(92, 208);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.helpProvider.SetShowHelp(this.textBoxPhoneNumber, true);
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(525, 20);
            this.textBoxPhoneNumber.TabIndex = 7;
            // 
            // labelAltPhone
            // 
            this.labelAltPhone.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAltPhone.AutoSize = true;
            this.labelAltPhone.Location = new System.Drawing.Point(3, 237);
            this.labelAltPhone.Name = "labelAltPhone";
            this.labelAltPhone.Size = new System.Drawing.Size(56, 13);
            this.labelAltPhone.TabIndex = 8;
            this.labelAltPhone.Text = "Al&t Phone:";
            // 
            // textBoxAltPhone
            // 
            this.textBoxAltPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxAltPhone, "Alternate phone number by which you can be contacted.");
            this.textBoxAltPhone.Location = new System.Drawing.Point(92, 234);
            this.textBoxAltPhone.Name = "textBoxAltPhone";
            this.helpProvider.SetShowHelp(this.textBoxAltPhone, true);
            this.textBoxAltPhone.Size = new System.Drawing.Size(525, 20);
            this.textBoxAltPhone.TabIndex = 9;
            // 
            // labelSkype
            // 
            this.labelSkype.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSkype.AutoSize = true;
            this.labelSkype.Location = new System.Drawing.Point(3, 263);
            this.labelSkype.Name = "labelSkype";
            this.labelSkype.Size = new System.Drawing.Size(54, 13);
            this.labelSkype.TabIndex = 10;
            this.labelSkype.Text = "S&kype ID:";
            // 
            // textBoxSkypeID
            // 
            this.textBoxSkypeID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxSkypeID, "Skype ID by which you can be contacted.");
            this.textBoxSkypeID.Location = new System.Drawing.Point(92, 260);
            this.textBoxSkypeID.Name = "textBoxSkypeID";
            this.helpProvider.SetShowHelp(this.textBoxSkypeID, true);
            this.textBoxSkypeID.Size = new System.Drawing.Size(525, 20);
            this.textBoxSkypeID.TabIndex = 11;
            // 
            // labelTeamViewerID
            // 
            this.labelTeamViewerID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTeamViewerID.AutoSize = true;
            this.labelTeamViewerID.Location = new System.Drawing.Point(3, 289);
            this.labelTeamViewerID.Name = "labelTeamViewerID";
            this.labelTeamViewerID.Size = new System.Drawing.Size(83, 13);
            this.labelTeamViewerID.TabIndex = 12;
            this.labelTeamViewerID.Text = "&TeamViewer ID:";
            // 
            // textBoxTeamViewer
            // 
            this.textBoxTeamViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxTeamViewer, "Team Viewer ID by which you can be contacted.");
            this.textBoxTeamViewer.Location = new System.Drawing.Point(92, 286);
            this.textBoxTeamViewer.Name = "textBoxTeamViewer";
            this.helpProvider.SetShowHelp(this.textBoxTeamViewer, true);
            this.textBoxTeamViewer.Size = new System.Drawing.Size(525, 20);
            this.textBoxTeamViewer.TabIndex = 13;
            // 
            // labelBioData
            // 
            this.labelBioData.AutoSize = true;
            this.labelBioData.Location = new System.Drawing.Point(3, 309);
            this.labelBioData.Name = "labelBioData";
            this.labelBioData.Size = new System.Drawing.Size(25, 13);
            this.labelBioData.TabIndex = 14;
            this.labelBioData.Text = "&Bio:";
            this.labelBioData.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxBioData
            // 
            this.textBoxBioData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxBioData, "Biographical Data about the member (e.g. age, back-ground, education level, quali" +
        "fications, address, etc)");
            this.textBoxBioData.Location = new System.Drawing.Point(92, 312);
            this.textBoxBioData.Multiline = true;
            this.textBoxBioData.Name = "textBoxBioData";
            this.helpProvider.SetShowHelp(this.textBoxBioData, true);
            this.textBoxBioData.Size = new System.Drawing.Size(525, 178);
            this.textBoxBioData.TabIndex = 15;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(244, 513);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 513);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // EditMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(644, 548);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tableLayoutPanelMemberInformation);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditMemberForm";
            this.Text = "Edit Member Information";
            this.tableLayoutPanelMemberInformation.ResumeLayout(false);
            this.tableLayoutPanelMemberInformation.PerformLayout();
            this.groupBoxRole.ResumeLayout(false);
            this.groupBoxRole.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMemberInformation;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.GroupBox groupBoxRole;
        private System.Windows.Forms.RadioButton radioButtonJustViewing;
        private System.Windows.Forms.RadioButton radioButtonCoach;
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
        private System.Windows.Forms.Label labelBioData;
        private System.Windows.Forms.TextBox textBoxBioData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonEnglishBackTranslator;
        private System.Windows.Forms.RadioButton radioButtonConsultantInTraining;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.RadioButton radioButtonIndependentConsultant;
        private System.Windows.Forms.Button buttonSetDefaultTasks;
        private System.Windows.Forms.CheckBox checkBoxProjectFacilitator;
        private System.Windows.Forms.CheckBox checkBoxCrafter;
        private System.Windows.Forms.CheckBox checkBoxUns;
        private System.Windows.Forms.CheckBox checkBoxLangSpecialtyReviewer;
    }
}