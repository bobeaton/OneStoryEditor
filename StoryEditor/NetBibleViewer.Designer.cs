namespace OneStoryProjectEditor
{
    partial class NetBibleViewer
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.textBoxSizer = new System.Windows.Forms.TextBox();
            this.webBrowserNetBible = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpinControls = new OneStoryProjectEditor.DynamicTableLayoutPanel();
            this.domainUpDownBookNames = new System.Windows.Forms.DomainUpDown();
            this.numericUpDownChapterNumber = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownVerseNumber = new System.Windows.Forms.NumericUpDown();
            this.textBoxNetFlixViewer = new System.Windows.Forms.TextBox();
            this.radioButtonShowOtherSwordResources = new System.Windows.Forms.RadioButton();
            this.checkBoxAutoHide = new System.Windows.Forms.CheckBox();
            this.contextMenuStripBibleBooks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanelSpinControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChapterNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVerseNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // textBoxSizer
            // 
            this.textBoxSizer.Location = new System.Drawing.Point(0, 0);
            this.textBoxSizer.Multiline = true;
            this.textBoxSizer.Name = "textBoxSizer";
            this.textBoxSizer.Size = new System.Drawing.Size(100, 20);
            this.textBoxSizer.TabIndex = 1;
            // 
            // webBrowserNetBible
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webBrowserNetBible, 2);
            this.webBrowserNetBible.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserNetBible.Location = new System.Drawing.Point(3, 34);
            this.webBrowserNetBible.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserNetBible.Name = "webBrowserNetBible";
            this.webBrowserNetBible.Size = new System.Drawing.Size(577, 225);
            this.webBrowserNetBible.TabIndex = 1;
            this.webBrowserNetBible.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserNetBible_DocumentCompleted);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.webBrowserNetBible, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelSpinControls, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.checkBoxAutoHide, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(583, 262);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanelSpinControls
            // 
            this.tableLayoutPanelSpinControls.ColumnCount = 5;
            this.tableLayoutPanelSpinControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSpinControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSpinControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSpinControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpinControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSpinControls.Controls.Add(this.domainUpDownBookNames, 0, 0);
            this.tableLayoutPanelSpinControls.Controls.Add(this.numericUpDownChapterNumber, 1, 0);
            this.tableLayoutPanelSpinControls.Controls.Add(this.numericUpDownVerseNumber, 2, 0);
            this.tableLayoutPanelSpinControls.Controls.Add(this.textBoxNetFlixViewer, 3, 0);
            this.tableLayoutPanelSpinControls.Controls.Add(this.radioButtonShowOtherSwordResources, 4, 0);
            this.tableLayoutPanelSpinControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSpinControls.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelSpinControls.Name = "tableLayoutPanelSpinControls";
            this.tableLayoutPanelSpinControls.RowCount = 1;
            this.tableLayoutPanelSpinControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSpinControls.Size = new System.Drawing.Size(557, 25);
            this.tableLayoutPanelSpinControls.TabIndex = 2;
            // 
            // domainUpDownBookNames
            // 
            this.domainUpDownBookNames.AutoSize = true;
            this.domainUpDownBookNames.Items.Add("Gen");
            this.domainUpDownBookNames.Items.Add("Exod");
            this.domainUpDownBookNames.Items.Add("Lev");
            this.domainUpDownBookNames.Items.Add("Num");
            this.domainUpDownBookNames.Items.Add("Deut");
            this.domainUpDownBookNames.Items.Add("Josh");
            this.domainUpDownBookNames.Items.Add("Judg");
            this.domainUpDownBookNames.Items.Add("Ruth");
            this.domainUpDownBookNames.Items.Add("1Sam");
            this.domainUpDownBookNames.Items.Add("2Sam");
            this.domainUpDownBookNames.Items.Add("1Kgs");
            this.domainUpDownBookNames.Items.Add("2Kgs");
            this.domainUpDownBookNames.Items.Add("1Chr");
            this.domainUpDownBookNames.Items.Add("2Chr");
            this.domainUpDownBookNames.Items.Add("Ezra");
            this.domainUpDownBookNames.Items.Add("Neh");
            this.domainUpDownBookNames.Items.Add("Esth");
            this.domainUpDownBookNames.Items.Add("Job");
            this.domainUpDownBookNames.Items.Add("Ps");
            this.domainUpDownBookNames.Items.Add("Prov");
            this.domainUpDownBookNames.Items.Add("Eccl");
            this.domainUpDownBookNames.Items.Add("Song");
            this.domainUpDownBookNames.Items.Add("Isa");
            this.domainUpDownBookNames.Items.Add("Jer");
            this.domainUpDownBookNames.Items.Add("Lam");
            this.domainUpDownBookNames.Items.Add("Ezek");
            this.domainUpDownBookNames.Items.Add("Dan");
            this.domainUpDownBookNames.Items.Add("Hos");
            this.domainUpDownBookNames.Items.Add("Joel");
            this.domainUpDownBookNames.Items.Add("Amos");
            this.domainUpDownBookNames.Items.Add("Obad");
            this.domainUpDownBookNames.Items.Add("Jonah");
            this.domainUpDownBookNames.Items.Add("Mic");
            this.domainUpDownBookNames.Items.Add("Nah");
            this.domainUpDownBookNames.Items.Add("Hab");
            this.domainUpDownBookNames.Items.Add("Zeph");
            this.domainUpDownBookNames.Items.Add("Hag");
            this.domainUpDownBookNames.Items.Add("Zech");
            this.domainUpDownBookNames.Items.Add("Mal");
            this.domainUpDownBookNames.Items.Add("Matt");
            this.domainUpDownBookNames.Items.Add("Mark");
            this.domainUpDownBookNames.Items.Add("Luke");
            this.domainUpDownBookNames.Items.Add("John");
            this.domainUpDownBookNames.Items.Add("Acts");
            this.domainUpDownBookNames.Items.Add("Rom");
            this.domainUpDownBookNames.Items.Add("1Cor");
            this.domainUpDownBookNames.Items.Add("2Cor");
            this.domainUpDownBookNames.Items.Add("Gal");
            this.domainUpDownBookNames.Items.Add("Eph");
            this.domainUpDownBookNames.Items.Add("Phil");
            this.domainUpDownBookNames.Items.Add("Col");
            this.domainUpDownBookNames.Items.Add("1Thess");
            this.domainUpDownBookNames.Items.Add("2Thess");
            this.domainUpDownBookNames.Items.Add("1Tim");
            this.domainUpDownBookNames.Items.Add("2Tim");
            this.domainUpDownBookNames.Items.Add("Titus");
            this.domainUpDownBookNames.Items.Add("Phlm");
            this.domainUpDownBookNames.Items.Add("Heb");
            this.domainUpDownBookNames.Items.Add("Jas");
            this.domainUpDownBookNames.Items.Add("1Pet");
            this.domainUpDownBookNames.Items.Add("2Pet");
            this.domainUpDownBookNames.Items.Add("1John");
            this.domainUpDownBookNames.Items.Add("2John");
            this.domainUpDownBookNames.Items.Add("3John");
            this.domainUpDownBookNames.Items.Add("Jude");
            this.domainUpDownBookNames.Items.Add("Rev");
            this.domainUpDownBookNames.Location = new System.Drawing.Point(3, 3);
            this.domainUpDownBookNames.Name = "domainUpDownBookNames";
            this.domainUpDownBookNames.Size = new System.Drawing.Size(58, 20);
            this.domainUpDownBookNames.TabIndex = 0;
            this.domainUpDownBookNames.Text = "Gen";
            this.domainUpDownBookNames.SelectedItemChanged += new System.EventHandler(this.domainUpDownBookNames_SelectedItemChanged);
            // 
            // numericUpDownChapterNumber
            // 
            this.numericUpDownChapterNumber.Location = new System.Drawing.Point(67, 3);
            this.numericUpDownChapterNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownChapterNumber.Name = "numericUpDownChapterNumber";
            this.numericUpDownChapterNumber.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownChapterNumber.TabIndex = 1;
            this.numericUpDownChapterNumber.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownChapterNumber.ValueChanged += new System.EventHandler(this.numericUpDownChapter_ValueChanged);
            // 
            // numericUpDownVerseNumber
            // 
            this.numericUpDownVerseNumber.Location = new System.Drawing.Point(126, 3);
            this.numericUpDownVerseNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownVerseNumber.Name = "numericUpDownVerseNumber";
            this.numericUpDownVerseNumber.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownVerseNumber.TabIndex = 2;
            this.numericUpDownVerseNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownVerseNumber.ValueChanged += new System.EventHandler(this.numericUpDownVerse_ValueChanged);
            // 
            // textBoxNetFlixViewer
            // 
            this.textBoxNetFlixViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxNetFlixViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNetFlixViewer.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNetFlixViewer.Location = new System.Drawing.Point(170, 0);
            this.textBoxNetFlixViewer.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxNetFlixViewer.Name = "textBoxNetFlixViewer";
            this.textBoxNetFlixViewer.ReadOnly = true;
            this.textBoxNetFlixViewer.Size = new System.Drawing.Size(330, 22);
            this.textBoxNetFlixViewer.TabIndex = 6;
            this.textBoxNetFlixViewer.TabStop = false;
            this.textBoxNetFlixViewer.Text = "Bible";
            this.textBoxNetFlixViewer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // radioButtonShowOtherSwordResources
            // 
            this.radioButtonShowOtherSwordResources.AutoSize = true;
            this.radioButtonShowOtherSwordResources.Location = new System.Drawing.Point(503, 3);
            this.radioButtonShowOtherSwordResources.Name = "radioButtonShowOtherSwordResources";
            this.radioButtonShowOtherSwordResources.Size = new System.Drawing.Size(51, 17);
            this.radioButtonShowOtherSwordResources.TabIndex = 7;
            this.radioButtonShowOtherSwordResources.TabStop = true;
            this.radioButtonShowOtherSwordResources.Text = "Other";
            this.radioButtonShowOtherSwordResources.UseVisualStyleBackColor = true;
            this.radioButtonShowOtherSwordResources.CheckedChanged += new System.EventHandler(this.radioButtonShowOtherSwordResources_CheckedChanged);
            // 
            // checkBoxAutoHide
            // 
            this.checkBoxAutoHide.AutoSize = true;
            this.checkBoxAutoHide.Checked = true;
            this.checkBoxAutoHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoHide.Location = new System.Drawing.Point(566, 3);
            this.checkBoxAutoHide.Name = "checkBoxAutoHide";
            this.checkBoxAutoHide.Size = new System.Drawing.Size(14, 14);
            this.checkBoxAutoHide.TabIndex = 3;
            this.toolTip.SetToolTip(this.checkBoxAutoHide, "Uncheck this box to have the Bible Pane automatically hide when you are typing. T" +
                    "hen you can right-click on the box to manually open and close the Bible Pane");
            this.checkBoxAutoHide.UseMnemonic = false;
            this.checkBoxAutoHide.UseVisualStyleBackColor = true;
            this.checkBoxAutoHide.CheckStateChanged += new System.EventHandler(this.checkBoxAutoHide_CheckStateChanged);
            this.checkBoxAutoHide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkBoxAutoHide_MouseUp);
            // 
            // contextMenuStripBibleBooks
            // 
            this.contextMenuStripBibleBooks.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStripBibleBooks.Name = "contextMenuStripBibleBooks";
            this.contextMenuStripBibleBooks.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStripBibleBooks.ShowImageMargin = false;
            this.contextMenuStripBibleBooks.ShowItemToolTips = false;
            this.contextMenuStripBibleBooks.Size = new System.Drawing.Size(36, 4);
            // 
            // NetBibleViewer
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "NetBibleViewer";
            this.Size = new System.Drawing.Size(583, 262);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tableLayoutPanelSpinControls.ResumeLayout(false);
            this.tableLayoutPanelSpinControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChapterNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVerseNumber)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.TextBox textBoxSizer;
        #endregion
        private System.Windows.Forms.WebBrowser webBrowserNetBible;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private DynamicTableLayoutPanel tableLayoutPanelSpinControls;
        private System.Windows.Forms.DomainUpDown domainUpDownBookNames;
        private System.Windows.Forms.NumericUpDown numericUpDownChapterNumber;
        private System.Windows.Forms.NumericUpDown numericUpDownVerseNumber;
        private System.Windows.Forms.TextBox textBoxNetFlixViewer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripBibleBooks;
        private System.Windows.Forms.RadioButton radioButtonShowOtherSwordResources;
        private System.Windows.Forms.ToolTip toolTip;
        internal System.Windows.Forms.CheckBox checkBoxAutoHide;
    }
}
