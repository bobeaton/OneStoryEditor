namespace OneStoryProjectEditor
{
    partial class SayMoreImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SayMoreImportForm));
            this.dataGridViewEvents = new System.Windows.Forms.DataGridView();
            this.ColumnImport = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnSessions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnParticipant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControlImport = new System.Windows.Forms.TabControl();
            this.tabPageProjects = new System.Windows.Forms.TabPage();
            this.listBoxProjects = new System.Windows.Forms.ListBox();
            this.tabPageEvents = new System.Windows.Forms.TabPage();
            this.tabPageFieldMatching = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonAsAnswers = new System.Windows.Forms.RadioButton();
            this.radioButtonAsRetelling = new System.Windows.Forms.RadioButton();
            this.radioButtonNewStory = new System.Windows.Forms.RadioButton();
            this.buttonImport = new System.Windows.Forms.Button();
            this.dataGridViewTiers = new System.Windows.Forms.DataGridView();
            this.ColumnTierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTierLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTierSample = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTierField = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTiers)).BeginInit();
            this.tabControlImport.SuspendLayout();
            this.tabPageProjects.SuspendLayout();
            this.tabPageEvents.SuspendLayout();
            this.tabPageFieldMatching.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewEvents
            // 
            this.dataGridViewEvents.AllowUserToAddRows = false;
            this.dataGridViewEvents.AllowUserToDeleteRows = false;
            this.dataGridViewEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEvents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnImport,
            this.ColumnSessions,
            this.ColumnTitle,
            this.ColumnDate,
            this.ColumnParticipant});
            this.dataGridViewEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewEvents.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewEvents.Name = "dataGridViewEvents";
            this.dataGridViewEvents.RowTemplate.Height = 24;
            this.dataGridViewEvents.Size = new System.Drawing.Size(695, 237);
            this.dataGridViewEvents.TabIndex = 0;
            this.dataGridViewEvents.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewEventsCellContentClick);
            // 
            // ColumnImport
            // 
            this.ColumnImport.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnImport.HeaderText = "";
            this.ColumnImport.MinimumWidth = 100;
            this.ColumnImport.Name = "ColumnImport";
            this.ColumnImport.Text = "";
            this.ColumnImport.ToolTipText = "Click on one of the buttons below to import that story";
            // 
            // ColumnSessions
            // 
            this.ColumnSessions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnSessions.HeaderText = "Sessions";
            this.ColumnSessions.Name = "ColumnSessions";
            this.ColumnSessions.ReadOnly = true;
            this.ColumnSessions.Width = 74;
            // 
            // ColumnTitle
            // 
            this.ColumnTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnTitle.HeaderText = "Title";
            this.ColumnTitle.Name = "ColumnTitle";
            this.ColumnTitle.ReadOnly = true;
            this.ColumnTitle.Width = 52;
            // 
            // ColumnDate
            // 
            this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnDate.HeaderText = "Date";
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.Width = 55;
            // 
            // ColumnParticipant
            // 
            this.ColumnParticipant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnParticipant.HeaderText = "Speaker";
            this.ColumnParticipant.Name = "ColumnParticipant";
            this.ColumnParticipant.ReadOnly = true;
            this.ColumnParticipant.Width = 72;
            // 
            // tabControlImport
            // 
            this.tabControlImport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlImport.Controls.Add(this.tabPageProjects);
            this.tabControlImport.Controls.Add(this.tabPageEvents);
            this.tabControlImport.Controls.Add(this.tabPageFieldMatching);
            this.tabControlImport.Location = new System.Drawing.Point(13, 13);
            this.tabControlImport.Name = "tabControlImport";
            this.tabControlImport.SelectedIndex = 0;
            this.tabControlImport.Size = new System.Drawing.Size(709, 329);
            this.tabControlImport.TabIndex = 2;
            this.tabControlImport.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabControlSelecting);
            // 
            // tabPageProjects
            // 
            this.tabPageProjects.Controls.Add(this.listBoxProjects);
            this.tabPageProjects.Location = new System.Drawing.Point(4, 22);
            this.tabPageProjects.Name = "tabPageProjects";
            this.tabPageProjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProjects.Size = new System.Drawing.Size(701, 303);
            this.tabPageProjects.TabIndex = 0;
            this.tabPageProjects.Text = "Projects";
            this.tabPageProjects.UseVisualStyleBackColor = true;
            // 
            // listBoxProjects
            // 
            this.listBoxProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxProjects.FormattingEnabled = true;
            this.listBoxProjects.Location = new System.Drawing.Point(3, 3);
            this.listBoxProjects.Name = "listBoxProjects";
            this.listBoxProjects.Size = new System.Drawing.Size(695, 237);
            this.listBoxProjects.TabIndex = 0;
            this.listBoxProjects.SelectedIndexChanged += new System.EventHandler(this.ListBoxProjectsSelectedIndexChanged);
            // 
            // tabPageEvents
            // 
            this.tabPageEvents.Controls.Add(this.dataGridViewEvents);
            this.tabPageEvents.Location = new System.Drawing.Point(4, 22);
            this.tabPageEvents.Name = "tabPageEvents";
            this.tabPageEvents.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEvents.Size = new System.Drawing.Size(701, 303);
            this.tabPageEvents.TabIndex = 1;
            this.tabPageEvents.Text = "Sessions";
            this.tabPageEvents.UseVisualStyleBackColor = true;
            // 
            // tabPageFieldMatching
            // 
            this.tabPageFieldMatching.Controls.Add(this.groupBox1);
            this.tabPageFieldMatching.Controls.Add(this.dataGridViewTiers);
            this.tabPageFieldMatching.Controls.Add(this.buttonImport);
            this.tabPageFieldMatching.Location = new System.Drawing.Point(4, 22);
            this.tabPageFieldMatching.Name = "tabPageFieldMatching";
            this.tabPageFieldMatching.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFieldMatching.Size = new System.Drawing.Size(701, 303);
            this.tabPageFieldMatching.TabIndex = 3;
            this.tabPageFieldMatching.Text = "Choose Fields";
            this.tabPageFieldMatching.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAsAnswers);
            this.groupBox1.Controls.Add(this.radioButtonAsRetelling);
            this.groupBox1.Controls.Add(this.radioButtonNewStory);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import into?";
            // 
            // radioButtonAsAnswers
            // 
            this.radioButtonAsAnswers.AutoSize = true;
            this.radioButtonAsAnswers.Location = new System.Drawing.Point(372, 20);
            this.radioButtonAsAnswers.Name = "radioButtonAsAnswers";
            this.radioButtonAsAnswers.Size = new System.Drawing.Size(151, 17);
            this.radioButtonAsAnswers.TabIndex = 2;
            this.radioButtonAsAnswers.TabStop = true;
            this.radioButtonAsAnswers.Text = "&Answers to Test Questions";
            this.radioButtonAsAnswers.UseVisualStyleBackColor = true;
            this.radioButtonAsAnswers.CheckedChanged += new System.EventHandler(this.RadioButtonNewStoryCheckedChanged);
            // 
            // radioButtonAsRetelling
            // 
            this.radioButtonAsRetelling.AutoSize = true;
            this.radioButtonAsRetelling.Location = new System.Drawing.Point(135, 20);
            this.radioButtonAsRetelling.Name = "radioButtonAsRetelling";
            this.radioButtonAsRetelling.Size = new System.Drawing.Size(66, 17);
            this.radioButtonAsRetelling.TabIndex = 1;
            this.radioButtonAsRetelling.TabStop = true;
            this.radioButtonAsRetelling.Text = "&Retelling";
            this.radioButtonAsRetelling.UseVisualStyleBackColor = true;
            this.radioButtonAsRetelling.CheckedChanged += new System.EventHandler(this.RadioButtonNewStoryCheckedChanged);
            // 
            // radioButtonNewStory
            // 
            this.radioButtonNewStory.AutoSize = true;
            this.radioButtonNewStory.Checked = true;
            this.radioButtonNewStory.Location = new System.Drawing.Point(17, 20);
            this.radioButtonNewStory.Name = "radioButtonNewStory";
            this.radioButtonNewStory.Size = new System.Drawing.Size(74, 17);
            this.radioButtonNewStory.TabIndex = 0;
            this.radioButtonNewStory.TabStop = true;
            this.radioButtonNewStory.Text = "&New Story";
            this.radioButtonNewStory.UseVisualStyleBackColor = true;
            this.radioButtonNewStory.CheckedChanged += new System.EventHandler(this.RadioButtonNewStoryCheckedChanged);
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonImport.Location = new System.Drawing.Point(313, 274);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 3;
            this.buttonImport.Text = "&Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.ButtonImportClick);
            // 
            // dataGridViewTiers
            // 
            this.dataGridViewTiers.AllowUserToAddRows = false;
            this.dataGridViewTiers.AllowUserToDeleteRows = false;
            this.dataGridViewTiers.AllowUserToResizeRows = false;
            this.dataGridViewTiers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTiers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTierName,
            this.ColumnTierLanguage,
            this.ColumnTierSample,
            this.ColumnTierField});
            this.dataGridViewTiers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewTiers.Location = new System.Drawing.Point(13, 72);
            this.dataGridViewTiers.MultiSelect = false;
            this.dataGridViewTiers.Name = "dataGridViewTiers";
            this.dataGridViewTiers.RowHeadersVisible = false;
            this.dataGridViewTiers.RowTemplate.Height = 24;
            this.dataGridViewTiers.Size = new System.Drawing.Size(679, 190);
            this.dataGridViewTiers.TabIndex = 1;
            this.dataGridViewTiers.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridViewTiersDataError);
            // 
            // ColumnTierName
            // 
            this.ColumnTierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnTierName.HeaderText = "Tier";
            this.ColumnTierName.Name = "ColumnTierName";
            this.ColumnTierName.ReadOnly = true;
            this.ColumnTierName.ToolTipText = "The tier (or line) of data found in the file being imported";
            // 
            // ColumnTierLanguage
            // 
            this.ColumnTierLanguage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnTierLanguage.HeaderText = "Language";
            this.ColumnTierLanguage.Name = "ColumnTierLanguage";
            this.ColumnTierLanguage.ReadOnly = true;
            // 
            // ColumnTierSample
            // 
            this.ColumnTierSample.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnTierSample.HeaderText = "First line";
            this.ColumnTierSample.Name = "ColumnTierSample";
            this.ColumnTierSample.ReadOnly = true;
            // 
            // ColumnTierField
            // 
            this.ColumnTierField.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnTierField.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.ColumnTierField.HeaderText = "Import into";
            this.ColumnTierField.MinimumWidth = 200;
            this.ColumnTierField.Name = "ColumnTierField";
            this.ColumnTierField.ToolTipText = "Choose which field of the story lines this tier should be imported into";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(330, 354);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // SayMoreImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 389);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControlImport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SayMoreImportForm";
            this.Text = "Import from SayMore";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTiers)).EndInit();
            this.tabControlImport.ResumeLayout(false);
            this.tabPageProjects.ResumeLayout(false);
            this.tabPageEvents.ResumeLayout(false);
            this.tabPageFieldMatching.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewEvents;
        private System.Windows.Forms.TabControl tabControlImport;
        private System.Windows.Forms.TabPage tabPageProjects;
        private System.Windows.Forms.TabPage tabPageEvents;
        private System.Windows.Forms.ListBox listBoxProjects;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnImport;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSessions;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnParticipant;
        private System.Windows.Forms.TabPage tabPageFieldMatching;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.DataGridView dataGridViewTiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTierLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTierSample;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnTierField;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonAsRetelling;
        private System.Windows.Forms.RadioButton radioButtonNewStory;
        private System.Windows.Forms.RadioButton radioButtonAsAnswers;
    }
}