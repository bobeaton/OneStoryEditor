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
            this.StoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPurpose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Stage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EditToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripProjectStages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
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
            this.Stage,
            this.EditToken});
            this.dataGridViewPanorama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPanorama.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPanorama.Name = "dataGridViewPanorama";
            this.dataGridViewPanorama.Size = new System.Drawing.Size(709, 362);
            this.dataGridViewPanorama.TabIndex = 0;
            this.dataGridViewPanorama.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPanorama_CellMouseDown);
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
            // 
            // Stage
            // 
            this.Stage.HeaderText = "Stage";
            this.Stage.Items.AddRange(new object[] {
            "Crafter enters the {0} back-translation",
            "Crafter enters the English back-translation",
            "Crafter adds biblical anchors",
            "Crafter adds story testing questions",
            "Consultant adds round 1 exegetical notes",
            "Coach reviews round 1 notes",
            "Consultant revises round 1 notes based on Coach\'s feedback",
            "Crafter revises story based on round 1 notes and enters round 1 responses",
            "Crafter has 1st online review with consultant",
            "Crafter enters test 1 retelling back-translation",
            "Crafter enters test 1 answers to story questions",
            "Consultant adds round 2 notes",
            "Coach reviews round 2 notes",
            "Consultant revises round 2 notes based on Coach\'s feedback",
            "Crafter revises story based on round 2 notes and enters round 2 responses",
            "Crafter has 2nd online review with consultant",
            "Crafter enters test 2 retelling back-translation",
            "Crafter enters test 2 answers to story questions",
            "Story testing complete (waiting for panorama completion review)"});
            this.Stage.Name = "Stage";
            this.Stage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Stage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // EditToken
            // 
            this.EditToken.HeaderText = "Who Edits";
            this.EditToken.Name = "EditToken";
            this.EditToken.ReadOnly = true;
            // 
            // contextMenuStripProjectStages
            // 
            this.contextMenuStripProjectStages.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11});
            this.contextMenuStripProjectStages.Name = "contextMenuStripProjectStages";
            this.contextMenuStripProjectStages.Size = new System.Drawing.Size(462, 246);
            this.contextMenuStripProjectStages.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripProjectStages_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem1.Text = "Crafter enters the {0} back-translation";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem2.Text = "Crafter enters the English back-translation";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem3.Text = "Crafter adds biblical anchors";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem4.Text = "Crafter adds story testing questions";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem5.Text = "Consultant adds round 1 exegetical notes";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem6.Text = "Coach reviews round 1 notes";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem7.Text = "Consultant revises round 1 notes based on Coach\'s feedback";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem8.Text = "Crafter revises story based on round 1 notes and enters round 1 responses";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem9.Text = "Crafter has 1st online review with consultant";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem10.Text = "Crafter enters test 1 retelling back-translation";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(461, 22);
            this.toolStripMenuItem11.Text = "Crafter enters test 1 answers to story questions";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn StoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPurpose;
        private System.Windows.Forms.DataGridViewComboBoxColumn Stage;
        private System.Windows.Forms.DataGridViewTextBoxColumn EditToken;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripProjectStages;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
    }
}