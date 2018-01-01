using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using NetLoc;
using SIL.Keyboarding;
using OneStoryProjectEditor.Properties;

namespace OneStoryProjectEditor
{
    public class CtrlTextBox : TextBox
    {
        internal VerseControl _ctrlVerseParent;
        protected string _strKeyboardName;
        internal string _strLabel;
        protected string _strLangName;
        internal string _strFullStop;
        internal CtrlTextBox NationalBtSibling;
        internal CtrlTextBox EnglishBtSibling;
        private StoryEditor.TextFields _eFieldType;

        public delegate void ThrowIfNotCorrectEditor(TeamMemberData.UserTypes eLoggedInMember, TeamMemberData.UserTypes eRequiredEditor);
        protected ThrowIfNotCorrectEditor _delegateRequiredEditorCheck;
        protected TeamMemberData.UserTypes _eRequiredEditor = TeamMemberData.UserTypes.Undefined;

        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, 
            ResizableControl ctrlParent, StringTransfer stData,
            ProjectSettings.LanguageInfo li, string strLabel,
            StoryEditor.TextFields eFieldType, Color clrFont)
        {
            InitComponent(strLabel);
            Init(strName, strLabel, li, stData, ctrlParent, ctrlVerseParent, eFieldType,
                clrFont);
        }

        private void Init(string strName, string strLabel, ProjectSettings.LanguageInfo li, 
            StringTransfer stData, ResizableControl ctrlParent, VerseControl ctrlVerseParent,
            StoryEditor.TextFields eFieldType, Color clrFont)
        {
            Name = strName;
            _strLabel = strLabel;
            _strLangName = li.LangName;
            Font = li.FontToUse;
            ForeColor = clrFont;
            BorderStyle = BorderStyle.FixedSingle;
            // BackColor = SystemColors.ButtonFace;
            if (li.DoRtl)
                RightToLeft = RightToLeft.Yes;
            stData.SetAssociation(this);
            InsureLanguageNameLabel();
            Margin = new Padding(1, 0, 0, 0);
            Padding = new Padding(0);
            Size = GetPreferredSize(Size);
            TextChanged += ctrlParent.textBox_TextChanged;
            MouseMove += CtrlTextBox_MouseMove;
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _ctrlVerseParent = ctrlVerseParent;
            _strKeyboardName = li.Keyboard;
            _strFullStop = li.FullStop;
            _eFieldType = eFieldType;
        }

        void CtrlTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            var form = _ctrlVerseParent.FindForm();
            if (!(form is StoryEditor))
                return;
            var theSe = (StoryEditor)form;
            theSe.CheckBiblePaneCursorPosition();
        }

        // get the string transfer associated with this box (we do this when we're about
        //  to repaint the screen and afterwards do 'Focus'. So let's keep track of what's
        //  selected so that when we eventually do Focus, we can set the selected text again.
        protected static int _SelectionStart, _SelectionLength;

        protected bool HasStringTransfer
        {
            get
            {
                return (Tag != null) && (Tag is StringTransfer);
            }
        }

        public StringTransfer MyStringTransfer
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasStringTransfer);
                _SelectionStart = SelectionStart;
                _SelectionLength = SelectionLength;
                return (StringTransfer) Tag;
            }
        }

        public static void ResetSelection()
        {
            _SelectionStart = _SelectionLength = 0;
        }

        public new bool Focus()
        {
            _ctrlVerseParent.Focus();
            base.Focus();
            Visible = true;
            if (_SelectionStart + _SelectionLength <= Text.Length)
            {
                SelectionStart = _SelectionStart;
                SelectionLength = _SelectionLength;
            }

            return true;
        }

        public void InsureLanguageNameLabel()
        {
            if (!String.IsNullOrEmpty(Text) || (Controls.Count != 0))
                return;

            var label = new Label
                            {
                                AutoSize = true,
                                ForeColor = SystemColors.AppWorkspace,
                                Location = new Point(2, 3),
                                Text = _strLangName,
                                Name = Name + _strLangName
                            };
            label.MouseDown += LabelMouseDown;
            Controls.Add(label);
        }

        static void LabelMouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is Label);
            var label = sender as Label;
            if (label != null)
            {
                label.Hide();
                System.Diagnostics.Debug.Assert((label.Parent != null) && (label.Parent is CtrlTextBox));
                label.Parent.Focus();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Console.WriteLine(String.Format("KeyCode: {0}; KeyData: {1}, KeyValue: {2}",
                e.KeyCode,
                e.KeyData,
                e.KeyValue));

            var form = FindForm();
            if (!(form is StoryEditor))
                return;

            var theSe = (StoryEditor)form;
            try
            {
                // certain keys (like arrow keys), we just want to allow in any case.
                if (!IsKeyAutomaticallyAllowed(theSe, e))
                {
                    if (!theSe.IsInStoriesSet)
                        throw theSe.CantEditOldStoriesEx;

                    // if the creator has defined a particular required editor (e.g. for consultant notes,
                    //  the *mentor* must be a *consultant*), then throw if we don't have one and always
                    //  allow the edit otherwise (since no one else can, we don't have to worry about conflicts).
                    if (_delegateRequiredEditorCheck != null)    // ... i.e. a req. editor checking delegate is defined
                    {
                        // throws if failure
                        _delegateRequiredEditorCheck(theSe.LoggedOnMember.MemberType, _eRequiredEditor);
                    }

                    // finally, the last possible blockage is if the currently logged on member isn't the 
                    //  right editor for the state we are in (which has to do with who has the edit token)
                    StoryData theStory = theSe.TheCurrentStory;
                    theSe.LoggedOnMember.ThrowIfEditIsntAllowed(theStory);

                    // one more finally, don't allow it if it's blocked by the consultant
                    ProjectSettings projSettings = theSe.StoryProject.ProjSettings;
                    if (TeamMemberData.IsUser(theSe.LoggedOnMember.MemberType,
                        TeamMemberData.UserTypes.ProjectFacilitator))
                    {
                        ProjectSettings.LanguageInfo li;
                        if (!CheckForTaskPermission((li = projSettings.Vernacular), StoryEditor.TextFields.Vernacular, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.VernacularLangFields))
                            || !CheckForTaskPermission((li = projSettings.NationalBT), StoryEditor.TextFields.NationalBt, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.NationalBtLangFields))
                            || !CheckForTaskPermission((li = projSettings.InternationalBT), StoryEditor.TextFields.InternationalBt, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.InternationalBtFields))
                            || !CheckForTaskPermission((li = projSettings.FreeTranslation), StoryEditor.TextFields.FreeTranslation, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.FreeTranslationFields)))
                        {
                            throw new ApplicationException(
                                String.Format(Localizer.Str("The consultant hasn't given you permission to edit the '{0}' language fields"),
                                              li.LangName));
                        }
                    }
                }

                // if we get here, we're all good!
                base.OnKeyDown(e);
                theSe.Modified = true;  // to trigger save if exit.
                theSe.LastKeyPressedTimeStamp = DateTime.Now;   // so we can delay the autosave while typing

                // update the status bar (in case we previously put an error there
                theSe.ResetStatusBar();
            }
            catch (Exception ex)
            {
                Console.Beep();
                if (theSe != null)
                    theSe.SetStatusBar(String.Format("Error: {0}", ex.Message));
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private bool CheckForTaskPermission(ProjectSettings.LanguageInfo li, StoryEditor.TextFields eType, bool isTaskOn)
        {
            if (_eFieldType == eType)
                return (!li.HasData || isTaskOn);
            return true;
        }

        // if we've click (or tabbed) into another edit box, then the 'last place we were
        //  searching from' indices are no longer valid.
        protected void ClearSearchIndices()
        {
            var form = _ctrlVerseParent.FindForm();
            if (!(form is StoryEditor))
                return;
            var theSe = (StoryEditor)form;
            if (theSe.m_frmFind != null)
                theSe.m_frmFind.ResetSearchStartParameters();
        }

        /*
         * nah... this wasn't such a good idea
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                try
                {
                    // make sure the current user *can* edit this TB
                    StoryEditor theSE;
                    if (!_ctrlVerseParent.CheckForProperEditToken(out theSE))
                        return;

                    if (_delegateRequiredEditorCheck != null)    // ... i.e. a req. editor checking delegate is defined
                    {
                        // throws if failure
                        _delegateRequiredEditorCheck(theSE.LoggedOnMember.MemberType, _eRequiredEditor);
                    }

                    drgevent.Effect = DragDropEffects.Copy;
                }
                catch { }   // noop
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(NetBibleViewer)))
            {
                try
                {
                    // make sure the current user *can* edit this TB
                    StoryEditor theSE;
                    if (!_ctrlVerseParent.CheckForProperEditToken(out theSE))
                        return;

                    NetBibleViewer theNetBibleViewer = (NetBibleViewer)drgevent.Data.GetData(typeof(NetBibleViewer));
                    SelectedText = theNetBibleViewer.ScriptureReference;

                    Focus();

                    // indicate that we've changed something so that we don't exit without offering
                    //  to save.
                    theSE.Modified = true;
                }
                catch { }   // noop
            }
        }
        */

        internal static CtrlTextBox _inTextBox;
        internal static int _nLastVerse = -1;

        protected override void OnEnter(EventArgs e)
        {
            KeepTrackOfLastTextBoxSelected();
            base.OnEnter(e);
            if (Controls.Count > 0)
            {
                Control label = Controls[0];
                if (label.Visible)
                    Controls[0].Hide();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            KeepTrackOfLastTextBoxSelected();

            if (StoryEditor.TextPaster != null)
            {
                StoryEditor.TextPaster.TriggerPaste(e.Button == MouseButtons.Left, _inTextBox);
            }
            base.OnMouseDown(e);
        }

        protected void KeepTrackOfLastTextBoxSelected()
        {
            _inTextBox = this;
            try
            {
                if (!String.IsNullOrEmpty(_strKeyboardName))
                    // Keyboard.Controller.SetKeyboard(_strKeyboardName);
                    Keyboard.Controller.GetKeyboard(_strKeyboardName).Activate();
            }
            catch (System.IO.FileLoadException)
            {
#if !DEBUG
                // I'm so sick of getting this while debugging... giving it up
                throw;
#endif
            }

            if (_nLastVerse != _ctrlVerseParent.VerseNumber)
            {
                _nLastVerse = _ctrlVerseParent.VerseNumber;

                // start a timer that will wake up shortly and set the focus to the other panes as well.
                _ctrlVerseParent.TheSE.SetFocusTimer(_nLastVerse);
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            _inTextBox = null;
            try
            {
                if ((Controls.Count > 0) && String.IsNullOrEmpty(Text))
                {
                    Control label = Controls[0];
                    if (!label.Visible)
                        label.Show();
                }
                Program.ActivateDefaultKeyboard();
            }
            catch (System.IO.FileLoadException)
            {
#if !DEBUG
                // I'm so sick of getting this while debugging... giving it up
                throw;
#endif
            }
            base.OnLeave(e);
        }

        protected bool IsKeyAutomaticallyAllowed(StoryEditor theSE, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys keyData = e.KeyData;
            System.Diagnostics.Debug.WriteLine(String.Format("keyCode: {0}; keyData: {1}", keyCode, keyData));
            
            // pass on the control-S
            if (keyData == (Keys.Control | Keys.S))
            {
                theSE.SaveClicked();
                e.Handled = true;
                return true;
            }

            if (keyData == (Keys.Control | Keys.A))
            {
                SelectAll();
                e.Handled = true;
                return true;
            }

            if ((keyData == (Keys.Control | Keys.C))    // copy
                || (keyData == (Keys.Control | Keys.ControlKey)) // Control
                )// Replace
                return true;

            if (keyData == (Keys.Control | Keys.F)) // Find
            {
                theSE.LaunchSearchForm();
                e.Handled = true;
                return true;
            }
            
            if (keyData == (Keys.Control | Keys.H)) // Replace
            {
                theSE.LaunchReplaceForm();
                e.Handled = true;
                return true;
            }

            if (keyData == Keys.F3)
            {
                theSE.findNextToolStripMenuItem_Click(null, null);
                e.Handled = true;
                return true;
            }

            if (keyData == Keys.F1)
                return true;

            if (keyData == (Keys.Control | Keys.Down))
            {
                // start a timer that will wake up shortly and set the focus to the other panes as well.
                _ctrlVerseParent.TheSE.SetFocusTimer(_ctrlVerseParent.VerseNumber + 1);
                return true;
            }

            switch (keyCode)
            {
                case Keys.Left:
                    return true;
                case Keys.Right:
                    return true;
                case Keys.Up:
                    return true;
                case Keys.Down:
                    return true;
                case Keys.Menu:
                    return true;
                case Keys.ShiftKey:
                    return true;
            }
            return false;
        }

        protected void InitComponent(string strLabel)
        {
            var ctxMenu = CreateContextMenuStrip(strLabel);
            ContextMenuStrip = ctxMenu;
            Multiline = true;
            Dock = DockStyle.Fill;
            HideSelection = false;
            AllowDrop = true;
            MouseUp += CtrlTextBox_MouseUp;
            MouseWheel += CtrlTextBox_MouseWheel;
        }

        private ContextMenuStrip CreateContextMenuStrip(string strLabel)
        {
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Items.Add(StoryEditor.CstrAddNoteOnSelected, null, onAddNewNote);
            ctxMenu.Items.Add(StoryEditor.CstrAddNoteToSelfOnSelected, null, onAddNoteToSelf);
            ctxMenu.Items.Add(StoryEditor.CstrJumpToReference, null, onJumpToBibleRef);
            ctxMenu.Items.Add(StoryEditor.CstrConcordanceSearch, null, onConcordanceSearch);
            ctxMenu.Items.Add(StoryEditor.CstrAddLnCNote, null, onAddLnCNote);
            ctxMenu.Items.Add(new ToolStripSeparator());
            if (StoryEditor.IsTestQuestionBox(strLabel))
            {
                ctxMenu.Items.Add(StoryEditor.CstrAddAnswerBox, null, onAddAnswerBox);
                ctxMenu.Items.Add(new ToolStripSeparator());
            }
            else if (StoryEditor.IsTqAnswerBox(strLabel))
            {
                ctxMenu.Items.Add(StoryEditor.CstrRemAnswerBox, null, onRemAnswerBox);
                ctxMenu.Items.Add(StoryEditor.CstrRemAnswerChangeUns, null, onChangeUns);
                ctxMenu.Items.Add(new ToolStripSeparator());
            }

            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToNational, null, onGlossTextToNational);
            ctxMenu.Items.Add(StoryEditor.CstrGlossTextToEnglish, null, onGlossTextToEnglish);
            ctxMenu.Items.Add(StoryEditor.CstrReorderWords, null, onReorderWords);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(StoryEditor.CstrCutSelected, null, onCutSelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopySelected, null, onCopySelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrCopyOriginalSelected, null, onCopyOriginalText);
            ctxMenu.Items.Add(StoryEditor.CstrPasteSelected, null, onPasteSelectedText);
            ctxMenu.Items.Add(StoryEditor.CstrUndo, null, onUndo);
            ctxMenu.Opening += CtxMenuOpening;
            return ctxMenu;
        }

        private void onGlossTextToNational(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Assert((NationalBtSibling != null) && (_ctrlVerseParent != null)
                    && (_eFieldType == StoryEditor.TextFields.Vernacular));
                if (!MyStringTransfer.HasData)
                    return;

                var dlg = new GlossingForm(_ctrlVerseParent.TheSE.StoryProject.ProjSettings,
                                           MyStringTransfer.ToString(),
                                           ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.
                                               VernacularToNationalBt,
                                           _ctrlVerseParent.TheSE.LoggedOnMember,
                                           _ctrlVerseParent.TheSE.advancedUseWordBreaks.Checked,
                                           MyStringTransfer.Transliterator);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    NationalBtSibling.Text = dlg.TargetSentence;

                    // but only update the source data if it wasn't being transliterated
                    if (MyStringTransfer.Transliterator == null)
                        Text = dlg.SourceSentence;  // cause the user might have corrected some spelling

                    _ctrlVerseParent.TheSE.Modified = true;
                    if (dlg.DoReorder)
                    {
                        var dlgReorder = new ReorderWordsForm(NationalBtSibling.MyStringTransfer, 
                                                              NationalBtSibling.Font,
                                                              _ctrlVerseParent.TheSE.StoryProject.ProjSettings.NationalBT.FullStop);
                        if (dlgReorder.ShowDialog() == DialogResult.OK)
                            NationalBtSibling.Text = dlgReorder.ReorderedText;
                    }
                    
                    NationalBtSibling.Focus();
                }
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
        }

        private void onGlossTextToEnglish(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Assert((EnglishBtSibling != null) && (_ctrlVerseParent != null)
                                                &&
                                                ((_eFieldType == StoryEditor.TextFields.Vernacular) ||
                                                 (_eFieldType == StoryEditor.TextFields.NationalBt)));
                if (!MyStringTransfer.HasData)
                    return;
                ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection = (_eFieldType ==
                                                                                        StoryEditor.TextFields.
                                                                                            Vernacular)
                                                                                           ? ProjectSettings.
                                                                                                 AdaptItConfiguration.
                                                                                                 AdaptItBtDirection.
                                                                                                 VernacularToInternationalBt
                                                                                           : ProjectSettings.
                                                                                                 AdaptItConfiguration.
                                                                                                 AdaptItBtDirection.
                                                                                                 NationalBtToInternationalBt;
                var dlg = new GlossingForm(_ctrlVerseParent.TheSE.StoryProject.ProjSettings,
                                           MyStringTransfer.ToString(), eBtDirection,
                                           _ctrlVerseParent.TheSE.LoggedOnMember,
                                           _ctrlVerseParent.TheSE.advancedUseWordBreaks.Checked,
                                           MyStringTransfer.Transliterator);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    EnglishBtSibling.Text = dlg.TargetSentence;

                    // but only update the source data if it wasn't being transliterated
                    if (MyStringTransfer.Transliterator == null)
                        Text = dlg.SourceSentence;  // because the user may have corrected spelling

                    _ctrlVerseParent.TheSE.Modified = true;

                    if (dlg.DoReorder)
                    {
                        var dlgReorder = new ReorderWordsForm(EnglishBtSibling.MyStringTransfer,
                            EnglishBtSibling.Font,
                            _ctrlVerseParent.TheSE.StoryProject.ProjSettings.InternationalBT.FullStop);
                        if (dlgReorder.ShowDialog() == DialogResult.OK)
                            EnglishBtSibling.Text = dlgReorder.ReorderedText;
                    }

                    EnglishBtSibling.Focus();
                }
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
        }

        private void onReorderWords(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(HasStringTransfer);
            var dlg = new ReorderWordsForm(MyStringTransfer, Font, _strFullStop);
            if (dlg.ShowDialog() == DialogResult.OK)
                Text = dlg.ReorderedText; // should trigger an update of MyStringTransfer
        }

        private void onAddLnCNote(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) && (_ctrlVerseParent.TheSE != null));
            _ctrlVerseParent.TheSE.AddLnCNote();
        }

        void CtxMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // don't ask... I'm not sure why Items.ContainsKey isn't finding this...
            foreach (ToolStripItem x in ContextMenuStrip.Items)
                if (x.Text == StoryEditor.CstrCopyOriginalSelected)
                {
                    x.Enabled = (HasStringTransfer && (MyStringTransfer.Transliterator != null));
                }
                else if (x.Text == StoryEditor.CstrReorderWords)
                {
                    x.Enabled = HasStringTransfer;
                }
                else if (x.Text == StoryEditor.CstrGlossTextToNational)
                {
                    x.Visible = (NationalBtSibling != null);
                }
                else if (x.Text == StoryEditor.CstrGlossTextToEnglish)
                {
                    x.Visible = (EnglishBtSibling != null);
                }
                else if (x.Text == StoryEditor.CstrAddLnCNote)
                    CheckForLnCNoteLookup((ToolStripMenuItem)x);
        }

        private void CheckForLnCNoteLookup(ToolStripMenuItem x)
        {
            x.DropDownItems.Clear();
            var mapFoundString2LnCnote = _ctrlVerseParent.TheSE.StoryProject.LnCNotes.FindHits(SelectedText, _eFieldType);
            foreach (KeyValuePair<string, LnCNote> kvp in mapFoundString2LnCnote)
            {
                ToolStripItem tsi = x.DropDownItems.Add(kvp.Key, null, onLookupLnCnote);
                tsi.Tag = kvp.Value;
                tsi.Font = Font;
            }
        }

        private void onLookupLnCnote(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripItem;
            if (tsi != null)
            {
                var note = (LnCNote)tsi.Tag;
                var dlg = new AddLnCNoteForm(_ctrlVerseParent.TheSE, note) {Text = Localizer.Str("Edit L & C Note")};
                if ((dlg.ShowDialog() == DialogResult.OK) && (note != null))
                    _ctrlVerseParent.TheSE.Modified = true;
            }
        }

        void CtrlTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            _ctrlVerseParent.SendScrollWheelToParentFormLayoutPanel(e);
        }

        private void onJumpToBibleRef(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) && (_ctrlVerseParent.TheSE != null));
            _ctrlVerseParent.TheSE.SetNetBibleVerse(SelectedText);
        }

        private void onConcordanceSearch(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) && (_ctrlVerseParent.TheSE != null));
            _ctrlVerseParent.TheSE.concordanceToolStripMenuItem_Click(null, null);
        }

        private void onAddAnswerBox(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) && (_ctrlVerseParent.TheSE != null));
            StoryEditor theSE;
            if (!_ctrlVerseParent.CheckForProperEditToken(out theSE))
                return;

            var theVerseCtrl = _ctrlVerseParent as VerseBtControl;
            if (theVerseCtrl == null)
                return;

            var testQuestionData = StoryEditor.GetTestQuestionData(_strLabel, theVerseCtrl);
            LineMemberData theNewAnswer;
            if (theSE.AddSingleTestResult(testQuestionData, out theNewAnswer))
                theVerseCtrl.UpdateViewOfThisVerse(theSE);
        }

        private void onRemAnswerBox(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) 
                && (_ctrlVerseParent.TheSE != null));
            StoryEditor theSE;
            if (!_ctrlVerseParent.CheckForProperEditToken(out theSE))
                return;

            var theVerseCtrl = _ctrlVerseParent as VerseBtControl;
            if (theVerseCtrl == null)
                return;

            AnswersData answers;
            var answerData = theSE.GetTqAnswerData(_strLabel, theVerseCtrl, out answers);
            if (answerData != null)
            {
                answers.Remove(answerData);
                theVerseCtrl.UpdateViewOfThisVerse(theSE);
            }
        }

        private void onChangeUns(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) 
                && (_ctrlVerseParent.TheSE != null));
            StoryEditor theSE;
            if (!_ctrlVerseParent.CheckForProperEditToken(out theSE))
                return;

            var theVerseCtrl = _ctrlVerseParent as VerseBtControl;
            if (theVerseCtrl == null)
                return;

            if (theSE.ChangeAnswerBoxUns(_strLabel, theVerseCtrl))
                theVerseCtrl.UpdateViewOfThisVerse(theSE);
        }

        void CtrlTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            ClearSearchIndices();
        }

        private void onAddNewNote(object sender, EventArgs e)
        {
            _ctrlVerseParent.TheSE.AddNoteAbout(_ctrlVerseParent, false);
        }

        private void onAddNoteToSelf(object sender, EventArgs e)
        {
            _ctrlVerseParent.TheSE.AddNoteAbout(_ctrlVerseParent, true);
        }

        private static void onCutSelectedText(object sender, EventArgs e)
        {
            if (_inTextBox != null)
            {
                StoryEditor theSE;
                if (!_inTextBox._ctrlVerseParent.CheckForProperEditToken(out theSE))
                    return;
                _inTextBox.Cut();
                theSE.Modified = true;
            }
        }

        private static void onCopySelectedText(object sender, EventArgs e)
        {
            if (_inTextBox != null) _inTextBox.Copy();
        }

        private void onCopyOriginalText(object sender, EventArgs e)
        {
            if (HasStringTransfer && (MyStringTransfer.Transliterator != null))
            {
                try
                {
                    Clipboard.SetText(MyStringTransfer.ToString(), TextDataFormat.UnicodeText);
                }
                catch { }   // sometimes it's just busy... just ignore it
            }
            else
                onCopySelectedText(sender, e);
        }

        private static void onPasteSelectedText(object sender, EventArgs e)
        {
            if (_inTextBox != null)
            {
                StoryEditor theSE;
                if (!_inTextBox._ctrlVerseParent.CheckForProperEditToken(out theSE))
                    return;
                _inTextBox.Paste();
                theSE.Modified = true;

                // deal with label control (by pretending there was a mouse down, which
                //  has the side effect we want)
                if (_inTextBox.Controls.Count > 0)
                    LabelMouseDown(_inTextBox.Controls[0], null);
            }
        }

        private static void onUndo(object sender, EventArgs e)
        {
            if (_inTextBox != null)
            {
                StoryEditor theSE;
                if (!_inTextBox._ctrlVerseParent.CheckForProperEditToken(out theSE))
                    return;
                _inTextBox.Undo();
                theSE.Modified = true;
            }
        }
    }
}
