namespace OneStoryProjectEditor
{
    partial class AddLnCNoteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddLnCNoteForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelVernacular = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxVernacular = new System.Windows.Forms.TextBox();
            this.labelNationalBT = new System.Windows.Forms.Label();
            this.textBoxNationalBT = new System.Windows.Forms.TextBox();
            this.labelInternationalBT = new System.Windows.Forms.Label();
            this.textBoxInternationalBT = new System.Windows.Forms.TextBox();
            this.labelNotes = new System.Windows.Forms.Label();
            this.textBoxNotes = new System.Windows.Forms.TextBox();
            this.labelOptionalKeyTerms = new System.Windows.Forms.Label();
            this.buttonKeyTermSelect = new System.Windows.Forms.Button();
            this.textBoxKeyTerms = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.labelVernacular, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxVernacular, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelNationalBT, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxNationalBT, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelInternationalBT, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxInternationalBT, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelNotes, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxNotes, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.labelOptionalKeyTerms, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonKeyTermSelect, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxKeyTerms, 2, 4);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(517, 340);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(264, 314);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelVernacular
            // 
            this.labelVernacular.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelVernacular.AutoSize = true;
            this.labelVernacular.Location = new System.Drawing.Point(59, 6);
            this.labelVernacular.Name = "labelVernacular";
            this.labelVernacular.Size = new System.Drawing.Size(49, 13);
            this.labelVernacular.TabIndex = 0;
            this.labelVernacular.Text = "Story Lg:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(183, 314);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxVernacular
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxVernacular, 2);
            this.textBoxVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxVernacular, resources.GetString("textBoxVernacular.HelpString"));
            this.textBoxVernacular.Location = new System.Drawing.Point(114, 3);
            this.textBoxVernacular.Name = "textBoxVernacular";
            this.helpProvider.SetShowHelp(this.textBoxVernacular, true);
            this.textBoxVernacular.Size = new System.Drawing.Size(400, 20);
            this.textBoxVernacular.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxVernacular, "Enter the different spellings of this word separated by commas. Press F1 for furt" +
                    "her instructions on how to enter data in this field.");
            this.textBoxVernacular.Leave += new System.EventHandler(this.textBox_Leave);
            this.textBoxVernacular.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // labelNationalBT
            // 
            this.labelNationalBT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNationalBT.AutoSize = true;
            this.labelNationalBT.Location = new System.Drawing.Point(27, 32);
            this.labelNationalBT.Name = "labelNationalBT";
            this.labelNationalBT.Size = new System.Drawing.Size(81, 13);
            this.labelNationalBT.TabIndex = 2;
            this.labelNationalBT.Text = "National Lg BT:";
            // 
            // textBoxNationalBT
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxNationalBT, 2);
            this.textBoxNationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxNationalBT, resources.GetString("textBoxNationalBT.HelpString"));
            this.textBoxNationalBT.Location = new System.Drawing.Point(114, 29);
            this.textBoxNationalBT.Name = "textBoxNationalBT";
            this.helpProvider.SetShowHelp(this.textBoxNationalBT, true);
            this.textBoxNationalBT.Size = new System.Drawing.Size(400, 20);
            this.textBoxNationalBT.TabIndex = 3;
            this.toolTip.SetToolTip(this.textBoxNationalBT, "Enter the different spellings of this word separated by commas. Press F1 for furt" +
                    "her instructions on how to enter data in this field.");
            this.textBoxNationalBT.Leave += new System.EventHandler(this.textBox_Leave);
            this.textBoxNationalBT.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // labelInternationalBT
            // 
            this.labelInternationalBT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelInternationalBT.AutoSize = true;
            this.labelInternationalBT.Location = new System.Drawing.Point(8, 58);
            this.labelInternationalBT.Name = "labelInternationalBT";
            this.labelInternationalBT.Size = new System.Drawing.Size(100, 13);
            this.labelInternationalBT.TabIndex = 4;
            this.labelInternationalBT.Text = "International Lg BT:";
            // 
            // textBoxInternationalBT
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxInternationalBT, 2);
            this.textBoxInternationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxInternationalBT, resources.GetString("textBoxInternationalBT.HelpString"));
            this.textBoxInternationalBT.Location = new System.Drawing.Point(114, 55);
            this.textBoxInternationalBT.Name = "textBoxInternationalBT";
            this.helpProvider.SetShowHelp(this.textBoxInternationalBT, true);
            this.textBoxInternationalBT.Size = new System.Drawing.Size(400, 20);
            this.textBoxInternationalBT.TabIndex = 5;
            this.toolTip.SetToolTip(this.textBoxInternationalBT, "Enter the different spellings of this word separated by commas. Press F1 for furt" +
                    "her instructions on how to enter data in this field.");
            this.textBoxInternationalBT.Leave += new System.EventHandler(this.textBox_Leave);
            this.textBoxInternationalBT.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // labelNotes
            // 
            this.labelNotes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNotes.AutoSize = true;
            this.labelNotes.Location = new System.Drawing.Point(70, 162);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(38, 13);
            this.labelNotes.TabIndex = 6;
            this.labelNotes.Text = "Notes:";
            // 
            // textBoxNotes
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxNotes, 2);
            this.textBoxNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxNotes, "Enter notes regarding this entry (e.g. which community of people use the term, if" +
                    " it\'s made up of parts, what do they mean, etc)");
            this.textBoxNotes.Location = new System.Drawing.Point(114, 81);
            this.textBoxNotes.Multiline = true;
            this.textBoxNotes.Name = "textBoxNotes";
            this.helpProvider.SetShowHelp(this.textBoxNotes, true);
            this.textBoxNotes.Size = new System.Drawing.Size(400, 176);
            this.textBoxNotes.TabIndex = 7;
            this.toolTip.SetToolTip(this.textBoxNotes, "Enter notes on this term");
            // 
            // labelOptionalKeyTerms
            // 
            this.labelOptionalKeyTerms.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOptionalKeyTerms.AutoSize = true;
            this.helpProvider.SetHelpString(this.labelOptionalKeyTerms, "Click this button to associate a biblical key term with this word");
            this.labelOptionalKeyTerms.Location = new System.Drawing.Point(3, 268);
            this.labelOptionalKeyTerms.Name = "labelOptionalKeyTerms";
            this.helpProvider.SetShowHelp(this.labelOptionalKeyTerms, true);
            this.labelOptionalKeyTerms.Size = new System.Drawing.Size(105, 13);
            this.labelOptionalKeyTerms.TabIndex = 8;
            this.labelOptionalKeyTerms.Text = "Optional KeyTerm(s):";
            this.labelOptionalKeyTerms.Visible = false;
            // 
            // buttonKeyTermSelect
            // 
            this.buttonKeyTermSelect.Location = new System.Drawing.Point(114, 263);
            this.buttonKeyTermSelect.Name = "buttonKeyTermSelect";
            this.buttonKeyTermSelect.Size = new System.Drawing.Size(144, 23);
            this.buttonKeyTermSelect.TabIndex = 9;
            this.buttonKeyTermSelect.Text = "Select Key Term(s)";
            this.toolTip.SetToolTip(this.buttonKeyTermSelect, "You can associate one or more biblical key terms with this note so that you can s" +
                    "earch all anchored verses where these terms are used");
            this.buttonKeyTermSelect.UseVisualStyleBackColor = true;
            this.buttonKeyTermSelect.Visible = false;
            this.buttonKeyTermSelect.Click += new System.EventHandler(this.buttonKeyTermSelect_Click);
            // 
            // textBoxKeyTerms
            // 
            this.textBoxKeyTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxKeyTerms.Enabled = false;
            this.textBoxKeyTerms.Location = new System.Drawing.Point(264, 263);
            this.textBoxKeyTerms.Name = "textBoxKeyTerms";
            this.textBoxKeyTerms.ReadOnly = true;
            this.textBoxKeyTerms.Size = new System.Drawing.Size(250, 20);
            this.textBoxKeyTerms.TabIndex = 10;
            this.textBoxKeyTerms.Visible = false;
            // 
            // AddLnCNoteForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(542, 364);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddLnCNoteForm";
            this.Text = "Add L&C Note";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelNationalBT;
        private System.Windows.Forms.TextBox textBoxNationalBT;
        private System.Windows.Forms.Label labelVernacular;
        private System.Windows.Forms.TextBox textBoxVernacular;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.Label labelInternationalBT;
        private System.Windows.Forms.TextBox textBoxInternationalBT;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.TextBox textBoxNotes;
        private System.Windows.Forms.Label labelOptionalKeyTerms;
        private System.Windows.Forms.Button buttonKeyTermSelect;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxKeyTerms;
    }
}