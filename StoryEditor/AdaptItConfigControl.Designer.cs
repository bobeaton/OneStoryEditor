namespace OneStoryProjectEditor
{
    partial class AdaptItConfigControl
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonShared = new System.Windows.Forms.RadioButton();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.labelBT = new System.Windows.Forms.Label();
            this.labelProjectPath = new System.Windows.Forms.Label();
            this.textBoxProjectPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.groupBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelBT, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProjectPath, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxProjectPath, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonBrowse, 2, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(653, 74);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // groupBox
            // 
            this.groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.SetColumnSpan(this.groupBox, 2);
            this.groupBox.Controls.Add(this.radioButtonShared);
            this.groupBox.Controls.Add(this.radioButtonLocal);
            this.groupBox.Controls.Add(this.radioButtonNone);
            this.groupBox.Location = new System.Drawing.Point(136, 0);
            this.groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox.Size = new System.Drawing.Size(378, 47);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            // 
            // radioButtonShared
            // 
            this.radioButtonShared.AutoSize = true;
            this.radioButtonShared.Location = new System.Drawing.Point(181, 17);
            this.radioButtonShared.Name = "radioButtonShared";
            this.radioButtonShared.Size = new System.Drawing.Size(134, 17);
            this.radioButtonShared.TabIndex = 2;
            this.radioButtonShared.TabStop = true;
            this.radioButtonShared.Text = "Shared Adapt It project";
            this.toolTip.SetToolTip(this.radioButtonShared, "Select this option if you want to use a shared Adapt It project for this back tra" +
                    "nslation");
            this.radioButtonShared.UseVisualStyleBackColor = true;
            this.radioButtonShared.Click += new System.EventHandler(this.radioButtonShared_Click);
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.Location = new System.Drawing.Point(49, 17);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(126, 17);
            this.radioButtonLocal.TabIndex = 1;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.Text = "Local Adapt It project";
            this.toolTip.SetToolTip(this.radioButtonLocal, "Select this option if you want to use a local (not-shared) Adapt It project for t" +
                    "his back translation");
            this.radioButtonLocal.UseVisualStyleBackColor = true;
            this.radioButtonLocal.Click += new System.EventHandler(this.radioButtonLocal_Click);
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.radioButtonNone.Location = new System.Drawing.Point(4, 17);
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.Size = new System.Drawing.Size(39, 17);
            this.radioButtonNone.TabIndex = 0;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "&No";
            this.toolTip.SetToolTip(this.radioButtonNone, "Select this option if you don\'t want to use Adapt It for this back translation");
            this.radioButtonNone.UseVisualStyleBackColor = true;
            // 
            // labelBT
            // 
            this.labelBT.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelBT.AutoSize = true;
            this.labelBT.Location = new System.Drawing.Point(3, 17);
            this.labelBT.Name = "labelBT";
            this.labelBT.Size = new System.Drawing.Size(130, 13);
            this.labelBT.TabIndex = 2;
            this.labelBT.Text = "Use AdaptIt for X to Y BT:";
            // 
            // labelProjectPath
            // 
            this.labelProjectPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelProjectPath.AutoSize = true;
            this.labelProjectPath.Location = new System.Drawing.Point(3, 54);
            this.labelProjectPath.Name = "labelProjectPath";
            this.labelProjectPath.Size = new System.Drawing.Size(68, 13);
            this.labelProjectPath.TabIndex = 3;
            this.labelProjectPath.Text = "Project Path:";
            // 
            // textBoxProjectPath
            // 
            this.textBoxProjectPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxProjectPath.Location = new System.Drawing.Point(139, 50);
            this.textBoxProjectPath.Name = "textBoxProjectPath";
            this.textBoxProjectPath.Size = new System.Drawing.Size(487, 20);
            this.textBoxProjectPath.TabIndex = 4;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(629, 47);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(24, 23);
            this.buttonBrowse.TabIndex = 5;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // AdaptItConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "AdaptItConfigControl";
            this.Size = new System.Drawing.Size(653, 74);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.RadioButton radioButtonShared;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelBT;
        private System.Windows.Forms.Label labelProjectPath;
        private System.Windows.Forms.TextBox textBoxProjectPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
