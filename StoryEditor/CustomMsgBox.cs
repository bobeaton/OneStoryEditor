using NetLoc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class CustomMsgBox : Form
    {
        private CustomMsgBox()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        /// <summary>
        /// caller passes the text to use in the frame, and message body, as well as the two left-most buttons
        /// </summary>
        /// <param name="frameText"></param>
        /// <param name="messageBody"></param>
        /// <param name="okButtonText"></param>
        /// <param name="retryButtonText"></param>
        public CustomMsgBox(string frameText, string messageBody, string okButtonText, string retryButtonText)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            Text = frameText;
            textBoxMessage.Text = messageBody;
            buttonOK.Text = okButtonText;
            buttonRetry.Text = retryButtonText;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonOther_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
