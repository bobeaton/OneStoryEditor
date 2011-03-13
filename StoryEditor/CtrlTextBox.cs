using System;
using System.Windows.Forms;
using System.Drawing;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public class CtrlTextBox : TextBox
    {
        protected StoryStageLogic _stageLogic;
        internal VerseControl _ctrlVerseParent;
        protected string _strKeyboardName;
        internal string _strLabel;
        protected string _strLangName;
        internal string _strFullStop;
        protected ContextMenuStrip _ctxMenu;
        internal CtrlTextBox NationalBtSibling;
        internal CtrlTextBox EnglishBtSibling;
        private StoryEditor.TextFieldType _eFieldType;

        public delegate void ThrowIfNotCorrectEditor(TeamMemberData.UserTypes eLoggedInMember, TeamMemberData.UserTypes eRequiredEditor);
        protected ThrowIfNotCorrectEditor _delegateRequiredEditorCheck;
        protected TeamMemberData.UserTypes _eRequiredEditor = TeamMemberData.UserTypes.eUndefined;

        /* old method for MultiLine and 'cn' lines, but it wasn't working well
        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, Font font,
            ResizableControl ctrlParent, StringTransfer stData, string strLabel)
        {
            InitComponent();
            Name = strName;
            Font = font;
            _strLabel = strLabel;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlVerseParent = ctrlVerseParent;
        }

        // was used by the ConNotes, which are now done differently
        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, ResizableControl ctrlParent, StringTransfer stData,
            ThrowIfNotCorrectEditor delegateRequiredEditorCheck, TeamMemberData.UserTypes eRequiredEditor)
        {
            InitComponent();
            Font = new Font("Arial Unicode MS", 12);
            Name = strName;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlVerseParent = ctrlVerseParent;
            _delegateRequiredEditorCheck = delegateRequiredEditorCheck; // call to check if the proper member is logged in!
            _eRequiredEditor = eRequiredEditor;
        }
        */

        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, 
            ResizableControl ctrlParent, StringTransfer stData, 
            ProjectSettings.LanguageInfo li, string strLabel, bool bAddTqFlag,
            StoryEditor.TextFieldType eFieldType, Color clrFont)
        {
            InitComponent(bAddTqFlag);
            Init(strName, strLabel, li, stData, ctrlParent, ctrlVerseParent, eFieldType, clrFont);
        }

        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, 
            ResizableControl ctrlParent, StringTransfer stData,
            ProjectSettings.LanguageInfo li, string strLabel,
            StoryEditor.TextFieldType eFieldType, Color clrFont)
        {
            InitComponent(false);
            Init(strName, strLabel, li, stData, ctrlParent, ctrlVerseParent, eFieldType,
                clrFont);
        }

        private void Init(string strName, string strLabel, ProjectSettings.LanguageInfo li, 
            StringTransfer stData, ResizableControl ctrlParent, VerseControl ctrlVerseParent,
            StoryEditor.TextFieldType eFieldType, Color clrFont)
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
            _stageLogic = ctrlParent.StageLogic;
            _ctrlVerseParent = ctrlVerseParent;
            _strKeyboardName = li.Keyboard;
            _strFullStop = li.FullStop;
            _eFieldType = eFieldType;
        }

        void CtrlTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            var theSE = (StoryEditor)_ctrlVerseParent.FindForm();
            if (theSE != null)
                theSE.CheckBiblePaneCursorPosition();
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

            StoryEditor theSE = (StoryEditor)_ctrlVerseParent.FindForm();
            try
            {
                if (theSE == null)
                    throw new ApplicationException(
                        "Unable to edit the file! Try rebooting and if it persists, contact bob_eaton@sall.com");

                // certain keys (like arrow keys), we just want to allow in any case.
                if (!IsKeyAutomaticallyAllowed(theSE, e))
                {
                    if (!theSE.IsInStoriesSet)
                        throw theSE.CantEditOldStoriesEx;

                    // if the creator has defined a particular required editor (e.g. for consultant notes,
                    //  the *mentor* must be a *consultant*), then throw if we don't have one and always
                    //  allow the edit otherwise (since no one else can, we don't have to worry about conflicts).
                    if (_delegateRequiredEditorCheck != null)    // ... i.e. a req. editor checking delegate is defined
                    {
                        // throws if failure
                        _delegateRequiredEditorCheck(theSE.LoggedOnMember.MemberType, _eRequiredEditor);
                    }

                    // finally, the last possible blockage is if the currently logged on member isn't the 
                    //  right editor for the state we are in (which has to do with who has the edit token)
                    theSE.LoggedOnMember.ThrowIfEditIsntAllowed(_stageLogic.MemberTypeWithEditToken);

                    // one more finally, don't allow it if it's blocked by the consultant
                    if ((theSE.StoryProject != null) 
                        && (theSE.theCurrentStory != null)
                        && (theSE.LoggedOnMember != null))
                    {
                        ProjectSettings projSettings = theSE.StoryProject.ProjSettings;
                        StoryData theStory = theSE.theCurrentStory;
                        if (theSE.LoggedOnMember.MemberType == TeamMemberData.UserTypes.eProjectFacilitator)
                        {
                            ProjectSettings.LanguageInfo li;
                            if (!CheckForTaskPermission((li = projSettings.Vernacular), StoryEditor.TextFieldType.Vernacular, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.VernacularLangFields))
                                || !CheckForTaskPermission((li = projSettings.NationalBT), StoryEditor.TextFieldType.NationalBt, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.NationalBtLangFields))
                                || !CheckForTaskPermission((li = projSettings.InternationalBT), StoryEditor.TextFieldType.InternationalBt, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.InternationalBtFields))
                                || !CheckForTaskPermission((li = projSettings.FreeTranslation), StoryEditor.TextFieldType.FreeTranslation, TasksPf.IsTaskOn(theStory.TasksAllowedPf, TasksPf.TaskSettings.FreeTranslationFields)))
                            {
                                throw new ApplicationException(
                                    String.Format(Properties.Resources.IDS_DontHaveTaskPermission,
                                                  li.LangName));
                            }
                        }
                    }
                }

                // if we get here, we're all good!
                base.OnKeyDown(e);
                theSE.Modified = true;  // to trigger save if exit.
                theSE.LastKeyPressedTimeStamp = DateTime.Now;   // so we can delay the autosave while typing

                // update the status bar (in case we previously put an error there
                theSE.ResetStatusBar();
            }
            catch (Exception ex)
            {
                Console.Beep();
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message));
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private bool CheckForTaskPermission(ProjectSettings.LanguageInfo li, StoryEditor.TextFieldType eType, bool isTaskOn)
        {
            if (_eFieldType == eType)
                return (!li.HasData || isTaskOn);
            return true;
        }

        // if we've click (or tabbed) into another edit box, then the 'last place we were
        //  searching from' indices are no longer valid.
        protected void ClearSearchIndices()
        {
            StoryEditor theSE = (StoryEditor)_ctrlVerseParent.FindForm();
            if (theSE.m_frmFind != null)
                theSE.m_frmFind.ResetSearchStartParameters();

        }

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

                    /* this was already done by CheckForProperEditToken above???
                    // finally, the last possible blockage is if the currently logged on member isn't the 
                    //  right editor for the state we are in (which has to do with who has the edit token)
                    theSE.LoggedOnMember.ThrowIfEditIsntAllowed(_stageLogic.MemberTypeWithEditToken);
                    */
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
            base.OnMouseDown(e);
        }

        protected void KeepTrackOfLastTextBoxSelected()
        {
            _inTextBox = this;
            try
            {
                if (!String.IsNullOrEmpty(_strKeyboardName))
                    KeyboardController.ActivateKeyboard(_strKeyboardName);
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
                _ctrlVerseParent.TheSE.myFocusTimer.Tag = _ctrlVerseParent.VerseNumber;
                _ctrlVerseParent.TheSE.myFocusTimer.Start();
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
                KeyboardController.DeactivateKeyboard();
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
                _ctrlVerseParent.TheSE.myFocusTimer.Tag = _ctrlVerseParent.VerseNumber + 1;
                _ctrlVerseParent.TheSE.myFocusTimer.Start();

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

        protected const string CstrAddNoteOnSelected = "&Add Note on Selected Text";
        protected const string CstrJumpToReference = "&Jump to Bible Reference";
        protected const string CstrConcordanceSearch = "Concordance &Search";
        protected const string CstrAddLnCNote = "Add &L&&C Note";
        protected const string CstrCutSelected = "C&ut";
        protected const string CstrCopySelected = "&Copy";
        protected const string CstrCopyOriginalSelected = "Copy &Original Text (before transliteration)";
        protected const string CstrPasteSelected = "&Paste";
        protected const string CstrUndo = "U&ndo";
        protected const string CstrAddAnswerBox = "Add Ans&wer Box";
        protected const string CstrReorderWords = "&Reorder words";
        protected const string CstrGlossTextToNational = "&Back-translate to National Language";
        protected const string CstrGlossTextToEnglish = "Back-translate to &English";

        protected void InitComponent(bool bAddAnswerBox)
        {
            _ctxMenu = new ContextMenuStrip();
            _ctxMenu.Items.Add(CstrAddNoteOnSelected, null, onAddNewNote);
            _ctxMenu.Items.Add(CstrJumpToReference, null, onJumpToBibleRef);
            _ctxMenu.Items.Add(CstrConcordanceSearch, null, onConcordanceSearch);
            _ctxMenu.Items.Add(CstrAddLnCNote, null, onAddLnCNote);
            _ctxMenu.Items.Add(new ToolStripSeparator());
            if (bAddAnswerBox)
            {
                _ctxMenu.Items.Add(CstrAddAnswerBox, null, onAddAnswerBox);
                _ctxMenu.Items.Add(new ToolStripSeparator());
            }

            _ctxMenu.Items.Add(CstrGlossTextToNational, null, onGlossTextToNational);
            _ctxMenu.Items.Add(CstrGlossTextToEnglish, null, onGlossTextToEnglish);
            _ctxMenu.Items.Add(CstrReorderWords, null, onReorderWords);
            _ctxMenu.Items.Add(new ToolStripSeparator());
            _ctxMenu.Items.Add(CstrCutSelected, null, onCutSelectedText);
            _ctxMenu.Items.Add(CstrCopySelected, null, onCopySelectedText);
            _ctxMenu.Items.Add(CstrCopyOriginalSelected, null, onCopyOriginalText);
            _ctxMenu.Items.Add(CstrPasteSelected, null, onPasteSelectedText);
            _ctxMenu.Items.Add(CstrUndo, null, onUndo);
            _ctxMenu.Opening += _ctxMenu_Opening;
            ContextMenuStrip = _ctxMenu;
            Multiline = true;
            Dock = DockStyle.Fill;
            HideSelection = false;
            AllowDrop = true;
            MouseUp += CtrlTextBox_MouseUp;
            MouseWheel += CtrlTextBox_MouseWheel;
        }

        private void onGlossTextToNational(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Assert((NationalBtSibling != null) && (_ctrlVerseParent != null)
                    && (_eFieldType == StoryEditor.TextFieldType.Vernacular));
                if (!MyStringTransfer.HasData)
                    return;

                var dlg = new GlossingForm(_ctrlVerseParent.TheSE.StoryProject.ProjSettings, 
                    MyStringTransfer.ToString(),
                    ProjectSettings.AdaptItConfiguration.AdaptItBtDirection.VernacularToNationalBt,
                    _ctrlVerseParent.TheSE.LoggedOnMember);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    NationalBtSibling.Text = dlg.TargetSentence;
                    Text = dlg.SourceSentence;  // cause the user might have corrected some spelling
                    _ctrlVerseParent.TheSE.Modified = true;
                    if (dlg.DoReorder)
                    {
                        var dlgReorder = new ReorderWordsForm(NationalBtSibling);
                        if (dlgReorder.ShowDialog() == DialogResult.OK)
                            NationalBtSibling.Text = dlgReorder.ReorderedText;
                    }
                    
                    NationalBtSibling.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void onGlossTextToEnglish(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Assert((EnglishBtSibling != null) && (_ctrlVerseParent != null)
                                                &&
                                                ((_eFieldType == StoryEditor.TextFieldType.Vernacular) ||
                                                 (_eFieldType == StoryEditor.TextFieldType.NationalBt)));
                if (!MyStringTransfer.HasData)
                    return;
                ProjectSettings.AdaptItConfiguration.AdaptItBtDirection eBtDirection = (_eFieldType ==
                                                                                        StoryEditor.TextFieldType.
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
                                           _ctrlVerseParent.TheSE.LoggedOnMember);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    EnglishBtSibling.Text = dlg.TargetSentence;
                    Text = dlg.SourceSentence;  // because the user may have corrected spelling
                    _ctrlVerseParent.TheSE.Modified = true;

                    if (dlg.DoReorder)
                    {
                        var dlgReorder = new ReorderWordsForm(EnglishBtSibling);
                        if (dlgReorder.ShowDialog() == DialogResult.OK)
                            EnglishBtSibling.Text = dlgReorder.ReorderedText;
                    }

                    EnglishBtSibling.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, OseResources.Properties.Resources.IDS_Caption);
            }
        }

        private void onReorderWords(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(HasStringTransfer);
            var dlg = new ReorderWordsForm(this);
            if (dlg.ShowDialog() == DialogResult.OK)
                Text = dlg.ReorderedText; // should trigger an update of MyStringTransfer
        }

        private void onAddLnCNote(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((_ctrlVerseParent != null) && (_ctrlVerseParent.TheSE != null));
            _ctrlVerseParent.TheSE.AddLnCNote();
        }

        void _ctxMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // don't ask... I'm not sure why Items.ContainsKey isn't finding this...
            foreach (ToolStripItem x in _ctxMenu.Items)
                if (x.Text == CstrCopyOriginalSelected)
                {
                    x.Enabled = (HasStringTransfer && (MyStringTransfer.Transliterator != null));
                }
                else if (x.Text == CstrReorderWords)
                {
                    x.Enabled = HasStringTransfer;
                }
                else if (x.Text == CstrGlossTextToNational)
                {
                    x.Visible = (NationalBtSibling != null);
                }
                else if (x.Text == CstrGlossTextToEnglish)
                {
                    x.Visible = (EnglishBtSibling != null);
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
            var theVerseCtrl = _ctrlVerseParent as VerseBtControl;
            if (theVerseCtrl == null)
                return;

            var testQuestionData = StoryEditor.GetTestQuestionData(_strLabel, theVerseCtrl);
            if (_ctrlVerseParent.TheSE.AddSingleTestResult(testQuestionData))
                theVerseCtrl.UpdateViewOfThisVerse(_ctrlVerseParent.TheSE);
        }

        void CtrlTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            ClearSearchIndices();
        }

        private void onAddNewNote(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = sender as ToolStripMenuItem;
            _ctrlVerseParent.TheSE.AddNoteAbout(_ctrlVerseParent);
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
