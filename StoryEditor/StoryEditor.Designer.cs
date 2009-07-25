namespace StoryEditor
{
    partial class StoryEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoryEditor));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.teamMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelVerses = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerUpper = new System.Windows.Forms.SplitContainer();
            this.splitContainerUpDown = new System.Windows.Forms.SplitContainer();
            this.menuStrip.SuspendLayout();
            this.splitContainerUpper.Panel1.SuspendLayout();
            this.splitContainerUpper.SuspendLayout();
            this.splitContainerUpDown.Panel1.SuspendLayout();
            this.splitContainerUpDown.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(895, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.teamMembersToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.projectToolStripMenuItem.Text = "&Project";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // teamMembersToolStripMenuItem
            // 
            this.teamMembersToolStripMenuItem.Name = "teamMembersToolStripMenuItem";
            this.teamMembersToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.teamMembersToolStripMenuItem.Text = "Edit &Team members";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            // 
            // flowLayoutPanelVerses
            // 
            this.flowLayoutPanelVerses.AutoScroll = true;
            this.flowLayoutPanelVerses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelVerses.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelVerses.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelVerses.Name = "flowLayoutPanelVerses";
            this.flowLayoutPanelVerses.Size = new System.Drawing.Size(521, 291);
            this.flowLayoutPanelVerses.TabIndex = 1;
            this.flowLayoutPanelVerses.WrapContents = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "osp";
            this.openFileDialog.Filter = "OneStory Project file|*.osp";
            this.openFileDialog.Title = "Open OneStory Project File";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "osp";
            this.saveFileDialog.FileName = "StoryProjectName";
            this.saveFileDialog.Filter = "OneStory Project file|*.osp";
            this.saveFileDialog.Title = "Open OneStory Project File";
            // 
            // splitContainerUpper
            // 
            this.splitContainerUpper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUpper.Location = new System.Drawing.Point(0, 0);
            this.splitContainerUpper.Name = "splitContainerUpper";
            // 
            // splitContainerUpper.Panel1
            // 
            this.splitContainerUpper.Panel1.Controls.Add(this.flowLayoutPanelVerses);
            this.splitContainerUpper.Size = new System.Drawing.Size(895, 293);
            this.splitContainerUpper.SplitterDistance = 523;
            this.splitContainerUpper.TabIndex = 2;
            this.splitContainerUpper.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerUpper_SplitterMoved);
            // 
            // splitContainerUpDown
            // 
            this.splitContainerUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUpDown.Location = new System.Drawing.Point(0, 24);
            this.splitContainerUpDown.Name = "splitContainerUpDown";
            this.splitContainerUpDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerUpDown.Panel1
            // 
            this.splitContainerUpDown.Panel1.Controls.Add(this.splitContainerUpper);
            this.splitContainerUpDown.Size = new System.Drawing.Size(895, 453);
            this.splitContainerUpDown.SplitterDistance = 293;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 477);
            this.Controls.Add(this.splitContainerUpDown);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "StoryEditor";
            this.Text = "OneStory Editor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerUpper.Panel1.ResumeLayout(false);
            this.splitContainerUpper.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.ResumeLayout(false);
            this.splitContainerUpDown.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem teamMembersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelVerses;
        private System.Windows.Forms.SplitContainer splitContainerUpper;
        private System.Windows.Forms.SplitContainer splitContainerUpDown;
    }
}

