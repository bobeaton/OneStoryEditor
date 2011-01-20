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
            this.moveSelectedTextToANewLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addANewVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewVersesAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.hideVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTheWholeVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyVerseToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitStoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddTestQuestion = new System.Windows.Forms.ToolStripMenuItem();
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
            this.moveSelectedTextToANewLineToolStripMenuItem,
            this.moveItemsToolStripMenuItem,
            this.toolStripSeparator2,
            this.addANewVerseToolStripMenuItem,
            this.addNewVersesAfterToolStripMenuItem,
            this.hideVerseToolStripMenuItem,
            this.deleteTheWholeVerseToolStripMenuItem,
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem,
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem,
            this.copyVerseToClipboardToolStripMenuItem,
            this.toolStripSeparator1,
            this.splitStoryToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(343, 258);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // moveSelectedTextToANewLineToolStripMenuItem
            // 
            this.moveSelectedTextToANewLineToolStripMenuItem.Name = "moveSelectedTextToANewLineToolStripMenuItem";
            this.moveSelectedTextToANewLineToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.moveSelectedTextToANewLineToolStripMenuItem.Text = "Move &selected text to a new line";
            this.moveSelectedTextToANewLineToolStripMenuItem.ToolTipText = "This will take the selected text from the Story language, the National BT and the" +
                " English BT boxes and move them into a new line following this line";
            this.moveSelectedTextToANewLineToolStripMenuItem.Click += new System.EventHandler(this.moveSelectedTextToANewLineToolStripMenuItem_Click);
            // 
            // moveItemsToolStripMenuItem
            // 
            this.moveItemsToolStripMenuItem.Name = "moveItemsToolStripMenuItem";
            this.moveItemsToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.moveItemsToolStripMenuItem.Text = "&Move or Delete items";
            this.moveItemsToolStripMenuItem.ToolTipText = "Click this menu to move Testing Questions, Consultant Notes and/or Coach Notes to" +
                " another line";
            this.moveItemsToolStripMenuItem.Click += new System.EventHandler(this.moveItemsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(339, 6);
            // 
            // addANewVerseToolStripMenuItem
            // 
            this.addANewVerseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8});
            this.addANewVerseToolStripMenuItem.Name = "addANewVerseToolStripMenuItem";
            this.addANewVerseToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.addANewVerseToolStripMenuItem.Text = "Insert new &line(s) before";
            this.addANewVerseToolStripMenuItem.ToolTipText = "This line will move down and the new line(s) will be inserted before";
            this.addANewVerseToolStripMenuItem.Click += new System.EventHandler(this.addANewVerseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem3.Text = "2";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem4.Text = "3";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem5.Text = "5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem6.Text = "10";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem7.Text = "20";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem8.Text = "30";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // addNewVersesAfterToolStripMenuItem
            // 
            this.addNewVersesAfterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13,
            this.toolStripMenuItem14,
            this.toolStripMenuItem15});
            this.addNewVersesAfterToolStripMenuItem.Name = "addNewVersesAfterToolStripMenuItem";
            this.addNewVersesAfterToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.addNewVersesAfterToolStripMenuItem.Text = "&Add new line(s) after";
            this.addNewVersesAfterToolStripMenuItem.ToolTipText = "The new line(s) will be added after this line";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem9.Text = "1";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem10.Text = "2";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem11.Text = "3";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem12.Text = "5";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem13.Text = "10";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem14.Text = "20";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem15.Text = "30";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // hideVerseToolStripMenuItem
            // 
            this.hideVerseToolStripMenuItem.Name = "hideVerseToolStripMenuItem";
            this.hideVerseToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.hideVerseToolStripMenuItem.Text = "&Hide line";
            this.hideVerseToolStripMenuItem.ToolTipText = "Hide this line (use \'View\', \'Show Hidden\' to see them later)";
            this.hideVerseToolStripMenuItem.Click += new System.EventHandler(this.hideVerseToolStripMenuItem_Click);
            // 
            // deleteTheWholeVerseToolStripMenuItem
            // 
            this.deleteTheWholeVerseToolStripMenuItem.Name = "deleteTheWholeVerseToolStripMenuItem";
            this.deleteTheWholeVerseToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.deleteTheWholeVerseToolStripMenuItem.Text = "&Delete line";
            this.deleteTheWholeVerseToolStripMenuItem.ToolTipText = "Delete this line";
            this.deleteTheWholeVerseToolStripMenuItem.Click += new System.EventHandler(this.deleteTheWholeVerseToolStripMenuItem_Click);
            // 
            // pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem
            // 
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Name = "pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Text = "Paste line from clipboard and insert af&ter this line";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.ToolTipText = "Use this to paste a previously copied line (see \'Copy line to clipboard\' command)" +
                ". The copied line will be inserted before this line (see \'Paste line from clipbo" +
                "ard\' commands)";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Click += new System.EventHandler(this.pasteVerseFromClipboardAfterThisOneToolStripMenuItem_Click);
            // 
            // pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem
            // 
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Name = "pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Text = "&Paste line from clipboard and insert before this line";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.ToolTipText = "Use this to paste a previously copied line (see \'Copy line to clipboard\' command)" +
                ". The copied line will be inserted after this line.";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Click += new System.EventHandler(this.pasteVerseFromClipboardToolStripMenuItem_Click);
            // 
            // copyVerseToClipboardToolStripMenuItem
            // 
            this.copyVerseToClipboardToolStripMenuItem.Name = "copyVerseToClipboardToolStripMenuItem";
            this.copyVerseToClipboardToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.copyVerseToClipboardToolStripMenuItem.Text = "&Copy line to clipboard";
            this.copyVerseToClipboardToolStripMenuItem.ToolTipText = "Use this to copy a line to be pasted in another location";
            this.copyVerseToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyVerseToClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(339, 6);
            // 
            // splitStoryToolStripMenuItem
            // 
            this.splitStoryToolStripMenuItem.Name = "splitStoryToolStripMenuItem";
            this.splitStoryToolStripMenuItem.Size = new System.Drawing.Size(342, 22);
            this.splitStoryToolStripMenuItem.Text = "Split st&ory";
            this.splitStoryToolStripMenuItem.ToolTipText = "Move this and all following lines to a new story inserted immediately after this " +
                "story";
            this.splitStoryToolStripMenuItem.Click += new System.EventHandler(this.splitStoryToolStripMenuItem_Click);
            // 
            // menuAddTestQuestion
            // 
            this.menuAddTestQuestion.Name = "menuAddTestQuestion";
            this.menuAddTestQuestion.Size = new System.Drawing.Size(282, 22);
            this.menuAddTestQuestion.Text = "Add a story testing &question";
            this.menuAddTestQuestion.ToolTipText = "Click to add a Story Testing Question";
            this.menuAddTestQuestion.Click += new System.EventHandler(this.menuAddTestQuestion_Click);
            // 
            // VerseBtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
        private System.Windows.Forms.ToolStripMenuItem deleteTheWholeVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addANewVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewVersesAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem copyVerseToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveSelectedTextToANewLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem moveItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem splitStoryToolStripMenuItem;
    }
}
