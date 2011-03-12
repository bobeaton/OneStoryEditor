namespace OneStoryProjectEditor
{
    partial class TaskBarForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskBarForm));
            this.taskBarControl = new OneStoryProjectEditor.TaskBarControl();
            this.SuspendLayout();
            // 
            // taskBarControl
            // 
            this.taskBarControl.AutoSize = true;
            this.taskBarControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.taskBarControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.taskBarControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskBarControl.Location = new System.Drawing.Point(0, 0);
            this.taskBarControl.Name = "taskBarControl";
            this.taskBarControl.Size = new System.Drawing.Size(203, 355);
            this.taskBarControl.TabIndex = 0;
            // 
            // TaskBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(203, 355);
            this.Controls.Add(this.taskBarControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TaskBarForm";
            this.ShowInTaskbar = false;
            this.Text = "Tasks";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TaskBarControl taskBarControl;

    }
}