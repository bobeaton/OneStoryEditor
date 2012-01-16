namespace OneStoryProjectEditor
{
    partial class ViewSwordOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSwordOptionsForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.linkLabelLinkToSword = new System.Windows.Forms.LinkLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageInstalled = new System.Windows.Forms.TabPage();
            this.checkedListBoxSwordBibles = new System.Windows.Forms.CheckedListBox();
            this.tabPageSeedConnect = new System.Windows.Forms.TabPage();
            this.checkedListBoxDownloadable = new System.Windows.Forms.CheckedListBox();
            this.tabControl.SuspendLayout();
            this.tabPageInstalled.SuspendLayout();
            this.tabPageSeedConnect.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.AutoSize = true;
            this.buttonOK.Location = new System.Drawing.Point(171, 402);
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
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(253, 401);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // linkLabelLinkToSword
            // 
            this.linkLabelLinkToSword.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.linkLabelLinkToSword.AutoSize = true;
            this.linkLabelLinkToSword.Location = new System.Drawing.Point(13, 363);
            this.linkLabelLinkToSword.Name = "linkLabelLinkToSword";
            this.linkLabelLinkToSword.Size = new System.Drawing.Size(366, 17);
            this.linkLabelLinkToSword.TabIndex = 3;
            this.linkLabelLinkToSword.TabStop = true;
            this.linkLabelLinkToSword.Text = "Click here to learn about other Bible versions you can download and use";
            this.linkLabelLinkToSword.UseCompatibleTextRendering = true;
            this.linkLabelLinkToSword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLinkToSword_LinkClicked);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageInstalled);
            this.tabControl.Controls.Add(this.tabPageSeedConnect);
            this.tabControl.Location = new System.Drawing.Point(13, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(462, 335);
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageInstalled
            // 
            this.tabPageInstalled.Controls.Add(this.checkedListBoxSwordBibles);
            this.tabPageInstalled.Location = new System.Drawing.Point(4, 22);
            this.tabPageInstalled.Name = "tabPageInstalled";
            this.tabPageInstalled.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInstalled.Size = new System.Drawing.Size(454, 309);
            this.tabPageInstalled.TabIndex = 0;
            this.tabPageInstalled.Text = "Installed";
            this.tabPageInstalled.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxSwordBibles
            // 
            this.checkedListBoxSwordBibles.CheckOnClick = true;
            this.checkedListBoxSwordBibles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxSwordBibles.FormattingEnabled = true;
            this.checkedListBoxSwordBibles.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxSwordBibles.Name = "checkedListBoxSwordBibles";
            this.checkedListBoxSwordBibles.Size = new System.Drawing.Size(448, 303);
            this.checkedListBoxSwordBibles.TabIndex = 1;
            // 
            // tabPageSeedConnect
            // 
            this.tabPageSeedConnect.Controls.Add(this.checkedListBoxDownloadable);
            this.tabPageSeedConnect.Location = new System.Drawing.Point(4, 22);
            this.tabPageSeedConnect.Name = "tabPageSeedConnect";
            this.tabPageSeedConnect.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSeedConnect.Size = new System.Drawing.Size(454, 309);
            this.tabPageSeedConnect.TabIndex = 1;
            this.tabPageSeedConnect.Text = "Download";
            this.tabPageSeedConnect.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxDownloadable
            // 
            this.checkedListBoxDownloadable.CheckOnClick = true;
            this.checkedListBoxDownloadable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxDownloadable.FormattingEnabled = true;
            this.checkedListBoxDownloadable.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxDownloadable.Name = "checkedListBoxDownloadable";
            this.checkedListBoxDownloadable.Size = new System.Drawing.Size(448, 303);
            this.checkedListBoxDownloadable.TabIndex = 2;
            // 
            // ViewSwordOptionsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(487, 437);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.linkLabelLinkToSword);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewSwordOptionsForm";
            this.Text = "Sword Biblical Texts";
            this.tabControl.ResumeLayout(false);
            this.tabPageInstalled.ResumeLayout(false);
            this.tabPageSeedConnect.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.LinkLabel linkLabelLinkToSword;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageInstalled;
        private System.Windows.Forms.CheckedListBox checkedListBoxSwordBibles;
        private System.Windows.Forms.TabPage tabPageSeedConnect;
        private System.Windows.Forms.CheckedListBox checkedListBoxDownloadable;
    }
}