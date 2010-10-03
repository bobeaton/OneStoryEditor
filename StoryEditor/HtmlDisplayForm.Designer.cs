namespace OneStoryProjectEditor
{
    partial class HtmlDisplayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlDisplayForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSelectReportOptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelSettings = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxRevisionsBy = new System.Windows.Forms.GroupBox();
            this.radioButtonShowAllWithState = new System.Windows.Forms.RadioButton();
            this.radioButtonShowAllRevisions = new System.Windows.Forms.RadioButton();
            this.radioButtonRevsByChangeOfState = new System.Windows.Forms.RadioButton();
            this.groupBoxViewOptions = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxFrontMatter = new System.Windows.Forms.CheckBox();
            this.checkBoxLangVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxLangNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangInternationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxAnchors = new System.Windows.Forms.CheckBox();
            this.checkBoxStoryTestingQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxAnswers = new System.Windows.Forms.CheckBox();
            this.checkBoxRetellings = new System.Windows.Forms.CheckBox();
            this.dataGridViewRevisions = new System.Windows.Forms.DataGridView();
            this.ColumnOldParent = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnNewChild = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnRevNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tabPageDisplayChangeReport = new System.Windows.Forms.TabPage();
            this.htmlStoryBtControl = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.backgroundWorkerCheckRevisions = new System.ComponentModel.BackgroundWorker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.tabPageSelectReportOptions.SuspendLayout();
            this.tableLayoutPanelSettings.SuspendLayout();
            this.groupBoxRevisionsBy.SuspendLayout();
            this.groupBoxViewOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRevisions)).BeginInit();
            this.tabPageDisplayChangeReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageSelectReportOptions);
            this.tabControl.Controls.Add(this.tabPageDisplayChangeReport);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(737, 585);
            this.tabControl.TabIndex = 1;
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabPageSelectReportOptions
            // 
            this.tabPageSelectReportOptions.Controls.Add(this.tableLayoutPanelSettings);
            this.tabPageSelectReportOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageSelectReportOptions.Name = "tabPageSelectReportOptions";
            this.tabPageSelectReportOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSelectReportOptions.Size = new System.Drawing.Size(729, 559);
            this.tabPageSelectReportOptions.TabIndex = 0;
            this.tabPageSelectReportOptions.Text = "Settings";
            this.tabPageSelectReportOptions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelSettings
            // 
            this.tableLayoutPanelSettings.ColumnCount = 2;
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanelSettings.Controls.Add(this.groupBoxRevisionsBy, 0, 0);
            this.tableLayoutPanelSettings.Controls.Add(this.groupBoxViewOptions, 1, 0);
            this.tableLayoutPanelSettings.Controls.Add(this.dataGridViewRevisions, 0, 1);
            this.tableLayoutPanelSettings.Controls.Add(this.progressBar, 0, 2);
            this.tableLayoutPanelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSettings.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelSettings.Name = "tableLayoutPanelSettings";
            this.tableLayoutPanelSettings.RowCount = 3;
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelSettings.Size = new System.Drawing.Size(723, 553);
            this.tableLayoutPanelSettings.TabIndex = 0;
            // 
            // groupBoxRevisionsBy
            // 
            this.groupBoxRevisionsBy.Controls.Add(this.radioButtonShowAllWithState);
            this.groupBoxRevisionsBy.Controls.Add(this.radioButtonShowAllRevisions);
            this.groupBoxRevisionsBy.Controls.Add(this.radioButtonRevsByChangeOfState);
            this.groupBoxRevisionsBy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRevisionsBy.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRevisionsBy.Name = "groupBoxRevisionsBy";
            this.groupBoxRevisionsBy.Size = new System.Drawing.Size(283, 145);
            this.groupBoxRevisionsBy.TabIndex = 0;
            this.groupBoxRevisionsBy.TabStop = false;
            this.groupBoxRevisionsBy.Text = "Show Revisions By";
            // 
            // radioButtonShowAllWithState
            // 
            this.radioButtonShowAllWithState.AutoSize = true;
            this.radioButtonShowAllWithState.Location = new System.Drawing.Point(6, 64);
            this.radioButtonShowAllWithState.Name = "radioButtonShowAllWithState";
            this.radioButtonShowAllWithState.Size = new System.Drawing.Size(174, 17);
            this.radioButtonShowAllWithState.TabIndex = 1;
            this.radioButtonShowAllWithState.Text = "&Show All (with state information)";
            this.toolTip.SetToolTip(this.radioButtonShowAllWithState, resources.GetString("radioButtonShowAllWithState.ToolTip"));
            this.radioButtonShowAllWithState.UseVisualStyleBackColor = true;
            this.radioButtonShowAllWithState.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonShowAllRevisions
            // 
            this.radioButtonShowAllRevisions.AutoSize = true;
            this.radioButtonShowAllRevisions.Location = new System.Drawing.Point(6, 41);
            this.radioButtonShowAllRevisions.Name = "radioButtonShowAllRevisions";
            this.radioButtonShowAllRevisions.Size = new System.Drawing.Size(66, 17);
            this.radioButtonShowAllRevisions.TabIndex = 1;
            this.radioButtonShowAllRevisions.Text = "Show &All";
            this.toolTip.SetToolTip(this.radioButtonShowAllRevisions, "This options lists all versions of the project file whether the story was actuall" +
                    "y changed or not.");
            this.radioButtonShowAllRevisions.UseVisualStyleBackColor = true;
            this.radioButtonShowAllRevisions.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonRevsByChangeOfState
            // 
            this.radioButtonRevsByChangeOfState.AutoSize = true;
            this.radioButtonRevsByChangeOfState.Checked = true;
            this.radioButtonRevsByChangeOfState.Location = new System.Drawing.Point(6, 19);
            this.radioButtonRevsByChangeOfState.Name = "radioButtonRevsByChangeOfState";
            this.radioButtonRevsByChangeOfState.Size = new System.Drawing.Size(100, 17);
            this.radioButtonRevsByChangeOfState.TabIndex = 0;
            this.radioButtonRevsByChangeOfState.TabStop = true;
            this.radioButtonRevsByChangeOfState.Text = "&Change of state";
            this.toolTip.SetToolTip(this.radioButtonRevsByChangeOfState, resources.GetString("radioButtonRevsByChangeOfState.ToolTip"));
            this.radioButtonRevsByChangeOfState.UseVisualStyleBackColor = true;
            this.radioButtonRevsByChangeOfState.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBoxViewOptions
            // 
            this.groupBoxViewOptions.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxViewOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxViewOptions.Location = new System.Drawing.Point(292, 3);
            this.groupBoxViewOptions.Name = "groupBoxViewOptions";
            this.groupBoxViewOptions.Size = new System.Drawing.Size(428, 145);
            this.groupBoxViewOptions.TabIndex = 1;
            this.groupBoxViewOptions.TabStop = false;
            this.groupBoxViewOptions.Text = "Include in report";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBoxFrontMatter);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangVernacular);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateVernacular);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangNationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateNationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangInternationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxAnchors);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxStoryTestingQuestions);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxAnswers);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxRetellings);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(422, 126);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // checkBoxFrontMatter
            // 
            this.checkBoxFrontMatter.AutoSize = true;
            this.checkBoxFrontMatter.Checked = true;
            this.checkBoxFrontMatter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFrontMatter.Location = new System.Drawing.Point(3, 3);
            this.checkBoxFrontMatter.Name = "checkBoxFrontMatter";
            this.checkBoxFrontMatter.Size = new System.Drawing.Size(143, 17);
            this.checkBoxFrontMatter.TabIndex = 6;
            this.checkBoxFrontMatter.Text = "Story Header Information";
            this.toolTip.SetToolTip(this.checkBoxFrontMatter, "Check this box to have the Story Header Information (e.g. Crafter, UNSs, etc) sho" +
                    "wn in the report");
            this.checkBoxFrontMatter.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangVernacular
            // 
            this.checkBoxLangVernacular.AutoSize = true;
            this.checkBoxLangVernacular.Checked = true;
            this.checkBoxLangVernacular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangVernacular.Location = new System.Drawing.Point(3, 26);
            this.checkBoxLangVernacular.Name = "checkBoxLangVernacular";
            this.checkBoxLangVernacular.Size = new System.Drawing.Size(101, 17);
            this.checkBoxLangVernacular.TabIndex = 0;
            this.checkBoxLangVernacular.Text = "LangVernacular";
            this.toolTip.SetToolTip(this.checkBoxLangVernacular, "Check this box to have the Vernacular language data shown in the report");
            this.checkBoxLangVernacular.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateVernacular
            // 
            this.checkBoxLangTransliterateVernacular.AutoSize = true;
            this.checkBoxLangTransliterateVernacular.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateVernacular.Location = new System.Drawing.Point(3, 49);
            this.checkBoxLangTransliterateVernacular.Name = "checkBoxLangTransliterateVernacular";
            this.checkBoxLangTransliterateVernacular.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateVernacular.TabIndex = 11;
            this.checkBoxLangTransliterateVernacular.Text = "Transliterate";
            this.checkBoxLangTransliterateVernacular.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateVernacular.Visible = false;
            // 
            // checkBoxLangNationalBT
            // 
            this.checkBoxLangNationalBT.AutoSize = true;
            this.checkBoxLangNationalBT.Checked = true;
            this.checkBoxLangNationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangNationalBT.Location = new System.Drawing.Point(3, 72);
            this.checkBoxLangNationalBT.Name = "checkBoxLangNationalBT";
            this.checkBoxLangNationalBT.Size = new System.Drawing.Size(103, 17);
            this.checkBoxLangNationalBT.TabIndex = 1;
            this.checkBoxLangNationalBT.Text = "LangNationalBT";
            this.toolTip.SetToolTip(this.checkBoxLangNationalBT, "Check this box to have the National language back-translation shown in the report" +
                    "");
            this.checkBoxLangNationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateNationalBT
            // 
            this.checkBoxLangTransliterateNationalBT.AutoSize = true;
            this.checkBoxLangTransliterateNationalBT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateNationalBT.Location = new System.Drawing.Point(3, 95);
            this.checkBoxLangTransliterateNationalBT.Name = "checkBoxLangTransliterateNationalBT";
            this.checkBoxLangTransliterateNationalBT.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateNationalBT.TabIndex = 12;
            this.checkBoxLangTransliterateNationalBT.Text = "Transliterate";
            this.checkBoxLangTransliterateNationalBT.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateNationalBT.Visible = false;
            // 
            // checkBoxLangInternationalBT
            // 
            this.checkBoxLangInternationalBT.AutoSize = true;
            this.checkBoxLangInternationalBT.Checked = true;
            this.checkBoxLangInternationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangInternationalBT.Location = new System.Drawing.Point(152, 3);
            this.checkBoxLangInternationalBT.Name = "checkBoxLangInternationalBT";
            this.checkBoxLangInternationalBT.Size = new System.Drawing.Size(165, 17);
            this.checkBoxLangInternationalBT.TabIndex = 2;
            this.checkBoxLangInternationalBT.Text = "&English back translation fields";
            this.toolTip.SetToolTip(this.checkBoxLangInternationalBT, "Check this box to have the English language back-translation shown in the report");
            this.checkBoxLangInternationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnchors
            // 
            this.checkBoxAnchors.AutoSize = true;
            this.checkBoxAnchors.Checked = true;
            this.checkBoxAnchors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnchors.Location = new System.Drawing.Point(152, 26);
            this.checkBoxAnchors.Name = "checkBoxAnchors";
            this.checkBoxAnchors.Size = new System.Drawing.Size(65, 17);
            this.checkBoxAnchors.TabIndex = 3;
            this.checkBoxAnchors.Text = "&Anchors";
            this.toolTip.SetToolTip(this.checkBoxAnchors, "Check this box to have the Anchors shown in the report");
            this.checkBoxAnchors.UseVisualStyleBackColor = true;
            // 
            // checkBoxStoryTestingQuestions
            // 
            this.checkBoxStoryTestingQuestions.AutoSize = true;
            this.checkBoxStoryTestingQuestions.Checked = true;
            this.checkBoxStoryTestingQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStoryTestingQuestions.Location = new System.Drawing.Point(152, 49);
            this.checkBoxStoryTestingQuestions.Name = "checkBoxStoryTestingQuestions";
            this.checkBoxStoryTestingQuestions.Size = new System.Drawing.Size(132, 17);
            this.checkBoxStoryTestingQuestions.TabIndex = 4;
            this.checkBoxStoryTestingQuestions.Text = "Story &testing questions";
            this.toolTip.SetToolTip(this.checkBoxStoryTestingQuestions, "Check this box to have the Testing Questions (and answers, if available) shown in" +
                    " the report");
            this.checkBoxStoryTestingQuestions.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnswers
            // 
            this.checkBoxAnswers.AutoSize = true;
            this.checkBoxAnswers.Checked = true;
            this.checkBoxAnswers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnswers.Location = new System.Drawing.Point(152, 72);
            this.checkBoxAnswers.Name = "checkBoxAnswers";
            this.checkBoxAnswers.Size = new System.Drawing.Size(132, 17);
            this.checkBoxAnswers.TabIndex = 9;
            this.checkBoxAnswers.Text = "Test question &answers";
            this.checkBoxAnswers.UseVisualStyleBackColor = true;
            // 
            // checkBoxRetellings
            // 
            this.checkBoxRetellings.AutoSize = true;
            this.checkBoxRetellings.Checked = true;
            this.checkBoxRetellings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRetellings.Location = new System.Drawing.Point(152, 95);
            this.checkBoxRetellings.Name = "checkBoxRetellings";
            this.checkBoxRetellings.Size = new System.Drawing.Size(72, 17);
            this.checkBoxRetellings.TabIndex = 5;
            this.checkBoxRetellings.Text = "&Retellings";
            this.toolTip.SetToolTip(this.checkBoxRetellings, "Check this box to have the Retelling fields (if available) shown in the report");
            this.checkBoxRetellings.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRevisions
            // 
            this.dataGridViewRevisions.AllowUserToAddRows = false;
            this.dataGridViewRevisions.AllowUserToDeleteRows = false;
            this.dataGridViewRevisions.AllowUserToResizeRows = false;
            this.dataGridViewRevisions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRevisions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRevisions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnOldParent,
            this.ColumnNewChild,
            this.ColumnRevNumber,
            this.ColumnDate,
            this.ColumnPerson,
            this.ColumnState});
            this.tableLayoutPanelSettings.SetColumnSpan(this.dataGridViewRevisions, 2);
            this.dataGridViewRevisions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRevisions.Location = new System.Drawing.Point(3, 154);
            this.dataGridViewRevisions.Name = "dataGridViewRevisions";
            this.dataGridViewRevisions.RowHeadersVisible = false;
            this.dataGridViewRevisions.Size = new System.Drawing.Size(717, 376);
            this.dataGridViewRevisions.TabIndex = 2;
            this.dataGridViewRevisions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRevisions_CellClick);
            // 
            // ColumnOldParent
            // 
            this.ColumnOldParent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnOldParent.FillWeight = 35F;
            this.ColumnOldParent.HeaderText = "Old";
            this.ColumnOldParent.Name = "ColumnOldParent";
            this.ColumnOldParent.Width = 35;
            // 
            // ColumnNewChild
            // 
            this.ColumnNewChild.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnNewChild.FillWeight = 35F;
            this.ColumnNewChild.HeaderText = "New";
            this.ColumnNewChild.Name = "ColumnNewChild";
            this.ColumnNewChild.Width = 35;
            // 
            // ColumnRevNumber
            // 
            this.ColumnRevNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnRevNumber.HeaderText = "Revision";
            this.ColumnRevNumber.Name = "ColumnRevNumber";
            this.ColumnRevNumber.Width = 73;
            // 
            // ColumnDate
            // 
            this.ColumnDate.FillWeight = 106.4044F;
            this.ColumnDate.HeaderText = "Date";
            this.ColumnDate.Name = "ColumnDate";
            // 
            // ColumnPerson
            // 
            this.ColumnPerson.FillWeight = 106.4044F;
            this.ColumnPerson.HeaderText = "Person";
            this.ColumnPerson.Name = "ColumnPerson";
            // 
            // ColumnState
            // 
            this.ColumnState.FillWeight = 106.4044F;
            this.ColumnState.HeaderText = "State";
            this.ColumnState.Name = "ColumnState";
            // 
            // progressBar
            // 
            this.tableLayoutPanelSettings.SetColumnSpan(this.progressBar, 2);
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(3, 536);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(717, 14);
            this.progressBar.TabIndex = 3;
            this.progressBar.Visible = false;
            // 
            // tabPageDisplayChangeReport
            // 
            this.tabPageDisplayChangeReport.Controls.Add(this.htmlStoryBtControl);
            this.tabPageDisplayChangeReport.Location = new System.Drawing.Point(4, 22);
            this.tabPageDisplayChangeReport.Name = "tabPageDisplayChangeReport";
            this.tabPageDisplayChangeReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisplayChangeReport.Size = new System.Drawing.Size(729, 559);
            this.tabPageDisplayChangeReport.TabIndex = 1;
            this.tabPageDisplayChangeReport.Text = "View changes";
            this.tabPageDisplayChangeReport.UseVisualStyleBackColor = true;
            // 
            // htmlStoryBtControl
            // 
            this.htmlStoryBtControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControl.Location = new System.Drawing.Point(3, 3);
            this.htmlStoryBtControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControl.Name = "htmlStoryBtControl";
            this.htmlStoryBtControl.ParentStory = null;
            this.htmlStoryBtControl.Size = new System.Drawing.Size(723, 553);
            this.htmlStoryBtControl.StoryData = null;
            this.htmlStoryBtControl.TabIndex = 0;
            this.htmlStoryBtControl.TheSE = null;
            this.htmlStoryBtControl.ViewSettings = null;
            // 
            // backgroundWorkerCheckRevisions
            // 
            this.backgroundWorkerCheckRevisions.WorkerReportsProgress = true;
            this.backgroundWorkerCheckRevisions.WorkerSupportsCancellation = true;
            this.backgroundWorkerCheckRevisions.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCheckRevisions_DoWork);
            this.backgroundWorkerCheckRevisions.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerCheckRevisions_RunWorkerCompleted);
            this.backgroundWorkerCheckRevisions.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerCheckRevisions_ProgressChanged);
            // 
            // HtmlDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 585);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HtmlDisplayForm";
            this.Text = "Revision History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HtmlDisplayForm_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageSelectReportOptions.ResumeLayout(false);
            this.tableLayoutPanelSettings.ResumeLayout(false);
            this.groupBoxRevisionsBy.ResumeLayout(false);
            this.groupBoxRevisionsBy.PerformLayout();
            this.groupBoxViewOptions.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRevisions)).EndInit();
            this.tabPageDisplayChangeReport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private HtmlStoryBtControl htmlStoryBtControl;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageSelectReportOptions;
        private System.Windows.Forms.TabPage tabPageDisplayChangeReport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSettings;
        private System.Windows.Forms.GroupBox groupBoxRevisionsBy;
        private System.Windows.Forms.RadioButton radioButtonRevsByChangeOfState;
        private System.Windows.Forms.RadioButton radioButtonShowAllRevisions;
        private System.Windows.Forms.RadioButton radioButtonShowAllWithState;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxLangVernacular;
        private System.Windows.Forms.CheckBox checkBoxLangNationalBT;
        private System.Windows.Forms.CheckBox checkBoxLangInternationalBT;
        private System.Windows.Forms.CheckBox checkBoxAnchors;
        private System.Windows.Forms.CheckBox checkBoxStoryTestingQuestions;
        private System.Windows.Forms.CheckBox checkBoxRetellings;
        private System.Windows.Forms.GroupBox groupBoxViewOptions;
        private System.Windows.Forms.DataGridView dataGridViewRevisions;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCheckRevisions;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnOldParent;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnNewChild;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRevNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnState;
        private System.Windows.Forms.CheckBox checkBoxFrontMatter;
        private System.Windows.Forms.CheckBox checkBoxAnswers;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateVernacular;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateNationalBT;
    }
}