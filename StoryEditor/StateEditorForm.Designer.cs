namespace OneStoryProjectEditor
{
    partial class StateEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateEditorForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxStateName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxInstructions = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelViews = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxVernacular = new System.Windows.Forms.CheckBox();
            this.checkBoxNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxEnglishBT = new System.Windows.Forms.CheckBox();
            this.checkBoxAnchors = new System.Windows.Forms.CheckBox();
            this.checkBoxStoryTestingQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxStoryTestingQuestionAnswers = new System.Windows.Forms.CheckBox();
            this.checkBoxRetelling = new System.Windows.Forms.CheckBox();
            this.checkBoxConsultantNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxCoachNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxBiblePane = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTransitionDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.flowLayoutPanelViews.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxStateName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxInstructions, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelViews, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxTransitionDescription, 1, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(638, 399);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(381, 373);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "State &Description:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(300, 373);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxStateName
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxStateName, 2);
            this.textBoxStateName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxStateName.Font = new System.Drawing.Font("Arial Unicode MS", 12F);
            this.textBoxStateName.Location = new System.Drawing.Point(121, 3);
            this.textBoxStateName.Name = "textBoxStateName";
            this.textBoxStateName.Size = new System.Drawing.Size(514, 29);
            this.textBoxStateName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 70);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(70, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Instructions:";
            // 
            // textBoxInstructions
            // 
            this.textBoxInstructions.AcceptsReturn = true;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxInstructions, 2);
            this.textBoxInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInstructions.Location = new System.Drawing.Point(121, 73);
            this.textBoxInstructions.Multiline = true;
            this.textBoxInstructions.Name = "textBoxInstructions";
            this.textBoxInstructions.Size = new System.Drawing.Size(514, 192);
            this.textBoxInstructions.TabIndex = 5;
            // 
            // flowLayoutPanelViews
            // 
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanelViews, 2);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxVernacular);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxNationalBT);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxEnglishBT);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxAnchors);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxStoryTestingQuestions);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxStoryTestingQuestionAnswers);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxRetelling);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxConsultantNotes);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxCoachNotes);
            this.flowLayoutPanelViews.Controls.Add(this.checkBoxBiblePane);
            this.flowLayoutPanelViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelViews.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelViews.Location = new System.Drawing.Point(121, 271);
            this.flowLayoutPanelViews.Name = "flowLayoutPanelViews";
            this.flowLayoutPanelViews.Size = new System.Drawing.Size(514, 96);
            this.flowLayoutPanelViews.TabIndex = 6;
            // 
            // checkBoxVernacular
            // 
            this.checkBoxVernacular.AutoSize = true;
            this.checkBoxVernacular.Location = new System.Drawing.Point(3, 3);
            this.checkBoxVernacular.Name = "checkBoxVernacular";
            this.checkBoxVernacular.Size = new System.Drawing.Size(125, 17);
            this.checkBoxVernacular.TabIndex = 0;
            this.checkBoxVernacular.Text = "Show &story language";
            this.toolTip.SetToolTip(this.checkBoxVernacular, "Check this box to have the story language boxes visible when in this state");
            this.checkBoxVernacular.UseVisualStyleBackColor = true;
            this.checkBoxVernacular.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxNationalBT
            // 
            this.checkBoxNationalBT.AutoSize = true;
            this.checkBoxNationalBT.Location = new System.Drawing.Point(3, 26);
            this.checkBoxNationalBT.Name = "checkBoxNationalBT";
            this.checkBoxNationalBT.Size = new System.Drawing.Size(157, 17);
            this.checkBoxNationalBT.TabIndex = 1;
            this.checkBoxNationalBT.Text = "Show &national language BT";
            this.toolTip.SetToolTip(this.checkBoxNationalBT, "Check this box to have the national language back-translation boxes visible when " +
                    "in this state");
            this.checkBoxNationalBT.UseVisualStyleBackColor = true;
            this.checkBoxNationalBT.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxEnglishBT
            // 
            this.checkBoxEnglishBT.AutoSize = true;
            this.checkBoxEnglishBT.Location = new System.Drawing.Point(3, 49);
            this.checkBoxEnglishBT.Name = "checkBoxEnglishBT";
            this.checkBoxEnglishBT.Size = new System.Drawing.Size(107, 17);
            this.checkBoxEnglishBT.TabIndex = 2;
            this.checkBoxEnglishBT.Text = "Show &English BT";
            this.toolTip.SetToolTip(this.checkBoxEnglishBT, "Check this box to have the English language back-translation boxes visible when i" +
                    "n this state");
            this.checkBoxEnglishBT.UseVisualStyleBackColor = true;
            this.checkBoxEnglishBT.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxAnchors
            // 
            this.checkBoxAnchors.AutoSize = true;
            this.checkBoxAnchors.Location = new System.Drawing.Point(3, 72);
            this.checkBoxAnchors.Name = "checkBoxAnchors";
            this.checkBoxAnchors.Size = new System.Drawing.Size(95, 17);
            this.checkBoxAnchors.TabIndex = 3;
            this.checkBoxAnchors.Text = "Show &Anchors";
            this.toolTip.SetToolTip(this.checkBoxAnchors, "Check this box to have the Anchors visible when in this state");
            this.checkBoxAnchors.UseVisualStyleBackColor = true;
            this.checkBoxAnchors.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxStoryTestingQuestions
            // 
            this.checkBoxStoryTestingQuestions.AutoSize = true;
            this.checkBoxStoryTestingQuestions.Location = new System.Drawing.Point(166, 3);
            this.checkBoxStoryTestingQuestions.Name = "checkBoxStoryTestingQuestions";
            this.checkBoxStoryTestingQuestions.Size = new System.Drawing.Size(168, 17);
            this.checkBoxStoryTestingQuestions.TabIndex = 4;
            this.checkBoxStoryTestingQuestions.Text = "Show Story Testing &Questions";
            this.toolTip.SetToolTip(this.checkBoxStoryTestingQuestions, "Check this box to have the story testing questions visible when in th" +
                    "is state");
            this.checkBoxStoryTestingQuestions.UseVisualStyleBackColor = true;
            this.checkBoxStoryTestingQuestions.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxStoryTestingQuestionAnswers
            // 
            this.checkBoxStoryTestingQuestionAnswers.AutoSize = true;
            this.checkBoxStoryTestingQuestionAnswers.Location = new System.Drawing.Point(166, 26);
            this.checkBoxStoryTestingQuestionAnswers.Name = "checkBoxStoryTestingQuestionAnswers";
            this.checkBoxStoryTestingQuestionAnswers.Size = new System.Drawing.Size(206, 17);
            this.checkBoxStoryTestingQuestionAnswers.TabIndex = 9;
            this.checkBoxStoryTestingQuestionAnswers.Text = "Show Story Testing Question &Answers";
            this.toolTip.SetToolTip(this.checkBoxStoryTestingQuestionAnswers, "Check this box to have the story testing questions visible when in th" +
                    "is state");
            this.checkBoxStoryTestingQuestionAnswers.UseVisualStyleBackColor = true;
            this.checkBoxStoryTestingQuestionAnswers.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxRetelling
            // 
            this.checkBoxRetelling.AutoSize = true;
            this.checkBoxRetelling.Location = new System.Drawing.Point(166, 49);
            this.checkBoxRetelling.Name = "checkBoxRetelling";
            this.checkBoxRetelling.Size = new System.Drawing.Size(102, 17);
            this.checkBoxRetelling.TabIndex = 5;
            this.checkBoxRetelling.Text = "Show &Retellings";
            this.toolTip.SetToolTip(this.checkBoxRetelling, "Check this box to have the retelling boxes visible when in this state");
            this.checkBoxRetelling.UseVisualStyleBackColor = true;
            this.checkBoxRetelling.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxConsultantNotes
            // 
            this.checkBoxConsultantNotes.AutoSize = true;
            this.checkBoxConsultantNotes.Location = new System.Drawing.Point(166, 72);
            this.checkBoxConsultantNotes.Name = "checkBoxConsultantNotes";
            this.checkBoxConsultantNotes.Size = new System.Drawing.Size(137, 17);
            this.checkBoxConsultantNotes.TabIndex = 6;
            this.checkBoxConsultantNotes.Text = "Show &Consultant Notes";
            this.toolTip.SetToolTip(this.checkBoxConsultantNotes, "Check this box to have the consultant notes pane visible when in this state");
            this.checkBoxConsultantNotes.UseVisualStyleBackColor = true;
            this.checkBoxConsultantNotes.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxCoachNotes
            // 
            this.checkBoxCoachNotes.AutoSize = true;
            this.checkBoxCoachNotes.Location = new System.Drawing.Point(378, 3);
            this.checkBoxCoachNotes.Name = "checkBoxCoachNotes";
            this.checkBoxCoachNotes.Size = new System.Drawing.Size(118, 17);
            this.checkBoxCoachNotes.TabIndex = 7;
            this.checkBoxCoachNotes.Text = "Show Co&ach Notes";
            this.toolTip.SetToolTip(this.checkBoxCoachNotes, "Check this box to have the coach notes pane visible when in this state");
            this.checkBoxCoachNotes.UseVisualStyleBackColor = true;
            this.checkBoxCoachNotes.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBiblePane
            // 
            this.checkBoxBiblePane.AutoSize = true;
            this.checkBoxBiblePane.Location = new System.Drawing.Point(378, 26);
            this.checkBoxBiblePane.Name = "checkBoxBiblePane";
            this.checkBoxBiblePane.Size = new System.Drawing.Size(107, 17);
            this.checkBoxBiblePane.TabIndex = 8;
            this.checkBoxBiblePane.Text = "Show &Bible Pane";
            this.toolTip.SetToolTip(this.checkBoxBiblePane, "Check this box to have the Bible pane visible when in this state");
            this.checkBoxBiblePane.UseVisualStyleBackColor = true;
            this.checkBoxBiblePane.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "&Transition Description:";
            // 
            // textBoxTransitionDescription
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxTransitionDescription, 2);
            this.textBoxTransitionDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTransitionDescription.Font = new System.Drawing.Font("Arial Unicode MS", 12F);
            this.textBoxTransitionDescription.Location = new System.Drawing.Point(121, 38);
            this.textBoxTransitionDescription.Name = "textBoxTransitionDescription";
            this.textBoxTransitionDescription.Size = new System.Drawing.Size(514, 29);
            this.textBoxTransitionDescription.TabIndex = 3;
            // 
            // StateEditorForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(662, 423);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StateEditorForm";
            this.Text = "State Editor";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.flowLayoutPanelViews.ResumeLayout(false);
            this.flowLayoutPanelViews.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStateName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxInstructions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelViews;
        private System.Windows.Forms.CheckBox checkBoxVernacular;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkBoxNationalBT;
        private System.Windows.Forms.CheckBox checkBoxEnglishBT;
        private System.Windows.Forms.CheckBox checkBoxAnchors;
        private System.Windows.Forms.CheckBox checkBoxStoryTestingQuestions;
        private System.Windows.Forms.CheckBox checkBoxRetelling;
        private System.Windows.Forms.CheckBox checkBoxConsultantNotes;
        private System.Windows.Forms.CheckBox checkBoxCoachNotes;
        private System.Windows.Forms.CheckBox checkBoxBiblePane;
        private System.Windows.Forms.CheckBox checkBoxStoryTestingQuestionAnswers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTransitionDescription;
    }
}