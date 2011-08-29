namespace OneStoryProjectEditor
{
    partial class AnchorAddCommentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnchorAddCommentForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelScriptureReference = new System.Windows.Forms.Label();
            this.labelJumpTarget = new System.Windows.Forms.Label();
            this.labelComment = new System.Windows.Forms.Label();
            this.textBoxAnchorComment = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.labelScriptureReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelJumpTarget, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelComment, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxAnchorComment, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonSave, 1, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(585, 103);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelScriptureReference
            // 
            this.labelScriptureReference.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelScriptureReference.AutoSize = true;
            this.labelScriptureReference.Location = new System.Drawing.Point(13, 10);
            this.labelScriptureReference.Name = "labelScriptureReference";
            this.labelScriptureReference.Size = new System.Drawing.Size(105, 13);
            this.labelScriptureReference.TabIndex = 0;
            this.labelScriptureReference.Text = "Scripture Reference:";
            // 
            // labelJumpTarget
            // 
            this.labelJumpTarget.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelJumpTarget.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelJumpTarget, 2);
            this.labelJumpTarget.Location = new System.Drawing.Point(124, 10);
            this.labelJumpTarget.Name = "labelJumpTarget";
            this.labelJumpTarget.Size = new System.Drawing.Size(153, 13);
            this.labelJumpTarget.TabIndex = 1;
            this.labelJumpTarget.Text = "<no need to localize/translate>";
            // 
            // labelComment
            // 
            this.labelComment.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelComment.AutoSize = true;
            this.labelComment.Location = new System.Drawing.Point(13, 29);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(54, 13);
            this.labelComment.TabIndex = 2;
            this.labelComment.Text = "Comment:";
            // 
            // textBoxAnchorComment
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxAnchorComment, 2);
            this.textBoxAnchorComment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAnchorComment.Location = new System.Drawing.Point(124, 26);
            this.textBoxAnchorComment.Name = "textBoxAnchorComment";
            this.textBoxAnchorComment.Size = new System.Drawing.Size(448, 20);
            this.textBoxAnchorComment.TabIndex = 3;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(424, 67);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSave.Location = new System.Drawing.Point(197, 67);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "&Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // AnchorAddCommentForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(585, 103);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AnchorAddCommentForm";
            this.Text = "Add a Comment to an Anchor";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelJumpTarget;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.TextBox textBoxAnchorComment;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelScriptureReference;
    }
}