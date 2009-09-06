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
        protected TeamMemberData.UserTypes _eRequiredEditor = TeamMemberData.UserTypes.eUndefined;
        protected StoryStageLogic _stageLogic = null;
        protected ResizableControl _ctrlParent = null;
        protected string _strKeyboardName = null;

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData, 
            TeamMemberData.UserTypes eRequiredEditor)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
            _eRequiredEditor = eRequiredEditor; // can only be edited by this member!
        }

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData,
            ProjectSettings.LanguageInfo li)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            Font = li.LangFont;
            ForeColor = li.FontColor;
            if (li.IsRTL)
                RightToLeft = RightToLeft.Yes;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
            _strKeyboardName = li.Keyboard;
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
                if (!IsKeyAutomaticallyAllowed(e.KeyCode, e.KeyData))
                {
                    // if the creator has defined a particular required editor (e.g. for consultant notes,
                    //  the *mentor* must be a *consultant*), then throw if we don't have one and always
                    //  allow the edit otherwise (since no one else can, we don't have to worry about conflicts).
                    if (_eRequiredEditor != TeamMemberData.UserTypes.eUndefined)    // ... i.e. a req. editor is defined
                    {
                        // then throw if this is not him or her (fall thru otherwise)
                        if (theSE.LoggedOnMember.MemberType != _eRequiredEditor)
                            throw new ApplicationException(String.Format("Only a '{0}' can edit this field", TeamMemberData.GetMemberTypeAsDisplayString(_eRequiredEditor)));
                    }

                    // finally, the last possible blockage is if the currently logged on member isn't the 
                    //  right editor for the state we are in (which has to do with who has the edit token)
                    else if (!_stageLogic.IsEditAllowed(theSE.LoggedOnMember))
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

        protected override void OnEnter(EventArgs e)
        {
            if (!String.IsNullOrEmpty(_strKeyboardName))
                KeyboardController.ActivateKeyboard(_strKeyboardName);

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            KeyboardController.DeactivateKeyboard();
            base.OnLeave(e);
        }
        protected bool IsKeyAutomaticallyAllowed(Keys keyCode, Keys keyData)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("keyCode: {0}; keyData: {1}", keyCode, keyData));
            
            // pass on the control-S
            if (keyData == (Keys.Control | Keys.S))
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
