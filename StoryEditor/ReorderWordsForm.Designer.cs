namespace OneStoryProjectEditor
{
    partial class ReorderWordsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReorderWordsForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanelWords = new System.Windows.Forms.FlowLayoutPanel();
            this.textBoxReorderedText = new System.Windows.Forms.TextBox();
            this.buttonUndoLast = new System.Windows.Forms.Button();
            this.flowLayoutPanelPunctuation = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelWords, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxReorderedText, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonUndoLast, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelPunctuation, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(613, 357);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(241, 331);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(322, 331);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelWords
            // 
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanelWords, 3);
            this.flowLayoutPanelWords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelWords.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelWords.Name = "flowLayoutPanelWords";
            this.flowLayoutPanelWords.Size = new System.Drawing.Size(607, 103);
            this.flowLayoutPanelWords.TabIndex = 2;
            // 
            // textBoxReorderedText
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxReorderedText, 3);
            this.textBoxReorderedText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReorderedText.Location = new System.Drawing.Point(3, 221);
            this.textBoxReorderedText.Multiline = true;
            this.textBoxReorderedText.Name = "textBoxReorderedText";
            this.textBoxReorderedText.Size = new System.Drawing.Size(607, 103);
            this.textBoxReorderedText.TabIndex = 3;
            // 
            // buttonUndoLast
            // 
            this.buttonUndoLast.Location = new System.Drawing.Point(534, 330);
            this.buttonUndoLast.Name = "buttonUndoLast";
            this.buttonUndoLast.Size = new System.Drawing.Size(75, 23);
            this.buttonUndoLast.TabIndex = 4;
            this.buttonUndoLast.Text = "&Undo Last";
            this.buttonUndoLast.UseVisualStyleBackColor = true;
            this.buttonUndoLast.Click += new System.EventHandler(this.buttonUndoLast_Click);
            // 
            // flowLayoutPanelPunctuation
            // 
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanelPunctuation, 3);
            this.flowLayoutPanelPunctuation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelPunctuation.Location = new System.Drawing.Point(3, 112);
            this.flowLayoutPanelPunctuation.Name = "flowLayoutPanelPunctuation";
            this.flowLayoutPanelPunctuation.Size = new System.Drawing.Size(607, 103);
            this.flowLayoutPanelPunctuation.TabIndex = 5;
            // 
            // ReorderWordsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(638, 382);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReorderWordsForm";
            this.Text = "Reorder Words";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelWords;
        private System.Windows.Forms.TextBox textBoxReorderedText;
        private System.Windows.Forms.Button buttonUndoLast;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPunctuation;
        private System.Windows.Forms.ToolTip toolTip;
    }
}