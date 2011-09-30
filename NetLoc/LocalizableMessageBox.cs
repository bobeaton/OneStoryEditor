using System;
using System.Windows.Forms;

namespace NetLoc
{
    public partial class LocalizableMessageBox : Form
    {
        /// <summary>
        /// Replacement for MessageBox.Show so that you can localize the button
        /// labels. Also includes an "InputBox" method to have a localizable 
        /// version of Microsoft.VisualBasic.Interaction.InputBox
        /// </summary>
        public LocalizableMessageBox()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public static DialogResult Show(string strMessage, string strCaption, 
            MessageBoxButtons buttons)
        {
            var dlg = new LocalizableMessageBox
                          {
                              labelMessage = {Text = strMessage},
                              Text = strCaption
                          };

            if (Localizer.Default.LocLanguage.Font != null)
                dlg.Font = Localizer.Default.LocLanguage.Font;

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    dlg.buttonRightMost.Text = LabelCancel;
                    dlg.buttonMiddle.Visible = true;
                    dlg.buttonMiddle.Text = LabelOk;
                    dlg.CancelButton = dlg.buttonRightMost;
                    dlg.AcceptButton = dlg.buttonMiddle;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    dlg.buttonRightMost.Text = LabelCancel;
                    dlg.buttonMiddle.Visible = true;
                    dlg.buttonMiddle.Text = LabelNo;
                    dlg.buttonLeftMost.Visible = true;
                    dlg.buttonLeftMost.Text = LabelYes;
                    dlg.CancelButton = dlg.buttonRightMost;
                    dlg.AcceptButton = dlg.buttonLeftMost;
                    break;
                case MessageBoxButtons.RetryCancel:
                    dlg.buttonRightMost.Text = LabelCancel;
                    dlg.buttonMiddle.Visible = true;
                    dlg.buttonMiddle.Text = LabelRetry;
                    dlg.CancelButton = dlg.buttonRightMost;
                    dlg.AcceptButton = dlg.buttonMiddle;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    dlg.buttonRightMost.Text = LabelIgnore;
                    dlg.buttonMiddle.Visible = true;
                    dlg.buttonMiddle.Text = LabelRetry;
                    dlg.buttonLeftMost.Visible = true;
                    dlg.buttonLeftMost.Text = LabelAbort;
                    dlg.CancelButton = dlg.buttonLeftMost;
                    dlg.AcceptButton = dlg.buttonRightMost;
                    break;
                case MessageBoxButtons.OK:
                    dlg.buttonRightMost.Text = LabelOk;
                    dlg.AcceptButton = dlg.buttonRightMost;
                    break;
                case MessageBoxButtons.YesNo:
                    dlg.buttonRightMost.Text = LabelNo;
                    dlg.buttonMiddle.Visible = true;
                    dlg.buttonMiddle.Text = LabelYes;
                    dlg.CancelButton = dlg.buttonRightMost;
                    dlg.AcceptButton = dlg.buttonMiddle;
                    break;
            }
            return dlg.ShowDialog();
        }

        public static DialogResult Show(string strMessage, string strCaption)
        {
            var dlg = new LocalizableMessageBox
                          {
                              labelMessage = {Text = strMessage},
                              buttonRightMost = {Text = LabelOk},
                              Text = strCaption
                          };

            if (Localizer.Default.LocLanguage.Font != null)
                dlg.Font = Localizer.Default.LocLanguage.Font;

            dlg.AcceptButton = dlg.buttonRightMost;
            return dlg.ShowDialog();
        }

        public static string InputBox(string strMessage, string strCaption,
            string strDefaultValue)
        {
            var dlg = new LocalizableMessageBox
                          {
                              labelMessage = {Text = strMessage},
                              textBoxInput = {Text = strDefaultValue, Visible = true},
                              buttonRightMost = {Text = LabelCancel},
                              buttonMiddle = {Text = LabelOk, Visible = true},
                              Text = strCaption
                          };

            if (Localizer.Default.LocLanguage.Font != null)
                dlg.Font = Localizer.Default.LocLanguage.Font;

            dlg.CancelButton = dlg.buttonRightMost;
            dlg.AcceptButton = dlg.buttonMiddle;
            return (dlg.ShowDialog() == DialogResult.OK)
                       ? dlg.textBoxInput.Text
                       : null;
        }

        private static string LabelOk
        {
            get { return Localizer.Str("&OK"); }
        }

        private static string LabelCancel
        {
            get { return Localizer.Str("&Cancel"); }
        }

        private static string LabelYes
        {
            get { return Localizer.Str("&Yes"); }
        }

        private static string LabelNo
        {
            get { return Localizer.Str("&No"); }
        }

        private static string LabelAbort
        {
            get { return Localizer.Str("&Abort"); }
        }

        private static string LabelRetry
        {
            get { return Localizer.Str("&Retry"); }
        }

        private static string LabelIgnore
        {
            get { return Localizer.Str("&Ignore"); }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is Button);
            var button = sender as Button;
            if (button == null)
                return;
            if (button.Text == LabelOk)
                DialogResult = DialogResult.OK;
            else if (button.Text == LabelCancel)
                DialogResult = DialogResult.Cancel;
            else if (button.Text == LabelYes)
                DialogResult = DialogResult.Yes;
            else if (button.Text == LabelNo)
                DialogResult = DialogResult.No;
            else if (button.Text == LabelAbort)
                DialogResult = DialogResult.Abort;
            else if (button.Text == LabelRetry)
                DialogResult = DialogResult.Retry;
            else if (button.Text == LabelIgnore)
                DialogResult = DialogResult.Ignore;
            else
                return;
            Close();
        }
    }
}
