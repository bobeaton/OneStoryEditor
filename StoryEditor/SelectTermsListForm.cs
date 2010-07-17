using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NetLoc;
using OneStoryProjectEditor;

namespace Paratext.BiblicalTerms
{
    public partial class SelectTermsListForm : Form
    {
        class BiblicalTermsFileName
        {
            public string Display;
            public string FilePath;

            public override string ToString()
            {
                return Display;
            }
        }
        public string FileName
        {
            get
            {
                if (biblicalTermsListBox.SelectedItem == null)
                    return null;

                BiblicalTermsFileName fileName = (BiblicalTermsFileName) biblicalTermsListBox.SelectedItem;
                return fileName.FilePath;
            }
        }

        public SelectTermsListForm(string biblicalTermsPath)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            string path1 = Path.Combine(StoryProjectData.GetRunningFolder, "BiblicalTerms");

            AddIfPresent(path1, "BiblicalTerms.xml", biblicalTermsPath);
            AddIfPresent(path1, "AllBiblicalTerms.xml", biblicalTermsPath);
            AddIfPresent(path1, "MyBiblicalTerms.xml", biblicalTermsPath);
        }

        private SelectTermsListForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        private void AddIfPresent(string directory, string fileName, string biblicalTermsPath)
        {
            string path = Path.Combine(directory, fileName);
            if (!File.Exists(path))
                return;

            var item = new BiblicalTermsFileName() { Display = BiblicalTermsList.GetCaptionFromFileName(path), FilePath = path };
            biblicalTermsListBox.Items.Add(item);

            int index = biblicalTermsListBox.Items.Count - 1;
            if (index == 0 || path == biblicalTermsPath)
                biblicalTermsListBox.SelectedIndex = index;
        }

        private void biblicalTermsListBox_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
