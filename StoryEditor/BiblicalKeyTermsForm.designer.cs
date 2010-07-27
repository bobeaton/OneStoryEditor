namespace OneStoryProjectEditor
{
    partial class BiblicalKeyTermsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiblicalKeyTermsForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewKeyTerms = new System.Windows.Forms.DataGridView();
            this.ColumnTermLemma = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGlossEnglish = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRenderings = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddKeyTerm = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditKeyTerm = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditRenderings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelectKeyTermsList = new System.Windows.Forms.ToolStripButton();
            this.progressBarLoadingKeyTerms = new System.Windows.Forms.ProgressBar();
            this.webBrowser = new onlyconnect.HtmlEditor();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.textBoxInstructions = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeyTerms)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewKeyTerms);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.progressBarLoadingKeyTerms);
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(602, 327);
            this.splitContainer1.SplitterDistance = 163;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataGridViewKeyTerms
            // 
            this.dataGridViewKeyTerms.AllowDrop = true;
            this.dataGridViewKeyTerms.AllowUserToAddRows = false;
            this.dataGridViewKeyTerms.AllowUserToDeleteRows = false;
            this.dataGridViewKeyTerms.AllowUserToOrderColumns = true;
            this.dataGridViewKeyTerms.AllowUserToResizeRows = false;
            this.dataGridViewKeyTerms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKeyTerms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTermLemma,
            this.ColumnStatus,
            this.ColumnGlossEnglish,
            this.ColumnRenderings});
            this.dataGridViewKeyTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewKeyTerms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.helpProvider.SetHelpString(this.dataGridViewKeyTerms, resources.GetString("dataGridViewKeyTerms.HelpString"));
            this.dataGridViewKeyTerms.Location = new System.Drawing.Point(0, 25);
            this.dataGridViewKeyTerms.MultiSelect = false;
            this.dataGridViewKeyTerms.Name = "dataGridViewKeyTerms";
            this.dataGridViewKeyTerms.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridViewKeyTerms.RowHeadersVisible = false;
            this.dataGridViewKeyTerms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.helpProvider.SetShowHelp(this.dataGridViewKeyTerms, true);
            this.dataGridViewKeyTerms.Size = new System.Drawing.Size(602, 138);
            this.dataGridViewKeyTerms.TabIndex = 0;
            this.dataGridViewKeyTerms.VirtualMode = true;
            this.dataGridViewKeyTerms.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKeyTerms_CellDoubleClick);
            this.dataGridViewKeyTerms.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewKeyTerms_CellValueNeeded);
            this.dataGridViewKeyTerms.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridViewKeyTerms_DragOver);
            this.dataGridViewKeyTerms.SelectionChanged += new System.EventHandler(this.dataGridViewKeyTerms_SelectionChanged);
            this.dataGridViewKeyTerms.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewKeyTerms_DragDrop);
            // 
            // ColumnTermLemma
            // 
            this.ColumnTermLemma.HeaderText = "Key Term";
            this.ColumnTermLemma.Name = "ColumnTermLemma";
            this.ColumnTermLemma.ReadOnly = true;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.HeaderText = "Status";
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            // 
            // ColumnGlossEnglish
            // 
            this.ColumnGlossEnglish.HeaderText = "English Gloss";
            this.ColumnGlossEnglish.Name = "ColumnGlossEnglish";
            this.ColumnGlossEnglish.ReadOnly = true;
            // 
            // ColumnRenderings
            // 
            this.ColumnRenderings.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnRenderings.HeaderText = "Renderings";
            this.ColumnRenderings.Name = "ColumnRenderings";
            this.ColumnRenderings.ReadOnly = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditRenderings,
            this.toolStripButtonSelectKeyTermsList,
            this.toolStripButtonAddKeyTerm,
            this.toolStripButtonEditKeyTerm});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(602, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddKeyTerm
            // 
            this.toolStripButtonAddKeyTerm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddKeyTerm.Image = global::OneStoryProjectEditor.Properties.Resources.AddTableHS;
            this.toolStripButtonAddKeyTerm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddKeyTerm.Name = "toolStripButtonAddKeyTerm";
            this.toolStripButtonAddKeyTerm.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddKeyTerm.Text = "Add Key Term";
            this.toolStripButtonAddKeyTerm.ToolTipText = "Click this button to add a new key term";
            this.toolStripButtonAddKeyTerm.Click += new System.EventHandler(this.toolStripButtonAddKeyTerm_Click);
            // 
            // toolStripButtonEditKeyTerm
            // 
            this.toolStripButtonEditKeyTerm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditKeyTerm.Image = global::OneStoryProjectEditor.Properties.Resources.EditTableHS;
            this.toolStripButtonEditKeyTerm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditKeyTerm.Name = "toolStripButtonEditKeyTerm";
            this.toolStripButtonEditKeyTerm.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditKeyTerm.Text = "Edit Key Term";
            this.toolStripButtonEditKeyTerm.ToolTipText = "Click this button to edit the selected key term (e.g. to add references to it)";
            this.toolStripButtonEditKeyTerm.Click += new System.EventHandler(this.toolStripButtonEditKeyTerm_Click);
            // 
            // toolStripButtonEditRenderings
            // 
            this.toolStripButtonEditRenderings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditRenderings.Image = global::OneStoryProjectEditor.Properties.Resources.EditInformationHS;
            this.toolStripButtonEditRenderings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditRenderings.Name = "toolStripButtonEditRenderings";
            this.toolStripButtonEditRenderings.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditRenderings.Text = "Edit Renderings";
            this.toolStripButtonEditRenderings.ToolTipText = "Click this button to edit the renderings for the selected key term";
            this.toolStripButtonEditRenderings.Click += new System.EventHandler(this.toolStripButtonEditRenderings_Click);
            // 
            // toolStripButtonSelectKeyTermsList
            // 
            this.toolStripButtonSelectKeyTermsList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelectKeyTermsList.Image = global::OneStoryProjectEditor.Properties.Resources.OpenSelectedItemHS;
            this.toolStripButtonSelectKeyTermsList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelectKeyTermsList.Name = "toolStripButtonSelectKeyTermsList";
            this.toolStripButtonSelectKeyTermsList.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSelectKeyTermsList.Text = "Select Key Terms List";
            this.toolStripButtonSelectKeyTermsList.ToolTipText = "Click this button to select which key terms list you want to use";
            this.toolStripButtonSelectKeyTermsList.Click += new System.EventHandler(this.toolStripButtonSelectKeyTermsList_Click);
            // 
            // progressBarLoadingKeyTerms
            // 
            this.progressBarLoadingKeyTerms.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBarLoadingKeyTerms.Location = new System.Drawing.Point(0, 137);
            this.progressBarLoadingKeyTerms.Name = "progressBarLoadingKeyTerms";
            this.progressBarLoadingKeyTerms.Size = new System.Drawing.Size(602, 23);
            this.progressBarLoadingKeyTerms.TabIndex = 1;
            // 
            // webBrowser
            // 
            this.webBrowser.DefaultComposeSettings.BackColor = System.Drawing.Color.White;
            this.webBrowser.DefaultComposeSettings.DefaultFont = new System.Drawing.Font("Arial", 10F);
            this.webBrowser.DefaultComposeSettings.Enabled = false;
            this.webBrowser.DefaultComposeSettings.ForeColor = System.Drawing.Color.Black;
            this.webBrowser.DefaultPreamble = onlyconnect.EncodingType.UTF8;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.DocumentEncoding = onlyconnect.EncodingType.WindowsCurrent;
            this.helpProvider.SetHelpString(this.webBrowser, resources.GetString("webBrowser.HelpString"));
            this.webBrowser.IsActivationEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.webBrowser.SelectionBackColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionBullets = false;
            this.webBrowser.SelectionFont = null;
            this.webBrowser.SelectionForeColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionNumbering = false;
            this.helpProvider.SetShowHelp(this.webBrowser, true);
            this.webBrowser.Size = new System.Drawing.Size(602, 160);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.BeforeNavigate += new onlyconnect.BeforeNavigateEventHandler(this.webBrowser_BeforeNavigate);
            this.webBrowser.MouseMove += new System.Windows.Forms.MouseEventHandler(this.webBrowser_MouseMove);
            this.webBrowser.KeyDown += new System.Windows.Forms.KeyEventHandler(this.webBrowser_KeyDown);
            this.webBrowser.ReadyStateChanged += new onlyconnect.ReadyStateChangedHandler(this.webBrowser_ReadyStateChanged);
            // 
            // textBoxInstructions
            // 
            this.textBoxInstructions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxInstructions.Location = new System.Drawing.Point(0, 333);
            this.textBoxInstructions.Multiline = true;
            this.textBoxInstructions.Name = "textBoxInstructions";
            this.textBoxInstructions.Size = new System.Drawing.Size(602, 75);
            this.textBoxInstructions.TabIndex = 2;
            this.textBoxInstructions.Text = resources.GetString("textBoxInstructions.Text");
            // 
            // BiblicalKeyTermsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 408);
            this.Controls.Add(this.textBoxInstructions);
            this.Controls.Add(this.splitContainer1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BiblicalKeyTermsForm";
            this.Text = "Key Terms";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BiblicalKeyTermsForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeyTerms)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewKeyTerms;
        private onlyconnect.HtmlEditor webBrowser;
        private System.Windows.Forms.ProgressBar progressBarLoadingKeyTerms;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTermLemma;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnGlossEnglish;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRenderings;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.TextBox textBoxInstructions;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddKeyTerm;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditRenderings;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditKeyTerm;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelectKeyTermsList;

    }
}

