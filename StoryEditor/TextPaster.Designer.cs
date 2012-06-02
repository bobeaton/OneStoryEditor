namespace OneStoryProjectEditor
{
    partial class TextPaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextPaster));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBoxCurrentLine = new System.Windows.Forms.TextBox();
            this.textBoxNextLine = new System.Windows.Forms.TextBox();
            this.textBoxNextNextLine = new System.Windows.Forms.TextBox();
            this.buttonDeleteLine = new System.Windows.Forms.Button();
            this.buttonUndoLast = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxPause = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "txt";
            this.openFileDialog.Filter = "Text files|*.txt|All files|*.*";
            this.openFileDialog.Title = "Open the text file that contains the lines of text you want to paste into OSE";
            // 
            // textBoxCurrentLine
            // 
            this.textBoxCurrentLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurrentLine.Location = new System.Drawing.Point(12, 11);
            this.textBoxCurrentLine.Name = "textBoxCurrentLine";
            this.textBoxCurrentLine.Size = new System.Drawing.Size(524, 20);
            this.textBoxCurrentLine.TabIndex = 0;
            // 
            // textBoxNextLine
            // 
            this.textBoxNextLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNextLine.Location = new System.Drawing.Point(12, 35);
            this.textBoxNextLine.Name = "textBoxNextLine";
            this.textBoxNextLine.ReadOnly = true;
            this.textBoxNextLine.Size = new System.Drawing.Size(524, 20);
            this.textBoxNextLine.TabIndex = 1;
            // 
            // textBoxNextNextLine
            // 
            this.textBoxNextNextLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNextNextLine.Location = new System.Drawing.Point(12, 60);
            this.textBoxNextNextLine.Name = "textBoxNextNextLine";
            this.textBoxNextNextLine.ReadOnly = true;
            this.textBoxNextNextLine.Size = new System.Drawing.Size(524, 20);
            this.textBoxNextNextLine.TabIndex = 2;
            // 
            // buttonDeleteLine
            // 
            this.buttonDeleteLine.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDeleteLine.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.buttonDeleteLine.Location = new System.Drawing.Point(260, 97);
            this.buttonDeleteLine.Name = "buttonDeleteLine";
            this.buttonDeleteLine.Size = new System.Drawing.Size(25, 23);
            this.buttonDeleteLine.TabIndex = 3;
            this.buttonDeleteLine.Text = "  &D";
            this.toolTip.SetToolTip(this.buttonDeleteLine, "Click (or press Alt+D) to &delete current line");
            this.buttonDeleteLine.UseVisualStyleBackColor = true;
            this.buttonDeleteLine.Click += new System.EventHandler(this.ButtonDeleteLineClick);
            // 
            // buttonUndoLast
            // 
            this.buttonUndoLast.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonUndoLast.Image = global::OneStoryProjectEditor.Properties.Resources.Edit_Undo;
            this.buttonUndoLast.Location = new System.Drawing.Point(315, 97);
            this.buttonUndoLast.Name = "buttonUndoLast";
            this.buttonUndoLast.Size = new System.Drawing.Size(26, 23);
            this.buttonUndoLast.TabIndex = 4;
            this.buttonUndoLast.Text = "  &a";
            this.toolTip.SetToolTip(this.buttonUndoLast, "Click (or press Alt+A) to undo the l&ast change");
            this.buttonUndoLast.UseVisualStyleBackColor = true;
            this.buttonUndoLast.Click += new System.EventHandler(this.ButtonUndoLastClick);
            // 
            // checkBoxPause
            // 
            this.checkBoxPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkBoxPause.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxPause.Image = global::OneStoryProjectEditor.Properties.Resources.Pause;
            this.checkBoxPause.Location = new System.Drawing.Point(204, 97);
            this.checkBoxPause.Name = "checkBoxPause";
            this.checkBoxPause.Size = new System.Drawing.Size(26, 24);
            this.checkBoxPause.TabIndex = 6;
            this.checkBoxPause.Text = "  &p";
            this.toolTip.SetToolTip(this.checkBoxPause, "Click (or press Alt+P) to pause the pasting (e.g. if you want to move things arou" +
        "nd without a \'click\' meaning \'paste\')");
            this.checkBoxPause.UseVisualStyleBackColor = true;
            this.checkBoxPause.CheckedChanged += new System.EventHandler(this.CheckBoxPauseCheckedChanged);
            // 
            // TextPaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 132);
            this.Controls.Add(this.checkBoxPause);
            this.Controls.Add(this.buttonUndoLast);
            this.Controls.Add(this.buttonDeleteLine);
            this.Controls.Add(this.textBoxNextNextLine);
            this.Controls.Add(this.textBoxNextLine);
            this.Controls.Add(this.textBoxCurrentLine);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextPaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Text Paster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextPasterFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBoxCurrentLine;
        private System.Windows.Forms.TextBox textBoxNextLine;
        private System.Windows.Forms.TextBox textBoxNextNextLine;
        private System.Windows.Forms.Button buttonDeleteLine;
        private System.Windows.Forms.Button buttonUndoLast;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkBoxPause;
    }
}