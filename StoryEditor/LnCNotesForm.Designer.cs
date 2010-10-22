namespace OneStoryProjectEditor
{
    partial class LnCNotesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LnCNotesForm));
            this.dataGridViewLnCNotes = new System.Windows.Forms.DataGridView();
            this.ColumnGloss = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRenderings = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddLnCNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditLnCNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteKeyTerm = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLnCNotes)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewLnCNotes
            // 
            this.dataGridViewLnCNotes.AllowUserToAddRows = false;
            this.dataGridViewLnCNotes.AllowUserToOrderColumns = true;
            this.dataGridViewLnCNotes.AllowUserToResizeRows = false;
            this.dataGridViewLnCNotes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewLnCNotes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridViewLnCNotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLnCNotes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnGloss,
            this.ColumnRenderings,
            this.ColumnNotes});
            this.dataGridViewLnCNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLnCNotes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewLnCNotes.Location = new System.Drawing.Point(0, 25);
            this.dataGridViewLnCNotes.MultiSelect = false;
            this.dataGridViewLnCNotes.Name = "dataGridViewLnCNotes";
            this.dataGridViewLnCNotes.RowHeadersWidth = 25;
            this.dataGridViewLnCNotes.Size = new System.Drawing.Size(702, 332);
            this.dataGridViewLnCNotes.TabIndex = 1;
            this.dataGridViewLnCNotes.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewLnCNotes_CellMouseDoubleClick);
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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddLnCNote,
            this.toolStripButtonEditLnCNote,
            this.toolStripButtonDeleteKeyTerm,
            this.toolStripSeparator1,
            this.toolStripButtonSearch});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(702, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddLnCNote
            // 
            this.toolStripButtonAddLnCNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddLnCNote.Image = global::OneStoryProjectEditor.Properties.Resources.AddTableHS;
            this.toolStripButtonAddLnCNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddLnCNote.Name = "toolStripButtonAddLnCNote";
            this.toolStripButtonAddLnCNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddLnCNote.Text = "Add L && C Note";
            this.toolStripButtonAddLnCNote.ToolTipText = "Click this button to add a L&C Note";
            this.toolStripButtonAddLnCNote.Click += new System.EventHandler(this.toolStripButtonAddLnCNote_Click);
            // 
            // toolStripButtonEditLnCNote
            // 
            this.toolStripButtonEditLnCNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditLnCNote.Image = global::OneStoryProjectEditor.Properties.Resources.EditInformationHS;
            this.toolStripButtonEditLnCNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditLnCNote.Name = "toolStripButtonEditLnCNote";
            this.toolStripButtonEditLnCNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditLnCNote.Text = "Edit L&&C Note";
            this.toolStripButtonEditLnCNote.ToolTipText = "Click this button to edit the selected L&C Note";
            this.toolStripButtonEditLnCNote.Click += new System.EventHandler(this.toolStripButtonEditLnCNote_Click);
            // 
            // toolStripButtonDeleteKeyTerm
            // 
            this.toolStripButtonDeleteKeyTerm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteKeyTerm.Image = global::OneStoryProjectEditor.Properties.Resources.DeleteHS;
            this.toolStripButtonDeleteKeyTerm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteKeyTerm.Name = "toolStripButtonDeleteKeyTerm";
            this.toolStripButtonDeleteKeyTerm.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteKeyTerm.Text = "Delete Selected L&&C Note";
            this.toolStripButtonDeleteKeyTerm.ToolTipText = "Click this button to delete the selected L&C Note";
            this.toolStripButtonDeleteKeyTerm.Click += new System.EventHandler(this.toolStripButtonDeleteKeyTerm_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSearch.Image = global::OneStoryProjectEditor.Properties.Resources.search;
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSearch.Text = "Search";
            this.toolStripButtonSearch.ToolTipText = "Click this button to search the stories for this term";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // LnCNotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 357);
            this.Controls.Add(this.dataGridViewLnCNotes);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LnCNotesForm";
            this.Text = "Language and Culture Notes";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLnCNotes)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewLnCNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnGloss;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRenderings;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNotes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddLnCNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditLnCNote;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteKeyTerm;
    }
}