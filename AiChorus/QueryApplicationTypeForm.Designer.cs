namespace AiChorus
{
    partial class QueryApplicationTypeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryApplicationTypeForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxApplicationType = new System.Windows.Forms.GroupBox();
            this.radioButtonAdaptIt = new System.Windows.Forms.RadioButton();
            this.radioButtonOse = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBoxApplicationType.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(153, 411);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(79, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.Location = new System.Drawing.Point(238, 411);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.groupBoxApplicationType, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(471, 437);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // groupBoxApplicationType
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBoxApplicationType, 2);
            this.groupBoxApplicationType.Controls.Add(this.radioButtonAdaptIt);
            this.groupBoxApplicationType.Controls.Add(this.radioButtonOse);
            this.groupBoxApplicationType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxApplicationType.Location = new System.Drawing.Point(3, 3);
            this.groupBoxApplicationType.Name = "groupBoxApplicationType";
            this.groupBoxApplicationType.Size = new System.Drawing.Size(465, 44);
            this.groupBoxApplicationType.TabIndex = 2;
            this.groupBoxApplicationType.TabStop = false;
            this.groupBoxApplicationType.Text = "Choose application for project";
            // 
            // radioButtonAdaptIt
            // 
            this.radioButtonAdaptIt.AutoSize = true;
            this.radioButtonAdaptIt.Location = new System.Drawing.Point(290, 20);
            this.radioButtonAdaptIt.Name = "radioButtonAdaptIt";
            this.radioButtonAdaptIt.Size = new System.Drawing.Size(62, 17);
            this.radioButtonAdaptIt.TabIndex = 1;
            this.radioButtonAdaptIt.TabStop = true;
            this.radioButtonAdaptIt.Text = "&Adapt It";
            this.radioButtonAdaptIt.UseVisualStyleBackColor = true;
            this.radioButtonAdaptIt.CheckedChanged += new System.EventHandler(this.radioButtonAdaptIt_CheckedChanged);
            // 
            // radioButtonOse
            // 
            this.radioButtonOse.AutoSize = true;
            this.radioButtonOse.Location = new System.Drawing.Point(100, 20);
            this.radioButtonOse.Name = "radioButtonOse";
            this.radioButtonOse.Size = new System.Drawing.Size(99, 17);
            this.radioButtonOse.TabIndex = 0;
            this.radioButtonOse.TabStop = true;
            this.radioButtonOse.Text = "&OneStory Editor";
            this.radioButtonOse.UseVisualStyleBackColor = true;
            this.radioButtonOse.CheckedChanged += new System.EventHandler(this.RadioButtonOseCheckedChanged);
            // 
            // QueryApplicationTypeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 462);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QueryApplicationTypeForm";
            this.Text = "Configure Project Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QueryApplicationTypeForm_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBoxApplicationType.ResumeLayout(false);
            this.groupBoxApplicationType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox groupBoxApplicationType;
        private System.Windows.Forms.RadioButton radioButtonAdaptIt;
        private System.Windows.Forms.RadioButton radioButtonOse;
    }
}