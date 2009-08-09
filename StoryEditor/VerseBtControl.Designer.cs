namespace OneStoryProjectEditor
{
    partial class VerseBtControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelReference = new System.Windows.Forms.Label();
            this.buttonDragDropHandle = new System.Windows.Forms.Button();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddTestQuestion = new System.Windows.Forms.ToolStripMenuItem();
            this.addRetellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTestQuestionAnswerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTheWholeVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.labelReference.Location = new System.Drawing.Point(3, 0);
            this.labelReference.Name = "labelReference";
            this.labelReference.Size = new System.Drawing.Size(79, 13);
            this.labelReference.TabIndex = 0;
            this.labelReference.Text = "labelReference";
            // 
            // buttonDragDropHandle
            // 
            this.buttonDragDropHandle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDragDropHandle.ContextMenuStrip = this.contextMenuStrip;
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
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddTestQuestion,
            this.addRetellingToolStripMenuItem,
            this.addTestQuestionAnswerToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.deleteTheWholeVerseToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(257, 136);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // menuAddTestQuestion
            // 
            this.menuAddTestQuestion.Name = "menuAddTestQuestion";
            this.menuAddTestQuestion.Size = new System.Drawing.Size(256, 22);
            this.menuAddTestQuestion.Text = "Add a &Test Question";
            this.menuAddTestQuestion.ToolTipText = "Click to add a Story Testing Question";
            this.menuAddTestQuestion.Click += new System.EventHandler(this.menuAddTestQuestion_Click);
            // 
            // addRetellingToolStripMenuItem
            // 
            this.addRetellingToolStripMenuItem.Name = "addRetellingToolStripMenuItem";
            this.addRetellingToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.addRetellingToolStripMenuItem.Text = "Add a UNS &Retelling";
            this.addRetellingToolStripMenuItem.Click += new System.EventHandler(this.addRetellingToolStripMenuItem_Click);
            // 
            // addTestQuestionAnswerToolStripMenuItem
            // 
            this.addTestQuestionAnswerToolStripMenuItem.Name = "addTestQuestionAnswerToolStripMenuItem";
            this.addTestQuestionAnswerToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.addTestQuestionAnswerToolStripMenuItem.Text = "Add a UNS Answer to a &Test Question";
            this.addTestQuestionAnswerToolStripMenuItem.Click += new System.EventHandler(this.addTestQuestionAnswerToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // deleteTheWholeVerseToolStripMenuItem
            // 
            this.deleteTheWholeVerseToolStripMenuItem.Name = "deleteTheWholeVerseToolStripMenuItem";
            this.deleteTheWholeVerseToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.deleteTheWholeVerseToolStripMenuItem.Text = "&Delete the whole verse";
            this.deleteTheWholeVerseToolStripMenuItem.Click += new System.EventHandler(this.deleteTheWholeVerseToolStripMenuItem_Click);
            // 
            // VerseBtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "VerseBtControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelReference;
        private System.Windows.Forms.Button buttonDragDropHandle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuAddTestQuestion;
        private System.Windows.Forms.ToolStripMenuItem addRetellingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTestQuestionAnswerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTheWholeVerseToolStripMenuItem;
    }
}
