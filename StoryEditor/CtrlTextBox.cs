using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Palaso.UI.WindowsForms.Keyboarding;

namespace OneStoryProjectEditor
{
    public class CtrlTextBox : TextBox
    {
        protected StoryStageLogic _stageLogic = null;
        protected ResizableControl _ctrlParent = null;
        protected string _strKeyboardName = null;

        public delegate void ThrowIfNotCorrectEditor(TeamMemberData.UserTypes eLoggedInMember, TeamMemberData.UserTypes eRequiredEditor);
        protected ThrowIfNotCorrectEditor _delegateRequiredEditorCheck;
        protected TeamMemberData.UserTypes _eRequiredEditor = TeamMemberData.UserTypes.eUndefined;

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
        }

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData,
            ThrowIfNotCorrectEditor delegateRequiredEditorCheck, TeamMemberData.UserTypes eRequiredEditor)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
            _delegateRequiredEditorCheck = delegateRequiredEditorCheck; // call to check if the proper member is logged in!
            _eRequiredEditor = eRequiredEditor;
        }

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData,
            ProjectSettings.LanguageInfo li, string strOverrideKeyboard)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            Font = li.LangFont;
            ForeColor = li.FontColor;
            if (li.IsRTL)
                RightToLeft = RightToLeft.Yes;
            stData.SetAssociation(this);
            TextChanged += ctrlParent.textBox_TextChanged;
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
            _strKeyboardName = (String.IsNullOrEmpty(strOverrideKeyboard)) ? li.DefaultKeyboard : strOverrideKeyboard;
        }

        public new bool Focus()
        {
            StoryEditor theSE = (StoryEditor)_ctrlParent.FindForm();
            _ctrlParent.Focus();
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

            StoryEditor theSE = (StoryEditor)_ctrlParent.FindForm();
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

        internal static CtrlTextBox _inTextBox = null;

        protected override void OnEnter(EventArgs e)
        {
            _inTextBox = this;
            if (!String.IsNullOrEmpty(_strKeyboardName))
                KeyboardController.ActivateKeyboard(_strKeyboardName);

            base.OnEnter(e);
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

            if (keyData == (Keys.Control | Keys.C))
                return true;

            if (keyData == Keys.F1)
                return true;

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
    }
}
