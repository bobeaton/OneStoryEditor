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
    public partial class TaskBarForm : Form
    {
        public TaskBarForm(StoryEditor theSE, StoryProjectData theStoryProjectData, 
            StoryData theStory)
        {
            InitializeComponent();
            taskBarControl.Initialize(theSE, theStoryProjectData, theStory);
        }
    }
}
