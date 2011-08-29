using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class AnchorAddCommentForm : Form
    {
        private AnchorAddCommentForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public AnchorAddCommentForm(string strJumpTarget, string strComment)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            this.labelJumpTarget.Text = strJumpTarget;
            CommentText = strComment;
        }

        public string CommentText
        {
            get { return textBoxAnchorComment.Text; }
            set { textBoxAnchorComment.Text = value; }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
