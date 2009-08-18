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
            this.checkedListBoxSwordBibles = new System.Windows.Forms.CheckedListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.linkLabelLinkToSword = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // checkedListBoxSwordBibles
            // 
            this.checkedListBoxSwordBibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxSwordBibles.CheckOnClick = true;
            this.checkedListBoxSwordBibles.FormattingEnabled = true;
            this.checkedListBoxSwordBibles.Location = new System.Drawing.Point(13, 13);
            this.checkedListBoxSwordBibles.Name = "checkedListBoxSwordBibles";
            this.checkedListBoxSwordBibles.Size = new System.Drawing.Size(462, 334);
            this.checkedListBoxSwordBibles.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
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
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(253, 401);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
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
            // ViewSwordOptionsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(487, 437);
            this.Controls.Add(this.linkLabelLinkToSword);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkedListBoxSwordBibles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewSwordOptionsForm";
            this.Text = "Sword Biblical Texts";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxSwordBibles;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.LinkLabel linkLabelLinkToSword;
    }
}