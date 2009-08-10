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
        protected StoryEditor _theSE = null;

        public CtrlTextBox(string strName, ResizableControl ctrlParent, StringTransfer stData)
        {
            Name = strName;
            Multiline = true;
            Dock = DockStyle.Fill;
            stData.SetAssociation(this);
            TextChanged += new EventHandler(ctrlParent.textBox_TextChanged);
            System.Diagnostics.Debug.Assert(ctrlParent.StageLogic != null);
            _stageLogic = ctrlParent.StageLogic;
            _theSE = (StoryEditor)ctrlParent.FindForm();
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
            _theSE = (StoryEditor)ctrlParent.FindForm();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Console.WriteLine(String.Format("KeyCode: {0}; KeyData: {1}, KeyValue: {2}",
                e.KeyCode.ToString(),
                e.KeyData.ToString(),
                e.KeyValue.ToString()));

            try
            {
                if (IsKeyAllowed(e.KeyCode) || _stageLogic.IsEditAllowed)
                    base.OnKeyDown(e);
            }
            catch (Exception ex)
            {
                _theSE.SetStatusBar("Error occurred. Hover the mouse here", ex.Message);
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
