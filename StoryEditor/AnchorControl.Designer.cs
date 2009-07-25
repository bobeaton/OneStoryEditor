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
            this.tableLayoutPanelAnchor = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxAnchor = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelAnchor.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelAnchor
            // 
            this.tableLayoutPanelAnchor.ColumnCount = 1;
            this.tableLayoutPanelAnchor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAnchor.Controls.Add(this.textBoxAnchor, 0, 0);
            this.tableLayoutPanelAnchor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAnchor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelAnchor.Name = "tableLayoutPanelAnchor";
            this.tableLayoutPanelAnchor.RowCount = 2;
            this.tableLayoutPanelAnchor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAnchor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAnchor.Size = new System.Drawing.Size(706, 230);
            this.tableLayoutPanelAnchor.TabIndex = 0;
            // 
            // textBoxAnchor
            // 
            this.textBoxAnchor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAnchor.Location = new System.Drawing.Point(3, 3);
            this.textBoxAnchor.Multiline = true;
            this.textBoxAnchor.Name = "textBoxAnchor";
            this.textBoxAnchor.Size = new System.Drawing.Size(700, 22);
            this.textBoxAnchor.TabIndex = 0;
            // 
            // AnchorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelAnchor);
            this.Name = "AnchorControl";
            this.Size = new System.Drawing.Size(706, 230);
            this.tableLayoutPanelAnchor.ResumeLayout(false);
            this.tableLayoutPanelAnchor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAnchor;
        private System.Windows.Forms.TextBox textBoxAnchor;
    }
}
