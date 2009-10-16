namespace OneStoryProjectEditor
{
    partial class HgRepoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HgRepoForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxHgRepoUrlBase = new System.Windows.Forms.TextBox();
            this.labelUrlBase = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxHgRepoUrl = new System.Windows.Forms.TextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.textBoxHgRepoUrlBase, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelUrlBase, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxUsername, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelUsername, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxPassword, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelPassword, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxHgRepoUrl, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.labelUrl, 0, 7);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(518, 316);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxHgRepoUrlBase
            // 
            this.textBoxHgRepoUrlBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHgRepoUrlBase.Location = new System.Drawing.Point(3, 3);
            this.textBoxHgRepoUrlBase.Name = "textBoxHgRepoUrlBase";
            this.textBoxHgRepoUrlBase.Size = new System.Drawing.Size(512, 20);
            this.textBoxHgRepoUrlBase.TabIndex = 0;
            this.textBoxHgRepoUrlBase.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelUrlBase
            // 
            this.labelUrlBase.AutoSize = true;
            this.labelUrlBase.Location = new System.Drawing.Point(3, 26);
            this.labelUrlBase.Name = "labelUrlBase";
            this.labelUrlBase.Size = new System.Drawing.Size(166, 13);
            this.labelUrlBase.TabIndex = 0;
            this.labelUrlBase.Text = "Internet host address of repository";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxUsername.Location = new System.Drawing.Point(3, 82);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(512, 20);
            this.textBoxUsername.TabIndex = 1;
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(3, 105);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(155, 13);
            this.labelUsername.TabIndex = 2;
            this.labelUsername.Text = "Username for the repository site";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPassword.Location = new System.Drawing.Point(3, 161);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(512, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(3, 184);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(153, 13);
            this.labelPassword.TabIndex = 4;
            this.labelPassword.Text = "Password for the repository site";
            // 
            // textBoxHgRepoUrl
            // 
            this.textBoxHgRepoUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHgRepoUrl.Location = new System.Drawing.Point(3, 240);
            this.textBoxHgRepoUrl.Name = "textBoxHgRepoUrl";
            this.textBoxHgRepoUrl.ReadOnly = true;
            this.textBoxHgRepoUrl.Size = new System.Drawing.Size(512, 20);
            this.textBoxHgRepoUrl.TabIndex = 5;
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(3, 263);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(178, 13);
            this.labelUrl.TabIndex = 6;
            this.labelUrl.Text = "Internet address of project repository";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(193, 335);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(275, 335);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // HgRepoForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(543, 370);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HgRepoForm";
            this.Text = "Internet Repository";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelUrlBase;
        private System.Windows.Forms.TextBox textBoxHgRepoUrlBase;
        private System.Windows.Forms.TextBox textBoxHgRepoUrl;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}