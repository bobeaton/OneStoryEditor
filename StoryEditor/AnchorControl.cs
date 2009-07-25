using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StoryEditor
{
    public partial class AnchorControl : UserControl
    {
        public string[] ThreeLetterBookNames = 
        { 
            "gen",
            "exo"
        };

        public AnchorControl()
        {
            InitializeComponent();
        }
    }
}
