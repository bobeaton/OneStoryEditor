namespace OneStoryProjectEditor
{
    partial class CutItemPicker
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Cultural and Exegetical Notes (cn)");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Testing Questions (tst)");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CutItemPicker));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.treeViewItems = new OneStoryProjectEditor.MyTreeView();
            this.flowLayoutPanelLines = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.treeViewItems, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelLines, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(35, 31);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 119F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1880, 820);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // treeViewItems
            // 
            this.treeViewItems.CheckBoxes = true;
            this.treeViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewItems.Location = new System.Drawing.Point(8, 7);
            this.treeViewItems.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.treeViewItems.Name = "treeViewItems";
            treeNode1.Name = "CulturalNotes";
            treeNode1.Text = "Cultural and Exegetical Notes (cn)";
            treeNode2.Name = "TestingQuestions";
            treeNode2.Text = "Testing Questions (tst)";
            this.treeViewItems.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeViewItems.ShowRootLines = false;
            this.treeViewItems.Size = new System.Drawing.Size(857, 687);
            this.treeViewItems.TabIndex = 2;
            this.treeViewItems.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewItems_NodeMouseClick);
            // 
            // flowLayoutPanelLines
            // 
            this.flowLayoutPanelLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelLines.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelLines.Location = new System.Drawing.Point(1014, 7);
            this.flowLayoutPanelLines.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.flowLayoutPanelLines.Name = "flowLayoutPanelLines";
            this.flowLayoutPanelLines.Size = new System.Drawing.Size(858, 687);
            this.flowLayoutPanelLines.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::OneStoryProjectEditor.Properties.Resources.Arrow;
            this.pictureBox1.Location = new System.Drawing.Point(881, 291);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(117, 119);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel.SetColumnSpan(this.buttonCancel, 3);
            this.buttonCancel.Location = new System.Drawing.Point(618, 741);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(643, 72);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Close without moving items";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // CutItemPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1947, 880);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(21, 17, 21, 17);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CutItemPicker";
            this.Text = "Choose the item(s) to move and then click the line to move to";
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private MyTreeView treeViewItems;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLines;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonCancel;
    }
}