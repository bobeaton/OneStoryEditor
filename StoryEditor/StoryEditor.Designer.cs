namespace OneStoryProjectEditor
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
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.teamMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewVernacularLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNationalLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEnglishBTFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAnchorFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewRetellingFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewConsultantNoteFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCoachNotesFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewNetBibleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxStorySelector = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripTextBoxChooseStory = new System.Windows.Forms.ToolStripTextBox();
            this.flowLayoutPanelVerses = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerLeftRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerUpDown = new System.Windows.Forms.SplitContainer();
            this.textBoxStoryVerse = new System.Windows.Forms.TextBox();
            this.netBibleViewer = new OneStoryProjectEditor.NetBibleViewer();
            this.splitContainerMentorNotes = new System.Windows.Forms.SplitContainer();
            this.textBoxConsultantNotesTable = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelConsultantNotes = new System.Windows.Forms.FlowLayoutPanel();
            this.textBoxCoachNotes = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelCoachNotes = new System.Windows.Forms.FlowLayoutPanel();
            this.macTrackBarProjectStages = new XComponent.SliderBar.MACTrackBar();
            this.menuStrip.SuspendLayout();
            this.splitContainerLeftRight.Panel1.SuspendLayout();
            this.splitContainerLeftRight.Panel2.SuspendLayout();
            this.splitContainerLeftRight.SuspendLayout();
            this.splitContainerUpDown.Panel1.SuspendLayout();
            this.splitContainerUpDown.Panel2.SuspendLayout();
            this.splitContainerUpDown.SuspendLayout();
            this.splitContainerMentorNotes.Panel1.SuspendLayout();
            this.splitContainerMentorNotes.Panel2.SuspendLayout();
            this.splitContainerMentorNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.comboBoxStorySelector,
            this.toolStripTextBoxChooseStory});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(895, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator4,
            this.teamMembersToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 21);
            this.projectToolStripMenuItem.Text = "&Project";
            this.projectToolStripMenuItem.DropDownOpening += new System.EventHandler(this.projectToolStripMenuItem_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Click to open an existing OneStory project";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.ToolTipText = "Click to create a new OneStory project";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.ToolTipText = "Click to save the OneStory project";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.recentFilesToolStripMenuItem.Text = "&Recent files";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // teamMembersToolStripMenuItem
            // 
            this.teamMembersToolStripMenuItem.Name = "teamMembersToolStripMenuItem";
            this.teamMembersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.teamMembersToolStripMenuItem.Text = "Se&ttings";
            this.teamMembersToolStripMenuItem.Click += new System.EventHandler(this.teamMembersToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewVernacularLangFieldMenuItem,
            this.viewNationalLangFieldMenuItem,
            this.viewEnglishBTFieldMenuItem,
            this.viewAnchorFieldMenuItem,
            this.viewStoryTestingQuestionFieldMenuItem,
            this.toolStripSeparator5,
            this.viewRetellingFieldMenuItem,
            this.toolStripSeparator6,
            this.viewConsultantNoteFieldMenuItem,
            this.viewCoachNotesFieldMenuItem,
            this.toolStripSeparator3,
            this.viewNetBibleMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
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
            this.viewVernacularLangFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewNationalLangFieldMenuItem
            // 
            this.viewNationalLangFieldMenuItem.Checked = true;
            this.viewNationalLangFieldMenuItem.CheckOnClick = true;
            this.viewNationalLangFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNationalLangFieldMenuItem.Name = "viewNationalLangFieldMenuItem";
            this.viewNationalLangFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewNationalLangFieldMenuItem.Text = "National language &back translation field";
            this.viewNationalLangFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewEnglishBTFieldMenuItem
            // 
            this.viewEnglishBTFieldMenuItem.Checked = true;
            this.viewEnglishBTFieldMenuItem.CheckOnClick = true;
            this.viewEnglishBTFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewEnglishBTFieldMenuItem.Name = "viewEnglishBTFieldMenuItem";
            this.viewEnglishBTFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewEnglishBTFieldMenuItem.Text = "&English back translation field";
            this.viewEnglishBTFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewAnchorFieldMenuItem
            // 
            this.viewAnchorFieldMenuItem.Checked = true;
            this.viewAnchorFieldMenuItem.CheckOnClick = true;
            this.viewAnchorFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewAnchorFieldMenuItem.Name = "viewAnchorFieldMenuItem";
            this.viewAnchorFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewAnchorFieldMenuItem.Text = "&Anchor fields";
            this.viewAnchorFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionFieldMenuItem
            // 
            this.viewStoryTestingQuestionFieldMenuItem.Checked = true;
            this.viewStoryTestingQuestionFieldMenuItem.CheckOnClick = true;
            this.viewStoryTestingQuestionFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionFieldMenuItem.Name = "viewStoryTestingQuestionFieldMenuItem";
            this.viewStoryTestingQuestionFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewStoryTestingQuestionFieldMenuItem.Text = "Story &testing questions field";
            this.viewStoryTestingQuestionFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
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
            this.viewRetellingFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
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
            this.viewConsultantNoteFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewConsultantNoteFieldMenuItem_CheckedChanged);
            // 
            // viewCoachNotesFieldMenuItem
            // 
            this.viewCoachNotesFieldMenuItem.Checked = true;
            this.viewCoachNotesFieldMenuItem.CheckOnClick = true;
            this.viewCoachNotesFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewCoachNotesFieldMenuItem.Name = "viewCoachNotesFieldMenuItem";
            this.viewCoachNotesFieldMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewCoachNotesFieldMenuItem.Text = "Coac&h notes field";
            this.viewCoachNotesFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewCoachNotesFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(259, 6);
            // 
            // viewNetBibleMenuItem
            // 
            this.viewNetBibleMenuItem.Checked = true;
            this.viewNetBibleMenuItem.CheckOnClick = true;
            this.viewNetBibleMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNetBibleMenuItem.Name = "viewNetBibleMenuItem";
            this.viewNetBibleMenuItem.Size = new System.Drawing.Size(262, 22);
            this.viewNetBibleMenuItem.Text = "&NetBible";
            this.viewNetBibleMenuItem.CheckedChanged += new System.EventHandler(this.viewNetBibleMenuItem_CheckedChanged);
            // 
            // comboBoxStorySelector
            // 
            this.comboBoxStorySelector.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.comboBoxStorySelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxStorySelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxStorySelector.Name = "comboBoxStorySelector";
            this.comboBoxStorySelector.Size = new System.Drawing.Size(290, 21);
            this.comboBoxStorySelector.Text = "<type the name of a story to create and hit Enter>";
            this.comboBoxStorySelector.ToolTipText = "Select the Story to edit or type in a new name to add a new story";
            this.comboBoxStorySelector.SelectedIndexChanged += new System.EventHandler(this.comboBoxStorySelector_SelectedIndexChanged);
            this.comboBoxStorySelector.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStorySelector_KeyUp);
            // 
            // toolStripTextBoxChooseStory
            // 
            this.toolStripTextBoxChooseStory.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBoxChooseStory.Name = "toolStripTextBoxChooseStory";
            this.toolStripTextBoxChooseStory.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBoxChooseStory.Text = "Choose Story:";
            // 
            // flowLayoutPanelVerses
            // 
            this.flowLayoutPanelVerses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelVerses.AutoScroll = true;
            this.flowLayoutPanelVerses.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelVerses.Location = new System.Drawing.Point(0, 29);
            this.flowLayoutPanelVerses.Name = "flowLayoutPanelVerses";
            this.flowLayoutPanelVerses.Size = new System.Drawing.Size(521, 148);
            this.flowLayoutPanelVerses.TabIndex = 1;
            this.flowLayoutPanelVerses.WrapContents = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "onestory";
            this.openFileDialog.Filter = "OneStory Project file|*.onestory";
            this.openFileDialog.Title = "Open OneStory Project File";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "onestory";
            this.saveFileDialog.FileName = "StoryProjectName";
            this.saveFileDialog.Filter = "OneStory Project file|*.onestory";
            this.saveFileDialog.Title = "Open OneStory Project File";
            // 
            // splitContainerLeftRight
            // 
            this.splitContainerLeftRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerLeftRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeftRight.Location = new System.Drawing.Point(0, 25);
            this.splitContainerLeftRight.Name = "splitContainerLeftRight";
            // 
            // splitContainerLeftRight.Panel1
            // 
            this.splitContainerLeftRight.Panel1.Controls.Add(this.splitContainerUpDown);
            this.splitContainerLeftRight.Panel1.SizeChanged += new System.EventHandler(this.splitContainerLeftRight_Panel1_SizeChanged);
            // 
            // splitContainerLeftRight.Panel2
            // 
            this.splitContainerLeftRight.Panel2.Controls.Add(this.splitContainerMentorNotes);
            this.splitContainerLeftRight.Panel2.SizeChanged += new System.EventHandler(this.splitContainerLeftRight_Panel2_SizeChanged);
            this.splitContainerLeftRight.Size = new System.Drawing.Size(895, 277);
            this.splitContainerLeftRight.SplitterDistance = 523;
            this.splitContainerLeftRight.TabIndex = 2;
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
            this.splitContainerUpDown.Panel1.Controls.Add(this.textBoxStoryVerse);
            this.splitContainerUpDown.Panel1.Controls.Add(this.flowLayoutPanelVerses);
            // 
            // splitContainerUpDown.Panel2
            // 
            this.splitContainerUpDown.Panel2.Controls.Add(this.netBibleViewer);
            this.splitContainerUpDown.Size = new System.Drawing.Size(523, 277);
            this.splitContainerUpDown.SplitterDistance = 177;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // textBoxStoryVerse
            // 
            this.textBoxStoryVerse.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxStoryVerse.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStoryVerse.Location = new System.Drawing.Point(0, 0);
            this.textBoxStoryVerse.Name = "textBoxStoryVerse";
            this.textBoxStoryVerse.ReadOnly = true;
            this.textBoxStoryVerse.Size = new System.Drawing.Size(521, 29);
            this.textBoxStoryVerse.TabIndex = 3;
            this.textBoxStoryVerse.Text = "Story";
            this.textBoxStoryVerse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // netBibleViewer
            // 
            this.netBibleViewer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.netBibleViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netBibleViewer.Location = new System.Drawing.Point(0, 0);
            this.netBibleViewer.Margin = new System.Windows.Forms.Padding(0);
            this.netBibleViewer.Name = "netBibleViewer";
            this.netBibleViewer.ScriptureReference = "gen 1:1";
            this.netBibleViewer.Size = new System.Drawing.Size(521, 94);
            this.netBibleViewer.TabIndex = 0;
            // 
            // splitContainerMentorNotes
            // 
            this.splitContainerMentorNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerMentorNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMentorNotes.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMentorNotes.Name = "splitContainerMentorNotes";
            this.splitContainerMentorNotes.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMentorNotes.Panel1
            // 
            this.splitContainerMentorNotes.Panel1.Controls.Add(this.textBoxConsultantNotesTable);
            this.splitContainerMentorNotes.Panel1.Controls.Add(this.flowLayoutPanelConsultantNotes);
            // 
            // splitContainerMentorNotes.Panel2
            // 
            this.splitContainerMentorNotes.Panel2.Controls.Add(this.textBoxCoachNotes);
            this.splitContainerMentorNotes.Panel2.Controls.Add(this.flowLayoutPanelCoachNotes);
            this.splitContainerMentorNotes.Size = new System.Drawing.Size(368, 277);
            this.splitContainerMentorNotes.SplitterDistance = 167;
            this.splitContainerMentorNotes.TabIndex = 0;
            // 
            // textBoxConsultantNotesTable
            // 
            this.textBoxConsultantNotesTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxConsultantNotesTable.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConsultantNotesTable.Location = new System.Drawing.Point(0, 0);
            this.textBoxConsultantNotesTable.Name = "textBoxConsultantNotesTable";
            this.textBoxConsultantNotesTable.ReadOnly = true;
            this.textBoxConsultantNotesTable.Size = new System.Drawing.Size(366, 29);
            this.textBoxConsultantNotesTable.TabIndex = 1;
            this.textBoxConsultantNotesTable.Text = "Consultant Notes";
            this.textBoxConsultantNotesTable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // flowLayoutPanelConsultantNotes
            // 
            this.flowLayoutPanelConsultantNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelConsultantNotes.AutoScroll = true;
            this.flowLayoutPanelConsultantNotes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelConsultantNotes.Location = new System.Drawing.Point(0, 29);
            this.flowLayoutPanelConsultantNotes.Name = "flowLayoutPanelConsultantNotes";
            this.flowLayoutPanelConsultantNotes.Size = new System.Drawing.Size(366, 138);
            this.flowLayoutPanelConsultantNotes.TabIndex = 0;
            this.flowLayoutPanelConsultantNotes.WrapContents = false;
            // 
            // textBoxCoachNotes
            // 
            this.textBoxCoachNotes.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxCoachNotes.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCoachNotes.Location = new System.Drawing.Point(0, 0);
            this.textBoxCoachNotes.Name = "textBoxCoachNotes";
            this.textBoxCoachNotes.ReadOnly = true;
            this.textBoxCoachNotes.Size = new System.Drawing.Size(366, 29);
            this.textBoxCoachNotes.TabIndex = 2;
            this.textBoxCoachNotes.Text = "Coach Notes";
            this.textBoxCoachNotes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // flowLayoutPanelCoachNotes
            // 
            this.flowLayoutPanelCoachNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelCoachNotes.AutoScroll = true;
            this.flowLayoutPanelCoachNotes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelCoachNotes.Location = new System.Drawing.Point(0, 27);
            this.flowLayoutPanelCoachNotes.Name = "flowLayoutPanelCoachNotes";
            this.flowLayoutPanelCoachNotes.Size = new System.Drawing.Size(366, 79);
            this.flowLayoutPanelCoachNotes.TabIndex = 1;
            this.flowLayoutPanelCoachNotes.WrapContents = false;
            // 
            // macTrackBarProjectStages
            // 
            this.macTrackBarProjectStages.BackColor = System.Drawing.Color.Transparent;
            this.macTrackBarProjectStages.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.macTrackBarProjectStages.BorderStyle = XComponent.SliderBar.MACBorderStyle.Etched;
            this.macTrackBarProjectStages.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.macTrackBarProjectStages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.macTrackBarProjectStages.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(125)))), ((int)(((byte)(123)))));
            this.macTrackBarProjectStages.IndentHeight = 6;
            this.macTrackBarProjectStages.IndentWidth = 8;
            this.macTrackBarProjectStages.LargeChange = 1;
            this.macTrackBarProjectStages.Location = new System.Drawing.Point(0, 302);
            this.macTrackBarProjectStages.Maximum = 19;
            this.macTrackBarProjectStages.Minimum = 1;
            this.macTrackBarProjectStages.Name = "macTrackBarProjectStages";
            this.macTrackBarProjectStages.Size = new System.Drawing.Size(895, 34);
            this.macTrackBarProjectStages.TabIndex = 3;
            this.macTrackBarProjectStages.TickColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(146)))), ((int)(((byte)(148)))));
            this.macTrackBarProjectStages.TickHeight = 1;
            this.macTrackBarProjectStages.TrackerColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(130)))), ((int)(((byte)(198)))));
            this.macTrackBarProjectStages.TrackerSize = new System.Drawing.Size(6, 6);
            this.macTrackBarProjectStages.TrackLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(93)))), ((int)(((byte)(90)))));
            this.macTrackBarProjectStages.TrackLineHeight = 1;
            this.macTrackBarProjectStages.Value = 1;
            this.macTrackBarProjectStages.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.macTrackBarProjectStages_HelpRequested);
            this.macTrackBarProjectStages.ValueChanged += new XComponent.SliderBar.ValueChangedHandler(this.macTrackBarProjectStages_ValueChanged);
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 336);
            this.Controls.Add(this.splitContainerLeftRight);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.macTrackBarProjectStages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "StoryEditor";
            this.Text = "OneStory Editor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerLeftRight.Panel1.ResumeLayout(false);
            this.splitContainerLeftRight.Panel2.ResumeLayout(false);
            this.splitContainerLeftRight.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.PerformLayout();
            this.splitContainerUpDown.Panel2.ResumeLayout(false);
            this.splitContainerUpDown.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel1.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel1.PerformLayout();
            this.splitContainerMentorNotes.Panel2.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel2.PerformLayout();
            this.splitContainerMentorNotes.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        internal System.Windows.Forms.ToolStripMenuItem viewVernacularLangFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewNationalLangFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewEnglishBTFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewAnchorFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewStoryTestingQuestionFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewRetellingFieldMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem viewConsultantNoteFieldMenuItem;
        private NetBibleViewer netBibleViewer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem viewNetBibleMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerMentorNotes;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelConsultantNotes;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCoachNotes;
        private System.Windows.Forms.ToolStripMenuItem viewCoachNotesFieldMenuItem;
        private System.Windows.Forms.TextBox textBoxConsultantNotesTable;
        private System.Windows.Forms.TextBox textBoxCoachNotes;
        private System.Windows.Forms.TextBox textBoxStoryVerse;
        private System.Windows.Forms.ToolStripComboBox comboBoxStorySelector;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxChooseStory;
        private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private XComponent.SliderBar.MACTrackBar macTrackBarProjectStages;
    }
}

