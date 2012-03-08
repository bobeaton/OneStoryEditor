#define UsingHtmlDisplayForConNotes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    partial class StoryEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(StoryEditor));
            this.menuStrip = new MenuStrip();
            this.projectToolStripMenu = new ToolStripMenuItem();
            this.projectRecentProjectsMenu = new ToolStripMenuItem();
            this.projectSendReceiveMenu = new ToolStripMenuItem();
            this.projectCloseProjectMenu = new ToolStripMenuItem();
            this.projectSaveProjectMenu = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.projectBrowseForProjectFileMenu = new ToolStripMenuItem();
            this.projectFromTheInternetMenu = new ToolStripMenuItem();
            this.projectToTheInternetMenu = new ToolStripMenuItem();
            this.projectToAThumbdriveMenu = new ToolStripMenuItem();
            this.projectFromASharedNetworkDriveMenu = new ToolStripMenuItem();
            this.toolStripSeparator4 = new ToolStripSeparator();
            this.projectSettingsMenu = new ToolStripMenuItem();
            this.projectLoginMenu = new ToolStripMenuItem();
            this.projectExportToToolboxMenu = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.projectPrintMenu = new ToolStripMenuItem();
            this.toolStripSeparator12 = new ToolStripSeparator();
            this.projectExitMenu = new ToolStripMenuItem();
            this.editToolStripMenu = new ToolStripMenuItem();
            this.editCopyToolStripMenu = new ToolStripMenuItem();
            this.editCopySelectionMenu = new ToolStripMenuItem();
            this.editCopyStoryMenu = new ToolStripMenuItem();
            this.editCopyNationalBtMenu = new ToolStripMenuItem();
            this.editCopyEnglishBtMenu = new ToolStripMenuItem();
            this.editCopyFreeTranslationMenu = new ToolStripMenuItem();
            this.editPasteMenu = new ToolStripMenuItem();
            this.editDeleteToolStripMenu = new ToolStripMenuItem();
            this.editDeleteStoryLinesMenu = new ToolStripMenuItem();
            this.editDeleteNationalBtMenu = new ToolStripMenuItem();
            this.editDeleteEnglishBtMenu = new ToolStripMenuItem();
            this.editDeleteFreeTranslationMenu = new ToolStripMenuItem();
            this.editDeleteTestToolStripMenu = new ToolStripMenuItem();
            this.toolStripSeparator9 = new ToolStripSeparator();
            this.editFindMenu = new ToolStripMenuItem();
            this.editFindNextMenu = new ToolStripMenuItem();
            this.editReplaceMenu = new ToolStripMenuItem();
            this.toolStripSeparator10 = new ToolStripSeparator();
            this.editAddRetellingTestResultsMenu = new ToolStripMenuItem();
            this.editAddInferenceTestResultsMenu = new ToolStripMenuItem();
            this.editAddGeneralTestQuestionMenu = new ToolStripMenuItem();
            this.viewToolStripMenuItem = new ToolStripMenuItem();
            this.viewNonBiblicalStoriesMenu = new ToolStripMenuItem();
            this.toolStripSeparator5 = new ToolStripSeparator();
            this.viewShowHideFieldsMenu = new ToolStripMenuItem();
            this.viewUseSameSettingsForAllStoriesMenu = new ToolStripMenuItem();
            this.toolStripSeparator16 = new ToolStripSeparator();
            this.viewVernacularLangMenu = new ToolStripMenuItem();
            this.viewNationalLangMenu = new ToolStripMenuItem();
            this.viewEnglishBtMenu = new ToolStripMenuItem();
            this.viewFreeTranslationMenu = new ToolStripMenuItem();
            this.viewAnchorsMenu = new ToolStripMenuItem();
            this.viewExegeticalHelps = new ToolStripMenuItem();
            this.viewGeneralTestingsQuestionMenu = new ToolStripMenuItem();
            this.viewStoryTestingQuestionsMenu = new ToolStripMenuItem();
            this.viewStoryTestingQuestionAnswersMenu = new ToolStripMenuItem();
            this.viewRetellingsMenu = new ToolStripMenuItem();
            this.toolStripSeparator6 = new ToolStripSeparator();
            this.viewConsultantNotesMenu = new ToolStripMenuItem();
            this.viewCoachNotesMenu = new ToolStripMenuItem();
            this.toolStripSeparator3 = new ToolStripSeparator();
            this.viewBibleMenu = new ToolStripMenuItem();
            this.toolStripSeparator7 = new ToolStripSeparator();
            this.viewRefreshMenu = new ToolStripMenuItem();
            this.toolStripSeparator8 = new ToolStripSeparator();
            this.viewHistoricalDifferencesMenu = new ToolStripMenuItem();
            this.viewLnCNotesMenu = new ToolStripMenuItem();
            this.viewConcordanceMenu = new ToolStripMenuItem();
            this.viewStateTransitionHistoryMenu = new ToolStripMenuItem();
            this.viewProjectNotesMenu = new ToolStripMenuItem();
            this.viewOldStoriesMenu = new ToolStripMenuItem();
            this.toolStripSeparator11 = new ToolStripSeparator();
            this.viewHiddenVersesMenu = new ToolStripMenuItem();
            this.viewOnlyOpenConversationsMenu = new ToolStripMenuItem();
            this.toolStripSeparator13 = new ToolStripSeparator();
            this.viewTransliterationsToolStripMenu = new ToolStripMenuItem();
            this.viewTransliterationVernacular = new ToolStripMenuItem();
            this.viewTransliteratorVernacularConfigureToolStripMenuItem = new ToolStripMenuItem();
            this.viewTransliterationNationalBT = new ToolStripMenuItem();
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem = new ToolStripMenuItem();
            this.viewTransliterationInternationalBt = new ToolStripMenuItem();
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem = new ToolStripMenuItem();
            this.viewTransliterationFreeTranslation = new ToolStripMenuItem();
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem = new ToolStripMenuItem();
            this.comboBoxStorySelector = new ToolStripComboBox();
            this.storyToolStripMenu = new ToolStripMenuItem();
            this.storyStoryInformationMenu = new ToolStripMenuItem();
            this.storyDeleteStoryMenu = new ToolStripMenuItem();
            this.storyCopyWithNewNameMenu = new ToolStripMenuItem();
            this.storySplitIntoLinesMenu = new ToolStripMenuItem();
            this.storyRealignStoryLinesMenu = new ToolStripMenuItem();
            this.storyOverrideTasksMenu = new ToolStripMenuItem();
            this.toolStripSeparator14 = new ToolStripSeparator();
            this.storyUseAdaptItForBackTranslationMenu = new ToolStripMenuItem();
            this.storyAdaptItVernacularToNationalMenu = new ToolStripMenuItem();
            this.storyAdaptItVernacularToEnglishMenu = new ToolStripMenuItem();
            this.storyAdaptItNationalToEnglishMenu = new ToolStripMenuItem();
            this.toolStripSeparator15 = new ToolStripSeparator();
            this.storySynchronizeSharedAdaptItProjectsMenu = new ToolStripMenuItem();
            this.panoramaToolStripMenu = new ToolStripMenuItem();
            this.panoramaShowMenu = new ToolStripMenuItem();
            this.panoramaInsertNewStoryMenu = new ToolStripMenuItem();
            this.panoramaAddNewStoryAfterMenu = new ToolStripMenuItem();
            this.advancedToolStripMenu = new ToolStripMenuItem();
            this.advancedProgramUpdatesToolStripMenu = new ToolStripMenuItem();
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu = new ToolStripMenuItem();
            this.advancedProgramUpdatesCheckNowMenu = new ToolStripMenuItem();
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu = new ToolStripMenuItem();
            this.advancedLocalizationMenu = new ToolStripMenuItem();
            this.advancedOverrideLocalizeStateViewSettingsMenu = new ToolStripMenuItem();
            this.advancedNewProjectMenu = new ToolStripMenuItem();
            this.advancedChangeStateWithoutChecksMenu = new ToolStripMenuItem();
            this.advancedSaveTimeoutToolStripMenu = new ToolStripMenuItem();
            this.advancedSaveTimeoutEnabledMenu = new ToolStripMenuItem();
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu = new ToolStripMenuItem();
            this.advancedResetStoredInformationMenu = new ToolStripMenuItem();
            this.advancedChangeProjectFolderRootMenu = new ToolStripMenuItem();
            this.advancedEmailMenu = new ToolStripMenuItem();
            this.advancedUseOldStyleStoryBtPaneMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedUseWordBreaks = new ToolStripMenuItem();
            this.aboutToolStripMenu = new ToolStripMenuItem();
            this.openFileDialog = new OpenFileDialog();
            this.saveFileDialog = new SaveFileDialog();
            this.splitContainerLeftRight = new SplitContainer();
            this.splitContainerUpDown = new MinimizableSplitterContainer();
            this.linkLabelTasks = new LinkLabel();
            this.linkLabelVerseBT = new LinkLabel();
            this.contextMenuStripVerseList = new ContextMenuStrip(this.components);
            this.flowLayoutPanelVerses = new VerseBtLineFlowLayoutPanel();
            this.textBoxStoryVerse = new TextBox();
            this.htmlStoryBtControl = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.netBibleViewer = new NetBibleViewer();
            this.splitContainerMentorNotes = new SplitContainer();
            this.linkLabelConsultantNotes = new LinkLabel();
            this.htmlConsultantNotesControl = new HtmlConsultantNotesControl();
            this.textBoxConsultantNotesTable = new TextBox();
            this.linkLabelCoachNotes = new LinkLabel();
            this.htmlCoachNotesControl = new HtmlCoachNotesControl();
            this.textBoxCoachNotes = new TextBox();
            this.helpProvider = new HelpProvider();
            this.toolStripRecordNavigation = new ToolStrip();
            this.toolStripButtonShowPanoramaStories = new ToolStripButton();
            this.toolStripButtonFirst = new ToolStripButton();
            this.toolStripButtonPrevious = new ToolStripButton();
            this.toolStripButtonNext = new ToolStripButton();
            this.toolStripButtonLast = new ToolStripButton();
            this.statusLabel = new ToolStripStatusLabel();
            this.statusStrip = new StatusStrip();
            this.backgroundWorker = new BackgroundWorker();
            this.advancedImportHelper = new ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            ((ISupportInitialize)(this.splitContainerLeftRight)).BeginInit();
            this.splitContainerLeftRight.Panel1.SuspendLayout();
            this.splitContainerLeftRight.Panel2.SuspendLayout();
            this.splitContainerLeftRight.SuspendLayout();
            ((ISupportInitialize)(this.splitContainerUpDown)).BeginInit();
            this.splitContainerUpDown.Panel1.SuspendLayout();
            this.splitContainerUpDown.Panel2.SuspendLayout();
            this.splitContainerUpDown.SuspendLayout();
            ((ISupportInitialize)(this.splitContainerMentorNotes)).BeginInit();
            this.splitContainerMentorNotes.Panel1.SuspendLayout();
            this.splitContainerMentorNotes.Panel2.SuspendLayout();
            this.splitContainerMentorNotes.SuspendLayout();
            this.toolStripRecordNavigation.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
            this.projectToolStripMenu,
            this.editToolStripMenu,
            this.viewToolStripMenuItem,
            this.comboBoxStorySelector,
            this.storyToolStripMenu,
            this.panoramaToolStripMenu,
            this.advancedToolStripMenu,
            this.aboutToolStripMenu});
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(881, 31);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // projectToolStripMenu
            // 
            this.projectToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
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
            this.projectToolStripMenu.Size = new Size(56, 27);
            this.projectToolStripMenu.Text = "&Project";
            this.projectToolStripMenu.DropDownOpening += new EventHandler(this.projectToolStripMenuItem_DropDownOpening);
            // 
            // projectRecentProjectsMenu
            // 
            this.projectRecentProjectsMenu.Name = "projectRecentProjectsMenu";
            this.projectRecentProjectsMenu.Size = new Size(286, 22);
            this.projectRecentProjectsMenu.Text = "&Recent projects";
            this.projectRecentProjectsMenu.ToolTipText = "This shows the projects that have at one time or other been opened on this machin" +
    "e";
            // 
            // projectSendReceiveMenu
            // 
            this.projectSendReceiveMenu.Name = "projectSendReceiveMenu";
            this.projectSendReceiveMenu.Size = new Size(286, 22);
            this.projectSendReceiveMenu.Text = "Sen&d/Receive...";
            this.projectSendReceiveMenu.ToolTipText = "Click to synchronize this project with the Internet (or thumbdrive) repository";
            this.projectSendReceiveMenu.Click += new EventHandler(this.sendReceiveToolStripMenuItem_Click);
            // 
            // projectCloseProjectMenu
            // 
            this.projectCloseProjectMenu.Name = "projectCloseProjectMenu";
            this.projectCloseProjectMenu.Size = new Size(286, 22);
            this.projectCloseProjectMenu.Text = "&Close project";
            this.projectCloseProjectMenu.ToolTipText = "Click to save the OneStory project";
            this.projectCloseProjectMenu.Click += new EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // projectSaveProjectMenu
            // 
            this.projectSaveProjectMenu.Name = "projectSaveProjectMenu";
            this.projectSaveProjectMenu.ShortcutKeys = ((Keys)((Keys.Control | Keys.S)));
            this.projectSaveProjectMenu.Size = new Size(286, 22);
            this.projectSaveProjectMenu.Text = "&Save project";
            this.projectSaveProjectMenu.ToolTipText = "Click to save the OneStory project";
            this.projectSaveProjectMenu.Click += new EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(283, 6);
            // 
            // projectBrowseForProjectFileMenu
            // 
            this.projectBrowseForProjectFileMenu.Name = "projectBrowseForProjectFileMenu";
            this.projectBrowseForProjectFileMenu.Size = new Size(286, 22);
            this.projectBrowseForProjectFileMenu.Text = "&Browse for project file";
            this.projectBrowseForProjectFileMenu.ToolTipText = "Click this option to open an existing OneStory project";
            this.projectBrowseForProjectFileMenu.Click += new EventHandler(this.browseForProjectToolStripMenuItem_Click);
            // 
            // projectFromTheInternetMenu
            // 
            this.projectFromTheInternetMenu.Name = "projectFromTheInternetMenu";
            this.projectFromTheInternetMenu.Size = new Size(286, 22);
            this.projectFromTheInternetMenu.Text = "&From the Internet...";
            this.projectFromTheInternetMenu.ToolTipText = "Click here to enter an Internet address to get a project from (e.g. if a team mat" +
    "e has already uploaded it to the internet repository)";
            this.projectFromTheInternetMenu.Click += new EventHandler(this.projectFromTheInternetToolStripMenuItem_Click);
            // 
            // projectToTheInternetMenu
            // 
            this.projectToTheInternetMenu.Name = "projectToTheInternetMenu";
            this.projectToTheInternetMenu.Size = new Size(286, 22);
            this.projectToTheInternetMenu.Text = "&To the Internet...";
            this.projectToTheInternetMenu.ToolTipText = "Click here to enter the Internet address of the repository to send this project t" +
    "o (e.g. if you have created a new project and want to \"push\" it to an existing i" +
    "nternet repository)";
            this.projectToTheInternetMenu.Click += new EventHandler(this.toTheInternetToolStripMenuItem_Click);
            // 
            // projectToAThumbdriveMenu
            // 
            this.projectToAThumbdriveMenu.Name = "projectToAThumbdriveMenu";
            this.projectToAThumbdriveMenu.Size = new Size(286, 22);
            this.projectToAThumbdriveMenu.Text = "Transfer via thum&bdrive...";
            this.projectToAThumbdriveMenu.ToolTipText = resources.GetString("projectToAThumbdriveMenu.ToolTipText");
            this.projectToAThumbdriveMenu.Click += new EventHandler(this.toAThumbdriveToolStripMenuItem_Click);
            // 
            // projectFromASharedNetworkDriveMenu
            // 
            this.projectFromASharedNetworkDriveMenu.Name = "projectFromASharedNetworkDriveMenu";
            this.projectFromASharedNetworkDriveMenu.Size = new Size(286, 22);
            this.projectFromASharedNetworkDriveMenu.Text = "&Associate with a shared network folder...";
            this.projectFromASharedNetworkDriveMenu.ToolTipText = "Click here to associate this project with a repository on a network drive (e.g. f" +
    "or working together at a workshop where Internet connectivity is difficult).";
            this.projectFromASharedNetworkDriveMenu.Click += new EventHandler(this.projectFromASharedNetworkDriveToolStripMenu_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new Size(283, 6);
            // 
            // projectSettingsMenu
            // 
            this.projectSettingsMenu.Name = "projectSettingsMenu";
            this.projectSettingsMenu.Size = new Size(286, 22);
            this.projectSettingsMenu.Text = "Se&ttings...";
            this.projectSettingsMenu.ToolTipText = "Click here to open the Project Settings dialog in order to edit the language prop" +
    "erties (fonts, keyboards, etc) or other project configuration information";
            this.projectSettingsMenu.Click += new EventHandler(this.projectSettingsToolStripMenuItem_Click);
            // 
            // projectLoginMenu
            // 
            this.projectLoginMenu.Name = "projectLoginMenu";
            this.projectLoginMenu.Size = new Size(286, 22);
            this.projectLoginMenu.Text = "&Login...";
            this.projectLoginMenu.ToolTipText = "Click to login as a specific member name";
            this.projectLoginMenu.Click += new EventHandler(this.projectLoginToolStripMenuItem_Click);
            // 
            // projectExportToToolboxMenu
            // 
            this.projectExportToToolboxMenu.Name = "projectExportToToolboxMenu";
            this.projectExportToToolboxMenu.Size = new Size(286, 22);
            this.projectExportToToolboxMenu.Text = "E&xport to Toolbox";
            this.projectExportToToolboxMenu.ToolTipText = "Click here to export the OneStory Editor project to a Toolbox readable format (in" +
    " the \'Toolbox\' sub-folder of the OSE project folder)";
            this.projectExportToToolboxMenu.Click += new EventHandler(this.exportToToolboxToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(283, 6);
            // 
            // projectPrintMenu
            // 
            this.projectPrintMenu.Name = "projectPrintMenu";
            this.projectPrintMenu.Size = new Size(286, 22);
            this.projectPrintMenu.Text = "&Print...";
            this.projectPrintMenu.ToolTipText = "Click here to configure a print preview of the stories that can then be printed o" +
    "r saved in HTML format";
            this.projectPrintMenu.Click += new EventHandler(this.printPreviewToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new Size(283, 6);
            // 
            // projectExitMenu
            // 
            this.projectExitMenu.Name = "projectExitMenu";
            this.projectExitMenu.ShortcutKeys = ((Keys)((Keys.Alt | Keys.F4)));
            this.projectExitMenu.Size = new Size(286, 22);
            this.projectExitMenu.Text = "&Exit";
            this.projectExitMenu.ToolTipText = "Click to exit the program and synchronize with the internet repositories";
            this.projectExitMenu.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenu
            // 
            this.editToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
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
            this.editToolStripMenu.Size = new Size(39, 27);
            this.editToolStripMenu.Text = "&Edit";
            this.editToolStripMenu.DropDownOpening += new EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // editCopyToolStripMenu
            // 
            this.editCopyToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.editCopySelectionMenu,
            this.editCopyStoryMenu,
            this.editCopyNationalBtMenu,
            this.editCopyEnglishBtMenu,
            this.editCopyFreeTranslationMenu});
            this.editCopyToolStripMenu.Name = "editCopyToolStripMenu";
            this.editCopyToolStripMenu.Size = new Size(269, 22);
            this.editCopyToolStripMenu.Text = "&Copy";
            // 
            // editCopySelectionMenu
            // 
            this.editCopySelectionMenu.Name = "editCopySelectionMenu";
            this.editCopySelectionMenu.Size = new Size(386, 22);
            this.editCopySelectionMenu.Text = "Sele&ction";
            this.editCopySelectionMenu.ToolTipText = "Copy the selected text from the active text box to the clipboard";
            this.editCopySelectionMenu.Click += new EventHandler(this.editCopySelectionToolStripMenuItem_Click);
            // 
            // editCopyStoryMenu
            // 
            this.editCopyStoryMenu.Name = "editCopyStoryMenu";
            this.editCopyStoryMenu.Size = new Size(386, 22);
            this.editCopyStoryMenu.Text = "&Story";
            this.editCopyStoryMenu.ToolTipText = "Copy all of the lines of text in the story language into one big paragraph of tex" +
    "t";
            this.editCopyStoryMenu.Click += new EventHandler(this.copyStoryToolStripMenuItem_Click);
            // 
            // editCopyNationalBtMenu
            // 
            this.editCopyNationalBtMenu.Name = "editCopyNationalBtMenu";
            this.editCopyNationalBtMenu.Size = new Size(386, 22);
            this.editCopyNationalBtMenu.Text = "&National back-translation";
            this.editCopyNationalBtMenu.ToolTipText = "Copy all of the lines of text in the National back-translation language into one " +
    "big paragraph of text";
            this.editCopyNationalBtMenu.Click += new EventHandler(this.copyNationalBackTranslationToolStripMenuItem_Click);
            // 
            // editCopyEnglishBtMenu
            // 
            this.editCopyEnglishBtMenu.Name = "editCopyEnglishBtMenu";
            this.editCopyEnglishBtMenu.Size = new Size(386, 22);
            this.editCopyEnglishBtMenu.Text = "&English back-translation of the whole story to the clipboard";
            this.editCopyEnglishBtMenu.ToolTipText = "Copy all of the lines of text in the English back-translation into one big paragr" +
    "aph of text";
            this.editCopyEnglishBtMenu.Click += new EventHandler(this.copyEnglishBackTranslationToolStripMenuItem_Click);
            // 
            // editCopyFreeTranslationMenu
            // 
            this.editCopyFreeTranslationMenu.Name = "editCopyFreeTranslationMenu";
            this.editCopyFreeTranslationMenu.Size = new Size(386, 22);
            this.editCopyFreeTranslationMenu.Text = "&Free translation of the whole story to the clipboard";
            this.editCopyFreeTranslationMenu.ToolTipText = "Copy all of the lines of text in the Free translation into one big paragraph of t" +
    "ext";
            this.editCopyFreeTranslationMenu.Click += new EventHandler(this.copyFreeTranslationMenuItem_Click);
            // 
            // editPasteMenu
            // 
            this.editPasteMenu.Name = "editPasteMenu";
            this.editPasteMenu.Size = new Size(269, 22);
            this.editPasteMenu.Text = "&Paste";
            this.editPasteMenu.ToolTipText = "Paste the contents of the clipboard into the currently selected text box";
            this.editPasteMenu.Click += new EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // editDeleteToolStripMenu
            // 
            this.editDeleteToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.editDeleteStoryLinesMenu,
            this.editDeleteNationalBtMenu,
            this.editDeleteEnglishBtMenu,
            this.editDeleteFreeTranslationMenu,
            this.editDeleteTestToolStripMenu});
            this.editDeleteToolStripMenu.Name = "editDeleteToolStripMenu";
            this.editDeleteToolStripMenu.Size = new Size(269, 22);
            this.editDeleteToolStripMenu.Text = "&Delete";
            // 
            // editDeleteStoryLinesMenu
            // 
            this.editDeleteStoryLinesMenu.Name = "editDeleteStoryLinesMenu";
            this.editDeleteStoryLinesMenu.Size = new Size(310, 22);
            this.editDeleteStoryLinesMenu.Text = "&Story (all lines)";
            this.editDeleteStoryLinesMenu.ToolTipText = "Delete the contents of all of the text boxes of the story in the story language (" +
    "the lines will remain, but just be emptied)";
            this.editDeleteStoryLinesMenu.Click += new EventHandler(this.deleteStoryVersesToolStripMenuItem_Click);
            // 
            // editDeleteNationalBtMenu
            // 
            this.editDeleteNationalBtMenu.Name = "editDeleteNationalBtMenu";
            this.editDeleteNationalBtMenu.Size = new Size(310, 22);
            this.editDeleteNationalBtMenu.Text = "&National language back-translation (all lines)";
            this.editDeleteNationalBtMenu.ToolTipText = "Delete the contents of all of the text boxes of the national back-translation of " +
    "the story (the lines will remain, but just be emptied)";
            this.editDeleteNationalBtMenu.Click += new EventHandler(this.deleteStoryNationalBackTranslationToolStripMenuItem_Click);
            // 
            // editDeleteEnglishBtMenu
            // 
            this.editDeleteEnglishBtMenu.Name = "editDeleteEnglishBtMenu";
            this.editDeleteEnglishBtMenu.Size = new Size(310, 22);
            this.editDeleteEnglishBtMenu.Text = "&English back-translation (all lines)";
            this.editDeleteEnglishBtMenu.ToolTipText = "Delete the contents of all of the text boxes of the English back-translation of t" +
    "he story (the lines will remain, but just be emptied)";
            this.editDeleteEnglishBtMenu.Click += new EventHandler(this.deleteEnglishBacktranslationToolStripMenuItem_Click);
            // 
            // editDeleteFreeTranslationMenu
            // 
            this.editDeleteFreeTranslationMenu.Name = "editDeleteFreeTranslationMenu";
            this.editDeleteFreeTranslationMenu.Size = new Size(310, 22);
            this.editDeleteFreeTranslationMenu.Text = "&Free translation (all lines)";
            this.editDeleteFreeTranslationMenu.ToolTipText = "Delete the contents of all of the text boxes of the Free translation of the story" +
    " (the lines will remain, but just be emptied)";
            this.editDeleteFreeTranslationMenu.Click += new EventHandler(this.deleteFreeTranslationToolStripMenuItem_Click);
            // 
            // editDeleteTestToolStripMenu
            // 
            this.editDeleteTestToolStripMenu.Name = "editDeleteTestToolStripMenu";
            this.editDeleteTestToolStripMenu.Size = new Size(310, 22);
            this.editDeleteTestToolStripMenu.Text = "&Test";
            this.editDeleteTestToolStripMenu.ToolTipText = "Delete the answers to the testing questions and the retellings associated with a " +
    "particular testing helper (UNS). The text boxes will be deleted completely";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new Size(266, 6);
            // 
            // editFindMenu
            // 
            this.editFindMenu.Enabled = false;
            this.editFindMenu.Name = "editFindMenu";
            this.editFindMenu.ShortcutKeys = ((Keys)((Keys.Control | Keys.F)));
            this.editFindMenu.Size = new Size(269, 22);
            this.editFindMenu.Text = "&Find";
            this.editFindMenu.Click += new EventHandler(this.editFindToolStripMenuItem_Click);
            // 
            // editFindNextMenu
            // 
            this.editFindNextMenu.Enabled = false;
            this.editFindNextMenu.Name = "editFindNextMenu";
            this.editFindNextMenu.ShortcutKeys = Keys.F3;
            this.editFindNextMenu.Size = new Size(269, 22);
            this.editFindNextMenu.Text = "Find &Next";
            this.editFindNextMenu.Click += new EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // editReplaceMenu
            // 
            this.editReplaceMenu.Enabled = false;
            this.editReplaceMenu.Name = "editReplaceMenu";
            this.editReplaceMenu.ShortcutKeys = ((Keys)((Keys.Control | Keys.H)));
            this.editReplaceMenu.Size = new Size(269, 22);
            this.editReplaceMenu.Text = "&Replace";
            this.editReplaceMenu.Click += new EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new Size(266, 6);
            // 
            // editAddRetellingTestResultsMenu
            // 
            this.editAddRetellingTestResultsMenu.Name = "editAddRetellingTestResultsMenu";
            this.editAddRetellingTestResultsMenu.Size = new Size(269, 22);
            this.editAddRetellingTestResultsMenu.Text = "&Add retelling test boxes";
            this.editAddRetellingTestResultsMenu.ToolTipText = "Click here to add boxes for the retellings of the story";
            this.editAddRetellingTestResultsMenu.Click += new EventHandler(this.editAddTestResultsToolStripMenuItem_Click);
            // 
            // editAddInferenceTestResultsMenu
            // 
            this.editAddInferenceTestResultsMenu.Name = "editAddInferenceTestResultsMenu";
            this.editAddInferenceTestResultsMenu.Size = new Size(269, 22);
            this.editAddInferenceTestResultsMenu.Text = "Add &story test question answer boxes";
            this.editAddInferenceTestResultsMenu.ToolTipText = "Click here to add boxes for the answers to the testing questions";
            this.editAddInferenceTestResultsMenu.Click += new EventHandler(this.editAddInferenceTestResultsToolStripMenuItem_Click);
            // 
            // editAddGeneralTestQuestionMenu
            // 
            this.editAddGeneralTestQuestionMenu.Name = "editAddGeneralTestQuestionMenu";
            this.editAddGeneralTestQuestionMenu.Size = new Size(269, 22);
            this.editAddGeneralTestQuestionMenu.Text = "Add &general test question";
            this.editAddGeneralTestQuestionMenu.Click += new EventHandler(this.addgeneralTestQuestionToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
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
            this.viewToolStripMenuItem.Size = new Size(44, 27);
            this.viewToolStripMenuItem.Text = "&View";
            this.viewToolStripMenuItem.DropDownOpening += new EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // viewNonBiblicalStoriesMenu
            // 
            this.viewNonBiblicalStoriesMenu.CheckOnClick = true;
            this.viewNonBiblicalStoriesMenu.Name = "viewNonBiblicalStoriesMenu";
            this.viewNonBiblicalStoriesMenu.Size = new Size(284, 22);
            this.viewNonBiblicalStoriesMenu.Text = "&Non-biblical Stories...";
            this.viewNonBiblicalStoriesMenu.ToolTipText = "Check this menu to edit the set of non-biblical stories (uncheck for biblical sto" +
    "ries)";
            this.viewNonBiblicalStoriesMenu.CheckedChanged += new EventHandler(this.ViewNonBiblicalStoriesMenuCheckedChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new Size(281, 6);
            // 
            // viewShowHideFieldsMenu
            // 
            this.viewShowHideFieldsMenu.Name = "viewShowHideFieldsMenu";
            this.viewShowHideFieldsMenu.Size = new Size(284, 22);
            this.viewShowHideFieldsMenu.Text = "&Show/Hide multiple fields at once...";
            this.viewShowHideFieldsMenu.Click += new EventHandler(this.showHideFieldsToolStripMenuItem_Click);
            // 
            // viewUseSameSettingsForAllStoriesMenu
            // 
            this.viewUseSameSettingsForAllStoriesMenu.CheckOnClick = true;
            this.viewUseSameSettingsForAllStoriesMenu.Name = "viewUseSameSettingsForAllStoriesMenu";
            this.viewUseSameSettingsForAllStoriesMenu.Size = new Size(284, 22);
            this.viewUseSameSettingsForAllStoriesMenu.Text = "&Use same settings for all stories";
            this.viewUseSameSettingsForAllStoriesMenu.Click += new EventHandler(this.useSameSettingsForAllStoriesToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new Size(281, 6);
            // 
            // viewVernacularLangMenu
            // 
            this.viewVernacularLangMenu.Checked = true;
            this.viewVernacularLangMenu.CheckOnClick = true;
            this.viewVernacularLangMenu.CheckState = CheckState.Checked;
            this.viewVernacularLangMenu.Name = "viewVernacularLangMenu";
            this.viewVernacularLangMenu.Size = new Size(284, 22);
            this.viewVernacularLangMenu.Text = "Story &Language field";
            this.viewVernacularLangMenu.ToolTipText = "Show the text boxes for the story lines in the story language";
            this.viewVernacularLangMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewNationalLangMenu
            // 
            this.viewNationalLangMenu.Checked = true;
            this.viewNationalLangMenu.CheckOnClick = true;
            this.viewNationalLangMenu.CheckState = CheckState.Checked;
            this.viewNationalLangMenu.Name = "viewNationalLangMenu";
            this.viewNationalLangMenu.Size = new Size(284, 22);
            this.viewNationalLangMenu.Text = "National language &back translation field";
            this.viewNationalLangMenu.ToolTipText = "Show the text boxes for the national language back-translation of the story lines" +
    "";
            this.viewNationalLangMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewEnglishBtMenu
            // 
            this.viewEnglishBtMenu.Checked = true;
            this.viewEnglishBtMenu.CheckOnClick = true;
            this.viewEnglishBtMenu.CheckState = CheckState.Checked;
            this.viewEnglishBtMenu.Name = "viewEnglishBtMenu";
            this.viewEnglishBtMenu.Size = new Size(284, 22);
            this.viewEnglishBtMenu.Text = "&English back translation fields";
            this.viewEnglishBtMenu.ToolTipText = "Show the text boxes for the English language back-translation of the story lines";
            this.viewEnglishBtMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewFreeTranslationMenu
            // 
            this.viewFreeTranslationMenu.Checked = true;
            this.viewFreeTranslationMenu.CheckOnClick = true;
            this.viewFreeTranslationMenu.CheckState = CheckState.Checked;
            this.viewFreeTranslationMenu.Name = "viewFreeTranslationMenu";
            this.viewFreeTranslationMenu.Size = new Size(284, 22);
            this.viewFreeTranslationMenu.Text = "&Free Translation";
            this.viewFreeTranslationMenu.ToolTipText = "Show the text boxes for the Free Translation of the story lines";
            this.viewFreeTranslationMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewAnchorsMenu
            // 
            this.viewAnchorsMenu.Checked = true;
            this.viewAnchorsMenu.CheckOnClick = true;
            this.viewAnchorsMenu.CheckState = CheckState.Checked;
            this.viewAnchorsMenu.Name = "viewAnchorsMenu";
            this.viewAnchorsMenu.Size = new Size(284, 22);
            this.viewAnchorsMenu.Text = "&Anchors";
            this.viewAnchorsMenu.ToolTipText = "Show the Anchor toolbar";
            this.viewAnchorsMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewExegeticalHelps
            // 
            this.viewExegeticalHelps.Checked = true;
            this.viewExegeticalHelps.CheckOnClick = true;
            this.viewExegeticalHelps.CheckState = CheckState.Checked;
            this.viewExegeticalHelps.Name = "viewExegeticalHelps";
            this.viewExegeticalHelps.Size = new Size(284, 22);
            this.viewExegeticalHelps.Text = "&Exegetical/Cultural notes";
            this.viewExegeticalHelps.ToolTipText = "Show the Exegetical/Cultural notes (\'cn\') fields";
            this.viewExegeticalHelps.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewGeneralTestingsQuestionMenu
            // 
            this.viewGeneralTestingsQuestionMenu.CheckOnClick = true;
            this.viewGeneralTestingsQuestionMenu.Name = "viewGeneralTestingsQuestionMenu";
            this.viewGeneralTestingsQuestionMenu.Size = new Size(284, 22);
            this.viewGeneralTestingsQuestionMenu.Text = "&General testing questions";
            this.viewGeneralTestingsQuestionMenu.ToolTipText = "Show the text boxes for the general testing questions";
            this.viewGeneralTestingsQuestionMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionsMenu
            // 
            this.viewStoryTestingQuestionsMenu.Checked = true;
            this.viewStoryTestingQuestionsMenu.CheckOnClick = true;
            this.viewStoryTestingQuestionsMenu.CheckState = CheckState.Checked;
            this.viewStoryTestingQuestionsMenu.Name = "viewStoryTestingQuestionsMenu";
            this.viewStoryTestingQuestionsMenu.Size = new Size(284, 22);
            this.viewStoryTestingQuestionsMenu.Text = "Story &testing questions";
            this.viewStoryTestingQuestionsMenu.ToolTipText = "Show the text boxes for the story testing questions";
            this.viewStoryTestingQuestionsMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewStoryTestingQuestionAnswersMenu
            // 
            this.viewStoryTestingQuestionAnswersMenu.Checked = true;
            this.viewStoryTestingQuestionAnswersMenu.CheckOnClick = true;
            this.viewStoryTestingQuestionAnswersMenu.CheckState = CheckState.Checked;
            this.viewStoryTestingQuestionAnswersMenu.Name = "viewStoryTestingQuestionAnswersMenu";
            this.viewStoryTestingQuestionAnswersMenu.Size = new Size(284, 22);
            this.viewStoryTestingQuestionAnswersMenu.Text = "Ans&wers";
            this.viewStoryTestingQuestionAnswersMenu.ToolTipText = "Show the text boxes for the UNS\'s answers to testing questions";
            this.viewStoryTestingQuestionAnswersMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // viewRetellingsMenu
            // 
            this.viewRetellingsMenu.Checked = true;
            this.viewRetellingsMenu.CheckOnClick = true;
            this.viewRetellingsMenu.CheckState = CheckState.Checked;
            this.viewRetellingsMenu.Name = "viewRetellingsMenu";
            this.viewRetellingsMenu.Size = new Size(284, 22);
            this.viewRetellingsMenu.Text = "&Retellings";
            this.viewRetellingsMenu.ToolTipText = "Show the text boxes for the UNS retelling responses";
            this.viewRetellingsMenu.CheckedChanged += new EventHandler(this.viewFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new Size(281, 6);
            // 
            // viewConsultantNotesMenu
            // 
            this.viewConsultantNotesMenu.Checked = true;
            this.viewConsultantNotesMenu.CheckOnClick = true;
            this.viewConsultantNotesMenu.CheckState = CheckState.Checked;
            this.viewConsultantNotesMenu.Name = "viewConsultantNotesMenu";
            this.viewConsultantNotesMenu.Size = new Size(284, 22);
            this.viewConsultantNotesMenu.Text = "&Consultant notes";
            this.viewConsultantNotesMenu.ToolTipText = "Show the Consultant Notes pane";
            this.viewConsultantNotesMenu.CheckedChanged += new EventHandler(this.viewConsultantNoteFieldMenuItem_CheckedChanged);
            // 
            // viewCoachNotesMenu
            // 
            this.viewCoachNotesMenu.Checked = true;
            this.viewCoachNotesMenu.CheckOnClick = true;
            this.viewCoachNotesMenu.CheckState = CheckState.Checked;
            this.viewCoachNotesMenu.Name = "viewCoachNotesMenu";
            this.viewCoachNotesMenu.Size = new Size(284, 22);
            this.viewCoachNotesMenu.Text = "Coach &notes";
            this.viewCoachNotesMenu.ToolTipText = "Show the Coach Notes pane";
            this.viewCoachNotesMenu.CheckedChanged += new EventHandler(this.viewCoachNotesFieldMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new Size(281, 6);
            // 
            // viewBibleMenu
            // 
            this.viewBibleMenu.Checked = true;
            this.viewBibleMenu.CheckOnClick = true;
            this.viewBibleMenu.CheckState = CheckState.Checked;
            this.viewBibleMenu.Name = "viewBibleMenu";
            this.viewBibleMenu.Size = new Size(284, 22);
            this.viewBibleMenu.Text = "&Bible viewer";
            this.viewBibleMenu.ToolTipText = "Show the Bible Viewer pane";
            this.viewBibleMenu.CheckedChanged += new EventHandler(this.viewNetBibleMenuItem_CheckedChanged);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new Size(281, 6);
            // 
            // viewRefreshMenu
            // 
            this.viewRefreshMenu.Enabled = false;
            this.viewRefreshMenu.Name = "viewRefreshMenu";
            this.viewRefreshMenu.ShortcutKeys = Keys.F5;
            this.viewRefreshMenu.Size = new Size(284, 22);
            this.viewRefreshMenu.Text = "Re&fresh";
            this.viewRefreshMenu.ToolTipText = "Refresh the screen (if it doesn\'t look like it updated something properly)";
            this.viewRefreshMenu.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new Size(281, 6);
            // 
            // viewHistoricalDifferencesMenu
            // 
            this.viewHistoricalDifferencesMenu.Name = "viewHistoricalDifferencesMenu";
            this.viewHistoricalDifferencesMenu.Size = new Size(284, 22);
            this.viewHistoricalDifferencesMenu.Text = "Historical di&fferences...";
            this.viewHistoricalDifferencesMenu.ToolTipText = "Click to launch the Revision History dialog to compare different, saved versions " +
    "of this story";
            this.viewHistoricalDifferencesMenu.Click += new EventHandler(this.historicalDifferencesToolStripMenuItem_Click);
            // 
            // viewLnCNotesMenu
            // 
            this.viewLnCNotesMenu.Name = "viewLnCNotesMenu";
            this.viewLnCNotesMenu.Size = new Size(284, 22);
            this.viewLnCNotesMenu.Text = "L && C Notes...";
            this.viewLnCNotesMenu.Click += new EventHandler(this.viewLnCNotesMenu_Click);
            // 
            // viewConcordanceMenu
            // 
            this.viewConcordanceMenu.Name = "viewConcordanceMenu";
            this.viewConcordanceMenu.Size = new Size(284, 22);
            this.viewConcordanceMenu.Text = "Concor&dance...";
            this.viewConcordanceMenu.ToolTipText = "Click to launch the Concordance dialog to search for the occurrences of words in " +
    "the story";
            this.viewConcordanceMenu.Click += new EventHandler(this.concordanceToolStripMenuItem_Click);
            // 
            // viewStateTransitionHistoryMenu
            // 
            this.viewStateTransitionHistoryMenu.Name = "viewStateTransitionHistoryMenu";
            this.viewStateTransitionHistoryMenu.Size = new Size(284, 22);
            this.viewStateTransitionHistoryMenu.Text = "&Turn Transition History...";
            this.viewStateTransitionHistoryMenu.ToolTipText = "Click here to view information about when the story was in different turns and wh" +
    "ose turn it was";
            this.viewStateTransitionHistoryMenu.Click += new EventHandler(this.stateTransitionHistoryToolStripMenuItem_Click);
            // 
            // viewProjectNotesMenu
            // 
            this.viewProjectNotesMenu.Name = "viewProjectNotesMenu";
            this.viewProjectNotesMenu.Size = new Size(284, 22);
            this.viewProjectNotesMenu.Text = "&Project Notes...";
            this.viewProjectNotesMenu.Click += new EventHandler(this.projectNotesToolStripMenuItem_Click);
            // 
            // viewOldStoriesMenu
            // 
            this.viewOldStoriesMenu.Name = "viewOldStoriesMenu";
            this.viewOldStoriesMenu.Size = new Size(284, 22);
            this.viewOldStoriesMenu.Text = "&Old Stories...";
            this.viewOldStoriesMenu.ToolTipText = "View older (obsolete) versions of the stories (that were earlier stored in the \'O" +
    "ld Stories\' list from the \'Panorama View\' window--see \'Panorama\' menu, \'Show\' co" +
    "mmand)";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new Size(281, 6);
            // 
            // viewHiddenVersesMenu
            // 
            this.viewHiddenVersesMenu.CheckOnClick = true;
            this.viewHiddenVersesMenu.Name = "viewHiddenVersesMenu";
            this.viewHiddenVersesMenu.Size = new Size(284, 22);
            this.viewHiddenVersesMenu.Text = "H&idden lines";
            this.viewHiddenVersesMenu.ToolTipText = "Check this menu to show hidden lines and hidden consultant note comments";
            this.viewHiddenVersesMenu.CheckStateChanged += new EventHandler(this.hiddenVersesToolStripMenuItem_CheckStateChanged);
            // 
            // viewOnlyOpenConversationsMenu
            // 
            this.viewOnlyOpenConversationsMenu.CheckOnClick = true;
            this.viewOnlyOpenConversationsMenu.Name = "viewOnlyOpenConversationsMenu";
            this.viewOnlyOpenConversationsMenu.Size = new Size(284, 22);
            this.viewOnlyOpenConversationsMenu.Text = "Onl&y open conversations";
            this.viewOnlyOpenConversationsMenu.ToolTipText = "Check this menu to hide all closed conversations (i.e. whose \"End Conversation\" b" +
    "utton has been clicked)";
            this.viewOnlyOpenConversationsMenu.CheckStateChanged += new EventHandler(this.viewOnlyOpenConversationsMenu_CheckStateChanged);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new Size(281, 6);
            // 
            // viewTransliterationsToolStripMenu
            // 
            this.viewTransliterationsToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewTransliterationVernacular,
            this.viewTransliterationNationalBT,
            this.viewTransliterationInternationalBt,
            this.viewTransliterationFreeTranslation});
            this.viewTransliterationsToolStripMenu.Name = "viewTransliterationsToolStripMenu";
            this.viewTransliterationsToolStripMenu.Size = new Size(284, 22);
            this.viewTransliterationsToolStripMenu.Text = "&Transliterations";
            this.viewTransliterationsToolStripMenu.DropDownOpening += new EventHandler(this.viewTransliterationsToolStripMenuItem_DropDownOpening);
            // 
            // viewTransliterationVernacular
            // 
            this.viewTransliterationVernacular.CheckOnClick = true;
            this.viewTransliterationVernacular.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewTransliteratorVernacularConfigureToolStripMenuItem});
            this.viewTransliterationVernacular.Name = "viewTransliterationVernacular";
            this.viewTransliterationVernacular.Size = new Size(158, 22);
            this.viewTransliterationVernacular.Text = "Story Language";
            this.viewTransliterationVernacular.ToolTipText = "Check this menu to turn on a transliterator for the story language boxes";
            this.viewTransliterationVernacular.Click += new EventHandler(this.viewTransliterationVernacular_Click);
            // 
            // viewTransliteratorVernacularConfigureToolStripMenuItem
            // 
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Name = "viewTransliteratorVernacularConfigureToolStripMenuItem";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Size = new Size(136, 22);
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorVernacularConfigureToolStripMenuItem.Click += new EventHandler(this.viewTransliteratorVernacularConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationNationalBT
            // 
            this.viewTransliterationNationalBT.CheckOnClick = true;
            this.viewTransliterationNationalBT.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem});
            this.viewTransliterationNationalBT.Name = "viewTransliterationNationalBT";
            this.viewTransliterationNationalBT.Size = new Size(158, 22);
            this.viewTransliterationNationalBT.Text = "National BT";
            this.viewTransliterationNationalBT.ToolTipText = "Check this menu to turn on a transliterator for the national language BT boxes";
            this.viewTransliterationNationalBT.Click += new EventHandler(this.viewTransliterationNationalBT_Click);
            // 
            // viewTransliteratorNationalBTConfigureToolStripMenuItem
            // 
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Name = "viewTransliteratorNationalBTConfigureToolStripMenuItem";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Size = new Size(136, 22);
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorNationalBTConfigureToolStripMenuItem.Click += new EventHandler(this.viewTransliteratorNationalBTConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationInternationalBt
            // 
            this.viewTransliterationInternationalBt.CheckOnClick = true;
            this.viewTransliterationInternationalBt.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem});
            this.viewTransliterationInternationalBt.Name = "viewTransliterationInternationalBt";
            this.viewTransliterationInternationalBt.Size = new Size(158, 22);
            this.viewTransliterationInternationalBt.Text = "International BT";
            this.viewTransliterationInternationalBt.ToolTipText = "Check this menu to turn on a transliterator for the International/English languag" +
    "e boxes";
            this.viewTransliterationInternationalBt.Click += new EventHandler(this.viewTransliterationInternationalBt_Click);
            // 
            // viewTransliteratorInternationalBtConfigureToolStripMenuItem
            // 
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Name = "viewTransliteratorInternationalBtConfigureToolStripMenuItem";
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Size = new Size(136, 22);
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorInternationalBtConfigureToolStripMenuItem.Click += new EventHandler(this.viewTransliteratorInternationalBtConfigureToolStripMenuItem_Click);
            // 
            // viewTransliterationFreeTranslation
            // 
            this.viewTransliterationFreeTranslation.CheckOnClick = true;
            this.viewTransliterationFreeTranslation.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem});
            this.viewTransliterationFreeTranslation.Name = "viewTransliterationFreeTranslation";
            this.viewTransliterationFreeTranslation.Size = new Size(158, 22);
            this.viewTransliterationFreeTranslation.Text = "Free Translation";
            this.viewTransliterationFreeTranslation.ToolTipText = "Check this menu to turn on a transliterator for the Free Translation language box" +
    "es";
            this.viewTransliterationFreeTranslation.Click += new EventHandler(this.viewTransliterationFreeTranslation_Click);
            // 
            // viewTransliteratorFreeTranslationConfigureToolStripMenuItem
            // 
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Name = "viewTransliteratorFreeTranslationConfigureToolStripMenuItem";
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Size = new Size(136, 22);
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Text = "&Configure...";
            this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem.Click += new EventHandler(this.viewTransliteratorFreeTranslationConfigureToolStripMenuItem_Click);
            // 
            // comboBoxStorySelector
            // 
            this.comboBoxStorySelector.Alignment = ToolStripItemAlignment.Right;
            this.comboBoxStorySelector.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.comboBoxStorySelector.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.comboBoxStorySelector.Font = new Font("Arial Unicode MS", 10F);
            this.comboBoxStorySelector.MaxDropDownItems = 30;
            this.comboBoxStorySelector.Name = "comboBoxStorySelector";
            this.comboBoxStorySelector.Size = new Size(290, 27);
            this.comboBoxStorySelector.Text = "<to create a story, type its name here and hit Enter>";
            this.comboBoxStorySelector.ToolTipText = "Select the Story to edit or type in a new name to add a new story";
            this.comboBoxStorySelector.SelectedIndexChanged += new EventHandler(this.LoadStory);
            this.comboBoxStorySelector.KeyUp += new KeyEventHandler(this.comboBoxStorySelector_KeyUp);
            // 
            // storyToolStripMenu
            // 
            this.storyToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.storyStoryInformationMenu,
            this.storyDeleteStoryMenu,
            this.storyCopyWithNewNameMenu,
            this.storySplitIntoLinesMenu,
            this.storyRealignStoryLinesMenu,
            this.storyOverrideTasksMenu,
            this.toolStripSeparator14,
            this.storyUseAdaptItForBackTranslationMenu});
            this.storyToolStripMenu.Name = "storyToolStripMenu";
            this.storyToolStripMenu.Size = new Size(46, 27);
            this.storyToolStripMenu.Text = "&Story";
            this.storyToolStripMenu.DropDownOpening += new EventHandler(this.storyToolStripMenuItem_DropDownOpening);
            // 
            // storyStoryInformationMenu
            // 
            this.storyStoryInformationMenu.Name = "storyStoryInformationMenu";
            this.storyStoryInformationMenu.Size = new Size(245, 22);
            this.storyStoryInformationMenu.Text = "S&tory Information...";
            this.storyStoryInformationMenu.ToolTipText = "Enter information about this story, such as the reason it\'s in the set, the resou" +
    "rces used, etc.";
            this.storyStoryInformationMenu.Click += new EventHandler(this.enterTheReasonThisStoryIsInTheSetToolStripMenuItem_Click);
            // 
            // storyDeleteStoryMenu
            // 
            this.storyDeleteStoryMenu.Name = "storyDeleteStoryMenu";
            this.storyDeleteStoryMenu.Size = new Size(245, 22);
            this.storyDeleteStoryMenu.Text = "&Delete story";
            this.storyDeleteStoryMenu.ToolTipText = "Click to delete the current story";
            this.storyDeleteStoryMenu.Click += new EventHandler(this.deleteStoryToolStripMenuItem_Click);
            // 
            // storyCopyWithNewNameMenu
            // 
            this.storyCopyWithNewNameMenu.Name = "storyCopyWithNewNameMenu";
            this.storyCopyWithNewNameMenu.Size = new Size(245, 22);
            this.storyCopyWithNewNameMenu.Text = "&Copy with new name";
            this.storyCopyWithNewNameMenu.ToolTipText = "Click to make a duplicate copy of the current story with a new name";
            this.storyCopyWithNewNameMenu.Click += new EventHandler(this.storyCopyWithNewNameToolStripMenuItem_Click);
            // 
            // storySplitIntoLinesMenu
            // 
            this.storySplitIntoLinesMenu.Name = "storySplitIntoLinesMenu";
            this.storySplitIntoLinesMenu.Size = new Size(245, 22);
            this.storySplitIntoLinesMenu.Text = "S&plit into Lines";
            this.storySplitIntoLinesMenu.ToolTipText = "Click to split a paragraph of text into lines based on sentence final punctuation" +
    " (alternates with \'Collapse into 1 line\' menu)";
            this.storySplitIntoLinesMenu.Click += new EventHandler(this.splitIntoLinesToolStripMenuItem_Click);
            // 
            // storyRealignStoryLinesMenu
            // 
            this.storyRealignStoryLinesMenu.Name = "storyRealignStoryLinesMenu";
            this.storyRealignStoryLinesMenu.ShortcutKeys = ((Keys)((Keys.Control | Keys.F5)));
            this.storyRealignStoryLinesMenu.Size = new Size(245, 22);
            this.storyRealignStoryLinesMenu.Text = "&Re-align story lines";
            this.storyRealignStoryLinesMenu.ToolTipText = "Click to collapse the lines into a paragraph of text followed by \"Split into line" +
    "s\"";
            this.storyRealignStoryLinesMenu.Click += new EventHandler(this.realignStoryVersesToolStripMenuItem_Click);
            // 
            // storyOverrideTasksMenu
            // 
            this.storyOverrideTasksMenu.Name = "storyOverrideTasksMenu";
            this.storyOverrideTasksMenu.Size = new Size(245, 22);
            this.storyOverrideTasksMenu.Text = "&Override Tasks...";
            this.storyOverrideTasksMenu.ToolTipText = resources.GetString("storyOverrideTasksMenu.ToolTipText");
            this.storyOverrideTasksMenu.Click += new EventHandler(this.storyOverrideTasks_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new Size(242, 6);
            // 
            // storyUseAdaptItForBackTranslationMenu
            // 
            this.storyUseAdaptItForBackTranslationMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.storyAdaptItVernacularToNationalMenu,
            this.storyAdaptItVernacularToEnglishMenu,
            this.storyAdaptItNationalToEnglishMenu,
            this.toolStripSeparator15,
            this.storySynchronizeSharedAdaptItProjectsMenu});
            this.storyUseAdaptItForBackTranslationMenu.Name = "storyUseAdaptItForBackTranslationMenu";
            this.storyUseAdaptItForBackTranslationMenu.Size = new Size(245, 22);
            this.storyUseAdaptItForBackTranslationMenu.Text = "&Use Adapt It for back-translation";
            // 
            // storyAdaptItVernacularToNationalMenu
            // 
            this.storyAdaptItVernacularToNationalMenu.Name = "storyAdaptItVernacularToNationalMenu";
            this.storyAdaptItVernacularToNationalMenu.Size = new Size(267, 22);
            this.storyAdaptItVernacularToNationalMenu.Text = "&Story language to National language";
            this.storyAdaptItVernacularToNationalMenu.Click += new EventHandler(this.storyAdaptItVernacularToNationalMenuItem_Click);
            // 
            // storyAdaptItVernacularToEnglishMenu
            // 
            this.storyAdaptItVernacularToEnglishMenu.Name = "storyAdaptItVernacularToEnglishMenu";
            this.storyAdaptItVernacularToEnglishMenu.Size = new Size(267, 22);
            this.storyAdaptItVernacularToEnglishMenu.Text = "Story &language to English";
            this.storyAdaptItVernacularToEnglishMenu.Click += new EventHandler(this.storyAdaptItVernacularToEnglishMenuItem_Click);
            // 
            // storyAdaptItNationalToEnglishMenu
            // 
            this.storyAdaptItNationalToEnglishMenu.Name = "storyAdaptItNationalToEnglishMenu";
            this.storyAdaptItNationalToEnglishMenu.Size = new Size(267, 22);
            this.storyAdaptItNationalToEnglishMenu.Text = "National language to &English";
            this.storyAdaptItNationalToEnglishMenu.Click += new EventHandler(this.storyAdaptItNationalToEnglishMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new Size(264, 6);
            // 
            // storySynchronizeSharedAdaptItProjectsMenu
            // 
            this.storySynchronizeSharedAdaptItProjectsMenu.Name = "storySynchronizeSharedAdaptItProjectsMenu";
            this.storySynchronizeSharedAdaptItProjectsMenu.Size = new Size(267, 22);
            this.storySynchronizeSharedAdaptItProjectsMenu.Text = "Synchronize Shared Adapt It &projects";
            this.storySynchronizeSharedAdaptItProjectsMenu.ToolTipText = "Click to Send/Receive the shared Adapt It repository";
            this.storySynchronizeSharedAdaptItProjectsMenu.Click += new EventHandler(this.synchronizeSharedAdaptItProjectsToolStripMenuItem_Click);
            // 
            // panoramaToolStripMenu
            // 
            this.panoramaToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.panoramaShowMenu,
            this.panoramaInsertNewStoryMenu,
            this.panoramaAddNewStoryAfterMenu});
            this.panoramaToolStripMenu.Name = "panoramaToolStripMenu";
            this.panoramaToolStripMenu.Size = new Size(73, 27);
            this.panoramaToolStripMenu.Text = "Pa&norama";
            this.panoramaToolStripMenu.DropDownOpening += new EventHandler(this.panoramaToolStripMenuItem_DropDownOpening);
            // 
            // panoramaShowMenu
            // 
            this.panoramaShowMenu.Name = "panoramaShowMenu";
            this.panoramaShowMenu.Size = new Size(235, 22);
            this.panoramaShowMenu.Text = "&Show...";
            this.panoramaShowMenu.ToolTipText = "Show the Panorama View window to see all the stories in the set and their current" +
    " state";
            this.panoramaShowMenu.Click += new EventHandler(this.toolStripMenuItemShowPanorama_Click);
            // 
            // panoramaInsertNewStoryMenu
            // 
            this.panoramaInsertNewStoryMenu.Name = "panoramaInsertNewStoryMenu";
            this.panoramaInsertNewStoryMenu.Size = new Size(235, 22);
            this.panoramaInsertNewStoryMenu.Text = "&Insert new story before current";
            this.panoramaInsertNewStoryMenu.ToolTipText = "Click to insert a new, empty story before the one currently shown";
            this.panoramaInsertNewStoryMenu.Click += new EventHandler(this.insertNewStoryToolStripMenuItem_Click);
            // 
            // panoramaAddNewStoryAfterMenu
            // 
            this.panoramaAddNewStoryAfterMenu.Name = "panoramaAddNewStoryAfterMenu";
            this.panoramaAddNewStoryAfterMenu.Size = new Size(235, 22);
            this.panoramaAddNewStoryAfterMenu.Text = "&Add new story after current";
            this.panoramaAddNewStoryAfterMenu.ToolTipText = "Click to add a new, empty story after the one currently shown";
            this.panoramaAddNewStoryAfterMenu.Click += new EventHandler(this.addNewStoryAfterToolStripMenuItem_Click);
            // 
            // advancedToolStripMenu
            // 
            this.advancedToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.advancedProgramUpdatesToolStripMenu,
            this.advancedLocalizationMenu,
            this.advancedOverrideLocalizeStateViewSettingsMenu,
            this.advancedNewProjectMenu,
            this.advancedChangeStateWithoutChecksMenu,
            this.advancedSaveTimeoutToolStripMenu,
            this.advancedResetStoredInformationMenu,
            this.advancedChangeProjectFolderRootMenu,
            this.advancedEmailMenu,
            this.advancedUseOldStyleStoryBtPaneMenu,
            this.advancedUseWordBreaks,
            this.advancedImportHelper});
            this.advancedToolStripMenu.Name = "advancedToolStripMenu";
            this.advancedToolStripMenu.Size = new Size(72, 27);
            this.advancedToolStripMenu.Text = "A&dvanced";
            this.advancedToolStripMenu.DropDownOpening += new EventHandler(this.advancedToolStripMenuItem_DropDownOpening);
            // 
            // advancedProgramUpdatesToolStripMenu
            // 
            this.advancedProgramUpdatesToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu,
            this.advancedProgramUpdatesCheckNowMenu,
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu});
            this.advancedProgramUpdatesToolStripMenu.Name = "advancedProgramUpdatesToolStripMenu";
            this.advancedProgramUpdatesToolStripMenu.Size = new Size(314, 22);
            this.advancedProgramUpdatesToolStripMenu.Text = "Program &Updates";
            this.advancedProgramUpdatesToolStripMenu.DropDownOpening += new EventHandler(this.programUpdatesToolStripMenuItem_DropDownOpening);
            // 
            // advancedProgramUpdatesAutomaticallyCheckAtStartupMenu
            // 
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.CheckOnClick = true;
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Name = "advancedProgramUpdatesAutomaticallyCheckAtStartupMenu";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Size = new Size(250, 22);
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.Text = "&Automatically check at startup";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.ToolTipText = "Uncheck this menu to stop the program from automatically checking for program upd" +
    "ates when the program is started (this can save startup time)";
            this.advancedProgramUpdatesAutomaticallyCheckAtStartupMenu.CheckStateChanged += new EventHandler(this.automaticallyCheckAtStartupToolStripMenuItem_CheckStateChanged);
            // 
            // advancedProgramUpdatesCheckNowMenu
            // 
            this.advancedProgramUpdatesCheckNowMenu.Name = "advancedProgramUpdatesCheckNowMenu";
            this.advancedProgramUpdatesCheckNowMenu.Size = new Size(250, 22);
            this.advancedProgramUpdatesCheckNowMenu.Text = "&Check now";
            this.advancedProgramUpdatesCheckNowMenu.ToolTipText = "Click this menu to have the program manually check for program updates";
            this.advancedProgramUpdatesCheckNowMenu.Click += new EventHandler(this.checkForProgramUpdatesNowToolStripMenuItem_Click);
            // 
            // advancedProgramUpdatesCheckNowForNextMajorUpdateMenu
            // 
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Name = "advancedProgramUpdatesCheckNowForNextMajorUpdateMenu";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Size = new Size(250, 22);
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Text = "Check now for next &major update";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.ToolTipText = "Click this menu to have the program check if the next major update is available (" +
    "which wouldn\'t otherwise be installed by default)";
            this.advancedProgramUpdatesCheckNowForNextMajorUpdateMenu.Click += new EventHandler(this.checkNowForNextMajorUpdateToolStripMenuItem_Click);
            // 
            // advancedLocalizationMenu
            // 
            this.advancedLocalizationMenu.Name = "advancedLocalizationMenu";
            this.advancedLocalizationMenu.Size = new Size(314, 22);
            this.advancedLocalizationMenu.Text = "&Localization...";
            this.advancedLocalizationMenu.ToolTipText = "Click this to change or translate the user interface language";
            this.advancedLocalizationMenu.Click += new EventHandler(this.localizationToolStripMenuItem_Click);
            // 
            // advancedOverrideLocalizeStateViewSettingsMenu
            // 
            this.advancedOverrideLocalizeStateViewSettingsMenu.Name = "advancedOverrideLocalizeStateViewSettingsMenu";
            this.advancedOverrideLocalizeStateViewSettingsMenu.Size = new Size(314, 22);
            this.advancedOverrideLocalizeStateViewSettingsMenu.Text = "&Override/Localize turn field viewing settings...";
            this.advancedOverrideLocalizeStateViewSettingsMenu.ToolTipText = "Click to see the \'turn table\' in which you can override which fields are displaye" +
    "d by default and localize the status bar message and instructions for the variou" +
    "s turns";
            this.advancedOverrideLocalizeStateViewSettingsMenu.Click += new EventHandler(this.advancedOverrideLocalizeStateViewSettingsMenu_Click);
            // 
            // advancedNewProjectMenu
            // 
            this.advancedNewProjectMenu.Name = "advancedNewProjectMenu";
            this.advancedNewProjectMenu.ShowShortcutKeys = false;
            this.advancedNewProjectMenu.Size = new Size(314, 22);
            this.advancedNewProjectMenu.Text = "&New Project...";
            this.advancedNewProjectMenu.ToolTipText = "Click to create a new OneStory project (not a new story)";
            this.advancedNewProjectMenu.Click += new EventHandler(this.newToolStripMenuItem_Click);
            // 
            // advancedChangeStateWithoutChecksMenu
            // 
            this.advancedChangeStateWithoutChecksMenu.Name = "advancedChangeStateWithoutChecksMenu";
            this.advancedChangeStateWithoutChecksMenu.Size = new Size(314, 22);
            this.advancedChangeStateWithoutChecksMenu.Text = "&Change Turn without checks...";
            this.advancedChangeStateWithoutChecksMenu.ToolTipText = resources.GetString("advancedChangeStateWithoutChecksMenu.ToolTipText");
            this.advancedChangeStateWithoutChecksMenu.Click += new EventHandler(this.changeStateWithoutChecksToolStripMenuItem_Click);
            // 
            // advancedSaveTimeoutToolStripMenu
            // 
            this.advancedSaveTimeoutToolStripMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.advancedSaveTimeoutEnabledMenu,
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu});
            this.advancedSaveTimeoutToolStripMenu.Name = "advancedSaveTimeoutToolStripMenu";
            this.advancedSaveTimeoutToolStripMenu.Size = new Size(314, 22);
            this.advancedSaveTimeoutToolStripMenu.Text = "&Automatic saving";
            // 
            // advancedSaveTimeoutEnabledMenu
            // 
            this.advancedSaveTimeoutEnabledMenu.CheckOnClick = true;
            this.advancedSaveTimeoutEnabledMenu.Name = "advancedSaveTimeoutEnabledMenu";
            this.advancedSaveTimeoutEnabledMenu.Size = new Size(269, 22);
            this.advancedSaveTimeoutEnabledMenu.Text = "&Enabled";
            this.advancedSaveTimeoutEnabledMenu.ToolTipText = "This menu enables a 5 minute timeout to remind you to save (disable at your own r" +
    "isk)";
            this.advancedSaveTimeoutEnabledMenu.CheckStateChanged += new EventHandler(this.enabledToolStripMenuItem_CheckStateChanged);
            // 
            // advancedSaveTimeoutAsSilentlyAsPossibleMenu
            // 
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.CheckOnClick = true;
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Name = "advancedSaveTimeoutAsSilentlyAsPossibleMenu";
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Size = new Size(269, 22);
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.Text = "&Automatically save without reminder";
            this.advancedSaveTimeoutAsSilentlyAsPossibleMenu.ToolTipText = "This menu indicates whether the program will query you (unchecked) or not (checke" +
    "d) to save the project file";
            // 
            // advancedResetStoredInformationMenu
            // 
            this.advancedResetStoredInformationMenu.Name = "advancedResetStoredInformationMenu";
            this.advancedResetStoredInformationMenu.Size = new Size(314, 22);
            this.advancedResetStoredInformationMenu.Text = "&Reset Stored Information";
            this.advancedResetStoredInformationMenu.ToolTipText = resources.GetString("advancedResetStoredInformationMenu.ToolTipText");
            this.advancedResetStoredInformationMenu.Click += new EventHandler(this.resetStoredInformationToolStripMenuItem_Click);
            // 
            // advancedChangeProjectFolderRootMenu
            // 
            this.advancedChangeProjectFolderRootMenu.Name = "advancedChangeProjectFolderRootMenu";
            this.advancedChangeProjectFolderRootMenu.Size = new Size(314, 22);
            this.advancedChangeProjectFolderRootMenu.Text = "Change &Project Folder Root";
            this.advancedChangeProjectFolderRootMenu.ToolTipText = "Click this to use a different location for the root folder (i.e. \"OneStory Editor" +
    " Projects\") besides in your \"My Documents\" folder";
            this.advancedChangeProjectFolderRootMenu.Click += new EventHandler(this.changeProjectFolderRootToolStripMenuItem_Click);
            // 
            // advancedEmailMenu
            // 
            this.advancedEmailMenu.Checked = true;
            this.advancedEmailMenu.CheckOnClick = true;
            this.advancedEmailMenu.CheckState = CheckState.Checked;
            this.advancedEmailMenu.Name = "advancedEmailMenu";
            this.advancedEmailMenu.Size = new Size(314, 22);
            this.advancedEmailMenu.Text = "&Email via MAPI+";
            this.advancedEmailMenu.ToolTipText = resources.GetString("advancedEmailMenu.ToolTipText");
            this.advancedEmailMenu.Click += new EventHandler(this.advancedEmailMenu_Click);
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
            // advancedUseWordBreaks
            // 
            this.advancedUseWordBreaks.CheckOnClick = true;
            this.advancedUseWordBreaks.Name = "advancedUseWordBreaks";
            this.advancedUseWordBreaks.Size = new Size(314, 22);
            this.advancedUseWordBreaks.Text = "Use automatic word &breaking when Glossing";
            // 
            // aboutToolStripMenu
            // 
            this.aboutToolStripMenu.Name = "aboutToolStripMenu";
            this.aboutToolStripMenu.Size = new Size(52, 27);
            this.aboutToolStripMenu.Text = "&About";
            this.aboutToolStripMenu.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.splitContainerLeftRight.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainerLeftRight.Dock = DockStyle.Fill;
            this.splitContainerLeftRight.Location = new Point(0, 31);
            this.splitContainerLeftRight.Name = "splitContainerLeftRight";
            // 
            // splitContainerLeftRight.Panel1
            // 
            this.splitContainerLeftRight.Panel1.Controls.Add(this.splitContainerUpDown);
            this.splitContainerLeftRight.Panel1.SizeChanged += new EventHandler(this.splitContainerLeftRight_Panel1_SizeChanged);
            // 
            // splitContainerLeftRight.Panel2
            // 
            this.splitContainerLeftRight.Panel2.Controls.Add(this.splitContainerMentorNotes);
            this.splitContainerLeftRight.Panel2.SizeChanged += new EventHandler(this.splitContainerLeftRight_Panel2_SizeChanged);
            this.splitContainerLeftRight.Size = new Size(881, 613);
            this.splitContainerLeftRight.SplitterDistance = 453;
            this.splitContainerLeftRight.TabIndex = 2;
            // 
            // splitContainerUpDown
            // 
            this.splitContainerUpDown.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainerUpDown.Dock = DockStyle.Fill;
            this.splitContainerUpDown.Location = new Point(0, 0);
            this.splitContainerUpDown.Name = "splitContainerUpDown";
            this.splitContainerUpDown.Orientation = Orientation.Horizontal;
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
            this.splitContainerUpDown.Size = new Size(453, 613);
            this.splitContainerUpDown.SplitterDistance = 391;
            this.splitContainerUpDown.TabIndex = 2;
            // 
            // linkLabelTasks
            // 
            this.linkLabelTasks.AutoSize = true;
            this.linkLabelTasks.Location = new Point(63, 5);
            this.linkLabelTasks.Name = "linkLabelTasks";
            this.linkLabelTasks.Size = new Size(36, 13);
            this.linkLabelTasks.TabIndex = 4;
            this.linkLabelTasks.TabStop = true;
            this.linkLabelTasks.Tag = 1;
            this.linkLabelTasks.Text = "Tasks";
            this.linkLabelTasks.Visible = false;
            this.linkLabelTasks.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelTasks_LinkClicked);
            // 
            // linkLabelVerseBT
            // 
            this.linkLabelVerseBT.AutoSize = true;
            this.linkLabelVerseBT.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelVerseBT, "Click here to jump to the indicated line number. You can also right-click on this" +
        " to get a list of all lines to jump to.");
            this.linkLabelVerseBT.Location = new Point(11, 5);
            this.linkLabelVerseBT.Name = "linkLabelVerseBT";
            this.helpProvider.SetShowHelp(this.linkLabelVerseBT, true);
            this.linkLabelVerseBT.Size = new Size(31, 13);
            this.linkLabelVerseBT.TabIndex = 4;
            this.linkLabelVerseBT.TabStop = true;
            this.linkLabelVerseBT.Tag = 1;
            this.linkLabelVerseBT.Text = "Ln: 1";
            this.linkLabelVerseBT.Visible = false;
            this.linkLabelVerseBT.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelVerseBT_LinkClicked);
            // 
            // contextMenuStripVerseList
            // 
            this.contextMenuStripVerseList.Name = "contextMenuStripVerseList";
            this.contextMenuStripVerseList.Size = new Size(61, 4);
            this.contextMenuStripVerseList.Opening += new CancelEventHandler(this.contextMenuStripVerseList_Opening);
            // 
            // flowLayoutPanelVerses
            // 
            this.flowLayoutPanelVerses.AutoScroll = true;
            this.flowLayoutPanelVerses.Dock = DockStyle.Fill;
            this.flowLayoutPanelVerses.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanelVerses.LastControlIntoView = null;
            this.flowLayoutPanelVerses.LineNumberLink = null;
            this.flowLayoutPanelVerses.Location = new Point(0, 23);
            this.flowLayoutPanelVerses.Name = "flowLayoutPanelVerses";
            this.flowLayoutPanelVerses.Size = new Size(451, 366);
            this.flowLayoutPanelVerses.TabIndex = 1;
            this.flowLayoutPanelVerses.WrapContents = false;
            this.flowLayoutPanelVerses.MouseMove += new MouseEventHandler(this.CheckBiblePaneCursorPositionMouseMove);
            // 
            // htmlStoryBtControl
            // 
            this.htmlStoryBtControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControl.IsWebBrowserContextMenuEnabled = false;
            this.htmlStoryBtControl.Location = new System.Drawing.Point(0, 23);
            this.htmlStoryBtControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControl.Name = "htmlStoryBtControl";
            this.htmlStoryBtControl.ParentStory = null;
            this.htmlStoryBtControl.Size = new System.Drawing.Size(451, 366);
            this.htmlStoryBtControl.StoryData = null;
            this.htmlStoryBtControl.TabIndex = 5;
            this.htmlStoryBtControl.TheSE = null;
            this.htmlStoryBtControl.ViewSettings = null;
            // 
            // textBoxStoryVerse
            // 
            this.textBoxStoryVerse.BorderStyle = BorderStyle.FixedSingle;
            this.textBoxStoryVerse.Dock = DockStyle.Top;
            this.textBoxStoryVerse.Enabled = false;
            this.textBoxStoryVerse.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStoryVerse.Location = new Point(0, 0);
            this.textBoxStoryVerse.Name = "textBoxStoryVerse";
            this.textBoxStoryVerse.ReadOnly = true;
            this.textBoxStoryVerse.Size = new Size(451, 23);
            this.textBoxStoryVerse.TabIndex = 3;
            this.textBoxStoryVerse.TabStop = false;
            this.textBoxStoryVerse.Text = "Story/BT";
            this.textBoxStoryVerse.TextAlign = HorizontalAlignment.Center;
            // 
            // netBibleViewer
            // 
            this.netBibleViewer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.netBibleViewer.Dock = DockStyle.Fill;
            this.netBibleViewer.JumpTarget = null;
            this.netBibleViewer.Location = new Point(0, 0);
            this.netBibleViewer.Margin = new Padding(0);
            this.netBibleViewer.Name = "netBibleViewer";
            this.netBibleViewer.ScriptureReference = "Gen 1:1";
            this.netBibleViewer.Size = new Size(451, 216);
            this.netBibleViewer.TabIndex = 0;
            // 
            // splitContainerMentorNotes
            // 
            this.splitContainerMentorNotes.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainerMentorNotes.Dock = DockStyle.Fill;
            this.splitContainerMentorNotes.Location = new Point(0, 0);
            this.splitContainerMentorNotes.Name = "splitContainerMentorNotes";
            this.splitContainerMentorNotes.Orientation = Orientation.Horizontal;
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
            this.splitContainerMentorNotes.Size = new Size(424, 613);
            this.splitContainerMentorNotes.SplitterDistance = 356;
            this.splitContainerMentorNotes.TabIndex = 0;
            // 
            // linkLabelConsultantNotes
            // 
            this.linkLabelConsultantNotes.AutoSize = true;
            this.linkLabelConsultantNotes.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelConsultantNotes, "Click here to jump to the indicated line number. You can also right-click on this" +
        " to get a list of all lines to jump to.");
            this.linkLabelConsultantNotes.Location = new Point(11, 5);
            this.linkLabelConsultantNotes.Name = "linkLabelConsultantNotes";
            this.helpProvider.SetShowHelp(this.linkLabelConsultantNotes, true);
            this.linkLabelConsultantNotes.Size = new Size(64, 13);
            this.linkLabelConsultantNotes.TabIndex = 3;
            this.linkLabelConsultantNotes.TabStop = true;
            this.linkLabelConsultantNotes.Tag = 0;
            this.linkLabelConsultantNotes.Text = "Story (Ln: 0)";
            this.linkLabelConsultantNotes.Visible = false;
            this.linkLabelConsultantNotes.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelConsultantNotes_LinkClicked);
            // 
            // htmlConsultantNotesControl
            // 
            this.htmlConsultantNotesControl.AllowWebBrowserDrop = false;
            this.htmlConsultantNotesControl.Dock = DockStyle.Fill;
            this.htmlConsultantNotesControl.Location = new Point(0, 23);
            this.htmlConsultantNotesControl.MinimumSize = new Size(20, 20);
            this.htmlConsultantNotesControl.Name = "htmlConsultantNotesControl";
            this.htmlConsultantNotesControl.Size = new Size(422, 331);
            this.htmlConsultantNotesControl.StoryData = null;
            this.htmlConsultantNotesControl.TabIndex = 2;
            this.htmlConsultantNotesControl.TheSE = null;
            // 
            // textBoxConsultantNotesTable
            // 
            this.textBoxConsultantNotesTable.BorderStyle = BorderStyle.FixedSingle;
            this.textBoxConsultantNotesTable.Dock = DockStyle.Top;
            this.textBoxConsultantNotesTable.Enabled = false;
            this.textBoxConsultantNotesTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConsultantNotesTable.Location = new Point(0, 0);
            this.textBoxConsultantNotesTable.Name = "textBoxConsultantNotesTable";
            this.textBoxConsultantNotesTable.ReadOnly = true;
            this.textBoxConsultantNotesTable.Size = new Size(422, 23);
            this.textBoxConsultantNotesTable.TabIndex = 1;
            this.textBoxConsultantNotesTable.TabStop = false;
            this.textBoxConsultantNotesTable.Text = "Consultant Notes";
            this.textBoxConsultantNotesTable.TextAlign = HorizontalAlignment.Center;
            // 
            // linkLabelCoachNotes
            // 
            this.linkLabelCoachNotes.AutoSize = true;
            this.linkLabelCoachNotes.ContextMenuStrip = this.contextMenuStripVerseList;
            this.helpProvider.SetHelpString(this.linkLabelCoachNotes, "Click here to jump to the indicated line number. You can also right-click on this" +
        " to get a list of all lines to jump to.");
            this.linkLabelCoachNotes.Location = new Point(11, 5);
            this.linkLabelCoachNotes.Name = "linkLabelCoachNotes";
            this.helpProvider.SetShowHelp(this.linkLabelCoachNotes, true);
            this.linkLabelCoachNotes.Size = new Size(64, 13);
            this.linkLabelCoachNotes.TabIndex = 4;
            this.linkLabelCoachNotes.TabStop = true;
            this.linkLabelCoachNotes.Tag = 0;
            this.linkLabelCoachNotes.Text = "Story (Ln: 0)";
            this.linkLabelCoachNotes.Visible = false;
            this.linkLabelCoachNotes.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelCoachNotes_LinkClicked);
            // 
            // htmlCoachNotesControl
            // 
            this.htmlCoachNotesControl.AllowWebBrowserDrop = false;
            this.htmlCoachNotesControl.Dock = DockStyle.Fill;
            this.htmlCoachNotesControl.Location = new Point(0, 23);
            this.htmlCoachNotesControl.MinimumSize = new Size(20, 20);
            this.htmlCoachNotesControl.Name = "htmlCoachNotesControl";
            this.htmlCoachNotesControl.Size = new Size(422, 228);
            this.htmlCoachNotesControl.StoryData = null;
            this.htmlCoachNotesControl.TabIndex = 3;
            this.htmlCoachNotesControl.TheSE = null;
            // 
            // textBoxCoachNotes
            // 
            this.textBoxCoachNotes.BorderStyle = BorderStyle.FixedSingle;
            this.textBoxCoachNotes.Dock = DockStyle.Top;
            this.textBoxCoachNotes.Enabled = false;
            this.textBoxCoachNotes.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCoachNotes.Location = new Point(0, 0);
            this.textBoxCoachNotes.Name = "textBoxCoachNotes";
            this.textBoxCoachNotes.ReadOnly = true;
            this.textBoxCoachNotes.Size = new Size(422, 23);
            this.textBoxCoachNotes.TabIndex = 2;
            this.textBoxCoachNotes.TabStop = false;
            this.textBoxCoachNotes.Text = "Coach Notes";
            this.textBoxCoachNotes.TextAlign = HorizontalAlignment.Center;
            // 
            // toolStripRecordNavigation
            // 
            this.toolStripRecordNavigation.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.toolStripRecordNavigation.Dock = DockStyle.None;
            this.toolStripRecordNavigation.GripStyle = ToolStripGripStyle.Hidden;
            this.toolStripRecordNavigation.Items.AddRange(new ToolStripItem[] {
            this.toolStripButtonShowPanoramaStories,
            this.toolStripButtonFirst,
            this.toolStripButtonPrevious,
            this.toolStripButtonNext,
            this.toolStripButtonLast});
            this.toolStripRecordNavigation.Location = new Point(471, 0);
            this.toolStripRecordNavigation.Name = "toolStripRecordNavigation";
            this.toolStripRecordNavigation.Size = new Size(118, 25);
            this.toolStripRecordNavigation.TabIndex = 3;
            this.toolStripRecordNavigation.Text = "<no need to localize/translate>";
            // 
            // toolStripButtonShowPanoramaStories
            // 
            this.toolStripButtonShowPanoramaStories.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowPanoramaStories.Image = Resources.ShowAllCommentsHS;
            this.toolStripButtonShowPanoramaStories.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonShowPanoramaStories.Name = "toolStripButtonShowPanoramaStories";
            this.toolStripButtonShowPanoramaStories.Size = new Size(23, 22);
            this.toolStripButtonShowPanoramaStories.Text = "Show Panorama Stories";
            this.toolStripButtonShowPanoramaStories.ToolTipText = "Click to view the list of full stories (same as \"Panorama\", \"Show\")";
            this.toolStripButtonShowPanoramaStories.Click += new EventHandler(this.toolStripMenuItemShowPanorama_Click);
            // 
            // toolStripButtonFirst
            // 
            this.toolStripButtonFirst.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFirst.Image = Resources.DataContainer_MoveFirstHS;
            this.toolStripButtonFirst.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonFirst.Name = "toolStripButtonFirst";
            this.toolStripButtonFirst.Size = new Size(23, 22);
            this.toolStripButtonFirst.Text = "First Story";
            this.toolStripButtonFirst.ToolTipText = "Click to go to the first story (hold down the Ctrl key and click to keep the same" +
    " fields visible)";
            this.toolStripButtonFirst.Click += new EventHandler(this.toolStripButtonFirst_Click);
            // 
            // toolStripButtonPrevious
            // 
            this.toolStripButtonPrevious.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevious.Image = Resources.DataContainer_MovePreviousHS;
            this.toolStripButtonPrevious.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonPrevious.Name = "toolStripButtonPrevious";
            this.toolStripButtonPrevious.Size = new Size(23, 22);
            this.toolStripButtonPrevious.Text = "Previous Story";
            this.toolStripButtonPrevious.ToolTipText = "Click to go to the previous story (hold down the Ctrl key and click to keep the s" +
    "ame fields visible)";
            this.toolStripButtonPrevious.Click += new EventHandler(this.toolStripButtonPrevious_Click);
            // 
            // toolStripButtonNext
            // 
            this.toolStripButtonNext.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNext.Image = Resources.DataContainer_MoveNextHS;
            this.toolStripButtonNext.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonNext.Name = "toolStripButtonNext";
            this.toolStripButtonNext.Size = new Size(23, 22);
            this.toolStripButtonNext.Text = "Next Story";
            this.toolStripButtonNext.ToolTipText = "Click to go to the next story (hold down the Ctrl key and click to keep the same " +
    "fields visible)";
            this.toolStripButtonNext.Click += new EventHandler(this.toolStripButtonNext_Click);
            // 
            // toolStripButtonLast
            // 
            this.toolStripButtonLast.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLast.Image = Resources.DataContainer_MoveLastHS;
            this.toolStripButtonLast.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonLast.Name = "toolStripButtonLast";
            this.toolStripButtonLast.Size = new Size(23, 22);
            this.toolStripButtonLast.Text = "Last Story";
            this.toolStripButtonLast.ToolTipText = "Click to go to the last story (hold down the Ctrl key and click to keep the same " +
    "fields visible)";
            this.toolStripButtonLast.Click += new EventHandler(this.toolStripButtonLast_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            this.statusLabel.BorderStyle = Border3DStyle.Sunken;
            this.statusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(866, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = ContentAlignment.MiddleRight;
            this.statusLabel.Click += new EventHandler(this.statusLabel_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new Point(0, 644);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(881, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "<no need to localize/translate>";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // advancedImportHelper
            // 
            this.advancedImportHelper.Name = "advancedImportHelper";
            this.advancedImportHelper.Size = new Size(314, 22);
            this.advancedImportHelper.Text = "&Import helper (text paster)";
            this.advancedImportHelper.Click += new EventHandler(this.advancedImportHelper_Click);
            // 
            // StoryEditor
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(881, 666);
            this.Controls.Add(this.toolStripRecordNavigation);
            this.Controls.Add(this.splitContainerLeftRight);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "StoryEditor";
            this.Text = "OneStory Editor";
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += new FormClosingEventHandler(this.StoryEditor_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerLeftRight.Panel1.ResumeLayout(false);
            this.splitContainerLeftRight.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainerLeftRight)).EndInit();
            this.splitContainerLeftRight.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.ResumeLayout(false);
            this.splitContainerUpDown.Panel1.PerformLayout();
            this.splitContainerUpDown.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainerUpDown)).EndInit();
            this.splitContainerUpDown.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel1.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel1.PerformLayout();
            this.splitContainerMentorNotes.Panel2.ResumeLayout(false);
            this.splitContainerMentorNotes.Panel2.PerformLayout();
            ((ISupportInitialize)(this.splitContainerMentorNotes)).EndInit();
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
        private BackgroundWorker backgroundWorker;
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
        internal ToolStripMenuItem advancedUseWordBreaks;
        private ToolStripMenuItem advancedUseOldStyleStoryBtPaneMenu;
        private ToolStripMenuItem advancedImportHelper;
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

