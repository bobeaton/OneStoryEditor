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
            this.labelReference = new System.Windows.Forms.Label();
            this.tableLayoutPanelVerse = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxAnchor = new System.Windows.Forms.TextBox();
            this.labelAnchor = new System.Windows.Forms.Label();
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
            this.tableLayoutPanelVerse.Controls.Add(this.textBoxAnchor, 1, 4);
            this.tableLayoutPanelVerse.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanelVerse.Controls.Add(this.labelAnchor, 0, 4);
            this.tableLayoutPanelVerse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelVerse.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelVerse.Name = "tableLayoutPanelVerse";
            this.tableLayoutPanelVerse.RowCount = 5;
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelVerse.Size = new System.Drawing.Size(669, 225);
            this.tableLayoutPanelVerse.TabIndex = 1;
            // 
            // textBoxAnchor
            // 
            this.textBoxAnchor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAnchor.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAnchor.Location = new System.Drawing.Point(37, 16);
            this.textBoxAnchor.Multiline = true;
            this.textBoxAnchor.Name = "textBoxAnchor";
            this.textBoxAnchor.Size = new System.Drawing.Size(629, 206);
            this.textBoxAnchor.TabIndex = 8;
            // 
            // labelAnchor
            // 
            this.labelAnchor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAnchor.AutoSize = true;
            this.labelAnchor.Location = new System.Drawing.Point(3, 112);
            this.labelAnchor.Name = "labelAnchor";
            this.labelAnchor.Size = new System.Drawing.Size(28, 13);
            this.labelAnchor.TabIndex = 7;
            this.labelAnchor.Text = "anc:";
            // 
            // VerseBtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelVerse);
            this.Name = "VerseBtControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.tableLayoutPanelVerse.ResumeLayout(false);
            this.tableLayoutPanelVerse.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelReference;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVerse;
        private System.Windows.Forms.Label labelAnchor;
        private System.Windows.Forms.TextBox textBoxAnchor;
    }
}
