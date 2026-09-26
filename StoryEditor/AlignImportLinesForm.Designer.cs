namespace OneStoryProjectEditor
{
    partial class AlignImportLinesForm
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonJoinWithNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonJoinWithPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInsertBlankRow = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteBlankRow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelSource = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxSource = new System.Windows.Forms.ToolStripComboBox();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.dataGridViewAlign = new System.Windows.Forms.DataGridView();
            this.ColumnLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInclude = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.joinRowWithNextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.joinRowWithPreviousMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBlankRowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBlankRowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.joinCellWithNextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.joinCellWithPreviousMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBlankCellMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBlankCellMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.undoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAlign)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // toolStrip
            //
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonJoinWithNext,
            this.toolStripButtonJoinWithPrevious,
            this.toolStripButtonInsertBlankRow,
            this.toolStripButtonDeleteBlankRow,
            this.toolStripSeparator1,
            this.toolStripButtonUndo,
            this.toolStripSeparator2,
            this.toolStripLabelSource,
            this.toolStripComboBoxSource});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(984, 25);
            this.toolStrip.TabIndex = 0;
            //
            // toolStripButtonJoinWithNext
            //
            this.toolStripButtonJoinWithNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonJoinWithNext.Name = "toolStripButtonJoinWithNext";
            this.toolStripButtonJoinWithNext.Size = new System.Drawing.Size(88, 22);
            this.toolStripButtonJoinWithNext.Text = "&Join with next";
            this.toolStripButtonJoinWithNext.ToolTipText = "Join the imported lines of this row with the row below it (Ctrl+J)";
            this.toolStripButtonJoinWithNext.Click += new System.EventHandler(this.JoinRowWithNextClick);
            //
            // toolStripButtonJoinWithPrevious
            //
            this.toolStripButtonJoinWithPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonJoinWithPrevious.Name = "toolStripButtonJoinWithPrevious";
            this.toolStripButtonJoinWithPrevious.Size = new System.Drawing.Size(111, 22);
            this.toolStripButtonJoinWithPrevious.Text = "Join with &previous";
            this.toolStripButtonJoinWithPrevious.ToolTipText = "Join the imported lines of this row with the row above it (Ctrl+Shift+J)";
            this.toolStripButtonJoinWithPrevious.Click += new System.EventHandler(this.JoinRowWithPreviousClick);
            //
            // toolStripButtonInsertBlankRow
            //
            this.toolStripButtonInsertBlankRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonInsertBlankRow.Name = "toolStripButtonInsertBlankRow";
            this.toolStripButtonInsertBlankRow.Size = new System.Drawing.Size(100, 22);
            this.toolStripButtonInsertBlankRow.Text = "&Insert blank row";
            this.toolStripButtonInsertBlankRow.ToolTipText = "Insert a blank row above this one, e.g. for a story line that wasn't in the recording (Ctrl+Insert)";
            this.toolStripButtonInsertBlankRow.Click += new System.EventHandler(this.InsertBlankRowClick);
            //
            // toolStripButtonDeleteBlankRow
            //
            this.toolStripButtonDeleteBlankRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDeleteBlankRow.Name = "toolStripButtonDeleteBlankRow";
            this.toolStripButtonDeleteBlankRow.Size = new System.Drawing.Size(104, 22);
            this.toolStripButtonDeleteBlankRow.Text = "&Delete blank row";
            this.toolStripButtonDeleteBlankRow.ToolTipText = "Delete this row, if it has no imported text in it (Ctrl+Delete)";
            this.toolStripButtonDeleteBlankRow.Click += new System.EventHandler(this.DeleteBlankRowClick);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            // toolStripButtonUndo
            //
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(40, 22);
            this.toolStripButtonUndo.Text = "&Undo";
            this.toolStripButtonUndo.ToolTipText = "Undo the last change (Ctrl+Z)";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.UndoClick);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            //
            // toolStripLabelSource
            //
            this.toolStripLabelSource.Name = "toolStripLabelSource";
            this.toolStripLabelSource.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabelSource.Text = "Show story in:";
            //
            // toolStripComboBoxSource
            //
            this.toolStripComboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxSource.Name = "toolStripComboBoxSource";
            this.toolStripComboBoxSource.Size = new System.Drawing.Size(250, 25);
            this.toolStripComboBoxSource.ToolTipText = "Choose which language of the story (or test questions) to show on the left";
            this.toolStripComboBoxSource.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSourceSelectedIndexChanged);
            //
            // labelInstructions
            //
            this.labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInstructions.Location = new System.Drawing.Point(12, 30);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(960, 42);
            this.labelInstructions.TabIndex = 1;
            this.labelInstructions.Text = "The imported lines in each included row will go with the line on the left. To line them up, join rows (e.g. if one l" +
    "ine of the story was said in several lines), insert blank rows (e.g. for a line that was left out), or uncheck 'Include' for talk that isn't part of it. To split a line, click on it and press Enter where it should be split (do this for each tier); Delete at the end of a line (or Backspace at the start) joins it back.";
            //
            // dataGridViewAlign
            //
            this.dataGridViewAlign.AllowUserToAddRows = false;
            this.dataGridViewAlign.AllowUserToDeleteRows = false;
            this.dataGridViewAlign.AllowUserToResizeRows = false;
            this.dataGridViewAlign.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewAlign.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewAlign.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewAlign.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAlign.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnLine,
            this.ColumnSource,
            this.ColumnInclude});
            this.dataGridViewAlign.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridViewAlign.Location = new System.Drawing.Point(12, 75);
            this.dataGridViewAlign.MultiSelect = false;
            this.dataGridViewAlign.Name = "dataGridViewAlign";
            this.dataGridViewAlign.RowHeadersVisible = false;
            this.dataGridViewAlign.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewAlign.Size = new System.Drawing.Size(960, 438);
            this.dataGridViewAlign.TabIndex = 2;
            this.dataGridViewAlign.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGridViewAlignCellBeginEdit);
            this.dataGridViewAlign.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewAlignCellEndEdit);
            this.dataGridViewAlign.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewAlignCellMouseDown);
            this.dataGridViewAlign.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewAlignCellContentClick);
            this.dataGridViewAlign.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewAlignCellContentClick);
            this.dataGridViewAlign.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.DataGridViewAlignCellPainting);
            this.dataGridViewAlign.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridViewAlignDataError);
            //
            // ColumnLine
            //
            this.ColumnLine.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnLine.HeaderText = "Line";
            this.ColumnLine.Name = "ColumnLine";
            this.ColumnLine.ReadOnly = true;
            this.ColumnLine.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // ColumnSource
            //
            this.ColumnSource.FillWeight = 100F;
            this.ColumnSource.HeaderText = "Story";
            this.ColumnSource.Name = "ColumnSource";
            this.ColumnSource.ReadOnly = true;
            this.ColumnSource.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // ColumnInclude
            //
            this.ColumnInclude.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnInclude.HeaderText = "Include";
            this.ColumnInclude.Name = "ColumnInclude";
            this.ColumnInclude.ReadOnly = true;
            this.ColumnInclude.ToolTipText = "Uncheck this to leave the row out, e.g. for talk that isn't part of the story (Space)";
            //
            // contextMenuStrip
            //
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinRowWithNextMenu,
            this.joinRowWithPreviousMenu,
            this.insertBlankRowMenu,
            this.deleteBlankRowMenu,
            this.toolStripSeparator3,
            this.joinCellWithNextMenu,
            this.joinCellWithPreviousMenu,
            this.insertBlankCellMenu,
            this.deleteBlankCellMenu,
            this.toolStripSeparator4,
            this.undoMenu});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(360, 214);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripOpening);
            //
            // joinRowWithNextMenu
            //
            this.joinRowWithNextMenu.Name = "joinRowWithNextMenu";
            this.joinRowWithNextMenu.ShortcutKeyDisplayString = "Ctrl+J";
            this.joinRowWithNextMenu.Size = new System.Drawing.Size(289, 22);
            this.joinRowWithNextMenu.Text = "Join row with next";
            this.joinRowWithNextMenu.Click += new System.EventHandler(this.JoinRowWithNextClick);
            //
            // joinRowWithPreviousMenu
            //
            this.joinRowWithPreviousMenu.Name = "joinRowWithPreviousMenu";
            this.joinRowWithPreviousMenu.ShortcutKeyDisplayString = "Ctrl+Shift+J";
            this.joinRowWithPreviousMenu.Size = new System.Drawing.Size(289, 22);
            this.joinRowWithPreviousMenu.Text = "Join row with previous";
            this.joinRowWithPreviousMenu.Click += new System.EventHandler(this.JoinRowWithPreviousClick);
            //
            // insertBlankRowMenu
            //
            this.insertBlankRowMenu.Name = "insertBlankRowMenu";
            this.insertBlankRowMenu.ShortcutKeyDisplayString = "Ctrl+Ins";
            this.insertBlankRowMenu.Size = new System.Drawing.Size(289, 22);
            this.insertBlankRowMenu.Text = "Insert blank row above";
            this.insertBlankRowMenu.Click += new System.EventHandler(this.InsertBlankRowClick);
            //
            // deleteBlankRowMenu
            //
            this.deleteBlankRowMenu.Name = "deleteBlankRowMenu";
            this.deleteBlankRowMenu.ShortcutKeyDisplayString = "Ctrl+Del";
            this.deleteBlankRowMenu.Size = new System.Drawing.Size(289, 22);
            this.deleteBlankRowMenu.Text = "Delete blank row";
            this.deleteBlankRowMenu.Click += new System.EventHandler(this.DeleteBlankRowClick);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(286, 6);
            //
            // joinCellWithNextMenu
            //
            this.joinCellWithNextMenu.Name = "joinCellWithNextMenu";
            this.joinCellWithNextMenu.ShortcutKeyDisplayString = "Del at end";
            this.joinCellWithNextMenu.Size = new System.Drawing.Size(289, 22);
            this.joinCellWithNextMenu.Text = "Join this cell with next (this tier only)";
            this.joinCellWithNextMenu.Click += new System.EventHandler(this.JoinCellWithNextClick);
            //
            // joinCellWithPreviousMenu
            //
            this.joinCellWithPreviousMenu.Name = "joinCellWithPreviousMenu";
            this.joinCellWithPreviousMenu.ShortcutKeyDisplayString = "Backspace at start";
            this.joinCellWithPreviousMenu.Size = new System.Drawing.Size(289, 22);
            this.joinCellWithPreviousMenu.Text = "Join this cell with previous (this tier only)";
            this.joinCellWithPreviousMenu.Click += new System.EventHandler(this.JoinCellWithPreviousClick);
            //
            // insertBlankCellMenu
            //
            this.insertBlankCellMenu.Name = "insertBlankCellMenu";
            this.insertBlankCellMenu.Size = new System.Drawing.Size(289, 22);
            this.insertBlankCellMenu.Text = "Insert blank cell above (this tier only)";
            this.insertBlankCellMenu.Click += new System.EventHandler(this.InsertBlankCellClick);
            //
            // deleteBlankCellMenu
            //
            this.deleteBlankCellMenu.Name = "deleteBlankCellMenu";
            this.deleteBlankCellMenu.Size = new System.Drawing.Size(289, 22);
            this.deleteBlankCellMenu.Text = "Delete blank cell (this tier only)";
            this.deleteBlankCellMenu.Click += new System.EventHandler(this.DeleteBlankCellClick);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(286, 6);
            //
            // undoMenu
            //
            this.undoMenu.Name = "undoMenu";
            this.undoMenu.ShortcutKeyDisplayString = "Ctrl+Z";
            this.undoMenu.Size = new System.Drawing.Size(289, 22);
            this.undoMenu.Text = "Undo";
            this.undoMenu.Click += new System.EventHandler(this.UndoClick);
            //
            // labelStatus
            //
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.AutoEllipsis = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 525);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(798, 23);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonAccept
            //
            this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAccept.Location = new System.Drawing.Point(816, 525);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 4;
            this.buttonAccept.Text = "&Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.ButtonAcceptClick);
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(897, 525);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            //
            // AlignImportLinesForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(984, 560);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.dataGridViewAlign);
            this.Controls.Add(this.labelInstructions);
            this.Controls.Add(this.toolStrip);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "AlignImportLinesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Line up the imported lines with the story";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAlign)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonJoinWithNext;
        private System.Windows.Forms.ToolStripButton toolStripButtonJoinWithPrevious;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsertBlankRow;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteBlankRow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSource;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxSource;
        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.DataGridView dataGridViewAlign;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSource;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnInclude;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem joinRowWithNextMenu;
        private System.Windows.Forms.ToolStripMenuItem joinRowWithPreviousMenu;
        private System.Windows.Forms.ToolStripMenuItem insertBlankRowMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteBlankRowMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem joinCellWithNextMenu;
        private System.Windows.Forms.ToolStripMenuItem joinCellWithPreviousMenu;
        private System.Windows.Forms.ToolStripMenuItem insertBlankCellMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteBlankCellMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem undoMenu;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonCancel;
    }
}
