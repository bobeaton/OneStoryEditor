#define UsingHtmlDisplayForConNotes

using System.Collections.Generic;
using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoryEditor));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendReceiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.browseForProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectFromTheInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toTheInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectFromASharedNetworkDriveToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.projectSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToToolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectState = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopySelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyNationalBackTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyEnglishBackTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFreeTranslationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBackTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteStoryVersesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteStoryNationalBackTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEnglishBacktranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFreeTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.editFindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.editAddRetellingTestResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAddInferenceTestResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useSameSettingsForAllStoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewVernacularLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNationalLangFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEnglishBTFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFreeTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAnchorFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionAnswerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewRetellingFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewConsultantNoteFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCoachNotesFieldMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewNetBibleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.historicalDifferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLnCNotesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.concordanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stateTransitionHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOldStoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.hiddenVersesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOnlyOpenConversationsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.viewTransliterationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationVernacular = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorVernacularConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationNationalBT = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxStorySelector = new System.Windows.Forms.ToolStripComboBox();
            this.storyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteStoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyCopyWithNewNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitIntoLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realignStoryVersesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.useAdaptItForBacktranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItVernacularToNationalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItVernacularToEnglishMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItNationalToEnglishMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertNewStoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewStoryAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShowPanorama = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeProjectFolderRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetStoredInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeStateWithoutChecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticallyCheckAtStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForProgramUpdatesNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTimeoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asSilentlyAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerLeftRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerUpDown = new OneStoryProjectEditor.MinimizableSplitterContainer();
            this.linkLabelVerseBT = new System.Windows.Forms.LinkLabel();
            this.contextMenuStripVerseList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowLayoutPanelVerses = new OneStoryProjectEditor.VerseBtLineFlowLayoutPanel();
            this.textBoxStoryVerse = new System.Windows.Forms.TextBox();
            this.netBibleViewer = new OneStoryProjectEditor.NetBibleViewer();
            this.splitContainerMentorNotes = new System.Windows.Forms.SplitContainer();
            this.linkLabelConsultantNotes = new System.Windows.Forms.LinkLabel();
            this.htmlConsultantNotesControl = new OneStoryProjectEditor.HtmlConsultantNotesControl();
            this.textBoxConsultantNotesTable = new System.Windows.Forms.TextBox();
            this.linkLabelCoachNotes = new System.Windows.Forms.LinkLabel();
            this.htmlCoachNotesControl = new OneStoryProjectEditor.HtmlCoachNotesControl();
            this.textBoxCoachNotes = new System.Windows.Forms.TextBox();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.toolStripRecordNavigation = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonFirst = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLast = new System.Windows.Forms.ToolStripButton();
            this.buttonsStoryStage = new System.Windows.Forms.ToolStripSplitButton();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
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
            this.toolStripRecordNavigation.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.toolStripMenuItemSelectState,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.comboBoxStorySelector,
            this.storyToolStripMenuItem,
            this.panoramaToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(881, 31);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recentProjectsToolStripMenuItem,
            this.sendReceiveToolStripMenuItem,
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.browseForProjectToolStripMenuItem,
            this.projectFromTheInternetToolStripMenuItem,
            this.toTheInternetToolStripMenuItem,
            this.projectFromASharedNetworkDriveToolStripMenu,
            this.toolStripSeparator4,
            this.projectSettingsToolStripMenuItem,
            this.projectLoginToolStripMenuItem,
            this.exportToToolboxToolStripMenuItem,
            this.toolStripSeparator2,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator12,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(56, 27);
            this.projectToolStripMenuItem.Text = "&Project";
            this.projectToolStripMenuItem.DropDownOpening += new System.EventHandler(this.projectToolStripMenuItem_DropDownOpening);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.recentProjectsToolStripMenuItem.Text = "&Recent projects";
            this.recentProjectsToolStripMenuItem.ToolTipText = "This shows the projects that have at one time or other been opened on this machin" +
                "e";
            // 
            // sendReceiveToolStripMenuItem
            // 
            this.sendReceiveToolStripMenuItem.Name = "sendReceiveToolStripMenuItem";
            this.sendReceiveToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.sendReceiveToolStripMenuItem.Text = "Sen&d/Receive";
            this.sendReceiveToolStripMenuItem.ToolTipText = "Click to synchronize this project with the Internet (or thumbdrive) repository";
            this.sendReceiveToolStripMenuItem.Click += new System.EventHandler(this.sendReceiveToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShowShortcutKeys = false;
            this.newToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.ToolTipText = "Click to create a new OneStory project";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.ToolTipText = "Click to save the OneStory project";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(274, 6);
            // 
            // browseForProjectToolStripMenuItem
            // 
            this.browseForProjectToolStripMenuItem.Name = "browseForProjectToolStripMenuItem";
            this.browseForProjectToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.browseForProjectToolStripMenuItem.Text = "&Browse for project file";
            this.browseForProjectToolStripMenuItem.ToolTipText = "Click this option to open an existing OneStory project that is not in the default" +
                " project directory.";
            this.browseForProjectToolStripMenuItem.Click += new System.EventHandler(this.browseForProjectToolStripMenuItem_Click);
            // 
            // projectFromTheInternetToolStripMenuItem
            // 
            this.projectFromTheInternetToolStripMenuItem.Name = "projectFromTheInternetToolStripMenuItem";
            this.projectFromTheInternetToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.projectFromTheInternetToolStripMenuItem.Text = "&From the Internet";
            this.projectFromTheInternetToolStripMenuItem.ToolTipText = "Click here to enter an Internet address to get a project from (e.g. if your team " +
                "mates have already uploaded it to the internet repository)";
            this.projectFromTheInternetToolStripMenuItem.Click += new System.EventHandler(this.projectFromTheInternetToolStripMenuItem_Click);
            // 
            // toTheInternetToolStripMenuItem
            // 
            this.toTheInternetToolStripMenuItem.Name = "toTheInternetToolStripMenuItem";
            this.toTheInternetToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.toTheInternetToolStripMenuItem.Text = "&To the Internet";
            this.toTheInternetToolStripMenuItem.ToolTipText = "Click here to enter the Internet address of the repository to send this project t" +
                "o (e.g. if someone has created a repository for this project)";
            this.toTheInternetToolStripMenuItem.Click += new System.EventHandler(this.toTheInternetToolStripMenuItem_Click);
            // 
            // projectFromASharedNetworkDriveToolStripMenu
            // 
            this.projectFromASharedNetworkDriveToolStripMenu.Name = "projectFromASharedNetworkDriveToolStripMenu";
            this.projectFromASharedNetworkDriveToolStripMenu.Size = new System.Drawing.Size(277, 22);
            this.projectFromASharedNetworkDriveToolStripMenu.Text = "&Associate with a shared network folder";
            this.projectFromASharedNetworkDriveToolStripMenu.ToolTipText = "Click here to associate this project with a repository on a network drive (e.g. f" +
                "or working together at a workshop where Internet connectivity is difficult).";
            this.projectFromASharedNetworkDriveToolStripMenu.Click += new System.EventHandler(this.projectFromASharedNetworkDriveToolStripMenu_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(274, 6);
            // 
            // projectSettingsToolStripMenuItem
            // 
            this.projectSettingsToolStripMenuItem.Name = "projectSettingsToolStripMenuItem";
            this.projectSettingsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.projectSettingsToolStripMenuItem.Text = "Se&ttings";
            this.projectSettingsToolStripMenuItem.ToolTipText = "Click this option to go to the Project Settings dialog in order to either log in " +
                "as a team member, add a new team member, or edit the language properties (fonts," +
                " keyboards, etc)";
            this.projectSettingsToolStripMenuItem.Click += new System.EventHandler(this.projectSettingsToolStripMenuItem_Click);
            // 
            // projectLoginToolStripMenuItem
            // 
            this.projectLoginToolStripMenuItem.Name = "projectLoginToolStripMenuItem";
            this.projectLoginToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.projectLoginToolStripMenuItem.Text = "&Login";
            this.projectLoginToolStripMenuItem.ToolTipText = "Click to login with a specific name/role";
            this.projectLoginToolStripMenuItem.Click += new System.EventHandler(this.projectLoginToolStripMenuItem_Click);
            // 
            // exportToToolboxToolStripMenuItem
            // 
            this.exportToToolboxToolStripMenuItem.Name = "exportToToolboxToolStripMenuItem";
            this.exportToToolboxToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.exportToToolboxToolStripMenuItem.Text = "E&xport to Toolbox";
            this.exportToToolboxToolStripMenuItem.Click += new System.EventHandler(this.exportToToolboxToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(274, 6);
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.printPreviewToolStripMenuItem.Text = "&Print";
            this.printPreviewToolStripMenuItem.ToolTipText = "Click here to configure a print preview of the stories that can then be printed";
            this.printPreviewToolStripMenuItem.Click += new System.EventHandler(this.printPreviewToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(274, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.ToolTipText = "Click to exit the program and synchronize with the internet repository";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItemSelectState
            // 
            this.toolStripMenuItemSelectState.Name = "toolStripMenuItemSelectState";
            this.toolStripMenuItemSelectState.Size = new System.Drawing.Size(79, 27);
            this.toolStripMenuItemSelectState.Text = "Select S&tate";
            this.toolStripMenuItemSelectState.Click += new System.EventHandler(this.toolStripMenuItemSelectState_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteBackTranslationToolStripMenuItem,
            this.toolStripSeparator9,
            this.editFindToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.toolStripSeparator10,
            this.editAddRetellingTestResultsToolStripMenuItem,
            this.editAddInferenceTestResultsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 27);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCopySelectionToolStripMenuItem,
            this.copyStoryToolStripMenuItem,
            this.copyNationalBackTranslationToolStripMenuItem,
            this.copyEnglishBackTranslationToolStripMenuItem,
            this.copyFreeTranslationMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // editCopySelectionToolStripMenuItem
            // 
            this.editCopySelectionToolStripMenuItem.Name = "editCopySelectionToolStripMenuItem";
            this.editCopySelectionToolStripMenuItem.Size = new System.Drawing.Size(386, 22);
            this.editCopySelectionToolStripMenuItem.Text = "Sele&ction";
            this.editCopySelectionToolStripMenuItem.ToolTipText = "Copy the selected text from the active text box to the clipboard";
            this.editCopySelectionToolStripMenuItem.Click += new System.EventHandler(this.editCopySelectionToolStripMenuItem_Click);
            // 
            // copyStoryToolStripMenuItem
            // 
            this.copyStoryToolStripMenuItem.Name = "copyStoryToolStripMenuItem";
            this.copyStoryToolStripMenuItem.Size = new System.Drawing.Size(386, 22);
            this.copyStoryToolStripMenuItem.Text = "&Story";
            this.copyStoryToolStripMenuItem.ToolTipText = "Copy all of the lines of text in the story language into one big paragraph of tex" +
                "t";
            this.copyStoryToolStripMenuItem.Click += new System.EventHandler(this.copyStoryToolStripMenuItem_Click);
            // 
            // copyNationalBackTranslationToolStripMenuItem
            // 
            this.copyNationalBackTranslationToolStripMenuItem.Name = "copyNationalBackTranslationToolStripMenuItem";
            this.copyNationalBackTranslationToolStripMenuItem.Size = new System.Drawing.Size(386, 22);
            this.copyNationalBackTranslationToolStripMenuItem.Text = "&National back-translation";
            this.copyNationalBackTranslationToolStripMenuItem.ToolTipText = "Copy all of the lines of text in the National back-translation language into one " +
                "big paragraph of text";
            this.copyNationalBackTranslationToolStripMenuItem.Click += new System.EventHandler(this.copyNationalBackTranslationToolStripMenuItem_Click);
            // 
            // copyEnglishBackTranslationToolStripMenuItem
            // 
            this.copyEnglishBackTranslationToolStripMenuItem.Name = "copyEnglishBackTranslationToolStripMenuItem";
            this.copyEnglishBackTranslationToolStripMenuItem.Size = new System.Drawing.Size(386, 22);
            this.copyEnglishBackTranslationToolStripMenuItem.Text = "&English back-translation of the whole story to the clipboard";
            this.copyEnglishBackTranslationToolStripMenuItem.ToolTipText = "Copy all of the lines of text in the English back-translation into one big paragr" +
                "aph of text";
            this.copyEnglishBackTranslationToolStripMenuItem.Click += new System.EventHandler(this.copyEnglishBackTranslationToolStripMenuItem_Click);
            // 
            // copyFreeTranslationMenuItem
            // 
            this.copyFreeTranslationMenuItem.Name = "copyFreeTranslationMenuItem";
            this.copyFreeTranslationMenuItem.Size = new System.Drawing.Size(386, 22);
            this.copyFreeTranslationMenuItem.Text = "&Free translation of the whole story to the clipboard";
            this.copyFreeTranslationMenuItem.ToolTipText = "Copy all of the lines of text in the Free translation into one big paragraph of t" +
                "ext";
            this.copyFreeTranslationMenuItem.Click += new System.EventHandler(this.copyFreeTranslationMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.ToolTipText = "Paste the contents of the clipboard into the currently selected text box";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteBackTranslationToolStripMenuItem
            // 
            this.deleteBackTranslationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteStoryVersesToolStripMenuItem,
            this.deleteStoryNationalBackTranslationToolStripMenuItem,
            this.deleteEnglishBacktranslationToolStripMenuItem,
            this.deleteFreeTranslationToolStripMenuItem,
            this.deleteTestToolStripMenuItem});
            this.deleteBackTranslationToolStripMenuItem.Name = "deleteBackTranslationToolStripMenuItem";
            this.deleteBackTranslationToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.deleteBackTranslationToolStripMenuItem.Text = "&Delete";
            // 
            // deleteStoryVersesToolStripMenuItem
            // 
            this.deleteStoryVersesToolStripMenuItem.Name = "deleteStoryVersesToolStripMenuItem";
            this.deleteStoryVersesToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteStoryVersesToolStripMenuItem.Text = "&Story (all lines)";
            this.deleteStoryVersesToolStripMenuItem.ToolTipText = "Delete the contents of all of the text boxes of the story in the story language (" +
                "the lines will remain, but just be emptied)";
            this.deleteStoryVersesToolStripMenuItem.Click += new System.EventHandler(this.deleteStoryVersesToolStripMenuItem_Click);
            // 
            // deleteStoryNationalBackTranslationToolStripMenuItem
            // 
            this.deleteStoryNationalBackTranslationToolStripMenuItem.Name = "deleteStoryNationalBackTranslationToolStripMenuItem";
            this.deleteStoryNationalBackTranslationToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteStoryNationalBackTranslationToolStripMenuItem.Text = "&National language back-translation (all lines)";
            this.deleteStoryNationalBackTranslationToolStripMenuItem.ToolTipText = "Delete the contents of all of the text boxes of the national back-translation of " +
                "the story (the lines will remain, but just be emptied)";
            this.deleteStoryNationalBackTranslationToolStripMenuItem.Click += new System.EventHandler(this.deleteStoryNationalBackTranslationToolStripMenuItem_Click);
            // 
            // deleteEnglishBacktranslationToolStripMenuItem
            // 
            this.deleteEnglishBacktranslationToolStripMenuItem.Name = "deleteEnglishBacktranslationToolStripMenuItem";
            this.deleteEnglishBacktranslationToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteEnglishBacktranslationToolStripMenuItem.Text = "&English back-translation (all lines)";
            this.deleteEnglishBacktranslationToolStripMenuItem.ToolTipText = "Delete the contents of all of the text boxes of the English back-translation of t" +
                "he story (the lines will remain, but just be emptied)";
            this.deleteEnglishBacktranslationToolStripMenuItem.Click += new System.EventHandler(this.deleteEnglishBacktranslationToolStripMenuItem_Click);
            // 
            // deleteFreeTranslationToolStripMenuItem
            // 
            this.deleteFreeTranslationToolStripMenuItem.Name = "deleteFreeTranslationToolStripMenuItem";
            this.deleteFreeTranslationToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteFreeTranslationToolStripMenuItem.Text = "&Free translation (all lines)";
            this.deleteFreeTranslationToolStripMenuItem.ToolTipText = "Delete the contents of all of the text boxes of the Free translation of the story" +
                " (the lines will remain, but just be emptied)";
            this.deleteFreeTranslationToolStripMenuItem.Click += new System.EventHandler(this.deleteFreeTranslationToolStripMenuItem_Click);
            // 
            // deleteTestToolStripMenuItem
            // 
            this.deleteTestToolStripMenuItem.Name = "deleteTestToolStripMenuItem";
            this.deleteTestToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteTestToolStripMenuItem.Text = "&Test";
            this.deleteTestToolStripMenuItem.ToolTipText = "Delete the answers to the testing questions and the retellings associated with a " +
                "particular testing helper (UNS). The text boxes will be deleted completely";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(204, 6);
            // 
            // editFindToolStripMenuItem
            // 
            this.editFindToolStripMenuItem.Enabled = false;
            this.editFindToolStripMenuItem.Name = "editFindToolStripMenuItem";
            this.editFindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.editFindToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editFindToolStripMenuItem.Text = "&Find";
            this.editFindToolStripMenuItem.Click += new System.EventHandler(this.editFindToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.findNextToolStripMenuItem.Text = "Find &Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Enabled = false;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.replaceToolStripMenuItem.Text = "&Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(204, 6);
            // 
            // editAddRetellingTestResultsToolStripMenuItem
            // 
            this.editAddRetellingTestResultsToolStripMenuItem.Name = "editAddRetellingTestResultsToolStripMenuItem";
            this.editAddRetellingTestResultsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editAddRetellingTestResultsToolStripMenuItem.Text = "&Add retelling test results";
            this.editAddRetellingTestResultsToolStripMenuItem.ToolTipText = "Click here to add boxes for the retellings of the story";
            this.editAddRetellingTestResultsToolStripMenuItem.Click += new System.EventHandler(this.editAddTestResultsToolStripMenuItem_Click);
            // 
            // editAddInferenceTestResultsToolStripMenuItem
            // 
            this.editAddInferenceTestResultsToolStripMenuItem.Name = "editAddInferenceTestResultsToolStripMenuItem";
            this.editAddInferenceTestResultsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editAddInferenceTestResultsToolStripMenuItem.Text = "&Add inference test results";
            this.editAddInferenceTestResultsToolStripMenuItem.ToolTipText = "Click here to add boxes for the answers to the testing questions";
            this.editAddInferenceTestResultsToolStripMenuItem.Click += new System.EventHandler(this.editAddInferenceTestResultsToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideFieldsToolStripMenuItem,
            this.useSameSettingsForAllStoriesToolStripMenuItem,
            this.toolStripSeparator5,
            this.viewVernacularLangFieldMenuItem,
            this.viewNationalLangFieldMenuItem,
            this.viewEnglishBTFieldMenuItem,
            this.viewFreeTranslationToolStripMenuItem,
            this.viewAnchorFieldMenuItem,
            this.viewStoryTestingQuestionMenuItem,
            this.viewStoryTestingQuestionAnswerMenuItem,
            this.viewRetellingFieldMenuItem,
            this.toolStripSeparator6,
            this.viewConsultantNoteFieldMenuItem,
            this.viewCoachNotesFieldMenuItem,
            this.toolStripSeparator3,
            this.viewNetBibleMenuItem,
            this.toolStripSeparator7,
            this.refreshToolStripMenuItem,
            this.toolStripSeparator8,
            this.historicalDifferencesToolStripMenuItem,
            this.viewLnCNotesMenu,
            this.concordanceToolStripMenuItem,
            this.stateTransitionHistoryToolStripMenuItem,
            this.viewOldStoriesToolStripMenuItem,
            this.toolStripSeparator11,
            this.hiddenVersesToolStripMenuItem,
            this.viewOnlyOpenConversationsMenu,
            this.toolStripSeparator13,
            this.viewTransliterationsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 27);
            this.viewToolStripMenuItem.Text = "&View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // showHideFieldsToolStripMenuItem
            // 
            this.showHideFieldsToolStripMenuItem.Name = "showHideFieldsToolStripMenuItem";
            this.showHideFieldsToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.showHideFieldsToolStripMenuItem.Text = "&Show/Hide multiple fields at once";
            this.showHideFieldsToolStripMenuItem.Click += new System.EventHandler(this.showHideFieldsToolStripMenuItem_Click);
            // 
            // useSameSettingsForAllStoriesToolStripMenuItem
            // 
            this.useSameSettingsForAllStoriesToolStripMenuItem.CheckOnClick = true;
            this.useSameSettingsForAllStoriesToolStripMenuItem.Name = "useSameSettingsForAllStoriesToolStripMenuItem";
            this.useSameSettingsForAllStoriesToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.useSameSettingsForAllStoriesToolStripMenuItem.Text = "&Use same settings for all stories";
            this.useSameSettingsForAllStoriesToolStripMenuItem.Click += new System.EventHandler(this.useSameSettingsForAllStoriesToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(281, 6);
            // 
            // viewVernacularLangFieldMenuItem
            // 
            this.viewVernacularLangFieldMenuItem.Checked = true;
            this.viewVernacularLangFieldMenuItem.CheckOnClick = true;
            this.viewVernacularLangFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewVernacularLangFieldMenuItem.Name = "viewVernacularLangFieldMenuItem";
            this.viewVernacularLangFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewVernacularLangFieldMenuItem.Text = "Story &Language field";
            this.viewVernacularLangFieldMenuItem.ToolTipText = "Show the text boxes for the story lines in the story language";
            this.viewVernacularLangFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewNationalLangFieldMenuItem
            // 
            this.viewNationalLangFieldMenuItem.Checked = true;
            this.viewNationalLangFieldMenuItem.CheckOnClick = true;
            this.viewNationalLangFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNationalLangFieldMenuItem.Name = "viewNationalLangFieldMenuItem";
            this.viewNationalLangFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewNationalLangFieldMenuItem.Text = "National language &back translation field";
            this.viewNationalLangFieldMenuItem.ToolTipText = "Show the text boxes for the national language back-translation of the story lines" +
                "";
            this.viewNationalLangFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewEnglishBTFieldMenuItem
            // 
            this.viewEnglishBTFieldMenuItem.Checked = true;
            this.viewEnglishBTFieldMenuItem.CheckOnClick = true;
            this.viewEnglishBTFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewEnglishBTFieldMenuItem.Name = "viewEnglishBTFieldMenuItem";
            this.viewEnglishBTFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewEnglishBTFieldMenuItem.Text = "&English back translation fields";
            this.viewEnglishBTFieldMenuItem.ToolTipText = "Show the text boxes for the English language back-translation of the story lines";
            this.viewEnglishBTFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewFreeTranslationToolStripMenuItem
            // 
            this.viewFreeTranslationToolStripMenuItem.Checked = true;
            this.viewFreeTranslationToolStripMenuItem.CheckOnClick = true;
            this.viewFreeTranslationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewFreeTranslationToolStripMenuItem.Name = "viewFreeTranslationToolStripMenuItem";
            this.viewFreeTranslationToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewFreeTranslationToolStripMenuItem.Text = "&Free Translation";
            this.viewFreeTranslationToolStripMenuItem.ToolTipText = "Show the text boxes for the Free Translation of the story lines";
            this.viewFreeTranslationToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewAnchorFieldMenuItem
            // 
            this.viewAnchorFieldMenuItem.Checked = true;
            this.viewAnchorFieldMenuItem.CheckOnClick = true;
            this.viewAnchorFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewAnchorFieldMenuItem.Name = "viewAnchorFieldMenuItem";
            this.viewAnchorFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewAnchorFieldMenuItem.Text = "&Anchors";
            this.viewAnchorFieldMenuItem.ToolTipText = "Show the Anchor toolbar";
            this.viewAnchorFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionMenuItem
            // 
            this.viewStoryTestingQuestionMenuItem.Checked = true;
            this.viewStoryTestingQuestionMenuItem.CheckOnClick = true;
            this.viewStoryTestingQuestionMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionMenuItem.Name = "viewStoryTestingQuestionMenuItem";
            this.viewStoryTestingQuestionMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewStoryTestingQuestionMenuItem.Text = "Story &testing questions";
            this.viewStoryTestingQuestionMenuItem.ToolTipText = "Show the text boxes for the testing questions";
            this.viewStoryTestingQuestionMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionAnswerMenuItem
            // 
            this.viewStoryTestingQuestionAnswerMenuItem.Checked = true;
            this.viewStoryTestingQuestionAnswerMenuItem.CheckOnClick = true;
            this.viewStoryTestingQuestionAnswerMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionAnswerMenuItem.Name = "viewStoryTestingQuestionAnswerMenuItem";
            this.viewStoryTestingQuestionAnswerMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewStoryTestingQuestionAnswerMenuItem.Text = "Story testing question ans&wers";
            this.viewStoryTestingQuestionAnswerMenuItem.ToolTipText = "Show the text boxes for the UNS\'s answers to testing questions";
            this.viewStoryTestingQuestionAnswerMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewRetellingFieldMenuItem
            // 
            this.viewRetellingFieldMenuItem.Checked = true;
            this.viewRetellingFieldMenuItem.CheckOnClick = true;
            this.viewRetellingFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewRetellingFieldMenuItem.Name = "viewRetellingFieldMenuItem";
            this.viewRetellingFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewRetellingFieldMenuItem.Text = "&Retellings";
            this.viewRetellingFieldMenuItem.ToolTipText = "Show the text boxes for the UNS retelling responses";
            this.viewRetellingFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(281, 6);
            // 
            // viewConsultantNoteFieldMenuItem
            // 
            this.viewConsultantNoteFieldMenuItem.Checked = true;
            this.viewConsultantNoteFieldMenuItem.CheckOnClick = true;
            this.viewConsultantNoteFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewConsultantNoteFieldMenuItem.Name = "viewConsultantNoteFieldMenuItem";
            this.viewConsultantNoteFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewConsultantNoteFieldMenuItem.Text = "&Consultant notes";
            this.viewConsultantNoteFieldMenuItem.ToolTipText = "Show the Consultant Notes pane";
            this.viewConsultantNoteFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewConsultantNoteFieldMenuItem_CheckedChanged);
            // 
            // viewCoachNotesFieldMenuItem
            // 
            this.viewCoachNotesFieldMenuItem.Checked = true;
            this.viewCoachNotesFieldMenuItem.CheckOnClick = true;
            this.viewCoachNotesFieldMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewCoachNotesFieldMenuItem.Name = "viewCoachNotesFieldMenuItem";
            this.viewCoachNotesFieldMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewCoachNotesFieldMenuItem.Text = "Coach &notes";
            this.viewCoachNotesFieldMenuItem.ToolTipText = "Show the Coach Notes pane";
            this.viewCoachNotesFieldMenuItem.CheckedChanged += new System.EventHandler(this.viewCoachNotesFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(281, 6);
            // 
            // viewNetBibleMenuItem
            // 
            this.viewNetBibleMenuItem.Checked = true;
            this.viewNetBibleMenuItem.CheckOnClick = true;
            this.viewNetBibleMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNetBibleMenuItem.Name = "viewNetBibleMenuItem";
            this.viewNetBibleMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewNetBibleMenuItem.Text = "&Bible viewer";
            this.viewNetBibleMenuItem.ToolTipText = "Show the Bible Viewer pane";
            this.viewNetBibleMenuItem.CheckedChanged += new System.EventHandler(this.viewNetBibleMenuItem_CheckedChanged);
            this.viewNetBibleMenuItem.Click += new System.EventHandler(this.viewNetBibleMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(281, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Enabled = false;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.refreshToolStripMenuItem.Text = "Re&fresh";
            this.refreshToolStripMenuItem.ToolTipText = "Refresh the screen (if it doesn\'t look like it updated something properly)";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(281, 6);
            // 
            // historicalDifferencesToolStripMenuItem
            // 
            this.historicalDifferencesToolStripMenuItem.Name = "historicalDifferencesToolStripMenuItem";
            this.historicalDifferencesToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.historicalDifferencesToolStripMenuItem.Text = "Historical di&fferences";
            this.historicalDifferencesToolStripMenuItem.ToolTipText = "Click to launch the Revision History dialog to compare different, saved versions " +
                "of this story";
            this.historicalDifferencesToolStripMenuItem.Click += new System.EventHandler(this.historicalDifferencesToolStripMenuItem_Click);
            // 
            // viewLnCNotesMenu
            // 
            this.viewLnCNotesMenu.Name = "viewLnCNotesMenu";
            this.viewLnCNotesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewLnCNotesMenu.Text = "L && C Notes";
            this.viewLnCNotesMenu.Click += new System.EventHandler(this.viewLnCNotesMenu_Click);
            // 
            // concordanceToolStripMenuItem
            // 
            this.concordanceToolStripMenuItem.Name = "concordanceToolStripMenuItem";
            this.concordanceToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.concordanceToolStripMenuItem.Text = "Concor&dance";
            this.concordanceToolStripMenuItem.ToolTipText = "Click to launch the Concordance dialog to search for the occurrances of words in " +
                "the story";
            this.concordanceToolStripMenuItem.Click += new System.EventHandler(this.concordanceToolStripMenuItem_Click);
            // 
            // stateTransitionHistoryToolStripMenuItem
            // 
            this.stateTransitionHistoryToolStripMenuItem.Name = "stateTransitionHistoryToolStripMenuItem";
            this.stateTransitionHistoryToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.stateTransitionHistoryToolStripMenuItem.Text = "&State Transition History";
            this.stateTransitionHistoryToolStripMenuItem.Click += new System.EventHandler(this.stateTransitionHistoryToolStripMenuItem_Click);
            // 
            // viewOldStoriesToolStripMenuItem
            // 
            this.viewOldStoriesToolStripMenuItem.Name = "viewOldStoriesToolStripMenuItem";
            this.viewOldStoriesToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewOldStoriesToolStripMenuItem.Text = "&Old Stories";
            this.viewOldStoriesToolStripMenuItem.ToolTipText = "View older (obsolete) versions of the stories (that were earlier stored in the \'O" +
                "ld Stories\' list from the \'Panorama View\' window--see \'Panorama\' menu, \'Show\' co" +
                "mmand)";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(281, 6);
            // 
            // hiddenVersesToolStripMenuItem
            // 
            this.hiddenVersesToolStripMenuItem.CheckOnClick = true;
            this.hiddenVersesToolStripMenuItem.Name = "hiddenVersesToolStripMenuItem";
            this.hiddenVersesToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.hiddenVersesToolStripMenuItem.Text = "H&idden lines";
            this.hiddenVersesToolStripMenuItem.ToolTipText = "Check this menu to show hidden lines and hidden consultant note comments";
            this.hiddenVersesToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.hiddenVersesToolStripMenuItem_CheckStateChanged);
            // 
            // viewOnlyOpenConversationsMenu
            // 
            this.viewOnlyOpenConversationsMenu.CheckOnClick = true;
            this.viewOnlyOpenConversationsMenu.Name = "viewOnlyOpenConversationsMenu";
            this.viewOnlyOpenConversationsMenu.Size = new System.Drawing.Size(284, 22);
            this.viewOnlyOpenConversationsMenu.Text = "Onl&y open conversations";
            this.viewOnlyOpenConversationsMenu.ToolTipText = "Check this menu to hide all closed conversations (i.e. whose \"End Conversation\" b" +
                "utton has been clicked)";
            this.viewOnlyOpenConversationsMenu.CheckStateChanged += new System.EventHandler(this.viewOnlyOpenConversationsMenu_CheckStateChanged);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(281, 6);
            // 
            // viewTransliterationsToolStripMenuItem
            // 
            this.viewTransliterationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliterationVernacular,
            this.viewTransliterationNationalBT});
            this.viewTransliterationsToolStripMenuItem.Name = "viewTransliterationsToolStripMenuItem";
            this.viewTransliterationsToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this.viewTransliterationsToolStripMenuItem.Text = "&Transliterations";
            this.viewTransliterationsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewTransliterationsToolStripMenuItem_DropDownOpening);
            // 
            // viewTransliterationVernacular
            // 
            this.viewTransliterationVernacular.CheckOnClick = true;
            this.viewTransliterationVernacular.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorVernacularConfigureToolStripMenuItem});
            this.viewTransliterationVernacular.Name = "viewTransliterationVernacular";
            this.viewTransliterationVernacular.Size = new System.Drawing.Size(156, 22);
            this.viewTransliterationVernacular.Text = "Story Language";
            this.viewTransliterationVernacular.Click += new System.EventHandler(this.viewTransliterationVernacular_Click);
            // 
            // viewTransliteratorVernacularConfigureToolStripMenuItem
            // 
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Name = "viewTransliteratorVernacularConfigureToolStripMenuItem";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Text = "&Configure";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorVernacularConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationNationalBT
            // 
            this.viewTransliterationNationalBT.CheckOnClick = true;
            this.viewTransliterationNationalBT.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem});
            this.viewTransliterationNationalBT.Name = "viewTransliterationNationalBT";
            this.viewTransliterationNationalBT.Size = new System.Drawing.Size(156, 22);
            this.viewTransliterationNationalBT.Text = "National BT";
            this.viewTransliterationNationalBT.Click += new System.EventHandler(this.viewTransliterationNationalBT_Click);
            // 
            // viewTransliteratorNationalBTConfigureToolStripMenuItem
            // 
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Name = "viewTransliteratorNationalBTConfigureToolStripMenuItem";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Text = "&Configure";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorNationalBTConfigureToolStripMenuItem_Click);
            // 
            // comboBoxStorySelector
            // 
            this.comboBoxStorySelector.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.comboBoxStorySelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxStorySelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxStorySelector.Font = new System.Drawing.Font("Arial Unicode MS", 10F);
            this.comboBoxStorySelector.MaxDropDownItems = 30;
            this.comboBoxStorySelector.Name = "comboBoxStorySelector";
            this.comboBoxStorySelector.Size = new System.Drawing.Size(290, 27);
            this.comboBoxStorySelector.Text = "<to create a story, type its name here and hit Enter>";
            this.comboBoxStorySelector.ToolTipText = "Select the Story to edit or type in a new name to add a new story";
            this.comboBoxStorySelector.SelectedIndexChanged += new System.EventHandler(this.comboBoxStorySelector_SelectedIndexChanged);
            this.comboBoxStorySelector.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStorySelector_KeyUp);
            // 
            // storyToolStripMenuItem
            // 
            this.storyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem,
            this.deleteStoryToolStripMenuItem,
            this.storyCopyWithNewNameToolStripMenuItem,
            this.splitIntoLinesToolStripMenuItem,
            this.realignStoryVersesToolStripMenuItem,
            this.toolStripSeparator14,
            this.useAdaptItForBacktranslationToolStripMenuItem});
            this.storyToolStripMenuItem.Name = "storyToolStripMenuItem";
            this.storyToolStripMenuItem.Size = new System.Drawing.Size(46, 27);
            this.storyToolStripMenuItem.Text = "&Story";
            this.storyToolStripMenuItem.DropDownOpening += new System.EventHandler(this.storyToolStripMenuItem_DropDownOpening);
            // 
            // enterTheReasonThisStoryIsInTheSetToolStripMenuItem
            // 
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Name = "enterTheReasonThisStoryIsInTheSetToolStripMenuItem";
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Text = "S&tory Information";
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem.ToolTipText = "Enter information about this story, such as the reason it\'s in the set, the resou" +
                "rces used, etc.";
            this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem.Click += new System.EventHandler(this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click);
            // 
            // deleteStoryToolStripMenuItem
            // 
            this.deleteStoryToolStripMenuItem.Name = "deleteStoryToolStripMenuItem";
            this.deleteStoryToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.deleteStoryToolStripMenuItem.Text = "&Delete story";
            this.deleteStoryToolStripMenuItem.ToolTipText = "Click to delete the story currently shown";
            this.deleteStoryToolStripMenuItem.Click += new System.EventHandler(this.deleteStoryToolStripMenuItem_Click);
            // 
            // storyCopyWithNewNameToolStripMenuItem
            // 
            this.storyCopyWithNewNameToolStripMenuItem.Name = "storyCopyWithNewNameToolStripMenuItem";
            this.storyCopyWithNewNameToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.storyCopyWithNewNameToolStripMenuItem.Text = "&Copy with new name";
            this.storyCopyWithNewNameToolStripMenuItem.ToolTipText = "Click to make a duplicate copy of the current story with a new name";
            this.storyCopyWithNewNameToolStripMenuItem.Click += new System.EventHandler(this.storyCopyWithNewNameToolStripMenuItem_Click);
            // 
            // splitIntoLinesToolStripMenuItem
            // 
            this.splitIntoLinesToolStripMenuItem.Name = "splitIntoLinesToolStripMenuItem";
            this.splitIntoLinesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.splitIntoLinesToolStripMenuItem.Text = "S&plit into Lines";
            this.splitIntoLinesToolStripMenuItem.Click += new System.EventHandler(this.splitIntoLinesToolStripMenuItem_Click);
            // 
            // realignStoryVersesToolStripMenuItem
            // 
            this.realignStoryVersesToolStripMenuItem.Name = "realignStoryVersesToolStripMenuItem";
            this.realignStoryVersesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.realignStoryVersesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.realignStoryVersesToolStripMenuItem.Text = "&Re-align story lines";
            this.realignStoryVersesToolStripMenuItem.Click += new System.EventHandler(this.realignStoryVersesToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(242, 6);
            // 
            // useAdaptItForBacktranslationToolStripMenuItem
            // 
            this.useAdaptItForBacktranslationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storyAdaptItVernacularToNationalMenuItem,
            this.storyAdaptItVernacularToEnglishMenuItem,
            this.storyAdaptItNationalToEnglishMenuItem});
            this.useAdaptItForBacktranslationToolStripMenuItem.Name = "useAdaptItForBacktranslationToolStripMenuItem";
            this.useAdaptItForBacktranslationToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.useAdaptItForBacktranslationToolStripMenuItem.Text = "&Use Adapt It for back-translation";
            // 
            // storyAdaptItVernacularToNationalMenuItem
            // 
            this.storyAdaptItVernacularToNationalMenuItem.Name = "storyAdaptItVernacularToNationalMenuItem";
            this.storyAdaptItVernacularToNationalMenuItem.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItVernacularToNationalMenuItem.Text = "&Story language to National language";
            this.storyAdaptItVernacularToNationalMenuItem.Click += new System.EventHandler(this.storyAdaptItVernacularToNationalMenuItem_Click);
            // 
            // storyAdaptItVernacularToEnglishMenuItem
            // 
            this.storyAdaptItVernacularToEnglishMenuItem.Name = "storyAdaptItVernacularToEnglishMenuItem";
            this.storyAdaptItVernacularToEnglishMenuItem.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItVernacularToEnglishMenuItem.Text = "Story &language to English";
            this.storyAdaptItVernacularToEnglishMenuItem.Click += new System.EventHandler(this.storyAdaptItVernacularToEnglishMenuItem_Click);
            // 
            // storyAdaptItNationalToEnglishMenuItem
            // 
            this.storyAdaptItNationalToEnglishMenuItem.Name = "storyAdaptItNationalToEnglishMenuItem";
            this.storyAdaptItNationalToEnglishMenuItem.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItNationalToEnglishMenuItem.Text = "National language to English";
            this.storyAdaptItNationalToEnglishMenuItem.Click += new System.EventHandler(this.storyAdaptItNationalToEnglishMenuItem_Click);
            // 
            // panoramaToolStripMenuItem
            // 
            this.panoramaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertNewStoryToolStripMenuItem,
            this.addNewStoryAfterToolStripMenuItem,
            this.toolStripMenuItemShowPanorama});
            this.panoramaToolStripMenuItem.Name = "panoramaToolStripMenuItem";
            this.panoramaToolStripMenuItem.Size = new System.Drawing.Size(73, 27);
            this.panoramaToolStripMenuItem.Text = "Pa&norama";
            this.panoramaToolStripMenuItem.DropDownOpening += new System.EventHandler(this.panoramaToolStripMenuItem_DropDownOpening);
            // 
            // insertNewStoryToolStripMenuItem
            // 
            this.insertNewStoryToolStripMenuItem.Name = "insertNewStoryToolStripMenuItem";
            this.insertNewStoryToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.insertNewStoryToolStripMenuItem.Text = "&Insert new story before current";
            this.insertNewStoryToolStripMenuItem.ToolTipText = "Click to insert a new, empty story before the one currently shown";
            this.insertNewStoryToolStripMenuItem.Click += new System.EventHandler(this.insertNewStoryToolStripMenuItem_Click);
            // 
            // addNewStoryAfterToolStripMenuItem
            // 
            this.addNewStoryAfterToolStripMenuItem.Name = "addNewStoryAfterToolStripMenuItem";
            this.addNewStoryAfterToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.addNewStoryAfterToolStripMenuItem.Text = "&Add new story after current";
            this.addNewStoryAfterToolStripMenuItem.ToolTipText = "Click to add a new, empty story after the one currently shown";
            this.addNewStoryAfterToolStripMenuItem.Click += new System.EventHandler(this.addNewStoryAfterToolStripMenuItem_Click);
            // 
            // toolStripMenuItemShowPanorama
            // 
            this.toolStripMenuItemShowPanorama.Name = "toolStripMenuItemShowPanorama";
            this.toolStripMenuItemShowPanorama.Size = new System.Drawing.Size(235, 22);
            this.toolStripMenuItemShowPanorama.Text = "&Show";
            this.toolStripMenuItemShowPanorama.ToolTipText = "Show the Panorama View window to see all the stories in the set and their current" +
                " state";
            this.toolStripMenuItemShowPanorama.Click += new System.EventHandler(this.toolStripMenuItemShowPanorama_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeProjectFolderRootToolStripMenuItem,
            this.resetStoredInformationToolStripMenuItem,
            this.changeStateWithoutChecksToolStripMenuItem,
            this.programUpdatesToolStripMenuItem,
            this.saveTimeoutToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 27);
            this.advancedToolStripMenuItem.Text = "A&dvanced";
            this.advancedToolStripMenuItem.DropDownOpening += new System.EventHandler(this.advancedToolStripMenuItem_DropDownOpening);
            // 
            // changeProjectFolderRootToolStripMenuItem
            // 
            this.changeProjectFolderRootToolStripMenuItem.Name = "changeProjectFolderRootToolStripMenuItem";
            this.changeProjectFolderRootToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.changeProjectFolderRootToolStripMenuItem.Text = "Change &Project Folder Root";
            this.changeProjectFolderRootToolStripMenuItem.Click += new System.EventHandler(this.changeProjectFolderRootToolStripMenuItem_Click);
            // 
            // resetStoredInformationToolStripMenuItem
            // 
            this.resetStoredInformationToolStripMenuItem.Name = "resetStoredInformationToolStripMenuItem";
            this.resetStoredInformationToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.resetStoredInformationToolStripMenuItem.Text = "&Reset Stored Information";
            this.resetStoredInformationToolStripMenuItem.ToolTipText = resources.GetString("resetStoredInformationToolStripMenuItem.ToolTipText");
            this.resetStoredInformationToolStripMenuItem.Click += new System.EventHandler(this.resetStoredInformationToolStripMenuItem_Click);
            // 
            // changeStateWithoutChecksToolStripMenuItem
            // 
            this.changeStateWithoutChecksToolStripMenuItem.Name = "changeStateWithoutChecksToolStripMenuItem";
            this.changeStateWithoutChecksToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.changeStateWithoutChecksToolStripMenuItem.Text = "Change &State without checks";
            this.changeStateWithoutChecksToolStripMenuItem.ToolTipText = resources.GetString("changeStateWithoutChecksToolStripMenuItem.ToolTipText");
            this.changeStateWithoutChecksToolStripMenuItem.Click += new System.EventHandler(this.changeStateWithoutChecksToolStripMenuItem_Click);
            // 
            // programUpdatesToolStripMenuItem
            // 
            this.programUpdatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.automaticallyCheckAtStartupToolStripMenuItem,
            this.checkForProgramUpdatesNowToolStripMenuItem});
            this.programUpdatesToolStripMenuItem.Name = "programUpdatesToolStripMenuItem";
            this.programUpdatesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.programUpdatesToolStripMenuItem.Text = "Program &Updates";
            this.programUpdatesToolStripMenuItem.DropDownOpening += new System.EventHandler(this.programUpdatesToolStripMenuItem_DropDownOpening);
            // 
            // automaticallyCheckAtStartupToolStripMenuItem
            // 
            this.automaticallyCheckAtStartupToolStripMenuItem.CheckOnClick = true;
            this.automaticallyCheckAtStartupToolStripMenuItem.Name = "automaticallyCheckAtStartupToolStripMenuItem";
            this.automaticallyCheckAtStartupToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.automaticallyCheckAtStartupToolStripMenuItem.Text = "&Automatically check at startup";
            this.automaticallyCheckAtStartupToolStripMenuItem.ToolTipText = "Uncheck this menu to stop the program from automatically checking for program upd" +
                "ates when the program is started (this can save startup time)";
            this.automaticallyCheckAtStartupToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.automaticallyCheckAtStartupToolStripMenuItem_CheckStateChanged);
            // 
            // checkForProgramUpdatesNowToolStripMenuItem
            // 
            this.checkForProgramUpdatesNowToolStripMenuItem.Name = "checkForProgramUpdatesNowToolStripMenuItem";
            this.checkForProgramUpdatesNowToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.checkForProgramUpdatesNowToolStripMenuItem.Text = "&Check now";
            this.checkForProgramUpdatesNowToolStripMenuItem.ToolTipText = "Click this menu to have the program manually check for program updates";
            this.checkForProgramUpdatesNowToolStripMenuItem.Click += new System.EventHandler(this.checkForProgramUpdatesNowToolStripMenuItem_Click);
            // 
            // saveTimeoutToolStripMenuItem
            // 
            this.saveTimeoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enabledToolStripMenuItem,
            this.asSilentlyAsPossibleToolStripMenuItem});
            this.saveTimeoutToolStripMenuItem.Name = "saveTimeoutToolStripMenuItem";
            this.saveTimeoutToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.saveTimeoutToolStripMenuItem.Text = "Save &Timeout";
            // 
            // enabledToolStripMenuItem
            // 
            this.enabledToolStripMenuItem.CheckOnClick = true;
            this.enabledToolStripMenuItem.Name = "enabledToolStripMenuItem";
            this.enabledToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.enabledToolStripMenuItem.Text = "&Enabled";
            this.enabledToolStripMenuItem.ToolTipText = "This menu enables a 5 minute timeout to remind you to save (disable at your own r" +
                "isk)";
            this.enabledToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.enabledToolStripMenuItem_CheckStateChanged);
            // 
            // asSilentlyAsPossibleToolStripMenuItem
            // 
            this.asSilentlyAsPossibleToolStripMenuItem.CheckOnClick = true;
            this.asSilentlyAsPossibleToolStripMenuItem.Name = "asSilentlyAsPossibleToolStripMenuItem";
            this.asSilentlyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.asSilentlyAsPossibleToolStripMenuItem.Text = "&As silently as possible";
            this.asSilentlyAsPossibleToolStripMenuItem.ToolTipText = "This menu indicates whether the program will query you (unchecked) or not (checke" +
                "d) to do the save";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 27);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.splitContainerLeftRight.Location = new System.Drawing.Point(0, 31);
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
            this.splitContainerLeftRight.Size = new System.Drawing.Size(881, 470);
            this.splitContainerLeftRight.SplitterDistance = 453;
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
            this.splitContainerUpDown.Panel1.Controls.Add(this.linkLabelVerseBT);
            this.splitContainerUpDown.Panel1.Controls.Add(this.flowLayoutPanelVerses);
            this.splitContainerUpDown.Panel1.Controls.Add(this.textBoxStoryVerse);
            // 
            // splitContainerUpDown.Panel2
            // 
            this.splitContainerUpDown.Panel2.Controls.Add(this.netBibleViewer);
            this.splitContainerUpDown.Size = new System.Drawing.Size(453, 470);
            this.splitContainerUpDown.SplitterDistance = 300;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // linkLabelVerseBT
            // 
            this.linkLabelVerseBT.AutoSize = true;
            this.linkLabelVerseBT.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelVerseBT, "Click here to jump to the indicated line number. You can also right-click on this" +
                    " to get a list of all lines to jump to.");
            this.linkLabelVerseBT.Location = new System.Drawing.Point(11, 5);
            this.linkLabelVerseBT.Name = "linkLabelVerseBT";
            this.helpProvider.SetShowHelp(this.linkLabelVerseBT, true);
            this.linkLabelVerseBT.Size = new System.Drawing.Size(31, 13);
            this.linkLabelVerseBT.TabIndex = 4;
            this.linkLabelVerseBT.TabStop = true;
            this.linkLabelVerseBT.Tag = 1;
            this.linkLabelVerseBT.Text = "Ln: 1";
            this.linkLabelVerseBT.Visible = false;
            this.linkLabelVerseBT.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVerseBT_LinkClicked);
            // 
            // contextMenuStripVerseList
            // 
            this.contextMenuStripVerseList.Name = "contextMenuStripVerseList";
            this.contextMenuStripVerseList.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStripVerseList.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripVerseList_Opening);
            // 
            // flowLayoutPanelVerses
            // 
            this.flowLayoutPanelVerses.AutoScroll = true;
            this.flowLayoutPanelVerses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelVerses.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelVerses.LastControlIntoView = null;
            this.flowLayoutPanelVerses.LineNumberLink = null;
            this.flowLayoutPanelVerses.Location = new System.Drawing.Point(0, 23);
            this.flowLayoutPanelVerses.Name = "flowLayoutPanelVerses";
            this.flowLayoutPanelVerses.Size = new System.Drawing.Size(451, 275);
            this.flowLayoutPanelVerses.TabIndex = 1;
            this.flowLayoutPanelVerses.WrapContents = false;
            this.flowLayoutPanelVerses.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CheckBiblePaneCursorPositionMouseMove);
            // 
            // textBoxStoryVerse
            // 
            this.textBoxStoryVerse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxStoryVerse.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxStoryVerse.Enabled = false;
            this.textBoxStoryVerse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStoryVerse.Location = new System.Drawing.Point(0, 0);
            this.textBoxStoryVerse.Name = "textBoxStoryVerse";
            this.textBoxStoryVerse.ReadOnly = true;
            this.textBoxStoryVerse.Size = new System.Drawing.Size(451, 23);
            this.textBoxStoryVerse.TabIndex = 3;
            this.textBoxStoryVerse.TabStop = false;
            this.textBoxStoryVerse.Text = "Story/BT";
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
            this.netBibleViewer.Size = new System.Drawing.Size(451, 164);
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
            this.splitContainerMentorNotes.Panel1.Controls.Add(this.linkLabelConsultantNotes);
            this.splitContainerMentorNotes.Panel1.Controls.Add(this.htmlConsultantNotesControl);
            this.splitContainerMentorNotes.Panel1.Controls.Add(this.textBoxConsultantNotesTable);
            // 
            // splitContainerMentorNotes.Panel2
            // 
            this.splitContainerMentorNotes.Panel2.Controls.Add(this.linkLabelCoachNotes);
            this.splitContainerMentorNotes.Panel2.Controls.Add(this.htmlCoachNotesControl);
            this.splitContainerMentorNotes.Panel2.Controls.Add(this.textBoxCoachNotes);
            this.splitContainerMentorNotes.Size = new System.Drawing.Size(424, 470);
            this.splitContainerMentorNotes.SplitterDistance = 273;
            this.splitContainerMentorNotes.TabIndex = 0;
            // 
            // linkLabelConsultantNotes
            // 
            this.linkLabelConsultantNotes.AutoSize = true;
            this.linkLabelConsultantNotes.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelConsultantNotes, "Click here to jump to the indicated line number. You can also right-click on this" +
                    " to get a list of all lines to jump to.");
            this.linkLabelConsultantNotes.Location = new System.Drawing.Point(11, 5);
            this.linkLabelConsultantNotes.Name = "linkLabelConsultantNotes";
            this.helpProvider.SetShowHelp(this.linkLabelConsultantNotes, true);
            this.linkLabelConsultantNotes.Size = new System.Drawing.Size(64, 13);
            this.linkLabelConsultantNotes.TabIndex = 3;
            this.linkLabelConsultantNotes.TabStop = true;
            this.linkLabelConsultantNotes.Tag = 0;
            this.linkLabelConsultantNotes.Text = "Story (Ln: 0)";
            this.linkLabelConsultantNotes.Visible = false;
            this.linkLabelConsultantNotes.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelConsultantNotes_LinkClicked);
            // 
            // htmlConsultantNotesControl
            // 
            this.htmlConsultantNotesControl.AllowWebBrowserDrop = false;
            this.htmlConsultantNotesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlConsultantNotesControl.Location = new System.Drawing.Point(0, 23);
            this.htmlConsultantNotesControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlConsultantNotesControl.Name = "htmlConsultantNotesControl";
            this.htmlConsultantNotesControl.Size = new System.Drawing.Size(422, 248);
            this.htmlConsultantNotesControl.StoryData = null;
            this.htmlConsultantNotesControl.TabIndex = 2;
            this.htmlConsultantNotesControl.TheSE = null;
            // 
            // textBoxConsultantNotesTable
            // 
            this.textBoxConsultantNotesTable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxConsultantNotesTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxConsultantNotesTable.Enabled = false;
            this.textBoxConsultantNotesTable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConsultantNotesTable.Location = new System.Drawing.Point(0, 0);
            this.textBoxConsultantNotesTable.Name = "textBoxConsultantNotesTable";
            this.textBoxConsultantNotesTable.ReadOnly = true;
            this.textBoxConsultantNotesTable.Size = new System.Drawing.Size(422, 23);
            this.textBoxConsultantNotesTable.TabIndex = 1;
            this.textBoxConsultantNotesTable.TabStop = false;
            this.textBoxConsultantNotesTable.Text = "Consultant Notes";
            this.textBoxConsultantNotesTable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // linkLabelCoachNotes
            // 
            this.linkLabelCoachNotes.AutoSize = true;
            this.linkLabelCoachNotes.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelCoachNotes, "Click here to jump to the indicated line number. You can also right-click on this" +
                    " to get a list of all lines to jump to.");
            this.linkLabelCoachNotes.Location = new System.Drawing.Point(11, 5);
            this.linkLabelCoachNotes.Name = "linkLabelCoachNotes";
            this.helpProvider.SetShowHelp(this.linkLabelCoachNotes, true);
            this.linkLabelCoachNotes.Size = new System.Drawing.Size(64, 13);
            this.linkLabelCoachNotes.TabIndex = 4;
            this.linkLabelCoachNotes.TabStop = true;
            this.linkLabelCoachNotes.Tag = 0;
            this.linkLabelCoachNotes.Text = "Story (Ln: 0)";
            this.linkLabelCoachNotes.Visible = false;
            this.linkLabelCoachNotes.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCoachNotes_LinkClicked);
            // 
            // htmlCoachNotesControl
            // 
            this.htmlCoachNotesControl.AllowWebBrowserDrop = false;
            this.htmlCoachNotesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlCoachNotesControl.Location = new System.Drawing.Point(0, 23);
            this.htmlCoachNotesControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlCoachNotesControl.Name = "htmlCoachNotesControl";
            this.htmlCoachNotesControl.Size = new System.Drawing.Size(422, 168);
            this.htmlCoachNotesControl.StoryData = null;
            this.htmlCoachNotesControl.TabIndex = 3;
            this.htmlCoachNotesControl.TheSE = null;
            // 
            // textBoxCoachNotes
            // 
            this.textBoxCoachNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCoachNotes.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxCoachNotes.Enabled = false;
            this.textBoxCoachNotes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCoachNotes.Location = new System.Drawing.Point(0, 0);
            this.textBoxCoachNotes.Name = "textBoxCoachNotes";
            this.textBoxCoachNotes.ReadOnly = true;
            this.textBoxCoachNotes.Size = new System.Drawing.Size(422, 23);
            this.textBoxCoachNotes.TabIndex = 2;
            this.textBoxCoachNotes.TabStop = false;
            this.textBoxCoachNotes.Text = "Coach Notes";
            this.textBoxCoachNotes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStripRecordNavigation
            // 
            this.toolStripRecordNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStripRecordNavigation.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripRecordNavigation.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripRecordNavigation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonFirst,
            this.toolStripButtonPrevious,
            this.toolStripButtonNext,
            this.toolStripButtonLast});
            this.toolStripRecordNavigation.Location = new System.Drawing.Point(494, 0);
            this.toolStripRecordNavigation.Name = "toolStripRecordNavigation";
            this.toolStripRecordNavigation.Size = new System.Drawing.Size(95, 25);
            this.toolStripRecordNavigation.TabIndex = 3;
            this.toolStripRecordNavigation.Text = "toolStrip1";
            // 
            // toolStripButtonFirst
            // 
            this.toolStripButtonFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFirst.Image = global::OneStoryProjectEditor.Properties.Resources.DataContainer_MoveFirstHS;
            this.toolStripButtonFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFirst.Name = "toolStripButtonFirst";
            this.toolStripButtonFirst.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFirst.Text = "First Story";
            this.toolStripButtonFirst.ToolTipText = "Click to go to the first story (hold down the Ctrl key and click to keep the same" +
                " fields visible)";
            this.toolStripButtonFirst.Click += new System.EventHandler(this.toolStripButtonFirst_Click);
            // 
            // toolStripButtonPrevious
            // 
            this.toolStripButtonPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevious.Image = global::OneStoryProjectEditor.Properties.Resources.DataContainer_MovePreviousHS;
            this.toolStripButtonPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevious.Name = "toolStripButtonPrevious";
            this.toolStripButtonPrevious.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevious.Text = "Previous Story";
            this.toolStripButtonPrevious.ToolTipText = "Click to go to the previous story (hold down the Ctrl key and click to keep the s" +
                "ame fields visible)";
            this.toolStripButtonPrevious.Click += new System.EventHandler(this.toolStripButtonPrevious_Click);
            // 
            // toolStripButtonNext
            // 
            this.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNext.Image = global::OneStoryProjectEditor.Properties.Resources.DataContainer_MoveNextHS;
            this.toolStripButtonNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNext.Name = "toolStripButtonNext";
            this.toolStripButtonNext.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNext.Text = "Next Story";
            this.toolStripButtonNext.ToolTipText = "Click to go to the next story (hold down the Ctrl key and click to keep the same " +
                "fields visible)";
            this.toolStripButtonNext.Click += new System.EventHandler(this.toolStripButtonNext_Click);
            // 
            // toolStripButtonLast
            // 
            this.toolStripButtonLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLast.Image = global::OneStoryProjectEditor.Properties.Resources.DataContainer_MoveLastHS;
            this.toolStripButtonLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLast.Name = "toolStripButtonLast";
            this.toolStripButtonLast.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLast.Text = "Last Story";
            this.toolStripButtonLast.ToolTipText = "Click to go to the last story (hold down the Ctrl key and click to keep the same " +
                "fields visible)";
            this.toolStripButtonLast.Click += new System.EventHandler(this.toolStripButtonLast_Click);
            // 
            // buttonsStoryStage
            // 
            this.buttonsStoryStage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonsStoryStage.DropDownButtonWidth = 0;
            this.buttonsStoryStage.Image = ((System.Drawing.Image)(resources.GetObject("buttonsStoryStage.Image")));
            this.buttonsStoryStage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonsStoryStage.Name = "buttonsStoryStage";
            this.buttonsStoryStage.Size = new System.Drawing.Size(65, 20);
            this.buttonsStoryStage.Text = "Next State";
            this.buttonsStoryStage.ToolTipText = "Use the \'Select State\' menu to move to a previous state";
            this.buttonsStoryStage.Click += new System.EventHandler(this.buttonsStoryStage_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(801, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLabel.Click += new System.EventHandler(this.statusLabel_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonsStoryStage,
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 501);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(881, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 523);
            this.Controls.Add(this.toolStripRecordNavigation);
            this.Controls.Add(this.splitContainerLeftRight);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "StoryEditor";
            this.Text = "OneStory Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StoryEditor_FormClosing);
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
            this.toolStripRecordNavigation.ResumeLayout(false);
            this.toolStripRecordNavigation.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem projectToolStripMenuItem;
        private ToolStripMenuItem browseForProjectToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem projectSettingsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        internal VerseBtLineFlowLayoutPanel flowLayoutPanelVerses;
        internal SplitContainer splitContainerLeftRight;
        internal MinimizableSplitterContainer splitContainerUpDown;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        internal ToolStripMenuItem viewVernacularLangFieldMenuItem;
        internal ToolStripMenuItem viewNationalLangFieldMenuItem;
        internal ToolStripMenuItem viewEnglishBTFieldMenuItem;
        internal ToolStripMenuItem viewAnchorFieldMenuItem;
        internal ToolStripMenuItem viewStoryTestingQuestionMenuItem;
        internal ToolStripMenuItem viewRetellingFieldMenuItem;
        internal ToolStripMenuItem viewConsultantNoteFieldMenuItem;
        internal ToolStripMenuItem viewNetBibleMenuItem;
        internal ToolStripMenuItem viewCoachNotesFieldMenuItem;
        private NetBibleViewer netBibleViewer;
        private ToolStripSeparator toolStripSeparator3;
        private SplitContainer splitContainerMentorNotes;
        private TextBox textBoxConsultantNotesTable;
        private TextBox textBoxCoachNotes;
        private TextBox textBoxStoryVerse;
        private ToolStripComboBox comboBoxStorySelector;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem recentProjectsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private HelpProvider helpProvider;
        private ToolStripMenuItem insertNewStoryToolStripMenuItem;
        private ToolStripMenuItem deleteStoryToolStripMenuItem;
        private ToolStripMenuItem addNewStoryAfterToolStripMenuItem;
        private ToolStripMenuItem enterTheReasonThisStoryIsInTheSetToolStripMenuItem;
        private ToolStripMenuItem storyToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem copyStoryToolStripMenuItem;
        private ToolStripMenuItem copyNationalBackTranslationToolStripMenuItem;
        private ToolStripMenuItem copyEnglishBackTranslationToolStripMenuItem;
        private ToolStripMenuItem deleteBackTranslationToolStripMenuItem;
        private ToolStripMenuItem deleteStoryNationalBackTranslationToolStripMenuItem;
        private ToolStripMenuItem deleteEnglishBacktranslationToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem splitIntoLinesToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem viewOldStoriesToolStripMenuItem;
        private ToolStripMenuItem deleteStoryVersesToolStripMenuItem;
        private ToolStripMenuItem deleteTestToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemShowPanorama;
        private ToolStripMenuItem panoramaToolStripMenuItem;
        private ToolStripMenuItem editAddRetellingTestResultsToolStripMenuItem;
        private ToolStripMenuItem editCopySelectionToolStripMenuItem;
        private ToolStripMenuItem projectFromTheInternetToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem editFindToolStripMenuItem;
        private ToolStripMenuItem findNextToolStripMenuItem;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem changeProjectFolderRootToolStripMenuItem;
        private ToolStripMenuItem projectFromASharedNetworkDriveToolStripMenu;
        private ToolStripMenuItem realignStoryVersesToolStripMenuItem;
        private ToolStripMenuItem toTheInternetToolStripMenuItem;
        private ToolStripMenuItem storyCopyWithNewNameToolStripMenuItem;
        private ToolStripMenuItem projectLoginToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        internal ToolStripMenuItem hiddenVersesToolStripMenuItem;
        private ToolStripMenuItem exportToToolboxToolStripMenuItem;
        private ToolStripMenuItem showHideFieldsToolStripMenuItem;
        internal HtmlConsultantNotesControl htmlConsultantNotesControl;
        internal HtmlCoachNotesControl htmlCoachNotesControl;
        private ToolStripMenuItem viewTransliterationsToolStripMenuItem;
        internal ToolStripMenuItem viewTransliterationVernacular;
        internal ToolStripMenuItem viewTransliterationNationalBT;
        private ToolStripMenuItem viewTransliteratorVernacularConfigureToolStripMenuItem;
        private ToolStripMenuItem viewTransliteratorNationalBTConfigureToolStripMenuItem;
        internal LinkLabel linkLabelConsultantNotes;
        internal LinkLabel linkLabelCoachNotes;
        internal LinkLabel linkLabelVerseBT;
        private ContextMenuStrip contextMenuStripVerseList;
        private ToolStripMenuItem resetStoredInformationToolStripMenuItem;
        private ToolStripMenuItem historicalDifferencesToolStripMenuItem;
        private ToolStripMenuItem useSameSettingsForAllStoriesToolStripMenuItem;
        private ToolStripMenuItem printPreviewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        internal ToolStripMenuItem viewStoryTestingQuestionAnswerMenuItem;
        private ToolStripMenuItem concordanceToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemSelectState;
        private ToolStrip toolStripRecordNavigation;
        private ToolStripButton toolStripButtonFirst;
        private ToolStripButton toolStripButtonPrevious;
        private ToolStripButton toolStripButtonNext;
        private ToolStripButton toolStripButtonLast;
        private ToolStripSplitButton buttonsStoryStage;
        private ToolStripStatusLabel statusLabel;
        private StatusStrip statusStrip;
        private ToolStripSeparator toolStripSeparator13;
        internal ToolStripMenuItem viewOnlyOpenConversationsMenu;
        private ToolStripMenuItem stateTransitionHistoryToolStripMenuItem;
        private ToolStripMenuItem changeStateWithoutChecksToolStripMenuItem;
        private ToolStripMenuItem programUpdatesToolStripMenuItem;
        private ToolStripMenuItem automaticallyCheckAtStartupToolStripMenuItem;
        private ToolStripMenuItem checkForProgramUpdatesNowToolStripMenuItem;
        private ToolStripMenuItem saveTimeoutToolStripMenuItem;
        private ToolStripMenuItem enabledToolStripMenuItem;
        private ToolStripMenuItem asSilentlyAsPossibleToolStripMenuItem;
        private ToolStripMenuItem sendReceiveToolStripMenuItem;
        private ToolStripMenuItem viewLnCNotesMenu;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem useAdaptItForBacktranslationToolStripMenuItem;
        private ToolStripMenuItem storyAdaptItVernacularToNationalMenuItem;
        private ToolStripMenuItem storyAdaptItVernacularToEnglishMenuItem;
        private ToolStripMenuItem storyAdaptItNationalToEnglishMenuItem;
        internal ToolStripMenuItem viewFreeTranslationToolStripMenuItem;
        private ToolStripMenuItem copyFreeTranslationMenuItem;
        private ToolStripMenuItem deleteFreeTranslationToolStripMenuItem;
        private ToolStripMenuItem editAddInferenceTestResultsToolStripMenuItem;
    }

#if UsingHtmlDisplayForConNotes
#else
    public class ConNoteFlowLayoutPanel : LineFlowLayoutPanel
    {
        protected List<ConsultNotesDataConverter> lstCNsD = new List<ConsultNotesDataConverter>();

        public override void Clear()
        {
            base.Clear();
            lstCNsD.Clear();
        }

        public void AddCtrl(ConsultNotesControl aCtrl)
        {
            Controls.Add(aCtrl);
            System.Diagnostics.Debug.Assert(!lstCNsD.Contains(aCtrl._theCNsDC));
            lstCNsD.Add(aCtrl._theCNsDC);
        }

        public bool Contains(ConsultNotesDataConverter aCNsD)
        {
            return lstCNsD.Contains(aCNsD);
        }
    }
#endif
}

