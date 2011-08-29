using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

//! capture popup?

namespace NetLoc
{

    public partial class ScreenCaptureForm : Form
    {
        private Form form;
        ScreenCapture sc = new ScreenCapture();
        private string languageId;

        public ScreenCaptureForm(Form form)
        {
            InitializeComponent();

            if (form.IsMdiChild)
                form = form.MdiParent;

            this.form = form;

            cmdMenus.Enabled = (form.MainMenuStrip != null && form.MainMenuStrip.Items.Count != 0);

            txtFileName.Text = form.Name;
            if (txtFileName.Text.EndsWith("Form"))
                txtFileName.Text = form.Name.Substring(0, form.Name.Length - 4);
        }

        /// <summary>
        /// Capture the current window as a .jpg
        /// Do this for all UI languages defined.
        /// Place these files in My Paratext Projects/Doc/figures/"LanguageId"/"FormName".
        ///  </summary>
        private void cmdCapture_Click(object sender, EventArgs e)
        {
            string fileName = GetFileName(txtFileName.Text);
            txtFileName.Text = fileName;

            DoForAllLanguages(
                delegate()
                {
                    form.Refresh();

                    string path2 = Path.Combine(GetFiguresPath(), languageId);
                    sc.CaptureWindowToFile(
                        form.Handle, 
                        Path.Combine(path2, fileName + ".jpg"), 
                        ImageFormat.Jpeg);
                });
        }

        /// <summary>
        /// Capture pictures of all menus of the current form for all languages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdMenus_Click(object sender, EventArgs e)
        {
            DoForAllLanguages(
               delegate()
               {
                   string path2 = Path.Combine(GetFiguresPath(), languageId);

                   foreach (var menu in form.MainMenuStrip.Items)
                   {
                       ToolStripDropDownItem item = menu as ToolStripDropDownItem;
                       if (item == null)
                           continue;

                       string name = item.Name;
                       name = name.Replace("ToolStripMenuItem", "");

                       string fileName = GetFileName(txtFileName.Text + "." + name + "Menu");

                       item.ShowDropDown();
                       form.Refresh();
                       Application.DoEvents();

                       sc.CaptureWindowToFile(
                           form.Handle, 
                           Path.Combine(path2, fileName + ".jpg"), 
                           ImageFormat.Jpeg);
                   }
               });
        }

        private string GetFileName(string fileName)
        {
            if (chkReplace.Checked)
                return fileName;

            if (!ImageFileExists(fileName))
                return fileName;

            fileName = fileName.TrimEnd(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});

            int ind = 2;
            while (true)
            {
                if (!ImageFileExists(fileName + ind))
                    return fileName + ind;
                ind = ind + 1;
            }
        }

        private bool ImageFileExists(string fileName)
        {
            string path = GetFiguresPath();

            foreach (var language in Localizer.Default.Languages)
            {
                languageId = language.Id;
                string path2 = Path.Combine(GetFiguresPath(), language.Id);
                if (File.Exists(Path.Combine(path2, fileName + ".jpg")))
                    return true;
            }

            return false;
        }

        delegate void MethodInvoker();

        private void DoForAllLanguages(MethodInvoker method)
        {
            string current = Localizer.Default.LanguageId;

            foreach (var language in Localizer.Default.Languages)
            {
                languageId = language.Id;

                string path2 = Path.Combine(GetFiguresPath(), language.Id);
                if (!Directory.Exists(path2))
                    Directory.CreateDirectory(path2);

                Localizer.Default.LanguageId = language.Id;

                method();
            }

            Localizer.Default.LanguageId = current;
        }

        /// <summary>
        /// Get the path to the "figures" folder of the "Doc" project.
        /// </summary>
        /// <returns></returns>
        private string GetFiguresPath()
        {
            if (Localizer.Default.ScreenCapturePath == null)
                Localizer.Default.ScreenCapturePath = "c:\\My Paratext Projects\\Doc";

            if (!Directory.Exists(Localizer.Default.ScreenCapturePath))
                Directory.CreateDirectory(Localizer.Default.ScreenCapturePath);

            string path = Path.Combine(Localizer.Default.ScreenCapturePath, "figures");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}