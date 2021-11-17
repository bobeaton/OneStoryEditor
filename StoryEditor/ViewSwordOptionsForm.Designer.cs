using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    partial class ViewSwordOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSwordOptionsForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.linkLabelLinkToSword = new System.Windows.Forms.LinkLabel();
            this.labelDownloadProgress = new System.Windows.Forms.Label();
            this.backgroundWorkerLoadDownloadListBox = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkedListBoxSwordBibles = new System.Windows.Forms.CheckedListBox();
            this.listViewSwordModules = new System.Windows.Forms.ListView();
            this.tabPageDownloadTree = new System.Windows.Forms.TabPage();
            this.treeViewSourcesDownloadable = new System.Windows.Forms.TreeView();
            this.labelFilter = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageDownloadTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AutoSize = true;
            this.buttonOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOK.Location = new System.Drawing.Point(192, 372);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 26);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonCancel.Location = new System.Drawing.Point(273, 372);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 26);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // linkLabelLinkToSword
            // 
            this.linkLabelLinkToSword.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelLinkToSword, 2);
            this.linkLabelLinkToSword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabelLinkToSword.Location = new System.Drawing.Point(3, 352);
            this.linkLabelLinkToSword.Name = "linkLabelLinkToSword";
            this.linkLabelLinkToSword.Size = new System.Drawing.Size(534, 17);
            this.linkLabelLinkToSword.TabIndex = 3;
            this.linkLabelLinkToSword.TabStop = true;
            this.linkLabelLinkToSword.Text = "Click here to learn about other Bible versions you can download and use";
            this.linkLabelLinkToSword.UseCompatibleTextRendering = true;
            this.linkLabelLinkToSword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLinkToSword_LinkClicked);
            // 
            // labelDownloadProgress
            // 
            this.labelDownloadProgress.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelDownloadProgress, 2);
            this.labelDownloadProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDownloadProgress.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelDownloadProgress.Location = new System.Drawing.Point(3, 317);
            this.labelDownloadProgress.Name = "labelDownloadProgress";
            this.labelDownloadProgress.Size = new System.Drawing.Size(534, 35);
            this.labelDownloadProgress.TabIndex = 5;
            this.labelDownloadProgress.Text = "label";
            // 
            // backgroundWorkerLoadDownloadListBox
            // 
            this.backgroundWorkerLoadDownloadListBox.WorkerReportsProgress = true;
            this.backgroundWorkerLoadDownloadListBox.WorkerSupportsCancellation = true;
            this.backgroundWorkerLoadDownloadListBox.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoadDownloadListBox_DoWork);
            this.backgroundWorkerLoadDownloadListBox.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerLoadDownloadListBox_ProgressChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelDownloadProgress, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelLinkToSword, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonOK, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFilter, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelFilter, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(540, 401);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFilter.Location = new System.Drawing.Point(273, 3);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(264, 20);
            this.textBoxFilter.TabIndex = 6;
            this.textBoxFilter.Visible = false;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // tabControl
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl, 2);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPageDownloadTree);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 29);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(534, 285);
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkedListBoxSwordBibles);
            this.tabPage1.Controls.Add(this.listViewSwordModules);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(526, 259);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Installed";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxSwordBibles
            // 
            this.checkedListBoxSwordBibles.CheckOnClick = true;
            this.checkedListBoxSwordBibles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxSwordBibles.FormattingEnabled = true;
            this.checkedListBoxSwordBibles.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxSwordBibles.Name = "checkedListBoxSwordBibles";
            this.checkedListBoxSwordBibles.Size = new System.Drawing.Size(520, 253);
            this.checkedListBoxSwordBibles.Sorted = true;
            this.checkedListBoxSwordBibles.TabIndex = 2;
            // 
            // listViewSwordModules
            // 
            this.listViewSwordModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSwordModules.HideSelection = false;
            this.listViewSwordModules.Location = new System.Drawing.Point(3, 3);
            this.listViewSwordModules.Name = "listViewSwordModules";
            this.listViewSwordModules.Size = new System.Drawing.Size(520, 253);
            this.listViewSwordModules.TabIndex = 0;
            this.listViewSwordModules.UseCompatibleStateImageBehavior = false;
            // 
            // tabPageDownloadTree
            // 
            this.tabPageDownloadTree.Controls.Add(this.treeViewSourcesDownloadable);
            this.tabPageDownloadTree.Location = new System.Drawing.Point(4, 22);
            this.tabPageDownloadTree.Name = "tabPageDownloadTree";
            this.tabPageDownloadTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDownloadTree.Size = new System.Drawing.Size(526, 259);
            this.tabPageDownloadTree.TabIndex = 1;
            this.tabPageDownloadTree.Text = "Download";
            this.tabPageDownloadTree.UseVisualStyleBackColor = true;
            // 
            // treeViewSourcesDownloadable
            // 
            this.treeViewSourcesDownloadable.CheckBoxes = true;
            this.treeViewSourcesDownloadable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSourcesDownloadable.Location = new System.Drawing.Point(3, 3);
            this.treeViewSourcesDownloadable.Name = "treeViewSourcesDownloadable";
            this.treeViewSourcesDownloadable.Size = new System.Drawing.Size(520, 253);
            this.treeViewSourcesDownloadable.TabIndex = 0;
            this.treeViewSourcesDownloadable.Visible = false;
            this.treeViewSourcesDownloadable.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSourcesDownloadable_AfterCheck);
            // 
            // labelFilter
            // 
            this.labelFilter.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelFilter.AutoSize = true;
            this.labelFilter.Location = new System.Drawing.Point(198, 6);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(69, 13);
            this.labelFilter.TabIndex = 7;
            this.labelFilter.Text = "Search &Filter:";
            this.labelFilter.Visible = false;
            // 
            // ViewSwordOptionsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(564, 425);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewSwordOptionsForm";
            this.Text = "Sword Resources (Biblical Texts, etc.)";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPageDownloadTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.LinkLabel linkLabelLinkToSword;
        private System.Windows.Forms.Label labelDownloadProgress;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadDownloadListBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TabControl tabControl;
        private TabPage tabPage1;
        private ListView listViewSwordModules;
        private TabPage tabPageDownloadTree;
        private TreeView treeViewSourcesDownloadable;
        private CheckedListBox checkedListBoxSwordBibles;
        private TextBox textBoxFilter;
        private Label labelFilter;
    }
}