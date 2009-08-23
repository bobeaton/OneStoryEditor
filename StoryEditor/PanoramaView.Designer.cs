namespace OneStoryProjectEditor
{
    partial class PanoramaView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanoramaView));
            this.dataGridViewPanorama = new System.Windows.Forms.DataGridView();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.StoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPurpose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StoryStage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewPanorama
            // 
            this.dataGridViewPanorama.AllowUserToAddRows = false;
            this.dataGridViewPanorama.AllowUserToOrderColumns = true;
            this.dataGridViewPanorama.AllowUserToResizeRows = false;
            this.dataGridViewPanorama.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridViewPanorama.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPanorama.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StoryName,
            this.ColumnPurpose,
            this.EditToken,
            this.StoryStage});
            this.dataGridViewPanorama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPanorama.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewPanorama.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPanorama.MultiSelect = false;
            this.dataGridViewPanorama.Name = "dataGridViewPanorama";
            this.dataGridViewPanorama.ReadOnly = true;
            this.dataGridViewPanorama.RowHeadersWidth = 25;
            this.dataGridViewPanorama.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewPanorama.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPanorama.Size = new System.Drawing.Size(709, 362);
            this.dataGridViewPanorama.TabIndex = 0;
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveDown.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_movedown1;
            this.buttonMoveDown.Location = new System.Drawing.Point(671, 199);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(25, 23);
            this.buttonMoveDown.TabIndex = 2;
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveUp.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_moveup1;
            this.buttonMoveUp.Location = new System.Drawing.Point(671, 141);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(26, 23);
            this.buttonMoveUp.TabIndex = 1;
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDelete.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.buttonDelete.Location = new System.Drawing.Point(671, 170);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(26, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // StoryName
            // 
            this.StoryName.Frozen = true;
            this.StoryName.HeaderText = "Story Name";
            this.StoryName.Name = "StoryName";
            this.StoryName.ReadOnly = true;
            this.StoryName.Width = 87;
            // 
            // ColumnPurpose
            // 
            this.ColumnPurpose.HeaderText = "Purpose";
            this.ColumnPurpose.Name = "ColumnPurpose";
            this.ColumnPurpose.ReadOnly = true;
            this.ColumnPurpose.Width = 180;
            // 
            // EditToken
            // 
            this.EditToken.HeaderText = "Who Edits";
            this.EditToken.Name = "EditToken";
            this.EditToken.ReadOnly = true;
            this.EditToken.Width = 81;
            // 
            // StoryStage
            // 
            this.StoryStage.HeaderText = "Stage";
            this.StoryStage.Name = "StoryStage";
            this.StoryStage.ReadOnly = true;
            this.StoryStage.ToolTipText = "Right-click to change state";
            this.StoryStage.Width = 290;
            // 
            // PanoramaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 362);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonMoveDown);
            this.Controls.Add(this.buttonMoveUp);
            this.Controls.Add(this.dataGridViewPanorama);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanoramaView";
            this.Text = "Panorama View";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPanorama;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn StoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPurpose;
        private System.Windows.Forms.DataGridViewTextBoxColumn EditToken;
        private System.Windows.Forms.DataGridViewTextBoxColumn StoryStage;
    }
}