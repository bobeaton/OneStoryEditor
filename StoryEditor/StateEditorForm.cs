﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class StateEditorForm : Form
    {
        protected StoryStageLogic.StateTransition _stateTransition;

        public StateEditorForm(StoryStageLogic.StateTransition stateTransition)
        {
            InitializeComponent();

            _stateTransition = stateTransition;
            textBoxStateName.Text = stateTransition.StageDisplayString;
            textBoxInstructions.Text = stateTransition.StageInstructions;

            checkBoxVernacular.Checked = stateTransition.IsVernacularVisible;
            checkBoxNationalBT.Checked = stateTransition.IsNationalBTVisible;
            checkBoxEnglishBT.Checked = stateTransition.IsEnglishBTVisible;
            checkBoxAnchors.Checked = stateTransition.IsAnchorVisible;
            checkBoxStoryTestingQuestions.Checked = stateTransition.IsStoryTestingQuestion;
            checkBoxRetelling.Checked = stateTransition.IsRetellingVisible;
            checkBoxConsultantNotes.Checked = stateTransition.IsConsultantNotesVisible;
            checkBoxCoachNotes.Checked = stateTransition.IsCoachNotesVisible;
            checkBoxBiblePane.Checked = stateTransition.IsNetBibleVisible;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            _stateTransition.StageDisplayString = textBoxStateName.Text;
            _stateTransition.StageInstructions = textBoxInstructions.Text;
            _stateTransition.IsVernacularVisible = checkBoxVernacular.Checked;
            _stateTransition.IsNationalBTVisible = checkBoxNationalBT.Checked;
            _stateTransition.IsEnglishBTVisible = checkBoxEnglishBT.Checked;
            _stateTransition.IsAnchorVisible = checkBoxAnchors.Checked;
            _stateTransition.IsStoryTestingQuestion = checkBoxStoryTestingQuestions.Checked;
            _stateTransition.IsRetellingVisible = checkBoxRetelling.Checked;
            _stateTransition.IsConsultantNotesVisible = checkBoxConsultantNotes.Checked;
            _stateTransition.IsCoachNotesVisible = checkBoxCoachNotes.Checked;
            _stateTransition.IsNetBibleVisible = checkBoxBiblePane.Checked;
        }
    }
}
