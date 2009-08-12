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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanoramaView));
            this.dataGridViewPanorama = new System.Windows.Forms.DataGridView();
            this.contextMenuStripProjectStages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.StoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPurpose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StoryStage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).BeginInit();
            this.contextMenuStripProjectStages.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewPanorama
            // 
            this.dataGridViewPanorama.AllowUserToAddRows = false;
            this.dataGridViewPanorama.AllowUserToOrderColumns = true;
            this.dataGridViewPanorama.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPanorama.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StoryName,
            this.ColumnPurpose,
            this.EditToken,
            this.StoryStage});
            this.dataGridViewPanorama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPanorama.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPanorama.Name = "dataGridViewPanorama";
            this.dataGridViewPanorama.Size = new System.Drawing.Size(709, 362);
            this.dataGridViewPanorama.TabIndex = 0;
            this.dataGridViewPanorama.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPanorama_CellMouseDown);
            // 
            // contextMenuStripProjectStages
            // 
            this.contextMenuStripProjectStages.Name = "contextMenuStripProjectStages";
            this.contextMenuStripProjectStages.Size = new System.Drawing.Size(462, 246);
            this.contextMenuStripProjectStages.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripProjectStages_Opening);
            // 
            // StoryName
            // 
            this.StoryName.Frozen = true;
            this.StoryName.HeaderText = "Story Name";
            this.StoryName.Name = "StoryName";
            this.StoryName.ReadOnly = true;
            // 
            // ColumnPurpose
            // 
            this.ColumnPurpose.HeaderText = "Purpose";
            this.ColumnPurpose.Name = "ColumnPurpose";
            this.ColumnPurpose.Width = 200;
            // 
            // EditToken
            // 
            this.EditToken.HeaderText = "Who Edits";
            this.EditToken.Name = "EditToken";
            this.EditToken.ReadOnly = true;
            // 
            // StoryStage
            // 
            this.StoryStage.ContextMenuStrip = this.contextMenuStripProjectStages;
            this.StoryStage.HeaderText = "Stage";
            this.StoryStage.Name = "StoryStage";
            this.StoryStage.ReadOnly = true;
            this.StoryStage.ToolTipText = "Right-click to change state";
            this.StoryStage.Width = 300;
            // 
            // PanoramaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 362);
            this.Controls.Add(this.dataGridViewPanorama);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanoramaView";
            this.Text = "Panorama View";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).EndInit();
            this.contextMenuStripProjectStages.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPanorama;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripProjectStages;
        private System.Windows.Forms.DataGridViewTextBoxColumn StoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPurpose;
        private System.Windows.Forms.DataGridViewTextBoxColumn EditToken;
        private System.Windows.Forms.DataGridViewTextBoxColumn StoryStage;
    }
}