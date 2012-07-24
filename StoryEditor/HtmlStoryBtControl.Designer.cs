using System;

namespace OneStoryProjectEditor
{
    partial class HtmlStoryBtControl
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripLineOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveSelectedTextToANewLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddTestQuestion = new System.Windows.Forms.ToolStripMenuItem();
            this.addExegeticalCulturalNoteBelowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveLineUp = new System.Windows.Forms.ToolStripMenuItem();
            this.moveLineDown = new System.Windows.Forms.ToolStripMenuItem();
            this.addANewVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewVersesAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.hideVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTheWholeVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyVerseToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitStoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripAnchorOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertNullAnchorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripLineOptions.SuspendLayout();
            this.contextMenuStripAnchorOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripLineOptions
            // 
            this.contextMenuStripLineOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveSelectedTextToANewLineToolStripMenuItem,
            this.moveItemsToolStripMenuItem,
            this.deleteItemsToolStripMenuItem,
            this.menuAddTestQuestion,
            this.addExegeticalCulturalNoteBelowToolStripMenuItem,
            this.toolStripSeparator2,
            this.moveLineUp,
            this.moveLineDown,
            this.addANewVerseToolStripMenuItem,
            this.addNewVersesAfterToolStripMenuItem,
            this.hideVerseToolStripMenuItem,
            this.deleteTheWholeVerseToolStripMenuItem,
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem,
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem,
            this.copyVerseToClipboardToolStripMenuItem,
            this.toolStripSeparator1,
            this.splitStoryToolStripMenuItem});
            this.contextMenuStripLineOptions.Name = "contextMenuStrip";
            this.contextMenuStripLineOptions.Size = new System.Drawing.Size(418, 376);
            this.contextMenuStripLineOptions.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripLineOptions_Opening);
            // 
            // moveSelectedTextToANewLineToolStripMenuItem
            // 
            this.moveSelectedTextToANewLineToolStripMenuItem.Name = "moveSelectedTextToANewLineToolStripMenuItem";
            this.moveSelectedTextToANewLineToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.moveSelectedTextToANewLineToolStripMenuItem.Text = "Move &selected text to a new line";
            this.moveSelectedTextToANewLineToolStripMenuItem.ToolTipText = "This will take the selected text from the Story language, the National BT and the" +
    " English BT boxes and move them into a new line following this line";
            this.moveSelectedTextToANewLineToolStripMenuItem.Click += new System.EventHandler(this.MoveSelectedTextToANewLineToolStripMenuItemClick);
            // 
            // moveItemsToolStripMenuItem
            // 
            this.moveItemsToolStripMenuItem.Name = "moveItemsToolStripMenuItem";
            this.moveItemsToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.moveItemsToolStripMenuItem.Text = "&Move items";
            this.moveItemsToolStripMenuItem.ToolTipText = "Click this menu to move testing questions, anchors, exegetical notes, etc to anot" +
    "her line";
            this.moveItemsToolStripMenuItem.Click += new System.EventHandler(this.MoveItemsToolStripMenuItemClick);
            // 
            // deleteItemsToolStripMenuItem
            // 
            this.deleteItemsToolStripMenuItem.Name = "deleteItemsToolStripMenuItem";
            this.deleteItemsToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.deleteItemsToolStripMenuItem.Text = "&Delete items";
            this.deleteItemsToolStripMenuItem.ToolTipText = "Click this menu to delete testing questions, anchors, exegetical notes, etc";
            this.deleteItemsToolStripMenuItem.Click += new System.EventHandler(this.DeleteItemsToolStripMenuItemClick);
            // 
            // menuAddTestQuestion
            // 
            this.menuAddTestQuestion.Name = "menuAddTestQuestion";
            this.menuAddTestQuestion.Size = new System.Drawing.Size(417, 24);
            this.menuAddTestQuestion.Text = "Add a story testing &question";
            this.menuAddTestQuestion.ToolTipText = "Click to add a story testing question";
            this.menuAddTestQuestion.Click += new System.EventHandler(this.MenuAddTestQuestionClick);
            // 
            // addExegeticalCulturalNoteBelowToolStripMenuItem
            // 
            this.addExegeticalCulturalNoteBelowToolStripMenuItem.Name = "addExegeticalCulturalNoteBelowToolStripMenuItem";
            this.addExegeticalCulturalNoteBelowToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.addExegeticalCulturalNoteBelowToolStripMenuItem.Text = "Add &Exegetical/Cultural Note below";
            this.addExegeticalCulturalNoteBelowToolStripMenuItem.Click += new System.EventHandler(this.addExegeticalCulturalNoteBelowToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(414, 6);
            // 
            // moveLineUp
            // 
            this.moveLineUp.Name = "moveLineUp";
            this.moveLineUp.Size = new System.Drawing.Size(417, 24);
            this.moveLineUp.Text = "Move Line Up";
            this.moveLineUp.ToolTipText = "Click this menu to move the line up (i.e. switch places with the line above)";
            this.moveLineUp.Click += new System.EventHandler(this.MoveLineUp);
            // 
            // moveLineDown
            // 
            this.moveLineDown.Name = "moveLineDown";
            this.moveLineDown.Size = new System.Drawing.Size(417, 24);
            this.moveLineDown.Text = "Move Line Down";
            this.moveLineDown.ToolTipText = "Click this menu to move the line down (i.e. switch places with the line below)";
            this.moveLineDown.Click += new System.EventHandler(this.MoveLineDown);
            // 
            // addANewVerseToolStripMenuItem
            // 
            this.addANewVerseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8});
            this.addANewVerseToolStripMenuItem.Name = "addANewVerseToolStripMenuItem";
            this.addANewVerseToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.addANewVerseToolStripMenuItem.Text = "Insert new &line(s) before";
            this.addANewVerseToolStripMenuItem.ToolTipText = "This line will move down and the new line(s) will be inserted before";
            this.addANewVerseToolStripMenuItem.Click += new System.EventHandler(this.addANewVerseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem2.Text = "1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem3.Text = "2";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem4.Text = "3";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem5.Text = "5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem6.Text = "10";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem7.Text = "20";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem8.Text = "30";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.addNewVersesBeforeMenuItem_Click);
            // 
            // addNewVersesAfterToolStripMenuItem
            // 
            this.addNewVersesAfterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13,
            this.toolStripMenuItem14,
            this.toolStripMenuItem15});
            this.addNewVersesAfterToolStripMenuItem.Name = "addNewVersesAfterToolStripMenuItem";
            this.addNewVersesAfterToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.addNewVersesAfterToolStripMenuItem.Text = "&Add new line(s) after";
            this.addNewVersesAfterToolStripMenuItem.ToolTipText = "The new line(s) will be added after this line";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem9.Text = "1";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem10.Text = "2";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem11.Text = "3";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem12.Text = "5";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem13.Text = "10";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem14.Text = "20";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(94, 24);
            this.toolStripMenuItem15.Text = "30";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.addNewVersesAfterMenuItem_Click);
            // 
            // hideVerseToolStripMenuItem
            // 
            this.hideVerseToolStripMenuItem.Name = "hideVerseToolStripMenuItem";
            this.hideVerseToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.hideVerseToolStripMenuItem.Text = "&Hide line";
            this.hideVerseToolStripMenuItem.ToolTipText = "Hide this line (use \'View\', \'Show Hidden\' to see them later)";
            this.hideVerseToolStripMenuItem.Click += new System.EventHandler(this.hideVerseToolStripMenuItem_Click);
            // 
            // deleteTheWholeVerseToolStripMenuItem
            // 
            this.deleteTheWholeVerseToolStripMenuItem.Name = "deleteTheWholeVerseToolStripMenuItem";
            this.deleteTheWholeVerseToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.deleteTheWholeVerseToolStripMenuItem.Text = "&Delete line";
            this.deleteTheWholeVerseToolStripMenuItem.ToolTipText = "Delete this line";
            this.deleteTheWholeVerseToolStripMenuItem.Click += new System.EventHandler(this.DeleteTheWholeVerseToolStripMenuItemClick);
            // 
            // pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem
            // 
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Name = "pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Text = "&Paste line from clipboard and insert before this line";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.ToolTipText = "Use this to paste a previously copied line (see \'Copy line to clipboard\' command)" +
    ". The copied line will be inserted before this line (see \'Paste line from clipbo" +
    "ard\' commands)";
            this.pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem.Click += new System.EventHandler(this.pasteVerseFromClipboardToolStripMenuItem_Click);
            // 
            // pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem
            // 
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Name = "pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Text = "Paste line from clipboard and insert af&ter this line";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.ToolTipText = "Use this to paste a previously copied line (see \'Copy line to clipboard\' command)" +
    ". The copied line will be inserted after this line";
            this.pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem.Click += new System.EventHandler(this.pasteVerseFromClipboardAfterThisOneToolStripMenuItem_Click);
            // 
            // copyVerseToClipboardToolStripMenuItem
            // 
            this.copyVerseToClipboardToolStripMenuItem.Name = "copyVerseToClipboardToolStripMenuItem";
            this.copyVerseToClipboardToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.copyVerseToClipboardToolStripMenuItem.Text = "&Copy line to clipboard";
            this.copyVerseToClipboardToolStripMenuItem.ToolTipText = "Use this to copy a line to be pasted in another location";
            this.copyVerseToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyVerseToClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(414, 6);
            // 
            // splitStoryToolStripMenuItem
            // 
            this.splitStoryToolStripMenuItem.Name = "splitStoryToolStripMenuItem";
            this.splitStoryToolStripMenuItem.Size = new System.Drawing.Size(417, 24);
            this.splitStoryToolStripMenuItem.Text = "Split st&ory";
            this.splitStoryToolStripMenuItem.ToolTipText = "Move this and all following lines to a new story inserted immediately after this " +
    "story";
            this.splitStoryToolStripMenuItem.Click += new System.EventHandler(this.splitStoryToolStripMenuItem_Click);
            // 
            // contextMenuStripAnchorOptions
            // 
            this.contextMenuStripAnchorOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.addCommentToolStripMenuItem,
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem,
            this.insertNullAnchorToolStripMenuItem});
            this.contextMenuStripAnchorOptions.Name = "contextMenuStripAnchorOptions";
            this.contextMenuStripAnchorOptions.Size = new System.Drawing.Size(307, 136);
            this.contextMenuStripAnchorOptions.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripAnchorOptionsOpening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(306, 22);
            this.deleteToolStripMenuItem.Text = "&Delete Anchor";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteAnchorToolStripMenuItemClick);
            // 
            // addCommentToolStripMenuItem
            // 
            this.addCommentToolStripMenuItem.Name = "addCommentToolStripMenuItem";
            this.addCommentToolStripMenuItem.Size = new System.Drawing.Size(306, 22);
            this.addCommentToolStripMenuItem.Text = "&Add Anchor Comment (becomes a tooltip)";
            this.addCommentToolStripMenuItem.Click += new System.EventHandler(this.AddCommentToolStripMenuItemClick);
            // 
            // addConsultantCoachNoteOnThisAnchorToolStripMenuItem
            // 
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem.Name = "addConsultantCoachNoteOnThisAnchorToolStripMenuItem";
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem.Size = new System.Drawing.Size(306, 22);
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem.Text = "Add &Consultant/Coach Note on this Anchor";
            this.addConsultantCoachNoteOnThisAnchorToolStripMenuItem.Click += new System.EventHandler(this.AddConsultantCoachNoteOnThisAnchorToolStripMenuItemClick);
            // 
            // insertNullAnchorToolStripMenuItem
            // 
            this.insertNullAnchorToolStripMenuItem.Name = "insertNullAnchorToolStripMenuItem";
            this.insertNullAnchorToolStripMenuItem.Size = new System.Drawing.Size(306, 22);
            this.insertNullAnchorToolStripMenuItem.Text = "Insert \"&Empty\" Anchor";
            this.insertNullAnchorToolStripMenuItem.ToolTipText = "Use this to add an empty anchor for lines of the story that don\'t really have a b" +
    "iblical anchor";
            this.insertNullAnchorToolStripMenuItem.Click += new System.EventHandler(this.InsertNullAnchorToolStripMenuItemClick);
            // 
            // HtmlStoryBtControl
            // 
            this.AllowWebBrowserDrop = false;
            this.contextMenuStripLineOptions.ResumeLayout(false);
            this.contextMenuStripAnchorOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripLineOptions;
        private System.Windows.Forms.ToolStripMenuItem moveSelectedTextToANewLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuAddTestQuestion;
        private System.Windows.Forms.ToolStripMenuItem addExegeticalCulturalNoteBelowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addANewVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem addNewVersesAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem hideVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTheWholeVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteVerseFromClipboardAndInsertBeforeThisVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteVerseFromClipboardAndInsertAfterThisVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyVerseToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem splitStoryToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAnchorOptions;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConsultantCoachNoteOnThisAnchorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertNullAnchorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveLineUp;
        private System.Windows.Forms.ToolStripMenuItem moveLineDown;

    }
}
