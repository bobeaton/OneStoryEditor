namespace NetLoc
{
	partial class LocDataEditorForm
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
            this.uiMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiFileAddLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.uiFileDeleteLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.placeholderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiFileCopyLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.uiFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.uiFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lMXFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.exportKeyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToKeyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeUnusedTranslationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useGoogleAutoTranslatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useMicrosoftAutoTranslatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.uiAutoTranslate = new System.Windows.Forms.ToolStripMenuItem();
            this.placeholderToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.uiFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.uiFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.uiLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.placeholderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uiSearch = new System.Windows.Forms.ToolStripTextBox();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiSaveXmlFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.uiOpenXmlFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.uiOpenLmxFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.uiProgress = new System.Windows.Forms.ProgressBar();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.uiLocDataControl = new NetLoc.LocDataControl();
            this.uiMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiMenu
            // 
            this.uiMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.uiLanguage,
            this.uiSearch,
            this.fontToolStripMenuItem});
            this.uiMenu.Location = new System.Drawing.Point(0, 0);
            this.uiMenu.Name = "uiMenu";
            this.uiMenu.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.uiMenu.Size = new System.Drawing.Size(710, 27);
            this.uiMenu.TabIndex = 0;
            this.uiMenu.Text = "uiMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiFileAddLanguage,
            this.uiFileDeleteLanguage,
            this.uiFileCopyLanguage,
            this.toolStripSeparator1,
            this.uiFileImport,
            this.uiFileExport,
            this.toolStripMenuItem1,
            this.uiAdvanced,
            this.toolStripSeparator3,
            this.uiAutoTranslate,
            this.toolStripSeparator2,
            this.uiFileSave,
            this.uiFileClose});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // uiFileAddLanguage
            // 
            this.uiFileAddLanguage.Name = "uiFileAddLanguage";
            this.uiFileAddLanguage.Size = new System.Drawing.Size(214, 22);
            this.uiFileAddLanguage.Text = "Add Language...";
            this.uiFileAddLanguage.Click += new System.EventHandler(this.uiFileAddLanguage_Click);
            // 
            // uiFileDeleteLanguage
            // 
            this.uiFileDeleteLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeholderToolStripMenuItem});
            this.uiFileDeleteLanguage.Name = "uiFileDeleteLanguage";
            this.uiFileDeleteLanguage.Size = new System.Drawing.Size(214, 22);
            this.uiFileDeleteLanguage.Text = "Delete Language";
            this.uiFileDeleteLanguage.DropDownOpening += new System.EventHandler(this.uiFileDeleteLanguage_DropDownOpening);
            // 
            // placeholderToolStripMenuItem
            // 
            this.placeholderToolStripMenuItem.Name = "placeholderToolStripMenuItem";
            this.placeholderToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.placeholderToolStripMenuItem.Text = "(placeholder)";
            // 
            // uiFileCopyLanguage
            // 
            this.uiFileCopyLanguage.Name = "uiFileCopyLanguage";
            this.uiFileCopyLanguage.Size = new System.Drawing.Size(214, 22);
            this.uiFileCopyLanguage.Text = "Copy Language...";
            this.uiFileCopyLanguage.Click += new System.EventHandler(this.uiFileCopyLanguage_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // uiFileImport
            // 
            this.uiFileImport.Name = "uiFileImport";
            this.uiFileImport.Size = new System.Drawing.Size(214, 22);
            this.uiFileImport.Text = "Import Language...";
            this.uiFileImport.Click += new System.EventHandler(this.uiFileImport_Click);
            // 
            // uiFileExport
            // 
            this.uiFileExport.Name = "uiFileExport";
            this.uiFileExport.Size = new System.Drawing.Size(214, 22);
            this.uiFileExport.Text = "Export Current Language...";
            this.uiFileExport.Click += new System.EventHandler(this.uiFileExport_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lMXFileToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuItem1.Text = "Import From";
            this.toolStripMenuItem1.Visible = false;
            // 
            // lMXFileToolStripMenuItem
            // 
            this.lMXFileToolStripMenuItem.Name = "lMXFileToolStripMenuItem";
            this.lMXFileToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.lMXFileToolStripMenuItem.Text = "LMX File";
            this.lMXFileToolStripMenuItem.Click += new System.EventHandler(this.lMXFileToolStripMenuItem_Click);
            // 
            // uiAdvanced
            // 
            this.uiAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportKeyListToolStripMenuItem,
            this.compareToKeyListToolStripMenuItem,
            this.removeUnusedTranslationsToolStripMenuItem,
            this.useGoogleAutoTranslatorToolStripMenuItem,
            this.useMicrosoftAutoTranslatorToolStripMenuItem});
            this.uiAdvanced.Name = "uiAdvanced";
            this.uiAdvanced.Size = new System.Drawing.Size(214, 22);
            this.uiAdvanced.Text = "Advanced";
            this.uiAdvanced.DropDownOpening += new System.EventHandler(this.uiAdvanced_DropDownOpening);
            // 
            // exportKeyListToolStripMenuItem
            // 
            this.exportKeyListToolStripMenuItem.Name = "exportKeyListToolStripMenuItem";
            this.exportKeyListToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.exportKeyListToolStripMenuItem.Text = "Export Key List...";
            this.exportKeyListToolStripMenuItem.Click += new System.EventHandler(this.exportKeyListToolStripMenuItem_Click);
            // 
            // compareToKeyListToolStripMenuItem
            // 
            this.compareToKeyListToolStripMenuItem.Name = "compareToKeyListToolStripMenuItem";
            this.compareToKeyListToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.compareToKeyListToolStripMenuItem.Text = "Compare to Key List...";
            this.compareToKeyListToolStripMenuItem.Click += new System.EventHandler(this.compareToKeyListToolStripMenuItem_Click);
            // 
            // removeUnusedTranslationsToolStripMenuItem
            // 
            this.removeUnusedTranslationsToolStripMenuItem.Name = "removeUnusedTranslationsToolStripMenuItem";
            this.removeUnusedTranslationsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.removeUnusedTranslationsToolStripMenuItem.Text = "Remove Unused Translations";
            this.removeUnusedTranslationsToolStripMenuItem.Click += new System.EventHandler(this.removeUnusedTranslationsToolStripMenuItem_Click);
            // 
            // useGoogleAutoTranslatorToolStripMenuItem
            // 
            this.useGoogleAutoTranslatorToolStripMenuItem.Name = "useGoogleAutoTranslatorToolStripMenuItem";
            this.useGoogleAutoTranslatorToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.useGoogleAutoTranslatorToolStripMenuItem.Text = "Use Google Auto-Translator";
            this.useGoogleAutoTranslatorToolStripMenuItem.Click += new System.EventHandler(this.useGoogleAutoTranslatorToolStripMenuItem_Click);
            // 
            // useMicrosoftAutoTranslatorToolStripMenuItem
            // 
            this.useMicrosoftAutoTranslatorToolStripMenuItem.Name = "useMicrosoftAutoTranslatorToolStripMenuItem";
            this.useMicrosoftAutoTranslatorToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.useMicrosoftAutoTranslatorToolStripMenuItem.Text = "Use Microsoft Auto-Translator";
            this.useMicrosoftAutoTranslatorToolStripMenuItem.Click += new System.EventHandler(this.useMicrosoftAutoTranslatorToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(211, 6);
            // 
            // uiAutoTranslate
            // 
            this.uiAutoTranslate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeholderToolStripMenuItem2});
            this.uiAutoTranslate.Name = "uiAutoTranslate";
            this.uiAutoTranslate.Size = new System.Drawing.Size(214, 22);
            this.uiAutoTranslate.Text = "Auto Translate From";
            this.uiAutoTranslate.DropDownOpening += new System.EventHandler(this.uiAutoTranslate_DropDownOpening);
            // 
            // placeholderToolStripMenuItem2
            // 
            this.placeholderToolStripMenuItem2.Name = "placeholderToolStripMenuItem2";
            this.placeholderToolStripMenuItem2.Size = new System.Drawing.Size(144, 22);
            this.placeholderToolStripMenuItem2.Text = "(placeholder)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // uiFileSave
            // 
            this.uiFileSave.Name = "uiFileSave";
            this.uiFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.uiFileSave.Size = new System.Drawing.Size(214, 22);
            this.uiFileSave.Text = "Save";
            this.uiFileSave.Click += new System.EventHandler(this.uiFileSave_Click);
            // 
            // uiFileClose
            // 
            this.uiFileClose.Name = "uiFileClose";
            this.uiFileClose.Size = new System.Drawing.Size(214, 22);
            this.uiFileClose.Text = "Close";
            this.uiFileClose.Click += new System.EventHandler(this.uiFileClose_Click);
            // 
            // uiLanguage
            // 
            this.uiLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeholderToolStripMenuItem1});
            this.uiLanguage.Name = "uiLanguage";
            this.uiLanguage.Size = new System.Drawing.Size(71, 23);
            this.uiLanguage.Text = "&Language";
            this.uiLanguage.DropDownOpening += new System.EventHandler(this.uiLanguage_DropDownOpening);
            // 
            // placeholderToolStripMenuItem1
            // 
            this.placeholderToolStripMenuItem1.Name = "placeholderToolStripMenuItem1";
            this.placeholderToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.placeholderToolStripMenuItem1.Text = "(placeholder)";
            // 
            // uiSearch
            // 
            this.uiSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.uiSearch.ForeColor = System.Drawing.Color.DarkGray;
            this.uiSearch.Name = "uiSearch";
            this.uiSearch.Size = new System.Drawing.Size(100, 23);
            this.uiSearch.Text = "Search <Enter>";
            this.uiSearch.Leave += new System.EventHandler(this.uiSearch_Leave);
            this.uiSearch.Enter += new System.EventHandler(this.uiSearch_Enter);
            this.uiSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.uiSearch_KeyPress);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(149, 23);
            this.fontToolStripMenuItem.Text = "&Change Translation Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // uiSaveXmlFileDialog
            // 
            this.uiSaveXmlFileDialog.Filter = "XML Files|*.xml";
            // 
            // uiOpenXmlFileDialog
            // 
            this.uiOpenXmlFileDialog.Filter = "XML Files|*.xml";
            // 
            // uiOpenLmxFileDialog
            // 
            this.uiOpenLmxFileDialog.Filter = "LMX Files|*.lmx";
            // 
            // uiProgress
            // 
            this.uiProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiProgress.Location = new System.Drawing.Point(0, 662);
            this.uiProgress.Maximum = 1000;
            this.uiProgress.Name = "uiProgress";
            this.uiProgress.Size = new System.Drawing.Size(710, 16);
            this.uiProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.uiProgress.TabIndex = 2;
            this.uiProgress.Visible = false;
            // 
            // uiLocDataControl
            // 
            this.uiLocDataControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLocDataControl.Location = new System.Drawing.Point(0, 27);
            this.uiLocDataControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.uiLocDataControl.Name = "uiLocDataControl";
            this.uiLocDataControl.Size = new System.Drawing.Size(710, 651);
            this.uiLocDataControl.TabIndex = 1;
            // 
            // LocDataEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 678);
            this.Controls.Add(this.uiProgress);
            this.Controls.Add(this.uiLocDataControl);
            this.Controls.Add(this.uiMenu);
            this.MainMenuStrip = this.uiMenu;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LocDataEditorForm";
            this.Text = "Localizer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocDataEditorForm_FormClosing);
            this.uiMenu.ResumeLayout(false);
            this.uiMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip uiMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem uiFileSave;
		private System.Windows.Forms.ToolStripMenuItem uiFileClose;
		private System.Windows.Forms.ToolStripMenuItem uiLanguage;
		private LocDataControl uiLocDataControl;
		private System.Windows.Forms.ToolStripMenuItem uiFileAddLanguage;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem uiFileDeleteLanguage;
		private System.Windows.Forms.ToolStripMenuItem placeholderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem placeholderToolStripMenuItem1;
		private System.Windows.Forms.SaveFileDialog uiSaveXmlFileDialog;
		private System.Windows.Forms.ToolStripMenuItem uiFileExport;
		private System.Windows.Forms.ToolStripMenuItem uiFileImport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.OpenFileDialog uiOpenXmlFileDialog;
		private System.Windows.Forms.ToolStripMenuItem uiFileCopyLanguage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem lMXFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog uiOpenLmxFileDialog;
        private System.Windows.Forms.ToolStripTextBox uiSearch;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar uiProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem uiAutoTranslate;
        private System.Windows.Forms.ToolStripMenuItem placeholderToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem uiAdvanced;
        private System.Windows.Forms.ToolStripMenuItem exportKeyListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareToKeyListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUnusedTranslationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useGoogleAutoTranslatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useMicrosoftAutoTranslatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog;
	}
}