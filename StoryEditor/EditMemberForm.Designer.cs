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
            this.radioButtonEnglishBackTranslator = new System.Windows.Forms.RadioButton();
            this.radioButtonJustViewing = new System.Windows.Forms.RadioButton();
            this.radioButtonCoach = new System.Windows.Forms.RadioButton();
            this.radioButtonProjectFacilitator = new System.Windows.Forms.RadioButton();
            this.radioButtonFirstPassMentor = new System.Windows.Forms.RadioButton();
            this.radioButtonUNS = new System.Windows.Forms.RadioButton();
            this.radioButtonStoryCrafter = new System.Windows.Forms.RadioButton();
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
            this.radioButtonConsultantInTraining = new System.Windows.Forms.RadioButton();
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
            this.tableLayoutPanelMemberInformation.Size = new System.Drawing.Size(447, 450);
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
            this.textBoxName.Location = new System.Drawing.Point(92, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(352, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // groupBoxRole
            // 
            this.groupBoxRole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRole.Controls.Add(this.radioButtonEnglishBackTranslator);
            this.groupBoxRole.Controls.Add(this.radioButtonJustViewing);
            this.groupBoxRole.Controls.Add(this.radioButtonCoach);
            this.groupBoxRole.Controls.Add(this.radioButtonProjectFacilitator);
            this.groupBoxRole.Controls.Add(this.radioButtonConsultantInTraining);
            this.groupBoxRole.Controls.Add(this.radioButtonFirstPassMentor);
            this.groupBoxRole.Controls.Add(this.radioButtonUNS);
            this.groupBoxRole.Controls.Add(this.radioButtonStoryCrafter);
            this.groupBoxRole.Location = new System.Drawing.Point(92, 29);
            this.groupBoxRole.Name = "groupBoxRole";
            this.groupBoxRole.Size = new System.Drawing.Size(352, 122);
            this.groupBoxRole.TabIndex = 3;
            this.groupBoxRole.TabStop = false;
            this.groupBoxRole.Text = "What role do you have in the team?";
            // 
            // radioButtonEnglishBackTranslator
            // 
            this.radioButtonEnglishBackTranslator.AutoSize = true;
            this.radioButtonEnglishBackTranslator.Location = new System.Drawing.Point(30, 66);
            this.radioButtonEnglishBackTranslator.Name = "radioButtonEnglishBackTranslator";
            this.radioButtonEnglishBackTranslator.Size = new System.Drawing.Size(132, 17);
            this.radioButtonEnglishBackTranslator.TabIndex = 2;
            this.radioButtonEnglishBackTranslator.TabStop = true;
            this.radioButtonEnglishBackTranslator.Text = "&English back-translator";
            this.radioButtonEnglishBackTranslator.UseVisualStyleBackColor = true;
            // 
            // radioButtonJustViewing
            // 
            this.radioButtonJustViewing.AutoSize = true;
            this.radioButtonJustViewing.Location = new System.Drawing.Point(195, 88);
            this.radioButtonJustViewing.Name = "radioButtonJustViewing";
            this.radioButtonJustViewing.Size = new System.Drawing.Size(85, 17);
            this.radioButtonJustViewing.TabIndex = 7;
            this.radioButtonJustViewing.TabStop = true;
            this.radioButtonJustViewing.Text = "&Just Looking";
            this.radioButtonJustViewing.UseVisualStyleBackColor = true;
            // 
            // radioButtonCoach
            // 
            this.radioButtonCoach.AutoSize = true;
            this.radioButtonCoach.Location = new System.Drawing.Point(195, 65);
            this.radioButtonCoach.Name = "radioButtonCoach";
            this.radioButtonCoach.Size = new System.Drawing.Size(56, 17);
            this.radioButtonCoach.TabIndex = 6;
            this.radioButtonCoach.TabStop = true;
            this.radioButtonCoach.Text = "Coac&h";
            this.radioButtonCoach.UseVisualStyleBackColor = true;
            // 
            // radioButtonProjectFacilitator
            // 
            this.radioButtonProjectFacilitator.AutoSize = true;
            this.radioButtonProjectFacilitator.Location = new System.Drawing.Point(30, 89);
            this.radioButtonProjectFacilitator.Name = "radioButtonProjectFacilitator";
            this.radioButtonProjectFacilitator.Size = new System.Drawing.Size(106, 17);
            this.radioButtonProjectFacilitator.TabIndex = 4;
            this.radioButtonProjectFacilitator.TabStop = true;
            this.radioButtonProjectFacilitator.Text = "&Project Facilitator";
            this.radioButtonProjectFacilitator.UseVisualStyleBackColor = true;
            // 
            // radioButtonFirstPassMentor
            // 
            this.radioButtonFirstPassMentor.AutoSize = true;
            this.radioButtonFirstPassMentor.Location = new System.Drawing.Point(195, 19);
            this.radioButtonFirstPassMentor.Name = "radioButtonFirstPassMentor";
            this.radioButtonFirstPassMentor.Size = new System.Drawing.Size(106, 17);
            this.radioButtonFirstPassMentor.TabIndex = 5;
            this.radioButtonFirstPassMentor.TabStop = true;
            this.radioButtonFirstPassMentor.Text = "&First Pass Mentor";
            this.radioButtonFirstPassMentor.UseVisualStyleBackColor = true;
            // 
            // radioButtonUNS
            // 
            this.radioButtonUNS.AutoSize = true;
            this.radioButtonUNS.Location = new System.Drawing.Point(30, 43);
            this.radioButtonUNS.Name = "radioButtonUNS";
            this.radioButtonUNS.Size = new System.Drawing.Size(126, 17);
            this.radioButtonUNS.TabIndex = 3;
            this.radioButtonUNS.Text = "&Testing Helper (UNS)";
            this.radioButtonUNS.UseVisualStyleBackColor = true;
            // 
            // radioButtonStoryCrafter
            // 
            this.radioButtonStoryCrafter.AutoSize = true;
            this.radioButtonStoryCrafter.Checked = true;
            this.radioButtonStoryCrafter.Location = new System.Drawing.Point(30, 19);
            this.radioButtonStoryCrafter.Name = "radioButtonStoryCrafter";
            this.radioButtonStoryCrafter.Size = new System.Drawing.Size(83, 17);
            this.radioButtonStoryCrafter.TabIndex = 1;
            this.radioButtonStoryCrafter.TabStop = true;
            this.radioButtonStoryCrafter.Text = "&Story Crafter";
            this.radioButtonStoryCrafter.UseVisualStyleBackColor = true;
            // 
            // labelEmail
            // 
            this.labelEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(3, 160);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(35, 13);
            this.labelEmail.TabIndex = 4;
            this.labelEmail.Text = "&Email:";
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEmail.Location = new System.Drawing.Point(92, 157);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(352, 20);
            this.textBoxEmail.TabIndex = 8;
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(3, 186);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(41, 13);
            this.labelPhoneNumber.TabIndex = 6;
            this.labelPhoneNumber.Text = "&Phone:";
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(92, 183);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(352, 20);
            this.textBoxPhoneNumber.TabIndex = 9;
            // 
            // labelAltPhone
            // 
            this.labelAltPhone.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAltPhone.AutoSize = true;
            this.labelAltPhone.Location = new System.Drawing.Point(3, 212);
            this.labelAltPhone.Name = "labelAltPhone";
            this.labelAltPhone.Size = new System.Drawing.Size(56, 13);
            this.labelAltPhone.TabIndex = 8;
            this.labelAltPhone.Text = "Al&t Phone:";
            // 
            // textBoxAltPhone
            // 
            this.textBoxAltPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAltPhone.Location = new System.Drawing.Point(92, 209);
            this.textBoxAltPhone.Name = "textBoxAltPhone";
            this.textBoxAltPhone.Size = new System.Drawing.Size(352, 20);
            this.textBoxAltPhone.TabIndex = 10;
            // 
            // labelSkype
            // 
            this.labelSkype.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSkype.AutoSize = true;
            this.labelSkype.Location = new System.Drawing.Point(3, 238);
            this.labelSkype.Name = "labelSkype";
            this.labelSkype.Size = new System.Drawing.Size(54, 13);
            this.labelSkype.TabIndex = 10;
            this.labelSkype.Text = "S&kype ID:";
            // 
            // textBoxSkypeID
            // 
            this.textBoxSkypeID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSkypeID.Location = new System.Drawing.Point(92, 235);
            this.textBoxSkypeID.Name = "textBoxSkypeID";
            this.textBoxSkypeID.Size = new System.Drawing.Size(352, 20);
            this.textBoxSkypeID.TabIndex = 11;
            // 
            // labelTeamViewerID
            // 
            this.labelTeamViewerID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTeamViewerID.AutoSize = true;
            this.labelTeamViewerID.Location = new System.Drawing.Point(3, 264);
            this.labelTeamViewerID.Name = "labelTeamViewerID";
            this.labelTeamViewerID.Size = new System.Drawing.Size(83, 13);
            this.labelTeamViewerID.TabIndex = 12;
            this.labelTeamViewerID.Text = "&TeamViewer ID:";
            // 
            // textBoxTeamViewer
            // 
            this.textBoxTeamViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTeamViewer.Location = new System.Drawing.Point(92, 261);
            this.textBoxTeamViewer.Name = "textBoxTeamViewer";
            this.textBoxTeamViewer.Size = new System.Drawing.Size(352, 20);
            this.textBoxTeamViewer.TabIndex = 12;
            // 
            // labelBioData
            // 
            this.labelBioData.AutoSize = true;
            this.labelBioData.Location = new System.Drawing.Point(3, 284);
            this.labelBioData.Name = "labelBioData";
            this.labelBioData.Size = new System.Drawing.Size(25, 13);
            this.labelBioData.TabIndex = 14;
            this.labelBioData.Text = "&Bio:";
            this.labelBioData.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxBioData
            // 
            this.textBoxBioData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBioData.Location = new System.Drawing.Point(92, 287);
            this.textBoxBioData.Multiline = true;
            this.textBoxBioData.Name = "textBoxBioData";
            this.textBoxBioData.Size = new System.Drawing.Size(352, 160);
            this.textBoxBioData.TabIndex = 13;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(157, 470);
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
            this.buttonCancel.Location = new System.Drawing.Point(239, 470);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // radioButtonConsultantInTraining
            // 
            this.radioButtonConsultantInTraining.AutoSize = true;
            this.radioButtonConsultantInTraining.Location = new System.Drawing.Point(195, 42);
            this.radioButtonConsultantInTraining.Name = "radioButtonConsultantInTraining";
            this.radioButtonConsultantInTraining.Size = new System.Drawing.Size(127, 17);
            this.radioButtonConsultantInTraining.TabIndex = 5;
            this.radioButtonConsultantInTraining.TabStop = true;
            this.radioButtonConsultantInTraining.Text = "&Consultant in Training";
            this.radioButtonConsultantInTraining.UseVisualStyleBackColor = true;
            // 
            // EditMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(471, 505);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tableLayoutPanelMemberInformation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.RadioButton radioButtonFirstPassMentor;
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
        private System.Windows.Forms.Label labelBioData;
        private System.Windows.Forms.TextBox textBoxBioData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonEnglishBackTranslator;
        private System.Windows.Forms.RadioButton radioButtonProjectFacilitator;
        private System.Windows.Forms.RadioButton radioButtonConsultantInTraining;
    }
}