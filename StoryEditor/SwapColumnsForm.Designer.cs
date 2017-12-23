namespace OneStoryProjectEditor
{
    partial class SwapColumnsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwapColumnsForm));
            this.groupBoxMoveFrom = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelColumn1 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonVernacularTranscription1 = new System.Windows.Forms.RadioButton();
            this.radioButtonNationalBtTranscription1 = new System.Windows.Forms.RadioButton();
            this.radioButtonInternationalBtTranscription1 = new System.Windows.Forms.RadioButton();
            this.radioButtonFreeTrTranscription1 = new System.Windows.Forms.RadioButton();
            this.groupBoxMoveTo = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelColumn2 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonVernacularTranscription2 = new System.Windows.Forms.RadioButton();
            this.radioButtonNationalBtTranscription2 = new System.Windows.Forms.RadioButton();
            this.radioButtonInternationalBtTranscription2 = new System.Windows.Forms.RadioButton();
            this.radioButtonFreeTrTranscription2 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxFieldsToSwap = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelFieldsToSwap = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxStoryLines = new System.Windows.Forms.CheckBox();
            this.checkBoxRetellings = new System.Windows.Forms.CheckBox();
            this.checkBoxTestQuestions = new System.Windows.Forms.CheckBox();
            this.checkBoxTestQuestionAnswers = new System.Windows.Forms.CheckBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonMoveToNextLineBefore = new System.Windows.Forms.Button();
            this.buttonMoveToPrevLineBefore = new System.Windows.Forms.Button();
            this.htmlStoryBtControlBefore = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.labelBefore = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonMoveToPrevLineAfter = new System.Windows.Forms.Button();
            this.buttonMoveToNextLineAfter = new System.Windows.Forms.Button();
            this.htmlStoryBtControlAfter = new OneStoryProjectEditor.HtmlStoryBtControl();
            this.labelAfter = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonSkip = new System.Windows.Forms.Button();
            this.groupBoxMoveFrom.SuspendLayout();
            this.flowLayoutPanelColumn1.SuspendLayout();
            this.groupBoxMoveTo.SuspendLayout();
            this.flowLayoutPanelColumn2.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBoxFieldsToSwap.SuspendLayout();
            this.flowLayoutPanelFieldsToSwap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMoveFrom
            // 
            this.groupBoxMoveFrom.Controls.Add(this.flowLayoutPanelColumn1);
            this.groupBoxMoveFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMoveFrom.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBoxMoveFrom.Location = new System.Drawing.Point(3, 3);
            this.groupBoxMoveFrom.Name = "groupBoxMoveFrom";
            this.groupBoxMoveFrom.Size = new System.Drawing.Size(387, 111);
            this.groupBoxMoveFrom.TabIndex = 2;
            this.groupBoxMoveFrom.TabStop = false;
            this.groupBoxMoveFrom.Text = "&Column 1 (choose the 1st column to swap)";
            // 
            // flowLayoutPanelColumn1
            // 
            this.flowLayoutPanelColumn1.Controls.Add(this.radioButtonVernacularTranscription1);
            this.flowLayoutPanelColumn1.Controls.Add(this.radioButtonNationalBtTranscription1);
            this.flowLayoutPanelColumn1.Controls.Add(this.radioButtonInternationalBtTranscription1);
            this.flowLayoutPanelColumn1.Controls.Add(this.radioButtonFreeTrTranscription1);
            this.flowLayoutPanelColumn1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelColumn1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelColumn1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelColumn1.Name = "flowLayoutPanelColumn1";
            this.flowLayoutPanelColumn1.Size = new System.Drawing.Size(381, 92);
            this.flowLayoutPanelColumn1.TabIndex = 0;
            // 
            // radioButtonVernacularTranscription1
            // 
            this.radioButtonVernacularTranscription1.AutoSize = true;
            this.radioButtonVernacularTranscription1.Checked = true;
            this.radioButtonVernacularTranscription1.Location = new System.Drawing.Point(3, 3);
            this.radioButtonVernacularTranscription1.Name = "radioButtonVernacularTranscription1";
            this.radioButtonVernacularTranscription1.Size = new System.Drawing.Size(100, 17);
            this.radioButtonVernacularTranscription1.TabIndex = 0;
            this.radioButtonVernacularTranscription1.TabStop = true;
            this.radioButtonVernacularTranscription1.Text = "S&tory Language";
            this.radioButtonVernacularTranscription1.UseVisualStyleBackColor = true;
            this.radioButtonVernacularTranscription1.Visible = false;
            this.radioButtonVernacularTranscription1.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonNationalBtTranscription1
            // 
            this.radioButtonNationalBtTranscription1.AutoSize = true;
            this.radioButtonNationalBtTranscription1.Location = new System.Drawing.Point(3, 26);
            this.radioButtonNationalBtTranscription1.Name = "radioButtonNationalBtTranscription1";
            this.radioButtonNationalBtTranscription1.Size = new System.Drawing.Size(175, 17);
            this.radioButtonNationalBtTranscription1.TabIndex = 1;
            this.radioButtonNationalBtTranscription1.TabStop = true;
            this.radioButtonNationalBtTranscription1.Text = "&National/Regional language BT";
            this.radioButtonNationalBtTranscription1.UseVisualStyleBackColor = true;
            this.radioButtonNationalBtTranscription1.Visible = false;
            this.radioButtonNationalBtTranscription1.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonInternationalBtTranscription1
            // 
            this.radioButtonInternationalBtTranscription1.AutoSize = true;
            this.radioButtonInternationalBtTranscription1.Location = new System.Drawing.Point(3, 49);
            this.radioButtonInternationalBtTranscription1.Name = "radioButtonInternationalBtTranscription1";
            this.radioButtonInternationalBtTranscription1.Size = new System.Drawing.Size(123, 17);
            this.radioButtonInternationalBtTranscription1.TabIndex = 2;
            this.radioButtonInternationalBtTranscription1.TabStop = true;
            this.radioButtonInternationalBtTranscription1.Text = "&English language BT";
            this.radioButtonInternationalBtTranscription1.UseVisualStyleBackColor = true;
            this.radioButtonInternationalBtTranscription1.Visible = false;
            this.radioButtonInternationalBtTranscription1.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonFreeTrTranscription1
            // 
            this.radioButtonFreeTrTranscription1.AutoSize = true;
            this.radioButtonFreeTrTranscription1.Location = new System.Drawing.Point(3, 72);
            this.radioButtonFreeTrTranscription1.Name = "radioButtonFreeTrTranscription1";
            this.radioButtonFreeTrTranscription1.Size = new System.Drawing.Size(101, 17);
            this.radioButtonFreeTrTranscription1.TabIndex = 3;
            this.radioButtonFreeTrTranscription1.TabStop = true;
            this.radioButtonFreeTrTranscription1.Text = "&Free Translation";
            this.radioButtonFreeTrTranscription1.UseVisualStyleBackColor = true;
            this.radioButtonFreeTrTranscription1.Visible = false;
            this.radioButtonFreeTrTranscription1.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // groupBoxMoveTo
            // 
            this.groupBoxMoveTo.Controls.Add(this.flowLayoutPanelColumn2);
            this.groupBoxMoveTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMoveTo.Location = new System.Drawing.Point(396, 3);
            this.groupBoxMoveTo.Name = "groupBoxMoveTo";
            this.groupBoxMoveTo.Size = new System.Drawing.Size(387, 111);
            this.groupBoxMoveTo.TabIndex = 3;
            this.groupBoxMoveTo.TabStop = false;
            this.groupBoxMoveTo.Text = "Column &2 (choose the 2nd column to swap)";
            // 
            // flowLayoutPanelColumn2
            // 
            this.flowLayoutPanelColumn2.Controls.Add(this.radioButtonVernacularTranscription2);
            this.flowLayoutPanelColumn2.Controls.Add(this.radioButtonNationalBtTranscription2);
            this.flowLayoutPanelColumn2.Controls.Add(this.radioButtonInternationalBtTranscription2);
            this.flowLayoutPanelColumn2.Controls.Add(this.radioButtonFreeTrTranscription2);
            this.flowLayoutPanelColumn2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelColumn2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelColumn2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelColumn2.Name = "flowLayoutPanelColumn2";
            this.flowLayoutPanelColumn2.Size = new System.Drawing.Size(381, 92);
            this.flowLayoutPanelColumn2.TabIndex = 0;
            // 
            // radioButtonVernacularTranscription2
            // 
            this.radioButtonVernacularTranscription2.AutoSize = true;
            this.radioButtonVernacularTranscription2.Location = new System.Drawing.Point(3, 3);
            this.radioButtonVernacularTranscription2.Name = "radioButtonVernacularTranscription2";
            this.radioButtonVernacularTranscription2.Size = new System.Drawing.Size(100, 17);
            this.radioButtonVernacularTranscription2.TabIndex = 0;
            this.radioButtonVernacularTranscription2.TabStop = true;
            this.radioButtonVernacularTranscription2.Text = "&Story Language";
            this.radioButtonVernacularTranscription2.UseVisualStyleBackColor = true;
            this.radioButtonVernacularTranscription2.Visible = false;
            this.radioButtonVernacularTranscription2.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonNationalBtTranscription2
            // 
            this.radioButtonNationalBtTranscription2.AutoSize = true;
            this.radioButtonNationalBtTranscription2.Location = new System.Drawing.Point(3, 26);
            this.radioButtonNationalBtTranscription2.Name = "radioButtonNationalBtTranscription2";
            this.radioButtonNationalBtTranscription2.Size = new System.Drawing.Size(175, 17);
            this.radioButtonNationalBtTranscription2.TabIndex = 1;
            this.radioButtonNationalBtTranscription2.TabStop = true;
            this.radioButtonNationalBtTranscription2.Text = "National/&Regional language BT";
            this.radioButtonNationalBtTranscription2.UseVisualStyleBackColor = true;
            this.radioButtonNationalBtTranscription2.Visible = false;
            this.radioButtonNationalBtTranscription2.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonInternationalBtTranscription2
            // 
            this.radioButtonInternationalBtTranscription2.AutoSize = true;
            this.radioButtonInternationalBtTranscription2.Location = new System.Drawing.Point(3, 49);
            this.radioButtonInternationalBtTranscription2.Name = "radioButtonInternationalBtTranscription2";
            this.radioButtonInternationalBtTranscription2.Size = new System.Drawing.Size(123, 17);
            this.radioButtonInternationalBtTranscription2.TabIndex = 2;
            this.radioButtonInternationalBtTranscription2.TabStop = true;
            this.radioButtonInternationalBtTranscription2.Text = "En&glish language BT";
            this.radioButtonInternationalBtTranscription2.UseVisualStyleBackColor = true;
            this.radioButtonInternationalBtTranscription2.Visible = false;
            this.radioButtonInternationalBtTranscription2.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // radioButtonFreeTrTranscription2
            // 
            this.radioButtonFreeTrTranscription2.AutoSize = true;
            this.radioButtonFreeTrTranscription2.Location = new System.Drawing.Point(3, 72);
            this.radioButtonFreeTrTranscription2.Name = "radioButtonFreeTrTranscription2";
            this.radioButtonFreeTrTranscription2.Size = new System.Drawing.Size(101, 17);
            this.radioButtonFreeTrTranscription2.TabIndex = 3;
            this.radioButtonFreeTrTranscription2.TabStop = true;
            this.radioButtonFreeTrTranscription2.Text = "Free Tr&anslation";
            this.radioButtonFreeTrTranscription2.UseVisualStyleBackColor = true;
            this.radioButtonFreeTrTranscription2.Visible = false;
            this.radioButtonFreeTrTranscription2.Click += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.groupBoxMoveFrom, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.groupBoxMoveTo, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.groupBoxFieldsToSwap, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.splitContainer, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonSkip, 0, 3);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(786, 583);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(396, 557);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxFieldsToSwap
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBoxFieldsToSwap, 3);
            this.groupBoxFieldsToSwap.Controls.Add(this.flowLayoutPanelFieldsToSwap);
            this.groupBoxFieldsToSwap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFieldsToSwap.Location = new System.Drawing.Point(3, 120);
            this.groupBoxFieldsToSwap.Name = "groupBoxFieldsToSwap";
            this.groupBoxFieldsToSwap.Size = new System.Drawing.Size(780, 44);
            this.groupBoxFieldsToSwap.TabIndex = 9;
            this.groupBoxFieldsToSwap.TabStop = false;
            this.groupBoxFieldsToSwap.Text = "Fields to swap";
            // 
            // flowLayoutPanelFieldsToSwap
            // 
            this.flowLayoutPanelFieldsToSwap.Controls.Add(this.checkBoxStoryLines);
            this.flowLayoutPanelFieldsToSwap.Controls.Add(this.checkBoxRetellings);
            this.flowLayoutPanelFieldsToSwap.Controls.Add(this.checkBoxTestQuestions);
            this.flowLayoutPanelFieldsToSwap.Controls.Add(this.checkBoxTestQuestionAnswers);
            this.flowLayoutPanelFieldsToSwap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelFieldsToSwap.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelFieldsToSwap.Name = "flowLayoutPanelFieldsToSwap";
            this.flowLayoutPanelFieldsToSwap.Size = new System.Drawing.Size(774, 25);
            this.flowLayoutPanelFieldsToSwap.TabIndex = 0;
            // 
            // checkBoxStoryLines
            // 
            this.checkBoxStoryLines.AutoSize = true;
            this.checkBoxStoryLines.Location = new System.Drawing.Point(3, 3);
            this.checkBoxStoryLines.Name = "checkBoxStoryLines";
            this.checkBoxStoryLines.Size = new System.Drawing.Size(78, 17);
            this.checkBoxStoryLines.TabIndex = 0;
            this.checkBoxStoryLines.Text = "Story Lines";
            this.checkBoxStoryLines.UseVisualStyleBackColor = true;
            this.checkBoxStoryLines.CheckStateChanged += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // checkBoxRetellings
            // 
            this.checkBoxRetellings.AutoSize = true;
            this.checkBoxRetellings.Location = new System.Drawing.Point(87, 3);
            this.checkBoxRetellings.Name = "checkBoxRetellings";
            this.checkBoxRetellings.Size = new System.Drawing.Size(72, 17);
            this.checkBoxRetellings.TabIndex = 1;
            this.checkBoxRetellings.Text = "Retellings";
            this.checkBoxRetellings.UseVisualStyleBackColor = true;
            this.checkBoxRetellings.CheckStateChanged += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // checkBoxTestQuestions
            // 
            this.checkBoxTestQuestions.AutoSize = true;
            this.checkBoxTestQuestions.Location = new System.Drawing.Point(165, 3);
            this.checkBoxTestQuestions.Name = "checkBoxTestQuestions";
            this.checkBoxTestQuestions.Size = new System.Drawing.Size(97, 17);
            this.checkBoxTestQuestions.TabIndex = 2;
            this.checkBoxTestQuestions.Text = "Test Questions";
            this.checkBoxTestQuestions.UseVisualStyleBackColor = true;
            this.checkBoxTestQuestions.CheckStateChanged += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // checkBoxTestQuestionAnswers
            // 
            this.checkBoxTestQuestionAnswers.AutoSize = true;
            this.checkBoxTestQuestionAnswers.Location = new System.Drawing.Point(268, 3);
            this.checkBoxTestQuestionAnswers.Name = "checkBoxTestQuestionAnswers";
            this.checkBoxTestQuestionAnswers.Size = new System.Drawing.Size(66, 17);
            this.checkBoxTestQuestionAnswers.TabIndex = 3;
            this.checkBoxTestQuestionAnswers.Text = "Answers";
            this.checkBoxTestQuestionAnswers.UseVisualStyleBackColor = true;
            this.checkBoxTestQuestionAnswers.CheckStateChanged += new System.EventHandler(this.UpdateAfterDisplay);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel.SetColumnSpan(this.splitContainer, 3);
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(3, 170);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer.Size = new System.Drawing.Size(780, 381);
            this.splitContainer.SplitterDistance = 190;
            this.splitContainer.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.buttonMoveToNextLineBefore, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonMoveToPrevLineBefore, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.htmlStoryBtControlBefore, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelBefore, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 186);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonMoveToNextLineBefore
            // 
            this.buttonMoveToNextLineBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveToNextLineBefore.Image = global::OneStoryProjectEditor.Properties.Resources.FillDownHS;
            this.buttonMoveToNextLineBefore.Location = new System.Drawing.Point(750, 3);
            this.buttonMoveToNextLineBefore.Name = "buttonMoveToNextLineBefore";
            this.buttonMoveToNextLineBefore.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveToNextLineBefore.TabIndex = 8;
            this.buttonMoveToNextLineBefore.UseVisualStyleBackColor = true;
            this.buttonMoveToNextLineBefore.Click += new System.EventHandler(this.ButtonMoveToNextLineBeforeClick);
            // 
            // buttonMoveToPrevLineBefore
            // 
            this.buttonMoveToPrevLineBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveToPrevLineBefore.Image = global::OneStoryProjectEditor.Properties.Resources.FillUpHS;
            this.buttonMoveToPrevLineBefore.Location = new System.Drawing.Point(720, 3);
            this.buttonMoveToPrevLineBefore.Name = "buttonMoveToPrevLineBefore";
            this.buttonMoveToPrevLineBefore.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveToPrevLineBefore.TabIndex = 9;
            this.buttonMoveToPrevLineBefore.UseVisualStyleBackColor = true;
            this.buttonMoveToPrevLineBefore.Click += new System.EventHandler(this.ButtonMoveToPrevLineBeforeClick);
            // 
            // htmlStoryBtControlBefore
            // 
            this.htmlStoryBtControlBefore.AllowWebBrowserDrop = false;
            this.tableLayoutPanel1.SetColumnSpan(this.htmlStoryBtControlBefore, 3);
            this.htmlStoryBtControlBefore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControlBefore.IsWebBrowserContextMenuEnabled = false;
            this.htmlStoryBtControlBefore.Location = new System.Drawing.Point(3, 32);
            this.htmlStoryBtControlBefore.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControlBefore.Name = "htmlStoryBtControlBefore";
            this.htmlStoryBtControlBefore.ParentStory = null;
            this.htmlStoryBtControlBefore.Size = new System.Drawing.Size(770, 151);
            this.htmlStoryBtControlBefore.StoryData = null;
            this.htmlStoryBtControlBefore.TabIndex = 4;
            this.htmlStoryBtControlBefore.TheSE = null;
            this.htmlStoryBtControlBefore.ViewSettings = null;
            // 
            // labelBefore
            // 
            this.labelBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelBefore.AutoSize = true;
            this.labelBefore.Location = new System.Drawing.Point(3, 16);
            this.labelBefore.Name = "labelBefore";
            this.labelBefore.Size = new System.Drawing.Size(41, 13);
            this.labelBefore.TabIndex = 7;
            this.labelBefore.Text = "Before:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.buttonMoveToPrevLineAfter, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonMoveToNextLineAfter, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.htmlStoryBtControlAfter, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelAfter, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(776, 183);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonMoveToPrevLineAfter
            // 
            this.buttonMoveToPrevLineAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveToPrevLineAfter.Image = global::OneStoryProjectEditor.Properties.Resources.FillUpHS;
            this.buttonMoveToPrevLineAfter.Location = new System.Drawing.Point(721, 3);
            this.buttonMoveToPrevLineAfter.Name = "buttonMoveToPrevLineAfter";
            this.buttonMoveToPrevLineAfter.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveToPrevLineAfter.TabIndex = 9;
            this.buttonMoveToPrevLineAfter.UseVisualStyleBackColor = true;
            this.buttonMoveToPrevLineAfter.Click += new System.EventHandler(this.ButtonMoveToPrevLineAfterClick);
            // 
            // buttonMoveToNextLineAfter
            // 
            this.buttonMoveToNextLineAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveToNextLineAfter.Image = global::OneStoryProjectEditor.Properties.Resources.FillDownHS;
            this.buttonMoveToNextLineAfter.Location = new System.Drawing.Point(750, 3);
            this.buttonMoveToNextLineAfter.Name = "buttonMoveToNextLineAfter";
            this.buttonMoveToNextLineAfter.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveToNextLineAfter.TabIndex = 8;
            this.buttonMoveToNextLineAfter.UseVisualStyleBackColor = true;
            this.buttonMoveToNextLineAfter.Click += new System.EventHandler(this.ButtonMoveToNextLineAfterClick);
            // 
            // htmlStoryBtControlAfter
            // 
            this.htmlStoryBtControlAfter.AllowWebBrowserDrop = false;
            this.tableLayoutPanel2.SetColumnSpan(this.htmlStoryBtControlAfter, 3);
            this.htmlStoryBtControlAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlStoryBtControlAfter.IsWebBrowserContextMenuEnabled = false;
            this.htmlStoryBtControlAfter.Location = new System.Drawing.Point(3, 32);
            this.htmlStoryBtControlAfter.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlStoryBtControlAfter.Name = "htmlStoryBtControlAfter";
            this.htmlStoryBtControlAfter.ParentStory = null;
            this.htmlStoryBtControlAfter.Size = new System.Drawing.Size(770, 148);
            this.htmlStoryBtControlAfter.StoryData = null;
            this.htmlStoryBtControlAfter.TabIndex = 4;
            this.htmlStoryBtControlAfter.TheSE = null;
            this.htmlStoryBtControlAfter.ViewSettings = null;
            // 
            // labelAfter
            // 
            this.labelAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAfter.AutoSize = true;
            this.labelAfter.Location = new System.Drawing.Point(3, 16);
            this.labelAfter.Name = "labelAfter";
            this.labelAfter.Size = new System.Drawing.Size(32, 13);
            this.labelAfter.TabIndex = 8;
            this.labelAfter.Text = "After:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&Swap";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonSkip
            // 
            this.buttonSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Size = new System.Drawing.Size(75, 23);
            this.buttonSkip.TabIndex = 5;
            this.buttonSkip.Text = "S&kip";
            this.buttonSkip.UseVisualStyleBackColor = true;
            this.buttonSkip.Click += new System.EventHandler(this.buttonSkip_Click);
            // 
            // SwapColumnsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 608);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SwapColumnsForm";
            this.Text = "Swap Columns";
            this.groupBoxMoveFrom.ResumeLayout(false);
            this.flowLayoutPanelColumn1.ResumeLayout(false);
            this.flowLayoutPanelColumn1.PerformLayout();
            this.groupBoxMoveTo.ResumeLayout(false);
            this.flowLayoutPanelColumn2.ResumeLayout(false);
            this.flowLayoutPanelColumn2.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBoxFieldsToSwap.ResumeLayout(false);
            this.flowLayoutPanelFieldsToSwap.ResumeLayout(false);
            this.flowLayoutPanelFieldsToSwap.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMoveFrom;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelColumn1;
        private System.Windows.Forms.RadioButton radioButtonVernacularTranscription1;
        private System.Windows.Forms.RadioButton radioButtonNationalBtTranscription1;
        private System.Windows.Forms.RadioButton radioButtonInternationalBtTranscription1;
        private System.Windows.Forms.RadioButton radioButtonFreeTrTranscription1;
        private System.Windows.Forms.GroupBox groupBoxMoveTo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelColumn2;
        private System.Windows.Forms.RadioButton radioButtonVernacularTranscription2;
        private System.Windows.Forms.RadioButton radioButtonNationalBtTranscription2;
        private System.Windows.Forms.RadioButton radioButtonInternationalBtTranscription2;
        private System.Windows.Forms.RadioButton radioButtonFreeTrTranscription2;
        private HtmlStoryBtControl htmlStoryBtControlBefore;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private HtmlStoryBtControl htmlStoryBtControlAfter;
        private System.Windows.Forms.Label labelBefore;
        private System.Windows.Forms.Label labelAfter;
        private System.Windows.Forms.GroupBox groupBoxFieldsToSwap;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelFieldsToSwap;
        private System.Windows.Forms.CheckBox checkBoxStoryLines;
        private System.Windows.Forms.CheckBox checkBoxRetellings;
        private System.Windows.Forms.CheckBox checkBoxTestQuestions;
        private System.Windows.Forms.CheckBox checkBoxTestQuestionAnswers;
        private System.Windows.Forms.Button buttonMoveToNextLineBefore;
        private System.Windows.Forms.Button buttonMoveToPrevLineBefore;
        private System.Windows.Forms.Button buttonMoveToPrevLineAfter;
        private System.Windows.Forms.Button buttonMoveToNextLineAfter;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonSkip;
    }
}