namespace OneStoryProjectEditor
{
    partial class PrintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxSelectAllFields = new System.Windows.Forms.CheckBox();
            this.checkedListBoxStories = new System.Windows.Forms.CheckedListBox();
            this.groupBoxViewOptions = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxShowLineNumbers = new System.Windows.Forms.CheckBox();
            this.checkBoxFrontMatter = new System.Windows.Forms.CheckBox();
            this.checkBoxLangVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxLangNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangInternationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateInternationalBt = new System.Windows.Forms.CheckBox();
            this.checkBoxLangFreeTranslation = new System.Windows.Forms.CheckBox();
            this.checkBoxLangTransliterateFreeTranslation = new System.Windows.Forms.CheckBox();
            this.checkBoxAnchors = new System.Windows.Forms.CheckBox();
            this.checkBoxExegeticalHelpNote = new System.Windows.Forms.CheckBox();
            this.checkBoxRetellings = new System.Windows.Forms.CheckBox();
            this.checkBoxGeneralTestingQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxStoryTestingQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxAnswers = new System.Windows.Forms.CheckBox();
            this.checkBoxShowHidden = new System.Windows.Forms.CheckBox();
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPagePrintPreviewConfig = new System.Windows.Forms.TabPage();
            this.tabPagePrintPreview = new System.Windows.Forms.TabPage();
            this.printViewer = new OneStoryProjectEditor.PrintViewer();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.checkBoxConsultantComments = new System.Windows.Forms.CheckBox();
            this.checkBoxCoachComments = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBoxViewOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPagePrintPreviewConfig.SuspendLayout();
            this.tabPagePrintPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.checkBoxSelectAllFields, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.checkedListBoxStories, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBoxViewOptions, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.checkBoxSelectAll, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(671, 357);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // checkBoxSelectAllFields
            // 
            this.checkBoxSelectAllFields.AutoSize = true;
            this.checkBoxSelectAllFields.Checked = true;
            this.checkBoxSelectAllFields.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelectAllFields.Location = new System.Drawing.Point(221, 3);
            this.checkBoxSelectAllFields.Name = "checkBoxSelectAllFields";
            this.checkBoxSelectAllFields.Size = new System.Drawing.Size(82, 17);
            this.checkBoxSelectAllFields.TabIndex = 5;
            this.checkBoxSelectAllFields.Text = "&Deselect All";
            this.checkBoxSelectAllFields.ThreeState = true;
            this.checkBoxSelectAllFields.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllFields.CheckStateChanged += new System.EventHandler(this.checkBoxSelectAllFields_CheckStateChanged);
            // 
            // checkedListBoxStories
            // 
            this.checkedListBoxStories.CheckOnClick = true;
            this.checkedListBoxStories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxStories.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxStories.FormattingEnabled = true;
            this.checkedListBoxStories.Location = new System.Drawing.Point(3, 26);
            this.checkedListBoxStories.Name = "checkedListBoxStories";
            this.checkedListBoxStories.Size = new System.Drawing.Size(212, 328);
            this.checkedListBoxStories.TabIndex = 0;
            // 
            // groupBoxViewOptions
            // 
            this.groupBoxViewOptions.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxViewOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxViewOptions.Location = new System.Drawing.Point(221, 26);
            this.groupBoxViewOptions.Name = "groupBoxViewOptions";
            this.groupBoxViewOptions.Size = new System.Drawing.Size(447, 328);
            this.groupBoxViewOptions.TabIndex = 4;
            this.groupBoxViewOptions.TabStop = false;
            this.groupBoxViewOptions.Text = "Include in report";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBoxShowLineNumbers);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxFrontMatter);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangVernacular);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateVernacular);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangNationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateNationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangInternationalBT);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateInternationalBt);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangFreeTranslation);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLangTransliterateFreeTranslation);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxAnchors);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxExegeticalHelpNote);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxRetellings);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxGeneralTestingQuestions);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxStoryTestingQuestions);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxAnswers);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxShowHidden);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxConsultantComments);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxCoachComments);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(441, 309);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // checkBoxShowLineNumbers
            // 
            this.checkBoxShowLineNumbers.AutoSize = true;
            this.checkBoxShowLineNumbers.Checked = true;
            this.checkBoxShowLineNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowLineNumbers.Location = new System.Drawing.Point(3, 3);
            this.checkBoxShowLineNumbers.Name = "checkBoxShowLineNumbers";
            this.checkBoxShowLineNumbers.Size = new System.Drawing.Size(91, 17);
            this.checkBoxShowLineNumbers.TabIndex = 0;
            this.checkBoxShowLineNumbers.Text = "Line Numbers";
            this.checkBoxShowLineNumbers.UseVisualStyleBackColor = true;
            // 
            // checkBoxFrontMatter
            // 
            this.checkBoxFrontMatter.AutoSize = true;
            this.checkBoxFrontMatter.Checked = true;
            this.checkBoxFrontMatter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFrontMatter.Location = new System.Drawing.Point(3, 26);
            this.checkBoxFrontMatter.Name = "checkBoxFrontMatter";
            this.checkBoxFrontMatter.Size = new System.Drawing.Size(143, 17);
            this.checkBoxFrontMatter.TabIndex = 1;
            this.checkBoxFrontMatter.Text = "Story Header Information";
            this.checkBoxFrontMatter.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangVernacular
            // 
            this.checkBoxLangVernacular.AutoSize = true;
            this.checkBoxLangVernacular.Checked = true;
            this.checkBoxLangVernacular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangVernacular.Location = new System.Drawing.Point(3, 49);
            this.checkBoxLangVernacular.Name = "checkBoxLangVernacular";
            this.checkBoxLangVernacular.Size = new System.Drawing.Size(205, 17);
            this.checkBoxLangVernacular.TabIndex = 2;
            this.checkBoxLangVernacular.Text = "LangVernacular <no need to localize>";
            this.checkBoxLangVernacular.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateVernacular
            // 
            this.checkBoxLangTransliterateVernacular.AutoSize = true;
            this.checkBoxLangTransliterateVernacular.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateVernacular.Location = new System.Drawing.Point(3, 72);
            this.checkBoxLangTransliterateVernacular.Name = "checkBoxLangTransliterateVernacular";
            this.checkBoxLangTransliterateVernacular.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateVernacular.TabIndex = 3;
            this.checkBoxLangTransliterateVernacular.Text = "Transliterate";
            this.checkBoxLangTransliterateVernacular.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateVernacular.Visible = false;
            // 
            // checkBoxLangNationalBT
            // 
            this.checkBoxLangNationalBT.AutoSize = true;
            this.checkBoxLangNationalBT.Checked = true;
            this.checkBoxLangNationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangNationalBT.Location = new System.Drawing.Point(3, 95);
            this.checkBoxLangNationalBT.Name = "checkBoxLangNationalBT";
            this.checkBoxLangNationalBT.Size = new System.Drawing.Size(207, 17);
            this.checkBoxLangNationalBT.TabIndex = 4;
            this.checkBoxLangNationalBT.Text = "LangNationalBT <no need to localize>";
            this.checkBoxLangNationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateNationalBT
            // 
            this.checkBoxLangTransliterateNationalBT.AutoSize = true;
            this.checkBoxLangTransliterateNationalBT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateNationalBT.Location = new System.Drawing.Point(3, 118);
            this.checkBoxLangTransliterateNationalBT.Name = "checkBoxLangTransliterateNationalBT";
            this.checkBoxLangTransliterateNationalBT.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateNationalBT.TabIndex = 5;
            this.checkBoxLangTransliterateNationalBT.Text = "Transliterate";
            this.checkBoxLangTransliterateNationalBT.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateNationalBT.Visible = false;
            // 
            // checkBoxLangInternationalBT
            // 
            this.checkBoxLangInternationalBT.AutoSize = true;
            this.checkBoxLangInternationalBT.Checked = true;
            this.checkBoxLangInternationalBT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangInternationalBT.Location = new System.Drawing.Point(3, 141);
            this.checkBoxLangInternationalBT.Name = "checkBoxLangInternationalBT";
            this.checkBoxLangInternationalBT.Size = new System.Drawing.Size(165, 17);
            this.checkBoxLangInternationalBT.TabIndex = 6;
            this.checkBoxLangInternationalBT.Text = "&English back translation fields";
            this.checkBoxLangInternationalBT.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateInternationalBt
            // 
            this.checkBoxLangTransliterateInternationalBt.AutoSize = true;
            this.checkBoxLangTransliterateInternationalBt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateInternationalBt.Location = new System.Drawing.Point(3, 164);
            this.checkBoxLangTransliterateInternationalBt.Name = "checkBoxLangTransliterateInternationalBt";
            this.checkBoxLangTransliterateInternationalBt.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateInternationalBt.TabIndex = 7;
            this.checkBoxLangTransliterateInternationalBt.Text = "Transliterate";
            this.checkBoxLangTransliterateInternationalBt.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateInternationalBt.Visible = false;
            // 
            // checkBoxLangFreeTranslation
            // 
            this.checkBoxLangFreeTranslation.AutoSize = true;
            this.checkBoxLangFreeTranslation.Checked = true;
            this.checkBoxLangFreeTranslation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLangFreeTranslation.Location = new System.Drawing.Point(3, 187);
            this.checkBoxLangFreeTranslation.Name = "checkBoxLangFreeTranslation";
            this.checkBoxLangFreeTranslation.Size = new System.Drawing.Size(125, 17);
            this.checkBoxLangFreeTranslation.TabIndex = 8;
            this.checkBoxLangFreeTranslation.Text = "&Free translation fields";
            this.checkBoxLangFreeTranslation.UseVisualStyleBackColor = true;
            // 
            // checkBoxLangTransliterateFreeTranslation
            // 
            this.checkBoxLangTransliterateFreeTranslation.AutoSize = true;
            this.checkBoxLangTransliterateFreeTranslation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLangTransliterateFreeTranslation.Location = new System.Drawing.Point(3, 210);
            this.checkBoxLangTransliterateFreeTranslation.Name = "checkBoxLangTransliterateFreeTranslation";
            this.checkBoxLangTransliterateFreeTranslation.Size = new System.Drawing.Size(84, 17);
            this.checkBoxLangTransliterateFreeTranslation.TabIndex = 9;
            this.checkBoxLangTransliterateFreeTranslation.Text = "Transliterate";
            this.checkBoxLangTransliterateFreeTranslation.UseVisualStyleBackColor = true;
            this.checkBoxLangTransliterateFreeTranslation.Visible = false;
            // 
            // checkBoxAnchors
            // 
            this.checkBoxAnchors.AutoSize = true;
            this.checkBoxAnchors.Checked = true;
            this.checkBoxAnchors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnchors.Location = new System.Drawing.Point(3, 233);
            this.checkBoxAnchors.Name = "checkBoxAnchors";
            this.checkBoxAnchors.Size = new System.Drawing.Size(65, 17);
            this.checkBoxAnchors.TabIndex = 10;
            this.checkBoxAnchors.Text = "&Anchors";
            this.checkBoxAnchors.UseVisualStyleBackColor = true;
            // 
            // checkBoxExegeticalHelpNote
            // 
            this.checkBoxExegeticalHelpNote.AutoSize = true;
            this.checkBoxExegeticalHelpNote.Checked = true;
            this.checkBoxExegeticalHelpNote.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExegeticalHelpNote.Location = new System.Drawing.Point(3, 256);
            this.checkBoxExegeticalHelpNote.Name = "checkBoxExegeticalHelpNote";
            this.checkBoxExegeticalHelpNote.Size = new System.Drawing.Size(143, 17);
            this.checkBoxExegeticalHelpNote.TabIndex = 11;
            this.checkBoxExegeticalHelpNote.Text = "&Exegetical/cultural notes";
            this.checkBoxExegeticalHelpNote.UseVisualStyleBackColor = true;
            // 
            // checkBoxRetellings
            // 
            this.checkBoxRetellings.AutoSize = true;
            this.checkBoxRetellings.Checked = true;
            this.checkBoxRetellings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRetellings.Location = new System.Drawing.Point(3, 279);
            this.checkBoxRetellings.Name = "checkBoxRetellings";
            this.checkBoxRetellings.Size = new System.Drawing.Size(72, 17);
            this.checkBoxRetellings.TabIndex = 12;
            this.checkBoxRetellings.Text = "&Retellings";
            this.checkBoxRetellings.UseVisualStyleBackColor = true;
            // 
            // checkBoxGeneralTestingQuestions
            // 
            this.checkBoxGeneralTestingQuestions.AutoSize = true;
            this.checkBoxGeneralTestingQuestions.Checked = true;
            this.checkBoxGeneralTestingQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGeneralTestingQuestions.Location = new System.Drawing.Point(216, 3);
            this.checkBoxGeneralTestingQuestions.Name = "checkBoxGeneralTestingQuestions";
            this.checkBoxGeneralTestingQuestions.Size = new System.Drawing.Size(145, 17);
            this.checkBoxGeneralTestingQuestions.TabIndex = 13;
            this.checkBoxGeneralTestingQuestions.Text = "&General testing questions";
            this.checkBoxGeneralTestingQuestions.UseVisualStyleBackColor = true;
            // 
            // checkBoxStoryTestingQuestions
            // 
            this.checkBoxStoryTestingQuestions.AutoSize = true;
            this.checkBoxStoryTestingQuestions.Checked = true;
            this.checkBoxStoryTestingQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStoryTestingQuestions.Location = new System.Drawing.Point(216, 26);
            this.checkBoxStoryTestingQuestions.Name = "checkBoxStoryTestingQuestions";
            this.checkBoxStoryTestingQuestions.Size = new System.Drawing.Size(132, 17);
            this.checkBoxStoryTestingQuestions.TabIndex = 14;
            this.checkBoxStoryTestingQuestions.Text = "Story &testing questions";
            this.checkBoxStoryTestingQuestions.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnswers
            // 
            this.checkBoxAnswers.AutoSize = true;
            this.checkBoxAnswers.Checked = true;
            this.checkBoxAnswers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnswers.Location = new System.Drawing.Point(216, 49);
            this.checkBoxAnswers.Name = "checkBoxAnswers";
            this.checkBoxAnswers.Size = new System.Drawing.Size(155, 17);
            this.checkBoxAnswers.TabIndex = 15;
            this.checkBoxAnswers.Text = "Story test question &answers";
            this.checkBoxAnswers.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowHidden
            // 
            this.checkBoxShowHidden.AutoSize = true;
            this.checkBoxShowHidden.Location = new System.Drawing.Point(216, 72);
            this.checkBoxShowHidden.Name = "checkBoxShowHidden";
            this.checkBoxShowHidden.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowHidden.TabIndex = 11;
            this.checkBoxShowHidden.Text = "Show &hidden lines";
            this.checkBoxShowHidden.UseVisualStyleBackColor = true;
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Checked = true;
            this.checkBoxSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(3, 3);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(82, 17);
            this.checkBoxSelectAll.TabIndex = 3;
            this.checkBoxSelectAll.Text = "&Deselect All";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.CheckStateChanged += new System.EventHandler(this.checkBoxSelectAll_CheckStateChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPagePrintPreviewConfig);
            this.tabControl.Controls.Add(this.tabPagePrintPreview);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(685, 389);
            this.tabControl.TabIndex = 1;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // tabPagePrintPreviewConfig
            // 
            this.tabPagePrintPreviewConfig.Controls.Add(this.tableLayoutPanel);
            this.tabPagePrintPreviewConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPagePrintPreviewConfig.Name = "tabPagePrintPreviewConfig";
            this.tabPagePrintPreviewConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrintPreviewConfig.Size = new System.Drawing.Size(677, 363);
            this.tabPagePrintPreviewConfig.TabIndex = 0;
            this.tabPagePrintPreviewConfig.Text = "Configure";
            this.tabPagePrintPreviewConfig.ToolTipText = "Configure what you want to see in the print report";
            this.tabPagePrintPreviewConfig.UseVisualStyleBackColor = true;
            // 
            // tabPagePrintPreview
            // 
            this.tabPagePrintPreview.Controls.Add(this.printViewer);
            this.tabPagePrintPreview.Location = new System.Drawing.Point(4, 22);
            this.tabPagePrintPreview.Name = "tabPagePrintPreview";
            this.tabPagePrintPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrintPreview.Size = new System.Drawing.Size(677, 363);
            this.tabPagePrintPreview.TabIndex = 1;
            this.tabPagePrintPreview.Text = "Print Preview";
            this.tabPagePrintPreview.ToolTipText = "Click this tab to see the preview";
            this.tabPagePrintPreview.UseVisualStyleBackColor = true;
            // 
            // printViewer
            // 
            this.printViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printViewer.Location = new System.Drawing.Point(3, 3);
            this.printViewer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.printViewer.Name = "printViewer";
            this.printViewer.Size = new System.Drawing.Size(671, 357);
            this.printViewer.TabIndex = 0;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "html";
            this.saveFileDialog.Filter = "Web page|*.htm;*.html|All files|*.*";
            this.saveFileDialog.Title = "Save project in HTML format";
            // 
            // checkBoxConsultantComments
            // 
            this.checkBoxConsultantComments.AutoSize = true;
            this.checkBoxConsultantComments.Checked = true;
            this.checkBoxConsultantComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxConsultantComments.Location = new System.Drawing.Point(216, 95);
            this.checkBoxConsultantComments.Name = "checkBoxConsultantComments";
            this.checkBoxConsultantComments.Size = new System.Drawing.Size(127, 17);
            this.checkBoxConsultantComments.TabIndex = 16;
            this.checkBoxConsultantComments.Text = "&Consultant comments";
            this.checkBoxConsultantComments.UseVisualStyleBackColor = true;
            // 
            // checkBoxCoachComments
            // 
            this.checkBoxCoachComments.AutoSize = true;
            this.checkBoxCoachComments.Checked = true;
            this.checkBoxCoachComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCoachComments.Location = new System.Drawing.Point(216, 118);
            this.checkBoxCoachComments.Name = "checkBoxCoachComments";
            this.checkBoxCoachComments.Size = new System.Drawing.Size(108, 17);
            this.checkBoxCoachComments.TabIndex = 16;
            this.checkBoxCoachComments.Text = "C&oach comments";
            this.checkBoxCoachComments.UseVisualStyleBackColor = true;
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 389);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PrintForm";
            this.Text = "Print";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintForm_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.groupBoxViewOptions.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPagePrintPreviewConfig.ResumeLayout(false);
            this.tabPagePrintPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckedListBox checkedListBoxStories;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPagePrintPreviewConfig;
        private System.Windows.Forms.TabPage tabPagePrintPreview;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
        private System.Windows.Forms.GroupBox groupBoxViewOptions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxFrontMatter;
        private System.Windows.Forms.CheckBox checkBoxLangVernacular;
        private System.Windows.Forms.CheckBox checkBoxLangNationalBT;
        private System.Windows.Forms.CheckBox checkBoxLangInternationalBT;
        private System.Windows.Forms.CheckBox checkBoxAnchors;
        private System.Windows.Forms.CheckBox checkBoxStoryTestingQuestions;
        private System.Windows.Forms.CheckBox checkBoxAnswers;
        private System.Windows.Forms.CheckBox checkBoxRetellings;
        private System.Windows.Forms.CheckBox checkBoxSelectAllFields;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateVernacular;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateNationalBT;
        private System.Windows.Forms.CheckBox checkBoxShowHidden;
        private System.Windows.Forms.CheckBox checkBoxLangFreeTranslation;
        private System.Windows.Forms.CheckBox checkBoxGeneralTestingQuestions;
        private System.Windows.Forms.CheckBox checkBoxExegeticalHelpNote;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateInternationalBt;
        private System.Windows.Forms.CheckBox checkBoxLangTransliterateFreeTranslation;
        private PrintViewer printViewer;
        private System.Windows.Forms.CheckBox checkBoxShowLineNumbers;
        private System.Windows.Forms.CheckBox checkBoxConsultantComments;
        private System.Windows.Forms.CheckBox checkBoxCoachComments;
    }
}