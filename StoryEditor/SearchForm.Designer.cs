﻿namespace OneStoryProjectEditor
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRegExHelper = new System.Windows.Forms.Button();
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
            this.aBMatchesEitherAOrBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.regularExpressionHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxFindWhat = new System.Windows.Forms.ComboBox();
            this.buttonLookInExpander = new System.Windows.Forms.Button();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.flowLayoutPanelLookIn = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxLookInStoryLanguage = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInNationalBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInEnglishBT = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInConsultantNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInCoachNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInTestQnA = new System.Windows.Forms.CheckBox();
            this.checkBoxLookInRetellings = new System.Windows.Forms.CheckBox();
            this.checkBoxAllStories = new System.Windows.Forms.CheckBox();
            this.checkBoxUseRegex = new System.Windows.Forms.CheckBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStripExprBuilder.SuspendLayout();
            this.flowLayoutPanelLookIn.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.buttonRegExHelper, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxFindWhat, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonLookInExpander, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindNext, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanelLookIn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelStatus, 0, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 366);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonRegExHelper
            // 
            this.buttonRegExHelper.ContextMenuStrip = this.contextMenuStripExprBuilder;
            this.buttonRegExHelper.Enabled = false;
            this.buttonRegExHelper.Location = new System.Drawing.Point(216, 16);
            this.buttonRegExHelper.Name = "buttonRegExHelper";
            this.buttonRegExHelper.Size = new System.Drawing.Size(23, 21);
            this.buttonRegExHelper.TabIndex = 2;
            this.buttonRegExHelper.Text = ">";
            this.buttonRegExHelper.UseVisualStyleBackColor = true;
            this.buttonRegExHelper.Click += new System.EventHandler(this.buttonRegExHelper_Click);
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
            this.aBMatchesEitherAOrBToolStripMenuItem,
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem,
            this.toolStripSeparator3,
            this.regularExpressionHelpToolStripMenuItem});
            this.contextMenuStripExprBuilder.Name = "contextMenuStripExprBuilder";
            this.contextMenuStripExprBuilder.Size = new System.Drawing.Size(378, 484);
            this.contextMenuStripExprBuilder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripExprBuilder_ItemClicked);
            // 
            // MatchAnyCharacterToolStripMenuItem
            // 
            this.MatchAnyCharacterToolStripMenuItem.Name = "MatchAnyCharacterToolStripMenuItem";
            this.MatchAnyCharacterToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.MatchAnyCharacterToolStripMenuItem.Text = ". Match any single character";
            // 
            // match0OrMoreTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Name = "match0OrMoreTimesAsManyAsPossibleToolStripMenuItem";
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.match0OrMoreTimesAsManyAsPossibleToolStripMenuItem.Text = "* Match 0 or more times, as many as possible";
            // 
            // match0Or1TimesButPreferOneTimeToolStripMenuItem
            // 
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Name = "match0Or1TimesButPreferOneTimeToolStripMenuItem";
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.match0Or1TimesButPreferOneTimeToolStripMenuItem.Text = "? Match 0 or 1 times, but prefer one time";
            // 
            // match0OrMoreTimesAsFewAsPossibleToolStripMenuItem
            // 
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Name = "match0OrMoreTimesAsFewAsPossibleToolStripMenuItem";
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.match0OrMoreTimesAsFewAsPossibleToolStripMenuItem.Text = "*? Match 0 or more times, as few as possible";
            // 
            // match1OrMoreTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Name = "match1OrMoreTimesAsManyAsPossibleToolStripMenuItem";
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.match1OrMoreTimesAsManyAsPossibleToolStripMenuItem.Text = "+ Match 1 or more times, as many as possible";
            // 
            // match1OrMoreTimesAsFewAsPossibleToolStripMenuItem
            // 
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Name = "match1OrMoreTimesAsFewAsPossibleToolStripMenuItem";
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.match1OrMoreTimesAsFewAsPossibleToolStripMenuItem.Text = "+? Match 1 or more times, as few as possible";
            // 
            // nMatchExactlyNTimesToolStripMenuItem
            // 
            this.nMatchExactlyNTimesToolStripMenuItem.Name = "nMatchExactlyNTimesToolStripMenuItem";
            this.nMatchExactlyNTimesToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.nMatchExactlyNTimesToolStripMenuItem.Text = "{n} Match exactly n times";
            // 
            // nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem
            // 
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Name = "nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem";
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.nmMatchBetweenNAndMTimesAsManyAsPossibleToolStripMenuItem.Text = "{n,m} Match between n and m times, as many as possible";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(374, 6);
            // 
            // rParagraphCharacterToolStripMenuItem
            // 
            this.rParagraphCharacterToolStripMenuItem.Name = "rParagraphCharacterToolStripMenuItem";
            this.rParagraphCharacterToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.rParagraphCharacterToolStripMenuItem.Text = "\\r Matches a carriage return (\\u000D)";
            // 
            // matchAtTheBeginningOfALineToolStripMenuItem
            // 
            this.matchAtTheBeginningOfALineToolStripMenuItem.Name = "matchAtTheBeginningOfALineToolStripMenuItem";
            this.matchAtTheBeginningOfALineToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.matchAtTheBeginningOfALineToolStripMenuItem.Text = "^ Match at the beginning of a line";
            // 
            // matchAtTheEndOfALineToolStripMenuItem
            // 
            this.matchAtTheEndOfALineToolStripMenuItem.Name = "matchAtTheEndOfALineToolStripMenuItem";
            this.matchAtTheEndOfALineToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.matchAtTheEndOfALineToolStripMenuItem.Text = "$ Match at the end of a line";
            // 
            // bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem
            // 
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Name = "bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem";
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.bMatchIfTheCurrentPositionIsAWordBoundaryToolStripMenuItem.Text = "\\b Match if the current position is a word boundary";
            // 
            // bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem
            // 
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Name = "bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem";
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.bMatchIfTheCurrentPositionIsNotAWordBoundaryToolStripMenuItem.Text = "\\B Match if the current position is not a word boundary";
            // 
            // dMatchAnyNumberOrDecimalDigitToolStripMenuItem
            // 
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Name = "dMatchAnyNumberOrDecimalDigitToolStripMenuItem";
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.dMatchAnyNumberOrDecimalDigitToolStripMenuItem.Text = "\\d Match any number or decimal digit";
            // 
            // dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem
            // 
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Name = "dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem";
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.dMatchAnyCharacterThatIsNotADecimalDigitToolStripMenuItem.Text = "\\D Match any character that is not a decimal digit";
            // 
            // sMatchAWhiteSpaceCharacterToolStripMenuItem
            // 
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Name = "sMatchAWhiteSpaceCharacterToolStripMenuItem";
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.sMatchAWhiteSpaceCharacterToolStripMenuItem.Text = "\\s Match a white space character";
            // 
            // sMatchANonwhiteSpaceCharacterToolStripMenuItem
            // 
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Name = "sMatchANonwhiteSpaceCharacterToolStripMenuItem";
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.sMatchANonwhiteSpaceCharacterToolStripMenuItem.Text = "\\S Match a non-white space character";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(374, 6);
            // 
            // patternMatchAnyOneCharacterFromTheSetToolStripMenuItem
            // 
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Name = "patternMatchAnyOneCharacterFromTheSetToolStripMenuItem";
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.patternMatchAnyOneCharacterFromTheSetToolStripMenuItem.Text = "[pattern] Match any one character from the set";
            // 
            // aBMatchesEitherAOrBToolStripMenuItem
            // 
            this.aBMatchesEitherAOrBToolStripMenuItem.Name = "aBMatchesEitherAOrBToolStripMenuItem";
            this.aBMatchesEitherAOrBToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.aBMatchesEitherAOrBToolStripMenuItem.Text = "| \'A|B\' matches either A or B";
            // 
            // uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem
            // 
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Name = "uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem";
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.uhhhhMatchTheCharacterWithTheHexValueHhhhToolStripMenuItem.Text = "\\uhhhh Match the character with the hex value hhhh";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(374, 6);
            // 
            // regularExpressionHelpToolStripMenuItem
            // 
            this.regularExpressionHelpToolStripMenuItem.Name = "regularExpressionHelpToolStripMenuItem";
            this.regularExpressionHelpToolStripMenuItem.Size = new System.Drawing.Size(377, 22);
            this.regularExpressionHelpToolStripMenuItem.Text = "Regular Expression &Help";
            this.regularExpressionHelpToolStripMenuItem.Click += new System.EventHandler(this.regularExpressionHelpToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Look in:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // comboBoxFindWhat
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxFindWhat, 2);
            this.comboBoxFindWhat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxFindWhat.FormattingEnabled = true;
            this.comboBoxFindWhat.Location = new System.Drawing.Point(3, 16);
            this.comboBoxFindWhat.Name = "comboBoxFindWhat";
            this.comboBoxFindWhat.Size = new System.Drawing.Size(207, 21);
            this.comboBoxFindWhat.TabIndex = 1;
            // 
            // buttonLookInExpander
            // 
            this.buttonLookInExpander.Location = new System.Drawing.Point(3, 56);
            this.buttonLookInExpander.Name = "buttonLookInExpander";
            this.buttonLookInExpander.Size = new System.Drawing.Size(23, 23);
            this.buttonLookInExpander.TabIndex = 4;
            this.buttonLookInExpander.Text = "+";
            this.buttonLookInExpander.UseVisualStyleBackColor = true;
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonFindNext.Location = new System.Drawing.Point(74, 325);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(94, 23);
            this.buttonFindNext.TabIndex = 6;
            this.buttonFindNext.Text = "&Find Next";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // flowLayoutPanelLookIn
            // 
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInStoryLanguage);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInNationalBT);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInEnglishBT);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInConsultantNotes);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInCoachNotes);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInTestQnA);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxLookInRetellings);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxAllStories);
            this.flowLayoutPanelLookIn.Controls.Add(this.checkBoxUseRegex);
            this.flowLayoutPanelLookIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelLookIn.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelLookIn.Location = new System.Drawing.Point(32, 85);
            this.flowLayoutPanelLookIn.Name = "flowLayoutPanelLookIn";
            this.flowLayoutPanelLookIn.Size = new System.Drawing.Size(178, 234);
            this.flowLayoutPanelLookIn.TabIndex = 7;
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
            // checkBoxLookInTestQnA
            // 
            this.checkBoxLookInTestQnA.AutoSize = true;
            this.checkBoxLookInTestQnA.Location = new System.Drawing.Point(3, 118);
            this.checkBoxLookInTestQnA.Name = "checkBoxLookInTestQnA";
            this.checkBoxLookInTestQnA.Size = new System.Drawing.Size(163, 17);
            this.checkBoxLookInTestQnA.TabIndex = 13;
            this.checkBoxLookInTestQnA.Text = "&Testing Questions && Answers";
            this.checkBoxLookInTestQnA.UseVisualStyleBackColor = true;
            this.checkBoxLookInTestQnA.CheckedChanged += new System.EventHandler(this.checkBoxLookInTestQnA_CheckedChanged);
            // 
            // checkBoxLookInRetellings
            // 
            this.checkBoxLookInRetellings.AutoSize = true;
            this.checkBoxLookInRetellings.Location = new System.Drawing.Point(3, 141);
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
            this.checkBoxAllStories.Location = new System.Drawing.Point(3, 164);
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
            this.checkBoxUseRegex.Location = new System.Drawing.Point(3, 187);
            this.checkBoxUseRegex.Name = "checkBoxUseRegex";
            this.checkBoxUseRegex.Size = new System.Drawing.Size(143, 17);
            this.checkBoxUseRegex.TabIndex = 15;
            this.checkBoxUseRegex.Text = "&Use Regular expressions";
            this.checkBoxUseRegex.UseVisualStyleBackColor = true;
            this.checkBoxUseRegex.CheckedChanged += new System.EventHandler(this.checkBoxUseRegex_CheckedChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this.labelStatus, 3);
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.Location = new System.Drawing.Point(3, 351);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(236, 15);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "Click Find";
            // 
            // SearchForm
            // 
            this.AcceptButton = this.buttonFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 390);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchForm";
            this.Text = "Find";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuStripExprBuilder.ResumeLayout(false);
            this.flowLayoutPanelLookIn.ResumeLayout(false);
            this.flowLayoutPanelLookIn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxFindWhat;
        private System.Windows.Forms.Button buttonRegExHelper;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonLookInExpander;
        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLookIn;
        internal System.Windows.Forms.CheckBox checkBoxLookInTestQnA;
        internal System.Windows.Forms.CheckBox checkBoxLookInRetellings;
        internal System.Windows.Forms.CheckBox checkBoxLookInCoachNotes;
        internal System.Windows.Forms.CheckBox checkBoxLookInConsultantNotes;
        internal System.Windows.Forms.CheckBox checkBoxLookInEnglishBT;
        internal System.Windows.Forms.CheckBox checkBoxLookInNationalBT;
        internal System.Windows.Forms.CheckBox checkBoxLookInStoryLanguage;
        internal System.Windows.Forms.CheckBox checkBoxAllStories;
        private System.Windows.Forms.Label labelStatus;
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
    }
}