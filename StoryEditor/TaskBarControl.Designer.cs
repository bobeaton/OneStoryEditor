﻿namespace OneStoryProjectEditor
{
    partial class TaskBarControl
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
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanelTasks = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonViewTasksPf = new System.Windows.Forms.Button();
            this.buttonViewTasksCit = new System.Windows.Forms.Button();
            this.buttonAddStory = new System.Windows.Forms.Button();
            this.buttonVernacular = new System.Windows.Forms.Button();
            this.buttonNationalBt = new System.Windows.Forms.Button();
            this.buttonInternationalBt = new System.Windows.Forms.Button();
            this.buttonFreeTranslation = new System.Windows.Forms.Button();
            this.buttonAnchors = new System.Windows.Forms.Button();
            this.buttonViewRetellings = new System.Windows.Forms.Button();
            this.buttonAddRetellingBoxes = new System.Windows.Forms.Button();
            this.buttonViewTestQuestions = new System.Windows.Forms.Button();
            this.buttonViewTestQuestionAnswers = new System.Windows.Forms.Button();
            this.buttonAddBoxesForAnswers = new System.Windows.Forms.Button();
            this.buttonStoryInformation = new System.Windows.Forms.Button();
            this.buttonViewPanorama = new System.Windows.Forms.Button();
            this.buttonCopyRecordingToDropbox = new System.Windows.Forms.Button();
            this.contextMenuDropbox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dropboxStory = new System.Windows.Forms.ToolStripMenuItem();
            this.dropboxRetelling = new System.Windows.Forms.ToolStripMenuItem();
            this.dropboxTqAnswers = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSendToConsultant = new System.Windows.Forms.Button();
            this.buttonSendToEnglishBter = new System.Windows.Forms.Button();
            this.buttonReturnToProjectFacilitator = new System.Windows.Forms.Button();
            this.buttonSendToCoach = new System.Windows.Forms.Button();
            this.buttonSendToCIT = new System.Windows.Forms.Button();
            this.buttonMarkPreliminaryApproval = new System.Windows.Forms.Button();
            this.buttonMarkFinalApproval = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanelTasks.SuspendLayout();
            this.contextMenuDropbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelTasks
            // 
            this.flowLayoutPanelTasks.AutoSize = true;
            this.flowLayoutPanelTasks.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewTasksPf);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewTasksCit);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonAddStory);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonVernacular);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonNationalBt);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonInternationalBt);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonFreeTranslation);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonAnchors);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewRetellings);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonAddRetellingBoxes);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewTestQuestions);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewTestQuestionAnswers);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonAddBoxesForAnswers);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonStoryInformation);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonViewPanorama);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonCopyRecordingToDropbox);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonSendToConsultant);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonSendToEnglishBter);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonReturnToProjectFacilitator);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonSendToCoach);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonSendToCIT);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonMarkPreliminaryApproval);
            this.flowLayoutPanelTasks.Controls.Add(this.buttonMarkFinalApproval);
            this.flowLayoutPanelTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelTasks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelTasks.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelTasks.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.flowLayoutPanelTasks.Name = "flowLayoutPanelTasks";
            this.flowLayoutPanelTasks.Size = new System.Drawing.Size(216, 667);
            this.flowLayoutPanelTasks.TabIndex = 1;
            // 
            // buttonViewTasksPf
            // 
            this.buttonViewTasksPf.Location = new System.Drawing.Point(3, 3);
            this.buttonViewTasksPf.Name = "buttonViewTasksPf";
            this.buttonViewTasksPf.Size = new System.Drawing.Size(210, 23);
            this.buttonViewTasksPf.TabIndex = 18;
            this.buttonViewTasksPf.Text = "&View Project Facilitator Tasks";
            this.buttonViewTasksPf.UseVisualStyleBackColor = true;
            this.buttonViewTasksPf.Visible = false;
            this.buttonViewTasksPf.Click += new System.EventHandler(this.buttonViewTasksPf_Click);
            // 
            // buttonViewTasksCit
            // 
            this.buttonViewTasksCit.Location = new System.Drawing.Point(3, 32);
            this.buttonViewTasksCit.Name = "buttonViewTasksCit";
            this.buttonViewTasksCit.Size = new System.Drawing.Size(210, 23);
            this.buttonViewTasksCit.TabIndex = 21;
            this.buttonViewTasksCit.Text = "&View CIT Tasks";
            this.buttonViewTasksCit.UseVisualStyleBackColor = true;
            this.buttonViewTasksCit.Visible = false;
            this.buttonViewTasksCit.Click += new System.EventHandler(this.buttonViewTasksCit_Click);
            // 
            // buttonAddStory
            // 
            this.buttonAddStory.Location = new System.Drawing.Point(3, 61);
            this.buttonAddStory.Name = "buttonAddStory";
            this.buttonAddStory.Size = new System.Drawing.Size(210, 23);
            this.buttonAddStory.TabIndex = 0;
            this.buttonAddStory.Text = "Add &Story";
            this.buttonAddStory.UseVisualStyleBackColor = true;
            this.buttonAddStory.Visible = false;
            this.buttonAddStory.Click += new System.EventHandler(this.buttonAddStory_Click);
            // 
            // buttonVernacular
            // 
            this.buttonVernacular.Location = new System.Drawing.Point(3, 90);
            this.buttonVernacular.Name = "buttonVernacular";
            this.buttonVernacular.Size = new System.Drawing.Size(210, 23);
            this.buttonVernacular.TabIndex = 1;
            this.buttonVernacular.Text = "View Story Language (&Vernacular)";
            this.buttonVernacular.UseVisualStyleBackColor = true;
            this.buttonVernacular.Visible = false;
            this.buttonVernacular.Click += new System.EventHandler(this.buttonVernacular_Click);
            // 
            // buttonNationalBt
            // 
            this.buttonNationalBt.Location = new System.Drawing.Point(3, 119);
            this.buttonNationalBt.Name = "buttonNationalBt";
            this.buttonNationalBt.Size = new System.Drawing.Size(210, 23);
            this.buttonNationalBt.TabIndex = 2;
            this.buttonNationalBt.Text = "View &National/Regional BT";
            this.buttonNationalBt.UseVisualStyleBackColor = true;
            this.buttonNationalBt.Visible = false;
            this.buttonNationalBt.Click += new System.EventHandler(this.buttonNationalBt_Click);
            // 
            // buttonInternationalBt
            // 
            this.buttonInternationalBt.Location = new System.Drawing.Point(3, 148);
            this.buttonInternationalBt.Name = "buttonInternationalBt";
            this.buttonInternationalBt.Size = new System.Drawing.Size(210, 23);
            this.buttonInternationalBt.TabIndex = 3;
            this.buttonInternationalBt.Text = "View &English BT";
            this.buttonInternationalBt.UseVisualStyleBackColor = true;
            this.buttonInternationalBt.Visible = false;
            this.buttonInternationalBt.Click += new System.EventHandler(this.buttonInternationalBt_Click);
            // 
            // buttonFreeTranslation
            // 
            this.buttonFreeTranslation.Location = new System.Drawing.Point(3, 177);
            this.buttonFreeTranslation.Name = "buttonFreeTranslation";
            this.buttonFreeTranslation.Size = new System.Drawing.Size(210, 23);
            this.buttonFreeTranslation.TabIndex = 4;
            this.buttonFreeTranslation.Text = "View &Free Translation (UNS BT)";
            this.buttonFreeTranslation.UseVisualStyleBackColor = true;
            this.buttonFreeTranslation.Visible = false;
            this.buttonFreeTranslation.Click += new System.EventHandler(this.buttonFreeTranslation_Click);
            // 
            // buttonAnchors
            // 
            this.buttonAnchors.Location = new System.Drawing.Point(3, 206);
            this.buttonAnchors.Name = "buttonAnchors";
            this.buttonAnchors.Size = new System.Drawing.Size(210, 23);
            this.buttonAnchors.TabIndex = 5;
            this.buttonAnchors.Text = "View &Anchors";
            this.buttonAnchors.UseVisualStyleBackColor = true;
            this.buttonAnchors.Visible = false;
            this.buttonAnchors.Click += new System.EventHandler(this.buttonAnchors_Click);
            // 
            // buttonViewRetellings
            // 
            this.buttonViewRetellings.Location = new System.Drawing.Point(3, 235);
            this.buttonViewRetellings.Name = "buttonViewRetellings";
            this.buttonViewRetellings.Size = new System.Drawing.Size(210, 23);
            this.buttonViewRetellings.TabIndex = 6;
            this.buttonViewRetellings.Text = "View &Retellings";
            this.buttonViewRetellings.UseVisualStyleBackColor = true;
            this.buttonViewRetellings.Visible = false;
            this.buttonViewRetellings.Click += new System.EventHandler(this.buttonRetellings_Click);
            // 
            // buttonAddRetellingBoxes
            // 
            this.buttonAddRetellingBoxes.Location = new System.Drawing.Point(3, 264);
            this.buttonAddRetellingBoxes.Name = "buttonAddRetellingBoxes";
            this.buttonAddRetellingBoxes.Size = new System.Drawing.Size(210, 23);
            this.buttonAddRetellingBoxes.TabIndex = 13;
            this.buttonAddRetellingBoxes.Text = "Add &boxes for Retellings";
            this.buttonAddRetellingBoxes.UseVisualStyleBackColor = true;
            this.buttonAddRetellingBoxes.Visible = false;
            this.buttonAddRetellingBoxes.Click += new System.EventHandler(this.buttonAddRetellingBoxes_Click);
            // 
            // buttonViewTestQuestions
            // 
            this.buttonViewTestQuestions.Location = new System.Drawing.Point(3, 293);
            this.buttonViewTestQuestions.Name = "buttonViewTestQuestions";
            this.buttonViewTestQuestions.Size = new System.Drawing.Size(210, 23);
            this.buttonViewTestQuestions.TabIndex = 7;
            this.buttonViewTestQuestions.Text = "View &Testing Questions";
            this.buttonViewTestQuestions.UseVisualStyleBackColor = true;
            this.buttonViewTestQuestions.Visible = false;
            this.buttonViewTestQuestions.Click += new System.EventHandler(this.buttonTestQuestions_Click);
            // 
            // buttonViewTestQuestionAnswers
            // 
            this.buttonViewTestQuestionAnswers.Location = new System.Drawing.Point(3, 322);
            this.buttonViewTestQuestionAnswers.Name = "buttonViewTestQuestionAnswers";
            this.buttonViewTestQuestionAnswers.Size = new System.Drawing.Size(210, 23);
            this.buttonViewTestQuestionAnswers.TabIndex = 8;
            this.buttonViewTestQuestionAnswers.Text = "View Ans&wers";
            this.buttonViewTestQuestionAnswers.UseVisualStyleBackColor = true;
            this.buttonViewTestQuestionAnswers.Visible = false;
            this.buttonViewTestQuestionAnswers.Click += new System.EventHandler(this.buttonTestQuestionAnswers_Click);
            // 
            // buttonAddBoxesForAnswers
            // 
            this.buttonAddBoxesForAnswers.Location = new System.Drawing.Point(3, 351);
            this.buttonAddBoxesForAnswers.Name = "buttonAddBoxesForAnswers";
            this.buttonAddBoxesForAnswers.Size = new System.Drawing.Size(210, 23);
            this.buttonAddBoxesForAnswers.TabIndex = 14;
            this.buttonAddBoxesForAnswers.Text = "Add bo&xes for Answers";
            this.buttonAddBoxesForAnswers.UseVisualStyleBackColor = true;
            this.buttonAddBoxesForAnswers.Visible = false;
            this.buttonAddBoxesForAnswers.Click += new System.EventHandler(this.buttonAddBoxesForAnswers_Click);
            // 
            // buttonStoryInformation
            // 
            this.buttonStoryInformation.Location = new System.Drawing.Point(3, 380);
            this.buttonStoryInformation.Name = "buttonStoryInformation";
            this.buttonStoryInformation.Size = new System.Drawing.Size(210, 23);
            this.buttonStoryInformation.TabIndex = 9;
            this.buttonStoryInformation.Text = "View Story Infor&mation";
            this.buttonStoryInformation.UseVisualStyleBackColor = true;
            this.buttonStoryInformation.Visible = false;
            this.buttonStoryInformation.Click += new System.EventHandler(this.buttonStoryInformation_Click);
            // 
            // buttonViewPanorama
            // 
            this.buttonViewPanorama.Location = new System.Drawing.Point(3, 409);
            this.buttonViewPanorama.Name = "buttonViewPanorama";
            this.buttonViewPanorama.Size = new System.Drawing.Size(210, 23);
            this.buttonViewPanorama.TabIndex = 10;
            this.buttonViewPanorama.Text = "View &Panorama Information";
            this.buttonViewPanorama.UseVisualStyleBackColor = true;
            this.buttonViewPanorama.Visible = false;
            this.buttonViewPanorama.Click += new System.EventHandler(this.buttonViewPanorama_Click);
            // 
            // buttonCopyRecordingToDropbox
            // 
            this.buttonCopyRecordingToDropbox.ContextMenuStrip = this.contextMenuDropbox;
            this.buttonCopyRecordingToDropbox.Location = new System.Drawing.Point(3, 438);
            this.buttonCopyRecordingToDropbox.Name = "buttonCopyRecordingToDropbox";
            this.buttonCopyRecordingToDropbox.Size = new System.Drawing.Size(210, 23);
            this.buttonCopyRecordingToDropbox.TabIndex = 22;
            this.buttonCopyRecordingToDropbox.Text = "Copy Recording to &Dropbox";
            this.buttonCopyRecordingToDropbox.UseVisualStyleBackColor = true;
            this.buttonCopyRecordingToDropbox.Visible = false;
            this.buttonCopyRecordingToDropbox.Click += new System.EventHandler(this.ButtonCopyRecordingToDropboxClick);
            // 
            // contextMenuDropbox
            // 
            this.contextMenuDropbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dropboxStory,
            this.dropboxRetelling,
            this.dropboxTqAnswers});
            this.contextMenuDropbox.Name = "contextMenuDropbox";
            this.contextMenuDropbox.Size = new System.Drawing.Size(257, 92);
            // 
            // dropboxStory
            // 
            this.dropboxStory.Name = "dropboxStory";
            this.dropboxStory.Size = new System.Drawing.Size(256, 22);
            this.dropboxStory.Text = "Story recording";
            this.dropboxStory.Click += new System.EventHandler(this.DropboxStoryClick);
            // 
            // dropboxRetelling
            // 
            this.dropboxRetelling.Name = "dropboxRetelling";
            this.dropboxRetelling.Size = new System.Drawing.Size(256, 22);
            this.dropboxRetelling.Text = "Retelling recording";
            this.dropboxRetelling.Click += new System.EventHandler(this.DropboxRetellingClick);
            // 
            // dropboxTqAnswers
            // 
            this.dropboxTqAnswers.Name = "dropboxTqAnswers";
            this.dropboxTqAnswers.Size = new System.Drawing.Size(256, 22);
            this.dropboxTqAnswers.Text = "Testing question answer recording";
            this.dropboxTqAnswers.Click += new System.EventHandler(this.DropboxTqAnswersClick);
            // 
            // buttonSendToConsultant
            // 
            this.buttonSendToConsultant.Location = new System.Drawing.Point(3, 467);
            this.buttonSendToConsultant.Name = "buttonSendToConsultant";
            this.buttonSendToConsultant.Size = new System.Drawing.Size(210, 23);
            this.buttonSendToConsultant.TabIndex = 11;
            this.buttonSendToConsultant.Text = "Set to &Consultant\'s turn";
            this.buttonSendToConsultant.UseVisualStyleBackColor = true;
            this.buttonSendToConsultant.Visible = false;
            this.buttonSendToConsultant.Click += new System.EventHandler(this.buttonSendToConsultant_Click);
            // 
            // buttonSendToEnglishBter
            // 
            this.buttonSendToEnglishBter.Location = new System.Drawing.Point(3, 496);
            this.buttonSendToEnglishBter.Name = "buttonSendToEnglishBter";
            this.buttonSendToEnglishBter.Size = new System.Drawing.Size(210, 23);
            this.buttonSendToEnglishBter.TabIndex = 19;
            this.buttonSendToEnglishBter.Text = "Set to &English Back-translator\'s turn";
            this.buttonSendToEnglishBter.UseVisualStyleBackColor = true;
            this.buttonSendToEnglishBter.Visible = false;
            this.buttonSendToEnglishBter.Click += new System.EventHandler(this.buttonSendToEnglishBter_Click);
            // 
            // buttonReturnToProjectFacilitator
            // 
            this.buttonReturnToProjectFacilitator.Location = new System.Drawing.Point(3, 525);
            this.buttonReturnToProjectFacilitator.Name = "buttonReturnToProjectFacilitator";
            this.buttonReturnToProjectFacilitator.Size = new System.Drawing.Size(210, 23);
            this.buttonReturnToProjectFacilitator.TabIndex = 12;
            this.buttonReturnToProjectFacilitator.Text = "Set to &Project Facilitator\'s turn";
            this.buttonReturnToProjectFacilitator.UseVisualStyleBackColor = true;
            this.buttonReturnToProjectFacilitator.Visible = false;
            this.buttonReturnToProjectFacilitator.Click += new System.EventHandler(this.buttonReturnToProjectFacilitator_Click);
            // 
            // buttonSendToCoach
            // 
            this.buttonSendToCoach.Location = new System.Drawing.Point(3, 554);
            this.buttonSendToCoach.Name = "buttonSendToCoach";
            this.buttonSendToCoach.Size = new System.Drawing.Size(210, 23);
            this.buttonSendToCoach.TabIndex = 16;
            this.buttonSendToCoach.Text = "Set to Coac&h\'s turn";
            this.buttonSendToCoach.UseVisualStyleBackColor = true;
            this.buttonSendToCoach.Visible = false;
            this.buttonSendToCoach.Click += new System.EventHandler(this.buttonSendToCoach_Click);
            // 
            // buttonSendToCIT
            // 
            this.buttonSendToCIT.Location = new System.Drawing.Point(3, 583);
            this.buttonSendToCIT.Name = "buttonSendToCIT";
            this.buttonSendToCIT.Size = new System.Drawing.Size(210, 23);
            this.buttonSendToCIT.TabIndex = 17;
            this.buttonSendToCIT.Text = "Set to CIT\'s t&urn";
            this.buttonSendToCIT.UseVisualStyleBackColor = true;
            this.buttonSendToCIT.Visible = false;
            this.buttonSendToCIT.Click += new System.EventHandler(this.buttonSendToCIT_Click);
            // 
            // buttonMarkPreliminaryApproval
            // 
            this.buttonMarkPreliminaryApproval.Location = new System.Drawing.Point(3, 612);
            this.buttonMarkPreliminaryApproval.Name = "buttonMarkPreliminaryApproval";
            this.buttonMarkPreliminaryApproval.Size = new System.Drawing.Size(210, 23);
            this.buttonMarkPreliminaryApproval.TabIndex = 15;
            this.buttonMarkPreliminaryApproval.Text = "&Mark story with Preliminary Approval";
            this.buttonMarkPreliminaryApproval.UseVisualStyleBackColor = true;
            this.buttonMarkPreliminaryApproval.Visible = false;
            this.buttonMarkPreliminaryApproval.Click += new System.EventHandler(this.buttonMarkPreliminaryApproval_Click);
            // 
            // buttonMarkFinalApproval
            // 
            this.buttonMarkFinalApproval.Location = new System.Drawing.Point(3, 641);
            this.buttonMarkFinalApproval.Name = "buttonMarkFinalApproval";
            this.buttonMarkFinalApproval.Size = new System.Drawing.Size(210, 23);
            this.buttonMarkFinalApproval.TabIndex = 20;
            this.buttonMarkFinalApproval.Text = "&Mark story with Final Approval";
            this.buttonMarkFinalApproval.UseVisualStyleBackColor = true;
            this.buttonMarkFinalApproval.Visible = false;
            this.buttonMarkFinalApproval.Click += new System.EventHandler(this.buttonMarkFinalApproval_Click);
            // 
            // TaskBarControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.flowLayoutPanelTasks);
            this.Name = "TaskBarControl";
            this.Size = new System.Drawing.Size(216, 667);
            this.flowLayoutPanelTasks.ResumeLayout(false);
            this.contextMenuDropbox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTasks;
        private System.Windows.Forms.Button buttonAddStory;
        private System.Windows.Forms.Button buttonVernacular;
        private System.Windows.Forms.Button buttonNationalBt;
        private System.Windows.Forms.Button buttonInternationalBt;
        private System.Windows.Forms.Button buttonFreeTranslation;
        private System.Windows.Forms.Button buttonAnchors;
        private System.Windows.Forms.Button buttonViewRetellings;
        private System.Windows.Forms.Button buttonViewTestQuestions;
        private System.Windows.Forms.Button buttonViewTestQuestionAnswers;
        private System.Windows.Forms.Button buttonStoryInformation;
        private System.Windows.Forms.Button buttonViewPanorama;
        private System.Windows.Forms.Button buttonSendToConsultant;
        private System.Windows.Forms.Button buttonReturnToProjectFacilitator;
        private System.Windows.Forms.Button buttonAddRetellingBoxes;
        private System.Windows.Forms.Button buttonAddBoxesForAnswers;
        private System.Windows.Forms.Button buttonMarkPreliminaryApproval;
        private System.Windows.Forms.Button buttonSendToCoach;
        private System.Windows.Forms.Button buttonSendToCIT;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonViewTasksPf;
        private System.Windows.Forms.Button buttonSendToEnglishBter;
        private System.Windows.Forms.Button buttonMarkFinalApproval;
        private System.Windows.Forms.Button buttonViewTasksCit;
        private System.Windows.Forms.Button buttonCopyRecordingToDropbox;
        private System.Windows.Forms.ContextMenuStrip contextMenuDropbox;
        private System.Windows.Forms.ToolStripMenuItem dropboxStory;
        private System.Windows.Forms.ToolStripMenuItem dropboxRetelling;
        private System.Windows.Forms.ToolStripMenuItem dropboxTqAnswers;
    }
}
