namespace NetLoc
{
    partial class ScreenCaptureForm
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
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.chkReplace = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCapture = new System.Windows.Forms.Button();
            this.cmdMenus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(123, 37);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(261, 22);
            this.txtFileName.TabIndex = 0;
            // 
            // chkReplace
            // 
            this.chkReplace.AutoSize = true;
            this.chkReplace.Location = new System.Drawing.Point(19, 85);
            this.chkReplace.Name = "chkReplace";
            this.chkReplace.Size = new System.Drawing.Size(174, 21);
            this.chkReplace.TabIndex = 1;
            this.chkReplace.Text = "Replace Existing File(s)";
            this.chkReplace.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "File Name";
            // 
            // cmdCapture
            // 
            this.cmdCapture.Location = new System.Drawing.Point(248, 175);
            this.cmdCapture.Name = "cmdCapture";
            this.cmdCapture.Size = new System.Drawing.Size(137, 32);
            this.cmdCapture.TabIndex = 5;
            this.cmdCapture.Text = "Current Screen";
            this.cmdCapture.UseVisualStyleBackColor = true;
            this.cmdCapture.Click += new System.EventHandler(this.cmdCapture_Click);
            // 
            // cmdMenus
            // 
            this.cmdMenus.Location = new System.Drawing.Point(20, 176);
            this.cmdMenus.Name = "cmdMenus";
            this.cmdMenus.Size = new System.Drawing.Size(137, 30);
            this.cmdMenus.TabIndex = 6;
            this.cmdMenus.Text = "All Menus";
            this.cmdMenus.UseVisualStyleBackColor = true;
            this.cmdMenus.Click += new System.EventHandler(this.cmdMenus_Click);
            // 
            // ScreenCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 223);
            this.Controls.Add(this.cmdMenus);
            this.Controls.Add(this.cmdCapture);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkReplace);
            this.Controls.Add(this.txtFileName);
            this.Name = "ScreenCaptureForm";
            this.Text = "Capture Localized Screens";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.CheckBox chkReplace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCapture;
        private System.Windows.Forms.Button cmdMenus;
    }
}