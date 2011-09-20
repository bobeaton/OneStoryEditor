namespace NetLoc
{
    partial class LocalizableMessageBox
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRightMost = new System.Windows.Forms.Button();
            this.buttonMiddle = new System.Windows.Forms.Button();
            this.buttonLeftMost = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonRightMost, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonMiddle, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonLeftMost, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxInput, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(353, 149);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonRightMost
            // 
            this.buttonRightMost.Location = new System.Drawing.Point(275, 123);
            this.buttonRightMost.Name = "buttonRightMost";
            this.buttonRightMost.Size = new System.Drawing.Size(75, 23);
            this.buttonRightMost.TabIndex = 1;
            this.buttonRightMost.UseVisualStyleBackColor = true;
            this.buttonRightMost.Click += new System.EventHandler(this.ButtonClick);
            // 
            // buttonMiddle
            // 
            this.buttonMiddle.Location = new System.Drawing.Point(194, 123);
            this.buttonMiddle.Name = "buttonMiddle";
            this.buttonMiddle.Size = new System.Drawing.Size(75, 23);
            this.buttonMiddle.TabIndex = 2;
            this.buttonMiddle.UseVisualStyleBackColor = true;
            this.buttonMiddle.Visible = false;
            this.buttonMiddle.Click += new System.EventHandler(this.ButtonClick);
            // 
            // buttonLeftMost
            // 
            this.buttonLeftMost.Location = new System.Drawing.Point(113, 123);
            this.buttonLeftMost.Name = "buttonLeftMost";
            this.buttonLeftMost.Size = new System.Drawing.Size(75, 23);
            this.buttonLeftMost.TabIndex = 3;
            this.buttonLeftMost.UseVisualStyleBackColor = true;
            this.buttonLeftMost.Visible = false;
            this.buttonLeftMost.Click += new System.EventHandler(this.ButtonClick);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelMessage, 4);
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMessage.Location = new System.Drawing.Point(3, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(347, 94);
            this.labelMessage.TabIndex = 0;
            // 
            // textBoxInput
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxInput, 4);
            this.textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInput.Location = new System.Drawing.Point(3, 97);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(347, 20);
            this.textBoxInput.TabIndex = 4;
            this.textBoxInput.Visible = false;
            // 
            // LocalizableMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 173);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalizableMessageBox";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonRightMost;
        private System.Windows.Forms.Button buttonMiddle;
        private System.Windows.Forms.Button buttonLeftMost;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.TextBox textBoxInput;
    }
}