namespace OneStoryProjectEditor
{
    partial class ConsultNoteControl
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
            this.components = new System.ComponentModel.Container();
            this.labelRound = new System.Windows.Forms.Label();
            this.buttonDragDropHandle = new System.Windows.Forms.Button();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNoteOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStripNoteOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRound
            // 
            this.labelRound.AutoSize = true;
            this.labelRound.Location = new System.Drawing.Point(3, 0);
            this.labelRound.Name = "labelRound";
            this.labelRound.Size = new System.Drawing.Size(79, 13);
            this.labelRound.TabIndex = 0;
            this.labelRound.Text = "labelRound";
            // 
            // buttonDragDropHandle
            // 
            this.buttonDragDropHandle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDragDropHandle.ContextMenuStrip = this.contextMenuStripNoteOptions;
            this.buttonDragDropHandle.Location = new System.Drawing.Point(0, 0);
            this.buttonDragDropHandle.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDragDropHandle.MaximumSize = new System.Drawing.Size(15, 15);
            this.buttonDragDropHandle.Name = "buttonDragDropHandle";
            this.buttonDragDropHandle.Size = new System.Drawing.Size(15, 15);
            this.buttonDragDropHandle.TabIndex = 1;
            this.buttonDragDropHandle.UseVisualStyleBackColor = true;
            this.buttonDragDropHandle.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.buttonDragDropHandle_QueryContinueDrag);
            this.buttonDragDropHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonDragDropHandle_MouseDown);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteMenuItem.Text = "&Delete";
            this.deleteMenuItem.ToolTipText = "Click to delete this note from the project";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // hideMenuItem
            // 
            this.hideMenuItem.Name = "hideMenuItem";
            this.hideMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hideMenuItem.Text = "&Hide";
            this.hideMenuItem.ToolTipText = "Click to hide this note, but keep it in the project";
            this.hideMenuItem.Click += new System.EventHandler(this.hideMenuItem_Click);
            // 
            // contextMenuStripNoteOptions
            // 
            this.contextMenuStripNoteOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteMenuItem,
            this.hideMenuItem});
            this.contextMenuStripNoteOptions.Name = "contextMenuStripNoteOptions";
            this.contextMenuStripNoteOptions.Size = new System.Drawing.Size(153, 70);
            this.contextMenuStripNoteOptions.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripNoteOptions_Opening);
            // 
            // ConsultNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Name = "ConsultNoteControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.contextMenuStripNoteOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelRound;
        private System.Windows.Forms.Button buttonDragDropHandle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNoteOptions;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMenuItem;
    }
}
