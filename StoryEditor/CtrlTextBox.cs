using System;
using System.Windows.Forms;
using System.Drawing;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public class CtrlTextBox : TextBox
    {
        protected StoryStageLogic _stageLogic = null;
        internal VerseControl _ctrlVerseParent = null;
        protected string _strKeyboardName = null;
        internal string _strLabel = null;
        protected ContextMenuStrip _ctxMenu = null;

        public delegate void ThrowIfNotCorrectEditor(TeamMemberData.UserTypes eLoggedInMember, TeamMemberData.UserTypes eRequiredEditor);
        protected ThrowIfNotCorrectEditor _delegateRequiredEditorCheck;
        protected TeamMemberData.UserTypes _eRequiredEditor = TeamMemberData.UserTypes.eUndefined;

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

        public CtrlTextBox(string strName, VerseControl ctrlVerseParent, ResizableControl ctrlParent, StringTransfer stData,
            ProjectSettings.LanguageInfo li, string strOverrideKeyboard, string strLabel)
        {
            InitComponent();
            Name = strName;
            _strLabel = strLabel;
            Font = li.LangFont;
            ForeColor = li.FontColor;
            if (li.IsRTL)
                RightToLeft = RightToLeft.Yes;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlVerseParent = ctrlVerseParent;
            _strKeyboardName = (String.IsNullOrEmpty(strOverrideKeyboard)) ? li.DefaultKeyboard : strOverrideKeyboard;
        }

        public new bool Focus()
        {
            _ctrlVerseParent.Focus();
            base.Focus();
            Visible = true;
            return true;
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
                    if (!_stageLogic.IsEditAllowed(theSE.LoggedOnMember))
                        throw _stageLogic.WrongMemberTypeEx;
                }

                // if we get here, we're all good!
                base.OnKeyDown(e);
                theSE.Modified = true;  // to trigger save if exit.

                // update the status bar (in case we previously put an error there
                StoryStageLogic.StateTransition st = StoryStageLogic.stateTransitions[_stageLogic.ProjectStage];
                theSE.SetStatusBar(String.Format("{0}  Press F1 for instructions", st.StageDisplayString));
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

                    // finally, the last possible blockage is if the currently logged on member isn't the 
                    //  right editor for the state we are in (which has to do with who has the edit token)
                    if (!_stageLogic.IsEditAllowed(theSE.LoggedOnMember))
                        throw _stageLogic.WrongMemberTypeEx;

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

                    // indicate that we've changed something so that we don't exit without offering
                    //  to save.
                    theSE.Modified = true;
                }
                catch { }   // noop
            }
        }

        internal static CtrlTextBox _inTextBox = null;
        internal static int _nLastVerse = -1;

        protected override void OnEnter(EventArgs e)
        {
            KeepTrackOfLastTextBoxSelected();
            base.OnEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            KeepTrackOfLastTextBoxSelected();
            base.OnMouseDown(e);
        }

        protected void KeepTrackOfLastTextBoxSelected()
        {
            _inTextBox = this;
            if (!String.IsNullOrEmpty(_strKeyboardName))
                KeyboardController.ActivateKeyboard(_strKeyboardName);

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
            KeyboardController.DeactivateKeyboard();
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
                theSE.findNextToolStripMenuItem_Click(null, null);
                e.Handled = true;
                return true;
            }
            
            if (keyData == (Keys.Control | Keys.H)) // Replace
            {
                theSE.replaceToolStripMenuItem_Click(null, null);
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
        protected const string CstrCutSelected = "C&ut";
        protected const string CstrCopySelected = "&Copy";
        protected const string CstrPasteSelected = "&Paste";
        protected const string CstrUndo = "U&ndo";

        protected void InitComponent()
        {
            _ctxMenu = new ContextMenuStrip();
            _ctxMenu.Items.Add(CstrAddNoteOnSelected, null, onAddNewNote);
            _ctxMenu.Items.Add(new ToolStripSeparator());
            _ctxMenu.Items.Add(CstrCutSelected, null, onCutSelectedText);
            _ctxMenu.Items.Add(CstrCopySelected, null, onCopySelectedText);
            _ctxMenu.Items.Add(CstrPasteSelected, null, onPasteSelectedText);
            _ctxMenu.Items.Add(CstrUndo, null, onUndo);

            ContextMenuStrip = _ctxMenu;
            Multiline = true;
            Dock = DockStyle.Fill;
            HideSelection = false;
            AllowDrop = true;
            MouseUp += new MouseEventHandler(CtrlTextBox_MouseUp);
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
            if (_inTextBox != null) _inTextBox.Cut();
        }

        private static void onCopySelectedText(object sender, EventArgs e)
        {
            if (_inTextBox != null) _inTextBox.Copy();
        }

        private static void onPasteSelectedText(object sender, EventArgs e)
        {
            if (_inTextBox != null) _inTextBox.Paste();
        }

        private static void onUndo(object sender, EventArgs e)
        {
            if (_inTextBox != null) _inTextBox.Undo();
        }
    }
}
