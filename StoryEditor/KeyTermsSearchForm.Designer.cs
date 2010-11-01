namespace OneStoryProjectEditor
{
    partial class KeyTermsSearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyTermsSearchForm));
            this.webBrowser = new onlyconnect.HtmlEditor();
            this.progressBarLoadingKeyTerms = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.DefaultComposeSettings.BackColor = System.Drawing.Color.White;
            this.webBrowser.DefaultComposeSettings.DefaultFont = new System.Drawing.Font("Arial", 10F);
            this.webBrowser.DefaultComposeSettings.Enabled = false;
            this.webBrowser.DefaultComposeSettings.ForeColor = System.Drawing.Color.Black;
            this.webBrowser.DefaultPreamble = onlyconnect.EncodingType.UTF8;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.DocumentEncoding = onlyconnect.EncodingType.WindowsCurrent;
            this.webBrowser.IsActivationEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.webBrowser.SelectionBackColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionBullets = false;
            this.webBrowser.SelectionFont = null;
            this.webBrowser.SelectionForeColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionNumbering = false;
            this.webBrowser.Size = new System.Drawing.Size(728, 337);
            this.webBrowser.TabIndex = 6;
            // 
            // progressBarLoadingKeyTerms
            // 
            this.progressBarLoadingKeyTerms.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBarLoadingKeyTerms.Location = new System.Drawing.Point(0, 337);
            this.progressBarLoadingKeyTerms.Name = "progressBarLoadingKeyTerms";
            this.progressBarLoadingKeyTerms.Size = new System.Drawing.Size(728, 23);
            this.progressBarLoadingKeyTerms.TabIndex = 7;
            // 
            // KeyTermsSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 360);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.progressBarLoadingKeyTerms);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KeyTermsSearchForm";
            this.Text = "Key Term Search";
            this.ResumeLayout(false);

        }

        #endregion

        private onlyconnect.HtmlEditor webBrowser;
        private System.Windows.Forms.ProgressBar progressBarLoadingKeyTerms;
    }
}