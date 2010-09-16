namespace OneStoryProjectEditor
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxEnableFind = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableReplace = new System.Windows.Forms.CheckBox();
            this.labelFindWhat = new System.Windows.Forms.Label();
            this.comboBoxFindWhat = new System.Windows.Forms.ComboBox();
            this.buttonFindRegExHelper = new System.Windows.Forms.Button();
            this.contextMenuStripExprBuilder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MatchAnyCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nMatchExactlyNTimesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rParagraphCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchAtTheBeginningOfALineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchAtTheEndOfALineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findWhatGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aBMatchesEitherAOrBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.regularExpressionHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelReplaceWith = new System.Windows.Forms.Label();
            this.comboBoxReplaceWith = new System.Windows.Forms.ComboBox();
            this.buttonReplaceRegExHelper = new System.Windows.Forms.Button();
            this.contextMenuStripReplaceWithExprBuilder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.findWhatTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taggedExpression1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taggedExpression2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taggedExpression3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxLookInExpander = new System.Windows.Forms.CheckBox();
            this.labelLookIn = new System.Windows.Forms.Label();
            this.flowLayoutPanelLookIn = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxLookInStoryLanguage = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInEnglishBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInConsultantNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInCoachNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInTestQs = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInTestAs = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInRetellings = new System.Windows.Forms.CheckBox();
            this.checkBoxAllStories = new System.Windows.Forms.CheckBox();
            this.checkBoxUseRegex = new System.Windows.Forms.CheckBox();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.contextMenuStripExprBuilder.SuspendLayout();
            this.contextMenuStripReplaceWithExprBuilder.SuspendLayout();
            this.flowLayoutPanelLookIn.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.checkBoxEnableFind, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.checkBoxEnableReplace, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.labelFindWhat, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.comboBoxFindWhat, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonFindRegExHelper, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.labelReplaceWith, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.comboBoxReplaceWith, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonReplaceRegExHelper, 3, 4);
            this.tableLayoutPanel.Controls.Add(this.checkBoxLookInExpander, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.labelLookIn, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelLookIn, 1, 6);
            this.tableLayoutPanel.Controls.Add(this.buttonFindNext, 1, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonReplace, 2, 7);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(272, 431);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // checkBoxEnableFind
            // 
            this.checkBoxEnableFind.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxEnableFind.AutoSize = true;
            this.checkBoxEnableFind.Checked = true;
            this.checkBoxEnableFind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel.SetColumnSpan(this.checkBoxEnableFind, 2);
            this.checkBoxEnableFind.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxEnableFind.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxEnableFind.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnableFind.Name = "checkBoxEnableFind";
            this.checkBoxEnableFind.Size = new System.Drawing.Size(132, 23);
            this.checkBoxEnableFind.TabIndex = 12;
            this.checkBoxEnableFind.Text = "Find";
            this.checkBoxEnableFind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxEnableFind.UseVisualStyleBackColor = false;
            this.checkBoxEnableFind.CheckedChanged += new System.EventHandler(this.checkBoxEnableFind_CheckedChanged);
            // 
            // checkBoxEnableReplace
            // 
            this.checkBoxEnableReplace.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxEnableReplace.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.checkBoxEnableReplace, 2);
            this.checkBoxEnableReplace.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxEnableReplace.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxEnableReplace.Location = new System.Drawing.Point(141, 3);
            this.checkBoxEnableReplace.Name = "checkBoxEnableReplace";
            this.checkBoxEnableReplace.Size = new System.Drawing.Size(128, 23);
            this.checkBoxEnableReplace.TabIndex = 13;
            this.checkBoxEnableReplace.Text = "Replace";
            this.checkBoxEnableReplace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxEnableReplace.UseVisualStyleBackColor = false;
            this.checkBoxEnableReplace.CheckedChanged += new System.EventHandler(this.checkBoxEnableReplace_CheckedChanged);
            // 
            // labelFindWhat
            // 
            this.labelFindWhat.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelFindWhat, 2);
            this.labelFindWhat.Location = new System.Drawing.Point(3, 29);
            this.labelFindWhat.Name = "labelFindWhat";
            this.labelFindWhat.Size = new System.Drawing.Size(56, 13);
            this.labelFindWhat.TabIndex = 0;
            this.labelFindWhat.Text = "Find what:";
            // 
            // comboBoxFindWhat
            // 
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxFindWhat, 3);
            this.comboBoxFindWhat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxFindWhat.FormattingEnabled = true;
            this.comboBoxFindWhat.Location = new System.Drawing.Point(3, 45);
            this.comboBoxFindWhat.Name = "comboBoxFindWhat";
            this.comboBoxFindWhat.Size = new System.Drawing.Size(238, 21);
            this.comboBoxFindWhat.TabIndex = 1;
            this.comboBoxFindWhat.TextChanged += new System.EventHandler(this.comboBoxFindWhat_TextChanged);
            // 
            // buttonFindRegExHelper
            // 
            this.buttonFindRegExHelper.ContextMenuStrip = this.contextMenuStripExprBuilder;
            this.buttonFindRegExHelper.Location = new System.Drawing.Point(247, 45);
            this.buttonFindRegExHelper.Name = "buttonFindRegExHelper";
            this.buttonFindRegExHelper.Size = new System.Drawing.Size(22, 22);
            this.buttonFindRegExHelper.TabIndex = 2;
            this.buttonFindRegExHelper.Text = ">";
            this.buttonFindRegExHelper.UseVisualStyleBackColor = true;
            this.buttonFindRegExHelper.Visible = false;
            this.buttonFindRegExHelper.Click += new System.EventHandler(this.buttonRegExHelper_Click);
            // 
            // contextMenuStripExprBuilder
            // 
            this.contextMenuStripExprBuilder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MatchAnyCharacterToolStripMenuItem,
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem,
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem,
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem,
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem,
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem,
            this.nMatchExactlyNTimesToolStripMenuItem,
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem,
            this.toolStripSeparator1,
            this.rParagraphCharacterToolStripMenuItem,
            this.matchAtTheBeginningOfALineToolStripMenuItem,
            this.matchAtTheEndOfALineToolStripMenuItem,
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem,
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem,
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem,
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem,
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem,
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem,
            this.toolStripSeparator2,
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem,
            this.findWhatGroupToolStripMenuItem,
            this.aBMatchesEitherAOrBToolStripMenuItem,
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem,
            this.toolStripSeparator3,
            this.regularExpressionHelpToolStripMenuItem});
            this.contextMenuStripExprBuilder.Name = "contextMenuStripExprBuilder";
            this.contextMenuStripExprBuilder.Size = new System.Drawing.Size(351, 506);
            this.contextMenuStripExprBuilder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripExprBuilder_ItemClicked);
            // 
            // MatchAnyCharacterToolStripMenuItem
            // 
            this.MatchAnyCharacterToolStripMenuItem.Name = "MatchAnyCharacterToolStripMenuItem";
            this.MatchAnyCharacterToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.MatchAnyCharacterToolStripMenuItem.Text = ". Match any single character";
            // 
            // match0OrMoreTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Name = "match0OrMoreTimesAsManyAsPossibleToolStripMenuItem";
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Text = "* Match 0 or more times, as many as possible";
            // 
            // match0Or1TimesButPreferOneTimeToolStripMenuItem
            // 
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Name = "match0Or1TimesButPreferOneTimeToolStripMenuItem";
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Text = "? Match 0 or 1 times, but prefer one time";
            // 
            // match0OrMoreTimesAsFewAsPossibleToolStripMenuItem
            // 
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Name = "match0OrMoreTimesAsFewAsPossibleToolStripMenuItem";
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Text = "*? Match 0 or more times, as few as possible";
            // 
            // match1OrMoreTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Name = "match1OrMoreTimesAsManyAsPossibleToolStripMenuItem";
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Text = "+ Match 1 or more times, as many as possible";
            // 
            // match1OrMoreTimesAsFewAsPossibleToolStripMenuItem
            // 
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Name = "match1OrMoreTimesAsFewAsPossibleToolStripMenuItem";
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Text = "+? Match 1 or more times, as few as possible";
            // 
            // nMatchExactlyNTimesToolStripMenuItem
            // 
            this.nMatchExactlyNTimesToolStripMenuItem.Name = "nMatchExactlyNTimesToolStripMenuItem";
            this.nMatchExactlyNTimesToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.nMatchExactlyNTimesToolStripMenuItem.Text = "{n} Match exactly n times";
            // 
            // nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Name = "nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem";
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Text = "{n,m} Match between n and m times, as many as possible";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(347, 6);
            // 
            // rParagraphCharacterToolStripMenuItem
            // 
            this.rParagraphCharacterToolStripMenuItem.Name = "rParagraphCharacterToolStripMenuItem";
            this.rParagraphCharacterToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.rParagraphCharacterToolStripMenuItem.Text = "\\r Matches a carriage return (\\u000D)";
            // 
            // matchAtTheBeginningOfALineToolStripMenuItem
            // 
            this.matchAtTheBeginningOfALineToolStripMenuItem.Name = "matchAtTheBeginningOfALineToolStripMenuItem";
            this.matchAtTheBeginningOfALineToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.matchAtTheBeginningOfALineToolStripMenuItem.Text = "^ Match at the beginning of a line";
            // 
            // matchAtTheEndOfALineToolStripMenuItem
            // 
            this.matchAtTheEndOfALineToolStripMenuItem.Name = "matchAtTheEndOfALineToolStripMenuItem";
            this.matchAtTheEndOfALineToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.matchAtTheEndOfALineToolStripMenuItem.Text = "$ Match at the end of a line";
            // 
            // bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem
            // 
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Name = "bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem";
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Text = "\\b Match if the current position is a word boundary";
            // 
            // bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem
            // 
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Name = "bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem";
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Text = "\\B Match if the current position is not a word boundary";
            // 
            // dMatchAnyNumberOrDecimalDigitToolStripMenuItem
            // 
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Name = "dMatchAnyNumberOrDecimalDigitToolStripMenuItem";
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Text = "\\d Match any number or decimal digit";
            // 
            // dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem
            // 
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Name = "dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem";
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Text = "\\D Match any character that is not a decimal digit";
            // 
            // sMatchAWhiteSpaceCharacterToolStripMenuItem
            // 
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Name = "sMatchAWhiteSpaceCharacterToolStripMenuItem";
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Text = "\\s Match a white space character";
            // 
            // sMatchANonwhiteSpaceCharacterToolStripMenuItem
            // 
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Name = "sMatchANonwhiteSpaceCharacterToolStripMenuItem";
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Text = "\\S Match a non-white space character";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(347, 6);
            // 
            // patternMatchAnyOneCharacterFromTheSetToolStripMenuItem
            // 
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Name = "patternMatchAnyOneCharacterFromTheSetToolStripMenuItem";
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Text = "[pattern] Match any one character from the set";
            // 
            // findWhatGroupToolStripMenuItem
            // 
            this.findWhatGroupToolStripMenuItem.Name = "findWhatGroupToolStripMenuItem";
            this.findWhatGroupToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.findWhatGroupToolStripMenuItem.Text = "() Match groups";
            // 
            // aBMatchesEitherAOrBToolStripMenuItem
            // 
            this.aBMatchesEitherAOrBToolStripMenuItem.Name = "aBMatchesEitherAOrBToolStripMenuItem";
            this.aBMatchesEitherAOrBToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.aBMatchesEitherAOrBToolStripMenuItem.Text = "| \'A|B\' matches either A or B";
            // 
            // uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem
            // 
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Name = "uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem";
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Text = "\\uhhhh Match the character with the hex value hhhh";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(347, 6);
            // 
            // regularExpressionHelpToolStripMenuItem
            // 
            this.regularExpressionHelpToolStripMenuItem.Name = "regularExpressionHelpToolStripMenuItem";
            this.regularExpressionHelpToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.regularExpressionHelpToolStripMenuItem.Text = "Regular Expression &Help";
            this.regularExpressionHelpToolStripMenuItem.Click += new System.EventHandler(this.regularExpressionHelpToolStripMenuItem_Click);
            // 
            // labelReplaceWith
            // 
            this.labelReplaceWith.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelReplaceWith, 2);
            this.labelReplaceWith.Location = new System.Drawing.Point(3, 76);
            this.labelReplaceWith.Name = "labelReplaceWith";
            this.labelReplaceWith.Size = new System.Drawing.Size(72, 13);
            this.labelReplaceWith.TabIndex = 9;
            this.labelReplaceWith.Text = "Replace with:";
            this.labelReplaceWith.Visible = false;
            // 
            // comboBoxReplaceWith
            // 
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxReplaceWith, 3);
            this.comboBoxReplaceWith.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxReplaceWith.FormattingEnabled = true;
            this.comboBoxReplaceWith.Location = new System.Drawing.Point(3, 92);
            this.comboBoxReplaceWith.Name = "comboBoxReplaceWith";
            this.comboBoxReplaceWith.Size = new System.Drawing.Size(238, 21);
            this.comboBoxReplaceWith.TabIndex = 1;
            this.comboBoxReplaceWith.Visible = false;
            // 
            // buttonReplaceRegExHelper
            // 
            this.buttonReplaceRegExHelper.ContextMenuStrip = this.contextMenuStripReplaceWithExprBuilder;
            this.buttonReplaceRegExHelper.Location = new System.Drawing.Point(247, 92);
            this.buttonReplaceRegExHelper.Name = "buttonReplaceRegExHelper";
            this.buttonReplaceRegExHelper.Size = new System.Drawing.Size(22, 22);
            this.buttonReplaceRegExHelper.TabIndex = 15;
            this.buttonReplaceRegExHelper.Text = ">";
            this.buttonReplaceRegExHelper.UseVisualStyleBackColor = true;
            this.buttonReplaceRegExHelper.Visible = false;
            this.buttonReplaceRegExHelper.Click += new System.EventHandler(this.buttonReplaceRegExHelper_Click);
            // 
            // contextMenuStripReplaceWithExprBuilder
            // 
            this.contextMenuStripReplaceWithExprBuilder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findWhatTextToolStripMenuItem,
            this.taggedExpression1ToolStripMenuItem,
            this.taggedExpression2ToolStripMenuItem,
            this.taggedExpression3ToolStripMenuItem});
            this.contextMenuStripReplaceWithExprBuilder.Name = "contextMenuStripReplaceWithExprBuilder";
            this.contextMenuStripReplaceWithExprBuilder.Size = new System.Drawing.Size(153, 92);
            this.contextMenuStripReplaceWithExprBuilder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripReplaceWithExprBuilder_ItemClicked);
            // 
            // findWhatTextToolStripMenuItem
            // 
            this.findWhatTextToolStripMenuItem.Name = "findWhatTextToolStripMenuItem";
            this.findWhatTextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.findWhatTextToolStripMenuItem.Text = "\'&Find What\' Text";
            // 
            // taggedExpression1ToolStripMenuItem
            // 
            this.taggedExpression1ToolStripMenuItem.Name = "taggedExpression1ToolStripMenuItem";
            this.taggedExpression1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.taggedExpression1ToolStripMenuItem.Text = "Match Group &1";
            // 
            // taggedExpression2ToolStripMenuItem
            // 
            this.taggedExpression2ToolStripMenuItem.Name = "taggedExpression2ToolStripMenuItem";
            this.taggedExpression2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.taggedExpression2ToolStripMenuItem.Text = "Match Group &2";
            // 
            // taggedExpression3ToolStripMenuItem
            // 
            this.taggedExpression3ToolStripMenuItem.Name = "taggedExpression3ToolStripMenuItem";
            this.taggedExpression3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.taggedExpression3ToolStripMenuItem.Text = "Match Group &3";
            // 
            // checkBoxLookInExpander
            // 
            this.checkBoxLookInExpander.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxLookInExpander.AutoSize = true;
            this.checkBoxLookInExpander.Checked = true;
            this.checkBoxLookInExpander.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLookInExpander.Location = new System.Drawing.Point(3, 120);
            this.checkBoxLookInExpander.Name = "checkBoxLookInExpander";
            this.checkBoxLookInExpander.Size = new System.Drawing.Size(26, 23);
            this.checkBoxLookInExpander.TabIndex = 14;
            this.checkBoxLookInExpander.Text = " +";
            this.checkBoxLookInExpander.UseMnemonic = false;
            this.checkBoxLookInExpander.UseVisualStyleBackColor = false;
            this.checkBoxLookInExpander.CheckedChanged += new System.EventHandler(this.checkBoxLookInExpander_CheckedChanged);
            // 
            // labelLookIn
            // 
            this.labelLookIn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelLookIn.AutoSize = true;
            this.labelLookIn.Location = new System.Drawing.Point(35, 125);
            this.labelLookIn.Name = "labelLookIn";
            this.labelLookIn.Size = new System.Drawing.Size(45, 13);
            this.labelLookIn.TabIndex = 3;
            this.labelLookIn.Text = "Look in:";
            // 
            // flowLayoutPanelLookIn
            // 
            this.flowLayoutPanelLookIn.AutoSize = true;
            this.flowLayoutPanelLookIn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanelLookIn, 2);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInStoryLanguage);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInNationalBT);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInEnglishBT);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInConsultantNotes);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInCoachNotes);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInTestQs);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInTestAs);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInRetellings);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxAllStories);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxUseRegex);
            this.flowLayoutPanelLookIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelLookIn.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelLookIn.Location = new System.Drawing.Point(35, 149);
            this.flowLayoutPanelLookIn.Name = "flowLayoutPanelLookIn";
            this.flowLayoutPanelLookIn.Size = new System.Drawing.Size(206, 250);
            this.flowLayoutPanelLookIn.TabIndex = 7;
            this.flowLayoutPanelLookIn.WrapContents = false;
            // 
            // checkBoxLookInStoryLanguage
            // 
            this.checkBoxLookInStoryLanguage.AutoSize = true;
            this.checkBoxLookInStoryLanguage.Location = new System.Drawing.Point(3, 3);
            this.checkBoxLookInStoryLanguage.Name = "checkBoxLookInStoryLanguage";
            this.checkBoxLookInStoryLanguage.Size = new System.Drawing.Size(101, 17);
            this.checkBoxLookInStoryLanguage.TabIndex = 7;
            this.checkBoxLookInStoryLanguage.Text = "&Story Language";
            this.checkBoxLookInStoryLanguage.UseVisualStyleBackColor = true;
            this.checkBoxLookInStoryLanguage.CheckedChanged += new System.EventHandler(this.checkBoxLookInStoryLanguage_CheckedChanged);
            // 
            // checkBoxLookInNationalBT
            // 
            this.checkBoxLookInNationalBT.AutoSize = true;
            this.checkBoxLookInNationalBT.Location = new System.Drawing.Point(3, 26);
            this.checkBoxLookInNationalBT.Name = "checkBoxLookInNationalBT";
            this.checkBoxLookInNationalBT.Size = new System.Drawing.Size(133, 17);
            this.checkBoxLookInNationalBT.TabIndex = 8;
            this.checkBoxLookInNationalBT.Text = "&National Language BT";
            this.checkBoxLookInNationalBT.UseVisualStyleBackColor = true;
            this.checkBoxLookInNationalBT.CheckedChanged += new System.EventHandler(this.checkBoxLookInNationalBT_CheckedChanged);
            // 
            // checkBoxLookInEnglishBT
            // 
            this.checkBoxLookInEnglishBT.AutoSize = true;
            this.checkBoxLookInEnglishBT.Location = new System.Drawing.Point(3, 49);
            this.checkBoxLookInEnglishBT.Name = "checkBoxLookInEnglishBT";
            this.checkBoxLookInEnglishBT.Size = new System.Drawing.Size(77, 17);
            this.checkBoxLookInEnglishBT.TabIndex = 9;
            this.checkBoxLookInEnglishBT.Text = "&English BT";
            this.checkBoxLookInEnglishBT.UseVisualStyleBackColor = true;
            this.checkBoxLookInEnglishBT.CheckedChanged += new System.EventHandler(this.checkBoxLookInEnglishBT_CheckedChanged);
            // 
            // checkBoxLookInConsultantNotes
            // 
            this.checkBoxLookInConsultantNotes.AutoSize = true;
            this.checkBoxLookInConsultantNotes.Location = new System.Drawing.Point(3, 72);
            this.checkBoxLookInConsultantNotes.Name = "checkBoxLookInConsultantNotes";
            this.checkBoxLookInConsultantNotes.Size = new System.Drawing.Size(107, 17);
            this.checkBoxLookInConsultantNotes.TabIndex = 10;
            this.checkBoxLookInConsultantNotes.Text = "&Consultant Notes";
            this.checkBoxLookInConsultantNotes.UseVisualStyleBackColor = true;
            this.checkBoxLookInConsultantNotes.CheckedChanged += new System.EventHandler(this.checkBoxLookInConsultantNotes_CheckedChanged);
            // 
            // checkBoxLookInCoachNotes
            // 
            this.checkBoxLookInCoachNotes.AutoSize = true;
            this.checkBoxLookInCoachNotes.Location = new System.Drawing.Point(3, 95);
            this.checkBoxLookInCoachNotes.Name = "checkBoxLookInCoachNotes";
            this.checkBoxLookInCoachNotes.Size = new System.Drawing.Size(88, 17);
            this.checkBoxLookInCoachNotes.TabIndex = 11;
            this.checkBoxLookInCoachNotes.Text = "C&oach Notes";
            this.checkBoxLookInCoachNotes.UseVisualStyleBackColor = true;
            this.checkBoxLookInCoachNotes.CheckedChanged += new System.EventHandler(this.checkBoxLookInCoachNotes_CheckedChanged);
            // 
            // checkBoxLookInTestQs
            // 
            this.checkBoxLookInTestQs.AutoSize = true;
            this.checkBoxLookInTestQs.Location = new System.Drawing.Point(3, 118);
            this.checkBoxLookInTestQs.Name = "checkBoxLookInTestQs";
            this.checkBoxLookInTestQs.Size = new System.Drawing.Size(111, 17);
            this.checkBoxLookInTestQs.TabIndex = 13;
            this.checkBoxLookInTestQs.Text = "&Testing Questions";
            this.checkBoxLookInTestQs.UseVisualStyleBackColor = true;
            this.checkBoxLookInTestQs.CheckedChanged += new System.EventHandler(this.checkBoxLookInTestQs_CheckedChanged);
            // 
            // checkBoxLookInTestAs
            // 
            this.checkBoxLookInTestAs.AutoSize = true;
            this.checkBoxLookInTestAs.Location = new System.Drawing.Point(3, 141);
            this.checkBoxLookInTestAs.Name = "checkBoxLookInTestAs";
            this.checkBoxLookInTestAs.Size = new System.Drawing.Size(104, 17);
            this.checkBoxLookInTestAs.TabIndex = 16;
            this.checkBoxLookInTestAs.Text = "&Testing Answers";
            this.checkBoxLookInTestAs.UseVisualStyleBackColor = true;
            this.checkBoxLookInTestAs.CheckedChanged += new System.EventHandler(this.checkBoxLookInTestAs_CheckedChanged);
            // 
            // checkBoxLookInRetellings
            // 
            this.checkBoxLookInRetellings.AutoSize = true;
            this.checkBoxLookInRetellings.Location = new System.Drawing.Point(3, 164);
            this.checkBoxLookInRetellings.Name = "checkBoxLookInRetellings";
            this.checkBoxLookInRetellings.Size = new System.Drawing.Size(72, 17);
            this.checkBoxLookInRetellings.TabIndex = 12;
            this.checkBoxLookInRetellings.Text = "&Retellings";
            this.checkBoxLookInRetellings.UseVisualStyleBackColor = true;
            this.checkBoxLookInRetellings.CheckedChanged += new System.EventHandler(this.checkBoxLookInRetellings_CheckedChanged);
            // 
            // checkBoxAllStories
            // 
            this.checkBoxAllStories.AutoSize = true;
            this.checkBoxAllStories.Location = new System.Drawing.Point(3, 187);
            this.checkBoxAllStories.Name = "checkBoxAllStories";
            this.checkBoxAllStories.Size = new System.Drawing.Size(72, 17);
            this.checkBoxAllStories.TabIndex = 14;
            this.checkBoxAllStories.Text = "A&ll Stories";
            this.checkBoxAllStories.UseVisualStyleBackColor = true;
            this.checkBoxAllStories.CheckedChanged += new System.EventHandler(this.checkBoxAllStories_CheckedChanged);
            // 
            // checkBoxUseRegex
            // 
            this.checkBoxUseRegex.AutoSize = true;
            this.checkBoxUseRegex.Location = new System.Drawing.Point(3, 210);
            this.checkBoxUseRegex.Name = "checkBoxUseRegex";
            this.checkBoxUseRegex.Size = new System.Drawing.Size(143, 17);
            this.checkBoxUseRegex.TabIndex = 15;
            this.checkBoxUseRegex.Text = "&Use Regular expressions";
            this.checkBoxUseRegex.UseVisualStyleBackColor = true;
            this.checkBoxUseRegex.CheckedChanged += new System.EventHandler(this.checkBoxUseRegex_CheckedChanged);
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindNext.Location = new System.Drawing.Point(35, 405);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(100, 23);
            this.buttonFindNext.TabIndex = 6;
            this.buttonFindNext.Text = "&Find Next";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonReplace.Enabled = false;
            this.buttonReplace.Location = new System.Drawing.Point(141, 405);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(100, 23);
            this.buttonReplace.TabIndex = 16;
            this.buttonReplace.Text = "&Replace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Visible = false;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.buttonFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 455);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchForm";
            this.Text = "Find";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.contextMenuStripExprBuilder.ResumeLayout(false);
            this.contextMenuStripReplaceWithExprBuilder.ResumeLayout(false);
            this.flowLayoutPanelLookIn.ResumeLayout(false);
            this.flowLayoutPanelLookIn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelFindWhat;
        private System.Windows.Forms.ComboBox comboBoxFindWhat;
        private System.Windows.Forms.Button buttonFindRegExHelper;
        private System.Windows.Forms.Label labelLookIn;
        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLookIn;
        internal System.Windows.Forms.CheckBox checkBoxLookInTestQs;
        internal System.Windows.Forms.CheckBox checkBoxLookInRetellings;
        internal System.Windows.Forms.CheckBox checkBoxLookInCoachNotes;
        internal System.Windows.Forms.CheckBox checkBoxLookInConsultantNotes;
        internal System.Windows.Forms.CheckBox checkBoxLookInEnglishBT;
        internal System.Windows.Forms.CheckBox checkBoxLookInNationalBT;
        internal System.Windows.Forms.CheckBox checkBoxLookInStoryLanguage;
        internal System.Windows.Forms.CheckBox checkBoxAllStories;
        private System.Windows.Forms.CheckBox checkBoxUseRegex;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripExprBuilder;
        private System.Windows.Forms.ToolStripMenuItem MatchAnyCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem match0OrMoreTimesAsManyAsPossibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem match0Or1TimesButPreferOneTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem match0OrMoreTimesAsFewAsPossibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem match1OrMoreTimesAsManyAsPossibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem match1OrMoreTimesAsFewAsPossibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nMatchExactlyNTimesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem rParagraphCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchAtTheBeginningOfALineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchAtTheEndOfALineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dMatchAnyNumberOrDecimalDigitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sMatchAWhiteSpaceCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sMatchANonwhiteSpaceCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem patternMatchAnyOneCharacterFromTheSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aBMatchesEitherAOrBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem regularExpressionHelpToolStripMenuItem;
        private System.Windows.Forms.Label labelReplaceWith;
        private System.Windows.Forms.ComboBox comboBoxReplaceWith;
        private System.Windows.Forms.CheckBox checkBoxEnableFind;
        private System.Windows.Forms.CheckBox checkBoxEnableReplace;
        private System.Windows.Forms.CheckBox checkBoxLookInExpander;
        private System.Windows.Forms.Button buttonReplaceRegExHelper;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripReplaceWithExprBuilder;
        private System.Windows.Forms.ToolStripMenuItem findWhatTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taggedExpression1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taggedExpression2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taggedExpression3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findWhatGroupToolStripMenuItem;
        internal System.Windows.Forms.CheckBox checkBoxLookInTestAs;
    }
}