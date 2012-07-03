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
            this.radioButtonAsRetelling = new System.Windows.Forms.RadioButton();
            this.radioButtonNewStory = new System.Windows.Forms.RadioButton();
            this.buttonImport = new System.Windows.Forms.Button();
            this.groupBoxTranslation = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelTranslation = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonVernacularTranslation = new System.Windows.Forms.RadioButton();
            this.radioButtonNationalBtTranslation = new System.Windows.Forms.RadioButton();
            this.radioButtonInternationalBtTranslation = new System.Windows.Forms.RadioButton();
            this.radioButtonFreeTrTranslation = new System.Windows.Forms.RadioButton();
            this.groupBoxTranscription = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelTranscription = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonVernacularTranscription = new System.Windows.Forms.RadioButton();
            this.radioButtonNationalBtTranscription = new System.Windows.Forms.RadioButton();
            this.radioButtonInternationalBtTranscription = new System.Windows.Forms.RadioButton();
            this.radioButtonFreeTrTranscription = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).BeginInit();
            this.tabControlImport.SuspendLayout();
            this.tabPageProjects.SuspendLayout();
            this.tabPageEvents.SuspendLayout();
            this.tabPageFieldMatching.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxTranslation.SuspendLayout();
            this.flowLayoutPanelTranslation.SuspendLayout();
            this.groupBoxTranscription.SuspendLayout();
            this.flowLayoutPanelTranscription.SuspendLayout();
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
            this.tabControlImport.Size = new System.Drawing.Size(709, 269);
            this.tabControlImport.TabIndex = 2;
            this.tabControlImport.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabControlSelecting);
            // 
            // tabPageProjects
            // 
            this.tabPageProjects.Controls.Add(this.listBoxProjects);
            this.tabPageProjects.Location = new System.Drawing.Point(4, 22);
            this.tabPageProjects.Name = "tabPageProjects";
            this.tabPageProjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProjects.Size = new System.Drawing.Size(701, 243);
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
            this.tabPageEvents.Size = new System.Drawing.Size(701, 243);
            this.tabPageEvents.TabIndex = 1;
            this.tabPageEvents.Text = "Sessions";
            this.tabPageEvents.UseVisualStyleBackColor = true;
            // 
            // tabPageFieldMatching
            // 
            this.tabPageFieldMatching.Controls.Add(this.groupBox1);
            this.tabPageFieldMatching.Controls.Add(this.buttonImport);
            this.tabPageFieldMatching.Controls.Add(this.groupBoxTranslation);
            this.tabPageFieldMatching.Controls.Add(this.groupBoxTranscription);
            this.tabPageFieldMatching.Location = new System.Drawing.Point(4, 22);
            this.tabPageFieldMatching.Name = "tabPageFieldMatching";
            this.tabPageFieldMatching.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFieldMatching.Size = new System.Drawing.Size(701, 243);
            this.tabPageFieldMatching.TabIndex = 3;
            this.tabPageFieldMatching.Text = "Choose Fields";
            this.tabPageFieldMatching.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAsRetelling);
            this.groupBox1.Controls.Add(this.radioButtonNewStory);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 47);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import into?";
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
            this.buttonImport.Location = new System.Drawing.Point(313, 214);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 4;
            this.buttonImport.Text = "&Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.ButtonImportClick);
            // 
            // groupBoxTranslation
            // 
            this.groupBoxTranslation.Controls.Add(this.flowLayoutPanelTranslation);
            this.groupBoxTranslation.Location = new System.Drawing.Point(13, 141);
            this.groupBoxTranslation.Name = "groupBoxTranslation";
            this.groupBoxTranslation.Size = new System.Drawing.Size(688, 54);
            this.groupBoxTranslation.TabIndex = 1;
            this.groupBoxTranslation.TabStop = false;
            this.groupBoxTranslation.Text = "Translation &Field";
            // 
            // flowLayoutPanelTranslation
            // 
            this.flowLayoutPanelTranslation.Controls.Add(this.radioButtonVernacularTranslation);
            this.flowLayoutPanelTranslation.Controls.Add(this.radioButtonNationalBtTranslation);
            this.flowLayoutPanelTranslation.Controls.Add(this.radioButtonInternationalBtTranslation);
            this.flowLayoutPanelTranslation.Controls.Add(this.radioButtonFreeTrTranslation);
            this.flowLayoutPanelTranslation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelTranslation.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelTranslation.Name = "flowLayoutPanelTranslation";
            this.flowLayoutPanelTranslation.Size = new System.Drawing.Size(682, 35);
            this.flowLayoutPanelTranslation.TabIndex = 0;
            // 
            // radioButtonVernacularTranslation
            // 
            this.radioButtonVernacularTranslation.AutoSize = true;
            this.radioButtonVernacularTranslation.Location = new System.Drawing.Point(3, 3);
            this.radioButtonVernacularTranslation.Name = "radioButtonVernacularTranslation";
            this.radioButtonVernacularTranslation.Size = new System.Drawing.Size(100, 17);
            this.radioButtonVernacularTranslation.TabIndex = 0;
            this.radioButtonVernacularTranslation.TabStop = true;
            this.radioButtonVernacularTranslation.Text = "&Story Language";
            this.radioButtonVernacularTranslation.UseVisualStyleBackColor = true;
            this.radioButtonVernacularTranslation.Visible = false;
            // 
            // radioButtonNationalBtTranslation
            // 
            this.radioButtonNationalBtTranslation.AutoSize = true;
            this.radioButtonNationalBtTranslation.Location = new System.Drawing.Point(109, 3);
            this.radioButtonNationalBtTranslation.Name = "radioButtonNationalBtTranslation";
            this.radioButtonNationalBtTranslation.Size = new System.Drawing.Size(175, 17);
            this.radioButtonNationalBtTranslation.TabIndex = 1;
            this.radioButtonNationalBtTranslation.TabStop = true;
            this.radioButtonNationalBtTranslation.Text = "&National/Regional language BT";
            this.radioButtonNationalBtTranslation.UseVisualStyleBackColor = true;
            this.radioButtonNationalBtTranslation.Visible = false;
            // 
            // radioButtonInternationalBtTranslation
            // 
            this.radioButtonInternationalBtTranslation.AutoSize = true;
            this.radioButtonInternationalBtTranslation.Location = new System.Drawing.Point(290, 3);
            this.radioButtonInternationalBtTranslation.Name = "radioButtonInternationalBtTranslation";
            this.radioButtonInternationalBtTranslation.Size = new System.Drawing.Size(123, 17);
            this.radioButtonInternationalBtTranslation.TabIndex = 2;
            this.radioButtonInternationalBtTranslation.TabStop = true;
            this.radioButtonInternationalBtTranslation.Text = "&English language BT";
            this.radioButtonInternationalBtTranslation.UseVisualStyleBackColor = true;
            this.radioButtonInternationalBtTranslation.Visible = false;
            // 
            // radioButtonFreeTrTranslation
            // 
            this.radioButtonFreeTrTranslation.AutoSize = true;
            this.radioButtonFreeTrTranslation.Location = new System.Drawing.Point(419, 3);
            this.radioButtonFreeTrTranslation.Name = "radioButtonFreeTrTranslation";
            this.radioButtonFreeTrTranslation.Size = new System.Drawing.Size(101, 17);
            this.radioButtonFreeTrTranslation.TabIndex = 3;
            this.radioButtonFreeTrTranslation.TabStop = true;
            this.radioButtonFreeTrTranslation.Text = "&Free Translation";
            this.radioButtonFreeTrTranslation.UseVisualStyleBackColor = true;
            this.radioButtonFreeTrTranslation.Visible = false;
            // 
            // groupBoxTranscription
            // 
            this.groupBoxTranscription.Controls.Add(this.flowLayoutPanelTranscription);
            this.groupBoxTranscription.Location = new System.Drawing.Point(10, 72);
            this.groupBoxTranscription.Name = "groupBoxTranscription";
            this.groupBoxTranscription.Size = new System.Drawing.Size(688, 54);
            this.groupBoxTranscription.TabIndex = 0;
            this.groupBoxTranscription.TabStop = false;
            this.groupBoxTranscription.Text = "&Transcription Field";
            // 
            // flowLayoutPanelTranscription
            // 
            this.flowLayoutPanelTranscription.Controls.Add(this.radioButtonVernacularTranscription);
            this.flowLayoutPanelTranscription.Controls.Add(this.radioButtonNationalBtTranscription);
            this.flowLayoutPanelTranscription.Controls.Add(this.radioButtonInternationalBtTranscription);
            this.flowLayoutPanelTranscription.Controls.Add(this.radioButtonFreeTrTranscription);
            this.flowLayoutPanelTranscription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelTranscription.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelTranscription.Name = "flowLayoutPanelTranscription";
            this.flowLayoutPanelTranscription.Size = new System.Drawing.Size(682, 35);
            this.flowLayoutPanelTranscription.TabIndex = 0;
            // 
            // radioButtonVernacularTranscription
            // 
            this.radioButtonVernacularTranscription.AutoSize = true;
            this.radioButtonVernacularTranscription.Location = new System.Drawing.Point(3, 3);
            this.radioButtonVernacularTranscription.Name = "radioButtonVernacularTranscription";
            this.radioButtonVernacularTranscription.Size = new System.Drawing.Size(100, 17);
            this.radioButtonVernacularTranscription.TabIndex = 0;
            this.radioButtonVernacularTranscription.TabStop = true;
            this.radioButtonVernacularTranscription.Text = "&Story Language";
            this.radioButtonVernacularTranscription.UseVisualStyleBackColor = true;
            this.radioButtonVernacularTranscription.Visible = false;
            // 
            // radioButtonNationalBtTranscription
            // 
            this.radioButtonNationalBtTranscription.AutoSize = true;
            this.radioButtonNationalBtTranscription.Location = new System.Drawing.Point(109, 3);
            this.radioButtonNationalBtTranscription.Name = "radioButtonNationalBtTranscription";
            this.radioButtonNationalBtTranscription.Size = new System.Drawing.Size(175, 17);
            this.radioButtonNationalBtTranscription.TabIndex = 1;
            this.radioButtonNationalBtTranscription.TabStop = true;
            this.radioButtonNationalBtTranscription.Text = "&National/Regional language BT";
            this.radioButtonNationalBtTranscription.UseVisualStyleBackColor = true;
            this.radioButtonNationalBtTranscription.Visible = false;
            // 
            // radioButtonInternationalBtTranscription
            // 
            this.radioButtonInternationalBtTranscription.AutoSize = true;
            this.radioButtonInternationalBtTranscription.Location = new System.Drawing.Point(290, 3);
            this.radioButtonInternationalBtTranscription.Name = "radioButtonInternationalBtTranscription";
            this.radioButtonInternationalBtTranscription.Size = new System.Drawing.Size(123, 17);
            this.radioButtonInternationalBtTranscription.TabIndex = 2;
            this.radioButtonInternationalBtTranscription.TabStop = true;
            this.radioButtonInternationalBtTranscription.Text = "&English language BT";
            this.radioButtonInternationalBtTranscription.UseVisualStyleBackColor = true;
            this.radioButtonInternationalBtTranscription.Visible = false;
            // 
            // radioButtonFreeTrTranscription
            // 
            this.radioButtonFreeTrTranscription.AutoSize = true;
            this.radioButtonFreeTrTranscription.Location = new System.Drawing.Point(419, 3);
            this.radioButtonFreeTrTranscription.Name = "radioButtonFreeTrTranscription";
            this.radioButtonFreeTrTranscription.Size = new System.Drawing.Size(101, 17);
            this.radioButtonFreeTrTranscription.TabIndex = 3;
            this.radioButtonFreeTrTranscription.TabStop = true;
            this.radioButtonFreeTrTranscription.Text = "&Free Translation";
            this.radioButtonFreeTrTranscription.UseVisualStyleBackColor = true;
            this.radioButtonFreeTrTranscription.Visible = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(330, 294);
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
            this.ClientSize = new System.Drawing.Size(734, 329);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControlImport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SayMoreImportForm";
            this.Text = "Import from SayMore";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).EndInit();
            this.tabControlImport.ResumeLayout(false);
            this.tabPageProjects.ResumeLayout(false);
            this.tabPageEvents.ResumeLayout(false);
            this.tabPageFieldMatching.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxTranslation.ResumeLayout(false);
            this.flowLayoutPanelTranslation.ResumeLayout(false);
            this.flowLayoutPanelTranslation.PerformLayout();
            this.groupBoxTranscription.ResumeLayout(false);
            this.flowLayoutPanelTranscription.ResumeLayout(false);
            this.flowLayoutPanelTranscription.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxTranscription;
        private System.Windows.Forms.GroupBox groupBoxTranslation;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTranscription;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTranslation;
        private System.Windows.Forms.RadioButton radioButtonVernacularTranscription;
        private System.Windows.Forms.RadioButton radioButtonVernacularTranslation;
        private System.Windows.Forms.RadioButton radioButtonNationalBtTranscription;
        private System.Windows.Forms.RadioButton radioButtonNationalBtTranslation;
        private System.Windows.Forms.RadioButton radioButtonInternationalBtTranscription;
        private System.Windows.Forms.RadioButton radioButtonInternationalBtTranslation;
        private System.Windows.Forms.RadioButton radioButtonFreeTrTranscription;
        private System.Windows.Forms.RadioButton radioButtonFreeTrTranslation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonAsRetelling;
        private System.Windows.Forms.RadioButton radioButtonNewStory;
    }
}