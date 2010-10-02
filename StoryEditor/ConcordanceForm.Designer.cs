namespace OneStoryProjectEditor
{
    partial class ConcordanceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConcordanceForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelVernacular = new System.Windows.Forms.Label();
            this.labelNationalBT = new System.Windows.Forms.Label();
            this.labelInternationalBT = new System.Windows.Forms.Label();
            this.textBoxWordsToSearchForVernacular = new System.Windows.Forms.TextBox();
            this.textBoxWordsToSearchForNationalBT = new System.Windows.Forms.TextBox();
            this.textBoxWordsToSearchForInternationalBT = new System.Windows.Forms.TextBox();
            this.webBrowser = new onlyconnect.HtmlEditor();
            this.progressBarLoadingKeyTerms = new System.Windows.Forms.ProgressBar();
            this.buttonBeginSearch = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Controls.Add(this.labelVernacular, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelNationalBT, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.labelInternationalBT, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordsToSearchForVernacular, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordsToSearchForNationalBT, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordsToSearchForInternationalBT, 3, 1);
            this.tableLayoutPanel.Controls.Add(this.webBrowser, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.progressBarLoadingKeyTerms, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonBeginSearch, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(744, 444);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelVernacular
            // 
            this.labelVernacular.AutoSize = true;
            this.labelVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVernacular.Location = new System.Drawing.Point(84, 0);
            this.labelVernacular.Name = "labelVernacular";
            this.labelVernacular.Size = new System.Drawing.Size(215, 13);
            this.labelVernacular.TabIndex = 7;
            this.labelVernacular.Text = "Vernacular";
            this.labelVernacular.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelNationalBT
            // 
            this.labelNationalBT.AutoSize = true;
            this.labelNationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNationalBT.Location = new System.Drawing.Point(305, 0);
            this.labelNationalBT.Name = "labelNationalBT";
            this.labelNationalBT.Size = new System.Drawing.Size(215, 13);
            this.labelNationalBT.TabIndex = 7;
            this.labelNationalBT.Text = "National BT";
            this.labelNationalBT.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelInternationalBT
            // 
            this.labelInternationalBT.AutoSize = true;
            this.labelInternationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelInternationalBT.Location = new System.Drawing.Point(526, 0);
            this.labelInternationalBT.Name = "labelInternationalBT";
            this.labelInternationalBT.Size = new System.Drawing.Size(215, 13);
            this.labelInternationalBT.TabIndex = 7;
            this.labelInternationalBT.Text = "English";
            this.labelInternationalBT.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxWordsToSearchForVernacular
            // 
            this.textBoxWordsToSearchForVernacular.AcceptsReturn = true;
            this.textBoxWordsToSearchForVernacular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxWordsToSearchForVernacular, resources.GetString("textBoxWordsToSearchForVernacular.HelpString"));
            this.textBoxWordsToSearchForVernacular.Location = new System.Drawing.Point(84, 16);
            this.textBoxWordsToSearchForVernacular.Name = "textBoxWordsToSearchForVernacular";
            this.helpProvider.SetShowHelp(this.textBoxWordsToSearchForVernacular, true);
            this.textBoxWordsToSearchForVernacular.Size = new System.Drawing.Size(215, 20);
            this.textBoxWordsToSearchForVernacular.TabIndex = 0;
            this.textBoxWordsToSearchForVernacular.Text = "word1, word2, \"phrase 3\"";
            this.toolTip.SetToolTip(this.textBoxWordsToSearchForVernacular, "Enter the word(s) to search. Press F1 for further instructions on how to enter da" +
                    "ta in this field.");
            // 
            // textBoxWordsToSearchForNationalBT
            // 
            this.textBoxWordsToSearchForNationalBT.AcceptsReturn = true;
            this.textBoxWordsToSearchForNationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxWordsToSearchForNationalBT, resources.GetString("textBoxWordsToSearchForNationalBT.HelpString"));
            this.textBoxWordsToSearchForNationalBT.Location = new System.Drawing.Point(305, 16);
            this.textBoxWordsToSearchForNationalBT.Name = "textBoxWordsToSearchForNationalBT";
            this.helpProvider.SetShowHelp(this.textBoxWordsToSearchForNationalBT, true);
            this.textBoxWordsToSearchForNationalBT.Size = new System.Drawing.Size(215, 20);
            this.textBoxWordsToSearchForNationalBT.TabIndex = 0;
            this.textBoxWordsToSearchForNationalBT.Text = "word1, word2, \"phrase 3\"";
            this.toolTip.SetToolTip(this.textBoxWordsToSearchForNationalBT, "Enter the word(s) to search. Press F1 for further instructions on how to enter da" +
                    "ta in this field.");
            // 
            // textBoxWordsToSearchForInternationalBT
            // 
            this.textBoxWordsToSearchForInternationalBT.AcceptsReturn = true;
            this.textBoxWordsToSearchForInternationalBT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxWordsToSearchForInternationalBT, resources.GetString("textBoxWordsToSearchForInternationalBT.HelpString"));
            this.textBoxWordsToSearchForInternationalBT.Location = new System.Drawing.Point(526, 16);
            this.textBoxWordsToSearchForInternationalBT.Name = "textBoxWordsToSearchForInternationalBT";
            this.helpProvider.SetShowHelp(this.textBoxWordsToSearchForInternationalBT, true);
            this.textBoxWordsToSearchForInternationalBT.Size = new System.Drawing.Size(215, 20);
            this.textBoxWordsToSearchForInternationalBT.TabIndex = 0;
            this.textBoxWordsToSearchForInternationalBT.Text = "word1, word2, \"phrase 3\"";
            this.toolTip.SetToolTip(this.textBoxWordsToSearchForInternationalBT, "Enter the word(s) to search. Press F1 for further instructions on how to enter da" +
                    "ta in this field.");
            // 
            // webBrowser
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webBrowser, 4);
            this.webBrowser.DefaultComposeSettings.BackColor = System.Drawing.Color.White;
            this.webBrowser.DefaultComposeSettings.DefaultFont = new System.Drawing.Font("Arial", 10F);
            this.webBrowser.DefaultComposeSettings.Enabled = false;
            this.webBrowser.DefaultComposeSettings.ForeColor = System.Drawing.Color.Black;
            this.webBrowser.DefaultPreamble = onlyconnect.EncodingType.UTF8;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.DocumentEncoding = onlyconnect.EncodingType.WindowsCurrent;
            this.helpProvider.SetHelpString(this.webBrowser, "This browser window displays the story lines that contain the word(s) being searc" +
                    "hed for");
            this.webBrowser.IsActivationEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(3, 45);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.webBrowser.SelectionBackColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionBullets = false;
            this.webBrowser.SelectionFont = null;
            this.webBrowser.SelectionForeColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionNumbering = false;
            this.helpProvider.SetShowHelp(this.webBrowser, true);
            this.webBrowser.Size = new System.Drawing.Size(738, 367);
            this.webBrowser.TabIndex = 5;
            this.webBrowser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.webBrowser_KeyPress);
            this.webBrowser.BeforeNavigate += new onlyconnect.BeforeNavigateEventHandler(this.webBrowser_BeforeNavigate);
            // 
            // progressBarLoadingKeyTerms
            // 
            this.tableLayoutPanel.SetColumnSpan(this.progressBarLoadingKeyTerms, 4);
            this.progressBarLoadingKeyTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBarLoadingKeyTerms.Location = new System.Drawing.Point(3, 418);
            this.progressBarLoadingKeyTerms.Name = "progressBarLoadingKeyTerms";
            this.progressBarLoadingKeyTerms.Size = new System.Drawing.Size(738, 23);
            this.progressBarLoadingKeyTerms.TabIndex = 6;
            this.progressBarLoadingKeyTerms.Visible = false;
            // 
            // buttonBeginSearch
            // 
            this.buttonBeginSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpProvider.SetHelpString(this.buttonBeginSearch, "Click here to begin searching");
            this.buttonBeginSearch.Location = new System.Drawing.Point(3, 16);
            this.buttonBeginSearch.Name = "buttonBeginSearch";
            this.helpProvider.SetShowHelp(this.buttonBeginSearch, true);
            this.buttonBeginSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonBeginSearch.TabIndex = 1;
            this.buttonBeginSearch.Text = "&Search";
            this.toolTip.SetToolTip(this.buttonBeginSearch, "Click here to begin searching");
            this.buttonBeginSearch.UseVisualStyleBackColor = true;
            this.buttonBeginSearch.Click += new System.EventHandler(this.buttonBeginSearch_Click);
            // 
            // ConcordanceForm
            // 
            this.AcceptButton = this.buttonBeginSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 468);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConcordanceForm";
            this.Text = "Concordance";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonBeginSearch;
        private onlyconnect.HtmlEditor webBrowser;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.Label labelVernacular;
        private System.Windows.Forms.Label labelNationalBT;
        private System.Windows.Forms.Label labelInternationalBT;
        private System.Windows.Forms.ProgressBar progressBarLoadingKeyTerms;
        private System.Windows.Forms.TextBox textBoxWordsToSearchForNationalBT;
        private System.Windows.Forms.TextBox textBoxWordsToSearchForInternationalBT;
        private System.Windows.Forms.TextBox textBoxWordsToSearchForVernacular;
    }
}