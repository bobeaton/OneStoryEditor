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
            this.labelAnchor = new System.Windows.Forms.Label();
            this.toolStripAnchors = new System.Windows.Forms.ToolStrip();
            this.labelExegeticalHelp = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.toolStripAnchors.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripAnchors.Location = new System.Drawing.Point(34, 0);
            this.toolStripAnchors.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.toolStripAnchors.Name = "toolStripAnchors";
            this.toolStripAnchors.Size = new System.Drawing.Size(672, 25);
            this.toolStripAnchors.TabIndex = 0;
            this.toolStripAnchors.Text = "toolStripAnchors";
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
            this.Name = "AnchorControl";
            this.Size = new System.Drawing.Size(706, 71);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripAnchors;
        private System.Windows.Forms.Label labelAnchor;
        private System.Windows.Forms.Label labelExegeticalHelp;
    }
}
