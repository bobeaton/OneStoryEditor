using EditResxLocalization.Properties;

namespace EditResxLocalization
{
    partial class FormEditResx
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportMerge = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportMergeFromResx = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnEnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "resx";
            this.openFileDialog.FileName = "Resources.resx";
            this.openFileDialog.Filter = "Resource Files|*.resx";
            this.openFileDialog.Title = "Browse for the Resource (.resx) file";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "elf";
            this.saveFileDialog.Filter = "Localization Files|*.elf";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(702, 320);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuImportMerge,
            this.menuLanguage});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(702, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSaveAs,
            this.toolStripSeparator1,
            this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = global::EditResxLocalization.Properties.Resources.IDS__File;
            this.menuFile.DropDownOpening += new System.EventHandler(this.MenuFileDropDownOpening);
            // 
            // menuFileNew
            // 
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.Size = new System.Drawing.Size(138, 22);
            this.menuFileNew.Text = global::EditResxLocalization.Properties.Resources.IDS__New;
            this.menuFileNew.ToolTipText = "Create a new (empty) Localization File (*.elf)";
            this.menuFileNew.Click += new System.EventHandler(this.MenuFileNewClick);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.Size = new System.Drawing.Size(138, 22);
            this.menuFileOpen.Text = global::EditResxLocalization.Properties.Resources.IDS__Open;
            this.menuFileOpen.ToolTipText = "Open a Localization File (*.elf)";
            this.menuFileOpen.Click += new System.EventHandler(this.MenuFileOpenClick);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuFileSave.Size = new System.Drawing.Size(138, 22);
            this.menuFileSave.Text = global::EditResxLocalization.Properties.Resources.IDS__Save;
            this.menuFileSave.ToolTipText = "Save the Localization File (*.elf)";
            this.menuFileSave.Click += new System.EventHandler(this.MenuFileSaveClick);
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileSaveAs.Size = new System.Drawing.Size(138, 22);
            this.menuFileSaveAs.Text = global::EditResxLocalization.Properties.Resources.IDS_Save__As;
            this.menuFileSaveAs.ToolTipText = "Save the Localization File (*.elf) with a new name";
            this.menuFileSaveAs.Click += new System.EventHandler(this.MenuFileSaveAsClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(135, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menuFileExit.Size = new System.Drawing.Size(138, 22);
            this.menuFileExit.Text = global::EditResxLocalization.Properties.Resources.IDS_Exit;
            this.menuFileExit.ToolTipText = "Exit the program";
            // 
            // menuImportMerge
            // 
            this.menuImportMerge.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImportMergeFromResx});
            this.menuImportMerge.Name = "menuImportMerge";
            this.menuImportMerge.Size = new System.Drawing.Size(94, 20);
            this.menuImportMerge.Text = global::EditResxLocalization.Properties.Resources.IDS__Import_Merge;
            this.menuImportMerge.DropDownOpening += new System.EventHandler(this.MenuImportMergeDropDownOpening);
            // 
            // menuImportMergeFromResx
            // 
            this.menuImportMergeFromResx.Name = "menuImportMergeFromResx";
            this.menuImportMergeFromResx.Size = new System.Drawing.Size(133, 22);
            this.menuImportMergeFromResx.Text = global::EditResxLocalization.Properties.Resources.IDS__From___resx;
            this.menuImportMergeFromResx.ToolTipText = "Import and merge the information in a .resx file into the current Localization Fi" +
                "le";
            this.menuImportMergeFromResx.Click += new System.EventHandler(this.MenuImportMergeFromResxClick);
            // 
            // menuLanguage
            // 
            this.menuLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localizationToolStripMenuItem});
            this.menuLanguage.Name = "menuLanguage";
            this.menuLanguage.Size = new System.Drawing.Size(71, 20);
            this.menuLanguage.Text = "&Language";
            this.menuLanguage.DropDownOpening += new System.EventHandler(this.MenuLanguageDropDownOpening);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnId,
            this.ColumnEnValue,
            this.ColumnTranslation});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 24);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(702, 296);
            this.dataGridView.TabIndex = 3;
            // 
            // ColumnId
            // 
            this.ColumnId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnId.HeaderText = "Key";
            this.ColumnId.Name = "ColumnId";
            this.ColumnId.ReadOnly = true;
            // 
            // ColumnEnValue
            // 
            this.ColumnEnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnEnValue.FillWeight = 200F;
            this.ColumnEnValue.HeaderText = "Default Value";
            this.ColumnEnValue.Name = "ColumnEnValue";
            // 
            // ColumnTranslation
            // 
            this.ColumnTranslation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnTranslation.FillWeight = 200F;
            this.ColumnTranslation.HeaderText = "Translation";
            this.ColumnTranslation.Name = "ColumnTranslation";
            // 
            // localizationToolStripMenuItem
            // 
            this.localizationToolStripMenuItem.Name = "localizationToolStripMenuItem";
            this.localizationToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.localizationToolStripMenuItem.Text = "Localization...";
            this.localizationToolStripMenuItem.Click += new System.EventHandler(this.localizationToolStripMenuItem_Click);
            // 
            // FormEditResx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 320);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.menuStrip);
            this.Name = "FormEditResx";
            this.Text = "Edit Localization Resources";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuImportMerge;
        private System.Windows.Forms.ToolStripMenuItem menuImportMergeFromResx;
        private System.Windows.Forms.ToolStripMenuItem menuLanguage;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnEnValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTranslation;
        private System.Windows.Forms.ToolStripMenuItem localizationToolStripMenuItem;
    }
}

