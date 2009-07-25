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
            this.textBoxVernacular = new System.Windows.Forms.TextBox();
            this.textBoxNationalBT = new System.Windows.Forms.TextBox();
            this.labelVernacular = new System.Windows.Forms.Label();
            this.labelNationalBT = new System.Windows.Forms.Label();
            this.labelInternationalBT = new System.Windows.Forms.Label();
            this.textBoxInternationalBT = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanelVerse.Controls.Add(this.textBoxVernacular, 1, 1);
            this.tableLayoutPanelVerse.Controls.Add(this.textBoxNationalBT, 1, 2);
            this.tableLayoutPanelVerse.Controls.Add(this.labelVernacular, 0, 1);
            this.tableLayoutPanelVerse.Controls.Add(this.labelNationalBT, 0, 2);
            this.tableLayoutPanelVerse.Controls.Add(this.labelInternationalBT, 0, 3);
            this.tableLayoutPanelVerse.Controls.Add(this.textBoxInternationalBT, 1, 3);
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
            this.textBoxAnchor.Location = new System.Drawing.Point(37, 94);
            this.textBoxAnchor.Multiline = true;
            this.textBoxAnchor.Name = "textBoxAnchor";
            this.textBoxAnchor.Size = new System.Drawing.Size(629, 128);
            this.textBoxAnchor.TabIndex = 8;
            // 
            // textBoxVernacular
            // 
            this.textBoxVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxVernacular.Font = new System.Drawing.Font("Arial Unicode MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVernacular.Location = new System.Drawing.Point(37, 16);
            this.textBoxVernacular.MaximumSize = new System.Drawing.Size(630, 80);
            this.textBoxVernacular.Multiline = true;
            this.textBoxVernacular.Name = "textBoxVernacular";
            this.textBoxVernacular.Size = new System.Drawing.Size(629, 20);
            this.textBoxVernacular.TabIndex = 1;
            // 
            // textBoxNationalBT
            // 
            this.textBoxNationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNationalBT.Font = new System.Drawing.Font("Arial Unicode MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNationalBT.Location = new System.Drawing.Point(37, 42);
            this.textBoxNationalBT.MaximumSize = new System.Drawing.Size(630, 80);
            this.textBoxNationalBT.Multiline = true;
            this.textBoxNationalBT.Name = "textBoxNationalBT";
            this.textBoxNationalBT.Size = new System.Drawing.Size(629, 20);
            this.textBoxNationalBT.TabIndex = 2;
            // 
            // labelVernacular
            // 
            this.labelVernacular.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelVernacular.AutoSize = true;
            this.labelVernacular.Location = new System.Drawing.Point(3, 19);
            this.labelVernacular.Name = "labelVernacular";
            this.labelVernacular.Size = new System.Drawing.Size(24, 13);
            this.labelVernacular.TabIndex = 3;
            this.labelVernacular.Text = "xnr:";
            // 
            // labelNationalBT
            // 
            this.labelNationalBT.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNationalBT.AutoSize = true;
            this.labelNationalBT.Location = new System.Drawing.Point(3, 45);
            this.labelNationalBT.Name = "labelNationalBT";
            this.labelNationalBT.Size = new System.Drawing.Size(28, 13);
            this.labelNationalBT.TabIndex = 4;
            this.labelNationalBT.Text = "hnd:";
            // 
            // labelInternationalBT
            // 
            this.labelInternationalBT.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelInternationalBT.AutoSize = true;
            this.labelInternationalBT.Location = new System.Drawing.Point(3, 71);
            this.labelInternationalBT.Name = "labelInternationalBT";
            this.labelInternationalBT.Size = new System.Drawing.Size(22, 13);
            this.labelInternationalBT.TabIndex = 5;
            this.labelInternationalBT.Text = "en:";
            // 
            // textBoxInternationalBT
            // 
            this.textBoxInternationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInternationalBT.Font = new System.Drawing.Font("Times New Roman", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInternationalBT.Location = new System.Drawing.Point(37, 68);
            this.textBoxInternationalBT.MaximumSize = new System.Drawing.Size(630, 80);
            this.textBoxInternationalBT.Multiline = true;
            this.textBoxInternationalBT.Name = "textBoxInternationalBT";
            this.textBoxInternationalBT.Size = new System.Drawing.Size(629, 20);
            this.textBoxInternationalBT.TabIndex = 6;
            // 
            // labelAnchor
            // 
            this.labelAnchor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAnchor.AutoSize = true;
            this.labelAnchor.Location = new System.Drawing.Point(3, 151);
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
        private System.Windows.Forms.TextBox textBoxVernacular;
        private System.Windows.Forms.TextBox textBoxNationalBT;
        private System.Windows.Forms.Label labelVernacular;
        private System.Windows.Forms.Label labelNationalBT;
        private System.Windows.Forms.Label labelInternationalBT;
        private System.Windows.Forms.TextBox textBoxInternationalBT;
        private System.Windows.Forms.Label labelAnchor;
        private System.Windows.Forms.TextBox textBoxAnchor;
    }
}
