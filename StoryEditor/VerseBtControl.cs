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
    public partial class VerseBtControl : UserControl
    {
        public VerseBtControl(StoryProject.verseRow aVerse, int nVerseNumber)
        {
            InitializeComponent();

            this.labelReference.Text = String.Format("Verse: {0}", nVerseNumber);
            this.textBoxVernacular.Text = aVerse.GetVernacularRows()[0].Vernacular_text;
            this.textBoxNationalBT.Text = aVerse.GetNationalBTRows()[0].NationalBT_text;
            this.textBoxInternationalBT.Text = aVerse.GetInternationalBTRows()[0].InternationalBT_text;
        }

        public void SetWidth(int nWidth)
        {
            this.tableLayoutPanelVerse.SuspendLayout();
            this.SuspendLayout();

            this.Width = nWidth - this.Margin.Left - SystemInformation.VerticalScrollBarWidth;
            this.tableLayoutPanelVerse.Width = this.Width;
            
            this.tableLayoutPanelVerse.ResumeLayout(false);
            this.tableLayoutPanelVerse.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
