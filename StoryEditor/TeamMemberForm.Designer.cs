using System.Windows.Forms;

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
            this.buttonMergeProjectFacilitators = new System.Windows.Forms.Button();
            this.buttonMergeCrafter = new System.Windows.Forms.Button();
            this.buttonMergeConsultant = new System.Windows.Forms.Button();
            this.buttonMergeCoach = new System.Windows.Forms.Button();
            this.buttonMergeUns = new System.Windows.Forms.Button();
            this.buttonDeleteMember = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.buttonAddNewMember = new System.Windows.Forms.Button();
            this.buttonEditMember = new System.Windows.Forms.Button();
            this.listBoxTeamMembersEditors = new System.Windows.Forms.ListBox();
            this.listBoxTeamMembersCollaborators = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelTeamMembers = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMemberNames = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEditors = new System.Windows.Forms.TabPage();
            this.tabCollaborators = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelTeamMembers.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabEditors.SuspendLayout();
            this.tabCollaborators.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(358, 451);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "&Cancel";
            this.toolTip.SetToolTip(this.buttonCancel, "Click to cancel this dialog");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(277, 451);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&Login";
            this.toolTip.SetToolTip(this.buttonOK, "Click to login as the selected member");
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonMergeProjectFacilitators
            // 
            this.buttonMergeProjectFacilitators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMergeProjectFacilitators.Location = new System.Drawing.Point(517, 182);
            this.buttonMergeProjectFacilitators.Name = "buttonMergeProjectFacilitators";
            this.buttonMergeProjectFacilitators.Size = new System.Drawing.Size(190, 30);
            this.buttonMergeProjectFacilitators.TabIndex = 6;
            this.buttonMergeProjectFacilitators.Text = "&Merge Project Facilitators";
            this.toolTip.SetToolTip(this.buttonMergeProjectFacilitators, "Click this button to merge a different Project Facilitator with the selected Proj" +
        "ect Facilitator (so all references will be to the selected Project Facilitator)");
            this.buttonMergeProjectFacilitators.UseVisualStyleBackColor = true;
            this.buttonMergeProjectFacilitators.Visible = false;
            this.buttonMergeProjectFacilitators.Click += new System.EventHandler(this.buttonMergeProjectFacilitators_Click);
            // 
            // buttonMergeCrafter
            // 
            this.buttonMergeCrafter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMergeCrafter.Location = new System.Drawing.Point(517, 218);
            this.buttonMergeCrafter.Name = "buttonMergeCrafter";
            this.buttonMergeCrafter.Size = new System.Drawing.Size(190, 30);
            this.buttonMergeCrafter.TabIndex = 7;
            this.buttonMergeCrafter.Text = "&Merge Crafters";
            this.toolTip.SetToolTip(this.buttonMergeCrafter, "Click this button to merge a different Crafter with the selected Crafter (so all " +
        "references will be to the selected Crafter)");
            this.buttonMergeCrafter.UseVisualStyleBackColor = true;
            this.buttonMergeCrafter.Visible = false;
            this.buttonMergeCrafter.Click += new System.EventHandler(this.buttonMergeCrafter_Click);
            // 
            // buttonMergeConsultant
            // 
            this.buttonMergeConsultant.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMergeConsultant.Location = new System.Drawing.Point(517, 146);
            this.buttonMergeConsultant.Name = "buttonMergeConsultant";
            this.buttonMergeConsultant.Size = new System.Drawing.Size(190, 30);
            this.buttonMergeConsultant.TabIndex = 5;
            this.buttonMergeConsultant.Text = "&Merge Consultant/CIT";
            this.toolTip.SetToolTip(this.buttonMergeConsultant, "Click this button to merge a different Consultant or CIT with the selected Consul" +
        "tant/CIT (so all references will be to the selected Consultant/CIT)");
            this.buttonMergeConsultant.UseVisualStyleBackColor = true;
            this.buttonMergeConsultant.Visible = false;
            this.buttonMergeConsultant.Click += new System.EventHandler(this.buttonMergeConsultant_Click);
            // 
            // buttonMergeCoach
            // 
            this.buttonMergeCoach.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMergeCoach.Location = new System.Drawing.Point(517, 110);
            this.buttonMergeCoach.Name = "buttonMergeCoach";
            this.buttonMergeCoach.Size = new System.Drawing.Size(190, 30);
            this.buttonMergeCoach.TabIndex = 4;
            this.buttonMergeCoach.Text = "&Merge Coaches";
            this.toolTip.SetToolTip(this.buttonMergeCoach, "Click this button to merge a different Coach with the selected Coach (so all refe" +
        "rences will be to the selected Coach)");
            this.buttonMergeCoach.UseVisualStyleBackColor = true;
            this.buttonMergeCoach.Visible = false;
            this.buttonMergeCoach.Click += new System.EventHandler(this.buttonMergeCoach_Click);
            // 
            // buttonMergeUns
            // 
            this.buttonMergeUns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMergeUns.Location = new System.Drawing.Point(517, 254);
            this.buttonMergeUns.Name = "buttonMergeUns";
            this.buttonMergeUns.Size = new System.Drawing.Size(190, 30);
            this.buttonMergeUns.TabIndex = 8;
            this.buttonMergeUns.Text = "&Merge UNSs";
            this.toolTip.SetToolTip(this.buttonMergeUns, "Click this button to merge a different UNS with the selected UNS (so all referenc" +
        "es will be to the selected UNS)");
            this.buttonMergeUns.UseVisualStyleBackColor = true;
            this.buttonMergeUns.Visible = false;
            this.buttonMergeUns.Click += new System.EventHandler(this.buttonMergeUns_Click);
            // 
            // buttonDeleteMember
            // 
            this.helpProvider.SetHelpString(this.buttonDeleteMember, "Click to delete the selected member (only works for members added this session)");
            this.buttonDeleteMember.Location = new System.Drawing.Point(517, 290);
            this.buttonDeleteMember.Name = "buttonDeleteMember";
            this.helpProvider.SetShowHelp(this.buttonDeleteMember, true);
            this.buttonDeleteMember.Size = new System.Drawing.Size(190, 30);
            this.buttonDeleteMember.TabIndex = 3;
            this.buttonDeleteMember.Text = "&Delete Member";
            this.toolTip.SetToolTip(this.buttonDeleteMember, "The selected member isn\'t associated with any stories, so you can delete it if yo" +
        "u want.");
            this.buttonDeleteMember.UseVisualStyleBackColor = true;
            this.buttonDeleteMember.Visible = false;
            this.buttonDeleteMember.Click += new System.EventHandler(this.buttonDeleteMember_Click);
            // 
            // fontDialog
            // 
            this.fontDialog.ShowColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // buttonAddNewMember
            // 
            this.buttonAddNewMember.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonAddNewMember, "Click to Add a new team member");
            this.buttonAddNewMember.Location = new System.Drawing.Point(517, 38);
            this.buttonAddNewMember.Name = "buttonAddNewMember";
            this.helpProvider.SetShowHelp(this.buttonAddNewMember, true);
            this.buttonAddNewMember.Size = new System.Drawing.Size(190, 30);
            this.buttonAddNewMember.TabIndex = 2;
            this.buttonAddNewMember.Text = "&Add New Member";
            this.buttonAddNewMember.UseVisualStyleBackColor = true;
            this.buttonAddNewMember.Click += new System.EventHandler(this.buttonAddNewMember_Click);
            // 
            // buttonEditMember
            // 
            this.buttonEditMember.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEditMember.Enabled = false;
            this.helpProvider.SetHelpString(this.buttonEditMember, "Click to edit the selected member\'s profile");
            this.buttonEditMember.Location = new System.Drawing.Point(517, 74);
            this.buttonEditMember.Name = "buttonEditMember";
            this.helpProvider.SetShowHelp(this.buttonEditMember, true);
            this.buttonEditMember.Size = new System.Drawing.Size(190, 30);
            this.buttonEditMember.TabIndex = 3;
            this.buttonEditMember.Text = "&Edit Member";
            this.buttonEditMember.UseVisualStyleBackColor = true;
            this.buttonEditMember.Click += new System.EventHandler(this.buttonEditMember_Click);
            // 
            // listBoxTeamMembersEditors
            // 
            this.listBoxTeamMembersEditors.ColumnWidth = 151;
            this.listBoxTeamMembersEditors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTeamMembersEditors.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.listBoxTeamMembersEditors, resources.GetString("listBoxTeamMembersEditors.HelpString"));
            this.listBoxTeamMembersEditors.Location = new System.Drawing.Point(3, 3);
            this.listBoxTeamMembersEditors.Name = "listBoxTeamMembersEditors";
            this.helpProvider.SetShowHelp(this.listBoxTeamMembersEditors, true);
            this.listBoxTeamMembersEditors.Size = new System.Drawing.Size(494, 372);
            this.listBoxTeamMembersEditors.Sorted = true;
            this.listBoxTeamMembersEditors.TabIndex = 1;
            this.listBoxTeamMembersEditors.SelectedIndexChanged += new System.EventHandler(this.listBoxTeamMembersEditors_SelectedIndexChanged);
            this.listBoxTeamMembersEditors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxTeamMembersEditors_MouseDoubleClick);
            // 
            // listBoxTeamMembersCollaborators
            // 
            this.listBoxTeamMembersCollaborators.ColumnWidth = 151;
            this.listBoxTeamMembersCollaborators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTeamMembersCollaborators.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.listBoxTeamMembersCollaborators, resources.GetString("listBoxTeamMembersCollaborators.HelpString"));
            this.listBoxTeamMembersCollaborators.Location = new System.Drawing.Point(3, 3);
            this.listBoxTeamMembersCollaborators.Name = "listBoxTeamMembersCollaborators";
            this.helpProvider.SetShowHelp(this.listBoxTeamMembersCollaborators, true);
            this.listBoxTeamMembersCollaborators.Size = new System.Drawing.Size(494, 372);
            this.listBoxTeamMembersCollaborators.Sorted = true;
            this.listBoxTeamMembersCollaborators.TabIndex = 2;
            this.listBoxTeamMembersCollaborators.SelectedIndexChanged += new System.EventHandler(this.listBoxTeamMembersCollaborators_SelectedIndexChanged);
            this.listBoxTeamMembersCollaborators.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxTeamMembersCollaborators_MouseDoubleClick);
            // 
            // tableLayoutPanelTeamMembers
            // 
            this.tableLayoutPanelTeamMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelTeamMembers.ColumnCount = 2;
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTeamMembers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTeamMembers.Controls.Add(this.textBoxMemberNames, 0, 0);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonAddNewMember, 1, 1);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonDeleteMember, 1, 8);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonEditMember, 1, 2);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeCoach, 1, 3);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeProjectFacilitators, 1, 5);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeCrafter, 1, 6);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeConsultant, 1, 4);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.buttonMergeUns, 1, 7);
            this.tableLayoutPanelTeamMembers.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanelTeamMembers.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTeamMembers.Name = "tableLayoutPanelTeamMembers";
            this.tableLayoutPanelTeamMembers.RowCount = 9;
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTeamMembers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTeamMembers.Size = new System.Drawing.Size(710, 445);
            this.tableLayoutPanelTeamMembers.TabIndex = 1;
            // 
            // textBoxMemberNames
            // 
            this.textBoxMemberNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMemberNames.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMemberNames.Location = new System.Drawing.Point(3, 3);
            this.textBoxMemberNames.Name = "textBoxMemberNames";
            this.textBoxMemberNames.ReadOnly = true;
            this.textBoxMemberNames.Size = new System.Drawing.Size(508, 29);
            this.textBoxMemberNames.TabIndex = 0;
            this.textBoxMemberNames.Text = "Team Members";
            this.textBoxMemberNames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabEditors);
            this.tabControl1.Controls.Add(this.tabCollaborators);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 38);
            this.tabControl1.Name = "tabControl1";
            this.tableLayoutPanelTeamMembers.SetRowSpan(this.tabControl1, 9);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(508, 404);
            this.tabControl1.TabIndex = 9;
            // 
            // tabEditors
            // 
            this.tabEditors.Controls.Add(this.listBoxTeamMembersEditors);
            this.tabEditors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabEditors.Location = new System.Drawing.Point(4, 22);
            this.tabEditors.Name = "tabEditors";
            this.tabEditors.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditors.Size = new System.Drawing.Size(500, 378);
            this.tabEditors.TabIndex = 1;
            this.tabEditors.Text = "Editors";
            this.tabEditors.UseVisualStyleBackColor = true;
            // 
            // tabCollaborators
            // 
            this.tabCollaborators.Controls.Add(this.listBoxTeamMembersCollaborators);
            this.tabCollaborators.Location = new System.Drawing.Point(4, 22);
            this.tabCollaborators.Name = "tabCollaborators";
            this.tabCollaborators.Padding = new System.Windows.Forms.Padding(3);
            this.tabCollaborators.Size = new System.Drawing.Size(500, 378);
            this.tabCollaborators.TabIndex = 1;
            this.tabCollaborators.Text = "Collaborators";
            this.tabCollaborators.UseVisualStyleBackColor = true;
            // 
            // TeamMemberForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(711, 485);
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
            this.tabControl1.ResumeLayout(false);
            this.tabEditors.ResumeLayout(false);
            this.tabCollaborators.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void TabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
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
        private System.Windows.Forms.ListBox listBoxTeamMembersEditors;
        private System.Windows.Forms.TextBox textBoxMemberNames;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonMergeProjectFacilitators;
        private System.Windows.Forms.Button buttonMergeCrafter;
        private System.Windows.Forms.Button buttonMergeCoach;
        private System.Windows.Forms.Button buttonMergeConsultant;
        private Button buttonMergeUns;
        private TabControl tabControl1;
        private TabPage tabEditors;
        private TabPage tabCollaborators;
        private ListBox listBoxTeamMembersCollaborators;
    }
}