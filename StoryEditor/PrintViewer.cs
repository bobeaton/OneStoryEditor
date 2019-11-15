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
using GemBox.Document;

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

        private void ButtonExportWordClick(object sender, EventArgs e)
        {
            if (saveWordFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                ComponentInfo.FreeLimitReached += (senders, e1) => e1.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
                string strDocumentText = webBrowser.DocumentText;

                var htmlLoadOptions = new HtmlLoadOptions();
                using (var htmlStream = new MemoryStream(htmlLoadOptions.Encoding.GetBytes(strDocumentText)))
                {
                    var document = DocumentModel.Load(htmlStream, htmlLoadOptions);
                    // Save output PDF file.
                    document.Save(saveWordFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }
    }
}
