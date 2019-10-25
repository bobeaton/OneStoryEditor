namespace OneStoryProjectEditor
{
    partial class PrintViewer
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
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExportWord = new System.Windows.Forms.Button();
            this.webBrowser = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonSaveHtml = new System.Windows.Forms.Button();
            this.saveWordFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "html";
            this.saveFileDialog.Filter = "Web page|*.htm;*.html|All files|*.*";
            this.saveFileDialog.Title = "Save project in HTML format";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.buttonExportWord, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.webBrowser, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonClose, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonPrint, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonSaveHtml, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 242);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // buttonExportWord
            // 
            this.buttonExportWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportWord.Location = new System.Drawing.Point(3, 216);
            this.buttonExportWord.Name = "buttonExportWord";
            this.buttonExportWord.Size = new System.Drawing.Size(81, 23);
            this.buttonExportWord.TabIndex = 4;
            this.buttonExportWord.Text = "&Export Word";
            this.buttonExportWord.UseVisualStyleBackColor = true;
            this.buttonExportWord.Click += new System.EventHandler(this.ButtonExportWordClick);
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.tableLayoutPanel1.SetColumnSpan(this.webBrowser, 4);
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ParentStory = null;
            this.webBrowser.Size = new System.Drawing.Size(432, 207);
            this.webBrowser.StoryData = null;
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TheSE = null;
            this.webBrowser.ViewSettings = null;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClose.Location = new System.Drawing.Point(308, 216);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonCloseClick);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPrint.Location = new System.Drawing.Point(202, 216);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(75, 23);
            this.buttonPrint.TabIndex = 1;
            this.buttonPrint.Text = "&Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.ButtonPrintClick);
            // 
            // buttonSaveHtml
            // 
            this.buttonSaveHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveHtml.Location = new System.Drawing.Point(90, 216);
            this.buttonSaveHtml.Name = "buttonSaveHtml";
            this.buttonSaveHtml.Size = new System.Drawing.Size(81, 23);
            this.buttonSaveHtml.TabIndex = 3;
            this.buttonSaveHtml.Text = "&Save (HTML)";
            this.buttonSaveHtml.UseVisualStyleBackColor = true;
            this.buttonSaveHtml.Click += new System.EventHandler(this.ButtonSaveHtmlClick);
            // 
            // saveWordFileDialog
            // 
            this.saveWordFileDialog.Title = "Save project in Word format";
            this.saveWordFileDialog.Filter = "Word Documents (*.docx)|*.docx";
            this.saveWordFileDialog.DefaultExt = ".docx";
            // 
            // PrintViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PrintViewer";
            this.Size = new System.Drawing.Size(438, 242);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonPrint;
        public HtmlStoryBtControl webBrowser;
        private System.Windows.Forms.Button buttonSaveHtml;
        private System.Windows.Forms.Button buttonExportWord;
        public System.Windows.Forms.SaveFileDialog saveWordFileDialog;
    }
}
