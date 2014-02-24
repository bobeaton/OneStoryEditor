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
    public partial class OsMetaDataForm : Form
    {
        public OsMetaDataForm(ProjectSettings projectSettings)
        {
            InitializeComponent();

            InitializeIfEmpty(textBoxLanguageName, projectSettings.Vernacular.LangName);
        }

        private void InitializeIfEmpty(TextBox textBox, string strToInitializeWith)
        {
            if (String.IsNullOrEmpty(textBox.Text))
                textBox.Text = strToInitializeWith;
        }
    }
}
