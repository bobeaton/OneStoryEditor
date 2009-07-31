namespace OneStoryProjectEditor
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
            this.tableLayoutPanelAnchor = new OneStoryProjectEditor.DynamicTableLayoutPanel();
            this.labelAnchor = new System.Windows.Forms.Label();
            this.toolStripAnchors = new System.Windows.Forms.ToolStrip();
            this.labelExegeticalHelp = new System.Windows.Forms.Label();
            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelAnchor
            // 
            this.tableLayoutPanelAnchor.ColumnCount = 2;
            this.tableLayoutPanelAnchor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAnchor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAnchor.Controls.Add(this.labelAnchor, 0, 0);
            this.tableLayoutPanelAnchor.Controls.Add(this.toolStripAnchors, 1, 0);
            this.tableLayoutPanelAnchor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAnchor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelAnchor.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelAnchor.Name = "tableLayoutPanelAnchor";
            this.tableLayoutPanelAnchor.RowCount = 1;
            this.tableLayoutPanelAnchor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAnchor.Size = new System.Drawing.Size(706, 71);
            this.tableLayoutPanelAnchor.TabIndex = 0;
            // 
            // labelAnchor
            // 
            this.labelAnchor.AutoSize = true;
            this.labelAnchor.Location = new System.Drawing.Point(3, 0);
            this.labelAnchor.Name = "labelAnchor";
            this.labelAnchor.Size = new System.Drawing.Size(28, 13);
            this.labelAnchor.TabIndex = 8;
            this.labelAnchor.Text = "anc:";
            // 
            // toolStripAnchors
            // 
            this.toolStripAnchors.Location = new System.Drawing.Point(34, 0);
            this.toolStripAnchors.Name = "toolStripAnchors";
            this.toolStripAnchors.Size = new System.Drawing.Size(672, 25);
            this.toolStripAnchors.TabIndex = 0;
            this.toolStripAnchors.Text = "toolStrip1";
            // 
            // labelExegeticalHelp
            // 
            this.labelExegeticalHelp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelExegeticalHelp.AutoSize = true;
            this.labelExegeticalHelp.Location = new System.Drawing.Point(4, 42);
            this.labelExegeticalHelp.Name = "labelExegeticalHelp";
            this.labelExegeticalHelp.Size = new System.Drawing.Size(22, 13);
            this.labelExegeticalHelp.TabIndex = 9;
            this.labelExegeticalHelp.Text = "cn:";
            // 
            // AnchorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelAnchor);
            this.Name = "AnchorControl";
            this.Size = new System.Drawing.Size(706, 71);
            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OneStoryProjectEditor.DynamicTableLayoutPanel tableLayoutPanelAnchor;
        private System.Windows.Forms.ToolStrip toolStripAnchors;
        private System.Windows.Forms.Label labelAnchor;
        private System.Windows.Forms.Label labelExegeticalHelp;
    }
}
