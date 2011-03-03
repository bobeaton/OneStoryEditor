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
    public partial class TopForm : Form
    {
        private bool _bSuspendSaveTimer;

        public TopForm()
        {
            InitializeComponent();
        }

        public TopForm(bool bSuspendSaveTimer)
        {
            InitializeComponent();
            _bSuspendSaveTimer = bSuspendSaveTimer;
        }

        private void TopForm_Load(object sender, EventArgs e)
        {
            Top = 0;
            if (_bSuspendSaveTimer)
                StoryEditor.mySaveTimer.Stop();
        }

        private void TopForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_bSuspendSaveTimer)
                StoryEditor.mySaveTimer.Start();
        }
    }
}
