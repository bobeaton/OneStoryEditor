namespace StoryEditor
{
    partial class StoryEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoryEditor));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.teamMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelVerses = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerLeftRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerUpDown = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.webBrowserNetBible = new System.Windows.Forms.WebBrowser();
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
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewVernacularLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNationalLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEnglishBTFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAnchorFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewExegeticalCulturalNotesFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewRetellingFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewConsultantNoteFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.splitContainerLeftRight.Panel1.SuspendLayout();
            this.splitContainerLeftRight.SuspendLayout();
            this.splitContainerUpDown.Panel1.SuspendLayout();
            this.splitContainerUpDown.Panel2.SuspendLayout();
            this.splitContainerUpDown.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.toolStripBibleReference.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(895, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.teamMembersToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.projectToolStripMenuItem.Text = "&Project";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // teamMembersToolStripMenuItem
            // 
            this.teamMembersToolStripMenuItem.Name = "teamMembersToolStripMenuItem";
            this.teamMembersToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.teamMembersToolStripMenuItem.Text = "Edit &Team members";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            // 
            // flowLayoutPanelVerses
            // 
            this.flowLayoutPanelVerses.AutoScroll = true;
            this.flowLayoutPanelVerses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelVerses.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelVerses.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelVerses.Name = "flowLayoutPanelVerses";
            this.flowLayoutPanelVerses.Size = new System.Drawing.Size(521, 199);
            this.flowLayoutPanelVerses.TabIndex = 1;
            this.flowLayoutPanelVerses.WrapContents = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "osp";
            this.openFileDialog.Filter = "OneStory Project file|*.osp";
            this.openFileDialog.Title = "Open OneStory Project File";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "osp";
            this.saveFileDialog.FileName = "StoryProjectName";
            this.saveFileDialog.Filter = "OneStory Project file|*.osp";
            this.saveFileDialog.Title = "Open OneStory Project File";
            // 
            // splitContainerLeftRight
            // 
            this.splitContainerLeftRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerLeftRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeftRight.Location = new System.Drawing.Point(0, 24);
            this.splitContainerLeftRight.Name = "splitContainerLeftRight";
            // 
            // splitContainerLeftRight.Panel1
            // 
            this.splitContainerLeftRight.Panel1.Controls.Add(this.splitContainerUpDown);
            this.splitContainerLeftRight.Size = new System.Drawing.Size(895, 312);
            this.splitContainerLeftRight.SplitterDistance = 523;
            this.splitContainerLeftRight.TabIndex = 2;
            this.splitContainerLeftRight.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerUpper_SplitterMoved);
            // 
            // splitContainerUpDown
            // 
            this.splitContainerUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUpDown.Location = new System.Drawing.Point(0, 0);
            this.splitContainerUpDown.Name = "splitContainerUpDown";
            this.splitContainerUpDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerUpDown.Panel1
            // 
            this.splitContainerUpDown.Panel1.Controls.Add(this.flowLayoutPanelVerses);
            // 
            // splitContainerUpDown.Panel2
            // 
            this.splitContainerUpDown.Panel2.Controls.Add(this.tableLayoutPanel);
            this.splitContainerUpDown.Size = new System.Drawing.Size(523, 312);
            this.splitContainerUpDown.SplitterDistance = 201;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.webBrowserNetBible, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.toolStripBibleReference, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(521, 105);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // webBrowserNetBible
            // 
            this.webBrowserNetBible.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserNetBible.Location = new System.Drawing.Point(3, 28);
            this.webBrowserNetBible.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserNetBible.Name = "webBrowserNetBible";
            this.webBrowserNetBible.Size = new System.Drawing.Size(515, 74);
            this.webBrowserNetBible.TabIndex = 0;
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
            this.toolStripBibleReference.Size = new System.Drawing.Size(521, 25);
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
            this.toolStripComboBoxBookName.Size = new System.Drawing.Size(55, 21);
            this.toolStripComboBoxBookName.ToolTipText = "Choose the book of the Bible you want to see";
            this.toolStripComboBoxBookName.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxBookName_SelectedIndexChanged);
            // 
            // toolStripButtonPrevBook
            // 
            this.toolStripButtonPrevBook.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevBook.Image = global::StoryEditor.Properties.Resources.GoRtlHS;
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
            this.toolStripButtonNextBook.Image = global::StoryEditor.Properties.Resources.GoLtrHS;
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
            this.toolStripComboBoxChapterNumber.Size = new System.Drawing.Size(45, 21);
            this.toolStripComboBoxChapterNumber.ToolTipText = "Choose the Chapter you want to see";
            this.toolStripComboBoxChapterNumber.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxChapterNumber_SelectedIndexChanged);
            // 
            // toolStripButtonPrevChap
            // 
            this.toolStripButtonPrevChap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevChap.Image = global::StoryEditor.Properties.Resources.GoRtlHS;
            this.toolStripButtonPrevChap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevChap.Name = "toolStripButtonPrevChap";
            this.toolStripButtonPrevChap.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevChap.Text = "toolStripButton1";
            this.toolStripButtonPrevChap.Click += new System.EventHandler(this.toolStripButtonPrevChap_Click);
            // 
            // toolStripButtonNextChap
            // 
            this.toolStripButtonNextChap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNextChap.Image = global::StoryEditor.Properties.Resources.GoLtrHS;
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
            this.toolStripComboBoxVerseNumber.Size = new System.Drawing.Size(45, 21);
            this.toolStripComboBoxVerseNumber.ToolTipText = "Choose the Verse you want to see";
            this.toolStripComboBoxVerseNumber.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxVerseNumber_SelectedIndexChanged);
            // 
            // toolStripButtonPrevVerse
            // 
            this.toolStripButtonPrevVerse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevVerse.Image = global::StoryEditor.Properties.Resources.GoRtlHS;
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
            this.toolStripButtonNextVerse.Image = global::StoryEditor.Properties.Resources.GoLtrHS;
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
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewVernacularLangFieldMenuItem,
            this.viewNationalLangFieldMenuItem,
            this.viewEnglishBTFieldMenuItem,
            this.viewAnchorFieldMenuItem,
            this.viewExegeticalCulturalNotesFieldMenuItem,
            this.viewStoryTestingQuestionFieldMenuItem,
            this.toolStripSeparator5,
            this.viewRetellingFieldMenuItem,
            this.toolStripSeparator6,
            this.viewConsultantNoteFieldMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // viewVernacularLangFieldMenuItem
            // 
            this.viewVernacularLangFieldMenuItem.Checked = true;
            this.viewVernacularLangFieldMenuItem.CheckOnClick = true;
            this.viewVernacularLangFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewVernacularLangFieldMenuItem.Name = "viewVernacularLangFieldMenuItem";
            this.viewVernacularLangFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewVernacularLangFieldMenuItem.Text = "Story &Language field";
            this.viewVernacularLangFieldMenuItem.CheckedChanged += new System.EventHandler(viewFieldMenuItem_CheckedChanged);
            // 
            // viewNationalLangFieldMenuItem
            // 
            this.viewNationalLangFieldMenuItem.Checked = true;
            this.viewNationalLangFieldMenuItem.CheckOnClick = true;
            this.viewNationalLangFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNationalLangFieldMenuItem.Name = "viewNationalLangFieldMenuItem";
            this.viewNationalLangFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewNationalLangFieldMenuItem.Text = "National language &back translation field";
            this.viewNationalLangFieldMenuItem.CheckedChanged += new System.EventHandler(viewFieldMenuItem_CheckedChanged);
            // 
            // viewEnglishBTFieldMenuItem
            // 
            this.viewEnglishBTFieldMenuItem.Checked = true;
            this.viewEnglishBTFieldMenuItem.CheckOnClick = true;
            this.viewEnglishBTFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewEnglishBTFieldMenuItem.Name = "viewEnglishBTFieldMenuItem";
            this.viewEnglishBTFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewEnglishBTFieldMenuItem.Text = "&English back translation field";
            this.viewEnglishBTFieldMenuItem.CheckedChanged += new System.EventHandler(viewFieldMenuItem_CheckedChanged);
            // 
            // viewAnchorFieldMenuItem
            // 
            this.viewAnchorFieldMenuItem.Checked = true;
            this.viewAnchorFieldMenuItem.CheckOnClick = true;
            this.viewAnchorFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewAnchorFieldMenuItem.Name = "viewAnchorFieldMenuItem";
            this.viewAnchorFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewAnchorFieldMenuItem.Text = "&Anchor field";
            this.viewAnchorFieldMenuItem.CheckedChanged += new System.EventHandler(viewFieldMenuItem_CheckedChanged);
            // 
            // viewExegeticalCulturalNotesFieldMenuItem
            // 
            this.viewExegeticalCulturalNotesFieldMenuItem.Checked = true;
            this.viewExegeticalCulturalNotesFieldMenuItem.CheckOnClick = true;
            this.viewExegeticalCulturalNotesFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewExegeticalCulturalNotesFieldMenuItem.Name = "viewExegeticalCulturalNotesFieldMenuItem";
            this.viewExegeticalCulturalNotesFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewExegeticalCulturalNotesFieldMenuItem.Text = "Exegetical/Cultural &notes field";
            // 
            // viewStoryTestingQuestionFieldMenuItem
            // 
            this.viewStoryTestingQuestionFieldMenuItem.Checked = true;
            this.viewStoryTestingQuestionFieldMenuItem.CheckOnClick = true;
            this.viewStoryTestingQuestionFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionFieldMenuItem.Name = "viewStoryTestingQuestionFieldMenuItem";
            this.viewStoryTestingQuestionFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewStoryTestingQuestionFieldMenuItem.Text = "Story &testing questions field";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(259, 6);
            // 
            // viewRetellingFieldMenuItem
            // 
            this.viewRetellingFieldMenuItem.Checked = true;
            this.viewRetellingFieldMenuItem.CheckOnClick = true;
            this.viewRetellingFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewRetellingFieldMenuItem.Name = "viewRetellingFieldMenuItem";
            this.viewRetellingFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewRetellingFieldMenuItem.Text = "&Retelling field";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(259, 6);
            // 
            // viewConsultantNoteFieldMenuItem
            // 
            this.viewConsultantNoteFieldMenuItem.Checked = true;
            this.viewConsultantNoteFieldMenuItem.CheckOnClick = true;
            this.viewConsultantNoteFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewConsultantNoteFieldMenuItem.Name = "viewConsultantNoteFieldMenuItem";
            this.viewConsultantNoteFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewConsultantNoteFieldMenuItem.Text = "&Consultant notes field";
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 336);
            this.Controls.Add(this.splitContainerLeftRight);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "StoryEditor";
            this.Text = "OneStory Editor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerLeftRight.Panel1.ResumeLayout(false);
            this.splitContainerLeftRight.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.ResumeLayout(false);
            this.splitContainerUpDown.Panel2.ResumeLayout(false);
            this.splitContainerUpDown.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.toolStripBibleReference.ResumeLayout(false);
            this.toolStripBibleReference.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem teamMembersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelVerses;
        private System.Windows.Forms.SplitContainer splitContainerLeftRight;
        private System.Windows.Forms.SplitContainer splitContainerUpDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.WebBrowser webBrowserNetBible;
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
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        internal System.Windows.Forms.ToolStripMenuItem viewVernacularLangFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewNationalLangFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewEnglishBTFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewAnchorFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewExegeticalCulturalNotesFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewStoryTestingQuestionFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewRetellingFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewConsultantNoteFieldMenuItem;
    }
}

