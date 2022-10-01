using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NetLoc
{
    public class MyLocalizableMessageBox : Form
    {
        private IContainer components = null;

        private TableLayoutPanel tableLayoutPanel1;

        private Button buttonRightMost;

        private Button buttonMiddle;

        private Button buttonLeftMost;

        private Label labelMessage;

        private TextBox textBoxInput;

        private static string LabelOk => Localizer.Str("&OK");

        private static string LabelCancel => Localizer.Str("&Cancel");

        private static string LabelYes => Localizer.Str("&Yes");

        private static string LabelNo => Localizer.Str("&No");

        private static string LabelAbort => Localizer.Str("&Abort");

        private static string LabelRetry => Localizer.Str("&Retry");

        private static string LabelIgnore => Localizer.Str("&Ignore");

        public MyLocalizableMessageBox()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public static DialogResult Show(string strMessage, string strCaption, MessageBoxButtons buttons)
        {
            MyLocalizableMessageBox MyLocalizableMessageBox = new MyLocalizableMessageBox();
            MyLocalizableMessageBox.labelMessage.Text = strMessage;
            MyLocalizableMessageBox.Text = strCaption;
            MyLocalizableMessageBox.StartPosition = FormStartPosition.CenterScreen;
            MyLocalizableMessageBox MyLocalizableMessageBox2 = MyLocalizableMessageBox;
            if (Localizer.Default.LocLanguage.Font != null)
            {
                MyLocalizableMessageBox2.Font = Localizer.Default.LocLanguage.Font;
            }

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelCancel;
                    MyLocalizableMessageBox2.buttonMiddle.Visible = true;
                    MyLocalizableMessageBox2.buttonMiddle.Text = LabelOk;
                    MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonRightMost;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonMiddle;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelCancel;
                    MyLocalizableMessageBox2.buttonMiddle.Visible = true;
                    MyLocalizableMessageBox2.buttonMiddle.Text = LabelNo;
                    MyLocalizableMessageBox2.buttonLeftMost.Visible = true;
                    MyLocalizableMessageBox2.buttonLeftMost.Text = LabelYes;
                    MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonRightMost;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonLeftMost;
                    break;
                case MessageBoxButtons.RetryCancel:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelCancel;
                    MyLocalizableMessageBox2.buttonMiddle.Visible = true;
                    MyLocalizableMessageBox2.buttonMiddle.Text = LabelRetry;
                    MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonRightMost;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonMiddle;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelIgnore;
                    MyLocalizableMessageBox2.buttonMiddle.Visible = true;
                    MyLocalizableMessageBox2.buttonMiddle.Text = LabelRetry;
                    MyLocalizableMessageBox2.buttonLeftMost.Visible = true;
                    MyLocalizableMessageBox2.buttonLeftMost.Text = LabelAbort;
                    MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonLeftMost;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonRightMost;
                    break;
                case MessageBoxButtons.OK:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelOk;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonRightMost;
                    break;
                case MessageBoxButtons.YesNo:
                    MyLocalizableMessageBox2.buttonRightMost.Text = LabelNo;
                    MyLocalizableMessageBox2.buttonMiddle.Visible = true;
                    MyLocalizableMessageBox2.buttonMiddle.Text = LabelYes;
                    MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonRightMost;
                    MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonMiddle;
                    break;
            }

            return MyLocalizableMessageBox2.ShowDialog();
        }

        public static DialogResult Show(string strMessage, string strCaption)
        {
            MyLocalizableMessageBox MyLocalizableMessageBox = new MyLocalizableMessageBox();
            MyLocalizableMessageBox.labelMessage.Text = strMessage;
            MyLocalizableMessageBox.buttonRightMost.Text = LabelOk;
            MyLocalizableMessageBox.Text = strCaption;
            MyLocalizableMessageBox.StartPosition = FormStartPosition.CenterParent;
            MyLocalizableMessageBox MyLocalizableMessageBox2 = MyLocalizableMessageBox;
            if (Localizer.Default.LocLanguage != null && Localizer.Default.LocLanguage.Font != null)
            {
                MyLocalizableMessageBox2.Font = Localizer.Default.LocLanguage.Font;
            }

            MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonRightMost;
            return MyLocalizableMessageBox2.ShowDialog();
        }

        public static string InputBox(string strMessage, string strCaption, string strDefaultValue)
        {
            MyLocalizableMessageBox MyLocalizableMessageBox = new MyLocalizableMessageBox();
            MyLocalizableMessageBox.labelMessage.Text = strMessage;
            MyLocalizableMessageBox.textBoxInput.Text = strDefaultValue;
            MyLocalizableMessageBox.textBoxInput.Visible = true;
            MyLocalizableMessageBox.buttonRightMost.Text = LabelCancel;
            MyLocalizableMessageBox.buttonMiddle.Text = LabelOk;
            MyLocalizableMessageBox.buttonMiddle.Visible = true;
            MyLocalizableMessageBox.Text = strCaption;
            MyLocalizableMessageBox.StartPosition = FormStartPosition.CenterParent;
            MyLocalizableMessageBox MyLocalizableMessageBox2 = MyLocalizableMessageBox;
            if (Localizer.Default.LocLanguage.Font != null)
            {
                MyLocalizableMessageBox2.Font = Localizer.Default.LocLanguage.Font;
            }

            MyLocalizableMessageBox2.CancelButton = MyLocalizableMessageBox2.buttonRightMost;
            MyLocalizableMessageBox2.AcceptButton = MyLocalizableMessageBox2.buttonMiddle;
            return (MyLocalizableMessageBox2.ShowDialog() == DialogResult.OK) ? MyLocalizableMessageBox2.textBoxInput.Text : null;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(sender is Button);
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            if (button.Text == LabelOk)
            {
                base.DialogResult = DialogResult.OK;
            }
            else if (button.Text == LabelCancel)
            {
                base.DialogResult = DialogResult.Cancel;
            }
            else if (button.Text == LabelYes)
            {
                base.DialogResult = DialogResult.Yes;
            }
            else if (button.Text == LabelNo)
            {
                base.DialogResult = DialogResult.No;
            }
            else if (button.Text == LabelAbort)
            {
                base.DialogResult = DialogResult.Abort;
            }
            else if (button.Text == LabelRetry)
            {
                base.DialogResult = DialogResult.Retry;
            }
            else
            {
                if (!(button.Text == LabelIgnore))
                {
                    return;
                }

                base.DialogResult = DialogResult.Ignore;
            }

            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonRightMost = new System.Windows.Forms.Button();
            buttonMiddle = new System.Windows.Forms.Button();
            buttonLeftMost = new System.Windows.Forms.Button();
            labelMessage = new System.Windows.Forms.Label();
            textBoxInput = new System.Windows.Forms.TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(labelMessage, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonRightMost, 3, 2);
            tableLayoutPanel1.Controls.Add(buttonMiddle, 2, 2);
            tableLayoutPanel1.Controls.Add(buttonLeftMost, 1, 2);
            tableLayoutPanel1.Controls.Add(textBoxInput, 0, 1);
            tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(353, 149);
            tableLayoutPanel1.TabIndex = 0;
            buttonRightMost.Location = new System.Drawing.Point(275, 123);
            buttonRightMost.Name = "buttonRightMost";
            buttonRightMost.Size = new System.Drawing.Size(75, 23);
            buttonRightMost.TabIndex = 1;
            buttonRightMost.UseVisualStyleBackColor = true;
            buttonRightMost.Click += new System.EventHandler(ButtonClick);
            buttonMiddle.Location = new System.Drawing.Point(194, 123);
            buttonMiddle.Name = "buttonMiddle";
            buttonMiddle.Size = new System.Drawing.Size(75, 23);
            buttonMiddle.TabIndex = 2;
            buttonMiddle.UseVisualStyleBackColor = true;
            buttonMiddle.Visible = false;
            buttonMiddle.Click += new System.EventHandler(ButtonClick);
            buttonLeftMost.Location = new System.Drawing.Point(113, 123);
            buttonLeftMost.Name = "buttonLeftMost";
            buttonLeftMost.Size = new System.Drawing.Size(75, 23);
            buttonLeftMost.TabIndex = 3;
            buttonLeftMost.UseVisualStyleBackColor = true;
            buttonLeftMost.Visible = false;
            buttonLeftMost.Click += new System.EventHandler(ButtonClick);
            labelMessage.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(labelMessage, 4);
            labelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            labelMessage.Location = new System.Drawing.Point(3, 0);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new System.Drawing.Size(347, 94);
            labelMessage.TabIndex = 0;
            tableLayoutPanel1.SetColumnSpan(textBoxInput, 4);
            textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxInput.Location = new System.Drawing.Point(3, 97);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new System.Drawing.Size(347, 20);
            textBoxInput.TabIndex = 4;
            textBoxInput.Visible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(377, 173);
            base.Controls.Add(tableLayoutPanel1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MyLocalizableMessageBox";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }
    }
}
