namespace AiChorus
{
    partial class PresentProjectsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresentProjectsForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dataGridViewProjects = new System.Windows.Forms.DataGridView();
            this.ColumnButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnApplication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnProjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFolderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnServerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProjects)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonSave, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewProjects, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(711, 244);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(277, 218);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "&Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(358, 218);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // dataGridViewProjects
            // 
            this.dataGridViewProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnButton,
            this.ColumnApplication,
            this.ColumnProjectId,
            this.ColumnFolderName,
            this.ColumnServerName});
            this.tableLayoutPanel.SetColumnSpan(this.dataGridViewProjects, 2);
            this.dataGridViewProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewProjects.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewProjects.Name = "dataGridViewProjects";
            this.dataGridViewProjects.Size = new System.Drawing.Size(705, 209);
            this.dataGridViewProjects.TabIndex = 6;
            this.dataGridViewProjects.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OptionsButtonClicked);
            // 
            // ColumnButton
            // 
            this.ColumnButton.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnButton.HeaderText = "Options";
            this.ColumnButton.Name = "ColumnButton";
            this.ColumnButton.ToolTipText = "Click this to download (clone) or synchronize (send/receive) this project";
            this.ColumnButton.Width = 49;
            // 
            // ColumnApplication
            // 
            this.ColumnApplication.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnApplication.HeaderText = "Program";
            this.ColumnApplication.Name = "ColumnApplication";
            this.ColumnApplication.ReadOnly = true;
            this.ColumnApplication.Width = 71;
            // 
            // ColumnProjectId
            // 
            this.ColumnProjectId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnProjectId.HeaderText = "Project Id";
            this.ColumnProjectId.Name = "ColumnProjectId";
            this.ColumnProjectId.ReadOnly = true;
            this.ColumnProjectId.ToolTipText = "This is the internet repository identifier (20 characters or less, all lower case" +
    ", no spaces, underscores, etc)";
            this.ColumnProjectId.Width = 77;
            // 
            // ColumnFolderName
            // 
            this.ColumnFolderName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnFolderName.HeaderText = "Folder Name";
            this.ColumnFolderName.Name = "ColumnFolderName";
            this.ColumnFolderName.ReadOnly = true;
            this.ColumnFolderName.ToolTipText = "The sub-folder of the Application\'s root data store where this project will be do" +
    "wnloaded (cloned)";
            this.ColumnFolderName.Width = 92;
            // 
            // ColumnServerName
            // 
            this.ColumnServerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnServerName.HeaderText = "Server name";
            this.ColumnServerName.Name = "ColumnServerName";
            this.ColumnServerName.ReadOnly = true;
            this.ColumnServerName.ToolTipText = "The server on which this project is stored (i.e. private.languageDepot.org or lan" +
    "guageDepot.org, etc)";
            this.ColumnServerName.Width = 92;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "cpc";
            this.saveFileDialog.Filter = "Chorus Project Configuration Files|*.cpc|All files|*.*";
            this.saveFileDialog.Title = "Save Project Configuration";
            // 
            // PresentProjectsForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(735, 268);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PresentProjectsForm";
            this.Text = "Your Chorus Projects";
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProjects)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridView dataGridViewProjects;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnApplication;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnProjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFolderName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnServerName;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}