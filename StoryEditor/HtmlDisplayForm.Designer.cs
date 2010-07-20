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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlDisplayForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSelectReportOptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelSettings = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxLangVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxLangNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangInternationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxAnchors = new System.Windows.Forms.CheckBox();
            this.checkBoxStoryTestingQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxRetellings = new System.Windows.Forms.CheckBox();
            this.groupBoxRevisionsBy = new System.Windows.Forms.GroupBox();
            this.radioButtonShowAllRevisions = new System.Windows.Forms.RadioButton();
            this.radioButtonRevsByChangeOfState = new System.Windows.Forms.RadioButton();
            this.tabPageDisplayChangeReport = new System.Windows.Forms.TabPage();
            this.groupBoxViewOptions = new System.Windows.Forms.GroupBox();
            this.htmlStoryBtControl = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.tabControl.SuspendLayout();
            this.tabPageSelectReportOptions.SuspendLayout();
            this.tableLayoutPanelSettings.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBoxRevisionsBy.SuspendLayout();
            this.tabPageDisplayChangeReport.SuspendLayout();
            this.groupBoxViewOptions.SuspendLayout();
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
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSettings.Controls.Add(this.groupBoxRevisionsBy, 0, 0);
            this.tableLayoutPanelSettings.Controls.Add(this.groupBoxViewOptions, 1, 0);
            this.tableLayoutPanelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSettings.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelSettings.Name = "tableLayoutPanelSettings";
            this.tableLayoutPanelSettings.RowCount = 2;
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelSettings.Size = new System.Drawing.Size(723, 553);
            this.tableLayoutPanelSettings.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangVernacular);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangNationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangInternationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxAnchors);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxStoryTestingQuestions);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxRetellings);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(482, 73);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // checkBoxLangVernacular
            // 
            this.checkBoxLangVernacular.AutoSize = true;
            this.checkBoxLangVernacular.Checked = true;
            this.checkBoxLangVernacular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangVernacular.Location = new System.Drawing.Point(3, 3);
            this.checkBoxLangVernacular.Name = "checkBoxLangVernacular";
            this.checkBoxLangVernacular.Size = new System.Drawing.Size(101, 17);
            this.checkBoxLangVernacular.TabIndex = 0;
            this.checkBoxLangVernacular.Text = "LangVernacular";
            this.checkBoxLangVernacular.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangNationalBT
            // 
            this.checkBoxLangNationalBT.AutoSize = true;
            this.checkBoxLangNationalBT.Checked = true;
            this.checkBoxLangNationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangNationalBT.Location = new System.Drawing.Point(3, 26);
            this.checkBoxLangNationalBT.Name = "checkBoxLangNationalBT";
            this.checkBoxLangNationalBT.Size = new System.Drawing.Size(103, 17);
            this.checkBoxLangNationalBT.TabIndex = 1;
            this.checkBoxLangNationalBT.Text = "LangNationalBT";
            this.checkBoxLangNationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangInternationalBT
            // 
            this.checkBoxLangInternationalBT.AutoSize = true;
            this.checkBoxLangInternationalBT.Checked = true;
            this.checkBoxLangInternationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangInternationalBT.Location = new System.Drawing.Point(3, 49);
            this.checkBoxLangInternationalBT.Name = "checkBoxLangInternationalBT";
            this.checkBoxLangInternationalBT.Size = new System.Drawing.Size(165, 17);
            this.checkBoxLangInternationalBT.TabIndex = 2;
            this.checkBoxLangInternationalBT.Text = "&English back translation fields";
            this.checkBoxLangInternationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnchors
            // 
            this.checkBoxAnchors.AutoSize = true;
            this.checkBoxAnchors.Checked = true;
            this.checkBoxAnchors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnchors.Location = new System.Drawing.Point(174, 3);
            this.checkBoxAnchors.Name = "checkBoxAnchors";
            this.checkBoxAnchors.Size = new System.Drawing.Size(65, 17);
            this.checkBoxAnchors.TabIndex = 3;
            this.checkBoxAnchors.Text = "&Anchors";
            this.checkBoxAnchors.UseVisualStyleBackColor = true;
            // 
            // checkBoxStoryTestingQuestions
            // 
            this.checkBoxStoryTestingQuestions.AutoSize = true;
            this.checkBoxStoryTestingQuestions.Checked = true;
            this.checkBoxStoryTestingQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStoryTestingQuestions.Location = new System.Drawing.Point(174, 26);
            this.checkBoxStoryTestingQuestions.Name = "checkBoxStoryTestingQuestions";
            this.checkBoxStoryTestingQuestions.Size = new System.Drawing.Size(132, 17);
            this.checkBoxStoryTestingQuestions.TabIndex = 4;
            this.checkBoxStoryTestingQuestions.Text = "Story &testing questions";
            this.checkBoxStoryTestingQuestions.UseVisualStyleBackColor = true;
            // 
            // checkBoxRetellings
            // 
            this.checkBoxRetellings.AutoSize = true;
            this.checkBoxRetellings.Checked = true;
            this.checkBoxRetellings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRetellings.Location = new System.Drawing.Point(174, 49);
            this.checkBoxRetellings.Name = "checkBoxRetellings";
            this.checkBoxRetellings.Size = new System.Drawing.Size(72, 17);
            this.checkBoxRetellings.TabIndex = 5;
            this.checkBoxRetellings.Text = "&Retellings";
            this.checkBoxRetellings.UseVisualStyleBackColor = true;
            // 
            // groupBoxRevisionsBy
            // 
            this.groupBoxRevisionsBy.Controls.Add(this.radioButtonShowAllRevisions);
            this.groupBoxRevisionsBy.Controls.Add(this.radioButtonRevsByChangeOfState);
            this.groupBoxRevisionsBy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRevisionsBy.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRevisionsBy.Name = "groupBoxRevisionsBy";
            this.groupBoxRevisionsBy.Size = new System.Drawing.Size(223, 92);
            this.groupBoxRevisionsBy.TabIndex = 0;
            this.groupBoxRevisionsBy.TabStop = false;
            this.groupBoxRevisionsBy.Text = "Show Revisions By";
            // 
            // radioButtonShowAllRevisions
            // 
            this.radioButtonShowAllRevisions.AutoSize = true;
            this.radioButtonShowAllRevisions.Location = new System.Drawing.Point(139, 38);
            this.radioButtonShowAllRevisions.Name = "radioButtonShowAllRevisions";
            this.radioButtonShowAllRevisions.Size = new System.Drawing.Size(66, 17);
            this.radioButtonShowAllRevisions.TabIndex = 1;
            this.radioButtonShowAllRevisions.Text = "Show &All";
            this.radioButtonShowAllRevisions.UseVisualStyleBackColor = true;
            // 
            // radioButtonRevsByChangeOfState
            // 
            this.radioButtonRevsByChangeOfState.AutoSize = true;
            this.radioButtonRevsByChangeOfState.Checked = true;
            this.radioButtonRevsByChangeOfState.Location = new System.Drawing.Point(17, 38);
            this.radioButtonRevsByChangeOfState.Name = "radioButtonRevsByChangeOfState";
            this.radioButtonRevsByChangeOfState.Size = new System.Drawing.Size(100, 17);
            this.radioButtonRevsByChangeOfState.TabIndex = 0;
            this.radioButtonRevsByChangeOfState.TabStop = true;
            this.radioButtonRevsByChangeOfState.Text = "&Change of state";
            this.radioButtonRevsByChangeOfState.UseVisualStyleBackColor = true;
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
            // groupBoxViewOptions
            // 
            this.groupBoxViewOptions.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxViewOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxViewOptions.Location = new System.Drawing.Point(232, 3);
            this.groupBoxViewOptions.Name = "groupBoxViewOptions";
            this.groupBoxViewOptions.Size = new System.Drawing.Size(488, 92);
            this.groupBoxViewOptions.TabIndex = 1;
            this.groupBoxViewOptions.TabStop = false;
            this.groupBoxViewOptions.Text = "Include in report";
            // 
            // htmlStoryBtControl
            // 
            this.htmlStoryBtControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControl.Location = new System.Drawing.Point(3, 3);
            this.htmlStoryBtControl.LoggedOnMember = null;
            this.htmlStoryBtControl.MembersData = null;
            this.htmlStoryBtControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControl.Name = "htmlStoryBtControl";
            this.htmlStoryBtControl.Size = new System.Drawing.Size(723, 553);
            this.htmlStoryBtControl.StoryData = null;
            this.htmlStoryBtControl.TabIndex = 0;
            this.htmlStoryBtControl.TheSE = null;
            // 
            // HtmlDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 585);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HtmlDisplayForm";
            this.Text = "Print Preview";
            this.tabControl.ResumeLayout(false);
            this.tabPageSelectReportOptions.ResumeLayout(false);
            this.tableLayoutPanelSettings.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBoxRevisionsBy.ResumeLayout(false);
            this.groupBoxRevisionsBy.PerformLayout();
            this.tabPageDisplayChangeReport.ResumeLayout(false);
            this.groupBoxViewOptions.ResumeLayout(false);
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxLangVernacular;
        private System.Windows.Forms.CheckBox checkBoxLangNationalBT;
        private System.Windows.Forms.CheckBox checkBoxLangInternationalBT;
        private System.Windows.Forms.CheckBox checkBoxAnchors;
        private System.Windows.Forms.CheckBox checkBoxStoryTestingQuestions;
        private System.Windows.Forms.CheckBox checkBoxRetellings;
        private System.Windows.Forms.GroupBox groupBoxViewOptions;
    }
}