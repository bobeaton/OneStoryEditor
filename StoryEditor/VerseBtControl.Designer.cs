namespace StoryEditor
{
    partial class VerseBtControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerseBtControl));
            this.labelReference = new System.Windows.Forms.Label();
            this.tableLayoutPanelVerse = new DynamicTableLayoutPanel();
            this.labelAnchor = new System.Windows.Forms.Label();
            this.toolStripAnchors = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNewAnchor = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanelVerse.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.tableLayoutPanelVerse.SetColumnSpan(this.labelReference, 2);
            this.labelReference.Location = new System.Drawing.Point(3, 0);
            this.labelReference.Name = "labelReference";
            this.labelReference.Size = new System.Drawing.Size(79, 13);
            this.labelReference.TabIndex = 0;
            this.labelReference.Text = "labelReference";
            // 
            // tableLayoutPanelVerse
            // 
            this.tableLayoutPanelVerse.ColumnCount = 2;
            this.tableLayoutPanelVerse.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelVerse.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelVerse.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanelVerse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelVerse.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelVerse.Name = "tableLayoutPanelVerse";
            this.tableLayoutPanelVerse.RowCount = 1;
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.Size = new System.Drawing.Size(669, 225);
            this.tableLayoutPanelVerse.TabIndex = 1;
            // 
            // labelAnchor
            // 
            this.labelAnchor.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            this.labelAnchor.AutoSize = true;
            this.labelAnchor.Location = new System.Drawing.Point(3, 112);
            this.labelAnchor.Name = "labelAnchor";
            this.labelAnchor.Size = new System.Drawing.Size(28, 13);
            this.labelAnchor.TabIndex = 7;
            this.labelAnchor.Text = "anc:";
            // 
            // toolStripAnchors
            // 
            this.toolStripAnchors.Location = new System.Drawing.Point(34, 13);
            this.toolStripAnchors.Name = "toolStripAnchors";
            this.toolStripAnchors.Size = new System.Drawing.Size(635, 25);
            this.toolStripAnchors.TabIndex = 8;
            this.toolStripAnchors.Text = "toolStripAnchors";
            // 
            // toolStripButtonNewAnchor
            // 
            this.toolStripButtonNewAnchor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNewAnchor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNewAnchor.Image")));
            this.toolStripButtonNewAnchor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewAnchor.Name = "toolStripButtonNewAnchor";
            this.toolStripButtonNewAnchor.Size = new System.Drawing.Size(61, 22);
            this.toolStripButtonNewAnchor.Text = "Add New";
            this.toolStripButtonNewAnchor.ToolTipText = "Click to add a new anchor";
            // 
            // VerseBtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelVerse);
            this.Name = "VerseBtControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.tableLayoutPanelVerse.ResumeLayout(false);
            this.tableLayoutPanelVerse.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelReference;
        private DynamicTableLayoutPanel tableLayoutPanelVerse;
        private System.Windows.Forms.Label labelAnchor;
        private System.Windows.Forms.ToolStrip toolStripAnchors;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewAnchor;
    }
}
