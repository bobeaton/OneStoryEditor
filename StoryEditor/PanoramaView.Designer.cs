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
            this.richTextBoxPanoramaFrontMatter = new System.Windows.Forms.RichTextBox();
            this.tabControlSets = new System.Windows.Forms.TabControl();
            this.tabPageFrontMatter = new System.Windows.Forms.TabPage();
            this.tabPagePanorama = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewPanorama = new System.Windows.Forms.DataGridView();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCopyToOldStories = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.tabPageObsolete = new System.Windows.Forms.TabPage();
            this.tabPageKeyTerms = new System.Windows.Forms.TabPage();
            this.dataGridViewKeyTerms = new System.Windows.Forms.DataGridView();
            this.ColumnGloss = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRenderings = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTimeInState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLineCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTestQuestions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnWordCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControlSets.SuspendLayout();
            this.tabPageFrontMatter.SuspendLayout();
            this.tabPagePanorama.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).BeginInit();
            this.tabPageKeyTerms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeyTerms)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBoxPanoramaFrontMatter
            // 
            this.richTextBoxPanoramaFrontMatter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxPanoramaFrontMatter.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxPanoramaFrontMatter.Name = "richTextBoxPanoramaFrontMatter";
            this.richTextBoxPanoramaFrontMatter.Size = new System.Drawing.Size(848, 468);
            this.richTextBoxPanoramaFrontMatter.TabIndex = 0;
            this.richTextBoxPanoramaFrontMatter.Text = "";
            this.richTextBoxPanoramaFrontMatter.TextChanged += new System.EventHandler(this.richTextBoxPanoramaFrontMatter_TextChanged);
            // 
            // tabControlSets
            // 
            this.tabControlSets.Controls.Add(this.tabPageFrontMatter);
            this.tabControlSets.Controls.Add(this.tabPagePanorama);
            this.tabControlSets.Controls.Add(this.tabPageObsolete);
            this.tabControlSets.Controls.Add(this.tabPageKeyTerms);
            this.tabControlSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSets.Location = new System.Drawing.Point(0, 0);
            this.tabControlSets.Name = "tabControlSets";
            this.tabControlSets.SelectedIndex = 0;
            this.tabControlSets.Size = new System.Drawing.Size(862, 500);
            this.tabControlSets.TabIndex = 4;
            this.tabControlSets.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlSets_Selected);
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
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
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
            this.dataGridViewTextBoxColumn4,
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
            this.dataGridViewPanorama.Size = new System.Drawing.Size(810, 462);
            this.dataGridViewPanorama.TabIndex = 0;
            this.dataGridViewPanorama.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewPanorama_CellBeginEdit);
            this.dataGridViewPanorama.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPanorama_CellEndEdit);
            this.dataGridViewPanorama.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPanorama_CellMouseDoubleClick);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonMoveUp.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_moveup1;
            this.buttonMoveUp.Location = new System.Drawing.Point(819, 179);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(26, 23);
            this.buttonMoveUp.TabIndex = 1;
            this.toolTip.SetToolTip(this.buttonMoveUp, "Move selected story up");
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDelete.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.buttonDelete.Location = new System.Drawing.Point(819, 208);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(26, 23);
            this.buttonDelete.TabIndex = 2;
            this.toolTip.SetToolTip(this.buttonDelete, "Delete the selected story");
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCopyToOldStories
            // 
            this.buttonCopyToOldStories.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonCopyToOldStories.Image = global::OneStoryProjectEditor.Properties.Resources.CopyHS;
            this.buttonCopyToOldStories.Location = new System.Drawing.Point(819, 237);
            this.buttonCopyToOldStories.Name = "buttonCopyToOldStories";
            this.buttonCopyToOldStories.Size = new System.Drawing.Size(26, 23);
            this.buttonCopyToOldStories.TabIndex = 3;
            this.toolTip.SetToolTip(this.buttonCopyToOldStories, "Copy the selected story to the \"Old Stories\" list. Then you can use the \'View\' me" +
                    "nu, \'View Old Stories\' command (from the main window) to view stories in the \'Ol" +
                    "d Stories\' list.");
            this.buttonCopyToOldStories.UseVisualStyleBackColor = true;
            this.buttonCopyToOldStories.Click += new System.EventHandler(this.buttonCopyToOldStories_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonMoveDown.Image = global::OneStoryProjectEditor.Properties.Resources.BuilderDialog_movedown1;
            this.buttonMoveDown.Location = new System.Drawing.Point(819, 266);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(25, 23);
            this.buttonMoveDown.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonMoveDown, "Move the selected story down");
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // tabPageObsolete
            // 
            this.tabPageObsolete.Location = new System.Drawing.Point(4, 22);
            this.tabPageObsolete.Name = "tabPageObsolete";
            this.tabPageObsolete.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageObsolete.Size = new System.Drawing.Size(854, 474);
            this.tabPageObsolete.TabIndex = 2;
            this.tabPageObsolete.Text = "Old Stories";
            this.tabPageObsolete.UseVisualStyleBackColor = true;
            // 
            // tabPageKeyTerms
            // 
            this.tabPageKeyTerms.Controls.Add(this.dataGridViewKeyTerms);
            this.tabPageKeyTerms.Location = new System.Drawing.Point(4, 22);
            this.tabPageKeyTerms.Name = "tabPageKeyTerms";
            this.tabPageKeyTerms.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKeyTerms.Size = new System.Drawing.Size(854, 474);
            this.tabPageKeyTerms.TabIndex = 3;
            this.tabPageKeyTerms.Text = "Key Terms";
            this.tabPageKeyTerms.UseVisualStyleBackColor = true;
            // 
            // dataGridViewKeyTerms
            // 
            this.dataGridViewKeyTerms.AllowUserToAddRows = false;
            this.dataGridViewKeyTerms.AllowUserToOrderColumns = true;
            this.dataGridViewKeyTerms.AllowUserToResizeRows = false;
            this.dataGridViewKeyTerms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewKeyTerms.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridViewKeyTerms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKeyTerms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnGloss,
            this.ColumnRenderings,
            this.ColumnNotes});
            this.dataGridViewKeyTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewKeyTerms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewKeyTerms.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewKeyTerms.MultiSelect = false;
            this.dataGridViewKeyTerms.Name = "dataGridViewKeyTerms";
            this.dataGridViewKeyTerms.RowHeadersWidth = 25;
            this.dataGridViewKeyTerms.Size = new System.Drawing.Size(848, 468);
            this.dataGridViewKeyTerms.TabIndex = 0;
            this.dataGridViewKeyTerms.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewKeyTerms_CellMouseDoubleClick);
            this.dataGridViewKeyTerms.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewKeyTerms_CellMouseDoubleClick);
            // 
            // ColumnGloss
            // 
            this.ColumnGloss.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGloss.HeaderText = "Gloss";
            this.ColumnGloss.Name = "ColumnGloss";
            this.ColumnGloss.ReadOnly = true;
            this.ColumnGloss.Width = 58;
            // 
            // ColumnRenderings
            // 
            this.ColumnRenderings.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRenderings.HeaderText = "Renderings";
            this.ColumnRenderings.Name = "ColumnRenderings";
            this.ColumnRenderings.Width = 86;
            // 
            // ColumnNotes
            // 
            this.ColumnNotes.HeaderText = "Notes";
            this.ColumnNotes.Name = "ColumnNotes";
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
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Stage";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.ToolTipText = "Right-click to change state";
            // 
            // ColumnTimeInState
            // 
            this.ColumnTimeInState.HeaderText = "Time in Stage";
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
            // PanoramaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 500);
            this.Controls.Add(this.tabControlSets);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanoramaView";
            this.Text = "Panorama View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PanoramaView_FormClosing);
            this.tabControlSets.ResumeLayout(false);
            this.tabPageFrontMatter.ResumeLayout(false);
            this.tabPagePanorama.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).EndInit();
            this.tabPageKeyTerms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeyTerms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxPanoramaFrontMatter;
        private System.Windows.Forms.TabControl tabControlSets;
        private System.Windows.Forms.TabPage tabPageFrontMatter;
        private System.Windows.Forms.TabPage tabPagePanorama;
        private System.Windows.Forms.TabPage tabPageObsolete;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.DataGridView dataGridViewPanorama;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonCopyToOldStories;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage tabPageKeyTerms;
        private System.Windows.Forms.DataGridView dataGridViewKeyTerms;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnGloss;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRenderings;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimeInState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLineCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTestQuestions;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnWordCount;
    }
}