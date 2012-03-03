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
    public partial class VerseEditorForm : Form
    {
        public VerseEditorForm(Control ctrl)
        {
            InitializeComponent();
            SuspendLayout();
            Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
            ClientSize = ctrl.Size;
            ResumeLayout(false);
        }

        private void VerseEditorFormSizeChanged(object sender, EventArgs e)
        {
            var aVerseCtrl = Controls[0] as VerseBtControl;
            if (aVerseCtrl != null)
            {
                aVerseCtrl.UpdateHeight(ClientSize.Width);
            }
        }
    }
}
