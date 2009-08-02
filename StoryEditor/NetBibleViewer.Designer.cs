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
            this.toolStripBibleReference = new System.Windows.Forms.ToolStrip();
            this.toolStripComboBoxBookName = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonPrevBook = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNextBook = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBoxChapterNumber = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonPrevChap = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNextChap = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBoxVerseNumber = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonPrevVerse = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNextVerse = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.textBoxSizer = new System.Windows.Forms.TextBox();
            this.webBrowserNetBible = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripBibleReference.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripBibleReference
            // 
            this.toolStripBibleReference.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxBookName,
            this.toolStripButtonPrevBook,
            this.toolStripButtonNextBook,
            this.toolStripComboBoxChapterNumber,
            this.toolStripButtonPrevChap,
            this.toolStripButtonNextChap,
            this.toolStripComboBoxVerseNumber,
            this.toolStripButtonPrevVerse,
            this.toolStripButtonNextVerse});
            this.toolStripBibleReference.Location = new System.Drawing.Point(0, 0);
            this.toolStripBibleReference.Name = "toolStripBibleReference";
            this.toolStripBibleReference.Size = new System.Drawing.Size(583, 25);
            this.toolStripBibleReference.TabIndex = 1;
            this.toolStripBibleReference.Text = "toolStrip1";
            // 
            // toolStripComboBoxBookName
            // 
            this.toolStripComboBoxBookName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripComboBoxBookName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.toolStripComboBoxBookName.AutoSize = false;
            this.toolStripComboBoxBookName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxBookName.Items.AddRange(new object[] {
            "Gen",
            "Exod",
            "Lev",
            "Num",
            "Deut",
            "Josh",
            "Judg",
            "Ruth",
            "1Sam",
            "2Sam",
            "1Kgs",
            "2Kgs",
            "1Chr",
            "2Chr",
            "Ezra",
            "Neh",
            "Esth",
            "Job",
            "Ps",
            "Prov",
            "Eccl",
            "Song",
            "Isa",
            "Jer",
            "Lam",
            "Ezek",
            "Dan",
            "Hos",
            "Joel",
            "Amos",
            "Obad",
            "Jonah",
            "Mic",
            "Nah",
            "Hab",
            "Zeph",
            "Hag",
            "Zech",
            "Mal",
            "Matt",
            "Mark",
            "Luke",
            "John",
            "Acts",
            "Rom",
            "1Cor",
            "2Cor",
            "Gal",
            "Eph",
            "Phil",
            "Col",
            "1Thess",
            "2Thess",
            "1Tim",
            "2Tim",
            "Titus",
            "Phlm",
            "Heb",
            "Jas",
            "1Pet",
            "2Pet",
            "1John",
            "2John",
            "3John",
            "Jude",
            "Rev"});
            this.toolStripComboBoxBookName.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.toolStripComboBoxBookName.Name = "toolStripComboBoxBookName";
            this.toolStripComboBoxBookName.Size = new System.Drawing.Size(55, 23);
            this.toolStripComboBoxBookName.ToolTipText = "Choose the book of the Bible you want to see";
            this.toolStripComboBoxBookName.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxBookName_SelectedIndexChanged);
            // 
            // toolStripButtonPrevBook
            // 
            this.toolStripButtonPrevBook.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevBook.Image = global::OneStoryProjectEditor.Properties.Resources.GoRtlHS;
            this.toolStripButtonPrevBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevBook.Name = "toolStripButtonPrevBook";
            this.toolStripButtonPrevBook.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevBook.Text = "PrevBook";
            this.toolStripButtonPrevBook.ToolTipText = "Click here to go to the previous book";
            this.toolStripButtonPrevBook.Click += new System.EventHandler(this.toolStripButtonPrevBook_Click);
            // 
            // toolStripButtonNextBook
            // 
            this.toolStripButtonNextBook.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNextBook.Image = global::OneStoryProjectEditor.Properties.Resources.GoLtrHS;
            this.toolStripButtonNextBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNextBook.Name = "toolStripButtonNextBook";
            this.toolStripButtonNextBook.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNextBook.Text = "NextBook";
            this.toolStripButtonNextBook.ToolTipText = "Click here to go to the next book";
            // 
            // toolStripComboBoxChapterNumber
            // 
            this.toolStripComboBoxChapterNumber.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripComboBoxChapterNumber.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.toolStripComboBoxChapterNumber.AutoSize = false;
            this.toolStripComboBoxChapterNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxChapterNumber.Name = "toolStripComboBoxChapterNumber";
            this.toolStripComboBoxChapterNumber.Size = new System.Drawing.Size(45, 23);
            this.toolStripComboBoxChapterNumber.ToolTipText = "Choose the Chapter you want to see";
            this.toolStripComboBoxChapterNumber.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxChapterNumber_SelectedIndexChanged);
            // 
            // toolStripButtonPrevChap
            // 
            this.toolStripButtonPrevChap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevChap.Image = global::OneStoryProjectEditor.Properties.Resources.GoRtlHS;
            this.toolStripButtonPrevChap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevChap.Name = "toolStripButtonPrevChap";
            this.toolStripButtonPrevChap.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevChap.Text = "toolStripButton1";
            this.toolStripButtonPrevChap.Click += new System.EventHandler(this.toolStripButtonPrevChap_Click);
            // 
            // toolStripButtonNextChap
            // 
            this.toolStripButtonNextChap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNextChap.Image = global::OneStoryProjectEditor.Properties.Resources.GoLtrHS;
            this.toolStripButtonNextChap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNextChap.Name = "toolStripButtonNextChap";
            this.toolStripButtonNextChap.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNextChap.Text = "toolStripButton1";
            this.toolStripButtonNextChap.Click += new System.EventHandler(this.toolStripButtonNextChap_Click);
            // 
            // toolStripComboBoxVerseNumber
            // 
            this.toolStripComboBoxVerseNumber.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripComboBoxVerseNumber.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.toolStripComboBoxVerseNumber.AutoSize = false;
            this.toolStripComboBoxVerseNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxVerseNumber.Name = "toolStripComboBoxVerseNumber";
            this.toolStripComboBoxVerseNumber.Size = new System.Drawing.Size(45, 23);
            this.toolStripComboBoxVerseNumber.ToolTipText = "Choose the Verse you want to see";
            this.toolStripComboBoxVerseNumber.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxVerseNumber_SelectedIndexChanged);
            // 
            // toolStripButtonPrevVerse
            // 
            this.toolStripButtonPrevVerse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevVerse.Image = global::OneStoryProjectEditor.Properties.Resources.GoRtlHS;
            this.toolStripButtonPrevVerse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevVerse.Name = "toolStripButtonPrevVerse";
            this.toolStripButtonPrevVerse.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevVerse.Text = "PrevVerse";
            this.toolStripButtonPrevVerse.ToolTipText = "Click here to go to the previous verse";
            this.toolStripButtonPrevVerse.Click += new System.EventHandler(this.toolStripButtonPrevVerse_Click);
            // 
            // toolStripButtonNextVerse
            // 
            this.toolStripButtonNextVerse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNextVerse.Image = global::OneStoryProjectEditor.Properties.Resources.GoLtrHS;
            this.toolStripButtonNextVerse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNextVerse.Name = "toolStripButtonNextVerse";
            this.toolStripButtonNextVerse.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNextVerse.Text = "NextVerse";
            this.toolStripButtonNextVerse.ToolTipText = "Click here to go to the next verse";
            this.toolStripButtonNextVerse.Click += new System.EventHandler(this.toolStripButtonNextVerse_Click);
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
            this.webBrowserNetBible.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserNetBible.Location = new System.Drawing.Point(3, 28);
            this.webBrowserNetBible.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserNetBible.Name = "webBrowserNetBible";
            this.webBrowserNetBible.Size = new System.Drawing.Size(577, 231);
            this.webBrowserNetBible.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.toolStripBibleReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.webBrowserNetBible, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(583, 262);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // NetBibleViewer
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "NetBibleViewer";
            this.Size = new System.Drawing.Size(583, 262);
            this.toolStripBibleReference.ResumeLayout(false);
            this.toolStripBibleReference.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ToolStrip toolStripBibleReference;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxChapterNumber;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxVerseNumber;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevChap;
        private System.Windows.Forms.ToolStripButton toolStripButtonNextChap;
        private System.Windows.Forms.ToolStripButton toolStripButtonNextBook;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevBook;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevVerse;
        private System.Windows.Forms.ToolStripButton toolStripButtonNextVerse;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxBookName;
        private System.Windows.Forms.TextBox textBoxSizer;
        #endregion
        private System.Windows.Forms.WebBrowser webBrowserNetBible;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}
