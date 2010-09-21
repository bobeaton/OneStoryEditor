namespace OneStoryProjectEditor
{
    partial class TransitionHistoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransitionHistoryForm));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnTimeStamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnWindowsUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOneStoryUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFromState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnToState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTimeStamp,
            this.ColumnWindowsUsername,
            this.ColumnOneStoryUser,
            this.ColumnFromState,
            this.ColumnToState});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(809, 343);
            this.dataGridView.TabIndex = 0;
            // 
            // ColumnTimeStamp
            // 
            this.ColumnTimeStamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnTimeStamp.HeaderText = "Date & Time";
            this.ColumnTimeStamp.Name = "ColumnTimeStamp";
            this.ColumnTimeStamp.Width = 90;
            // 
            // ColumnWindowsUsername
            // 
            this.ColumnWindowsUsername.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnWindowsUsername.HeaderText = "Windows Username";
            this.ColumnWindowsUsername.Name = "ColumnWindowsUsername";
            this.ColumnWindowsUsername.Width = 116;
            // 
            // ColumnOneStoryUser
            // 
            this.ColumnOneStoryUser.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnOneStoryUser.HeaderText = "Team Member";
            this.ColumnOneStoryUser.Name = "ColumnOneStoryUser";
            this.ColumnOneStoryUser.Width = 92;
            // 
            // ColumnFromState
            // 
            this.ColumnFromState.HeaderText = "From State";
            this.ColumnFromState.Name = "ColumnFromState";
            // 
            // ColumnToState
            // 
            this.ColumnToState.HeaderText = "To State";
            this.ColumnToState.Name = "ColumnToState";
            // 
            // TransitionHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 343);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransitionHistoryForm";
            this.Text = "Transition History";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimeStamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnWindowsUsername;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOneStoryUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToState;
    }
}