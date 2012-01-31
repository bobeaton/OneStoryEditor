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
            this.projectToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectRecentProjectsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectSendReceiveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectCloseProjectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectSaveProjectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.projectBrowseForProjectFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectFromTheInternetMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToTheInternetMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToAThumbdriveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectFromASharedNetworkDriveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.projectSettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectLoginMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExportToToolboxMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.projectPrintMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.projectExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopySelectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyStoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyNationalBtMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyEnglishBtMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyFreeTranslationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editPasteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteStoryLinesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteNationalBtMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteEnglishBtMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteFreeTranslationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteTestToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.editFindMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editFindNextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editReplaceMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.editAddRetellingTestResultsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editAddInferenceTestResultsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editAddGeneralTestQuestionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNonBiblicalStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewShowHideFieldsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewUseSameSettingsForAllStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.viewVernacularLangMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNationalLangMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEnglishBtMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFreeTranslationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAnchorsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewExegeticalHelps = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGeneralTestingsQuestionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStoryTestingQuestionAnswersMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewRetellingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewConsultantNotesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCoachNotesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewBibleMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.viewRefreshMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.viewHistoricalDifferencesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLnCNotesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConcordanceMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStateTransitionHistoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewProjectNotesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOldStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.viewHiddenVersesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOnlyOpenConversationsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.viewTransliterationsToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationVernacular = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorVernacularConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationNationalBT = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationInternationalBt = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliterationFreeTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxStorySelector = new System.Windows.Forms.ToolStripComboBox();
            this.storyToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyStoryInformationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyDeleteStoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyCopyWithNewNameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storySplitIntoLinesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyRealignStoryLinesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyOverrideTasksMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.storyUseAdaptItForBackTranslationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItVernacularToNationalMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItVernacularToEnglishMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAdaptItNationalToEnglishMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.storySynchronizeSharedAdaptItProjectsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaShowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaInsertNewStoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaAddNewStoryAfterMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedProgramUpdatesToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedProgramUpdatesCheckNowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedLocalizationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedOverrideLocalizeStateViewSettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedNewProjectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedChangeStateWithoutChecksMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedSaveTimeoutToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedSaveTimeoutEnabledMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedResetStoredInformationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedChangeProjectFolderRootMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedEmailMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedUseOldStyleStoryBtPaneMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainerLeftRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerUpDown = new OneStoryProjectEditor.MinimizableSplitterContainer();
            this.linkLabelTasks = new System.Windows.Forms.LinkLabel();
            this.linkLabelVerseBT = new System.Windows.Forms.LinkLabel();
            this.contextMenuStripVerseList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowLayoutPanelVerses = new OneStoryProjectEditor.VerseBtLineFlowLayoutPanel();
            this.textBoxStoryVerse = new System.Windows.Forms.TextBox();
            this.htmlStoryBtControl = new OneStoryProjectEditor.HtmlStoryBtControl();
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
            this.toolStripButtonShowPanoramaStories = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFirst = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLast = new System.Windows.Forms.ToolStripButton();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
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
            this.projectToolStripMenu,
            this.editToolStripMenu,
            this.viewToolStripMenuItem,
            this.comboBoxStorySelector,
            this.storyToolStripMenu,
            this.panoramaToolStripMenu,
            this.advancedToolStripMenu,
            this.aboutToolStripMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(881, 31);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenu
            // 
            this.projectToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectRecentProjectsMenu,
            this.projectSendReceiveMenu,
            this.projectCloseProjectMenu,
            this.projectSaveProjectMenu,
            this.toolStripSeparator1,
            this.projectBrowseForProjectFileMenu,
            this.projectFromTheInternetMenu,
            this.projectToTheInternetMenu,
            this.projectToAThumbdriveMenu,
            this.projectFromASharedNetworkDriveMenu,
            this.toolStripSeparator4,
            this.projectSettingsMenu,
            this.projectLoginMenu,
            this.projectExportToToolboxMenu,
            this.toolStripSeparator2,
            this.projectPrintMenu,
            this.toolStripSeparator12,
            this.projectExitMenu});
            this.projectToolStripMenu.Name = "projectToolStripMenu";
            this.projectToolStripMenu.Size = new System.Drawing.Size(56, 27);
            this.projectToolStripMenu.Text = "&Project";
            this.projectToolStripMenu.DropDownOpening += new System.EventHandler(this.projectToolStripMenuItem_DropDownOpening);
            // 
            // projectRecentProjectsMenu
            // 
            this.projectRecentProjectsMenu.Name = "projectRecentProjectsMenu";
            this.projectRecentProjectsMenu.Size = new System.Drawing.Size(286, 22);
            this.projectRecentProjectsMenu.Text = "&Recent projects";
            this.projectRecentProjectsMenu.ToolTipText = "This shows the projects that have at one time or other been opened on this machin" +
    "e";
            // 
            // projectSendReceiveMenu
            // 
            this.projectSendReceiveMenu.Name = "projectSendReceiveMenu";
            this.projectSendReceiveMenu.Size = new System.Drawing.Size(286, 22);
            this.projectSendReceiveMenu.Text = "Sen&d/Receive...";
            this.projectSendReceiveMenu.ToolTipText = "Click to synchronize this project with the Internet (or thumbdrive) repository";
            this.projectSendReceiveMenu.Click += new System.EventHandler(this.sendReceiveToolStripMenuItem_Click);
            // 
            // projectCloseProjectMenu
            // 
            this.projectCloseProjectMenu.Name = "projectCloseProjectMenu";
            this.projectCloseProjectMenu.Size = new System.Drawing.Size(286, 22);
            this.projectCloseProjectMenu.Text = "&Close project";
            this.projectCloseProjectMenu.ToolTipText = "Click to save the OneStory project";
            this.projectCloseProjectMenu.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // projectSaveProjectMenu
            // 
            this.projectSaveProjectMenu.Name = "projectSaveProjectMenu";
            this.projectSaveProjectMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.projectSaveProjectMenu.Size = new System.Drawing.Size(286, 22);
            this.projectSaveProjectMenu.Text = "&Save project";
            this.projectSaveProjectMenu.ToolTipText = "Click to save the OneStory project";
            this.projectSaveProjectMenu.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(283, 6);
            // 
            // projectBrowseForProjectFileMenu
            // 
            this.projectBrowseForProjectFileMenu.Name = "projectBrowseForProjectFileMenu";
            this.projectBrowseForProjectFileMenu.Size = new System.Drawing.Size(286, 22);
            this.projectBrowseForProjectFileMenu.Text = "&Browse for project file";
            this.projectBrowseForProjectFileMenu.ToolTipText = "Click this option to open an existing OneStory project";
            this.projectBrowseForProjectFileMenu.Click += new System.EventHandler(this.browseForProjectToolStripMenuItem_Click);
            // 
            // projectFromTheInternetMenu
            // 
            this.projectFromTheInternetMenu.Name = "projectFromTheInternetMenu";
            this.projectFromTheInternetMenu.Size = new System.Drawing.Size(286, 22);
            this.projectFromTheInternetMenu.Text = "&From the Internet...";
            this.projectFromTheInternetMenu.ToolTipText = "Click here to enter an Internet address to get a project from (e.g. if a team mat" +
    "e has already uploaded it to the internet repository)";
            this.projectFromTheInternetMenu.Click += new System.EventHandler(this.projectFromTheInternetToolStripMenuItem_Click);
            // 
            // projectToTheInternetMenu
            // 
            this.projectToTheInternetMenu.Name = "projectToTheInternetMenu";
            this.projectToTheInternetMenu.Size = new System.Drawing.Size(286, 22);
            this.projectToTheInternetMenu.Text = "&To the Internet...";
            this.projectToTheInternetMenu.ToolTipText = "Click here to enter the Internet address of the repository to send this project t" +
    "o (e.g. if you have created a new project and want to \"push\" it to an existing i" +
    "nternet repository)";
            this.projectToTheInternetMenu.Click += new System.EventHandler(this.toTheInternetToolStripMenuItem_Click);
            // 
            // projectToAThumbdriveMenu
            // 
            this.projectToAThumbdriveMenu.Name = "projectToAThumbdriveMenu";
            this.projectToAThumbdriveMenu.Size = new System.Drawing.Size(286, 22);
            this.projectToAThumbdriveMenu.Text = "Transfer via thum&bdrive...";
            this.projectToAThumbdriveMenu.ToolTipText = resources.GetString("projectToAThumbdriveMenu.ToolTipText");
            this.projectToAThumbdriveMenu.Click += new System.EventHandler(this.toAThumbdriveToolStripMenuItem_Click);
            // 
            // projectFromASharedNetworkDriveMenu
            // 
            this.projectFromASharedNetworkDriveMenu.Name = "projectFromASharedNetworkDriveMenu";
            this.projectFromASharedNetworkDriveMenu.Size = new System.Drawing.Size(286, 22);
            this.projectFromASharedNetworkDriveMenu.Text = "&Associate with a shared network folder...";
            this.projectFromASharedNetworkDriveMenu.ToolTipText = "Click here to associate this project with a repository on a network drive (e.g. f" +
    "or working together at a workshop where Internet connectivity is difficult).";
            this.projectFromASharedNetworkDriveMenu.Click += new System.EventHandler(this.projectFromASharedNetworkDriveToolStripMenu_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(283, 6);
            // 
            // projectSettingsMenu
            // 
            this.projectSettingsMenu.Name = "projectSettingsMenu";
            this.projectSettingsMenu.Size = new System.Drawing.Size(286, 22);
            this.projectSettingsMenu.Text = "Se&ttings...";
            this.projectSettingsMenu.ToolTipText = "Click here to open the Project Settings dialog in order to edit the language prop" +
    "erties (fonts, keyboards, etc) or other project configuration information";
            this.projectSettingsMenu.Click += new System.EventHandler(this.projectSettingsToolStripMenuItem_Click);
            // 
            // projectLoginMenu
            // 
            this.projectLoginMenu.Name = "projectLoginMenu";
            this.projectLoginMenu.Size = new System.Drawing.Size(286, 22);
            this.projectLoginMenu.Text = "&Login...";
            this.projectLoginMenu.ToolTipText = "Click to login as a specific member name";
            this.projectLoginMenu.Click += new System.EventHandler(this.projectLoginToolStripMenuItem_Click);
            // 
            // projectExportToToolboxMenu
            // 
            this.projectExportToToolboxMenu.Name = "projectExportToToolboxMenu";
            this.projectExportToToolboxMenu.Size = new System.Drawing.Size(286, 22);
            this.projectExportToToolboxMenu.Text = "E&xport to Toolbox";
            this.projectExportToToolboxMenu.ToolTipText = "Click here to export the OneStory Editor project to a Toolbox readable format (in" +
    " the \'Toolbox\' sub-folder of the OSE project folder)";
            this.projectExportToToolboxMenu.Click += new System.EventHandler(this.exportToToolboxToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(283, 6);
            // 
            // projectPrintMenu
            // 
            this.projectPrintMenu.Name = "projectPrintMenu";
            this.projectPrintMenu.Size = new System.Drawing.Size(286, 22);
            this.projectPrintMenu.Text = "&Print...";
            this.projectPrintMenu.ToolTipText = "Click here to configure a print preview of the stories that can then be printed o" +
    "r saved in HTML format";
            this.projectPrintMenu.Click += new System.EventHandler(this.printPreviewToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(283, 6);
            // 
            // projectExitMenu
            // 
            this.projectExitMenu.Name = "projectExitMenu";
            this.projectExitMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.projectExitMenu.Size = new System.Drawing.Size(286, 22);
            this.projectExitMenu.Text = "&Exit";
            this.projectExitMenu.ToolTipText = "Click to exit the program and synchronize with the internet repositories";
            this.projectExitMenu.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenu
            // 
            this.editToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCopyToolStripMenu,
            this.editPasteMenu,
            this.editDeleteToolStripMenu,
            this.toolStripSeparator9,
            this.editFindMenu,
            this.editFindNextMenu,
            this.editReplaceMenu,
            this.toolStripSeparator10,
            this.editAddRetellingTestResultsMenu,
            this.editAddInferenceTestResultsMenu,
            this.editAddGeneralTestQuestionMenu});
            this.editToolStripMenu.Name = "editToolStripMenu";
            this.editToolStripMenu.Size = new System.Drawing.Size(39, 27);
            this.editToolStripMenu.Text = "&Edit";
            this.editToolStripMenu.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // editCopyToolStripMenu
            // 
            this.editCopyToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCopySelectionMenu,
            this.editCopyStoryMenu,
            this.editCopyNationalBtMenu,
            this.editCopyEnglishBtMenu,
            this.editCopyFreeTranslationMenu});
            this.editCopyToolStripMenu.Name = "editCopyToolStripMenu";
            this.editCopyToolStripMenu.Size = new System.Drawing.Size(269, 22);
            this.editCopyToolStripMenu.Text = "&Copy";
            // 
            // editCopySelectionMenu
            // 
            this.editCopySelectionMenu.Name = "editCopySelectionMenu";
            this.editCopySelectionMenu.Size = new System.Drawing.Size(386, 22);
            this.editCopySelectionMenu.Text = "Sele&ction";
            this.editCopySelectionMenu.ToolTipText = "Copy the selected text from the active text box to the clipboard";
            this.editCopySelectionMenu.Click += new System.EventHandler(this.editCopySelectionToolStripMenuItem_Click);
            // 
            // editCopyStoryMenu
            // 
            this.editCopyStoryMenu.Name = "editCopyStoryMenu";
            this.editCopyStoryMenu.Size = new System.Drawing.Size(386, 22);
            this.editCopyStoryMenu.Text = "&Story";
            this.editCopyStoryMenu.ToolTipText = "Copy all of the lines of text in the story language into one big paragraph of tex" +
    "t";
            this.editCopyStoryMenu.Click += new System.EventHandler(this.copyStoryToolStripMenuItem_Click);
            // 
            // editCopyNationalBtMenu
            // 
            this.editCopyNationalBtMenu.Name = "editCopyNationalBtMenu";
            this.editCopyNationalBtMenu.Size = new System.Drawing.Size(386, 22);
            this.editCopyNationalBtMenu.Text = "&National back-translation";
            this.editCopyNationalBtMenu.ToolTipText = "Copy all of the lines of text in the National back-translation language into one " +
    "big paragraph of text";
            this.editCopyNationalBtMenu.Click += new System.EventHandler(this.copyNationalBackTranslationToolStripMenuItem_Click);
            // 
            // editCopyEnglishBtMenu
            // 
            this.editCopyEnglishBtMenu.Name = "editCopyEnglishBtMenu";
            this.editCopyEnglishBtMenu.Size = new System.Drawing.Size(386, 22);
            this.editCopyEnglishBtMenu.Text = "&English back-translation of the whole story to the clipboard";
            this.editCopyEnglishBtMenu.ToolTipText = "Copy all of the lines of text in the English back-translation into one big paragr" +
    "aph of text";
            this.editCopyEnglishBtMenu.Click += new System.EventHandler(this.copyEnglishBackTranslationToolStripMenuItem_Click);
            // 
            // editCopyFreeTranslationMenu
            // 
            this.editCopyFreeTranslationMenu.Name = "editCopyFreeTranslationMenu";
            this.editCopyFreeTranslationMenu.Size = new System.Drawing.Size(386, 22);
            this.editCopyFreeTranslationMenu.Text = "&Free translation of the whole story to the clipboard";
            this.editCopyFreeTranslationMenu.ToolTipText = "Copy all of the lines of text in the Free translation into one big paragraph of t" +
    "ext";
            this.editCopyFreeTranslationMenu.Click += new System.EventHandler(this.copyFreeTranslationMenuItem_Click);
            // 
            // editPasteMenu
            // 
            this.editPasteMenu.Name = "editPasteMenu";
            this.editPasteMenu.Size = new System.Drawing.Size(269, 22);
            this.editPasteMenu.Text = "&Paste";
            this.editPasteMenu.ToolTipText = "Paste the contents of the clipboard into the currently selected text box";
            this.editPasteMenu.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // editDeleteToolStripMenu
            // 
            this.editDeleteToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDeleteStoryLinesMenu,
            this.editDeleteNationalBtMenu,
            this.editDeleteEnglishBtMenu,
            this.editDeleteFreeTranslationMenu,
            this.editDeleteTestToolStripMenu});
            this.editDeleteToolStripMenu.Name = "editDeleteToolStripMenu";
            this.editDeleteToolStripMenu.Size = new System.Drawing.Size(269, 22);
            this.editDeleteToolStripMenu.Text = "&Delete";
            // 
            // editDeleteStoryLinesMenu
            // 
            this.editDeleteStoryLinesMenu.Name = "editDeleteStoryLinesMenu";
            this.editDeleteStoryLinesMenu.Size = new System.Drawing.Size(310, 22);
            this.editDeleteStoryLinesMenu.Text = "&Story (all lines)";
            this.editDeleteStoryLinesMenu.ToolTipText = "Delete the contents of all of the text boxes of the story in the story language (" +
    "the lines will remain, but just be emptied)";
            this.editDeleteStoryLinesMenu.Click += new System.EventHandler(this.deleteStoryVersesToolStripMenuItem_Click);
            // 
            // editDeleteNationalBtMenu
            // 
            this.editDeleteNationalBtMenu.Name = "editDeleteNationalBtMenu";
            this.editDeleteNationalBtMenu.Size = new System.Drawing.Size(310, 22);
            this.editDeleteNationalBtMenu.Text = "&National language back-translation (all lines)";
            this.editDeleteNationalBtMenu.ToolTipText = "Delete the contents of all of the text boxes of the national back-translation of " +
    "the story (the lines will remain, but just be emptied)";
            this.editDeleteNationalBtMenu.Click += new System.EventHandler(this.deleteStoryNationalBackTranslationToolStripMenuItem_Click);
            // 
            // editDeleteEnglishBtMenu
            // 
            this.editDeleteEnglishBtMenu.Name = "editDeleteEnglishBtMenu";
            this.editDeleteEnglishBtMenu.Size = new System.Drawing.Size(310, 22);
            this.editDeleteEnglishBtMenu.Text = "&English back-translation (all lines)";
            this.editDeleteEnglishBtMenu.ToolTipText = "Delete the contents of all of the text boxes of the English back-translation of t" +
    "he story (the lines will remain, but just be emptied)";
            this.editDeleteEnglishBtMenu.Click += new System.EventHandler(this.deleteEnglishBacktranslationToolStripMenuItem_Click);
            // 
            // editDeleteFreeTranslationMenu
            // 
            this.editDeleteFreeTranslationMenu.Name = "editDeleteFreeTranslationMenu";
            this.editDeleteFreeTranslationMenu.Size = new System.Drawing.Size(310, 22);
            this.editDeleteFreeTranslationMenu.Text = "&Free translation (all lines)";
            this.editDeleteFreeTranslationMenu.ToolTipText = "Delete the contents of all of the text boxes of the Free translation of the story" +
    " (the lines will remain, but just be emptied)";
            this.editDeleteFreeTranslationMenu.Click += new System.EventHandler(this.deleteFreeTranslationToolStripMenuItem_Click);
            // 
            // editDeleteTestToolStripMenu
            // 
            this.editDeleteTestToolStripMenu.Name = "editDeleteTestToolStripMenu";
            this.editDeleteTestToolStripMenu.Size = new System.Drawing.Size(310, 22);
            this.editDeleteTestToolStripMenu.Text = "&Test";
            this.editDeleteTestToolStripMenu.ToolTipText = "Delete the answers to the testing questions and the retellings associated with a " +
    "particular testing helper (UNS). The text boxes will be deleted completely";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(266, 6);
            // 
            // editFindMenu
            // 
            this.editFindMenu.Enabled = false;
            this.editFindMenu.Name = "editFindMenu";
            this.editFindMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.editFindMenu.Size = new System.Drawing.Size(269, 22);
            this.editFindMenu.Text = "&Find";
            this.editFindMenu.Click += new System.EventHandler(this.editFindToolStripMenuItem_Click);
            // 
            // editFindNextMenu
            // 
            this.editFindNextMenu.Enabled = false;
            this.editFindNextMenu.Name = "editFindNextMenu";
            this.editFindNextMenu.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.editFindNextMenu.Size = new System.Drawing.Size(269, 22);
            this.editFindNextMenu.Text = "Find &Next";
            this.editFindNextMenu.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // editReplaceMenu
            // 
            this.editReplaceMenu.Enabled = false;
            this.editReplaceMenu.Name = "editReplaceMenu";
            this.editReplaceMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.editReplaceMenu.Size = new System.Drawing.Size(269, 22);
            this.editReplaceMenu.Text = "&Replace";
            this.editReplaceMenu.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(266, 6);
            // 
            // editAddRetellingTestResultsMenu
            // 
            this.editAddRetellingTestResultsMenu.Name = "editAddRetellingTestResultsMenu";
            this.editAddRetellingTestResultsMenu.Size = new System.Drawing.Size(269, 22);
            this.editAddRetellingTestResultsMenu.Text = "&Add retelling test boxes";
            this.editAddRetellingTestResultsMenu.ToolTipText = "Click here to add boxes for the retellings of the story";
            this.editAddRetellingTestResultsMenu.Click += new System.EventHandler(this.editAddTestResultsToolStripMenuItem_Click);
            // 
            // editAddInferenceTestResultsMenu
            // 
            this.editAddInferenceTestResultsMenu.Name = "editAddInferenceTestResultsMenu";
            this.editAddInferenceTestResultsMenu.Size = new System.Drawing.Size(269, 22);
            this.editAddInferenceTestResultsMenu.Text = "Add &story test question answer boxes";
            this.editAddInferenceTestResultsMenu.ToolTipText = "Click here to add boxes for the answers to the testing questions";
            this.editAddInferenceTestResultsMenu.Click += new System.EventHandler(this.editAddInferenceTestResultsToolStripMenuItem_Click);
            // 
            // editAddGeneralTestQuestionMenu
            // 
            this.editAddGeneralTestQuestionMenu.Name = "editAddGeneralTestQuestionMenu";
            this.editAddGeneralTestQuestionMenu.Size = new System.Drawing.Size(269, 22);
            this.editAddGeneralTestQuestionMenu.Text = "Add &general test question";
            this.editAddGeneralTestQuestionMenu.Click += new System.EventHandler(this.addgeneralTestQuestionToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewNonBiblicalStoriesMenu,
            this.toolStripSeparator5,
            this.viewShowHideFieldsMenu,
            this.viewUseSameSettingsForAllStoriesMenu,
            this.toolStripSeparator16,
            this.viewVernacularLangMenu,
            this.viewNationalLangMenu,
            this.viewEnglishBtMenu,
            this.viewFreeTranslationMenu,
            this.viewAnchorsMenu,
            this.viewExegeticalHelps,
            this.viewGeneralTestingsQuestionMenu,
            this.viewStoryTestingQuestionsMenu,
            this.viewStoryTestingQuestionAnswersMenu,
            this.viewRetellingsMenu,
            this.toolStripSeparator6,
            this.viewConsultantNotesMenu,
            this.viewCoachNotesMenu,
            this.toolStripSeparator3,
            this.viewBibleMenu,
            this.toolStripSeparator7,
            this.viewRefreshMenu,
            this.toolStripSeparator8,
            this.viewHistoricalDifferencesMenu,
            this.viewLnCNotesMenu,
            this.viewConcordanceMenu,
            this.viewStateTransitionHistoryMenu,
            this.viewProjectNotesMenu,
            this.viewOldStoriesMenu,
            this.toolStripSeparator11,
            this.viewHiddenVersesMenu,
            this.viewOnlyOpenConversationsMenu,
            this.toolStripSeparator13,
            this.viewTransliterationsToolStripMenu});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 27);
            this.viewToolStripMenuItem.Text = "&View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // viewNonBiblicalStoriesMenu
            // 
            this.viewNonBiblicalStoriesMenu.CheckOnClick = true;
            this.viewNonBiblicalStoriesMenu.Name = "viewNonBiblicalStoriesMenu";
            this.viewNonBiblicalStoriesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewNonBiblicalStoriesMenu.Text = "&Non-biblical Stories...";
            this.viewNonBiblicalStoriesMenu.ToolTipText = "Check this menu to edit the set of non-biblical stories (uncheck for biblical sto" +
    "ries)";
            this.viewNonBiblicalStoriesMenu.CheckedChanged += new System.EventHandler(this.ViewNonBiblicalStoriesMenuCheckedChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(281, 6);
            // 
            // viewShowHideFieldsMenu
            // 
            this.viewShowHideFieldsMenu.Name = "viewShowHideFieldsMenu";
            this.viewShowHideFieldsMenu.Size = new System.Drawing.Size(284, 22);
            this.viewShowHideFieldsMenu.Text = "&Show/Hide multiple fields at once...";
            this.viewShowHideFieldsMenu.Click += new System.EventHandler(this.showHideFieldsToolStripMenuItem_Click);
            // 
            // viewUseSameSettingsForAllStoriesMenu
            // 
            this.viewUseSameSettingsForAllStoriesMenu.CheckOnClick = true;
            this.viewUseSameSettingsForAllStoriesMenu.Name = "viewUseSameSettingsForAllStoriesMenu";
            this.viewUseSameSettingsForAllStoriesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewUseSameSettingsForAllStoriesMenu.Text = "&Use same settings for all stories";
            this.viewUseSameSettingsForAllStoriesMenu.Click += new System.EventHandler(this.useSameSettingsForAllStoriesToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(281, 6);
            // 
            // viewVernacularLangMenu
            // 
            this.viewVernacularLangMenu.Checked = true;
            this.viewVernacularLangMenu.CheckOnClick = true;
            this.viewVernacularLangMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewVernacularLangMenu.Name = "viewVernacularLangMenu";
            this.viewVernacularLangMenu.Size = new System.Drawing.Size(284, 22);
            this.viewVernacularLangMenu.Text = "Story &Language field";
            this.viewVernacularLangMenu.ToolTipText = "Show the text boxes for the story lines in the story language";
            this.viewVernacularLangMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewNationalLangMenu
            // 
            this.viewNationalLangMenu.Checked = true;
            this.viewNationalLangMenu.CheckOnClick = true;
            this.viewNationalLangMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewNationalLangMenu.Name = "viewNationalLangMenu";
            this.viewNationalLangMenu.Size = new System.Drawing.Size(284, 22);
            this.viewNationalLangMenu.Text = "National language &back translation field";
            this.viewNationalLangMenu.ToolTipText = "Show the text boxes for the national language back-translation of the story lines" +
    "";
            this.viewNationalLangMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewEnglishBtMenu
            // 
            this.viewEnglishBtMenu.Checked = true;
            this.viewEnglishBtMenu.CheckOnClick = true;
            this.viewEnglishBtMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewEnglishBtMenu.Name = "viewEnglishBtMenu";
            this.viewEnglishBtMenu.Size = new System.Drawing.Size(284, 22);
            this.viewEnglishBtMenu.Text = "&English back translation fields";
            this.viewEnglishBtMenu.ToolTipText = "Show the text boxes for the English language back-translation of the story lines";
            this.viewEnglishBtMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewFreeTranslationMenu
            // 
            this.viewFreeTranslationMenu.Checked = true;
            this.viewFreeTranslationMenu.CheckOnClick = true;
            this.viewFreeTranslationMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewFreeTranslationMenu.Name = "viewFreeTranslationMenu";
            this.viewFreeTranslationMenu.Size = new System.Drawing.Size(284, 22);
            this.viewFreeTranslationMenu.Text = "&Free Translation";
            this.viewFreeTranslationMenu.ToolTipText = "Show the text boxes for the Free Translation of the story lines";
            this.viewFreeTranslationMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewAnchorsMenu
            // 
            this.viewAnchorsMenu.Checked = true;
            this.viewAnchorsMenu.CheckOnClick = true;
            this.viewAnchorsMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewAnchorsMenu.Name = "viewAnchorsMenu";
            this.viewAnchorsMenu.Size = new System.Drawing.Size(284, 22);
            this.viewAnchorsMenu.Text = "&Anchors";
            this.viewAnchorsMenu.ToolTipText = "Show the Anchor toolbar";
            this.viewAnchorsMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewExegeticalHelps
            // 
            this.viewExegeticalHelps.Checked = true;
            this.viewExegeticalHelps.CheckOnClick = true;
            this.viewExegeticalHelps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewExegeticalHelps.Name = "viewExegeticalHelps";
            this.viewExegeticalHelps.Size = new System.Drawing.Size(284, 22);
            this.viewExegeticalHelps.Text = "&Exegetical/Cultural notes";
            this.viewExegeticalHelps.ToolTipText = "Show the Exegetical/Cultural notes (\'cn\') fields";
            this.viewExegeticalHelps.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewGeneralTestingsQuestionMenu
            // 
            this.viewGeneralTestingsQuestionMenu.CheckOnClick = true;
            this.viewGeneralTestingsQuestionMenu.Name = "viewGeneralTestingsQuestionMenu";
            this.viewGeneralTestingsQuestionMenu.Size = new System.Drawing.Size(284, 22);
            this.viewGeneralTestingsQuestionMenu.Text = "&General testing questions";
            this.viewGeneralTestingsQuestionMenu.ToolTipText = "Show the text boxes for the general testing questions";
            this.viewGeneralTestingsQuestionMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionsMenu
            // 
            this.viewStoryTestingQuestionsMenu.Checked = true;
            this.viewStoryTestingQuestionsMenu.CheckOnClick = true;
            this.viewStoryTestingQuestionsMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionsMenu.Name = "viewStoryTestingQuestionsMenu";
            this.viewStoryTestingQuestionsMenu.Size = new System.Drawing.Size(284, 22);
            this.viewStoryTestingQuestionsMenu.Text = "Story &testing questions";
            this.viewStoryTestingQuestionsMenu.ToolTipText = "Show the text boxes for the story testing questions";
            this.viewStoryTestingQuestionsMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionAnswersMenu
            // 
            this.viewStoryTestingQuestionAnswersMenu.Checked = true;
            this.viewStoryTestingQuestionAnswersMenu.CheckOnClick = true;
            this.viewStoryTestingQuestionAnswersMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewStoryTestingQuestionAnswersMenu.Name = "viewStoryTestingQuestionAnswersMenu";
            this.viewStoryTestingQuestionAnswersMenu.Size = new System.Drawing.Size(284, 22);
            this.viewStoryTestingQuestionAnswersMenu.Text = "Ans&wers";
            this.viewStoryTestingQuestionAnswersMenu.ToolTipText = "Show the text boxes for the UNS\'s answers to testing questions";
            this.viewStoryTestingQuestionAnswersMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewRetellingsMenu
            // 
            this.viewRetellingsMenu.Checked = true;
            this.viewRetellingsMenu.CheckOnClick = true;
            this.viewRetellingsMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewRetellingsMenu.Name = "viewRetellingsMenu";
            this.viewRetellingsMenu.Size = new System.Drawing.Size(284, 22);
            this.viewRetellingsMenu.Text = "&Retellings";
            this.viewRetellingsMenu.ToolTipText = "Show the text boxes for the UNS retelling responses";
            this.viewRetellingsMenu.CheckedChanged += new System.EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(281, 6);
            // 
            // viewConsultantNotesMenu
            // 
            this.viewConsultantNotesMenu.Checked = true;
            this.viewConsultantNotesMenu.CheckOnClick = true;
            this.viewConsultantNotesMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewConsultantNotesMenu.Name = "viewConsultantNotesMenu";
            this.viewConsultantNotesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewConsultantNotesMenu.Text = "&Consultant notes";
            this.viewConsultantNotesMenu.ToolTipText = "Show the Consultant Notes pane";
            this.viewConsultantNotesMenu.CheckedChanged += new System.EventHandler(this.viewConsultantNoteFieldMenuItem_CheckedChanged);
            // 
            // viewCoachNotesMenu
            // 
            this.viewCoachNotesMenu.Checked = true;
            this.viewCoachNotesMenu.CheckOnClick = true;
            this.viewCoachNotesMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewCoachNotesMenu.Name = "viewCoachNotesMenu";
            this.viewCoachNotesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewCoachNotesMenu.Text = "Coach &notes";
            this.viewCoachNotesMenu.ToolTipText = "Show the Coach Notes pane";
            this.viewCoachNotesMenu.CheckedChanged += new System.EventHandler(this.viewCoachNotesFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(281, 6);
            // 
            // viewBibleMenu
            // 
            this.viewBibleMenu.Checked = true;
            this.viewBibleMenu.CheckOnClick = true;
            this.viewBibleMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewBibleMenu.Name = "viewBibleMenu";
            this.viewBibleMenu.Size = new System.Drawing.Size(284, 22);
            this.viewBibleMenu.Text = "&Bible viewer";
            this.viewBibleMenu.ToolTipText = "Show the Bible Viewer pane";
            this.viewBibleMenu.CheckedChanged += new System.EventHandler(this.viewNetBibleMenuItem_CheckedChanged);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(281, 6);
            // 
            // viewRefreshMenu
            // 
            this.viewRefreshMenu.Enabled = false;
            this.viewRefreshMenu.Name = "viewRefreshMenu";
            this.viewRefreshMenu.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.viewRefreshMenu.Size = new System.Drawing.Size(284, 22);
            this.viewRefreshMenu.Text = "Re&fresh";
            this.viewRefreshMenu.ToolTipText = "Refresh the screen (if it doesn\'t look like it updated something properly)";
            this.viewRefreshMenu.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(281, 6);
            // 
            // viewHistoricalDifferencesMenu
            // 
            this.viewHistoricalDifferencesMenu.Name = "viewHistoricalDifferencesMenu";
            this.viewHistoricalDifferencesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewHistoricalDifferencesMenu.Text = "Historical di&fferences...";
            this.viewHistoricalDifferencesMenu.ToolTipText = "Click to launch the Revision History dialog to compare different, saved versions " +
    "of this story";
            this.viewHistoricalDifferencesMenu.Click += new System.EventHandler(this.historicalDifferencesToolStripMenuItem_Click);
            // 
            // viewLnCNotesMenu
            // 
            this.viewLnCNotesMenu.Name = "viewLnCNotesMenu";
            this.viewLnCNotesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewLnCNotesMenu.Text = "L && C Notes...";
            this.viewLnCNotesMenu.Click += new System.EventHandler(this.viewLnCNotesMenu_Click);
            // 
            // viewConcordanceMenu
            // 
            this.viewConcordanceMenu.Name = "viewConcordanceMenu";
            this.viewConcordanceMenu.Size = new System.Drawing.Size(284, 22);
            this.viewConcordanceMenu.Text = "Concor&dance...";
            this.viewConcordanceMenu.ToolTipText = "Click to launch the Concordance dialog to search for the occurrences of words in " +
    "the story";
            this.viewConcordanceMenu.Click += new System.EventHandler(this.concordanceToolStripMenuItem_Click);
            // 
            // viewStateTransitionHistoryMenu
            // 
            this.viewStateTransitionHistoryMenu.Name = "viewStateTransitionHistoryMenu";
            this.viewStateTransitionHistoryMenu.Size = new System.Drawing.Size(284, 22);
            this.viewStateTransitionHistoryMenu.Text = "&Turn Transition History...";
            this.viewStateTransitionHistoryMenu.ToolTipText = "Click here to view information about when the story was in different turns and wh" +
    "ose turn it was";
            this.viewStateTransitionHistoryMenu.Click += new System.EventHandler(this.stateTransitionHistoryToolStripMenuItem_Click);
            // 
            // viewProjectNotesMenu
            // 
            this.viewProjectNotesMenu.Name = "viewProjectNotesMenu";
            this.viewProjectNotesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewProjectNotesMenu.Text = "&Project Notes...";
            this.viewProjectNotesMenu.Click += new System.EventHandler(this.projectNotesToolStripMenuItem_Click);
            // 
            // viewOldStoriesMenu
            // 
            this.viewOldStoriesMenu.Name = "viewOldStoriesMenu";
            this.viewOldStoriesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewOldStoriesMenu.Text = "&Old Stories...";
            this.viewOldStoriesMenu.ToolTipText = "View older (obsolete) versions of the stories (that were earlier stored in the \'O" +
    "ld Stories\' list from the \'Panorama View\' window--see \'Panorama\' menu, \'Show\' co" +
    "mmand)";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(281, 6);
            // 
            // viewHiddenVersesMenu
            // 
            this.viewHiddenVersesMenu.CheckOnClick = true;
            this.viewHiddenVersesMenu.Name = "viewHiddenVersesMenu";
            this.viewHiddenVersesMenu.Size = new System.Drawing.Size(284, 22);
            this.viewHiddenVersesMenu.Text = "H&idden lines";
            this.viewHiddenVersesMenu.ToolTipText = "Check this menu to show hidden lines and hidden consultant note comments";
            this.viewHiddenVersesMenu.CheckStateChanged += new System.EventHandler(this.hiddenVersesToolStripMenuItem_CheckStateChanged);
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
            // viewTransliterationsToolStripMenu
            // 
            this.viewTransliterationsToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliterationVernacular,
            this.viewTransliterationNationalBT,
            this.viewTransliterationInternationalBt,
            this.viewTransliterationFreeTranslation});
            this.viewTransliterationsToolStripMenu.Name = "viewTransliterationsToolStripMenu";
            this.viewTransliterationsToolStripMenu.Size = new System.Drawing.Size(284, 22);
            this.viewTransliterationsToolStripMenu.Text = "&Transliterations";
            this.viewTransliterationsToolStripMenu.DropDownOpening += new System.EventHandler(this.viewTransliterationsToolStripMenuItem_DropDownOpening);
            // 
            // viewTransliterationVernacular
            // 
            this.viewTransliterationVernacular.CheckOnClick = true;
            this.viewTransliterationVernacular.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorVernacularConfigureToolStripMenuItem});
            this.viewTransliterationVernacular.Name = "viewTransliterationVernacular";
            this.viewTransliterationVernacular.Size = new System.Drawing.Size(158, 22);
            this.viewTransliterationVernacular.Text = "Story Language";
            this.viewTransliterationVernacular.ToolTipText = "Check this menu to turn on a transliterator for the story language boxes";
            this.viewTransliterationVernacular.Click += new System.EventHandler(this.viewTransliterationVernacular_Click);
            // 
            // viewTransliteratorVernacularConfigureToolStripMenuItem
            // 
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Name = "viewTransliteratorVernacularConfigureToolStripMenuItem";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorVernacularConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationNationalBT
            // 
            this.viewTransliterationNationalBT.CheckOnClick = true;
            this.viewTransliterationNationalBT.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem});
            this.viewTransliterationNationalBT.Name = "viewTransliterationNationalBT";
            this.viewTransliterationNationalBT.Size = new System.Drawing.Size(158, 22);
            this.viewTransliterationNationalBT.Text = "National BT";
            this.viewTransliterationNationalBT.ToolTipText = "Check this menu to turn on a transliterator for the national language BT boxes";
            this.viewTransliterationNationalBT.Click += new System.EventHandler(this.viewTransliterationNationalBT_Click);
            // 
            // viewTransliteratorNationalBTConfigureToolStripMenuItem
            // 
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Name = "viewTransliteratorNationalBTConfigureToolStripMenuItem";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorNationalBTConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationInternationalBt
            // 
            this.viewTransliterationInternationalBt.CheckOnClick = true;
            this.viewTransliterationInternationalBt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem});
            this.viewTransliterationInternationalBt.Name = "viewTransliterationInternationalBt";
            this.viewTransliterationInternationalBt.Size = new System.Drawing.Size(158, 22);
            this.viewTransliterationInternationalBt.Text = "International BT";
            this.viewTransliterationInternationalBt.ToolTipText = "Check this menu to turn on a transliterator for the International/English languag" +
    "e boxes";
            this.viewTransliterationInternationalBt.Click += new System.EventHandler(this.viewTransliterationInternationalBt_Click);
            // 
            // viewTransliteratorInternationalBtConfigureToolStripMenuItem
            // 
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Name = "viewTransliteratorInternationalBtConfigureToolStripMenuItem";
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorInternationalBtConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationFreeTranslation
            // 
            this.viewTransliterationFreeTranslation.CheckOnClick = true;
            this.viewTransliterationFreeTranslation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem});
            this.viewTransliterationFreeTranslation.Name = "viewTransliterationFreeTranslation";
            this.viewTransliterationFreeTranslation.Size = new System.Drawing.Size(158, 22);
            this.viewTransliterationFreeTranslation.Text = "Free Translation";
            this.viewTransliterationFreeTranslation.ToolTipText = "Check this menu to turn on a transliterator for the Free Translation language box" +
    "es";
            this.viewTransliterationFreeTranslation.Click += new System.EventHandler(this.viewTransliterationFreeTranslation_Click);
            // 
            // viewTransliteratorFreeTranslationConfigureToolStripMenuItem
            // 
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Name = "viewTransliteratorFreeTranslationConfigureToolStripMenuItem";
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Click += new System.EventHandler(this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem_Click);
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
            this.comboBoxStorySelector.SelectedIndexChanged += new System.EventHandler(this.LoadStory);
            this.comboBoxStorySelector.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStorySelector_KeyUp);
            // 
            // storyToolStripMenu
            // 
            this.storyToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storyStoryInformationMenu,
            this.storyDeleteStoryMenu,
            this.storyCopyWithNewNameMenu,
            this.storySplitIntoLinesMenu,
            this.storyRealignStoryLinesMenu,
            this.storyOverrideTasksMenu,
            this.toolStripSeparator14,
            this.storyUseAdaptItForBackTranslationMenu});
            this.storyToolStripMenu.Name = "storyToolStripMenu";
            this.storyToolStripMenu.Size = new System.Drawing.Size(46, 27);
            this.storyToolStripMenu.Text = "&Story";
            this.storyToolStripMenu.DropDownOpening += new System.EventHandler(this.storyToolStripMenuItem_DropDownOpening);
            // 
            // storyStoryInformationMenu
            // 
            this.storyStoryInformationMenu.Name = "storyStoryInformationMenu";
            this.storyStoryInformationMenu.Size = new System.Drawing.Size(245, 22);
            this.storyStoryInformationMenu.Text = "S&tory Information...";
            this.storyStoryInformationMenu.ToolTipText = "Enter information about this story, such as the reason it\'s in the set, the resou" +
    "rces used, etc.";
            this.storyStoryInformationMenu.Click += new System.EventHandler(this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click);
            // 
            // storyDeleteStoryMenu
            // 
            this.storyDeleteStoryMenu.Name = "storyDeleteStoryMenu";
            this.storyDeleteStoryMenu.Size = new System.Drawing.Size(245, 22);
            this.storyDeleteStoryMenu.Text = "&Delete story";
            this.storyDeleteStoryMenu.ToolTipText = "Click to delete the current story";
            this.storyDeleteStoryMenu.Click += new System.EventHandler(this.deleteStoryToolStripMenuItem_Click);
            // 
            // storyCopyWithNewNameMenu
            // 
            this.storyCopyWithNewNameMenu.Name = "storyCopyWithNewNameMenu";
            this.storyCopyWithNewNameMenu.Size = new System.Drawing.Size(245, 22);
            this.storyCopyWithNewNameMenu.Text = "&Copy with new name";
            this.storyCopyWithNewNameMenu.ToolTipText = "Click to make a duplicate copy of the current story with a new name";
            this.storyCopyWithNewNameMenu.Click += new System.EventHandler(this.storyCopyWithNewNameToolStripMenuItem_Click);
            // 
            // storySplitIntoLinesMenu
            // 
            this.storySplitIntoLinesMenu.Name = "storySplitIntoLinesMenu";
            this.storySplitIntoLinesMenu.Size = new System.Drawing.Size(245, 22);
            this.storySplitIntoLinesMenu.Text = "S&plit into Lines";
            this.storySplitIntoLinesMenu.ToolTipText = "Click to split a paragraph of text into lines based on sentence final punctuation" +
    " (alternates with \'Collapse into 1 line\' menu)";
            this.storySplitIntoLinesMenu.Click += new System.EventHandler(this.splitIntoLinesToolStripMenuItem_Click);
            // 
            // storyRealignStoryLinesMenu
            // 
            this.storyRealignStoryLinesMenu.Name = "storyRealignStoryLinesMenu";
            this.storyRealignStoryLinesMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.storyRealignStoryLinesMenu.Size = new System.Drawing.Size(245, 22);
            this.storyRealignStoryLinesMenu.Text = "&Re-align story lines";
            this.storyRealignStoryLinesMenu.ToolTipText = "Click to collapse the lines into a paragraph of text followed by \"Split into line" +
    "s\"";
            this.storyRealignStoryLinesMenu.Click += new System.EventHandler(this.realignStoryVersesToolStripMenuItem_Click);
            // 
            // storyOverrideTasksMenu
            // 
            this.storyOverrideTasksMenu.Name = "storyOverrideTasksMenu";
            this.storyOverrideTasksMenu.Size = new System.Drawing.Size(245, 22);
            this.storyOverrideTasksMenu.Text = "&Override Tasks...";
            this.storyOverrideTasksMenu.ToolTipText = resources.GetString("storyOverrideTasksMenu.ToolTipText");
            this.storyOverrideTasksMenu.Click += new System.EventHandler(this.storyOverrideTasks_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(242, 6);
            // 
            // storyUseAdaptItForBackTranslationMenu
            // 
            this.storyUseAdaptItForBackTranslationMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storyAdaptItVernacularToNationalMenu,
            this.storyAdaptItVernacularToEnglishMenu,
            this.storyAdaptItNationalToEnglishMenu,
            this.toolStripSeparator15,
            this.storySynchronizeSharedAdaptItProjectsMenu});
            this.storyUseAdaptItForBackTranslationMenu.Name = "storyUseAdaptItForBackTranslationMenu";
            this.storyUseAdaptItForBackTranslationMenu.Size = new System.Drawing.Size(245, 22);
            this.storyUseAdaptItForBackTranslationMenu.Text = "&Use Adapt It for back-translation";
            // 
            // storyAdaptItVernacularToNationalMenu
            // 
            this.storyAdaptItVernacularToNationalMenu.Name = "storyAdaptItVernacularToNationalMenu";
            this.storyAdaptItVernacularToNationalMenu.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItVernacularToNationalMenu.Text = "&Story language to National language";
            this.storyAdaptItVernacularToNationalMenu.Click += new System.EventHandler(this.storyAdaptItVernacularToNationalMenuItem_Click);
            // 
            // storyAdaptItVernacularToEnglishMenu
            // 
            this.storyAdaptItVernacularToEnglishMenu.Name = "storyAdaptItVernacularToEnglishMenu";
            this.storyAdaptItVernacularToEnglishMenu.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItVernacularToEnglishMenu.Text = "Story &language to English";
            this.storyAdaptItVernacularToEnglishMenu.Click += new System.EventHandler(this.storyAdaptItVernacularToEnglishMenuItem_Click);
            // 
            // storyAdaptItNationalToEnglishMenu
            // 
            this.storyAdaptItNationalToEnglishMenu.Name = "storyAdaptItNationalToEnglishMenu";
            this.storyAdaptItNationalToEnglishMenu.Size = new System.Drawing.Size(267, 22);
            this.storyAdaptItNationalToEnglishMenu.Text = "National language to &English";
            this.storyAdaptItNationalToEnglishMenu.Click += new System.EventHandler(this.storyAdaptItNationalToEnglishMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(264, 6);
            // 
            // storySynchronizeSharedAdaptItProjectsMenu
            // 
            this.storySynchronizeSharedAdaptItProjectsMenu.Name = "storySynchronizeSharedAdaptItProjectsMenu";
            this.storySynchronizeSharedAdaptItProjectsMenu.Size = new System.Drawing.Size(267, 22);
            this.storySynchronizeSharedAdaptItProjectsMenu.Text = "Synchronize Shared Adapt It &projects";
            this.storySynchronizeSharedAdaptItProjectsMenu.ToolTipText = "Click to Send/Receive the shared Adapt It repository";
            this.storySynchronizeSharedAdaptItProjectsMenu.Click += new System.EventHandler(this.synchronizeSharedAdaptItProjectsToolStripMenuItem_Click);
            // 
            // panoramaToolStripMenu
            // 
            this.panoramaToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panoramaShowMenu,
            this.panoramaInsertNewStoryMenu,
            this.panoramaAddNewStoryAfterMenu});
            this.panoramaToolStripMenu.Name = "panoramaToolStripMenu";
            this.panoramaToolStripMenu.Size = new System.Drawing.Size(73, 27);
            this.panoramaToolStripMenu.Text = "Pa&norama";
            this.panoramaToolStripMenu.DropDownOpening += new System.EventHandler(this.panoramaToolStripMenuItem_DropDownOpening);
            // 
            // panoramaShowMenu
            // 
            this.panoramaShowMenu.Name = "panoramaShowMenu";
            this.panoramaShowMenu.Size = new System.Drawing.Size(235, 22);
            this.panoramaShowMenu.Text = "&Show...";
            this.panoramaShowMenu.ToolTipText = "Show the Panorama View window to see all the stories in the set and their current" +
    " state";
            this.panoramaShowMenu.Click += new System.EventHandler(this.toolStripMenuItemShowPanorama_Click);
            // 
            // panoramaInsertNewStoryMenu
            // 
            this.panoramaInsertNewStoryMenu.Name = "panoramaInsertNewStoryMenu";
            this.panoramaInsertNewStoryMenu.Size = new System.Drawing.Size(235, 22);
            this.panoramaInsertNewStoryMenu.Text = "&Insert new story before current";
            this.panoramaInsertNewStoryMenu.ToolTipText = "Click to insert a new, empty story before the one currently shown";
            this.panoramaInsertNewStoryMenu.Click += new System.EventHandler(this.insertNewStoryToolStripMenuItem_Click);
            // 
            // panoramaAddNewStoryAfterMenu
            // 
            this.panoramaAddNewStoryAfterMenu.Name = "panoramaAddNewStoryAfterMenu";
            this.panoramaAddNewStoryAfterMenu.Size = new System.Drawing.Size(235, 22);
            this.panoramaAddNewStoryAfterMenu.Text = "&Add new story after current";
            this.panoramaAddNewStoryAfterMenu.ToolTipText = "Click to add a new, empty story after the one currently shown";
            this.panoramaAddNewStoryAfterMenu.Click += new System.EventHandler(this.addNewStoryAfterToolStripMenuItem_Click);
            // 
            // advancedToolStripMenu
            // 
            this.advancedToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedProgramUpdatesToolStripMenu,
            this.advancedLocalizationMenu,
            this.advancedOverrideLocalizeStateViewSettingsMenu,
            this.advancedNewProjectMenu,
            this.advancedChangeStateWithoutChecksMenu,
            this.advancedSaveTimeoutToolStripMenu,
            this.advancedResetStoredInformationMenu,
            this.advancedChangeProjectFolderRootMenu,
            this.advancedEmailMenu,
            this.advancedUseOldStyleStoryBtPaneMenu});
            this.advancedToolStripMenu.Name = "advancedToolStripMenu";
            this.advancedToolStripMenu.Size = new System.Drawing.Size(72, 27);
            this.advancedToolStripMenu.Text = "A&dvanced";
            this.advancedToolStripMenu.DropDownOpening += new System.EventHandler(this.advancedToolStripMenuItem_DropDownOpening);
            // 
            // advancedProgramUpdatesToolStripMenu
            // 
            this.advancedProgramUpdatesToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu,
            this.advancedProgramUpdatesCheckNowMenu,
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu});
            this.advancedProgramUpdatesToolStripMenu.Name = "advancedProgramUpdatesToolStripMenu";
            this.advancedProgramUpdatesToolStripMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedProgramUpdatesToolStripMenu.Text = "Program &Updates";
            this.advancedProgramUpdatesToolStripMenu.DropDownOpening += new System.EventHandler(this.programUpdatesToolStripMenuItem_DropDownOpening);
            // 
            // advancedProgramUpdatesAutomaticallyCheckAtStartupMenu
            // 
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.CheckOnClick = true;
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Name = "advancedProgramUpdatesAutomaticallyCheckAtStartupMenu";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Size = new System.Drawing.Size(250, 22);
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Text = "&Automatically check at startup";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.ToolTipText = "Uncheck this menu to stop the program from automatically checking for program upd" +
    "ates when the program is started (this can save startup time)";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.CheckStateChanged += new System.EventHandler(this.automaticallyCheckAtStartupToolStripMenuItem_CheckStateChanged);
            // 
            // advancedProgramUpdatesCheckNowMenu
            // 
            this.advancedProgramUpdatesCheckNowMenu.Name = "advancedProgramUpdatesCheckNowMenu";
            this.advancedProgramUpdatesCheckNowMenu.Size = new System.Drawing.Size(250, 22);
            this.advancedProgramUpdatesCheckNowMenu.Text = "&Check now";
            this.advancedProgramUpdatesCheckNowMenu.ToolTipText = "Click this menu to have the program manually check for program updates";
            this.advancedProgramUpdatesCheckNowMenu.Click += new System.EventHandler(this.checkForProgramUpdatesNowToolStripMenuItem_Click);
            // 
            // advancedProgramUpdatesCheckNowForNextMajorUpdateMenu
            // 
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Name = "advancedProgramUpdatesCheckNowForNextMajorUpdateMenu";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Size = new System.Drawing.Size(250, 22);
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Text = "Check now for next &major update";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.ToolTipText = "Click this menu to have the program check if the next major update is available (" +
    "which wouldn\'t otherwise be installed by default)";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Click += new System.EventHandler(this.checkNowForNextMajorUpdateToolStripMenuItem_Click);
            // 
            // advancedLocalizationMenu
            // 
            this.advancedLocalizationMenu.Name = "advancedLocalizationMenu";
            this.advancedLocalizationMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedLocalizationMenu.Text = "&Localization...";
            this.advancedLocalizationMenu.ToolTipText = "Click this to change or translate the user interface language";
            this.advancedLocalizationMenu.Click += new System.EventHandler(this.localizationToolStripMenuItem_Click);
            // 
            // advancedOverrideLocalizeStateViewSettingsMenu
            // 
            this.advancedOverrideLocalizeStateViewSettingsMenu.Name = "advancedOverrideLocalizeStateViewSettingsMenu";
            this.advancedOverrideLocalizeStateViewSettingsMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedOverrideLocalizeStateViewSettingsMenu.Text = "&Override/Localize turn field viewing settings...";
            this.advancedOverrideLocalizeStateViewSettingsMenu.ToolTipText = "Click to see the \'turn table\' in which you can override which fields are displaye" +
    "d by default and localize the status bar message and instructions for the variou" +
    "s turns";
            this.advancedOverrideLocalizeStateViewSettingsMenu.Click += new System.EventHandler(this.advancedOverrideLocalizeStateViewSettingsMenu_Click);
            // 
            // advancedNewProjectMenu
            // 
            this.advancedNewProjectMenu.Name = "advancedNewProjectMenu";
            this.advancedNewProjectMenu.ShowShortcutKeys = false;
            this.advancedNewProjectMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedNewProjectMenu.Text = "&New Project...";
            this.advancedNewProjectMenu.ToolTipText = "Click to create a new OneStory project (not a new story)";
            this.advancedNewProjectMenu.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // advancedChangeStateWithoutChecksMenu
            // 
            this.advancedChangeStateWithoutChecksMenu.Name = "advancedChangeStateWithoutChecksMenu";
            this.advancedChangeStateWithoutChecksMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedChangeStateWithoutChecksMenu.Text = "&Change Turn without checks...";
            this.advancedChangeStateWithoutChecksMenu.ToolTipText = resources.GetString("advancedChangeStateWithoutChecksMenu.ToolTipText");
            this.advancedChangeStateWithoutChecksMenu.Click += new System.EventHandler(this.changeStateWithoutChecksToolStripMenuItem_Click);
            // 
            // advancedSaveTimeoutToolStripMenu
            // 
            this.advancedSaveTimeoutToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedSaveTimeoutEnabledMenu,
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu});
            this.advancedSaveTimeoutToolStripMenu.Name = "advancedSaveTimeoutToolStripMenu";
            this.advancedSaveTimeoutToolStripMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedSaveTimeoutToolStripMenu.Text = "&Automatic saving";
            // 
            // advancedSaveTimeoutEnabledMenu
            // 
            this.advancedSaveTimeoutEnabledMenu.CheckOnClick = true;
            this.advancedSaveTimeoutEnabledMenu.Name = "advancedSaveTimeoutEnabledMenu";
            this.advancedSaveTimeoutEnabledMenu.Size = new System.Drawing.Size(269, 22);
            this.advancedSaveTimeoutEnabledMenu.Text = "&Enabled";
            this.advancedSaveTimeoutEnabledMenu.ToolTipText = "This menu enables a 5 minute timeout to remind you to save (disable at your own r" +
    "isk)";
            this.advancedSaveTimeoutEnabledMenu.CheckStateChanged += new System.EventHandler(this.enabledToolStripMenuItem_CheckStateChanged);
            // 
            // advancedSaveTimeoutAsSilentlyAsPossibleMenu
            // 
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.CheckOnClick = true;
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Name = "advancedSaveTimeoutAsSilentlyAsPossibleMenu";
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Size = new System.Drawing.Size(269, 22);
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Text = "&Automatically save without reminder";
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.ToolTipText = "This menu indicates whether the program will query you (unchecked) or not (checke" +
    "d) to save the project file";
            // 
            // advancedResetStoredInformationMenu
            // 
            this.advancedResetStoredInformationMenu.Name = "advancedResetStoredInformationMenu";
            this.advancedResetStoredInformationMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedResetStoredInformationMenu.Text = "&Reset Stored Information";
            this.advancedResetStoredInformationMenu.ToolTipText = resources.GetString("advancedResetStoredInformationMenu.ToolTipText");
            this.advancedResetStoredInformationMenu.Click += new System.EventHandler(this.resetStoredInformationToolStripMenuItem_Click);
            // 
            // advancedChangeProjectFolderRootMenu
            // 
            this.advancedChangeProjectFolderRootMenu.Name = "advancedChangeProjectFolderRootMenu";
            this.advancedChangeProjectFolderRootMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedChangeProjectFolderRootMenu.Text = "Change &Project Folder Root";
            this.advancedChangeProjectFolderRootMenu.ToolTipText = "Click this to use a different location for the root folder (i.e. \"OneStory Editor" +
    " Projects\") besides in your \"My Documents\" folder";
            this.advancedChangeProjectFolderRootMenu.Click += new System.EventHandler(this.changeProjectFolderRootToolStripMenuItem_Click);
            // 
            // advancedEmailMenu
            // 
            this.advancedEmailMenu.Checked = true;
            this.advancedEmailMenu.CheckOnClick = true;
            this.advancedEmailMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.advancedEmailMenu.Name = "advancedEmailMenu";
            this.advancedEmailMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedEmailMenu.Text = "&Email via MAPI+";
            this.advancedEmailMenu.ToolTipText = resources.GetString("advancedEmailMenu.ToolTipText");
            this.advancedEmailMenu.Click += new System.EventHandler(this.advancedEmailMenu_Click);
            // 
            // advancedUseOldStyleStoryBtPaneMenu
            // 
            this.advancedUseOldStyleStoryBtPaneMenu.CheckOnClick = true;
            this.advancedUseOldStyleStoryBtPaneMenu.Name = "advancedUseOldStyleStoryBtPaneMenu";
            this.advancedUseOldStyleStoryBtPaneMenu.Size = new System.Drawing.Size(314, 22);
            this.advancedUseOldStyleStoryBtPaneMenu.Text = "Use old-style Story BT pane";
            this.advancedUseOldStyleStoryBtPaneMenu.ToolTipText = "This setting switches the Story BT pane to use the old-style (and slower) control" +
    "s for editing the Story BT data";
            this.advancedUseOldStyleStoryBtPaneMenu.Click += new System.EventHandler(this.advancedUseOldStyleStoryBtPaneMenu_Click);
            // 
            // aboutToolStripMenu
            // 
            this.aboutToolStripMenu.Name = "aboutToolStripMenu";
            this.aboutToolStripMenu.Size = new System.Drawing.Size(52, 27);
            this.aboutToolStripMenu.Text = "&About";
            this.aboutToolStripMenu.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.saveFileDialog.Title = "Save OneStory Project File";
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
            this.splitContainerLeftRight.Size = new System.Drawing.Size(881, 613);
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
            this.splitContainerUpDown.Panel1.Controls.Add(this.linkLabelTasks);
            this.splitContainerUpDown.Panel1.Controls.Add(this.linkLabelVerseBT);
            this.splitContainerUpDown.Panel1.Controls.Add(this.flowLayoutPanelVerses);
            this.splitContainerUpDown.Panel1.Controls.Add(this.htmlStoryBtControl);
            this.splitContainerUpDown.Panel1.Controls.Add(this.textBoxStoryVerse);
            // 
            // splitContainerUpDown.Panel2
            // 
            this.splitContainerUpDown.Panel2.Controls.Add(this.netBibleViewer);
            this.splitContainerUpDown.Size = new System.Drawing.Size(453, 613);
            this.splitContainerUpDown.SplitterDistance = 391;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // linkLabelTasks
            // 
            this.linkLabelTasks.AutoSize = true;
            this.linkLabelTasks.Location = new System.Drawing.Point(63, 5);
            this.linkLabelTasks.Name = "linkLabelTasks";
            this.linkLabelTasks.Size = new System.Drawing.Size(36, 13);
            this.linkLabelTasks.TabIndex = 4;
            this.linkLabelTasks.TabStop = true;
            this.linkLabelTasks.Tag = 1;
            this.linkLabelTasks.Text = "Tasks";
            this.linkLabelTasks.Visible = false;
            this.linkLabelTasks.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTasks_LinkClicked);
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
            this.flowLayoutPanelVerses.Size = new System.Drawing.Size(451, 366);
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
            // htmlStoryBtControl
            // 
            this.htmlStoryBtControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControl.IsWebBrowserContextMenuEnabled = false;
            this.htmlStoryBtControl.Location = new System.Drawing.Point(-1, 23);
            this.htmlStoryBtControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControl.Name = "htmlStoryBtControl";
            this.htmlStoryBtControl.ParentStory = null;
            this.htmlStoryBtControl.Size = new System.Drawing.Size(451, 269);
            this.htmlStoryBtControl.StoryData = null;
            this.htmlStoryBtControl.TabIndex = 5;
            this.htmlStoryBtControl.TheSE = null;
            this.htmlStoryBtControl.ViewSettings = null;
            // 
            // netBibleViewer
            // 
            this.netBibleViewer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.netBibleViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netBibleViewer.Location = new System.Drawing.Point(0, 0);
            this.netBibleViewer.Margin = new System.Windows.Forms.Padding(0);
            this.netBibleViewer.Name = "netBibleViewer";
            this.netBibleViewer.ScriptureReference = "Gen 1:1";
            this.netBibleViewer.Size = new System.Drawing.Size(451, 216);
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
            this.splitContainerMentorNotes.Size = new System.Drawing.Size(424, 613);
            this.splitContainerMentorNotes.SplitterDistance = 356;
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
            this.htmlConsultantNotesControl.Size = new System.Drawing.Size(422, 331);
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
            this.htmlCoachNotesControl.Size = new System.Drawing.Size(422, 228);
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
            this.toolStripButtonShowPanoramaStories,
            this.toolStripButtonFirst,
            this.toolStripButtonPrevious,
            this.toolStripButtonNext,
            this.toolStripButtonLast});
            this.toolStripRecordNavigation.Location = new System.Drawing.Point(471, 0);
            this.toolStripRecordNavigation.Name = "toolStripRecordNavigation";
            this.toolStripRecordNavigation.Size = new System.Drawing.Size(118, 25);
            this.toolStripRecordNavigation.TabIndex = 3;
            this.toolStripRecordNavigation.Text = "<no need to localize/translate>";
            // 
            // toolStripButtonShowPanoramaStories
            // 
            this.toolStripButtonShowPanoramaStories.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowPanoramaStories.Image = global::OneStoryProjectEditor.Properties.Resources.ShowAllCommentsHS;
            this.toolStripButtonShowPanoramaStories.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowPanoramaStories.Name = "toolStripButtonShowPanoramaStories";
            this.toolStripButtonShowPanoramaStories.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowPanoramaStories.Text = "Show Panorama Stories";
            this.toolStripButtonShowPanoramaStories.ToolTipText = "Click to view the list of full stories (same as \"Panorama\", \"Show\")";
            this.toolStripButtonShowPanoramaStories.Click += new System.EventHandler(this.toolStripMenuItemShowPanorama_Click);
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
            // statusLabel
            // 
            this.statusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(866, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLabel.Click += new System.EventHandler(this.statusLabel_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 644);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(881, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "<no need to localize/translate>";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 666);
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
        private ToolStripMenuItem projectToolStripMenu;
        private ToolStripMenuItem projectBrowseForProjectFileMenu;
        private ToolStripMenuItem projectSaveProjectMenu;
        private ToolStripMenuItem projectExitMenu;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem projectSettingsMenu;
        private ToolStripSeparator toolStripSeparator2;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        internal VerseBtLineFlowLayoutPanel flowLayoutPanelVerses;
        internal SplitContainer splitContainerLeftRight;
        internal MinimizableSplitterContainer splitContainerUpDown;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        internal ToolStripMenuItem viewVernacularLangMenu;
        internal ToolStripMenuItem viewNationalLangMenu;
        internal ToolStripMenuItem viewEnglishBtMenu;
        internal ToolStripMenuItem viewAnchorsMenu;
        internal ToolStripMenuItem viewStoryTestingQuestionsMenu;
        internal ToolStripMenuItem viewRetellingsMenu;
        internal ToolStripMenuItem viewConsultantNotesMenu;
        internal ToolStripMenuItem viewBibleMenu;
        internal ToolStripMenuItem viewCoachNotesMenu;
        private NetBibleViewer netBibleViewer;
        private ToolStripSeparator toolStripSeparator3;
        private SplitContainer splitContainerMentorNotes;
        private TextBox textBoxConsultantNotesTable;
        private TextBox textBoxCoachNotes;
        private TextBox textBoxStoryVerse;
        private ToolStripComboBox comboBoxStorySelector;
        private ToolStripMenuItem advancedNewProjectMenu;
        private ToolStripMenuItem projectRecentProjectsMenu;
        private ToolStripSeparator toolStripSeparator4;
        private HelpProvider helpProvider;
        private ToolStripMenuItem panoramaInsertNewStoryMenu;
        private ToolStripMenuItem storyDeleteStoryMenu;
        private ToolStripMenuItem panoramaAddNewStoryAfterMenu;
        private ToolStripMenuItem storyStoryInformationMenu;
        private ToolStripMenuItem storyToolStripMenu;
        private ToolStripMenuItem editToolStripMenu;
        private ToolStripMenuItem editCopyToolStripMenu;
        private ToolStripMenuItem editCopyStoryMenu;
        private ToolStripMenuItem editCopyNationalBtMenu;
        private ToolStripMenuItem editCopyEnglishBtMenu;
        private ToolStripMenuItem editDeleteToolStripMenu;
        private ToolStripMenuItem editDeleteNationalBtMenu;
        private ToolStripMenuItem editDeleteEnglishBtMenu;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem viewRefreshMenu;
        private ToolStripMenuItem editPasteMenu;
        private ToolStripMenuItem storySplitIntoLinesMenu;
        private ToolStripMenuItem aboutToolStripMenu;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem viewOldStoriesMenu;
        private ToolStripMenuItem editDeleteStoryLinesMenu;
        private ToolStripMenuItem editDeleteTestToolStripMenu;
        private ToolStripMenuItem panoramaShowMenu;
        private ToolStripMenuItem panoramaToolStripMenu;
        private ToolStripMenuItem editAddRetellingTestResultsMenu;
        private ToolStripMenuItem editCopySelectionMenu;
        private ToolStripMenuItem projectFromTheInternetMenu;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem editFindMenu;
        private ToolStripMenuItem editFindNextMenu;
        private ToolStripMenuItem editReplaceMenu;
        private ToolStripMenuItem advancedToolStripMenu;
        private ToolStripMenuItem advancedChangeProjectFolderRootMenu;
        private ToolStripMenuItem projectFromASharedNetworkDriveMenu;
        private ToolStripMenuItem storyRealignStoryLinesMenu;
        private ToolStripMenuItem projectToTheInternetMenu;
        private ToolStripMenuItem storyCopyWithNewNameMenu;
        private ToolStripMenuItem projectLoginMenu;
        private ToolStripSeparator toolStripSeparator11;
        internal ToolStripMenuItem viewHiddenVersesMenu;
        private ToolStripMenuItem projectExportToToolboxMenu;
        private ToolStripMenuItem viewShowHideFieldsMenu;
        internal HtmlConsultantNotesControl htmlConsultantNotesControl;
        internal HtmlCoachNotesControl htmlCoachNotesControl;
        private ToolStripMenuItem viewTransliterationsToolStripMenu;
        internal ToolStripMenuItem viewTransliterationVernacular;
        internal ToolStripMenuItem viewTransliterationNationalBT;
        internal ToolStripMenuItem viewTransliterationInternationalBt;
        internal ToolStripMenuItem viewTransliterationFreeTranslation;
        private ToolStripMenuItem viewTransliteratorVernacularConfigureToolStripMenuItem;
        private ToolStripMenuItem viewTransliteratorNationalBTConfigureToolStripMenuItem;
        internal LinkLabel linkLabelConsultantNotes;
        internal LinkLabel linkLabelCoachNotes;
        internal LinkLabel linkLabelVerseBT;
        internal LinkLabel linkLabelTasks;
        private ContextMenuStrip contextMenuStripVerseList;
        private ToolStripMenuItem advancedResetStoredInformationMenu;
        private ToolStripMenuItem viewHistoricalDifferencesMenu;
        private ToolStripMenuItem viewUseSameSettingsForAllStoriesMenu;
        private ToolStripMenuItem projectPrintMenu;
        private ToolStripSeparator toolStripSeparator12;
        internal ToolStripMenuItem viewStoryTestingQuestionAnswersMenu;
        private ToolStripMenuItem viewConcordanceMenu;
        private ToolStrip toolStripRecordNavigation;
        private ToolStripButton toolStripButtonFirst;
        private ToolStripButton toolStripButtonPrevious;
        private ToolStripButton toolStripButtonNext;
        private ToolStripButton toolStripButtonLast;
        private ToolStripStatusLabel statusLabel;
        private StatusStrip statusStrip;
        private ToolStripSeparator toolStripSeparator13;
        internal ToolStripMenuItem viewOnlyOpenConversationsMenu;
        private ToolStripMenuItem viewStateTransitionHistoryMenu;
        private ToolStripMenuItem advancedChangeStateWithoutChecksMenu;
        private ToolStripMenuItem advancedProgramUpdatesToolStripMenu;
        private ToolStripMenuItem advancedProgramUpdatesAutomaticallyCheckAtStartupMenu;
        private ToolStripMenuItem advancedProgramUpdatesCheckNowMenu;
        private ToolStripMenuItem advancedSaveTimeoutToolStripMenu;
        private ToolStripMenuItem advancedSaveTimeoutEnabledMenu;
        private ToolStripMenuItem advancedSaveTimeoutAsSilentlyAsPossibleMenu;
        private ToolStripMenuItem projectSendReceiveMenu;
        private ToolStripMenuItem viewLnCNotesMenu;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem storyUseAdaptItForBackTranslationMenu;
        private ToolStripMenuItem storyAdaptItVernacularToNationalMenu;
        private ToolStripMenuItem storyAdaptItVernacularToEnglishMenu;
        private ToolStripMenuItem storyAdaptItNationalToEnglishMenu;
        internal ToolStripMenuItem viewFreeTranslationMenu;
        private ToolStripMenuItem editCopyFreeTranslationMenu;
        private ToolStripMenuItem editDeleteFreeTranslationMenu;
        private ToolStripMenuItem editAddInferenceTestResultsMenu;
        private ToolStripMenuItem storySynchronizeSharedAdaptItProjectsMenu;
        private ToolStripMenuItem editAddGeneralTestQuestionMenu;
        internal ToolStripMenuItem viewGeneralTestingsQuestionMenu;
        private ToolStripMenuItem projectToAThumbdriveMenu;
        private ToolStripMenuItem projectCloseProjectMenu;
        internal ToolStripMenuItem viewExegeticalHelps;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private ToolStripButton toolStripButtonShowPanoramaStories;
        private ToolStripMenuItem advancedProgramUpdatesCheckNowForNextMajorUpdateMenu;
        private ToolStripMenuItem viewTransliteratorInternationalBtConfigureToolStripMenuItem;
        private ToolStripMenuItem viewTransliteratorFreeTranslationConfigureToolStripMenuItem;
        private ToolStripMenuItem storyOverrideTasksMenu;
        private ToolStripMenuItem viewProjectNotesMenu;
        private ToolStripMenuItem advancedLocalizationMenu;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem advancedOverrideLocalizeStateViewSettingsMenu;
        private ToolStripMenuItem advancedEmailMenu;
        private ToolStripMenuItem viewNonBiblicalStoriesMenu;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripSeparator toolStripSeparator5;
        private HtmlStoryBtControl htmlStoryBtControl;
        private ToolStripMenuItem advancedUseOldStyleStoryBtPaneMenu;
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

