namespace OneStoryProjectEditor
{
    partial class LnCNotePrintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LnCNotePrintForm));
            this.printViewer = new OneStoryProjectEditor.PrintViewer();
            this.SuspendLayout();
            // 
            // printViewer
            // 
            this.printViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printViewer.Location = new System.Drawing.Point(0, 0);
            this.printViewer.Name = "printViewer";
            this.printViewer.Size = new System.Drawing.Size(662, 335);
            this.printViewer.TabIndex = 0;
            // 
            // LnCNotePrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 335);
            this.Controls.Add(this.printViewer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LnCNotePrintForm";
            this.Text = "Print L & C Notes";
            this.ResumeLayout(false);

        }

        #endregion

        private PrintViewer printViewer;

    }
}