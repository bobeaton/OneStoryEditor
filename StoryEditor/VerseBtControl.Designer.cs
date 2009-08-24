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
            this.splitStoryIntoVersesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuAddTestQuestion = new System.Windows.Forms.ToolStripMenuItem();
            this.addTestQuestionAnswerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRetellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTheWholeVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.glossNationalBTToEnglishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.glossVernacularToNationalBTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.splitStoryIntoVersesToolStripMenuItem,
            this.addANewVerseToolStripMenuItem,
            this.addNewVersesAfterToolStripMenuItem,
            this.menuAddTestQuestion,
            this.addTestQuestionAnswerToolStripMenuItem,
            this.addRetellingToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.deleteTheWholeVerseToolStripMenuItem,
            this.glossVernacularToNationalBTToolStripMenuItem,
            this.glossNationalBTToEnglishToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(273, 246);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // splitStoryIntoVersesToolStripMenuItem
            // 
            this.splitStoryIntoVersesToolStripMenuItem.Name = "splitStoryIntoVersesToolStripMenuItem";
            this.splitStoryIntoVersesToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.splitStoryIntoVersesToolStripMenuItem.Text = "&Split story into verses";
            this.splitStoryIntoVersesToolStripMenuItem.Click += new System.EventHandler(this.splitStoryIntoVersesToolStripMenuItem_Click);
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
            this.addANewVerseToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.addANewVerseToolStripMenuItem.Text = "Insert new &verse(s) before";
            this.addANewVerseToolStripMenuItem.ToolTipText = "This verse will move down and the new verse will be inserted before";
            this.addANewVerseToolStripMenuItem.Click += new System.EventHandler(this.addANewVerseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem2.Text = "1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem3.Text = "2";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem4.Text = "3";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem5.Text = "5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem6.Text = "10";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem7.Text = "20";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(86, 22);
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
            this.addNewVersesAfterToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.addNewVersesAfterToolStripMenuItem.Text = "Add new verse(s) after";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem9.Text = "1";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem10.Text = "2";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem11.Text = "3";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem12.Text = "5";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem13.Text = "10";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem14.Text = "20";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(86, 22);
            this.toolStripMenuItem15.Text = "30";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // menuAddTestQuestion
            // 
            this.menuAddTestQuestion.Name = "menuAddTestQuestion";
            this.menuAddTestQuestion.Size = new System.Drawing.Size(272, 22);
            this.menuAddTestQuestion.Text = "Add a story testing &question";
            this.menuAddTestQuestion.ToolTipText = "Click to add a Story Testing Question";
            this.menuAddTestQuestion.Click += new System.EventHandler(this.menuAddTestQuestion_Click);
            // 
            // addTestQuestionAnswerToolStripMenuItem
            // 
            this.addTestQuestionAnswerToolStripMenuItem.Name = "addTestQuestionAnswerToolStripMenuItem";
            this.addTestQuestionAnswerToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.addTestQuestionAnswerToolStripMenuItem.Text = "Add a UNS answer to a &testing question";
            this.addTestQuestionAnswerToolStripMenuItem.Click += new System.EventHandler(this.addTestQuestionAnswerToolStripMenuItem_Click);
            // 
            // addRetellingToolStripMenuItem
            // 
            this.addRetellingToolStripMenuItem.Name = "addRetellingToolStripMenuItem";
            this.addRetellingToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.addRetellingToolStripMenuItem.Text = "Add a &UNS retelling";
            this.addRetellingToolStripMenuItem.Click += new System.EventHandler(this.addRetellingToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // deleteTheWholeVerseToolStripMenuItem
            // 
            this.deleteTheWholeVerseToolStripMenuItem.Name = "deleteTheWholeVerseToolStripMenuItem";
            this.deleteTheWholeVerseToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.deleteTheWholeVerseToolStripMenuItem.Text = "&Delete the whole verse";
            this.deleteTheWholeVerseToolStripMenuItem.Click += new System.EventHandler(this.deleteTheWholeVerseToolStripMenuItem_Click);
            // 
            // glossNationalBTToEnglishToolStripMenuItem
            // 
            this.glossNationalBTToEnglishToolStripMenuItem.Name = "glossNationalBTToEnglishToolStripMenuItem";
            this.glossNationalBTToEnglishToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.glossNationalBTToEnglishToolStripMenuItem.Text = "&Gloss Vernacular in English";
            this.glossNationalBTToEnglishToolStripMenuItem.Click += new System.EventHandler(this.glossNationalBTToEnglishToolStripMenuItem_Click);
            // 
            // glossVernacularToNationalBTToolStripMenuItem
            // 
            this.glossVernacularToNationalBTToolStripMenuItem.Name = "glossVernacularToNationalBTToolStripMenuItem";
            this.glossVernacularToNationalBTToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.glossVernacularToNationalBTToolStripMenuItem.Text = "Gloss Vernacular in the &National language";
            this.glossVernacularToNationalBTToolStripMenuItem.Click += new System.EventHandler(this.glossVernacularToNationalBTToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem addRetellingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTestQuestionAnswerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem splitStoryIntoVersesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem glossNationalBTToEnglishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem glossVernacularToNationalBTToolStripMenuItem;
    }
}
