namespace OneStoryProjectEditor
{
    partial class StageEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StageEditorForm));
            this.dataGridViewStates = new System.Windows.Forms.DataGridView();
            this.ColumnProjectFacilitatorStages = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnEnglishBackTranslator = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnFirstPassMentor = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnConsultantInTraining = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnCoach = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxProjectFacilitator = new System.Windows.Forms.CheckBox();
            this.checkBoxOutsideEnglishBackTranslator = new System.Windows.Forms.CheckBox();
            this.checkBoxFirstPassMentor = new System.Windows.Forms.CheckBox();
            this.radioButtonIndependentConsultant = new System.Windows.Forms.RadioButton();
            this.radioButtonManageWithCoaching = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStates)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewStates
            // 
            this.dataGridViewStates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewStates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnProjectFacilitatorStages,
            this.ColumnEnglishBackTranslator,
            this.ColumnFirstPassMentor,
            this.ColumnConsultantInTraining,
            this.ColumnCoach});
            this.dataGridViewStates.Location = new System.Drawing.Point(13, 36);
            this.dataGridViewStates.Name = "dataGridViewStates";
            this.dataGridViewStates.RowHeadersVisible = false;
            this.dataGridViewStates.Size = new System.Drawing.Size(932, 295);
            this.dataGridViewStates.TabIndex = 5;
            this.dataGridViewStates.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStates_CellValueChanged);
            this.dataGridViewStates.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStates_CellEndEdit);
            this.dataGridViewStates.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStates_CellContentClick);
            // 
            // ColumnProjectFacilitatorStages
            // 
            this.ColumnProjectFacilitatorStages.HeaderText = "Project Facilitator";
            this.ColumnProjectFacilitatorStages.Name = "ColumnProjectFacilitatorStages";
            // 
            // ColumnEnglishBackTranslator
            // 
            this.ColumnEnglishBackTranslator.HeaderText = "English Back-translator";
            this.ColumnEnglishBackTranslator.Name = "ColumnEnglishBackTranslator";
            // 
            // ColumnFirstPassMentor
            // 
            this.ColumnFirstPassMentor.HeaderText = "First Pass Mentor";
            this.ColumnFirstPassMentor.Name = "ColumnFirstPassMentor";
            // 
            // ColumnConsultantInTraining
            // 
            this.ColumnConsultantInTraining.HeaderText = "Consultant-in-Training";
            this.ColumnConsultantInTraining.Name = "ColumnConsultantInTraining";
            // 
            // ColumnCoach
            // 
            this.ColumnCoach.HeaderText = "Coach";
            this.ColumnCoach.Name = "ColumnCoach";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSave.Location = new System.Drawing.Point(400, 337);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "&Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Visible = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(481, 337);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxProjectFacilitator
            // 
            this.checkBoxProjectFacilitator.AutoSize = true;
            this.checkBoxProjectFacilitator.Checked = true;
            this.checkBoxProjectFacilitator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProjectFacilitator.Enabled = false;
            this.checkBoxProjectFacilitator.Location = new System.Drawing.Point(44, 11);
            this.checkBoxProjectFacilitator.Name = "checkBoxProjectFacilitator";
            this.checkBoxProjectFacilitator.Size = new System.Drawing.Size(107, 17);
            this.checkBoxProjectFacilitator.TabIndex = 0;
            this.checkBoxProjectFacilitator.Text = "Project Facilitator";
            this.checkBoxProjectFacilitator.UseVisualStyleBackColor = true;
            // 
            // checkBoxOutsideEnglishBackTranslator
            // 
            this.checkBoxOutsideEnglishBackTranslator.AutoSize = true;
            this.checkBoxOutsideEnglishBackTranslator.Location = new System.Drawing.Point(188, 11);
            this.checkBoxOutsideEnglishBackTranslator.Name = "checkBoxOutsideEnglishBackTranslator";
            this.checkBoxOutsideEnglishBackTranslator.Size = new System.Drawing.Size(172, 17);
            this.checkBoxOutsideEnglishBackTranslator.TabIndex = 1;
            this.checkBoxOutsideEnglishBackTranslator.Text = "&Outside English back-translator";
            this.checkBoxOutsideEnglishBackTranslator.UseVisualStyleBackColor = true;
            this.checkBoxOutsideEnglishBackTranslator.CheckedChanged += new System.EventHandler(this.checkBoxOutsideEnglishBackTranslator_CheckedChanged);
            // 
            // checkBoxFirstPassMentor
            // 
            this.checkBoxFirstPassMentor.AutoSize = true;
            this.checkBoxFirstPassMentor.Location = new System.Drawing.Point(397, 11);
            this.checkBoxFirstPassMentor.Name = "checkBoxFirstPassMentor";
            this.checkBoxFirstPassMentor.Size = new System.Drawing.Size(106, 17);
            this.checkBoxFirstPassMentor.TabIndex = 2;
            this.checkBoxFirstPassMentor.Text = "&First-pass Mentor";
            this.checkBoxFirstPassMentor.UseVisualStyleBackColor = true;
            this.checkBoxFirstPassMentor.CheckedChanged += new System.EventHandler(this.checkBoxFirstPassMentor_CheckedChanged);
            // 
            // radioButtonIndependentConsultant
            // 
            this.radioButtonIndependentConsultant.AutoSize = true;
            this.radioButtonIndependentConsultant.Location = new System.Drawing.Point(774, 11);
            this.radioButtonIndependentConsultant.Name = "radioButtonIndependentConsultant";
            this.radioButtonIndependentConsultant.Size = new System.Drawing.Size(138, 17);
            this.radioButtonIndependentConsultant.TabIndex = 4;
            this.radioButtonIndependentConsultant.Text = "&Independent Consultant";
            this.radioButtonIndependentConsultant.UseVisualStyleBackColor = true;
            // 
            // radioButtonManageWithCoaching
            // 
            this.radioButtonManageWithCoaching.AutoSize = true;
            this.radioButtonManageWithCoaching.Checked = true;
            this.radioButtonManageWithCoaching.Location = new System.Drawing.Point(540, 11);
            this.radioButtonManageWithCoaching.Name = "radioButtonManageWithCoaching";
            this.radioButtonManageWithCoaching.Size = new System.Drawing.Size(197, 17);
            this.radioButtonManageWithCoaching.TabIndex = 3;
            this.radioButtonManageWithCoaching.TabStop = true;
            this.radioButtonManageWithCoaching.Text = "&Consultant-in-Training with Coaching";
            this.radioButtonManageWithCoaching.UseVisualStyleBackColor = true;
            this.radioButtonManageWithCoaching.CheckedChanged += new System.EventHandler(this.radioButtonManageWithCoaching_CheckedChanged);
            // 
            // StageEditorForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(957, 372);
            this.Controls.Add(this.radioButtonManageWithCoaching);
            this.Controls.Add(this.radioButtonIndependentConsultant);
            this.Controls.Add(this.checkBoxFirstPassMentor);
            this.Controls.Add(this.checkBoxOutsideEnglishBackTranslator);
            this.Controls.Add(this.checkBoxProjectFacilitator);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.dataGridViewStates);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StageEditorForm";
            this.Text = "OneStory Process State Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStates)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewStates;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxProjectFacilitator;
        private System.Windows.Forms.CheckBox checkBoxOutsideEnglishBackTranslator;
        private System.Windows.Forms.CheckBox checkBoxFirstPassMentor;
        private System.Windows.Forms.RadioButton radioButtonIndependentConsultant;
        private System.Windows.Forms.RadioButton radioButtonManageWithCoaching;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnProjectFacilitatorStages;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnEnglishBackTranslator;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnFirstPassMentor;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnConsultantInTraining;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnCoach;
    }
}