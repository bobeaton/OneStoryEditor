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
            this.progressBarLoadingKeyTerms = new System.Windows.Forms.ProgressBar();
            this.textBoxWordsToSearchFor = new System.Windows.Forms.TextBox();
            this.lableWordsToSearchFor = new System.Windows.Forms.Label();
            this.buttonBeginSearch = new System.Windows.Forms.Button();
            this.webBrowser = new onlyconnect.HtmlEditor();
            this.flowLayoutPanelLanguageChoice = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonSearchVernacular = new System.Windows.Forms.RadioButton();
            this.radioButtonSearchNationalBT = new System.Windows.Forms.RadioButton();
            this.radioButtonSearchInternationalBT = new System.Windows.Forms.RadioButton();
            this.labelLanguageToSearchIn = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel.SuspendLayout();
            this.flowLayoutPanelLanguageChoice.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.progressBarLoadingKeyTerms, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordsToSearchFor, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.lableWordsToSearchFor, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonBeginSearch, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.webBrowser, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelLanguageChoice, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelLanguageToSearchIn, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(744, 444);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // progressBarLoadingKeyTerms
            // 
            this.tableLayoutPanel.SetColumnSpan(this.progressBarLoadingKeyTerms, 2);
            this.progressBarLoadingKeyTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBarLoadingKeyTerms.Location = new System.Drawing.Point(3, 418);
            this.progressBarLoadingKeyTerms.Name = "progressBarLoadingKeyTerms";
            this.progressBarLoadingKeyTerms.Size = new System.Drawing.Size(738, 23);
            this.progressBarLoadingKeyTerms.TabIndex = 6;
            this.progressBarLoadingKeyTerms.Visible = false;
            // 
            // textBoxWordsToSearchFor
            // 
            this.textBoxWordsToSearchFor.AcceptsReturn = true;
            this.textBoxWordsToSearchFor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxWordsToSearchFor, resources.GetString("textBoxWordsToSearchFor.HelpString"));
            this.textBoxWordsToSearchFor.Location = new System.Drawing.Point(125, 34);
            this.textBoxWordsToSearchFor.Multiline = true;
            this.textBoxWordsToSearchFor.Name = "textBoxWordsToSearchFor";
            this.tableLayoutPanel.SetRowSpan(this.textBoxWordsToSearchFor, 2);
            this.helpProvider.SetShowHelp(this.textBoxWordsToSearchFor, true);
            this.textBoxWordsToSearchFor.Size = new System.Drawing.Size(616, 91);
            this.textBoxWordsToSearchFor.TabIndex = 0;
            this.textBoxWordsToSearchFor.Text = "word1, word2, \"phrase 3\"";
            this.toolTip.SetToolTip(this.textBoxWordsToSearchFor, "Enter the word(s) to search. Press F1 for further instructions on how to enter da" +
                    "ta in this field.");
            // 
            // lableWordsToSearchFor
            // 
            this.lableWordsToSearchFor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lableWordsToSearchFor.AutoSize = true;
            this.lableWordsToSearchFor.Location = new System.Drawing.Point(3, 49);
            this.lableWordsToSearchFor.Name = "lableWordsToSearchFor";
            this.lableWordsToSearchFor.Size = new System.Drawing.Size(109, 13);
            this.lableWordsToSearchFor.TabIndex = 2;
            this.lableWordsToSearchFor.Text = "Word(s) to search for:";
            // 
            // buttonBeginSearch
            // 
            this.buttonBeginSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpProvider.SetHelpString(this.buttonBeginSearch, "Click here to begin searching");
            this.buttonBeginSearch.Location = new System.Drawing.Point(44, 93);
            this.buttonBeginSearch.Name = "buttonBeginSearch";
            this.helpProvider.SetShowHelp(this.buttonBeginSearch, true);
            this.buttonBeginSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonBeginSearch.TabIndex = 1;
            this.buttonBeginSearch.Text = "&Search";
            this.toolTip.SetToolTip(this.buttonBeginSearch, "Click here to begin searching");
            this.buttonBeginSearch.UseVisualStyleBackColor = true;
            this.buttonBeginSearch.Click += new System.EventHandler(this.buttonBeginSearch_Click);
            // 
            // webBrowser
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webBrowser, 2);
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
            this.webBrowser.Location = new System.Drawing.Point(3, 131);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.webBrowser.SelectionBackColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionBullets = false;
            this.webBrowser.SelectionFont = null;
            this.webBrowser.SelectionForeColor = System.Drawing.Color.Empty;
            this.webBrowser.SelectionNumbering = false;
            this.helpProvider.SetShowHelp(this.webBrowser, true);
            this.webBrowser.Size = new System.Drawing.Size(738, 281);
            this.webBrowser.TabIndex = 5;
            this.webBrowser.BeforeNavigate += new onlyconnect.BeforeNavigateEventHandler(this.webBrowser_BeforeNavigate);
            // 
            // flowLayoutPanelLanguageChoice
            // 
            this.flowLayoutPanelLanguageChoice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanelLanguageChoice.Controls.Add(this.radioButtonSearchVernacular);
            this.flowLayoutPanelLanguageChoice.Controls.Add(this.radioButtonSearchNationalBT);
            this.flowLayoutPanelLanguageChoice.Controls.Add(this.radioButtonSearchInternationalBT);
            this.flowLayoutPanelLanguageChoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelLanguageChoice.Location = new System.Drawing.Point(125, 3);
            this.flowLayoutPanelLanguageChoice.Name = "flowLayoutPanelLanguageChoice";
            this.flowLayoutPanelLanguageChoice.Size = new System.Drawing.Size(616, 25);
            this.flowLayoutPanelLanguageChoice.TabIndex = 1;
            // 
            // radioButtonSearchVernacular
            // 
            this.radioButtonSearchVernacular.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonSearchVernacular, "Select to search in the language of the stories");
            this.radioButtonSearchVernacular.Location = new System.Drawing.Point(3, 3);
            this.radioButtonSearchVernacular.Name = "radioButtonSearchVernacular";
            this.helpProvider.SetShowHelp(this.radioButtonSearchVernacular, true);
            this.radioButtonSearchVernacular.Size = new System.Drawing.Size(76, 17);
            this.radioButtonSearchVernacular.TabIndex = 0;
            this.radioButtonSearchVernacular.TabStop = true;
            this.radioButtonSearchVernacular.Text = "Vernacular";
            this.toolTip.SetToolTip(this.radioButtonSearchVernacular, "Select to search in the Vernacular language fields");
            this.radioButtonSearchVernacular.UseVisualStyleBackColor = true;
            this.radioButtonSearchVernacular.CheckedChanged += new System.EventHandler(this.radioButtonSearchVernacular_CheckedChanged);
            // 
            // radioButtonSearchNationalBT
            // 
            this.radioButtonSearchNationalBT.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonSearchNationalBT, "Select to search in the National language BT fields");
            this.radioButtonSearchNationalBT.Location = new System.Drawing.Point(85, 3);
            this.radioButtonSearchNationalBT.Name = "radioButtonSearchNationalBT";
            this.helpProvider.SetShowHelp(this.radioButtonSearchNationalBT, true);
            this.radioButtonSearchNationalBT.Size = new System.Drawing.Size(81, 17);
            this.radioButtonSearchNationalBT.TabIndex = 1;
            this.radioButtonSearchNationalBT.TabStop = true;
            this.radioButtonSearchNationalBT.Text = "National BT";
            this.toolTip.SetToolTip(this.radioButtonSearchNationalBT, "Select to search in the National language BT fields");
            this.radioButtonSearchNationalBT.UseVisualStyleBackColor = true;
            this.radioButtonSearchNationalBT.CheckedChanged += new System.EventHandler(this.radioButtonSearchNationalBT_CheckedChanged);
            // 
            // radioButtonSearchInternationalBT
            // 
            this.radioButtonSearchInternationalBT.AutoSize = true;
            this.helpProvider.SetHelpString(this.radioButtonSearchInternationalBT, "Select to search in the English language BT fields");
            this.radioButtonSearchInternationalBT.Location = new System.Drawing.Point(172, 3);
            this.radioButtonSearchInternationalBT.Name = "radioButtonSearchInternationalBT";
            this.helpProvider.SetShowHelp(this.radioButtonSearchInternationalBT, true);
            this.radioButtonSearchInternationalBT.Size = new System.Drawing.Size(76, 17);
            this.radioButtonSearchInternationalBT.TabIndex = 2;
            this.radioButtonSearchInternationalBT.TabStop = true;
            this.radioButtonSearchInternationalBT.Text = "English BT";
            this.toolTip.SetToolTip(this.radioButtonSearchInternationalBT, "Select to search in the English language BT fields");
            this.radioButtonSearchInternationalBT.UseVisualStyleBackColor = true;
            this.radioButtonSearchInternationalBT.CheckedChanged += new System.EventHandler(this.radioButtonSearchInternationalBT_CheckedChanged);
            // 
            // labelLanguageToSearchIn
            // 
            this.labelLanguageToSearchIn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelLanguageToSearchIn.AutoSize = true;
            this.labelLanguageToSearchIn.Location = new System.Drawing.Point(3, 9);
            this.labelLanguageToSearchIn.Name = "labelLanguageToSearchIn";
            this.labelLanguageToSearchIn.Size = new System.Drawing.Size(116, 13);
            this.labelLanguageToSearchIn.TabIndex = 0;
            this.labelLanguageToSearchIn.Text = "Language to search in:";
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
            this.flowLayoutPanelLanguageChoice.ResumeLayout(false);
            this.flowLayoutPanelLanguageChoice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label lableWordsToSearchFor;
        private System.Windows.Forms.TextBox textBoxWordsToSearchFor;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonBeginSearch;
        private onlyconnect.HtmlEditor webBrowser;
        private System.Windows.Forms.ProgressBar progressBarLoadingKeyTerms;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLanguageChoice;
        private System.Windows.Forms.RadioButton radioButtonSearchVernacular;
        private System.Windows.Forms.RadioButton radioButtonSearchNationalBT;
        private System.Windows.Forms.RadioButton radioButtonSearchInternationalBT;
        private System.Windows.Forms.Label labelLanguageToSearchIn;
        private System.Windows.Forms.HelpProvider helpProvider;
    }
}