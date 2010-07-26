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
            this.ColumnProjectFacilitatorStages = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnEnglishBackTranslator = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnFirstPassMentor = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnConsultantInTraining = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnCoach = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStates)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewStates
            // 
            this.dataGridViewStates.AllowUserToAddRows = false;
            this.dataGridViewStates.AllowUserToDeleteRows = false;
            this.dataGridViewStates.AllowUserToResizeRows = false;
            this.dataGridViewStates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewStates.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnProjectFacilitatorStages,
            this.ColumnEnglishBackTranslator,
            this.ColumnFirstPassMentor,
            this.ColumnConsultantInTraining,
            this.ColumnCoach});
            this.dataGridViewStates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewStates.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewStates.MultiSelect = false;
            this.dataGridViewStates.Name = "dataGridViewStates";
            this.dataGridViewStates.RowHeadersVisible = false;
            this.dataGridViewStates.Size = new System.Drawing.Size(867, 326);
            this.dataGridViewStates.TabIndex = 1;
            this.dataGridViewStates.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStates_CellContentClick);
            // 
            // ColumnProjectFacilitatorStages
            // 
            this.ColumnProjectFacilitatorStages.HeaderText = "Project Facilitator";
            this.ColumnProjectFacilitatorStages.Name = "ColumnProjectFacilitatorStages";
            this.ColumnProjectFacilitatorStages.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnEnglishBackTranslator
            // 
            this.ColumnEnglishBackTranslator.HeaderText = "English Back-translator";
            this.ColumnEnglishBackTranslator.Name = "ColumnEnglishBackTranslator";
            this.ColumnEnglishBackTranslator.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnFirstPassMentor
            // 
            this.ColumnFirstPassMentor.HeaderText = "First Pass Mentor";
            this.ColumnFirstPassMentor.Name = "ColumnFirstPassMentor";
            this.ColumnFirstPassMentor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnConsultantInTraining
            // 
            this.ColumnConsultantInTraining.HeaderText = "Consultant-in-Training";
            this.ColumnConsultantInTraining.Name = "ColumnConsultantInTraining";
            this.ColumnConsultantInTraining.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnCoach
            // 
            this.ColumnCoach.HeaderText = "Coach";
            this.ColumnCoach.Name = "ColumnCoach";
            this.ColumnCoach.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // StageEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 326);
            this.Controls.Add(this.dataGridViewStates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StageEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "To go forward, click a green state button. To go backwards (for revisions), click" +
                " a red state button";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStates)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewStates;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnProjectFacilitatorStages;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnEnglishBackTranslator;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnFirstPassMentor;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnConsultantInTraining;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnCoach;
    }
}