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
    public partial class HgRepoForm : TopForm
    {
        public HgRepoForm()
        {
            InitializeComponent();
        }

        public string ProjectName { get; set; }
        
        public string UrlBase
        {
            get
            {
                return textBoxHgRepoUrlBase.Text;
            }
            set
            {
                textBoxHgRepoUrlBase.Text = value;
            }
        }

        public string Url
        {
            get { return textBoxHgRepoUrl.Text; }
        }

        public string Username
        {
            get { return textBoxUsername.Text; }
            set { textBoxUsername.Text = value; }
        }

        protected string Password 
        {
            get { return textBoxPassword.Text; }
            set { textBoxPassword.Text = value; }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateUrlTextBox();
            }
            catch
            {
            }
        }

        protected void UpdateUrlTextBox()
        {
            var uri = new Uri(UrlBase);
            if (!String.IsNullOrEmpty(textBoxUsername.Text))
            {
                textBoxPassword.Enabled = true;
                textBoxHgRepoUrl.Text = String.Format("{0}://{1}:{2}@{3}/{4}",
                    uri.Scheme, Username, Password, uri.Host, ProjectName);
            }
            else
            {
                textBoxPassword.Text = null;
                textBoxPassword.Enabled = false;
                textBoxHgRepoUrl.Text = String.Format("{0}://{1}/{2}",
                    uri.Scheme, uri.Host, ProjectName);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
