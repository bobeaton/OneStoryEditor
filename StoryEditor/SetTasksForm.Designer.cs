namespace OneStoryProjectEditor
{
    partial class SetTasksForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetTasksForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkedListBoxAllowedTasks = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxRequiredTasks = new System.Windows.Forms.CheckedListBox();
            this.textBoxAllowedTasksLabel = new System.Windows.Forms.TextBox();
            this.textBoxRequiredTasksLabel = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(160, 258);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(242, 258);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.checkedListBoxAllowedTasks, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.checkedListBoxRequiredTasks, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxAllowedTasksLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxRequiredTasksLabel, 1, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(452, 240);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // checkedListBoxAllowedTasks
            // 
            this.checkedListBoxAllowedTasks.CheckOnClick = true;
            this.checkedListBoxAllowedTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxAllowedTasks.FormattingEnabled = true;
            this.checkedListBoxAllowedTasks.Location = new System.Drawing.Point(3, 38);
            this.checkedListBoxAllowedTasks.Name = "checkedListBoxAllowedTasks";
            this.checkedListBoxAllowedTasks.Size = new System.Drawing.Size(220, 199);
            this.checkedListBoxAllowedTasks.TabIndex = 4;
            // 
            // checkedListBoxRequiredTasks
            // 
            this.checkedListBoxRequiredTasks.CheckOnClick = true;
            this.checkedListBoxRequiredTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxRequiredTasks.FormattingEnabled = true;
            this.checkedListBoxRequiredTasks.Location = new System.Drawing.Point(229, 38);
            this.checkedListBoxRequiredTasks.Name = "checkedListBoxRequiredTasks";
            this.checkedListBoxRequiredTasks.Size = new System.Drawing.Size(220, 199);
            this.checkedListBoxRequiredTasks.TabIndex = 2;
            // 
            // textBoxAllowedTasksLabel
            // 
            this.textBoxAllowedTasksLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAllowedTasksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAllowedTasksLabel.Location = new System.Drawing.Point(3, 3);
            this.textBoxAllowedTasksLabel.Name = "textBoxAllowedTasksLabel";
            this.textBoxAllowedTasksLabel.ReadOnly = true;
            this.textBoxAllowedTasksLabel.Size = new System.Drawing.Size(220, 29);
            this.textBoxAllowedTasksLabel.TabIndex = 1;
            this.textBoxAllowedTasksLabel.Text = "Allowed Tasks";
            this.textBoxAllowedTasksLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxRequiredTasksLabel
            // 
            this.textBoxRequiredTasksLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRequiredTasksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRequiredTasksLabel.Location = new System.Drawing.Point(229, 3);
            this.textBoxRequiredTasksLabel.Name = "textBoxRequiredTasksLabel";
            this.textBoxRequiredTasksLabel.ReadOnly = true;
            this.textBoxRequiredTasksLabel.Size = new System.Drawing.Size(220, 29);
            this.textBoxRequiredTasksLabel.TabIndex = 3;
            this.textBoxRequiredTasksLabel.Text = "Required Tasks";
            this.textBoxRequiredTasksLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SetTasksForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(477, 293);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetTasksForm";
            this.Text = "Set Tasks for Project Facilitator";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.Button buttonOK;
        protected internal System.Windows.Forms.Button buttonCancel;
        protected internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        protected internal System.Windows.Forms.CheckedListBox checkedListBoxAllowedTasks;
        protected internal System.Windows.Forms.CheckedListBox checkedListBoxRequiredTasks;
        protected internal System.Windows.Forms.TextBox textBoxAllowedTasksLabel;
        protected internal System.Windows.Forms.TextBox textBoxRequiredTasksLabel;
    }
}