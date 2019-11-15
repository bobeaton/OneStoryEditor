using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class TaskBarForm : Form
    {
        private TaskBarForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public TaskBarForm(StoryEditor theSe, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            InitializeComponent();
            Localizer.Ctrl(this);
            taskBarControl.Initialize(theSe, theStoryProjectData, theStory);
        }

        public bool CheckForAutoSendReceive
        {
            get
            {
                return this.taskBarControl.CheckForAutoSendReceive;
            }
        }
    }
}
