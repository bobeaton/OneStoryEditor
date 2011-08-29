namespace NetLoc
{
	partial class LocDataControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.uiGrid = new NetLoc.DataGridView2();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefaultColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.uiGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.uiGrid, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 429F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(724, 429);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// uiGrid
			// 
			this.uiGrid.AllowUserToAddRows = false;
			this.uiGrid.AllowUserToDeleteRows = false;
			this.uiGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.uiGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.uiGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.uiGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PathColumn,
            this.DefaultColumn,
            this.ValueColumn});
			this.tableLayoutPanel1.SetColumnSpan(this.uiGrid, 2);
			this.uiGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.uiGrid.Location = new System.Drawing.Point(3, 3);
			this.uiGrid.Name = "uiGrid";
			this.uiGrid.RowHeadersVisible = false;
			this.uiGrid.RowTemplate.Height = 24;
			this.uiGrid.Size = new System.Drawing.Size(718, 423);
			this.uiGrid.TabIndex = 2;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.DataPropertyName = "Path";
			this.dataGridViewTextBoxColumn1.HeaderText = "Path";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			this.dataGridViewTextBoxColumn1.Width = 120;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.DataPropertyName = "Default";
			this.dataGridViewTextBoxColumn2.HeaderText = "Default";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn3
			// 
			this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn3.DataPropertyName = "Value";
			this.dataGridViewTextBoxColumn3.HeaderText = "Value";
			this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			// 
			// PathColumn
			// 
			this.PathColumn.DataPropertyName = "Path";
			this.PathColumn.HeaderText = "Key";
			this.PathColumn.Name = "PathColumn";
			this.PathColumn.ReadOnly = true;
			this.PathColumn.Width = 200;
			// 
			// DefaultColumn
			// 
			this.DefaultColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.DefaultColumn.DataPropertyName = "Default";
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DefaultColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.DefaultColumn.HeaderText = "Default Value";
			this.DefaultColumn.Name = "DefaultColumn";
			this.DefaultColumn.ReadOnly = true;
			// 
			// ValueColumn
			// 
			this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ValueColumn.DataPropertyName = "Value";
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.ValueColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.ValueColumn.HeaderText = "Translation";
			this.ValueColumn.Name = "ValueColumn";
			// 
			// LocDataControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "LocDataControl";
			this.Size = new System.Drawing.Size(724, 429);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.uiGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private NetLoc.DataGridView2 uiGrid;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefaultColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
	}
}
