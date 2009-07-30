namespace StoryEditor
{
    partial class AnchorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanelAnchor = new DynamicTableLayoutPanel();
            this.toolStripAnchors = new System.Windows.Forms.ToolStrip();
            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelAnchor
            // 
            this.tableLayoutPanelAnchor.ColumnCount = 1;
            this.tableLayoutPanelAnchor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAnchor.Controls.Add(this.toolStripAnchors, 0, 0);
            this.tableLayoutPanelAnchor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAnchor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelAnchor.Name = "tableLayoutPanelAnchor";
            this.tableLayoutPanelAnchor.RowCount = 1;
            this.tableLayoutPanelAnchor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAnchor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanelAnchor.Size = new System.Drawing.Size(706, 54);
            this.tableLayoutPanelAnchor.TabIndex = 0;
            // 
            // toolStripAnchors
            // 
            this.toolStripAnchors.Location = new System.Drawing.Point(0, 0);
            this.toolStripAnchors.Name = "toolStripAnchors";
            this.toolStripAnchors.Size = new System.Drawing.Size(706, 25);
            this.toolStripAnchors.TabIndex = 0;
            this.toolStripAnchors.Text = "toolStrip1";
            // 
            // AnchorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelAnchor);
            this.Name = "AnchorControl";
            this.Size = new System.Drawing.Size(706, 54);
            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DynamicTableLayoutPanel tableLayoutPanelAnchor;
        private System.Windows.Forms.ToolStrip toolStripAnchors;
    }
}
