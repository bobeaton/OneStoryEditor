using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OneStoryProjectEditor
{
    public class CtrlTextBox : TextBox
    {
        protected StoryStageLogic _stageLogic = null;
        protected ResizableControl _ctrlParent = null;

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

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData, Font font, Color colorText)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            Font = font;
            ForeColor = colorText;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _ctrlParent = ctrlParent;
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
                        "Unable to edit file! Try rebooting and if it persists, contact bob_eaton@sall.com");

                if (IsKeyAllowed(e.KeyCode) || _stageLogic.IsEditAllowed)
                    base.OnKeyDown(e);
                theSE.Modified = true;
                theSE.SetViewBasedOnProjectStage(_stageLogic.ProjectStage);
            }
            catch (Exception ex)
            {
                if (theSE != null)
                    theSE.SetStatusBar(String.Format("Error: {0}", ex.Message), null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected bool IsKeyAllowed(Keys keyCode)
        {
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
            }
            return false;
        }
    }
}
