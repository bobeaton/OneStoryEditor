namespace OneStoryProjectEditor
{
    partial class AiRepoSelectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AiRepoSelectionForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxInternet = new System.Windows.Forms.CheckBox();
            this.checkBoxNetwork = new System.Windows.Forms.CheckBox();
            this.textBoxNetwork = new System.Windows.Forms.TextBox();
            this.labelNetworkPath = new System.Windows.Forms.Label();
            this.buttonBrowseNetwork = new System.Windows.Forms.Button();
            this.buttonPushToInternet = new System.Windows.Forms.Button();
            this.buttonPullFromInternet = new System.Windows.Forms.Button();
            this.buttonPushToNetwork = new System.Windows.Forms.Button();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.labelFullInternetUrl = new System.Windows.Forms.Label();
            this.comboBoxServer = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.checkBoxInternet, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.checkBoxNetwork, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxNetwork, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.labelNetworkPath, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonBrowseNetwork, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonPushToInternet, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonPullFromInternet, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonPushToNetwork, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.labelProjectName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxProjectName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelFullInternetUrl, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.comboBoxServer, 1, 2);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(622, 214);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // checkBoxInternet
            // 
            this.checkBoxInternet.AutoSize = true;
            this.checkBoxInternet.Location = new System.Drawing.Point(3, 49);
            this.checkBoxInternet.Name = "checkBoxInternet";
            this.checkBoxInternet.Size = new System.Drawing.Size(97, 17);
            this.checkBoxInternet.TabIndex = 2;
            this.checkBoxInternet.Text = "&Internet server:";
            this.toolTip.SetToolTip(this.checkBoxInternet, "Check this box to enter an internet (http) address for the Adapt It repository");
            this.checkBoxInternet.UseVisualStyleBackColor = true;
            this.checkBoxInternet.CheckedChanged += new System.EventHandler(this.checkBoxInternet_CheckedChanged);
            // 
            // checkBoxNetwork
            // 
            this.checkBoxNetwork.AutoSize = true;
            this.checkBoxNetwork.Location = new System.Drawing.Point(3, 128);
            this.checkBoxNetwork.Name = "checkBoxNetwork";
            this.checkBoxNetwork.Size = new System.Drawing.Size(98, 17);
            this.checkBoxNetwork.TabIndex = 7;
            this.checkBoxNetwork.Text = "&Network Folder";
            this.toolTip.SetToolTip(this.checkBoxNetwork, "Check this box to enter a network shared folder path for the Adapt It repository");
            this.checkBoxNetwork.UseVisualStyleBackColor = true;
            this.checkBoxNetwork.CheckedChanged += new System.EventHandler(this.checkBoxNetwork_CheckedChanged);
            // 
            // textBoxNetwork
            // 
            this.textBoxNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNetwork.Location = new System.Drawing.Point(107, 128);
            this.textBoxNetwork.Name = "textBoxNetwork";
            this.textBoxNetwork.Size = new System.Drawing.Size(482, 20);
            this.textBoxNetwork.TabIndex = 8;
            this.toolTip.SetToolTip(this.textBoxNetwork, "Network path to the shared network folder for this Adapt It project (e.g. \'\\\\Stud" +
        "ioXPS\\SharedDrive\\Adapt It Unicode Work\\Hindi to English adaptations\')");
            this.textBoxNetwork.TextChanged += new System.EventHandler(this.textBoxNetwork_TextChanged);
            // 
            // labelNetworkPath
            // 
            this.labelNetworkPath.AutoSize = true;
            this.labelNetworkPath.Location = new System.Drawing.Point(107, 154);
            this.labelNetworkPath.Name = "labelNetworkPath";
            this.labelNetworkPath.Size = new System.Drawing.Size(0, 13);
            this.labelNetworkPath.TabIndex = 10;
            // 
            // buttonBrowseNetwork
            // 
            this.buttonBrowseNetwork.Location = new System.Drawing.Point(595, 128);
            this.buttonBrowseNetwork.Name = "buttonBrowseNetwork";
            this.buttonBrowseNetwork.Size = new System.Drawing.Size(24, 23);
            this.buttonBrowseNetwork.TabIndex = 9;
            this.buttonBrowseNetwork.Text = "...";
            this.toolTip.SetToolTip(this.buttonBrowseNetwork, "Click to browse for the network folder where the repository is stored");
            this.buttonBrowseNetwork.UseVisualStyleBackColor = true;
            this.buttonBrowseNetwork.Click += new System.EventHandler(this.buttonBrowseNetwork_Click);
            // 
            // buttonPushToInternet
            // 
            this.buttonPushToInternet.Enabled = false;
            this.buttonPushToInternet.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_moveup1;
            this.buttonPushToInternet.Location = new System.Drawing.Point(595, 49);
            this.buttonPushToInternet.Name = "buttonPushToInternet";
            this.buttonPushToInternet.Size = new System.Drawing.Size(24, 23);
            this.buttonPushToInternet.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonPushToInternet, "Click to upload the shared project to the internet");
            this.buttonPushToInternet.UseVisualStyleBackColor = true;
            this.buttonPushToInternet.Click += new System.EventHandler(this.buttonPushToInternet_Click);
            // 
            // buttonPullFromInternet
            // 
            this.buttonPullFromInternet.Enabled = false;
            this.buttonPullFromInternet.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_movedown1;
            this.buttonPullFromInternet.Location = new System.Drawing.Point(595, 78);
            this.buttonPullFromInternet.Name = "buttonPullFromInternet";
            this.buttonPullFromInternet.Size = new System.Drawing.Size(24, 23);
            this.buttonPullFromInternet.TabIndex = 6;
            this.toolTip.SetToolTip(this.buttonPullFromInternet, "Click to download the shared project from the internet");
            this.buttonPullFromInternet.UseVisualStyleBackColor = true;
            this.buttonPullFromInternet.Click += new System.EventHandler(this.buttonPullFromInternet_Click);
            // 
            // buttonPushToNetwork
            // 
            this.buttonPushToNetwork.Enabled = false;
            this.buttonPushToNetwork.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_moveup1;
            this.buttonPushToNetwork.Location = new System.Drawing.Point(595, 157);
            this.buttonPushToNetwork.Name = "buttonPushToNetwork";
            this.buttonPushToNetwork.Size = new System.Drawing.Size(24, 21);
            this.buttonPushToNetwork.TabIndex = 11;
            this.toolTip.SetToolTip(this.buttonPushToNetwork, "Click to upload the shared project to the network shared folder");
            this.buttonPushToNetwork.UseVisualStyleBackColor = true;
            this.buttonPushToNetwork.Click += new System.EventHandler(this.buttonPushToNetwork_Click);
            // 
            // labelProjectName
            // 
            this.labelProjectName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelProjectName.AutoSize = true;
            this.labelProjectName.Location = new System.Drawing.Point(27, 6);
            this.labelProjectName.Name = "labelProjectName";
            this.labelProjectName.Size = new System.Drawing.Size(74, 13);
            this.labelProjectName.TabIndex = 0;
            this.labelProjectName.Text = "Project Name:";
            // 
            // textBoxProjectName
            // 
            this.textBoxProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxProjectName.Location = new System.Drawing.Point(107, 3);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(482, 20);
            this.textBoxProjectName.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxProjectName, resources.GetString("textBoxProjectName.ToolTip"));
            this.textBoxProjectName.TextChanged += new System.EventHandler(this.textBoxProjectName_TextChanged);
            // 
            // labelFullInternetUrl
            // 
            this.labelFullInternetUrl.AutoSize = true;
            this.labelFullInternetUrl.Location = new System.Drawing.Point(107, 75);
            this.labelFullInternetUrl.Name = "labelFullInternetUrl";
            this.labelFullInternetUrl.Size = new System.Drawing.Size(0, 13);
            this.labelFullInternetUrl.TabIndex = 5;
            // 
            // comboBoxServer
            // 
            this.comboBoxServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxServer.FormattingEnabled = true;
            this.comboBoxServer.Location = new System.Drawing.Point(107, 49);
            this.comboBoxServer.Name = "comboBoxServer";
            this.comboBoxServer.Size = new System.Drawing.Size(482, 21);
            this.comboBoxServer.TabIndex = 3;
            this.comboBoxServer.SelectedIndexChanged += new System.EventHandler(this.comboBoxServer_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(245, 250);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 250);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // AiRepoSelectionForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(647, 285);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AiRepoSelectionForm";
            this.Text = "Adapt It Repository";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxInternet;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkBoxNetwork;
        private System.Windows.Forms.TextBox textBoxNetwork;
        private System.Windows.Forms.Label labelNetworkPath;
        private System.Windows.Forms.Button buttonBrowseNetwork;
        private System.Windows.Forms.Button buttonPushToInternet;
        private System.Windows.Forms.Button buttonPullFromInternet;
        private System.Windows.Forms.Button buttonPushToNetwork;
        private System.Windows.Forms.Label labelProjectName;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Label labelFullInternetUrl;
        private System.Windows.Forms.ComboBox comboBoxServer;
    }
}