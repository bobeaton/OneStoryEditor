using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class PrintViewer : UserControl
    {
        public PrintViewer()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        private void ButtonSaveHtmlClick(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK) 
                return;

            string strDocumentText = webBrowser.DocumentText;
            File.WriteAllText(saveFileDialog.FileName, strDocumentText, Encoding.UTF8);
        }

        private void ButtonPrintClick(object sender, EventArgs e)
        {
            webBrowser.ShowPrintPreviewDialog();
        }

        private void ButtonCloseClick(object sender, EventArgs e)
        {
            var parent = FindForm();
            if (parent != null) 
                parent.Close();
        }
    }
}
