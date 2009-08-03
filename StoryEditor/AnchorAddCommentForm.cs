using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class AnchorAddCommentForm : Form
    {
        public AnchorAddCommentForm(string strJumpTarget, string strComment)
        {
            InitializeComponent();
            this.labelJumpTarget.Text = strJumpTarget;
            CommentText = strComment;
        }

        public string CommentText
        {
            get { return textBoxAnchorComment.Text; }
            set { textBoxAnchorComment.Text = value; }
        }

        private void buttonSavePlusExNote_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
