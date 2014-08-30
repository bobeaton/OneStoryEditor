using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    partial class PanoramaView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanoramaView));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonCopyToOldStories = new System.Windows.Forms.Button();
            this.contextMenuMove = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveToStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToNonBibStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToNonBibStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToOldStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToOldStoriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.tabPageOldStories = new System.Windows.Forms.TabPage();
            this.tabPageNonBibStories = new System.Windows.Forms.TabPage();
            this.tabPagePanorama = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewPanorama = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTimeInState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLineCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTestQuestions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnWordCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonCopyToClipboard = new System.Windows.Forms.Button();
            this.tabPageFrontMatter = new System.Windows.Forms.TabPage();
            this.richTextBoxPanoramaFrontMatter = new System.Windows.Forms.RichTextBox();
            this.tabControlSets = new System.Windows.Forms.TabControl();
            this.contextMenuMove.SuspendLayout();
            this.tabPagePanorama.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).BeginInit();
            this.tabPageFrontMatter.SuspendLayout();
            this.tabControlSets.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonMoveDown.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_movedown1;
            this.buttonMoveDown.Location = new System.Drawing.Point(819, 251);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(25, 23);
            this.buttonMoveDown.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonMoveDown, "Move the selected story down");
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonCopyToOldStories
            // 
            this.buttonCopyToOldStories.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonCopyToOldStories.ContextMenuStrip = this.contextMenuMove;
            this.buttonCopyToOldStories.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonCopyToOldStories.Image = global::OneStoryProjectEditor.Properties.Resources.CopyHS;
            this.buttonCopyToOldStories.Location = new System.Drawing.Point(819, 222);
            this.buttonCopyToOldStories.Name = "buttonCopyToOldStories";
            this.buttonCopyToOldStories.Size = new System.Drawing.Size(26, 23);
            this.buttonCopyToOldStories.TabIndex = 3;
            this.toolTip.SetToolTip(this.buttonCopyToOldStories, "Copy the selected story to one of the other lists");
            this.buttonCopyToOldStories.UseVisualStyleBackColor = true;
            this.buttonCopyToOldStories.Click += new System.EventHandler(this.ButtonCopyToOldStoriesClick);
            // 
            // contextMenuMove
            // 
            this.contextMenuMove.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToStoriesMenu,
            this.copyToStoriesMenu,
            this.moveToNonBibStoriesMenu,
            this.copyToNonBibStoriesMenu,
            this.moveToOldStoriesMenu,
            this.copyToOldStoriesMenu});
            this.contextMenuMove.Name = "contextMenuMove";
            this.contextMenuMove.Size = new System.Drawing.Size(327, 136);
            this.contextMenuMove.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuMoveOpening);
            // 
            // moveToStoriesMenu
            // 
            this.moveToStoriesMenu.Name = "moveToStoriesMenu";
            this.moveToStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.moveToStoriesMenu.Text = "Move selected story to \'&Stories\' tab";
            this.moveToStoriesMenu.Click += new System.EventHandler(this.MoveToStoriesMenuClick);
            // 
            // copyToStoriesMenu
            // 
            this.copyToStoriesMenu.Name = "copyToStoriesMenu";
            this.copyToStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.copyToStoriesMenu.Text = "Copy selected story to \'&Stories\' tab";
            this.copyToStoriesMenu.Click += new System.EventHandler(this.CopyToStoriesMenuClick);
            // 
            // moveToNonBibStoriesMenu
            // 
            this.moveToNonBibStoriesMenu.Name = "moveToNonBibStoriesMenu";
            this.moveToNonBibStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.moveToNonBibStoriesMenu.Text = "Move selected story to \'&Non-Biblical Stories\' tab";
            this.moveToNonBibStoriesMenu.Click += new System.EventHandler(this.MoveToNonBibStoriesMenuClick);
            // 
            // copyToNonBibStoriesMenu
            // 
            this.copyToNonBibStoriesMenu.Name = "copyToNonBibStoriesMenu";
            this.copyToNonBibStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.copyToNonBibStoriesMenu.Text = "Copy selected story to \'&Non-Biblical Stories\' tab";
            this.copyToNonBibStoriesMenu.Click += new System.EventHandler(this.CopyToNonBibStoriesMenuClick);
            // 
            // moveToOldStoriesMenu
            // 
            this.moveToOldStoriesMenu.Name = "moveToOldStoriesMenu";
            this.moveToOldStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.moveToOldStoriesMenu.Text = "Move selected story to \'&Old Stories\' tab";
            this.moveToOldStoriesMenu.Click += new System.EventHandler(this.MoveToOldStoriesMenuClick);
            // 
            // copyToOldStoriesMenu
            // 
            this.copyToOldStoriesMenu.Name = "copyToOldStoriesMenu";
            this.copyToOldStoriesMenu.Size = new System.Drawing.Size(326, 22);
            this.copyToOldStoriesMenu.Text = "Copy selected story to \'&Old Stories\' tab";
            this.copyToOldStoriesMenu.Click += new System.EventHandler(this.CopyToOldStoriesMenuClick);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDelete.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.buttonDelete.Location = new System.Drawing.Point(819, 193);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(26, 23);
            this.buttonDelete.TabIndex = 2;
            this.toolTip.SetToolTip(this.buttonDelete, "Delete the selected story");
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDeleteClick);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonMoveUp.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_moveup1;
            this.buttonMoveUp.Location = new System.Drawing.Point(819, 164);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(26, 23);
            this.buttonMoveUp.TabIndex = 1;
            this.toolTip.SetToolTip(this.buttonMoveUp, "Move selected story up");
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // tabPageOldStories
            // 
            this.tabPageOldStories.Location = new System.Drawing.Point(4, 22);
            this.tabPageOldStories.Name = "tabPageOldStories";
            this.tabPageOldStories.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOldStories.Size = new System.Drawing.Size(854, 474);
            this.tabPageOldStories.TabIndex = 3;
            this.tabPageOldStories.Text = "Old Stories";
            this.tabPageOldStories.UseVisualStyleBackColor = true;
            // 
            // tabPageNonBibStories
            // 
            this.tabPageNonBibStories.Location = new System.Drawing.Point(4, 22);
            this.tabPageNonBibStories.Name = "tabPageNonBibStories";
            this.tabPageNonBibStories.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNonBibStories.Size = new System.Drawing.Size(854, 474);
            this.tabPageNonBibStories.TabIndex = 2;
            this.tabPageNonBibStories.Text = "Non-Biblical Stories";
            this.tabPageNonBibStories.UseVisualStyleBackColor = true;
            // 
            // tabPagePanorama
            // 
            this.tabPagePanorama.Controls.Add(this.tableLayoutPanel);
            this.tabPagePanorama.Location = new System.Drawing.Point(4, 22);
            this.tabPagePanorama.Name = "tabPagePanorama";
            this.tabPagePanorama.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePanorama.Size = new System.Drawing.Size(854, 474);
            this.tabPagePanorama.TabIndex = 1;
            this.tabPagePanorama.Text = "Stories";
            this.tabPagePanorama.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.dataGridViewPanorama, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonMoveUp, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonDelete, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCopyToOldStories, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonMoveDown, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonCopyToClipboard, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(848, 468);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // dataGridViewPanorama
            // 
            this.dataGridViewPanorama.AllowUserToAddRows = false;
            this.dataGridViewPanorama.AllowUserToOrderColumns = true;
            this.dataGridViewPanorama.AllowUserToResizeRows = false;
            this.dataGridViewPanorama.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPanorama.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridViewPanorama.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPanorama.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.ColumnTimeInState,
            this.ColumnLineCount,
            this.ColumnTestQuestions,
            this.ColumnWordCount});
            this.dataGridViewPanorama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPanorama.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewPanorama.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewPanorama.MultiSelect = false;
            this.dataGridViewPanorama.Name = "dataGridViewPanorama";
            this.dataGridViewPanorama.RowHeadersWidth = 25;
            this.dataGridViewPanorama.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel.SetRowSpan(this.dataGridViewPanorama, 4);
            this.dataGridViewPanorama.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewPanorama.Size = new System.Drawing.Size(810, 432);
            this.dataGridViewPanorama.TabIndex = 0;
            this.dataGridViewPanorama.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewPanorama_CellBeginEdit);
            this.dataGridViewPanorama.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPanorama_CellEndEdit);
            this.dataGridViewPanorama.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewPanoramaCellMouseDoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Story Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Purpose";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Who Edits";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnTimeInState
            // 
            this.ColumnTimeInState.HeaderText = "Time in Turn";
            this.ColumnTimeInState.Name = "ColumnTimeInState";
            this.ColumnTimeInState.ReadOnly = true;
            this.ColumnTimeInState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnLineCount
            // 
            this.ColumnLineCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnLineCount.HeaderText = "# of Lines";
            this.ColumnLineCount.Name = "ColumnLineCount";
            this.ColumnLineCount.ReadOnly = true;
            this.ColumnLineCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnLineCount.Width = 60;
            // 
            // ColumnTestQuestions
            // 
            this.ColumnTestQuestions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnTestQuestions.HeaderText = "# of TQs";
            this.ColumnTestQuestions.Name = "ColumnTestQuestions";
            this.ColumnTestQuestions.ReadOnly = true;
            this.ColumnTestQuestions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnTestQuestions.Width = 55;
            // 
            // ColumnWordCount
            // 
            this.ColumnWordCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnWordCount.HeaderText = "# of Words";
            this.ColumnWordCount.Name = "ColumnWordCount";
            this.ColumnWordCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnWordCount.Width = 66;
            // 
            // buttonCopyToClipboard
            // 
            this.buttonCopyToClipboard.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonCopyToClipboard.Location = new System.Drawing.Point(327, 441);
            this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
            this.buttonCopyToClipboard.Size = new System.Drawing.Size(161, 23);
            this.buttonCopyToClipboard.TabIndex = 5;
            this.buttonCopyToClipboard.Text = "&Copy to clipboard";
            this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyToClipboard.Click += new System.EventHandler(this.buttonCopyToClipboard_Click);
            // 
            // tabPageFrontMatter
            // 
            this.tabPageFrontMatter.Controls.Add(this.richTextBoxPanoramaFrontMatter);
            this.tabPageFrontMatter.Location = new System.Drawing.Point(4, 22);
            this.tabPageFrontMatter.Name = "tabPageFrontMatter";
            this.tabPageFrontMatter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFrontMatter.Size = new System.Drawing.Size(854, 474);
            this.tabPageFrontMatter.TabIndex = 0;
            this.tabPageFrontMatter.Text = "Front Matter";
            this.tabPageFrontMatter.UseVisualStyleBackColor = true;
            // 
            // richTextBoxPanoramaFrontMatter
            // 
            this.richTextBoxPanoramaFrontMatter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxPanoramaFrontMatter.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxPanoramaFrontMatter.Name = "richTextBoxPanoramaFrontMatter";
            this.richTextBoxPanoramaFrontMatter.Size = new System.Drawing.Size(848, 468);
            this.richTextBoxPanoramaFrontMatter.TabIndex = 0;
            this.richTextBoxPanoramaFrontMatter.Text = "";
            this.richTextBoxPanoramaFrontMatter.TextChanged += new System.EventHandler(this.RichTextBoxPanoramaFrontMatterTextChanged);
            // 
            // tabControlSets
            // 
            this.tabControlSets.Controls.Add(this.tabPageFrontMatter);
            this.tabControlSets.Controls.Add(this.tabPagePanorama);
            this.tabControlSets.Controls.Add(this.tabPageNonBibStories);
            this.tabControlSets.Controls.Add(this.tabPageOldStories);
            this.tabControlSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSets.Location = new System.Drawing.Point(0, 0);
            this.tabControlSets.Name = "tabControlSets";
            this.tabControlSets.SelectedIndex = 0;
            this.tabControlSets.Size = new System.Drawing.Size(862, 500);
            this.tabControlSets.TabIndex = 4;
            this.tabControlSets.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControlSetsSelected);
            // 
            // PanoramaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 500);
            this.Controls.Add(this.tabControlSets);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanoramaView";
            this.Text = "Panorama View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PanoramaViewFormClosing);
            this.contextMenuMove.ResumeLayout(false);
            this.tabPagePanorama.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).EndInit();
            this.tabPageFrontMatter.ResumeLayout(false);
            this.tabControlSets.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage tabPageNonBibStories;
        private System.Windows.Forms.TabPage tabPageOldStories;
        private System.Windows.Forms.TabPage tabPagePanorama;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.DataGridView dataGridViewPanorama;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimeInState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLineCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTestQuestions;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnWordCount;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCopyToOldStories;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.TabPage tabPageFrontMatter;
        private System.Windows.Forms.RichTextBox richTextBoxPanoramaFrontMatter;
        private System.Windows.Forms.TabControl tabControlSets;
        private ContextMenuStrip contextMenuMove;
        private ToolStripMenuItem copyToNonBibStoriesMenu;
        private ToolStripMenuItem copyToOldStoriesMenu;
        private ToolStripMenuItem copyToStoriesMenu;
        private ToolStripMenuItem moveToStoriesMenu;
        private ToolStripMenuItem moveToNonBibStoriesMenu;
        private ToolStripMenuItem moveToOldStoriesMenu;
        private Button buttonCopyToClipboard;
    }
}