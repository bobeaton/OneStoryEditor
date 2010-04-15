namespace OneStoryProjectEditor
{
    partial class HtmlDisplayForm
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
            this.htmlCoachNotesControl1 = new OneStoryProjectEditor.HtmlCoachNotesControl();
            this._webBrowser = new OneStoryProjectEditor.HtmlConsultantNotesControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // htmlCoachNotesControl1
            // 
            this.htmlCoachNotesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlCoachNotesControl1.Location = new System.Drawing.Point(0, 0);
            this.htmlCoachNotesControl1.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlCoachNotesControl1.Name = "htmlCoachNotesControl1";
            this.htmlCoachNotesControl1.Size = new System.Drawing.Size(721, 166);
            this.htmlCoachNotesControl1.StoryData = null;
            this.htmlCoachNotesControl1.TabIndex = 1;
            this.htmlCoachNotesControl1.TheSE = null;
            // 
            // _webBrowser
            // 
            this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._webBrowser.Location = new System.Drawing.Point(0, 0);
            this._webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(721, 169);
            this._webBrowser.StoryData = null;
            this._webBrowser.TabIndex = 0;
            this._webBrowser.TheSE = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._webBrowser);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.htmlCoachNotesControl1);
            this.splitContainer1.Size = new System.Drawing.Size(721, 339);
            this.splitContainer1.SplitterDistance = 169;
            this.splitContainer1.TabIndex = 2;
            // 
            // HtmlDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 339);
            this.Controls.Add(this.splitContainer1);
            this.Name = "HtmlDisplayForm";
            this.Text = "HtmlDisplayForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private HtmlConsultantNotesControl _webBrowser;
        private HtmlCoachNotesControl htmlCoachNotesControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}