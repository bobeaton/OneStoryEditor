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
            this.buttonDelete = new System.Windows.Forms.Button();
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
            this.labelInstructions = new System.Windows.Forms.Label();
            this.tabPageFrontMatter = new System.Windows.Forms.TabPage();
            this.richTextBoxPanoramaFrontMatter = new System.Windows.Forms.RichTextBox();
            this.tabControlSets = new System.Windows.Forms.TabControl();
            this.contextMenuStripTabs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPagePanorama.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).BeginInit();
            this.tabPageFrontMatter.SuspendLayout();
            this.tabControlSets.SuspendLayout();
            this.contextMenuStripTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDelete.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.buttonDelete.Location = new System.Drawing.Point(1646, 373);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(6);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(52, 44);
            this.buttonDelete.TabIndex = 2;
            this.toolTip.SetToolTip(this.buttonDelete, "Delete the selected story");
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDeleteClick);
            // 
            // tabPagePanorama
            // 
            this.tabPagePanorama.Controls.Add(this.tableLayoutPanel);
            this.tabPagePanorama.Location = new System.Drawing.Point(4, 34);
            this.tabPagePanorama.Margin = new System.Windows.Forms.Padding(6);
            this.tabPagePanorama.Name = "tabPagePanorama";
            this.tabPagePanorama.Padding = new System.Windows.Forms.Padding(6);
            this.tabPagePanorama.Size = new System.Drawing.Size(1716, 924);
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
            this.tableLayoutPanel.Controls.Add(this.buttonDelete, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCopyToClipboard, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelInstructions, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(1704, 912);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // dataGridViewPanorama
            // 
            this.dataGridViewPanorama.AllowDrop = true;
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
            this.dataGridViewPanorama.Location = new System.Drawing.Point(6, 6);
            this.dataGridViewPanorama.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridViewPanorama.MultiSelect = false;
            this.dataGridViewPanorama.Name = "dataGridViewPanorama";
            this.dataGridViewPanorama.RowHeadersWidth = 25;
            this.dataGridViewPanorama.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel.SetRowSpan(this.dataGridViewPanorama, 3);
            this.dataGridViewPanorama.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPanorama.Size = new System.Drawing.Size(1628, 778);
            this.dataGridViewPanorama.TabIndex = 0;
            this.dataGridViewPanorama.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewPanorama_CellBeginEdit);
            this.dataGridViewPanorama.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPanorama_CellEndEdit);
            this.dataGridViewPanorama.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewPanoramaCellMouseDoubleClick);
            this.dataGridViewPanorama.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewPanorama_DragDrop);
            this.dataGridViewPanorama.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridViewPanorama_DragOver);
            this.dataGridViewPanorama.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewPanorama_KeyUp);
            this.dataGridViewPanorama.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridViewPanorama_MouseDown);
            this.dataGridViewPanorama.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewPanorama_MouseUp);
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
            this.ColumnLineCount.Width = 112;
            // 
            // ColumnTestQuestions
            // 
            this.ColumnTestQuestions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnTestQuestions.HeaderText = "# of TQs";
            this.ColumnTestQuestions.Name = "ColumnTestQuestions";
            this.ColumnTestQuestions.ReadOnly = true;
            this.ColumnTestQuestions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnTestQuestions.Width = 90;
            // 
            // ColumnWordCount
            // 
            this.ColumnWordCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnWordCount.HeaderText = "# of Words";
            this.ColumnWordCount.Name = "ColumnWordCount";
            this.ColumnWordCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnWordCount.Width = 110;
            // 
            // buttonCopyToClipboard
            // 
            this.buttonCopyToClipboard.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonCopyToClipboard.Location = new System.Drawing.Point(659, 862);
            this.buttonCopyToClipboard.Margin = new System.Windows.Forms.Padding(6);
            this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
            this.buttonCopyToClipboard.Size = new System.Drawing.Size(322, 44);
            this.buttonCopyToClipboard.TabIndex = 5;
            this.buttonCopyToClipboard.Text = "&Copy to clipboard";
            this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyToClipboard.Click += new System.EventHandler(this.buttonCopyToClipboard_Click);
            // 
            // labelInstructions
            // 
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Location = new System.Drawing.Point(6, 790);
            this.labelInstructions.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Padding = new System.Windows.Forms.Padding(8);
            this.labelInstructions.Size = new System.Drawing.Size(1621, 66);
            this.labelInstructions.TabIndex = 6;
            this.labelInstructions.Text = resources.GetString("labelInstructions.Text");
            // 
            // tabPageFrontMatter
            // 
            this.tabPageFrontMatter.Controls.Add(this.richTextBoxPanoramaFrontMatter);
            this.tabPageFrontMatter.Location = new System.Drawing.Point(4, 34);
            this.tabPageFrontMatter.Margin = new System.Windows.Forms.Padding(6);
            this.tabPageFrontMatter.Name = "tabPageFrontMatter";
            this.tabPageFrontMatter.Padding = new System.Windows.Forms.Padding(6);
            this.tabPageFrontMatter.Size = new System.Drawing.Size(1716, 924);
            this.tabPageFrontMatter.TabIndex = 0;
            this.tabPageFrontMatter.Text = "Front Matter";
            this.tabPageFrontMatter.UseVisualStyleBackColor = true;
            // 
            // richTextBoxPanoramaFrontMatter
            // 
            this.richTextBoxPanoramaFrontMatter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxPanoramaFrontMatter.Location = new System.Drawing.Point(6, 6);
            this.richTextBoxPanoramaFrontMatter.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBoxPanoramaFrontMatter.Name = "richTextBoxPanoramaFrontMatter";
            this.richTextBoxPanoramaFrontMatter.Size = new System.Drawing.Size(1704, 912);
            this.richTextBoxPanoramaFrontMatter.TabIndex = 0;
            this.richTextBoxPanoramaFrontMatter.Text = "";
            this.richTextBoxPanoramaFrontMatter.TextChanged += new System.EventHandler(this.RichTextBoxPanoramaFrontMatterTextChanged);
            // 
            // tabControlSets
            // 
            this.tabControlSets.AllowDrop = true;
            this.tabControlSets.Controls.Add(this.tabPageFrontMatter);
            this.tabControlSets.Controls.Add(this.tabPagePanorama);
            this.tabControlSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSets.Location = new System.Drawing.Point(0, 0);
            this.tabControlSets.Margin = new System.Windows.Forms.Padding(6);
            this.tabControlSets.Name = "tabControlSets";
            this.tabControlSets.SelectedIndex = 0;
            this.tabControlSets.Size = new System.Drawing.Size(1724, 962);
            this.tabControlSets.TabIndex = 4;
            this.tabControlSets.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControlSetsSelected);
            this.tabControlSets.DragDrop += new System.Windows.Forms.DragEventHandler(this.tabControlSets_DragDrop);
            this.tabControlSets.DragOver += new System.Windows.Forms.DragEventHandler(this.tabControlSets_DragOver);
            this.tabControlSets.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControlSets_MouseClick);
            // 
            // contextMenuStripTabs
            // 
            this.contextMenuStripTabs.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStripTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddNew,
            this.menuRename,
            this.menuDelete});
            this.contextMenuStripTabs.Name = "contextMenuStripTabs";
            this.contextMenuStripTabs.Size = new System.Drawing.Size(282, 112);
            // 
            // menuAddNew
            // 
            this.menuAddNew.AutoToolTip = true;
            this.menuAddNew.Name = "menuAddNew";
            this.menuAddNew.Size = new System.Drawing.Size(281, 36);
            this.menuAddNew.Text = "&Add new story set";
            this.menuAddNew.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.menuAddNew.ToolTipText = "Add new story set to the right of this tab";
            this.menuAddNew.Click += new System.EventHandler(this.menuAddNew_Click);
            // 
            // menuRename
            // 
            this.menuRename.AutoToolTip = true;
            this.menuRename.Name = "menuRename";
            this.menuRename.Size = new System.Drawing.Size(281, 36);
            this.menuRename.Text = "&Rename story set";
            this.menuRename.Click += new System.EventHandler(this.menuRename_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.AutoToolTip = true;
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(281, 36);
            this.menuDelete.Text = "&Delete story set";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // PanoramaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1724, 962);
            this.Controls.Add(this.tabControlSets);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(12);
            this.Name = "PanoramaView";
            this.Text = "Panorama View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PanoramaViewFormClosing);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PanoramaView_KeyUp);
            this.tabPagePanorama.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPanorama)).EndInit();
            this.tabPageFrontMatter.ResumeLayout(false);
            this.tabControlSets.ResumeLayout(false);
            this.contextMenuStripTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
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
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TabPage tabPageFrontMatter;
        private System.Windows.Forms.RichTextBox richTextBoxPanoramaFrontMatter;
        private System.Windows.Forms.TabControl tabControlSets;
        private Button buttonCopyToClipboard;
        private Label labelInstructions;
        private ContextMenuStrip contextMenuStripTabs;
        private ToolStripMenuItem menuAddNew;
        private ToolStripMenuItem menuRename;
        private ToolStripMenuItem menuDelete;
    }
}