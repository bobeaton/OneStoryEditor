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
            this.htmlStoryBtControl = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.SuspendLayout();
            // 
            // htmlStoryBtControl
            // 
            this.htmlStoryBtControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControl.Location = new System.Drawing.Point(0, 0);
            this.htmlStoryBtControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControl.Name = "htmlStoryBtControl";
            this.htmlStoryBtControl.Size = new System.Drawing.Size(721, 339);
            this.htmlStoryBtControl.StoryData = null;
            this.htmlStoryBtControl.TabIndex = 0;
            this.htmlStoryBtControl.TheSE = null;
            // 
            // HtmlDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 339);
            this.Controls.Add(this.htmlStoryBtControl);
            this.Name = "HtmlDisplayForm";
            this.Text = "HtmlDisplayForm";
            this.ResumeLayout(false);

        }

        #endregion

        private HtmlStoryBtControl htmlStoryBtControl;

    }
}