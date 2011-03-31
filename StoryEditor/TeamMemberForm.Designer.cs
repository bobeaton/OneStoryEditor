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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonMergeMember = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.buttonAddNewMember = new System.Windows.Forms.Button();
            this.buttonEditMember = new System.Windows.Forms.Button();
            this.buttonDeleteMember = new System.Windows.Forms.Button();
            this.listBoxTeamMembers = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelTeamMembers = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMemberNames = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelTeamMembers.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(320, 451);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.toolTip.SetToolTip(this.buttonCancel, "Click to cancel this dialog");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(239, 451);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "&Login";
            this.toolTip.SetToolTip(this.buttonOK, "Click to login as the selected member");
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonMergeMember
            // 
            this.buttonMergeMember.Location = new System.Drawing.Point(520, 110);
            this.buttonMergeMember.Name = "buttonMergeMember";
            this.buttonMergeMember.Size = new System.Drawing.Size(111, 30);
            this.buttonMergeMember.TabIndex = 3;
            this.buttonMergeMember.Text = "&Merge UNSs";
            this.toolTip.SetToolTip(this.buttonMergeMember, "Click this button to merge a different UNS with the selected UNS (so all referenc" +
                    "es will be to the selected UNS)");
            this.buttonMergeMember.UseVisualStyleBackColor = true;
            this.buttonMergeMember.Visible = false;
            this.buttonMergeMember.Click += new System.EventHandler(this.buttonMergeMember_Click);
            // 
            // fontDialog
            // 
            this.fontDialog.ShowColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.Personal;
            // 
            // buttonAddNewMember
            // 
            this.helpProvider.SetHelpString(this.buttonAddNewMember, "Click to Add a new team member");
            this.buttonAddNewMember.Location = new System.Drawing.Point(520, 38);
            this.buttonAddNewMember.Name = "buttonAddNewMember";
            this.helpProvider.SetShowHelp(this.buttonAddNewMember, true);
            this.buttonAddNewMember.Size = new System.Drawing.Size(111, 30);
            this.buttonAddNewMember.TabIndex = 1;
            this.buttonAddNewMember.Text = "&Add New Member";
            this.buttonAddNewMember.UseVisualStyleBackColor = true;
            this.buttonAddNewMember.Click += new System.EventHandler(this.buttonAddNewMember_Click);
            // 
            // buttonEditMember
            // 
            this.buttonEditMember.Enabled = false;
            this.helpProvider.SetHelpString(this.buttonEditMember, "Click to edit the selected member\'s profile");
            this.buttonEditMember.Location = new System.Drawing.Point(520, 74);
            this.buttonEditMember.Name = "buttonEditMember";
            this.helpProvider.SetShowHelp(this.buttonEditMember, true);
            this.buttonEditMember.Size = new System.Drawing.Size(111, 30);
            this.buttonEditMember.TabIndex = 2;
            this.buttonEditMember.Text = "&Edit Member";
            this.buttonEditMember.UseVisualStyleBackColor = true;
            this.buttonEditMember.Click += new System.EventHandler(this.buttonEditMember_Click);
            // 
            // buttonDeleteMember
            // 
            this.helpProvider.SetHelpString(this.buttonDeleteMember, "Click to delete the selected member (only works for members added this session)");
            this.buttonDeleteMember.Location = new System.Drawing.Point(520, 150);
            this.buttonDeleteMember.Name = "buttonDeleteMember";
            this.helpProvider.SetShowHelp(this.buttonDeleteMember, true);
            this.buttonDeleteMember.Size = new System.Drawing.Size(111, 30);
            this.buttonDeleteMember.TabIndex = 3;
            this.buttonDeleteMember.Text = "&Delete Member";
            this.buttonDeleteMember.UseVisualStyleBackColor = true;
            this.buttonDeleteMember.Visible = false;
            this.buttonDeleteMember.Click += new System.EventHandler(this.buttonDeleteMember_Click);
            // 
            // listBoxTeamMembers
            // 
            this.listBoxTeamMembers.ColumnWidth = 151;
            this.listBoxTeamMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTeamMembers.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.listBoxTeamMembers, resources.GetString("listBoxTeamMembers.HelpString"));
            this.listBoxTeamMembers.Location = new System.Drawing.Point(3, 38);
            this.listBoxTeamMembers.Name = "listBoxTeamMembers";
            this.tableLayoutPanelTeamMembers.SetRowSpan(this.listBoxTeamMembers, 4);
            this.helpProvider.SetShowHelp(this.listBoxTeamMembers, true);
            this.listBoxTeamMembers.Size = new System.Drawing.Size(511, 394);
            this.listBoxTeamMembers.Sorted = true;
            this.listBoxTeamMembers.TabIndex = 0;
            this.listBoxTeamMembers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxTeamMembers_MouseDoubleClick);
            this.listBoxTeamMembers.SelectedIndexChanged += new System.EventHandler(this.listBoxTeamMembers_SelectedIndexChanged);
            // 
            // tableLayoutPanelTeamMembers
            // 
            this.tableLayoutPanelTeamMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelTeamMembers.ColumnCount = 2;
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonAddNewMember, 2, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonEditMember, 2, 2);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonDeleteMember, 2, 4);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.listBoxTeamMembers, 0, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.textBoxMemberNames, 0, 0);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeMember, 1, 3);
            this.tableLayoutPanelTeamMembers.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTeamMembers.Name = "tableLayoutPanelTeamMembers";
            this.tableLayoutPanelTeamMembers.RowCount = 5;
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTeamMembers.Size = new System.Drawing.Size(634, 445);
            this.tableLayoutPanelTeamMembers.TabIndex = 1;
            // 
            // textBoxMemberNames
            // 
            this.textBoxMemberNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberNames.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberNames.Location = new System.Drawing.Point(3, 3);
            this.textBoxMemberNames.Name = "textBoxMemberNames";
            this.textBoxMemberNames.ReadOnly = true;
            this.textBoxMemberNames.Size = new System.Drawing.Size(511, 29);
            this.textBoxMemberNames.TabIndex = 8;
            this.textBoxMemberNames.Text = "Team Members";
            this.textBoxMemberNames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TeamMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(635, 485);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tableLayoutPanelTeamMembers);
            this.Controls.Add(this.buttonOK);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TeamMemberForm";
            this.Text = "Login";
            this.tableLayoutPanelTeamMembers.ResumeLayout(false);
            this.tableLayoutPanelTeamMembers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTeamMembers;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAddNewMember;
        private System.Windows.Forms.Button buttonEditMember;
        private System.Windows.Forms.Button buttonDeleteMember;
        private System.Windows.Forms.ListBox listBoxTeamMembers;
        private System.Windows.Forms.TextBox textBoxMemberNames;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonMergeMember;
    }
}