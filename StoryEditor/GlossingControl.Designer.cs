namespace OneStoryProjectEditor
{
    partial class GlossingControl
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
            this.contextMenuStripAmbiguityPicker = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editTargetWordsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripForSplitting = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctSpellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retranslateSourceWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTranslationDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel = new OneStoryProjectEditor.DynamicTableLayoutPanel();
            this.textBoxSourceWord = new System.Windows.Forms.TextBox();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.textBoxTargetWord = new System.Windows.Forms.TextBox();
            this.contextMenuStripAmbiguityPicker.SuspendLayout();
            this.contextMenuStripForSplitting.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripAmbiguityPicker
            // 
            this.contextMenuStripAmbiguityPicker.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editTargetWordsToolStripMenuItem1});
            this.contextMenuStripAmbiguityPicker.Name = "contextMenuStripAmbiguityPicker";
            this.contextMenuStripAmbiguityPicker.Size = new System.Drawing.Size(116, 26);
            this.contextMenuStripAmbiguityPicker.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripAmbiguityPicker_Opening);
            // 
            // editTargetWordsToolStripMenuItem1
            // 
            this.editTargetWordsToolStripMenuItem1.Name = "editTargetWordsToolStripMenuItem1";
            this.editTargetWordsToolStripMenuItem1.Size = new System.Drawing.Size(115, 22);
            this.editTargetWordsToolStripMenuItem1.Text = "&Edit List";
            this.editTargetWordsToolStripMenuItem1.Click += new System.EventHandler(this.editTargetWordsToolStripMenuItem_Click);
            // 
            // contextMenuStripForSplitting
            // 
            this.contextMenuStripForSplitting.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.splitToolStripMenuItem,
            this.correctSpellingToolStripMenuItem,
            this.retranslateSourceWordToolStripMenuItem,
            this.editTranslationDatabaseToolStripMenuItem});
            this.contextMenuStripForSplitting.Name = "contextMenuStripForSplitting";
            this.contextMenuStripForSplitting.Size = new System.Drawing.Size(226, 114);
            this.contextMenuStripForSplitting.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripForSplitting_Opening);
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            this.splitToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.splitToolStripMenuItem.Text = "&Split source phrase";
            this.splitToolStripMenuItem.Click += new System.EventHandler(this.splitToolStripMenuItem_Click);
            // 
            // correctSpellingToolStripMenuItem
            // 
            this.correctSpellingToolStripMenuItem.Name = "correctSpellingToolStripMenuItem";
            this.correctSpellingToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.correctSpellingToolStripMenuItem.Text = "&Correct source word spelling";
            this.correctSpellingToolStripMenuItem.Click += new System.EventHandler(this.correctSpellingToolStripMenuItem_Click);
            // 
            // retranslateSourceWordToolStripMenuItem
            // 
            this.retranslateSourceWordToolStripMenuItem.Name = "retranslateSourceWordToolStripMenuItem";
            this.retranslateSourceWordToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.retranslateSourceWordToolStripMenuItem.Text = "&Retranslate source word";
            this.retranslateSourceWordToolStripMenuItem.Click += new System.EventHandler(this.retranslateSourceWordToolStripMenuItem_Click);
            // 
            // editTranslationDatabaseToolStripMenuItem
            // 
            this.editTranslationDatabaseToolStripMenuItem.Name = "editTranslationDatabaseToolStripMenuItem";
            this.editTranslationDatabaseToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editTranslationDatabaseToolStripMenuItem.Text = "&View translation database";
            this.editTranslationDatabaseToolStripMenuItem.Click += new System.EventHandler(this.editTranslationDatabaseToolStripMenuItem_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.textBoxSourceWord, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonJoin, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxTargetWord, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(61, 52);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxSourceWord
            // 
            this.textBoxSourceWord.ContextMenuStrip = this.contextMenuStripForSplitting;
            this.textBoxSourceWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSourceWord.Location = new System.Drawing.Point(3, 3);
            this.textBoxSourceWord.Name = "textBoxSourceWord";
            this.textBoxSourceWord.ReadOnly = true;
            this.textBoxSourceWord.Size = new System.Drawing.Size(27, 20);
            this.textBoxSourceWord.TabIndex = 3;
            this.textBoxSourceWord.TabStop = false;
            this.textBoxSourceWord.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonJoin
            // 
            this.buttonJoin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonJoin.AutoSize = true;
            this.buttonJoin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonJoin.Image = global::OneStoryProjectEditor.Properties.Resources.Hyperlink;
            this.buttonJoin.Location = new System.Drawing.Point(36, 3);
            this.buttonJoin.Name = "buttonJoin";
            this.tableLayoutPanel.SetRowSpan(this.buttonJoin, 2);
            this.buttonJoin.Size = new System.Drawing.Size(22, 22);
            this.buttonJoin.TabIndex = 1;
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.buttonJoin_Click);
            // 
            // textBoxTargetWord
            // 
            this.textBoxTargetWord.ContextMenuStrip = this.contextMenuStripAmbiguityPicker;
            this.textBoxTargetWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTargetWord.Location = new System.Drawing.Point(3, 29);
            this.textBoxTargetWord.Name = "textBoxTargetWord";
            this.textBoxTargetWord.Size = new System.Drawing.Size(27, 20);
            this.textBoxTargetWord.TabIndex = 0;
            this.textBoxTargetWord.WordWrap = false;
            this.textBoxTargetWord.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxTargetWord.Leave += new System.EventHandler(this.textBoxTargetWord_Leave);
            this.textBoxTargetWord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTargetWord_KeyPress);
            this.textBoxTargetWord.Enter += new System.EventHandler(this.textBoxTargetWord_Enter);
            // 
            // GlossingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "GlossingControl";
            this.Size = new System.Drawing.Size(61, 52);
            this.contextMenuStripAmbiguityPicker.ResumeLayout(false);
            this.contextMenuStripForSplitting.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DynamicTableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TextBox textBoxSourceWord;
        private System.Windows.Forms.TextBox textBoxTargetWord;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAmbiguityPicker;
        private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTargetWordsToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripForSplitting;
        private System.Windows.Forms.ToolStripMenuItem correctSpellingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retranslateSourceWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTranslationDatabaseToolStripMenuItem;
    }
}
